# Unity Licensing Client 재연결 실패 감사 및 재발 방지

상태: 운영 가드레일 적용

## 결론

이번 실패의 현재 주원인은 Personal entitlement 부재나 버전 불일치가 아니다. entitlement 파일은 존재하고, 전역 `Unity.Licensing.Client`는 Hub 요청에 핸드셰이크 `200`을 반환하며 entitlement 파일도 정상 파싱한다. 프로젝트 Editor는 `6000.6.0f1`, 내장/Hub Licensing Client는 모두 `1.18.3`으로 정렬됐다. 재감사에서 확인한 직접 배치 실행의 실제 원인은 실행 주체의 Windows 보안 경계다. Unity가 실행된 `codexsandboxoffline` 프로세스는 논리 채널 `LicenseClient-me` 연결에 실패했고, Hub가 만든 실제 `Unity-LicenseClient-me` named pipe 접근에서는 `Access to the path is denied`가 재현됐다. 그 뒤 Editor가 내장 Licensing Client를 다시 띄우지만 초기화가 끝나지 않아 `Licensing is not yet initialized` 상태로 남는다.

현재 설치 조합은 다음과 같이 정렬됐다.

| 구성요소 | 버전 | 경로 |
|---|---:|---|
| 프로젝트 고정 Editor | `6000.6.0f1` | `C:\Program Files\Unity\Hub\Editor\6000.6.0f1` |
| 해당 Editor 내장 Licensing Client | `1.18.3` | `Editor\Data\Resources\Licensing\Client\Unity.Licensing.Client.exe` |
| Hub 전역 Licensing Client | `1.18.3` | `C:\Program Files\Unity Hub\UnityLicensingClient_V1\Unity.Licensing.Client.exe` |

실패 순서는 `LicenseClient-me` 채널 거부 → 내장 Licensing Client 재실행 시도 → Licensing 초기화 미완료/타임아웃 → 재연결 실패다. 따라서 뒤따르는 NUnit/TestAttribute 또는 패키지 오류는 원인이 아니라 라이선스 초기화 실패의 2차 증상이다. 과거의 성공 로그는 다른 실행 주체에서 Editor가 전역 클라이언트와 통신한 사례이므로, 현재 샌드박스 배치 실행의 pipe 권한 문제를 해소했다는 증거로 사용하지 않는다. pipe ACL을 느슨하게 바꾸는 대신, 라이선스가 필요한 QA는 Hub와 같은 일반 Windows 사용자 세션에서 실행해야 한다.

일반 Windows 사용자 세션에서 재시도한 결과, Licensing Client 핸드셰이크·entitlement 해석·라이선스 갱신은 모두 성공했다. 초기 재시도에서 확인된 Unity 6000.6 API 호환성 오류와 테스트 코드 컴파일 오류를 정리한 뒤, Bootstrap 단독 테스트 `1/1` 및 전체 EditMode 회귀 `372/372`가 통과했다. QA 래퍼에는 Unity 6000.6의 비동기 XML 생성을 기다리는 최대 120초 증거 대기 절차도 반영했다.

## 적용한 장치

`qa/tools/Invoke-UnityQa.ps1`가 Unity 실행 전에 다음을 검사한다.

- 프로젝트의 `ProjectVersion.txt`와 실행할 `Unity.exe` 버전 일치
- 내장/전역 Licensing Client의 존재 여부와 Authenticode 상태
- 내장/전역 Licensing Client 버전 불일치
- entitlement 파일 존재·비어 있지 않음
- 같은 프로젝트를 이미 열고 있는 Unity Editor 중복 실행
- 라이선스 클라이언트 프로세스 중복 경고
- 배치 실행 시 `-accept-apiupdate` 자동 포함
- 실행 후 Licensing/entitlement 오류 로그를 별도 차단
- 결과 XML이 없거나 라이선스 오류가 있으면 테스트 실패로 기록하지 않음
- 기존 증적 파일은 덮어쓰지 않고 실행별 타임스탬프 파일 생성

사전점검만 실행하려면:

```powershell
pwsh -NoProfile -File qa/tools/Invoke-UnityQa.ps1 -PreflightOnly
```

Unity 테스트를 실행하려면:

