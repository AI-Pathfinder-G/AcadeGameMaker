# C3 R5 정상 확인 콜백·게이트 경계의 솔 독립 구조 증거

- 날짜: 2026-09-29; 검토자·실제 모델: 솔, `gpt-6-sol`
- 추적: `REQ-M5D7QC3-001/003/005/006/007`, `AC-M5D7QC3-001/003/005/006/009/010`.
  실행 연결 전 AC007/008 부분 범위이며 C3 전체·C4 수용 판정이 아니다.
- 근거: [제한 보정 승인](../approvals/2026-09-29-c3-intake-cancel-and-gate-correction-approval.md).
- 코드/시험 수정·Unity 실행·재컴파일 없음. 최종 R5 소스의 독립 읽기 검수다.
- 통합 원장 `artifacts/c3-upper-r5-frozen-source-manifest.json`의 실제 SHA-256은
  `B9DC34D615ECE99775C927FB434D88BB53F0B2F9E356BF8FECB2EB2D4483C9EE`다.
  원장에 명시된 14개 실제 파일 지문을 각각 재계산하여 모두 일치함을 확인했다.

## 동결 자료와 역사 구분

아래 경로의 접두사는 `Assets/AcadeGameMaker/Runtime/`이다. 기존 Confirm
보고서 SHA `AFBDC066B55B36A16590A793EE30FC7EE75142A85B0570042DC46428EB3D89D5`는
그 당시 owner `4723...`의 역사로 그대로 보존했다. 이 문서는 새 owner와 새
현재 결속 검사의 호출 범위를 다시 대조한 R5 자료다.

| 경로 | 실제 재조회 SHA-256 |
| --- | --- |
| HubPresentation/Unity/NewGameConfirmationOwnerV1.cs | `CAEBD7B5DA99AF94A25C5C300789AA48CA87908CA6D8894B99AE49C2391D7043` |
| HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs | `F47C61A043DAF4695CC53E41C6BE9BB67CA147C722F7B830459AA6304B45B3B5` |
| HubPresentation/Unity/HubMenuPresenterV1.cs | `393B325BA5B8F6D79CA983F1A91098215E7A6993DE37E9F5D98BA775760E4900` |
| Input/Unity/HubEntryHandoffLatchV1.cs | `1201240D6E874E596E00D36EFD37330D3C37AEF0591EDA38BAF30D635E4C267C` |
| Input/Unity/ProfileNewGameConfirmationV1.cs | `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` |
| Input/Unity/DesktopProfileLaunchAdapterV1.cs | `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB` |
| Input/Unity/InputRouter.cs | `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB` |
| Profile/ProfileResetDiskTransactionV1.cs | `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2` |
| Profile/ProfileNewGameResetServiceV1.cs | `23D7248B1A603A89BC8FF0FFDBB24FA04E640A66018DDC5689F97EC80701FBE5` |
| Profile/ProfileCanonicalDecoderV1.cs | `F361E860E09217B1DD32CEDD48980FE0E6F22F993800C20B6EF7AA13029EBE42` |
| Profile/ProfileCanonicalEncoder.cs | `5326FDB385A058A4590B9BD16B07D78A0F0033FF485D4971C8C6FC56A3CABDF5` |
| Profile/ProfileBindingOverridesJson.cs | `91E5EBD41A99ECDCC6B4F1EAE2122E4F23B8F0FD8AF084ADE00A85C5E5D8FD14` |
| Profile/ProfileRecoveryPlannerV1.cs | `E72FE64417385170391E2B7EE6DC7740E448AC1828DC77BFF7A603DB235F56F8` |

새 Edit 지문 `647BEB1256A18EA90A3F73BB3D81B3219A248EBCCE8A7DBB9EA26D176CCB897A`와
Play 지문 `2D3403E910E0DBA349854A194F7140DE65759190DF53920F2A989C6D0D07127C`도
14경로 원장에 일치했다. 작업자 `artifacts/c3-upper-r5-terra-implementation-evidence.md`,
`c3-upper-local-compile-r5/commands.json`의 runtime/input-editmode/input-playmode
각 Exit=0 및 `input-editmode-final-command.json`의 마지막 Edit 지문/Exit=0을
읽었다. 기존 Bee 참조를 사용한 작업자 컴파일 기록이며 이 검토자가 실행한
독립 컴파일이나 Unity 실제 시험 결과로 재사용하지 않는다.

## 일곱 진입점의 실제 종료 처리 배치

줄 번호는 owner 기준이다. 각 앞선 인증은 아직 예약이 아니며 게이트를
획득한 후 같은 실제 참조/발급 증거를 다시 확인한다.

