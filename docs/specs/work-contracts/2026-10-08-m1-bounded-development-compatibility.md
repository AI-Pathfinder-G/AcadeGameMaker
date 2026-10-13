# M1 원정 연결의 한정 개발·검수 호환 부록

- 상태: **Review — 실행 권한 없음**. 이 초안은 기존 Approved의 대체 적용을 아직 발효하지 않는다.
- 작성: 아스트라 `gpt-6-astra/ultra`, 2026-10-08. 독립 위험 판독·최종 승인은 미발급이다.
- 사용자 결과: 방 기본 이동에서 거점 출정→일반 방→보스 진입, 낙하·압착 실패와 저장 보존→거점 복귀→재출정까지 잇는 M1 흐름. 현재 이 부록이 다루는 쓰기 범위는 그중 **기승인 G3 관측 정확7파일과 최소 컴파일 준비**다. 이름이 M1이라는 이유로 나머지 제품 파일을 추가하지 않는다.
- 절차 근거: [활성 전략](./2026-10-03-play-first-delivery-strategy.md)의 REQ/AC-PROC-011·012, [운영 모델](../../agent-operating-model.md), ADR-0039·0043. 제품 권위는 [G3 계약](./2026-10-08-b1-basic-movement-observation-and-acceptance.md)와 [CH1 주 계약](./2026-10-03-chapter1-playable-delivery.md)이다.
- 현재 작성 산출물: 이 문서1. 기존 계약·master·전략·AGENTS·원장·제품·스테이징 수정0, 프로필 IO·컴파일·native·Unity·GUI·자식 배정0.

## 1. 실제 출발점과 원문 보존

아래 SHA는 이미 닫힌 출발 증거다. 다음 보정의 새 바이트를 이 SHA로 표시하지 않으며, 이후 일반 설명 수정 때문에 원본을 재발급하지 않는다. 경로의 `작업출력/`은 저장소의 `output/chapter1-playable-r1/`을 뜻한다.

| 입력 | 실제 원문·SHA256 | 한정 의미 |
| --- | --- | --- |
| G3 Approved | `docs/specs/work-contracts/2026-10-08-b1-basic-movement-observation-and-acceptance.md` · `39C64FF77AA6F8B89EAEAD430C0BAE5B19E780F9842182F37CDA61B29E8E222F` | 정확7·행동·소유·필수 검사 권위 |
| 최초 현재 소스 | `작업출력/evidence/b1-final-source-checkpoint-sol61.v8.json` · 133803바이트 · `58E1E09A8CD5E1ADBE3350DB5BFB648824CAE9393823569BC51BCD6D415A0F31` | 실제590, Window1 보정 뒤 원장 |
| 독립 소스 검수 | `작업출력/evidence/b1-basic-movement-source-review-luna6.v2.json` · 14279바이트 · `B407A30AC9CEBF71D7DF0FED0BEE51C8894EA74C98D34F18030A902EE6FF4845` | 실제 Luna/max, 정수 P0/P1=0/0, 정적 지적 폐쇄·참조 재사용 |
| 한정 수용 | `작업출력/preparation/b1-basic-movement-source-limited-acceptance-astra.v1.json` · 12952바이트 · `50BDD7049CE80471F388F621F636A9019EDD323491E3A893F5C1FE585F831891` | 실제590·정확 델타·정적 참조만 수용, 실행 권한0 |
| 최초 소유 계보 | `작업출력/preparation/b1-basic-movement-source-handoff-root.v1.json` · `806DD07AFEF8346F882ABF2C060F9FD2642234A51BCE1ADF25BC304FA0AF36ED` | 원래586에서 기존3/신규4/나머지583 |
| 보정 계보 | Window 승인 `EB7D3B69C18793C0452F81045E949F0BD20B856B6F569D19CB8B755E51A8968D`, Root 인계 `544817D76F713A3E84742D9DC90861D88F488751C15662C2991CD46A5694ED16`, 실제 구현 `7A52067DE433BCB89F4A9B0B895A28887BB1CEC8C48DF465FD704F35117F69CE` | 최초 v7·보정 전 QA·원시를 보존한 Window1/589 계보 |

Root 단일 작업 원장은 `작업출력/evidence/b1-basic-movement-minimal-compile-work-ledger-root.v1.json`이다. 읽기 시 `owner=Root`, `workLedgerMutableUntilBundleClosure=true`, `writingClosed=false`였다. 이 부록은 작성 중 원장의 SHA를 고정하지 않으며 원장에 쓰지 않는다. Root는 이미 기록한 실패·최소 재현을 지우지 않고 변경·소유·실제 입력·검사·예산·결과를 같은 원장에 누적한다. 원시 로그·체크포인트·이미 소비한 계획/승인은 불변 산출물로 별도 보존한다.

현재 준비 도구는 `작업출력/tools/b1-basic-movement-minimal-csc-root.v3.ps1`, 준비 설명은 `작업출력/preparation/b1-basic-movement-minimal-compile-tool-plan-sol61.v3.json`이다. 이 초안은 작업 중 최종 지문이나 독립 수용을 발급하지 않는다. 실제 계획 후보는 `작업출력/preparation/b1-basic-movement-minimal-compile-plan-root.v3.json`, 출력 접두사는 `작업출력/project/Library/b1-minimal-csc-g3-observation-r3`이다. 실제 계획·검수·실행 소유·native 승인의 미래 SHA는 여기서 채우지 않는다.

v2 Prepare 세션75624의 도구 exit1·`필수 필드 누락: path`, 컴파일/Unity 시작0·plan2/prefixr2 부재는 원장에 보존됐다. Root가 작성 반환 전 12938바이트/SHA `97ADD39193F3E77B09368A2C71D0406B9EB609D5B6AEB1D3BD8B73D40D47C175`를 소비한 오류도 보존한다. 최종50BDD의 발급은 그 조기 소비를 소급 승인하지 않는다.

## 2. 한 묶음의 허용 경로·행동·소유

**REQ-M1COMP-001:** 아래 경로는 `작업출력/project/` 기준이다. 최초 Approved의 인터페이스·관측 의미와 원래 게임 본문을 보존하는 타입·파서·경로·국소 코드 오류 보정만 같은 묶음에 포함한다.

