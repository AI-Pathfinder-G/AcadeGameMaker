# B1 저작 장면 순서·로그 직렬화의 한정 복구

- 상태: **Approved** — §7의 제품1파일·도구/검사 계획 준비만 한정 승인한다. 실제 native/Unity 실행은 별도 승인 전0.
- 사용자 동작: 검증 조건을 유지한 CH1 방 후보를 생성해 실제 기본 이동 검수의 선행을 복구한다. 실제 이동·B1 최종·CH1 완주 수용은 별도다.
- 담당: 아스트라 `gpt-6-astra/ultra` 설계·승인, 솔 `gpt-6.1-sol` 정확1파일 구현, 루나 `gpt-6-luna` 독립 규범·변경·도구·결과 검수, Root 단일 실행.
- 이번 쓰기·종료: 이 Review와 `output/chapter1-playable-r1/preparation/b1-authoring-scene-and-log-recovery-astra.v1.json`만 작성하고 SHA를 Root에 반환한다. 기존 권위·원시·제품·스테이징 쓰기 및 native/Unity/프로필/원격 실행0.

## 1. 근거와 새 관측 목적

이전 [회복 계약 §6](./2026-10-08-b1-unity-context-reobservation.md) SHA `7C526D2169EA065D1663B948F75D65EF3C1FE8A72C49584F5E0D3A371EAA5C7A`, [주 계약 §28](./2026-10-03-chapter1-playable-delivery.md) SHA `BCD60C45866854C1DA7D1903607DB981E063104900D2A1DC7D763B51C109E152`, 소비된 실행 승인 SHA `147D40A0D14C458117DDEC50CAAAC6E1E5394212B7D182370BEB2B84635C357A`를 보존한다. 이전900초와 r1/r2 단회 이력을 초기화하지 않는다. 아래 상대 경로의 기준은 `output/chapter1-playable-r1/`이다.

| 직접 읽은 근거 | 실제 SHA·한계 |
| --- | --- |
| `preparation/b1-authoring-named-scene-cause-sol61.v1.json` | 최종 쓰기 종료본 `7606C69CF0B7BA097335AE6DFEC09A8C026022E8DD8140E0D7D64E10FCBC4536`. 정확 Build 한 줄 이동 후보, 현재568 불일치0. 작성 중간 지문은 승인 입력이 아니다. |
| `evidence/b1-authoring-context-r2-first-failure-root.v1.json` | `06F8652677E85D6DE3EE8BF7B65F3601FFCB99E8435034BBCA90AC40A3D60983`. 시작·원시7·전후568 불변·후보18 부재. |
| `evidence/b1-authoring-context-r2-failure-review-luna6.v1.json` | `216322C5BDA0394C61ED3D1C3480E694A5870ADFBC5F981B0B471C9F3CE82E3F`. 실제 루나/max, P0=0/P1=1. P1-001은 종료 원장 직렬화 결함이며 새 도구 독립 검수 전 미폐쇄다. |
| `evidence/minimum-log-match-serialization-probe-root.v2.json` | `5E70A0A4A19DEA3EE5AE4FA2050736B5FD14C44F3DC8DFCA8A1115F2BABC01FD`. 실제 PowerShell5.1에서 원래 변수명 직렬화 실패·분리 변수명 성공, native0. |
| `preparation/b1-unity-log-correction-candidate-root.v1.ps1` | `2BE4A41882CFFE8C910727C683740D3F0EC19C8251890AFB6B748FFBBA62A942`. 기존 도구13434…CD70의 `$matches→$stageRows` 정확4리터럴 치환만 직접 확인. r2 경로/핀은 남아 있어 실행용이 아니다. |

r2는 실제 UPM 연결0.1초와 스크립트 컴파일 단계를 지나 Build:75에서 `CombatTarget.ValidateForAuthoring`의 named scene/hierarchy 예외로 끝났다. Editor 로그의 return code1, 호출 도구 exit1, **end.json 부재로 회수되지 않은 native exit=null**을 구별한다. 원시 값이나 종료 원장을 사후 복원하지 않는다. 실패 시 scene.name 문자열 자체의 계측값은 없으며, 호출 순서·기존 복제/계층 규칙으로 원인을 좁힌 정적 판정이다.

