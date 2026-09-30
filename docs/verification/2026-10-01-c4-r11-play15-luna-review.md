# R11 Play 15개 독립 실행 결과

- 검토 범위: v15 큐의 여섯 번째 실행 `c4-r2-play15`만 확인했다. 추적 기준은 REQ-M5D7QC4-007, AC-M5D7QC4-009/010 및 공동 AC-M5D7QC3-007/008이다. 적용 계획 `artifacts/c4-final-validation-queue-plan-v15.json`의 SHA는 `43125A3558F6B1F3FDDC595FB47F602883B4861D0EF24690F1FEE27083EC9082`다.
- 실제 XML `artifacts/c4-r2-play15.xml` SHA `85DFF326B421C2660FD7A931FEF1E644D45A67D3A598680CC2B6E17318E7A490`에서 계획된 15개 전체 이름이 정확히 한 번씩 나타났고 모두 Passed였다. 실패·건너뜀·미분류·누락·초과·중복은 0이다.
- C4 행 대조 `artifacts/c4-r2-play15-required-row-comparison.json` SHA `56FE1752E5A6E8C4AAEEF2A69F68F5F61063A242E664B459D1FBF2D58DDDC4E9`는 이 선택에서 C4 필수 행 0개를 확인했다. 계획·도달·통과·실패 및 누락·예상 밖·잘못된 ID가 모두 0이고 `SourceMatched=true`, `EvidenceMatched=true`다. 별도 Play15 경계 회귀 자료 `artifacts/c4-play15-boundary-r4-evidence.json`은 허용된 정확 두 JSON 진단만 대상으로 하며, 본 XML에는 JSON 출력이 정확히 두 줄 있었다. 두 출력은 다음과 같다.

  - `C5-same-cancel-absent`: `status=passed`, `observedFields=183`, `currentMemoryAbsent=true`, `wholeGameplaySessionClaimed=false`
  - `C5-same-cancel-present`: `status=passed`, `observedFields=183`, `currentMemoryAbsent=true`, `wholeGameplaySessionClaimed=false`

  다른 JSON 형식 출력은 없었다. 이 두 진단을 C4 필수 행이나 전체 게임 세션 보존 증거로 확대하지 않는다.
- 실제 종료 자료: native 관측 `artifacts/c4-r2-play15-native-exit-observation.json` SHA `1BDB5F6CD6DD9E2C5CEF8C044E07BEB4BE823B682FF600644DC3CDCBD5FDD228`, 실제 종료 코드 0; QA/외부 반환 `artifacts/c4-r2-play15-qa-tool-return.json` SHA `F39B726476BC5AE6D9CEC335F0335FD32F0996910913D82E3175E2F2F9630385`, 두 코드 모두 0이다. 검증 `artifacts/c4-r2-play15-verification.json` SHA `A110176063AFCF632B85F56C77420B65FED1451BF227D239F289ADB97E258300`은 `Verified=true`, 입력/동결 차이 0, `WholeAccepted=false`를 기록한다.
- 입력 전후 스냅샷은 각각 `artifacts/c4-r2-play15-source-before.json` SHA `C8A48C6E815D5706AA52DB7EF2E3D05501603A0B37BB4B18D666F7DA2113B6DD`, `artifacts/c4-r2-play15-source-after.json` SHA `497146D3B90A005BDC65174AE52E28FD546F3253E2E8884E853A7513A54841EE`이며, 각각 1084개 경로로 비교 차이는 0이다.
- 판정: 이 묶음의 AC-M5D7QC4-010 실행 조건 및 해당 Play15 경계 확인은 통과했다. 다음 필수 Edit 실행이 진행 중이므로 이 결과는 부분 결과다. 전체 C4와 통합 수용은 판정하지 않았다.