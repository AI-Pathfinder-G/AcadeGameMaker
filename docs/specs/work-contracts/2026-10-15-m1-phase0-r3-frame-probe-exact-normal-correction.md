# M1 R3 프레임 정상 사례의 정확 구문 교정

상태: Draft
작성일: 2026-10-15
설계/후속 정적 구현: Sol(gpt-6.1-sol) 단일 소유
독립 검수: Luna(gpt-6-luna)
승인·정확 권위: Astra(gpt-6-astra)
원장·실제 결과 저장: Root 단일 소유

## 1. 목적·현재 허용·P1 보존

REQ-M1P0R3N-001. 사용자 목표는 현재 소스의 본편1챕터 직접 조작·완주 로컬 빌드이며 SourceReady=false다. 이 문서는 R3 프레임 작은 재현의 정상 사례가 실제 frame-body 보정과 다른 구문을 사용한 P1의 최소 교정 설계다. 기존 R3 계약21460바이트/SHA256 C6F0357625BE05750FC8D2EA4FA1011A084C67F3158134A467A962BDCD91A09E, 기존8후보·정적 검수/한정 수용·P1 원문·실패·소진 예산을 보존한다. 스캐너 계측/9사례/정적 수용 영향0이다.

현재 허용은 이 새 Draft 문서1개 작성뿐이다. 후보 수정/작성/평가·동적 재현·호스트/스캐너/Unity·원장/결과파일쓰기·새 실행/저장 권위 발급0이다. 기존 정적 수용을 실제 호스트 실행 승인으로 바꾸지 않는다.

P1은 정상 사례가 task/awaiter/value를 변수에 받아 반환형을 기록하는 반면 실제 프레임 본문은 $null = 완료호출이라는 점이다. 양쪽 모두 출력 억제 효과가 있을 수 있어도 정상 사례가 정확 동일한 보정 구문을 시험했다고 할 수 없다. 반환형 계측 자체의 변수 할당이 출력에 영향을 준다는 한계를 보존하고 이 차이를 설명문만으로 해소했다고 표시하지 않는다.

## 2. 정확 기준·단일 정상 분기 교정

REQ-M1P0R3N-002. 기준은 output/chapter1-playable-r1/evidence/ 아래 m1-phase0-r3-frame-probe-design.tmp13599바이트/SHA256 73EC5FA9379CAFCA0D4BE7EA739289A153C9EAE1257A80272B86AE7CA4EBE54B다. cases 중 id='normal'의 codeText3081/SHA256 79648642919925E629FA492F7CD4D8F1098BA67F718D406F72928461DB5C6484,17행에 기존 normal 분기가 정확1곳 있다. 실제 최초 판독에서 해당 위치/발생수/이전 구문핀을 고정하고 다르면 작성 전 반환한다.

정확한 교정 후 분기는 다음 한 줄로 제안한다.

~~~powershell
   elseif($mode-ceq'normal'){$null = $stream.WriteAsync([byte[]]$part,0,$part.Length,$ct.Token).GetAwaiter().GetResult()}
~~~

이는 m1-phase0-r3-frame-body-design.tmp17241/SHA256 CD2FCF5F986CC98521ECEAF1832F4C093B9A111823E3F57236C7892E5B1A69A8의 선택된 완료값 억제 호출과 문자 단위로 같은 호출식이다. 정상 루프는 같은 식으로 meta/raw/receipt 세 part를 각각1회 보낸다. task/awaiter/value 계측과 types.Add는 이 normal 분기에서 제거한다. 그 외 raw/receipt/meta 생성·길이/SHA·WriteAsync 인수·취소·QPC/60·65·70·Flush·첫 오류/진단/폐쇄 동작은 변경하지 않는다.

기준 JSON의 baseline/failure 사례 codeText는 각각3083/DF8010321B5882C3077DAD6192246BC552F54D50282AE9894595089B81322BE0,3082/5FFFAC418CBA4D810B3F01D3CE363B50939347B615A8555DF946B0ADAD70C187로 그대로 보존한다. 각 코드 안에 있는 실행되지 않는 normal 분기까지 전체 치환하지 않는다. 고정mode가 외부 인수로 바뀌지 않으며 해당 사례의 codeText핀도 불변이어야 한다.

