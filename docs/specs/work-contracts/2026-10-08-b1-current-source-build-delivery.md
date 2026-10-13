# B1 현재 소스 Windows 빌드와 전달 근거

- 상태: **Review — 구현·실행·빌드·사용자 전달 수용 권한 없음**.
- 작성: 아스트라 `gpt-6-astra/ultra`, 2026-10-08. 솔 `gpt-6.1-sol/ultra`의 한정 구현, 루나 `gpt-6-luna/max`의 독립 검수, 아스트라 최종 통합, Root 단일 native 실행을 제안한다. 역사 코드의 당시 참여 표기는 보존한다.
- 목적: 기존 B1 G8의 **현재 소스 standalone1회/900초**를 실제 B1 프로젝트·장면·설정·산출물에 연결하고, 사용자가 실행할 한국어 안내와 독립 결과를 같은 빌드에 결속한다. B1의 거점·원정·보스 진입/실패 복귀 범위와 C1C 통합 뒤 CH1 전체 완주 전달을 구별한다.
- 권위: [CH1 주 계약](./2026-10-03-chapter1-playable-delivery.md) §11.4·§12·§26~30, [G3 Approved](./2026-10-08-b1-basic-movement-observation-and-acceptance.md) REQ/AC-C1B-G3OBS-010, [활성 전략](./2026-10-03-play-first-delivery-strategy.md) REQ/AC-PROC-011·012. 이 문서는 해당 원문·소진 이력·필수 AC를 바꾸지 않는다.
- 현재 허용 작업: 이 Review 문서1 작성뿐이다. 제품·설정·README·기존 문서·원장 쓰기0, 컴파일·Unity·native·GUI·프로필 IO·자식 배정0.

## 1. 직접 읽은 현재 상태

프로젝트 기준 경로는 `output/chapter1-playable-r1/project/`다. 아래 값은 준비 시점의 관측이며 최종 빌드 입력이 아니다.

