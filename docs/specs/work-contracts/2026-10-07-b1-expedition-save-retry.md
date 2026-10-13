# B1 출정 저장 실패 후 새 입력으로 저장 재확인

- 상태: **Approved** — §8의 아스트라 한정 승인으로 정확 여섯 파일·두 실패 분류·새 입력 복구·35검사 범위만 채택한다. 제품 소유 인계·구현·실행·독립 수용은 별도다.
- 작성: 솔 `gpt-6.1-sol`, `ultra`, 작업 `sol61_b1_save_retry_contract`. 승인·통합: 아스트라 `gpt-6-astra`. 독립 검수: 루나 `gpt-6-luna`, `max`. 이 문서가 주 대화 설정을 변경하지 않는다.
- 권위: [CH1 계약](./2026-10-03-chapter1-playable-delivery.md) §11.6~11.7/12/19~24, [실제 플레이 전략](./2026-10-03-play-first-delivery-strategy.md), ADR-0039/0041/0042/0043. 기존 §24 최소 컴파일 보정과 별도 승인 묶음이다.
- 사용자 동작·산출물: 출정 저장 오류를 확인하고 입력을 놓은 뒤 `저장 다시 확인`을 한 번 요청한다. 이전 거점의 실제 입력 종료 뒤 새 거점이 준비된다. 새 거점에서도 입력을 놓고 별도 새 `출정` 입력을 보내야 첫 방에 진입한다.
- 실제 선행: 현재 568개 입력 기준 재대조, 루나의 이 초안 사전 검토, 아스트라의 정확 범위 승인, 별도 솔 구현·루나 델타 검수. 출정이 필요한 실제 장치 검사는 기존 여섯 방 기본 이동 수용 이후다. 게시·Q0 완료를 모든 로컬 설계의 선행으로 추가하지 않는다.
- 종료 조건: 허용 실패/차단 실패의 분류, 실제 중립·새 의도·파기·새 준비·별도 출정·저장 결과·첫 완료 표시의 연속 증거 및 독립 검수. CH1 완주·빌드·원격 게시 수용을 뜻하지 않는다.

## 1. 현재 입력과 실제 누락

기준 프로젝트는 `output/chapter1-playable-r1/project/`다. 원본 `Assets/`와 다른 프로젝트는 수정하지 않는다. 구현 전 기준은 `output/chapter1-playable-r1/evidence/b1-final-source-checkpoint-sol61.v4.json`, SHA256 `8F67CDB99E8CD5CCBF968623640BBB09103E03FDDAF8EC043F7ABC98E9215159`, 정확 568개다. 이번 정적 재대조는 누락·길이·SHA 불일치 0이며 실행 결과가 아니다. `Runtime/Run/AssemblyInfo.cs`는 §24의 273바이트/SHA256 `25C6517A094023C9F3E9E2855F171E61A4A81BC1D245613D3EBA7261CDD0437A`를 유지한다. 세 신규 B1 검사 파일의 정적 선택자는 정확 31개다.

`Chapter1PlayableFlowOwner.HandleHubRequest`는 Hub/ReturnedToHub만 받아 요청을 먼저 소비한다. `RequestExpedition`은 Saving에서 실제 프로필 재관측·계획 선택·공개 저장을 수행하고, 예외는 `Stop`으로 보내 영구 Diagnostic 상태가 된다. Update/FixedUpdate가 Diagnostic을 건너뛰므로 §11.7의 저장 실패 후 새로운 중립·새 입력 재요청 경로가 없다. 저장 실패 표시도 현재는 일반 진단 문구이며 전용 출정 실패 문구가 아니다.

`Chapter1DevelopmentProfile`은 공개 Observe/Select와 실제 prepared 문서를 대조한다. 보존·복구 필요나 문서 불일치면 거절하며, 저장 결과가 정확 확정이 아니면 `_ready=false`로 닫는다. 이 객체를 다시 활성화하는 방식은 사용하지 않는다. `Chapter1HubLifetimeOwner.BeginRetirement`의 closure는 실제 InputRouter 파기 및 actions 종료를 확인한다. 새 prepared 거점만이 기존 adapter의 관측·보존·복구·저장 수명을 새로 수행할 수 있다.

`InputRouter.CommitHubUiOnlyFrame`의 `_chapterHubNeutralReady`는 최초 실제 중립 뒤 다시 false로 돌아오지 않는다. 따라서 Diagnostic→Hub만 바꾸거나 입력 edge가 없다는 사실을 release로 취급하면 실패 당시 held Submit/Click/Navigate를 새 중립으로 오인한다.

### 기존 저장 실패 검사의 원인 표기 정정

`SaveFailureShowsErrorWithoutRunStarted`는 `profile.json`을 `FileAccess.Read/FileShare.Read`로 연다. 공개 Observe의 실제 `ReadAllClosed` 역시 Read/Read이므로 이 잠금은 재관측 읽기를 허용한다. 저장의 `ReadAllExclusive`는 `FileShare.None`이며 `SaveHeld`의 ExistingPrimaryValidation에서 실패하면 실제 FailedBeforeCommit을 반환한다. 현재 검사의 “재관측 읽기 실패·저장 결과 미발급” 주석은 정적 소스와 모순된다. 기존 원문·역사 증거는 보존하고, 같은 시험 파일의 현재 주석·assertion을 실제 결과에 맞춰 보정한다. 이번 작업은 그 파일 잠금을 실행하지 않았으므로 실제 OS 결과까지 단정하지 않는다.

