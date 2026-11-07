# M1 단계0 R2 정확 식별자·지속 참조 보정

상태: Draft
작성일: 2026-10-15
설계/후속 정적 구현: Sol(gpt-6.1-sol) 단일 소유
독립 검수: Luna(gpt-6-luna)
승인·정확 권위·통합: Astra(gpt-6-astra)
원장·실제 결과 저장: Root 단일 소유

## 1. 목적·현재 허용과 역사

REQ-M1P0R2-001. 사용자 목표는 현재 소스의 본편1챕터 직접 조작·완주 로컬 빌드다. 이 문서는 단계0 선행 읽기의 다음 고유 R2 입력을 정확히 준비하는 설계이며 SourceReady=false다. 도구 정비를 플레이/콘텐츠 완료로 합산하지 않는다. 현재 허용은 이 새 Draft 문서1개 작성뿐이다. 실제 코드 후보 작성/평가·호스트/스캐너/부모/감독기/능력/Unity·결과파일/원장쓰기·새 실행 권위 발급0이다.

기존 단계0 실패의 hostSpawnAttempts=0·exact integer 첫 영수증과 권위 소진, 회수되지 않은 firstErrorObject.stack·원시, 모든 계약/후보/P1/검수/예산을 보존한다. host0은 같은 권위를 재사용할 근거가 아니다. 기존 실패의 미회수 원시를 새 코드/새 관측으로 재구성하지 않는다. 기존 원장28164706/SHA256 BD39951F248E153B784B4BC81FC10CAE163B585F6D7805CD8E6BE0E743840838은 역사 입력이며 새 실행 직전 최신핀을 별도 확정한다.

Astra는 새 직접 최상위 const 참조의 두 실제 호출에서 접근/동일참조/Buffer/4바이트 SHA 일치를 한정 수용했고 Luna도 AC-M1P0REC-R1-003을 그 범위로 한정 수용했다. Root 전달 요약의 코드1 길이544·지문접두사6E1C..., 코드2 길이971·지문접두사DD6E...는 전체 지문이 아니다. 실제 원문/전체핀/영수증·독립 수용 원문을 후속 준비에 정확히 결속해야 하며 생략된 지문을 발명하지 않는다. 두 관측의5초 이내 완료 수치는 확보되지 않았으므로 시간 상한 충족을 주장하지 않는다. 이 한정 수용이 실제 수집기의 firstError/raw/child/stream/late event 보존 전체를 입증하지 않는다.

## 2. 고정 R2 제안값과 기준핀

REQ-M1P0R2-002. 후속 Approved 뒤 적용할 값은 session m1-preparent-phase0-20261015-r2, nonce m1-readonly-two-key-absence-phase0-20261015-r2, 직접 최상위 상태명 m1PreParentPhase0StatesR2, authorityId capabilityPreParentR1Phase0ExternalReadAuthority2, 결과파일명 m1-preparent-r1-phase0-external-result-20261015-r2.json이다. 본문의 내부 resultId도 capabilityPreParentR1Phase0ExternalReadResult2로 제안한다. 이는 코드 식별자 제안이며 실제 권위 발급이 아니다. phase='phase0'와 목적·두 금지키는 유지한다. 기존 input/gate의 직접 최상위 이름은 불필요하게 바꾸지 않으며 실제 입력 데이터만 새 권위에 결속한다.

정확 기준은 output/chapter1-playable-r1/evidence/ 아래 다음 여섯이다.

| 역할 | 기준 파일명 | 바이트 | SHA256 |
|---|---|---:|---|
| 본문 | m1-preparent-r1-phase0-external-scan-final-contract-pin-design.tmp | 17233 | 489F8347F185001D5BBA1CDA33E1687DAAC63219CBE2D3E455E9447708BF4D90 |
| 수집기 | m1-phase0-input-boundary-recovery-r1-collector-design.tmp | 25064 | B3F7A8EE9C6C5F135D781A2A593A82DC53D7268E32D5057F459ADCEAB786AEDF |
| 권위설계 | m1-preparent-r1-phase0-external-authority-template-final-contract-pin-design.tmp | 27017 | BBA02AE5206D281BE6B37A4CB3217289060184FAF0BA455309E0D5CCFA9707B2 |
| 실행결속 | m1-preparent-r1-phase0-external-execution-binding-final-contract-pin-design.tmp | 56362 | 544408797BA97E29E27401B6622C54E5B919A90EBD2B7F23163FB00649D918B5 |
| 호출레시피 | m1-preparent-r1-phase0-external-invocation-recipe-final-contract-pin-design.tmp | 22380 | 726EAEAF12C3EE1D0CFDDE47E897BD3B520F03E8DF5BFD83C8B57ABB3D07CEC1 |
| 저장설계 | m1-preparent-r1-phase0-result-persistence-final-contract-pin-design.tmp | 35315 | 2BC05ECE019D6A17433AD299F22859C7EAC41733E91E626C640A4854D477BE86 |