```powershell
pwsh -NoProfile -File qa/tools/Invoke-UnityQa.ps1 `
  -TestPlatform PlayMode `
  -TestFilter 'AcadeGameMaker.Tests.PlayMode.CombatUnity.OrdanBossEncounterAuthoredGraphPlayModeTests.FreshGraphPublishesTheExactBossPhaseOrderWithHiddenAudit'
```

버전 불일치를 확인만 하면서 강제로 실행하는 우회는 명시적인 진단용으로만 허용한다.

```powershell
pwsh -NoProfile -File qa/tools/Invoke-UnityQa.ps1 -PreflightOnly -AllowClientVersionMismatch
```

## 운영 순서

1. Unity Hub에서 프로젝트를 열고 `6000.6.0f1`이 선택되어 있는지 확인한다.
2. Hub가 라이선스 상태를 회복할 때까지 기다린다. Personal 라이선스의 활성화·반환은 Unity Hub에서 처리한다.
3. Unity Editor와 Hub가 프로젝트를 사용 중인 상태에서 직접 배치 실행하지 않는다.
4. 라이선스가 필요한 QA는 `Invoke-UnityQa.ps1`를 Codex 샌드박스가 아닌 Hub와 동일한 일반 Windows 사용자 세션에서 실행한다. 샌드박스에서 실행한 결과는 환경 진단 증적으로만 취급한다.
5. 버전 불일치가 FAIL이면 바이너리를 수동 교체하거나 라이선스 파일을 삭제하지 않는다. Hub/Editor 업데이트 정책을 정해 동일 계열로 맞춘 뒤 다시 점검한다.
6. Hub 자체에서 라이선스가 정상인데 배치 실행만 IPC 거부를 내면 Licensing Client 재시작으로 해결하려 하지 않는다. 실행 세션을 Hub와 동일한 일반 Windows 사용자 컨텍스트로 바꾼 뒤 재시도한다. 이 스크립트는 전역 라이선스 프로세스를 강제 종료하거나 named pipe ACL을 변경하지 않는다.
7. 결과 XML이 생성되지 않은 실행은 acceptance evidence로 승격하지 않는다. 로그와 사전점검 결과를 환경 차단 증적으로 보관한다.

## 판단 기준

- `exit 0`: 라이선스 차단 신호가 없고 결과 XML이 있으며 failed=0인 실행
- `exit 3`: 사전점검 차단 또는 실행 중 Licensing/entitlement 차단
- `exit 4`: 결과 XML 없음/손상
- `exit 5`: Unity 실행 또는 테스트 자체 실패

이 구분은 `AC-PLAT-001/002` 환경·실행 증적과 `AC-PLAT-010~012` QA 산출물의 검증 결과가 서로 오염되지 않도록 하기 위한 것이다. 이 도구는 `Assets/`, `Packages/`, `ProjectSettings/`와 런타임 계약을 변경하지 않는다.

## 증거 위치

Windows Unity 로그의 표준 위치는 다음과 같다.

- Editor: `%LOCALAPPDATA%\Unity\Editor\Editor.log`
- Package Manager: `%LOCALAPPDATA%\Unity\Editor\upm.log`
- Licensing Client: `%LOCALAPPDATA%\Unity\Unity.Licensing.Client.log`
- Entitlement audit: `%LOCALAPPDATA%\Unity\Unity.Entitlements.Audit.log`

공식 참고: [Unity Command line arguments](https://docs.unity3d.com/es/current/Manual/CommandLineArguments.html), [Unity Windows log file locations](https://docs.unity3d.com/cn/2022.3/Manual/LogFiles.html), [Managing your Unity license](https://docs.unity3d.com/kr/6000.0/Manual/ManagingYourUnityLicense.html).

## GPT participation record

- GPT subagents: not applicable — 반복 가능한 QA fixture가 아니라 실행 전 환경 차단기와 운영 절차를 추가했고, 실패 모드는 Unity/Hub 프로세스, named pipe, Editor 로그의 직접 대조로 판정했다.
- 이 문서에는 외부 모델 호출이나 현재 위임 지시가 없다. 향후 구현·독립 QA 위임은 ADR-0032의 GPT Terra/Luna 표준을 따른다.
