# C4 QA 증거 프로토콜

- 상태: **Draft — 구현·실행·수용 권한 없음**. 2026-09-29, 솔, 실제 `gpt-6-sol`.
- 소유 범위: `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-001..010`, 공동 `AC-M5D7QC3-007/008`의 선택·동결·행·프로세스 관측·최종 검증 형식.
- 동작·허용 경로 소유 문서: [C4 r4 통합 개정 계약](2026-09-29-c4-r4-exact-implementation-amendment.md). 둘 다 아스트라 Approved 전환 전에는 도구 구현·실행하지 않는다. 새 제품 권한을 만들지 않는다.
- 원문 제안 SHA `67271D88CEBBDD4A688F36F7FB766B19DA20FEE060BECD7122A33AC1B9E9E26A`를 보존하며 그 네 미결정을 아래 기술 선택으로 해소한다. 원문은 규범 소유자가 아니다. 실제 도구·fixture·생성 원장·개수·run 결과는 아직 없다.
## 공통 직렬화 규칙

아래 표의 키는 정확하며 추가·누락 키, 잘못된 형식, 중복 JSON 키를 거절한다. 이 문서에서 상위 키를 열거한 증거 객체는 `SchemaVersion` 정수1과 `Trace` 문자열 배열을 가진다. FileBinding/CaseBinding/SourceBinding 및 아래 명시된 중첩 객체는 해당 별도 키만 가진다. 시험 행은 명시된 소문자 schemaVersion 및 requirementIds/acceptanceIds를 사용한다. 경로는 작업루트 상대 `/` 형식, 해시는 대문자 64자리 SHA-256, 시각은 UTC ISO-8601 문자열이다. 실행 경로만 정규화된 절대 경로를 사용한다. 배열 순서는 의미를 가진다. 진단의 불확실성은 `Unknown` 또는 null이며 false·0·성공으로 바꾸지 않는다. 실제 값이 없는 이 초안은 새 개수·생성 해시·실행 사실을 발급하지 않는다. 아래 기존 바이트 SHA는 읽기 재조회 근거이며 새 결과 발급이 아니다.

`FileBinding`은 `{Path:string,Sha256:string}`이다. `CaseBinding`은 `{FullName:string,Order:int}`이며 Order는 0부터 연속이다. `SourceBinding`은 `{Path:string,Sha256:string,Kind:string,ChangeAuthority:string}`이고 Kind는 Runtime/Test/Meta/Audit/Tool/Evidence 중 하나다. 최종 권한 상태는 모든 실행 비교 객체에서 `WholeAccepted:false`이며 이 값은 도구가 true로 변경할 수 없다.

## 선택과 엄격 파서

선택 객체의 키는 `SchemaVersion,Trace,Platform,SelectionKind,ExpectedCount,ExpectedQualifiedNames,Selector,SourceFiles,ParserVersion,ParserEvidence,WholeAccepted`다. Platform은 EditMode/PlayMode, SelectionKind는 FocusedEdit/FocusedPlay/FocusedHub/PredecessorPart, 이름은 중복 없는 문자열 배열, SourceFiles는 FileBinding 배열이다. ParserEvidence는 `{DeclaredAttributeCount:int,ParsedAttributeCount:int,UnsupportedDeclarations:string[],EnumSources:FileBinding[]}`다. 개수는 최종 source를 실제 파싱한 뒤만 발급한다. 선언 개수 불일치와 UnsupportedDeclarations 비어 있지 않음은 실패다.

기존 C3 생성기의 쉼표 Split 및 단순 문자열 정규식을 복제하지 않는다. 최종 Test/TestCase/UnityTest 선언과 정확 namespace/class/method를 읽고 bool/int/string/enum만 지원한다. 문자열·이스케이프·인자 구분을 먼저 해석하고, 선언된 enum의 실제 이름/값 결속을 검증한 뒤 실제 NUnit fullname을 구성한다. 표현식, unknown enum, 지원하지 않는 named argument/attribute/source는 누락시키지 않고 Unsupported로 실패한다. source 파싱과 XML의 실제 fullname은 실행 뒤 정확 비교한다. 허용 문법과 fullname 표기는 아래 확정 문법 절을 따른다. 입증되지 않은 복잡 표기는 지원하지 않고 실행 전에 실패한다.