기존 preparent Cfinal 계약235977/D1669F8001E63E3F89D52CDFC24DD2A0D4D98E0E7B3DE09FC247BDD20C2EA04B와 복구 계약19300/F3DFAE2D35A1448217BABF506AF236A3A7EE9C88FFBAB7CD1603B3A74D423B18은 변경하지 않는다. 이 R2 계약의 최종 Approved핀은 Root 외부 정확 권위에서 별도 결속한다. 현재 코드의 기존 물리 계약검사를 새 문서핀으로 자동 교체하거나 이 Draft 자기 최종지문을 안에 넣지 않는다.

## 3. 실제 최초 판독과 정확 변경 범위

REQ-M1P0R2-003. 이번 읽기 전용 최초 설계 판독에서 다음 실제 위치/발생수를 확인했다. 후속 구현 최초 판독 때 정확 기준핀과 함께 다시 고정하고 불일치면 작성 전 반환한다. 역사 원문/계보와 활성 JSON 참조를 구분하며 파일 전체 문자열 일괄치환을 금지한다.

- 본문: session/nonce 각1곳(각5행), authorityId/resultId 각1곳(각9행). 위 네 리터럴만 R2로 치환한다.
- 수집기: session/nonce/authorityId 각1곳(각88행). 상태명은7곳이며1행에2곳,5·7·44·75·142행에 각1곳이다. 1행의 조건문 안 var 선언 전체를 직접 최상위 const m1PreParentPhase0StatesR2=Object.create(null);로 바꾸고 나머지5곳을 R2 상태명으로 치환한다. 결과적으로 새 상태명은 선언1+나머지5=6곳이 된다. 그 외 firstError/raw/child/stream/late event 동작 코드를 재작성하지 않는다.
- 권위설계의 활성 JSON 경로: /authorityId 1곳, /sessionId와 /approvalSchema/sessionId 각1곳, /nonce와 /approvalSchema/nonce 각1곳. 승인/권위 객체의 정확 키 집합은 유지한다.
- 호출레시피: /followupCode의 상태명2곳과 /rawDestination의 상태명1곳을 R2로 전파한다. 입력11필드·별도 source gate JSON 데이터 조립은 유지한다. 실제 새 식별값/권위 데이터와 새 코드핀을 결속한다.
- 저장설계: /resultPath의 결과파일명1곳. /codeText 안 결과파일명1곳, session/nonce/authorityId 각1곳, 상태명3곳이다. 이 정확 리터럴만 바꾸고 새 저장 codeText bytes/SHA를 따로 계산한다. wx/상한131072·동결/pack/base64·sync/close/재판독·실패/부분파일 보존은 불변이다.
- 실행결속은 위 R1 session/nonce/authority/state/result파일명의 활성 리터럴이 없었다. 활성 기준/파생 코드핀·정확 후보/저장 경로·상태 참조 및 수용/미확정 기록을 새 R2로 연결한다. 역사 원문·차집합·실패·기존 소진은 유지한다.

위 JSON 판독은 derivation/finalContractPropagation/basis/deltas/inverse의 역사 부분을 제외했다. 네 설계의 bodyPin/collectorPin·승인 템플릿의 해당 핀·후보 경로/핀 표·저장 codePin 등 실제 활성 필드별 경로/기존값/새값과 발생수는 후속 최초 판독에서 완전한 차집합으로 기록한다. 역사 문자열과 임의 숫자1을 재귀 치환하지 않는다. 기준 권위/결속/레시피/저장설계의 수집기 참조는 복구25064/B3F7...에서 파생한 R2 핀으로 연결한다. 그 참조핀 교정 외의 동작 변경은 금지한다.

복구 후보의 세 gate Number 경계·기존 int/parse/samePin·네 exactKeys guard6/12/13/19, 스캐너9332/B9FDD·RequiredKeys없음/ForbiddenKeys2/scan1·호스트핀·argv8·원문상한/프레임6656·최초 QPC60/65/70·Node80/85/90·kill0/재시도0은 불변이다. 본문 식별자의 길이/인용문자 집합 변경을 실제 산술로 확인하고 새 모델B·파생핀 적용 수용과 최종 argv/Lmodel을 별도 관문으로 남긴다. 과거17471/17487을 자동 승인값으로 복사하지 않는다.

