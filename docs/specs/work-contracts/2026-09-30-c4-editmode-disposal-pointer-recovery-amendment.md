# C4 편집 모드 입력 자산 해제와 소스 위치 관측 복구 초안

- 상태: **Approved — 지정된 제한 구현만 승인, Unity 재검증과 통합 수용 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`. 독립 루나 설계 검토 `docs/verification/2026-09-30-c4-editmode-disposal-pointer-recovery-luna-design-review-r2.md` SHA-256 `F2D2446E814D122DC8B9F75388DB057503014891553BF6FC6D59CCA788959835`, 검토 대상 초안 SHA-256 `5850DE350B6B90501BDDCCBFE48CCCAB72FD9C17E7724A157D5BAFFF3D982063`, P0/P1=0/0.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-30.
- 추적: `REQ-M5D7QC2-004`, `REQ-M5D7QC4-001/003/007`, `AC-M5D7QC2-002/004`, `AC-M5D7QC4-003/008/009/010`.
- 선행: Approved C2, C4 r4, C4 QA r2, C4 집중 Edit 관측 복구 계약. 이 초안의 승인 전에는 해당 계약을 변경하지 않는다.

계획 v6의 실제 첫 실행 `c4-r4-focused-edit`는 139건 중 5건 통과, 134건 실패 후 중단됐다. `artifacts/c4-final-validation-queue-result-v5.json`, 첫 실행의 XML·로그·원시 종료·입력 전후 포착은 불변 실패 증거로 보존한다. 77건은 시험 fixture의 소스 위치 판별 오류이고, 57건은 생성 `GameInputActions.Dispose()`가 EditMode에서 `UnityEngine.Object.Destroy(asset)`를 호출해 낸 Unity 오류 로그다. 오류 로그가 먼저 시험을 중단시키므로 그 뒤의 제품 단언은 통과로 해석하지 않는다.

## C2 처분 의미의 제한 개정

C2 계약의 “정확한 소유 객체의 실제 `Dispose` 1회” 문구를 **EditMode에서 생성 입력 자산을 해제하는 경우에만** 아래로 좁힌다. `InputRouter`가 원본 소유 증거로 확정한 바로 그 `GameInputActions`의 `asset`을 원래 Unity 스레드에서 `DestroyImmediate`로 한 번 해제한다. `Application.isPlaying`인 Editor 재생 모드와 빌드에서는 기존 `actions.Dispose()`를 정확히 한 번 호출한다. 생성된 `GameInputActions.cs`와 `.inputactions`는 수정하지 않는다. `DisposeOld`·`DisposeNew` 제어의 동일 종류·순서·정확 소유 객체·한 번의 시도, 이미 기록된 terminal/attempted 원장, map 비활성화·callback 해제, catch의 영구 폐쇄, 재진입 거절은 보존한다. EditMode 자산이 실제 해제됐는지 관측하며 오류 로그를 예상 등록하거나 무시하지 않는다. 이 개정은 다른 C2 경로, 재생 모드, 공개 API, worker의 Unity 접근 권한을 바꾸지 않는다.

Terra의 허용 제품 변경은 `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs`의 비공개 처분 도우미와 C2 원본 소유쌍의 기존 처분 호출점 445/467/491/516/517에 한정한다. 일반 `CloseActions`의 1310행은 이번 복구에서 변경하지 않고 기존 `Dispose()`를 유지한다. C2 처분 호출의 기존 시도 횟수와 소유 증거는 그대로 둔다. `#if UNITY_EDITOR` 분기 안에서만 EditMode 판정을 수행해 빌드 동작에 Editor 의존성을 넣지 않는다. 생성 파일에 조건문을 삽입하지 않는다. Luna는 실제 수정 전후 호출점·소유 증거·재생 모드 경로와 C2 의미가 좁게 보존됐는지 독립 검수한다.

## 시험 소스 위치 관측 복구

Terra는 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs`의 `FrozenNoSameCaseSavePointers`만 수정한다. 부모 시험의 선언 포인터는 현재 시험의 정확한 `public void <MethodName>(`으로 시작하는 선언행에서 유일하게 찾고, 본문에 인용된 문자열은 선언으로 세지 않는다. fixture 포인터는 정확한 대상 메서드의 선언·본문 범위에서 찾으며 `ObserveActualBarrier`의 두 오버로드 중 실제 파일 조회 문장을 포함한 오버로드만 고른다. 모든 대상의 선언·조회 위치 유일성, 정확 source path·line, 두 파일의 동결 SHA, 부모 본문의 `ProfileAtomicSaveServiceV1.Save(` 부재 검증은 유지한다. 값을 찾지 못하거나 중복이면 실패한다. 139개 선택 이름과 188개 기대 행·사례 순서·기대 결과는 바꾸지 않는다. Luna는 48건 오버로드와 29건 부모 선언 인용을 각각 원시 R4 XML·현재 소스와 대조하고, 디버그 심볼의 line 표기가 실제 새 빌드의 source와 맞는지 확인한다.

## 동결과 재실행 경계

제품·fixture의 최종 바이트를 Luna가 독립 검수한 뒤 현재 Router의 Q0 감사 pin, source manifest v9, Edit 선택 v4, Hub 선택 v2, checkpoint 행 v4, 입력 경로 목록 v9, queue 계획 v7을 새 불변 버전으로 발급한다. 이전 버전과 실패 출력은 수정·삭제하지 않는다. `docs/evidence/c4-audit-current-source-pins.json`은 보존하고 새 pin을 별도 `CreateNew` 경로로 발급한다. Q0의 현재 pin 경로·SHA·Router 승인/검수 결속은 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs`에서, 동일 pin 경로의 엄격 비교는 `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs`에서 정확히 갱신한다. 다른 Q0/엄격 감사 검사와 이전 pin은 보존하고 Q0 감사가 통과해야 한다. 이 두 시험 파일은 이 결속 보정에 한해 추가 허용한다. Strict 감사 시험의 SHA 변경은 Hub 선택 v2의 `SourceFiles`에 반영한다. Edit 선택 v4는 v3의 139개 이름·순서·원본 시험 `SourceFiles`를 실제 파일 SHA와 재대조한 뒤 동일하게 `CreateNew` 발급한다. checkpoint 행 v4는 188행·순서를 보존하고 바뀐 fixture·Strict 소스 SHA를 반영한다. Play 선택과 후속 8회 실행의 논리적 계획은 보존한다. Edit fixture의 현재 source manifest 자기 결속은 v9로 갱신하되 선행 제품 소스의 역사 결속은 바꾸지 않는다. queue 도구는 새 결과 경로 v6만 추가하고 기존 fail-stop·CreateNew·원시 종료값 검사를 유지한다. 계획과 출력 경로의 정확 지문·개수는 동결 담당자가 산출하여 Luna가 독립 확인하고, 아스트라가 별도 배분한 뒤에만 Unity를 실행한다. 새 실패가 나오면 첫 지점에서 멈춰 증거를 보존하며 수정 없이 성공으로 판정하지 않는다.
