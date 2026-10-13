# C4 Edit checkpoint 및 사전진입 증거 독립 사전검수

## 범위와 지문

읽기 전용으로 현재 Edit 시험 본문, fixture, 고정 Bridge 및 C1 구현, 발급 세대 규약을 대조했다. 시험·fixture는 작성 중인 스냅샷이며 Unity 실행이나 최종 수용을 뜻하지 않는다.

| 대상 | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs` | `00A8860AC38BAAA729697A2E3AA36F1168177D551E004EDC8DE0C1E72B3EFE99` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs` | `EFB729E6C8D602CC4ADADE3A8F82DD316FDDC2C4CF4D32A992854C0E74F37BC9` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs` | `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs` | `55B8C19F88B07C5888128C86BF05A743D747AB206E6D44942110ABE0AAFDD38B` |
| `docs/specs/work-contracts/2026-09-30-c4-issued-generation-evidence-amendment.md` | `FF88BFC0471FB48C2828765A1C6FBD0F3913F0DD123CDEB16BCBADD4F8F2DCB6` |

## 판정

이 스냅샷에서 P0/P1 차단 항목은 찾지 못했다. 이는 정적 사전검수 결과이며 ExpectedRows 원장 완성, 컴파일, Unity 실행 또는 실제 행 통과를 확인한 것이 아니다.

- 시험은 14–16과 26을 C2 영수증은 발급됐지만 C4 결과/ResultRecord가 없는 경우로 분리한다. fixture는 원본 `Executions` CWT 레코드와 Adapter/Router 원본 쌍, 세션·후보·영수증의 실제 CWT witness, 동일 generation/root/document를 확인한다. 14에서는 observer가 아직 호출되지 않은 실제 영수증을 Adapter에서 읽고, 15·16·26에서는 C2 observer가 받은 실제 결과와 영수증 참조를 대조한다. `receipt.Validate()`도 직접 호출된다. 이는 `ReceiptWithoutExecutionResult` 적용 조건과 부합한다.
- 18과 27은 이미 발급된 실제 실행 결과를 반환·검증하고 원본 결과 기록을 관찰하므로 무결과 규칙으로 분류하지 않는다. 19–25는 실제 fresh 결과를 받고 fresh 처리 중 실패시킨 뒤 결과·예약·owner 기록 및 실패 이력을 대조한다. checkpoint 26은 `BeforeTerminalPublish`, 27은 `AfterTerminalPublish`로 구별되어 있다.
- AC007의 outcome 손상 시험은 실제 C2 observer가 전달한 결과의 `_outcome`만 기본값으로 바꾼다. 영수증과 실제 receipt witness를 별도로 검증하고, 손상된 memory 결과의 검증 거절을 기록한다. 실제 결과 발급 전 영수증 사건을 손상 결과의 유효성으로 대체하지 않는다.
- 사전진입 거절은 실행 CWT 등록과 C1 진입보다 앞선 실제 Bridge 순서를 소스에서 확인하고, execution/memory generation·proof·receipt를 `Unknown`으로 둔다. 실제 미관측 객체를 `Absent`로 주장하지 않으며 guard 미진입 및 저장 장벽 증거 부재와 분리한다.
- source-only 계수와 경계 순서 주장은 고정 Bridge/C1 source에 연결한다. 실제 callback 계수는 시험에서 받은 객체·호출 수를 사용한다. 승인된 REQ/AC 연결도 AC008 checkpoint 행과 AC007 손상 행에서 유지된다.

## 남은 한계

현재 스냅샷만 검토했다. 생성 중인 ExpectedRows의 exact schema·순서 대조, 최종 동결 지문, 실행 결과, 실제 영수증/결과 fixture 동작은 아직 검증되지 않았다. 이 문서는 C3 또는 C4 수용 판정이 아니다.
