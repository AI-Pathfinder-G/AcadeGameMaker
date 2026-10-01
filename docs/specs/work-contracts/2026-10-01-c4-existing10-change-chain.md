# C4 기존 후보 변경 열 파일의 바이트 변경 사슬 복원 계약

- 상태: **Approved — 아래 열 파일의 읽기 전용 근거 대조와 테라의 새 증거 세 파일 작성만 승인한다.** 게시·클린 복제·Unity/QA/Git/원격 실행 권한 없음.
- 작성: 2026-10-01, 솔 역할 `gpt-6-sol`. 선행 [Approved 22파일 조회 계약](2026-10-01-c4-changed-file-evidence-closure.md) SHA-256 `FDBC8ECE8DA136CDA62E58E23CCAD8C3CABE8A197492A2BFB5ADBDB8A06E48E5`, 22파일 원장 (`artifacts/c4-changed-file-evidence-closure-v1.json`; 로컬 원본) SHA-256 `1B80A52913CFD1119F97BAA9B667A6DBE2773FBBB33A167E647785AAB0A655BB`, [루나 독립 검수](../../verification/2026-10-01-c4-changed-file-evidence-closure-luna-result-review.md) SHA-256 `9EA4709166F861D5F5D7E0B590345C835E1B54F6C145B77B1FE4342A00A93AD5`, [아스트라 제한 수용](../../approvals/2026-10-01-c4-changed-file-evidence-closure-limited-acceptance.md) SHA-256 `D9A02137DC82CF6DB454E39290DC5E80B4BE496388A572FA1C25ABC0ADE62278`. 위 수용은 정확 분류만 인정했고 22파일 변경 사슬 폐쇄는 0개다.
- 추적: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`. 파일별 실제 구현 REQ/AC는 원장 행 및 대응 Approved 제한 계약에서 다시 확인한다.

## 정확 대상과 기준 지문

부모 501행의 `정확 변경 승인 필요` 24개 가운데 v19와 겹친 22개 중 **`Origin=InitialCandidate489`인 변경 열 파일만** 조사한다. C4 신규 source/test6·meta6 및 범위 밖 C3 도구2를 포함하지 않는다. 아래 시작점은 원래 489 후보 SHA, 끝점은 22파일 원장과 C4 v19의 현재 SHA다. 중간 지문·변경 횟수·승인 시점을 이 표만으로 추정하지 않는다.

| 경로 | 원래 후보 SHA-256 | v19 현재 SHA-256 |
| --- | --- | --- |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` | `F47C61A043DAF4695CC53E41C6BE9BB67CA147C722F7B830459AA6304B45B3B5` | `D891961CF4E006F2E58B3516C7AFE1B5872B49CFA26DC5E7A83AFB35D458A3F6` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` | `393B325BA5B8F6D79CA983F1A91098215E7A6993DE37E9F5D98BA775760E4900` | `6B1D1FB6EDDAFD29ACD3A7A326D56E0B6AD98093ABE2DCBC9F1965BD634B6A59` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` | `CAEBD7B5DA99AF94A25C5C300789AA48CA87908CA6D8894B99AE49C2391D7043` | `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs` | `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB` | `4B157B98511A0978D19B25434F6D00E23C0CC6FB53BA8A14969182865C3BDC0E` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB` | `5A210D7ED80DE86C9D3092865E7F4712C00C65B84372E479018BF43956F6695F` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` | `8248404F3F237936954979F3FEC2848E9F419CF0AA2A781F732D5CA379FF9809` |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs` | `60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275` | `55B8C19F88B07C5888128C86BF05A743D747AB206E6D44942110ABE0AAFDD38B` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs` | `C27116AC35BF83D4AB8794FA9D3E379AD6F127B0E52B48C51399F49ADDF862D4` | `20F5E1C963B6F969649DDE58FA0B48338B2446049AB56949DE3BA60EB3812567` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs` | `647BEB1256A18EA90A3F73BB3D81B3219A248EBCCE8A7DBB9EA26D176CCB897A` | `A0A1928CE83A22D324F785476B38DCDE595B71E53122139F10535C5B3AF6F9D6` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs` | `2D3403E910E0DBA349854A194F7140DE65759190DF53920F2A989C6D0D07127C` | `2FA406515CEB7C885A5F41EC6438ACC85123714AB00302B520A58B36066C9E23` |

## 단계별 증거 규칙

