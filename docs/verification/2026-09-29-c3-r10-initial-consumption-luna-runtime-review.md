# C3 R10 중립 프레임 소비 구현 독립 정적 검토

대조 지문: 승인 `docs/approvals/2026-09-29-c3-initial-neutral-cursor-consumption-approval.md` SHA-256 `C7453AC12551432E167E32B59D1886FDC3F0A357B4E1C89CA332DECC4DBD67CD`; Play 소스 `B573AA593F8624C4A1E6F86059EC02E4DC26EEB3203F8774E74828B166B7850B`; 14파일 source manifest `796057D37EF6331CB5E43B7D1DC9CBA7F21F82848B24E8FC259C27A7DC21E6D0`; Terra 증거 `FCE4345DD8AE00AD3F9E4B0153BEE81804F0969E635430BFABE40E26681F6736`.

**P0 0, P1 0.** 승인된 초기 중립 발행 바로 뒤에 `PresenterFixed()` 한 번 추가된 구현이다. 원래 R9 파일 백업에서 현재 Play 소스의 해당 줄만 제거하면 텍스트가 정확히 R9 원본과 일치한다. R9와 R10 manifest 간 경로별 차이는 승인된 Play Owner 시험 파일 한 개뿐이다. R10 manifest의 14개 실제 파일을 재해시한 결과 14/14 지문이 일치하고 나머지 13개 source/meta 경로는 그대로다. 기존 R8/R9 history 보존과 evidence의 changed path 주장도 일치한다.

정상 선택 helper에서 새 흐름은 중립 `Publish()`→`PresenterFixed()`→원래 회복/비회복 선택이다. 비회복 Down의 실제 NavigateChanged/음수 Y assertion, 이후 실제 Enter Submit, RequestReady와 NewGame request 검사 및 기존 release가 유지된다. successor AC006에서는 Rearm 뒤 새 `Publish()`가 삽입되지 않고 첫 Enter로 계속 시작한다. 이 변경은 커서가 이미 발행된 초기 중립 프레임을 정상 연속 소비하도록 할 뿐 새 입력·권한·상태를 직접 제조하지 않는다. 제품/runtime/settings/assembly/API 변경은 없다.

R10 기대 원장은 R9와 같은 377행이며 전체 행 내용 비교 차이 0, 91 고유 matrix case다. 기존 선택 240 Edit/15 Play가 보존된다. 새 네 분할은 Edit 91+149, Play 2+13으로 정확 anchored selector, 부분별 예상 이름 일치, 양 플랫폼별 합집합 동일, 중복/누락 0이며 각 SourceFiles 배열은 해당 R10 원본 선택과 같다. 계획은 `ExecutionPerformed=false`, `AcceptanceGranted=false`로 표시됐다. R10 분할 생성기는 R9 버전 경로 치환 외 내용 차이가 없으며 추가 말미 공백 행 하나를 제외하면 같다. 분할 인자 길이 14,455/24,170/753/2,435자로 모두 Windows 한계 아래다.

Unity나 컴파일을 실행하지 않았다. 따라서 R10이 R9의 Closed/take 실패를 해결했는지, 실제 request가 NewGame인지, 두 probe와 remaining 13이 통과했는지는 미확인이다. 후속 실행은 실제 XML·native 종료·정확 이름·입력 전후지문 및 R10 source manifest와 대조해야 한다. 이 정적 검토는 실행 수용이나 C3/C4 수용이 아니다.
