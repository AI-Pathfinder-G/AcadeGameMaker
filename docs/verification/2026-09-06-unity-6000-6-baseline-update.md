# Unity 6000.6.0f1 기준 갱신 검증

일자: 2026-09-06  
범위: 프로젝트 Editor baseline 갱신과 새 Editor 실행 사전검증  
Requirement IDs: `REQ-PLAT-001`, `REQ-PLAT-002`, `REQ-PLAT-005`, `REQ-PLAT-007`  
Acceptance-criterion IDs: `AC-PLAT-001`, `AC-PLAT-005`

## 변경 결과

- `ProjectSettings/ProjectVersion.txt` → `6000.6.0f1 (f7f8ed4d1e24)`
- `Assets/Settings/PlatformBaseline.md`의 active Editor 기준 갱신
- bootstrap Editor test의 기준 문자열 갱신
- 새 기준 작업 계약: [`2026-09-06-unity-6000-6-baseline-update.md`](../specs/work-contracts/2026-09-06-unity-6000-6-baseline-update.md)
- 기존 6000.3/6000.5에서 생성된 검증 증적은 재작성하지 않음
- manifest와 packages-lock의 기존 명시 버전은 임의로 올리지 않음

## 로컬 환경 확인

| 항목 | 결과 |
|---|---|
| Editor executable | 존재, Authenticode `Valid` |
| Editor ProductVersion | `6000.6.0f1_f7f8ed4d1e24` |
| 내장 Licensing Client | `1.18.3`, Authenticode `Valid` |
| Hub Licensing Client | `1.18.3`, Authenticode `Valid` |
| entitlement file | 존재·비어 있지 않음 |
| package manifest pins | Input System `1.20.0`, URP `17.3.0`, Test Framework `1.6.0` 유지 |

## 검증 결과

| 검사 | 결과 | AC 근거 |
|---|---|---|
| Licensing preflight | PASS, exit `0` | `AC-PLAT-001`, `AC-PLAT-005` |
| PowerShell QA wrapper parse/diff check | PASS | `AC-PLAT-005` |
| 기존 pre-Unity catalog self-test | PASS, `7 self-tests / 13 scenarios / 68 AC` | `AC-PLAT-005` |
| 새 Editor bootstrap EditMode smoke | BLOCKED, 결과 XML 미생성 | `AC-PLAT-001`, `AC-PLAT-005` |

새 Editor smoke의 마지막 로그는 다음과 같다.

```text
[Licensing::IpcConnector] Connection to channel LicenseClient-me refused
[Licensing::Module] Licensing is not yet initialized.
Unity produced no test-results XML
```

이는 새 Editor 설치나 클라이언트 버전 불일치가 아니라, 이미 정렬된 `1.18.3` 클라이언트와 직접 `-batchmode` Editor 사이의 IPC 초기화 blocker다. 이 결과로 bootstrap 코드나 패키지를 실패 판정하지 않는다.

## 다음 실행 기준

새 Editor를 Hub에서 한 번 열어 import/licensing 초기화를 완료한 뒤 Editor 내부 Test Runner를 먼저 확인한다. Hub 경로가 성공하고 직접 batchmode만 실패하면, `6000.6.0f1` 설치를 Hub에서 재설치하거나 Unity의 최신 호환 패치로 이동하는 별도 운영 조치가 필요하다. Personal 라이선스 재활성화는 Hub 경로만 사용한다.

## 모델 작업 기록

- Kimi K3: `not applicable` — baseline migration이며 런타임 알고리즘 구현이 아니다.
- GLM 5.2: `not applicable` — 새 실패 모드는 실제 Editor/Licensing 로그로 확인했다.
- MiniMax M3: `not applicable` — 새 fixture나 validation tool을 추가하지 않았다.
- Luna: `pending` — 새 Editor의 독립 bootstrap 재현과 package/settings 검토는 IPC blocker 해소 후 수행한다.