정상 codeText의 types 목록은 기존 초기화/진단 출력 경로를 유지하므로 빈 배열이 된다. 정상 판정 설명은 types3개/반환형 관측 성공을 요구하지 않고 types빈배열·firstError=null·mode normal인 작은 진단stderr와 엄격3구간을 요구하도록 새 설계에서 정확히 연결한다. 이미 types3개를 관측한 것처럼 기록하지 않는다. baseline과failure의 별도 진단은 기존 범위를 보존한다. 실제 단계0의 stderr0 규칙을 정상 프로브의 진단stderr 허용과 혼동하지 않는다.

## 3. 세 증거의 분리와 호스트 수량

REQ-M1P0R3N-003. 기본 경로는 기존 프레임 host3 제안 수량을 유지하며 새 반환형 계측 호스트를 자동 추가하지 않는다.

- baseline: 원래 미억제 완료 체인의 stdout 전체를 그대로 관측한다. 추가30 0D 0A가 재현돼도 frameValid=false를 유지하고 가설과의 일치 여부만 별도 기록한다. 재현되지 않으면 원인이 재현됐다고 주장하지 않는다.
- normal: 정확 $null = 완료호출로 보낸 세 구간의 엄격 길이/SHA/추가bytes0·실제 UTF8/JSON·firstError없음·EOF/exit/close를 확인한다. 정상 성공은 그 보정 경로의 증거이며 baseline에서 관측하지 못한 반환형을 입증하지 않는다.
- failure: 첫 part 뒤 명시된 시험 예외·부분 stdout/실패진단·폐쇄를 확인한다. 불완전 프레임을 정상 성공으로 바꾸지 않는다.

반환형 계측은 위 정상 증거에서 분리한 미실행 설계 사항으로 남긴다. 기존 정상 코드의 변수 할당/형식 기록은 역사 원문으로 보존하며, 그 구문이 출력 억제를 포함하므로 원래 미억제 stdout과 같은 관측이라고 부르지 않는다. 기존8후보에 별도 실제 반환형 관측 성공 증거가 있는 것처럼 소급하지 않는다. 이 교정 범위에서는 반환형 관측 코드를 새로 작성하거나 실행하지 않는다.

실제 반환형의 독립 관측을 반드시 원하면 새 별도 관측 코드/입력/원문 수신/명령행·권위와 host1 추가가 필요하다. 이 경우 프레임 전체 수량은3→4가 되므로 정확 호환 수용과 실제 단회 권위를 먼저 받아야 한다. 이 Draft의 기본host3이나 향후 정적 준비 승인을 host4 실행 승인으로 해석하지 않는다. 그 추가 승인 전 수량 확대0이며 실제 반환형은 미입증이다. 원인 가설의 관측·정확 보정의 정상 증거·반환형 관측을 서로 대체하지 않는다.

## 4. 신규 후보·활성 핀 전파·역복구

REQ-M1P0R3N-004. 후속 Approved 뒤 Sol 단일 소유로 제안할 경로는 output/chapter1-playable-r1/evidence/ 아래 다음 세 개뿐이다. 현재는 작성0이다.

1. m1-phase0-r3-frame-probe-normal-correction1-design.tmp
2. m1-phase0-r3-frame-binding-normal-correction1-design.tmp
3. m1-phase0-r3-recovery-preparation-normal-correction1-design.tmp

Root가 실제 신규 경로 부재·공유 소유·정확 기준핀을 먼저 확인한다. 기존 frame-body 및 다른7후보는 수정하지 않는다. 기존 frame-probe도 보존하고 새 후보로 파생한다. 결속 기준은 m1-phase0-r3-frame-binding-design.tmp3421/SHA256 2AF028CB0AE62896305921B23CC1A78EBE1DBE6AF0E987BFB5EDE33D3DDD2FE2, 준비 기준은 m1-phase0-r3-recovery-preparation-design.tmp5344/5ED277A0529547A42C3EC9BCC29FBF7D3BDE40ABECFA20D8531F546336AF07C1다.

새 probe는 cases에서normal의 codeText1개 및 그 codePin만 직접 파생하고 normal의 기대진단/정상증거 설명·반환형 계측 미승인 표시를 교정한다. baseline/failure 원문/핀·수신기의 엄격 프레임 parserCodeText·원시 상한·시간/호스트 수량은 불변이다. 새 binding/preparation은 새 probe 파일핀·정상 case codePin·관련 경로·정상 기대값 설명과 미확정/계보만 활성 전파한다. 입력/동적 권위/실제 Node 수신본문·새 session/nonce/상태/출처·B/Lmodel·저장경로는null/false로 남긴다.

