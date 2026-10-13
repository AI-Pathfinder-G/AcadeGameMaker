# Q0 외부 권위와 단명 일회 서명 발급

상태: **Approved** — 2026-10-02 실제 `gpt-6-astra` 계약 승인. 소유 `REQ-PLAT-068/069/070`, `AC-PLAT-066/067/068`의 실제 발급 부분 및 `REQ-PLAT-061/062`, `AC-PLAT-059/060`의 문맥 결속 준비다. 사용자의 기존 문서 변경 요청·병합과 후속 구현·통상 기술 판단 위임을 적용한다. 같은 범위의 사용자 승인을 다시 요구하지 않는다. 이 계약의 작성자는 문서 검토만 수행했으며 실제 키·코드·Git·원격 실행을 수행하지 않았다.

채택 근거는 외부 권위 초안 SHA-256 `64C2011B4C1D1C348EBCEDDE4A468DC6D5C59E5126880CC7622DD66F4E807E9D`, 두 단계 발급 모듈 `qa/tools/Q0EphemeralIssuerV2.Core.psm1` SHA-256 `0F6937BD52732C49E979C8F8A3F897C81733DD21028BE6EEB194AC0B381D012A`와 [루나 27/27 검수](../../verification/2026-10-02-q0-issuer-v2-luna-review.md) SHA-256 `2637C33F899ACEF3D3980E23AE891E4B98C765ACCB3B3692863F294B51A87C3F`다. [준비 계약](2026-10-02-q0-cleanclone-signed-preparation.md)의 원본 보호와 역사/현재 바이트 분리는 유지한다.

## 승인 범위와 역할

이 계약은 아래 준비 구현, 시작 조건 충족 뒤 운영키 한 개 생성, 정확 공개 권위 문서 게시·병합·인증 조회, 정확 계획 한 건 서명과 독립 서명 검수를 승인한다. 키 생성과 권위 결속·서명은 주 조정자의 신뢰된 수동 호출이 소유한다. 테라 `gpt-6-sol`은 키 없는 준비 구현과 공개값의 소스 고정을 수행한다. 루나 `gpt-6-luna`는 구현자와 분리해 준비 검수와 실제 발급 결과를 검수한다. 아스트라는 시작 조건·원시 계획·최종 한정 수용을 판정한다. 키 세션 식별자나 모듈 호출 가능성은 그 자체로 아스트라 권위가 아니다.

현재 조정자가 인증된 GitHub 조회로 전달한 기준은 저장소 `AI-Pathfinder-G/AcadeGameMaker`, 저장소 ID `1344230225`, 기본 가지 `main`, 커밋 `e95bb5169ad2ed08106bd342915d4637a1241263`, 트리 `93c4e1fabbaddabd4c237e5964db8dd735965f95`, 쓰기 권한 있음이다. 이는 조정자의 관측이며 본 작성자의 독립 원격 재조회로 표시하지 않는다. 실행 때 다시 인증 조회하여 저장소 ID와 현재 원격을 확인한다. 가지가 이동했으면 정확 문서 경로 충돌과 두 파일 차이를 재검토하고 최신 기준으로 변경 요청을 만들 수 있다. 저장소 ID 변경·인증 실패는 중단한다.

## 정확 허용 경로

기존 준비 구현 완료 뒤 `output/q0-cleanclone-signed-prep-r1`을 전임 원장과 함께 불변으로 보존하고, 부재 확인한 `output/q0-external-authority-r1`에 그 승인된 준비 결과만 원시 바이트로 복사한다. 재분석점·링크 이탈을 거절하고 복사 전후 전체 경로/SHA를 기록한다. 복제 전체는 원격 게시 대상이 아니다. 이 새 복제에서 변경·생성할 수 있는 소스는 다음 다섯 개뿐이다.

