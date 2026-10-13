# B1 실제 Unity의 새 실행 맥락 단회 재관측

- 상태: **Approved** — §6의 아스트라 한정 승인으로 Root의 새 실행 맥락 가져오기·저작1회·합산900초만 허용한다. 실제 실행 결과·B1/CH1 수용은 별도다.
- 설계·판정: 아스트라 `gpt-6-astra`·`ultra`. 제품 여섯 파일 및 영향 최소 컴파일 계획은 솔 `gpt-6.1-sol`, 독립 규범/소스/실행 제어 도구/원시 검수는 루나 `gpt-6-luna`, 단일 실행과 프로세스 관리는 Root 소유다. 역사 담당·모델은 보존한다.
- 권위: 주 계약 §27까지 SHA `CA1E1194A1C5C059D4868BAE009BB1F619AD14C2BC92CC260390DF2B8D4C5F83`; §26 저장 재시도 승인과 §27 G3→집중35 순서를 유지한다. 본 작성은 그 원문을 수정하지 않는다.

## 1. 사용자 동작·새 관측 목적·종료

사용자가 현재 소스의 CH1에서 검증된 방으로 출정할 수 있도록 실제 가져오기·컴파일과 기존 scene1/prefab8 저작의 다음 원시 근거를 확보한다. 아직 실제 플레이·B1 수용·CH1 완주 전달은 아니다.

첫 G1/G2 r1은 실제 Unity가 시작된 뒤 31.502초에 UPM IPC 초기화 실패·exit1로 종료했다. 당시568 불변·후보9/meta0이며 컴파일/저작 완료 증거가 없었다. 이 실패와 단회 소진 이력은 그대로 남긴다. 새 시도는 그 권한을 복구하거나 자동 반복하는 것이 아니다.

새 목적은 **도구의 명시 승인 실행 맥락에서 실제 Editor가 자체 UPM 연결 단계를 지나 현재 최종 소스 컴파일·기존 저작 입구까지 도달하는지 한 번 관측**하는 것이다. 별도 프로토콜 없는 최소 비교에서 같은 서버/클라이언트 스크립트 SHA `053FD53F6C99922DF50042BC92A8AAFACAC7D6E05A32A585CE99BF6BEFDE8C8F`로 기본 맥락은 게시 후 접속 권한 거부, 명시 승인 맥락은 게시 후 연결 성공이었다. 둘 다 정확 소유 프로세스를 종료했고 프로토콜 바이트·패키지 요청·Unity 추가 실행은0이었다. 실제 Editor 성공이나 r1의 확정 원인을 뜻하지 않는다. 승인 맥락의 실제 환경이 기본 맥락과 완전히 같았다는 주장도 하지 않는다.

이 문서 작업의 종료는 새 Review 및 준비 원장의 지문을 남기고 독립 검토에 인계하는 때다. 향후 허용될 단회의 종료는 정상/실패/상한 어느 경우든 정확 소유 프로세스 종료와 원시·전후 지문·독립 결과 대조까지다. 다음 관문을 자동 연쇄 실행하지 않는다.

## 2. 확인한 근거와 아직 필요한 근거

