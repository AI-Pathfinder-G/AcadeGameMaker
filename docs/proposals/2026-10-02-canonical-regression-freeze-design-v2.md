# 534개 정규화 후보의 C3/C4 회귀·감사 동결 보완안

- 상태: **Draft**. 기존 [505개 회귀 초안](2026-10-02-canonical-regression-freeze-design.md) SHA `319F8F4EA80C9022FE557BDF48E4FEE200A85CF853D424B7BD92CAD841377400`을 보존하고, 그 초안의 시험 파일 2개 한정을 보완한다. 이 문서는 구현·실행·승인·게시 권한이 아니다.
- 기준: `artifacts/c4-publication-lf534-baseline-v2.json` SHA `89BC3D6EFDD2B1344E8E6F4F74E819382F7AB2F9C67EBFE373B3BF62F4D26F9E`. 534개 후보와 각 행의 원본·정규화 SHA는 이 동결에서만 읽는다. 별도 엄격 검사기 계약의 결과와 루나 독립 검수는 아직 이 문서의 수용 증거가 아니다.
- 추적: `REQ-PLAT-038/039/040/054..056`, `AC-PLAT-036..038/052..054`; `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, `AC-M5D7QC3-007/008`. 현재 원래 C4 수용과 정규화 후보 회귀 수용은 서로 다른 사건이다.

## 기존 초안에서 빠진 실제 바이트 결속

534 원장은 기존 505의 18개 전이에 7개 LF 전이를 추가한다. 추가 원본→후보 SHA는 다음처럼 정확히 고정한다. 이는 CRLF 제거 근거이지 과거 검수의 새 바이트 수용 선언이 아니다.

| 경로 | 원본 SHA-256 → 후보 SHA-256 | 실제 사용 지점 |
| --- | --- | --- |
| `artifacts/c3-r11-edit-matrix-selection.json` | `B04192B8627B3BC03F934B102ADA35D3CD095757C88DC4ED2AD9FD476F2ACBED` → `6D5DB2EE398B6BE007D058D0D80BBC8FE744F0495B50E1692B653781129E3DB6` | C4 엄격 감사 361–369행, 행 검증기 44·52·63행 |
| `artifacts/c3-r11-edit-remaining-selection.json` | `F8CCD04044B67A76E3033CD3D6A5A169B98BA5B25107801B26C667520B125EAF` → `5D01063A6C845F692D1521A374D824D4DA0DABBFD8299FAE688001FC4DED7416` | C4 엄격 감사 361–369행, 행 검증기 45·53행 |
| `artifacts/c4-predecessor-selection-name-map-v1.json` | `7647BB0AB3DD488080A53794C8C27DB0D7600FD7D3F79FB21E40CA9102851C91` → `3B481B1A6BA40F7A107234BD11188B2C6FF93132619BE4E68B3E24835F01EBF1` | C4 계획 `PredecessorSelectionMap`, 행 검증기 59–63행 |
| `artifacts/c4-verify-required-rows.ps1` | `E20892F551795B6CBC021C56007BC9C2B4F0FB8515230DB267FA8331594B22E6` → `46D91AD296CF479104A09747525287AFDBD941974E2BC2868BD8064E44C7A06B` | 원래 포착기 3행·선택 실행기 3행·큐 3/39행·실행 검증기 3행이 읽거나 호출. 원래 계획 도구 SHA도 고정 |
| `docs/evidence/c4-audit-current-source-pins-v2.json` | `5DD70E7A7BD8E79A997F10362EF2DBAD148A913BD3221E20CF5F0DF8D9F33AF5` → `47B1CB421811595FD6A757B10CDB199592B6C64B7CF68AAE95C9F4E7C5EA26D3` | Q0 감사 시험 258–273행의 원시 지문과 엄격 감사 284–311·330–331행 |
| `docs/evidence/c4-runtime-predecessor-manifest.json` | `C8FD2B74EF842F45B8D202B1DE3DC408F09F20F69C1BD030FE589E43B8DDD7F7` → `983DA25E704213C02E037477FC388C7BF97B57D5740FE457E5614CD1FCC76530` | Q0 감사 시험 275–290행과 엄격 감사 332·343–355행 |
| `docs/verification/2026-09-30-c4-editmode-disposal-pointer-recovery-luna-implementation-review.md` | `03AC446E15FBD950CEA90E8D50644B9E5DE1F299E2932DE8CEFCAD2D9A017863` → `A2CCBB583556C2ED16B80D26E255575FA35ABC310DE34945BD1CAD52A59545BC` | Q0 감사 시험 296–300행의 원시 review 결속. **과거 루나 검수 내용과 대상 바이트는 그대로**다 |

엄격 감사 원본 `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs` SHA `6EEFEA85B5E1A63BC47B6CF2B8341BAC71B2455BBAD9BEA462DB22679DFCC713`는 534 후보에서 바이트 동일하다. Q0 원본 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs`는 `20F5E1C963B6F969649DDE58FA0B48338B2446049AB56949DE3BA60EB3812567` → `AE3FBA200DB824F21CC0EF454EE17390A85C2EA6DE4F6DF740CA1FE1EF720554`의 LF 변환만 이미 505에 기록됐다. 원래 시험 두 파일은 새 계약에서만 후속 편집할 수 있고 현재 바이트를 소급해서 바꾸지 않는다.

