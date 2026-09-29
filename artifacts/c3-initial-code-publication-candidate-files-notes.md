# C3 초기 코드 게시 후보 파일 원장 설명

2026-09-29. 솔의 실제 모델은 `gpt-6-sol`이다. 이 자료는 사용자 승인된
게시를 위한 구체적 검토 준비이며 초기 기반 계약 승인·게시·빌드/시험 통과가
아니다. 코드·자산·meta·설정·진행 중 QA 입력/선택/기존 증거를 수정하지
않았고 Unity·컴파일·Git·네트워크를 실행하지 않았다.

기준은 [초기 기반 제안](../docs/proposals/2026-09-29-c3-initial-code-publication-baseline-contract.md)
SHA `CEB7F6A1184F8FA32C96F0EE50B6363BD278C7B6D8C35FA16E15A978EE08211E`다.
기존 평가 `78292AE307723AF226C1FA2C25B910EB0923B4187E45F735B6A4DD966038A239`를
보존했고 원격 기준 `46b14e97be2c5fc6017d92c09e3200899599830a`를 재조회하지 않았다.

## 정확 목록과 출처의 의미

[JSON 원장](c3-initial-code-publication-candidate-files.json)에는 중복 없는
정확 **489개 후보 파일**을 경로순으로 기록했다. 각 행은 현재 SHA-256,
meta 경로/GUID 또는 해당 없음, 조립 소속, 포함 이유, 파일 내 실제 REQ
언급, 기존 승인 상태 spec의 정확 경로/파일명 언급 위치와 REQ 후보를 가진다.
조립 외 자산·설정·문서는 조립 외 자료로, 공통 폴더 meta는 관련 조립 목록으로
구분한다. 폴더 meta 포함은 그 폴더 내용 전체의 게시 허가가 아니다.

| 후보 구분 | 파일 수 |
| --- | ---: |
| 20개 명시 조립의 C#/asmdef | 188 |
| 명시 실행 자산·플랫폼 자산 | 20 |
| 파일 meta / 상위 폴더 meta | 208 / 40 |
| 파일별 검토할 최소 ProjectSettings 후보 | 10 |
| manifest/lock/줄바꿈 기반 | 3 |
| QA 도구·선택·예상 행 원장 | 13 |
| Q0가 직접 읽는 pin 문서 | 4 |
| 감사 시험이 직접 읽는 선행 원문 | 1 |
| 실제 51개 선택이 호출하는 외부 작업자 | 2 |

13개 런타임, HubAuthoring 저작 1개, Profile/InputUnity/Hub 편집 3개,
InputUnity/Hub 실행 2개 및 초기 플랫폼 Bootstrap 1개 조립을 명시 선택했다.
현재 시험 조립 소스의 일부를 잘라내거나 조립/friend 방향을 변경하지 않았다.
Runtime/Run/Costumes/Presentation과 관련 조립, 전투/이동/의상 자산 전체,
ProjectSettings 전체, 미추적 자료 전체는 후보에 포함하지 않았다.

285개 파일에 승인 상태 spec의 정확 경로 언급 후보가 있고, 파일명만 언급된
15개를 합치면 300개다. 나머지 189개는 현재 조회로 파일 단위 spec 출처를
입증하지 못했다. **300개도 승인 완료 파일 수가 아니다.** 문서의 예시·설명
언급이 허용 목록인지, meta까지 허용하는지, 현재 바이트가 어떤 승인 후속인지
확정하지 않은 항목을 승인으로 추정하지 않았다. 모든 후보는 원장의
`PublicationApproved=false`, `CurrentBytesAcceptanceProved=false`와
파일별 `AuthorizationAssessment` 검토 필요 상태를 유지한다. spec 언급과
파일 내 REQ 문자열은 출처 확인의 단서이며 최종 파일별 권한 판정이 아니다.
출처 줄은 문서 경로/줄/REQ 식별자로 기록하여 영어 설명 원문을 복사하지 않았다.

## 실제 선택과 직접 읽기 자원

