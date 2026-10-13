# C4 Play15 선행 진단 경계 구현·국소 회귀

- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`.
- 승인 계약: `docs/specs/work-contracts/2026-10-01-c4-play15-legacy-json-boundary-amendment.md`, SHA-256 `FCFB61233AAC3935FAE4109319B9A19B897A8687F4D9BCA42995816FD18E9B27`.
- 구현: `artifacts/c4-verify-required-rows.ps1` SHA-256 `2F38951B91744F5DC643B507AC86B64A7167D528367975A3DBD5A4FAD02C9F94`; `artifacts/c4-final-validation-queue.ps1` SHA-256 `3395C8D7DC9A69038987F9B6AB8AB1ABE6CE1063CC599FE4E28669D5C19A261B`.
- 검증기는 계획의 0개 필수 행, Play15 선택 경로·SHA 및 선택 실물 SHA, 현재 시험 소스 SHA를 확인한 뒤에만 두 정확 C5 진단 ID를 분리한다. 각 진단은 전체 부모 이름·통과 상태·다섯 JSON 키와 값·단일 출력 여부를 확인한다. 중복 JSON 키는 기존 엄격 파서가 거부한다. 대소문자 변형 ID와 그 밖의 `id` JSON은 C4 엄격 경로에 남긴다. C5 진단은 C4 행 수에 들어가지 않는다.
- 큐 도구는 결과 허용 경로만 `result-v12`에서 `result-v13`으로 변경했다. 그 한 줄을 역치환한 SHA-256은 이전 원본 `13484AAA0C5CAF60FFA53BB674D02B3F79B0BEF3055BDC91950C63F9D44455E7`과 같다. 두 도구의 파서 오류는 각각 0개다.
- 최종 국소 회귀 원시 근거: `artifacts/c4-play15-boundary-r3-evidence.json`, SHA-256 `43E19B8A3A256B907B65E08ED53FC2C8BE7A1EA19AE5D96332C70F0AEBB8DB5A`. 각 사례의 실제 명령, 입력·출력 SHA, 종료값, `InvalidIds`·`UnexpectedIds`·행 수와 원본 SHA 보존 확인을 담았다. 재현 명령은 `pwsh -NoProfile -File artifacts/c4-play15-boundary-r3-regression.ps1`이다.
- 판정: Play15 0행 정상, Remaining149 0행 정상, Play35 35행 정상; C5 값 변조·교환·누락·중복·중복 키·부모 실패/중복/오귀속·ID 대소문자 변형은 종료 1; 예상 밖 정상형 C4는 `UnexpectedIds=1`과 종료 1, 형식 불량 C4는 `InvalidIds=1`과 종료 1; Play35 기록 누락·중첩값 불일치는 종료 1. 전체 19사례가 예상 판정과 일치했고 Matrix91 원본 검증은 377/377 통과했다.
- 역사 입력·출력은 보존했다. `artifacts/c4-final-validation-queue-result-v13.json`은 만들지 않았다. 이 검증은 자체 국소 확인이며 Unity 실행·큐 재실행·전체 C4 수용을 뜻하지 않는다. 독립 검수와 새 동결·계획은 별도 절차다.
