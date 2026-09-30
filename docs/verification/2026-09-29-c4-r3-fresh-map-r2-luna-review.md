# C4 r3 fresh 결과 map r2 독립 검토

검토 대상 `docs/proposals/2026-09-29-c4-r3-fresh-api-and-result-map-r2-draft.md`의 SHA-256은 `B1B5A36757595F9DB993950819A61DB72ECBE44FFBDB369CA45D5C674FED50F6`이다. C401 원본·원본 C4 Review 계약·r3 정확 개정 및 178A 규범 한정 수용과 대조했다. 이전 C401 및 Luna 검토 원문은 수정하지 않는다.

**설계 정적 판정: P0 0건, P1 0건.** 이전 P1이던 closed output enum/phase 변경은 r2에서 복구되었다. 이 결론은 Draft 구현 승인 또는 실행 수용이 아니다. C4는 Review이고 C3의 마지막 필수 610 및 선행 수용은 아직 끝나지 않았다.

## 원본 closed output 복구

원본 C4 계약의 Outcome 다섯 값 `Completed`, `Busy`, `ConfirmationStale`, `ReloadRequired`, `ManualRepairRequired`와 Phase 네 값 `PreC1`, `C1Returned`, `C2Invoked`, `Completed`를 같은 이름과 의미로 보존한다. `FreshC3Required`는 enum outcome/phase가 아니라 별도 가드·생명주기 상태다. default 0과 정의 밖 값은 거절한다. C1 Busy/NoBarrier와 ConfirmationStale/NoBarrier는 각각 자기 Outcome·typed row·fresh handback을 가진 개별 `C1Returned` 행이다. C2 Busy는 C1 Busy와 분리되어 post-DiskPrepared `C2Invoked` terminal `ReloadRequired` composition으로 유지된다. 실제 C2 완료만 `Completed/Completed`이며 같은 C1 row·실제 C2 receipt를 보존한다.

C1 typed row를 얻기 전에 예외가 나면 result를 발급하지 않는다는 제한은 PreC1 phase 멤버의 존재와 모순되지 않는다. PreC1은 계약상 허용 enum 구성원으로 유지하되 해당 정상 return matrix를 만들지 않고 fault/trace 단계로만 사용할 수 있다고 설명한다. C1 terminal typed 행의 C1Returned 및 C2 비완료의 C2Invoked도 각 실제 호출 경계에 맞는다.

## Authority와 구성 경계

내부 결과는 actual registered sealed object이며 외부 getter는 Outcome·Phase·ExecutionGeneration으로 한정한다. C1 row/C2 result/receipt/fresh opaque는 사적 구성에 머물고 Hub는 C1 identity·prepared proof·root를 역추출할 getter가 없다. 모든 getter/Validate의 전체 enum/nullability/pair/root/generation/nested actual correlation 검증, C1 전체 row의 원래 typed 값, C2 원본 등록결과·동일 receipt, actual handback의 동일 result 연결을 요구한다. 정상 forensic read가 재소비·native cleanup·현재 live Unity 재인증을 일으키지 않는다는 구분도 적절하다.

lower fresh callable은 actual Busy/Stale row, 실제 C3 committed witness/actual C4 consume, 기존 원래 Owner 인스턴스·epoch/generation·pair/root/thread·pending guard를 확인하며 새 epoch token은 실제 same Owner operation의 비사용 참조여야 한다. CWT 등록된 opaque reservation이 bearer authority이고, same result/request당 한 번만 reserve/complete되며 완료/폐쇄 이력은 live permission이 아니다. `Close`는 reserve가 null인 부분실패도 actual result·original owner/pair를 확인해 fault/closed를 먼저 기록한 뒤 원본 pair를 종료한다. 정상 늦은 loser는 destructive terminal 경로 밖에서 비변경 거절한다. CWT·append history 및 old C3/Hub 이력 보존도 요구에 포함된다.

lower가 Hub ack를 역조회하지 않는 경계는 이 제안의 하위-상위 의존 방향과 양립한다. 제안은 Owner gate와 immutable operation context 아래 Q-B reserve→Q-A prepare/ack→Q-B commit→Owner same capability consume→lower Complete 순서를 규범화하고 새 owner slot은 lower Complete 전에는 live permission이 아니라고 한다. 이 private Owner source dominance를 실제 코드에서 proof해야 하며 reservation에 접근 가능한 다른 callsite나 조기 Complete 경로가 없어야 한다. 실패 시 원래 Owner catch가 lower Close와 신규 Q-A/Q-B 슬롯 폐쇄를 수행하고 gate는 finally 해제되어야 한다. 새로운 reverse Hub 증명 파라미터가 필요하지 않다는 설계 선택은 타당하나 구현 수용은 source 검수 이후다.

## 범위와 미완료 증거

원래 8개 runtime 경로 및 3 test+2 fixture 경로/meta, Q0 exact changed-source pin, Q-A/Q-B 별도 strict audit, old selections/names/180초 보존, source→pin→Q0→manifest→selection/rows→input capture→queue→run의 비재귀 freeze 순서가 C401/r3 범위와 일치한다. R11의 고정 14 및 884 입력 큐/선택을 C4 신규 source 검증에 재사용하지 않고 별도 C4 manifest·선택·QA protocol을 만들도록 한다. Q-A/Q-B ack를 lower Complete가 직접 인증하는 것처럼 주장하지 않고 Owner source 근거를 별도 산출하도록 한다.

남은 일은 정확 amendment의 Astra 승인, C3 필수 610 종료 및 선행 수용, 계획 API/새 thread fields를 포함한 최종 runtime/test/meta 구현, 실제 frozen manifest 및 named selection, 편집·실행 검증과 독립 QA다. 현재는 설계 텍스트만 확인했으며 source, QA 원장, 선택 또는 Unity 결과가 존재한다고 주장하지 않는다. 코드/QA/Git/network/Unity/컴파일/실행을 변경하거나 수행하지 않았다.
