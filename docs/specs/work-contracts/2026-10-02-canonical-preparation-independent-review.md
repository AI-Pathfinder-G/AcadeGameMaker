# 정규화 후보와 검사기 준비의 독립 검수

상태: **Approved**, 2026-10-02 아스트라 역할 승인. 사용자가 루나 한도 회복 확인과 후속 작업 재개를 요청했다. 실제 `gpt-6-luna`가 아래 완료된 준비 작업을 독립 판독하고 단일 결과 문서를 작성한다. 오류가 나면 원문을 보존하고 반복 호출·대체 모델·별도 결제 경로를 사용하지 않는다.

## 정확 입력과 판정 범위

| 입력 | SHA-256 | 범위 |
| --- | --- | --- |
| `artifacts/c4-publication-lf505-baseline-v1.json` | `9E23DCFBFAA7C1B3C2AE006FBD6BDEC58F456974E7C4C878E9F38416C654762C` | 원문/후보505 및18 전이 |
| `artifacts/c4-code-first-input-selection-v1.json` | `DB2D4098FECCA9DCD7C4539E7D10145DA286DE44A0FE4A1E46FC599110C458B3` | 추가29의 직접 근거37변과 보류/미입증 구분 |
| `artifacts/c4-publication-lf534-baseline-v2.json` | `89BC3D6EFDD2B1344E8E6F4F74E819382F7AB2F9C67EBFE373B3BF62F4D26F9E` | 정확534·무변환509·LF25·v1 보존 |
| `qa/tools/Test-PublicationCanonicalBaseline.ps1` | `80B277304ADA6C06CCEAC2325D258047E60366CB1579F3AE7C635D127225D7B5` | 실제 검사 논리·경로·바이트·지문·거절 경계 |
| `artifacts/c4-canonical-baseline-validator-v1.json` | `91E3AB70066814321A32D8FAC1807299D44580EE88C83A2E8373FBFB37190F9F` | 정상2·음성8·최초 전후 기록 공백 보존 |
| `artifacts/c4-canonical-baseline-validator-preflight-v2.json` | `A2E156BA94773F7E30DD23CA2A7074D3CA2F3ED578C7042D1E046F6C3190353D` | 실제 실행 전2067 경계행·메타7 |
| `artifacts/c4-canonical-baseline-validator-v2.json` | `A73D7FDBEED75EEA6165C29B0527FA58FC39A9D71A4F047FCCF631F50790AAE0` | 최종 도구 두 실행·시각·종료·현재 지문 |
| `docs/proposals/2026-10-02-canonical-regression-freeze-design-v2.md` | `5159947790E91815766B07BEA6299E69D3526BBC2C02EB2C1C43BB56B6849479` | 후속 시험/감사/QA 결속 Draft의 위험 검토 |

정규화534 문서는 역사 원본SHA를 대체하지 않는다. 도구 실행본SHA `E18C46C3CF31EBAAEDCDEFE20E0CA44B78F5DF1016EFA2BF2D90FEB8EE5E250E`와 최종본은 추적주석2행만 다르다는 실제 재현도 확인한다. 모든 입력 지문을 읽기 전 대조하고 차이가 있으면 기준을 갱신하지 않고 보고한다.

Approved owning 계약은 `2026-10-02-publication-lf505-baseline.md`, `2026-10-02-code-first-input-selection.md`, `2026-10-02-publication-lf534-selected-inputs.md`, `2026-10-02-lf534-transition-count-supplement.md`, `2026-10-02-canonical-baseline-validator.md`, `2026-10-02-canonical-validator-preflight-replay.md`다. 각각 `AC-PLAT-036..038/046..048/049..051/055..056/052..054`로 검수한다. 새 Draft의 검토는 `REQ-M5D7QC4-007`/`AC-M5D7QC4-009/010`, `AC-M5D7QC3-007/008`의 회귀·역사 보존 요구를 근거로 하되 구현 승인을 발급하지 않는다.

코드의 실제 바이트 전이·경로 정규형/중복·재분석 지점·누락/예상 밖 파일·원장SHA 선검증·Git 객체 계산·거짓 플래그 검사와 합성 거절 결과를 독립 판독한다. 현재 원본/505/534 파일 지문, 사전 기록→두 실행→현재 대조 및 역사 기록의 불변을 확인한다. 재실행 없이 기록만으로 확인할 수 없는 사항은 미확인으로 쓴다. 도구 직접 호출·자체 검사/Unity/빌드/네트워크/Git 실행은 이 계약에서 허용하지 않는다.

## 출력과 한계

새 허용 출력은 `docs/verification/2026-10-02-canonical-preparation-luna-review.md` 한 파일이며 현재 부재를 확인한다. P0/P1에 정확 AC·파일·줄·현재 지문을 결속하고 준비 증거 수용 가능성과 Draft의 보완점을 분리한다. 결과는150행 이하로 핵심만 기록한다. 모델 오류는 검수 성공 문서로 대신하지 않는다.

소스·도구·후보·원장·역사 증거·계약·Git·원격은 변경하지 않는다. 534 후보의 전체 내용/권한/의존 폐쇄·깨끗한 복제·게임 회귀 통과·파일별 게시 허용은 검수 범위 밖이다. 루나가 준비 증거를 검수했다고 입력들의 거짓 플래그를 참으로 바꾸지 않는다. 아스트라가 별도 정확 제한 수용을 판정한다.
