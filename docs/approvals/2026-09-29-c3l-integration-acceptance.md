# C3L 관찰 단위 통합 수용

- 승인자: 아스트라. 날짜: 2026-09-29.
- 범위: Approved C3L의 AC-M5D7QC3L-001..005 및 사용자 승인 내부 읽기 전용 조회 함수.
- 구현: 테라. 실제 실행: 아스트라. 독립 검증: 루나.

아스트라는 [루나 최종 독립 검수](../verification/2026-09-29-c3l-final-regression-luna-acceptance-review.md)의 P0=0/P1=0과 원본 증거를 확인해 C3L만 Verified로 수용한다. 집중 11개, 현재 필수 회귀의 고유 613개를 확인했다. 필수 회귀 원장 SHA-256은 `13B8AEC521EF350714BABE9C4D83A040CDC071A50CA2C83A4616CFA3134025C8`이다. 선택한 현재 증거의 실패·건너뜀·판정보류 0, 시험 이름·중복 차이 0, 모든 실행 전후 입력 874개의 지문 차이 0이다.

R4 전체 결과는 561 통과·1 시간 초과 실패·실제 종료 2로 보존한다. 같은 소스·같은 180초 제한으로 수행한 R6의 정확한 한 행 통과와 실제 종료 0을 별도 증거로 선택했다. R5 별도 프로세스 51개도 새 실행 결과이며 과거 통과를 재사용하지 않았다. R4 실패를 소급 통과로 변경하지 않고 각 행의 실제 실행 출처를 유지한다.

## 수용한 정확한 소스

| 경계 | SHA-256 |
| --- | --- |
| `DesktopProfileLaunchAdapterV1.cs` | `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB` |
| `ProfileNewGameResetServiceV1.cs` | `23D7248B1A603A89BC8FF0FFDBB24FA04E640A66018DDC5689F97EC80701FBE5` |
| `ProfileResetDiskTransactionV1.cs` | `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2` |
| `ProfileNewGameConfirmationV1.cs` | `CF03C0E9FACAFD5D8967D3B2DB5A011AF29CFB4071BF47944417C5F883DE7F8F` |
| `ProfileNewGameObservationLeaseV1Tests.cs` | `E899F8531763CD47BE3366619A31CD91700F65EF6B2770339728B59E66E1E1AA` |

기존 Acquire, CaptureConfirmationIdentity, Begin, Resume 및 durable 본문은 보존했다. 실제 시간 초과만 Busy이며 나머지 접근 실패는 읽기 불가로 닫고, 관찰 캡처와 투영은 실제 내부 발급 증거로 검증한다. 재분석 지점은 정적 경로 보호 근거만 있으며 실제 운영체제 재분석 지점 시험을 통과했다고 주장하지 않는다.

C3 소유자·취소·재무장과 C3 전체 AC-M5D7QC3-010은 아직 수용하지 않는다. C3L 이후 구현은 ADR-0036의 단계적 검증 및 새 소스의 필수 회귀를 따른다. C4는 Review이며 실제 C1/C2 실행·맵·장면·목적지·화면·의상 변경을 허용하지 않는다. 문서 브랜치·위키 게시와 변경 요청 생성은 별도이며 변경 요청 인증 차단은 아직 남아 있다.
