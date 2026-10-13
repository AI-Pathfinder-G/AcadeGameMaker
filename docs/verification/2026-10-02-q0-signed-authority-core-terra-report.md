# Q0 분리 서명 검증 핵심 — 테라 구현·실행 보고

- 소유 요구사항: `REQ-PLAT-068/069/070`. 승인 계약: `docs/specs/work-contracts/2026-10-02-q0-signed-authority-core-implementation.md` (`Approved`). 근거 설계 SHA-256: `155A43134BACC29BFD3627E99E42E827E671E770AD29BE537429106F2D23BC89`.
- 출력은 계약이 허용한 새 파일 다섯 개뿐이다. 기존 Q0/감사/C4/제품 소스, 534개 후보, 원장, 캐시, Git, 원격을 변경하지 않았다. 저장소에는 별도 작업에서 이미 존재하던 많은 변경이 있으므로 전체 작업 트리의 청결을 주장하지 않는다.
- 운영 진입점은 외부 인증된 승인 루트와 고정 공개키가 없는 준비 상태에서 종료코드 2로 거절한다. caller가 계획 SHA를 함께 제공해도 거절한다. 공개키 교체·서명 생략·합성 모드는 운영 진입점에 없다.
- 내부 핵심은 원시 UTF-8 JSON의 중복/미지/누락 필드, 정확 값·시각·절대 경로·표준 Base64·서명 길이, 원시 계획 SHA, 고정형 ASCII/LF payload, 공개키 지문과 RSA-3072/SHA-256/PKCS1 서명을 검사한다. 키 인수는 격리 시험의 내부 함수에만 있다.
- 합성 시험은 단명 메모리 RSA 객체 두 개를 사용하고 `finally`에서 폐기한다. 개인키 내보내기·파일·로그·원격 기록은 하지 않았다. 실제 운영 키를 생성하거나 서명하지 않았다.

## 실제 실행

- 호스트: PowerShell `7.6.5`, .NET `10.0.11`, 키 제공자 `System.Security.Cryptography.RSABCrypt`.
- `pwsh -NoProfile -File qa/tools/Test-Q0SignedPlanAuthority.SelfTest.ps1`: 최종 `15/15` 통과, 실패 0. 세부 각 사례와 실제 거절 사유는 `artifacts/c4-q0-signed-authority-core-tests-v1.json`에 기록했다.
- 정상 합성 1건과 계획·계약·권위·서명 변조, 다른 키, 과거 문맥, 누락·중복·미지 필드, 비정규 Base64, 잘못된 서명 길이·시각·경로, caller SHA만 일치하는 운영 호출 거절 14건을 관측했다. 형식 오류는 서명 불일치로 둔갑시키지 않고 실제 선행 거절 사유로 기록했다.
- 실행 과정에서 시험 입력을 만드는 식 오류와 비정규 Base64 시험의 예상 사유 오류를 수정한 뒤 최종 재실행했다. 최종 관측만 위 수치로 보고한다.

## 소스 지문

| 파일 | SHA-256 |
|---|---|
| `qa/tools/Test-Q0SignedPlanAuthority.ps1` | `A35D7AE6E21FFFB20F80837D201EF338BD13486A2383BDF3F9873928598795A7` |
| `qa/tools/Test-Q0SignedPlanAuthority.Core.psm1` | `CC589ADCDD1AE072ACF7E00165BCDFC8A3DADFF150E6D75A5B3084D816982738` |
| `qa/tools/Test-Q0SignedPlanAuthority.SelfTest.ps1` | `9A030B9B48BD6B536E929E89088074D76D65855906BABAEAAE736FACC763F876` |

## 인수 경계

- `AC-PLAT-068`: 핵심 형식·변조 거절·운영 우회 부재의 구현과 합성 실행 증거를 제출한다. 독립 루나 검수와 아스트라 수용은 아직 없다.
- `AC-PLAT-066/067`: 외부 인증된 루트, 실제 공개키/소스 동결, 실사용 서명 및 비순환 승인 절차는 아직 수행하지 않았다.
- 결과 플래그 `DependencyClosure=false`, `LunaReviewed=false`, `WholeAccepted=false`, `PublicationApproved=false`.
