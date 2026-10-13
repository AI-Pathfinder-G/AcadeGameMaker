# 테라 자체 보고 — Renderer2D GUID7 API 보정 진단

REQ-PLAT-047에 따라 v1 진단 소스를 SHA-256 29481075EE7F103A104EF695CC7EB31D16E5DA25B0BD196DBBF465BE56F91794로 보존하고, 승인된 세 위치만 보정했다. v2 소스 SHA-256 1D572AAEC5E693E949F0BA5AF847DDA1020434270C3E70FF37823ACF601068C0. v1 결과 네 파일은 변경되지 않았다.

권한 심사 뒤 Unity를 한 번 실행해 종료 코드 0, 로그 SHA-256 819CA7FA6BB078AA67DAB365695FC87C4101E900C1B364193247118A14C6B4DA를 얻었다. 일곱 GUID 경로는 모두 빈 문자열이었다. 앞의 여섯 속성은 ObjectReference로 존재하지만 실제 로드 객체는 null이고 EntityId 문자열은 관측됐다. m_FallOffLookup 속성은 부재하여 타입·참조·null·EntityId가 미관측이다. 로드 Renderer2DData의 GetDependencies 19경로를 기록했으나 일곱 참조의 해석 증명으로 쓰지 않는다. 라이선스 토큰, 종료 Curl·스레드 메시지도 로그에 남았다.

AC-PLAT-045와 AC-PLAT-042..044를 자체 대조했다. 원본 501·추가 4·정규화 505·실행본 후보 505 바이트의 불일치는 0개다. 모든 게시·폐쇄·전체 수용·루나·깨끗한 복제 플래그 false이며 기존 P1은 유지한다. 상세 원장 artifacts/c4-renderer-guid7-loaded-probe-v2.json, 설명 artifacts/c4-renderer-guid7-loaded-probe-v2.md, 로그 artifacts/c4-renderer-guid7-loaded-probe-v2.log. 루나 독립 검수는 별도다.
