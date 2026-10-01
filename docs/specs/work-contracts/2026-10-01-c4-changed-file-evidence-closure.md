# C4 변경·신규 파일의 바이트 근거 사슬 폐쇄 계약

- 상태: **Approved — 아래 정확한 읽기 전용 조회와 테라의 새 증거 세 파일 작성만 승인한다. 게시·클린 복제·Unity 실행 권한 없음.** 기존 파일과 Git 상태를 변경하지 않는다.
- 작성: 2026-10-01, 솔 역할 `gpt-6-sol`. 승인·범위 충돌 판정은 아스트라, 구현 뒤 독립 검수는 루나가 소유한다.
- 선행: [Approved 최초 바이트 근거 폐쇄 계약](2026-10-01-initial-code-publication-byte-closure.md) SHA-256 `98025CC092B945622D26C07334D46D96BD08473C12DE3CC0068577229CE13B7C`, 501경로 원장 (`artifacts/c4-initial-publication-byte-closure-v1.json`; 로컬 원본) SHA-256 `E43A4268ABC87A56AD4F4A198D96893135706EB1364ABB33D70BA27741765B17`, [루나 독립 검수](../../verification/2026-10-01-initial-code-publication-byte-closure-luna-result-review.md) SHA-256 `9609BCBA56D404D1DBA74890F68CA5D29943ADC0382C713D5B3B9A52F77E2840`, [아스트라 제한 수용](../../approvals/2026-10-01-initial-code-publication-byte-closure-limited-acceptance.md) SHA-256 `F5FB6874B52894F587D884BC7C6AF31459079499AA0142DE6DAEA8B23B73E4B2`. 수용 범위는 원장의 분류·바이트 대조뿐이고 정확 게시 허용 목록은 빈 집합이다.
- 추적: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`. 각 파일의 실제 구현 요구·AC는 원장의 `RequirementIds/AcceptanceIds`와 해당 Approved 허용 목록에 다시 결속한다. 출처 조사는 제품 구현 또는 공동 최종 수용을 대신하지 않는다.

## 정확 범위와 착수 전제

원장 501행 중 `ByteEvidenceState="정확 변경 승인 필요"` 24행에서 **C4 v19 Assets에 결속된 22행만** 고른다. 이는 초기 후보489와 겹치는 변경 source/test 10행 및 `C4AdditionalCandidate12`의 source/test 6행·짝 meta 6행이다. 나머지 2행인 C3 행/예상 원장 생성 도구는 이번 범위 밖으로 그대로 남긴다. 원장 489/61/428과 501행의 `PublicationApproved=false`·게시 제외를 바꾸지 않는다. C4 v19 source 원장·입력의 실제 지문 및 선택된 22파일의 현 SHA가 달라졌거나 동일 경로가 두 번 나타나면 중지하고 아스트라에게 보고한다. 최종 v15 결과가 없거나 다른 source로 다시 동결되면 새 행을 소급 수용하지 않는다.

초기 후보의 변경 10경로는 다음과 같다.

| 경로 | 선행·현재 바이트 대조의 소유 범위 |
| --- | --- |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` | Q-B 후속·fresh 슬롯 |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` | Q-A presenter·cursor |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs` | C3 결정·C4 실행 조율 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs` | 실제 root/세대·가드 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs` | 입력 가드·종료 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs` | lower 발급·소비 이력 |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs` | 제한 C2 경계 |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs` | Q0 정확 후속 감사 |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs` | C3/C4 실제 편집 시험 |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs` | 실제 실행 시험·선행 역사 |

추가 후보 12경로는 아래 정확 source/test와 meta **두 파일씩**이다. meta는 source의 허용에서 자동 파생하지 않는다.

- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs`, `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs`, `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs`, `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs.meta`
- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs`, `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs.meta`
- `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs`, `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs.meta`
- `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs`, `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs.meta`

## 파일별 증거 판정

각 행에서 원장 `CurrentSha256/CandidateSha256/C4FrozenSha256`, 실제 현재 파일 SHA, 원래 Approved 정확 경로 또는 정확 파일명 허용 문맥·줄·문서 SHA와 적용 시점, 제한 후속 amendment, 구현자 증거, **별도 루나의 현 SHA·실제 변경 범위 대조**, 메타 GUID/소유 source/조립 asmdef 참조를 연결한다. 변경 10행은 이전 후보 바이트에서 현 바이트까지의 원본별 변경 사슬과 후속 Q0/Q-A/Q-B strict audit·기존 선택 보존을 따로 표시한다. 신규 source/test 6행은 실제 r4 허용 목록과 후속 보정 계약에, 신규 meta 6행은 정확 GUID·짝 source·최종 원장 및 별도 현 바이트 검수에 결속한다. 원장에 이미 적힌 `ApprovedExactPathMentions`와 `IndependentCurrentShaMentions`의 개수는 시작점일 뿐이다. 특히 meta 6행에 독립 현 SHA 표기가 없으면 그 공백을 명시하고 source의 검수를 대신 쓰지 않는다.

파일당 판정은 `정확 승인 경로 확인`, `현재 바이트 독립 검수 확인`, `변경 사슬 확인`, `GUID·조립 결속 확인` 네 조건을 각각 참/거짓/미입증으로 남긴다. 하나라도 미입증이면 그 파일의 근거 사슬은 열린 상태다. 허용 계약이 소스 경로만 명시했다면 meta를, 정적 검수가 지문만 명시했다면 실제 실행 수용을, 기존 C3 R11 지문이 일치했다면 변경된 C4 지문을 승인한 것으로 확장하지 않는다. 기존 원장 24행 상태·게시 제외를 이번 결과로 덮어쓰지 않는다.

## 산출·독립 검수·중단

테라 구현자에게는 읽기 전용 자료 조회와 새 `artifacts/c4-changed-file-evidence-closure-v1.json`, `artifacts/c4-changed-file-evidence-closure-v1.md`, `docs/verification/2026-10-01-c4-changed-file-evidence-closure-terra-report.md` 세 파일 작성만 허용한다. 루나의 독립 검토 기록은 구현자 산출물 제한과 별도로 새 `docs/verification/2026-10-01-c4-changed-file-evidence-closure-luna-result-review.md` 한 파일만 허용한다. 착수 전 이 검토 경로 역시 v19 동결 입력·소스 목록 밖이고 아직 존재하지 않음을 확인한다. 세 출력과 이 Draft 경로는 착수 전 존재하지 않았고 v19 입력 1084경로·source 원장 193경로에 없음을 확인했다. 출력은 원본 22행의 경로·SHA·출처/검수 문서 줄·상태·meta GUID·REQ/AC와 증거 공백, 제외 유지, 원본 원장·최종 동결 결속을 담는다. 원래 24 중 대상22·범위밖2, 변경10·신규12, 신규 source6·meta6, 고유 경로와 실제 SHA를 기계적으로 재계산한다.

루나는 22행 집합과 실제 파일, Approved 허용 목록의 정확성, 이전/현재 SHA 사슬, 독립 검수의 해당 파일·현 지문·범위, 여섯 meta GUID/조립 경계, 빠진 근거의 보존 및 기존 입력 불변을 독립 검토한다. P0/P1=0과 아스트라의 **근거 분류 한정 수용**이 있더라도 게시 허용·클린 복제·C3/C4 전체 수용은 별도 판정이다. 본 Approved 계약은 위 한정 원장 산출의 착수만 허용한다. 설계 원본 Draft SHA-256 `7760C95692662B543C8233E48DAD77382473AA6C7DE9CDDFD8EE1D52F9A9BA5E`와 루나 설계 검토 SHA-256 `73A80A38706CB8D18CB8CD081CB26FE1F74FAE7ADE63DBFC7BC0976259ADA80C`를 보존한다. 검토의 P1은 구현자 세 파일과 루나 별도 한 파일의 정확 경계를 이 본문에 명시해 해소했다. 기존 source, meta, 자산, 설정, QA/선택, 승인·동결·실행 증거, Git/원격을 수정하지 않고 Unity·컴파일·클린 복제를 실행하지 않는다.
