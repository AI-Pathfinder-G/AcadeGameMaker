# C4 미반환 호출 계수 certainty 보정 재검수

- 검토 대상 검증기: `artifacts/c4-verify-required-rows.ps1` SHA256 `ABECF98EFDC2627262BE031CFEDC9D8DB05DC967632CD386C7964939238E5F6F`.
- 합성 확인 기록: `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r4.json` SHA256 `CE4F708E304AFF9E4973F0FE1EBA844AA516742BD28CADFC0ABC6C2AE6A91BC2`.
- 범위: certainty 강제 규칙과 보고된 166개 국소 합성 검사의 구조만 재검토했다. Unity, 컴파일, CWT 실행 증거는 검증하지 않았다.

## 판정

P0=0, P1=0. 이전 certainty P1은 닫혔다. 검증기는 계획행과 실제 `after.facts` 모두에서 `c1Begin`, `c2Finalize`, `preparedObserver`, `c2Observer`, `freshReserve` 다섯 leaf를 `Observed`로 강제하고, `freshComplete`만 `SourceEstablished`로 강제한다. 여섯 값 각각의 반대 certainty를 거절하는 분기가 확인된다.

다섯 leaf는 시험 콜백/실행 중 증가하는 실제 정수 계수기에서 기록된다. `freshComplete=0`은 callback 계수기가 아니며 해당 미반환 경계에서 완료 경로가 source상 도달 불가능하다는 값이므로 `SourceEstablished`가 맞다. 이를 `Observed`로 올리거나 나머지 다섯 값을 source 추론으로 낮추지 않는다.

국소 JSON은 합성 전용으로 166/166 일치, 12개 certainty 반전 거절을 포함한다. `FullCanonicalRecordValidated=false`, `ActualCwtObserved=false`, `ActualUnityExecuted=false`, `WholeAccepted=false`를 유지하므로 이 결과는 실제 실행이나 전체 수용을 증명하지 않는다.
