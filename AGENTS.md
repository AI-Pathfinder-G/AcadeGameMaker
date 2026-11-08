## 최신 사용자 지시 — 2026-11-08 Unity 개발 재개

사용자가 “유니티 프로그램 호출해서 개발 시작해”라고 명시하여 11월7일 Unity 실행 보류를 해제했다. 현재 활성 worktree의 output/chapter1-playable-r1/project에서 실제 Unity 컴파일·집중 검사·필요한 구현을 재개한다. 이전 실패 원문과 원본 보존본은 유지하고 다른 프로젝트 프로세스·파일은 변경하지 않는다.

## 최신 사용자 지시 — 2026-11-07 원격 통합과 자원 경합 방지

통합 감사를 통과한 안전한 변경을 원격 main에 통합한 뒤 설계·구현을 계속한다. 산사 개발의 자원을 우선하므로 Unity Editor·Hub·batchmode·Player·빌드 실행은 보류한다. Unity 앱 개발 자체를 금지하는 지시가 아니며 가벼운 비Unity 정적 검증과 코드·설계 작업은 계속한다. 다른 프로젝트 프로세스는 중지하거나 변경하지 않는다. 이전 실행 의무는 이 보류 범위에서 재개하지 않는다.

## 최신 사용자 추가 지시 — 2026-11-05 반복 차단과 주요 체크포인트 보고

- 사용자 직접 지시: 불필요한 준비·승인 의존을 한 번에 정리하고 실제 Unity 검사와 Windows 실행파일 제작으로 진행한다. 규칙만 추가한 뒤 같은 작업을 반복하지 않는다. 무한 토큰 소비·자동 재시도·반복 구현을 금지한다. 아래 장치는 새 검사기나 승인 계층을 만드는 요구가 아니다.
- **단일 실행 작업판:** 메인 담당은 기존 작업 원장 하나에 현재 체크포인트, 제품 변경, 실제 다음 실행, 필수 선행/단순 기록의 구분, 담당, 잔여 실행 횟수·누적 시간 한도, 실패 원인과 다음 경로를 적고 실행 때마다 갱신한다. 새 원장·새 지문 고정·새 원장 검수는 만들지 않는다. 준비 문서 변경은 제품 결과를 자동 무효화하지 않는다. 실제 권한·제품 정확성·프로세스/데이터 보호·독립 검증에 필요한 조건은 유지하고, 충돌하는 세부 계약은 현존 차단 전부를 모아 한 번의 호환 판정으로 정리한다. 과거 실패 보존과 현재 실행 선행을 분리한다.
- **재시도 차단:** 모든 하위 배정에 제품 목적·필수 산출물·종료 조건·기존 예산 안의 유한 실행 횟수/누적 시간을 포함한다. 기존 단회 제한은 유지한다. 같은 원인 두 번이면 그 경로의 자동 재시도를 중단하며, 작업자 교체·새 폴더·새 승인 사본·새 시계로 횟수나 누적 시간을 초기화하지 않는다. 원인 보정과 새로운 관측 목적이 없는 재실행은 하지 않는다. 허용 예산이 없거나 소진되면 그 경로는 실패와 잔여 일을 보고하고 중단한다. 충돌 없는 승인 제품 작업은 계속하며, 별도 승인 없는 예산 확대·새 반복 묶음 발급을 금지한다.
- **정체 감지:** 실제 제품 변경 또는 실제 시험의 새로운 관측 없이 준비·승인 작업만30분 이어지면 정체로 보고한다. 해당30분은 성공 증거를 무효화하는 새 시험 기한이 아니다. 기존 원시를 재사용해 의존 제거·더 단순한 허용 경로·비충돌 제품 구현 중 하나로 작업 방향을 바꾸고 변경 이유를 보고한다. 같은 준비 순환을 새 이름으로 이어가지 않는다. 토큰 실측이 없으면 사용량을 만들지 않고 경과시간·도구 호출·재시도 수로 운영한다.
- **주요 체크포인트:** ① 불필요한 실행 선행 정리와 첫 실제 Unity 검사 결과 확보, ② 필요한 수정 구현·설계/REQ/AC 대조와 집중 검사·복구·자연 보스·필수 회귀 통과, ③ 현재 소스 Windows 실행파일/압축본 제작, ④ 동일 배포본 양분기·실패 재시도·저장/복귀 확인과 최종 전달. 기존 승인 단계 순서는 유지하고, 부분 성공을 다음 체크포인트 완료로 확대하지 않는다. 이미 달성한 단계는 새로 반복하지 않고 증거 적용성을 확인해 이어간다.
- **달성·실패 보고 의무:** 각 주요 체크포인트 달성 즉시 사용자에게 완료한 게임 기능/수정, 실제 시험 결과와 근거 경로, 설계 일치/남은 결함, 이전 대비 줄인 준비·승인 반복, 다음 실행을 짧게 보고한다. 미달·정체·예산 소진도 즉시 같은 형식으로 사실을 알린다. 이는 중간 사용자 승인 요청이나 전체 진척 감사가 아니다. 기존 일일 감사와 별개인 제품 체크포인트 보고를 허용·의무화하며, 앞선 체크포인트 보고 제한보다 이 최신 사용자 지시를 우선한다. 정기 예약이나 별도 감시 도구는 만들지 않는다.
- **적용 확인:** 메인 담당은 이 지시 수신 뒤 실제 현재 단계·잔여 차단 목록·유한 한도·첫 실행을 한 번 보고하고 작업한다. 이행 증거는 규칙 문서나 수신 답변이 아니라 실제 Unity 결과·제품 수정·실행파일이다. 규칙 이행을 확인하는 새 승인/검토 사업을 만들지 않는다.

