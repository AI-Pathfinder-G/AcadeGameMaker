# C4 신규 메타 여섯 파일의 정확 경로·현재 바이트 근거 계약

- 상태: **Approved — 아래 여섯 meta의 읽기 전용 근거 조회와 테라의 새 증거 세 파일 작성만 승인한다. 게시·클린 복제·Unity 실행 권한 없음.** 원본 source/meta·QA·Git·원격을 변경하지 않는다.
- 작성: 2026-10-01, 솔 역할 `gpt-6-sol`. 선행 원장 22파일 증거 (`artifacts/c4-changed-file-evidence-closure-v1.json`; 로컬 원본) SHA-256 `1B80A52913CFD1119F97BAA9B667A6DBE2773FBBB33A167E647785AAB0A655BB`, [테라 보고](../../verification/2026-10-01-c4-changed-file-evidence-closure-terra-report.md) SHA-256 `C95D08006C9FDF0EF59DFA848689DAA538CC40F72B68304AB3009C84510D9D8B`.
- 추적: `REQ-M5D7QC3-007`, `REQ-M5D7QC4-007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`; 실제 짝 source의 상세 요구·AC는 선행 22파일 원장과 Approved C4 r4 계약의 정확 행을 따른다.

## 문제와 역사 경계

22파일 원장의 `정확 변경 승인 필요` 대상은 기존 변경10·신규 source/test6·신규 meta6이다. 기존 source/test16의 정확 경로 허용과 현 SHA 검수는 확인됐지만, meta6은 `ApprovedExactPathEvidence=[]`, `IndependentCurrentByteReviewEvidence=[]`다. GUID·짝 source·asmdef 결속은 확인됐어도 **정확 meta 허용과 현 바이트 독립 검수는 아직 미입증**이다. 더구나 22파일 전체의 이전→현재 변경 사슬은 별도 미입증이므로 이번 meta6 조사만으로 전체 파일 폐쇄를 발급하지 않는다.

[Approved C4 r4](2026-09-29-c4-r4-exact-implementation-amendment.md)는 신규 runtime `ProfileResetExecutionBridgeV1.cs`와 `.cs.meta`를 같은 행에 명시한다(50행). 신규 시험 다섯 source는 정확 경로 표(143–147행)에 있고, 각 `.cs`의 같은 경로 `.cs.meta` 동반을 일반 규칙으로 허용한다(139행). 두 방식은 **허용 문장의 형태가 다르다**. 특히 다섯 시험 meta의 완전 경로를 원문이 각각 직접 적었다고 소급 주장하지 않는다. 기존 meta 불변 규칙도 신규 meta의 현재 SHA 승인으로 바꾸지 않는다. 후속 Approved 계약에 각 짝 source나 fixture 변경이 있더라도 meta의 별도 정확 경로·지문 수용으로 자동 확장하지 않는다.

## 이 Draft가 제안하는 정확 대상

다음 표의 여섯 경로만 향후 읽기 전용 증거 계약의 검사 대상으로 제안한다. 이 표가 Approved가 되더라도 **과거 r4 문서의 허용 표현을 고치거나 새 meta 파일 바이트를 게시 승인하지 않는다.** SHA는 v19와 선행 원장의 대조 기준이며 조사 때 현재 파일을 다시 해시해야 한다.

| 신규 meta의 완전 경로 | v19 SHA-256 | r4 허용의 정확 해석 |
| --- | --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs.meta` | `A993C65D74081EDE2481A9112AC0B243BB0F36DF38E516F93C4F0DDA014EC621` | runtime source와 같은 행의 `.cs.meta` 명시; 현재 바이트 별도 미검수 |
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs.meta` | `F3488A1D7C47844EBC13BD103D0FC79D5D863B357BBAA64C87ABACE122AC78D8` | 정확 source 표+동반 일반 규칙; 완전 meta 경로는 현재 제안에서만 적시 |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs.meta` | `2D495B07A08BD26897B0CA3C6CA9E21B25D98265FE440EBCEC51813C9906921F` | 정확 source 표+동반 일반 규칙; 완전 meta 경로는 현재 제안에서만 적시 |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs.meta` | `B5126D48799BB55D4D922ED438DAFF4A03F57858157C0A7A8FCC4A7518E2105B` | 정확 source 표+동반 일반 규칙; 완전 meta 경로는 현재 제안에서만 적시 |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs.meta` | `DA0918DAB600E3521ABBCE4228F33BB8AF5FFB164ECDB1E346B9067EB34CB978` | 정확 source 표+동반 일반 규칙; 완전 meta 경로는 현재 제안에서만 적시 |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs.meta` | `9192D1061BDD5D125D6EE06E6A8B8B2BC439B6DF39D1BBB04A65BAAA7CB237A3` | 정확 source 표+동반 일반 규칙; 완전 meta 경로는 현재 제안에서만 적시 |

