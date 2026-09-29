# C3 초기 코드 게시 기반의 제한 계약 제안

- 날짜: 2026-09-29; 작성자·실제 모델: 솔, `gpt-6-sol`
- 상태: 읽기 전용 의존 폐쇄와 초기 게시 계약 제안. 승인·게시·빌드·시험
  검증 완료가 아니다. 코드/자산/meta/설정/Git 수정, Git 명령·네트워크 조회,
  Unity·컴파일 실행을 하지 않았다. 현재 동결·캡처 자료는 변경하지 않았다.
- 기존 [게시 의존 평가](2026-09-29-c3-code-publication-dependency-assessment.md)
  SHA-256 `78292AE307723AF226C1FA2C25B910EB0923B4187E45F735B6A4DD966038A239`는
  그대로 보존했다. 원격 기준 `46b14e97be2c5fc6017d92c09e3200899599830a`에
  Assets/Packages/ProjectSettings가 없다는 당시 확인 결과를 전제로 한다.
  이번 작업은 실시간 원격 존재 여부를 다시 조회하지 않았다.
- 추적: `REQ-M5D7QC3-001..007`, `AC-M5D7QC3-009/010`의 게시·재현 기반.
  C3 전체 AC007/008과 Review C4의 상태를 바꾸지 않는다.

## 최소 범위의 정의

현재 조립 구조를 유지하면 C3 몇 파일만으로 독립 Unity 프로젝트가 되지 않는다.
런타임 시작점 `AcadeGameMaker.Hub.Presentation.Unity`의 실제 asmdef 참조를
전이적으로 따라가면 아래 **11개 로컬 조립**이 닫힌다. 각 조립에 속하는
기존 C# 소스·asmdef·AssemblyInfo와 해당 meta, 상위 폴더 meta를 게시 후보로
묶는다. 하위 별도 asmdef 경계는 별도 조립으로 계산한다. 쓰지 않는 타입을
잘라내거나 asmdef/friend 방향을 바꾸어 최소화하는 새 구현은 제안하지 않는다.

| 조립/현재 디렉터리 | 실제 참조 근거 | 기존 Approved 또는 수용된 요구 연결 |
| --- | --- | --- |
| `Core` | Movement/Transfer/Combat/Input.Unity/Hub에서 공통 참조 | VD01 `REQ-MOV-001..004/006..010`, VD02 `REQ-WT-001..008`, VD03 `REQ-COM-001/003/004/006`의 기존 고정 단계·공통 값 기반 |
| `Profile` | Hub와 Input.Unity가 직접 참조 | M5D3–6의 profile 값, M5D7A–L의 encoder/decoder/선택·복구·저장·준비; `REQ-M5D7QC1-001..007`, `REQ-M5D7QC2-001..007`, `REQ-M5D7QC2R-001..006`, `REQ-M5D7QC3L-001..003`의 기존 범위 |
| `Input` | Input.Unity와 Hub가 직접 참조; `Unity.InputSystem` 참조 | [M5B3 입력 자산](../specs/work-contracts/2026-09-08-vd07-m5b3-device-action-asset.md)의 Approved `REQ-UX-001/006/007/009`, `REQ-PLAT-002/007` 연결과 실제 wrapper·의미 입력 값 |
| `Input/Unity` | Hub가 직접 참조; Core/Profile/Input 및 Movement/Transfer/Combat의 양쪽 조립 참조 | [M5B5](../specs/work-contracts/2026-09-08-vd07-m5b5-device-input-router.md)의 `REQ-UX-001/004/006/007/008/009/013`, M5D7M/N/O/P-A와 `REQ-M5D7Q0-001..011`, 기존 C2/C2R 및 현재 C3 lower |
| `Movement`, `Movement/Unity` | Input.Unity·Transfer.Unity·Combat.Unity의 실제 참조 | [VD01](../specs/work-contracts/2026-08-25-vd01-movement-sandbox.md) Approved `REQ-MOV-001..004/006..010`와 M2 ignore-pair 후속 범위 |
| `Transfer`, `Transfer/Unity` | Input.Unity·Combat.Unity의 실제 참조; Transfer.Unity→Movement 양쪽 | [VD02](../specs/work-contracts/2026-08-25-vd02-weight-transfer.md)의 승인·수용 M1/M2 `REQ-WT-001..008`, `REQ-MOV-004` |
| `Combat`, `Combat/Unity` | Input.Unity의 실제 참조; Combat.Unity→Movement/Transfer 양쪽 | 기존 VD03 M1–M4의 승인 범위. 특히 [M4B1](../specs/work-contracts/2026-09-05-vd03-combat-m4b1-ordan-unity-combat-bridge.md) `REQ-COM-001/003/004/006`, `REQ-WT-001/005/006`의 실제 Ordan driver |
| `HubPresentation/Unity` | Core/Input/Input.Unity/Profile/Unity.TextMeshPro 직접 참조 | `REQ-M5D7QA-001..010`, `REQ-M5D7QB-001..005`, `REQ-M5D7QC3-001..007`와 승인된 후속 세대·감사·시험 위치·재인증 보정 |

