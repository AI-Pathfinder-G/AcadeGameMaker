# 정상 조명 갱신 뒤 렌더 대조 진단

상태: **Approved**, 2026-10-02 통합 책임자 승인. 소유 `REQ-PLAT-075/076`, `AC-PLAT-073/074`. 솔 초안 `docs/proposals/2026-10-02-renderer-normal-update-diagnostic-contract-draft.md` SHA `513EB6E45EA0298D7DBD658CFF7904F0F7D7727E89AC012F08CDD13A74E1269C`의 실제 API·정상 갱신 경계·관측 한계를 채택한다. v5 두 프레임의 차이0은 보존하고 갱신 누락을 확정 원인으로 쓰지 않는다.

테라 `gpt-6-sol`의 새 허용 출력은 `output/u505-r1/Assets/Editor/PublicationRendererNormalUpdateProbeV6.cs`와 메타, `artifacts/c4-renderer-normal-update-probe-source-v6.cs.txt`, `artifacts/c4-renderer-normal-update-diagnostic-v6.json`, 같은 접두사 로그, `docs/verification/2026-10-02-renderer-normal-update-v6-terra-report.md`이다. 이전 진단 소스/결과·이동 보관 v4는 불변이다.

Unity 한 프로세스에서 -quit 없이 정상 update/QueuePlayerLoopUpdate 단계를 사용한다. 최소 서로 다른 두 갱신 틱을 거치고 실제 점 조명 경계구의 중심/양수 반경·장면/등록·활성/계층 조건을 읽기 전용 확인한 뒤에만 켜짐 렌더 한 번을 제출한다. 꺼짐 전환 후 다시 정상 갱신을 거쳐 동일 fixture로 두 번째 렌더를 제출한다. 내부 갱신 함수 직접 호출이나 private 상태 대입은 금지한다. 자동 카메라 렌더가 추가 관측을 만들지 않도록 카메라 enabled는 양 프레임에서 동일하게 false로 둘 수 있으며 명시 요청의 실제 지원·콜백을 확인한다. 경계구가 미입증이면 대조를 억지 실행하지 않는다.

구성 완료 후45초 내부 기한, 프로세스180초 외부 감시를 적용한다. Run 반환 후 갱신 구독으로 이어가며 종료/예외/기한 초과에서 정확 구독 해제·자원/미리보기 장면 정리·CreateNew 새 결과 기록·EditorApplication.Exit을 수행한다. 외부 중단은 실패이며 성공 결과로 꾸미지 않는다. 정수 SceneHandle은 UInt64 GetRawData로 읽고 초기값과 정상 갱신 이후값을 구별한다. 두 실제 RGBA SHA와 차이 픽셀, 요청/콜백 수와 오류를 기록한다.

기존534/505/실행본505·설정/패키지·이전 진단/증거의 사전사후 SHA와 정확 실행 소스/로그 지문을 확인한다. 자산·장면 저장은 금지한다. 실패는 보존하고 계약 내 임의 패치·추가 실행은 하지 않는다. 조명 영향이 있어도 패스 내부 useLights/lookup 소비와 기존 Renderer7 P1·전체 의존/회귀/게시·독립 수용을 자동 종결하지 않는다. 관련 결과 플래그는 false이다.
