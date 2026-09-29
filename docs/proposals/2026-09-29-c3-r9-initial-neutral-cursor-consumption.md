# C3 R9 최초 중립 발행의 정상 커서 소비 제안

2026-09-29. 실제 검토자 `gpt-6-sol`. REQ-M5D7QC3-001/005/007, AC-M5D7QC3-001/006/009/010 및 선행 REQ-M5D7PA-006의 실제 커서 계약을 추적한다. 읽기 검토와 제안 작성만 수행했다. 코드·runtime·설정·조립·원시 증거 수정이나 시험 실행은 없다. 아스트라 승인 전 구현하지 않는다.

## 실제 결과와 정적 후보의 구분

R9 XML `artifacts/c3-r9-play-probes-r1.xml` SHA-256 `B70589124F3889F6647375061DF5E948EFD53E52E3C5FBBB67B8634F1AD45763`은 두 실패를 기록한다. AC001은 7.644901초, AC006은 7.454794초이며 최초 Down의 실제 NavigateChanged/음수 Y와 Enter의 Submit=true를 확인한 뒤 `SelectInitialNewGame:437`에서 Q-B RequestReady 대신 Closed를 관측한다. 요청 Item 확인·실제 take·Owner intake와 이후 취소/새 커서 경계에는 도달하지 않았다. 실제 편집기 종료 2·QA 반환 5·입력 884개 전후 차이 0·이름/중복 차이 0인 실패를 보존한다. Navigation이 발생했다는 사실만으로 NewGame 선택이 완료됐다고 주장하지 않는다.

현재 시험 소스 `6A3FB8BE37AE8644E446AA04FAE63D4BF75469C4DA704D253D146E715D086FEF`의 427행은 최초 중립 `Publish()`를 추가했으나 즉시 PresenterFixed로 소비하지 않는다. 이후 431행 Down을 다시 발행하고 434행에서야 PresenterFixed를 수행한다. 이전 중립 발행 제안에는 이 정상 소비가 빠졌다. 이번 제안은 그 발행/소비 불균형만 바로잡는 제한 후속안이다.

`UiSemanticFrameV1.cs:143–178`의 정상 cursor.TryAdvance는 164–166행에서 baseline의 tick와 frame ordinal이 **각각 정확히 +1**인 다음 영수증만 허용한다. 최초 Create 단계에서 정상 커서 baseline이 마련된 뒤 중립 발행을 소비하지 않고 Down 발행까지 진행하면 이 연속성 검사에 걸릴 정적 경로가 있다. 실제 R9의 cursor baseline/ordinal은 기록되지 않았으므로 이 예외가 발생한 것으로 확정하지 않는다.

## Closed가 가능한 실제 호출 경로

Presenter `FixedUpdate:132–147`은 `_cursor.TryAdvance` 140행 또는 해석/Render의 예외를 잡아 `Fail`로 전환한다. `Fail:374–378`은 Presenter를 Failed로 만들고 선택/버튼 표시를 닫는다. Q-B `LateUpdate:127–140`은 131행에서 Presenter Failed/Closed를 보면 `Close()`하고 반환한다. Q-B `Close:299–303`은 live request를 비우고 Closed로 바꾼다. 따라서 catch 내부 예외가 외부 로그에 남지 않아도 현재 Closed 관측과 일치할 수 있다. 로그 예외 없음은 정상 처리 완료의 증거가 아니다.

다른 폐쇄 경계도 존재한다. Presenter OnDisable/OnDestroy 380–381행은 CloseSuccessorFailure 354–358행으로 양측·Owner를 닫는다. Q-B OnDisable/OnDestroy 297–298행도 Close 및 Owner.CloseFromPeer를 수행한다. Owner.Terminal 370–378행은 peer 폐쇄, OnDisable/OnDestroy 388–389행은 자기 Closed까지 수행한다. 현재 실행은 어떤 수명주기 경계를 지났는지 관측하지 않았으므로 이를 원인으로 배제하거나 주장하지 않는다. Q-B의 일반 ValidateState 오류 catch는 `Fail`(305행)로 Failed가 되므로 단독 일반 검증 실패와 현재 Closed를 혼동하지 않는다.

