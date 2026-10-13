# 정규화 후보와 과거 시험 지문 예비 대조

상태는 읽기 전용 기계 대조이며 시험 실행이나 독립 수용이 아니다. `REQ-PLAT-048..050` / `AC-PLAT-046..048`의 후속 검증 설계 입력으로 505 원장（`../../artifacts/c4-publication-lf505-baseline-v1.json`；로컬 보존 자료）의 원본 SHA와 후보 SHA를 과거 동결 원장의 같은 정확 경로와 비교했다. 과거 원장이나 소스를 변경하지 않았다.

| 역사 원장 | 후보와 겹치는 경로 | 후보 지문 불일치 | 원본은 같고 LF만 다른 경로 | 역사와 현재 원본 내용도 다른 경로 |
| --- | ---: | ---: | ---: | ---: |
| v1 | 37 | 12 | 4 | 8 |
| v16 | 37 | 9 | 8 | 1 |
| v17 | 37 | 9 | 9 | 0 |
| v19 | 37 | 9 | 9 | 0 |

원장 SHA-256은 v1 `C31A610613CA4CD5F5F3B69720F08158CEECCE090A665C7496DCA8D57528A4EF`, v16 `02218AB9466E964952D5BB4D81A630D90E12F403A260410BD65CC72B1A19973F`, v17 `E3D8626C7DA9F058D228022ED357110A7FF6B0A14A05A15B88FC4FA8C8E668D6`, v19 `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`다. 겹치는 37경로의 비교이므로 원장 전체 폐쇄를 뜻하지 않는다.

v16의 실제 내용 차이 한 경로는 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs`다. v1의 여덟 내용 차이는 후속 코드·시험 변경을 포함하므로 모두를 줄바꿈 전이라고 취급할 수 없다. 시험이 읽는 각 원장과 실제 단언 대상의 정확 부분집합을 따로 확인해야 한다.

`C4ActualExecutionEditFixtureV1.cs`의 `AssertFrozenNoOpSingleCallSource`는 v16 원장과 하드코딩된 소스 지문을 비교한 뒤 현재 원문 바이트를 SHA-256으로 확인한다. `ProfileResetExecutionBridgeV1.cs`는 LF 후보에서 그 바이트가 달라지므로 기존 지문 단언을 그대로 통과할 수 없다. 무조건 정규화해 허용하거나 역사 SHA를 덮어쓰는 대신 새 승인 원장과 명시적 변환 결속을 설계한다. 기존 행위·호출 횟수 단언을 유지하고 모든 새 바이트의 별도 검증이 필요하다.

`PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`, `CleanCloneExecuted`는 모두 거짓이다. 과거 C3/C4 수용은 당시 바이트의 역사로 보존하며 이 예비 대조를 새 후보 수용으로 확대하지 않는다.