| 구분 | 직접 확인한 지문 또는 현재 상태 | 의미 |
| --- | --- | --- |
| 첫 G1/G2 원시 독립 보고 | `B12F9DE7EC3B71FECC4D20CE7B34DAB981751D2657AFF7F20E229D25D81FAC8C` | r1 환경 실패와568 불변·후보0. 소스 컴파일 실패로 변경0 |
| 기본 맥락 비교 r2 결과 | `4867A0551270BFAE547F0672D3F5E45D4DF069B7CDFC8B7FD23ACF8469457E7E` | 게시true, `UnauthorizedAccessException`, 연결false |
| 명시 승인 맥락 비교 r2 결과 | `134C844F469416D03D78151335F39DF45B89F4FE6C4098A1BF00D7800FE38583` | 게시true, 연결true, 원시의 소유 종료·관리오류0 |
| 비교 도구 최초 r1 | 스크립트 `7520720FB73B23852F0BAC2CDD95FC8A854F76EDA60A488C99E43AC833162402`, 결과 `5423877F246DDFD372ACE526F9E9068AEA8473308CBE8A3BFAB3F729DEFEB33E` | stdout 공유 읽기 충돌·정리 성공을 보존. r2의 ReadWrite 공유 reader 보정과 구별 |
| B1 현재 관측 v5 | `A603D541D4D3C4BE782CFFB32D27BA5EB0974384291F11B41D9F6BF2D4F9AD41`, 568/정확6변경/쓰기 종료 표시 | 구현자 원장 관측. 최종 독립 델타나 실행 직전568 전수 재결속을 대신하지 않음 |
| UPM 비교 독립 보고 | `D6DA33AF4BBA2B1A7468D345857584C1D16EDC3260FED7DFB102146C03617F1F`, 실제 루나 원시 검토 P0/P1=0/0을 직접 읽음 | 단순 IPC 맥락 차이만 입증. 현재568·실제 Editor·추가 실행 권한을 발급한 보고가 아님 |
| 제품 독립 델타·최소 컴파일·제어 도구 | 현재 결과/확정 지문 미결속 | 아래 실제 선행을 모두 충족해야 함 |

**REQ-C1B-CTX-001:** Root는 실제 실행 전에 다음을 모두 하나의 준비 원장에 결속한다.

1. 실제 루나의 UPM 비교 원시 검토와 이 Review 규범 검토를 직접 읽어 미결 지적을 처리하고 아스트라의 명시 Approved를 받는다. 현재 Review만으로 실행0이다.
2. 솔의 정확 여섯 파일 구현 종료, 현재 최종568 원장 및 `sourceWritingClosed=true`, 루나의 최종 델타·소유/접근·35선택 검토, 아스트라의 그 한정 소스 판정을 확인한다. 현재 v5를 잠정 핀으로 적었으나 보정이 생기면 실제 최종 원장/지문으로 다시 결속한다. 쓰기 진행 중 Unity0이다.
3. 영향 분석에 따른 **기존 최소 컴파일 계획·선택·입력·단회 상한과 독립 검토**를 확정한다. 그 계획이 요구하는 실행/독립 결과까지 완료한 뒤 진행한다. 본 계약은 별도 native 컴파일 권한·성공 어셈블리 재컴파일·추가 대형 진단을 만들지 않는다. 실제 Unity의 자체 컴파일은 아래 같은900초 안의 G1 단계다.
4. 아래 실행 제어 도구의 실제 파일·SHA와 루나 독립 검토를 먼저 결속한다. 기존 r1 도구 SHA `889641EA861D56844BCEA1C965886A7EEA4DD26B8AA12A8B234E448DA4F1AB10`는 역사로 보존한다. 새 도구는 현재 최종 원장·승인 지문·원시 경로·맥락·종료/후대조만 최소 보정하며 기발급 결과를 덮지 않는다.
5. 현재 프로젝트 및 충돌하는 Editor가 종료됐고 Root가 실행 단일 소유를 인계받았는지 확인한다. 동일 프로젝트의 다른 쓰기/컴파일/시험/Unity를 병행하지 않는다. 별도 C1C 사본 작업만 기존 소유 경계 안에서 가능하다.
6. 실행 직전 Assets/Packages/ProjectSettings의 정확 최종568 경로·길이·SHA·중복/여분0을 다시 대조하고, 아래9후보와 각각.meta 모두 부재를 확인한다. 기존 부모 meta도 현재568에 존재해야 한다. 모호한 이전 부분 산출은 지우거나 덮지 않고 중단한다.

실행 제어 도구는 단일 인계 원장의 정확 경로·SHA와 `ToolExecutionContext=require_escalated`를 필수 인자로 받고, 인계 원장의 각 실제 path+SHA를 다시 대조한다. 인계에는 helper, 후속 Approved 계약/아스트라 승인, 실제 최종 checkpoint(보정 v6 등 가능)/소스 한정 판정/루나 델타, r5 최소 계획·결과 및 각각의 루나 검토, helper 루나 검토, r1/UPM 독립 보고, 설치 EXE와 주 계약을 포함한다. 미완료 근거는 null 상태로 실행을 차단하며 현재 v5로 대체하지 않는다.