디렉터리 접두사는 `Assets/AcadeGameMaker/Runtime/`다. 타입 참조도 이 폐쇄를
뒷받침한다. `InputRouter :20`은 실제 `GameInputActions` 두 callback 형식을
구현하고 `:23–25`는 PlayerMovementController/TransferSimulationDriver/
OrdanBossCombatSimulationDriver/OrdanBossTerminalTeardown 필드를 보유한다.
Hub owner는 실제 DesktopProfileLaunchAdapterV1/InputRouter 및 lower 확인
형식을 사용한다. Gameplay 호출이 C3에서 없더라도 이 타입과 asmdef 의존은
사라지지 않는다. 다만 이 조립 포함이 실제 플레이 실행·장면·전투 콘텐츠의
새 승인이나 C4 실행 연결을 의미하지 않는다.

표는 각 그룹을 기존 계약에 연결한 **후보 범위**다. 그룹에 들어 있는 모든
파일의 최신 바이트가 승인되었다고 단정하지 않는다. 최초 기반 계약은 정확한
파일별 기존 승인·후속 지문을 대조한 목록을 별도로 동결해야 한다. 출처 연결이
불가능한 파일은 추가 검토 대상으로 분리하며 그 그룹 전체의 일괄 게시로
숨기지 않는다.

## 현재 집중·필수 회귀를 재현하기 위한 추가 조립

| 포함 후보 | 현재 실제 필요성 | 승인·검증 연결 |
| --- | --- | --- |
| `Runtime/Camera`, `Runtime/Camera/Unity` | 기존 Input.Unity.PlayMode.Tests asmdef가 두 조립과 URP runtime/2D를 직접 참조. Camera.Unity→Core/Movement 양쪽/Input.Unity | M5C1/M5C2의 기존 승인·수용; `REQ-ART-009/010/011`, `REQ-UX-013`의 고정 프레임 카메라. C3 제품 카메라 추가 아님 |
| `Editor/HubAuthoring` | Input.Unity.EditMode.Tests와 Hub.Presentation.EditMode.Tests가 직접 참조; HubAuthoring→Core/Input.Unity/Hub/TMP | Q0 `REQ-M5D7Q0-001..011`, Q-A `REQ-M5D7QA-009`, Q-B 기존 저작 검증 |
| `Tests/EditMode/Profile`, `Tests/EditMode/InputUnity`, `Tests/EditMode/HubPresentation` | 현재 562개 필수 편집 이름·하위/owner/감사 집중 및 기존 fixture가 이 조립들에 있음 | 원본 562 선택과 이름, 별도 집중 원장·377개 내부 행을 그대로 보존 |
| `Tests/PlayMode/InputUnity`, `Tests/PlayMode/HubPresentation` | 현재 610개 필수 실행 이름과 owner/Hub 집중, 실제 preparation friend 및 실제 asset fixture | 기존 610 이름과 별도 51 결함 행·신규 집중 이름 구분 유지; C3 Play 위치 amendment 보존 |
| `Tests/Bootstrap/Editor` | 최초 패키지/설정의 기존 플랫폼 검사 조립 | 초기 기반 계약의 `AC-PLAT-001/005` 부분 재현을 명시할 때 포함 |

