# C4 Router 컴파일 복구 설계 독립 검토

검토 대상: `docs/specs/work-contracts/2026-09-30-c4-router-compile-recovery-draft.md` — SHA-256 `F6712A3D43D17673AB935F6996CBB280574098DC7BB1C3D59E6499700AD78D05`.

## 판정

Draft 설계 P0 0건, P1 0건이다. 제안은 실제 두 컴파일 오류와 Q0 핀 검사 제약을 반영한 제한 변경으로 성립한다. 소스 및 원장 바이트가 아직 발급되지 않았으므로 이번 판정은 실행 승인이나 수용이 아니다.

## 소스 경계

실제 Router SHA는 `542FB7DCCC92B8B59DDDBE4A77741BBEFEEE65546F315F5BB2AA3FA8D22B57F5`이며 `_preparedLaunchOwner`는 `object`다. 로그의 두 CS1503 위치는 Router의 `ValidateExecutionGuardContext` 및 `IsExecutionGuardedPair` 호출이다. 각각의 선언은 `DesktopProfileLaunchAdapterV1` 인자를 요구한다. 정확히 이 두 자리에서 기존 필드 값이 Adapter 형식인지 먼저 확인하고 그 참조를 그대로 넘기면 타입 오류를 해소하면서 원소유자 동일성 검사를 보존한다. 형식 불일치가 null/대체 Adapter로 조용히 바뀌지 않고 예외 또는 기존 거절 경로로 끝나야 한다. 두 지점 외 C1/C2·Profile 동작이나 guard 전이를 바꾸지 않는 조건은 적절하다.

실패 기록은 `artifacts/c4-focused-edit-native-exit-observation.json`의 native 종료 1, `artifacts/c4-focused-edit.log`의 542·571행 컴파일 오류 및 `artifacts/c4-final-validation-queue-result-v2.json`의 빈 실행 목록·거짓 전체 수용으로 뒷받침된다. 기존 부분 실행 증거를 덮어쓰지 않고 첫 stem을 바꾸는 계획도 합리적이다.

## 핀·감사 경계

현재 Q0 source SHA는 `567233B703E56C792FA63947A7CCD81E8802BD991E0432DD702DB8D20C0DD878`, 현재 pin resource SHA는 `FCA015B48B9EDE8253D4E80C21521B92342DCA7F5B7C87F0BF70A59CE7C74548`다. Q0의 `AssertC4CurrentSuccessorPins`는 두 행 모두에 기존 r4 승인·기존 검수 지문을 고정한다. 그러므로 Router 행에만 새 승인·독립 검수를 연결하고 Adapter 행은 기존 지문을 유지하는 경로별 조건이 필요하다. Draft의 이번 보정은 이 고정 검사와 맞으며, 경로 수·중복·전임자 및 C2/C2R 역사 검사는 계속 유지해야 한다. pin 파일 변경 전 바이트를 별도 `CreateNew` snapshot으로 남기고 Q0의 현재 pin 상수만 새 resource SHA에 맞추는 순서도 순환 참조를 만들지 않는다. 새 review 문서는 Router 지문을 적되 pin resource 지문에 되먹임하지 않아야 한다.

`C4ExecutionBridgeStrictAuditTests.cs` SHA `1089AFEC1CD19AD231F1EBC7EB0A7E272A17C7B90705F195D916B45AB8860772`는 수정하지 않는 것이 타당하다. 그 검사는 Q0의 현재 helper가 고정 자원을 먼저 해시 확인하고 정확 predecessor·승인·검수를 확인하는 구조를 읽는다. 조건부 정확 binding은 Q0 helper 자체가 실행해 검증하되, `return`/예외 삼키기 경로를 새로 넣지 않아야 한다. 기존 focused 선택의 이름·필수 188행은 변경하지 않는다는 조건도 방법명·ExpectedRows가 그대로일 때 유효하다. `Edit562` 실행 집합에 포함된 Q0 테스트는 유지된다.

## 다음 지문에서 확인할 항목

현재는 초안만 검토했다. 실제 변경 후 Router, Q0, pin snapshot/resource의 정확 지문과 조건부 Adapter/Router binding을 대조해야 한다. v6 원장 102개 파일의 네 지문 변경·허용 변경 26개, 입력 979개, 계획 v4의 첫 run 전용 새 stem/여덟 출력 및 나머지 여덟 run 불변은 발급 뒤 각각 다시 검수해야 한다. 결과-v2·이전 계획·기존 원장을 보존하고 결과-v3 및 72개 출력이 비어 있는지도 재확인해야 한다. Unity 재실행은 이번 검토에서 하지 않았다.
