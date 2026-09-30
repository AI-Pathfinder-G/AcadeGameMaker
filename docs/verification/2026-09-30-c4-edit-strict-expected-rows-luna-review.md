# C4 Edit·Strict 예상 행 독립 정적 검수

검수 대상은 Edit fixture `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs` (SHA-256 `7F44FA6E34403605963A9F79FF9C0DC2CB43E34333FEFE419AD455019503160F`), Edit 부모 시험 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs` (`CC22D0F558AC45A493A318AD744F2CDCC23CAA4719671F11F93B8C3FFFCF20C1`), Strict 시험 `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs` (`1089AFEC1CD19AD231F1EBC7EB0A7E272A17C7B90705F195D916B45AB8860772`)의 내장 예상 행이다. 규범 대조에는 승인 C4 r4 (`7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`), QA r2 (`EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`) 및 관련 세대·사전 C1/C2·미반환 보정 문서를 사용했다.

## 결과

내장 JSON은 Edit 148행(Order 0–147)과 Strict 5행(Order 183–187)이다. 전체 Edit·Play·Strict literal을 읽어 합산 188행, 고유 ID 188개, 고유 순서 188개, 중복 0개를 확인했다. 각 행의 예상 필드와 required facts 간에는 이번 주요 경계 검수에서 모순을 찾지 못했다.

- C1 실제 checkpoint 1–51 행은 발급된 실행 결과와 실제 callback 계수를 구분한다. 각 C1 내부 예외는 디스크 트랜잭션에서 처리되어 C4가 `ManualRepairRequired` 또는 `ReloadRequired` 결과를 반환한다. 행은 `c1Begin=1`, `c2Finalize=0`, prepared/C2 관찰자 0, proof·C2 결과·receipt 부재, fresh 완료 미발행으로 기록한다. 디스크 결과가 `ManualRepairRequired`인 2–32와 `ReloadRequired`인 33–51의 구분도 C1 변환 경로와 일치한다.
- C4 미반환/receipt 행은 14를 `NotInspected`, 15·16·26을 실제 receipt 검증 경로로 구분한다. AC007 기본 C2 결과 거절 행은 receipt 발급 후 composition 거절·terminal 처리로 기록된다. 관측되지 않은 proof·결과를 `Absent`로 과장한 불일치는 보이지 않았다.
- FreshPartial 세 부모 집합은 checkpoint 19–25, 21–25, 그리고 별도 checkpoint 21 행으로 구성된다. `c1Begin=1`, `c2Finalize=0`, `preparedObserver=0`, `c2Observer=0`은 시험 제어 callback에서 직접 세며 `Observed`로 기록한다. 19는 예약 전이므로 `freshReserve=0`, 20–25는 예약 후여서 `1`; `freshComplete=0`은 완료 사건 미발행의 source 증거로 분리한다. 원본 실행은 Busy/C1Returned, 두 guard는 종료, 영구 fault는 기록됨으로 fixture의 실패 처리와 맞는다.
- FreshOwner 여섯 부모의 원본 Busy/Stale 행은 각각 successor 행과 별도 ID로 존재한다. 원본 행은 AcceptFresh 뒤 새 intent 처리 전에 기록되며 `oldTakePreserved`, `oldCursorPreserved`, null receipt 기준 `receiptReferencePreserved`가 모두 Yes이고 guard는 `CompletedFresh/CompletedFresh`, fault는 No다. 원본 행의 fresh 예약·완료는 1/1이고 C2는 0이다. 후속 Completed 행은 실제 후속 result/receipt 및 원본 예약·세대 연결을 기록한다. 별도 successor 슬롯을 사용하므로 이전 take/cursor 보존은 Yes이며 정상 종료 guard는 Closed, fault는 No다.
- Strict 다섯 행은 Order 183–187이며 AC009/AC010의 구조 검사만 다룬다. 실행 권한이나 런타임 결과를 주장하지 않는 구조 범위와 일치한다.

P0/P1 차단 결함은 발견하지 않았다. 본 문서는 JSON·소스 연결의 정적 검수만 기록한다. Unity 실행, 컴파일, 실제 NUnit 행 검증 및 C4 전체 수용은 포함하지 않는다.