NewGame과 Continue의 Q-A 활성화는 같은 Presenter `Activate:169–176`→controller `TryActivate:217–229`→`TryTakeIntent:234–252`→보존된 intent 전달을 거친다. controller의 알려진 메뉴/현재 Ready/상호작용 가능 여부를 검사하고 실제 Item을 보존한다. 별도 NewGame 게임 실행 효과는 이 경로에 없다. Presenter.TryTakeRetainedIntent 69–89행은 불변 launch 영수증으로 실제 요청을 만들며, Q-B LateUpdate 133–138행이 이를 실제 RequestReady로 발행한다.

Render의 버튼은 `HubMenuButtonViewV1.Render:17–24`에서 텍스트·interactable·Image enabled만 변경한다. 제품에 추가 입력 delegate나 게임 실행 호출은 없다. Presenter.Render는 notification 표시를 변경할 수 있으며 정상 Unity 표시 콜백과 수명주기 가능성을 근거 없이 없다고 단정하지 않는다. 현재 fixture 330–331행의 notice 대상은 실제 `SafeFrame/Notification` 객체로 결속되어 있다. 이번 후보를 위해 prefab/렌더 callback 구조를 바꾸지 않는다.

## 최소 정상 소비 보정 후보

기존 `SelectInitialNewGame` 첫 순서를 **`Publish(); PresenterFixed();`**로 정한다. 신규 발행·cursor reset·epoch 대입 없이, 이미 추가된 중립 발행을 정상 현재 cursor가 즉시 한 번 소비하게 하는 보정이다. 이후 기존 Down Publish→PresenterFixed→release Publish→PresenterFixed→Enter Publish→PresenterFixed→Late 순서를 유지한다. 회복 경로에도 최초 중립 발행의 정상 소비 원칙은 동일하되 회복 시험 통과를 미리 주장하지 않는다.

이것은 initial epoch의 격리 해제 발행이며 successor 생성 이후가 아니다. Cancel/Rearm 이후의 첫 actual Publish는 여전히 실제 Enter Submit이고 그 true 프레임을 폐기한 뒤에만 release와 다음 Enter로 새 actual take가 성공해야 한다. 추가 빈 successor 프레임이나 가짜 입력을 넣지 않는다. 초기 RequestReady와 NewGame 실제 Item 확인, 이후 실제 opaque take/Accept 검증도 약화하지 않는다.

필요한 관측은 광범위 action/영수증 getter 대신 같은 단일/두 probe의 정상 호출 사이에서 첫 중립 소비 직후와 Down 소비 직후의 Presenter `_state`, 원래 `_confirmationInteractionClosed`, Q-B `_state`만 정확 `SnapshotField`로 읽는 제한 후보이다. 정확 선언/필드 타입은 `HubMenuPresenterV1`의 `HubPresenterStateV1`·bool, `HubMenuIntentHandoffOwnerV1`의 `HubMenuIntentHandoffOwnerStateV1`이다. 바인딩은 receiver exact type·DeclaredOnly·field type·비정적 여부를 확인하고, 진단 오류는 원래 assertion을 가리지 않게 분리한다. 제품 메서드를 진단 목적으로 다시 호출하거나 caught exception을 복원하려는 반사 실행은 금지한다. 원래 실패가 계속되면 소비 경계별 실제 상태를 근거로 다음 제한 진단을 승인받는다.

승인 후보는 기존 Play Owner 시험 helper의 정상 PresenterFixed 한 호출과 필요 시 위 최소 읽기 기록뿐이다. 원래 240 Edit/15 Play 이름, 377행/91개 행렬, 180초 제한, runtime·InputSettings·ProjectSettings·asmdef·friend·API·자산은 불변이다. 실제 실행·독립 검수·수용은 후속 단계이며 이번 구조 검토는 성공을 예고하지 않는다.

## 읽은 지문

- cursor 계약 구현 `Assets/AcadeGameMaker/Runtime/Input/Unity/UiSemanticFrameV1.cs`: `CCD7763A1103BA096DD64E02C5B91A99A3ADA21D49506CC81E5F0601E06D316F`.
- Presenter: `393B325BA5B8F6D79CA983F1A91098215E7A6993DE37E9F5D98BA775760E4900`.
- Q-B: `F47C61A043DAF4695CC53E41C6BE9BB67CA147C722F7B830459AA6304B45B3B5`.

현재 시험과 이전 모든 원시 실행 증거를 보존한다. 원인 확정·제품 결함 판정·시험 통과·C3 전체 수용을 주장하지 않는다.
