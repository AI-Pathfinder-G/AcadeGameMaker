# Q0 포착·행 비교·실행 검증 라이브러리 적응

상태: **Approved** — 2026-10-02 실제 `gpt-6-astra` 승인. 소유 `REQ-PLAT-061/062/068/069/070`, `REQ-M5D7QC4-007`; 검증 `AC-PLAT-059/060/066/067/068`, `AC-M5D7QC4-009/010`, `AC-M5D7QC3-007/008`. 원계약4F9·중간 등록839·기존 입력 최종04AE 및 실패 증거는 불변이다. 이번 아스트라 작업은 기존 도구 읽기와 계약 작성이며 코드·키·Git·원격·Unity를 실행하지 않았다.

## 정확 허용 범위

준비 복제 `output/q0-trusted-child-r1/artifacts/`의 `c4-canonical-capture-inputs.ps1`, `c4-canonical-verify-required-rows.ps1`, `c4-canonical-verify-run.ps1`, `c4-canonical-verify-context.ps1` 네 기존 파일만 이 보완의 변경 대상이다. 원 저장소 파일과 불변 C3 도구를 바꾸지 않는다. 기존 큐의 이미 승인된 통합은 이 인터페이스를 사용한다. 새 소스·환경변수·프로세스 인증 역할·계획 필드·출력 경로를 추가하지 않는다.

읽은 현재 도구는 역사 Plan.Runs/SourceManifest/InputPathList 및 원래 출력 경로를 직접 요구하여 새 서명 실행 계획과 맞지 않는다. 이 차이를 아래 정확 결속으로 적응하며 188/377행의 기대 의미·결과 상태·판정 본문을 완화하지 않는다.

## 같은 큐 프로세스의 호출 경계

`LibraryOnly`는 함수 선언만 적재한다. 적재 시 인증 성공·파일 쓰기·자식 생성·Git 변경을 수행하지 않는다. 포착/행 비교/실행 검증 함수는 제한된 읽기와 계산으로 결과 객체를 반환하며 실제 증거 쓰기는 이미 활성화된 동일 큐가 정확 승인 경로에 CreateNew로 수행한다. 동일 함수의 외부 호출이 결과나 승인 플래그를 주입하여 큐를 대신하지 못한다.

효과 있는 큐 전용 진입마다 실제 PID·전체 시작 ticks·실행파일·큐/lease 소스SHA·실제 호스트 argv가 서명 TrustedParentIdentity와 승인 호스트 호출에 일치하는지 검사한다. 현재 메모리의 실제 호스트 활성 상태와 정확 execution-consumed 파일 결속을 함께 확인한다. 직렬화 TrustedRootContext, caller boolean, plan에 적힌 PID만으로 성공시키지 않는다. 키 없는 합성 상태는 운영 활성 상태로 승격할 수 없다.

미등록 일반 PlanPath/RepositoryRoot CLI는 exit2를 유지한다. 현재 C# 소유 Role=Verifier의 정확9토큰 입구와 SourcePath는 `qa/tools/Test-Q0FixedExecutionAuthority.ps1`이며 이 경로를 보존한다. 계획 VerifierLaunchRows/SourcePath와 C#의 고정 경로 문자열·호출을 `c4-canonical-verify-context.ps1`로 대체하지 않는다. 그 고정 helper가 실제 피어/슬롯/부모/서명을 검증한 뒤 최종13필드 응답을 내는 기존 경로를 유지한다. 이번 verify-context는 순수 함수와 실제 큐 신원 확인을 그 helper의 검증 함수에 연결하며 새 Verifier 진입점이 아니다. 라이브러리 재사용과 큐 전용 효과 허용을 구분하여 기존 자식 경로를 막거나 권위 없는 일반 CLI를 허용하지 않는다.

## 현재·역사 원장과 정확 출력