실제 `profile.tmp.json`을 생성·배타 잠금하는 출정 시험은 프로필 생성자의 Temp 관측이 먼저 읽기 불가/보존 필요가 될 수 있다. 이를 TempWriteFlushClose 실패라고 단정하는 검사도 채택하지 않는다. 정확 실패 위치는 실제 batch/Outcome/FailureStage로 확인한다.

## 2. 한정 변경 파일과 소유

다음 여섯 **기존 파일만** 향후 구현 후보로 허용한다. 새 파일·meta·GUID·어셈블리·참조·friend·InputAction·장치 콜백·테스트 전용 우회 입구는 0이다. 이 표는 Review이므로 현재 쓰기 권한은 문서와 준비 원장 두 파일뿐이다.

| 프로젝트 상대 경로 | 필요한 최소 변경 | 한정 소유·영향 |
|---|---|---|
| `Assets/AcadeGameMaker/Runtime/Run/Unity/Chapter1PlayableFlowOwner.cs` | 출정 단계별 실제 관측·결과 분류, 오류 보존, retry 의도 검증, 기존 거점 종료 후 새 준비 | 솔 구현자 한 명. 실패 복귀/보스/선택 권위 변경 없음 |
| `Assets/AcadeGameMaker/Runtime/Run/Unity/Chapter1RunView.cs` | 세 복구 표시 단계와 불변 오류·현재 시도 결과 투영 | 표시가 저장·중립·출정 권한을 만들지 않음 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1HubLifetimeOwner.cs` | 실제 소비 token 보관, 일회 재무장, 새 정확 retry intent의 기존 발급 경로 | pending/객체 동일성/단일 endpoint 유지 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | 실제 CH1 거점의 중립 재무장·새 입력 update와 완료 frame 관문 | InitializeHubUiOnly/CommitHubUiOnlyFrame 및 한정 내부 관문. 일반 거점·게임플레이 수명 불변 |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/Chapter1ExpeditionPresenter.cs` | 기존 출정 버튼 위치에 재확인 문구·의도, 복구 중 잠금·오류 표시 | 기존 여섯 버튼 저작 구조·safe frame·cursor 유지 |
| `Assets/AcadeGameMaker/Tests/PlayMode/RunUnity/Chapter1ExpeditionPlayModeTests.cs` | 기존 검사 보완 및 §6의 정확 신규 네 검사 | 실제 InputTestFixture 장치·저작 장면·공개 저장. 기존 31 삭제/Ignore 없음 |

`Chapter1DevelopmentProfile.cs`, `DesktopProfileLaunchAdapterV1.cs`, Profile 소스, `Chapter1EncounterLifetimeOwner.cs`는 읽기 입력이며 수정하지 않는다. 프로필 생성자를 바꿀 필요 없이 FlowOwner가 공개 Observe의 실제 batch를 **읽기 전용으로 먼저** 확인하여 저장 호출 전 읽기 불가를 구분한다. 그 다음 기존 생성자의 재관측도 그대로 유지한다. 추가 관측과 생성자 사이 변경·prepared 불일치는 Diagnostic으로 닫는다. 추가 파일이 필요하다는 새 근거가 나오면 이 표의 승인을 확대하지 말고 아스트라에게 정확 차이를 인계한다.

## 3. 실제 실패 분류와 보존

**REQ-C1B-SRETRY-001 — 복구 가능한 범위.** 실제 실제 출처에 결속된 출정 token을 한 번 소비한 현재 시도이며, 아직 그 시도의 원정키·게임플레이 graph·RunStarted가 발급되지 않았고 같은 거점/라우터가 살아 있는 경우만 아래 두 분류를 허용한다. 이전 원정의 누적 RunStartedRevision이 0이어야 한다고 요구하지 않는다. 이번 시도 시작 때의 실제 카운터와 비교하여 이번 시도의 발급 0을 검증한다.

| 실제 출처 | 현재 시도 결과 | 처리 |
|---|---|---|
| 인증된 고정 root에서 공개 Observe가 반환하고 Validate를 통과한 batch의 Primary/Previous/Temp 중 실제 Kind=Unreadable, 생성자·Select·Save 호출 전 | `ReadUnavailableNoSaveIssued`; 저장 Outcome/CommittedRevision은 미발급 | 전용 출정 실패 상태와 새 중립 재무장. 관측 batch 원문 사실 보존 |
| 기존 공개 SaveForExpedition이 실제 반환하고 Validate를 통과한 `FailedBeforeCommit` | 실제 Outcome/FailureStage/세 파일 상태/CommittedRevision=-1 및 실제 기대 revision | 전용 출정 실패 상태와 새 중립 재무장. temp 존재·파일 상태를 임의 복원하지 않음 |
| `CommitOutcomeUncertain`, 유효한 확정 Outcome이지만 기대 revision 불일치, 저장 호출의 결과 미발급 예외 | 실제 결과 또는 기술 오류 그대로 | 기존 Diagnostic 유지, 재확인·재출정 intent 수용 0 |
| 고정 root/링크/권위/receipt/token/catalog/산술/프로필 생성자·선택·왕복/graph 오류, 공개 Observe 자체의 예외 또는 잘못된 batch | 기술 원인 그대로, 저장 실패 Outcome으로 변환하지 않음 | 기존 Diagnostic 유지 |
| SavingReturn의 어떤 실패·불확실 결과, 이번 시도 RunStarted/graph 발급 이후 오류 | 실제 기존 실패 및 누적 카운터 | 기존 Diagnostic 유지, ReturnedToHub/복구 권한 새 발급 0 |

