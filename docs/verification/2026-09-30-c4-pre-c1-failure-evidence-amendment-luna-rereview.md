# C4 C1 이전 실패 증거 초안 필드명 재검토

보존된 원문 `docs/specs/work-contracts/2026-09-30-c4-pre-c1-failure-evidence-reviewed-draft.md` SHA-256 `5AA8812873040272389EC46CBE761F30A8DCB98569685A056C8DB34AEF4995F3`과 현재 초안을 대조했다. 수정 초안 `docs/specs/work-contracts/2026-09-30-c4-pre-c1-failure-evidence-amendment.md`의 SHA-256은 `F198C7EED7DEACD07B85866221CBF86CC25C672073D2A302C9DDC6532F77B0F1`이다.

실제 텍스트 차이는 한 곳뿐이다. checkpoint 조건의 `RequiredReach=RequiredReached`를 `ExpectedReach=RequiredReached`로 바꿨다. 원문에는 구 필드가 한 번 있었고, 수정본에는 새 필드가 한 번 있으며 반대 필드는 없다. QA r2 예상 행 객체는 `ExpectedReach`를 사용하므로 수정은 스키마와 일치한다.

GenerationRule의 위치·허용값, CWT 발급·소비·fault 조건, 체크포인트 경계와 나머지 행 조건은 변경되지 않았다. 한정 재검수 판정: P0 0, P1 0. 이는 필드명 보정의 정적 확인이며 Draft 상태와 구현·Unity 실행 미승인은 그대로다.
