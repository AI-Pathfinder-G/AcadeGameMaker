# 테라 원격 객체 지문 보완 보고

승인 계약 `docs/specs/work-contracts/2026-10-02-content-survey-remote-object-correction.md` (SHA-256 `3D71567C18592FEAD74BC3D6703ADD07749AED1087B4CAE31762AE17C752061A`), `REQ-PLAT-018/019`, `AC-PLAT-016/017`. 입력 조사 v1 SHA-256 `AE4DE92037B0A345ADB9AA42710112C07EB54A9BD512C1A6309444EFB02A4F4A`는 보존했다.

새 원격 관측 UTC `2026-10-02T02:42:55.736Z`부터 `2026-10-02T02:44:22.126Z`까지. GitHub 연결 도구의 읽기 요청으로 기본 가지 `main`의 HEAD `e95bb5169ad2ed08106bd342915d4637a1241263`를 시작·끝에 확인했다. Git 커밋 객체의 `tree.sha`와 정확 재귀 트리 응답의 `sha`는 모두 `93c4e1fabbaddabd4c237e5964db8dd735965f95`이며, 전체 1042항목·잘림 없음이다. 커밋 ID, 트리 ID, blob ID는 JSON에서 별도 필드다.

501 정확 집합에서 원격 존재 7·부재 494를 다시 계산했다. 존재 7개의 새 blob 응답은 Base64로 보관했고 복원한 실제 크기·SHA-256·Git blob SHA-1이 트리의 blob 객체와 일치했다. 로컬 원문501도 시작·끝 501/501 동일하다.

원문 바이트 충돌: `artifacts/c3-required-decision-expected-rows.json`, `qa/tools/Test-QaCatalog.ps1` (2경로). Git 정규화 바이트 충돌: `qa/tools/Test-QaCatalog.ps1` (1경로). 원격 조건이 독립 수용됐다는 판정이나 파일별 게시 허용은 발급하지 않는다.

조사 v1은 `TreeSha`에 커밋 ID를 기록한 역사 결함이 있다. 이번 정확 트리 ID는 새 시각의 관측이며 v1을 소급 수정하지 않는다. 전체 501 내용·간접 의존 재검수, 역사 전이, 깨끗한 복제와 루나 독립 검수는 남아 있다.

자체 결과: `AC-PLAT-016/017`의 새 객체 결속 사실은 충족, 독립 수용은 미완료. `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`, `CleanCloneExecuted`, `RemoteConditionAccepted` 모두 거짓.
