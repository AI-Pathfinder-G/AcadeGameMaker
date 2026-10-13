# Unity 참조 후보 19행 객체 조회 테라 보고

Approved REQ-PLAT-071/072·AC-PLAT-069/070 범위에서 격리 Unity 6000.6.0f1을 한 번 실행했다. e GUID는 실제 Library/unity default resources, f GUID는 Resources/unity_builtin_extra 경로로 조회됐다. 19행 모두 원문 GUID·long fileID와 정확 일치하는 객체가 하나씩 열거됐다. Hub 장면의 skybox와 lightingDataAsset 공개 필드 2행도 직접 일치했다. 나머지 17행은 객체 튜플만 일치하며 원문 필드의 실제 사용은 미입증이다. 이름 또는 형식 유사성으로 결론을 대체하지 않았다.

Bootstrap·Hub 장면 조회는 수행됐고 저장 API는 호출하지 않았다. 실행 전 열린 장면이 0개여서 RestoreSceneManagerSetup은 유효한 활성 장면이 없다는 오류로 실패했다. 따라서 장면 구성 복원을 성공으로 기록하지 않는다. Unity 종료코드는 0이고, 원문534·LF534·LF505·실행본505·캐시·설정·API 문서·기존 증거 전후 지문 일치: True. 정확 행과 원시 관측, 오류, UTC, 소스·메타·로그 SHA는 JSON에 보존했다. 실패 뒤 패치·재실행은 없다.

DependencyClosure, PublicationApproved, RendererGuid7P1Closed, WholeAccepted, LunaReviewed는 모두 false이다. 루나 독립 검수 전 자체 수용하지 않는다.
