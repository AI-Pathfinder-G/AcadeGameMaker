# Multi-Agent Operating Model

## 현재 구현 우선순위 — 2026-10-03

최신 사용자 지시와 [ADR-0042](./adr/0042-chapter1-playable-first.md)에 따라 최우선 결과는 [1챕터 전체 체험 전달](./specs/work-contracts/2026-10-03-chapter1-playable-delivery.md)이다. 기존 최초 로컬 체험을 그 기반 단계로 재배치하고 원정·선택·기술·서사·실패 복귀·현재 빌드까지 이어서 구현한다. 작업판의 P0-CH1~P3-CH1-DELIVERY를 적용하며 전체 데모 완료 후에야 1챕터 작업을 시작하는 순서를 사용하지 않는다. 우선순위 변경과 각 제품 단계의 구현 승인은 구분한다.

[실제 플레이 우선 전략](./specs/work-contracts/2026-10-03-play-first-delivery-strategy.md)의 Approved 절차와 [ADR-0039](./adr/0039-play-first-integration-and-bounded-recovery.md)를 적용한다. 최신 작업판·REQ-PROC/AC-PROC는 전략 문서 한곳에 유지한다. 아스트라는 다음 주력으로 최초 로컬 체험의 제품 계약·실제 의존 조사를 배정하고, 솔은 영향 조사/승인된 구현, 루나는 독립 검수를 맡는다. 제품 구현은 해당 별도 계약 Approved 이후다.

동일 오류의 큰 재실행은 최소 재현·변경 근거 없이 반복하지 않는다. 현재 Q0 인간 승인·필수9회·게시/키/세대 관문과 동결 증거는 유지한다. [13사례 세부 감사](./verification/implementation-audits/2026-10-03-03.md)를 근거로 실제 생산 정상 경로 사전검사·영향별 회귀/증거재사용·진행 관측·첫 기본 아트 패키지 수용을 적용한다. 절차 변경을 게임 구현 진척에 더하지 않는다.

## 구현 진척 감사 — 2026-10-03

사용자 명시 지시에 따라 구현 감사는 [고정 기준](./qa/implementation-audit-standard.md)과 [서식](./qa/implementation-audit-template.md)을 사용한다. 아스트라는 아트·소스·유니티 장면의 목표 목록과 퍼센트 근거, 이전 대비 증감 및 현재 작업을 보고한다. 솔 구현 담당은 실제 산출물·실행 사실을 제공하고 루나의 기존 독립 증거와 미검증을 구분한다. 최신 사용자 지시에 따라 매일 한국시간 오전 8시 하루 한 번 전일 대비 감사를 수행하고 구현부·중요 체크포인트별 진척 감사는 하지 않는다. 별도 사용자 감사 요청은 예외다. 작업 증거 기록과 필수 독립 검수는 유지하며, 일일 보고가 승인 계약이나 독립 수용을 대체하지 않는다. [최신 감사 색인](./verification/implementation-audits/README.md)에 기록한다.

## 현재 모델 배정 — 2026-10-07

모델 버전의 활성 선택 규칙은 [ADR-0041](./adr/0041-automatic-model-version-upgrade.md), 현재 역할의 기준은 [ADR-0043](./adr/0043-remove-terra-role.md)이다. 테라 역할과 테라 이름의 신규·재시도·대체 작업자 배정을 제거하고 설계·구현은 솔에 통합한다. 아래 값은 현재 기준이며, 실제 호출 목록에 같은 계열의 상위 안정 숫자 버전이 제공되면 새 배정 직전에 재승인 없이 올린다. 매시간 모델 기준 자동갱신 예약 `automation-2`는 2026-10-07 사용자 요청으로 삭제했으며 재생성하지 않는다. 새 배정 직전 확인은 유지한다. 실행 중인 작업·과거 실제 모델·동결 승인 원문은 보존한다.

아스트라는 `gpt-6-astra`, 솔은 `gpt-6.1-sol`, 루나는 `gpt-6-luna`를 사용한다. 솔의 신규 작업·재시도·대체 호출에 `gpt-6-sol` 작업자를 재개하지 않는다. GPT-5.6 모델은 모든 역할에서 호출·재개하지 않는다. 솔 과업은 필요한 경우 서로 다른 작업자가 담당하지만 테라라는 별도 역할이나 이름을 다시 만들지 않는다. 구현과 루나 독립 검수의 소유 분리는 유지한다. [ADR-0040](./adr/0040-sol-terra-gpt61-routing.md), [ADR-0037](./adr/0037-gpt6-role-model-migration.md)과 과거 증거의 실제 모델·역할 표기는 역사로 보존하며 소급 변경하지 않는다.

## Previous routing decision — 2026-09-15

