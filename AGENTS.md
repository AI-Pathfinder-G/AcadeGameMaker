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
