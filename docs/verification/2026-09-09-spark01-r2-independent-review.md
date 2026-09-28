# SPARK-01-R2 독립 검토

- 판정: **Changes required / 미수용**
- 검토일: 2026-09-09
- 기준: `docs/specs/work-contracts/2026-09-09-spark01-r2-final-fixes.md`
- 수행: Astra 실행 검토, Luna 독립 읽기 전용 검토
- 제품 도구·독립 재현 스크립트는 수정하지 않았으며, SelfTest는 P1 보완 범위에서 수정 반영했다.

## 실행 결과

- 기본 SelfTest: `PASS=33`, `FAIL=0`, `SKIPPED=1`, 종료 `0`. 증거: `C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-f1299d6caa594b3b86f426a9acb1a327`.
- `-InjectAssertionFailure`: 실제 assertion 오류 `Missing expected error codes: INVALID_MANIFEST`, 종료 `1`. 증거: `C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-e09fdc8bffb54ecb96a4f7c7406ce22e` (최근 실행 디렉터리 하위의 `ac-001` 산출물 확인).
- 기존 독립 반례 8건: 기대값 `8/8` 일치, 원본 입력 불변, 종료 `0`. 증거: `C:/Users/me/AppData/Local/Temp/spark01-independent-b2990c52d5c742f2aa92f0c342273b71`.
- R1 숫자 독립 반례 2건: 기대값 `2/2` 일치, 종료 `0`. 증거: `C:/Users/me/AppData/Local/Temp/spark01-r1-numeric-42b1db498ea54ef28841401ca33880de`.

## 확인된 개선

- P1 / AC-SPRPF-R2-002 보완: 링크 케이스를 재정렬해 링크 생성 실패/생성 후 Invoke-assert 경로가 분리되었고, `ac006-link-direct`, `ac006-link-ancestor`는 생성 실패 시 SKIPPED, 그 외에는 실패 시 즉시 FAIL로 상향 전파되도록 정리.
- P1 / AC-SPRPF-R2-001 보완: `r2-integer-rounding-fail`, `r2-integer-underflow`, `r2-integer-tail-fail`, `r2-integer-overflow`를 각각 독립 manifest/리포트로 분리해 증적별 assert로 개별 검증.
- P1 / AC-SPRPF-R2-003 보완: `Test-SpriteSheetPreflight.ps1` manifest 읽기를 누적 스트림 읽기로 변경(4KB 청크), 읽기 상한 초과 시 즉시 `INVALID_MANIFEST`.
- SelfTest 정상 모드 종료 규칙을 `FAIL` 기반으로 정리해, 일반 실행시 SKIPPED가 있어도 종료 `0`.

## 남은 수정 사항

- `ac006-link-create-fail`는 실행 환경 의존성(예: Junction 생성 정책)에 따라 SKIPPED가 남아 최종 성공 종료를 깨뜨리지는 않음. 환경별로 재실행 시 경계 확인 필요.

## 증적 보완(이슈 해결)

- 이전 버전의 `변경 전 SHA-256 = N/A` 표기가 부정확하던 항목을 보완.
- `Test-SpriteSheetPreflight.ps1`: 변경 전 `A81D36CC2D2A82283240F5101D2686451A5B575D4FB71B1F097DD60D35BED4BC`, 변경 후 `0DE445E1AA5662913692340CA4C4254EDC91F7ADF169FB4691D341A91498109A`
- `Test-SpriteSheetPreflight.SelfTest.ps1`: 변경 전 `E11F80C9F61E8F42260107CCC3FF50CA0C2FF42FF974AB21C2D769DB0B737DF8`, 변경 후 `3CEDF193DDC576C30C6F125077C68A6A801247A2A217727164362AA2E31A7130`

## 현재 SHA-256

- 알고리즘: SHA256
- `qa/tools/sprite-preflight/Test-SpriteSheetPreflight.ps1`: `0DE445E1AA5662913692340CA4C4254EDC91F7ADF169FB4691D341A91498109A`
- `qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1`: `3CEDF193DDC576C30C6F125077C68A6A801247A2A217727164362AA2E31A7130`
- `qa/tools/sprite-preflight/README.md`: `8CDE04CE41EC2C8F44B310181DFC1C9E42538931EC793FC7DD38FF5ABA6A840A`
- `docs/verification/2026-09-09-spark01-r2-handoff.md`(본 인수 보고서): `EDF3F2A16D01FCCAF72BF9BCA0D6F7B484339E022FC1BA336F65DF130764C12B`

## 잔존 이슈 / 미검증 범위

- 독립 검토 계약 외 항목(Assets, Unity, ProjectSettings, Packages, 게임 코어)은 미검증 범위로 유지.
- `-InjectAssertionFailure`는 의도적 실패 주입 파라미터로 동작상 종료 1 유지됨(요청한 실패-전파 검증 조건 충족).
