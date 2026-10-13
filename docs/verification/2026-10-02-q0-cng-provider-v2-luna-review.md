# Q0 CNG 비영속 제공자 v2 독립 검수

대상 Approved 계약 `docs/specs/work-contracts/2026-10-02-q0-cng-provider-v2-independent-review.md`. v2 시험 소스 SHA-256 `4890196EC9D3C75C10EF286D1BD5F679230FC7BDB6E10E685186E4F138ED609B`, 기존 결과 SHA-256 `5552F2D5627B9A621D1E4B24088FFA862CE94DCBEF18F1AB275370534AD62E87`, 테라 보고 SHA-256 `A6BB575C625A82E76221F2371250203867387794E800B70071FC47D59A27C2E9`를 일치 확인했다.

## 판정

- **P0: 0. 새 제공자 P1: 0.** AC-PLAT-067/068은 메모리 전용 합성 제공자 관측·서명 호환 범위에서 부분 충족이다. 외부 권위 P1과 운영 Q0 연결은 열린 상태다.
- V1 원본 결과 SHA `6C66F0EF4F8362CA614480E326BA0D57BD1753E406A75392CDD2F97752AC70F7`를 보존했다. V1은 PowerShell `$null`을 C# 문자열 경계에서 빈 문자열로 전달해 `IsEphemeral=False`였다. V2는 일반 `$null`=empty, `NullString.Value`=null, `''`=empty를 구별하고, 생성 결과 `IsEphemeral=True`, `KeyNameIsNull=True`를 관측한다.
- 허용 격리 경로 `output/q0-cng-luna-r1`에서 동일 SHA 소스를 한 번 실행했다. PowerShell 7.6.5/.NET 10.0.11, 관리자 아님, `System.Security.Cryptography.CngKey` 및 `Microsoft Software Key Storage Provider`로 10/10 통과했다. CNG 권한 거절은 발생하지 않아 반복하지 않았다.
- 격리 실행은 RSA 3072, `ExportPolicy=None`, modulus 384바이트/exponent `010001`, 개인 blob 내보내기 거절, SHA-256/PKCS1 서명 검증, 변조 거절 및 dispose 뒤 사용 거절을 재현했다. 결과에는 합성 공개키 지문과 비밀 없는 관측만 있고 개인키 바이트를 출력·저장하지 않았다. 격리 결과 SHA `EE98549141C5D8F1355ACCE052577311713750345E268A95012A1ABBF421B1`은 원본 결과와 별도 경로에 보존했다.
- 합성 제공자 검수는 외부 승인 루트·실제 운영 키/계획 서명·검증기/계획 동결을 증명하지 않는다. `DependencyClosure`, `PublicationApproved`, `WholeAccepted`, `LunaReviewed`는 거짓으로 유지한다. 제품/Unity/네트워크/Git 실행과 기존 결과 수정은 하지 않았다.

## V1 정확 소유 키 정리 증거 읽기 대조

V1 정리 JSON SHA-256 `AFD1763FD867B701A17452B1CB5BA30E6AFF40576AC1833B04F5D67C63366742`, 정리 소스 SHA-256 `A95EF4A48D30121FE93034EA68F15EADAC00CBFCD6D783C808E963B518A744CF`, 지정 정리 보고 SHA-256 `4B65429CF50E6CC3039B05722917B6DDCFFFFCD8E404EC55FFEF07B357FA5FEB`를 확인했다. JSON과 보고는 비관리자 환경 및 정확한 빈 이름·Microsoft Software Key Storage Provider의 조회, 공개 지문 `9C0F6B568E14B7CA407657E5BE0A38F68F0CCF520A4C79ED4E2CB0BEAF15543A` 일치, 권한 경로를 통한 해당 키 삭제 성공, 같은 이름 재조회 `0x80090016`을 일치되게 기록한다. 이는 보고된 단일 V1 키 정리의 증거이며 다른 키 탐색·삭제, V2 격리 키 또는 운영 키의 정리를 입증하지 않는다. 정리 실행은 재현하지 않았다.
