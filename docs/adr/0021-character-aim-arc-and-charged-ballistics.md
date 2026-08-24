# 캐릭터 중심 AimArc와 차지 탄도를 사용한다

- Status: accepted
- Date: 2026-08-24
- Decision owner: Sol
- Approved by: user

기존 pointer reticle을 유지하면서 캐릭터 주변에 조준 방향 쪽 180도 반원과 방향 화살표를 표시하는 `AimArc`를 추가한다. 차지 기술은 화살표 내부가 꼬리에서 촉까지 차오르는 비색상 신호를 사용한다. 정확한 반지름·두께·화살표 geometry·표시 생명주기는 별도 `OD-AIMARC-001`에서 고정한다.

후속 활 무기는 차지 단계에 따라 초기 속도·도달 거리·피해·경직이 증가하고 동일한 중력 가속도를 받는 실제 곡사 탄도를 사용한다. 탄도 preview를 제공한다면 실제 발사 계산과 같은 함수를 사용해야 하며 장식용 가짜 궤적은 허용하지 않는다.

AimArc는 수직 데모 UI 후보 범위에 포함하지만 활 무기와 완성형 탄도 시스템은 후속 챕터 범위다. 이 결정은 해결된 pointer reticle 계약을 대체하거나 `OD-ART-001`을 다시 열지 않는다.

## Consequences

- 화면 끝에서도 캐릭터 기준 조준 방향과 차지량을 판독할 수 있다.
- 색맹 접근성을 위해 차지량은 shape fill로 전달한다.
- 수직 데모 Approved 전환 전 `OD-AIMARC-001`을 해결해야 한다.

## References

[ADR-0019](./0019-pointer-aimed-sidescroller-controls.md), [VD-07](../specs/vertical-demo/07-input-ui-and-feedback.md), [VD-08](../specs/vertical-demo/08-art-and-asset-integration.md)