새 목적은 **기존 명명 장면을 첫 복제 검증 전에 준비한 동일 Build가 모든 기존 검증과 후보9/meta9 저작을 완료하는지, 보정된 로그 수집이 실제 종료 원장을 보존하는지 한 번 관측**하는 것이다. 기존 IPC 진단이나 전체5 최소 컴파일을 반복하는 목적이 아니다.

## 2. 정확 제품1파일 후보

**REQ-C1B-ASL-001:** 기존568 중 `Assets/AcadeGameMaker/Editor/CombatAuthoring/Chapter1ExpeditionAuthoringBuilder.cs`의 `Build`만 변경 후보로 한다. 제품 상대 루트는 `project/`다. 변경 전14033바이트·SHA `2D9B6845840A4119D74EF41072E07F04D06C120C0A808A7628E90933E5CBEEF2`를 확인한다.

기존78행의 다음 한 줄을 원래 위치에서 제거하고,74행의 regular/Ordan/font null 검사 뒤이자75행의 `Chapter1RegularEncounterAuthoringValidator.Validate(regular)` 바로 앞으로 이동한다.

```csharp
var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);scene.name="Chapter1ExpeditionAuthoring";
```

장면 생성은 여전히 한 번이며 같은 지역변수를 최종 SaveScene에 쓴다. 이후 계획4·BuildRoom6·보스·Hub·scene의 구성/검사/정확 저장 및 finally 파기는 그대로다. 재배치 뒤 입력 검증 실패가 생기면 미저장 임시 장면만 남을 수 있으며 이를 저장하지 않고 종료한다.

나머지567의 경로/길이/SHA, 원본 regular prefab/meta의 기존 보존 SHA, Ordan·계획·폰트, CombatTarget/TransferTarget/TransferLineOfSight와 모든 validator를 보존한다. 가드 완화·예외 삼키기·원본 재저장·SaveAssets·부수 줄바꿈 정규화·신규 C#/API/friend/asmdef·런타임/입력/저장/프로필 변경0이다. Review/AcceptBasicMovement 입구도 변경하지 않는다. Approved 뒤 솔 단일 쓰기→새568(v5 덮어쓰기0)·정확1파일 차분·567불변·쓰기 종료→루나 독립 델타→아스트라 한정 소스 판정 순이다.

## 3. 로그·실행 제어의 한정 호환보정

**REQ-C1B-ASL-002:** 기존 r2 도구와 위4치환 준비 후보를 보존하고 Root가 별도 r3 실행 도구를 준비한다. 동작 보정은 DrainLog 수집 변수 네 곳의 이름 분리다. 같은 정규식·원시 행번호/100행 상한·SHA·공유 읽기·단조상한·종료/회수 방식은 유지한다. 정식 실행 전에 순수 자료의 JSON 왕복과 관리 실패 기록까지 독립 검수하여 P1-001을 닫는다.

필요한 나머지 변경은 새 helper/계획/실행 인계·새 원시8경로·현재 입력/승인 핀·아래 최소1 결과 구조의 결속뿐이다. r2 경로 재사용·부분 산출 삭제·과거 end.json 생성0. 정확 경로와 하위 필드는 Root의 새 도구 계획에서 고정하고 독립 검수 전 실행 승인0이다.

기존 **정확20 핵심 입력 이름, 승인 `boundPinSha256` 정확19(자기 승인 제외), 최상위7 SHA 직접 비교**를 유지한다. `minimumPlan/Result`와 그 검수는 새 Editor1 결과를 가리킨다. r6 실제5·미변경 참조는 새 최소 계획의 명명된 재사용 근거로 별도 검증한다. 기존 `results.Count=5`를 그대로 요구하거나 과거4행을 새 결과에 복사해 다섯 새 실행으로 세지 않는다.

새 helperPlan은 솔 원인·r2 Root/루나 실패 보고·순수 재현·이전 권위를 각각 실제 path/SHA의 명명된 근거로 결속하고 도구가 연결 필드를 직접 검증한다. 새 sourceApproval은 현재 checkpoint/sourceDelta와 이전 v5 `A603D541D4D3C4BE782CFFB32D27BA5EB0974384291F11B41D9F6BF2D4F9AD41`·여섯 소스 독립 검수 `69625E47DE089714672F9296D02B847BC2C74B4460884004C1267C2529CD5F11`·한정 수용 `212F79D6CEC9AFF70BF66CB229732C8863018805F5050FBD737357A743ECC155`의 계보를 구별한다.

