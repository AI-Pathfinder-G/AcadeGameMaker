# C4 Router 핀과 Q0 감사 독립 정적 검수

검토 기준은 승인 계약 `docs/specs/work-contracts/2026-09-30-c4-router-compile-recovery-amendment.md` SHA-256 `E77E006AB8FCF302F1B5EC6994E8791CE7444CC810FB24F44AC5971C0255B9DD` 및 Router 구현 검수 `docs/verification/2026-09-30-c4-router-compile-recovery-implementation-luna-review.md` SHA-256 `EADB8B29A057A112AB8E32FC1D3133B5A7A0947D4D5560C5E3D88BC6D6AF7A68`이다.

## 판정

P0 0건, P1 0건이다. 현재 pin 자원은 Adapter의 기존 binding을 보존하고 Router에만 새 승인·정적 검수·소스 지문을 연결한다. Q0 검사는 이 구분을 정확한 경로와 SHA로 강제한다. 검토는 정적 자료 대조이며 Unity 실행이나 전체 C4 수용이 아니다.

## 지문 및 변경 범위

현재 `docs/evidence/c4-audit-current-source-pins.json` SHA-256은 `818AA68759C0BCBA3FC393C9E5DDF5AAEE23C23E076EBE8697CC19F3BE04F6B0`이고, 이전 자원 `docs/evidence/c4-audit-current-source-pins-precompile.json` SHA-256은 `FCA015B48B9EDE8253D4E80C21521B92342DCA7F5B7C87F0BF70A59CE7C74548`이다. 이전 자원의 두 행을 대조한 결과 Adapter 행은 현재와 완전히 같다. Router 행은 소스 SHA, 승인 문서 경로/SHA, 정적 검수 문서 경로/SHA만 새 값이다. 현재 자원에서 이 다섯 변경값을 이전 값으로 되돌리면 이전 자원의 전체 SHA가 정확히 재현됐다. 스키마, 추적 목록, 행 수, 순서, predecessor 결속은 그대로다.

현재 Adapter 소스 SHA는 `4B157B98511A0978D19B25434F6D00E23C0CC6FB53BA8A14969182865C3BDC0E`이며 이전 r4 승인 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D` 및 기존 정적 검수 `06D5CE50E6B217B62F0AF3D6CFA102C5FDE2F25764A0F53A6D5CCA8C50BE9F9F`를 유지한다. Router 행은 소스 SHA `E51E2A7903A4BB1C38F3F45306B98C2117ACFE1E7DA2127F334A8D16E707BF18`, 위 승인 계약, 위 독립 구현 검수에 각각 결속한다. 두 predecessor 파일의 경로와 SHA도 현재 실제 바이트와 일치했다.

## Q0 및 StrictAudit 결속

`Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs` SHA-256은 `C052483C93A02DB2BFAC8A5F1BD3B1E10674639C19FAD8DC9149A26DF5A0918E`이다. `C4CurrentSuccessorPinSha256` 상수는 현재 pin 자원의 SHA와 일치한다. `AssertC4CurrentSuccessorPins`는 두 경로의 집합·중복, 자원 SHA, 실제 runtime SHA, predecessor 자원 및 원본, 승인 상태, 정적 검수와 소스 SHA 결속을 검사한다. Router 경로에만 새 승인·검수 경로와 정확 SHA를 요구하고, 나머지 Adapter 경로에는 기존 r4 binding을 요구한다. 해당 분기에서 다른 키나 fallback 경로는 발견하지 못했다.

Q0의 나머지 변경이 승인된 세 지점뿐인지 이전 지문과 역대조했다. 현재 Q0를 이전 pin SHA와 Router의 기존 승인·정적 검수 binding으로 되돌렸을 때 이전 SHA `567233B703E56C792FA63947A7CCD81E8802BD991E0432DD702DB8D20C0DD878`가 정확히 재현됐다. 따라서 검토 자료 기준 변경은 승인된 현재 자원 SHA 상수와 Router 전용 두 조건 분기뿐이다.

`Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs`는 SHA-256 `1089AFEC1CD19AD231F1EBC7EB0A7E272A17C7B90705F195D916B45AB8860772`로 기존 pin 경로를 읽는 구조를 유지한다. 변경되지 않았으며 별도 분기나 완화는 확인되지 않았다.

Unity 및 컴파일은 실행하지 않았다. 새 pin/Q0 연결이 실제 테스트에서 통과했다는 주장은 하지 않으며, 이후 원장·계획·실제 실행은 별도 검증 대상이다.
