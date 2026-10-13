# Q0 단명 CNG 제공자 실제 검증 — 테라 보고

- 승인 계약: `docs/specs/work-contracts/2026-10-02-q0-ephemeral-cng-provider-verification.md` (`Approved`). 범위는 `REQ-PLAT-069/070`과 `AC-PLAT-067/068`의 설치 제공자 확인 부분이다. 근거 초안 SHA-256은 `64C2011B4C1D1C348EBCEDDE4A468DC6D5C59E5126880CC7622DD66F4E807E9D`로 확인했다.
- 계약의 새 출력 세 파일만 작성했다. 기존 핵심 도구와 15개 시험 증거, 534/505 후보, 설정, Git 및 원격은 변경하지 않았다.
- PowerShell `7.6.5`, .NET `10.0.11`, Windows CNG `System.Security.Cryptography.CngKey` / `Microsoft Software Key Storage Provider`에서 이름 없는 합성 RSA 키를 `Length=3072`, `ExportPolicy=None`으로 요청했다. 시험 소스 SHA-256은 `8730FEE32EE7FC14EC54A2B0EED1BBB2ED66B90B96B27CE37D535BEA637CAE4C`이다.

## 실제 결과와 미충족 관문

첫 제한 환경 실행은 `CngKey.Create`에서 접근 거부되어 키 생성 전 중단됐다. 승인된 권한 경로에서 같은 시험을 한 번 실행한 최종 결과는 **8/9 통과, 1 실패**다. 세부 관측은 `artifacts/c4-q0-ephemeral-cng-provider-tests-v1.json`에 있다.

실패는 비영속성 표시다. `KeyName`은 없지만 실제 `IsEphemeral=False`였다. 설치 참조 문서가 이름 없는 키의 비영속을 설명하더라도 이번 제공자의 실제 상태 값은 계약이 요구한 `true`가 아니다. 따라서 단명 키 안전성이 확인됐다고 판정할 수 없으며 실사용 발급은 중단해야 한다.

나머지 관측은 `KeySize=3072`, `ExportPolicy=None`, 공개 modulus 384바이트와 exponent `010001`, 개인 blob 내보내기 거절, SHA-256/PKCS1 서명의 독립 공개 검증, 변조 본문 검증 거절, RSA/CNG 객체 폐기 후 사용 거절이다. 공개키 지문은 합성 시험에서만 `9C0F6B568E14B7CA407657E5BE0A38F68F0CCF520A4C79ED4E2CB0BEAF15543A`로 계산됐다. 이 값은 매 실행에서 새로 생성되는 시험 키의 지문이며 운영 고정 키가 아니다. 개인키 반환·출력·저장·직렬화는 없었고 `finally`에서 객체를 폐기했다.

`AC-PLAT-067/068`의 제공자 관문은 실패 상태다. 외부 권위 루트, 실사용 키와 서명, 전체 의존성·게시 및 루나 독립 검수는 이 시험 범위 밖이며 미수용이다. 결과 플래그 `DependencyClosure=false`, `LunaReviewed=false`, `WholeAccepted=false`, `PublicationApproved=false`를 유지했다. 자기 수용은 하지 않았다.