인증된 root 검증과 `_catalog.Validate(true)`는 복구 분류 밖에서 수행한다. 공개 Observe가 던진 IOException만 보고 읽기 불가로 간주하지 않는다. 해당 예외는 root 잠금·reset barrier·경로 안전 실패일 수도 있다. 실제 batch의 Unreadable만 미발급 읽기 분류의 근거로 사용한다. 이를 FailedBeforeCommit으로 이름 바꾸지 않는다. 한 시도에서 batch가 읽기 불가면 이전 파일 선택/default 선택을 호출하지 않는다.

**REQ-C1B-SRETRY-002 — 사실과 오류 보존.** FlowOwner는 현재 시도의 양수 단조 증가 식별값, 실제 소비 token 객체/ordinal·거점 generation·prepared receipt·UI receipt, 요청 seed/명시 검증 여부, 시도 시작 RunStartedRevision, 발급된 batch 또는 저장 결과, 기대 revision이 실제 계산됐는지, 오류 원인을 불변 기록으로 보관한다. 미발급 결과는 null/별도 분류이며 이전 저장 결과를 이번 결과로 쓰지 않는다. 마지막 실패 기록은 새 준비나 성공 출정으로 덮어 지우지 않는다. 현재 진행 오류와 최초 출정 오류를 구분하며 복구 중 추가 실패도 사실대로 보존한다. 읽기 전용 투영만 노출하고 값 setter·발급 factory는 추가하지 않는다.

원래 `저장에 실패하여 출정하지 못했습니다`를 표시하고 원인을 함께 보여 준다. 이 상태는 RunStarted나 성공 복귀 표시가 아니다. 계획은 출정 후보로만 보존하며 마지막 제안 seed를 메모리에서 확정한 것으로 표시하지 않는다. 파일 원문·revision·지속 필드는 공개 준비/저장의 실제 결과만 따른다.

## 4. 실제 중립·의도·거점 교체

**REQ-C1B-SRETRY-003 — 실제 소비 객체로 한 번 재무장.** 기존 `ConsumePendingRequest`가 소비한 실제 token을 Hub의 비공개 슬롯에 보관한다. 한정 내부 `ArmExpeditionRetryAfterFailure(endpoint,consumedToken)`은 endpoint/owner/router/generation/prepared/현재 frame/ordinal이 같은 실제 소비 객체인지, `_dispatching` 안에서 pending이 이미 소비됐는지, 이번 token의 재무장이 아직 없는지 대조한다. 복사값·외부 owner/router·이전 세대·미소비·중복 재무장은 거절한다. 검증을 마친 뒤 일회 재무장 상태를 먼저 고정하고 Router의 중립 관문을 호출한다. 실패하면 Diagnostic으로 닫으며 자동 재무장하지 않는다. 이 API는 token을 새로 발급하는 입구가 아니다.

Router의 한정 내부 입구는 해당 GameObject의 실제 CH1 Hub owner가 현재 그 token의 재무장 호출을 수행 중인지 다시 확인한다. UIOnly/활성/실제 prepared publication/동일 현재 receipt/미fault/미disposed/실제 adopted actions 소유 조건을 모두 유지한다. receipt의 값만 같은 외부 호출로 중립 래치를 바꾸지 못한다. 원래 과거 중립 true를 즉시 재사용하지 않는다.

**REQ-C1B-SRETRY-004 — 입력 update와 완료 frame의 새 중립.** 초기 새 CH1 거점과 실패 후 재무장 모두 같은 물리 관문을 적용한다. 재무장한 실제 InputSystem update 식별값과 실패/재무장 receipt를 기록하고, 그 뒤의 다른 입력 update와 더 늦은 권위 UI frame에서 Submit/Click의 모든 실제 결속 control을 released로, Navigate의 실제 결속 control을 0으로 관측한다. action의 순간 edge 부재나 IsPressed만으로 물리 release를 대신하지 않는다. 포인터 위치 자체는 0일 필요가 없다.

중립 대기 동안 UI 후보의 edge·NavigateChanged는 비우고 실제 cursor는 각 frame을 소비한다. 중립을 얻은 그 frame도 Empty 의미 frame으로 게시하여 edge를 버린다. release를 관측한 입력 update에 남은 edge도 버린다. 출정 또는 재확인 token은 중립 관측 receipt보다 tick/frameOrdinal이 모두 높은 현재 frame이고, 더 뒤의 입력 update에서 온 새 Submit/Click edge인 경우만 발급된다. 과거 tick/frameOrdinal/modeEpoch를 감소시키거나 가짜 영수증을 발행하지 않는다. 이 강화는 실제 CH1 Hub가 결속된 경로에만 적용하며 일반 UIOnly/게임플레이의 기존 입력 확정·콜백 소유는 유지한다.

**REQ-C1B-SRETRY-005 — 정확 새 의도와 두 번의 사용자 입력.** 새 의미 의도 ID는 `chapter1.retry-expedition-storage` 하나다. 기존 Submit/Click 액션과 같은 실제 출처에 결속된 token 생성·검증·한 번 소비 경로를 사용한다. 새 InputAction/callback을 만들지 않는다. 검증 seed는 null이며 실패한 출정 token/seed를 자동 재실행하지 않는다. 실패 전용 상태에서는 이 의도만 받아들이고 start/검증 seed/return intent를 거절한다. Hub/ReturnedToHub/진행/Diagnostic에서는 retry 의도를 거절한다. click+submit이 같은 frame이면 재확인 intent 1개이며 `_processingRequest`와 실제 token 소비가 파기·관측·저장보다 먼저다.

