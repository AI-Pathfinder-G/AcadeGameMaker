# 저장소 줄바꿈 기준 505개 게시 검토 후보 작성

- 상태: **Approved**, 2026-10-02 아스트라 역할 승인. 사용자가 두 추천안, 즉 저장소 줄바꿈 기준 재검증과 QA 검사기·입력 네 개의 별도 후보 검토를 선택했다.
- 원본: v2 원장 SHA-256 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`의 501개와 [정규화 조사](../../verification/2026-10-01-initial-publication-git-canonicalization-precheck.md). QA 추가 후보는 `qa/schema/qa-scenario-catalog.schema.json`, `qa/coverage/required-ac-ids.json`, `qa/coverage/allowed-blocker-ids.json`, `qa/catalog/vertical-demo.json` 네 개다. 검사기 `qa/tools/Test-QaCatalog.ps1`은 기존 501개에 포함된다.

## 범위와 불변 조건

원본 501개는 v2의 현재 지문과 모두 일치해야 한다. 추가 네 파일의 SHA-256은 [직접 입력 예비 대조](../../verification/2026-10-01-initial-publication-qa-catalog-dependency-precheck.md)의 값과 일치해야 한다. 원본·v1/v2 원장·과거 동결·두 Unity 진단 복사본은 수정하지 않는다. `.gitattributes`와 Git 설정도 변경하지 않는다.

새 `output/publication-lf505`가 존재하지 않는지 확인하고 정확한 505개만 복사한다. 각 원본의 필터 없는 Git 객체와 저장소 속성을 적용한 Git 객체를 읽기 전용으로 비교한다. 같은 경우 바이트 그대로, 다른 경우 CRLF→LF 변환만 허용하고 변환 바이트의 Git 객체가 후자와 정확히 같아야 한다. 다른 변환이나 지문 변경은 중단 사유다. 원문·후보 SHA-256, 원문·저장소 Git 객체, 변환 종류, 신규 범위와 조회 시각을 505행에 기록한다. 과거 승인·검수의 원문 SHA를 새 SHA로 바꾸지 않고, 줄바꿈 전이 근거와 새 바이트 검수 미완료를 기록한다.

검사기와 네 입력의 현재 내용·직접 읽기 관계를 VD-11 `REQ-PLAT-012..014`, `AC-PLAT-010..012`에 결속한다. 새 복사본에서 검사기 일반 실행과 자체 음성 검사를 각각 한 번 수행하고 실제 출력·종료값과 입력 전후 지문을 기록한다. Unity·빌드·네트워크·Git 쓰기·게시·추가 파일 복사는 허용하지 않는다. 후보 밖 의존은 자동 포함하지 않고 명시적으로 미폐쇄로 남긴다.

허용 출력은 위 복사본, `artifacts/c4-publication-lf505-baseline-v1.json`, 같은 이름 `.md`, `docs/verification/2026-10-02-publication-lf505-terra-report.md`다. 모든 행과 전체의 `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`, `CleanCloneExecuted`는 거짓이다. 후보 수 505는 게시 허용 파일 수가 아니다. 루나 검수는 사용자 지시대로 한도 회복 뒤 별도다.

| 요구 | 인수 기준 |
| --- | --- |
| `REQ-PLAT-038`: 원문을 보존하고 저장소 기록 바이트를 새 후보 기준으로 결속한다. | `AC-PLAT-036`: 원본 501/501 및 추가 네 지문 일치, 새 후보 505개 고유 경로, 원본 전후 차이 0, 후보 밖 초기 복사 0. 모든 변환은 CRLF→LF만이며 저장소 Git 객체와 일치한다. |
| `REQ-PLAT-039`: QA 검사기와 네 직접 입력을 함께 검토한다. | `AC-PLAT-037`: 정확 다섯 경로·지문·읽기 위치·VD-11 요구/AC를 기록하고, 후보 복사본에서 일반·자체 검사 출력과 종료값·입력 전후 지문을 기록한다. 후보 밖 의존과 독립 검수는 미완료로 유지한다. |
| `REQ-PLAT-040`: 새 후보와 과거 수용·게시 권한을 구별한다. | `AC-PLAT-038`: 원문 501 기준과 과거 증거를 보존하고, 505행 모두 새 바이트 검수 상태와 다섯 거짓 플래그를 가진다. Unity·원격·Git 변경 및 코드 게시를 수행하지 않는다. |

테라 구현 역할의 `gpt-6-sol`이 제한 작업과 자체 보고를 맡고, 아스트라가 기계 대조한다. 이는 루나 독립 수용을 대신하지 않는다.