## 4. 지속 참조의 경계와 위험

REQ-M1P0R2-004. 새 상태 저장소는 실제 도구 입력의 직접 최상위 const여야 하며 if/비동기함수/블록/문자열 평가 안으로 감싸지 않는다. 기존 입력/gate 선언과 고정 수집기를 직접 이어 붙이는 호출 레시피를 유지하고 최종 실제 Node 본문의 외부 UTF8핀과 동일 전달을 Root가 확인한다. top-level await 전에 새 상태 참조가 만들어져야 한다.

p0N1은 R2 저장소의 phase0 속성에 보관한다. 기존 fail이 고정한 firstFailure와 firstErrorObject, 원문 authority/evidence/sourceVerification/approval, stdout/stderr Buffer와 실제 bytes/observed/rejected, firstReceiptText/원시 프레임, argv/시계·오류·최초/늦은 이벤트·원래 ChildProcess/스트림/리스너는 같은 p0N1 참조에서 유지한다. 내부 지역변수를 복사한 요약만 남기는 방식으로 바꾸지 않는다. 최초 영수증 뒤에도 원래 참조를 지우거나 실패를 성공으로 변경하지 않는다. 후속85/90 조회·Root 원문회수·저장 코드는 같은 R2 저장소를 사용해야 한다.

기존 p0N1이 만들어지기 전 입력/단회 검사에서 예외가 나면 p0N1.firstErrorObject에 저장됐다고 주장하지 않는다. 그 경계의 실제 도구 예외 원문은 Root가 별도 확보해야 한다. 이 계약은 최초 상태 생성 순서를 확장 재설계하지 않으며 그 원문을 회수하지 못하면 unavailable로 반환한다. 실제 복합 수집기의 후속 접근성·원문/프로세스 참조 생존은 아직 미검증이다. 앞선 const/4바이트 한정 관측을 전체 폐쇄/급사 정리 보장으로 확대하지 않는다.

REPL의 재선언 허용 때문에 같은 R2 코드 재전달 시 직접 const 초기화가 새 객체를 만들 수 있는 위험을 명시한다. 기존 phase0 속성 단회 guard만으로 전체 도구 재전달까지 방지했다고 주장하지 않는다. Root는 실제 도구 경계에서 새 R2 이름/nonce 미소진과 정확 단회 호출을 외부 기록하여 재전달0을 강제해야 한다. 기존 R1 저장소/실패/권위는 초기화/삭제하지 않는다. R2 저장소를 재초기화하는 fallback이나 자동 이름변경0이다. 실제 도구에서 기존 R2 바인딩이 이미 쓰였거나 미소진을 입증할 수 없으면 시작0이다.

## 5. 신규 후보·소유·역복구·정적 예산

REQ-M1P0R2-005. 후속 Approved 뒤 새 정적 후보는 output/chapter1-playable-r1/evidence/ 아래 정확 여섯 개로 제안한다.

1. m1-phase0-r2-exact-body-design.tmp
2. m1-phase0-r2-exact-collector-design.tmp
3. m1-phase0-r2-exact-authority-template-design.tmp
4. m1-phase0-r2-exact-execution-binding-design.tmp
5. m1-phase0-r2-exact-invocation-recipe-design.tmp
6. m1-phase0-r2-exact-result-persistence-design.tmp

Sol 단일 소유이며 Root의 실제 신규 경로 부재·기준핀·공유 소유 확인 뒤 각1회 작성한다. 기존 후보/계약/원장/결과파일을 덮어쓰거나 이동/삭제하지 않는다. 실제 읽기/저장 권위와 원문/source gate/latest input/argv/Lmodel·상태 접근성 전체 수용은 false/null로 남긴다. 고정 R2 제안 식별값을 실제 권위라고 표시하지 않는다.

제안 정적 예산은 Sol 최초 판독1묶음·각 새 후보작성1회·동일 범위 보정/재읽기전체최대1회, Luna 전체 독립 정적 판독1회·같은범위 재읽기최대1회다. 파일별로 곱하거나 기존 소진 예산을 복원하지 않는다. 실제 후보평가/동적 숫자·상태·성능 시험은 정적 예산에 포함0이다.

