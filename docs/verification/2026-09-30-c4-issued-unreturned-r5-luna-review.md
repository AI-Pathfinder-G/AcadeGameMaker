# C4 미반환 규칙 r5 독립 재검수

- 검증기: `artifacts/c4-verify-required-rows.ps1` SHA256 `E1D99FEDBA0538A0C3D92EEF3DDB219FF497F46CAD49C62129B23E1A821EF572`.
- 국소 합성 결과: `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r5.json` SHA256 `D977EC2F04824AAA6104F6B1491C1FB8C0561EC5E68F9502FF9B16DBAFB75B1A`.
- 실제 fixture 및 시험 파일 SHA256: `C4ActualExecutionEditFixtureV1.cs` `971E5D120B5540B387633BCA36F046F45A1C6A2AEC1C7D5E6F98A988C9968269`, `ProfileResetExecutionBridgeV1Tests.cs` `2C13DFC7D5C1B0EB572175F3FBD37C03DCBCBB1C181DFFBD8A3625354748E0C0`.

## 판정

P0=0, P1=1. r5는 Approved 미반환 부모·Busy 출처 보정을 실제 계획/기록 검증에 연결했지만, proof 거절 부모 허용 검사는 정확한 NUnit 정규 이름이 아니라 메서드 접미부만 검사한다.

## 닫힌 범위와 근거

검증기의 `C4-UnreturnedConfig`는 기존 proof 손상 부모와 추가된 `AC005_ActualProofPairRootDamageStopsBeforeCutover`만 허용하고, checkpoint 17에서만 `actualRootLeaseHeldAtCheckpoint=Yes`·Observed 및 `c1FreshOutcome=Busy`·SourceEstablished를 강제한다. 계획행과 실제 `after.facts` 모두 certainty를 검사하며 추가 진단이 빠지거나 값·certainty가 바뀌면 거절한다. `freshComplete`는 SourceEstablished, 실제 callback 계수기 다섯 leaf는 Observed로 유지된다.

17번 시험은 해당 checkpoint에서 `actualLease.HoldsRootAtCheckpoint(f.Root)`를 직접 확인한 뒤 동일 예외를 던진다. worker 생성은 정확 비공개 `Acquire(string)` 결속을 요구하고, 준비 신호는 실제 lease 획득 뒤에만 설정된다. worker는 해제 신호까지 기다리며, 관측 함수는 root 일치·획득·실패 없음·worker 생존·미해제 상태를 모두 확인한다. 사후 정리도 같은 worker의 root, release, lease 반환 및 Join을 검사한다. 동결 `Begin`은 실제 lock 획득 실패를 `Busy/NoBarrier` 결과로 반환하고 C4는 결과를 지역 분기로만 사용하므로, Busy를 source로 확정하고 C1 typed 결과를 관측했다고 주장하지 않는 분류가 정확하다.

추가 proof 손상 부모는 별도 NUnit 부모·행 ID로 증거를 만들고 기존 private helper를 재사용한다. 원본 CWT/실제 proof 참조/손상 후 검증 실패/observer 및 C1·C2 계수는 부모별 실행에서 다시 검사한다. 국소 r5 결과는 175/175이며 synthetic-only, actual CWT 미관측, Unity 미실행, 전체 수용 false로 표시되어 한계를 보존한다.

## 남은 P1

`C4-UnreturnedConfig`의 두 proof 손상 분기는 현재 `\.AC005_(ActualNestedPreparedProofRootNullStopsBeforeFinalize|ActualProofPairRootDamageStopsBeforeCutover)$` 접미부 패턴으로 부모를 허용한다. checkpoint 17도 같은 방식으로 메서드명 접미부만 검사한다. 이는 Approved 개정의 “정확한 부모” 범위보다 넓다. 실제 fixture가 현재 한 클래스에서만 이 helper를 사용하고 XML/ExpectedCase를 일치시키더라도, verifier 자체는 다른 namespace 또는 test class의 같은 메서드명을 원장에 선언하는 것을 막지 않는다. 해당 세 부모는 정확한 NUnit fullname으로 비교하고, 두 proof 손상 부모 외 이름을 거절하도록 제한해야 한다.

검토는 국소 스크립트·fixture·시험의 정적 대조와 전달된 합성 기록에 한정된다. Unity, 컴파일, 실제 CWT 결과는 검증하지 않았다.