fullname 구성 뒤 regex escape와 양끝 anchor를 적용하고 마지막에 Windows 명령행 인코딩을 한다. 문자열 quote/backslash, 괄호·쉼표, enum의 표기를 서로 다른 단계에서 처리한다. 전체 Unity 실행 명령행 및 숨김 하위 pwsh 명령행이 32767 UTF-16 한도 이내인지 실행 전 검사한다. 초과하면 실행하지 않는다. 사전 분할은 기존 순서·서로소 합집합·전체 names·행의 ExpectedCase·180초를 보존한 고정 Parts로 계획하고 아스트라/루나가 동결한다. 실행 중 동적 분할은 금지한다.

## 동결 소스와 입력 목록

소스 원장 키는 `SchemaVersion,Trace,Files,Count,PredecessorManifest,AllowedChanges,WholeAccepted`다. Files는 SourceBinding 배열이고 Count는 실제 길이다. PredecessorManifest는 불변 R11 FileBinding이며 AllowedChanges는 `{Path:string,RequirementIds:string[],Reason:string}` 배열이다. 원장 자신의 해시는 내부 Files에 넣지 않는다.

입력 목록 키는 `SchemaVersion,Trace,Paths,Count,PredecessorCapture,ReasonMap,ExcludedOutputPatterns,WholeAccepted`다. Paths는 정렬된 중복 없는 경로 배열이고 Count는 실제 길이다. PredecessorCapture는 R11 입력 포착 FileBinding이다. ReasonMap은 `{Path:string,Disposition:string,Reason:string}` 배열이며 Disposition은 Retained/Added/Removed다. 기존 884에서의 모든 추가·제외를 경로별 설명하고 기존 Assets/Packages/ProjectSettings/qa 포착 범위를 숨겨 축소하지 않는다. 직접 읽는 문서·자원과 새 도구·manifest·selection·checkpoint를 포함한다.

목록 자신, queue plan/result, XML/log, before/after capture, native/QA/verification/row comparison, 실행 후 검수·수용 문서는 입력에서 제외한다. capture 키는 `SchemaVersion,Trace,InputListBinding,CapturedAtUtc,Count,Files`이고 Files는 같은 고정 Paths의 FileBinding 배열이다. 자기/출력/재귀 hashing을 금지한다. 같은 Paths의 before/after 및 실행 간 hash를 대조한다. 884 또는 fixed14를 C4 현재 개수로 하드코딩하지 않는다.

## 실행 계획과 선행 선택 지도

계획 키는 `SchemaVersion,Trace,ProjectPath,UnityPath,SourceManifest,InputPathList,CheckpointRows,Tools,PredecessorSelectionMap,Runs,ProcessLock,FailurePolicy,WholeAccepted`다. 결속 자원은 FileBinding이며 Tools에는 새 7개와 변경하지 않은 Invoke-UnityQa가 모두 들어간다. ProcessLock은 `{Scope:"SingleProject",ProjectPath:string,ProjectPathSha256:string,MutexName:string,Exclusive:true,AbandonedPolicy:"FailStop"}`이고 FailurePolicy는 `"FailStop"`이다. 계획 자신의 SHA는 실행 시작 인자로 별도 전달하고 시작·각 경계에서 검사한다. 입력 목록에 계획을 넣지 않는다.

Runs의 각 객체는 `{Stem:string,NativeWitnessRunStem:string,Platform:string,Selection:FileBinding,Selector:string,ExpectedCount:int,ExpectedQualifiedNames:string[],RequiredRowIds:string[],ObservationSeconds:int,CaseTimeoutSeconds:180,CommandLineLength:int,OutputPaths:object,PartId:string}`다. NativeWitnessRunStem은 Stem과 정확히 같다. OutputPaths의 정확 키는 Xml/Log/Before/After/NativeExit/QaReturn/Verification/RequiredRowComparison이며 계약의 c4 stem 출력 경로만 사용한다. 실행 전에 모든 출력의 부재·승인 stem·명령행 한도·결속 SHA를 검사한다. Runs/Parts는 실행 전 불변이다.

선행 지도 키는 `SchemaVersion,Trace,Selections,WholeAccepted`다. Selections는 `{Id:string,SourceSelection:FileBinding,ExpectedCount:int,ExpectedQualifiedNames:string[],Parts:object[]}` 배열이다. Id는 Edit240/Play15/Edit562/Worker51/Play610이다. Parts는 `{Id:string,OrderedNames:string[],Selection:FileBinding}`다. Edit240의 Matrix91/Remaining149 서로소 순서를 보존하고 기존 전체 names를 변경하지 않는다. 기존 내부 377행 원장과 검증 프로그램은 보존하며 해당 정확 원본을 새 소스 실행에서도 별도로 대조한다. 새 C4 행이 기존 행을 대체하지 않는다.

## 실제 편집기 종료 관찰

