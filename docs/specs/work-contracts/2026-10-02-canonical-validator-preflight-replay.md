# 정규화 검사기의 실행 전 지문 보완

상태: **Approved**, 2026-10-02 아스트라 역할 승인. 최초 도구 결과（`../../../artifacts/c4-canonical-baseline-validator-v1.json`；로컬 보존 자료） SHA-256 `91E3AB70066814321A32D8FAC1807299D44580EE88C83A2E8373FBFB37190F9F`의 자체 검사10/10 및505/534 대조 성공은 보존한다. `AC-PLAT-054`의 실제 실행 직전 전수 기록이 빠져 기존 결과의 그 기준은 보류다. 새 기록으로 소급 성공이라고 바꾸지 않는다.

추적은 기존 `REQ-PLAT-054..056`, `AC-PLAT-052..054`다. 새 기능·지문 완화·자체 검사 재실행·Unity·네트워크·Git·게시·원본 변경은 금지한다. 최종 검사기 SHA `80B277304ADA6C06CCEAC2325D258047E60366CB1579F3AE7C635D127225D7B5`는 기존 실행본에 추적 주석2행만 추가한 버전이다. 주 에이전트가 두 행을 제외한 원시 바이트SHA가 기존 실행SHA `E18C46C3CF31EBAAEDCDEFE20E0CA44B78F5DF1016EFA2BF2D90FEB8EE5E250E`와 같음을 확인했다.

주 에이전트는 새 `artifacts/c4-canonical-baseline-validator-preflight-v2.json`에 실제 UTC와 정확 원본523행·원본505행·505 후보505행·534 후보534행의 경로/루트종류/SHA/바이트를 기록한다. 범위는 선별523 원장·505 원장·534 v2 원장의 정확 행만이다. 원문/후보 SHA를 각 owning 원장과 비교해 불일치0일 때만 실행을 배정한다. 입력 원장·v1 결과·검사기 자체SHA도 결속한다. 이 전수 기록을 루나 검수라고 주장하지 않는다.

구현·QA 역할의 별도 `gpt-6-sol`은 이 정확 사전 기록과 SHA·UTC 및 최종 도구SHA를 대조한 뒤 최종 도구로505·534v2 각각 한 번 읽기 검사를 수행한다. 예상 원장SHA는 각각 `9E23DCFBFAA7C1B3C2AE006FBD6BDEC58F456974E7C4C878E9F38416C654762C`, `89BC3D6EFDD2B1344E8E6F4F74E819382F7AB2F9C67EBFE373B3BF62F4D26F9E`다. 원본 루트는 현 저장소, 후보는 각각 `output/publication-lf505`, `output/publication-lf534`, 예상 행수505/534를 명시한다. 각각 UTC 시작/종료·정확 인수·stdout·실제 종료를 새v2에 남긴다. 실패하면 원출력을 보존하고 자동 반복하지 않는다.

실행 후 주 에이전트가 동일2067 경계행과 메타입력 지문을 다시 대조한다. 새 허용 출력은 사전 기록, `artifacts/c4-canonical-baseline-validator-v2.json`, 같은 이름 `.md`, `docs/verification/2026-10-02-canonical-validator-preflight-replay-terra-report.md`, `docs/verification/2026-10-02-canonical-baseline-validator-astra-check.md`다. 실행자는 지정 결과/보고를 작성하고 주 에이전트가 사전·사후 기계 대조를 작성한다. 원본·후보·도구·v1 기록을 수정하지 않는다.

`AC-PLAT-054` 새 실행 증거의 조건은 실행보다 앞선 완전2067행 지문·두 실제 종료0·사후 동일2067행·메타입력 차이0이며 최초v1 보류를 역사로 보존한다. 자체 검사10/10은 그 원래 실행SHA의 증거를 인용하고 재실행하지 않는다. 전체/행 `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`, `CleanCloneExecuted`는 모두 거짓이며 새 후보 전체 회귀·독립 수용·게시를 발급하지 않는다.
