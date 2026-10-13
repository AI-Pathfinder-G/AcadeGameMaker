# Q0 R4 고정 입력 캡처 경계의 한정 복구

상태: **Approved** — 2026-10-03 실제 gpt-6-astra의 키없는 준비·구현 한정 승인. 실제 gpt-6-luna의 독립검토 및 종료 관측 주체 보정 확인을 완료했다. 새 운영키·운영큐·원격 게시·실제 아홉 회 실행은 최종 인간 승인과 별도 관문 전 금지한다. REQ-PLAT-061/062/068/069/070, REQ-M5D7QC4-007 및 AC-PLAT-059/060/066/067/068, AC-M5D7QC4-009/010, AC-M5D7QC3-007/008을 잇는다.

## 역사와 불변 경계

기본4F9 `4F9C58EA0CFDA8318C7A2D6FB345C5B9484E9B9E07458DE898993DB8CCE32925`, 제어19DC `19DCFF8E4B323B9E7BAAEC61264541029DA758CDADA788B3FCE9F28514538364`, R2보완85A4 `85A49EAD8E7B8CD3DBE0D97E703663B03944ECC6921532108ABB8046075A1BAB`, R3보완A440 `A44000B10810412130DAFE990AC689A411BD146524AA3F1BAEA6AC727FEBBE03`의 의미와 기한을 유지한다. 공개권위·서명 기본 ContractSha256는4F9, 시작10은19DC다. core 원문 `0F6937BD52732C49E979C8F8A3F897C81733DD21028BE6EEB194AC0B381D012A`를 변경하지 않는다.

R3는 새 키1·서명1·발급기 종료0·공개문서2 PR9 병합 `d2f49e4a1ed5843ae803d23c328d39f00e51d01f` 후 실제 Authorize1·소비1을 수행했다. 소비 원문 SHA `B81FCFCCDC4CDFB6F24F6763E8B1A1D9C0F5E6DA3668490EDE111A327A6E91AC`, 큐 종료 원문 SHA `051B4598AE859742A0B51F2E88822C65859A998CADF0E5C87F544E8EA2C8BA41`에 결속한다. 첫 Before 캡처가 승인 입력의 접미사를 산출물로 오인해 RUN_REJECTED를 출력했다. 큐 자식41356의 원시 종료 영수증은 native2·배수완료·정리false다. 별도 상위 소유자 도구 세션83020의 종료1은 Root 도구 관측이며 큐 자식 종료값과 구별한다. Unity 운영 실행과 아홉 출력은0이며 전체 수용은 아니다. 키 개수는 흐름의 관측 범위이며 별도 암호 저장소 전수 조사를 주장하지 않는다.

R1/R2/R3 원문·소비·승인·원장·실패·준비 및 실행 디렉터리는 불변이다. R3 운영 전체 handoff 지문 `52CD9D7E8246FE6568146E086DAD48A7C1C01CEC1A73B7DCEF8559C98F6FFDC5`는 종료된 세대의 역사로 고정한다. 운영 종료 이후 기존 handoff에 실패·R4 설계·수용을 추가할 수 있지만 새 전체 지문은 R3 재사용을 허용하지 않는다. 기존 승인·서명·nonce·세대·큐·기한을 재사용하거나 연장하지 않는다.

## 준비 전이와 유일한 의미 변경

새 고정 디렉터리는 output/q0-trusted-child-prep-r4, output/q0-trusted-child-run-r4, output/q0-trusted-child-evidence-r4, output/q0-trusted-child-luna-r4다. R3 수용 준비570 원문을 전임으로 복사하고 길이·SHA·링크·메타257를 전수 대조한다. R3 최종 공개검증기 대신 R3 비공개 준비의 UNISSUED 원문을 전임으로 사용한다.

변경 허용 상대경로는 R3의 여섯 경로와 캡처 한 경로, 정확 일곱 개다.

- qa/tools/Test-Q0FixedExecutionAuthority.ps1
- artifacts/c4-canonical-execution-plan.schema.json
- artifacts/c4-canonical-trusted-child-lease.psm1
- artifacts/c4-canonical-final-validation-queue.ps1
- artifacts/c4-canonical-trusted-child-selftest.ps1
- Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs
- artifacts/c4-canonical-capture-inputs.ps1

