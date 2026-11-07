# M1 R3 출처 상한의 정확 계약 결속·파서 최소 복원

상태: Draft
작성일: 2026-10-16
설계·후속 정적 구현: Sol(gpt-6.1-sol) 단일 소유
독립 검수: Luna(gpt-6-luna)
승인: Astra(gpt-6-astra)
실제 입력·원장·외부 결속: Root

## 1. 목적·보존·현재 허용

REQ-M1R3EB-001. 본편1챕터 직접 플레이·완주 목표와 SourceReady=false를 유지한다. 기존 source-log-bounds §7의 정적4후보에 대한 P1 두 건, 즉 새 상한 계약의 실제 권위/물리파일 결속 부재와 파서 기준 선언·참조 불일치를 분리해 최소 정정한다. 기존 후보·독립 검토·P1·승인·실패·소진 예산은 보존한다. 이번 허용은 이 신규 Draft1파일 작성뿐이다. 후보 수정/평가·동적 검사·프로세스/호스트/스캐너/Unity·원장/결과·권위 발급0이다. 기존 예산의 잔여를 추정하거나 복원하지 않는다.

## 2. 정확 기준과 신규 경로

REQ-M1R3EB-002. 기준은 output/chapter1-playable-r1/evidence/ 아래 다음 네 파일이다. 작성 전에 Root가 기준핀·신규경로 부재·Sol 단일 소유를 다시 확인한다.

| 기준 파일 | 바이트 | SHA256 |
|---|---:|---|
|m1-r3-frame-small-observation-receiver-source-log-bounds-correction1-design.tmp|40331|11AA6E761100F2A5F0AAF15605474736A0AD8E558707A464C6A3E3A803D545ED|
|m1-r3-frame-small-observation-invocation-source-log-bounds-correction1-design.tmp|16061|5B7777912B16D42F9DAB7E01C4DEA1FEEF3D426F7A50DF305CB0904EE167238F|
|m1-r3-frame-small-observation-authority-template-source-log-bounds-correction1-design.tmp|22492|5C5BCDB0D29EB193FDEF492C2AC375CB98DA51CE4ACFFD1DAF14FD37A6CD80AB|
|m1-r3-frame-small-observation-preparation-source-log-bounds-correction1-design.tmp|30227|34D09C86F753E06D126F960C70B72AE3EEB068D30CF62BA7BCBAFBAC5EE1ABF7|

기준 receiver codeText는29926/E32BFACD13D6878CB19D2E3939E113C013F597FE86A16EDC0FF3FF609E1BDA08이다. 새 경로는 같은 폴더의 다음 네 개만 제안한다.

1. m1-r3-frame-small-observation-receiver-exact-binding-correction1-design.tmp
2. m1-r3-frame-small-observation-invocation-exact-binding-correction1-design.tmp
3. m1-r3-frame-small-observation-authority-template-exact-binding-correction1-design.tmp
4. m1-r3-frame-small-observation-preparation-exact-binding-correction1-design.tmp

이번 후보 작성0이며 후속 Approved 및 실제 부재/기준핀 확인 전 생성하지 않는다.

## 3. 계약핀 의미·정확 키 집합

REQ-M1R3EB-003. 선택안은 기존 authority.contractPin과 approval.contractPin의 의미를 현재 수신기 출처 상한/스키마를 지배하는 source-log-bounds §7 계약으로 명시 교체하는 것이다. 새 키 추가로 권위/승인 구조를 확장하지 않는다. 정확 값은 docs/specs/work-contracts/2026-10-16-m1-r3-frame-source-log-bounds.md,16053바이트,SHA C91162589871182CA960736030F737AD6EF0922B913F40EB3C676DC99944F998다. 이 교체는 후속 승인 대상 새 코드와 새 제안 권위원문에만 적용하며 과거 권위/승인 원문은 불변이다. 이번 보정 Draft 자체의 핀을 그 안에 재귀 삽입하지 않는다. 이 보정은 위 고정 bounds 계약에 대한 한정 구현 호환 승인 근거로 외부 결속한다.

