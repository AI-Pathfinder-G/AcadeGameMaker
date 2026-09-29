# R7 집중 시험 보정 독립 정적 검수

## 판정

승인된 시험 보정의 정적 구현 대조는 **P0 0건, P1 0건**이다. 이 판정은 구현·원장·선택의 정적 범위에 한정된다. Unity/C# 컴파일/Git 실행은 하지 않았고 R7 시험을 실행하거나 수용하지 않았다. 기존 R6 실행의 세 P1(실제 비활성화 콜백, Submit publication, 네 NUnit timeout)은 새 소스의 실제 실행으로 닫아야 한다.

## 승인·동결 지문과 변경 범위

- 적용 승인: [R6 집중 시험 보정 승인](</C:/Users/me/Documents/GPT-workspace/AcadeGameMaker/docs/approvals/2026-09-29-c3-r6-focused-test-correction-approval.md>), SHA-256 `4DCEB60D41ED5E2945B0550F022CA8DDAC213D89946D8A171DE782899B78B495`. 부모 C3 명세의 마지막 보정 단락도 읽었다. 설계 r2 SHA `DE6A88E4041114A4116720ED8639F3C17EA0C6D5AF27A4DE586863722DDEDE43`와 독립 설계 검수의 P0/P1=0에 부합한다.
- 최종 상위 14개 소스 원장 [r7 manifest](</C:/Users/me/Documents/GPT-workspace/AcadeGameMaker/artifacts/c3-upper-r7-frozen-source-manifest.json>): SHA-256 `F72905E1CBA99D4044B0500DF9EA78632598535245C5F88373CDDC631953DB59`. 원장 14개 전부 현재 파일 SHA와 일치했다. R6 14개 원장과 경로 집합을 비교한 결과 경로 증감 0, 현재 SHA 불일치 0, 변경은 승인된 Edit/Play 시험 파일 2개뿐이며 나머지 12개 SHA는 동일하다. 편집 파일은 `96B763452BF8A70B8C2232E9B93449D2576C871971A16A9C9D8D9A25CA1D7CE2`, 실행 파일은 `20B7EA2340402149B20DD0D7080FD4DFA1AFF400E701C6BE2050370E001BA3A4`다.
- 테라 변경 manifest SHA는 `A50BD5A475BF86E2E341C28A3DC7B0E95D70C0B5D4EC3FB3C09208A35C48AF7C`다. 여덟 항목의 사전 보존본은 모두 기록된 기존 SHA와 일치하고, 현재 파일은 사후 SHA와 일치했다. 실제 변경 파일은 두 시험, 예상행 생성기, 행 검증기, 두 새 집중 선택 및 새 예상행 원장이다. 집중 선택 생성기는 변경되지 않았다. 원본 파일 사본과 제한 diff가 남아 있다.

## 시험 및 행 원장 대조

Edit 집중 선택 241개와 Play 집중 선택 15개는 각각 선언 이름 수와 일치하고 중복 0·교차 중복 0이다. 원래 R6 선택과 집합 비교 시 Edit의 다섯 제거는 종전 disable 1개와 종전 AC002/AC004 네 케이스뿐이고, 그 자리에 disable 1개는 Play 선택에 추가되고 분할 행렬 사례 91개가 들어갔다. 기존 Edit 이름 150개와 Play 이름 14개는 그대로 남았다.

새 예상행 원장 SHA는 `EEA9CAFD13AAE95852DAB176FFEEC54AB85DD33902C4078553BDD10597B11364`, 매핑 정적 증거 SHA는 `BC123945AB0D4119E94CF15C50E008605D008244F152CA03968CB94D9E8A2DB0`이다. 원본 행 377개와 새 행 377개를 직접 순서대로 비교해 ID·그룹·역할·역할 이름·before/after·예상 결과·잠금 역할·상태·세대 규칙 차이 0을 확인했다. 추가된 원래 인덱스, 그룹 인덱스, batch 크기/번호, batch 안 인덱스 및 사례 이름도 377행 전부 분할 산식과 일치한다. 고유 사례 91개(분류 22, 전이 69)가 모두 Edit 선택에 포함되고 누락 사례·중복 ID 0이다.