## 최신 사용자 지시 — 2026-11-05 구현 순환과 시간 대비 제품 진전

- 사용자가 프로젝트 규칙으로 직접 지정했다. 개발의 기본 순환은 **구현 → 시험 → 설계 일치 여부 확인 → 수정 구현**이다. 승인된 제품 범위에서 이 순환을 계속하고, 검증 도구·준비·승인 기록 작업이 본업을 대체하도록 반복하지 않는다. 제품 범위 최초 승인과 필수 독립 검증·모델 역할·보안 권한은 유지한다.
- **6시간 이상 작업 시 해당 챕터 구현에 최소 10% 이상의 진전**을 사용자 성과 기준으로 삼는다. 착수 시 해당 챕터의 고정된 기능·콘텐츠·장면 목표 단위, 완료 조건과 시작 상태를 정하고 동일 분모의 이전/현재/증가분과 실제 동작 증거로 평가한다. 기준이 없으면 산정 불가를 밝히고 구체적 구현 완료 단위·실행 결과를 제시한다. 수치를 맞추기 위한 분모 축소·재가중·점수 부풀리기·도구 성과 합산을 금지한다. 기존 챕터 패키지 성숙도와 세부 구현량을 구분한다.
- 위 기준은 자동 달성 보장이 아니라 작업 성과의 평가·시정 기준이다. 6시간 동안 진전을 입증하지 못하면 정상 진행으로 포장하지 않고 미달 원인·실제 산출물·남은 병목·변경할 실행 경로를 짧게 밝히고 작업 방식을 바로잡는다. 중간 반복 동작을 사용자에게 넘기지 않는다. 정기 감사 주기는 유지하고 작업 원장의 증거를 재사용한다.
- 같은 승인 묶음 안의 국소 수정은 필요한 컴파일·집중 시험을 바로 수행하고, 설계/REQ/AC와 결과를 대조해 수정 구현으로 돌아간다. 독립 검수·통합 승인·전체 회귀는 기존 필수 선행을 지키면서 기능 묶음 경계에 모은다. 작은 보정·지문·읽기량·표시 형식마다 새 계약·승인·검토 도구를 연쇄 추가하지 않는다. 실제 계약 충돌은 정확 차이를 모아 한 번의 호환 보정으로 처리한다.
- 검증 작업에는 연결되는 제품 기능·막는 결함·최소 검사·종료 조건이 있어야 한다. 준비/검토 실패가 반복되면 동일 대형 검토를 재시작하지 말고 저장 원시·최소 재현·원인 보정으로 좁히거나 더 단순한 허용 실행 경로로 전환한다. 새 보조 도구를 만들기 전에 기존 도구·공개 시험 API·배치/단독 실행으로 해결 가능한지 확인한다. 필수 검증을 생략하거나 실패·미검증을 성공으로 바꾸지 않는다.
- 현재 CH1의 다음 성과는 실제 Unity 검사 결과와 그 뒤 필수 검증을 통과한 현재 소스 Windows 실행파일이다. 문서·준비도구 완료만을 다음 제품 성과로 제시하지 않는다. 이 직접 사용자 지시의 기록·적용 자체를 별도 승인 대기나 새 검증 사업으로 만들지 않는다.