후속 아스트라 실행 승인 JSON은 `finalApproval=true`, `recoveryContractSha256`, `helperSha256`, `checkpointSha256`, `minimumResultSha256`, `sourceDeltaReviewSha256`, `minimumResultReviewSha256`, `executionHelperReviewSha256`, `singleExecutionCount=1`, `aggregateLimitSeconds=900`, `executionOwner=Root`, `toolExecutionContext=require_escalated`, `allowedExecuteMethod=AcadeGameMaker.Combat.Authoring.Chapter1ExpeditionAuthoringBuilder.Build`를 정확히 결속해야 한다. 이 목록은 미래 승인 형식이며 현재 true나 SHA를 발급한 것이 아니다. 인자 문자열 자체는 도구 심사 승인 증거가 아니며 실제 호출 맥락과 심사 결과를 별도로 기록한다.

## 3. 제안하는 정확 단회 범위

**REQ-C1B-CTX-002:** §2가 충족되고 별도 명시 Approved가 발급된 뒤에만 Root가 **새 단회1회·합산900초**를 요청할 수 있다. 범위는 같은 Unity6000.6.0f1(`f7f8ed4d1e24`)의 실제 가져오기·자체 컴파일→기존 `AcadeGameMaker.Combat.Authoring.Chapter1ExpeditionAuthoringBuilder.Build` 한 번이다. 가져오기900초와 저작900초로 나누지 않는다. G1 실패 뒤 별도 저작 실행, G2만 재호출, 실패 후 자동 반복0이다.

- 프로젝트: `output/chapter1-playable-r1/project`.
- Editor: `C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Unity.exe`. 설치 버전·실제 EXE 지문을 실행 입력으로 기록한다. 다른 설치나 재설치는 범위 밖이다.
- 의미 인수: `-batchmode -nographics -quit -projectPath <정확 프로젝트> -executeMethod AcadeGameMaker.Combat.Authoring.Chapter1ExpeditionAuthoringBuilder.Build -logFile <새 원시 로그>`.
- 도구 실행 맥락: 정확 검토된 제어 도구를 `require_escalated`로 **정식 자동심사**에 요청한다. 이는 관리자 권한 또는 특정 토큰 수준을 미리 보장한다는 뜻이 아니다. 실행 시 실제 심사 결과·맥락·명령·지문을 기록한다.
- `-upmIpcPath` 주입·외부 서버 선기동·패키지 해석 우회·보안 ACL/프록시/환경 설정 변경·캐시 삭제·설치·자격증명/라이선스 조작은0이다. 부모 Editor가 기존 방식으로 UPM을 사용하도록 둔다.
- 원시 경로 접두사: `output/chapter1-playable-r1/evidence/unity-b1-import-authoring-context-r2`. 이 새 시도의 start/end/source-before/source-after/process-observations JSON과 Editor/stdout/stderr 로그를 각각 분리하고 존재 시 덮어쓰기0이다. 제어 도구 후보는 `output/chapter1-playable-r1/tools/b1-unity-authoring-context-r2-root.ps1`로 한정한다. 아직 생성/검수되었다고 주장하지 않는다.

**REQ-C1B-CTX-003:** 저작 가능한 산출은 다음9개 및 각각의.meta뿐이다. 기존568 바이트 변경·삭제, 다른 신규 source/meta/assembly/friend 또는 부모 meta 추가는 허용하지 않는다. Library/Logs/Temp/UserSettings의 통상 Unity 생성물은 소스568과 분리해 관측하며 삭제·초기화하지 않는다.

| 개수 | 정확 경로 |
| --- | --- |
| scene1 | `Assets/Scenes/Chapter1Playable.unity` |
| prefab2 | `Assets/Prefabs/Chapter1/Hub.prefab`, `Assets/Prefabs/Chapter1/OrdanEncounter.prefab` |
| prefab6 | `Assets/Prefabs/Chapter1/R01.prefab`, `R02.prefab`, `R03.prefab`, `R04.prefab`, `R05.prefab`, `R06.prefab` |

