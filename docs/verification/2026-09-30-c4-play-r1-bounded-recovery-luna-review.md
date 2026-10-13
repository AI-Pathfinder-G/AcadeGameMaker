# C4 Play R1 제한 복구 설계 독립 검토

- 검토 대상 제안: `docs/proposals/2026-09-30-c4-play-r1-bounded-recovery-design.md`, SHA-256 `BD909D40F2F831DA494C9B3E456C374BC36E4EFB541EA85DEE1F106583853351`.
- 규범 기준: 승인된 C4 R4 `docs/specs/work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md`, SHA-256 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`.
- 검토 시점의 원본 런타임: 실행 브리지 `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`, 소유자 `8248404F3F237936954979F3FEC2848E9F419CF0AA2A781F732D5CA379FF9809`, Router `5A210D7ED80DE86C9D3092865E7F4712C00C65B84372E479018BF43956F6695F`, Adapter `4B157B98511A0978D19B25434F6D00E23C0CC6FB53BA8A14969182865C3BDC0E`.
- 시험 입력: Play fixture `30330B3276F61F9686D9743CD7BFCA20B5CE6A5FFDC2E0994F8847949FAB1A0E`, Play 부모 `1CDA66FB944D17489D6003AED24FAB665E4979C74ABAD242D81C7914133DDAA6`.

## 판정

설계 자체의 P0 0건, P1 0건이다. 세 보정은 실패한 실제 증거를 삭제하거나 기대를 낮추지 않고 각각 시험 포인터, 시험 초기 상태, lifecycle 실패 시 원래 C4 쌍의 폐쇄를 겨냥한다. 이는 설계 검토일 뿐이며 구현 안전성·Unity 재실행·C4 수용을 뜻하지 않는다.

## 근거와 경계

원시 Play XML `artifacts/c4-focused-play.xml`(SHA-256 `92B143BEC6A26D89169CB07A5E1D8F9396362A3E647ED38EF532011CC92A0D7E`)은 32건 중 19 통과, 13 실패다. 11건은 모두 `FrozenNoSameCaseSavePointers`의 `Expected -1 / Actual 843`에서 멈춘다. fixture에는 844행의 세 인자 `ObserveActualBarrier` 전달 오버로드와 846행의 네 인자 실제 관찰 오버로드가 있으나, 903행 검사는 메서드명 부분 문자열만으로 선언을 찾으므로 두 선언을 모두 발견해 실패한다. 제안된 정확한 네 인자 서명 결속은 원래 포인터·해시 검사를 유지하면서 모호성만 제거한다. 이 11건은 장벽·가드 관찰 결과가 검증된 통과로 바뀌지 않는다.

반복 취소 실패는 실제 `Confirmed`와 기대 `DecisionRequired`의 불일치다. 현재 시험은 `Create()`의 기본값 `threeLeaves=false`를 사용한다. fixture는 기본 `profile.json`만 만들고, 이 기본 snapshot은 분류상 `NoConfirmationRequired`가 될 수 있어 Owner가 `Confirmed`를 직접 반환한다. `Create(true,true)`는 커밋을 보존하면서 실제 Primary/Previous/Temp를 구성해 분류가 확인을 요구하도록 하는 시험 전제 보정이다. 이 변경은 제품 분류를 바꾸지 않으며, 실제 신규 요청·취소·재무장 반복을 그대로 검사해야 한다. 세 leaf 구성 후에도 예상 결정 결과가 나오지 않으면 기대를 완화하지 말고 그 실행을 별도 분석해야 한다.

fresh 비활성화 실패는 별개 런타임 경계다. 테스트는 실제 C1 Busy 결과를 확보하고 Router를 비활성화한 뒤 fresh handback 수락을 거절시키지만 Adapter의 terminal 상태가 참이라고 보장하지 못했다. 현재 `RecordExecutionLifecycleFault`는 실제 GuardedAdapter/GuardedRouter 원장과 원래 스레드를 확인해 오류 이력만 기록한다. 제안처럼 그 기록을 원래 `ExecutionRecord`에 남긴 뒤 잠금 밖에서 `!NativeClosed`일 때 기존 `CloseOriginalExecution(e)`를 호출하는 방식은 원래 Adapter/Router 참조와 기존 guard를 재사용하며 새 권한·새 세션·C1/C2 호출을 만들지 않는다. 원래 스레드 확인을 닫기보다 먼저 유지하고, 기록 후 잠금 밖에서 처리하는 순서도 재진입 중복 처분을 제한한다.

정상 C2 Completed 행은 이미 `NativeClosed`이므로 새 lifecycle 기록이 그 완료 쌍을 다시 처분하지 않아야 한다. 반면 실제 Busy/Stale handback 또는 양측 guard가 부분 진입한 상태는 `NativeClosed`가 아니므로 lifecycle fault 후 양측을 닫아야 한다. `CloseOriginalExecution`은 종료 플래그를 적용 전에 설정하고 Adapter와 Router 각각을 시도하며 실패 시 해당 half의 기존 latch를 호출한다. 이 재진입 경로가 무한 반복하지 않고 양측을 terminal로 만들며, 원래 결과의 `Validate()`와 예외가 보존되는지는 정적 설계만으로 입증되지 않는다. 구현 후 실제 Play 행에서 두 half의 폐쇄, C1/C2 계수 불변, Busy 결과 이력 유지, fresh 재사용 거절, cleanup과 정상 C2 Completed 보존을 확인해야 한다.

따라서 구현 허용 시 변경은 제안에 한정된 fixture 포인터 선택, 해당 취소 부모의 `Create(true,true)`, lifecycle 기록 뒤 원래 실행쌍 폐쇄여야 한다. 승인된 원장·선택·행·입력 동결을 새 SHA로 다시 결속하고, 기존 13 실패 원본은 보존한 채 13개 사례를 재실행해야 한다. 본 검토에서는 소스·시험을 수정하거나 Unity를 실행하지 않았다.
