# C3 최초 실제 입력 선택 준비 승인

2026-09-29. 아스트라 승인, 상태 `Approved`.
REQ-M5D7QC3-001/005/007, AC-M5D7QC3-001/006/009/010.

[솔 제안](../proposals/2026-09-29-c3-r8-play-initial-selection-order.md) SHA `DE355A7010232DE4DD1CD5975E7EA9C830A26C591A2199C062D1C1636209F9E1` 및 [독립 설계 검수](../verification/2026-09-29-c3-r8-play-initial-selection-order-luna-design-review.md) SHA `FC060D5D4851467CAD04EA91BCA3FD953578FDE26B22590DBF7ED060ECE72FE7`, P0/P1=0을 근거로 승인한다.

기존 Play Owner 시험 `SelectInitialNewGame`의 최초 선택 전에 정상 released `Publish()` 한 번만 추가한다. 원래 비회복 Down의 실제 NavigateChanged 및 음수 NavigateYQ4096, 실제 제출 이후 RequestReady와 실제 요청 Item=NewGame을 정상 assertion으로 확인한다. 정확 Q-B 선언 `_request` nullable 타입, boxed 원본 struct의 정확 선언 `_item` enum 타입을 검증해 읽는다. 기존 정확 읽기 도우미를 활용하고 이름 단독 fallback이나 상태 대입을 추가하지 않는다. 기존 callback·입력 권한·CWT·프레임을 제조하지 않는다. 정상 입력 갱신과 실제 Router 단계만 사용한다.

현재 Play 원본 `FC431157BEBF276EDA2C36ACF7682E2C3144A999AF39A18375D492B1D0D116A2`와 원장을 먼저 보존한다. 초기 중립 발행은 최초 선택 경계에만 위치하며 Cancel/Rearm 이후 첫 실제 Submit 전에 추가 빈 발행을 하지 않는다. 나머지 전체 검증·순서와 180초 제한·240/15 이름·377행/91분할·런타임·조립·friend·설정·meta·필수 562/51/610 선택은 유지한다. 새 R9 지문/선택/원장만 생성하며 기존 증거와 생성기/기본 출력은 불변이다.

별도 `gpt-6-sol` 구현 후 `gpt-6-luna` 독립 검수가 필요하다. 최종 소스의 probes 2와 remaining 13을 모두 실행해 공용 helper 영향 검증을 수행한다. 정확 이름·실제 종료·실패/건너뜀/판정 불가·전후 지문을 확인한다. 초기 요청 Item은 이전 실행에서 미관측이므로 원인 확정이나 이전 실패 수용을 주장하지 않는다. 이 승인은 시험 준비 보정에 한정하며 제품 수정·C3/C4 수용이 아니다.