이들은 **구조유효·기본 이동 미수용 후보**다. 성공 exit0만으로9개 생성/검증·meta·실제 Build 완료를 추정하지 않는다. 실제 경로·내용·참조·구조 검수와 로그 근거를 원장에 기록한다. 기존 Build가 부여하지 않은 이동 수용값을 삽입하지 않는다. 부분 생성·예상 밖 추가/변경은 보존하고 그 시점에서 종료한다. 원래 r1의 전후568과 새 저장 재시도 구현 뒤의 최종568은 서로 다른 시점이며 같은 소스의 통제 비교라고 표현하지 않는다.

## 4. 실행 제어·원시 증거·실패 종료

**REQ-C1B-CTX-004:** 실행 제어 도구는 먼저 검토된 모든 권위·원장·독립 보고·최소 컴파일 결과 핀과 현재 소스를 검증한다. 실제 시작이 없으면 없다고 기록한다. 시작 직후 정확 실행파일·인수·시작UTC·PID·StartTicks(원장에는 정밀도 손실 없는 문자열)·심사 맥락·900초 단조 시간 한계를 남긴다. 폴링·도구 wait를 사용하더라도 부모가 직접900초를 관리하며 한 번의 도구 대기 시간을 실행 상한으로 오인하지 않는다. 장시간 대기는 짧게 나누고 로그 침묵만으로 멈추지 않는다.

실제 UPM 부모/자식/게시·연결, 컴파일 진입/완료·오류, executeMethod 시작/완료·후보 생성 증거를 가능한 원시에서 단계별로 기록한다. 짧게 살아 사라져 관측하지 못한 자식 PID/게시/연결은 미관측으로 남긴다. 다른 시도의 UPM 로그·종료된 IPC·과거 최소 컴파일을 이번 성공으로 대입하지 않는다. 실제 떠 있는 stdout/stderr를 읽어야 하면 r2에서 확인한 공유 읽기 방식을 적용한다. 전체 환경변수/인증 비밀은 출력하지 않는다.

**REQ-C1B-CTX-005:** 정상 종료·명시 오류·900초 상한·관리 예외 뒤에는 **자신이 시작하고 PID+StartTicks가 일치하는 Editor**만 종료/확인한다. 새 UPM 자식은 같은 실행의 정확 EXE·부모 계보·PID/StartTicks가 실제로 확인된 경우에만 그 소유 범위로 다룬다. 이름만으로 일괄 종료하거나 기존 Hub/라이선스·타 Editor/다른 서버를 종료하지 않는다. 종료 확인 최대5초·출력 회수 최대5초는 별도로 기록하며900초 실행 증액에 사용하지 않는다. 미종료/미확인 자식이나 회수 실패는 관리 실패로 기록하고 후속 제품 쓰기·실행을 중단한다. 소유가 불명확하면 추측 종료0이다.

원시 before/start/end/log/process 및 소스 after를 새 경로에 보존한다. 원래568의 변경·삭제와 정확18 신규 산출을 분리해 길이/SHA를 기록한다. 종료가 불명확하면 after를 안정된 최종 상태로 수용하지 않는다. 종료코드·timeout·관리오류·컴파일 오류·Build 완료·9개/meta 존재는 서로 독립 필드다.

| 관측 결과 | 판정과 다음 행동 |
| --- | --- |
| 도구 심사 거절 | 거절 행동·이유를 보존, 우회/다른 셸·다른 도구 재요청0. 별도 사용자 판단이 필요한 경우 정확 자동심사 거절을 설명한다. 이 계약이 거절을 무효화하지 않는다. |
| 실행 시작 불가/UPM 재실패 | 첫 시도 원시와 새 맥락을 보존하고 정지. 소스 컴파일 실패로 승격0, 같은 큰 실행 반복0 |
| UPM 통과 후 실제 컴파일 오류 | 정확 처음 오류/입력/어셈블리를 분류하고 최소 원인 조사로 인계. 후보 저작·다른 대형 검사0 |
| 컴파일 후 저작 실패/부분9 | 실제 Build 단계·부분 산출 원시 보존, 삭제/덮어쓰기/Build 재호출0 |
| exit0 및 구조유효9/meta | 루나 원시·산출 독립 대조 뒤 G1/G2 한정 결과만 아스트라 판정. 방 이동·시험·B1/CH1 수용0 |
| 상한/관리/종료·회수 실패 | 정확 소유 종료 시도와 미완료를 기록하고 중단. 성공 수치 대입·자동 재실행0 |