검수받은 Review SHA와 미래 Approved SHA를 분리한다. 승인 전이 시 검수 원문과 기존 master §28 접두사를 보존하고 상태/명시 승인 절 외 본문이 동일한지 검증한다. 설명 속 임의 SHA 검색·null 근거·자기지문 순환으로 명명된 결속을 대체하지 않는다. 즉시 process handle·정확 PID/StartTicks/EXE/부모 계보·정수 native exit·Root 실제 도구 심사 결과는 기존 §6대로 유지한다.

## 4. 실제 선행과 최소1 검사

**REQ-C1B-ASL-003:** 이 규범의 독립 검수·아스트라 명시 승인 뒤에만 §2 제품1파일 구현과 한정 도구 준비로 진행한다. r2 실패 보고를 성공 보고로 바꾸지 않으며 새 규범/도구의 미해결 범위·관리 지적은 닫아야 한다. 최소 검사와 Unity 실행은 각 실제 선행·정확 별도 실행 승인 뒤다.

변경 조립체는 `AcadeGameMaker.Combat.Authoring.Editor` 하나다. 최소 후보는 Root **native1회·60초·종료5초/회수5초**, 첫 실패 중단이다. r6 실제5 결과 `113336B69DCBF8A013EA38A4ABCCB4065C38CFEFFBCF4B1AE0EEC013B8D4CA43`와 독립 보고 `4E5C954240D23176F127B4AE506F0596C6C2D02C89CF41560A6FFA1F851D5C70`는 역사 및 미변경 참조 재사용 근거다. 전체5/추가4 재컴파일0이다.

r6 Editor rsp `BA09CAA868998FC9D761957FCD52AC61780F4BFCF2D7A3CDBDD54DDC86FE35F9`의 기존 소스 선택·정의·참조·분석기·자원을 유지하고 새 Library r7의 out/refout **두 경로만** 바꾼다. 그 한 소스의 새 바이트, 실제 참조 DLL/ref·컴파일러/자원·출력 계획을 경로/길이/SHA로 고정하고 루나가 영향·재사용을 대조한다. 기존 RSP/산출물 덮어쓰기·공유 compiler server0이다. 입력 차이로 재사용이 성립하지 않으면 확대 실행 전에 정확 영향부터 다시 판정한다.

실제 실행 순서는 **새568/독립 델타·소스 한정 수용→최소1 계획/도구/입력 독립 검토·실행 승인→실제1 결과·루나 독립 검수→새 Unity 도구/핀 검수와 P1 폐쇄→아스트라 정확 Unity 실행 승인·Root 단일 소유 인계**다. 준비·결과가 미발급이면 null로 남기며 이전 최소5나 소스 수용을 새 결과로 사용하지 않는다.

## 5. 새 저작 단회와 종료

**REQ-C1B-ASL-004:** 위 선행 충족 뒤 Root만 정식 `require_escalated` 심사를 거쳐 같은 Unity6000.6.0f1의 `AcadeGameMaker.Combat.Authoring.Chapter1ExpeditionAuthoringBuilder.Build`를 **새1회·가져오기/자체 컴파일/저작 합산900초** 실행하는 후보로 한다. 별도 저작900초·같은 호출 재사용·실패 후 자동 반복0이다. 심사 거절은 우회하지 않는다.

실행 직전 새568 전수 지문·sourceWritingClosed·다른 writer/runner0·Editor/UPM 폐쇄·기존 부모 meta·정확 후보18 부재를 확인한다. 실행 동안 그568 변경/삭제0이다. 추가 가능 파일은 기존 `Assets/Scenes/Chapter1Playable.unity`, `Assets/Prefabs/Chapter1/Hub.prefab`, `OrdanEncounter.prefab`, `R01.prefab~R06.prefab`의9개와 각.meta9뿐이다. 구조와 미수용 상태를 확인하며 이동 수용 SHA 주입0. r2→새568의 승인1파일 차분과 실행 전후568 불변, 후보18 추가를 분리한다.

**REQ-C1B-ASL-005:** 새 시작/종료·전후 소스·프로세스·Editor/stdout/stderr 원시8개에 실제 단계·단조시간·소유·도구 승인·native 종료를 기록한다. 정수 exit0만으로 구조/정확18/568보존을 수용하지 않는다. 로그상 return code·도구 exit·native exit를 분리하고 종료 원장 부재/null을 성공 수치로 채우지 않는다.

