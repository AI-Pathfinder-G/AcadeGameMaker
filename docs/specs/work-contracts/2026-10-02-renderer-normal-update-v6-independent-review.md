# 정상 갱신 렌더 v6 독립 검수

상태: **Approved**, 2026-10-02 통합 책임자 승인. 루나 `gpt-6-luna`가 `REQ-PLAT-075/076`, `AC-PLAT-073/074`의 실제 정상 갱신·조명 대조·보존을 읽기 전용으로 검수한다. JSON SHA `68B60F6446D6499746C4A179F8AF2B4D5BB7A992F6F83EAA2E6337DAD2EE8609`, 보고 SHA `14FD0694C62D39702ECF7E15D6BE237DA5A90B8CBD4889E791C30E747A8FC634`, 소스 SHA `344ED438ACE70B8AEEB2844752CF1EF817AEA0126AB9A45C698D64B40A1D2056`, 로그 SHA `19075A71B9C5DB28430ECA9C6E4D89FBD14768B9AE873C8580C2202446153C43`를 실제 대조한다.

정상 update4회와 두 단계 각2회, 실제 경계구 반경0→3과 중심 일치, 강제 내부 갱신 호출 부재, 정확 재질/셰이더·등록·활성·계층·카메라 조건, 켜짐/꺼짐 각한 요청과 콜백 각2회·두 RGBA SHA·실제 차이16픽셀을 로그/소스/원시 관측에서 확인한다. -quit 없이 단일 프로세스·내부45초/외부180초·정리/구독 해제/CreateNew/Exit과 보호534/505/캐시/설정·기존 증거 불변도 확인한다. 픽셀 차이는 격리 fixture의 조명 영향까지만 판정한다. 내부 useLights/lookup 소비·GUID7 무영향을 대리하지 않는다.

허용 새 보고는 `docs/verification/2026-10-02-renderer-normal-update-v6-luna-review.md` 하나이다. Unity/시험/Git/네트워크 실행과 기존 파일 수정은 금지한다. 실제 AC/P0/P1과 관측 한계를 짧게 보고한다. 전체 의존·회귀·복제·게시 승인은 별개이다.
