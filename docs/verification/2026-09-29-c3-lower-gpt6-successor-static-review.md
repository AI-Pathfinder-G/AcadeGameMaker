# C3 하위 보정 successor 정적 독립 검수

- 검수자·실제 호출 모델: Luna, `gpt-6-luna`
- 날짜: 2026-09-29
- 범위: 동결 하위 런타임·시험 정적 검수. Unity 실행·소스/시험 수정 없음.
- 런타임 SHA-256: `19E142269AD34C94D5B01DD7F4EB603200E1639AFE99F7CCCAA4A1BBCE802114` — 지정값과 일치
- 시험 SHA-256: `79DC9982D74D7FAB470CDFE30C5C6CB35D1F7CFD05F6F62169FE5C7C7ADBC32E` — 지정값과 일치

## 판정

P0=0, P1=1. 앞서 발견한 두 결함은 보정됐다. `ConfirmedRequestWitness`는 실제 source result와 source witness, 원본 capture를 함께 보존하며 요청 root/capture와의 참조 동일성을 검사한다. 발급 결과 검증은 source witness의 분류를 현재 projection에서 재계산한다. 요청 인증 후 예약 전 검증과 예약 후 완료 검증의 예외는 `Closed`로 끝나며, 인증 단계에서 거부된 foreign 문맥은 원래 요청 상태를 소모하지 않는다.

테스트 소스는 47개의 `[TestCase]` 행과 12개의 독립 `[Test]` 행으로 정적 확장 총 59개다. 추가 행에서 참조하는 result/issuer/request/source/leaf/root 필드, CWT witness 접근 helper 및 clone helper는 현재 런타임에 존재한다. 실제 컴파일·Unity 실행은 하지 않았으므로 59개는 대기 시험 수이지 통과 결과가 아니다.

## P1 — 단일 CAS 필드 반사 롤백으로 소비 권한 부활 가능

두 witness의 mutable CAS 값만 반사로 이전 상태에 되돌리는 경우를 차단하는 검증이 없다. `ConfirmedRequestWitness.State`를 요청 완료/종료 상태에서 `0`으로 바꾸면 이후 `ReserveExecutionCommit`이 다시 성공할 수 있다. `ResultWitness.RequestMintState`를 요청 발급 후 `0`으로 바꾸면 동일 실제 capture에서 두 번째 확인 요청을 발급할 수 있다. 이는 전체 private state 임의 위조를 가정하는 지적이 아니라, 단일 lifecycle CAS 필드만 바꿔 이미 소비한 권한을 되살릴 수 있는지에 대한 범위 내 반례다. 현 시험 행에는 `State` 또는 `RequestMintState` 단독 변경·복구 행이 없다.

필요 보정은 두 필드 각각에 대해 단독 반사 롤백을 시도하고, 예약·완료 또는 재발급 권한이 복구되지 않는지 검증하는 것이다. 구현도 해당 단일 필드 변경을 발급/소비 witness와 대조해 fail-closed 처리해야 한다. 적용 추적은 REQ-M5D7QC3-001/006, AC-M5D7QC3-001/008 하위 증거다.

## 수용 범위

- 이전 capture-result 출처 결속 P1: 폐쇄
- 예약 CAS 전 인증 요청 실패 terminal-close P1: 폐쇄
- 현재 판정: P0=0/P1=1, 독립 수용 보류
- Unity 21/21 이전 결과는 이 successor 소스·59개 시험의 실행 증거로 재사용하지 않음
- C3 전체, AC-M5D7QC3-007/008 전체 및 C4: 수용 안 함
