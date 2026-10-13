# C4 R6 집중 Edit 실행 결과 독립 검토

- 계획 v8: `artifacts/c4-final-validation-queue-plan-v8.json` — SHA-256 `B2CB339E5EFA7E18B3C5A5862AD7DD1D2905D37602C716984DCF58D51D00868A`
- 선택 v5: `artifacts/c4-focused-edit-selection-v5.json` — SHA-256 `B66218B56C4F88F626EBC68B5345DAF09A4582DFB03E89AE2D6A51D1F566803D`
- 행 v5: `artifacts/c4-required-checkpoint-rows-v5.json` — SHA-256 `287050D0B5DEDE53566D1229EB31324716FD4C8308165889A31B0C1A1CF1AB88`
- 실행: `artifacts/c4-r6-focused-edit`
- 범위: 첫 집중 Edit 묶음의 실제 XML·종료 관측·QA 반환·입력 스냅샷·행 비교만 독립 확인했다. 문서는 실행 입력/출력 목록 밖에 작성했다. 후속 실행과 전체 수용은 판정하지 않는다.

계획의 선택·manifest·입력·행 지문은 현재 파일과 모두 일치한다. 계획된 139개 전체 테스트 이름과 실제 XML 이름을 독립 비교해 누락·초과·중복 없이 일치함을 확인했다. XML은 139/139 통과, 실패·건너뜀·미결정 0이다. Native 관측은 실제 Unity 프로세스의 경로와 실행 인자를 확인한 뒤 종료값 0을 기록했고, QA 반환과 외부 종료값도 각각 0이다. 실행 검증은 계획 지문과 결속되며 `WholeAccepted=false`다.

전후 입력 스냅샷은 각 1,008개 항목이고 경로·값이 모두 같다. 검증 결과의 입력 차이 및 동결 지문 차이도 각각 0이다. 필수 Edit 행 비교는 계획 148, 도달 148, 통과 148, 실패·미도달·누락·초과·무효 0이며 `SourceMatched=true`, `EvidenceMatched=true`다. 이는 행 148개의 증거 비교 수치이며 NUnit 테스트 148개라는 뜻은 아니다.

수정 대상 `AC001_UncommittedActualRequestCannotStart`는 XML에서 실제 `Passed`다. 행 비교의 최종 실행 기록에서 `history.originalCommitPreserved=Yes`가 관측됐고, 승인된 기준점 캡처 후 생성한 실제 커밋 참조와 일치한다. 초기 거절 검증과 이어지는 정상 실행 순서도 유지됐다.

checkpoint 17 미반환 행은 `Passed`이며 `diagnostic.unreturnedSourceBoundaryVerified=Yes`의 포인터가 `ProfileResetExecutionBridgeV1.cs:302`로 결속된다. C1 Busy의 별도 receipt 미발급 및 관련 경계 사실은 각자의 기존 출처를 유지한다. 이번 검토에서는 서로 다른 두 source 증거를 같은 경계로 합치지 않았다.

R5의 과거 비교 실패는 그대로 보존된다. R6은 그와 다른 계획·선택·행 지문을 쓴 새 실행이다. 이번 결과는 첫 집중 Edit 묶음의 실행 및 행 증거만 충족하며, 뒤이은 8개 실행이 진행 중이므로 C4 전체 완료나 수용을 뜻하지 않는다.

판정: 이번 부분 실행에 대해 P0=0, P1=0. 다른 계획 묶음, 전체 회귀, C3 선행 게이트 또는 C4 통합 수용에 대한 판정은 하지 않았다.

증거 지문: XML `41423E1E68732F28E3D7D518D618263069893CAA7775AE97CF7264838130C148`; 검증 `E6FB719C3F64AC215DCB02A7F2C0FA6B628EE609B716623F2E98170E37F9FB6D`; 행 비교 `89E2A147FB1AF221612BC397BC61C20EF0D814A76D3AC7DF3B8A28C3809CD36E`; Native 종료 관측 `DD84AF63E143D2F9ACFC5FAE0A863804AC712F364E436F9DDC1F3EF8DE6667A1`; QA 반환 `9DE5EE22872FDB709EBFA655C322C542CAF8D156D62F4C3404A599947826FF08`; source-before `7DE6610266CAD6D2456EF4DB94A21AC78FA8BD2D7C1C75166223FD1B27AE54D5`; source-after `B51BCC959A1915B9643CDFAE9FC39997D4D269C2651BF82861F521F5EE80170B`.
