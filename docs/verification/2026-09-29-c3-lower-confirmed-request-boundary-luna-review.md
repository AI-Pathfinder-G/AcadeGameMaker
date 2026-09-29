# C3 lower confirmed request 경계 설계 독립 검수

- 검수자: Luna
- 검수일: 2026-09-29
- 대상 설계 SHA-256: `EAA6CBE69CACC2BCA91515296768FC6E0C76F3C2BDCF604AF661D77A70A3218D`
- 대조 기준: Approved C3, 승인 설계 `038E568911410202273CE867D85C594B4929248DE025F326F03444D5DA0ABB30`

## 판정

설계상 P0=0, P1=0이다. 이 counter-design은 Q-B issuance·presenter topology·실제 Confirm CAS를 lower 단위의 증거로 가장하지 않고 상위 owner의 reciprocal commit 증거로 분리한다. 따라서 lower `Committed` 상태 하나를 상위 권한 폐쇄의 단독 증거로 사용하지 않는다는 점이 Approved C3 및 ADR-0036과 일치한다.

actual capture registry는 adapter/router/root와 opaque owner/epoch/generation을 묶고, `Captured` 결과의 private mint CAS만 허용한다. 두 실제 capture의 세 leaf 전체 identity 비교를 거쳐야 하며, revision/hash 단독 비교·decoded 문서 동등성·caller boolean은 배제한다. meaningful/ambiguous 변경은 request 없이 `FreshDecisionRequired`로 남고, exact-default 변화만 fresh capture에서 별도 발급한다.

반환값은 closed disposition과 선택적 opaque request뿐이며 identity/root/document getter가 없다. `IdentityForExecution`과 `ExecutorConsumed`는 C4가 `Approved`가 된 뒤에만 추가하도록 명시되어 C4 `Review`와 Profile identity 비노출 경계를 보존한다. Hub가 C1 결과·scalar outcome·root를 재구성할 seam도 없다.

## 구현 전제 및 음성 시험

`ownerToken`·`epochToken`·generation은 단순 새 `object`나 caller scalar로 만들 수 없는 owner-authenticated witness여야 한다. lower API를 직접 호출해 fabricated token으로 request를 양성 발급하는 행은 반드시 거부해야 하며, 이 전제가 지켜지지 않으면 P1로 재평가한다. 또한 foreign pair/root, copied capture, duplicate mint/commit, reservation 중간 fault, old capability 재사용, reflection 변조의 fail-closed 행을 포함해야 한다.

이 설계는 lower 단위의 bounded counter-design일 뿐 C3 owner/Q-B/presenter/rearm 구현, C3 전체 `AC-M5D7QC3-007/008`, `AC-M5D7QC3-010` 또는 C4 수용 근거가 아니다. C3L Verified predecessor와 기존 source/test pin은 덮어쓰지 않으며, 후속 구현 후 현재 소스의 집중·필수 회귀와 독립 검수가 필요하다.

코드·시험·현재 계약은 수정하지 않았다.