## 최상위 사용자 지시 — 2026-10-24 역할 제한·예외 사전 승인

이 절은 모델 역할과 예외 승인에 관한 프로젝트 최상위 작업지침이다. 이전 역할 설명, ADR·계약·인계의 배정 문구와 충돌하면 이 절과 ADR-0045를 우선한다. 역사적 승인·실행·실패 원문과 실제 모델 표기는 보존한다.

- **Astra (`gpt-6-astra`)**: 통합 승인·최종 통합 판정, 프로젝트 전체 레벨의 기획·설계 변경이 필요한 경우의 결정으로 사용을 한정한다. 이에 필요한 범위·인터페이스·계약 변경 승인과 승인 기록은 허용한다. 개별 파트 상세 설계, 일상적 원인 조사·디버깅·반복 원시 판독, 구현·시험·빌드 실행을 맡기지 않는다. 제품 코드·콘텐츠·시험 코드·검사기·실행 도구 등 직접 구현은 금지한다. 통합 과정의 코드 수정·충돌 해결 구현도 Sol이 담당하고 Astra는 판정한다.
- **Sol (`gpt-6.1-sol`)**: 개별 파트의 기획·설계·계약 초안·영향 조사·원인 분석·구현·구현 검사·시험 자료·검사기·실행 도구를 담당한다. 승인된 범위의 실제 시험·빌드·증거 정리와 구현을 계속한다. 프로젝트 전체 기획설계 또는 승인 경계 변경이 필요할 때만 Astra에 정확 차이와 결정 사항을 제출한다.
- **Luna (`gpt-6-luna`)**: 기존 직무를 유지한다. 사전·사후 반례 검토, 독립 코드·서사 검수, 회귀·빌드 검증 및 증거 요약을 담당한다. 구현자가 자기 결과를 독립 수용하지 않는다.
- **예외는 사용자 명시적 사전 승인 필수**: 위 역할분담의 예외가 불가피하면 수행 전에 사용자에게 사유, 정상 역할로 처리할 수 없는 이유와 검토한 대안, 대상 모델·작업·파일 범위, 위험과 유한 기간/예산을 제출하고 명시적 승인을 받은 경우에만 해당 범위에서 수행한다. Astra·Root·다른 에이전트의 자체 승인, 기존 자율 위임, 긴급성, 한도·도구 오류, 응답 없음은 예외 승인이 아니다. 승인은 해당 예외에만 한정하며 이후 포괄 허용으로 재사용하지 않는다.
- **예외 대기와 정상 작업 분리**: 승인 전에는 예외 작업만 보류하고 충돌 없는 기승인 작업을 계속한다. 2026-10-21의 중간 사용자 참여 면제는 정상 역할 안의 작업에 계속 적용하지만 이 예외 사전 승인 조건을 면제하지 않는다. Root·총괄 등 이름으로 실제 모델의 역할 제한을 우회하지 않는다.
- **묶음 운영**: 상세 조사와 증거는 Sol·Luna가 모으고 Astra에는 통합 승인 또는 프로젝트 전체 결정에 필요한 차이·선택지만 제출한다. 허용 범위 내 국소 보정·핀마다 Astra 호출·재승인·대기를 반복하지 않는다. 일일 감사 자료의 수집·계산도 Sol·Luna가 담당하고 Astra는 필요한 통합 판정만 맡는다.
- **루나 기준 버전 원복 — 2026-10-24 최신 사용자 지시**: Luna의 현재 기준은 `gpt-6-luna`다. 앞선 6.1 기준 지정과 그 미제공에 따른 배정 보류를 대체하며 신규·재시도·대체 작업에 실제 제공되는 `gpt-6-luna`를 사용할 수 있다. 향후 실제 제공되는 같은 계열 상위 안정 버전은 ADR-0041에 따라 적용한다. 기존 직무·역할 예외의 사용자 사전 승인 조건과 과거 실제 모델 기록은 유지한다.
- 모델 기준·동일 계열 상위 안정 버전의 실제 가용성 확인(ADR-0041), 기본 추론 설정, 공유 파일 단일 쓰기 소유와 독립 검수는 유지한다. 모델 버전 승격은 역할 확대나 예외 승인으로 해석하지 않는다.

