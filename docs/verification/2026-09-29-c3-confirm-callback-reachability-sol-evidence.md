# C3 정상 확인 경로의 동일 스레드 콜백 도달 범위

- 날짜: 2026-09-29
- 검토자·실제 모델: 솔, `gpt-6-sol`
- 범위: 승인된 재진입 해석의 읽기 전용 구조 증거. 코드·시험 수정/Unity 실행 없음.
- 추적: `REQ-M5D7QC3-003`, `AC-M5D7QC3-003`의 구조 부분.

## 발견

현재 정상 live Confirm의 decision 예약 이후, decision이 사용됨으로 닫히기 전까지
외부 UI/delegate/UnityEvent/준비 포트 콜백을 호출하는 경로를 찾지 못했다.
실제 사슬은 원본 참조 인증, CAS, 동기 파일 관찰·정규 해석, 비공개 CWT 등록이다.
같은 스레드에서 callback을 통해 live Confirm에 다시 진입하는 경계는 이 사슬에
없다. 이는 실제 재진입 호출을 실행한 시험 사례가 아니며 실행 개수에 넣지 않는다.

## 정확한 소스 지문

아래 경로의 기준은 `Assets/AcadeGameMaker/Runtime/`이다. 모두 실제 조회 시
SHA-256이며, owner는 지정된 unchanged 지문과 일치했다.

| 경로 | SHA-256 |
| --- | --- |
| HubPresentation/Unity/NewGameConfirmationOwnerV1.cs | `4723C8CE75022BE00911D7E5BEBCD4BE349D7902BD3442BDA0642AA1EDE37742` |
| Input/Unity/ProfileNewGameConfirmationV1.cs | `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` |
| Input/Unity/DesktopProfileLaunchAdapterV1.cs | `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB` |
| Input/Unity/InputRouter.cs | `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB` |
| Profile/ProfileResetDiskTransactionV1.cs | `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2` |
| Profile/ProfileNewGameResetServiceV1.cs | `23D7248B1A603A89BC8FF0FFDBB24FA04E640A66018DDC5689F97EC80701FBE5` |
| Profile/ProfileCanonicalDecoderV1.cs | `F361E860E09217B1DD32CEDD48980FE0E6F22F993800C20B6EF7AA13029EBE42` |
| Profile/ProfileCanonicalEncoder.cs | `5326FDB385A058A4590B9BD16B07D78A0F0033FF485D4971C8C6FC56A3CABDF5` |
| Profile/ProfileBindingOverridesJson.cs | `91E5EBD41A99ECDCC6B4F1EAE2122E4F23B8F0FD8AF084ADE00A85C5E5D8FD14` |
| Profile/ProfileRecoveryPlannerV1.cs | `E72FE64417385170391E2B7EE6DC7740E448AC1828DC77BFF7A603DB235F56F8` |

## 호출 사슬과 경계

- Owner `:205`의 AuthenticateDecision은 실제 Decisions CWT·원본 owner/epoch/
  generation/display/issued/state를 대조한다. `:212`의 Enter는 `:320`에서
  `_operation` CAS만 수행한다. `:316` Validate는 Cohorts CWT와 exact presenter/
  Q-B/adapter/router, Unity 객체 소속 및 상태 증거를 읽는다. 이 단계에 렌더/
  delegate/외부 포트 실행은 없다.
- Confirm `:216`에서 decision CAS로 검사권을 예약하고 `:217`에서 CheckFault를
  호출한다. `:346`의 CheckFault는 정해진 문자열 일치 시 예외를 던지는 함수다.
  callable port/delegate가 아니며 테스트 주입 예외를 콜백 재진입으로 계산하지 않는다.
- Confirm `:218`→lower `:147` Capture→`:152` adapter
  GetAuthenticatedLaunchRootForNewGame→`:153` C1 CaptureConfirmationObservation이다.
  adapter `:223`는 `:91/:116/:210`의 비공개 원본 cohort/receipt/router 진단과
  root 두 witness·경로 정규화만 확인한다. launch 당시 환경의
  GetPersistentDataPath/GetUtcNow 또는 preparation.Prepare를 다시 호출하지 않는다.