기존 `qa/tools/Invoke-UnityQa.ps1` 60~100행의 Windows 인코딩/역파싱, 101~144행의 선언 경로 검사 및 `Test-OwnedEditorSnapshot`, 145~175행의 완전 inventory/후보/creation/attach가 직접 근거다. 실제 함수명은 `ConvertFrom-WindowsArgumentLine`, `Read-UnityQaArguments`, `Assert-DeclaredTestRunArgumentPaths`, `Get-OwnedEditorCandidates`, `Test-CreationTimeMatches`, `Attach-OwnedEditor`다. QA의 218행은 실제 Process.ExitCode를 읽지만 263~264행의 성공 출력은 numeric native 값 없이 QA 결과만 반환한다. QA 반환값을 native 증거로 복제하지 않는다.

새 runner는 숨김 하위 pwsh로 변경하지 않은 QA를 정확 한 번 실행한다. UseShellExecute=false/CreateNoWindow=true와 ArgumentList 또는 Start-Process -WindowStyle Hidden을 사용한다. 이 하위 실행과 병행하여 읽기 전용 inventory를 관찰한다. 정확 Unity binary/project/results/log 네 경로 일치, runTests 1개, assetWorker 0개, 선언 경로 각각 1개, 완전 inventory, 후보 정확 1개를 요구한다. 절대 경로·대소문자 무시 정규화와 quote 파싱은 위 원본 규칙을 보존한다. filter/platform은 계획/QA 인자와 결속하며 소유권 네 경로 검사를 대체하지 않는다.

PID+creation UTC와 native Process object/handle을 종료 전에 보관하고 StartTime을 CIM creation과 마이크로초 정규화하여 정확 대조한다. PID 재사용, fast exit 전에 attach 실패, partial inventory, 후보 없음/복수, 실제 종료값 미관측은 NativeEvidenceUnavailable로 실패다. 추가 Editor 실행·임의 kill·launcher 종료값 대체는 금지한다. 프로세스 핸들은 관찰 종료 finally에서만 해제한다. 관찰 창 만료 때 살아 있는 프로세스를 죽이거나 다음 실행을 시작하지 않는다.

native 객체 키는 `SchemaVersion,Trace,RunStem,PlanSha256,Status,ProcessId,CreationUtc,MatchedPaths,CommandLineSha256,RunTestsCount,AssetWorkerCount,AttachedBeforeExit,ActualExitCode,ObservedExitUtc,FailureReason,WholeAccepted`다. Status는 Observed/NativeEvidenceUnavailable이다. MatchedPaths는 `{UnityPath:string,ProjectPath:string,ResultsPath:string,LogPath:string}`다. 미확증 필드는 null이다. raw commandline·라이선스·환경 비밀은 저장/출력하지 않고 일치 필드와 SHA만 기록한다. Observed는 인증 attach와 실제 종료 관측을 모두 요구하고 ActualExitCode는 실제 정수다.

QA 반환 객체 키는 `SchemaVersion,Trace,RunStem,PlanSha256,QaToolBinding,QaChildProcessId,QaToolReturnedCode,OuterExitCode,AncillaryLauncherStatus,CompletedAtUtc,WholeAccepted`다. AncillaryLauncherStatus는 Unknown/ObservedZero/ObservedNonZero/StillRunning이며 부수 정보다. OuterExitCode는 하위 runner의 실제 종료값으로 큐가 관찰한다. QA/native/outer 세 값은 독립적이며 모두 0이어야 한다. runner 내부에서 아직 종료하지 않은 자기 종료값을 성공으로 추정하지 않는다. 최종 작성자는 큐 하나뿐이며 아래 CreateNew 작성 순서를 따른다.

## 전체 행: fixture 발행과 검증 공유 형식

시험 출력은 TestContext.Out의 한 줄 JSON이며 최상위 키 `id`를 첫 번째로 쓴다. 정확 키는 `id,schemaVersion,requirementIds,acceptanceIds,expectedCase,status,actualEvidenceKind,checkpointFamily,checkpointName,checkpointValue,outcome,phase,applicability,reached,before,after,callCounts,authorityCorrelation,guard,barrier,history,cleanup,failureReason`다. status는 Planned/Reached/Passed/Failed/NotReached, actualEvidenceKind는 PureValidator/Structure/FullBridge/ActualCallback/ActualWorker다. 서로 다른 종류의 증거를 full bridge로 합산하지 않는다.