한도·도구 인증·모델 오류 첫회에도 멈추고 반복하지 않는다. 최소 IPC 비교의 권한 거부라는 **관측 결과**와 도구 실행 승인 거절은 별개로 기록한다.

## 5. 검사·추적·보존 관문

**REQ-C1B-CTX-006:** 본 후보는 기존 G3 실제 여섯 방/지원 A/B 수용→G4 원래31+승인4=35 단회 합산900초→G5 전체44 클래스 EditMode/PlayMode 각1회 합산1800초·종료/회수 각5초→G6 snapshot4×3 별도 수동12→G7 종단4·실제 창→G8 현재 소스 빌드 관문을 그대로 둔다. 기존9회·Q0 인간 승인·키·세대·프로필·원격 및 C1C의 B1 독립 최종 수용 이후 공유 통합 관문도 유지한다. 이번 G1/G2는 이들 검사·수용·완주를 포함하지 않는다.

| 수용 기준 | 확인할 내용 | 연결 |
| --- | --- | --- |
| AC-C1B-CTX-001 | 모든 독립 보고·최종568·쓰기 종료·영향 최소 컴파일 및 제어 도구 핀의 직접 대조, 미발급 근거로 시작0 | REQ-C1B-CTX-001 |
| AC-C1B-CTX-002 | 명시 도구 심사·같은6000.6·정확 입구·새1회 합산900초·금지 변경0, 이전 소진 이력 보존 | REQ-C1B-CTX-002 |
| AC-C1B-CTX-003 | 기존568 변경/삭제0, 후보9+.meta 정확18만 생성, before/after·구조·미수용 이동 지문 | REQ-C1B-CTX-003 |
| AC-C1B-CTX-004 | 실제 단계별 원시·PID/StartTicks·native exit·단조상한·미관측 구별, stdout 공유 읽기 보존 | REQ-C1B-CTX-004 |
| AC-C1B-CTX-005 | 자기 소유 종료/회수 각5초와 첫 실패 정지, 도구 거절 우회0·실패 결과/부분 산출 보존 | REQ-C1B-CTX-005 |
| AC-C1B-CTX-006 | G3→35→44→수동12·종단·빌드 및9회/Q0/키/세대/원격·C1C 관문 면제0 | REQ-C1B-CTX-006 |

기존 REQ-CH1-009/010, REQ-C1B-EVIDENCE-001/ROOM-001/HUB-001과 AC-CH1-006/007, AC-C1B-EVIDENCE-001/ROOM-001, AC-ROOM-001/004를 함께 추적한다. 현재 어떤 실제 AC도 통과로 기록하지 않는다.

이번 쓰기 소유는 본 새 Review와 `output/chapter1-playable-r1/preparation/b1-unity-context-reobservation-astra-2026-10-08.v1.json`뿐이다. 별도 C1C 정적 소스 제한 판정은 새 JSON 하나에 구분한다. 기존 계약·준비/로그·제품·스테이징 수정0, Unity·컴파일·시험·프로필·원격0이다. 독립 검수와 아스트라의 후속 명시 승인 전 새900초 권한은 발급되지 않는다.


## 6. 아스트라 한정 승인 및 실제 r6 선행 결속 — Approved

