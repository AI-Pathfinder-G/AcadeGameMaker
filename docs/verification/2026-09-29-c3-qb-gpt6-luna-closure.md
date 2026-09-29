# C3 Q-B·owner 일관 설계 후속 폐쇄 검수

- 검수자·실제 호출 모델: Luna, `gpt-6-luna`
- 날짜: 2026-09-29
- 범위: 두 제안의 고정 지문을 기준으로 한 독립 정적 설계 검수. 소스·시험·Unity 실행·구현 수용은 수행하지 않음.
- Q-B 발급 handle 설계 SHA-256: `28926347FAA0E247726177578C3B3A79358993F9ED0FC61C9C5886563C5F5CD3` — 지정 지문과 일치
- owner 일관 단위 계획 SHA-256: `E68280CD03EA28B3F0CC9B60530FD88762BB27BBE24B966BAD2810D02DF1DC59` — 지정 지문과 일치
- 기준: Approved C3, ADR-0036, 이전 Q-B handle 후속 검수 P1

## 판정

P0=0, P1=0. 이전 후속 검수의 P1인 최초 세대 무-history와 successor의 현재 슬롯 pristine 범위 혼동은 폐쇄됐다. 최초 take에서는 owner 전체의 history 부재를 요구하고, successor에서는 prior append-only history 및 기존 `_takenRequest`/`RequestTaken`을 보존한 채 현재 epoch 슬롯만 비어 있음을 요구한다. successor 검사에 최초 no-history 조건을 재사용하지 않는다고 명시한다.

기존 Q-B legacy take 본문·최초 history는 유지된다. 새 C3 경로는 별도 actual take의 등록된 opaque handle만 intake 권한으로 사용한다. copied row나 미등록 후보는 값 기반 검색·pending 자동 탐색 없이 거부되며, clean mismatch는 take 전에 모든 관련 상태를 보존하고 lower capture/root 경계에 진입하지 않는다. 실제 take 뒤 발급·등록·양쪽 bind 중 실패는 기록을 보존하고 terminal close한다.

successor는 정확한 현재 슬롯과 checked epoch/fresh token을 사용하고 옛 기록을 덮거나 재활성화하지 않는다. presenter는 실제 새 cursor를 사용하며 `TryAdvance == false`를 대기 처리하고, 첫 유효 `true` frame은 검증 후 버린다. 그 frame에서 입력 해석·활성화·hit test·알림 해제를 하지 않고 baseline 격리를 유지한다.

구현 계획은 `Hub.Presentation.Unity -> Input.Unity -> Profile` 방향을 유지하고 Input.Unity에 Hub 역참조나 새 friend/asmdef를 두지 않는다. C4 report/identity 추출, C1 `Begin`, C2, live UI graph, scene/map/gameplay 실행은 명시적으로 제외한다. 승인 설계와 C3 추적 기준을 약화하는 내용은 발견하지 못했다.

## 수용 범위

이번 결과는 두 고정 제안의 설계 폐쇄에 한정된다. 구현 승인, 코드·시험 수용, C3 Verified, AC-M5D7QC3-007/008 전체 또는 C4 수용을 의미하지 않는다. 이후 구현은 별도 승인 범위와 실제 고정 소스 검수·필수 회귀·Astra 통합 수용을 거쳐야 한다.
