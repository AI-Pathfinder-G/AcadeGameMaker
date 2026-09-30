# C4 동결 입력 경로 검사 보정

- 상태: **Approved — 경로 검사 도구의 한정 수정만 승인, Unity 실행·통합 수용 미완료**.
- 승인: 아스트라, 실제 `gpt-6-astra`, 2026-09-30. 독립 루나 검수 `docs/verification/2026-09-30-c4-input-path-guard-luna-review.md` SHA-256 `F48A6CCE6433041CFC1F1A3D7FE6B29D3EE6421216F685381F62001313F342D9`, 초안 SHA-256 `B3E723CAF11E18AF4AF1F6A2A81D5A33817D61DB63AA3B3638E6B5B4D6F8C9C4`, P0/P1=0/0.
- 작성: 아스트라, 실제 `gpt-6-astra`, 2026-09-30.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`.
- 선행: Approved C4 r4·QA r2, 동결 증거 보정, 입력 정렬 보정. 제품 동작·시험 행·선택·Unity 설정은 바꾸지 않는다.

권위 입력 목록 v2의 969개 경로 중 기존 R11의 8개 경로에 한글 또는 이름 내부 공백이 있다. 승인된 `artifacts/c4-verify-required-rows.ps1`의 `C4-Relative`는 ASCII 문자만 허용하여 첫 `c4-capture-inputs.ps1` 실행이 Unity 시작 전 실패한다. 이 여덟 경로는 이전 884개 입력의 일부이므로 삭제·개명·우회하지 않는다. 실제 queue plan·Unity 실행은 아직 없다.

아스트라는 Terra에게 `C4-Relative`의 정확 한 함수만 보정하도록 배분한다. 상대 경로 허용 문자에 유니코드 문자·결합 문자·숫자와 경로 구성요소 **내부** 공백을 더한다. 기존 ASCII 문자 `A-Z a-z 0-9 _ . - /`는 유지한다. 경로 맨 앞 `/`, 역슬래시, 콜론 및 Windows 금지 특수문자, 제어문자, 빈 구성요소, `.` 또는 `..` 구성요소, 구성요소 첫·끝 공백과 끝 마침표는 거절한다. 끝 줄바꿈·널 문자를 포함한 제어문자도 거절하도록 문자열 전체를 정확히 결속한다. 기존 `C4-Path`의 작업루트 내부 실경로 검사도 유지한다. 새 허용 규칙은 실제 969개를 모두 수용하고 이전에 거절해야 하는 절대·상위이동·제어문자·출력·재귀 경로는 계속 거절해야 한다. capture의 정렬·이유·현재 전체 Assets/Packages/ProjectSettings/qa 재열거·동일 입력 hash 검사 등 다른 함수는 변경하지 않는다.

도구 변경 뒤 `artifacts/c4-frozen-source-manifest-v3.json`을 `FileMode.CreateNew`로 한 번 발급한다. v3는 v2의 `Files` 경로·순서·Count 102와 나머지 필드 값을 유지하되 위 정확 도구 한 파일의 `Sha256`만 새 실제 값으로 바꾼다. `AllowedChanges`에는 그 도구 경로를 추가하고 `RequirementIds`는 `REQ-M5D7QC4-007`만, `Reason`에는 `AC-M5D7QC4-009/010`의 한정 경로 검사 근거를 기록한다. 기존 20개 변경 행은 유지한다. v1·v2 원장 파일은 이력으로 보존한다. Edit/Play fixture는 v1 `Files`에서 바뀌지 않은 C1/C2/Bridge/시험 바이트를 확인하며, 실행 계획·runner·verifier는 v3을 권위 원장으로 사용한다. 독립 검수는 fixture가 바뀐 도구의 v1 행을 승인으로 사용하지 않음을 확인한다.

`artifacts/c4-frozen-input-paths-v3.json`도 `FileMode.CreateNew`로 한 번 발급한다. 입력 목록 v2의 969개 경로·이유를 모두 보존하고 새 v3 소스 원장과 본 최종 Approved 계약 경로를 `Added`로 포함해 971개 경로·이유로 만든다. `Sort-Object -CaseSensitive` 순서를 유지한다. 권위 계획에는 v3 소스 원장과 v3 입력 목록의 정확 경로·SHA를 결속한다. 목록 자신·기존 입력 목록 v1/v2·queue plan/result·실행 출력은 입력에서 제외한다. fixture가 실제 읽는 소스 원장 v1은 그대로 포함한다. 기존 소스 원장 v2는 이력 입력으로 남지만 권위 확인에는 사용하지 않는다.

Terra는 수정 함수의 유효 입력 969개와 거절 표본을 결정적으로 검사하고 정확 도구 SHA를 보고한다. Luna는 도구 변경의 한정성, v3의 102/102 현재 SHA, v2와의 단일 파일 차이, 변경 권한 21행, 새 971개 입력 및 capture 사전검사 일치를 독립 검수한다. 아스트라는 그 결과와 queue plan을 별도 승인한 뒤에만 Unity 실행을 배분한다. 어떤 기존 산출물도 덮어쓰거나 `WholeAccepted`를 true로 바꾸지 않는다.
