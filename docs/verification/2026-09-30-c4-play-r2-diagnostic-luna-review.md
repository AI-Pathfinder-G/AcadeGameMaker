# C4 Play R2 진단 결과 독립 검토

검토는 제공된 진단 결과와 현재 원본 파일의 읽기 전용 대조에 한정한다. Unity 재실행, 코드·원장 변경은 하지 않았다.

## 확인 결과

`artifacts/c4-play-r2-diagnostic.xml` (SHA-256 `E093FB7539679B60B8782156A244056EA6336833A4B820C2033ABFDCEC805F84`)에는 선택된 32개 테스트가 모두 있고, 32개 모두 실패했으며 건너뜀·미결정은 없다. 모든 실패는 동일한 첫 실패 지점 `C4ActualExecutionPlayFixtureV1.AssertFrozenNoOpSingleCallSource`, `C4ActualExecutionPlayFixtureV1.cs:991`에서 발생한다. 메시지도 모두 동일하다. 과거 Bridge SHA `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`를 기대했으나 현재 Bridge의 SHA `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`와 달라 즉시 중단됐다.

실제 원인은 fixture가 `artifacts/c4-frozen-source-manifest-v1.json`을 읽고 그 안의 이전 Bridge 지문을 고정 비교하는 것이다. 현재 기준 원장 `artifacts/c4-frozen-source-manifest-v11.json`에는 Bridge가 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`, `NewGameConfirmationOwnerV1.cs`가 `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7`로 기록되어 있고, 두 실제 파일의 지문과 일치한다. stale한 값은 Bridge 하나뿐이다. fixture의 원래 단일 호출 검증은 이 지문 assertion보다 뒤에 있으므로 이번 실행에서 호출 횟수·보호 경로·11개 이전 실패군의 수정 여부는 관측되지 않았다.

입력 전후 원장 스냅샷은 각각 1,017개 경로이며 파일 목록과 지문이 같다. SHA-256은 `artifacts/c4-play-r2-diagnostic-source-before.json`이 `ACA620CA6CEF3EED706A7CE83AF2C1FFDA03CD9ADF924D5388D936119D692D1C`, `...source-after.json`이 `252E7DBAFAB1189951381AF0134D19E7DB5D75E0861E60348C4231C0F544AFDE`다. 로그 SHA-256은 `8C50F43E2CB63823998E678032D8ED98357544986723E4A51CF28BF66D19A6B1`이다. 실행 결과는 실패이며 테스트 동작 통과 증거가 아니다.

## 판정

P0 0건, P1 1건이다. P1은 Play fixture의 source-freeze 검사가 이전 manifest와 이전 Bridge SHA를 참조해 32개 사례 모두가 의도한 동작 검증에 진입하지 못한 점이다. 이는 테스트 고정 지문 결속 오류이며, 이번 결과만으로 제품 런타임 결함을 단정할 수 없다. 동시에 이전 11개 포인터·AC004·AC008 실패가 해결됐다고 볼 수도 없다.

최소 보정은 승인된 현재 동결 원장에 fixture 검사를 다시 결속하고, 원장의 Bridge 및 Owner 항목과 실제 파일 바이트를 대조하는 검사를 유지하는 것이다. `AssertFrozenNoOpSingleCallSource`의 정확한 C1/C2 호출 토큰·한 번 호출 검증을 제거하거나 완화해서는 안 된다. fixture 변경에 따라 source manifest, input snapshot, 선택/행 자료와 계획의 후속 지문 결속을 새 버전으로 맞춘 뒤 실행해야 한다. 해당 재결속 및 재실행은 이 검토 범위에서 수행되지 않았다.

## 검토 지문

- R1 보정 승인: `docs/specs/work-contracts/2026-09-30-c4-play-r1-bounded-recovery-amendment.md`, SHA-256 `2F07A599BCA19A515F9FD2644124D23480676B8FC74A5F84207C729BCD0DFC94`.
- 동결 원장 v11: `artifacts/c4-frozen-source-manifest-v11.json`, SHA-256 `36B4655904C898176B79655E0243005B997A607FFBB32D1FF85E789552C85EFB`.
- 입력 전후: 각 1,017개 항목, 내용 동일.
- Unity 재실행 및 원본·결과 수정: 없음.