- `qa/tools/Test-Q0FixedPlanAuthority.ps1`: 기존 엄격 핵심을 단독 파일에 포함한 고정 공개키 검증기. 원본 핵심의 형식·RSA·변조 거절 규칙을 보존한다.
- `qa/tools/Invoke-Q0ExternalAuthorityIssuance.ps1`: 주 조정자가 수동 제어하는 단명 발급 세션 진행자. 아래 단계별 입력만 받아 실제 issuer 모듈을 호출하고 키를 노출하지 않는다.
- `qa/tools/Test-Q0ExternalAuthorityIssuance.ps1`: 합성 입력 전용 준비 시험. 운영 경로를 합성 모드로 전환하는 매개변수는 없다.
- `artifacts/c4-canonical-verify-context.ps1`: 기존 함수/명령행 호환을 보존하고 아래 신뢰 호출과 엄격 결과 대조를 추가한다.
- `artifacts/c4-canonical-authority-plan.schema.json`: 현재 계획의 정확 스키마. 준비 단계에 완성·독립 검수하며 키 생성 뒤 의미 변경하지 않는다.

발급 모듈은 위 고정 SHA의 원본을 읽기 전용으로 사용하고 실행 직전 재확인한다. 기존 모듈·원본 운영 거절 진입점·네 C# 시험·메타·제품·자산·설정·나머지 QA 도구는 이 계약에서 변경하지 않는다. C# 추가 변경이 필요하면 정확 파일과 인터페이스를 아스트라 개정으로 먼저 승인한다.

공개 출력은 복제 밖 `output/q0-external-authority-evidence-r1`의 정확 파일 `preparation.json`, `source-freeze.json`, `public-session.json`, `remote-anchor.json`, `current-plan.json`, `current-plan.approval.json`, `issuance.json`, `negative-tests.json`, `luna-review.json`만 새로 생성한다. 이미 존재하면 덮어쓰지 않는다. 생성 단계별 빈 파일 예약은 하지 않으며 각 원시 바이트와 SHA를 별도 다음 단계 기록에 결속한다. 보고는 `docs/verification/2026-10-02-q0-external-authority-terra-report.md`, `docs/verification/2026-10-02-q0-external-authority-luna-review.md`, 최종 한정 수용은 `docs/approvals/2026-10-02-q0-external-authority-limited-acceptance.md`로 한정한다. 비밀·키 객체·세션 내부를 직렬화하지 않는다.

원격 변경 요청의 파일 허용 집합은 이 계약 `docs/specs/work-contracts/2026-10-02-q0-external-authority-issuance.md`와 새 공개 권위 기록 `docs/approvals/2026-10-02-q0-public-authority-r1.json` **정확 두 개**다. 권위 JSON은 로컬에도 같은 경로로 생성할 수 있다. 원격 가지는 `codex/q0-public-authority-r1`, 대상은 `main`이다. 기존 경로 존재/내용 충돌이 있으면 덮어쓰기나 다른 경로 추측 없이 아스트라가 정확 개정을 판단한다. QA·게임 소스·키·출력 전체 업로드는 금지한다. 원격 문서 전용 변경은 연결 도구로 수행하고 원래 로컬 Git을 변경하지 않는다. 이번 계약은 위키 수정도 요구하지 않는다.

## 키 생성 전 시작 관문

