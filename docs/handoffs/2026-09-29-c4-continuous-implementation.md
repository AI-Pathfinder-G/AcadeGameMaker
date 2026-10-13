# C4 연속 후속 작업

- 사용자 지시: 다음 작업을 진행하며 실제 사용자 결정 사항이 생길 때까지 계속한다.
- 기술 승인·배분·통합: 아스트라, 실제 `gpt-6-astra`.
- 근거: ADR-0036/0037, Approved-only 구현과 REQ/AC 추적을 유지한다.

C3 R11의 실행 직전 선행 수용은 `docs/approvals/2026-09-29-c3-r11-pre-c4-integration-acceptance.md` SHA `92343D3C5AE287B1D4F6B939CFDECD1B6424E6E11B9D29CB6AD4C19F64808F89`다. C3 전체와 AC-M5D7QC3-007/008의 공동 최종 수용은 아직 미완료다. 과거 실행 증거를 변경된 C4 소스의 실행 증거로 재사용하지 않는다.

현재 C4 통합 r3 초안 SHA `FB1BCEB6DC20CA4E5122CB3756A559EC6FEB57858B8F6A6A1E587A85A254F4D1`의 독립 검수에서 P0=0/P1=1을 확인했다. `docs/verification/2026-09-29-c4-r3-consolidated-contract-luna-review.md` SHA `450B52AA9CE7332EA8CA480E22B8B4933FB99BAC9CAF07DEFF208ED4A9B54849`의 실제 미결은 C1 Busy/Stale 뒤 동일 소유자의 새 요청 상태·세대·이력 보존 전환 규칙이다. 솔이 원본 초안과 검수 이력을 보존하고 새 r4 규범 초안을 작성한다. 이 기술 보완에는 사용자 결정이 필요하지 않다.

검증 도구는 기존 QA 도구를 바꾸지 않고 실제 편집기 종료값을 독립 관찰한다. `docs/proposals/2026-09-29-c4-qa-evidence-protocol-r1-draft.md` SHA `67271D88CEBBDD4A688F36F7FB766B19DA20FEE060BECD7122A33AC1B9E9E26A`는 제안이며 아직 구현 권한이 없다. 아스트라는 프로젝트 경로 지문에 결속한 운영체제 배타 잠금, 큐가 실제 하위 실행기 종료값을 관측한 뒤 최종 반환 증거를 한 번만 생성하는 절차, 실제 NUnit 이름 표기의 근거 확인, 시험·검증기 사이 동일 행 형식을 기술 선택으로 지정했다. 솔의 규범화와 루나 독립 검수를 진행한다.

## 실제 참여와 배분

| 역할·작업자 | 실제 모델 | 이 단계의 책임 |
| --- | --- | --- |
| 아스트라 `/root` | `gpt-6-astra` | 기술 선택, 승인, 최종 통합 |
| 솔 `/root/sol6_c3_design` | `gpt-6-sol` | r4 소유 계약과 QA 증거 규범 보정 |
| 테라 `/root/terra6_c3_resume` | `gpt-6-sol` | 승인 전 읽기 전용 구현 영향·실제 시험 이름 근거 확인; 승인 후 제한된 실행 연결과 시험 구현 |
| 테라 `/root/terra6_c4_qa` | `gpt-6-sol` | 새 QA 증거 규약 제안; 승인 후 정확한 새 검증 도구 구현 |
| 루나 `/root/luna6_c3_review` | `gpt-6-luna` | 설계·소스·실행 증거 독립 검수 |

## 보존과 게시 상태

기존 작업 파일, R11 소스 동결, 이전 실패·통과 증거, 기본 체크아웃·인덱스를 보존한다. 계약 Approved 이전에는 실행 소스·시험·도구를 구현하지 않는다. 새 소스에는 새 지문과 실제 입력 목록을 발급한다. 변경 요청 #3 및 기존 위키 게시 완료는 과거 게시분이며 이 문서·새 C4 작업·선행 최종 수용의 추가 문서를 게시했다고 주장하지 않는다. 게임 코드 게시 범위는 기존 미승인 게시 목록과 별개다.

## 보정 계약 독립 검수

r4 계약 SHA `EF54AE99EEF6E482C267C949A0C1B53709FF19611E9791300E07764EBD9256C0`의 루나 검수 SHA `1D4083A48C044ED868BA672B1645D8F1FCCB63AB7F0488D69522574B1822BB2F`는 정적 P0/P1=0/0이다. 동일 소유자의 준비 상태·새 세대·이력·최종 공개 규범을 확인했으며 구현 수용은 아니다.