이번 Approved 범위에서 기존 원장, C3 R11 이전 동결, C4 v1–v19 동결 source/입력/선택, 실제 Approved C3·C4 및 후속 amendment, 구현자 변경 증거, 루나 독립 검수, 현재 파일을 **읽기만** 한다. 각 파일에 대해 관측된 서로 다른 SHA를 시간 순서대로 나열하고 `시작 후보 SHA → 각 실제 변경 SHA → v19 SHA`의 연속성을 검증한다. 변경이 없던 단계는 같은 SHA로 기록하되 새 변경으로 세지 않는다. 특정 단계의 파일 바이트 또는 지문이 자료에 없으면 추정으로 건너뛰지 않고 미입증 간격으로 기록한다. 문서 작성 순서와 파일 변경 순서를 같다고 가정하지 않는다.

각 **실제 SHA 전이**에 해당 시점의 Approved 정확 파일 경로/허용 내용·문서 SHA/줄, 구현 산출의 정확 SHA·변경 범위, 별도 루나의 그 변경 바이트/행동 범위 검수, 다음 동결 원장의 파일별 SHA를 독립된 링크로 결속한다. 승인 전 구현이나 허용 파일 밖 수정, source만 허용한 계약의 test/meta 확장, 이전 R11 또는 C1/C2의 과거 수용 지문을 현재 C4 바이트로 소급 적용하는 일은 허용하지 않는다. 후속 계약이 감사·시험 지문만 바꿨으면 제품 runtime 변경 승인으로 확장하지 않는다. 정상/실패 실행의 지문과 결과를 구분하고 실패 증거를 성공으로 뒤집지 않는다.

파일당 `PreviousCandidateSha256`, `Transitions[{FromSha256,ToSha256,ObservedAtEvidence,ApprovedScope,ImplementationEvidence,IndependentReview,FrozenManifest,Requirements,Acceptance,Finding}]`, `CurrentSha256`, `UnresolvedIntervals[]`, `ChainClosed`를 남긴다. 전이는 정확한 출처 파일/줄·그 파일의 SHA와 당시 상태를 포함한다. 하나라도 승인·구현 바이트·루나 범위·동결 사이의 연결이 비면 `ChainClosed=false`다. 승인 문서가 파일 전체를 허용했는지 제한된 수정만 허용했는지 구분하고, 해당 수정이 실제 차이의 범위에 속하는지 확인한다. 현재 파일 SHA와 v19가 다르거나 원본 후보 SHA가 부모 원장과 다르면 조사 중단·아스트라 보고이며, 새 파일/조건을 임의 추가하지 않는다.

## 새 산출·독립 검수·한계

승인된 **테라 산출 세 경로**는 `artifacts/c4-existing10-change-chain-v1.json`, `artifacts/c4-existing10-change-chain-v1.md`, `docs/verification/2026-10-01-c4-existing10-change-chain-terra-report.md`뿐이다. 별도로 허용된 **루나 독립 출력 한 경로**는 `docs/verification/2026-10-01-c4-existing10-change-chain-luna-review.md`다. 이 Draft와 네 출력은 착수 전 모두 부재하고 v19 입력1084·source193 경로 밖임을 확인했다. 기존 501/22파일 원장과 source/QA/선택/증거의 원문은 수정하지 않는다. 테라는 열 파일 집합·끝점 SHA·고유 전이·미입증 간격·범위 밖 12+2의 보존을 검증 기록에 남긴다. 루나는 원본 바이트와 각 승인·구현·검수·동결 링크를 **독립 재조회**하고 P0/P1 및 열 행의 폐쇄/미폐쇄를 보고한다. 아스트라만 그 근거 사슬 분류의 한정 수용을 판단한다.

열 파일 사슬이 닫혀도 신규 meta6·source/test6 및 나머지 초기 게시 공백은 별도다. 이번 한정 계약은 기존 게시 허용 빈 집합을 바꾸지 않으며 클린 복제·Unity 실행·C3/C4 전체 수용을 승인하지 않는다. Git/원격·원본 소스/meta·QA·현재 v15 동결 입력/결과도 건드리지 않는다.


설계 Draft SHA-256 `6FB59AF732C0D26134AFA4BCE2CC0C79BE51AFB3F9715AAF8BA99E350BCE7B19`와 루나 독립 설계 검토 SHA-256 `2D6727240F4D3A0A5CDED593ADB5A4E760079642D92BEC61226D0F98706D589C`를 보존한다. 검토의 P0/P1은 0/0이다.
