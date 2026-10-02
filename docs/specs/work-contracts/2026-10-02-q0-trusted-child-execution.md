# Q0 신뢰 자식과 현재 바이트 아홉 선택 실행

상태: **Approved** — 2026-10-02 실제 `gpt-6-astra`가 아래 단계별 기술 관문을 조건으로 승인한다. 구현·격리 지원 확인부터 현재 바이트 계획 발급·아홉 선택·독립 수용까지 이 계약 한 개로 진행한다. 별도 사용자 재승인은 요구하지 않는다. 관문 실패는 해당 단계에서 중단하고 정확 원인·바이트를 보존한다. 소유 `REQ-PLAT-061/062/068/069/070`, `AC-PLAT-059/060/066/067/068`; 기존 `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, `AC-M5D7QC3-007/008`의 감사·선택·행 의미를 유지한다. 새 제품 동작·게임/QA 원격 게시 권한은 없다.

선행 [자식 초안](../../proposals/2026-10-02-q0-trusted-child-execution-contract-draft.md) SHA-256 `E15733998623D0261D3104773D23321049B94CF3C569856305EA4E0BF7B6B1E4`와 [설치 API 근거](../../proposals/2026-10-02-q0-child-channel-api-evidence.md) `9522DADDB7AA88D19E62784A8015CB596746361BF8A70DE14EF6675F93632A73`를 아래 구체 범위로 채택한다. 기존 [미실행 계획 발급 계약](2026-10-02-q0-external-authority-issuance.md) SHA `451998AB32068DF1879E76B0A4CEC2ACB13619CDE7D48F25CB1E0E25A88AADEC`와 현재 발급 작업은 독립적으로 진행하며 이 계약의 미래 Unity 지원 확인으로 막지 않는다. 그 계약·소스·키 세대·`ExecutionAuthorized=false` 계획을 수정하거나 현재 서명을 새 실행에 재사용하지 않는다.

## 실제 기준과 역할

역사 `artifacts/c4-final-validation-queue-plan-v15.json` SHA `43125A3558F6B1F3FDDC595FB47F602883B4861D0EF24690F1FEE27083EC9082`가 정확 선택·순서·도구/행 의미의 기준이다. 그 `artifacts/c4-frozen-source-manifest-v19.json` SHA `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`는193행, `artifacts/c4-frozen-input-paths-v19.json` SHA `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`는1084경로다. 둘은 `HistoricalBaseline`이며 현재 복제·현재 검수의 원장이 아니다. LF534 기준은 `artifacts/c4-publication-lf534-baseline-v2.json` SHA `89BC3D6EFDD2B1344E8E6F4F74E819382F7AB2F9C67EBFE373B3BF62F4D26F9E`다.

아스트라는 정확 현재 포함/제외와 동결·발급·실행 개시·수용을 판단한다. 테라 `gpt-6-sol`은 아래 허용 소스와 원장·구현 시험을 담당한다. 루나 `gpt-6-luna`는 별도 복사본의 실제 독립 검수와 최종 증거 대조를 담당한다. 본 계약 작성의 실제 아스트라 참여는 문서·원장 읽기와 승인뿐이며 키·Git·원격·Unity를 실행하지 않았다.

## 경로와 변경 허용 집합

부재 확인한 `output/q0-trusted-child-r1`은 준비/구현 복제다. 완료·수용된 `output/q0-external-authority-r1`의 정확 공개 소스/입력을 원장으로 열거해 원시 복사한다. 현재 약20개라는 추정값을 고정하지 않고 최종 실제 집합과 SHA를 먼저 기록한다. 기존 복제의 selftest 저장소·키/출력·`.git`은 복사하지 않는다. 실행 복제는 별도 부재 확인한 `output/q0-trusted-child-run-r1`이다. 정규화 절대 경로와 모든 부모의 링크/재분석점 이탈을 거절한다. 원래 저장소와 원문534·LF534·LF505·기존 준비/발급 복제·원래 Git은 불변이다.

준비 복제에서 의미 변경할 수 있는 C#은 다음 네 파일뿐이며 각 메타/GUID는 불변이다.

- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs`
- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs`
- `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs`

의미 변경할 PowerShell은 `artifacts/c4-canonical-capture-inputs.ps1`, `artifacts/c4-canonical-final-selected-run.ps1`, `artifacts/c4-canonical-final-validation-queue.ps1`, `artifacts/c4-canonical-verify-required-rows.ps1`, `artifacts/c4-canonical-verify-run.ps1`, `artifacts/c4-canonical-verify-context.ps1`다. 새 허용 보조는 `artifacts/c4-canonical-trusted-child-lease.psm1`, `artifacts/c4-canonical-trusted-child-selftest.ps1` 두 개다. 기존 저장소 읽기 보조는 바이트 동일하게 사용한다. lease 모듈에 부모 시작/등록/채널·점유를 집중하고 별도 실행자 파일을 계속 늘리지 않는다.

실행 계획용 정확 후속 파일은 `qa/tools/Test-Q0FixedExecutionAuthority.ps1`, `artifacts/c4-canonical-execution-plan.schema.json`이다. 현재 미실행 발급 검증기/스키마는 불변으로 보존하고 이 둘만 실행 권한·정확 현재 원장·자식 결속을 추가한다. 기존 issuer v2 모듈은 승인 SHA를 확인해 읽기 전용 사용한다. 공개키 자리 이외 소스는 운영키 생성 전에 완성·검수한다. 다른 C#/메타/asmdef/friend/제품/API/패키지/설정 변경은 허용하지 않는다. 필요한 기존 파일은 아래 입력 관문을 거쳐 원시 복사만 할 수 있다.

새 출력 루트는 `output/q0-trusted-child-evidence-r1`, 루나 격리 루트는 `output/q0-trusted-child-luna-r1`이다. 파일은 새로 생성하고 덮어쓰지 않는다. 증거 루트의 정확 공통 파일명은 `input-disposition.json`, `preparation.json`, `channel-tests.json`, `unity-compile.json`, `source-freeze.json`, `current-source-manifest.json`, `current-input-manifest.json`, `current-selection-manifest.json`, `current-row-manifest.json`, `repository-before.json`, `repository-after.json`, `authority-public.json`, `remote-authority.json`, `execution-plan.json`, `execution-plan.approval.json`, `issuance.json`, `execution-consumed.json`, `queue-result.json`, `luna-preflight.json`, `luna-final.json`이다. 설치/패키지 진단은 `unity-compile.log`, 한정 컴파일 시험 XML은 `unity-compile.xml`이다. 합성 저장소는 두 준비/루나 루트의 `selftest-repositories` 내부만 허용한다. 단계별 추가 출력은 아래 아홉 실행의 정해진 조합뿐이다. 보고는 `docs/verification/2026-10-02-q0-trusted-child-terra-report.md`, `docs/verification/2026-10-02-q0-trusted-child-luna-review.md`, 수용은 `docs/approvals/2026-10-02-q0-trusted-child-limited-acceptance.md`에 기록한다. 후속 실패를 덮어쓰는 재시도는 하지 않는다.

## 첫 관문 — 현재 게임 입력과 바이트 확정

허용 조사 후보의 유한 집합은 LF534 정확 경로, 역사193/1084 정확 경로 및 위 명시 후속 파일이다. 조사 자체와 원시 복사 준비는 허용하지만 역사1084를 현재 입력으로 일괄 승격하거나 미디어까지 자동 포함하지 않는다. 각 경로의 현재 존재·원시SHA·소유/직접읽기/Unity 의존 근거·복제 포함 또는 외부 역사 참조·제외 이유를 `input-disposition.json`에 작성한다. 후보 밖 새 실제 필수 경로가 발견되면 그 경로만 아스트라에게 올려 정확 범위를 개정하며 임의 폴더 복사를 하지 않는다. 이 미확정 집합 때문에 현재 실행 가능·의존 폐쇄를 주장하지 않는다.

현재 포함 집합은 최소 실제 실행 코드·시험·메타/GUID·조립·프로젝트/패키지 설정과 아홉 선택/188·377행·QA 직접 읽기 파일이 빠짐없이 존재해야 한다. 설치 패키지 캐시는 이식성 증거로 계산하지 않는다. 기존 요청/잠금 버전 차이와 현재 실제 설치 버전을 기록하고 격리 가져오기에서 실제 해결 결과를 대조한다. 설정·lock 또는 보호 tracked 바이트가 바뀌면 성공으로 덮지 않고 중단한다. 패키지 자동 변경을 새 승인으로 삼지 않는다.

`CurrentSourceRows`와 `IncludedPaths`는 실제 복제에서 다시 열거한 정확 집합·원시SHA·역할을 담는다. 역사 v19 원장은 별도 `HistoricalBaseline`으로만 연결하고, 원문25개는 승인 raw-lineage 결과의 실제 보존 바이트를 읽기 전용 외부 입력으로 핀한다. 네 C#은 원본 raw→LF534→준비 의미본→자식 의미본을 각각 기록한다. 새 의미 변경을 줄바꿈 변환으로 부르지 않는다. 새 `StaticReview`는 현재 코드의 루나 검수만 가리킨다. 역사 핀/선택/금지 토큰/본문 보존 단언을 제거하거나 과거 review SHA를 LF 값으로 바꾸어 통과시키지 않는다.

아스트라가 파일별 포함/제외 원장과 보호 차이0을 확인하고 루나가 실제 필요한 입력·메타/자원·원문25·현재 의미 계보를 검수한 뒤 복제를 구성한다. 이 판단은 이 계약 안의 기술 관문이며 새 사용자 질문이나 문서 전용 중간 발급을 필요로 하지 않는다. 남은 전체 Renderer7 P1을 자동 폐쇄하지 않고 현재 시험 범위에 미치는 영향·실행 중단 여부를 사전 원장에 명시한다.

## 둘째 관문 — 부모/자식 실제 채널과 한정 Unity 확인

신뢰 부모는 주 조정자가 인증 연결 도구로 확인한 저장소 `1344230225`의 불변 권위·정확 원시 blob·계약/검증기/현재 세대를 실제 동결값과 대조한 수동 호출이다. 공개 단계 자료의 표준입력 전달은 허용하되 입력 그 자체/`Authenticated=true`/임의 SHA를 인증으로 인정하지 않는다. 키나 세션 내부 객체는 전달하지 않는다. 일반 PlanPath/RepositoryRoot 두 인자 호출은 계속 거절한다.

부모는 로컬 단일 `NamedPipeServerStream`과 유한 대기 자식을 사용한다. 실제 서버 `SafePipeHandle`에서 `GetNamedPipeClientProcessId`를 호출하고 OS PID·UTC 생성 시각·부모 계보를 부모가 실제 시작한 원장과 대조한다. 큐→실행자→QA 실행 도구→Unity 및 각 검증 도구의 중간 프로세스를 빠뜨리지 않는다. 자식이 주장한 PID·경로·채널명은 인증이 아니다. 연결 전에 부모가 실제 핸들/계보를 등록하지 못하면 실패한다. 자식도 연결된 서버 프로세스의 실제 신원과 승인된 부모 수명/계보를 확인한다. 설치 Win32 서버 PID 조회 지원을 구현 시험에서 확인하고, 가짜 서버의 일치 JSON만 신뢰하지 않는다. 같은 계정 악성 코드에 대한 강한 격리는 주장하지 않는다.

교환은 정확 `InvocationId`, 원시 계획SHA, 현재 세대, `Role`, `Stem`, 호출순번, 실제 프로세스 신원에 결속한다. 도전값은 각 요청마다 한 번 발급/소비한다. 같은 Unity 프로세스가 여러 시험에서 재검사하는 것은 허용하되 새 순번·도전값을 쓰고 이전 도전값/잘못된 역할·Stem의 재사용은 거절한다. 각 자식은 계획/분리 서명·공개키/권위 요약을 재검사하고 실제 Git·파일SHA도 직접 검사한다. 부모가 주는 빈 Git 출력은 사용하지 않는다. 환경변수는 계획 경로 하나만 사용한다. 현재 계획에서 결정한 채널 위치도 그 자체로 권위는 아니다.

자식/부모 종료·시간초과·계보 불확정·핸들 조회 실패·가짜 서버·동시 중복 호출·도전값 순서/재사용·다른PID/세대/Stem·권위 없는 직접 호출을 실제 격리 합성으로 검증한다. 소유한 프로세스의 핸들·생성시각을 확인한 뒤 그 자식만 종료할 수 있다. 다른 사용자 Unity를 일괄 종료하지 않는다. 루나도 별도 루트에서 핵심 음성을 실제 재현한다.

준비 복제의 필요한 입력 확정 뒤 Unity6000.6.0f1에서 네 수정 시험이 속한 실제 조립의 컴파일과 저장하지 않는 채널 연결/부모 종료 경계를 한정 실행한다. 기존 네 파일 안의 독립적인 합성 검증 경로만 사용하며 새 C#/asmdef는 만들지 않는다. 합성 부모/계획은 운영 실행으로 승격하지 않는다. 외부 출력 경로와 보호 전후SHA를 기록하고 구문 파서 성공만으로 Unity 지원을 주장하지 않는다. 라이선스/패키지/컴파일 실패는 실제 실패로 남긴다. 지원 검증은 이 계약이 지금 허용하는 준비 단계이며 운영 아홉 선택을 미리 실행하는 승인이 아니다.

## 셋째 관문 — 실행 복제·동결·새 권위

입력과 소스가 완성되고 루나 준비 범위 P0/P1=0을 확인하면 정확 현재 바이트를 실행 복제에 복사하고 전수 해시 대조한다. 로컬 Git 초기화·add/commit은 `output/q0-trusted-child-run-r1` 안에서만 허용한다. 원격 추가·fetch/push와 원래 Git 변경은 금지한다. `.gitignore`는 이 실행 복제에서만 새로 만들 수 있고 Unity 재생성 디렉터리 `/Library/`, `/Temp/`, `/Obj/`, `/Logs/`, `/UserSettings/`만 지정한다. 게임/도구/증거 파일을 넓은 패턴으로 숨기지 않는다. ignored 파일은 tracked 승인 입력이 아니며 해당 캐시 생성/패키지 사용 관측은 별도로 기록한다. 모든 증거/계획/원장은 실행 복제 밖에 둔다.

실제 `status --porcelain=v1 -z --untracked-files=all`, top-level, HEAD/tree, 정확 `ls-files`, 원시SHA·메타/GUID·링크 검사로 청결·누락/추가0을 확인한다. 입력/출력 절대 경로 구분과 순환 금지를 적용한다. 먼저 제품/시험/도구 최종 바이트→현재 소스/입력/선택/행 원장→권위 기록→계획→서명→검수/수용 순서다. 원장 자체나 미래 계획/결과SHA를 자기 핀 집합에 넣지 않는다. 공개키 고정 전 Git/원장은 예비 값일 뿐이다. 새 키의 공개값을 실행용 검증기에 고정한 뒤 최종 복제 바이트와 소스/입력 지문을 재계산하고 로컬 최종 커밋/트리를 확정한다. 그 뒤 권위 문서·실행 계획을 발급하며 계획 때문에 그 트리를 다시 쓰지 않는다. 이 최종 순서를45분 단명 발급 창 안에서 완료할 수 있도록 사전 준비를 끝낸다.

현재 미실행 발급 세대를 재사용하지 않고 **이 실행용 새 단명 운영키 한 개**를 위 issuer v2의 실제 null·CNG RSA3072·내보내기 금지·단조45분·실패/대기만료 폐기·한 번 결속/서명 규칙으로 발급할 수 있다. 키 생성 전에 공개키 자리 외 모든 코드와 실행 스키마가 완성·독립 검수되어야 한다. 실행용 검증기는 새 키 공개값만 고정하고 최종SHA를 확인한다. 기존 검증기 SHA를 바꾸지 않는다. 진행자는 준비 완료된 기존 발급 진행자의 제한 호출을 사용할 수 있지만 그 미실행 스키마·계약·원격 경로를 속여 바꾸지 않는다. 지원하지 않는 단계는 새 lease 모듈의 신뢰 부모 발급 경로에서 정확 구현·검수한다.

원격 문서 허용 집합은 이 계약 `docs/specs/work-contracts/2026-10-02-q0-trusted-child-execution.md`와 새 `docs/approvals/2026-10-02-q0-execution-public-authority-r1.json` 정확 두 파일이다. 가지 `codex/q0-execution-public-authority-r1`, 대상 `main`, 저장소 `AI-Pathfinder-G/AcadeGameMaker` ID `1344230225`에 문서만 게시·정확 변경 요청 원본 병합한다. 키/코드/QA/실행복제는 업로드하지 않는다. 경로 부재·실제 원격 충돌·정확 두 파일 차이를 확인하고 불변 병합 커밋의 실제 권위 blob·계약SHA·공개키·검증기SHA를 인증 조회한다. 공개 권위 스키마는 기존12필드를 유지하되 정확 이 계약/후속검증기/새세대를 결속한다. 미래 계획SHA를 넣지 않는다.

아스트라는 실제 인증값을 현재 세대로 고정하고 그 SHA를 같은 단명 프로세스에 한 번 결속한다. 정확 실행 계획은 `ExecutionAuthorized=true`이며 이 계약·새권위·현재 소스/입력/선택/행·실제 Git·원문25·현재 루나 검수·정확 출력·아홉 실행을 묶는다. 최종 원시 계획SHA/nonce를 검토한 뒤 기존 payload로 한 번 서명하고 키를 즉시 폐기한다. 서명자45분 수명과 장시간 시험을 소유하는 신뢰 부모 수명은 다르다. 실행 부모는 개인키를 보유하지 않는다. 발급 실패 때 두 번째 키를 자동 생성하지 않고 아스트라가 원인·정확 새세대 경로를 이 계약 개정으로 판단한다. 기존 실패/서명 자료는 보존한다.

## 넷째 관문 — 정확 아홉 선택 실행

루나가 새권위·서명·현재 계획과 실제 실행복제 결속을 검수하고 아스트라가 개시하면 `execution-consumed.json`을 복제 밖에 `CreateNew`로 먼저 기록한다. 키는 실행 nonce+InvocationId이며 이미 존재·실패·중단된 실행을 재사용하지 않는다. 같은 키를 한 큐의 아홉 선택에만 사용하고 각 요청 도전값은 별개다. 부모 종료/채널 단절은 실패와 소비 상태를 남긴다. 파일을 삭제해 재개하지 않는다.

아래 정확 순서·선택 원본의 시험 이름/순서/selector를 유지한다. 선택 파일 LF 전이는534의 별도 후보SHA를 사용하되 선택 의미를 변경하지 않는다. 현재 실행 Stem은 역사 결과와 구별하며 아래대로 고정한다.

| 현재 Stem | 플랫폼/건수 | 선택 원본 |
|---|---|---|
| `q0-r1-edit139` | EditMode139 | `artifacts/c4-focused-edit-selection-v6.json` |
| `q0-r1-play32` | PlayMode32 | `artifacts/c4-focused-play-selection-v4.json` |
| `q0-r1-hub5` | EditMode5 | `artifacts/c4-focused-hub-selection-v3.json` |
| `q0-r1-matrix91` | EditMode91 | `artifacts/c3-r11-edit-matrix-selection.json` |
| `q0-r1-remaining149` | EditMode149 | `artifacts/c3-r11-edit-remaining-selection.json` |
| `q0-r1-play15` | PlayMode15 | `artifacts/c3-r11-playmode-focused-selection.json` |
| `q0-r1-edit562` | EditMode562 | `artifacts/c3l-edit-regression-selection.json` |
| `q0-r1-worker51` | EditMode51 | `artifacts/c2-r36-selection-preflight.json` |
| `q0-r1-play610` | PlayMode610 | `artifacts/c3l-play-regression-selection.json` |

각 Stem 출력은 증거 루트의 `<Stem>.xml`, `<Stem>.log`, `<Stem>-before.json`, `<Stem>-after.json`, `<Stem>-native.json`, `<Stem>-qa.json`, `<Stem>-rows.json`, `<Stem>-verification.json` 정확 여덟 파일이다. 원래 QA 도구가 외부 절대 결과 경로를 지원하는지 준비 단계에서 검증하며 임의 기존 도구 변경으로 우회하지 않는다. 검증/큐 잠금·180초 사례 제한·선택별21600초 관찰 창·native/QA/outer 실제0·실패/건너뜀/불확정/누락/추가/중복0을 유지한다. 아홉 실행 합계1654는 고유 시험 수가 아니다.

C4 기대 원본 `artifacts/c4-required-checkpoint-rows-v9.json` SHA `2769FA04AF42317D116C9C28D26EDA7948B58ECB598778EC9FE204538EB07EDE`의188행과 C3 `artifacts/c3-r11-required-decision-expected-rows.json` SHA `D1B9CBDA4AF477B5C10FC14893F0C58B2BAE676A38E15987C87B7D918658C9C6`의377행을 정확 ID·내용·순서/선택 관계로 확인한다. Matrix91+Remaining149의 서로소240 관계와 Play15의 기존 JSON 경계·예상0행·XML 부모 결속도 유지한다. 신뢰 채널 성공으로 행 판정을 건너뛰지 않는다. 각 선택 전후·자식 진입에서 실제 Git/파일 입력과 서명 문맥을 재검증하고 실패 즉시 남은 큐를 중단한다.

## 마지막 관문과 미수용 범위

루나는 실제 채널/컴파일·시작 조건·서명·아홉 원시 XML/로그/종료·188/377행·동일 입력·보호 지문·소비 원장과 실패 이력을 독립 검수한다. 아스트라는 `AC-PLAT-059/060`의 현재 바이트/Q0 결속과 `AC-PLAT-066/067/068`의 실제 권위/서명 부분을 증거가 충족한 만큼만 수용한다. 생성 도구의 `LunaReviewed`, `WholeAccepted`, `PublicationApproved`는 거짓으로 유지하고 독립 문서에서 판정한다. 역사14전이·428공백·전체 Renderer7·다른 플랫폼/플러그인 이식성·게임 코드 최초 원격 게시를 자동 종결하지 않는다. 자료가 닫히지 않은 필수 입력·지원 실패·원격 충돌·새 코드 범위가 있으면 해당 지점만 보고하고 확정된 독립 준비는 계속한다.