위 경로의 접두사는 `Assets/AcadeGameMaker/`다. 각 시험 조립의 기존 소스와
meta를 함께 포함하되, 선택되지 않은 시험이 빌드된다는 사실과 그 시험의
실제 실행을 구분한다. 562/610에 없는 다른 게임 회귀까지 실행·수용했다고
주장하지 않는다. `Run`, `Costumes`, `Costumes/IO`, `Presentation` 및
`Presentation/Unity`, 관련 저작·시험 조립은 위 전이 그래프에서 도달하지
않는다. 현재 계약에 필요하다고 입증되지 않은 이들 자료를 일괄 추가하지 않는다.
이를 별도 제품 기반 전체 수용으로 확대하려면 독립 계약이 필요하다.

## 실제 실행 자산과 설정의 폐쇄

| 최소 자산/설정 후보 | 실제 참조와 보존 이유 | 기존 승인 연결 |
| --- | --- | --- |
| `Assets/GameInput.inputactions`와 meta, `Runtime/Input/GameInputActions.cs`와 meta | importer meta가 wrapper 경로·namespace와 generateWrapperCode=1을 지정. 입력 시험은 실제 wrapper/map/asset를 사용 | M5B3 및 `REQ-PLAT-007`. 재생성 없이 원본 자산·wrapper·고정 GUID 동시 보존 |
| `Assets/Prefabs/Hub/HubRuntimeRoot.prefab`, `HubMenuRoot.prefab`, `Assets/Scenes/Hub.unity`와 meta/부모 폴더 meta | 신규 actual cohort가 MenuPrefab을 읽고 기존 필수 Hub Play는 실제 Hub 장면과 두 prefab을 로드. Hub 장면 GUID는 두 prefab과 실제 router/latch로 해석됨 | Q0/Q-A/Q-B Approved 경계, `REQ-M5D7QA-001/007/009`, `REQ-M5D7QB-004/005` |
| `Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab`, `Assets/Prefabs/Player/CombatEncounterPlayer.prefab`와 meta/부모 폴더 meta | 현재 610개 필수 원장의 OrdanBossTerminalTransitionRequesterPlayModeTests 실제 9개 이름이 graph prefab을 로드한다. graph의 중첩 player prefab과 실제 컴포넌트 GUID 전이까지 포함 | 기존 M4B3A 저작 graph·M5D1 수용 `REQ-M5D1-001..006`, `REQ-UX-004/006/007/008/013`, `REQ-COM-004/006`. C3의 새 전투 실행 연결 아님 |
| `Assets/UI/Fonts/Hub`의 두 NotoSansCJKkr OTF와 두 SDF, OFL-1.1.txt 및 meta | 저작 builder가 두 원본 font/SDF/license 존재를 명시 검사. prefab은 Regular-SDF GUID 사용, 두 SDF는 현재 Hub shader GUID 참조 | Q-A `REQ-M5D7QA-007/009`; 기존 글꼴 원본·라이선스·고정 label 프로필 보존 |
| `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader`, `TMPro_Properties.cginc`, `Assets/UI/Resources/TMP Settings.asset`와 meta/폴더 meta | SDF material→Hub shader→local include. TMP Settings는 실제 uGUI 패키지 TMP_Settings 스크립트 GUID | Q-A `REQ-M5D7QA-007`, P-B `REQ-M5D7PB-001..004`. 별도 TextMeshPro 패키지나 예제 자료 추가 없음 |
| `Assets/Scenes/Bootstrap.unity`, `Assets/Settings`의 UniversalRP/Renderer2D/GlobalSettings/DefaultVolumeProfile와 meta, PlatformBaseline 문서/meta | EditorBuildSettings가 Bootstrap만 enabled로 고정. QualitySettings는 UniversalRP, GraphicsSettings는 GlobalSettings, UniversalRP는 Renderer2D, GlobalSettings는 DefaultVolumeProfile을 참조 | [초기 플랫폼](../specs/work-contracts/2026-08-25-vd09-unity-bootstrap.md) 및 [6000.6 후속](../specs/work-contracts/2026-09-06-unity-6000-6-baseline-update.md)의 Approved `REQ-PLAT-001/002/005/007` |
| `Packages/manifest.json`, `packages-lock.json` | InputSystem 1.20.0, physics2d 1.0.0, URP 17.3.0, test-framework 1.6.0, uGUI 2.6.0 직접 의존. lock의 전이 의존을 그대로 보존 | 플랫폼 후속 및 P-B Approved. Unity.TextMeshPro는 uGUI 안의 실제 조립 |
| 필요한 기존 `ProjectSettings` | ProjectVersion 6000.6.0f1, ProjectSettings activeInputHandler=1, EditorBuildSettings/Graphics/Quality, 고정 timestep·Physics2D·Tag/Layer 등 현재 플랫폼 값 | 원래 bootstrap 최소 설정 allowlist와 후속 플랫폼 계약. 전체 설정의 게시 여부는 파일별 기존 baseline 원장으로 확정 |

