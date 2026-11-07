# 단독 입력 소유·검사 플랫폼 통합 차이안

상태: Draft. Sol `gpt-6.1-sol`의 상세 설계·정확 변경 제안이며 제품 구현·실행 승인이 아닙니다. Luna 독립 반례와 Astra의 정확 승인 후 아래 범위만 구현합니다. 사용자 중간 조작은 요구하지 않습니다.

## 1. 확인한 권위와 현재 상태

AGENTS.md, docs/README.md, CONTEXT.md, docs/agent-operating-model.md, ADR-0041/0043/0044/0045, Approved 자율 CH1 전달·동일빌드 입력 검증 계약, 원 BVP scope-decision와 결합 draft-contract, 원소유 점프 scope-decision, 11월4일 인계·우선순위를 읽었습니다. 원소유 승인의 2파일·12사례와 미승인 단독안의 4파일·32사례를 구분합니다. 역사 계약·실패·소진 기록은 변경하지 않습니다.

실제 HEAD는 e189fb0125a52ade30626c195e2cf1c794325cce입니다. 추적 변경은 사용자 측 docs/README.md뿐이며 보존합니다. 2026-11-04T09:25:55.9172903Z 기준 Source83 after.json의 666파일·58,900,890바이트 전수 SHA256 대조 불일치0입니다. 이 확인은 새 준비·Unity 성공이나 독립 수용이 아닙니다. 목록 지문은 83B90E87B4DA0B9D68127455984FD2CA86FADEA9317C817B9A80FDC7B00FBC17입니다. 아직 제품쓰기·국소 컴파일·Unity 실행0입니다.

실제 Input 시험 asmdef는 includePlatforms=[Editor], Run 시험 asmdef는 includePlatforms=[]입니다. Run.Unity→Input.Unity 참조와 양쪽의 Run 시험 내부 접근은 존재합니다. Input 시험은 Run.Unity를 참조하지 않습니다. 따라서 단일 플랫폼 32실행을 폐기하고 **EditMode 24 → PlayMode 8 → EditMode 자연 복구2 → EditMode 자연 보스1**의 네 호출로 분리합니다. 폴더명 PlayMode를 선택 플랫폼의 근거로 삼지 않습니다.

## 2. 제품 단일 쓰기와 정확 네 파일

지정 Sol만 활성 제품 Assets/AcadeGameMaker 아래 다음 기존 파일을 씁니다. CRLF와 기존 meta·asmdef·AssemblyInfo·참조·장면·패키지·입력/이동/전투/저장 생산코드를 보존합니다. 나머지662파일은 Source83 바이트와 동일해야 합니다.

1. Runtime/Input/Unity/Chapter1VerificationInputPilot.cs — 내부 단독 소유 종류·권한 인계·장치 사건/상태 관측·유한 토큰.
2. Runtime/Run/Unity/Chapter1SameBuildVerificationDriver.cs — 실제 자기 Pilot만 인계하고 모든 종료의 첫 단계에서 철회.
3. Tests/PlayMode/InputUnity/Chapter1BossVerificationPolicyPlayModeTests.cs — 실제 정책 경계의 기존12와 단독 권한 미발급 거부12, 총24.
4. Tests/PlayMode/RunUnity/Chapter1ExpeditionPlayModeTests.cs — 단독 허가 거절·철회8, 기존 실행·저장 사례 불변.

구현 요구 식별자는 기존 REQ-BAP-EMP/OWN/CARRY/OUTCOME-001과 아래 REQ-SOP-001~004, 수용 식별자는 AC-SOP-001~005입니다. 구현자는 독립 수용하지 않습니다.

## 3. 좁은 소유 종류·권한·관측 변경

**REQ-SOP-001 / AC-SOP-001:** 일반 생성자는 소유 종류 None을 유지합니다. 검사 소유 종류는 기존 실제 Source/case/lease/epoch와 ID/time/device/payload 검증을 그대로 유지합니다. 새 단독 종류는 fixture Source/case/lease를 만들지 않습니다. 실제 sealed Driver 형식과 assembly를 대조하는 내부 인터페이스를 기존 Pilot 파일 안에 둡니다. Driver는 정확 옵션1개, 비Editor·비개발 실행, 증거 CreateNew 성공, 빈 전용 저장 확인을 마친 뒤 실제 `_pilot` 대입 객체에만 실행 세션을 인계합니다. 단순 참값·임의 인터페이스 구현자·미초기화 Driver·다른 Pilot·중복 Attach·세션 재생은 거절합니다. branch1/branch2만 계속 정책을 허용하며 failure-retry는 기존 운전을 유지합니다. 실제 실행환경 허가는 순수 검사에서 양성 발급했다고 주장하지 않습니다.