엄격 감사 284–338행은 현재 핀 자원 내부의 **원래** 소스 SHA·승인 문서·루나 review를 결속하며, Q0 266–307행도 원래 자원과 과거 predecessor·review SHA를 대조한다. 자원의 외피만 LF로 바꿔 그 안의 과거 `StaticReview`를 새 바이트의 독립 검수로 재해석할 수 없다. 원래 review `03AC...`는 역사 수용의 대상이고 정규화 review `A2CC...`는 동일 텍스트의 다른 원시 바이트일 뿐이다. 역사 원본 바이트를 별도 불변 경로·객체에서 실제 재조회할 수 없으면 원래 SHA를 검증했다고 주장할 수 없고 새 감사 수용을 중단한다. 새 제품 바이트·새 감사 자원·새 시험 코드에 대한 루나 검수는 **새 독립 증거**가 있어야 하며 현재 `LunaReviewed=false`를 유지한다.

## 후속 한정 계약의 분해와 선행 순서

1. **바이트 전이 계약.** Approved 505 원장과 534 v2의 합집합을 정확히 비교하고 25개 CRLF→LF의 실제 원본 바이트·후보 바이트·쌍 수를 엄격 검사기로 검증한다. 505 원장에 없던 7행과 기존 18행은 각 원본 SHA가 역사 근거의 해당 SHA와 같을 때만 변환으로 연결한다. 505/534 원장 자체, 원본 증거·역사 시험 원문은 불변이다. 검사기 성공은 정규화 기계 사실에 한하며 C4 행동이나 새 독립 검수 통과를 뜻하지 않는다.
2. **Q0·엄격 감사 시험 계약.** 새 허용 시험 파일은 기존 두 fixture 외에 정확히 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs`와 `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/C4ExecutionBridgeStrictAuditTests.cs` 두 개를 추가해야 한다. 현재 자원·predecessor·review의 역사 핀과 원래 메서드 본문 보존 단언은 그대로 유지한다. 새 정규화 분기는 별도 새 핀 자원 및 명시된 현재 SHA를 **원시 지문 먼저** 검증한다. 그 안에서 역사 predecessor·승인·review는 `Historical` 역할로 원래 SHA와 원래 대상 버전을 고정하고, 정규화된 파일은 534 전이 원장으로 별도 검증한다. `StaticReview`가 현 정규화 소스의 검수라고 요구하는 기존 호출의 의미를 무작정 뒤집지 않는다. 현재 바이트의 신규 독립 review가 발급되기 전까지 새 회귀의 감사 수용은 중단한다. 두 시험의 사후 SHA, 메서드·선택 이름 수, 기존 AC009/010 본문/negative 조건을 새 원장에 고정한다. 과거 문서나 역사 Q0 단언을 지워 통과시키지 않는다.
3. **선택·행·도구 계약.** 원래 `c4-verify-required-rows.ps1` 43–63행의 여섯 선행 선택 SHA와 `C4-LegacyRun` Matrix91 고정 경계, 40·337–356행의 focused 선택/행 결속을 새 전이로 정확 갱신한 *새 도구 분기*를 발급한다. Matrix91·Remaining149의 이름·순서·선택 문자열·부모 Edit240·서로소 91+149 관계는 바꾸지 않는다. predecessor name map도 새 SHA를 고정하고 기존 240/15/562/51/610 이름·순서·선택은 보존한다. 원래 행 검증기 자체가 LF만 바뀌어도 도구 SHA가 달라지므로 기존 포착기·실행기·큐·실행 검증기의 import/도구 고정 경로까지 정확히 추적해 필요한 successor를 만든다. 새 도구의 소스 지문과 정해진 실행 경로를 입력 원장에 직접 넣고 이전 v19 계획/도구는 불변으로 남긴다. 정확 closure 없이 옛 도구와 새 원장을 혼합하지 않는다.
4. **새 동결과 실제 회귀 계약.** 편집 후 fixture 2개·감사 시험 2개의 SHA는 LF534 후보 SHA와 **별도 승인 시험 수정 전이**로 기록한다. 새 선택·행·도구를 만든 다음 새 source manifest·직접 입력 원장·9회 계획을 동결한다. 원장 자체결속은 순환을 피하도록 파일 생성 순서와 직접 입력을 명시하고 이전 v1/v16/v17/v19의 원본 SHA를 역사 근거로만 참조한다. 실행 전 9개 선택의 현재 소스 핀·정확 이름/순서/selector와 필수 188행 전체 기대 레코드, 534 파일 의존·클린 복제 가용성을 검증한다. 새 실행은 원래 139/32/5/91/149/15/562/51/610 선택, 180초, 원시 종료 0·QA/외부 0, 실패/건너뜀/불확정 0, 전후 동일 입력, 전체 행 및 예상 밖 ID 거절을 유지한다. 534개만으로 실행 입력이 닫혔다고 주장하지 않는다.

중단 조건: 역사 review SHA를 정규화 SHA로 바꾸고 이를 새 검수로 부를 때, 감사 본문의 원래 토큰·음성·원본 역할 단언을 삭제할 때, old selection의 내부 원시 핀을 전체 치환할 때, 새 시험 파일/도구의 최종 지문 전에 원장을 동결할 때, 독립 검수 없이 수용 플래그를 올릴 때, 계획 밖 입력을 자동 포함할 때. 이 문서의 후속 범위는 시험·증거·QA 결속뿐이며 제품 runtime·자산·설정·조립·공개 API의 변경을 허용하지 않는다.
