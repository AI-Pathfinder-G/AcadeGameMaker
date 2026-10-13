# Q0 신뢰 자식 실행 준비 한정 수용

상태: **Approved — 준비 범위 한정 수용**. 2026-10-03 실제 `gpt-6-astra`가 테라 구현 보고와 루나 독립 보고 및 원시 자료를 읽고 판정했다. 적용 계약은 `2026-10-02-q0-trusted-child-execution.md` SHA `4F9C58EA0CFDA8318C7A2D6FB345C5B9484E9B9E07458DE898993DB8CCE32925`, 정밀도 보정 계약은 SHA `5787810A884ED916BDD2ED2DDFE2814A5B88656AD67D45F3B6E473028D6C07F2`다.

다음 표기는 고정된 판정 식별자이며, 호출자가 전달하는 승인 불리언이나 운영 서명을 대체하지 않는다.

- AcceptanceId: `AGM-Q0-TRUSTED-CHILD-PREPUBLIC-V2`
- Decision: `AcceptedLimited`
- ReviewerModel: `gpt-6-astra`
- AcceptedScope: `PrePublicPreparation570AndKeylessSupportAndInt64Evidence`
- PrecisionP1: `ClosedWithinAcceptedScope`
- OperationalKeyStartAuthorized: `false`
- WholeAccepted: `false`
- PublicationApproved: `false`

## 정확 근거

| 자료 | 경로 | 원시 SHA-256 |
|---|---|---|
| 준비 | `output/q0-trusted-child-evidence-r1/preparation-v2.json` | `9D078CF1AE59C0417B1C8E1BD8D215B167010B556F0F215A6A54755F75282029` |
| 채널 | `output/q0-trusted-child-evidence-r1/channel-tests-v2.json` | `263F54645DC207C5B7B0EAF380CDB031D3A192B938F8D93EB90882B3044C29A2` |
| 유니티 | `output/q0-trusted-child-evidence-r1/unity-compile-v2.json` | `8E3395BBCC83083A875072FF6BCE20D8ECC3280D29B266910C404FCFC40BA267` |
| 테라 | `docs/verification/2026-10-02-q0-trusted-child-terra-report-v2.md` | `89318565B96ED9D4BEDA7B9BFF10E96E6E2C5A52A2B88AA91C6E76BE3355BDDA` |
| 루나 | `docs/verification/2026-10-02-q0-trusted-child-luna-review-v2.md` | `5AB8A9B910601E7D903CE8168DD855E25405F6319140F3B5DEE2615109D8B68D` |

`REQ-PLAT-061/062`, `AC-PLAT-059/060`의 키 없는 준비 부분과 `REQ-PLAT-068/069/070`, `AC-PLAT-066/067/068`에 필요한 증거 정밀도·선행 결속 부분을 수용한다. 후자의 운영 기준 전체가 충족됐다는 판정은 아니다.

아스트라가 현재 준비 원시 570개 경로의 길이·SHA를 다시 읽어 불일치 0을 확인했다. 공개 검증기 네 리터럴은 모두 `UNISSUED`이며 현재 지문은 `6EE265B41418C12ACF69299A55DC6A3C4E7A63E20AFF71F7F4BB61047FE56378`이다. 루나는 엄격 UTF-8·중복키·모든 숫자 토큰 17/77/603개와 원문 객체의 전체 결속을 독립 확인했다. 아스트라는 수집기 5768바이트/SHA `67A1ABD7A0C6F35DB063BA568CE472FBEF37505B26B5965C52EF5F2F31BF8838` 및 실제 Int64 `639265539138890254`, 네 유니티 원문 영수증 지문과 내장 시작 ticks를 직접 재대조했다.

키 없는 실제 채널 71개·사건 14개·종료 0과 현재 C# 바이트의 유니티 04 지원 관측을 한정 수용한다. 04 영수증 SHA는 `1FB0AFDD32ACBAA9A0E476BF6BEE06727E70CFD16507874EBA173B16912B04F5`이며 실제 시작 ticks는 `639265547678170929`다. 공개 벡터 4건, 실제 파이프 피어, 순번 집합 1..4·최종 4, 보호 501개 불변을 확인한 루나의 실제 관측을 사용한다. Root의 570개 전후 읽기는 별도 원시 파일 없는 보고 증언으로만 유지한다. 아스트라나 루나의 당시 전후 실행으로 소급하지 않는다.

