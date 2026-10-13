# Q0 CNG null 이름 전달 보정 — 테라 실행 보고

- 승인 계약: `docs/specs/work-contracts/2026-10-02-q0-cng-null-name-correction.md` (`Approved`), 소유 `REQ-PLAT-069/070`, 부분 검증 `AC-PLAT-067/068`.
- 새 출력은 V2 시험 도구·결과·이 보고서 세 파일이다. V1 도구와 실패 결과 및 기존 핵심 15개 결과는 고치지 않았다. V1 결과 JSON의 SHA-256은 `6C66F0EF4F8362CA614480E326BA0D57BD1753E406A75392CDD2F97752AC70F7`이다.
- .NET 10 공식 `CngKey.Create` 소스 55–60행은 실제 `keyName==null`일 때 `IsEphemeral=true`를 설정한다. V1의 빈 문자열 경계를 비밀 없는 C# 문자열 함수로 확인한 후 `NullString.Value`를 새 생성 호출에 사용했다. `IsEphemeral` 값을 강제로 쓰지 않았다.

## 실행 결과

`pwsh -NoProfile -File qa/tools/Test-Q0EphemeralCngKeyV2.ps1`을 승인된 권한 경로에서 **1회 실행, 10/10 통과**했다. 프로세스의 실제 관리자 역할 검사값은 `IsAdministrator=False`였다. PowerShell `7.6.5`, .NET `10.0.11`, 제공자 타입 `System.Security.Cryptography.CngKey`, 제공자 `Microsoft Software Key Storage Provider`를 관측했다. 자세한 개별 결과는 `artifacts/c4-q0-ephemeral-cng-provider-tests-v2.json`에 있다.

비밀 없는 인수 시험은 일반 PowerShell `$null`을 C# 문자열 함수가 `empty`, `NullString.Value`를 `null`, 빈 문자열을 `empty`로 받았음을 확인했다. 보정된 키 생성에서는 실제 `KeyNameIsNull=True`와 `IsEphemeral=True`를 각각 확인했다. `KeySize=3072`, `ExportPolicy=None`, 공개 modulus 384바이트·exponent `010001`, 개인 blob 내보내기 거절, SHA-256/PKCS1 서명과 독립 공개 검증, 변조 본문 거절, RSA/CNG 폐기 후 사용 거절도 통과했다.

V2 소스 SHA-256은 `4890196EC9D3C75C10EF286D1BD5F679230FC7BDB6E10E685186E4F138ED609B`이다. 합성 공개키 지문 `20AF0ACC660A8873A3CB724B967BA527641ACD877C7F44F28B04766BC8B19347`은 이 한 번의 단명 시험에만 해당하며 운영 키가 아니다. 개인키 바이트를 반환·출력·저장·직렬화하지 않았고 `finally`에서 객체를 폐기했다. V1에서 만들어진 키의 식별 근거가 없어 다른 키 삭제를 시도하지 않았다.

이 결과는 제공자 API의 비영속·비내보내기 및 서명 호환 부분에 한한다. 외부 권위 발급, 실제 계획 서명, 의존성 폐쇄, 게시와 루나 독립 검수는 수행하지 않았다. `DependencyClosure=false`, `LunaReviewed=false`, `WholeAccepted=false`, `PublicationApproved=false`이며 자기 수용은 하지 않는다.
