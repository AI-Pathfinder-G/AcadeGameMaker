# C4 Play R2 동결 결속 후속 한정 계약

- 상태: **Approved — 아래 증거 결속 발급만 승인; 실제 실행·통합 수용은 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`. 기존 Play 행동 `AC-M5D7QC4-001..008`은 그대로 유지한다.
- 설계: `docs/proposals/2026-09-30-c4-play-r2-freeze-binding-successor-design.md`, SHA-256 `4C9A0901B2D6CA9B9F4F30B2DD68FE57E215271910538A5F2AE7BE7EE7D4EC0F`.
- 독립 설계 검수: `docs/verification/2026-09-30-c4-play-r2-freeze-binding-successor-luna-review.md`, SHA-256 `C10B3BA90EF3BFB7687BCC01E815A4ED9708807848A36F21D2556C80FB21AC67`, P0/P1=0/0.
- 선행: Approved C4 r4, R1 Play 보정, R2 현재 소스 지문 결속 계약. 이 문서는 R2 계약의 Play 선택 v3 불변 조건만 새 v4 후속 발급으로 한정 변경하며 제품·시험 행동 권한을 넓히지 않는다.

Luna의 v12 동결 검수 SHA-256 `926E05E4EC25ACB5E346E820C8BCDE6673FD9B9DB6DDEBE55131D52A6FB4822A`는 P0=0/P1=1이다. Play 선택 v3의 `ParserEvidence.EnumSources` Bridge 지문은 옛 `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`이고 현재 실물·원장 v12·188행 v7은 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`다. v3의 32개 이름·순서·선택식·부모 SHA는 맞다. v12 원장의 Edit fixture `ChangeAuthority` 문구는 v11을 가리키지만 `AllowedChanges.Reason`과 승인 계약은 v12 변경을 정확히 허용한다. 원본 v3·v12·입력 v12 및 이전 실패 출력은 수정하지 않는다.

## 허용 발급

1. 기존 `c4-build-focused-selection.ps1`의 필요한 소스·도구를 임시 동일 상대경로 작업 공간에 최소 복사해 Play 선택을 실제 재생성한다. 임시 공간의 고정 출력 `artifacts/c4-focused-play-selection-v1.json`은 실행 전 존재하지 않아야 하며, 존재하면 `CreateNew` 실패로 멈춘다. 저장소의 기존 v1은 건드리지 않는다. 재생성 결과를 새 `artifacts/c4-focused-play-selection-v4.json`으로 `CreateNew` 발급하고, v3과 전체 구조를 비교해 Bridge `ParserEvidence.EnumSources.Sha256` **한 값만** 현행 지문으로 바뀌었음을 증명한다. 32개 이름·순서·선택식·부모 `SourceFiles` SHA·그 밖의 모든 필드는 정확히 같아야 한다.
2. 새 `artifacts/c4-frozen-source-manifest-v13.json`은 v12의 모든 기존 `Files` 경로·순서·SHA·종류와 `AllowedChanges`를 보존한다. Edit fixture의 `ChangeAuthority` 한 문구만 R2 v12 승인 결속으로 정정한다. 새 선택 v4, 원장 v12, 이 승인 계약, 위 설계 제안서, Luna 독립 설계 검토서를 각각 정확한 SHA의 `Evidence`로 한 번씩 추가하고 요구사항·수용 기준 사유를 `AllowedChanges`에 연결한다. 최종 `Count`는 실제 파일 수이며 경로는 유일하다. 기존 Runtime·fixture·부모의 바이트 또는 SHA는 바꾸지 않는다.
3. 새 `artifacts/c4-frozen-input-paths-v13.json`은 입력 v12의 1024개 기존 경로·사유를 보존하고, 선택 v4·원장 v13·이 승인 계약·설계 제안서·독립 검토서를 직접 입력으로 추가한다. 실제 수를 계산하고 파일 존재·경로·사유를 검증한다. 최종 계획은 v13 원장/입력, v7 행, Play 선택 v4의 경로와 실제 SHA를 가리켜야 한다. 기존 계획·결과 파일을 덮어쓰지 않는다.

Play/Edit fixture는 v12를 계속 읽고 v13은 그 **동일한 최종 소스 바이트**를 다시 검증한다. v12와 v13은 모두 v13 입력 목록에 결속한다. 일곱 단일 호출 토큰, 제품 코드, QA 판정 규칙, 188행 내용·순서·값과 기존 Play 32개 사례는 변경하지 않는다. Luna는 v4의 유일한 구조 차이와 v13 변경 사유·증거·실물 SHA, v12→v13 입력 보존 및 fixture의 v12 참조 관계를 독립 정적 검수한다. 그 뒤 새 Play 진단을 별도 출력으로 실행하고 원시 종료·32개 실제 행·입력 전후 차이를 기록한다. 성공해도 최종 9회 큐를 대체하지 않는다. 실패는 별도 분류하고 기존 0/32 또는 19/32를 소급 변경하지 않는다.