정밀도 P1은 위 수용 범위에서 폐쇄하며 추가 P0/P1은 확인되지 않았다. 준비 v2의 `KnownCurrentP1=1`과 대기 표기는 발급 당시 상태로 보존하고, 이 문서가 이후 폐쇄 판정이다. v1 채널·유니티·준비·테라 보고의 정밀도 실패, 첫 수집의 원시 자료/종료 미관측, 유니티 01/03 실패는 그대로 보존한다. 02는 이전 C# 바이트의 역사다. 실패를 성공으로 소급하거나 기존 파일을 수정하지 않는다.

## 후속 기술 경계

향후 `CurrentPreparationEvidence`는 정확 두 역할 `Preparation`→위 준비 v2, `NegativeTests`→위 채널 v2를 결속하고, 준비 v2가 유니티 v2를 결속한다. 현재 `PreparationRows` 16개는 역사 계보로 유지한다.

최종 `CurrentSourceRows` 570개의 `PreparationSha256`는 준비 v2의 동일 경로 SHA다. 공개 검증기 한 파일의 네 리터럴만 정확 한 번씩 치환하고 역치환 지문이 위 준비 SHA와 같아야 하며 나머지 569개는 동일해야 한다. 역할은 실제 소유에 따라 C#4=`CSharpSemantic`, 메타=`Meta`, 후속 QA=`QaSuccessor` 또는 `CanonicalTool`, 고정 검증기=`Verifier`, 스키마=`Schema`, 나머지 신규=`New`, 기타=`Source` 또는 `Helper`로 기록한다. 역할 이름은 바이트·집합 검증의 대체물이 아니다.

`SourcePaths`는 최종 입력 원장의 준비 소스 20개와 승인 신규 5개의 고유 합집합인 의미 도구 25개 관점으로 한정한다. 전체 포함·실행 입력의 권위는 실제 `IncludedPaths`/`CurrentSourceRows` 570개이며 두 TMP 공백 경로를 포함한다. `StaticReview`에는 위 실제 루나 v2 경로·SHA를 기록하고 역사 검수를 구분한다. 새 필수 계획 필드나 입력 파일을 추가하지 않는다.

별도 발급 제어 계약 SHA `19DCFF8E4B323B9E7BAAEC61264541029DA758CDADA788B3FCE9F28514538364`의 도구 구현·키 없는 검수·정확 도구 SHA 수용은 아직 별도 관문이다. 준비 수용을 이 도구 완료와 혼동하지 않는다. Boot은 준비 570·계약/도구 지문·실제 소유 TTY·예약 실행복제 경로를 확인하며 아직 존재하지 않는 큐 Ready를 요구하지 않는다. 공개키 발급·네 자리 동결·실행복제/Git·정확 두 문서 게시/인증 뒤에 큐 Ready를 만들고, Sign 단계에서 실제 생존 큐 신원·최종 계획 원문/nonce와 아스트라 기술 검토를 결속한다. 이 순서 해석은 새 키의 즉시 개시 승인이 아니다.

서명자는 45분 단명이고 키 없는 장수명 큐는 별도 소유 세션이다. 서명 후 루나 최종 사전검수와 실제 아스트라 개시 판정을 모두 Ready 45분 안에 완료한 뒤에만 Authorize를 전송한다. 실패 세대의 자동 재발급·재서명은 없다. 원격 범위는 원계약과 `docs/approvals/2026-10-02-q0-execution-public-authority-r1.json` 두 문서만이다.

QA 104건은 성공 운영 출력의 단일 JSON·표준오류 분리를 입증하지 않는다. 실제 서명 계획/외부 권위/소비, 아홉 선택 1654건, 실제 XML·188/377행 및 전체 게시 수용은 미완료다. `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, `AC-M5D7QC3-007/008`의 전체 실행 결과와 과거 전체 P1을 이 문서로 종결하지 않는다. 이번 참여는 실제 아스트라의 문서·원시 지문 검토이며 테라 구현, 루나 실행·독립 검수, 모델명이 확인되지 않은 Root 기계검수와 구분한다.
