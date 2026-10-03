# Q0 R3 만료 세대의 한정 복구

상태: **Approved** — 2026-10-03 실제 gpt-6-astra의 키없는 준비·구현 한정 승인. 실제 gpt-6-luna의 독립 검토와 지적 보정 확인을 완료했다. 새 운영키·공개문서 게시·운영 실행은 본문의 인간 최종 승인 및 별도 관문 전 금지한다. REQ-PLAT-061/062/068/069/070, AC-PLAT-059/060/066/067/068 및 REQ-M5D7QC4-007, AC-M5D7QC4-009/010, AC-M5D7QC3-007/008을 잇는다.

## 승인 범위와 선행 역사

기본 계약4F9 `4F9C58EA0CFDA8318C7A2D6FB345C5B9484E9B9E07458DE898993DB8CCE32925`, 제어19DC `19DCFF8E4B323B9E7BAAEC61264541029DA758CDADA788B3FCE9F28514538364`, R2 보완85A4 `85A49EAD8E7B8CD3DBE0D97E703663B03944ECC6921532108ABB8046075A1BAB`의 보안·출처·시험 의미를 유지한다. 본 문서는 그 제한을 완화하지 않는 다음 세대 경로·출처 보완이다. 서명 payload와 공개권위의 기본 ContractSha256는4F9, 정확10 제어 시작 앵커의 ContractSha256는19DC를 유지한다.

R2는 실제 키 한 개·Sign 한 번·발급기 종료 후, 사용자 응답 전 큐의 45분 단조 기한이 만료되어 native2로 종료했다. 기존 종료 원문 SHA `CF53146D57D007D938D77513635853334EEE48F9FF8184EBB262B93CD081A1CA`와 계획A46A·승인CEC716·PR8 병합428a7fd710123dad19775e904108440afab1046e를 불변 보존한다. 소비·실제 시험 출력은 없었다. 사용자 “승인한다”는 고정 계획 한 번과 로컬 아홉 회·1654시험에 대한 의향이며 기존 기한을 소급 연장하지 않는다. 새 운영키는 이전 승인 질문에서 제외되어 있었으므로 아래 인간 최종 승인 전 생성 금지다.

## 준비 경로와 허용 차이

새 경로는 output/q0-trusted-child-prep-r3, output/q0-trusted-child-run-r3, output/q0-trusted-child-evidence-r3, output/q0-trusted-child-luna-r3다. 원래 Git/R1/R2의 모든 자료·원장은 수정하지 않는다. R2 수용 준비570을 원시 복사하되 정확 집합·길이/SHA·메타/GUID·링크·원문을 전수 대조한다. R2→R3 전임 행을 별도로 남기며 역사 R1 자료를 R3 것으로 치환하지 않는다. 변경 허용 상대경로는 R2의 동일6개다.

- qa/tools/Test-Q0FixedExecutionAuthority.ps1
- artifacts/c4-canonical-execution-plan.schema.json
- artifacts/c4-canonical-trusted-child-lease.psm1
- artifacts/c4-canonical-final-validation-queue.ps1
- artifacts/c4-canonical-trusted-child-selftest.ps1
- Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs

변경은 현재 세대의 고정 디렉터리·새 공개권위 경로·필요한 R3 증거 결속만 허용한다. 배열 보정·schema 필드53·유효성·정수·기한·서명 검증 의미는 불변이다. 최대6 변경/나머지564 동일이며 실제 변경 수를 기록한다. C# 나머지3 및 선택 의미·1654 실행 합계는 그대로다. Stem q0-r1-*와 출력 basename은 유지한다. 준비 공개4는 UNISSUED, 새 발급 후 정확4 자리 치환/역치환 SHA 검사를 유지한다. 최종570 중 검증기1만 준비에서 달라질 수 있다. Source570/Input565/추적571/SourcePaths25·역사const16·원문25·lease입력0·sourcelease1을 유지한다.

570 밖 신규 제어 두 파일은 qa/tools/Invoke-Q0TrustedChildIssuerR3.ps1 및 qa/tools/Test-Q0TrustedChildIssuerR3.ps1이다. 수용된 R2 발급기373466·시험22B22를 원시 전임으로 기록하고 고정 R3 경로·계약·준비·앵커만 바꾼다. 불변 core SHA0F6937BD52732C49E979C8F8A3F897C81733DD21028BE6EEB194AC0B381D012A를 직접 제한 호출한다. 임의 root/권위/계약 인자를 만들지 않는다. 준비파일 전용1MiB strict parser와 정확 원문SHA 핀은 새 수용 준비에 결속하고 wire16KiB는 유지한다. 이미 해결된 native held handle 관측·한정 exact child 정리·실제 생산 모의 호출횟수 검사를 유지한다.

