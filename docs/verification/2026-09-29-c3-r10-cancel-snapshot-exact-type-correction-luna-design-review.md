# C3 R10 취소 스냅샷 정확 타입 보정안 독립 검토

검토 제안 SHA-256 `5919567813731A1C17AF53B5D6E6C32C0AE74C5BC61C9C3114C618573FA125BC`를 읽고 현재 Play 시험 소스와 실제 Movement 선언을 읽기 전용으로 대조했다. 현재 시험 SHA-256은 `B573AA593F8624C4A1E6F86059EC02E4DC26EEB3203F8774E74828B166B7850B`이다.

**설계 검수 P0 0, P1 0.** 제안은 `CancelSnapshot`의 `_player` 타입 문자열에서 namespace만 `AcadeGameMaker.Movement.Unity`에서 `AcadeGameMaker.Movement`로 고치고 실제 assembly 이름 `AcadeGameMaker.Movement.Unity`는 유지한다. 실제 클래스 선언도 `AcadeGameMaker.Movement.PlayerMovementController`이며 assembly 메타데이터와 부합한다. 다른 네 실행 필드의 정확 타입·assembly는 그대로 두며, `_player` 필드명이나 snapshot key를 바꾸지 않는다. `Type.GetType(..., true)`와 정확 선언·필드 타입 검증을 유지하므로 느슨한 대체 조회로 실패를 숨기지 않는다.

제안은 전체 취소 snapshot 항목, 두 `(False/True)` 사례, 기존 선택·원장·180초 제한 및 원본 실패 증거의 보존을 명시한다. 원래 실패는 snapshot helper가 취소 전에 예외를 낸 것이므로 이 보정만으로 Cancel 동작이 검증됐다고 간주할 수 없다. 보정 후 두 사례의 실제 Unity 결과에서 snapshot 생성, 같은 실제 권한으로 취소, 전체 전후 snapshot 일치를 확인해야 한다.

따라서 이 설계안에는 구현 전 차단 결함이 없다. 그러나 실행 수용은 아직 차단 상태다. 본 검토는 코드·시험·조립을 수정하거나 컴파일/Unity를 실행하지 않았고, 승인이나 실제 취소 성공을 주장하지 않는다.
