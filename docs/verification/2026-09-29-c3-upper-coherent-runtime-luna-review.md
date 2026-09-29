# C3 상위 일관 단위 최종 동결 독립 구현 검수

- 검수자·실제 모델: 루나, `gpt-6-luna`
- 기준 manifest SHA-256: `0849AE5B55FDD448175A3534CB2A6E67F36102257FDFA8FEA27C0D39DAEF1175` (일치)
- 독립 재계산: manifest 14개 항목 중 불일치 0건. 최종 freeze 요약은 13개 항목이며 lower 2개 파일은 별도 고정 항목으로 보존됨.
- 상위 runtime: Q-B `D8DDF69750D6BA4931CEA450EE637A29BF3D04C12EB760A8CA51249CA81A5A6E`, presenter `827A7219F29DB5A9DEEE6A160319E0FB3F965CFBF16412A8BAAFEA37F2AEF74E`, owner `4723C8CE75022BE00911D7E5BEBCD4BE349D7902BD3442BDA0642AA1EDE37742`.
- lower runtime/test: `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` / `F80D57376F3845E16BB3D347CFB3834ACE8FCCB53F539376D880A844A90F9581`.
- 실행 전 원장: EditMode 132개/PlayMode 7개 이름이 각 예상 수와 일치하고 중복 0개. Unity는 실행하지 않음.

## 판정

**P0=0, P1=1.** 대부분의 구현은 승인된 Q-B issued-handle 및 owner 일관 단위 계획과 대조해 실제 Q-A→Q-B 발급 참조, 상위 intake, 최초 이력과 후속 현재 슬롯 분리, lower capture/분류, decision·retry·rearm 상태, commit 순서, 종료 경계를 반영한다. 다만 실제 후속 rearm을 끝낸 뒤 같은 성공 reservation으로 presenter 준비를 다시 호출할 수 있는 재사용 경계 결함이 남아 있어 구현 수용은 보류해야 한다.

## P1 — 완료된 rearm reservation으로 후속 presenter를 다시 초기화 가능

`HubMenuPresenterV1.PrepareNewGameSuccessor`는 `MatchesSuccessorReservation`이 참인 reservation을 받아 새 controller/cursor와 successor slot을 기록한다. Q-B `MatchesSuccessorReservation`은 현재 pending reservation뿐 아니라 이미 `Committed`이고 현 successor slot이 같은 reservation인 경우도 허용한다. 이어 확인하는 owner `IsActualRearm`은 CWT witness의 owner/cohort/token/epoch 일치만 확인하고 rearm witness의 `Used`/`Consumed`나 owner의 현재 state·operation을 제한하지 않는다. 정상 `Rearm`은 성공 후 witness를 Used 처리하고 `_rearm`을 비우며 owner epoch를 전진시키지만, 그때도 위 역사 검증은 여전히 참이다.

따라서 정상 rearm으로 만들어진 reservation/token/epoch를 그대로 보존하고, 실제 successor intent를 transfer한 상태에서 같은 reservation으로 `PrepareNewGameSuccessor`를 재호출하면 새 controller/cursor/slot을 현재 epoch에 덮어쓸 수 있다. 이는 private 필드 변경으로 권한을 제조하는 반례가 아니라 완료된 정상 rearm의 실제 tuple 재사용이다. 승인 설계는 rearm capability를 한 번만 소비하고 partial disagreement를 닫도록 요구한다. 그러나 현재 시험에는 완료 후 authentic tuple 재사용을 거부하는 행이 없다. 해결은 진행 중 rearm 권한과 완료 이력 조회를 구분하고, presenter 준비가 owner의 현재 `Rearming` operation 및 미사용 rearm 증거가 있는 동안에만 허용되도록 제한하는 것이다. 반례 시험은 실제 정상 rearm/transfer가 만든 tuple을 재사용하고, 재호출 거부 또는 요구된 terminal containment와 현재 cursor/controller/intent/epoch 보존을 확인해야 한다.

## 승인 범위·권한 대조

