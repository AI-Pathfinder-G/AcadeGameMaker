# Q0 단명 발급 세션 V2 두 단계 결속 — 테라 구현·실행 보고

- 승인 계약: `docs/specs/work-contracts/2026-10-02-q0-issuer-two-phase-binding.md` (`Approved`), 소유 `REQ-PLAT-068..070`, 부분 검증 `AC-PLAT-067/068`.
- 새 허용 출력 네 파일만 작성했다. V1 발급 세션, 기존 엄격 Q0 검증기, 제공자 V1/V2와 정리 도구·증거, 보호 원문·534/505 후보·제품·Git·원격은 변경하지 않았다.
- V2 세션 생성은 계약 SHA·`CurrentPublicationCleanClone` 문맥·단조 TTL만 고정한다. CNG의 실제 null 이름과 `IsEphemeral=True`, `ExportPolicy=None`, RSA 3072비트 관문을 통과한 뒤 공개키를 읽는다. 살아 있는 같은 세션에 별도 명시 함수로 권위 SHA를 한 번 결속하며, 결속 전 서명과 두 번째 결속을 거절한다. 결속 뒤에도 계약·문맥·공개키는 바뀌지 않고 키 교체·권위 복구 기능은 없다.

## 실제 실행

`pwsh -NoProfile -File qa/tools/Test-Q0EphemeralIssuerV2Core.ps1`을 승인된 권한 경로에서 **1회 실행, 27/27 통과**했다. PowerShell `7.6.5`, .NET `10.0.11`, 제공자 `System.Security.Cryptography.CngKey` / `Microsoft Software Key Storage Provider`, 프로세스 관리자 역할 `False`를 관측했다. 각 시험의 실제 결과는 `artifacts/c4-q0-ephemeral-issuer-core-tests-v2.json`에 있다.

합성 시험은 미결속 서명, 비정규 권위 SHA, 동일 값 재결속, 다른 값 재결속, 다른 계약·권위·과거 문맥·비정규 계획 SHA·nonce 서명, 재사용, TTL 0 만료 뒤 결속·서명, 폐기 뒤 결속·서명을 거절했다. 결속 전후 공개 modulus·exponent·지문이 동일함을 확인했다. 유효한 합성 서명은 기존 `Test-Q0SignedPlanAuthority.Core.psm1`의 엄격 공개 검증을 통과했다. 모든 세션은 시험의 `finally`에서 폐기했다.

| 새 파일 | SHA-256 |
|---|---|
| `qa/tools/Q0EphemeralIssuerV2.Core.psm1` | `0F6937BD52732C49E979C8F8A3F897C81733DD21028BE6EEB194AC0B381D012A` |
| `qa/tools/Test-Q0EphemeralIssuerV2Core.ps1` | `01727BE84692D2A6280EEFDBD1A06E5EC7F3A1C12CCB1A7A64CB527982F60F75` |
| `artifacts/c4-q0-ephemeral-issuer-core-tests-v2.json` | `410D949D832243A35AC2D3401D2F8F16F3E6EED8F217B7E000A5084CCA6B2FC8` |

합성 공개키 지문 `2D7830CC61D44DF749A225E5BC137D91379C01F5811A435F319EC26609283227`은 이 시험 세션에서만 유효하다. 모듈은 외부 권위 기록의 진위를 스스로 인증하지 않는다. 실제 운영 키·권위 기록·계획·원격을 발급하지 않았고, 개인키를 내보내거나 저장·기록·복원하지 않았다. `DependencyClosure=false`, `LunaReviewed=false`, `WholeAccepted=false`, `PublicationApproved=false`이며 독립 검수와 운영 발급 승인은 별도다. 자기 수용은 하지 않았다.