## 키없는 검증과 순환 없는 수용

예약 파일명과 아홉 Stem별8출력은 R2와 동일한 이름으로 R3 증거 루트에만 CreateNew한다. 추가 예약 schema-regression.json, issuer-control-keyless.json, unity-compile-attempt-01.log/xml/json을 유지한다. 실패는 원시 보존하며 자동 다음 수집은 금지하고 실제 원인과 정확 수정 지문을 Astra가 판정한다. 후보는 새 준비/루나 selftest-repositories에만 둔다. 새 보고 대신 기존 handoff에 R3 접두사로 기록한다.

R3 실제 parser→유효53 schema·배열경계·타입/필드 음성→생산 Sign 전처리의 실제 import/Git/input/launch/public 연결을 검증한다. 모의 Bound/commit/preparation/Ready/owner/time/암호중단 경계를 명시하고 모의를 실제 권위로 주장하지 않는다. 채널71 사례와 기존 제어74 사례를 보존하고 새 R3 계약 인증 음성·양성을 추가한 최종 제어시험의 현재 소스 실행·원시·native 소유/종료/배수 증거를 각각 새로 수집한다. 읽기 조사에서 C#의 공개권위 고정경로 한 곳이 R3로 바뀌어야 함을 확인했다. 따라서 변경된 최종 C# 바이트로 같은 Unity6000.6.0f1 지원 메서드를 새로 한 번 검증하는 것이 필수다. R2 지원 증거는 비교용 역사일 뿐 R3 성공으로 재사용하지 않는다.

준비4 증거 preparation/channel-tests/unity-compile/schema-regression을 먼저 고정한다. Q0R3PreparationLunaReviewJson 정확12 및 Q0R3PreparationAcceptanceJson 정확13은 R2 구조를 그대로 사용하되 SchemaVersion의 R3와 본 계약SHA, 새 경로·실제 증거SHA를 결속한다. SourceCount570·P0/P1=0·OperationalKeyCreatedfalse 및 제한 준비 Decision을 유지한다. Acceptance의 IndependentReview는 루나 raw JSON UTF8 SHA다. 준비 증거는 제어 소스나 이후 제어시험 결과를 참조하지 않는다.

준비 수용13 rawSHA를 R3 발급기 고정 핀에 넣은 뒤 최종 제어 소스2/키없는 시험을 수집하고 Root가 최종 제어 증거를 CreateNew한다. Luna가 전수 검수한 IssuerControlLunaReviewR3Json 정확10과 Astra의 IssuerControlStartAcceptanceR3Json 정확10은 기존19DC 구조·SingleNewGenerationUnder19DC를 유지하며 R3 소스2/증거/준비13/독립검수 rawSHA에 결속한다. R1/R2 앵커를 대안으로 수용하지 않는다. Int64 원문·엄격 파서·중복키 거절·전체 handoffSHA 핀을 유지한다.

## 인간 최종 승인과 시간 순서

본 계약의 Approved 상태는 키없는 구현·검증·준비만 허용한다. Draft→Approved 문구 및 독립검토 지적을 모두 확정한 뒤 실제 최종 원문SHA를 계산하고 기존 handoff에 고정한다. 본 계약은 자기 원문SHA를 본문에 포함하지 않는다. 구현의 계약 상수·키없는 후보·준비13·제어10·원격 인증 검사는 이 최종 지문에만 결속한다. 이후 계약 수정이 필요하면 승인과 모든 종속 핀 검토를 먼저 다시 하며 낡은 지문으로 실행하지 않는다. R3 자료와 진행 도구를 모두 고정하고 Luna의 최종 독립검수를 마친 뒤, Root는 인간에게 정확한 새 단명 운영키 한 개와 해당 키의 공개권위 두 문서 게시/병합, 고정 같은 아홉 회·1654 로컬시험을 한 세대로 수행할 범위를 명시한다. 이 명시 승인 한 번은 새 키 한 개 생성과 그 키로 완성되는 정확 두 공개 문서의 게시·병합까지 포괄해야 한다. 키에 종속되는 modulus/epoch/최종 verifierSHA는 승인 요청 때 아직 생성되지 않았음을 설명하고, 승인 전에는 고정 공개문서 구조·계약 최종SHA·생성/검사 절차·원격 두 경로를 구체적으로 제시한다. 키 생성 뒤에는 완성된 두 원문을 실제 Astra가 그 승인된 구조·범위와 대조하고 고정해야 하며 범위가 달라지면 게시하지 않는다. 원격 코드 게시와 추가 세대는 제외한다. 인간의 명시 승인이 실제 기록되기 전 운영 Boot·새키·운영큐를 시작하지 않는다. 기존 시험 승인을 새키 승인으로 추정하지 않는다.

