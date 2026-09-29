# C3 Q-B 프레임워크 가져오기와 기존 권한 감사의 후속 유지안

- 날짜: 2026-09-29
- 상태: 제안 — 아스트라의 별도 승인 전 기존 시험 수정 금지
- 설계자·실제 모델: 솔, `gpt-6-sol`
- 추적: `REQ-M5D7QC3-001/005/007`, `AC-M5D7QC3-001/005/009/010`,
  기존 `REQ-M5D7QB-001..005`, `AC-M5D7QB-004`

## 실제 충돌과 최소 선택

현재 `HubMenuIntentHandoffEditModeTests.cs`의
`AC_M5D7QB_004_PrefabAndRequestSurfaceAreClosed`는 Q-B 원본 파일 전체에 대해
`Does.Not.Contain("Run")`을 검사한다. 현재 Q-B의 두 private
`ConditionalWeakTable`을 위한 정확한 선언
`using System.Runtime.CompilerServices;`에는 `Run` 부분 문자열이 있다. 조회한
Q-B 원본에서 기존 금지 목록에 걸리는 유일한 항목은 이 두 번째 줄이다.
이는 실행 권한 호출이 아니지만 현 시험은 실제로 실패한다. 독립 실행은 하지 않았다.

Q-B source 조회 지문은 `C8217CF5455A85F833DAE6BE3E15DAD4A3AA82D96AB6A070667F2AD38328D17B`이며
작업 중 관찰 지문이지 승인·최종 동결 지문이 아니다. 변경 전 기존 시험 지문은
`FC7DD9F902C9783036137CE68423D43A28C00DE187DA125FDB8C41CDCF79B421`이다.
시험은 현재 로컬 HEAD tree에 없으므로 Git 과거 파일만으로 역사 보존을 대신할 수 없다.

최소안은 정확한 한 줄 가져오기 선언만 권한 감사 입력에서 구별하고 나머지 전체
텍스트에 **원래 부분 문자열 금지 목록과 동일 assertion**을 유지하는 것이다.
단어 경계 정규식이나 일반 namespace/comment 제거는 하지 않는다. 이 변경은
Approved C3의 기존 시험 수정 허용 목록에 없으므로 별도 제한 보정 승인이 필요하다.

## 다른 소스 구조의 비교

- 완전 한정 형식명도 `System.Runtime.CompilerServices`를 포함해 같은 감사 실패를
  일으킨다. Unicode 표기, 문자열/namespace 조각 분리나 별칭으로 숨기는 방식은 금지한다.
- CWT를 새 확인 소유자 파일의 helper로 옮기면 Q-B의 private 실제 발급 등록부를
  다른 형식으로 옮기거나 접근 함수를 늘려야 한다. 순환 소유와 외부 접근 표면을
  늘리는 변경이므로 정확한 선언 한 줄 구별보다 큰 설계 변경이다. 새 assembly나
  public 접근이 없는 구현도 가능할 수 있으나 이번 승인된 private Q-B 발급 소유를
  그대로 보존하는 최소안은 아니다.
- Dictionary로 교체하면 weak-key 수명과 보존 특성이 달라진다. 자체 weak-key
  저장소 구현도 동시성/수명 검증 범위를 키운다. 단지 감사 문자열 통과를 위해
  등록부 구현을 바꾸지 않는다.

권고안은 런타임·assembly·public 권한을 바꾸지 않고 시험 입력의 정확한 프레임워크
선언만 구별하는 후속 시험 유지다. 게임플레이·프로필·durable 권한 금지는 유지한다.

## 제안하는 제한 승인 보정

아스트라가 Approved C3의 허용 목록에 다음 범위만 추가하는 보정을 승인해야 한다.

1. 기존 `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffEditModeTests.cs`
   수정은 해당 감사 메서드의 **Q-B source 입력 한 군데**에 엄격한 후속 검사 helper를
   적용하는 것으로 제한한다. prefab/request field 검사, presenter transfer 범위,
   원래 forbidden 배열·`Does.Not.Contain` 반복 본문, 모든 행동 시험은 보존한다.