before/after는 `{snapshotSha256:string|null,facts:Fact[]}`이고 Fact는 `{name:string,value:string,certainty:string,evidenceReference:string|null}`다. certainty는 Observed/SourceEstablished/Unknown이다. 실제 참조를 문자열 주소나 정상 권한으로 발급하지 않는다. authorityCorrelation은 `{confirmed:string,owner:string,adapter:string,router:string,root:string,proof:string,c2Result:string,receipt:string,executionGeneration:long|null,memoryGeneration:long|null,nextEpoch:string,checked:bool}`이며 문자열 상관 값은 SameActual/Absent/Foreign/Unknown만 허용한다. checked=false가 정상 상관으로 처리되지 않는다.

callCounts는 `{c1Begin:int|null,c2Finalize:int|null,freshReserve:int|null,freshComplete:int|null,preparedObserver:int|null,c2Observer:int|null,nativeCallback:int|null}`다. guard는 `{adapter:string,router:string,permanentFaultRecorded:string}`이며 상태 값은 Entered/Closed/CompletedFresh/NotEntered/Unknown, fault 값은 Yes/No/Unknown이다. barrier는 `{state:string,ordinaryWriterBlocked:string,evidenceReference:string|null}`이며 state는 NoBarrier/DurableBarrier/Uncertain/Unknown, 차단은 Yes/No/Unknown이다. history는 `{originalCommitPreserved:string,originalEpochPreserved:string,oldTakePreserved:string,oldCursorPreserved:string,receiptReferencePreserved:string}`이며 Yes/No/Unknown이다. cleanup은 `{required:bool,releaseSignaled:string,workerLeaseReleased:string,workerJoined:string,originalThreadCleanup:string,exception:string|null}`이며 사실 값은 Yes/No/NotApplicable/Unknown이다. cleanup 필수인데 Unknown/No이면 실패다.

예상 행 객체 키는 `Id,Order,RequirementIds,AcceptanceIds,ExpectedCase,EvidenceKind,CheckpointFamily,CheckpointName,CheckpointValue,Outcome,Phase,Applicability,ExpectedReach,RequiredFacts,ExpectedCallCounts`다. Applicability는 Required/IntentionalNotApplicable이며 사유는 RequiredFacts에 포함한다. ExpectedReach는 RequiredReached/RequiredNotReached다. IntentionalNotApplicable은 의도적 미도달 기록을 요구하지만 통과 행 수에 포함하지 않는다. RequiredReached 행의 미관측/미도달은 RequiredMissing이며 허용되지 않는다. RequiredNotReached의 명시적 음성 증거 규칙은 아래 공유 형식 절을 따른다. RequiredNotReached는 예컨대 사전 차단 이후 C2가 호출되지 않았다는 검증 대상으로, 관측 사건과 0 call 증거가 필요하며 checkpoint 자체 도달로 세지 않는다.

원장 키는 `SchemaVersion,Trace,SourceFiles,EnumSources,Rows,Count,WholeAccepted`다. 최종 source의 정확 C4 throw 27개, C1 enum 1..51, C2 enum 1..19를 이름/값/선언 순서로 검증한다. 27+51+19 산술을 NUnit 개수나 실제 full row 개수로 발급하지 않는다. A 정지1/B observer2는 일반 throw와 다른 종류·호출 수·순서로 기록한다. outcome별 실제 도달과 의도적 미적용을 최종 fixture의 계획과 대조한다. 행 분할/내용/순서 변경은 재승인한다.

각 행은 Planned 1개 뒤 Reached→Passed 또는 Reached→Failed, 미도달은 NotReached로 종결한다. 중복/역순/미종결/예상 밖 ID/잘못된 키는 실패다. 출력의 expectedCase를 신뢰하지 않고 부모 XML test-case fullname과 예상 ExpectedCase를 대소문자 구분 정확 비교한다. 부모 result가 Passed가 아니면 내부 Passed는 수용 불가다. skipped/inconclusive·NUnit 실패를 내부 행으로 덮지 않는다. 반사 순수 음성, source 지배 증명, 실제 C1/C2 실행, 실제 canceled, 실제 worker 경합을 각각 명시한다.

## 검증 및 실행 비교 결과

행 비교 키는 `SchemaVersion,Trace,RunStem,ExpectedRowsBinding,XmlBinding,SourceMatched,PlannedCount,ReachedCount,PassedCount,FailedCount,NotReachedCount,IntentionalNotApplicableCount,RequiredMissingIds,UnexpectedIds,InvalidIds,Rows,EvidenceMatched,WholeAccepted`다. Rows는 `{Id:string,ExpectedCase:string,ActualCase:string|null,CaseResult:string|null,Matched:bool,Records:object[],Issues:string[]}`다. 행 증거 원문은 Records에 보존한다.