앞 여섯 파일의 변경은 R4 경로·공개권위·세대 증거 결속뿐이다. 일곱 번째 파일만 다음 의미 보정을 허용한다. 현재 정규식에서 .git 디렉터리와 output 디렉터리는 무조건 거절한다. `-before/-after/-native/-qa/-rows/-verification.json` 접미 거절에는 정확 상대경로 `artifacts/c3-r11-required-decision-expected-rows.json`이며 서명된 source map SHA가 `D1B9CBDA4AF477B5C10FC14893F0C58B2BAE676A38E15987C87B7D918658C9C6`인 경우만 예외를 둔다. 대소문자까지 정확해야 하며 동적 허용 목록이나 일반 Role 기반 예외를 추가하지 않는다. 이후 canonical 경로·루트 내부·링크 금지·실제 파일 존재·원시 바이트 SHA 일치 검사를 모두 그대로 수행한다. 기대행 파일 자체는 바꾸지 않는다.

최대7 변경/나머지563 동일, 메타257 불변이며 실제 전이 행을 기록한다. Source570/Input565/추적571/SourcePaths25/역사const16·원문25/lease입력0·source1/53필드를 유지한다. Stem q0-r1-*와 출력 basename 및 선택 의미·총1654는 불변이다. 최종570에서는 준비 대비 검증기 공개4 자리만 달라질 수 있으며 역치환 지문을 검사한다.

## 소비 전에 필요한 실제 캡처 본문 검사

키없는 준비에서 생산 Get-C4CurrentInputCapture 본문을 직접 사용해 Before/After 각각570의 양성을 검증한다. Assert-C4PostprocessHost만 명시한 가상 활성 host/context로 대체할 수 있고, 파일 읽기·source map·경로·링크·실제SHA 검사는 실제로 수행한다. 모의 host는 운영 세대·서명·브로커·실제 소비를 입증하지 않는다. 원래 R3 파일은 변경하지 않으며 변조 음성은 독립 fixture 복사본에서 수행한다.

필수 음성은 예외 경로의 유사 이름·대소문자 차이, 같은 경로의 선언SHA 변조와 실제바이트 변조, .git/output, 나머지 접미 산출물, 중복·누락·입력/source 불일치 및 링크다. 링크 생성이 불가능하면 미검증으로 기록하고 전체 통과로 집계하지 않는다. 정확 경로와 기준SHA를 제외한 새 허용은 없어야 한다.

추가로 생산 큐의 첫 Before 호출 순서와 실제 capture 함수의 연결을 키없는 fixture에서 검증한다. 가상 활성 host와 Unity 실행 대신 호출 도달 표시만 모의하며, 유효570에서는 capture 반환 뒤에만 실행 표시로 도달하고 음성에서는 그 전에 닫힘을 확인한다. 운영 코드의 소비 순서는 바꾸지 않는다. 실제 운영은 소비 후 완전한 실제 host 검증과 Before/After570을 계속 수행한다.

새 공개값을 고정한 최종 clone 이후에도 실제 최종 plan/source570으로 동일 capture 본문 Before/After를 키없이 확인해 서명 및 Authorize 이전 검수에 포함한다. 추가 capture 증거는 허용 fixture에 두고 고정 계획의53필드에 새 필드를 만들지 않는다. fixture는 실제 출력 경로에 Before/After 영수증을 쓰거나 소비하지 않는다.

## 증거 순서와 독립 수용

R4 parser/schema53·배열/원시음성·Git/input/launch/합성public 연결, 채널71 및 추가된 캡처 양성/음성을 새 소스로 수집한다. C# 권위 경로가 바뀌므로 Unity6000.6.0f1 키없는 지원1회가 필요하다. 지원4벡터·보호501·C#4/도구9 전후·native pipe 신원·원시로그와 XML 한계는 R3와 같다. 실제 시험9회 성공으로 확대하지 않는다. 자동 재수집은 없고 실패 보존 뒤 정확 소스별 Astra 판정이 필요하다.

준비4는 preparation/channel-tests/unity-compile/schema-regression이며, 캡처 본문과 큐 첫 Before 연결 검사는 schema-regression의 별도 제한 범위로 결속한다. 준비4는 outside issuer/control·미래제어·진행자를 직접 또는 간접 참조하지 않는다. Root CreateNew → Luna 정확12 Q0R4PreparationLunaReviewJson → Astra 정확13 Q0R4PreparationAcceptanceJson 순서다. R3 구조를 유지하고 R4 schema/계약/실제path·SHA와 source570/P0/P1/OperationalKeyCreatedfalse를 기록한다.