- Approved C3, Q-B 발급 설계 SHA `28926347FAA0E247726177578C3B3A79358993F9ED0FC61C9C5886563C5F5CD3`, 일관 단위 계획 SHA `E68280CD03EA28B3F0CC9B60530FD88762BB27BBE24B966BAD2810D02DF1DC59`, 하위 CAS 보정 및 감사/PlayMode 위치 보정 승인을 읽고 대조했다. Q-B issued handle은 등록된 실제 참조이고 copied row 및 미등록 후보는 intake 권한이 아니다. legacy 최초 take 이력과 후속 current slot/append-only history의 분리 방향은 source에 있다.
- Edit 집중 원장의 lower 73행은 actual source/capture provenance, 외부 문맥 비소모, 복제·단일 필드 복원·세 leaf projection/fingerprint·mint/reserve/complete·경합·partial registration 사례를 포함한다. 새 owner 행은 actual Q-A/Q-B take 및 clean foreign/non-NewGame 거절, history/rearm, decision race, Busy capability 유지, first true cursor frame 폐기, partial successor fault와 commit fault를 다룬다. Play 7행은 임시 루트·실제 preparation·실제 입력/Q-A/Q-B·cancel/rearm history·submit frame 폐기·skip close·dismissed notice·commit을 실제 fixture로 잇는다.
- 상위 정상 호출의 reflection은 정확 assembly-qualified 형식, `DeclaredOnly`, 전체 서명과 by-ref, 반환 형식으로 결속하고 실제 opaque 참조를 전달한다. 양성 발급의 private registry/proof/receipt 제조나 정상 권한 clone은 발견하지 않았다. owner `SetFaultForTests`는 internal이며 허용 문자열 네 경계(`BeforeCapture`, `AfterSuccessorPrepare`, `AfterLowerReserve`, `AfterUpperClosure`)만 설정하고 예외를 일으킨다. 이는 승인 C3의 named deterministic failure control에 해당하며 권한 발급/증거 대체가 아니므로 이 검수에서 scope/API authority 확장 P1로 판정하지 않는다.
- Q-B 후속 감사는 header의 정확한 5개 평범한 using과 공백, 단일 `System.Runtime.CompilerServices` 선언, 기존 금지 배열 및 음성 변형 행을 검사한다. 기존 감사 시험 복원 evidence는 원본 bytes SHA `FC7DD9F902C9783036137CE68423D43A28C00DE187DA125FDB8C41CDCF79B421`가 일치하고 승인된 wrapper만 원문 대조에서 되돌린다. 별도 legacy `TryTakeRequest`는 원본 전체 bytes가 없으며 보존된 구현 전 메서드 발췌와 현 발췌의 줄바꿈/끝 공백 정규화 문자열 동등만 증명한다. 초기 전체 Q-B SHA의 bytes 동등으로 확대하지 않는다.

## AC 추적 및 실행 전 coverage 결손

- `AC-M5D7QC3-001`: 실제 issued handle·foreign/copy/unregistered·non-NewGame·first/successor/history 사례가 선택 목록에 있다. P1은 rearm-successor lifetime의 단일 소비 예외다.
- `AC-M5D7QC3-002/003/004`: Busy/Unreadable, 정확 한국어 copy, Confirm recapture 및 changed/default/ambiguous classification, 실제 Primary/Previous/Temp 투영과 지문 검사 경계를 lower/owner 집중 시험에서 확인했다.
- `AC-M5D7QC3-005/006`: cancel의 프로필·receipt 불변, 반복 epoch history, 새 cursor baseline/첫 실제 true frame 폐기, skipped/partial failure containment 시험이 있다. P1 reservation replay를 완료 후 추가해야 한다.
- `AC-M5D7QC3-007/008`: 계획 및 코드 범위상 부분 근거만 허용된다. commit은 lower reserve 뒤 upper decision/retry/rearm/Q-A/Q-B 폐쇄, owner `ExecutionCommitted`, lower complete 순서다. 성공 Play 행과 `AfterLowerReserve`/`AfterUpperClosure` 오류 행은 있지만 C1 Begin·보고 및 전체 barrier 후 결과는 대상이 아니므로 두 AC는 Open/Not Verified로 남겨야 한다.
- `AC-M5D7QC3-009`: 런타임 권한·API·금지 동작 감사 및 strict Q-B import successor가 선택에 포함된다. 승인 범위를 넘는 C1/C2, scene/map, public/friend/asmdef 변경은 발견하지 않았다.
- `AC-M5D7QC3-010`: 132 Edit/7 Play 목록은 선택 원장일 뿐 실제 Unity runner의 열거/실행 결과가 아니다. standalone C# compile 4개 종료 0은 기록돼 있으나 이전 Unity Library/Bee 참조를 사용하므로 Unity 전체 빌드나 시험 통과를 입증하지 않는다. 기존 562/51/610 회귀, 새 집중 실행, 실제 XML 이름·종료·입력 지문 대조가 남아 있다.

