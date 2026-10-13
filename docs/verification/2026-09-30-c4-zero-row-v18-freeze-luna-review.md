# C4 0행 복구 v18 동결 독립 검수

검토 대상은 v18 source manifest, input path list, 직접 사전 캡처다. 적용한 승인 문서는 `docs/specs/work-contracts/2026-09-30-c4-remaining149-zero-row-verifier-recovery-amendment.md`(SHA-256 `94E8B2F782C70DFE00E3299690B590BDDCBF181F20B68240DAADA8292252C59D`) 및 `docs/specs/work-contracts/2026-09-30-c4-zero-row-unexpected-id-successor-amendment.md`(SHA-256 `7EFA4CCCE58378125531FB47E13840C527411C75793EEFE59B511D521CCF11F9`)다. r2 독립 회귀 검토 `docs/verification/2026-09-30-c4-zero-row-regression-r2-luna-review.md`(SHA-256 `3EC6A60157CE215D82CE4328DE5AA82872D9EB9727CC277AA63247F30DC8EC05`)도 대조했다.

## 원장 및 입력

- `artifacts/c4-frozen-source-manifest-v18.json` SHA-256 `637CB631D8F25F630139F26A2B4AEA552D64F8E64EBA7526AAB3825079BC4569`: 186개 파일, 111개 허용 변경 기록, `WholeAccepted=false`. v17의 171개 경로와 순서는 보존되고, 두 기존 도구(`c4-verify-required-rows.ps1`, `c4-final-validation-queue.ps1`)의 현재 지문만 바뀌었다. 15개 신규 항목은 승인 계약·설계·검토·회귀 증거다. 186개 파일 모두 실제 경로와 SHA-256이 원장에 일치한다.
- `artifacts/c4-frozen-input-paths-v18.json` SHA-256 `69DE040ACA02BB02F454B45D09AF418E942278E395E366E54BFFE60AA54F0ABC`: 1,076개 경로와 사유가 일대일이며, 중복·누락·현재 존재하지 않는 경로가 없다. 경로는 대소문자 구분 정렬이고 사유표 순서도 일치한다. v17의 1,060개 경로와 사유를 모두 보존하고 v18 source manifest 및 신규 15개 증거 경로를 추가했다. 모든 manifest 경로가 입력에 포함된다. 입력 목록과 사전 캡처 자체는 입력으로 포함하지 않으며 제외 패턴에 해당하는 경로도 없다.
- v17에서 이어받은 `AllowedChanges` 설명은 보존됐다. 큐 도구의 설명은 과거 결과 경로 변경을 가리키지만, 현재 queue 및 verifier 지문은 원장에 고정되어 있고 이번 승인 계약과 독립 검토·회귀 기록도 신규 증거로 포함돼 현재 변경의 승인 추적이 가능하다.

## 사전 캡처 및 판정

`artifacts/c4-zero-row-preflight-v18.json` SHA-256 `2CFDD9B55D2D73CEB9539FDBAD5DD7AA7142AB31A3D6EEAC6A7B4C7B11885AF1`은 입력 목록을 정확히 결속한다. 캡처 수와 파일 수는 모두 1,076이며, 경로 집합·각 현재 파일 지문이 입력과 일치하고 중복·누락·추가 캡처가 없다. 별도 실행 보고의 사전 캡처 종료값은 0이다.

P0 0, P1 0. 원장·입력·캡처 결속에 차단 결함을 찾지 못했다. 계획 v14 발급과 큐/Unity 실행은 검토하지 않았으며, 본 판정은 실제 C4 실행 수용이나 전체 수용이 아니다.