1. 기존 Q0 준비 구현이 종료되어 정확 파일·인터페이스·보호 지문이 확정되고 루나 독립 검수에서 해당 준비 범위 P0/P1=0을 얻어야 한다. 새 복제의 다섯 소스·계획 스키마와 이 계약의 신뢰 전달·실패 경계를 테라가 구현하고 루나가 합성 검수한다. 실제 키가 없어도 합성 전용 복사본에서 시험할 수 있으나 그 키/소스를 운영으로 승격하지 않는다.
2. 단독 검증기의 공개키 자리와 공개키 지문·키 세대 식별자 외 코드가 완성되어야 한다. 실제 공개값 고정은 지정 자리의 정확 문자열 대입만 허용한다. 대입 뒤 최종 바이트를 다시 루나가 읽기 검수하고 아스트라가 동결한다. 이 대입 외 변경이 생기면 현재 키를 폐기하고 준비 검수로 돌아간다.
3. 정확 계획 스키마·현재 바이트 목록·전임 원문25 참조·네 시험/도구 인터페이스가 완성되어야 한다. 계획은 현재 복제의 정규화 절대 경로, 문맥, 실행 식별자, 권위 참조, 계약/검증기/스키마/소스/입력/선택/행 원장의 SHA, 정확 포함·제외 목록과 원격 기준을 담는다. 자기 계획 SHA·미래 서명·미래 루나 결과를 계획 내부에 넣지 않는다. 이 계약에서 계획의 `ExecutionAuthorized`, `WholeAccepted`, `PublicationApproved`는 거짓이다. 필요한 계획 입력이 미정이면 키 생성 전에 해결한다.
4. 인증된 연결 도구의 읽기·두 문서 변경 요청·정확 변경 요청 원본 병합 경로를 사용할 수 있어야 한다. 조정자는 키 생성 전에 공개 자료만으로 절차와 출력 경로를 확인한다. 비대화형 자동 재시도·키 발급 자동화·예약 실행은 허용하지 않는다. 이러한 관문은 추가 인간 승인이 아닌 아스트라의 기술적 시작 판정이다.

## 신뢰 호출 인터페이스

일반 명령행의 `PlanPath`, `RepositoryRoot` 두 값과 환경변수의 계획 경로는 문맥 신호일 뿐 신뢰 입력이 아니다. 공개키·권위 SHA·커밋·검증기 SHA·빈 Git 출력·`Approved=true`를 호출자나 환경에서 받아 권위로 사용하지 않는다.

단독 검증기는 `-LibraryOnly`로 함수를 정의할 수 있고, 신뢰된 주 조정자만 인증 조회와 원시 소스 SHA 대조를 완료한 같은 수동 호출에서 `Confirm-Q0FixedPlanAuthority(PlanPath, RepositoryRoot, TrustedRootContext)`를 호출한다. `TrustedRootContext`는 파일/환경/명령행에서 역직렬화하지 않는 그 호출의 메모리 객체다. 필수 필드는 `RepositoryId`, `AuthorityCommit`, `AuthorityPath`, `AuthorityRawSha256`, `AuthorityRawBytes`, `ContractSha256`, `VerifierSha256`, `KeyEpoch`, `PublicKeyFingerprint`, `InvocationId`, `AuthenticatedAtUtc`다. 그 이름의 객체를 임의 호출자가 만드는 것만으로 인증되지 않는다. 주 조정자가 인증 도구 응답으로 채우고 그 실행의 검증 소스·helper·정확 모듈 지문을 먼저 비교한 수동 절차 전체가 신뢰 경계다. 임의 로컬 사용자나 동일 계정의 코드 실행자를 막는 보안 격리를 주장하지 않는다.

문맥 보조의 `Assert-C4CanonicalSignedContext(ExactPlanPath, ExactRepositoryRoot, TrustedRootContext)`는 이 같은 신뢰 호출 안에서만 단독 검증기를 호출한다. 기존 두 인자 호출과 일반 명령행은 신뢰 문맥 부재로 `Verified=false`, 종료2를 유지한다. 따라서 C#의 기존 별도 프로세스 호출을 이번 계약만으로 양성으로 바꾸지 않는다. 향후 Unity 실행 관문에서 주 조정자의 신뢰 확인을 자식 프로세스에 결속하는 별도 실행 계약이 필요하다. 이 구분은 실제 계획 서명과 그 서명의 수동 독립 검수를 막지 않는다.

정상 결과는 엄격 단일 JSON의 `SchemaVersion`, `Context`, `Verified`, `PlanSha256`, `ContractSha256`, `AuthorityRawSha256`, `AuthorityCommit`, `VerifierSha256`, `PublicKeyFingerprint`, `KeyEpoch`, `RepositoryRoot`, `InvocationId`만 반환한다. 중복·미지·누락 필드, 다른 계획/루트/문맥·신뢰 호출 식별자·키 세대는 거절한다. 검증 함수는 서명을 확인하고 원시 계획 바이트를 결속할 뿐 Git 청결·의존 폐쇄·Unity 실행 성공을 반환하지 않는다. 외부 인증 전 파일이나 계획에서 취득한 권위 정보를 신뢰 문맥으로 되먹이지 않는다.

