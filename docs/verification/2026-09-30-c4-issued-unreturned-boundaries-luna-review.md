# C4 발급 후 미반환 경계 개정 독립 설계 검수

검수 대상은 Draft `docs/specs/work-contracts/2026-09-30-c4-issued-unreturned-boundaries-amendment.md` SHA `5E8E7844117AB21AE14C6654105EAABF2897DAA567335ABB666C55C783E3669B`이다. 검수는 정적 소스·규범 대조만 수행했다. 코드, 시험, Unity, 컴파일은 실행하거나 변경하지 않았다.

## 근거 지문

- `ProfileResetExecutionBridgeV1.cs`: `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`
- Edit 부모 시험: `4480A268BBA37E258A43266D796858F5FA94106884A933E85E890180905A6F25`
- Play 부모 시험: `442A94F3C37B6F7AD4E56817AA7367E24E35263B9CB8757BD87B982800A71937`
- Edit fixture: `FBECB66D704C59C12B35A12B43B6ABB1412CAD304DDE44D91DC9FF322BBAAEDC`
- Play fixture: `5829F2BD8176371BA63235F2EF3F176BF44104BEF6F994F94DC2412458D79CA6`
- 검증기 현재판: `DEF2391963186E331EE1EA3C04BDF63E21D8DE3F4E271222CC15C2E865F6065B`

## 판정

P0=0, P1=0. Draft의 일곱 원인/경계 사건은 실제 경로와 부합하며, 기존 C1·C2 미호출 규칙 및 발급 세대 규칙과 범위가 분리되어 있다. 이는 규범 설계 검수이며 구현·실행 수용은 아니다.

일곱 사건은 투영 손상 2건, 정상 반환 Disable 뒤 liveness 거절 1건, 관찰된 proof 손상 재검증 거절 1건, proof 검증 뒤 checkpoint callback 예외 12·13 두 건, C1 Busy 반환 뒤 fresh 게시 checkpoint 17 예외 1건으로 구성된다. CWT `Executions.Add`는 후속 투영/검증과 C4 checkpoint보다 앞서며, 실패는 catch의 원본 fault 처리로 닫힌다. projection 두 건은 consume 전이고 나머지는 consume 후다. 세 `CheckpointFamily=None` 사례도 행 자체는 `ExpectedReach=RequiredReached`이며, `firstC4CheckpointReached=No`는 C4 callback 미도달을 뜻한다는 보정은 QA 행 도달 의미와 맞는다.

12/13은 proof getter·observer 후 disk 검증이 성공한 다음 callback이 던진 사건이다. proof 실제 동일성·검증 성공과 callback 예외 동일성을 기록할 수 있다. 반대로 observer 뒤 `_root`가 null로 손상된 사례는 동일 객체 참조만 `SameActual`로 식별하고, 재검증은 `Rejected`, `checked=false`로 남긴다. 따라서 손상 proof를 정상 권한으로 오인하지 않으며 정상 typed barrier 근거로 재사용하지 못하게 한 문구가 필요하고 충분하다.

checkpoint 14–16 및 26과 AC007 outcome 손상은 C4 결과 미발급이나 실제 C2 receipt가 존재하는 경로다. 유효 receipt의 원본 CWT/session pair 검증을 조건으로 기존 `ReceiptWithoutExecutionResult`로 분류하는 것은 기존 규칙과 일치한다. checkpoint 18 및 27은 이미 C4 결과가 발급된 뒤이므로 새 무결과 규칙에서 제외한다. 26을 결과 발급 후로 취급했던 이전 문장은 현재 초안에서 `BeforeTerminalPublish=26`으로 바로잡혔다.

`IssuedWithoutResult`의 원본 confirmed-key CWT, 원본 issue/thread, 결과와 ResultRecord 부재, fault 및 consume 이력을 유지하고 memory 미발급을 별도 규칙으로 둔 점은 세대 규칙의 역할을 섞지 않는다. proof Unknown은 실제 observer 미도달 사례에만 두고, SameActual은 실제 observer가 받은 원본 참조 사례에만 두는 경계도 맞다. 허용된 행은 기존 receipt/Positive 결과와 구별할 수 있다.

## 구현 전 확인 조건과 한계

현재 verifier는 새 memory 규칙 이름과 변형별 진단을 아직 알지 못한다. 승인 후 구현 시 새 규칙 다섯 이름, 행별 checkpoint/도달·consume·호출 수·proof 조합, 진단 값·certainty·순서, source 포인터를 정확 비교하도록 행 builder와 verifier를 함께 보정해야 한다. Draft는 이 도구 변경을 명시적으로 허용하므로 현 상태를 Draft 자체의 P1로 분류하지 않았다.

특히 checkpoint 12/13의 proof observer 값은 실제 시험이 동일 객체 참조와 검증 성공을 계측해야 하며, `PreparedProofRejectedAfterObserver`는 같은 객체를 보유한 채 손상 후 재검증 실패를 계측해야 한다. 합성 행이나 기존 카운터만으로 이를 대체하면 안 된다. 정적 검수는 일곱 Unity 사례의 실제 성공, receipt 경계의 실제 수용, 전체 C4 수용을 증명하지 않는다.
