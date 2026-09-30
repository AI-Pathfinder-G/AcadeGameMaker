# C4 작업자 51건 실행 결과의 독립 검토

대상은 v15 큐의 `c4-worker51` 부분 실행이다. 범위는 `AC-M5D7QC4-010` 및 검증 보고의 `AC-M5D7QC3-007/008` 추적에 한정하며, 전체 C4 수용을 판정하지 않는다.

- 계획 `artifacts/c4-final-validation-queue-plan-v15.json`: `43125A3558F6B1F3FDDC595FB47F602883B4861D0EF24690F1FEE27083EC9082`에 고정된 `Worker51` 선택은 51개다. 실제 XML의 정규화된 전체 시험 이름 51개를 계획 이름과 대조했으며 누락·초과·중복은 모두 0이다.
- 원시 XML `artifacts/c4-worker51.xml`: SHA-256 `647B347808BCDD13B8C6C8C99FD6391C771912815CB2FEA07E0D1F6269E141EF`; 51/51 통과, 실패·건너뜀·판정불가 0이다. 실제 네이티브 종료 관찰 `artifacts/c4-worker51-native-exit-observation.json` SHA `44CAF4DD6C0DA974736F0B266FE0C9857A0B7408AFAE8C3FC1CC81ED8A101AB2`는 계획 결속 및 종료 0을 기록한다. QA 도구 반환 `artifacts/c4-worker51-qa-tool-return.json` SHA `B1C5B26458FBC40E84D5CC5875E73E2FE31DCCF763E86574AC5D61A2333056CB`도 도구 종료 0과 바깥 종료 0을 기록한다.
- 전·후 입력 캡처는 각각 1084개 경로다. 경로·SHA를 전체 대조한 차이는 0이다. 전 캡처 SHA `227E4253A919218547C9DA1303C0B64A83690DC3E5678E011E6099CC5EFD79A3`, 후 캡처 SHA `F035DAB516D55EC991936FA14F6760C7AE8F7FD4883BC42541EFEE4A3294F99D`다. 두 캡처 모두 입력 목록 v19 `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1C`에 결속한다.
- 행 비교 `artifacts/c4-worker51-required-row-comparison.json` SHA `90618B4C51E5AF3320ABA408F4CE541CF9852B5B2B622385714B1BDFC4B1695A`는 C4 필수 행 0개에 대해 누락·예상 밖·무효 0 및 `EvidenceMatched=true`를 기록한다. 최종 검증 `artifacts/c4-worker51-verification.json` SHA `DA76943D3BCF9C8E4D83EF8C3A53743805E084DA6735AD7FC050FB0C09CF39D4`는 XML·캡처·네이티브·QA·행 비교의 파일 지문을 서로 연결하고 `Verified=true`, 종료 코드 셋 모두 0을 기록한다.

이 결과는 선택된 작업자 회귀 묶음의 실제 통과만 뒷받침한다. `WholeAccepted=false`를 유지한다. 다른 실행 묶음, 전체 C4 또는 공동 C3/C4 수용으로 확대하지 않는다. 검토 기록 경로가 입력 목록 v19 밖임을 쓰기 전에 확인했다. 기존 입력·실행 산출물은 변경하지 않았다.