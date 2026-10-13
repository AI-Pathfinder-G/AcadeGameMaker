# Q0 최초 게시 기반 재현의 좁은 후속 계약 초안

- 상태: Draft, 아스트라 승인 전 구현·실행·게시 금지
- 작성: 2026-09-29, 별도 테라 역할 `gpt-6-sol`
- 추적: 기존 REQ-M5D7Q0-001/009/010 및 AC-M5D7Q0-011, C3 REQ-M5D7QC3-001..007·AC-M5D7QC3-009/010의 게시·재현 근거에 한정

현재 읽기 전용 작업은 [승인 계약](../specs/work-contracts/2026-09-29-c3-initial-publication-evidence-readonly.md)의 범위다. 이 초안은 제품 규범·기존 시험·pin·선택·게시 권한을 바꾸지 않는다.

## 실제 전제와 한계

`Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs`의 현재 SHA-256은 `C27116AC35BF83D4AB8794FA9D3E379AD6F127B0E52B48C51399F49ADDF862D4`다. 66~89행의 `AC011_ActualRepositoryManifestClassifiesTheExactQ0DeltaWithoutFolderInference`는 실제 Git 변경·미추적 목록과 정확 DeclaredQ0Paths의 교집합을 계산하며 80행에서 비어 있지 않음을, 81~82행에서 필수 경로의 존재를 요구한다. 175~180행의 도우미는 실제 `git status --porcelain=v1 -z --untracked-files=all`을 실행한다. 이번 작업은 코드만 읽었으며 Git을 실행하지 않았다.

정상 깨끗한 게시 기반은 그 변경 집합이 비어 있을 수 있다. 따라서 같은 시험이 깨끗한 기반에서 통과할 것이라고 주장할 수 없으며, 실패를 제품 결함이나 최초 게시 재현 실패로 곧바로 판정할 수 없다. 85~88행의 기존 이미 변경된 작업 공간 한계도 유지한다. 역사 7개 현재 바이트와 별도 후속 2개 pin은 221~235행의 기존 배열 그대로 보존한다. Router `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`, Adapter `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB`를 새 값이나 복수 허용 값으로 바꾸지 않는다.

## 제안하는 최소 후속 규칙

1. 아스트라가 별도 계약에서 게시 기준 commit과 정확 경로·바이트·meta·조립 및 제외 항목의 원장을 승인한다. 이번 27개 근거 원장은 검토 입력이며 그 승인 원장이 아니다. 오래된 로컬 원격 캐시와 상위 문서 병합 보고만으로 기준을 고르지 않는다.
2. 기존 실제 변경 집합 검사는 기존 이름·본문·필수 선택과 역사 증거를 보존하여 해당 전제의 검증으로 유지한다. 깨끗한 기반에 이를 적용하여 필수 변경 경로가 없다는 실패를 숨기거나 가짜 delta를 제조하지 않는다.
3. 깨끗한 기반 재현에는 별도 양성 검사를 제안한다. 승인 기준의 실제 추적 파일 집합과 작업 바이트/meta·GUID/조립을 대조하고, 깨끗함과 승인된 생성물 제외 조건을 별도로 검증한다. 현재 디스크 폴더 접두사로 파일을 승인하지 않는다. 외부·미승인·누락·변경 파일은 실패 또는 정확한 차단으로 기록한다.
4. 새 검사 허용 후보는 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0InitialPublicationBaselineEditModeTests.cs`와 그 meta, 승인된 고정 게시 원장 한 개, 별도 정확 선택 한 개뿐이다. 기존 조립을 사용하며 asmdef/friend/runtime/settings는 변경하지 않는다. 제안 사례 이름은 `AC011_CleanInitialBaselineMatchesApprovedExactFileBytes`와 `AC011_HistoricalDeltaContractRemainsSeparateFromCleanBaseline`이다. 승인 전 파일·시험은 만들지 않는다.
5. 새 검사가 통과해도 기존 562/51/610 이름·수와 해당 전제의 실행 증거를 대체하지 않는다. 깨끗한 기반에서 기존 실제 delta 사례와 필수 선택을 어떻게 실행·보고할지 아스트라가 명시해야 한다. 전제 충돌 사례를 임의 제외·건너뜀·통과 처리하지 않는다. 정확 selector 구분이 승인되지 않으면 깨끗한 기반의 필수 실행 수용은 차단으로 남긴다.
6. 승인 이후 독립 루나 검수와 실제 깨끗한 복제/Unity import·정확 실행을 수행해야 한다. Git·네트워크·복제·Unity 실행 권한은 이번 문서 작성 권한에서 발생하지 않는다. 현재 미해석 Renderer2D GUID 7개와 설정 10개·도구 입력 7개를 그 과정의 정확 차단으로 유지한다.

게시 승인과 현재 C3 실행 수용은 별도다. AC007/008은 미수용, C4는 Review이며 이 초안은 C1/C2 실행이나 최초 게임 코드 게시를 개시하지 않는다. 원래 489개 후보·61개 근거·428개 공백 및 다섯 게시 차단은 유지한다.