## 최신 사용자 위임 — 2026-10-21, 현재 우선 규칙

- 사용자는 1챕터 정상 플레이 가능한 최종 Windows 결과물을 받을 때까지 중간 승인·재생 버튼·키 입력·중립 확인·경로 입력 등 반복 작업에 참여하지 않는다. 에이전트가 대체 방법을 찾아 개발·시험·보정을 계속한다. 이 지시를 추가 사용자 확인 사유로 삼지 않는다.
- 현재 권위는 `docs/adr/0044-autonomous-chapter1-local-delivery.md`와 Approved `docs/specs/work-contracts/2026-10-21-autonomous-chapter1-delivery.md`다. 새 로컬 경로에서 에이전트의 공개 Unity 시험 API·격리 합성 입력·지정 게임 창의 지원된 공개 입력·시작/정지/재시도/빌드를 허용한다. 기존 사용자 전담 Play/입력·중간 진단 대기는 이 범위에서 대체된다.
- 기존 실패·소진·동결 R01/G3/Q0 원문은 보존한다. 자동/실제 창/독립 검수/인간 체험 증거를 구분하며 자동 시험을 사람 완주로 표시하지 않는다. 외부 게시와 역사적 인간 검수 대기를 모든 로컬 개발/전달의 선행으로 확대하지 않는다.
- OS 보안·인증·권한창 대리 처리는 금지한다. 차단된 경로 대신 허용된 배치/단독 빌드/API 경로를 사용하고 미검증은 정직하게 남긴다. 실제 사용자 프로필·키·원격 게시 권한은 확대하지 않는다.
- Astra/Sol/Luna 역할·허용 모델·단일 쓰기 소유·독립 검수를 유지한다. 한 계약의 정확 범위와 유한 예산 안에서는 작은 보정·동적 핀·버튼마다 새 승인 사본/사용자 질문을 반복하지 않는다. 최종 대상은 CH1 전체 양분기 종결·시작/조작/결과/실패 재시도와 실행파일이며 준비창이나 보스 단독 구간을 최종 결과로 대체하지 않는다.

## 기존 원격 운영 규칙 — 역사와 비충돌 규칙 보존

최상단 최신 사용자 지시와 역할 제한이 우선하며 아래의 이전 실행 지시는 현재 Unity 실행 허가가 아니다.

# Agent Operating Rules

## 실제 플레이 통합 우선 — 2026-10-03 사용자 개선 지시