활성 JSON 경로별 이전/이후값·변경수·실제 전체bytes/SHA와 선택한 구문을 최초 판독에 기록한다. 역사 basis/raw/차집합/P1 원문은 재귀 치환하지 않는다. 구문 길이/인용 집합이 바뀌므로 정상case의 명령행 모델 후보 산술과 코드핀을 새로 계산하되 기존3246이나 다른B 수용을 자동 적용하지 않는다. 실제 argv/Lmodel·정확Node본문 외부핀은 후속 실제 준비에서 별도 확정한다. 자신을 포함한 전체 지문을 자신의 원문에 넣지 않는다.

역복구는 정상 codeText의 새 분기를 기록한 옛17행으로 정확1회 되돌려3081/796486... 원문과 동일함을 확인하고, 나머지 활성 설명/핀/경로 변경을 역복원해 기준13599/73EC... JSON bytes와 일치하도록 한다. 새 binding/preparation도 필드별 차집합/기준원문을 보존하여 역복구한다. 실제 기존파일 수정/삭제로 복구하지 않는다. baseline/failure·frame-body·스캐너4후보와 기존 정적 수용은 변경0이다.

## 5. 새 정적 예산·실제 관문·종료

REQ-M1P0R3N-005. 후속 Approved 뒤 제안할 새 정적 예산은 Sol 최초 판독1묶음·새3파일 각작성1회·동일 범위 보정/재읽기전체최대1회, Luna 전체 독립 정적 판독1회·같은범위 재읽기최대1회다. 기존8후보/부분별 소진 예산을 복원하거나 파일 수로 곱하지 않는다. 정상구문의 정확 일치·기준/의도실패 핀 보존·진단types빈배열·반환형 미관측·활성핀 전파/역복구·host수량을 한 묶음으로 검수한다. 동적 구문/사례/프로세스 시험은 포함0이다.

후속 순서는 Luna 설계 검수→Astra 정적 Approved→새 정적 후보/Luna 독립 판독/Astra 한정 수용→실제 Node 수신/판정의 엄격 프로토콜·사례별 새session/nonce/직접최상위 상태·실제출처/미소진·호스트/계약/코드핀·argv/승인모델B/Lmodel·원시/EOF/close·첫 예상밖 실패 중단을 정확 결속→별도 실제 host3 권위다. baseline의 정상/오염 관측과 의도실패를 예상밖 전송실패와 구분하는 실제 판정은 아직 미완성이다. 타입 관측host추가·스캐너·큰 원장 읽기·결과저장은 자동 승인0이다.

프레임stdout4096/stderr2048/모델영수증2048 및 raw1024/receipt1024/모든meta512, 기존QPC60/65/70·Node80/85/90·kill0/재시도0을 유지한다. stderr 동기쓰기·진단cap 초과가 주catch 밖에 있는 기존 한계는 이 구문교정으로 해소됐다고 주장하지 않는다. 그 한계가 실제 호출/원문 회수 승인과 충돌하면 별도 정확 수신/폐쇄 계약에서 해결하기 전 실행0이다. 결과경로 부재/별도Root 저장권위·원장목적지 관문도 그대로 남긴다.

기준/발생수 불일치·범위 밖 코드/수량 변화·예산 소진·출처/실제입력/B/Lmodel/권위/원문/폐쇄 미충족이면 기존 P1/실패를 보존하고 중단한다. 이번 종료는 새 Draft 경로·UTF8 bytes/SHA와 요구/수용 식별자·남은 관문 반환이며 문서외 후보수정/실행/호스트/스캐너/원장/결과쓰기0이다.

## 6. 수용 기준

AC-M1P0R3N-001: 신규P1·기존8후보/정적검수/원문·본편 목표와SourceReady=false·스캐너 영향0을 확인한다.
AC-M1P0R3N-002: normal17행1곳의 정확 $null = 체인과 세 part 실행·types빈배열·baseline/failure/본문 불변을 확인한다.
AC-M1P0R3N-003: 미억제stdout/정확정상보정/별도반환형계측의 억제효과를 분리하고 기본host3·추가host미승인/자동확대0을 확인한다.
AC-M1P0R3N-004: 신규3경로·기준핀·활성참조/새산술·원문보존/정확역복구·지문재귀0을 확인한다.
AC-M1P0R3N-005: 새 유한정적/독립검수·실제수신/권위/저장 미충족·기존시간/출력/폐쇄한계·이번문서외 실행0을 확인한다.

## 7. 정상 구문 교정의 R3 호환·세 후보 정적 준비 승인 — Approved

