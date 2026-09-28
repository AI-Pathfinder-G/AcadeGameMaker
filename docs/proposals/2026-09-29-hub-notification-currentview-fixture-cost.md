# Hub 알림 CurrentView 반복 검증 비용의 조건부 QA 개선안

- 날짜: 2026-09-29
- 상태: 제안 — 현재 시험 변경 권한이 아님
- 대상: `HubMenuPresentationControllerV1Tests.AssertNotification`의 QA fixture만

## 관찰과 한계

R4에서
`AC004_RecoveryCompletedNotificationUsesPreviousReceipt`는 기본 180초를 넘어
187.413초에 종료됐고, 같은 행은 이전 R35에서 175.735초에 통과했다. 이 두 결과는
경계에 가까운 비용을 보여 주지만 단독으로 실패 원인이나 회귀를 확정하지 않는다.
R6 결과가 반복 실패를 보이면 아래 변경을 승인 검토할 수 있다.

현재 helper는 controller 상태 변경 없이 `CurrentView`를 세 번 연속 호출하여
`NotificationAnchor`, `NotificationKind`, `NotificationState`를 각각 단언한다.
각 `CurrentView`는 `ValidateOperational`과 `ValidateState`를 다시 실행하므로 동일한
handoff/proof, notification/proof, view/proof, intent 상태 검증이 세 번 반복된다.
복구 완료 receipt는 중첩 receipt·notification 상관 검증 비용도 함께 갖는다.

## 허용 가능한 최소 QA 변경

승인될 경우 helper 안에서만 다음 형태로 한 번 snapshot을 얻는다.

```csharp
var view = controller.CurrentView;
Assert.That(view.NotificationAnchor, Is.EqualTo(HubNotificationAnchorV1.TopRight));
Assert.That(view.NotificationKind, Is.EqualTo(expectedKind));
Assert.That(view.NotificationState,
    Is.EqualTo(expectedPresent ? HubNoticeStateV1.Visible : HubNoticeStateV1.Absent));
```

이 변경은 원래 세 assertion의 값과 순서를 유지한다. 세 assertion 사이에는
controller mutation, callback, thread handoff 또는 clock/filesystem 관찰이 없다.
따라서 첫 `CurrentView`가 실제 controller의 handoff·notification·view·state proof를
검증하고, 반환된 immutable `HubMenuViewV1`의 각 getter가 자기 backing/proof와 closed
상태를 다시 검증한다. 다음 경계도 그대로 남는다.

1. 실제 fixture가 launch receipt와 notification을 발급한다.
2. `ProfileLaunchNotificationV1.FromReceipt`가 receipt를 검증하고 종류를 도출한다.
3. `HubMenuPresentationControllerV1.Create`가 handoff, 기대 notification 및 exact
   correlation을 검증한다.
4. 단 한 번의 `CurrentView`가 현재 controller 전체 운영 상태를 검증한다.
5. snapshot의 세 getter와 기존 notification kind 및 `SameReceipt` assertion이 원래
   표시 값과 caller authority 상관을 검증한다.

제거되는 것은 상태 변화가 없는 동일 시점의 controller 전체 재검증 두 번뿐이다.
생산 runtime 캐싱, 검증 생략, receipt scalar 투영 추가, caller authority 완화,
timeout 증가는 제안하지 않는다.

## 승인과 재검증 조건

R6 반복 실패 시에도 이 문서는 변경 권한이 아니다. Luna가 원래 assertion과 위
다섯 경계가 보존되는지 독립 검수하고 Astra가 정확한 test-only 변경을 승인한 뒤에만
Terra가 fixture 한 곳을 수정할 수 있다. 수정 후에는 최소한 단일 실패 행, helper를
공유하는 AC004 알림 행, 전체 `HubMenuPresentationControllerV1Tests`, 당시 승인된
전체 EditMode 및 worker 회귀 범위를 현재 소스로 다시 실행해야 한다. 실패·건너뜀·
판정보류 0과 Luna P0=0/P1=0 전에는 기존 실패나 C3L 수용 증거를 대체하지 않는다.

R6가 통과하더라도 이 제안만으로 원인이 확정되거나 변경이 필요해지지 않는다.
