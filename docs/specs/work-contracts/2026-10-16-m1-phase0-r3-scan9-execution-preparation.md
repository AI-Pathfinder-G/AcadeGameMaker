# M1 단계0 R3 작은 스캔9 실행 준비 호환 계약

- 상태: Draft
- 날짜: 2026-10-16
- 담당: Sol 작성, Luna 독립 검수, Astra 승인·통합, Root 실제 입력 결속·제출·원시 보존
- 현재 허용: 이 문서 신규작성1·조사1묶음·같은 범위 문구보정최대1
- SourceReady=false, executionApproved=false

## 1. 목적과 선행 판정

REQ-M1P0R3S9-001. 사용자 산출물은 현재 소스의 본편1챕터 직접 체험·완주 빌드다. 이번 중간 관측은 원장 전체 재실행 전에 작은 입력에서 스캐너의 정확 성공/거절·커서·단계·공유시계를 확보하는 것이다. 기존 R3 계약 2026-10-15-m1-phase0-r3-frame-scan-recovery.md §4·§9와 정상 교정 계약 2026-10-15-m1-phase0-r3-frame-probe-exact-normal-correction.md §3·§7을 함께 적용한다.

정상 교정 §7은 반환형 관측을 별도 미실행 사항으로 분리했다. 따라서 반환형 계측은 scan9의 필수 선행이 아니다. 다음 실제 핵심 선행 관측은 한 호스트의 합성 scan9다. 반환형 가설을 확정된 원인으로 쓰거나 별도 반환형 호스트를 자동 추가하지 않는다. 필수 판단이 바뀌면 구체 충돌과 별도 승인 전 실행0이다.

AC-M1P0R3S9-001. 독립 검수는 기존 계약의 반환형 분리 근거, frame3의 정상 보정·폐쇄 한정 성공, scan9가 아직 미실행임을 대조하여 이 우선순위를 판정한다. 반환형 미관측만으로 불필요 시험을 추가하지 않는다. 이 Draft 자체는 실행 승인이 아니다.

## 2. 실제 프레임 증거의 재사용 한계

REQ-M1P0R3S9-002. 결과 output/chapter1-playable-r1/evidence/m1-r3-three-case-rerun2-result1.json의 직접 읽기 전체핀은9010바이트/SHA256 954EB65F12FE7C0543BB5CB09536B8B9FA3D0EF100CEF2D9DCC8A002A0139841이다. Root 보고상 preflight1+case3=4호출, host3·followup0·kill0·세 사례 폐쇄 완료이며 Luna P0/P1=0, Astra 최종 한정 수용이다. baseline은 characterizationOutcome=exact-known-contamination·frameValid=false, normal은 characterizationOutcome=exact-normal·frameValid=true, failure는 characterizationOutcome=exact-intended-failure·frameValid=false·expectedFailureObserved=true를 각각 보존한다. 이 상태 요약·독립 판정은 Root 전달 근거이며 Sol의 신규 실행·전체 상태 직접 검증이 아니다.

재사용은 정확 핀이 일치하는 프레임 분할/UTF8·SHA·추가바이트 거절·첫 오류·EOF/exit/close·늦은 사건 보존 설계와 해당 작은 세 사례의 관측에 한정한다. scan9의 raw8192/receipt2048 및 다른 스키마/예상 결과에 frame3의 판정을 자동 적용하지 않는다. 수신기 원문을 고쳐야 하면 별도 정적 후보의 정확 차집합·독립 검수부터 필요하다. 같은 전역 상태·신원·소진 권위를 재사용하지 않는다.

raw child Buffer 내보내기0, 설치 직렬화 동일성 미입증, SourceReady=false를 유지한다. frame3 성공은 scan9·원장·부모/감독기·컴파일·Unity·제품 완료 증거가 아니다.

AC-M1P0R3S9-002. 실제 결과 전체핀과 재사용 가능한 구성요소핀/행동을 개별 대조하고, 새 스키마·상한·입력/결과 검증의 차이를 빠짐없이 표시한다.

## 3. 동결 네 후보와 입력

REQ-M1P0R3S9-003. 아래 경로는 모두 output/chapter1-playable-r1/evidence/ 아래이며 이번 직접 파일 읽기로 전체핀을 확인했다. 후보는 실행 권위가 아니다.

|파일|UTF8 바이트|SHA256|
|---|---:|---|
|m1-phase0-r3-scan-instrumentation-design.tmp|9979|14577B26FCE9740A413317313D9E3B4E4D8EABB62C228B8C73ED4EFE6C707452|
|m1-phase0-r3-scan-fixtures-design.tmp|28758|8E9BA5AFA5CB8C210C3C8C6EFAF46125307E50A2B90F91A574316A3B6E1CD75B|
|m1-phase0-r3-scan-probe-design.tmp|24459|624CDBB82C82B7E86888C06AAB4EA8F806C6B469243D8C085CA5188E555AE534|
|m1-phase0-r3-scan-binding-design.tmp|3961|3FD470598136B8D4E8094C1549B71C5ADE7984A3B96B108A58A4E4E061003EDB|

