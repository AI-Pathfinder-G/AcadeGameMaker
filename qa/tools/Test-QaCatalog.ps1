[CmdletBinding()]
param(
    [switch]$SelfTest
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$script:ErrorTokens = @(
    'INVALID_JSON',
    'DUPLICATE_SCENARIO_ID',
    'MISSING_REQUIRED_FIELD',
    'INVALID_ENUM',
    'UNKNOWN_AC',
    'UNCOVERED_AC',
    'UNAUTHORIZED_BLOCKER'
)

function New-ValidationError {
    param([string]$Token, [string]$Message)
    [pscustomobject]@{ Token = $Token; Message = $Message }
}

function Read-JsonDocument {
    param([string]$Path)
    try {
        return Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json -Depth 100
    }
    catch {
        throw "INVALID_JSON: $Path - $($_.Exception.Message)"
    }
}

function Get-ExpectedAcceptanceCriteria {
    $groups = [ordered]@{
        SCOPE = 4; MOV = 6; WT = 6; COM = 4; ROOM = 6
        RUN = 4; CHOICE = 4; UX = 13; ART = 12; PLAT = 9
    }
    $result = [System.Collections.Generic.List[string]]::new()
    foreach ($entry in $groups.GetEnumerator()) {
        for ($i = 1; $i -le $entry.Value; $i++) {
            $result.Add(('AC-{0}-{1:D3}' -f $entry.Key, $i))
        }
    }
    return $result.ToArray()
}

function Test-RequiredManifest {
    param($Manifest)
    $errors = [System.Collections.Generic.List[object]]::new()
    $expected = @(Get-ExpectedAcceptanceCriteria)
    $allowed = @('schemaVersion','acceptanceCriterionIds')
    if ($null -eq $Manifest.PSObject.Properties['schemaVersion'] -or $null -eq $Manifest.PSObject.Properties['acceptanceCriterionIds']) {
        return ,(New-ValidationError 'MISSING_REQUIRED_FIELD' 'Manifest requires schemaVersion and acceptanceCriterionIds.')
    }
    if ($Manifest.schemaVersion -ne 1) {
        $errors.Add((New-ValidationError 'INVALID_ENUM' 'Manifest.schemaVersion must be 1.'))
    }
    foreach ($name in @($Manifest.PSObject.Properties.Name)) {
        if ($name -notin $allowed) { $errors.Add((New-ValidationError 'INVALID_ENUM' "Unknown manifest field: $name")) }
    }
    if ($Manifest.acceptanceCriterionIds -isnot [System.Array]) {
        $errors.Add((New-ValidationError 'INVALID_ENUM' 'Manifest.acceptanceCriterionIds must be an array.'))
        return $errors.ToArray()
    }
    $actual = @($Manifest.acceptanceCriterionIds)
    if ($actual.Count -ne $expected.Count -or @($actual | Sort-Object -Unique).Count -ne $expected.Count) {
        $errors.Add((New-ValidationError 'UNKNOWN_AC' 'The required-AC manifest must contain exactly 68 unique IDs.'))
        return $errors.ToArray()
    }
    foreach ($id in $actual) {
        if ($id -notin $expected) {
            $errors.Add((New-ValidationError 'UNKNOWN_AC' "Unknown manifest acceptance criterion: $id"))
        }
    }
    foreach ($id in $expected) {
        if ($id -notin $actual) {
            $errors.Add((New-ValidationError 'UNCOVERED_AC' "Manifest omits required acceptance criterion: $id"))
        }
    }
    return $errors.ToArray()
}

function Test-CatalogObject {
    param($Catalog, [string[]]$RequiredAcIds)

    $errors = [System.Collections.Generic.List[object]]::new()
    $requiredScenarioFields = @('scenarioId','domain','caseType','priority','phase','status','title','requirementIds','acceptanceCriterionIds','fixtureRefs','steps','oracle','evidence','blockedBy','tags')
    $domains = @('TITLE','MENU','SETTINGS','PAUSE','INPUT','REBIND','HUD','AIM','MOVE','WEIGHT','COMBAT','ENEMY','BOSS','ROOM','RUN','FAIL','CHOICE','SKILL','PROFILE','RENDER','LIGHT','PLATFORM','BUILD','E2E')
    $caseTypes = @('POS','NEG','BND','LIFE','DET','REC','VIS','READ','BUILD')
    $priorities = @('P0','P1','P2')
    $phases = @('pre_unity','editmode','playmode','windows_build','manual')
    $statuses = @('draft','ready','blocked')
    $oracleKinds = @('exact_state','exact_event_sequence','exact_bytes_or_hash','numeric_tolerance','visual_contract','performance_budget','manual_observation')
    $comparisons = @('exact','within_contract','sequence_match','hash_match','manual')
    $evidenceTypes = @('log','state_dump','replay','screenshot','video','profile_bytes','build_record','performance_capture','manual_report')
    $blockers = @('OD-ART-001','OD-SCENE-001')
    $reqPattern = '^REQ-(SCOPE-(?:00[1-4])|MOV-(?:00[1-9]|010)|WT-00[1-8]|COM-00[1-6]|ROOM-00[1-7]|RUN-00[1-6]|CHOICE-00[1-7]|UX-(?:00[1-9]|01[0-4])|ART-(?:00[1-9]|01[0-5])|PLAT-(?:00[1-9]|01[0-4]))$'
    $acPattern = '^AC-(SCOPE-00[1-4]|MOV-00[1-6]|WT-00[1-6]|COM-00[1-4]|ROOM-00[1-6]|RUN-00[1-4]|CHOICE-00[1-4]|UX-(?:00[1-9]|01[0-3])|ART-(?:00[1-9]|01[0-2])|PLAT-00[1-9])$'

    $catalogFields = @('schemaVersion','scenarios')
    if ($null -eq $Catalog.PSObject.Properties['schemaVersion'] -or $null -eq $Catalog.PSObject.Properties['scenarios']) {
        return ,(New-ValidationError 'MISSING_REQUIRED_FIELD' 'Catalog requires schemaVersion and scenarios.')
    }
    if ($Catalog.schemaVersion -ne 1) { $errors.Add((New-ValidationError 'INVALID_ENUM' 'Catalog.schemaVersion must be 1.')) }
    foreach ($name in @($Catalog.PSObject.Properties.Name)) {
        if ($name -notin $catalogFields) { $errors.Add((New-ValidationError 'INVALID_ENUM' "Unknown catalog field: $name")) }
    }

    if ($null -eq $Catalog.scenarios -or $Catalog.scenarios -isnot [System.Array] -or @($Catalog.scenarios).Count -eq 0) {
        return ,(New-ValidationError 'MISSING_REQUIRED_FIELD' 'Catalog.scenarios is required.')
    }

    $seenIds = @{}
    $covered = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($scenario in @($Catalog.scenarios)) {
        $missing = $false
        foreach ($field in $requiredScenarioFields) {
            if ($null -eq $scenario.PSObject.Properties[$field] -or $null -eq $scenario.$field) {
                $errors.Add((New-ValidationError 'MISSING_REQUIRED_FIELD' "Scenario is missing: $field"))
                $missing = $true
            }
        }
        foreach ($name in @($scenario.PSObject.Properties.Name)) {
            if ($name -notin $requiredScenarioFields) { $errors.Add((New-ValidationError 'INVALID_ENUM' "Unknown scenario field: $name")) }
        }
        if ($missing) { continue }
        $id = [string]$scenario.scenarioId
        if ($id -notmatch '^QA-[A-Z0-9]+-(POS|NEG|BND|LIFE|DET|REC|VIS|READ|BUILD)-[0-9]{3}$') {
            $errors.Add((New-ValidationError 'INVALID_ENUM' "Invalid scenario ID: $id"))
        }
        if ($seenIds.ContainsKey($id)) {
            $errors.Add((New-ValidationError 'DUPLICATE_SCENARIO_ID' "Duplicate scenario ID: $id"))
        } else { $seenIds[$id] = $true }

        if ($scenario.domain -notin $domains -or $scenario.caseType -notin $caseTypes -or $scenario.priority -notin $priorities -or $scenario.phase -notin $phases -or $scenario.status -notin $statuses) {
            $errors.Add((New-ValidationError 'INVALID_ENUM' "Invalid scenario enum in $id"))
        }
        $arrayFields = @('requirementIds','acceptanceCriterionIds','fixtureRefs','steps','evidence','blockedBy','tags')
        $invalidArrayShape = $false
        foreach ($field in $arrayFields) {
            if ($scenario.$field -isnot [System.Array]) {
                $errors.Add((New-ValidationError 'INVALID_ENUM' "$field must be an array in $id"))
                $invalidArrayShape = $true
            }
        }
        if ($invalidArrayShape) { continue }
        foreach ($field in @('requirementIds','acceptanceCriterionIds','steps','evidence')) {
            if (@($scenario.$field).Count -eq 0) { $errors.Add((New-ValidationError 'MISSING_REQUIRED_FIELD' "$field must not be empty in $id")) }
        }
        foreach ($field in @('requirementIds','acceptanceCriterionIds','fixtureRefs','evidence','blockedBy','tags')) {
            $values = @($scenario.$field)
            if (@($values | Sort-Object -Unique).Count -ne $values.Count) {
                $errors.Add((New-ValidationError 'INVALID_ENUM' "$field must contain unique values in $id"))
            }
        }
        foreach ($req in @($scenario.requirementIds)) {
            if ([string]$req -notmatch $reqPattern) {
                $errors.Add((New-ValidationError 'INVALID_ENUM' "Unknown requirement ID in ${id}: $req"))
            }
        }
        foreach ($ac in @($scenario.acceptanceCriterionIds)) {
            $acText = [string]$ac
            if ($acText -notmatch $acPattern -or $acText -notin $RequiredAcIds) {
                $errors.Add((New-ValidationError 'UNKNOWN_AC' "Unknown acceptance criterion in ${id}: $acText"))
            } else { [void]$covered.Add($acText) }
        }

        for ($i = 0; $i -lt @($scenario.steps).Count; $i++) {
            $step = $scenario.steps[$i]
            if ($null -eq $step.PSObject.Properties['order'] -or $null -eq $step.PSObject.Properties['action'] -or $null -eq $step.PSObject.Properties['expected']) {
                $errors.Add((New-ValidationError 'MISSING_REQUIRED_FIELD' "Step in $id requires order/action/expected."))
                continue
            }
            foreach ($name in @($scenario.steps[$i].PSObject.Properties.Name)) {
                if ($name -notin @('order','action','expected')) { $errors.Add((New-ValidationError 'INVALID_ENUM' "Unknown step field in $id`: $name")) }
            }
            if ([int]$step.order -ne ($i + 1) -or [string]::IsNullOrWhiteSpace([string]$step.action) -or [string]::IsNullOrWhiteSpace([string]$step.expected)) {
                $errors.Add((New-ValidationError 'MISSING_REQUIRED_FIELD' "Steps in $id must be ordered 1..N and include action/expected."))
                break
            }
        }
        $oracle = $scenario.oracle
        if ($oracle -is [System.Array] -or $null -eq $oracle.PSObject.Properties['kind'] -or $null -eq $oracle.PSObject.Properties['comparison'] -or $null -eq $oracle.PSObject.Properties['expected']) {
            $errors.Add((New-ValidationError 'MISSING_REQUIRED_FIELD' "Oracle in $id requires kind/comparison/expected."))
            continue
        }
        foreach ($name in @($oracle.PSObject.Properties.Name)) {
            if ($name -notin @('kind','comparison','expected')) { $errors.Add((New-ValidationError 'INVALID_ENUM' "Unknown oracle field in $id`: $name")) }
        }
        if ($oracle.kind -notin $oracleKinds -or $oracle.comparison -notin $comparisons -or [string]::IsNullOrWhiteSpace([string]$oracle.expected)) {
            $errors.Add((New-ValidationError 'INVALID_ENUM' "Invalid oracle in $id"))
        }
        foreach ($item in @($scenario.evidence)) {
            if ($item -notin $evidenceTypes) {
                $errors.Add((New-ValidationError 'INVALID_ENUM' "Invalid evidence declaration in $id"))
            }
        }
        $scenarioBlockers = @($scenario.blockedBy)
        foreach ($blocker in $scenarioBlockers) {
            if ($blocker -notin $blockers) {
                $errors.Add((New-ValidationError 'UNAUTHORIZED_BLOCKER' "Unauthorized blocker in ${id}: $blocker"))
            }
        }
        if (($scenario.status -eq 'blocked' -and $scenarioBlockers.Count -eq 0) -or ($scenario.status -ne 'blocked' -and $scenarioBlockers.Count -gt 0)) {
            $errors.Add((New-ValidationError 'UNAUTHORIZED_BLOCKER' "Status/blocker mismatch in $id"))
        }
    }

    foreach ($ac in $RequiredAcIds) {
        if (-not $covered.Contains($ac)) {
            $errors.Add((New-ValidationError 'UNCOVERED_AC' "No scenario covers: $ac"))
        }
    }
    return $errors.ToArray()
}

function Copy-JsonObject {
    param($Value)
    return $Value | ConvertTo-Json -Depth 100 | ConvertFrom-Json -Depth 100
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$schemaPath = Join-Path $repoRoot 'qa\schema\qa-scenario-catalog.schema.json'
$manifestPath = Join-Path $repoRoot 'qa\coverage\required-ac-ids.json'
$catalogPath = Join-Path $repoRoot 'qa\catalog\vertical-demo.json'

try {
    $null = Read-JsonDocument $schemaPath
    $manifest = Read-JsonDocument $manifestPath
    $catalog = Read-JsonDocument $catalogPath
}
catch {
    Write-Error $_.Exception.Message
    exit 1
}

$manifestErrors = @(Test-RequiredManifest $manifest)
$catalogErrors = @(Test-CatalogObject $catalog @($manifest.acceptanceCriterionIds))
$errors = @($manifestErrors) + @($catalogErrors)

if (-not $SelfTest) {
    if ($errors.Count -gt 0) {
        $errors | ForEach-Object { Write-Error ("{0}: {1}" -f $_.Token, $_.Message) }
        exit 1
    }
    $covered = @($catalog.scenarios.acceptanceCriterionIds | Sort-Object -Unique)
    Write-Output ("PASS scenarios={0} coveredAC={1}" -f @($catalog.scenarios).Count, $covered.Count)
    exit 0
}

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Error ("BASELINE_{0}: {1}" -f $_.Token, $_.Message) }
    exit 1
}

