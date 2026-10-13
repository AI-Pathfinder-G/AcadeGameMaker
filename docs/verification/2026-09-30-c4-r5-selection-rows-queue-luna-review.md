# C4 R5 선택·행·큐 도구 독립 정적 검토

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-r5-evidence-row-alignment-amendment.md` — SHA-256 `18CF0BC99C0709220022E3998D003264F8B95A89FD1F5D5AA28DE71805DBA88C`
- 독립 구현 검토: `docs/verification/2026-09-30-c4-r5-row-alignment-luna-implementation-review.md` — SHA-256 `884F0D7A1573F619D391C2EDC8F2CB03B01590B180F708117C90E885E513982F`
- Edit 선택 v5: `artifacts/c4-focused-edit-selection-v5.json` — SHA-256 `B66218B56C4F88F626EBC68B5345DAF09A4582DFB03E89AE2D6A51D1F566803D`
- 기대 행 v5: `artifacts/c4-required-checkpoint-rows-v5.json` — SHA-256 `287050D0B5DEDE53566D1229EB31324716FD4C8308165889A31B0C1A1CF1AB88`
- 큐 도구: `artifacts/c4-final-validation-queue.ps1` — SHA-256 `7C30717BD734E8246A47756E76033673797A2B3558E827F047B70A82736AF788`
- 범위: 선택·행 자료와 큐 코드를 정적으로 대조했다. Unity·큐 실행 및 동결 수용은 하지 않았다.

선택 v4와 v5를 비교한 결과 `ExpectedQualifiedNames` 139개, 원래 순서, selector, 플랫폼, parser 증거와 나머지 메타데이터가 같다. `SourceFiles`의 부모 시험 경로는 유지되고 SHA만 기존 `5F6E…`에서 현재 부모 시험의 실제 SHA `8299C67885CFA63BD3B1A09AAC355A669F21862A8DA5643A90B23EAB008979E6`로 갱신됐다. 해당 값은 부모 시험 현재 바이트와 일치한다.

기대 행 v4/v5는 각각 188행이다. 상위 `SourceFiles` 외 메타데이터는 동일하고, 세 입력 파일 중 Edit fixture의 SHA만 실제 현재 바이트 `A9677499FC8528656387CBA8926C030589A7E7522162C2031F90DAE4E1902F5B`로 바뀌었다. 행을 순서대로 비교해 85개 변경, 103개 동일을 확인했다. 동일 103개는 Edit 63개와 Play/Strict 40개다. 모든 행의 ID·Order·ExpectedCase·RequirementIds·AcceptanceIds는 유지되며, 추가·누락 행은 없다. 변경 85개는 승인된 C1 fault 55행, 정상 완료 24행, 최종 장벽 6행에 해당한다. AC006의 실제 저장 차단 사실과 CP17 진단 한정 변경은 직전 구현 검토에서 확인한 의미를 그대로 유지한다.

현재 큐 코드는 허용 결과 경로를 `artifacts/c4-final-validation-queue-result-v7.json`으로 고정한다. 기존 결과가 있으면 실행 전 거절하고, 가져온 `C4-Write`는 `FileMode.CreateNew`를 사용한다. 프로젝트 mutex는 마지막 결과 기록까지 잡고 있으며 실패 시 중단, 결과의 `WholeAccepted=false`, 실패 종료 동작도 보존된다. 과거 큐 코드 바이트는 현재 작업 트리에 별도 파일로 남아 있지 않아 이전 버전과의 원시 바이트 차분까지 주장하지 않는다. 검토 범위에서는 승인된 결과 경로 변경 외 동작을 바꾼 근거가 발견되지 않았다.

판정: P0=0, P1=0. 세 자료의 현재 바이트는 승인된 선택·행·결과 경로 변경과 일치한다. 실제 실행 결과, 새 동결 원장·입력 목록·계획 및 Unity 재검증은 이 검토의 범위 밖이며 C3/C4 전체 수용을 뜻하지 않는다.
