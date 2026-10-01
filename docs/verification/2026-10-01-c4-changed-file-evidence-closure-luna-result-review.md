# C4 파일별 바이트 근거 원장 독립 검토

- 검토 대상 JSON: artifacts/c4-changed-file-evidence-closure-v1.json
- JSON SHA-256: 1B80A52913CFD1119F97BAA9B667A6DBE2773FBBB33A167E647785AAB0A655BB
- 요약 문서 SHA-256: 12AD67063501692BB1B1CB8E1A53440745AB7389D554FDF26EB9405906CFE869
- 구현자 검증 기록 SHA-256: C95D08006C9FDF0EF59DFA848689DAA538CC40F72B68304AB3009C84510D9D8B
- 승인 계약 SHA-256: FDBC8ECE8DA136CDA62E58E23CCAD8C3CABE8A197492A2BFB5ADBDB8A06E48E5
- 부모 원장 SHA-256: E43A4268ABC87A56AD4F4A198D96893135706EB1364ABB33D70BA27741765B17
- v19 동결 원장/입력 SHA-256: 980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595 / 274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C
- 판정: P0 0, P1 0. 이는 파일별 근거 원장의 독립 대조이며 파일별 근거 사슬의 폐쇄, 게시, 클린 복제 또는 전체 C3/C4 수용 판정이 아니다.

착수 전 확인한 독립 검토 경로 docs/verification/2026-10-01-c4-changed-file-evidence-closure-luna-result-review.md는 존재하지 않았고 v19 입력 1084경로 및 source manifest 193파일에 없었다.

부모 원장 501행의 “정확 변경 승인 필요” 24행 중 v19 동결 파일과 교차하는 행은 22개다. 구성은 기존 변경 10개, 신규 source/test 6개, 짝 meta 6개이며 고유 경로는 22개다. 범위 밖 두 C3 경로는 artifacts/c3-build-required-decision-expected-rows.ps1와 artifacts/c3-required-decision-expected-rows.json으로 정확히 남았다. 22개 모두 부모 원장, v19 source manifest 및 실제 파일의 현재 SHA가 일치하고, 모두 v19 입력에 포함된다. 22개 REQ/AC 배열은 부모 원장 값과 동일하다. 구현자 산출물 세 경로는 v19 입력과 manifest 모두에 포함되지 않는다.

승인 경로 증거는 16개 행에 있고 나머지 6개는 신규 meta 행으로 미입증 처리됐다. 독립 현재 바이트 검수도 16개 행에서만 확인되고 meta 6개는 미입증이다. JSON의 승인 문서 참조 56개와 독립 검토 참조 69개를 각각 대상 문서 SHA 및 지정 줄의 경로/현재 지문으로 대조했다. 인용 문자열은 지정 줄에 포함돼 있다. 각 행의 이전 후보 지문은 부모 원장과 일치한다. 기존 source/test 및 신규 source/test의 현재 바이트는 동결 지문과 일치한다.

여섯 신규 meta의 실제 GUID는 JSON, 짝 source, 고유 소유자 값과 일치한다. 선택 파일이 연결하는 16개 GUID는 Assets 아래 445개 meta를 대상으로 확인했으며 중복이 없다. 22행의 paired meta, asmdef 경로와 SHA 연결도 JSON의 “참” 표기와 일치한다.

변경 사슬은 22개 모두 “미입증”, 폐쇄 0·열림 22다. 이 미입증 상태는 과잉 승격을 막는 정직한 결과다. 특히 신규 meta 6개를 source 승인·검수에서 승계시키지 않았으며, 모든 행의 PublicationApproved=false, 전체 수용 false다. 원장 489/61/428 분류와 게시 제외는 보존됐다. 세 구현자 산출물의 착수 전 부재는 구현자 검증 기록에 기재돼 있으나, 사후 독립 재구성은 불가능하므로 그 부분은 구현자 기록에 의존한다. v19 목록에서 제외됐다는 사실은 확인했다.

Unity, 컴파일, Git, 네트워크, 기존 입력·소스·QA 또는 증거 파일 변경은 수행하지 않았다.
