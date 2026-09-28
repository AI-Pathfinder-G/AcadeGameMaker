# C3L timeout issuer 경계 독립 재검수 — Luna

날짜: 2026-09-28. 역할: GPT Luna 독립 소스 검수.

## 지문

`Assets/AcadeGameMaker/Runtime/Profile/ProfileNewGameResetServiceV1.cs`
현재 SHA-256:
`1B004F9951CEDD4FE9F24F6FE88445BD93B11CCA6CBC744625E3E03FF4A03BB1`.

`ProfileResetDiskTransactionV1.cs`는 기존 지문
`13675608C636F1C6B05B10CADB62F4E157FA6300742B37FCDBCA5A6FA949317E`를
유지한다.

## 독립 확인

- timeout 예외 형식은 `ProfileRootOperationLockV1` 내부 private nested type이다.
- timeout 생성과 weak registry 등록은 private `MintAuthenticatedObservationTimeout`
  하나로 닫혀 있으며, 실제 `AcquireForObservation`의 sharing violation과
  기존 5초 경과 분기에서만 호출된다.
- lower C3에는 생성·등록 API가 노출되지 않고, 내부 verifier
  `IsAuthenticatedObservationContentionTimeout(IOException)`만 노출된다.
- registry는 timeout과 원래 cause의 참조 동일성을 보존한다. 원래 cause의
  HRESULT를 snapshot하고, cause 또는 timeout HRESULT가 변경되면 verifier가
  거부한다. fabricated IOException은 registry에 없으므로 수용되지 않는다.

## AC 판정

- **AC-M5D7QC3L-002:** issuer·실제 cause 참조·5초 경과 분기·HRESULT
  snapshot을 소스에서 확인했다. 실제 retained FileStream fixture는 아직
  실행하지 않았으므로 실행 통과로 기록하지 않는다.
- **AC-M5D7QC3L-003:** 직접 생성·등록 경로와 cause/timeout HRESULT 변조는
  소스상 fail-closed다. 실제 fixture와 반사 변조 시험은 미실행이다.
- **AC-M5D7QC3L-004:** capture projection/identity agreement 보강은 아직
  남아 있으므로 이 기록에서 판정하지 않는다.

## 판정

timeout issuer 경계에 대한 정적 검수는 **P0=0/P1=0**이다. 이는 C3L 전체
수용이 아니다. capture agreement 결함과 실제 filesystem fixture 검증이
남아 있으며, AC-M5D7QC3L-005 통과도 주장하지 않는다.
