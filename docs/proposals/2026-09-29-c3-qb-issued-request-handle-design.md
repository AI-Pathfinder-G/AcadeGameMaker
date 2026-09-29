# C3 Q-B 실제 요청 발급 handle 설계

- 날짜: 2026-09-29
- 상태: bounded proposal — 구현·수용 권한 아님
- 기준: Approved C3와 승인된 owner 설계 SHA-256 `038E5689...ABB30`

## 결론

`HubMenuIntentRequestV1`은 readonly value row이므로 Item/Receipt 동등성으로 실제
Q-B out 값과 재구성한 값을 구별할 수 없다. 실제 owner를 함께 전달해도
`ClaimNewGameIssuance(owner, presenter, router, copiedRow)`가 pending record를 값으로
찾는다면 위조 row가 원래 권한을 먼저 소비할 수 있다. request 값은 데이터로만
사용하고, 실제 take가 만든 별도 one-shot 참조 handle만 C3 intake 권한으로 삼아야
한다.

request 내부 private stamp 방식은 권고하지 않는다. 값 대입 복사에 stamp 참조도
복사되고 기존 struct layout·동등성·constructor 의미를 바꾼다. scalar epoch,
receipt hash, bool 또는 enum도 권한 증거가 아니다.

## 권장 interface와 순서

기존 `TryTakeRequest(out HubMenuIntentRequestV1)`의 본문, 최초 `_request`,
`_takenRequest`, proof, `RequestTaken` 상태와 기존 시험 의미는 정확히 보존한다. 이
legacy overload는 C3 issuance를 만들지 않는다. 별도 C3 overload는 최초 세대에서만
이를 한 번 호출한 직후 같은 동기 호출 안에서 handle을 발급·owner-bind한다.
successor는 아래의 전용 현재 슬롯 take를 사용한다.

```csharp
internal bool TryTakeNewGameForConfirmation(
    NewGameConfirmationOwnerV1 owner,
    HubMenuPresenterV1 presenter,
    InputRouter router,
    out IssuedNewGameRequestV1 issued);

internal HubMenuIntentRequestV1 ConsumeNewGameIssuance(
    IssuedNewGameRequestV1 issued,
    NewGameConfirmationOwnerV1 owner,
    HubMenuPresenterV1 presenter,
    InputRouter router);
```

`IssuedNewGameRequestV1`은 getter와 public constructor가 없는 sealed 참조형이다.
별도 최상위 형식의 private constructor를 Q-B가 직접 호출하는 구현은 C#에서
컴파일되지 않으므로 사용하지 않는다. 이 제안의 컴파일 가능한 생성 경계는 다음과
같은 internal constructor이며, 생성 자체에는 발급 권한이 없다.

```csharp
internal sealed class IssuedNewGameRequestV1
{
    internal IssuedNewGameRequestV1() { }
}
```

동일 assembly의 다른 호출자가 만든 객체는 **미등록 후보**다. Q-B의 private
registry에 witness가 등록되고 exact epoch record 및 owner pending 참조와 함께
결속된 객체만 실제 발급 handle이다. registry 또는 등록·바인딩 factory를 caller에게
노출하지 않는다. `ConsumeNewGameIssuance`는 미등록 후보를 값 조회·pending 검색·
자동 등록 없이 거부하고, 등록된 실제 handle 및 양쪽 pending/역사를 변경하지
않는다. 같은 형식·생성자 사용 여부만으로 actual issuance를 인증하지 않는다.
Q-B의 `ConditionalWeakTable` witness는 exact Q-B owner, confirmation owner,
presenter, router, epoch token, checked epoch number, 실제 taken request 값과 해당
epoch record 참조를 readonly로 묶는다. `int Consumed`만 CAS를 위한 private 가변
필드이며 0에서 1로 한 번 전진한다. readonly 필드를 ref CAS 대상으로 삼지 않는다.

첫 세대의 발급 순서는 다음과 같다. 1~3은 모두 legacy take **전**의 읽기 전용
선검증이다.

1. exact authored confirmation owner/presenter/router와 현재 Q-B owner, 현재 epoch
   number/token을 참조 동일성으로 검증한다. 최초 epoch의 confirmation owner는
   정확히 `AwaitingRequest`이고 private pending-issued handle/issuance/decision과
   과거 epoch history가 모두 없는 **first no-history pristine**이어야 한다.
   Cancel/rearm 뒤 successor epoch에는 이 조건을 재적용하지 않는다. successor는
   checked 증가한 epoch와 fresh 참조 token 및 exact rearm capability의 실제 발급
   출처를 요구하고, **현재 epoch의 owner pending/issuance/decision slot만**
   pristine이어야 한다. 이전 append-only records, 최초 `_takenRequest`와 Q-A retained
   intent history는 존재해야 하며 그대로 보존한다.
2. 최초 세대에서는 Q-B가 `RequestReady`이며 private live `_request/_requestProof`가
   정확히 하나 있고 `_takenRequest/_takenRequestProof`가 아직 없음을 검증한다.
   successor에서는 최초 `RequestTaken` 상태와 `_takenRequest/_takenRequestProof`를
   그대로 보존한 채 **현재 `SuccessorRequestSlot`만** `RequestReady`이고 live
   request/proof가 하나, 현재 taken record와 pending handle이 없음으로 검증한다.
   두 경우 모두 선택한 현재 live row 자체의 closed proof를 검증한다. 최초 필드의
   무-history 조건을 successor 슬롯 검사에 재사용하지 않는다.
