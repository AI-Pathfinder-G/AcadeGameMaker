# Q0 발급 검증기 라이브러리 범위 보정

상태: **Approved** — 2026-10-03 실제 `gpt-6-astra`. `REQ-PLAT-068/069/070`, `AC-PLAT-066/067/068`의 실제 서명 전 검증기 연결을 보정한다. 원계약4F9, 발급 제어19DC, 준비수용3C958, 정수보정3084의 기존 SHA와 원격 정확 두 문서 범위는 불변이다.

## 실제 결함

동결 issuer SHA `6D41A75D7AF69124AB36A8BBEC3CDA638F5C35DDBFBBF337D9B915E083175E5B`는 `Invoke-Q0Sign`에서 `Import-Q0IssuerVerifier`를 일반 함수 호출한다. 함수 안의 dot-source로 생성된 검증 명령이 반환 후 소멸한다. 아스트라가 실제 준비 검증기의 LibraryOnly 입구를 사용한 키 없는 함수 중첩 재현에서 `Assert-Q0ExecutionSchema`의 일반 호출 후 가시성 false, dot 호출 후 true를 직접 관측했다. 계획 읽기·키·서명·원격·파일 생성은 없었다. 기존68건은 import 효과를 모의 처리했으므로 이 실제 연결을 입증하지 않았다.

## 정확 변경과 증거

기존 `qa/tools/Invoke-Q0TrustedChildIssuer.ps1` 안에서 생산 Sign의 import 호출을 `. Import-Q0IssuerVerifier ...`로 바꾸어 현재 Sign 호출 범위에 함수를 남긴다. 공개키·계약·상태·서명 payload·게이트 의미를 바꾸지 않는다. `qa/tools/Test-Q0TrustedChildIssuer.ps1`에 실제 LibraryOnly 명령 지속과 생산 Sign 전처리의 연결 검수를 추가한다. 기존570/보정 lease/준비v2는 변경하지 않는다.

새 증거는 정확 `output/q0-trusted-child-evidence-r1/issuer-control-keyless-v2.json` 한 파일만 허용한다. v1 증거34882바이트 SHA `1F0A3881603A897D94B3A6224E9C0504E8266DBBD5157332FFB32651EE86C1D6`와 기존 검수 줄은 실패 발견 전 검수 범위로 불변 보존한다. 새 결과에는 기존68 근거와 이번 실제 연결 검증의 관측 범위를 구분하고, 변경된 두 소스의 전후 지문·실제 프로세스 신원/종료·원시 stdout/stderr·시험 기대/관측 및 키/원격 미실행을 결속한다. 기존 생산 Sign 모의는 실제 import/실제 parser/schema까지 유지하고 이후 외부 효과만 시험 프로세스에서 차단한다. 명령 존재 확인만으로 최종 계획/서명 성공을 주장하지 않는다. actual Sign 함수에서 import 반환 후 실제 schema 호출 도달과 형식 오류 거절을 확인하고 core Sign 호출0을 관측한다. 기존 Sign1/실패 정리·commit/v2/lease/entry 결속 회귀는 영향 범위에 맞게 한 번 검증한다.

후보 원시는 메모리에서 Root 검수 후 동일바이트를 v2 지정 경로에 CreateNew 한 번 보존할 수 있다. Luna가 실제 파일을 전체 검수한 후에만 새 검수 앵커를 작성한다. 이 순서는 앞선 수집 보완과 같으며 운영키 이전 독립 전체 검수는 필수다. 실패 자료 삭제·덮어쓰기·임의 v3·키/실제 SignHash 호출은 금지한다.

## 앵커와 시작 경계

기존 `IssuerControlLunaReviewJson` 한 줄을 수정하거나 복제하지 않는다. 새 독립 줄은 정확 유일 접두사 `IssuerControlLunaReviewV2Json: `이며 기존 정확10키/스키마 `AGM-Q0-ISSUER-CONTROL-LUNA-REVIEW-V1`/Decision `AcceptedLimitedKeylessControl`을 유지한다. ContractSha256는 계속19DC, KeylessEvidence.Path는 위 v2 경로, 소스2 SHA와 증거SHA는 실제 최종값이다. 인접 설명에 본 보정 계약 경로·SHA와 기존68 한계를 기록한다. SHA는 접두사 뒤 JSON 원시 UTF-8, 줄끝 제외다.

issuer Boot의 독립 검수 검색 접두사와 허용 증거 경로를 각각 위 V2 접두사/v2 경로로 한정 갱신한다. 기존 줄을 대안으로 받아들이지 않는다. Boot은 본 보정 계약 원문 SHA도 확인한다. 아직 부재인 시작 앵커는 기존 `IssuerControlStartAcceptanceJson: ` 정확10키 그대로이며 실제 Astra가 새 독립 줄/최종 소스/증거를 읽은 뒤 발급한다. IndependentReview.EvidenceSha256은 V2 JSON 원시 SHA만 참조한다. 최종 handoff 전체 SHA는 Root가 직접 고정한다. 자기소스/미래문서 SHA를 소스에 사전삽입하는 순환은 금지한다.

별도 Root 메모리 진행자의 기존 시작 앵커10 검사는 유지한다. Root의 사전 독립 검수 검색은 V2로 갱신한다. 전체 계획53·Input565·Source570·미래568불변·정확 두 원격 문서·신규 한 세대 범위는 늘리지 않는다. 테라 구현→루나 독립 검수→실제 Astra 시작 판정까지 키0·원격 쓰기0이다. 이번 계약은 코드 보정 승인일 뿐 운영 개시나 전체 실행 수용이 아니다.
