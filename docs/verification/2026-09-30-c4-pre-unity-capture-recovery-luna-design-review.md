# Unity 진입 전 캡처 복구 제안 독립 검토

검토 대상: `docs/proposals/2026-09-30-c4-final-queue-pre-unity-capture-recovery-design.md`\
검토 대상 SHA-256: `5B1F93B2C90E6EED31231CCDC1C365B096596F053DFC6077B89B116DEE5173A1`\
판정: **P0 0건, P1 1건. 제안은 아직 실행 승인으로 볼 수 없다.**

현재 `artifacts/c4-final-validation-queue-result-v8.json`은 `Runs=[]`, 중단 stem `c4-r7-focused-edit`, 전체 수용 거짓을 기록한다. 실제 오류는 Unity 시험 전 `artifacts/c4-capture-inputs.ps1`의 출력·재귀 입력 경로 거부에서 발생했다. 캡처 도구는 `artifacts/c4-*.log`를 입력에서 거부하고, 현재 입력 목록의 제외 패턴도 같은 계열을 제외한다. 따라서 두 선택기 builder 로그를 실행 입력에서 빼고, 캡처·검증 규칙은 유지하며, 결과 경로만 새 버전으로 바꾸려는 복구 범위는 원인과 맞는다.

**P1 — 로그가 v14 원장의 `Evidence`에 들어 있다는 설명이 실제 원장과 다르다.** 두 로그는 `artifacts/c4-frozen-source-manifest-v14.json`의 `Evidence`가 아니라 `Files`와 `AllowedChanges`에 각각 한 항목씩 있다. 실제 SHA는 Edit 로그 `BC21AB73E764EF30CE23B7345B3B097459F1E05BFED586879A0D98F97882F949`, Hub 로그 `FADC7CB5FB0EB47C7DC2BC89F1B2DE4748E3CB84B0CAD6059853D3F9A29EFF28`이다. 두 파일은 v14 입력 목록에도 `Added` 사유와 함께 포함돼 있다. 그러므로 제안의 “v14 소스 원장의 Evidence에 각각 정확 SHA로 이미 보존”이라는 문구는 고쳐야 한다. 정확한 연결은 v14 원장의 `Files` 항목 → v14 원장 파일 자체의 SHA 보존이며, v15에서 그 원장과 v14 입력 목록을 `Evidence` 및 직접 입력으로 고정해 이전 상태를 간접적으로 보존하는 것이다. 이 표현을 바로잡지 않으면 후속 원장에서 존재하지 않는 `Evidence` 레코드를 찾거나 로그 지문을 빠뜨릴 수 있다.

그 설명만 수정하면 제안된 데이터 흐름은 현재 도구 계약과 맞는다. v15 실행용 `Files`·`AllowedChanges`에서 두 로그를 제외하고, v15 `Paths`·`ReasonMap`에서도 제외하면 캡처 도구의 정규 경로/출력 거부 및 선행 884 경로와 현재 경로의 정확 사유 계산을 유지할 수 있다. 로그가 선행 884 목록에 없으므로 `Removed` 행을 만들지 않는다는 설명도 맞다. v14 원장과 입력 목록의 실제 SHA를 v15에 결속하면 기존 이력 파일은 변경 없이 보존된다. v15에 큐 도구의 새 지문을 반영하고 결과 경로를 v9로만 전환하며, 9개 실행·선택·188행·실제 종료 관측·CreateNew·실패 중단 조건을 그대로 두는 범위는 한정적이다.

검토 확인값: v14 source manifest SHA-256 `A7D793CAA0F07373FE014B1D33E9ED9F172DFB50CA024631F5D92CA9E6B0CAAE`; v14 input paths SHA-256 `19F1747F66E6B987124BEF6062DEA6557E9314ABABD7FA8BDB68B88B274B3AB6`. 본 검토는 설계와 현행 도구·원장 구조의 대조만 수행했다. 코드·원장·결과는 수정하지 않았고 Unity/큐도 실행하지 않았다. 제안의 P1이 수정·승인되고 v15 원장·입력·계획이 각각 새 지문으로 발급된 뒤에만 재검토할 수 있다.