시험 소스는 분류의 `[TestCase(0..21)]` 22개와 역할·batch 조합의 AC004 `[TestCase(role,batch)]` 69개를 명시한다. 분류는 원래 배열의 연속 8행(마지막 5행), 전이는 역할별 연속 3행(마지막 2행)을 처리한다. 원래 각 행 ID는 런타임 코드에서 동일 원본 배열과 `Batch`의 `GetRange`로 선택된다. verifier의 제한 diff는 과거 상위 단일 케이스 추정만 `ExpectedCase` 정확 이름으로 교체하고, 정확 case가 `Passed`, 행별 planned/passed 각각 1회, 실패·미지·중복 0, 실제 before/after·결과·authority·세대·source SHA 조건을 통과시켜야 하는 나머지 판정을 보존한다. 이 로직이 실행돼 377행을 수용했다는 주장은 하지 않는다.

종전 disable 시험은 PlayMode 기존 fixture 안에서 활성화·실제 입력 선택·opaque handle take 후 `Behaviour.enabled=false`를 수행하고 Owner/handoff 폐쇄, 늦은 take의 비변경 거절 및 snapshot/history/operation 보존을 확인한다. 해당 시험은 private `OnDisable`을 직접 부르지 않는다. EditMode의 기존 AC008 teardown 시험에는 기존 직접 메서드 호출이 남아 있으나, 이것은 새 정상 생명주기 증거를 대신하지 않는다. AC006 보정은 실제 키보드의 released `InputSystem.Update`를 successor 전에 두고 새 successor 뒤 첫 실제 `SubmitPressed=true` 프레임 폐기, 미활성화 및 다음 정상 release/Enter 이후 새 opaque take·새 decision을 계속 검사한다. 임의 입력 프레임·억제 상태 반사 설정·제품 API 변경은 보이지 않는다.

## 시험 실행 분할 계획

현재 생성된 artifact-only 분할 계획은 실행 전에 선택을 고정하기 위한 부수 자료다. builder SHA `E59510702285E18F7C4675C222FC9CEFCBAD493A1CAAD00E9E46424AE90A13D1`, 계획 JSON SHA `74168C2B727450213E3DA33F3FA3C17F8B0F7B455616A1A0D31B9C0300AFE92C`이다. 다섯 subset의 full-name escaped anchored selector가 각 subset 예상 이름과 정확히 맞고, 원래 sourcefile 참조가 유지된다. Edit는 1+91+149=241개, Play는 1+14=15개이며 원래 이름 합집합과 동일, 각 이름 membership 정확히 1, 중복·누락 0이다. 기존 QA 인자 serializer를 이용해 기록된 Windows 명령 길이는 각각 589, 14,453, 24,168, 608, 2,578자이며 32,767자 한계보다 작다. 이 계획은 합성 XML을 만들지 않고 실행·통과로 표시하지 않는다.

따라서 Edit의 Submit 단독, 91개 matrix, 나머지 149개와 Play의 disable 단독, 나머지 14개로 나누는 관찰 실행 계획은 승인된 선택 내용을 바꾸지 않는 분할로 성립한다. 다만 다섯 실행 각각이 정확 subset의 실제 XML·native exit·전후 884 입력 캡처 및 동일 R7 source manifest를 만족해야 한다. 이어 실제 matrix XML 하나에만 377행 verifier를 적용하고, 다섯 독립 실행의 실제 이름 집합을 별도 aggregate JSON으로 대조해야 한다. XML을 합성하거나 한 subset의 성공을 다른 subset에 전파해서는 안 된다. R6 당시 before/after 884 경로 지문 차이 0은 그 과거 실행의 사실이며 R7의 새 실행 지문으로 재사용하지 않는다. R7 manifest의 두 시험 지문 변화는 그 이후의 승인된 변경이다.

## 미완료 검증

R7의 정적 선택 241/15 및 예상행 매핑은 실제 실행 결과가 아니다. 제한 180초, 원래 377행, 기존 562/51/610 회귀 선택 및 Q0 pin은 바뀌지 않았다. AC-M5D7QC3-010의 집중 실행 수용과 기존 필수 회귀, 독립 사후 검수, 아스트라 통합 수용은 아직 남았다. 기존 R6 실패 XML·로그 및 원시 비교는 변경하지 않았다.
