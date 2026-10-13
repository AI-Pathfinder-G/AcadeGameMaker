# C4 컴파일 복구 동결 자료와 재실행 계획 독립 사전검수

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-test-compile-recovery-amendment.md` — SHA-256 `EF6B0966836FEA1896AF8BA8313D052F8C24E43F93DCD92D27B392377399B4CB`.
- 동결 자료: manifest v7 SHA-256 `14E7C555B3EB79DABB5DCDA1F663212CF287F35FBCA76998C6C44CA5CCF99B5D` (105 Files, 29 AllowedChanges), input v7 `04029893E9EDB0A4B8E2FE3FEDBE5B5F8EC33970E005A2BCDECB5AC40A834255` (985 경로), plan v5 `73C3BF79BBB159ABF48CEBF4950BF13AA03A7471553763588FCD594BD1F2E429` (9회).
- 비교 자료: manifest v6 `A7340448E4ED9FA96CA88CFF07B542ED0D57A8D657C7677FA3896F91B473BC83`, plan v4 `F762E8048F83AFBDBBF2411959FF5338344EE28320EC3F9BE1F1C1CE25BE5877`.
- 판정: 사전검수 P0 0건, P1 0건. Unity·큐 실행은 하지 않았다.

Manifest v7은 v6의 102개 경로와 기존 순서를 유지한다. 기존 경로 중 SHA가 바뀐 것은 승인된 네 시험 소스와 큐 도구 다섯 개뿐이며, 새 Evidence 세 파일(선택 v2 둘, 행 원장 v2 하나)을 추가해 105개가 됐다. 기존 파일 삭제나 그 밖의 SHA 변경은 없다. 105개 파일의 현재 SHA가 모두 원장과 일치한다. 허용 변경 목록은 29개이며 기존 권한 항목을 유지하면서 새 증거 세 항목을 추가한 형태다.

Input v7은 v6의 979개 경로를 모두 보존하고 정확히 여섯 경로를 추가했다: manifest v7, Approved 복구 계약, 이번 검수 문서, 선택 v2 두 개, 행 원장 v2다. 985개 모두 존재하고 각 경로에 이유가 있으며 중복은 없다. `Sort-Object -CaseSensitive` 순서와 일치한다. 입력의 출력 제외 규칙은 큐 plan/result 및 C4 실행 결과 경로를 제외한다. 자기 참조 입력은 없다.

Plan v5는 manifest v7, input v7, 행 원장 v2, 선택 v2와 고정 도구의 지문을 올바르게 결속한다. v4와 비교해 9개 실행의 순서, 플랫폼, selector, 기대 이름·개수, 행 ID, 180초 사례 제한, 21,600초 관찰 제한은 바뀌지 않았다. 첫 실패 run만 `c4-r2-focused-edit`에서 `c4-r3-focused-edit`로 바뀌고 그 8개 출력 경로가 새 stem으로 분리됐다. 나머지 8개 run의 출력 경로는 v4와 같다. 첫 run은 Edit 선택 v2를 사용하며 기대 수는 139다.

계획된 72개 출력 경로는 서로 다르고 모두 아직 존재하지 않는다. `c4-final-validation-queue-result-v4.json`도 없다. 각 run의 Unity 명령행 길이를 runner의 Windows 인자 인용 규칙으로 다시 계산했으며 계획 값과 모두 일치한다. 길이는 503~24,161자 범위로 32,767 제한보다 작다. plan의 `WholeAccepted`는 false다.

이 검토는 새 plan의 사전 조건과 바인딩만 확인했다. 컴파일 복구가 실제로 빌드되는지, Unity 결과가 통과하는지, 전체 C4 증거가 수용되는지는 아직 확인되지 않았다. 실행은 별도 승인·배분 뒤 수행해야 한다.
