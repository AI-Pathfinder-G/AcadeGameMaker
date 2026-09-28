# 세령 v8 H1 이동 제작 기록

2026-09-14. Astra 원본 선택·고정 배율 검토·통합, Terra 원본/가공 도구/목록 제작, Luna 독립 검토. 승인 계약: `docs/specs/work-contracts/2026-09-14-seryeong-v8-production.md`.

## 실제 산출물과 범위

- REQ-V8-001 / AC-V8-001: `images/seryeong/shoulder-v2/v8`의 탐험복 V1–V4, 정장 T1–T4, 숙녀복·긴 치마 L1–L4, 수영복 B1–B4를 사용했다. H1은 검은 긴 생머리다. V1 매듭 셔츠, V3 가죽 재킷, T4 닫힌 셔츠 깃, L1/L2/L4 긴소매, B2 단일 교차 장식을 수정했다. 선택 원본은 V1/V3 v3, T4/L1/L2/L4/B2 v2, 나머지 v1이며 이전 원본은 보존했다.
- REQ-V8-002 / AC-V8-002: 16종 × H1 × 24칸 = 384개의 이동 프레임 후보를 저장했다. 각 시트는 대기·걷기·달리기·점프/착지의 네 행이다. 전체 동작·양방향·다른 헤어를 완료한 조합은 0개다. v8의 192조합과 전체 432조합 요건은 축소하지 않는다.
- REQ-V8-003 / AC-V8-003: 몸체 64px 기준, 셀 128×128, 아틀라스 768×512. 정수리20/발바닥254, 원본 기준점150/254를 검토하고 해시에 결합했다. 선택된 V4 투명 PNG 16개, 행별 GIF 128개(원배율64개·4배64개)를 확인했다. 384칸 모두 비어 있지 않고 셀 경계 잘림은 0개다. 원본/아틀라스 해시가 모두 일치한다. `manifest/v8-h1-integration-check.json`에 실제 검사값을 저장했다.
- REQ-V8-004 / AC-V8-004: 새 의상의 보정값을 기존 의상 파일에 의존하지 않고 처리하도록 오프라인 도구를 확장했다. 의상·헤어·원본 해시 일치와 안전한 원본 파일명 검사를 적용했다. Unity 실행/런타임 연결은 수행하지 않았다.
- REQ-V8-005 / AC-V8-005: 모든 결과는 `source_only` 제작 후보다. 독립 검토는 `2026-09-14-seryeong-v8-review.md`를 따른다. 목록과 미리보기는 `images/sprites/seryeong-appearance-v3/manifest/v8-viewer.html`에 둔다.

전체 비교 PNG: `images/sprites/seryeong-appearance-v3/manifest/v8-h1-movement-overview.png`. 16종 걷기 비교 GIF: `images/sprites/seryeong-appearance-v3/manifest/v8-h1-walk-overview.gif`. 두 파일 모두 실제 V4 출력으로 구성했으며 Astra가 전체 비교 PNG를 확인했다. 제작 담당자의 모든 출력 쓰기 종료를 확인했다.

## 남은 검수와 제작

64px에서 B4 금색 상의와 피부의 색 대비가 약하며, 일부 머리·옷 가장자리의 보라색 픽셀은 팔레트/배경 잔색 구분 검토가 필요하다. 현재 파일을 최종 게임용 수용 자산으로 선언하지 않는다. H2–H12, 전투·스킬, 마을 동작, 도언과의 관계 반응, 서하 상점 반응, 방향별 검증 및 게임 연결은 남아 있다.
