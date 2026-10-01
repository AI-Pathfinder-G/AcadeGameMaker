# 최초 코드 게시 현재 출처 원장 v2의 한정 보정 계약

- 상태: **Approved — 아래 읽기 전용 재조회와 새 증거 네 파일만 승인.** 2026-10-01 아스트라(`gpt-6-astra`). 게임 코드 게시·깨끗한 복제·Unity 실행 권한은 없다.
- 승인 근거: 검토본 SHA-256 `426DD223C2B83B71969D423194728E91BB0DB1288DE3D36B685A9EBA4AFA5B6C`; [루나 독립 재검토](../../verification/2026-10-01-initial-code-publication-forward-provenance-v2-correction-luna-review.md) SHA-256 `0C77B026BE155924314BB571EAA6857A3A7CA024EAE292A41C78D1B65E57C3C5`, 설계 P0/P1=0/0. v1 실증 P1 두 건은 v2 결과 검수 전까지 열린다.
- 검토 이력: 원본 v2 Draft SHA-256 `2DB944C117F38509F6CD17B24249F8BA493DC9619090D4147B7CA7FDF3AAC306`은 불변이다. 루나가 지적한 표의 문서 SHA 한 곳만 실물 값으로 바로잡았으며 다른 계약 조항은 유지한다.
- 원본 보존: [Approved v1 계약](2026-10-01-initial-code-publication-forward-provenance.md) SHA-256 `3D928CEF5DD68F20B4FD13F25B7812167378B62332C198C6079476C17EF31E86`, v1 원장 (`artifacts/c4-initial-publication-forward-provenance-v1.json`; 로컬 원본) SHA-256 `C5D9C98C1F44A9D5A68F8F5C573787CF539E6069D2CE5EB7F0F35BB36B753EBE`, [루나 결과 검토](../../verification/2026-10-01-initial-code-publication-forward-provenance-result-luna-review.md) SHA-256 `D964D98D5C8D01FC08CE9FFE820DD9D9E30D0B60EAB71A4150090B658D3C1E56`의 P0=0/P1=2를 역사 그대로 둔다. 이 문서는 v1의 세 참조 및 원격 시각 보정 후보이며 코드·QA·동결 자료를 수정하지 않는다.
- 추적: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`. 새 기준 선택 B의 과거 공백 보존, 파일별 출처, 독립 검수, 게시 전 게이트를 유지한다.

## P1-1 — 세 행의 독립 근거와 역사 참조 분리

v1의 `CurrentByteIndependentReviewEvidence` 세 참조는 `docs/verification/gpt-participation-ledger.md`의 **과거 SHA** `23085FFE1965391849BAD920C0C7DBBB92789F1FABFD05870302AF55126BFCC2`를 현 독립근거로 복사했다. 현재 그 문서 SHA `E8965FB27F7C963A6E7104F564AB9D352914A9E5254D749DAF3D0D5E2C9827C`와 다르다. 줄·텍스트가 같아도 과거 전체 문서 바이트 검증을 대신하지 않는다. 아래 세 행의 해당 참조만 현 근거 배열에서 제거하고 `HistoricalContextEvidence`에 과거 SHA·줄 `404/360/410`, 원장 v1의 원래 발췌, `HistoricalByteUnavailableOrUnverified=true`, `CurrentByteAuthority=false`로 구분한다. 과거 SHA를 현재 원장 SHA로 단순 치환하거나 v1을 덮어쓰지 않는다. 과거 원문 바이트가 실제 독립 보존 자료로 나타나지 않으면 역사 근거는 미입증이다.

나머지 문서 참조는 각 파일의 **현재 SHA·실제 줄·발췌 접두·대상 파일의 현재 SHA 언급**을 다시 확인한다. 다음 3/3/2개는 v1에 기재된 나머지 참조이며, 사전 읽기에서 문서 SHA 8/8, 줄 접두 8/8, 대상 SHA 언급 8/8이 일치했다. 결과 원장에는 이 사실을 문서별로 새로 기록한다. 특히 중간 행의 첫 번째는 구현 증거 문서이므로 독립 루나 검수 문서로 잘못 분류하지 않는다.

| 대상 파일 | 보존해 재검증할 문서 SHA-256·줄 |
| --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | `2026-09-30-c4-editmode-disposal-pointer-recovery-luna-implementation-review.md` `03AC446E15FBD950CEA90E8D50644B9E5DE1F299E2932DE8CEFCAD2D9A017863`:4; `2026-09-30-c4-play-r1-bounded-recovery-luna-review.md` `70C5A9E8B8AA84D8DF675161A65EC199BC3DCEAB9CFD84BDFFE02418E87A93CB`:5; `2026-10-01-c4-v19-core-static-luna-review.md` `84FDEE13D70A609A110E04ECD9C79C71F96FFD242073F008FD2733EB870073C7`:4 |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileNewGameConfirmationV1Tests.cs` | `2026-09-29-c3-lower-cas-integrity-implementation-evidence.md` `C49BFCDDDBAE8518F04D89C52E87974F63916B553C227E204E933373D16E271E`:20; `2026-09-29-c3-lower-compile-access-correction-luna-review.md` `C0A9A2A83A9EC1F2F13C5D646B317D37DF5D0DECD55A6044AB0508553455B47D`:5; `2026-09-29-c3-upper-coherent-runtime-luna-review.md` `95BB6951380077708FBEBCBB80B821660E8C60F7107FB8EAEDDAB465A5BCC6DB`:7 |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs` | `2026-09-30-c4-r5-row-alignment-luna-implementation-review.md` `884F0D7A1573F619D391C2EDC8F2CB03B01590B180F708117C90E885E513982F`:5; `2026-09-30-c4-r5-selection-rows-queue-luna-review.md` `C220799B5658B633F654E261B41170FD80AA1275E673685CE21092A678BB88D4`:10 |

이 여덟 인용의 범위는 문서가 실제로 말하는 당시/현재 파일 SHA와 검수 종류로 제한한다. 다른 독립 검수의 존재만으로 제품 바이트의 전체 출처·게시 의존성이 닫혔다고 하지 않는다. 세 행을 제외한 498행의 현 근거·상태는 새 원격 조회와 기준 전후 검증 결과가 달라지지 않는 한 v1과 같아야 한다. 세 행도 위 두 P1과 직접 관련 없는 의존·REQ/AC·게시 상태를 바꾸지 않는다.

## P1-2 — 원격 시각과 정확 스냅샷의 재현

v1은 `RemoteMainObservation.ObservedAtUtc=null`이면서 `9d3e2573cf4a1b6646c4332b349a2a988f8dd827`/트리 `96530a7cce8aae4bb933ac0cbffb7e5cc85c744a`의 7존재·494부재를 적었다. 이것은 기록된 조사자 관측으로만 보존하고 현재 원격의 승인 근거로 재사용하지 않는다. 새 조회는 **UTC 시작 기록 → 읽기 전용 `git ls-remote --heads origin main` → 해당 정확 HEAD의 트리와 501경로/존재 파일 blob·바이트 조회 → HEAD 재조회 → UTC 종료 기록** 순서로 한다. `ObservedAtUtc`는 종료 시각으로 채우고 별도 `StartedAtUtc`/`EndedAtUtc`를 남긴다. 두 HEAD가 다르거나 시각·트리·blob·바이트를 한 HEAD에 결속할 수 없으면 원격 상태는 미입증이며 파일별 포함 후보 판정을 멈춘다. 로컬 `.git` 객체 생성·fetch·자격증명 변경·원격 쓰기는 허용하지 않는다. 정확 HEAD의 트리/파일 바이트를 읽기 전용 수단으로 얻을 수 없으면 성공으로 대체하지 않는다.

새 HEAD의 **501개 정확 경로 전체**를 원격 트리에서 다시 분류한다. 기존 7개가 그대로라면 일곱 blob의 식별자·SHA-256·로컬 현재 SHA와 비교하고, 두 충돌 경로 `artifacts/c3-required-decision-expected-rows.json` 및 `qa/tools/Test-QaCatalog.ps1`도 실제 바이트 차이와 처리 미결정을 기록한다. 개수나 어느 바이트라도 v1과 달라지면 7/494·동일5/상이2를 복사하지 않고 501경로의 존재/부재/지문·충돌을 **전수 재계산**한다. 원격 시각은 UTC 시작≤종료의 정확 값이어야 하며 `null`, 로컬 파일 시각, 문서 작성일 또는 과거 `4d864…` 원장 시각으로 대신하지 않는다. 게시 직전에는 이 v2 조회도 다시 확인해야 한다.

로컬 기준도 조사 시작·종료 UTC와 정확 501 `Path/ForwardBaselineSha256`의 전후 실물 SHA를 다시 묶어 501/501 일치·경로 고유·누락0·변경0을 검사한다. 원본 489/61/428/현재직접60, C4 신규12, 기존 변경10의 전이14·열린14·닫힌0은 v1과 같아야 한다. 불일치하면 자동 기준 갱신이나 일부 행 보정 없이 중단하고 아스트라에 보고한다. `PublicationApproved=false`, 모든 포함 미결정, `CleanCloneExecuted=false`, `UnityExecutedByThisTask=false`, `WholeAccepted=false`를 유지한다.

## 정확 출력과 한계

승인된 테라의 새 출력 경로는 `artifacts/c4-initial-publication-forward-provenance-v2.json`, `artifacts/c4-initial-publication-forward-provenance-v2.md`, `docs/verification/2026-10-01-initial-code-publication-forward-provenance-v2-terra-report.md` 세 경로다. 별도 루나 독립 결과는 `docs/verification/2026-10-01-initial-code-publication-forward-provenance-v2-luna-result-review.md` 한 경로다. 네 경로와 이 계약은 승인 전에 부재했고 v19 입력1084·source193 밖이었다. v1 원장·보고·검토와 모든 기존 증거는 불변이다. v2는 바뀐 세 행의 참조·역사 문맥과 전체 원격 501 재대조를 각각 명확한 차이표로 기록한다. 루나는 여덟 문서의 실물 SHA·줄, 세 역사 참조의 분리, UTC/HEAD/트리/blob/501 재현 및 전후 지문·미승인 상태를 독립 검수한다. 아스트라만 한정 증거 수용을 판단한다.

이번 승인 계약은 새 기준 출처 **조사의 P1 보정**만 다룬다. 파일별 포함·제외 목록, 과거 14전이 폐쇄, 최초 코드 게시, 깨끗한 복제, Unity/빌드/Q0 실행, 공동 전체 수용을 허용하지 않는다. 기존 소스·메타·자산·설정·QA·Git/원격을 수정하지 않는다.