새 상태는 `ExpeditionStorageFailed`, `RetiringHubForStorageRetry`, `PreparingHubAfterStorageRetry` 세 개다. FixedUpdate는 이 세 상태에서 게임플레이/실패 중재를 수행하지 않는다. 첫 상태의 실제 새 retry token을 소비한 뒤 인증된 exactRoot와 공개 Observe의 관문을 한 번 재확인한다. 예외/읽기 불가면 추가 오류를 보존하고 Diagnostic으로 닫으며 동일 token·새 세대 자동 반복 0이다. 성공하면 기존 Hub.BeginRetirement를 한 번 호출한다. 실제 closure가 Destroyed와 actions Closed를 모두 확인하기 전 새 거점을 만들지 않는다.

closure 완료 후 기존 CreateFreshHub로 같은 prefab·같은 고정 root·증가한 새 hub generation을 한 번 만든다. old hub/router를 활성화하거나 두 actions 소유자를 겹치지 않는다. 새 adapter는 기존 생산 Observe/Select/preservation/recovery/save/prepared 수명을 그대로 수행한다. 새 준비가 실패·fault·PersistencePending이면 원래 오류와 새 준비 사실을 보존하고 Diagnostic으로 닫는다. 시간 경과만으로 prepared 성공을 만들지 않는다. 새 준비 receipt가 유효하고 PersistencePending=false인 뒤에도 실제 새 중립을 기다린다.

새 준비 성공은 출정 요청이 아니다. 출정 SaveForExpedition·graph·RunStarted 0이며 Phase=Hub로 돌아와 오류 이력을 유지한다. 사용자가 **다시** 입력을 놓고 더 늦은 새 출정 edge를 보내야 별도 새 token이 발급된다. 그 출정은 기존 프로필 생성자의 실제 재관측·계획 선택·공개 Save의 정확 결과/기대 revision·거점 종료·첫 graph 완료 view·RunStarted 순서를 따른다. 복구 중 자동 원정키·자동 seed 진행·ReturnedToHub 증가 0이다.

**REQ-C1B-SRETRY-006 — 표시와 요청의 일치.** 실패 전용 상태는 기존 출정 버튼 0의 동일 hit rect에 `저장 다시 확인`을 표시하고 나머지 버튼을 숨긴다. 새로운 여섯 번째/일곱 번째 저작 버튼이나 scene/prefab 수정은 없다. 물리 중립 전에는 회색/잠금 안내, 중립 뒤에만 재확인 요청을 표시한다. Presenter는 `_cursor`를 계속 전진시켜 잠금 중 요청을 큐에 쌓지 않는다. 파기/준비 중에는 버튼을 모두 잠그고 최초 저장 오류와 준비 상태를 표시한다. Diagnostic에는 재확인 버튼이 없다. 새 Hub 준비 뒤 출정/검증 버튼과 기본 focus 0을 복원하지만 최초 오류 기록을 지우지 않는다. 기존 640×360 변환·최소24×24·한 frame 한 intent를 유지한다.

## 5. 저장·세대·시작 순서의 불변 조건

**REQ-C1B-SRETRY-007 — 저장 권위 불변.** 지정 root는 `C:\Users\me\Documents\GPT-workspace\AcadeGameMaker\output\local-playable-slice-r1\profile\chapter1-development`뿐이다. 사용자 profile fallback·다른 root·링크·저장 파일 삭제·reset 호출·재시도용 원문 되돌림·Profile 내부 UnderHeldRoot/fault 호출·스키마 변경·보존 필드 삭제·`_ready` 복구는 0이다. 기존 공개 prepared 소유자의 규범 보존은 원문/전이 증거를 기록한다. 새로운 보존/복구 권위를 FlowOwner에 만들지 않는다. 공개 Observe가 reset barrier/잠금 오류를 거절하면 retry를 중단하며 reset 경로로 넘기지 않는다.

출정 실패는 이미 실제 temp를 남겼을 수 있다. 새 prepared lifecycle의 정상 stale-temp 보존은 삭제/reset과 구별한다. 준비가 실제 복구 저장을 수행하면 그 실제 source/final/committed revision을 그대로 기록하며 “재확인에는 디스크 쓰기 0”이라고 주장하지 않는다. 하지만 실패한 출정 SaveForExpedition을 자동 다시 호출하지 않는다.

자동 출정은 기존 `101→303→202→404→101` 제안 seed 순환을 따른다. 저장 확정 전 lastOfferedSeed 진행 0이며 명시 검증 seed 요청은 기존 lastOfferedSeed를 보존한다. 새 준비가 원래 데이터보다 다른 권위 revision/seed를 제공하면 실제 공개 복구 근거를 기록하고 이전 값을 임의 덮어쓰지 않는다. 예상하지 못한 prepared 문서/현재 프로필 불일치는 Diagnostic이다.

RunStartedRevision은 이번 출정의 정확 저장 확정·old hub 실제 종료·첫 graph 완료 view 이후 기존 위치에서 한 번만 증가한다. retry intent/새 prepared/새 neutral/graph 구성 요청만으로 증가시키지 않는다. hub generation만 새 거점마다 증가하며 gameplay generation은 별도 기존 lifetime이 실제 새 원정에서 1부터 발급한다. 실패복귀·누적 RunEndCommittedRevision/ReturnedToHubRevision·기존 이전 원정의 증거를 초기화하거나 새 복구 성공 이벤트로 이중 집계하지 않는다.

