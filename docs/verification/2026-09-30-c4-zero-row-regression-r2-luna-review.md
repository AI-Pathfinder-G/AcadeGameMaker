# C4 0행 회귀 r2 독립 결과 검토

독립 대조 대상은 `artifacts/c4-zero-row-regression-r2-evidence.json`(SHA-256 `7325C48F543996B957A964FE1A428CBCC97233646E083FBDDDB16B23A7CDE3F2`), 구현자 기록 `docs/verification/2026-09-30-c4-zero-row-regression-r2.md`(SHA-256 `E39ADFCE3FF985A909AA2F6468C59E464124F2C9D759E78171AD9D3994761846`) 및 검증기 SHA-256 `A947C9C6D51C94D177F8A8268B4930607C1669232602B0F4C1C6E960FA16D9DC`다.

7개 사례의 명령·출력 경로·종료값과 결과 JSON의 현재 SHA를 대조했다. 0행 양성은 종료 0, `Rows=[]`, 모든 계수·오류 목록 0, source 및 evidence 일치다. 정상 형식의 예상 밖 행은 정확한 ID `play_AC002_GuardedHistoryReadDoesNotPublishAndFreshCursorUsesActualPair`를 `UnexpectedIds`에 기록하고 종료 1 및 `EvidenceMatched=false`를 남긴다. 잘못된 형식 행은 `InvalidIds` 1건과 종료 1이다. Play 35행 양성은 35/35 통과, 누락 및 중첩 값 불일치는 각각 누락 1건·종료 1로 기록된다. Matrix91 C3 비교는 377/377 일치다. 모든 회귀 출력 경로와 fixture 지문이 증거 원장과 맞는다.

9개 기존 입력·실행 바인딩, 4개 변조 fixture, 7개 결과 경로, r1 증거 지문을 현재 파일과 대조한 불일치는 0건이다. R1 자료는 그대로 보존됐고 r2 기록은 `UnityRun=false`, `OriginalsWritten=false`, `WholeAccepted=false`를 명시한다. 따라서 이는 행 검증기 회귀의 확인이지 Unity 실행 또는 C4 전체 수용이 아니다.

## 판정

P0 0, P1 0. 예상 밖 정상 기록의 구조화된 거절이 이제 보고서와 종료값으로 확인되고, 0행·malformed·비영 행·Matrix91 경계도 제출된 회귀 증거와 일치한다. 다음 단계의 동결 원장·계획 검수와 실제 큐 실행은 별도다.
