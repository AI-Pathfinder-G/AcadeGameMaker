# 테라 자체 보고 — Renderer2D 일곱 GUID 조회

승인 계약 docs/specs/work-contracts/2026-10-02-renderer-guid7-loaded-probe.md의 REQ-PLAT-044..046에 따라 격리 실행본에 진단 소스만 추가했다. 소스 SHA-256 29481075EE7F103A104EF695CC7EB31D16E5DA25B0BD196DBBF465BE56F91794. 원본 501개·추가 네 파일·505 후보·실행본 후보 505개 전후 SHA 불일치는 모두 0개다.

권한 심사 승인 뒤 Unity를 한 번 실행했으나 종료 코드 1이었다. 로그 42CC5578C63289547BD1C907B608E4792793E3F1E0671D34A8A512B81B615039의 810·815·817행에 진단 코드 99행의 CS0619 컴파일 오류, 819행에 컴파일 오류 종료가 있다. 진단 메서드는 실행되지 않았고 재시도하지 않았다. 일곱 원문 GUID/fileID/행은 JSON에 보존했으나 GUID 경로·속성 존재·타입·객체/null·instanceID·대상·의존성은 모두 미관측이다. 기본값 false/0을 관측으로 해석하지 않는다.

AC-PLAT-042 미충족, AC-PLAT-043 보존·실행·실패 기록만 충족하고 조회 미완료, AC-PLAT-044 수용 경계 유지. 모든 수용 플래그 false. 기존 P1과 루나 과거 검수는 유지하며 새 독립 검수는 없었다. 상세 원장 artifacts/c4-renderer-guid7-loaded-probe-v1.json, 설명 artifacts/c4-renderer-guid7-loaded-probe-v1.md, 원본 로그 artifacts/c4-renderer-guid7-loaded-probe-v1.log.