## 6. 검사와 수용 기준

**REQ-C1B-SRETRY-008 — 독립 검증과 정확 선택.** 기존 31 선택자를 삭제·이름 변경·Ignore하지 않는다. 기존 SaveFailureShowsErrorWithoutRunStarted는 실제 Outcome/FailureStage를 확인하고 키보드 실패→held→release→새 retry→파기/새 준비→held 차단→새 중립/새 출정→첫 완료 view/RunStarted의 연속 trace로 보완한다. 기존 재진입·오래된 token·다른 owner/router/endpoint·잘못된 phase 검사도 새 의도 거절을 덧붙이며 기존 assertion을 유지한다.

**기존 저장 실패 검사의 정확 기대 교체 — 사전 검수 P1-001 보정안.** 루나 사전 검수 `output/chapter1-playable-r1/evidence/b1-expedition-save-retry-prereview-luna6.v1.json` SHA256 `51C6E9C84A1E849767084F1AE4754CEB96B9457B3BDEDEE8D6E94A7455E635EC`의 지적에 따라 다음 절차를 기존 `SaveFailureShowsErrorWithoutRunStarted` 안에서 적용한다. 원래 메서드 이름·선택자·실제 primary Read/Read 잠금은 유지하며, 이 검사의 `LogAssert.Expect`가 일반 `원정 연결 중단: `을 기다리던 기대와 `WaitDiagnostic()` 호출만 아래 전용 실패 관측으로 교체한다. 복구 가능한 실패 처리의 원인 로그는 `출정 저장 실패: ` 접두사를 가진 오류 로그로 한 번 남기고 이 검사는 그 정확 접두사를 기대한다. `Stop`의 일반 진단 로그와 Diagnostic 대기는 복구 불가 음성 검사에 그대로 유지한다.

1. 실제 prepared 거점과 공개 프로필의 잠금 전 revision을 관측하고, 실제 InputTestFixture 키 이벤트로 출정한다. 실패 뒤의 키를 실제 누른 채 유지하도록 같은 파일의 입력 도우미를 한정 보완하며 자동 release가 held 검사 구간에 섞이지 않게 한다. FlowOwner를 직접 호출하여 실패를 만들지 않는다.
2. 같은 파일의 기존 입력 추적 도우미에 주입 기능 없는 `WaitExpeditionStorageFailed` 관측 대기를 두어 최대180 완료 고정 프레임 안에 `Flow.View.Phase == ExpeditionStorageFailed`를 확인한다. 그 전에 Diagnostic에 도달하면 이 검사는 즉시 실패한다. 오래 기다린 Diagnostic이나 화면 문구만 일치하는 결과를 성공으로 인정하지 않는다. 기존 `WaitDiagnostic` 도우미는 삭제하거나 의미를 바꾸지 않는다.
3. 실제 `Flow.LastSaveResult.HasValue == true`, 유효한 Outcome=FailedBeforeCommit, FailureStage=ExistingPrimaryValidation, CommittedRevision=-1, 실제 잠금 전 revision+1과 실패 기록의 기대 revision 일치를 확인한다. 이번 실제 시도의 RunStartedRevision=0, RunKey=null, 게임플레이 router 미발급도 함께 확인한다. 이 primary Read/Read 사례가 Unreadable이나 다른 OS 결과가 되면 목표 결과 미달로 실패하며 읽기 불가 검사의 성공으로 대체하지 않는다.
4. 실패 상태의 읽기 전용 `Flow.View.Error`가 정확 `저장에 실패하여 출정하지 못했습니다` 문구와 보존된 실제 원인을 포함하는지 확인한다. Presenter의 실패 표시 분기는 이 투영 문자열을 그대로 상태 문구에 사용하고, 그 거점의 첫 버튼에 `저장 다시 확인`을 표시한다. 현재 시험 조립체에는 HubPresentation/TMP 직접 참조가 없으므로 새 참조·friend·반사로 표시를 읽지 않는다. 이 장치 검사는 표시 투영과 생산 Presenter 분기의 정합성을 검증하고, 실제 화면에 렌더된 문구·버튼은 기존 독립 실제 창 검수에서 따로 확인한다. 투영 확인만으로 실제 화면 판독을 수용했다고 주장하지 않는다.
5. 잠금을 해제해도 held 입력만으로 저장/새 거점/RunStarted가 추가되지 않음을 확인한 뒤, 실제 release→새 중립→새 retry 누름→기존 파기/actions 종료→새 generation/prepared→새 거점의 held 차단→다시 release/새 중립→별도 새 출정 누름→실제 정확 commit/첫 완료 view/RunStarted1을 같은 trace로 확인한다. 오류 관측 이후에도 §4의 두 사용자 입력 관문과 원래 상한을 유지한다.

신규 `StorageReadUnavailableRequiresFreshHubAndNewStart`는 같은 전용 상태·오류 투영 대기를 사용하되, primary Read/None 잠금에서 공개 batch의 실제 Primary.Kind=Unreadable 및 `ReadUnavailableNoSaveIssued`, `Flow.LastSaveResult.HasValue == false`, 기대 revision/CommittedRevision 미발급을 확인한다. 이 관측을 FailedBeforeCommit으로 바꾸거나 0/-1의 가짜 저장 결과로 채우지 않는다. 해당 읽기 불가 시도도 RunStartedRevision=0/RunKey=null이며 Diagnostic에 도달하면 실패다. 불확실 저장·SavingReturn·기술 오류의 기존/신규 음성 검사는 반대로 Diagnostic을 기대하고 재확인 가능 상태로 바뀌면 실패한다. 이 기대 교체는 기존 31 검사 중 해당 메서드의 잘못된 상태/로그 가정만 보정하며, 저장·발급·차단을 검증하던 기존 인수 조건을 삭제하지 않는다.

