# C3 상위 r3 동결 구현 독립 검수

- 검수자·실제 모델: 루나, `gpt-6-luna`
- Root manifest: `artifacts/c3-upper-r3-frozen-source-manifest.json`, SHA-256 `AFC814D68AFCA1DF9FFF01CB36A0475CE80AE7E7B5C0772F1F12BA769E1371A4` (일치, 14개 파일 재계산 불일치 0)
- 예상 행 원장: `artifacts/c3-required-decision-expected-rows.json`, SHA-256 `72054463BEF19AE4C82534081843FCD3F3B6F8601BF1781A8A1F3E626C76115D` (일치; 173 분류 + 204 전이 = 377개, ID 중복 0)
- 핵심 런타임 SHA-256: Q-B `9B9AAFA1A0898E27FAADC6CAC389838E170D1527A177893060DF5397723C34E7`, presenter `393B325BA5B8F6D79CA983F1A91098215E7A6993DE37E9F5D98BA775760E4900`, owner `4723C8CE75022BE00911D7E5BEBCD4BE349D7902BD3442BDA0642AA1EDE37742`, lower `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1`.
- 집중 선택: Edit 142개/중복 0, Play 12개/중복 0. 컴파일 기록은 네 standalone 명령 exit 0이나 기존 Bee 응답 파일/참조 기반이다. Unity 전체 신규 컴파일·시험은 미실행.
- 범위: 정확 지문 독립 정적 검수. 소스·시험·Unity 수정/실행 없음.

## 판정

**P0=0, P1=3.** 이전 네 설계/구현 P1 중 reservation replay는 현재 소스에서 폐쇄됐고, 새 4개 Edit 및 4개 Play phase 행이 완료된 실제 reservation 재전달 거부와 상태 보존을 확인한다. 하지만 상위 승인 AC001의 intake 거절 행, AC005 취소 보존 snapshot, 실제 runner에서 377행 증거를 수집하는 출력 경로가 여전히 완결되지 않아 Unity 집중 실행 수용은 보류다.

## 구현 및 시험 대조

- Q-B `HasLiveSuccessorPermission`은 현재 `_pendingSuccessor` 참조, `!Committed`, 이전 현재 epoch/proof, `owner.IsRearmInProgress(...)`를 모두 검사한다. presenter 준비와 commit은 이 live predicate를 사용한다. `MatchesSuccessorReservation`은 committed/current successor에 대한 history 일치 판정으로 남아 있지만 presenter 준비 권한으로 단독 사용되지 않는다. owner `IsRearmInProgress`는 Rearming 상태/proof, operation=1, 현재 `_rearm`, CWT witness의 미사용 및 예약 소비 상태를 확인한다. 따라서 실제 정상 rearm tuple 재전달이 live 준비로 승인되지 않는다.
- AC006 late-reservation Edit 시험은 AwaitingBaseline/Ready/Transferred/AwaitingDecision 네 단계에서 실제 완료 reservation을 다시 준비·commit하려 하며 기존 presenter slot/cursor, Q-B slot, 양쪽 pending issued, owner 상태를 보존한 뒤 정상 후속 취소/재무장을 이어간다. Play에도 네 단계가 있다. reflection은 실제 tuple 읽기에만 쓰이며 권한 조작은 없다.
- 정확한 fixture helper는 표에 든 실제 선언을 `DeclaredOnly`, 정확 매개변수/by-ref/out, 반환 형식, 비제네릭, 비선택형으로 결속한다. Positive path는 opaque Q-B handle과 반환물을 그대로 사용한다. copied-row는 정확한 intake `MethodInfo`를 고정한 후 잘못된 row를 Invoke해 `ArgumentException`을 유지한다.
- AC002/004는 실제 임시 파일 세 leaf를 재설정·재관찰한다. 원장 각 173/204 ID 및 기대값은 코드 전개와 맞고, byte fingerprint·분류·세대·실제 `ConfirmedRequest`에서 readonly `SourceResult`를 찾아 이전 display와 fresh capture 양쪽을 대조한다. exact-default 세 역할은 승인 해석대로 g 유지·fresh capture Confirmed, promptful 전이는 g+1·old 거부·새 권한 성공이다. root manifest의 lower ABD4/F80 및 요구된 이전 이력은 그대로다.
- AC003 음성 행은 Confirm/Cancel 각각 null, unregistered, MemberwiseClone, foreign-owner 권한을 거부하고 실제 권한의 비소모, 새 세대 old 권한 거부 및 새 권한 동작을 확인하도록 구성됐다. Play same-thread 실제 UI dirty callback 행은 Rearm 중첩 시도를 실행하고 callback 진입·thread id·상태/파일 snapshot 및 외부 rearm 완료를 요구한다. 실제 Confirm/Cancel 경합은 기존 별도 Edit test에 남아 있다. 이는 승인된 재진입 경계와 맞고, live Confirm operation-gate 안으로 재진입했다고 과장하지 않는다.
- 제공된 callback reachability 증거 SHA `AFBDC066B55B36A16590A793EE30FC7EE75142A85B0570042DC46428EB3D89D5`와 대조한 열 개 의존 소스 SHA는 전부 일치했다. 보고서는 정상 Confirm 호출 사슬이 callback-free임을 한정해 주장하고 durable Checkpoint delegate 경계는 별도라고 명시한다. 이 구조 증거는 실제 nested UI 시험이나 actual race 시험을 대체하지 않는다.
- AC006 cursor first-true 폐기, skipped/partial successor 종료, monotonic append-only history 및 실제 입력 재무장 증거가 있다. AC009 감사 후속은 기존 forbidden 본문과 제한된 import 예외를 검사한다. AC007/008은 C4 이전 부분 근거뿐이며 전체 수용/실행 연결이 아니라고 유지해야 한다.

