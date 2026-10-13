# Q0 클린 복제 준비 독립 검수

Approved 계약 `docs/specs/work-contracts/2026-10-02-q0-cleanclone-preparation-independent-review.md`의 REQ-PLAT-061/062, AC-PLAT-059/060 준비·거절 범위를 검수했다. 원장 JSON SHA-256 `4B2C0F3D0B5D9F0F5465AEB22324792566A2F3D1CA7877C5FD94C9AEBAB69342`, 테라 보고 `ABCC9011AE8EF7BA40306CE3F50C4CDB9AF65356A1BBEB79E2608F5F1EEFAA66`가 계약값과 일치한다.

## 판정

- **P0: 0. 준비·실제 Git 보조의 격리 범위: 독립 대조 충족.** 준비 저장소의 selftest 밖 파일 16개와 원본/준비 지문, 다섯 QA 후속 도구·세 helper 지문은 원장과 현재 파일이 모두 일치했다. 보호·원문·기존 결과를 수정하지 않았다.
- 네 C#의 `원본 raw → LF534 전임 → 준비 의미본` SHA 및 원장 기록은 모두 일치했다. 독립 계산 추가/삭제 행도 기록과 일치하고 네 파일 모두 원본 행 삭제 0이다. 각 `.meta` SHA와 GUID도 일치한다.

| C# 파일 | 원본 raw | LF534 전임 | 준비 의미본 | 추가/삭제 |
|---|---|---|---|---|
| `HubUiOnlyQ0ScopeAuditEditModeTests.cs` | `20F5E1C9…3812567` | `AE3FBA20…720554` | `A5B0370F…7A2998` | 30/0 |
| `C4ExecutionBridgeStrictAuditTests.cs` | `6EEFEA85…DFCC713` | 동일 | `EEF28316…B8E6AE` | 31/0 |
| `C4ActualExecutionEditFixtureV1.cs` | `DEDF385E…15A0668` | `C92F2E1F…965BBC` | `0FF1917F…542EECBC` | 28/0 |
| `C4ActualExecutionPlayFixtureV1.cs` | `2237DACC…DA4B7F` | `47CBC589…FE4966` | `E7242251…F816F070` | 28/0 |

- 검증기 helper는 외부 고정 권위·공개키 미발급 사유로 `Verified=false`, 종료 2를 반환하는 기록과 맞는다. 다섯 QA 후속 스크립트 모두 이 검증기를 먼저 불러 권위 거절에서 종료하도록 첫 실행 흐름을 둔다. 기존 QA 시험 결과도 다섯 모두 종료 1·미발급 거절이며, 새로 실행하지 않았다. C# 본문은 원본을 보존하고 현재 문맥 관문을 추가했으며, 원장에 기록된 Roslyn/PowerShell 구문 검사 오류 0은 Unity 컴파일 증거로 취급하지 않았다.
- 저장소 보조와 selftest helper를 새 격리 `output/q0-preparation-luna-r1`에 복사해 실제 로컬 Git만 한 번 재현했다. 새 실행 루트는 `output/q0-preparation-luna-r1/selftest-repositories/run-a793578024654878bed97e8ea51c555b`; 정상 상태와 unstaged/staged/deleted/renamed/untracked, HEAD/tree/tracked 집합/파일 SHA/meta GUID 불일치, junction 이탈의 **13/13**이 통과했다. 주입된 Git 결과값은 없고 원래 `.git`이나 준비 저장소를 대상으로 Git을 실행하지 않았다.

**열린 P1:** 미발급 상태의 음성 경계만 검증됐다. 실제 Unity/C# 양성 경로와 역사 엄격 비교, 9개 선택·188/377행 판정, 실제 권위/계획 서명, 전체 회귀·클린 복제·게시 수용은 미수행·미수용이다. 결과 플래그 `DependencyClosure`, `LunaReviewed`, `WholeAccepted`, `PublicationApproved`는 false로 유지한다.
