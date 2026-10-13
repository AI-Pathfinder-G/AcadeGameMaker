# 패키지 요청·잠금 버전 차이의 기존 승인 문맥 대조

- 범위: 현 `Packages/manifest.json`과 `packages-lock.json`의 직접 버전 차이 두 건을 기존 승인·검증 문서와 대조한 읽기 전용 기록이다. `AC-PLAT-001/005`, `AC-M5D7PB-002/003`의 새 실행 통과 판정이나 깨끗한 복제 검증이 아니다.
- 현 실물: [패키지 캐시 예비 조사](2026-10-01-initial-code-publication-package-owner-astra-mechanical-check.md)의 `AC-PLAT-025`에서 렌더 파이프라인 요청 `17.3.0`·잠금/현 캐시 `17.6.0`, 시험 도구 요청 `1.6.0`·잠금/현 캐시 `1.8.0`을 정확히 읽었다.

1. [Unity 6000.6.0f1 기준 계약](../specs/work-contracts/2026-09-06-unity-6000-6-baseline-update.md) SHA-256 `CAEF795932861B0D7F5E31F4CDD85BF21C9685DB83E7FD7C1ACE5C2D6835E2B6`은 기존 요청 값 URP `17.3.0`, Test Framework `1.6.0`을 편집기 변경만을 이유로 올리지 말라고 명시했다. 따라서 현재 차이를 발견했다는 이유만으로 요청 파일을 수정할 근거는 없다.
2. [고정 방 카메라 계약](../specs/work-contracts/2026-09-08-vd07-m5c2-fixed-room-camera-driver.md) SHA-256 `E95920F22B51065A615E26824CC6219C9855400C2F778162ADFA810DD6E177AD`은 실제 해석된 URP `17.6.0`과 요청 `17.3.0`의 차이를 **명시적으로 기록**하고, 그 해석 소스의 API에 맞춰 구현·검증했다. [UI 패키지 기준 계약](../specs/work-contracts/2026-09-20-vd09-m5d7p-b-ui-package-baseline.md) SHA-256 `BEEE9E7DFF9F4717A751220D70EADC00981F92618EB1A6EC93A9289F34BB5774`는 작업 전부터 잠금 파일이 원격 HEAD와 넓게 달랐고 당시 작업 바이트를 보존하도록 했다. [후속 시험 계약](../specs/work-contracts/2026-09-27-m5d7m-duplicate-liveness-recovery.md) SHA-256 `A55294926DA66676AAC96E55902F18C0843BE1071C9837E5C8D8A2C22FFCBD06`은 실제 Test Framework `1.8.0` 소스를 지칭한다.

따라서 두 버전 차이는 이번 조사에서 새로 발생한 변경이 아니며, 현 로컬의 실제 해석 소스가 기존 구현 문맥과 맞는다는 정적 근거가 있다. 이것은 **깨끗한 복제가 같은 버전을 해석한다는 증명은 아니다**. 요청·잠금 중 어느 쪽을 새 게시 기준으로 고를지는 격리된 복제에서 패키지 해석·가져오기와 승인 시험을 실제 수행한 뒤 별도 계약으로 판정한다. 현재 501 후보 바이트와 패키지 파일을 수정하지 않는다.
