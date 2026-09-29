# C3 실제 Submit 동등 시험 이전 승인

2026-09-29. 아스트라 승인, 상태 `Approved`.
REQ-M5D7QC3-001/005/007, AC-M5D7QC3-006/009/010.

[솔 설계](../proposals/2026-09-29-c3-r7-submit-playmode-equivalent-transfer.md) SHA `CB0AAC702505ED4ECC68DC988852C8E27016FF945B70FF7606600350D28ED3A0` 및 [루나 설계 검수](../verification/2026-09-29-c3-r7-submit-playmode-equivalent-transfer-luna-design-review.md) SHA `2BD76ECED01DD1873E042DD64050058C8314A65A4F730FF2508A128E509C7D2A`, P0/P1=0을 근거로 설계의 동등 이전만 승인한다.

허용 경로는 기존 신규 Owner Edit/Play 시험 두 파일이다. 원본 지문 DF9FE80D362457ED8DEBB03556C644F8D669ADF03C20CBE74BFD5A695B691B3C 및 20B7EA2340402149B20DD0D7080FD4DFA1AFF400E701C6BE2050370E001BA3A4를 이력으로 먼저 보존한다. Edit 단일 AC006과 전용 관측 helper를 제거하고 기존 Play `AC006_ImmediateReadyFactoryDiscardsActualSubmitBeforeNewTake`에 모든 검증을 옮긴다. 정확 타입·조립·선언·서명으로 기존 정상 API와 실제 불투명 결과를 사용한다. 첫 실제 Submit=true 프레임 폐기, 원래 cursor/retained/taken 역사와 RequestTaken 상태, Phase=Ready, 다음 Submit=true·정상 take·Outcome=DecisionRequired·새 결정 참조를 모두 요구한다. Rearm 후 첫 true 발행 전 추가 빈 발행을 하지 않는다.

377행·91개 고정 행렬·180초 제한·런타임·설정·조립·friend·API·meta·자산·Q0 pin·기존 필수 562/51/610 원장은 불변이다. 새 파일 원장·정확 240/15 집중 선택·377행 원장의 소스 지문을 새 버전으로 동결할 수 있다. 원래 행 ID/내용/순서/권한/세대/파일 조건과 매핑·검증기 코드는 변경하지 않는다. 이전 실행과 원장은 보존한다.

별도 `gpt-6-sol` 구현과 `gpt-6-luna` 독립 검수 후 실제 강화 Play 사례를 우선 실행하고 최종 동일 소스의 필수 실행을 수행한다. 선택 이름·실제 종료·건너뜀/판정 불가·누락/중복·전후 지문을 검증한다. 관측은 Editor 경로와 일치할 뿐 단일 원인 확정이 아니며, 이 승인은 실제 통과나 C3/C4 수용이 아니다.
