# C4 검증 증거 규약 r1 제안

- 상태: ContractDraft. 구현·Unity 실행·수용 권한 없음.
- 작성: 2026-09-29, 테라 구현 역할, 실제 `gpt-6-sol`.
- 추적: `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-001..010`, 공동 `AC-M5D7QC3-007/008`.
- 규범 소유 후보: `docs/specs/work-contracts/2026-09-29-c4-r3-exact-implementation-amendment.md`. 이 제안은 승인된 계약을 대신하지 않는다. 아스트라 기술 선택과 루나 독립 검수 뒤 별도 규범 보정으로 옮긴다.

## 공통 직렬화 규칙

아래 표의 키는 정확하며 추가·누락 키, 잘못된 형식, 중복 JSON 키를 거절한다. 모든 객체는 `SchemaVersion` 정수 1과 `Trace` 문자열 배열을 가진다. 경로는 작업루트 상대 `/` 형식, 해시는 대문자 64자리 SHA-256, 시각은 UTC ISO-8601 문자열이다. 실행 경로만 정규화된 절대 경로를 사용한다. 배열 순서는 의미를 가진다. 진단의 불확실성은 `Unknown` 또는 null이며 false·0·성공으로 바꾸지 않는다. 실제 값이 없는 이 초안은 새 개수·해시·실행 사실을 발급하지 않는다.

`FileBinding`은 `{Path:string,Sha256:string}`이다. `CaseBinding`은 `{FullName:string,Order:int}`이며 Order는 0부터 연속이다. `SourceBinding`은 `{Path:string,Sha256:string,Kind:string,ChangeAuthority:string}`이고 Kind는 Runtime/Test/Meta/Audit/Tool/Evidence 중 하나다. 최종 권한 상태는 모든 실행 비교 객체에서 `WholeAccepted:false`이며 이 값은 도구가 true로 변경할 수 없다.

## 선택과 엄격 파서

선택 객체의 키는 `SchemaVersion,Trace,Platform,SelectionKind,ExpectedCount,ExpectedQualifiedNames,Selector,SourceFiles,ParserVersion,ParserEvidence,WholeAccepted`다. Platform은 EditMode/PlayMode, SelectionKind는 FocusedEdit/FocusedPlay/FocusedHub/PredecessorPart, 이름은 중복 없는 문자열 배열, SourceFiles는 FileBinding 배열이다. ParserEvidence는 `{DeclaredAttributeCount:int,ParsedAttributeCount:int,UnsupportedDeclarations:string[],EnumSources:FileBinding[]}`다. 개수는 최종 source를 실제 파싱한 뒤만 발급한다. 선언 개수 불일치와 UnsupportedDeclarations 비어 있지 않음은 실패다.

기존 C3 생성기의 쉼표 Split 및 단순 문자열 정규식을 복제하지 않는다. 최종 Test/TestCase/UnityTest 선언과 정확 namespace/class/method를 읽고 bool/int/string/enum만 지원한다. 문자열·이스케이프·인자 구분을 먼저 해석하고, 선언된 enum의 실제 이름/값 결속을 검증한 뒤 실제 NUnit fullname을 구성한다. 표현식, unknown enum, 지원하지 않는 named argument/attribute/source는 누락시키지 않고 Unsupported로 실패한다. source 파싱과 XML의 실제 fullname은 실행 뒤 정확 비교한다. 예상 이름의 NUnit 표기는 승인 전 미결정이며 실제 관측 없이 추측을 확정하지 않는다.

fullname 구성 뒤 regex escape와 양끝 anchor를 적용하고 마지막에 Windows 명령행 인코딩을 한다. 문자열 quote/backslash, 괄호·쉼표, enum의 표기를 서로 다른 단계에서 처리한다. 전체 Unity 실행 명령행 및 숨김 하위 pwsh 명령행이 32767 UTF-16 한도 이내인지 실행 전 검사한다. 초과하면 실행하지 않는다. 사전 분할은 기존 순서·서로소 합집합·전체 names·행의 ExpectedCase·180초를 보존한 고정 Parts로 계획하고 아스트라/루나가 동결한다. 실행 중 동적 분할은 금지한다.