probe codeText20364/SHA256 26772EA661EC79E667CAC83541559760E92E4A2B7D439D435FA1F8E0E513557D, descriptorJSON3948/SHA256 9D5B6A94BF7BB262A28FCF384C753B1FA78CFD9AA8B699BB75D3F730B17BDE21를 결속 기준으로 한다. 원래 scanner9332/B9FDD278BADC2F602C8BF570E278318E6513BB8D6147C8D0766C5AAA274C2CA5와 삽입647바이트의 의미불변·역복구 기록을 보존한다.

입력 순서와 실제 크기는 ascii-1024=1024, ascii-16384=16384, escaped-unicode=88, duplicate-top-key=19, forbidden-early=54, forbidden-late=327, invalid-utf8=10, truncated-root=7, wrong-close-offset=8바이트다. 합계17921·최대16384이며 기존 각65536/전체131072 상한 안이다. 각 입력핀·고정 생성기·키/닫힘 기대값은 동결 fixtures 원문에서 결속한다. 큰 원장·접두사·복제본 입력0이다.

앞3개는 정확 정상, 뒤6개는 각각 동결된 중복키·금지키·잘못된UTF8·루트닫힘누락·닫힘위치불일치의 정확 거절이다. 예상 거절을 예상밖 실패로 취급하지 않고, 다른 예외·입력핀/결과/계측 불일치는 첫 예상밖 실패로 중단한다. 실제 raw에 completed·행 순서/핀·matchedExpected·첫 예외·계측오류·커서UTF16/Steps/Keys/Depth·시계를 남긴다. UTF16 커서를 바이트 진행률로 바꾸지 않는다.

AC-M1P0R3S9-003. 실제 입력9행 전량핀·순서·총량·각 한도·정상/정확 음성 기대값을 독립 대조한다. 계측 오버헤드를 무계측 성능으로 표현하지 않으며 작은 성공으로28MB의60초 완료를 보장하지 않는다.

## 4. 새 정적 준비와 실제 권위의 경계

REQ-M1P0R3S9-004. 후속 Approved 뒤 제안하는 신규 후보는 evidence 아래 m1-phase0-r3-scan9-execution-preparation1-design.tmp 한 파일이다. Root가 부재·부모 비링크·단일 소유를 먼저 확인하고 Sol이 수신/판정 코드·정확 도구인수/스키마·Root 제어·구성요소핀·출처/미소진 관문을 데이터로만 작성한다. 기존 네 후보 수정0이다. 새로운 동작이 필요하면 작성 전 정확 범위를 Astra에 반환한다.

새 session/nonce/상태·권위/승인·실제 출처·전달/Root 수신 확인은 현재 null/미발급이다. 역사 승인/배정/프레임 결과가 새 실행 승인이나 현재 전달을 대체하지 않는다. 권위원문과 독립 기대핀·명시 모델 배정/현재 승인 발신·키/필드집합·실제 코드/입력/효과·미소비를 교차 대조한다. 증거→gate→verification→최종 호출문→외부 최종핀 순으로 비재귀 결속하고 자기전체핀 삽입0이다.

모든 실행코드·필수원문은 최종 단일 도구 제출 본문에 직접 공개한다. 공유 자료는 정확 원문핀·추출 span·유일성·키순서·정수 표현·역복구로 확인한다. 파일/메모리에서 숨긴 코드 평가·압축·분할·다른 도구 우회0이다. 실제 UTF8 전체≤64000을 독립 실측하고 초과하면 제출0이다. 기존20364나 명령행 후보20515를 최종 전체크기 또는 승인 B로 대신하지 않는다.

실제 호스트 경로/전체핀·설치호환 가정·고정 PS codeText·actual argv JSON·인용모델 revision·Lmodel/B를 별도로 산출하고 상호 결속한다. 설치 libuv 동일성 및 실제 직렬화는 미입증으로 명시한다. 기존 frame3의 B/Lmodel을 승계하지 않는다.

AC-M1P0R3S9-004. 정확 새 준비 후보·도구본문·인수·권위/출처·argv/B·단회소비·최종 직접공개/64000을 독립 검수하고 Astra가 정확 최종 제출문을 별도 허가하기 전 Root는 실행을 차단한다.

## 5. 수량·공유시간·폐쇄·원시

