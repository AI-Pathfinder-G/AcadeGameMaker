# B1 내부 반환 형식의 시험 접근 경계 보정

- 상태: Approved. 아래 아스트라 기록의 정확 시험 friend1 및 델타 검수 후 Run+시험 최소 진단 한 묶음만 허용한다. 제품·runtime 수용은 별도다.
- 산출물: 실제 생산 실패 출처 닫기와 중복 거절을 삭제하지 않고 현재 세 시험을 컴파일할 수 있는 입력. 테라 구현, 루나 독립 검수, Root 최소 진단, 아스트라 승인이다.

## 반증·최소 재현

r3 `project/Library/b1-minimal-csc-r3-test/result.json` SHA `1A2BD23EB049D47F1C73F73610881530123CF71371954AC7E486979AD2A21FCE`는 같은 Chapter1FailureSourceTests.cs:79 두 호출에서 CS0122로 끝났다. §23의 직접 참조1은 CS0012 메타데이터 누락을 해소했지만 내부 반환형식 접근까지 해소하지 못했다. 반환값을 버리면 충분하다는 앞선 추론을 폐기한다. 실제568v3·468 재사용·10자원 일치와 단회 결과를 보존하고 재실행하지 않는다.

CloseCompleted는 Run.Unity 내부이며 그 assembly는 이미 시험 assembly를 friend로 둔다. 반환 RunFailureTickInput은 순수 Run 내부이고 순수 Run friend에는 이 시험 assembly가 없다. 실제 생산 FlowOwner는 Run.Unity라 접근 가능하다. 문제는 두 권한 층의 조합이며 런타임 clear 로직 결함이라고 주장하지 않는다.

## 대안 비교와 선택

| 대안 | 파일·행동 | 수용 위험과 판단 |
| --- | --- | --- |
| 순수 Run의 정확 시험 friend1 | 기존 Runtime/Run/AssemblyInfo.cs 한 파일에 해당 시험 이름1 추가. 테스트/실패 생산 함수/반환형식 불변 | 해당 시험 assembly가 순수 Run internal 전체를 볼 수 있게 되는 실제 접근 확대다. 생산 assembly의 권한/동작은 불변. 정확 시험에 한정하고 기존 소스 검증을 유지하므로 선택 |
| 생산 닫기와 결과 읽기 분리 | Chapter1FailureSources.cs의 void 닫기+retained 결과, FlowOwner의 실제 소비 경로도 변경. 시험 wrapper만 추가하는 것은 금지 | 닫기/읽기의 stale·단회소비·revision 순서·실패 부분 확정이라는 새 생산 상태와 API가 생긴다. 이 컴파일 보정에 비해 행동 위험·검사 범위가 커서 채택하지 않음 |

선택은 첫 안이다. friend가 선언 메타데이터라는 이유로 권한 확대0이라고 표현하지 않는다. 기존 ‘신규 friend0’의 **정확 시험 assembly 하나에 대한 예외**를 아래와 같이 명시 승인받고 순수 Run 내부 형식은 public으로 바꾸지 않는다.

## 정확 규범 보정 후보

제품 루트 `output/chapter1-playable-r1/project/`의 `Assets/AcadeGameMaker/Runtime/Run/AssemblyInfo.cs`에 다음 한 줄만 추가한다.

```csharp
[assembly: InternalsVisibleTo("AcadeGameMaker.Run.Unity.PlayMode.Tests")]
```

기존 friend2·using·meta/GUID·asmdef의 §23 Run 참조1·생산/시험 소스는 보존한다. 주 계약 §11.6 및 §23, B1 시험 참조 보정의 ‘이외 friend0/새 friend0’를 이 시험 friend1에 한해서 명시 보정하는 후보이며 다른 조립체/제품 접근의 확대는0이다. C1C friend 범위와 혼합하지 않는다. 공개 factory/ForTests/반사/테스트용 wrapper·시험삭제/Ignore0이다.

세 시험은 이미 실제 source 닫기/토큰·완료view·Profile 결과를 쓰며 새 직접 코어 생성 코드가 필요하지 않다. 이번에는 metadata 직접참조와 양쪽 assembly의 internal 접근을 모두 닫는다. 범용 friend 집합·모든 시험 wildcard를 허용하지 않는다. 테스트 assembly는 기존 optionalUnityReferences TestAssemblies이며 생산 실행 조립체가 아니다.

REQ-C1B-FAIL-001/EVIDENCE-001/ROOM-001/RETURN-001 및 AC-C1B-FAIL-001/EVIDENCE-001/ROOM-001/RETURN-001/E2E-001/E2E-002에 결속한다. 정확 오류 선택자 `Chapter1FailureSourceTests.RejectStaleOrAllFalseFailureBatch`의 정상 닫기와 중복 거절 및 기존31을 유지한다. 접근 허용은 그 시험을 우회 수용하는 권한이 아니다.

## 새로운 검사 목적과 종료

