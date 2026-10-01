# C3·C4 v15 9회 실행 최종 독립 검수

- 최종 큐 계획: `artifacts/c4-final-validation-queue-plan-v15.json`, SHA-256 `43125A3558F6B1F3FDDC595FB47F602883B4861D0EF24690F1FEE27083EC9082`.
- 동결 원장/입력: v19 manifest `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`, 193개 파일; v19 입력 목록 `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`, 1084개 경로.
- 최종 큐 결과: `artifacts/c4-final-validation-queue-result-v13.json`, SHA-256 `56F3931E7128218F1E4A57AA22CBDC9D6852A2F3E1354F78E7BEAEAF8D3C830A`; 실행 비교 성공, 동일 입력, `WholeAccepted=false`.
- 판정: 실행 증거 정합성 P0 0건/P1 0건. 이는 독립 증거 검토이며 아스트라의 수용 결정을 대체하지 않는다.

## 실제 실행 대조

아홉 원시 XML 각각의 모든 `test-case` 정식 이름을 계획의 선택 원장과 대소문자 구분하여 비교했다. 아홉 실행 모두 예상 개수와 일치하고, 누락·초과·중복·비통과 항목은 0이다. 각 실행의 native 종료 관측, QA 반환, 바깥 실행 종료는 모두 0이며 verification은 `Verified=true`다. 각 실행 전후 입력 캡처는 각각 1084개 경로이고, 경로별 SHA 대응 차이는 0이다. v19 manifest에 든 193개 파일도 실제 경로와 SHA를 전부 재계산해 불일치 0건을 확인했다.

| 계획 선택 | XML 시험 | C4 행 대조 | C3 원장 대조 | 종료/판정 |
| --- | ---: | --- | --- | --- |
| `c4-r11-focused-edit` | 139/139 통과 | 148 계획·도달·통과, 누락·초과·잘못된 ID 0, 증거 일치 | — | native/QA/바깥 0, Verified |
| `c4-r5-focused-play` | 32/32 통과 | 35 계획·도달·통과, 누락·초과·잘못된 ID 0, 증거 일치 | — | native/QA/바깥 0, Verified |
| `c4-r3-focused-hub` | 5/5 통과 | 5 계획·도달·통과, 누락·초과·잘못된 ID 0, 증거 일치 | — | native/QA/바깥 0, Verified |
| `c4-r3-edit-matrix91` | 91/91 통과 | 해당 선택의 C4 행 없음 | 기존 C3 내부 377행, 계획·통과·일치 377, 실패·미상·누락 0 | native/QA/바깥 0, Verified |
| `c4-r3-edit-remaining149` | 149/149 통과 | 0행 선택. 누락·예상 밖·잘못된 ID 0, 증거 일치 | — | native/QA/바깥 0, Verified |
| `c4-r2-play15` | 15/15 통과 | 0행 선택. 예상 밖·잘못된 ID 0, 증거 일치; 승인된 두 C5 원문 진단 경계 포함 | — | native/QA/바깥 0, Verified |
| `c4-edit562` | 562/562 통과 | 0행 선택. 누락·예상 밖·잘못된 ID 0, 증거 일치 | 필수 회귀 묶음 | native/QA/바깥 0, Verified |
| `c4-worker51` | 51/51 통과 | 0행 선택. 누락·예상 밖·잘못된 ID 0, 증거 일치 | 필수 작업자 묶음 | native/QA/바깥 0, Verified |
| `c4-play610` | 610/610 통과 | 0행 선택. 누락·예상 밖·잘못된 ID 0, 증거 일치 | 필수 실행 모드 묶음 | native/QA/바깥 0, Verified |

