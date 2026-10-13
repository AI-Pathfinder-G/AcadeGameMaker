# 정규화 후보·검사기 준비 독립 검수 — Luna

- 결과: 승인 계약의 준비 증거 범위에서 수용 가능. P0 0건, P1 0건.
- 계약: `docs/specs/work-contracts/2026-10-02-canonical-preparation-independent-review.md`, SHA-256 `E4A07C718D409D8AE4CB72462AC1207C83BB19DBE34CB42B9A8181FBC2F55E25`.
- 추적: AC-PLAT-036..038/046..051/052..056; Draft 위험 검토는 REQ-M5D7QC4-007, AC-M5D7QC4-009/010, AC-M5D7QC3-007/008.

## 증거 대조

- **지정 입력 8개 SHA 일치:** `artifacts/c4-publication-lf505-baseline-v1.json` `9E23DCFBFAA7C1B3C2AE006FBD6BDEC58F456974E7C4C878E9F38416C654762C`; `artifacts/c4-code-first-input-selection-v1.json` `DB2D4098FECCA9DCD7C4539E7D10145DA286DE44A0FE4A1E46FC599110C458B3`; `artifacts/c4-publication-lf534-baseline-v2.json` `89BC3D6EFDD2B1344E8E6F4F74E819382F7AB2F9C67EBFE373B3BF62F4D26F9E`; 검사기 `qa/tools/Test-PublicationCanonicalBaseline.ps1` `80B277304ADA6C06CCEAC2325C258047E60366CB1579F3AE7C635D127225D7B5`; 기록 v1 `91E3AB70066814321A32D8FAC1807299D44580EE88C83A2E8373FBFB37190F9F`; 사전 v2 `A2E156BA94773F7E30DD23CA2A7074D3CA2F3ED578C7042D1E046F6C3190353D`; 실행 v2 `A73D7FDBEED75EEA6165C29B0527FA58FC39A9D71A4F047FCCF631F50790AAE0`; Draft `5159947790E91815766B07BEA6299E69D3526BBC2C02EB2C1C43BB56B6849479`.
- **승인 소유 계약 6개:** LF505, 입력 선별, LF534, 전이 보충, 검사기, 사전 재현 계약 모두 실물 상태 `Approved`. 계약 ID 연결은 AC-PLAT-036..038(`2026-10-02-publication-lf505-baseline.md:18–20`), 046..048(`2026-10-02-code-first-input-selection.md:22–24`), 049..051(`2026-10-02-publication-lf534-selected-inputs.md:15–17`), 055..056(`2026-10-02-lf534-transition-count-supplement.md:13–14`), 052..054(`2026-10-02-canonical-baseline-validator.md:15–17`, `2026-10-02-canonical-validator-preflight-replay.md:13`)이다.
- **AC-PLAT-036..038 — 505 후보:** v1은 505 고유 경로, 오류·초과 복사 0을 기록한다. 현재 원본 505/505와 후보 505/505 행별 SHA가 일치한다. 기록 결과는 무변환 487·CRLF→LF 18이며 승인·의존 폐쇄·전체 수용 플래그는 거짓이다.
- **AC-PLAT-046..048 — 추가 입력:** 523개 중 선정 29·보류 6·제외 488이다. 37개 직접 근거 간선의 원본/대상 실물 SHA와 줄·발췌가 37/37 일치한다. 미해결 조건과 거짓 승인 플래그가 보존된다.
- **AC-PLAT-049..051/055..056 — 534 후보·전이:** 정확한 고유 경로 534개(505+29), 누락·초과 0. 총 무변환 509·CRLF→LF 25이며 추가 29개는 무변환 22·정규화 7이다. 현재 원본 534/534와 후보 534/534가 기록 SHA와 일치하고 의존성 미해결 상태가 유지된다.
- **AC-PLAT-052/053 — 검사기:** 현재 검사기 SHA는 `80B277304ADA6C06CCEAC2325C258047E60366CB1579F3AE7C635D127225D7B5`. 경로/바이트/지문/거짓 플래그 대조 논리는 22–289행, 정상·거절 사례는 292–479행이다. 기록된 자체 검사는 정상 2·음성 8, 10/10, 종료값 0이다. 이 검수에서는 도구를 호출하지 않았다.
- **AC-PLAT-054 — 사전·두 실행·현재 지문:** 사전 v2의 네 그룹 2,067행(523+505+505+534)과 별도 메타 7개를 현재 실물과 각각 대조해 불일치 0을 확인했다. 실행 기록 v2는 사전 완료 UTC `01:46:39.8333878Z` 이후 505 실행 `01:47:56.638852Z`–`01:48:11.947195Z`, 534 실행 `01:48:11.949191Z`–`01:48:29.085859Z`를 기록하며 둘 다 종료 0이다. 현재 2,067행·메타7은 사전 지문과 일치한다. 최초 v1의 AC-PLAT-054 보류는 유지되고, 새 실행의 모든 수용 플래그도 거짓이다.
- **실행본 지문 재현:** 최종 도구에서 첫 두 추적 주석 행만 메모리상 제외한 SHA-256 `E18C46C3CF31EBAAEDCDEFE20E0CA44B78F5DF1016EFA2BF2D90FEB8EE5E250E`가 기록 실행본 SHA와 일치한다. 정적 바이트 계산이며 재실행이 아니다.
- **Draft 위험, REQ-M5D7QC4-007/AC-M5D7QC4-009/010/AC-M5D7QC3-007/008:** Draft 23행은 과거 `StaticReview`와 정규화 현재 SHA를 분리하고 역사 바이트 재조회 실패 시 중단하도록 한다. 28–32행은 새 독립 검수 전 새 감사 수용을 금하고 `LunaReviewed=false`를 유지한다. 과거 review의 정규화 전후 SHA를 새 독립 검수로 오용하지 않는다. Draft는 승인된 실행 결과가 아니다.

## 한계

정적 파일 지문·기록·검사 논리 검수만 수행했다. 실제 코드 실행·Unity·Git·네트워크를 수행하지 않았다. 534개 전체 내용/의존 폐쇄, 깨끗한 복제, 게임 전체 회귀, 파일별 게시 허용, 독립 최종 수용 또는 게시 권한을 승인하지 않는다. 후속 작업은 별도 Approved 계약과 아스트라 수용이 필요하다.
