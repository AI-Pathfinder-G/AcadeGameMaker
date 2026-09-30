# C4 기본 제어 경로의 호출 수 증거 해석

- 작성·기술 선택: 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-001/003/005/007`, `AC-M5D7QC4-001/003/005/007/009/010`.
- 근거: Approved [QA 증거 규약 r2](../specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md)와 [C4 r4](../specs/work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md). 형식은 기존 RequiredFacts의 Observed/SourceEstablished와 exact callCounts를 유지한다. 신규 런타임 관찰자·factory·친구 조립·손상 경계 추가를 승인하지 않는다.
- 상태: 기존 승인 형식 안의 한정 기술 해석을 구현 담당에게 배분했다. 독립 규범 대조와 최종 source 지배 검수는 진행 중이며 실제 실행·수용은 아니다. 독립 검수에서 추가 규범 개정이 필요한 것으로 판정하면 해당 분기는 중지하고 규범 검수부터 보정한다.

Owner의 정상3인자/기본 제어 경로를 다른6인자 실행으로 바꾸면 검증 대상이 달라진다. 원래 본문을 보존한다. 실제 정상 Validate를 통과한 same-execution CWT ResultRecord 및 실제 Disk/Memory/Receipt 원본 결속과 동결 source의 정확 단일 호출·미호출 분기 지배를 함께 확인한 경우에만 호출 수1 또는0을 SourceEstablished로 기록한다. 반환 객체나 스칼라 값 하나만으로 이 조건을 충족하지 않는다.

이 행의 `diagnostic.noOpCallCountEvidenceKind`는 정확 `FrozenSourceSingleCallAndActualTypedRecord`·SourceEstablished로 사전 고정한다. 실제 제어 계수기를 관측했다고 주장하지 않는다. 원장 원본 검증·실제 반환 사건 및 source 지배 중 하나라도 없으면 통과시킬 수 없다. 실제 제어를 관찰한6인자 시험의 계수기는 별도로 Observed로 기록한다. 정상 권한·원본 참조 상관의 Observed 검증을 이 정적 호출 수 증명으로 대체하지 않는다.

기본 제어의 prepared/C2 관찰자 호출도 실제 동결 source 지배 범위만 SourceEstablished로 구분한다. 관찰하지 않은 nativeCallback을0으로 발행하지 않는다. 그 관측이 해당 사례의 필수 검증 밖이면 exact null과 관찰자 부재의 정확 SourceEstablished 진단을 원장에 선언한다. 네이티브 사건 관측이 필수인 사례는 기존 승인된 실제 callback 관찰을 유지하며 null로 우회하지 않는다.

C1/C2 내부 예외로 반환 뒤 체크포인트에 도달하지 못한 것을 호출0으로 오기하지 않는다. 실제 내부 control 경계와 정상 반환 뒤 경계는 별도 관측이다. C2 내부19개 장애 경계는 기존 Approved Play 배치·friend로 관측하며 Edit 접근성을 확대하지 않는다. BeforeC2 자체 예외나 C2 호출 이전 원본 검증 거절은 실제 미호출로 구분한다. 이 기록은 새 호출 수나 시험 개수를 발급하지 않는다.

2026-09-30 추가 기술 해석: 실제6인자 제어를 사용하지만 해당 REQ/AC에 native 사건 관측이 필수가 아닌 행은 `diagnostic.nativeCallbackObservationScope=NoNativeObserverRequiredForThisControlRow`·SourceEstablished를 사전 고정하고 nativeCallback의 exact null을 사용한다. 실제 native 관찰자가 없으며 해당 사례의 승인 검증 범위에 native 사건이 없다는 소스·사례 대조가 필수다. 이는 NoOp 전용 부재 진단과 구분하고, native 관측 필수 사례에는 적용하지 않는다. 관측하지 않은 호출 수를0으로 만들거나 필수 실제 입력·수명주기 관측을 면제하지 않는다.

호출 수·writer 차단의 소스 증명에 실제 조회하는 변경 없는 `Assets/AcadeGameMaker/Runtime/Profile/ProfileAtomicSaveServiceV1.cs` SHA `80DDBF925C74A8A81DD3B3950F25B3843D76B81F62ED942F3F3585CFB52B3F87` 및 `ProfileNewGameResetServiceV1.cs` SHA `23D7248B1A603A89BC8FF0FFDBB24FA04E640A66018DDC5689F97EC80701FBE5`는 최종 SourceManifest.Files의 Runtime과 실제 InputPathList에 포함한다. 실제 조회하는 다른 장벽 소스도 동일하게 결속한다. 읽기 전용 증거 범위이며 해당 파일의 변경 권한을 추가하지 않는다.
