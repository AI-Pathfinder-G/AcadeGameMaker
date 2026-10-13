# C4 편집 모드 처분·포인터 복구 구현 정적 검토

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-editmode-disposal-pointer-recovery-amendment.md` SHA-256 `01D638189329EEE39C52FA663D1EF18242EB321E60875156F7204DCA1FD45791`.
- 검토 대상 `InputRouter.cs` SHA-256 `5A210D7ED80DE86C9D3092865E7F4712C00C65B84372E479018BF43956F6695F`.
- 검토 대상 `C4ActualExecutionEditFixtureV1.cs` SHA-256 `46C6A7D0054CC6B6BCE601EC30E71DF6CE86A31DBB35E47B8772CC9B7CCF5F12`.
- 판정: P0 0건, P1 0건. 정적 범위만 확인했으며 Unity·컴파일·통합 실행은 하지 않았다.

`InputRouter`에는 승인된 다섯 C2 처분점만 새 `DisposeResetOwnedActions` 도우미를 호출한다. 445/467/516/517행은 `TryBeginOriginalDispose` 성공 뒤 원본 소유쌍의 정확한 `originalNew` 또는 `originalOld`를 전달한다. 491행은 별도 `TryBeginRestartRecoveryCandidateDispose` 성공 분기 안에서 반환된 같은 `recoveryCandidate`를 전달한다. 일반 `CloseActions`의 1310행은 기존 `actions.Dispose()`를 유지한다. 따라서 편집 모드 해제 분기가 C2 외 준비·legacy 정리로 확장되지 않았다.

도우미는 null wrapper를 거절하고 `UNITY_EDITOR && !Application.isPlaying`에서 정확 wrapper의 `actions.asset`에 `DestroyImmediate`를 호출한다. 다른 구성에서는 기존 `actions.Dispose()`를 호출한다. 다섯 호출점은 기존 `DisposeOld`/`DisposeNew` control wrapper 안에 남아 있어 실제 처분 시도 계수 및 앞뒤 예외 경계가 유지된다. 기존 선행 시도 원장, 비활성화/콜백 해제, 예외 처리·terminal 폐쇄 경로도 변경되지 않았다. 이는 코드 정적 확인이며 실제 asset 해제나 Unity 오류 로그 부재를 증명하지 않는다.

fixture의 위치 탐색은 Edit 부모 선언을 `public void <현재 테스트명>(`로 시작하는 선언행에 한정하고 중복을 거절한다. fixture 쪽은 지정 메서드 선언에서 다음 내부/비공개 메서드 경계까지 검사하고 조회 문장과 일치하는 오버로드에서만 포인터를 취한다. 정확 경로, source manifest v9의 SHA 결속, 부모 본문의 `ProfileAtomicSaveServiceV1.Save(` 부재 검증이 유지된다. 현재 두 `ObserveActualBarrier` 오버로드 중 실제 파일 조회 문장이 포함된 쪽을 선택하는 구조와 맞으며, 본문에서 부모 이름을 인용한 문자열은 선언 중복으로 세지 않는다.

선택 v3의 부모 시험 파일 현재 SHA가 일치하고 선언 기대 개수는 139다. Edit/Play/Strict fixture에 내장된 기대 행은 각각 148/35/5개이며, 세 묶음을 합친 188개 행이 `artifacts/c4-required-checkpoint-rows-v3.json`의 행과 정확히 같다. Unity 실행 전 새 manifest·선택 v4·행 v4·Hub 선택 v2·입력 v9·계획 v7의 실제 바이트/지문과 출력 경로를 별도로 검수해야 한다. 과거 139개 실패 실행은 이 정적 확인으로 성공으로 바뀌지 않는다.

따라서 이 두 파일은 승인된 제한 변경 범위에서 정적 P0/P1 차이가 없다. 실제 재실행과 AC 결과는 아직 검증되지 않았다.