REQ-M1P0R3S9-005. 후속 실제 상한 제안은 host1·scan9 각1회·재시도0·kill0이다. 원래 최초 sourceOrigin/frequency 하나에서 전체 작업60초·실패구성65초·회수70초를 공유하며 각 입력에서 재시작하지 않는다. 사전 입력 확인/생성/해시·계측 비용도 같은 호스트 예산 안이다. 첫 예상밖 실패 뒤 나머지 입력0이며 중단한 행은 미실행으로 남긴다.

Node80초 첫 실패·85/90초 비차단 후속 각최대1·도구timeout100000ms·Root 제어양보≤30000ms를 기존 제안 상한으로 보존한다. 정확 새 상태 이름 부재를 확인할 필요가 있으면 별도 고정 무효과 preflight최대1을 새 권위에 열거한다. 불필요 능력/반환형 시험을 추가하지 않는다. 이 경우 실행 도구 호출은 preflight1+host제출1+조건부 후속2=최대4이며 preflight를 scan/host 수로 세지 않는다. 후속은 새 host/scan을 시작하지 않는다. 실제 종료 보장이나 Root 벽시계의 단조성을 주장하지 않는다.

프레임 raw8192·receipt2048·meta512·stdout10752·stderr4096·모델영수증2048의 동결 상한, 엄격 분할·두 SHA·UTF8/JSON·스키마·행별 예상값·완료9·firstUnexpected=null·계측오류0을 전부 요구한다. 무절단·추가바이트0이며 초과/응답유실은 성공 보충0이다. stdout/stderr 원시 Buffer·첫/후속 오류·원래 ErrorRecord·부분출력·EOF/exit/close·미폐쇄·늦은 사건을 지속 상태에 보존한다. 요약만으로 원문 검사를 대신하지 않는다.

응답 미반환/상위 중단 때 intent만 남으면 호출진입·실제 host시작 미확정으로 보존하고 권위를 소비해 재시도하지 않는다. EOF·exit·close가 모두 관측되지 않으면 폐쇄 성공0이다. 저장은 별도 새 경로·wx·상한·동기화·닫힘·재판독·정확 저장권위가 필요하며 현재 경로/권위 null이다.

AC-M1P0R3S9-005. 전체 공유시간·누적 호출/scan/host 수·첫 예상밖 실패 종료·원시 무절단/폐쇄·늦은 사건·미확정 처리를 독립 검수한다. 예상 음성 결과와 전송실패를 구분한다.

## 6. 원장 접근·예산·종료

REQ-M1P0R3S9-006. Root가 조사 중 원장 전체에 ConvertFrom-Json을2회 잘못 적용한 절차위반은 별도 역사로 보존한다. 해당 조사결과는 이 설계의 근거에서 제외하고 파일변경0이라는 Root 보고를 기록한다. 이번 조사에서는 고정 작은 후보 JSON만 자료로 읽었으며 원장 전체 객체화/파싱/스캔0이다. 추가 원장 접근은 이미 추출된 고정 자료 또는 별도 승인된 선택 스캐너만 허용한다. 이번 문서가 큰 원장 재실행/복제/사전시험을 승인하지 않는다.

현재 사용은 조사1묶음·이 Draft 신규작성1·문구보정0이다. 후보구현·코드평가·Node/host·큰원장스캔·컴파일·Unity·원장/결과쓰기·권위발급0이다. 후속 정적 준비 제안 예산은 Sol 조사/구현1·지정 후보작성1·같은 범위 보정1·국소 자료검사1/보정 재검사1, Luna 독립1/재판독1, Astra 한정 수용1이다. 승인 전 사용0이며 기존 소진 예산을 복원하지 않는다.

AC-M1P0R3S9-006. 이번 종료는 문서 전체핀·다음 핵심 선행의 AC판정·미확정과 소유 반환이다. 실제 종료는 새 입력/출처/권위/최종본문 독립 검수와 별도 허가 뒤9행 정확 관측·원시/폐쇄·Luna 판독·Astra 한정 수용 또는 첫 실패 보존이다. 실제 관측 후 필요한 성능보정은 별도 정확 승인으로 다루고 큰 원장 재실행은 계속 별도다. SourceReady=false를 유지한다.

## 7. Astra 설계 승인·정적 준비 묶음 — Approved

Astra는 본문 전체를 직접 판독하고 Luna 최초 오염 표기 P1 및 보정 후 P0=0/P1=0을 근거로 REQ/AC-M1P0R3S9-001~006을 승인한다. 앞 Draft와 예산 표시는 작성 역사로 보존하며 이 절이 현재 한정 상태를 정한다. 기존11494바이트 접두사는 변경하지 않는다. 반환형 전체 원인 증명과 scan9를 분리하고, 다음 준비는 작은 scan9로 한정한다. 실제 scan9 수용·실행 권위는 아직 없다.

