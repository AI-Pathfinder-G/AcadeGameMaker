# C3 필수 decision 시험 matrix 독립 설계 검수

- 검수자·실제 모델: 루나, `gpt-6-luna`
- 검수 대상: `docs/proposals/2026-09-29-c3-required-decision-test-matrix.md`
- 대상 SHA-256: `B213890A1204B72BDBE9B6263154341A6668F95C7BBC3D224EBAB8D15D41599D` (일치)
- 대조: Approved C3 AC-M5D7QC3-002/004, 실제 decoder·recovery planner·confirmation owner, 기존 상위 독립 검수
- 범위: 설계 정적 검수만. 소스·시험·Unity 수정 또는 실행 없음.

## 판정

**P0=0, P1=1.** 세 실제 leaf의 관찰 분류와 Confirm 재관찰 행렬은 Approved C3의 제품 필드 분류·full-identity 재검증 경계를 구체화하고, 실제 공개 스키마/인코더/decoder와 정합한다. 다만 exact-default 결과와 AC004의 “새 decision generation” 문구 사이의 승인 해석이 아직 명시 승인되지 않았다. 이를 실행 기대값으로 임의 고정하기 전에 아스트라가 문서상 해석을 확정해야 한다.

## P1 — exact-default 직접 Confirm과 AC004 generation 문구의 승인 해석 필요

제안은 Primary 의미값→정확 기본값, Previous/Temp 삭제 후 Primary 기본값만 남는 세 실제 전이를 fresh actual capture로 확인하고, `NoConfirmationRequired`이면 새 prompt와 generation을 만들지 않은 채 fresh capture에 결속된 `Confirmed` request를 반환한다고 본다. 이 경로는 C3의 승인된 동작과 owner 구현에 부합한다. C3는 no-confirmation classification도 `Confirmed`만 발급할 수 있다고 정하고, owner Confirm은 fresh recapture가 해당 classification이면 `ConfirmedReady`로 직접 이동하며 `OpenFreshDecision`을 호출하지 않는다. 따라서 새 prompt가 없는 직접 확인에서 generation을 증가시키지 않는 것이 실제 계약/구현 해석이다.

남은 승인 쟁점은 AC-M5D7QC3-004가 “every leaf changed … including exact-default transitions … proves … a new decision generation”이라고 쓰여 있어 문자 그대로는 exact-default 전이에도 generation 증가를 요구하는 듯 읽힌다는 점이다. 상위 승인 본문은 같은 exact-default fresh recapture를 별도 no-confirmation 경로로 허용한다. 아스트라는 “새 generation은 fresh classification이 의미/모호하여 새 prompt를 여는 경우에만 적용하며, exact-default 직접 Confirm은 fresh identity에 결속된 별도 no-prompt Confirmed 경로”라는 해석을 승인 기록에 명시해야 한다. 실행 결과에 맞춰 기대를 바꾸지 말라는 제안의 원칙은 적절하다.

## 설계 정합성

- F01–F13의 필드 변형은 현재 snapshot 스키마에 존재한다. F12/F13의 선택·기술 쌍은 정상 생성자가 허용하는 완전한 쌍으로 시험하고 불일치 쌍은 실제 encoded invalid 바이트로 분리한다. `ProfileBindingOverridesJson.Parse("{}")`는 빈 문자열과 달리 canonical text가 비어 있지 않으므로 F07은 유효 의미 차이로 분류된다.
- unsupported 행의 `{"schemaVersion":2}`는 현재 decoder가 객체의 단일 정수 schema를 검사한 뒤 v1 envelope 검증 전에 `UnsupportedProfileSchema`를 반환하므로 제시한 간단한 실제 바이트가 유효하다. binding recovery는 payload hash를 다시 계산하고 public decoder 결과를 확인하도록 했다. unreadable은 실제 파일 잠금 실패를 관찰하며 root-lock Busy와 혼동하지 않는다.
- D 부분집합 7개, 전 leaf 부재, 역할별 단독 변형 및 의미/모호/unreadable 우선순위 배치는 AC002의 각 역할·분류 경계를 빠뜨리지 않는다. 각 행을 고정 ID·실제 파일 hash·decoder 결과와 묶고 실패 이후 미실행 행을 통과 처리하지 않는 실행 원장도 적절하다.
- AC004 변경 행은 실제 세 파일을 바꾸고 owner Confirm에 실제 재관찰을 제공한다. promptful fresh classification에서만 `OpenFreshDecision` 후 generation +1, stale capability 거절 및 새 capability 확인을 요구한다. 파일 전후 지문과 C1 미호출 확인도 승인 경계에 맞다.