QA 소유 초안 SHA `A883D0AF0FAC7D62BA5840676EB09377EC9F59CADAAC3F7DE770F248F932BCB1`의 루나 검수 SHA `84D0137F450E3DF3E7DA0CE6FD348509BF14FAF63F150B0338784FCBF3F960BC`는 P0=0/P1=1이다. 사실 목록과 실제 중첩 증거값의 정확한 결속·고유성·필수 범위 규범이 부족하므로 도구 구현 승인을 발급하지 않는다. 아스트라는 원본 초안 보존 후 소유 규약에 정확 중첩 기대값·형식·비교 규칙을 추가하도록 솔에게 배분했다. 이 기술 보완과 재검수 이후 두 규약을 승인하고 제한 구현을 진행한다.

## 현재 승인·구현 배분

위 승인 대기는 당시 기록이다. QA r2 검수 원문 SHA `F7B07E26C45C9B0DCD4CAE6915BAF4D0D8DD5CBE30E0D1D5D3878C060A2674D0`의 루나 재검수 SHA `BF145A1D4609943FC8EC91C795B2DA3A6734145B1C66151133F86AADD2002E1B`는 P0/P1=0/0이다. 다섯 중첩 기대 객체 및 정규 경로36개의 일대일 비교 규범을 대조했다. 독립 검수 원문 두 개를 reviewed-draft 파일로 정확 보존한 뒤 아스트라가 두 소유 규격을 Approved로 전환했다.

현재 구현 승인 기록 SHA `E1E66DF2E99FAB65E1994B220CE9ACB4FEEC16AFE5F918488CBDD866CA04CCDD`, Approved r4 SHA `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`, Approved QA r2 SHA `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`를 실제 배분에 전달했다. 테라 `/root/terra6_c3_resume`가 제한 실행 연결·신규 시험을, 별도 테라 `/root/terra6_c4_qa`가 새 도구7개를 담당한다. 둘은 같은 최종 행 스키마·소스 선언 형식을 조율하며 다른 담당 파일을 수정하지 않는다. 실제 Unity 실행은 아직 배분하지 않았다. 원래 인덱스·브랜치·C1 알고리즘·기존 QA는 보존한다.

`docs/README.md`의 현재 규범 안내만 새 승인으로 갱신했다. 따라서 과거 게시 선택 원장의 README 지문은 과거 게시 사실이며 현재 로컬 README와 동일하다고 주장하지 않는다. 새 문서·소스 원격 게시와 새 실행·최종 수용은 미완료다. 사용자 결정 사항이 생기기 전까지 구현·기술 충돌 보완·독립 검수를 이어간다.

## 구현 중 추가 연결의 현재 상태

편집 조립에서 실제 Profile 내부 제어 타입에 접근할 수 없는 공백을 기존 정상 경로와 분리한 승인 시험 제어 연결로 보정했다. 현재 Approved 추가 계약 `2026-09-29-c4-edit-control-accessibility-amendment.md` SHA `38A88A9778B38D0169FFF52979EE9753A93E2388D7E621100E2229CE5580B667`, 루나 검수 SHA `110726B562565FF89879671FAD6B7D25A125E8EA66E63B6D3119FC9BCA0BCAE8`에 결속한다. 기존377 행 결과는 새 C4 행인 척 변환하지 않고 정확 Matrix91 분기만 보존한다. 해당 Approved 추가 계약 SHA `2BD6D5CBF9F9B73BA0EBBC9B243BF5E3CA3083647F699176C2B93604EA3F9267`, 루나 최종 검수 SHA `A43CB05F103B20ED6288FC0FAAF6A1EA41FF3D566E1A21179A54B0CE20CA5513`다.

아스트라가 실제 변경 전 원본9개를 byte 대조해 `docs/evidence/c4-runtime-predecessor-manifest.json` SHA `C8FD2B74EF842F45B8D202B1DE3DC408F09F20F69C1BD030FE589E43B8DDD7F7`로 보존했다. 현재 변경 소스는 작성 중이며 동결·컴파일·실제 시험 성공이 아니다. 새 도구7개의 자체 지역 검사는 구현자의 결과이며 독립 수용을 대신하지 않는다. 잠금 최종 기록 순서·중복·정확 정수형 검사 보완을 진행한다.

현재 추가 초안 `2026-09-29-c4-fresh-checkpoint-accessibility-amendment.md` SHA `D8E0B90432CE0CE9900AABC281F3DEE05BC12475C11194631D66E7677318C988`은 새 Hub 예약 인증, 기존 제어 전달, native 이전 생명주기 fault 기록, 실제 guarded 역사 pair 읽기, 실제 C2 제어 행의 Play 배치, 원래 숫자 세대 사본과 비소비 fresh 결과 사전 인증을 한정한다. 아직 Draft이며 추가 API 구현 권한이 아니다. 루나가 실제 원본과 독립 검수하고 아스트라가 기술 승인한다. 원본 선행 선택의 서로 다른 버전 형식은 정확 경로·지문 한정 예외를 별도로 검수한다.

