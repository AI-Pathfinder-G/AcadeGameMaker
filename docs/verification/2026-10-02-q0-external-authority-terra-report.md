# Q0 외부 권위 발급의 키 없는 준비 구현 보고

상태: 테라 구현·합성 검증 완료. 독립 루나 검수와 아스트라 시작 판정 전이다. 운영키, 원격 변경, 운영 권위 문서, 운영 계획·서명은 발급하지 않았다. 근거는 승인 계약 `REQ-PLAT-068/069/070`, 문맥 준비 `REQ-PLAT-061/062`와 `AC-PLAT-066/067/068`, `AC-PLAT-059/060`이다. 승인 계약 SHA-256은 `451998AB32068DF1879E76B0A4CEC2ACB13619CDE7D48F25CB1E0E25A88AADEC`다.

기존 `output/q0-cleanclone-signed-prep-r1`의 자체 시험 저장소를 제외한 16개 파일을 `output/q0-external-authority-r1`에 원시 복사했다. 복제는 현재 정확 20개 파일이다. 기존 준비본 16개의 SHA는 승인 준비 원장의 각 준비 SHA와 다시 대조해 불일치 0개였다. 새 복제에서 수정·추가한 파일은 승인된 정확 다섯 경로뿐이다. 네 C# 파일과 네 메타, 다른 준비 후속 도구는 현재 복제에서 준비본과 SHA가 같다. 기존 준비본과 원본 Git에는 쓰지 않았다.

| 새 복제의 허용 소스 | 현재 SHA-256 |
|---|---|
| `qa/tools/Test-Q0FixedPlanAuthority.ps1` | `F8CE251F99980715AFE6E5DF2A0127066E511516428BB1B494EFFA7D3393C7DD` |
| `qa/tools/Invoke-Q0ExternalAuthorityIssuance.ps1` | `91FEA5E0974272F768594AB923BFF77AC80640F1421E94DD0F74BE293C3B448A` |
| `qa/tools/Test-Q0ExternalAuthorityIssuance.ps1` | `2920900166CA0580392B053BE9143E9214A19C492602451F6B031F72B7186510` |
| `artifacts/c4-canonical-verify-context.ps1` | `9D550A579BA1419A261C2E1877828E51F3B6BA7B2A7D30AF3AD4EFA20B40103E` |
| `artifacts/c4-canonical-authority-plan.schema.json` | `F33E0D8FCB1083715D1E8DEB125B482A1D04074A1782B3154D757211B1678B32` |

준비 원장은 `output/q0-external-authority-evidence-r1/preparation.json` SHA `1E96EA32DD1A08AED2A4F737F15864AE200928BBE0689FE953DC9C278E0D9AEA`다. 복사 전 16개와 복사 후 현재 20개 각 경로/SHA, 실제 제외한 `selftest-repositories`, 역사 기준과 현재 범위를 분리했다. 합성 결과는 같은 위치의 `negative-tests.json` SHA `2B74D4B051CA0AE1CE5629D51D00098E683A8E9357BE84B049A5AB61C3D07A24`다. 이후 단계용 공개 증거 파일은 만들지 않았다.

계획 스키마는 역사 v19 입력 1,084경로, 역사 v19 소스·선택·행 원장 SHA, 전임 원시 계보 25행, 예정 시험 9개를 역사/계획 참조로 둔다. `IncludedPaths`와 `SourcePaths`는 현재 복제에 실제 있는 20개 경로이고 `CurrentSourceRows`는 각 현재 원시 SHA·역할·준비본 SHA를 기록한다. `PreparationRows`는 16개 준비본과 네 C#의 원본 raw·LF 전임·준비 의미본 연결을 고정한다. 원본 raw 534, LF 전임 534, LF 입력 505는 보호 원장의 SHA·개수만 별도 참조하며 현재 복제에 포함됐다고 주장하지 않는다. 검증기는 현재 복제 20개 이외의 파일, 링크, 실제 바이트 SHA 불일치와 허용 다섯 파일 외 준비본 변경을 거절한다.