| 신규 정확 검사 이름 | 실제 입력·실패 자료 | 수용 기준 |
|---|---|---|
| `StorageReadUnavailableRequiresFreshHubAndNewStart` | 준비 뒤 primary를 Read/None으로 잠그고 실제 장치 출정. 실제 공개 preflight batch의 Unreadable과 저장 결과 미발급을 확인 | 잠금 해제만으로 변화 0, 실제 중립/새 retry, 새 준비, 별도 새 출정 edge에서만 성공 |
| `HeldClickSaveFailureRequiresReleaseAndFreshStart` | primary Read/Read 잠금과 기존 포인터 hit rect에서 held Click, 실제 FailedBeforeCommit 여부 확인 | held Click/Navigate 차단, release frame edge 폐기, click+submit retry 1개, 새 Hub held 차단과 별도 출정 |
| `ExpeditionSaveUncertainRemainsDiagnostic` | 출정 단계에서 실제 previous Read/Read 잠금. 공개 File.Replace 경계의 실제 결과 확인 | 실제 CommitOutcomeUncertain·CommittedRevision=-1, Diagnostic·retry/새 graph/RunStarted 추가 0. 다른 OS 결과면 이 사례 성공 아님 |
| `RejectCopiedStaleOrConsumedStorageRetryToken` | 기존 실제 prepared Hub와 실제 장치 frame을 사용하는 동기 endpoint 경계 검사. 실제 발급 token을 기록 | 미발급 복사/다른 source·router·endpoint·generation·frame/중복 소비·중복 재무장·동기 재진입 모두 거절. 실패 token 재전송으로 저장/복구/RunStarted 추가 0 |

신규 검사 수는 정확 4개, 총 정적 선택 후보는 **35개**다. 이는 실제 Unity 발견/실행 수가 아니다. 기존 31과 추가 네 개의 실제 발견 ID를 실행 전 결속하고 35 누락·중복을 확인한다. 보안 음성 검사만 기존 내부 생성자/동기 endpoint를 활용하며 정상 사용자 UI/실제 저장 성공 AC를 FlowOwner 직접 호출이나 frame 직접 생성으로 대체하지 않는다. 새 ForTests 함수·모의 save/preparation·reflection·HP/phase/revision setter는 0이다. OS 파일 잠금 fixture가 목표 결과를 만들지 못하면 실패/미완료로 남긴다.

| AC ID | 독립 관측·판정 | REQ 결속 및 기존 AC |
|---|---|---|
| AC-C1B-SRETRY-001 | 실제 읽기 불가와 실제 FailedBeforeCommit을 구별하고 미발급 결과/기대 revision의 유무를 보존. 기존 Read/Read 검사는 전용 ExpeditionStorageFailed/오류 투영과 실제 ExistingPrimaryValidation·CommittedRevision=-1을, Read/None 검사는 Unreadable·저장 결과 미발급을 관측. 두 사례의 Diagnostic 도달은 실패, 이번 시도 RunStarted0/RunKey=null/graph0. 실제 화면 판독은 별도 독립 창 검수 | 001/002/007/008; AC-C1B-HUB-001, AC-CH1-007 |
| AC-C1B-SRETRY-002 | held Submit/Click/Navigate로 요청0, 더 뒤 실제 release update/frame에서 edge0, 이후 새 edge만 retry1. 동시 click+submit1 | 003/004/005/006; AC-C1B-HUB-001, AC-C1B-INPUT-001 |
| AC-C1B-SRETRY-003 | consumed 실제 객체의 재무장1, copied/stale/foreign/consumed/reentrant/중복 rearm 거절, 저장·세대·RunStarted 추가0 | 003/005; AC-C1B-HUB-001 |
| AC-C1B-SRETRY-004 | old Hub 파기/actions closure 뒤 new Hub/new router/new prepared; 동시 actions0. 새 준비만으로 저장 출정/원정키/RunStarted0 | 004/005/007; AC-C1B-HUB-001, AC-C1B-E2E-003 |
| AC-C1B-SRETRY-005 | 새 Hub 중립보다 늦은 별도 출정 edge→실제 공개 정확 commit/기대 revision→거점 종료→첫 완료 view→RunStarted 증가1. 지속 필드·자동/검증 seed 규칙 보존 | 002/005/007; AC-C1B-HUB-001, AC-C1B-E2E-003, AC-CH1-007 |
| AC-C1B-SRETRY-006 | CommitOutcomeUncertain·기대revision 불일치·저장 결과 미발급 기술오류·SavingReturn·이미 시작·catalog/권위 오류는 Diagnostic 유지. 이전 실제 카운터/원문 보존 | 001/002/005/007; AC-C1B-RETURN-001, AC-C1B-E2E-003 |
| AC-C1B-SRETRY-007 | 정확 여섯 파일 델타/31+4 발견/필수 회귀/소유 분리·입력 지문 결속. 실제 장치 시험을 인간 실제 창 플레이로 확대하지 않음 | 008; AC-CH1-006/007, AC-C1B-EVIDENCE-001 |