이 검수는 정적 구현 결과이며 Unity 실행 수용이 아니다. P1 수정과 새 정확 지문의 독립 재검수 및 루트 승인 전에는 집중/회귀 실행 수용으로 진행할 수 없다. C3 `AC-M5D7QC3-007/008` 전체는 Open/Not Verified, C4는 Review로 유지한다.

## AC-M5D7QC3-003 시험 coverage 보충 — 초기 manifest 검수의 후속

- 확인 지문은 기존 manifest `0849AE5B55FDD448175A3534CB2A6E67F36102257FDFA8FEA27C0D39DAEF1175`와 동일하다. owner Edit 시험 `116B30344606A42F8BA5F28B777E0EE3C34B4962FF840C41CA3DE48D0EB953C4`, Play 시험 `8E878D8FE2EE8BB4E239A50962032471C9AD94BE6EFEF4DE7EDE0594DEAD25DE`도 초기 지문과 일치한다.
- Approved AC-M5D7QC3-003은 golden 한국어 문구·정확 라벨과 함께 default/forged/copied/late/concurrent/reentrant Confirm/Cancel 시험을 요구한다. 동결된 상위 owner Edit/Play 두 파일 전체를 재검색했다.

**추가 coverage 판정: P1=1, 총 P1=2 (P0=0).** 실제 owner capability를 대상으로 한 golden copy, 취소 뒤 중복·늦은 호출 거절, Confirm/Cancel 경합 1개 행은 있다. 하지만 default/null decision capability, 같은 형식의 미등록 후보, 실제 decision capability 복제본, 다른 실제 owner의 capability, 새 generation을 연 뒤 구 capability의 거절과 fresh capability의 사용을 함께 보이는 음성 행은 찾지 못했다. Play 시험은 fresh generation capability가 이전 것과 다른지 확인하지만 이전 capability로 Confirm/Cancel이 거부되고 새 capability가 정상 작동함을 증명하지 않는다. lower capture/request clone 시험은 상위 decision capability의 인증을 대신하지 않으며, 현 경합 시험은 동일 스레드 재진입 증거가 아니다.

최소 보완은 실제 정상 intake가 발급한 decision capability를 기준으로 위 각 거절 입력을 적용하고, 거절 후 원래 capability가 한 번 성공하는 비소모를 snapshot으로 증명하는 집중 음성 행이다. 두 owner fixture를 사용해 foreign actual capability를 확인하고, changed recapture 후 `OpenFreshDecision`으로 세대를 실제 증가시켜 구 capability 거절 및 새 capability 성공을 확인한다. 복제 행은 진짜 capability 객체의 clone을 거절해야 하며 새 proof/registry 등록이나 private mint/state 조작으로 정상 권한을 제조하면 안 된다. reentrant 검증은 같은 스레드에서 실제 호출 스택이 중첩되는 재진입을 써야 한다. 실제 동기 진입점이 필요하다면 test fixture의 임시 환경 경계가 정상 recapture 중 owner API를 재호출하는 방식으로 증명하고, `SetFaultForTests`를 delegate/callback 설정 API로 확대하거나 단순 동시 작업 호출을 reentrancy라고 부르면 안 된다.

이 추가 P1은 실행 전 승인 coverage 결손이며 새 assertion/실행으로 폐쇄할 수 있다. 현 시험 소스의 정적 재검색 결과는 위 미포함 행을 뒷받침한다. 코드 수정·시험 실행은 하지 않았다.

## AC-M5D7QC3-002/004 실제 분류·재확인 행 coverage 보충

