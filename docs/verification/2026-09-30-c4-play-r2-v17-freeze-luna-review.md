# C4 Play R2 v17 동결 자료 독립 검수

검토 범위는 Approved 계약 `docs/specs/work-contracts/2026-09-30-c4-play-r2-three-row-source-alignment-amendment.md`(SHA-256 `AA9FDB5A6055991E9726891167EE16209578862E3178C587CB44903E8505ADCD`)에 따른 v17 소스 원장, 입력 목록, 사전 캡처뿐이다. 계획 또는 Unity 실행의 승인·수용을 뜻하지 않는다.

## 대조 결과

- `artifacts/c4-frozen-source-manifest-v17.json` SHA-256 `E3D8626C7DA9F058D228022ED357110A7FF6B0A14A05A15B88FC4FA8C8E668D6`: 171개 파일과 96개 허용 변경 경로가 선언되어 있다. v16의 164개 파일은 모두 보존됐고, 그중 지문 변경은 Play fixture와 큐 도구 두 파일뿐이다. 신규 7개는 rows v9, builder 증거, 승인 계약, 제안서 및 세 검토 기록이다. 171개 파일의 현재 바이트 지문은 모두 원장과 일치한다.
- `artifacts/c4-frozen-input-paths-v17.json` SHA-256 `DF6B55D8C9D2620D6AB20013B14A1F45B9CB0843DD4DA631F5F503F04AD2FA2D`: 1,060개 경로와 사유가 각각 1,060개이며 중복은 없다. 경로는 대소문자 구분 정렬이고 사유표의 경로 순서와 일치한다. v16의 1,052개 경로 및 사유는 모두 그대로 남았고, 추가 8개는 v17 원장, rows v9와 builder 증거, 승인 계약 및 관련 제안·검토 기록이다. 모든 입력이 존재하며 원장 파일 전부가 입력에 포함된다.
- `artifacts/c4-play-r2-three-row-preflight-v17.json` SHA-256 `F80D7DC01FEF1C4E5CAA2B50ED6C65A9E72BDECFF1DB317F825D3EBE9D421FB5`: 캡처 수 1,060이며 각 경로와 현재 SHA-256이 입력 목록과 일치한다. 캡처는 입력 목록 자체와 사전 캡처 결과물을 입력으로 포함하지 않는다. 별도 실행 보고에 기록된 직접 캡처 종료값은 0이다. 이 검수에서는 큐나 Unity를 실행하지 않았다.

## 판정

P0 0, P1 0. 확인한 동결·입력 결속에 차단 결함은 없었다. 이 판정은 사전 자료의 정합성에 한정된다. 실행 계획, 실제 Unity 결과, C4 전체 수용은 별도 검증 전까지 미판정이다.