기존 작은 관측 계약 docs/specs/work-contracts/2026-10-16-m1-r3-frame-small-observation-authority.md,21363/SHA 3C3603CFED38ABD184C9B0E277D9262EE7CD6D23E8DA66BF9D5F99BE296C9971은 host3·프레임·시간/도구 수량·시계·폐쇄 등의 원래 행동 계약으로 보존한다. 이를 새 상한 계약으로 소급 개정하거나 기존 승인 효력을 승계하지 않는다. 새 receiver는 이 기존 파일도 정확 고정 길이/SHA로 직접 읽어 대조하는 독립 의존 검사로 유지한다. 이를 새 authority 필드라고 주장하지 않는다.

authority 정확17키는 schema,authorityId,sessionId,nonce,stateName,executionApproved,independentReviewAccepted,writingClosed,attempted,receiverPin,probePin,hostPin,contractPin,preflightCodePin,cases,limits,quotingModel이다. approval 정확16키는 schema,approvalId,approverRole,assignedModel,approvedAt,timeSource,verdict,sessionId,nonce,stateName,authorityRawPin,receiverPin,probePin,hostPin,contractPin,preflightCodePin이다. 기존 parsedPin/samePin의 숫자 래퍼·외부 문자열 경계와 승인16키 대조는 그대로 유지한다.

실제 변경 위치는 receiver의 기존 fixed sources 조건에서 a.contractPin.bytes/SHA 각1곳을 새16053/C911…로 교체하고, 기존 물리 검사 배열의 계약 파일 경로1곳을 bounds 경로로 교체하는 것이다. 물리 검사에서 새 파일의 실제 bytes/SHA를 a.contractPin에 대조한다. 같은 출처 검사 구간에 이전 작은 관측 파일의 고정 길이21363/SHA3C360… 검사를1개 추가한다. 기존 precheck80 제한을 해당 읽기 뒤에도 적용하며 시간기산점/한도를 확대하지 않는다. 파일 읽기 실패/핀 불일치면 host 시작0, 기존 첫 오류/latch/원문 보존 경로로 반환한다.

template의 authority/approval contractPin 두 객체 및 외부 설계 wrapper의 활성 contractPin/준비 contract 참조를 새 값으로 전파한다. 관련 세 후보의 활성 receiver-file/code핀·candidatePins·경로와 미확정 설명만 전파한다. 역사 basis·sourceLogBoundsDerivation.contract·이전 차집합/원문은 보존하며 재귀 전역치환0이다. 실제 최초판독에서 정확 JSON 경로·발생수·이전/이후값을 기록하고 예상과 다르면 작성전 반환한다. 기존 evidence11/rr16/sg15/vr17/proof6/link6/child7/newApprovalSource6 구조는 불변이다.

## 4. 기준 파서 선언·참조 최소 복원

REQ-M1R3EB-004. 기존 R2 기준 parser sourceText의 p0N1.parse 대입 선언과 실제 알고리즘 원문2230바이트/SHA12CB2C93494E43585ECD712A9B687C0D18CA5490C04A8DFFC7C007F7491F0039를 기준으로 한다. 현재 후보는 const parse 대입대상으로 치환한 derivedText를 사용한다. 이 P1을 보존하고 새 후보에서만 기준 선언을 복원한다.

후속 구현은 함수 안의 파서 구간 직전에 const p0N1=Object.create(null); 한 개를 두고, 대입 구간을 위 sourceText 그대로 넣는다. 코드 내 로컬 parse(...) 호출만 p0N1.parse(...)로 정확 치환한다. 최초 한정 문자열 판독 결과 const parse = 선언1개와 독립 로컬 호출12개가 확인되었다. JSON.parse, Object 메서드, 설명/sourceText 원문은 치환 대상이 아니다. 후속 첫 판독에서 같은12개인지 재확인하고 다르면 작성전 반환한다. 객체 p0N1은 해당 run 함수의 로컬 상태이며 전역/지속 상태 또는 다른 case와 공유하지 않는다.