솔의 완료된 설계 참여는 이력으로 남긴다. 추가 작업자 생성은 현재 작업자 한도로 실패했으며 재시도하지 않았다. 아스트라가 추가 한정 설계를 담당하고 루나가 독립 검수한다. 실제 참여 모델은 계속 아스트라 `gpt-6-astra`, 구현 담당 두 테라 `gpt-6-sol`, 검수 루나 `gpt-6-luna`다. GPT-5.6 작업자와 다른 제공자 호출은 재개하지 않았다. 기본 HEAD `309f2204cf19a321ae74c92f3be0e3fc94e3499e`, index SHA `933406B74802B4AAE96DC8B77AF515E60C0BA41D23975CE860DF62B0E58C2504`는 추가 초안 작성 후에도 실제 동일하다.

선행 선택 형식 초안 `2026-09-29-c4-predecessor-selection-format-amendment.md` SHA `DFBF92EB722CEEEFBD8A16F85338B776F01C2AC692C4DA058FB6BC79F582B754`의 독립 검수를 배분했다. 실제 Worker51 원본은11키이며 선행 실행은 EditMode다. 신규 스키마를 원본에 소급 요구하거나 원본을 재발급하지 않는 정확6경로 분기다.

위 runtime 추가 Draft 대기는 당시 기록이다. 루나 독립 검수 `2026-09-29-c4-fresh-checkpoint-accessibility-luna-review.md` SHA `F16A23DAE0B43F6FB4D3274B3C6202F031B58ADDBBB5FA601734F5CAFE0ABE63` P0/P1=0/0을 확인하고 동일 검수 원문을 보존했다. 아스트라가 현재 Approved SHA `7C4BE9991171CEEAA44EE7C615B20BF76982AB66552F76F0F2F111BFAC6D28BF`로 전환해 runtime 담당에게 추가 내부 연결을 배분했다. 실제 실행 승인은 아니다. 작성 속도를 위해 기존 r4 허용 신규 strict 감사 파일·meta 두 개의 구현 소유는 별도 QA 테라에게 이전하고 runtime 테라는 실제 동작 시험·fixture·runtime에 집중한다. 루나의 독립 검수 역할은 유지하며 구현자 자신의 수용 권한은 없다.

2026-09-30 선행 선택 형식도 루나 설계 P0/P1=0/0, 검수 SHA `B27EA96872216350E0EF3CC8E368728BD3E2C07EAD73DBB81F86239DE5CDB071`을 확인하고 동일 Draft 원문을 보존한 뒤 Approved SHA `26C180D05DEAA4DA59515BD891FF94638A5548E259F4BE18EC218EF90087A789`로 전환했다. 도구의 필드 없는 Edit562/Worker51/Play610에서 고정 플랫폼을 강제하지 않는 별도 구현 P1을 발견해 QA 테라에게 보정을 배분했다. 설계 승인과 실제 도구 수용을 구분하며 아직 Unity 실행은 배분하지 않았다.

위 도구 P1은 이후 한정 재검수로 닫혔다. 독립 루나 도구 검수 원문 SHA `5CF6508FF4E44837C7BE312F0BFFDB4276BC2A5C0E4D116C64C344AAF9571FAF`은 보존한다. 수정 verifier SHA `A022A2AB896910050570DF0A54266497446738D126C22379835F788E7C55E0CC`, 독립 재검수 `2026-09-30-c4-predecessor-platform-luna-rereview.md` SHA `55A13AA8E51528445C4629EC06C7434946474CCA71C155CE8D06F4978F19387A`의 실제 지역 함수 정상6/오플랫폼6/오지도6/오부분2 대조와 다른6도구의 동일 지문을 확인했다. 이는 정확 한정 분기의 정적·지역 검수 폐쇄이며 최종 fixture/원장/프로세스/큐/Unity 실행 수용이 아니다. 아스트라는 해당 한정 보정을 통합하고 전체 입력·계획 동결 전 최종 대조를 유지한다.

## 작성 중 소스의 선행 경계 보정

아스트라가 실제 `IsExecutionAvailable`의 null owner 조회와 원래 Owner 참조 요구의 차이를 확인했다. 구현 담당이 관리 조회 입구에서 null을 비변경 거절하고 정상3인자 실행의 내부 원장 소유자 추출을 유지했다. 별도 fresh 관리 조회는 이미 예약·종료 기록이 있는 경우를 함께 거절해 단일 현재 head 복원으로 재사용하지 못하게 했다. 아직 실제 NUnit 실행은 없다.