## 별도 승인 시험으로 남는 항목

이 문서는 AC002/004 행렬이며 상위 승인 수용 전체를 닫지 않는다. AC003의 실제 decision capability null/default, 미등록 후보, 실제 capability 복제본, foreign owner, 새 generation에서 old 거절·fresh 성공 및 진짜 same-thread 재진입 음성 행은 별도 시험 설계/승인으로 추가해야 한다. lower capture clone 증거를 상위 decision capability 증거로 대체할 수 없다. Confirm call stack에 외부 callback 경계가 없다는 구조 검토는 실제 재진입 실행과 구별해야 하며, Rearm UI callback 재진입 시도는 live Confirm operation-gate 진입 증거가 아니다.

완료된 정상 rearm의 예약을 `PrepareNewGameSuccessor`에 재전달하는 기존 P1도 이 matrix로 폐쇄되지 않는다. 별도 보정은 실제 현재 pending reservation, 미커밋 상태, `owner.IsRearmInProgress(...)`가 모두 성립할 때만 준비를 허용하고 committed/current-slot은 history 조회에만 사용해야 한다. 실제 성공 rearm으로 얻은 tuple을 다시 전달하는 음성 행을 요구한다.

따라서 이 설계 문서 자체는 구현·실행 수용이 아니다. 위 AC003 capability/reentrancy 및 늦은 reservation P1과 전체 실제 runner 실행 증거가 해결될 때까지 C3 전체 수용은 보류다.

## 후속 폐쇄 검수 — 아스트라 승인 해석 및 재진입 증거 경계

- 대조 추가: Approved C3 후속 해석 `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c3-confirmation-owner.md`의 2026-09-29 절, 승인 `docs/approvals/2026-09-29-c3-required-decision-evidence-implementation-approval.md` SHA-256 `947853D21D1139C17E7339061A0C36A4A9AF8A3B8F0BE8A9E62A59C51144B994`, 재진입 제안 SHA-256 `DFF389EBE18B29EC5B6DC790FD8D73060A505CBEA3E99FCE17CA5161D62A3E79` (세 지문 일치).
- 범위: 승인 해석·설계만 재검수. 최종 구현·실행 시험의 P1 폐쇄를 주장하지 않는다.

아스트라는 AC-M5D7QC3-002/004의 해석을 명시했다. 새 capture가 `NoConfirmationRequired`이면 exact-default 실제 전이는 fresh 전체 identity에 결속된 직접 `Confirmed` 경로로 끝나며 새 prompt·decision capability·generation을 만들지 않는다. 의미/모호 fresh classification이 새 prompt를 요구할 때에만 `FreshDecisionRequired` 후 `OpenFreshDecision`으로 generation을 한 번 증가시키고 구 권한을 거부한다. 세 파일별 direct-default 전이를 각각 실행 시험하라는 조건도 명시했다. 따라서 이 승인으로 기존 matrix의 유일한 설계 P1(exact-default와 AC004 generation 문구 모호성)은 폐쇄됐다. matrix의 기대값은 승인 해석과 일치한다.

재진입 제안은 정확한 지문에서 승인 해석과 일치한다. 실제 Cancel이 발급한 구 decision/rearm 객체로 실제 Q-A 렌더의 UI 그래픽 콜백 안에서 같은 스레드 nested Confirm/Cancel/Rearm을 시도하고 비변경 거부를 확인한다. 이는 실제 중첩 호출이지만 이미 소비된 decision 또는 Rearming/소비된 rearm의 앞선 state/authentication 검사에서 거부되므로 live Confirm operation gate 재진입을 증명한다고 주장하지 않는다. Confirm의 frozen call chain에 정상 외부 callback 경계가 없는 사실은 별도 정확 지문·호출 사슬 구조 증거로 기록한다. 실제 Confirm/Cancel concurrent 승자 시험은 별도 유지하며 그 결과를 재진입으로 세지 않는다. 콜백이 실제로 호출되지 않으면 시험 실패다. 합성 제품 callback/API나 private 상태 대입 없이 이 세 증거 유형을 나누는 것은 승인된 AC003 해석과 부합한다.

**후속 설계 판정: P0=0, P1=0.** 이는 exact-default 해석과 재진입 증거 경계 설계만의 판정이다. AC003에서 승인한 null/default·미등록·복제·foreign·구 세대 상위 decision capability 음성/비소모 시험의 실제 구현·실행 및 앞서 기록한 completed reservation replay runtime P1은 별도이며 여기서 폐쇄하지 않았다. Terra의 새 frozen source와 신규 시험도 아직 독립 검수 대상이 아니고, Unity/실행 수용은 수행하지 않았다.