Approved AC-M5D7QC3-002는 `Primary/Previous/Temp` 각 위치의 missing, exact default, 모든 single non-default field, input-recovery-required, invalid, unsupported, unreadable 조합으로 닫힌 분류를 증명하도록 요구한다. 동결된 132 Edit 이름/본문 중 이 조합을 실제 파일로 만드는 분류 행은 확인되지 않았다. C3L 11개는 lease/provenance, root/lock 및 unreadable 접근 경계 증거이지 세 leaf 분류 matrix가 아니다. lower 73개는 capture identity/projection/reflection, 각 leaf projection/hash 손상과 lower issue/reserve/complete 경계가 중심이다. 정상 profile bootstrap 및 Primary 변경 사례만으로 모든 leaf 위치의 분류표를 대체할 수 없다.

Approved AC-M5D7QC3-004는 표시 후 각 leaf의 실제 변경, exact-default 전이, 추가·삭제·ambiguous bytes가 owner Confirm의 fresh generation으로 이어지고 stale identity가 넘어가지 않음을 요구한다. owner Edit의 실제 Confirm case는 equal/default/ambiguous 3종이다. 이 중 default는 Previous 제거, ambiguous는 Temp에 invalid bytes 기록이고, 세 역할 각각의 실제 변경 matrix와 Primary/Previous/Temp의 add/remove 및 default 전이 결과를 상위 owner generation까지 잇는 행은 없다. lower의 leaf별 반사 projection/hash 변조는 실제 디스크 변경 후 owner Confirm을 대신하지 않는다. Play 7행에도 해당 classification/recapture matrix는 없다.

따라서 이 실행 전 coverage 결손은 AC-M5D7QC3-002용 P1 1건과 AC-M5D7QC3-004용 P1 1건이다. 최소 보완은 같은 승인 집중 시험 범위에서 각 leaf 위치별 실제 임시 파일 조합의 분류 결과를 기록하고, 의미 있는 실제 초기 캡처 뒤 각 leaf별 변경·추가·삭제·default 전이를 owner Confirm에 공급해 결과 분류, fresh capability generation, stale capability 거절과 새 capability의 비소모 동작을 확인하는 것이다. 정해진 AC matrix 밖의 비공개 필드 동시 위조를 요구하지 않는다.

### AC003 재진입 시험 가능성의 한계

임시 UI의 TMP label을 실제 `RenderSuccessor` 갱신과 다르게 두고 `Graphic.RegisterDirtyVerticesCallback`에 same-thread 중첩 API 시도를 등록하는 방법은 Unity UI 구현에서 동기 콜백이므로 실제 reentrant 호출 스택을 만들 수 있다. 다만 현 실제 호출 순서에서는 이 콜백이 Cancel 후 owner `Rearm` 실행 중 발생한다. 이때 이전 decision은 이미 소모됐고 owner의 현재 state는 `Rearming`이며 같은 rearm은 한 번 소비된 상태다. 따라서 nested Confirm/Cancel/Rearm은 owner의 `Enter()` operation guard가 아니라 앞선 stale-capability/state guard에서 거부된다. 이는 재진입 시도 자체와 비소모를 입증할 수 있지만 AC003의 live decision을 대상으로 한 진짜 decision-gate reentry 증거는 아니다.

반대로 owner `Confirm`의 실제 lower recapture는 입력된 environment/preparation port를 부르지 않는다. 인증된 launch-root 조회는 저장된 cohort를 동기 검증하고 실제 디스크 관찰은 콜백을 제공하지 않아, 현재 source의 실제 Confirm call stack 안에는 합법적으로 중첩 Confirm/Cancel을 일으킬 application callback 경계가 보이지 않는다. operation gate를 직접 reflection으로 선점하거나 `SetFaultForTests`를 delegate callback으로 바꾸는 것은 적절한 대안이 아니다. 진짜 gate 재진입이 필수라면 승인된 새 deterministic seam이 필요하며 그 전까지 해당 세부 증거는 미확인으로 남겨야 한다. 단순 worker 경합을 reentrancy로 집계하지 않는다.

### 보충 판정 집계

첫 동결 manifest 기준 P0=0, P1=4다. 이는 runtime rearm reservation replay 1건, AC003 상위 decision-auth/reentrant 음성 coverage 1건, AC002 실제 분류 matrix 1건, AC004 owner 실제 leaf-change/fresh-generation matrix 1건이다. AC003/AC002/AC004 보완은 이 manifest source set의 실행 전 coverage 결손이다. 이 기록은 승인된 추가 시험 경로, 소스 변경 또는 Unity 실행을 허용하지 않는다.
