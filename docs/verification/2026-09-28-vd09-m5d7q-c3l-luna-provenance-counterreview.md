# C3L provenance 반대 검토 — Luna

날짜: 2026-09-28. 역할: GPT Luna 독립 소스 검수.

## 결함

`ProfileResetObservationContentionTimeoutExceptionV1`의 생성자가 현재
`internal`이며 `IOException` 원인의 HRESULT가 `0x80070020` 또는
`0x80070021`인지 만 확인한다. 따라서 lower C3 코드나 같은 내부 가시성을
가진 시험이 실제 `AcquireForObservation`의 5초 연속 점유 없이 해당 HRESULT를
가진 fabricated `IOException`을 넣어 timeout 예외를 직접 만들 수 있다.
예외 타입 안에는 실제 acquire 발급자, retained lock, bounded-wait witness,
또는 폐쇄된 issuer identity가 없다. `AcquireForObservation`의 정상 경로가
실제 점유와 시간 제한을 거쳐 발급한다는 사실만으로 직접 생성 가능한 예외의
provenance가 인증되지는 않는다.

이는 다음 기준의 P1 결함이다.

- `AC-M5D7QC3L-002`: 실제 retained exclusive `FileStream`과 실제 bounded
  timeout만 timeout 증거를 발급해야 한다.
- `AC-M5D7QC3L-003`: forged/default/reflection-corrupt provenance는
  `Unavailable` 또는 terminal fail-closed여야 한다.

## 최소 수정 방향

Terra는 기존 `Acquire`, `CaptureConfirmationIdentity`, `Begin`, `Resume` 및
내구성 본문을 건드리지 않고, observation lane 내부에만 폐쇄형 issuer 증거를
추가해야 한다. timeout 예외의 직접 `internal` 생성자를 제거하거나 private로
닫고, 실제 `AcquireForObservation`의 sharing-violation 재시도 루프가 보유한
private issuer/witness를 통해서만 생성하게 한다. lower consumer는 원인
HRESULT만 보지 말고 issuer identity와 정합성까지 확인하며, issuer가 없거나
변조·기본값이면 `CaptureBusy`가 아니라 `Unavailable`/terminal로 닫아야 한다.
시험의 throw-only 제어는 timeout issuer를 만들 수 없어야 한다.

## 판정

현재 C3L 소스는 이 반례 기준 **P0=0/P1=1**이다. 실제 Unity 실행이나
AC-M5D7QC3L-002/003 통과는 주장하지 않는다. 수정 후 Terra가 새 지문을
제출하면 Luna가 issuer 생성·소비·반사 변조 행을 다시 독립 검수해야 한다.