인간 승인 원문·시간·범위를 기존 handoff에 기록한 뒤 실제 Astra가 최종 시작10을 발급한다. 아직 존재하지 않는 결과나 승인으로 앵커를 미리 만들지 않는다. 시작10과 최종 전체handoffSHA를 사용한 실제 파일·준비·570 전수 사전검사를 키없이 수행하고, 추출함수 출처환경 치환 및 native entry 모의는 분명히 구별한다. 원시 소유 발급기 수집 경로도 키 전에 검토하여 실제 stdout/stderr/전체ticks/종료를 보존한다. 기존 45분 발급 및 Ready45분, 전송30초, 출력상한·배수5초·외부기한은 변경하지 않는다.

키1→공개4동결→최종clone/Git/원장5→원격정확2 병합·인증→Bind1→별도 소유큐Ready→실제 unsigned53 계획 Astra 검토→Sign1/키Close→Luna 서명후독립검수→Astra Authorize 한 번 순서를 유지한다. 인간 승인·키없는 준비·순수 진행자 검토는 반드시 키 전에 끝내어 운영 기한 안에 인간 응답을 기다리는 구조를 반복하지 않는다. 추가 사용자 결정이 필요한 범위가 생기거나 시간이 부족하면 안전하게 실패·폐기하며 기한 연장·서명 재사용·자동재키를 하지 않는다.

## 전용 발급기 원시 수집자의 소유 경계

19DC의 전용 TTY 소유 규칙을 R3에서 다음처럼 구체화한다. Root의 정상 현재사용자 tty=true 도구 세션은 고정 발급기 소유 수집자 pwsh 한 개를 유지하고, 수집자는 고정 pwsh -NoProfile -File qa/tools/Invoke-Q0TrustedChildIssuerR3.ps1 -TrustedIssuer 자식 한 개를 직접 생성 반환 핸들로 소유한다. 발급기 자식의 표준입출력은 전용 파이프로 수집자에만 연결하고 별도 공개 파이프·서버·다중 호출자·임의 실행 인수는 만들지 않는다. 이는 발급기 자식이 직접 TTY에 연결되었다는 주장이 아니다. 키와 인증·서명 효과는 수용된 발급기 자식에만 있으며 수집자는 키·토큰·핵심 세션에 접근하지 않는다. 장수명 실행 큐는 별도 소유자 세션에서 생성하고 발급기나 그 수집자의 자식으로 만들지 않는다.

수집자는 현재 전체handoffSHA·실제 시작10·고정 발급기/core·인간 승인 범위를 확인하고 완성된 Boot 원문을 자식 생성 전에 준비한다. 생성 후 정확 PID·보유핸들·native 전체ticks·실행 경로·부모·고정 실제 인수를 결속하고 즉시 Boot를 한 번 전달한다. 첫 입력30초 및 전체45분은 발급기 자신의 기존 시계로 유지하며 수집자가 늦으면 실패한다. 초기 전송을 위해 불완전 Boot나 임의 승인값을 보내지 않는다.

수집자는 기존 검토된 비동기 중계·출력상한(stdout1MiB/한줄16KiB/32줄, stderr64KiB)·동일 전송30초·종료후배수5초와 유한 외부기한을 유지한다. TTY 장치 에코·수집자 메타정보·자식 원시 stdout을 구별한다. Public/Bound/Signed payload를 strict parser로 정확 형태·순서·현재 계획/세대에 결속하여 허용 fixture에 CreateNew하고, 줄끝 제외 payload 범위와 완전 stdout/stderr 원시바이트/길이/SHA를 별도로 보존한다. Int64는 원문으로 유지한다. 예상밖 응답·중복·쓰기실패·전송손실·배수미완료를 성공으로 취급하거나 재전송하지 않는다.