연속 trace는 실제 시도·요청 ordinal/hub generation·prepared/UI receipt·물리 update/neutral receipt·root 관측/저장 결과·기대/확정 revision·old closure·새 prepared source/final revision·새 token·첫 gameplay 완료 view·RunStartedRevision을 같은 소스 지문으로 결속한다. 원시 파일/오류/실패 결과를 보존하고 실패된 attempt의 token을 새 attempt의 것처럼 쓰지 않는다.

### 영향 검사·실행 상한

1. 이번 초안 작업의 컴파일/Unity/장치/저장/빌드/실제 플레이 실행은 모두 0이다. 현재 §24의 Run+시험 최소 컴파일 묶음에 이 범위를 추가하지 않는다.
2. Approved 후 별도 솔이 여섯 파일을 바꾸고 루나가 접근권·토큰·실제 중립·결과 분류·기존 입력 영향·정확35 선택을 정적으로 독립 대조한다. 그때 현재 실제 입력 델타에 맞는 최소 컴파일 계획/자료를 새 이름으로 결속한다. 이 문서는 기존 응답파일/성공5 또는6 재사용 원장에 새 제품 소스를 몰래 포함시키지 않는다.
3. 기존 미소비 집중 합산900초에 원래31+신규4를 모두 포함하는 **한 번의 35검사 후보**다. 신규4를 실행하고 같은 원문 31을 별도900초 반복하는 권한을 만들지 않는다. 첫 실패/상한은 원시 보존 후 중단한다. 35가900초 안에 불가능하면 정확 선택·각AC·상한을 유지하는 별도 아스트라 결정이 먼저이며 임의 실행 증액/분할 재실행0이다.
4. 실제 출정의 기본 이동 수용 선행을 유지한다. 시험을 위해 room.IsBasicMovementAccepted를 가짜 true/SHA로 채우지 않는다. 컴파일 성공만으로 출정·방·런타임 AC를 수용하지 않는다.
5. 기존 B0 정확25클래스와 B1 기존20파일의 중복1을 합친44 전체 클래스 회귀를 유지한다. EditMode/PlayMode 각1회·합산1800초, 종료/배수 각5초와 원래 발견 ID 기록을 따른다. InputRouter/Hub prepared/Profile/전이·전투·M5A/M5D2 회귀를 과거299로 대체하지 않는다.
6. 기존 snapshot4×3 수동12, B1 정상/세 실패 종단4·각30분/전체120분, 실제 창 키보드/포인터 각30분, 현소스빌드1회900초·Q0 인간 승인·기존 필수9회·원격 게시 관문은 유지한다. 재시도 장치 검사가 이것들을 대체하지 않는다. 이 계약의 여섯 파일 보정 성공만으로 B1 전체/CH1 완주를 판정하지 않는다.

## 7. 아스트라 판정과 인계

`output/chapter1-playable-r1/preparation/b1-runtime-gates-astra.v1.json`의 D3와 파일 후보 여섯 개는 일치한다. 다만 D3의 불확실 저장도 새 prepared로 복구할 수 있다는 제안은 채택하지 않는다. 실제 CommitOutcomeUncertain은 이 계약에서 Diagnostic 유지이며, 기존 읽기잠금 검사와 tmp 잠금의 원인 가정은 §1대로 실제 결과 중심으로 정정한다. 그 준비 원장과 주 CH1 계약·동결 증거는 이 문서 작업으로 수정하지 않는다.

아스트라의 승인 판정 항목은 ① 실제 batch Unreadable/유효 FailedBeforeCommit 두 분류만 복구 가능하고 기술·불확실 오류를 닫는지, ② 여섯 파일과 읽기 전용 추가 Observe가 충분한지, ③ 실제 소비 token/새 물리 중립/새 retry 의도/파기/새 prepared/별도 새 출정이라는 두 사용자 입력이 기존 §11.7과 양립하는지, ④ 기존31+정확 신규4 및900초 상한·필수44/종단을 유지하는지다. 미결 사실은 독립 사전 검토로 판정하며 이 초안을 스스로 Approved로 승격하지 않는다.

루나 사전 검토 후 아스트라 Approved → 별도 솔 구현 → 루나 델타/실제 결과 검수 → 아스트라 한정 통합 순서다. 한도·인증·모델 오류는 즉시 중단하고 반복/외부 모델/유료 경로를 사용하지 않는다. 실제 참여와 구현/독립 검수 분리는 새 단계 증거에 기록한다. 일일 오전8시 감사·모델 갱신 예약 삭제 정책을 바꾸지 않는다.

## 8. 아스트라 한정 승인 — Approved

2026-10-07 아스트라 `gpt-6-astra`는 이 문서 §1~7의 검수 원문 SHA `0E8F2ECAFB49090811627D1498F1A7B17573E437725730B752690CB58D4D2163`과 솔 준비 v2 SHA `92822F8666D0641F74640CDA8D54856309045F4EBA76661D1445B9042ABB1847`를 직접 대조했다. 루나 전체 사전검토 v1 SHA `51C6E9C84A1E849767084F1AE4754CEB96B9457B3BDEDEE8D6E94A7455E635EC`의 P0=0/P1=1과, 그 P1-001의 검사 상태·로그 기대 보정을 닫은 좁은 재검토 v2 SHA `F90D3DD29E62836AF659233E02E7C5EBF40FD8901888E59B444E28EFB07AFF8B`의 남은 P0/P1=0/0을 합하여 정확 범위를 Approved로 채택한다. v2 하나가 전체 설계를 새로 검수하거나 현재568을 재수용했다고 표현하지 않는다. 앞선 Review·후보·승인 전 표시는 작성 당시 이력이며 이 절이 아래 범위에서만 대체한다. 상태 및 이 승인 기록 외 검수된 규범 원문은 보존한다.

