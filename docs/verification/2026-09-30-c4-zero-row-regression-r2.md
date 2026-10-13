# C4 0행 회귀 r2 자체 검증

- 추적: REQ-M5D7QC4-007 / AC-M5D7QC4-009/010, 공동 AC-M5D7QC3-007/008.
- 새 검증기 SHA-256:
A947C9C6D51C94D177F8A8268B4930607C1669232602B0F4C1C6E960FA16D9DC
- 실제 명령·종료값·입출력 SHA:
artifacts/c4-zero-row-regression-r2-evidence.json
- 0행 양성: 종료 0, Rows 0·모든 카운트 0, SourceMatched/EvidenceMatched=true.
- 정상 형식 예상 밖 C4 행: 종료 1, 보고서 UnexpectedIds 정확 1, EvidenceMatched=false.
- 형식 오류: 종료 1, InvalidIds 정확 1.
- Play 양성: 종료 0, 35/35. 누락·nested 불일치 음성: 각각 종료 1, RequiredMissingIds 1.
- Matrix91 C3 원본 검증: 종료 0, 377/377.
- r1 fixture/결과/증거와 원본 XML·계획·행·입력 캡처 지문을 보존함. Unity 미실행. 이는 자체 회귀이며 독립 수용 아님.
