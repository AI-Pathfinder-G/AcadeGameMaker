# B1 기본 이동 관측·연속 위험 원문·사후 수용의 한정 계약

- 상태: **Approved — 정확 7파일 관측 구현·후속 검사 계획 준비 한정**. 현재 native·컴파일·Unity·GUI·Play·방 수용 기록 실행 권한은0이다.
- 승인: 2026-10-08 실제 아스트라 `gpt-6-astra/ultra`. 구현은 솔 `gpt-6.1-sol/ultra`, 독립 검수는 루나 `gpt-6-luna/max`, 실제 프로세스·GUI 실행은 별도 승인 뒤 Root 단독 소유다.
- 사용자 동작: 기준 이동만으로 여섯 방의 지원 열 구성을 실제 통과하고 독립 검수한 방을 정규 출정 풀에 편입한다. 최종 목표는 현재 소스의 로컬 CH1 체험·완주다.
- 검수 기준: 솔 v3 54097바이트/SHA `F2425B91B5B77932650D3DB3D60B439759B7B002D51A0DE3EBA574DE0B907113`, 루나 규범 검수 v1 SHA `A5C98C1F0760307E10F184CFF0796CEF9A76490F846D9D4CDF6EEDB3E93D8FCC`의 실제 정수 P0/P1=0/0.
- 원문 합성: 아래 §1~9는 검수한 v3의 해당 본문을 그대로 보존한 사본이다. 당시 초안·미발급·쓰기 종료 표현과 정책 질문은 역사이며, **현재 규범 채택·권한·실제 기준은 §10의 명시 판정**을 따른다. 준비 원문 자체는 변경하지 않는다.
- 실제 선행·종료: R3 한정 수용과 실제586 기준을 결속했다. Root의 정확7파일 단일 소유 인계 뒤 솔 구현, 새 실제 원장·쓰기 종료·루나 독립 코드/델타 검수·아스트라 소스 판정까지 진행한다. 실제 실행·동작 수용은 각각 후속 관문이다.

## 1. 실제 근거와 현재 미발급 상태

아래 `preparation/`, `evidence/`, `project/`의 기준은 `output/chapter1-playable-r1/`이다. 과거 관측과 현재 파일을 구별한다.

| 근거 | 직접 읽은 SHA·적용 범위 |
| --- | --- |
| `preparation/b1-basic-movement-observation-and-acceptance-design-astra.v1.json` | `1F620A4CF217A673E9BD845F68EE25E34A2D4D03C6804C76BCEEEC382307E5F3`. 새 Editor1+meta 후보와 미결4이며 승인 아님. |
| `evidence/b1-basic-movement-observation-design-review-luna6.v1.json` | `581717ABC719F14580418333344874AC518725D1F7E0C68E77D89DC33CFAEEA6`. 실제 루나/max, P0/P1=0/0은 설계의 한계 표시를 검수한 결과다. 실제 순수 읽기·명령·플레이 수용 아님. |
| `preparation/b1-basic-movement-pure-getter-map-sol61.v1.json` | `C6A4BC533A4443D40ADAC56BFB3B399E56CADEE58A5A0DE89A38E26F341DF322`. 기존 getter 본문과 순수/조건부/발행 경로 구분. |
| `preparation/b1-basic-movement-actual-window-plan-sol61.v2.json` | `CAE5EC3AF9A652E14F8D816AC791684F63F387E57D85B3751B559A55949557C4`. 열 구성과 기존 동선. R06의 `(14,0)` 포함. 실행 승인 아님. |
| `docs/specs/work-contracts/2026-10-07-b1-runtime-gate-order.md` | `A9165E0C6C4CA3E491EE541C8F62336D1A8083143F258115BE69CD3B4BD2E255`, Approved §5. G3 실제 여섯 방/지원10 수용→집중35 순서와 기존 상한 유지. |
| `evidence/b1-final-source-checkpoint-sol61.v6.json` | `C1D5D35F899E2D2D7599E4233BBF60549B643A38F273018B3D63D433D23DC8F1`. v6 당시568, v5 뒤 Builder 한 문장 이동1. 이 초안의 작성자는 해당 소스의 독립 수용을 발급하지 않는다. |
| 현재 저작 복구 권위 | ASL Approved `10CF2D4BD5DB53499A22698721CA9E4293EA644733BCB60DF41DEA807C410E34`; 준비 승인 `08C6DB3D110EB3197814D31D6045605EF84C555A98995A7C0C080052CB3E6718`; master §29 `EFA3020DB138D3434C6660A8092418B34BA76C40D6B7D9C4F6680CE5AF8258EB`. 현재 r7/r3의 고정 입력이며 이 초안으로 수정하지 않는다. |

현재 실제 GUI 능력·새 관측 코드·그 코드의 검사·관측10·수용6은 미발급이다. r3 실제 생성9/meta9·586과 독립 수용/소유 반환은 Root의 진행 중 저작 복구 결과로 판정하며 이 초안에서 미래 성공을 발급하지 않는다. `568+18=586`, 정확7 후보의 기존3 변경+신규4 뒤 `590`은 조건부 산술 예상일 뿐 현재 파일 수가 아니다. 향후 각 단계의 실제 전수 원장을 새로 기록한다.

기존 `Lifetime.TryGetCompletedEvidence→InputRouter.TryGetChapter1CompletedEvidence`는 체력 객체/리비전을 발행할 수 있다. `CurrentReceipt`는 오류를 기록할 수 있고, Movement `Snapshot/NextExpectedTick`는 `EnsureMotor`를 통해 물리 설정과 모터를 만들 수 있다. 관측기가 이 경로로 정상 제작자의 역할을 대신하면 안 된다.

### 1.1 6000.6 공식 자료·현재 조립체의 직접 조사

이번 작성은 실제 `gpt-6.1-sol/ultra`의 정적 조사다. `ProjectVersion.txt` 83바이트/SHA `39584C3E6C1E571D53A79EF2DD6EA91C0568AC086792CC435E86401A0BA824ED`는 `6000.6.0f1 (f7f8ed4d1e24)`다. 설치 `C:\Program Files\Unity\Hub\Editor\6000.6.0f1\Editor\Unity.exe`의 파일 속성도 ProductVersion `6000.6.0f1_f7f8ed4d1e24`, FileVersion `6000.6.0.16251117`로 읽었다. EXE를 시작하지 않았다. 6000.0 링크뿐 아니라 아래 **6000.6 공식 문서**를 직접 확인했다.

