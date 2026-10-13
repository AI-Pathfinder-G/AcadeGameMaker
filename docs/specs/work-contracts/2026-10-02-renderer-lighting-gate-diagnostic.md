# Renderer2D 실제 재질과 조명 대조 진단

상태: **Approved**, 2026-10-02 통합 책임자 승인. 소유 `REQ-PLAT-073/074`, `AC-PLAT-071/072`. 솔 초안 `docs/proposals/2026-10-02-renderer-lighting-gate-diagnostic-contract-draft.md` SHA `C1B87DB2EF5F0CD2E03751C6AFDFAA0A05E7AC913A5AEF93902A3BDD4BD7EE66`의 실제 패키지 지문·재질/셰이더 식별·정상 등록/컬링·대조 판정을 채택한다.

테라 `gpt-6-sol`은 v3 원본을 보존하고 격리 프로젝트에 `Assets/Editor/PublicationRendererLightingGateProbeV4.cs`와 메타를 구현한다. 허용 추가 출력은 `artifacts/c4-renderer-lighting-gate-probe-source-v4.cs.txt`, `artifacts/c4-renderer-lighting-gate-diagnostic-v4.json`, 같은 접두사 로그, `docs/verification/2026-10-02-renderer-lighting-gate-v4-terra-report.md`이다. 실행은 Unity 한 번, 정상 조명 켜짐/점 조명 꺼짐 렌더 요청 각 한 번이다. 전역 조명은 두 프레임에서 동일한 값으로 유지하고 점 조명만 정상 enabled 상태로 대조한다.

현재 설치 패키지의 실제 Sprite-Lit 재질과 셰이더의 정확 GUID/fileID 및 Universal2D 패스를 확인하고 임시 SpriteRenderer.sharedMaterial에 지정한다. 미리보기 장면/카메라/조명의 실제 장면 핸들·미리보기 판별·활성 상태·정렬 계층·컬링 조건과 Sprite의 실제 재질을 기록한다. 내부 목록을 관측하면 읽기만 하며 private 상태 변경이나 내부 등록 메서드 강제 호출은 금지한다. lookup 생성 getter 직접 호출이나 가짜 렌더 성공은 허용하지 않는다.

두 프레임의 원시 RGBA 바이트 SHA, 비검정/색 수와 실제 차이 픽셀 수를 기록한다. 차이가 없으면 미입증으로 남기며 요청/콜백/조명효과를 서로 구분한다. 외부 전역 텍스처 읽기는 참고값일 뿐 패스 내부 소비의 증거가 아니다. `useLights`와 lookup 소비를 직접 관측하지 않으면 해당 부분과 기존 GUID7 P1은 유지한다.

이전 승인 계약의 원문534·후보534·후보505·실행본505/캐시/설정/진단·증거 보호를 실행 전후 SHA 대조한다. 임시 장면 객체만 변경하고 자산·장면·패키지는 저장하지 않는다. 정확 소스/출력/로그 지문과 실제 오류/종료를 기록하며 실패 시 계약 내 추가 패치·재실행을 하지 않는다. 독립 루나 검수와 통합 수용은 별도다. `DependencyClosure`, `PublicationApproved`, `RendererGuid7P1Closed`, `WholeAccepted`, `LunaReviewed`는 false이다.