## 실제 한 번 발급 순서

1. 주 조정자가 실제 PowerShell 단일 프로세스·단일 모듈 세션에서 `New-Q0EphemeralIssuerV2Session`을 이 계약의 원시 SHA, `CurrentPublicationCleanClone`, TTL 45분으로 호출한다. 실사용 키는 이번 계약에서 정확 한 개다. `NullString.Value`로 실제 null 이름, `Microsoft Software Key Storage Provider`, RSA3072, `IsEphemeral=true`, `KeyName=null`, `ExportPolicy=None`, 공개 modulus384바이트/exponent `010001`을 확인한다. 공개 값만 출력한다. 일반 `$null`이나 빈 이름, 다른 제공자, 내보내기 허용으로 대체하지 않는다.
2. 진행자는 단조 시계로 시작부터 45분을 넘기지 않으며 각 명령 전 검사한다. 기존 issuer의 접근 시 만료 검사만으로 방치된 키가 즉시 폐기된다고 주장하지 않는다. 입력 대기에도 제한 시간이 있는 루프를 사용하고 기한에 세션을 닫은 뒤 프로세스를 종료한다. 모든 종료·오류 경로의 `finally`에서 `Close-Q0EphemeralIssuerV2Session`을 호출한다. 실제 서명 직후에도 즉시 폐기한다. 개인키를 표준출력·파일·환경·명령행·채팅·원격·다른 작업자로 넘기거나 복구/재개하지 않는다. 실제 TTL·폐기는 독립 증거로 남긴다.
3. 공개 키 세대 `KeyEpoch`는 이번 세션의 고유 식별자다. 검증기 지정 자리의 공개 modulus/exponent·지문·세대를 고정하고 최종 소스 SHA를 동결한다. 최종 helper와 스키마 등 모든 의존 소스도 동결한다. 코드에는 미래 권위 raw SHA·커밋·계획·서명·검수 지문을 넣지 않는다.
4. 공개 권위 기록의 필드는 정확 `SchemaVersion`, `RepositoryId`, `ContractPath`, `ContractSha256`, `Context`, `VerifierPath`, `VerifierSha256`, `PublicModulusBase64`, `PublicExponentBase64`, `PublicKeyFingerprint`, `KeyEpoch`, `IssuedAtUtc`로 한정한다. 스키마 값은 `AGM-Q0-PUBLIC-AUTHORITY-V1`이다. 자기 SHA·자기 커밋·미래 계획/검수/서명을 포함하지 않는다. 두 문서만 게시하고 변경 요청의 실제 파일 집합·내용·원본 커밋을 확인한 뒤 그 정확 원본을 병합한다. 게임/QA 코드 게시 권한으로 확대하지 않는다.
5. 병합 뒤 인증된 GitHub 연결 도구로 저장소 ID, 병합 불변 커밋·트리와 정확 권위 경로의 blob을 직접 읽는다. blob 원시 바이트·Git blob 식별자·SHA-256을 계산하고 계약·공개키·최종 검증기 SHA/세대를 실제 동결값과 대조한다. 미병합 가지·로컬 사본·사용자가 공급한 SHA 문자열로 대체하지 않는다. 조회한 커밋이 실제 병합 결과인지 확인하고 계약 파일 원시 SHA도 일치시킨다. 원격 저장소의 전체 최신성을 이 문서 두 개의 검사로 주장하지 않는다.
6. 현재 키 세대는 이 신뢰된 수동 세션에서 확인한 한 개뿐이다. 이전 세대·다른 공개키·옛 권위 커밋을 가져온 호출을 거절한다. 주 조정자만 실제 권위 원시 SHA를 `Bind-Q0EphemeralIssuerV2Authority`에 한 번 결속한다. 같은 SHA 재결속도 허용하지 않는다. 외부 확인 전 가짜 SHA를 먼저 넣지 않는다.
7. 고정된 현재 자료로 정확 `current-plan.json`을 만들고 원시 SHA와 대문자 32자리 nonce를 아스트라가 확인한다. 계획의 권위 참조는 인증된 불변 커밋·정확 경로·원시 SHA다. 동일 생존 세션에서 `Sign-Q0EphemeralIssuerV2Payload`에 계약 SHA·권위 SHA·문맥·계획 SHA·nonce를 넣어 정확 한 번 서명한다. 기존 `AGM-Q0-PLAN-V1` ASCII/LF 정규 payload와 RSA3072/SHA-256/PKCS1을 유지한다. 분리 승인 JSON은 기존 엄격 핵심의 정확10필드 스키마를 사용한다. 경로/시각 같은 서명 payload 밖 필드는 서명된 계획에도 결속하고 실제 값과 대조한다.
8. 주 조정자는 인증 문맥으로 서명을 수동 검증하고 공개 증거를 저장한다. 루나는 키 없이 인증 조회 증거·불변 원격 기록·최종 검증 소스·계획 원시 바이트·서명·실제 관측을 독립 대조한다. 같은 공개 서명의 반복 검증은 허용하지만 새 계획 발급이나 실행 nonce 재사용 권한을 만들지 않는다. 이 계획은 아직 실행 미승인이므로 실행 소비 원장은 후속 실행 계약에서 별도로 지정해야 한다.

