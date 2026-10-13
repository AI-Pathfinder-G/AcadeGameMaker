# C4 편집 모드 처분·위치 포인터 복구 설계 재검토

- 대상 초안: `docs/specs/work-contracts/2026-09-30-c4-editmode-disposal-pointer-recovery-draft.md`
- 최종 검토 SHA-256: `5850DE350B6B90501BDDCCBFE48CCCAB72FD9C17E7724A157D5BAFFF3D982063`
- 판정: P0 0건, P1 0건. 초안은 Draft이며 구현·재실행 승인이 아니다.
- 범위: 설계 문구 및 현재 소스 경로 정적 대조. 코드 수정·컴파일·Unity 실행 없음.

최신 초안은 일반 `CloseActions` 1310행을 변경 대상에서 제외하고 C2 처분 소유 경로 445/467/491/516/517행만 허용한다. 따라서 앞선 검토에서 제기한 편집 모드 처분 범위 확대 우려는 해소됐다. 445/467/516/517은 `TryBeginOriginalDispose` 성공 뒤 exact original pair의 실제 객체를 처분한다. 491은 다른 경로이므로 구현 검수에서 `TryBeginRestartRecoveryCandidateDispose` 성공과 반환된 동일 `recoveryCandidate` 참조를 유지하는지 별도 확인해야 한다. 그 외에는 시도 원장, map 비활성화, callback 해제, terminal 폐쇄·재진입 거절을 보존하도록 제한되어 있다.

R4의 57건은 EditMode에서 생성 wrapper의 `Dispose()`가 `UnityEngine.Object.Destroy(asset)`를 호출해 실제 Unity 오류 로그를 냈다. 편집 모드에서만 정확히 소유한 `GameInputActions.asset`을 원래 Unity 스레드에서 동기 해제하고, 재생 모드와 빌드에서는 기존 `actions.Dispose()`를 유지하는 제안은 이 실패 원인에 한정된다. 생성 파일 수정, 오류 로그 억제, worker의 Unity 호출은 허용하지 않는다. 구현 검수에서는 해제 대상 asset 참조·실제 해제 여부와 각 원래 operation wrapper가 한 번만 실행되는지 확인해야 한다.

R4 나머지 77건의 원인은 fixture 위치 탐색의 중복 매치로 설명된다. 48건은 실제 파일 읽기 문장이 있는 `ObserveActualBarrier` 오버로드를 고르는 문제이고, 29건은 시험 선언과 본문 인용을 구분하지 않은 문제다. 최신 초안의 정확 선언/메서드 범위/실제 조회 문장 선택과 중복 거절은 이 오류를 숨기지 않고 해결하는 방향이다. 현재 fixture의 1201행 중복 assertion과 1206행 검사 위치는 보고된 원문과 맞으므로 별도 디버그 심볼 불일치 문제는 남지 않는다.

동결 계획은 Q0 pin을 새 경로에 보존하면서 Q0 및 Strict 감사의 정확 결속만 갱신한다. Strict 시험 지문은 Hub 선택 v2에 반영하고, 자기 manifest를 읽는 Edit fixture는 v9 결속으로 갱신한다. Edit 선택 v4, 188행 자료 v4, 입력 목록 v9, source manifest v9 및 queue v7의 새 SHA 결속도 초안에 반영됐다. 변경하지 않는 Play 선택과 나머지 실행 순서는 보존하며 과거 실패 증거는 수정·삭제하지 않는다. 새 산출물의 실제 바이트·선택·행·출력 경로와 491 recovery gate는 구현 뒤 별도 정적 검수 대상이다.

이 판정은 승인이나 실제 실행 성공을 의미하지 않는다. 다음 단계에서 허용된 변경만 구현한 뒤 새 source/selection/rows/manifest/input/plan 지문을 독립 검수하고 별도 실행 배분을 받아야 한다.