각 선택은 서로 일부 겹치는 회귀 실행이다. 위 시험 수의 합은 실행 건수 합이지 고유 시험 수가 아니다. C4 checkpoint 원장 v9 `artifacts/c4-required-checkpoint-rows-v9.json` SHA-256 `2769FA04AF42317D116C9C28D26EDA7948B58ECB598778EC9FE204538EB07EDE`의 188행은 첫 세 선택의 148+35+5 행 대조에서 모두 증거 일치로 통과했다. 각 행에는 `REQ-M5D7QC4-*` 및 `AC-M5D7QC4-*` 연결이 보존되어 있다. 승인 기준별 행 수는 서로 중첩되므로 합산해 188을 다시 세는 방식으로 해석하지 않는다.

C3 행렬 결과 `artifacts/c3-r11-edit-matrix-r1-decision-row-comparison.json` SHA-256 `B5FC981638BDF0CD91F3723462380BFC3FBC701D65376DD27AC32236D0B02B74`는 XML `c4-r3-edit-matrix91.xml`과 기존 C3 기대 원장 SHA `D1B9CBDA4AF477B5C10FC14893F0C58B2BAE676A38E15987C87B7D918658C9C6`에 결속되어 있다. 결과는 377/377 계획·통과·일치, 실패 0, 미상 0, 증거 일치다. 377은 91개 NUnit 사례에 속한 내부 행 수이며 독립 시험 수가 아니다.

각 실행의 원시 파일·verification 지문은 계획 v15의 실행 항목과 일치한다. XML/verification의 SHA-256은 다음과 같다.

| 실행 | verification SHA-256 | XML SHA-256 | 행 비교 SHA-256 |
| --- | --- | --- | --- |
| `c4-r11-focused-edit` | `B4F3429855B937198ACFC243C584BA2A2A0ED57B7AA620F7014C1EF6C23FBB23` | `114AC193B4ECF7BFA4CB88883018587D67F6EC41CE1093137602DE9A413A86E9` | `4502D1CAD2019AB28E636755385970A35E30FEA82A0C7D625805D04C147DB2E9` |
| `c4-r5-focused-play` | `7EA0C5A051404D7F17E07AD6A8F34B3923DD27CDA4DAA26E92940F55C8D891E2` | `E9427463A22E42E87FD567E0D835F4BD4A5C5BB7E2BC7804D91F4EA697F755EB` | `9B721A2AE90514CB25C27DEE4BE8B1FB61E7788216D4543C2F0486F4E990127B` |
| `c4-r3-focused-hub` | `A00F15128E9F7C556D8DFA870EA57A61B67790C789B19B59D4193CB6A5FE001C` | `6A4B0B47D834AC39D42B7DD495109DFA3B7C9A8F71140D0C02721929CF429502` | `4E8F8871B0BBDCB352EFBCDCC61C1986C534E2D1FA65C15994902DB201A0EC72` |
| `c4-r3-edit-matrix91` | `BA9CA6F5E0DEE14FA34C6E18084AAE314395624E92CD11C1590DFCD6D821C985` | `99DC1262B9FB163BDC9B325F50C2162EDBDB9F1E8E4C6D6D699BA985C817660B` | `953F7A0C30F938ADB7D56516CD82FE945B625A557F4A0151A3C9C683FB236F09` |
| `c4-r3-edit-remaining149` | `A9D242DC7E3BD003DCC630B5BF9797923BE390CE4401E321930DDE6236D1EE0F` | `55E8DC006BC00D41A55CD1FF58C8898E5AB853DB5DB5D6B0D592CE025103EA57` | `45EFF64BB486C0C02BAB12BCD4BD7A059DF9A00413F34A8C6A4129132D25B7DC` |
| `c4-r2-play15` | `A110176063AFCF632B85F56C77420B65FED1451BF227D239F289ADB97E258300` | `85DFF326B421C2660FD7A931FEF1E644D45A67D3A598680CC2B6E17318E7A490` | `56FE1752E5A6E8C4AAEEF2A69F68F5F61063A242E664B459D1FBF2D58DDDC4E9` |
| `c4-edit562` | `0AD870B9538995129577EFEDE3500845EB13707F48A22AC1C07CA22583E8710E` | `ADF033B4069CDFF51467332D8A7504BA7FB4A53C3E9DBE594DCFAF3033A00ECA` | `1E529BD72FB23587AF50F97A6F4E69BFA91D7B27B259D8FA85C78929C92F646F` |
| `c4-worker51` | `DA76943D3BCF9C8E4D83EF8C3A53743805E084DA6735AD7FC050FB0C09CF39D4` | `647B347808BCDD13B8C6C8C99FD6391C771912815CB2FEA07E0D1F6269E141EF` | `90618B4C51E5AF3320ABA408F4CE541CF9852B5B2B622385714B1BDFC4B1695A` |
| `c4-play610` | `EDF2BD762CA68FB8F516C2447953FF620A0EC8C96A81DDC4867EFA699E15B75A` | `CA0D13AC4EB2C48DB344B4B55B82DE8CC151196BEE7FDC892F22B192638ADE7F` | `C76A392E947108C90DDEB7F93D2C7E9C8E24B9F1A3A4FBC7C4D1766D2BA9728B` |

