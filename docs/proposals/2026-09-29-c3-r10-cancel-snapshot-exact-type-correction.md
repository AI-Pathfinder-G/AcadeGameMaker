# C3 R10 취소 스냅샷 정확 타입 이름 보정 제안

2026-09-29. 실제 검토자 `gpt-6-sol`. REQ-M5D7QC3-001/005/007 및 AC-M5D7QC3-005/009/010을 추적한다. 코드·조립·설정 변경, 컴파일·Unity·Git 실행 없이 실제 소스와 기존 조립 메타데이터를 읽었다. 아스트라 승인 전 구현하지 않는다.

## 실제 실패 단계

`artifacts/c3-r10-play-remaining-r1.xml` SHA-256 `74A7C664074ECACBDD564A44D4418B3B0E464C04E960E66C5E82FCE5179AB1F7`은 13개 중 통과 11개·실패 2개이다. 두 `AC005_SameActualCancelPreservesCompleteObservedFixtureSnapshot(False/True)`는 각각 8.169329/8.119383초이며 취소 전 `CancelSnapshot:420`에서 잘못된 movement 타입 이름으로 `TypeLoadException`이 발생했다. before snapshot 자체가 완료되지 않았으므로 실제 Cancel의 전후 중립성이 실패했거나 검증됐다는 결과가 아니다. 실제 편집기 종료 2·QA 반환 5·이름 차이 0·입력 884개 전후 차이 0과 원본 실패를 보존한다.

## 정확 선언·타입·조립 대조

현재 Play 시험 SHA-256은 `B573AA593F8624C4A1E6F86059EC02E4DC26EEB3203F8774E74828B166B7850B`, Router 소스는 `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`이다. Router 22–26행의 다섯 선언을 모두 확인했다. 아래 조립 이름은 각 asmdef와 기존 `Library/ScriptAssemblies` PE 메타데이터의 타입 정의/Router 필드 서명을 읽어 교차 확인했다. 제품 조립을 실행하거나 다시 컴파일하지 않았다.

| Router 실제 필드 | 정확 전체 타입 | 실제 조립 | 현재 시험 판정 |
|---|---|---|---|
| `_player` | `AcadeGameMaker.Movement.PlayerMovementController` | `AcadeGameMaker.Movement.Unity` | 타입 namespace 한 곳이 잘못됨 |
| `_transfer` | `AcadeGameMaker.Transfer.Unity.TransferSimulationDriver` | `AcadeGameMaker.Transfer.Unity` | 현재 연결 정확 |
| `_combat` | `AcadeGameMaker.Combat.Unity.OrdanBossCombatSimulationDriver` | `AcadeGameMaker.Combat.Unity` | 현재 연결 정확 |
| `_terminalTeardown` | `AcadeGameMaker.Combat.Unity.OrdanBossTerminalTeardown` | `AcadeGameMaker.Combat.Unity` | 현재 연결 정확 |
| `_cameraProviderBehaviour` | `UnityEngine.MonoBehaviour` | `UnityEngine.CoreModule` | 현재 `typeof(MonoBehaviour)` 정확 |

Movement 원본 `Runtime/Movement/Unity/PlayerMovementController.cs:7,12`의 namespace는 `AcadeGameMaker.Movement`이고 해당 asmdef 이름은 `AcadeGameMaker.Movement.Unity`이다. namespace와 조립 이름을 동일하다고 추정한 것이 오류이다. Transfer 소스 8/13행, OrdanBossCombatSimulationDriver 소스 10/16행, OrdanBossTerminalTeardown 소스 9/14행도 표와 일치한다. 별도 `CombatSimulationDriver` 타입이 존재하지만 Router의 `_combat` 선언은 그것이 아니다.

현재 시험 419행의 movement 필드 이름은 이미 `_player`이다. `_movement`로 바꾸거나 `_movement`를 정정했다고 주장하지 않는다. 스냅샷 키 `execution._player` 및 나머지 대칭 키도 그대로 유지한다.

## 최소 승인 후보

기존 Play Owner 시험 419행의 문자열 하나만 다음처럼 정정한다.

`AcadeGameMaker.Movement.Unity.PlayerMovementController, AcadeGameMaker.Movement.Unity`

→ `AcadeGameMaker.Movement.PlayerMovementController, AcadeGameMaker.Movement.Unity`

420행의 `Type.GetType(item[1],true)`와 기존 `SnapshotField:366–371`의 receiver 정확 타입·DeclaredOnly·DeclaringType·FieldType 검사 및 실패 처리를 유지한다. 이름 단독 fallback, 다른 타입을 허용하는 catch, null이면 검사 생략 같은 보정은 금지한다. 나머지 정확한 네 항목은 수정하지 않는다. 실제 선언/기존 메타데이터로 전체 목록을 한 번에 대조했으므로 추측으로 타입 이름을 순차 교체하는 재시도는 필요하지 않다.

파일 세 역할/bytes·memory 및 실제 memory 부재 한계·reset 상태·actions/maps 참조/활성·양측 영수증·scene/run 상태와 실제 실행 객체 부재/활성의 기존 전체 스냅샷 조건을 삭제하거나 완화하지 않는다. null은 실제 임시 cohort의 원래 관측값으로 기록할 뿐 필드 타입 검증을 생략하는 근거가 아니다. runtime·asmdef·friend·공개 API·InputSettings·ProjectSettings·자산 변경은 없다. 선택 이름/개수와 180초 시험 제한도 유지한다.

승인 후 두 정확 사례 `(False)`와 `(True)`를 실제 임시 root에서 새로 실행하여 before snapshot 완료→동일 실제 Cancel→after snapshot 전체 일치를 검증해야 한다. 이번 보정은 취소 전 타입 조회 장애를 제거하는 후보이며 실제 Cancel 통과를 미리 주장하지 않는다. 원본 R10 결과와 다른 11개 통과 출처를 보존하고 새 두 행의 실제 XML·종료 코드·실패/건너뜀/판정보류·이름/중복·전후 입력 지문을 별도로 기록한다. 신규 결과를 C3 전체 수용으로 확대하지 않는다.

## 읽은 기존 조립 지문과 한계

- `Library/ScriptAssemblies/AcadeGameMaker.Input.Unity.dll`: `A697B0E613B978BF9DD0680409B87624AB8EDB0BEAF7CED5DC94E54032B1E18B`.
- `Library/ScriptAssemblies/AcadeGameMaker.Movement.Unity.dll`: `18060DD808CCA902D9D7A64B78CDB4C43D6560730FEC6F36D18056D0E83E35B4`.
- `Library/ScriptAssemblies/AcadeGameMaker.Transfer.Unity.dll`: `97A258E0F01E23B202004B6BF3412504915EC687AEA91C2CE33ADE491FAD1F27`.
- `Library/ScriptAssemblies/AcadeGameMaker.Combat.Unity.dll`: `E232908F9A0E4241DA33EE00A6182639D79D5F9D2412B9E9CBBB618310D5B035`.

이는 디스크에 존재하는 기존 조립 메타데이터의 읽기 증거이며 새 빌드 성공이나 현재 편집기의 로드 상태를 증명하지 않는다. 원시 실행 증거·소스·설정은 수정하지 않았다.
