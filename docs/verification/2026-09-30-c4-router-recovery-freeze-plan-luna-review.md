# C4 Router 복구 동결 원장 및 재실행 계획 독립 정적 검수

## 판정

현재 동결 manifest, 입력 목록, 계획에서 P0 0건, P1 0건이다. 검토는 정적 바이트·구조 대조뿐이며 Unity 큐 실행이나 C4 전체 수용이 아니다. 각 문서와 계획의 `WholeAccepted`는 `false`로 유지된다.

## 동결 원장과 입력 목록

검토한 SHA-256은 다음과 같다.

- `artifacts/c4-frozen-source-manifest-v6.json`: `A7340448E4ED9FA96CA88CFF07B542ED0D57A8D657C7677FA3896F91B473BC83` — 102개 파일, 허용 변경 26개.
- `artifacts/c4-frozen-input-paths-v6.json`: `1D3532AA26A544F57365E04B46279111135F6D15BDF859435B30577B997BA74F` — 979개 경로와 이유 항목.
- `artifacts/c4-final-validation-queue-plan-v4.json`: `F762E8048F83AFBDBBF2411959FF5338344EE28320EC3F9BE1F1C1CE25BE5877` — 9개 실행 계획.

Manifest의 102개 `Files` 경로는 모두 존재하고 선언 SHA가 현재 파일 바이트와 일치했다. v5의 102개 경로·순서는 보존됐다. 변경 SHA는 승인된 네 경로뿐이다: `InputRouter.cs`, `HubUiOnlyQ0ScopeAuditEditModeTests.cs`, `c4-audit-current-source-pins.json`, `c4-final-validation-queue.ps1`. v5 대비 `AllowedChanges`는 25개에서 26개로 늘었다. Router·Q0·queue 세 항목의 사유가 이번 승인에 맞게 갱신되고 pin 자원 한 항목이 추가됐으며 다른 허용 경로는 그대로다. 원장 predecessor는 C3 R11 manifest의 정확 SHA를 가리킨다.

입력 목록은 v5의 975개 경로와 975개 이유 객체를 전부 유지하고, 새 원장·이전 pin snapshot·승인 계약·독립 Router 검수 문서 네 경로를 추가해 979개가 됐다. 이전 항목의 이유와 정렬은 그대로이고, 중복·누락·존재하지 않는 경로는 없었다. 새 직접 입력 네 개는 모두 목록에 있으며 목록 자신, plan, 결과 및 실행 출력은 제외됐다. 직접 참조되는 manifest, checkpoint rows, predecessor selection map, 선택 파일, 도구 10개도 목록에 포함되고 plan의 SHA와 실제 바이트가 일치했다.

## 계획 비교와 선택 집합

v3와 v4를 구조 비교했다. 허용된 차이는 source manifest와 input list 지문, queue 도구 지문, 첫 run의 `c4-focused-edit` stem을 `c4-r2-focused-edit`로 바꾼 것과 그 stem을 쓰는 여덟 출력 경로, 그에 따른 명령행 길이 증가 6자뿐이다. 이 항목을 이전 값으로 되돌렸을 때 v3 계획 전체가 정확히 재현됐다. 나머지 여덟 run과 모든 공통 선택·행·실행 설정은 v3와 동일하다.

세 기존 focused 선택 파일은 v3와 지문·이름·selector가 같다. 각각의 이름 수와 계획 수는 Edit 139, Play 32, Hub 5이며 이름 중복은 없다. 이 세 선택의 필수 행 ID 수는 148, 35, 5로 총 188개이고 전부 유일하며 188행 registry에 존재한다. 이 숫자는 선택 시험 이름 176개와 구별된다. 91/149 및 후속 회귀 선택의 계획 항목도 v3와 불변이다.

9개 실행의 실제 Windows 인자 quoting 규칙으로 다시 계산한 명령행 길이는 계획 값과 모두 일치했고 한도 32,767보다 짧았다. 순서대로 길이는 19,566, 4,937, 1,125, 14,444, 24,161, 503, 2,848, 483, 2,401자다. 첫 run도 요청한 19,566자와 일치한다. 모든 run은 사례 제한 180초, 프로세스 관찰 21,600초를 유지한다. 단일 프로젝트 배타 잠금, 중단 시 실패 정책도 보존됐다.

새 계획의 9회×8개 출력 경로는 총 72개이며 모두 서로 다르고 현재 존재하지 않는다. `artifacts/c4-final-validation-queue-result-v3.json`도 없다. 첫 출력 stem을 바꾼 이유대로 이전 실패 산출물을 덮지 않는 상태다.

Unity와 큐는 실행하지 않았다. 따라서 컴파일 오류 해소, 재실행 결과 또는 실제 AC 통과는 아직 입증되지 않았다. 본 검수는 입력·동결·계획의 실행 전 결속만 확인한다.
