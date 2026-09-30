# C4 발급 후 C1 미호출 실패 증거 개정 검토

검토 대상 Draft SHA-256: `5AA8812873040272389EC46CBE761F30A8DCB98569685A056C8DB34AEF4995F3`.

대조 규범은 Approved QA r2 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`, C4 r4 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`, 발급 세대 개정 `FF88BFC0471FB48C2828765A1C6FBD0F3913F0DD123CDEB16BCBADD4F8F2DCB6`, 기존 C2 미호출 개정 `7F1BAA3821580E7382258B7C8BD0F045818FF51B9334AD0C905EB7B9A6388356`이다. 실제 Bridge SHA는 `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`다.

정적 판정: P0 0, P1 0. Draft는 기존 `IssuedWithoutResult`의 실제 CWT 발급·결과 부재·원본 thread·fault/consume 이력 조건을 유지하면서 C1 이전 실패에서만 memory generation과 proof를 null/Unknown으로 표현한다. `checked=false`를 고정해 발급 기록을 전체 payload 검증으로 오인하지 않게 한다. 검증 대상은 FullBridge/실제 callback 또는 worker, 사전 도달 및 동일 예외로 제한되고, SourceEstablished 구조 증명과 실제 control/발급 관측을 분리한다.

Draft의 체크포인트 1~7 이름과 정수는 Bridge의 enum 및 호출 순서와 일치한다. 첫 체크포인트는 consume 전이며 나머지 여섯은 consume 후다. C1 진입 전이므로 C1/C2 및 observer·fresh 호출 0 조건은 고정 소스의 지배 관계와 실제 control 관측으로 구분할 수 있다. proof를 Absent나 실제 권한으로 승격하지 않으며 기존 C2 미호출 규칙을 변경하지 않는다. 따라서 QA r2의 좁은 GenerationRule 확장으로 구현 가능한 설계다.

범위는 Draft에 적힌 일곱 경계에 한정한다. 합성 행·국소 검사·정적 소스 추적은 실제 emitter나 Unity 실행 결과가 아니다. 아직 Draft이며 구현·실행 승인은 아니다.