첫 실패·관리 오류·900초 상한에서 정확 소유 프로세스만 종료5초, 출력 회수5초로 정리한다. Hub나 소유 불명 프로세스의 추측 종료0, 무로그만으로 정지 판정0이다. 부분 후보·원시를 보존하며 삭제/덮어쓰기/Build 재호출0. 새 예외는 추가 큰 검사 전에 최소 원인·새 관측 목적부터 판정한다.

이전 회복 §6의 보안/ACL/프록시 변경·패키지 우회·별도 UPM/IPC 주입·캐시 삭제·설치·자격증명 조작 금지를 그대로 적용한다. 프로필/입력/저장/런타임·실제 창·원격 작업0이며 한도/인증/모델 오류도 첫회 중단·반복0이다.

## 6. REQ/AC·남는 관문

| 수용 기준 | 연결 요구·검사 |
| --- | --- |
| AC-C1B-ASL-001 | REQ-C1B-ASL-001: builder1 한 줄 이동·567불변·원본/meta/validator/저장 입구 보존 |
| AC-C1B-ASL-002 | REQ-C1B-ASL-002: 네 치환·새 정확20/19+7·최소1/역사5 분리·직렬화/관리 원시·P1 폐쇄 |
| AC-C1B-ASL-003 | REQ-C1B-ASL-003: 새568·쓰기 종료·독립 델타·최소1 계획/60초/실제 결과·각 독립 검수 |
| AC-C1B-ASL-004 | REQ-C1B-ASL-004: 정식 심사·Root1회900초·같은 Build·현재568불변·후보18 구조/이동 미수용 |
| AC-C1B-ASL-005 | REQ-C1B-ASL-005: 실제 종료·원시8·종료5/회수5·첫 실패·부분 산출/이전 소진 보존 |
| AC-C1B-ASL-006 | REQ-C1B-ASL-006: 아래 후속 실제 검사·인간 승인·통합 관문 유지 |

**REQ-C1B-ASL-006:** 기존 [ORDER §5](./2026-10-07-b1-runtime-gate-order.md)·master §26~28을 유지한다. G3 실제6방/지원10구성 수용→집중35 단1회900초→전체44 EditMode/PlayMode 각각1회 합산1800초→별도 snapshot4×3 수동12·회당1800초→정상1/실패3 종단 각1800초·합산7200초와 별도 실제 창 키보드/포인터 각1800초→현재 소스 빌드1회900초를 면제·축소·증액하지 않는다.

기존 REQ-C1B-ROOM-001/EVIDENCE-001, REQ-CH1-009/010, CTX-001~006와 대응 AC 및 AC-ROOM-001/004·AC-CH1-006/007을 유지한다. 필수9회·Q0 인간 승인·키/세대·프로필·원격 관문도 그대로다. **B1 실제 독립 FINAL 전 C1C 공유 통합0**이며 최소1·저작 성공으로 FINAL을 대신하지 않는다. C1C 미결5 계약의 승인도0이다.

솔 최종 원인7606과 루나 r2 실패216322는 실제 도착했다. **새 규범 검수, 제품1파일/새568·독립 델타·소스 수용, 최소1 계획/결과/각 검수, 새 도구/계획/검수·실행 승인은 아직 미발급**이다. 본 Review의 실제 AC 통과·제품/스테이징/native/Unity/시험/프로필/원격 실행은 모두0이다.

## 7. 제품1파일·도구/검사 계획 준비의 한정 승인 — 2026-10-08

아스트라 gpt-6-astra/ultra는 검수 원문 SHA 1D2E70D1FA8FE2277F31A2E1E88308AB6E38EBFBFD904BFB8481C5F344906A36, 준비 원장 SHA 4E07249F4A9E5116E0B429252D063C2DF3ED71E145E1670DFBF286AC72E25463와 실제 루나 gpt-6-luna/max 독립 규범 보고 SHA E248F163A4E0806FB503B25B1B3A8130BE951C6158157F8C38926C4F6B699E39의 신규 P0/P1=0/0을 직접 대조하여 아래 준비 범위만 최종 승인한다. 검수 원문은 output/chapter1-playable-r1/preparation/b1-authoring-scene-and-log-reviewed-contract.v1.md에 동일 바이트로 보존한다. 상태줄 외 §§1~6 원문을 보존하며 당시 미발급·Review·쓰기 범위 표시는 역사다. 이번 명시 승인은 그중 규범 및 아래 구현/준비 선행만 해소한다.

