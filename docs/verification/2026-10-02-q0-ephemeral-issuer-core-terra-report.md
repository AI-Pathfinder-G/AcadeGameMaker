# Q0 단명 발급 세션 핵심 — 테라 구현·실행 보고

- 승인 계약: `docs/specs/work-contracts/2026-10-02-q0-ephemeral-issuer-core-implementation.md` (`Approved`). 구현 소유 `REQ-PLAT-068/069/070`, 부분 검증 `AC-PLAT-067/068`.
- 계약이 허용한 새 네 파일만 작성했다. 기존 Q0 공개 검증 핵심, 제공자 V1/V2 도구와 15/10개 결과, V1 키 정리 기록, 보호 원문·후보·설정·Git·원격은 변경하지 않았다.
- 모듈은 생성 시 대문자 64자리 계약·권위 SHA와 `CurrentPublicationCleanClone` 문맥을 고정한다. CNG 실제 null 이름, RSA 3072비트, `IsEphemeral=True`, `ExportPolicy=None`, 공개 지수 `010001`을 관문으로 검사한다. 반환 가능한 세션 식별자와 공개 정보에는 개인키가 없으며, 개인키 내보내기·저장·복원 기능은 없다. 세션은 단조 `Stopwatch`로 기본 45분, 허용 정수 0–45분을 관리하며 한 번의 성공 서명 뒤 재서명을 거절한다. 만료·폐기 때 객체를 해제한다.

## 실제 실행

`pwsh -NoProfile -File qa/tools/Test-Q0EphemeralIssuerCore.ps1`을 승인된 권한 경로에서 한 번 실행해 **20/20 통과**했다. PowerShell `7.6.5`, .NET `10.0.11`, 제공자 `System.Security.Cryptography.CngKey` / `Microsoft Software Key Storage Provider`, 프로세스 관리자 역할 `False`를 관측했다. 각 시험의 이름과 실제 거절 사유는 `artifacts/c4-q0-ephemeral-issuer-core-tests-v1.json`에 있다.

생성 단계의 비정규 계약·권위 SHA, 과거 문맥, 음수·소수·상한 초과 TTL을 거절했다. 서명 단계에서 다른 계약·권위, 과거 문맥, 비정규 계획 SHA·nonce를 거절한 뒤 같은 세션에서 유효한 합성 서명이 성공했다. 생성된 384바이트 RSA 서명은 기존 `Test-Q0SignedPlanAuthority.Core.psm1`의 엄격 JSON·원시 계획 SHA·공개키 지문 검증에서 통과했다. 그 후 재사용, TTL 0의 즉시 만료, 폐기 후 서명·공개 정보 재접근을 거절했다. 모든 합성 세션은 시험의 `finally`에서 폐기했다.

| 새 파일 | SHA-256 |
|---|---|
| `qa/tools/Q0EphemeralIssuer.Core.psm1` | `020C7902BB1897CEE7553A1B2123DFD62E6B9D711A8844085A8FDBEC25143475` |
| `qa/tools/Test-Q0EphemeralIssuerCore.ps1` | `CDDF78A31BF831C1B2DB786E1CA9AB97DCA6051FAAC15A32C5416650B7D0E9E4` |
| `artifacts/c4-q0-ephemeral-issuer-core-tests-v1.json` | `77631C8FBC86651E330B784F5D056527369D6BE2033AFC9A187285BC46B8304A` |

한 번의 합성 공개키 지문 `6860AE6C766991538678B8CF1C052E8584A11B4EBFF1AF5DE1F61497E7D1228C`는 운영 고정 키가 아니다. 모듈은 외부 승인 루트의 진위를 인증하지 않으며 실제 계획·권위 기록·실사용 키·원격 객체를 발급하지 않았다. `DependencyClosure=false`, `LunaReviewed=false`, `WholeAccepted=false`, `PublicationApproved=false`이다. 루나 독립 검수와 운영 발급 승인 및 최종 수용은 별도이며 자기 수용은 하지 않았다.
