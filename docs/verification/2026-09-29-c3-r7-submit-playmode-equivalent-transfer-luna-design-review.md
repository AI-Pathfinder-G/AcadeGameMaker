# C3 R7 Submit 동등 PlayMode 이전 설계 검토

검토 제안 SHA-256: `CB0AAC702505ED4ECC68DC988852C8E27016FF945B70FF7606600350D28ED3A0`.

**P0 0, P1 0.** 승인 전 설계 검토 기준에서 Edit 전용 환경 실패를 성공으로 가장하지 않고, 같은 AC006의 실제 입력 경로를 정상 `InputTestFixture`가 이미 사용하는 PlayMode 사례에 옮기는 방향은 타당하다. 수정은 별도 구현 승인 후 진행해야 하며, 현 제안만으로 실제 실행 통과나 이전 완료를 주장할 수 없다.

현재 Edit 시험은 prompt 경로의 정상 primary default, 초기 launch 후 previous default를 준비하고 최초 intent를 만든 뒤 실제 request take/Accept/Cancel/Rearm를 수행한다. 기존 Play fixture는 `Create(false)`에서 동일한 primary default를 만들고 정상 launch 이후 previous default를 추가하며, `SelectInitialNewGame()`에서 실제 Down/Enter 입력으로 최초 Q-A 의도와 Q-B 요청을 만든다. 양쪽 모두 회복 notice나 합성 request를 요구하지 않는 구성임을 확인했다. Play 시험 클래스는 기존 `InputTestFixture`를 상속하며, 기존 Play `Publish`는 정상 키 상태 이벤트→`InputSystem.Update()`→`StepForTests()` 순서다.

이전할 AC006 검증도 기존 Play `AC006_ImmediateReadyFactoryDiscardsActualSubmitBeforeNewTake`와 같은 핵심 순서를 공유한다: 실제 take/Accept로 decision을 얻고 Cancel/Rearm, successor cursor Ready 및 새 publication 전 BaselinePending, 첫 실제 Enter Submit, presenter 후 BaselinePending 해제와 retained intent 없음 및 다음 take 거절, 그 뒤 release와 두 번째 Enter로 새 실제 take/decision을 얻는다. 제안이 유지하도록 요구한 원래 `_cursor`/intent/taken history 동일성, Q-B 상태, 실제 `DecisionRequired` 결과 및 새 capability 참조 구분은 기존 Edit 검증의 손실을 막는 구체적 목록이다. 첫 true Submit 이전 빈 프레임을 만들지 않는 조건도 명확하다. 첫 입력 취소·무시, 반환 핸들 위조, 직접 callback·사적 lifecycle 호출을 허용하지 않는다.

잠정 Edit 240/Play 15는 테스트 이름을 정확 재집계한 실행 전 가설로만 남긴다. 승인 범위도 Owner 시험 두 파일과 선택 원장으로 제한되어 runtime/asmdef/API/settings/asset 변경을 요구하지 않는다. 기존 377 결정 행렬은 이동 범위 밖으로 보존된다.

남은 한계는 동등한 제품 상태여도 Editor와 Play 입력 런타임이 다르다는 점이다. 이는 바로 이번 관측에서 확인된 Editor 갱신 종류 문제를 다루기 위한 시험 위치 선택이며, Play의 성공은 아직 관찰하지 않았다. 구현 뒤에는 첫 실제 Enter가 실제로 발행되고 폐기되며 다음 입력만 정상 선택되는지, 파일·입력 상태 및 정확 선택 이름·종료 결과가 실제 XML에서 확인되어야 한다. 이번 검토는 정적 대조만 수행했고 구현·Unity·컴파일 실행은 없다.