| 진입점 | 앞선 인증→Enter | Terminal catch 밖의 실제 재확인 | 안쪽 미소모 권한 검증/작업→종료 처리 | 바깥 finally |
| --- | --- | --- | --- | --- |
| AcceptNewGame | 148→149 | 152: 정상 종료 상태 또는 동일 명시 handle의 Q-B 분류가 미소모=1이 아니면 거부 | 155 Validate, 156 실제 pending/state/token/epoch, 157 기존 Q-B consume; 162 Terminal | 164 Exit |
| RetryIntake | 168→169 | 173: 동일 CWT witness 참조, Closed=false | 176 Validate, 177 current retry/epoch/token/accepted issued/기대 상태, 178 CAS; 184 Terminal | 186 inspecting을 예약한 호출만 0 복구 후 Exit |
| Confirm | 224→224 | 227: AuthenticateDecision 재조회와 같은 witness; 이미 Used인 실제 결정은 여기서 거부 | 230 Validate, 231 ValidateLiveDecision, 232 CAS; 246 Terminal | 248 Exit |
| Cancel | 271→271 | 274: 같은 실제 decision witness 재인증/Used 거부 | 277 Validate, 278 ValidateLiveDecision, 279 CAS; 286 Terminal | 288 Exit |
| OpenFreshDecision | 252–253→254 | 257: 앞서 잡은 동일 fresh·generation·epoch, 종료 상태/교체 여부 | 260 Validate, 261 현재 실제 fresh 및 FreshDecisionRequired, 262–263 새 결정 발급; 265 Terminal | 267 Exit |
| Rearm | 296→297 | 300: 동일 actual CWT witness, Used=false, Consumed=0 | 303 Validate, 304 current rearm/token/epoch/Cancelled, 305 CAS, 308–316 기존 reserve→prepare→commit→history→새 epoch; 318 Terminal | 320 Exit |
| CommitForExecution | 324→325 | 328: 동일 actual confirmed 참조 및 정상 종료/교체 거부 | 331 Validate, 332 현재 준비 상태, 333 lower reserve→335–337 상위 폐쇄→339 same request/reservation complete; 343 lower close와 344 Terminal | 347 Exit |

`Enter :359`의 CAS 실패는 각 바깥 try/finally보다 앞에 있다. 따라서
획득 실패 호출이 정상 승자의 게이트를 해제하지 않는다. 획득 성공 뒤
외부 재확인 거부·내부 성공·내부 예외 모두 바깥 `Exit :361`로 해제한다.
재확인 뒤에 둔 안쪽 try가 Terminal catch의 범위를 정하므로 완료된 권한의
거부가 정상 승자의 새 결과나 후속 epoch를 닫는 이전 정적 후보는 이
구조에서 차단된다. Retry Busy는 같은 retry를 유지하고 inspecting만 복귀하며,
Confirm Busy `:235–236`은 같은 실제 decision의 State를 0으로 되돌린다.
완료 이력이 없는 동일 권한의 승인된 재시도는 보존한다.

Q-B `ClassifyNewGameIssuance :238–243`는 명시 actual handle의 기존 CWT에서
issuer/owner/presenter/router/record/handle 참조를 확인하고 실제 `_epochs`
append 사슬에서 같은 record를 찾는다. 실제 witness.Consumed를 읽어
외부/미등록=0, 미소모=1, 소비 완료=2만 반환한다. token/epoch/current 슬롯과
proof 검증을 이 외부 분류에서 대신하지 않으므로 owner `:155–156`와 기존
Q-B `Consume :224–226`이 안쪽에서 유지한다. 새 레지스트리/권한 반환이나
자동 pending 선택·등록·소비·이력 변경은 이 보조 몸체에 없다.

실제 미소모 handle에서 단일 owner `_stateProof` 또는 `_epochProof` 손상은
앞선 Classify 결과를 바꾸지 않는다. `IsNormallyEnded :360`도 정상 종료
상태의 state/proof 일치일 때만 참이다. actual pending의 proof 손상은 안쪽
`Validate :356`로 도달해 `:162` 전체 폐쇄 경로다. Q-B `_takenRequestProof`
손상은 actual 분류를 통과하고 안쪽 `ValidateC3State`로 도달한다.
adapter `_launchRootWitnessB` 손상은 actual consume 뒤 Capture의 root 인증에서
실패하여 같은 안쪽 종료 처리로 간다. 이는 정확 네 단일 음성 손상 행의
구조 대조이며 실행 성공 보고나 여러 private 필드 동시 공격 검증이 아니다.