Root가 §4 신규 경로 부재·부모 비링크·소유와 기준 네 파일/코드핀을 확인한 뒤 Sol 단일 소유로 m1-phase0-r3-scan9-execution-preparation1-design.tmp 한 파일을 정적으로 준비한다. Sol 조사/구현1묶음·신규작성1·동일범위 보정최대1, 국소 자료검사1묶음·보정 재검사최대1, Luna 독립1·같은범위 재판독최대1을 발급한다. 사례/필드 수로 예산을 곱하지 않으며 과거 소진 예산을 복원하지 않는다. 범위 안 보정 외 새 동작/출처검사 변경은 작성 전 반환한다.

조사 첫 단계에서 전체 직접공개 크기 장부를 작성한다. 고정 PS20364와 제안 수신기, 모든 필수 출처/승인/권위/9입력 및 기대값 자료, 선언/조립/검사/호출 비용을 각각 계상한다. 현재 frame 수신기34274를 그대로 쓰면 두 코드만54638이며 남은9362가 전체 자료의 충분한 공간이라는 주장은 금지한다. 원문 공유의 정확 근거·단일 추출/유일성/핀과 새 scan 전용 판정 차집합을 함께 제시한다. 알려진 필수 크기만으로64000을 넘거나 필수 자료를 숨겨야만 가능하면 구현을 진행하지 않고 장부·최소 충돌을 보고한다. 미래 승인/전달의 길이는 미확정이며 초기 추정 통과를 실제 최종 본문 통과로 승격하지 않는다.

허용 수신/판정 준비는 기존 프레임·권위/출처·시계·원시/폐쇄 구조를 근거로 scan9의 고정 스키마·상한·9행 기대값과 단일 host/공유시간에 맞춘 정확 차집합을 명시하는 범위다. 필수 승인/출처/핀/정수/첫실패/폐쇄 검사를 삭제하거나 축소하지 않는다. PS/scanner/fixture 네 동결 후보는 불변이며 전체 원장 입력/파싱0이다. 코드 압축·난독화·간접평가·분할·다른 도구에 의한 검토 우회0이다. 실행함수/검사 선택에 새로운 권한 또는 규범 변경이 필요하면 구현 전에 반환한다.

국소 검사는 functions.exec의 직접 작성된 자료 처리 및 tools.exec_command의 PowerShell 파일 읽기/해시/작은 고정 JSON 처리로 한정한다. 문자열/키순서/정수형/핀/UTF8 크기/원문 추출과 자료 직렬화만 비교한다. 후보에서 읽은 코드의 eval/Function/AsyncFunction·receiver/PS/scanner/builder 실행0, Node REPL 호출0, 별도 시험 host/컴파일러 시작0이다. 구문검사나 실행시험을 이 예산으로 임의 추가하지 않는다. 전달 절단·미확정 원문은 성공으로 보충하지 않는다.

종료는 신규 후보 또는 조기 차단 장부의 정확 핀·범위/역복구·국소검사·독립 판정·남은 실제 입력/출처/argv/B/크기 관문 제출이다. 최종 정적 수용은 Astra가 결과로 판단한다. 새 preflight/host/scan·권위 발급·큰 원장·결과저장·컴파일·Unity는 이번 승인0이다. 원장 객체화2회 위반과 기존 실패/소진을 보존하고 SourceReady=false를 유지한다.


## 8. 단일 호스트 scan9의 권위·출처 인터페이스 호환 — Draft

이 부록은 기존15077바이트/SHA256 4685268369BB48EAEF23E5DAF5038E4C224F66DE63572AE27F8AF5D353FAC95E를 정확 접두사로 보존한다. 현재 허용은 부록작성1·같은 범위 문구보정최대1뿐이다. 아래 새 schema와 역사 proof 제외는 Astra Approved 전 구현 권위가 아니다. SourceReady=false다.

### 8.1 필수 요구 대응과 역사 보존

REQ-M1P0R3S9-007. 기존 frame3 권위/승인을 scan9에 재사용하지 않는다. 아래는 전용 수신기의 허용 변경 제안이며 일반 출처·안전 검사를 줄이지 않는다.

|기존 frame3 전용 요구|scan9 대응|
|---|---|
|baseline/normal/failure 순서·서로 다른3 nonce·PS3핀|단일 scan9 사례 nonce·고정 PS20364/26772EA661EC79E667CAC83541559760E92E4A2B7D439D435FA1F8E0E513557D·동결9입력 순서/핀/기대값|
|execute-three-cases-once·host3/scan0|execute-scan9-once·host1/scan9·각 입력1회·재시도0|
|frame3 schema/프레임 판정|scan9 전용 권위/출처 schema·기존 scan raw/receipt schema·정확9행/공유시계 판정|
|옛 target·옛 배정/승인3행과 sourceLogProof10703|역사 증거로만 보존하고 scan9 활성 입력에서 제외|
|현재 배정 proof·새 승인 전달·Root 수신/재확인|모두 유지하고 새 scan9 권위/승인/신원·코드/입력/효과에 교차 결속|
|80/85/90·첫 실패·원시·폐쇄|같은 관문 유지, 수량과 출력 cap만 §5의 scan9 값 적용|

