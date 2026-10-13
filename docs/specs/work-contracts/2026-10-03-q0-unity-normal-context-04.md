# Q0 Unity 정상 권한 환경 대조 네 번째 시도

상태: **Approved** — 2026-10-03 실제 `gpt-6-astra`의 한정 기술 승인. `REQ-PLAT-061/062`, `AC-PLAT-059/060`의 동일 최종 바이트 지원 확인에만 적용한다. 기존 세 시도·4F9·원장·원본은 불변이며 최종 운영 수용이 아니다.

## 실제 근거

아스트라가03 JSON 원문/SHA `D68DC47C782D52891B0CDB2F6A4F2B55EC50D07B5D2A6D439D448AEB24E36C38`를 읽었다. 일반 해시 읽기는 공유잠금으로 실패했지만 FileShare.ReadWrite 원시 읽기로 로그6070바이트/SHA `4A462FA019A4216FA42D217D9316D37F563C22B91A85EB5AEE6C0DE5C7EC54DF`를 직접 대조했다. Unity PID44204/ticks639265542226848562가 종료1이고 timeout=false, 지원 메서드 미관측·XML 부재·보호501 및 네 C# 전후 동일이다. 명시적 종료 경로는 Package Manager의 Upm-44204 IPC 연결30초 실패다. BIOS/networkadapter WMI 접근 거절이 함께 있지만 그것만으로 라이선스 실패나 IPC의 인과 원인을 확정하지 않는다.

실제 루나는03 exec_command의 sandbox_permissions 생략/use_default, PowerShell 셸→고정 pwsh7.6.5 -NoProfile -File 호출을 확인했다. wrapper는 `output/q0-trusted-child-luna-r1/selftest-repositories/core-prototype-71-independent-20261003/run-unity-attempt03.ps1`, SHA `718A91918219E5293B989EE0D0EA443A43D0B636CDF24E2FDA761E08DBE623B5`다. root는 식별값을 출력하지 않는 동일 WMI 읽기의 제한 환경 접근거절/정상 권한 성공을 관측했다. 루나는 잔존 Licensing.Client PID47360의 실제 부모44204/시작시각을 대조한 뒤 정확 그 소유 자식만 종료했다고 보고했다. 이들 실행을 아스트라 자신의 실행으로 소급하지 않는다.

## 정확 추가 경로와 실행

추가 허용은 다음 네 파일뿐이다.

- `output/q0-trusted-child-evidence-r1/unity-compile-attempt-04.log`
- `output/q0-trusted-child-evidence-r1/unity-compile-attempt-04.xml`
- `output/q0-trusted-child-evidence-r1/unity-compile-attempt-04.json`
- `output/q0-trusted-child-luna-r1/selftest-repositories/core-prototype-71-independent-20261003/run-unity-attempt04.ps1`

실제 도구의 require_escalated 정상 권한 경로로 정확 한 번 실행한다. 도구 승인 절차를 우회하지 않는다. wrapper03을 원천으로04 시도번호/위 출력경로·독립 진단 InvocationId/pipe 이름을 바꾸고, 기존 유한기한·원시 공유읽기·map 정리 전 직접관측·소유 핸들/전체ticks·보호 전후 대조를 유지한다. 소스/지원메서드/프로필을 바꾸어 환경 비교를 섞지 않는다. wrapper 차이·원시SHA와 실제 권한 호출 메타데이터를 기록한 뒤 시작한다.

실행 대상은 같은 준비 복제, Unity6000.6.0f1, `AcadeGameMaker.Tests.EditMode.InputUnity.Q0TrustedChildClient.KeylessSupport`다. 시작 전 네 C# 현재SHA가03의 최종 바이트(175D5F1C… 및 CF59056C…/44A1BC16…/B969FECE…)와 전수 일치하는지 확인하고 다른 runtime 소스도 실행 전후 대조한다. 달라졌으면 이 동일 바이트 승인으로 시작하지 않는다. 라이선스 재설정·패키지/프로젝트 설정 변경·프록시 우회·일괄 프로세스 종료는 금지한다.

## 증거와 실패 처리

03 실패/로그/누락 관측은 그대로 보존한다.04 로그/XML은 미사용 경로에서 생성하며 종료 뒤 원시바이트를 확정한다. XML 미생성이면 부재를 기록한다. 실제 종료·pipe 피어/서버 신원·공개벡터·공유순번/map·보호/소스·로그 안정성의 완성 결과를 검사한 뒤04 JSON을 CreateNew로 한 번 생성한다. 부분 성공을 전체 지원으로 기록하지 않는다. 실패하면 정확 원인과 실제 권한 환경을 남기고05 또는 같은 경로 재시도를 자동 수행하지 않는다. 소유 자식 정리는 핸들·PID·전체ticks·경로/계보를 확인한 대상만 허용하며 외부 중단을 자연 종료/관리 정리 성공으로 쓰지 않는다.

이 승인은 이전 세 시도 소진 규칙에 대한 환경 대조 한 번의 정확 예외다. 공통 준비/channel/unity 최종 증거는04와 현재 독립 검수 완료 전 발급하지 않는다. 새 키·실행 복제·Git·원격·아홉 운영 선택은 승인하지 않는다. 정상 권한에서 성공하더라도03의 제한 환경에서 실패한 이력과 일반 실행환경 지원 한계를 보존한다.