## 동결 소스와 입력 목록

소스 원장 키는 `SchemaVersion,Trace,Files,Count,PredecessorManifest,AllowedChanges,WholeAccepted`다. Files는 SourceBinding 배열이고 Count는 실제 길이다. PredecessorManifest는 불변 R11 FileBinding이며 AllowedChanges는 `{Path:string,RequirementIds:string[],Reason:string}` 배열이다. 원장 자신의 해시는 내부 Files에 넣지 않는다.

입력 목록 키는 `SchemaVersion,Trace,Paths,Count,PredecessorCapture,ReasonMap,ExcludedOutputPatterns,WholeAccepted`다. Paths는 정렬된 중복 없는 경로 배열이고 Count는 실제 길이다. PredecessorCapture는 R11 입력 포착 FileBinding이다. ReasonMap은 `{Path:string,Disposition:string,Reason:string}` 배열이며 Disposition은 Retained/Added/Removed다. 기존 884에서의 모든 추가·제외를 경로별 설명하고 기존 Assets/Packages/ProjectSettings/qa 포착 범위를 숨겨 축소하지 않는다. 직접 읽는 문서·자원과 새 도구·manifest·selection·checkpoint를 포함한다.

목록 자신, queue plan/result, XML/log, before/after capture, native/QA/verification/row comparison, 실행 후 검수·수용 문서는 입력에서 제외한다. capture 키는 `SchemaVersion,Trace,InputListBinding,CapturedAtUtc,Count,Files`이고 Files는 같은 고정 Paths의 FileBinding 배열이다. 자기/출력/재귀 hashing을 금지한다. 같은 Paths의 before/after 및 실행 간 hash를 대조한다. 884 또는 fixed14를 C4 현재 개수로 하드코딩하지 않는다.

## 실행 계획과 선행 선택 지도

계획 키는 `SchemaVersion,Trace,ProjectPath,UnityPath,SourceManifest,InputPathList,CheckpointRows,Tools,PredecessorSelectionMap,Runs,ProcessLock,FailurePolicy,WholeAccepted`다. 결속 자원은 FileBinding이며 Tools에는 새 7개와 변경하지 않은 Invoke-UnityQa가 모두 들어간다. ProcessLock은 `{Scope:"SingleProject",ProjectPath:string,Exclusive:true}`이고 FailurePolicy는 `"FailStop"`이다. 계획 자신의 SHA는 실행 시작 인자로 별도 전달하고 시작·각 경계에서 검사한다. 입력 목록에 계획을 넣지 않는다.

Runs의 각 객체는 `{Stem:string,NativeWitnessRunStem:string,Platform:string,Selection:FileBinding,Selector:string,ExpectedCount:int,ExpectedQualifiedNames:string[],RequiredRowIds:string[],ObservationSeconds:int,CaseTimeoutSeconds:180,CommandLineLength:int,OutputPaths:object,PartId:string}`다. NativeWitnessRunStem은 Stem과 정확히 같다. OutputPaths의 정확 키는 Xml/Log/Before/After/NativeExit/QaReturn/Verification/RequiredRowComparison이며 계약의 c4 stem 출력 경로만 사용한다. 실행 전에 모든 출력의 부재·승인 stem·명령행 한도·결속 SHA를 검사한다. Runs/Parts는 실행 전 불변이다.

선행 지도 키는 `SchemaVersion,Trace,Selections,WholeAccepted`다. Selections는 `{Id:string,SourceSelection:FileBinding,ExpectedCount:int,ExpectedQualifiedNames:string[],Parts:object[]}` 배열이다. Id는 Edit240/Play15/Edit562/Worker51/Play610이다. Parts는 `{Id:string,OrderedNames:string[],Selection:FileBinding}`다. Edit240의 Matrix91/Remaining149 서로소 순서를 보존하고 기존 전체 names를 변경하지 않는다. 기존 내부 377행 원장과 검증 프로그램은 보존하며 해당 정확 원본을 새 소스 실행에서도 별도로 대조한다. 새 C4 행이 기존 행을 대체하지 않는다.

