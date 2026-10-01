# C4 신규 메타 여섯 파일의 독립 바이트 검수

- 승인 계약: `docs/specs/work-contracts/2026-10-01-c4-meta6-exact-byte-evidence.md`, SHA-256 `710CE568BA359D1BA329D1DDBA16B138CAB9F5E97EF1D7BAF62C4DBA338D3FBB`.
- 계약 승인: `docs/approvals/2026-10-01-c4-meta6-exact-byte-evidence-contract-approval.md`, SHA-256 `975084441F598EF1F7CDDADBFBEE603C4150AD72153905293AC42E9058D4AE4D`.
- 테라 JSON: `artifacts/c4-meta6-exact-byte-evidence-v1.json`, SHA-256 `524F34CA8745E443C4A4A61179D234200A4B5289EF3CC2686EA6A07231C09937`.
- 테라 설명: `artifacts/c4-meta6-exact-byte-evidence-v1.md`, SHA-256 `24D14B5CF3461C5A73B1711AED4111FDDDA5754F07D4BF09DD1DA6B6C40E4BA8`.
- 테라 보고: `docs/verification/2026-10-01-c4-meta6-exact-byte-evidence-terra-report.md`, SHA-256 `6A71AEB76C18645B33961E0AC0F645D5231117E39F0A88F7958746A27BC2E556`.
- 기준 원장: v19 source manifest SHA-256 `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`; input list SHA-256 `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`.
- 추적: `REQ-M5D7QC3-007`, `REQ-M5D7QC4-007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`.

## 결과

P0 0건, P1 0건. 여섯 메타를 현재 파일에서 각각 직접 SHA-256으로 다시 계산했다. 모두 JSON의 `CurrentSha256` 및 `FrozenSha256`, 그리고 v19 source manifest의 SHA와 일치한다. 짝 source SHA도 원장과 일치한다. 여섯 GUID는 전부 고유하며, Assets 아래 445개 `.meta` 파일을 검색했을 때 각각 해당 파일 하나만 소유자로 나타났다. 짝 source와 조립 정의의 실제 파일·SHA를 별도로 확인했고, 조립 정의는 v19 입력 목록에 포함돼 있다.

| 메타 파일 | 독립 재계산 SHA-256 | 짝 source SHA-256 | 조립 정의 SHA-256 |
| --- | --- | --- | --- |
| `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs.meta` | `A993C65D74081EDE2481A9112AC0B243BB0F36DF38E516F93C4F0DDA014EC621` | `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A` | `CD3E47DD94BCCB9E537CA7AF4C6029FC8327EA0154C90EC7B6FD305218FE4C0E` |
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs.meta` | `F3488A1D7C47844EBC13BD103D0FC79D5D863B357BBAA64C87ABACE122AC78D8` | `6EEFEA85B5E1A63BC47B6CF2B8341BAC71B2455BBAD9BEA462DB22679DFCC713` | `FFF73083608E9EEB8539A84A164E8E6A7665A986B777DC6EADD6E054A1BB1D0F` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs.meta` | `2D495B07A08BD26897B0CA3C6CA9E21B25D98265FE440EBCEC51813C9906921F` | `DEDF385EC466A7B7A7CED2F75AE3EE70E290A4AFDC1C40A4AB7C0F34F15A0668` | `1ED81F65C60ACA26C6BEDE2C76DE2B81A3A08FD1D22544E58EC41AF76677BA72` |
| `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs.meta` | `B5126D48799BB55D4D922ED438DAFF4A03F57858157C0A7A8FCC4A7518E2105B` | `8299C67885CFA63BD3B1A09AAC355A669F21862A8DA5643A90B23EAB008979E6` | `1ED81F65C60ACA26C6BEDE2C76DE2B81A3A08FD1D22544E58EC41AF76677BA72` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs.meta` | `DA0918DAB600E3521ABBCE4228F33BB8AF5FFB164ECDB1E346B9067EB34CB978` | `2237DACC6344E0BDD2B31B24AEE6852C2912DCC47B20472D376FB76ED2DA4B7F` | `5F79096CA7294D07C77AF7AA8612B439114148C415FE53A8B07DD19207B6595F` |
| `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/ProfileResetExecutionBridgePlayModeTests.cs.meta` | `9192D1061BDD5D125D6EE06E6A8B8B2BC439B6DF39D1BBB04A65BAAA7CB237A3` | `BC0C22518CE1A267547F72B758160610464E70D053D3A08F01CE2B80A5624BE9` | `5F79096CA7294D07C77AF7AA8612B439114148C415FE53A8B07DD19207B6595F` |

각 메타 경로·짝 source·조립 정의는 v19 입력 경로 목록에 한 번씩 포함됐다. source manifest에는 메타와 짝 source가 각각 정확히 한 번 들어 있고, 조립 정의는 source manifest 범위 밖이지만 입력 목록에는 들어 있다. 이 구분은 계약이 요구하는 입력 결속과 맞는다. JSON의 직렬화 필드·GUID, 선행 22파일 원장의 대응 경로와 요구사항·수용 기준도 대조했다. 게시 승인 값은 여섯 행과 전체 JSON 모두 `false`다.

## 허용 범위와 한계

Approved C4 r4 50행은 runtime source와 `.cs.meta`를 같은 행에 직접 명시한다. 시험 메타 다섯 개는 r4 139행의 같은 경로 `.cs.meta` 동반 규칙과 143–147행의 정확 source 경로 표를 합쳐 해석한다. 다섯 메타의 완전 경로가 r4에 개별적으로 직접 열거됐다고 보지는 않는다. 이 증거는 여섯 현재 메타 바이트와 GUID·짝 source·조립 결속을 독립 확인한 결과다.

22파일 변경 사슬은 여전히 미입증이고 게시 허용 목록은 비어 있다. 이 제한 검토는 메타 여섯 경로의 파일별 증거만 확인했다. Unity, 컴파일, 클린 복제, 게시, Git 및 원격 작업은 수행하지 않았으며 C3/C4 전체 수용을 뜻하지 않는다.