문자/이스케이프·중복·노드/깊이 한도·선택 raw span·numberText·정수·에러 문자열/발생조건·구문 소비 순서는 한 바이트도 바꾸지 않는다. 새 파서/JSON.parse 전체 대체/정수 완화/오류 숨김0이다. 원래 sourceText와 복원된 파서 대입 구간의 bytes/SHA를 정적으로 직접 비교한다. 실제 코드를 평가하거나 구문/성능 검사를 실행하지 않는다. 기존 parserReuse metadata의 역사 source/derived 원문은 보존하고 새 활성 선언/참조 차집합을 별도 기록한다.

## 5. 차집합·역복구·유한 예산

REQ-M1R3EB-005. 제안 새 예산은 Sol 최초 기준4파일 판독1묶음·각 신규파일 작성1회·동일 범위 보정/재읽기 전체최대1회, Luna 독립 전체판독1회·같은범위 재읽기최대1회다. 기존 bounds 작성·재읽기 소진과 독립이며 과거 예산 복원이 아니다. 이번 Draft 작성1회·동일범위 보정최대1회만 허용한다.

정적 검사에서는 기준핀/신규경로·위 계약 활성 참조·권위17/승인16 및 다른 정확키 불변·parser 기준2230핀·로컬 호출12·PS3/host/argv/시간/수량/출력/프레임/폐쇄/저장 불변을 한 묶음으로 확인한다. 기준 sourceText 선언 복원을 역치환하고 로컬 p0N1 객체 추가 및 독립 옛 계약 의존 검사를 제거하며 나머지 활성 계약핀/물리 경로·전파 참조를 이전값으로 복원했을 때 네 기준 전체 bytes/SHA와 정확 일치해야 한다. 신규 metadata를 포함한 정확 before/after 경로와 코드 구간을 기록한다. 실제 기존 파일 수정/삭제0이다.

후속 후보는 SourceReady=false·executionReady=false·권위 미발급을 유지한다. 새 receiver코드핀→새 제안 권위원문핀→실제 새 Astra 승인/전달 원문→외부 evidence/sourceGate·최종 Node호출문/argv/B/Lmodel 확인 순서로 결속한다. 실제 실행·저장 권위는 별도 관문이며 정적 수용으로 자동 발급하지 않는다. bounds 물리 계약핀은 동결 입력이며 보정 문서의 지문을 자기에 삽입하는 순환을 만들지 않는다. 실제 승인 이후 동결 입력 변경이 있으면 해당 핀을 임의 전파하지 않고 다시 반환한다.

## 6. 수용·종료

AC-M1R3EB-001: 두P1·기존4후보/승인/검토·소진 보존, Draft 외 쓰기/실행0, SourceReady=false.
AC-M1R3EB-002: 기준4핀과 새4경로·단일 소유·부재 확인 선행.
AC-M1R3EB-003: contractPin 새16053/C911… 교체·물리검사 및 과거21363/3C360… 별도 의존 검사·권위17/승인16 정확집합·활성 참조만 전파.
AC-M1R3EB-004: 기준 p0N1.parse 선언2230핀 복원·로컬 호출12만 치환·알고리즘/정수/오류 불변·평가0.
AC-M1R3EB-005: 정확 차집합/전체 역복구·새 유한 Sol/Luna 예산·독립검수/Astra Approved/새 정확 실행·저장 관문.

종료는 이 Draft의 경로·UTF8 bytes/SHA와 남은 승인 관문 반환이다. 실제 후보 작성은 이 Draft가 Approved가 되기 전0이며 코드 평가·동적 시험·호스트·원장·결과·권위0이다.

## 7. 정확 계약 결속·파서 복원의 네 후보 정적 준비 승인 — Approved

2026-10-16 아스트라는 이 Draft 전체를 직접 판독하고 Root가 전달한 루나 독립 설계 P0/P1=0 및 REQ/AC-M1R3EB-001~005 판정을 근거로 지정 네 신규 후보 작성·유한 정적 검수만 Approved로 확정한다. 이 기록 앞의9957바이트/SHA256 593F1FCD67291A212F91747B64B6B8C3E0A15B6C7925A8AEBBFAFA8A7785E7A5를 정확 접두사로 보존한다. 명시 배정 모델은 gpt-6-astra이며 독립 런타임 조회를 주장하지 않는다. 앞선 Draft·작성0·종료 표현은 당시 이력이며 이번 정적 허용에는 이 기록이 우선한다.