아스트라는 mutable peer projection을 종료 대상으로 삼는 Owner Terminal 경계와, `AfterFreshConsume`의 정상 반환 뒤 종료된 상위 슬롯을 재검증 없이 최종 공개할 수 있던 경계를 선행 검수에 배분했다. 기존 Approved 원본·부분 실패 규칙 안에서 Terminal의 readonly Cohorts 참조 사용, 마지막 callback 뒤 Owner/원래 commit/Q-A 준비·커서/lower 미종료 예약 재인증, Q-B의 실제 Preparing Owner 폐쇄 전파를 보정했다. 새 callable은 추가하지 않았다.

루나가 메시지로 전달한 작성 중 스냅샷은 Bridge `27A601C24F8BAA3E0B6C800900A23D800DDA1F08DB4DCFBE28D67699EDB6B313`, lower `8248404F3F237936954979F3FEC2848E9F419CF0AA2A781F732D5CA379FF9809`, Owner `5394938E5743B94E10B224B48A2E04CF2F8777BA29306FD516DD694F7D8FDEC7`이다. 독립 읽기 전용 확인에서 앞 두 위험의 당시 정적 경로는 보정됐으며 별도 cohort bool 이후 실제 발급 전 검증의 우회는 확인하지 못했다. 이는 최종 소스 지문·완성·실행 수용 기록이 아니며 최종 호출 지배를 다시 대조한다. C4 fresh 이후 C3 Cancel/Rearm의 최신 슬롯 우선순위, 전체 동작 시험·공유 행 정의, Q0 최종 입력 결속은 아직 작성 중이다.

아스트라가 r4 승인 범위의 `docs/evidence/c4-audit-predecessor/` 원본4개도 덮어쓰기 없이 보존했다. Q0 `C27116AC35BF83D4AB8794FA9D3E379AD6F127B0E52B48C51399F49ADDF862D4`, Q-B legacy `57E7EFB17307BFB156D338F7BDCE25104E3192748FD6C159F7BB7B4A7483DAD6`, C3 successor `1B0F8D0919770CD1FBD06F908EC23E370A18E911EC5AAC09938FAF0BFDA174E6`, Q-A scope `1DB3012C82DD6E0A1348ADEAF954EF0056EB28E925108716B6317289AAA93D60`를 각 원본과 복사본에서 실제 대조했다. 신규 strict 감사가 읽을 입력으로 원본 byte를 보존한 것이며 실행 수용은 아니다. 아스트라의 새 도구7개 문법 재검사도 오류0이며 당시 실제 Unity 프로세스는 없었다.

## 2026-09-30 현재 배분과 발급 역사 구분

신규 `ProfileResetExecutionBridgeV1Tests.cs` 편집 시험 본문의 구현 소유를 QA 테라 `/root/terra6_c4_qa`에게 이전했다. runtime 테라는 해당 파일 편집을 중단하고 runtime·두 fixture·실행 시험·공유 행 정의·Q0 입력 결속을 담당한다. 기존 파일·사례 이름·본문 순서와 승인된 손상 경계는 보존하며 새 foreign Router 교체나 손상 seam을 만들지 않는다. 독립 검수자는 계속 별도 루나다.

실제 execution CWT 등록은 전체 payload 검증과 consume 앞에 발생하며, C2 영수증 발급은 C4 결과 발급보다 앞에 발생한다. 예외 이후 이미 발급된 세대를 Absent/Unobserved로 보고하거나 전체 검증 실패를 정상 Positive로 인정할 수 없다. 아스트라가 좁은 추가 초안 `2026-09-30-c4-issued-generation-evidence-amendment.md` SHA `148E66886B58584B2C0C5AC825FDF4FA1A4991C00F6FF789C53D4601A3EEF3B6`을 작성해 루나에게 독립 검수를 배분했다. 새 두 규칙은 아직 Draft이며 관련 행·도구 분기 구현은 승인 대기다. 다른 승인된 동작 시험과 runtime 구현은 계속한다. 현재 새 source 동결·실제 Unity 실행·최종 수용·원격 게시를 주장하지 않는다.

위 Draft 대기는 당시 이력이다. r1 검수 SHA `823FCB3E66FFC98791F9C285A574C53EDAB2708407DC0EE9552CF0042044EAF8`의 P1은 receipt와 same C4 record의 정확 session/pair 연결 경로 부족이었다. 실제 원장 조회로 source 안의 필드와 생존을 확인하고, 원본 GuardedAdapters/Routers→두 Adapter cohort→readonly session/receipt/candidate witness를 대조하는 r2를 작성했다. 지역 capability 객체를 관찰했다고 주장하거나 약한 원장을 열거하지 않으며 실제 생성·소비·발행 지배는 독립 동결 source 증거로 분리한다. r2 원문 SHA `AF4051D04CCB629A2C0DE051C36D2258AAAB3E398DBDF014C6FD8481FE7F408E`을 exact 보존하고 루나 재검수 SHA `FC09020E5159BA1E062674308EF757EBCA5DA84244775F936D6E39F16F2C680A` 정적 P0/P1=0/0을 확인했다. 아스트라가 Approved SHA `FF88BFC0471FB48C2828765A1C6FBD0F3913F0DD123CDEB16BCBADD4F8F2DCB6`로 전환해 fixture/emitter와 두 행 도구의 한정 규칙 구현을 배분했다. 실제 실행·최종 수용은 아니다.