2026-10-08 아스트라 `gpt-6-astra`·`ultra`는 검수 원문 SHA `3450AB7549418997135051D8EB11D28472F940167E109EB6A0D074F8CF5786F7` 및 루나 `gpt-6-luna`·`max`의 규범 검토 v2 SHA `47D2B7F27C8F24F37785B846A242DF588CC83C6B74A28068F01A824FF884C1B4`를 직접 읽고 §1~5의 정확 범위를 한정 승인한다. 물리 IPC 비교 독립 보고 SHA `D6DA33AF4BBA2B1A7468D345857584C1D16EDC3260FED7DFB102146C03617F1F`는 실행 맥락별 접속 차이만 증명하며 실제 Editor 원인 확정이나 이번 실행 성공을 뜻하지 않는다. 원문 Review는 `output/chapter1-playable-r1/preparation/b1-unity-context-reviewed-contract.v1.md`에 같은 바이트로 보존한다. 이 승인에서 기존 상태행만 변경하고 본 절을 추가하며, 앞선 현재 미발급·후보·쓰기 소유 표시는 작성 당시 이력으로 보존한다.

승인 범위를 다음 세 가지로 구별한다. 첫째, 현재568 원장 SHA `A603D541D4D3C4BE782CFFB32D27BA5EB0974384291F11B41D9F6BF2D4F9AD41`의 정확 여섯 소스는 루나 델타 SHA `69625E47DE089714672F9296D02B847BC2C74B4460884004C1267C2529CD5F11`와 아스트라 정적 제한 수용 SHA `212F79D6CEC9AFF70BF66CB229732C8863018805F5050FBD737357A743ECC155`의 범위다. 둘째, 아래 r6 다섯 native 컴파일의 실제 결과를 그 검사에만 한정 수용한다. 셋째, 아직 수행하지 않은 실제 Unity의 새 실행 맥락 가져오기·저작 한 번을 승인한다. 이 세 범위는 실제 이동·저장 재시도 장치 시험·B1 최종·CH1 완주 수용으로 확대하지 않는다.

§2의 r5 최소 계획/결과 지칭은 당시 준비 이력이다. r5 실제 첫 Input.Unity는 종료했으나 종료코드가 null인 결과 SHA `3128A81F9028C7FE9BAF232777DDC6AB50BE5C223183FB1B435455F7F09C5F5F`를 남겼고 나머지4는 시작되지 않았다. 그 단회와 원문을 보존하며 DLL/경고로 성공을 소급하지 않는다. 최소 자체 자식 재현 SHA `955E92588B2747606E04F6AE5D6EFD764ECC83650D4EE5E67DA52B0E35DC3960`와 별도 r6 승인 SHA `755C87A1D536A999B30863EBF76FE1B1EE1E6EBA579F0F65BF8BDF21FF923AD7`를 거친 현재 실행 선행은 다음 정확 r6로 호환 결속한다. 기존 r5 본문·계획·결과나 원래 한정 준비 결정을 수정하지 않는다.

- 최소 도구 `output/chapter1-playable-r1/tools/b1-minimal-csc-r6-root.ps1`: SHA `89B18B52ECA21E395EBF2414945C1BA0BE01988DECE215D61DCDEA457AB3A5E8`.
- 최소 계획 `preparation/b1-minimal-csc-r6-plan-root.v3.json`: SHA `D8ED7D14476CF334B3E6D38A5ED5F1A2CE259D372AFC0D02D3756A5E1876DC53`; 루나 계획 검토 `evidence/b1-minimal-csc-r6-plan-review-luna6.v3.json`: SHA `8596AFF042FD2A2C3B27A0D46D99CB43A3CFE4112A60A7975F6F6D9152CFA508`, P0/P1=0/0.
- 실제 결과 `project/Library/b1-minimal-csc-r6-save-retry/result.json`: SHA `113336B69DCBF8A013EA38A4ABCCB4065C38CFEFFBCF4B1AE0EEC013B8D4CA43`; 루나 결과 검토 `evidence/b1-minimal-csc-r6-result-review-luna6.v1.json`: SHA `4E5C954240D23176F127B4AE506F0596C6C2D02C89CF41560A6FFA1F851D5C70`, P0/P1=0/0. 이 세 상대 경로의 기준은 `output/chapter1-playable-r1/`이다.

