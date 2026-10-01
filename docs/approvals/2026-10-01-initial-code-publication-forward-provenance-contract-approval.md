# 현재 바이트 독립 출처 기준 계약 승인

- 결정: 2026-10-01, 아스트라(`gpt-6-astra`). 사용자의 명시적 선택 B를 적용한다.
- 승인 계약: [최초 게임 코드 게시의 현재 바이트 독립 출처 기준 계약](../specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance.md), SHA-256 `3D928CEF5DD68F20B4FD13F25B7812167378B62332C198C6079476C17EF31E86`.
- 설계 원본: 솔(`gpt-6-sol`) Draft SHA-256 `6AADE94D85746E687FAB018C0E8720DD88E85C62DEFB633AF1EDA3492A643827`; 보정 검토본 SHA-256 `1883C27FA6E2D225643E6F61D4B8FEF02D01E3D0A48D96067248163E8B38B90C`.
- 독립 검토: 루나(`gpt-6-luna`) [초안 P1](../verification/2026-10-01-initial-code-publication-forward-provenance-luna-design-review.md) SHA-256 `50BFF37776012D19F00B551D2395164FAAD102EC617F8ECE3E4629E6F0BFB7AE`; [보정본 P0/P1=0/0](../verification/2026-10-01-initial-code-publication-forward-provenance-luna-rereview.md) SHA-256 `847DA156579069D7900709C5F0C05DD34272D25CA4B9665098D606DC82064630`.
- 추적: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`.

현재 501개 후보의 읽기 전용 바이트·출처·의존·원격 대조와 계약에서 지정한 새 증거 세 파일, 별도 루나 검토 한 파일의 작성만 승인한다. 역사적 직접 근거 61·공백 428, 기존 변경 10파일의 14개 미입증 전이와 완결 사슬 0/10은 원본대로 보존한다. 현재 바이트가 새 기준과 일치해도 과거 구현 바이트가 복원되거나 과거 승인으로 소급되지 않는다.

이번 승인은 파일별 게시 허용 목록, 게임 코드 변경 요청, 원격 병합, 깨끗한 복제, Unity 재실행, Q0 시험 변경을 허가하지 않는다. 테라의 조사 결과를 루나가 독립 대조하고 아스트라가 제한 수용한 뒤에야 정확 파일별 허용·제외와 후속 검증 계약을 별도로 결정한다.
