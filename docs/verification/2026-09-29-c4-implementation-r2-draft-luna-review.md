# C4 구현 계약 r2 초안 독립 검토

검토 대상 `docs/proposals/2026-09-29-c4-implementation-contract-ready-r2-draft.md` 확인 SHA-256: `A549466BBC08B69DFE576AFB391CAAA1473355982771E9C0613BBE940A83EA7A`. 대조 기준은 Review 상태 C4 계약 SHA `3F29B8BC7D3F0172717C0FC94101BF4086FEE1FE475B65A3B2E67E0AE4FA5DC0`, 원본 C4 구현 초안 SHA `8339895BC58985997AD55A9577045F99D2EF3C74B1821CAA3E450EFCA6C9E9BE`, A/B/C 경계 후보와 이번 thread/reentry 후보다.

**판정: P0 0건, P1 3건 유지.** R2는 기술 선택을 더 구체화했으나 Draft이며, 승인·구현·실행 권한은 없다. 특히 관리 전용 containment가 기존 허용 Adapter/Router 안에서 모든 writer/capture를 실제로 차단할 수 있는지는 미해결이라 초안 자체도 구현 Ready로 볼 수 없다. C3 전체 선행 수용 미완료, AC007/008 Open, 필수 Play610 결과 대기도 그대로다.

A의 단일 정지점은 실제 C1 DiskPrepared 전체 검증과 양측 guard 뒤, C2 호출 직전으로 한정된다. 독립 worker의 실제 임시 root lease로 실제 C2 Acquire Busy를 얻거나, 실제 `profile.operation.lock` 파일을 삭제한 뒤 같은 경로를 디렉터리로 만들어 C2의 원래 ManualRepair 분기를 관측한다. 합성 typed outcome이나 일반 27개 throw-only checkpoint 확장으로 바꾸지 않고, worker 준비 실패/timeout이면 C2를 호출하지 않으며 원래 스레드 재개·finally 해제와 여유가 있는 bounded wait를 요구한다. 현재 C2의 실제 Acquire Busy는 이 시점에 이미 C1 DiskPrepared이므로 terminal이며 C1 Busy/Stale handback과 다르다는 구분도 맞다. 이 정지와 파일시스템 조건은 기존 C4 계약의 throw-only 범위 밖이므로 정확한 별도 Approved amendment, 고정 timeout/도달원장 및 실제 실행이 필요하다(P1 A).

B는 readonly struct인 C1 결과를 by-value 전달하거나 boxed copy를 수정해도 executor의 저장 local이 손상되는 것이 아님을 명확히 한다. 순수 validator의 boxed-copy default/unknown 거절과 모든 proof/C2 분기를 그 validator가 지배한다는 source 분석은 가능한 보완 증거지만, 실제 executor local 손상 실행은 아니다. 이 조합이 AC003을 만족하는지 규범 판단이 남는다. Nested proof의 실제 `_root=null` 및 실제 C2 결과의 `_outcome=default`는 각각 동일 발급 객체의 단일 음성 손상으로 getter/composition 거절을 확인하는 후보다. 이는 실제 결과 생성 뒤 행이며, C2 반환 뒤 observer는 C2 호출 전 거절을 증명하지 않는다. 필드·형식·시점에 대한 별도 승인과 실제 full bridge 시험이 필요하다. 따라서 P1 B는 닫히지 않는다.

C는 실제 held Submit이 C2의 기존 old UI Disable에서 canceled callback을 일으킬 때, fixture listener가 이미 소비된 동일 opaque request와 pair로 한 번 재호출하여 같은 스레드 clean loser를 확인한다. InputSystem 1.20.0의 Disable→ResetActionState→Canceled→listener 경로 및 Router의 old UI Disable 후 자체 구독 해제 순서는 후보를 뒷받침하지만 실제 held 상태/이벤트는 시험 전까지 미관측이다. 기존 Play 흐름은 Enter 뒤 Presenter 처리 및 후속 Publish를 하므로 held 상태를 보장하지 않으며, 신규 fixture에서 정상 input/owner/C3 경로와 실제 event 관측을 별도로 입증해야 한다. 이 후보는 C2 terminal 구간의 consumed loser만 시험하고 callback 없는 구간 또는 pre-auth worker 경쟁 전부를 닫지 않는다.

계획된 birth thread witness는 현 C3 issued witness/event에 없고 새 private 상태 및 발급 이력 amendment를 요구한다. 미소비 최초 호출이 wrong-thread이면 Unity 조회·소비·guard·C1/C2 전 clean rejection, 이후 발급 스레드의 동일 request 성공 짝이 필요하다. 실제 손상에서 worker가 Unity API를 호출하지 않는 조건도 독립적으로 입증해야 한다. 기존 fail-stop은 안전한 대체가 아니다. 실제 Adapter `LatchResetCutoverFailure`는 Router로 전달하고, Router `LatchResetCutoverFailure`는 owned action의 Disable/RemoveCallbacks 등 Unity/InputSystem 작업을 수행한다. 따라서 이를 worker에서 호출하면 안 된다는 제한은 정확하다.

초안이 새 managed-only pair terminal latch 및 writer/capture admission 선행 확인을 미결정으로 남긴 것은 적절하다. 다만 이를 단순화해 “bridge writer만” 차단 범위로 축소할 수 없고 C1/Profile 금지도 넓힐 수 없다. 허용 Adapter/Router 변경만으로 모든 관련 입력·저장 writer를 즉시 막는 원본 pair 결속·가시성·호출 경로가 증명되지 않으면 이 상태는 구현 차단으로 유지해야 한다. Worker cleanup을 위해 전역 dispatcher를 새로 넣는 것도 후보 경계를 넘는다(P1 C의 containment 부분).

경로·역사 경계는 유지된다. 원래 8 runtime 경로, 새 bridge/partial과 C3 owner, Q-A/Q-B 별도 amendment 조건, 테스트 3개·fixture 2개와 신규 meta 후보가 보존된다. Q0의 Adapter/Router exact current successor, Q-B strict 금지 본문, Q-A transfer audit는 각각 별도 감사 권한으로 남는다. 14 R11 pin과 과거 240/15/562/51/610 정식 이름은 유지하지만, 이 보존은 새 C4 코드의 QA 대상을 포함하지 않는다.

현재 C3 큐는 고정된 R11 manifest/선택 및 884개 입력 집합과 같은 원장·검증기를 대상으로 한다. C4 bridge·C3 witness 변경·새 테스트/fixture/meta를 넣으면 소스 집합과 hash가 달라지므로 그 큐, 884 hardcode, R11 frozen manifest를 새 C4 결과 검증에 재사용할 권한이 없다. 기존 선택 이름·순서는 역사로 보존하되, 구현 승인 후 최종 C4 source/meta의 새 manifest, exact focused 및 필수 회귀 선택, 예상 이름/count, 실제 source capture/QA protocol과 결과 대조를 별도로 생성·독립 검수해야 한다. 추가 named 행을 초안 표에 더한 것으로 NUnit 실제 이름/count가 고정된 것은 아니다.

초안의 핵심 잔여는 AC003 순수 validator+생산 지배 분석의 규범 충분성, 단일 참조 음성 observer 시점/권한, actual held canceled 행, 신규 birth witness, 무엇보다 managed-only containment가 기존 writer 경계 전부를 차단하는지다. 이들이 검증되기 전 A/B/C 및 C4 허용을 승인할 수 없다. 소스·QA·기존 원장·Git·네트워크·Unity·컴파일을 변경하거나 실행하지 않았다.
