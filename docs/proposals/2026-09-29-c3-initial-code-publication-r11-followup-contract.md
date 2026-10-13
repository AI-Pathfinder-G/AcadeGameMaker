# C3 최초 코드 게시 준비의 R11 보완 계약 제안

2026-09-29. 솔, 실제 모델 `gpt-6-sol`. 읽기 전용 비교와 다음 작업 계약의 제안이다. 새 승인·구현·게시를 수행하지 않았으며 기존 소스·자산·설정·원장·증거를 수정하지 않았다. Unity·컴파일·네트워크 실행도 없다. 기존 GPT-5.6 참여와 수용 기록은 실제 역사 그대로 보존한다.

추적은 `REQ-M5D7QC3-001..007`, `AC-M5D7QC3-009/010`의 게시·재현 기반이며, 실행 증거의 제한 수용은 해당 승인 문서의 `REQ-M5D7QC3-001/005/007`, `AC-M5D7QC3-001/005/006/009/010` 범위만 따른다. 제품 요구나 C4 권한을 추가하지 않는다.

## 독립 비교 결과

기존 [최초 기반 제안](2026-09-29-c3-initial-code-publication-baseline-contract.md)과 후보 489개 원장, 바이트 근거 r2는 보존했다. 새 비교 자료는 `artifacts/c3-initial-code-publication-r11-supplement.json`, SHA-256 `B2FEAB2DB9CBC1E0828B792AB0B22B0345B08A6F8A2AD344822C621484A53D2E`다. 이 자료에 정확한 이전/현재 SHA, R11 14파일 비교, 기존 61개 경로 및 63개 근거 문서 지문, 최신 QA 자료와 도구 입력을 분리했다.

489개를 실제 재해시한 결과 485개는 동일하고 아래 4개만 달라졌다. 누락은 0개다. 정확한 두 SHA는 새 JSON에 기록했다.

| 달라진 경로 | 현재 상태와 허용 근거 |
| --- | --- |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs` | R11 지문 `A0A1928CE83A22D324F785476B38DCDE595B71E53122139F10535C5B3AF6F9D6`. R6 보정 승인 21행 및 Submit 이전 승인 8~10행의 시험·매핑 범위. 최종 Edit 240개 실행 수용은 아직 남음 |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs` | R11 지문 `2FA406515CEB7C885A5F41EC6438ACC85123714AB00302B520A58B36066C9E23`. 이전 승인과 최초 중립 소비 승인 8행, 취소 타입 승인 8행의 후속. 최종 Play 15개에 한정한 실제 수용 |
| `artifacts/c3-build-required-decision-expected-rows.ps1` | R6 보정 승인 25~26행의 행별 사례 결속 허용. 바이트 차이가 있다는 관찰을 전체 승인으로 치환하지 않음 |
| `artifacts/c3-required-decision-expected-rows.json` | R6 보정 승인 29~37행의 별도 매핑 원장 범위. 기존 원장과 후속 377행/91사례 연결을 게시 전 정확 대조할 대상 |

근거 문서는 [R6 보정 승인](../approvals/2026-09-29-c3-r6-focused-test-correction-approval.md), [Submit 시험 이전 승인](../approvals/2026-09-29-c3-submit-playmode-transfer-approval.md), [최초 중립 소비 승인](../approvals/2026-09-29-c3-initial-neutral-cursor-consumption-approval.md), [취소 관찰 타입 승인](../approvals/2026-09-29-c3-cancel-snapshot-type-correction-approval.md)이다. 이들은 각 제한 변경의 구현 권한이며 최초 기반 파일의 게시 권한은 아니다.

최종 R11 원장 `0A36C9E7CCF3EBBF477B96C1B742344021D642586AD906042E48C755258571EB`의 14개 파일은 모두 실제 현재 바이트와 일치한다. 그중 최초 후보와 달라진 것은 위 두 시험뿐이며, 제품 런타임·meta는 동일하다. 기존 바이트 근거 r2의 61개 경로와 63개 근거 문서 지문도 모두 동일하다. 기존 60개 한정 역사/규범 근거와 1개 R6 meta 정적 근거를 R11 전체 수용으로 승격하지 않는다. 원래 428개 직접 근거 공백은 그대로다. Q-A 과거 검수의 manifest 지문과 현재 manifest 지문이 다른 기존 제외 항목도 보존했다.

원래 489개에는 최신 R11 선택/매핑 이름이 없다. 새 JSON은 아래 6개를 **추가 검토 대상**으로 기록하며 후보 수를 임의로 늘리지 않는다.

- `artifacts/c3-r11-editmode-focused-selection.json`: 240개
- `artifacts/c3-r11-playmode-focused-selection.json`: 15개
- `artifacts/c3-r11-required-decision-expected-rows.json`: 377행/91사례
- `artifacts/c3-verify-decision-rows.ps1`: R6 승인 27~28행의 행별 대조 도구
- `artifacts/c3-r11-edit-matrix-selection.json`: 91개 분할 선택
- `artifacts/c3-r11-edit-remaining-selection.json`: 나머지 149개 선택

