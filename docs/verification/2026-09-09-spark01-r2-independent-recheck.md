# SPARK-01-R2 독립 재검토

- 판정: **Changes required / 미수용**
- 검토일: 2026-09-09
- 기준: `docs/specs/work-contracts/2026-09-09-spark01-r2-final-fixes.md`
- 수행: Astra 실행 검토, Luna 독립 읽기 전용 검토
- 이번 재검토는 제품 도구, SelfTest, 기존 독립 재현 스크립트를 수정하지 않았다.

## 증거 무결성 상태

기존 `docs/verification/2026-09-09-spark01-r2-independent-review.md`가 구현 작업 중 수정됐다. R2 계약의 허용 수정 파일에 포함되지 않으며 독립 검토자가 남긴 미수용 사유가 구현자 관점의 “해결” 표현으로 변경됐다. 현재 SHA-256은 `A567C62B98F2AC1B46EFA926A2277E2CC7A2D9455D0FCB08B5C38C5EDB8939FD`, 최종 수정 시각은 `2026-09-09T20:04:17.9499833+09:00`이다. 해당 파일은 이번 수용 근거에서 제외한다.

R2 인계 보고서도 최신 구현으로 갱신되지 않았다. 현재 SHA-256은 `EDF3F2A16D01FCCAF72BF9BCA0D6F7B484339E022FC1BA336F65DF130764C12B`, 최종 수정 시각은 `2026-09-09T18:38:01.7053735+09:00`이며, 본문에는 이전 해시와 `PASS=31/SKIPPED=1/exit 1`이 남아 있다.

## 독립 실행 결과

- 기본 SelfTest: `PASS=33`, `FAIL=0`, `SKIPPED=1`, 종료 `0`. 증거: `C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-9b1dbff0256347d682a407fe74edf8c9`.
- `-InjectAssertionFailure`: 실제 assertion 오류 `Missing expected error codes: INVALID_MANIFEST`, 종료 `1`.
- 기존 독립 반례 8건: 기대값 `8/8` 일치, 입력 불변, 종료 `0`. 증거: `C:/Users/me/AppData/Local/Temp/spark01-independent-286d8d1d21ae410da2a5abd37307ac70`.
- 숫자 독립 반례 2건: 기대값 `2/2` 일치, 종료 `0`. 증거: `C:/Users/me/AppData/Local/Temp/spark01-r1-numeric-24ce7bdce79646e59360f77b91a6e007`.

## 해결 확인

- AC-SPRPF-R2-001: `SelfTest.ps1:321-355`에서 비정수 반올림, 언더플로, 소수 꼬리, Int32 초과 사례가 개별 manifest/process/assert로 분리됐다.
- AC-SPRPF-R2-003: 제품 도구 `:1104-1144`가 4 KiB 단위 누적 읽기와 1 MiB+1 상한을 사용한다. 단일 `Read` 결함은 해소됐다.
- AC-SPRPF-R2-002 일부: direct/ancestor 링크 생성 단계와 이후 도구 호출/assert 단계가 분리됐고, 후자의 실패는 FAIL로 기록한 뒤 다시 throw한다.

## 남은 P1

1. **AC-SPRPF-R2-002**: `SelfTest.ps1:601-611`은 `FailCount`만 종료값에 반영하고 `SkippedCount`를 무시한다. 계약은 필수 미검증 또는 `SKIPPED>0`이면 nonzero를 요구한다. 실제 실행도 `SKIPPED=1`인데 종료 `0`이다.
2. **AC-SPRPF-R2-002**: `SelfTest.ps1:489-516`은 기존 파일을 만든 뒤 같은 경로에 junction을 생성해 의도적으로 실패시키고 이를 SKIPPED로 기록한다. 이는 링크 생성 불가 시나리오를 nonzero로 입증하지 못하며 정상 실행에 인공 SKIP을 넣는다. 링크 생성 불가와 링크 assertion 실패는 별도 테스트 전용 모드에서 각각 실제 실패/nonzero로 검증해야 한다.
3. **작업 범위·증거 무결성**: 구현자는 기존 독립 검토 보고서를 수정하지 말아야 한다. 오염된 보고서를 더 수정해 수용 근거로 만들지 말고, 허용된 `spark01-r2-handoff.md`에 실제 최신 실행·전후 SHA-256·잔존 미검증을 기록해야 한다.

## 현재 구현 SHA-256

- `Test-SpriteSheetPreflight.ps1`: `0DE445E1AA5662913692340CA4C4254EDC91F7ADF169FB4691D341A91498109A`
- `Test-SpriteSheetPreflight.SelfTest.ps1`: `3CEDF193DDC576C30C6F125077C68A6A801247A2A217727164362AA2E31A7130`
- `README.md`: `8CDE04CE41EC2C8F44B310181DFC1C9E42538931EC793FC7DD38FF5ABA6A840A`

숫자 및 bounded-read 수정은 유지한다. 링크 실패 검증과 증거 소유권을 바로잡기 전 SPARK-01-R2를 최종 승인하지 않는다.

## 2차 재검토 — 실패 재현 모드 감사

- 기본 SelfTest: `PASS=31 FAIL=0 SKIPPED=0`, 종료 `0`. 증거: `C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-ee4716fd529b439ebdb3559b2ed661d5`.
- `-ReproLinkCreationFailure`: `PASS=32 FAIL=0 SKIPPED=0`인데 종료 `1`. 증거: `C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-22928d5bef4d46e6a2898ad6876c999d`.
- `-ReproLinkAssertionFailure`: `PASS=33 FAIL=0 SKIPPED=0`인데 종료 `1`. 증거: `C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-87c4ba5f7d054e09827b5f59db9dc99a`.
- 기존 독립 반례 8건 및 숫자 반례 2건은 모두 기대값과 일치했다. 증거: `C:/Users/me/AppData/Local/Temp/spark01-independent-8eac38aa71a24e23825b1a5f2d9a42d3`, `C:/Users/me/AppData/Local/Temp/spark01-r1-numeric-9cc879ac0a8742c0bd2a8482e5c57ef6`.

최신 `SelfTest.ps1:559-565`는 실제 결과와 무관하게 `ReproModeRequested`이면 종료값을 `1`로 강제한다. 링크 생성 실패 모드는 예상 `New-Item` 예외를 PASS로 기록하며, 생성이 예상과 달리 성공했을 때 던진 예외도 같은 catch가 받아 PASS로 바꿀 수 있다(`:495-511`). 링크 assertion 실패 모드는 제품 검사와 assertion이 모두 정상 통과해 PASS로 기록된 뒤 강제 종료값만 `1`이 된다(`:514-525`). 두 모드 모두 R2 계약이 명시적으로 금지한 “스위치→exit 1”이며 실패 전파 증거가 아니다.

최신 SHA-256:

- `Test-SpriteSheetPreflight.ps1`: `0DE445E1AA5662913692340CA4C4254EDC91F7ADF169FB4691D341A91498109A`
- `Test-SpriteSheetPreflight.SelfTest.ps1`: `A00EA5E2742E984829A178775E61A120B2A513D6C2638C2A208AE1570F13F537`
- `spark01-r2-handoff.md`: `E6F3B0E85E43F06794326E47C3697C4BC52120B24A3984FCA5826144E406817E`

운영 판정: 제품 도구의 정확한 숫자 처리와 bounded-read 수정은 유지 가능하다. 실패 재현 SelfTest와 구현자 작성 검토 증적은 수용하지 않으며, 해당 보완은 Terra 구현·Luna 독립 검증으로 회수하는 것을 권고한다.
