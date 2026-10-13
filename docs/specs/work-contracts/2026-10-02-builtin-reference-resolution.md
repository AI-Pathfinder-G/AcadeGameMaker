# Unity 내장 참조 후보 19행 객체 확인

상태: **Approved**, 2026-10-02 통합 책임자 승인. 소유 `REQ-PLAT-071/072`, `AC-PLAT-069/070`. 솔 초안 `docs/proposals/2026-10-02-builtin-reference-resolution-contract-draft.md` SHA `A8C671890AAB7BCF8B376BB8C333C57AC9E05EB85E828F6DE5EAF1F742967014`의 입력19행·실제 API·판정 조건을 채택한다. 두 GUID가 내장이라는 추측을 성공으로 쓰지 않는다.

테라 `gpt-6-sol`이 격리 `output/u505-r1`에서 새 `Assets/Editor/PublicationBuiltinReferenceProbe.cs`와 메타를 구현하고 Unity 한 번을 실행한다. 추가 허용 출력은 `artifacts/c4-builtin-reference-probe-source-v1.cs.txt`, `artifacts/c4-builtin-reference-resolution-v1.json`, 같은 접두사 실행 로그, `docs/verification/2026-10-02-builtin-reference-resolution-terra-report.md`이다. 이전 진단 소스/결과는 불변이다.

19개 출발 위치와 실제 GUID/fileID/type을 원문으로 재확인한다. 실제 GUID 경로와 객체 열거·GUID/long 식별자 대조, 두 격리 장면의 공개 skybox/lightingDataAsset 읽기를 수행한다. 장면은 저장하지 않고 격리 실행 동안만 열고 종료한다. 원래 장면 구성 복귀를 시도하고 실패는 그대로 기록한다. 직렬화·설정·캐시·패키지 변경을 허용하지 않는다. 경로 공백/null/조회 실패/모호/공개 접근 불가와 실제 유일 객체 일치를 분리한다. 이름이나 형식만 비슷한 객체로 성공 처리하지 않는다. 실제 객체 튜플이 맞아도 원문 필드의 사용 또는 렌더 소비를 대리하지 않는다.

실행 전후 원문534·LF534·LF505·실행본505와 캐시/설정·사용 API 문서·분류 원장을 SHA 대조한다. 실행 소스와 출력 지문, 실제 종료/오류/조회 시도와 성공 행 수를 기록한다. 실패 시 이 계약 내 추가 패치·재실행을 하지 않고 정확 보정 범위로 넘긴다. `DependencyClosure`, `PublicationApproved`, `RendererGuid7P1Closed`, `WholeAccepted`, `LunaReviewed`는 모두 false이다. 기존 일곱 렌더 GUID의 무영향과 Q0·전체 회귀·게시 관문은 별개이다. 자기 결과를 독립 수용하지 않는다.
