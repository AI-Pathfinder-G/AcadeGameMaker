# Q0 외부 권위 발급 준비 v3 구현 보고

작성 역할: 테라(gpt-6-sol). 이 문서는 구현·자체 시험 기록이며 독립 수용 판정이 아니다.

## 범위와 기준

승인 계약 2026-10-02-q0-strict-numeric-and-process-cleanup.md, SHA-256 55DAED7944E63010D6F278E8316BC8BC1EB99C51D7BAD12CCEC35C9380F7C84D에 따라 REQ-PLAT-068/069/070의 키 없는 준비를 보정했다. 검증 근거는 AC-PLAT-066/067/068에 한정한다. 원래 발급 계약 SHA-256 451998AB32068DF1879E76B0A4CEC2ACB13619CDE7D48F25CB1E0E25A88AADEC와 불변 issuer V2 모듈 SHA-256 0F6937BD52732C49E979C8F8A3F897C81733DD21028BE6EEB194AC0B381D012A를 다시 확인했다.

원시 JSON 숫자 검사는 증거의 중첩 필드를 포함해 종류, 정수 토큰, 범위, 필수 집합 순서로 검사한다. 발급 전 진행자도 동일한 엄격 읽기 함수를 호출한다. 정리는 세션·입력·모듈·시계의 중첩 후속 시도를 기록하고, 키를 소유할 전용 자식의 자발 종료 또는 정확 소유 핸들 종료를 부모가 관측하도록 구현했다. 시작 후 relay·관측 단계의 예외는 최상위 finally 정리 대상으로 남으며 HostError나 종료 미관측은 발급 성공으로 처리하지 않는다. 실제 운영키로 오류를 유발하지 않았다.

## 실제 시험

qa/tools/Test-Q0ExternalAuthorityIssuance.ps1의 격리 합성 실행은 45/45 통과했다. 시험용 RSA는 합성 복사본 안에서만 사용했고 운영키 생성은 0건이다. AC-PLAT-066/067/068에 연결된 관측은 다음과 같다.

| 범위 | 관측 |
|---|---|
| 입력 5경로 | 열린 표준입력 무응답의 제한 종료, EOF, 정상 한 줄, 잘못된 형식, 잘못된 UTF-8을 자식 프로세스로 시험했다. 다섯 결과 모두 입력 닫기·폐기 후 재사용 거절·finally 실행을 기록했다. |
| 숫자 원시 형식 | 8개 의미군마다 허용 정수/경계와 문자열, null, 불리언, 배열, 객체, 소수, 지수, 범위 위반을 원시 JSON으로 치환했다. 8행 모두 통과했다. 별도 숫자 변조·중복키 13사례도 거절됐다. |
| 정리 4경로 | 첫 세션 닫기 예외, 중간 입력 닫기 예외, 정리 지연, 무응답 자식 강제 종료를 실제 자식 관측으로 확인했다. 오류 뒤에도 남은 단계 시도가 기록됐다. |
| 부모 감시 | 자발 종료, 시작 후 합성 예외, 기한 초과 강제 종료에서 정확 시작한 자식의 종료 관측을 확인했다. 시작 후 예외는 HostError로 남는다. |
| 공개 검증 | 지정 네 공개 자리만 고정한 합성 검증은 긍정 대조를 통과했고 원시 계획·권위·소스·스키마 변조 및 일반 호출은 거절됐다. |

숫자 의미군의 정상 경계는 SchemaVersion=3, CandidateFileCount=20, 입력 경과 1/4999ms, 입력 제한 200ms, 입력 종료값 0/2, 정리 자식 PID 1/2147483647, 정리 경과 1/4999ms, 정리 종료값 -2147483648/2147483647이다. 각 행의 정확 치환 토큰과 결과는 negative-tests-v3-draft.json의 NumericFieldMatrixRows에 기록했다. 준비·음성 초안은 동일 PreparationId db8657603b0d4661b33d14a8cfa23492, 현재 소스 20행, 숫자 8행 및 45사례를 결속하며 공유 엄격 파서로 재읽기 통과했다.

## 바이트 보존과 변경 집합

수정 전 실패 기록 sources-v2-failed.json은 현재20소스와 v2 준비/음성 JSON·테라 v2 보고의 원시 23개 항목을 Base64·길이·SHA로 보존한다. SHA-256은 F9AD33E2EBAF01FB8FA98E04B50FA7DF87D923166B9FB6D82F72D5CD41A2243F다. 보존 기록에서 원래 소스를 복원한 건수는 0이다. 기존 실패 증거는 삭제·덮어쓰기·소급 수용하지 않았다.

| 소스 | 보정 전 SHA-256 | 보정 후 SHA-256 |
|---|---|---|
| qa/tools/Invoke-Q0ExternalAuthorityIssuance.ps1 | 9607242503F5F6C16445356B0D4716E37F89F1C9716B2D29A0539A97A8161836 | 2130411C090451F5405B0675A61A065BAF8D64271D8D47779D8931933BA513F0 |
| qa/tools/Test-Q0ExternalAuthorityIssuance.ps1 | B560E9F18405D5C4295F0E2CED2FAFBCD7727FCD2E0DB9515810C8A92C476DB5 | 62FF70074F0A33F03A0FE7AAD90CA00434B11B2AB21ECB0342D5292B529B040A |
| qa/tools/Test-Q0FixedPlanAuthority.ps1 | 5F337B208D58988C4AB43B91EC58ACAA90E48EE43E2B0A7C09CB11FE9DAF0E9E | B63BEFE421CCF9C3E6FCE9CD422CF8C3A279E3EAE26D8F5CE4EF8548D3E98592 |

현재20소스 중 변경은 위 3개, 나머지 17개는 보존 기록과 바이트 동일하다. 허용된 네 번째 스키마 artifacts/c4-canonical-authority-plan.schema.json은 SHA-256 F35D4CDCB2C1702D6532BF14463295B6516D08E5CEF28609C0320FA60EAF0F6B로 불변이다. 원본 issuer V2 모듈도 불변이다. 세 PowerShell 소스의 구문 검사 오류는 0건이었다.

수정 가능한 초안 preparation-v3-draft.json SHA-256 685CE7EA4E6B486E542CDBFD40F389AABA3E946E89B47010F68646E07FC54AE3, negative-tests-v3-draft.json SHA-256 5A9B6CAEB1815256BF8809D567A18150A589237A39E3F170CEF12D4C170615BE를 작성했다. 이 두 초안의 CandidateRows는 현재 키 없는 20개 소스의 원시 SHA를 기록한다. 운영 공개키 지정 자리 고정 뒤 최종 20개 SHA는 별도 단계에서 다시 결속해야 한다.

## 남은 관문

최종 preparation-v3.json과 negative-tests-v3.json은 아직 생성되지 않았다. 운영키·운영 서명·원격 쓰기·Unity·Git 실행은 0건이다. WholeAccepted, PublicationApproved, LunaAccepted는 모두 거짓이다. 초안의 독립 검수와 정확 원시 바이트 결속 판단은 루나·아스트라가 별도로 수행한다. 이 보고는 그 결과를 예측하거나 자체 수용으로 대체하지 않는다.
