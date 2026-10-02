# Q0 R2 배열 검증 및 발급 복구

상태: **Approved** — 2026-10-03 실제 `gpt-6-astra`의 한정 설계·구현 승인. `REQ-PLAT-061/062/068/069/070`, `AC-PLAT-059/060/066/067/068` 및 `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, `AC-M5D7QC3-007/008`을 잇는다. 구현 테라(gpt-6-sol)→독립 루나(gpt-6-luna)→최종 아스트라 순서다. 기존 계약4F9 SHA `4F9C58EA0CFDA8318C7A2D6FB345C5B9484E9B9E07458DE898993DB8CCE32925`는 계획·서명 payload·공개권위의 기본 ContractSha256로 계속 사용한다. 본 보완을 기본 계약값으로 바꿔 끼우지 않는다.

## R1 보존과 원인

Root 보고의 R1 signer Abort 종료2/NewSession1, 큐 Abort 후 AUTHORIZE_REJECTED/종료2 및 소유 프로세스 부재를 실패 종료로 보존한다. 계획/승인/소비/큐결과 네 파일은 발급되지 않았다. 관리 Dispose 성공을 종료코드만으로 주장하지 않는다. 병합 `ef56487bbcf28d8ce44b70c50f964ea19b7d2eb0`, 권위SHA `9A1030C037AFBE5B4C69A83A74B3AFD1D769675F77ED8DF6A2E7875826005CDD`, 검증기SHA `BBF64C3CB8F18B40BAC7AA0B1AB261FE60AFE417851BE20D8E400B15196C1499`와 R1 모든 복제·원장·증거·키 세대는 불변이다.

실제 unsigned 계획 검사 stack은 Test-Q0ExecutionSchemaNode260의 prefix.Count였다. 조건식 파이프의 빈 배열 출력이 null로 대입돼 StrictMode에서 정상 배열 계획을 거절했다. 전체 조건식을 배열로 포착하여 prefix 없음/0/1/복수를 정확 배열로 유지한다. schema 의미·정확 필드·형식 거절을 완화하지 않는다.

## 정확 작업 경로와 변경 목록

새 준비는 `output/q0-trusted-child-prep-r2`, 최종 실행은 `output/q0-trusted-child-run-r2`, 증거는 `output/q0-trusted-child-evidence-r2`, 독립 격리는 `output/q0-trusted-child-luna-r2`다. 실제 R1 최종570 경로를 원시 복사해 준비570을 만들고 복사 전후 길이/SHA·메타/GUID·링크·정확 집합을 확인한다. 원래 Git과 R1 바이트는 변경하지 않는다. 준비에서는 다음 상대경로6개만 배열 결함·필수 R2 위치/권위 경로 전환·그 검증에 필요한 범위에서 수정한다.

- `qa/tools/Test-Q0FixedExecutionAuthority.ps1`
- `artifacts/c4-canonical-execution-plan.schema.json`
- `artifacts/c4-canonical-trusted-child-lease.psm1`
- `artifacts/c4-canonical-final-validation-queue.ps1`
- `artifacts/c4-canonical-trusted-child-selftest.ps1`
- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs`

나머지564는 원시 동일, 네 C# 중 위1 외3/메타/선택 의미는 불변이다. 준비 검증기의 공개4는 이전 공개값을 UNISSUED로 명시 재설정하고 그 차이를 기록한다. 새 키 발급 후 정확 네 자리만 치환하여 역치환이 수용된 R2 준비 지문과 같아야 한다. 무차별 문자열 치환으로 역사 R1 경로나 지문을 현재값으로 위조하지 않는다. Source570/Input565/추적571/계획53/SourcePaths25/역사const16·원문25를 유지한다. CurrentInputRows는 원래565 경로의 현재SHA이며 lease는 포함0이다. CurrentSourceRows lease1은 기존3084 보정 의미를 유지한다.

R2 발급 도구는 기존 Root 도구를 원시 기반으로 별도 `qa/tools/Invoke-Q0TrustedChildIssuerR2.ps1`, `qa/tools/Test-Q0TrustedChildIssuerR2.ps1` 두 파일에 구현한다. 기존 issuer/core/test는 불변이다. 새 두 파일은570 밖 제어 소스이며 원격 게시하지 않는다. 기존 불변 Q0EphemeralIssuerV2.Core.psm1 SHA `0F6937BD52732C49E979C8F8A3F897C81733DD21028BE6EEB194AC0B381D012A`를 직접 제한 호출한다. 실제 파일 입구/native argv·고정경로·타입·준비·수용 지문을 새 정확 경로에 맞춘다. 임의 root/권위/계약 허용 인자는 만들지 않는다. 새 scriptroot는 기존 qa/tools이므로 core 위치를 속이지 않는다.

## 키 전 통합 관문과 증거

원계약 증거 루트의 공통 파일명/아홉 Stem별 짧은8출력은 R2 증거 루트에 그대로 예약한다. 실패 R1 이름을 덮지 않는다. R2 준비 검증에 추가로 허용하는 파일은 `schema-regression.json`, `issuer-control-keyless.json`, `unity-compile-attempt-01.log`, `unity-compile-attempt-01.xml`, `unity-compile-attempt-01.json`이다. 합성 fixture는 R2 준비/루나의 `selftest-repositories` 아래에서만 사용한다. 보고·검수·수용은 기존 handoff에 새 R2 접두사로 기록하며 새 보고 파일을 늘리지 않는다.