- C1 `:630`은 `:632` 경로 보호 후 `:633`의 실제 AcquireForObservation lease를
  얻는다. ResetService `:75`는 BCL Directory/FileStream 및 `:90`의 Thread.Sleep을
  사용하는 동기 잠금 대기다. `:118`의 소유 확인과 `:138`의 실제 barrier probe도
  경로·파일 검사다. 입력 장치/프레임/UI message pump나 사용자 포트를 실행하지 않는다.
- C1 `:645`는 실제 세 leaf의 `:901` Read와 public decoder `:110` Decode를
  호출한다. decoder `:123` DecodeCore는 UTF-8/JSON/integrity/정규 바이트와
  closed snapshot을 검증하며 binding parser·encoder를 직접 호출한다. decoded
  배열과 복사된 읽기 collection은 제품 값이며 호출자가 제공한 비교 delegate가 없다.
  C1 `:655`의 원본 capture 등록은 `:479`의 CWT.Add와 자체 불변 witness 생성이다.
- lower Capture는 capture.Validate와 `:426` Classify를 거쳐 실제 결과를 등록한다.
  Classify의 기본값 비교는 실제 PlanDefaultBootstrap과 closed snapshot 비교다.
  Confirm의 Busy는 `:220`에서 같은 capability를 복귀시키지만 callback을 호출하지
  않는다. 그 외에는 `:221`에서 decision을 Used/Consumed로 닫고 `_decision`을
  비운 뒤 `:223`의 ResolveConfirmedRecapture로 진행한다.
- lower `:194` Resolve는 `:323` GetCapturedWitness, `:408` GetAuthenticatedRoot,
  실제 identity 비교와 `:283` 발급을 호출한다. 발급은 lifecycle 잠금/CAS와
  CWT 등록, `:146` resolution 등록이며 외부 callback/렌더 경계가 없다. 이 단계는
  이미 기존 decision을 닫은 뒤다. Owner `:311`의 결과 등록도 CWT.Add만 수행한다.
- Cancel `:248` 인증/Enter→`:252` CAS→`:253` decision 종료→`:255` rearm 등록→
  `:256` 상태 전진/결과 반환에는 렌더나 외부 포트가 없다. 실제 UI 렌더가 가능한
  Rearm의 presenter prepare 경계는 Cancel과 구분하며 별도 실제 시험 대상이다.

디스크 파일에는 durable Begin/Resume용 Checkpoint delegate/시험 포트도 존재한다.
**파일 전체에 delegate가 없다고 주장하지 않는다.** 위 관찰 사슬은 그 overload를
호출하지 않는다. 예외 후 owner Terminal은 `:334`에서 callbacks를 무효화한 뒤
`:337`에서 참여 interaction을 닫는다. 따라서 종료 UI에서 콜백이 발생해도 살아 있는
decision 검사권에 대한 정상 재진입 경계라고 볼 수 없다.

## 증거 한계와 최종 대조

검토 종료 시 위 열 개 파일 지문을 다시 계산했고 표와 모두 일치했다. 최종 통합
판정 시에도 현재 정확한 소스 지문을 재계산해야 한다. 최종 시험 소스/런타임
동결 지문이 달라지면 정확한 호출 사슬·줄
번호를 다시 대조해야 하며 이 보고서를 무조건 재사용하지 않는다. 코드 위치 확인은
실제 실행의 스레드·콜백 발생·CAS 승자를 증명하지 않는다.

실제 Graphic callback Rearm 중첩 시도, actual Confirm/Cancel 경합, late/duplicate/
foreign 거부 시험은 각각 실제 실행 자료로 남겨야 한다. 이 구조 증거가 그 시험을
대체하거나 AC003 전체 통과를 의미하지 않는다. 루나 독립 검수와 아스트라의
최종 현재 소스 대조·판정이 남아 있다.
