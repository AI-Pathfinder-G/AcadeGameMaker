# Unity Licensing Client 재연결 실패 가드레일 감사

일자: 2026-09-06  
범위: Unity 실행 환경과 QA 실행 차단기  
Requirement IDs: `REQ-PLAT-001`, `REQ-PLAT-002`, `REQ-PLAT-012~014`  
Acceptance-criterion IDs: `AC-PLAT-001`, `AC-PLAT-002`, `AC-PLAT-010~012`

## 판정

현재 문제는 Personal entitlement 부재나 Editor/Client 버전 불일치로 판정되지 않는다. 샌드박스 실행에서는 Hub 라이선싱 named pipe 접근이 차단되지만, 일반 Windows 사용자 세션에서는 라이선스와 Unity 실행이 정상 완료된다.

- 프로젝트: Unity `6000.6.0f1 (f7f8ed4d1e24)`
- 프로젝트 Editor 내장 Licensing Client: `1.18.3`, Authenticode `Valid`
- Unity Hub 전역 Licensing Client: `1.18.3`, Authenticode `Valid`
- entitlement 파일: `%LOCALAPPDATA%\Unity\licenses\UnityEntitlementLicense.xml` 존재·비어 있지 않음
- Hub 라이선싱 로그: `Hub 3.21.1` 핸드셰이크 `200`, entitlement 파일 파싱 성공, PACL revision `52` 로드
- 직접 실행 주체: `desktop-3p3u8vk\codexsandboxoffline` (`CodexSandboxUsers` 그룹)
- 직접 실행 로그: `LicenseClient-me` 거부 → 내장 Client 재실행 시도 → Licensing 초기화 미완료, 결과 XML 미생성
- 실제 pipe 점검: `LicenseClient-me` 연결 시간 초과, `Unity-LicenseClient-me` 접근 거부

따라서 문제를 `해소된 클라이언트 버전 불일치`와 `샌드박스 실행 주체의 named pipe 접근 거부`로 분리한다. 패키지 미등록 뒤 발생한 NUnit/TestAttribute 관련 오류는 2차 증상이며 코드 결함 판정에 사용하지 않는다.

## 적용 변경

- `qa/tools/Invoke-UnityQa.ps1` 추가
  - 프로젝트 고정 Editor와 실행 파일 버전 확인
  - 내장/전역 Licensing Client 존재·서명·버전 확인
  - 버전 불일치 시 기본 실행 차단
  - entitlement 파일 확인
  - 중복 Unity Editor와 Licensing Client 확인 시도
  - `-accept-apiupdate` 포함
  - 라이선스 차단 로그와 테스트 실패를 분리
  - 기존 XML/로그 덮어쓰기 금지
- `docs/qa/unity-licensing-runbook.md` 추가
  - 원인, 복구 순서, exit code와 증거 보존 규칙 기록
- `qa/README.md`, `docs/README.md`에 진입점 추가

## 검증 결과

| 검사 | 결과 | AC 근거 |
|---|---|---|
| PowerShell AST 구문 검사 | PASS | `AC-PLAT-010~012` |
| 기본 preflight (정렬 전) | exit `3`, 버전 불일치 차단 | `AC-PLAT-001`, `AC-PLAT-002`, `AC-PLAT-012` |
| 기본 preflight (정렬 후) | exit `0`, 버전 정렬 확인 | `AC-PLAT-001`, `AC-PLAT-002`, `AC-PLAT-012` |
| Licensing Client 재시작 | 중복 2개를 정리하고 동일 경로 응답 프로세스 1개 확인 | `AC-PLAT-001`, `AC-PLAT-002` |
| `-AllowClientVersionMismatch -PreflightOnly` | exit `0`, 경고 유지 | `AC-PLAT-001`, `AC-PLAT-002` |
| 기존 QA catalog validator | `PASS scenarios=13 coveredAC=68` | `AC-PLAT-010~012` |
| Unity EditMode bootstrap smoke (샌드박스 실행) | 결과 XML 미생성, `LicenseClient-me` 거부 후 초기화 정지 | 라이선스 차단으로 유효한 테스트 증적 생성 조건 미충족 |
| 실행 주체/pipe 직접 대조 | `codexsandboxoffline`에서 실제 pipe 접근 거부 | `AC-PLAT-001`, `AC-PLAT-002` |
| Unity EditMode bootstrap smoke (일반 사용자 세션) | Licensing 핸드셰이크·entitlement 해석 성공; 초기 컴파일 오류는 수정 후 통과 | `AC-PLAT-001`, `AC-PLAT-002` |
| Bootstrap 단독 EditMode 테스트 | `passed=1, failed=0, skipped=0, UnityExitCode=0` | `AC-PLAT-010~012` |
| 전체 EditMode 회귀 | `passed=372, failed=0, skipped=0, UnityExitCode=0` | `AC-PLAT-010~012` |

정렬 전 기본 preflight가 발견한 실제 차단 메시지는 다음과 같다.

```text
[FAIL] Hub and embedded Licensing Client versions differ (1.18.3 vs 1.18.1); direct batchmode launch is blocked.
Preflight blocked the Unity launch. No test result is being produced.
```

정렬 후 실제 실행에서도 다음 상태가 재현됐다.

```text
[Licensing::IpcConnector] Connection to channel LicenseClient-me refused
[Licensing::Module] Licensing is not yet initialized.
Unity produced no test-results XML
```

재시작과 버전 정렬 후 샌드박스 실행에서는 `Hub 소유 named pipe` 접근이 차단됐지만, 일반 사용자 세션 재실행에서는 Licensing 핸드셰이크와 entitlement 처리가 성공했다. Unity 6000.6 호환 수정 및 QA 래퍼의 비동기 증거 대기 보완 후 Bootstrap 단독과 전체 EditMode 회귀가 모두 통과했으므로 현재 해당 QA 경로의 라이선스·컴파일 blocker는 해소됐다. 라이선스 파일 삭제, 바이너리 교체, pipe ACL 완화로 우회하지 않는다.

이번 감사에서 Unity 프로세스 명령줄 열거는 권한/환경 문제로 확인되지 않아 중복 Editor 검사는 `WARN`으로 남겼다. 이 검사는 프로세스를 강제 종료하지 않으며, 장치의 핵심 차단 조건은 라이선스 클라이언트 버전 확인과 실행 후 IPC/entitlement 로그 분리다.

## 모델 작업 기록

- Kimi K3: `not applicable` — 런타임 알고리즘·Unity 코드 구현이 아닌 로컬 환경 차단기다.
- GLM 5.2: `not applicable` — 실제 Editor/Hub/Licensing 로그와 바이너리 증적으로 실패 모드를 판정했다.
- MiniMax M3: `not applicable` — 반복 입력 fixture가 아니라 사전점검과 실행 래퍼가 대상이다.
- Luna: `not applicable in this supplementary pass` — 별도 Luna 작업 스레드를 생성하지 않았으며, 로컬 검증 결과는 위 표에 명시한 실행 증적으로 제한한다.
