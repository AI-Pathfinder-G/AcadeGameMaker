# 최초 코드 게시 출처 원장 v2 독립 결과 검수 — Luna

- 검수 결과: **제한 범위 수용 가능**. P0 0건, P1 0건.
- 검수자: Luna (`gpt-6-luna`), 구현 독립 검수.
- 검수 대상: `artifacts/c4-initial-publication-forward-provenance-v2.json`, SHA-256 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`.
- 승인 기준: `docs/specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance-v2-correction.md`, 승인 상태 `Approved`.
- 추적 기준: REQ-M5D7QC3-001/005/006/007, REQ-M5D7QC4-001..007; AC-M5D7QC3-009/010, AC-M5D7QC4-009/010.

## 독립 대조 결과

- **AC-M5D7QC3-009/010, AC-M5D7QC4-009/010 — v1 보존 및 역사 상태:** v1 원장 실물 SHA는 계약에 명시된 `C5D9C98C1F44A9D5A68F8F5C573787CF539E6069D2CE5EB7F0F35BB36B753EBE`와 일치한다. v2는 501개의 고유 경로를 보유하고 원본 501 경로 집합과 일치한다. 초기 489행의 원래 현재 SHA와 조사 전·후·새 기준 SHA가 모두 동일하다. 역사 공백 428, 기존 변경 전이 14·열림 14·닫힘 0이 보존된다.
- **AC-M5D7QC3-009/010 — 세 과거 문서 참조:** `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`, `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileNewGameConfirmationV1Tests.cs`, `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs` 각 1행에서 오래된 참여 원장 지문이 현재 근거 배열에서 빠지고 역사 문맥으로 이동했다. 옛 SHA, 줄 404/360/410, 원문 발췌가 보존되며 `HistoricalByteUnavailableOrUnverified=true`, `CurrentByteAuthority=false`다. 현 문서 SHA를 과거 SHA로 소급하지 않았다.
- **AC-M5D7QC3-009/010, AC-M5D7QC4-009/010 — 남은 문서 근거 8건:** 계약에 지정된 8개 인용의 실제 문서 SHA, 줄 번호, 발췌 접두 및 대상 파일 SHA 언급을 확인했다. 원장의 승인 경로 근거와 현재 독립 근거 총 170건도 실물 문서 지문·줄·발췌를 대조해 불일치 0건이다. `ProfileNewGameConfirmationV1Tests.cs` 근거 중 `2026-09-29-c3-lower-cas-integrity-implementation-evidence.md`는 구현 증거로 남아 있으며 독립 루나 검수로 오분류하지 않았다.
- **AC-M5D7QC3-009/010 — 기록된 원격 관측 재현:** UTC `2026-10-01T02:53:17.986430Z`–`02:53:19.253916Z`, HEAD `9d3e2573cf4a1b6646c4332b349a2a988f8dd827`, 트리 `96530a7cce8aae4bb933ac0cbffb7e5cc85c744a`의 기록을 원장과 대조했다. 보유된 해당 커밋 객체의 트리·blob을 읽어 후보 501개 중 존재 7·부재 494와 기록된 blob 식별자·SHA가 모두 일치하고 불일치 0건이다. 이는 그 기록 시점 증거의 재현이며 현재 원격 HEAD 주장으로 사용하지 않는다. 두 충돌 경로도 원장상 충돌 2건으로 유지된다.
- **AC-M5D7QC3-009/010, AC-M5D7QC4-009/010 — 바이트·미승인 표식:** 501개 현 파일 SHA와 `ForwardBaselineSha256`가 모두 일치한다. `PublicationApproved=false`, 포함 미결정 501, `CleanCloneExecuted=false`, `UnityExecutedByThisTask=false`, `WholeAccepted=false`다.

## 판정 및 미수용 범위

v2 보정 결과는 승인 계약의 범위에서 수용 가능하며, 종전 v1 결과의 두 P1(세 과거 문서 참조의 현 근거 오사용, UTC 및 정확 원격 스냅샷 결속 누락)을 v2 결과가 해결했다. 이 판정은 과거 428 공백이나 14 전이를 닫지 않으며, 501개 전체 내용·의존성, 파일별 게시 허용, 게시, 깨끗한 복제, Unity 또는 전체 수용을 승인하지 않는다. 현재 원격 상태도 판단하지 않았다.