2026-10-16 아스트라는 이 Draft 전체를 직접 판독하고 Root가 전달한 루나 독립 설계 판정 P0/P1=0 및 REQ/AC-M1P0R3N-001~005 부합을 근거로 정확 정상 구문 교정 설계와 새 세 후보의 정적 작성·유한 검수만 Approved로 확정한다. 이 기록 앞의11424바이트/SHA256 3C41594F05ADCD85C79DB1E1CE26A31481AE14191815546430224E0BF0031EF5를 정확 접두사로 보존한다. 명시 배정 모델은 gpt-6-astra이며 독립 런타임 조회를 주장하지 않는다. 앞선 Draft·작성0·종료 문장은 당시 이력이며 이번 정적 허용에는 이 기록이 우선한다.

R3 §3의 정확 정상 보정 구문 실행 요구는 이 교정으로 유지한다. 기존 세 사례 안에서 실제 반환형까지 관측하겠다는 설계 목표는 이번 호환 범위에서 별도 미실행 관측으로 분리한다. 기본host3은 미억제stdout·정확정상3구간·의도실패를 위한 제안 상한으로 유지하고 반환형 미입증을 명시한다. 이는 반환형을 관측했다고 간주하거나 R3 동결 원문을 수정하는 결정이 아니다. 반환형 관측을 추가하려면 정확 코드·수량3→4·별도 권위의 호환 승인이 필요하며 이번에는 그 코드작성/추가host 승인0이다.

허용 경로는 §4의 m1-phase0-r3-frame-probe-normal-correction1-design.tmp, m1-phase0-r3-frame-binding-normal-correction1-design.tmp, m1-phase0-r3-recovery-preparation-normal-correction1-design.tmp 세 개뿐이다. Sol 단일 쓰기 소유이며 Root의 실제 신규 경로 부재·공유 소유·기준핀 확인 뒤 작성한다. normal 사례의 기존17행 한 분기만 frame-body와 문자 단위로 동일한 $null = 완료체인으로 바꾸고 codePin·정상 진단types빈배열·기대설명 및 관련 활성 참조핀을 전파한다. baseline/failure codeText와 핀·엄격 parserCodeText·frame-body·스캐너 후보는 불변이다. 실행되지 않는 다른 사례의 normal 분기까지 치환하지 않는다.

새 정상 구문은 루프의meta/raw/receipt 세 part에 적용한다. 정상 증거는 정확 길이/SHA/추가bytes0·작은 JSON/UTF8·firstError없음·types빈배열·EOF/exit/close 범위이며 반환형 증명이 아니다. 정상 프로브의 진단stderr와 실제 단계0 stderr0 경계는 구분한다. 기존stderr 동기쓰기·주catch 밖 상한 오류의 한계를 해결했다고 표시하지 않으며 실제 수신/폐쇄 권위와 충돌하면 별도 정확 계약 전 실행0이다.

정적 예산은 Sol 최초판독1묶음·새3파일 각작성1회·동일 범위 보정/재읽기전체최대1회, Luna 전체 독립 판독1회·같은범위 재읽기최대1회다. 과거 예산을 복원하거나 파일 수로 곱하지 않는다. 정상 분기 역치환의3081/79648642919925E629FA492F7CD4D8F1098BA67F718D406F72928461DB5C6484 일치, 활성 필드별 역복구·기준 JSON 원문·정확 지문을 확인한다. 새 명령행 산술은 후보로만 기록하고 기존B 자동 승계0이다. 실제 기존파일 수정/삭제로 복구하지 않는다.

기존8후보·정적 검수/수용·이번P1 원문과 실패 소진을 보존한다. 이번 승인은 P1의 정적 교정 준비이며 실제 새 후보 독립 검수 전 P1 해소를 선언하지 않는다. 동적 평가/시험·호스트/스캐너/Unity·원장/결과파일쓰기·실제 실행 및 저장 권위 발급0이다. SourceReady=false와 실제Node본문·출처·미소진·새신원·B/Lmodel·원문/폐쇄·별도 저장 관문을 유지한다.

세 후보의 지문·정확 차집합/역복구·사용 예산·미충족을 Root에 반환하고 독립 정적 검수와 한정 수용을 구분한다. 승인 담당은 이 문서 끝 기록1회만 추가했다. 접두사 및 최종 지문 확인 뒤 문서 소유를 Root에 반환하며 후속 실제 승인/호출문 결속은 외부에서 비재귀로 수행한다.