## 실제 편집기 종료 관찰

기존 `qa/tools/Invoke-UnityQa.ps1` 60~100행의 Windows 인코딩/역파싱, 101~144행의 선언 경로 검사 및 `Test-OwnedEditorSnapshot`, 145~175행의 완전 inventory/후보/creation/attach가 직접 근거다. 실제 함수명은 `ConvertFrom-WindowsArgumentLine`, `Read-UnityQaArguments`, `Assert-DeclaredTestRunArgumentPaths`, `Get-OwnedEditorCandidates`, `Test-CreationTimeMatches`, `Attach-OwnedEditor`다. QA의 218행은 실제 Process.ExitCode를 읽지만 263~264행의 성공 출력은 numeric native 값 없이 QA 결과만 반환한다. QA 반환값을 native 증거로 복제하지 않는다.

새 runner는 숨김 하위 pwsh로 변경하지 않은 QA를 정확 한 번 실행한다. UseShellExecute=false/CreateNoWindow=true와 ArgumentList 또는 Start-Process -WindowStyle Hidden을 사용한다. 이 하위 실행과 병행하여 읽기 전용 inventory를 관찰한다. 정확 Unity binary/project/results/log 네 경로 일치, runTests 1개, assetWorker 0개, 선언 경로 각각 1개, 완전 inventory, 후보 정확 1개를 요구한다. 절대 경로·대소문자 무시 정규화와 quote 파싱은 위 원본 규칙을 보존한다. filter/platform은 계획/QA 인자와 결속하며 소유권 네 경로 검사를 대체하지 않는다.

PID+creation UTC와 native Process object/handle을 종료 전에 보관하고 StartTime을 CIM creation과 마이크로초 정규화하여 정확 대조한다. PID 재사용, fast exit 전에 attach 실패, partial inventory, 후보 없음/복수, 실제 종료값 미관측은 NativeEvidenceUnavailable로 실패다. 추가 Editor 실행·임의 kill·launcher 종료값 대체는 금지한다. 프로세스 핸들은 관찰 종료 finally에서만 해제한다. 관찰 창 만료 때 살아 있는 프로세스를 죽이거나 다음 실행을 시작하지 않는다.

native 객체 키는 `SchemaVersion,Trace,RunStem,PlanSha256,Status,ProcessId,CreationUtc,MatchedPaths,CommandLineSha256,RunTestsCount,AssetWorkerCount,AttachedBeforeExit,ActualExitCode,ObservedExitUtc,FailureReason,WholeAccepted`다. Status는 Observed/NativeEvidenceUnavailable이다. MatchedPaths는 `{UnityPath:string,ProjectPath:string,ResultsPath:string,LogPath:string}`다. 미확증 필드는 null이다. raw commandline·라이선스·환경 비밀은 저장/출력하지 않고 일치 필드와 SHA만 기록한다. Observed는 인증 attach와 실제 종료 관측을 모두 요구하고 ActualExitCode는 실제 정수다.

QA 반환 객체 키는 `SchemaVersion,Trace,RunStem,PlanSha256,QaToolBinding,QaChildProcessId,QaToolReturnedCode,OuterExitCode,AncillaryLauncherStatus,CompletedAtUtc,WholeAccepted`다. AncillaryLauncherStatus는 Unknown/ObservedZero/ObservedNonZero/StillRunning이며 부수 정보다. OuterExitCode는 하위 runner의 실제 종료값으로 큐가 관찰한다. QA/native/outer 세 값은 독립적이며 모두 0이어야 한다. runner 내부에서 아직 종료하지 않은 자기 종료값을 성공으로 추정하지 않는다. 외부 관찰 저장 시점/작성 소유자는 아래 미결정에 남긴다.