- 활성 구현 전략은 `docs/specs/work-contracts/2026-10-03-play-first-delivery-strategy.md`, 결정 근거는 ADR-0039와 `docs/verification/implementation-audits/2026-10-03-03.md`다. 절차 전략의 Approved 상태를 확인하고 적용한다.
- 최신 최우선 목표는 사용자가 본편 1챕터를 직접 체험하고 완주하는 현재 소스 로컬 빌드다. ADR-0042와 `docs/specs/work-contracts/2026-10-03-chapter1-playable-delivery.md`를 따른다. 단일 보스 구간은 첫 연결 단계이며 최종 목표를 대체하지 않는다. 다음 주력은 CH1 의존 조사와 즉시 구현 가능한 첫 연결 계약 확정이다. 해당 제품 계약이 Approved가 된 뒤 구현한다. 게시 대기와 충돌 없는 설계·영향 조사를 분리하고, 게시 완료를 모든 로컬 작업의 암묵적 선행조건으로 삼지 않는다.
- 작업마다 사용자 동작/산출물·담당·실제 선행 조건·검사 계획·종료 조건을 적는다. 같은 사용자 행동·위험/소유 범위의 보정은 일관된 승인 묶음으로 관리한다.
- 2026-10-08 사용자 검수 개선 지시: 독립 검수·통합 승인·전체 회귀를 매 작은 개발건마다 호출하지 않고 원정 연결(M1), 보스 이후 연결(M2), 최종 사용자 빌드(M3)의 기능 묶음 완료 시점에 모은다. 구현 중 필요한 컴파일·국소 검사는 유지하며 작은 보정은 한 원장에 누적한다. 활성 전략의 REQ/AC-PROC-011을 따른다. 기존 계약의 실제 선행·단회·필수 검사/권한 경계와 충돌하면 정확 호환 계획을 묶음으로 먼저 승인하고 동결 원문·실패 기록은 보존한다.
- 2026-10-08 사용자 탄력 개발 지시: 활성 전략 REQ/AC-PROC-012에 따라 한 묶음의 허용 경로·행동·소유·인터페이스·유한 검사/국소 재실행 예산을 처음에 고정하고 그 범위 안 수정·검사를 계속한다. 작은 보정마다 새 승인/전체 회귀/검수 문서를 반복하지 않는다. 실제 실행 입력과 승인 개정만 결속하고 문서 지문 재귀 의존을 새로 만들지 않는다. 현재 G3의 직렬 소스/컴파일/실행 승인과 충돌하는 범위는 M1 호환 부록 하나에서 먼저 해결한다. 실제 앱 동의·권한과 필수 AC를 우회하지 않으며 막힌 경로와 충돌 없는 본 개발은 계속한다.
- 2026-10-08 사용자 검수 개선 지시: 독립 검수·통합 승인·전체 회귀를 매 작은 개발건마다 호출하지 않고 원정 연결(M1), 보스 이후 연결(M2), 최종 사용자 빌드(M3)의 기능 묶음 완료 시점에 모은다. 구현 중 필요한 컴파일·국소 검사는 유지하며 작은 보정은 한 원장에 누적한다. 활성 전략의 REQ/AC-PROC-011을 따른다. 기존 계약의 실제 선행·단회·필수 검사/권한 경계와 충돌하면 정확 호환 계획을 묶음으로 먼저 승인하고 동결 원문·실패 기록은 보존한다.
- 2026-10-08 사용자 탄력 개발 지시: 활성 전략 REQ/AC-PROC-012에 따라 한 묶음의 허용 경로·행동·소유·인터페이스·유한 검사/국소 재실행 예산을 처음에 고정하고 그 범위 안 수정·검사를 계속한다. 작은 보정마다 새 승인/전체 회귀/검수 문서를 반복하지 않는다. 실제 실행 입력과 승인 개정만 결속하고 문서 지문 재귀 의존을 새로 만들지 않는다. 현재 G3의 직렬 소스/컴파일/실행 승인과 충돌하는 범위는 M1 호환 부록 하나에서 먼저 해결한다. 실제 앱 동의·권한과 필수 AC를 우회하지 않으며 막힌 경로와 충돌 없는 본 개발은 계속한다.
- 2026-10-11 사용자 지시: 결정이 필요한 항목은 보류 목록에 남기고 해당 결정과 충돌 없는 승인된 작업을 계속한다. 보류를 승인으로 해석하거나 앱 동의·권한·필수 인간 관문을 우회하지 않는다.
- 동일 오류 두 번이면 큰 검사를 반복하지 않고 원시 증거·최소 재현·원인/변경·새 관측 목적부터 정리한다. 기존 단회 계약이면 첫 실패부터 해당 제한을 따른다. 로그 침묵만으로 시험 정지를 판정하지 않는다.
- 집중 검사·필수 회귀·증거 재사용은 영향 분석과 독립 검토를 거쳐 정한다. 기존 필수 검사·Q0 인간 승인·세대·키·9회 실행·원격 게시 관문은 유지한다. 동결 계약/소스/인계 원문은 이 절차 문서 반영을 이유로 수정하지 않는다.
- 기본 아트 패키지와 실제 장면 연결을 먼저 수용한 뒤 변형을 확대한다. 다음 전달물은 시작·조작·결과·재시도·현재 소스 빌드와 독립 플레이 증거다. 전체 데모/본편 목표와 기존 AC는 유지한다.