562 편집·610 실행·51 process·C3L 11개·C3 상위 155/14개 선택 원장과
377개 내부 행 예상 원장을 정확 파일로 기록했다. 선택 이름·내용을 변경하거나
다시 생성하지 않았다. 610의 실제 Ordan requester 9개 이름 때문에 graph와
nested CombatEncounterPlayer prefab 및 meta를 포함했다. 별도 전투 장면은
이 선택의 직접 의존으로 입증되지 않아 포함하지 않았다.

외부 작업자는 `c2-r36-selection-preflight.json`의 정확 51개
ProfileResetDiskProcessV1Tests 선택, RequiredWorkerInputs 및 실제 시험
`:119`의 csproj 경로로 확인한 Program.cs/csproj 두 파일뿐이다. 작업자는
net10.0이며 그 실제 SDK/호스트 실행 전제는 후속 계약에서 확인해야 한다.
선택 호출 근거가 없는 RootLockHolder.ps1과 C2RRestart README는 제외했다.

Q0 감사가 File.ReadAllText로 읽는 네 문서는 과거 Q0 implementation evidence,
C2 r25 Luna digest, C2R process execution, adapter successor SHA approval다.
다른 Q0 허용 문서·게임 문서는 이름이 언급됐다는 이유만으로 추가하지 않았다.
감사 후속 시험 `HubMenuIntentHandoffC3AuditSuccessorTests :85`가 직접 읽는
선행 전체 원문 `.txt` 한 파일은 부모의 추가 지시를 근거로 별도 포함했다.

## 게시 계약에서 해결해야 하는 경계

- Q0 `AC011_ActualRepositoryManifestClassifiesTheExactQ0DeltaWithoutFolderInference`
  `:74–88`은 실제 Git 변경·미추적 목록에 Q0 행과 RequiredCurrentQ0Paths가
  있어야 한다. 깨끗한 최초 기반 게시 이후 동일 선택을 그대로 재현할 수
  있다는 주장과 이 시험의 작업 상태 전제는 다르다. 기존 시험/선택/역사를
  유지한 채 게시 기준의 정확 후속 검사를 별도 계약에서 해석·승인해야 한다.
  이 작업에서 깨끗한 checkout 시험 실패를 실행·실증하지 않았다.
- Renderer2D의 일곱 GUID는 로컬 Assets/PackageCache meta 조회로 미해석이다.
  엔진 내장 자료·직렬화 이력 등의 출처와 실제 import 진단을 추가로 확인한다.
  자산 결손이나 현재 진행 중인 시험 실패로 단정하지 않는다.
- 최소 설정은 명시한 10개 파일만 후보이며 파일별 플랫폼 승인 연결과
  환경별 값 검토를 마쳐야 한다. 어떤 설정 파일을 실제 최초 기준으로 받을지
  이 원장이 승인하지 않는다.
- 보존한 QA 도구 중 Test-QaCatalog의 schema/coverage/catalog 네 입력과
  c3l-regression-selection 생성기의 과거 선택 두 파일·XML 한 입력은
  `NotIncludedToolInputs`에 별도 경로/지문으로 기록했다. 이번 후보에 임의로
  추가하지 않았다. 기존 원격 존재와 재생성 도구 사용 여부를 별도 확인해야
  하며, 현재 후보만으로 그 도구의 모든 입력이 닫혔다고 주장하지 않는다.
  고정된 현재 선택 원장 자체와 역사 재생성 실행은 구분한다.
- 현재 후보의 14개 C3 동결 대상은 조회한 R6 원장
  `84B36B9A4E62BCE98206320EA4D3CBE26CCBCBF590FFA505486CF932E568776B`와
  전부 일치한다. 동결 원장은 읽기 기준으로 기록했으며 최종 통합 수용이나
  해당 원장 자체의 게시 승인으로 치환하지 않는다. 기존 Q0 pin과 역사,
  `.gitattributes` 바이트 처리, 모든 후보 지문을 실제 게시 전 다시 대조한다.

기존 승인 범위를 파일별로 확정하고 현재 소스에서 필요한 독립 검수·실행
증거 및 첫 깨끗한 프로젝트 재현을 받는 작업이 남아 있다. 새 실행·보정·승인은
하지 않았으며 가짜 전역 상태나 권한 자료도 만들지 않았다.