Hub prefab에 있는 package script GUID 네 개는 현재 PackageCache의
CanvasScaler/Image/EventSystem/TextMeshProUGUI meta로, TMP Settings GUID는
같은 uGUI 패키지의 TMP_Settings meta로 해석했다. Bootstrap의 두 script GUID도
URP UniversalAdditionalCameraData/Light2D meta로 해석했다. PackageCache는
로컬 출처 조회에만 썼으며 게시 대상이 아니다. 패키지는 manifest/lock으로
복원하는 계약을 유지한다.

현재 `Assets/Settings` GUID 직접 대조 결과 UniversalRP 4개, GlobalSettings
174개, DefaultVolumeProfile 20개 및 두 Hub SDF 각각 2개 비기본 GUID는
Assets 또는 PackageCache meta로 해석됐다. Renderer2D 26개 중 다음 7개는
그 meta 조회로 해석되지 않았다: `e5c6678ed2aaa91408dd3df699057aae`,
`03cfc4915c15d504a9ed85ecc404e607`, `53a11f4ebaebf4049b3638ef78dc9664`,
`8f96cd657dc40064aa21efcc7e50a2e7`, `57d7c4c16e2765b47a4d2069b311bffe`,
`24ec0e140fb444a44ab96ee80844e18e`, `5688ab254e4c0634f8d6c8e0792331ca`.
이들은 probeVolume debug shader/mesh/texture 여섯 항목과 m_FallOffLookup다.
**메타 조회 미해석은 자산 결손이나 현재 시험 실패 판정이 아니다.** 엔진 내장
자료 또는 패키지 직렬화 이력 등의 추가 출처 대조와 기존 플랫폼 수용 자료,
정확 에디터의 실제 import/build 진단을 게시 전 범위로 기록해야 한다.
현재 자산을 삭제·재생성·보정하지 않는다.

현재 610 필수 원장에 실제 OrdanBossTerminalTransitionRequesterPlayModeTests
9개 이름이 포함되어 있어 graph와 nested player prefab을 위 최소 실행
자산에 명시했다. GUID 사슬은 Camera.Unity의 SimulationCameraDriver,
Combat.Unity/Movement.Unity/Transfer.Unity/Input.Unity의 실제 컴포넌트와
위 두 prefab으로 닫히며 외부 script GUID 한 개는 현재 URP의
PixelPerfectCamera meta로 해석했다. 추가 게임 조립이 생기지 않는다.
이 시험은 prefab을 로드하며 별도 OrdanBossEncounterSandbox 장면 게시를
자동 요구하지 않는다. 전투 player/enemy/장면 전체를 일괄 게시할 근거로
확대하지 않는다. 임시 `__m5d7q0-override-base`
경로는 시험이 생성·삭제하는 음성 fixture다. Assets/TextMesh Pro 경로는
비승인 예제 부재 검사와 구분하며 해당 폴더를 게시하라는 참조가 아니다.

## 초기 기반 게시 계약에 필요한 구체적 승인 항목

아스트라가 별도 초기 기반 계약을 Approved로 만들 때 다음을 명시하는 안을
제안한다. 기존 제품 요구의 동작 변경 없이 **원격에 없는 선행 기반의 첫
게시와 재현 범위**만 승인한다.

1. 기준 원격 SHA, 위 11+2 런타임/1 저작/5 시험 조립과 선택한 Bootstrap
   시험, 실제 자산/설정 후보를 **정확 파일 목록**으로 동결한다. 각 파일에
   원래 spec/REQ·승인 또는 후속 결정·실제 SHA·meta GUID·조립 소속을 기록한다.
   관련 source AssemblyInfo/friend도 원본 바이트 그대로 포함한다. 목록에서
   빠진 외부 참조와 출처 미확인 파일은 별도 차단/검토 행으로 남긴다.