## 구현 진척 감사 — 2026-10-03 사용자 지시

- 구현 상태·진척 감사 요청에는 항상 `docs/qa/implementation-audit-standard.md`와 `docs/qa/implementation-audit-template.md`를 사용한다.
- 2026-10-08 사용자 지시: 첫 보고 표는 1챕터 기준과 전체8챕터 기준을 병기한다. `inventory-v2.md`의 챕터 전달 패키지 성숙도·실제 완주·상세 제작량 완료율을 구분하고 공통47행 등의 일부 수치를 챕터 전체 완성도로 대신하지 않는다. 공통 기반은 별도로 보고한다.
- 2026-10-08 사용자 지시: 첫 보고 표는 1챕터 기준과 전체8챕터 기준을 병기한다. `inventory-v2.md`의 챕터 전달 패키지 성숙도·실제 완주·상세 제작량 완료율을 구분하고 공통47행 등의 일부 수치를 챕터 전체 완성도로 대신하지 않는다. 공통 기반은 별도로 보고한다.
- 아트(원화·스프라이트·동작·배경·화면 이미지 등), 소스(공통 기능·챕터별·스킬·공격·저장 등), 유니티 모델·장면(캐릭터·메인화면·설정·챕터 내부·마을·상점 등)을 나누어 세부 항목의 이전/현재 퍼센트와 증감(%p), 근거 및 남은 작업을 표시한다.
- 데모와 8챕터 본편의 범위·분모를 분리하고 고정된 단위/산정 규칙으로 계산한다. 근거 없는 전체 퍼센트나 과거 추정치를 재사용하지 않는다. 분모나 증거가 부족하면 산정 불가와 이유를 표시한다.
- 현재 및 향후 구현 세션의 진척 감사는 매일 한국시간(Asia/Seoul) 오전 8시 하루 한 번 수행하고 전일 일일 감사 대비로 보고한다. 구현부·중요 체크포인트마다 추가 진척 감사를 하지 않는다. 사용자의 별도 감사 요청은 예외다. 구현 중 산출물·필수 검사·독립 수용 증거는 계속 기록하고 다음 일일 감사에서 합산한다. 일일 보고는 감사 대화에 모으고 구현 세션에서 중복 작성하지 않는다.
- 검증·게시 도구의 진척을 실제 게임 콘텐츠 진척에 합산하지 않는다. 기록은 `docs/verification/implementation-audits/`에 보존하고 최신 색인을 갱신한다.

## Current authority — 2026-10-07 user correction / ADR-0043·0041

This section is the active staffing and approval model. ADR-0027 preserves historical approvals and artifacts; it does not preserve superseded delegation rules. Documentation authority, Approved-only implementation, REQ/AC traceability and behavioral contracts remain binding.