옛 proof는 당시 frame3의 특정 원문·target·승인 검증 목적이며 새 scan9 승인 사실이 아니다. 이를 제외하는 근거는 범용 출처 증명을 없애기 위해서가 아니라 현재 배정과 새 실제 승인 전달을 별도로 검증하기 때문이다. 기존 파일·receiver·역사 원문/핀·실패·소진 권위는 수정0이다. 현재 proof4079도 새 실행 승인으로 취급하지 않는다.

### 8.2 정확 schema·공통 타입·권위

REQ-M1P0R3S9-008. 아래 키 목록은 순서까지 고정하며 추가/누락 키를 거절한다. 각 raw는 UTF8 엄격 디코딩·중복키/후행자료 거절·정수 원문 보존 파서로 검증한다. 보존 raw와 파싱값을 구별하며 재직렬화한 값을 원래 raw라고 주장하지 않는다. 내부 pin 키는 bytes,sha256이며 bytes는 양의 안전정수 JSON 숫자 토큰, sha256은 대문자16진64자다. input 바깥 네 pin만 bytes를 정규 양의 십진 문자열로 받으며 gate의 pin.bytes는 안전정수 Number로 받는다. 모든 타입/값·정확 길이/SHA를 교차 대조한다.

권위 schema=m1-r3-scan9-authority.v1, 정확17키:
schema,authorityId,sessionId,nonce,stateName,executionApproved,independentReviewAccepted,writingClosed,attempted,receiverPin,probePin,hostPin,contractPin,preflightCodePin,cases,limits,quotingModel.

신원은 새 비어 있지 않은 문자열128자 이하, stateName은 새 m1R3Scan9State_ 접두사의 영숫자/밑줄1~80자이며 실제 외부 상태 이름과 같아야 한다. executionApproved/independentReviewAccepted/writingClosed=true, attempted=false만 수락한다. 현재 값은 모두 미발급 null이다. cases는 정확1행이며 키 id,nonce,codePin, id=scan9·새 사례 nonce·동결 PS 전체핀을 요구한다. probePin은 동결 scan-probe24459 전체핀, hostPin은 path,bytes,sha256으로 정확 실제 host 경로/전체핀, contractPin은 이 계약의 승인된 실제 전체핀, receiverPin은 전용 수신기 실제 코드핀이다. probe가 참조하는 instrumentation/fixtures/descriptor/code핀은 동결값과 각각 대조한다.

preflight를 사용하면 정확 코드핀·실제5필드 영수증 schema=m1-r3-scan9-preflight.v1, 키 schema,stateName,available,sessionId,nonce와 available=true·새 신원 일치를 요구한다. 불필요 별도 시험을 추가하지 않으며 생략하려면 승인에 preflightCalls=0과 대체된 동등한 새 이름 부재 확인 경계를 명시해야 한다. 조용히 null로 통과시키지 않는다. 현재 기본 제안은 preflight1이다.

limits 정확키:
hostStarts,scannerCalls,toolCallsMaximum,preflightCalls,caseCalls,followupCallsMaximum,workSeconds,failureSeconds,drainSeconds,nodeFailureSeconds,followup85Each,followup90Each,killRequests,retries,rawBytes,receiptBytes,metaBytes,stdoutBytes,stderrBytes,modelReceiptBytes,toolTimeoutMs,outerYieldMaximumMs.
대응 값은 각각1,9,4,1,1,2,60,65,70,80,1,1,0,0,8192,2048,512,10752,4096,2048,100000,30000이며 모든 값과 키를 정확 대조한다.

quotingModel 정확키 id,sourceRevision,sourceSHA256,modelApproved,compatibilityAccepted,boundApproved,includesTerminationNul,boundByCase. id=libuv-quote-cmd-arg-fixed.v1, revision40자16진·출처SHA256·세 승인 boolean과 includesTerminationNul=true, boundByCase 정확키 scan9의 양의 정수 B≤32767을 요구한다. actual argv와 Lmodel을 해당 실제 host/PS로 계산해 Lmodel≤B≤32767을 확인한다. 설치 직렬화 동일성은 별도 미입증 가정으로 보존한다.