작성 중 Q-B 정리에서 mutable fresh 슬롯이 다른 발급자의 actual witness를 가리킬 때 외부 witness를 닫을 위험을 확인했다. 단순 거절로 자기 폐쇄를 건너뛰지 않도록 실제 등록 직후 보존한 private readonly 원본 역사와 original issuer/thread/peer association을 정리의 지배 조건으로 구현 보정한다. Q-A도 같은 위험을 점검한다. 새 callable·등록부·발급 경로나 손상 seam은 추가하지 않으며 기존 r4의 원본 cohort 폐쇄와 foreign winner 비변경 요구에 한정한다. 최종 source 대조는 남아 있다.

## 두 현재 파일의 실제 지문 자원과 불변 선행 지도

runtime 담당이 Adapter/Router 두 파일의 추가 편집 계획이 없음을 선언했다. 루나 독립 제한 정적 검수 `2026-09-30-c4-adapter-router-pin-luna-review.md` SHA `06D5CE50E6B217B62F0AF3D6CFA102C5FDE2F25764A0F53A6D5CCA8C50BE9F9F` P0/P1=0/0과 아스트라의 exact byte 재대조 후, Adapter `4B157B98511A0978D19B25434F6D00E23C0CC6FB53BA8A14969182865C3BDC0E` 및 Router `542FB7DCCC92B8B59DDDBE4A77741BBEFEEE65546F315F5BB2AA3FA8D22B57F5`를 현재 후보로 고정했다. 아스트라가 `docs/evidence/c4-audit-current-source-pins.json` SHA `FCA015B48B9EDE8253D4E80C21521B92342DCA7F5B7C87F0BF70A59CE7C74548`을 CreateNew 한 번으로 발급했다. 원본 binding은 실제 보존 txt 경로의 옛 SHA와 연결하고, 승인·독립 검수 원문을 실제 파일 SHA로 결속한다. Q0와 strict 감사는 이 정확 자원부터 검사한다. 다른 runtime·fixture·전체 통합·실행의 수용은 아니다. 이 두 후보가 바뀌면 현재 자원의 새 수용 근거로 재사용하지 않는다.

아스트라는 원본 선택7개 파일 지문과 실제 names/count를 읽어 원본 Edit240/Play15/Edit562/Worker51/Play610 및 Matrix91/Remaining149 순서를 그대로 담은 `artifacts/c4-predecessor-selection-map.json` SHA `7647BB0AB3DD488080A53794C8C27DB0D7600FD7D3F79FB21E40CA9102851C91`을 CreateNew 한 번으로 발급했다. Matrix91와 Remaining149의 실제 서로소 합집합이 원래240과 일치함을 확인했으며 전체240 이름 순서를 연결한 두 부분 순서로 재발급하지 않았다. 기존 숫자는 불변 선행 선택의 실제 값이며 신규 C4 개수·계획·실행·통합 수용을 뜻하지 않는다.

루나의 작성 중 cleanup 사전 검수는 원본 history 또는 original slot을 같은 발급자의 이전 유효 노드로 되돌리는 경우 최신 실제 슬롯이 닫히지 않을 수 있는 P1 후보를 보고했다. 구현 담당은 독립 current/pending/CWT/history의 actual 자기 발급·readonly tuple/thread를 확인해 최신 자기 슬롯을 정리하도록 보정 중이다. 외부 witness는 정리 대상이 아니며 자기 상태 폐쇄는 finally로 지배한다. 이 부분의 최종 정확 지문 검수는 아직 남아 있으며 두 Adapter/Router 지문 검수와 합쳐 폐쇄했다고 주장하지 않는다.

## 런타임 사전 검수와 남은 증거 공백