실행 검증 키는 `SchemaVersion,Trace,RunStem,PlanSha256,SelectionBinding,SourceManifestBinding,InputListBinding,XmlBinding,BeforeBinding,AfterBinding,NativeBinding,QaReturnBinding,RowComparisonBinding,Total,Passed,Failed,Skipped,Inconclusive,MissingNames,ExtraNames,DuplicateNames,ActualQualifiedNames,InputDifferences,FreezeDifferences,NativeExitCode,QaExitCode,OuterExitCode,Verified,WholeAccepted`다. 결과 counters는 정수, 차이 목록은 배열이다. XML counters와 실제 case 수 일치도 필수다. Verified는 full names/count/parent pass/행 대조/source freeze/동일 입력/세 실제 종료값0 모두 만족할 때만 true다. 검증기는 종료 관측 미정값을 0으로 변환하지 않는다.

큐 결과 키는 `SchemaVersion,Trace,PlanBinding,SourceManifestBinding,InputListBinding,Runs,SameInputs,ExecutionComparisonPassed,StoppedAtStem,FailureReason,WholeAccepted`다. Runs는 `{Stem:string,VerificationBinding:FileBinding,OuterExitCode:int|null,Completed:bool}` 배열이다. 선행 실패·중단·살아 있는 편집기·증거 부재 이후 다음 실행은 없다. 최종 비교는 독립 검수와 아스트라 수용을 대신하지 않는다.

## 확정 초기 문법과 기존 실제 fullname 근거

파서는 고정 namespace/class/method와 `[Test]`, `[UnityTest]`, 고정 인자 `[TestCase(...)]`만 지원한다. 한 선언의 인자는 bool literal, Int32 범위의 10진 정수 literal, 큰따옴표로 둘러싼 1~64자 ASCII 문자열 `[A-Za-z0-9_]+`, 또는 source에서 정확히 선언한 단일 enum 멤버만 허용한다. 다중 인자는 위 인자들의 고정 나열이며 쉼표의 문자열 내 출현은 허용하지 않는다. enum 선언은 고유한 정수 값과 이름의 유일 결속을 요구하며 별칭·Flags·numeric cast·연산 표현식은 금지한다. negative Int32 literal은 `-`와 숫자를 하나의 정수 token으로 처리하고 invariant decimal로 표기한다.

지원하는 NUnit fullname은 `namespace.class.method` 뒤 TestCase 인자를 `(...)`로 결속한다. bool은 True/False, 정수는 invariant decimal, 위 제한 문자열은 큰따옴표를 포함한 값, enum은 단일 멤버 이름이다. 다중 인자는 쉼표로 연결하고 임의 공백을 추가하지 않는다. 이름의 XML entity는 XML parser로 한 번 decode하며 source string quoting·NUnit 표기·selector Regex.Escape·Windows 인자 encoding을 구분한다. 정규식 selector는 각 fullname 또는 그 정확 합집합의 양끝을 anchor한다.

실제 source/XML의 직접 근거는 다음과 같다. 기존 결과를 새 시험 실행으로 사용하지 않는다.

| 형식 | 기존 실제 소스 | 기존 실제 XML |
| --- | --- | --- |
| enum | `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetMemoryCutoverFaultMatrixV1Tests.cs`18행 `TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeStaging)` | `artifacts/c3-r11-required-play-r1.xml`3366행 fullname 인자 `(BeforeStaging)` |
| 제한 문자열 | `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs`22행 `TestCase("OwnerBeforeTake")` | `artifacts/c3-r11-edit-remaining-r1.xml`289행 XML decode 후 `("OwnerBeforeTake")` |
| bool | `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs`38행 `TestCase(false)` 선언 | `artifacts/c3-r11-play-focused-r1.xml`50행 fullname `(False)` |
| 다중 int | 기존 Edit Owner122~124행의 고정 `TestCase(0,0)` 등 | `artifacts/c3-r11-edit-matrix-r1.xml`529행 `(0,0)` 및26행 단일 `(0)`을 정확 source 선언과 직접 대조 |

설치 자료 `Library/PackageCache/com.unity.ext.nunit@0198eae3b53e/package.json` SHA `77EA0282B7D03AABCA98ED3EDF69441C52A9AED65E826F05F50965D631E32387`는 package2.1.0 및 NUnit3.5 기반이라고 명시한다. 같은 경로의 `net472/unity-custom/nunit.framework.dll` SHA `A607364879B5F5C337B979E9DE23654459DF45BA1A7324D54AA9DF93540DC7A2`를 조회했으나 formatter 원문은 제공된 로컬 source에서 확인하지 못했다. 따라서 복잡 escaping·truncation·alias/flags 표기 지원을 추정하지 않는다. Library를 수정하거나 DLL을 호출해 새 사례를 생성하지 않았다.