## P1 — 필수 보완

1. **AC-M5D7QC3-001 intake mismatch·반사 손상 matrix 부족.** 현재 상위 owner 시험은 실제 발급·한 번 소비, unregistered/copy request, non-NewGame, foreign cohort 및 초기 무이력/선취 후 실패 이력을 확인한다. 하지만 승인된 AC001의 owner/router/root/topology/epoch 각 불일치와 reflection-corrupt intake matrix를 상위 실제 Q-B→owner 경로에서 개별 증명하는 행은 확인되지 않는다. 하위 AC001 root/receipt, lower capture/CWT corruption 시험은 이 상위 intake를 대체하지 않는다. 실제 cohort 둘과 실제 opaque 반환물을 사용해 각 승인된 문맥 mismatch를 독립 음성행으로 추가하고, take 전이면 request/history/display 비소모 또는 take 후면 양쪽 terminal closure를 기대값으로 명시해야 한다. 반사 손상은 요구된 정확한 단일 witness/proof 손상 경계만 다루며 사적 권한 제조나 광범위 동시 필드 위조로 확대하지 않는다.
2. **AC-M5D7QC3-005 취소 snapshot 범위 부족.** 실제 취소/재무장 시험은 intent/taken history, 일부 receipt와 cursor, profile primary 바이트 불변을 확인하지만 세 profile leaf 전체, 메모리/reset snapshot, 실제 InputAction/map 참조·활성 상태, scene/run 상태가 취소 전후 동일하다는 명시 대조를 갖추지 않는다. Edit의 primary/previous 확인과 Play의 실제 입력 수행을 합쳐도 같은 cancel cohort에서 이 승인 snapshot을 입증하지 못한다. 실제 cancel 직전/직후의 세 leaf 존재·bytes/hash, adapter/router receipt, action asset/사용 맵 상태, 실행/scene 관찰값을 같은 cohort에서 기록·대조하고 역사만 새 epoch로 append됨을 분리해 확인해야 한다.
3. **377행 증거 출력 수집성 미입증.** 분류/전이 원장과 reentry 행은 `TestContext.Progress.WriteLine`에 결과 JSON을 쓴다. 현재 설치 Test Framework의 `TestListenerWrapper.TestOutput(TestOutput output)` 본문은 비어 있어 이 메시지가 Unity runner/XML 결과에 보존된다고 보장할 수 없다. 현재는 Unity 실행도 없으므로 planned 377개와 각 실제 passed/failed, 내부 행 before/after fingerprint 및 generation을 AC010에서 대조할 수 없다. 승인된 한 줄 증거 출력 보정을 새 정확 소스 SHA로 검수하고, 실제 runner가 결과를 보존하는지 확인할 때까지 행렬 통과 수를 수용 근거로 취급하지 않는다.