증거 공백의 최신 처리: 일반 저장 미관측 규범 `2026-09-30-c4-ordinary-writer-observation-amendment.md`는 루나 설계 검수 SHA `84BEB74F72B825846D280278C161A97FAF9135B60B1E9753B486FE1228C82AF5` 정적 P0/P1=0/0과 원문 SHA `9D24687E2BA0A1C301AE42BCCB920F8DB16152E9454C39D0AEE92AA994A0E8EB` 보존 뒤 아스트라 Approved SHA `B886A65AD69DCED7230CA8E1E766E23483F93EFD047073FECF8C89F8D61A7EDE`가 됐다. 같은 사례의 실제 public Save가 없을 때 writerBlocked 한 leaf만 Unknown이고 AC006 실제 관측은 유지한다. C1 미호출 일곱 경계 규범 `2026-09-30-c4-pre-c1-failure-evidence-amendment.md`도 원문 검수 SHA `C65C62A9D67B9F2447AE9949C0E2C29ABE0CA071A2EB91DF7F0FA1FDB6F3F928`, 스키마 키 철자만 고친 후 재검수 SHA `4F42CDEAE8C6CE8DAD78CFF668CDB61D884340E4BB39B5F57CCE182DDE07B53F` 모두 P0/P1=0/0, 원문 초안 SHA `5AA8812873040272389EC46CBE761F30A8DCB98569685A056C8DB34AEF4995F3` 보존 후 Approved SHA `8B312BF1374B9B8A0164D4937A9D84ECAF31C69B8058DB7D4CEB64B1D6339F1D`가 됐다. 두 구현 담당에게 정확 범위만 배분했으며 실제 결과는 미발급이다.

운영 도구 독립 검수 `2026-09-30-c4-operational-tools-luna-review.md` SHA `717F7F7943EB8EBC21A262BB41E952CF9BA474CBFB5335D1A96E3BB447AF29EF`가 신규 C4 행의 XML과 실제 실행 XML 결속 누락 P1을 발견했다. QA 담당이 verifier를 SHA `EEE8F4E22A107D54776589E706746327FC6A7DCF1D49F52603D768F8474E2BFC`로 보정하고 국소4/4를 기록했다. 루나 한정 재검수 `2026-09-30-c4-verify-run-xml-binding-luna-rereview.md` SHA `B4DC98B372D4EF56E6D0FC13256DC61BE3A0FAAC239B0F7C352443A6D30CB4A3` 정적 P0/P1=0/0은 해당 P1만 닫는다. 열거 source 직접 입력의 목록 포함 조건은 최종 계획에서 실제 `ParserEvidence.EnumSources` 경로를 전부 InputPathList에 결속해 별도로 확인한다. Unity·전체 원장/계획/검증 큐의 수용은 아니다.

도구 상태 추가: QA 담당이 네 C2 미호출 경계의 한정 두 도구 분기 및 국소66/66(정상4·음성62, 실제 PowerShell 종료0)를 기록했다. 구현자 자료 `2026-09-30-c4-pre-c2-terra-localcheck.md` SHA `D73208C4FC4F876A89D16E729A6A53805F7382D18C1FF0215A658C7497255948`는 합성 조건 검사이며 실제 CWT·전체 행·Unity 증명은 아니다. Root가 실제 Bridge의 무관한 private 암시 enum 때문에 집중 선택 생성기가 실패할 구체적 파서 P1을 발견해 한정 보정을 배분했다. QA 수정후 도구 SHA `CAE02BEF76CCEC382AE44AC34E3D552097189F79402589DB91A30230667260CB`, 실제 source 대상 국소10/10, 구현자 기록 `2026-09-30-c4-focused-enum-terra-localcheck.md` SHA `2A9F8D38F389922DBE04118553823AE7FC142E0437714E3500A15F67C885C67E`이다. 루나는 변경 전 P1을 독립 확인했고 운영 도구 나머지 및 최종 파서 바이트 검수를 계속한다. 선택 JSON 발급·실제 Unity 실행은 아직 없다.

실제 marker/lease 두 게이트가 없어도 손상된 profile.json이나 다른 검증 실패로 일반 Save 호출이 실패할 수 있다. 따라서 SourceEstablished로 `barrier.ordinaryWriterBlocked=No`를 단순 발급하는 분기는 보류했다. 구현 담당은 현재 네 C2 실패 Play 행과 Busy/Stale fresh 세 행의 원본 사후 상태를 바꾸지 않고 public Save를 호출할 수 없다고 식별했다. 기존 AC006의 실제 writer 관측은 별도로 유지하며, 루나에게 `No`의 규범 의미와 한정 미관측 표현을 독립 검토하도록 배분했다. 이 공백이 닫히기 전 일곱 행의 원장 통과는 주장하지 않는다.

최신 갱신: 아래 네 경계 증거 초안은 독립 루나 검수 `2026-09-30-c4-pre-c2-failure-evidence-luna-review.md` SHA `65C7D8CEA657634AF6234B10C28508A75480BA88BE8B204EA4DC467583BFB790` P0/P1=0/0을 완료했다. 검수 원문 SHA `39DBF9B54C6722AD0AAF5705E5EE3FE4E4C2A28DA5373E52B8544BB6A2C95686`을 그대로 보존하고 아스트라가 Approved SHA `7F1BAA3821580E7382258B7C8BD0F045818FF51B9334AD0C905EB7B9A6388356`로 전환했다. 두 구현 담당에게 정확 네 실패행·한정 세대 규칙·진단·도구 음성 검사를 배분했다. 아래 초안 대기는 당시 이력이며 새 API·runtime 변경·실제 실행의 승인은 아니다. 추가로 루나에게 운영 도구5개의 잠금·프로세스·입력 동결 경계 독립 검수를 배분했다.

