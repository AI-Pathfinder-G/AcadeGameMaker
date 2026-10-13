# 게시 원문 바이트 보존 도구

상태: **Approved**, 2026-10-02 아스트라 역할 승인. 사용자 후속 판단 위임에 따라 별도 테라 역할 `gpt-6-sol`이 구현한다. 선행 근거는 정규화 준비 제한 수용 v2와 회귀 설계 v2의 원문 보존 관문이다. 제품 코드·기존 시험·후보·역사 증거를 변경하지 않는다.

`REQ-PLAT-059`: 검증된 534 원장의 CRLF→LF 전이25개에 대해 현재 원문 바이트를 새 불변 보관 JSON에 Base64로 보존하고 원시 SHA·Git 정규화 후보 SHA·경로·전이 쌍 수·현재 원문 역할을 구분한다. 역사 검수 바이트와의 동일성은 별도 검증 없이는 선언하지 않는다.

`AC-PLAT-057`: 새 도구가 지정 원장 SHA를 먼저 검사하고 정확534 경로·원문/후보 지문 및 전이25/무변환509를 대조한 뒤25개 Base64를 복원해 원문 지문과 일치시키며 후보 지문·전이 쌍 수도 재확인한다. 성공 결과는 원문 보존 사실에 한정하며 게시·게임 수용 플래그는 모두 거짓이다.

`REQ-PLAT-060`: 기존 원문·후보·원장·출력과 경로 경계를 보존하고 오염된 입력·중복·재분석 지점·기존 출력에는 실패한다.

`AC-PLAT-058`: 정상 합성1 및 실제534 대조, 원장SHA변조·원문변조·후보변조·기존출력 거절의 서로 독립된 합성 음성4를 실제 실행해 종료/오류를 기록한다. 합성 입력은 임시 경로에서만 만들며 실제 원문·후보는 읽기 전용이다. 실제 실행 전후534 원문/후보 지문이 동일해야 한다.

정확 입력은 `artifacts/c4-publication-lf534-baseline-v2.json` SHA-256 `89BC3D6EFDD2B1344E8E6F4F74E819382F7AB2F9C67EBFE373B3BF62F4D26F9E`, `output/publication-lf534`, 현재 원문 및 기존 엄격 검사기 `qa/tools/Test-PublicationCanonicalBaseline.ps1` SHA-256 `80B277304ADA6C06CCEAC2325D258047E60366CB1579F3AE7C635D127225D7B5`다. 검사기 SHA를 먼저 검증하고 이를 호출해 경로·원장·바이트·재분석 거절을 재사용한다. 정상·음성 합성도 실제 검사기 통과/거절 경계에서 검사한다. 예상534/25는 실제 호출에 고정하며 합성은 별도 명시 매개변수로 경계를 줄이고 실제 실행과 구별한다.

새 허용 출력은 `qa/tools/New-PublicationRawLineageSnapshot.ps1`, `artifacts/c4-publication-raw-lineage-v1.json`, `artifacts/c4-publication-raw-lineage-tool-tests-v1.json`, `docs/verification/2026-10-02-publication-raw-lineage-terra-report.md` 네 파일과 합성 시험 임시 디렉터리 `output/raw-lineage-selftest-v1`다. 기존 출력 덮어쓰기·삭제·Git 객체/인덱스/설정·네트워크·Unity·빌드·게시를 금지한다. 실제 도구 출력은 지정 새 JSON 하나이며 대상이 있으면 중단한다. 사전/사후 지문 및 실제 호출 시각·종료값·최종 도구 지문을 시험 원장에 기록한다.

새 도구·보관 JSON을534 후보에 자동 포함하지 않는다. 모든 원문 역할은 `CurrentRawBaseline`이며 역사14전이를 복원했다고 주장하지 않는다. 별도 루나가 최종 소스·실행 증거를 독립 검수한 뒤 아스트라가 제한 수용한다. 한도·인증·모델 오류는 반복하지 않는다.
