# B1 시험 조립체의 순수 Run 직접 참조 보정

- 상태: Approved. 아래 아스트라 한정 승인 기록의 정확 asmdef 참조1 및 후속 델타 검수·최소 컴파일1회만 허용한다. 제품·시험 수용은 별도다.
- 사용자 산출물: 실제 방 진행·실패 재도전을 검증할 수 있는 현재 소스 빌드. 담당은 테라 수정, 루나 규범·델타 검수, Root 최소 컴파일, 아스트라 승인이다.
- 근거: `project/Library/b1-minimal-csc-r2/result.json` SHA `BB34C3BCB91ED9F0E6A78911B8A2C06133DA5D845E0D0ACBF546211755CAF984`와 실제 시험3개·Run.Unity API·asmdef/친구 선언을 읽었다.

## 반복 오류의 최소 원인과 범위

앞선 §21 오류는 Input이 순수 Transfer 내부 멤버를 읽으려던 생산 접근 오류였다. 그 두 파일 보정 뒤 r2의 Run/Transfer.Unity/Input.Unity/Run.Unity/Hub.Presentation.Unity/Combat.Authoring.Editor 여섯 컴파일은 종료코드0이다. 이는 해당 컴파일 범위이며 시험 성공·전체 B1 수용이 아니다.

이번 오류는 마지막 `AcadeGameMaker.Run.Unity.PlayMode.Tests`만 종료코드1이고 `Chapter1FailureSourceTests.cs:79`의 `sources.CloseCompleted(view)` 두 호출이 CS0012다. 호출은 둘 다 반환값을 버리며 생성·멤버 접근을 하지 않는다. 선언은 Run.Unity의 접근 가능한 내부 `RunFailureTickInput CloseCompleted(Chapter1EncounterEvidenceView completed)`이고 반환 형식 정의는 순수 Run assembly 내부다. 참조 목록에는 Run.Unity/Input.Unity/Core/Profile/InputSystem/TestFramework가 있고 순수 Run이 없다. 전이 참조만으로 반환 형식의 메타데이터 참조를 충족했다고 가정하지 않는다.

순수 Run의 friend는 기존 EditMode 시험과 Run.Unity뿐이다. 직접 참조는 메타데이터 해석을 제공할 뿐 내부 생성자·멤버 접근권을 주지 않는다. 현재 시험은 반환값을 쓰지 않으므로 새 friend가 필요한 동작을 하지 않는다. 이 한정 추론의 실제 폐쇄 여부는 새 최소 컴파일에서 확인하며 컴파일 성공을 미리 선언하지 않는다. 내부 반환값을 변수에 보관하거나 인자를 새로 만들거나 멤버를 검사하는 변경은 이번 범위 밖이다.

## 세 시험 및 반환 형식 대조

| 시험 | 직접 소비 API/반환 형식 | 참조 판단 |
| --- | --- | --- |
| Chapter1RoomPlanTests | RoomCatalog.ReadPlan→Chapter1RoomPlan, RoomInstance, Flow.View→Chapter1RunView, lifetime completed view | Run.Unity/Input.Unity/Core와 기존 Unity/NUnit 참조로 연결; 순수 Run 내부 반환 소비 없음 |
| Chapter1FailureSourceTests | FailureSources.CloseCompleted→내부 RunFailureTickInput(두 호출 모두 결과 폐기); 나머지 Hazard/FailurePublication/AcceptedFailureToken 및 bool/void | 순수 Run 직접 참조 누락1. 새 friend 또는 반환 API 변경 없이 참조1 후보 |
| Chapter1ExpeditionPlayModeTests 및 같은 파일 trace | Flow/RunView, Hub token/frame, Profile snapshot/SaveResult, Input completed view, bool/정수/void/iterator | Run.Unity/Input.Unity/Core/Profile 및 기존 Unity/InputSystem/TestFramework/NUnit 참조. 순수 Run 내부 생성·멤버 접근 없음 |

Run.Unity의 나머지 공개된 시험 소비 API도 RoomCatalog/RoomInstance/DevelopmentProfile/Flow/View/FailureSources 반환·인자 형식을 작게 대조했다. 이번 r2 컴파일이 보고한 직접 참조 누락은 위 한 형식/두 호출뿐이다. 컴파일러 진단은 모든 미래 경로의 무결성 증명이 아니다. 같은 계열 두 오류의 원시 기록을 보존하고 큰 Unity 실행으로 재탐색하지 않는다.

## 정확 변경 및 규범 호환 후보

제품 루트 `output/chapter1-playable-r1/project/`의 기존 `Assets/AcadeGameMaker/Tests/PlayMode/RunUnity/AcadeGameMaker.Run.Unity.PlayMode.Tests.asmdef` 한 파일 references에 `AcadeGameMaker.Run` 하나만 중복 없이 추가한다. 나머지 JSON 값·참조·meta/GUID·세 시험 소스·생산 소스는 그대로다. 신규 friend/public factory/ForTests/반사·시험 삭제·Ignore 추가는0이다.

주 계약 §11.6의 정확 시험 참조 네 개 및 기존 시험 지원 집합에 이 순수 Run 참조1을 추가하는 명시 보정이다. C1C §4에 이미 있는 Run 참조 후보를 B1의 과거 권한으로 소급하지 않는다. C1C의 나머지 참조4를 함께 추가하지 않는다. 본 Review 승인 전 기존 참조 범위를 조용히 확대하지 않는다.