위 cleanup 보정 뒤 루나가 실제 런타임8개 지문을 확인한 `2026-09-30-c4-runtime-eight-luna-prereview.md` SHA `7B7731B4F887A4C2DBFDE26FF0CA7543390B05CFCAB80161571D7988BCA8FFB9`를 발급했다. 런타임 정적 P0/P1=0/0이다. 정상 C2 완료의 종료는 오류를 만들지 않으며, 이후 실제 파괴의 새 생명주기 오류 사건은 불변 결과 이력을 변경하지 않는다는 규범 대조도 완료했다. fixture/원장/실제 실행·전체 수용은 이 판정의 범위 밖이다.

같은 검수에서 네 C1 반환 뒤 C2 미호출 예외행의 memory 미관측 표현 규칙이 부족한 별도 증거 P1을 확인했다. 아스트라는 `2026-09-30-c4-pre-c2-failure-evidence-amendment.md` 초안을 작성하고 독립 검수를 배분했다. 해당 분기만 승인 대기하며 다른 승인된 시험 연결을 계속한다. proof 미관측을 부재나 정상 참조로 바꾸지 않고, memory 세대 null/Unknown도 정확 네 경계의 추가 규칙 없이는 수용하지 않는다.

발급 세대 도구의 합성 지역 검사33개 뒤 진단 값 오류5개 검사가 추가됐다. 구현자 기록 `2026-09-30-c4-issued-generation-terra-localcheck-r2.md` SHA `319AC19AB46243FEE29280B2D417328AFADF5F2F1FCC910B7D313FCBD9F9FD10`, 독립 루나 재검수 `2026-09-30-c4-issued-generation-tools-luna-r2-review.md` SHA `71C0DECDA855551DE7A63894759B984AFA37B22C68734150BC03BDCC2E7A9920`는 이전 검사 범위 P1의 한정 폐쇄다. actual CWT·emitter 지배·Unity 결과의 증명이 아니다.

기본 제어의 호출 수와 비필수 native 관측 부재는 [한정 기술 해석](../approvals/2026-09-30-c4-noop-call-count-evidence-interpretation.md)에 따라 SourceEstablished/exact null로 분리한다. 실제 읽는 변경 없는 writer 소스도 최종 source/input 결속에 포함하며 변경 권한을 확대하지 않는다. 아스트라가 변경 금지 R11 소스·시험·meta10개, 기본 HEAD `309f2204cf19a321ae74c92f3be0e3fc94e3499e`, index SHA `933406B74802B4AAE96DC8B77AF515E60C0BA41D23975CE860DF62B0E58C2504`가 그대로임을 다시 확인했다. 읽기 전용 프로세스 조회에서 Unity 실행은 없었다. 새 컴파일·검증 큐·Unity 실행·최종 원장·원격 게시를 발급하지 않았다.

2026-09-30 추가 증거 경계: 실제 CWT 발급 뒤 결과 반환 전 실패 중 기존 C1/C2 callback throw 개정으로 표현할 수 없는 원본 투영 손상2·callback 반환 뒤 생명주기 실패1·prepared proof 손상1·proof 관측 뒤 checkpoint12/13 실패2·Busy 새 handback 공개 전 checkpoint17 실패1의 정확 일곱 사건을 [한정 개정](../specs/work-contracts/2026-09-30-c4-issued-unreturned-boundaries-amendment.md)으로 구별했다. 루나 독립 설계 검수 SHA `3BFD9475BDA9CD6C47E152A5272892D1D9E5C42BF3B426EB9B872DBDF73F8B89` P0/P1=0/0, 원문 초안 SHA `5E8E7844117AB21AE14C6654105EAABF2897DAA567335ABB666C55C783E3669B` 보존 뒤 아스트라 Approved SHA `0AEC1429EB90B924218C070DF2C07816E4B1C9CE1F9B0E648F71D6F912AD1B6D`로 전환해 두 구현 담당에게 해당 부모/fixture/ExpectedRows와 builder/verifier의 제한 분기만 배분했다. checkpoint26은 결과 전이지만 기존 receipt 규칙, 18/27은 이미 발급된 결과 규칙이다. QA 도구의 pre-C1·writer Unknown 한정 독립 검수 SHA `190BCD91517D21ABDAE8B10BA48755F9BD919C9CE552C3ED1AE714E1B7202345` P0/P1=0/0은 합성 검사에 한정된다. 실제 Unity·전체 원장·동결·통합 수용은 아직 없다.