**REQ-SOP-002 / AC-SOP-002:** 단독 발행 직전 요청 키/버튼/포인터·자기 장치 참조/ID·sequence·세션/초점 세대를 고정합니다. 자연 Dynamic의 자기 StateEvent payload와 실제 After 장치 상태가 같은 요청임을 모두 확인한 뒤 OwnDeviceStateDown을 게시합니다. 실제 관측 eventId/time를 큐 전 고정 fixture 영수증으로 부르지 않습니다. 상태 누락/불일치·중복·sequence 간극/재생·장치 교체/해제·변경 비소유 사건·초점 변화는 권한/토큰을 무효화합니다. 기존 공개 QueueStateEvent와 자연 갱신만 사용합니다. 생산 deviceId=미관측·배타귀속=false·모터 발효/미래 물리안전=미입증을 유지합니다.

**REQ-SOP-003 / AC-SOP-003:** Driver BeginFinish/CloseAndQuit/EmergencyClose/OnDestroy와 Pilot Dispose의 첫 동기 단계에서 세션 봉인을 철회합니다. 이후 정리 예외에도 살아나지 않습니다. After는 이미 결속된 봉인의 활성/세대 값만 읽으며 Driver 콜백·새 시계·게임 getter를 호출하지 않습니다. 기존 fixture After의 검증/예외/소비/pending 순서는 유지하고 부수 관측은 비throw·무할당 값 게시입니다. 철회는 토큰뿐이며 기존 중립 발행·장치/구독 정리는 계속합니다.

**REQ-SOP-004 / AC-SOP-004:** 기존 지상 승인 원방향+Space 의도와 해당 종류의 실제 자기 Space 해제→눌림이 토큰의 선행입니다. run/generation/router/player/modeEpoch/초점세대·장치·세션·발행/소비 sequence·완료tick 연속·동일tick 멱등을 결속합니다. DebtLine 동일 slot/phaseStart, Telegraph age1..46, 원 jumpUntil/패턴 발행tick+2/발급tick+50의 최솟값·만료tick 미포함을 유지합니다. 일반 후보 평가가 먼저이며 알려진 위험·예약접촉·당김·벽·잠금·공격 release/cooldown·조준·중립 재개·시간/정리 guard는 완화하지 않습니다. 모르는 사실을 true로 채우지 않습니다. 생산 HP/위치/보스/시간/진행·저장 상태 조작0입니다.

이는 REQ/AC-BAP-OWN-001의 종류별 Source/소비 증거 대응과 CARRY-001의 종류별 신원 대응, SAMEBUILD-001/002/003/004/006/007의 명시 인계·철회·관측 한계에 대한 좁은 부록 제안입니다. 미래안전/생산 장치 귀속을 토큰 필수 사실로 결정한다면 이 단독 종류도 거절해야 합니다.

## 4. 정확 사례·플랫폼과 실패 폐쇄

**AC-SOP-005:** 새 실행 루트 output/ch1-standalone-owner-platform-20261104/attempts/ 아래 네 슬롯만 사용합니다. 기존 boss-continuation-contract-01와 smoke04/integration03 슬롯은 재사용하지 않습니다. 각 슬롯 존재 시 덮어쓰기/자동 번호증가 없이 중단합니다. XML 선택수=실행수=성공수, 실패·건너뜀·미실행0과 정확 사례 이름 집합을 모두 대조합니다. 0사례나 상위 suite 통과를 성공으로 삼지 않습니다.

| 순서·슬롯 | 실제 testPlatform | 정확 선택과 기대수 | 선행 |
|---|---|---|---|
| owner-common-01 | EditMode | BossOwnedJumpContinuationPolicyTests 기존12와 각각 대응하는 단독 권한 미발급 거부12, 24 | 준비 Luna 수용·새Source·4모듈 계보 |
| owner-driver-01 | PlayMode | RunUnity의 새 Chapter1StandaloneOwnerPolicyTests 아래8, 8 | 직전24 실제 성공·Luna 수용 |
| owner-recovery-01 | EditMode | OwnedInputIsolationRestoresAfterNormalBody / OwnedInputIsolationRestoresAfterBodyFailure, 2 | 직전8 실제 성공·Luna 수용 |
| owner-boss-01 | EditMode | IsolatedApiBossPolicyFromAuthoredFlow, 1 | 복구2 실제 성공·Luna 수용 |