`AC-PLAT-066/068`: 합성 RSA 3072 공개키로 네 지정 대입문만 치환한 임시 검증기에서 서명 양성 1건, 다른 저장소·권위 바이트·계획 바이트·현재 소스 바이트·추가 복제 파일·스키마 바이트·다른 키 세대·신뢰 문맥 부재·일반 검증기/진행자/문맥 명령행 음성 11건을 실행해 12/12 통과했다. 모든 시험은 운영키가 아닌 합성키로 수행했고 임시 시험 복제와 합성키는 시험 종료 시 폐기했다. 다섯 소스의 PowerShell 구문 분석 오류는 0개였다. 운영 CNG 키, 실제 원격, 45분 대기·연결 실패, Unity 실행은 이 결과에 포함하지 않는다.

`AC-PLAT-067`: 수동 진행자는 정확 기존 V2 발급 모듈 SHA를 확인하고 단일 PowerShell 프로세스에서 키를 만든다. 실제 null 이름·CNG RSA3072·내보내기 금지·제공자·공개키 형상은 기존 V2 모듈과 진행자 양쪽에서 확인한다. 단조 45분 시계로 공개 입력 각 줄을 기한 내 받으며 오류·만료·종료의 `finally`에서 세션을 닫는다. 공개 소스 네 대입문만 고정하고 SHA를 기록한다. 병합 뒤 주 조정자가 인증 연결 도구의 공개 결과와 비교한 다섯 공개 줄을 전달받아 원시 권위 SHA와 Git blob SHA-1을 재계산한다. 이 입력 자체는 인증이 아니며, 주 조정자의 실제 연결 도구 조회·대조가 신뢰 경계다. 현재 계획은 먼저 `CreateNew`로 기록해 아스트라가 원시 바이트·SHA·nonce를 읽게 하고, 정확 SHA 확인 후 계획·스키마·20개 소스를 재검사하여 한 번 서명한다. 서명 직후 세션을 닫고 고정 검증기로 공개 서명을 다시 검사한다. 정상 명령행은 종료 코드 2를 반환한다.

주 조정자의 실제 수동 호출은 다음 한 프로세스다. 시작 전 이 보고의 소스와 준비 증거를 독립 검수하고 인증 연결 도구의 읽기·두 문서 병합 경로를 확인한다. `exec_command`로 `pwsh -NoProfile -Command ". 'C:\Users\me\Documents\GPT-workspace\AcadeGameMaker\output\q0-external-authority-r1\qa\tools\Invoke-Q0ExternalAuthorityIssuance.ps1' -LibraryOnly; Invoke-Q0ManualProtocol"`을 대화형 생존 프로세스로 시작하고, 이후 `write_stdin`만으로 공개 줄을 전달한다. `Q0_PUBLIC_READY` 뒤 공개 modulus/exponent·지문·세대와 동결 SHA가 `public-session.json`/`source-freeze.json`에 기록된다. 주 조정자는 계약과 정확 공개 권위 문서 두 파일만 원격 변경 요청으로 병합하고 인증 조회를 수행한다. `Q0_AWAIT_AUTHORITY_PUBLIC_LINES`에는 인증된 불변 커밋(소문자 40자), 권위 blob SHA-1(소문자 40자), 권위 원시 SHA-256(대문자 64자), 권위 원시 바이트의 표준 Base64, 인증 관측 UTC 시각을 이 순서로 각각 한 줄씩 전달한다. `Q0_PLAN_READY` 뒤 `current-plan.json` 원시 바이트와 표시 SHA·nonce를 아스트라가 확인하고, `Q0_AWAIT_EXACT_PLAN_SHA_CONFIRMATION`에 표시된 SHA만 한 줄로 전달한다. 키·세션 식별자·신뢰 객체는 표준입력이나 파일로 전달하지 않는다. 실제 결과는 루나가 독립 검수하고 아스트라가 한정 수용한다.

이 구현은 동일 사용자 계정의 임의 코드 실행을 격리하는 권한 체계를 주장하지 않는다. 메모리 `TrustedRootContext`는 이름만 맞춘 객체로 인증되지 않으며, 주 조정자가 인증 원격 응답을 직접 대조하고 같은 생존 프로세스에 공개값을 전달한 절차 전체가 근거다. 일반 자식 프로세스와 기존 C# 호출은 계속 종료 2이며 현재 계획의 `ExecutionAuthorized`, `WholeAccepted`, `PublicationApproved`는 거짓이다. 실제 운영키 시작, 원격 병합, 운영 서명은 루나 독립 검수와 아스트라 시작 판정 뒤 별도 실제 관측으로 보고한다.
