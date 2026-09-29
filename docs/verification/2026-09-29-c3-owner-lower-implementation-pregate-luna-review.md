# C3 lower 구현 사전 실패 모드 검수

- 검수자: Luna
- 검수일: 2026-09-29
- 기준: Approved C3, ADR-0036, 승인 설계 SHA `038E568911410202273CE867D85C594B4929248DE025F326F03444D5DA0ABB30`
- 현재 범위: `ProfileNewGameConfirmationV1.cs`와 전용 lower 시험·메타만

## 현재 lower 경계

현재 `Capture`는 실제 adapter root 조회와 실제 세 leaf capture/classification을 수행하고, private `ConditionalWeakTable` witness로 결과 발급을 인증한다. 그러나 현재 파일에는 Q-B request/issuance, displayed identity 비교, `ConfirmedProfileResetRequestV1`, owner commit 또는 one-shot execution publication이 없다. 그러므로 다음 상위 경계를 lower 구현에 미리 넣거나 시험으로 양성 발급해서는 안 된다.

## 필수 실패 행

1. **발급 cohort 위조:** foreign adapter, router, root, owner, epoch을 같은 값·새 객체·반사 필드로 바꾼다. exact authored adapter/router/root witness와 opaque owner/epoch 참조가 모두 일치하지 않으면 capture·identity 반환·확인 요청 발급이 무변이로 거부되어야 한다.
2. **capture 결과 위조:** 직접 생성·복제·기본값·reflection 변경한 결과, outcome/classification/root/capture/identity/leaf projection 변경, 실패 결과에 남은 root·capture 증거를 각각 getter 전에 거부한다. `IdentityFor`는 정확한 owner/epoch와 `Captured` 결과에서만 허용한다.
3. **fresh identity 누락:** 표시 후 Primary·Previous·Temp의 추가·삭제·바이트·분류·revision·제품 필드가 하나라도 바뀐 행, exact-default 전환, ambiguous leaf 행을 모두 stale로 거부하고 기존 identity를 C1 또는 confirmed request에 전달하지 않아야 한다. displayed capture를 제자리 교체하거나 revision/hash만 비교하면 안 된다.
4. **Q-B provenance 우회:** 동일 Item/Receipt 값의 복사 request, 실제 take 없는 caller boolean, scalar owner/epoch/generation, 새 issuance record, foreign presenter/Q-B owner를 거부한다. 실제 Q-B one-shot issuance를 claim한 exact request만 capture intake로 들어와야 한다.
5. **confirmed request 이중 발급:** `ConfirmedReady`가 아닌 상태, confirm/cancel retry 중, 다른 interaction·decision generation, 다른 adapter/router/root, 다른 owner/epoch에서 `ConfirmedProfileResetRequestV1`을 발급하지 않는다. 발급 witness는 exact capture와 모든 참조를 묶고 one-shot CAS를 가져야 하며, 두 번째 commit·복제·reflection 생성은 거부해야 한다.
6. **commit 원자성:** `CommitForExecution`은 Q-B issuance·decision capability·capture retry·rearm을 먼저 무효화한 뒤에만 opaque request를 한 번 게시해야 한다. 중간 예외·teardown·disable/destroy·동시/reentrant 호출에서 request가 부분 게시되거나 옛 confirm/cancel/rearm이 되살아나면 안 된다.
7. **Cancel/rearm 경계:** Cancel은 profile bytes, action/router, receipt, map/scene 상태를 바꾸지 않고 정확한 pending capability 한 번만 소비해야 한다. foreign/duplicate/late Cancel, partial successor, fresh cursor의 첫 연속 frame 미폐기, epoch 감소·재사용은 terminal close와 무변이로 끝나야 한다.
8. **Hub 누출 및 C4 조기 연결:** Hub가 Profile identity, root, document, C1 result 또는 scalar outcome을 꺼낼 getter·변환을 추가하지 않는다. 현재 lower 시험에는 C1 `Begin`, C2, typed executor report issuer/intake, `ReportExecution` 양성 경로가 없어야 하며, C4 Review 경계를 넘는 시험은 금지한다.

## 판정

위 행은 현재 lower 파일의 결함 수용 판정이 아니라 Approved C3 owner 구현의 필수 pre-gate다. C3L 관찰 단위의 partial evidence는 이를 대신하지 않는다. 구현 후에도 C3 `AC-M5D7QC3-007/008`은 실행 연결 전 `Open / Not Verified`이고, 이 검수는 C3 전체·`AC-M5D7QC3-010`·C4 수용을 주장하지 않는다. 코드·시험·현재 계약은 수정하지 않았다.