기존12는 원 승인 이름과 fixture 정책 값 검증의 의미를 유지합니다. 추가12는 각 기존 이름 뒤 StandaloneUnowned를 붙인 별도 Test 메서드로 고정합니다. None 또는 거절된 단독 인계에서 같은 신원/시간/명령 반례를 가했을 때 유한계속이 열리지 않고 기존 일반 후보의 의미가 유지됨을 확인합니다. Editor에서 유효 standalone owner/evidence/소비/토큰을 합성하지 않습니다. 따라서 이24는 단독 권한 양성·단독 토큰 수명 양성을 입증하지 않습니다. 유효한 단독 세션의 발급·사건/상태소비·유한기한·무재발급은 빌드 후 실제 원시에 별도 관측하며, 빌드 전에는 해당 양성을 미관측으로 남깁니다. 기존 fixture 값 검증도 실제 fixture 생명주기 수용과 다릅니다.

단독8 이름은 RejectsDefaultConstructorOwner, RejectsUninitializedOrDuplicateOwner, RejectsWrongPilotOrReplayedSession, RejectsUnsupportedOrFailureRetryMode, RejectsEditorDevelopmentOrUnpreparedSave, RejectsMissingOwnedEventOrStateMismatch, RejectsForeignInputOrFocusEpochChange, RevokeOnCloseDisposeOrDeviceReplacement입니다. 각 메서드는 Test 1개이며 수 맞추기용 매개 확장0입니다. 생산 코드가 실제로 사용하는 내부 입장/철회 판정과 미초기화 Driver 거부만 검사합니다. 합성 참값을 권한으로 만드는 helper·가짜 Driver 초기화·Editor 허용 예외·반사·Run.Unity 역참조는 만들지 않습니다. 각 실제 실패는 즉시 후속0·재실행0·원시/원인/관측 목적 보존으로 닫습니다.

기존 원 BVP §7의 Play11/Edit 오르단5+정책6 및 최대3호출과 원소유 부록의 12→2→1 최대3호출을 이 좁은 소유 묶음에서 위 **최대4호출**로 대체 제안합니다. 원 필수 Play11/오르단5/정책6의 회귀 의무는 면제하지 않으며 후속 회귀 권위/예산과 분리합니다. 이전 E2 시간5는 fixture 시계/영수증·After 직접 불변과 Luna 영향 수용에 한해서만 재사용하며 새Source 실행 성공으로 표기하지 않습니다. 영향 미확정이면 준비false·실행0이고 시간5 추가 권한은 없습니다.

## 5. 도구·모듈·새 부모·한정 준비

실행/검사 도구는 기존 output/ch1-natural-input-diagnosis-20261030/tools/run-unity-owned.ps1 및 check-unity.ps1 정확2파일의 원문을 새 준비루트 before에 CreateNew 보존하고, 신규 output/ch1-standalone-owner-platform-20261104/tools/run-unity-owned.ps1 및 check-unity.ps1 두 파생본에만 수정합니다. 원 도구는 불변입니다. 허용 차이는 새 EvidenceId 네 개/플랫폼/선택자·기대수/선행·현재 부모·Source/4파일 허용목록/4모듈 컴파일 계보·기존 HubPresentation 시험 계보와 자원·기한 결속뿐입니다. 기존 프로세스 소유·포착·파이프·폐쇄·실패식·소스 전수검사·격리 수용식은 보존합니다. 기존 gate의 성공 변경0, 새 메타는 신규 루트에서만 생성합니다.

새 부모는 e189fb0이며 역사 eecdb881을 현재 부모로 사용하지 않습니다. Source83은 실제 before 기준, 새 after는 같은666파일·최대60,000,000바이트이며 정확 네 파일 밖 불변입니다. 현재 도구의 Get-R14SourceCheck가 옛 before 해시/3파일 집합을 고정한 사실을 확인했으므로 이 부분의 정확 변경 승인이 필요합니다.