역복구는 본문의 네 리터럴, 수집기의 직접 const 선언 전체/나머지 상태참조와 세 식별자, 저장 codeText의 정확 리터럴을 기록한 이전 원문으로 복원하여 각 기준 bytes/SHA와 동일성을 확인한다. 네 설계의 활성 필드별 차집합과 기준 원문핀을 보존하여 역복구한다. 저장 codeText 자체 핀과 저장설계 파일의 전체핀을 구분하고 자신을 포함하는 지문을 안에 넣지 않는다. 최종 실행결속은 자신을 제외한 후보핀만 담는다. 실제 기존파일 수정/삭제로 역복구하지 않는다.

## 6. 새 실행·저장 권위와 종료

REQ-M1P0R2-006. 순서는 이 Draft의 Luna 독립 검수→Astra Approved→새 정적 후보/원문핀·정확 차집합/역복구·새 산술의 Luna 독립 검수/Astra 한정 수용→실제 최신 입력·닫힘·R2 미소진·정확 출처/배정/승인 원문·모델/B/최종 Lmodel·도구본문 외부핀 결속→Astra 별도 정확 단회 읽기 권위다. const 두 관측의 실제 코드/영수증/전체핀·독립 한정 수용 원문을 준비에 연결하며5초 수치 미확보를 보존한다. 실제 host1/scan1·85/90각1은 별도 정확 권위의 제안 상한이고 재시도0이다.

새 결과의 정확 경로는 output/chapter1-playable-r1/evidence/m1-preparent-r1-phase0-external-result-20261015-r2.json이다. Root는 host 전 literal 경로 부재·부모 실제 작업공간 범위/링크 여부를 확인해 실제 근거를 외부 결속한다. 파일/링크 존재·검사실패면 시작0이며 다른 경로를 즉석 선택하지 않는다. 이 Draft는 결과를 생성하거나 부재관문 시험을 실행하지 않는다.

읽기와 저장 권위를 분리한다. 실제 Root 저장은 새 저장 codeText핀·정확 호출입력·실제 Astra 저장 승인/배정/출처·정확 R2 경로·131072·단일wx/동기화/폐쇄/재판독·미폐쇄/부분파일/원문보존을 결속한 별도 권위 뒤만 가능하다. 읽기 본문과 수집기는 파일쓰기0이다. 원장 목적지 키 부재/쓰기 권한은 별도 미증명으로 남기며 결과/권위/후속 준비를 자동 append하지 않는다.

기존 Cfinal/복구 계약의 원문은 동결하고 이 계약의 최종 승인핀·새 코드핀·실제 입력/승인 원문은 외부 메모리의 정확 권위에서 비재귀 순서로 결속한다. 최종 실제 Node 호출문 핀을 그 호출문 안의 authority/evidence로 역삽입하지 않는다. 실제100000ms 지속보장 부재와 외부30초 이하 제어양보를 구분하며 선행 준비로 QPC60/65/70을 연장하지 않는다. 실제 원시/EOF/close/첫 오류·늦은 폐쇄를 사실대로 보존하고 SourceReady=false를 유지한다.

중단조건은 기준/발생수 불일치·범위 밖 참조/동작 변경·정적 예산 소진·R2 이름/nonce 기사용 또는 불명·원문/상태/출처 접근 불가·모델/B/argv/Lmodel/최신 입력·정확 읽기/저장 권위 미충족·결과경로 존재다. 기존 실패를 수정하거나 같은 호출을 반복하지 않고 확보 원문/미확정을 반환한다. 이번 종료는 새 Draft 경로·bytes/SHA와 실제 판독 범위/미충족 반환이며 후보/원장/결과파일/동적 실행0이다.

## 7. 수용 기준

AC-M1P0R2-001: 사용자 플레이 목표/SourceReady=false·기존 host0 실패/소진/미회수 스택과 const 관측의 한정 수용·5초 미입증을 보존한다.
AC-M1P0R2-002: R2 고유 제안값·기준6핀·기존 계약 동결·별도 최종 승인핀을 확인한다.
AC-M1P0R2-003: 실제 줄/발생수·활성 JSON 경로/핀 전파·한정 저장경로 변경·기존 경계/시간/프레임 불변·정확 역차집합을 확인한다.
AC-M1P0R2-004: 직접 최상위 const·p0N1과 firstError/raw/child/stream/late event 참조 연결·초기화 전 예외 한계·도구 재전달 재초기화 위험/단회 외부 관문을 확인한다.
AC-M1P0R2-005: 정확 신규6경로·단일 소유/부재·유한 정적/독립 검수·역복구와 지문 재귀0을 확인한다.
AC-M1P0R2-006: 최신 입력/모델B/Lmodel·실제 원시/폐쇄·별도 새 단회 읽기/저장 권위·R2 결과경로 부재·원장보류와 중단/실행0을 확인한다.