승인 schema=m1-r3-scan9-approval.v1, 정확16키:
schema,approvalId,approverRole,assignedModel,approvedAt,timeSource,verdict,sessionId,nonce,stateName,authorityRawPin,receiverPin,probePin,hostPin,contractPin,preflightCodePin.
approverRole=Astra, assignedModel=gpt-6-astra, verdict=execute-scan9-once와 새 신원 일치·실제 권위원문/구성요소핀 일치를 요구한다. approvedAt/timeSource는 실제 발급 관측만 사용하며 미확보이면 둘 다 null; 발명0이다. 발급시각 null이 실제 승인 전달/Root 수신시각 검사를 면제하지 않는다.

### 8.3 출처 raw·현재 배정·교차핀

REQ-M1P0R3S9-009. evidence schema=m1-r3-scan9-evidence.v1, 정확11키:
schema,approvalRawText,approvalRawPin,modelAssignmentRawText,modelAssignmentRawPin,receptionRecord,preflightRawText,preflightRawPin,preflightCodePin,newApprovalSource,currentAssignmentProof.
역사 sourceLogProof 키만 제외하며 현재 배정 검증과 새 전달 검사는 유지한다. 승인 raw≤8192·현재 배정 arguments raw≤5672·preflight raw≤2048·Root verification raw≤2048·현재 proof≤8192·evidence 전체≤67584의 기존 상한을 확대하지 않는다. 별도 raw의 원문길이/핀과 evidence 내 실제 문자열 합계·탈출 인용 증가를 실측한다.

현재 proof schema=m1-r3-current-assignment-proof.v1, 정확키 schema,parentLogPath,spawn,returned,argumentsPin,childMetadata,rootEvidenceReadAt,verificationScope를 유지한다. 기존4079/SHA256 5A5F60B10C182656F1ABBDDA1371591335126D2A903F76E342A8FD3DC38EEA4B 원문과 정확 일치·현재 Astra 배정의 지속 유효성을 확인한다. spawn/returned의 line,rawText,rawPin과 각 원문핀·동일 call_id·spawn_agent/실제 task_name/model/fork_turns·반환 task path·자식 session/parent/agentPath 관계를 그대로 검증한다. childMetadata 전체 원문을 불필요 복제하지 않으며 기존 독립 원문 대조 근거/핀은 보존한다. modelAssignmentRawText는 검증한 spawn.rawText의 payload.arguments와 정확 동일하고 argumentsPin과 일치해야 한다. gpt-6-astra는 배정 근거이며 독립 런타임 모델 조회 주장이 아니다.

newApprovalSource 정확6키 logPath,line,rawText,rawPin,messageId,rootEvidenceReadAt. 실제 새 부모 전달행 raw를 보존하고 정확 log 경로·양의 행번호·메시지 종류/ID·현재 승인 author=/root/astra_approval·실제 수신 대상/관계·UTC 기록시각을 대조한다. 전달 content에서 승인/권위 raw가 각각 정확1회 존재하는 고정 span과 독립핀을 검증하며 span은 실제 문자열 기준으로 산출한다. 임의 별칭/허용집합 확대0이다. 새 전달 raw는 기존 receiver의 exactRaw(ns.rawText,ns.rawPin,8192)와 동일하게 UTF8≤8192를 적용한다. 한 JSONL 행의 전체 원문·정확핀을 검사하며 초과 시 절단·생략·상한 확대 없이 거절한다.

receptionRecord 정확17키:
rootLocalRecordId,recordIdNamespace,sourceTaskPath,messageType,rootReceivedAt,rootTimeSource,approvalRawPin,modelAssignmentRawPin,assignedModel,assignmentTargetTaskPath,rootSourceVerificationRawText,rootSourceVerificationRawPin,sourceLogRecordedAt,sourceLogTimeSource,rootEvidenceReadAt,rootEvidenceReadTimeSource,currentAssignmentProofPin.
recordIdNamespace=root-local-not-tool-issued, 신원/ID 비공백128자 이하·task path256자 이하·messageType64자 이하, sourceTaskPath=assignmentTargetTaskPath=/root/astra_approval, assignedModel=gpt-6-astra를 요구한다. 실제 새 Root 수신시각은 관측한 UTC와 그 벽시계 출처만 기록하고 단조보장으로 쓰지 않는다. 원래 최초 처리시각을 못 얻으면 rootReceivedAt=null·rootTimeSource=historical-first-processing-unavailable로 명시하되 새 전달행 기록시각/출처·Root 실제 재확인 시각/출처는 각각 필수 UTC/비공백 문자열로 보존한다.

sourceGate schema=m1-r3-scan9-source-gate.v1, 정확16키:
schema,sourceTaskPath,messageType,assignmentTargetTaskPath,assignedModel,rootReceivedAt,rootTimeSource,rootLocalRecordId,approvalRawPin,modelAssignmentRawPin,rootSourceVerificationRawPin,sourceLogRecordedAt,sourceLogTimeSource,rootEvidenceReadAt,rootEvidenceReadTimeSource,currentAssignmentProofPin.

