# C3 R9 초기 선택 보정 구현 독립 정적 검토

기준 승인 `docs/approvals/2026-09-29-c3-play-initial-selection-preparation-approval.md` SHA-256 `5EBB39A6347B6E30C610F0790E05D27591AAFEC6D6572D14D2020108730A2BA3`, R9 소스 manifest SHA-256 `AD3095ADEF0C880F48F6DEA4EB2E061C1A4179D1C9A2459427F8B2CD3A1189F3`, 구현 증거 `artifacts/c3-r9-initial-selection-terra-evidence.json` SHA-256 `FFE0F60AEF19E596F78E7DCDAA1935702B50B2F4FEFDA633264FC7F9304F4B8B`를 대조했다. Play 시험 소스 SHA-256은 `6A3FB8BE37AE8644E446AA04FAE63D4BF75469C4DA704D253D146E715D086FEF`다.

**P0 0, P1 0.** 승인된 초기 선택 경계 변경과 일치하며 정확 읽기 요건도 충족한다. R8 14개 파일 manifest와 R9 14개 현재 경로를 독립 재해시해 모두 일치했다. R8 대비 변경 경로는 Play Owner 시험 한 개뿐이고 나머지 13개 지문은 동일하다. Edit 원장은 변경되지 않았다. 변경된 `SelectInitialNewGame`은 기존 실제 선택 호출 앞에서만 중립 `Publish()`를 한 번 더 수행하며, 비회복 Down 직후 기존 실제 `CurrentUiFrame` 조회로 NavigateChanged와 음수 Navigate Y를 assertion한다. 실제 Enter Submit 및 Q-B RequestReady 뒤 원래 요청 항목을 NewGame으로 assertion한다. 승인대로 Rearm 뒤 successor의 첫 Enter 이전에는 추가 `Publish()`가 없다.

요청 항목 읽기는 nullable `_request` 필드의 실제 `Nullable<HubMenuIntentRequestV1>` 타입을 확인하고, 비어 있지 않은 값을 정확한 boxed request struct로 결속한 뒤 정확 선언형의 `_item: HubMenuItemV1`을 읽는다. 기존 `SnapshotField`는 대상 실형·선언형·정확 필드 타입을 검사한다. 실제 권한은 여전히 `Take`/`Accept` 정상 API로 획득하며 이 읽기는 사적 슬롯을 쓰거나 새 권한을 만들지 않는다. 증거가 주장한 변경 소스 1개 및 이전된 기존 시험 0, 새 시험 선언 0도 소스와 일치한다.

R8/R9 기대 행렬은 각각 377행이며 행 내용 비교는 차이 0, 91개 고유 행렬 case다. 집중 선택은 Edit 240/Play 15이고 선택 분할은 Edit 91+149, Play probe 2+remaining 13으로 정확 이름 합집합·중복/누락 0이다. 분할 계획은 실행하지 않은 원장으로 표시되어 있으며, R8 분할 생성기와 비교 시 R9 경로 치환 외에는 내용 차이가 없고 말미 공백 한 줄 차이만 있다. 네 실행 각각의 command 길이는 14,453/24,168/751/2,433자로 Windows 한도 미만이다. 원본 R8 계획과 원장도 보존된다.

실제 probe 두 건은 다음 실행 단계이며 아직 실행되지 않았다. 해당 실행이 성공해도 공유 `SelectInitialNewGame` helper 영향 검증을 위해 나머지 13개 Play 시험이 필요하다. 이 정적 검토는 최초 요청이 실제 NewGame인지 확인하는 보정까지의 소스 근거이며 제품 원인 확정, 실제 take 성공, successor AC006 통과, 240/15 실제 실행 완료 또는 C3/C4 수용을 주장하지 않는다. Unity·컴파일 실행은 없었다.