이 목록은 최초 원장의 R5 선택 155/14개를 현재 선택으로 사용하는 혼동을 막기 위한 것이다. 이전 선택은 역사로 유지한다. 새 선택의 정적 이름 수와 실제 통과 수는 다르며, 현재 [R11 제한 수용](../approvals/2026-09-29-c3-r11-play-focused-partial-acceptance.md) 6~10행은 Play 15/15만 수용한다. Edit 240개, 377행/91사례, 필수 562/51/610 및 최종 독립 검수는 남았다. AC007/008은 미검증, C4는 Review다.

## 여전히 열린 정확한 차단 묶음

[루나 최초 게시 검수](../verification/2026-09-29-c3-initial-code-publication-luna-review.md) 17~25행의 P0=0/P1=5를 따른다. [바이트 원장 루나 검수](../verification/2026-09-29-c3-initial-code-publication-byte-evidence-luna-review.md) 23~25행에서 닫힌 것은 r2의 포인터 하나뿐이다.

| 차단 | R11 이후에도 필요한 근거 |
| --- | --- |
| 파일별 승인, 19행 | 300개 문구 후보를 바이트 승인으로 추정하지 않음. 189개 출처 미입증 및 전체 428개 직접 근거 공백을 정확 파일별로 해결하거나 격리. 변경 4개도 현재 SHA와 제한 변경 이력의 정확 연결 필요 |
| Q0 첫 게시 전제, 20행 | 실제 Git 변경·미추적 집합을 요구하는 기존 시험과 깨끗한 최초 게시 상태의 관계를 별도 계약으로 규정. 시험·pin·역사·선택을 유지. 수정이 필요하면 아스트라의 별도 승인 전 구현하지 않음 |
| 도구 입력 7개, 21행 | 현재 로컬 입력 지문과 기존 캐시 존재를 새 JSON에 기록. 캐시 `46b14e97...`에는 schema/required-ac/catalog 3개가 있고 나머지 4개는 없음. 이는 최신 원격 부재나 재현 실패 판정이 아님. 실제 최신 기준의 존재/바이트 또는 승인된 포함·재생성 절차가 필요 |
| Renderer2D GUID 7개, 22행 | 정확 엔진·패키지 출처와 실제 import 진단이 아직 없음. 메타 검색 미해석을 결손이나 실패로 단정하지 않음. 현재 읽기 범위에서 자산 변경·진단 실행하지 않음 |
| 설정과 기반 동결, 23행 | 10개 설정의 파일별 허용 목록·승인·환경 값·바이트를 확정. 원격 기준과 줄바꿈/meta/Q0 pin은 실제 게시 직전 재대조 |

상위가 보고한 문서 PR 병합 `4d864484f75939dcf9c93c055d296613cf1ba1dd` 및 위키 `c8cf42aad2ec5fdb14b27373c0ff8ea563a16161`은 문서 게시다. 이번 로컬 `origin/main` 캐시는 여전히 `46b14e97...`이며 네트워크 재조회가 없었다. 문서 병합을 게임 기반이나 코드 게시 승인으로 해석하지 않는다.

## 다음 최소 작업 계약 초안

첫 작업은 기존 승인 범위 안의 **읽기 전용 파일별 근거 폐쇄**다. 원래 489개를 보존하고, 변경 4개·위 6개 QA 후속·도구 입력 7개·설정 10개에만 우선순위를 둔다. 정확 경로/현재 SHA/meta GUID/조립/실제 Approved 허용 문맥/REQ·AC/이전 지문/후속 승인 줄을 분리한다. 이미 증명된 61개 연결을 그룹 접두사로 확대하지 않는다. 원문이 파일별 현재 바이트를 보장하지 않으면 미입증으로 남긴다. 모든 미입증 항목에 승인 요청 또는 게시 제외의 결정을 필요 조건으로 적으며, 현재 작성자가 승인하지 않는다.

두 번째 산출물은 Q0 최초 게시 전제의 좁은 계약 초안이다. 기존 실제 Git delta 시험의 역사적 의미와 깨끗한 최초 기반에서 무엇을 재현할지 분리하고, pin 및 원본 시험·선택을 그대로 보존한다. 새로운 후속 시험이 필요하면 정확 허용 경로·요구·이름·역사 대조를 아스트라에게 제출한다. 현재 시험을 무단 고치거나 기존 수용을 재현했다고 주장하지 않는다.

승인된 C3 검증 작업은 별도 진행할 수 있다. 최종 동일 소스에서 아직 남은 Edit 240개(행렬 91개+나머지 149개), 내부 377행의 정확 사례별 판정, 필수 562/51/610과 독립 검수를 완료해야 한다. 이 제안에서는 실행하지 않았다. 통과한 Play 15개를 소스·조건 변화 없이 반복하는 작업을 이 준비 계약에 자동 추가하지 않는다. 실제 검증 통과도 최초 기반의 파일별 게시 승인이나 깨끗한 복제 증거를 대신하지 않는다.

최신 원격 기준 확인, GUID의 실제 import 진단, 설정의 최종 허용 목록과 깨끗한 복제 재현은 다음 승인 검토의 별도 조건이다. 이들을 닫기 전에는 초기 기반 계약을 Approved로 선언하거나 코드 PR을 게시하지 않는다. 조립/friend/API·제품 동작·C1/C2 실행·전체 미추적 자료 포함으로 범위를 넓히지 않는다.