verification raw schema=m1-r3-scan9-source-verification.v1, 정확18키:
schema,recordIdNamespace,rootLocalRecordId,sourceTaskPath,messageType,assignmentTargetTaskPath,assignedModel,rootReceivedAt,rootTimeSource,approvalRawPin,modelAssignmentRawPin,modelAssignmentEvidenceDescription,sourceReceptionEvidenceDescription,sourceLogRecordedAt,sourceLogTimeSource,rootEvidenceReadAt,rootEvidenceReadTimeSource,currentAssignmentProofPin.

현재 proof 메타는 currentProofBytes=해당 proof 원문 UTF8 전체길이, currentProofStringBytes=spawn.rawText와 returned.rawText 각각의 UTF8(JSON.stringify(t)) 길이에서 인용부호2바이트를 뺀 값의 합, currentProofMetaBytes=currentProofBytes-currentProofStringBytes로 계산한다. 각 raw는 엄격 JSON이며 각 인용 내용 길이≤2×원문 UTF8 길이, 0≤currentProofMetaBytes≤2048을 요구한다. 제외 영역은 이 두 raw 문자열의 JSON 탈출 내용뿐이고 나머지 proof 메타는 모두 포함한다.

전체 evidence 잔여 메타는 UTF8(evidenceRaw).length-currentProofBytes-stringContentBytes로 계산한다. stringContentBytes는 approvalRawText,modelAssignmentRawText,receptionRecord.rootSourceVerificationRawText,preflightRawText,newApprovalSource.rawText 각각의 UTF8(JSON.stringify(t)).length-2의 합이다. 이 다섯 raw는 엄격 JSON·각 인용 내용 길이≤2×원문 UTF8 길이를 요구하며 0≤잔여메타≤4096이어야 한다. 제외 영역은 현재 proof 전체와 이 다섯 raw의 탈출 내용뿐이다. 기존 frame receiver의 역사 proofBytes 차감은 §8.1에서 역사 sourceLogProof를 제외했으므로0이며, 존재하지 않는 자료를 추가 차감하지 않는다. 이 계산은 구분자·키·인용부호·핀·record/gate 관련 나머지 메타를 생략하지 않는다. proof 메타2048·evidence 잔여메타4096·새 전달8192는 evidence 전체67584 및 다른 모든 개별 cap과 동시에 적용하고, 어느 하나 초과해도 절단·생략·상한 확대0으로 거절한다.

gate/verification/record의 공통 값은 타입 포함 정확 일치하고 승인·현재 배정·현재 proof·verification 원문핀을 각각 대조한다. 두 description은 실제 증거 경로/관계의 구체 비공백 설명이며 사실 미확보를 문장으로 대체하지 않는다. 외부 독립 기대핀과 실제 원문을 키 소비/host 시작 전에 대조한다. 반환봉투/영수증/요약을 원시 승인/출처로 대신하지 않는다.

### 8.4 발급·결속 순서와 scan 판정

REQ-M1P0R3S9-010. 순서는 동결 소스/수신기·입력/argv/B 및 외부 고정목록→독립 정적 검수→정확 새 preflight→별도 권위원문→그 원문핀을 참조한 승인→실제 전달행/Root 수신·재확인→verification→evidence/gate→최종 직접공개 도구본문·외부 전체핀→독립 실제 입력 검수→Astra 정확 최종 제출 허가다. writingClosed는 열거한 정적 입력 작성 종료이며 승인 이후 고정 입력이 달라지면 소비0이다. 후속 실제 증거 조립은 누락 입력/상한 확대를 허용하지 않는다. 자기본문 전체핀을 본문 안에 삽입하지 않는다. 현재 발급·실제 결속·도구 제출은0이다.

input 정확9키 caseId,codeText,codePin,receiverPin,authorityRawText,authorityPin,evidenceText,evidencePin,sourceGate를 유지하고 caseId=scan9다. 전용 receiver는 input/authority/evidence/approval/source/preflight/host/PS/argv를 모두 대조한 뒤 단일 host를 최대1회 시작한다. 한 preflight·한 case·조건부85/90 각1·누적4·미소비/latch를 유지한다. 이전 사례3개 상태/출처를 요구하지 않는다.