국소 컴파일은 Input.Unity → Run.Unity → Input.Unity.PlayMode.Tests → Run.Unity.PlayMode.Tests 최대4호출, 첫 실패 후 추가/대체 호출0입니다. 응답파일은 실제 Library/Bee/artifacts/1900b0aE.dag의 해당4개가 존재합니다. 입력별 원문·기호·기존 참조·출력 지문을 보존하고 새 Input/Run 산출물을 후속 참조에 연결합니다. 예전2호출 재사용0입니다. Unity의 변경4모듈 새 Csc/Copy/PE와 관련 기존 Hub.Presentation.Unity.PlayMode.Tests의 직접 불변/계보 총5를 대조합니다. 국소 Csc 성공은 실제 Unity 검사 대체0입니다.

승인 뒤 최초 준비 읽기 전에 공식 pwsh로 UTC·단조시각을 새 기록합니다. 이번 읽기전용 조사 시작의 공식 단조시각을 소급 제작하지 않습니다. 새 준비1800초(코드/국소검사1200·기록예약600), 범위 안 메모리 구문/호출 자체보정 최대2묶음, Source after 전수산출1회입니다. 준비 Luna300초/읽기8MiB, 각 단계 메타완성180초(120/60)·Luna300초/8MiB로 한정합니다. 작은 핀은 같은 승인·같은Source·실제 직전 결과만 결속하고 재승인을 반복하지 않습니다.

Unity 각600초(작업560·회수30·폐쇄10), 결과 Luna 각300초/읽기2MiB입니다. 공식 pwsh.exe -NoLogo -NoProfile -File, login=false, require_escalated만 사용합니다. 심사 거절/불명확 시 기본 문맥·API·다른 셸 우회0입니다. Unity 시작 전 단일 쓰기 폐쇄, 정확 Editor 경로·동시성0·실제 부모/출생 소유·빈 슬롯·기한·strict gate를 확인합니다. 로그 침묵만으로 종료를 판단하지 않습니다. 상한 안에 정상 종료/폐쇄가 안 되면 명시 실패이며 후속0입니다.

## 6. 자원 차이와 실제 예약 구분

직전 미승인 단독안의 보수계획 준비472,785,403 / 실행1,599,909,093 / 전체2,072,694,496바이트를 출발점으로 삼되 실측값이라 주장하지 않습니다. 플랫폼 분리의 추가 실행1회는 기존 강도를 유지한 Source 전수 대조3회를 예약하므로 3×60,000,000=180,000,000바이트를 실행에 추가합니다. 과거 소비·미계측·미소비 예약은 삭제하거나 복원하지 않습니다. 이전 실행9S 예약을 새12S로 대체하는 것이며 9S와12S를 중복 가산하지 않습니다.

| 항목 | 새 보수계획 | 기존 상한 | 제안 상한 |
|---|---:|---:|---:|
| 준비 | 472,785,403 | 512MiB | 512MiB 유지 |
| 실행 | 1,779,909,093 | 1536MiB | **1792MiB로 확대** |
| 전체 | 2,252,694,496 | 2048MiB | **2304MiB로 확대** |

이는 기존 상한 안이라는 주장이 아니라 정확 상한 변경 요청입니다. 준비/검수/메타 출력8MiB·제어묶음64MiB·각 실제 로그/파이프 한도는 유지합니다. 새Source 실제 크기와 원장 분류별 소비/잔여·중복 없는 예약을 준비에서 다시 결속하고 어느 상한이든 불명/초과면 실행0입니다. 이번 조사 전수 읽기58,900,890바이트와 설계/권위 읽기도 별도 조사 소비로 보존하고 새 준비 소비에서 숨기지 않습니다. 추가 도구/검수량은 기존 보수 여유로 자동 면제하지 않고 실제 추가분을 계상합니다.

## 7. 이후 전달과 비순환 선행

이 차이안은 P0와 아래 새 후속 전달 슬롯을 함께 승인 요청합니다. 통합1은 fixture 자연 보스 인과이지 실제 Windows 단독 권한 양성의 대체가 아닙니다. 빌드 전에는 순수/정책·복구·자연 보스·필수 회귀 및 독립 영향 검수만 선행이며, **새 빌드의 단독 권한 양성을 빌드 선행으로 요구하지 않습니다.**

과거 BVP의 tests-play/check.json과 tests-editor/check.json은 둘 다 passed=false이며 과거 Source 지문에 결속되어 있습니다. 기존 호출3·빌드2·자동6의 미소비 여부를 신규 권한으로 추정하지 않습니다. 다음은 과거 슬롯 복원이나 미소비와의 이중계상 없이 **새 유한 슬롯**으로 요청합니다. 이 안 밖 이전 슬롯은 사용하지 않습니다.

