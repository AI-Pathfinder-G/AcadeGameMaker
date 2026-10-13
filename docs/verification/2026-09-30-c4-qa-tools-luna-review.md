# C4 QA 도구의 선행 선택 분기 독립 검수

- 기준: Approved QA r2 `docs/specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md`, SHA-256 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`.
- 선행 형식 예외 Draft: `docs/specs/work-contracts/2026-09-29-c4-predecessor-selection-format-amendment.md`, SHA-256 `DFBF92EB722CEEEFBD8A16F85338B776F01C2AC692C4DA058FB6BC79F582B754`.
- 검수: 실제 `gpt-6-luna`, 2026-09-30. 읽기 전용 정적 소스 대조이며 실행·독립 수용이 아니다.
- 판정: P0=0, P1=1 (legacy 플랫폼 고정 검증 누락). Unity/QA 도구 실행 결과는 없다.

## 도구 지문

| 파일 | SHA-256 |
| --- | --- |
| `artifacts/c4-build-focused-selection.ps1` | `B16362720E9DE2E69FC44990A664EA221E6A928D8A795A5F70C959D785D1015C` |
| `artifacts/c4-build-required-checkpoint-rows.ps1` | `D1B91A300016EA55FC8A5F69395DCFE0D63E36EA222BE3B4F62E5CC4ACEF8BBD` |
| `artifacts/c4-capture-inputs.ps1` | `5552B87255EF601E8D9A7FB6076DEE892F2082E85F01E2F05AF93637E91C74DB` |
| `artifacts/c4-final-selected-run.ps1` | `ED2BF1FA01A41559189A63296A7C749EFDD09BEF6408729038280422BDB5A1E4` |
| `artifacts/c4-final-validation-queue.ps1` | `B1E9BA59BE887667A9F1764FACEA673015F9D08D43A37A278711A8101E46B9F4` |
| `artifacts/c4-verify-required-rows.ps1` | `7C413284E2819D866BC237E05EBCD68C04EE8680CC2EF94AD131B10B1A013E51` |
| `artifacts/c4-verify-run.ps1` | `FA610EABEAAA3FD6DB36B32F7BFA3B7B96E4F1F22791B1F4283231507B9EB414` |

## 차단사항

**P1 — 기존 선택의 고정 플랫폼이 모든 legacy 경로에서 강제되지 않는다.** `artifacts/c4-verify-required-rows.ps1`의 `C4-RunSelection`(약 50–53행)은 여섯 허용 경로의 SHA, 원래 키 집합, 지도 부분, 이름·Selector·Count를 확인한다. A 형식에는 `Platform`이 있어 실행 계획과 비교하지만 B/C 형식에는 그 필드가 없다. 이어지는 공통 조건은 필드가 있을 때만 비교한다. `C4-LegacyRun`이 `EditMode`를 강제하는 예외는 Matrix91 한 건뿐이다. 따라서 같은 원본을 사용한 Edit562, Worker51 또는 Play610 실행 계획의 `Platform`이 잘못 지정되어도 이 검증 분기만으로는 제안서가 요구하는 각각 `EditMode`, `EditMode`, `PlayMode`에 묶이지 않는다. 플랫폼을 원본 경로/PartId/고정 지도 ID에 직접 결속하고, 해당 여섯 이름 배열과 정확히 일치하지 않으면 실행 전에 거절해야 한다. 이 결함은 입력 JSON의 변조를 뜻하지 않으며, 제안 Draft의 기준을 현재 도구가 아직 충족하지 않는다는 구현 차이다.

선행 형식 개정은 현재 Draft다. QA r2의 신규 선택 스키마를 여섯 기존 파일에 대한 예외로 바꾸는 권한은 아직 발급되지 않았다. 따라서 현재 코드에 legacy 분기가 존재한다는 사실을 예외 승인이나 실행 가능성으로 해석해서는 안 된다. Astra의 개정 승인 후에도 위 P1 수정과 정확 지문 검수가 선행되어야 한다.

## 나머지 구조 대조

현재 `C4-RunSelection`의 경로별 구분표는 지정된 여섯 경로만 legacy로 분기하고, 그 밖에는 QA r2 신규 선택 키 집합을 요구한다. 원본을 새 키로 복사하는 fallback은 보이지 않는다. 여섯 원본을 직접 읽어 대조한 결과 경로·SHA·키 형식·이름 수는 Draft와 일치했으며 Worker51은 11키 형식·51개 이름이다. 승인 예외 후에도 C의 `EditMode`를 원래 고정 큐 배분과 연결해야 한다.

구현자 자체 점검 기록은 과거 네 정합성 결함을 보고했지만 독립 근거로 쓰지 않았다. 현 도구 본문에서 다음 보정을 직접 확인했다: 큐 도구 배열은 정확 길이 10과 고유 경로 10을 함께 요구하고, 프로젝트 경로를 `OrdinalIgnoreCase`로 비교하며, 큐 최종 기록은 mutex 해제보다 먼저 수행된다. 공통 엄격 JSON 파서는 알려진 정수 필드를 숫자 토큰의 Int64로 검사하고 문자열 숫자를 정수로 바꾸지 않는다. 큐는 실행별 경계에서 계획 지문과 입력 결속을 재검사한다. 이 정적 구조 대조는 동시 실행·프로세스 수명·실제 종료 관측이 성공했다고 입증하지 않는다.

따라서 QA 도구 자체 점검에서 언급한 mutex 기록 순서, 10개 도구의 개수/중복, 경로 대소문자 비교, 정수 문자열 강제변환의 네 항목은 현재 바이트에서 P1로 재현되지 않았다. 실제 실행, 최종 Unity 선택, 결과 검증은 수행하지 않았으며 어떤 C4 행이나 전체 수용도 주장하지 않는다.
