# C4 입력 경로 검사 보정 최종 산출물 독립 검토

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-input-path-guard-amendment.md`, SHA-256 `640DC7A45E4501488EBEB97F754841DDEBB471D464FB3CEA8C8D77EAFB5655C4`.
- 수정 verifier: `artifacts/c4-verify-required-rows.ps1`, SHA-256 `74F26AC5BFD34A2ABD78814B981844CB2A9CACCADED1062973FB22665BB4CD12`.
- 이전 verifier: SHA-256 `20418B63A488C6ED8D3EF2088BE8FAE719E7E142315B92EE029CFA3AB9C3CD69`.
- v2/v3 소스 원장: `artifacts/c4-frozen-source-manifest-v2.json` SHA-256 `52E4A870B9892C586B8AFA94C032B467342EC913B8E1071B4A30B10193DA197E`; `artifacts/c4-frozen-source-manifest-v3.json` SHA-256 `9BED196FADEAF2BDE4F4C911A3CA848675C779C9E40F76110CE9B87367416D3F`.
- v2/v3 입력 목록: `artifacts/c4-frozen-input-paths-v2.json` SHA-256 `289B265ADA3583C60E1950CE95FDCCFD49D99E9FB43C58D4818119C53C8C3BDC`; `artifacts/c4-frozen-input-paths-v3.json` SHA-256 `07346132F8ACFD13920D112D6A1E28118A31FB7BFBB61CD4E6A28965AD8EA974`.
- 판정: P0 0, P1 0. 제한된 정적 검수이며 Unity·전체 capture·queue 실행이나 C4 수용 판정이 아니다.

변경된 `C4-Relative`는 문자열 전체에 `\A[\p{L}\p{M}\p{Nd}_./ -]+\z`를 적용하고 각 경로 구성요소의 빈 값, `.`·`..`, 처음·끝 공백, 끝 마침표를 거절한다. 기존 `C4-Path`의 작업 루트 내부 검사도 유지한다. 이에 따라 한글 파일명과 `TMP Settings`의 공백은 허용하되 절대경로, 역슬래시, 콜론·허용되지 않은 특수문자, 제어문자, 빈 구성요소, 상위 이동은 거부한다.

수정 함수만 이전 정규식으로 메모리에서 되돌렸을 때 전체 파일 SHA가 이전 verifier의 정확 SHA `20418B63A488C6ED8D3EF2088BE8FAE719E7E142315B92EE029CFA3AB9C3CD69`로 복원됐다. 따라서 수정 범위는 승인된 함수 한 곳이다. `C4-Relative`와 `C4-Sha`를 읽기 전용으로 불러 입력 목록 v3의 경로를 모두 검사·해시했으며 971/971 통과했다. 절대·상위 이동·빈 경로 구성요소·선행/후행 공백·끝 마침표·개행·널·콜론·역슬래시·별표를 포함한 부정 경로 12/12도 거부했다. 전체 capture 도구는 실행하지 않았다.

v3 소스 원장은 v2와 같은 102개 파일·순서·나머지 필드 값을 유지하고, `artifacts/c4-verify-required-rows.ps1`의 SHA 한 곳만 변경했다. 현재 원장에 기재된 102개 파일 해시는 전부 일치한다. `AllowedChanges`는 기존 20개 항목을 보존하고 해당 도구를 `REQ-M5D7QC4-007`·`AC-M5D7QC4-009/010`의 한정 경로 검사 사유로 한 건 추가해 21개다. v3가 실행 권위 원장이며 fixture가 기존 source manifest v1에서 검증하는 제품 소스 바이트는 유지된다.

입력 목록 v3는 v2의 969개 경로·사유를 모두 유지하고 v3 소스 원장과 Approved 경로 검사 계약을 추가해 971개로 만든다. 두 추가 경로가 `Added`로 정확히 사유화되고, 기존 884개 R11 경로와 source manifest v1/v2/v3는 모두 포함된다. 실행에서 직접 쓰이지 않는 입력 목록 v1/v2와 새 목록 자신은 제외한다. `Paths`와 `ReasonMap.Path`는 모두 `Sort-Object -CaseSensitive` 순서이고, 중복·미존재 경로·누락된 사유·출력 경로는 없다. 현재 Assets/Packages/ProjectSettings/qa 열거 886개도 전부 포함되며 `WholeAccepted:false`다.

앞서 발견한 ASCII 경로 제한 P1은 이 승인된 한정 수정과 v3 원장·입력 목록으로 닫혔다. 실제 입력 capture, queue plan, Unity 실행 및 전체 선택 검증은 후속 별도 승인 대상이다.