- Astra (`gpt-6-astra`) owns orchestration, allocation, global design, interfaces, canon, contract approval, conflict resolution and final integration.
- Sol (`gpt-6.1-sol`) owns bounded complex design, contract drafts, architectural counter-review, subsystem/content implementation, implementation tests, fixtures, validators, replay/Editor tools and QA program implementation; it has no competing final approval authority. Separate Sol workers may own bounded tasks, but no Terra role or Terra-named worker is assigned. See `docs/adr/0043-remove-terra-role.md`.
- Luna (`gpt-6-luna`) owns pre/post adversarial QA, independent code/narrative review, regression/build verification and evidence digests. A worker cannot independently accept its own implementation.
- Sol new work, retries and fallbacks use `gpt-6.1-sol`; do not resume `gpt-6-sol` workers. Luna uses `gpt-6-luna`, never GPT-5.6. Preserve the actual models and role labels of historical evidence. ADR-0040 records the previous routing; ADR-0043 removes the Terra role for subsequent assignments.
- 사용자 승인 자동 승격: 위 모델은 현재 기준이다. 새 작업 배정 직전에 실제 호출 도구의 허용 목록에서 같은 `astra`/`sol`/`luna` 계열의 더 높은 안정 숫자 버전이 제공되면 재승인 없이 기준과 이후 배정을 즉시 올린다. 숫자 버전으로 비교하며 미제공 식별자·다른 계열·하위 버전은 사용하지 않는다. 진행 중 작업과 역사 증거의 실제 모델은 보존한다. 고정 계약과 충돌하면 해당 실행의 호환 보정을 먼저 해결하고 동결 원문이나 실제 모델 표기를 바꾸지 않는다. `docs/adr/0041-automatic-model-version-upgrade.md`를 따르되 테라 역할에 관한 과거 배정은 ADR-0043으로 대체한다. 매시간 모델 기준 자동갱신 예약 `automation-2`는 2026-10-07 사용자 요청으로 삭제했으며 재생성하지 않는다. 새 배정 직전 가용 목록 확인은 유지한다.
- No GPT-5.6 model may be invoked for new work, retries or fallbacks. Existing GPT-5.6 agents are historical and must not be resumed. Use fresh agents with the explicit GPT-6 model assignments above. Historical evidence retains the actual models used.
- Astra/Sol/Luna are the only model families used for future project work. No Ollama, Kimi, GLM, MiniMax or Qwen calls, probes, retries, schedules or paid API routes are authorized. Historical records remain factual history and are not rewritten as current assignments.
- Spark is preferred for bounded verification when actually exposed by the client/tool. This session's subagent model enum does not expose Spark; do not invent IDs or claim execution. Luna is the allowed fallback. Start with local deterministic checks to minimize GPT tokens; batch external tasks once, request short findings only, and stop on quota/auth/model errors rather than looping.
- 2026-10-11 사용자 원복 지시: 최대 병렬 배정과 아스트라·솔 ultra/루나 max 강제 설정을 해제한다. 서브에이전트 수는 실제 과업에 필요한 만큼만 배정하며 동시 한도를 채우거나 한도 소진을 목적으로 늘리지 않는다. 새 배정은 추론 설정 상향값을 지정하지 않고 호출 도구의 기본 설정을 사용한다. 모델 계열·기준 버전, 테라 제거, 구현과 독립 검수 분리, 공유 파일 단일 소유 및 필수 승인 경계는 유지한다. 과거 실제 모델·설정·증거는 보존하고 주 대화 설정을 변경했다고 주장하지 않는다.
- Pro escalation is conditional on actual tool/client support. Current subagent controls expose model and effort, NOT Pro mode; never claim higher effort is Pro or invent Pro model IDs. No separately billed API route is authorized by this change.
- Astra approves bounded contracts before Sol implements; Luna verifies independently; Astra integrates. Additional Sol design review joins difficult bounded design as needed, not as a mandatory serial step. The implementer cannot accept its own implementation independently.
- Historical Sol approvals remain valid. Future escalation/approval in legacy contracts now means Astra. New evidence records actual GPT participation and distinct implementation/verification ownership in the GPT participation ledger; it does not create an Ollama utilization requirement.

Read [docs/README.md](./docs/README.md), [CONTEXT.md](./CONTEXT.md), the applicable ADRs in `docs/adr/`, the applicable approved specs in `docs/specs/`, and [docs/agent-operating-model.md](./docs/agent-operating-model.md) before working.

## Documentation authority

- `CONTEXT.md` owns project vocabulary only; it must not contain implementation requirements.
- `docs/canon/` owns the current game, narrative, and art truth.
- `docs/adr/` records consequential decisions and their rationale; superseded ADRs remain as history.
- `docs/specs/` owns normative, testable behavior. Implementation may begin only for a spec whose status is `Approved`.
- `wiki/` is a publication and navigation view. It never overrides canon, ADRs, or specs.
- Every implementation change must cite requirement IDs and every verification result must cite acceptance-criterion IDs.
