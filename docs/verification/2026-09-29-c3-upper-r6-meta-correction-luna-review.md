# C3 R6 메타 식별 정보 보정 독립 검수

2026-09-29. 실제 독립 검수자 `gpt-6-luna`. 승인된 시험 `.meta` 단일 GUID 보정만 읽기 전용으로 대조했다. 코드·시험·컴파일·Unity를 수정하거나 실행하지 않았다.

## 지문 대조

R6 manifest `artifacts/c3-upper-r6-frozen-source-manifest.json`의 재계산 SHA-256은 `84B36B9A4E62BCE98206320EA4D3CBE26CCBCBF590FFA505486CF932E568776B`이고, 원장에 든 14개 파일의 개별 지문은 모두 일치한다. 선행 R5 manifest `B9DC34D615ECE99775C927FB434D88BB53F0B2F9E356BF8FECB2EB2D4483C9EE`와 파일별 대조한 결과, 변경된 항목은 승인된 감사 시험 meta 하나뿐이다.

`Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffC3AuditSuccessorTests.cs.meta`는 R5의 61바이트, SHA-256 `D8098D01444B011F288F405C9C313B8BE7499EFA4951AEE0001BE2EC7F3F70AB`에서 R6의 60바이트, SHA-256 `F1FFFA239E271C037AC818FF4C03B698DC3B07B561F8024887F4E80677A2FEFB`로 바뀌었다. GUID 문자열은 승인 전 `50b803be06dd4777ac62bd70ca5368f5e`(33자), 승인 후 `50b803be06dd4777ac62bd70ca5368f5`(32자)이며 마지막 문자 하나를 제거한 관계를 확인했다.

승인 증거 `artifacts/c3-audit-meta-guid-correction/evidence.json`의 파일 경로·전후 크기·지문·변경 경로는 현 자료와 일치한다. 승인된 변경은 `docs/approvals/2026-09-29-c3-audit-test-meta-guid-correction-approval.md`의 단일 GUID 문자열 수정 범위에 정확히 한정된다.

## 자산 검색과 컴파일 영향

`Assets` 아래 438개 `.meta`를 직접 순회해 GUID 줄 438개를 확인했다. 모두 32자리 소문자 16진수이고 유효 GUID 중복은 0개이며, 새 GUID는 한 곳에서만 사용된다. 감사 시험 `.cs`는 해당 `.meta`와 같은 폴더에 있고, 파일 안에 `HubMenuIntentHandoffC3AuditInputV1` 정의가 있다. 파일은 `AcadeGameMaker.Hub.Presentation.Unity.EditMode.Tests` 편집 모드 asmdef가 적용되는 `Tests/EditMode/HubPresentation` 아래에 있다. 기존 감사 시험이 참조하는 형식과의 소스상 이름·namespace도 일치한다.

보존된 R5 실제 첫 집중 실패 `artifacts/c3-r5-focused-edit-r1-failure-verification.json` 및 로그 SHA-256 `961C42729B0743FA8B1F48A5A1C5670889E07EA3D35BED90626C27DCB86E472E`는 실제 종료 코드 1, XML 부재, 155개 시험 미실행, 884개 입력의 전후 차이 0, `.meta` GUID 길이로 인한 시험 자산 제외와 그 결과인 `CS0103`을 기록한다. 이 실패는 R6에서 지우거나 성공으로 바꾸지 않았다. 승인된 원인과 보정으로 그 누락 형식 참조가 편집 시험 어셈블리에 포함될 수 있는 구조는 확인했으며, 이 정적 확인만으로 새 Unity 컴파일 성공을 보증하지는 않는다. R5의 다른 컴파일 기록·한계와 377행 원장은 변경 없이 역사 자료로 남는다.

## 판정

**제한 정적 보정 검수 P0 0건, P1 0건.** 새 원장은 R5와 지정 meta 항목 하나만 다르고, 438개 GUID 형식·중복 및 시험 자산 위치에 추가 차단 결함은 발견하지 않았다. 이는 앞서 실제 실행된 R5 실패를 해소했다는 판정이나 시험/컴파일 통과 판정이 아니다. 다음 실제 Unity 재실행에서 편집 시험 자산 검색, 컴파일, XML, 집중 선택 결과와 종료 코드를 확인해야 한다. 기존 정적 R5 검수는 유지되며 C3 전체 AC-007/008은 Open/Not Verified, C4는 Review다.