`Validate :356`은 모든 상태에 원본 Cohort/역할·GameObject·state/epoch/token
proof를 유지한다. `:357`은 enum 순서상 AwaitingRequest부터 Rearming까지
살아 있는 여덟 상태에만 현재 presenter 결속을 요구한다. 정상 폐쇄 뒤의
ExecutionCommitted/TerminalFailure/Closed에는 현재 살아 있는 결속만
요구하지 않아 `State :124` 같은 정상 후기 getter의 기존 검증을 보존한다.
손상 proof를 그대로 둔 후기 getter는 여전히 실패할 수 있으며, 이를
검증 우회나 proof 복구를 통한 재활성화 허가로 해석하지 않는다.

## 정상 Confirm의 동일 스레드 외부 콜백 도달 범위

owner `:224/227`의 실제 Decisions CWT 조회와 `:359` CAS 이후,
`:230` Validate→presenter `:225–228`의 현재 결속 검사는 역할 참조만
읽는다. latch `:31–32`는 실제 adapter/router 필드를 반환하고 owner `:132`는
adapter 참조 비교만 한다. UI 렌더/UnityEvent/사용자 delegate·준비 포트를
호출하지 않는다. `ValidateLiveDecision :221`도 참조·값 검사다.

decision 예약 `:232`→`CheckFault :233`은 `:386`의 문자열 일치 예외만
던진다. callback 포트가 아니다. `:234` lower `Capture :147–162`→adapter
`GetAuthenticatedLaunchRootForNewGame :223–235`은 실제 원본 Cohort·receipt/
router·root 증거와 경로 정규화만 대조한다. launch 환경의
GetPersistentDataPath/GetUtcNow 또는 preparation.Prepare를 다시 호출하지 않는다.

이어 C1 `CaptureConfirmationObservation :630–657`→`:633` 실제 관찰 lease
→`:638` barrier 검사→`:645` 세 leaf의 FileStream Read `:901` 및 decoder
`Decode :110`→`:655` 원본 capture CWT 등록 `:479`다. ResetService의
`AcquireForObservation :75`는 동기 파일 잠금과 `Thread.Sleep :90`이며
입력/프레임 message pump나 사용자 callback을 호출하지 않는다. decoder
`DecodeCore :123`는 UTF-8/JSON/integrity/정규 바이트와 제품 closed snapshot,
binding parser/encoder를 검사한다. lower `Classify :426`의 기본값 비교는
`PlanDefaultBootstrap :450`이며 자체 실제 값 비교다. 등록은 비공개 원본
witness/CWT 생성이고 외부 delegate를 실행하지 않는다.

Busy 외 결과에서는 owner `:237`이 기존 decision Used=true/State=2로
닫고 슬롯을 비운 **뒤** `:239` lower `ResolveConfirmedRecapture :194`로
간다. GetCapturedWitness `:323`, GetAuthenticatedRoot `:408`, 실제 identity
비교와 Mint `:283`의 lifecycle 잠금/CAS/CWT 등록 및 결과 등록 `:349–352`은
외부 UI 콜백을 호출하지 않는다. 이 정상 경로에서 같은 스레드의 외부
콜백으로 살아 있는 Confirm 예약에 다시 들어갈 경계를 찾지 못했다.

디스크 파일에는 별도 durable Begin/Resume checkpoint 포트가 있다.
파일 전체에 delegate가 없다고 주장하지 않으며 이 관찰 사슬은 해당
실행 overload를 호출하지 않는다. 오류 Terminal `:374`는 권한을 무효화한
뒤 `:377`에서 peer UI를 닫는다. 이후 엔진 UI 알림이 가능해도 정상 live
decision 예약 중 콜백으로 집계하지 않는다. Cancel의 실제 효과 경로와
메모리 부재의 합성 한계는 갱신한 별도 취소 호출 보고서에 기록했다.

## 한계와 남은 실제 판정

이 제한 구조 대조에서 승인된 재인증/종료/finally 및 정상 Confirm 호출
사슬에 추가 수용 차단 구조 결함을 찾지 못했다. 독립 Luna 전체 검수와
아스트라 통합 판정을 대신하지 않는다. 인증 직후 호출을 지연시켜 경쟁
승자가 Exit한 뒤 늦은 호출이 진입하는 **정확 일정은 실행하지 않았다**.
새 대기 callback·반사 gate monitor·제품 seam을 만들지 않았다. 해당 창의
완료 거부 배치는 구조 증거이며 실행 재현 행 또는 통과 개수로 세지 않는다.

실제 Graphic 재무장 callback 중첩, 정상 Confirm/Cancel 동시·중복·늦은
호출과 Busy 재시도, 실제 intake 음성/취소 snapshot 집중 시험 및 필수
회귀는 별도 실제 실행 자료가 필요하다. 컴파일 Exit=0이나 14경로 지문
일치는 그 실행을 대신하지 않는다. 최종 통합 전에 원장과 모든 해당
자료 지문을 다시 확인하며 변경되면 이 줄/호출 검수를 다시 수행한다.