### 7.1 새 필수 회귀 두 호출

P0 네 단계가 같은 새Source에서 실제 통과·기존 요구에 따른 Luna 수용을 마친 뒤 같은 루트 attempts/regression-play-01과 regression-edit-01을 사용합니다. 각600초(560/30/10), 최대2회·각 단회·첫 실패 후 후속0입니다. P0와 합하여 Unity 총6회이며 보정용 Unity 재실행 슬롯0입니다.

PlayMode 정확11은 Chapter1ExpeditionPlayModeTests의 AuthoredHubSubmitStartsExpedition, AuthoredHubClickStartsExpedition, HeldSubmitDoesNotStartOrRestart, AuthoredFailureReturnsAndAllowsFreshStart, RestartDoesNotResumeRun, AuthoredReturnPreservesDevelopmentProfile, VerificationExitApproachBrakesAndRecovers 7개와 Chapter1ClosingPersistencePlayModeTests의 BothBranchesCloseOnlyAfterRealPassageAndSave / AuthoredBarrierStartsAboveFloorWithOpenSight 각 인수1/2 네 사례입니다. 원 BVP §7의 완전 이름과 최종 실제 선언을 대조하고 정확 집합으로 고정합니다.

EditMode 정확10은 Chapter1OrdanStaticDisplayPlayModeTests의 AuthoredGraphOwnsOnlyOrdan, RejectsDuplicateOrWrongCollider, RejectsWrongShapeOffsetAndGraph, ImportedOriginalResourcesMatchProfile, BodyScaleAndFootPivotMatchApprovedCoordinates 5개와 Chapter1BossVerificationPolicyPlayModeTests의 CompletedTickIdentityAndDuplicateCalls, AttackRequiresConsumedRelease, FocusResumeRequiresTwoNeutralUpdates, ApprovedGeometryBoundariesAndTimingOffsets, BossDamageTraceCausalityAndLimits 5개입니다. 원 초안의 NaturalBossPolicyFromAuthoredFlow 제안명은 현재 실제 IsolatedApiBossPolicyFromAuthoredFlow로 바뀌었으며 P0 마지막 통합1의 같은Source 실제 통과를 재사용합니다. 이를 포함한 원 정책6의 의미를 모두 보존하고 자연 통합을 중복 실행하지 않습니다. 시간5 재사용 조건은 §4와 같습니다. XML 각11/10 및 fixture Enter/Exit·정리 원시가 요구됩니다. 회귀2를 한 번의 Luna 묶음 검수300초/읽기4MiB로 검수하며 사소한 핀마다 반복 검수를 늘리지 않습니다.

### 7.2 새 빌드·포장·같은 배포본

회귀 및 AC-BVP-005/007 충족 뒤 새 build-01 최대1회1800초(작업1760/회수30/폐쇄10)와 포장/전체지문 단계600초를 요청합니다. 실제 기존 Chapter1AutonomousBuild.BuildWindows를 재사용하고 제품 빌더는 수정하지 않습니다. 이 빌더는 output/chapter1-autonomous-20261021/build/windows 아래 최초 비존재 attempt-NNN을 자동 선택하므로 실행 직전 실제 최초 빈 leaf 하나를 확인·핀 고정하며 예상과 다르면 중단합니다. 기존 출력 덮어쓰기0입니다. 완성 배포본은 새 루트 release와 release.zip에 복사/포장하고 원본과 전수 동일성을 확인합니다. 시험조립체/NUnit/TestRunner가 릴리스에 들어가면 실패입니다. 사후 고정 PID3952/LC83672 검사기를 새 성공 평가기로 사용하지 않습니다.

새 Windows 자동 검증은 branch1-01 → branch2-01 → failure-retry-01 정확3사례, 각220초(준비/작업180·회수30·폐쇄10), 전체 실제창/후검900초 묶음 안에서만 수행합니다. 사례마다 전수동일한 새 작업공간 복사본·빈 저장·증거 CreateNew이며 같은 최종Source/배포 지문을 결속합니다. 단독 양성은 실제 Driver 인계→자기 사건/상태소비→유한계속을 각각 관측하고 실제 피해·보스종결·양분기종결을 대조합니다. 미발급/미소비·수명/신원/종결 불명은 실패입니다. failure-retry는 자연 피해→HP0→실패/저장→거점→새세대 재출정을 요구합니다. 첫 실패·기한·미폐쇄·공식 심사 거절에서 후속0이며 새 보정 빌드나 자동 재시도 예산0입니다.