outside 신규2는 qa/tools/Invoke-Q0TrustedChildIssuerR4.ps1 및 qa/tools/Test-Q0TrustedChildIssuerR4.ps1이다. R3 issuer70EDD31B6CDFB80F4DA9C3E572496040434D03E084FD9D80C08B6C743527B681/testFB73A4D718F4F03E7AE8A1C37DA46AA6C3AEB6D1D9EA97890B3E9C47A82251A7를 전임 원문으로 보존한다. 준비13 뒤 전용1MiB prep파서·준비/제어 핀·R4 경로를 고정한다. wire16KiB·정확타입·기한·일회서명 의미는 불변이다. 제어78 기존사례를 유지하고 R4 계약 인증 음성을 추가하여 최소79사례와 생산 Bind/Sign 모의 경계·호출횟수를 검수한다. 같은 고정 merge의 인증조회는 아래 다섯 원문이다. 최종 제어증거 Root 발급 → Luna 정확10 IssuerControlLunaReviewR4Json 순서다.

R3 소유 수집자·비협력 취소 실패·TTY·최초30초 실증은 원래 소스와 당시 모의 경계를 보존한 역사다. R4 도구는 명시된 경로·계약·준비 핀·최종참조 전이만 허용하며 실제 전후 rawSHA·지정 치환 재현·relay 핵심 본문 불변을 Luna가 확인한다. 의미 변경이 있으면 관련 실제 키없는 경계를 새로 검증한다. 단순 경로 전이도 최종 도구의 고정 entry 인수 및 키없는 실제 모의 정상 생성·종료를 최소1회 확인한다. 새 세대의 운영 수집자 원시 생성/응답/종료는 반드시 새로 수집한다. R3 역사 합본을 R4 실행 성공으로 재명명하지 않는다.

## 원격과 인간 최종 관문

원격 가지 codex/q0-execution-public-authority-r4, 저장소ID1344230225, main 대상으로 정확 두 새 문서만 게시한다: docs/approvals/2026-10-03-q0-execution-public-authority-r4.json 및 본 계약 docs/specs/work-contracts/2026-10-03-q0-r4-capture-boundary-recovery.md. 같은 고정 병합에서 기본4F9/R2 85A4/R3 A440/R4최종계약/새권위 원문SHA 다섯 개를 인증 대조한다. 로컬 고정 evidence-r4/authority-public.json 원문과 인증 원격 바이트SHA도 일치해야 한다. R3 공개권위 문서를 R4 권위로 재사용하지 않는다. 공개권위12필드·기본4F9·새키/epoch/최종verifier 결속, 정확변경2·head/base·부모2·앱첨부·병합 전 재대조를 유지한다.

Approved는 키없는 구현·준비만 승인한다. 독립검토를 반영해 Approved 최종 원문을 고정한 뒤 실제 SHA를 handoff에 기록하고 모든 종속 핀을 결속한다. 자기 SHA를 본문에 넣지 않는다. 모든 준비·제어·최종 진행자 검수를 완료한 뒤 인간에게 추가 세대 정확1개, 새키1·키종속 공개문서2의 게시/병합·동일세대9회1654를 묶어 한 번 명시 승인받는다. 이전 인간020ACF는 R3 한 세대만 승인했고 이미 소비되었으므로 R4 키 승인으로 해석하지 않는다. 원격 코드 게시와 추가 세대는 제외한다.

실제 질문/응답/UTC관측의 인간8·범위7은 R3 구조를 R4로 전환하고 새 실승인 뒤에만 기록한다. 실제 Astra 의미 확인 후 IssuerControlStartAcceptanceR4Json 정확10을 발급한다. 준비13·제어Luna10·human8·최종전체handoff·core 및 실제source570의 키전 검사를 수행한다. 그 후 키1→공개/clone·원장5→공개두문서 대조·원격병합→Bind1→소유큐Ready→실제plan·최종capture fixture→Astra Sign1→서명후Luna→Astra Authorize1 순서다. 발급/Ready45분·전송30초·배수5초·정확세대 정리 한도는 바꾸지 않는다. 실패·소비·만료를 재발급 또는 재전송의 자동 권한으로 취급하지 않는다.
