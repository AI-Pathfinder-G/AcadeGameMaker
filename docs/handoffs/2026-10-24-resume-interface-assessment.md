# 2026-10-24 재개 대조와 실제 창 인터페이스 조사

작성: Sol `gpt-6.1-sol`, 기본 추론. 독립 수용과 통합 판정은 작성자가 수행하지 않는다. 사용자 목표는 CH1 전체 정상 플레이 Windows 전달이며 이번 범위는 기존 지문·소비 기록의 읽기 대조 및 공개 인터페이스 조사다. 제품·진단 소스·기존 원시는 쓰기 폐쇄다.

권위는 AGENTS.md, ADR-0041/0043/0044/0045 및 Approved `2026-10-21-autonomous-chapter1-delivery.md` 최신 절이다. REQ-AUTOCH1-001/002/003/005/007/008, AC-AUTOCH1-001/002/003/005/006 및 REQ/AC-AUTOCH1-BOOTINPUT-001에 연결한다. 이번 대조는 새 게임 실행이나 AC 최종 통과가 아니다. main HEAD는 `93aa934bc5b83eac6bbeb48ff3f4754f9d6a14a8`; 추적파일 변경 없이 시작했고 미추적 역사 산출물은 삭제·흡수하지 않았다.

## 현재 바이트 대조

원시: `preparation/session-resume-20261024/fingerprints.v1.json`. 기존 검사를 실행하지 않고 Get-FileHash로 현재 파일을 읽었다. 관측 UTC 2026-10-23T23:22:20.3414656Z, 한국시간 2026-10-24다.

| 대상 | 실제 결과 | 기준 |
|---|---|---|
| 제품 Assets/Packages/ProjectSettings | 630개, 길이·SHA 불일치0 | session-checkpoint-20261024/product-source630.v1.json |
| v48 승인핀 | 135개, 길이·SHA 불일치0 | implementation-files.v48.json |
| 진단 복제본 | 634개, 누락0, 추가4·변경1 정확5차이 모두 일치 | implementation-inputs.v2.json |
| Ship exe | 667136바이트, 인계 SHA 일치 | A4C71B1BFC3E42D02F2F129FE04ADEF3EBF2343A662884E4B99598E9DFC90E7D |
| Ship ZIP | 42378708바이트, 인계 SHA 일치 | 82ACE4A9BACFF1385E2951CC0275C9854EF0FD54DDFFC858528B7C5F97ABFB8E |
| round-3 raw/closure/요약 | 2047/13169/1296바이트, 인계·v31 SHA 일치 | 원시 JSON의 전체 지문 |

v48 원장 SHA256은 DD0835FD0A3439729004264A7EAD61BFA63F665C0AB6411127D9041BEF92C4D6다. 진단 차이는 BootInputProbe.cs/.meta, BootInputProbeBuild.cs/.meta 신규4 및 Run.Unity.asmdef 변경1이다. 단순 파일수만으로 판정하지 않았다.

기존 계약의 「최종 파일 전달 통합 기록」은 Root/Luna의207파일·ZIP 독립 대조 및 Astra 파일 패키지 수용을 기록한다. 「자연 부팅·첫 입력의 실제 통과 통합 기록」 마지막 문단은 Luna 독립 검수 후 Astra가 AC-AUTOCH1-BOOTINPUT-001 한정 결과를 수용하고 묶음을 종료했다. v31과 verification-summary는 실제 창/최종 수용 false다. 이번 Sol 대조를 독립 재수용으로 표시하지 않는다.

## 공개 인터페이스 지원 범위

computer-use skill SKILL.md 및 docs/guidance.md·api.md를 읽었다. sky API에는 list_apps/list_windows, get_window/get_window_state, activate_window, press_key/click 등이 있으나 유효한 앱 접근 승인을 조회하는 별도 메서드는 문서화돼 있지 않다. 창목록 존재는 권한 증거가 아니다. 일반 복구 지침은 프로젝트 한정 복구·소비 기록을 확대하지 않는다.

현 mcp__cua_repl 설명은 Native computer APIs disabled다. Browser 지원을 Unity Ship 네이티브 창 지원으로 해석하지 않는다. 노출된 remote_desktop_commander는 파일·프로세스·검색·설정·기록 기능이며 네이티브 창 캡처/키 전달 API가 없다. 셸 실행 기능은 SendInput·스크린샷·직접 OS API 우회 권한이 아니다. 도구 설치·권한 변경도 하지 않았다.

기존 access-observation.v1.json은 앱 승인 시간초과, PID2320의180.6083801초 강제종료, 캡처0/입력0, helperReset1/Initialize1/inventory1, 복구 대상창0, validAccessPermissionObserved=false를 기록한다. 이번에 이를 바꾼 새 앱 접근 근거는 없다. 따라서 비보호 상태조회 명목의 재초기화·목록 재조회나 보호된 get_window/state를 호출하지 않았다. 문서 조사와 실제 앱 승인 관측을 구분한다.

## 소비 기록과 종료 경계

| 범위 | 실제 소비·종료 | 현재 해석 |
|---|---|---|
| Ship 빌드1 | ship-r1 완료 소비 | 새 빌드0 |
| 최초 실제 창 | 실행1·180초 소비·승인 실패, 한정 복구 소비 | 1200초/3실행을 새 잔여 예산으로 계산하지 않음 |
| 진단 최초 빌드 | 자식 발견 경쟁상태 실패 소비 | 실패 보존·반복0 |
| 진단 round-2 | 빌드 완료·첫 Enter 전 단언 실패 소비 | 실패 실제 phase 미관측 보존 |
| 진단 round-3 | 빌드/실행·한정 통과·독립검수·통합종료 | 묶음 종료·보정/실행 예산 복원0 |

파일·지문·기존 로그·저장·안내·압축 대조는 이미 완료된 안전 대안이며 이번에는 무결성만 확인했다. 가상입력 진단 반복이나 추가 자동 시험은 Ship의 OS 키 전달·창 초점·화면을 입증하지 못한다. 같은 목적을 새 이름으로 반복하지 않는다.

구체 새 실행 근거가 없으므로 실행 계약이나 새 예산을 발급하지 않는다. 이번 종료 조건은 지문 대조, 소비·지원 경계 기록, Luna 독립 검수와 Astra 기능 묶음 판정이다. 제품 실행은 보류하며 actualWindowAcceptance=false, finalAcceptance=false 및 사람 완주 미검증을 유지한다.

## 다음 재개 조건

지원된 공개 인터페이스에 대상 Ship의 유효 앱 접근 승인을 확인할 비보호 조회가 새로 노출되거나 이미 유효한 권한을 입증하는 새 결과가 제공돼야 실제 창 경로를 재평가한다. 단순 창목록, 세션 재시작, 동일 launcher 지문, 일반 프로세스 실행 권한은 충분하지 않다. 새 근거가 생기면 Sol이 정확 도구/동작/파일·Ship 지문/새 관측 목적/기존 소비와 분리된 유한 예산/정리 경계를 초안으로 묶고 실제 계약 경계 변경만 Astra가 승인한 뒤 실행한다. 현재 문서에서 예산 수치나 실행 권한을 선취하지 않는다. 사용자 반복 승인·재생·키·경로 입력은 요청하지 않는다.

보호창 호출0, OS 입력/캡처 우회0, 권한 변경0, Unity/빌드/Ship 재실행0, 별도 c4-integration 변경0, 금지 거대 원장 내용 접근/검색/스캔0이다.
