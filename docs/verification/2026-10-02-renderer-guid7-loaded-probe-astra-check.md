# Renderer2D 일곱 참조 실제 조회 기계 대조

[최초 계약](../specs/work-contracts/2026-10-02-renderer-guid7-loaded-probe.md)의 `AC-PLAT-042..044`와 [API 보정 계약](../specs/work-contracts/2026-10-02-renderer-guid7-api-correction.md)의 `AC-PLAT-045`에 한정한다. 테라 역할이 만든 조회 구현과 결과의 대조이며 루나 독립 수용이 아니다.

첫 진단은 코드의 제거된 API CS0619로 실패하여 일곱 조회가 모두 미실행이었다. v1 실패 원장（`../../artifacts/c4-renderer-guid7-loaded-probe-v1.json`；로컬 보존 자료） SHA-256 `F1F13950E8CD2DBAA1E744970101F1E7822B51881877BC5DAA10FBA1EDEA3BAC`은 보존했다. 실패 소스 SHA-256 `29481075EE7F103A104EF695CC7EB31D16E5DA25B0BD196DBBF465BE56F91794`, 보정 소스 `1D572AAEC5E693E949F0BA5AF847DDA1020434270C3E70FF37823ACF601068C0`의 차이를 읽어 API 읽기·DTO 문자열·v2 출력 경로 세 곳만 바뀐 것을 확인했다. 저장/직렬화 수정 호출은 없다.

v2 결과（`../../artifacts/c4-renderer-guid7-loaded-probe-v2.json`；로컬 보존 자료） SHA-256 `A42A5E41E3D83DBE89B5AE51A95975F8C3EC378CE0BFCA217B310350462699F8`, 로그 SHA-256 `819CA7FA6BB078AA67DAB365695FC87C4101E900C1B364193247118A14C6B4DA`, [테라 보고](2026-10-02-renderer-guid7-loaded-probe-v2-terra-report.md)를 대조했다. 보정 실행 한 번은 종료코드 0이다. 일곱 GUID의 `GUIDToAssetPath`가 모두 빈 경로이며, 여섯 `probeVolumeResources` 속성은 실제 ObjectReference이지만 로드 객체가 null이다. `m_FallOffLookup` 속성은 부재라 null 여부와 식별자도 미관측으로 기록됐다. `GetDependencies` 19경로는 일곱 원문 GUID의 해석 성공을 뜻하지 않는다.

아스트라가 원본·정규화 후보·실행본 각 505개 SHA를 다시 전수 확인했다. 지문 불일치 0이다. 라이선스 토큰·종료 통신/스레드 메시지는 로그에 보존돼 있다. 이 결과는 저장 참조가 현재 로드 상태에서 해결되지 않은 사실을 좁히며, 제품의 누락 자산 결함·무영향·자동 이관·빌드 성공을 증명하지 않는다. 기존 일곱 GUID 미해석 P1과 모든 게시/전체/의존/루나/깨끗한 복제 게이트는 미완료다.