## 전체 행: fixture 발행과 검증 공유 형식

시험 출력은 TestContext.Out의 한 줄 JSON이며 최상위 키 `id`를 첫 번째로 쓴다. 정확 키는 `id,schemaVersion,requirementIds,acceptanceIds,expectedCase,status,actualEvidenceKind,checkpointFamily,checkpointName,checkpointValue,outcome,phase,applicability,reached,before,after,callCounts,authorityCorrelation,guard,barrier,history,cleanup,failureReason`다. status는 Planned/Reached/Passed/Failed/NotReached, actualEvidenceKind는 PureValidator/Structure/FullBridge/ActualCallback/ActualWorker다. 서로 다른 종류의 증거를 full bridge로 합산하지 않는다.

before/after는 `{snapshotSha256:string|null,facts:Fact[]}`이고 Fact는 `{name:string,value:string,certainty:string,evidenceReference:string|null}`다. certainty는 Observed/SourceEstablished/Unknown이다. 실제 참조를 문자열 주소나 정상 권한으로 발급하지 않는다. authorityCorrelation은 `{confirmed:string,owner:string,adapter:string,router:string,root:string,proof:string,c2Result:string,receipt:string,executionGeneration:long|null,memoryGeneration:long|null,nextEpoch:string,checked:bool}`이며 문자열 상관 값은 SameActual/Absent/Foreign/Unknown만 허용한다. checked=false가 정상 상관으로 처리되지 않는다.

callCounts는 `{c1Begin:int|null,c2Finalize:int|null,freshReserve:int|null,freshComplete:int|null,preparedObserver:int|null,c2Observer:int|null,nativeCallback:int|null}`다. guard는 `{adapter:string,router:string,permanentFaultRecorded:string}`이며 상태 값은 Entered/Closed/CompletedFresh/NotEntered/Unknown, fault 값은 Yes/No/Unknown이다. barrier는 `{state:string,ordinaryWriterBlocked:string,evidenceReference:string|null}`이며 state는 NoBarrier/DurableBarrier/Uncertain/Unknown, 차단은 Yes/No/Unknown이다. history는 `{originalCommitPreserved:string,originalEpochPreserved:string,oldTakePreserved:string,oldCursorPreserved:string,receiptReferencePreserved:string}`이며 Yes/No/Unknown이다. cleanup은 `{required:bool,releaseSignaled:string,workerLeaseReleased:string,workerJoined:string,originalThreadCleanup:string,exception:string|null}`이며 사실 값은 Yes/No/NotApplicable/Unknown이다. cleanup 필수인데 Unknown/No이면 실패다.

예상 행 객체 키는 `Id,Order,RequirementIds,AcceptanceIds,ExpectedCase,EvidenceKind,CheckpointFamily,CheckpointName,CheckpointValue,Outcome,Phase,Applicability,ExpectedReach,RequiredFacts,ExpectedCallCounts`다. Applicability는 Required/IntentionalNotApplicable이며 사유는 RequiredFacts에 포함한다. ExpectedReach는 RequiredReached/RequiredNotReached다. IntentionalNotApplicable은 의도적 미도달 기록을 요구하지만 통과 행 수에 포함하지 않는다. Required 행 미관측/미도달은 RequiredMissing이며 허용되지 않는다. RequiredNotReached는 예컨대 사전 차단 이후 C2가 호출되지 않았다는 검증 대상으로, 관측 사건과 0 call 증거가 필요하며 checkpoint 자체 도달로 세지 않는다.

원장 키는 `SchemaVersion,Trace,SourceFiles,EnumSources,Rows,Count,WholeAccepted`다. 최종 source의 정확 C4 throw 27개, C1 enum 1..51, C2 enum 1..19를 이름/값/선언 순서로 검증한다. 27+51+19 산술을 NUnit 개수나 실제 full row 개수로 발급하지 않는다. A 정지1/B observer2는 일반 throw와 다른 종류·호출 수·순서로 기록한다. outcome별 실제 도달과 의도적 미적용을 최종 fixture의 계획과 대조한다. 행 분할/내용/순서 변경은 재승인한다.

