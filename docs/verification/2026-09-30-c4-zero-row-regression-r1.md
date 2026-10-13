# C4 0행 회귀 자체 검증

- 추적: REQ-M5D7QC4-007 / AC-M5D7QC4-009/010, 공동 AC-M5D7QC3-007/008.
- 검증기 SHA-256:
68FFA7ABD658332794B70F623F740D23CD06181C54587ECE3745B8FE711E8838
- 원시 명령·종료값·입출력 SHA:
artifacts/c4-zero-row-regression-r1-evidence.json
- 0행 양성: 종료 0, Rows=[] 및 모든 카운트 0, SourceMatched/EvidenceMatched=true.
- 형식 오류 음성: 종료 1, InvalidIds 1. 비영 Play 양성: 종료 0, 35/35.
- 비영 누락·nested 불일치 음성: 각각 종료 1, RequiredMissingIds 1. Matrix91 원본 C3 검증: 종료 0, 377/377.
- 미해결 P1: 정상 형식의 예상 밖 C4 행 음성은 종료 1이나 빈 배열의 rows.Id 접근에서 예외가 발생해 UnexpectedIds 결과 JSON이 생성되지 않음.
- 원본 XML/계획/행/결과와 입력 파일은 수정하지 않았으며 Unity는 실행하지 않음.