| 정확 제품 경로 | 계속 보정 가능한 한계 |
| --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | 추가된 `TryReadChapter1PublishedObservationForAuthoring`의 기승인 저장값 순수 읽기. 원래 생산 본문·입력 callback·가드·프로필 경로 변경0 |
| `Assets/AcadeGameMaker/Runtime/Movement/Unity/PlayerMovementController.cs` | 추가된 `TryReadExistingMotionForAuthoring`의 기존 모터 순수 읽기. 생성·물리/좌표 쓰기·진행0 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1HazardObservation.cs` | 추가된 `TryReadPublishedHazardsForAuthoring`의 원래 위험 객체 읽기. 재계산·발행·게임 쓰기0 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs` | 기승인 전체 `UNITY_EDITOR` 타입, 실제 host/완료 tick 읽기·관측 수명·한 원시 writer. 게임 API·순서·권위 생성0 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs.meta` | 이미 발급된 GUID/기존 바이트 보존. 이 부록은 추가 meta 변경을 허용하지 않음 |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs` | 기승인 임시 구성·고정 표시·명시 문서 읽기·기존 수용 입구의 가드. 필수 필드/실제 타입·누락/중복/형식 거절을 보존 |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs.meta` | 이미 발급된 GUID/기존 바이트 보존. 이 부록은 추가 meta 변경을 허용하지 않음 |

정확7 밖 기존583·기존3의 원래 본문·자산·Builder/validator·asmdef/friend·참조 추가·새 공개/내부 공유 API·저장 포맷·프로필·키/세대·캐논·게임 수치 변경은 범위 밖이다. 새 파일이 필요하거나 입력 수명/접근권·성공 판정이 달라지면 줄 수와 무관하게 경계 변경으로 아스트라에게 되돌린다. 기존590이라는 수를 맞추려고 경로를 교환하지 않는다.

솔 `gpt-6.1-sol/ultra`은 Root가 정확 경로를 인계한 구간에서 제품 또는 도구의 유일한 작성자다. Root는 실행·작업 원장 소유다. 루나 `gpt-6-luna/max`는 독립 판독을, 아스트라는 권위·충돌·통합을 소유한다. 읽기 전용 영향 조사와 다른 공유 파일의 기승인 작업은 병렬로 진행할 수 있다. 제품 작성 중 동일 소스의 Prepare/컴파일/GUI0, 실행/출력 회수 중 제품 작성0이다. 같은 파일의 소유 이전은 원장에 이전 작성 종료·수신 소유를 남긴 뒤 수행한다.

준비 도구의 사전/객체 필드 판독·정확 경로·같은 바이트의 JSON/SHA 판독 보정도 이 준비 묶음 안에서 처리한다. 현재 v3와 그 준비 설명 두 파일의 미소비 수정은 기존 지적 목록에 누적한다. 소비한 v1/v2·실패를 덮지 않는다. 별도 보조 도구·버전 번호·승인 문서를 작은 보정마다 만들지 않되 이미 소비된 바이트를 보존해야 하는 실제 변경에서는 그 원문과 새 실행 입력을 구별한다. 새 도구 종류·프로세스 관리/보안/실행 권한 확대는 자동 포함하지 않는다.

## 3. 대체하는 선행과 그대로 유지하는 선행

**REQ-M1COMP-002:** 아래 대체는 이 부록이 독립 위험 판독 뒤 Approved로 채택되고 해당 실행 승인이 그 개정을 명명한 이후에만 적용한다. 동결 원문은 수정하지 않는다.

| 기존 경계 | 이 부록의 정확 적용 |
| --- | --- |
| G3 §2·§10.3의 소스 구현 종료→Luna 독립 델타/영향→Astra 한정 판정 | 최초 v8/B407/50BDD는 이미 충족한 출발 근거로 유지. 동일 정확7·행동·인터페이스 안의 이후 작은 보정마다 전체 SourceQA·새 한정 수용을 다시 발급하는 주기만 대체한다. 변경·국소 검사·실제 소스 개정은 원장에 누적하고 최종 M1 제출 바이트에서 영향 부분을 독립 재검수한다. 이전 QA를 변경 후 바이트의 실제 검수로 표시하지 않는다. |
| Review인 사후 계획9A6E00의 도구QA→Prepare→계획QA 직렬 운영 | v3 작성 종료→Root PS5 국소 확인/동적 핀 재확인→native0 Prepare1→Luna 한 과업에서 toolQA3를 먼저 닫고 그 SHA를 참조한 planQA3를 닫는 묶음으로 배치한다. Native 전 독립 도구·정확 입력 판독은 유지한다. |
| G3 §8·§10.2와 master §30의 별도 실행 승인 | 유지. 현재3×60/180의 실제 계획·도구/계획QA·Root 소유가 닫힌 뒤 Astra native 승인1이 필요하며 실제 호출 도구의 심사는 별개다. 소스 수용·이 초안·국소 검사 성공으로 대신하지 않는다. |
| 이미 소비한 R3의20핀·승인19명명핀/최상위7SHA와 당시 ASL/master | 그대로 보존. 이 부록은 종료된 R3를 반복하거나 당시 필수 핀을 줄이지 않는다. 다음 실행이 실제 소비하는 권위/입력만 그 실행 계획에 결속하며 종료된 실행의 핀을 새 문서에 재귀 전파하지 않는다. |
| 기존 첫 실패·단회·검사 횟수/시간 | 현재 native3 시작·단일 Execute·첫 실패 종료는 그대로다. 아래 §5의 준비 보정과 예산 계산은 추가 시작을 만들지 않는다. 다른 단회 계약의 제한도 이 부록으로 완화하지 않는다. |

현재 helper v3는 v8/QA2/50BDD의 정확 일치와 각각의 필수 스칼라를 검사한다. 따라서 **변경 후 소스를 과거 B407/50BDD에 맞는 것처럼 넣어 통과시킬 수 없다.** 현재 plan3/native 승인 후보는 실제 v8 고정 입력으로 준비한다. 향후 정확7 내부 보정에 대해 위 검수 주기를 적용하려면, 승인된 영향 범위와 현재 소스의 차이를 분리하여 읽는 실행 입력 소비 규칙을 같은 부록/기존 계획 묶음에서 먼저 호환시켜야 한다. 그 구현·독립 영향 판독과 실제 native 권한이 없으면 코드 보정·실행 없는 국소 확인까지만 진행한다. 이 초안은 동적 소스 권한을 이미 구현했다고 주장하지 않는다.

다음 변경은 묶음 종료를 기다리지 않고 영향 범위를 조기에 독립 검토한다: 최초 경계 밖 인터페이스/접근권·원시 의미/신뢰경계 변경, 참조 재사용 근거가 무효가 되는 변경, 실행 관리·입력 핀 검사의 결함, 후속 게임 진행을 막는 기술 결함. 같은 지적 목록에서 그 영향만 다시 판독하며 전체 원장을 재검수하는 별도 관문을 자동 추가하지 않는다.

## 4. 실행 입력의 비순환 결속과 하나의 원장

**REQ-M1COMP-003:** 새 실제 소스 SHA·전수 경로/길이·변경/불변 집합은 Root 원장 안의 `RuntimeInput` 개정과 실제 소비 계획에만 현재값으로 결속한다. 이미 닫힌 최초 체크포인트는 보존한다. 이 절은 새 파일명이나 이미 없는 개정값을 발급하지 않으며 Root가 원장을 갱신할 때의 내용 요구다.

`RuntimeInput`에는 실제 개정 식별자·작성 종료/소유 반환·소스 체크포인트/행 집합·기준 대비 정확 delta·원래3 본문/583/meta 보존·영향 REQ/AC·원래 영향QA의 적용범위·새 국소 결과·무효화된 증거·참조/RSP/도구/출력·소진 예산을 기록한다. 일반 설명 문서의 모든 SHA를 새로운 입력으로 요구하지 않는다. 기존 helper가 실제 소비하는 필수 checkpoint/sourceQA/sourceApproval/권위/도구/계획 핀은 명시 호환 전 유지한다.

닫힌 입력의 의존 방향은 **승인 개정·실제 소스/도구→실제 계획→toolQA→planQA→Root 실행 소유→Astra 실행 승인→실제 원시→결과QA/최종 수용**이다. toolQA가 planQA의 미래 SHA를 요구하거나 계획이 자신의 SHA를 포함하지 않는다. 원장은 이 결과들을 누적하지만, 계획이 변하는 원장 전체 SHA에 결속되고 원장이 다시 계획 SHA를 넣는 순환은 만들지 않는다. 필요하면 해당 시점의 `RuntimeInput` 불변 내용을 실제 계획에 포함하여 소비 바이트를 닫는다. 승인자가 자기 승인 SHA를 자기 필드로 넣지 않는다.

Root는 소유 반환 전 문서를 실행 입력으로 읽지 않는다. 닫힌 입력의 길이/SHA를 매개변수와 대조한 **같은 원문 바이트**에서 JSON을 읽고, 계획 생성 전·쓰기 직전·실행 전 가드에서 그 원문을 재확인한다. 읽은 객체와 나중에 재조회한 다른 바이트의 지문을 합성하지 않는다. 소스/도구/계획이 달라지면 영향받는 실행 입력을 갱신하고 미소비 상태에서 다시 닫는다. 설명 문구만 달라진 일은 제품 시험 재실행 사유가 아니다.

## 5. 현재 최소 컴파일의 유한 예산·실패·재개

**REQ-M1COMP-004:** 현재 준비 중인 예산을 늘리지 않는다.

| 항목 | 현재 상한·소유 |
| --- | --- |
| 조립체 순서 | `AcadeGameMaker.Movement.Unity`→`AcadeGameMaker.Input.Unity`→`AcadeGameMaker.Combat.Authoring.Editor` |
| compiler 시작 | 실제 최대3, 각 단계60초, 합산180초. 실제 시작 실패/성공 수를 모두 원시에 기록 |
| 정리 | 종료 확인5초·출력 회수5초. 기존 helper의 측정 범위를 축소하지 않으며 각 정리 한도도 독립 충족 |
| 자동 반복 | 0. 최초 실패에서 이후 단계 시작0, 원래 동일 프로세스 신원/handle로 종료·회수 |
| 실행 소유 | Root만. compiler server0·Unity0·Player 조건 컴파일0·GUI0·실제 게임 시험0 |
| 응답·출력 | 실제 plan3의 응답3, 조립체별 dll/ref/선택 pdb와 원시 CreateNew. 과거 prefix/로그/부분 출력을 덮지 않음 |

첫 실패 때 원시·실제 시작 수·경과·출력/종료 상태를 먼저 보존하고, 게임/도구/환경/증거 작성 오류를 구별한다. 입력 작성자는 해당 실행의 모든 프로세스·스트림 폐쇄와 Root의 소유 반환을 확인한 뒤 보정한다. 종료/회수 미확인, 신원 불명, 시간초과, 입력 차이, 권한 거절은 같은 실행을 계속할 수 있는 조건이 아니다. 로그 침묵은 실패 판정 근거가 아니다.

재개 가능성은 남은 횟수 `3-실제 시작 수`와 남은 시간 `180-기존 누적 실행 경과`를 둘 다 계산하여 판단한다. 실패한 단계와 그 변경의 영향을 받는 모든 후속 소비자를 포함한 필수 시작 수가 남은 횟수 이하이어야 하며, 승인된 각60/전체180·종료5/회수5를 그대로 집행하는 계획이어야 한다. 남은 시간을 이유로 각 단계의 승인 상한을 임의로 바꾸지 않는다. 이미 쓴 시간을 새 계획/새 프로세스에서0으로 초기화하지 않는다. 정리 시간은 기존 helper가 합산에 넣는 범위 그대로 누적하며 별도 정리 상한도 유지한다.

**현재 정확3 정상 결과가 필요한 이 계획에는 실제 실패 후 컴파일 재시작 여유가 없다.** 첫 단계 실패 뒤 남은2로 필요한3, 둘째 실패 뒤 남은1로 필요한2, 셋째 실패 뒤 남은0으로 필요한1을 충족할 수 없다. 성공한 이전 단계도 소스/참조 영향이 있으면 재사용하지 못하므로 요구량은 줄지 않는다. 따라서 native 시작 뒤 첫 실패에서는 이3회 묶음의 성공 수용을 닫고 코드·원인 조사만 이어간다. 추가 시작이나 별도 실행 권한이 필요하면 그 실제 차이를 한정 개정으로 판정하며, 자동 승인·숫자 재해석·미사용 단계의 성공 처리0이다.

native 시작 전의 순수 준비/필드 판독 실패는 compiler 시작 예산을 소비하지 않는다. 최초 오류와 최소 재현을 원장에 남기고 같은 허용 도구 경계에서 보정·실행 없는 국소 확인은 계속할 수 있다. 그 뒤 전체 Prepare 재호출은 정확 출력 부재/부분 출력 보존·단일 소유·현재 핀·명명된 단회 계획을 충족한 경우에만 수행한다. 이번 v3 Prepare는1회이며 실패 후 자동 재호출0이다. 같은 원인 두 번이면 큰 준비 호출도 멈추고 최소 재현·변경·새 관측 목적을 먼저 정리한다.

현재 v3 helper는 단일 Execute와 CreateNew 원시를 전제로 하고 재개 기능이 없다. **일부 예산이 남았다는 산술만으로 두 번째 Execute를 호출할 수 없다.** 이전 프로세스가 시작되지 않은 실행 전 거절도 원시/경로 상태·권한의 소비 의미를 확인해야 한다. 미래 재개를 허용하려면 해당 별도 native 승인이 명시한 재개 조건과 이를 검사하는 소비 도구가 먼저 있어야 한다. 이 초안은 그 기능 구현이나 현재 단회의 소급 변경을 승인하지 않는다.

## 6. 현재 제출 순서와 독립 검사

**REQ-M1COMP-005:** 다음 순서를 하나의 작업 묶음으로 관리한다.

1. 솔은 helper/준비 v3의 보정을 닫고 Root에 반환한다. Root는 실제 PS5에서 사전·순서 사전·일반 객체의 필드 조회, 누락/잘못된 자료형 거절, 동일 바이트의 동적 핀 검사를 국소 확인한다. 전체 helper 실행·compiler 시작과 이 순수 확인을 구별한다.
2. Root는 닫힌 실제 v8/B407/50BDD와 권위·원래 RSP/참조/자원·Editor 부재·새 경로 부재를 확인하고 native0 Prepare1을 수행한다. 실제 응답3/계획1의 원문을 닫아 같은 원장에 기록한다.
3. 루나는 한 과업에서 helper의 영향 delta와 실제 계획을 독립 판독한다. `tool-review-luna6.v3.json`을 먼저 닫고, 그 SHA를 명명한 `plan-review-luna6.v3.json`을 다음에 닫는다. 실제 모형/노력·정수 P0/P1·명시 bool·입력/출력/예산·참조 재사용·소유 가드를 검사한다. 앞선 전체 sourceQA는 제품 바이트가 같은 경우 반복하지 않는다.
4. 실제 계획3/독립 판독과 이 부록의 위험 판독이 충족된 뒤 아스트라가 이 부록의 채택 범위를 확정한다. Root가 그 권위와 정확 plan3를 결속한 실행 소유 원장을 닫고, 아스트라가 기존 경로의 별도 native 승인1을 발급한다. 부록을 가리키는 승인 개정은 한 방향으로만 결속하며 자신의 미래 SHA나 Root 소유의 미래 SHA를 부록에 요구하지 않는다.
5. Root는 최종 권위·계획·양 QA·소유·실행 승인·입력을 다시 대조하고 실제 도구 심사를 거쳐 단일 Execute를 수행한다. 결과는 정수 종료·최대3 시작·시간·출력/ref·소스 불변·프로세스 폐쇄·회수 원시로만 판단한다.
6. 루나는 실제 결과의 영향 범위를 독립 판독한다. 최소 컴파일 수용은 파서 반례 실행·Player 조건·GUI·방 이동·M1 완주 수용으로 확대하지 않는다. 별도 실제 검사 중 미실행 항목은 계속 미실행으로 남긴다.

본 부록을 채택하는 시점의 실제 계획이 기존39C64만 필수 권위로 소비한다면 그 핀은 보존하고 별도 native 승인에서 채택된 부록 개정을 추가 명명할 수 있다. 다만 부록이 실제 source 선행·재개·예산 가드를 바꾸는 실행에는 기존 helper와 불일치가 없는지 독립 판독해야 한다. 부록의 설명을 근거로 실제 가드를 생략하지 않는다. 현재 plan3의 v8 고정·3회 단일 실행에는 예산/재개 확대를 적용하지 않는다.

## 7. M1 제출·남은 실제 선행·막힌 경로

**REQ-M1COMP-006:** 최종 M1 제출은 같은 최종 바이트·실제 입력의 관측/실패/복귀/재출정 증거와 모든 필수 AC를 묶는다. 국소 확인은 솔의 자체 확인이며 독립 수용이 아니다. 루나는 최종 변경 소스·장면·입력 원시·영향 AC를 검수하고, 아스트라는 실제 결과의 한정 범위를 통합한다. 중간 보정마다 전체 회귀를 반복하지 않으며 새 실패/변경의 영향만 다시 검사한다. 필요한 필수 검사는 묶음 끝에서 같은 최종 입력으로 모두 충족한다.

G3 실제6방/10구성 이동과 연속 원시→방별 Luna 검수/Astra 수용→정확6 수용 기록→집중35 단1회900초→회귀44 두 모드 각1회 합산1800초→별도 수동12→종단 정상1/실패3 각1800초·합산7200초와 별도 실제 창 키보드/포인터→현재 소스 빌드1회900초를 유지한다. 실제6방 수용 전 출정 풀 편입0, 필수9회·Q0 인간 승인·키/세대·프로필·원격 권한 및 B1 실제 독립 FINAL 전 C1C 공유 통합0이다. 이 부록은 G3 이후 제품 쓰기나 그 실행 예산을 추가 발급하지 않는다.

GUI는 앱 동의와 별도 native 연속 관측 예외 결정이 대기 중이다. 앱 동의만으로 연속 화면/입력 증거가 확보됐다고 표시하지 않는다. 허용 API의 지점 화면·`press_key`를 연속 녹화·키 지속/해제 성공으로 바꾸지 않는다. 관측기 tick JSONL은 별도의 native 증거를 대체하지 않는다. 인간 결정 전 캡처 보조수단 제작·설치·실행·전역 입력 관측·외부/숨은 API 우회0이다. 막힌 GUI 밖의 충돌 없는 기승인 설계·소스·준비는 계속하되 native 권한을 암묵적으로 옮기지 않는다.

## 8. 인수 기준과 종료 조건

| AC | 연결 REQ·측정 가능한 종료 조건 |
| --- | --- |
| AC-M1COMP-001 | REQ001: 실제 delta가 정확 허용 경로/메서드·인터페이스 안이고 원래 본문/583/meta·프로필/저장/캐논이 보존된다. 단일 작성 종료/반환과 영향 REQ/AC가 원장에 있다. |
| AC-M1COMP-002 | REQ002: 실제 발효 승인에 대체하는 검수 주기와 유지하는 Native 선행이 명시되고, 과거 B407/50BDD를 새 바이트 검수로 표기하지 않는다. 소비 도구와 새 권위 간 불일치가 없으며 무승인 경계 변경0이다. |
| AC-M1COMP-003 | REQ003: 실제 실행 입력·권위 개정·소진 예산의 비순환 결속을 원문에서 재현할 수 있다. 작성 중 원장 전체 SHA·자기 SHA·미래 검수 SHA 순환0이고 반환 전 소비0이다. |
| AC-M1COMP-004 | REQ004: 실패/부분 출력이 보존되고 실제 시작≤3·각≤60·합산≤180·종료/회수 각각≤5, 자동 반복0이다. 부족한 예산이나 재개 미지원 때 두 번째 Execute0이다. |
| AC-M1COMP-005 | REQ005: native0 Prepare 결과와 종료된 toolQA→planQA, Root 소유→Astra 별도 승인→실제 도구 심사→실제 결과 판독의 순서를 원문으로 확인한다. 같은 제품 입력의 불필요 전체 SourceQA/회귀 반복0이다. |
| AC-M1COMP-006 | REQ006: M1 제출물에서 필수 원시/AC와 미실행을 구분하고 G3/35/44/12/종단/빌드·9회·Q0·B1/C1C 경계를 유지한다. GUI 동의·연속 관측 예외를 우회한 행동0이다. |

기존 REQ/AC-C1B-G3OBS-001~012와 REQ/AC-PROC-011·012를 함께 추적한다. 이 문서 작성만으로 위 AC 통과·효율 개선 실적·제품 진척을 발급하지 않는다. 현재 실제 AC 통과 목록은 비어 있고 최종 승인·native/Unity/GUI/컴파일 권한은 모두 없다.

독립 위험 판독에서 확인할 핵심은 정확7 내부 보정과 권한 확대의 구별, 동적 현재 입력과 역사 QA의 적용 범위, 별도 native 승인/실제 helper 가드의 일치, 최대3의 재개 불가능 경계다. 현재 준비 설명의 helper 지문과 실행 소유 필드 예시가 서로 다르면 같은 미소비 묶음에서 실제 값으로 맞춘 뒤 제출한다. 단순 이전 예시를 새 소유/승인으로 복사하지 않는다.

초안 작성 종료 뒤 문서 소유를 Root에 반환한다. 이 문서는 독립 위험 판독과 실제 native 준비의 권위 정리가 성립하기 전까지 Review를 유지한다. 최종 채택 때도 기존 master/역사/원시를 소급 수정하지 않으며 Root의 단일 작업 원장에 채택 개정과 실제 적용 결과를 남긴다.

## 9. 현재 고정 v8·계획3에 대한 한정 채택 — Approved

- 현재 적용 상태: **Approved — 이 절의 고정 입력·준비 묶음·단일 native 실행 승인 연결에 한정**. §1~8의 Review 본문은 당시 제안 원문으로 보존하며, 미래 변경 소스의 검수 주기 대체·동적 권한·재개 기능은 채택하지 않는다.
- 실제 승인자: 아스트라 `gpt-6-astra/ultra`, 2026-10-08. 검수 원문은 이 절 앞의 정확25010바이트/SHA `56CDC6654B97CF061A045AF71B37B7F23FA40CC047EF1A57C5C2206D693F7BAB`다.
- 실제 도구 독립 검수: `output/chapter1-playable-r1/evidence/b1-basic-movement-minimal-compile-tool-review-luna6.v3.json`, 9474바이트/SHA `2C37C41EF45CD460577952E0185BEC77B524F22766DD76A8A10B66C8DC02100A`.
- 실제 계획·부록 사전 위험 판독: `output/chapter1-playable-r1/evidence/b1-basic-movement-minimal-compile-plan-review-luna6.v3.json`, 10997바이트/SHA `A54193A66EA7DB036184ABA414FE2FDC5F6219F83D3AC260BCFA6A0B69ECFB5A`. 두 검수는 실제 `gpt-6-luna/max`, 정수 P0/P1=0/0이며 쓰기 종료·Root 반환을 확인했다. 계획QA의 사전 위험 판독 범위만 채택하며 미래 코드/실행의 검수로 확대하지 않는다.
- 실제 계획: `output/chapter1-playable-r1/preparation/b1-basic-movement-minimal-compile-plan-root.v3.json`, 230973바이트/SHA `5A383F80CEA9DB01BA83DFD8E6573656983CD59CE08A2D800147B558325E2A76`. helper SHA `FB26B48A2A524C07649DEEE28A5DCEFC4B95BC51868E5C1290B3B253075350ED`, 현재 v8/QA2/소스 수용은 §1의 58E1/B407/50BDD다. 기존 G3 Approved39C64·원래 소유806DD와 모든 실제 필수 입력을 유지한다.
- Root 단일 실행 소유: `output/chapter1-playable-r1/preparation/b1-basic-movement-minimal-compile-ownership-root.v1.json`, 4964바이트/SHA `BCC6CD1917652F76B9AE081C59A339BDD78F8A113618FFB6CD79585DDB9A95B1`. 실제 Root `gpt-6.1-sol/medium`·sourceWritingClosed/rootSingleWriterConfirmed/writingClosed=true, 소유Root와 응답3·출력9·정확 경로/예산을 대조했다.

채택 범위는 이미 이루어진 국소 Property/동적 핀 보정→Root native0 Prepare3→한 루나 과업의 toolQA3/실제 planQA3→Root 소유→별도 Astra native 승인1→실제 결과 독립 판독의 묶음 운영이다. 실제 준비 세션40422의 exit0·응답3/계획1과 입력491·자원12·기존 로컬 참조15, 실제 소스590 계보를 판독했다. 제품 미변경에 대해 중복 전체 SourceQA/590 재해싱을 요구하지 않는다. 실제 실행 입력과 도구 가드·별도 호출 권한 검사는 그대로다.

이번 실행은 Movement→Input→Editor 정확3 정상 결과가 필요한 **최대3 native 시작·각60초/합산180초·종료5초/회수5초·단일 Execute·첫 실패 후 나머지0·자동 재시도0**다. Unity/GUI/Player 조건/게임 시험/프로필 실행0이며 compiler server0이다. 실패 뒤 남은 시작 수로 전체3 성공을 만들 수 없고 helper에 재개 기능도 없으므로 두 번째 Execute0이다. §5의 잔여 예산 계산은 재시작 권한을 발급하지 않는다.

별도 실행 승인 경로는 `output/chapter1-playable-r1/preparation/b1-basic-movement-minimal-compile-execution-approval-astra.v1.json`이다. 그 승인에서 이 채택 후 부록의 실제 SHA·이미 닫힌 Root 소유/검수/계획을 결속한다. 이 부록은 자신의 SHA나 미래 승인 SHA를 포함하지 않는다. 기존 plan3/helper의 소비 권위39C64와 동적 원문3핀을 바꾸지 않으며 아스트라 승인이 추가 채택 근거를 한 방향으로 참조한다. 실제 도구 심사·Root 실행 직전 입력/소유·원시 부재 검사는 별도 필수다.

현재 검수된 v8 이외의 소스, 새로운 API/파일/행동, 작은 보정 뒤의 새 컴파일, 예산 증액·동적 sourceQA 대체·재개 기능·GUI/빌드는 이번 채택 대상이 아니다. §1~8에서 제안한 미래 대체는 그 영향·소비 도구 호환·별도 실행 권위가 실제 닫히기 전까지 Review로 남는다. G3 실제6방/정확6 수용·35/44/수동12/종단4/현재 빌드·9회/Q0/키세대/프로필/원격·B1 FINAL 전 C1C 통합0은 유지한다.

REQ/AC-C1B-G3OBS-001/010, REQ/AC-PROC-011/012 및 AC-M1COMP-002~005의 현재 준비/권한 경계만 결속했다. 실제 컴파일 결과·제품 AC 통과·M1/B1/CH1 수용은 아직0이다. 원래 Prepare 실패75624, 반환 전97ADD 소비, 국소 확인 harness 오류·실패 원시를 보존한다. 승인자 제품·도구·원장·프로필·native·GUI·자식 배정0, 이 부록 채택과 별도 실행 승인1만 작성하며 종료 뒤 Root에 반환한다.
## 10. Unity6000.6 식별값 호환과 Input→Editor 새 진단 묶음 — Review

### 10.1 상태·원인·이미 닫힌 권한

이 절은 **Review**다. §1~9의 정확29693바이트/SHA `6BDC84F87176CCE5B52CD5DAA68A1D9EA682A09E45E68D72CED6B577402ECAD7`를 접두사 그대로 보존하며 당시 한정 승인과 소비 사실을 바꾸지 않는다. 소비된 원문의 바이트 보존 사본은 `output/chapter1-playable-r1/evidence/b1-m1-compatibility-approved-before-uint64-recovery.md`이며 정확29693바이트/위6BDC SHA로 CreateNew 보존했다. 이 절의 구현·helper4·추가 native·승인v2는 아직 발급하지 않는다. 작성자는 아스트라 `gpt-6-astra/ultra`, 허용 쓰기는 이 절 추가1과 소비된 원문 보존 사본1뿐이며 제품/도구/원장/프로필/native/GUI/자식 배정0이다.

실제 `project/Library/b1-minimal-csc-g3-observation-r3/result.json`은 8347바이트/SHA `39FF5DE973D7B34599F6868C90888613349BE7CAFC737594C81096E910B1DFFC`다. Movement 정수exit0, Input 정수exit1, Editor 미시작이며 actualStarted2·notStarted1·success=false다. 시작한 두 프로세스 폐쇄/회수true·sessionErrors0·자동 재시도0이다. 세션 경과92.8710416초와 단계별 보고 경과 합36.5900427초를 구분한다. 남은1로 필요한 Input+Editor2 성공을 만들 수 없으며 기존 단일 Execute에는 재개 입구가 없다.

Input stdout의 직접 오류는 Observer273/274의 `GetInstanceID()`7회와 `SceneHandle`의 암묵적 int 변환1회 CS0619다. 정렬 인자를 포함한 FindObjectsByType 관련 CS0618 및 기존 다른 파일의 CS0414는 별도 경고이며 이 실행의 정수exit1 원인8과 구별한다. 아직 컴파일하지 않은 Window295에도 `collector.GetInstanceID()`와 scene.handle의 int 대입이 있다. 같은 오류 계열을 두 파일에서 한 번에 처리하며 Observer만 보정해 Editor 실패를 반복하는 계획을 제안하지 않는다.

원래 helper3 `FB26B48A…`, plan3 `5A383F80…`, 승인1 `3E8ECC16…`, Root 소유 `BCC6CD19…`, v8/QA2/수용 `58E1/B407/50BDD` 및 모든 실패·원시·부분 산출은 그대로 보존한다. 현재 실패의 Luna 독립 결과QA는 이 절 작성 때 아직 전달되지 않았고 그 SHA/한정 수용은 `null`이다. 위 실패 원문을 읽은 사실과 독립 수용을 구별한다.

### 10.2 정확2파일의 후보 보정

**REQ-M1COMP-R1-001:** 다음 상대 경로는 `output/chapter1-playable-r1/project/` 기준이다. 기존 정확7 중 두 C#만 보정 후보이며 새 제품 파일/meta·원래 reader3/본문·Movement 인터페이스·게임 상태/행동·프로필/영속 저장 포맷·asmdef/friend·참조 추가0이다.

| 파일 | 후보 변경 |
| --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs` | 실제 Unity 객체/scene 식별값의 공개 UInt64 원문 API와 관측 DTO의 정확10필드·그 직렬화/역직렬화 연결만 호환. 기존 callback/순서/단일 writer·32참조표/ReferenceEquals·receipt/RunKey/Generation·scene 신원 비교는 보존 |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs` | 설치 설정의 식별값2 생산과 표본 읽기의 식별값8을 같은 UInt64 계약에 연결. 기존 엄격 문서 가드·구성/표시·소유/수용 입구·원시 상한은 보존 |

후보10필드는 Configuration의 `installedInstanceId`, `installedSceneHandle`2개와 SampleRow의 `hostId`, `routerId`, `hazardsId`, `playerId`, `combatId`, `transferId`, `collectorId`, `sceneHandle`8개다. 기존 필드명은 호환 대상으로 유지하며 `unityPid`, 순서값, frame/tick, 수명/참조표 지역번호 등 다른 int/long 필드를 일괄 바꾸지 않는다. 이10개는 Unity의 실제 객체/scene 식별 원문용 unsigned64 후보이지 게임 권위나 지역 참조번호를 새로 발급하는 API가 아니다.

솔의 최종 읽기 조사와 아스트라의 설치 XML·공식 문서 직접 대조로 객체 원문 API는 `EntityId.ToULong(obj.GetEntityId())`, scene 원문 API는 `scene.handle.GetRawData()`로 확인했다. 앞의 메서드는 정적 `ulong ToULong(EntityId)`이고 뒤의 메서드는 인스턴스 `ulong GetRawData()`다. 설치 `C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Managed/UnityEngine.xml`의 34079/101986행과 공식 [EntityId.ToULong](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/EntityId.ToULong.html), [SceneHandle.GetRawData](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/SceneManagement.SceneHandle.GetRawData.html)를 근거로 한다. EntityId 내부 비트 배열의 장기 불변성이나 세션 간 영속 신원은 주장하지 않는다. `confirmedEntityIdAccessor`와 `confirmedSceneHandleAccessor`는 위 실제 공개 API이며, 스칼라 계약의 채택 `approvedScalarSchemaRevision`과 실제 직렬화 왕복 성공은 아직 미발급/미실행이다. 하위32비트·GetHashCode·unchecked 축소·문자열 임시 치환·상수0·새 지역번호로 원래 식별자를 대신하는 방안은 제외한다.

G3 원문의 instanceId/식별표 요구는 실제 Unity 객체 식별 원문과 같은 세션 참조 동일성 보존을 위한 것이다. 이 기술 호환을 채택할 때 UInt64 원문에 대한 producer/DTO/reader/원시 검수 의미를 명시해야 한다. 기존32개 참조표의 번호/소유와 UInt64 원문 식별자를 혼합하지 않는다. 과거 실제 관측 JSON이 있다면 재작성하지 않고 호환 여부를 별도로 판독한다. 이 후보는 기존 `schemaVersion` 필드/값과 필드명·키/세대를 보존하고, 정확10 스칼라의 손실 없는 UInt64 표현을 이 절의 명시 호환 개정으로 결속한다. 새 영속 스키마·세대·기존 원문 재작성0이다. 식별값의 표현 호환을 기존 일반 정수/긴 정수 필드나 역사 자료의 재해석으로 확대하지 않는다. 정렬 모드 경고 보정도 이 정확2파일 API/스칼라 범위 밖이다.

### 10.3 명시 UInt64 읽기와 직렬화 한계

**REQ-M1COMP-R1-002:** Window295의 설치 설정2 생산과 697/698의 기존 sample projector8곳만 위 API 및 단일 내부 reader `UInt64Integer`로 연결한다. 기존 `Integer`/`LongInteger` 본문과 나머지 호출은 불변이다. 새 reader는 기존 필수 키 존재 검사·JSON Number 종류·원문 비음수 십진 정수 리터럴을 확인하고 `TryGetUInt64`로 0~18446744073709551615 범위를 검증한 뒤 실제 UInt64 값을 반환한다. `null`, 누락, bool, 문자열, 배열/객체, 음수와 음수0, 소수점/지수, 범위 밖·잘못된 JSON을 거절한다. 형식 보정으로 중복/escape 동명키·문자열/UTF-8·루트/배열/깊이·크기 가드를 약화하지 않는다. 중복키는 기존 문서 경계에서 거절하며 이 스칼라 reader가 임의 배열을 순회하지 않는다.

0을 읽을 수 있다는 형식 규칙은 null 객체나 잘못된 scene을 유효 신원으로 승인하는 규칙이 아니다. 기존 실제 객체/유효 scene·소유·원문 전후 비교를 유지한다. 설치 설정2는 Window가 실제 객체/scene에서 생산해 Observer가 저장하고 재로드하는 경로이므로 sample reader8을 바꾸는 것만으로 충분하다고 가정하지 않는다. `JsonUtility.ToJson` 및 기존 Configuration 재로드의 ulong 원문 보존과 모든 producer/consumer를 함께 조사한다.

기존 JsonUtility 유지의 정적 근거는 설치 `project/Library/PackageCache/com.unity.test-framework@7a3849e09bd0/UnityEngine.TestRunner/UnityTestProtocol/MessageForRetryRepeat.cs`의 public ulong time과 ToJson 사용, 설치 `Managed/UnityEditor.xml` 68384행 ulongValue·68587행 UInt64다. 공식 [JsonUtility.ToJson](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/JsonUtility.ToJson.html)은 공개 필드를 Unity 직렬화기로 처리하는 경계를 설명한다. 이는 기존 직렬화기 선택의 지지 근거이며 최대값·Configuration 재로드·sample 전체의 실제 왕복 성공 증거는 아니다. 새 serializer·참조 추가0이다.

최소 경계 검토 대상은 0, Int32 최대 초과, Int64 최대 초과, UInt64 최대의 정확 값 보존과 누락/null/음수/음수0/실수/지수/최대 초과/중복키 거절이다. 검사 파일·selector·생산 hook·새 프레임워크를 추가하지 않는다. 현재 `actualSerializerRoundtripExecuted=false`, `actualParserCounterexamplesExecuted=false`다. 실제 최대값 직렬화 왕복과 설치 설정2/표본8의 같은 형식 읽기 확인은 **별도 승인되는 최초 Play 안**에서 수행하도록 해당 실행 계획에 입구·원시 관측·수용 조건을 명명하며, 예비 Play·별도 Unity 실행0이다. 최대값 형식 확인을 실제 객체 ID로 꾸미거나 관측 sample에 주입하지 않는다. 원래 R01의 연속성·첫 실패·단회·예산·GUI 동의 조건을 유지하며 이 절은 그 Play 실행 권한을 발급하지 않는다. 아래 두 compiler 성공을 직렬화 실행으로 계산하지 않고, 최초 실제 관측에서 형식이 보존되지 않거나 필요한 증거를 얻지 못하면 수용하지 않는다.

### 10.4 기존 Movement 결과의 조건부 재사용

**REQ-M1COMP-R1-003:** 실제 r3 Movement 결과만 새 Input/Editor의 읽기 전용 선행 참조 후보로 재사용한다. 전체 실패 세션을 전체 컴파일 성공으로 수용하지 않는다.

| 기존 실제 산출 | 길이·SHA256 |
| --- | --- |
| `project/Library/b1-minimal-csc-g3-observation-r3/AcadeGameMaker.Movement.Unity.dll` | 28672 · `BA874B466ED2E4D43697BE1D5D809E910D134F02AAA909C20CD0F75CC336727F` |
| 같은 경로의 `AcadeGameMaker.Movement.Unity.ref.dll` | 10240 · `0920429D88AADE31BC34B8F6A8E6CC27C70F16A249E8B9657755634A6EFADBAB` |
| 같은 경로의 `AcadeGameMaker.Movement.Unity.pdb` | 25824 · `3B89C8802FFD1BB22063698303F683CC538F1150485D1532A81E6CE09FC225DD` |

Movement 실제 소스2는 `Runtime/Movement/Unity/AssemblyInfo.cs` 1110바이트/SHA `1C0E7370AA05439FA9DF9A0BDDCCA28ED4C5719F4124D969FB2D08783DAD79BB`, `PlayerMovementController.cs` 43508바이트/SHA `1C368BE8919E9DF8FAEF4321157FCBE98208BF6514BD26411C36F2E68651A542`다. 아스트라 읽기에서 이2개와 전후 RSP·산출3·로그2의 길이/SHA가 실제 결과/닫힌v8에 일치했다. 새 보정 후에도 Movement 소스·인터페이스·원래 RSP/define/참조/자원·실제 DLL/ref의 생성 계보가 유지되는지 Luna가 영향 판독해야 한다. 현재 `movementReuseIndependentlyAccepted=false`다.

새 Input RSP는 위 실제 r3 Movement ref를 읽는다. 새 Editor RSP는 같은 r3 Movement ref와 **이번 새 Input의 성공 산출 ref**를 읽는다. 과거 Bee의 Movement/Input ref나 실패하여 생성되지 않은 r3 Input ref로 대체하지 않는다. 최초 입력 계획26AB의 원본 RSP를 기준으로 Input[1]의4행·Editor[2]의5행만 실제 준비에서 변형하며 양쪽 Movement 참조는 위 r3 ref10240바이트/0920429D에 연결한다. Movement 새 컴파일 시작0, 기존 산출 덮어쓰기0이며 Movement를 새 실행 results에 넣어 새2 시작으로 세지 않는다. 새 Input ref의 SHA는 실제 Input 성공·폐쇄·회수 뒤에만 발급하고 Editor 시작 직전 확인한다. Input이 실패하면 Editor 시작0이다. 출력/ref 원시와 새 source/meta/조건부 소비 관계의 독립 판독이 필요하며 실제 불변 파일의 반복 전수 sourceQA로 이를 대신하지 않는다.

### 10.5 새로운 입력·준비·검수·승인 한 묶음

**REQ-M1COMP-R1-004:** 기존 v3의 정확v8/SourceQA2/50BDD 동일성 가드는 현재새소스를 받아들이지 못한다. 그 가드를 끄거나 기존 승인/QA를 고치는 방법을 사용하지 않는다. 새 helper4에서 **역사 기준·실제 보정 입력·독립 델타 판정·현재 실행 권위**를 구별하는 계약을 이 절과 함께 한 번 호환시킨다.

1. 작성 종료된 솔 API/직렬화 조사와 Luna 실제 실패 결과를 판독하여 정확2파일 기술 호환·새 유한 예산의 범위를 아스트라가 먼저 승인한다. 지금은 후보다. Root가 실행 폐쇄를 확인하고 Sol에 정확2의 유일한 쓰기 소유를 인계한다. 아직 이 단계의 승인·제품 쓰기는0이다.
2. 솔은 범위 안 보정/국소 확인을 같은 원장에 누적하고 쓰기를 닫아 Root에 반환한다. Root가 실제 파일 수/전수 행·정확2 변경/나머지588 불변을 담은 새 `evidence/b1-final-source-checkpoint-sol61.v9.json`을 작성한다. 경로는 제안이고 내용·SHA는 현재 `null`이다. Root 작성자의 실제 모델을 기록하며 파일명의 역사 역할 표기를 실제 작성 모델로 오인하지 않는다. 현재590을 그대로 복사하거나 미래 SHA를 채우지 않는다.
3. 새 helper4의 Prepare는 native0이다. 현재 v9·채택된 §10·Root 쓰기 종료·v8 대비 정확2 delta/나머지588 불변·기존58E1/B407/50BDD 역사 기준·실제 r3 Movement 개별 정수0/현재 ref핀만을 선행으로 요구하여 정확 Input/Editor 응답2와 실제 계획을 만든다. **이 Prepare는 보정 소스의 독립 수용을 발급하지 않는다.** 미래 toolQA4/planQA4·Root 실행 소유v2·source/native 결합 승인v2를 Prepare 입력으로 요구하지 않아 순환을 막는다. 기존 전체 r3 실패와 당시 P1=2는 역사로 보존하며 그 안 Movement 개별 성공을 전체 성공으로 바꾸지 않는다. 새 source 전체QA가 없다는 이유로 과거 sourceApproved=true를 현재 수용으로 투영하지 않는다.
4. Luna 한 과업이 실제 보정2 delta·정적 원문/형식·Movement 재사용 영향·helper4 source 가드/예산·실제 RSP2/계획을 함께 검수한다. 필요한 toolQA를 먼저 닫고 그 SHA를 소비하는 실제 계획QA를 다음에 닫는다. 계획QA에 현재 checkpoint·정확2 delta·나머지588의 실제 계보와 Movement 재사용 판정을 명시하여 별도의 전체590 SourceQA·상태 보고를 반복하지 않는다. 바뀐2의 영향을 기존 B407에 소급하지 않는다.
5. Root가 실제 계획/양 검수/현재 source·Movement ref/출력·소유를 닫는다. Astra는 **소스 한정 판정+추가 native 예산 승인v2를 한 파일** `preparation/b1-basic-movement-minimal-compile-execution-approval-astra.v2.json`에 함께 발급하는 방안을 채택 후보로 둔다. 별도 새 sourceApproval 사본을 다시 요구하지 않는다. 기존50BDD와B407는 그 파일에서 기준 증거로만 명명한다.
6. 실제 호출 도구 심사 뒤 Root만 새 Input→Editor를 실행한다. 실패/원시/종료/입력 불변을 독립 결과 검수하고, 기존Movement 실제 성공과 새2 결과를 정확 생성 계보로 연결해 최소 컴파일 범위만 판정한다.

신규 계열은 helper/도구 계획/실제 계획/도구QA/계획QA의 v4, checkpoint v9, Root 실행 소유/native 승인 v2, Library의 r4다. 새 실제 helper4/준비/계획/검수/Root 소유/승인v2/실행 접두사의 정확 경로·최종 SHA는 실제 작성 종료 때 닫는다. 현재 helper3·plan3·승인1·기존 원문은 수정0이다. 현재 proposedCheckpoint/actualDelta/actualPlan/toolReview/planReview/rootOwnership/nativeApproval의 새 지문은 모두 `null`이며 sourceAcceptedNow/nativeExecutionAuthorizedNow는 아직false다.

실제 Native 단계는 PlanReview4 한 파일에서 현재 source2 delta/588 불변·Movement 재사용·currentCheckpoint·helper/plan/toolQA4를 직접 결속하고 결합승인v2의 `sourceLimitedAcceptedNow`·native 권한·Root 실행 소유v2를 필수 대조한다. 별도 SourceQA4/SourceApproval2 파일이나 그 매개변수를 새로 요구하지 않는다. 새 실행 가드는 결합승인v2의 현재 checkpoint·delta 검수·도구/계획/소유·실제Movement 결과 및 ref·개정 권위·예산을 대조해야 한다. 역사 source QA/승인을 current source 권위 필드로 혼용하지 않는다. currentInput590 전체 스냅샷은 필요한 실제 실행 가드에서 확인하되, 별도 독립 sourceQA 단계마다 같은590을 반복 해싱하지 않는다. 실제 경로/자료형·명명 핀의 국소 확인을 먼저 하여 준비를 실패 재현 도구로 쓰지 않는다.

### 10.6 추가 예산과 소비 의미

**REQ-M1COMP-R1-005:** 제안은 **새2 native 시작·Input→Editor·각60초/합산120초·종료5초/회수5초·단일 Execute·첫 실패 후 남은0·자동 재시도0**다. Unity/GUI/Player 조건/실제게임/serializer 런타임 추가 실행0, compiler server0이다. 정확2 성공이 필요하므로 이 새 묶음도 실패 뒤 자동 재개 여유가 없다.

이는 기존 최대3/180의 남은1이나 재개가 아니라 **추가 예산 개정**이다. 기존 actual2/세션92.8710416초·단계 보고합36.5900427초와 새 실제 소비를 별도 열로 누적한다. 기존 시계를0으로 만들거나 새2를 원래3에 포함된 것으로 표시하지 않는다. 추가2/120의 채택은 현재 없음이며, 실제 API/형식 영향과 독립 검수·Root 소유를 닫은 Astra 새 승인v2 이후에만 시작한다. 별도 설치/유료API/보안·앱 동의 우회 권한을 추가하지 않는다.

helper4는 기존 정수 가드·Property·ReadPinned·프로세스 관리 본문을 보존하고 단계 목록·시작 Count·결과/상한의 모든 숫자를 새2/120에 일관되게 맞춘다. 미소비 helper4의 국소 타입/파서/경로 보정은 같은 파일·원장 안에 누적할 수 있게 설계하되, 소스·권위·실제 소비 계획이 닫힌 뒤에는 임의로 바꾸지 않는다. 실제 첫 실패는 원시 보존→원인/최소 재현→영향 수정→필요한 새 유한 권위 판단 순서이며, 여러 작은 실행을 계획 밖에 숨기지 않는다. 불필요한 승인/검수 파일 반복을 줄이는 것과 native 예산 경계 유지가 동시에 필요하다.

### 10.7 한 작업 원장·변경되는 권위의 정확 범위

**REQ-M1COMP-R1-006:** Root의 기존 `evidence/b1-basic-movement-minimal-compile-work-ledger-root.v1.json`에 실패 원인·정확2 보정·국소 확인·현재 RuntimeInput·재사용 근거·새 예산/소유·실제 결과를 누적한다. 작성 중 원장 전체 SHA를 실행 계획에 요구하지 않으며 검수/승인 자기 SHA·미래 문서 SHA·일반 설명 지문의 재귀 결속0이다. 실제 소비 입력과 승인된 권위 개정만 비순환으로 고정한다.

발효에 필요한 호환은 G3 §2/§10.3의 매 보정별 소스 한정 판정 주기를 **정확2 delta가 포함된 같은 Luna 실제 계획 검수와 Astra 결합승인v2**로 처리하는 부분, 현재 v3의 고정 source 가드를 역사/현재 분리된 helper4 계약으로 바꾸는 부분, §9의 소진된 고정3 시작 밖에 새2/120을 명시 승인하는 부분이다. G3 §8/§10.2의 독립 영향/도구/정확 입력·별도 native 권위·실제 호출 심사 선행은 유지한다. 단순 프로세스 전략을 근거로 충돌하는 source 가드를 생략하지 않는다.

이 기술 호환은 승인된 관측 식별값을 실제 Unity API와 손실 없는 원문 형식으로 표현하기 위한 후보다. 기존 게임 행동/프로필/영속 포맷을 바꾸지 않는 범위이면 사용자 재승인을 일괄 요구하지 않고 아스트라가 근거·권위 경계를 판정한다. 실제 추가 native는 아스트라 예산 개정과 도구 심사 대상이며 자동 허용이 아니다. 별도 GUI 동의/연속 관측 예외·Q0 인간 결정은 그대로 대기한다.

### 10.8 인수 기준·미결·종료

| AC | 연결 REQ·독립 확인 범위 |
| --- | --- |
| AC-M1COMP-R1-001 | REQ001: 정확2와 식별필드10/reader8의 기술 호환만 변경되고 원래 reader3/meta/Movement 인터페이스/실제 참조 신원·게임/프로필은 불변이다. 실제 API·형식/버전 정책 근거가 닫혀 있다. |
| AC-M1COMP-R1-002 | REQ002: UInt64 전체 범위·필수 존재/실제 Number·정수 리터럴·중복/잘못된 형식 거절과 configuration 재로드가 같은 형식으로 연결된다. 정적 근거와 실제 runtime 미실행을 구별한다. |
| AC-M1COMP-R1-003 | REQ003: r3 Movement 실제 성공/출력과 소스/RSP/자원 불변·현재핀의 독립 재사용 판정, 새 Input→Editor ref 연결을 재현할 수 있다. Movement 재컴파일/실패Input 재사용0이다. |
| AC-M1COMP-R1-004 | REQ004: native0 Prepare 뒤 같은 독립 묶음의 실제delta/도구/계획 검수, Root 소유, source+native 결합승인v2의 비순환 순서와 helper 가드 일치를 확인한다. 과거 QA의 새source 허위 적용0이다. |
| AC-M1COMP-R1-005 | REQ005: 과거 소비와 추가2/120이 구별되고 각60/정리5·5·단일호출/첫실패/자동retry0이 실제 권위·도구·결과에 일치한다. 추가 승인 전 native0이다. |
| AC-M1COMP-R1-006 | REQ006: 같은 원장과 명시 권위 호환으로 작은 전체검수/승인 사본 반복을 줄이며 기존 G3·35/44/수동12/종단4/빌드1·9회/Q0/키세대/프로필/원격·B1 FINAL 전 C1C 통합0을 유지한다. |

기존 REQ/AC-C1B-G3OBS-001/005/008/010/012 및 REQ/AC-PROC-011/012를 함께 추적한다. 현재 이 절의 실제 AC 통과0, source 보정0·새 checkpoint0·새 helper/계획/승인0·추가 native0이다. 솔의 실제 API/JsonUtility 조사와 루나 actual failureQA가 닫힌 뒤 빠른 범위·위험 반대검토로 구현/준비 승인 여부를 결정한다. 승인 문서가 다시 승인 문서를 요구하는 별도 순환을 추가하지 않는다. 이 절 작성 종료 뒤 문서 소유를 Root에 반환하며 기존 §9 승인 바이트와 실행 실패는 역사대로 보존한다.
### 10.9 정확2 소스·도구4·실행 없는 준비의 한정 채택 — Approved

- 채택일/실제 승인자: 2026-10-08, 아스트라 `gpt-6-astra/ultra`.
- 승인 대상은 §10.2~10.7의 **정확2파일 기술 호환과 helper4/도구 계획4 작성, Root의 실제 checkpoint v9 및 native0 Prepare**다. 이 절의 `finalApproval=true`·`boundedPreparationApproved=true`는 그 준비 범위에만 적용한다. `sourceLimitedAcceptedNow=false`, `movementReuseIndependentlyAccepted=false`, `nativeExecutionAuthorizedNow=false`, `unityExecutionAuthorizedNow=false`, `guiExecutionAuthorizedNow=false`, `actualSerializerRoundtripExecuted=false`, `actualParserCounterexamplesExecuted=false`를 유지한다.
- 검토된 원문은 이 절 앞의 정확51698바이트/SHA `D96416843DE46086A9752D2F8C27D1C87FEF740EE4E66CB7F4048DB9619F9246`다. 이를 바이트 접두사 그대로 보존한다. §1~9의 이전29693바이트/SHA `6BDC84F87176CCE5B52CD5DAA68A1D9EA682A09E45E68D72CED6B577402ECAD7`와 §10.1에 명명한 보존 사본도 유지한다. 본 채택은 앞선 Review 작성 당시의 미발급 상태를 소급 수정하지 않고, 아래 준비 범위만 이후 발효시킨다.
- 최종 사전 반대검토는 실제 루나 `gpt-6-luna/max`가 D964로 닫힌 §10을 읽고 Root가 전달한 작성 종료 메시지다. 정확2/UInt6410/reader8·기존 signed/서수/32참조/영수증/키/세대/tick/liveScene·588 불변·RSP/Movement 계보·native0 준비를 승인 가능한 범위로 판독했다. 별도 보고 파일이나 그 SHA를 임의로 만들지 않는다. 이 검토는 최대값 왕복·실제 런타임·미래 source delta/계획4의 수용이 아니다.
- 실제 실패 결과 검수 원문은 `output/chapter1-playable-r1/evidence/b1-basic-movement-minimal-compile-result-review-luna6.v1.json`, 15118바이트/SHA `5454B06F4D91CE99D2EA58BBD22F4B04270AECE461FBCCA03F5B43B7814C3467`, 실제 루나 `gpt-6-luna/max`, 정수 P0/P1=0/2다. 원래 r3 전체 실패·P1-COMP-001/002·최대3 중 실제2 시작·첫 실패 정지와 전체 최소 컴파일 거절은 그대로다. 이번 준비 승인으로 지적 폐쇄나 실행 성공을 발급하지 않는다.

#### 10.9.1 쓰기 경로·소유 전이

아래 경로만 준비 쓰기에 허용한다. 기준은 저장소 루트다.

| 담당과 허용 파일 | 실제 허용 범위 |
| --- | --- |
| Sol 소스 담당: `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs` | Configuration 식별2와 SampleRow 식별8의 UInt64 필드·공개 API 생산·기존 직렬화 경로에 필요한 타입 호환만 |
| 같은 Sol 소스 담당: `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs` | 설치2의 API 생산과 표본8의 엄격 UInt64 읽기만 |
| 별도 Sol 도구 담당: `output/chapter1-playable-r1/tools/b1-basic-movement-minimal-csc-root.v4.ps1` | 소비된 v3를 보존한 새4의 역사/현재 source 가드·native0 Prepare·Input→Editor 2단계·실제 Movement 참조·새 유한 예산·결합 승인 소비를 구현 |
| 같은 Sol 도구 담당: `output/chapter1-playable-r1/preparation/b1-basic-movement-minimal-compile-tool-plan-sol61.v4.json` | 위 helper4의 현재 설계·정확 입출력·검사/예산·미실행을 같은 기능 묶음으로 기록 |
| Root: `output/chapter1-playable-r1/evidence/b1-final-source-checkpoint-sol61.v9.json` | 소스 담당 쓰기 종료·반환 후 실제590 경로/길이/SHA, v8 대비2 변경/588 불변, 실제 Root 작성자와 계보를 기록 |
| Root: `output/chapter1-playable-r1/preparation/b1-basic-movement-minimal-compile-plan-root.v4.json` | 국소 가드 확인 뒤 실제 native0 Prepare의 새 계획1을 CreateNew 생성 |
| Root: `output/chapter1-playable-r1/project/Library/b1-minimal-csc-g3-observation-r4/AcadeGameMaker.Input.Unity.rsp`, 같은 디렉터리의 `AcadeGameMaker.Combat.Authoring.Editor.rsp` | 위 Prepare가 새 r4 디렉터리에서 생성하는 정확 응답2. 현재 단계의 DLL/ref/PDB/컴파일 로그/실행 시작·결과 원시 생성0 |

소스의 승인 전 기준은 v8 `58E1E09A8CD5E1ADBE3350DB5BFB648824CAE9393823569BC51BCD6D415A0F31`다. 직접 재확인한 Observer는39189바이트/SHA `18A4E18B6D3336FE89599384A28B1847019B060C6B07FDC87D0FC2AC6628D66E`, Window는62832바이트/SHA `15B958FA392A1C72F978CF85FEA03ACF951084B10ABAE3D3AB478D44B6C54927`다. 메타·추가 파일·다른588·기존7의 나머지5에 대한 제품 쓰기는0이다.

현재 소스 소유는 Root다. 이 승인은 소유 전이를 완료했다고 표시하지 않는다. Root가 정확2를 `/root/sol61_g3_uint64_source` 실제 `gpt-6.1-sol/ultra` 한 명에게 명시 인계한 뒤 그 담당만 쓴다. helper4/도구 계획4는 별도 `/root/sol61_g3_compile_tool` 담당에게 두 경로만 인계하고 실제 참여 모델/설정을 기록한다. 두 담당의 파일은 겹치지 않는다. 각 담당은 쓰기 종료·지문을 Root에 반환하고, Root가 sourceWritingClosed/단일 소유를 닫은 뒤 checkpoint/Prepare를 수행한다. 승인자인 아스트라의 제품·도구·원장·실행 쓰기는0이다.

#### 10.9.2 채택된 기술 경계

Configuration의 `installedInstanceId`, `installedSceneHandle` 및 SampleRow의 `hostId`, `routerId`, `hazardsId`, `playerId`, `combatId`, `transferId`, `collectorId`, `sceneHandle`만 ulong으로 표현한다. 객체는 `EntityId.ToULong(obj.GetEntityId())`, scene은 `scene.handle.GetRawData()`를 사용한다. 원래 Unity 객체와 세션 내 관측 원문을 나타내며 영속 키·세션 간 신원·비트 배열 안정성 권위로 사용하지 않는다. 기존 schemaVersion/필드명/field serializer·32참조표/ReferenceEquals·영수증/RunKey/Generation/tick/liveScene 비교·signed 정수/서수/프레임값은 불변이다.

Window는 해당8필드를 기존 Required/문서 검증 뒤 단일 내부 `UInt64Integer`로 읽는다. 원문 숫자가 0 또는 0이 아닌 숫자로 시작하는 비음수 십진 정수 리터럴인지 확인하고 `TryGetUInt64`로 전체0~18446744073709551615 범위를 확인한다. 누락/null/문자열/bool/배열/객체/음수/음수0/소수/지수/범위 초과/중복키·잘못된 JSON은 거절한다. 기존 `Integer`/`LongInteger` 본문과 다른 projection·엄격 문서 가드·원시 상한은 그대로 둔다. 새 serializer/참조/프레임워크/테스트 파일/selector/생산 hook·정렬 경고 보정은 추가하지 않는다.

정확2 보정과 도구4 내부의 타입·파서·경로 국소 확인은 같은 작업 원장에 누적하며 작은 수정마다 전체 SourceQA/상태 사본을 새로 만들지 않는다. 이는 C# 컴파일·Unity/Player 조건·serializer 런타임 검사를 허용하지 않는다. 실제 최대값 직렬화 왕복과 Configuration 재로드/sample8은 여전히 미입증이므로 **AC-M1COMP-R1-002의 런타임 수용은 미통과**다. 최초 승인 Play 계획에서 비영속·비제품 확인 방법과 원시 근거를 구체화하지 못하면 해당 AC는 미해결로 남긴다. 실제 Unity ID/관측 sample에 최대값 주입0·예비 Play0·GUI 동의/연속 관측 예외 우회0이다.

#### 10.9.3 Prepare와 후속 수용의 비순환 순서

이 채택은 G3 §2/§10.3의 이번 보정 소스 확인을 §10.5의 같은 Luna 계획4 검수에 포함한 delta2/588 불변·Movement 영향 판독과 Astra 결합승인v2로 처리하도록 한정 호환한다. 기존 v3의 고정58E1/B407/50BDD를 바꾸거나 새 소스 수용으로 재사용하지 않는다. helper4의 Prepare는 현재 v9·이 준비 권위·Root 쓰기 종료·역사 v8 대비2/588·실제 r3 Movement 개별 성공과 현재 ref핀을 확인한다. 미래 toolQA4/planQA4·owner2·native 승인v2는 Prepare의 선행 입력이 아니다. 따라서 준비 자체에 아직 없는 독립 수용을 요구하지 않지만, Prepare 성공 역시 독립 소스 수용을 발급하지 않는다.

원본26AB RSP 기준 Input[1]4행·Editor[2]5행 변형을 그대로 재구성한다. 양쪽의 Movement는 r3 실제 ref10240바이트/SHA `0920429D88AADE31BC34B8F6A8E6CC27C70F16A249E8B9657755634A6EFADBAB`를 참조하고 Editor의 Input 참조는 새 r4 Input.ref 경로다. 기존 Movement 성공은 부모 실행의 개별 근거로 분리하여 준비에서 직접 대조한다. 새로운 delta/참조 영향에서 재사용이 유효한지는 Luna 계획QA4가 독립 판정한다. Movement 재컴파일·과거 실패 Input 재사용·Movement를 새2회 결과에 끼워 넣기0이다. 향후 새 Input ref SHA는 실제 성공·폐쇄·회수 후 생성하여 Editor 직전 확인하도록 도구를 준비한다.

소스/도구 쓰기 종료→Root 실제 v9/국소 검사→native0 Prepare→Luna 한 과업의 toolQA4 종료→실제 planQA4 종료 순서를 사용한다. planQA4 한 파일은 현재 source2 delta·588 불변·Movement 재사용·currentCheckpoint/helper/plan/toolQA4를 결속한다. 별도 SourceQA4/SourceApproval2 파일·매개변수를 추가하지 않는다. 그 뒤 Root owner2와 Astra의 CreateNew `preparation/b1-basic-movement-minimal-compile-execution-approval-astra.v2.json` 한 파일에 `sourceLimitedAcceptedNow`와 별도 native 권한을 함께 판정한다. **현재 그 승인v2를 발급하지 않으며 실제 v9/계획/QA/소유 지문을 미래값으로 채우지 않는다.**

미소비 helper4는 기존 정수 가드·Property·ReadPinned·프로세스 관리 본문을 보존하고 준비/실행 목록·Count·예산·결과를 정확2에 일관되게 맞춘다. source 가드를 끄거나 예외로 통과시키지 않는다. 독립 도구/정확 실행 입력/Root 단독 실행/별도 실제 호출 심사 등 G3 §8/§10.2 선행은 유지한다. Root의 기존 단일 작업 원장에 현재 RuntimeInput·국소 확인·변경 이유·역사/실제 소비를 누적하며 자기 SHA/설명 문서의 재귀 결속0이다.

#### 10.9.4 추가 실행은 미승인·종료

도구에 준비하는 다음 유한 계획은 **새 Input→Editor 2 native 시작·각60초/합산120초·종료5초/회수5초·단일 Execute·첫 실패 후 남은 시작0·자동 재시도0**다. 이 숫자는 구현할 가드와 향후 별도 승인 상한이며 현재 실행권이 아니다. 원래 r3의3/180 중 남은1이나 재개로 계산하지 않는다. 기존 소비2/세션92.8710416초/단계합36.5900427초와 향후 새 소비를 같은 원장의 별도 항목으로 보존한다. 현재 native0·Unity0·Play0·GUI0·Player 조건0·프로필IO0이다.

REQ/AC-M1COMP-R1-001~006과 기존 REQ/AC-C1B-G3OBS-001/005/008/010/012·PROC-011/012의 구현·준비 경계만 채택한다. 실제 source/compile/runtime AC 통과를 발급하지 않는다. 실제6방 관측·정확6 적용·35/44·수동12·종단4·현재 빌드1·9회·Q0·키세대/프로필/원격·B1 FINAL 전 C1C 통합0은 불변이다. 대기 중 GUI 인간 결정은 이 API 기술 호환에 별도 사용자 승인을 요구하는 근거로 확대하지 않는다.

본 준비 승인 쓰기 종료 후 문서 소유를 Root에 반환한다. 다음 행동은 Root의 명시 소유 인계이며 그 전 소스/도구 쓰기는0이다. 아스트라는 현재 부록의 이 추가 승인만 작성했고 기존 승인1·실패·input/source v8·D964 검토 접두사·6BDC 보존 사본을 수정하지 않았다.


### 10.10 첫 R01-A의 UInt64 순수 진단과 필요한 Editor 단일 컴파일 후보 — Review

- 상태: **Review / Approved=false**. 작성자는 아스트라 gpt-6-astra/ultra, 2026-10-08이다. Root의 실제 모델/설정은 gpt-6.1-sol/medium으로 보존한다. 이 절은 구현·Prepare·컴파일·GUI·진단 실행 권한이나 독립 수용을 발급하지 않는다.
- 사용자 동작/목적: 기존 첫 R01-A의 정상 Play에서 tick0부터 연속 수집된 실제 원시를 보존하고, 실제 Pause 중 설치 설정2의 재로드 및 표본 식별8의 숫자 형식을 확인한다. 경계값은 분리된 DTO에서만 검사하며 실제 Unity 식별자·게임 입력·상태를 조작하지 않는다.
- 추적: REQ/AC-M1COMP-R1-001~006, REQ/AC-C1B-G3OBS-005/008/010/012, REQ/AC-PROC-011/012. 기존 UInt64 형식·필수 AC·실제 플레이 관문을 바꾸거나 새 게임 검사 선택자를 발급하지 않는다.
- 원문 보존: 이 절 앞의 정확63289바이트/SHA 8F42DC07E099BD0A5FD3700A303C1565C97E0D9836805C1E41C82BA559DE6067를 바이트 접두사 그대로 보존한다. 이미 소비된 승인v2·helper4·계획4·r4 원시가 명명한 부록은 그 당시 접두사 원문이다. 현재 전체 문서 지문으로 역사 입력을 덮지 않으며, 정확 길이와 접두사 SHA로 복원·대조한다. 별도 승인 사본·상태 보고·문서 SHA 재귀 결속을 추가하지 않는다.
- 현재 사실: Root는 session27901의 Execute 종료·정수 exit0·두 컴파일 성공을 통지했다. 작성자는 이 과업에서 r4 실행 원시를 읽지 않았다. Root의 종료/회수/source-after 확인과 Luna 독립 결과 검수는 별도 실제 근거다. 이 절의 r4 결과 수용 SHA·실제 Input ref SHA·후속 checkpoint/계획/QA/승인 SHA는 모두 null/미발급이다. Root의 통지만으로 부모 재사용이나 새 Window 컴파일 성공을 발급하지 않는다.

#### 10.10.1 정확 제품 변경·호출·소유 후보

제품 변경은 다음 기존 C# 정확1파일뿐이다.

output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs

읽기 기준은 실제590의 v9 checkpoint SHA 28F1B590C504030F05C43BC599FD5B476C2E2A27F0B69E2D8DF48BB35F3FDBCE다. 현재 Window는63841바이트/SHA 92685D0BF7D3F9041C2F6E27E97347405EFD664654D55E23A81D3A6F20D63746다. 후속 기대 차집합은 이 파일1 변경·다른589 불변·추가0·삭제0이며 미래 실제 지문은 발급하지 않는다.

기존 EditorWindow 안에 private 단회 버튼 처리와 private 순수 진단·결과 보유만 추가한다. 호출 경계 후보는 private void RunUInt64DiagnosticsOnce()와 private static UInt64DiagnosticResult RunUInt64Diagnostics(string configurationJson, string sampleJson)다. UInt64DiagnosticResult는 같은 파일의 private 중첩 결과 DTO다. 범위 제한 원시 읽기·비교·결과 직렬화는 같은 파일의 private 구현으로만 둔다. 새 public/internal 생산 API·메뉴·C#·meta·assembly·friend·참조·패키지·serializer는0이다.

기존 ParseStrictDocument/Required/UInt64Integer/ReadSampleProjection/Integer/LongInteger 본문과 정상 판독·방 적용 동작은 변경하지 않는다. 새 버튼은 “숫자 원시 형식 검사” 하나다. 기존 TryPrepare 안에서 실제 Configure에 전달할 Configuration 설치2·session/case만 창 자체의 직렬화된 원시 문자열에 보관할 수 있다. 기존 EditorWindow 재로드 메타데이터와 같은 수명이며 EditorPrefs/SessionState/자산/프로필 저장0이다. 실제 Configuration의 전달값과 Observer 구성·검사·writer는 바꾸지 않는다.

아스트라의 범위 채택과 Root의 명시 인계 뒤 Sol 소스 담당 한 명만 Window를 쓴다. 별도 Sol 도구 담당은 helper5/도구 계획5만 쓴다. 쓰기 종료·Root 반환 후 현재 입력을 확정한다. 이 Review 시점에는 sourceWriting 권한·sourceLimitedAcceptedNow·runtimeSourceAccepted·sourceReady·GUI 권한을 발급하지 않는다.

#### 10.10.2 실제 재로드·표본과 경계값의 분리

정상 증거는 기존 Observer start event와 실제 sample 원문이다. start event의 Configuration은 실제 Start의 JsonUtility.FromJson<Configuration> 및 기존 Validate 뒤 기록된다. 그 설치2를 창이 Play 전 보유한 실제 전달값과 대조한다. 실제 sample8은 기존 EntityId.ToULong/SceneHandle.GetRawData 생산·JsonUtility.ToJson 원문을 기존 ReadSampleProjection으로 읽는다. 설치 시 ID가 장면 재로드 후 실제 객체 ID와 항상 같다고 가정하지 않는다. 설정 값의 재로드 보존과 실제 표본 생산을 따로 기록한다.

진단의 사본은 정상 원문에서 만든 분리된 Observer.Configuration/Observer.SampleRow DTO다. collector, 실제 configuration/session, _configurationJson, _latestJson, Unity 객체·scene/transform, 정상 sample/events/seal 원시에 진단값을 대입하거나 대체하지 않는다. 첫 실제 sample은 읽기 전용으로 보존한다. 신규 source/세대/receipt/capture를 진단에서 발급하지 않는다.

정확 필드 순서는 Configuration의 installedInstanceId/installedSceneHandle 2개, SampleRow의 hostId/routerId/hazardsId/playerId/combatId/transferId/collectorId/sceneHandle 8개다. 경계값은 0, 4294967296, 9223372036854775808, 18446744073709551615 정확4개다.

- 각 값마다 config 사본의2와 sample 사본의8을 그 값으로 설정한다. 기존 JsonUtility.ToJson 및 FromJson으로 두 DTO를 각각 왕복한다. 따라서 직렬화 DTO 묶음은 config4+sample4=8개, 경계 왕복 부분의 ToJson8회/FromJson8회, 검증하는 필드·값 쌍은 config8+sample32=정확40개다.
- 각40개 쌍은 기대 ulong, 실제 ToJson의 원문 정수 표현, 실제 FromJson 필드값, strict 판독 결과가 모두 일치해야 한다. config2는 같은 기존 ParseStrictDocument→UInt64Integer, sample8은 같은 기존 ReadSampleProjection 내부의 UInt64Integer를 사용한다. 사본을 검사한 사실을 collector의 실제 식별자로 기록하지 않는다.
- 정상 config2/sample8의 원시 비교와 경계40 비교는 서로 다른 결과 집합이다. 4값 검사가 모든 UInt64 값의 전수 실행이라는 주장은0이다.

아래 반례19종은 위10필드 각각에 적용해 정확190개 최소 문서를 만든다. 각 문서는 해당 실제 필드명 하나와 아래 값/구조만 가진 분리된 문자열이며 Configuration/SampleRow 전체나 정상 원시를 변조하지 않는다. 같은 ParseStrictDocument→UInt64Integer(root,해당필드명)를 직접 호출한다. config 필드2×19=38회, sample 필드8×19=152회다. N16 같은 루트 거절도 각 호출 행에 동일 규칙으로 기록한다. 이는 공유 strict 함수의 음성 검사이며, 전체 sample DTO 반례를190개 실행했다고 주장하지 않는다. sample8의 실제 투영 연결은 정상 표본 및 위40개 양성 비교가 확인한다.

| 번호 | 최소 문서 규칙 |
| --- | --- |
| N01 | 빈 객체: 해당 필드 누락 |
| N02 | 해당 필드 값 null |
| N03 | 해당 필드 값 문자열 "0" |
| N04 | 해당 필드 값 true |
| N05 | 해당 필드 값 [] |
| N06 | 해당 필드 값 {} |
| N07 | 해당 필드 숫자 -1 |
| N08 | 해당 필드 숫자 -0 |
| N09 | 해당 필드 숫자 0.0 |
| N10 | 해당 필드 숫자 1e0 |
| N11 | 해당 필드 숫자 18446744073709551616 |
| N12 | 해당 필드 숫자 표기 01 |
| N13 | 해당 필드 숫자 표기 +1 |
| N14 | 같은 이름의 필드2개, 값0과1 |
| N15 | 같은 이름2개 중 둘째 이름의 첫 ASCII 문자만 JSON 유니코드 escape로 표기, 값0과1. 예: hostId와 \u0068ostId |
| N16 | 객체 대신 루트 [] |
| N17 | 해당 이름과 콜론 뒤 값·닫기 없는 불완전 JSON |
| N18 | 해당 필드0 뒤 후행 쉼표가 있는 객체 |
| N19 | 해당 필드 값0 앞에 /*진단*/ 주석이 있는 객체 |

정확 순서는 경계4의 config/sample40 비교 뒤 N01~N19 각각 필드10 순서의190 음성이다. 총 scalar 판정은 정상 원시10개와 별도로 **양성40+음성190=230개**다. 새 게임 선택자·Play 횟수·기존15/35/44 실행 수를 늘리는 검사가 아니다.

예상 거절 성공은 System.IO.IOException 또는 System.Text.Json.JsonException만 허용하고 실제 예외형을 구분한다. 다른 예외·무예외 수용·기대값 불일치·직렬화 절단/부호 변화는 첫 실패다. 예상 예외 자체는 음성 검사의 정상 결과이며 로그를 숨겨 게임 실패를 성공으로 바꾸는 예외가 아니다. 첫 예기치 않은 실패 뒤 나머지는 미실행으로 기록하며 반복0이다.

#### 10.10.3 첫 Play·Pause·재로드 후 단회 경계

기존 첫 R01-A Play 단1회에서 첫 실제 완료 tick0의 원문과 Pause까지의 연속 수집이 확인된 뒤에만 수행한다. _armed/_playEntered·현재 case/session·단일 원래 collector Live·기존 실패0·isPlaying && isPaused·compiling/updating0가 필수다. Pause 순간의 현재 tick까지 반드시0이어야 한다는 조건은 추가하지 않는다. 첫 tick을 강제로 만들거나 누락을 진단으로 복구하지 않는다.

private 단회 상태는 창의 기존 재로드 메타데이터와 함께 직렬화한 attempted 및 attemptedSessionId로 보유한다. 버튼은 같은 session에서 시도를 시작하기 전에 이를 소비한다. 단순 OnEnable/OnGUI/창 재열기에서 초기화하지 않는다. 정규 최초 TryPrepare에서 아직 수행하지 않은 새 승인 사례를 결속할 때만 새 상태를 정하며, 첫 R01-A의 재실행 권한은 만들지 않는다. 여기에 고정 진단 경로의 CreateNew 예약을 결합한다. attempted=true이거나 같은 고정 파일이 이미 있으면 검사0·거절이다. IO 실패로 빈 파일만 남아도 소모된 시도이며 삭제/다른 이름으로 재실행0이다. 창 종료/재컴파일/재로드로 실제 수집기가 실패하면 기존 첫 실패 규칙이 우선한다.

승인된 첫 사례의 자기 start/sample 경로만 읽는다. 활성 writer가 있는 자기 원시는 FileShare.ReadWrite로 제한 읽기하며 쓰기·truncate·rename·replace0이다. 읽기 전후 길이 일치, 완전한 레코드·엄격 UTF-8·동일 session/case를 확인한다. start는 기존8MiB 상한, sample 레코드는32KiB 상한 안이다. 부분 줄·길이 변화·중복 start·다른 세션이면 첫 실패로 거절한다. 기존 ReadStrictDocument의 FileShare.Read로 활성 writer를 읽을 수 있다고 가정하지 않는다. 각 레코드의 경로·offset/length·SHA와 읽기 전후 길이를 보유한다.

Pause/중립/창 포커스/버튼/Game 복귀/Resume·Stop의 실제 사건은 기존 연속 native/input 원시에 남긴다. 버튼이 Play/Pause/Resume/Stop을 호출하지 않고 Root만 승인된 GUI에서 조작한다. FixedUpdate/Advance/Physics.Simulate/PlayerLoop·가상 입력·게임 getter 신규 호출·합성 sample 발행0이다. 현재 원시 표시는 기존 수집 문자열만 사용한다.

Pause·진단·포커스 전환·원시 기록 시간은 기존 사례1800초/전체18000초에 계속 합산한다. 별도 시간 연장0이다. 동기 진단 호출의 국소 상한 후보는5초이며 각 유한 단계 전후 실제 경과로 확인하고 초과는 실패다. 동기 Unity API를 도중에 강제 중단할 수 있다는 보장은 하지 않는다. 추가 Play·예비 Play·Unity 시작·재진단0이다. 진단 첫 예기치 않은 실패도 G3 최초 실패이므로 원시 보존·Root Stop·나머지 사례0을 적용한다. 기대 실패 이후 정상 게임을 계속하도록 표시하지 않는다.

#### 10.10.4 진단 JSON1과 정상 원시의 작성 경계

후보 경로는 output/chapter1-playable-r1/evidence/b1-basic-movement-uint64-runtime-r1.json 정확1이며 현재 생성·결과 발급0이다. 정상 sample/events/seal의 collector 단독 writer는 그대로 유지한다. Window가 이 별도 진단 JSON1만 기록하는 예외는 같은 범위에서 명시 채택해야 한다. 정상 원시 스트림을 공동 사용하거나 진단을 정상 표본에 합치지 않는다.

단회 처리에서 고정 경로를 CreateNew로 예약한 뒤 유한 결과를 메모리에 보유하고 엄격 UTF-8 JSON1로 기록·flush·close한다. 전체32KiB 상한, 덮어쓰기/삭제/재시도0이다. IO 실패의 빈/부분 파일도 실패 원문으로 보존한다. 자기 JSON SHA는 본문에 쓰지 않는다.

32KiB 안에서 입력 규칙19와 필드명10의 사전을 한 번만 기록하고, 양성40/음성190은 번호·필드 인덱스·예상형·실제 ulong 또는 예외형 코드·결과·수행 여부의 최소 행으로 기록한다. 실제 직렬화 원시 숫자와 기대 ulong은 손실 없는 십진 문자열 또는 엄격 정수로 구별한다. 예외 메시지는 같은 종류의 원형을 제한 길이로 보유하고, 첫 예상 외 실패는 원래 예외형과 메시지를 한 번 보유한다. 크기가 넘치면 자르거나 성공으로 처리하지 않고 실패다. full DTO/configuration590행/sample 전체/반례 전체 JSON190개를 복제하지 않는다. 변환 규칙·행 번호와 선택된 정규 레코드 핀으로 독립 재구성이 가능해야 한다.

필수 메타데이터는 schemaVersion/kind, 실제 checkpoint·Window/Observer·실행 승인·진단 범위 권위의 입력 핀, Unity PID/start ticks/EXE/version/project/session/case, 실제 Pause·first/current tick, 정규 원문 레코드 핀/안정 길이, 시작·종료·경과·첫 실패·시도 소비·파일 폐쇄다. 단계별 값/명시 필드/실제 타입과 수행하지 않은 행을 보존한다. 성공 bool만으로 검사 원문을 대신하지 않는다.

passed는 형식 진단만 뜻한다. runtimeSourceAccepted/independentAcceptance/actualRoomAccepted/gameplayAccepted는 파일 작성 당시 false다. 정상 재로드2·실제 표본8과 사본 경계40·반례190을 구분한다. 게임 경로·위험·native/input 연속성·완료 키를 진단으로 수용하지 않는다. Luna 실제 판독과 Astra 한정 판정 전 AC002는 미통과다.

#### 10.10.5 필요한 준비·단일 Editor 입력의 정확 경로 후보

Window 변경 후 현재 소스의 컴파일 근거는 새로 결속한다. 소비된 helper4/계획4/승인v2/r4 원시는 덮지 않는다. 아래 경로는 범위 Approved 및 명시 인계 뒤에만 허용할 후보이며 현재 파일 생성 권한0이다.

| 담당 | 정확 경로 후보 |
| --- | --- |
| Sol 소스 단독 | §10.10.1 Window.cs1 |
| 별도 Sol 도구 단독 | output/chapter1-playable-r1/tools/b1-basic-movement-minimal-csc-root.v5.ps1 및 preparation/b1-basic-movement-minimal-compile-tool-plan-sol61.v5.json |
| Root 현재 checkpoint | output/chapter1-playable-r1/evidence/b1-final-source-checkpoint-sol61.v10.json |
| Root native0 Prepare | output/chapter1-playable-r1/preparation/b1-basic-movement-minimal-compile-plan-root.v5.json 및 project/Library/b1-minimal-csc-g3-observation-r5/AcadeGameMaker.Combat.Authoring.Editor.rsp |
| Luna 같은 과업 순서 | output/chapter1-playable-r1/evidence/b1-basic-movement-minimal-compile-tool-review-luna6.v5.json 뒤 b1-basic-movement-minimal-compile-plan-review-luna6.v5.json |
| Root 소유 | output/chapter1-playable-r1/preparation/b1-basic-movement-minimal-compile-ownership-root.v3.json |
| Astra 결합 판정 | output/chapter1-playable-r1/preparation/b1-basic-movement-minimal-compile-execution-approval-astra.v3.json |
| Luna 실제 결과 | output/chapter1-playable-r1/evidence/b1-basic-movement-minimal-compile-result-review-luna6.v3.json |

표의 축약된 preparation/project 경로는 모두 output/chapter1-playable-r1 아래이며, 계획QA5는 바로 앞 toolQA5와 같은 evidence 디렉터리다. 독립 source 델타는 planQA5 한 파일에 포함하고 별도 SourceQA5/SourceApproval 파일·매개변수는0이다. 실제 모델은 소스/도구 Sol gpt-6.1-sol/ultra, Luna gpt-6-luna/max, Astra gpt-6-astra/ultra, Root 실제 gpt-6.1-sol/medium을 기록한다. 자기 구현 독립 수용0이다.

r5 실행 출력 후보는 그 디렉터리의 AcadeGameMaker.Combat.Authoring.Editor.dll/.ref.dll/.pdb 및 같은 조립체의 .start.json/.result.json/.stdout.log/.stderr.log, 세션 source-before.json/execution-start.json/source-after.json/result.json 및 실제 실패 때만 failure.json이다. RSP1 외 실행 원시는 별도 native 승인·실제 호출 심사 전 생성0이다. source-after는 모든 시작 프로세스의 실제 폐쇄·드레인 뒤에만 작성한다.

Root가 Window 쓰기 종료/반환 뒤 실제590의 v10을 작성하고 v9 대비1/589·추가삭제0을 기록한다. R4의 실제 Input 개별 성공·정수 exit0·retained handle 폐쇄/드레인·실제 ref 및 독립 결과 검수와 현재 Input 소스/입력 불변이 닫혀야 Input 재컴파일0·r4 Input ref 제한 재사용을 채택할 수 있다. Movement는 r3 개별 성공4E610C/ref092042의 기존277입력/source2 및 현재 영향 불변에 한정한다. r3 전체 실패39FF/P1두건/소비된3E8은 보존한다. r4 Editor 성공은 이전 Window 기준의 역사다.

새 Editor RSP는 닫힌 r4 Editor RSP에서 DLL/ref 출력 접두사 두 리터럴만 r5로 바꾸고, Input ref는 실제 r4·Movement ref는 실제 r3을 유지하는 최소 후보다. 현재 계획4의 소스13/참조318/define146은 비교 기준이며 actual RSP5에서 다시 확인한다. 다른 source/define/reference 확장은 자동 허용0이다. 미래 Input ref·RSP SHA를 만들지 않고 새 Window SHA는 실제 v10/RuntimeInput에 결속한다.

helper5는 명시 정수/bool·Property·ReadPinned·경로/중복·부모 성공/입력·프로세스 가드를 보존한다. Prepare 선행은 이 범위의 Approved·실제 v10·Root 쓰기 종료·부모 성공/검수/현재 핀이며 미래 QA5/owner3/승인v3는 요구하지 않는다. Prepare 성공은 source 수용이 아니다. 이후 Luna 계획QA5가 현재 source1/589·부모 Input/Movement 재사용·정확 컴파일 입력을 함께 판정한다. 실제 checkpoint 생성·Execute의 source 불변 검사는 유지하되 작은 보정마다 전체590 독립 재해싱을 반복하지 않는다.

#### 10.10.6 별도 Editor1 예산·측정·종료

추가 컴파일이 필요한 경우의 정확 후보 상한은 **Editor 조립체1·실제 native 시작 최대1·각60초/합산60초·종료5초/출력회수5초·단일 Execute·자동재시도0·재개0·두 번째 Execute0**다. Root만 실제 도구 호출 심사 뒤 실행한다. compiler server0, Input/Movement 재컴파일0, Unity/Player 조건 컴파일0, 예비 Player build0이다. 기존 r3 남은1이나 r4 추가2의 재개로 계산하지 않는다. 첫 실패 뒤 남은 새 native 시작은0이다.

새 도구의 단계 Stopwatch는 Process.Start 직전부터 실제 프로세스 종료 관측까지 측정하고, startedUtc/endedUtc도 그 같은 경계를 명명한다. PID의 실제 StartTime은 별도 processStartUtc/startTicks로 보유한다. 종료/드레인 소요는 각각 따로 기록한다. aggregate 시계는 Execute의 최초 원시/source-before 준비 직전부터 최종 종료·드레인·source-after/결과 기록 준비까지 연속으로 계수하여 대조 구간을 명시하고, 프로세스 대기는 min(각60,합산60의 실제 잔여)로 제한한다. 종료/드레인은 각5초 자체 상한도 동시에 적용하며 aggregate 성공 조건을 넘긴 결과를 성공으로 수용하지 않는다. 승인 검증·입력 해싱의 시각을 native startedUtc로 잘못 표시하거나 서로 다른 구간을 같은 elapsed라고 쓰지 않는다.

Root가 통지한 r4의 elapsed와 단계 UTC 간격 차이는 그 원래 결과 검수에서 사실대로 보존할 사항이며, 이 문서 작성자는 원시를 읽거나 그 수용을 대신하지 않았다. 미래 도구는 위 시작점/종료점/UTC/monotonic 경계를 명시해 같은 혼동을 피한다. 시간 초과·비정수/null exit·관리 오류·미회수·source/input 변화는 실패이며 원시를 지우지 않는다.

소스/도구 쓰기 종료→Root checkpoint/국소 확인→native0 Prepare→Luna 도구QA5/실제 계획QA5(소스 delta·재사용 포함)→Root owner3→Astra 결합 승인v3→Root 실제 도구 심사→Editor1→Luna 실제 결과 순서다. 허용된 경로·행동·인터페이스 안의 미소비 국소 보정은 기존 Root 단일 원장에 원인/최소 재현/변경/새 관측 목적을 누적한다. 새 범위·소비 후 재실행을 작은 수정으로 숨기지 않는다. 설명 문서·승인 사본·전체 SourceQA를 재귀 증식하지 않는다.

컴파일 수용 뒤에도 GUI 승인·Root 소유·앱 동의·연속 화면/키 증거 수단이 첫 R01-A의 실제 선행이다. GUI 승인에 이 단회 진단 경로/출력/기존 예산을 명명해야 한다. 현재 대기 중인 인간 질문2는 그대로이며, 이 기술 보정 자체를 새 사용자 제품 선택으로 확대하지 않는다. 실제 원시가 닫히면 기존 M1/G3 실제 결과 검수 묶음에서 AC002를 판정하고 별도 진단 승인 보고서를 추가하지 않는다.

최대값 왕복·config 재로드·실제 sample8·음성 반례·Player 제외·실제6방/정확6 적용·35/44·수동12·종단4·G8·B1 FINAL·CH1 완주는 현재 미수용이다. 9회·Q0·키/세대·프로필·원격·B1 FINAL 전 C1C 공유 통합0은 불변이다.

이번 종료는 이 Review 절1의 쓰기 종료·새 전체 SHA·원래63289바이트 접두사 확인·Root 소유 반환뿐이다. Luna의 한정 사전 반대판독과 Astra 범위 채택 전 제품/도구/checkpoint/Prepare/승인v3/진단/실행 작성0이다.


#### 10.10.7 독립 검수 세 지적의 정확 보정 — Review

상태는 Review / Approved=false이며 작성자는 실제 gpt-6-astra/ultra다. Luna gpt-6-luna/max의 §10.10 읽기 전용 독립 검수 P0=0/P1=3을 받아 승인 필드 결속, 최초 R01-A 단회, 실제 원시 연속성의 세 지적과 파일 바이트 상한을 이 절에서 보정 제안한다. 새 독립 보고·승인 사본은 만들지 않는다. 앞선 전체85328바이트/SHA F3D86DE3483343DF8F3669F7BB006D20527D6A9F3120148F72EA3B68112FCCD9를 그대로 보존한다. 이 보정의 독립 재판독·범위 채택은 미발급이며 지적의 정식 폐쇄를 선취하지 않는다. 앞의 §10.10.1~6과 차이가 있는 필드·초기화·읽기·바이트 설명에만 아래 후보를 우선한다. 제품 경로는 같은 Window.cs1, 새 진단 원시는 같은 JSON1뿐이며 Runtime·정상 serializer·다른 writer·Play·컴파일/실행 권한을 추가하지 않는다.

**가. 실행 승인과 사례 승인에 실제 필드·상수를 직접 결속한다.**

현재 Window의 ExecutionApproval/CaseApproval 및 ReadExecutionApproval/ProjectCaseApproval에는 아래 항목이 없다. 같은 네 선언/투영과 TryPrepare/진단 handler만 한정 보강 후보에 포함한다. 기존 strict 공용 함수의 본문과 다른 게임/적용 reader는 불변이다. 최종 GUI 승인 JSON은 이미 소비된 파일을 덮지 않는 실제 Root 실행 승인에서 이 추가 필드를 명시해야 하며, 아직 값/권한이 없는 현재 Review를 승인 JSON으로 쓰지 않는다.

| 정확 JSON 필드 | 위치와 실제 타입 | 요구 값/검사 |
| --- | --- | --- |
| uint64DiagnosticsAuthorizedNow | ExecutionApproval 및 각 CaseApproval, JSON boolean | 실행 승인 true와 선택 R01-A 사례 true가 함께 필요. 나머지9 사례는 명시 false. 문자열·정수·null·누락 거절 |
| uint64DiagnosticsCaseId | ExecutionApproval 및 각 CaseApproval, JSON string | 모두 정확 R01-A. 이 상수는 해당9 사례의 false 권한을 true로 만들지 않음 |
| uint64DiagnosticsPath | ExecutionApproval 및 각 CaseApproval, JSON string | 모두 아래 고정 절대 경로와 동일. 원문 경로 및 정규화 경로 둘 다 확인 |
| uint64DiagnosticsCallLimitSeconds | ExecutionApproval 및 각 CaseApproval, JSON integer/Int32 | 정확5. 5.0·5e0·문자열·누락 거절 |
| uint64DiagnosticsFileLimitBytes | ExecutionApproval 및 각 CaseApproval, JSON integer/Int32 | 정확32768 |
| uint64DiagnosticsInvocationLimit | ExecutionApproval 및 각 CaseApproval, JSON integer/Int32 | 정확1 |
| uint64DiagnosticsWindowId | ExecutionApproval, JSON UInt64 정수 리터럴 | 실제 승인된 이 EditorWindow의 EntityId.ToULong(GetEntityId())와 동일한 0 아닌 값. 기존 UInt64Integer로 엄격 투영 |
| uint64DiagnosticsNativeInputAuthorizedNow | ExecutionApproval, JSON boolean | 실제 GUI/연속 native·input 경로의 별도 권한이 닫힌 승인에서만 true. 현재 인간 결정 대기를 true로 채우지 않음 |
| uint64DiagnosticsNativeInputAuthority | ExecutionApproval, 기존 FilePin 객체 | 실제 닫힌 캡처 권위 원문 path/string·length/Int64 정수·sha256/string. 기존 FilePin/경로·길이·SHA 검증을 사용. 미래 경로/지문으로 생성 금지 |

고정 출력은 C:\Users\me\Documents\GPT-workspace\AcadeGameMaker\output\chapter1-playable-r1\evidence\b1-basic-movement-uint64-runtime-r1.json이다. 링크·다른 root·별칭 출력·사례별 접미사·새 파일명으로 전환하지 않는다. 나머지 사례에도 경로/한도를 명시하여 누락 기본값을 허용하지 않지만 해당 사례의 권한은 false이며 handler 상수 검사에서 다시 거절한다. 이 추가 필드들은 관측기 Configuration에 전파하거나 정상 sample/events/seal 스키마에 넣지 않는다.

실제 타입은 ParseStrictDocument의 중복/형식 거절 뒤 Required, Boolean, Text, Integer, UInt64Integer, FilePin으로 읽는다. 입력 DTO를 JsonUtility 기본값으로 채워 승인하지 않는다. 수치 리터럴의 부호/범위·필수 존재는 기존 strict 함수 기준을 그대로 적용한다. 권위 핀의 실제 의미는 아스트라의 같은 GUI 승인·Root 실행 소유가 담당하고 Window가 문서 상태나 인간 동의를 만들어 발급하지 않는다.

Root는 기존 GUI 소유 범위 안에서 이 관측 창에 표시된 자기 창의 UInt64 식별값을 읽어 실제 GUI 승인에 결속할 수 있다. 이를 위한 읽기 전용 표시 한 줄은 같은 Window 내부만의 후보다. 게임 객체 조회나 native 창 열거/숨은 API가 아니며 새 메뉴/파일/원시 writer를 만들지 않는다. 창 식별값이 재로드 뒤 유지되지 않거나 현재 승인된 창인지 확인할 수 없으면 거절하며 새 ID로 승인 파일을 고치거나 진단 권한을 자동 이전하지 않는다.

최초 준비 때 execution 승인 원문과 해당 R01-A의 위 필드·기존 samples/events/native/input 경로·source/Window/Observer 핀을 정확 값과 승인 파일 길이/SHA로 보유한다. handler는 버튼 활성화 bool을 신뢰하지 않고 같은 파일을 strict 재판독하여 그 길이/SHA, sessionId, R01-A, WindowId, Unity PID/StartTicks/EXE/version/project, 기존 모든 GUI guard 및 위 필드의 상수/양쪽 일치를 재확인한다. 같은 session이라도 승인 원문이 달라지면 진단0이다. 현재 승인된 collector의 session/case/Live/비봉인 및 기존 오류0와 같은 원문 경로도 다시 대조한다. 새 source 전체590 재해싱이나 외부 일반 경로 읽기는 추가하지 않는다.

**나. 준비 결속과 시도 소비를 분리하고 재무장하지 않는다.**

handler는 모든 분기에서 _caseId와 collector.CaseId 및 선택 승인.caseId가 문자열 상수 R01-A인지 확인한다. 다른 사례 이름이나 표시 문자열의 변경으로 진입할 수 없다. 기존 _sessionId와 실제 승인 sessionId도 일치해야 한다.

같은 Window에 직렬화한 private 상태는 diagnosticsPrepared, diagnosticsPreparedSessionId, diagnosticsPreparedExecutionSha256, diagnosticsPreparedWindowId, diagnosticsAttempted, diagnosticsAttemptedSessionId, diagnosticsPermanentlyBlocked로 한정한다. 기존 _armed/_playEntered와 함께 보관하며 Profile/EditorPrefs/SessionState/자산에 저장하지 않는다. diagnosticsPrepared와 session/pin은 이 승인된 원래 Window에서 최초 R01-A TryPrepare 한 번의 진입에만 결속한다. 임시 객체 구성 등 준비 도중 오류가 나도 그 결속을 지우지 않고 permanentlyBlocked로 닫는다. 다른 사례 준비, CleanupOwnedRoot, OnEnable/OnGUI, 승인 경로 변경, 실패 후 다시 누름, 새 창에서 재개로 이 상태를 초기화하지 않는다.

정상 첫 Play 진입의 기존 재로드는 이미 결속된 메타데이터의 복원만 허용하며 새 준비·새 시도 부여가 아니다. 전체 tuple의 일부가 없거나 모순이면 재구성이 아니라 거절이다. 수집 시작 후 domain reload/compile/창 비활성·파기는 기존 실패와 함께 permanentlyBlocked다. 창을 닫고 새로 연 객체는 원래 승인 WindowId가 아니므로 새 TryPrepare/handler로 같은 권한을 재사용할 수 없다. 이 식별값·메타데이터가 실제로 유지되는지는 첫 승인 사례 안의 관측 사항이며 현재 성공으로 가정하지 않는다.

handler에 최초 진입하여 같은 준비 tuple임을 확인하면 검사/IO 전에 diagnosticsAttempted=true, diagnosticsAttemptedSessionId=그 session을 고정한다. 같은 처리 내 이후 승인 재판독·Pause·범위 IO·순수 검사·크기 확인 실패도 시도를 되돌리지 않는다. 별도 원시를 FileMode.CreateNew로 최초 예약하며 존재하는 파일은 길이0이어도 무조건 거절한다. 그 파일이 자기 것으로 보인다는 이유로 다시 열어 이어 쓰거나 완료시키지 않는다. 시도 상태가 이미 true, permanentlyBlocked, 승인 창 교체, session 불일치이면 호출0이다. 새 session/새 파일명으로 같은 첫 R01-A를 반복하는 권한은 없다.

진단 첫 실패는 기존 collector.SealForAuthoring의 normalStop=false 한정 종료를 사용해 수집기의 기존 writer가 자기 실패/봉인을 남기게 할 수 있다. Window가 정상 sample/events/seal에 직접 쓰지는 않는다. 새 예외가 원래 진단 실패를 대신하지 않게 최선으로 닫고 Root에게 실제 Stop을 표시한다. Play/Pause/Resume/Stop 호출·게임 상태 수정·자동 재시도는0이다.

**다. Live와 원문 연속성을 구분하고 실제 검증식을 고정한다.**

진단 진입/종료에 EditorApplication.isPlaying && isPaused, !isCompiling && !isUpdating와 같은 collector Live/비봉인/FirstFailureJson 비어 있음/SealError 비어 있음을 직접 읽는다. Live=true만으로 아래 판독을 생략하지 않는다. Window의 _latestJson 캐시는 권위가 아니며 collector.LatestRowJson의 진입/종료 문자열과 마지막 실제 sample 원문을 직접 대조한다. 이들은 기존 관측기 표시 getter이고 게임 생산자 추가 조회가 아니다.

자기 samples/events 전체의 그 시점 완전한 prefix를 한 번 순차 판독한다. 같은 승인 경로의 실제 파일 크기를 기존 samples128MiB/events8MiB 안에서 제한하고 최대108001 sample, start1개 및 기존 행 상한을 적용한다. 과거 표본/게임 객체 목록을 메모리에 축적하지 않고 현재 행1, 첫/마지막 행 pin, 단순 카운터/해시만 유지한다. 아래 중 하나라도 확인할 수 없으면 형식 진단을 시작하지 않고 최초 실패다.

1. 사건 원문은 start 정확1개, schemaVersion=1, session/case가 승인과 같고 configuration의 session/case·소스 핀·실제 전달 설정2·경로·Unity 신원이 최초 준비와 일치해야 한다. start는 첫 sample 전 기록되므로 현재 실제 형식의 runKey=null/generation=0 및 lastTick=-1를 그대로 인정한다. 없는 원정 키를 정상값으로 채우지 않는다. start의 configuration/session/case를 첫 sample에 연결하고 첫 sample의 실제 비어 있지 않은 runKey/양수 generation을 해당 사례의 고정 키로 삼는다. start에 실제 키/세대가 들어 있는 다른 형식은 현재 스키마로 임의 수용하지 않는다. 이후 실제 표본·표본 이후 사건은 이 같은 키/세대여야 한다.
2. 각 sample을 기존 ReadSampleProjection으로 읽고 원문 순서 i=0..N-1에서 kind=sample, tick=i, distinctCount=i+1, nextExpectedTick=tick+1, startCall=1, firstTickZero=true, sameSourcesBeforeAfter=true를 대조한다. N>=1, 마지막 tick=N-1, N=collector.DistinctSampleCount다. session/case/room/socket/runKey/generation 및 표본 식별8은 같은 실제 사례 안에서 불변이어야 한다. frameOrdinal은 이전+1, modeEpoch는 기존 승인된 입력 순서, 시간·fixedCall·displayCall/frameCount는 역행0이다. 기존 HP/위험/Transfer·세 원본 유효성 및 G3 수용 기준을 이 순수 진단으로 대체하지 않는다.
3. sample.streamBytes는 해당 JSONL 행의 실제 시작 byte offset과 같아야 한다. LF까지 포함한 각 행 길이의 누적이 다음 offset이며 읽은 마지막 완전 행 끝과 samples prefix 길이가 같아야 한다. 전 행의 sample JSON 자체는 기존32768바이트 한도이고 LF는 별도로 계수한다. 누락·재정렬·중복 sample 행을 정규화하여 통과시키지 않는다.
4. sample과 duplicate/unavailable 사건을 자기 fixedCall 번호로 순서 대조한다. 정상 완료·duplicate·초기 unavailable은 각각 하나의 FixedUpdate 결과이며 fixedCall=그 시점 samples+duplicates+unavailable, samples=N까지 전진한다. duplicate는 별도 사건 수/누적 duplicates와만 대조하고 distinct sample이나 gap 보충으로 세지 않는다. duplicate의 lastTick/receipt frameOrdinal/modeEpoch/키/세대는 그 직전 원래 sample과 같아야 한다. 처음 sample 전 unavailable은 기존 첫60초/미발행 조건만 허용하며 첫 sample 이후 unavailable은0이다. 진단에서 unavailable 전체0이나 duplicate 전체0을 새로 요구하지 않는다. start의 미발행 값과 사건 카운터는 원문대로 보존한다.
5. 사건 전체에 firstFailure/failure·비정상 종료·봉인·source 교체가 없고 collector와 Window의 기존 첫 실패도 비어 있어야 한다. 서로 다른 tick 순서 검사와 duplicate 수 검사는 각각 기록한다. gapCount는 실제 i 대조의 누락0에서 계산하며 미판독 구간이 있으면0으로 채우지 않는다.
6. 마지막 sample의 JSON UTF-8 바이트(LF 제외)가 collector.LatestRowJson과 바이트 동일해야 하며 읽기 전후 그 문자열·DistinctSampleCount 및 원래 승인/session/collector 신원이 불변이어야 한다. 다른 JSON 재직렬화 결과나 _latestJson 캐시로 비교를 대신하지 않는다. events의 마지막 complete 상태가 마지막 sample 이후 duplicate를 포함하면 별도 최종 사건 카운터로 남기며 마지막 sample의 과거 duplicateCount를 현재 총수로 덮지 않는다.

현재 start의 미발행 키 상태를 위와 같이 연결하는 것은 원시 변경이 아니다. 첫 sample의 값만으로 Start가 이미 그 키를 확인했다고 쓰지 않으며, 실제 생산 참조 검증은 기존 collector/독립 QA 책임으로 유지한다.

**라. 동일 읽기 handle의 범위 지문과 native/input 참조를 사용한다.**

범위 판독은 Window private 구현에서 FileStream(path,Open,Read,FileShare.ReadWrite) 하나를 열고, 그 동일 handle의 Length를 읽기 전·후에 확인한다. Observer.Sha 또는 다른 FileShare.Read 전용 재열기는 활성 원시의 해시에 사용하지 않는다. 엄격 UTF-8 디코딩·완전 LF 행 경계·범위 SHA256은 그 handle에서 실제 읽은 bytes로 계산한다. 매 파일 전체 prefix의 offset=0/length/sha256, start 및 첫/마지막 sample의 offset/length/sha256를 보유한다. bytes 범위는 offset 이상 offset+length 미만이며 행 pin은 실제 LF 포함 여부를 includeLf bool로 명시한다. partial 줄·크기 변경·읽기 소실·공유 모드 거절은 단회 실패이고 대기/안정화 반복0이다. 핸들을 열 수 없다고 writer 공유모드나 OS 설정을 바꾸지 않는다.

samples/events의 SHA는 실제 읽은 prefix의 내용 지문이다. 안정된 길이가 동시 같은 길이의 덮어쓰기 부재를 입증한다고 주장하지 않는다. 정상 append 전용 collector 단독 소유 및 현재 Pause·source 불변 조건을 함께 명명하고 최종 독립 원시 대조가 이를 확인한다. Window는 원시 rename/replace/truncate/쓰기0이다.

nativeEvidencePath/inputEvidencePath는 동일 R01-A 승인에서 이미 허용된 정확 두 경로만 읽기 참조한다. 각 현재 prefix [0,안정 길이)의 같은 handle 범위 해시를 계산하되 native2GiB/input8MiB 기존 상한과 진단5초/전체 시간 상한을 넘기지 않는다. 실제 범위 bytes가 없거나 공유/안정 길이/현재 권위 pin을 확인할 수 없으면 거절한다. 아직 정해지지 않은 영상 컨테이너·입력 문서 스키마나 Pause 사건 offset을 추정하지 않는다. 필요한 경우 Root의 정확 GUI/native-input 계획이 실제 형식과 사건 범위 판독을 먼저 닫아야 하며 현재 읽기 권한만으로 의미 검증을 새로 구현하지 않는다.

진단 JSON에는 nativePrefix/inputPrefix 각각 path·offset=0·length·sha256·readLengthBefore/After와 관측 UTC를 기록한다. 실제 Pause의 진입/종료 bool·UTC/단조 시각·Unity PID/StartTicks/WindowId 및 승인/세션/사례와 함께 결속한다. 이 범위 pin은 이후 루나가 실제 연속 native/input에서 Pause·초점·진단 버튼 사건과 같은 세션을 대조할 참조다. 이벤트 byte offset/시각 대응이 확인되기 전 nativePauseEventRange와 inputPauseEventRange는 null, nativeInputContinuityAccepted=false, nativePauseInputCorrespondenceAccepted=false다. prefix 존재나 실제 Editor isPaused=true만으로 그 null/false를 성공으로 바꾸지 않는다. 진단 호출 뒤 Resume/Stop의 미래 원시는 이 파일에서 핀을 만들지 않고 기존 최종 원시 검수에서 연결한다. 인간의 앱 동의/관측 예외 결정도 실제 별도 GUI 권위 전 미발급이다.

추가 허용 IO는 기존 승인 원문·그 핀 검증, 같은 R01-A samples/events/native/input의 위 제한 읽기와 진단 JSON1의 CreateNew 쓰기에 한정한다. 임의 화면/키 수집·다른 파일·프로필·native helper 제작·추가 원시 writer는0이다. 권한/원시 형식 미결 때문에 준비가 막히면 기존5초 안에서 진단을 시도했다는 성공 기록을 만들지 않는다.

**마. 파일 상한을 직렬화 뒤 실제 바이트로 검증한다.**

진단 출력은 BOM 없는 엄격 UTF-8의 JSON 문서1과 끝 LF 한 바이트다. 최종 GetByteCount(json)+1<=32768을 검사하고 동일 bytes를 한 번 쓴다. 문자 수·UTF-16 길이·개행 제외32KiB·OS 기본 개행으로 대체하지 않는다. 기존 정상 sample의 행 상한과 이 파일 전체 상한은 별개다.

최소 출력 구조는 필드10/변환19/결과 코드 사전1, 정상 원시10 비교, 경계40 행, 반례190 행과 필수 입력/범위/시도/시간/실패 메타데이터다. 경계 행은 [필드번호,값번호,원문숫자,왕복값,strict값,결과코드]이며 세 숫자는 최대20 ASCII 숫자의 문자열로 기록한다. 각 행 쉼표를 포함해 최대192바이트다. 음성 행은 [필드번호,반례번호,예외코드,결과코드]의 작은 정수 배열이며 쉼표 포함 최대48바이트다. 기대 타입/기대 거절은 변환 사전에서 정확히 복원한다. 예상 IOException/JsonException의 반복 메시지190개는 저장하지 않고 실제 예외형 코드만 각 행에 남긴다. 미실행 행은 별도 결과 코드로 명시하며 수행 성공으로 채우지 않는다.

최악 인코딩 예산은 메타데이터/정상10/사전/한 번의 오류 전체12288 + 양성40×192=7680 + 음성190×48=9120 + 외곽 구문128 + LF1 = 정확29217바이트 이하로 제한한다. 이는 full DTO/JSON190개를 저장하는 예산이 아니다. 필수 경로의 JSON 이스케이프 포함 길이는 각1536바이트, 식별 문자열은 각256바이트, 최초 예상 외 예외형은256바이트·메시지는512바이트를 넘지 않도록 사전 검증한다. 전체 메타데이터12288을 만족하는지 개별 상한과 별도로 확인한다. 실제 원문 경로/식별값을 줄여 이 제한에 맞추지 않는다.

최초 준비 때 현재 입력과 최대 길이 placeholder/모든230 결과 행을 사용한 비실행 직렬화 길이 산정으로 위 최악 예산을 확인한다. 이는 게임/JsonUtility 왕복이나 parser 음성 검사 실행이 아니다. 아직 없는 실제 runKey/native 사건을 placeholder로 결과에 발급하지 않는다. handler에서도 실제 결과 bytes를 최종 확인한다. 예외 메시지/필수 값이 한도를 넘거나 실제 구조가 이 산정에 안 들어가면 진단 실패다. 잘라 성공 JSON으로 저장하지 않고 예약된 빈/부분 원문을 보존하며 Window에 최초 크기 실패와 원래 오류를 표시하고 기존 실패 봉쇄/Root Stop으로 끝낸다. 최종 JSON 폐쇄를 확인할 수 없으면 fileClosed/pass를 true로 쓰지 않는다.

진단5초는 handler의 최초 시도 소비 직전부터 승인 재판독·범위 읽기/해시·연속성 검사·왕복8묶음/양성40·음성190·직렬화·flush/close까지 전부 계수한다. 각 단계 전후 남은 시간을 검사하고 이미 한도가 모자라면 뒤의 검사를 시작하지 않는다. main thread의 동기 API를 강제로 선점하거나 파일 hash의 미래 완료 시간을 보장하지 않는다. 전체 원시가 커서 단회5초 안에 못 읽으면 미완료/첫 실패이며 생략·표본추출·추가 시간·반복0이다. 1800/18000 시계는 Pause/진단 동안도 계속 계수한다.

최종 passed는 위 승인/단회/자기 원시 연속성 및 UInt64 형식 검사의 실제 완료에만 한정된다. runtimeSourceAccepted/independentAcceptance/actualRoomAccepted/gameplayAccepted/nativeInputContinuityAccepted/nativePauseInputCorrespondenceAccepted는 작성 당시 false다. native/input 사건 의미와 인간 결정·첫 R01-A 전체 결과는 기존 승인/독립 검수에서 별도로 닫으며 형식검사 통과가 이를 대신하지 않는다.

이번 보정 작성은 같은 §10.10의 Review 수정 제안일 뿐이다. 정확 Window1/JSON1 외 제품·Runtime·정상 serializer·도구·checkpoint·권위·원장·실행 쓰기는0이다. 기존 Editor1 컴파일 후보의 별도 선행·예산, Input/Movement 재컴파일0, R4 소비 원문, 실제 G3/6방·필수 회귀·B1 FINAL·C1C 공유 선행 및 대기 중 인간 결정은 불변이다. 종료 시 기존85328바이트/F3D 접두사와 현재 최종 파일 지문을 확인하고 쓰기를 닫아 Root에 반환한다.


#### 10.10.8 Window1·도구 준비2·native0 Prepare의 한정 채택 — Approved

- 채택일/실제 승인자: 2026-10-08, 아스트라 gpt-6-astra/ultra. Root의 실제 모델/설정은 gpt-6.1-sol/medium이다.
- 판정: §10.10.1~6을 §10.10.7의 명시 보정과 함께 **아래 준비 범위에만 Approved로 채택**한다. finalApproval=true, boundedPreparationApproved=true다. sourceLimitedAcceptedNow=false, runtimeSourceAccepted=false, sourceReady=false, nativeExecutionAuthorizedNow=false, compilerExecutionAuthorizedNow=false, unityExecutionAuthorizedNow=false, guiExecutionAuthorizedNow=false, diagnosticsInvocationAuthorizedNow=false, nativeInputContinuityAccepted=false, actualRoomAccepted=false, b1FinalAccepted=false, chapter1Accepted=false를 유지한다. AC-M1COMP-R1-002의 런타임 및 AC-C1B-G3OBS-010은 미수용이다.
- 검토 원문/보존: 이 절 앞의 정확106812바이트/SHA 30688C37D043D719D22C9A436909D87A26320CF58F30B3C0B9E779678B5E5EB8를 바이트 접두사 그대로 보존한다. 앞선 Review 당시 사실·미발급 표시를 소급 수정하지 않는다. 소비된63289바이트/8F42DC07E099BD0A5FD3700A303C1565C97E0D9836805C1E41C82BA559DE6067 및 기존 승인·실패·원시는 불변이다.
- 독립 근거: 실제 Luna gpt-6-luna/max가 위106812바이트 원문을 재검수한 작성 종료 결과를 Root가 전달했다. P0=0/P1=0이고 기존 GUI 승인 결속·첫 R01-A 단회·Pause 원시 연속성의3지적은 **계약 설계상 폐쇄**다. 별도 보고파일/SHA는 발급되지 않았으므로 만들지 않는다. 이 근거는 새 구현·예비 실행·최대값 왕복·native/input 연속성의 수용이 아니다.
- 사용자 동작/산출물: 이후 실제 권한이 닫힌 첫 R01-A Play 안에서 tick0부터 Pause까지 자기 원문을 대조하고 분리 DTO의 UInt64 형식을 단회 검사할 수 있도록 기존 편집기 창과 제한 컴파일 준비를 구현한다. 이번 산출물은 Window 델타1, helper5/도구 계획5, 실제 현재 checkpoint와 native0 입력 계획이다. 실제 버튼 호출·JSON 진단 결과는 이번 산출물이 아니다.
- 추적: REQ/AC-M1COMP-R1-001~006, REQ/AC-C1B-G3OBS-005/008/010/012, REQ/AC-PROC-011/012. 기존 필수 AC·게임 행동·UInt64 형식·9회/Q0·키/세대·프로필·원격·B1 FINAL 전 C1C 공유 통합0을 바꾸지 않는다.

**정확 쓰기·준비 경로와 담당**

아래 상대 경로의 기준은 저장소 루트다. 현재 제품/도구 소유는 Root이며, 이 문서가 담당자에게 소유를 이미 넘겼다고 기록하지 않는다. Root의 명시 인계 뒤 아래 서로 겹치지 않는 소유만 활성화한다.

| 담당 | 정확 허용 경로 | 채택 범위 |
| --- | --- | --- |
| Sol 소스 단독, 실제 gpt-6.1-sol/ultra | output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs | §10.10.1~4/7의 편집기 전용 private 진단/최소 표시, 실행·사례 승인 필드와 strict 투영, 첫 R01-A/원래 Window 식별·시도 수명, 자기 원시 제한 판독/범위 해시, 분리 DTO230판정 및 별도 JSON1 writer의 구현. 기존 정상 parser 함수 본문·게임/방 적용/Runtime·meta는 불변 |
| 별도 Sol 도구 단독, 실제 gpt-6.1-sol/ultra | output/chapter1-playable-r1/tools/b1-basic-movement-minimal-csc-root.v5.ps1 | 소비된 helper4를 보존한 native0 Prepare 및 미래 Editor-only Execute1의 가드/입력/실제 시간 경계 구현. 구현은 허용하나 Execute 호출은0 |
| 같은 Sol 도구 담당 | output/chapter1-playable-r1/preparation/b1-basic-movement-minimal-compile-tool-plan-sol61.v5.json | helper5의 실제 필수 필드·경로·부모 재사용·한도·검사·미실행을 기록 |
| Root | output/chapter1-playable-r1/evidence/b1-final-source-checkpoint-sol61.v10.json | 소스 쓰기 종료/Root 반환 후 현재590 행 및 v9 대비 정확1 변경/589 불변/추가삭제0를 실제 기록. 작성자 모델은 실제 Root로 표시 |
| Root native0 Prepare 단1회 | output/chapter1-playable-r1/preparation/b1-basic-movement-minimal-compile-plan-root.v5.json | 실제 helper5/현재 checkpoint·입력·부모 근거를 확인한 새 계획을 CreateNew 생성 |
| 위 같은 Prepare | output/chapter1-playable-r1/project/Library/b1-minimal-csc-g3-observation-r5/AcadeGameMaker.Combat.Authoring.Editor.rsp | 새 r5 디렉터리의 정확 응답1. DLL/ref/PDB·컴파일 로그/실행 시작·결과/진단 raw 생성0 |

각 Sol은 실제 변경·REQ/AC·국소 확인·미실행·최종 길이/SHA·쓰기 종료를 같은 Root 작업 원장으로 반환한다. 새 상태/승인 사본·전체 SourceQA 반복을 만들지 않는다. 소스/도구의 타입·필드·파서·경로 및 최소 크기 산정의 비실행/비native 국소 확인은 범위 안에서 진행할 수 있다. Unity/CLR 실행·진단 handler/왕복/반례 실행·Add-Type/암묵 compiler·네이티브 시험은 이번 준비 권한에 없다. Prepare 첫 실패/기존 경로 존재/불일치는 원문을 보존하고 종료하며 자동 재호출0이다. 범위/입력 변화가 필요한 경우 같은 원장에 원인·최소 재현·변경·새 관측 목적을 남겨 정확 기존 한도를 다시 판정한다.

**현재 기준과 부모 결과의 한정 의미**

작성 직전 직접 대조한 기준은 다음과 같다. 전체590 실제 파일 재해싱을 독립 QA처럼 반복하지 않았다.

- v9 checkpoint: 125249바이트/SHA 28F1B590C504030F05C43BC599FD5B476C2E2A27F0B69E2D8DF48BB35F3FDBCE.
- 변경 전 Window: 63841바이트/SHA 92685D0BF7D3F9041C2F6E27E97347405EFD664654D55E23A81D3A6F20D63746.
- 불변 Observer: 39505바이트/SHA DD5B06F63207B62AD05C97B848AF6819FFA127D8C454C129075F9BA23393C9D6.
- 기존 G3 권위: 62045바이트/SHA 39C64FF77AA6F8B89EAEAD430C0BAE5B19E780F9842182F37CDA61B29E8E222F.
- 닫힌 R4 원시 result: 151238바이트/SHA A7FE8B71B1829D44AB26F08D765C6AAC45ACA29B50154A3B4B92D4053AD0CF09. 실제 Luna 결과QA2: 15397바이트/SHA AE0EB134755EE25AA69689634388EC552243BCA8CC250F2F8CCFC0DA2DD19B9E, 실제 정수 P0/P1=0/0. 실제 두 컴파일 정수0·폐쇄/회수 및 제한 수용 AC-M1COMP-R1-001/003/004/005만 근거다. 경고15와 elapsed/UTC 차이의 비차단 기록은 보존한다.
- R4 Input ref: 76288바이트/SHA C2E2EF7EE9CEC637537EFAEB5BD0C634B8BAEF67BAC38C0826E45FD05614D2E0. R3 Movement ref: 10240바이트/SHA 0920429D88AADE31BC34B8F6A8E6CC27C70F16A249E8B9657755634A6EFADBAB. 해당 개별 성공과 현재 정확 입력 영향이 유지되는 경우에만 후속 독립 재사용 판정 대상이다.

R3 전체 실패/P1두건·소비된 승인1, R4 승인2/도구/계획/결과/QA는 덮지 않는다. R4 Editor 성공은 변경 전 Window의 역사이며 새 Window의 컴파일 성공이 아니다. 새 Input/Movement 컴파일0, 새 ref 생성0이다. 실제 새 Source SHA는 v10/실제 실행 입력에 결속하고 설명 문서의 미래 SHA를 상호 요구하지 않는다. 이번 준비 승인이나 미래 plan5가 자기 독립 소스 수용을 발급하지 않는다.

**필수 검사·후속 실제 권한·종료**

1. source1/tool2 쓰기 종료·Root 반환과 현재 입력의 정확 차집합 확인 뒤 Root가 실제 v10 및 국소 가드를 확인한다. helper5 Prepare는 본 준비 권위·실제 v10·Root 소유/쓰기 종료·부모 개별 성공/결과QA/현재 입력 핀을 받는다. 미래 toolQA5/planQA5/owner3/승인v3를 Prepare의 입력으로 요구하지 않는다.
2. native0 Prepare1 뒤 Luna gpt-6-luna/max가 같은 기능 묶음에서 소스1/589·GUI 필드/상수·단회 수명·원시 연속/범위 IO·29217/32768바이트 산정·조건부 Player 제외와 helper5/실제 RSP1·부모 Input/Movement 재사용을 독립 판독한다. 기존 명명된 tool-review-luna6.v5 뒤 plan-review-luna6.v5 순서이며, 계획QA5에 현재 소스 델타 수용과 부모 재사용을 포함한다. 별도 SourceApproval/SourceQA 문서를 추가하지 않는다. 계획의 toolQA 자기 SHA 순환은0이다.
3. 실제 QA·현재 입력이 닫힌 뒤 Root의 minimal-compile-ownership-root.v3와 Astra의 minimal-compile-execution-approval-astra.v3를 기존 §10.10.5 정확 경로에 새로 발급해야 한다. 그때에만 sourceLimitedAcceptedNow와 실제 컴파일 권한을 판정한다. 새 결합 승인은 **Editor1×60초/합산60초·종료5초/회수5초·단일 Execute·실제 시작 최대1·자동재시도0·재개0·두 번째 Execute0** 및 실제 모든 입력을 결속한다. Root 단독 도구 권한 심사를 별도로 통과한 실제 호출 전 compiler 실행0이다. 기존 실패/소진 예산의 재개로 계산하지 않는다.
4. 실제 Editor 결과는 Luna 결과QA3와 Astra 한정 판독 뒤에만 수용한다. 이 단계까지도 GUI/Play/실제 진단 호출은0이다. 앱 동의·native/input 관측 예외에 관한 인간 질문2는 아직 답변 미도착이다. 같은 첫 R01-A의 실제 GUI 승인에 §10.10.7 필드·원래 Window·고정 JSON1·5초/32768/1회·실제 캡처 권위가 명명되고 원시 수단이 닫힌 뒤에만 기존 Play 안의 진단 실행을 별도 판단한다. 본 준비 승인으로 uint64DiagnosticsAuthorizedNow 또는 uint64DiagnosticsNativeInputAuthorizedNow를 실행 JSON에 true로 발급하지 않는다.
5. 준비의 종료는 정확 소스/도구 작성 종료와 Root 반환, 실제 v10/Prepare 입력, 독립 묶음 검수에 제출할 구체 결과다. 이후 실제 최대값/설정 재로드/sample8/반례·native/input 사건 의미·방/제품/CH1 수용은 별도 미완료다. 아스트라의 이번 문서 작업은 기존106812바이트 접두사 검증·최종 길이/SHA·쓰기 종료·Root 반환으로 닫으며 다른 파일을 쓰지 않는다.

위 범위 안의 준비만 이후 효력을 갖는다. 최초 Play 횟수·1800/18000 시간·원래 첫 실패 중단·Q0 인간 관문·현재 프로필 범위·키/세대·원격·B1 FINAL 전 C1C 공유 통합0은 그대로다.

#### 10.10.9 사용자 원복에 따른 신규 배정 설정 호환 — Approved

상태는 Approved이며 2026-10-11 사용자 원복 지시와 현재 AGENTS.md·운영 모델을 근거로 실제 아스트라 gpt-6-astra가 승인한다. 이번 작업의 호출은 추론 설정 상향값을 지정하지 않은 도구 기본 설정이다. 과거 ultra/max 설정이나 주 대화 설정을 변경했다고 기록하지 않는다. 변경 전 전체116936바이트/SHA 931620BC0638083F727F1C82CC999C8742DDA58688D5C192737E7B000ACC0621을 그대로 보존하며 이 절만 덧붙인다.

§10.10.8의 이후 솔 소스 담당·별도 솔 도구 담당은 gpt-6.1-sol, 독립 루나 검수 담당은 gpt-6-luna, 승인·통합 아스트라는 gpt-6-astra를 사용하되 신규 호출에 상향 추론 설정을 지정하지 않고 도구 기본 설정으로 기록한다. 이 세 역할의 과거 ultra/max 강제 표기는 이후 배정에 한해 이 조항으로 대체한다. 실제 필요한 담당 수만 배정하며 동시 한도 충족을 목적으로 배정하지 않는다. ADR-0041의 실제 가용 상위 버전 확인과 ADR-0043의 역할·소유 분리 및 테라 제거를 유지한다. 과거 실제 모델·설정·승인·실패·검수 증거는 수정하지 않는다.

사용자 산출물은 §10.10.8에서 이미 승인한 Window1·helper5/tool-plan5 준비2와 Root의 v10·native0 Prepare1이다. 실제 선행, 정확 경로·행동·인터페이스·쓰기 소유, REQ/AC 추적, 검사 순서·유한 예산·최초 실패 제한·종료 조건은 같은 절 그대로 적용한다. 소유 이전은 Root의 명시 인계 뒤에만 활성화한다. 실제 앱 동의와 추가 Unity 관측 예외의 인간 결정은 미도착으로 보존하며 컴파일·GUI·Play·진단 실행 권한은 이 보정으로 발생하지 않는다. 해당 결정과 의존하지 않는 기승인 준비는 계속한다.

이 절은 REQ-PROC-011/012 및 AC-PROC-011/012의 기존 묶음 운영 안에서 사용자 정정과 신규 배정 설정 충돌만 닫는다. 새 기능 승인·전체 회귀·추가 독립 검수 문서를 발급하지 않는다. 이번 문서 검사는 기준 접두사 바이트 동일성·최종 길이/SHA 확인만이며 제품 AC 실행 성공을 뜻하지 않는다. 작성 종료와 결과 반환 뒤 이 파일의 단독 쓰기 소유를 Root에 반환한다.

#### 10.10.10 단일 쓰기와 실제 폐쇄 증거의 의미 보정 — Approved

실제 아스트라 gpt-6-astra가 도구 기본 추론 설정으로 판정한다. §10.10.4/7의 단일 쓰기·재열기 금지와 폐쇄 후 성공 조건을 동시에 지키기 위해 같은 준비 구현 안에서 아래 의미 정정을 Approved로 채택한다. 기존119351바이트/SHA 06F820C6FA54B9FF2F958C90C8B21EDB63F16F577504F92C53A23CEFA3D0F7B5 접두사는 보존한다. REQ-M1COMP-R1-002 및 기존 형식 진단 AC의 필수 검사를 면제하지 않는다.

단일 진단 JSON 본문은 직렬화 시점의 사실만 담는다. fileClosed는 반드시 false이며 이는 당시 자기 출력 handle의 폐쇄가 아직 미관측이라는 뜻이다. 전체 완료를 뜻하는 pass 또는 passed가 있으면 본문에서는 반드시 false다. 별도 validationPassed boolean은 그 직렬화 직전까지 승인·단회·자기 원시 연속성과 요구된 UInt64 형식 검사가 모두 실제 완료됐는지만 기록하며, 쓰기·flush·close·최종 시간 상한·독립 수용을 포함하지 않는다. 미실행·실패 검사가 하나라도 있으면 false다. 기존 수용 관련 false 필드와 null 원시 사건 범위를 그대로 유지한다. 이 의미는 본문 메타데이터 또는 고정 사전에서 복원 가능해야 하며 기존29217/32768바이트 산정 안에 포함한다.

같은 최초 handle에 한 번 기록한 뒤 flush와 Dispose/close가 예외 없이 실제 반환하고 최종 경과가5초 이내이며 최초 실패가 없음을 확인한 경우에만 창의 비직렬화 메모리 결과에서 fileClosed=true와 최종 passed=true를 표시할 수 있다. 최종 passed는 validationPassed도 true여야 한다. 시간 초과·쓰기/flush/폐쇄 예외·폐쇄 확인 소실은 최종 성공이 아니며 첫 실패·봉쇄 및 기존 Root Stop 절차를 적용한다. 재로드 등으로 이 메모리 증거가 소실되면 폐쇄 성공을 파일 존재나 본문에서 재구성하지 않는다.

Root는 기존 한 작업 원장에 실제 관측한 창의 최종 상태·시각·session/case/WindowId·고정 출력 경로와 이후 독립적으로 확인한 실제 파일 길이/SHA를 결속한다. 본문 validationPassed만으로 최종 성공이나 AC 수용을 발급하지 않는다. 독립 검수는 원시 내용·실제 폐쇄 관측·5초 전체 경과·동일 파일 핀을 함께 확인하며 하나라도 없으면 미완료다. 이 원장 기록은 새 실행 권한이나 새 QA 파일을 발급하는 규칙이 아니다.

출력 본문을 폐쇄 후 보정하거나 재열기·덮어쓰기·추가 쓰기하지 않는다. Window의 사후 자기 파일 읽기도 추가하지 않는다. 정확 Window1/JSON1·CreateNew·시도1·5초·32768바이트·필수230판정·최초 실패 보존 및 현재 준비 한정 권한은 그대로다. 컴파일·GUI·진단 실행0을 유지한다. 이번 작업은 이 문서1의 덧붙임과 접두사/최종 지문 확인으로 종료하며 쓰기 소유를 Root에 반환한다.

#### 10.10.11 신규 참가자 기본 설정의 판독 호환 준비 — Approved

2026-10-13 실제 아스트라 gpt-6-astra가 도구 기본 설정으로 이 준비 범위만 Approved로 채택한다. 기존122464바이트/SHA D633704CF504F4A221B0EF7084E009805C9D947DA62630B23F4181DFCA3BE774 접두사를 보존한다. 사용자 원복 지시와 §10.10.9의 신규 배정 규칙을 기존 Window 참가자 판독에 연결하며, 독립 검수의 deferredGuiParticipantCompatibility P1을 기술 보정 대상으로 삼는다. 이 승인만으로 지적 폐쇄·실제 GUI 동의·실행·제품 수용을 발급하지 않는다.

사용자 동작/산출물은 향후 기본 설정으로 작성된 실제 아스트라 승인·루나 방 검수가 참가자 설정 때문에 잘못 거절되지 않도록 하는 Window1의 판독 보정이다. 담당은 Root가 명시 인계한 솔 gpt-6.1-sol 한 명이며 상향 추론 설정을 지정하지 않은 도구 기본값을 사용한다. 단독 허용 경로는 output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs다. 출발 원문은120492바이트/SHA 4D455A1E0CDFC8F505414D5B6DD6CE3B3529528E3D83BDF4B1F39741D512FD9C이며 쓰기 전 동일성을 확인한다. r5의 해당 바이트 컴파일 성공·독립 결과QA3·한정 수용은 역사로 보존한다.

정확 의미와 허용 변경은 다음과 같다.

1. 현재 TryPrepare·진단 handler·방 수용 쓰기 승인·방별 아스트라 승인에서 공용 Astra(Participant)를 호출하는 네 소비 위치는 모두 이후 새 권한/현재 방 수용을 판정한다. 이 함수는 participant 존재, model 정확 gpt-6-astra, reasoningEffort 정확 '도구 기본값, override 없음', role의 비어 있지 않은 비공백 문자열을 요구한다. 공용 함수의 이 조건 변경만 채택하며 네 위치의 나머지 조건은 유지한다.
2. 현재 방별 루나 검수의 직접 참가자 조건도 같은 방식으로 model 정확 gpt-6-luna, reasoningEffort 정확 '도구 기본값, override 없음', 비어 있지 않은 비공백 role을 요구하도록 한정 보정한다. 같은 Window의 작은 private 판독 함수로 추출할 수 있다. 다른 모델/역할 계열·누락/null·빈 문자열/공백 role·임의 추론값·대소문자/공백 정규화에 의한 수용은0이다. role의 기존 자유 설명을 새 임의 역할명 상수로 좁히지 않는다.
3. 역사 원문의 gpt-6-astra/ultra 및 gpt-6-luna/max는 당시 실제 설정으로 그대로 읽고 보존한다. 현재 ProjectParticipant의 엄격 문자열 투영을 변경하거나 역사 값을 기본값으로 치환하지 않는다. 위 네 아스트라 소비와 방별 루나 소비는 현재 권한 판단이므로 역사 설정을 OR 조건으로 허용하지 않는다. 현재 Window에는 역사 승인 재사용을 새로 발급하는 입구를 추가하지 않는다. 과거 핀으로 닫힌 기록의 역사 판독과 현재 실행 권한 수용은 구분하며, 향후 실제 역사 권한 재사용이 필요하면 그 정확 원문 핀·세대·용도를 별도로 닫아야 한다. 파일명·시각·설정 문자열만으로 역사 권위를 추정하거나 새 승인에 ultra/max를 허위 기록하지 않는다.
4. 참가자 DTO/공개 API/스키마/메뉴·문자열 투영·정상 parser6·UInt64 serializer·Runtime·게임·UI 동작·프로필은 변경하지 않는다. 기존 승인 파일 SHA·실행 소유·키/세대·원시 핀·최초 시도·권한/시간 조건·P0/P1·방/사례/현재 소스 결속은 모두 유지한다. 이번 승인으로 이전의 정상/방 적용 불변 경계에서 예외가 되는 부분은 위 참가자 조건뿐이다.

REQ-M1COMP-R1-002, REQ-C1B-G3OBS-005/008/010/012 및 REQ/AC-PROC-011/012를 추적한다. 검사 계획은 정확 소스1의 문자/구조 델타와 네 공용 호출·루나 소비1, 신규 아스트라/루나 각각 정상1 및 누락/null/빈값/공백 role/잘못 모델/임의 설정/역사 설정의 현재 권한 거절 표를 비실행으로 대조하는 한 국소 묶음이다. 역사 투영 원문 불변과 나머지 권한 조건 불변도 확인한다. 실제 C# 실행·컴파일·Unity·GUI·진단·프로필 접근은0이다. 정적 검사표를 실제 parser/GUI 시험 통과로 표시하지 않는다. 처음 정적 확인1회와 같은 허용 델타 안 발견 보정의 국소 재판독 최대2회를 허용하며, 같은 오류 두 번 또는 경계/예산 초과에는 원인·최소 재현·변경·새 관측 목적을 기존 원장으로 반환한다. 작은 보정마다 새 승인·전체 회귀·QA 사본을 만들지 않는다.

종료 조건은 솔의 실제 변경/REQ·검사 범위/미실행·최종 길이/SHA·쓰기 종료와 Root 소유 반환이다. Root는 실제 새 checkpoint v11로 현재 소스를 고정하고, 루나는 현재 델타와 영향 범위를 같은 묶음에서 독립 판독한다. AC-M1COMP-R1-002 런타임 및 AC-C1B-G3OBS-010은 계속 미수용이다. 참가자 P1 폐쇄는 변경 후 독립 판독 결과로 판단하며 작성자가 선취하지 않는다. r5 단일 Execute 권한은 이미 소진됐으며 새 바이트의 컴파일 성공으로 재사용할 수 없다. 이후 새 컴파일 계획·정확 입력·독립 검수·소유·별도 아스트라 실행 승인이 필요하다. 지금 helper6/새 계획/권위·도구·제품 다른 파일의 쓰기는 허용하지 않는다.

실제 GUI/Play/진단·Unity·프로필 권한과 대기 중 인간 결정은0/보류 상태를 유지한다. sourceReady/runtimeSourceAccepted/방/B1/CH1 수용도 false다. 이 문서 작업은 정확1파일 덧붙임·접두사 보존·최종 길이/SHA 확인 뒤 종료하고 쓰기 소유를 Root에 반환한다. 이후 Window 소유는 Root의 명시 인계로만 활성화한다.

#### 10.10.12 r6 현재 입력·소유·승인의 기존 원장 직접 결속 — Approved

2026-10-13 실제 아스트라 gpt-6-astra가 도구 기본 설정으로 이 한정 호환 절차와 native0 계획 준비만 Approved로 채택한다. 기존128418바이트/SHA D7FFC792BA7E5D7AD8B6C158087FC0A90F793734D3F103FFA1DAD1AE3459B92D 접두사를 보존한다. REQ/AC-PROC-011/012에 따라 §10.10.8의 새 도구QA/계획QA/소유/승인 사본 직렬 발급을 아래 같은 원장 안의 실제 계획·독립 판독·소유·아스트라 최종 메시지 순서로만 대체한다. 필수 검사 내용과 실행 전 승인 자체는 유지한다. 이 절은 실제 compiler 호출 권한을 아직 발급하지 않는다.

사용자 산출물은 §10.10.11 참가자 판독 보정 후 현재 Window의 새 제한 컴파일 증거다. 현재 Window는120673바이트/SHA 837EF1B4E8004E2F8DE91C63BD6D2533F021AD0CDA1DCC2F3F6DD9BFEE259242이며 v11은 output/chapter1-playable-r1/evidence/b1-final-source-checkpoint-sol61.v11.json,125439바이트/SHA 06A82987CA49D4692B811DECF7BCD8E78565A845C13733AA6A1F368903F802A0이다. 현재590·직전 v10 대비1변경/589불변/추가삭제0와 솔 쓰기 종료·Root 반환을 실제 원장에 결속한다. 루나의 현재 델타 독립 판독과 아스트라 한정 판정은 정적 참가자 조건에 한정하며 동적 GUI/런타임/현재 바이트 컴파일 성공을 선취하지 않는다. r5 실행·성공·실패 이력과 이미 소진된 권한은 그대로다.

담당은 Root의 단일 준비·실행·증거 소유, 루나 gpt-6-luna의 독립 델타/계획·결과 판독, 아스트라의 실제 입력 최종 승인·한정 결과 수용이다. 새 배정은 도구 기본 설정을 사용한다. Root의 실제 설정은 관측한 사실대로 기록한다. 신규 helper6/tool-plan6/SourceQA/QA6/owner4/approval4 파일을 만들지 않는다. 직접 호출의 정확 관리 코드와 입력은 실행 전에 기존 원장의 불변 항목으로 제시하여 독립 판독 대상에 포함한다. 과거 helper5 검수를 새 관리 코드의 검수로 바꾸지 않는다.

**준비 출력과 불변 계획.** Root는 native0으로 현재 입력 계획1을 기존 작업 원장의 새 닫힌 항목에 기록한다. 정확 직렬화 원문 bytes·길이/SHA를 항목 안에 보존하되 계획 자체에 자기 SHA를 넣지 않는다. 가변 원장 전체 SHA·미래 최종 결과 SHA는 입력이 아니다. 계획은 이 승인 개정의 닫힌 접두사 길이/SHA와 절 번호, v11·Window·실제 부모 결과/참조·r5 결과/QA/계획/RSP·현재 소스13/참조318/define146·입력501·compiler 자원12의 실제 핀과 역할을 결속한다. 입력501은 기존 고정 집합의 현재 바이트로 확인하며 변경 Window 핀을 v11로 교체한다. 중복 경로·누락·임의 추가삭제·입력 수 변화는 거절한다. 부모 Input/Movement 개별 성공 재사용·상호 영향과 R3 전체 실패 보존 판정은 그대로 명명한다.

새 출력 접두사는 output/chapter1-playable-r1/project/Library/b1-minimal-csc-g3-observation-r6 정확1이며 실행 준비 전 부재여야 한다. Root만 CreateNew로 디렉터리와 AcadeGameMaker.Combat.Authoring.Editor.rsp 정확1을 준비한다. r5의 실제 RSP에서 -out/-refout의 r5→r6 출력 접두사 두 문자열만 바꾸고 다른 모든 bytes/소스·참조·define 순서를 보존한다. 실제 차이2·생성 RSP 길이/SHA·최종 명령줄과 작업 디렉터리를 계획에 닫는다. 대상은 계획에 핀을 명명한 기존 dotnet 실행 파일과 csc/compiler 자원이며 다른 runtime/compiler/PATH 탐색/서버/설치0이다. r6가 이미 있거나 준비 일부가 실패하면 원문 보존·종료이며 지우기/새 prefix/자동 준비 재호출0이다.

이 단계의 실제 선행은 소스 단독 소유 종료·v11·루나 델타 판독·r5 닫힌 원시와 QA·정확 부모 참조 재사용 근거다. 루나는 같은 묶음에서 실제 RSP/현재501·자원12의 계획 결속과 아래 직접 관리 코드의 소유·시간·원시 조건을 독립 판독하여 P0/P1 및 범위를 기존 원장에 반환한다. 새 문서 파일은 필요 없지만 새 관리 경로를 독립 판독 없이 실행하지 않는다. 이후 Root의 실제 단독 소유·쓰기 종료와 아스트라의 정확 계획 항목 길이/SHA를 지목한 명시 메시지 승인을 같은 원장에 보존한다. 마지막 실제 실행 도구 권한 심사는 별도다. 하나라도 미결이면 native0이다.

**새 단회 실행 경계.** 최종 실제 입력 승인 후 허용될 예산은 Root의 Editor1·단일 직접 호출·최대 native 시작1·전체60초·종료 확인 전체5초·회수 전체5초·첫 실패 중단·자동 재시도0·재개0·두 번째 호출0이다. 이는 r5 권한 재사용이 아닌 새 r6 한정 예산이다. 합산 시계는 실제 실행 작업의 입력 재확인·실행 전 source snapshot을 시작하기 직전의 단조 기준부터 정상 결과 봉인까지 계수한다. compiler 시작 직전 별도 단계 시각도 기록하되 합산 시계를 다시 시작하지 않는다. 준비 계획 생성과 실제 실행 관측을 구별한다. 필요한 일이60초 안 닫히지 않으면 미완료/실패이며 추가 시간을 숨기지 않는다.

Root 직접 관리는 현재 호출 호스트의 System.Diagnostics.Process/ProcessStartInfo로 UseShellExecute=false/CreateNoWindow=true, 실제 핀의 dotnet+csc+정확 RSP1 명령만 사용한다. 생성 시 원래 Process/SafeProcessHandle·PID/StartTime·executable을 보유하고 마지막까지 같은 객체로 실제 정수 exit·종료 상태를 관측한다. 시간 초과/오류 때 그 원래 소유 객체만 Kill·유한 WaitForExit로 정리한다. PID 재조회·프로세스명 전체 종료·Kill(true)·추가 helper/watcher·서버·다른 앱 편입0이다. Process.Start 반환 지연/handle 미확보는 미관측 실패이며 시간 제한만으로 강제 폐쇄를 보장했다고 주장하지 않는다.

stdout/stderr를 독립 비동기 byte 판독으로 동시에 회수하고 기존 helper5의 로그/결과 크기 상한과 첫 실패 조건을 계획에 실제 값으로 명명해 유지한다. source/로그를 무제한 메모리로 적재하거나 문자열 변환으로 원시를 바꾸지 않는다. EOF·최종 byte·원래 handle 종료·각 폐쇄와 실제 회수 시간을 관측한다. 회수/폐쇄 미확인 또는 필수 원시 누락이면 정상 성공/source-after 수용0이다. compiler 자식/관리 오류·nullable exit·부분 생성·실패 원시는 숨기지 않는다.

**정확 실행 원시와 검증.** r6 아래 허용 파일은 준비 RSP1, AcadeGameMaker.Combat.Authoring.Editor.dll/.ref.dll/.pdb와 같은 조립체명의 .stdout.log/.stderr.log, execution-start.json, result.json, source-before.json, source-after.json뿐이다. 결과는 각 파일의 실제 생성 여부/길이/SHA를 구별하고 부재를 성공으로 채우지 않는다. start/result/source 원시는 Root 단독 CreateNew이며 기존 r5 파일을 덮지 않는다. 실행 직전 execution-start.json을 단회 소비 표식으로 예약하고, 시작 전 사실·시각·계획 핀·시도1을 기록한다. 아직 모르는 PID/native 시작/exit는 null이다. 실제 Start 반환 뒤의 PID/시각/handle/exit/종료·회수는 result.json에서 관측한 사실로 결속하며 start를 덮어쓰지 않는다. 기존 표식/결과가 있으면 실행0이다.

source-before/after는 기존 Assets/Packages/ProjectSettings 전체590을 실제 실행 경계에서 기록하고 v11의 경로별 완전 집합·길이/SHA·중복0·변경0으로 대조한다. 단순 이전 manifest 복사로 실제 snapshot을 대신하지 않는다. source-after는 모든 시작 프로세스 종료/출력 회수 확인 후에만 생성하고, 확인 불가하면 부재/실패 이유를 result에 남긴다. 독립 판독자가 같은 전체590 물리 재해싱을 반복할 필요는 없으며 실제 두 snapshot·현재 Window·영향 입력과 기존 독립 증거를 대조한다. 원시 result의 accepted/independentAcceptance는 false이며 후속 루나 결과 판독과 아스트라 최종 메시지를 같은 원장으로 결속한다.

REQ/AC-M1COMP-R1-001/003/004/005와 기존 REQ/AC-C1B-G3OBS·REQ/AC-PROC-011/012를 추적한다. 준비1·필수 읽기 대조·독립 실제 계획 판독1·최종 승인1·실행1·독립 결과 판독1·최종 메시지가 같은 묶음의 유한 검사다. 범위 내 비실행 보정은 같은 원장에 원인/최소 재현/변경/목적을 누적하되 실행 소비 뒤 추가 호출이나 필수 검사 면제를 만들지 않는다. 오류 반복·권한/범위 확장은 원문 보존 후 다시 판정한다. 종료는 Root의 닫힌 원시/독립 결과/한정 수용 또는 실패 반환이다.

이 절 작성 시 native/compiler 실행 권한은0이다. Runtime·Unity·GUI·Play·진단·프로필·방 수용·SourceReady·B1/CH1은 계속 미승인이다. 인간 결정 보류와 G3 실제 관문·필수 회귀·C1C 선행·Q0·키/세대·9회·원격 관문은 그대로다. M3 구문 검사 성공은 이 M1 실행 승인이나 도구 능력 수용을 대신하지 않는다. 이번 아스트라 문서1 작업은 접두사 보존·최종 길이/SHA 확인 뒤 닫고 쓰기 소유를 Root에 반환한다.

##### 10.10.12.1 r6 신규 바이트 상한과 역사 기술 정정 — Approved

2026-10-13 실제 아스트라 gpt-6-astra가 도구 기본 설정으로 아래 native0 준비 보정만 Approved로 채택한다. 기존137949바이트/SHA EB679836AC5D7FA656020442C715E45B72A708F62DEFB09750985C8C0E43E138 접두사는 보존한다. §10.10.12의 '기존 helper5의 로그/결과 크기 상한'이라는 설명은 오류다. 실제 helper5 65073바이트/SHA 763CA268ADCB11CFFD5DF3EE3820F484CEFD1A01B552943FF5DBDC7BEA82EEC6은 stdout/stderr의 CopyToAsync 직접 파일 회수이며 명시 바이트 상한을 두지 않았다. 이를 과거에 존재한 보장으로 표시하지 않는다. 당시 원문·실행·검수는 그대로 보존한다.

r6 직접 관리에는 다음 새 유한 상한을 적용한다. MiB는1048576바이트이며 각 수치는 실제 UTF-8/원시 출력 bytes 기준이다. 정확 값과 검사 지점을 계획 및 독립 판독에 포함한다.

| 항목 | 새 상한 |
| --- | --- |
| stdout.log, stderr.log | 각각1MiB, 두 독립4096바이트 ReadAsync 펌프와 파일 단일 작성자 |
| result.json, source-before.json, source-after.json, execution-start.json | 각각1MiB, 개별 JSON 문서와 끝 개행을 포함한 총량 |
| 기존 원장에 결속하는 계획 원문, 직접 관리 코드 원문 | 각각2MiB, 고정 원문 bytes·길이/SHA. 원장 전체나 기존 역사 자료 전체에 이 상한을 소급 적용하지 않음 |

로그는 읽은 chunk를 쓰기 전에 남은 용량과 대조한다. 한도를 넘는 첫 chunk는 잘라 정상 기록하지 않고 최초 실패로 남기며 해당 chunk 전체를 기록하지 않았다는 수와 실제 읽은 수를 보존한다. 저장 상한 준수가 생산된 모든 출력의 보존을 뜻하지 않는다. 초과 후에도 같은5초 종료/5초 회수 안에서 두 스트림을 동시에 유한 배출할 수 있으나 버린 byte와 EOF 여부를 구별하고 completeOutputCaptured=false 및 전체 성공=false로 닫는다. 무제한 메모리 적재·추가 로그·다른 접미사·재시도·시간 연장0이다.

JSON은 실제 직렬화 bytes의 한도와 길이/필드 수를 기록 전에 확인한다. 무한 컬렉션이나 오류 문자열을 먼저 적재한 뒤 크기를 재는 방식은 허용하지 않는다. 고정590행 source·명명된 입력/경로·단계별 오류의 유한 개수와 문자열 제한을 계획에 명명한다. source JSON이 한도를 넘으면 부분 내용을 정상 snapshot으로 쓰거나 데이터를 삭제해 성공시키지 않는다. 부재/부분 원문과 최초 크기 실패를 보존한다. result의1MiB 안에서64KiB를 최초 실패·후속 정리·미관측/불완전 원시 설명에 예약하고 정규 내용은 나머지 범위 안에 둔다. 예약은 별도 파일/추가 예산이 아니며 result 자체 폐쇄 실패도 성공으로 채우지 않는다. 계획/관리 코드2MiB 초과는 실행 전 거절이며 임의 생략/축약으로 같은 지문을 주장하지 않는다.

새 상한은 직접 실행 코드의 독립 판독 대상이며 기존 검사 성공을 대신하지 않는다. r6 단회60초·종료5초·회수5초, 정확 파일/소유·입력/권한 및 실제 최종 메시지 승인 선행은 그대로다. 이번 보정은 실행 승인이 아니며 native0을 유지한다. REQ/AC-M1COMP-R1-004/005와 REQ/AC-PROC-012의 유한 관리·실패 보존을 구체화한다. 문서1 덧붙임·접두사/최종 지문 확인으로 작업을 닫고 쓰기 소유를 Root에 반환한다.

##### 10.10.12.2 r6 첫 실패 보존과 스트림 매개변수의 native0 복구 준비 — Approved

2026-10-13 실제 아스트라 gpt-6-astra가 도구 기본 설정으로 아래 준비만 Approved로 채택한다. 기존141565바이트/SHA D348A469F7E192B21B00EA43756D64FA105D313E9B92221D365614B82175E875 접두사를 보존한다. Root가 반환한 r6 실패는 compiler 실제 시작1 뒤 NewPump의 [IO.Stream]$input 매개변수가 PowerShell 자동 변수와 충돌해 첫 회수 펌프 생성 전에 실패한 것이다. 보고된 원래 PID78780의 종료는 확인됐지만 exit=-1, drain=false, sourceAfter=false이며 실행 도구의 JSON 반환 exit0은 컴파일 성공이 아니다. result2045바이트/SHA AC1504AB931C583640520EF86A66377419FD70BBED909E2569934EDA2B6DEFD5 및 RSP/start/result/source-before 원문과 소비된 계획·코드·승인을 보존한다. 실제 실패의 독립 판독은 루나가 별도로 닫으며 이번 원인 보정이 그 판독을 대신하지 않는다.

r6 권한은 소진됐고 결과 수용0·재시도/재개/두 번째 호출0이다. 구현과 실행 전 정적 검토가 예약 변수 충돌을 놓친 사실을 기존 원장에 남긴다. 이미 종료된 프로세스에 다시 접근하거나 누락된 로그/source-after를 사후 생성하지 않는다. 기존 r6 원시·계획·관리 코드·종료 사실을 덮어쓰지 않는다.

사용자 산출물은 다음 실제 계획 검토에 제출할 관리 코드의 정확 매개변수 보정과 최소 원인 확인이다. Root는 기존 원장에 새 불변 후보 항목을 작성할 수 있으며, 변경은 NewPump 선언의 $input을 $sourceStream으로 바꾸고 그 매개변수의 실제 참조만 같은 이름으로 바꾸는 데 한정한다. 펌프 상태 객체의 input 키와 그 소비 $s.input, 경로/크기/시간/소유/실행 로직은 별개이므로 전역 문자열 치환하지 않는다. 함수 밖 자동 $input을 건드리거나 다른 동작·오류 처리를 이 보정에 섞지 않는다. 이전/이후 정확 코드 bytes·길이/SHA와 문자 델타를 같은 원장에 보존하며 새 helper·계획 파일·새 출력 prefix/RSP 생성은0이다.

국소 검사 예산은 현재 호출 호스트 안의 비네이티브 최소 재현 정상1·거절1, 전체10초 이내 단1묶음이다. 닫힌 후보의 정확 [IO.Stream]$sourceStream 선언·참조를 분리한 인메모리 검사에서 작은 MemoryStream의 알려진 bytes를4096바이트 이하 버퍼의 ReadAsync로 읽고 실제 읽은 수/bytes·EOF를 대조한다. 음성은 비스트림 객체1을 같은 형식 매개변수에 전달해 실제 바인딩 거절을 확인한다. 각 자료 최대4096바이트, 완료 대기는 남은 시간 안으로 제한하고 소유 MemoryStream만 닫는다. 실제 FileStream/파일 생성·NewPump 전체 파일 writer·관리 코드 전체·compiler·새 PowerShell/Python 프로세스·Add-Type·Unity·GUI·프로필은0이다. 이 최소 재현은 매개변수 바인딩과 메모리 읽기의 확인이며 실제 프로세스 배관/파일 회수 성공을 입증하지 않는다. 최초 예상 밖 실패에서 나머지0·자동 반복0이며 원문/원인/변경/새 관측 목적을 기존 원장에 반환한다.

Root의 코드 준비 종료·원장 소유 반환 뒤 루나는 보정 델타·실제 국소 결과와 실패 원문을 같은 묶음으로 독립 판독한다. 새 compiler 권한/예산·새 prefix·준비 호출은 아직0이다. 이후 실제 새로운 입력 계획과 원시 경로·유한 예산을 정확히 명명하고 독립 판독·아스트라 최종 메시지·실제 도구 권한 심사를 거쳐야 하며 이 복구 준비를 다음 실행 승인으로 사용하지 않는다. 기존60/5/5와 필수 원시/소유/입력 검사를 약화하지 않는다.

REQ/AC-M1COMP-R1-004/005 및 REQ/AC-PROC-011/012의 첫 실패 보존·최소 재현·유한 보정 원칙을 적용한다. Runtime·GUI·Play·진단·SourceReady·방·B1/CH1 수용은 계속0이며 독립 M2 설계 조사는 이 실패를 선행으로 삼지 않는다. 이번 아스트라 작업은 문서1 덧붙임·접두사/최종 지문 확인으로 닫고 쓰기 소유를 Root에 반환한다. 이번 작성자는 국소 검사·코드/원장 수정·실행을 수행하지 않는다.

##### 10.10.12.3 최소 재현 함수 범위 보정의 별도 단회 확인 — Approved

2026-10-13 실제 아스트라 gpt-6-astra가 도구 기본 설정으로 아래 native0 국소 확인만 Approved로 채택한다. 기존145944바이트/SHA C7C37CE4D25814BE2EFA3E0561610EAA52B12C8E0E4C488A1926F4F0DDE7FBED 접두사를 보존한다. §12.2의 첫 검사에서 '& $testBlock'이 자식 범위에 함수를 정의한 뒤 부모 범위의 NewPump 호출이 함수 미발견으로 실패했다는 Root 원시를 보존한다. 경과1.3104053초·음성0·반복0·MemoryStream 폐쇄true·native0이며 예약 변수 보정 성공을 뜻하지 않는다. 첫 단회 검사 예산은 소진됐다.

현재 관리 코드 후보12569바이트/SHA 2D7F19BD7E6A329BA66FBDCAD6A2C78D162E3B6E996615FE6075C407ADD7DB8E 및 기존 실패 원문은 변경하지 않는다. Root가 새 불변 검사 항목에서 바꿀 수 있는 부분은 최소 재현 하네스의 '& $testBlock'을 '. $testBlock'으로 바꾸는 정확1곳뿐이다. 직전의 인메모리 함수 본문·입력·판정은 그대로 재사용한다. 여기서 점 소싱 대상은 그 닫힌 검사 함수 정의 블록뿐이며 제품/파일/전체 관리 코드·NewPump 실제 파일 writer를 점 소싱하거나 실행하지 않는다. 정의 블록 안의 다른 최상위 동작이나 경계 밖 함수가 발견되면 실행0으로 반환한다.

새 관측 목적은 보정 매개변수의 실제 바인딩, MemoryStream의 ReadAsync 실제 수/bytes/EOF와 비스트림 객체의 바인딩 거절이다. Root가 현재 호스트 안에서 정상1·거절1, 전체10초 이내의 별도 단1묶음만 수행한다. 자료 최대4096바이트·남은 시간 안 대기·소유 메모리 스트림 폐쇄 조건은 §12.2 그대로다. 실제 native/새 프로세스·컴파일·새 prefix/RSP·파일 IO·Unity·GUI·프로필은0이다. 최초 예상 밖 실패에서 나머지0·자동 반복0이며 이번에도 실패하면 또 재호출하지 않는다.

이 승인으로 역사 검사 합계는 최초 실패1묶음과 새 확인 최대1묶음이다. 최초 실패를 삭제하거나 같은 시도의 재개로 세지 않는다. 실제 함수 정의/검사 원문과 변경1·경과·정상/음성 각각 실행 여부·오류·폐쇄를 기존 Root 원장에 결속하고 독립 판독에 반환한다. 성공하더라도 메모리 바인딩 확인만 수용하며 실제 프로세스 배관/파일 회수·새 compiler 실행은 미승인이다. REQ/AC-M1COMP-R1-004/005 및 REQ/AC-PROC-012를 따른다. 이번 문서1 작업은 접두사/최종 지문 확인 후 닫고 쓰기 소유를 Root에 반환한다.

##### 10.10.12.4 예약 변수 보정 뒤 r7의 native0 입력 준비 — Approved

2026-10-13 실제 아스트라 gpt-6-astra가 도구 기본 설정으로 아래 r7 준비만 Approved로 채택한다. 기존148642바이트/SHA 46A6696A8E5758F44E566F8494FDB98E3C52141764F3CAA15260979C148DFC58 접두사를 보존한다. r6 실패·소진 권한, 두 최소 재현 하네스 실패와 별도 음성 관측의 실제 원문/횟수는 그대로다. 보고된 정상6바이트·EOF0·MemoryStream 폐쇄 및 별도 음성의 정확 ParameterArgumentTransformationError,NewPump/InvalidData·정상반환false는 메모리 바인딩 근거이며 실제 compiler 배관 성공이 아니다.

다음 관측 목적은 정확 예약 변수 보정의 실제 compiler 배관·폐쇄·바이트 상한과 현재 Window 컴파일이다. 코드 후보는12569바이트/SHA 2D7F19BD7E6A329BA66FBDCAD6A2C78D162E3B6E996615FE6075C407ADD7DB8E로 고정한다. 루나의 해당 후보·국소 관측·실패 원문 독립 판독이 닫히고 실제 반환된 결론을 기존 원장에 결속한 뒤 Root가 아래 준비를 단독 수행한다. 독립 결론이 미발급/불충분하면 계획 설명 읽기는 가능하나 새 prefix/RSP 작성0이다.

새 허용 prefix는 output/chapter1-playable-r1/project/Library/b1-minimal-csc-g3-observation-r7 정확1이다. Root는 그 부재와 재분석 지점/루트·현재 소스/도구 작성 소유 종료를 먼저 확인한다. native0으로 해당 디렉터리와 AcadeGameMaker.Combat.Authoring.Editor.rsp 정확1을 CreateNew 준비할 수 있다. r6 실제 RSP의 -out/-refout 두 출력 문자열에서 r6→r7만 변경하고 나머지 bytes를 보존한다. 원문 r6 RSP/실패 원시를 수정하지 않는다. 준비는 단1회이며 기존 경로·입력 차이·부분 실패에서 보존 후 종료, 자동 재호출·삭제·새 접미사0이다.

기존 원장의 새 불변 r7 항목에 계획 원문 bytes/길이/SHA, 보정 코드 원문/핀, 실제 r7 RSP와 차이2, v11·현재590/변경 Window·입력501·자원12·소스13/참조318/define146, 부모 Input/Movement 개별 성공 재사용·상호 영향, r5 성공QA 및 r6 실패·하네스 실패/실제 국소 결과·루나 결론을 결속한다. 소스 v11은125439바이트/SHA 06A82987CA49D4692B811DECF7BCD8E78565A845C13733AA6A1F368903F802A0이며 Window120673바이트/SHA 837EF1B4E8004E2F8DE91C63BD6D2533F021AD0CDA1DCC2F3F6DD9BFEE259242다. 실제 입력이 바뀌면 이 고정 준비로 진행하지 않는다. r6 실패 후 생성물/과거 권한은 새 성공 근거나 미소비 권한으로 바꾸지 않는다.

§12/12.1의 직접 관리·소유·불변 항목·독립 실제 계획 판독·아스트라 최종 입력 메시지·실제 도구 심사 순서와 바이트 상한을 r7에도 그대로 적용한다. 계획/코드 각각2MiB, 로그/JSON 각각1MiB 및 결과 실패예약64KiB, 실제 source-before/after590·원래 Process/SafeHandle·동시 회수·원시 accepted=false를 유지한다. 새 helper/계획QA/소유/승인 사본 파일0이다. 실행 시 허용될 원시 파일 집합은 §12의 정확 이름을 r7 prefix에 한정한 동일 집합이며 지금 생성 허용은 RSP1뿐이다.

r7 계획에 명명할 후보 실행 예산은 Editor1·native최대1·합산60초·종료5초·회수5초·최초 실패 중단·자동 재시도0·재개0·두 번째 호출0이다. 현재 nativeExecutionAuthorized=false이며 본 절은 해당 실제 실행을 승인하지 않는다. 닫힌 실제 r7 계획·코드·소유·루나 독립 판독에 대한 아스트라의 별도 최종 메시지 전 compiler0이다. r6 소진 예산을 재개하거나 원래 승인 메시지를 재사용하지 않는다.

REQ/AC-M1COMP-R1-001/003/004/005와 REQ/AC-PROC-011/012를 추적한다. 준비 종료는 실제 입력/차이·미실행·계획 닫힘·Root 단독 소유의 기존 원장 반환이다. Runtime·Unity·GUI·Play·진단·프로필·SourceReady·방·B1/CH1 및 필수 인간 결정/제품 선행은 변경하지 않는다. 이번 아스트라 작업은 문서1 덧붙임·접두사/최종 지문 확인 후 닫고 쓰기 소유를 Root에 반환한다. 이번 작성자의 prefix/RSP·코드·원장 쓰기와 실행은0이다.

같은 §12.4의 정확 추가 허용은 새 r7 불변 코드 항목에서 result.kind 상수 'r6-direct-editor-single-compile' 한 곳을 'direct-editor-single-compile'로 바꾸는 메타데이터 정정1이다. 기존 r6 및12569바이트/2D7F19BD7E6A329BA66FBDCAD6A2C78D162E3B6E996615FE6075C407ADD7DB8E 복구 후보 원문은 보존한다. 위 고정 후보를 출발 입력으로 삼고 그 외 실행/오류/시간/파일/인수 동작은 변경하지 않는다. 새 실제 코드 길이/SHA는 작성 종료 후 r7 계획·소유·독립 판독·최종 승인 메시지에 결속한다. 메타데이터 정정은 과거 실패의 이름을 바꾸거나 새로운 실행 권한을 발급하지 않는다. 루나가 기존 보정과 이 상수1 델타를 포함해 준비 선행의 독립 판독을 닫기 전 prefix/RSP 작성0은 그대로다.

##### 10.10.13 사용자 직접 플레이의 게임 수신 입력 관측 소스2 준비 — Approved

2026-10-13 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 아래 정확2 소스의 준비 구현과 유한 정적 검사만 Approved로 채택한다. 기존153798바이트/SHA256 `0FADD89754E31418A180D1A60935F240587A994E41A9F553D12B7C71FB407C6B` 접두사를 보존한다. 사용자 직접 플레이 정책은 기존 native-capture 결정문서39092바이트/SHA256 `300596B7F7868E5F3C198A364E7C57310AD6EAB2BBC6E9EA66965D8ECBE50AFB`의 마지막 Approved 부록이다. 역사 G3 원문·실패·소진 승인·v11 및 r8 한정 컴파일 증거를 수정하지 않는다.

**사용자 산출물·담당·종료:** 사용자가 실제 플레이하며 Unity가 받은 지정 입력과 tick/화면을 구별해 기록할 수 있도록 관측 소스를 준비한다. 솔 gpt-6.1-sol은 정확2파일 단일 작성자, Root는 인계·원장·후속 실행 소유, 루나 gpt-6-luna는 작성 종료 후 한 묶음 독립 판독, 아스트라는 정확 규범/최종 수용 담당이다. 새 추론 상향 override는 없다. 종료는 코드 차집합·각 길이/SHA·국소 검사와 미입증 경계의 기존 원장 인계다. 구현자는 자기 결과를 독립 수용하지 않는다.

**정확 쓰기 범위:** 제품 루트 `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/` 아래 다음 두 기존 파일만 허용한다.

- `Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs`: 기준39505바이트/SHA256 `DD5B06F63207B62AD05C97B848AF6819FFA127D8C454C129075F9BA23393C9D6`.
- `Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs`: 기준120673바이트/SHA256 `837EF1B4E8004E2F8DE91C63BD6D2533F021AD0CDA1DCC2F3F6DD9BFEE259242`.

meta·asmdef·패키지·새 C#·게임플레이·다른 producer·기존 도구/원시/승인 파일 쓰기0이다. Observer의 UNITY_EDITOR 경계를 유지하며 Runtime 조립체에서 UnityEditor를 참조하지 않는다. InputSystem은 현재 Input.Unity의 기존 참조를 사용한다. 기존 tick 관측의 +210/생산자+200·게임 진행/저장 권한은 그대로다. 독립 연속 창 영상의 수단 설계는 별도이며 이 소스 준비의 선행이 아니다. 실제 GUI 수용에는 여전히 해당 원문이 필요하다.

**Actor와 승인 입력:** 새 실제 실행 승인은 owner=`Root`, performer=`User`, performerModel은 정확 빈 문자열, performerRole=`사용자 직접 플레이`, inputEvidenceKind=`UnityReceivedAllowedControls`로 한정한다. 이 값은 명시 존재/정확 자료형을 요구하며 null/누락/공백/다른 역할을 대체값으로 받지 않는다. 기존 Window의 실행·진단·Apply 관련 performer 판독과 Observer 설정 검사는 해당 현행 경로에 한해 일관되게 보정한다. 승인 participant의 모델/기본설정·역할, 원문 SHA·소유·키·세대·원시 핀·진단 별도 권한을 약화하지 않는다. 과거 Root performer 승인을 새 입력 기록 실행으로 재사용하지 않는다.

**GameView 신원의 정확 표현:** 승인 원문의 새 `inputGameViewWindowId`는 기존 UInt64Integer와 같은 엄격 UInt64 JSON 숫자 판독으로 읽고 0을 거절한다. Configuration에는 ulong 필드를 추가하지 않고 `gameViewInstanceIdDecimal` 문자열로만 전달한다. ASCII 십진 숫자만, 선행0·부호·공백·지수·소수 금지, UInt64 파싱 성공 및 InvariantCulture ToString과 원문 완전일치를 요구한다. 실제 bridge가 읽은 EntityId.ToULong(GetEntityId())와 정확 비교한다. 기존 Configuration 식별2+Sample 식별8, 10필드/230진단·반례·왕복 자료형/의미는 변경0이다. 새 문자열 검사는 별도 좁은 입력검사이며 그 230개를 늘리거나 성공으로 간주하지 않는다. 기존 strict parser6의 중복/자료형/누락 거절을 보존하고 현행 투영에 위 명명 필드만 추가한다.

**Editor와 Observer 연결:** Observer 파일 안에 작은 internal 읽기 전용 `IReceivedInputContext`와 불변 상태를 두고 Window가 구현하는 경계를 허용한다. 역할은 실제 동일 GameView 신원·현재 게임 포커스·등록 세션의 유효성을 반환하는 것뿐이다. 초기 실제 객체는 Application.isFocused와 EditorWindow.focusedWindow의 실제 참조에서 결속하며 window ID와 대조한다. 타입명 추정·반사·창 자동검색/생성·Focus 호출·타창 제목/내용 읽기0이다. 세부 internal 메서드/상태 필드명은 이 역할에 한정하여 솔이 구현 차집합에 명명할 수 있으나 추가 제품 권위·공개 API·다른 파일 caller를 만들지 않는다. bridge는 같은 세션·관측기·실제 객체의 원본 신원을 보유하며 다른 창이나 새 객체로 자동 교체하지 않는다.

최초 준비에서 실제 GameView 객체를 검증한 뒤 관측기에 bridge를 결속한다. AddComponent에 따른 구성 전 OnEnable은 구독/원시 생성0으로 처리하고, Configure 완료 후 같은 초기 경로에서 준비 등록을 한 번 수행할 수 있다. 재로드된 구성은 Observer.OnEnable에서 첫 이벤트/틱 처리 전에 설정과 현재 bridge를 획득·검증하는 경로를 둔다. Window.OnEnable/EnteredPlayMode의 나중 등록만으로 최초 이벤트 이전 확보를 보장했다고 주장하지 않는다. 재로드 후 bridge 누락·중복·신원 변경·등록 지연을 발견하면 불완전 상태를 고정하고 대기 후 재개/자동 재설치0이다. 미설정 컴포넌트의 무동작과 이미 구성된 관측의 실패를 구별한다. Start는 준비 성공 여부를 확인한 뒤에만 기존 기록/live 전이를 진행한다. 첫 수신 전에 파일/시각/소유가 준비되지 않았다면 이벤트를 버퍼에 숨겨 정상 시작으로 보충하지 않고 불완전으로 닫는다. 실제 callback/Start/첫tick/DomainReload 순서의 완전성은 아직 미입증이며 최초 R01-A에서 확인한다.

**새 onEvent 경계:** InputSystem.onEvent의 읽기 전용 구독·해제를 이번 두 파일 범위의 새 callback으로 명시 허용한다. payload/control 값을 읽기 전에 같은 bridge 세션·실제 창 ID·게임 포커스·관측 유효성을 확인한다. 실패 시 payload 읽기0, 첫 실패 기록/폐쇄이며 포커스 복귀 자동 재개0이다. onEvent 등록/해제는 한 소유자만 수행하고 Seal/OnDisable/OnDestroy/재로드 경계에서 중복 해제에 안전해야 한다. event.handled 변경·Update 호출·QueueEvent·게임 상태 setter·가상 장치·키 주입0이다.

허용 control은 A/D/Space/LeftShift/RightShift/Q/E/MouseLeft/MouseRight 정확9개다. 정책의 논리 Shift 하나를 실제 좌우2로 분리한 것이며 다른 입력 범위 추가가 아니다. 허용 control만 상태/변화를 읽고 전체 이벤트/장치 payload/다른 키·문자·텍스트를 복사·열거 기록하지 않는다. 첫 상태는 unknown이며 관측된 초기 값과 그 뒤 변화, 알려지지 않은 이전 상태를 구분한다. delta 사건에서 해당 control 값이 제공되지 않는 경우와 읽기 오류를 구별하고 어느 경우도 release=false로 조작하지 않는다. 읽기 오류·장치 변경/제거로 신뢰가 깨지면 미완료 종료한다. 다른 장치·허용 control을 포함하지 않는 사건은 그 payload를 해독하지 않는다. 상태 표본을 OS 무손실 사건이라고 기록하지 않는다.

**입력 원시·스키마·폐쇄:** 기존 inputEvidencePath를 새 수신 JSONL의 단일 writer 경로로 사용하고 원시 경로를 늘리지 않는다. samples/events/seal/nativeEvidence/inputEvidence 다섯 경로는 정규화 후 pairwise 비충돌, 루트·재분석 지점·CreateNew 및 기존 핀 정책을 만족해야 한다. Window와 Observer 양쪽에서 충돌을 거절한다. 입력 writer는 소유한 stream만 닫으며 native 영상 파일의 writer는 소유하지 않는다. 입력 총8MiB·행32KiB 이하와 기존 사건8MiB를 각각 넘기지 않는다. 상한 초과·쓰기/flush/폐쇄 실패는 첫 실패다.

수신 행은 별도 schemaVersion과 kind, session/case, inputEvidenceKind, performer, 단조 시각/UTC·event 시각(서로 의미 구별), 실제 마지막 관측 tick, control 이름, 이전값 known 여부/이전값, 현재값 known 여부/현재값, 변화 여부, 등록/포커스 유효 상태를 명시하는 한정 DTO로 둔다. 초기 unknown을 숫자0/false 상태로 해석하지 않도록 known 필드를 필수로 한다. 이벤트 원시와 tick 실제 소비를 같은 증거로 합치지 않는다. 등록·불완전·종료 행과 봉인에는 실제 구독/해제·손실/오류·행수/바이트·폐쇄 여부 및 inputEvidence 파일의 길이/SHA를 결속한다. 정확 필드명/자료형은 솔 제출 차집합에 전부 명명하고 Window의 해당 strict 판독과 함께 닫는다. 이 범위 밖 임의 확장 필드나 승인 parser 완화는0이다. 핀은 실제 flush/폐쇄 뒤 발급하며 미래 폐쇄 성공을 본문에 예측하지 않는다. 실패한 입력 폐쇄/핀을 정상 completionKey나 Apply 수용으로 넘기지 않는다. 기존 SealRow/ReadSealRow 및 Apply의 필요한 입력 핀 확인만 함께 보정하고 독립수용 false는 유지한다.

**검사·후속 컴파일·실행 한계:** 이번 허용 검사는 파일 읽기 기반 국소 정적 검사1묶음, 그 범위 안 수정에 대한 재읽기 최대2회다. Actor exact값·문자열ID 정규성·5경로 충돌·9control 외 읽기0·등록/폐쇄·unknown/오류 처리·parser/schema 짝·10/230 보존·금지 호출/파일 차집합을 한 번에 확인한다. 새 검사 프레임워크·실제 CLR/Unity/InputSystem/API 호출·진단230 실행·native·GUI·Play·컴파일0이다. 예상 밖 실패는 원문/원인/변경/새 관측 목적을 기존 원장에 남기며 유한 재읽기를 실제 실행 재시도로 확대하지 않는다.

새 interface/Observer는 Input producer와 Editor consumer에 영향을 준다. 작성 종료 뒤 Root가 새 소스 체크포인트·정확 입력과 영향 분석을 닫고, 현재 Movement r3의 실제 소스/ref 계보 재사용 가능성을 확인한 뒤 Input→Editor 최소 컴파일2의 새 출력·핀·시간·소유 계획을 별도로 준비한다. 과거 Input r4 또는 Editor r8 ref를 새 ABI 성공으로 대신하지 않는다. 지금 새 helper/RSP/실행 권한은0이며 정확 계획·독립 판독·아스트라 최종 입력 승인·도구 심사 전 호출0이다.

기존 Play10·각1800초/총18000초·종료5/회수5·샘플108001/128MiB·native2GiB, 최초 R01-A 확인·추가 예비 Play0·첫 실패 뒤 나머지0·Q0·키/세대·필수9회·B1 FINAL 공유 통합 선행은 불변이다. REQ/AC-C1B-G3OBS-005/006/007/010/012, REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012를 추적한다. 실제 등록 시점·게임 수신·tick 소비·연속 화면·Runtime/SourceReady/B1/CH1은 미수용으로 남긴다. 이번 아스트라 작업은 문서1 append·접두사 확인 후 소유 반환이며 코드/원장 쓰기와 실행은0이다.
같은 §10.10.13의 제출 설계 확정: bridge의 정확 최소 서명은 `IReceivedInputContext.TryReadReceivedInputContext(out ReceivedInputContext context)`와 `BindReceivedInputContextForAuthoring(IReceivedInputContext context)`다. 상태는 sessionId/caseId/gameViewInstanceIdDecimal 및 isPlaying/isPaused/isCompiling/isUpdating/gameViewFocused의 get-only 값만 포함한다. Bind는 pristine 관측기에 같은 승인 tuple로 단1회, 비직렬화 참조를 결속한다. paused/컴파일/가져오기 및 포커스 경계에서 payload 읽기를 허용하는 것으로 해석하지 않는다.

수신 행의 위 의미를 구현할 정확 압축 표현은 schemaVersion=1, kind=`received-input`, sessionId, caseId, inputEvidenceKind, performer, receivedOrdinal(Int64), observedStopwatchTicks(Int64), stopwatchFrequency(Int64), eventTime(유한 double), lastCompletedTick(Int64, 미관측 -1), gameViewInstanceIdDecimal, deviceKind(`keyboard` 또는 `mouse`), controlMask/pressedMask/knownBeforeMask/changedMask(Int32)다. 비트 순서는 A,D,Space,LeftShift,RightShift,Q,E,MouseLeft,MouseRight의0~8이며 범위 밖 비트0이다. controlMask는 이번에 실제 읽힌 control, pressedMask는 그 안의 현재 눌림, knownBeforeMask는 그 안의 이전 known, changedMask는 이전 known이며 실제 값이 달라진 control만 나타낸다. 초기 knownBeforeMask=0의 최초 pressed를 release→press로 소급하지 않는다. 이전 상태는 같은 세션의 실제 이전 행에서만 추적하고 mask 밖 미관측값을 release로 읽지 않는다. 등록/실패/종료 행의 UTC·오류/상태와 위 단조 수신 행을 결속한다. 이는 위 개별 known/값 설명의 정확 mask 표현이며 별도 무제한 배열/행 확장이 아니다. delta 미포함 control의 ReadValueFromEvent=false는 상태 변경0, 정상 StateEvent의 해당 control 읽기 불능·잘못된 format은 최초 불완전 실패다. 장치 불일치는 다른 장치의 payload 해독으로 복구하지 않는다.
초기 등록의 정확 경계 보정도 같은 준비 묶음에 포함한다. 구성된 Observer.OnEnable은 onEvent와 payload를 읽지 않는 onBeforeUpdate를 단1회 등록한다. OnEnable에서 bridge가 아직 없다는 사실만으로 정상 재로드를 즉시 실패로 확정하지 않고, **최초 Play 입력 onBeforeUpdate 또는 더 먼저 도달한 onEvent/첫 tick 경계 중 가장 이른 시점**까지 실제 결속과 input writer 준비 완료를 요구한다. Window는 기존 OnEnable/EnteredPlayMode 경계에서만 같은 보유 GameView 원본·승인 tuple·collector를 확인해 최초 Bind를 할 수 있다. 이 유한 초기 경계 전에만 결속 가능하며 경계에서 미완료이면 영구 불완전으로 닫고 이후 Bind/대기/자동 재등록0이다. 이는 앞선 누락/등록 지연 실패의 정확 판정 시점이며 실패 후 재개 허용이 아니다. 성공 Bind 뒤 반복 Bind0, update 폴링·창검색0이다. 새 onBeforeUpdate는 위 준비 확인 이외의 게임/API 진행 권한이 없다.

input writer는 승인·5경로 검증 후 첫 입력 경계 전에 CreateNew로 열고 등록 행을 쓴다. samples/events의 기존 Start는 유지한다. 그보다 앞선 적법 수신 행에는 lastCompletedTick=-1 및 sampleCollectorLive=false를 명시하며 뒤 tick을 소급 배정하지 않는다. 수신 행의 추가 정확 필드는 sampleCollectorLive(bool) 하나다. Start 이후에도 실제 표본이 없으면 lastCompletedTick=-1을 유지한다. 새 callback 둘 모두를 봉인/비활성/파기에서 해제한다. 이 순서의 실제 성립과 손실 없음은 정적 준비의 성공 주장이 아니며 최초 R01-A 확인 전 sourceReady=false다.
##### 10.10.14 대상 Unity 창 연속 영상의 단일 소스 준비 — Approved

2026-10-13 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 아래 **실행 없는 소스1 준비만 Approved**로 채택한다. 기존168867바이트/SHA256 `337588810C23BA9AAAA245E0C1F9F655E02368EDD88F6F5938E989716295F251` 접두사와 동결 원문을 보존한다. 사용자에게 필요한 결과는 직접 플레이의 실제 대상 창 연속 화면 원문이다. 기존 읽기 관측 허가 안의 영상 수단이며 키 기록/조작·전체 바탕화면·다른 앱 범위를 추가하지 않는다. 게임 수신 입력 source2 준비/후속 컴파일과 독립 진행하고 별도 문서 사본을 만들지 않는다.

**소유·정확 경로·종료:** 솔 gpt-6.1-sol이 `output/chapter1-playable-r1/tools/b1-window-video-capture.v1.cpp` 정확 신규 소스1을 작성한다. 현재 경로 부재를 확인한 뒤 최초 생성1, 같은 파일 안의 이 범위 구현 보정만 허용한다. 다른 소스·header·project·build script·실행파일·QA framework·제품·원장·설치 파일 생성0이다. Root는 기존 작업/참여 원장 단독 소유와 향후 원래 프로세스 실행 소유, 루나는 코드 묶음 독립 판독, 아스트라는 정확 실행/최종 수용 담당이다. 준비 종료는 제출 코드·길이/SHA·한정 정적 판독·미입증 기능을 Root에 반환하는 시점이다. 같은 범위 보정은 원장에 누적하며 작은 수정마다 승인 사본을 만들지 않는다.

**근거와 최소 선택:** 솔의 공식 문서/현지 SDK 읽기 설계는 CreateForWindow(HWND), CreateFreeThreaded의 BGRA 프레임풀2, D3D11 CPU staging, Media Foundation RGB32→H.264/MP4 및 `MFCreateSinkWriterFromURL(NULL, boundedIMFByteStream, attrs)`의 MPEG4 컨테이너 지정을 후보로 확인했다. 기존 설치 MSVC/SDK 파일 존재는 보고됐지만 빌드/encoder/권한/GPU/종료 성공의 증거가 아니다. MP4는 기존 규범의 필수 포맷이 아니라 즉시 판독 가능한 최소 후보로 채택한다. 미지원일 때 새 codec·앱 설치·다른 캡처 API·PNG 대체 경로로 자동 전환하지 않는다.

**실행 입력 설계:** 실제 실행 전 Root의 닫힌 입력에는 승인 개정·세션/case·허용 대상 프로젝트/Unity EXE 길이/SHA·원래 process handle/PID/creation FILETIME·HWND/원래 창 thread·시작 크기·단일 출력 경로·기존 원시 예산·잔여 절대 시간·helper 소스/실행파일 핀·Root 종료/회수 소유를 결속해야 한다. 실행 input parser는 필수값·자료형·범위·중복·누락을 거절하고 임의 command 실행·동적 대상 선택을 받지 않는다. 정확 argv/승인 파일 schema·handle 전달 방식은 소스가 구현하는 내용과 함께 후속 실제 실행 계획에서 고정하며 현재 프로세스/파일 생성 권한은0이다. 사전 검증 실패이면 캡처/출력 생성0이다.

Root가 원래 Unity 프로세스와 helper 프로세스의 실제 소유 handle을 보유하고, helper에는 문서화된 명시 handle 전달/동일성 검증을 설계한다. PID만 보고 새 프로세스에 붙거나 종료하지 않는다. helper는 Unity를 종료하거나 키/포커스/메뉴/Play를 조작하지 않는다. GetWindowThreadProcessId·PID/creation/EXE·원래 HWND/thread/크기를 확인하고 최초 CaptureItem 하나를 유지한다. HWND 재사용 문제를 PID 확인만으로 해결했다고 주장하지 않으며 실제 대상의 연속성을 확인할 수 없으면 실패한다. 전체 앱/창 열거·다른 HWND 검색·창 재선택·CaptureItem 교체0이다.

**읽기 캡처 경계:** CreateForWindow만 사용하고 CreateForMonitor·전체 화면·다른 앱·키/마우스 수집0이다. 보호/동의 UI와 캡처 테두리를 유지하며 borderless/권한 우회0이다. 해당 Unity 창 밖 화면이나 인증/개인정보 창이 섞이지 않는 실제 경계는 실행 선행으로 확인한다. 대상 종료·CaptureItem.Closed·크기 변경·신원 불일치·장치/encoder/권한 오류는 첫 실패이며 신규 프레임 수집/인코딩0, 부분 원문을 보존한다. 검은 프레임이나 중복 합성으로 빈 구간을 메우지 않는다. 같은 원본 크기의 프레임만 처리하고 자동 resize/recreate/reconnect0이다.

GPU/프레임 자원은 프레임풀2와 단일 소유 staging/현재 인코딩 자료의 유한 보유로 제한하며 무제한 프레임 큐/전체영상 메모리 적재0이다. width/height/stride/바이트 산술은 overflow와 승인 크기 상한을 먼저 검사한다. 처리 속도가 부족하면 실제 관측 간격/오류를 남기고 무손실이라고 하지 않는다. 실제 SystemRelativeTime의 QPC 기반100ns와 Root/수신 단조 시각, 수신·인코딩 순번/간격을 구별한다. 정확 누락 개수를 알 수 없으면 null/미관측으로 남긴다. 수신 호출 수가 encoder 출력 수 또는 실제 디스플레이 프레임 전체 수라는 가정은0이다.

**파일 상한·원문·정리:** MP4는 후속 계획의 nativeEvidencePath 정확1, 재분석 지점/루트 검증 후 CreateNew로 생성한다. 기존 파일 덮기/이어쓰기/삭제0이다. IMFByteStream wrapper는 쓰기 시작 전부터 파일 최대 extent 2147483648바이트를 강제한다. Write/BeginWrite/SetLength/Seek의 모든 경로에서 음수·overflow·위치+길이·예약된 비동기 쓰기를 검증하고 직렬화된 위치/extent 관리로 동시 요청이 상한을 우회하지 못하게 한다. EndWrite/Flush/Close 및 비동기 callback의 수명·오류 전파를 정확히 구현한다. 예약은 실패 시 일관되게 정리하고 부분 쓰기를 실제 수로 기록한다. 위치 변경에 의한 sparse extent나 Finalize의 뒤늦은 헤더 쓰기도 상한 안이다. 사후 filesize 감시만으로 엄격 상한 충족을 주장하지 않는다. 상한 도달은 실패이며 출력 계속/새 분할 파일0이다.

상태·시각·프레임 수·간격·오류·종료 원문은 Root가 수집하는 제한된 stdout/stderr로 전달하는 구조를 준비한다. helper가 새 sidecar·원장을 만들지 않는다. 출력 누적은 각1MiB 이하·개별 행32KiB 이하로 제한하고 초과/회수 불가를 실패로 처리한다. 실제 실행 계획에서 Root의 동시 회수·봉인 경로와 이 예산을 기존 원장에 결속해야 한다. 이미 기록된 오류를 성공으로 덮지 않는다. MP4 길이/SHA는 실제 writer/프로세스 폐쇄 후 Root가 발급하며 helper가 미래 폐쇄를 성공으로 예측하지 않는다.

기존 case1800초/전체18000초 안에 준비·수집·종료·회수를 모두 포함한다. 영상에 별도 추가 실행 시간을 주지 않는다. 정상 종료는 남은 시간 안에서 수집 중지→프레임 자원 폐쇄→SinkWriter Finalize→writer 폐쇄→MFShutdown의 소유 순서를 지킨다. 종료5초·출력 회수5초 안의 응답은 현재 미입증이다. 불가하면 Root가 승인된 원래 helper handle로만 종료하고 부분 MP4/출력·미폐쇄/미관측 사실을 보존하며 영상 수용0이다. thread 취소만으로 COM/GPU/Finalize 종료가 보장됐다고 쓰지 않는다. 첫 실패·단회 소진·자동 재시도/재개/두 번째 대상0이다.

**검사와 미승인 경계:** 이번 허용 검사는 source1의 비실행 정적 판독1묶음과 범위 안 보정 재읽기 최대2회다. 검토 대상은 대상 한정·입력/주입0·유한 버퍼·모든 ByteStream 상한 경로·자원/비동기 수명·첫 실패·부분원문 보존·시간/출력 상한·금지 대체 경로0이다. 새 테스트 프레임워크·실제 API probe·컴파일·링크·실행파일 생성·native/Unity/GUI/Play0이다. 실제 빌드/능력/encoder/권한/영상 판독·연속성·5초 종료는 후속 정확 계획과 독립 판독·아스트라 실행 승인·도구 심사 전 미수용이다. 설치 확인이나 소스 구현을 capabilityAccepted=true로 올리지 않는다.

REQ/AC-C1B-G3OBS-005/006/007/012 및 REQ/AC-PROC-011/012를 추적한다. 최초 R01-A 안의 실제 확인·추가 예비 Play0·실패 뒤 나머지 구성0, native2GiB·기존 입력/샘플 한도·Q0·키/세대·필수9회·B1 FINAL 선행·CH1 목표는 그대로다. 이번 작성자는 문서1 append만 수행하며 source/helper/제품/원장 쓰기와 실행0이다. 접두사·최종 지문 확인 후 Root에 쓰기 소유를 반환한다.
##### 10.10.15 Input 이후 Editor의 단일 모듈 관리 코드 준비 — Approved

2026-10-13 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 아래 native0 준비만 Approved로 채택한다. 기존177477바이트/SHA256 `A3265E97BDCAE8C0FBE9AFC2B9EA512F48385AE486CB91B8EB3FAA853D40439F` 접두사를 보존한다. 목적은 변경될 Input producer와 Editor consumer를 각각 원래 단일 프로세스 관리 수명으로 검사할 수 있도록 고정 입력 수를 엄격한 계획값으로 전달하는 것이다. source2 작성과 독립 병행하며 실제 소스·RSP·디렉터리·컴파일 준비/실행 권한은 이 절에서 발급하지 않는다.

읽어 확인한 기존 원장 r8DirectPreparation.managementCode의 정확13111바이트/SHA256 `22715B196613AD9B88B9EEADA8631902C0CD2CB58FB80B2AB41FB925F5C3FE0B`를 출발 원문으로 고정한다. r8 계획·코드·성공/실패 역사와 원시는 불변이다. Root는 기존 원장에 새 불변 코드 항목1만 작성할 수 있다. 허용 변경5는 `Verify $p.inputFiles 501`의 전/후2곳을 `Verify $p.inputFiles $p.inputFileCount`로, `Verify $p.compilerResources 12`의 전/후2곳을 `Verify $p.compilerResources $p.compilerResourceCount`로 바꾸고 kind 상수1을 `direct-module-single-compile`로 바꾸는 것뿐이다. 다른501/12 문자열의 전역 치환0이며 source snapshot590은 그대로다.

추가 허용은 위 계획값에 대한 시작 전 검증 블록1이다. inputFileCount와 compilerResourceCount는 명시 존재하는 정수 자료형만 받고 문자열·bool·실수·null·누락·강제 변환을 거절한다. 각각1~5000, 1~64 범위를 요구하고 실제 inputFiles/compilerResources 배열 길이와 정확 일치해야 한다. 이 검증은 파일 생성/프로세스 시작 전에 이루어지며 불일치는 실행0으로 닫는다. 후속 정확 계획 판독에서도 같은 조건을 대조한다. 상한은 입력 허용 목록을 임의 확대하는 권한이 아니며 각 실행의 정확 경로·길이/SHA는 여전히 독립 판독된 계획으로 고정한다. 추가 fallback 기본값·자동 개수 추론·참조 검색0이다.

그 외 원래 Process/SafeHandle·PID/시작 신원·동시 byte pump·출력 cap·512문자/16오류·유한 JSON 봉인/핀·실패 원문·60초/종료5/회수5·첫 실패 중단·단회 소유 동작은 변경0이다. success/accepted/independentAcceptance=false와 실제 후봉인 원장 판정도 유지한다. 계획/코드 각2MiB, 로그/JSON 각1MiB·오류 예약64KiB, 실제 소스 전후590 검사는 그대로다. 모듈을 반복하는 루프·한 호출의 두 프로세스·재시도/재개·두 번째 호출을 코드에 추가하지 않는다.

Root의 허용 국소 검사는 새 코드 원문에 대한 비실행 PowerShell AST 구문 판독1회와 정확 델타/가드 읽기를 합한 전체5초 이내 단1묶음이다. 코드 점소싱·함수/본문 호출·native/새 호스트·컴파일·파일 writer 실행0이며 검사기 파일을 만들지 않는다. 최초 예상 밖 실패에서 중단하고 원문·원인·미실행을 기존 원장에 보존한다. 새 코드 원문·길이/SHA·정확5변경과 검증 블록·실제 AST 결과를 닫아 루나의 후속 묶음 독립 판독에 인계한다. 전체 회귀·새 QA/승인 사본·helper 파일0이다.

후속 실행 구조 후보는 Input1의 정확 새 계획→실제 단회 컴파일→프로세스/출력/결과 폐쇄·성공 판독→실제 Input ref 핀 결속→Editor1의 별도 정확 계획 순서다. 첫 Input 실패이면 Editor 준비/실행0, Editor 실패이면 추가 실행0이며 두 모듈 합계 최대2·각1회, 각60/5/5다. 이 수량은 후속 계획의 상한이며 지금 native 권한은0이다. source2 작성 종료·독립 델타 판독·현재590/실제 소스 및 부모 Movement 계보 확인 뒤에만 정확 RSP/prefix 준비의 별도 판정을 받는다. 각 실제 계획·원시 입력 핀·소유·루나 판독·아스트라 최종 입력 메시지·도구 심사 전 실행0이다. 영상 소스 준비를 이 컴파일의 선행으로 삼지 않는다.

REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012를 추적한다. Root 관리 코드 준비와 독립 판독만 닫으며 Runtime·GUI·Play·진단·SourceReady·B1·CH1 수용은 불변이다. 이번 아스트라 작업은 문서1 append·접두사/최종 지문 확인뿐이며 코드/원장 쓰기·실제 검사·실행0, 종료 후 쓰기 소유를 Root에 반환한다.
###### 10.10.13.1 공개 입력 읽기의 불명확 delta 처리 — 준비 한정 Approved

2026-10-13 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 §10.10.13 정확2파일 안의 아래 기술 보정만 채택한다. 기존182065바이트/SHA256 `A3838427AE4A3686C0C525B43E5D15A6780B92D5FB35F5551BA8956FBE7ADFAE` 접두사를 보존한다. Root/솔의 실제 API 판독에서 ReadValueFromEvent=false만으로 delta의 control 미포함과 format 불일치를 구분할 수 없고 공개 stateFormat 직접 판독은 현재 허용 경계 안에서 확보되지 않았다. unsafe·asmdef 변경·내부 반사·전체 control 열거·전체 payload 복사는 계속0이다. 새 사용자 선택은 필요하지 않다.

delta 사건에서는 허용 control 각각의 실제 읽기 성공값만 controlMask/pressedMask에 반영한다. 일부 control 읽기 성공은 해당 읽기의 성공 근거일 뿐, 전체 payload 유효성·나머지 false의 정확 원인·OS/Unity 사건의 완전성 증명이 아니다. false control은 이전 known/값을 유지하되 이번 관측 성공 mask에서 제외하고 새 release/변화를 만들지 않는다. 공개 API 읽기 성공 외 별도 가정으로 값/누락 원인을 복원하지 않는다.

허용9 control 모두 false이면 정상 위치 전용 delta인지 손상/불일치인지는 **미확정**으로 기록한다. 이를 malformed로 확정하거나 정상 무관 사건으로 인증하지 않는다. 원문에 불명 사유를 남기고 inputObservationComplete=false를 고정하여 최초 불완전 실패·기존 폐쇄 경로로 종료한다. 이미 관측한 값은 역사로 보존하지만 그 뒤 정상 사건/포커스 복귀로 완전성이나 실행을 복구하지 않는다. 이 때문에 정상 position-only 사건에서도 보수적으로 중단될 수 있다는 한계를 유지한다. 실패 문구에 raw payload나 허용 밖 control/값을 담지 않는다.

전체 StateEvent에서 해당 장치의 허용 control 하나라도 읽기 실패이면 기존 최초 실패 조건을 그대로 적용한다. 예외·장치 신원 변경·잘못된 형식이 별도로 확인된 경우도 실패다. delta의 불명확 원인을 공개 API로 더 구별할 수 없다는 사실은 기존 malformed 필수 거절·입력 완전성 AC를 충족했다는 뜻이 아니다. 해당 동적 구별과 실제 정상 관측의 지속 가능성은 미해결이며, SourceReady/G3 실제 수용0을 유지한다. 정적 준비는 계속할 수 있으나 이 한계를 숨긴 성공·금지 입력 불수신·무손실 주장은0이다.

정확2파일·기존 국소 검사/재읽기 예산·행/파일 상한·단회·첫 실패·전체 StateEvent 엄격 실패·게임 쓰기/입력 주입0은 불변이다. 새로운 실험/Play/native/컴파일 예산을 추가하지 않는다. REQ/AC-C1B-G3OBS-005/006/007/010/012 및 REQ/AC-PROC-011/012의 미관측 보존과 실제 필수 수용 경계를 따른다. 이번 작성은 문서1 append만이며 제품/원장 쓰기·실행0, 최종 지문 확인 후 Root에 소유를 반환한다.
##### 10.10.16 단일 컴파일 감독 경로의 실행 없는 소스 준비 — Approved

2026-10-13 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 아래 소스 준비만 Approved로 채택한다. 기존185206바이트/SHA256 `C2EAAE72A2A2C4032C368B499E281A0433CB09A2872A75A5B7D95CE6955D15C8` 접두사를 보존한다. r10 실제 실패와 소진 권한, 이후 관리 후보15615바이트/SHA256 `09042636FA36430C0591EEB4A5D9DF5A2561392DA256E4C357F56331E35B9D60` 및 AST 오류0의 사실은 불변이다. 같은 스레드의 동기 신원 조회가 멈추면 5초를 선점하지 못한다는 P1 실행 관문은 아직 닫히지 않았다.

**목적·범위·소유:** 사용자 체험 빌드를 위한 현재 Input/Editor 컴파일 준비에서, 조회 정체와 독립적으로 원래 프로세스를 감독할 경로를 작성한다. 솔 gpt-6.1-sol의 새 쓰기 소유는 `output/chapter1-playable-r1/tools/m1-single-csc-supervisor.v1.py` 정확1이다. 최초 파일 생성1과 이 범위 안 보정만 허용한다. Root는 기존 원장의 불변 준비 codeitem1 및 실제 부모 제어 소유를 맡고, 루나는 완성 코드 묶음 독립 판독, 아스트라는 정확 실행 승인·최종 한정 수용을 맡는다. 별도 helper/QA framework/보고·승인 사본·새 제품 파일0이다. 기존 M3 PS/PY2·Observer/Window2·영상 CPP1·현재 소스 체크포인트 및 원시는 수정하지 않는다.

기존 M3 PY56987바이트/SHA 접두2E07E와 PS31422바이트/SHA 접두C7B5의 실제 전체 핀을 준비 원장에 결속하고 원문을 보존한다. 이 도구들의 same-thread identity 및 PS MainModule 경로는 현 P1의 해결 근거가 아니므로 실행하지 않는다. 새 소스는 기존 x64 ctypes 구조·공개 Win32 API·QPC·CreatePipe/PipePump·엄격 parser·핀 검증·소유 Close 조각만 재사용한다. 조각별 원래 위치와 새 ABI/서명 차이를 명명하며 전체 M3 실행 모드나 fallback을 가져오지 않는다.

**단일 대상·신원·감독:** 허용될 역할은 정확 설치 dotnet/csc/RSP의 SourceCompilerOnce 하나다. 동적 명령/셸·다른 역할·창/프로세스 이름 검색·PID로 재취득·재시작0이다. 명시 허용 pipe handle만 전달하는 확장 시작 정보와 CREATE_SUSPENDED를 사용하도록 준비한다. CreateProcess 반환 뒤 main이 원래 PROCESS_INFORMATION의 hProcess/hThread를 즉시 소유한다. 신원 query worker1에는 조회 최소 접근권으로 복제한 handle만 넘기고, 용량1 결과 queue로 PID/creation time/실제 실행 경로의 결과 또는 오류를 전달한다. 조회 worker에는 Resume/Terminate/프로세스 생성 권한을 주지 않는다. main은 worker의 조회·잠금·join 완료를 기다리지 않으며 비차단 결과 확인과 원래 QPC 기준 최대5초 감독을 수행한다.

QueryFullProcessImageNameW는 모듈 목록 기반 MainModule과 다른 공개 경로지만 무정지 보장은 아니다. WinDLL의 GIL 해제는 독립 감독의 설계 근거일 뿐 실제 스케줄 기한 보장이 아니다. 신원 결과가 같은 원래 handle의 PID/creation 및 승인 EXE 경로·핀과 결속된 뒤에만 ResumeThread를 정확1회 허용하고 반환값으로 기존 suspend count1을 확인한다. null/다른 경로/오류/조회 시간 초과·이미 종료·불일치는 실패이며 예상 경로로 채우지 않는다. 재개 전 loader/실행 성공을 주장하지 않는다.

시간 초과/실패에는 main이 보유한 원래 process handle만 사용한다. 새 명시 API TerminateProcess의 ctypes 서명은 HANDLE,DWORD→BOOL이며 반환·GetLastError·원래 handle의 Wait/실제 정수 exit를 기록한다. 다른 프로세스 종료0이다. 전체60초 안의 신원5초, 종료5초·EOF 회수5초와 기존 원문 상한을 유지하는 구조를 준비한다. 종료·회수는 서로 실제 시각을 기록하고 다음 실행에 남은 예산을 이월하지 않는다. 두 pipe는 동시 유한 회수·독립 오류/EOF 상태를 갖고 부분 회수를 전체 성공으로 승격하지 않는다. 대기 중인 worker를 중단했다고 가정하거나 handle을 조기 폐쇄해 재사용 위험을 만들지 않는다. worker/복제 handle이 남으면 supervisor 폐쇄 미완료이며 Root가 원래 supervisor handle로 관리하는 후속 종료 경계가 필수다.

**자식 소유와 능력 관문:** 현재 대상이 자식을 만들지 않는다는 관측 없는 주장은0이다. 기존 M3의 JOB_LIST·kill-on-close·프로세스 집계 조각은 새 소스 안에 정확 선언/사용 범위를 드러내는 준비 재사용을 허용한다. 생성 시점부터 원래 Job에 귀속되고 종료 시 집계0·모든 pipe EOF가 확인되는 경로를 실행 필수 조건으로 둔다. 실제 JOB_LIST/중첩 Job/handle 상속 능력이 입증되지 않으면 생성0이며 생성 후 Assign만으로 원래 생성부터의 소유와 동등하다고 대체하지 않는다. Root의 supervisor 원래 handle·Job 수명 소유까지 후속 계획에서 명명해야 한다. DEBUG/초기 loader 인증은 이번 범위0이며 M3의 해당 미수용 상태를 완화하지 않는다. suspended 생성만으로 loaderAccepted=true를 기록하지 않는다.

CreateProcess 반환 전에는 PI handle을 확보하지 못할 수 있고 OS/API 자체 정체도 선점 보장되지 않는다. 이 한계를 source 준비로 해결했다고 주장하지 않는다. Root 부모 감독·원래 supervisor handle·target Job 소유·반환 전 불확실 상태의 실패 처리와 실제 ABI/query/종료/회수 능력이 후속 실행의 선행이다. 불명확한 시작·미관측 폐쇄는 실패이며 자동 재시도/새 PID 탐색0이다. 가용 방법으로 이 경계를 닫지 못하면 실행 차단을 유지한다.

**파일·판정·검사:** 정확 계획/RSP/소스 핀을 읽고, 기존 raw10의 CreateNew·source-before/after590·로그/JSON 각1MiB·계획/코드 각2MiB·오류 예약64KiB·유한 메모리 및 원문 보존 의미를 유지하도록 준비한다. 새 source가 원시를 쓰는 실제 권한은 아직0이다. 핀/정수/배열 중복·원래 입력305/자원10 등의 실제 값은 차후 정확 계획에서 고정하며 과거 계획을 자동 소비하지 않는다. accepted/independentAcceptance 및 Runtime/SourceReady/B1/CH1은 false로 유지한다. 종료·봉인 후 실제 길이/SHA/경과는 Root의 외부 판정에 결속한다.

이번 허용 검사는 비실행 정적 읽기1묶음 및 범위 안 보정 재읽기 최대2회다. 구문검사 계획1도 작성할 수 있으나 실제 Python 실행 권한은0이다. 구문 확인은 닫힌 새 source의 정확 핀과 설치 Python·정확 argv·소유/시간/회수를 명명하여 별도 승인받은 읽기 AST 검사1로 한정할 계획이며, 대상 import/exec/WinDLL 로딩으로 바꾸지 않는다. 지금 Python/Win32/API probe·컴파일·실행파일·RSP/prefix 생성·GUI/Play0이다. 소스 작성만으로 P1 폐쇄·hard-wall 보장·capabilityAccepted를 주장하지 않는다.

향후 supervisor1+compiler1은 실제 native 최대2이므로 과거 native1 승인으로 실행할 수 없다. 실제 정확 개수·부모/자식/Job 소유·ABI·조회/종료/EOF 관문·유한 예산을 묶음으로 독립 검토하고 아스트라의 새 최종 승인과 도구 심사를 거치기 전 실행0이다. Input 성공/폐쇄·ref 핀·독립 판독 전 Editor 준비0은 그대로다. REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012를 추적한다. 준비 종료는 정확 코드·재사용 델타·정적 판독·미해결 실행 조건의 기존 원장 반환이다. 이번 아스트라 작업은 문서1 append만이며 제품/도구/원장 쓰기와 실행0, 최종 지문 확인 후 Root에 소유를 반환한다.
##### 10.10.17 정상·지연 폐쇄의 한정 능력 검증 준비 — Approved

2026-10-13 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 아래 코드 준비만 Approved로 채택한다. 기존193126바이트/SHA256 `D145E85CAE6DCB9384EAD8A4FA197C8B930ADA2DD3F9D73D5F13AC225D340E2A` 접두사를 보존한다. §16 소스49945바이트/SHA256 `AB23F6B304E53DC313FE3E6235C4E455236B07B562F341E323DA78B633522DBA`의 AST-only 실행 PID48884·exit0·폐쇄/EOF 확인·0.3175179초와 독립 구문 판독은 구문만의 근거다. Win32/Job/감독 능력을 수용한 역사로 바꾸지 않는다.

목적은 감독기 자체 능력이 선행 수용돼야 검증을 실행할 수 있다는 순환을 한정 예외로 해소하는 것이다. **CapabilityOnce의 능력 검증 자체에는 prior capabilityAccepted=true를 요구하지 않는다.** 이 예외는 SourceCompilerOnce나 일반 실행에 전파하지 않는다. 정상 신원/재개 경로와 지연 중 독립 종료 경로를 실제로 구별할 준비이며, 현재 실제 검증 실행 권한은0이다.

**정확 쓰기·담당:** 솔 gpt-6.1-sol은 기존 `output/chapter1-playable-r1/tools/m1-single-csc-supervisor.v1.py` 정확1에 CapabilityOnce 역할1과 아래 한정 분기를 추가한다. 원래 source/AST 증거를 기존 Root 원장에 보존하고 새 코드 핀을 발급한다. Root는 같은 기존 원장에 inline 부모 codeitem1 및 정확 준비 입력을 불변 항목으로 작성한다. 새 helper·C#/Python 파일·QA framework·계획/승인 사본0이다. M3 PS/PY2·G8·제품·Observer/Window·영상CPP·RSP/prefix는 변경0이다. 루나는 전체 변경 묶음 독립 판독, 아스트라는 실제 실행/한정 수용을 맡는다. 현재 새 모델 추론 상향 override0이다.

부모는 이미 실행 중인 PowerShell 안에서 메모리 C# P/Invoke를 사용할 코드만 준비한다. Add-Type·그 코드 호출·native API 실행은 별도 정확 실행 승인 전0이다. 디스크 출력 assembly를 만들지 않는다. 새 부모 프로세스0이나 부모의 Win32 호출이 없는 것은 아니므로 실행 관문에서 이를 명시한다. 허용 API/구조체·호출 인자·x64 ABI·오류 처리를 코드와 계획에 모두 닫고 임의 native 호출/셸 명령 입력은 받지 않는다.

**고정 사례와 수량:** 한 supervisor가 정상1 다음 지연폐쇄1을 순차 수행하는 구조다. 실제 native 총3은 supervisor1+검증 Python 자식2이며 과거 native1/2 예산 재사용이 아니다. 각 자식은 사전 핀으로 고정한 설치 Python의 정확 `-I -S -B -c pass` 하나이고 제품 모듈 import/컴파일/Win32 대상 코드 실행0이다. 동시에 활성 query worker는 최대1, 결과 queue는 용량1이다. 정상 사례의 모든 필수 원문과 폐쇄가 성공한 뒤에만 두 번째 사례를 시작한다. 예상 밖 실패이면 나머지0이며 대체 자식/추가 사례/재시도0이다.

정상 사례는 별도 privateJob에 생성 시 귀속된 suspended 자식의 원래 PI handle을 보유하고, 실제 PID/creation/EXE 핀을 조회 worker로 확인한 뒤 ResumeThread 정확1·이전 suspend count1을 요구한다. 실제 exit0·양쪽 EOF·원래 프로세스/복제handle 폐쇄·privateJob total1/active0을 확인한다. 성공으로 추정한 신원이나 예정 경로를 실제 조회값 대신 쓰지 않는다.

지연 사례는 새 privateJob과 새 원래 자식 PI를 사용한다. query worker의 정확 단1 Sleep1500ms를 시험 자극으로 명명한다. main은 그 호출과 독립된 QPC500ms 기한을 감시하며 결과 미도착을 관측하면 Resume0을 유지하고 원래 process handle로 TerminateProcess·Wait/실제 exit·EOF 회수를 수행한다. 종료5초/회수5초를 각각 실제 기록한다. worker 지연 후의 실제 query 결과 또는 오류와 복제handle 폐쇄를 별도 기록한다. 종료된 대상의 사후 query 오류는 그 오류 그대로 보존하며 신원 조회 성공으로 바꾸지 않는다. 예상 deadline 자극의 관측과 종료 실패·미폐쇄·원시 손실은 별도 판정이다. worker가 예상 시점에 살아 있었음/감독 진행/실제 종료의 시각 증거가 없으면 독립 감독 성공으로 수용하지 않는다.

**부모·Job 소유:** 부모는 비상속 outerJob의 유일한 handle과 원래 supervisor PI를 보유한다. supervisor도 생성 시 JOB_LIST로 outerJob에 결속하고 그 자식은 각 privateJob 및 상속된 outerJob의 소유 안에 남는다. breakaway·나중 PID 재검색·이름 종료·동적 재취득0이다. 실제 집계는 outerJob total3/active0, 각 privateJob total1/active0을 요구한다. 생성/중첩/집계 능력이 실패하면 원시 실패로 닫으며 해당 능력을 가정하여 다음 대상을 시작하지 않는다. 모든 관련 handle의 실제 폐쇄와 pipe EOF를 확인해야 한다. Job을 닫았다는 사실만으로 모든 프로세스 종료/출력 회수 성공을 추론하지 않는다.

**시간 보장과 수용 범위:** 전체60초·각 종료5/회수5, stdout/stderr 각1MiB·오류64KiB, 유한 queue/메모리를 유지한다. supervisor와 자식·worker·부모의 실제 시각과 첫 실패를 한 원장에 결속한다. hardWallGuaranteed=false는 계속 유지한다. CreateProcess 반환 전 PI 미확보, 동기 OS 호출 정체, 실제 스케줄 지연의 절대 상한은 이 두 사례로 증명하지 못한다. 그 상태를 미관측/미폐쇄 실패로 기록하고 성공으로 보충하지 않는다. 검증이 성공하더라도 **반환된 원래 handle과 생성 시 Job 소유가 확인된 구간의 실제 정상/지연 감독·폐쇄 능력만** 한정 수용할 수 있다. 이 명시 한정은 절대 무정지 증명 요구를 검증 실행의 순환 선행으로 쓰지 않도록 하는 호환이며, 종료/원문/권한 AC를 면제하는 것이 아니다.

CapabilityOnce의 정확 계획 parser는 고정 사례2와 위 고정 명령/수량만 허용한다. 기존 원장의 정확 inline capability 항목 선택과 길이/SHA 결속으로 결과를 전달하며 새 capability 파일을 생성해 그 파일이 선행 승인돼야 한다는 순환을 만들지 않는다. 원장 전체를 임의 실행 입력으로 해석하지 않는다. capability 결과 소비는 아스트라의 외부 한정 수용 메시지와 정확 항목 핀이 선행이고, 미래 SourceCompilerOnce가 어떤 동일 코드/환경/능력만 재사용하는지는 별도 계획에서 명시한다. compiler·제품·RSP·raw10 생성 권한은 이번 사례에0이다.

**검사·종료:** 준비 검사는 비실행 정적 판독1묶음+범위 안 보정 재읽기 최대2회다. 구문검사는 계획만 준비하며 닫힌 source 핀·정확 Python/argv·관리 소유·예산의 별도 승인 전 실제 syntax 프로세스0이다. 실제 CapabilityOnce3개 실행도 닫힌 source/부모코드/검증명령·환경핀·예산·소유·독립 판독과 아스트라 최종 입력 승인·도구 심사 전0이다. 추가 동작실험/probe를 자동 삽입하지 않는다. 실패 원문과 소진 횟수는 보존한다.

REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012를 추적한다. 준비 종료는 정확 source/부모codeitem·차집합·미입증 경계·정적 판독을 기존 Root 원장에 인계하는 시점이다. SourceReady·Runtime·GUI·Play·G3·B1·CH1 수용과 필수 인간/제품 관문은 불변이다. 이번 아스트라 작업은 문서1 append·접두사 확인뿐이며 코드/원장 쓰기·실행0, 작성 종료 후 Root에 소유를 반환한다.
같은 §10.10.17의 정확 입력 운반은 CapabilityOnce 역할에 한해 다음으로 한정한다. 현재 기존 원장 전체는2MiB를 넘으므로 supervisor가 전체 원장을 plan으로 읽지 않는다. 부모가 선택한 불변 itemID의 planText 정확 UTF8 길이/SHA를 검증한 후 stdin에 명시 길이 프레임으로 최대2MiB를 전달하고, 이어 parentBind 프레임 최대16KiB를 전달한다. 명시 프레임 수2·길이·UTF8·원문 SHA·역할·itemID·필수 필드를 검증하고 짧은 읽기/중복/누락/초과/추가 프레임은 실패로 닫는다. stdin 대기도 기존 전체60초 감독 안이며 무제한 ReadToEnd·선언 길이만 믿는 무한 할당0이다. parentBind는 실제 부모/outerJob/원래 supervisor 소유와 정확 계획 핀을 결속하는 제한된 자료이며 임의 명령·새 권한을 담지 않는다. 세부 schema/handle 전달/비동기 회수는 코드와 독립 판독에 명명한다. 새 계획 파일·전체 원장 SHA·문서 지문 재귀 의존0이다.

이 stdin 계획 경로를 SourceCompilerOnce에 자동 적용하지 않는다. 그 기존 계획/RSP/권한 선행은 그대로이며, 미래 compiler 계획이 capability의 원장 불변 항목을 정확 투영·소비하는 방식은 실제 실행 핀 확정 때 별도 승인한다. CapabilityOnce 검증 결과가 존재한다는 사실만으로 다른 역할의 입력 또는 prior capability 검사를 우회하지 않는다.
##### 10.10.18 CapabilityOnce의 콘솔 비연결 생성 준비 보정 — Approved

2026-10-14 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 아래 정확2사이트의 실행 없는 준비 보정만 Approved로 채택한다. 기존202318바이트/SHA256 `CBD0806A44F3B12DFB12896EE66209E9D4C907FA0E5DBC35A4BA4AA5F93AFD82` 접두사를 보존한다. §17 실제 첫 실행은 정상 사례 privateJob total2/active0으로 기대1과 달라 실패했고 지연 사례0이었다. 원래 supervisor PID51836 exit1, 부모 outerJob total4/active0으로 기대3과 달랐으며 whole2.9344364초·보유 핸들 폐쇄/EOF 원문은 보존한다. 이 값들을 예상3 성공이나 전체 계보 확인으로 바꾸지 않는다. 당시 source74555/3163599FF81F20E666E69E5453FD3D732043CB0555AEE0FC5D03AC7038C8C5FD 및 parent36566/59CADBB056CA09D7BA9A77D9F2220625482912360814AACD66163FC2ACB26D5A, 실제 계획·입출력·단회 소진은 불변이다.

Root/솔의 파일 조사로 cached Python의 Py_Main/python312.dll/3.12.14와 pyvenv.cfg 부재 등이 확인됐으나 venv redirector 가설은 확정되지 않았다. 공식 콘솔/프로세스 생성 문서에 비춰 콘솔 서비스 동반 생성은 가능한 가설이지만 실제 추가 PID가 conhost였다는 신원 증거는0이다. 이번 보정은 원인을 확정하는 판정이 아니라 **콘솔 비연결 생성 조건에서 동일한 신원·pipe·Job 집계·폐쇄를 관측할 새 목적**을 갖는다.

허용 변경은 기존 `output/chapter1-playable-r1/tools/m1-single-csc-supervisor.v1.py`의 CapabilityOnce 검증 자식 생성 사이트1과 기존 Root 원장 inline 부모 코드의 supervisor 생성 사이트1에서 CREATE_NO_WINDOW 값0x08000000을 DETACHED_PROCESS 값0x00000008로 대체하는 것뿐이다. 합성된 나머지 생성 플래그는 그대로 두며 두 플래그를 함께 지정하지 않는다. 필요한 명명 상수는 해당 사이트의 정확 값 대체를 표현하는 범위만 허용한다. SourceCompilerOnce의 원래 생성 플래그·공유 기본 동작은 변경0이다. STARTF_USESTDHANDLES·명시 pipe·HANDLE_LIST·JOB_LIST·CREATE_SUSPENDED·원래 handle·조회 worker/queue·Resume·종료·회수·수량·시간·오류 판정은 그대로다. 새 PID 조회 API·전체 프로세스 열거·대체 Python·추가 helper/framework/파일·설치0이다.

솔은 source1 단일 쓰기, 부모 코드 작성자는 Root가 위임한 기존 원장 불변 새 codeitem1의 단일 쓰기를 맡는다. Root는 위임 반환 전 같은 원장에 병행 쓰지 않는다. 기존 실패 source/parent 원문은 먼저 원장에 보존하고 수정 뒤 정확 길이/SHA·2사이트 차이 및 그 외 내용 불변을 닫는다. 새 승인/QA 사본0이다. 루나는 변경 묶음을 독립 판독하고 아스트라는 후속 실제 입력 실행 여부를 별도 판정한다.

허용 검사는 비실행 정적 판독1묶음과 정확2사이트·역치환 동일성·역할 분리 확인뿐이다. 이 준비 승인에서 새 구문검사 프로세스·Add-Type·native API·CapabilityOnce·compiler·GUI·Play 실행0이다. 정적 확인에서 예상 밖 차이/실패가 나오면 원문 보존 후 중단하고 자동 재읽기·실행을 추가하지 않는다.

정상+지연 두 사례의 의미와 기대 privateJob 각 total1/active0, outerJob total3/active0은 유지한다. 숫자를 실제 실패 집계에 맞춰 늘려 수용하지 않는다. DETACHED_PROCESS가 추가 프로세스 제거·출력 전달·Job 귀속·독립 감독 성공을 보장한다고 주장하지 않는다. 실제 가용성은 후속 정확 핀·독립 판독·아스트라 최종 메시지·도구 심사로 승인된 새 단회 관측에서만 판단한다. 과거 권한 재사용·자동 재시도0, 현 capabilityAccepted/hardWallGuaranteed=false·compiler0을 유지한다. 전체60초와 실패 시 종료5/회수5, 최초 QPC/clock5 발급 규칙 및 기존 출력 한도도 불변이다.

REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012를 추적한다. 준비 종료는 정확 코드 핀·불변 원문·독립 판독 대상의 Root 반환이다. 이번 아스트라 작업은 문서1 append만이며 source/원장 쓰기·실행0, 최종 지문 확인 후 소유를 Root에 반환한다.
##### 10.10.19 관측 능력의 한정 재사용과 Input 단회 컴파일 준비 — Approved

2026-10-14 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 아래 실행 없는 준비만 Approved로 채택한다. 기존206665바이트/SHA256 `EFAC4D46E4FFD0514AEBAF154D1486E1D86E37FA3497FB46D5FB3BBACDCC91C0` 접두사를 보존한다. §18의 실제 정상·지연 사례와 부모 집계/폐쇄에 대한 외부 한정 수용은 기존 원장 capabilityDetachedLimitedAcceptance에 보존하며 원시 accepted/권한·과거 실패를 소급 수정하지 않는다. 목적은 이 증거의 정확 한정 범위만 재사용하여 현재 Input 컴파일을 준비하는 것이다.

**정확 소유·변경:** 솔은 기존 tools/m1-single-csc-supervisor.v1.py 소스1, 별도 위임받은 부모 작성자는 기존 Root 원장의 새 불변 부모 codeitem1만 작성한다. Root는 원장 쓰기 위임 중 병행 쓰지 않으며 반환 뒤 후속 정확 wrapper/계획을 소유한다. 새 helper·QA/승인 사본·검사 framework·RSP·출력 디렉터리·컴파일 계획 파일은 지금 생성0이다. M3/G8·제품 소스590/v14·Observer/Window·영상 CPP는 변경0이다. 원래 검증 source74555/A9B8B0F25F4D73A22C5BF7495A1B0D1C38218B0FEB59BC30B570B50D06C67263 및 parent36566/5B401007A6111675B543259E01405D086A30139D637703B852D1E6A9F2F69A4A를 원문으로 보존한다.

SourceCompilerOnce의 capabilityEvidence 파일핀 소비 지점만 기존 원장 불변 항목의 정확 투영 검증으로 바꾼다. 전체 원장을 source에서 읽거나 전체 원장 SHA를 요구하지 않는다. 투영은 원래 선택 itemID·UTF8 원문 길이/SHA·원래 source/parent/Python/host 핀·실제 normal/delayed 원시·parent outer3/active0·루나 정정 판독·아스트라 외부 한정 수용을 결속한다. 과거 capability session과 새 compiler session은 별개이며 같은 세션으로 재작성하지 않는다. 새 compiler 계획이 정확 원래 증거 item을 명시 참조해야 한다. 선택 투영을 임의 bool 묶음으로 바꾸거나 불명확한 원문 없이 accepted=true만 신뢰하지 않는다. 실제 전달 schema/길이 상한/엄격 parser와 누락·중복·형식·핀 불일치 거절을 구현 차집합에 명명한다.

기존 capabilities의 정확6필드를 없애거나 자동true로 채우지 않는다. 다음은 재사용할 근거와 한계의 대응이다.

| 필드 | 필요한 실제 근거·한정 |
| --- | --- |
| win32AbiAccepted | 이번 동일 x64 API/구조체의 실제 신원 조회·생성·Resume/Terminate/Wait/폐쇄 성공. 다른 ABI/API 일반화0. |
| jobListAtBirthAccepted | 실제 원래 PI와 생성 시 Job 결속, 정상/지연 privateJob 각 total1/active0. 생성 반환 전 소유 보장0. |
| nestedJobCompatibleAccepted | 같은 환경의 supervisor outerJob과 privateJob 중첩, 최종 outer3/active0. 다른 호스트/권한/Job 환경 일반화0. |
| pipeInheritanceAccepted | 명시 handle 전달·원래 양쪽 pipe EOF·폐쇄·누락0. 임의 상속/다른 pipe 구성 수용0. |
| queryWorkerGILProgressAccepted | 실제 Sleep 약1513ms 동안 main QPC의500ms 감독·원래 handle 종료 진행. 일반 스케줄/OS 무정지 보장0. |
| parentSupervisorDeadlineAccepted | 실제 부모 원래 handle/Job의 감독·정리·봉인이 전체60초 안에 완료됨. 절대 hard wall·CreateProcess 반환 전 선점 보장0. |

원래 제한수용 기능6과 위 schema의 의미는 서로 이름만 같다고 간주하지 않고 이 표와 실제 원문 필드로 대조한다. hardWallGuaranteed/returnBeforePIClosureGuaranteed/loader/G8/제품 SourceReady는 false/미수용을 유지한다. source/parent의 파생 핀이 바뀌면 원래 검증 핀을 새 핀으로 위조하지 않는다. 정확 역델타 및 실제 Win32 경로·구조체·handle/pipe/Job/worker/시간 동작 불변의 독립 영향 판독 후 아스트라의 명시 재사용 판정을 받아야 한다. 이번 절은 그 조건을 규정하며 아직 완성되지 않은 파생 코드의 재사용 수용을 선발급하지 않는다.

SourceCompilerOnce 생성 플래그1사이트만 CREATE_NO_WINDOW에서 이번 관측 DETACHED_PROCESS로 바꾸는 것을 허용한다. CapabilityOnce의 생성 branch는 변경0이다. 다른 flags·HANDLE_LIST/JOB_LIST·CREATE_SUSPENDED·정확 handle 소유·질의 worker·단회 Resume·종료/회수는 불변이다. 추가 API·PID 검색·프로세스 탐색·대체 Python·새 검증 사례0이다. 콘솔 비연결이면 항상 추가 프로세스가 없다는 주장은0이며 실제 compiler 실행의 집계를 다시 확인해야 한다.

새 부모 codeitem에는 CapabilityOnce와 분리된 SourceCompilerOnce 한정 분기를 둔다. supervisor CLI는 정확 핀 계획파일의 `--plan` 및 `--plan-sha256` 경로이며 임의 모드/명령을 받지 않는다. 지금 계획파일을 만들지 않는다. parentBind는 줄바꿈으로 끝나는 단1자료·최대16KiB이며 CapabilityOnce의 stdin2프레임을 compiler 분기에 혼용하지 않는다. exact parentBind schema·원래 부모/outerJob/PI·계획/코드핀 결속을 명명한다. native 예상수는 supervisor1+compiler1=2, outerJob total2/active0, target privateJob total1/active0이다. 첫 실패이면 추가 생성0이다. 기존 CapabilityOnce의 native3/job3/2프레임 분기와 불변 원문을 보존하고 새 분기로 과거 승인을 소비하지 않는다.

같은 P/Invoke ABI·원래 PI/handle·QPC·두 pipe 유한 회수·원문·상한을 유지한다. 전체60초 정상 폐쇄/봉인과 실패 시 +65/+70 정리, 최초 QPC1회부터 전검사/Add-Type 포함·clock5만 지연 결속·그 외 불변·기한 재발급0을 따른다. 계획/코드/출력의 기존 유한 상한과 source-before/after590·실제 정수 exit·EOF·Job 집계·폐쇄 후 핀·acceptedfalse 원시를 유지한다. 감독기 정상 종료만으로 compiler 성공을 추론하지 않는다.

**준비 검사·후속 실행:** 비실행 정적 판독1묶음과 범위 안 보정 재읽기 최대2회를 허용한다. 숫자/역할/입력 전달·capability 투영·플래그·기존 Win32 경로와의 정확 차이를 한 번에 닫는다. 실제 Python 구문검사·Add-Type·native·compiler·GUI·Play는 별도 정확 승인 전0이다. 새 원시 증거 생성이나 테스트 framework를 추가하지 않는다. 루나는 source+부모 묶음 독립 판독, 아스트라는 조건부 증거 재사용과 실제 입력을 판정한다.

첫 허용 대상 후보는 AcadeGameMaker.Input.Unity 한 모듈뿐이다. 준비 코드 종료 뒤 실제 새 Input RSP·입력305·자원10·소스22·metadata·v14 snapshot590·Movement 개별 성공 계보를 별도 정확 계획으로 닫아야 한다. 새 prefix/RSP 준비도 별도 승인이다. 계획/코드/wrapper·실제 source/parent 핀·루나 판독·아스트라 최종 입력 승인·도구 심사 뒤에만 새 단회 컴파일이 가능하다. Input의 실제 성공·폐쇄·ref 핀·독립 판독·한정 수용 전 Editor 준비0이다. 과거 r9/r10 및 capability 시도 소진을 재사용하지 않는다.

REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012를 추적한다. 종료는 정확 코드/투영schema/차집합·독립 판독 자료를 기존 원장에 반환하는 시점이다. 이번 아스트라 작업은 문서1 append만이며 코드/원장 쓰기·실행0, 접두사와 최종 지문 확인 후 Root에 소유를 반환한다.
##### 10.10.20 실제 Input 수용 후 Editor 단회 컴파일의 한정 준비 — Approved

2026-10-14 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 실행 없는 아래 준비를 Approved로 채택한다. 기존214239바이트/SHA256 CD7EA1C00BF2CF4234A6701B47637B81D0F91D9B6B453C3CB324AE59EF910743 접두사와 모든 동결 실패·소진 원문을 보존한다. 사용자 산출물은 현재 소스의 1챕터 직접 체험 빌드이며, 이번 단계의 산출물은 Input의 실제 참조 산출물에 연결되는 Editor 컴파일 준비 묶음이다. 제품·GUI·Play 수용을 대신하지 않는다.

**실제 선행·Input 한정 수용:** 기존 원장 r14ActualInputCompileObservation 및 r14InputIndependentResultReview의 반환709331바이트/SHA256 622E7B7895CBE57FD4A2A473CD24E86205DDE82BEE310650D8E60BD762F06796에 근거해 r14 Input 모듈 컴파일만 최종 수용한다. 실제 target48960 exit0, supervisor3228 exit0, private1/active0·outer2/active0, 실제 handle/EOF 폐쇄, 소스 전후590, 원시10개·산출물3개, 경고7/오류0, Root 후봉인26.5242626초 및 루나 추가P0/P1=0을 근거로 한다. 원시 accepted/independentAcceptance=false는 수정하지 않고 외부 한정 판정으로 기록한다. 진단 인코딩 일반 보장0, Runtime/SourceReady/GUI/Play/loader/G8/B1/CH1 수용0이다. 다음 참조 선행은 실제 r14 AcadeGameMaker.Input.Unity.ref.dll 78848바이트/SHA256 6DA880A4779F28FC45602FB8A4B8DDBBE039C80FB9F84AD86BD1E53E083538A8이며 원래 성공 계보와 실제 파일 핀을 결속한다.

**담당·정확 쓰기 범위:** 솔은 기존 output/chapter1-playable-r1/tools/m1-single-csc-supervisor.v1.py 한 파일만 담당한다. 별도 위임된 솔은 Root 기존 원장의 새 불변 Editor 부모 codeitem 및 계획/승인/wrapper 틀과 정확 새 출력 디렉터리1/RSP1을 담당한다. Root는 원장 위임 동안 병행 쓰지 않으며 반환 후 실행 통제를 단독 소유한다. 루나는 닫힌 묶음을 독립 판독하고 아스트라는 증거 재사용·실제 실행 입력을 별도 판정한다. 새 helper/검사 framework/QA·승인 사본/제품 소스/영상 CPP/M3 파일 변경0이다. 새 부모 itemID 및 출력 접두사의 구체 경로는 준비 시작 전에 기존 원장에 하나씩 고정하며 임의 대체·추가 모듈은 허용하지 않는다.

**최소 코드 차집합:** 기존 source89979/E878D60DCCC794B5B5AAA8943E767CAB756011B1A2421E81E3C9EBF16CE743B8과 부모50002/D5CF359B81D8ACF67A31E38D3119D62C0C22EE9AAA2E5F23FD8F55462E8CAA3B 원문을 보존한다. SourceCompilerOnce의 고정 모듈1곳을 AcadeGameMaker.Input.Unity에서 AcadeGameMaker.Combat.Authoring.Editor로 바꾸고 고정부모 itemID를 새 불변 Editor 항목으로 결속한다. 새 부모 항목은 모듈 고정 리터럴3곳(PSExpectedItemId/PSmodule/C#Needitem에 대응하는 실제 검증 위치), 해당 항목ID 및 그 필수 오류 문구만 보정한다. 실제 위치·개수·역치환을 독립 판독으로 닫으며 다른 실행 동작 변경은 이 승인에 포함하지 않는다. CapabilityOnce, 공유 Native API/구조, 생성 flags, 프로젝트 cwd, 원래 PI·handle·Job·worker·pipe·QPC·종료·회수 및 경로 대소문자 검증은 불변이다. 기존 Input 준비/성공 원문을 Editor로 덮어쓰지 않는다.

**정확 Editor 입력 준비:** 보존된 AcadeGameMaker.Combat.Authoring.Editor.rsp 원문을 기준으로 새 접두사의 out/refout2곳, 현재 r14 Input ref 참조1곳, /utf8output CRLF행1개(13바이트) 삭제만 허용한다. 나머지 바이트는 역비교로 동일해야 한다. 해당 원문에 예상 행·참조가 정확히 존재하지 않거나 다른 참조 보정이 필요하면 준비 실패로 보존하고 범위를 재판정한다. 원래 argv의 /nostdlib 및 /noconfig를 조용히 삭제하지 않는다. 새 입력·자원 개수는 Editor 실제 RSP와 물리 핀으로 산정하며 Input305/resource10을 복사해 확정하지 않는다. 현재 제품 소스590/v14, Movement 개별 성공 재사용 계보, Input 실제 성공과 ref 핀, 정확 source/parent 및 원래 한정 능력 원시4를 함께 결속한다. 새 계획·승인 파일의 물리 생성은 현재0이며 후속 정확 실행 승인에서만 CreateNew 각1개로 발급한다. 준비 단계는 새 디렉터리1·RSP1과 기존 원장의 불변 틀뿐이다. 부분 생성 실패를 보존하고 동일 접두사 재시도0이다.

**검사·종료 및 후속 경계:** 비실행 정적 판독1묶음·범위 내 보정 재읽기 최대2회를 허용한다. 소스/부모/RSP/wrapper의 역델타, 엄격 모듈·itemID·실제 입력 소비, 원래 Native 동작 불변을 닫는다. 실제 AST·Add-Type·Python·native·compiler 실행0이다. 새 source 지문과 독립 영향 판독 후 AST1 및 원래 관측 능력의 조건부 재사용은 별도 정확 판정을 받는다. 재사용은 반환 PI·출생Job 확인 이후의 기존6능력만이며 prePI/절대hardwall/loader/G8 일반화0이다. 종료는 정확 닫힌 핀·차집합·입력 목록·독립 판독 대상을 Root에 반환하는 시점이다.

후속 실행 후보는 supervisor1+Editor csc1의 native최대2와 메모리 Add-Type1이며 지금 실행을 선승인하지 않는다. 정상 전체60초에 최초QPC·전검사·Add-Type·폐쇄·봉인을 포함하고 실패 시에만 +65/+70 정리를 허용한다. clock5 단회 지연결속→authoritySHA→approval핀→planSHA의 비재귀 발급, 원래 상한·실제 source-before/after590·원시10개·폐쇄 후 출력핀·원시 acceptedfalse를 유지한다. 정확 계획/코드/실제 입력·루나 판독·아스트라 최종 단회 승인·실제 도구 심사 후에만 실행한다. 첫 실패 중단·재시도/재개/두 번째 호출0이다. 역사 r14 성공권한을 새 Editor 실행에 재사용하지 않는다.

REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012를 추적한다. 이번 아스트라 쓰기는 본 문서1 append만이며 원장/source/RSP 쓰기와 실행0이다. 접두사 보존·최종 지문 확인 뒤 문서 소유를 Root에 반환한다.
##### 10.10.21 영상 보조 프로그램의 고정 빌드 코드 준비 — Approved

2026-10-14 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 다음 실행 없는 코드 준비만 승인한다. 기존220465바이트/SHA256 8FF88588A90B143CF0584AD7C986F8F10933A56D838452CC794CBA17EA806DCF 접두사를 보존한다. 목적은 사용자 직접 플레이의 영상 관측 보조 프로그램을 설치된 도구만으로 빌드할 정확 입력과 감독 경계를 준비하는 것이다. r14 Input/r15 Editor 실제 한정 성공과 소진 원문, 제품590소스, 영상CPP61590바이트의 닫힌 원문은 변경0이다.

솔 단일 소유로 기존 output/chapter1-playable-r1/tools/m1-single-csc-supervisor.v1.py 한 파일에 고정 VideoCppCompileOnce 및 VideoCppLinkOnce 역할만 준비할 수 있다. 기존 CapabilityOnce/SourceCompilerOnce의 동작과 원문은 보존하고 새 역할로 기존 권한을 소비하지 않는다. 별도 위임 솔은 기존 Root 원장의 새 videoCppBuildParentPreparation.codeText 불변 항목 및 같은 준비 묶음의 계획 틀만 작성한다. Root는 원장 위임 중 병행 쓰지 않으며 반환 후 실행 통제를 소유한다. 임의 명령/모듈/도구 경로를 받는 범용 실행기화0, 새 helper/검사framework/QA·승인 사본0이다. 실제 RSP·디렉터리·계획/승인 파일·obj/exe 생성은 현재0이다.

컴파일 역할은 닫힌 CPP1과 설치된 정확 cl.exe의 /c만, 링크 역할은 별도 수용된 obj1과 정확 link.exe만 허용한다. 컴파일 후보 옵션은 /std:c++17 /EHsc /MT /utf-8 /X /showIncludes /sourceDependencies, 명시 include6개다. 링크 후보는 /MACHINE:X64 /SUBSYSTEM:CONSOLE /INCREMENTAL:NO /MANIFEST:NO /VERBOSE:LIB, 명시 lib경로3개·직접lib11개다. 정확 argv/순서/경로/출력 이름·개수·모든 부수 산출물·상한은 실행 전 닫힌 계획으로 명명해야 한다. 옵션의 실제 지원·성공은 준비로 입증되지 않는다. obj 실제 성공·폐쇄·핀·독립 수용 전 링크의 실제 입력 발급과 실행0이다.

부모 환경 상속 대신 SystemRoot/WINDIR/정확 PATH/TEMP/TMP/VSLANG만 명명한 유니코드 환경 블록을 구성한다. CL/_CL_/LINK/INCLUDE/LIB/LIBPATH는 부재를 검사한다. 정확 값과 환경 블록 전달·종료문자·크기·쓰기 가능한 TEMP/TMP 범위 및 부수 파일 처리도 계획에 닫는다. 포함·lib·도구 검색 디렉터리의 전체 목록/파일 핀을 고정하고 종료 후 대조한다. 실제 헤더 목록·링커진단의 입력을 고정 집합과 대조하며 누락/외부경로/판독불명은 실패다. 전후 동일성은 동시 변경 부재나 시스템 loader 전체 폐쇄의 증명이 아니므로 loaderAccepted=false를 유지한다.

신규 Job 제한 준비는 private active1/memory2GiB 및 outer active2/memory3GiB를 생성 전에 설정하고 readback으로 검증하는 범위뿐이다. 정확 flag·구조체 필드·정수 범위와 원래 생성 시 Job 귀속을 명명한다. escape/breakaway0, 원래 PI·질의worker·pipe·EOF·handle 폐쇄를 유지하고 최종 private total1/active0 및 outer total2/active0을 요구한다. 이 제한은 자식이 필요한 경로를 실패시키는 제한이며 자식 부재나 누적 생성수 사전 보장이 아니다. 예상 집계를 넘으면 실패로 보존하고 사후 한도 확대0이다. 새 API가 필요하면 명칭·ABI·이유를 판독 반환해 별도 승인받으며 임의 추가0이다.

단계별 후보 예산은 최초 요청1·재시도/재개0, 정상 전체120초와 실패 종료5초·회수5초, stdout/stderr 각1MiB다. 120초는 새 빌드 역할의 후보일 뿐 기존60초 역할을 변경하지 않는다. 실제 입력 발급·전검사·생성·폐쇄·봉인을 포함하고 실패 정리시간은 성공기한 연장이 아니다. 출력/의존성자료/계획/코드/오류의 유한 상한과 Root 반환 원문 봉인 시각을 실제 실행 계획에서 확정한다. 현재 프로세스 시작/Add-Type/구문검사/컴파일/링크/캡처0이다.

준비 검사는 비실행 정적1묶음과 보정 재읽기 최대2회다. 루나는 고정 역할·환경·입력 폐쇄·Job 제한·기존 경로 역델타를 한 묶음으로 판독한다. 종료는 source/부모/틀의 정확 핀과 독립 판독 자료의 Root 반환이다. 기존6능력 중 불변 부분만 독립 영향 판독과 아스트라 재사용 판정 대상으로 삼는다. 신규 환경 전달과 Job 제한/readback·중첩 적용은 기존 증거로 수용하지 않는다. 정확 코드가 닫힌 뒤 기존 고정 검증자식을 사용하는 최소 한정 능력 관측 또는 새 빌드 첫 단계와 결합한 검증의 입력·수량·실패 기준을 별도 판정한다. 두 방식 모두 실제 실행 전 정확 승인 필수이며 이번 준비는 관측 횟수를 발급하지 않는다.

REQ/AC-C1B-G3OBS-005, REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012에 추적한다. 실제 빌드 성공도 캡처 권한·인코더·연속영상·Finalize시간·SourceReady·GUI·Play·B1/CH1 수용이 아니다. 이번 아스트라 쓰기는 문서1 append만이며 원장/source/빌드쓰기 및 실행0, 접두사와 최종 지문 확인 후 Root에 문서 소유를 반환한다.
## 22. 동일 영상 자식·동일 캡처 항목의 실제 크기 결속 준비 — 2026-10-14

상태: Approved — 아스트라 `gpt-6-astra`의 도구 기본 설정 판정에 따라 준비 구현만 승인한다. §21까지의 원문225835바이트와 과거 실패·수용 기록을 보존한다. REQ/AC-C1B-G3OBS-005/006/007/012, M1COMP-R1-002/004/006 및 PROC-011/012를 추적한다.

사용자 동작·산출물은 기존 사용자 직접 플레이와 Unity 수신 지정 입력·단일 Unity 창의 연속 영상이다. 루트가 가시 실행·원래 프로세스 핸들을 소유하고, 솔이 한정 준비 구현, 루나가 통합 독립 판독, 아스트라가 정확 실행 권위와 최종 통합을 맡는다. 이번 쓰기는 기존 `b1-window-video-capture.v1.cpp`1개와 기존 작업 원장의 관리자 코드·고정 입력 틀에 제한한다. 새 도우미·범용 실행 기능·생산 소스590·키·세대·프로필·동결 원문 변경은0이다.

GetClientRect·GetWindowRect와 화면 조작 도구의 논리 픽셀 크기는 WGC의 GraphicsCaptureItem.Size 실측을 대신하지 않는다. PREPARE는 정확10순서의 사례당 영상 자식1·동시1·캡처 항목1을 준비 권위로 생성한다. 원래 Unity·관리자·정지 사건과 출력2의 정확5개 핸들만 상속하고 입력 핸들·새 사건 추가0이다. CAPTURE는 자식이나 항목을 재생성하지 않고 최종 계획 게시로 촬영을 허용한다. 첫 실패·필수 사실 미확인 시 나머지0, 동일 사례 재시도·Unity 재시작·예비 Play0이다.

준비 권위에는 세션·사례·실제 원래 핸들과 프로세스 신원·소스/실행파일/계약/창 선택 핀·HWND/thread·창 크기·출력 경로·기존 절대 시계와 상한·고정 최종 입력 경로를 결속한다. 준비 중 framepool·촬영 session·MP4 생성은0이다. 같은 항목을 생성한 뒤 Closed를 구독하고 실제 Size를 알리는 행은32KiB 이하다. 실제 자식 신원·창·크기·QPC·준비 계획 지문과 정확 행 바이트의 지문을 보존한다. 크기 짝수·상한과 변경/폐쇄 조건을 검사하며 추정 크기를 실측으로 표기하지 않는다.

루트가 고정 proposal·최종 authority·최종 selection 각1을 CreateNew·flush·close로 준비한다. 관리자는 CAPTURE에서 해당 바이트·지문·실제 크기 알림·불변 준비 튜플을 검증한 뒤 고정 finalplan1을 CreateNew/FileShare.None으로 쓰고 flush·close한다. 이 방식은 작성 핸들 소유 중 부분 판독을 막으며 파일 전체 원자 교체·저장장치 영속성·동기 I/O 강제 시간 보장을 주장하지 않는다. 경로·작성자·파일 수와 허용 튜플 차집합은 고정 입력 틀에 확정한다.

CPP는 finalplan 최초 읽기 개방에서 ERROR_FILE_NOT_FOUND·ERROR_SHARING_VIOLATION만 유한 대기로 처리한다. 최초 성공한 동일 읽기 핸들을 WRITE/DELETE 공유 없이 보유하여 최대64KiB를 한 번 소비한다. 빈 내용·부분 내용·상한·형식·다른 오류·권위/크기 알림 불일치는 첫 실패이며 안정화 재시도0이다. 실제 소비 지문은 증거이며 실행 권위를 대신하지 않는다. 기존 executionTuple/acceptedPins 검증과 정확 알림 지문 결속을 유지하고 승인·계획 지문의 재귀 의존0이다.

대기마다 정지 사건·원래 Unity/관리자 종료·Closed·기존 사례/전체 절대 기한을 확인하고, 최종 초기화 직전에 같은 항목의 Size·창 신원·Closed를 다시 확인한다. 준비·권위 대기·초기화는 사례1800초와 전체18000초에 포함하며 종료5초·회수5초를 증액하지 않는다. 원래 자식 핸들을 보유한 부모가 종료/EOF를 관측한다. 영상 writer 폐쇄는 직접 관측과 자식 증거를 구분한다. 동기 COM/I/O·셸 급사 정리·실제 연속성은 미입증으로 남긴다.

사례 사이 ACCEPT는 루트의 정확 외부 진행 영수증만 소비하고 최종 게임 수용을 주장하지 않는다. 독립 검수는 기존10 관측 후 방별6 묶음으로 유지한다. 이번 준비의 정적 판독1묶음·보정 재읽기 최대2회와 공유 파일 단일 소유를 적용한다. 종료 조건은 실제 차집합·API/ABI·정리·권위 결속을 닫은 준비 산출물과 통합 루나 판독이다. 변경 소스의 새 r4 빌드·실행은 별도 정확 승인 대상이다. 기존 r3 시간 실패·준비 전용 산출물 수용을 보존하며 이번 승인으로 빌드·Unity 시작·Play·촬영 권한을 발급하지 않는다.
##### 10.10.23 표준입력 연결 실패 후 가시 Unity 시작의 별도 단회 호환 — Approved

2026-10-14 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 아래 정확 단회 권위만 승인한다. 기존230477바이트/SHA256 C8F411EA77B8D27EA3B485952B0A8BFDD72F8B123EB044F0DA49EE147B5BE6B3 접두사, G3 동결 문서 및 실패 원문은 보존한다. 사용자 산출물은 현재 프로젝트의 실제 가시 Editor와 관측 준비 상태이며, 본편1챕터 직접 플레이 목표의 선행 준비다. 실제 플레이·SourceReady 수용을 대신하지 않는다.

기존 관리자1은 Add-Type CS0170으로 실제 Unity 시작0에서 실패했고, 관리자2는 실제 Unity PID74852 시작1 후 plain pipes 표준입력의 즉시 EOF로 실패하여 원래 프로세스 종료·폐쇄를 기록했다. 두 시도는 소진됐으며 기존 재시작0 예산을 재사용하지 않는다. 이후 비Unity tty 연결1에서 READ/STOP의 정확 두 행과 정상 종료가 관측됐고, 출력 반환 검사에서는 실제 WriteAsync Task의 GetResult 정수0을 확인했다. 과거 두 0행의 원인은 확정하지 않는다. prep3는 GetResult2곳의 void 캐스트와 Library 로그 r2 경로3곳 및 코드의 계획SHA1곳만 변경했으며 독립 전체 역치환·P0/P1=0, PSParser1 오류0/0.4344153초를 결속한다. C#13296바이트/SHA256 15E13B1D54FEFDEEDF36BB668EE1320C56A41D47F47DC6E776071486E58F29F6은 기존 실제 메모리 컴파일 원문과 동일하여 그 한정 증거만 재사용한다.

정확 실행 입력은 기존 원장 unityVisibleOwnedStartPreparation3의 code45356바이트/SHA256 473FC5A65D0830D2C35114A4CF1629613D838BC49E379B213F756EF462EA2C75 및 plan22636바이트/SHA256 12AE2BEDAB2B1E80855FF7819C2B88736FA22C5E79730BC38B10D6ECF27B2BE3다. 실행 연결은 tty:true이며 새 고정 Library/b1-unity-g3-owned-r2 로그 경로를 사용한다. Root만 관리자 호출1·추가 Unity 시작 최대1을 소유한다. 과거 실제 Unity 시작1은 별도로 유지하여 역사 합계를 숨기지 않는다. 정확 기존 Unity.exe279976바이트/SHA256 748952E2BE083672D36FCA166483291A93A064EB898342492B7596723A917EB4와 고정 프로젝트, 실제 checkpoint125747바이트/SHA256 579F64500359B96F0C8B703711C89BE5F9FF2A85A2DB996C42C336B73067882F 및 제품590소스를 결속한다.

호출 전 Root는 전체 코드/계획/실행파일 지문·프로젝트·로그 경로 부재·미소비를 확인하고 정확 외부 실행 권위를 기록한다. 준비 원문의 false/null을 소급 수정하거나 이를 실제 런타임 핀으로 소비하지 않는다. 새 최초 QPC부터 전체18000초에 시작·가져오기·컴파일·대기를 포함하고 종료5초·회수5초를 유지한다. 시계 재발급·자동 재호출·재시작0, 첫 실패 뒤 나머지0이다. 원래 Process/SafeHandle·PID·생성시각 소유를 유지하고 이름 종료·PID 재열기·다른 앱 종료0이다. 동기 API/IO의 절대 시간 보장은 미입증이다.

초기 허용 명령은 READ/BIND/STOP뿐이다. 실제 sky 반환 단일 창을 선택·활성화한 뒤 직전 미선택 후보와 실제 전경 HWND/thread/원래 PID·생존을 대조해 BIND를 단회 확정한다. opaque sky ID를 HWND로 변환하지 않는다. BIND 실패 후 재선택0이며 BIND 전 후보 변화와 이후 영구 대상 고정을 구분한다. UI 입력은 승인된 sky 경로만 사용한다. 가져오기/컴파일 오류·인증/보호 동의가 나타나면 진행 중단 및 계획된 STOP 정리를 수행하고 동의 화면을 자동 처리하지 않는다.

메뉴·관측 타입·프로젝트 로드 상태는 실제 관측 사실로 기록하되 수동 컴파일/발견만으로 SourceReady를 수용하지 않는다. RESERVE/PREPARE/CAPTURE/Play·진단230·프로필·게임 입력·영상 실행은 별도 정확 사례 권위 전0이다. 앱 동의는 사용자가 수행하며 사용자 직접 플레이 정책은 유지한다. 최초 R01-A·기존10구성/각1800초·키/세대·Q0·필수9회·원격 관문은 변경0이다. 이번 새 시작 시간 예산은 현재 단회 세션에만 적용하며 향후 사례 권위가 발급되더라도 같은 세션의 경과를 초기화하지 않는다.

Root는 실제 입력·출력·시각·종료/폐쇄·부분 실패를 기존 원장에 보존하고 루나는 실행 결과를 독립 판독하며 아스트라는 후속 한정 수용을 판정한다. REQ/AC-C1B-G3OBS-006/007/010/012, REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012를 추적한다. 실제 도구 심사는 별도이며 본 절의 정확 입력과 호출 전 조건 충족 때만 새 단회 권위가 유효하다. 이번 작성은 문서1 append만으로 실제 관리자/Unity/GUI 실행0이며, 접두사와 최종 지문 확인 후 문서 소유를 Root에 반환한다.
##### 10.10.24 Unity 초기화 실패 후 일반 로컬 실행 맥락의 별도 단회 호환 — Approved

2026-10-14 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 아래 정확 환경 비교 단회 권위를 승인한다. 기존235385바이트/SHA256 7EF6A1E48C700235AEFF4336BF6EB670CA009BB9805C9DBF4B33424EB6499C2D 접두사·G3 동결 원문·기존 실패·소진은 보존한다. 사용자 산출물은 정확 현재 프로젝트의 가시 Editor 초기화 관측이며 본편1챕터 직접 체험 목표의 선행 준비다. 게임 소스를 고치거나 실제 플레이를 수용하는 권위가 아니다.

직전 prep3는 실제 Unity PID48880 시작 후83.0052522초에 자체 종료했고 원래 프로세스 종료·Dispose 및 정리0.1452223초가 관측됐다. BIND/GUI입력/Play/캡처0이다. editor.log36962바이트/SHA256 4B8F037DBD78DA283B7D756EFB6C4B9D51B7FEE993955973351AC95765605218의 라이선스 IPC 초기화 실패와 소켓/Mono 초기화 충돌은 제한 실행 환경과의 충돌 가능성을 뒷받침하지만 원인은 미확정이다. 기존 실제 Unity 시작 누적2회를 유지한다. 재시작0인 기존 권한을 재사용하지 않고 일반 로컬 실행 맥락에서 동일 초기화를 관측할 새 목적의 단회만 명명한다.

정확 입력은 기존 원장 unityVisibleOwnedStartPreparation4의 code45356바이트/SHA256 520326C5B228C4532C6658585FD578CD9FAACBDD9DC1C98DE09013DD3A7F14E9 및 plan22636바이트/SHA256 BF7BB68CDA48E042E7F34163F9F7A4D5F5C7DABE01E36DD38E9C68C86D0A4CC9다. prep3 대비 Library 로그 r2에서 r3로 경로3곳과 코드의 계획SHA1곳만 바뀌었다. 루나의 전체 역치환·새P0/P1=0과 실제 PSParser1 오류0/0.4230226초를 결속한다. C#13296바이트/SHA256 15E13B1D54FEFDEEDF36BB668EE1320C56A41D47F47DC6E776071486E58F29F6은 동일 원문이며 기존 메모리 컴파일 증거만 한정 재사용한다.

Root 단독으로 tty:true 및 exec_command의 sandbox_permissions=require_escalated 정상 승인 절차를 사용한 추가 관리자 호출1·Unity 시작 최대1을 요청할 수 있다. 이 계약은 도구 승인을 대신하지 않으며 실제 심사 허용 전 실행0이다. 거절·불명확한 승인 결과는 원문으로 보고하고 다른 도구/셸/호스트로 우회하지 않는다. require_escalated 실행은 관리자 권한 상승이나 전역 보안 설정 변경의 허가가 아니다. 글로벌 보안·방화벽·권한 설정·설치·새 Unity 옵션·자격증명 추출·로그인/보호 동의 자동화0이다. 앱 보호·로그인 동의가 실제 요구되면 사용자가 처리해야 하며 현재 호출은 진행 중단·계획된 STOP 정리 경계를 유지한다.

호출 전 Root는 전체 코드/계획 핀, 기존 Unity.exe279976바이트/SHA256 748952E2BE083672D36FCA166483291A93A064EB898342492B7596723A917EB4, 고정 프로젝트·체크포인트125747바이트/SHA256 579F64500359B96F0C8B703711C89BE5F9FF2A85A2DB996C42C336B73067882F·제품590, 새 Library/b1-unity-g3-owned-r3 로그 경로 부재와 미소비를 확인한다. 새 최초 QPC부터 정상 전체18000초와 종료5초·회수5초를 적용한다. 시작·가져오기·컴파일·대기는 전체에 포함한다. 원래 Process/SafeHandle·PID·생성시각 소유를 유지하며 자동 재시작/재호출·PID 재열기·프로세스명 일괄 종료0이다. 이번 실제 시작이 일어나면 기존2회와 별도로 누적3회로 기록한다. 최초 실패 뒤 나머지0, 동기 호출의 절대 시간 보장0이다.

초기 명령은 READ/BIND/STOP뿐이다. sky가 반환한 단일 창의 선택·활성화 뒤 직전 미선택 후보와 실제 전경 HWND/thread/원래 PID·생존을 확인하여 BIND를 단회 확정한다. opaque ID 변환·실패 후 재선택0이다. 준비 원문의 false/null은 역사로 보존하고 실제 권위·핀을 외부 결속한다. SourceReady·RESERVE/PREPARE/CAPTURE·Play·진단230·게임 입력·프로필·촬영은 별도 정확 권위 전0이다. 사용자 직접 플레이 정책·키/세대·Q0·필수9회·기존10구성·원격 관문은 불변이다. 나중에 같은 세션의 사례 권위가 발급돼도 이 세션의 시작 시각을 초기화하지 않는다.

Root는 실제 도구 심사·연결·초기화·로그·시각·종료/폐쇄·부분 실패를 기존 원장에 보존한다. 루나는 결과를 독립 판독하고 아스트라는 후속 한정 수용을 판단한다. REQ/AC-C1B-G3OBS-006/007/010/012, REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012를 추적한다. 종료 조건은 정확 단회 결과와 미수용 상태의 원문 반환이다. 이번 아스트라 작업은 문서1 append만이며 관리자/Unity/GUI 실행0, 접두사·최종 지문 확인 후 Root에 문서 소유를 반환한다.
##### 10.10.25 원시 제어 출력 보정 후 가시 Unity 시작의 별도 단회 호환 — Approved

2026-10-14 아스트라 gpt-6-astra, 추론 설정 `도구 기본값, override 없음`이 정확 prep5의 아래 단회 실행을 승인한다. 기존240254바이트/SHA256 835739659CB641702385C43A977B8C75EEDD97B06B06653FB5F38F317E671B81 접두사와 G3 동결 원문·실패·소진을 보존한다. 목적은 현재1챕터 프로젝트의 가시 초기화와 원시 제어 운송을 확인하여 사용자 직접 플레이의 선행을 닫는 것이다. 제품590·CPP·배치된 r4 exe 및 게임 요구사항은 변경0이다.

직전 세션은 원래 Unity PID79780의 가시 창과 BIND까지 관측했으나 TTY 화면 줄바꿈/커서 처리로 원문 JSON의 전송 동일성을 인증할 수 없었다. 수신 descriptor의 Pump flush가 EOF에만 있어 짧은 준비 행이 버퍼에 남을 잠복 순환도 발견됐다. 실제 RESERVE/자식/Play/캡처0을 유지한 채 미무장 창을 닫고 STOP으로 원래 프로세스 종료·Dispose를 관측했다. STOP는 원래 핸들의 강제 종료이며 정상 저장 종료가 아니다. 기존 실제 Unity 시작 누적3회를 보존하며 이 절은 기존 권한의 재시도가 아닌 별도 실패 호환 단회다.

정확 입력은 unityVisibleOwnedStartPreparation5의 code47083바이트/SHA256 FF58FA1EEE6AB7EE4AE81BF8417BC5EACC45607E3E14A48221D68E3D60CF8840 및 plan22793바이트/SHA256 6096195F624C9EA78688837DA7FD40F50F13E23E178F0DDE797F73FC59C6E2AC다. 독립9코드변경 역치환 및 계획 역복구·추가P0/P1=0, 실제 PSParser1 오류0·분리 C# 메모리 Add-Type1 성공/0.8628192초를 결속한다. C#13387바이트/SHA256 FCFB183EA2011A646C7E5AD148093879C4C79190341531E6E5B09023D34E3BAC는 이 검사 원문이다. 구문/컴파일 통과는 실제 출력 가시성·이중 전달·IO 기한의 동작 수용이 아니다.

Root는 tty:true와 require_escalated 정상 도구 심사를 거쳐 관리자 호출1·추가 Unity 시작 최대1·런타임 메모리 Add-Type1만 실행할 수 있다. 실제 도구 거절은 우회하지 않는다. 호출 전 전체 코드/계획·기존 고정 Unity exe·프로젝트·체크포인트·제품590·배치 r4 exe 지문, 새 Library/b1-unity-g3-owned-r4 경로 부재와 미소비를 확인한다. 새 로그/control 산출물은 정확 editor.log와 control.jsonl뿐이며 기존 사례 영상/권위 파일 생성0이다. 코드의 준비 false/null을 실제 동적 핀으로 소비하거나 동결 원문을 소급 수정하지 않는다. 이번 실제 시작 시 역사 누적4회를 기록한다.

새 최초 QPC부터 정상 전체18000초, 종료5초·회수5초를 적용하고 시작·가져오기·컴파일·관측·대기를 포함한다. 원래 Process/SafeHandle·PID·생성시각을 유지하며 첫 실패 뒤 나머지0·자동 재호출/재시작0·PID 재열기/이름 일괄 종료0이다. STOP의 실제 종료코드·강제 종료·폐쇄를 기록하고 정상 저장으로 표현하지 않는다. 동기 IO/Native 호출의 절대 시간 보장0이다.

control.jsonl은 고정경로 CreateNew·단독 writer·쓰기 전 누적1MiB 한도를 적용한다. Emit의 같은 UTF8 행 바이트를 기록/flush하고 어느 출력 경로의 실패도 숨기지 않는다. Root는 파일의 완결된 행만 원시로 소비하고 부분 행·중복·누락·잘림·기록 실패를 발견하면 중단한다. TTY 원시와 제어 파일은 분리 보존하며 화면 복원 후보를 권위 원문으로 대체하지 않는다. writer 폐쇄 후 외부 길이/SHA를 기록한다. 자식 Pump의 Write 뒤 Flush(false)는 준비행 가시성을 위한 보정이며 실제 descriptor의 생산자 길이/SHA·엄격 검증을 대신하지 않는다. 이번 실행에서 자식 준비는 허용0이므로 그 실제 효과도 아직 수용0이다.

초기 명령은 READ/BIND/STOP만 허용한다. sky가 실제 반환한 단일 창 선택·활성화 뒤 직전 후보/실제 전경 HWND/thread/원래 PID·생존을 확인해 BIND를 단회 확정한다. opaque sky ID의 HWND 변환·실패 뒤 재선택0이다. 같은 세션에서 실제 Game 탭 초점화와 정확 Basic Movement Evidence 메뉴1회의 미무장 창 관측까지 허용한다. 기존 메뉴 본문 범위를 넘어 준비/Apply/진단/Play 버튼은 누르지 않는다. 실제 로그/가져오기 오류 또는 인증/보호 동의가 요구되면 진행을 중단하고 계획된 STOP 정리를 수행한다. 보호 동의·로그인은 사용자 소유이며 자동화0이다.

이전 PID/UTC/QPC/HWND/GameView ID·중립값·사례 권위를 재사용하지 않는다. 새 실제 신원을 관측한 뒤 최초 R01-A의 정확 별도 권위를 결속한다. SourceReady·RESERVE/PREPARE/CAPTURE·Play·진단230·게임 입력·프로필·촬영은 이번 승인에 포함하지 않는다. 키/세대·Q0·필수9회·기존10구성·원격 관문·사용자 직접 플레이 정책은 불변이다. 이후 권위가 발급돼도 같은 세션의 시계를 초기화하지 않는다. 보안/방화벽/권한 설정·설치·새 Unity 옵션·자격증명 추출·새 framework0이다.

Root는 원시 제어·TTY·로그·실제 신원·시각·종료/폐쇄를 기존 원장에 보존한다. 루나는 실제 결과를 독립 판독하고 아스트라는 후속 한정 수용을 판단한다. REQ/AC-C1B-G3OBS-005/006/007/010/012, REQ/AC-M1COMP-R1-002/004/006 및 REQ/AC-PROC-011/012를 추적한다. 새 부록 지문을 코드에 재귀 삽입하지 않고 개별 코드/계획 핀과 외부 권위로 결속한다. 이번 작성은 문서1 append만이며 원장/제품/도구/실행0, 접두사 및 최종 지문 확인 뒤 문서 소유를 Root에 반환한다.