CurrentInputRows/CurrentSourceRows의 실제 서명 집합을 사용한다. 570은 현재 결과이지 고정 성공 조건이 아니며 중복·누락·추가·역할별 포함/제외를 전수 비교한다. CurrentSelectionManifest에서 정확 선택 이름·순서·개수·Selector를 읽고 NineRuns와 대조한다. 기대행 원장 파일 자체의 SHA는 현재 입력 행에 결속한다. 그 원장의 역사 SourceFiles SHA는 정확 승인된 준비/역사 원천과 대조하고, 실제 변경된 소스의 지문은 CurrentSourceRows와 실제 전후 포착으로 별도 검증한다. 역사 단언을 삭제하거나 현재SHA로 소급 치환하지 않는다. 기존 단언이 현재 의미 주장이라면 역사 역할로 이름만 바꾸어 우회하지 말고 정확 의미를 유지하거나 불일치로 실패한다.

Stem별 출력은 서명 Xml/Log가 같은 승인 외부 증거 루트에 있고 각각 정확 Stem.xml/Stem.log인지 먼저 확인한다. 그 같은 루트와 Stem에서 `.xml`, `.log`, `-source-before.json`, `-source-after.json`, `-native-exit-observation.json`, `-qa-tool-return.json`, `-required-row-comparison.json`, `-verification.json`의 정확8개를 결정한다. 저장소 내부·다른 루트·링크 이탈·다른 Stem·기존 출력은 거절한다. 포착의 입력 집합에 현재/미래 출력이나 계획/원장 자기SHA를 넣지 않는다. XML·로그는 실제 실행 산출물로 읽고 JSON은 완성 관측을 검증한 뒤 한 번 생성한다.

NativeExit는 QA가 실제 Unity 시작에서 보유한 핸들·전체 ticks·실제 argv·종료를 관측한 자료로 만든다. 시작자와 편집기가 다르면 DED9/839의 양쪽 핸들·등록 계보를 유지하고 실제 편집기의 자연 종료0을 구별한다. 큐/검증 함수는 그 기록을 실제 broker 등록과 비교한다. 실행자 사후 PID 검색만으로 직접 시작 관측을 대체하지 않으며 외부 강제 종료·미관측을 자연 종료0으로 쓰지 않는다. QA 반환0과 Unity 자연 종료0도 별개로 검증한다.

불변 C3 도구는 승인된 소스의 기존 인터페이스에 정확 외부 절대 인수만 전달하는 권위 없는 계산/검증 자식으로 호출할 수 있다. 큐가 실제 생성 핸들·전체 ticks·argv·기한·종료와 stdout/stderr를 소유하고 경로/부재·결과 원시바이트/엄격 형식·행 의미를 재검증한다. C3 자식에는 슬롯 생성·도전값·권위 요약 발급 권한이 없고 새 broker 인증 Role을 만들지 않는다. 기존 도구가 승인된8경로 밖의 새 출력이나 추가 입력을 요구하면 자동 확장하지 않고 해당 호출을 중단한다.

## 한정 검증과 잔여 관문

현재 계획의 정상 순수 포착/비교, 과거 계획 거절, 임의 CLI/직렬화 문맥/다른 PID·ticks/미소비 큐 거절, 입력 누락·추가·SHA변경·잘못된 역사 대체, 출력 루트/Stem/기존파일 거절, XML 이름/순서/개수 오류, 비정상 QA/Unity 종료,188/377행 의미 회귀를 기존 자체시험·승인 증거 경로에서 검증한다. 같은 프로세스 통합과 실제 원시 입력 대조를 확인하며 객체만 꾸민 시험을 운영 성공으로 부르지 않는다.

테라 구현 후 루나가 독립 검수하고 아스트라가 충족 범위만 수용한다. 새 키·실행 복제 Git·원격 쓰기·Authorize·아홉 운영 선택은 기존 미충족 관문을 유지한다. 다른 원본/동결 문서의 SHA를 변경하지 않는다.
