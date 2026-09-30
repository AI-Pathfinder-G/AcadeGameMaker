# C4 실행 가드 소유자 형식과 실패 실행 재검증

- 상태: **Approved — 지정된 컴파일·감사 결속 보정만 승인, Unity 재검증과 통합 수용 미완료**.
- 승인: 아스트라, 실제 `gpt-6-astra`, 2026-09-30. 독립 루나 검수 `docs/verification/2026-09-30-c4-router-compile-recovery-luna-design-review.md` SHA-256 `F8679A9E17655884F27CF90161662FA8D1096A8D544621F8611DE4F702FCE7F0`, 초안 SHA-256 `F6712A3D43D17673AB935F6996CBB280574098DC7BB1C3D59E6499700AD78D05`, P0/P1=0/0.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-30.
- 추적: `REQ-M5D7QC4-002/006/007`, `AC-M5D7QC4-002/006/009/010`, 공동 `AC-M5D7QC3-007/008`.
- 선행: Approved C4 r4·QA r2 및 이후 승인된 한정 보정. 사용자 제품 동작과 C1/C2/Profile 구현은 변경하지 않는다.

계획 v3의 실제 첫 `c4-focused-edit` Unity 실행은 native 종료값 1로 실패했다. 원시 로그는 `InputRouter.cs` 542·571행에서 `_preparedLaunchOwner`의 정적 형식 `object`를 `DesktopProfileLaunchAdapterV1` 매개변수에 전달하여 발생한 CS1503 두 건을 보인다. 다른 경고 하나는 이 컴파일 실패의 원인이 아니다. 첫 run의 before/after/native/log와 `artifacts/c4-final-validation-queue-result-v2.json`은 실패의 불변 증거로 보존한다. XML과 QA 최종 반환은 없고 나머지 8회는 실행되지 않았다. 동일 9회 선택을 현재 출력에 덮어쓰지 않는다.

아스트라는 Terra에게 `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`의 정확 두 호출 경계에서 `_preparedLaunchOwner`가 **원래 `DesktopProfileLaunchAdapterV1` 참조인지 명시적으로 확인**한 다음 그 동일 참조를 기존 `ValidateExecutionGuardContext` 또는 `IsExecutionGuardedPair`에 전달하도록 배분한다. 형식이 다르면 예외 또는 기존 false→예외 경계로 fail-closed한다. 새 Adapter 생성, 대체 owner/root, scalar authority, public API, C1/C2/Profile 알고리즘, 다른 guard 상태 전이는 금지한다. 이 수정은 Approved r4 `REQ-M5D7QC4-002/006`의 정적 타입 연결이며 제품 선택을 바꾸지 않는다. 컴파일 오류 두 건 해소를 Unity 재실행에서 확인해야 하고, 정적 검사만으로 전체 수용하지 않는다.

Router의 새 바이트가 독립 정적 검수를 통과한 뒤, 기존 `docs/evidence/c4-audit-current-source-pins.json`의 이전 정확 바이트를 새 `docs/evidence/c4-audit-current-source-pins-precompile.json`에 `FileMode.CreateNew`로 보존한다. 원래 경로의 현재 pin resource는 Router 행의 `Sha256`, `ChangeApproval`, `StaticReview` 결속만 새 Router SHA·본 최종 Approved 계약 SHA·독립 Luna 정적 검수 SHA로 갱신한다. Adapter 행·두 predecessor 행·schema/Trace·순서는 그대로다. Q0 감사 `HubUiOnlyQ0ScopeAuditEditModeTests.cs`에서는 `C4CurrentSuccessorPinSha256` 상수와 `AssertC4CurrentSuccessorPins` 안의 **Router 행에 한정된** `ChangeApproval`·`StaticReview` 정확 경로/SHA 검사만 보정한다. Adapter 행은 기존 r4 승인·기존 정적 검수의 정확 경로/SHA를 계속 요구한다. 임의 문서 fallback, 둘 중 하나의 승인·검수를 양쪽 행에 적용하는 완화, 원본 predecessor 및 C2/C2R 역사 행 변경은 금지한다. 다른 감사 메서드는 변경하지 않는다. `C4ExecutionBridgeStrictAuditTests.cs`는 기존 pin 경로를 동적으로 읽고 Q0의 현재 resource SHA와 두 정확 binding 검사를 확인하므로 변경하지 않는다. 새 pin resource와 Q0 source는 별도 루나 정적 검수 대상이며 기존 원본은 위 snapshot과 과거 입력 포착에 남긴다.

다음 큐 결과는 `artifacts/c4-final-validation-queue-result-v3.json`에만 `CreateNew`로 작성한다. Terra는 `artifacts/c4-final-validation-queue.ps1`의 고정 `-ResultPath` 허용값 `v2`를 `v3`로 한정 변경하며, fail-stop·하위 stderr/종료값·계획/잠금/선택 검사는 그대로 둔다. 기존 result-v1/v2와 run 출력은 보존한다.

최종 변경을 독립 검수한 뒤 `artifacts/c4-frozen-source-manifest-v6.json`을 `CreateNew`로 발급한다. v5의 102개 `Files` 경로·순서에서 Router, Q0 감사, pin resource, queue 도구 네 SHA만 현재 바이트로 교체한다. `AllowedChanges`는 기존 Router/Q0/queue 행의 이유를 이 승인·검수에 맞게 좁혀 갱신하고 pin resource 한 행을 `REQ-M5D7QC4-007`로 더해 26행으로 만든다. 선택 3개와 필수 행 188개는 그 source 바이트가 바뀌지 않은 경우에만 그대로 사용한다. Edit/Play fixture가 읽는 v1 소스 원장은 변경되지 않은 시험·C1/C2/Bridge 파일의 바이트 확인에만 사용하고, 실행 권위는 v6에 둔다.

`artifacts/c4-frozen-input-paths-v6.json`은 v5의 975개 경로·이유를 모두 보존하고 v6 원장·본 최종 Approved 계약·새 Luna Router 정적 검수·pin 이전 바이트 snapshot을 포함해 979개로 발급한다. 입력 목록 자신·실패/새 queue 결과·plan·run 출력은 제외한다. 이전 884개 경로와 현재 Assets/Packages/ProjectSettings/qa 완전성 및 `Sort-Object -CaseSensitive`를 유지한다.

`artifacts/c4-final-validation-queue-plan-v4.json`은 9회 순서와 선택·행·잠금·사례 180초·관찰 21,600초를 유지한다. v6 소스·입력 원장과 변경된 queue 도구만 새 지문에 결속하고, 출력이 이미 발급된 첫 run만 새 `c4-r2-focused-edit` stem 및 이에 맞는 정확 8개 출력 경로·NativeWitnessRunStem·실제 Windows 명령행 길이로 바꾼다. 다른 8개 run과 출력 경로는 기존 계획 v3과 같다. 현재 새 72개 출력과 result-v3은 모두 없어야 한다. 루나가 새 source/pin/Q0/원장/계획을 독립 검수하고 아스트라가 별도 배분한 뒤에만 Unity 큐를 재실행한다. 성공 여부와 `WholeAccepted`는 실제 새 증거로만 판단한다.
