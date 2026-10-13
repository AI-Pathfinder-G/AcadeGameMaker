# 501파일 격리 Unity 첫 진단 아스트라 기계 대조

- 범위: [Approved 한 번 진단 계약](../specs/work-contracts/2026-10-01-initial-code-publication-provisional-501-unity-import.md) SHA-256 `7DC5AD6BF1EF0D5E1CF00F3D6FD9F14478F368733DA2E0453BC8B4CDEEA6DB16`의 `AC-PLAT-030..032`. 루나 독립 검수 또는 가져오기 수용이 아니다.
- 테라 결과: JSON `artifacts/c4-initial-publication-provisional-501-unity-import-v1.json` SHA-256 `3784A44CC1C0779E7F51D783360919CBDAE17D47631B3C6A18622A956011CD5A`; 판독 문서 SHA-256 `C735D106DA10FFBBF891A67B438D66BF05120AFB05B9EC00136AB46422260DA0`; [테라 보고](2026-10-01-initial-code-publication-provisional-501-unity-import-terra-report.md) SHA-256 `FB4BE03964D33E1CEE7AEFD82DCD108227801329F25E926102569CC5B368D898`; 전체 로그 SHA-256 `9DEA1F0C823E9A9441987333C3959F65A0B9810349105DF5B6619BEF26F8B99A`.

- `AC-PLAT-030`: 아스트라가 원본 501개 현재 SHA를 다시 읽어 501/501 일치를 확인했다. 테라 결과는 원본 조사 전후·복사본 실행 전후 모두 501/501 일치, 초기 후보 밖 복사 0이다. 복사본은 깊은 시각화 경로에 보존돼 있다.
- `AC-PLAT-031`: Unity 호출 1회, 실제 종료 코드 0, 시간 초과 없음과 전체 로그 SHA를 대조했다. 로그에 C# `error CS...` 0줄, `Asset import failed` 13줄, `DirectoryNotFoundException` 40줄이 실제로 있다. 대표 패키지 경로 접근 예외는 로그 3562·7207줄, 렌더 파이프라인의 `TraceRenderingLayerMask.urtshader` 형식 오류는 7521~7542줄, `Curl error 42`는 7624줄이다. 실제 해석된 URP `17.6.0`·시험 도구 `1.8.0`이 로그 207~208줄에 나온다. 종료 코드 0은 가져오기 오류를 해소하지 않는다.
- `AC-PLAT-032`: JSON에는 생성 파일 18,873개의 경로·지문이 있고, 원본/복사본 `manifest.json`과 `packages-lock.json`은 전후 SHA·직접 의존 값이 같다. `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`, `CleanCloneExecuted`는 모두 거짓이다. 실제 `Renderer2D` 일곱 GUID 문자열이 로그에 없다는 사실만으로 해석 성공을 주장하지 않는다.

기계 대조로 확인한 진단 결과는 **패키지 자산 가져오기 실패를 동반한 실행**이다. 앞선 격리 경로의 패키지 자산 경로 길이는 적어도 한 사례에서 263자이며, Windows 경로 길이·격리 위치가 예외에 영향을 주었을 가능성이 있다. 원본 프로젝트 결함이나 경로만의 문제라고 확정하지 않는다. 별도 짧은 경로의 한 번 재현으로 환경 요인을 분리해야 한다. `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`의 수용 범위를 확대하지 않는다.
