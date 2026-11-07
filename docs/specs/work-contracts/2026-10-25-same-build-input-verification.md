# 동일 Windows 릴리스의 입력전용 무인 검증

- 상태: Approved — 2026-10-25, Astra `gpt-6-astra`. 정확 범위·대체 예산·수용 경계는 `preparation/ch1-current-source-delivery-20261025/astra-same-build-scope.md`의 통합 판정을 따른다.
- 권위: ADR-0044/0045, Approved 자율 CH1 전달 계약 및 사용자2026-10-25 동일빌드 검증 요청. 기존 캐논·전투·저장·입력 소유·필수 회귀·실제 창 관측을 축소하지 않는다.
- 담당: 지정 Sol 제품4파일 단독쓰기, Root 실행·증거 소유, Luna 독립검수, Astra 범위·결과 통합판정. Unity 실행 중 제품쓰기0이다.

## 정확 제품 범위

기준은 output/chapter1-playable-r1/project/Assets/AcadeGameMaker/다. 신규 Runtime/Input/Unity/Chapter1VerificationInputPilot.cs 및 .meta, Runtime/Run/Unity/Chapter1SameBuildVerificationDriver.cs 및 .meta 정확4파일만 추가한다. 기존630파일·asmdef·friend·패키지·장면·prefab·저장 API·빌드기호 변경0이다.

**REQ-AUTOCH1-SAMEBUILD-001:** 최종 비개발 Windows 릴리스에 포함된 운전기는 정확 --ch1-verify=branch1, branch2, failure-retry 중 하나가 명시된 실행에서만 작동한다. 이 명시 옵션을 REQ-AUTOCH1-006의 검증용 개발 실행 구성으로 한정 정의한다. 기본 실행에서는 운전 객체·장치·구독·파일쓰기·자동입력을 생성하지 않는다. 미지원/중복옵션은 검증실패로 종료하며 검증메뉴나사용자게임흐름을추가하지 않는다.

**REQ-AUTOCH1-SAMEBUILD-002:** 운전기는 소유 가상 Keyboard/Mouse를 공개 InputSystem.AddDevice/QueueStateEvent/RemoveDevice 및 자연 onBeforeUpdate/onAfterUpdate 경로로만 다룬다. InputSystem.Update, Physics.Simulate, 수동 Update/FixedUpdate, 시간배율/실장치/OS설정 조작0이다. 원래 실장치를 비활성화하거나 바인딩·기본생산입력설정을바꾸지 않는다. 예외·성공·기한종료 때 중립 입력과 소유 구독/장치를 정리한다.

**REQ-AUTOCH1-SAMEBUILD-003:** 기존게임내부 API는상태관측에만사용한다. 플레이어HP/위치·적·피해·보상·기술·선택·진행단계·저장상태쓰기,생산핸들러직접호출,장면생성/로드/활성화,강제실패/성공/무적은0이다. 운전 객체 자체의 생명주기·검증증거쓰기·자기실행종료는예외적으로허용한다. 실제기본출정 입력을쓰고생산제공seed/계획/RoomId순서를읽는다. 릴리스에서꺼진검증seed메뉴활성화나seed주입0이다.

**REQ-AUTOCH1-SAMEBUILD-004:** 양분기는자연부팅→중립/기본출정→새runKey/세대→실제4방·적대응·이동/점프/대시/조준/공격/전이→오르단실제전투→유담해당선택제안/확인과정상저장→봉쇄선의전이/선택기술과실제통과→세령각종결줄의중립/60UI틱후새입력→생산완료영수증/분기저장을관측한다. 발행입력과실제소비완료tick/장치상태/이동/적HP등관측을분리기록하며상태주입을정상완주로표시하지않는다.

**REQ-AUTOCH1-SAMEBUILD-005:** failure-retry는새프로필에서초기HP양수와실제적의공격/피해증거를관측하고공격하지않는입력전략으로자연HP0→기대AcceptedFailureKind→정상저장revision/결과→실패복귀UI→거점복귀입력→중립→재출정의다른runKey/새세대·첫방실제완료tick을인과순서로연결한다. HP0요청/실패API호출/저장주입0이다. 증거접근이없거나인과를못관측하면실패/미검증으로남긴다. 낙사·압사기존회귀를이한사례로대체하지않는다.

**REQ-AUTOCH1-SAMEBUILD-006:** 원시는실행파일부모의전용검증증거디렉터리내최대512KiB/1024행,입력·관측행은주요변화/최대주기만기록한다. 사례당30분·권위진전없음60초가상한이다. 진전은실제생산phase/방/세대전이,목적방향의이전최선위치를넘는이동,현재적실제HP감소/실제처치,전이·봉쇄선영수증,선택저장revision,실제종결줄/완료영수증이다. 틱·프레임·입력발행만으로진전시계를리셋하지않는다. 준비/중립/60UI틱대기·정리·화면관측용중립최대15초는단계별정당대기로기록하되60초/전체30분기한을늘리지않는다. 예외/원시초과/폐쇄실패는명시실패다.