허용 신규 경로는 §2의 evidence 아래 m1-r3-frame-small-observation-receiver-exact-binding-correction1-design.tmp, m1-r3-frame-small-observation-invocation-exact-binding-correction1-design.tmp, m1-r3-frame-small-observation-authority-template-exact-binding-correction1-design.tmp, m1-r3-frame-small-observation-preparation-exact-binding-correction1-design.tmp 네 개뿐이다. Sol 단일 소유이며 Root가 기준4핀·신규경로 부재·공유소유·두 실제 계약파일핀을 확인한 뒤 작성한다. 첫 판독에서 계약검사 리터럴·물리경로·활성JSON경로·파서선언1/독립호출12의 정확 발생수·위치를 고정하며 불일치면 작성 전 반환한다.

새 후보의 authority/approval contractPin은 bounds 계약16053/C91162589871182CA960736030F737AD6EF0922B913F40EB3C676DC99944F998에 결속한다. 수신기의 정확 값 대조와 해당 물리파일검사를 함께 변경하며 이전 작은 관측 계약21363/3C3603CFED38ABD184C9B0E277D9262EE7CD6D23E8DA66BF9D5F99BE296C9971은 별도 고정 의존 검사로 보존한다. 추가읽기 뒤에도 기존 precheck80을 적용한다. 권위17/승인16키에 새 키를 추가하지 않고 과거 권위·승인 원문은 변경하거나 승계하지 않는다. 이번 보정 승인핀은 외부 근거로만 결속한다.

파서 구간 직전 로컬 const p0N1=Object.create(null); 한 개와 기준 p0N1.parse 대입 원문2230/12CB2C93494E43585ECD712A9B687C0D18CA5490C04A8DFFC7C007F7491F0039를 복원한다. 독립 로컬 parse 호출12개만 p0N1.parse로 연결하고 JSON.parse·다른 메서드·역사 source/derived 원문은 치환하지 않는다. 알고리즘·문자/이스케이프·중복/노드/깊이·정수 표현·오류·소비순서 불변을 정확 원문 비교로 확인한다. 로컬 객체를 지속/전역/사례공유 상태로 사용하지 않는다.

다른 세 후보에는 활성 계약/receiver-file/code핀·경로·관련 미확정 설명만 전파한다. 기존 evidence11/rr16/sg15/vr17/proof6/link6/child7/newApprovalSource6·PS3본문/host/argv·프레임/공유latch·시간/수량/출력/폐쇄/late·저장은 불변이다. 신규metadata를 포함한 before/after와 삽입/치환을 기록하고 역복구하면 네 기준 전체 bytes/SHA와 같아야 한다. 실제 원본 수정/삭제·역사 재귀치환0이다.

새 예산은 Sol 최초4파일판독1묶음·네 신규 각작성1회·동일범위 보정/재읽기전체최대1회, Luna 전체 독립판독1회·같은범위 재읽기최대1회다. 과거 소진 복원이나 파일별 증식0이다. 두P1은 실제 파생후보 독립 검수/Astra 한정 수용 전 해소로 표시하지 않는다. 기준/발생수·범위·예산 불일치면 중단하여 원문과 미충족을 반환한다.

후보 작성과 정적 원문/차집합 비교만 허용하며 코드평가·동적구문/성능시험·호스트/스캐너/Unity·원장/결과쓰기·실제실행/저장권위 발급0, SourceReady=false다. 새 수신기핀에 대한 정확 새 권위/승인·새출처·전체증거 상한·최종호출문/argv/B/Lmodel·미소진 관문은 유지한다. 종료는 네 후보핀·차집합/전체역복구·사용예산·미충족 반환이다. 승인 담당은 끝 기록1회만 추가했으며 접두사/최종지문 확인 뒤 문서 소유를 Root에 반환한다.
