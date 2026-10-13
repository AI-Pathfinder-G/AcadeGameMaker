# CNG 키 이름의 실제 null 전달 보정

상태: **Approved**, 2026-10-02 통합 책임자 승인. `REQ-PLAT-069/070`, `AC-PLAT-067/068`의 제공자 확인 범위를 유지한다. v1은 이름 부재와 IsEphemeral=False를 관측했으며 실패 결과를 보존한다. .NET 10 공식 CngKey.Create 소스는 keyName==null일 때 IsEphemeral을 true로 설정한다. 근거: https://raw.githubusercontent.com/dotnet/runtime/v10.0.0/src/libraries/System.Security.Cryptography/src/System/Security/Cryptography/CngKey.Create.cs 의55..60행. 따라서 null과 빈 문자열의 호출 경계를 실제 확인한다. 비영속 표시를 강제로 대입하는 보정은 금지한다.

새 허용 출력은 `qa/tools/Test-Q0EphemeralCngKeyV2.ps1`, `artifacts/c4-q0-ephemeral-cng-provider-tests-v2.json`, `docs/verification/2026-10-02-q0-ephemeral-cng-provider-v2-terra-report.md`이다. 원본 v1과 핵심 도구/결과는 불변이다. 먼저 비밀 없는 단순 문자열 인수 확인으로 PowerShell의 $null과 실제 null/빈 문자열을 구별해 관측한다. 새 키 생성에서는 코드 안의 C# literal null을 쓰는 좁은 공개 생성 함수 또는 실제 null 전달을 확인한 NullString.Value를 사용한다. Length3072/ExportPolicyNone·정상 서명/검증/변조·개인 내보내기 거절/폐기 후 거절은 그대로 유지한다.

키 이름이 실제 null인지와 IsEphemeral을 각각 확인한다. 어느 관문도 실패하면 실사용 키로 승격하지 않는다. 이전 생성의 비영속 실패를 소급 성공으로 고치지 않는다. 이전 키 정리가 추가로 필요하면 식별 근거 없이 다른 키를 삭제하지 않고 정확 범위를 보고한다. 개인키 반환/출력/저장 금지와 단명 객체 finally 폐기를 유지한다. 새 결과 지문과 실제 권한 환경·실행 횟수/오류를 기록한다. 제공자 진단만 승인하며 실제 권위 발급/계획 서명/Git/원격 쓰기는 금지한다.