루나 규범 검토→아스트라 명시 Approved→테라 정확 한 파일·전후 SHA→루나 델타→Root의 **순수 Run과 실패 시험 assembly 두 개만** 새 이름의 최소 진단 묶음 한 번 순서다. 순수 Run의 friend 메타데이터가 달라지므로 r2 Run DLL을 그대로 재사용하면 안 된다. Run을 새 출력으로 컴파일하고 시험 response는 그 새 Run DLL을 참조한다. r3 이후 시험 소스/RunUnity 함수는 불변이며 새 목적은 동일 두 호출의 CS0122가 사라지는지 직접 확인하는 것이다. 성공을 사전 선언하지 않는다.

기존 나머지 성공5는 소스·참조·컴파일 자원 지문이 같고 새 Run이 friend 메타데이터1 외 공개/내부 타입·메서드 서명/행동을 바꾸지 않는다는 델타 검수를 거친 범위만 재사용한다. Run.Unity 등 기존 DLL의 assembly 참조 identity는 유지하고 private implementation 호환을 임의 추정하지 않는다. Run 출력과 그 reference DLL이 필요하면 둘 모두 새 지문으로 결속한다. 새 두 response 차분은 정확 AssemblyInfo 입력변경/새 출력 경로와 시험의 Run 참조 경로만 허용하며 다른 입력 추가는 사전 영향 판정 대상이다. 원래468/10 원장은 역사로 남기고 새 재사용5·새 Run 연결 원장을 작성한다.

첫 실패에서 즉시 원시 결과·진단을 보존하고 자동 재시도/큰 Unity/성공5 재컴파일0이다. 종료는 새 두 컴파일의 실제 결과·독립 검수 및 기존31/회귀/종단 미실행 상태의 인계다. 실제 runtime 수용·전체 B1 수용·C1C 공유 기준 충족을 의미하지 않는다. 제품/시험 소스 행동 변경과 Q0·프로필·원격 권한은 추가하지 않는다. 현재 문서 작업의 제품 쓰기·컴파일·Unity·시험은0이다.

## 아스트라 한정 승인 기록

2026-10-03 아스트라는 루나 독립 규범 검토 `output/chapter1-playable-r1/evidence/b1-test-return-access-review-luna6.v1.json` SHA `302B69CB6575AF1DF82ADA3F701C31C1A553A1704F167A0349CB7F7CF479B348`를 직접 읽고 P0/P1=0인 정확 범위를 승인한다. 검토 원문 SHA `7D480702D45351F829A3C13F60C4975F995265B9CA744BD3174BC9BA931F5E4F`, 준비 v1 SHA `7949E7C9771B35027A4B3A3FF42334BC8F53AD011503D79925AA3748FDA92E44`, §23의 반증된 추론 및 r1/r2/r3 원시 결과는 보존한다. 앞선 승인 전 표현은 작성 당시 이력이며 이 기록이 정확 범위에서 대체한다.

승인 변경은 `Runtime/Run/AssemblyInfo.cs`의 기존 using/두 friend를 유지한 채 `InternalsVisibleTo("AcadeGameMaker.Run.Unity.PlayMode.Tests")` 한 줄 추가뿐이다. 변경 전 길이199/SHA `2BDE9F85BCA41C5EE4AC73B611461DFA0D744FE507CA4AF7C7D52B4495B11BC9`를 대조한다. 시험 조립체 전체에 순수 Run internal 접근을 주는 실제 QA 권한 확대임을 인정하고 이 정확 시험 이름 하나에 한정 승인한다. 생산 조립체의 접근·행동·API/테스트 assertions 변경은0이다. 기존 ‘새 friend0’ 원문을 삭제하지 않고 이 예외로만 보정한다.

루나는 현재568v3 SHA `0627A45B7841BDCC90D45A334AB5C1697C686D371C3EDADEA534A0FFCB0619DC` 및 기존468/10 입력 지문 일치를 확인했다. 테라가 정확1파일을 변경한 뒤 루나 델타 검수, Root의 새 Run+시험 두 조립체 최소 진단 **한 묶음 한 번**, 결과 독립 대조 순서로 진행한다. 새 Run DLL과 필요 ref DLL은 새로 만들고 시험은 그 산출물을 참조한다. 성공5만 변경 없는 입력/자원·assembly identity를 재확인하여 조건부 재사용하며 새 Run 변경을 기존468 원장에 소급하지 않는다.

두 컴파일 중 첫 실패에서 뒤 실행을 중단하고 원시 진단을 보존한다. 자동 반복·큰 Unity 실행·성공5 재컴파일·다른 friend/생산 API 변경은0이다. 목적은 이번 정확 권한 폐쇄가 두 CS0122를 해소하는지 관측하는 것이며 사전 성공 판단이 아니다. 신규31·필수 회귀·종단은 미실행이고 B1 최종 수용은 남아 있다. C1C 공유 쓰기 관문과 Q0/프로필/원격 권한은 불변이다. 이번 문서 작성의 제품 쓰기·컴파일·Unity·시험은0이다.
