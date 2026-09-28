# VD-02 Unity M2B2B session driver·owner sink 증적

- 검증일: 2026-08-27
- 구현: Terra
- 통합 판단: Sol
- 독립 검증: Luna
- 판정: **PASS — M2B2B Verified, P0/P1 없음**
- 관련 요구사항: `REQ-WT-001`, `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-006`, `REQ-WT-008`, `REQ-MOV-004`
- 부분 증적 인수 기준: `AC-WT-001`, `AC-WT-003`, `AC-WT-004`, `AC-WT-005`, `AC-WT-006`, `AC-MOV-004`

## 수용 범위

- execution order `-200`에서 movement preflight → 선택적 단일 `Physics2D.SyncTransforms`·관측 capture → `TransferSession.Process` → 단일 final movement reflection
- exact tick-indexed internal input, removal의 capture 전 제외와 session 우선 처리, lifecycle input 소비
- target sink 거부와 checked capture overflow 시 session revision·player gravity·target body·sink 상태의 부분 변경 방지
- box owner의 base mass·gravity와 Q1000 impact multiplier 가역 적용·복구
- enemy·boss-payload modifier ID를 health·AI·base physics 소유 없이 격리한 sandbox owner stub
- 전체 snapshot 및 removal/press event scalar를 비교하는 30/60/144 render-group replay
- 새 public Unity component·provider·device input·검색 기반 registry 없음

## 실행 결과

| 시험 | 결과 | SHA256 |
|---|---:|---|
| `TestResults/vd02-m2b2b-driver-final2.xml` | 9/9 PASS | `1E3B41223EEE636B19289A87F32E3E6F83A9321DD83C2642F3D2DE9B9261951F` |
| `TestResults/vd02-m2b2b-transfer-unity-playmode-final.xml` | 19/19 PASS | `F7F80DB894FE0DD8AC990095DDD17E6BB0764EC89F22F3E4B4EC751A58397C90` |
| `TestResults/vd02-m2b2b-transfer-unity-editmode.xml` | 18/18 PASS | `814B2D0F21B6305205056A5352180F52523341615C9C3082F4F9376F99E14E64` |
| `TestResults/vd02-m2b2b-movement-playmode.xml` | 12/12 PASS | `E9C0267F170B485C1339154791D412C8FC1BA5E367D154F16D0CA1DC3E088F86` |
| `TestResults/vd02-m2b2b-transfer-m1-editmode.xml` | 17/17 PASS | `98C4FFDDF869D259CC720FF75018F2CFAD6389C67962E0600B4669476675A67C` |

## 핵심 소스 고정값

- `TransferSimulationDriver.cs`: `4E10F987D19AF38AA01D6400F7668B52C2375A6A0CABECFB0282BD8FA40072D9`
- `TransferSandboxOwnerSinks.cs`: `EE599FDA0B28FF010E37AD08500733FE07BF786E13CEB763C1D210CEB8BA3266`
- `TransferSimulationDriverPlayModeTests.cs`: `7DAA746BF7DC12844488B7533E0E5779ECCF0F09D02DE5332FA179C23A8DFFA0`

## 검토 이력과 통합 판단

첫 Unity 집중 실행은 테스트 마우스가 월드 X=1 target의 승인 카메라 투영점이 아니라 화면 중앙을 가리켜 5/9에 그쳤다. 런타임 계약을 바꾸지 않고 픽스처를 정확한 normalized grid X=1014로 교정한 뒤 9/9가 통과했다.

Luna의 첫 독립 검토는 30/60/144 trace가 일부 snapshot·event 필드를 생략한 점을 P1 증적 결함으로 판정했다. Sol은 최종 `TransferSessionSnapshot`, nullable `TransferAttemptResult`와 nested session, `TransferStateChanged`, `TransferCleared`의 모든 scalar 필드를 trace에 포함시켰다. 재실행 9/9와 전체 Transfer Unity PlayMode 19/19 이후 Luna는 P1 종료와 잔여 P0/P1 없음으로 최종 PASS했다.

Sol은 M2B2B를 수용한다. M2A·M2B1·M2B2A와 결합해 VD-02 Unity adapter milestone은 Verified다. 다만 실제 일반 적 반응과 실제 enemy·boss consumer에 해당하는 `AC-WT-002` 및 `AC-WT-005` 일부는 승인 계약대로 VD-03에 남는다. 따라서 이 문서는 전체 VD-02 기능 완료나 최종 UI 피드백 완료를 주장하지 않는다.
