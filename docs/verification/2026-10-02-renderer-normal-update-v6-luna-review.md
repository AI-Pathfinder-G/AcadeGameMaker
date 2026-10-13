# 정상 갱신 렌더 v6 독립 검수

승인 계약 `docs/specs/work-contracts/2026-10-02-renderer-normal-update-v6-independent-review.md`의 REQ-PLAT-075/076, AC-PLAT-073/074만 읽기 대조했다. 결과 JSON SHA-256 `68B60F6446D6499746C4A179F8AF2B4D5BB7A992F6F83EAA2E6337DAD2EE8609`, 테라 보고 `14FD0694C62D39702ECF7E15D6BE237DA5A90B8CBD4889E791C30E747A8FC634`, 소스 `344ED438ACE70B8AEEB2844752CF1EF817AEA0126AB9A45C698D64B40A1D2056`, Unity 로그 `19075A71B9C5DB28430ECA9C6E4D89FBD14768B9AE873C8580C2202446153C43`가 계약과 일치한다. 실행 소스와 격리 복사본 지문도 일치한다.

## 판정

- **P0: 0. AC-PLAT-073: 관측 충족.** 소스 237–259행은 비공개 경계구를 읽어 중심/반경 및 실제 장면·등록·활성·계층 조건을 확인하고 내부 갱신 함수나 상태를 쓰지 않는다. JSON의 중심은 `(0,0,0)` 유지, 반경은 `0→3`, 정상 편집기 갱신 콜백은 총 4회(켜짐 전 2회, 꺼짐 후 2회)다. 동일 미리보기 장면 핸들, 읽기 전용 등록, 카메라 마스크 포함, 기본 정렬층, 켜진 두 조명과 일치하는 점 조명 반경도 확인했다.
- **AC-PLAT-074: 제한된 fixture 관측 충족.** 지정 Sprite-Lit 재질 GUID `a97c105638bdf8b4a8650670310a4cd3` 및 셰이더 GUID `e260cfa7296ee7642b167f1eb5be5023`, 명시 요청 각각 1회, 시작/종료 콜백 각각 2회, 두 RGBA SHA와 16픽셀 차이가 기록돼 있다. 소스 193–219행의 요청·readback 및 342–375행의 단계 전환/픽셀 비교가 기록 방식과 맞는다. RawObservation UTF-8 SHA-256을 재계산해 `940399B923DC2C77E34699828B4C5126C45A3FD1BE7E0F137B5BC0EB82EBAF4F`와 일치했고, 핵심 필드 15개가 파싱된 관측과 JSON 요약에 동일하다. 실제 RGBA 바이트는 보존되지 않아 픽셀 수의 바이트 수준 독립 재계산은 하지 않았다.
- **열린 P1: 내부 조명 경로 미입증.** 콜백 시점과 요청 후 모두 `_FalloffLookup`이 `UnityBlack`이다. 결과도 `UseLightsDirectlyObserved=false`, `FalloffLookupConsumed=false`, `RendererGuid7P1Closed=false`로 기록한다. 관측된 차이는 격리 fixture에서 정상 갱신 뒤 점 조명 영향만 보이며 내부 lookup 소비나 기존 Renderer GUID7 P1을 종결하지 않는다.

사전/사후 534·505 요약과 실행본505 지문이 일치하고 각 mismatch 목록은 비었다. 보호 파일·패키지·v5 증거 지문 맵은 사전/사후 차이 0이며, 현 보호 파일 및 패키지 경로의 재계산 지문도 불일치 0이다. 소스 383–412행에서 구독 해제, 임시 오브젝트·텍스처·미리보기 장면 정리, 새 결과 생성 및 종료 호출을 확인했다. 계약대로 실행은 재개하지 않았다.

이 검수는 증거 파일과 fixture 절차에 한정한다. 게임 전체 동작·의존 폐쇄·회귀·클린 복제·게시 권한은 수용하지 않는다. `DependencyClosure`, `PublicationApproved`, `RendererGuid7P1Closed`, `WholeAccepted`, `LunaReviewed`는 모두 거짓 상태다.