옵션 없는 같은 빌드 기본실행은 별도 default-ui-01 최대300초, 잘못된 옵션 거절은 invalid-option-01 최대30초를 요청합니다. 기존 지원된 정확 게임창의 일반 마우스/키만 최대캡처12·클릭6·일반키4로 사용합니다. 지속키 옵션/Native/별도입력기0이며 실제 지원이 없거나 필요한 관측이 부족하면 미검증으로 남기고 중간 사용자 조작을 요구하지 않습니다. 자동 사례 실제창은 각사진8·활성2·게임클릭2 상한이며 자동 운전과 충돌하면 클릭0입니다. 물리 지속키·사람 체험은 이 예산의 성공으로 대체하지 않습니다.

### 7.3 후속 도구 범위와 자원

기존 P0 두 도구 외 새 루트 tools/delivery-run-unity-owned.ps1, delivery-check-unity.ps1, run-window-owned.ps1 정확3파생파일을 추가 요청합니다. 각각 원 BVP tools/run-unity-owned.ps1 / check-unity.ps1와 r14 공식 문맥 run-window-owned.ps1를 before에 원문 보존한 뒤 현재 부모·Source·새 필터/기대수·새 준비/결과 gate·빈 출력/복사본·기한·전체지문/실제 양성 원시 평가만 재결속합니다. 기존 프로세스/파이프/소유·비소유 LC·보호·폐쇄 수용식은 그대로 유지하며 새 감시 체계/고정 과거PID 수용0입니다. 원 BVP가 이미 승인한 정확 비소유 반복LC 예외만 재사용합니다. 공식 창 도구의 실제 기능/옵션은 준비에서 확인하고 지원되지 않으면 대체 기술을 새로 만들지 않습니다. 지원된 도구로 닫을 수 없으면 해당 관측은 미완료로 남깁니다.

후속 도구 준비는 별도1200초(코드/국소600·기록600), 메모리 구문/연결 자체보정 최대1묶음이며 제품 추가 변경0입니다. P0Source·종료/gate 불변을 확인한 후 Luna 영향 검수300초/읽기8MiB로 닫습니다. 최종 Luna 묶음 검수600초/읽기16MiB와 Astra 통합1회를 요청합니다. 승인 범위 내 동적 실행 PID·빌드 leaf·새 실제 핀은 재승인 사유로 삼지 않습니다.

후속 회귀2·빌드1·포장/전수검사·복사본3·기본/잘못된옵션·결과검수의 신규 자원은 P0 §6과 분리한 준비256MiB·실행4096MiB·전체4352MiB 상한을 제안합니다. 이는 실측 예측이 아닌 유한 상한 요청입니다. Source는 회귀6S+빌드 전후2S=최대480,000,000바이트, 배포본 크기 상한256MiB와 원본/ZIP/복사본 검증 합최대12B=3,221,225,472바이트를 예약합니다. 후속 준비/도구/로그/검수 추가는 최대512MiB, 합4,238,096,384바이트로 전체 상한4,563,402,752 안에 둡니다. 준비/실행의 각 분류에도 실제 계상하여 어느 개별 상한이든 초과하면 중단합니다. P0 기존소비와 신규후속 자원을 합쳐 허위 잔여를 만들지 않습니다. 실제 B가256MiB를 넘으면 복사/재스캔을 시작하지 않고 계획false로 닫습니다.

실제창/표현/물리키/최종수용은 모두false를 유지하며 Luna 묶음 독립검수·Astra 통합 전 완료를 선언하지 않습니다. 기능 기준 빌드는 최종 미디어 완결 수용과 구분하며 이 차이안은 추가 아트/장면/음향 변경을 승인하지 않습니다.

검토 요청: Luna는 권한 위조·철회 예외·sequence/장치/초점 누락·플랫폼24+8 선택·추가3S/상한 확대·시계5 재사용 영향·양성 선행 비순환을 반례 검토해 주십시오. Astra에는 상세 조사 재수행이 아닌 위 종류별 Source 대응·정확4제품/2도구·4호출·자원상한 변경·후속 선행 경계의 결정만 요청합니다.