## 8. R2 여섯 후보 정적 준비 한정 승인 기록 — Approved

2026-10-15 아스트라는 이 Draft 전체를 직접 판독하고 Root가 전달한 루나 독립 정적 판정 P0/P1=0과 AC-M1P0R2-001~006 설계 부합 판정을 근거로 REQ-M1P0R2-001~006의 설계 및 지정 여섯 후보의 정적 작성·유한 검수만 Approved로 확정한다. 이 기록 앞의15737바이트/SHA256 0063C2A10FCB1D4E6015AF34C3295A48D3DF195C655D7D5442177902507D8A86을 정확 접두사로 보존한다. 명시 배정 모델은 gpt-6-astra이며 독립 런타임 모델 조회를 수행했다고 주장하지 않는다. 앞선 Draft·작성0·종료 표현은 작성 당시 기록이며 이번 정적 허용에는 이 승인 기록이 우선한다.

허용 신규 경로는 §5의 m1-phase0-r2-exact-body-design.tmp, m1-phase0-r2-exact-collector-design.tmp, m1-phase0-r2-exact-authority-template-design.tmp, m1-phase0-r2-exact-execution-binding-design.tmp, m1-phase0-r2-exact-invocation-recipe-design.tmp, m1-phase0-r2-exact-result-persistence-design.tmp 여섯뿐이다. Sol 단일 소유이며 Root가 실제 경로 부재·공유 소유·여섯 기준핀을 확인한 뒤 작성한다. 최초 판독에서 §3의 정확 발생수와 활성 JSON 필드별 이전/이후값을 고정하며 불일치·범위 밖 변경이면 작성 전 반환한다. 과거 원문/계보에 대한 재귀 치환과 기존 파일 덮어쓰기·이동·삭제0이다.

본문의 네 식별자, 수집기의 직접 최상위 const 선언 전체·나머지 상태참조·세 식별자, 호출레시피의 후속 조회/원문 참조, 저장 codeText의 고정 경로·상태·식별자 및 설계의 활성 코드핀 전파만 허용한다. 직접 최상위 선언을 함수/블록/문자열 평가 안에 넣지 않는다. p0N1과 원문/첫 오류/프로세스/스트림/늦은 이벤트 참조를 유지하며 기존 수집기의 다른 동작은 재작성하지 않는다. 세 Number 경계·원시 파서·정확 키 집합·스캐너·호스트·시간/프레임·저장 단일wx/상한/보존 동작은 불변이다. 정확 역복구와 각 전체핀·저장 codeText핀을 구분한다.

정적 예산은 Sol 최초 판독1묶음·새 여섯 각작성1회·동일 범위 보정/재읽기전체최대1회, Luna 전체 독립 판독1회·같은범위 재읽기최대1회다. 파일 수에 따른 증식·과거 예산 복원·후보 평가·동적 시험0이다. const 검증의 실제 원문/전체핀/독립 판정이 확보된 범위만 준비에 결속하고5초 미입증·복합 수집기 참조 생존 미검증·초기 상태 생성 전 예외 한계를 유지한다.

Root가 실제 도구 경계에서 R2 이름/nonce의 미소진과 정확 단회 전달을 보장해야 하며 새 const 선언이 재전달 초기화를 막는다고 주장하지 않는다. 기존 소진 권위와 실패 상태를 초기화하거나 새 관측으로 대체하지 않는다. 새 B 산술 및 파생핀 적용 수용·최종 argv/Lmodel·최신 입력·실제 출처/승인·결과 경로 사전부재는 별도 관문이다. 지정 R2 값과 후보 작성을 실제 읽기/저장 권위 발급으로 해석하지 않는다.

이번 승인은 후보 여섯의 정적 준비에 한정한다. 실제 호스트/스캐너/부모/감독기/컴파일/Unity·동적 관측·결과파일/원장쓰기·새 실행 및 저장 권위 발급0이며 SourceReady=false다. 기존 계약은 동결하며 이 문서의 최종 지문은 Root가 밖에서 산정한다. 후속 실제 승인 원문·입력·최종 호출문핀은 외부에서 비재귀 순서로 결속한다. 후보 지문·차집합/역복구·실제 예산·미충족을 반환하고 독립 정적 검수 및 한정 수용을 별도로 기록한다.

승인 담당은 이 문서 끝 기록1회만 추가했다. 정확 접두사 및 최종 지문 확인 뒤 문서 소유를 Root에 반환한다.