## 승인 후보의 읽기 전용 조사·출력

이번 Approved 범위에서 위 여섯 파일과 짝 source, 실제 소유 asmdef, v19 원장·입력, Approved r4 및 후속 제한 계약, 22파일 원장만 읽는다. 각 행에서 실제 meta 바이트 SHA, 원장 SHA 일치, `guid` 문자열과 GUID 소유권, 짝 source 경로·SHA, 소유 asmdef 경로·SHA와 조립, 현재 후보 전체의 GUID 중복/참조를 확인한다. 메타의 다른 직렬화 내용도 원문 바이트·실제 필드와 대조하되 의미 없는 `guid` 일치만으로 전체 meta SHA를 승인하지 않는다. 후속 계약이 특정 source를 바꿨으면 해당 계약의 meta 허용 여부를 정확 경로·줄로 **별도** 판정한다. 경로·SHA·GUID·조립 중 하나가 달라지면 미입증으로 남기고 아스트라에 보고한다.

테라 구현자에게 허용한 새 산출은 `artifacts/c4-meta6-exact-byte-evidence-v1.json`, `artifacts/c4-meta6-exact-byte-evidence-v1.md`, `docs/verification/2026-10-01-c4-meta6-exact-byte-evidence-terra-report.md` 세 파일뿐이다. 루나의 별도 독립 검토 기록은 새 `docs/verification/2026-10-01-c4-meta6-exact-byte-evidence-luna-result-review.md` 한 파일만 허용하며, 착수 전 이 경로도 v19 동결 입력·소스 밖이고 존재하지 않음을 확인한다. 각 여섯 행에는 `Path/CurrentSha256/FrozenSha256/Guid/PairedSource/PairedSourceSha256/Asmdef/AsmdefSha256`, r4의 **명시 실행 코드·일반 시험 동반** 구분, 후속 계약·줄·SHA, 원본 22파일 행과의 대응, `ExactPathEvidence`와 `CurrentByteReviewEvidence`의 확인/미입증, `PublicationApproved=false`를 기록한다. 출력은 여섯 고유 경로·여섯 소스/메타 쌍·GUID 고유성·현재 지문·선행 원장/보고 및 v19 입력 불변을 재계산하고 다른 16행이나 이전 후보489를 수정하지 않는다. 이 Draft와 출력 세 경로는 착수 전 모두 부재했고 v19 입력 1084경로·소스193경로 밖임을 확인했다.

구현자가 작성한 JSON·보고는 자기 수용이 아니다. 루나는 **각 meta 실제 파일을 독립 재해시**하고 r4 문장의 두 허용 방식, 후속 Approved 범위, v19 SHA, GUID/짝 source/asmdef, 원본22행 및 출력 집합·중복을 별도로 검증해 P0/P1=0 또는 남은 공백을 보고한다. 아스트라는 그 뒤 정확 meta 여섯 경로의 근거 분류만 제한 판정할 수 있다. 독립 검수 없는 현재 SHA 승인, 소스 승인에서 meta 승인 추론, 원장 상태 소급 수정은 금지한다. 파일별 증거가 닫혀도 게시 허용 목록·클린 복제·Unity 검증·C3/C4 전체 수용은 별도 게이트이며 이번 계약 범위 밖이다.

설계 원본 Draft SHA-256 `94AF2606ABC3C96C0F3570DA55E9063782114A7ACB43DF900322BE0268A8724E`와 루나 설계 검토 SHA-256 `DEEEB0C9107140FA308088A6172062CEF43B07270EB0C74E493CAD51FF8CB9D8`를 보존한다. 검토 P1은 테라 세 산출과 루나 별도 한 기록의 정확 경계를 이 승인본에 명시해 해소했다. 선행 22파일 원장 루나 검토 SHA-256 `9EA4709166F861D5F5D7E0B590345C835E1B54F6C145B77B1FE4342A00A93AD5`와 아스트라 제한 수용 SHA-256 `D9A02137DC82CF6DB454E39290DC5E80B4BE496388A572FA1C25ABC0ADE62278`에 따라 게시 허용 목록은 계속 비어 있다.