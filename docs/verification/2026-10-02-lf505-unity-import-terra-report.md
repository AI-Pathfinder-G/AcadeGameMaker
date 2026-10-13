# 테라 자체 보고 — 505개 후보 Unity 진단

승인 계약 docs/specs/work-contracts/2026-10-02-lf505-unity-import-diagnostic.md의 REQ-PLAT-041..043을 수행했다. 원본 501개·추가 네 파일·505 후보의 전후 SHA 불일치는 모두 0개다. 새 복사본은 시작에 정확히 505개였고 실행 후 후보 바이트 변경 0개, 생성물 18881개다.

Unity 6000.6.0f1_f7f8ed4d1e24을 권한 심사 승인 뒤 한 번 실행했다. UTC 2026-10-01T22:52:45.642194Z부터 2026-10-01T22:53:47.775023Z까지, 종료 코드 0, 시간 초과 False. 로그 SHA-256 B538CC5B2F8BA04DFB616944DD93932137300FFFE91878F8E4C61C318EE094F1. 패키지 25개 해석 및 잠금 버전 일치, 최초 자산 새로고침 종료와 C# 컴파일 오류 0건이 관측됐다. 라이선스 토큰 갱신 오류(89행), 컴파일 캐시 실패(860~863행), 종료 중 Curl 오류(7002행)는 남았다.

AC-PLAT-039: 지문과 복사본 차이 기록 자체 확인. AC-PLAT-040: 실행·오류·패키지 근거 자체 확인, 오류 없는 결과로 보지 않음. AC-PLAT-041: 모든 게시·의존 폐쇄·전체 수용·루나 검수·깨끗한 복제 플래그 false. Renderer2D GUID 해석과 제품 수용은 확정하지 않음.

상세 원장 artifacts/c4-lf505-unity-import-v1.json, 결과 artifacts/c4-lf505-unity-import-v1.md, 원본 로그 artifacts/c4-lf505-unity-import-v1.log. 루나 독립 검수는 별도다.
