# C3 실제 임시 루트 실행 모드 시험의 위치 보정안

- 날짜: 2026-09-29
- 상태: 제안 — 아스트라 승인 전 시험 위치 허용 확대 없음
- 설계자·실제 모델: 솔, `gpt-6-sol`
- 추적: `REQ-M5D7QC3-001/005/007`, `AC-M5D7QC3-001/005/006/009/010`
- 읽기 전용 확인. 소스·시험·asmdef·friend 변경 및 Unity 실행 없음.

## 확인한 접근 경계

`Input/Unity/AssemblyInfo.cs`는 `AcadeGameMaker.Input.Unity.PlayMode.Tests`,
`AcadeGameMaker.Input.Unity.EditMode.Tests`, 제품 Hub.Presentation.Unity에 내부
접근을 허용한다. Hub.Presentation.Unity.PlayMode.Tests에는 허용하지 않는다.
따라서 현재 허용된 Hub 실행 시험에서 `IDesktopProfileLaunchEnvironmentV1`을
직접 구현하거나 `DesktopProfileLaunchAdapterV1.ConfigureForTests`에 안전한 임시
루트 환경과 실제 preparation을 형식으로 제공할 수 없다. 기존
`ConfigureHubUiOnlyForAuthoring`은 router만 받으며 임시 경로 환경을 설정하지 않는다.
기본 환경은 실제 `Application.persistentDataPath`를 조회하므로 이번 양성 시험의
대안이 아니다.

기존 Input.Unity.PlayMode.Tests에는 이미 해당 내부 접근이 있고 Input.Unity/Profile/
Input System 시험 프레임워크를 참조한다. 그러나 **Hub.Presentation.Unity 참조 및
그 내부 friend가 없다**. 신규 파일 하나를 이 위치에 넣는 것만으로 Hub 내부 형식을
직접 호출할 수 있다고 주장하면 잘못이다. C# namespace 이름을 Hub 이름으로 바꿔도
assembly friend 접근 권한은 생기지 않는다. Hub runtime은 현재 `autoReferenced:true`인
일반 Unity 조립이지만 이것이 명시 참조나 friend를 대신하지 않는다.

## 권고하는 최소 시험 위치 보정

새 파일 두 경로만 Approved C3 시험 허용 목록에 추가하는 방안을 권고한다.

- `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs`
- 같은 파일의 `.meta`

시험 namespace는 현재 조립의 관례인
`AcadeGameMaker.Tests.PlayMode.InputUnity`를 사용하고, 실제 권한은 기존
`AcadeGameMaker.Input.Unity.PlayMode.Tests` 조립에서 얻는다. 파일 자체에 임시 root
`IDesktopProfileLaunchEnvironmentV1` 구현을 두고, inactive cohort에서
`ConfigureForTests(router, environment, new DesktopProfileLaunchPreparationPortV1())`를
호출한다. 이 실제 포트는 실제 `ProfileLaunchPreparationCoordinatorV1.Prepare`를
호출한다. 준비 영수증·proof·capture를 생성하거나 다른 시험 private factory를
결합하지 않는다. 환경의 root/UTC만 결정론적으로 공급하며 preparation은 대체하지 않는다.

상위 Hub는 새 assembly 참조/friend 대신 **이미 존재하는 정상 authoring/intake/
decision API를 참조 조회로 호출**할 수 있다. `Type.GetType`에 실제 형식·assembly
이름을 지정하고 `GameObject.AddComponent(Type)`로 실제 컴포넌트를 만든다.
`ConfigureForAuthoring`, Q-B actual take, `AcceptNewGame`, `Confirm/Cancel/Rearm`에
실제 반환 객체를 그대로 전달한다. 이는 필드 조작이나 권한 제조가 아니라 실제
제품 메서드의 호출 방식이다. 양성 fixture는 private 상태/receipt/proof를 쓰지 않고
실제 lifecycle과 발급을 통과해야 한다. 기존 정상 메서드의 참조 호출조차 금지하는
별도 지시가 있으면 이 두 파일만의 보정안으로는 해결되지 않으며, 그 경우 별도
구조 결정을 받아야 한다. 이번 안은 그러한 권한 확대를 승인하지 않는다.

