# SPARK-01-R2 Terra recovery — Luna 독립 검증

- 판정: **PASS / Verified**
- 검증일: 2026-09-10
- 구현: Terra
- 독립 검증: Luna
- 최종 수용: Astra
- 기준: `docs/specs/work-contracts/2026-09-09-spark01-r2-final-fixes.md`
- 이전 구현자가 수정한 `2026-09-09-spark01-r2-independent-review.md`는 수용 근거에서 제외했다. 이번 검증 중 제품·SelfTest·기존 독립 재현 파일은 수정하지 않았다.

## AC 결과

| AC | 결과 | 독립 증거 |
| --- | --- | --- |
| AC-SPRPF-R2-001 | PASS | 숫자 사례가 독립 manifest/assert로 분리됐고 기존 숫자 재현 2/2가 기대 종료값과 일치했다. |
| AC-SPRPF-R2-002 | PASS | 정상 실행은 33/0/0·exit 0. 일반 assertion 실패는 FAIL=1·exit 1, 링크 생성 불가는 SKIPPED=1·exit 1, 링크 assertion 실패는 실제 `Assert-Result` mismatch와 FAIL=1·exit 1이다. 종료값은 FAIL/SKIPPED 카운터에서만 계산된다. |
| AC-SPRPF-R2-003 | PASS | 제품 도구는 4 KiB 반복 read와 1 MiB+1 상한을 사용하며 정확히 1 MiB/1 MiB+1 경계가 기대대로 동작했다. |
| AC-SPRPF-001~007 및 R1 회귀 | PASS | 기존 독립 일반 반례 8/8, 숫자 반례 2/2 일치. 정상 direct/ancestor junction 거부도 각각 제품 종료 2로 확인했다. |

## Luna 독립 실행

- 기본 SelfTest: `PASS=33 FAIL=0 SKIPPED=0`, exit `0`. 증거: `C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-d90175eee1b6482880f0b10db8aef754`.
- `-InjectAssertionFailure`: 실제 assertion 오류, `PASS=33 FAIL=1 SKIPPED=0`, exit `1`. 증거: `C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-687a0f3324c64d62acc959077686eea2`.
- `-ReproLinkCreationFailure`: 실제 junction 생성 불가, `SKIPPED=1`, exit `1`. 증거: `C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-69de8413e4574ed5ba38ba7869d67cdb`.
- `-ReproLinkAssertionFailure`: 실제 `Assert-Result` mismatch, `FAIL=1`, exit `1`. 증거: `C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-49ed7ddec57b47238e49cde79ab7ab76`.
- 기존 독립 일반 반례: 8/8 일치, exit `0`. 증거: `C:/Users/me/AppData/Local/Temp/spark01-independent-3b7aa5a48b4e485b8e13f50a827f45cb`.
- 숫자 독립 반례: 2/2 일치, exit `0`. 증거: `C:/Users/me/AppData/Local/Temp/spark01-r1-numeric-ffed11c7c3cc45fdb7614c402ce040c4`.

## 정적 검토와 해시

- 무조건적인 mode→exit 1 분기는 제거됐다. `SelfTest.ps1:591-598`은 FAIL 또는 SKIPPED 카운터만으로 종료값을 정한다.
- 링크 생성 catch와 이후 copy/tool/assert가 분리됐고, assertion 실패는 FAIL로 전파된다(`SelfTest.ps1:242-281`).
- 제품 도구 SHA-256: `0DE445E1AA5662913692340CA4C4254EDC91F7ADF169FB4691D341A91498109A`.
- SelfTest SHA-256: `C71B34B7C898E8953921222EB083350473886DA540DD40014F4197F25E400A61`.
- 위 해시는 Terra 인계 보고와 일치한다.

## 수용 범위

Verified는 승인된 HeaderAndGrid 사전검사 범위에 한정한다. PNG 전체 decode/CRC, 픽셀 품질, 라이선스, Unity import, gameplay 및 실제 아트 에셋은 검사 범위가 아니다. 임시 증거 디렉터리는 계약대로 보존한다.