실제5는 Input.Unity→Run.Unity→Hub.Presentation.Unity→Combat.Authoring.Editor→Run.Unity.PlayMode.Tests 순서로 모두 정수 exit0·timeoutfalse·관리오류0·소유 프로세스 종료, 총10.7500141초였다. 기록된 DLL/ref10개와 추가 PDB5개를 구별한다. 기존568의 전후 원문 SHA는 모두 `6CBA43EB2B50905275F6EFB7DE4A6E4D7AE1514573E326D5C74172C6442EE687`이며 현재568/입력500/자원10도 다시 대조했다. 미변경 읽기 입력의 CS0414 경고1은 차단 오류가 아니며 제품 수정 권한을 만들지 않는다. 원시 result의 accepted=false는 보존하고 이 절에서만 최소 컴파일 한정 수용을 기록한다. 추가 native 컴파일·재호출 권한은0이다.

새 Unity 도구는 `output/chapter1-playable-r1/tools/b1-unity-authoring-context-r2-root.ps1` SHA `13434CE716B56E3CD13E4098F6C689C0B9862973E7E630206E16571CA600CD70`, 솔 준비 v4 SHA `12685528D5E5E37AFD8038354545822676F7EA8B4FCC72DA32F4DFB5C4036DE1`, 루나 최종 도구 검토 v4 SHA `D6BF4896A778C5CF69D22194317ACDA2C87077769C46420D4D7FB4743F308B5B`의 P0/P1=0/0에 결속한다. 독립 도구 검토는 r6 v3 경로·명명된 SHA 연결, 정확 Hub 경로, 즉시 handle 확보·정수 종료코드, 원시/소유 종료 경계를 확인했다. 당시 최소 결과 미완료 표시는 위 실제 결과/독립 보고로 현재 해소하며 과거 보고 원문을 바꾸지 않는다.

Root만 정확20 입력 인계 원장을 만들고, 실제 현재 Approved 계약/주 계약 SHA와 위 모든 실제 증거를 연결하여 검수된 도구의 정식 `require_escalated` 자동심사에 요청한다. 아스트라 실행 승인 `preparation/b1-unity-context-reobservation-execution-approval-astra.v1.json`은 `reviewedContractSha256`와 미래가 아닌 실제 Approved `recoveryContractSha256`를 구별하고, 지정19개 `boundPinSha256` 및 최상위7개 SHA를 각각 직접 결속한다. 이 승인 기록의 기준 경로 역시 `output/chapter1-playable-r1/`이다. 자기지문 순환 결속이나 설명 속 SHA 검색은 사용하지 않는다. Root는 실행 직전 현재568·쓰기 종료·9후보/meta18 부재·새 원시8경로·충돌 실행 폐쇄·단일 소유를 다시 확인한다.

허용은 동일 Unity6000.6.0f1의 §3 Build 입구로 가져오기·자체 컴파일·scene1/prefab8 저작을 **새1회·합산900초** 수행하는 범위뿐이다. 종료 확인5초·출력 회수5초, 첫 실패 중단, 원래568 변경/삭제0, 정확9후보와.meta18 외 소스 추가0, 실제 단계/원시/종료/후보의 독립 대조를 유지한다. 성공 exit0만으로 구조나 기본 이동을 수용하지 않는다. 기존 r1 소진은 그대로이며 G1/G2 분할·반복·별도 UPM 서버·IPC 주입·패키지 우회·보안/프록시/설치/캐시/인증 조작0이다. 정식 도구 심사 거절은 우회하지 않는다.

REQ-C1B-CTX-001~006과 기존 REQ-CH1-009/010, AC-C1B-CTX-001~006·AC-CH1-006/007의 연결을 채택한다. 실제 G3→집중35 합산900초→전체44 두 모드 합산1800초→수동12→종단4/실제 창→현소스빌드, 기존9회·Q0·프로필·키/세대·원격 및 B1 독립 최종 이후 C1C 공유 통합 관문을 보존한다. 승인 작성자의 실제 Unity/native/시험/제품/스테이징/프로필/원격 실행은0이며, 이번 실행 후에도 루나 원시·정확 산출 독립 검수와 아스트라 G1/G2 한정 판정이 별도다.