expressions/named argument/TestCaseSource/Values/Range/조합 source/generic/nested fixture/사용자 TestName/복잡 escape/입증되지 않은 문법은 UnsupportedDeclarations에 그대로 기록하고 **실행 전 실패**한다. 파서가 이해하지 못하는 선언을 건너뛰어 expected count를 줄일 수 없다. 신규 source 담당자는 이 고정 문법으로 시험을 작성하며 checkpoint 전달에 enum 또는 고정 int를 쓰는 선택은 actual source와 enum 원장에 결속한다. 최종 XML names의 차이가0이어야 실제 검증된다. 새 counts는 최종 source와 정확 선언 개수 대조 뒤에만 발급한다. 원래240/15/562/51/610 이름은 불변 선행 selection map으로 유지하며 parser 제한 때문에 기존 이름을 삭제하거나 바꾸지 않는다.

## 단일 프로젝트 named mutex

큐만 mutex를 획득·보유·해제한다. runner는 같은 mutex를 다시 획득하지 않으며 승인된 queue child 실행 외에 standalone QA entry로 사용하지 않는다. 새 잠금 파일·Global namespace를 만들지 않는다.

정규화는 Windows `Path.GetFullPath(projectPath)`의 절대 경로에서 `/`를 `\`로 통일하고, drive/UNC root 자체를 보존한 채 불필요한 말단 구분자를 제거한 뒤 invariant 대문자로 만든다. 이 문자열을 UTF-8 BOM 없이 인코딩한 바이트의 SHA256을 대문자64자로 계산한다. 계획의 ProcessLock.ProjectPath와 최상위 ProjectPath는 같은 정규화된 프로젝트를 가리키고 ProjectPathSha256/MutexName을 실행 경계마다 다시 계산·비교한다. 이름은 정확 `Local\AcadeGameMaker-C4-<SHA256>`이다. 실제 경로 비교는 기존 Windows 소유권 경계와 같은 OrdinalIgnoreCase이고, 별도 alias 경로로 병렬 실행을 허용하지 않으며 기존 QA 소유 Editor inventory 검사도 유지한다.

실제 프로젝트가 reparse/alias 때문에 서로 다른 정규화 경로로 같은 checkout을 가리킬 수 있으면 임의 동일성 추정을 하지 않는다. 승인된 선언 프로젝트 경로 하나만 받으며 다른 project path는 실패한다. mutex의 즉시 획득 실패·AbandonedMutexException·이미 살아 있는 owned Editor·불완전 inventory는 fail-stop이다. abandoned 예외가 소유권을 넘기더라도 정상 획득으로 간주하지 않고, finally 필요 해제 후 종료한다. Unity를 새로 기동하거나 기존 Editor를 kill하여 해결하지 않는다. 큐가 모든 계획 실행·최종 관측/검증을 끝낼 때까지 보유하고 finally 자신이 실제 획득한 잠금만 해제·dispose한다.

## partial 반환과 최종 QaReturn의 단일 작성

runner partial의 정확 키는 `SchemaVersion,Trace,RunStem,PlanSha256,QaToolBinding,QaChildProcessId,QaToolReturnedCode,AncillaryLauncherStatus,CompletedAtUtc,WholeAccepted`다. final QaReturn과 달리 OuterExitCode는 없다. SchemaVersion1·WholeAccepted=false이며 나머지는 앞 절의 동일 형식이다. actual QA child 종료 전에는 성공 partial을 발행하지 않는다. 미관측 값을0으로 대체하지 않는다.

1. queue는 계획·출력 부재·same inputs·잠금·owned Editor 조건을 확인하고 숨김 runner process를 정확 한 번 시작한다. runner stdout/stderr를 별도로 포착한다.
2. runner는 숨김 QA child stdout/stderr를 **자신의 구조화 stdout과 분리해** 버퍼에 포착한다. 기존 QA의 WriteHost/진행 출력/성공 JSON을 그대로 runner stdout에 전달하지 않는다. QA child raw 출력은 검증·진단에만 사용하며 승인된 기존 Log 경로 외의 임의 새 파일을 만들지 않는다. 민감한 원문을 재출력하지 않는다. runner stderr는 짧은 진단 채널이고 stdout에는 partial JSON 객체 한 개만 쓴다. 빈/복수/혼합 JSON은 실패다.
3. runner는 실제 QA child Process.ExitCode와 native 별도 witness를 관측하고 자신의 처리 결과에 맞는 process exit로 종료한다. 아직 종료하지 않은 자기 exit 숫자를 partial에 쓰지 않는다. finally에서 핸들과 stream을 정리하며 Unity 자체를 kill하지 않는다.
4. queue는 runner 종료를 기다린 뒤 실제 runner Process.ExitCode를 읽는다. partial의 정확 schema·RunStem/PlanSha/QaTool 결속·actual QA child 코드 및 필수 관측을 확인한다. partial이 없거나 손상됐으면 성공 QaReturn을 만들지 않고 fail-stop queue 진단으로 기록한다. runner가 실패했고 유효 partial이 있으면 actual 비영 outer 값을 그대로 기록한다.
5. queue만 최종 QaReturn을 구성해 계획의 OutputPaths.QaReturn에 `FileMode.CreateNew`로 정확 한 번 쓴다. partial 값과 실제 outer 값을 함께 넣고 writerCompletedAtUtc를 위한 새 키를 추가하지 않는다. CompletedAtUtc는 partial의 실제 QA 처리 종료 시각을 유지한다. 이미 파일이 있으면 실패이며 덮어쓰기/재시도/수정은 금지한다.
6. **이 최종 파일이 닫힌 뒤에만** queue가 c4-verify-required-rows/c4-verify-run을 호출한다. runner는 native/QA 관측과 capture의 생산자이며 final verifier의 호출자가 아니다. r4 계약의 runner 역할 중 최종 verifier 호출은 이 정확 순서로 대체한다. native/QA/outer 세 독립값0과 모든 source/name/row 조건이 맞아야 Verified=true다.

검증 파일·최종 큐 결과도 CreateNew로 한 번 기록하고 실패/미관측은 덮어써 통과로 바꾸지 않는다. runner Process.Start 실패에서는 actual OuterExitCode=null이며 정상 QaReturn을 발급하지 않는다. 최종 queue Runs.Completed는 process 종료·필수 산출물 생성 완료 사실이며 Verified나 수용과 구분한다. WholeAccepted는 어떤 도구도 true로 바꾸지 않는다.

## source 담당자의 공유 행 형식 발행

신규 fixture source 담당자는 같은 최종 source에서 row emitter와 ExpectedCase/행 계획을 정의하고 생성기가 이를 읽어 위 ExpectedRows를 만든다. 도구 담당자가 시험의 outcome/checkpoint/applicability/권한 결속을 추측해 다른 행 목록을 만들지 않는다. 모든 emitter는 앞 절의 exact record keys·중첩 형식을 쓰고 새 키를 임의 추가하지 않는다. row generator·fixture의 schemaVersion1/keys/enum/nullable 규칙이 다르면 source freeze 이전 실패다.

위 ExpectedRows의 `RequiredFacts`는 정확 `{Name:string,ExpectedValue:string,RequiredCertainty:string}` 배열이다. RequiredCertainty는 Observed 또는 SourceEstablished이고 실제 Fact.name/value/certainty와 대응한다. SourceEstablished를 실제 실행 Observed로 바꾸지 않는다. `ExpectedCallCounts`는 callCounts와 같은 키·int|null 형식이며 null은 기대하는 관측 부재이고 wildcard가 아니다. Required full bridge의 실제 C1/C2 및 핵심 권한 검증을 Unknown/null로 채워 pass하지 않는다. 예상 결과에 필요한 call count는 실제 관측하는 값을 명시한다.

CheckpointFamily는 None/C4/C1/C2, CheckpointName은 정확 enum 이름 또는 null, CheckpointValue는 정확 int 또는 null이며 None일 때만 둘 다 null이다. Outcome은 Completed/Busy/ConfirmationStale/ReloadRequired/ManualRepairRequired 또는 null/Unknown, Phase는 PreC1/C1Returned/C2Invoked/Completed 또는 null/Unknown이다. typed 결과 미발급 음성의 null은 관측된 정상 결과 없음이며 값0으로 대신하지 않는다. Unknown을 RequiredFacts의 정상 결과 상관으로 사용하지 않는다. ExpectedRows의 Outcome/Phase nullable 값은 정확 기대값이며 wildcard가 아니다. authorityCorrelation.checked=true도 실제 reference·원본 역사 검증 근거 없이는 발행하지 않는다.

RequiredReached는 Planned→Reached→Passed/Failed의 한 경로만 허용한다. RequiredNotReached는 Planned→NotReached를 종결로 허용하지만, 부모 실제 NUnit Passed·명시된 관측 사실·actual C1/C2/해당 사건 call0·사유가 있어야 Matched=true다. 이는 실제 checkpoint 도달/pass가 아니므로 ReachedCount/PassedCount로 합산하지 않는다. IntentionalNotApplicable도 사유 있는 NotReached를 보존하되 required pass 수에 포함하지 않는다. RequiredReached의 미도달과 assertion 미실시는 RequiredMissing으로 실패한다. 전체 EvidenceMatched는 기대 종류·상관·값·부모 결과까지 모두 맞을 때만 true이며 PassedCount==전체Count를 유일 수용식으로 사용하지 않는다.

source 담당자가 발행한 고정 Id/Order/ExpectedCase/내용/증거 종류를 source manifest 및 row binding으로 결속한다. expectedCase는 XML 부모 case fullname과 정확 비교한다. 한 사례의 내부 모든 행이 passed여도 부모 NUnit 실패/timeout이면 Matched=false·미수용이다. 기존377행 원본과 검사기를 수정하거나 새 row schema로 소급 변환하지 않는다. 새 C4 row와 역사377 비교는 각 원장 버전별 별도 출력으로 남긴다. 최종 신규 names/count/행 수는 소스 기반 생성·parser 대조 뒤 동결하며 현재 숫자를 발급하지 않는다.

## 미결정 1..4의 해소와 실제 미실시

1. fullname은 위 실제 source/XML로 입증된 초기 문법만 지원한다. 복잡 escaping·미지원 선언은 실행 전 실패하며 지원 범위 확대는 정확 기술 승인과 근거를 요구한다.
2. actual outer 값은 queue가 runner 종료 후 읽고 final QaReturn CreateNew1회 작성 뒤 verifier를 호출한다. self exit 예측·혼합 stdout·덮어쓰기는 금지한다.
3. fixture source 담당자가 동일 exact schema의 emitter/expected rows를 발행한다. 실제 count는 최종 source에서 생성하며 임의 산술 개수는 없다.
4. queue 단독 Local named mutex를 위 정규화/SHA 이름으로 고정하고 잠금 실패·abandoned는 fail-stop, 기존 inventory도 유지한다.

이 네 선택은 Draft 규범이다. 실제 소스/도구 구현·신규 schema 출력·기존 primary formatter 원문 검증·Unity 실행·새 수용은 아직 수행하지 않았다. 두 소유 계약의 루나 독립 검수·아스트라 승인 뒤에만 구현할 수 있다. 구현 불일치·근거 없는 fullname·미관측 종료·활성 Editor·행/입력 차이는 기술 차단으로 아스트라에게 보고한다. 제품 범위 확대 외에는 새 사용자 결정을 요구하지 않는다.
기존 직접 관측 XML SHA를 재조회했다. required Play `F69B7F15B4C826F96CF3E68727443F45F6D4D4D4A234F72B3CB0688D26D7830A`, Edit remaining `FF7DFC92C99E636C45E3DC4D2215C38DE6870D58953A02A4F6373F04586179E0`, focused Play `6E8C427027138E1034F820EEE30DBEA9901B5469245579B34AFBDB574D6D18C7`, matrix Edit `E90D83041C678955AE3938818BBFD9A5DAAFBCD9614CA24572C34E9BFC950533`이다. 새 source/run의 수용 SHA가 아니다.

정확 Test/TestCase/UnityTest 선언 개수만 DeclaredAttributeCount/ParsedAttributeCount에 집계한다. 생명주기 SetUp/TearDown/UnitySetUp/UnityTearDown 및 정확 Timeout(180000)은 사례 생성자가 아닌 허용 metadata로 구분한다. 다른 사례 생성 attribute/인자 annotation을 metadata로 숨기지 않는다. 미지원 source·TestCaseSource·Values는 반드시 Unsupported로 실패한다. 기존 선행 시험에 적용된 원래 metadata를 새 선택 parser의 편의로 변경하지 않는다.

QA partial/final schema의 QaChildProcessId와 QaToolReturnedCode는 actual 관측이 없으면 null을 허용하며 그러한 반환은 실패다. OuterExitCode는 최종 QaReturn에 실제 runner 종료 정수만 기록한다. runner 시작 자체가 실패하면 최종 QaReturn을 만들지 않고 queue Runs의 OuterExitCode=null/FailureReason으로 남긴다. 빈·여분 출력·trailing 비JSON 출력·복수 JSON·Unknown 또는 비영 QA/native/outer는 모두 fail-stop이다. 종료값/상관 미관측은0으로 바꾸지 않는다. 부분 실패의 유효 final 객체를 남기는 것이 검증 통과는 아니다.