2. C3 동결 원장은 구현자·아스트라의 최종 독립 검수본으로 고정한다. 시험이
   진행 중인 현재 R5/R6 중 하나를 이 제안이 최종 수용본으로 고르지 않는다.
   최초 기반 commit과 승인 C3 후속 변경을 별도로 식별하거나, 같은 PR이면
   두 파일 집합과 지문 관계를 분리한다. 기존 과거 수용을 새 게시의 빌드
   통과 증거로 치환하지 않는다.
3. 실행 도구 `qa/tools/Invoke-UnityQa.ps1`, `Test-UnityQaOwnedEditorWait.ps1`,
   기존 Test-QaCatalog와 정확 선택/예상 이름·행 원장 생성/대조 도구를
   필요한 목록에 포함한다. `REQ-PLAT`와 C3 `AC009/010`의 기존 승인을
   연결하고, 현재 562/51/610·집중·377 내부 행의 원본 이름/순서와 역사
   지문 문서를 보존한다. Q0 pin이 직접 읽는 문서·fixture는 실행 의존이다.
4. ProfileResetCrashWorker의 Program.cs/csproj 및 root-lock holder 같은
   외부 fixture는 실제 선택이 호출하는 경우에만 별도 명시한다. process
   회귀는 562에서 제외된 역사와 구분한다. 전체 C1 process/restart 회귀를
   계약에 추가한다면 현재 net10.0 worker 및 설치 경로의 실제 실행 환경과
   필요한 파일을 승인 범위에 명시한다. 생성 bin/obj는 포함하지 않는다.
5. 정상 깨끗한 checkout에서 정확 Unity/패키지 import, 조립 누락·script GUID
   진단, 승인된 집중/필수 회귀 및 bootstrap 범위 검증을 후속 수행한다.
   프로젝트 설정·URP 7개 미해석 GUID의 대조도 이 단계에서 실제 판정한다.
   기존 로컬 Bee 컴파일이나 실행 중인 현재 프로젝트 시험만으로 원격 첫
   게시가 재현됐다고 주장하지 않는다. Unity 전체 게임 빌드/실행 수용은
   해당 계약에 명시하지 않았다면 별도 범위다.

`.gitattributes`의 실제 eol 규칙과 작업 파일 바이트·meta/GUID를 모두 대조해
게시 과정의 줄바꿈/재생성으로 Q0 엄격 pin을 바꾸지 않는다. 역사 9행·현재
7행·후속 2행 및 adapter `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB`,
router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB` pin을
보존한다. C3L·C1·C2·Q-A/Q-B의 선행 수용 지문과 현재 후속 변경의 관계도
원장에 남긴다. 허용 pin을 늘리거나 과거 증거 파일 내용을 새 모델/값으로
고쳐 게시를 통과시키지 않는다.

포함하지 않는 생성/임시 범위는 Library/Temp/Logs/obj/bin/UserSettings,
임의 InitTestScene, 현 임시 프로필/root 자료, 자격증명/라이선스, 관계없는
미디어·의상·sprite 생산 자료다. 이 자료를 현재 미추적 상태라는 이유로
추가하지 않는다. 기존 프로젝트 설정 안의 개인정보/환경별 경로 유무는
정확 목록 단계에서 실제 내용으로 확인하며 설정 그룹 전체를 자동 수용하지 않는다.

## 조회 지문과 남은 불확실성

| 조회 자료 | 실제 SHA-256 |
| --- | --- |
| Packages/manifest.json | `AD3A5D2329D5A8FD71BE89873C75BA2CAA98B4391558A69D83D81CADFE684E1E` |
| Packages/packages-lock.json | `B2B9B1DE4381F3C289837ACD1BDE69013C1F7581511E26861DFDF4F5AE00E550` |
| ProjectSettings/ProjectVersion.txt | `39584C3E6C1E571D53A79EF2DD6EA91C0568AC086792CC435E86401A0BA824ED` |
| ProjectSettings/EditorBuildSettings.asset | `71EF0DE27D164D41A67D502E6EC79D5875D64ED4B61C831C1E57BD0D544D6ADB` |

미승인 자료를 기존 수용 그룹에 섞지 않도록 파일별 최신 승인/바이트와 설정
목록을 확정하는 작업, 일곱 GUID의 추가 출처 진단, 깨끗한 첫 checkout 재현,
실시간 원격 상태와 실제 게시 절차는 남아 있다. 위 조립·자산 그룹은 최소
의존 범위의 구체적 제안이며 완결된 게시 allowlist나 검증 통과 원장이 아니다.