**REQ-AUTOCH1-SAMEBUILD-007:** 같은최종비개발빌드전체(실행파일·UnityPlayer·Data/Managed·의존파일)를전수지문고정한뒤사례별신규작업공간복사본으로전수일치검증한다. 저장은그위치에서자연SaveData/Chapter1을생성한다. 기존저장복사·이식·교체·실사용프로필변경0이다. 검증중생성된저장/증거는빌드바이트와분리보존하며원본배포폴더는불변이다.

## 수용 기준과 실행 묶음

**AC-AUTOCH1-SAMEBUILD-001:** 정확4제품파일추가/기존630불변·기본비활성/잘못된옵션거절·입력해제/정리/기한/원시상한·금지상태변경0을국소검사/독립코드검수한다. 릴리스에NUnit/TestRunner/시험조립체를추가하지않는다.

**AC-AUTOCH1-SAMEBUILD-002:** 같은최종릴리스바이너리의branch1/branch2/failure-retry3사례가REQ004/005의실제입력·완료·저장인과를만족해야한다. 단독플레이어정상초기화/자연InputSystem/실제물리·전투를사용하며별도진단빌드나테스트플레이어로대체하지않는다. 같은최종버전에서전사례를통과해야하며중간버전성공을합산하지않는다.

**AC-AUTOCH1-SAMEBUILD-003:** 같은최종빌드옵션없는기본실행에서CUA의지원된일반마우스·키로초기UI→출정→월드→자연실패후잔상없는UI→거점→재출정을관측한다. 정확창의현재상태/제목·앱·소유프로세스신원/기한을결속하며창ID를PID로추정하지않는다. 지속키미지원옵션/숨은OS입력0. 읽기전용자동운전원시·실제창영상/화면·CUA입력·독립검수·사람체험을구분하고자동입력을사람완주나OS지속입력으로표시하지않는다.

**AC-AUTOCH1-SAMEBUILD-004:** 정상경로저장revision·선택/완료분기·중복0·자연실패보존·재출정새세대를독립대조하고모든원본/복사본빌드지문동일을확인한다. 실제C1C/CH1필수회귀와기존AC를완화하지않는다.

이계약이Approved가되면같은묶음에서다음한정예산을제안한다. 기존630focused4실행은실제완료한기준선으로별도보존하고새634검사로간주하지않는다. 이전첫승인문에서아직미소비인extended/build는아래최종소스검사/빌드와중복실행하지않도록이계약으로대체한다.

- Sol신규4파일구현1묶음/해당4파일내원인보정최대2묶음. 도구/증거출력은 preparation/ch1-current-source-delivery-20261025/와 output/ch1-current-source-delivery-20261025/이다.
- 최종634의정확10PlayMode(기존focused4+extended6)1회1800초,해당4파일원인보정뒤동일선택재검사1회1800초. 무변경재시도0. 기존시험의GUID격리저장과정확Unity파생목록보존권위는 execution-plan.md를유지한다.
- 최종검사통과뒤비개발Windows빌드최대2회/각1800초. 기존BuildWindows의새attempt-001/필요시새attempt-002만사용한다. 이전출력덮어쓰기0·선행미통과빌드0.
- 동일최종빌드자동3사례/각1800초,원인보정새최종빌드후전사례재검증최대3회/각1800초. 외부소유감시·회수30초/폐쇄10초. 기본실행CUA최대2회/각300초,캡처총24·click총12·일반키총8,첫실행에서대상접근/바인딩미도달일때만두번째허용. 별도잘못된옵션거절플레이어1회30초를허용하며자동완주로계산하지않는다.
- 공식 require_escalated 문맥의소유실행기·정확Editor/게임경로동시성·기존Hub/라이선스클라이언트보호·공개CIM부모/출생확인을유지한다. 동기호출선점·미관측전자손포획한계는명시하고신원불명/회수실패는후속중단한다.
- 테스트/최종빌드/검증실행중제품쓰기0. source/manifest/배포전체핀은실제최종버전에결속한다. 기능묶음의Luna독립검수1회와영향재검수1회,Astra최종통합으로종료한다. 사용자중간조작·인증/약관/보안설정대리0이다.

처음선택10검사,같은빌드기본UI/양분기/실패재시도및독립수용을실제로확인하기전 actualWindowAcceptance/presentationClearAccepted/finalAcceptance를올리지않는다.
