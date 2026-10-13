# Q0 신뢰 자식 준비 증거 정밀도 독립 검수

검수 기준은 승인된 docs/specs/work-contracts/2026-10-03-q0-evidence-int64-precision-correction.md(SHA-256 5787810A884ED916BDD2ED2DDFE2814A5B88656AD67D45F3B6E473028D6C07F2)이다. 검수 범위는 REQ-PLAT-061/062/068/069/070 및 AC-PLAT-059/060/066/067/068의 준비 증거 정밀도와 결속이다.

## 독립 확인

v2 세 JSON의 원시 바이트를 직접 읽어 SHA-256, 엄격 UTF-8, 중복 키, 중첩 숫자 토큰을 확인했다.

| 자료 | 바이트 | SHA-256 | 숫자 토큰 |
|---|---:|---|---:|
| output/q0-trusted-child-evidence-r1/channel-tests-v2.json | 31,014 | 263F54645DC207C5B7B0EAF380CDB031D3A192B938F8D93EB90882B3044C29A2 | 17 |
| output/q0-trusted-child-evidence-r1/unity-compile-v2.json | 35,699 | 8E3395BBCC83083A875072FF6BCE20D8ECC3280D29B266910C404FCFC40BA267 | 77 |
| output/q0-trusted-child-evidence-r1/preparation-v2.json | 105,509 | 9D078CF1AE59C0417B1C8E1BD8D215B167010B556F0F215A6A54755F75282029 | 603 |

중복 키와 비정수·범위 초과·비정규 숫자 토큰은 0개였다. 모든 숫자를 Int64로 읽어 원문 십진 토큰과 왕복 대조했다. 2^53을 넘는 9개 토큰도 정밀하게 보존됐다. 채널의 두 값은 639265539138890254, Unity 영수증의 일곱 값은 639265460748965343 2곳, 639265467445018083 2곳, 639265542226848562 1곳, 639265547678170929 2곳이다.

채널 수집기 envelope는 Base64 복원 길이 5,768바이트와 SHA 67A1ABD7A0C6F35DB063BA568CE472FBEF37505B26B5965C52EF5F2F31BF8838이 기록과 일치했다. 원문과 RawCollectorParsed 전체 필드가 같고, 원시 StartTicks는 639265539138890254였다. 이는 수집기 StartedAtUtc와 다른 프로세스 시작 ticks다. 자체시험 원시 stdout도 3,131바이트/SHA B368A366703A29A0F00CA743611918E792442193D55C1DB6AB56AFD43AE7E04B로 복원됐으며, 71개 사례 모두 예상 분류와 일치하고 14개 사건, 실제 종료 0, timeout 없음, stderr 0을 확인했다. 다섯 입력 소스의 전후 SHA가 같고 현재 준비 복제의 SHA와도 일치했다. 첫 수집 실패는 MissingRaw·UnknownExit로 보존돼 성공으로 승격되지 않았다.

Unity v2의 각 01–04 OriginalReceiptRaw를 디스크 원문과 바이트 단위로 비교했다. 네 영수증의 원시 SHA와 길이가 일치했고 ObservedReceipt 전체 객체도 원문 파싱 결과와 같았다. 각 UnityStartTicks, NativePipePeerStartTicks, 종료 코드, 판정이 원문과 정확히 일치하며 네 UnityStartUtc도 ticks와 일치했다. 각 시도의 로그 SHA도 실제 로그와 같았다. 1차는 종료 0이어도 기록 읽기 오류로 FAIL, 2차는 이전 C# 바이트의 역사적 PASS, 3차는 제한 권한 환경의 Package Manager IPC 실패로 종료 1·FAIL, 4차는 현재 C# 바이트에서 정상 권한으로 종료 0·PASS다. 4차는 실제 pipe peer, 공개 벡터 4건, 순번 집합 1..4, 최종 map 4를 확인했고 보호 파일 501개의 변경·추가·삭제는 0이었다. 4차 XML은 생성되지 않았다.

준비 v2의 570개 경로는 570개 고유 항목이며, 현재 준비 복제에서 파일 누락과 길이/SHA 불일치는 각각 0이었다. 9개 선택 파일도 기대 SHA와 현재 파일 SHA가 모두 일치했다. 준비 v2는 채널·Unity v2의 정확 경로와 원시 SHA를 결속하고, 이전 정밀도 오류 파일 네 개를 원래 SHA와 함께 FailPrecisionSources로 보존한다. 공개 검증기 리터럴은 모두 UNISSUED이며 운영 키·권위 사용·전체 수용·게시 승인은 모두 거짓이다.

4차 전후 570개 원시 지문 동일 관측은 준비 v2가 RootReportedBeforeAfterWitness로 기록한 주 조정자 보고다. 별도 원시 witness 파일은 없다. 나는 준비 표의 570개를 현재 복제에서 다시 대조했으나, 주 조정자의 시점별 전후 읽기를 직접 수행한 것으로 주장하지 않는다.

## 판정과 경계

이 검수에서 정밀도 보정 자료 자체의 잔여 P0는 없다. 보존된 원문과 v2 자료 사이에서 정밀도 P1을 재현하지 못했다. v1 네 파일과 v1 테라 보고는 기존 SHA 그대로 실패 원천으로 남고, 성공 근거로 사용하지 않는다. preparation-v2.json의 KnownCurrentP1=1 및 검토 대기 표기는 발급된 바이트 그대로 보존한다. 최종 P1 폐쇄와 제한 수용은 아스트라의 별도 판정 대상이다.

AC-PLAT-059/060에 관해서는 키 없는 채널 71개와 현재 바이트의 Unity 지원 확인까지만 검증했다. QA 104건은 편집기 소유 기능의 회귀 기록이며 성공 운영 출력의 단일 JSON·표준오류 분리를 입증하지 않는다. 서명된 계획, 외부 권위, 소비 원장, 아홉 운영 선택, 실제 XML·188/377행 결과와 동일 입력은 미검증이다. 따라서 AC-PLAT-066/067/068 및 전체 게시 수용은 이 보고로 충족되지 않는다. WholeAccepted와 PublicationApproved는 계속 거짓이다.