**허용하는 제품 변경은 §2의 기존 Builder 한 문장 이동1뿐**이다. 현재568 원장 A603D541D4D3C4BE782CFFB32D27BA5EB0974384291F11B41D9F6BF2D4F9AD41와 변경 전 Builder 2D9B6845840A4119D74EF41072E07F04D06C120C0A808A7628E90933E5CBEEF2를 기준으로 Root가 솔 단일 쓰기 소유를 정확 인계한 뒤 구현한다. 신규 파일/다른567/원본 graph·meta/validator/런타임·입력·저장·프로필·API/friend/메뉴/Review·Accept 입구는 변경하지 않는다. 솔 최종 원인 7606C69CF0B7BA097335AE6DFEC09A8C026022E8DD8140E0D7D64E10FCBC4536만 사용한다. 구현 뒤 새568·쓰기 종료·정확1파일 차분·루나 독립 델타·아스트라 한정 소스 판정이 필요하며 이전 v5/여섯 소스 수용 원문은 덮지 않는다.

Root는 §3~4의 새 로그 도구·핀/원시 계획과 **Editor1 최소 검사 계획을 준비할 수 있다**. 실제 native 실행은 이 승인 범위에 없다. 솔 영향 원장 EF19562D844B26F331CBF2E1DC8929FEE56AF3F77F31AA3A87AAF8F8E9629D6E의 고유 입력329·소스12·참조 지시문318·분석기7 및 고정 자원10은 준비 근거이며 새 계획/실제 입력의 독립 검수를 대체하지 않는다. r6 응답의 out/refout 두 경로만 새 r7로 바꾸는 후보를 유지한다. 실제 r6 다섯 결과와 새 Editor1을 분리하며 전체5/추가4 반복0이다. 직접 소비자 AcadeGameMaker.Combat.Unity.EditMode.Tests는 그 재사용4에 포함되지 않으므로 변경 후 그 소비자의 컴파일·시험 수용을 발급하지 않는다. 필수 G4/G5는 유지한다.

r2 실패 원시7·end.json 부재·native exit=null·도구 exit1/로그 return1·기존 단회 소진은 그대로다. r2 독립 보고 216322C5BDA0394C61ED3D1C3480E694A5870ADFBC5F981B0B471C9F3CE82E3F의 **P1-001은 미폐쇄**다. 순수 재현 5E70A0A4A19DEA3EE5AE4FA2050736B5FD14C44F3DC8DFCA8A1115F2BABC01FD와 네 치환 후보만으로 폐쇄하지 않으며, 새 helper의 직렬화·관리 종료·정확20/19+7 명명 결속을 루나가 독립 검수해야 한다.

**현재 컴파일/Unity 실행 권한은0**이다. 최소1은 새 소스 수용·정확 계획/입력/도구 독립 검수 뒤 별도 아스트라 실행 승인으로만 Root1회·60초·종료5/회수5를 허용할 수 있다. 그 실제 결과와 독립 검수, 새 Unity 도구 검수/P1 폐쇄 뒤 별도 실행 승인·정식 도구 심사를 거쳐야 §5의 새1회·합산900초 후보가 실행 가능해진다. 미래 checkpoint/최소 결과/helper 검수 SHA는 아직 null이며 이전 승인이나 본 finalApproval=true를 실행 인자로 사용하지 않는다.

REQ/AC-C1B-ASL-001~006과 §6의 G3→35→44→수동12→종단/실제 창→현재 빌드, 필수9회·Q0 인간 승인·키/세대·프로필·원격 및 B1 실제 독립 FINAL 전 C1C 공유 통합0을 모두 보존한다. 이 판정은 실행 AC 통과·B1 FINAL·CH1 수용이 아니다. 승인 원장은 output/chapter1-playable-r1/preparation/b1-authoring-scene-and-log-preparation-approval-astra.v1.json이며 Root가 제품1파일 소유 인계와 별도 도구 준비를 분리한다. 이 승인 작성자의 제품·스테이징·GUI·native/Unity·시험·프로필·원격 실행0으로 종료한다.
