# VD-09 M5D7Q C3L 하위 런타임 발급 경계 보정 후 독립 검수

- 검수자: Luna
- 일자: 2026-09-28
- 범위: Terra 보정 후 동결 후보의 읽기 전용 재검수
- 대상 지문:
  - `ProfileNewGameResetServiceV1.cs`: `23D7248B1A603A89BC8FF0FFDBB24FA04E640A66018DDC5689F97EC80701FBE5`
  - `ProfileResetDiskTransactionV1.cs`: `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`
  - `ProfileNewGameConfirmationV1.cs`: `CF03C0E9FACAFD5D8967D3B2DB5A011AF29CFB4071BF47944417C5F883DE7F8F`

## 판정

이전 P1 두 건은 현재 정적 소스에서 보정되었다. P0=0, P1=0으로 정적 closure를 기록한다. 이는 시험·Unity 실행을 포함한 전체 수용이 아니다.

## AC별 확인

- **AC-M5D7QC3L-001/003 — 정적 PASS:** 정상 `Acquire` 경로와 관찰 전용 `AcquireForObservation` 경로가 분리되어 있다. 관찰 timeout은 private nested 예외와 private mint/CWT 등록으로만 만들어진다. `RegisterIssuedConfirmationCapture`는 private이고 `CaptureConfirmationObservation` 한 경로에서만 호출된다. lower 결과도 private `Issue`와 private CWT로만 발급된다. public ABI, friend, 새 assembly, 결과 Mint/Captured/Failure API는 추가되지 않았다.
- **AC-M5D7QC3L-002/003 — 정적 조건 PASS, 실행 미검증:** timeout witness가 원인 참조, 원인 HRESULT, 래퍼 HRESULT를 각각 저장한다. verifier는 timeout의 실제 HResult가 witness의 래퍼 값과 같고, 원인 참조 및 원인 원래 HResult가 유지되며, 원인이 정확한 Windows sharing violation인지 확인한다. 따라서 직접 생성·반사 변조·원인 HResult 0x20→0x21 변경은 닫힌다. 실제 5초 contention과 root-as-file 등 fixture는 실행하지 않았다.
- **AC-M5D7QC3L-004 — 정적 PASS:** 같은 실제 lease 안에서 Primary/Previous/Temp 세 역할을 한 번씩 읽고 identity와 projection을 함께 만든다. disk CWT verifier는 identity 참조, 세 leaf, projection 존재와 값 일치를 재대조한다. missing leaf에는 projection을 허용하지 않는다.
- **AC-M5D7QC3-002/004 — 정적 PASS:** lower 결과는 private 필드를 유지하면서 내부 읽기 전용 `MatchesIssued`를 통해 witness와 비교한다. `ValidateIssuedResult`는 witness의 실제 발급 인자를 대조하고, captured 결과의 root·identity root·세 leaf agreement를 재검증한다. `ProfileSnapshotV1`의 `Settings`, `Input`, `Tutorial`, `Progression` 및 `IsDefaultProduct`가 참조하는 모든 필드는 현재 public property와 일치한다.
- **AC-M5D7QC3-010 / AC-M5D7QC3L-005 — 미검증:** 컴파일러·EditMode/PlayMode·Unity 및 Terra 시험 fixture를 실행하지 않았다.

## 정상 Busy 및 분류 경로

lower의 Busy 분기는 인증된 observation timeout만 수용한다. root 생성·권한·보안·비경합 I/O는 Unreadable로 닫히며, 인증되지 않은 IOException은 Busy가 되지 않는다. 관찰 성공 시 세 leaf를 모두 분류하고, Previous/Temp 존재·복구 필요·invalid·unsupported는 ambiguous로 남기며, primary 유효 current의 비기본 제품 필드는 meaningful로 분류한다. 실패 결과에는 root/capture/classification을 포함하지 않는다.

## 보존 증거

기존 legacy 본문과 디스크 본문 보존에 관해 Terra가 제시한 재구성 SHA는 각각 service additive 제거 기준 `2E1DCE184495D4AABAC135AFC6910121A94D69498A85669A7D7EA1BD5E554098`, disk legacy 재구성 기준 `168D1539EAF359C3242ECABADAC0776956370E52105007D9224566F7F9C3FDC4`이다. 본 검수는 해당 보존 증거를 읽기 전용으로 확인했으며, 시험파일·legacy 본문을 수정하지 않았다.

이 기록은 Terra 구현의 자기 수용이나 C3/C3L 전체 통과 선언이 아니며, Astra의 통합 및 실제 시험 결과가 필요하다.
