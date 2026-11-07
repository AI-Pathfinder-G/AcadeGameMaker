# M1 R3 준비 설명의 권위 키 수 최소 정정

상태: Draft
작성일: 2026-10-16
설계/후속 정적 구현: Sol(gpt-6.1-sol) 단일 소유
독립 검수: Luna(gpt-6-luna)
승인·외부 핀 결속: Astra(gpt-6-astra), Root

## 1. 실제 P1·현재 허용

REQ-M1R3AK-001. 기존 작은 프레임 관측 준비파일의 newSchemaSets.authority는18로 기록됐으나 실제 receiver와 authority-template은정확17키를사용한다. Astra가이설명불일치를P1으로판정했다. 기존4후보·P1원문·정적검수/수용상태·실패/소진예산은보존한다. 이정정은권위스키마확대/축소나실제수신기동작변경이아니다. 사용자목표는본편1챕터직접플레이/완주이며SourceReady=false다.

이번허용은새Draft문서1개작성뿐이다. 후보작성/수정/평가·실제Node/동적시험/host/scanner·원장/결과파일쓰기·실제실행/저장권위발급0이다. 현재후보의정적재읽기예산은소진상태이며이초안으로기존후보를편집하지않는다.

## 2. 기준·정확 단일 차집합

REQ-M1R3AK-002. 기준경로는output/chapter1-playable-r1/evidence/m1-r3-frame-small-observation-preparation-design.tmp13424바이트/SHA256 32663250180BD1ED9FBF2347470868E1BA841B9C41F2C1E533A64A257E338D4B다. 정확대상JSON경로는/newSchemaSets/authority,기준131행의값18이며해당값출현1곳이다.

후속Approved뒤제안할유일한새후보는output/chapter1-playable-r1/evidence/m1-r3-frame-small-observation-preparation-authority-count-correction1-design.tmp다. Sol단일소유로Root가실제신규경로부재·정확기준핀·공유소유를먼저확인한다. 현재그후보작성0이다.

허용변경은그숫자값18→17뿐이다. UTF8 BOM없는기준의0부터세는값시작바이트위치는9156이며ASCII31 38이31 37로바뀐다. 실제단일변경은offset9157의38→37(16진수)이고파일길이13424는불변이다. 다른같은숫자·설명·metadata·계보·핀·codeText를전체치환하지않는다. 기존개행/인코딩/키순서/공백을바꾸거나전체JSON재직렬화하지않는다.

실제정확키17개는schema,authorityId,sessionId,nonce,stateName,executionApproved,independentReviewAccepted,writingClosed,attempted,receiverPin,probePin,hostPin,contractPin,preflightCodePin,cases,limits,quotingModel이다. 수신기m1-r3-frame-small-observation-receiver-design.tmp31374/SHA256 377DB6EBA5A2B317ABFD1A36CADB1EFECB0ACCC436B2459852C9C94739EB235F와권위템플릿m1-r3-frame-small-observation-authority-template-design.tmp7179/69D54407C698FF37202B4513639A05365F3CD86BE17B2B0FDF3ED75E2DDCADF2는변경하지않는다. 실제authority객체의17키와설명숫자의일치만회복한다.

새예상후보핀은아래외부계산값이다. 이는메모리상의단일바이트치환계산이며후보파일작성/실행/권위발급이아니다.

13424바이트 / SHA256 A430C48C3EBB0115A09CD907923FFC0A4A4913277BBE5E735589DA878BF01762

역복구는새후보offset9157의37→38을되돌려기준13424바이트/SHA와bytes동일함을확인한다. 변경점집합은정확1바이트,앞9157바이트와뒤9158부터끝까지는기준과동일해야한다. 실제기존파일수정/삭제로역복구하지않는다. 새후보에계보필드나자기지문을추가하지않는다.

## 3. 외부 결속·예산·종료

REQ-M1R3AK-003. 새후보의실제길이/SHA·기준/차집합/역복구는Root가외부메모리기록에결속한다. 기존invocation/authority-template/receiver/기존preparation의참조핀을이번범위로수정하지않는다. 후속실제준비에서활성준비핀을교정후보로사용할정확범위는외부결속/별도승인으로정하고과거핀을소급변경하지않는다. 원장끝추가·전체재직렬화권한은없다.

제안할새정적예산은Sol최초판독1묶음·새후보작성1회·같은범위보정/재읽기전체최대1회,Luna독립전체판독1회·같은범위재읽기최대1회다. 기존소진을복원하거나파일별증식하지않는다. 실제정수값·정확단일offset·나머지bytes동일성·역복구·receiver/template17키불변을같은묶음으로검수한다. 후보평가/동적JSON시험/프로세스시험0이며정적자체비교와Luna독립판정을구분한다.

