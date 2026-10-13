# C4 편집 모드 처분·위치 포인터 복구 설계 검토

- 검토 대상: `docs/specs/work-contracts/2026-09-30-c4-editmode-disposal-pointer-recovery-draft.md`
- 검토 대상 SHA-256: `93701265A2DA91C51D5507088884E9C0A9D507CB98BA0E910A599291C604F232`
- 범위: 설계 정적 검토. 구현·컴파일·Unity 재실행은 수행하지 않았다.
- 판정: P0 0건, P1 1건. 초안은 아직 Draft이므로 실행 승인으로 볼 수 없다.

R4 첫 집중 실행은 139개 중 5개 통과, 134개 실패였다. 실패의 57개는 실제 `GameInputActions.Dispose()`가 `UnityEngine.Object.Destroy(asset)`를 불러 편집 모드의 Unity 오류 로그를 낸 사례다. 이 로그를 예상값으로 등록하거나 무시하지 않고, 편집 모드에서 정확히 소유한 `GameInputActions.asset`을 동기 해제하며 실제 해제를 관측하고, 재생 모드·빌드에서는 기존 `actions.Dispose()`를 유지한다는 제한은 원인에 맞다. `GameInputActions.cs`는 JSON으로 임시 `InputActionAsset`을 만드는 생성 코드이므로 생성 파일 변경을 금지한 점도 적절하다. 맵 비활성화, 콜백 해제, 처분 시도 원장 선기록, 오류 후 terminal 폐쇄 및 재진입 거절도 보존 조건으로 남아 있다.

P1 — 편집 모드 해제 분기의 호출 대상을 더 좁혀야 한다. 현재 `InputRouter`에는 C2 처분 경로 445/467/491/516/517행 외에 일반 `CloseActions`의 1310행이 있다. 초안의 “준비 실행 종료 경로도 같은 규칙”을 일반 정리 경로까지 적용하면 C2의 원본 소유 증거가 없는 준비·legacy 종료에도 `DestroyImmediate`가 넓어질 수 있다. 편집 모드 분기는 C2 원본 소유 쌍이 확인되고 `TryBeginOriginalDispose`가 성공한 처분에만 허용한다고 명시해야 한다. 소유 증거가 없는 일반 `CloseActions`는 현 동작을 유지해야 한다. 기존 C2 작업 이름 `DisposeOld`/`DisposeNew`는 제어 사건 이름으로 보존하되, 편집 모드의 실제 효과가 생성 wrapper의 `Dispose()` 호출이 아니라 같은 wrapper의 `asset` 해제임을 시험에서 구별해야 한다.

포인터 복구의 원인 분리는 타당하다. 48개 오버로드 오류는 `ObserveActualBarrier` 두 오버로드 중 실제 파일 조회 문장이 있는 본문을 선택하지 못한 문제이고, 나머지 29개 중복 오류는 부모 선언 토큰과 본문 인용을 같은 문자열 검색으로 셌던 문제다. 제안처럼 선언행과 메서드 본문을 분리하고 대상 조회 문장의 유일성을 유지하면 실패를 숨기지 않고 해결할 수 있다. 현재 fixture의 1201행 중복 assertion과 1206행 검사 위치는 R4 지문·원문과 일치해 디버그 심볼 불일치 주장은 남지 않는다.

동결 절은 Q0 pin을 새 경로로 보존하고 Q0/Strict 감사의 정확 pin 결속만 갱신하며, Strict 시험 변경을 Hub 선택 v2에 반영한다. Edit 선택 v4, 188행 자료 v4, source manifest v9, 입력 목록 v9, queue 계획 v7의 새 결속도 일관된다. Edit fixture의 manifest 자기 결속을 v9로 올리고, Edit 선택의 시험 파일은 불변인 경우 이름·순서·SourceFiles를 재발급할 수 있게 했다. Play 선택과 과거 실패 자료는 보존한다. 새 원장과 계획의 실제 파일 SHA·출력 부재를 별도 검수하기 전에는 Unity를 재실행하지 않는 경계도 명확하다.

이 검토는 제안된 구현의 실제 동작, 새 포인터의 줄 번호, Unity 로그 부재를 입증하지 않는다. P1의 C2 소유 경계가 승인 문구에 명시된 뒤에만 구현을 검수할 수 있다.
