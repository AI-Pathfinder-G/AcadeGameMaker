# C4 기존 변경 10개 파일 사슬 원장의 독립 검수

- 승인 계약: `docs/specs/work-contracts/2026-10-01-c4-existing10-change-chain.md`, SHA-256 `119470C78C66ACCCA7EF19518A57622948675D6B1801FD4CE3DFE37818A4FC15`.
- 테라 JSON: `artifacts/c4-existing10-change-chain-v1.json`, SHA-256 `FB4D6929BBB3325DE452FE3386E6A88EEF847D4CF992CAD5742A5F63AA4F727F`.
- 테라 설명: `artifacts/c4-existing10-change-chain-v1.md`, SHA-256 `5B05885BA22A856D2647CE665DEE1D02759667CD09A35D1B62ABD5148F8AFDEA`.
- 테라 보고: `docs/verification/2026-10-01-c4-existing10-change-chain-terra-report.md`, SHA-256 `E8592E4F402E5427129C1D83DA2D65B7DE8C3F9317AD4B4C4B6F3C410CD70294`.
- 대조 원장: 부모 501파일 SHA-256 `E43A4268ABC87A56AD4F4A198D96893135706EB1364ABB33D70BA27741765B17`, 이전 22파일 JSON SHA-256 `1B80A52913CFD1119F97BAA9B667A6DBE2773FBBB33A167E647785AAB0A655BB`, v19 source manifest SHA-256 `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`, input list SHA-256 `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`.
- 추적: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`.

## 독립 대조 결과

P0 0건, P1 0건(원장 정확성 판정). 테라 JSON은 기존 변경 10개만 포함하고 신규 source/meta 12개 및 C3 2개를 분리한다. 부모 원장의 10개 `CandidateSha256`와 이전 22파일 원장의 `PreviousCandidateSha256`를 모두 대조해 일치했다. 열 실제 파일을 재해시한 현재 SHA 10개도 원장의 현재 SHA 및 v19 파일별 SHA와 일치한다.

각 파일의 타임라인 20항목(원래 후보 1개와 C4 동결 v1~v19)을 v1~v19 매니페스트 실물 19개와 대조했다. 각 매니페스트의 실제 SHA, 해당 경로의 SHA, 타임라인 값 및 `FileByteAvailableNow` 표시가 일치했다. 원장 14개 전이는 타임라인에서 SHA가 실제로 달라지는 지점을 다시 계산한 결과와 모두 일치한다. 전이 14개와 미해결 구간 14개, 닫힌 사슬 0개/열린 파일 10개라는 집계에도 모순이 없다.

전이의 승인 범위 45개 인용은 각 문서의 현재 SHA·기록 줄·문맥과 대조해 모두 일치했다. 독립 검토 인용 40개도 현재 문서 SHA·줄·범위 인용이 일치했다. 각 전이의 관측/다음 동결 매니페스트 14개 역시 실제 파일과 지문이 맞는다. 그러나 14개 전이 전부 `ImplementationEvidence`가 비어 있다. 승인·검토 링크는 바이트가 승인된 시점과 변경 범위를 완전히 잇지 못하며, 원래 후보 바이트 자체도 현재 보유되지 않았다고 원장이 표시한다. 따라서 현 SHA와 v19 일치 또는 매니페스트 간 변경 관측만으로는 승인된 구현 변경 사슬을 증명할 수 없다.

## 판정 범위와 남은 공백

이 결과는 테라가 14개 전이를 관측하고 14개 구간을 미입증으로 남긴 분류가 정확함을 확인한다. 이는 10개 사슬을 닫는 수용이 아니다. 실제 구현 산출 바이트·허용 범위와 시점·해당 바이트를 다룬 독립 검수·후속 동결을 전이마다 연결하는 자료가 보충되기 전까지 `ChainClosed=false`를 유지해야 한다. 게시 승인 플래그는 전체와 행별 모두 `false`이며, 22파일 전체 변경 사슬은 기존처럼 폐쇄 0·열림 22다. 신규 source/meta 12개와 C3 2개는 이번 조사에서 제외됐다.

새 루나 결과 경로 `docs/verification/2026-10-01-c4-existing10-change-chain-luna-review.md`는 작성 전에 없었고 v19 입력·source manifest 밖이었다. 이번 검토는 읽기 전용이었다. Unity, QA, Git, 원격, 클린 복제 및 게시를 수행하지 않았으며, C3/C4 전체 수용을 주장하지 않는다.
