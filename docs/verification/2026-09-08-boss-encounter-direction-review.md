# 보스전 구성 결정 독립 검토

- Date: 2026-09-08
- Result: Documentation consistency PASS — Luna independent review, Astra integration; P0/P1/P2=0.
- User authority: ADR-0028; OD-M5D1-001 resolved for early single-screen Ordan.
- Reviewed: CONTEXT terms, boss-encounter-direction canon, game/narrative canon links, BOSS-F design constraints, NAR-00 reference and local wiki draft.

REQ-BOSSF-001..007의 문서 간 일관성을 확인했다. AC-BOSSF-001..004의 향후 설계/실전 검수 경로는 기록했지만 아직 실제 전장·후반 보스의 인수 기준을 실행·통과한 것은 아니다. 진보스의 라겐 정체성, NPC 자발적 공명 허용 범위와 비참전, 도언 기술 보존, 엔딩 자격 불변, 후반 구현과 수직 데모의 범위 분리가 일치한다.

변경은 문서뿐이다. 이번 반영으로 게임 코드·장면·프리팹·패키지를 변경하지 않았고 Unity 테스트를 새로 실행하지 않았다. 직전 구현 기준은 M5C2의 전체1047/1047 통과 증적이며 이 문서 검토를 새로운 런타임 테스트 수로 합산하지 않는다. 위키는 로컬 원고 반영이며 원격 게시하지 않았다.

후속 초기 전투장 연결의 기술 검토: 고정 카메라 중심(0,4.5), pixel18 중심(0,81), Z=-10 및 동일 최소/최대 경계로 기존 전투 지형을 유지할 수 있다. 보스 종료 안전 입력 잠금은 Run 성공/보상/선택과 분리해야 한다. 정확한 런타임·저작 변경은 별도 Approved 작업 계약에서 진행한다.

## 같은 날 후속 사용자 정정

후속 문서 독립 검토: Luna PASS, P0/P1/P2=0. REQ008..011과 패턴 지침, NPC 공명의 비공격적 기여, 정상 기술/편법 구분, 레벨 체계·수치의 미확정 경계가 일치함을 확인했다. Astra는 이 디자인 문서 정정을 수용한다.

위 최초 검토는 REQ001..007 시점의 이력이다. 이후 사용자는 진보스가 이벤트전이 아니라 중력 스킬 편법 없이 상대적 저파괴력 기술과 축적한 레벨·실력·숙련으로 승리해야 하는 더 어려운 결전임을 명확히 했다. ADR-0028 정정, 캐논, BOSS-F REQ008..011/AC005..007, 서사 캐논과 로컬 위키에 반영했다. NPC 공명은 싸울 기회를 만들고 도언이 직접 승리를 완수한다. 학습 가능한 고난도 패턴과 진엔딩 가능한 연대 구성의 승리 경로를 후속 실제 검증 대상으로 추가했으며, 아직 밸런스나 런타임 테스트를 실행한 결과는 없다.