참조 조회에는 다음 필수 결속 규칙을 적용한다. 각 상위 제품 형식은 예를 들어
`AcadeGameMaker.Hub.Presentation.Unity.NewGameConfirmationOwnerV1, AcadeGameMaker.Hub.Presentation.Unity`
처럼 정확한 assembly-qualified 이름으로 한 번 해석하고 실제 `Assembly.GetName().Name`
및 `FullName`을 대조한다. 메서드는 `BindingFlags.DeclaredOnly`와 정확한 instance/
static 및 nonpublic 조건을 적용하고, 선언 형식·이름·전체 매개변수 형식 배열·반환
형식을 모두 대조한다. out/ref 매개변수는 해당 실제 형식의 `MakeByRefType()`까지
포함한다. generic/optional/유사 overload로 대체하지 않는다. 예를 들어 actual Q-B
take의 마지막 인수는 실제 `IssuedNewGameRequestV1`의 by-ref 형식이며 반환 형식은
정확히 `bool`이어야 한다. owner intake는 실제 issued 형식과 실제 lower adapter/
router 형식 세 인수 및 실제 `NewGameConfirmationStartResultV1` 반환 형식을 요구한다.
각 호출 전에 실제 선언 형식과 대상 객체의 형식/assembly 결속도 확인한다.

이름 단독 `GetMethod`, 선언 형식을 생략한 상속 검색, parameter 개수만의 검색,
해석 실패 뒤 다른 형식/overload로 넘어가는 fallback은 금지한다. 형식·서명 해석
실패와 `TargetInvocationException`을 포함한 호출 실패는 실제 원인을 보존한 시험
실패로 보고하며 성공·건너뜀·판정보류로 바꾸지 않는다. actual opaque handle,
decision/retry/rearm capability 및 confirmed request는 null과 정확한 형식을 검사하고
최초 반환 참조를 보관한다. 다음 API에 넘길 때 `ReferenceEquals`로 같은 객체임을
확인하며 값 복사·생성자 재구성·clone·pending 검색·반사 field 수정으로 대체하지
않는다. request row 등 값 데이터가 필요한 별도 검증과 참조 권한 전달을 구분한다.

Hub UI의 TMP/버튼/프레임 객체도 실제 런타임 형식을 참조 조회로 구성하거나 기존
자산의 실제 인스턴스를 사용한다. 이미 활성화돼 기본 환경으로 Awake가 실행된
prefab을 뒤늦게 임시 환경으로 덮어쓰지 않는다. 반드시 inactive cohort를 구성해
환경 주입을 끝내고 활성화하여 실제 launch→latch→presenter→Q-B 순서를 검증한다.
기존 API로 필요한 UI를 구성할 수 없으면 멈추고 누락된 경계를 보고하며, private
상태 대입이나 합성 receipt로 양성 경로를 대체하지 않는다.

## 양성 경로와 입력 증거

새 class는 Input System의 기존 시험 프레임워크로 실제 가상 입력 장치를 사용한다.
실제 UIOnly 입력과 router의 프레임 게시를 통해 Q-A가 NewGame을 선택·retain하고,
Q-B의 실제 LateUpdate와 C3 take가 만든 opaque handle을 소유자에 전달한다.
의미 있는 프로필은 임시 root의 실제 파일로 준비한다. Confirm은 실제 재관찰 후
반환된 권한을 검사하며 C1 Begin을 호출하지 않는다. Cancel/Rearm은 실제 결정
capability와 후속 세대 handshake를 사용한다. 후속 cursor의 첫 실제 유효 프레임은
게시 순서로 공급하고, 그 프레임이 폐기되어 선택·알림 닫기·새 take를 일으키지
않으며 다음 적법 입력만 새 권한을 발급함을 확인한다. 모든 양성 객체는 실제 발급
결과여야 한다. negative corruption 반례와 양성 구성은 명시적으로 구분한다.

정리 시 생성한 게임 객체·입력 장치·임시 디렉터리만 회수하고 실제 저장 경로를
조회·변경하지 않는다. 기존 adapter/root/Q0 핀, assembly 방향, 제품 public API와
C4 권한은 유지한다. 이 설계가 컴파일·실행됐다는 판정은 아직 없다.

