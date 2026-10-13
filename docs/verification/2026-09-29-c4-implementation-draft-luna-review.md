# C4 구현 계약 준비 초안 독립 검토

검토 대상은 `docs/proposals/2026-09-29-c4-implementation-contract-ready-draft.md`이며 확인한 SHA-256은 `8339895BC58985997AD55A9577045F99D2EF3C74B1821CAA3E450EFCA6C9E9BE`다. 대조 기준은 Review 상태의 C4 계약 `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c4-reset-execution-bridge.md`, Approved C3 계약 및 R11 경계 독립 검토 `C9E969C33BF58F8EF401E5D468C6674239BF785BFBB952B936F3E5CE47592AA4`다. 정적 문서 검토만 수행했다. 구현·실행·수용 승인은 아니다.

**판정: P0 0건, P1 3건(초안에 명시된 미결정 A/B/C).** 초안은 Draft·구현 권한 없음, C4 Review 유지, C3 전체 선행 수용 미완료와 필수 Play 610 결과 대기 상태를 명시한다. 따라서 이 판정은 초안을 Ready 또는 Approved로 바꾸지 않는다.

허용 runtime 8개 경로는 C3 lower partial, adapter, router, 필요한 경우의 C2 guard 진입, C3 owner 및 별도 amendment가 필요한 Q-B/Q-A 두 경계로 한정되어 있다. 신규 bridge와 meta만 새 파일 후보이며 기존 meta 수정, C1/Profile 알고리즘·결과·proof 변경, asmdef/friend/public ABI, 제품 자산·설정은 제외한다. 테스트 3개와 독립 fixture 2개 및 각 meta도 지정 조립에 맞으며, Hub 정상 호출은 정확한 reflection binding으로 실제 opaque 반환 객체를 그대로 전달하고 사적 상태나 증표를 만들어 권한을 제조하지 않는다는 경계가 일관된다.

27개 C4 throw-only 지점은 정상 경로의 결과·권한을 대체하지 않고 예외만 일으키도록 제한한다. 기존 C1 51개 및 C2 19개 checkpoint를 그대로 전달하고, 실제 하위 구현이 생성한 typed 결과를 사용하며, 미도달 지점을 통과로 세지 않는 조건도 적절하다. AC001–008의 named 행은 actual request, guard, C1/C2 단일 호출, handback, barrier, lifecycle와 실패 경계를 구체화한다. AC009–010은 정적/API·감사 및 정확 focused/필수 선택·실행 검증으로 분리되어 있다.

Q0 감사는 변경된 Adapter/Router에 한정한 strict current successor 및 별도 승인으로, Q-B는 정확 import 예외와 기존 금지 본문을 보존하는 strict successor로, Q-A는 기존 transfer 감사 보존과 별도 amendment로 적혀 있다. 기존 감사 역사·선택을 덮어쓰거나 넓은 금지 규칙 예외를 만드는 내용은 없다. 14개 R11 predecessor pin을 고정하고, 새 사례 이름·NUnit 실제 이름/count·checkpoint 도달 원장은 구현 승인 후 최종 선언에서 생성하도록 구분한다. 기존 240/15/562/51/610 선택을 새 focused로 대체하지 않는다는 조건도 명확하다.

남은 P1은 모두 실제 증거를 합성하지 않고 해소해야 하는 계약 준비 차단이다.

- **P1 A — C2 Busy/ManualRepair:** 전체 C1→C2 연결에서 실제 lease 경쟁과 ManualRepair 조건을 결정적으로 만드는 fixture가 확정되지 않았다. 기존 C2 단독 행이나 제어점에서 만든 가짜 결과로 AC007 연결 행을 닫을 수 없다.
- **P1 B — 결과/proof 손상 시점:** 실제 C1 결과 생성과 C4 검증 사이의 손상을 throw-only 범위에서 관찰하는 fixture가 미정이다. getter 음성행 및 생산 검증 순서의 구조 검토를 AC003/005의 전체 실행 증거로 간주할지 판정이 남아 있다.
- **P1 C — 재진입/지연 경쟁:** callback 없는 동기 실행기의 same-thread 재진입 및 지연 pre-auth 경쟁창을 실제로 재현할 수 있는 범위가 미정이다. 도달 불가능한 콜백이나 시험 대기 gate를 추가해 재현성을 만들면 승인 경계를 벗어난다. 실행 가능한 실제 경쟁과 구조적 근거는 서로 다른 증거로 남겨야 한다.

따라서 A/B/C에 대한 Astra의 기술 판정과 정확 시험 경계 승인 전에는 이 초안을 구현 Ready로 취급할 수 없다. 특히 Play 610 결과는 아직 대기 중이며, C3 AC-007/008은 Open이고 C3 전체 선행 수용도 성립하지 않았다. C4는 계속 Review다. 실제 AC 실행·native 종료·집중 및 필수 선택 결과를 이 정적 검토로 주장하지 않는다.
