# C4 미반환 부모 fullname r6 한정 재검수

- 검증기: `artifacts/c4-verify-required-rows.ps1` SHA256 `E946A1C40ABC0B29C0524D6366E3D16DF30CF5CF70FC6F61212C26B531DF3F59`.
- 국소 검사: `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r6.json` SHA256 `F4461502341D53A50C3E01978EF0342D1DD78C10EB05448EFB003757AFE5A554`.
- 판정 범위: 두 proof 손상 부모의 fullname allowlist 보정. Unity·컴파일·실제 CWT 실행은 하지 않았다.

## 결과

proof 손상 부모의 이전 P1은 닫혔다. 검증기는 다음 두 full name만 allowlist에 둔다.

- `AcadeGameMaker.Tests.EditMode.InputUnity.ProfileResetExecutionBridgeV1Tests.AC005_ActualNestedPreparedProofRootNullStopsBeforeFinalize`
- `AcadeGameMaker.Tests.EditMode.InputUnity.ProfileResetExecutionBridgeV1Tests.AC005_ActualProofPairRootDamageStopsBeforeCutover`

r6 국소 검사는 이 두 부모를 포함해 다른 namespace와 test class를 사용한 동일 메서드명 4건을 거절했다. 179/179 검사가 일치했고 보고서는 synthetic-only, 실제 CWT 미관측, Unity 미실행, 전체 수용 false로 표시한다.

그러나 전체 부모 범위 P1은 아직 남는다. `AC008_ActualC4CheckpointFailureNeverRestoresAuthority(17)` 검사는 여전히 `\.AC008_ActualC4CheckpointFailureNeverRestoresAuthority\(17\)$` 접미부 패턴이다. Approved 규칙은 해당 정확 부모의 17번 행 하나만 허용하므로, 이 분기도 동일한 전체 NUnit fullname으로 제한해야 한다. r6의 `wrong_parent_fresh17`은 메서드명이 다른 경우만 거절하며, 다른 namespace/class의 같은 메서드명을 거절하는 검사가 아니다.

또한 proof allowlist는 PowerShell의 기본 대소문자 비구분 `-notin`을 사용한다. 실제 레코드와 NUnit XML의 후속 대조는 문자열을 ordinal 비교해 잘못된 case가 최종 실행 검증을 통과하지 못하게 하지만, 계획 단계 allowlist 자체를 규범의 ordinal exact 비교와 일치시키려면 case-sensitive 비교 연산자를 쓰는 편이 정확하다. 이는 proof 부모의 실행 결과 수용을 우회하는 결함으로 확인되지는 않았고, 위 checkpoint17 fullname 잔여와 함께 좁게 보정할 수 있다.

## 요약

P0=0, P1=1. 두 proof 부모의 namespace/class/fullname 범위는 닫혔으나 checkpoint17의 동일 fullname 제한은 남아 있어 전체 이전 P1을 완전히 닫았다고 판정하지 않는다.