2. 새 집중 파일
   `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffC3AuditSuccessorTests.cs`
   및 `.meta`에서 같은 시험 assembly의 비공개/내부 helper와 별도 엄격 후속 행을
   추가한다. 원래 감사 입력을 바꿨다는 사실을 명시하며 원본 시험 그대로의 실행을
   주장하지 않는다. 다른 기존 시험·asmdef·friend·런타임은 수정하지 않는다.
   새 행은 새 `HubMenuIntentHandoffC3AuditSuccessorTests` class에만 두고 기존 class의
   메서드 이름·개수는 바꾸지 않는다. 기존 편집 회귀 562개 selector가 Q-B 원래 class
   전체를 선택하므로 이 방법으로 기존 562개 이름 목록을 보존한다. 신규 class는
   별도 정확한 집중 selector/예상 이름 원장으로 실행하고 기존 562개 결과와 구분한다.
   실제 합집합에는 이름 중복·누락이 없는지 확인하며 신규 행을 기존 562개에 섞어
   예상 개수만 바꾸지 않는다. 기존 메서드 입력은 바뀌므로 이전 562개 실행 결과를
   현재 통과로 재사용할 수 없고 같은 이름 목록을 새 소스로 실행해야 한다.
3. 수정 전 기존 시험 전체 바이트를 `docs/evidence/c3-qb-audit-predecessor/` 아래
   `.txt` 증거로 보존하고 위 SHA-256을 검증한다. 해당 감사 본문의 원본을 후속
   증거에 함께 기록한다. Q-B의 기존 Verified 검수/실행 지문은 당시 역사로 보존하며
   현재 작업 지문으로 덮어쓰지 않는다. 이력 증거 파일을 새 현재 source로 간주하지 않는다.
4. 새 정확한 시험/source 지문, 이 보정 승인과 독립 검수의 출처를 연결하고 집중 및
   필수 회귀를 새 동결 소스에서 수행한다. 과거 Q-B 통과나 C3 설계 폐쇄로 대체하지 않는다.

## 엄격한 후속 입력 규칙

helper는 현재 Q-B source 전체를 입력받는다. 물리 줄을 읽어 정확히
`using System.Runtime.CompilerServices;`인 줄이 **한 개**이고 namespace/type 선언
이전의 일반 가져오기 영역에 있는지 확인한다. CRLF/LF 줄 구분만 동일하게 처리한다.
공백 변형·별칭·`global using`·주석·전처리 영역·namespace 내부 선언을 허용하려고
정규화하지 않는다. 정확한 줄이 없거나 둘 이상이면 실패한다. 이 가져오기 영역에는
현재 승인된 평범한 using 선언과 공백만 허용하고 다른 코드를 섞으면 실패한다.

그 정확한 한 물리 줄만 제외한 텍스트에 기존 forbidden 배열의 부분 문자열 검사를
그대로 적용한다. 범용 `Replace("System.Runtime.CompilerServices", "")`를 사용하지
않는다. 실행 본문·형식명·주석에 나타난 `Run` 또는 같은 namespace는 그대로
검사 대상이다. 후속 행은 현재 전체 source에 helper와 금지 목록을 직접 적용하며,
두 CWT가 private 등록부로 남는지 실제 형식/필드 표면을 추가 확인한다. 호출 가능한
게임플레이 API의 의미 검증을 이 문자열 검사 하나가 완전히 증명한다고 주장하지 않는다.

## 필수 음성 행

- 정확한 선언 1개인 현재 source는 후속 입력 규칙을 만족해야 한다.
- 같은 source의 본문에 `Run`, `RunSession`, `Profile`, `ProfileResetDiskTransactionV1.Begin`,
  `SceneManager`, `Application.Quit`, `Settings`, `Wardrobe`, `CIO`, `CUA`, `InputSystem`,
  `UnityEvent`, `delegate` 각각을 추가한 행은 모두 거부한다. 주석 위치도 검사에서
  삭제하지 않으며 원래 금지 토큰을 우회하는 형식명 허용은 없다.
- 정상 선언에 더해 본문의 완전 한정 `System.Runtime.CompilerServices` 참조를
  추가하거나 같은 선언을 중복한 행, namespace 내부/전처리 아래로 이동한 행,
  별칭/global/공백 변형은 거부한다. 선언 앞뒤로 실행 코드를 붙인 줄도 거부한다.
- 기존 prefab/request 표면 및 presenter transfer의 모든 금지 assertion은 그대로
  실행하고 기존 Q-B 행동 시험을 유지한다.

권고 시험 이름은 `AC004_ExactFrameworkImportHasOneStrictSuccessorException`,
`AC004_GameplayAndDurableTokensRemainForbiddenAfterImportExclusion`,
`AC004_ImportCopiesAliasesAndNonHeaderLocationsAreRejected`,
`AC004_PredecessorAuditBytesAndForbiddenBodyRemainEvidenced`다.

이 문서는 소스/시험 수정이나 실행을 수행하지 않았다. 아스트라의 보정 승인과 루나
독립 설계 검수 전에는 기존 감사 실패를 건너뛰거나 통과로 기록하지 않는다.