REQ-C1B-FAIL-001/EVIDENCE-001/ROOM-001/RETURN-001, AC-C1B-FAIL-001/EVIDENCE-001/ROOM-001/RETURN-001/E2E-001/E2E-002를 추적한다. 정확 오류 선택자는 `Chapter1FailureSourceTests.RejectStaleOrAllFalseFailureBatch`다. 이 접근 보정으로 정상 source close 및 중복 close 거절 검사를 삭제하지 않는다. 나머지 신규31 선택과 기존 필수 회귀·종단도 유지하며 아직 실행0이다.

## 새 관측 목적·검사 재사용·종료

루나 규범 검수→아스트라 Approved→568 변경 전 기준과 asmdef 지문 대조→테라 한 파일 변경·전후 SHA→루나 델타 검수→Root가 실패한 시험 조립체만 새 이름으로 최소 컴파일 한 번 수행한다. 목적은 순수 Run 메타데이터 참조1로 두 CS0012가 없어지고 내부 접근 오류 없이 현재 세 시험이 컴파일되는지 확인하는 것이다. 실패 시 원시 진단을 보존하고 자동 반복·friend 추가·시험 변경 없이 원인 판정으로 돌아간다.

r2 성공6의 소스/참조/DLL·컴파일러 입력 지문이 동일함을 Root가 새 실행 전 고정하고 루나가 확인한 범위만 재사용한다. r2 DLL의 실제 경로·길이/SHA와 새 시험 response의 참조를 연결한다. asmdef 한 파일 외 입력 변경·누락 DLL·지문 불일치가 있으면 재사용을 중단하고 정확 영향부터 다시 판정한다. 성공6을 재컴파일하거나 Unity 전체 실행으로 확대할 권한은 없다. 새 시험 response의 변경은 승인 Run DLL 참조1과 새 출력 경로만 허용하고 나머지는 r2와 대조한다.

종료는 한 파일 정확 델타·시험 조립체 최소 컴파일 실제 결과·루나 독립 대조·미실행31 및 회귀/종단 상태를 인계하는 것이다. 이후 runtime 검사는 기존 별도 관문을 따른다. §21 생산 보정의 성공 컴파일 범위와 이번 새 미해결 시험 접근 결함을 분리한다. C1C 공유 쓰기는 독립 B1 최종 기준 전0이며 Q0·프로필·원격·Unity 실행은 이 설계 작업에서0이다.

## 아스트라 한정 승인 기록

2026-10-03 아스트라는 루나 독립 검토 `output/chapter1-playable-r1/evidence/b1-test-run-reference-review-luna6.v1.json` SHA `4EABDE87E2B82707AC9CB4D4BE9BC28AE384AEBA35C3D4B1D93746F49CF0420F`를 직접 읽고 P0/P1=0인 위 정확 보정을 승인한다. 검토 원문 SHA `DE4114FB7D07D6B0B31D0C27C1F31AABF352A971CE5948CEA4B252750FA92DB0`, 준비 v1 SHA `4F9F3208DCAA322794621E82572BC67B514AB355BF9FD42CCDE510571560042B`와 과거 실패 원시 결과를 보존한다. 앞선 Review/승인 전 문구는 그 작성 이력이며 이 기록이 정확 범위에서 대체한다.

루나가 현재568·준비8 지문 불일치0, 성공6의 재사용468파일과 컴파일 자원10파일 지문 불일치0을 독립 확인했다. `preparation/b1-r2-successful-six-reuse-pin-root.v1.json` SHA `5C6C12B3C2D0CC9765374CC4A829D6C02AEEEBAF56C4EB59AF83152E11026E35`, `preparation/b1-r2-compiler-resources-pin-root.v1.json` SHA `DF946905A47CC3C724FC503F5AA677FB9BEAD6E8BF22263A8980A4470148FDC9`의 해당 현재 입력을 실행 직전 동일성 조건부로 한정 재사용 승인한다. 원장의 당시 false 상태를 true로 소급 변경하지 않고 이 승인 기록을 결속한다. 캐시 엔진 참조를 사용한 컴파일 진단이며 Unity 가져오기·runtime AC·현재 빌드 수용이 아니다.

테라는 정확 asmdef 한 파일에 Run 참조1만 추가하고 후보/적용 바이트 및 전후 지문을 인계한다. 루나는 순수 Run 참조1 외 차이0·시험/생산 소스·meta 불변을 확인한다. 그 후 Root는 실패 시험 조립체의 새 response에서 승인 Run DLL 참조1과 새 출력 경로만 r2 대비 변경됐음을 대조하고 같은468/10 지문을 재확인한 뒤 최소 컴파일을 한 번 수행한다. 기존 성공6 재컴파일·큰 Unity 실행·새 friend·시험 삭제0이다. 실패면 자동 반복하지 않는다.

이전 두 실패와 r2 성공6은 각각의 원시 범위로 보존한다. 신규31 시험과 필수 회귀·종단은 아직 미실행이며 참조 보정이나 컴파일 진단을 통과 증거로 세지 않는다. C1C의 충돌 없는 신규 후보 준비는 별도 기승인 범위이고 공유 제품 쓰기는 B1 독립 최종 기준 및 C1C 통합 관문 전0이다. 이번 문서 작업의 제품 쓰기·컴파일·Unity·시험·프로필·Q0·원격은0이다.
