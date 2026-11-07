# 사용자 중간 조작 없는 CH1 개발·검증·Windows 전달

- 상태: Approved — 2026-10-21, Astra `gpt-6-astra`.
- 권위: 최신 사용자 위임, ADR-0044, 기존 CH1 전달 계약 §17/22/25 및 C1-C 한정 구현 Approved. 전체 캐논과 8챕터 승인은 확대하지 않는다.
- 문서 단독 소유: Astra. 제품·시험 구현 단독 소유: 지정 Sol. 독립 검수: Luna. Root는 실행·증거·통합 작업을 조정한다.
- 현재 수용: 계획과 아래 개발 권한만. 실제 CH1 완주·입력 해결·최종 빌드는 아직 미수용.

## 1. 사용자 산출물과 기존 경계의 호환

**REQ-AUTOCH1-001.** 최종 산출물은 실행 가능한 Windows 폴더/압축본, 한글 시작·조작·저장·재시도 안내, 파일 지문과 알려진 제한이다. Editor·관측 도구·시험 버튼·개발 장면 전환 없이 CH1 양분기가 종결된다. 준비 상태나 보스 단독 구간을 최종 산출물로 제출하지 않는다.

**REQ-AUTOCH1-002.** 새 경로에서 사용자 중간 승인·최초 Play·키 입력·중립 보고·진단 버튼·반복 준비를 요구하지 않는다. Root/Sol은 승인된 정확 게임 대상의 로컬 시작/정지/입력/재시도와 공개 Unity 시험 API를 사용할 수 있다. 기존 사용자 전담 입력 규칙은 새 경로에 한해 이 조항으로 대체한다. 이전 R01/G3 실행은 소진·실패·종료 상태 그대로 보존하고 재사용하지 않는다. r1 진단은 불변이며 r2 미소비 여부는 실제 기록으로 남긴다. 새 시험에 옛 고정 진단 파일을 재발급하는 절차를 요구하지 않는다.

**REQ-AUTOCH1-003.** 실제 OS 보안·인증·권한창의 대리 처리는 금지한다. 배치 검사/단독 빌드/공개 API 대안을 사용한다. 보안창이 활성인 상태에서 그 뒤 게임에 입력을 보내지 않는다. 방화벽 변경·계정 로그인·권한 우회·외부 게시·Q0 키·실사용 저장 변경은 범위 밖이다. 실패나 환경 차단을 숨기지 않고 해당 증거만 미완료로 남기며 충돌 없는 개발을 계속한다.

## 2. 기능 묶음과 담당

| 묶음 | 산출물·담당 | 실제 선행과 종료 |
|---|---|---|
| A 입력·자동 검사 | Sol: 현재 v24 입력/성능 원인 확인, 공개 시험 진입과 격리 입력/저장, Root: 실행 | 원래 실행 종료·정확 변경 목록. 실제 입력→행동·초점·실패/정리 및 비용 근거 |
| B CH1 연결 | Sol: 기존 Approved B/C 생산 연결과 누락 저작/종결 보완 | 실제 기존 파일/API/승인 차이표. 양선택·기술·봉쇄선·서사·중복 방지·실패 복귀 |
| C 전달 | Sol: Windows 빌드/안내, Luna: 독립 같은 빌드 검수, Astra: 통합 | A/B와 영향 회귀. 양분기 완료 및 실패 재시도·재실행 검증, 패키지 지문 |

**REQ-AUTOCH1-004.** Sol은 기존 Approved C1-C 문서와 현재 구현을 읽어 재사용/누락/충돌을 정확 파일·소유 API·REQ/AC 표로 제출한다. 이미 승인된 파일과 행동은 그 범위 안에서 이어서 구현할 수 있다. 새 시험 진입·입력 연결·저장 격리·빌드와 미승인 제품 차이는 이 문서의 한 변경 표로 Astra가 추가 승인한다. Review 전체를 일괄 승인하거나 디렉터리 전체 편집 권한으로 해석하지 않는다. 사용자 재질문 없이 내부 판정을 진행한다.

허용된 선행 산출물은 `preparation/autonomous-ch1-20261021/` 아래 Sol의 현재 구현 차이표·설계·시험 계획과 결과, `docs/verification/autonomous-ch1-20261021/` 아래 Luna 검수와 Root 통합 증거다. 신규 파일은 덮어쓰기 전 기존 소유와 상태를 확인한다. 동결 증거·대형 원장은 스캔하지 않는다. 제품 변경 전 정확 경로 표가 필요하며 별도 승인 사본은 만들지 않는다.

## 3. 검증과 증거

**REQ-AUTOCH1-005.** 검사 순서는 순수 국소 검사→Unity 배치 컴파일/공개 시험→단독 Windows 화면 입력→같은 최종 빌드 양분기·실패/재시도다. 실제 설치 도구와 공개 API의 지원 범위를 먼저 확인한다. 입력 시험은 실제 사용 API·가상/물리 장치·창 대상·시간·행동 결과를 기록한다. 상태 직접 주입, 무적/순간이동/보상 강제 설정을 정상 생산 완주로 기록하지 않는다. 생산 경로 완주와 격리 단위시험을 명확히 나눈다.

**REQ-AUTOCH1-006.** 시험 전용 제어는 명시적 개발 구성에서만 활성화한다. 배포 기본 실행에서 자동 운전/시험 메뉴/강제 진행이 꺼져 있음을 검사한다. 저장은 새 작업공간 전용 프로필로 격리하며 실제 사용자 프로필과 이전 시험 원문을 덮어쓰지 않는다. 제품 저장/선택/완료 소유권·실제 피해·세대·필수 순서·캐논 경계는 유지한다.

**REQ-AUTOCH1-007.** 기존 필수 회귀의 각 항목을 동일 의미로 실행/영향 재사용/새 경로에서 대체 검증으로 매핑한다. 현재9회나 양분기 조건을 이름만 지우지 않는다. 사람 3명 분량 검수·과거 Q0 인증은 별도 미완료로 명시하되 새 로컬 개발·전달의 인간 선행으로 삼지 않는다. 독립 Luna는 구현자의 결과를 그대로 승인하지 않고 실패 원문·빌드 지문·실제 게임 흐름을 확인한다.

## 4. 유한 실행 예산과 지속 권한

**REQ-AUTOCH1-008.** 묶음별 기본 예산은 국소 검사 1묶음과 원인 보정 후 2묶음, Unity 컴파일/시험 최대3회(각20분), Windows 빌드 최대2회(각30분), 양분기·실패/재시도 최초3사례와 원인 보정 후 최대3사례(각30분), Luna 독립 검수1과 영향 재검수1이다. 정확 선택 사례·명령·파일/빌드 핀·PID/시작시각·제한시간·종료 결과를 단일 실행 원장에 기록한다. 이미 실행 중인 기한을 연장하거나 과거 예산을 복원하지 않는다.

Root는 한 묶음 승인과 최종 정적 검수 뒤 개별 핀마다 Astra 응답을 기다리지 않고 허용된 실행을 계속할 수 있다. 실패 시 같은 입력을 무변경 반복하지 않는다. 같은 오류 두 번 또는 단회 사례 첫 실패면 그 사례를 닫고 최소 재현·원인·변경·새 관측 목적을 남긴다. 예산 밖은 Astra가 동일 문서에서 내부적으로 좁은 연장을 판정하며 사용자 반복 승인을 요구하지 않는다. 정식 도구 심사·실제 권한은 유지한다.

## 5. 수용 기준

| AC | 실제 완료 조건 |
|---|---|
| AC-AUTOCH1-001 | 사용자 중간 조작 요청 없이 자동 검사와 빌드가 진행되고 각 실행의 실제 결과/제한이 보존됨 |
| AC-AUTOCH1-002 | 정상 릴리스 입력 경로의 이동·점프·조준·전이·전투·초점/재시도 검증, 입력 미수신을 사용자 무행동으로 단정하지 않음 |
| AC-AUTOCH1-003 | 같은 최종 빌드의 시작/거점/원정/보스/양선택/각 기술/봉쇄선/CH1 종결, 실패와 재시도가 실제 이어짐 |
| AC-AUTOCH1-004 | 저장·로드·재시도에서 선택/보상/완료 중복0, CH2 잠금과 캐논 유지, 실제 사용자 프로필 변경0 |
| AC-AUTOCH1-005 | 영향 회귀와 독립 Luna 검수 완료, 자동/실제 창/사람 증거 구분, 남은 중대 기능 결함0 |
| AC-AUTOCH1-006 | 실행파일·의존 파일·한글 안내·최종 지문 제공, 시험 제어 기본비활성, Editor 없이 시작·결과·재시도 가능 |

통과 전 정상 플레이 가능·CH1 완료·SourceReady 승격을 선언하지 않는다. 최종 결과 전달 후 사용자 체험은 개발의 반복 관문으로 사용하지 않는다.

## 6. 실행 환경·프로젝트 소유 확정 — Approved

Root의 실제 환경 조사에서 Unity6000.6.0f1, WindowsStandaloneSupport, InputSystem1.20.0, TestFramework1.6 및 Mono가 확인됐다. 이번 묶음은 기존 `output/chapter1-playable-r1/project`를 단일 제품 프로젝트로 사용한다. 별도 복제 프로젝트를 기본 경로로 만들지 않으며 같은 프로젝트의 Unity 실행 중 Sol 제품 쓰기를 금지한다. Root가 실행 소유권을 받고 종료·로그 폐쇄를 확인한 뒤 Sol에 쓰기 소유권을 반환한다. 정확 최종 제품/메타/패키지 집합은 한 빌드 명세로 결속하고 과거 590행을 새 변경 뒤 불변이라고 주장하지 않는다.

Root는 실제 설치된 Unity의 공식 batchmode/runTests/executeMethod를 지원 범위 안에서 사용하며 시험 필터·명시적 메서드·입출력 경로·기한을 실행 원장에 고정한다. 출력은 `output/chapter1-autonomous-20261021/` 아래 build, evidence, profiles, logs로 분리한다. profile은 시험별 신규 하위폴더를 사용한다. 재사용 빌드 출력은 새 시도 폴더로 보존하고 기존 증거를 덮어쓰지 않는다. 읽기·구현계획 작성과 기존 Approved 변경은 즉시 진행할 수 있으며 새로운 실행 메서드의 정확 파일/API는 §2의 동일 변경 표에서 닫는다.

독립 Windows 빌드의 실제 창을 공개 컴퓨터조작 도구로 식별한 뒤 Root의 press_key 등 지원된 게임 입력을 허용한다. 이는 사용자 최신 위임에 따른 지정 게임 대상 시험이며 입력 후킹·숨은 OS API·보안창 뒤 입력을 허용하지 않는다. 키 눌림/해제와 초점·화면·행동 결과를 기록하고, 시험 종료 시 대상 게임의 입력을 해제한다. PlayMode 합성 입력과 실제 창 입력을 별도 검수한다.

최종 기본 실행에는 R01 관측기, 최초 표본 강제Pause, 개발 진단 버튼, 1790초 영상 종료 정책을 연결하지 않는다. 이는 생산 피해/저장/단회 보상 가드를 삭제하는 권한이 아니다. 시험 전용 관측과 제어를 분리하고 릴리스 구성에서 비활성/미연결임을 Luna가 검사한다. 녹화 도구·외부 보안 팝업을 매 시험의 선행으로 두지 않는다. 필요한 화면 증거를 확보하지 못하면 그 AC는 미완료로 남긴다.
## 7. 현재 v24 기준선 검사 — Approved

제품 쓰기가 없는 현재 구간에 Root는 기존 프로젝트의 설치된 TestFramework와 기존 시험 조립체만 사용하는 EditMode1·PlayMode1 기준선 실행을 허용받는다. 둘은 §4의 Unity 실행 최대3회 중2회를 소비한다. 각20분 상한, 순차 실행, 같은 프로젝트 동시 Unity0이다. 실제 설치 CLI 지원과 기존 시험 목록을 읽고 실행 전에 명시한 testPlatform/기존 assembly 또는 testFilter/결과 XML·로그 경로를 `output/chapter1-autonomous-20261021/evidence/baseline/`의 실행 기록에 남긴다. 새로운 시험코드·시험 장면·프로필 정책을 이 승인으로 작성하지 않는다.

시작 직전 현재 v24 제품/메타/패키지 집합과 설치 Unity 신원, 기존 Unity 부재, 시험이 실사용 프로필/Q0/외부 네트워크/고정 R01 진단에 쓰지 않는 범위를 확인한다. 안전한 기존 시험 필터를 선택할 수 없으면 해당 사례를 실행하지 않고 Sol 설계로 넘긴다. 공식 batchmode/runTests와 지원된 platform/결과 경로를 사용하며 새 래퍼 프레임워크를 만들지 않는다. 합성 입력이면 evidenceClass=synthetic, 실제 시험이 실행되지 않으면 skipped 또는 미관측으로 남긴다.

Root는 실제 시작 프로세스 객체/PID·시작시각·단조 시간 기한을 소유하고 타임아웃/오류에는 그 소유 프로세스와 확인된 자식만 정리해 종료·출력 폐쇄를 기록한다. 로그 침묵이나 exit0만으로 시험 통과를 판정하지 않고 실제 XML의 선택/실행/실패/건너뜀 수와 로그를 대조한다. 제품·씬 변경이나 공유 소유 충돌이 감지되면 후속 실행을 멈춘다. 출력은 신규 시도 경로에 보존하고 큰 원장/옛 r1·r2/옛 session을 소비하지 않는다. 동일 실패 자동 재실행0이며 결과는 기준선 검사에 한정한다. CH1 완주·Windows 입력·최종 빌드 수용을 대체하지 않는다.

Luna 사전 조사에서 지적한 G3 6방/10구성·집중35/회귀44·수동12·종단4/실제창·G8 최종 빌드의 개별 assertion은 Sol의 같은 변경/검사 표에서 합성/실제 OS 입력/재사용으로 정확 매핑한다. 독립 Luna가 동일 최종 exe를 실행·대조한다. 과거 Q0 키·서명·세대·원격 게시 및 동결 증거는 이 기준선으로 면제되지 않는다.
### §7 기존 RunUnity 시험 전용 저장 경로의 한정 호환

Root 읽기 조사에서 Chapter1DevelopmentProfile.ExactRoot는 저장소 내 `output/local-playable-slice-r1/profile/chapter1-development`로 고정되고 현재 해당 디렉터리가 부재다. 기존 RunUnity 시험은 이 경로의 자체 생성 leaf에 실패 주입과 정리를 수행한다. 기준선 실행 직전 정확 절대경로가 작업공간 안인지, 조상 비링크와 목표 부재를 다시 확인한 경우에만 해당 기존 시험의 자체 생성/쓰기/leaf 정리를 허용한다. 실제 사용자 저장으로 보지 않으며 사용자 백업·수동 리셋을 요구하지 않는다.

기존 데이터가 발견되면 덮어쓰기·삭제하지 않고 해당 필터를 제외한다. 허용은 이 실행이 새로 만든 경로와 항목뿐이며 다른 profile 디렉터리·부모 재귀 삭제를 포함하지 않는다. 종료 뒤 생성/잔존/정리 결과와 신규 파일 핀을 기록한다. 두 번째 시험에서 앞 실행의 잔존물이 있으면 동일 원장에 소유가 입증된 항목만 기존 시험 규약으로 다루고, 소유 불명확이면 실행하지 않는다. 최종 Windows 빌드의 별도 저장 격리 설계는 Sol 변경 표에서 확정하며 이 하드코딩을 최종 제품 저장 정책으로 승인한 것은 아니다.
### 기준선 실제 결과 및 공통 수명 오류의 조사 경계

Root 보고: EditMode Run21/21 통과·exit0/9.59초, XML SHA26D2FD70D8DC163C65B686D94FD381D3D1B54B33E1B75B41675A641F5DE4F8A7. 선택 PlayMode3/3 실패·exit2/11.08초, XML SHAE13F7D68959F5D4411D8E09F410464D4CBFD8396B098FD27DFAA84D7FD61479F. 최초 보고 경로는 ValidateHubUiOnlyLifecycle→CurrentReceipt→UiSemanticFrameCursorV1.Create→Chapter1ExpeditionPresenter.FixedUpdate이며 동일 메시지 `HubUIOnly lifecycle is terminal or unknown.`다. 이는 Root 실행 보고이며 이 절만으로 Astra가 XML 전체를 재검수했다고 주장하지 않는다. tool93843 회수·Editor/PID 부재·현재590 불변·개발 profile 부재 보고 뒤 제품 쓰기 소유를 Sol에 반환한다. Unity 예산3 중2 소비, 무변경 재실행0이다.

Astra 직접 소스 판독에서 Presenter는 CurrentHub 변경 시 cursor Create를 try 밖·Ready 검사 전에 호출한다. Router 오류는 허용된 준비 상태 외의 상태를 거절하므로 초기 준비 지연, 이전 소유자 노출, 종료/정리 중 호출을 원시 없이 단정하지 않는다. Sol의 정확 변경 표에 이 공통 수명 원인 조사와 최소 소비자 보정을 포함할 수 있다. 종료 상태 허용·검사 무력화·예외 삼키기로 정상처럼 진행하지 않으며 유효 소유자/준비 전이·퇴역 후 소비 금지와 실제 UI 입력 시험을 함께 검증한다.
## 8. CH1 공유 병합·자동 검증·배포 구현 묶음 — Approved

Astra는 Sol 설계 보완 `output/chapter1-playable-r1/preparation/chapter1-autonomous-delivery-route-sol61.v1.md` 14079바이트/SHA03CB0038699DDFAC4484278061B5A6A927E9992FF1A0B0EB4CD4E90E9417420B와 Luna의 사전 P1 여섯 항목, Root의 저장·입력 도구·순환 지문 제약을 반영해 아래 범위를 승인한다. 상태가 제안인 해당 초안을 구현 권위로 삼는 대신 이 절의 선택을 따른다. Root의 기준선 종료·제품 쓰기 반환 뒤 Sol은 즉시 공유 병합을 진행한다. 제품 단독 쓰기 Sol, 실제 실행 Root, 독립 수용 Luna, 최종 통합 Astra다.

### 8.1 정확 변경 경로

아래 제품 경로는 `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/` 기준이다. 각 소스의 기존 meta는 보존하고 신규 파일은 GUID 충돌 검사 후 meta를 한 번 만든다. 후보를 디렉터리째 덮어쓰지 않는다. 변경 전 원문은 `preparation/autonomous-ch1-20261021/before/Assets/` 아래 원래 상대경로로 CreateNew/닫힌 핀 보존하며 이미 자기 소유로 보존된 파일은 동일핀 확인 후 중복 생성하지 않는다.

| 구분 | 허용 경로와 의미 |
|---|---|
| C1-C 신규 | v5의 `Runtime/Core/ChoiceSkillContracts.cs`, `Runtime/Run/ChoiceSkillSession.cs`, `Runtime/Run/Unity/Chapter1CompletionReceipt.cs`, `Runtime/Input/Unity/Chapter1ChoicePhaseAdapter.cs`, `Runtime/Combat/Unity/Chapter1ChoiceEnvironmentOwner.cs`, `Runtime/Combat/Unity/BarrierCombatSimulationDriver.cs`, `Runtime/HubPresentation/Unity/Chapter1ChoicePresenter.cs`, `Tests/EditMode/Run/Chapter1ChoiceSkillSessionTests.cs`와 각 meta |
| 종결 신규 | v6의 `Runtime/HubPresentation/Unity/Chapter1ClosingPresenter.cs`와 meta; Assets 기준 `Chapter1/Narrative/Chapter1Closing.json`, `Prefabs/Chapter1/Closing.prefab`과 각 meta. 부재인 `Chapter1/Narrative` 폴더1과 후보 `Narrative.meta`는 기존 GUID 충돌0을 확인해 신규 생성 허용. 기존 부모 meta를 덮지 않음 |
| 생산 병합 | `Runtime/Run/Unity/Chapter1PlayableFlowOwner.cs`, `Chapter1RunView.cs`, `Chapter1DevelopmentProfile.cs`; `Runtime/Input/Unity/InputRouter.cs`, `Chapter1EncounterLifetimeOwner.cs`, `DesktopProfileLaunchAdapterV1.cs`; `Runtime/Transfer/Unity/TransferSimulationDriver.cs`; `Runtime/Transfer/TransferSession.cs`; `Runtime/HubPresentation/Unity/Chapter1ExpeditionPresenter.cs` |
| 방·저작 | `Runtime/Run/Unity/Chapter1RoomInstance.cs`, `Chapter1RoomCatalog.cs`; `Editor/CombatAuthoring/Chapter1ExpeditionAuthoringBuilder.cs`, `Chapter1ExpeditionAuthoringValidator.cs` |
| 시험·빌드 신규/병합 | v5 `Tests/PlayMode/RunUnity/Chapter1ChoiceBarrierPlayModeTests.cs`, `Chapter1ClosingPersistencePlayModeTests.cs`와 meta; 현재 `Tests/PlayMode/RunUnity/Chapter1ExpeditionPlayModeTests.cs`; 신규 `Editor/CombatAuthoring/Chapter1AutonomousBuild.cs`와 meta, 신규 `Tests/PlayMode/RunUnity/Chapter1RoomProductionAcceptanceTests.cs`와 meta |
| 조립 참조 | `Runtime/Input/Unity/AcadeGameMaker.Input.Unity.asmdef`의 정확 Run 참조; `Tests/PlayMode/RunUnity/AcadeGameMaker.Run.Unity.PlayMode.Tests.asmdef`의 Run/Transfer.Unity/Combat.Unity/Movement.Unity/Hub.Presentation.Unity 참조. 기존 항목 재사용·중복/순환 참조0 |
| friend | `Runtime/Core/AssemblyInfo.cs`의 Input.Unity/Transfer.Unity, `Runtime/Run/AssemblyInfo.cs`의 Input.Unity, `Runtime/Input/Unity/AssemblyInfo.cs`의 Run.Unity/Run.Unity.PlayMode.Tests, `Runtime/Combat/Unity/AssemblyInfo.cs`의 Run.Unity.PlayMode.Tests만. 실제 경로가 없으면 동명 파일을 추정하지 말고 현재 해당 조립체의 AssemblyInfo 경로를 기록해 같은 목록의 경로만 정정 |

자산은 Assets 기준 현재 `Scenes/Chapter1Playable.unity`, `Prefabs/Chapter1/Hub.prefab`, `R01.prefab`~`R06.prefab`, 현재 오르단 참조 prefab, 신규 `Prefabs/Chapter1/Choice.prefab`·`Barrier.prefab`, 위 Closing/서사 및 해당 meta, `UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset`와 그 기존 종속 아틀라스에 한정한다. 실제 씬·오르단 자산 경로는 현재 builder의 정확 상수/GUID에서 읽어 변경 명세에 고정한다. 새 추정 경로·기존 승인 밖 콘텐츠를 추가하지 않는다. ProjectSettings/EditorBuildSettings.asset의 CH1 장면 목록 한정 갱신을 허용한다. 신규 폴더 meta는 위 정확 신규 대상의 필요한 부모에만 허용한다.

### 8.2 생산 행동과 수명

**REQ/AC-AUTOCH1-009.** Flow가 실제 보스 정리 결과의 ChoiceRequired/BarrierPreparationRequired를 승인 C1-C adapter에 연결하고 Suppressed는 완료로 승격하지 않는다. 같은 run/generation·원본 UI token·저장 receipt·실제 문/발판/출구·Closing 마지막 새 edge/원본 pending receipt를 검증한 뒤 기존 TryCommitChapterCompletion만 사용한다. 새로운 완료 플래그·직접 피해/위치·저장 성공 대입0. 선택 기술 port는 기존 단일 capture와 Prepare/Validate/Commit/Cancel 소유를 유지한다. v24 입력·원본 wrapper/콜백·재개 및 현재 안전가드를 무관하게 덮어쓰지 않는다.

**REQ/AC-AUTOCH1-010.** Presenter는 실제 현재 허브 소유·준비 publication·퇴역 상태에 따라 cursor를 생성/폐기한다. 준비 전 또는 terminal router에 getter를 시험 호출해 준비 여부를 추정하지 않는다. 필요한 내부 읽기 전용 readiness 투영은 위 Router/Adapter/허브 소비 경계에서 추가할 수 있으며, `Runtime/Input/Unity/Chapter1HubLifetimeOwner.cs`를 이 투영과 소유 확인에 한정해 추가 허용한다. 중립 frame 소비·held/old edge 거절·단일 callback bind/unbind·실제 terminal 거절은 유지한다. 예외 삼키기나 guard 삭제0. Root가 직접 확인한 현재 Hub proof1을 인정하며 Sol의 proof 누락 주장은 철회된 오판으로 보존한다. 현재 builder를 통한 정확 schema 검증/저작은 허용하지만 원인으로 확정하지 않는다.

### 8.3 저장·빌드·표시

**REQ/AC-AUTOCH1-011.** persistentDataPath 제안은 채택하지 않는다. 최종 portable Windows 빌드 기본 저장은 실제 실행파일 부모의 `SaveData/Chapter1`이다. 인증 adapter가 계산한 정규화 절대경로를 Profile이 동일 비교한다. 부모/목표 비링크·범위 검증, 기존 원자 저장·복구·V1 pair 규칙을 유지하고 다른 사용자 저장 fallback/삭제0. 쓰기 불가면 설명 가능한 오류로 종료 또는 안전 메뉴를 제공하며 임의 다른 폴더로 옮기지 않는다. 시험 override는 시험 구성에서만 작업공간 `output/chapter1-autonomous-20261021/profiles/<runId>`로 제한하고 릴리스의 임의 명령행 경로를 허용하지 않는다. 기존 baseline 하드코딩 경로는 역사 시험과 분리한다.

공개 `AcadeGameMaker.Combat.Authoring.Chapter1AutonomousBuild.BuildWindows()`는 현재 CH1 씬을 명시해 Windows64를 빌드하고 BuildReport 성공/실제 exe·의존 파일/지문을 확인한다. 기본 출력은 `output/chapter1-autonomous-20261021/build/windows/AcadeGameMakerChapter1.exe`이며 기존 결과가 있으면 새 attempt 하위폴더를 사용한다. 런타임 default 저장도 해당 새 exe 부모에 따른다. 기존 출력 삭제/덮기0.

릴리스에는 R01 관측기·자동 운전·시험 장치/API/장면·개발 seed101~404 버튼·자동Pause·영상 종료 정책을 넣지 않는다. 개발 조건 분리와 실제 버튼/의도 목록을 함께 고친다. 승인 폰트의 전체 버튼/안내/대사 문자 집합과 누락 ‘검’을 포함해 SDF를 갱신하고 실제 렌더 확인한다. raw production-v1 원화 시트를 완성 sprite로 오인해 가져오지 않는다. 사용 가능한 승인 미디어만 재사용하고 나머지는 기존 허용의 구분 가능한 단색 기하·한글 역할/상태로 표시하며 임시 표현임을 최종 안내에 명시한다. 정상 플레이 기능을 미완성 아트 제작에 묶지 않는다.

### 8.4 방 수용과 순환 없는 증거

**REQ/AC-AUTOCH1-012.** 6방10구성은 현재 생산 입력 경로의 합성 InputSystem trace로 이동·점프·대시·소켓·출구·실패를 관측한다. 각 구성의 요구 행동은 시험 manifest에 사전 고정하며 비적용 행동을 true로 채우지 않는다. 측정기는 catalog를 승인하지 않는다. Root가 원문을 폐쇄하고 Luna가 실제 source/prefab/입력·runId·case 결과를 검수한 뒤 Astra가 동일 묶음에서 수용한 명세만 편입한다.

새 `chapter1-room-production-acceptance.v1.json`의 최상위는 `schemaVersion,runId,inputKind,sourceManifestSha256,cases`로 고정하며 inputKind=`synthetic-input-system`이다. case는 `roomId,socketId,prefabSha256,startTick,endTick,sampleCount,entered,moveReached,jumpObserved,dashObserved,socketObserved,completed,unexpectedFailureCount,evidenceSha256`로 닫는다. 해당 manifest가 행동 적용 여부와 생산 source/geometry 원문 집합을 정의한다. ID정확10·누락중복0·정수시각·실제 표본/위험/완료·추가키 거절을 검사한다. 측정원문에 reviewer 승인을 자체 발급하지 않는다.

별도 편입 명세는 측정 원문 SHA·원래 시험 source/geometry manifest SHA·Luna 판정 파일 SHA·Astra 승인 절 ID·정확 변경할 acceptance metadata 목록을 가진다. 편입 시 바꿀 수 있는 것은 RoomInstance의 기존 수용 필드와 이 명세를 결속하는 최소 파생 필드뿐이다. 시험 전 prefab 지문과 편입 후 파생 prefab 지문을 다르게 기록하며, 편입 후 지문이 과거 시험 입력과 같아야 한다는 순환 조건을 만들지 않는다. 기하·생산 코드·게임 값 변경은 metadata로 분류하지 않으며 영향 사례를 다시 검증한다. 최종 빌드 전체 manifest는 따로 고정한다. 기존 인간 증거 변조·측정기가 flag를 직접 true로 만드는 우회0.

### 8.5 검사와 지속 보정

**REQ/AC-AUTOCH1-013.** 네 Ignore 시험 `UiSubmitClickHeldCancelAndDuplicate`, `BothBranchesCloseOnlyAfterRealPassageAndSave`, `RejectForgedIncompleteOrUncertainCompletion`, `RestartPreservesV1PairButNotChapterReceipt`를 실제 생산 씬/저장/원본 receipt 경로로 구현한다. 필수 선택 검사 skipped0이며 Assert.Ignore 제거만으로 통과시키지 않는다. 기존 baseline3 실패를 보존하고 같은 원인 정상/음성 국소 사례를 포함한다. 기존 G3/집중35/회귀44/수동12/종단4/9회 등 요구는 항목별 같은 assertion과 새 실행 방법을 명세에 매핑한다.

OS 도구는 현재 단일 press/chord만 지원하므로 장시간 keydown/up을 숨은 API로 만들지 않는다. 전체 양분기는 생산 DLL/assets 지문이 결속된 Unity StandaloneTestplayer의 합성 입력 trace로 검증하고, 최종 시험코드 없는 ship exe는 공개 OS tap/클릭으로 시작·초점·키 반응·화면·실패/재시도·종료를 독립 검증한다. 동일 source의 시험용/릴리스 바이너리 차이와 실제 커버리지를 기록한다. 이를 사람 완주나 OS 장시간 완주로 표기하지 않는다. 이 한정 조합을 새 로컬 전달의 검사방법으로 채택하며 실행 불가능/미검증 동작은 완료로 채우지 않는다.

§4 기본 예산 중 baseline2회는 이미 소비됐다. 새 통합 코드에는 추가 Unity 컴파일/집중 검사 최대3회(각20분), 실제 원인 보정 후 추가2회(각20분)를 이번 묶음으로 명시 허용한다. Windows 빌드2회/각30분, 최초3종단 사례+원인 보정후3사례/각30분, Luna1+영향1을 유지한다. 도구 출력/결과 XML·실제 exit·PID/회수·건너뜀을 기록한다. 같은 오류 무변경 반복0·두 번이면 최소 재현 후 같은 문서에 원인/변경을 남긴다. 위 파일/행동 안의 국소 보정은 새 승인 사본이나 사용자 질문 없이 계속하고 인터페이스·외부 경로 확대만 내부 Astra 판정으로 닫는다. 구현 완료 전 Root Unity 실행0, 쓰기 반환·현재 핀/필수 국소 검증 뒤 Root는 이 예산 안에서 실행할 수 있다.
### §8 최초 저장 부모 준비·정확 공유 API 명확화

Root 조사에서 기존 ProfileResetDiskTransactionV1.VerifyBeforeLease가 부재 부모의 File.GetAttributes를 호출하고 adapter가 IOException을 복구 차단으로 바꾸는 경로가 확인됐다. 실제 baseline 전체 인과는 추가 대조 대상이나 새 portable/시험 root도 부모 부재를 정상 첫 실행으로 처리해야 한다. 허용된 DesktopProfileLaunchAdapterV1 안에서 인증 root를 확정한 뒤 저장소/실행파일 부모 경계·각 기존 조상 비링크를 확인하고 정확 `SaveData/Chapter1` 또는 시험 runId의 필요한 부모만 생성한 후 기존 reset/lease 검사를 호출하는 한정 준비를 허용한다. 기존 파일/링크 충돌·권한 오류는 거절하고 다른 경로 fallback0, 공유 ProfileResetDiskTransactionV1의 안전검사 완화0이다. 최초 부재·정상 재실행·파일 충돌·링크·쓰기 실패 국소 검사를 포함한다.

§8.2의 기승인 API 병합은 Router의 TryReadChapter1ClosingControlsNeutral/ClosingHost, Lifetime의 정확 closing host 발급·소유·정리/새 UI세대, Transfer의 pending choice context·단일 capture와 원자 회수 후보, Flow의 ValidatePendingChapter1ClosingStart/CurrentChapter1CompletionReceipt 및 원본 demo/save/closing 완료 통지를 포함한다. 실제 현재 publication/원본 token만 사용하며 읽지 않은 중립이나 완료를 만들어 내지 않는다. 이는 새 별도 구현 대기를 요구하지 않는 §17/22/25의 정확 공유 연결 범위다.
### §8 독립 실행형 시험의 편집기 의존 분리·G7 예산 명확화 — Approved

Astra는 아래 세 asmdef의 includePlatforms 빈 배열과 그 조립체 소스의 비조건부 UnityEditor/AssetDatabase 사용, RunUnity/Chapter1RoomPlanTests.cs의 같은 의존을 직접 확인했다. Root의 설치 TestFramework PlayerLauncher 조사에 따르면 IncludeTestAssemblies 빌드는 testFilter만으로 타 시험 소스 컴파일 제외를 보장하지 않는다. Sol은 Assets/AcadeGameMaker/Tests/PlayMode 기준 아래 정확4개 파일만 추가 변경할 수 있다.

- CombatUnity/AcadeGameMaker.Combat.Unity.PlayMode.Tests.asmdef
- HubPresentation/AcadeGameMaker.Hub.Presentation.Unity.PlayMode.Tests.asmdef
- InputUnity/AcadeGameMaker.Input.Unity.PlayMode.Tests.asmdef
- RunUnity/Chapter1RoomPlanTests.cs

세 asmdef의 includePlatforms를 정확 ["Editor"]로 지정하고 나머지 참조/설정/시험 의미를 유지한다. Chapter1RoomPlanTests.cs의 편집기 전용 using 및 해당 시험 클래스를 UNITY_EDITOR 조건으로 한정한다. 기존 Editor PlayMode 검사는 계속 실행 가능해야 하며 시험 삭제/Ignore로 성공 대체0이다. RunUnity의 독립 실행형 양분기·실패/재시도 시험은 포함하고, 편집기 전용 방 계획 검사의 assertion은 기존 Editor 검수와 런타임 방/계획 검증에 추적한다. Standalone 포함/제외 목록과 선택 실행/건너뜀 결과를 실제 빌드에서 확인한다. 전체 시험 조립체를 광범위하게 Editor로 옮기는 권한은 없다.

G7 정상1+실패3종(HP0·낙하·압착)의 필수 사례를 줄이지 않기 위해 §4/§8의 최초 종단3사례 예산은 이 묶음에서 최초4사례로 정정한다. 원인 보정 후 최대3사례·각1800초는 유지한다. 정상의 양분기 검사와 각 실패의 실제 producer/정리/재도전 assertion을 검사 명세에 배치하며 합성 강제 상태와 실제 생산 실패를 구분한다. 나머지 실행/빌드 예산은 변경하지 않는다. 단일 원장에 소비를 기록하며 사용자 재승인·문서 사본 반복0이다.
### §8 상자 전이 생산 컴포넌트 분리 — Approved

Astra가 현재 TransferSandboxOwnerSinks.cs를 직접 판독했다. TransferSandboxBoxOwnerSink는 다중 클래스 파일 안에 있고 ConfigureForTests만 제공한다. 실제 Unity 직렬화 실패를 이미 관측했다고 주장하지 않으며, 정확 파일명/생산 저작 API로 프리팹을 구성하기 위해 신규 `Assets/AcadeGameMaker/Runtime/Transfer/Unity/Chapter1CounterweightOwnerSink.cs`와 meta를 허용한다. Sol 단독 소유로 기존 ITransferTargetModifierSink 계약의 동일 상자 행동을 분리한다. 기존 sandbox 원문과 시험 사용처를 덮어쓰지 않는다.

Target.Box.Heavy.v1·정확 targetId/실제 Rigidbody2D 원본·중복 적용 거절, 적용 직전 mass/gravityScale 보존→mass×3/gravity×2·impact multiplier1500, Clear에서 원래 값·1000 복원·중복 clear 무동작을 유지한다. 새로운 피해/HP·위치·속도·시간·소유권을 만들지 않는다. ConfigureForAuthoring(targetId,body)는 비활성 프리팹의 단회 구성, 비어 있지 않은 정확 ID와 같은 저작 상자의 body를 검증하고 런타임 재구성하지 않는다. Builder/Validator/기승인 상자 검사에서 새 실제 컴포넌트 참조를 사용한다.

검사는 실제 생성 프리팹의 스크립트/필드 참조 roundtrip, 정확 요청·오류 ID/수정자·중복 적용, 적용/clear 복원과 기존 생산 전이/회수 경로를 포함한다. 이는 §8 국소 보정/검사 예산 안이며 별도 대규모 실행이나 임의 물리 상태 주입 권한을 추가하지 않는다. 신규 meta GUID 충돌0·변경 명세·최종 빌드 지문을 유지한다.
### §8 시험 직접 참조·필수 실행 단계 예산 확정 — Approved

Astra는 현재 RunUnity 시험 asmdef와 TMP_Text/Combatants/MovementActionState 사용을 직접 판독했다. 허용된 `Tests/PlayMode/RunUnity/AcadeGameMaker.Run.Unity.PlayMode.Tests.asmdef`의 references에 정확 `Unity.TextMeshPro`, `AcadeGameMaker.Combat`, `AcadeGameMaker.Movement` 세 직접 의존을 중복 없이 추가한다. 새 조립체/friend/생산 역참조0이며 기존 includePlatforms와 시험 의미는 유지한다. 실제 공개 타입/출판 검사를 위한 컴파일 의존이고 읽기 불가능한 내부 상태를 우회 노출하는 권한이 아니다.

필수 저작·모드별 검사를 검증되지 않은 CLI 혼합이나 누락으로 축소하지 않기 위해 §8의 신규 Unity 정상3+보정2는 다음 정상6+원인 보정 후 최대2로 대체한다. 각 실행1200초 상한, 순차·단일 소유다. 이미 수행한 baseline2는 별도 소비 이력으로 남고 복원하지 않는다.

1. 현재 씬·프리팹 한정 갱신 저작 및 컴파일.
2. 새 허브 준비와 방10구성 생산 측정.
3. 독립 방 편입 수용 뒤 G4 정확35 및 C1-C 추가 집중 검사.
4. G5 EditMode 해당 고정 집합.
5. G5 PlayMode 해당 고정 집합.
6. StandaloneTestplayer 검증: 계획12 및 정상/실패4·양분기 요구를 논리 사례별로 분리 집계.

같은 mode에서 필요한 시험은 정확 ID/필터를 고정해 한 실행으로 묶되 선택/실행/통과/실패/건너뜀과 기존 AC 매핑을 각각 기록한다. Root가 실행 직전 실제 코드/자산·명령/필터·출력·기한·소유를 원장에 결속하며 개별 핀 재승인 대기는 요구하지 않는다. source 쓰기 반환 전 Unity0, 동일 오류 무변경 재실행0, 첫 실패 원시 보존·원인/변경/관측 목적을 유지한다. 정적 검사에서 차단되면 실행 예산을 임의 소비/복원하지 않는다.

Windows 빌드 총2는 Testplayer1과 Ship1로 명시한다. 빌드 자체 각1800초, 위6번 시험 실행은1200초 안에 계획하고 실제 각 종단사례도 기존1800초를 넘지 않는다. 총6번 시험이 기한 안에 담기지 않으면 시작 전에 실제 사례/측정 근거로 같은 계약의 추가 분할을 판단하며 실행 중 기한을 늘리지 않는다. 빌드나 시험 성공을 서로 대체하지 않는다. 과거 Q0의9선택은 별도 역사적 Q0 관문이며 CH1 9개 시험/이번 실행 선행으로 재활성화하지 않는다.
### §8 순수 C1-C 경계 검사의 정확 시험 friend — Approved

Astra가 실제 asmdef의 이름 `AcadeGameMaker.Tests.EditMode.Run`·Editor 전용/TestAssemblies와 ChoicePhaseContext 내부 생성자 및 기존 ChoiceSkillSessionTests의 미완성 Ignore5를 직접 확인했다. 기존 허용 `Assets/AcadeGameMaker/Runtime/Core/AssemblyInfo.cs`에 이 정확 조립체의 InternalsVisibleTo1을 추가한다. 가능하면 UNITY_EDITOR 조건에 두어 릴리스 Core의 friend 선언에서도 제외한다. 다른 생산 조립체/공개 생성자/반사 접근을 추가하지 않는다.

같은 허용 Chapter1ChoiceSkillSessionTests에서 격리 context를 구성해 age20/21/22, cooldown299/300·419/420, 예약35/36/37, hold59/60, platform149/150 및 copied/foreign/overflow 등 실제 순수 경계를 검증할 수 있다. fixture는 evidenceClass=synthetic이며 직접 만든 context를 실제 driver-issued publication으로 표시하지 않는다. 기존 Ignore5의 전체 의미 중 UI·실제 저장·생산 callback·원본 receipt/capture 요구는 해당 실제 PlayMode/종단 시험에 정확 추적하고 순수 검사만으로 수용하지 않는다. 단순 Ignore 삭제·검사 없는 pass0이며 현재 국소/독립검수 예산 안에서 구현한다. 생산 context 발급·pending 검증·단일 capture·저장 권한은 변경하지 않는다.
### §8 실제 Csc의 두 시험 접근 누락 보정 — Approved

Root 정상 호스트 authoring-r2에서 발생한7개 Csc 오류를 원문으로 보존한다. Astra는 실제 `CombatSession.cs`의 internal CombatantSnapshot, `PlayerMovementController.cs`의 internal Snapshot, 정확 RunUnity 시험 asmdef 이름을 직접 확인했다. 기존 `Assets/AcadeGameMaker/Runtime/Combat/AssemblyInfo.cs` 및 `Assets/AcadeGameMaker/Runtime/Movement/Unity/AssemblyInfo.cs`에 `UNITY_EDITOR || UNITY_INCLUDE_TESTS` 조건의 `InternalsVisibleTo("AcadeGameMaker.Run.Unity.PlayMode.Tests")`를 각각1개 추가하는 최소 보정을 승인한다. 정확 두 파일의 전후핀을 기록하고 기존 선언을 보존한다.

이는 기존 생산 publication/완료 snapshot을 시험에서 읽는 접근이다. 생산 타입 공개화·새 생산 참조·순수 Movement friend·피해/물리값 변경·직접 body 조작·추가 tick 주입을 허용하지 않는다. friend가 기술적으로 내부 전체 접근을 열더라도 시험의 승인된 사용은 이 읽기 관측으로 제한한다. 실제 Testplayer 컴파일에서 해당 조건이 효력을 갖는지 확인하고 릴리스에서는 선언 제외를 확인한다. 심볼 이름만으로 빌드 성공을 주장하지 않는다.

Sol은 Root의 Unity 종료·쓰기 반환 뒤 원문 보존/두 선언/참조 폐쇄/현재 명세핀 갱신을 같은 보정 묶음에서 수행하고 Root에 반환한다. 새 실제 실행은 기존 예산의 원인 보정 실행으로 기록하며 무변경 재실행0이다. 제한 환경의 첫 crash와 정상 호스트 Csc 실패는 별개 실패로 보존하고 실제 컴파일 미완료를 저작 성공으로 표시하지 않는다.
### §8 독립 방 편입 실행의 분리

방10 측정 원문 폐쇄→Luna 독립 검수→Astra 수용 뒤 기존 승인 `ApplyAutonomousRoomAcceptance()`가6개 prefab의 허용 수용 metadata를 저장하는 별도 Unity 실행1을 정상 단계에 추가한다. 측정기가 자기 승인하거나 executeMethod와 runTests를 미검증 혼합하지 않기 위한 분리다. 정상 계획은 저작 / 준비·방10 / 독립 편입 / G4·C1-C / G5 EditMode / G5 PlayMode / Testplayer의7회, 각1200초로 확정한다. 독립 편입 전에는 명세·검수·승인 미완료를 true로 채우지 않으며 제품 기하/생산 코드 변경0을 대조한다.

원인 보정2회는 늘리지 않는다. 제한 환경 실패 r1 및 정상 호스트 Csc 실패 r2의 실제 소비 기록을 보존하며 다음 보정 실행의 사용 여부는 Root 원장의 기존 계산에 따른다. 새로운 원인이 추가되면 실제 원시·원인/최소 변경·새 관측 목적을 근거로 같은 문서에서 판정하고 실행 중 기한 연장/예산 복원0이다. Windows Testplayer/Ship 총2와 다른 제한은 그대로다.
### §8 저작 모드 물리 장면 API 차이의 보정·추가 저작1 — Approved

Root 보고 authoring-r3는16.85초/exit1·PID121968 종료·로그폐쇄이며 Csc 오류0·글리프340 추가·Choice.prefab 생성 뒤 ValidateInitialBarrierSight의 SceneManager.CreateScene(LocalPhysicsMode.Physics2D)가 Editor 저작 모드에서 거절됐다. 로그 SHA250E45FB1F72A21F602416B1CE04AEDF024EF9483F38FAED1D95E60160E29646 및 부분 저작 산출물을 보존한다. Astra는 해당 함수가 정적 layer8의 활성 비trigger BoxCollider를 복제해 raycast 후 finally CloseScene하는 원문을 직접 확인했다. 실제 지원 API 문제를 시야 검사 통과나 제품 gameplay 실패로 확대하지 않는다.

Sol은 이미 허용된 Chapter1ExpeditionAuthoringValidator.cs의 이 함수에 한정해 설치 버전에서 지원되는 공개 Editor 장면/물리 조회 API를 확인하고 정적 검사 격리·정리 경계를 보정한다. 실제 player/weight 위치·layer8·활성 collider·trigger 제외·방향/최대거리·막힘이면 실패라는 조건을 보존한다. 원본 생산 graph 활성화·물리 simulation·게임 값 변경·전역 physics 설정 변경·시야 검사 생략0이다. 다른 열린 장면의 collider가 검사에 섞이지 않도록 조회 대상/소유를 검증하며 격리 실패는 거절한다. 지원성 근거와 변경 전후 정적 기하 동일성·정상/막힘 음성·예외 정리를 국소 확인한다. 새로운 API를 지원된다고 추정해 실행하지 않는다.

기존 보정2 소비를 복원하지 않고 이 새 구체 원인의 보정 후 추가 저작1/1200초를 허용한다. Root의 현재 source626 및 실제 허용 자산 전후 차이를 명세에 보존하고, 소유 반환·현재핀·Unity 부재 확인 뒤 새 시도 경로에서 정상 호스트 실행한다. 기존 글리프/Choice 생성 원문을 삭제하거나 실패 기록을 성공으로 바꾸지 않는다. 새 관측 목적은 지원된 시야 검사 실제 정상/거절과 전체 저작 종료 확인이며 무변경 재실행·실행 중 기한 연장0이다. 다시 실패하면 그 원시와 최소 원인부터 정리한다.
### §8 최초 봉쇄선 시야의 제한된 정적 기하 검사 — Approved

앞 절의 지원된 Editor 물리 조회 후보를 이 정확 함수에 한해 다음 정적 기하 방식으로 대체한다. NewPreviewScene의2D격리를 추정하지 않는다. Sol은 ValidateInitialBarrierSight 안에서 실제 barrier 소유 BoxCollider2D의 layer8·enabled·비trigger 필터를 그대로 읽고 생산 객체 활성화·임시장면 생성·Physics2D.SyncTransforms/Simulation·전역 물리 설정 변경 없이 검사한다. 이번 추가 저작1 예산을 유지하며 더 추가하지 않는다.

지원 기하는 현재 저작된 평면 XY·회전 identity·월드 배율(1,1,1)·edgeRadius0의 상자뿐이다. 정확 transform/부모 누적 변환, 양의 유한 size, 유한 offset/position, origin/aim과 양의 유한 선분 길이를 검증한다. 지원 밖 기하를 축정렬로 근사하지 않고 명시적으로 거절한다. 지원 기하의 실제 world center=position+offset 및 size/2로 폐구간 AABB를 만들고 double slab 방식으로 원래 origin→aim의 t∈[0,1] 선분 교차를 판정한다. 축 방향0은 나눗셈 없이 해당 축의 포함 여부로 처리한다. 모든 계산의 유한성/범위를 검사하며 distance/시야 통과를 추정 상수로 채우지 않는다.

상자 내부 시작·경계 접선·끝점 접촉을 포함한 교차는 보수적으로 막힘으로 거절한다. 이는 모든 Physics2D 설정의 raycast와 완전 동일하다는 주장이 아니라 현재 제한 기하의 저작 시 열린 시야 보장 검사다. 해당 생산 물리 시야/법선·거리·목표 원본은 후속 실제 생산 입력/전이 검증에서 계속 확인한다. blocker가 없다고 collider나 필터를 제거해 성공시키지 않는다.

국소 검사는 실제 helper에 대해 열린 선분/정면 막힘/평행 바깥·안쪽/접선·끝점/내부 시작/음수 방향/축 방향0/거리 바깥 및 NaN·무한·0size·회전·배율·edgeRadius 지원 밖 거절을 포함한다. 현재 저작 origin/aim·각 blocker의 실제 변환/size/offset와 판정 결과를 기록한다. 생성 자원0을 명시하고 기존 시야 검사 목적·생산 가드를 유지한다. 검증 후 Root는 이미 승인된 추가 저작1을 현재 핀과 새 관측 목적으로 실행할 수 있다.
### §8 부분 저작 글꼴의 멱등 갱신 보정·추가 저작1 — Approved

Root 보고 authoring-r4는11.18초/exit1·로그폐쇄 SHA29E0FD3EFE28ADDB1DB7807EED36F8426CDCBC5B0E072F9939BAD1F7DE5CA93E이며 새 LOS 도달 전에 UpdateChapter1Font가340자를 모두 missing으로 반환했다. 부분 font CharacterTable372/Static/sourceFont 참조0 관측과 이전 실행 글리프 추가 사실을 보존한다. Astra 직접 판독에서 함수가 기존 coverage 확인 없이 Dynamic 전환·SerializedObject 원본 지정·전체 TryAddCharacters를 반복함을 확인했다. FontEngine 원인 세부는 설치 TMP의 실제 API/실행 근거로 확인하며 추정 성공으로 채우지 않는다.

Sol은 허용 Builder의 해당 함수에 한정해 현재 요구 문자 집합→실제 font character/glyph/atlas 결속 검증을 먼저 수행하는 멱등 갱신으로 보정한다. 단순 CharacterTable 개수나 문자 키 존재만으로 렌더 가능한 coverage를 선언하지 않는다. glyph index/유효 atlas 참조·직사각형 범위 등 실제 글리프 자료를 검증하고, 모두 포함되면 불필요한 TryAddCharacters를 호출하지 않는다. 누락된 문자만 승인 원본 OTF와 설치 지원 API를 통해 추가하며 원본font 결속 실패·아틀라스 용량 부족·누락 잔존은 정확히 실패시킨다. fallback font/글자 삭제/문구 축소/누락무시0, 기존 정상 글리프·부분 자산 원문을 보존한다. 공백 등 비가시 문자 처리는 TMP 실제 규칙으로 구분하며 임의 그림 면적을 요구하지 않는다.

재로드 후 동일 요구집합 coverage와 원본/atlas핀을 재대조하고 실제 한글 UI 렌더 검증을 후속 수용 조건으로 유지한다. 정상 이미충족·실제누락·원본없음/참조실패·손상 glyph/atlas·두 번째 실행 불필요 변경0을 국소 확인한다. SerializedObject 설정이나 TryAdd 반환 하나만으로 성공을 판단하지 않는다.

기존 모든 실패/보정 소비를 보존한 채 이 별도 원인 보정 뒤 추가 저작1/1200초를 승인한다. 새 관측 목적은 이미 추가된 글꼴의 재사용/실제 누락 보완과 다음 저작 단계의 진행이다. Sol 쓰기 반환·현재 핀·정상 호스트·Unity 부재 확인 후 Root가 새 시도 경로에서 실행한다. r4 실패·부분 자산·이전 LOS 미도달을 유지하고 무변경 재실행·기한 연장0이다. 독립 QA/양분기/최종 렌더 및 완주 조건은 면제하지 않는다.
### §8 방 측정 첫 실패의 원인 분리 진단·재관측1 — Approved

Root 보고 room-measurement-r1은160.47초/exit2·선택2실패·소스변경0·로그폐쇄 SHAC1328BBC2488D7D6488E328C73B3ABA0D796F6E62C6AF51D02D99E10882BD349다. 허브 오류가 XML TearDown에 기록된 사실과 방 handoff drift를 분리한다. 입력 fixture 복원 뒤 활성 router의 map 검사 가능성, 정상 Lightweight의 gravity 변화 가능성은 원시 old/current가 없는 현재 가설이며 확정하지 않는다.

추가 허용 제품 경로는 정확 `Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyEncounterHandoffAdapter.cs` 하나다. Astra가 현재 retained/current 전체 Equals와 BodyState의 gravity 포함 비교를 직접 확인했다. 이 파일은 실패 예외의 진단 정보만 보강한다. 최초 불일치에 원본/현재 ID·참조 동일 여부, size/offset/scale/rotation 및 bodyType/simulated/constraints/gravity/interpolation/fullKinematic 중 실제 다른 필드를 이름과 이전/현재값으로 기록한다. 기존 읽기 가능한 실제 PlayerModifier/Transfer publication의 유무·신원·tick을 결속하고 없으면 미관측으로 남긴다. 진단 getter가 새 guard 예외나 상태 변경을 일으키지 않도록 이미 캡처된 값/안전한 기존 투영을 사용한다. 새로운 friend/생산 API 없이 불가능한 필드는 미관측으로 둔다.

기존 전체 Equals/거절 시점/범위와 모든 collider/body/원본 소유 가드는 이번 진단에서 변경하지 않는다. 정상 gravity 허용은 실제 원인 확인 뒤 별도 정확 보정으로 판정한다. 예외 로그는 최초 실패1·최대8192UTF8바이트, 유한 필드만 허용하며 매tick 출력0·원시 배열 전체 덤프0이다.

기존 허용 시험 `Chapter1ExpeditionPlayModeTests.cs`, `Chapter1RoomProductionAcceptanceTests.cs`에 각 사례의 시작/준비/첫입력/완료/원래실패/teardown 단계·Unity frame/fixedtick/단조경과 및 입력 fixture 복원·graph정리의 실제 순서를 유한 기록한다. 사례당 최대16행·각4096UTF8바이트다. 최초 실패를 정리 오류로 덮지 않는다. 정리 순서를 명시적으로 graph중지/씬Unload완료→fixtureRestore로 옮기는 한정 보정은 허용하되 실제 완료를 확인하고 원래 실패를 무시하지 않는다. LogAssert 무시·시간기반 가짜 통과·준비 가드 삭제0이다.

이 원인 분리를 위한 정확 기존 두 대상 재관측1/1200초를 추가 허용한다. 원문 실패와 기존 모든 소비를 보존하며 동일 핀 무변경 재실행이 아니라 위 진단/정리 변경·새 관측 목적을 명세에 결속한다. Sol 쓰기 반환/현재핀·Root 단일 실행 소유 뒤 수행하고, 다른 실패가 나타나면 그대로 기록한다. r5 저작 성공은 보존하지만 방/허브 성공이나 방 수용flag를 발급하지 않는다.
### §8 정상 경량화의 중력 검사 호환·방 재관측1 — Approved

Root 진단r2 보고는166.02초/exit2·로그폐쇄 SHAE6B1C6F9E395C54BE57AE1C993124CA9414CFAFCC913ACB69ADBA3C5D4ABA758, 허브 Passed125.65초·정리 map 오류0, 방 Failed25.18초다. 실제 불일치 필드는 player.gravity 하나로 old3.1315/current2.035475·transferTick320/PlayerStateLightweight/revision1이다. directPlayerModifier는 미관측으로 보존한다. Astra 직접 소스 확인에서 Movement의 기준 상수3.1315f와 Lightweight×0.65f 및 Baseline 복원 규칙, handoff의 실제 동일tick publication 검증을 확인했다.

정확 RegularEnemyEncounterHandoffAdapter.cs에서 player gravity만 영구불변 비교 대신 검증된 동일 원본 Transfer publication의 PlayerState에 따른 값 검사로 대체한다. Baseline은3.1315f, Lightweight는 동일 float연산3.1315f*0.65f로 고정한다. 관측 current에서 기준값을 역산/갱신하지 않는다. 상태enum 미지원·publication없음/옛tick/다른owner·원래graph불일치는 거절한다. CaptureExactSources의 기존 ValidateEncounterHandoffPublication 및 source exact 재대조를 통과한 해당tick 상태를 두 preflight 검사에 결속하고, 중간 상태 변경도 거절한다. 실제 Movement modifier를 직접 읽었다고 주장하지 않는다.

player의 나머지 bodyType/simulated/constraints/interpolation/fullKinematic, 모든 참조·size/offset/scale/rotation, surveyor/walker 전체 body/gravity·collision pair·기하 검사는 그대로다. player gravity를 비교에서 빼고 검증 없이 진행하지 않는다. 최초 관측부터 상태에 맞는 실제gravity를 확인하며 정상 적용·복원만 허용한다. 기존 최초불일치 진단을 유지한다. 시험은 Baseline→Lightweight→Baseline 정상, 임의gravity·wrongstate·stale/missing/foreign publication, targetgravity/다른기하 변경 거절을 포함한다. 필요한 순수 판정 helper는 같은 파일 내부로 제한하고 생산 Movement/Transfer 수치·공개권한은 변경하지 않는다.

허용된 Chapter1ChoiceBarrierPlayModeTests와 Chapter1ClosingPersistencePlayModeTests의 fixture 정리도 graph중지/씬정리 완료→InputTestFixture 복원 순서로 동일 한정 보정한다. 실제 최초 실패와 후속 정리 오류를 분리하고 LogAssert 삼키기·검사 삭제0이다.

원인 보정 뒤 방 대상 재관측1/1200초를 추가 허용한다. 허브 성공은 같은 소스 영향분석에 한해 재사용하며 정상 방 측정 전체 성공·수용명세를 앞당겨 발급하지 않는다. 기존 실행·실패·소비를 보존하고 변경/국소검사/현재핀·Root 소유 반환 뒤 새 시도 경로로 실행한다. 같은 원인 무변경 반복0, 새 실패는 실제값으로 보고한다.
### §8 방 시험 입력 전략의 폐루프 보정·재관측1 — Approved

Root 보고 room 재관측r3은48.30초/exit2·로그폐쇄 SHA32C46C49D87A918233AD3D93C05D893C920E335A2B7ADC3EB93AFA52ECFF7027, 중력가드 오류0이며 실제 R01-A trace는 tick0/x0/HP5/적2에서 tick774/x34.38/y2.06/HP0/적2로 끝났다. 기존 입력 전략이 적을 제거하지 못한 채 횡단했다는 관측을 보존한다. 게임 자체 무적/피해/이동 조정이나 검사 통과를 뜻하지 않는다.

Sol은 허용된 Chapter1RoomProductionAcceptanceTests.cs 및 Chapter1ExpeditionPlayModeTests.cs의 기존 시험 입력 trace를 실제 현재 생산 publication에 반응하는 유한 전략으로 보정한다. 실제 player/적 위치·생존/공격거리·지형·플레이어 HP·현재 입력/행동 상태를 읽어 접근·조준·공격·회피·필요한 전이/회수·적 처리 뒤 횡단을 결정한다. 실제 카메라 투영으로 포인터 입력을 만들고 키/버튼의 down/up edge와 중립 frame을 정상 InputSystem 이벤트로 공급한다. 적 HP·위치·완료·문/소켓 상태 직접 대입, teleport, 무적, 시간/난이도 변경, collider비활성, 생산callback 직접 성공 호출0이다. 숨은 상태를 실제 시야처럼 주장하지 않고 시험이 읽은 publication/행동과 evidenceClass=synthetic를 기록한다.

전략에는 사례별 시작 tick/단조시간 기준으로 최대3000 완료tick 및60초 무진전 감시를 둔다. 같은 행동 반복도 이 한도 안이며 목표 적/구간 전환 시 전체 사례 기한을 초기화하지 않는다. 전체 실행1200초가 우선이다. 사망/위험/입력경로 오류·출판 소유불일치면 현재 사례 실패를 보존하고 자동 새게임/재시도로 성공을 덮지 않는다. 생존·적 처리 여부·구간 진행을 실제값으로 기록하며 목표 좌표/행동 예산은 실행 전 시험 명세에 고정한다. 10구성 전체 및 기존 movement/jump/dash/socket assertion은 줄이지 않는다.

같은 러시 trace를 재사용하는 C1-C 시험에 영향 분석을 적용하고 동일 허용 helper를 공유할 수 있다. 각 양분기/봉쇄선 목표·생산 receipt 조건은 유지한다. 공격 성공·적 제거·도달의 독립 기준을 국소 검토하며 입력 전략 변경을 생산 보정으로 기록하지 않는다.

원인 결속 보정 후 방10 측정 재관측1/1200초를 추가 허용한다. 기존 r3 실패/CSV·모든 소비를 보존하고 Sol 쓰기 반환·현재핀·전략 명세 뒤 Root가 새 시도 경로로 실행한다. 반복 실패 시 최소 사례와 실제 공격/조준/피해 관측부터 확인하며 무변경 큰 검사 반복0이다. 방 수용명세/최종 플레이 수용은 실제 통과·독립 판정 전 발급하지 않는다.
같은 정리 순서 보정은 기승인 `Tests/PlayMode/RunUnity/Chapter1RoomPlanTests.cs`의 RoomExitRequiresActualCompletion fixture에도 적용한다. 실제 graph중지/씬Unload완료 뒤 InputTestFixture 복원을 수행하며 Editor 조건·미완료 출구 거절·독립 RejectUnvalidatedRoom의 빈 fixture와 의미는 유지한다. 추가 생산파일/검사 삭제/별도 실행 예산을 허용하는 조항이 아니다.
### §8 실제 이동 규칙에 맞춘 상태 보유 시험 입력·재관측 최대2 — Approved

Root 보고 room-measurement-r4는52.49초/exit2·로그폐쇄 SHA87CBE6A8785D87F4EC14B2E748E20CF3074DC04F7DF194D2CF6AADBEFF3DDAAE다. R01-A는338tick에 통과했고 R02-A는913tick에 사망했다. 해당 CSV rooms-524a3483a9744c16914e520af44a3f44의 말미 x9.658/y1.619/state2/SurveyorHP6와 D 유지, 벽에서 점프 새 입력이 없는 관측을 보존한다. Astra가 실제 PlayerMovementMotor를 읽어 상승 중 JumpReleased가 속도를45%로 줄이고 airborne의 새 JumpPressed와 유효 wall이 벽점프를 시작하는 규칙을 확인했다. 게임 전체 실패 원인을 이 입력 결함 하나로 단정하지 않는다.

Sol은 기승인 Chapter1ExpeditionPlayModeTests.cs의 Chapter1ProductionRoomPilot 및 이를 쓰는 Chapter1RoomProductionAcceptanceTests.cs 안에서 시험 입력 상태만 보정한다. Space는 최대24 완료tick 유지 뒤 최소2 완료tick 해제하여 새 edge를 만들며, 실제 grounded/wall/이동 snapshot 및 저작 Terrain/Floors를 근거로 이륙·벽점프를 선택한다. airborne 진입만으로 즉시 해제하지 않는다. 실제 같은 벽 잠금과 정상 모터 조건은 유지하고 입력 상태를 생산 완료 상태로 대신하지 않는다. 각 호출이 아니라 실제 완료tick으로 유지/해제 예산을 센다.

Sol의 여섯 방 기하 사전 대조를 시험 명세에 보존한다. R01 평지, R02 단차, R03 계단, R04 발판과 틈, R05 천장·주기 압착, R06 틈·단차에 대해 실제 원본 기하 및 현재 publication으로 정책을 고정한다. R04/R06의 실제 Floors 틈 가장자리 전방1.5m 이륙, R02/R03/R06의 전방 단차 또는 실제 wall 기반 새 점프를 허용한다. R05는 실제 firstTick에 결속된 기존 IsClosedAt의 현재부터45tick까지 열린 구간을 읽어 x10.5 앞 대기 후 정상 D/dash로 통과하고 x9.5..16.5 천장 구간의 불필요한 점프를 억제할 수 있다. 이는 저작 주기를 읽는 합성 시험 정책이며 인간 반응 또는 관측된 미래 상태로 표기하지 않는다. 계산은 실제 생산 시간·주기·상태를 진행하거나 바꾸지 않는다. 지원 기하/원본/tick이 불명확하면 추정 성공 대신 실패 정보를 보존한다.

allowJump=false인 의도적 낙하·압착 시험에는 새 안전 회피를 적용하지 않는다. 기존 적 처리·정확 포인터/공격 edge·이동/jump/dash/socket assertion,3000 완료tick/60초 무진전/120초 사례 및 전체1200초 제한을 유지한다. HP·위치·속도·피해·충돌·시간·수용flag 직접 쓰기0, 새 생산 API/숨은 입력 API0이다. 같은 원본 입력 helper의 유지/해제/벽 새 edge/틈/천장/주기 경계 및 의도적 실패 분기를 국소 검사하고 실행 입력 명세와 실제 입력/전후 publication CSV를 결속한다.

이 원인 결속 보정 관측을 최대2회, 각1200초로 한정 승인한다. 첫 실행 전 Sol 쓰기 반환·국소 결과·현재 핀·Root 소유를 확인한다. 두 번째는 첫 결과의 구체 원인과 변경·새 관측 목적을 같은 원장에 기록한 경우에만 허용하며 무변경 재실행0이다. 같은 원인이 두 번 나오면 큰 검사를 더 반복하지 않고 최소 사례·원시 입력/기하부터 분리한다. 모든 기존 실패와 소비를 보존하고 방10 전체 수용·최종 완주를 실제 측정/독립 검수 전에 발급하지 않는다.
### §8 방 측정의 필수 대시 입력 명시·재관측1 — Approved

Root 보고 r6은42.41초/종료2·닫힌 로그 SHA33324001B0E8F0B52454C54EF666C6C20F7770ABA8F57298DEC56A88DCD907ED다. R01-A340tick 통과, R02-A369tick 적 제거·이동·점프·소켓·출구 완료 및 사망0이나 dashObserved=false로 필수 assertion에서 실패했다. R02 전체 수용은 발급하지 않는다. 기존 대시의 주기적 입력 시점 전에 출구에 도달한 시험 전략 누락을 보정하며 실제 대시 요구를 제거하지 않는다.

Sol 수정 범위는 기존 Tests/PlayMode/RunUnity/Chapter1RoomProductionAcceptanceTests.cs 한 파일이다. 각 정확 구성에서 첫 평지 구간의 원본 입구·지형·적 위치와 실제 준비/grounded 상태를 확인한 뒤 정상 InputSystem의 새 LeftShift+D 입력을 단 한 번 명시적으로 발행하고 해제한다. 입구 x0, 초기 단차8 이상·가까운 적5 이상이라는 사전 기하 대조를 현재 원본 구성마다 확인하며 대시 이동 범위의 위험/틈이 없는지 고정 정책에서 확인한다. 조건이 안 맞으면 임의 위치/속도 변경이나 보이지 않는 준비 성공을 만들지 않고 실제 불일치를 기록한다. 이 전처리 입력도 같은 사례 시간/tick 예산과 실제 CSV에 포함한다.

공유 pilot 및 게임 수치·피해·기하·가드·완료 조건은 변경하지 않는다. 입력 발행 여부와 실제 MovementActionState.Dashing 관측을 별도 취급하며 dashObserved는 기존 실제 완료 publication에서만 갱신한다. 대시 실패 시 자동 반복하여 통과시키지 않는다. 기존10구성·모든 행동 assertion·3000tick/120초 사례·전체1200초를 유지한다. 정상 준비/새 edge/해제·기하 미지원·대시 미관측 거절과 명령 CSV 대응을 국소 검사하고 실제 설치 참조의 컴파일 확인 후 Root에 소유를 반환한다.

이 구체 변경에 대한 방 측정 재관측1/1200초를 추가 승인한다. r5 컴파일 실패·r6 실제 필수 행동 미관측 및 이전 소비를 보존하고 예산을 복원하지 않는다. 현재 소스/명세/입력 정책과 새 관측 목적을 새 실행 원문에 결속한다. 같은 원인이 반복되면 큰 무변경 검사를 반복하지 않고 정확 대시 입력/모터 판정 최소 사례로 분리한다. 최종 방 명세 편입과 독립 수용 조건은 그대로다.
### §8 방 편입 뒤 허브 검증 비용의 한정 측정 — Approved

Root는 허브 준비 통과125초와 특정 frame14→15 약20초/후속 두 frame 약103초를 보고했다. Sol 정적 호출 대조는 Ready 내부의 CurrentReceipt 두 번 및 PersistencePending 검증에서 반복 원문/지문 검사가 발생할 가능성을 제시했다. 이는 비용 비중의 실측이 아니며 현재 캐시 추가·검증 생략의 근거로 사용하지 않는다. Astra는 실제 Ready와 시험 WaitReady의 호출 구성을 확인했다.

현재 방 측정이 결속하는 Runtime·Expedition helper·RoomProductionAcceptance 소스는 진행 중인 방 측정과 독립 수용/편입까지 이 성능 조사 때문에 변경하지 않는다. 대시 보정은 별도 기승인 범위로 계속한다. 방 원문 폐쇄→Luna 판독→Astra 수용→메타데이터 편입이 끝난 뒤, G4 큰 선택 실행 전에 기존 Chapter1ExpeditionPlayModeTests.cs의 허브 검사에 한정 진단을 넣을 수 있다. 별도 신규 생산 fixture/API/캐시는 이번 범위가 아니다.

진단은 기존 WaitReady의 실제 Ready 호출 바로 앞뒤 Stopwatch.GetTimestamp로 경과를 측정하고 실제 반환/예외를 그대로 사용한다. 같은 호출을 로그용으로 다시 수행하지 않는다. 첫 최대8번의 호출에 frame·완료tick을 안전하게 읽을 수 있을 때의 값·전후timestamp/frequency·반환/예외 및 walltime을 기록한다. 추가로 준비 성공 뒤 기존 공개 CurrentReceipt 읽기와 그 반환 원본의 PersistencePending 읽기를 분리 측정하는 묶음은 최대1회 허용한다. 이 추가 호출 비용은 정상 Ready의 내부 비용과 구분해 명명하며 내부 Prepared.Validate별 실제 비용이나 총점유율을 직접 계측했다고 주장하지 않는다. 부수 출력은 총16행/각4096UTF8바이트 이하이며 getter/실패 guard를 로그에서 재호출하지 않는다. 원래 예외와 정리 결과를 보존한다.

Sol은 이 정확 시험 진단의 구문/설치 참조 컴파일 확인 뒤 Root에 쓰기를 반환한다. Root는 기존 허브 대상 검사1회/1200초를 별도 새 진단 원문 경로에서 수행할 수 있다. 현재 소스·정확 필터·시간/프로세스 소유를 결속하고 기존 방 입력 명세 원문을 고쳐 재해시하지 않는다. 생산 Runtime과 방 기하/편입 메타데이터가 그대로인지 기록하여 방 증거의 영향 범위를 재검토한다. 측정은 준비 성능의 정상 수용이나 원인 보정 반복 예산을 발급하지 않는다.

공개 경계의 변조/다른 원본/옛 세대 거절 AC를 유지한다. 결과 뒤에만 원문 검증 비용·프레임 지연의 원인을 좁혀 별도 정확 보정을 판단한다. 검증 생략·캐시로 검사 강도 축소·허브 지연을 시험 타임아웃 확대로 숨기기0이다. 실제 시작/메뉴 반응의 성능 문제가 미해결이면 최종 사용자 빌드를 수용하지 않는다.
### §8 사례 사이 실제 중립 소비 관문·단계별 관측 — Approved

r7은37.59초/종료2·닫힌 로그 SHA50B5BAB5A0E0D298D050E955C6AD73520CDCB05DB4BE849B801120F61B3CA994이며 R01-A348tick 통과/HP5 뒤 R02의 단회 대시 준비에서 released=false로 거절됐다. 중립 발행 명령이 있었다는 사실과 실제 해당 장치 해제 소비는 다르다. Sol의 설치 소스 판독에 따라 Fixed/Dynamic/Manual의 player 버퍼가 서로 별개라는 가설은 기각한다. Editor 버퍼/버튼 캐시는 분리되고 무인수 Update가 Editor를 선택할 가능성이 있으나 r7의 실제 updateType/raw가 없어 직접 원인을 확정하지 않는다.

수정은 기존 Chapter1RoomProductionAcceptanceTests.cs 한 파일에 한정한다. 이전 graph가 중지·제거되고 다음 생존 graph 활성 전, 시험 소유 Keyboard/Mouse에 중립 QueueStateEvent를 각1회 발행한다. 발행 전에 공개 onEvent/onAfterUpdate를 구독하고 장치 ID/참조·updateCount를 고정한다. 동일 Keyboard의 실제 상태 이벤트에서 ReadValueFromEvent로 Shift0을 관측한 뒤 실제 Dynamic onAfterUpdate에서 발행 전보다 증가한 updateCount, 동일 장치 added/enabled, ReadValue()==0 및 isPressed==false를 모두 확인해야 다음 graph와 단회 새 대시 edge로 진행한다. 관련 Mouse 중립도 소유 장치/상태 사건으로 구분해 보존한다. onEvent에서는 상태를 수정하거나 이벤트를 처리 완료로 바꾸지 않는다. 다른 장치 사건·Editor 갱신·단순 Queue 성공을 해제 성공으로 대신하지 않는다.

최대30 렌더 frame/5초 중 먼저 도달하는 한도이며 이 대기는 해당 사례120초 및 전체1200초 안에 포함한다. 사례 시계는 준비 전 시작하고 graph 활성화 시 초기화하지 않는다. 성공/실패/예외 모두 finally에서 구독을 해제한다. 실패는 최대4행·각4096UTF8바이트에 device/updateType/count/raw/isPressed/사건·갱신 횟수와 실패 이유를 기록하고 graph를 시작하지 않는다. internal Update(type), fixture.runtime, 반사·직접 상태 덮어쓰기·장치 reset/held 가짜 해제·게임 시간 변경0이다. 기존 단회 edge/실제 Dashing 관측과 모든 게임 가드는 유지한다.

같은 파일의 누적 CSV는 사례 루프 try/finally에서 실패 때도 폐쇄 보존한다. 원래 게임/준비 예외를 기록 오류로 덮지 않고 두 오류를 분리하며 정상 결과 명세를 실패에서 발급하지 않는다. 정상/Editor만 수신/다른 장치/이벤트 없이 raw0/이벤트 후 raw유지/기한/구독 해제 경계를 국소 확인하고 실제 설치 참조의 격리 Csc 뒤 Root에 소유를 반환한다.

검사 예산은 단계별 각1회/각1200초다. 먼저 기존 R01-A→R02-A 두 사례의 같은 생산 흐름과 실제 사례 사이 준비를 최소 관측한다. 정확 선택 목록을 실행 전 기록하고 부분 결과를 방10 최종 수용 파일로 발급하지 않는다. 첫 단계에서 두 사례 및 중립 소비·새 대시 edge가 실제 통과한 경우에만 이어 별도 새 경로에서 기존10구성 전체를1회 측정한다. 이 두 번째는 미측정 구성까지 포함하는 필수 전체 관측이지 첫 실패의 재시도가 아니다. 첫 단계 실패 시 두 번째를 소비하거나 자동 진행하지 않고 원시 원인부터 분리한다. 두 실행 사이 정책/제품 변경이 필요하면 이 조건부 전체 진행 권위를 성공으로 간주하지 않는다. 기존 실패/소비·방 source manifest·독립 수용/편입 경계는 보존한다.
### §8 G6 12사례·G7 4경로의 시험 목록 완결 계획 — Approved

Astra는 b1-runtime-gates-astra.v1.json의 G6/G7 및 현재 Chapter1ExpeditionPlayModeTests.cs/Chapter1ClosingPersistencePlayModeTests.cs를 직접 대조했다. G6 원문은101/202/303/404 각각3회 총12개의 전이·선택기술 없는 필수경로 trace이며 G7은 정상101과 HP0/낙하/압착 각1이다. 현재 Expedition의 정상 경로는101 고정이고 G6 4×3을 발견/집계할 명시적 시험 목록은 아직 없다. AuthoredFailureReturnsAndAllowsFreshStart 안에는3원인 반복이 있으나 XML 한 시험으로 묶여 있으므로 개별 사례 식별/실패 원인별 폐쇄가 부족하다. Closing의 양분기 ValueSource2는 별도로 존재하며 G6 12개나 G7 정상/실패를 대체하지 않는다. 이는 현재 코드의 준비 공백 판정이며 실제 Standalone 실행 실패를 주장하지 않는다.

방 측정·독립 편입이 끝날 때까지 그 manifest에 결속된 Expedition/helper/Runtime/Room 측정 소스는 이 목록 보완 때문에 수정하지 않는다. 그 뒤 Sol은 기승인 Chapter1ExpeditionPlayModeTests.cs 한 파일에서 G6 seed×반복번호의 정확12개 목록과 G7 정상1/실패3 개별 식별을 구현할 수 있다. 설치 TestFramework가 지원하는 매개변수/ValueSource 방식 또는 명시적12 메서드 중 실제 발견 가능한 방식을 사용하고, 실행 전 test 목록/XML 예상 이름에12+4 식별자를 고정한다. Repeat 속성만으로 동일 runId/원문을 덮어쓰거나 실패를 다음 반복의 성공으로 대체하지 않는다.

G6 각 사례는 독립 runId·원정키·초기 준비·해당 snapshot seed·반복1~3과 현재 소스/기하/빌드 결속을 갖고 실제 정상 입력으로 네 방 필수경로를 통과한다. 전이·선택기술 입력0을 발행 로그로 확인하고 실제 방문 순서/방 구성/소켓/입력/완료tick/HP와 정상 끝 조건을 보존한다. 실제 snapshot 구성과 seed 대응을 확인하며 seed 선택만으로 예상 구성이 실행됐다고 간주하지 않는다. 게임 상태·피해·완료·원정키 직접 대입0이다. G3 방10 관측을 G6 회수에 중복 계산하지 않는다.

G7의 기존 정상101 및 HP0/낙하/압착 검사를 정확4사례로 독립 집계한다. 실패3의 기존 실제 producer/accepted failure 종류·commit 전후 정리·저장 revision·Returned·중립·새 edge·새 원정키 assertion은 보존한다. 기존 반복 내부에서 그 assertion을 재사용하도록 공통 본문을 분리할 수 있으나 실패 종류를 enum명/정수 일치만으로 가짜 생성하지 않는다. 기존 필터 이름 변경은 실행 선택표에서 명시하고 누락/이중집계를 거절한다. 정상의 보스 진입과 CH1 양분기 최종 종결은 서로 다른 범위로 기록한다.

이 보완은 기존 승인 StandaloneTestplayer의12계획·정상/실패4 및 별도 양분기를 실제로 준비하는 것이며 새 Unity 실행/빌드 예산을 추가하지 않는다. 합성 InputSystem 생산 경로 증거로 명시하고 사람 직접입력 증거라고 기록하지 않는다. 기존 실제 OS 창 검사는 별도다. 현재 승인된 각 사례 상한과 전체 실행 상한 중 먼저 도달하는 한도를 유지하며 예상 시간상 전체 상한에 들어가지 않으면 실행 전에 정확 분할/예산안을 제시한다. 실제 실행 중 연장이나 미실행 사례 통과 처리0이다. 같은 파일의 허브 비용 진단은 편입 뒤 함께 조정할 수 있으나 별도 목적과 결과를 구분한다.

Sol은 국소 사례 목록의 유일성/12개 seed별3회·G7각원인1·원문 경로 중복0·현재 Standalone 조건/컴파일을 확인한다. Luna는 실행 전 필수 원문 요구와 실제 발견 목록을 대조하고 실행 후 skipped0·정확 개별 결과/지문을 판독한다. 아직 G6/G7 전체·최종 CH1 완주 수용은 없다. 기존 인간 검수 원문을 고치지 않고 최신 자율 계약의 명시적 대체 검증으로만 연결한다.
### §8 ROOM-ADMISSION-R9 — 방6개·10구성 합성 관측 통합 수용 및 편입 — Approved

Astra는 REQ/AC-AUTOCH1-012에 따라 runId rooms-9af995c56300413eb8662822da4ab023의 방6개·10구성 합성 InputSystem 관측을 한정 통합 수용한다. 직접 판독한 측정 원문은 output/chapter1-autonomous-20261021/evidence/room-production/rooms-9af995c56300413eb8662822da4ab023/chapter1-room-production-acceptance.v1.json 4085바이트/SHA256 D60A9B25E92C803118B4E0627D8FFF9517AC6B9C8F16361E752D5CAF8DD3A738, 원래 source-geometry-manifest.v1.json은30358바이트/SHA256 B9A6755E62D958BB109860CD1A4678027657542CF828D6108A021B42A6670FCB다. 입력169개와 정확10개 요구 구성이 결속돼 있다.

Luna 독립 판정 docs/verification/autonomous-ch1-20261021/room-production-r9-independent-review.md 5027바이트/SHA256 97CCF9EB69227D0CFFDB6C1EAC229FEB7F04D1B8DEECF9E5299F2DD8FB9F78C2를 직접 읽고 핀 일치를 확인했다. P0/P1 없음·선택 XML1통과/실패0/건너뜀0·종료0·99.126초, 제품628개 전후 차이0,10CSV 총3788행의 지문/연속tick/동일 전투tick·실제 대시/점프·출구·HP양수·적0 및 사례 사이 Dynamic 중립 소비10개가 독립 대조됐다. Root 폐쇄 원문 room-measurement-r9/root-closure.v1.json SHA256 EE3F3C4FD977211B6B0BFC270534EBD931E2D8EC49909A83D75A655F859737B9의 한정 기계 대조와 독립 판정을 구분한다. 원래 실패·부분r8 및 로그의 경고는 보존한다.

편입 대상은 정확 R01/S01-A, R02/S02-A·S02-B, R03/S03-A·S03-B, R04/S04-A·S04-B, R05/S05-A·S05-B, R06/S06-A다. Root는 이 승인 ID ROOM-ADMISSION-R9와 위 측정/원래 입력 manifest/독립 검수 SHA 및 정확 수용 metadata 변경 목록을 결속한 별도 room-admission.v1.json을 신규 폐쇄 작성할 수 있다. 이어 기승인 Builder.ApplyAutonomousRoomAcceptance의 독립 편입1회/1200초를 수행한다. 소스·기하·현재 prefab 입력 핀을 실행 직전 재대조하고, 변경은 RoomInstance의 기존 수용 필드 및 최소 파생 명세 결속 필드로 한정한다. 생산 코드·기하·게임 값·시험 원문 변경0이다.

편입 전 입력 prefab과 편입 후 파생 prefab의 서로 다른 지문을 각각 기록하고 변경 diff가 정확 metadata뿐임을 확인한다. 과거 입력 manifest를 수정하거나 파생 지문과 동일하도록 다시 만드는 순환0이다. 허용 밖 변경·입력 불일치·편입 실패는 원문 보존 후 보고하며 자동 재실행하지 않는다. 편입 폐쇄/차이 확인 뒤에만 기승인 허브 비용 진단 및 G6/G7 목록 보완의 소스 동결 조건이 해제된다.

이 수용은 현재 고정 생산 경로의 방10 요구 행동과 안전한 사례 전환 관측에 한정된다. 사람/실제 OS 입력, G6 원정12·G7 종단4, 보스 이후 양분기, 전체 CH1, 최종 화면·Windows 빌드·성능 및 최종 SourceReady 승격을 뜻하지 않는다. 방 측정 도구가 자기 승인한 결과가 아니며 Luna 독립 판정과 Astra의 이 한정 결정 뒤 별도 편입하는 절차다.
### §8 G6/G7 실제 소켓·입력·저장 증거 완결 — Approved

Luna의 v12 독립 정적 대조에서 G6 명시12/G7 명시4 발견 목록은 일치하나 실제 초기 소켓, 발행 key/pointer/attack와 전이·선택기술 미사용, G7 저장 결과/기대 revision 연결 증거가 아직 미완성이다. 목록 존재를 실행 수용으로 확대하지 않는다. 현재 Root 허브 비용 진단 프로세스가 종료·회수되고 파일이 폐쇄되어 쓰기 소유를 반환한 뒤에만 Sol이 아래 보완을 수행한다. 성능 캐시/생산 검증 변경은 이 승인에 포함하지 않는다.

허용 제품 변경은 기존 Chapter1ExpeditionPlayModeTests.cs 한 파일이다. G6용 읽기 전용 시험 MonoBehaviour 관찰자를 같은 파일에 두고 시험 소유 오브젝트에 한 번 설치할 수 있다. DefaultExecutionOrder(-10000)의 Fixed 관측은 Router보다 앞서 읽기 위한 수단일 뿐 초기 상태 증명 자체가 아니다. 현재 정확 flow/lifetime/router/graph 참조와 runKey/generation, 실제 완료 publication 부재 및 기존 공개 읽기로 확인 가능한 첫 생산 전 상태를 함께 검사해야 한다. 새 graph 첫 Walker/Surveyor 실제 localPosition을 기록하고 선택 socket의 원본 기대좌표와 대조한다. 계획 SocketAt만 기록하거나 첫 생산 이후의 좌표를 초기좌표로 소급하지 않는다. 최초 미생산 시점이 관측되지 않거나 참조/세대가 다르면 해당 증거는 미관측 실패로 남긴다. 지원되는 읽기로 이 증명이 불가능하면 새 API/반사로 우회하지 않고 정확 부족 항목을 보고한다.

관찰자는 생산 transform/component/시간/입력/순서 설정을 바꾸거나 생산 callback을 호출하지 않는다. 사례당 새 방 graph 최대4 및 보스 식별1의 기록으로 제한하고, 최초 생산 전 관측을 못 했다고 동일 graph를 재생성하지 않는다. 시험 소유 관찰자 해제/오브젝트 정리는 finally에서 기존 graph 정리와 fixture 복원 순서를 보존해 수행한다. 배포 조립체/장면/prefab에는 이 객체를 포함하지 않는다.

기존 Keyboard/Pointer 및 직접 QueueStateEvent 발행 지점 주위에 실제 발행한 키 배열·포인터 좌표·attack/transfer 버튼·장치 ID·갱신 종류/카운트·runId/runKey/generation/관측 가능한 tick을 기록한다. 기록은 발행값과 이후 완료 publication/결과를 분리하고 발행 성공을 소비 성공으로 대신하지 않는다. 기존 helper의 입력 결정·키 유지/해제·공격/조준·Queue/Update 횟수·yield 순서와 RoomPilot 몸체를 변경하지 않는다. 모든 해당 입력 발행 경로를 포함해 G6 전이/선택기술 발행0을 검사하고 실제 생산 publication에서 지원되는 전이 결과도 확인한다. 명시되지 않은 버튼 또는 외부 입력은 조용히 제외하지 않고 범위/결과에 표시한다.

G7은 같은 원본 run/generation의 기존 공개 LastSaveResult에서 실제 Outcome/CommittedRevision과 기대 profile revision을 읽어 실패 수용→저장→Returned→중립→새 edge→새 runKey의 연결을 기록한다. 실패 전/후 기대 profile을 동일 승인 저장 원본으로 읽고 실제 값과 비교하며 성공 enum이나 기대 revision을 임의 대입하지 않는다. 정확 노출 필드가 없으면 미관측을 숨기거나 새 생산 API를 추가하지 않는다. 원래 accepted failure 종류·commit/정리 순서·새 원정 assertion을 보존한다.

각 사례 원문은 최대20000행/16MiB로 제한하고 초과하면 잘라낸 성공 대신 한도 실패로 닫는다. 원래 사례/전체 기한을 유지한다. finally에서 부분 trace를 신규 폐쇄 보존하고 최초 실패와 기록/정리 실패를 분리한다. 각 실행 결과에 정확12/4/별도 양분기 및 합성 입력 등급을 명시한다. Room 원래 측정 manifest/CSV는 수정하지 않고 이번 시험 전용 증거 변경과 생산/pilot 무변경 영향 대조를 별도로 보존한다.

Sol은 초기/이미생산/다른graph·세대·누락소켓, 입력 발행 누락/금지기술, 저장 실패/옛 revision/다른 run 및 finally 원문 보존의 국소 검토와 실제 설치 참조 컴파일을 수행하고 Luna가 영향 재판독한다. 새 Unity/Windows 실행 예산은 추가하지 않으며 기존 G6/G7 실행 전 실제 발견 목록·핀·완결 증거 요구를 Root가 확인한다. 이 정적 보완으로 실제 초기 배치/입력 전달/저장 또는 최종 완주를 먼저 수용하지 않는다.
### §8 허브 receipt 내부 중복 검증 축소·공개 경계 보존 — Approved

Root hub-performance-r1은141.87초/종료0, 선택1통과/실패0/건너뜀0·case129.102초, 소스628 전후 차이0이다. 닫힌 로그 SHA256 B1B6EFC7C778C7C8ED1E69AAB82E917207B85B5A579D787BF583C3F8A1D28B57, Root 폐쇄 SHA256 7F253D8ADD1D723A465A8051222095C0DEAC39CCB69A11BF3016462E8C6FFC6B를 결속한다. 실제 true Ready 네 호출586~603ms, 추가 CurrentReceipt193.9293ms·반환 receipt.PersistencePending201.8058ms를 관측했다. 개별 Prepared.Validate 비용이나 전체129초 중 비중은 측정하지 않았다. 동일 지연의 무변경 큰 재실행은 하지 않는다.

Sol 생산 수정 범위는 Runtime/Input/Unity/ProfileLaunchPreparationCoordinatorV1.cs와 DesktopProfileLaunchAdapterV1.cs 두 파일이다. PreparedProfileLaunchV1에 internal ValidateAndReadEvidence()와 필요한 내부 readonly 값형 투영을 추가하여 호출할 때마다 기존 Validate 전체를1회 실행한 뒤 동일 객체의 private 증거 필드를 즉시 읽는다. DesktopProfileLaunchReceiptV1.ValidateEvidence의 해당 동기 호출 안에서 _proof의 반복 검증 getter와 Same(save,proof) 비교만 이 지역 투영으로 대체한다. 기존 document/preservation/save 검사, publication/hub proof, 원본/복사 증거 전체 일치, 선택·canonical bytes·binding·revision·보존/저장·소유 검증은 줄이지 않는다. 비교 대상 필드의 전후 일대일 목록으로 누락을 거절한다.

모든 기존 공개/외부 getter와 CurrentReceipt는 매 진입 검증을 유지한다. 투영을 필드·static·프레임·다음 호출에 저장하거나 검증 토큰/새 권위로 발급하지 않는다. 같은 동기 호출 밖 재사용·객체 신원만으로 통과·해시 비교 삭제·일정 시간 검증 생략0이다. 기존 접근보다 더 넓은 mutable 배열/객체 원문 노출을 만들지 않으며 필요한 바이트 비교는 기존 안전 읽기/복사 경계를 유지한다. 검증 후 비교 사이 새 callback/await/yield/재진입 호출을 추가하지 않는다. 예약·입양·actions/map 상태·소유 이전·legacy 경로·정리·실제 저장·게임값은 변경0이다. 이 보정은 내부 중복만 줄이며 전체 허브 지연 해소를 미리 주장하지 않는다.

시험 보완은 기존 Tests/PlayMode/InputUnity/DesktopProfileLaunchAdapterV1Tests.cs 및 ProfileLaunchPreparationCoordinatorV1Tests.cs 두 파일에 한정한다. 기존 PreparedHubProof3, PublishedReceipt 변조, Notification2, CurrentReceipt backing 재검증, Coordinator AC009 전체 projection/token mutation과 AC007 보존/저장 mismatch 선택을 유지한다. 같은 정상 공개 getter 첫 호출 뒤 nested proof를 시험 전용 반사로 변조하고 두 번째 호출이 거절되는 음성 사례를 추가한다. 반사는 이 기존 격리 음성 시험에만 허용하며 생산 경로/실제 프로필을 변경하지 않는다. 정상 원본 투영과 모든 비교 필드의 일치·공개 호출 간 캐시 없음도 검사한다. 기존 음성 fixture 이름/매개변수별 실제 발견 목록을 Root가 실행 전에 명세에 고정하며 실패/건너뜀을 제외해 수치를 맞추지 않는다.

Sol은 원문 보존·국소 검토·설치 참조 컴파일 뒤 소유를 반환하고 Luna는 검증 강도/영향 범위를 독립 대조한다. 추가 실행은 이 정확 음성·정상 선택 묶음1회/1200초와 그 통과 뒤 동일 허브 비용 검사1회/1200초로 한정 승인한다. 앞 실행 실패면 뒤 성능 검사는 진행하지 않는다. 기존 실패/예산 소비를 복원하지 않고 새 입력/핀/목적을 결속한다. G4/Standalone 큰 검사는 성능 결과와 원인/잔여 비용을 평가한 뒤 진행한다. 여전히 과도한 지연이면 이 결과를 최종 성능 수용으로 삼지 말고 남은 구간을 원시로 분리한다.

이 생산 두 파일 변경의 방 증거 영향은 별도 기록한다. 원래 방 측정 manifest는 불변으로 보존하고 이동/전투/기하/RoomPilot 무변경 및 정확 profile/허브 검증 경계 변경을 명시한다. 방10 통과를 새 source 전체 해시와 같다고 재작성하지 않는다. 최종 빌드 manifest와 G6/G7 실제 소스는 새 현재핀으로 결속하며 전체 CH1/성능/빌드 수용은 후속 실제 결과와 독립 검수 뒤다.
#### §8 G6/G7 완결의 독립 지적 보정 범위 확인 — Approved

v13의 초기 소켓 관찰자가 InRoom/AtBoss 이후만 읽는 조건은 실제 Flow가 첫 완료 evidence 뒤 그 phase를 발급하므로 최초 미생산 증명과 양립하지 않는다. Astra가 Flow와 observer를 직접 대조했다. 같은 시험 파일의 관찰 조건을 실제 StartingRoom/LeavingRoom 전환 중 새 graph의 활성화·원본 router/세대·runKey가 확인되는 시점으로 바로잡을 수 있다. 존재하지 않는 PreparingRoom 이름을 가정하지 않는다. flow의 이전 관측 generation과 새 lifetime generation을 혼동하지 않으며, 실제 player.TryReadExistingMotionForAuthoring의 motion.Tick==nextExpectedTick==Chapter1ConfiguredFirstTick 및 새 원본 완료 evidence 부재를 함께 요구한다. 같은 Fixed 중 생성되어 관측을 놓치면 실패로 남기며 생산 순서/상태를 변경하지 않는다.

기존 G7 RunFailureCase 본문에 실제 LastSaveResult·시작 전/복귀 후 profile·Outcome/CommittedRevision 기대 비교를 연결한다. 다른 시험에 같은 assertion이 있다는 이유로 G7 증거를 대신하지 않는다. 입력 발행에는 사례 내 단조 sequence를 붙이고 동일 run/graph/generation에서 후속 완료 publication과의 대응 범위를 기록·검증한다. 여러 발행이 한 완료tick 앞에 있거나 소비를 확인할 수 없는 발행은 그 사실을 구분하며 일대일 소비 성공을 꾸미지 않는다.

FinallyCloseEvidence의 로그만 남기는 경로는 정상 성공을 허용하지 않도록 보정한다. 원래 실패가 있으면 원래 원인을 보존하고 폐쇄 실패를 별도 추가하며, 원래 성공이면 폐쇄 실패로 시험을 실패시킨다. fixture 정리/입력 복원은 예외가 있어도 시도하되 정리 실패도 숨기지 않는다. 위 항목은 이미 승인한 동일 시험1파일 증거 완결의 실제 보정이며 새 생산 API·실행 예산을 추가하지 않는다. 성능 생산2파일 작업과 소유를 구분하고 국소/설치참조 컴파일 및 Luna 영향 재판독 뒤 반환한다.
#### §8 허브 변조 검사 플랫폼 입력 보정1 — Approved

Root 허브 음성 선택 r1은 PID91720·32.73초·종료0이나 XML total0/failed0/skipped0 및 No tests were executed로 실제 시험0이다. 닫힌 로그 SHA256 4F7FD80C8F9673202BA0B1B6900827C7E7C915AF49FE267B3CFC61D9F8C42F34를 보존하며 통과로 수용하지 않는다. Astra는 실제 AcadeGameMaker.Input.Unity.PlayMode.Tests.asmdef의 includePlatforms=[Editor]를 직접 확인했다. 폴더/조립체 이름을 실행 플랫폼으로 추정한 입력 오류를 보정한다.

동일 생산/시험 소스 핀과 hub-validation-test-selection.v1.json의 정확14메서드 필터를 유지하고 -testPlatform만 EditMode로 바꾼 새 실제 선택 실행1/1200초를 승인한다. 새 경로에 원래/보정 인수 차이와 실행 핀·원래 소비를 기록한다. 예상 매개변수 포함18사례의 실제 발견 이름/개수·실행·실패/건너뜀0을 XML로 확인한다. 발견0·이름/개수 불일치·선택 누락이면 실패이며 종료0으로 대체하지 않는다. 기존 r1 소비 복원0, 소스 변경0, 기한 연장0이다.

이 정확 정상/음성 선택이 통과하기 전에는 기승인 후속 허브 성능 재관측을 진행하지 않는다. 이후 G4/G5/Standalone 선택도 경로명만으로 모드를 고르지 않고 실제 asmdef/설치 시험 발견 결과와 명세를 대조한다. 이번 보정으로 다른 실행 예산이나 최종 수용을 확대하지 않는다.
#### §8 편집기 전용 시험의 실제 재생 환경 준비 보정 — Approved

허브 검증 r2는18개 실제 발견·9통과/9실패·건너뜀0,16.25초/종료2, 닫힌 로그 SHA256 BBAECC94ADED99DF89A67BBA63A0A029AC0A3CF3E184F52D32DB01149162A192다. Astra가 XML 실패를 직접 읽어 Desktop6개의 CombatTarget named scene/hierarchy 요구 실패와 Coordinator3개의 edit mode Destroy 오류를 분리했다. 이 결과는 검증 강도 보존 성공이 아니며 성능 후속은 계속 보류한다.

설치 TestFramework의 공개 UnityOneTimeSetUp/UnityOneTimeTearDown 문서는 [Test] 본문을 EnterPlayMode/ExitPlayMode 사이에서 실행하는 예제를 실제 제공한다. 같은 두 허용 시험 파일에서 이 지원 경로를 사용해 fixture 전체의 재생 진입/종료와 사례별 이름 있는 격리 runtime scene 준비·활성 장면 설정·정리를 구현할 수 있다. 실제 Application.isPlaying 및 scene valid/loaded/nonempty name을 확인한 뒤 기존 [Test] 본문을 수행한다. 순수 검사는 같은 fixture 안에서 기존 본문을 유지해 실행할 수 있다. 재생 진입이 실패하거나 reload 후 상태가 달라지면 성공을 꾸미지 않는다.

각 사례의 오브젝트는 새 정확 시험 장면 안에 생성되어야 하며 이전 활성 장면을 보존하고 정리 뒤 복원한다. 이전 제품 장면/자산/사용자 프로필을 삭제하거나 저장하지 않는다. 시험의 원래 Dispose/Destroy 흐름을 재생 상태에서 완료시키고 지연된 파괴·씬 Unload를 관측한 뒤 ExitPlayMode한다. 정리/종료 실패는 별도 오류로 보존하고 통과시키지 않는다. 필요한 시험 lifecycle 필드와 IEnumerator using/공개 SceneManager 접근만 같은 두 파일 안에 추가한다. asmdef 플랫폼 변경·전체 조립체 전환·생산 GameInputActions/CombatTarget guard 수정·DestroyImmediate 대체·LogAssert 오류 삼키기0이다.

Sol은 시험 setup/teardown과 격리 장면 소유만 바꾸고 기존18개 검증 본문/음성 조건이 동일함을 역대조한다. 설치 참조 Csc와 lifecycle 예외/이미재생/미재생 정리 경계를 국소 검토한 뒤 반환한다. Root는 -testPlatform EditMode 및 기존 정확 선택을 유지한 새 실제 검사1회/1200초를 추가 수행할 수 있다. 실제18 발견/실행·실패0/건너뜀0·재생/장면 준비와 폐쇄를 확인해야 기존 허브 성능 재관측으로 진행한다. r1시험0/r2실패와 모든 소비를 보존하며 이번을 무변경 플랫폼 재시도로 처리하지 않는다. 새 원인 발생 시 추가 큰 실행을 자동 반복하지 않는다.
위 lifecycle 보정의 재생 진입 전에는 공개 EditorSceneManager.GetSceneManagerSetup으로 원래 저장 장면 구성을 보존하고, 미저장 사용자 변경이 없는 배치 시험 상태를 확인한 뒤 NewScene(EmptyScene,Single)로 빈 시험 장면에서 EnterPlayMode한다. 기존 CH1 생산 장면을 함께 활성화하지 않는다. 씬 파일 쓰기/저장0이며 종료는 소유 runtime 장면 정리→ExitPlayMode→RestoreSceneManagerSetup 순서로 복원한다. 현재 setup이 복원 가능한 저장 장면 구성이 아니거나 미저장 변경이 있으면 임의 폐기하지 않고 실패로 남긴다. 복원 결과까지 원시로 확인한다.

이 lifecycle 첫 실제 관측의 실행 배치는 직전 추가1회를 대체하여 최대2회/각1200초로 한정한다. 먼저 기존 선택 중 Desktop 정상 원본1과 Coordinator 새 정상 투영1의 정확2개로 재생/이름 장면/Dispose/복원 경계 최소 관측을 수행한다. 실제2개 통과·폐쇄 확인 뒤에만 원래18개 전체를 새 경로에서1회 실행한다. 첫 실패에서 두 번째는 진행0이고, 부분2개를 전체18 수용으로 세지 않는다. 정확 메서드명/필터는 기존 선택표와 실제 발견 이름으로 실행 전 고정한다. 전체18 통과 뒤의 기승인 성능 재관측 조건은 그대로다.
#### §8 내용 없는 기본 배치 장면의 복원 예외 — Approved

위 시험 lifecycle의 저장 장면 구성 복원 원칙에 다음 한 경우만 추가한다. 실행 전 실제 SceneManagerSetup/장면 목록이 정확1개이고 그 원본 scene이 valid·loaded·active, path가 빈 문자열, isDirty=false, rootCount=0임을 모두 직접 읽어 기록한 경우에는 내용 없는 기본 배치 장면으로 분류할 수 있다. 빈 이름 또는 과거 오류만으로 빈 장면이라고 추정하지 않는다. 검사 순간의 이름/path/개수/loaded/active/dirty/rootCount와 복원 방식 선택을 원문에 보존한다.

이 경우에만 종료 후 공개 NewScene(EmptyScene,Single)으로 동일한 빈 상태 조건을 복원하고 실제 단일 valid/loaded/active·빈 path·dirtyfalse·root0을 다시 대조한다. 이는 같은 scene handle/객체 identity 복원을 주장하지 않는 상태 동등 복원이다. 저장된 원래 구성은 기존 RestoreSceneManagerSetup 경로를 그대로 사용한다. 다수 장면·내용 있는 미저장 장면·dirty·불명 원본은 거절하며 저장/삭제/자동Discard로 통과시키지 않는다. 원래 CH1 장면의 동시 활성화와 사용자 장면 파일 쓰기0이다.

정확 두 허용 시험 파일의 준비/복원 분기에만 적용하며 새로운 실행 예산은 없다. reload 전후 선택한 복원 방식과 원래 상태가 유실되면 실패로 남긴다. 최소2개→성공 뒤18개의 기존 관측 순서/기한·모든 원문 보존 조건을 유지한다. 실제 조건이 성립하지 않으면 해당 준비 실패를 보존하고 추가 실행을 자동 반복하지 않는다.
#### §8 구성 0개인 배치 시험 환경의 거절 전 원시 관측 — Approved

최소 fixture-r1은 정확2개 발견·0통과/2실패·건너뜀0, PID125200·14.34초·종료2이며 닫힌 로그 SHA256 4DC002686190662D74883113A28622187916962723483664BF0F565AA852EA03이다. 두 OneTimeSetUp은 GetSceneManagerSetup().Length>0 조건에서 실패했고 재생 진입/장면 쓰기에 도달하지 않았다. setup0은 실제 관측이나 SceneManager의 장면 수·활성·내용·변경 여부는 아직 미관측이다. 이를 빈 장면 또는 안전한 복원 상태로 승인하지 않는다.

Sol은 정확 output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/DesktopProfileLaunchAdapterV1Tests.cs 한 파일에서 기존 setup 길이 거절 바로 앞에 읽기 전용 원시 기록만 추가한다. setupCount, SceneManager.sceneCount, active scene 식별과 최대8개 장면의 name/path/valid/loaded/dirty/rootCount, 최대8개 setup의 path/loaded/active를 기록한다. 문자열은 항목당1024자, 총 기록은16384 UTF8바이트로 제한하고 초과·읽기 예외는 원시 관측 불완전으로 표시한다. 실제 전체 개수와 잘림 여부를 보존하며 생략된 항목이 없다고 주장하지 않는다. 기록 실패가 원래 setup 거절을 덮거나 성공으로 바꾸지 않는다. 장면 생성/복원/활성화·EnterPlayMode·생산 상태 쓰기·기존 guard 및 시험 본문 변경0이다.

원문 보존과 설치 참조 컴파일 후 Root는 기존 Desktop 정상1만 정확 선택하여 새 경로에서 진단1회/1200초를 수행한다. 예상 결과는 기존 guard 실패이며 통과를 목표로 조건을 완화하지 않는다. 실제 발견1·거절 위치·원시 상태·프로세스 종료/폐쇄를 기록한다. 이전 최소2 소비는 복원하지 않고 전체18 및 성능 재관측은 계속 보류한다. 실제 원시를 확인한 뒤에만 구성0에 대한 안전한 준비/복원 호환 여부를 별도로 판단하며, 새 가정에 의한 큰 재실행은 금지한다. REQ/AC-AUTOCH1의 검증 강도와 사용자 중간 조작 없는 개발 원칙을 유지한다.
#### §8 시험 실행기가 만든 일회성 배치 시작 장면의 소유 분리 — Approved

hub-zero-setup-diagnostic-r1은 종료2/13.78초이며 setupCount0, sceneCount1, 유효·적재·활성인 빈 이름/path 장면, dirtyfalse/rootCount2를 읽었다. 읽기 예외·잘림0이고 기존 guard 거절을 보존했다. 이는 기존 root0 예외의 충족이 아니다. Astra가 설치 TestFramework의 TaskList.cs와 CreateBootstrapSceneTask.cs를 직접 확인한 결과, EditMode 경로는 원래 구성을 먼저 보존한 뒤 DefaultGameObjects/Single의 미저장 bootstrap 장면을 만들고 시험 controller는 추가하지 않는다. 따라서 두 루트의 존재만으로 사용자 미저장 장면이라고 단정할 수 없으나 실제 객체 종류를 읽지 않은 상태에서 기본 Camera/Light로 확정하지도 않는다.

같은 두 fixture에만 별도 owned-batch 분기를 허용한다. Root가 정확 프로젝트에서 기존 Editor 부재를 확인하고 직접 시작한 일회성 batch 프로세스여야 한다. 실행 계획에는 고유 runId/정확 프로젝트 경로/선택 필터/명시 명령 표식(-autonomousOwnedBatchFixture)을 고정하고, fixture는 Application.isBatchMode·실제 명령 표식·정규화된 현재 프로젝트 경로·현재 PID와 시작시각을 원시로 남긴다. Root가 실제 소유 Process/PID/birth/명령과 대조한다. 표식만을 사용자 데이터 접근 권위로 삼지 않으며 기존 interactive Editor에 이 분기를 적용하지 않는다.

이 분기는 setup0·scene1·유효/적재/활성·빈 path·dirtyfalse인 실행기 시작 장면에 한정한다. 최초 변경 전에 루트들의 이름·hideFlags·component 실제 형식·자식 수를 제한 기록하고, 정확2루트에 자식0, 기본 Transform/Camera/AudioListener/Light 외 component0 및 MonoBehaviour0을 확인한다. 조건 불충족이면 장면을 바꾸지 않고 실패한다. 기본 객체의 실제 존재/구성은 다음 최소 관측에서 확인하며 예상 이름만으로 통과시키지 않는다. 현재 프로세스 외 장면·프로필·저장 자산 파일에는 쓰지 않는다.

확인된 실행기 소유 시작 장면은 공개 NewScene(EmptyScene,Single)로 대체하여 기존 EnterPlayMode 및 이름 있는 사례 장면 준비를 수행할 수 있다. 이 분기에서는 과거 시작 장면의 root0 상태 동등 복원을 주장하지 않는다. 소유 사례 객체/장면 정리→ExitPlayMode→빈 시험 장면의 실제 상태 확인→시험 실행기 정상 종료 및 Root의 해당 소유 프로세스 회수로 경계를 닫는다. 원래 저장 장면 및 기존 엄격한 빈 장면 분기는 변경하지 않는다. 재생/정리/시험 실행기 후처리/프로세스 폐쇄 실패는 실패로 보존하며 사용자 장면 저장·삭제·설정 변경0이다.

Sol은 같은 두 fixture의 이 분기와 원시 기록만 보정하고 기존 시험 본문·생산/asmdef는 유지한다. 설치 참조 컴파일과 Luna 영향 확인 뒤 Root는 정확 최소2건을 새 경로에서1회/1200초 재관측할 수 있다. 기존 실패 소비는 복원하지 않는다. 실제2 통과·후처리/소유 회수 성공 뒤에만 이미 승인된 전체18의 미소비1회, 그 통과 뒤 성능1회 조건을 사용한다. 객체 구성 불일치나 새 실패면 전체18로 진행하지 않으며, 원인 없는 같은 큰 실행 반복0이다.
#### §8 같은 배치의 다음 fixture를 위한 기본 장면 재구성 — Approved

v18 실행 전 검토에서 owned 분기의 Exit 뒤 빈 장면을 남기면, 전체 EditModeRun 종료 때만 수행되는 실행기 RestoreSceneSetupTask 이전에 다음 fixture가 setup0/root0을 받는 구조를 확인했다. 아직 실제 실행0이며 이 위험을 새 실패 소비로 기록하지 않는다. 대안 (나)를 채택한다. 같은 두 fixture의 owned-batch 종료 분기에서 기존 소유 사례 정리→ExitPlayMode→실제 단일 빈 장면 검증을 수행한 뒤 공개 EditorSceneManager.NewScene(DefaultGameObjects,Single)을 한 번 호출하고 그 실제 기본 장면을 검증한다.

재구성 뒤 setup0·scene1·valid/loaded/active·빈 path·dirtyfalse와 root2·각 child0·정확 Transform/Camera/AudioListener/Light 이외 component0·MonoBehaviour0을 기존 시작 검사와 같은 기준으로 확인하고 원시를 기록한다. 실제 장면/구성 불일치나 호출·검증 실패는 fixture 실패다. 이 단계는 실행기 소유 기본 장면의 재구성이며 과거 scene identity 또는 사용자 장면의 복원이라고 주장하지 않는다. 사용자 interactive 상태/저장 자산 쓰기/기존 저장 장면 및 root0 예외 분기 변경0이다. 시작 guard를 root0까지 넓히지 않는다.

이후 다음 fixture는 동일한 owned-batch 권위와 기본 장면 검사를 다시 수행한다. 전체 시험 실행기 자체의 후처리와 Root의 소유 프로세스 회수도 기존대로 확인한다. 변경은 정확 기존2시험 파일의 owned 종료 분기와 필요한 동일 검증 helper 재사용에 한정하며 시험 본문·생산 동작은 불변이다. 원문 보존·설치 참조 Csc·Luna 영향 대조 후 기존 미소비 최소2→18→성능 관측 순서로 진행한다. 새 실행 예산은 추가하지 않는다.
#### §8 중복 검증 보정 수용 범위와 실제 허브 유휴 프레임 관측 — Approved

hub-validation-r3은 정확18개/18통과·실패0·건너뜀0, PID101340·30.239초·종료0, 로그 SHA256 02E1060E3697C65FBD4B611E892D86E8D73738BA0235E6F30467A5BDF5C1D26F, Root 폐쇄 DEFD6A47063CFE70472BF5E3D4726987B0686F0A104057B2B499CAC17A7A14C3으로 기존 공개 경계 음성/정상 선택의 실제 통과를 보존한다. 성능r2는 PID113656·17.938초·종료0, Hub1통과(case7.451537초), 로그 SHA256 3805F826940DE3D5FE8150587AAF40164C9A465435AD5DE862C65FFF57E1E5D9다. trueReady31.9982/32.187/34.5674ms, 추가 CurrentReceipt9.3482ms와 Pending11.6054ms를 관측했다. 기존 case129.102초 대비 개선을 확인하되 시험 조회 비용을 정상 제품 프레임 비용으로 환산하지 않는다. 최종 사용자 화면 성능 수용은 아직 아니다.

Sol은 기존 Chapter1ExpeditionPlayModeTests.cs 한 파일의 허브 비용 검사에 유휴 프레임 관측만 추가할 수 있다. 기존 Ready 성공과 동일 실제 hub/생존/실패 없는 상태 확인 뒤 추가 Ready/CurrentReceipt/View 조회·입력 발행·제품 상태 변경 없이 yield null 경계120개를 관측한다. Stopwatch 단조 시각과 Time.frameCount의 각 전후 값으로 실제 구간·프레임 증가·벽시계 간격을 기록하고 최솟값/중앙값/95백분위/최댓값 및 전체 경과를 산출한다. frameCount 증가를 실제 표시 성공이나 GPU 렌더 시간으로 표현하지 않는다. 각 구간 원시120행과 설정·실행 환경·시작/종료 상태를 합해 최대64KiB로 보존한다. 측정 중 행별 로그/파일쓰기·문자열 생성은 피하고 고정 크기 버퍼에 저장한 뒤 폐쇄한다. 종료 뒤에만 동일 hub 생존/정상 상태를 확인하며 측정 중 제품 오류는 별도 실패로 보존한다.

시간 배율·VSync·targetFrameRate·배경 실행 설정·캐시·Flow/Presenter 검증·입력/pilot/사례의 assertion은 변경하지 않는다. 기존 단회 진단에 이번 구간을 추가한 소스 차이와 정확 현재 핀을 보존하고 설치 참조 컴파일 뒤 소유 반환한다. Root는 새 경로에서 해당 허브 검사1회/1200초를 추가 관측할 수 있다. 이전 성공/소비는 그대로이며 무변경 큰 실행이 아니다. 이 결과는 배치 Editor의 무입력 프레임 진행 측정 등급이며 실제 Windows 배포 화면/OS 입력/최종 성능 검증을 대체하지 않는다.

이 한정 관측이 폐쇄되면 실제 유휴 지연·기존 시험 및 후속 실행 상한에 대한 영향을 평가해 G4 진행과 최종 성능 잔여 항목을 구분한다. 임의 성공 임계값을 사후 설정하지 않고 기존 사용자 조작/화면 수용 기준을 유지한다. 결과 전 생산 캐시나 검증 강도 변경 권위는 부여하지 않는다. G5 환경 호환의 충돌 없는 읽기 설계는 계속한다.
#### §8 원정 화면의 무행동 의미 프레임 처리 비용 보정 — Approved

hub-idle-r1은 PID121796·176.102초·종료0, 검사1통과(case164.311초)이나 성능 통과가 아니다. 닫힌 로그 SHA256 6ADEB3B6890C7B92866D4030C601ED75B6F197CDA1230E0F1B7EE1E927777562, Root 폐쇄4223F254523188E6298F800A3BBC4D4D3F03FFE6962BA4D4A2D10FD3C89BE85E, idle-120.raw.csv6547바이트/SHA256 C0AFCA6614730AD648A98FB779B287E8E19FAE2F3E8635651CDC958DFEC0FDC6를 보존한다. 120구간 총157050.9448ms·중앙1300.1761ms·95백분위1323.9451ms·최대2451.6119ms·프레임 비증가0·생존true로 배치 무입력 진행 지연을 확인했다. GPU/실제 표시 시간은 아니다.

Astra가 실제 Chapter1ExpeditionPresenter.FixedUpdate와 UiSemanticFrameCursorV1을 직접 판독했다. 현재 실제 의미 프레임 advance 뒤 아무 UI 행동이 없어도 hub.Ready 및 phase를 위한 _flow.View가 호출된다. 생산 변경은 output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/Chapter1ExpeditionPresenter.cs 한 파일의 FixedUpdate 제어 흐름에 한정한다. 원본 hub 변경/PreparedForUi/커서 생성 및 TryAdvance를 동일하게 수행하고, advance 성공 뒤 frame의 검증된 getter로 NavigateChanged && NavigateYQ4096!=0, ClickPressed, SubmitPressed 중 하나라도 있는지 확인한다. 모두 없으면 기존 deep Ready/phase View 조회 앞에서 반환할 수 있다. 클릭점 부재/변조를 가짜 무행동으로 처리하지 않고 frame의 기존 검증·예외 경계를 유지한다.

실제 행동이 있으면 기존 hub.Ready·scaler·phase·실패/반환 상태·focus·hit rect·토큰·SubmitUiIntent 검사를 순서와 의미 그대로 수행한다. 중립/포인터 이동/스크롤/취소처럼 현재 이 Presenter에서 행동을 발급하지 않는 의미 프레임도 cursor에서 먼저 소비하며 오래된 edge를 저장하거나 재생하지 않는다. 반환 상태의 focus5/실패 focus0은 실제 행동 처리 전에 기존 조건으로 설정되어야 한다. 외부 공개 getter 검증, receipt/proof 코드, 캐시, 입력 발행·물리·시간·다른 생산 파일·LateUpdate/OnGUI의 표시 경로는 변경0이다.

Sol은 이 한 파일의 원문 보존·제어 흐름 역대조와 중립/이동만/scroll/cancel/수직 navigate changed 유무·0/nonzero/클릭/Submit/조합의 분기 국소 확인, 실제 설치 참조 컴파일을 수행한다. 단순 분기 검사를 실제 사용자 입력 성공으로 확대하지 않는다. Luna는 실제 행동에서 기존 모든 guard와 focus/hit/intent 유지, 무행동에서도 연속 cursor 소비를 독립 대조한다. Root 소유 반환 전에는 생산 쓰기0을 유지한다.

이 보정 후 동일 허브 유휴120구간 검사1회/1200초를 추가 승인한다. 정확 새 소스/원래 측정 조건과 결과를 비교하며 실패/소비는 복원하지 않는다. G4의 기존 실제 출정·복귀·저장 재시도·중립 뒤 새 edge·클릭/Submit 관련 필수 검증은 유지하고 별도 전체 회귀를 이번 작은 보정마다 추가하지 않는다. 유휴 결과와 G4 행동 결과가 닫히기 전 사용자 화면 정상 수용0이다. 잔여 LateUpdate/OnGUI 비용 또는 다른 지연이 드러나면 원시로 구분하며 이번 승인을 생산 검증 도메인 전체의 최적화 권위로 확대하지 않는다.
위 무행동 조기 반환의 표시 동등성을 위해 같은 Presenter.LateUpdate의 기존 failed/recovering focus0 처리 뒤, 이미 읽은 returned가 true이면 _focus=5로 설정하는 한 줄만 추가 허용한다. returned와 failed/recovering은 기존 phase 판정으로 구분되며 새 View/Ready 조회·입력·intent 발급을 추가하지 않는다. 새 반환 hub에서 무행동 상태의 반환 버튼 집중 표시를 보존하기 위한 정확 예외이고, 그 외 LateUpdate/OnGUI 변경 금지는 유지한다. 실제 Submit/Click 처리는 기존 Fixed의 전체 guard와 반환 focus 설정을 계속 따른다. 분기·표시 동등성 국소 대조에 포함하며 별도 실행 예산은 추가하지 않는다.
#### §8 유휴 증폭 보정 후 G4 진행 판정 — Approved

idle-r2는 PID111904·22.155초·종료0, Hub1통과(case9.782508초), 로그 SHA256 8FDD26BDEAB4A38B5B669D9A56A8DABCE4F7A36FA65FB288D74CF6AD26AB357E다. 원시6197바이트/SHA256 3CA630A94F7C3F5C931A269EAB37B4D1CDB539FB08E53E6E4D7939A19989A929, Root 폐쇄 E91D3363E0D832DEA118BC1565AB97A782264F463991F9C6D775218FE9F990BE를 결속한다. 동일120구간 총3879.3517ms·중앙31.473ms·95백분위36.6164ms·최대47.709ms·프레임 비증가0·생존true, 동일 batch/background/timeScale1/vsync0/target-1/D3D12와 소스628 전후 불변을 Root가 확인했다. 이전 약1.3초/구간의 증폭 해소에 한정 수용하며 GPU/최종 Windows 화면 성능이나 모든 UI 행동의 성공으로 확대하지 않는다.

현재 v21 manifest F8CB366BBED8A0BDE6978DDB182E48CA644894B9E61576103588BD0CBA0F7677의 정확 소스를 유지하고 기승인 G4 35 및 C1C 추가3메서드의 같은1200초 묶음을 진행할 수 있다. 추가 선택은 UiSubmitClickHeldCancelAndDuplicate, RejectForgedIncompleteOrUncertainCompletion의 bool2사례, RestartPreservesV1PairButNotChapterReceipt이며 예상 총39개다. 실제 발견 이름/매개변수와 고정 선택을 대조하고 누락·건너뜀·실패를 제외하여 성공 집계하지 않는다. 이 판정은 기존 정상 단계/예산 사용이며 새 선택/실행 횟수 추가가 아니다. 별도 양분기2는 원래 Testplayer 단계에서 유지한다.

실제 행동의 입력 guard·중립/새edge·토큰·중복거절·저장/재시도 회귀 결과와 Luna 검수를 통해 해당 기능을 수용한다. LateUpdate 준비 조회 비용과 최종 Windows 표시·실제 OS 입력·사용자 체감 성능은 잔여 검증으로 명시하며 이번 배치 측정만으로 최종 배포 성능 통과를 발급하지 않는다.
#### §8 미선택 C1C 종결 검사 네 사례의 정확 선택 입력 보정 — Approved

G4/C1C-r1은 G4 정확35개 중28통과/7실패·건너뜀0으로 폐쇄됐으며, 추가 C1C3메서드/4사례는 namespace 입력 오류 때문에 선택되지 않았다. 이 누락을 통과나 검사 범위 축소로 처리하지 않는다. Astra는 현재 실제 Chapter1ClosingPersistencePlayModeTests.cs의 namespace와 세 메서드를 직접 확인했다.

현재 v21 소스를 동결한 채 AcadeGameMaker.Run.Unity.PlayMode.Tests.Chapter1ClosingPersistencePlayModeTests의 UiSubmitClickHeldCancelAndDuplicate, RejectForgedIncompleteOrUncertainCompletion(bool false/true), RestartPreservesV1PairButNotChapterReceipt만 정확 선택한 추가 실행1회/1200초를 승인한다. 실행 전 실제 전체 필터·소스 핀·입력 보정 사유를 새 계획에 기록하고 실제 발견4개의 이름/매개변수·실행/실패/건너뜀을 XML로 대조한다. 이전 G435와 이미 통과28개를 이 실행에 재포함하지 않는다. 무선택/누락/개수 불일치는 성공이 아니다.

Root가 유일 실행 소유자이며 Sol의 G4 원인 조사는 읽기 전용으로 병행한다. 실행 중 생산/시험/자산 쓰기0이고 이전 실패·소비는 복원하지 않는다. 이 실행은 보스 이후 미관측 영역의 현재 사실을 확인하는 입력 오류 보정이며 G4 나머지 실패의 수정 승인이나 최종 CH1 수용이 아니다. 종료/회수/로그·XML 폐쇄와 전후 소스 대조 후에만 소유를 반환한다.

G4 정상 두 실패의 앞선 전달에서 BossEntry.Tick로 축약한 항목을 정정한다. 현재 실제118행은 InvulnerabilityEndTick 비교이며 Tick 연속/Health 검사가 앞서 통과했다는 Sol 원문 판독을 보존한다. 만료 무적 정규화 및 G6 숫자 tick 기록의 정확 보정 범위는 나머지 원인 조사와 함께 별도로 닫고, 이 선택 보정 실행 전에는 기대값/기록을 바꾸지 않는다.
#### §8 G4 확인된 시험 보정·G5 재생 환경 호환 묶음 — Approved

G4-r1의35개 중28통과/7실패와 별도 C1C 선택 보정의4개 중1통과/3실패를 보존한다. C1C의 상자 전이 미관측2건과 의도한 저장 실패의 예상 로그 누락1건은 아직 원인/보정 수용 전이다. 이 절로 해당 실패를 통과시키거나 실패 입력을 임의 완화하지 않는다. Root가 실행 폐쇄·현재 소스 소유 반환을 확인한 뒤 Sol이 아래 정확 범위만 단독 작성한다.

1. Chapter1ExpeditionPlayModeTests.cs의 정상 원정 무적 기대값을 실제 BossEntry.Tick과 이전 InvulnerabilityEndTick으로 계산한다. 이전 종료tick이 새 최초tick보다 작으면 기존 RegisterCarriedPlayer 규약대로 -1, 그렇지 않으면 원래 종료tick을 유지한다. 정확 경계/활성 무적·체력·tick 연속 assertion은 보존한다. 이는 CH1 §8.2의 이미 만료된 무적 정규화와 맞추는 시험 보정이며 생산 변경0이다. G6 nullable tick의 중복 키/CSV는 HasValue 확인 뒤 실제 숫자 Value.Value를 사용하고 미관측/null을0으로 대체하지 않는다. 실제 tick별 표본·입력 결속은 유지한다.

2. 같은 시험 파일에서 첫 Open/재Open의 runId/seed/방/세대·준비 단계·기존 공개 중립 상태·소유 장치 ID/added/enabled/raw Enter/Navigate/Mouse와 실제 updateType/count를 제한 기록한다. 의도한 압착은 실제 seed/방/위치/tick/HP/실패 publication을 함께 기록하며 현재 낙하 결과를 압착으로 바꾸지 않는다. 각 사례 진단 최대32행/64KiB, 기록 실패가 최초 실패를 덮지 않도록 보존한다. 재Open 전용 CloseForReopen은 기존 router/graph 종료·Close/Unload 완료 뒤 이 trace가 직접 AddDevice한 정확 Keyboard/Mouse 참조만 added 상태에서 RemoveDevice하고 필드를 비울 수 있다. 다른 장치/전체 fixture Reset/상태 강제 해제0이다. 일반 최종 Close 의미는 바꾸지 않으며 재Open 호출 지점과 소유2장치 생명주기 차이를 검토한다. 이 정리는 누적 장치가 전체 원인임을 확정하는 선언이 아니다. 두 동시 실패 입력 및 상자 전이/예상 로그 보정은 정확 원인 설계 후 별도 범위로 닫는다.

3. G5의 기존44클래스(분류37 EditMode/7 PlayMode)와 모든 시험 본문·assertion·매개변수·필수 발견 범위를 유지한다. 기존 Desktop/Coordinator 두 fixture의 실제 성공 구현은 그대로 둔다. 추가 wrapper 대상은 정확 다음15개다: CombatUnity의 OrdanBossCombatInputGatePlayModeTests, OrdanBossEncounterAuthoredGraphPlayModeTests, OrdanBossEncounterHandoffAdapterPlayModeTests, OrdanBossTerminalTeardownPlayModeTests; HubPresentation의 LocalPlayableSliceLifecyclePlayModeTests; InputUnity의 Chapter1EntryPlayModeTests, Chapter1RegularInputPlayModeTests, InputRouterActualCameraPlayModeTests, InputRouterAdversarialPlayModeTests, InputRouterFailureBoundaryPlayModeTests, InputRouterPlayModeTests, InputRouterTerminalPlayModeTests, LocalPlayableSliceInputPlayModeTests, OrdanBossTerminalTransitionRequesterPlayModeTests, ProfileBindingOverrideApplyAdapterV1Tests. 경로는 모두 현재 output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Tests/PlayMode 아래 해당 폴더/클래스.cs이며 루트 Assets의 다른 사본은 수정하지 않는다.

공통 시험 전용 상태 helper는 같은 InputUnity 폴더의 OwnedBatchRuntimeFixtureScope.cs와 meta 신규1쌍이다. [Serializable] 상태와 fixture의 [SerializeField]로 reload에 필요한 원래 구성·owned-batch 구분을 보존하고, 이미 검증한 OneTimeEnter/BeginCase/CloseCase/OneTimeExit의 공개 API 경계만 공유한다. 기존 InputTestFixture 상속은 유지하며 새 생산 API/조립체는 추가하지 않는다. CombatUnity와 HubPresentation의 기존 시험 asmdef에 AcadeGameMaker.Input.Unity.PlayMode.Tests 참조 각1만 추가하고 순환 부재를 확인한다. helper는 시험 조립체에만 존재하고 배포에 포함하지 않는다.

UnitySetUp의 첫 yield 전 동기로 이름 있는 사례 장면을 생성·활성화하여 NUnit Setup 전에 준비한다. InputTestFixture를 상속한9개는 기존 TearDown/정리 본문을 보존하며 base.TearDown 호출만 지연해 graph 정지·소유 scene Unload·지연 Destroy 완료 뒤 finally에서 정확1회 호출한다. 실제 실패에서도 입력 원상복구를 시도하고 정리 실패를 숨기지 않는다. 같은 순서가 적용되지 않는 fixture에 상속이나 복원 호출을 억지 추가하지 않는다. 실제 설치 시험 실행기 순서와 각 fixture의 lifecycle 호출표로 중복/누락을 대조한다.

Single Load로 이름 장면을 교체하는 정확2개 LocalPlayableSliceLifecyclePlayModeTests/LocalPlayableSliceInputPlayModeTests만 별도 옵션을 허용한다. 기존 LocalPlayableSlice 및 해당 시험의 기존 빈 Exit 장면만 소유 교체 대상으로 식별하며 파괴된 이전 handle을 복원됐다고 주장하지 않는다. 교체 후 현재 소유 graph를 중지·정리하고 공개 빈 장면/기본 장면 재구성으로 다음 fixture 경계를 맞춘다. 불명 장면·사용자 interactive 장면·다른 프로세스·저장 자산 파일에는 적용하지 않는다. owned-batch 명령 표식/프로젝트/PID·birth/원래 기본 component 확인/마지막 실행기 후처리·Root 회수 조건은 앞선 실제 통과2fixture와 같다.

Sol은 원문 보존·기존 본문 및 pilot 불변 대조·helper 경계/장면 교체/실패 정리의 국소 검사·설치 참조 Csc를 수행하고 Luna가 변경 범위와 순서를 독립 검토한다. G5 환경 최초 관측은 일반 runtime fixture, InputTestFixture 정리, Single Load를 실제 포함하는 최소 정확3사례를 실행 전 선택표에 고정하여1회/1200초 추가 허용한다. 실제 발견3·준비/정리/후처리 성공 뒤 기승인 전체 G5 EditMode/PlayMode 단계로 진행하며 기존44를 이3개로 대체하지 않는다. G4 보정 재관측과 C1C 재실행 예산은 이 절에서 추가하지 않으며 원인 조사 묶음의 정확 선택이 닫힌 뒤 별도 결속한다. 이번 변경은 사용자 중간 조작 없이 진행하고 생산 수치·검증·중재 guard를 변경하지 않는다.
앞 절의 C1C 저장 실패를 ‘예상 로그 누락’으로 분류한 표현은 미확정 가설로 정정한다. 실제 C1C-r1 PID33832·166.0228096초·종료2의 로그 SHA256 0D7D44B95422A897CD795A0DDD8E2752AA4401E1BDEF26535F8D96A792FDD283를 보존한다. 배타 읽기 실패 뒤 저장 준비 상태와 재시도 Snapshot 경계의 제품 결함 가능성을 포함해 조사하며, LogAssert만 추가해 오류를 숨기는 보정은 승인하지 않는다. 실제 저장 stage/result와 재관측 근거가 확인될 때까지 저장 보정은 앞 절의 구현 범위 밖이다.
#### §8 C1C 상자 전이·저장 재시도 실패의 한정 원시 진단 — Approved

현재 C1C 실패는 상자 전이의 실제 phase/outcome 및 저장 실패·재시도 준비 상태가 부족하여 제품 원인을 확정하지 못했다. Sol 단독 쓰기 소유가 유지되고 Unity 프로세스가 없는 동안 기존 Chapter1ExpeditionPlayModeTests.cs의 TransferToCounterweight와 Chapter1ClosingPersistencePlayModeTests.cs의 해당 저장 실패/재시도 시험 경계에 읽기 전용 진단만 추가할 수 있다. 기존 입력 결정·발행 횟수/순서·yield·assertion·생산 상태와 예외/LogAssert 정책은 변경하지 않는다.

상자 진단은 직전 준비, 실제 입력 발행 지점, 이후 최초 결과 또는 실패의 최대3단계에서 기존 Transfer.LatestPhasePublication의 존재/식별·phase/outcome·지원되는 입력 증거, 기존 읽기 전용 카메라/조준 및 동일 player/target pose·원본 참조/세대/tick을 기록한다. 아직 publication이 없으면 미발행으로 남기고 조회 때문에 생산 초기화를 일으키지 않는다. 입력 성공을 전이 성공으로 표기하지 않는다. 저장 진단은 기존 실패 반환 직후와 재시도 직전·최초 결과/실패의 최대3단계에서 실제 공개 result/stage, 기존 접근으로 확인되는 준비 상태와 해당 사례가 소유한 정확 시험 profile 파일들의 존재/길이/닫힌 파일 SHA를 기록한다. 배타 잠금으로 읽을 수 없는 파일은 잠금 해제·우회 없이 미판독으로 남긴다. getter 자체가 거절하면 그 예외를 별도 기록하고 원래 실패를 덮지 않는다.

정확 접근 API/필드와 profile 파일 경로 목록은 Sol이 구현 전 현재 소스에서 확인하여 진단 명세에 고정한다. 새 생산 API·private 반사·장치 상태 주입·저장 복구/삭제·사용자 profile 접근0이다. 지원된 읽기로 없는 값은 미관측이며 다른 값으로 추정하지 않는다. 각 사례 최대6개 기록/총64KiB, 문자열 항목1024자, 원시 초과/기록 실패를 명시하고 잘린 성공을 만들지 않는다. 최초 시험 실패를 보존하며 기록·정리 실패를 별도 추가한다. 기존 비진단 경로와 RoomPilot 몸체는 변경0이다.

설치 참조 Csc·국소 범위 대조·소유 반환 후, 기승인 G5 환경 최소3개가 실제 성공한 경우에만 Root는 정확 C1C RejectForgedIncompleteOrUncertainCompletion(false)와 UiSubmitClickHeldCancelAndDuplicate의2사례를 새 경로에서1회/1200초 진단 실행할 수 있다. 이전 실패를 재현할 수 있으나 같은 무변경 검사 반복이 아니라 위 원시의 확보가 목적이다. 정확 발견2·원문 폐쇄·프로세스 회수·소스 전후 대조를 확인하며 전체 G4/C1C 재실행으로 확대하지 않는다. 이 실행으로 제품 수정/오류 억제/기대값 완화 권위를 발급하지 않고 실제 증거 뒤 최소 원인 보정을 판단한다.
#### §8 G5 공통 fixture의 최상위 재생 전환 보정 — Approved

G5 최소r1은 정확3개 발견·0통과/3실패·건너뜀0, PID121656·16.9413676초·종료2, 닫힌 로그 SHA256 8001C7A0807D74FFE4EA34C97A4A7550D973C0696D1A171EF383DDEBC4C0CB2A 및 Root 폐쇄76E45EFC4386E8C9C4C4CBACA4218AC78C637416781B733705491E5DEE82C174를 보존한다. 실패는 중첩 IEnumerator의 EnterPlayMode이며 실제 재생 진입0이다. Astra는 설치 BeforeAfterTestCommandBase의 준비와 정리 양쪽에서 IEditModeTestYieldInstruction의 중첩 실행을 거절하는 조건을 직접 확인했다. 종료 ExitPlayMode에도 같은 제약을 적용해야 한다.

Sol은 이미 허용된15 wrapper와 OwnedBatchRuntimeFixtureScope.cs만 보정한다. helper의 재생 전 준비/재생 후 확인/종료 전 소유 확인/종료 후 복원을 동기 함수로 분리하고, 각 fixture의 최상위 UnityOneTimeSetUp IEnumerator에서 직접 yield return new EnterPlayMode(), UnityOneTimeTearDown에서 직접 yield return new ExitPlayMode()를 수행한다. helper IEnumerator를 yield해 그 내부에서 전환 명령을 내거나 수동 MoveNext로 실행기 상태를 우회하지 않는다. 같은 공개 API만 사용하고 생산·asmdef·시험 본문은 변경0이다.

reload 전에 저장한 직렬화 helper 상태로 소유/원래 장면/복원 분기를 재결속한다. 비직렬화 iterator/지역 참조가 reload를 통과했다고 가정하지 않는다. 전환 후 실제 Application.isPlaying과 원래 소유 상태를 확인하고, 중간 실패에서도 해당 소유만 정리한다. BeginCase/CloseCase의 일반 비전환 대기·InputTestFixture 복원 순서·Single Load 예외·기본 장면 재구성·최초 오류 보존은 기존 승인대로 유지한다. 전환0·중복 전환·소유 없음·복원 실패를 통과로 숨기지 않는다.

원문 보존·최상위 전환 위치/설치 실행기 재개와 domain reload 경계 검토·설치 참조 Csc·Luna 영향 확인 뒤 Root는 동일 정확 최소3개를 새 경로에서 보정 관측1회/1200초 수행할 수 있다. 이전 실패 소비는 복원하지 않는다. 실제3 통과·정리/후처리·회수 성공 전에는 전체G5 및 조건부 C1C2 진단을 실행하지 않는다. 이번 예산은 확인된 환경 원인 보정에만 한정되며 큰 무변경 재실행은 금지한다.
실행 순서 호환: C1C 원시 진단2는 별도의 기존 성공 PlayMode RunUnity 경로이며 G5의 Editor 전용 wrapper를 사용하지 않는다. 따라서 G5 최소 성공을 C1C 진단의 필수 선행으로 둔 앞 문구는 순서 제약에 한해 대체한다. 현재 v23 원문·Csc/독립 범위 검토·소스 동결·Editor0이 확인되면 Root는 이미 승인한 미소비 C1C 진단2의1회/1200초를 G5 보정 쓰기 전에 먼저 수행할 수 있다. 실행 중 Sol은 읽기만 하며 동시 Unity/소스 쓰기0, 회수·폐쇄·전후 대조 뒤 다시 단독 쓰기 소유를 반환한다. G5 최소3 성공→전체G5 및 최종 필수44 관문은 그대로이며 C1C 진단 횟수 추가/이전 소비 복원0이다.
#### §8 G4 만료 무적·재Open·압착 원시의 보정 관측 — Approved

현재 v24 manifest34666바이트/SHA256 002D2EB21CBDA5AE34BF809DB6D15CAC3EC556D0DC398C75FA2BD6741B8D7658의 소스를 동결하고, 이미 승인한 시험 기대 정규화·owned 장치 정리·한정 원시 기록의 실제 관측을 위해 Chapter1ExpeditionPlayModeTests의 AuthoredHubThroughFourRoomsToOrdan, RestartDoesNotResumeRun, ExpeditionSaveUncertainRemainsDiagnostic, AuthoredFailureReturnsAndAllowsFreshStart 정확4개만 새 경로에서1회/1200초 실행할 수 있다. 전체 namespace는 실제 source/발견 결과로 실행 전 결속하며 파일 위치나 유사 namespace를 추정하지 않는다.

목적은 만료 무적의 실제 규약 일치, 재Open에서 정확 소유 두 장치의 정리와 새 준비 상태, 의도한 압착 대신 낙하가 관측된 실제 seed/방/위치/실패 시점의 확보다. 압착 시험이 계속 실패하면 원래 의미로 실패를 보존하며 원시 획득을 기능 성공으로 세지 않는다. 기존4개 본문/조건·입력 정책 및 승인된32행/64KiB 기록 상한은 유지한다. 동시 원인2개·전체35·C1C를 이 실행에 추가하지 않는다.

Root는 현재 핀/설치 참조 Csc·해당 변경의 독립 범위 검토·Editor0·소유 반환을 확인한 뒤 실행하고 실제 발견4/누락·실패·건너뜀을 별도로 집계한다. G5 최소 관측과 동시에 Unity를 실행하지 않으며 실행 중 모든 소스 쓰기0이다. 종료/회수·로그/XML 폐쇄·전후 소스 대조 뒤만 구현 소유를 반환한다. G5 별도 fixture 영향 검토가 진행 중이라는 이유로 이 RunUnity 경로의 준비가 자동 충족되거나 불충족되는 것은 아니며, 해당 변경 범위 검토 결과를 구분한다. 이전 실패/소비는 복원하지 않는다.

상자 전이의 저작 player 중심과 barrier entry.Position 불일치 후보는 별도 제품 경계로 유지한다. 원시 LOS 거절 원인이 아직 미관측임을 명시하고 이번 관측을 시험 점프/입력 우회나 생산 spawn/LOS 수정 권위로 확대하지 않는다.
#### §8 동일 선택 저장 재시도·저작 봉쇄선 입구의 제품 보정 — Approved

현재 Root가 두 실행 폐쇄·Editor0·소스630 불변을 확인한 뒤 Sol에 단독 쓰기를 반환하는 조건으로 다음 정확4개 생산 파일을 허용한다: Runtime/Run/Unity/Chapter1DevelopmentProfile.cs, Chapter1PlayableFlowOwner.cs, Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs, Chapter1EncounterLifetimeOwner.cs. 모두 현재 output/chapter1-playable-r1/project/Assets/AcadeGameMaker 아래 원본이며 새 생산 파일/friend/asmdef/Profile 저장 서비스 변경0이다. Astra는 현재 Snapshot/_ready·SaveProgression·Flow retry 호출과 기존 root operation lease/ObserveUnderHeldRoot/SaveUnderHeldRoot 및 barrier entry 원문을 직접 대조했다.

REQ-AUTOCH1-RETRY-001: FailedBeforeCommit인 실제 선택 저장의 원래 before canonical, 동일 candidate canonical, choice/consent/skill, expected=beforeRevision+1, 실제 save result를 private pending으로 보존한다. 불확실 저장·다른 요청·다른 선택은 이 권위가 아니다. Snapshot의 readyfalse 거절은 유지하고, Flow는 현재 원본 adapter의 새 choice-save-retry token을 기존 검증/단회 소비한 경우에만 Snapshot보다 먼저 pending 기반 재시도 입구를 호출한다. 일반 confirm·새 선택·held/중복 token이 이 경로를 대신하지 않는다.

REQ-AUTOCH1-RETRY-002: 기존 Profile friend를 가진 DesktopAdapter의 좁은 내부 bridge에서 정확 인증 root와 before/candidate/result 전체 일치를 검증한 후 기존 ProfileRootOperationLockV1.Acquire를 한 번 보유한다. 같은 lease로 ObserveUnderHeldRoot→Select/검증을 수행하고 primary가 원래 before와 바이트/수정 세대가 동일하고, temp가 정확 pending candidate이며, 다른 primary/previous 보존 또는 복구 요구가 없고 허용된 temp StaleTemp 보존만 있는 경우에 한해 기존 PreservationExecutor로 원문을 격리 보존한다. 보존 성공/MayPersist 및 재관측에서 before 동일·temp 부재·추가 보존/복구 요구 없음까지 확인한 뒤 같은 lease의 SaveUnderHeldRoot(candidate)를 호출한다. 원래 temp 부재인 실패 경로를 지원하려면 그 부재와 동일 before/요청을 따로 엄격 검증하며 다른 temp는 삭제/덮어쓰기하지 않는다. 기존 reset barrier/링크/원문/lock 검증을 줄이지 않는다. 외부 임의 작성자까지 통제하는 새 원자성을 주장하지 않고 기존 저장 서비스의 재검증·동시성 거절을 유지한다.

잠금 실패·원본 교체·다른 revision/choice/candidate·보존 실패/불확실·source missing·reset barrier는 추가 저장 중단이다. primary가 이미 candidate/expected와 정확히 같다면 새 Save0·revision 증가0·중복 완료0으로 진단 유지한다. 이를 실제 atomic save 성공 결과로 조작하거나 선택 완료로 채택하지 않는다. 정확 실제 SaveResult가 이번 expected에 commit된 경우에만 문서 채택·ready 복구·pending 해제 및 기존 CompleteChoiceSave를 통한 성공 진행을 허용한다. 실패하면 원래/현재 실패를 구분하고 사용자 명시 재시도 상태를 유지하되 자동 반복 저장0이다. 기존 일반 저장 및 불확실 결과 처리 의미는 유지한다.

REQ-AUTOCH1-SPAWN-001: TryCompleteChapter1Barrier의 비활성 원본 graph에서 실제 저작 player.transform.position을 유한값·정확 원본 역할 검증 뒤 entry.Position으로 전달한다. graph 원점이나 상수 .81을 player 중심으로 대입하지 않는다. body 크기/offset·floor·지형·LOS layer/guard·체력/무적·tick/세대·소유·입력 bootstrap은 불변이다. 같은 경로의 안전 복귀도 동일 저작 입구를 사용한다. 현재 완료pose0,0과 저작 중심0,.81 불일치는 보정 근거이며 실제 LOS가 floor를 hit했다는 미관측 사실까지 확정하지 않는다.

AC-AUTOCH1-RETRY-001/002: 기존 허용 DesktopProfileLaunchAdapterV1Tests.cs와 Chapter1ClosingPersistencePlayModeTests.cs 안에서 동일 후보 실패→잠금 해제→원문 격리→실제 commit1/revision+1·V1 pair 및 새 edge 중복0을 확인한다. 다른 primary/candidate/선택/revision, 보존 실패/uncertain, lock/barrier, 이미 candidate commit의 추가 쓰기0을 실제 지원된 격리 시험으로 검증한다. 시험 편의를 위한 실제 성공 결과 조작/LogAssert로 예상 밖 오류 삭제0이다. 정확 추가 메서드/매개변수 목록과 파일 부수출력은 실행 전 선택표에 고정한다.

AC-AUTOCH1-SPAWN-001: 같은 Closing 시험의 실제 초기 player center/body·floor 비관통·지원된 원본 LOS와 실제 상자 전이, 양분기 생산 연결을 후속 검사한다. 미관측 내부 phase는 미관측으로 유지한다. 시험 점프로 잘못된 spawn을 덮지 않는다. 기존 방 측정 입력·geometry manifest는 동결 보존하고 이 보스 이후/저장 변경의 영향은 별도 기록한다.

추가 시험 보정은 Expedition의 압착 진단 예산 배분에 한정한다. 앞선 health/fall/Open이32행을 모두 소비해 cause2 원시를 남기지 못한 실패를 보존하고, 동일 총32행/64KiB 안에 각 failure 종류별 예약 구간 및 첫/최종 원인을 보장한다. 기록을 위해 입력/yield·게임 상태를 바꾸지 않는다. G5 Entry 카메라 오류는 정상 저장 host 진입과 별도 합성 carry graph를 먼저 정확 stack/코드로 구분한다. 정상 BeginHost 경로에 InitializeForTests를 넣는 권위는 없다. 기존 합성 graph를 시험이 직접 구성·활성화하고 첫 생산 전임이 확인되는 정확 지점에 한해서만 기존 InitializeForTests를 첫 yield 전에 호출하는 시험 준비 보정을 허용하며 생산 camera guard는 불변이다. 해당 지점/원본/첫tick 검증을 독립 검토한다.

Sol은 원문 보존·국소 정상/음성·현재 설치 참조 Csc 후 반환하고 Luna는4생산 파일과 정확 시험 변경을 한 묶음으로 독립 검토한다. 이번 절은 구현 권위이며 새 Unity 실행 예산을 아직 발급하지 않는다. 실제 반환 핀·음성 선택·제품 재시도/전이/시험 준비 목적을 묶은 최소 실행 계획을 Root가 고정한 뒤 판정한다. 동시 실패2 정책과 그 실행은 이 제품 보정에 포함하지 않는다. 사용자 추가 조작 요구0·최종 CH1/Windows 수용 선취0이다.
동일 묶음의 장치 수명 호환: Astra가 현재 Chapter1ClosingPersistencePlayModeTests.RestartPreservesV1PairButNotChapterReceipt의 첫 CompleteBranch/완료 receipt 확인 뒤 중간 Close→새 trace.Open 경계를 직접 확인했다. 그 정확 중간 호출1곳만 기존 승인·실제 재Open 검사에서 사용한 CloseForReopen으로 치환할 수 있다. 원래 graph 종료/Unload 완료 뒤 해당 trace가 직접 소유한 Keyboard/Mouse만 해제하는 의미를 그대로 재사용하며 일반 최종 Close·입력/yield 순서(정리 완료 대기 제외)·held/click·저장/완료 assertion은 바꾸지 않는다. 이 경계는 앞선 전이 실패로 아직 미관측이므로 완료 성공을 선취하지 않는다. 새 파일/행동/실행 예산 추가0이며 현재 한정 구현·독립 검수에 포함한다.
#### §8 v25 저장·입구 보정의 정확 순차 관측 묶음 — Approved, 독립 검수 조건부

Astra가 retry-spawn-root-execution-selection.v1.json4758바이트/SHA256 784B7A750FB5DDCCED5CEACCF89FFF15C8027061B4BDAA4C330E02ABF0954350의 전체 원문과 핀을 직접 판독했다. v25 implementation manifest34976/SHA256 51669C9BA835807CDC81A5A6090D422708D9A94B4C5FB84C480E43E03A522830, Sol 선택v2 SHA256 68BFEBDB14B6D78C0517356934BEAB1CF665B8E4281F5A73046610709EF41AA7, 국소 결과v2 SHA256 3B871CDBEDD262F2A2EB5AF782B65623E43F76559F0F02F6E02A5108464BDE7B를 결속한다. 후보 선택 파일의 executionApproved=false는 작성 당시 사실로 보존하고 이 절이 조건부 실행 권위를 발급한다.

Luna의 정확4생산/4시험 변경 독립 검수에서 미해소 P0/P1이 없고 Root의 현재 파일핀·Csc·소유 반환·Editor0 확인이 유지되는 경우에만 다음을 실행한다. 첫째 EditMode의 Chapter1ChoiceRetryClosedEvidence 정확12매개변수 및 SeedsAllProducersAtOneTick1, 합계13개를 owned-batch로1회/1200초. 둘째 첫 실행이 정확13통과/실패0/건너뜀0·후처리/회수 성공한 뒤 PlayMode의 같은 선택표 Closing전체8 및 G7_Crush1, 합계9개를1회/1200초 수행한다. 두 실행은 순차이며 실행 중 소스 쓰기0이다. 실제 이름/매개변수/발견 개수와 선택표를 대조하고 추가/누락/중복/건너뜀을 조용히 제외하지 않는다.

추가한 기존 BothBranchesCloseOnlyAfterRealPassageAndSave의1/2 및 RejectForgedIncompleteOrUncertainCompletion(True)는 동일 제품 보정의 실제 양분기/종결·불확실 저장 거절을 확인하기 위한 정확 영향 선택이다. G4전체35·동시 원인2는 포함하지 않는다. 압착은 원시 확보 목적의 기존 실패가 다시 발생할 수 있으며 기능 성공으로 바꾸지 않는다. 실패 시 다음 큰 실행을 자동 반복하지 않고 원시·소유 회수·최초 원인·부분 결과를 분리한다.

기존 G5 Combat/Single2 통과 재사용은 Luna가 이번 helper/wrapper/참조 및 해당 생산 경로의 변경 영향을 직접 대조해 유효하다고 판정한 경우에만 가능하다. 그 결과와 새 Entry1 실제 통과를 합쳐 환경 최소3 범위 충족을 판단하되 과거 실패 XML을 수정하거나 새 실행3통과로 표현하지 않는다. 독립 영향 판정이 부족하면 이 재사용은 보류이며 기존 G5전체44·Testplayer 필수 선택·최종 Windows/OS 관문은 삭제하지 않는다. 이번 실행을 최종 CH1 또는 화면 성능 수용으로 확대하지 않는다.
#### §8 v25 저장12 통과와 기존 Entry 실패의 후속 실행 분리 — Approved

Edit13은 Desktop 정확12통과 및 Entry1의 동일 camera Start 지연 오류 실패, PID22592·24.8065935초·종료2다. 로그 SHA256 2A44D17501F016E1E35D4C74A617AC433AA23C3ADE72743673725B9B38202CB0, Root 폐쇄1AF805E05313FB1181D83BAC4A02D4C2094C079E99273C6DAE11AC997CC73CBE와 소스630 전후 불변·Editor0를 보존한다. 합성 준비 보정 뒤에도 같은 오류가 있어 실제 발생 구간은 다시 확인해야 하며, 13 전체 성공이나 G5 Entry 수용을 발급하지 않는다. 같은 실패의 큰 무변경 재실행은 금지한다.

앞 절의 Edit13 전체 통과→Play9 선행을 이번 경로 분리에 한해 대체한다. 현재 고정 v25의 실제 Desktop12 성공·Csc·Luna 독립 범위 수용을 유지하고, Root는 기존 정확 선택표의 미소비 Play9를1회/1200초 실행할 수 있다. Entry는 별도 Editor 전용 저장 host 시험이고 C1C는 기존 MainFlow/RunUnity 생산 진입을 사용하므로 해당9의 관측을 Entry 조사 완료까지 막는 필수 제품 의존으로 취급하지 않는다. 같은 camera 계층의 관련 가능성은 여전히 기록하며 Play9에서도 실제 오류가 나면 그대로 실패시킨다.

이 실행은 기존 예산의 순서 호환이며 추가 횟수/소비 복원0이다. 현재소스 동결·동시Unity0·Sol읽기만·종료/회수/폐쇄/전후 대조 뒤 쓰기 반환을 유지한다. G5 Entry와 필수44·카메라 guard·최종 Windows 검증은 미완료로 남고 저장12 성공을 전체 게임 성공으로 확대하지 않는다. Entry의 다음 변경/관측은 실제 stage·원본 camera/graph·첫tick 경계를 구분한 최소 근거 뒤 별도 판정한다.
#### §8 기존 원정 호스트의 공통 카메라 준비와 Entry 단계 관측 — Approved

REQ-AUTOCH1-CAMERA-002: Astra는 현재 실제 output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs의 CreateSeededGraph와 ActivateExpeditionGraph, Runtime/Camera/Unity/SimulationCameraDriver.cs의 Prepare/Complete 구현을 직접 대조했다. 기존 호스트의 CreateSeededGraph 끝 활성화3문장에는 공통 카메라 준비가 없고, 공통 helper는 같은 graph 상태 대입과 활성화 전후에 정확 owner/router/player/scene/첫 미소비 tick 검증을 수행한다. 이 정적 차이는 확인됐으나 앞 Entry 오류가 어느 graph에서 발생했는지는 아직 미관측이다.

현재 Play9 실행의 종료·회수·폐쇄·소스 대조 후 Root가 단독 쓰기를 반환하면 Sol은 Lifetime의 해당 끝3문장만 기존 ActivateExpeditionGraph(staging,instance,router,player,firstTick) 호출로 대체할 수 있다. 공통 helper·Camera 소스·생산 Start/Fixed 순서·guard·완료 pose 발행·carry/체력/무적/세대는 변경하지 않는다. 정상 호스트에 InitializeForTests를 넣거나 이미 소비된 tick을 미소비로 바꾸지 않는다. 기존 catch의 소유 graph 비활성화/정리/실패 전달은 보존한다.

같은 묶음에서 기존 Chapter1EntryPlayModeTests의 SeedsAllProducersAtOneTick에 정상 BeginHost 첫 yield 전/후, Completed 뒤, 합성 seed20 활성화 전, 명시 시험 초기화 뒤의5단계 및 finally 마지막1행만 추가한다. 최대6행·각4096바이트, 기존 owned graph/player/camera의 신원·장면·활성·tick/next/configured tick·결속·fault/진단을 지원된 읽기로 기록한다. 읽기 예외는 미관측으로 구분하고 원래 시험 오류를 덮지 않는다. 전역 로그 구독·추가 yield/입력/초기화·로그 억제·반사·새 API는 금지한다. finally 기록이 없으면 그 공백을 보존한다.

AC-AUTOCH1-CAMERA-002: 원문 보존, 정확 호출 치환과 기존 상태 대입/실패 정리 동등성, 공통 bootstrap의 미소비·동일 소유 거절 보존, 설치 참조 Csc 및 Luna 영향 검토 후 Root는 해당 Entry1만 새 경로에서1회/1200초 관측할 수 있다. 기존 owned-batch 환경과 회수·전후 대조를 유지한다. 정상 호스트와 합성 seed20/21의 기존 assertion 모두 통과하고 실제 단계가 구분돼야 이 검사 범위만 수용한다. 실패하면 동일 큰 실행을 반복하지 않고 새 단계 원시로 최소 원인을 좁힌다. 기존 Desktop12를 재실행하지 않으며 G5 필수44와 최종 빌드/카메라 수용은 별도다. 진행 중 Play9 결과와 관계없이 동시 쓰기/Unity 실행은 허용하지 않는다.
#### §8 v25 Play9 폐쇄와 후속 원인 분리

Root 폐쇄 A40049F4E3419D97097AED7FFC5A8F24E598A3C5464651385A256307DC1E9FFA, retry-spawn-play-r1 종료2·347.1067599초·시간초과false·로그폐쇄true·소스630 불변·Editor0을 기록한다. Astra의 실제 XML 판독은 정확9 중4통과/5실패다. AuthoredBarrierStartsAboveFloorWithOpenSight 양분기, RejectForgedIncompleteOrUncertainCompletion(True), UiSubmitClickHeldCancelAndDuplicate가 통과했다. 이를 전체 종결 수용으로 확대하지 않는다.

BothBranches(1), Reject(False), Restart의 장면 연결 준비 기한 실패, BothBranches(2)의 실제 Q 기술 사용 미관측, G7_Crush의 예상 압착2 대비 실제 낙하1을 각각 보존한다. 후속 조사는 현재 소스의 기한 검사 대상 phase와 최초 전환/완료 원시, Q 발행에서 실제 기술 승인까지의 원본 상태·쿨다운·선택 종류, 압착 목적 실행의 실제 방/위치/실패 publication을 읽기로 구분한다. 기한 확대·Q 성공값 주입·오류 로그 억제·낙하를 압착으로 대체하거나 같은9를 그대로 반복하는 권위는 없다. 이미 승인한 카메라2파일 보정만 소유 반환 뒤 진행하며, 종결/입력 정책의 추가 쓰기·실행은 정확 원인과 최소 영향 범위가 닫힌 뒤 판정한다.
#### §8 비생성 Entry 기록과 R02 진행 경계 단독 진단 — Approved

REQ-AUTOCH1-CAMERA-002 기록 보완: Astra는 PlayerMovementController의 Snapshot/NextExpectedTick getter가 EnsureMotor를 호출하고 TryReadExistingMotionForAuthoring은 비활성/모터 부재에서 생성 없이 false를 반환하는 원문을 확인했다. Entry 원시 기록은 후자의 기존 API만 사용하고 false는 미관측으로 남긴다. 특히 합성 활성화 전 기록으로 모터를 만들지 않는다. 기존 시험의 의도된 초기화와 assertion은 이 기록 보완으로 바꾸지 않는다. 실행 전 발견된 v26 기록 부작용 후보와 무실행 사실을 보존하고 v27 실제 핀/Csc/영향 검토를 결속한다.

REQ-AUTOCH1-R02DIAG-001: 성공한 R02-S02-B 원문 CSV30143바이트/SHA256 C0D7DE4A3E6E8EB71F137AA9F18EC7067B493BD0EC70EE39E82AEA407BBB7D36과 Play9 로그260393/SHA256 65A09AFE0BE93C4BB61F3303C6DD9A3B7EC1F73F5723E829B2BE3EE257B3BDC6의 정보 차이를 근거로, G7_Crush의 seed202/R02/S02-B 선행 낙하를 조사한다. 성공 CSV는 별도 입력 경로의 성공 근거이며 이번 실패 원인이나 동일 발행 순서를 증명하지 않는다.

Sol은 현재 카메라 묶음의 단일 쓰기 소유 안에서 기존 Chapter1ExpeditionPlayModeTests.cs에 읽기 전용 기록만 추가할 수 있다. RoomPilot 본문·wrapper ordinal 증가·입구 대시·실제 발행 키/포인터·Queue/Update/yield·생산 source/guard는 변경0이다. 원래 발행과 후속 완료의 실제 서수/tick을 구분하고 phase/방/세대/router/pilot, 위치/속도/지면, HP/적 수/target HP/공격 결과 및 발행 입력을 지원된 기존 읽기로 기록한다. 조회 자체가 초기화/발행/소비하는 API는 사용하지 않으며 접근 불가 값은 미관측으로 남긴다.

새 실행의 고유 증거 디렉터리에 r02-progression-diagnostic.v1.jsonl 한 파일을 배타 신규 작성한다. 총32행/64KiB 안에서 전이 최대8행, R02 x27/30/32 각 최초 경계의 직전2·해당1·후속2 표본 최대15행, 최초 적0·첫y<0·최초 실패 각1행을 예약한다. 직전 표본은 제한된 메모리 고리로 보유하며 없는 표본을 합성하지 않는다. 같은 표본이 여러 경계를 넘으면 경계 원인을 명시하고 중복을 계수한다. 상한 초과/기록 누락을 숨기지 않는다. finally에서 실제 누적 원문을 폐쇄하고 원래 게임/시험 오류를 기록 오류로 대체하지 않는다.

AC-AUTOCH1-R02DIAG-001: 기존 G7_Crush 하나만 새 경로에서1회/1200초 관측한다. 기존3000틱·사례 실시간 상한을 유지한다. 실제 다른 실패가 먼저 관측되면 즉시 기존 실패 처리를 유지하며 추가 입력0이다. x32를 넘고도 최초 실패가 없다면 필요한 후속2 완료 표본까지만 관측하고 진단 목적 종료로 명시한다. 이 조기 종료는 압착 성공이나 G7 통과가 아니며 명시 미완료로 집계한다. 실패를 피하거나 목표 지점까지 이동시키기 위한 새 정책은 없다. 원문 보존·설치 Csc·Luna 영향 확인·Root 소유 반환 및 Editor0 뒤 실행하고 소스 동결/회수/전후 대조를 유지한다. 이번 진단은 기존 큰9/35 반복 또는 추가 보정 실행 권위가 아니다. 실제 발행 차이와 낙하 경계를 확보한 뒤만 다음 입력 정책 보정을 판정한다.
#### §8 선택 기술2의 동일 캡처 관측 연결 — Approved

REQ-AUTOCH1-CAPTURE-001: Astra가 실제 TransferSimulationDriver의 new object 캡처 토큰 생성/ChoicePhaseContext 전달과 Chapter1ChoiceEnvironmentOwner.PrepareReferencePlane의 IReadOnlyList 형식 변환을 직접 확인했다. 현재 토큰은 목록이 아니므로 해당 변환은 실패한다. 이 정적 결함을 인정하되 이전 Q 실행의 모든 내부 단계가 관측됐다고 주장하지 않는다.

Sol은 기존 Runtime/Transfer/Unity/TransferSimulationDriver.cs 및 Runtime/Combat/Unity/Chapter1ChoiceEnvironmentOwner.cs 두 파일에 한해 최소 연결을 구현할 수 있다. 기존 불투명 캡처 토큰과 context 참조 동일성은 그대로 유지한다. driver는 이번 단일 TransferObservationBuilder.Capture에서 실제 생성된 관측만 현재 토큰/tick에 결속해 제한된 내부 읽기 입구로 제공하고, 환경은 형식 변환 대신 그 입구를 사용한다. 기존 Combat.Unity friend를 사용하며 새 friend/공개 API/Core context 변경0이다. 실제 목록의 외부 변경을 막는 읽기 전용 보유가 필요하며 관측을 얻기 위해 다시 Capture/SyncTransforms/물리 질의를 호출하지 않는다.

읽기는 기존 ValidatePendingChoiceContext 전체 검증과 동일 tick·원본 토큰·이번 관측 준비 완료를 모두 요구한다. 이전 틱/다른 driver·adapter/context/세대/영수증·미준비·완료 후 context·기술 중단은 거절한다. 빈 실제 관측은 빈값으로 유지하며 상자 초기 좌표나 과거 관측을 대체하지 않는다. 새 틱 시작 및 정상/실패 종료에서 관측 권위가 남지 않도록 정리한다. 환경의 현재 targetId/ObservationTick 검증·발판 크기/기간/충돌·회수 예약/검증/단회 확정은 유지한다. 캡처 토큰을 목록 자체로 바꿔 기존 환경 선행 처리와 피해 권위 순서를 바꾸지 않는다.

AC-AUTOCH1-CAPTURE-001: 동일 캡처 정상 읽기, 빈 관측, 미준비·이전/복제/외부 context·다른 tick·종료 후 거절과 추가 Capture/Sync0을 국소 확인한다. 원문 보존·설치 참조 Csc·Luna 독립 영향 검토를 현재 묶음에 포함한다. 시험 파일의 새 변경이 필요하면 기존 허용 선택 기술 시험 안에서 이 경계 검사만 추가하며 입력 성공값/기술 사용값을 주입하지 않는다. 실제 분기2 Q 사용·발판/통과/종결 성공은 후속 정확 관측 전 미수용이다. 이번 절은 두 생산 파일의 구현 범위이며 새 Unity 실행 횟수는 발급하지 않는다. Root 실행 중 쓰기0·Sol 단일 소유·기존 카메라/진단 묶음 원문 보존을 유지한다.
#### §8 종결 준비 기한의 기존 평가값 단독 관측 — Approved

REQ-AUTOCH1-CLOSINGDIAG-001: Astra는 Flow의 CheckPostBossWait와5개 호출 phase를 직접 확인했다. 동일 오류 문구만으로 PreparingChoice/PreparingBarrier/EndingSuccessfulRun/PreparingClosing/RetiringCompleted 중 실패 단계를 확정할 수 없다. 기한5초 또는300렌더 프레임과 최초 검사 순서는 유지한다.

Root가 실행0 및 쓰기 소유를 반환하면 Sol은 기존 Chapter1PlayableFlowOwner.cs 한 파일만 진단 보완할 수 있다. CheckPostBossWait의 동일 평가에서 now/frame을 지역 변수로 한 번 읽어 기존 조건과 오류 기록에 함께 사용한다. 실패 직전 원래 phase, 시작/현재/차이 시간과 frame, 어느 임계값 충족인지, fixedTime, 기존 lastTick/세대/runKey 및 choiceAdapter 참조 유무를 최대4096바이트의 실패 메시지에 남긴다. 시작/전환 시각을 재설정하지 않으며 현재 phase를 Diagnostic으로 덮기 전에 기록한다.

이미 원래 분기에서 수행하던 TryComplete/준비 상태/UI frame/종료 delegate 평가 결과를 같은 호출 안에서 지역화하고 마지막 실제 평가 시점과 함께 제한된 진단 필드로 보존하는 것은 허용한다. 단 원래 short-circuit 순서·평가 횟수는 동일해야 하며 평가되지 않은 값은 미관측이다. 진단 때문에 TryComplete/CurrentUiFrame/Ready/closure를 추가 호출하지 않는다. 새 Lifetime snapshot/API·전역 로그 구독·카메라/입력/yield/기한/guard 변경은 이번 범위에 없다. 각 대기 시작 시 진단 필드만 초기화하고 이전 단계 값을 현재 사실로 표시하지 않는다.

AC-AUTOCH1-CLOSINGDIAG-001: 기존 평가 순서·호출 횟수/throw 조건의 동등성 국소 대조, 현재 설치 Csc 및 Luna 영향 검토 후 Root는 BothBranchesCloseOnlyAfterRealPassageAndSave(1) 정확1개만 새 경로에서1회/1200초 실행할 수 있다. Entry/R02 별도 실행과 순차로 수행하며 소스는 실행 동안 동결한다. 성공하면 원래 전체 assertion을 적용하고, 실패하면 닫힌 로그의 실제 phase와 임계값·마지막 평가만 수용한다. 실패 로그를 예상 성공으로 삼키지 않는다. 기존3기한 실패/Play9 원문은 불변, 전체9 반복0·추가 제품 해결 또는 기한 확대 권위0이다. 현재 v28 캡처 보정은 새 핀으로 결속하지만 이번 분기1을 분기2 실제 검증으로 대체하지 않는다.
R02 단독 진단 폐쇄 기록: Root 보고 PID126632·종료2·24.8865136초, 정확1 Inconclusive/통과0, 소스630 불변·Editor0, 로그978D7C7668FBCAC6130748050A0B75FDD9ECC703520B8D8DB34E7FBF8E518ACE 및 폐쇄D60E88F6B6A271A4A087E31C8327AC91D36FE9D652D2419A67A6D38F0D60A628를 결속한다. 원시26행/15482바이트·전이 예약 초과1은 보존한다. 보고된 완료 표본은 tick676 x30.272/y3.520,682 x31.955/y3.034,683 x32.216/y2.923으로 출구 접근 높이와 수평 통과 순서를 보여 준다. 이번 조기 종료는 실제 낙하 또는 압착 완료 수용이 아니다. 성공 G3의 지상 출구 표본과 차이를 근거로 정상 입력 감속·착지·재접근 wrapper를 읽기로 검토할 수 있으나 제품 출구/기하/모터·RoomPilot 변경 및 새 실행은 아직 허용하지 않는다. 전이 초과 기록을 없었던 것으로 처리하거나 기존 실패 증거를 덮지 않는다.
#### §8 종결 대기 시간 단위와 R02 완료 후 시험 대시 보정 — Approved

REQ-AUTOCH1-WAIT-001: closing-deadline-r1은 정확 분기1 실패·종료2·49.5719739초, 로그BD20263EBD957E716F0F0404FCB3082E1AB98C5E60AE446A32EF8421CC6357BC 및 폐쇄A37723FC4AE190ED77A939598CB2CA130765AD67174AA8A0F63D29D5D2397284를 보존한다. 실제 PreparingBarrier의 elapsed .0331983초/deltaFrame417/secondsReachedfalse/framesReachedtrue는 렌더 속도에 종속된 조기 기한 소진을 입증한다. 준비 성공을 입증하지는 않는다.

정확 생산 범위는 Chapter1PlayableFlowOwner.CheckPostBossWait와 Chapter1EncounterLifetimeOwner의 TryCompleteChapter1ClosingHost/TryCompleteChapter1Barrier/TryCompleteChapter1ChoiceHost에 있는 해당5초 OR300렌더프레임 대기 검사다. 이 네 검사에 한해 기존 realtime 시작값부터 유한5초를 상한으로 하고 렌더 수는 진단으로만 유지한다. 시작 시각 재설정·상한 연장·timeScale/실행속도 변경0, 실제 준비/폐쇄 delegate 및 토큰/세대/소유 검증0변경이다. 시간값 비유한/역행은 거절하며5초 경계에서 기존 의미로 실패한다. 이 절은 앞선300렌더프레임 조건을 해당 경계에서만 명시 대체한다. Review 최초 Dynamic5초/300 및 일반 파기 경계 등 나머지 검사는 자동 변경하지 않는다. 실제 동일 경로에 남은 프레임 제한이 있으면 숨기지 말고 영향 목록으로 반환한다.

AC-AUTOCH1-WAIT-001: 0.033초/417프레임은 시간 기한 미소진,4.999초/큰 프레임수 미소진,5초 이상/작은 프레임수 소진,비유한/역행 거절,기존 실제 준비 실패 유지 및 원래 시작점 고정의 국소 경계 검사를 수행한다. 원문과 기존 호출 순서·횟수를 대조하고 현재 설치 Csc 및 독립 영향 검토 후 실제 종결로 확인해야 한다. 가짜 준비 성공이나 로그 억제는 금지한다.

REQ-AUTOCH1-R02INPUT-001: 성공/실패 원시 비교와 모터의 지속 대시 특성에 따라 기존 Expedition 시험 PlayInput 공통 wrapper에 한해 R02·InRoom·동일 router/세대/완료tick의 실제 전투 발행에서 적0을 확인했을 때 이번 RoomPilot 명령의 LeftShift만 제거할 수 있다. 다른 키·포인터·공격·발행 횟수·ordinal·yield 및 RoomPilot 본문/G3 직접 호출은 불변이다. 실제 대시 중 상태를 취소했다고 주장하지 않으며 새 상태/속도/위치 대입0이다. R04/R05/R06 및 다른 방·적 생존·미관측/다른 원본에는 적용하지 않는다. 정상 G6와 실패 G7의 같은 R02 진행에 동일하게 적용해 한쪽만 우회하지 않는다. 원본 명령 목록을 변경해 이후 pilot 상태에 영향을 주지 않도록 이번 발행 값만 복사/필터한다.

AC-AUTOCH1-R02INPUT-001: R02 적0의 LeftShift만 제거, 나머지 키/포인터/공격 전체 동일,다른 방/적 생존/권위 불일치 적용0을 국소 대조한다. 미확인 상태를 적0으로 추정하지 않는다. 기존 진단 원시·상한·필수 대시 assertion과 실패 목표는 유지하며 전체 정상 진행/의도 압착 성공은 실제 후속 관측 전 미수용이다.

Sol 단독 소유 반환 뒤 위 생산2파일 및 기존 Expedition 시험1파일만 보정하고, 같은 한정 검사를 위한 기존 허용 시험/국소 검사에 한해 추가한다. 카메라/캡처 보정과 기존 원문 핀을 보존한다. 이번 절은 구현 승인이고 새 Unity 실행 예산은 아직 발급하지 않는다. 실제 반환 핀·변경 영향·남은 기한 경계 목록을 확인한 뒤 분기1/2와 R02 이후 실패 목표의 최소 선택을 결속한다. 전체9/35 무변경 반복·최종 CH1 성공 선취0이다.
#### §8 v30 시간 기한·R02 입력 보정의 정확 후속5 — Approved

Astra는 반환된 `preparation/autonomous-ch1-20261021/implementation-files.v30.json` 39446바이트/SHA256 70BAF4A4D77FAE0E1802871E56703380C19E27FEC16BB3FF5B2C3559E8E9240A, 실제 생산2파일의 네 시간 조건과 기존 Expedition 시험 PlayInput 필터를 직접 읽었다. Sol 보고의 설치 참조 Csc3개 종료0·회수true와 추출 실제 C# 조건/키복사144건 통과를 결속한다. 필터 권위17음성은 조건 모형 검사이며 실제 wrapper 실행 증거로 확대하지 않는다. 실제 후속 검사는 아직 미수용이다.

REQ/AC-AUTOCH1-WAIT-001 및 REQ/AC-AUTOCH1-R02INPUT-001의 실제 영향 확인을 위해 다음 정확5사례를 하나의 PlayMode 실행1회/1200초로 허용한다. 기존 소비 이력은 복원하지 않으며 과거 retry-spawn-play-r1의347.1067599초/9사례보다 선택을 좁힌 새 원인 보정 관측이다.

- `Chapter1ClosingPersistencePlayModeTests.BothBranchesCloseOnlyAfterRealPassageAndSave(1)`
- `Chapter1ClosingPersistencePlayModeTests.BothBranchesCloseOnlyAfterRealPassageAndSave(2)`
- `Chapter1ClosingPersistencePlayModeTests.RejectForgedIncompleteOrUncertainCompletion(False)`
- `Chapter1ClosingPersistencePlayModeTests.RestartPreservesV1PairButNotChapterReceipt`
- `Chapter1ExpeditionPlayModeTests.G7_Crush`

Root는 실제 완전 수식 이름·현재 명세·원문 파일 핀·명령·정확 예상 XML 집합을 신규 실행 계획에 결속한다. 설치 Csc 결과의 현재핀 일치, Luna 독립 영향 검토, Sol 쓰기 폐쇄와 현재 Editor0를 실행 선행으로 한다. 한 실행 안에서도 정확5의 선택/실행/통과/실패/건너뜀을 각각 남긴다. Root만 실제 PID·시작 신원·단조1200초·종료/회수·로그/XML 폐쇄·source630 전후를 소유하며 실행 중 제품 쓰기0·기한 연장0이다. 기존 실행 조정 코드의 저장 위치 복원은 새 권위나 새 시험 진입점이 아니다.

G7에는 `-autonomousR02DiagnosticRoot`를 전달하지 않는다. 이 인자의 부재는 기존 승인된 본 시험 경로를 선택해 x32 진단 조기 미완료 대신 실제 R05 압착·저장·복귀·재시도 assertion을 관측하기 위한 것이다. 과거26행 진단과 상한 초과1·Inconclusive는 불변이다. 다른 실제 실패가 먼저 발생하면 기존 실패 처리를 유지하며 목표 원인을 합성하지 않는다.

종결 양분기는 실제 Q 사용·봉쇄선 통과·저장·종결을 검증하되 OpenBarrier의 한정 시작을 CH1 처음부터 완주로 표시하지 않는다. Reject(False)는 실제 완료 원문과 위조 거절, Restart는 정상 원정 진행 및 재실행의 저장쌍 유지·CH1 영수증 비복원을 관측한다. 기존 통과4의 재사용은 독립 영향 판단 범위만 허용하며 이번5통과를 새9통과로 쓰지 않는다.

남은 Lifetime Review 최초 Dynamic5초/300과 일반 파기2곳의5초OR300은 원문 그대로다. 일반 파기는 실제 파기/폐쇄를 먼저 검사하고 미완료일 때 기한을 검사하므로 이번 준비 기한 보정과 구분한다. 해당 경계에서 실패하면 원시·실제 단계로 원인을 좁히며 선제 완화하지 않는다. 새 무변경 반복/추가 입력정책 권위0이며 G4 잔여·G5 고정44·Testplayer 정확18·Windows 최종 빌드/실제 창·Luna 최종 검수는 계속 별도 필수다.
#### §8 v30 후속5 폐쇄와 봉쇄선 실제 통과 미완료

Astra가 `wait-r02-focused-r1/results.xml` 및 Root 폐쇄를 직접 읽어 정확5 중 G7_Crush1통과·종결 관련4실패·건너뜀0을 확인했다. Root 기록 PID40052·종료2·284.6916718초, 로그EA31C2DB311EF345B62EE4CC56BA4156B8A269C88DE5651689C937E6BAB110F9, 폐쇄EBFD102190BD27C396E33AA015FF1C98EB3FABB5000A502C324A9C431BC836A2, source630 전후변경0·Editor0·정확선택 차이0을 결속한다. 위 단회 예산은 소비 완료다.

직전 절의 OpenBarrier가 한정 시작이라는 설명을 정정한다. 실제 helper는 Open→PlayToChoice→SelectBranch로 원정과 보스를 거친다. 해당 경로를 생략한 시험이라고 설명하지 않는다. 이번4는 UseBarrierSkill의 실제 HasActualSkill 확인 이후 PassBarrier1200의 출구·종결 미도달로 실패했으며, 실제 통과·종결·재시작 assertion 도달을 입증하지 않는다. 준비기한 오류와 Q 미사용 오류가 이번에 없었다는 범위만 관측하고 전체 종결 수용을 발급하지 않는다.

G7_Crush는 기존 실제 압착·저장 복귀·새 원정 재시도 본문 통과로 한정하며 자동 합성 입력 증거다. R02 입력 보정의 해당 실제 목표 진행을 인정하되 다른 G6 계획 전체 또는 최종 Windows 검증을 대체하지 않는다.

다음 읽기 원인은 실제 봉쇄선 초기·시간경과 기하, 상자의 낙하와 기술 피해/발판의 실제 작동, 출구 접촉 및 PassBarrier 발행 입력을 구분한다. HasActualSkill만으로 GateOpened나 출구 통과를 추정하지 않는다. 같은5 무변경 반복0·제품/시험 신규 쓰기0·새 Unity 예산0이다. 정확 원인 또는 필요한 최소 관측안을 같은 절에서 판정한 뒤 진행한다.
#### §8 봉쇄선 기술 이후 실제 지지·접촉의 단독 관측 — Approved

REQ-AUTOCH1-BARRIERDIAG-001: Astra가 실제 Builder의 왼쪽 바닥 x[-3,1]·오른쪽 바닥[4.5,9]·지지대[4,5]·초기 비활성 다리[1,4.5], Environment의 기술1 즉시 HasActualSkill→36틱 뒤 피해→다음 캡처 다리 활성 순서 및 기술2 실제 IsTouching 발판 접촉 조건을 직접 확인했다. 현재 PassBarrier는 HasActualSkill 이후 즉시 오른쪽 이동하며 x>3일 때만 점프한다. Flow는 y<=-2에서 같은 기술 세션으로 새 봉쇄선 graph를 만든다. 이 정적 차이는 원인 후보이며, 이번4실패의 실제 낙하/재생성/접촉 원인은 아직 미관측이다.

Root가 실행 폐쇄·쓰기 소유를 반환하면 Sol은 기존 `Tests/PlayMode/RunUnity/Chapter1ExpeditionPlayModeTests.cs` 한 파일에 기록만 보완할 수 있다. Q 발행 전, 최초 HasActualSkill, PassBarrier 시작과 최초 gate/bridge/platform/contact 변화, 세대·graph 변화, x1 이탈/y0/y-2 경계, 실패 직전 및 finally 요약을 포함한다. 사례당 최대24행/64KiB, 각 행 최대8192바이트이며 종료 요약1행 공간을 예약한다. 반복 재생성은 최초 전이만 상세 기록하고 나머지는 개수와 상한 초과 여부로 남긴다. 원래 발행 ordinal/키·포인터와 완료 tick의 관측을 구별하며 없는 이전 표본을 합성하지 않는다.

기존 비생성 읽기만 사용해 phase/방/세대/runKey/router·graph 신원, player.TryReadExistingMotionForAuthoring의 실제 위치·속도·지면, Environment.HasActualSkill/GateOpened, 정확 자식 collider의 enabled/bounds 및 player.IsTouching(platform), CurrentChapter1Passage 유무, BarrierCombat.LatestPublication의 원본 tick/세대/요청·결과·HP를 읽을 수 있다. publication과 현재 owner/graph/완료 tick의 결속 여부를 함께 기록하고 불일치 자료를 현재 완료 사실로 합치지 않는다. 접근 불가·읽기 실패는 미관측으로 남기고 원래 시험 오류를 덮지 않는다. getter가 초기화하는 Snapshot/NextExpectedTick을 기록에 새로 호출하지 않는다. 생산 API/friend/반사·SyncTransforms·물리 질의 추가·기술/피해/속도/기하·입력/yield/ordinal·기한 변경0이다. IsTouching은 위 명시된 기존 접촉 읽기만 허용하며 접촉을 생성하기 위한 물리 갱신0이다.

AC-AUTOCH1-BARRIERDIAG-001: 기존 입력/평가/순서·호출 횟수 불변, 기록 상한·누락 표시·원래 예외 보존, 설치 Csc와 Luna 영향 검토 및 현재핀/쓰기 폐쇄 뒤 Root는 BothBranchesCloseOnlyAfterRealPassageAndSave(1)/(2) 정확2만 신규 경로에서 한 실행1회/1200초 관측할 수 있다. 원래1200틱·assertion을 유지하며 기존4/5 전체 반복0, R02 진단 인자0이다. 실행 전 정확 선택 집합·현재핀/630파일·PID/시작 신원·기한을 결속하고 실행 중 동결·종료/회수/원문 폐쇄·전후 대조를 유지한다. 통과하면 해당 실제 assertion만 수용하고 실패하면 실제 지지/접촉/세대 경계 증거만 받아 다음 최소 보정을 판단한다. 이번 절은 진단이며 기다림/점프 정책 변경이나 제품 완화 권위가 아니다.
같은 BARRIERDIAG 기록 범위에 실제 player Rigidbody2D.bodyType/useFullKinematicContacts와 발판 body 유무·형식, 몸체 bounds.min.y에서 발판 bounds.max.y를 뺀 실제 간격을 포함한다. Root가 확인한 Kinematic/useFullKinematicContacts=false와 정적 발판의 조합은 접촉 미생성 후보이며 이번 실제 접촉을 아직 입증하지 않는다. 기존24행/64KiB 안에서 읽고 물리 설정/접촉 판정 변경0을 유지한다.
#### §8 봉쇄선2 원시 폐쇄·원인 경계

Astra는 `barrier-diagnostic-r1/Editor.log`의 CH1BarrierDiagnostic22행(사례당11행, dropped0/readFailures0)과 기존 소스를 직접 대조했다. Root 기록 PID46388·종료2·137.112초·정확2실패/건너뜀0, 로그AC8D92531EAFABA2E549FF18E7CF857404FDEC6D71744B9E52429B70F3BAE2EE 및 폐쇄72227ED3E55DA04B8B3055FB93AB46A1A579291D5489ACBF8E3DB6782EB83FDB, source630 전후변경0·Editor0을 결속한다. 진단 실행1은 소비 완료이며 새 반복 예산0이다.

분기1 실제2024틱 기술확정 뒤2035틱 x1.075/다리 비활성,2050틱 y-.086,2060틱 gate열림/다리활성이지만 이미y-1.247,2068틱 y-2.132 재생성 순서가 관측됐다. 새 graph는 기술미사용 상태이고 기존 PassBarrier는 기술을 재사용하지 않아 실패가 반복됐다. 기술 예약의 실제 피해/다리 활성보다 시험 이동이 먼저 바닥을 벗어난 경계로 한정한다.

분기2 실제2022틱 발판활성·발바닥간격0·groundedtrue·IsTouchingfalse·Kinematic/fullKinematicContactsfalse,2033틱 x1.075에서도 같은 접촉false,2171틱 발판만료와 x8.158/y2.641,2197틱 y-2.190 재생성을 관측했다. GateOpened는 끝까지false였다. 특정 발판을 지지한 원본 collider는 이 기록에 없으므로 grounded/좌표만으로 발판 접촉 권위를 대신하지 않는다. 운동 생산자의 기존 cast/충돌해결 결과와 같은 완료tick·원본 collider를 결속하는 설계, 시험의 실제 준비 이후 이동 및 출구 높이 접근 정책을 읽기로 좁힌다. 생산 물리 설정·새 질의·완료 권위 완화·추가 쓰기는 정확 설계 승인 전0이다.
#### §8 환경 최소3 재사용과 고정 G5의 순서 호환 — Approved

Luna의 v31 독립 영향 대조로 기존 g5-fixture-minimal-r2의 Combat/Single Load2 실제통과와 camera-entry-r1의 새 Entry1 실제통과를 합친 환경최소3 범위를 수용한다. 기존 두 사례/helper/meta/관련 asmdef의 원문 동일, 현재 Transfer의 선택port 없는 기존 처리 동일 및 CH1 선택/저장/종결 변경의 해당 경로 비도달, 새 Entry 성공 원문 동일과 이후 종결대기 보정 비도달을 근거로 한다. 과거 Entry 실패를 성공으로 수정하거나 새 실행3통과/전체G5 성공이라고 표시하지 않는다.

기존 G3→G4→G5 나열을 이번 정확 검사 순서에 한해 호환한다. G4의 미완료 동시원인2 및 봉쇄선 종결 미완료는 그대로이며 전체 수용의 필수 조건이다. 그러나 해당 실패 시험이나 봉쇄선 완료는 고정44클래스의 시험 입력/생성 선행이 아니므로, 환경최소3이 충족된 현재 v31에서 G5 검사를 먼저 진행할 수 있다. 이는 후속 검사와 충돌 없는 읽기 설계를 진행하기 위한 순서 호환이며 G4 면제/실패 합산 수용이 아니다.

Root는 현재 v31 manifest39941/SHA256 6483918BCAE346D196BD5EE12CF0E64856F3DA6C9B2324AD9978FE5539EE0A59와 고정 `g5-mode-impact-plan.v1.json`의 정확44클래스(실제 조립체 기준 Edit37/Play7)를 대조해 기존 미소비 정상 G5 EditMode 단계1회/1200초를 실행할 수 있다. PlayMode7의 기존 정상 단계 예산은 보존하되 이번에는 Edit37부터 실행하고 결과/실제 영향 뒤 진행한다. 추가 실행 수 발급이나 소비 복원0이다. 현 단일계약 정상단계 각1200초를 적용하며 실행 중 상한을 늘리지 않는다.

실행 전 Root의 신규 선택계획에 실제 완전수식 클래스37·필터·명세/630파일·설치 Unity·PID/시작 신원·기한·출력 경로를 결속한다. XML의 유일 classname 집합이 정확37과 일치하고 모든 실제 case의 결과/실패/건너뜀/미발견을 별도 집계해야 한다. 37은 클래스 수이며 사례 수로 쓰지 않는다. 실패/건너뜀을 제외해 성공집합을 맞추거나 필터를 축소하지 않는다. 기존 공개 실행 조정의 클래스집합 대조는 새 시험 또는 새로운 실행 권위가 아니다.

Sol은 실행 중 설계 읽기만 수행하고 현재 모든 소스/자산 쓰기0을 유지한다. Root만 시작·단조기한·정확 프로세스 회수·XML/로그 폐쇄·source630 전후 대조를 소유한다. 종료·회수·폐쇄 후에만 봉쇄선 구현 쓰기를 반환한다. 실행 환경/컴파일 실패와 기능 실패를 구분하며 무변경 전체 반복0, 현재 G5 결과를 향후 Movement/Environment 보정 후 자동 최종회귀로 재사용하지 않는다. 이후 정확 변경 영향과 Luna 판정으로 재사용/필수 재관측을 분리한다.
#### §8 같은 완료 틱의 실제 발판 지지와 봉쇄선 입력 보정 — Approved

REQ-AUTOCH1-SUPPORT-001: 기존 운동 생산자의 실제 하방 cast 선택을 같은 완료 틱의 지지 증거로 보존하고, 기술2 발판을 실제 지지한 경우에만 문을 연다. Astra는 Movement의 ReadValidatedContacts/ResolveAxis/CommitTick·ConsumeCoordinatedReceipt, Lifetime의 완료 view 전달, Environment의 기존 발판 접촉 판정을 직접 대조했다. Transfer.LatestCaptureReceipt는 조준용 형식이므로 전역 입력 영수증으로 비교하지 않는다.

정확 생산 변경은 `Runtime/Movement/Unity/PlayerMovementController.cs`, `Runtime/Combat/Unity/Chapter1ChoiceEnvironmentOwner.cs`, `Runtime/Input/Unity/Chapter1EncounterLifetimeOwner.cs` 세 파일이다. 기존 `Tests/PlayMode/RunUnity/Chapter1ExpeditionPlayModeTests.cs`의 봉쇄선 입력 보정을 포함한다. 같은 경계의 실제 시험 추가에 한해 기존 `Tests/PlayMode/Movement/PlayerMovementControllerPlayModeTests.cs` 및 `Tests/PlayMode/RunUnity/Chapter1ChoiceBarrierPlayModeTests.cs` 두 파일을 추가 허용한다. 모든 경로는 현재 단일 프로젝트 Assets/AcadeGameMaker 기준이며 기존 meta·다른 원문은 보존한다. 새 파일/공개 API/Core 계약/friend/조립 참조/물리 설정 변경0이다.

Movement는 원래 ReadValidatedContacts의 하방 FindBestHit 및 ResolveAxis의 하방 Y 해결에서 이미 선택한 Hit만 보존한다. cast/Sync/필터·정렬·모터/MovePosition·게임 값/기존 질의 순서·횟수는 바꾸지 않는다. 원래 ConsumeCoordinatedReceipt 검증을 통과한 이번 원본 영수증을 지역 값으로 보존하되 기존 소비·ordinal/epoch 검증을 그대로 유지한다. 전역 영수증 없는 일반 시험/운동은 지지 증거를 발급하지 않아도 기존 운동을 계속한다.

새 틱 시작에 이전 증거를 폐기하고 기존 Commit과 모든 후처리 성공 뒤에만 이번 증거를 게시한다. 하강 Y 해결이 위쪽 법선의 정확 collider를 선택했으면 그 원문을 우선 사용한다. Y변위0·수직속도0의 정지는 원래 하방 probe가 실제 선택한 collider만 후보로 한다. 단순 grounded 또는 다른 collider/좌표만으로 지지를 만들지 않는다. 이동 전 probe와 이동 후 완료 지지를 구분하며 최종 해결 XY로 계산한 동일 원본 수직 capsule의 몸체가 정확 지지 상자와 엄격한 수평 겹침·발바닥에서 상단까지0..기존Skin82Q4096 간격을 만족해야 한다. 하방 법선 기준은 기존 NormalQ4096을 사용한다. 플레이어 원본 size/offset/scale/rotation, 지원 collider의 원본 활성/shape/scene/transform/bounds가 관측과 읽기 사이 바뀌거나 비유한·지원 밖이면 증거없음으로 거절한다. 지원 밖 기하 때문에 원래 정상 운동에 새 예외를 만들지 않는다.

증거는 현재 controller·player collider·support collider·원본 receipt/tick·원래 선택 근거·완료 XY와 결속한다. 새 내부 읽기 입구는 초기화/질의/상태 변경 없이 현재 활성·모터 존재·이번 완료 tick/receipt·정확 요청 collider·불변 기하를 확인한다. 점프/상승·옆벽/천장·지지면 이탈·다른 원본/과거 tick/다른 receipt·실패/disable/새 lifecycle 뒤는 거절한다. 오류가 나면 이전 증거가 남지 않게 하며 원래 오류를 숨기지 않는다. 매틱 새 문자열/목록/증거 객체를 무제한 할당하지 않는 제한된 값/필드 보유를 사용한다.

Lifetime은 기존 실제 `completed.Tick`, opaque completedView와 같은 `completed.Receipt`를 Environment에 함께 전달하는 변경만 한다. 기존 전체 완료 source/캡처/세대/run/소유 검사·처리 순서는 보존한다. Environment는 기존 ValidateChapter1CompletedBarrierView를 먼저 유지하고, 현재 branch2/실제 기술/현재 발판 예약·생성 이후/150틱 만료 전/동일 graph/원본 플레이어 및 발판에 대해서만 새 지지 읽기를 확인한다. 이 정확 분기의 `_playerCollider.IsTouching(_platform)` 권위를 원본 완료 지지 증거로 대체한다. OR로 옆면/하부 접촉이나 좌표 추정을 추가 허용하지 않는다. 기존 문 열림 후 단회 상태·발판 수명150/상자 고정60·반환/정리·저장/통과 guard는 그대로다.

REQ-AUTOCH1-BARRIERINPUT-001: 기존 Expedition의 PassBarrier는 첫 분기의 실제 GateOpened와 정확 Bridge.enabled 및 같은 원본 완료 tick을 확인할 때까지 중립 입력으로 최대60틱 기다릴 수 있다. 이 대기는 기존 PassBarrier1200틱 안에 포함하고 원래 source/세대가 바뀌면 정상 준비로 삼지 않는다. 둘째 분기는 발판 생성 뒤 실제 이동을 허용하되 Grounded만으로 기술/문 열림 성공을 주입하지 않는다. 기존 Q/전이 발행·기술 사용 assertion·RoomPilot·다른 원정 입력은 변경0이다. 출구 높이 접근의 추가 감속/재접근 정책은 실제 완료몸체·출구기하·속도와 유한 입력 설계를 반환한 뒤 같은 묶음에서 판정한다. 이번 승인만으로 점프 정책 전반이나 출구 판정을 완화하지 않는다.

AC-AUTOCH1-SUPPORT-001: 실제 기존 cast→완료 지지의 정지/착지 정상, 왼쪽 바닥과 겹칠 때 다른 collider를 발판으로 대체하지 않음, 수평 이탈·점프·상승·천장/옆면·외부 발판·다른 receipt/tick/view·생성 전/만료 후·기하 변화·disable/실패 후 거절을 검사한다. 원래 운동 결과/질의 수/입력 소비가 동일함을 대조하며 실제 지원 없는 모형 검사와 실제 컴포넌트 검사를 구분한다. 시험의 고의 fixture 상태 구성은 합성 경계 검사로만 표시하고 정상 플레이 증거에 섞지 않는다. 새 내부 증거 읽기에 반사/직접 proof 주입은 금지한다. 기존 시험 fixture의 지원된 초기화/소유 입력으로 실제 증거를 얻는다.

AC-AUTOCH1-BARRIERINPUT-001: 준비 전 이동0·실제 준비 뒤 이동·60/1200 상한과 원래 성공/실패 assertion 보존을 대조한다. 국소 검사1묶음과 확인된 원인 보정 후 최대2묶음, 설치 참조 Csc·Luna 영향 검토를 같은 작업 원장에 남긴다. G5Edit37 종료·회수·원문 폐쇄·source630 대조와 Root의 소유 반환 후 Sol 단독 구현을 허용한다. 이 절은 구현/국소 검사 승인이고 새 Unity 실행 예산은 아직0이다. 반환된 정확 핀·실제 경계시험·영향 목록 뒤 현재 미소비 G5 Play7 및 최소 봉쇄선 후속을 같은 계약에서 결속한다. G4동시원인2·G5실패71·최종 Testplayer/Windows 관문을 면제하지 않는다.
같은 SUPPORT/BARRIERINPUT 묶음의 최종 세부 승인: Movement 기존 시험 파일에 `CompletedSupportUsesOriginalGroundOrDownwardHitAndRejectsForeignOrExpiredReceipt`, RunUnity 기존 ChoiceBarrier 파일에 `RejectStaleForeignOrExpiredCompletedSupportBeforeGateOpen`을 추가하고 기존 AuthoredSolidarityOneValidSkillPasses에 실제 Kinematic/fullcontactsfalse·정적발판 및 원본완료 기반 문 열림 검사를 더할 수 있다. 기존 공개/내부 지원 fixture의 Register/Prepare/Commit/Step으로 증거를 얻으며 proof 직접설정0이다. InputFrameCommitReceipt는 값형이므로 같은 유효 값 복사를 위조로 거절하지 않는다. 거절 대상은 다른 tick/frameOrdinal/modeEpoch/mode·source/controller 조합이며 opaque view는 기존 원본객체 동일성을 유지한다. 실패를 예상값 완화로 없애지 않는다.

출구 접근 정책도 같은 Expedition PassBarrier1200틱 안에서 허용한다. 현재 graph/세대/완료tick의 몸체·비생성 motion과 실제 Counterweight/Exit.bounds를 읽는다. 상자 접근에서 정상 D+Space 새 edge를 발행하고 몸체 minX가 상자 maxX+기존 skin을 넘을 때까지 유지한 뒤 해제하며, 상자 이후 추가 점프0으로 한다. 현재 수평속도와 기존 지상/공중 감속상수로 계산한 정지거리(v²/2a)+한틱 이동+skin 안에서 반대 방향 정상키로 제동할 수 있다. 실제 Exit 반폭에서 플레이어 반폭과 skin을 뺀 유효 내부구간을 목표로 D/A/중립을 선택하고, 구간 안에서 수평속도가 작은 경우 중립으로 착지를 기다린다. 구간 밖의 작은 재접근도 같은 루프 안의 일반 입력만 사용한다. 기하/속도 비유한·유효 내부폭 부재·동일 완료원문 미확인에는 진행 성공을 추정하지 않는다.

높이와 수평위치를 직접 대입하지 않으며 실제 생산 GateOpened·완료몸체/출구 교차·Closing 도달 assertion만 성공 권위다. 관측 경로와 새 입력 서수를 기존 제한 진단 안에 구분하고, 세대/graph가 바뀌면 이전 box/exit/motion을 폐기한다. 직접 Q 재발행이나 무한 재시도·총1200 밖 추가 대기·기술/통과값 주입0이다. 정상 입력 정책의 국소 검사는 제동방향·상자 뒤 점프해제·출구폭/기하거절·세대변경과 기존 상한 보존을 포함한다. 이번 추가는 앞 절의 출구정책 보류만 해소하며 새 Unity 실행예산0을 유지한다.
#### §8 G5 Edit37 실제 폐쇄·실패 원인 분리

Root 기록 g5-edit37-r1은 PID37196·종료2·179.4516423초, 로그60A58259D4424FFAE8D09459E3491C62DA24FB755A01BE545B3496FA70DDD53A 및 폐쇄627EE1BF299CAE1667B7D11B14BA2636233E848F43ED399A4421E421CBC071CA다. Astra가 실제 XML의 실패 클래스 분포와 폐쇄를 읽어 정확37클래스/590사례·519통과/71실패/건너뜀0·클래스차이0·source630불변·Editor0을 결속한다. 기존 Edit 정상1은 소비 완료이며 큰37 무변경 반복0이다. Play7 기존 미소비 예산은 보존한다.

실패 분포는 LocalPlayableSliceAuthoring1, AuthoredGraph14, Handoff3, Terminal29, Entry13, RegularInput3, LocalSliceInput8이다. Sol 읽기 조사에서67건은 사례 사이 재생 종료 뒤의 공통 fixture 준비 실패로 분리됐으며, 첫 본문3은 Editor 실행기의 WaitForFixedUpdate 처리와 실제 재생 coroutine 의미 차이 후보, 저작1은 현재 CH1 BuildSettings와 옛 단일장면 기대의 충돌이다. 이를 제품 결함67건이나 이미 해결된 실패로 표시하지 않는다. 설치 지원 실행 경로·본문 yield/정리/원문예외 의미와 실제 저작 계약을 먼저 대조하고, G5 helper/wrapper·기대값·제품 신규 쓰기 권위는 아직0이다. 봉쇄선 승인6파일 구현과 충돌 없는 읽기 설계만 계속한다.
#### §8 G5 시험 실행 의미 보존의 준비안 — Approved 준비 전용

REQ-AUTOCH1-G5BRIDGE-PREP-001: 설치 실행기 읽기 조사에서 RequiresPlayMode 지원은 발견되지 않았고, PlayModeTestsAttribute는 현재 asmdef 추적 조립체의 EditorOnly 분류를 바꾸지 않는 것으로 보고됐다. 실제 설치 파일·행과 판단 근거를 준비안에 보존한다. native PlayMode를 위해 EditorOnly/UnityEditor 참조/배포제외를 바꾸는 확대는 승인하지 않는다.

별도 지정 Sol은 `preparation/autonomous-ch1-20261021/` 아래 고유 G5 준비 파일에만 계획·정확 후보 diff·국소 검사 설계를 작성할 수 있다. 현재 봉쇄선6파일의 단독 제품쓰기와 분리하며 실제 Assets/ProjectSettings/helper/wrapper/asmdef 변경·Unity/Csc 실행0이다. 적용은 구현 권위 확정 및 Root의 소유 전환 뒤 별도다. 후보 대상은 기존 OwnedBatchRuntimeFixtureScope.cs, 앞 §8의 정확15 wrapper, 기존 LocalPlayableSliceAuthoringTests.cs 한 파일의 두 메서드뿐이다. 다른 파일이 필요하면 현재 파일/API 근거를 먼저 반환한다.

준비안은 사례별 재생 진입/준비→기존 본문→graph/장치 정리→재생 종료를 설치 실행기와 맞춰 명시한다. 시험용 MonoBehaviour host가 실제 Unity coroutine에서 원래 WaitForFixedUpdate/YieldInstruction/AsyncOperation을 실행하고 바깥 Editor iterator는 완료/원문예외만 유한 대기하는 후보를 검토한다. 실제 원래 본문의 IEnumerator 체인·yield 순서·assertion·expected/매개변수는 보존하며 자동 변환은 변환 후 각 원문 본체가 동일한지 대조한다. synchronous Test와 UnityTest, 중첩 열거자/사용자 CustomYieldInstruction/지원 밖 yield를 구분하고 지원 밖은 조용히 통과시키지 않는다.

중첩 MoveNext/Current/Dispose 예외를 최초 원문 stack으로 전달하고 finally·역순 Dispose·중복 호출 방지를 설계한다. NUnit/LogAssert의 같은 사례 문맥과 예상 로그 소비가 실제 유지되는지 최소 검증을 필수로 한다. exception을 Debug.Log해 원래 실패를 중복/다른 로그 오류로 바꾸지 않는다. Single 장면교체에도 소유 host를 보존하고 완료/실패/timeout마다 정확 host와 coroutine/열거자·소유 장면/장치를 정리한다. 의도치 않은 host소멸/reload/재생종료는 실패다. Enter 전·Exit 뒤 reload의 상태보존과 body 중 reload 거절을 구분한다. 사례/외부1200초의 실제 제한과 종료정책은 설치 기본값을 확인해 준비안에 제시하며 현재 실행 예산을 늘리지 않는다.

저작 검사는 두 지정 메서드 안에서만 원래 BuildSettings의 path/enabled/order 배열을 보존→기존 LocalSlice 단독 입력을 격리 적용→원래 정상/음성 검증→finally 정확 복원하는 후보로 한다. 생산 validator 조건이나 현 CH1 빌드 구성을 영구 변경하지 않는다. 복원 실패를 성공으로 숨기지 않으며 Root 전후 파일 대조에 포함한다.

AC-AUTOCH1-G5BRIDGE-PREP-001: 준비 산출물은 정확17소스 후보 목록과 실제 before핀, 원문 본체 보존 대조 방식, 연속2사례·첫tick0/1·원래 handoff/terminal·Single교체·중첩 assertion/예외·timeout정리·LogAssert 문맥·저작 정상/음성/설정복원 검사표, 실제 적용 순서/소유와 유한 실행안을 포함한다. 준비 완료는 구현/회귀 수용이 아니며 독립 영향 검토·정확 구현 승인 전 실제 적용0이다.
SUPPORT-001 동일 묶음의 캡슐 끝부분 거절 보완 — Approved: Luna 사전 검토의 둥근 하단 부분 겹침 위험을 반영한다. 기존 실제 cast/위쪽법선·원본 collider·완료tick/receipt·불변 기하와0..Skin 간격 검증에 더하여, 최종 수직 capsule 최하점의 x인 중심축이 정확 정적 BoxCollider2D의 minX/maxX 엄격 내부에 있어야 지지 증거를 허용한다. 회전0·지원된 수직capsule/상자 및 양수 유한 scale 한정은 유지한다. 둥근 가장자리만 겹치고 중심축이 범위 밖/끝점이면 증거없음으로 보수적으로 거절하며, 원래 운동/충돌 결과를 바꾸지 않는다. 기존 승인 Movement 경계 메서드에 실제 수평 이동으로 중심이 지지면 밖이되 몸체 bounds는 부분 겹치는 음성을 추가한다. 추가 질의/지원형상 확대/물리설정/실행예산 변경0이며 현재 단일쓰기 안에서 보완한다.
G5BRIDGE-PREP-001 경계 fixture 준비 보완 — Approved 준비 전용: 기존 후보 OwnedBatchRuntimeFixtureScope.cs 안에 별도 `OwnedBatchRuntimeFixtureScopeBridgeTests` 클래스를 준비할 수 있다. 새 파일/meta/asmdef0이며 고정44클래스에 몰래 합치지 않고 별도 집중선택으로 관리한다. 정확 준비 메서드는 `NestedFailuresPreserveFirstStackAndFinally`(MoveNext/Current/Dispose 원인별), `CustomYieldAndUnsupportedValueKeepFailureMeaning`, `ExpectedLogIsConsumedOnceInOriginalCase`, `UnexpectedLogRemainsRunnerFailure`, `OwnedHostLossIsFailureAndDisposesOnce`, `OneRealTimeoutStopsAndDisposesOwnedBody`다. 실제 사례 이름/개수를 준비 선택표에 고정하고 기존39본문 보존을 유지한다.

고의 열거자/assert/host소멸 오류는 바깥 시험에서 최초 ExceptionDispatchInfo 원인·stack/단회 역순 Dispose와 정리 결과를 예상으로 확인한다. 로그 오류를 모두 예외처럼 삼키지 않는다. UnexpectedLogRemainsRunnerFailure는 실제 실행기의 Failed를 예상하는 별도 음성 탐침으로 준비하고 정상 통과 집계에서 제외한다. LogAssert.Expect로 예상 밖 로그를 사후 등록하거나 전역 ignore로 성공시키지 않는다. 실행 승인 때 이 한 사례의 정확 예상 Failed와 오류 문구를 별도 고정해야 하며 준비안은 실행 권위가 아니다.

실제 timeout은 OneRealTimeout 한 사례만 준비한다. 동일 중단/Dispose 구현을 사용하는 시험전용 짧은 단조기한(예:250ms, 최대2초)을 명시적으로 주입할 수 있으나 정상 wrapper의160/170초 기본 정책을 바꾸거나 실시간 clock/timeScale을 바꾸지 않는다. 무한 body는 이 고정 짧은 기한에서 반드시 중단·소유 host/스택 정리가 관측돼야 하고 실패/정리미관측을 성공으로 표시하지 않는다. 다른 timeout 경계는 합성 기한 판단으로 구분한다. 새 클래스의 Root발견·기존 NUnit 문맥·Single교체/설정복원 검사는 기존 준비표와 함께 독립 검토한다. 실제 Assets 적용/Csc/Unity/추가예산0을 유지한다.
#### §8 v32 반환의 기존 G5 Play7 실행 결속 — Approved

Astra는 v32 manifest 43026바이트/SHA256 185F329C6863E17273CAEBA6CC7FDB5848CB9E8C3DB2BEA253760DF251CA661C 및 Root 선택 `g5-play7-root-selection.v1.json` SHA256 4744E5432AF12B6E46DFF3620D3C9DFE6DF01370718E222BF5B49328C4DC720E를 직접 대조했다. Root의131핀 일치·source630 확인과 Sol의 정확6파일 반환/쓰기폐쇄·설치참조 Csc6 종료0/회수, Luna 독립 검수의 잔여 P0/P1 없음에 결속한다. 추출 지원33·제동12는 합성 국소 검사이며 실제 컴포넌트 통과가 아니다.

PassBarrier의 Closing 분기 상한 후보는 원문 제어흐름으로 해소한다. ordinal N 진입 전 말미 WaitForFixedUpdate는 N회이고 Closing 분기는1회 뒤 즉시 종료하므로 N+1≤1200이다. 같은 순회 말미와 중복하지 않는다. 이후 렌더 양보를 포함한 실제 엔진 물리 진행 총량까지 정확1200이라고 확대하지 않으며 원래 입력/반복 상한 보존에 한정한다.

기존 미소비 G5 PlayMode 정상 단계1회/1200초를 현재 v32에서 실행하도록 결속한다. 신규 예산이나 과거 Edit37 소비 복원은 아니다. 정확 선택은 위 Root JSON의 Camera1·Movement3·Transfer3 총7클래스 전부이고, 신규 CompletedSupportUsesOriginalGroundOrDownwardHitAndRejectsForeignOrExpiredReceipt를 포함해 발견된 모든 사례의 결과를 보존한다. XML 유일 classname 집합 정확7·누락/추가/건너뜀/실패를 검증하며 사례 수를7로 오기하지 않는다. 기존 run-focused-owned.v2의 공개 실행/단조기한/정확 PID·시작 신원/회수/로그·XML 폐쇄/source630 전후 대조를 유지한다.

Root만 실행하며 현재 Editor0·쓰기 소유 인수 후 실행 중 소스/자산 쓰기0이다. 준비 전용 G5 후보 작성과 읽기 설계는 병행할 수 있다. 실패 시 원시 원인과 영향부터 판정하고 무변경 전체 반복0이다. 이 실행은 G5 Edit37의71실패나 남은 G4를 해결했다고 인정하지 않는다. RunUnity의 지지 경계2 및 양분기 종결2는 별도 정확 선택/유한 실행을 후속 결속하며 이번 승인에 섞어 실행하지 않는다. 최종 CH1/Windows 수용은 계속 미완료다.
#### §8 G5 Play7 폐쇄와 지지 기하의 정확 비교 보완 — Approved

Root의 g5-play7-r1은 PID42696·종료0·실제32.1233153초(실행 조정32.4163431초), 정확7클래스/78사례 전부 통과·실패0·건너뜀0·클래스차이0·source630불변·Editor0·회수 완료다. 로그A85682EC683CFD887318A75876BD88D952D8CA714A6EC5C63D3E8ADA545BA446 및 폐쇄E4B1D9C82D2DF72E98FBBC64B03D1F6C2521980881B9874692184E1369D53D8A를 결속한다. 신규 CompletedSupportUsesOriginalGroundOrDownwardHitAndRejectsForeignOrExpiredReceipt도 실제 통과했다. 기존 Play 정상1회는 소비 완료다. 이후 소스 변경의 검증을 이 결과로 자동 대체하지 않는다.

Luna 보완 검토와 Astra 원문 대조에서 SupportGeometry.Matches 및 PlayerSupportGeometry.Matches의 Vector2/Vector3/Quaternion/Bounds 연산자 비교가 근사 비교임을 확인했다. REQ/AC-AUTOCH1-SUPPORT-001의 관측 이후 기하 변경 거절을 정확히 구현하기 위해 Root가 폐쇄 후 반환하면 Sol은 기존 PlayerMovementController.cs 및 PlayerMovementControllerPlayModeTests.cs 두 파일만 보정할 수 있다. 지지 증거의 원본 size/offset/position/scale/rotation 및 bounds center/extents를 구성 float 값의 정확 동등성으로 대조한다. 유한값 검사를 유지하고 원래 운동/물리 질의/순서·횟수/입력·기하 값은 바꾸지 않는다. 같은 증거 경로에서 회전0 지원 여부를 근사 quaternion 비교로 판별하는 곳도 정확한 축정렬 조건으로 맞추되 지원 형상/회전 허용범위를 넓히지 않는다.

기존 새 Movement 경계 메서드 안에 Unity 근사 비교 범위보다 작지만 표현 가능한 실제 shape/transform 구성값 변경의 음성을 보완한다. 정확 비교 대상별 국소 추출 검사와 실제 컴포넌트의 대표 미세변경 거절을 구분한다. 새 파일/시험 클래스/공개 API/증거 직접설정0이다. 완료 위치는 기존 motor 완료 XY 권위를 유지하며 정상 MovePosition 지연과 충돌할 수 있는 현재 Transform 위치 대조를 추가하지 않는다. 기존 지원 밖/비유한 거절과 최초 원문 오류 보존은 유지한다.

설치참조 Csc·최소 국소 검사·반환핀과 Luna 해당 diff 확인 후 후속은 기존 Movement 경계1+RunUnity 지지 정상/음성2+종결 양분기2의 정확5를 우선 후보로 한다. 전체 Play7 무변경 반복0이며 반환핀/정확 선택 결속 전 새 Unity 실행권위0이다. G5 준비 r3는 준비 전용 권위로 계속 작성할 수 있으나 실제 적용은 여전히0이다.
#### §8 v33 지지·봉쇄선 정확5의 실제 후속 — Approved

Astra는 v33 manifest43844바이트/SHA256 412DA3EC2690F1DA9DDEB8A035852F1A1AC13DF7587017FC7B132E06D7A25039와 실제 Movement 정확 비교 구현 및 Root의 `support-barrier-root-selection.v1.json` SHA256 0FAA6D92B6063FB1B2276CD9D187AEC2A3468895467924AD5A2EADEB4099965F를 직접 대조했다. Root131핀 전수일치와 Sol의 정확2변경·Csc21812/21660 종료0/회수·쓰기폐쇄를 결속한다. 추출19 합성과 추가 미세변경8음성의 구현 완료는 실제 컴포넌트 재실행 통과를 뜻하지 않는다.

REQ/AC-AUTOCH1-SUPPORT-001 및 BARRIERINPUT-001의 새 실제 관측으로 위 선택표의 정확5사례를 단일 PlayMode 실행1회/1200초에 한해 허용한다. Movement의 기존 CompletedSupportUsesOriginalGroundOrDownwardHitAndRejectsForeignOrExpiredReceipt1, Chapter1ChoiceBarrierPlayModeTests의 RejectStaleForeignOrExpiredCompletedSupportBeforeGateOpen 및 AuthoredSolidarityOneValidSkillPasses2, Chapter1ClosingPersistencePlayModeTests의 BothBranchesCloseOnlyAfterRealPassageAndSave(1)/(2)2다. 완전 수식 이름은 선택표 원문과 동일하게 고정하고 다른 namespace나 클래스전체로 확대하지 않는다.

실행 선행은 Luna의 v33 변경분 독립 검수에서 미해결 실행차단 결함 없음 확인, Root 현재131핀/Editor0/소스630 확인과 실행 소유 인수다. 조건 충족 전 실행0이며 별도 사용자 확인은 요구하지 않는다. Root는 기존 공개 실행기에서 정확 XML 사례집합5·각 결과/건너뜀·PID/시작 신원·단조1200초·종료/회수/로그폐쇄·source630 전후를 보존한다. 실행 중 쓰기0·상한 연장0·자동재시도0·R02 진단 조기종료 인자0이다.

이 후속은 기존 Play7을 반복하는 예산이 아니며 정상지지/잘못된지지 거절과 양분기 실제 Q→봉쇄선→종결·저장 본문을 관측한다. 기존 실패 및 v32 Play7 통과는 그대로 남긴다. 실패 시 실제 단계/원시를 먼저 분리하며 동일 큰 검사 반복0이다. G5 r3 실제 적용은 이번 종료·회수·전후대조 이후 별도 소유 전환까지0, G4 잔여와 G5 Edit 실패·최종 Testplayer/Windows 검증은 여전히 필수다.Luna v33 변경분 독립 검수는 잔여 P0/P1 없음으로 반환됐다. 정확 구성값 비교와 기존 경계8음성·정확2파일 범위를 확인했으며 추출19는 합성으로 구분했다. 기존 v32 Play7의 나머지77은 원래 운동/cast/Commit/MovePosition/입력 불변과 선택적 지지증거의 비교 조건만 변경된 영향에 따라 재사용 가능하다는 독립 판정을 수용한다. v32 지지 경계1의 과거 통과는 새8음성을 포함하지 않으므로 이번 정확5에서 재관측해야 한다. 이를 새78통과 또는 전체G5/CH1 수용으로 표시하지 않는다.
#### §8 v33 정확5 폐쇄와 남은 시험 경계

Root의 support-barrier-focused-r1은 PID37456·종료2·179.2023012초(실행 조정179.4241132초), 정확5 중1통과/4실패/건너뜀0·선택차이0·source630불변·Editor0·회수 완료다. 로그F4AE067C0A34DD205455DA3256D7EFB11101B1544AD9FD616C271855112CB0D0 및 폐쇄2E7E790E42C5A4C417C1BF46F4BB8A8EDD7C84565140C83C049725803ABDF088를 결속하며 해당 단회 예산은 소비 완료다.

실제 새 Barrier 음성1은 통과했다. Solidarity 정상과 Closing 양분기3은 passage 존재 및 PreparingClosing 전환 뒤 PassBarrier의 원본 router 동일성 assertion에서 실패했다. Astra가 생산 Flow를 직접 대조한 정상 경로는 AtBarrier→EndingSuccessfulRun→PreparingClosing→Closing이며 원본 graph 폐쇄/파기를 포함한다. 실제 통과 뒤 정상 종료 대기에서 이전 graph를 요구한 시험 경계로 분리하며 종결 완료 성공으로 선취하지 않는다. AtBarrier 원본 검증과 낙하 재생성 거절은 유지하고, 정확 정상 종료2phase에서 기존1200 안 중립 대기하는 최소 시험 보정 설계를 진행한다.

Movement 경계는 기존 platform.size에1e-6을 더한 요청 뒤 지지 거절을 기대한 새563행에서 실패했다. 요청값과 실제 native getter 값의 동일성은 기록되지 않아 실제 기하가 변경됐는지 미관측이다. 현재 정확 비교 생산 구현을 완화하지 않는다. 다음 fixture는 요청/실제/복원 값을 구성 float 정확비교로 구분해야 하며, 미반영 요청을 실제변경 음성 통과로 쓰지 않는다. 원래 실제 변경 거절의 기대를 뒤집거나 조건부 무검사를 성공으로 채우지 않는다. 정확 원인/유한 후보 설계 반환 전 새 쓰기·Unity 예산0이며 같은5 무변경 반복0이다.
#### §8 정상 종료 전이와 실제 기하 변경 fixture 보정 — Approved

REQ/AC-AUTOCH1-BARRIERINPUT-001 및 SUPPORT-001의 확인된 실패만 보정한다. Root 폐쇄/회수/source630 확인 및 명시 소유 반환 뒤 Sol은 기존 Chapter1ExpeditionPlayModeTests.cs와 PlayerMovementControllerPlayModeTests.cs 두 시험 파일만 변경할 수 있다. 생산 파일/API/물리설정/원래 성공 assertion 변경0이다.

PassBarrier의 기존 for1200과 Closing 관측 분기는 유지한다. 정확 EndingSuccessfulRun 또는 PreparingClosing이면 키/버튼을 해제하고 기존 순회 말미 WaitForFixedUpdate 한 번으로 다음 순회를 기다린다. 이 두 정상 전이에서만 폐쇄된 이전 router/graph 동일성 검사를 적용하지 않는다. AtBarrier의 원본 router/세대·완료view/출구 제어는 그대로, Diagnostic과 PreparingBarrier 등 낙하 재생성의 기존 실패는 유지한다. 추가 yield/기한 연장/직접 Q/전이·영수증 주입0이며 실제 Closing과 후속 종결/저장 assertion까지 도달해야 통과한다.

Movement 새 경계 메서드의 미세변경8항목은 고정 증분1e-6,4e-6,8e-6,2e-5 최대4개만 시도할 수 있다. 각 시도의 기준 getter·요청·실제 getter·복원 getter를 구성 float의 R 형식과 정확 비교로 구분한다. 실제 getter가 기준과 같으면 SetterUnchanged로 기록하며 변경 거절 성공에 합산하지 않는다. 실제 구성이 달라진 첫 후보에는 기존 TryReadCompletedSupport==false를 반드시 검사한다. 모든 후보가 무변경이면 명시 실패이며 조건부 무검사 성공0이다. 요청 크기가 아니라 실제 getter 차이에 따라 Unity 근사 범위 안의 ActualTinyRejected와 밖의 ActualLargerRejected를 분리한다. 실제 tiny가 native setter에서 재현되지 않으면 합성 정확비교19만 해당 작은 입력의 비교 함수 증거로 남기고 실제 tiny 성공을 주장하지 않는다.

매 시도 finally에서 원본 값을 복원하고 getter의 구성값 정확일치를 확인한다. 원래 검사 예외가 있으면 복원 오류를 별도 기록하되 원래 원인을 덮지 않으며, 정상 경로의 복원 실패는 반드시 실패다. 정확 baseline이 회복되지 않으면 다음 후보로 진행하지 않는다. 기록은 최대32시도행+요약1행, 각 최대1900바이트·총64KiB 이내로 종료 요약 공간을 예약한다. 값 직접변경은 기존 격리 경계 fixture에만 한정하며 정상원정 입력 정책에 전파하지 않는다.

원문 역대조·설치참조 Csc·국소1묶음과 원인 보정 최대2묶음을 기존 범위에서 수행하고 정확 반환핀/기록 상한/실제와 합성 구분을 보고한다. Root의 실행 소유 인수와 Luna 변경분 검토 뒤 후속 최소 선택/단회 예산을 결속한다. 이번 절은 구현 승인이고 새 Unity 실행 예산0이다. v33 Barrier 음성 실제통과와 기존 원시 실패는 보존하며 무변경5 반복0이다.같은 fixture 보정의 고정 후보 조정 — Approved: 마지막 네 번째 증분2e-5를1f로 바꾼다. 앞선 세 미세 후보1e-6/4e-6/8e-6과 최대4회·실제 getter 변경 필수·정확 복원·기존 거절 assertion은 그대로다. 원시 실패563행에 도달하기 전 platform.size의1f 실제변경 거절은 이미 통과했으므로 해당 형상의 유효한 큰 변경 대조군으로 사용한다. 다른 속성까지 과거에 관측됐다고 확대하지 않으며 각 getter의 실제 변화와 복원을 새 실행에서 확인한다. 마지막 후보는 ActualLargerRejected로 구분하고 미세3개가 모두 native setter 무변경이면 실제 tiny 거절은 미관측으로 남긴다. 합성 정확비교와 큰 실제변경을 실제 미세변경 성공으로 합치지 않는다. 무한 탐색/후보 추가/생산 변경/새 실행예산0이다.
#### §8 v34 정상 전환·실제 getter 보정의 정확4 — Approved

Astra는 v34 manifest44687바이트/SHA256 4B4137D7C1D6EACD420CCCC4A791A6B5941E91425A11832F666BB102BD9E79F9, 실제 시험2파일의 정상 전이 분기와 고정4후보/정확 getter·원본proof 복원 코드를 직접 대조했다. Root131핀 일치, Sol의 생산0·정확시험2변경·Csc45384/7428 종료0/회수·쓰기폐쇄에 결속한다. 추출모형8/phase정적6은 native 실행 성공이 아니다.

새 관측 목적은 정상 종료2phase에서 기존 graph를 요구하지 않고 실제 Closing/저장까지 진행하는지, 그리고 각 native setter의 실제변경/미반영과 원본 지지거절/복원을 구분하는 것이다. 정확 선택 `support-barrier-root-selection.v2.json` SHA256 4EE3A9F8B7F0C9E260007A2E43ADF649151BEB25EC6C0D69BD3E9D6463931955의 Movement 경계1·AuthoredSolidarityOneValidSkillPasses1·BothBranchesCloseOnlyAfterRealPassageAndSave(1)/(2)2, 총4만 PlayMode1회/1200초로 허용한다. 기존5의 무변경 재실행이나 소비 복원이 아니다.

실행 선행은 Luna v34 변경분 검수에서 미해결 실행차단 결함 없음 및 v33 RejectStaleForeignOrExpiredCompletedSupportBeforeGateOpen 통과의 영향 재사용 가능 확인이다. 해당 음성 시험과 생산 원문은 불변이고 PassBarrier 정상전이 보정 경로와 분리돼 있음을 확인해야 한다. 선행 확인 전 실행0이며 Root만 현재핀/Editor0/source630·정확 XML집합4·PID/시작 신원·단조기한·종료/회수·원문폐쇄/전후대조를 소유한다. 실행 중 소스/자산 쓰기0·기한 연장/자동재시도/R02 조기종료 인자0이다.

원시 기하 기록의 SetterUnchanged/ActualTinyRejected/ActualLargerRejected와 baseline·복원 proof를 각각 보존하며 합성 정확비교19와 섞지 않는다. 성공은 실제 원래 assertion이 통과한 범위만 수용한다. G4 잔여·G5 Edit 실패·최종 Testplayer/Windows 관문은 계속 미완료이며 G5 r3 적용은 이번 실행 폐쇄 뒤 소유 전환까지0이다.Luna v34 독립 검수의 잔여 P0/P1 없음과 v33 Barrier 음성1 재사용 가능 판정을 수용한다. 정확시험2 변경·생산 불변·정상2phase 이외 경로/1200 보존·baseline/복원proof 및 기록상한을 확인했으며 실제 native getter/종결은 이번 실행에서 관측해야 한다.

#### §8 G4 보스 동시 사망의 정상 입력 후보 — Approved 준비 전용

REQ/AC-AUTOCH1-BOSSCOINCIDENT-PREP-001: 기존 PlayUntilBossAndPlayerDeath의1800은 실시간 초이며1800완료틱이 아니다. 정상 입력만으로 실제 boss/player 같은틱 사망과 원래 Flow 실패 우선/단회저장복귀를 관측하는 후보를 준비한다. 지정 Sol은 preparation/autonomous-ch1-20261021 아래 고유 후보 폴더에 기존 Chapter1ExpeditionPlayModeTests.cs 한 파일의 정확 helper 후보/diff·before핀·관측/영향 계획만 작성할 수 있다. 실제 Assets 적용·컴파일·Unity0이며 현재 봉쇄선 실행과 G5 적용 소유에 간섭하지 않는다. R05 health/crush 원인은 이 후보에 섞지 않는다.

후보는 실제 AtBoss 최초완료tick부터6000완료틱(재설정0), 기존1800초와 Root 실행상한 중 먼저 소진하는 유한 경계다. 실제 BossEntry/carryHP1..5·원본router/graph/gen/run·Combat/Hostile/Bridge와 완료receipt를 읽어 결속하며 초기HP5를 가정하지 않는다. 정상 이동/점프로 안전 준비 지점 x8.5~9에 접근하고 실제 속도/위치로 보정하는 경로는 아직 미검증으로 표시한다. 이동·피해·무적·HP/공격값을 직접 대입하지 않는다.

boss60→3 준비는 입력19회가 아니라 실제 유일 기본공격 요청의 FiredHit·AppliedAmount3·HP감소3 총19건으로 증명한다. 다른 보스피해/예상 밖 사망·원본변경은 실패다. 각 공격 뒤 버튼을 해제하고 실제 cooldown과 aim을 확인한다. 이후 공격/전이 없이 정상 이동으로 x5.2 부근에서 실제 적 피해를 받아 playerHP1을 준비한다. 사망을 재시도로 지우거나 임의 부활/피해 요청을 만들지 않는다.

완료t에서 실제 Hostile.Digest Queued/DeliveryTick=t+1/RequestId와 Combat.ReadPendingDeliveryForTests(t+1)의 source/delivery/Hostile1·payload없음, player무적종료≤t+1·공격쿨다운≤t+1·버튼해제·현재원본을 대조한 때만 새 정상 기본공격 edge1을 발행한다. 실제 입력 commit과 두 피해가 같은 Combat.Process(t+1)에 합쳐졌는지 후속 원시로 확인한다. 예상값만으로 예약을 합성하지 않는다. 최종 두 Applied·boss/player0·동일tick 실제 deathPublication·Flow 실패 우선 및 기존 WaitReturned의 단회 종료/저장/복귀 assertion만 수용 권위다.

준비안에는 실제 입력 지연/조준·안전왕복/HP준비의 미검증 위험, 원본 assertion 보존 대조, 제한된 전이 기록과 정확 실행 선택을 제시한다. 준비 완료는 구현 승인이나 동시사망 성공이 아니다. 별도 승인·소유 전환 전에 실제 시험파일 쓰기/새 생산입구/Unity 예산0이다.
#### §8 v34 정확4 폐쇄와 종결 화면 관측 경계

Root의 support-barrier-focused-r2는 PID6188·종료2·137.7477194초(실행 조정137.9808498초), 정확4 중2통과/2실패/건너뜀0·선택차이0·source630불변·Editor0·회수 완료다. 로그95B5D3DA5C40169E5EDD15F35BDDA5FB63FC919CADB02EE14E19711066CEEBFA 및 폐쇄F1F090D7F94C444BD41ABB97F11771C5D6505665F556F96FCD15608305F005A7를 결속한다. 해당 단회 예산은 소비 완료다.

Movement 경계와 Solidarity 정상은 실제 통과했다. TinyGeometry 실제20시도는 SetterUnchanged12·ActualTinyRejected4·ActualLargerRejected4, 누락0/기록실패0이며 기준proof·정확getter복원·복원proof True를 확인했다. 일부 size/offset의 tiny요청이 실제 getter에 반영되지 않은 것과 실제 작은변경 거절은 별도다. 추출19 합성으로 native 미관측을 채우지 않는다. 이전563행 실패의 요청/실제변경 혼동을 이 범위에서 해소한다.

Closing 양분기2는 실제 정상 통과·종결 첫 대사까지 도달했지만 FinishClosing의 다음 대사 기대에서 이전 첫 대사가 남아 실패했다. 최종대사/완료영수증은 아직 미수용이다. Astra 직접 읽기에서 UI tick은 InputRouter.FixedUpdate의 CommitHubUiOnlyFrame마다 증가하고, ClosingPresenter는 FixedUpdate에서 입력소비·대사 진행, LateUpdate에서 표시 문구를 갱신한다. 따라서 UI시계가 Dynamic 기준이라고 단정하지 않으며 실제 tick/ordinal/중립·입력edge/표시 갱신을 구분해 최소 시험 관측 설계를 진행한다. 생산 최소60틱·중립·원본소비 조건은 완화하지 않는다. 같은4 무변경 반복0·새 쓰기/Unity예산0이다.
#### §8 종결 표시 양보와 G5 실행 의미 보정의 단독쓰기 묶음 — Approved

REQ/AC-AUTOCH1-CLOSINGVIEW-001 및 G5BRIDGE-001: v34 실행 폐쇄 뒤 정상 종결 시험의 표시 시차와 G5 EditMode wrapper 실행 의미를 한 Sol 단독쓰기 묶음으로 보정한다. 정확 범위는 기존 Expedition 시험1과 G5 r3 후보17의 중복 없는 시험18파일이다. 생산/meta/asmdef/새 파일/현재 빌드설정의 영구변경0이다. 실제 적용 전 Root가 Editor0·폐쇄·소유 반환을 확인하고 지정한 한 Sol만 쓰며 다른 담당은 읽기만 수행한다.

종결 보정은 Chapter1ExpeditionPlayModeTests.cs의 FinishClosing에서 정상 `yield return ClosingEdge(line%2==1)` 직후 렌더 양보 `yield return null` 한 번을 추가하는 것으로 한정한다. 공용 ClosingEdge 및 keepHeld/위조음성 경로는 그대로다. 기존61 고정대기·생산60 UI tick·중립/원본영수증/입력값·문구/최종완료 assertion을 보존한다. 이 양보는 실제 LateUpdate 표시 이후 원래 다음문구를 읽기 위한 것이며 대사 인덱스를 직접 바꾸거나 표시문구를 성공권위로 대체하지 않는다. 추가120틱 재시도/생산시계 변경은 승인하지 않는다.

G5 적용 기준은 preparation/autonomous-ch1-20261021/g5-bridge-prep-sol61-r3/manifest.json 21265바이트/SHA256 E14C7988BFA5D558B5C46ACBE36917E7A0AEEA0664C9773244CF12977584E329, candidate.diff 83715바이트/SHA256 AED04C6C0A7382EFBE95AE6C05BF6BAB3E0EDE6FA12CF393639E7AF5A94EAF01이다. Astra는 현재17 before핀 전수 불일치0과 계획/핵심 helper를 직접 대조했고 Luna 독립 준비검수의 미해결 P0/P1 없음 및 실제 미검증 경계를 확인했다. 적용 직전17 before핀을 다시 확인하고 다르면 덮어쓰지 않는다. 정확 candidate17만 적용한다.

앞 G5BRIDGE-PREP-001의 사례별 Enter/Exit, 실제 Unity coroutine·원문 IEnumerator37/저작2 총39본문 보존, 동기시험/속성/예상값 불변, 최초 원문예외·역순 단회정리·소유host·NUnit/LogAssert 문맥 조건을 구현 권위로 승격한다. 정상160/170초/정리10초 및 설치180초 기본을 유지한다. 경계 fixture의 probe는0.25..2초/실제timeout1건이며 정상 정책을 바꾸지 않는다. 예상 Failed 로그1은 정상 통과7과 별도 실행/집계하며 Assert.Fail 포착 사례를 NUnit 실패상태 자체의 증거라고 확대하지 않는다.

저작2메서드는 실제 시험 중에만 원래 BuildSettings path/enabled/order를 보존하고 LocalSlice 단독 입력으로 원래 본문을 실행한 뒤 finally 복원한다. 실제 실행에서 Root가 EditorBuildSettings.asset 전후 바이트도 대조해야 하며 차이가 있으면 보존·실패/별도판정한다. 이번 적용 시 ProjectSettings 파일은 쓰지 않는다.

국소1묶음과 원인 보정 최대2묶음에서 정확18변경·G5 before17/후보핀·기존39본문/동기본문·속성/지원yield·종결1행 이외 불변을 대조하고 설치참조 Csc를 수행한다. 구현 중 컴파일 원인에 따른 같은 경계의 작은 보정은 근거/diff를 남긴다. 구현자가 독립 수용하지 않으며 반환핀·Luna 최종변경분 검수 뒤 Root가 실행을 소유한다.

새 Unity 실행예산은 이번 절에서0이다. 이후 정확 선택은 PlayMode 종결 양분기2, EditMode G5 정상 최소(기존 bootstrap0/1·handoff/terminal·연속사례/Single·저작2와 경계7), 별도 예상 Failed 로그1로 나누어 반환핀 뒤 결속한다. 고정44 필수집합과 기존 실패71은 보존하며 큰37 무변경 반복0이다. G4 보스 준비 후보와 R05 정책의 실제 적용은 이18파일 묶음에 포함하지 않는다.
#### §8 v35 종결2·G5 정상16·예상실패1의 분리 실행 — Approved

Astra는 v35 manifest45450바이트/132핀/SHA256 BE92CFDF964BCFA4F5A9BB9C4A0C261BFCB66C2186BBCA630BA1E31B98E41DC4, 정상 종결1행 및 Root 실제 선택을 직접 확인했다. Sol의 정확시험18변경·생산0·r3후보17 일치·기존39본문/동기124/속성 보존·Csc5 종료0/회수·쓰기폐쇄, Root132핀 불일치0에 결속한다. Luna 구현 검수의 미해결 구현 P0/P1 없음과 선택14/16 차이를 구분한다. 구현자 execution-selection.v35.json의14는 미실행 준비안으로 보존하고 실제 실행 권위는 아래 Root 정상16에만 부여한다.

실제 분리 선택은 다음과 같으며 각 신규경로/단회1회/단조1200초다.

- PlayMode `closing-render-root-selection.v1.json`, SHA256 D30279660579BD7451D89F44C70F5B54A8D1736A3E082D83EC22CAF18FE03DA4: BothBranchesCloseOnlyAfterRealPassageAndSave(1)/(2) 정확2, ownedBatchFixture=false.
- EditMode `g5-bridge-normal-root-selection.v1.json`, SHA256 A8CB4779699A83C5CCDC34997087CADDFEC5F9D2113E0B80F7C7FB6842FDDD14: 경계 정상7·Entry연속2·동기Combat연속2·handoff1·terminal1·Single1·저작2의 정확16, ownedBatchFixture=true. 모든 사례 이름/문자열 인자는 선택표 그대로다.
- EditMode `g5-bridge-negative-root-selection.v1.json`, SHA256 968C7EA29E3B8627E1B8E26836980B423F05E35012983541F5DE3151B7733352: UnexpectedLogRemainsRunnerFailure 정확1, ownedBatchFixture=true. XML Failed와 오류 CH1_BRIDGE_UNEXPECTED_LOG_PROBE가 실제 남는 것이 기대이며 원시 실패1로 보존한다. Passed/다른실패/미발견/건너뜀은 이 탐침의 관측 실패다. 정상 통과 수에 합치지 않는다.

현재 실행 조정 run-focused-owned.v3.ps1 8334바이트/SHA256 1C4DB56F5CD483C78682AC1CCEBD8C4F4E14053BCA0918ED688E7D5AB8948B57는 기존 v2를 보존한 별도 사본이며 정규식 필터 안 문자열 따옴표만 \x22로 인코딩해 Windows 인자 파서가 없애지 않게 한다. 사례 이름/예상 XML 집합은 그대로다. 해당 변경의 AST/정규식 일치·따옴표 유출0 및 Luna 조정 변경분 검토의 미해결 실행차단 결함 없음 확인을 실제 실행 선행으로 한다. 이 조정은 새 시험/API/임의 필터축소 권위가 아니다.

Root만 현재132핀/Editor0/source630·명령/출력·정확 XML 집합·각 결과/오류·PID/시작 신원·기한·종료/회수/로그폐쇄·전후대조를 소유한다. 저작 실행에는 EditorBuildSettings.asset 원문 전후 대조를 포함한다. 세 실행은 동시 수행하지 않으며 실행 중 제품/시험/자산 쓰기0, 자동재시도/기한 연장0이다. 서로 독립인 선택의 원시 결과를 분리하고 공통 컴파일/환경 차단이 발견되면 후속 무변경 실행을 강행하지 않는다.

기존 Edit37/Play7 및 봉쇄선 실패·소비는 그대로다. 이번은 변경 원인의 최소관측이며 고정44 전체수용/과거71실패 해소를 선취하지 않는다. 정상16/예상실패1의 실행 의미가 확인된 뒤 고정44의 영향 재사용과 필수 재관측을 따로 판정한다. G4 보스/R05·최종 Testplayer18/Windows/독립 검수는 여전히 필수다.
#### §8 v35 종결 렌더 후 실패 폐쇄·첫 입력 관측 — Approved 관측 구현

Root의 closing-render-r1은 PID30748·종료2·97.3831047초(실행 조정97.6653754초), 정확2 모두 첫 대사 유지 실패·건너뜀0·선택차이0·source630불변·Editor0·회수 완료다. 로그7E770659314D1E7082947349A5992C15B23FEF154FC64C661DA575A6CB3A786E 및 폐쇄5C9770DE44B4B185F166880F0E3DF7F1F7507F18B7AA9D0E1EE4980795A9E3BD를 결속한다. 해당 단회는 소비 완료이며 렌더 양보만으로 원인이 해결됐다는 가설은 기각한다. 공통컴파일/환경차단이 없으므로 기승인 G5 정상16/별도음성1은 현재 소스 동결로 독립 진행할 수 있다.

REQ/AC-AUTOCH1-CLOSINGDIAG-001: G5 실행 종료·회수·전후대조와 Root의 명시 소유 반환 뒤 지정 Sol은 기존 Chapter1ExpeditionPlayModeTests.cs 한 파일에 첫 종결줄/첫 입력의 읽기 기록만 추가할 수 있다. FinishClosing 진입·61고정대기 전후, ClosingEdge 중립대기 뒤/첫 Enter 발행 직후/첫fixed뒤/해제후fixed뒤, 기존 render 양보 뒤 및 실패finally 요약을 기록한다. 첫줄에만 적용하고 반복5줄 기록이나 원정 전구간 확대0이다.

원본 source/router/ClosingHostEntry 동일성·OwnsIssuedClosingHost·UiPhase, 현재 receipt와 UI frame의 tick/ordinal/mode·Submit/Click/Cancel, ClosingControlsNeutralReady 및 기존 TryReadChapter1ClosingControlsNeutral의 관측가능 여부/값, 현재 시험 장치 Enter/마우스 held·InputState.updateCount, 기존 scaler.IsSupported/IntegerScale·실제 Status를 읽는다. getter가 초기화하는 새 경로·반사/생산 API·사적 index/소비ordinal 추정0이다. 노출되지 않은 adapter 실제소비와 내부대사index는 미관측이라고 명시하고 frame 존재를 소비 성공으로 대체하지 않는다.

사례당 최대16행/총32KiB이며 각1800바이트 이내로 finally 요약1행 공간을 예약한다. 읽기실패/누락을 표시하고 원래 시험예외를 덮지 않는다. 원래 입력·yield/ordinal·61대기·60 UI tick/중립·문구/영수증 assertion·시간상한은 모두 그대로다. 기록 자체를 원인 보정이나 성공으로 삼지 않는다.

정확원문 역대조·설치Csc·기록 상한 국소 검사와 Luna 영향 확인 뒤 원인 관측은 BothBranchesCloseOnlyAfterRealPassageAndSave(1) 한 사례만 우선 후보로 한다. 현재 절은 관측 구현 승인이고 반환핀/선택 결속 전 새 Unity 예산0이다. 같은양분기2 무변경 반복0이며 G5 코드/보스후보 적용과 단독쓰기 충돌을 만들지 않는다.
#### §8 v35 G5 실행 의미의 실제 최소검사 폐쇄

Root의 g5-bridge-focused-r1 정상16은 PID41804·종료0·48.0857859초(실행 조정48.3736133초), 정확16 전부 통과·실패0·건너뜀0·집합차이0·source630불변·Editor0·회수 완료다. 로그712F2F278E816A6A4ED5ACB6876F73C2B9A7BA0EEBC07747C63BCD6FDE997923 및 폐쇄80CD6FADCDA9CF6A47D9F3B66080D70B498CA73D195DB21B6C415A2904159024를 결속한다. 문자열 매개변수3도 정확 발견됐으며 중첩예외/단회정리·실제timeout·예상로그 문맥·사례별재생·기존0/1/handoff/terminal/Single·저작2와 설정복원 검사가 통과했다.

별도 g5-bridge-negative-r1은 PID9280·종료2·10.4822285초(실행 조정10.7050159초), 정확1 XML Failed·건너뜀0·집합차이0·source630불변·Editor0·회수 완료다. 원문 오류는 Unhandled log message: '[Error] CH1_BRIDGE_UNEXPECTED_LOG_PROBE'이며 의도한 예상 밖 로그가 실행기 실패로 유지됐다. 로그734F9BD35D9597B694010C01DE45201F6C90890162BE2389CD727BDC724E5C61 및 폐쇄B2EF8BA9E7FB6C8F920420A127AF390A77C5DFA23C91F604A7D1F82377DA68CB를 결속한다.

각 단회는 소비 완료다. AC-AUTOCH1-G5BRIDGE-001의 해당 최소 실행 의미를 수용하며 정상16과 예상실패1을 새17통과로 합치지 않는다. 과거 고정37의71실패는 보존한다. 고정44의 영향 재사용/필수재관측은 Luna가 변경 helper/각 wrapper·본문·생산 의존을 대조한 뒤 별도 판정한다. 모든 실행 폐쇄 후 Root는 기승인 Closing 진단1파일 쓰기를 단독 반환할 수 있다.
#### §8 v36 첫 종결 입력1과 G5 영향16의 실제 관측 — Approved

Astra는 v36 manifest45935바이트/132핀/SHA256 E02762FE1AB96DAF0B37122F25C2E84B3A8E890A0BB62FF380609E0DBDEBAF4C 및 첫입력 기록 코드를 직접 확인했다. 정확 Expedition 시험1변경·전체역치환 원문 동일·입력/update/yield/61대기/assert 불변, Sol 최종Csc40316 종료0/회수·쓰기폐쇄와 Root132핀 불일치0에 결속한다. 최초Csc3820의 scaler 내부접근 거절은 보존하고 새friend/API/반사 없이 scaler 지원/scale을 미관측으로 남긴 후퇴를 수용한다. Screen 크기로 이를 추정하지 않는다. 기록16행/행1800/총32KiB·finally 예약과 소비/index 사적미관측 조건을 유지한다.

다음 두 실행은 각각 신규경로·단회1회/1200초로 허용하며 현재 v36 동결 소스에서 직렬 수행한다.

- `closing-input-root-selection.v1.json` SHA256 94928019E3DAAA22265A95FC8A6510AB6582ED6D4AA5F1CA7583F7E0C94847BC: PlayMode BothBranchesCloseOnlyAfterRealPassageAndSave(1) 정확1, ownedBatchFixture=false. 목적은 첫 UI 입력/중립/receipt/문구의 원시 관측이며 원래 실패를 정상 성공으로 바꾸지 않는다.
- `g5-affected16-root-selection.v1.json` SHA256 2E7162798194F39A470BFD0ADE9864D1329E82495C685328727336C729A616FA: EditMode 영향16클래스 전체, ownedBatchFixture=true. 기존 G5 XML에서 고정한192사례를 expectedWholeClassCaseNames로 함께 결속한다. 필터는 정확16클래스이고 최종 폐쇄는 클래스집합뿐 아니라 실제사례192의 다중집합/각결과/미발견·추가·중복·건너뜀까지 별도 원시로 대조한다. 정상 최소16을 전체192로 확대하지 않는다.

Luna v36 기록 변경분 검수의 미해결 실행차단 결함 없음이 실행 선행이다. Root만 현재핀/Editor0/source630·기존 v3 실행조정·명령/출력·PID/시작 신원·단조기한·회수/로그/XML/전후대조를 소유한다. G5에는 EditorBuildSettings.asset 바이트 복원 대조를 포함한다. 실행 중 제품/시험/자산 쓰기0·자동재시도/기한연장0이다. 공통컴파일/환경차단이 없으면 한쪽 기능실패를 다른 독립선택의 암묵적 차단으로 확대하지 않는다.

영향16은 helper/사례별수명 변경에 직접 연결돼 과거121통과도 재관측하며71실패 원문은 보존한다. 다른 Edit21의398통과는 테스트 원문동일 외 생산변경 경로/결과 영향의 독립근거가 완성된 뒤 재사용을 판정한다. Play7은 기존77+v34 경계1의 분리근거를 유지한다. 이번 실행 권위는 고정44 전체수용·CH1 종결·Windows 전달 완료를 선취하지 않는다.G5 영향 없는 Edit21 재사용 판정 — Luna 보완 근거를 수용한다. 실제 v31 실행핀과 v35핀의 해당 시험21원문은 전부 동일하고, 두 오래된 영향계획 지문은 실제 실행핀으로 대조했다. 세 생산 변경인 Movement 선택적 지지증거·ChoiceEnvironment·Lifetime receipt 전달에 대해 해당21의 ChoiceEnvironment/Lifetime/Flow/ChoiceAdapter/support reader 호출은 없다. Movement 참조2 중 OrdanBossEncounterAuthoringTests는 비활성 prefab의 assignment-only 저작 입구만 검증해 FixedUpdate/proof를 실행하지 않으며, DesktopProfileLaunchAdapterV1Tests는 BoxCollider2D 기반 UIOnly fake launch rig여서 지원 capsule 증거/reader를 사용하지 않는다. 따라서 기존 결과를 바꾸는 경로가 없다는 독립 판단에 따라 원시398/398 통과를 재사용할 수 있다. 이는 현재 실행한398건이라는 뜻이 아니며 Edit16 전사례 재관측과 Play77+v34경계1의 분리 조건은 그대로다.
#### §8 v36 첫 입력 진단 폐쇄

Root의 closing-input-r1은 PID39408·종료2·56.9846647초(실행 조정57.5911198초), 정확1실패/건너뜀0·source630불변·Editor0·회수 완료다. 로그03AA69E6473FEEC97CD95DA00562D139515C9C8AA85C9B09EE2B6A4CA571B15F 및 폐쇄4196256EE37BFE98ACAF135D589B3B649024A65413DEE26370C0BA989650D1DA를 결속한다. 진단10행·누락0·읽기실패0이며 단회 예산은 소비 완료다.

첫 Enter는 원본 UI tick2287/ordinal152/SubmitTrue로 관측됐고, 해제fixed와 render 후 실제두번째문구로 진행했다. 이번 실패는 네번째문구 기대/세번째문구 실제이며 기록범위 밖의 세번째edge 원인은 미확정이다. 첫입력이 항상 실패했다는 설명은 이 관측으로 정정한다. 해제fixed에서 실제 Enter heldTrue·physicalNeutralFalse였고 render 뒤 heldFalse지만 receipt2288/physicalNeutralFalse가 남아, 장치해제와 원본중립publication의 시차를 확인했다. 내부adapter소비/대사index/scaler 값은 여전히 미관측이다.

다음 최소설계는 정상 FinishClosing의 매줄에서 실제현재 UI tick 기준60 경과와 같은 원본source/router/entry·실제held해제/Ready/physicalNeutral을 확인한 뒤 새edge 단1회를 발행하고, 다음원본문구/마지막실제completion을 유한 관측하는 것이다. 생산60틱/중립·소비 조건이나 공용ClosingEdge의 keepHeld/음성 경로를 바꾸지 않는다. 정확 관측순회/완료tick 상한과 호출영향 설계 반환 및 현재 G5 실행 폐쇄 전 새 쓰기/Unity예산0이다. 같은종결사례 무변경 반복0을 유지한다.
#### §8 실제 UI 중립·완료 틱에 결속한 정상 종결 입력 — Approved

REQ/AC-AUTOCH1-CLOSINGINPUT-001: 현재 G5 실행 종료·회수·source630 대조와 Root의 명시 소유 반환 뒤 Sol은 Chapter1ExpeditionPlayModeTests.cs 한 파일의 FinishClosing 정상5줄 루프와 전용 private 관측 helper/제한logger만 보정할 수 있다. 공용 ClosingEdge 본문/다른 caller 및 Persistence 시험·생산 원문은 바꾸지 않는다. 기존59틱/held/cancel/중복 음성과 최종 원본문구/영수증 assertion은 보존한다.

각줄의 현재 실제 UI receipt tick B를 baseline으로 잡는다. 줄당 최대120 UI완료틱/180관측순회 중 먼저 소진하는 경계를 적용하고 한 순회는 render양보1+fixed양보1+읽기로 정의한다. 전체5줄 최대600완료틱/900순회 및 Root1200초 밖 추가대기0이다. baseline은 실제대사시작보다 늦을 수 있으므로 B+60까지 기다리는 보수적 정책이며 생산 displayStartTick을 읽거나 변경하지 않는다.

줄 시작에 기존 장치 키/버튼을 한 번 해제하고 같은 source/router/ClosingHostEntry·OwnsIssuedClosingHost·현재 receipt와 UI frame 동일쌍/UIOnly를 검증한다. 실제 Enter/좌우버튼 해제와 ClosingControlsNeutralReady·기존 TryReadChapter1ClosingControlsNeutral 성공/중립을 확인하고 현재tick≥B+60일 때만 기존 교대 Enter/Click 새edge1개를 발행한다. 같은 receipt 재관측은 허용하되 tick/ordinal 역행·원본교체는 실패이며 새receipt는 두 값의 양의 증가폭이 같아야 한다. render동안 실제fixed가 여러번 진행할 수 있으므로 매관측+1을 억지로 요구하지 않는다.

edge 발행 다음 관측순회부터 해제1회를 수행하고 같은 줄상한 안에서 원래 현재문구가 유지되는 대기는 허용하되, 실제 다음 원본문구 또는 마지막의 실제 ChapterCompleted+completion을 관측해야 진행한다. 이전/다음 외 예상 밖 문구·소유실패/진단·상한소진은 실패다. 동일줄 추가edge/재시도/줄건너뛰기/대사·위치·HP·시계 직접대입0이다. 중립Ready는 다음edge의 선행이며 발행직후 의도적으로 해제된 Ready를 성공소비로 가정하지 않는다. 실제 최종완료와 기존 save/branch/generation/ordinal assertion을 유지한다.

기록은 줄당 baseline/armed/issued/advanced 또는 failure 최대4행, 전체20행+finally요약을 기본으로 최대24행/각1800바이트/총64KiB 안에 요약공간을 예약한다. 현재receipt/실제held/중립/문구와 입력발행서수를 구분하며 사적소비/index는 미관측으로 유지한다. 기록/복원 오류로 최초 시험실패를 덮지 않는다. 기존 v36 첫입력 진단 원시는 보존하고 새 기록과 혼합하지 않는다.

국소1묶음과 원인보정 최대2묶음에서 같은원본/정지·역행·기한·단일edge·현재/다음문구·첫예외/기록상한을 확인하고 설치참조 Csc·원문역대조·정확반환핀을 보고한다. 실제 UI 동작은 합성국소로 수용하지 않으며 Luna 변경분 검토 뒤 양분기2의 최소 실제선택/단회예산을 결속한다. 이번은 구현 승인이고 새 Unity 실행예산0이다. G4 보스/R05 후보를 실제적용하지 않는다.
#### §8 v36 G5 영향16 전체 관측 폐쇄와 잔여 단일 실패

Root의 G5 영향16 실행은 PID43376·종료2·662.6410925초(실행 조정663.1993059초), 정확192사례 중191통과/1실패/건너뜀0이다. 원래192사례 다중집합·중복0·클래스16집합 차이0, source630불변·Editor0·회수 및 폐쇄를 확인했다. 로그 C76CB9D4A01243292CB35EAFF558E9446BCD0A3859D7E1A1CA7DCF4EFD987549, 폐쇄 10D2FDBFA533C5896F9CB8CE5FA2CEE7105CD3241EA4FC69AB8BB8457FAE8A77, 정확192 추가폐쇄 7C7DD3ACA996E4DE713043C584818EE1F11963BE7AD0914295C074FED9C03D9D를 결속한다. 해당 단회 예산은 소비 완료다.

유일 실패는 Chapter1EntryPlayModeTests.ProducersRejectCompletedMovementAsPriorSnapshot이며 예상 밖 InvalidOperationException: Boss handoff requires exact completed current publications 원문을 보존한다. 실제191 성공을 전체192/고정44 수용으로 확대하지 않는다. 전체192 무변경 재실행0이며 이 단일 음성의 의도한 거절·후속 실제 생산자 전임·시험 정리 시점을 읽기로 분리한 뒤 최소 보정/관측을 판정한다. 현재 새 G5 쓰기·Unity 예산0이고 오류 기대 추가나 생산 검증 완화를 선취하지 않는다.

실행 폐쇄 뒤 Root는 앞서 승인한 정상 FinishClosing의 정확 시험1파일 쓰기를 주 Sol에게 반환할 수 있다. G5 원인 조사는 읽기로 병행하며 동시 source writer를 만들지 않는다. G4 과거 증거 재사용은 별도 영향·독립 판정을 마친 범위만 수용한다.
#### §8 G5 단일 prior-snapshot 음성의 무관한 handoff 격리 — Approved

REQ/AC-AUTOCH1-G5PRIOR-001: Astra는 Chapter1EntryPlayModeTests.ProducersRejectCompletedMovementAsPriorSnapshotOriginalBody 원문에서 player/transfer/boss를 첫tick20으로 구성하고 실제 hostile/pull의 첫20 publication 및 완료 movement 재소비 거절만 검사함을 확인했다. Root의 두 producer 원문 조사에서 handoff/LatestPresentationView 역참조가 없고, 시험은 handoff 소비 결과를 사용하지 않는다. 원래 첫tick0의 활성 handoff가 이 합성 음성의20 publication을 소비하면서 낸 실제 추가 오류는 시험 격리 대상으로 한정한다.

정상 종결 시험1파일의 현재 Sol 쓰기·컴파일·핀 반환이 끝나고 Root가 명시 소유를 전환한 뒤, 지정 Sol은 기존 Chapter1EntryPlayModeTests.cs의 위 정확 메서드 안에서 비활성 원본 graph의 기존 OrdanBossEncounterHandoffAdapter 참조를 확인하고 활성화 전에 해당 component.enabled=false로 둘 수 있다. 실제 타입 식별자는 원문과 대조한다. 새 객체/파일/API/friend/생산 변경0이며 다른 메서드·정상 handoff 경로는 바꾸지 않는다. 기존 첫fixed 실제20 관측, hostile/pull 비활성화 시점·AdvanceForTests의 원래 예외 및 publication 불변 assertions·정리는 그대로다. 예상 밖 오류를 LogAssert 기대에 추가하거나 guard를 완화하지 않는다.

국소 원문 대조1묶음·설치Csc와 같은 원인의 보정 최대1묶음으로 정확1파일/1메서드 변경을 반환한다. Luna 독립 변경분 검토와 Root 현재핀/정확 선택 결속 뒤 해당 단일 사례만 새 실제 관측 대상으로 삼는다. 이 절의 새 Unity 실행예산은0이며 과거191통과/단일실패와192 전체 원시를 보존한다. 전체192 반복이나 G5 전체 수용을 선취하지 않는다.
#### §8 G4 순수4 및 v24 재개·불확실 저장2의 영향 재사용

Astra의 실제 g4-c1c-r1 source-before 대조와 Luna 독립 검토에 따라 KillPlaneBoundary, LethalCrushPhaseAndOverlap, FixedPlansAndSeedRotation, RejectUnvalidatedRoom의 과거 통과4를 재사용한다. 시험 원문과 해당 Hazard/FailureSources·RoomCatalog/RoomInstance·방 계획 및 저작 prefab 의존 지문이 동일하고 변경된 선택/봉쇄선/종결 실행 경로를 소비하지 않는다. 이는 현재 새로 실행한4건이 아니다.

v24 g4-correction-focused-r1의 RestartDoesNotResumeRun 및 ExpeditionSaveUncertainRemainsDiagnostic 실제 통과2도 재사용한다. v24 실행핀과 같은 retry-spawn-before의 Expedition 원문 SHA256 90E542DD43100D2FC93058D3BAB83DE38EC4C033E8DD88729D4EE17F17F88D9C에서 이미 CloseForReopen을 호출하며 현재와 동일한 Close/Unload→null 양보→이전 router 종료 단언→소유 Keyboard/Mouse 제거를 관측했다. Luna가 이 원문으로 대조한 뒤 초기 helper 증거 공백 후보를 철회했다. 두 검사는 Hub/첫 R01의 재개 초기화·불확실 저장 진단 경로이고 이후 선택저장 재시도·봉쇄선·전송 선택·종결·지원증거 경로에 도달하지 않는다. 원래 과거 실패 행을 수정하지 않고 별도 실제 통과 증거로 결속한다.

나머지 runtime23의 재사용은 세부 공용 helper/생산 영향의 독립 보강 전 후보로 유지한다. SameTickFailurePrecedesBossTransition은 변경된 PlayToBoss 경로 영향이 있어 그대로 재사용하지 않는다. 두 정상 원정 별칭은 현재 G7_Normal101과 같은 RunNormalCase(101,0)을 공유하지만 미래 실제 성공 및 전체 assertion 동등성 판정 전 수용하지 않는다. 세 실패원인의 순차 재Open과 두 실제 동시원인, 완료 후 Persistence 재개/저장 경계의 미통과 상태는 유지한다. 새 쓰기/Unity 예산0이다.
#### §8 정상 종결 최종 표시·단일 입력 결속 보강 — Approved

v38 Entry 단독쓰기 반환·Root 소유 인수 뒤 같은 Sol에게 기존 Expedition 시험1파일의 FinishClosingLine만 다시 반환할 수 있다. Luna v37의 최종 completion 직후 LateUpdate 이전 문구 단언 가능성을 해소한다. 마지막 줄은 실제 ChapterCompleted+completion을 관측해도 그 줄 issued=true를 필수 단언하고, 기존 완료문구 두 개가 실제 Status에 반영될 때까지 기존120 UI완료틱/180관측순회 안에서만 관측한다. 완성 receipt는 원래 최종 branch/save/generation/ordinal assertion으로 검증하며, 완료 후 합법적으로 끝난 source를 새 입력 대상으로 요구하지 않는다. 완료 대기에서도 새 edge/추가해제 반복/상한 밖 yield/기한 재설정0이다.

동일 tick/ordinal receipt를 다시 읽는 경우 전체 원본 receipt 값이 직전 값과 같음을 검증하고, 정상 줄 안 modeEpoch가 바뀌면 실패시킨다. 새 tick/ordinal의 같은 양의 증가폭, UIOnly·같은 source/router/entry·중립·60틱 선행 조건을 보존한다. 이 변경은 같은 정상 helper의 성공 관측과 원본 결속 보강이며 공용 ClosingEdge/음성/Persistence/Entry/생산 변경0이다.

정확 함수 밖 원문 불변·설치Csc·국소 상한/단일edge/마지막 표시대기 검사를1묶음 수행하고 원인 보정 최대1묶음 안에서 반환한다. Entry v38과 함께 최종 핀을 결속하고 Luna 변경분 확인 뒤 Closing양분기2와 Entry정확1을 직렬 실행할 수 있도록 별도 예산을 판정한다. 현재 새 Unity 예산0이며 v37/v38 소스·실패/컴파일 원시는 보존한다.
#### §8 v39 정상 종결 양분기2·G5 prior 음성1의 분리 실제 실행 — Approved

Astra는 v39 manifest46735바이트/132핀/SHA256 036E3F14C465BAF919E7AAD536E93C12234C59AC2F98DDC333695BCC9BF39B11와 정상 helper 최종 원문을 직접 확인했다. Sol의 v37 정상입력·v38 Entry 격리·v39 마지막표시/receipt 보강의 순차 단독쓰기 폐쇄, 최종Csc26904·Entry40936 종료0/회수, Root132핀 전수일치에 결속한다. Luna 독립 검토로 마지막 issued 필수·두 실제 완료문구·동일receipt/ModeEpoch 및 원래 음성/assertion 보존을 확인했다.

120틱 표현을 다음처럼 정확히 한정한다. 정상 완료 이전의 현재 UI 관측은 줄 기준120틱을 넘으면 실패하며 마지막 사전 관측에도 이를 적용한다. 합법 완료 뒤 종료된 source/router를 다시 읽지 않으므로 최종 완료 UI tick 자체는 미관측이며120 이내라고 주장하지 않는다. 완료 뒤 실제 두 문구 반영을 포함한 전체 줄 대기는 원래180순회, 전체5줄900순회 및 Root1200초 안이다. 이 유한 제한과 실제 단일edge·completion·문구·원본영수증 검증으로 이번 관측을 수행한다. 새 API/생산시계/무제한대기/재입력 권위0이다. 앞 절의 전체600완료틱 표현을 완료 이후까지 실제 측정했다는 의미로 사용하지 않는다.

다음 두 선택을 v39에 결속해 각각 새 경로·단회1회·단조1200초로 직렬 허용한다.

- closing-normal-root-selection.v1.json, SHA256 99C4CFA32FA9D55AFC6CC67FB15C9EEA2C98C5C124DB4BF6982F9D7300754604: PlayMode BothBranchesCloseOnlyAfterRealPassageAndSave(1)/(2) 정확2, 정상 종결 실제 입력/최종문구/저장/완료 확인.
- g5-prior-root-selection.v1.json, SHA256 A8E8F03595053C922D343191438A41BD913E9D3133197C8E5ABBC1E07308A5A9: EditMode Chapter1EntryPlayModeTests.ProducersRejectCompletedMovementAsPriorSnapshot 정확1, owned fixture 사용 및 실제 첫20 publication·잘못된 prior 재소비 거절 확인.

Root만 현재핀·Editor0·source630·기존 run-focused-owned.v3·명령/출력·PID/시작신원·단조기한·정확XML집합/각결과·로그/종료/회수·전후대조를 소유한다. 실행 중 제품/시험/자산 쓰기0·자동재시도/기한연장0이다. 서로 독립인 기능 실패를 다른 선택의 암묵적 차단으로 삼지 않되 공통 컴파일/환경차단이면 원인을 먼저 보존한다. 종결 과거 실패 및 G5 191통과/1실패 원시는 그대로며 새 실제 결과 전 전체 성공을 선취하지 않는다.
#### §8 v39 정상 종결2의 실제 중립 미성립 폐쇄

Root의 정상 종결 양분기2 실행은 PID46688·종료2·99.2805731초(실행 조정99.6815242초), 정확2실패/건너뜀0·source630불변·Editor0·회수 완료다. 로그 CBF6B475088658D78FE9B251FBC0174A1ED2F0C13FE1BEF184FA404C1FC1101B 및 폐쇄 3DCFC48CEC80231003C909F819E8A7FC3006AA817C8E8B807C4BD526F5D249AF를 결속한다. 단회 예산은 소비 완료다.

두 사례 모두 line0의 issued=false에서 실제관측121틱으로 제한을 넘겨 실패했다. 기록상 baseline/실패의 Enter와 오른쪽 버튼은 held이고 physicalNeutral=false였다. 첫 정상edge를 발행하지 못했으므로 대사 소비/최종 표시의 성공 또는 실패로 확대하지 않는다. 정상 입력 이전 선행 held 음성 및 기존 장치 해제 호출과 실제 업데이트 종류별 상태 버퍼/이벤트 소비를 최소 관측해야 한다. 대기 증가·같은양분기 무변경 반복·전역 입력 설정 변경·생산중립 guard 완화0이다.

공통 컴파일/환경 차단이 없어 기승인 Entry1의 독립 직렬 실행은 유지한다. 해당 실행 폐쇄 전 소스 쓰기0이며 다음 입력 보정은 실제 원인 및 정확 시험 API·최소재현 설계 반환 뒤 판정한다. 새 쓰기/Unity 예산은 이 절에서0이다.
#### §8 장치 중립 이벤트 최소 비교3 — Approved 구현

REQ/AC-AUTOCH1-NEUTRALPROBE-001: 모든 실행 폐쇄·Root의 명시 소유 반환 뒤 지정 Sol은 기존 Chapter1ExpeditionPlayModeTests.cs 한 파일에 같은 namespace의 별도 Chapter1InputNeutralPublicationPlayModeTests : InputTestFixture와 정확 NeutralOnceWithSeparateUpdates, NeutralRepeatedWithSeparateUpdates, NeutralOnceWithCombinedUpdate 및 private ProbeNeutral helper만 추가할 수 있다. 기존 원정/정상종결/음성 본문과 생산은 변경하지 않는다. 게임 graph·프로필·Flow·새파일/asmdef/API/전역입력설정/런타임시계 직접대입·직접 장치상태 주입0이다.

기존 InputTestFixture의 SetUp/TearDown을 그대로 상속하고 자체 Keyboard/Mouse 두 개만 만든다. 공개 QueueStateEvent와 원래 개별 InputSystem.Update로 Enter+오른쪽버튼 held를 실제로 구성하고 두 실제 장치값 True를 확인한다. 비교1은 각 장치 중립 event와 개별 Update를 한 번씩, 비교2는 같은 초기 발행 뒤 각 관측순회 시작에 같은 중립 상태만 재발행, 비교3은 두 중립 event를 모두 queue한 뒤 Update 한 번을 수행한다. 각 사례 최대8순회(render1+fixed1), 실제 두 held False면 끝내고 끝까지 남으면 명시 실패한다. 재누름/추가 Enter edge·시간연장0이며 이 성공은 생산 UI 중립 소비 성공과 구분한다.

지원된 onEvent는 정확 자체 deviceId2의 time/type/deviceId/handled를 읽기만 하고, onAfterUpdate는 updateCount/currentUpdateType/실제 held 관측만 집계한다. 이벤트 관측을 실제 소비 성공이라고 부르지 않는다. private 상태/시계/반사0이다. callback 최대12행을 포함해 사례당 최대32행/각1800바이트/총64KiB 안에 요약1행을 예약하고 누락·기록실패를 표시한다.

iterator finally에서 구독 두 개를 해제하고 자체 장치2를 정리한 뒤 기존 fixture TearDown이 수행되게 한다. 모든 정리 시도는 수행하며 본문 실패를 정리/기록 오류로 덮지 않는다. 본문 성공 뒤 정리 실패는 실패로 남긴다. 국소1묶음·같은원인 보정 최대1묶음에서 원문밖 추가범위/구독·소유·상한·실패보존을 대조하고 설치Csc/최종핀을 반환한다. Luna 독립 검토와 Root 정확3 선택 결속 뒤 단회 최소관측을 판정하며 이번 절의 새 Unity예산0이다. 실패 원인을 발견하기 전 정상 종결 helper나 생산 중립 조건은 바꾸지 않는다.
#### §8 고정 G5 44클래스의 영향별 증거 통합 수용

Root의 Entry 단일 후속은 PID2040·종료0·11.4622916초(실행 조정11.9459026초), 정확1통과/건너뜀0·집합차이0·source630불변·Editor0·회수 완료다. 로그 6E1782B6AA59FFFF4B4BE334F6AC45451769E787CC3EE9E4EC919719E38B39FA 및 폐쇄 9666170A202FBD67B26DE801A1D7B821FF8BC5C9F70E86AAAAFE1CDE1B2838E1를 결속한다. 해당 단회 예산은 소비 완료다.

Luna의 최종 독립 영향 판정을 수용해 고정 Edit37/Play7, 총44클래스의668 고유 사례를 다음 분리 증거로 통합 수용한다: 영향 없는 Edit21의398 과거통과 재사용 + v36 영향16의191 실제통과 + v39의 Entry 단일 실제통과1 + Play7의77 과거통과 재사용 + v34 Movement 경계 실제통과1. v38은 정확 실패 메서드의 격리 graph handoff 두 문장만 보정해 다른191/Play77에 영향이 없고, v37/39 Expedition 정상종결 helper는 고정44 선택 밖이다. 이름/집합/중복 및 원문·생산 의존 영향의 기존 판정을 유지한다.

이는 한 번의 현재 실행에서668개가 동시에 통과했다는 주장이 아니다. 과거71실패 및 후속 단일실패 원시는 보존하고 최신 정확1 성공으로 해당 수용 상태를 갱신한다. 최소 정상16/예상로그실패1은 실행 의미 검증이며668에 중복 합산하지 않는다. 고정 G5의 충족은 G4 동시원인·정상 CH1 종결·최종 Testplayer18/Windows빌드·동일 릴리스 실제 입력·독립 검수·안내 패키지의 완료를 대신하지 않는다.
#### §8 v40 중립 이벤트 비교3 실제 관측 — Approved

v40 manifest47357바이트/132핀/SHA256 1B11AB6561C38C5BE2DC0FB0764EB11A72DE988B351BB5C8AF96645129BE6BB1, Sol 쓰기폐쇄·Csc47772 종료0/회수·기존원문 역제거 동일 및 Root132핀 일치를 결속한다. Astra가 별도fixture 원문을 직접 읽었고 Luna 변경분 독립 검토의 미해결 P0/P1 없음에 따라 아래 최소 관측을 허용한다.

neutral-probe-root-selection.v1.json SHA256 B15DFF62D568569CE2818F57BAE2A4A0380C2E6BF7783B9C5B46CCF8D5926F55의 PlayMode 정확3 사례를 신규경로·단회1회·단조1200초로 실행한다. 원래 선택은 Chapter1InputNeutralPublicationPlayModeTests의 NeutralOnceWithSeparateUpdates, NeutralRepeatedWithSeparateUpdates, NeutralOnceWithCombinedUpdate다. 장치 held 초기상태/실제해제·callback 관측·누락/정리 오류를 각군별 보존하고 결과를 종결 성공이나 실제 생산UI 소비로 확대하지 않는다.

Root만 기존 v3 조정·현재핀/Editor0/source630·정확 명령/선택·PID/시작신원·기한·XML각결과/집합·로그/종료/회수/전후대조를 소유한다. 실행 중 모든 소스·시험·자산 쓰기0·자동재실행/기한연장0이다. 결과가 원시실패여도 관측 자료로 보존하며 성공으로 고쳐 쓰지 않는다. 이번 추가fixture는 고정 G5 44 선택 밖이고 기존 시험/생산 원문 불변이므로 앞선 G5 통합 수용에 영향을 주지 않는다. 정상 Closing 보정은 실제 비교 결과 이후 별도 판정한다.
#### §8 v40 중립 비교3 폐쇄 및 실제 종결 문맥으로 조사 축소

Root 최소비교3은 PID47208·종료0·12.8570075초(조정13.3733099초), 정확3통과·source630불변·Editor0·회수 완료다. 로그 A1D863C5DD111A8438C95021F752FB4E6268FCB8021FF0DE5B63BCD3B1AA8388 및 폐쇄 963C725FAB85BED1ABC34A4C190E58F7CAFA5AB3EE3754BAD6D3ABF418B2C665를 결속한다. 단회 예산은 소비 완료다.

세 방식 모두 중립 발행 직후 실제 held 두 값이 False였고 첫관측1순회에도 유지됐다. 실제 event관측4/4/6과 시간0.03~0.11, 제한 이후 관측생략219/186/167을 구분해 보존하며 정리오류0이다. 장면 없는 단순 입력 fixture에서는 잔류가 재현되지 않았으므로 원래 개별 Update 방식 자체가 항상 해제를 실패시킨다는 가설은 지지되지 않는다. 종결 정상화 성공이나 생산UI 중립소비를 수용하지 않는다.

다음 원인 관측은 실제 FinishClosingLine 첫줄의 기존 해제 앞/Keyboard 뒤/Pointer 뒤/첫 render·fixed 뒤에 장치 동일성·added/enabled/held/lastUpdateTime 및 공개 event time/updateType을 한정 기록하는 설계를 우선한다. 입력/시계/설정/상한을 바꾸지 않고 단일 분기1만 후보로 좁힌다. 정확 구독 수명·기록상한 설계 반환 전 새 쓰기/Unity 예산0이며 같은3 또는 종결양분기 무변경 반복0이다.
#### §8 실제 종결 첫 해제의 장치·이벤트 관측 — Approved 구현

REQ/AC-AUTOCH1-CLOSINGRELEASE-001: 모든 실행 폐쇄 뒤 Root의 명시 반환을 받아 지정 Sol은 기존 Expedition 시험1파일의 FinishClosingLine 첫줄 최초 Keyboard/Pointer 해제 주변에만 읽기 observer를 추가한다. 해제 앞·기존 Keyboard 직후·기존 Pointer 직후·기존 첫 render/fixed 관측 뒤 및 finally를 기록하고, 입력/Update/yield/120틱 관측·180순회/기존 조건·성공/실패 assertion을 바꾸지 않는다. 최소3 fixture와 다른 본문·생산은 그대로다.

구독 전에 자체 장치 두 참조/deviceId를 고정한다. 각 기록은 deviceId/added/enabled/공개 lastUpdateTime의 정확 R표기·Enter/좌우 held·InputState updateCount/type, 원본 UI receipt/Ready/TryRead·문구를 구분한다. 공개 onEvent는 해당 deviceId2의 최초4 event time/type/deviceId/handled 및 관측시 device.lastUpdateTime만 읽고, onAfterUpdate는 최초6개 update/held/time만 읽는다. 공개 event 관측과 실제 장치상태 반영을 동일시하지 않으며 event.time와lastUpdateTime 비교도 실제 관측 전 원인으로 확정하지 않는다.

기존 첫 관측순회 읽기 뒤 구독 두 개를 해제하며 예외 경로도 finally에서 해제한다. 새 관측 원시는 최대24행/각1800바이트/총64KiB·요약1행 예약이며 기존 정상 줄 기록의 별도 제한을 침범하지 않는다. callback10행과 스냅숏/정리요약을 구분하고 누락·읽기실패를 표시한다. 읽기/구독정리 오류가 기존 시험 최초예외를 덮지 않게 한다. 반사/사적상태·시계 직접대입·새API·설정변경·입력재발행0이다.

국소1묶음·원인보정최대1묶음에서 관측 역제거 원문동일·구독수명/상한/최초오류보존·설치Csc/핀을 반환하고 Luna 변경분 검토를 받는다. 반환핀 뒤 종결 분기1 정확1을 단회 후보로 결속하며 이번 절의 새 Unity예산0이다. 실제원인 관측 전 정상입력 정책이나 생산 중립을 보정하지 않는다.
두 logger의 합산 최대는 사례당48행/128KiB이며 각자 요약1행을 예약한다. 새 observer가 기존 정상logger의 예약 공간을 사용하지 않는다.

#### §8 G4 runtime23 영향 재사용 확정

Astra 직접 대조와 Luna의 최종 전수 보강에 따라 원래 G4 통과28 중 순수4와 구분되는 runtime23을 과거 증거로 재사용한다. 정확 범위는 과거 통과 Expedition17·FailureSource5·RoomExitRequiresActualCompletion1이며 SameTickFailurePrecedesBossTransition은 제외한다. g4-c1c-r1의 Expedition 실행 원문 SHA256 85866B86FBF66A94490B8BD40DE1B99B0058E9244B1ECF1FF99D3A8F3026855C와 일치하는 g4-g5-before 원문을 기준으로 대조했다.

Expedition17 본문·준비/정리·RecoverStorageAndStart·일반 저장/실패복구/assertion helper가 동일하다. WaitReady 차이는 읽기 로깅뿐이며 PlayToFailure의 대상 cause0 입력·assertion·1800초 상한도 동일하다. 추가 R02 진단은 원래 실행 인자에서 활성화되지 않고 cause1/2 입력/진단종료 경로는 해당23에 도달하지 않는다. FailureSource/RoomPlan 시험 원문 및 방 prefab6 지문은 동일하다. 일반 출정/실패 저장은 그대로이고 새 선택저장 retry·보스후선택/Transfer·Closing에 도달하지 않는다. 추가 support proof는 운동결정에 영향을 주지 않고 이 범위에서 후속 선택환경이 읽지 않는다.

따라서 원래28 중27의 재사용과 앞서 v24 후속 재개/불확실2의 재사용을 분리해 유지한다. 현재27개를 새로 실행했다거나 원래 실패7을 성공으로 수정했다는 뜻이 아니다. 보스전이1·정상원정 별칭2·세원인 순차 재Open 및 두 실제동시 실패는 앞선 미완/영향 판정을 유지한다. 새 실행예산0이며 최종 G6/G7/Testplayer/종결 필수집합을 줄이지 않는다.
#### §8 v41 첫 해제 장치 관측1 실제 실행 — Approved

v41 manifest47567바이트/132핀/SHA256 66D03CAF11942BFA83E27C6A9B57958255261E06D9331FB73E31773D235840DD와 Sol 쓰기폐쇄·최종Csc38300 종료0/회수·관측 역제거 원문동일, Root132핀 일치를 결속한다. Astra 직접 관측부 대조 및 Luna 독립 검토의 미해결 P0/P1 없음에 따라 closing-device-root-selection.v1.json SHA256 7218B1E3DA1FF4DFA55C12311DD5CEEC218FA8A263CE77F1D09E3D07BB69F258의 PlayMode BothBranchesCloseOnlyAfterRealPassageAndSave(1) 정확1만 신규경로·단회1회·단조1200초 실행한다.

이번 목적은 실제 첫 해제 event/time/장치 held/최종 갱신시각의 관측이다. 입력/Update/yield/원래조건은 불변이고 원래 실패도 그대로 보존한다. 새관측24행/64KiB와 정상관측24행/64KiB의 합산 최대48행/128KiB, 각요약예약 및 최초실패 보존을 확인한다. onEvent를 상태반영이나 생산중립소비로 대체하지 않는다.

Root만 기존 v3 조정·현재핀/Editor0/source630·정확명령/출력/선택·PID/시작신원·기한·XML결과/집합·로그/종료/회수·전후대조를 소유한다. 실행 중 소스/시험/자산쓰기0·자동재시도/기한연장0이다. 이번 원시 이후 실제 원인과 최소 입력정책 보정을 판정하며 정상양분기 성공이나 최종전달을 선취하지 않는다.
#### §8 v41 실제 관측 폐쇄 및 정상 종결의 playerloop 입력 발행 — Approved 구현

Root의 v41 단일 분기는 PID24880·종료2·55.7246379초(실행 조정56.2062618초), 정확1실패·source630불변·Editor0·회수 완료다. 로그 EDC62E2849399A0C41EA8CDADA8552FF27416F80A29DB2F1D1DF09B94166B30B 및 폐쇄 52388ACE2AFF938EC73636AE9F025BD5C9FC8A870141CC1EE87373ADC1EDD15F를 결속하고 단회 소비를 기록한다. 첫해제는 실제 event2/held해제/첫fixed중립을 보였고 line0의edge1로 다음문구에 도달했으나 line1은 Enter held/issued=false로121틱에 실패했다. line1의 event시각은 미관측이므로 시간거절을 원인으로 확정하지 않는다.

설치 InputDevice.enabled는 QueryEnabledStateFromRuntime에서 명령 및 조회 캐시를 갱신할 수 있다. 따라서 v41의 해당 공개getter 관측을 무부작용 순수읽기라고 확대하지 않는다. 실제 호출과 원시는 보존하며 첫줄 개선을 로그 시간이나 특정 시계 원인으로 귀속하지 않는다.

REQ/AC-AUTOCH1-CLOSINGQUEUE-001: 설치 InputTestFixture.Set은 UnityTest에서 queueEventOnly를 강제해 정상 playerloop가 이벤트를 처리하도록 한다. Astra/Root/Sol은 설치 원문을 대조했다. 이 지원 정책을 근거로 실행 폐쇄 후 Root의 명시 반환을 받은 Sol은 기존 Expedition 시험1파일의 정상 FinishClosingLine 전용 입력 발행만 보정할 수 있다. 최초 중립2장치, 교대 Enter/Click edge1, 그 뒤 해제2장치의 정확 세 지점에서 기존 fresh KeyboardState/MouseState를 QueueStateEvent로만 발행하고 수동 InputSystem.Update를 호출하지 않는다. 현재 소유 장치와 기존 RecordKeyboard/RecordPointer/RecordQueueSuccess 서수를 유지하며 직접상태/시계대입·장치교체·전역설정0이다.

전용 private helper는 이 정상 경로에서만 호출한다. 공용 Keyboard/Pointer/ClosingEdge와 선행 held/59틱/cancel/중복 음성은 그대로다. 기존 render+fixed·실제중립/Ready·B+60·줄당edge1/해제1·120 사전관측/180전체순회·원본문구/최종receipt assertion도 그대로다. 추가중립반복·대기증가·재edge0이다. v41 ClosingDeviceObservation의 정상 실행 호출/구독만 제거해 enabled조회 및 callback 관측이 새 정상정책에 참여하지 않게 한다. 미사용 관측 클래스 원문은 보존할 수 있고 기존 정상logger/최소3fixture는 변경하지 않는다.

국소1묶음·동일원인보정최대1묶음에서 정상전용세지점/수동Update0·기존서수/같은입력값·관측호출제거·나머지원문 불변을 대조하고 설치Csc/최종핀을 반환한다. Luna 독립검토 뒤 양분기2의 최소 실행을 별도 결속한다. 지원된 발행정책 대체이지 실제 원인 확정이나 해소 성공 선언이 아니며 이번 절의 새 Unity예산0이다.
#### §8 v42 정상 종결 queue 정책 양분기 실제 실행 — Approved

v42 manifest48133바이트/132핀/SHA256 B806567CDF88C6994B3ACFD8F1CF295ACA4AF995DEE2EA253D69E0313DF86574, Sol 단독쓰기 폐쇄·Csc43936 종료0/회수·observer 역제거=v40 및 정상queue 역치환=v40 전체동일, Root132핀 일치를 결속한다. Astra 직접 정상3지점/관측제거 대조 및 Luna 독립검토의 미해결 P0/P1 없음에 따라 closing-queue-root-selection.v1.json SHA256 A3D61E07487EA309C23AAFCFF2A6F956B5C0CAA496C0BBBA867500CDF67726DE의 PlayMode BothBranchesCloseOnlyAfterRealPassageAndSave(1)/(2) 정확2를 신규경로·단회1회·단조1200초로 허용한다.

실제 정상 종결 queue발행·중립/60틱·단일edge·기존 held음성·최종문구/저장/완료receipt를 원래 assertion으로 확인한다. 상태값/서수·기존yield와120사전관측/180전체순회는 그대로이며 실제 원인이나 성공은 실행 전 선취하지 않는다. Root만 기존 v3 조정·현재핀/Editor0/source630·명령/출력/정확선택·PID/시작신원·기한·XML각결과/집합·로그/종료/회수/전후대조를 소유한다. 실행 중 소스/시험/자산쓰기0·자동재시도/기한연장0이다.

이 정상helper 변경은 과거 미완 종결/완료 후 저장재개 경로에 영향이 있고 고정 G5 및 재사용된 G4 Hub/R01 경로에는 비도달이다. 과거 실패·관측 원시와 소비 이력은 보존한다. 이번 양분기 결과가 최종 Testplayer18/Windows 실제릴리스 입력·독립검수·패키지를 대체하지 않는다.
#### §8 보스 동시사망 정상입력 후보의 현재 소스 재기반 — Approved 구현

REQ/AC-AUTOCH1-BOSSCOINCIDENT-001: 현재 종결 실행 종료·회수·전후대조와 Root의 명시 소유 반환 뒤 지정 Sol은 기존 Expedition 시험의 PlayUntilBossAndPlayerDeath 한 helper만 승인된 boss-coincident-prep.v1 후보로 재기반할 수 있다. 옛 전체후보 파일 덮어쓰기0이며 현재 정상종결 queue/최소fixture/다른 helper를 보존한다. Sol의 준비시점 실제 support-barrier-focused-r2/source-before와 현재 Runtime C#127개 변경0, 바깥 FailureSourceTests 원문동일 및 현재 교체전helper 동일 대조를 근거로 한다.

적용 전 정확 교체전helper1700바이트/SHA256 E73718EE8D868661AF119D2309BEC32EB8D2ED42DD694379AB7E557EFE4340A5를 확인한다. 준비안 plan SHA256 17DA47426AD61B7F3DB0953B4495444E94489DA5A8AD3FE059587D7A409FFFAA 및 기존 독립 읽기 판정의 실제미검증 경계를 유지한다. 첫 BossEntry 완료tick부터6000완료틱 미만·기존1800실시간초·Root1200초 중 먼저 소진하며 기준 재설정0이다.

실제 고유19건의 Applied3 및 보스HP3, 정상입력 안전왕복과 플레이어HP1 실제피격 준비, 공격/전이 해제·payload 없음, 완료t의 실제 Hostile t+1 예약/무적·공격쿨다운/버튼해제/원본동일성 확인 뒤 마지막 정상edge1만 허용한다. 후속 같은t+1의 실제 두 Applied·양쪽HP0·원본 death publication·실패우선 및 바깥 단회 저장복귀 assertion을 유지한다. 강제 HP/위치/피해·가짜publication·새생산API·반사0이다. R05 동시원인이나 정상Closing queue 정책을 이 보스 helper에 섞지 않는다.

기록32행/행1900바이트/총64KiB·요약예약과 최초실패보존을 유지한다. 국소1묶음·같은원인보정최대2묶음 및 설치Csc에서 타입/시차/원본·입력횟수/상한·helper밖원문동일을 대조한다. 안전왕복·생존·마지막edge지연·실제동시사망은 컴파일이나 합성검사로 수용하지 않는다. 반환핀/Luna검수 뒤 정확 단일 실제선택/예산을 별도로 결속하며 이번 절의 Unity예산0이다.
#### §8 v42 종결 실패 폐쇄 및 StandaloneTestplayer 순서 호환 — Approved 준비

Root의 v42 양분기2는 PID2604·종료2·98.4913977초(실행 조정98.9788716초), 정확2실패·source630불변·Editor0·회수 완료다. 로그 566F0E1F6636F8DFBDF0BC6C5F2D804AC4A458D02D90634EFA29600A32D4D1A4 및 폐쇄 AF9B53F9ECC236151826E9B8589E70DE703C21D358FC5F735FF9F72BD72B9456를 결속하고 단회 소비를 기록한다. 두 사례 모두 첫줄 held잔류·issued=false·121틱 실패로, queue전용 정책의 실제 해소는 확인되지 않았다. 같은 Editor 문맥의 무변경 종결 반복은 하지 않는다.

REQ/AC-AUTOCH1-TESTPLAYERORDER-001: §8.5의 이미 승인된 StandaloneTestplayer 전체18 합성입력 경로를 Editor 종결 성공 및 G4잔여 완료보다 먼저 수행할 수 있도록 실행 순서만 호환한다. G5 고정44의 영향 통합 수용과 현재 제품핀·독립검수는 선행으로 유지한다. G4 실제동시원인/재Open/보스전이와 미완 Persistence는 최종 통합 수용의 필수로 그대로 남으며 이 순서 호환으로 면제하지 않는다.

새 관측 목적은 독립 Windows 플레이어의 실행루프/포커스 문맥에서 동일 정상입력·G6계획12/G7정상실패4/종결양분기2의 실제 결과를 확인하는 것이다. Standalone도 InputTestFixture의 격리 runtime을 사용할 수 있으므로 mock 연결이 완전히 제거된다고 선취하지 않는다. 같은 제품/자산 지문을 유지한 다른 지원 실행 경로이며 인간 완주/Ship실제창 입력으로 표기하지 않는다.

미소비 Windows 빌드 총2 중 Testplayer1/빌드1800초 및 기존 정상 Standalone 시험1/실행1200초만 사용할 후보로 한다. Ship1/1800초는 보존한다. 정확18/selector17을 줄이지 않고 실행 전에 실제 사례 소요 근거와 빌드/자식 시작기준 단계 기한·정확프로세스/경로 귀속·종료/회수/원시폐쇄를 결속한다. 실행 중 시간연장/자동재빌드·재시도0이다. 이 절은 순서와 준비 호환이며 현재핀/선택/명령/검수/소요근거가 결속된 실행승인 전 실제 빌드·시험0이다.

보스동시 helper 구현은 현재 선택18 밖이며 다음 소유 반환 뒤 정확helper 재기반 권위는 유지한다. 향후 시험만 변해도 Testplayer/최종제품의 의존 지문과 실제 커버리지를 대조해야 하며 과거 바이너리를 현재소스 전체와 같다고 주장하지 않는다.
#### §8 v42 StandaloneWindows64 정확18 빌드·실행 — Approved

Astra는 testplayer18-root-selection.v2.json SHA256 9ACA759DF72F5EC9974DF938B4F2EBC59F76096B502BC9CE9BB2BB397B9FC53D 원문과 현재 v42 manifest SHA256 B806567CDF88C6994B3ACFD8F1CF295ACA4AF995DEE2EA253D69E0313DF86574 결속을 직접 확인했다. 선택은 G6 12·G7 정상1/실패3·Closing 양분기2의 정확18이며 selector17이다. v1 준비안은 사용하지 않는다. 현재132핀·소스동결·Editor0 및 기존 G5수용/제품독립검수에 결속한다.

기존 미소비 Testplayer 빌드1회를1800초로, 해당 정확18 시험1회를 자식 실제출생 기준1200초로 허용한다. Ship 빌드1회/1800초는 미소비로 보존한다. 실행조정은 독립검토 완료·변경 없는 run-windows-owned.v3.ps1 13105바이트/SHA256 E52BE10650623EE191F9CF1AC1D7CC0C24F13B1CD9B9E4D9D03C7ED0840E376F를 사용한다. 출력은 신규 evidence/testplayer18-r1 및 build/testplayer18-r1/PlayerWithTests/PlayerWithTests.exe이고 모두 기존 output/chapter1-autonomous-20261021 아래다. 설치 공개 StandaloneWindows64 runTests 경로를 사용한다.

실행계획은 과거 정상30.179194초×13=392.329522초, 최근 종결양분기 약99초 및 G7압착 약23.5초를 참고한600~900초 추정이다. 다른seed/반복/플랫폼/나머지실패의 실제합계는 미측정이며1200초내 완료를 보장하지 않는다. 기한소진 시 원시를 폐쇄하고 자동재실행/연장/예산복원0이다. 이 추정으로 실패 사례를 제외하지 않는다.

Root만 실제 부모 Editor와 자식의 PID/시작신원·정확exe/부모PID·빌드1800 및 자식출생기준1200·정상종료/강제종료구분·회수·명령/결과XML·정확18다중집합/각결과·로그·산출물귀속·소스630전후대조를 소유한다. 강제자식종료나 XML폐쇄실패를 성공으로 삼지 않는다. 실행 중 모든 제품/시험/자산쓰기0이다. 빌드성공과18시험성공은 별도이며 빌드실패/기한초과/미발견/건너뜀을 원시대로 보존한다.

G4잔여·Persistence후속·최종 동일릴리스 실제창 입력·Luna 독립검수·Ship/안내패키지는 여전히 필수다. 이번은 지원된 독립플레이어 합성입력 검증이며 사람완주/실장치완주 또는 mock전체제거로 표기하지 않는다. 실제결과에 따라 원인과 후속 최소변경을 같은 계약에서 판정한다.
#### §8 Standalone18 결과·빌드 직렬화4 보존 및 정확 복원 — Approved 복원

Testplayer18-r1은 Editor29608 종료2·566.673967초, 자식24800 자연종료(자식 exit code 미관측), 강제종료0/정리오류0/Editor0/Player0·로그폐쇄·정확18집합차이0이다. 실제15통과는 Closing양분기2·G7전체4·G6의101/202/404 각3이며 Seed303반복3만 M3E1 role health does not conserve same-tick M1 applied damage 예외로 실패했다. 폐쇄 1DAA8D1AEA5FB6B22BA85BF3E05E99B5C3725763E7C0A106C66542AF13C94387, Editor로그 2D6C0D1E447FEFE917351B0CDEC4DD20C43153AF5634222805D8076DDE50D8EF 및 보존 Player.log5413988바이트/SHA256 09A3080C6CDC3ED64542240756803CC36DA1BD13102C573FE5268D929CDC5E2D를 결속한다. Testplayer빌드1/시험1은 소비 완료이며 Ship1은 미소비다.

빌드가 source630 중 아래4파일을 직렬화 변경했으므로 소스 불변 폐쇄는 실패다. 실제15성공은 원시 관측으로 보존하되 최종 현재소스/바이너리 동일성 수용을 선취하지 않는다. Root가 after 원문을 evidence/testplayer18-r1/build-mutated-source에 보존했다. Astra가 실제 폐쇄와 루트의 대응 before4 지문을 직접 대조했다.

Root에게 빌드 후 가역복구로 아래 정확4만 복원할 권위를 부여한다. 루트의 동일 상대경로 원본은 수정하지 않고, 복사 직전 길이/지문이 source-before와 같음을 다시 확인한 바이트를 output/chapter1-playable-r1/project 아래 대응 경로에만 복원한다.

- Assets/Settings/UniversalRenderPipelineGlobalSettings.asset:28956바이트/SHA256 75AC0247F483990A85E147DF1EC52C50C63F489C601E2C9B053570794982E288.
- Assets/Settings/UniversalRP.asset:4590바이트/SHA256 167D20E3285F9071DC9BCB0A4FEE7CDF8BB335DD849EC96E4B3FA39BD401F266.
- ProjectSettings/GraphicsSettings.asset:2559바이트/SHA256 BCDB6EE570B5EDC137B79E48F860495035136E494030E214FE39C151D16FE993.
- ProjectSettings/ProjectSettings.asset:20913바이트/SHA256 5A94F3C7571DC7F2EB0C51911CE70C3CA33F5351E409BC159C1D21D34270D028.

복원 전 after 보존지문·실행프로세스0을 확인하고 정확4 복원 뒤 전체630 및 v42핀을 대조한다. 원시 source-after/폐쇄/바이너리는 덮어쓰지 않는다. 복원은 빌드 당시 변경을 없었다고 바꾸는 행위가 아니며, 해당4의 의미변경 및 실제 빌드귀속 영향은 별도 읽기로 판정한다. 새Unity/재빌드/기한복원0이다. Seed303 실제생산 보존오류는 기대오류 추가·검증완화 없이 원인을 조사한다. 보스/R05 실제추가쓰기는 현재 복원/실행폐쇄 뒤 Root가 명시 반환하기 전0을 유지한다.
#### §8 일반 방 lifecycle의 원본 carried 체력 초기화 — Approved 구현

REQ/AC-AUTOCH1-CARRYBASELINE-001: Seed303 세 실패의 실제 원시와 Sol/Astra 읽기에서 일반 Combat은 방 진입 원본 carryHP를 적용하지만 RegularEnemyEncounterLifecycleSession의 초기 playerHP는5 고정이고 handoff는 source.Tick만 전달하는 불일치를 확인했다. 원시 CSV의 R03끝/R04진입 HP4 및 실제 예외를 보존하며 같은tick 역할/피해 상세가 원시에 없던 부분까지 관측했다고 주장하지 않는다.

정확4파일만 Sol 단독쓰기 대상으로 한다. 모두 현재 project/Assets/AcadeGameMaker 아래 Runtime/Combat/RegularEnemyEncounterLifecycleSession.cs, Runtime/Combat/Unity/CombatSimulationDriver.cs, Runtime/Combat/Unity/RegularEnemyEncounterHandoffAdapter.cs, Tests/PlayMode/InputUnity/Chapter1EntryPlayModeTests.cs다. Root의 전체630/v42 복원 확인·Editor0 및 명시 소유 반환 뒤 구현한다. 보스후보/R05를 동시에 적용하지 않는다.

순수 lifecycle 기존 constructor(firstTick)는 새 overload(firstTick,5)에 위임해 원래 의미를 보존하고, overload는 initialPlayerHealth1..5를 검증해 player 초기HP만 설정한다. 적HP6/9·감산보존/사망/순서/preview/commit/위조 거절은 그대로다. EncounterReset의 기존 full5/6/9 회복 의미도 변경하지 않는다. 현재 완료HP에서 피해를 더해 역산하거나 첫 publication을 무검증 학습하지 않는다.

CombatSimulationDriver에는 이미 비활성 초기설정에서 보존한 원본 _chapterCarryTick/_chapterCarryHealth만 읽는 internal 한정 reader를 추가한다. 미설정이면 기존5, 설정이면 요청 firstTick의 정확일치·HP1..5를 검증한다. 세션 생성/상태변경/사후HP참조/캐시갱신0이다. HandoffAdapter는 첫 lifecycle 생성에 이 원본 baseline만 전달하며 기존 source/geometry/preflight 검증과 단회 commit 순서를 보존한다. 새publicAPI/friend/파일0이다.

기존 Entry시험 파일에 CarriedPlayerHealthSeedsLifecycleAtExactEntry의 ValueSource CarriedEntryHealthCases={1,2,3,4,5} 정확5를 추가한다. 기존 runtime fixture wrapper와 원문 준비/정리를 재사용한다. 비활성 원본 regular graph를 소유하고 기존 player/transfer tick20 및 Combat carried 설정을 적용해 활성전 reader의 미설정5·설정값 반복동일·19/21 거절을 검증한다. 실제 첫두fixed20/21의 원본 Combat/Handoff lifecycle playerHP·동일tick·다음tick과 피해없음을 실제 결과로 확인한다. 새카메라/생산API로 시험멤버 오기를 보충하지 않는다.

같은 메서드의 분리 순수 fixture는 constructor HP0/6 거절, 빈피해인데HP변화 입력 거절, 실제 CombatSession.RegisterCarriedPlayer/Process 피해1 결과에 따른 감산 승인 및 불일치HP 거절, 거절 후 nextTick불변과 기존 EncounterReset5회복을 확인한다. 합성 순수피해를 게임 실제피해 증거로 합산하지 않으며 게임graph의 활성HP/위치/피해 직접주입0이다.

국소1묶음·같은원인보정최대2묶음 및 설치Csc로 정확4변경/기존guard·default5·reset·소유정리/원본baseline/처음두publication 경계를 확인하고 반환핀을 고정한다. Luna 독립 영향검토로 기존 G5/G4 및 Standalone15 실제성공의 재사용/필수재관측을 판정한다. Entry 클래스에 새5가 추가되므로 고정44의 클래스집합은 같아도 사례분모가 늘며 기존668을 새5 포함 통과로 표시하지 않는다. 실제검사는 새5 최소 및 Seed303 정확3을 후보로 하되 반환핀/경로/선택/예산은 별도 승인한다. 이번 절의 Unity/추가빌드예산0이다.실제 view 접근 정정: 기존 RegularEnemyEncounterPresentationView에는 Lifecycle/Roles/HP가 없으므로 새 view/API/반사를 추가하지 않는다. 실제20/21의 Combat.LatestOutcome.Combatants HP와 빈피해 결과, 동일tick Handoff.HasOwnedLifecycleForEncounterHandoff·LatestPresentationView.Tick/NextExpectedTick·PlayerAlive·빈 AppliedHitTargetIds를 원본쌍으로 확인한다. 순수session의 LatestSnapshot.Roles에서 직접 baseline/감산HP를 검증한다. 앞의 실제 Handoff HP 확인 표현은 이 엄격검증 후 최종발행 확인으로 한정한다.
#### §8 빌드 파생설정 보존·정확복원 조정 v4 — Approved 준비

Root는 기존 run-windows-owned.v3.ps1을 보존하고 preparation 아래 별도 v4 실행조정 후보만 작성할 수 있다. 제품/시험 소유와 충돌하지 않는 준비파일 작업이며 새 빌드/실행예산0이다. 빌드 직전 승인된 정확4 설정파일의 원문 바이트/길이/지문을 새 증거파일로 보존하고, 종료 및 소유 Editor/자식0 확인 뒤 실제 source-after·변경4 원문/지문·산출물 전체핀을 먼저 폐쇄한다.

자동 복원 대상은 앞 절의 정확4뿐이다. 보존한 before 바이트를 해당 실제프로젝트 경로에만 복원하고 전체630 및 실행manifest를 대조한다. 네파일 밖 변화·프로세스잔존·원문소유/보존실패·복원불일치는 실패로 남기며 다른 파일을 자동수정하지 않는다. 원시 변경diff와 복원diff를 분리하고 복원/파싱 오류가 원시 폐쇄 기록을 막지 않게 한다. 설정 파생차이를 의미무영향이라고 선언하지 않고 각 Testplayer/Ship 빌드의 옵션·실제after설정·바이너리·실행증거를 연결한다.

기존18 상수를 미래의 승인 선택표에 결속된 정확사례기수/이름다중집합으로 일반화할 수 있으나 임의선택/축소/현재24실행 권위는 아니다. 부모/자식 경로·PID/출생·빌드1800/자식1200·강제종료거절·회수·기존원시보존을 유지하고 AST/국소검사 및 Luna 독립 변경분 검토를 거쳐 반환한다. 실제 후속 선택·기한·추가 Testplayer 예산은 구현/영향범위가 닫힌 뒤 별도 승인한다. Ship1 미소비는 유지한다.
#### §8 v43 원본 이월 체력 Entry5 실제 관측 — Approved

REQ/AC-AUTOCH1-CARRYBASELINE-001의 구현 반환 manifest134핀/50216바이트/SHA256 A011710500B5A9CBFA3AD4EE15A59F9ACBEEFDEEB0342260B8C1B82762217F59, 정확 생산3/시험1 변경·Sol 쓰기폐쇄·설치Csc8992/8224/47832 종료0 및 회수, Root134핀 전수일치와 Luna 변경분 독립검수의 미해결 P0/P1 없음을 결속한다. 순수소스48 검사는 실제 Unity 첫20/21 발행을 대신하지 않는다.

Astra가 직접 확인한 carry-entry-root-selection.v1.json SHA256 43CB8B22FB362A18EC231B7165DCD3813EF44290ACC290C52BD0ACE3AF2B99B8의 CarriedPlayerHealthSeedsLifecycleAtExactEntry(1)..(5) 정확5를 EditMode/ownedBatchFixture=true/기존 run-focused-owned.v3 조정으로 신규경로 단회1회·단조1200초에 허용한다. 실제 비활성 원본 graph의 기존 carry 설정으로 시작한 첫20/21 Combat 결과와 엄격 Handoff 최종발행을 확인하며 새5의 통과를 미리 수용하지 않는다.

Root만 실행 전 현재134핀·source630·Editor0 확인, 명령/선택/PID·시작신원·기한·결과XML 정확5 다중집합/각결과/건너뜀·로그·종료/회수·소스 전후대조를 소유한다. 실행 중 모든 소스/시험/자산쓰기0, 자동재시도·기한연장0이다. 실패 시 최초 원시를 보존하고 원인과 최소 후속범위를 먼저 판정한다.

기존668 결과의 낮은 carry 첫 Handoff 도달 영향은 별도 독립 분류가 남아 있다. 이번5를 기존668에 선합산하거나 전체673 동시통과로 표기하지 않는다. Seed303 실제 원정3, 추가 Testplayer 빌드와 최종 Ship의 예산은 이번 승인에 포함하지 않는다. 기존 Windows18의 파생설정4 및 15통과/3실패 원시와 복원 사실도 그대로 보존한다.
#### §8 v43 Entry5 폐쇄 및 보스 후보 재기반 근거 갱신 — Approved

carry-entry-r1은 PID21728 종료0·25.4366374초(조정25.8746222초), 정확5 통과·건너뜀0·선택차이0·source630불변·Editor0·회수 완료다. 로그95AD9BA34BD060728903BF686F991B99EE3F2C6D72EEDBAC2F614D0622834D90 및 폐쇄36977F719D55428B7D38FD555760B670C45EC33EEBAFFDC531ABD0C3FE9B3FAD를 결속한다. 새 원본 carry1..5의 첫20/21 실제 발행 경계가 통과했다. Seed303 원정의 실제 후속 관측은 아직 남으며 이5를 해당 원정의 성공으로 대신하지 않는다.

기승인 BOSSCOINCIDENT-001의 정확 PlayUntilBossAndPlayerDeath helper 적용 권위를 유지한다. 적용 기준은 현재 v43이며 Expedition은 v42 원문과 같고 교체전 helper SHA256 E73718EE8D868661AF119D2309BEC32EB8D2ED42DD694379AB7E557EFE4340A5를 다시 대조한다. 준비 당시 Runtime127 전체불변 표현은 현재 기준으로 재사용하지 않는다. 이후 생긴 정확 생산3의 변경은 일반 방 lifecycle 원본 carry 초기화이며 기존 default5와 보스 전용 Driver 경로를 바꾸지 않는다. Sol은 적용 전 현재 변경3의 의존 대조를 기록하고 옛 전체후보 덮기 없이 현재 helper 하나만 교체한다.

Root의 Entry5 종료 폐쇄 및 명시 소유 반환 뒤 단독쓰기한다. 기존6000완료틱 미만/1800실시간초/Root1200초 중 먼저 소진, 실제 고유 피해·양쪽 같은tick 사망·단회복귀 검증, 로그한도, 국소1묶음 및 동일원인보정최대2묶음은 그대로다. 현재 carry 시험/정상종결/생산3 및 다른 본문 변경0이다. 반환핀과 독립검수 후 정확 실제선택을 별도로 결속하며 새 Unity/추가빌드예산0이다.
#### §8 R05 체력·압착 동시원인의 정상입력 후보 — Approved 준비

REQ/AC-AUTOCH1-R05COINCIDENT-001: Root는 별도 Sol 한 명에게 고유 preparation/autonomous-ch1-20261021/r05-coincident-prep.v1 아래의 준비 후보만 배정할 수 있다. 현재 제품·시험 단독 writer와 파일 소유를 분리하며 실제 Assets/ProjectSettings/Packages 변경·설치Csc·Unity·빌드0이다. 준비 산출물은 현재 PlayUntilHealthAndCrushCoincide 원본 지문, 해당 helper만의 교체 후보와 차이, 입력·원본 관측 정책, 의존/실패/정리/미검증 목록이다. 기존 전체 시험파일을 이후 그대로 덮어쓰는 방식은 승인하지 않는다.

후보는 실제 R05 첫 완료tick부터8000완료틱 미만, 기존1800실시간초 및 Root1200초 중 먼저 소진한다. 기준 재설정0이며 준비2500틱, 이후 최대32 Surveyor주기4448틱, 마지막 접근·동시실패확인180틱은 모두 전체8000 안의 부분상한이다. 남는 여유를 별도 추가기한으로 더하지 않는다. 추가 정렬주기·재Open·자동재시도0이다.

Walker는 정상 공격으로 처치하고, 원본 carryHP H를 읽어 실제 피해 H-1회로 HP1을 준비한다. H=5 가정이나 활성HP/위치/속도 직접설정0이다. HP1 이후 일반 예고는 실제 latched target과 몸체 폭으로 선을 피하며 생존해야 한다. 선택 예고의 latch phase15..29, 실제 선의 발사·만료 및 현재 원본 몸체 위치를 대조하고 마지막 열림 phase59의 정상 D+LeftShift 단일edge 접근 후보를 검토한다. 다음 phase60의 실제 피해처리·체력0·압착 겹침을 원본 publication으로 확인해야 하며 기하 상수나 위상 계산만으로 성공 처리하지 않는다. 32주기 창의 산술은 실제 표적 정렬·생존·입력 소비 성공을 보증하지 않는다.

현재 공개된 behavior/threat/몸체 완료 view 및 hazard 순수 조회만 사용한다. ThreatDeliveryCountForTests는 사전 보조 신호일 뿐 개별 피해예약의 RequestId를 관측했다고 주장하지 않는다. 최종 실제 CanonicalRequestTrace의 enemy.line 피해1/Applied와 HP0, 같은 원본 receipt의 health/crush 둘 다 참, 실제 Flow 복수 원인·실패우선 및 기존 단회 저장복귀를 요구한다. 새 큐읽기 API·반사·가짜 publication·피해 주입·기대 완화0이다. Dash에 피해무적이 있다고 가정하지 않는다.

기록은 최대32행/행1900바이트/총64KiB 및 최종 요약1행 예약, 누락/읽기실패와 최초 예외 보존으로 한정한다. 기존 assertion/바깥 종료·재개 검사는 유지하고 후보 밖 원문 동일성을 기록한다. 준비 단계에서 지원 멤버나 정상 생존 경로가 성립하지 않으면 미해결로 반환하며 API 확대나 실패 은폐로 보충하지 않는다. Astra 설계 판정·독립검토·Root 쓰기 소유 반환 전 실제 적용0이다. 이번 절은 구현/실행/추가 Testplayer 예산을 발급하지 않는다.
#### §8 v43 G5 영향 통합 및 빌드조정 v4 독립검수 기록

Luna는 기존 고정44/668의 원시 목록과 선택 본문을 대조했다. regular handoff에 닿는 기존 Entry의 SeedsAllProducersAtOneTick, InactiveConstructionSeedsBeforeAwake, ReplacementWaitsForDestroyAndActionsDispose 및 RegularInput의 RegularCommitUsesGlobalReceipt, PublishesOnlyCompletedProducerEvidence는 피해 없는 첫 완료tick/교체 또는 기본fixture의 초기HP5다. adapter 경계는 미설정 reader의 기존5 경로이며 HP3을 지정한 기존 Entry는 보스 전용 그래프로 일반 reader/handoff에 비도달이다. 나머지 선택 클래스와 Play7에서도 낮은 carry로 첫 regular handoff에 도달하는 기존 사례는 확인되지 않았다. 원래 constructor/default5 및 비도달 근거로 기존668을 영향 재사용하고 새 Entry5 실제통과를 별도 결속한다. 현재 고정44의673 고유사례 범위는 분리된 증거의 합이며 동시673 전수실행이라고 표기하지 않는다. Seed303 원정은 이 G5 분모 밖이며 여전히 실제 후속 관측 대상이다.

Root의 run-windows-owned.v4.ps1 준비본18967바이트/SHA256 A2D88A500744904667FE8A707F283652B3101D916087DF527EB8489CAB245F03는 AST오류0이고 Luna의 독립 변경분 검수에서 P0/P1 잔여없음이다. 정확4 보존/폐쇄 후 복원 및 전체차이·manifest 검증과 선택기수/정확이름 집합 검증을 확인했다. Astra도 보존·복원 선행과 실패격리 경로를 읽었다. 이는 미실행 준비본의 검수 결과이며 실제 복원·프로세스/파일 연동의 성공 증거가 아니다. 추가 Testplayer 또는 Ship 실행은 현재 승인된 선택/기한/산출물 결속을 별도로 닫기 전0이다.
#### §8 R05 준비 대상 실제 식별자 정정

R05COINCIDENT-001 준비 절의 PlayUntilHealthAndCrushCoincide는 행동을 설명하며 잘못 적은 함수명이다. Astra가 현재 실제 소스를 대조한 정확 대상은 Chapter1AuthoredInputTrace.PlayUntilConcurrentFailure이며 호출자는 Chapter1ExpeditionPlayModeTests.SingleReturnForConcurrentCauses다. 호출자는 seed202 시작 뒤 해당 helper를 실행하고 AcceptedFailureCauseCount>1 및 RunEndCommittedRevision/ReturnedToHubRevision 각각1을 검사한다. 준비 후보는 이 실제 helper 원문 지문과 호출자에 결속한다. 새 함수를 만들거나 이름을 바꾸는 권위가 아니며, 고유 준비폴더·정상입력/기한·원본 복수원인/단회복귀·실제 적용0 범위는 그대로다.
#### §8 v44 보스 동시사망·기존 보스 실패 정확2 실제 관측 — Approved

v44 manifest134핀/51342바이트/SHA256 DFA559C786AA639FB1DDBFBF30F9F3907BFDBFAC51BF39CA44C0BCAABAA439AD와 Expedition162856바이트/SHA256 670610EE6B91124506D3863FF9F8BC6E36A7F55582E04FFCEB2F6D3DFCB183B2를 결속한다. Sol 쓰기폐쇄·helper밖 원문동일·설치Csc5832 종료0/회수 및 Root134핀 일치·Luna 독립 변경분 검수의 미해결 P0/P1 없음을 확인했다. 초기 Csc 오류와 국소8의 합성/정적 한계를 보존하며 실제 양사망은 아직 미관측이다.

Astra가 선택 원문과 지문을 직접 확인한 boss-coincident-root-selection.v1.json SHA256 196E0223665988F80A3540612342C1E2A3BD43A86C889965DC70975A5CFBE7B4의 SameTickFailurePrecedesBossDeath 및 SameTickFailurePrecedesBossTransition 정확2를 PlayMode/ownedBatchFixture=false/기존 v3 조정으로 신규경로 단회1회·단조1200초에 허용한다. 새 Death 검사는 실제 양쪽 같은 tick 피해/죽음·실패우선·단회복귀를 요구한다. Transition은 PlayToBoss 뒤 기존 PlayUntilFailureAccepted를 호출하며 새 동시사망 helper를 호출하지 않는다. 따라서 별도 보스 구간 health 실패·복귀 관측으로 보존하고 이름만으로 동시사망 성공으로 합산하지 않는다.

Root만 현재핀·source630·Editor0·명령/정확선택·PID/출생신원·기한·XML 각결과와 이름다중집합·건너뜀·로그·종료/회수·전후대조를 소유한다. 기존 helper의6000완료틱 미만/1800실시간초와 Root1200초 중 먼저 소진하며 추가 대기·재시도·기한연장0이다. 실패는 최초 실제 원인과 단계 기록으로 폐쇄하고 무변경 반복0이다. 실행 중 제품/시험/자산쓰기는0이며 별도 R05 preparation 후보 쓰기는 실행 소스 밖에서만 허용한다. 추가 Windows 빌드 예산은 없다.
#### §8 v44 보스 정확2 실제 폐쇄와 다음 관측 한계

boss-coincident-r1은 PID8364 종료2·202.1398391초(조정202.5820688초), source630불변·Editor0·정확2·회수 완료다. 로그52B8547F0722B6B71FAC7E9074819DA2AAD6C907854285F0F0BB8E858D2FD1F9 및 폐쇄D70A9AC687038014A99B1B14E200410D646232A309036CB05CA523F3424CA2E4를 결속한다. SameTickFailurePrecedesBossTransition은 통과했고 기존 보스 구간 health 실패 관측으로 한정한다. SameTickFailurePrecedesBossDeath는6000완료틱 상한에서 실패했으며 실제 동시사망은 미관측이다.

Death 원시의 보스 진입carryHP4/firstTick1475, 실제피해 후HP3/tick1524, 안전 준비 위치8.81665/tick1627을 보존한다. 준비 공격 실제Applied는0이고 bossHP60으로 남았다. 공격 조건별 거절이나 실제 Attempt 상세가 기록되지 않아 기기 읽기 잔류를 원인으로 확정하지 않는다. Seed101을 정상 원정의 실제carryHP5로 가정하지 않는다. 앞의 기본5 동등성은 특정 기존 G5 fixture의 근거이며 전체 원정의 실입력 조건으로 확대하지 않는다. 후보 자체는 원본carry1..5를 읽으므로 이번4는 그 입력 범위 안이다.

같은 Editor 문맥의 무변경 재실행0이다. 다음 정상 발행 후보는 직전 발행false 이력과 실제 선택/쿨다운/화면조건을 사용하되 실제 소비·공격 성공은 AttackAttempt와 Canonical Applied로만 판정한다. 기기 held 읽기는 진단값으로 분리할 수 있으나 발행false를 실제 해제 관측이라고 표현하지 않는다. 마지막 edge 전에는 해제 발행 뒤 새 완료 publication의 원본 tick/receipt 및 NoAttempt 확인을 포함하는 정확 설계를 먼저 반환한다. 실제 고유19 Applied3·최종 단1edge·두 Applied/양HP0·실패우선/단회복귀 검사는 유지한다. 현재는 설계 읽기만이며 추가 구현/Unity/Windows 빌드예산0이다.
#### §8 보스 준비 공격의 발행 이력·완료 관측 결속 — Approved 구현

BOSSCOINCIDENT-001의 같은 helper 한곳에 한해 Sol 단독쓰기 보정을 허용한다. Root의 v44 실행 폐쇄 및 명시 소유 반환이 선행이다. 로컬에는 Pointer가 성공 반환한 마지막 attack/transfer 발행값, 발행 완료tick 및 로컬 발행서수만 기록하며 이를 실제 소비 상태로 표시하지 않는다. 기존 Keyboard/Pointer 구현과 입력값 형식·yield·6000완료틱 미만/1800실시간초·Root1200초는 바꾸지 않는다.

준비 공격은 직전 false/false 발행 뒤 새 완료tick의 같은 원본 receipt와 실제 Attack.Attempt.HasValue==false, 선택ordan·쿨다운≤다음tick·카메라 pixelRect 내부를 확인한 뒤 true/false edge1을 발행한다. NoAttempt라는 새 열거값/API는 만들지 않으며 기존 nullable Attempt 부재를 뜻한다. true 뒤 다음 순회에서는 false를 발행하고, 그 해제 뒤 새 완료 관측과 기존 쿨다운 조건 전에는 다시 true를 내지 않는다. 기기 held 읽기는 발행 선행에서 제거하고 진단 참고값으로만 남긴다. 완료 Attempt 부재만으로 물리 버튼 중립이 증명됐다고 주장하지 않는다.

최종 edge는 실제 고유19 Applied3·보스HP3·플레이어HP1, 원본 Hostile의 t+1 Queued 및 실제 pending의 단일 hostile/동일RequestId/payload없음, 무적·쿨다운·선택·화면조건, 직전해제 발행 및 그 뒤 새 완료 Attempt 부재가 함께 성립할 때 단1회만 발행한다. 그 다음 원본tick 두 Applied·양쪽HP0·같은tick death·실패우선·바깥 단회 저장복귀 assertion을 모두 유지한다. 누락 edge의 같은tick 재발행·기대완화·입력성공 추정0이다.

관측은 최초 준비 gate판정, 최초 준비true 발행, 그 다음 새 완료 결과의 정확3행을 추가한다. 각 행은 조건별 참/거짓·실제 참고held·선택/쿨다운/화면·발행tick/서수와 실제 Attempt/Canonical 결과를 구분한다. 기존 health변화 로그는 최초1회만 출력하고 전체변화수/최종HP를 요약에 남겨32행/행1900바이트/총64KiB·요약예약 한도를 유지한다. 실제19 피해 검증/기록 및 모든 HP 상태 갱신은 그대로다. 상한소진 시 최초 예외를 보존한다.

국소1묶음 및 동일원인보정최대2묶음, 설치Csc와 helper밖 원문동일·정확 입력횟수/완료관측 경계·로그한도 대조 뒤 반환핀을 고정한다. 원인미확정을 유지하며 같은 Editor 문맥 재실행0, 후속 Windows 묶음은 별도 선택·예산 승인 전0이다. R05 후보에 이 정책을 자동 적용하지 않고 해당 설계/관측 경계는 따로 검토한다.
#### §8 R05 준비 후보 독립검수 보정 r2 — Approved 준비

R05COINCIDENT-001 준비 Sol은 기존 preparation/autonomous-ch1-20261021/r05-coincident-prep.v1 원문을 보존하고 같은 고유 준비영역의 r2 이름을 가진 후보/차이/계획/지문만 추가할 수 있다. 별도 제품 writer의 보스 helper 쓰기와 겹치지 않으며 실제 Assets 적용·Csc·Unity·빌드0이다.

Luna가 제기한 Walker 정상처치 근거를 보강한다. Walker HP감소를 실제 같은 완료tick의 player BasicAttack 요청 및 대응 Applied 결과·RequestId·피해량과 결속하고, 다른 적/다른 원인 피해나 근거 없는 HP변화를 정상처치로 수용하지 않는다. Surveyor 생존과 실제 carry H-1 피해 준비·전체8000 및 부분상한은 그대로다.

Walker 공격 발행 선행의 기기held 의존은 보스 승인안과 같은 로컬 성공반환 발행이력 및 해제 뒤 새 완료 원본 Attack.Attempt.HasValue==false로 대체하는 준비안을 작성할 수 있다. 실제 target/쿨다운/원본receipt·화면조건 및 다음 완료의 Canonical Applied만 피해 성공 권위로 유지한다. true 다음 false, 그 뒤 새 완료 관측 전 재true0이며 발행이력을 실제 소비나 물리중립으로 표시하지 않는다.

마지막 D+LeftShift 발행은 입력 요청과 실제 소비를 구분한다. 현재 지원된 완료 motion/action/receipt에서 원본 같은tick 소비를 확인할 수 있으면 정확 멤버와 소유 관계를 결속한다. 지원 읽기가 없다면 대시 소비 자체는 미관측으로 명시하고, 새 API/반사/상태대입으로 보충하지 않는다. 그 경우에도 실제 다음 완료 몸체의 선 접촉·열림/압착 영역 진입과 최종 실제 enemy.line Applied·HP0·같은 receipt의 health/crush·실패우선·단회복귀가 필수다. 입력 발행만으로 대시 성공이라고 수용하지 않는다.

위 세 경계 외 행동·기한·기록32행/1900바이트/64KiB·최초실패 보존·바깥 assertion을 바꾸지 않는다. 실제 적용 권위는 r2 반환 및 독립검토 후 별도로 판정한다.
#### §8 Windows 자식 종료코드 귀속 조정 v5 — Approved 준비

Root는 검수된 run-windows-owned.v4.ps1을 보존하고 별도 v5 준비본만 작성할 수 있다. 자식 발견 시 기존 정확 executable 경로·부모PID·출생신원을 검증한 같은 공개 Process 객체의 SafeHandle을 확보해 종료 관측까지 수명을 유지한다. 핸들을 별도 새 프로세스의 신원으로 대체하거나 사적 API를 사용하지 않는다. 관측 전에 자식이 종료되어 신원/핸들/종료코드를 확보하지 못한 경우 미관측 실패로 남기며 재실행으로 보충하지 않는다.

자연종료와 강제종료 여부 및 관측한 종료코드를 분리하고 capturedPlayerExitCode를 원시 폐쇄에 기록한다. Testplayer의 종료코드 null 또는0 이외 값은 실제 전체통과로 수용하지 않는다. 강제종료는 종료코드0이더라도 기존 거절 조건을 유지한다. 종료코드 읽기/핸들 정리 예외가 부모 회수·설정 원시 보존/정확복원·최종 폐쇄 기록을 막지 않게 격리한다. Process 및 핸들 수명은 중복 폐기나 다른 프로세스에 대한 정리를 피하도록 명확히 기록한다.

기존 부모/자식 경로·PID/출생·부모관계·빌드1800/실행1200·선택 다중집합·소스630 및 승인설정4 보존/복원·원문 지문·강제종료거절은 그대로다. AST·국소 검사 및 Luna 독립 변경분 검토 후 반환한다. 이번 절은 준비파일 권위만이며 실제 실행/추가 Testplayer 빌드 예산0, Ship1 미소비 보존이다.
#### §8 R05 준비 r3 비생성 관측·기록 예외 격리 — Approved 준비

같은 준비 Sol은 r1/r2를 보존하고 r3 후보·차이·계획·지문만 추가할 수 있다. 정확 변경은 helper의 PlayerMovementController.Snapshot 두 접근을 기존 TryReadExistingMotionForAuthoring으로 바꾸어 성공·motion.Tick==현재 완료tick·next==tick+1을 단언하는 것이다. Snapshot getter는 EnsureMotor를 호출하므로 기존 모터가 없을 때 생성할 수 있다. 새 API 없이 현재 모터의 비생성 읽기로 대시 ActionState 및 속도/접지 관측을 유지한다.

아울러 현재 Record 함수 내부의 문자열/바이트계수/Debug.Log 예외를 회수하여 finally의 요약 기록이 최초 검사 예외를 덮지 않도록 한다. 기록실패 수는 별도 집계하고 가능한 요약에 표시하되 재귀 기록이나 새 로그 재시도0이다. 게임 입력·검사 예외를 잡아 성공 처리하지 않는다. 기존32행/1900바이트/64KiB·요약예약, 모든 행동/시한/판정과 바깥 assertion은 그대로다. 실제 Assets 적용·Csc·Unity·빌드0이며 반환 후 독립검토 대상이다.
#### §8 후속 정확24 소요 근거와 시험 전체상한 호환 v6 — Approved 준비

첫 Windows18 자식 약503초, 보스2 Editor 약202초, R05 후보8000완료틱의 관측속도 참고 약267초, 순차실패60~100초 및 Persistence2 100~160초의 합은1132~1232초다. R05 접근과 플랫폼 차이 여유가 포함되지 않아1200초 전체상한에는 구체적 소진 위험이 있다. 이 수치는 계획 근거이지 성공/소요 보증이 아니다. 이전1200초 승인·소비·실패 원시는 그대로 보존한다.

새 후속 Testplayer 정확24 묶음의 실행 전 상한을1800초로 정할 준비 호환을 허용한다. 실행 후 시간연장이 아니며 아직 실제 빌드/시험 예산은 발급하지 않는다. 빌드1800초는 별도다. 기존 helper 내부 완료틱/실시간 상한과 입력·assertion은 바꾸지 않는다. 해당 향후 묶음에만 바깥 Root 시험상한이1800초가 되는 것이며 과거 Root1200초 문구를 소급 변경하거나 각 사례에 별도1200초 감시가 있다고 주장하지 않는다.

Root는 v5를 보존하고 v6 준비본에서 kind=Testplayer, runId=testplayer24-r1, expectedCaseCount=24인 정확 선택만 runSeconds=1800을 허용할 수 있다. 그 외 기존 경로는1200을 유지한다. 자식출생 기준 시험 기한·진행기록·종료유예의 동일 상한을 selection.runSeconds에 결속하고, 빌드1800·프로세스신원/핸들/종료코드·선택다중집합·설정4보존복원·630대조·강제종료거절·폐쇄는 그대로다. 유예를 별도1800초로 더하지 않는다. AST/국소·Luna 독립검수 후 반환한다.

정확24는 기존18+보스2+R05동시1+세원인 순차재Open1+Persistence2의 후보이며 필수검사 축소0이다. 마지막 소스핀·정확이름/selector·소요·출력/검수 결속 뒤 추가 빌드1/시험1의 유한 실행을 별도 승인한다. 자동재시도0, Ship1 미소비 보존이다.
#### §8 R05 r3 정상입력 helper 실제 적용 — Approved 구현

R05COINCIDENT-001의 준비 r3 helper20605바이트/SHA256 36EFA039AFE4D39BE39614031F7FF5349E9B9F6F9CD09963D326653980C7C085를 적용 후보로 승인한다. Astra는 비생성 motion 읽기 두 곳·현재/다음tick·대시 상태와 기록 예외 격리를 직접 대조했고 Luna의 r3 독립 변경분 검수는 미해결 P0/P1 없음이다. 실제 컴파일/생존/동시원인 성공은 아직 미관측이다.

Root의 현재 v45 쓰기폐쇄·핀 일치·Unity0 및 명시 소유 반환 뒤 지정 Sol은 현재 Expedition 시험파일의 PlayUntilConcurrentFailure 한 helper만 재기반한다. 교체전 helper SHA256 34F7A4C431E17DD80479A92F5203536BEA5413511CF12C929AD830346E3143DA를 다시 확인한다. 후보 전체파일 덮기0, 보스 v45 helper/Closing/생산/호출자 및 바깥 복수원인·저장복귀 assertion 변경0이다.

준비 승인된 실제 정상 Walker 공격·요청/Applied/HP보존, carry H-1 피해/Surveyor생존, 실제 예고/선/위상·정상 대시 단1입력과 완료Dashing·최종 실제 enemy.line Applied/health/crush 같은receipt·실패우선/단회복귀를 그대로 구현한다. 기기held 대신 발행이력/해제 뒤 새 완료 Attempt 부재를 사용하는 경계와 실제 소비/발행의 구분을 유지한다. 전체8000완료틱 미만/준비2500/정렬4448/최종180 포함상한·기존1800실시간초·기록32행/1900바이트/64KiB·최초예외 보존을 유지한다. 향후 정확24 외부1800초 호환은 별도 실행 승인에 따르며 현재 실행0이다.

국소1묶음·같은원인보정최대2묶음 및 설치Csc에서 실제멤버/타입·서수/phase시차·로그한도·helper밖 원문동일을 검증하고 최종 manifest를 반환한다. 타입 오류는 기존 지원 멤버로만 고치며 생산API/friend/상태주입 확대0이다. 컴파일이나 순수 조건검사를 실제 정상처치·생존·동시원인 통과로 표시하지 않는다. 적용 뒤 독립검수/최종핀/정확선택을 결속하기 전 Unity/추가빌드0이다.

#### §8 후속24 상한 예외의 정확이름 결속 v7 — Approved 준비

v6의 runId/kind/count만으로는 임의의 다른24 사례도1800초 예외를 받을 수 있다는 독립 지적을 수용한다. Root는 v6를 보존하고 별도 v7 준비본에서 testplayer24-root-selection.draft.v1.json SHA256 DC00E3D88DFE69BC21EB6DE663DF0E07926871E91FD381BF37426650933268B2의 expectedCases 정확24 원형을 고정 배열로 결속할 수 있다. Astra가 해당 원문을 직접 확인했다. 선택의 이름 다중집합이 이24와 완전히 같고 기존 runId/kind/count도 맞을 때만1800을 허용한다. 중복/누락/다른이름은 거절한다. 실제 regex항목은24이며 과거18의 selector17 기록은 보존한다.

그 외 기한/빌드/신원/종료코드/설정복원/폐쇄 동작은 변경하지 않는다. AST·국소 및 Luna 독립 변경분 검수 후 반환하며 최종 소스manifest와 실제선택 SHA의 실행승인은 별도다. 준비안의 미기입 manifest를 실행에 사용하지 않는다.
#### §8 v46 StandaloneWindows64 정확24 추가 단회 빌드·시험 — Approved 실행

v46 최종 manifest134핀/53885바이트/SHA256 0DC30CA6EEAE1BD5547B05FADC96F7503ADB96A5E99860D9C61CAC95073F954B와 Expedition SHA256 0F6C3E68D296AC35E77B0966851C80C0CC3BB873B3A2B117C7C13566B2DF2735를 결속한다. 이전33A42 접두 준비핀은 사용하지 않는다. Sol 쓰기폐쇄·설치Csc14452 종료0/회수·국소15·helper밖 원문동일 및 Root134핀 전수일치/source630/Unity0, Luna의 실제 r3 적용·기존 G5/G4 추가영향없음/R05 직접관측 필요 판정을 선행으로 확인한다. v45 보스 발행 보정과 v46 R05는 실제 성공 미관측으로 유지한다.

Astra가 최종 원문과 지문을 직접 대조한 testplayer24-root-selection.v2.json 3557바이트/SHA256 E023E9F46693A95B4D1943067645D06FE31D2A8DCC4BDBED4CA7D82803647671를 유일 실행선택으로 한다. 구성은 G6 12·G7 4·Closing양분기2·보스동시/일반실패2·R05동시1·세원인순차재Open1·Persistence2의 정확24이며 실제 필터 항목24다. v1 및 draft는 원시 준비로 보존하고 실행에 사용하지 않는다.

이미 소비한 Testplayer18과 별도로 이번 원인보정 관측을 위한 추가 Testplayer 빌드1회/1800초 및 그 산출물의 정확24 시험1회/자식 실제출생 기준1800초를 승인한다. 최초 예상1132~1232초 이상과 접근/플랫폼 여유가 근거이며1800내 완료를 보장하지 않는다. 자동재빌드·재시도·실행 중 기한연장0이다. Ship 빌드1회/1800초는 미소비로 보존한다. 기존 각 helper의 내부 완료틱/실시간 상한·검증조건은 유지한다.

실행조정은 run-windows-owned.v7.ps1 22432바이트/SHA256 57ACDC2B4E16071F5BF0B573E5EF031A8E6673214C39FB3C5A94D58F5F14C66C를 사용한다. AST0·정확24 수용/상이명·중복·23개 거절 국소4 및 Luna의 마지막 지적 해소를 결속한다. Windows-runner-v7-local 원시 지문 F7470A30E91F0CD730F54973FCD4B38A6292DDE425047F25DE0BCBDA8CD8687C를 보존한다. 출력은 output/chapter1-autonomous-20261021/evidence/testplayer24-r1 및 build/testplayer24-r1/PlayerWithTests/PlayerWithTests.exe의 신규경로다.

Root만 공개 StandaloneWindows64 시험경로와 부모/자식 정확경로·부모PID·PID/출생·SafeHandle 수명·빌드/자식 기한·자연/강제종료·관측한 exit code·회수·XML 정확24 이름다중집합/각결과/건너뜀·로그/빌드파일 지문·폐쇄를 소유한다. 자식 exit code null/비0 또는 강제종료·집합차이·건너뜀은 전체통과로 수용하지 않는다. 실패 사례가 있어도 나머지 결과를 정해진 전체상한 안에서 수집하되 실패를 생략하지 않는다.

빌드직전 정확4 설정의 before 원문을 보존하고 모든 소유프로세스 종료0 뒤 실제 source-after/설정after·빌드파일귀속을 먼저 폐쇄한 다음 승인4만 정확복원하여 전체630 및 manifest를 대조한다. 승인4외 변화·원문보존/복원 실패는 실패다. raw 변경과 복원0을 구분하고 파생설정의 의미무영향을 선언하지 않는다. 이번 실제 결과는 해당 빌드의 옵션/after설정/바이너리 지문에 귀속하며 최종 Ship의 화면/기능 증거를 자동 대체하지 않는다.

실행 중 모든 제품·시험·자산 writer는 폐쇄하며 Root의 위 승인된 빌드 직렬화 보존/복원 이외 소스수정0이다. 실제 입력은 격리 자동시험으로 표시하고 인간 완주/실장치완주로 확대하지 않는다. 최종 Ship 동일릴리스 실제창 입력·독립검수·안내/패키지는 계속 필수다. 최초 실패와 원시 기록은 보존하고 후속은 실제 원인·영향에 따라 같은 계약에서 한정 판정한다.
#### §8 최종 Ship 및 실제 게임 창 증거 매핑 — Approved 준비

REQ/AC-AUTOCH1-SHIPMAP-001: 현재 Standalone24 실행은 계속 소스동결 상태로 진행한다. 이 절은 후속 Ship/실제 창 검사 준비이며 새 빌드·창입력·시험 예산을 발급하지 않는다. §8.5의 기승인 검사방법 조합을 적용한다. 전체 양분기는 생산소스·자산 지문이 결속된 StandaloneTestplayer 합성 입력으로 검증하고, 시험코드 없는 최종 Ship 자체는 지원 공개 OS tap/클릭으로 아래 실창 항목을 검증한다. 장시간 OS 입력의 전체양분기 완주를 새 암묵 선행으로 만들지 않으며, 시험용 exe와 Ship이 같은 바이너리라고 표시하지 않는다.

| 검증 요구 | 합성/기존 증거 | 최종 Ship 자체의 실제 필수 관측 |
|---|---|---|
| CH1 계획·방·보스·양선택·기술·봉쇄선·종결 | 현재 고정소스 Standalone 정확18와 새 원인 경계, 승인된 G3/G4/G5 영향 증거 | 실제 시작/거점/원정 진입과 화면·정상 입력 경로. OS 전체 완주는 관측한 경우만 별도 기록 |
| 체력·낙하·압착 실패, 동시원인·단회복귀 | 정확 G7 및 보스/R05·순차재Open의 각 실제 assertion | 정상 게임 입력/대기에서 실제 실패1과 거점복귀/재시도·새 원정 시작을 확인. HP/위치/저장 강제설정0 |
| 이동·점프·조준·공격·전이·초점 | 생산 InputSystem/이동/전투 경계와 실제 Standalone 관측 | 검증된 창에서 단일 지원행동 전후 위치/동작/조준·공격/전이 반응 및 초점복귀를 관측. 화면상 확인하지 못한 동작을 통과로 채우지 않음 |
| 저장·재실행·중복 방지 | Persistence2 및 기존 저장/위조/단회회귀 | Ship의 실행파일 부모 SaveData/Chapter1 실제 생성/오류표시·정상 종료/동일exe 재실행·동일 소유 저장 보존. 양선택 완료 저장 주장은 합성 증거로 구분 |
| 배포·표시·의존성 | 빌드보고/생산 입력 지문/조건부 컴파일 대조 | Editor 없이 같은 최종exe 실행, 한글/역할/상태 표시, 시험메뉴/자동운전/개발 seed 버튼 없음, 종료/재시작, 필수동반파일/최종지문 |

빌드 준비에서는 Testplayer와 Ship 각각의 BuildOptions·씬/시험bootstrap·조립체 목록/생산소스·자산·조건부 컴파일·승인설정4의 before/after 지문을 매핑한다. 기존 BuildWindows는 CH1 ScenePath 하나 및 BuildOptions.None을 사용하고 편입catalog.Validate(true)를 먼저 수행한다. Testplayer의 Development/시험조립체/시험bootstrap 및 저장override 차이는 허용된 차이로 명시하고 생산DLL 지문이 다르면 같은 바이트라고 주장하지 않는다. 실제 차이의 근거와 동작 비도달/실제 Ship 재관측을 연결해야 하며 설정4가 렌더에 무영향이라고 선언하지 않는다.

실제 창 절차는 먼저 정확 Ship 경로/PID/출생과 창을 결속하고 최신 화면으로 입력대상을 확인한다. 공개 도구가 제공하는 tap/chord/클릭만 사용하며 지원되지 않는 hold/keydown/up·숨은 OS입력·대기시간을 키 유지로 오인하는 우회0이다. 첫 이동은 단일 tap 전후 관측으로 실제반응을 확인하고, 매번 관측→한 행동→새 관측 순서를 지킨다. 같은 행동의 결과가 안 보이면 무한반복하지 않고 초점/실제표시/키맵을 구분하여 미관측을 남긴다. 현재 유휴 화면에 보안창이 없다는 사실을 이후 입력까지 가정하지 않으며 보안/인증/권한창이 나타나면 그 뒤 게임에 입력하지 않는다. 사용자 동작 요청0이다.

실패/재시도는 기존 게임의 정상 조작과 실제 피해/낙하만 사용한다. 도구의 한 번 누르기만으로 필요한 이동이 성립하지 않으면 해당 OS 증거를 미검증으로 남기고 허용된 대안을 설계하며 성공이나 양분기 완주를 만들어 채우지 않는다. 구체 action·관측횟수·사례기한은 최초 tap 결과와 지원 UI에 근거해 실행 전에 한정한다. 소유 게임 종료/회수와 실제 종료코드를 기록하고 키를 임의로 계속 누르는 루프를 만들지 않는다.

Root는 프로세스/창 대상·지문·실제행동/시간·전후화면·저장파일 지문·종료를 소유한다. Luna는 같은 Ship 지문을 확인하고 독립 검사순서/판정으로 실제 창 흐름과 원시 실패·화면을 검수한다. Root가 프로세스를 소유하는 독립 재실행에서도 Luna의 판단을 구현자의 자기승인으로 대체하지 않는다. 사람 체험 증거는 별도 미관측으로 유지한다.

최종 패키지는 검수한 Ship의 exe/동반파일 지문을 유지하고 한글 시작·조작·저장·재시도·알려진 임시 아트/미검증 안내를 포함한다. OS 검사용 새 SaveData는 소유 원시로 보존하고 사용자에게 시험 진행상태를 기본 저장으로 섞어 배포하지 않는다. 배포복사/압축 때 실행파일을 다시 빌드하거나 바꾸지 않고 검수원본과 최종 파일목록/지문을 대조한다. 압축/복사본의 저장 경로와 첫실행 검증은 최종 실행계획에서 결속하며 기존 증거/저장 삭제0이다. 현재 Ship1 미소비는 그대로다.
#### §8 v46 Standalone24 실제 폐쇄·22통과/2실패 분리

testplayer24-r1은 Editor40664 종료2·823.536381초, 자식20908 종료0·강제종료0·정리오류0·Editor/Player0·로그폐쇄로 회수됐다. Astra는 실제 XML의 정확24 중22 Passed/2 Failed 및 폐쇄 A233F11D444CAACB1814EA0B2469B5F1732ACD78C347DE001C17DC89921C4007의 자식exit0/정확귀속·복원630/원문차이0/manifest차이0을 직접 확인했다. Root의 정확이름집합차이0 및 XML파싱오류0을 결속한다. 보존 Player.log1727133바이트/SHA256 71404E3FD5FCC32977F0CC88C051412CB90484CB3DBDDF3337C52E99EE56C609, 실행색인v25 SHA256 9515E435FE5262A463F88C692E9F2E310ADBC0B396355EEFA50CB2C4F1880D3C를 보존한다.

기존18 전체가 통과했고 Seed303 반복3 및 양분기2를 포함한다. 세원인 순차실패/재Open1·Persistence2·보스 일반실패 Transition1도 통과했다. R05 SingleReturnForConcurrentCauses는 SurveyorHP0 생존조건에서, SameTickFailurePrecedesBossDeath는 마지막 입력 후 생산 Transfer 입력의 정확 미래tick/유일성 오류에서 실패했다. 보스 준비19 고유 BasicAttack Applied3·bossHP3 도달은 실제 관측됐지만 최종 같은tick 양사망과 단회복귀는 미수용이다. 자식exit0이나 준비피해19를 전체시험/동시사망 성공으로 대신하지 않는다.

승인된 설정4의 실제after 원문·빌드귀속을 보존한 뒤 before를 정확복원했으며630/134핀 불일치0이다. 이22통과는 해당 after설정 및 Testplayer 바이너리에 귀속한다. 최종 Ship을 같은 바이너리로 표시하지 않으며 후속 최소보정 뒤22의 영향 재사용은 정확 변경과 독립 판정으로 정한다. 전체24 무변경 반복0이다.

추가 Testplayer 빌드1/시험1은 소비 완료다. Ship1은 미소비로 유지한다. 두 Sol의 실패 원인 읽기와 최소변경/새관측 설계만 진행하며 현재 제품쓰기0이다. 후속 정확2 native 후보는 최소보정·독립영향·최종핀/선택/추가빌드 유한예산이 결속되기 전 실행0이다. 원래 실패/소진 원시는 수정하지 않는다.
#### §8 R05 최초 완료 원문의 실제 방 진입 결속 — Approved 구현

R05 실패의 firstTick716/generation2/LeavingRoom/PointerIssued0은 실제 R05가 아니라 이전 R02의 마지막 완료다. seed202의 실제R05는 다음generation3이며 Flow는 LeavingRoom 전환 때 roomIndex/표면 RoomId를 먼저 갱신한다. 따라서 이번 SurveyorHP0은 R05 pilot이 측량병을 죽였다는 근거가 아니다. 최초 관측대상 결속 오류로 정정하고 원시 실패는 보존한다.

Root의 현재 실행폐쇄·source630/134핀 복원·쓰기0 확인 및 명시 소유 반환 뒤 지정 R05 Sol은 Expedition의 PlayUntilConcurrentFailure 한 helper만 보정한다. first 결속 전 정상 InRoom이 아닌 전이는 StartingRoom/LeavingRoom으로 한정하고 각 전이묶음 진입에 기존 보유 keyboard/mouse를 확인한 Keyboard()/Pointer(false,false) 해제1회를 발행한다. 이후 해당 전이의 fixed 양보에서는 PlayInput/추가해제 발행0이다. 실패/Diagnostic/예상 밖 phase는 즉시 실패로 보존하며 전이대기로 숨기지 않는다. 누적 전이대기는 두 방 교체를 합쳐180fixed 미만이며 재설정0, 기존 전체 실시간 상한을 늘리지 않는다.

InRoom의 실제 parent Chapter1RoomInstance.RoomId가R05이고 Flow/owner/router/completed의 run·generation·tick·원본receipt가 서로 일치할 때만 first를 단1회 고정한다. 기존 비생성 읽기와 Combat.Chapter1ConfiguredCarryTick을 사용해 그 첫 완료가 실제 진입 원본tick과 같음을 요구하며 늦은 완료로 기준을 재설정하지 않는다. InRoom 비R05에서는 기존 정상pilot을 유지한다. 새API/생성getter/상태값주입0이다.

R05 실제 진입 뒤의 carry/적 생존·정상처치·피해/위상/대시/동시원인·기한/로그/assertion 및 보스/Closing/생산은 그대로다. 국소1묶음·같은원인보정최대2묶음 및 설치Csc로 전이중 입력1회·원본세대/첫tick·helper밖 불변을 대조하고 핀을 반환한다. 실제 원정 성공은 미관측이며 Unity/추가빌드0이다. 이 writer가 폐쇄된 뒤에만 다음 보스 생산 보정 writer에게 소유를 넘긴다.
#### §8 보스 terminal 제거예약 뒤 RunFailed 결합 — Approved 구현

REQ/AC-AUTOCH1-BOSSTERMINALMERGE-001: native24에서 실제19 준비공격 뒤 마지막 원본3188의 hostile3189 예약과 최종 입력이 있었으나3189 양사망 처리 중 terminal scheduler가3190 정확4 제거를 먼저 예약한 뒤 실패 requester가 같은3190에 일반 Submit을 호출하여 중복tick guard가 거절했다. 이 순서 충돌을 보정하며 일반 Submit의 미래tick/유일성 guard를 완화하지 않는다. 성공/오류 기대값으로 생산 예외를 숨기지 않는다.

Root의 v47 R05 writer폐쇄·manifest134핀/55663바이트/SHA256 514440981B1C5780F2F5A8AC5B965BD92DB810B36A6DC830BE9DA001ADE436EA·전체핀일치/Unity0 확인 및 명시 소유 반환 뒤 지정 주Sol은 현재 project/Assets/AcadeGameMaker 아래 정확3파일만 쓴다: Runtime/Transfer/Unity/TransferSimulationDriver.cs, Runtime/Input/Unity/OrdanBossTerminalTransitionRequester.cs, Tests/PlayMode/CombatUnity/OrdanBossTerminalTeardownPlayModeTests.cs. Expedition/R05/보스 입력helper 및 다른 생산은 변경0이다.

Transfer에는 internal MergeExactBossTerminalRunFailed(owner,tick) 한정 입구를 추가한다. 등록 inputGateOwner와 같은 router 참조·기존초기화/플레이어·현재완료 Transfer publication·미소비 bossTerminalCleanupTick·아직발행되지 않은 cleanup receipt·해당 큐항목의 정확4 제거/원래순서·동일tick·기존Lifecycle없음을 모두 검증한다. 호출순서는 Transfer-200의t발행→Combat-190/terminal scheduler의t+1예약→Movement0의t완료→Lifetime200/Flow210 요청이므로 latestTransfer.Tick+1=cleanupTick=player.NextExpectedTick가 정확 조건이다. 기존 scheduler의 엄격히 이후 tick 조건을 이 입구에 그대로 복사하지 않는다.

검증·불변 replacement 준비·checked generation 계산을 모든 대입 전에 수행한 뒤 큐 한 항목과 generation만 원자적으로 교체한다. 기존 Camera/Aim/Press/ExposureEnds/ChoiceEdgeOrdinal 및 정확4 제거를 보존하고 Lifecycle만 RunFailed로 추가한다. 다음 잠금 frame과 원래 lifecycle 우선소비가 상호작용을 차단하며 별도 즉시삭제 정책을 만들지 않는다. 외부owner·다른tick·없거나잘못된예약·기존Lifecycle·중복요청·이미소비/완료는 거절하고 큐/후보현재성을 바꾸지 않는다. 범용 병합·새publicAPI/friend·기존Submit중복허용0이다.

Requester는 이미 검증된 accepted token과 같은 원본의 양쪽사망 publication이 있을 때만 등록router를 위 한정입구에 전달한다. 보스 비사망 health 실패는 원래 Submit 경로를 유지한다. _chapterFailure/_deathTick/_requestedTick/_requestIssued는 큐결합 또는 Submit 및 RequestMode가 성공한 뒤 기록한다. RequestMode 이후 모든 외부실패를 되돌리는 범용 transaction/rollback을 만들지 않고 기존 fail-closed를 유지한다. 원래 durable=false 우선정리 receipt 및 failure source 동일성 검증을 보존한다.

기존 TerminalTeardown 시험파일/fixture에서 신규 정확10을 구현한다. BossFailureMergesAfterTerminalReservation(bool activePayload)의 false/true2는 제거예약 먼저→RunFailed 나중의 비활성/활성 payload 양성을 검사한다. BossFailureMergeRejectsInvalidEnvelopeWithoutMutation(string invalidCase)의 owner/pastTick/futureTick/noReservation/wrongFour/existingLifecycle/duplicate/consumed8은 거절과 비변경을 검사한다. 기존 PrepareInputFrame 뒤 ValidateInputFrameCandidate의 현재성, 이어 실제 phase.Input 및 cleanup receipt로 큐/세대 의미를 검증한다. 정상 merge는 이전candidate 무효와 새candidate 현재성의 양성대조를 포함한다. 새reader/반사 추가0이다. wrongFour가 일반 불완전제거로 terminal예약 자체가 없는 경우에는 무예약 거절 증거로 정확히 한정하며, 도달하지 않은 내부 guard 분기까지 실행했다고 주장하지 않는다.

국소1묶음·같은원인보정최대2묶음과 설치Csc에서 기존 Submit/반대순서/정상보스정리·실패정리 경계 및 정확3변경을 대조해 최종핀을 반환한다. 기존 opposite-order·유일성 검사를 삭제하거나 약화하지 않는다. Luna 독립영향 검수 뒤 신규10과 필수 기존영향 사례의 실제선택을 별도로 정하며 현재 Unity/추가빌드예산0이다. G5/기존22 재사용은 해당 생산 변경의 도달경로 영향으로 판정하고 동시사망 native 실제수용은 후속 정확관측 전0이다.
#### §8 Standalone22 귀속·G4 별칭 및 v47 독립검수 보강

Luna는 실행색인v25/폐쇄/Player.log 지문과355개 빌드파일 전수를 확인하고 정확24 중22통과/2실패/건너뜀0·자식exit0·전체630 정확복원을 검수했다. 22통과는 실제 after설정4와 그355파일에 귀속하며 Ship 수용을 선취하지 않는다. G4 AuthoredHubThroughFourRoomsToOrdan 및 FourRoomsThenBossWithCarriedHealth는 RunNormalCase(101,0)의 본문/인자/assertion과 같아 이번 G7_Normal101 실제통과에 영향 재사용할 수 있다. 이에 따라 G4의 남은 직접 실제검사는 R05 동시원인과 보스 동시사망2다. 별칭을 별도 새 실행통과로 중복 집계하지 않는다.

v47 R05 gate의 정확 국소변경·Csc/원문에 대한 Luna 독립검수는 새P0/P1 없음이다. 실제 R05 진입/동시원인 성공은 미관측으로 유지한다. v47 gate는 기존22를 직접 바꾸지 않으나 이후 승인된 보스 생산2 수정의 영향은 최종 변경분에서 다시 판정한다.

신규 TerminalTeardown10은 실제 includePlatforms=[Editor]인 CombatUnity 시험조립체에 속한다. 문자열8 사례의 이름 따옴표는 기존 focused v3의 정규식 문자코드 처리로 보존하여 EditMode에서 관측한다. 이10을 Windows 선택에 넣거나 asmdef를 넓히지 않는다. 후속 native 직접2는 무인자 Death/R05 사례이며 이 문자열 경계 때문에 Windows runner를 추가 변경하지 않는다. 실행선택/기한은 최종핀 뒤 별도 결속한다.
#### §8 보스 terminal merge 시험의 실제 관측 범위 정정

BOSSTERMINALMERGE-001 구현 중 설치Csc가 확인한 기존 LatestPhasePublication.Input은 전체 TransferSimulationInput이 아니라 HadAim/HadPress/RemovalCount/LifecycleReason 등의 요약이다. 앞 절의 실제 phase.Input 필드 대조는 이 지원된 요약에 한정한다. 실제 관측은 이 요약, LatestCompletedInputRemovalIds의 정확4, cleanup receipt 및 기존 PrepareInputFrame/ValidateInputFrameCandidate의 현재성으로 큐소비/세대 의미를 검증한다.

Camera/Aim/Press/ExposureEnds/ChoiceEdgeOrdinal의 원래 값 보존은 생산 replacement 코드의 정적 원문 대조로 별도 기록한다. 시험에서 읽지 못한 전체 값이 실제 동일했다고 관측 범위를 확대하지 않는다. 새로운 생산reader/반사·관측 API를 만들지 않으며 동일 원인 국소 보정 예산 안에서 시험 표현을 바로잡고 설치Csc를 다시 확인한다. 생산 guard/정확한정 merge·실패 거절·원자성 조건과 실제 후속검사는 그대로다. 컴파일 실패 원문은 보존하고 전체 통과를 선취하지 않는다.
#### §8 v48 terminal 경계10 및 조건부 Standalone 동시원인2 — Approved 실행

REQ/AC-AUTOCH1-BOSSTERMINALMERGE-001 및 R05COINCIDENT-001의 실제 후속관측을 다음 유한 직렬 묶음으로 승인한다. v48 manifest135핀/57805바이트/SHA256 DD0835FD0A3439729004264A7EAD61BFA63F665C0AB6411127D9041BEF92C4D6를 결속한다. Sol 정확 생산2+기존시험1 쓰기폐쇄·Csc38824/29944/1208 종료0와 회수, 초기42124 오류 보존, Root135핀 전수일치 및 Luna 최종 미해결 P0/P1 없음 판정을 확인한다. Expedition v47은 불변이다. 국소 조건모형14는 실제 큐 관측이 아니며 신규10의 성공을 선취하지 않는다.

첫 단계는 terminal-merge-boundary-root-selection.v1.json 2102바이트/SHA256 73F5CE38FAA547A476054799B606238C5EA11C382BC657CC558E8340F9B76EDF의 정확10을 EditMode/ownedBatchFixture=true로 단1회, 외부1200초 안에서 실행한다. 기존 run-focused-owned.v3.ps1 SHA256 1C4DB56F5CD483C78682AC1CCEBD8C4F4E14053BCA0918ED688E7D5AB8948B57을 사용한다. 정상 false/true2와 invalidCase 문자열8의 실제 이름/개수/결과/건너뜀을 대조하고 임의 이름·누락·중복을 허용하지 않는다. 실제 관측은 지원된 publication 요약·정확 제거ID·cleanup receipt·candidate 현재성에 한정하고 전체 입력값 복사는 정적 대조로 구분한다.

정확10이 모두 실제 통과하고 건너뜀/선택차이0, 소유 프로세스 종료·회수·로그/XML 폐쇄, source630 및 v48핀 차이0이 확인된 경우에만 두 번째 단계가 자동으로 유효해진다. 이 조건을 Root가 원시로 확인하면 별도의 단계별 승인 대기 없이 진행한다. 첫 단계 실패/미폐쇄/소스변경이면 두 번째 단계는 실행하지 않고 원인과 최소 후속범위를 판정한다. 자동 재실행이나 실행 중 기한연장0이다.

두 번째 단계는 testplayer-coincident2-root-selection.v1.json 1052바이트/SHA256 F2146A6259258F5716004533018C624F591B18313EFD6DE1B660C6AA74DC10A7의 SingleReturnForConcurrentCauses 및 SameTickFailurePrecedesBossDeath 정확2다. 기존에 소비한 시험빌드들과 별도로 추가 StandaloneWindows64 Testplayer 빌드1회/1800초 및 그 산출물 시험1회/자식 실제출생 기준1200초를 승인한다. runId는 testplayer-coincident2-r1이며 출력은 해당 이름의 신규 evidence/build 경로를 사용한다. 예상400~600초는 계획이며 완료보장이 아니다. helper의 기존8000/6000완료틱 미만·1800실시간초와 외부1200초 중 먼저 도달하는 상한을 유지한다. Ship 빌드1회/1800초는 미소비로 보존한다.

Windows 실행조정은 run-windows-owned.v7.ps1 SHA256 57ACDC2B4E16071F5BF0B573E5EF031A8E6673214C39FB3C5A94D58F5F14C66C를 그대로 사용한다. 정확24에만 적용하는1800초 시험 예외를 이2에 적용하지 않는다. Root만 부모/자식 정확경로·부모PID·PID/출생·SafeHandle·종료코드·자연/강제종료·기한·회수·XML 정확2 및 원시 폐쇄를 소유한다. 자식exit null/비0·강제종료·건너뜀·집합차이는 전체통과가 아니다.

빌드 전 승인설정4의 정확 before 원문을 보존하고 모든 소유 프로세스 종료 뒤 실제 after설정/전체 source-after·빌드파일 지문을 먼저 보존한다. 승인4외 변화는 실패이며 승인4만 정확복원해630 및135핀을 다시 대조한다. 실제 after설정에 귀속한 시험 결과와 복원소스 상태를 구분하고 설정이 무영향이라고 선언하지 않는다. 모든 단계에서 다른 제품·시험·자산 writer는 폐쇄한다. Root의 승인된 빌드 직렬화 보존/복원 이외 소스수정0이다.

Luna의 v48 독립 영향판정에 따라 기존 Standalone22는 새 양사망 결합 분기에 도달하지 않고, 기존 G5 고정44의673 사례도 기본 경로/기존 일반입력 및 병합 경계의 의미가 보존되어 각 원래 실행·바이너리·설정 지문에 귀속한 증거로 재사용한다. 새10이나 직접 동시원인2의 미실행 결과를 합산하지 않으며 동시에 전체 통과한 것으로 표시하지 않는다. 실제 두 동시원인·실패우선·단회복귀 성공은 새 원시 assertion으로만 수용한다. 기존 실패 원시는 보존하고 최종 Ship 및 같은 릴리스 실제 창 검사·독립검수·안내/패키지는 계속 필수다.
#### §8 v48 실제 경계12 폐쇄 및 최종 Ship·실제 창 한정 검사 — Approved 조건부 실행

Root가 보고한 terminal 경계10은 실제10통과/건너뜀0/집합차이0·39.45초·종료0/회수·Editor0/source630불변이며 폐쇄 SHA256 5744D1DB284EB91436349C6FE69C0208C2A6FF3D8DB3947E4483A32DA41D7AA9다. 이어 조건부 native2는 실제2통과, Editor9296 종료0/163.4939421초·자식28036 종료0·강제종료/정리오류/집합차이/XML오류0·로그폐쇄 및 승인설정4 보존 뒤630/135핀 정확복원으로 종료됐다. 폐쇄 SHA256 19A15000202A742E4D8222055AAEF3C47C86A84C2E26850CE5B43AD6D31FA4BC 및 실행색인v27 SHA256 D115A0B95A535BED2AB939986692A4F1C47F8B7B05D1CC69DDEBD0A8DC7EE4E9를 결속한다. R05는 실제4559의 같은receipt health/crush·line Applied1/HP0·단회복귀, 보스는3189 양Applied2/양HP0 및 RunFailed 처리가 통과했다. 이는 Root 실제보고이며 Luna 원시 독립검수 완료 전 최종 전달 수용을 선취하지 않는다. 과거 실패는 보존한다.

Luna가 위 실제12의 선택·결과·귀속·종료·복원을 독립 확인하여 미해결 P0/P1이 없고 Root가 현재 v48의135핀/source630 및 Unity/Player0을 재확인하면, 미소비 Ship 빌드1회/1800초를 실행할 수 있다. ship-root-selection.v1.json 590바이트/SHA256 9410F86E34C7F3BA40C7FBCA74CEB267AB635C6E07E71F18F02515D657F380D0를 사용하며 manifest DD0835FD0A3439729004264A7EAD61BFA63F665C0AB6411127D9041BEF92C4D6, BuildOptions.None, Assets/Scenes/Chapter1Playable.unity에 결속한다. 기존 v7 실행조정과 BuildWindows 공개입구를 유지한다. 선택의 승인 전 문구는 작성시점 기록이며 이 절이 실행권위다. 자동 재빌드0이다.

Root는 실제 빌드 출력의 유일한 성공경로/보고·모든 배포파일 지문을 결속하고 승인설정4 before/after·옵션·생산소스/자산 대응을 보존한다. 소유 프로세스 종료 뒤 승인4만 정확복원하여 source630/135핀을 대조한다. 승인4외 변화·빌드실패·귀속불명·복원오류면 창검사 단계는 진행하지 않는다. Testplayer의 시험조립체/자동시험 bootstrap과 Ship 차이를 명시하며 서로 같은 실행파일이라고 표시하지 않는다.

빌드와 복원 폐쇄가 성공하면 같은 Ship의 첫 실행·게임 창 확인·시작·이동 단1tap 반응 검사를 최대180초/게임 입력6회 안에서 진행한다. 실행경로/PID/출생과 창을 먼저 결속하고 매 입력 직전의 최신 화면/지원 접근성 관측을 근거로 한 행동만 수행한 뒤 새 관측으로 실제 반응을 확인한다. 초기 화면이 예상과 다르면 추측 좌표 입력을 하지 않는다. 공개 도구의 tap/chord/클릭만 허용하며 보안·인증·권한창 처리0, 지원하지 않는 hold/keydown/up 또는 숨은 입력 우회0이다. 입력6회는 성공을 위한 강제반복 횟수가 아니다.

첫 관측으로 정상 게임 창·초점·지원 입력의 실제 반응이 확인되면 Root는 관측한 키맵/화면 상태에 근거한 후속 행동 순서를 원시에 기록하고 추가 승인 대기 없이 같은 묶음의 Shipmap 검사를 계속할 수 있다. 전체 실제 창 묶음은 최초 실행부터 누적1200초/게임 입력120회/동일exe 실행 최대3회 이내이며 앞선180초/6회도 이 전체에 포함한다. 각 입력은 개별 관측에 근거하고 다음 입력 전 새 관측을 요구한다. 동일 무반응 행동은 확인 목적 최대1회만 추가할 수 있으며 계속 무반응이면 그 항목을 미관측으로 남기고 다른 확인 가능한 항목으로 이동한다. 횟수/시간을 늘리거나 반복 루프·자동 플레이를 추가하지 않는다.

후속 목적은 §8 SHIPMAP-001의 실제 시작/이동·점프·조준·공격·전이 반응/초점복귀/한글 및 상태표시/정상 게임 실패1·거점복귀·재시도/정상 종료·동일exe 재실행과 소유 SaveData 보존이다. 실제 게임 상태로 도달한 항목만 관측하며 HP/위치/저장값 주입0이다. 장시간 OS 양분기 완주를 새 선행으로 만들지 않고 전체 양분기 합성 증거와 Ship 실창 증거를 구분한다. 도달하지 못한 필수 실창 항목은 완료로 채우지 않고 관측한 제약과 허용 대안을 같은 계약에서 정한다. 사용자 중간 조작 요청0이다.

Root는 실행/입력/시간·전후화면·소유 저장 지문·정상/강제 종료와 회수를 기록한다. 게임 입력과 별도로 소유 프로세스 안전 종료/증거 보존은 허용하며 강제 종료를 정상 종료 성공으로 표시하지 않는다. Luna는 같은 Ship 파일 지문에 귀속한 독립 검사순서와 원시 화면/행동/저장 결과를 검수한다. 실제12 검수·빌드·첫 반응·후속 검사 각 조건이 충족될 때의 계속 권위를 이 절에서 함께 발급하며 버튼마다 새 승인을 요구하지 않는다. 최종 안내·배포 패키지와 사람 체험 미관측 구분은 유지하고 최종 수용은 실제 필수항목 및 독립검수 뒤에만 닫는다.
#### §8 Ship 최초 창 접근 시간초과와 승인 경계 보존 — Approved 한정 복구

Root 보고의 ship-r1은 Editor46408 종료0/46.9358초·205파일·BuildOptions.None·오류0/경고4이며 설정4 복원 뒤630/135핀 차이0과 프로세스0으로 폐쇄됐다. 실행색인v28 SHA256 5FEA6E778CC2A6D4B496B5D5B2B15B68584EDB403D8E5CD253ACBEEB8A7F2767, 폐쇄 F98395234F5CE781B0EB8E630B96EB19C06EFA6DB6A45E0F695E48E05779C6F9, 실제 Ship exe SHA256 A4C71B1BFC3E42D02F2F129FE04ADEF3EBF2343A662884E4B99598E9DFC90E7D를 기록한다. Ship 빌드1은 소비 완료이며 새빌드 권위0이다.

첫 OS 실행2320은 엔진 시작·Player.log 예외0 및 창목록의 정확1창만 관측됐다. 화면 접근 중 Computer Use app approval timed out가 발생하여 실제 화면/입력은0이고180초 소유 감시가 프로세스를 강제종료·회수했다. 이 결과는 정상 종료나 화면/조작 통과가 아니다. 최초180초와 실행1회는 기존 전체1200초/최대3실행에 포함하며 남은 한도를 다시 시작하지 않는다.

Windows Computer Use 기술 지침의 복구는 연결/창 결속 복구에 한정하며 앱 접근 승인을 대체하지 않는다. Root는 기존 오류/원시를 보존하고 공개 JavaScript 세션 초기화1회 및 문서의 Initialize와 경량 list_apps 또는 list_windows 재조회1회로 연결 상태를 확인할 수 있다. 이 동작은 앱 권한 승인·보안창 처리·권한 캐시 변경을 포함하지 않는다. 승인 시간초과를 일반 창 핸들 오류로 바꾸어 해석하지 않는다.

기존 지원 인터페이스에서 대상 앱 접근 권한이 이미 유효하다는 명확한 결과를 얻은 경우에만 새 실제창 결속·관측을 기존 잔여 예산 안에서 재개한다. 단순 창목록 존재는 접근 권한의 증거가 아니다. 승인 대기/거절/동일 시간초과가 남거나 유효 여부를 확인할 수 없으면 보호된 get_window/state·입력 호출을 반복하거나 다른 화면제어 도구/직접 OS API로 우회하지 않는다. 사용자 승인 요청이나 승인창 대리 처리는 하지 않으며 차단된 실창 항목은 미검증으로 남긴다.

충돌 없는 후속 작업은 같은 Ship의 파일목록/지문·빌드보고·생산소스/자산/옵션/설정 귀속 대조, 기존 정상 시작 로그와 실제 생성된 소유 저장파일의 읽기 검증, 한글 조작·저장·재시도 안내 및 배포 후보 복사/압축 준비, Luna 독립 파일/증거 검수다. 정상 게임 입력이 필요한 실패/재시도·화면/초점·동작 항목은 이 비화면 증거로 대체하지 않는다. 새 진단 코드/자동입력/저장편집·재빌드로 우회하지 않는다. 패키지 후보는 기존 검수원본을 보존하고 시험 SaveData를 기본 배포 상태에 섞지 않으며 실제 창 미검증을 명시한다. 최종 필수 실창 관문의 통과나 전체 전달 완료를 선취하지 않는다.
#### §8 최종 파일 전달 통합 기록 — 파일 수용·실제 창 검증 미완료

Root와 Luna의 최종 파일 검수에 따라 output/chapter1-autonomous-20261021/AcadeGameMaker-CH1-Windows.zip을 전달 가능한 파일 패키지로 수용한다. Astra도 실제 ZIP42378708바이트/SHA256 82ACE4A9BACFF1385E2951CC0275C9854EF0FD54DDFFC858528B7C5F97ABFB8E를 직접 확인했다. 원본 Ship205파일과 한글안내·PACKAGE-SHA256SUMS를 합친207파일이며 기본 배포 SaveData는0이다. 실행파일은 output/chapter1-autonomous-20261021/build/windows/AcadeGameMakerChapter1.exe다. 시험용 PlayerWithTests.exe를 배포 실행파일로 안내하지 않는다.

package-files.v1 SHA256 8B441D4822432E4EE31DC720E83882B77D9BA7606456203FC25C76778EE65AA0 및 zip-verification.v1 SHA256 A76C322E371AFC2C91A7781C1C5071F5D53484E9784C8621DB65F137C06C9D72를 결속한다. Luna는 ZIP207파일의 이름·길이·SHA를 스트림으로 전수 독립 대조하여 차이0을 확인했다. source-binary-map.v1 SHA256 7614D91524284475E44008CF5295A808F624CCDAF778CCE3C4AE04425F587DA6의 조건부 동작2·DLL15차이 및 UnityCollections 보조4 기본참조 보존 검수를 기록한다. 서로 다른 시험/배포 바이너리를 같은 바이트로 표현하지 않는다.

실제 창 접근 시간초과 원시는 access-observation SHA256 BB376A2404F20E3FAD19EC554DDED1AA4042ECEBC55F601691E6DA3C60ED0756에 보존됐다. 허용된 세션초기화1·Initialize1·창목록1 이후 게임창0이며 유효 앱 접근 승인 확인값이 없어 보호된 추가 관측/입력 호출을 하지 않았다. 실제 입력0·화면캡처0·추가 실행0·추가 빌드0·사용자 승인/조작 요청0이다. 최초 게임 시작 로그 예외0 및 생성된 profile/lock 읽기는 기록하되 화면·정상조작·정상종료·실패재시도 통과로 확대하지 않는다.

현재 상태는 파일 패키지 전달 가능, 자동 시험의 승인된 분리 증거 수용, 실제 Ship 창 조작 필수검증 미완료다. 전체 게임의 최종 수용은 false로 유지한다. 공개 도구의 앱 접근 승인 유효성을 확인할 수 없는 외부 차단을 다른 도구·직접OS·권한 변경으로 우회하지 않는다. 사용자 반복 참여를 요구하지 않으면서 가능한 비화면 작업은 완료했으며, 최종 안내에는 실행파일/ZIP과 실제 창 입력 미검증을 함께 명시한다. 정상 플레이 최종검증 완료·사람 완주·전체 목적 달성을 선언하지 않는다. 기존 원시 실패·시험저장·빌드·검수 지문은 보존한다.

#### §8 정상 부팅과 자연 입력 갱신의 격리 진단 — Approved 구현·조건부 실행, 2026-10-24

REQ-AUTOCH1-BOOTINPUT-001 / AC-AUTOCH1-BOOTINPUT-001: Astra는 Sol의 읽기 설계와 Root의 실제 조립체 참조 대조를 근거로 정상 첫 장면 부팅부터 최초 게임 입력까지의 미입증 범위를 승인한다. 기존 원정 시험은 InputTestFixture·시험 저장 경로·가상 장치와 장면 적재를 준비하고 입력 helper에서 InputSystem.Update를 직접 호출한다. 이번 관측 목적은 시험 프레임워크 없는 Windows 플레이어의 자연 Dynamic 갱신이 실제 거점 준비·중립 입력·새 시작 입력·원정 이동과 해제를 연결하는지 확인하는 것이다. 기존 자동 통과를 취소하거나 동일 시험을 반복하는 범위가 아니다.

앞선 접근 실패 절의 새 진단 코드·자동입력·재빌드 금지는 기존 Ship과 차단된 OS 화면 접근의 우회에 계속 적용한다. 이 절은 별도 복제본 내부 공개 Unity API를 이용하는 아래 정확 진단에만 한정 예외를 부여한다. 보안·권한 설정/캐시 변경, 승인창 처리, 보호된 창 관측 반복, 다른 화면제어 도구·직접 OS 입력/화면 API, 사용자 중간 조작 요청은 모두0이다. actualWindowAcceptance=false와 finalAcceptance=false 및 기존 Ship·ZIP·실패 원시는 보존한다. 이 진단을 실제 창 입력·초점·화면·사람 완주나 현재 Ship 바이트의 실행 증거로 표시하지 않는다.

Root는 output/chapter1-playable-r1/project의 ship-r1/source-before에 결속된630파일을 output/chapter1-boot-input-probe-20261024/project로 복제하고 변경 전 이름·길이·SHA256 전수를 대조한다. 원본630·135핀·기존 Ship 배포파일과 ZIP은 변경하지 않는다. 복제본의 기존 실행/편집기 조립체를 사용하며 허용 소스 차이는 정확 다음5파일이다. 신규4로 복제본 소스 목록은634가 된다.

- 신규 Assets/AcadeGameMaker/Runtime/Run/Unity/Chapter1BootInputProbe.cs 및 해당 .meta.
- 기존 Assets/AcadeGameMaker/Runtime/Run/Unity/AcadeGameMaker.Run.Unity.asmdef의 references에 Unity.InputSystem 참조1 추가. 다른 조립체 설정 변경0.
- 신규 Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BootInputProbeBuild.cs 및 해당 .meta.

Sol은 위5파일의 단일 구현 소유자다. Root는 preparation/autonomous-ch1-20261021/run-boot-input-probe-owned.v1.ps1 한 실행조정 파일, 복제·증거·프로세스 수명을 소유한다. 출력은 output/chapter1-boot-input-probe-20261024/build/windows/AcadeGameMakerChapter1BootProbe.exe, 원시는 같은 상위 경로의 evidence/boot-input-probe.raw.json 및 빌드/실행 로그·파일 원장으로 한정한다. 기존 경로가 이미 존재하면 덮어쓰지 않고 원인을 보고한다. 실행 중 모든 소스 writer는 폐쇄한다.

진단 코드는 전체 CH1_BOOT_PROBE 조건부 구획 안에 두고 이번 전용 빌드에서만 해당 기호를 추가한다. 공개 실행입구는 AcadeGameMaker.Combat.Authoring.Chapter1BootInputProbeBuild.BuildWindows이며 BuildOptions.None, StandaloneWindows64, Assets/Scenes/Chapter1Playable.unity 단독 첫 장면을 사용한다. UNITY_INCLUDE_TESTS·시험 조립체·InputTestFixture·ConfigureChapter1TestRun을 사용하지 않는다. 원래 생산 코드·장면·저장 규약·일반 빌드입구는 수정하지 않는다. 시험 저장은 새 실행파일 부모의 SaveData/Chapter1에 정상 생산 코드로 자연 생성하며 기존 사용자/개발/Ship 저장을 복사·편집·덮어쓰지 않는다.

RuntimeInitializeOnLoadMethod로 관찰자만 시작하고 실제 생산 Awake/Start와 첫 장면 초기화를 기다린다. 장면 교체·생산 host 재생성·반사·비공개 API·단계/위치/체력/저장값 주입0이다. 공개 InputSystem.AddDevice로 소유 Keyboard/Mouse를 만들고 QueueStateEvent로 입력을 공급한다. 수동 InputSystem.Update, InputSystem 설정·timeScale·runInBackground 변경0이며 자연 갱신의 실제 update 종류와 진행을 기록한다. 소유 가상 장치 이외 운영체제 장치 제어0이다. 기존 읽기용 공개/친구 접근으로 실제 준비 영수증과 완료 상태를 관측할 수 있으나 생산 상태를 고치지 않는다. 기존 API로 확인할 수 없는 항목은 미관측 또는 실패로 남긴다.

수용 순서는 자연 Dynamic 갱신 확인 → 실제 거점 준비와 중립 입력 완료 → 새 Enter edge와 해제 → RunStartedRevision=1·InRoom·새 원정 키·실제 생성 저장 확인 → D 입력 이후 완료 위치 증가 → D 해제 이후 실제 입력 중립/해제 완료 관측이다. 입력 요청과 소비 완료를 구분하고 단계별 실제 tick/상태/영수증을 기록한다. 같은 Enter의 반복으로 성공을 만들거나 위치 증가만으로 입력 해제를 추정하지 않는다. 전체 기록은 최대256행·256KiB이며 상한/누락/첫 예외를 명시하고 종료 요약 공간을 예약한다. 성공·실패 모두 원시를 폐쇄하고 소유 장치를 정리한 뒤 공개 Application.Quit로 정상 종료를 시도한다. 강제종료는 성공이 아니다.

구현·국소 컴파일 확인1묶음과 원인이 확인된 국소 보정 최대2묶음을 허용한다. Sol의 쓰기 폐쇄·정확5차이·금지 API/시험 기호 부재·Root의 복제 원장 확인 및 Luna의 실행 차단 결함 없음 판정이 충족되면 추가 사용자 승인이나 단계별 문서 발급 없이 Root가 빌드1회/1800초와 성공 산출물 실행1회/출생 기준180초를 직렬 수행할 수 있다. 자동 재빌드·실행 재시도·기한 연장0이다. 빌드 실패/기한초과/귀속불명은 실행하지 않으며 실제 실행 실패 뒤에는 원시와 최소 원인을 먼저 판정한다. 기존 실창1200초/3실행의 잔여 예산을 재사용하거나 복원하지 않는다.

빌드 시 CH1_BOOT_PROBE 기호는 공개 빌드 API의 해당 실행 범위로 전달한다. Unity가 직렬화할 수 있는 기존 승인설정4(Assets/Settings/UniversalRenderPipelineGlobalSettings.asset, Assets/Settings/UniversalRP.asset, ProjectSettings/GraphicsSettings.asset, ProjectSettings/ProjectSettings.asset)는 복제본에서만 before/after 원문과 지문을 보존한다. 승인한 조립체 참조1과 설정4를 제외한 기존625파일 및 원본 제품·Ship은 불변이어야 하며, 승인5 차이 및 설정4 외 소스 변화는 실패다. 복제본 설정4도 실제 빌드귀속을 남긴 뒤 소유 프로세스 종료 후 before로 복원하고634목록을 재대조한다. Library/Temp/Logs 등 Unity 생성 캐시는 제품630 원장과 구분한다. 설정 차이의 무영향이나 원본 Ship과 같은 바이너리임을 주장하지 않는다.

Root는 정확 명령·PID/출생·실행경로·부모/자식 소유·단조기한·종료코드·정상/강제종료·회수·로그 폐쇄·최종 소스/빌드파일/저장 지문을 기록한다. Luna는 구현과 분리하여 자연 갱신·정상 초기화·실제 입력 소비·원시 assertion·저장 귀속·종료와 기존 Ship 불변을 검수한다. 이 절의 종료 조건은 AC-AUTOCH1-BOOTINPUT-001의 한정 결과와 미관측을 독립 확인하는 것이며 OS 실창 수용을 대체하지 않는다. 실제 실행 입력과 최종 변경분만 결속하고 문서 지문의 재귀 선행이나 작은 보정마다 새 승인 사본을 만들지 않는다.

#### §8 부팅 진단 최초 실행조정 실패와 추가 빌드1 — Approved 한정 보정

REQ/AC-AUTOCH1-BOOTINPUT-001 후속: Astra는 evidence/closure.json 및 editor-exited.json과 실행조정 원문을 읽어 첫 빌드 시 자식37148의 GetProcessById 이미종료 예외가 CIM의 일시 잔존 항목 때문에 재던져지고 Editor47200이114.08초에 강제중단된 사실을 확인했다. 빌드 완료0·게임 실행0이며 제품 오류를 확정하지 않는다. 원래 빌드1은 실패 소비로 보존하고 해당 원시·로그·미완성 산출물을 덮어쓰지 않는다. Root의 원본630 차이0·Ship 지문 불변·관측 프로세스 회수·핸들 폐쇄 보고와 실제 closure의 복원오류0을 결속한다.

Root는 run-boot-input-probe-owned.v1.ps1에서 이미 종료된 자식의 발견 경쟁상태만 원시기록 후 관측 불가로 분류할 수 있다. CIM에 같은 PID가 남았다는 이유만으로 살아 있다고 단정하지 않는다. 접근거절·출생/경로 불일치·확인되지 않은 신원을 이미종료로 숨기지 않으며 PID 재사용은 새 출생 신원으로 구분한다. 실제 확보한 프로세스 핸들과 출생에만 종료권위를 결속하고 다른 재사용 PID를 종료하지 않는다. 자식 발견·회수 오류와 강제정리·미폐쇄는 계속 실패다.

보조 UnityShaderCompiler/Bee 등의 종료코드 비0은 원문 그대로 남기되 그 값만으로 전체 빌드 실패를 결정하는 일괄 all0 조건은 제거한다. 이것은 해당 코드의 정상 의미를 입증하는 판정이 아니다. 빌드 성공은 실제 부모 Editor 자연종료0, BuildReport Succeeded/errors0, 정확 산출물·보고 지문, 소유 프로세스 회수·로그/핸들 폐쇄 및 강제종료0을 함께 요구한다. 부모 비0·실제 빌드 오류·미완성·게임 비0을 이 예외로 허용하지 않는다.

Astra가 실제 PackageCache의 com.unity.test-framework.performance@0a60b857a52e/Editor/TestRunBuilder.cs를 읽어 OnPreprocessBuild의 CreateResourcesFolder/두 JSON 저장과 OnPostprocessBuild의 Cleanup을 확인했다. 강제중단 뒤 남은 Assets/Resources.meta, Assets/Resources/PerformanceTestRunInfo.json 및 .meta, Assets/Resources/PerformanceTestRunSettings.json 및 .meta 정확5파일을 이번 복제본의 빌드 파생물로 한정 허용한다. 생산630의 변경으로 재분류하거나 시험 성공으로 합산하지 않는다. Root는 최초5의 원문/지문을 evidence 아래 별도 보존한 뒤 링크가 없고 정확 복제본 내부임을 검증한 해당5파일만 정리할 수 있다. Resources 디렉터리는 다른 파일이 없을 때만 비재귀 제거할 수 있다. 패키지·EditorPrefs 직접변경0이다. 다음 빌드가 재생성하거나 정리하는 동일5도 before/after와 빌드귀속으로 보존하며, 최종 원장에서는 생산634와 별도 파생5를 구분한다. 다른 신규파일은 허용하지 않는다.

원인이 위 실행조정 경쟁상태로 좁혀졌으므로 별도 추가 빌드1회/1800초를 승인한다. 게임 실행1회/180초는 아직 미소비이며 새 빌드가 위 성공조건을 만족할 때만 사용한다. 추가 자동 재빌드/게임 재시도/기한연장0이다. Root는 evidence/round-2를 신규 회차 원시 경로로 사용하고, 필요하면 기존 미완성 build/windows 전체를 정확 작업 루트 안 evidence/round-1-build-preserved로 보존 이동하여 기존 빌드입구의 빈 출력 조건을 충족할 수 있다. 이동 전 절대경로·링크·소유프로세스0과 원문 지문을 확인한다. 기존 evidence의 원시 및 첫 실패 폐쇄는 불변이다. 진단의 고정 raw 출력이 evidence/boot-input-probe.raw.json인 경우 게임 미실행으로 해당 파일이 없음을 먼저 확인하고 최초 신규 생성만 허용하며, round-2 원장에 실제 위치와 지문을 결속한다.

제품·진단 C#5파일 변경0, 기존 Ship/ZIP 변경0, 권한/OS창 호출0을 유지한다. Root의 조정 보정 및 국소 문법·경쟁상태/신원불일치 분기 대조가 끝나면 이 절로 후속 실행을 진행한다. 새 문서 핀이나 별도 승인 사본을 기다리지 않으며 Luna는 최종 실행조정 차이와 회차2의 원시·파생5·기존 실패 보존을 같은 부팅 진단 묶음에서 독립 검수한다. 실제 실창/최종 수용 false는 유지한다.

위 보존 경로의 Root 실행 상세를 다음과 같이 확정한다. 정확 파생5는 evidence/interrupted-r1/generated-assets/동일상대경로로 LiteralPath 개별 이동하여 원문/핀을 보존하고 빈 Resources 폴더만 비재귀 제거한다. 사용된 실행조정 원문은 evidence/interrupted-r1/runner-used.ps1로 덮어쓰기 없이 복사·지문 대조한 뒤 기존 v1을 보정한다. 중단된 build/windows는 절대 원본/목적 경로가 모두 작업 루트 내부이고 링크/소유프로세스가 없음을 확인한 뒤 build/interrupted-r1로 보존 이동할 수 있다. 이는 앞 절의 evidence/round-1-build-preserved 대신 사용하는 정확 목적지다. 기존 중단본에 실행파일과201파일이 있어도 성공 BuildReport가 없으므로 실행하지 않는다. 전처리 뒤634를 대조하고 회차2는 evidence/round-2, 최초 고정 raw는 기존 evidence/boot-input-probe.raw.json 부재 확인 뒤 새로 작성한다.

GetProcessById 호출은 별도 try로 구분하여 실제 예외 체인의 ArgumentException으로 확인한 이미종료만 관측미귀속으로 기록하고 건너뛴다. 이후 핸들/출생/경로 확인 중 사라짐은 확보한 child.HasExited가 실제 참일 때만 종료전환으로 기록한다. 다른 오류·살아 있는 신원불일치·확인 실패는 계속 실패이며 넓은 catch 무시0이다. 정상 패키지 후처리 뒤634 복귀를 확인한다. 정확 파생5가 남으면 위 한정 이동 권위로 원시를 보존할 수 있으나 후처리 미완료 사실과 해당 실행 결과를 성공으로 덮지 않는다.

#### §8 자연 부팅의 준비 단계 공개 순서 보정 — Approved 한정 구현·추가 실행

REQ/AC-AUTOCH1-BOOTINPUT-001: Root가 보고한 round-2는 Editor27500 자연종료0·빌드11.9655초, Player39508 자연종료1·강제종료0·회수/로그폐쇄·원본630불변·복제634복원·Ship지문 불변이다. Astra는 evidence/boot-input-probe.raw.json의 success=false, dynamicUpdates=2, 관측2행과 첫 Enter 전 '시작 전 상태가 아닙니다.' 실패를 직접 확인했다. raw에는 실제 phase/revision/key가 없어 최초 실패값은 미관측이며 제품 오류나 정상 준비 상태였음을 확정하지 않는다. round-2 빌드와 게임 실행은 각각 소비 완료로 보존한다.

Astra의 생산 원문 대조에서 Chapter1PlayableFlowOwner는 DefaultExecutionOrder(210)의 Update에서 PreparingHub이고 hub.Ready이면 Hub를 공개한다. 기본 실행순서의 진단 Update가 hub.Ready 직후 Hub를 요구하므로 그 공개 전 전환을 관측할 수 있다. 이 정확 관측 순서 공백에 대해 Sol에게 복제본 Chapter1BootInputProbe.cs 한 파일의 국소 보정을 승인한다. 실제 ready를 처음 관측했을 때 phase/revision/key/frame/update를 기록하고, 정확 PreparingHub일 때만 기존 내부170초 안에서 다음 자연 Update를 기다린다. Hub가 된 뒤 기존 RunStartedRevision==0 및 RunKey==null, 준비/중립 영수증과 이후 입력/이동/해제 assertion은 그대로 요구한다. 다른 단계·생산 Diagnostic·기한초과는 실패이며 임의 단계 허용·assertion 완화·수동 Update·실행순서 변경·생산 코드 변경0이다. 최초 ready와 Hub 확정 관측은 제한된 행으로 기록하고 동일 대기 상태를 매 프레임 누적하지 않는다.

같은 파일의 raw 위치만 evidence/round-3/boot-input-probe.raw.json으로 바꿔 기존 실패 원문을 보존한다. 원래 evidence/boot-input-probe.raw.json과 round-2 전체 증거는 불변이다. Root는 기존 build/windows와 자연 생성 SaveData를 함께 build/completed-r2로 보존 이동할 수 있으며 정확 양쪽 절대경로·링크 없음·소유프로세스0·파일지문을 먼저 확인한다. 기존 실행조정 v1의 해당 실행 원문도 회차2 증거에 보존하고 새 증거 경로 evidence/round-3와 새 raw 읽기 위치만 결속한다. 새 build/windows는 비어 있고 SaveData가 없어야 하며 이전 저장의 편집·재사용0이다. 별도 소스 파일/기호/제품 API/설정 범위 추가0이다.

위 진단1파일 보정은 남은 국소 보정1묶음 안에서 수행한다. Sol의 정확 차이와 국소 컴파일/단계 분기 대조, Root의 소유 인수·634/원본/Ship 불변 확인 뒤 새 진단 빌드1회/1800초 및 성공 산출물 게임1회/180초를 승인한다. 동일 입력의 무변경 재시도가 아니며 자동 추가실행·기한연장0이다. 내부170초·256행/256KiB와 원시/종료 기준은 유지한다. Luna는 최종 같은 진단 묶음에서 변경과 실제 결과를 독립 검수하며 버튼별 승인·별도 문서 핀을 새로 요구하지 않는다. 이전 실패와 실제 창/최종 수용 false를 유지한다.

Sol의 동일 원문 분석에 따라 위 대기 조건은 정확 PreparingHub && RunStartedRevision==0 && RunKey==null로 더 한정한다. 이 세 조건을 함께 만족할 때만 다음 자연 Update로 넘기며 최초 ready 상태와 대기 사실을 각각 최대1회 기록한다. 준비 중 revision/key 불일치도 즉시 실패다. Hub 이후 기존 엄격 검사는 그대로 유지한다.

#### §8 자연 부팅·첫 입력의 실제 통과 통합 기록 — 한정 결과 확인

REQ/AC-AUTOCH1-BOOTINPUT-001의 round-3 실제 결과를 Astra가 원시·폐쇄·빌드보고·파일 원장으로 직접 대조했다. evidence/round-3/boot-input-probe.raw.json은 success=true·firstFailure/cleanupFailure 빈값·관측13행·자연 Dynamic16회다. 파일 전체는2047바이트이며 내부 bytes=1484는 관측 문자열 계수다. raw SHA256 4C50740B8427868299F58E284CAE2558B94B85A9158AAD468264DFC28AEF1578, closure SHA256 796E0CC499F1933FCDBE77559255372579EF3BDFC0AD32FEEE8F31D6FB0164C7, build-files 원장 SHA256 0BF52F7F8FF34AF76F12731BF8C2C53813D9902803F08016944232E01156D5CF를 기록한다. 이 세 상대경로의 기준은 output/chapter1-boot-input-probe-20261024다.

실제 frame2에서 PreparingHub/revision0/keynull을 관측하고 다음 자연 frame3에서 중립 영수증을 확인한 뒤 Enter를 발행했다. frame4 해제, frame6 원정 시작·세대1·완료tick8·x0과 정상 실행파일 부모 SaveData 경로를 관측했다. D 발행 후 frame8/tick33에 x=0.2915039와 move=(1,0)의 실제 완료를 확인했고, 해제 후 frame11/tick36에서 move=(0,0), frame16/tick41에서 완료 위치 안정3회를 확인했다. 요청 기록과 실제 소비/완료 기록을 구분하며 수동 InputSystem.Update 없이 정상 초기화→첫 출정→이동→해제를 연결한 별도 합성 입력 검사로 한정한다. 첫 실패의 실제 phase 미관측을 이번 결과로 소급 채우지 않는다.

Editor42600은 자연종료0·19.00857초, Player15900은 자연종료0·7.3709초이며 강제종료0·핸들 폐쇄·관측 프로세스 회수·로그 폐쇄를 확인했다. BuildReport는 Succeeded/errors0/warnings3, 자체 빌드시간9.3698899초로 외부 Editor 경과시간과 구분한다. closure의 error=null·복원오류0·복제634복원차이0·원본630차이0·잔존Editor/Player0 및 기존 Ship exe 지문 불변을 확인했다. 보조프로세스의 원시 비0은 그대로 보존하며 정상 프로토콜이라고 새로 단정하지 않는다.

진단 출력은 AcadeGameMakerChapter1BootProbe.exe와 해당 Managed/Data·전용기호를 포함하는 별도 빌드다. 실행파일 자체의 지문이 기존 Ship launcher와 같아도 전체 배포 바이너리 동등성은 입증되지 않는다. 이번 결과는 기존 Ship의 실제 OS 입력·창 초점·표시·사람 완주를 뜻하지 않으며 actualWindowAcceptance=false와 finalAcceptance=false를 유지한다. 기존 Ship/ZIP/시험실패와 회차2 빌드·저장·원시는 보존한다. 이번 한정 결과의 최종 독립 수용은 진행 중인 Luna 원시/변경분 검수 판정에 결속하며 그 완료를 선취하지 않는다. 새 시험·승인 사본·사용자 조작 요청은 추가하지 않는다.

Luna 최종 독립 검수 완료에 따른 한정 수용: Luna는 실제 raw 지문·전체2047바이트·13행/문자열1484바이트·자연 Dynamic16회, 별도 진단의 가상입력만 사용한 범위, 양 실행 정상종료·자식강제종료0·핸들/로그폐쇄·복제634복원·원본630불변·Ship 지문을 독립 대조하여 AC-AUTOCH1-BOOTINPUT-001 통과를 반환했다. Root도 빌드205파일 전수 지문 차이0, NUnit/TestRunner/AcadeGameMaker.Tests 미포함, 자연 생성 저장3파일 핀과 이전 round-2 실패 raw 불변을 확인하여 output/chapter1-boot-input-probe-20261024/verification-summary.v1.json에 보존했다. 이에 Astra는 정상 부팅→자연 입력 소비→첫 출정→이동/해제의 한정 검사 결과를 수용하고 이 진단 묶음을 종료한다. 위 독립검수 진행 중 문장은 작성 당시 상태이며 이 판정으로 해당 대기를 닫는다. 새 제품 검사·추가실행·사용자 반복 참여를 만들지 않는다. 실제 Ship 창 검증 미완료와 actualWindowAcceptance=false/finalAcceptance=false는 그대로 유지한다.