복구 가능 분류는 인증된 고정 개발 root의 검증된 실제 공개 observation batch가 가진 Unreadable 및 실제 공개 Save가 반환·검증한 FailedBeforeCommit 두 개뿐이다. 전자는 생성자·계획 선택·Save 이전이며 결과 미발급을 보존하고, 후자는 실제 Outcome/FailureStage·CommittedRevision=-1·계산된 기대 revision을 보존한다. Observe/Save의 결과 미발급 예외, 권위·root·catalog·생성자·prepared 불일치, 산술·graph 오류, CommitOutcomeUncertain, 확정 revision 불일치, SavingReturn 및 이번 시도 시작 이후 오류는 기존 Diagnostic으로 닫는다. 과거 누적 카운터를 초기화하지 않고 이번 시도 시작값 대비 미발급을 확인한다.

기존 실제 소비 token 객체에 한정한 단회 재무장→더 뒤의 실제 입력 update와 UI receipt에서 물리 release·중립 frame의 edge 폐기→더 뒤의 새 재확인 edge→기존 Hub 파기/actions closure→동일 고정 root의 새 generation/prepared→새 Hub의 새 중립→별도 새 출정 edge→실제 정확 저장·첫 gameplay 완료 view 뒤 RunStarted 순서를 승인한다. 새 의도는 `chapter1.retry-expedition-storage` 하나다. 실패 token·seed의 자동 재실행, 기존 profile 객체의 재활성화, 과거 중립 재사용, 두 actions의 중첩, 새 준비만으로 출정·ReturnedToHub 발급은0이다. 새 prepared 수명이 실제 수행한 보존·복구·저장은 그 원문과 revision을 보존하며 이를 디스크 쓰기0이라고 숨기지 않는다.

정확 여섯 파일은 §2의 Chapter1PlayableFlowOwner, Chapter1RunView, Chapter1HubLifetimeOwner, InputRouter, Chapter1ExpeditionPresenter, 기존 Chapter1ExpeditionPlayModeTests다. 추가 Observe는 FlowOwner의 공개 읽기이며 Profile·DevelopmentProfile·DesktopProfileLaunchAdapter·EncounterLifetimeOwner는 수정하지 않는다. 새 C#/meta/GUID/assembly/참조/friend/action/callback/ForTests/모의 저장/반사·스키마·일반 사용자 root·파일 삭제/reset 권한은0이다. 실제 CH1 Hub에 한정한 중립 보정이며 C1C Closing variant의 소유를 확대하지 않는다.

REQ-C1B-SRETRY-001~008과 AC-C1B-SRETRY-001~007을 채택하고 기존 REQ/AC-C1B-HUB-001, AC-C1B-INPUT-001/RETURN-001/E2E-003/EVIDENCE-001 및 AC-CH1-006/007을 보존한다. 기존 SaveFailureShowsErrorWithoutRunStarted의 이름·실제 primary Read/Read 잠금·기존 차단 인수 조건은 유지하되 잘못된 일반 Diagnostic 대기·로그 기대만 §6대로 전용 ExpeditionStorageFailed 관측으로 보정한다. Read/None의 실제 Unreadable 미발급 경계, Read/Read의 실제 FailedBeforeCommit/ExistingPrimaryValidation, 실제 불확실 결과의 Diagnostic은 서로 대체하지 않는다. OS 잠금이 목표 결과를 만들지 못하면 해당 검사는 실패/미완료다. 표시 투영 검증과 실제 창에서 렌더된 문구·버튼의 독립 판독도 구별한다.

기존31과 §6의 정확 신규4, 총35 선택자를 같은 미소비 집중1회·합산900초에 결속한다. 기존31 삭제·Ignore·신규4 별도900초·분할 재실행은0이다. 필수44 전체 클래스는 EditMode/PlayMode 각1회·합산1800초와 기존 종료/출력 회수 각5초를 유지한다. 실제 방 기본 이동 수용 선행, 별도 snapshot4×3 수동12, 정상/세 실패 종단4, 실제 창 키보드/포인터, 현소스빌드·9회·Q0 인간 승인·키·세대·원격 관문을 보존한다. 다른 실행 순서 Review의 승인 상태는 이 절이 변경하지 않는다.

구현 착수는 이번 승인만으로 즉시 공유 쓰기 권한을 넘기는 것이 아니다. Root의 G1/G2 r1 환경 초기화 실패에 대한 루나 원시 결과 검수, 현재568의 정적 재결속과 해당 project 실행 종료 확인, 정확 여섯 파일의 Root→솔 단일 소유 인계 뒤에만 제품을 쓴다. 첫 가져오기 실패는 코드 컴파일·executeMethod 진입 이전의 UPM 초기화/IPC 실패로 구분하고 이미 소비한 큰 실행을 되살리거나 이 계약으로 자동 반복하지 않는다. UPM 실행 맥락의 최소 진단·원인 보정과 제품 저장 재시도 구현은 별도 범위다. 이후 솔 구현→루나 델타·새 실제 입력 결속→기존 상한의 검사/실제 플레이→아스트라 한정 통합을 따른다. 이번 승인 작성의 제품·스테이징·Unity·컴파일·시험·프로필·원격 실행은0이며 구현·B1 전체·CH1 완주 수용은 아니다.