scan 결과 schema=m1-r3-scan-probe-raw.v1 및 m1-r3-scan-probe-receipt.v1은 동결 후보와 같아야 한다. 정확 raw/receipt 키·9행 키·정상/음성 기대값은 동결 PS가 생성하는 순서/타입과 전량 대조한다. completed=expected=9·행별 id/입력핀/예상값/시계·matchedExpected=true·firstUnexpected=null·계측오류0·정확 프레임/UTF8·EOF/exit/close를 모두 요구한다. 호스트 최초시계60/65/70·Node80/85/90·무절단 원시/부분/늦은 사건·첫 실패·미확정 소비·재시도0·별도 저장권위는 §5 그대로다. 수신 요약 schema는 m1-r3-scan9-case-receipt.v1로 별도 고정하되 기존 c/state의 원시/첫 오류·폐쇄 필드를 제거하지 않는다.

### 8.5 초기 크기·예산·종료

REQ-M1P0R3S9-011. 초기 장부는 PS20364·그 JSON 문자열20556, 기존 receiver34274, 현재 proof4079·동결 scan 프레임파서787이다. 기존 receiver의 숫자원문/핀 도우미3570, 권위입력구간1368, 일반 출처/승인구간5411, 역사proof 전용구간3701, 현재배정/새전달구간5986, preflight/host/argv구간3267, 종료/수집구간3178은 정적 행구간 실측이다. 이 구간은 scan 변경 전 frame 고정값을 포함하므로 새 receiver의 확정 크기나 최소값으로 더하지 않는다.

PS+기존receiver=54638의9362 여유는 미래 권위/승인/출처·인용 증가·조립/9행 기대값의 여유가 아니다. 전용 receiver 미작성, 실제 새 승인/전달 null이므로 최종 크기/여유는 미확정이다. 정확 자료는 공유하고 원문을 임의 축약하지 않으며 최종 매 호출 UTF8≤64000을 전량 실측한다. 알려진 필수 크기 초과·확정된 개별/메타/전체 cap 초과·필수검사/정확schema 충돌이면 작성 전 반환하고 상한 확대0이다.

이 부록 Approved 후 §7의 같은 지정 준비 후보·미사용 작성1/검사1·보정1/재검사1 안에서 재개한다. 최초 조사 사용과 중단 이력을 보존하고 추가 구현 예산을 발급하지 않는다. Luna 집중 판독1·같은 범위 재판독최대1이며 실제 권위/승인/Node/host/scan/원장/컴파일/저장0이다.

AC-M1P0R3S9-007~011. 대응표와 정확 schema/키·타입/상한·교차핀/발급순서, 역사proof 제외와 현재 출처 검사 전량 유지, scan9/시간/원시/폐쇄/단회, 초기/실제 크기 분리·비재귀·동일 예산을 확인한다. 현재 종료는 부록 전체/기존 접두사핀·미확정 반환이며 구현 승인을 소급하지 않는다.

### 8.6 Astra 인터페이스 승인·기존 정적 준비 재개 — Approved

Astra는 §8 전체를 직접 판독하고 Luna 집중 판독 및 세 상한 명확화 후 재판독 P0=0/P1=0을 근거로 REQ/AC-M1P0R3S9-007~011을 승인한다. 기존30767바이트 접두사와 Draft/중단/보정 이력을 보존하며 본 절이 §8의 현재 한정 승인 상태를 정한다.

scan9 전용 schema·단일 사례/host1·scan9 및 역사 sourceLogProof의 활성 입력 제외를 §8의 정확 범위에서 승인한다. 현재 배정/새 승인 전달/Root 시각·출처/독립핀/신원/정수/키/원시 검사는 유지한다. 세 상한 newApprovalSource8192·current proof 메타2048·evidence 잔여4096과 전체67584 및 다른 모든 개별 cap은 동시에 적용한다. 작성 중 누락/추가 동작/규범 충돌이 발견되면 임의 완화하지 않고 반환한다.

Root 경로 부재·부모·소유·기준핀 확인 뒤 Sol은 §7의 동일 신규 후보 m1-phase0-r3-scan9-execution-preparation1-design.tmp 준비를 재개할 수 있다. 최초 조사 사용은 유지하고 미사용 작성1·국소검사1·동일범위 보정1·재검사1만 사용한다. Luna 구현 후보 독립1·동일범위 재판독1은 §7의 남은 예산이며 이번 부록 설계 검수와 구분한다. 중복 예산 발급이나 소진 복원0이다.

최초 및 최종 크기 장부를 구분하고 필수 자료/코드의 알려진 합이64000을 넘으면 진행 중단·보고한다. 미래 승인/전달 미확정 길이로 최종 통과를 주장하지 않는다. 모든 실제 코드와 필수 원문 직접 공개, 후보 코드평가/Node/시험 host0, 큰 원장 접근0 및 §7 검사 수단은 불변이다. 구현 결과의 정적 수용, 새 정확 preflight/권위·승인/실제 입력 검수·최종 제출 허가·도구심사는 이후 별도 판단한다. SourceReady=false를 유지한다.