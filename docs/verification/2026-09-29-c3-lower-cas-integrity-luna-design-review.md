# C3 하위 CAS 단일 필드 무결성 설계 독립 검수

- 검수자·실제 호출 모델: Luna, `gpt-6-luna`
- 날짜: 2026-09-29
- 대상: `docs/proposals/2026-09-29-c3-lower-single-field-cas-integrity.md`
- 대상 SHA-256: `D23ED6F90FE58198A4C5778133083463CA4715BB72CF87DD377C3CDC1A6C6754` — 지정 지문과 일치
- 범위: 설계 정적 검수. 구현 승인·소스/시험 변경·Unity 실행 없음.

## 판정

P0=0, P1=0. 설계는 `ResultWitness` 및 `ConfirmedRequestWitness` 참조를 key로 하는 별도 비공개 CWT 생애 기록에 실제 불변 사건 참조를 두고, 사건에서 원본 witness·실제 request·reservation 쌍을 참조 동일성으로 연결한다. `RequestMintState`/`State` 정수만 과거 값으로 되돌리면 사건 head와 불일치하여 인증 후 종료되고, 숫자 쌍의 동등성만으로 권한을 내주지 않는다. 기존 CAS는 단일 승자 전이로 유지된다. 본 판정은 질문에서 지정한 단일 CAS 필드 변조에 한정하며 이벤트 head나 여러 private field의 동시 변조로 공격 범위를 넓히지 않는다.

인증 전 foreign adapter/router/owner/epoch/generation 불일치는 lifecycle 잠금·종료 경로 밖에서 거부되어 원래 권한을 소모하지 않는다. 인증 이후 알 수 없는 값, 이벤트/정수 불일치, 원본 출처·root 훼손은 종료한다. 발급 뒤 정상 duplicate mint, 예약/완료의 동시 패자는 현재 사건과 상태가 정상 승자에 의해 진행된 경우로 구별해 승자의 이력을 닫지 않는다. 실패 경로는 기록 생성 시 미리 준비된 `FailureClosed` 사건을 써서 추가 할당 없이 terminal close한다.

하위 전용 잠금은 CWT/상태/사건의 갱신을 한 임계 구역에 둔다. 상위 C3는 Confirm/Cancel/retry를 자체 CAS로 먼저 단일화하며 동시·재진입 호출을 비변이 거부하고 대기열에 넣지 않는다. 제안은 사용자 콜백을 잠금 안에서 부르지 않으며, 실제 launch-root getter도 동기 검증 후 값을 반환하는 내부 함수로 콜백을 호출하지 않는다. 따라서 하위 잠금이 상위 callback gate를 대체하거나 그 계약을 완화하지 않는다. 향후 구현은 같은 lifecycle 재진입을 중간 사건에서 허용하지 않아야 한다.

C4 보고·identity 추출, C1 `Begin`, C2, Hub 역참조나 assembly/friend/API 확장은 제안에서 제외되고 기존 C3 계약과 승인 파일 범위를 유지한다.

## 구현 전 고정할 세부사항

예외 경로의 `ClosedHistory` 기록은 실제 이전 사건을 잃지 않으며 추가 할당 없이 수행돼야 한다. 구현 시 생애 기록 생성 단계에서 필요한 유한 슬롯을 미리 마련하거나 동등한 비할당 방식임을 고정한다. prebuilt `FailureClosed`가 미리 알 수 없는 실제 reservation/witness를 포함해야 하는 지점은 해당 쌍을 기록에 안전하게 결속한 뒤 예외가 나도 복구·재사용이 불가능하도록 순서를 정하고, `AC008_IntermediateRegistrationFailureLeavesTerminalActualPair`로 확인한다. 이는 현 제안의 P0/P1 결함 판정은 아니며, 선언한 실패 보장을 구현에서 약화하지 않기 위한 필수 검증점이다.

요구된 단일 필드 시험은 발급 후 `RequestMintState` 복구, Reserved/Completed/Closed 각각에서 `State` 복구, 알 수 없는 값 후 복구, 실패 후 복구를 원본 사건 증거 그대로 두고 검증하도록 제안되어 있다. 정상 동시 패자의 비종료성, 외부 문맥 거절 뒤 원래 권한의 정상 1회 성공, 실제 등록 단계 예외의 terminal close도 별도 행으로 추적 가능하다. 이 시험명은 계획이며 결과 증거가 아니다.

## 수용 경계

이 결과는 정확 지문의 bounded counter-design만 설계 검수했다. 구현 승인이나 런타임 검증이 아니며, 기존 하위 P1은 구현 후 새 지문·실제 시험·독립 검수로만 폐쇄할 수 있다. AC-M5D7QC3-007/008 전체, C3 전체 및 C4는 이 검수에서 수용하지 않는다.
