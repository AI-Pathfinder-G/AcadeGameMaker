# C4 Play15 두 진단 원문 줄 결속 보정

- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`.
- 승인 계약: `docs/specs/work-contracts/2026-10-01-c4-play15-legacy-json-boundary-amendment.md`, SHA-256 `FCFB61233AAC3935FAE4109319B9A19B897A8687F4D9BCA42995816FD18E9B27`.
- 루나 지적에 따라 Play15 두 C5 진단의 실제 출력 줄 전체를 각각 고정 원문과 서수·대소문자 구별 비교한다. JSON 의미가 같아도 공백이나 키 순서가 바뀌면 거부한다. 기존 엄격 JSON 파서의 중복 키 거부, 정확 부모·선택·시험 소스 지문 결속 및 그 밖의 C4 행 판정은 유지한다.
- 최종 검증기 `artifacts/c4-verify-required-rows.ps1` SHA-256 `E20892F551795B6CBC021C56007BC9C2B4F0FB8515230DB267FA8331594B22E6`. 큐 도구 `artifacts/c4-final-validation-queue.ps1`은 이전 SHA-256 `3395C8D7DC9A69038987F9B6AB8AB1ABE6CE1063CC599FE4E28669D5C19A261B` 그대로다. 검증기 파서 오류는 0개다.
- 새 원시 회귀 `artifacts/c4-play15-boundary-r4-evidence.json` SHA-256 `22450EAB464A8AAC320AB29FFB55B547104F4DEDA9D2AE6999CDFD60C0CB31F2`. 재현 명령은 `pwsh -NoProfile -File artifacts/c4-play15-boundary-r4-regression.ps1`이다. 원시 증거에는 21사례의 실제 명령·입출력 SHA·종료값·오류 분류와 원본 지문 보존 확인이 있다.
- 판정: 원본 Play15 0행, Remaining149 0행, Play35 35행 양성은 종료 0이다. 공백 변형과 키 순서 변형은 각각 `InvalidIds=2`, 종료 1이다. 이전의 값 변조·교환·누락·중복·중복키·부모 실패/중복/오귀속·ID 대소문자 변형·예상 밖 C4·형식불량 C4·Play35 누락/불일치 음성도 기대대로 종료 1이다. Matrix91 원본 C3 검증은 377/377 통과했다.
- r3 도구·원시 증거·문서는 역사로 보존했고 이전 원시 증거 SHA-256은 `43E19B8A3A256B907B65E08ED53FC2C8BE7A1EA19AE5D96332C70F0AEBB8DB5A` 그대로다. Unity·큐를 실행하지 않았으며 `artifacts/c4-final-validation-queue-result-v13.json`은 부재다. 이 결과는 자체 국소 검증이며 전체 C4 수용 판정은 아니다.