## 실행 전 한계 및 최종 수용 경계

원장 377개는 실행 전 예상 행 수다. 142/12 선택 지문·중복 검사는 예상 선택 원장일 뿐 runner 실제 열거가 아니다. 네 독립 컴파일 exit 0은 기존 Bee 참조를 이용했고 r2 Play compile 실패 및 이후 correction의 이력도 보존돼 있다. Unity 신규 빌드나 focused/required regression 결과로 확대할 수 없다. 현재 Play reentrant row의 구현도 설계대로 callback이 실제 발생해야 하며, 실패·누락·시간초과는 성공이나 skip으로 처리하지 않아야 한다. AC005와 AC001 P1 및 수집 가능한 행렬 출력 보정 후 신규 manifest로 재검수하고 실제 Unity focused 142/12, 377 내부 행 대조, 승인된 562/51/610 회귀 및 exit/XML 전후 지문 검증이 남아 있다. `AC-M5D7QC3-007/008` 전체 Open/Not Verified, C3 전체 수용은 아니다.

## R4 후속 검수 — matrix 행 출력 경로 보정

- Root manifest `artifacts/c3-upper-r4-frozen-source-manifest.json` SHA-256 `C7F42B6DE27BC7916E6DB557B0869D15F32AB553C192F15F1BA240F58F505A47`를 재계산했고 14개 파일 불일치가 없다. r3 대비 변경 경로는 `Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs` 한 개뿐이며 SHA-256은 `753996EB6979EF739AEDA26CFAD31EF57F91280F8E12C10E44B46BC0344E68F8`이다.
- 예상 행 원장 SHA-256 `600CA6CEE3CB5D2C94FF044600E8D2385BEE4BE49AD4EE25D03DBEB1E445EBB8`가 지정 값과 일치한다. 보존된 r3 원장과 비교해 377행 ID 순서 및 행 내용이 동일하다.
- 기존 377행 JSON 기록 writer가 `TestContext.Progress.WriteLine`에서 `TestContext.Out.WriteLine`으로 변경되어 NUnit의 test output 채널로 행 JSON을 내보낸다. planned/passed/failed, 파일 전후 지문, 결과와 generation/authority 참조 정보 필드는 보존됐다. `TestListenerWrapper.TestOutput` 구현이 비어 있는 문제를 피하기 위한 제한 보정으로 scope를 넓히지 않는다.
- `artifacts/c3-upper-local-compile-r4/command.json`의 수정 EditMode standalone compile은 실제 Exit 0이고 입력 시험 지문은 R4와 일치한다. 기존 Bee 의존 참조 사용이며 Unity 실행·XML 저장 여부를 입증하지 않는다.

**R4 출력 보정만의 판정: P0=0, P1=0.** 코드 수준 출력 경로 우려는 닫혔다. NUnit/Unity runner가 377개 실제 row를 결과 XML/획득 가능한 시험 산출물에 온전히 보존하는지는 실행 뒤 실제 XML로 확인해야 하므로 AC010 실행 전 한계는 남는다. 앞 절의 AC001 intake mismatch/반사 손상 및 AC005 취소 snapshot 두 P1은 이 한 줄 수정과 무관하게 미해결이며, 본 후속 검수는 전체 수용으로 변경하지 않는다.
