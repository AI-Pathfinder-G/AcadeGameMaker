# C3 편집 fixture 정확 참조 결속 설계 검수

- 검수자·실제 모델: 루나, `gpt-6-luna`
- 대상: `docs/proposals/2026-09-29-c3-edit-fixture-exact-binding.md`
- SHA-256: `1F2B14D055F232CE737E06AAE40383B79DD0683E7FC66ACFBBDC45367149F98D` (일치)
- 범위: 실제 선언과 서명 표의 정적 대조. 소스·시험 수정 및 실행 없음.

**판정: P0=0, P1=0.** 표는 실제 Hub `ConfigureForAuthoring`, owner `AcceptNewGame`/Confirm/Cancel/Rearm, Q-B take/예약/commit, presenter successor 준비의 인수·반환 형식과 일치한다. `TryTakeNewGameForConfirmation`의 `out H`를 by-ref 형식과 `IsOut` 양쪽으로 검증하는 점, receiver가 아닌 정확한 declaring type과 `DeclaredOnly`를 요구하는 점은 null 및 동명 오버로드의 모호한 선택을 막는다. 각 읽기 property/field도 실제 선언된 형식과 맞으며 읽기 전용이다. 목록 밖 호출·상속/name-only fallback이 없고, 반환 opaque 참조를 재구성하지 않는다.

기존 copied-row 반례만 정확한 `AcceptNewGame(H,A,R) -> Z` 메서드를 먼저 고정한 뒤 잘못된 데이터 row를 직접 `Invoke`하여 binder `ArgumentException`을 관찰한다. 정상 helper가 인수를 사전 거부하거나 row를 H로 변환하지 않도록 한 설계는 기존 negative 증거를 보존한다. 새 helper의 실제 구현 지문 검수와 시험 실행은 별도 필요하다.