실패·연결 단절·만료·재시작·서명 오류·키 속성 불일치면 즉시 폐기하고 해당 세대를 사용 가능한 권위로 취급하지 않는다. 병합된 공개 문서는 실패 이력으로 보존하고 실패 보고에 해당 커밋/세대를 기록한다. 기존 공개 문서를 덮어써 성공으로 바꾸지 않는다. 이번 계약은 두 번째 운영 키 자동 발급을 허용하지 않으며, 실패 원인에 따른 다음 정확 경로·세대·변경 범위를 아스트라가 별도 개정한다. 통상 기술 재개 결정에 같은 사용자 승인을 다시 요구하지 않는다.

## 검증과 수용 경계

`AC-PLAT-066`: 실제 인증된 저장소·불변 권위/계약·고정 검증 소스·현재 세대·정확 계획/서명을 대조하고, 호출자가 맞춰 만든 다른 키/권위/소스 묶음·다른 저장소·옛 세대·미병합 기록·권위 바이트/계획 한 바이트 변조·일반 CLI를 거절한다. 합성 음성에서는 실제 운영키로 추가 서명하지 않는다.

`AC-PLAT-067`: 키 생성→공개 고정→소스 동결→두 문서 병합/인증→권위 일회 결속→계획/nonce→서명 한 번→폐기→루나 검수의 실제 시간·지문을 남긴다. 입력 대기 만료·연결 실패·미결속/재결속·서명 재호출·폐기 뒤 접근을 준비 합성 시험으로 검증하고 실제 운영 결과에는 관측한 사실만 기록한다. 실제 발생하지 않은 만료/실패를 운영 재현으로 쓰지 않는다.

`AC-PLAT-068`: 엄격 형식·변조 거절과 운영/합성 경계, 신뢰 문맥 부재 거절, 비밀 미출력, 보호 원본·후보·Git 불변을 검수한다. 테라 시험·루나 독립 검수·아스트라 한정 수용은 분리한다. 시작 전과 종료 후 보호 원문534·LF534·LF505·기존 서명/발급 소스 및 증거 지문을 비교한다.

전체9선택 Unity·실제 깨끗한 복제 Git 만들기/수정·게임 코드 게시·전체 회귀·의존 폐쇄는 이 계약의 실행 허용이 아니다. 계획 발급·서명 검수는 허용하되 실행 승인으로 계산하지 않는다. `AC-PLAT-059/060` 전체 충족과 역사14전이·Renderer GUID7 P1 종결은 별도다. 실제 외부 권위 부분도 실행 전에는 여전히 미발급이며, 루나 독립 검수와 아스트라 한정 수용 전 P1을 종결하지 않는다.