## 기준별 판정과 한계

**C3 `AC-M5D7QC3-007/008`.** 규범 `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c3-confirmation-owner.md`는 두 기준을 전체 `Open / Not Verified`로 유지한다. 이는 v15의 관련 실행 부재를 뜻하지 않는다. 실제 v15 XML에는 C3 소유자의 Busy 보존 사례 1개와 커밋·종료 권한 폐쇄 사례 4개가 포함되어 모두 통과했다. 또한 91개 행렬 사례의 377 내부 행이 일치했다. 이는 유효한 부분·공동 최종 검증 증거지만, 이 검토만으로 규범 상태를 자동 변경하지 않는다. 두 기준의 각 의미 범위 전체를 충족했는지 Astra가 기존 계약과 통합 증거에 따라 명시적으로 판정해야 한다.

**C4 `AC-M5D7QC4-001..010`.** 정확한 선택·필수 시험은 9개 실행 모두 통과했고, 188개 C4 행과 C5 두 진단 경계를 확인했다. 특히 `AC-M5D7QC4-009` 정적/API 범위는 Unity 통과와 구분했다. 선행 정적 검토 `docs/verification/2026-10-01-c4-v19-core-static-luna-review.md` SHA-256 `84FDEE13D70A609A110E04ECD9C79C71F96FFD242073F008FD2733EB870073C7`은 현재 v19의 핵심 런타임 5개 파일을 검토했다. 나머지 3개 핵심 파일은 `docs/verification/2026-09-30-c4-runtime-eight-luna-prereview.md` SHA-256 `7B7731B4F887A4C2DBFDE26FF0CA7543390B05CFCAB80161571D7988BCA8FFB9`에서 검토됐으며 v19의 실제 지문이 그 보고서 지문과 동일하다. 이 조합은 AC-009의 정적 경계 검토 자료지만, 본 문서는 이를 재수행하거나 전체 통합 정적 수용으로 확대하지 않는다.

아스트라가 공동 최종 수용을 진행해도 된다고 권고한다. 근거는 아홉 계획 실행의 실제 PASS와 일치하는 입력·동결·행 증거, 그리고 현재 핵심 런타임의 정적 검토 자료다. 다만 최종 수용 기록은 `AC-M5D7QC3-007/008`의 Open 상태를 명시적으로 처리하고, C3와 C4를 함께 판정해야 한다. 큐 결과 자체도 `WholeAccepted=false`이며, 이 문서는 이를 수용으로 바꾸지 않는다. Unity/컴파일 외의 깨끗한 복제·게시 또는 다른 범위의 승인은 이 실행으로 증명되지 않는다.

새 문서 경로는 작성 전 존재하지 않았고 v19 입력 목록 1084개 경로에도 없었다. 검토는 읽기 전용이었으며 기존 실행 결과·원장·소스는 수정하거나 재실행하지 않았다.
