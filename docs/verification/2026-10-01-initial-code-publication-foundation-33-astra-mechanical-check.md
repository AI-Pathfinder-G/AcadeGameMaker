# 기반 33파일 예비 조사 아스트라 기계 대조

- 범위: [Approved 예비 조사 계약](../specs/work-contracts/2026-10-01-initial-code-publication-foundation-33-precheck.md) SHA-256 `1AB552D823D1A1EC1B79C884A612DF19C0693293F356072E1A1DA9741633C16A`의 `AC-PLAT-018..020`. 이 문서는 루나 독립 검수나 아스트라 최종 수용이 아니다.
- 테라 출력: JSON `artifacts/c4-initial-publication-foundation-33-precheck-v1.json` SHA-256 `1CBA6A98A110B671BB85D53A4CE606D5D0F206E3F2132621090142F1D1688836`; 판독 문서 SHA-256 `5ECDF24DC0985EE89ABBE4E87F6F3B7518CEA528EC7A68090E424418A04471AB`; [테라 보고](2026-10-01-initial-code-publication-foundation-33-terra-report.md) SHA-256 `7A226C412DDE5D1A2A40C0F0A80B5522A19C26CB87FE7ADA68C0CB1B60C87095`.

## 로컬 기계 대조

- `AC-PLAT-018`: JSON의 33경로가 v2의 세 종류 3/10/20 경로 집합과 정확히 일치했다. 33개 현재 실물 지문이 출력 지문과 같고, 아스트라가 별도로 다시 읽은 전체 501개 실물도 v2 기준과 501/501 일치했다. 테라가 적은 조사 시작·끝 501/501은 원장 내부에서 확인했다. 원격 501 blob을 새 HEAD에서 확인한 결과는 아니다.
- `AC-PLAT-019`: 33개 모두 내용 값과 위치가 하나 이상 있고 읽기 오류가 없다. 직접 참조 배열의 실제 원소 수는 474개로 집계와 일치하며 각 원소에 출발 경로·원문 위치·원문 값·해석 상태가 있다. 내부 후보 117개, 엔진/패키지 후보 84개, 미해석 273개다. 미해석 중 272개는 로컬 메타의 GUID 소유 경로를 찾지 못했고 1개는 `ProjectSettings/ProjectSettings.asset`에 적힌 `Assets/Scenes/SampleScene.unity` 실물이 없다. 미해석은 결함 확정도 의존 폐쇄도 아니다.
- 추가 문맥: `SampleScene.unity` 경로는 프로젝트 설정의 `templateDefaultScene` 필드(264줄)에 있고, 실제 빌드 장면 목록은 `ProjectSettings/EditorBuildSettings.asset`의 `Bootstrap.unity`를 가리킨다. 이 사실만으로 템플릿 필드의 무해함을 확정하지 않는다. 자주 반복되는 미해석 GUID `fe87c0e1cc204ed48ad3b37840f39efc`는 `HubMenuRoot.prefab`의 `UnityEngine.UI.Image` 직렬화 블록(194~196줄)에 나타나며 패키지 요청에는 `com.unity.ugui` 2.6.0이 있다. 정확 GUID 소유 메타를 찾지 못했으므로 패키지 귀속은 후보 해석에 그친다.
- `AC-PLAT-020`: 출력 전체와 33개 행에서 `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`가 모두 거짓이다. 결과는 파일별 포함/제외 목록이 아니며 v2 독립 결과 검수, 출처 제한 수용, 전체 501 내용 조사, 현재 원격 재조회, 파일별 게시 결정을 남겨 두었다.

기계 대조에서 수량·경로·지문·필수 필드의 불일치는 발견하지 못했다. 이 결과는 테라 출력의 **예비 자료 적합성**만 확인한다. GUID의 실제 패키지/엔진 소유와 장면 실행 의미, 요구 충족, 원격 바이트, 루나 독립 검수는 여전히 미확인이다. `AC-M5D7QC3-009/010`과 `AC-M5D7QC4-009/010`의 기존 제품 수용이나 새 게시 허용을 확대하지 않는다.
