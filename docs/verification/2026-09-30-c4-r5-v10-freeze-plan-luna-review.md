# C4 R5 동결 원장·입력 목록·계획 독립 사전 검토

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-r5-evidence-row-alignment-amendment.md` — SHA-256 `18CF0BC99C0709220022E3998D003264F8B95A89FD1F5D5AA28DE71805DBA88C`
- 선택·행·큐 사전 검토: `docs/verification/2026-09-30-c4-r5-selection-rows-queue-luna-review.md` — SHA-256 `C220799B5658B633F654E261B41170FD80AA1275E673685CE21092A678BB88D4`
- 동결 manifest v10: `artifacts/c4-frozen-source-manifest-v10.json` — SHA-256 `AE5537D88DFF77603CA5156FF9AB5D4E5E59C15B9344F2A1E3FBA981D6833D7C`
- 입력 목록 v10: `artifacts/c4-frozen-input-paths-v10.json` — SHA-256 `BAE281834698C8288C465EECB9577FECA37A762188B45612334C3A84FC8C18F7`
- 계획 v8: `artifacts/c4-final-validation-queue-plan-v8.json` — SHA-256 `B2CB339E5EFA7E18B3C5A5862AD7DD1D2905D37602C716984DCF58D51D00868A`
- 범위: 파일 지문·원장·입력·계획을 읽기 전용 대조했다. Unity나 큐는 실행하지 않았다.

Manifest v10은 파일 123개와 허용 변경 48개를 선언한다. v9의 117개 파일이 같은 순서로 보존됐고 제거 경로는 없다. 공통 파일 중 SHA 변경은 큐 도구, Edit fixture, 부모 시험, 참여 원장 네 개뿐이며 각 현재 파일의 SHA가 manifest와 일치한다. 추가된 여섯 항목은 승인 계약, 세 독립 검토 기록, Edit 선택 v5, 기대 행 v5다. 전체 123개 실물 파일의 SHA를 대조해 누락·불일치는 없었다. 기존 허용 변경 항목 42개는 유지됐고 새 항목 6개가 더해져 48개다. `WholeAccepted`는 false다.

입력 목록 v10은 1,008개 경로와 같은 수의 이유 항목을 가진다. v9의 1,001개 경로 각각이 기존 분류와 이유 그대로 보존됐으며 추가 7개는 모두 `Added`로 분류됐다. 추가분은 새 Edit 선택·행 자료, 새 동결 manifest, 승인 계약과 세 독립 검토 문서다. 경로와 이유표의 중복·집합 차이는 없고 목록은 대소문자를 구분해 정렬돼 있다. 신규 C4 출력 파일은 입력으로 섞이지 않았다.

계획 v8은 9회 실행을 유지한다. 상위 결속은 현재 manifest v10, 입력 v10, 기대 행 v5를 가리킨다. 도구 결속을 포함한 모든 계획 바인딩의 실물 SHA가 맞다. 첫 실행만 `c4-r5-focused-edit`에서 `c4-r6-focused-edit`로 바뀌며 선택은 v5다. 139개 예상 이름, selector, 148개 행 ID와 실행 설정은 이전 첫 실행과 일치한다. 나머지 8개 실행 객체는 v7과 완전히 동일하다.

첫 실행의 8개 출력 경로는 새 stem을 사용하고 서로 겹치지 않는다. 전체 72개 계획 출력과 큐 결과 v7이 아직 존재하지 않는다. 각 실행의 `CaseTimeoutSeconds=180`, 관찰 창 21,600초이며 프로젝트 전용 mutex와 버려진 잠금 fail-stop, 전체 fail-stop 정책은 이전 계획과 같다. `WholeAccepted=false`가 유지된다. 실행기의 Windows 인자 인용 규칙을 따라 현재 Unity 실행 인자 길이를 독립 재계산했고 9개 모두 계획값과 일치하며 32,767자 미만이다.

판정: P0=0, P1=0. 세 원장과 계획은 승인된 R5 한정 보정과 일치하며 사전 실행 검사에 필요한 결속·출력 부재 조건을 충족한다. 이 검토는 실행 허가나 실제 시험 성공을 뜻하지 않는다. 앞선 R5 실행에서 관측된 행 불일치는 보존되며, 전체 C3/C4 수용 여부는 새 실행의 실제 결과 비교 전까지 미확정이다.