## 편집 시험에서 실행 상태 진입하는 대안과 선택 범위

현재 허용된 Input.Unity 편집 시험 assembly는 내부 하위 환경에 접근할 수 있으므로
편집 시험에서 실행 상태로 진입하는 접근도 검토 대상이다. 그러나 그 시험의 선언
조립은 여전히 Editor 범위이며 현재 별도 PlayMode selector에 자동 포함된다고
간주할 수 없다. 이번 로컬 확인에서 설치 시험 패키지의 EnterPlayMode 구현 자료를
찾지 못했고 실제 runner 열거·실행도 하지 않았으므로 구체적 재로드/fixture 유지
동작을 확인했다고 주장하지 않는다. 이 대안은 실행 상태에서의 증거와 PlayMode
시험 선택 범위가 달라 별도 증명/원장을 요구한다.

권고 위치는 실제 기존 PlayMode 조립의 신규 class여서 새 정확한 집중 selector와
예상 이름 원장을 만들기 쉽다. 기존 필수 PlayMode selector가 새 class를 포함한다고
가정하지 않는다. 기존 고정 회귀 이름 목록은 보존하고 신규 class를 별도 집중
실행한 뒤 정확한 합집합·중복·누락을 대조한다. 전체 조립 선택 selector는 신규
class 추가로 개수가 달라질 수 있으므로 실제 열거한 예상 목록으로 기록한다.
이전 실행 결과를 새 코드의 검증으로 재사용하지 않는다.

필수 회귀의 기존 536개와 fault 74개는 원래 정확한 이름 원장을 별도로 보존한다.
신규 class의 집중 예상 이름 원장은 세 번째 별도 원장으로 만든다. namespace/class를
고정한 신규 선택 예시는
`^AcadeGameMaker\.Tests\.PlayMode\.InputUnity\.NewGameConfirmationOwnerPlayModeTests\.`
이다. 이는 사용 runner의 필터 문법과 실제 전체 이름을 확인할 **선택 후보**이며
실행 가능한 필터가 확인됐다는 증거가 아니다. broad assembly/class 부분 문자열만으로
선택됐다고 판정하지 않는다. 최종 필터와 runner에 전달된 실제 인수는 원장에 저장한다.

시험 소스를 동결한 뒤 `[UnityTest]`, `[Test]`, 각 `[TestCase]`/관련 source가 만드는
실제 runner의 전체 이름을 XML 등 해당 runner의 실제 열거 결과로 확정한다.
소스의 메서드 수나 수작업 괄호 문자열만으로 전체 이름을 추정하지 않는다.
실제 결과 parser로 기존 536·74 및 신규 집중 원장 각각의 예상/실제 이름 집합과
개수, 누락·초과·중복을 대조한다. 기존 610개와 신규 집중 결과는 분리 집계하고
최종 합집합에서도 중복이 없음을 확인한다. selector의 0개 선택은 통과가 아니다.

각 실제 실행은 runtime/시험/프로젝트·검사 도구의 입력 manifest와 실행 전후 정확한
지문, 실제 selector, 예상 이름 원장 지문, 결과 XML/로그 경로·지문, 실제 프로세스
종료값과 passed/failed/skipped/inconclusive 수를 보존한다. 실패·건너뜀·판정보류나
누락·초과·중복·입력 지문 차이가 있으면 성공으로 집계하지 않는다. 536+74+신규의
실행 범위가 다른 기록을 합칠 때도 같은 동결 소스/입력의 증거를 확인한다.
설계 문서 통과는 컴파일·runner 포함·실행 성공·전체 수용 증거가 아니다. 이 두
보완 조건의 P1 폐쇄는 새 문서 지문에 대한 루나 재검수 뒤 아스트라가 결정한다.

아스트라의 시험 위치 보정 승인과 독립 검수 전에는 현재 허용 목록을 넓혀 시험을
추가하지 않는다. asmdef/friend/runtime/public API 변경, 실제 저장 경로 사용,
영수증 반사 제조, `Reflection.Emit`, 동적 대리 객체 및 다른 시험의 private factory
결합을 이 안은 허용하지 않는다.
