# C4 Play 현재 소스 고정 보정안 독립 검토

검토 대상은 제안서 SHA-256 `437E933ADA4D49D069ACB2CE63807B5A10E76A9B3366FEB28EC32B78C7229B39` 및 Approved R1 Play 보정 계약 SHA-256 `2F07A599BCA19A515F9FD2644124D23480676B8FC74A5F84207C729BCD0DFC94`다. 원본·시험 파일과 실행 결과는 수정하지 않았다.

제안은 실패 원인과 범위를 정확히 좁힌다. 현재 Play fixture `C4ActualExecutionPlayFixtureV1.cs`의 SHA-256은 `215A813596E2C19B8FBAA49B070CC30A4AA448A057F618E0ECF5ADA34198D6E3`이며, `AssertFrozenNoOpSingleCallSource`가 v1 원장의 이전 Bridge SHA를 고정 비교한다. 현행 v11 원장과 실제 Bridge SHA는 모두 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`다. Owner의 v1·v11·실물 SHA는 모두 `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7`로 같아 변경할 필요가 없다.

제안된 수정은 Play fixture 한 파일의 낡은 참조를 현재 동결 원장으로 바꾸고, 새 원장의 `Runtime` 경로·종류·SHA와 실제 파일 바이트를 계속 대조한다. Bridge와 Owner의 정확 경로 및 중복 항목 부재 검사가 유지된다. no-op 실행 표현식과 C1·C2·proof/result 관찰 토큰 7개의 정확히 한 번 출현 검사도 남는다. 별도 `AssertFrozenOrdinaryWriterSource`의 v1 참조는 변경하지 않아, 기존 일반 저장소 출처 검사는 역사적 기준에 그대로 결속된다.

제안은 32개 테스트 이름·선택 순서·기대 동작·제품 코드·QA 규칙을 바꾸지 않으며, 기존 0/32 진단과 앞선 19/32 결과를 역사로 보존한다. fixture SHA가 바뀌면 후속 source manifest, 입력 목록, 선택/행 자료의 source 결속 및 실행 계획을 새 버전으로 발급해야 한다. 원장 변경만으로 실패한 Play 동작을 통과 처리하거나 이전 검증 결과를 덮어쓰는 경로는 제안하지 않는다.

## 판정

P0 0건, P1 0건. 설계는 stale Bridge 지문만 바로잡고 원래 단일 호출 검증 및 저장 경계 검사를 보존하는 한정안으로 적절하다. 이는 설계 검토 결과일 뿐 구현 승인, 실행 성공, 32개 사례 통과 또는 전체 C4 수용을 뜻하지 않는다. 새 지문·입력·선택/행 결속을 발급한 뒤 독립 정적 검수와 승인된 실행이 필요하다.