수집자 EOF·사망·relay오류·기한 실패는 같은 발급기 세대의 실패 경계다. 살아 있는 수집자는 입력닫힘과 정확 소유자식 정리를 수행하고 멈춘 자식만 보유핸들/PID/native ticks/경로 재확인 후 Kill(false)한다. 비정상 사망한 수집자가 정리하거나 영수증을 썼다고 주장하지 않는다. Boot 전달 전 생성 관측 파일을 CreateNew/Flush(true)로 보존하고, 수집자 자신의 PID/native 전체ticks/exe/실제argv/부모 및 자식 PID/native 전체ticks/exe/고정argv/실제부모/생성반환핸들의 역사값을 결속한다. 그 파일을 끝내기 전 Boot를 전송하지 않아 생성관측 미완성 사망에는 키가 없도록 한다. 수집자만 자식 stdin 쓰기 끝을 보유하고 외부에 복제하지 않으며 수집자 종료의 EOF를 발급기 자신이 실패·종료로 처리하는 경계를 실제 키없는 자식으로 확인한다. 수집자 바깥의 살아 있는 Root가 TTY 도구 종료를 감시하고, 비정상 종료 통보 후 고정 생성 영수증을 읽어 자식의 새 읽기 핸들을 확보한다. 이 핸들은 사후 읽기 핸들이며 생성반환핸들이 아니다. Root는 보유 읽기 핸들의 PID/native ticks/exe와 실제부모/고정argv를 원래 영수증에 대조한 동일 세대만 관측하고 EOF 후5초 안에 종료하지 않으면 그 확인된 프로세스에만 Kill(false) 후5초 종료를 확인한다. 이미 종료되었거나 PID가 재사용되면 새 프로세스를 중단하지 않는다. 출처·신원 확인 실패나 종료미확인 시 안전 중단·불확정으로 보고하고 다른 프로세스 중단으로 보완하지 않는다. 수집자 사망 후 완전한 raw 스트림을 복원할 수 없으면 보존된 부분과 관측 한계를 명시하며 새 성공영수증을 조작하지 않는다. Root의 사후 도구/native 관측·원래생성 영수증SHA·정리 결과는 허용 fixture의 별도 CreateNew 실패 관측에 보존하고 기존 handoff에 결속한다. Root까지 종료되어 관측하지 못한 기간의 즉시 정리나 도구호스트/미검증 Job의 자동종료를 추정하지 않는다. 광역·재귀·PID단독 중단은 금지한다. 수집자는 실제 종료와 원시 최종영수증을 보존하며 관리 Dispose 성공은 관측없이 단정하지 않는다. 구현 초안은 R3 luna selftest-repositories/root-construction-reference에만 둔다. 운영 전 고정 소스의 실제 키없는 자식으로 정상·초기30초·EOF·수집자사망·장치에코분리·부분/과대출력·막힌전송·응답후즉시종료·배수실패 경계를 검증하고 Luna가 독립 검수한다. 키없는 시험이 실제 키생성/SignHash/원격으로 가지 않음을 증명한다. 최종 수집자 소스SHA·시험증거·실제 소유구조는 기존 handoff에서 Astra가 수용하고 인간 새 세대 승인 전에 고정한다.
## 공개권위와 최종 실행

가지 codex/q0-execution-public-authority-r3, 저장소ID1344230225, main 대상. 변경은 정확 새 문서2: docs/approvals/2026-10-03-q0-execution-public-authority-r3.json 및 본 문서 docs/specs/work-contracts/2026-10-03-q0-r3-expired-generation-recovery.md. 기본4F9와 기존R2보완85A4는 변경하지 않고 같은 고정 병합에서 원문 인증조회한다. R3 발급기는 기본4F9·선행85A4·본 계약SHA·새 권위 rawSHA를 모두 인증대조한 뒤 Bind1을 허용한다. Bind 전에 고정 로컬 output/q0-trusted-child-evidence-r3/authority-public.json의 CreateNew 원문SHA도 인증된 원격 권위 bytes SHA와 정확 대조한다. 원래 workspace/docs에 파일을 쓰거나 caller가 준 임의 SHA로 대체하지 않는다. local 누락·변조·동일 공개값이나 다른 원시 원격 bytes를 거절하는 키없는 음성검사를 추가한다. 새 공개권위는 기존12필드·기본4F9·새키/epoch/검증기SHA를 결속한다. 정확변경PR/head·base 고정 병합·부모2·인증GET을 유지한다.

서명후 Astra Authorize는 사용자에게 승인받은 동일 의미의 아홉 회·1654 실행과 해당 새 계획SHA/nonce/InvocationId/소유큐에 한정한다. 승인 소비는 한 번이고 실제 소비·중간실행·최종9회 출력·원장/188·377행·보호원문·Git/종료를 독립 확인한다. 사용자 승인 사실은 도구 안전심사에도 정확 전달하며 거절을 우회하지 않는다. 전체 역사 P1/전체 게임·게시 수용은 별도이며 WholeAccepted 및 PublicationApproved를 선취하지 않는다.
