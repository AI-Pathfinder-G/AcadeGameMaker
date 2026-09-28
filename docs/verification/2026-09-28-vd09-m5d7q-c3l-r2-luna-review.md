# VD-09 M5D7Q C3L focused R2 Luna 독립 검토

- 검수자: Luna
- 시험 XML SHA-256: `66C3DCC6D6CFFE8A939C32F981CD984393DB9A262DFBBE7FA7BDD7138D0F9D63`
- 시험 로그 SHA-256: `66F0FBBE698225B922466B17B3A529BE0401015929944CB143A7A82D71CB7F5C`
- 시험: 11개 중 10 passed, 1 failed, skipped/inconclusive 0, Editor exit 2

## 판정

R2는 **실행 통과가 아니라 부분 실행 증거**다. 실패는 runtime 결함이 아니라 `AC003_RootFileAndLockDirectoryAreUnreadableWithoutCaptureEvidence`의 fixture setup에서 발생했다. 첫 root-file 행 뒤 두 번째 fixture가 `profile.operation.lock` 디렉터리를 만들 때, production `AcquireForObservation`가 먼저 해당 이름의 lock 파일을 만들었기 때문에 `Directory.CreateDirectory`가 충돌했다. Terra가 소유된 lock 파일을 directory fixture 생성 전에 삭제하는 보정을 진행 중이다.

R2 XML의 10개 통과 결과는 실제 결과로 보존하지만 C3L 전체 수용이나 AC-M5D7QC3L-005/AC-M5D7QC3-010 통과로 승격하지 않는다. R2 source before/after manifest는 각각 874개이며 파일 경로·SHA 비교 차이 0이다. manifest JSON 자체 SHA가 다른 것은 capture 시각 등 메타데이터 차이이며 파일 행 차이는 없다.

동결 runtime 정적 판정은 service `23D7248B1A603A89BC8FF0FFDBB24FA04E640A66018DDC5689F97EC80701FBE5`, disk `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`, lower `CF03C0E9FACAFD5D8967D3B2DB5A011AF29CFB4071BF47944417C5F883DE7F8F` 기준 P0=0/P1=0을 유지한다. 다음 R3는 보정된 fixture의 새 결과로 독립 판정해야 한다.
