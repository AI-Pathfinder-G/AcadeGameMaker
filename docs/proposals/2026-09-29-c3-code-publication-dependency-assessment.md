# C3 코드 게시의 선행 의존 범위 확인

- 날짜: 2026-09-29
- 확인자·실제 모델: 솔, `gpt-6-sol`
- 상태: 읽기 전용 의존 평가 — 게시·기반 통합·새 구현 승인 아님
- 조회 대상: 로컬 원격 추적 참조 `origin/main`의
  `46b14e97be2c5fc6017d92c09e3200899599830a`
- 방법: `git ls-tree`, 로컬 파일·asmdef·참조·승인 증거 읽기. 네트워크 조회,
  자격증명 접근, Unity 실행, 코드 수정은 수행하지 않았다.

## 결론

현재 추적된 원격 기준에 C3 변경 파일만 추가하는 독립 코드 변경 요청은 실행 가능한
Unity 프로젝트가 되지 않는다. 원격에는 `Assets`, `Packages`, `ProjectSettings`가
전혀 없으며, `qa/tools`에는 `Test-QaCatalog.ps1` 하나만 있다. 로컬 HEAD에는 같은
조회 범위에 328개 경로가 있지만 원격 기준과 다르고, Input/Profile/HubPresentation
및 상당수 시험·도구는 로컬 미추적 파일이다. 이 수는 의존 폐쇄 목록이나 게시 승인이
아니다. 실시간 원격 상태를 조회하지 않았으므로 위 결론은 지정 추적 참조에 한한다.

따라서 기존 게임 기반의 검토된 통합이 선행하거나 동일 변경 요청에 명시적으로
포함되어야 한다. C3L·하위 경계·소유자·후속 세대 구현 몇 파일만 게시해서 선행
기반이 이미 원격에 있다고 간주할 수 없다. 필요한 기반 전체는 아래 실제 의존
폐쇄와 검증 자료를 의미하며, 로컬 미추적 자료 전부를 일괄 게시할 근거는 아니다.

## 실제 선행 범위

| 범위 | 실제 로컬 결속 | 지정 원격 기준 존재 |
| --- | --- | --- |
| C3L와 하위 경계 | `Runtime/Profile`의 관찰·디스크 트랜잭션·초기화 서비스 및 인코더/복구/잠금 기반, `Runtime/Input/Unity`의 실행 관리자·프로필 준비·입력 적용·확인 경계·메모리 전환 | 없음 |
| Q-B/소유자/후속 presenter | `Runtime/HubPresentation/Unity` 전체 선행 Q-A/Q-B 및 `Runtime/Input/Unity`의 허브 이관·메뉴 제어·의미 프레임·InputRouter | 없음 |
| 런타임 조립 기반 | Hub 조립은 Core/Input/Input.Unity/Profile와 `Unity.TextMeshPro` 참조. Input.Unity는 Input/Core/Profile 외 Movement/Movement.Unity/Transfer/Transfer.Unity/Combat/Combat.Unity를 참조. 이들 조립의 전체 소스·asmdef·AssemblyInfo·meta 및 전이 참조가 필요 | 없음 |
| 시험·저작 기반 | InputUnity 편집 시험은 Hub.Authoring.Editor를 참조. HubPresentation 편집/실행 시험과 Profile/Input/InputUnity 및 기존 필수 회귀 조립·fixture도 필요 | 없음 |
| 저작 자산 | `Editor/HubAuthoring`의 빌더/검증기는 `Assets/Prefabs/Hub`, `Assets/Scenes/Hub.unity`, `Assets/GameInput.inputactions`, `Assets/UI`의 TMP 설정·글꼴·SDF·shader와 고정 GUID/meta를 조회 | 없음 |
| Unity 설정 | `Packages/manifest.json`, `packages-lock.json`, 해당 `ProjectSettings` 및 필요한 Assets 설정·meta. 현재 에디터 `6000.6.0f1`, Input System `1.20.0`, URP `17.3.0`, 시험 프레임워크 `1.6.0`, uGUI `2.6.0` | 없음 |
| 검사 증거 재현 | `qa/tools/Invoke-UnityQa.ps1`, `Test-UnityQaOwnedEditorWait.ps1`, 실제 필수 회귀 fixture/원장/실행 증거와 Q0 검사가 읽는 문서 | 실행 도구 둘 없음, 문서는 개별 대조 필요 |

`Input.Unity`가 직접 게임플레이 조립을 참조하므로 확인 경계에서 게임플레이를
호출하지 않는다는 사실만으로 해당 조립을 게시 범위에서 제외할 수 없다. 현재
조립 방향을 바꿔 게시 범위를 줄이는 변경은 이번 확인의 권한에 포함되지 않는다.
새 소유자 시험의 최종 파일·meta는 구현 완료 후 확정해야 하며 이 문서는 빌드 성공이나
최소 파일별 게시 목록의 완결성을 주장하지 않는다. 전체 필수 회귀를 재현하려면
해당 회귀가 포함하는 다른 조립·fixture의 전이 의존도 함께 포함해야 한다.

## 지문과 선행 수용 보존

- [Q0 후속 승인](../approvals/2026-09-28-c3-adapter-successor-sha-approval.md)은
  실행 관리자 현재 SHA-256 `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB`,
  입력 관리자 현재 SHA-256 `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`을
  고정한다. `HubUiOnlyQ0ScopeAuditEditModeTests.cs`의 역사 9행·엄격 현재 7행·후속
  2행과 관련 증거 문서도 보존한다. 이전/새 해시 중 하나를 허용하거나 핀을 완화하지 않는다.
- [C3L 수용](../approvals/2026-09-29-c3l-integration-acceptance.md)의 정확한 다섯
  소스 지문과 고유 613개 회귀 원장, R4 실패 및 R5/R6 선택 증거는 당시 이력이다.
  이후 하위 확인 경계가 바뀐 현재 소스에 과거 C3L 지문이나 이전 집중 21개 통과를
  그대로 적용하지 않는다. 게시용 기준선에서는 승인된 변경만 증거의 후속 관계로
  연결하고 정확한 새 소스에서 필요한 검증을 수행해야 한다.
- `.gitattributes`·줄바꿈·meta/GUID·asmdef·패키지 잠금도 함께 보존해야 한다.
  게시 과정에서 파일을 재생성하거나 줄바꿈을 바꾸면 고정 바이트 지문이 달라진다.
  검사에서 차이가 보이면 이전 수용을 현재 수용으로 치환하지 말고 실제 후속 증거로
  구분해야 한다. 현 로컬 변경 전체의 수용·원격 게시를 이 문서가 승인하지 않는다.

추적 범위는 `AC-M5D7QC3-009/010`의 향후 게시·재현 가능성 확인이며 실행 판정이
아니다. C3 전체 및 C4 수용 상태는 변경하지 않는다.
