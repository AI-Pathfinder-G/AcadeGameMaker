# C4 신규 meta 6개 바이트 근거 초안 독립 설계 검토

- 대상: docs/specs/work-contracts/2026-10-01-c4-meta6-exact-byte-evidence-draft.md
- SHA-256: 94AF2606ABC3C96C0F3570DA55E9063782114A7ACB43DF900322BE0268A8724E
- 대조 기준: 22파일 선행 원장 1B80A52913CFD1119F97BAA9B667A6DBE2773FBBB33A167E647785AAB0A655BB; 선행 Luna 결과 9EA4709166F861D5F5D7E0B590345C835E1B54F6C145B77B1FE4342A00A93AD5; 제한 수용 D9A02137DC82CF6DB454E39290DC5E80B4BE496388A572FA1C25ABC0ADE62278; Approved C4 r4 7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D; v19 원장/입력 980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595 / 274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C.
- 판정: P0 0, P1 1. Draft 승인이나 meta 바이트 승인, 게시·Unity·전체 C3/C4 수용이 아니다.

선행 22파일 원장에서 여섯 meta는 정확 승인 경로, 독립 현 바이트 검수 및 이전→현재 사슬이 모두 미입증이었다. 이 Draft는 그 미입증을 유지하면서 해당 여섯 경로만 다시 조회하는 범위를 제안한다. 현재 각 meta의 실제 SHA는 Draft 표·선행 JSON·v19 manifest와 6/6 일치했다. GUID는 짝 source/선행 행과 맞고, 선정 GUID는 Assets 아래 445개 meta에서 중복이 없다. v19 입력에는 여섯 meta와 짝 source가 모두 포함된다. Draft의 세 구현자 산출물과 이 Draft는 v19 입력/manifest 밖이다. 제한 수용은 여전히 게시 허용 목록을 비워 두고, 완전히 닫힌 22파일은 0개라고 기록한다.

r4 해석도 신중하다. runtime Bridge 행은 .cs와 .cs.meta를 함께 명시한다(50행). 다섯 시험 source는 정확 경로 표에 있으며 각 신규 .cs에 같은 경로의 .cs.meta만 동반한다는 일반 규칙이 별도로 있다(139행, 143–147행). 따라서 Draft가 runtime 명시 쌍과 시험 source+sidecar 규칙을 구분하고, 시험 meta의 완전 경로가 원문에 직접 쓰였다고 소급 주장하지 않는 것은 적절하다. 후속 source 승인에서 meta 승인으로 자동 확장하지 않는 조건도 유지해야 한다. 이 규칙 해석은 정확 경로 권한의 근거를 기록할 수는 있어도 meta의 현재 SHA 독립 검수를 대신하지 않는다.

**P1 — 독립 검토 결과 파일의 허용 경로가 없다.** Draft는 테라가 만들 세 출력 세 경로만 명시하고, 뒤에서 루나의 별도 독립 검토를 요구하지만 루나 결과 기록 경로를 열지 않는다. 직전 22파일 계약은 구현자 세 출력과 별도 루나 검토 한 파일을 분리해 이 문제를 닫았다. 같은 구조를 여기에도 명시해야 한다. 예를 들어 테라 출력은 열거된 세 파일로 제한하고, 루나의 결과는 docs/verification/2026-10-01-c4-meta6-exact-byte-evidence-luna-result-review.md 한 파일만 별도 허용하며 v19 입력/manifest 밖인지 착수 전 검사하도록 적는다. 이 검토 경로 자체는 현재 존재하지 않고 v19 입력/manifest 밖이다.

제안된 출력 세 경로도 v19 동결 목록과 겹치지 않는다. 다만 승인 전에는 어떤 산출물도 쓰지 않는다. 현재 바이트 대조가 맞더라도 선행 원장의 여섯 meta 행은 계속 미입증·미폐쇄이며, 새 증거도 게시 승인으로 승격할 수 없다. Unity, QA 실행, Git, 원격 또는 기존 파일 변경은 수행하지 않았다.
