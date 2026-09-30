# C3 R11 실행 연결 전 선행 단계 수용

2026-09-29 22:24 한국 시간. 아스트라, 실제 `gpt-6-astra`.

[ADR-0036](../adr/0036-staged-confirmation-and-execution-acceptance.md) 및 Approved [C3 계약](../specs/work-contracts/2026-09-28-vd09-m5d7q-c3-confirmation-owner.md)에 따라 **R11 확인 소유자 구현의 C4 진입 전 선행 단계를 수용한다.** 추적은 `REQ-M5D7QC3-001..007`, 선행 범위의 `AC-M5D7QC3-001..006/009/010`이다. 전체 C3 Verified 또는 C4 구현 승인은 아니다.

## 독립 검수와 실제 결과

루나의 [최종 선행 독립 검수](../verification/2026-09-29-c3-r11-pre-c4-final-luna-review.md), SHA `0DF2C42F361B5676FAE13EF11F031BAF3BC4278EB33098CF866857E766AEFC6A`, 및 [최종 필수 회귀 검수](../verification/2026-09-29-c3-r11-required-regressions-final-luna-review.md), SHA `C6CBD31A22CC28C542CC7D7071FF1741ECA959FA03D37A65465D97889CD12220`, P0/P1=0을 확인했다. 구현 주체의 자체 판정을 독립 수용으로 사용하지 않았다.

| 실제 실행 | 결과 | 비교 기록 SHA |
| --- | --- | --- |
| 편집 집중 행렬 | 91/91 통과 | `5025B9D60B0CDC98D8E9337C52ED0BE0A1E7D106E08B85D75C67EF277FFEA748` |
| 편집 집중 나머지 | 149/149 통과 | `0DEC6C65A06FC49E24BBD3968DF805FC10DD820F9E826E2D0B49E105D05903BF` |
| 실행 모드 집중 | 15/15 통과 | `85C038941F678EC0E01000A2CA05321974E7BC58238EBDFA7AE3F1CC1DE4A246` |
| 필수 편집 회귀 | 562/562 통과 | `B8CF8ED6893C1B920144228662237796553661697B0461BFED01C730E0F19E25` |
| 필수 작업자 회귀 | 51/51 통과 | `C91392DF16E2E94C995721BB720912C0BBD2048768D34C7AC66458D84EAB36A6` |
| 필수 실행 모드 회귀 | 610/610 통과 | `A9EA3256E3AD4505B1ECB9262F355F61EB15AEB008AB6C1DB26F39947704DA5E` |

모든 실제 XML의 실패·건너뜀·판정불가·이름 차이·중복은 0이며, 각 실제 편집기와 QA 도구 및 전체 순차 실행 종료값은 0이다. 편집91과149는 서로소이고 정확 합집합240이 원래 집중 선택과 일치한다. 내부377행은 실제 통과한 행렬91개 사례에 결속하며 계획377·통과377·실패0·대조377·증거 일치를 확인했다. 377을 추가 NUnit 사례 수로 합산하지 않는다. 겹치는 선택의 실행건수 합계1478을 고유 시험 수로 기록하지 않는다.

순차 실행 결과 `artifacts/c3-r11-final-validation-queue-result.json`, SHA `7E1C67C2397A32B9AD3DBD1E3E11E981D4C500634737D2E9E6DCF3B0D608AB75`, 및 아스트라의 직접 원시 대조 `artifacts/c3-r11-root-final-execution-comparison.json`, SHA `115124CF926B0AF9DA1A0AD1D6B9B16AF5F978DCE464B96CF5680D0A11E1633D`를 확인했다. 실행 비교 원장의 PreC4Accepted=false는 자동 수용을 하지 않았다는 당시 상태로 보존한다. 선행 수용은 이 별도 기록에서만 부여한다.

## 정확 소스·API 동결과 수용 범위

최종14파일 원장 `artifacts/c3-upper-r11-frozen-source-manifest.json`, SHA `0A36C9E7CCF3EBBF477B96C1B742344021D642586AD906042E48C755258571EB`의 모든 현재 바이트를 확인했다. 여섯 실행의 12개 전후 입력 기록은 각각884개이며 실행 전후·실행 간·현재 입력의 경로와 지문 차이가 모두0이다. C1 디스크 소스는 `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`, C2 메모리 소스는 `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275`로 기존 독립 수용 지문을 보존한다.

Q-A/Q-B 원래 요청·이력, 확인/취소의 원본 capability, 취소 후 새 입력 세대, 첫 실제 Submit 폐기, 정상 Disable 후 늦은 intake 거절, lower 예약과 upper 폐쇄의 일관 단위를 선행 범위로 수용한다. 실제 `CommitForExecution`은 lower Reserve→결정 capability 무효화→Q-A/Q-B 폐쇄→ExecutionCommitted→lower Complete 순서를 따른다. 이 요청의 lower Completed 이력은 향후 C4의 실제 실행 소비와 구분한다. C1 Begin·C2 Finalize를 이 C3 소스가 연결했다고 주장하지 않는다.

동결 API는 원장의 정확 소스 선언을 기준으로 한다. Owner의 `AcceptNewGame`, `RetryIntake`, `Confirm`, `OpenFreshDecision`, `Cancel`, `Rearm`, `CommitForExecution` 및 lower의 원본 관찰·확인·예약·완료·폐쇄 경계를 후속 설계의 입력으로 사용한다. 새 normal issuer·identity/proof 추출 getter·public ABI·asmdef/friend·역방향 허브 참조를 추가한 것으로 간주하지 않는다. 후속 소스 변경은 별도 Approved 계약과 새 동결·회귀를 요구한다.

AC005 취소 증거는 각183개 관찰 항목을 가진 실제 시험 묶음의 파일·메모리/실행 대상 부재·actions/maps·영수증·장면/원정 관찰 범위에 한정한다. 전체 제품 게임 세션의 모든 상태 보존을 입증한 것으로 확대하지 않는다. R6~R10 실패와 보정 원문은 역사로 보존하며 이번 통과로 소급 변경하지 않는다.

## 남은 단계

`AC-M5D7QC3-007/008` 전체는 **Open/Not Verified**다. 실제 실행 결과 발급·장벽 이후 종료·C3/C4 공동 결과 출처는 Approved C4 구현 이후 공동 최종 검증해야 한다. 선행 단계의 구조·부분 실행 증거를 전체 기준 통과로 바꾸지 않는다.

C4는 **Review**이며 [정확 계약 설계의 제한 수용](2026-09-29-c4-r3-exact-contract-design-limited-acceptance.md)까지 준비했다. 이번 선행 수용이 C4 계약을 자동 Approved로 바꾸거나 신규 runtime·QA 도구 작성을 허용하지 않는다. 구현 전 owning spec의 정확 승인, 구현 후 새 source/meta·선택·입력·행 원장과 집중·필수 회귀, 루나 독립 검수 및 아스트라 공동 통합 수용을 유지한다.

문서137개 PR#3 병합 및 위키 게시의 기존 영수증은 유효하며, 최초 게임 코드 게시 후보의 다섯 차단은 그대로다. 원래 로컬 main/HEAD와 인덱스·미완료 작업물은 보존했다. 이 기록은 게임 코드 업로드 승인이나 깨끗한 최초 복제 수용을 부여하지 않는다.