필수 검증은 실제 원문 파서→전체53필드 유효 합성 계획의 실제 schema 양성, 실제 launch/입력/선택 검사 연결, 잘못된 타입/필드/값 음성, prefix 없음·0·1·복수 및 min/max/items/unique 경계다. 조건식 배열 대입의 같은 결함을 허용6소스와 schema 검사 경로에서 읽기 점검한다. 새 전체계획 시험은 단순 존재확인이나 schema 모의로 대체하지 않는다. 합성 공개 자료·임시 Git fixture는 격리 범위에서만 만들고 실제 키/SignHash/원격은0이다. 생산 Sign 전처리가 실제 import/parser/schema 및 commit/준비/파일/argv 관문까지 이어짐을 검증하고 이후 암호효과만 시험 소유 모의로 차단한다. 모의 권위를 운영에 삽입하는 CLI는 없다.

변경된 C#1은 최종 바이트로 정상 권한 Unity6000.6.0f1의 기존 지원 메서드를 한 번 검수한다. 나머지3 및 보호 파일/570 지문 변화를 구분하고 실제 종료·원시 로그·관측을 보존한다. 실패 시 자동02 발급은 없다. schema/채널/지원/제어의 원시 후보는 Root 검수 후 지정 파일에 CreateNew 한 번 보존하고 Luna가 최종 파일을 독립 전수 검수한다. 실제 숫자는 Int64 원문 경계로 보존하며 JS Number 재직렬화 금지다. 아직 성공하지 않은 파일을 미리 발급하지 않는다. 실패 자료는 불변 보존하며 임의 v2를 추가하지 않는다.

R2 준비 수용 전에는 새 운영키0이다. CurrentPreparationEvidence 두 역할은 R2 preparation.json/channel-tests.json에 결속하고 preparation이 R2 unity-compile.json을 결속한다. 기존 v2는 역사 근거다. R2 준비570의 수용SHA를 최종 CurrentSourceRows.PreparationSha256에 사용하되 각 행의 R1 전임 지문·정수보정 계보는 source-freeze/current-source-manifest에 별도 기록한다. 최종 공개 치환 뒤569개는 R2 준비와 같아야 한다. 기존 R1의568불변 조건은 R1에서R2준비 전이의6개 허용차이와 구분한다.

## 정확 원격 두 문서와 새 한 세대

R2 가지 `codex/q0-execution-public-authority-r2`, 저장소ID1344230225, 대상main. 변경 집합은 정확 다음 두 새 파일이다.

- `docs/approvals/2026-10-03-q0-execution-public-authority-r2.json`
- `docs/specs/work-contracts/2026-10-03-q0-r2-schema-and-issuance-recovery.md`

이미 게시된 기본4F9 문서는 변경 집합에 다시 넣지 않고 같은 병합commit에서 원래SHA로 인증 조회한다. 공개권위12필드는 유지하고 ContractPath/ContractSha256는 기본4F9, VerifierPath는 동일 상대경로, 새 verifierSHA/공개키/새epoch를 결속한다. 같은 최종commit의 새 보완 계약 원문SHA도 R2 issuer가 인증 GET 대조한 뒤에만 Bind1을 허용한다. 기본계약·보완계약·권위의 일치 중 하나라도 실패하면 폐기한다. 원격 게임/QA/복제 업로드는 금지한다.

로컬 실행 Git은 R2run만 허용한다. Git 실행파일 검색은 Application을 정확 하나 선택하고 배열을 실행경로로 넘기지 않는다. 원시보존 `.git/info/attributes` 정책/추적571·blob/current bytes/HEAD/tree/청결을 실제 확인한다. GitHub 요청은 명시 UTF-8 원시 bytes로 보내며 PowerShell 파이프 기본 인코딩에 의존하지 않는다. exact변경PR/head고정병합/인증GET을 유지한다.

키 전 준비·제어 통합 검수 및 실제 Astra R2 시작 수용 후 새 단명키 한 개→공개4동결→R2 최종clone/Git/원장→정확두문서 병합/인증→Bind1→별도 큐 Ready→unsigned 실제53계획 검수→Sign1/키Close→Luna signed preflight→Astra Authorize 순서다. 발급45분/Ready45분·실패폐기·자동재키금지는 유지한다. 이전 키/계획/nonce/InvocationId를 재사용하지 않는다.

Root 큐 진행자는 앞서 보정된 명시 stdout/stderr redirect·동시 비동기 즉시 중계·상한/유한배수·생성반환핸들/실제전체ticks·정확자식 종료를 적용한다. 상속stdout만으로 Ready를 받는 경로는 사용하지 않는다. stdout1MiB/한줄16KiB/보관32줄, stderr64KiB, 종료후 배수5초와 기존전송30초/외부198030초 상한을 유지한다. 원시 Ready9를 수신하기 전 계획 신원을 추정하지 않는다. 메모리 진행자의 R2 정확경로 변경/최종SHA를 기록한다.

독립 검수와 시작 수용의 새 유일 접두사는 `IssuerControlLunaReviewR2Json: ` 및 `IssuerControlStartAcceptanceR2Json: `이다. 기존 정확10키 구조·기본19DC ContractSha256·한세대 범위를 유지하면서 새 소스2/R2 증거를 결속하고, Boot은 본 계약SHA·R2준비 수용·R2앵커만 인정한다. R1 앵커는 대안이 아니다. 본 계약 자체는 R2 키 개시 승인이 아니며 최종 자료를 읽은 실제 Astra 앵커 발급을 기다린다. 전체 역사 P1/아홉 실행/188·377행/게임 게시 수용은 별도다.
