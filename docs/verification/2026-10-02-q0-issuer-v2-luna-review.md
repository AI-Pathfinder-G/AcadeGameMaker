# Q0 두 단계 합성 발급자 v2 독립 검수

Approved 계약 `docs/specs/work-contracts/2026-10-02-q0-issuer-v2-independent-review.md`의 REQ-PLAT-068..070, AC-PLAT-067/068을 검수했다. 핵심 모듈 SHA-256 `0F6937BD52732C49E979C8F8A3F897C81733DD21028BE6EEB194AC0B381D012A`, 시험 `01727BE84692D2A6280EEFDBD1A06E5EC7F3A1C12CCB1A7A64CB527982F60F75`, 기존 결과 `410D949D832243A35AC2D3401D2F8F16F3E6EED8F217B7E000A5084CCA6B2FC8`, 테라 보고 `2ED6262F93FCBD75794EF2B77DEC786902214FE201237096937E50103333B502`가 승인값과 일치한다.

## 판정

- **P0: 0. AC-PLAT-067/068: 합성 발급기 범위에서 관측 충족.** 동일 모듈·시험 소스를 `output/q0-issuer-luna-r1`에 복사해 1회 재현했으며 27/27 통과했다. 격리 결과 SHA-256 `956410C415EF3A48164B25D77F233090E3AD1C403E5AB9BF1453901AA9234BB6`; 원본 결과는 불변이다. 두 결과의 공개키 지문은 다르고 소스 지문은 동일하다. 서명 재사용 거부와 기존 공개 검증기 확인도 통과했다.
- 기존 핵심 15/15, 제공자 v2 10/10 및 V1 정확 소유 키 정리 증거는 읽기 대조해 보존을 확인했다. V1 정리 JSON SHA `AFD1763FD867B701A17452B1CB5BA30E6AFF40576AC1833B04F5D67C63366742`, 보고 SHA `4B65429CF50E6CC3039B05722917B6DDCFFFFCD8E404EC55FFEF07B357FA5FEB`; 추가 키 탐색/삭제는 하지 않았다.
- 소스 12–20행은 단조 Stopwatch TTL 만료 시 세션 제거·서명자/키 폐기, 22–61행은 CNG NullString null 이름·임시 키·내보내기 금지·실패 정리를 확인한다. 63–72행은 생성된 공개키 뒤 단일 authority SHA 바인딩과 재바인딩 거부, 74–101행은 계약/권위/문맥/계획/nonce 결속, 1회 서명 및 실패 시 폐기를 구현한다. 103–112행은 세션 종료 정리를 수행한다. 격리 결과도 비관리자·Microsoft Software Key Storage Provider·`SyntheticOnly=true`로 기록한다.
- **열린 P1: 외부 권위 발급·검증이 아님.** 바인딩 입력은 호출자가 제공하는 SHA 문자열이며 이 모듈은 그것이 승인된 권위 문서인지 외부 서명/고정 신뢰 루트로 검증하지 않는다. 본 검수는 실제 통합 책임자 개인키 발급, 현재 계획 승인, 운영 Q0 실행 연결을 증명하지 않는다. `DependencyClosure`, `PublicationApproved`, `WholeAccepted`, `LunaReviewed`는 모두 false로 유지한다.

개인키 내보내기·저장·복원, 원본 덮어쓰기, 운영키 발급 및 제품/Unity/Git/네트워크 실행은 없었다. 전체 의존·회귀·복제·게시 수용은 별도다.
