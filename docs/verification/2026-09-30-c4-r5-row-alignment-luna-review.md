# C4 R5 행 불일치 및 정렬 초안 독립 검토

- 대상 초안: `docs/specs/work-contracts/2026-09-30-c4-r5-evidence-row-alignment-draft.md` — SHA-256 `078AA8D533D967735F401815831519C6CC274E3C18DEEAEF1EB4F951D2869DA9`
- 대조 자료: row comparison `artifacts/c4-r5-focused-edit-required-row-comparison.json` — `51D162CA5B20EA91879F556BD025276B69FF56B3E78903DE2BA73B14E56C7FD2`; XML `artifacts/c4-r5-focused-edit.xml` — `15C34F08063F996D8084C96413F2F8B8002FD8680BF68C87CCCCE3282AB55C65`; ExpectedRows v4 — `536BCDB146B5D601C2570A33DC01FD31C41D317D949391BAADF17A29624BC6CD`; verifier — `74F26AC5BFD34A2ABD78814B981844CB2A9CACCADED1062973FB22665BB4CD12`.
- 범위: 읽기 전용 실제 결과·원장·source 대조. 소스 수정과 Unity 재실행은 하지 않았다.

R5 XML의 NUnit 결과는 139/139 통과하고 Unity·QA 종료값은 0이지만, 이는 AC 수용이 아니다. 독립 비교 결과 Edit 행 148개 중 86개가 불일치하여 `EvidenceMatched=false`, `WholeAccepted=false`이며 큐가 중단된 처리는 정확하다.

초안의 네 정렬 방향은 원자료와 부합한다. 55행은 C1 checkpoint 예외 51건과 C1 terminal Manual/Reload 4건이다. 실제 `FaultHistory` 관측은 모두 Yes인데 v4 기대는 No다. C1 checkpoint 경로는 Bridge의 `catch → FaultOriginalExecution`으로 영구 fault를 남기며, terminal C1 경로도 Manual/Reload 결과의 정상 종료 뒤 비완료 Router lifecycle을 폐쇄한다. 이 값만 Yes로 고치는 방향은 fault·폐쇄 증거를 약화하지 않는다.

24행에서는 최종 marker 관측이 `NoBarrier`이고 해당 사례의 일반 Save 호출이 없다. 실제 writer 차단은 Unknown이다. 이는 Approved ordinary-writer amendment의 해당 단일 leaf 예외와 일치한다. 초안은 해당 행에만 `NoSameCaseSaveObservation` 진단을 추가하고 `barrier.ordinaryWriterBlocked=Unknown`을 선언하도록 제한하며, AC006의 실제 Save 시험에는 면제를 적용하지 않는다. 별도로 불일치한 5개 행(C4 checkpoint 14/15/16/26, AC007 C2 기본 결과 거절)도 완료 후 `NoBarrier`와 writer Unknown으로 정렬하는 범위가 맞다.

AC006 실제 writer 행은 callback 시점의 marker 존재, public Save 차단 예외, 원본 marker/profile bytes 무변경을 직접 관측했다. 작업 완료 후 별도 관측에서는 marker가 없어 `barrier.state=NoBarrier`다. 최종 상태만 NoBarrier로 정렬하되 `ordinaryWriterBlocked=Yes`, callback 근거와 `ActualOrdinaryWriterObservation`을 유지한다는 초안 조건이 이 두 시점을 올바르게 분리한다.

CP17 미반환 행의 기존 `ProfileResetDiskTransactionV1.cs:693` 포인터는 C4 결과 발급 전 경계를 가리키지 않는다. Bridge의 `BeforeFreshHandbackPublish` 호출 위치 `ProfileResetExecutionBridgeV1.cs:302`로 바꾸는 것은 적절하다. 이 포인터만으로 미발급을 추론하지 말고 기존 실제 callback 예외 동일성, Result/ResultRecord 부재, CWT·fault 원본 검증을 그대로 요구해야 한다.

P1: 메모리 재평가에서 위 네 집단 이외의 숨은 한 행이 확인됐다. `edit_AC001_UncommittedActualRequestCannotStart`는 `Create(false)`로 시작해 최초 `_initialCommitHistory`가 null이다. 테스트는 미커밋 상태의 거절을 확인한 뒤 `CommitForExecution`으로 같은 요청을 정상 commit하고 실행한다. 이후 `ObserveActualHistory`가 생성 시점의 null 기준과 비교하므로 `history.originalCommitPreserved=No`가 나온다. 이는 원본 commit 손실을 입증하지 않는다. 그러나 정상 실행 RequiredFact의 현재 기대 `Yes`와는 일치하지 않아 초안의 네 수정만 적용해도 행 전체 비교는 계속 실패한다.

이 행은 기대를 `No`로 바꾸면 안 된다. `originalCommitPreserved`는 실제 실행을 승인한 commit 참조의 보존을 뜻해야 한다. `CommitForExecution` 직후이자 C4 실행 전의 실제 `CommitHistory`를 관측 기준으로 잡고 최종 동일성을 검사하거나, 미커밋 거절과 정상 후속 실행을 별도 사실로 분리해야 한다. 두 방법 모두 현재 초안의 “예상 원장과 미반환 helper만 변경” 범위를 넘으므로 parent 시험/fixture 근거, selection의 `SourceFiles`, rows, freeze manifest, input 목록 및 계획의 새 지문을 계약에 추가해야 한다. 이를 추가하기 전에는 R6 재실행 준비가 끝난 것으로 보지 않는다.

P0는 없다. 확인된 P1은 한 행의 관측 기준·수정 권한 누락이다. 그 행의 기준을 고친 뒤에도 숨은 비교 차이가 없는지 새 출력에서 다시 확인해야 하며, R5 결과를 소급 변경하지 않는다. C3 전체나 C4 수용 결론을 내리지 않는다.
