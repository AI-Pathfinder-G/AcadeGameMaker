# 최초 코드 게시 현재 바이트 출처 조사: 루나 독립 결과 검토

- 검토 대상 원장: `artifacts/c4-initial-publication-forward-provenance-v1.json`, SHA-256 `C5D9C98C1F44A9D5A68F8F5C573787CF539E6069D2CE5EB7F0F35BB36B753EBE`.
- 부속 한국어 보고: `artifacts/c4-initial-publication-forward-provenance-v1.md`, SHA-256 `632CA29B88F6B35A909B7C214308143F19D4EA63E820D3F9EC4CFDC0639CE5A8`.
- 테라 보고: `docs/verification/2026-10-01-initial-code-publication-forward-provenance-terra-report.md`, SHA-256 `2AC08D3A5FA462F6543DC749A85C918A66DADC1EB73543FCD573A00FEE9FC6FE`.
- 근거 계약: `docs/specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance.md`, SHA-256 `3D928CEF5DD68F20B4FD13F25B7812167378B62332C198C6079476C17EF31E86`.
- 승인 기록: `docs/approvals/2026-10-01-initial-code-publication-forward-provenance-contract-approval.md`, SHA-256 `FD52B1A556FE311E5511376537E189D484DAA388D474C5FBCEBBCC7AF6E184B5`.
- 선행 기준: 501파일 원장 `artifacts/c4-initial-publication-byte-closure-v1.json`, SHA-256 `E43A4268ABC87A56AD4F4A198D96893135706EB1364ABB33D70BA27741765B17`; 기존10 변경 사슬 `artifacts/c4-existing10-change-chain-v1.json`, SHA-256 `FB4D6929BBB3325DE452FE3386E6A88EEF847D4CF992CAD5742A5F63AA4F727F`.
- C4 고정 입력: v19 소스 원장 SHA-256 `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`, 입력 목록 SHA-256 `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`.
- 추적 범위: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`.

## 독립 대조 결과

원장의 501개 경로는 모두 고유하고 실제 파일이 존재했다. 각 파일을 다시 SHA-256으로 계산한 결과 501/501이 조사 전 SHA와 일치했고, 원장 자체에서도 조사 전후 SHA 차이가 0, 선행 원장의 현재 SHA와 다른 항목이 0이었다. v19 동결 소스 포함 표시는 37행 모두 실제 경로 집합과 일치했고 해당 행의 SHA 차이는 0이었다. 누락되거나 해시가 다른 실제 파일은 없었다.

역사 수치도 선행 원장과 일치한다. 원래 후보489, 직접 근거61, 공백428, 현 지문 동일 직접 근거60, C4 신규 후보12다. 기존 변경10 원장은 관측 전이14, 미해결 구간14, 닫힌 사슬0/10을 유지한다. 공백 분류 합계는 428이다. 새 원장은 과거 간격을 닫았다고 주장하지 않는다.

원장에 기록된 현재 출처 상태는 471개 독립 출처 미입증, 2개 원격 동일 경로 충돌, 22개 현재 SHA 관련 문서 근거는 있으나 게시 의존성 미입증, 6개 메타 제한 검수만 확인·전체 출처 미입증이다. 모든 501행은 포함 미결정이고 `PublicationApproved=false`; 최상위 `CleanCloneExecuted`, `UnityExecutedByThisTask`, `GitOrRemoteModified`, `WholeAccepted`도 모두 `false`다. REQ/AC가 비어 있는 행은 0이다. 신규 C4 12행은 6개 소스와 6개 메타로 구분되며, 원장상 v19 포함/현재 SHA 결속과 제한 검수 상태가 구별되어 있다.

## P1 — 현재 문서 지문과 다른 세 독립 검토 참조

173개 승인·독립검토 문서 참조 중 170개는 현 문서 SHA와 줄·발췌가 일치했다. 세 행의 `CurrentByteIndependentReviewEvidence`는 `docs/verification/gpt-participation-ledger.md`의 SHA-256을 `23085FFE1965391849BAD920C0C7DBBB92789F1FABFD05870302AF55126BFCC2`로 기록하지만, 현재 파일 SHA-256은 `E8965FB27F7C963A6E7104F564AB9D352914A9E5254D749DAF3D0D5E2C9827C2`다. 해당 참조는 다음 파일에 있다.

- `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` (원장 404행)
- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileNewGameConfirmationV1Tests.cs` (원장 360행)
- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs` (원장 410행)

세 발췌의 줄 위치와 텍스트는 현재 파일에서도 일치하지만, 원장이 결속한 문서 바이트는 현재 재현되지 않는다. 이 차이는 제품 소스의 SHA 불일치가 아니며 해당 문서가 조사 이후 바뀌었을 가능성과 양립한다. 다만 원래 문서 바이트나 동등한 불변 사본 없이 이 세 독립검토 인용을 현 문서 기준으로 재검증했다고 할 수 없다. 원본 원장을 고치지 말고, 기존 문서 이력으로 당시 SHA를 입증하거나 승인된 새 기준으로 새 결과를 발급해야 한다. 따라서 **완전한 계약 산출물의 검증은 P1로 열린다.**

## P1 — 원격 스냅샷의 정확 시각 및 독립 재조회 한계

원장은 원격 `main`을 커밋 `9d3e2573cf4a1b6646c4332b349a2a988f8dd827`, 트리 `96530a7cce8aae4bb933ac0cbffb7e5cc85c744a`로 기록하고, 후보 중 존재7·부재494·동일 경로 바이트 충돌2를 분류한다. 두 충돌은 `artifacts/c3-required-decision-expected-rows.json` 및 `qa/tools/Test-QaCatalog.ps1`이며, 원격과 현재 SHA가 각각 다르게 기록되어 있다. 그러나 `RemoteMainObservation.ObservedAtUtc`는 `null`이다. 계약은 원격 조회 시각을 기록하도록 요구하므로, 이 자료만으로는 해당 수치가 어느 시점의 원격 상태인지 확인할 수 없다.

저장소에 남은 원격 원시 증거는 이전 커밋 `4d864484f75939dcf9c93c055d296613cf1ba1dd`와 트리 `5de182aa5ed735755ed9ac9ee4fe071eac828381`을 가리킨다. 이는 이 원장의 `9d3e…` / `96530…` 관측을 독립 재현하지 않는다. 이번 검토에서 직접 API 재조회도 시도했으나 사용 가능한 조회 도구가 GitHub API 주소를 열지 못해 새 원격 값을 확인하지 못했다. 따라서 7/494와 충돌2는 원장에 기재된 조사자의 결과로 보존하되, 이 검토에서는 새 원격 관측으로 승인하지 않는다.

이는 원격 수치의 현재성·시각 결속에 한정된 P1이다. 계약이 요구하는 `미입증` 처리를 이미 적용하고, 모든 포함 판단과 게시 승인을 닫아 둔 점은 적절하다. 원격 최신 상태를 포함한 파일별 판정이나 게시 결정을 하려면 정확 UTC 시각을 가진 읽기 전용 재조회와 정확 커밋·트리·blob의 근거가 먼저 필요하다.

## 제한 결론

P0는 없다. 501개 로컬 현재 바이트, 전후 불변, 역사 집계, v19 표시 및 미결정/미승인 경계는 독립 대조에서 맞았다. 이 결과는 **현재 로컬 바이트와 역사 상태를 기록한 한정 스냅샷**으로만 사용할 수 있다. 세 독립검토 문서 SHA 불일치와 원격 관측 시각·재현 근거가 해소되기 전에는 완결된 출처조사 수용, 원격 상태 수용, 파일별 포함·제외, 게시 승인을 주장할 수 없다. 클린 복제·Unity·빌드도 검증하지 않았다.

이 검토는 지정된 새 출력 문서만 작성했다. 원장·원본·동결 입력·QA·Git·원격은 수정하지 않았고 Unity·컴파일·클린 복제를 실행하지 않았다.