3. 그 **private live row**의 Item이 `NewGame`이고 Receipt가 presenter의 immutable
   handoff receipt와 정확히 일치하는지 검증한다. 값이 정상인 non-NewGame, foreign
   owner/presenter/router/epoch, receipt mismatch, non-pristine owner는 false/clean
   rejection으로 끝내며 request, Q-B/owner 양쪽 pending 슬롯과 모든 legacy/history
   필드를 바꾸지 않는다. lower capture/root/lock 경계에도 들어가지 않는다.
   reflected state/proof corruption은 clean rejection이 아니라 기존 fail-closed
   terminal containment 예외를 따른다.
4. 위 선검증이 전부 끝난 뒤에만 최초 세대는 기존 legacy `TryTakeRequest(out row)`를
   그대로 호출해 실제 take와 `_takenRequest`/proof/`RequestTaken` 역사를 완성한다.
   successor는 현재 `SuccessorRequestSlot`의 실제 take를 한 번 수행하며 최초
   legacy take를 재호출하거나 최초 필드를 덮어쓰지 않는다. 현재 slot의 taken
   request/proof/phase만 영구 소비 이력으로 전진시킨다.
5. fresh sealed handle과 witness를 만들고 append-only epoch record에 연결한 뒤,
   exact owner의 private pending-issued 슬롯에 같은 handle 참조를 즉시 바인딩한다.
6. owner와 Q-B 양쪽의 handle/proof 참조가 일치한 뒤에만 out handle을 게시한다.
   4 이후 actual take, handle mint, registry 등록, record 연결 또는 양쪽 bind 중 예외는
   이미 완성된 legacy 역사를 그대로 보존하고 양쪽을 terminal close한다. rollback,
   live request 복구, handle 재발급 또는 이전 epoch 재활성화는 없다.

`ConsumeNewGameIssuance`는 handle registry와 양쪽 pending 참조를 먼저 확인하고
`Consumed 0 -> 1` CAS가 이긴 호출 하나에만 witness가 보관한 request 값을
돌려준다. 검색 키는 handle 참조이며 Item/Receipt/epoch scalar로 pending issuance를
찾는 fallback은 두지 않는다. C3 내부 `AcceptNewGame`은 실제
`IssuedNewGameRequestV1` handle을 필수 입력으로 받아야 한다. request 값만 받는
entry, owner의 pending handle을 값으로 자동 검색하는 entry, copied row와 actual
owner로 pending handle을 선택하는 entry는 두지 않는다. request row는 handle을
성공적으로 소비한 뒤 witness에서 얻는 데이터일 뿐 intake 권한이 아니다. 이
interface 보정은 Astra의 bounded 승인 후에만 구현한다.

## 최초 역사와 successor

최초 `_takenRequest`, `_takenRequestProof`, `RequestTaken`은 영구 불변 이력이다.
이름을 바꾸거나 초기화·덮어쓰기·AwaitingIntent 복귀를 하지 않는다. C3 전용으로만
`TakenEpochRecord` append-only chain과 현재 `SuccessorRequestSlot`을 추가한다.

- 첫 record는 기존 `_takenRequest`와 실제 첫 handle witness를 참조한다.
- rearm은 `checked(epoch + 1)`과 fresh epoch token, fresh successor slot을 만든다.
- successor take는 그 slot의 실제 request에서 fresh handle/record를 만들고 최초
  필드를 건드리지 않는다.
- 종료·부분 실패 record도 보존하며 이전 handle/request를 다시 활성화하지 않는다.

실행 보고 mint, C1/C4 scalar, receipt hash나 명확하지 않은 proof를 Q-B에 추가하지
않는다.

## 최소 양성·음성 구분

양성 시험은 실제 Q-A `NewGame` retain과 Q-B `LateUpdate`를 거쳐 `RequestReady`를
만들고 C3 overload로 실제 take/handle을 얻는다. exact owner/presenter/router로 한
번 consume하여 NewGame/receipt를 확인하고, legacy state가 `RequestTaken`, 최초
taken 값이 그대로이며 record가 하나 추가됐는지 확인한다. 두 번째 consume은
상태 비변경 거부여야 한다.

음성 copy 행은 양성 actual row의 Item/Receipt로
`new HubMenuIntentRequestV1(...)`을 만들고 actual owner/presenter/router를 함께
제공한다. handle 없는 copied row intake는 capture/root 조회 전에 거부되어야 하며
pending actual handle의 `Consumed`는 0으로 남아야 한다. 그 직후 원래 handle이
정상적으로 한 번 성공해야 위조 행이 원래 권한을 소비하지 않았음이 증명된다.
internal constructor로 생성했으나 registry에 등록되지 않은 후보도 실제 handle
consume 전에 거부하고 pending/Consumed/history의 비변경 뒤 원래 handle의 1회
성공을 확인한다. 같은 값의 foreign handle, 다른 owner/router/epoch, legacy take만 수행한 row,
duplicate handle도 각각 fail-closed로 확인한다. 별도 선검증 행은 clean
non-NewGame, foreign owner/presenter/router/epoch, non-pristine owner와 handoff
receipt mismatch가 실제 take 전 모든 request/pending/legacy/history snapshot을
그대로 유지하고 lower capture 호출 수가 0임을 확인한다. reflection-corrupt state는
동일한 clean rejection을 기대하지 않고 기존 terminal containment를 확인한다.
