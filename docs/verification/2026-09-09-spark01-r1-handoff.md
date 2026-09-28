# SPARK-01-R1 인계 보고서

- 작업일시(요청 기준): 2026-09-09
- 작업범위: `docs/specs/work-contracts/2026-09-09-spark01-r1-correctness-fixes.md`의 F01~F07 대응 수정만 수행
- 실행 툴: `pwsh 7.6.5`
- 인수일시: 2026-09-09

## 변경 파일 및 해시

- `qa/tools/sprite-preflight/Test-SpriteSheetPreflight.ps1`
  - 변경 전 해시: `N/A` (경로 미추적/HEAD 없음)
  - 변경 후 해시: `e3f1393dce30355c6f02dc4841baf3272ad62ffb`
- `qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1`
  - 변경 전 해시: `N/A` (경로 미추적/HEAD 없음)
  - 변경 후 해시: `11a65c0c7e841c8dca430e10e825d2e2fb495ebd`
- `qa/tools/sprite-preflight/README.md`
  - 변경 전 해시: `N/A` (경로 미추적/HEAD 없음)
  - 변경 후 해시: `588a203416f8fe4c3173c5e3a077f91af5a5ab18`
- `docs/verification/2026-09-09-spark01-r1-handoff.md`
  - 신규 작성

## F01~F07 대응 현황

- F01 (`AC-SPRPF-R1-001`): `ParseRoot`에서 하위 파싱 이슈(`ParseAnimation`/중첩 파싱)를 루트 이슈와 병합.
- F02 (`AC-SPRPF-R1-001`): 루트 객체 뒤 트레일링 토큰을 `reader.Read()`로 재검사해 JSON 후행값을 거부.
- F03 (`AC-SPRPF-R1-001`): 정수 JSON 숫자 판정 로직을 `TryGetInt32` 실패 시 `TryGetDouble` 정수값 판정으로 확장 (`32`, `32.0`, `1e0` 허용).
- F04 (`AC-SPRPF-R1-002`): PNG IHDR 길이/필드 계산을 `Get-UInt32BigEndian`로 폭넓은 비트 연산 처리.
- F05 (`AC-SPRPF-R1-005`): 결과 `animations`/`errors`를 `Ensure-JsonArray`로 강제 배열 직렬화, 빈 배열도 `[]`로 고정.
- F06 (`AC-SPRPF-R1-004`): 경로 검증에서 입력/리포트 경로의 조상 전체를 순회해 reparse point 검사 수행.
- F07 (`AC-SPRPF-R1-005`): SelfTest 케이스 보강(필수 실패/성공 경계, 링크 재현, 주입모드 실패 경로), 링크 테스트와 assert 실패 처리 분리.

## 실행 명령 및 결과

### 1) SelfTest
- `pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1`
- 종료코드: `0`
- Evidence: `C:\Users\me\AppData\Local\Temp\sprite-preflight-selftest-db4bed4e54334fe796f43797023b89c1`
- 결과: `PASS=26`, `SKIPPED=0`
- 출력 예시: `AC-SPRPF-001`~`AC-SPRPF-007` 항목 전체가 `PASS`

### 2) SelfTest(고의 실패 주입)
- `pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1 -InjectAssertionFailure`
- 종료코드: `1`
- Evidence: `C:\Users\me\AppData\Local\Temp\sprite-preflight-selftest-804a058ccd9b4df29a7f0ec541c32645\ac-001`(주입 실행 결과가 쓰인 디렉터리)
- 결과 메시지: `SELFTEST_FAIL: Missing expected error codes: INVALID_MANIFEST`
- 목적: 의도적 실패 주입이 nonzero를 반환함을 확인

### 3) 독립 재현
- `pwsh -NoProfile -File qa/reviews/2026-09-09-spark01-repro.ps1`
- 종료코드: `0`
- Evidence: `C:\Users\me\AppData\Local\Temp\spark01-independent-9787ce96c83a4b118c760796d7ab8b99`
- 8건 모두 `matches=true`, 입력 해시 불변(`inputUnchanged=true`)

## AC/Fx 실행 매트릭스

- AC-SPRPF-001: PASS
- AC-SPRPF-002: PASS
- AC-SPRPF-003: PASS
- AC-SPRPF-004: PASS
- AC-SPRPF-005: PASS
- AC-SPRPF-006: PASS
- AC-SPRPF-007: PASS (주입 모드에서 기대한 실패가 비정상 종료로 반영)
- F01~F07: 모두 해결 조치 반영(코드/케이스 기반)

미실행 항목: 없음.

## 미검증 범위

- Unity 임포트/실제 이미지 픽셀 디코딩/런타임 에셋 등록 검증
- 실 자산 파이프라인 최종 승인 절차
- PNG CRC/IDAT 무결성(설계상 검사 범위 외)

## 잔존 이슈 / 주의

- 셀프테스트 주입 모드 실패 시 점검 메시지가 `SELFTEST_FAIL`만 표준 에러로 출력되어 증빙에 evidence 디렉터리 자체출력은 누락될 수 있음. 종료코드와 주입 디렉터리 존재로 대체 증빙
- `qa/tools/sprite-preflight` 및 일부 문서 경로는 현재 작업트리에서 미추적 상태이므로, 베이스 해시 비교는 불가
- 구현 완료, 전체 게임 QA 완료로 해석하지 말 것. 독립 검토/최종 승인 대상은 Astra
