# 현재 바이트 출처 원장 v2 보정 계약 승인

- 결정: 2026-10-01, 아스트라(`gpt-6-astra`).
- 승인 계약: [v2 한정 보정](../specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance-v2-correction.md), SHA-256 `B6BCFD7C243053FC95E6BDF38E267CE13C01551B574AFDC239C33117C00A4E04`.
- 선행 원본: v1 원장 `artifacts/c4-initial-publication-forward-provenance-v1.json` SHA-256 `C5D9C98C1F44A9D5A68F8F5C573787CF539E6069D2CE5EB7F0F35BB36B753EBE`; [루나 v1 결과](../verification/2026-10-01-initial-code-publication-forward-provenance-result-luna-review.md) SHA-256 `D964D98D5C8D01FC08CE9FFE820DD9D9E30D0B60EAB71A4150090B658D3C1E56`, P0=0/P1=2.
- 설계: 솔(`gpt-6-sol`) 원본 Draft SHA-256 `2DB944C117F38509F6CD17B24249F8BA493DC9619090D4147B7CA7FDF3AAC306`, 보정 검토본 SHA-256 `426DD223C2B83B71969D423194728E91BB0DB1288DE3D36B685A9EBA4AFA5B6C`; [루나 독립 재검토](../verification/2026-10-01-initial-code-publication-forward-provenance-v2-correction-luna-review.md) SHA-256 `0C77B026BE155924314BB571EAA6857A3A7CA024EAE292A41C78D1B65E57C3C5`, 설계 P0/P1=0/0.
- 추적: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`.

테라 역할의 별도 `gpt-6-sol` 작업자에게 기존 v1과 역사 원문을 변경하지 않고 세 행의 오래된 참여 원장 참조를 역사 문맥으로 분리하며, 현재 독립 검토 근거 여덟 건을 정확 지문·줄·검토 종류로 다시 확인하는 읽기 전용 작업을 승인한다. 또한 시작·종료 UTC로 감싼 새 원격 `main` 조회, 정확 HEAD·트리·501경로·존재 blob·바이트 재대조와 변경 시 실패 정지를 승인한다. 지정된 v2 증거 세 파일과 별도 루나 검토 한 파일만 새로 작성한다.

v1의 P1은 v2 결과가 독립 검수되기 전까지 열려 있다. 파일별 포함·제외 및 게시 허용 목록은 계속 비어 있으며, 게임 코드 변경 요청·원격 병합·깨끗한 복제·Unity/빌드/Q0 실행은 이번 계약의 권한 밖이다.
