# C4 미반환 경계 QA 도구 독립 검수

검수는 Approved 일곱 미반환 경계 규칙의 행 builder/verifier 및 구현자 국소 검사만 대상으로 했다. 소스·시험은 읽기만 했으며 Unity, 컴파일, 국소 스크립트는 실행하지 않았다.

## 정확 지문

- Approved 규칙: `docs/specs/work-contracts/2026-09-30-c4-issued-unreturned-boundaries-amendment.md` — `0AEC1429EB90B924218C070DF2C07816E4B1C9CE1F9B0E648F71D6F912AD1B6D`
- Builder: `artifacts/c4-build-required-checkpoint-rows.ps1` — `29A9DCA4A1349519018FE7EB9F6F67ED49932B178216E8B915F53D15E2D2617D`
- Verifier: `artifacts/c4-verify-required-rows.ps1` — `E205E748B2A0263D01D9A95D1217C5ABF64112E4E0DCAB442ACA895875466826`
- 국소검사 원문: `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r2.json` — `CC7C897E90F541B19DB37BFF4668B572936A274C88382F8E3FB6E476B8232D2E`; 기록 문서 — `A46212EFB39417D5D03861B0FA967910F5A2D56ACEA3551C857D0A417DD5666E`
- 기준 QA r2: `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`; 세대 발급 규칙: `FF88BFC0471FB48C2828765A1C6FBD0F3913F0DD123CDEB16BCBADD4F8F2DCB6`.

## 판정

P0=0, P1=1. 이전 손상 proof 장벽 종류 문제는 현재 verifier에서 보정됐다. 남은 P1은 호출 수 certainty를 leaf별로 고정하지 않는 점이다.

Verifier의 `C4-UnreturnedConfig`는 일곱 허용 사례를 다섯 규칙으로 묶되 정확 부모 suffix, checkpoint family/name/value, 소비 전후, proof 상태, 여섯 호출 수, 결과 미발급·cleanup 범위를 고정한다. `C4-UnreturnedActual`은 원본 execution 발급 진단·fault/consume 상태·canonical generation/proof·guard/history/cleanup 및 다섯 동결 파일의 SHA를 대조한다. 새 rule은 `memoryGeneration`에만 허용하며 `executionGeneration`에 같은 규칙을 놓으면 거절한다. RequiredFacts 진단의 이름·값·certainty와 순서도 기존 IssuedWithoutResult 진단 뒤에 강제한다. 새 rule이 C1·C2 미호출이나 실제 receipt 발급 규칙을 대체하지 않는다.

추가 P1: `C4-UnreturnedPlan`의 `callCounts.*` 여섯 RequiredFact는 실제 값과 ExpectedRow를 대조하지만 certainty를 `Observed` 또는 `SourceEstablished` 중 아무 값이나 허용한다(검증기 171행). 실제 checkpoint/delegate에서 센 `c1Begin`, `c2Finalize`, `preparedObserver`, `c2Observer`, `freshReserve`는 `Observed`여야 한다. 반면 미반환 행의 `freshComplete=0`은 completion callback 관측이 아니라 발급 전 source 지배로 산출되므로 `SourceEstablished`가 맞다. 현재 검증기는 둘을 서로 바꿔도 통과시키고, 154개 국소 조건에는 certainty 교차변조 음성행이 없다. 다섯 관측 leaf와 source-only leaf를 정확히 구분하도록 field별 허용 certainty를 고정해야 한다.

손상 proof 행은 현재 plan에서 `ActualTypedC1Row` barrier kind를 제외하고 `FrozenSourceBoundary` 또는 `ActualOrdinaryWriterObservation`만 허용한다. 실제 marker source 포인터 또는 동일 사례의 실제 writer 차단 진단도 검사한다. 구현자 154/154 국소검사는 이 두 barrier 거절을 포함한다. 다른 네 규칙의 barrier 검증은 기존 정확 조건을 따른다.

`PreparedProofRejectedAfterObserver`의 규칙은 현재 승인된 `AC005_ActualNestedPreparedProofRootNullStopsBeforeFinalize`에만 결속된다. 동일 helper를 재사용하는 별도 공개 wrapper `AC005_ActualProofPairRootDamageStopsBeforeCutover`는 현재 QA row를 발행하지 않으므로 이번 exact parent 허용 대상이 아니다. 해당 wrapper에도 증거 행을 추가하려면 ExpectedRows와 validator 부모 허용 목록을 함께 바꾸는 별도 규범 범위 및 검수가 필요하다.

## 증거 한계

국소 기록은 합성 검사 154/154와 종료 0을 보고하며 실제 CWT·Unity·전체 행 검증은 수행하지 않았다고 명시한다. 따라서 현재 결론은 정적 조건 검수에 한정된다. 일곱 실제 사건의 row 출력, source·selection 동결, NUnit 결과 및 전체 C4 수용은 아직 확인되지 않았다.