추가 한정 보정: C4 checkpoint17의 C1 Busy 객체는 반환 전 CWT/observer에 보존되지 않아 직접 Observed로 발급할 수 없다. 실제 root lease 보유·checkpoint 도달은 관찰하고 동결 C1/C4 source로 Busy를 SourceEstablished로 분리한다. 같은 private proof 손상 본문의 두 번째 공개 AC005 부모는 별도 실행·행으로만 포함한다. [승인 문서](../specs/work-contracts/2026-09-30-c4-unreturned-parent-and-busy-source-amendment.md) SHA `D99B4306B9CDECB74E76415954ECF5E3CA28C17514D36AACB5DFDBE0FA45CDF8`, 루나 설계 검수 SHA `E455AE2922EC6099B1FA2526A4918D8B6C95B117233917960E8C027D2183BC7B` P0/P1=0/0, 원문 SHA `9042EF2F72EAF1D17B48E57527B3F01C7C737F1D24A373B6A8054D7F89CD08B4` 보존 뒤 두 구현 담당에게 정확 범위만 배분했다. QA 도구의 별도 certainty P1은 다섯 실제 계수 Observed/freshComplete source-only SourceEstablished 보정 후 독립 재검수 SHA `783F527BB88E120D4899B2856FF68252D628BF99AEA83A3F40AAF35AF252F41D` P0/P1=0/0으로 닫혔다. 실제 source 행·최종 전체 도구·Unity 결과는 여전히 미발급이다.

추가 QA 도구 한정 재검수: 손상 proof를 정상 C1 장벽 근거로 승격하던 P1을 닫고, 정확 공개 부모 이름의 namespace/class/case 결속을 두 차례 보완했다. 최종 이 분기 verifier SHA `0F4C91B2622F097CEED4DBE23D020C0D26D4F7A68535D3EBD88C3A76CC4E2326`, 합성184/184 실제 종료0, 루나 한정 재검수 SHA `927C59A0B2A68C614F1A0B3D9E824DD61383A0FCB8366AC6508619DDE25860BA` P0/P1=0/0이다. 이는 새 일곱 경계 및 추가 부모의 특정 도구 분기만 닫으며 작성 중 부모·fixture·ExpectedRows, 최종 전체 입력/계획, 실제 Unity/원본 CWT 관측을 수용하지 않는다.

이후 같은 verifier의 나머지 projection/lifecycle/proof 부모도 전체 NUnit 이름과 대소문자 기준으로 정확 한정했다. 새 분기 verifier SHA `20418B63A488C6ED8D3EF2088BE8FAE719E7E142315B92EE029CFA3AB9C3CD69`, 합성190/190 종료0, 루나 차이 재검수 SHA `A86940445E8C8061A6ADB749959853637DC8DBC423646BA05B63C7AC6EEDEC79` P0/P1=0/0이다. 루나는 14..16·26을 실제 receipt 검증 전제의 무결과 경계, 18·27을 이미 발급된 C4 결과, 19..25를 발급된 원본 Busy 결과 뒤 fresh 실패로 읽기 분류했다. 작성 중 fixture의 실증이나 Unity 결과는 이 정적 분류의 범위 밖이다.

편집 checkpoint 및 사전진입 시험의 독립 사전검수 `2026-09-30-c4-edit-checkpoints-and-unobserved-luna-prereview.md` SHA `711AC737A08F6A9166F511E71064F190DC831F6CA984BF4E1A1C1F2680F7F1BA`는 당시 소스에서 P0/P1 차단을 찾지 못했다. 이후 루나가 정상 receipt 경로의 C2 결과 검증 실패를 삼킬 수 있는 fixture P1 후보를 발견했고, QA 테라가 부모별 14=NotInspected, 15/16/26=Validated, AC007 손상=Rejected를 실제 Validate 결과와 단언하도록 보정했다. 루나는 이 한정 보정을 다시 확인했다. 두 fixture ExpectedRows는 작성 중이며 현재 정적 집계는 Edit148/Play35/Hub5 총188행이다. 이 숫자는 최종 원장·실행 수용이 아니다. 원본 Busy 행을 후속 성공의 현재 영수증으로 다시 덮는 Play 네 부모도 최초 관측을 보존하도록 보정 중이다.

아스트라는 원래 체크아웃의 HEAD·인덱스를 보존한 채 새 변경 요청용 격리 작업 공간을 만들고, 실제 원격 기본 가지 `4d864484f75939dcf9c93c055d296613cf1ba1dd`에서 `codex/c4-integration` 가지를 준비했다. 현재 C4 소스는 그 작업 공간에 게시하지 않았으며 최종 원장·입력 동결·Unity 실행·원격 게시도 아직 없다.