- [예약 폴더](https://docs.unity3d.com/6000.6/Documentation/Manual/SpecialFolders.html): Editor 폴더의 MonoBehaviour는 GameObject 구성 요소로 부착할 수 없다. v2의 같은 Editor 파일 수집기는 불가하다.
- [조건부 컴파일](https://docs.unity3d.com/6000.6/Documentation/Manual/conditional-compilation.html)과 [정의 기호](https://docs.unity3d.com/6000.6/Documentation/Manual/scripting-symbol-reference.html): `UNITY_EDITOR`로 코드 포함 여부를 한정한다. Runtime 위치와 Editor 전용 실행 조건은 별개이며, 이 초안은 타입 전체 조건부 제외를 명시한다.
- [기본 실행 순서](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/DefaultExecutionOrder.html): 작은 순서가 먼저 실행되고 UI 설정이 코드값보다 우선할 수 있다. 동일값의 타입 순서는 보장되지 않는다. [MonoImporter.GetExecutionOrder](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/MonoImporter.GetExecutionOrder.html)는 UI 순서 읽기라는 한계를 함께 기록한다.
- [Editor 갱신](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/EditorApplication-update.html): Play 갱신에 결속되지 않는 가변 주기다. 따라서 최신 위험 슬롯을 매 tick 읽는 수단으로 단독 수용하지 않는다.

공식 설명으로 배치 적법성과 후보 순서를 추론할 수 있지만 현재 프로젝트에서 부착/호출/최초tick/GUI 수집이 성공했다는 증거는 아니다. 웹 원문 SHA나 성공 수치는 발명하지 않는다.

| 직접 읽은 파일 | 실제 SHA·참조 판단 |
| --- | --- |
| `Runtime/Input/Unity/AcadeGameMaker.Input.Unity.asmdef` | `CD3E47DD94BCCB9E537CA7AF4C6029FC8327EA0154C90EC7B6FD305218FE4C0E`. 기존 Movement.Unity/Combat.Unity/Transfer.Unity 참조, 엔진 사용, 플랫폼 제외 없음. 새 수집기는 같은 조립체에 편입할 후보다. |
| `Runtime/Input/Unity/AssemblyInfo.cs` | `C77C59207241974696667B99BDD87894A7C66DE0A7E692C7076923FC94C44AA2`. Combat.Authoring.Editor friend가 이미 존재한다. |
| `Runtime/Movement/Unity/AssemblyInfo.cs` | `1C0E7370AA05439FA9DF9A0BDDCCA28ED4C5719F4124D969FB2D08783DAD79BB`. Input.Unity 및 Combat.Authoring.Editor friend가 이미 존재한다. |
| `Runtime/Combat/Unity/AssemblyInfo.cs` | `5EBBF05888EBDAFA263E0CED95C40086D9A542C7B76F0C13D95799B76C69D5C1`. Input.Unity 및 Combat.Authoring.Editor friend가 이미 존재한다. |
| `Runtime/Transfer/Unity/AssemblyInfo.cs` | `CAAA76CF3E164CC287899756544C8A20540EEC08231CE4DBFD161D33BACD7A44`. Input.Unity 및 Combat.Authoring.Editor friend가 이미 존재한다. |
| `Editor/CombatAuthoring/AcadeGameMaker.Combat.Authoring.Editor.asmdef` | `37E33E9BC854754368503F5B2FA87145F9002F95CBBC228A1D94FD14B8B05EA7`. Editor 플랫폼 전용이며 Input.Unity를 이미 참조한다. 새 창→조건부 수집기 참조만 필요하고 역참조는 필요 없다. |
| `ProjectSettings/EditorSettings.asset` | `4A87C54B3541C7BD242AAD68C3FC28661083DB1117EC56149694EE592156415E`. 실제 m_EnterPlayModeOptionsEnabled:1/m_EnterPlayModeOptions:0. 설정 변경0이며 재로드 뒤 관측 결속 성공은 별도 실제 관문이다. |
| `ProjectSettings/TimeManager.asset` | `8FA4333AC4E99D3C522B512E4F9EC78D73401504AC0EFAECE4AEAAFA0E45DFFF`. 실제 Fixed Timestep:0.016666668/TimeScale:1. 배속/프레임 강제 진행 변경0. |

위 Runtime/Editor 경로의 기준은 `project/Assets/AcadeGameMaker/`다. Lifetime 원문 SHA `4C3CE9B201D99D5D6E02D5BF3BE3E973E2FDBDE15FECBD59B04BA03FEEDAC6FC`, DefaultExecutionOrder200/Start256/Fixed321 및 실제 Health·위험 발행149–153을 다시 대조했다. Lifetime meta SHA `A13B17AE1048068D35D7C314C00ABF219CF38CDBBA47F90489168E0731A146E6`의 executionOrder는0이며 이 숫자를 코드200의 실재 호출 증명으로 사용하지 않는다. Combat 원문 `1884B8013727241F8C86D441466104E8F6E2E27A6D4B395ABF3C737BE137DF7B`, Transfer 원문 `9E4063610ED548B16E62EBD65565E6984166700A697EC5EE52A597FEF6244C92`, 현재 Builder `B7F8046CEB231DAF8C9CF782E7B092E1FFF0F95D66A4DE7476B8EC802877AAC5` 및 정확 Validator `Chapter1ExpeditionAuthoringValidator.cs`의 `EE1FCA735BF33898DD273AAB74031684A2B2D9D27B425CEE58FB772D5DE25F0D`도 직접 읽었다. 기존 reader 대상3의 실제 원문 지문은 §2와 같다.

## 2. 정확 제품 후보·승인 선행

**REQ-C1B-G3OBS-001:** 기존 Runtime reader3 변경과 신규 Runtime 수집기 C#/meta2·신규 Editor 창 C#/meta2의 **정확7파일**만 별도 한정 승인 후보로 제안한다. 기존 메서드 본문·생산 순서·입력 callback·상태 발행·직렬화 필드·가드·원본 graph/meta·Builder/validator/프로필/저장/선택 기술 변경0이다. 새 수집기 자신의 편집기 조건부 관측 필드·정상 수명 callback은 아래 명시 범위이며 기존 Runtime 필드/생산 callback 변경과 구별한다.

| 정확 제품 상대 경로 | 후보 변경·현재 원문 SHA |
| --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | 기존 저장값을 읽는 internal 메서드1 추가만. `E716B4FAB288DBFBB0EA2E1692F6CC04304473025B31A0BF08E59D4F50B074C5`. |
| `Assets/AcadeGameMaker/Runtime/Movement/Unity/PlayerMovementController.cs` | 기존 모터를 읽는 internal 메서드1 추가만. `A1595590B51CD429E675706E85C745E7A0BFA72F291E4DE1531BCEA1C650017A`. |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1HazardObservation.cs` | 실제 KillPlane/LethalCrush 원본 참조 reader1 추가만. `A4EA8D413872DA2FA12ECD8A5EED1219028672A0FE5C2D9F79E529E0F687603C`. |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs` | 신규1. 파일명과 같은 MonoBehaviour 타입 전체를 `#if UNITY_EDITOR`로 제외/포함한다. 기존 Input.Unity 안에서 자기 관측 설정·Start/FixedUpdate210·종료/정리·단일 원시 writer만 소유한다. `UnityEditor` 참조·게임 발행/진행 입구0. 현재 미생성. |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1BasicMovementTickObserver.cs.meta` | 신규1. 타입/import·명시 순서 후보와 결속할 새 지문. 기존 meta 변경0. 현재 미생성. |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs` | 신규1. 기존 Editor 조립체의 창·재생 전 단회 구성·최신 보존 행 표시·사후 승인 적용 명령. MonoBehaviour 정의/부착 대상0. 현재 미생성. |
| `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1BasicMovementEvidenceWindow.cs.meta` | 신규1. 기존 meta 변경0. 현재 미생성. |

게임의 새 runtime 필드·발행 API·완료 뷰 타입·context·token·receipt 생성0이다. 새 수집기의 관측 설정/현재 원본 참조/기록 상태는 `UNITY_EDITOR` 내부에만 존재하며 게임 상태 발행이나 게임 권위로 소비되지 않는다. 기존 `AcadeGameMaker.Input.Unity` 및 `AcadeGameMaker.Combat.Authoring.Editor` 참조·friend를 재사용한다. asmdef/friend/Runtime→Editor 역참조 추가0, 새 Action/event·생산 이력 버퍼·PlayerLoop 변경0이다. Runtime 추가 파일을 조건부로 둔다고 제품 변경을 Editor 파일1로 축소 집계하지 않는다. v2 정확5 후보에서 정확7로 명시 확대하되 이 초안은 승인/구현이 아니다.

실제 선행은 주력 r7/r3 실행 폐쇄·Root의 실제 생성9/meta9 결과 및 독립 QA·Astra 판정·쓰기 소유 반환이다. 후속 구현은 **R3가 실제 성공한 뒤 발급한 기존568+후보18=586의 실제 소유 반환 원장**을 출발점으로 새 현재 입력을 결속한다. 성공/586 원장이 미발급이면 구현 인계0이다. 이어 새 규범의 Luna 검수·Astra Approved 및 정확7파일 Root→Sol 단일 쓰기 인계가 필요하다. 기존3 delta+신규4가 실제 완성되면 조건부 총590이나 현재 실제590을 발급하지 않는다. v6 당시568을 후속 규범의 고정 현재 분모로 강제하지 않는다.

구현 뒤 실제586 출발 지문/나머지583 불변·기존3 delta/신규4·실제590을 새 전수 원장과 쓰기 종료로 기록하고 Luna 독립 델타·Astra 한정 소스 판정을 거친다. 숫자 조건은 원래 R3가 정확586을 실제 반환한 때에만 적용하며 다른 실제 파일 수이면 자동 보정하지 않고 새 인계부터 판정한다. 현재 ASL `10CF2D4B...`/master `EFA3020D...`와 r7/r3의 고정 입력은 Root native 결과 QA 전에 수정0이다. G3 Approved 전이는 그 종료/판정 뒤 별도 통합한다. 새 입력·권위 핀을 결속하기 전 실행은 닫힌다.

## 3. 저장된 원문만 읽는 세 메서드

**REQ-C1B-G3OBS-002:** InputRouter에 제안하는 이름/서명은 `internal bool TryReadChapter1PublishedObservationForAuthoring(out InputFrameCommitReceipt receipt, out Chapter1FailurePublication health)`다. 현재 이 API는 없다. 출력을 기본값/null로 두고 아래 이미 저장된 필드와 값만 검사한다.

- 기존 저장 위치는 `_currentReceipt`(252), `_uiPublicationProof`(261), `_chapterEntry/_chapterEntryInstalled`(85–86), `_chapterHealthPublication/_chapterHealthRevision`(114–115), `_chapterClosure`(89–90)다. 생산 입력은 기존987–990에서 proof 후 receipt를 마지막에 저장한다. 생산 완료 조회는 기존204–211에서만 Health를 발행한다. Review host의 기존 `FixedExpedition`(149–154)가 이 조회/캐시/위험 게시를 실제 실행한다. 관측 getter는 이 생산 호출을 하지 않는다.
- regular chapter entry가 실제 설치·초기화됐고 살아 있는 같은 issuer의 `IsIssuedEntry(this,_chapterEntry)`(Lifetime255)가 참이어야 한다. `_faulted/_disposed/_actionsClosed/_executionGuardBlocked/_executionGuardClosed/_resetCutoverTerminal`, Chapter closure, 준비/회복 예약, 미완료 fault stage는 거절한다. Hub·Desktop 준비/리셋·boss 경로는 이 G3 reader 대상이 아니다. getter는 해당 경계의 상태 전이·프로필 검사 API를 호출하지 않는다.
- receipt/proof가 모두 존재해 원래 값이 같아야 한다. `GameplayEnabled`, 원래 modeEpoch와 저장 `_modeEpoch`, 원래 frameOrdinal과 마지막 저장 `_frameOrdinal-1`이 맞고 UI frame/proof는 없어야 한다. 음수/잘못된 값은 false다. `ValidateReceipt`도 새 canonical receipt를 생성하므로 이 getter에서 부르지 않는다. 새로운 receipt를 조립해 같다고 주장하지 않는다.
- health 객체가 이미 존재하고 `OwnsChapter1HealthPublication`(174–175)의 실제 객체·token·issuer·router 동일성을 만족해야 한다. Health kind, 양수 revision/저장 revision, entry의 실제 RunKey/Generation, health의 원래 receipt와 현재 receipt 일치도 직접 비교한다. 원래 발행 객체와 원래 receipt 값만 out으로 반환한다. 생존 여부는 조회 성공과 별개이며 HP0/triggered를 살아 있음으로 바꾸지 않는다.
- 저장된 고정 body와 현재 실제 capsule/scale/rotation이 다르면 거절한다. 이 비교는 기존199–200의 읽기 항목을 사용하며 body/physics/transform에 쓰지 않는다. getter 내부 `CurrentReceipt`, `Chapter1Lifetime`, completed 조회, lazy 초기화, 발행, `LatchFault`, 로그/파일 쓰기0. false는 runtime 진단·revision·토큰·저장 필드에 무기록이다. false의 기본 out은 Editor 원시에서 null/미관측으로 남기며 완료 표본으로 내보내지 않는다.

**REQ-C1B-G3OBS-003:** Movement에 제안하는 이름/서명은 `internal bool TryReadExistingMotionForAuthoring(out PlayerMotionSnapshot motion, out SimulationTick nextExpectedTick)`다. 현재 이 API도 없다. 살아 있는 활성 controller의 `_motor`(24)가 null이면 기본 out/false로 끝낸다. 기존 모터의 순수 `Snapshot/NextExpectedTick`(PlayerMovementMotor192–193)만 반환한다. controller의 기존 getter146–147, `EnsureMotor`755–776, Get/AddComponent 초기화, 물리/위치 설정, Seed/Advance/Prepare/Commit 호출0이다. 새 모터·필드·발행0이며 이 메서드 단독 true를 완료된 게임 tick이라고 해석하지 않는다.

**REQ-C1B-G3OBS-011:** `Chapter1HazardObservation.cs`에 제안하는 정확 서명은 `internal bool TryReadPublishedHazardsForAuthoring(out Chapter1FailurePublication kill, out Chapter1FailurePublication crush)`다. 현재 API는 없다. false 때 둘 다 null이며 runtime 무기록이다. 실제 위험 true를 숨기기 위해 false를 반환하지 않는다.

- 기존 필드는 `_router/_definition/_firstTick/_configured`(107–110), 원본 `_killToken/_crushToken`(111), `_kill/_crush`(112), `_revision`(113)이다. 활성·구성 source와 실제 활성 regular router가 있어야 하고 기존 순수 `IsFaulted`는 false여야 한다. 파기/정체 불명은 거절한다.
- 두 원본이 존재하고 기존 `Owns`(150–152)에 각각 속한다. Issuer는 이 source, Token은 각 원래 private token, Router는 `_router`와 같은 참조다. Kind는 KillPlane/LethalCrush, Revision은 둘 다 양수 `_revision`, RunKey/Generation/원래 receipt는 서로 같다. tick은 `_firstTick` 이상이며 GameplayEnabled 표본만 대상이다. 두 원본 참조만 반환한다.
- `CurrentReceipt/TryGetCompleted/PublishCompleted`, 새 completedView/receipt/token/publication, `IsClosedAt`·위험 기하 재계산·물리 capture, 실패 중재·HP/모드·로그/파일/필드 쓰기0. Editor는 같은 host.CurrentHazards·actual Health/receipt/key/generation과 다시 대조한다. 위험 bool만 산출하거나 기본 false로 채우지 않는다.

정상 생산 경로는 Review 메뉴→Lifetime.Start(258–261)의 실제 review key/generation1/firstTick0→CreateExpeditionGraph producer 결속→Lifetime.FixedUpdate(200)의 FixedExpedition→Input completed 조회(149)와 Health 발행(Input204–209)→hazards.PublishCompleted(153)이다. 실제 발행 횟수는 미관측이며 source 존재만으로 발행을 가정하지 않는다. 이 key는 Review graph 키이며 출정 Save/RunStarted 권위가 아니다.

Review에는 `_port`/본편 FlowOwner·실패 중재자가 없다. Hazard138–141는 KillPlane/압착을 Health와 별개로 발행한다. R05 닫힌 phase `[60,120)`와 body 겹침은 crush.Triggered=true지만 HP0/host.IsFailed 자동 전이 코드는 없다. 둘 중 위험 true이면 HP>0이어도 관측 사례 첫 실패이며, 관측기는 게임 실패/HP/모드를 새로 만들지 않는다.
원래 저장값은 조회 동안 그대로 보존한다. Stop/파기 후 남은 관리 객체의 forensic 값이 있어도 새 live 성공을 반환하지 않는다. Editor는 종료 전 기록한 마지막 원문을 `live=false`로 표시할 수 있으나 다음 Play·세대·구성의 현재 표본으로 재사용하지 않는다. 새 runtime 캐시나 실패 재발행은 만들지 않는다.

## 4. 실제 완료 뒤 관측·연속 틱 보존·완주키

**REQ-C1B-G3OBS-004:** 새 창 메뉴 후보는 `AcadeGameMaker/Chapter1/Basic Movement Evidence`다. Editor 갱신은 표시/수명 관측에만 쓰며, 정상 성공 표본은 아래 동일 주 스레드의 실제 생산 완료 뒤에서 결합한다. 원시 소유자는 하나이며 파일을 동시에 두 작성자가 열지 않는다.

1. 같은 host/router/regular graph/player/Combat/Transfer/hazard의 실제 참조·활성·유일성 및 host IsFailed/IsReplacementPending/Generation/CurrentRouter/CurrentHazards를 고정한다. Play 전 메뉴/미저장 scene/수정 없는 SerializedObject의 `_reviewPrefab/_reviewRoom/_reviewSocket`을 결속한다. A/B는 scene 이름으로 판정하지 않는다.
2. Input 순수 reader가 실제 receipt/기존 Health 객체를 반환해야 한다. Health.RunKey/Generation은 host 세대와 같다. 관측 session ID는 게임 run key가 아니며 새 Guid/빈값/화면 문자열로 대체하지 않는다.
3. Combat `HasLatestOutcomeForEncounterHandoff→LatestOutcomeForEncounterHandoff`, Transfer `LatestPhasePublication`, Movement 새 reader를 읽는다. router bound player/combat/transfer·Combat BoundPlayer/BoundTransferDriver·Transfer BoundPlayer는 같은 실제 객체다.
4. hazard 새 reader의 원본 KillPlane/LethalCrush 둘을 읽는다. host.CurrentHazards와 issuer가 같은 source이며 Router/RunKey/Generation/receipt는 실제 Health/Input와 같다. kind/revision/token 신원은 §3대로 검사한다. Health revision과 hazard revision의 우연한 숫자 일치로 신원을 대신하지 않는다.
5. receipt/motion/combat/transfer/Health/kill/crush Tick이 같고 nextExpectedTick=tick+1이다. 실제 Combat player는 정확1; HP/dead/invulnerability와 Health.Triggered==(HP==0)를 대조한다. Transfer 원래 snapshot/입력 HadPress/HadAim을 읽으며 HadAim을 전이 사용으로 오인하지 않는다.
6. Input/Hazard reader와 host/객체/세대를 다시 읽어 같은 receipt, 같은 세 원본 publication/각 revision인지 확인한다. 바뀐 객체/세대/tick은 부분 표본을 버리고 실패 원시를 남긴다. 이전 최신값·기본 false로 채우지 않는다.

### 4.1 최신 슬롯의 덮어쓰기와 위험 누락

Health는 Input204–209, kill/crush는 Hazard138–142, Combat은 Driver342, Transfer는 Driver531에서 완료 tick마다 최신 슬롯을 덮어쓴다. Lifetime153의 `_latest`도 최신1개뿐이다. 현재 getter는 이력을 반환하지 않는다. 실제 simulation FixedUpdate가 두 번 이상 진행된 사이 EditorApplication.update가 한 번 읽는 경로에서는 중간 tick 원본이 이미 교체된다. Editor 갱신과 생산 FixedUpdate 사이 일대일 수집 경계는 현재 코드에 없다. 코드상 누락 가능성이고 실제 발생 횟수는 미관측이다. 최신 원문으로 누락을 복구했다고 주장하지 않는다.

이 v3 성공 조건은 첫 실제 Review tick0부터 최종 살아 있는 출구/정상 Stop 경계까지 연속 완전 표본이다. 첫 tick>0, 다음 distinct tick!=직전+1, 필수 hazard 부재/불일치·기록 오류는 첫 실패다. 마지막 false·영상 색·HP>0으로 누락 위험을 면제하지 않는다. 중복 tick은 별도 사건으로 세되 새 완료 표본으로 세지 않는다. 영상은 입력/지지 판독 보조이며 빠진 producer publication의 대체가 아니다.

### 4.2 배치 대조와 정확7 권고

**REQ-C1B-G3OBS-012:** v2의 같은 Editor 파일 `MonoBehaviour` 부착 안은 6000.6 공식 제한에 따라 폐기한다. 그 안을 미확인 후보/예비 실행으로 남기지 않는다. 권고는 §2 신규 Runtime/Input/Unity 파일의 `Chapter1BasicMovementTickObserver : MonoBehaviour` 타입 전체를 `#if UNITY_EDITOR` 안에 둔 구성 요소1이다. namespace/기존 Input.Unity 조립체/파일명-타입 결속을 유지하며 수집기에서 `UnityEditor`를 참조하지 않는다. 정상 Unity 호출의 자기 Start·FixedUpdate·OnDisable/OnDestroy 정리만 명시 추가하고 기존 생산 callback·PlayerLoop는 그대로 둔다.

| 수단 | 합법성·연속 원본 보존 대조 | 이 초안의 선택 |
| --- | --- |
| Editor 폴더/Editor 전용 조립체의 MonoBehaviour 부착 | 공식 폴더 제한 때문에 GameObject 수집기로 사용할 수 없다. `AddComponent` 성공을 가정하거나 별도 Play로 시험하지 않는다. | 제거. |
| EditorApplication.update만 사용 | 합법적 Editor 관측 입구이나 프로젝트 Play 루프에 결속되지 않는 가변 갱신이다. 두 FixedUpdate 사이 원문을 읽지 못하면 이미 덮어쓴 위험을 복구할 수 없다. | 표시/사건 수명만 담당. 연속 표본 수용 불가. |
| Editor 코드에서 공개 LowLevel.PlayerLoop API로 고정 단계 뒤 삽입 | 공개 API로 사용자 갱신을 추가하는 합법적 방식은 존재한다. 다만 현재 PlayerLoop를 교체하고 재로드/해제/원래 루프 복원·상대 단계·다른 삽입과의 충돌을 관리해야 한다. 기존 플레이 루프 변경0 범위를 넘어선다. | 이번 최소 범위에서 제외. 구현/호출0이며 별도 계약 없이는 대안 실행0. |
| Runtime/Input/Unity의 전체 UNITY_EDITOR 조건부 구성 요소 | 기존 Runtime assembly가 Editor에서 컴파일하는 정상 MonoBehaviour에 관측 callback을 두고, 기존 생산자(200) 뒤(210)에 각 완료 원본을 직접 기록할 수 있는 최소 배치다. Player에서 타입 전체가 제외된다. | 정확7 후보로 권고. 실제 부착/순서/첫tick은 미확인. |

이 판단은 [6000.6 폴더 제한](https://docs.unity3d.com/6000.6/Documentation/Manual/SpecialFolders.html), [Editor 갱신](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/EditorApplication-update.html), [PlayerLoop 교체](https://docs.unity3d.com/6000.6/Documentation/ScriptReference/LowLevel.PlayerLoop.SetPlayerLoop.html)의 공개 계약과 현재 참조 구조에서 추론한 권고다. 공개 API가 있다는 사실을 현재 실행 허가/실제 성공으로 해석하지 않는다.

### 4.3 재생 전 구성·200 이후 단일 작성자

1. Root가 기존 해당 Review 메뉴를 편집 상태에서 실행해 정상 생성한 named unsaved scene/host를 확인한다. 새 창이 승인된 동일 case·prefab/meta·source·실제 host 유일성을 읽기 대조한 뒤 그 장면의 **별도 임시 root GameObject1**에 수집기1을 재생 전에 단회 구성한다. host/graph/prefab의 자식/컴포넌트를 바꾸지 않고 이름 있는 실제 임시 객체/instanceId/scene을 원시에 결속한다. collector 자신의 구성은 새 필드에만 저장하며 원래 authoring setter를 재호출하지 않는다.

2. 재생 전 직렬화 경계를 넘길 후보는 case/room/socket/원시 정확 경로·기대 지문/승인 식별자 등 관측 스칼라와 **실제 저작 host에 대한 Unity 객체 참조1**뿐이다. live router/player/Health/kill/crush/receipt/token·게임 key/완료 권위를 저장/재사용하지 않는다. 기대 지문 문자열만으로 승인 권위를 만들지 않는다. prefab/scene/AssetDatabase.SaveAssets/EditorPrefs/SessionState/프로필에 설정을 쓰지 않는다. 현재 EnterPlayModeOptions를 바꾸지 않으며 Domain/Scene 재로드 후 live 원문은 다시 읽는다.

3. Play는 Root가 실제 GUI에서 단1회 누른다. 수집기를 EnteredPlayMode/Editor 다음 갱신에서 새로 만드는 경로는 tick0 손실 가능성이 있어 사용0이다. 기대 bootstrap 객체가 재로드 뒤 없거나 중복이면 실패다. 재로드 전/편집 상태의 수집기는 표본/게임 발행/원시 파일 작성0이며 ExecuteAlways/ExecuteInEditMode·DontDestroyOnLoad·상태 소유 기능0이다. 임시 장면을 저장하지 않는다.

4. 수집기 자신의 정상 Start210에서는 `Application.isPlaying`·자기 단회 설정·승인 원시 경계에 맞는 **자기 기록 준비만** 한다. 게임 준비/완료를 Start에서 만들거나 성공으로 선언하지 않는다. 정상 host Start200가 실제 graph를 준비하는 기존 경로는 그대로다. 게임 객체/세대/receipt/Health/위험의 최초 검증은 첫 정상 FixedUpdate210에서 §4와 순수 reader3로 수행한다. 새 instance의 비직렬화 writer/표시 상태는 초기화하되 이전 Play의 원시/게임 키로 자동 재개0이다.

5. 제안 attribute는 `DefaultExecutionOrder(210)`이며 Lifetime200보다 뒤인 **서로 다른 정상 스크립트 순서**다. 코드 attribute와 UI/importer 설정은 따로 기록하고 같은 값이라고 단정하지 않는다. 기존 meta의 executionOrder:0이나 MonoImporter.GetExecutionOrder의 UI값0을 Lifetime 실제순서0으로 오인하지 않는다. 새 파일/import 타입과 실제 설정을 독립 검수하고 허용 지문 밖의 순서/설정 변경은 거절한다. SetExecutionOrder/프로젝트 설정 변경0이다. 표본은 각 FixedUpdate의 생산 완료 원문이 실제 같은 tick임을 직접 확인해야 하며 정수210만으로 통과하지 않는다.

6. FixedUpdate210은 §4의 원문을 두 번 읽어 같은 참조/receipt/revision을 대조하고 현재 tick의 행을 그 자리에서 단일 writer가 직접 append/flush한다. collector가 새 게임 completed view/publication을 조립하거나 Lifetime.TryGetCompletedEvidence/CurrentReceipt/EnsureMotor/PublishCompleted를 호출하는 경로0이다. 원래 정상 producer가 이미 Health/kill/crush를 발행한 뒤의 source를 읽는다. 임시 객체에는 collider/rigidbody/input action·생산자/실패 중재자·Save/Run/모드 권한0이다.

7. collector는 원본 참조를 현재 읽기/전후 대조에만 보유한다. 직전 tick 식별 스칼라·고정 생산자/token 식별표·현재3 원본·최신 표본1/첫 실패1 외의 tick history·참조 누적·queue0이다. 현재 publication 식별은 지역 번호와 그 tick의 실제 ReferenceEquals/Owns 결과를 기록한다. 다음 tick의 Owns로 과거 원본을 재인증하거나 과거 참조를 전부 메모리에 남기지 않는다. 원시 JSONL가 보존 이력이며 창은 수집기 자신의 보존 행만 읽어 표시한다. Runtime→Editor callback/event/Action·두 작성자/백그라운드 게임 조회0이다.

8. 기록 I/O 시간·callback 번호·frameCount·실제 distinct tick과 중복/누락을 행에 남긴다. I/O가 실제 실행을 지연할 수 있는 한계도 원시/검수에 표시한다. 느린 writer가 게임 tick을 다시 만들거나 시각/입력을 보정하지 않는다. 행 작성/flush/크기 오류·정체/키 교체·source 변화·위험 true는 첫 실패로 닫고 성공/후속 사례를 발급하지 않는다. native hang/종료·마지막 행 소실도 정상 완주로 보충하지 않는다.

9. 정상 Play 진입 전의 재로드만 위 bootstrap 경계다. 수집 시작 뒤 domain reload/compile·창 파기/비활성·Play 종료·source 변화는 원시를 봉인하고 live=false로 닫는다. 자기 OnDisable/OnDestroy는 기존 게임 필드/graph를 건드리지 않고 열린 writer만 일회 정리하며 pre-Play/중복 callback은 새 기록을 만들지 않는다. Root가 실제 Stop/정확 프로세스를 소유하고 종료 뒤 남은 편집 임시 객체1만 정리한다. 불완전 종료를 새 도메인/Play에 자동 이어쓰기0이다.

첫tick0 이전 설치·스크립트 발견/부착·정상200→210·매tick 동시 원본 보존은 아직 미관측이다. 코드/참조/조건부 제외의 독립 검수 뒤 동일 최초 R01-A Play1 안에서만 실제 사실을 확인한다. 첫tick>0/원문 부재/연속성 실패면 Stop·나머지 사례0이다. 예비 Play, PlayerLoop 교체, 강제 FixedUpdate/Physics.Simulate/Advance, 생산 callback/history/event 추가로 실패를 보충하지 않는다. PD-G3-003을 포함한 새 Approved 및 쓰기 인계 전 제품 구현0이다.

아직 어떤 완료 publication도 없는 초기 Play에서만 구성 예산의 첫60초까지 unavailable 사건을 기록한다. 첫 정상 tick을 놓쳤거나 Health만 존재하고 같은 tick 위험이 빠지면 기다림으로 소급 완성하지 않는다. false/null은 미관측이지 완료된 false 위험값이 아니다. 실제 fault/폐쇄/교체/오류/위험 true·상한은 Editor 기록의 첫 실패 종료 이유로 보유하고 추가 성공 집계0이다. 게임 HP/host.IsFailed 변경·자동 Play/Stop/runtime 정리0이다.

새 파일/자기 callback 허용과 기존 Runtime 생산 callback 변경0은 별도 범위다. 정상 기록 종료 뒤에도 원시/완주키/첫 실패 행은 보존하며 live=false를 성공/게임 수용으로 해석하지 않는다. 창의 EditorApplication.update 구독1·playModeStateChanged 구독1은 표시/수명만 관찰한다. 실제 게임 원문을 다시 조회해 이전 행을 채우거나 Play 진입 후 수집기를 설치하지 않는다.

완주키는 `(관측세션, caseId, 실제 RunKey, 실제 Generation, 첫tick0, 최종 원래 receipt의 tick/frameOrdinal/modeEpoch)`다. 출구 순간과 Stop 마지막 정상 표본, 연속 tick 수/gap0·세 원본 식별표를 함께 봉인한다. 이는 게임 receipt/수용 권위 발급이 아니다. 구성 재결속은 Root Stop·원시 봉인 뒤 새 메뉴/새 Play에서만 한다.

## 5. 원시 스키마·표시·유한 보존

**REQ-C1B-G3OBS-005:** 실제 실행 승인에서 새 evidence 접두사와 정확 경로를 고정한다. 아래 이름은 스키마 후보이며 이번에 파일을 생성하지 않는다. 모든 파일은 CreateNew, 기존 원시 덮어쓰기·사후 성공 보충0이다.

| 원시 | 필수 명명 필드 |
| --- | --- |
| 세션 시작 | `schemaVersion/kind/sessionId`, 현재 `checkpointSha256`, 새 source/후보/meta/관측 도구 path·length·SHA, ORDER/새 Approved/실행 승인·도구 실제 심사 원문 지문, 실제 performer/model/역할, Unity EXE/version/PID/StartTicks/project/window/입력 대상, 단조 시작·예산 |
| 구성 시작 | `caseId/roomId/socketId/reviewMenu/sceneName`, 미저장 scene 정체, 실제 prefab/meta SHA, host와 actual graph 정체, 새 Play 사건과 실제 입력 중립, 원시 영상/입력 경로·collector 설치/첫 호출/Editor assembly/실제 실행순서·단일 작성자 정체 |
| 표본 JSONL | 실제 시각·단조 경과·collector 자기 Start/FixedUpdate/정리 호출번호/Editor 표시 갱신번호/frameCount·기록/flush 소요·첫tick bootstrap,  같은 세션의 실제 객체 참조 식별표/instanceId/계층, actual RunKey/Generation, 원래 receipt 네 값, 기존 Health/KillPlane/LethalCrush 세 원본의 지역 참조번호·Kind/issuer/token/router 동일성·각 revision/triggered, 실제 Combat HP/dead/invulnerability, motion 위치/속도/grounded/wall/state/다음tick, 실제 Transfer snapshot/revision/activeTarget/HadAim/HadPress/lifecycle, 전후 동일성·생산자/collector 순서·firstTick0·연속tick/중복/gap/부재·위험 첫tick/firstFailure |
| 연속 native 원시 | 실제 가시 창의 연속 영상 또는 동등하게 판독 가능한 연속 화면 원문, 실제 키·버튼 눌림/해제·포커스·Play/Pause/Stop/메뉴 사건과 도구 반환 원문. 호출 요청 문자열만을 실제 장치 이벤트 성공으로 바꾸지 않는다. |
| 구성/세션 종료 | 실제 종료·중립·오류/부재/상한/파기·시작/끝 시각, 실제 표본/누락/중복·세 원본 수·firstFailure와 완주키 또는 null, 전체 원시 path/length/SHA, 전후 source/후보/meta 비교, owner 종료·프로세스 폐쇄/실제 정수 exit 또는 null |

불투명 게임 token의 내용은 직렬화하거나 조작하지 않는다. 기존 객체에 붙인 Editor 지역 참조번호는 해당 세션의 ReferenceEquals 판독용이며 새 게임 토큰이 아니다. 계산한 body/발 위치는 실제 motion과 실제 collider/offset/scale에서 나온 투영으로 표시하며 runtime body나 completed view를 새로 만들지 않는다. 표본/원시에는 자동 `movementAccepted=true` 또는 독립 수용 통과를 기록하지 않는다.

연속 수집·표시/직접 append/flush/임시 객체 설치·정리는 전체18000초/각1800초에 포함한다. 구성당 서로 다른 tick 표본 최대108001, 표본 원시128MiB·사건 원시8MiB·native 영상/화면 원시2GiB를 유지한다. 표본 history/큐와 과거 publication 참조 누적0, 고정 식별표 최대32항목·최신 표시 행1/첫 실패1(각 UTF-8 직렬화 최대32KiB)을 후보로 명시한다. 이 크기를 실제 heap 사용량이라고 표기하지 않는다. 상한 도달·기록/flush 오류·기록 수단 소실은 중단하며 파일 삭제/경량 재작성으로 재개0이다. 이 수치는 미결 정책 후보이며 현재 실행 권한이 아니다. 창은 방/소켓, 실제 HP, 원래 tick, 지지/벽/전이 상태와 누락/종료를 읽기 쉽게 표시하며 최종 게임 HUD를 변경하지 않는다.

## 6. 실제 GUI 소유·10구성·완주 거절

**REQ-C1B-G3OBS-006:** GUI 후보는 Root 소유의 Unity6000.6.0f1 단일 가시 Editor 시작1회, 구성별 Play1회씩 정확10이다. G1/G2의 Build 또는 G7 별도 창 실행을 재사용/증액하지 않는다. 실제 GUI 전체 단조상한18000초를 제안하며 시작·가져오기/관측 코드 로드·메뉴전환·관측·Stop·종료를 모두 그 안에 포함한다. 구성별1800초와 전체 상한 중 먼저 닿는 쪽을 적용한다. 별도 startup900초 추가0, 자동 재시작/재시도0. 정확 소유 PID/StartTicks/EXE만 종료5초·출력 회수5초로 정리하는 별도 한정 종료를 계획에 결속한다.

같은 Editor에서 사후 적용 명령까지 수행한다면 Luna/Astra 원문 대기와 적용6도 전체18000초 안에 포함한다. 창을 먼저 종료했거나 남은 예산이 없으면 적용 명령은0이며, 새 창을 자동 열지 않는다. 별도 적용 전용 GUI 인계가 필요할 때는 플레이 재실행 없이 새 한정 승인/예산부터 명명한다.

실제 선행은 앞 실행의 폐쇄, 새 관측 코드/입력/도구 독립 검수, 새 Astra GUI 승인과 Root 소유 인계, native Editor 창·키/포인터·연속 원시를 실제로 제공하는 수단 확인이다. 현재 제공 여부는 미관측이며 browser 화면이나 설치 파일 존재로 native GUI 능력을 인증하지 않는다. 실제 창의 Play/Pause/Stop/Game/Console 명칭·언어·위치/단축키를 먼저 읽고 EXE/PID/ticks/project/window와 활성 Game 대상을 결속한다. Hub 설치 정확 두 경로는 Editor와 구별하며 어느 Hub도 종료하지 않는다.

Root가 현재 문서에서 확인한 Computer Use `@oai/sky`의 키 입구는 `press_key({window,key})`다. 지속 누름·기간 인자는 공개돼 있지 않다. 실제 반환된 window를 선택하고 **새 관측→단일 입력→즉시 새 관측**으로 조작한다. 없는 hold/duration/key-down 인자를 만들거나 단일 요청을 물리키 지속 누름으로 기록하지 않는다. 노드 sky 외 Win32/PowerShell GUI·입력 주입 경로0이다. 단일 입력이 실제 고정tick 이동/점프·중립으로 반영되는지, 연속 원시를 이 수단으로 충분히 남길 수 있는지는 G2 뒤 실제 창의 R01-A 안에서 확인할 미관측 조건이다. 이 확인을 별도 예비 Play로 추가하지 않는다. 입력 전달·해제·실제 이동·판독이 확인되지 않으면 미수용으로 정리하고 자동 대체 수단/재실행0이다.

Root만 실제 입력/프로세스를 조작한다. Luna는 같은 실행의 가시 창을 읽기 전용으로 독립 관측하거나, 전체 원시에서 판독 가능한 범위만 사후 검수한다. Root 수행을 Luna 직접 플레이로 표기하지 않는다. 필수 사실을 사후 판독할 수 없으면 미수용이며 새 실행은 별도 원인/승인 문제다. 독립 검수 편의를 이유로 동일10을 두 번 실행하지 않는다.

**REQ-C1B-G3OBS-007:** 메뉴는 `AcadeGameMaker/Chapter1/Review Basic Movement/` 아래 다음 정확 순서다. R01-A를 별도 예비 반복으로 추가하지 않는다.

| 순번·구성 | 기존 메뉴 끝부분 | 소켓 |
| --- | --- | --- |
| 1 R01-A | `R01` | `S01-A` |
| 2 R02-A / 3 R02-B | `R02` / `R02-B` | `S02-A` / `S02-B` |
| 4 R03-A / 5 R03-B | `R03` / `R03-B` | `S03-A` / `S03-B` |
| 6 R04-A / 7 R04-B | `R04` / `R04-B` | `S04-A` / `S04-B` |
| 8 R05-A / 9 R05-B | `R05` / `R05-B` | `S05-A` / `S05-B` |
| 10 R06-A | `R06` | `S06-A` |

Play 전 실제 A/B 구성·prefab/meta·미저장 review graph를 결속한다. Review는 ValidateRoom(prefab,false) 뒤 NewScene과 `Chapter1BasicMovementReviewRxx` 이름을 부여한다. 현재 ValidateRoom(14–35)은 RoomInstance.ValidateStructure·collider/producer 수·결속을 검사하며 r2 Build의 RegularEncounter validator→CombatTarget.Freeze 경로가 없다. 기존 활성 named scene이 Review의 필수 선행이라는 인과를 주장하지 않는다. Chapter1Playable.unity를 편집 상태에서 여는 것은 실제 GUI 문맥 확인이 필요한 때의 조건부 준비이며 별도 Play/scene 저장/프로필 권한이 아니다. 실제 Play graph의 named hierarchy는 Review가 정상 생성한 scene/name·host에서 판독한다. validator/Builder 변경0이다. standalone `Chapter1Playable.unity`의 실제 출정은 별도다. Game 포커스·키/버튼 해제 중립 뒤 A/D, Space의 새 눌림/해제, 필요 시 Shift 대시·유효 벽의 Space 벽차기로 진행한다. Q/E/Mouse 좌우 공격·전이/선택 기술, 가상장치/QueueStateEvent/직접 callback/ForTests/좌표 setter/transform 드래그/배속·프레임 강제 진행0이다. gameplay 중 포인터 조작이 필요하면 먼저 실제 Pause와 문맥을 원시에 남긴다. Pause 시간도 예산에 포함하며 무입력으로 가장하지 않는다.

기존 RequiredPath를 source/후보에 결속하고 실제 영상·원래 motion·실제 지지면으로 입구→필수 동선→출구를 판독한다. R02의 두1u 받침, R03 계단, R04의 x12→14 2u 틈, R05의 실제 압착 개방 바닥 경로, R06 x10→12 틈·(14,0)·x20~23 1u 받침을 생략하지 않는다. requiredPath는 발/바닥 위치이므로 중심좌표를 해당 y와 같다고 처리하지 않는다. 임의 좌표 허용치나 표시 색만으로 지지를 수용하지 않는다.

구성 종료 후보는 같은 actual key/Generation/graph에서 필수 동선·지지를 거친 실제 player가 기존 Exit `[30,0]~[32,3]`에 도달했고, 그 시점의 동일 완료 tick에서 HP>0/IsDead=false·Health/KillPlane/LethalCrush 모두 Triggered=false, GameplayEnabled 원래 receipt, Transfer Baseline/activeTarget 빈값/HadPress=false가 관측된 경우다. 전체 금지 입력 불사용은 연속 native 사건·영상과 publication 원시를 함께 검수한다. 건너뛴 tick을 Transfer 불사용/위험 false로 소급 계산하지 않는다. 첫tick부터 연속 원문이 완전하지 않거나 위험 원문을 읽지 못하면 그 사례 실패·미수용이다. Review HP0/host 실패 자동 전이·게임 rollback을 전제하지 않는다.

실제 Health/kill/crush 위험 true·사망·압착·필수 경로 막힘·금지 입력·외부 입력/포커스 불명·원문 변화·source/cohort/tick 불일치·오류·원시 소실·상한은 첫 실패이며 Play/관측을 정리하고 남은 구성0이다. 실패 raw는 보존한다. 실패 사례를 재설정해 같은 목록의 성공으로 교체하지 않는다. 실제 정상 종료 뒤에만 다음 구성의 새 메뉴·새 중립·새 Play를 허용한다.

## 7. 원시→독립 검수→기존 수용 입구

**REQ-C1B-G3OBS-008:** 관측 종료와 원시 봉인 뒤 R01/R06은 A1개, R02~R05는 A/B2개씩 방별 묶음6을 만든다. Luna의 실제 독립 보고는 `participant/reviewer.model=gpt-6-luna`, 실제 설정max, 정수 P0/P1, current checkpoint/observer SHA, roomId·expectedPrefabSha·metaSHA, 필요한 caseId/socketId의 정확 집합, 각 원시 manifest path/length/SHA·실제 key/Generation/firstTick0/최종tick/연속표본 수/gap0·세 원본 신원/완주키를 명명한다. 실제 endpoint/path/생존/금지입력/오류/종료의 판독 결과와 한계를 적는다. 누락/부족은 미수용이며 임의 SHA·Root 성공 문자열을 받아쓰기0이다.

Astra는 동일 보고/동일 후보/동일 source를 직접 대조한 방별 한정 수용과 exact6 적용 인계를 별도 발급한다. `sourceCheckpointSha256`, `observerHelperSha256`, `lunaRoomReviewSha256`, `roomId`, `expectedPrefabSha256`, `expectedMetaSha256`, `requiredCaseIds`, `applyOwner`, `finalApproval`을 명명 비교한다. 해시/schema는 오사용 방지이며 실제 GPT 참여/플레이를 자체 인증하는 장치가 아니다.

새 명령 후보 `AcadeGameMaker/Chapter1/Apply Approved Basic Movement Evidence`는 Play/컴파일/import가 꺼지고 모든6의 원문/승인/소유가 선행한 때만 직접 기존 internal `Chapter1ExpeditionAuthoringBuilder.AcceptBasicMovementReviewForAuthoring(id,expectedPrefabSha,evidenceFile,expectedEvidenceSha)`를 호출한다. `evidenceFile`은 해당 방의 실제 Luna 독립 보고 일반 파일이며 기존 evidence 루트 안·재분석 지점 아님·정확 보고SHA다. Builder49–63의 기존 검사/Save/finally와 RoomInstance42–46의 비활성·미수용·구조 검사·한 번 기록을 그대로 유지한다. reflection·새 friend·직렬화된 수용 필드 직접 대입0이다.

ORDER의 기존 Sol 자산 쓰기 소유를 Root가 누르는 GUI 명령으로 바꾸려면 Astra가 정확 `R01.prefab~R06.prefab` 6파일의 한정 수용 기록 소유를 명시 Sol→Root 인계해야 한다. 다른 product writer0. 전체6을 먼저 사전 대조하고 방당 API1회·첫 실패 뒤 남은 호출0이다. 여섯 Save를 원자 저장으로 주장하지 않으며 부분 기록/오류를 보존하고 rollback/전체 재호출0이다. 초안은 이 소유 전이를 실행하지 않는다.

**REQ-C1B-G3OBS-009:** 적용 때 허용한 delta는 여섯 방 prefab의 기존 수용 필드뿐이다. 각 실제 전후 SHA·reportSHA·meta 불변·나머지 자산/소스 불변·실제 호출/종료를 기록한다. 두 번째 방부터는 앞서 실제 적용·봉인한 prefab만 승인 delta로 구별하고 미적용 후보/모든 meta/Runtime/Editor 입력은 관측 당시 원문과 같아야 한다. 전체 원문이 여전히 같은 aggregate라고 주장하지 않는다. Luna 사후 독립 대조·Astra 한정 판정과 G4 현재 source/test/asset/meta/asmdef/발견ID 재결속 뒤에만 집중35를 시작한다.

## 8. 검사와 REQ/AC

**REQ-C1B-G3OBS-010:** 추가 source의 영향은 Movement.Unity/Input.Unity/Combat.Authoring.Editor 세 조립체가 최소 후보이며 정확 소비자·참조 재사용/검사 선택은 새 영향 원장과 Luna 독립 검토로 확정한다. 현재 r7 Editor1×60 또는 역사 r6의5를 새 getter/observer 검사로 사용하지 않는다. 새 Runtime 수집기가 포함된 Editor 조건 컴파일뿐 아니라 `UNITY_EDITOR` 없는 Player 조건에서 타입/Start/FixedUpdate/IO/설정 필드가 전부 제외되고 Runtime→Editor 참조가 없음을 독립 대조한다. 새 수집기 MonoScript GUID의 실제 prefab/저장 scene 참조0 및 빌드 대상 MainScene/원래6 prefab에 임시 수집기가 저장되지 않았음을 검사한다. 기존 G8의 단1회 실제 standalone 빌드에서 포함 타입/참조 누락·임시 구성 요소의 유출 없음을 새 입력으로 확인하며 이 조건을 이유로 예비 Player 빌드를 추가하지 않는다. Player 조건의 최소 컴파일 필요 여부/정확 입력/예산은 새 영향 검수와 별도 한정 승인에 남기고 현재 성공을 발급하지 않는다. 컴파일·GUI의 실제 횟수/상한·정확 입력·도구/결과는 각 별도 실행 승인 뒤다. 새 시험 선택자·기존35 변경0이며 G3 원시를 자동시험 성공으로 대체하지 않는다.

| 제안 AC | 연결 REQ·필수 검수 |
| --- | --- |
| AC-C1B-G3OBS-001 | REQ001: 실제 r3 생성/QA/소유 반환 원장 출발·독립 규범 승인·정확7 후보·기존3 delta/신규4·기존 권위 종료 뒤 승인 전이. |
| AC-C1B-G3OBS-002 | REQ002: Input reader의 실제 저장 필드/발행 출처 및 호출 그래프 대조; 반환 전후 원래 receipt/Health 객체·revision 동일, 거절 시 runtime 쓰기/새 객체 발행0. |
| AC-C1B-G3OBS-003 | REQ003: null motor에서 false만 반환, EnsureMotor/물리 설정/진행 도달 불가; 기존 모터의 저장 snapshot/next만 읽음. |
| AC-C1B-G3OBS-004 | REQ004: source/host/graph/키/세대/receipt 및 Health/kill/crush의 동일tick·원본 ref/전후 동일성. 누락 기본false/가짜 completed0. |
| AC-C1B-G3OBS-005 | REQ005: 원시 스키마·실제 기록 능력·CreateNew·상한·실패/종료/null·원문 해시·표본 한계와 완주 후 live=false 보존. |
| AC-C1B-G3OBS-006 | REQ006: 정확 Root 단일 GUI/PID/window·실제 도구 심사·명시 새 예산·종료/회수, 모델/실제 역할, 기존 소진 이력 보존. |
| AC-C1B-G3OBS-007 | REQ007: 실제10/각1·필수 경로/지지/출구·전체 위험/금지입력 판독·첫 실패. HP/host 자동 실패 가정0·Review/본편 구분·ValidateRoom 경로 정확 설명. |
| AC-C1B-G3OBS-008 | REQ008: 방별 A/B 원시→실제 Luna→Astra→정확6 소유 인계→기존 API 직접 호출·검수 전 메타 쓰기0. |
| AC-C1B-G3OBS-009 | REQ009: 허용6 수용 delta/meta·다른 입력 보존·부분 실패 원시·사후 독립 판정·G4 입력 재결속. |
| AC-C1B-G3OBS-010 | REQ010: 새 영향/검사/도구/결과의 한정 승인·독립 대조·Editor/Player 조건 및 standalone 참조/직렬화 유출0, 과거 결과의 허위 재사용0·기존 검사 보존. |
| AC-C1B-G3OBS-011 | REQ011: Hazard 기존 슬롯·Owns/token/issuer/router/key/generation/receipt/revision/kind 결합·위험true 원본 반환·false null·무쓰기/무발행/재계산0. |
| AC-C1B-G3OBS-012 | REQ012: Runtime 폴더의 전체 UNITY_EDITOR 타입1·Editor 창 분리·기존 assembly/friend 재사용·Play 전 임시 객체1/host 저작 참조·자기 callback/단일 직접 writer·실제200 이후/tick0/연속 원문/gap0·고정 표시·재로드 수명. 실제 부착/호출/누락 미확인 시 실패·예비Play/강제tick0. |

Luna는 getter3 본문/호출 그래프의 무쓰기·무생성·무발행 및 Runtime 위치/전체 조건부 타입·새 수집기 자신의 callback/Writer/Play 전 구성/표시 읽기의 범위·정상 호출/순서/일회 결속을 독립 검수하고 실제 같은 완료 tick의 반복 읽기 원시를 대조한다. getter가 호출한 생산 Health 발행처럼 보이게 원시를 만들지 않는다. pause/실제 scene 종료/누락 표본도 표시·원시의 실패 의미를 검수한다. 이 초안의 AC는 모두 미실행·미수용이다.

기존 REQ-C1B-ROOM-001/EVIDENCE-001/ORDER-001, REQ-ROOM-005/007, REQ-CH1-009/010 및 대응 AC를 유지한다. G3→집중35 단1회900초→회귀44 두 모드 각1회 합산1800초→별도 snapshot4×3 수동12/회당1800초→종단1+3 각1800초/합산7200초와 별도 실제 창 키보드/포인터 각1800초→현재 소스 빌드1회900초를 축소·증액·중복 집계하지 않는다. 필수9회·Q0 인간 승인·키/세대·프로필·원격 관문과 B1 실제 독립 FINAL 전 C1C 공유 통합0을 보존한다.

## 9. 승인자가 결정할 정책과 종료

1. **PD-G3-001:** 저장값 reader3+Runtime 조건부 수집기 C#/meta2+Editor 창 C#/meta2 정확7 범위를 채택할지, 세 조립체 후보 및 Player 조건 제외/standalone 참조 검사까지 정확 영향·컴파일 예산을 어떻게 한정할지. r3 실제586 원장/QA/소유 반환 뒤 새 규범·입력을 결속하며 현재568/미발급586/미발급590을 동일 현재로 쓰지 않는다.
2. **PD-G3-002:** Root 단일 GUI 시작1·Play10·전체18000초/각1800초·위 기록 크기 상한 및 기존 Sol→Root 정확6 수용 쓰기 소유 전이를 채택할지. native 연속 증거 능력·실제 창 인증은 실행 직전 반드시 관측할 사실이며 정책 승인만으로 통과하지 않는다.

3. **PD-G3-003:** §4의 Play 전 bootstrap/serialized 관측 스칼라·저작 host 참조1, 수집기 자신의 Start/FixedUpdate210/정리 callback·단일 직접 writer/고정 표시, 창의 update1/playModeStateChanged1 수명 관측을 명시 승인할지. 기존 Runtime callback/PlayerLoop/게임 상태 필드 변경0이다. 실제 부착·재로드 뒤 최초tick0·200 이후 매tick·기록 능력은 미확인이고 최초 R01-A1 안에서만 확인한다. 불가능하면 첫 실패 원시와 새 원인/정확 보정부터 판정하며 예비 Play나 다른 입구로 보충0이다.

이 세 결정을 포함한 독립 규범 검수·새 Astra Approved가 필요하다. Astra의 병렬 배치 조사 역시 같은 정확7/Play 전 구성 권고이며 새 counterdesign의 최종 지문·판정은 아직 이 초안에서 발급하지 않는다. 현재 r7/r3를 무효화하는 master/기존 승인 원문 수정은 이 초안에서 하지 않는다. 승인 전이가 기존 핀을 바꿔야 한다면 해당 실행·쓰기 종료 뒤 Root에 영향과 새 결속을 먼저 인계한다. 작성 종료 시 실제 제품·프로필·Unity/native·GUI·플레이·수용0, 신규 초안1이며 소유를 Root에 반환한다.


## 10. 아스트라 정책 판정·R3 한정 수용·정확 구현 승인 — Approved

### 10.1 실제 종료와 현재 기준

R3 원시 실행·소스 독립 검수 `C8DEE976F72616BD103897E8009B47202836C8C123071209A9561DFF6369F985`, 별도 후보 직렬화 구조 독립 검수 `9ADE6B8C7124D1FBF5B0B95F78FC3C5708BBB743B10AEFE4035B44DF27A3C678` 및 Root 실제 도구 관측·소유 반환 `AEE7E010A7AE028967EBF0ADFD9216E12C902188C0969819DD43DFB6A376D2D1`에 결속한다. 아스트라 R3 한정 수용（`../../../output/chapter1-playable-r1/preparation/b1-unity-import-authoring-r3-limited-acceptance-astra.v1.json`；로컬 보존 자료） SHA `C02BB01C6E58708E519D8CF3595218A1459B2AFA510714F8850A80D96E3F4DF0`은 원시8 및 현재586 전수 길이/SHA 직접 대조·불일치0을 기록했다.

실제 Root `require_escalated` 호출 세션22335·최종 도구 exit0, native 정수exit0·22.2815407초·handle 확보·관측 실행 프로세스 폐쇄·관리오류0이다. 종료0.4816678초/회수0.0263982초는 각각5초 이내다. 별도 자동심사 결정 문자열은 노출되지 않아 생성하지 않는다. 실제 원래568 변경/삭제0·정확 자산9/meta9 추가·현재586을 수용한다. `Build` 완료 문자열은 직접 관측하지 않았으며 코드 순서·정확 산출물·정상 종료에 근거한 추론으로 남긴다. helper의 `structureValidated=false`와 compiler/executeMethod 확인 null을 수정하지 않는다. 이는 도구의 독립 판정 미발급 표시이며 별도 실제 원시·YAML/meta 검수를 무효화하는 필수 true 가드가 아니다.

후속 현재 소스 기준은 R3 source-after（`../../../output/chapter1-playable-r1/evidence/unity-b1-import-authoring-context-r3-source-after.json`；로컬 보존 자료） 179017바이트/SHA `532DB8F433AF9779022FDEFF5FBA1A0476CCDCA90FD660026DD223B5C72A8D94`의 **실제586 경로/길이/SHA 행 집합**이다. 내부 `checkpointSha256=C1D5...`는 이전568 계보이며 새 현재 기준이 아니다. 원문 `ASL=10CF2D4BD5DB53499A22698721CA9E4293EA644733BCB60DF41DEA807C410E34`와 당시 master `EFA3020DB138D3434C6660A8092418B34BA76C40D6B7D9C4F6680CE5AF8258EB`는 R3 실행 입력으로 보존한다. 이 후속 승인으로 종료된 R3를 반복하거나 당시 핀을 재작성하지 않는다.

G1 가져오기·컴파일과 G2 후보 소스·직렬화 구조만 수용했다. 여섯 방의 이동 수용 필드는 비어 있으며 실제 이동·Play·시험·현재 빌드·B1 FINAL·CH1 완주는 미수용이다. G3와 이후 기존 관문을 면제하지 않는다.

### 10.2 세 정책의 명시 결정

**PD-G3-001 채택:** §2의 기존 reader3 한 메서드씩과 신규 조건부 수집기 C#/meta2·Editor 창 C#/meta2 **정확7파일**을 승인한다. §3~4의 순수 reader, 원본 Health/kill/crush, 저장값·같은 실제 tick/참조 전후 대조, 무발행·무게임쓰기·무생성 및 §8의 Player 조건 제외/기존 standalone 참조·직렬화 유출 검사 조건을 적용한다. 기존 method 본문·asmdef/friend·게임 callback·프로필·Builder/validator·원본 자산/meta 변경0이다. Runtime 관측 타입 전체는 `UNITY_EDITOR` 안이며 `UnityEditor` 역참조0이다.

솔 구현 계획 `preparation/b1-basic-movement-implementation-plan-sol61.v1.json` SHA `4A24419D4FA4BA2D4EC7EE952CD77BCB83D04F410E1A7525910226A6F87EB5D6`의 정확7·세 정책 범위를 아스트라가 반대 대조하여 제품/파일/API/예비 실행의 추가 확대를 찾지 못했다. 이는 코드의 독립 수용이 아니다. 새 영향·검사 계획 준비를 허용하되 Editor 최소3×60초·Player 조건 최소2×60초 제안은 **실행 권한이 아니다**. 구체 입력·영향·독립 검수·도구·횟수/시간을 결속한 별도 실행 승인 뒤에만 실행한다. 역사 r7/r6 결과로 새 코드를 수용하지 않는다. 실제 G8 빌드1회 외 예비 Player 빌드0이다.

기존 Input.Unity의 Profile assembly 참조와 기존 메모리 가드는 그대로다. 새 관측 흐름은 프로필 파일 IO·새 프로필 검사 호출을 추가하지 않는다. 기존 메모리 guard를 프로필 파일 읽기로 잘못 분류하여 삭제하거나 참조 조립체를 바꾸지 않는다. 이 판정은 기존 금지 범위의 의미를 명료화하며 새 제품 경로를 승인하지 않는다.

**PD-G3-002 채택:** Root 단일 가시 Editor 시작1·Play 정확10·각1800초·전체18000초 및 표본108001/JSONL128MiB/사건8MiB/native원시2GiB, 고정 식별표32/최신표시1/첫실패1 상한 정책을 채택한다. 시작·import·실제 도구능력 확인·대기·관측·적용이 같은 GUI에서 이뤄지면 모두 전체 상한에 포함한다. 종료5/회수5·첫 실패 후 나머지0·자동 반복0이다. **이번 native/GUI/Play 실행 권한0**이며 실제 새 코드 검수·도구·능력·창 인증·입력·연속 원시·예산 인계 뒤 별도 승인한다.

방별 실제 독립 원시가 모두 수용된 뒤 기존 API로 기록하는 정확 R01~R06 prefab6의 Sol→Root 소유 전이 **정책만** 채택한다. 현재 prefab6 쓰기 소유는 이전하지 않는다. 실제 Luna 방별 보고·Astra 방별 수용·정확6 입력 및 단일 인계가 모두 존재한 뒤 별도 권한을 발급한다. 부분 적용 실패를 보존하고 rollback/전체 재호출0이다.

**PD-G3-003 채택:** §4.3의 Play 전 별도 임시 root1/수집기1·관측 스칼라와 실제 저작 host 참조1, 새 수집기 자신의 Start·FixedUpdate210·일회 정리와 단일 직접 writer·고정 표시, 창 update1/playModeStateChanged1만 승인한다. 기존 Runtime 생산 callback·PlayerLoop·게임 상태와 소유 권위 변경0이다. 실제 부착·재로드 이후 최초tick0·Lifetime200 이후 모든tick·수집/flush·native 입력 전달/연속 원시 능력은 여전히 미관측이다. 최초 R01-A 한 Play 안에서만 확인하며 누락/실패를 별도 예비 Play·강제tick·가상 입력·다른 실행 입구로 보충하지 않는다.

### 10.3 소유·검사·실제 종료

REQ/AC-C1B-G3OBS-001~012를 위 범위로 채택한다. 전체 규범 독립 검수의 근거는 실제 루나 `gpt-6-luna/max` v1 `A5C98C1F0760307E10F184CFF0796CEF9A76490F846D9D4CDF6EEDB3E93D8FCC`의 P0/P1=0/0이다. 추가 독립 규범 검수 v2 SHA `C4CA211C1AD811684D97944ECA13A830306E7E5BF525169DB372C21FD78499E7`도 같은 F242 v3와 실제 `gpt-6-luna/max`·정수 P0/P1=0/0을 확인했다. 두 보고를 원문대로 보존하며 기존 메모리 프로필 가드/참조 유지와 새 관측 프로필 파일/API 접근0의 같은 경계를 함께 결속한다. 각 보고의 실제 검수 범위를 소급 확대하지 않고 임의 검수 문서를 새 일반 선행으로 늘리지 않는다.

Root가 현재586과 본 Approved 지문·실제 구현 담당을 결속한 정확7파일 독점 소유 인계를 작성한 뒤 솔이 구현한다. 기존583 불변·기존3 한정 delta·신규4를 실제 새 원장으로 기록하고 쓰기를 닫는다. `590`은 그 변경이 실제 완료된 경우의 산술 결과이며 현재 실제 파일 수로 기록하지 않는다. 루나의 독립 코드/영향/델타 검수와 아스트라 소스 한정 판정 뒤 별도 최소 컴파일·GUI 준비로 잇는다. 구현자가 자기 구현을 독립 수용하지 않는다.

필수 순서는 G3 실제6방/10구성과 원시·방별 독립 수용→정확6 기록→G4 현재 입력 재결속과 집중35 단1회900초→G5 회귀44 두 모드 각1회 합산1800초→G6 별도 수동12→G7 정상1/실패3 각1800초·합산7200초 및 별도 실제 창 키보드/포인터→G8 현재 소스 빌드1회900초다. 필수9회·Q0 인간 승인·키/세대·프로필·원격 및 B1 실제 독립 FINAL 전 C1C 공유 통합0을 보존한다.

본 승인 작성자의 제품·스테이징·프로필 IO·native/Unity·GUI·Play·시험·빌드·수용기록 실행0이다. 승인 산출물은 새 계약·주 계약 후속 절·현재 작업판 갱신·새 승인 원장에 한정한다. GPT 참여 원장 쓰기는 Root 소유다. 소유를 Root에 반환하고 다음 실제 전달물인 정확7 소스 구현으로 진행한다.
