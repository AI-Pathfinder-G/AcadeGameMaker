# VD-09 M5D7Q C3L 하위 런타임 발급 경계 독립 검수

- 검수자: Luna
- 일자: 2026-09-28
- 범위: Terra 동결 후보의 읽기 전용 정적 검수
- 대상 지문: `ProfileResetDiskTransactionV1.cs` SHA-256 `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`, `ProfileNewGameConfirmationV1.cs` SHA-256 `D66DEB968D546C5C225E7D1D1F67032FFFE0DB5C799948790DEB2EE03D997ABB`, C3L 서비스 SHA-256 `1B004F9951CEDD4FE9F24F6FE88445BD93B11CCA6CBC744625E3E03FF4A03BB1`

## 판정

현재 후보는 **수용하지 않는다**. `ProfileNewGameCaptureResultV1`의 `ResultWitness`와 `ValidateIssuedResult`가 다른 클래스의 private 필드(`_owner`, `_epoch`, `_root`, `_capture`, `_outcome`, `_classification`)에 직접 접근한다. C# 접근 규칙상 현재 소스는 컴파일 가능한 상태로 볼 수 없으며, 이는 C3 런타임 진행을 막는 P1이다. 또한 C3L 서비스의 timeout verifier가 timeout 예외의 HRESULT를 원인 HRESULT와 같아야 한다고 요구하지만, 현재 private nested timeout 예외 생성은 기본 HRESULT를 유지하므로 실제 sharing timeout도 인증되지 않는 P1이다. Terra가 두 지점을 보정한 새 지문으로 다시 검수해야 한다.

위 두 결함을 제외한 발급 경계의 정적 구조에는 이 검수에서 추가 P0/P1을 발견하지 않았다.

## 확인 결과

- **AC-M5D7QC3L-001/003, P0=0:** 디스크 관찰 발급은 `CaptureConfirmationObservation` 내부의 private `RegisterIssuedConfirmationCapture` 한 곳에서만 수행된다. `ValidateIssuedConfirmationCapture`는 CWT 등록 여부, identity 참조, 세 leaf, projection 존재와 값 일치를 읽기 전용으로 대조한다. 하위 결과는 private `Issue`와 private `IssuedResults` CWT로만 발급된다. 확인된 내부 공개 경계는 검증 함수와 실제 관찰 함수이며 `Mint`, `Captured`, `Failure` 발급 API는 존재하지 않는다.
- **AC-M5D7QC3L-004, P0=0:** 관찰은 실제 lease 안에서 Primary/Previous/Temp 세 역할을 한 번씩 읽고, identity leaf와 decoded projection을 함께 구성한다. 결과 검증은 owner/epoch/root/capture/outcome/classification을 발급 witness와 대조하고, captured 결과에서 root와 identity root, 세 leaf agreement를 다시 확인한다. missing leaf에는 projection을 허용하지 않으며 present leaf는 classification·revision·decoder 결과를 대조한다.
- **AC-M5D7QC3L-003, P1=1:** 설계된 경계는 private nested timeout 형식과 private mint/CWT 등록이며, root 생성·권한·보안·비경합 I/O를 timeout으로 승격하지 않는다. 그러나 현재 `ObservationContentionTimeoutException(IOException cause)`가 기본 HRESULT를 유지하는데 verifier는 `timeout.HResult == witness.HResult == cause.HResult`를 요구한다. 따라서 실제 5초 Windows sharing HRESULT `0x80070020` 또는 `0x80070021` timeout이 Busy 증거로 인증되지 않는 정상행 결함이다. timeout HRESULT를 발급 witness에 저장하고 verifier가 timeout 원래값과 cause 원래값을 각각 대조하도록 보정해야 한다.
- **AC-M5D7QC3-002/004, P0=0:** 하위 분류는 present/missing 세 leaf를 모두 순회한다. primary의 유효한 기본값만으로 우회하지 않고, previous/temp 존재·복구 필요·invalid·unsupported를 ambiguous로 남긴다. primary의 유효한 비기본 제품 필드는 meaningful로 분류한다. failure 결과에는 root/capture/classification을 싣지 않는다.
- **컴파일 가능성, P1=1:** `ProfileSnapshotV1`의 `Settings`, `Input`, `Tutorial`, `Progression` 및 `IsDefaultProduct`가 사용하는 모든 중첩 필드(`WindowMode`, 세 volume, 두 invert, input asset/schema/CanonicalText, `ConfirmedIds`, progression 다섯 필드)는 현재 public property로 존재한다. 다만 위의 private 필드 직접 접근 때문에 이 확인만으로 컴파일 통과를 주장할 수 없다.

## 보류 범위

실제 5초 contention, root-as-file, lock-directory, unreadable path, reflection 변조, 세 leaf fixture와 회귀/Unity 실행은 수행하지 않았다. 따라서 **AC-M5D7QC3L-002/003/004/005 및 AC-M5D7QC3-010은 미검증**이다. 이 기록은 C3/C3L 전체 수용이나 Terra 구현 수용이 아니다.
