# 2026-09-09 SPARK-01 Handoff

## 1) 수정 파일

- `qa/tools/sprite-preflight/Test-SpriteSheetPreflight.ps1`
- `qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1`
- `qa/tools/sprite-preflight/README.md`
- `qa/tools/sprite-preflight/example.manifest.json`
- `docs/verification/2026-09-09-spark-sprite-preflight-handoff.md`

## 2) REQ 요약

- REQ-SPRPF-001: 입력/출력 경로와 원본 보존 조건을 로컬 경로로 제한하고 UNC/와일드카드/`..`/재parse point를 거절했으며, 기존 결과 파일 덮어쓰기를 금지해 `REPORT_IO_ERROR`/`INVALID_PATH`를 반환.
- REQ-SPRPF-002: manifest는 v1 스키마에 맞지 않거나 누락/중복 키·타입/범위 위반·id 형식 위반을 `INVALID_MANIFEST`/`UNSUPPORTED_VERSION`로 거부.
- REQ-SPRPF-003: PNG는 `FileStream`으로 33바이트만 읽어 signature·IHDR length/type·width/height 범위를 검사하고 CRC/픽셀 디코딩은 수행하지 않음.
- REQ-SPRPF-004: `sheetWidth%frameWidth`, `sheetHeight%frameHeight` 체크 후 columns/rows/frameCount를 정수 범위 기반으로 계산하고, 범위를 벗어난 프레임 인덱스를 `FRAME_OUT_OF_RANGE`로 검증.
- REQ-SPRPF-005: 보고서는 고정 스키마(`schemaVersion`, `scope="HeaderAndGrid"`, `passed`, `checksNotPerformed`, `sheetWidth/Height`, `columns/rows/frameCount`, `sheetSha256`, `manifestSha256`, `animations`, `errors`)으로 UTF-8(BOM 없음, 압축 JSON) 출력.
- REQ-SPRPF-006: 종료코드 매핑은 0/1/2로 고정했고, 실패 유형은 content-error와 path/I/O를 구분해 전달.
- REQ-SPRPF-007: SelfTest는 별도 프로세스 호출로 성공/실패/경계/보존/결정론성까지 검증.

## 3) 실행한 명령 (정확한 명령)

- 도구 실행(예시):
  - `pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.ps1 -SheetPath <PNG> -ManifestPath <JSON> -ReportPath <NEW_JSON>`
- SelfTest 실행:
  - `pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1`
- 실행 환경:
  - `pwsh` (PowerShell Core 7)
  - SelfTest 실행시 PowerShell 프로세스 종료 코드를 기준으로 검증

## 4) AC별 자체 테스트 결과

- AC-SPRPF-001: PASS (정상 통과 시나리오 1건)
- AC-SPRPF-002: PASS (manifest 오류 시나리오 2건)
- AC-SPRPF-003: PASS (PNG header/바이트 부족 시나리오 2건)
- AC-SPRPF-004: PASS (격자 불일치, 범위 밖 인덱스 2건)
- AC-SPRPF-005: PASS (동일 입력, 서로 다른 출력 경로 바이트 동일성)
- AC-SPRPF-006: PASS (기존 보고서 보호, 경로 리터럴, UNC 거부)
- AC-SPRPF-007: PASS (독립 프로세스 실패 유도 케이스 포함)

- 실행 요약:
  - `SELFTEST completed PASS=13 SKIPPED=0`
- AC별 개별 라인: SelfTest 표준출력에 `AC-SPRPF-XXX` 단위로 기록

## 5) 임시 증거 경로

- `TemporaryEvidenceDir=C:\Users\me\AppData\Local\Temp\sprite-preflight-selftest-46426f181f534ed898196d19769ec931`

## 6) 미검증 범위

- 실제 PNG 전체 디코딩, CRC/IDAT 무결성, 투명도·팔레트·픽셀 품질 검사
- Unity 임포트 설정, 라이선스 확인, Gameplay 연동
- 게임 코어/씬/관계/엔딩 로직 연계 검증

## 7) 남은 문제 / 다음 검토자 확인사항

- 전체 게임 QA, 실 자산 임포트 승인, 외부 도구 연동은 SPARK-01 범위 바깥입니다.
- SelfTest는 현 시점에서 재현 가능한 환경에서 통과(AC 전부 PASS)했습니다. 추가 검토에서는 스펙 외 항목을 별도 AC로 이어받아 처리해주세요.