$tests = @(
    @{ Token='INVALID_JSON'; Run={ try { '{' | ConvertFrom-Json | Out-Null; $false } catch { $true } } },
    @{ Token='DUPLICATE_SCENARIO_ID'; Mutate={ param($x) $x.scenarios[1].scenarioId = $x.scenarios[0].scenarioId } },
    @{ Token='MISSING_REQUIRED_FIELD'; Mutate={ param($x) $x.scenarios[0].PSObject.Properties.Remove('title') } },
    @{ Token='INVALID_ENUM'; Mutate={ param($x) $x.scenarios[0].phase = 'unknown' } },
    @{ Token='UNKNOWN_AC'; Mutate={ param($x) $x.scenarios[0].acceptanceCriterionIds[0] = 'AC-FAKE-001' } },
    @{ Token='UNCOVERED_AC'; Mutate={ param($x) foreach ($s in $x.scenarios) { $s.acceptanceCriterionIds = @($s.acceptanceCriterionIds | Where-Object { $_ -ne 'AC-SCOPE-001' }) } } },
    @{ Token='UNAUTHORIZED_BLOCKER'; Mutate={ param($x) $x.scenarios[1].blockedBy = @('OD-ART-001'); $x.scenarios[1].status = 'draft' } }
)

$failed = [System.Collections.Generic.List[string]]::new()
foreach ($test in $tests) {
    if ($test.ContainsKey('Run')) {
        if (-not (& $test.Run)) { $failed.Add($test.Token) }
        continue
    }
    $copy = Copy-JsonObject $catalog
    & $test.Mutate $copy
    $result = @(Test-CatalogObject $copy @($manifest.acceptanceCriterionIds))
    if ($test.Token -notin @($result.Token)) { $failed.Add($test.Token) }
}

if ($failed.Count -gt 0) {
    Write-Error ("SELFTEST_FAILED: {0}" -f ($failed -join ','))
    exit 1
}
Write-Output ("PASS selfTest={0} scenarios={1} coveredAC={2}" -f $tests.Count, @($catalog.scenarios).Count, @($catalog.scenarios.acceptanceCriterionIds | Sort-Object -Unique).Count)
exit 0