[ADR-0032](./adr/0032-gpt-terra-luna-subagent-standard.md)의 당시 배정은 역사이며 현재 역할은 ADR-0043을 따른다. 외부 모델 금지는 유지하고 현재 작업은 아스트라·솔·루나 계열만 사용한다. Spark는 실제 도구에 제공되는 경우만 사용하며 제공되지 않으면 루나가 검증한다. 과거 ADR과 증거는 당시 사실을 보존한다.

## 현재 운영 역할 — 2026-10-07

[ADR-0043](./adr/0043-remove-terra-role.md)이 현재 인력 배정 기준이다. [ADR-0027](./adr/0027-astra-orchestration-and-gpt-only-delivery.md)의 과거 승인·증거와 계약의 행동 요구는 보존하며 이후 테라 담당 배정만 솔 구현 담당으로 대체한다.

| 역할 | 담당 |
|---|---|
| 아스트라 | 총괄, 전역 계약·설정, 승인과 최종 통합 |
| 솔 | 한정 설계·계약 초안·반대 검토, 구현·콘텐츠·구현 검사·시험 자료·검사기·편집기 및 재현 도구 |
| 루나 | 사전·사후 독립 검수, 코드·서사 검토, 회귀·빌드 검증과 증거 |

흐름은 아스트라 범위 설정 → 솔 영향 조사·루나 실패 사례 검토 → 아스트라 Approved → 솔 구현 → 루나 독립 검증 → 아스트라 최종 통합이다. 복잡한 설계의 추가 솔 검토는 필요한 경우 배정하며 매번 직렬 선행 단계로 강제하지 않는다. 구현자는 자기 구현의 독립 수용자가 될 수 없다.

No Ollama assignment is active or eligible for reassignment. Future implementation and QA work is dispatched to the GPT roles above, with GPT participation and distinct implementation/verification ownership recorded in new evidence. Historical artifacts and approval dates stay intact; old behavioral contracts remain binding. Future approvals and escalations under old contracts belong to Astra.

2026-10-11 사용자 지시로 최대 병렬 배정과 아스트라·솔 ultra/루나 max 강제 설정을 해제한다. 서브에이전트는 실제 과업에 필요한 만큼만 배정하고 동시 한도를 채우거나 한도 소진을 목표로 늘리지 않는다. 신규 배정에는 상향 추론 설정을 지정하지 않고 호출 도구의 기본값을 사용한다. 모델 계열·현재 기준 버전·테라 제거와 공유 파일 단일 소유·Approved 계약·구현 및 독립 검수의 분리는 유지한다. 과거 실행의 실제 설정은 보존한다. 주 대화의 설정을 변경했다고 주장하지 않는다.

Pro는 추론 설정과 구분한다. 실제 지원될 때만 사용으로 기록하며 현재 서브에이전트 도구에는 Pro 모드 제어가 없다. 별도 유료 API 경로는 사용하지 않는다.

## Approval record

탄력적 본 개발은 활성 전략 REQ/AC-PROC-012를 따른다. 한 기능 묶음의 정확 범위·소유·유한 검사/국소 재실행 예산을 승인하고 범위 내 보정은 이어 진행한다. 새 공유 API/저장/캐논/권한·예산 확대만 실제 경계로 재승인하며, 작은 보정마다 계약·전체 검수·새 도구를 만들지 않는다. 증거는 실제 실행 입력과 최종 제출 상태에 결속하고 기존 정확 핀/단회와 충돌하는 부분은 한정 호환 부록에서 한 번 정리한다. 앱 동의나 게시로 막힌 경로 밖의 충돌 없는 기승인 본 개발은 계속한다. 실제 G3 방 수용과 B1 최종 전 C1C 공유 통합 제한은 유지한다.

2026-10-08 사용자 지시에 따라 독립 검수 요청은 활성 전략의 M1 원정 연결/M2 보스 이후/M3 최종 전달 묶음의 제출 시점으로 모은다. 구현 중에는 필요한 컴파일·국소 검사를 수행하되 작은 개발건/허용 범위 내 보정마다 동일 독립 검수·전체 회귀·통합 승인 문서를 새로 발급하지 않는다. 구현자와 독립 검수자의 분리·기존 필수 AC는 유지하며 한 작업 원장에서 보정과 영향 재검수를 관리한다. 정확 기존 계약과 충돌하는 주기 보정은 한정 호환 계획으로 처리한다. 진척 감사의 일일 오전8시 주기는 별도다.

각 작업은 [스펙 규칙](./specs/README.md)의 상태와 식별자 규칙을 따른다. 아스트라가 소유 스펙을 `Approved`로 승인하고 솔 구현 담당은 인계에 `REQ-*`, 루나는 검증 보고에 `AC-*`와 실제 증거를 기록한다. 통합 수용은 아스트라가 기록한다. 식별자나 상태 누락은 승인으로 간주하지 않는다.

## Source

- [ADR-0043](./adr/0043-remove-terra-role.md)
- [ADR-0027](./adr/0027-astra-orchestration-and-gpt-only-delivery.md)
