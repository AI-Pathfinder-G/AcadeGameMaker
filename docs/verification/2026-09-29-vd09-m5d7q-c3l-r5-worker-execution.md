# C3L 별도 프로세스 필수 회귀 R5

- 담당: 아스트라. 추적: AC-M5D7QC3L-005.
- 실제 실행: 2026-09-29 00:22:50~00:27:21 한국 시각, 270.3006614초.

R4 실제 소유 편집기가 종료한 뒤 같은 동결 소스의 실제 별도 프로세스 검사 51개를 승인된 QA 도구로 실행했다. 실행 사전 확인을 통과했고 소유 편집기 29440의 실제 종료와 실행 도구 종료 코드 0, 51/51 통과를 확인했다. 실패·건너뜀·판정보류 0, 전체 시험 이름 차이 0, 중복 0, 전후 입력 874개의 경로·SHA-256 차이 0이다. R4 종료 입력과 R5 시작 입력도 정확히 일치한다.

실제 `.NET` SDK는 `10.0.401`이며 기존 fixture가 현재 `Profile` 조립 파일과 실제 작업자 소스로 별도 실행 파일을 구성했다. `Program.cs`와 프로젝트 파일을 입력 목록에 포함했고 저장소의 추가 SDK·빌드·패키지 설정 파일은 발견하지 않았다. 실제 자식 프로세스 종료·재시작·복구 시험이며 과거 R36 결과를 재사용하지 않았다.

## 원본 지문

| 원본 | SHA-256 |
| --- | --- |
| `artifacts/c3l-r5-worker.xml` | `2A4A4A21D891EC10B95BDC6E226BD049221F1AE7C1EFDD2A74AADD9126E59FC6` |
| `artifacts/c3l-r5-worker.log` | `52EDC242A94F41E423C1CB29E152F6FCBA46B1BCDFB6BC6D341A2F456F4D6738` |
| `artifacts/c3l-r5-source-before.json` | `D1FDF456F44AC9DC582B0B45C65BF2B80D55A7729942DDC873BA546AF37F39FF` |
| `artifacts/c3l-r5-source-after.json` | `33B1EF81F33C5E6E243700C8283BF5E125B93E7EED1AF087A5217BC1A2668698` |
| `artifacts/c3l-r5-verification.json` | `07BD5D79D5DD3A57D256A42C6880B504C1F4D836FCFA7F5EA265D7F84ABCB75D` |

R4의 알림 검사 1개 시간 초과는 여전히 미해결이다. R5 성공만으로 AC-M5D7QC3L-005나 C3L 전체를 수용하지 않는다. 소스·시험·제한 시간을 유지한 R6 한 행 진단과 루나의 독립 검토를 별도로 기록한다.
