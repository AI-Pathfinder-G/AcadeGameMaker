# VD-09 M5D7A 계약 독립 사전 게이트

- Date: 2026-09-13
- Reviewer: Luna
- Contract: [M5D7A profile v1 canonical encoder](../specs/work-contracts/2026-09-13-vd09-m5d7a-profile-v1-canonical-encoder.md)
- Final result: **PASS — P0=0, P1=0**

## 검토 이력

첫 검토는 M5D6 mismatch input이 구조적으로 유효하지만 VD-09 current profile 조건과 다른 상태를 encoder가 어떻게 처리하는지 불명확하다는 P1을 발견했다. 계약은 wrong asset ID와 binding schema `0/2/int.MaxValue`를 decoder/recovery가 재현할 수 있는 canonical candidate로 보존하되 current-compatible persisted-valid 또는 정상 save로 주장하지 않도록 보완했다. derived compatibility는 JSON에 포함하지 않고 후속 atomic writer가 repair/revision 증가 후 정상 commit한다.

second pre-gate에서 P1은 닫혔다. property order, progression revision projection, integrity 제외 hash/마지막 삽입, string/NFC/inner JCS outer escape, default/bypass/defensive copy, engine-free BCL과 allowlist도 일치했다.

## P2 구현 지시

- mismatch Encode가 compatibility를 호출해 거부/fallback하지 않는지 직접 테스트한다.
- mismatch candidate를 정상 save로 분류하는 API/문구가 없는지 정적 검토한다.
- lowercase 64 SHA, long/int invariant, UTF-8 no BOM/newline을 golden으로 고정한다.

## 결론

Astra 승인 뒤 exact allowlist로 구현 가능하다. 이 PASS는 encoder contract만 승인하며 decoder/file validity/atomic IO를 검증하지 않는다.