| 대상 | 현재 관측·SHA256 | 필요한 구별 |
| --- | --- | --- |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/LocalPlayableSliceBuild.cs` | 4488바이트 · `C0EFB6D82D7BE65C2ED94FD48A3041E88D9B97E9A7B12F35745B93729F26377D` | `output/local-playable-slice-r1/project`에만 허용된 경로 가드, 옛 단일 전투 장면/validator·옛 계약SHA·당시 Terra 표기·`LocalPlayableSlice.exe` 출력이다. B1 실행에 그대로 호출할 수 없다. |
| `ProjectSettings/EditorBuildSettings.asset` | 264바이트 · `C47C2F72BCD485E4B2A69592B9A06B672E4866E288762D615F11602AA2DEECA4` | 활성 장면1은 `Assets/Scenes/LocalPlayableSlice.unity`다. |
| `ProjectSettings/ProjectSettings.asset` | 20913바이트 · `5A94F3C7571DC7F2EB0C51911CE70C3CA33F5351E409BC159C1D21D34270D028` | company/product는 AcadeGameMaker, Standalone backend 직렬값0, InputSystem1, runInBackground0, 기존 창/로그/식별자 설정이다. 빌드 직전 공개 getter로 Mono를 확인하며 이 문서에서 설정을 자동 수리하지 않는다. |
| `Assets/Scenes/Chapter1Playable.unity` | 38532바이트 · `912589EB6DEF11EC4822306C3599A366ECC626DA77F83B48951463EEBAE420BA` | 실제 FlowOwner1·Hub/Ordan prefab·catalog·방6·원문 계획4 참조가 있다. 실제 실행·완주 증거는 아니다. |
| 같은 scene의 `.meta` | 155바이트 · `CE90DF5275E013053C4031B6736DE57CEF45C6BAE0A7659999595146056CD55A` | GUID `c23754d3cae29e34182629f0c3e731f9`. |
| `ProjectSettings/ProjectVersion.txt` | Unity `6000.6.0f1 (f7f8ed4d1e24)` | 저장소 루트 README의 오래된 Unity6.3/프로젝트 미생성 안내를 현재 빌드 사실로 재사용하지 않는다. |
| 사용자 안내 | 저장소 `README.md`는 존재하며 읽었다. B1 출력 루트·그 project 및 옛 출력 루트의 `README.md`는 부재 | 이 작업은 저장소 README를 고치지 않는다. 미래 B1 전달 폴더에 정확 안내1개를 제안한다. |

현재 v8 `58E1E09A8CD5E1ADBE3350DB5BFB648824CAE9393823569BC51BCD6D415A0F31`은 실제590의 출발점이다. Luna sourceQA2 `B407A30AC9CEBF71D7DF0FED0BEE51C8894EA74C98D34F18030A902EE6FF4845`와 Astra source 수용 `50BDD7049CE80471F388F621F636A9019EDD323491E3A893F5C1FE585F831891`은 정적 범위만 수용했다. 이후 빌드 파일·설정 구현, 실제6방 수용 기록과 선행 검사에 따라 최종 입력이 달라지므로 현재590·scene 지문을 미래 G8 최종값으로 고정하지 않는다.

`Chapter1DevelopmentProfile.ExactRoot`는 현재 `C:\Users\me\Documents\GPT-workspace\AcadeGameMaker\output\local-playable-slice-r1\profile\chapter1-development`다. B1 소스 폴더 이름과 다르다는 이유로 이를 변경·새 경로로 이관하지 않는다. 이번 B1 빌드는 해당 승인된 로컬 개발 환경용 후보이며 다른 컴퓨터에 복사하면 바로 저장·실행 가능하다고 설명하지 않는다. 제품 실행의 실제 프로필 접근은 해당 기존 실행 권위에 따라야 하고, **빌드 준비·빌드 도구는 이 프로필을 읽거나 쓰지 않는다.** 일반 사용자 프로필 fallback·자동 초기화/복구·임의 삭제0이다.

## 2. 정확 구현 제안과 단일 소유

**REQ-C1B-BUILD-001:** 이 계약이 Approved가 된 뒤 제안하는 제품 변경은 아래 정확3경로다. G3 정확7 승인에 빌드 파일이 이미 포함됐다고 해석하지 않는다.

| 프로젝트 상대 경로 | 허용 변경 |
| --- | --- |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1PlayableBuild.cs` | 신규 Editor 파일1. 같은 기존 조립체·네임스페이스에서 정확 B1 경로/권위·선행·입력·장면/설정 검사를 거쳐 명시 단일 scene의 Windows x64 Mono 빌드1, 원시/포함 근거/실제 출력 원장만 작성한다. |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1PlayableBuild.cs.meta` | 신규 meta1. 충돌 없는 새 GUID를 실제 발급하여 원장에 결속한다. 기존 GUID를 재사용하지 않는다. 새 부모 폴더0이다. |
| `ProjectSettings/EditorBuildSettings.asset` | `m_Scenes`를 활성 `Assets/Scenes/Chapter1Playable.unity`·현재 실제 meta GUID의 정확1행으로 바꾼다. 그 외 키/설정은 유지한다. |

기존 `LocalPlayableSliceBuild.cs`/meta·Builder/validator·실제 scene/prefab·Runtime·asmdef/friend·Packages·`ProjectSettings.asset`·PlayerSettings·렌더러·입력·폰트·프로필·Q0·스테이징·C1C 변경0이다. 새 빌드가 이미 승인된 scene/prefab의 수용 필드를 스스로 기록하거나 자산을 다시 생성/저장하지 않는다. 기능/의존 부족으로 다른 경로가 필요하면 정확 영향으로 이 계약의 범위를 먼저 판정한다.

Root는 기존 공유 제품 작성자·Unity/컴파일/GUI가 모두 종료한 시점의 실제 입력을 인계한다. 솔이 정확3의 유일한 작성자로 구현·범위 내 보정·국소 확인을 누적하고 쓰기를 닫는다. 루나는 같은 최종 변경과 영향 근거를 독립 검수하고 아스트라가 한정 승인한다. Root만 실제 빌드 실행·프로세스 소유를 가진다. 현재 진행 중 G3 최소 컴파일의 고정 입력을 먼저 닫지 않고 이 제안으로 제품을 고치지 않는다.

빌드 코드/설정은 가능하면 G4의 현재 입력 재결속 **전에** 구현·검수하여 기존 검사들이 같은 최종 바이트를 소비하도록 한다. 이미 수행한 필수 검사가 있다면 추가3의 영향과 재사용 가능 범위를 독립 판단한다. 과거 결과를 새 입력 결과로 바꾸거나 모든 검사를 자동 반복하지 않는다.

## 3. 빌드 진입점·설정·사전 거절

**REQ-C1B-BUILD-002:** 제안하는 유일한 실행 메서드는 `AcadeGameMaker.Combat.Authoring.Chapter1PlayableBuild.BuildWindows`다. 메뉴 자동 실행·빌드 후 앱 자동 실행·startup callback·외부 프로세스 시작·환경 설치/수리0이다. 기존 Unity6000.6.0f1에서 Root의 별도 정확 실행 계획이 이 메서드1만 호출한다.

진입점은 정규화한 실제 project가 위 B1 경로와 정확히 일치하고 경로/파일이 링크·재분석 지점으로 다른 영역에 연결되지 않았는지 확인한다. 프로젝트·필수 원시·승인/소유 파일·출력 경계, 현재 source/asset/meta/asmdef/Packages/설정 전수 입력과 실제 빌드 도구의 길이/SHA를 검사한다. 명시 필드 존재·실제 자료형·중복/잘못된 JSON·null 미결을 거절하며 잘못된 값의 기본0 투영을 승인으로 쓰지 않는다. 닫힌 실제 입력 원문만 소비한다.

현재 actual active build target은 아직 판정하지 않았다. G8의 별도 시작 계획은 처음부터 `StandaloneWindows64`를 지정해야 한다. 실제 active target과 `PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone)`의 Mono, 기존 InputSystem/패키지·설정 원문을 검사한다. 메서드 안에서 플랫폼 전환·define 변경·도메인 재로드·backend 수정으로 조건을 맞추지 않는다. 맞지 않으면 호출 전 거절하고 실제 원인을 남긴다.

`BuildPlayerOptions`는 `scenes = ["Assets/Scenes/Chapter1Playable.unity"]`, `target = StandaloneWindows64`, `locationPathName = <아래 정확 exe>`, `options = StrictMode | DetailedBuildReport`를 제안한다. Development/AllowDebugging/AutoRunPlayer/추가 define0이다. EditorBuildSettings의 정확1행과 명시 scene 인자가 같은지 검사한다. 사용자 전역 Build Profile이나 활성 목록을 암묵적으로 소비하지 않는다. `BuildPipeline.BuildPlayer(BuildPlayerOptions)`와 반환 BuildReport는 공식 공개 입구다. [Unity6000.6 빌드 API](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/BuildPipeline.BuildPlayer.html)

저장 scene만 열어 기존 `Chapter1ExpeditionAuthoringValidator.ValidateScene`을 호출하고 실제 catalog1의 기존 `Validate(true)`를 통해 방6의 실제 수용 선행을 확인한다. 각 room의 기존 `ValidateRoom(prefab,true)`, Hub/Boss의 기존 validator와 원문 meta/승인 증거를 대조한다. 그 결과가 GUI 이동 증거를 대신하지 않으며 실제 Luna 방별 보고·Astra 수용·정확6 기록 원장도 별도로 결속한다. scene/자산 변경·SaveScene/SaveAssets·Builder.Build·AcceptBasicMovement 호출0이다. 임시 Review scene·dirty scene·미저장 구성은 빌드하지 않는다.

BuildPlayer 전에는 필요한 scalar·경로/지문을 보관하고, 반환 뒤 기존 GameObject 참조를 그대로 성공 근거로 사용하지 않는다. Unity는 BuildPlayer 호출이 Editor 스크립트의 GameObject 참조를 무효화할 수 있다고 명시하므로 필요한 사후 대조는 원문/재취득으로 한다. [Unity6000.6 빌드 API](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/BuildPipeline.BuildPlayer.html)

## 4. 실제 G8 한 번의 포함·유출 검사

**REQ-C1B-BUILD-003:** 새 관측 타입 `AcadeGameMaker.Input.Unity.Chapter1BasicMovementTickObserver` 전체는 `UNITY_EDITOR` 안에 있어야 한다. 현재 meta GUID는 `2017cd174d9a4846982f8f14a4f7c90b`다. 이 사실을 Player에서 실제 제외됐다는 결과로 미리 표시하지 않는다.

빌드 전 저장 `Chapter1Playable.unity`, 방6, 그 실제 의존 자산에서 관측기/임시 Review root/관측 설정이 직렬화되지 않았는지 실제 컴포넌트·MonoScript GUID·원문을 읽어 대조한다. Hub/Ordan과 의존 목록도 누락하지 않는다. 전체 Runtime→Editor 참조0·기존 조건부 범위를 독립 소스 대조하며, 임시 수집기를 제거하고 다시 저장하는 자동 수리0이다.

같은 실제 G8의 `BuildReport`에서 summary·steps·실제 파일 목록·packedAssets와 그 contents·자산/scene 연결 정보를 원문으로 보존한다. `DetailedBuildReport`는 이1회에서 필요한 scene별 포함 정보를 요청하기 위한 옵션이다. 실제 시작 scene1·승인 방6/Hub/Ordan/계획4·필수 스크립트/폰트/의존 자산과 빌드 출력의 연결을 판독한다. 빌드 최적화로 개별 prefab 파일이 평문으로 나오지 않는다는 사실을 누락으로 오판하지 않고 실제 GUID/자산 매핑을 따른다. 필요한 매핑 자체가 관측되지 않으면 미수용이며 다른 빌드로 보충하지 않는다. [Unity BuildReport](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Build.Reporting.BuildReport.html), [PackedAssets](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/Build.Reporting.PackedAssets.html)

Mono 결과의 실제 `Chapter1Playable_Data/Managed/AcadeGameMaker.Input.Unity.dll`과 제품 Runtime 조립체들을 **코드 실행 없는 메타데이터 읽기**로 조사한다. 현재 Editor 원래 RSP에는 `Library/PackageCache/com.unity.nuget.mono-cecil@ecb9724e46ff/Mono.Cecil.dll` 참조가 이미 있고 실제 파일도 존재한다. 새 빌드 파일은 그 기존 라이브러리로 출력 DLL의 타입·메서드·필드·형식/멤버 참조·assembly 참조를 읽는 방안을 제안한다. 새 패키지/asmdef/ref 추가0, 출력 DLL을 실행 로드하거나 정적 초기화/게임 메서드를 호출하지 않는다. 관측기 타입 및 그 설정/IO/Start/FixedUpdate 멤버 부재, 관측기/Editor 타입 참조 부재, Runtime의 UnityEditor 참조 부재를 실제 메타데이터로 남긴다. 포괄적인 파일명/문자열 검색만으로 타입 비유출을 통과시키지 않는다.

원래 형식명이 필요한 실제 게임 타입·핵심 조립체의 포함도 확인하여 아무 코드도 없는 출력을 관측기 부재 성공으로 세지 않는다. 정확 Mono.Cecil 사용·기존 참조 호환·메타데이터 판독 경계는 솔 구현과 루나 영향 검수에서 확인한다. 그 입구를 사용할 수 없으면 해당 AC 미수용이며 설치·예비 Player 빌드·추가 실행을 자동 허용하지 않는다. 이 검사는 실제1회 빌드의 후처리이며 별도 Player build/Player 실행0이다.

## 5. 정확 출력·원시·한국어 안내

**REQ-C1B-BUILD-004:** 저장소 기준의 미래 출력 제안은 다음과 같다. 읽기 시 아래 build/evidence 접두사는 부재였으며 미래 실행 시 다시 확인한다.

| 경로 | 내용·소유 |
| --- | --- |
| `output/chapter1-playable-r1/build/b1-windows-r1/Chapter1Playable.exe` | 현재 입력의 실제 Windows x64 exe. Root G8 실행이 새 폴더에 생성 |
| 같은 `b1-windows-r1/`의 Unity 산출물 | 실제 `_Data`·런타임/DLL 등 원래 BuildReport 산출물 전체. exe만 떼어 전달하지 않음 |
| 같은 `b1-windows-r1/README.ko.md` | 빌드/검수 종료 뒤 Root가 검수된 한국어 안내를1회 작성. 실제 실행 경로·필수 동봉 파일·조작/종료·지원 범위·고정 로컬 환경/저장 경계·미완료 범위·빌드 지문을 기재 |
| `output/chapter1-playable-r1/evidence/b1-current-source-build-r1/` | 아래 원시/결과만 허용하는 실행 증거 접두사. 기존 실패·다른 실행 출력 덮어쓰기0 |

증거 파일의 제안 이름은 `execution-start.json`, `source-before.json`, `build-inputs.json`, `build-report.json`, `player-inclusion.json`, `build-outputs.json`, `source-after.json`, `execution-end.json`, `unity.log`, `stdout.log`, `stderr.log`다. 시작/프로세스 종료/원시 스트림은 Root의 실제 실행 수단이, BuildPlayer 반환/포함 결과는 새 Editor 진입점이 각각 단독 소유한다. 파일별 작성자를 실행 계획에서 고정하고 같은 파일을 두 작성자가 열지 않는다. 성공 시만 있는 report와 실패 때 부재한 report를 구별한다. JSON 원문/로그는 CreateNew·UTF-8·실제 예외/정수 종료/null 미관측을 보존한다. 출력 디렉터리가 있으면 삭제·덮어쓰기·기존 exe 재사용 없이 거절한다.

`build-inputs`는 최종 권위 개정·소유·실제 소스/자산/설정·실행 메서드/인자·Unity/target/backend·선행 증거 지문을 결속한다. `build-outputs`는 실제 산출 경로/길이/SHA와 report의 결과·크기·시간을 담는다. README는 compiler 입력이 아니며 최종 전달 파일 원장에 포함한다. 원장이 자기 SHA를 자기 안에 요구하거나 README↔원장 사이에 재귀 SHA를 넣지 않는다. 실패/부분 출력도 같은 실제 원문으로 보존하며 성공 목록으로 옮기지 않는다.

안내는 현재 실제 입력 매핑을 읽어 이동·점프·대시·공격·전이·거점 출정·실패 후 재도전·정상 창 종료를 확인한 범위에서만 적는다. G3에서 금지한 공격/전이는 그 관측 사례 한계와 사용자 B1 제품 조작을 구분한다. 작동을 직접 확인하지 않은 키·설정 메뉴·재시작 복구·CH1 종료를 성공 기능으로 설명하지 않는다. 별도 사용자 설치/권한 변경을 요구하지 않으며 환경 경계가 해결되지 않으면 실행 안내에 제약으로 남긴다.

## 6. 유한 검사·실행 순서와 첫 실패

**REQ-C1B-BUILD-005:** G8은 기존 순서에서 앞당기지 않는다.

1. 실제 G3 여섯 방/10구성의 연속 원시·독립 판독·방별 수용과 정확 prefab6 기록을 닫는다.
2. 새 build 코드/설정까지 포함한 현재 입력을 결속하고 G4 집중35 단1회900초, G5 필수44 두 모드 각1회·합산1800초를 수행한다. G6 별도 수동12, G7 정상1/실패3 각1800초·합산7200초 및 기존 별도 실제 창 키보드/포인터 관문을 유지한다.
3. 모든 실제 선행·정확 최종 입력/도구·Root 소유·독립 사전 위험/영향 검수·별도 Astra G8 실행 승인·실제 도구 권한 심사를 충족한 뒤 Root가 G8 하나를 실행한다.
4. 루나는 실제 입력/출력·BuildReport/메타데이터·원시 종료/시간·소스 불변·선행 적용성을 독립 검수한다. 아스트라는 B1 현재 소스 빌드 한정 수용만 발급하고 실제 미수용 기능을 명시한다.

구현 중 경로/필수 필드·입력/출력 구별·CreateNew·조건부/메타데이터 로직의 정상/거절 국소 확인은 같은 작업 원장에 누적한다. 작은 수정마다 새 전체검수/상태 보고·새 도구를 만들지 않는다. 이 준비는 추가 compiler 프로세스 예산0이다. 새 Editor 파일의 실제 컴파일은 선행 G4/G5의 기존 Unity 시작/가져오기 예산 안에서 확인하는 것을 우선 제안한다. 별도 최소 compiler가 실제 필요하다면 정확 입력/영향·유한 횟수/시간을 기존 실행 계획에서 먼저 한정하며, 지금 준비 중인 G3 최소3×60/180을 빌드 코드용으로 재사용하거나 증액하지 않는다.

G8 전체 단조상한900초에는 Unity 시작·가져오기/필요 컴파일·가드·단1회 BuildPlayer·포함 메타데이터/원시 봉인을 포함한다. 별도 startup900·보충 빌드·예비 Player 빌드0이다. 종료 확인5초와 출력 회수5초의 기존 한정 정리만 둔다. Root는 원래 보유한 정확 PID/StartTicks/EXE/handle의 프로세스만 관리하고 프로세스명 전체 종료0이다. 최초 실패·시간초과·입력 차이·필수 원시/종료/회수 누락에서 즉시 후속 작업0, 자동 retry0이다. BuildPlayer를 호출하기 전 실패하더라도 소비된 Unity 실행/시간과 원시는 그대로 기록하며 재시작을 무료 준비로 이름만 바꾸지 않는다.

실제 원시 크기 상한과 이를 지키는 직접 기록 경로는 G8 실행 계획에서 기존900/5/5 안의 정확 도구 능력으로 닫아야 한다. 현재 숫자를 임의 발급하거나 기록 실패를 성공 종료로 가리거나 로그 침묵만으로 멈췄다고 판단하지 않는다. 모든 실행/스트림 종료 뒤만 source-after를 기록하고 실제 입력 변경 여부를 대조한다. 빌드 후 exe 자동 실행·별도 새 GUI/Play·프로필 검사는 이 G8 예산에 포함하지 않는다.

## 7. 현재 미결과 B1·CH1 전체 전달 구별

**REQ-C1B-BUILD-006:** 다음 실제값은 현재 `null`/미수용이다. 소스 존재·계획·초안으로 채우지 않는다.

| 필드 | 현재 |
| --- | --- |
| `g3RoomAcceptances`, `sixPrefabApplyEvidence` | `null` |
| `g4Focus35`, `g5Regression44`, `g6Manual12`, `g7Terminal4AndWindow` | `null` |
| `finalBuildSourceCheckpoint`, `finalBuildInputSha256`, `buildEntryMetaGuid` | `null` |
| `activeBuildTargetObserved`, `exactExecutionPlan`, `rootExecutionOwnership`, `astraG8ExecutionApproval` | `null` |
| `actualBuildReport`, `playerInclusionEvidence`, `exeSha256`, `lunaBuildAcceptance`, `astraBuildAcceptance` | `null` |
| `c1cSharedIntegration`, `sameFinalBuildBothBranches`, `chapter1Complete` | `false` |

현재 GUI는 앱 동의와 native 연속 화면/입력 관측 예외 결정 대기다. 이 문서의 빌드 준비를 그 동의·능력·실제 입력 증거로 확대하지 않는다. 관측 보조수단 제작/실행·설치·숨은 API·전역 입력 관측·인증/보안/프로필 우회0이다.

B1의 실제 종료는 기존 보스 뒤 유효 handoff를 C1C가 받을 수 있는 잠긴 상태까지다. 이 빌드를 CH1 선택 양분기·기술2·봉쇄선·세령·CH1 종료가 가능한 전체 완주 빌드로 표시하지 않는다. B1 실제 독립 FINAL 전 C1C 공유 통합0이다. 이후 C1C 통합·필수 회귀·동일 최종 빌드의 Luna 실제 양분기 완주/실패 재도전·한국어 안내·Astra 통합이 닫혀야 M3/REQ-CH1-010·AC-CH1-006의 전체 사용자 전달을 판정한다. B1 G8의 단1회가 미래 C1C 빌드/플레이 횟수를 자동 승인하지 않는다. 다른 컴퓨터로 이동 가능한 프로필 경로 변경도 이3파일 제안 밖이다.

## 8. 인수 기준과 소유 반환

| AC | 연결 REQ·실제 검수 기준 |
| --- | --- |
| AC-C1B-BUILD-001 | REQ001: 신규 C#/meta2와 EditorBuildSettings1 외 제품 변경0, 기존 옛 빌드/scene/prefab/Runtime/설정 보존, 실제 단독 쓰기/반환과 새 입력 계보가 있다. |
| AC-C1B-BUILD-002 | REQ002: 정확 B1 root·유일 메서드·실제 장면/GUID1·Windows x64/Mono·active target·필수 권위/선행을 확인한다. 잘못된 입력·dirty/임시 scene·미수용 방에서 빌드0, 자동 설정 수리/자산 저장/프로필 IO0이다. |
| AC-C1B-BUILD-003 | REQ003: 실제1회 G8의 포함 원시와 코드 실행 없는 Player 메타데이터가 핵심 제품 포함 및 관측기/Editor 역참조·임시 직렬화 유출0을 뒷받침한다. 관측 누락을 부재 성공으로 세지 않는다. |
| AC-C1B-BUILD-004 | REQ004: 실제 exe/동봉 산출물/한국어 안내와 입력·출력 지문/원시 종료가 연결된다. 이전 exe 재사용·기존 출력 덮어쓰기·미확인 조작/완주 설명0이다. |
| AC-C1B-BUILD-005 | REQ005: G3→35→44→수동12→종단4/실제 창→G8 순서와 실제 횟수/900/5/5가 닫힌다. 예비/자동 반복/보충 Player 실행0, 실패 원시와 미소진/소진이 구별된다. |
| AC-C1B-BUILD-006 | REQ006: B1 빌드 한정 결과와 미완료 C1C/CH1 전체 전달이 구분된다. 실제 양분기 완주·동일 최종 빌드·실패 재도전 없이 전체 수용0이고 B1 FINAL/9회/Q0/키세대/프로필/원격 선행을 유지한다. |

기존 REQ-C1B-ROOM-001/EVIDENCE-001/ORDER-001, REQ-C1B-G3OBS-010, REQ-CH1-001/007/008/009/010 및 대응 AC·AC-CH1-006/007을 유지한다. 새로운 여섯 AC는 이 제안의 검수 항목이며 현재 통과 수0이다.

실제 구현을 위한 승인 전에는 정확3 변경·기존 참조/빌드 API·현재 scene/설정과 메타데이터 판독의 호환을 루나가 독립 위험 검토해야 한다. 실행 전에는 실제 최종 소스/선행/입력/출력/도구·원시 크기·소유·예산·정식 권한을 명명해야 한다. 이 Review 자체는 미래 파일/실행 성공이나 현재 source를 최종 입력으로 승인하지 않는다. 작성 종료 뒤 이 문서의 소유를 Root에 반환하고, 다른 제품/native 승인 과업과 독립적으로 보존한다.

## 9. 실행 계약 보강과 정확3 구현의 조건부 한정 승인 — Approved

2026-10-08 실제 아스트라 `gpt-6-astra/ultra`는 아래 조건으로 **§2의 정확3 파일 구현·범위 내 국소 보정 설계만 승인**한다. `boundedImplementationApproved=true`, `nativeExecutionAuthorizedNow=false`, `unityExecutionAuthorizedNow=false`, `buildExecutionAuthorizedNow=false`, `sourceAcceptedNow=false`, `buildAccepted=false`다. 공유 제품에 쓰는 시점과 소유는 §9.1을 따른다. 이 문서 작성자의 실제 제품·도구·실행·다른 원장 쓰기는0이다.

§1~8은 정확23680바이트/SHA `7F17B35FE7FCF8B4A6E3EF50A95A743F3ABD54352FB1AF81FFE9BA908BCDE404`의 검토 원문으로 보존한다. 이 절은 아래 명시 부분만 이후 채택·구체화하며 당시 Review·미실행 기록을 성공으로 고치지 않는다. Root가 전달한 별도 솔 M3 정적 반대검토는 P0/P1=0/0이며 실행 JSON·출력 부재/증거 폴더·실패 보존·자식 정리·현재 입력 결속을 보강할 사항으로 남겼다. 그 근거와 현재 코드/기존 Approved의 아스트라 직접 대조로 정확3의 기술 범위를 판단했다. 루나의 새 구현 검수나 실제 빌드 검증을 받은 것으로 표시하지 않는다.

적용 근거는 현행 AGENTS·운영 모델·ADR-0039/0041/0042/0043 및 Approved 전략 REQ/AC-PROC-011/012다. 같은 기능/소유 범위 안 국소 수정은 한 작업 원장에 누적한다. §8 Review가 제안했던 별도 루나 사전 설계 검토를 정확3 구현의 추가 직렬 문서로 요구하지 않는다. **최종 변경·영향과 실제 실행 도구/정확 계획의 독립 검수는 G8 실행 전에, 실제 결과 독립 검수는 실행 뒤에 유지한다.** 새 공유 API/설정 범위·프로필·저장·권한/횟수 확대는 이 승인에 포함되지 않는다.

### 9.1 공유 구현의 실제 선행과 한 소유

현재 G3 창은 source checkpoint의 fileCount/행을 정확590으로 검사한다. 새 빌드 C#/meta2를 그 전에 넣으면 그 가드와 충돌한다. 따라서 §2의 구현 시점은 **G3 실제590 입력의 여섯 방 수용·정확6 prefab 수용 기록 적용·실행/쓰기 소유 종료 뒤, G4 입력 재결속 전**으로 고정한다. 이 사실은 파일2 추가를 위해 G3 가드를 바꾸거나 현재 관측 소스를 다시 승인할 권한이 아니다.

Root가 닫힌 그 시점의 실제 소스/자산 입력을 한 솔 `gpt-6.1-sol/ultra`에게 인계한 뒤에만 다음 프로젝트 상대 경로에 쓴다: `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1PlayableBuild.cs`, 같은 `.cs.meta`, `ProjectSettings/EditorBuildSettings.asset`. 새 meta의 실제 GUID를 발급하고 장면1 설정 외 다른 키를 보존한다. 현재 Root 소유를 이 문서만으로 솔에게 이전하지 않는다. 소스·도구·GUI 작성자가 겹치면 먼저 해당 소유를 닫는다. 새 보조 실행기 파일·다른 제품 파일·임의 staging 사본을 이3경로 승인에 포함하지 않는다.

현재590 및 최근 v8/v9는 각 M1 시점의 실제 근거다. 빌드 파일 추가·여섯 prefab 적용 뒤의 최종 파일 수/길이/SHA는 실제 checkpoint에서 계산하며 지금 숫자를 발급하지 않는다. 수정된 M1 소스를 과거 B407/50BDD 또는 이 M3 문서의 정적 수용으로 대체하지 않는다. G3 원시와 이후 입력의 연결은 실제 변경3의 영향 판독으로 설명하고 G4/5/6/7이 같은 최종 빌드 입력을 소비하도록 한다.

### 9.2 실행 JSON의 필수 형태 — REQ-C1B-BUILD-002/004/005 구체화

JSON을 실제 실행 권위로 읽는 새 빌드 코드와 외부 실행 수단은 명시 키 존재·정확 자료형·중복/escape 동명 키·잘못된 UTF-8/JSON·정수 범위·허용 길이/깊이를 확인해야 한다. 기본값0/false가 누락을 대신하지 않는다. 버전은 명시 정수1이고, 길이/횟수/상한은 원문 정수 리터럴에서 손실 없이 읽는다. SHA는64자리16진수, 경로는 정규화한 실제 절대 경로와 허용 루트 포함관계/재분석 지점 검사를 통과해야 한다. JSON Number를 부동소수점으로 변환해 정수 지문/길이를 비교하지 않는다. 허용 schema 밖의 실행 권한 필드나 중복 경로는 거절한다.

공통 `FilePin`은 필수 `path:string`, `length:0 이상 Int64 정수`, `sha256:string`이다. 현행 문서/소유/권위 핀은 단일 바이트 읽기로 길이·SHA와 파싱 원문을 일치시킨다. source rows는 정규화 중복0·파일/디렉터리 혼동0·빈 목록0이며 실제 `fileCount`와 같아야 한다. 현재590을 새 빌드 parser의 고정 파일 수로 복사하지 않는다.

| 문서/객체 | 반드시 명시할 필드와 조건 |
| --- | --- |
| 닫힌 G8 실제 계획 | `schemaVersion=1`, `kind`, `gate="G8"`, `executionId`, `executionOwner="Root"`, `projectRoot`, `approvedBuildContract:FilePin`, `runtimeInput:FilePin`, `executor:FilePin`, `unityExecutable:FilePin`, `unityVersion`, `executeMethod`, `launchArguments:string[]`, `scenePath`, `sceneMeta:FilePin`, `target="StandaloneWindows64"`, `backend="Mono"`, `buildOptions:string[]`의 StrictMode/DetailedBuildReport 정확2, `outputRoot`, `outputExe`, `evidenceRoot`, `fileWriters`, `limits`, `prerequisites`, `sourceWritingClosed=true`, `ownershipReturnedTo="Root"`. 각 실제값은 null 없이 닫는다. |
| `runtimeInput` | 실제 최종 checkpoint의 FilePin·파일 수/행, 새 entry/meta·EditorBuildSettings·PlayerSettings/ProjectVersion·Packages/asmdef·scene1/방6/Hub/Ordan/계획4·모든 실제 의존 asset/meta의 핀 또는 checkpoint 내 정확 행 연결, 실제 컴파일/참조/패키지·빌드 도구 입력과 변경 영향. 요약 SHA만 두고 실제 행 집합을 생략하지 않는다. |
| `prerequisites` | G3 방6의 원시/Luna/Astra 수용과 지원 A/B 사례, 정확 prefab6 적용 전후 원장, G4 전체35, G5 전체44의 EditMode/PlayMode, 별도 수동12, 종단 정상1/실패3와 별도 실제 창 관문의 닫힌 결과 핀·AC·실제 입력 적용성. 단순 passed=true만 허용하지 않는다. 앞선 입력과 최종 입력이 다르면 독립 영향/재사용 근거를 명시한다. |
| Root 실행 소유 | 실제 계획/현재 입력/실행기/독립 검수 핀, `executionOwner="Root"`, `rootSingleWriterConfirmed=true`, `sourceWritingClosed=true`, `writingClosed=true`, 실행/출력 경로·실제 모델/설정·기존 실행 종료. 역할/작성자와 미래 실행 수용을 혼합하지 않는다. |
| 별도 Astra G8 실행 승인 | `schemaVersion=1`, `status="Approved"`, 실제 Astra 참여, `finalApproval=true`, `buildExecutionAuthorizedNow=true`, `nativeExecutionAuthorizedNow=true`, 정확 실제 계획/현재 입력/도구/독립 검수/Root 소유 핀, 단일 메서드/루트·예산. 현재 이 승인과 실제값은 없다. |
| `limits` | `unityStartCount=1`, `buildPlayerCallCount=1`, `aggregateLimitSeconds=900`, `terminationLimitSeconds=5`, `outputDrainLimitSeconds=5`, `automaticRetries=0`, `firstFailureStopsRemaining=true`; `maxInputJsonBytes`, `maxJsonDepth`, `maxRawJsonBytes`, `maxLogBytesPerStream`, `maxEvidenceBytes`는 양의 유한 정수로 실제 계획에서 닫는다. 현재 미정 숫자를 기본0 또는 무제한으로 처리하지 않는다. |
| `build-inputs.json` | Root가 실행 직전 만든 필수 `schemaVersion/kind/executionId`, 계획/소유/별도 승인·현재 RuntimeInput·입력 source rows와 권위 핀, 정확 인자/입출력/limits/fileWriters. `source-before.json`과 내용이 맞아야 한다. 실행 자체의 성공·미래 PID·미래 산출 SHA는 담지 않는다. |

계획은 아직 없는 실행 승인의 자기 SHA를 요구하지 않는다. Root가 닫힌 계획→소유/독립 검수→별도 Astra 실행 승인을 받은 뒤 build-inputs를 발급하고, 그 실제 FilePin을 실제 실행 호출과 execution-start가 결속한다. 새 진입점의 manifest 인자는 정확 `-b1BuildInputsPath`와 `-b1BuildInputsSha256` 한 쌍만 허용하며 누락/중복/잘못된 경로/실제 바이트 불일치면 BuildPlayer0이다. 승인 계획은 이 제한된 실행 직전 결속을 명시하고 임의 manifest/인자를 허용하지 않는다. 설명 문서 전체/같은 원장의 변경마다 새 제품 입력 SHA를 발급하지 않으며 README↔출력원장↔승인의 자기/상호 SHA 순환0이다.

위 스키마는 구현할 규약이다. 실제 미래 JSON 파일의 작성·실행이 완료됐다는 뜻이 아니다. 원시 결과는 모든 필드를 요구하되, 관측하지 못한 PID/exitCode/report/산출 핀은 허용된 실패 상태에서 **null과 그 사유**로 남긴다. null→0, 부재 파일→빈 성공 report, 관측되지 않은 false→입증된 부재로 투영하지 않는다.

### 9.3 증거 폴더와 빌드 출력 폴더의 구별

실행 전 Root는 `output/chapter1-playable-r1/evidence/b1-current-source-build-r1/`와 `output/chapter1-playable-r1/build/b1-windows-r1/`가 모두 부재인지 확인한다. 첫 경로는 **Root가 준비하는 증거 폴더**, 둘째는 **BuildPlayer 직전까지 부재해야 하는 제품 출력 폴더**다. 이 둘을 같은 Exists 거절 조건으로 묶지 않는다. 기존 경로가 있으면 지우거나 다른 이름으로 자동 반복하지 않고 이전/부분 시도인지 원문과 함께 정지한다.

별도 실행 승인 뒤 Root만 증거 폴더를 새로 예약하고 `source-before.json`, `build-inputs.json`을 CreateNew로 닫는다. Root의 실제 실행 수단이 stdout/stderr 전용 파일을 CreateNew로 열고 Unity에는 정확 `unity.log`만 지정한다. 실제 PID/StartTicks/EXE/보유 handle·단조 시작 기준을 얻은 Root가 `execution-start.json`을 CreateNew로 닫는다. 빌드 진입점은 Root의 닫힌 세 JSON과 허용 로그만 있는 증거 폴더를 확인한다. 실행-start가 아직 없거나 닫힌 근거가 불완전하면 BuildPlayer를 부르지 않는다. 증거 폴더의 존재 자체를 허가로 쓰거나 진입점이 다시 생성/삭제하지 않는다.

진입점이 허용하는 기존 이름은 위 Root 파일3와 `stdout.log`, `stderr.log`, `unity.log`뿐이다. 실제 파일 유형/경로·Root 소유·닫힌 JSON 지문을 대조하고 다른 내용/실행 출력이 있으면 거절한다. 진행 중 로그는 닫힌 핀이라고 부르지 않는다. 제품 출력 `b1-windows-r1/`은 여전히 부재해야 하며 해당 경로의 단독 소유를 Root가 보장한다. 모든 사전검사가 끝난 뒤 진입점만 새 출력 폴더를 마련하여 정확 exe 경로로 단1회 BuildPlayer를 호출한다. 기존 exe/폴더 덮어쓰기·복사·정리 후 재시도0이다.

| 원시 파일 | 단일 작성자와 기록 시점 |
| --- | --- |
| `source-before.json`, `build-inputs.json` | Root, Unity 시작 전 실제 최종 입력을 읽어 닫음 |
| `execution-start.json` | Root 실행 수단, 실제 생성/소유 확인 뒤 실제 프로세스 신원·계획/입력·시각 기록 |
| `stdout.log`, `stderr.log` | Root의 각 전용 스트림 수집기, 원시 바이트를 한 방향으로 기록하며 회수까지 소유 |
| `unity.log` | 같은 Unity 프로세스의 지정 로그 출력, Root가 위치/크기/최종 폐쇄를 관측 |
| `build-report.json` | 새 Editor 진입점, 가드/호출/반환·오류를 한 번 기록. 실제 BuildReport가 없으면 reportReturned=false/summary=null과 실제 예외를 남김 |
| `player-inclusion.json` | 같은 진입점, 실제 성공 산출의 메타데이터/패킹 근거 판독. 전 단계 실패 시 부재와 이유를 후속 end에서 명시 |
| `build-outputs.json` | 같은 진입점, 생성된 부분 또는 정상 출력의 실제 경로/길이/SHA·manifest/plan 결속. 실패 산출을 정상 전달 목록으로 승격하지 않음 |
| `source-after.json`, `execution-end.json` | Root, 아래 프로세스/스트림 정리 조건에 따라 작성 |

유효 증거 루트를 확인하기 전 거절하거나 진입점에 도달하지 못했다면 Editor 원시가 없는 것이 정상 실패 경계다. Root 로그/end에 부재 이유를 보존하고 없는 report를 사후 합성하지 않는다. 예외/쓰기 실패 시 최초 오류의 종류·단계·원문·실제 UTC/단조 시각과 실제 호출 여부를 보존하며 이후 정리 오류는 별도 목록에 추가한다. 최초 오류를 마지막 정리 오류로 덮지 않는다. 진행 중 원시를 읽기 편하도록 재작성·절단·바이트 인코딩 변환하지 않는다.

### 9.4 결과 필드와 소스 불변의 의미

`build-report.json`은 `schemaVersion/kind/executionId/inputManifestSha256`, `guardPassed`, `buildPlayerInvoked`, `buildPlayerCallCount`, `reportReturned`, `summary`, `steps`, `files`, `packedAssets`, `firstFailure`, `writerErrors`를 필수로 둔다. summary는 실제 BuildReport가 있을 때의 result/platform/outputPath/totalSize/totalTime/errors/warnings를 실제 타입으로 담는다. result 성공과 정수 native exit0는 서로 별도로 확인하며, 하나만으로 전체 수용하지 않는다.

`player-inclusion.json`은 실제 검사 대상 DLL/scene/asset/meta 핀, 사용한 기존 Mono.Cecil 핀, 읽은 형식/멤버/assembly 참조와 핵심 게임 타입 포함, 관측기/Editor 참조와 임시 직렬화 유출의 실제 결과·누락·예외를 담는다. `build-outputs.json`은 실제 출력 root와 정규화 중복 없는 rows/count/길이 합계·상태를 담고 Root가 정리 뒤 다시 대조한다. 빌드 원장에 자기 자신의 SHA를 포함하지 않는다.

`execution-end.json`은 `schemaVersion/kind/executionId`, 실제 시작/종료 UTC·단조 경과, main process 신원/정수 exit 또는 null, `unityStarted`, `buildPlayerInvoked`, `buildPlayerCallCount`, `firstFailure`, `cleanupErrors`, `childOwnership`, `allOwnedProcessesClosed`, `allStreamsDrained`, `streamCloseResults`, `rawFiles`, `missingExpectedFiles`, `sourceAfterCaptured`, `sourceUnchanged`, `automaticRetries=0`, `accepted=false`를 필수로 한다. Unity 로그 무응답을 종료 관측으로 쓰지 않는다. 첫 실패 뒤에는 후속 제품 검사/실행0이며 필요한 정리·실제 실패 원시 봉인만 한다.

모든 소유 프로세스 종료와 출력 회수가 확인된 뒤 Root만 source-after를 생성하여 최종 RuntimeInput의 실제 Assets/Packages/ProjectSettings 전체 파일 집합과 길이/SHA를 비교한다. 빌드 도구가 의도하지 않은 자산/설정 쓰기를 하면 실패이며 그 차이를 수리하지 않는다. Library/Temp/빌드 산출은 제품 source 스냅샷에 혼합하지 않고 자기 출력 영역으로 기록한다. 정리가 미확인되면 source-after를 만들지 않고 sourceAfterCaptured=false/sourceUnchanged=null로 남긴다. 비교 보고 오류를 이유로 제품 빌드를 재실행하지 않고 실제 원문에서 한정 판독한다.

README.ko.md는 실제 G8와 Luna 결과 검수 뒤 Root가 승인된 실제 조작/범위를 확인하여 같은 빌드 폴더에1회 작성한다. 실행 종료 원시를 README 추가 뒤 수정하지 않는다. README의 최종 지문과 동봉 파일 목록은 같은 전달 작업 원장에 연결하고 빌드 입력/컴파일 입력으로 소급하지 않는다. 실제 CH1 전체 완주 또는 이식 가능한 저장 경로가 미입증이면 B1·고정 로컬 개발 환경 한계를 명시한다.

### 9.5 소유한 자식 프로세스까지의 한정 정리

G8의900초에는 시작/가져오기/필요 컴파일/단1회 BuildPlayer/포함 판독·정상 봉인 전체가 들어간다. Unity나 빌드 내부의 실제 자식 compiler/worker는 같은 실행의 소유·시간에 포함하며 따로 추가900초를 받지 않는다. 내부 자식 수를 별도 Player 빌드로 허위 집계하지 않되 실제 생성 수/역할·사용 시간을 관측한다. Root가 추가 Unity/예비 compiler를 직접 시작하는 권한은 없다.

Root 실행 수단은 메인 PID만이 아니라 **이번 실행에서 생성된 자식의 소유를 생성 시점부터 유지하고 종료까지 확인할 수 있는 경계**를 실제 실행 계획/도구 검수에서 입증해야 한다. Windows 공개 Job Object 같은 수단은 후보이며 현재 설치/구현/작동을 가정하지 않는다. 사용한다면 탈출 조건·중첩 job·기존 프로세스와의 구분·정확 소유 handle·종료/활성0 조회를 함께 검수한다. 단순 PID 재검색이나 부모 종료만으로 자식 폐쇄를 선언하지 않는다. Microsoft 문서는 자식 귀속의 예외·탈출 설정과 종료/집계 기능을 구별하므로 그 한계를 그대로 반영한다. [Windows Job Object 공식 문서](https://learn.microsoft.com/en-us/windows/win32/procthread/job-objects)

최초 실패/900초 도달 시 원래 보유한 소유 프로세스 묶음만 종료하고 **전체 묶음 합산5초** 안 종료를 확인한다. 자식마다5초를 반복 부여하지 않는다. 이어 stdout/stderr·Unity 로그/Writer 폐쇄·마지막 바이트 회수를 **합산5초** 안 확인한다. 프로세스명 전체 종료·기존 Editor/Hub/서비스/다른 사용자의 프로세스 편입/종료0이다. stdout와 stderr는 동시 회수해 한 스트림 대기로 다른 스트림을 막지 않는다. 종료 코드와 종료/회수 성공은 별도 실제 관측으로 기록한다.

자식 소유를 보장할 수 없거나 남은 프로세스/로그 writer의 폐쇄가 미확인되면 자동 반복·source-after/빌드 성공 수용0이다. 이미 확보한 원시와 정확 실패 상태를 보존하고 Root에 미종료 소유 정보를 반환한다. 예외적인 추가 관리가 필요하면 그 정확 소유·행위를 별도 판단하며 이번5/5를 조용히 연장하지 않는다. 새 실행기 제작/실제 능력 확인은 아직 미결이고 정확3 제품 파일 승인으로 외부 도구 권한까지 발급하지 않는다.

### 9.6 묶음 검사·미결과 승인 경계

솔은 정확3 안 구현과 국소 보정을 이어가며 경로/필수 JSON 필드·실제 타입/배열0·1·복수/중복키·상한·Root 증거 폴더 대 제품 출력 부재·첫 오류 보존·메타데이터 판독의 정상/거절 조건을 같은 원장에 누적한다. 새 예비 Unity/Player/build/native compiler 실행 예산은0이다. 파일/문서 읽기·정적 검사로 확인한 것과 실제 코드 실행 검사를 구별한다. 실제 컴파일은 G4/5의 기존 실행 계획에서 해당 최종 입력·조건을 결속한 뒤 확인하며, 불가능한 경우 별도 유한 검사 예산의 정확 필요를 판정한다. M1 최소 컴파일의 과거 또는 새 예산을 가져오지 않는다.

공유 구현 완료 뒤 Luna는 정확3 delta·나머지 보존·참조/scene/설정·JSON/원시/실패·현재 선행 적용성과 실행 수단을 한 묶음으로 검수한다. 단순 문서 보정마다 전체 소스 검수·회귀를 반복하지 않는다. 기존 G3→정확6 적용→G4 전체35/900→G5 전체44 두 모드/합산1800→별도 수동12→종단4/실제 창→G8 빌드1/900 순서를 유지한다. 첫 실패/단회/자동재시도0이므로 실패 뒤 남은 시간이나 아직 호출하지 않은 BuildPlayer를 무료 재시작 권한으로 쓰지 않는다.

| 실행 승인 전에 닫을 미결 | 현재 판단 |
| --- | --- |
| G3 실제 수용/정확6 적용·공유 소스 소유 종료 | 미완료. 정확3의 제품 쓰기 선행이며 현재 인계/구현0 |
| 최종 RuntimeInput/파일 수/새 entry GUID·변경 영향·G4~G7 | null/미실행. 현재590이나 M1 지문으로 대신하지 않음 |
| 실제 Root G8 실행 수단·자식 소유/폐쇄·스트림/크기 감시 능력 | null/미구현. 정확 파일/도구 능력을 같은 기능 묶음의 실제 계획에서 닫아야 하며 현재 외부 helper 제작·실행 권한0 |
| raw JSON/로그/전체 원시의 정확 크기 상한 | null. 양의 유한값과 해당 수집 능력이 정확 실제 계획·독립 검수에 명명되기 전 실행0 |
| 별도 실행 JSON/Root 소유/실제 도구 심사/Astra G8 승인 | null. 이 조건부 구현 승인으로 대체하지 않음 |
| 실제 BuildReport/Player 포함·Luna 결과/Astra 수용 | null/미수용. 위 코드 구현의 자기 검증을 독립 수용으로 확대하지 않음 |

REQ-C1B-BUILD-001~006 및 대응 AC는 그대로이며 이 절의 구체 필드/소유·실패 조건을 각 AC002/004/005의 검사 기준에 포함한다. 새 시험 횟수·새 정규 selector·필수 AC 축소0이다. source/도구·메서드 경계 밖 문제는 이 기능 묶음에서 정확 영향으로만 판단한다. G8의 성공은 B1 한정 결과이며 B1 독립 FINAL 전 C1C 공유 통합0·9회/Q0/세대/키/프로필/원격 관문과 이후 같은 최종 CH1 빌드의 양분기 실제 완주 요구를 유지한다.

현재 종료 조건은 이 한 파일의 보강·조건부 범위 승인·쓰기 종료와 Root 소유 반환이다. 제품/도구 구현 및 실제 실행의 완료 조건은 위 실제 증거에 남겨 둔다. 이 문서는 자신의 최종 SHA나 미래 실행값을 본문에 포함하지 않으며 다른 원장/Approved 원문을 수정하지 않는다.

## 10. M3 Root 실행 수단의 소유·크기·능력 확인 후보 — Review

이 절은 실제 Root `gpt-6.1-sol/medium`의 후속 설계 후보다. §9의 정확3 조건부 제품 승인과 현재 실행 권한0을 그대로 유지한다. 새 외부 도구·시험·빌드 권한을 발급하지 않는다. 소비된 §9까지의45272바이트/SHA `E58A984ED0ABFEC5276E2BEE8D694FCDF51E6914CBF3A85E96D41106F93BE5C4`는 원문 접두사로 보존한다. 단순 문서 지문 변경 때문에 기존 제품 승인이나 실행 결과를 허위로 무효화하지 않으며, 새 도구 범위는 이 절의 별도 명시 채택 뒤에만 구현한다.

사용자 산출물은 빌드 실패·시간 초과 때 이 작업이 새로 만든 프로세스만 닫고 원시 증거를 보존하는 현재 소스 Windows 빌드다. 후보 담당은 한정 솔의 도구 구현, 루나의 묶음 독립 검수, 아스트라의 정확 범위 승인, Root의 실제 단회 실행이다. 현재 제품 파일·장면·프로필·Unity·네이티브 시험 쓰기/실행은0이다.

### 10.1 공개 Windows API의 제한과 후보 방식

전용 비상속 private Job을 만들고 이 작업의 새 대상만 결속한다. 기존 프로세스를 이름/PID 목록으로 찾아 편입하거나 종료하지 않는다. `BREAKAWAY_OK`·`SILENT_BREAKAWAY_OK`를 켜지 않는다. 부모 Job·중첩·UI 제한 및 서비스/WMI 생성 경로는 실제 환경에서 보장되지 않는다. Job 통지만으로 전체 생성/종료를 증명하지 않으며 실제 집계와 보유 handle을 대조한다. 이 한계는 [Windows Job Object 공식 문서](https://learn.microsoft.com/en-us/windows/win32/procthread/job-objects)에 근거하며 실제 Unity의 호환 성공은 미관측이다.

소유 확보 후보는 전용 Root 운영체제 스레드의 `CreateProcessW(DEBUG_PROCESS | CREATE_SUSPENDED)`와 생성 전에 설정한 `STARTUPINFOEX`의 Job 목록이다. Job의 출생 결속을 입증할 수 없으면 `ResumeThread`/Unity 실행0으로 실패한다. 생성 호출 전부터 단조 시계를 시작한다. `CREATE_PROCESS_DEBUG_EVENT`에서 user mode 진행 전에 실제 process handle을 별도 비상속 handle로 복제해 PID·생성 FILETIME·실제 executable·Job 소속을 기록한다. `lpImageName`의 null/문자열만을 출처로 쓰지 않는다. [디버그 사건 공식 문서](https://learn.microsoft.com/en-us/windows/win32/debug/debugging-events), [시작 속성 공식 문서](https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-updateprocthreadattribute)

debug handle에 메모리/레지스터 접근 권한이 포함되더라도 이를 제품 상태 관측/변조 수단으로 사용하지 않는다. `ReadProcessMemory`·`WriteProcessMemory`·`GetThreadContext`·`SetThreadContext`·기존 프로세스 attach·코드 주입0이다. 필수 생성/계속·소유 종료와 자기 원시 수집만 이 후보 범위이며 다른 앱의 GUI/입력 동의와 구별한다.

`WaitForDebugEvent`와 `ContinueDebugEvent`는 생성한 동일 전용 스레드에서 처리한다. 새 디버그 연쇄 때문에 출생 사건이 누락되는 자식은 성공으로 추정하지 않는다. Job의 실제 총 생성 수와 출생별 보유 handle의 집합이 일치하고, 각 보유 handle의 종료·실제 코드와 Job 활성0까지 함께 관측해야 폐쇄다. 종료 사건 뒤 운영체제가 닫는 원본 debug handle과 Root가 닫는 복제 handle·image file·pipe·Job·시작 속성 메모리의 소유를 구분한다. 디버그 예외를 일괄 성공 처리하지 않고 초기 loader breakpoint·첫 기회/두 번째 기회 예외 정책을 정확 도구 계획에 고정한다. [사건 대기 공식 문서](https://learn.microsoft.com/en-us/windows/win32/api/debugapi/nf-debugapi-waitfordebugevent)

이 방식은 디버그 상태·스케줄·heap·Unity/Bee의 작동에 영향을 줄 수 있다. `CreateProcessW` 호출 자체의 반환 시간을 단조 시계만으로 강제할 수 있다고 주장하지 않는다. 생성 전 Job 결속·외부 감시·동일 소유 폐쇄를 보장할 구체 구현이 없거나 운영체제 호출이 예산 안 반환하지 않으면 능력 확인 실패이며 시간 연장·기존 프로세스 종료·자동 반복0이다. 이 문서에서 실제 debugger/Job/프로브/설치/보안 설정 변경은0이다.

### 10.2 정확 후보 도구 범위와 원시 구조

구현 후보 파일은 `output/chapter1-playable-r1/tools/b1-current-source-build-root.v1.ps1`와 `output/chapter1-playable-r1/tools/b1-current-source-build-native.v1.cs` 정확2다. 같은 C# 파일의 도구 전용 한정 시험 모드는 Unity·제품 assembly·프로필·장면을 로드하지 않고 자신의 부모/자식만 만들도록 설계한다. 새 executable/DLL/PDB와 능력 확인 원시는 `output/chapter1-playable-r1/preparation/b1-build-root-capability-r1/` 아래 CreateNew로만 기록한다. 실제 빌드 원시는 최종 승인된 `evidenceFolderRoot` 아래 별도 고정 실행 폴더를 쓴다. 외부 helper 다운로드·설치·드라이버·별도 유료 API·일반 사용자 프로필·컴퓨터 사용 앱 동의 우회·다른 앱의 화면/입력 관측0이다.

Root 원장 한 곳에 도구 정확 입력·작업 소유·준비 실패·실제 능력 확인·최종 G8 호출을 누적한다. 범위 안 비실행 정적 보정마다 새 승인/보고 문서를 만들지 않는다. 실제 실행 입력은 승인 개정과 원본 파일 길이/SHA를 직접 결속하고 원장의 미래 최종 SHA를 재귀로 고정하지 않는다. 소유/실행/도구 JSON의 숫자는 실제 정수·유한 범위, bool은 실제 bool, 필수 null/누락/문자열 대체·중복키는 거절한다. 미관측 생성/종료/코드는 null로 보존하며 기본0·성공으로 채우지 않는다.

후보 원시 상한은 실행 JSON1 64KiB, 프로세스 사건1 8MiB, stdout1 32MiB, stderr1 32MiB, Unity 지정 log1 128MiB, 결과/요약 JSON1 8MiB, 전체 원시256MiB다. 제품 `BuildPlayer` 산출물 크기는 이 원시 예산과 구분하며 제품 결과/파일 목록은 기존 §9의 BuildReport 원시와 정확 연결한다. 최대 프로세스 세대는256이다. 상한에 닿으면 새 기록/대상 진행을 중단하고 기존 최초 실패와 소유 원시를 보존한다. 정규 결과가 상한 안 들어가는지 실제 준비로 확인하기 전 G8 실행0이다. 누적/단일 파일 상한을 확인한 뒤 쓰며 메모리 전체 복사나 무제한 줄/스트림 버퍼로 상한을 우회하지 않는다.

stdout/stderr는 상속 handle 목록에 명명한 자기 pipe만 사용하고 반대 끝·부모 writer 사본을 정확히 닫아 동시에 회수한다. Unity log는 명명된 자기 새 경로만 수집한다. EOF·writer 폐쇄·최종 바이트 수·SHA 관측을 원시별로 구분하며 안정 길이만으로 writer 종료를 선언하지 않는다. `CREATE_NO_WINDOW`가 GUI Unity까지 숨긴다고 가정하지 않고 실제 시작 창 정책은 사용자에게 불필요한 창을 열지 않도록 정확 구현에 고정한다.

### 10.3 능력 확인의 유한 후보와 실제 G8 분리

다음은 요청할 유한 기술 예산의 후보이며 현재 승인/소비0이다. 도구 컴파일은1회/60초 후보다. 능력 확인은 정상 부모+자식 종료, 부모 선종료 후 자식의 stdout/stderr 보유, 시간 초과 시 자기 부모+자식 종료의 정확3사례/각10초/합산30초 후보다. 각 사례 종료는 전체 자기 묶음 합산5초, 출력 회수는 합산5초다. 전체 준비 단계는 컴파일60+사례30+폐쇄15+회수15의 최대120초 후보이며 자동 재시도/첫 실패 뒤 남은 사례 실행0이다. 실제 controller/launcher/subject의 시작 수·명명된 호출·자기 자식의 생성 수와 종료 코드를 정확 도구 계획에 고정한 뒤만 아스트라가 실행 권한을 판단한다. 암묵적 `Add-Type` 컴파일/추가 부모·손자/자식별 추가5초는0이다.

프로브 성공은 해당 도구 전용 세 사례의 관측일 뿐 Unity/Bee/라이선스 서비스·SDK·G8 실제 호환 성공이 아니다. 실제 G8은 기존1회/900초를 유지한다. 최초 실패 또는900초에서 이 작업 소유 묶음만 종료하며 합산5초, 이어 전체 원시 회수 합산5초를 적용한다. 부분 생성·누락 handle·소유 집계 불일치·미종료·누락/초과 원시·배출 실패에는 source-after/빌드 성공 수용0이다. 새 관리가 필요하면 정확 현재 소유와 제한을 반환하고 자동 연장/새 실행0이다.

### 10.4 제출과 실제 미결

이 절의 종료는 동일 M3 계약의 후보 보강·독립 영향 판독·아스트라 범위 판정이며 실제 빌드 완료가 아니다. 도구2 구현은 정확 후보 채택과 Root 단일 작성자 인계 이후, 실제 능력 확인은 최종 도구/입력·Root 소유·별도 유한 실행 승인과 실제 도구 권한 심사 이후다. G8은 G3 실제 방 수용·정확6 적용·제품3 구현 및 소유 종료·기존 G4~G7·현재 최종 RuntimeInput·실제 SDK/도구 능력·한정 승인 전0이다. 미래 파일 길이/SHA·프로세스 수·실제 결과는 null/미발급이며 B1 FINAL·C1C 공유 통합·CH1 완주·9회·Q0·키·세대·원격 관문은 유지한다. REQ-C1B-BUILD-004/005/006와 AC-C1B-BUILD-004/005/006의 소유·검사·실패 증거를 보강하는 후보이며 기존 기준을 축소하지 않는다.

### 10.5 실제 아스트라 반대판독 반영과 좁은 대안 — Review 유지

실제 아스트라 `gpt-6-astra/ultra`의 읽기 반대판독은 §10의 방향과 다음 기술 미결을 구별했다. 인간의 새 제품 선택으로 넘길 사항은 확인하지 않았으며 도구/프로브/실행 승인0이다. 아래는 같은 기능 묶음의 후보 보정이다.

1. `STARTUPINFOEX` 사용에는 `EXTENDED_STARTUPINFO_PRESENT`가 필요하다. 생성 플래그 후보는 `DEBUG_PROCESS | CREATE_SUSPENDED | EXTENDED_STARTUPINFO_PRESENT`다. 반환된 process handle의 Job 소속을 확인하고 정확1회 Resume한 뒤 동일 OS 스레드에서 사건을 처리한다. 종료 사건의 Continue 성공 뒤 복제 handle이 signaled인지 관측하고 `LOAD_DLL` 사건의 hFile도 한 소유자가 닫는다.
2. 자기 C# 도구가 아직 컴파일되지 않은 동안 자신의 컴파일 소유를 보장했다고 주장할 수 없다. debugger/controller의 생성/사건 처리가 멈출 때 자기 Job만 종료할 별도 감시자의 시작·handle 공유·KILL_ON_JOB_CLOSE·최종 회수와 bootstrap 경계는 미결이다. 기존 프로세스 attach/종료로 보정하지 않는다.
3. §10.3의120초에는 컴파일의 종료5/회수5·controller 시작/종료도 포함해야 한다. 컴파일을 유지한다면 단계60 안에 해당10초를 예약하며 전체 단조 시계는 외부 controller를 만드는 직전부터 시작한다. 실제 compiler/controller/launcher/subject 부모·자식의 정확 시작 수가 발급되기 전 실행0이다. 예상 timeout 사례는 정해진 시간에 자기 대상만 종료/회수되는 것을 검사하며 예상 밖 최초 실패는 남은 사례를 멈춘다.
4.256은 실행 전체의 대상 출생 세대 수이며 동시 활성 수가 아니다. 한도를 넘는 첫 출생은 즉시 소유를 확보해 안전하게 닫되 성공으로 합산하지 않고, 종료/실패 원시를 위한 예약 공간과 handle 여유를 별도로 고정한다. Job 집계에는 결속 실패/한도 위반도 포함될 수 있으므로 집계와 실제 출생/handle 불일치는 그대로 실패다. 원시 행 삭제로 숫자를 맞추지 않는다.
5. helper의 stdout/stderr/사건 기록은 쓰기 전 상한을 적용할 수 있지만 Unity가 직접 쓰는 log는 사후 크기 감시밖에 없을 수 있다. 직접 writer를 그대로 쓸 때 이를128MiB의 엄격 사전 쓰기 보장으로 표시하지 않는다. 실제 승인된 배관 또는 감시 간격·최대 초과 보존량·전체256MiB와 실패 요약 예약을 고정하는 정확 방식이 미결이다. 상한/첫 실패 뒤에도 소유 종료를 위한 Continue/EXIT 사건 처리·회수는 계속하며 정상 사례 진행만 중단한다.
6. 최초 loader breakpoint의 정확 조건만 `DBG_CONTINUE` 후보이고 일반 첫 기회 예외는 `DBG_EXCEPTION_NOT_HANDLED`로 제품 처리기에 전달한다. 두 번째 기회 예외는 실패로 보존한다. 실제 정책 표와 제품의 managed 실패 구별이 닫히기 전 일반 예외를 삼키거나 모든 예외를 성공으로 해석하지 않는다.

bootstrap을 줄이는 대안은 이미 번들에 제공된 Python 표준 `ctypes`를 쓰는 PS1+PY 정확2다. 실제 `load_workspace_dependencies`는 `C:/Users/me/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe` 경로를 반환했다. 파일 실행·설치·패키지 추가0이며 런타임 버전/길이/SHA는 아직 실행 입력에 결속하지 않았다. 이 대안에서는 §10.2의 C# 후보 대신 `tools/b1-current-source-build-native.v1.py`를 제안하고 도구 컴파일 후보1을0으로 줄일 수 있다. 실제 controller1과 별도 감시자1은 같은 PY의 명명한 모드로만 실행하고 자기 private Job의 복제 handle만 공유하는 안이다. 정확 생성/소유 순서·총 시작 수·controller 정지 시 감시자의 실제 폐쇄/회수·helper 외부 Root의 최종 종료 관측은 미결이다. 기존 제안의120초를 새로운 free retry나 별도 실행으로 쓰지 않는다. 이 대안의 명시 채택 전 도구 파일 생성·Python 실행·프로브·Unity·GUI·메모리/입력 관측은0이다.

### 10.6 Python 정확2 대안의 생성·감시·원시 경계 — Review 후보

이 절은 실제 한정 솔 `gpt-6.1-sol/ultra`의 기술 설계 후보다. 기존59129바이트/SHA `9C1CE1FD728F3821D1D9485602D94499236E4FCC45A0D3080C5BD7EB2EA91E76` 전체 접두사를 그대로 보존한다. §10.2~10.5의 C# 정확2·컴파일1/60초·준비120초는 당시 후보 역사이며 이 대안을 실제 시행한 것으로 고치지 않는다. 여기서 제안하는 범위는 PS1+PY 정확2·컴파일0·별도 유한 준비75초다. §9의 조건부 제품 정확3 승인과 새 도구/실행 승인0을 구별한다. REQ/AC-C1B-BUILD-004/005/006의 실행 소유·상한·실패 증거만 구체화하며 제품 요구·정규 선택자·검사 회수·Unity/Player 빌드 수를 추가하지 않는다.

**정확 파일·기존 런타임과 읽기 사실.** 제안 구현 파일은 `output/chapter1-playable-r1/tools/b1-current-source-build-root.v1.ps1`, `output/chapter1-playable-r1/tools/b1-current-source-build-native.v1.py` 정확2다. PS1 호스트의 명명 경로는 `C:/Windows/System32/WindowsPowerShell/v1.0/powershell.exe`, Python은 `C:/Users/me/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe`다. 이번 읽기로 두 파일의 존재/파일 길이495616/106448바이트를 관측했으나 실행 버전·비트 수·실제 동작은 미관측이다. 이름의 `v1.0`을 실제 PowerShell 버전으로 해석하지 않는다. 구현/실행 입력에서는 실제 PS5.1·64비트 호스트/64비트 Python·파일 길이/SHA·표준 라이브러리 위치를 직접 결속하고 불일치/미결이면 시작0이다. 현재 PATH 검색·다른 Python·런타임 교체·설치·패키지 추가·C#/Add-Type·DLL/PDB·pyc 작성·제품 assembly 로딩은0이다. PY는 `-I -S -B`와 정확 파일/고정 역할 인자로만 시작하여 사용자 site·추가 모듈·bytecode 쓰기를 배제하고, `ctypes`와 기존 표준 라이브러리만 사용한다. 원시/준비 출력은 기존 `output/chapter1-playable-r1/preparation/b1-build-root-capability-r1/`의 새 단회 하위 실행 폴더로 제한하며 CreateNew·재분석 지점/허용 루트 대조를 적용한다.

PS1은 `System.Diagnostics.Process.Start`의 `UseShellExecute=false`, `CreateNoWindow=true`, 자기 stdin/stdout/stderr 방향별 전용 배관을 명시하여 watcher1·controller1을 만들고, 반환 당시의 각 `Process` 객체/실제 `SafeProcessHandle`을 폐쇄까지 소유한다. `GetProcessById`, 프로세스명 검색, PID만으로 새 객체를 얻어 종료, `Kill(true)`로 외부 트리를 검색, 일반 셸을 거치는 명령 실행은0이다. 종료는 생성 당시 보유한 바로 그 `Process`/handle의 `Kill()` 및 유한 `WaitForExit(milliseconds)`/실제 종료 코드로 확인한다. 이 .NET 경로의 실제 PS5.1 호환·handle 보유/접근권·숨김·동시 회수는 아직 실행되지 않았으며 단순 API 이름으로 성공 발급0이다.

**한 세션·한 Job·생성 전 두 단계 대조.** 준비는 같은 PY의 `watcher`, `controller`, `subject-parent`, `subject-child` 고정 역할만 사용한다. controller1/watcher1을 세 사례 동안 유지하고, 사례마다 새 helper를 만들지 않는다. controller는 비상속·이름 없는 준비 Job1을 만들고 `JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE=0x00002000`만 설정한다. BREAKAWAY 두 플래그·기존 프로세스 편입·일반 서비스/WMI 생성·타 작업 PID 조회/종료·기존 debugger attach는0이다. 자신의 새 대상용 Job과 도구 상위 환경의 기존 Job을 혼동하지 않는다. 이 부모/중첩 Job의 실제 적합성은 초기 능력 확인의 거절 조건이다.

1. PS1이 watcher1을 먼저 만들고 실제 process handle·PID·생성 FILETIME·정규 executable 경로·해당 파일 핀을 고정한 뒤 controller1을 만든다. PS1의 고정 전용 양방향 채널로 실행 ID·단회 nonce·두 helper의 실제 세대·상호 역할을 전달한다. 일반 입력에서 임의 PID/handle/명령을 받는 입구는 두지 않는다. Python이 `OpenProcess`를 쓰는 범위는 PS1이 방금 생성해 실제 handle을 보유한 이 두 helper와 자기 PS1 호스트의 좁은 원본 신원 대조뿐이다. 필요한 `PROCESS_DUP_HANDLE | PROCESS_QUERY_LIMITED_INFORMATION | SYNCHRONIZE` 권한만 열고, 실제 handle의 PID/FILETIME/executable가 PS1 원본과 모두 일치하지 않으면 닫고 거절한다. 운영체제 스레드/프로세스 이름 문자열만으로 소유를 발급하지 않는다.
2. controller는 watcher의 검증된 실제 process handle을 목적지로 `DuplicateHandle`하여 준비 Job의 비상속 복제 handle을 watcher에 전달한다. watcher는 자기 로컬 handle로 Job 설정·집계0을 읽고, `IsProcessInJob(GetCurrentProcess(), job)`가 false인 것을 확인한다. 이어 검증된 controller process handle을 목적지로 그 Job을 다시 복제한다. controller는 자기 로컬 원본 Job handle과 되돌아온 handle에 `CompareObjectHandles`를 호출하여 같은 커널 Job 객체임을 대조하고, 왕복 임시 handle을 정확1회 닫는다. 서로 다른 프로세스의 숫자 handle끼리 비교하거나 Job 이름/설정값 일치만으로 동일 객체를 발급하지 않는다. 같은 객체 대조의 공개 API 근거는 [CompareObjectHandles 공식 문서](https://learn.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-compareobjecthandles)이며 실제 왕복 성공은 미관측이다.
3. watcher가 단회 준비 완료·해당 Job/실행 ID·deadline·자기 Job 밖 상태를 확인하고 controller와 PS1이 모두 그 응답을 수신하기 전에는 **대상 `CreateProcessW` 호출0**이다. 초기화5초 안 어느 단계든 실패하면 대상0이며, 이미 만든 helper/handle만 최종 종료5/회수5 예약 안에서 닫는다. 허용되지 않은 상속/중첩·ABI·Job 대조 실패를 재생성으로 보정하지 않는다.
4. 각 사례 시작 전 이전 사례 ActiveProcesses=0·모든 보유 대상 handle signaled·해당 스트림 EOF/폐쇄·집계 delta가 닫힌 것을 대조한다. watcher는 같은 Job의 다음 사례 번호·현재 전체 deadline을 확인한 뒤 진행 응답을 준다. 첫 정상 사례 Job TotalProcesses=2, 두 번째 종료 뒤4, 세 번째 종료 뒤6과 각각의 증가량2를 요구한다. PID가 같더라도 생성 FILETIME/복제 handle이 다른 세대를 별도 행으로 기록하며 기존 세대를 재사용하지 않는다. 실제 집계가 이 숫자와 다르면 실패다. 값이2/4/6일 것이라는 예상으로 실제 행을 채우지 않는다.

**대상 생성·debug 사건과 실제 종료.** controller의 한 전용 OS 스레드가 대상 부모를 `DEBUG_PROCESS=0x1 | CREATE_SUSPENDED=0x4 | EXTENDED_STARTUPINFO_PRESENT=0x00080000 | CREATE_NO_WINDOW=0x08000000`의 플래그 조합으로 생성한다. 같은 OS thread ID를 생성 직전/각 `WaitForDebugEvent`/각 `ContinueDebugEvent`에 기록한다. `STARTUPINFOEXW.StartupInfo.cb=sizeof(STARTUPINFOEXW)`이며 생성 전 JOB_LIST에 실제 Job handle을, HANDLE_LIST에 자기 stdin/stdout/stderr의 명명한 상속 handle만 넣는다. `bInheritHandles=true`는 이 명시 목록 때문에만 허용하고 Job/helper/reader/진단 handle은 비상속으로 유지한다. 명령줄은 null 종결 포함32767 WCHAR 이내의 새 가변 UTF-16 배열로 만들고 초과는 생성 전 거절하며 문자열 셸 해석0이다. 반환된 실제 PI process handle의 Job 소속·PID/FILETIME를 대조한 뒤 초기 thread를 정확1회 Resume하고, Resume 실패/예상하지 않은 이전 suspend count에서는 대상 실행 성공0이다. 첫 debug 연결이 생긴 같은 스레드에서 `DebugSetProcessKillOnExit(true)`를 명시하여 debugger 스레드 손실 시 타 앱으로 detach하지 않는다. 호출 자체 실패도 첫 오류이며 watcher의 자기 Job 정리만 이어간다. 이 설정은 Job의 최종 handle 폐쇄/집계 관측을 대신하지 않는다. [시작 속성 공식 문서](https://learn.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-updateprocthreadattribute), [debugger 종료 정책 공식 문서](https://learn.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-debugsetprocesskillonexit)

같은 PY의 대상 부모는 명명된 자식1만 네이티브 `CreateProcessW`로 만들고, 같은 Python/파일·고정 자식 역할·자기 writer handle 목록만 전달한다. 자식 생성에 별도 DEBUG_PROCESS를 요청하거나 DEBUG_ONLY_THIS_PROCESS/BREAKAWAY를 켜지 않는다. 부모의 debug 연쇄와 Job 상속으로 생긴 실제 자식의 CREATE 사건/Job 소속/원본 handle을 controller가 확인해야 하며 자동 상속이라는 설명만으로 성공 처리0이다. 대상 부모가 보유한 자식 PI process/thread handle과 controller의 debug/복제 handle은 다른 소유 항목으로 닫는다.

CREATE_PROCESS 사건은 진행시키기 전에 실제 hProcess를 비상속 복제하여 `(PID, creationFILETIME, actualExe, actualHandleGeneration, jobMembership, caseNumber)`를 기록한다. image-name pointer 역참조·프로세스 메모리/레지스터 읽기·쓰기·주입0이다. CREATE_PROCESS/LOAD_DLL 사건의 hFile은 받은 한 소유자가 닫는다. 원본 debug process/thread handle은 운영체제가 EXIT Continue 뒤 닫는 항목이며 Root가 임의 중복 폐쇄하지 않는다. PI handle·Root 복제 process handle·별도 소유 pipe/Job handle은 각각 직접 폐쇄 항목이다. EXIT 사건의 코드만 보고 종료로 선언하지 않고, 같은 스레드의 Continue 성공 **뒤** 복제 hProcess의 `WaitForSingleObject(..., 0)=WAIT_OBJECT_0`와 `GetExitCodeProcess` 실제 DWORD 값을 기록한다. signaled 앞의 STILL_ACTIVE259는 실제 최종 exit로 승격하지 않으며, signaled 뒤 실제259도 숫자 그대로 남긴다. Job Active0·전체 출생 집계·모든 보유 handle/스트림 종료를 모두 대조한다. [debug 사건 공식 문서](https://learn.microsoft.com/en-us/windows/win32/debug/debugging-events), [사건 대기 공식 문서](https://learn.microsoft.com/en-us/windows/win32/api/debugapi/nf-debugapi-waitfordebugevent)

최초 loader breakpoint의 후보 조건은 해당 출생 세대의 초기화 중 최초 예외·초기 thread·`EXCEPTION_BREAKPOINT=0x80000003`·firstChance=1·아직 loader acknowledgement0의 정확1회다. 실제 초기 loader 사건이라고 판별할 근거가 닫히지 않으면 임의 첫 breakpoint를 삼키지 않고 거절한다. 그 조건만 `DBG_CONTINUE=0x00010002`다. 일반 첫 기회 예외는 `DBG_EXCEPTION_NOT_HANDLED=0x80010001`로 전달하고 기록하며 곧바로 처리된 성공/치명 실패로 단정하지 않는다. 두 번째 기회 예외·RIP·필수 사건 API 오류는 최초 실패를 기록하고 자기 Job 정리로 전환한다. 실제 managed 오류·BuildReport 실패·native exit는 별도 의무다. 상한/첫 실패 뒤에도 EXIT/Continue·소유 종료·배출의 한정 정리는 계속하되 다음 정상 사례/새 대상/자동 재시도는0이다. OS 호출 반환 지연을 시계 숫자만으로 강제했다는 주장은0이다.

**외부 감시·상호 장애·실제 handle 폐쇄.** watcher는 대상 Job 밖에 있으며 자기 로컬 복제 Job을 폐쇄까지 보유한다. 따라서 **controller 종료 ≠ KILL_ON_JOB_CLOSE 발동**이다. watcher의 실제 책임은 작업 deadline·controller/Root handle signaled·채널 폐쇄를 감지하면 같은 Job에 `TerminateJobObject`를 요청하고, 실제 ActiveProcesses=0을 읽어 자기 원시에 기록하는 것이다. 개별 대상의 native 종료 코드/EXIT Continue를 watcher가 사후 합성하지 않는다. PS1은 생성 시 보유한 controller process handle로 controller 종료/유한 회수·실제 코드 관측을 담당하고 watcher의 실제 생성 handle도 마지막까지 소유한다. watcher가 죽거나 준비/응답이 유실되면 controller의 원본 Job으로 종료/Active0을 요청하고 PS1은 보유한 watcher handle로 종료/회수한다. controller가 멈춰도 PS1과 watcher가 같은 deadline을 독립 감시하며, watcher 종료/회수도 마지막 예약에 포함한다.

controller 손실·중단 중 CreateProcess 반환 미관측·개별 handle/EXIT 원시 부족·상호 원시 불일치는 watcher Active0을 관측했어도 `sourceAfterCaptured=false`, 빌드/능력 확인 성공0이다. watcher까지 멈추면 PS1은 기존 실제 controller handle을 먼저 종료·회수하고 기존 watcher handle을 종료·회수하여 두 Job handle 소유자의 폐쇄를 시도한다. 최종 handle 폐쇄의 KILL_ON_JOB_CLOSE는 안전 폐쇄 수단이며 미관측 Active0/누락 원시를 성공으로 바꾸지 않는다. Root PS1 자체 손실은 두 helper가 보유한 검증된 Root process handle의 signaled/채널 EOF·고정 deadline으로 실패 정리할 후보이나, Root의 최종 원시 회수/실제 종료를 발급할 주체가 없어 성공0이다. 여러 감시 주체의 동시 OS 호출 정지·권한 실패·시간 내 미종료는 미해결 실패로 그대로 남기며 무한 기다림·새 watchdog/추가 실행·다른 앱 종료로 해결0이다.

명명 handle 원장은 원래 생성자·현재 소유 프로세스/스레드·실제 값·종류·대상 세대·복제 원본·상속 여부·폐쇄 호출/결과·운영체제 자동 폐쇄 구분을 갖는다. 각 준비/실패 경로에서 Job 원본/감시 복제/왕복 임시 handle, helper 상호 검증/Root 감시 process handle, PI process/thread, 대상 복제 process, CREATE/LOAD image hFile, pipe 양끝/부모 writer 사본, 시작 속성 메모리/버퍼의 수명을 닫는다. pseudohandle에는 CloseHandle0이다. 부분 생성된 항목만 finally로 닫고 무효0/INVALID_HANDLE_VALUE를 정상 항목으로 닫지 않는다. 처음 오류와 후속 CloseHandle/배출 오류는 별도로 보존한다. PS1의 실제 두 helper Process/SafeProcessHandle·전용 stream·비동기 수집 작업은 종료/회수 확인 뒤 Dispose하며, 종료/EOF 미관측을 Dispose 성공과 동일시하지 않는다.

**ctypes 전체 선언·64비트 정렬·오류 표.** 구현 필수 플랫폼은 Windows10 이상 64비트의 desktop Win32이며 JOB_LIST·CompareObjectHandles와 실제 기존 부모 Job의 호환이 필요하다. 실제 OS build/플랫폼/ctypes ABI·표준 DLL export를 초기화5초 안 확인하고, 미지원 시 fallback/설치/레지스트리 변경/대상 생성0이다. DLL은 Windows 시스템 경로의 `kernel32.dll` 및 CompareObjectHandles의 `kernelbase.dll`만 `WinDLL(..., use_last_error=true)`로 열고, **호출 전에 아래 모든 argtypes/restype를 명시**한다. 실제 ctypes 검사는 승인된 준비 초기화 안 의무이며 현재 DLL 로딩/구조체 실행 검사는0이다.

다음 표에서 `B=c_int32`, `D=c_uint32`, `W=c_uint16`, `H=c_void_p`, `V=c_void_p`, `Z=c_size_t`, `I64=c_int64`, `U64=c_uint64`, `CW=c_wchar`, `S=c_wchar_p`이고 `P(T)=POINTER(T)`다. HANDLE/pointer/SIZE_T/ULONG_PTR/DWORD_PTR은8바이트, DWORD/LONG/BOOL은4바이트, WCHAR/WORD는2바이트로 실제 assert하며 HANDLE을 int32로 잘라 저장하지 않는다. 표의 구조체 포인터에는 해당 실제 Structure를 쓴다. 실패 BOOL의 last-error는 다른 API/기록 호출 전에 `ctypes.get_last_error()`로 보존한다. 성공 BOOL·유효 handle과 오류값을 혼동하지 않는다. `CompareObjectHandles` false는 동일 객체 실패이며 GetLastError가0이어도 통과0이다.

| 호출 | argtypes → restype | 허용 반환/오류 해석 |
| --- | --- | --- |
| `CreateJobObjectW` | `P(SECURITY_ATTRIBUTES), S → H` | NULL 실패; unnamed/비상속만 |
| `SetInformationJobObject` | `H, c_int32, V, D → B` | 0 실패; ExtendedLimitInformation=9 |
| `QueryInformationJobObject` | `H, c_int32, V, D, P(D) → B` | 0 실패; BasicAccounting=1/ExtendedLimit=9/BasicProcessIdList=3, 반환 길이·배열 용량 대조 |
| `TerminateJobObject` | `H, D → B` | 0 실패; 요청 성공만으로 Active0/개별 종료 성공0 |
| `IsProcessInJob` | `H, H, P(B) → B` | 함수0 실패; out bool의 true/false를 역할별 별도 대조 |
| `OpenProcess` | `D, B, D → H` | NULL 실패; 위 방금 만든 helper/Root whitelist만, bInheritHandle=false |
| `DuplicateHandle` | `H, H, H, P(H), D, B, D → B` | 0 실패; 비상속·DUPLICATE_SAME_ACCESS=2, CLOSE_SOURCE0 |
| `CompareObjectHandles` | `H, H → B` | 두 로컬 handle이 동일 객체인 true만 통과 |
| `CloseHandle` | `H → B` | 0 폐쇄 실패; pseudohandle/무효 handle 입력0 |
| `GetCurrentProcess`, `GetCurrentProcessId`, `GetCurrentThreadId` | 각각 `[] → H`, `[] → D`, `[] → D` | process는 지역 pseudohandle; 실제 process/OS thread ID를 정수 그대로 기록 |
| `GetProcessId` | `H → D` | 0 실패; 실제 handle의 PID 대조 |
| `GetProcessTimes` | `H, P(FILETIME), P(FILETIME), P(FILETIME), P(FILETIME) → B` | 0 실패; 생성 `(high<<32)|low` UInt64 원문 보존 |
| `QueryFullProcessImageNameW` | `H, D, P(CW), P(D) → B` | 0 실패; 가변 최대32768 WCHAR·입출력 실제 길이/정규 경로 대조 |
| `GetExitCodeProcess` | `H, P(D) → B` | 0 실패; signaled 뒤 실제 DWORD 코드, null과0 구분 |
| `WaitForSingleObject` | `H, D → D` | WAIT_OBJECT_0=0/WAIT_TIMEOUT=258/WAIT_FAILED=0xffffffff 구분; 무한값 사용0 |
| `InitializeProcThreadAttributeList` | `V, D, D, P(Z) → B` | 첫 NULL 크기 질의만 0+ERROR_INSUFFICIENT_BUFFER122를 예상; 정확 용량의 실제 초기화는 non0 필수 |
| `UpdateProcThreadAttribute` | `V, D, Z, V, Z, V, P(Z) → B` | 0 실패; JOB_LIST=0x0002000d/HANDLE_LIST=0x00020002·배열 바이트 길이 명시 |
| `DeleteProcThreadAttributeList` | `V → None` | 초기화된 목록만1회; 그 뒤 소유 메모리 폐쇄 |
| `CreateProcessW` | `S, P(CW), P(SECURITY_ATTRIBUTES), P(SECURITY_ATTRIBUTES), B, D, V, S, P(STARTUPINFOW), P(PROCESS_INFORMATION) → B` | 0 실패; STARTUPINFOEX의 주소를 정확 base 포인터로 전달, 명령줄 가변 배열·lpEnvironment=NULL만, 성공 PI와 사건 handle 별도 |
| `ResumeThread` | `H → D` | 0xffffffff 실패; 생성된 초기 thread의 이전 count1만 허용 |
| `WaitForDebugEvent` | `P(DEBUG_EVENT), D → B` | 0이면 ERROR_SEM_TIMEOUT121만 대기 timeout; 그 외 실패. 무한 대기0 |
| `ContinueDebugEvent` | `D, D, D → B` | 0 실패; 같은 OS 스레드·실제 PID/TID·명시 continue 정책 |
| `DebugSetProcessKillOnExit` | `B → B` | 첫 debug 연결 이후 같은 스레드 true 요청; 0 실패 |
| `CreatePipe` | `P(H), P(H), P(SECURITY_ATTRIBUTES), D → B` | 0 실패; 방향별 자기 pipe, 요청 buffer 길이는 실제 커널 용량 보장이 아님 |
| `SetHandleInformation` | `H, D, D → B` | 0 실패; HANDLE_FLAG_INHERIT=1을 정확 끝에만 설정/해제 |
| `GetStdHandle` | `D → H` | STD_INPUT/OUTPUT/ERROR=(-10/-11/-12)의 DWORD 표현; NULL/INVALID_HANDLE_VALUE 실패 |
| `ReadFile`, `WriteFile` | 각각 `H, V, D, P(D), V → B` | 동기 자기 pipe·lpOverlapped=NULL만. Read의 ERROR_BROKEN_PIPE109는 writer 모두 폐쇄에 따른 EOF로 구분, 그 외 실패; 부분 byte 수 보존·크기 확인 |
| `PeekNamedPipe` | `H, V, D, P(D), P(D), P(D) → B` | 단일 reader의 가용 byte/EOF 확인; 0+109만 EOF, 가용0만으로 EOF0 |
| `QueryPerformanceCounter`, `QueryPerformanceFrequency` | 각각 `P(I64) → B` | 0/비양수 frequency 실패; Root/두 helper 단조 기준·빈도 대조 |

Win32 API는 위 명명 집합으로 고정하며 추가 API가 필요하면 정확 용도/서명·소유를 같은 M3 기술 범위에서 먼저 명명한다. 무명 함수의 ctypes 기본 c_int 반환/무선언 variadic 호출0이다. ERROR_ACCESS_DENIED5/INVALID_HANDLE6/NOT_ENOUGH_MEMORY8/INVALID_PARAMETER87/MORE_DATA234 등은 대조/부분 원시와 함께 실패이며 권한 상승·임의 용량 재시도/새 helper로 보정0이다. 실제 termination code 후보는 도구 timeout/안전 정리용 DWORD `0xe000c108`이며 대상에서 관측한 실제 값이 이를 따르는지는 별도 기록한다.

구조체는 Windows x64 기본 자연 정렬로 정의하며 `_pack_=1`/바이트 배열 강제 조립으로 틀린 선언을 숨기지 않는다. 아래 크기/주요 offset는 **구현이 assert할 ABI 기대값**으로 현재 실측값이 아니다. 모든 필드 이름/순서/포인터·union 선언을 공식 원형과 대조하고, 실제 sizeof/alignment/offset 표를 초기 원시에 남긴 뒤만 대상 생성한다.

| 필수 ctypes 구조체/union | x64 기대 크기·정렬과 필수 내부 형식 |
| --- | --- |
| `FILETIME` | 8/4; low/high DWORD2, UInt64 재조합 |
| `SECURITY_ATTRIBUTES` | 24/8; DWORD nLength·pointer lpSecurityDescriptor(offset8)·BOOL bInheritHandle(offset16) |
| `STARTUPINFOW`, `STARTUPINFOEXW` | 104/8 및112/8; base의 cb DWORD/문자열 pointer/위치·크기 DWORD/표시 WORD2/reserved byte pointer/std HANDLE3, EX attribute pointer(offset104). cb는112 |
| `PROCESS_INFORMATION` | 24/8; hProcess/hThread(offset0/8)·PID/TID DWORD(offset16/20) |
| `JOBOBJECT_BASIC_LIMIT_INFORMATION` | 64/8; I64 time2·DWORD flags(offset16)·SIZE_T working-set2·DWORD active limit·ULONG_PTR affinity·DWORD priority/scheduling |
| `IO_COUNTERS`, `JOBOBJECT_EXTENDED_LIMIT_INFORMATION` | 48/8 및144/8; IO_COUNTERS의 U64 여섯 필드, BasicLimit(offset0)·IO(offset64)·SIZE_T memory/peak 네 필드(offset112부터) |
| `JOBOBJECT_BASIC_ACCOUNTING_INFORMATION` | 48/8; I64 time4·DWORD pagefault/TotalProcesses/ActiveProcesses/TotalTerminatedProcesses(offset32/36/40/44) |
| `JOBOBJECT_BASIC_PROCESS_ID_LIST` | DWORD counts2+ULONG_PTR 배열; offset8, 257개 용량이면2064/8. NumberOfAssigned/NumberOfProcessIds와 실제 반환 바이트를 대조 |
| `EXCEPTION_RECORD`, `EXCEPTION_DEBUG_INFO` | 152/8 및160/8; DWORD code/flags·재귀 EXCEPTION_RECORD pointer·address pointer·DWORD parameter count·ULONG_PTR15（`offset32`；로컬 참조 경로, 원래 상대경로 미해결）, firstChance DWORD(offset152) |
| `CREATE_THREAD_DEBUG_INFO`, `CREATE_PROCESS_DEBUG_INFO` | 24/8 및72/8; thread/process/file HANDLE·base/TLS/start/image pointer·DWORD debug offset/length·WORD unicode를 공식 순서로 선언 |
| `EXIT_THREAD_DEBUG_INFO`, `EXIT_PROCESS_DEBUG_INFO` | 각4/4; 실제 DWORD exitCode |
| `LOAD_DLL_DEBUG_INFO`, `UNLOAD_DLL_DEBUG_INFO` | 40/8 및8/8; hFile/base/image pointer·DWORD offset/size·WORD unicode, unload는 base pointer1 |
| `OUTPUT_DEBUG_STRING_INFO`, `RIP_INFO` | 16/8 및8/4; string pointer·WORD unicode/length, DWORD error/type. 외부 문자열 pointer 역참조0 |
| `DEBUG_EVENT`의 전체 union·`DEBUG_EVENT` | union160/8, 전체176/8; DWORD code/PID/TID(offset0/4/8), 모든 위 사건 형식을 갖춘 union(offset16) |
| attribute 목록/handle 배열/가변 명령줄 | opaque 목록은 실제 크기 질의+pointer alignment8로 소유 메모리를 예약, HANDLE[1]/명시 std 목록 SIZE_T 바이트 길이·UTF-16 가변 command buffer를 CreateProcess 반환/attribute 삭제까지 유지 |

구조체의 공개 근거는 [DEBUG_EVENT 공식 문서](https://learn.microsoft.com/en-us/windows/win32/api/minwinbase/ns-minwinbase-debug_event), [Job 확장 한도 공식 문서](https://learn.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-jobobject_extended_limit_information)다. 이 표와 로컬 `ctypes/wintypes.py` 원문 읽기는 ABI 실행 증명이 아니다. 실제 exported API/OS 호출/포인터 수명/전체 구조체 성공은 현재 null이다.

**시작 역할9·준비75초·실제 G8 분리.** 이 계약에서 명명한 준비 실행 역할의 상한은 PS1 호스트1+controller1+watcher1+대상 부모/자식2×3=**9**다. 이는 도구 상위 셸/클라이언트/실행 제공자의 전체 신규 시스템 프로세스 수를 조사한 값이 아니다. Root 외부 실행 기반은 실제 역할/생성된 상위 인프라를 구별하여 원장에 기록하고 조사 없는 전체 시스템9 주장은0이다. PS1/controller/watcher는 대상 Job 집계6에 넣지 않되 각 실제 시작·생성 FILETIME·handle/종료·원시 시간은 따로 결속한다. 후보 외 compiler/별도 launcher/추가 watcher/Python 재실행/새 시험 선택자0이다.

전체 단조 기준은 Root가 명명 PS5 호스트의 실행을 요청하기 직전의 동일 머신 performance-counter ticks/frequency이며 호스트 시작 대기부터 포함한다. PS1 첫 명령에서 Root 원본 기준/빈도를 받아 실제 QPC와 대조하고 controller/watcher 생성 전 초기화 deadline을 고정한다. 호스트 실행 전 기준을 실제 실행 제공자가 남길 능력이 없거나 UTC/PS1 진입 이후 시각만 있으면 호스트 시작까지75초에 포함했다고 발급하지 않고 준비 시작0이다. 이 계측 입구의 실제 제공자 지원도 시행 전 판정 대상이며 현재 실행 입력값/측정 성공은 null이다. 별도 stage 시계를 새 총예산으로 시작하지 않는다. 컨트롤러/감시자의 준비·왕복 Job 대조는 첫 초기화5초 안, 뒤 각 사례의 준비 응답/대상 CreateProcess/동작은 해당10초 안이다. 두 helper의 명령 인자에도 같은 절대 초기/전체 deadline을 넣어 PS1 인증 응답이 오지 않을 때 자기 준비를 중단하게 한다. Process.Start/CreateProcess 자체가 반환하지 않아 실제 helper handle조차 확보되지 않으면 그 시작 시도와 실제 출생/종료 미관측을 구별해 실패로 남기며, timeout 숫자나 watcher 준비만으로 bootstrap 폐쇄를 보장했다고 주장하지 않는다. 해당 준비 성공/완전 회수는0이고 임의 PID를 찾아 보충 종료하지 않는다.

| 준비 단계 | 단조 상한/정확 실행과 종료 관측 |
| --- | --- |
| 공통 초기화 | 5초. PS1 호스트 진입·런타임/ABI/한 Job·watcher/controller 생성·실제 세대/왕복 Job 검증 포함. 실패 시 대상0·후속 사례0 |
| 정상 부모+자식 | 10초. 두 대상만, 자식이 자기 원시 marker를 쓰고 정상 종료, 부모가 자기 실제 자식 handle 종료를 기다린 뒤 정상 종료. 단순 예상0 대신 두 실제 native exit/EXIT/EOF 관측 |
| 그 사례 종료/회수 | 전체 자기 Job 종료 확인 합산5초+해당 전체 스트림 회수 합산5초. Active0·cumulative TotalProcesses2/delta2·개별 세대 종료·원시 봉인 |
| 부모 선종료+자식 pipe | 10초. 부모가 자식 생성 뒤 자신의 writer 사본을 닫고 먼저 종료, 자식이 stdout/stderr writer를 보유해 지연 marker를 기록한 뒤 종료. 부모 signaled 뒤 Active1·EOF 아직0과 실제 자식 EOF/exit를 구별; child 작업 지연 후보2초는 이10초 안 |
| 그 사례 종료/회수 | 합산5+5초. Active0·cumulative TotalProcesses4/delta2·부모 종료가 전체 회수로 오인되지 않았는지 실제 대조 |
| 시간 초과 부모+자식 | 10초. 부모/자식 모두 작업 대기 후보30초를 요청하지만 외부 사례10초 deadline에서 실제 두 생존 대상/보유 writer를 같은 Job으로 종료. 추가30초 예산0 |
| 그 사례 종료/회수 | 합산5+5초. TerminateJobObject·Active0·cumulative TotalProcesses6/delta2·각 실제 non0 또는 null exit·EOF/마지막 byte 확인. 예상 폐쇄 수용을 대상 exit0으로 바꾸지 않음 |
| 최종 helper 종료/회수 | controller/watcher 둘의 종료 합산5초+모든 남은 helper 채널/원시 회수 합산5초. 정상은 Job Active0 뒤 shutdown 응답/handle 폐쇄·실제 두 exit, 실패는 PS1 원래 handle로 한정 종료 |
| 전체 | **5+3×(10+5+5)+5+5=75초**. 초기/사례 최초 예상 밖 실패는 남은 사례0, 같은 총예산의 마지막10초는 helper 정리에 계속 예약. 이미 소비된 시간·정리 실패를 그대로 남김 |

정상 진행이 빨리 끝나도 남은 시간을 추가 시험/실행에 쓰지 않는다. 세 사례의 정규 도구 동작과 폐쇄 의무만 검증하며 controller/watchdog 장애를 만들려고 별도 네 번째 사례/새 Native 시작을 추가하지 않는다. 그 장애 경로는 구현의 한정 정적 판독과 이 세 사례에서 실제 발생한 원시로 구별하며 강제 장애 동적 검증은 현재 별도 허용0이다. 과거 컴파일60의 삭제는 실행 회수를 늘리는 잔여 예산이 아니다. 첫 실패 때 마지막10초 정리 예약을 먼저 사용하며 사례별 미소진 정리 시간을 총75 밖 연장하지 않는다.

실제 G8은 별도 승인 입력의 **Unity1/BuildPlayer1·전체900초+전체종료5초+전체회수5초**다. helper 초기화/최종 정상 봉인·실제 Unity 시작/가져오기/내부 compiler·guard·BuildPlayer·metadata·sourceAfter를900 안에 포함하고, 최초 실패/900초 뒤의 controller/watcher 포함 전체 종료/회수는 기존5/5 안이다. G8 역할은 PS1 호스트1/controller1/watcher1/Unity 대상1과 같은 대상 Job의 실제 내부 자식이며, 준비 대상6을 재사용하거나 별도 예비 Unity0이다. 관측하지 않은 Unity/Bee/서비스 출생 수는 null로 두고 전체 대상 세대256과 실제 집계로 제한한다. 준비 성공은 이 도구 세 사례 성공일 뿐 Unity6000.6/Bee/라이선스·직접 build 호환 성공이 아니다.

**세대256·초과1·원시 바이트 예약과 로그 배관.** 한 실행 전체 출생 세대256이며 동시 활성 한도가 아니다. 256개 정규 보유 process handle 표와 첫 초과 출생1의 안전폐쇄 전용257번째 슬롯/소유 handle을 미리 예약한다. 257번째 CREATE도 원본 handle/실제 신원·Job 소속을 확보하고, 정상 진행을 멈춰 TerminateJobObject/한정 Continue/EXIT/회수를 시행한다. 257번째를 정규 성공 수/256행으로 다시 줄이지 않는다. 추가 출생이 계속되거나 예약/handle 확보가 실패하면 실제 Job 집계·빠진 세대/원시·폐쇄 실패를 그대로 남기고 sourceAfter/성공0이다. 예약1은 추가 대상 생성을 허용하거나 전체 OS가257번째에서 더 이상 생성하지 않는다는 보장이 아니다. Job 관측만으로 미관측 개별 exit를 채우지 않는다.

모든 준비 사례는 합산 아래 동일 원시 예산을 쓴다. 사례마다32MiB/256MiB를 새로 받지 않는다. 원시 byte는 실제 기록 전 공유 잔여 예산과 파일별 한도를 원자적으로 확인하며, 읽기 chunk는 최대64KiB·제어 프레임은 최대16KiB로 고정한다. 단일 JSON은 아래 상한+허용 깊이32/최대 문자열32768 문자·중복 키/명시 타입/정수 범위·UTF-8 엄격 판독을 적용한다. EOF/마지막 byte/기록 SHA는 원시 스트림별 실제 값이며 줄 디코딩/전체 메모리 적재/바이트 절단을 정상 보존으로 표기0이다.

| 원시 종류 | 한 실행 합산 상한·단일 작성자/예약 |
| --- | --- |
| 실행/초기 계획 JSON | 64KiB. Root의 실제 계획/시작 JSON 총합; 미래 최종 원장 SHA 재귀 결속0 |
| 프로세스/감시 사건 | 8MiB. controller 정규6MiB+정리/초과1용1MiB, watcher1MiB로 사전 할당한 각 독립 원시. 서로 같은 파일에 쓰지 않음 |
| stdout | 32MiB. 대상 수집기의 `stdout.log`30MiB+PS1이 소유한 helper stdout/제어 원시 총2MiB. 한 실행/전체 사례 합산 |
| stderr | 32MiB. 대상 수집기의 `stderr.log`30MiB+PS1 helper stderr 총2MiB. 양방향 동시 회수 |
| Unity 지정 로그 기록 후보 | 128MiB. 아래 배관 채택 시 대상 stdout의 동일 원시 byte를 Root 전용 수집기가 `unity.log`에 함께 기록하며 두 파일에 쓴 byte를 총량에서 모두 계산. stdout30MiB 제한이 먼저 적용되어 이 후보에서 Unity tee의 실제 유효 상한도30MiB임을 숨기지 않음 |
| 결과/요약 JSON | 총8MiB. 기존 Editor의 report/inclusion/output 각1MiB=3MiB, Root의 source-before/build-inputs/source-after 각1MiB=3MiB, 정상 end/도구 요약 합산1MiB, 최초 실패/정리/누락/null/미종료 기록1MiB 예약. 기존 limits의 maxRawJsonBytes=1048576을 각 Editor JSON 상한에 결속하며 개별 상한을 공유8MiB 보장과 혼동하지 않음. 같은 작성자의 한도만 공유하고 다른 작성자에는 위 정적 하위 예산을 주어 사후 크기 합산으로 사전 한도를 대신하지 않음 |
| 전체 원시 | 256MiB=268435456byte. 위 최대 합은208MiB+64KiB=218169344byte이며 잔여50266112byte를 임의 새 실행/파일로 쓰지 않음. 각각의 예약은 해당 상한 안이며 외부 별도 예산0 |

원시 사건에는 API 호출 직전/직후의 UTC와 단조 ticks·OS thread·case/phase·PID/생성 FILETIME/실제 executable 핀·handle 소유/복제·Job 설정/TotalProcesses/Active·실제 error/exit/null·읽기/기록/초과 byte·EOF/handle 폐쇄·최초 실패/후속 정리 실패·소비된 시작 수/75초 또는900/5/5를 남긴다. Root의 기존 같은 전달 원장 한 곳에 정확 파일 핀·권한/소유·정적 보정·실제 세 사례·G8을 누적한다. 새 보고·검수 파일/작업 원장·전체 문서 지문 반복·미래 최종 SHA 재귀 의무0이다.

Unity가 직접 `unity.log` 파일을 쓰는 기존 방식은 **사후 크기 관측**이다. 단순50ms 감시나 파일 길이 제한으로 burst의 최대 초과량을 보장할 수 없으므로 직접 writer에128MiB의 엄격 사전 쓰기 보장/유한 초과량을 발급하지 않는다. 이 대안의 한정 해결 후보는 Unity의 `-logFile -`를 자기 명명 stdout pipe로 받고, 같은 원시 byte를 수집기가 기존 `stdout.log`/`unity.log`에 복제 기록하는 배관이다. Windows의 stdout 로그 경로는 [Unity6000.6 명령줄 공식 문서](https://docs.unity3d.com/6000.6/Documentation/Manual/EditorCommandLineArguments.html)에 있으나 이 실제 SDK/숨김/자식/BuildReport에서의 지원·내용/EOF·직접 파일 부재는 미관측이다. stdout에는 Unity 외 같은 배관의 출력도 포함될 수 있으며 이를 분류/삭제해 logger 원본만인 것처럼 꾸미지 않는다. 동일 byte 중복 저장·두 파일 작성자/지문·총 예산을 명시한다.

따라서 §9.3의 `unity.log` 직접 Unity writer/정확 launchArguments를 이 배관의 Root 단일 수집기/`-logFile -`로 기술 호환 채택하는 **정확 판정이 먼저 필요**하다. 기존 조건부 제품 승인만으로 자동 변경하거나 제품 가드의 현재 계획 대조를 우회0이다. 새 사용자 제품 선택이나 정규 회귀 확대가 아니라 같은 M3의 수단/작성자/원시 호환 미결이다. 이 판정/실제 수집기 검수/한정 준비 전 G8 직접 writer를 엄격한 상한을 갖춘 것으로 수용0이다. pipe 내부 kernel buffer의 순간 byte/Unity가 생산한 총 byte를 수집기 파일 사전 상한과 동일시하지 않는다. 최초 초과 read chunk의 실제 byte 수·불기록량/원시 부재를 실패 예약에 남기고 Job 폐쇄와 남은 배출을 한정 예산 안에서 수행한다. 초과 원시를 일부만 남겼으면 완전 회수/sourceAfter/성공0이다. 파이프가 막힘/쓰기 예외를 일으킨 실제 결과도 숨기지 않는다.

**현재 종료/미결.** 이 절의 현재 성과는 정확2 설계·초기화 전 대상0·왕복 Job 동일성·세 역할 상호 소유·75초/9개 명명 역할·모든 선언/수명·원시 예약·G8 로그 호환 후보를 한 문서에서 닫은 것이다. 실제 PS5 제공자의 시작 전 계측/Process.SafeHandle 동작·Win32 선언/ABI/호출·초기 loader 구별·프로세스/스트림 폐쇄·Unity 배관 호환과 미검증 동시 장애 경로는 아직 실행 증명0이다. 아스트라의 정확 기술 채택·파일 단독 인계·솔 구현·루나 묶음 독립 검수·최종 입력/소유/별도 유한 실행 승인 전 도구 파일/컴파일/Python/native/프로브/Unity/GUI/제품/다른 문서/원장 쓰기와 자식 실행0이다. 검토 문서 작성자가 자기 구현/도구 성공/독립 수용을 주장하지 않는다. B1 FINAL·C1C 공유 통합·CH1 완주와 기존 Q0/키/세대/9회/원격 관문은 그대로 미완료·별도 권위다.

### 10.7 정확 도구2의 실행 없는 코드 준비 한정 채택 — Approved

2026-10-13 실제 아스트라 gpt-6-astra가 상향 추론 설정을 지정하지 않은 도구 기본 설정으로 판정한다. §10.6을 아래 코드 작성 범위에만 Approved로 채택한다. codePreparationApproved=true이며 capabilityAccepted=false, preparationExecutionAuthorizedNow=false, nativeExecutionAuthorizedNow=false, unityExecutionAuthorizedNow=false, g8ExecutionAuthorizedNow=false다. 기존95629바이트/SHA DB482B8265470A9D258601806A4D5C1F2E2AC1016E70CEA25D3521C4C0B46B1E 전체 접두사와 역사 모델·설정·미실행 증거는 보존한다.

사용자 산출물은 향후 현재 소스 빌드의 실행 소유·시간·원시 상한을 구현할 도구 코드2다. Root가 단독 소유를 명시 인계한 뒤 솔 gpt-6.1-sol이 도구 기본 설정으로 아래 두 파일만 작성하고 반환한다.

- output/chapter1-playable-r1/tools/b1-current-source-build-root.v1.ps1
- output/chapter1-playable-r1/tools/b1-current-source-build-native.v1.py

승인된 작성은 단순 비활성 안내문이 아니라 §10.6의 고정 역할/입력 형식·명시 타입/중복 거절·경로/핀 검사·단조 예산 계산·정적 바이트 하위 예산·제어 프레임·세대/handle 소유 상태·최초 실패/후속 정리 구분·실제 관측과 null 구분을 구현하는 순수 준비 기능을 포함한다. 같은 두 파일 안에 PS1의 제한된 생성/회수 제어, PY의 controller/watcher/subject-parent/subject-child 고정 역할, 명시 Win32 선언/구조체, 왕복 Job 검증·사건 처리·종료·동시 배출·원시 작성 경로를 코드로 작성할 수 있다. 함수나 선언의 작성은 호출/로딩/지원 입증이 아니다. 원시 생성은 실제 실행 권위 뒤의 경로로만 두며 작성 중 출력·pyc·로그·프로세스를 만들지 않는다. 미결 조건은 명시 거절 분기로 유지하고 추정값으로 성공 경로를 완성하지 않는다.

필수 시작 경계는 다음과 같다.

1. 실제 실행 제공자가 PS5 호스트 요청 직전의 같은 머신 QPC 기준/빈도를 발급·결속하지 못하면 PS5 호스트 시작0이다. 호스트 내부 기준·UTC·임의 숫자·추가 launcher로 대체하지 않는다. 이 제공자 능력은 현재 미입증이며 코드 준비 완료로 해소되지 않는다.
2. Process.Start 반환 전 소유 handle/감시가 확보되지 않을 수 있다. 75초 강제 폐쇄를 보장했다고 표기하지 않는다. 미관측 출생·미확인 최종 폐쇄·시간 내 미회수는 실패이며 임의 PID 검색/추가 watcher/시간 연장0이다.
3. ABI·명명 함수·초기 loader 식별 근거가 미결이면 대상 생성0이다. 필요한 실제 초기화 검사와 그 입력 근거가 먼저 닫혀야 한다. 실행 중 예상하지 않은 loader/예외는 기존 실패 처리로 전환하며 첫 breakpoint를 추측으로 삼키거나 기본값/fallback/새 사례로 통과시키지 않는다.
4. §9.3의 Unity 직접 로그 작성자에서 수집기로의 호환과 helper/감시 원시의 정확 외부 저장 경로가 닫히기 전 G8 경로는 명시적으로 disabled다. 기존 증거 폴더 허용 파일 외 내용을 삽입하거나 제품 가드를 우회하지 않는다. 경로·작성자·로그 배관 지원은 같은 M3의 기술 선행으로 남기며 사용자 제품 선택으로 넘기지 않는다.

이번 허용 검사는 파일 원문 대조와 PS의 비실행 AST 정적 판독뿐이다. 점 소싱·스크립트 호출·전체 Prepare/Probe/Execute·Python 로딩/구문 검사 실행·ctypes/Win32/native·Unity·프로필 접근·제품 assembly 로딩은0이다. 준비75초/9개 명명 역할/원시256MiB, G8 Unity1/BuildPlayer1·900초+전체 종료5초+회수5초 및 최초 실패·자동 재시도0은 그대로다. 제품 소스3는 기존 G3 실제590 수용·정확6 적용·공유 쓰기 종료/소유 인계 선행 이후이며 이번 도구 준비가 그 선행을 충족하거나 제품 구현을 시작하지 않는다.

REQ/AC-C1B-BUILD-004/005/006의 소유·시간·원시 요구와 REQ/AC-PROC-011/012를 추적한다. 도구2는 제품 Assets/Packages/ProjectSettings 밖의 작성물이며 제품 소스 지문·checkpoint를 이번 작업으로 갱신하지 않는다. 실제 도구 길이/SHA·변경/정적 확인·미실행·미결·쓰기 종료를 Root의 기존 작업 원장으로 반환한다. 도구 지문은 향후 실제 소비 입력에 결속하고 문서의 미래 SHA나 원장의 자기 SHA를 재귀 요구하지 않는다.

사후 루나 gpt-6-luna가 도구 기본 설정으로 같은 도구 묶음을 독립 검수한다. 구현자는 자기 독립 수용을 발급하지 않는다. 실제 제공자/ABI/loader/경로·작성자 미결과 실제 입력·소유·한정 검사가 닫힌 뒤 아스트라의 별도 정확 실행 승인이 있어야만 해당 준비 실행을 판단한다. 도구 작성·정적 검사 성공으로 준비 능력·G8·B1 FINAL·CH1 완주 성공을 주장하지 않는다. 새 승인 사본·QA 보고 파일·제품/캐논/GUI/관측 권한은 이 조항에서 추가하지 않는다.

이 문서 작업은 정확1파일 덧붙임·기존 접두사 보존·최종 길이/SHA 확인으로 종료한다. 그 결과와 함께 문서 쓰기 소유를 Root에 반환하며 도구2 소유는 이후 Root의 별도 명시 인계로 활성화한다.

### 10.8 닫힌 Python 원문의 단회 구문 검사 — Approved

2026-10-13 실제 아스트라 gpt-6-astra가 도구 기본 설정으로 아래 읽기 구문 검사1만 Approved로 채택한다. 기존101166바이트/SHA E72AE0A37C755664F63052986E45626110D6E9749AA85343EBDEA308E5A5687B 접두사를 보존한다. §10.7의 Python 실행0에서 이 고정 검사 프로세스1만 예외이며 대상 도구 로딩·역할 실행·Win32 능력 확인은 계속0이다. 사용자 산출물은 닫힌 PY 코드의 문법 검사 결과이며 게임이나 실행 능력 수용이 아니다.

실행 소유는 Root 단독이다. 정확 검사 입력은 C:/Users/me/Documents/GPT-workspace/AcadeGameMaker/output/chapter1-playable-r1/tools/b1-current-source-build-native.v1.py, 56987바이트/SHA 2E07E18BFCACAF9F80B7C5125F037E1157E512664E117A4F1FBC1A92CC4B23C0이다. 실행 파일은 C:/Users/me/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe, 이번 읽기107312바이트/SHA 31073DAD142BC7F3374E3122AA668270052C9AD37D0AAAB803AE73F184C3A43B다. §10.6 당시106448바이트 관측은 역사로 보존하고 현재 파일과 같다고 주장하지 않는다. 현재 버전/동작은 아직 미실행이다. 다른 Python/PATH 탐색·교체·설치·패키지 추가0이다. 실행 직전 Root는 이 두 실제 핀과 원래 경로/재분석 지점 부재·소유 종료를 확인하고 차이가 있으면 시작0이다.

정확 프로세스 인자는 -I -S -B -c 와 Root가 같은 원장에 고정하는 한 인라인 검사 문자열 및 위 대상 절대경로1이다. -S를 추가하여 사용자 환경뿐 아니라 site 자동 초기화도 차단한다. 검사 문자열은 표준 ast/pathlib/hashlib/sys만 사용하고 sys.argv의 정확 경로1을 pathlib.Path.read_bytes로 한 번 읽는다. 읽은 같은 bytes의 길이와 SHA256을 위 상수와 대조한 뒤 ast.parse(bytes, filename=고정 경로, mode='exec')만 호출한다. 여기서 mode='exec'는 AST 문법 시작 규칙이며 exec 실행이 아니다. ast.parse의 내부 AST 생성 구현을 별도 bytecode 컴파일 실행으로 세지 않되 외부 compile/eval/exec/importlib/py_compile 호출은0이다. 대상 PY를 import하거나 함수·최상위 문장을 실행하지 않는다. ctypes/WinDLL/대상 역할·PS1 도구 전체 모드·Prepare/Probe/Execute·제품 assembly·Unity·GUI·프로필은0이다.

정상은 길이/SHA/구문 통과와 sys.version/sys.executable 및 isolated/no_site/dont_write_bytecode의 실제 값을 짧은 표준 출력에 기록하고 정수0으로 종료한다. 예상 판독/구문 오류는 종류·위치·제한된 설명을 기록하고 정수 비0으로 종료한다. 성공 출력만으로 실제 프로세스 종료를 대신하지 않는다. 대상 내용을 출력하거나 새로운 파일·pyc·보고서·helper를 생성하지 않는다. 실행 결과는 현재 원장 한곳에 실제 인라인 문자열/인자·실행 파일 및 입력 핀·출력·원래 오류·종료/회수 사실로 결속한다.

Root의 정확 관리 수단은 현재 호출 호스트 안의 System.Diagnostics.Process/ProcessStartInfo다. 별도 PS5 호스트·launcher·watcher를 추가하지 않는다. UseShellExecute=false/CreateNoWindow=true와 stdout/stderr 리디렉션을 사용하고 Process.Start 직전 Stopwatch 단조 기준을 고정한다. 반환된 Process 객체의 원래 SafeProcessHandle·PID/StartTime·정확 executable을 종료/회수까지 보유한다. 종료는 이 원래 객체/handle에 대해서만 Kill 및 유한 WaitForExit로 확인하며 PID만으로 재조회한 객체·프로세스명 전체 종료·Kill(true)·추가 자식 탐색0이다. 이 순수 검사에는 자식 생성 코드가 없다.

stdout/stderr는 각 BaseStream의 독립 ReadAsync와4096바이트 버퍼로 동시에 회수한다. 같은 루프에서 두 완료 상태·시간·실제 종료를 확인하고 스트림별 저장 상한64KiB를 적용한다. EOF는 실제0바이트 읽기와 최종 완료로 확인한다. 무제한 ReadToEnd/무한 WaitForExit·출력 파일·선행 한 스트림 동기 대기로 다른 스트림 차단0이다. 출력 상한 초과나 읽기 오류는 최초 실패로 보존하고 원래 소유 프로세스 종료·유한 회수로 전환한다. 일부 출력만 남으면 완전 회수 성공0이다.

예산은 이 구문 검사 native 프로세스 시작 최대1·검사 전체10초·종료 확인 전체5초·출력 회수 전체5초·자동 재시도0·재개0·두 번째 시작0이다. Process.Start 대기·Python 초기화/표준 모듈 로딩·입력 판독/해시·ast.parse·실제 종료 관측을10초에 포함한다. 시간 초과는 같은 원래 객체를 종료하고 정수 exit와 종료/회수 여부를 각각 관측한다. Start 반환 자체 지연으로 handle을 확보하지 못하면 출생/폐쇄 미관측 실패이며 10초 강제 폐쇄를 보장했다고 기록하지 않는다. 반환 뒤 경과 초과를 정상으로 바꾸거나 새 프로세스로 보충하지 않는다. 정리5/5는 검사 연장 예산이 아니다. 첫 실패·부분 원시·미종료·미회수는 보존하고 자동 반복하지 않는다. 실행 도구의 실제 권한 심사는 Root의 호출 직전 별도이며 이 문서 작성자는 호출하지 않는다.

이 구문 검사1은 §10.6 능력 확인9역할/75초와 별도의 명명된 유한 실행이다. 능력 확인을 수행한 것으로 계산하거나 그 수량/시간에 몰래 합산하지 않는다. 실제 전체 기록에는 구문 검사 시작 수와 미래 능력 확인 시작 수를 구분해 남긴다. capabilityAccepted=false와 G8 disabled, 256MiB 원시 설계, G8 900/5/5 및 제품 소스3의 G3 선행은 그대로다. 제공자 시작 전 QPC·ABI/loader·로그 작성자/외부 원시 경로 미결은 이 성공으로 해소되지 않는다.

REQ/AC-C1B-BUILD-004/005/006과 REQ/AC-PROC-011/012를 추적하되 전체 AC 통과를 발급하지 않는다. 종료 조건은 실제 핀 일치·구문 결과·정수 종료·10/5/5 관측·두 스트림 회수 및 원장 반환이며 실패도 같은 원문으로 닫는다. 루나는 기존 도구 묶음에서 이 결과의 한정 의미를 확인하고 새 QA 사본/승인 사본을 만들지 않는다. 이번 아스트라 작업은 문서1 덧붙임·접두사 보존·최종 길이/SHA만 확인하여 종료하며 문서 쓰기 소유를 Root에 반환한다. 도구2·다른 문서·제품·원장 쓰기와 실제 Python/native 실행은 이번 작성자가 수행하지 않는다.
