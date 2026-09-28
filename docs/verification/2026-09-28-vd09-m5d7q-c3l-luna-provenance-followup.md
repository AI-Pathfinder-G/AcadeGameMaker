# C3L provenance 보강 후속 반대 검토 — Luna

날짜: 2026-09-28. 역할: GPT Luna 독립 소스 검수.

## 지문

- `Assets/AcadeGameMaker/Runtime/Profile/ProfileNewGameResetServiceV1.cs`
  SHA-256: `6BCBDDB7BDA553DC49A888384FA58F6581D0A4BCC3318FA402947995CBFB2536`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs`
  SHA-256: `13675608C636F1C6B05B10CADB62F4E157FA6300742B37FCDBCA5A6FA949317E`

## 결함

- **AC-M5D7QC3L-002/003 — P1 지속:** timeout 예외의 일반 생성자는 private가
  되었지만 `MintFromElapsedObservationAcquire(IOException)`와
  `ProfileResetObservationTimeoutProvenanceV1.Register(...)`가 여전히
  `internal`이다. 따라서 같은 내부 가시성 경계의 호출자가 실제
  `AcquireForObservation`의 5초 루프를 거치지 않고 sharing-violation
  HRESULT를 가진 fabricated `IOException`을 넣어 timeout을 mint하고 registry에
  등록할 수 있다. weak registry가 예외와 원인 객체의 동일성을 확인해도 실제
  issuer를 증명하지는 않는다. private issuer 증거를 mint와 Register 양쪽에
  요구하거나 두 경로를 실제 acquire 클래스 내부로 닫아야 한다.
- **AC-M5D7QC3L-004 — P1 검증 공백:**
  `CaptureConfirmationObservation`은 같은 lease 안에서 세 role을 순회하고
  존재하는 leaf를 읽어 decode 결과와 `Leaf`를 같은 입력으로 만든다. 그러나
  `ProfileResetConfirmationCaptureV1` 생성자는 identity null만 확인하며,
  projection·identity의 role, presence, 길이, hash, classification,
  revision 일치를 검증하는 `Validate` 경계가 없다. 현재 생성 경로의 정상
  값은 일치하지만 reflection-corrupt 또는 lower 소비자 전달 전 변조를
  capture 타입 자체가 fail-closed한다고 입증할 수 없다.

## 통과·미검증 구분

기존 `Acquire`, `CaptureConfirmationIdentity`, `Begin`, `Resume` 본문은 이번
읽기 전용 대조에서 변경 징후가 없고, timeout 원인 HRESULT 검사와 실제
5초 루프 연결은 유지된다. 다만 위 결함 때문에 C3L의 provenance 및 capture
AC를 전체 통과로 판정하지 않는다. Unity 실행·실제 점유 fixture도 실행하지
않았다.

## 판정

현재 독립 소스 검수는 **P0=0/P1=2**이다. Terra가 private issuer를 실제
발급 경계에 연결하고 capture projection/identity 검증을 추가한 뒤 새 지문을
제출해야 Luna 재검수와 Astra 수용 판단을 진행할 수 있다.
