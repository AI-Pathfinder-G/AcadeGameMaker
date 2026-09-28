# M5D1 독립 검증 사전 감사 — BLOCKED / 증적 보완 필요

- 검증일: 2026-09-10
- 기준 계약: `docs/specs/work-contracts/2026-09-10-vd07-m5d1-ordan-playable-terminal-transition.md`
- 계약 권위: Astra
- 구현 소유: Terra
- 예정 독립 검증: Luna
- 이번 실행: Luna Reserve 호출 없이 수행한 메인 에이전트의 읽기 전용 사전 감사
- 판정: **M5D1 Verified 아님**

## 범위와 실행

- M5D1 runtime/builder/validator/test 파일의 정적 구조 대조
- QA catalog 일반 실행 및 self-test
- Unity 6000.6 환경 preflight
- `OrdanBossEncounterAuthoringTests` focused EditMode 실행 시도
- 코드·씬·프리팹·프로젝트 설정은 수정하지 않음

## 결과

| 항목 | 결과 | AC |
|---|---|---|
| QA catalog | PASS — `13 scenarios / 68 AC` | QA 도구 사전조건 |
| QA catalog self-test | PASS — `7` 변이 검사 | QA 도구 사전조건 |
| Unity preflight | PASS — Editor 6000.6.0f1, Licensing Client 1.18.3 정렬, entitlement 존재 | AC-PLAT-001/002/010~012 |
| Unity focused EditMode | **BLOCKED** — 테스트 시작 전 `Licensing is not yet initialized`; 결과 XML 미생성 | AC-M5D1-006 |
| M5D1 구조 정적 대조 | 부분 확인 — requester `-211`, router `-210`, authored initial mode 및 builder/validator 연결 코드 존재 | AC-M5D1-001 |
| M5D1 requester 실행 증적 | 미확인 — Unity 실행 차단 | AC-M5D1-002~005 |

Focused 실행 증적 디렉터리:

`C:/Users/me/AppData/Local/Temp/luna-m5d1-edit-20260910-113103`

로그에는 Licensing Client 서명 검증 코드 `10`, entitlement group `0`, 이후 `Licensing is not yet initialized`가 기록됐다. 이는 제품 테스트 실패가 아니라 Unity 테스트 실행 환경 차단으로 분류한다.

## 증적 공백

현재 소스 검색에서는 다음 M5D1 전용 실행 검증 참조를 찾지 못했다.

- `LatestCompletionReceipt`를 검증하는 테스트
- requester의 `InitializeForTests` / `AdvanceForTests`를 호출하는 테스트
- death source tick `t`에서 정확히 `Transition@t+1`을 한 번 요청하는 전용 assertion
- duplicate, disable-reenable, overflow, conflicting pending request의 requester-level 전용 assertion

기존 authoring/terminal 테스트가 관련 기반 동작을 다루지만, 위 항목의 부재를 Unity 실행 차단 상태에서 추정해 통과 처리하지 않는다.

## AC별 사전 판정

- `AC-M5D1-001`: **부분 확인**. builder/validator와 authored binding 정적 경로는 존재하나, 실행 결과와 byte-stable 증적은 없음.
- `AC-M5D1-002`: **미검증**. 정상 no-op 및 잘못된 terminal 증거 거부의 실행 증적 없음.
- `AC-M5D1-003`: **미검증**. 정확한 `t+1` locked empty frame의 실행 증적 없음.
- `AC-M5D1-004`: **미검증**. immutable completion proof와 재요청 방지의 실행 증적 없음.
- `AC-M5D1-005`: **부분 확인**. authored camera profile/bounds 정적 assertion은 있으나 640×360·2560×1440 completed-camera DTO 실행 증적 없음.
- `AC-M5D1-006`: **BLOCKED**. focused Unity 실행이 Licensing 단계에서 중단됐고 전체 EditMode/PlayMode zero failure/skip 증적이 없음.

## 다음 조치

1. Terra가 M5D1 계약에 맞는 requester/end-to-end 전용 EditMode·PlayMode assertion과 인계 증적을 보완한다.
2. Unity Hub가 관리하는 동일 일반 사용자 세션에서 Licensing 초기화를 회복한다. 라이선스 파일·pipe ACL·바이너리를 임의 변경하지 않는다.
3. Luna가 focused 및 전체 EditMode/PlayMode를 독립 실행하고 AC별 XML·명령·Unity 버전·해시를 기록한다.
4. Astra가 Luna 보고서와 계약 범위를 확인한 뒤에만 최종 통합 수용한다.

이번 문서는 M5D1 구현 수용이나 Luna 최종 판정을 의미하지 않는다.