기준/발생수/offset불일치·단일값외차이·예산소진이면작성/후속수정을멈추고P1을보존하여미충족반환한다. Luna독립검수/AstraApproved및새후보수용전P1해소를선언하지않는다. 실제고유신원·출처·최신host/계약핀·argv/B/Lmodel·실제Node본문/원문/폐쇄·새단회실행/저장권위는계속미확정이다. 숫자설명교정으로실제권위를발급하거나SourceReady를올리지않는다.

이번종료는Draft경로·전체UTF8길이/SHA·REQ/AC·예상단일바이트차집합과남은관문을Root에반환하는시점이다. 새문서외후보/원장/결과수정·Node/동적/host/scanner실행0이다.

## 4. 수용 기준

AC-M1R3AK-001: 기존P1·4후보/소진·SourceReady=false와이번Draft만허용을확인한다.
AC-M1R3AK-002: 정확기준핀·신규단일경로·/newSchemaSets/authority18→17·offset9157한바이트·길이/역복구·실제17키불변을확인한다.
AC-M1R3AK-003: 외부핀결속·기존참조핀미변경·새유한Sol/Luna예산·독립수용/실제관문미충족·문서외실행0을확인한다.

## 5. 단일 준비 후보 정적 파생 승인 — Approved

2026-10-16 아스트라는 이 Draft 전체를 직접 판독하고 Root가 전달한 루나 독립 설계 P0/P1=0 및 REQ/AC-M1R3AK-001~003 판정을 근거로 신규 단일 정적 파생 후보 작성·유한 검수만 Approved로 확정한다. 이 기록 앞의5466바이트/SHA256 215D591409CA49BDCA71BB7C9B58B8E54C9EB59302D8585C3384CE07D86FB50B를 정확 접두사로 보존한다. 명시 배정 모델은 gpt-6-astra이며 독립 런타임 조회를 주장하지 않는다. 앞선 Draft·작성0·종료 표현은 당시 이력이며 이번 허용에는 이 승인 기록이 우선한다.

허용 신규 경로는 output/chapter1-playable-r1/evidence/m1-r3-frame-small-observation-preparation-authority-count-correction1-design.tmp 하나다. Sol 단일 소유이며 Root가 경로 부재·공유 소유·기준13424/32663250180BD1ED9FBF2347470868E1BA841B9C41F2C1E533A64A257E338D4B를 확인한 뒤 작성한다. 허용 변경은 /newSchemaSets/authority의18→17, 정확offset9157의0x38→0x37 한 바이트뿐이다. 길이·나머지bytes·개행·인코딩·키순서·공백을 보존하고 전체JSON재직렬화·계보필드 추가·다른18 치환0이다. 발생수·위치·기준핀 불일치면 작성 전 반환한다.

실제 파생핀은 작성 뒤 별도 확인하며 예상13424/A430C48C3EBB0115A09CD907923FFC0A4A4913277BBE5E735589DA878BF01762와 대조한다. 단일 바이트 역치환으로 기준 원문 전체 동일성을 확인하고 원본 수정/삭제로 복구하지 않는다. receiver와 authority-template의 실제17키·동작·핀, invocation과 기존preparation의 참조핀은 변경하지 않는다. 새 활성 준비핀 사용은 외부 결속으로 구분하며 과거 핀을 소급 갱신하지 않는다.

새 예산은 Sol 최초판독1묶음·새후보작성1회·같은범위 보정/재읽기전체최대1회, Luna 전체 독립판독1회·같은범위 재읽기최대1회다. 기존 소진 복원·예산 증식0이다. 단일 차집합/역복구·17키 불변을 한 묶음으로 확인하며 후보 평가·동적JSON/프로세스 시험0이다. 실제 새 후보 독립 검수와 Astra 한정 수용 전 P1 해소를 선언하지 않는다.

기존4후보·P1·검수/수용 원문·실패/소진은 보존한다. 실제 신원·출처·host/계약·argv/B/Lmodel·Node본문·원문/폐쇄·정확실행/저장권위는 별도 미충족이다. 이번 승인으로 실행·원장/결과쓰기·권위발급0이며SourceReady=false다. 실제 후보 지문·차집합/역복구·예산·미충족 반환으로 정적 준비를 끝낸다. 승인 담당은 이 문서 끝 기록1회만 추가했고 접두사/최종 지문 확인 뒤 소유를 Root에 반환한다.