각 행은 Planned 1개 뒤 Reached→Passed 또는 Reached→Failed, 미도달은 NotReached로 종결한다. 중복/역순/미종결/예상 밖 ID/잘못된 키는 실패다. 출력의 expectedCase를 신뢰하지 않고 부모 XML test-case fullname과 예상 ExpectedCase를 대소문자 구분 정확 비교한다. 부모 result가 Passed가 아니면 내부 Passed는 수용 불가다. skipped/inconclusive·NUnit 실패를 내부 행으로 덮지 않는다. 반사 순수 음성, source 지배 증명, 실제 C1/C2 실행, 실제 canceled, 실제 worker 경합을 각각 명시한다.

## 검증 및 실행 비교 결과

행 비교 키는 `SchemaVersion,Trace,RunStem,ExpectedRowsBinding,XmlBinding,SourceMatched,PlannedCount,ReachedCount,PassedCount,FailedCount,NotReachedCount,IntentionalNotApplicableCount,RequiredMissingIds,UnexpectedIds,InvalidIds,Rows,EvidenceMatched,WholeAccepted`다. Rows는 `{Id:string,ExpectedCase:string,ActualCase:string|null,CaseResult:string|null,Matched:bool,Records:object[],Issues:string[]}`다. 행 증거 원문은 Records에 보존한다.

실행 검증 키는 `SchemaVersion,Trace,RunStem,PlanSha256,SelectionBinding,SourceManifestBinding,InputListBinding,XmlBinding,BeforeBinding,AfterBinding,NativeBinding,QaReturnBinding,RowComparisonBinding,Total,Passed,Failed,Skipped,Inconclusive,MissingNames,ExtraNames,DuplicateNames,ActualQualifiedNames,InputDifferences,FreezeDifferences,NativeExitCode,QaExitCode,OuterExitCode,Verified,WholeAccepted`다. 결과 counters는 정수, 차이 목록은 배열이다. XML counters와 실제 case 수 일치도 필수다. Verified는 full names/count/parent pass/행 대조/source freeze/동일 입력/세 실제 종료값0 모두 만족할 때만 true다. 검증기는 종료 관측 미정값을 0으로 변환하지 않는다.

큐 결과 키는 `SchemaVersion,Trace,PlanBinding,SourceManifestBinding,InputListBinding,Runs,SameInputs,ExecutionComparisonPassed,StoppedAtStem,FailureReason,WholeAccepted`다. Runs는 `{Stem:string,VerificationBinding:FileBinding,OuterExitCode:int|null,Completed:bool}` 배열이다. 선행 실패·중단·살아 있는 편집기·증거 부재 이후 다음 실행은 없다. 최종 비교는 독립 검수와 아스트라 수용을 대신하지 않는다.

## 승인 전 미결정

1. 실제 NUnit enum 및 이스케이프 문자열 fullname 표기를 확인할 최소 source/관측 대조 방식과 허용 attribute 문법. 아직 새 사례 개수 없음.
2. actual outer exit를 큐가 관측한 뒤 QA 반환 객체에 기록할 불변 작성 절차. 실행기가 자기 종료값을 미리 쓰는 방식은 금지하며 기존 파일 덮어쓰기도 금지한다.
3. source 담당자의 checkpoint/outcome별 ExpectedCase 및 전체 행 발행 계획과 위 필수 키 정렬. 실제 신규 API/행 수/실행 사실은 아직 없음.
4. 큐의 단일 프로젝트 배타 잠금 저장 매체. 생성 허용 범위 밖 잠금 파일을 임의로 추가하지 않으며 운영체제 named mutex 또는 승인된 경로를 확정해야 한다.

아스트라/루나 승인 전 이 제안의 스키마는 규범이 아니다. 기존 코드·도구·선택·증거는 변경하지 않는다.
