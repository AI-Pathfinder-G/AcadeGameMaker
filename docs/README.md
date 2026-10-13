# Documentation Map

- 2026-10-14 최신 세션: [1챕터 직접 플레이 준비 인계](./handoffs/2026-10-14-chapter1-session-handoff.md) · [새 세션 시작 프롬프트](./handoffs/2026-10-14-next-session-prompt.md). 기록 관리자 보정·국소 검사·원문 전달을 확인한 뒤 사용자가 플레이를 연기하여 세션을 정리했다. 실제 플레이·B1 FINAL·CH1 완주는 미완료이며 새 실행에는 새 동적 신원과 정확 권위가 필요하다.

- 탄력적 본 개발: [활성 전략](./specs/work-contracts/2026-10-03-play-first-delivery-strategy.md)의 REQ/AC-PROC-011/012를 따른다. 기능 묶음 한 승인·범위 내 연속 보정·유한 국소 검사 예산·M1/M2/M3 독립 검수·한 작업 원장으로 진행한다. 작은 개발건마다 승인/전체 회귀/문서 지문을 재발급하지 않으며 기존 실제 선행과 권한 충돌은 한 번의 호환 계획으로 닫는다.

- 최우선 전달 목표: 사용자가 본편 1챕터를 직접 체험할 수 있는 현재 소스의 Windows 빌드다. [CH1 전달 계약](./specs/work-contracts/2026-10-03-chapter1-playable-delivery.md), [현재 작업판](./specs/work-contracts/2026-10-03-play-first-delivery-strategy.md), [ADR-0042](./adr/0042-chapter1-playable-first.md)를 따른다. 기존 단일 전투 체험은 입력·표시·장면 수명주기를 연결하는 첫 단계이며 1챕터 완주를 대신하지 않는다. 해당 제품 단계가 Approved인 범위만 구현한다.

- 모델 상위 버전 자동 적용: [ADR-0041](./adr/0041-automatic-model-version-upgrade.md)에 따라 실제 호출 목록에 같은 계열의 상위 안정 버전이 제공되면 재승인 없이 다음 배정 기준을 올린다. 새 배정 직전 확인은 유지한다. 매시간 모델 기준 자동갱신 예약은 2026-10-07 사용자 요청으로 삭제했으며 재생성하지 않는다. 역사·동결 계약·독립 검수의 실제 모델은 보존한다.

- 현재 모델 배정: [ADR-0043](./adr/0043-remove-terra-role.md)과 [운영 모델](./agent-operating-model.md)에 따라 테라 역할을 제거하고 솔 `gpt-6.1-sol`이 설계·구현, 아스트라 `gpt-6-astra`가 승인·통합, 루나 `gpt-6-luna`가 독립 검수를 맡는다. 이후 솔의 `gpt-6-sol` 및 모든 역할의 GPT-5.6 호출·재개는 금지한다. [ADR-0040](./adr/0040-sol-terra-gpt61-routing.md)과 아래 모델 참여·실행 증거는 당시 실제 모델·역할의 역사다.

- 현재 구현 주력: [실제 플레이 우선 전략·작업판](./specs/work-contracts/2026-10-03-play-first-delivery-strategy.md) · [ADR-0039](./adr/0039-play-first-integration-and-bounded-recovery.md) · [13사례 비효율·반복 감사](./verification/implementation-audits/2026-10-03-03.md). 최초 로컬 체험 계약·의존 조사를 다음 작업으로 배정한다. 기존 Q0는 기술 준비 완료·인간 운영 승인 대기이며 제품 연결과 실제 선행 조건을 분리한다. 아래 게시 이력은 각 당시 단계이며 현재 제품 우선순위를 대신하지 않는다.

- [구현 진척 감사 기준](./qa/implementation-audit-standard.md) · [고정 서식](./qa/implementation-audit-template.md) · [최신 감사 색인](./verification/implementation-audits/README.md) — 2026-10-03 최신 사용자 지시. 매일 한국시간 오전 8시 하루 한 번 아트·소스·유니티 장면의 세부 퍼센트와 전일 대비 증감을 보고한다. 구현부·중요 체크포인트별 진척 감사는 중단하며 별도 사용자 요청은 예외다. 근거 없는 기존 전체 추정치는 비교 기준으로 사용하지 않는다.

- [조건부 현재 바이트 실행 계약](./specs/work-contracts/2026-10-02-q0-trusted-child-execution.md) · [입력 기한 보정](./specs/work-contracts/2026-10-02-q0-input-deadline-correction.md) · [준비 결과의 계획 결속](./specs/work-contracts/2026-10-02-q0-preparation-plan-binding.md) · [입력 분류 보정](./specs/work-contracts/2026-10-02-q0-input-disposition-correction.md) — 현재 준비의 실제 입력 대기 결함과 첫 분류 원장 오류를 보정한다. 실패 기록은 보존하며 키 생성·실행은 각각 독립 검수와 기술 관문을 충족한 뒤 진행한다.

- [Q0 준비 한정 수용](./approvals/2026-10-02-q0-cleanclone-preparation-limited-acceptance.md) · [현재 렌더 정적 원장 수용](./approvals/2026-10-02-renderer7-current-scope-limited-acceptance.md) · [외부 권위 발급 승인 계약](./specs/work-contracts/2026-10-02-q0-external-authority-issuance.md) — 실제 아스트라 검토를 마쳤고 다음 다섯 소스의 준비 구현·독립 검수로 이어간다. 현재 계획은 실행 미승인이고 새 원격 게시와 실사용 서명은 아직 수행하지 않았다.

- [정상 갱신 뒤 조명 대조 수용](./approvals/2026-10-02-renderer-normal-update-v6-limited-acceptance.md) · [내장19참조 객체 식별 수용](./approvals/2026-10-02-builtin-reference-object-limited-acceptance.md) · [두 단계 발급 핵심 수용](./approvals/2026-10-02-q0-issuer-v2-limited-acceptance.md) · [Q0 격리 구현 준비](./specs/work-contracts/2026-10-02-q0-cleanclone-signed-preparation.md) — 루나 독립 검수 뒤 다음 시험/QA 구현으로 이어간다. 렌더 패스 내부 소비·외부 권위 발급·전체 회귀·게시 수용은 별도다. 아래 진행 상태는 각 작성 당시 기록이다.

- [참조 분류 제한 수용](./approvals/2026-10-02-reference-classification-limited-acceptance.md) · [분류 독립 검수](./verification/2026-10-02-reference-classification-luna-review.md) — 미해석388행과 범위 밖68경로의 소유·생성물·부분 경로를 대조했다. 패키지 소유와 참조 분류만 수용하며 전체 의존 폐쇄는 미입증이다.
- [렌더 진단 v3](./verification/2026-10-02-renderer-current-use-v3-terra-report.md) · [Q0 서명 검증 핵심 구현 계약](./specs/work-contracts/2026-10-02-q0-signed-authority-core-implementation.md) — 실제 Renderer2D 카메라·렌더 요청·콜백·출력 픽셀을 관측했다. falloff 소비와 APV 조건부 사용은 미입증이며 독립 검수를 진행한다. Q0는 실사용 승인 루트 미발급을 명시하고 격리 검증 핵심을 구현한다. 코드 게시나 전체 수용을 뜻하지 않는다.

- [501개 조사·새 원격 객체 증거 제한 수용](./approvals/2026-10-02-content-survey-evidence-limited-acceptance.md) · [원격 보완 루나 독립 결과](./verification/2026-10-02-remote-object-correction-luna-review.md) — 실제 고정개정GET의 커밋/트리/501집합·7본문을 재현해 원격 지문 기록P1을 새 관측 범위에서 보완했다. 원본 오류·충돌과 미해석388개는 보존한다. 내용/간접 의존 폐쇄·Q0·렌더러·전체 회귀·게시는 별도다. 아래 배정/진행 표기는 각 당시 상태다.

- [원문 보존 도구 제한 수용](./approvals/2026-10-02-raw-lineage-limited-acceptance.md) · [실제 실행·구현 보고](./verification/2026-10-02-publication-raw-lineage-terra-report.md) · [루나 독립 검수](./verification/2026-10-02-raw-lineage-luna-review.md) — 새 도구의 최종 합성 정상1·음성4와 후보 내부 출력 거절, 실제534 대조·25 원문 복원·전후 변경0을 확인했다. 독립 보고의 합성 행수 오기는 수용 문서에서 실제2파일/전이1과 실제534/전이25로 명시 정정했다. 현재 원문 보존 기능만 수용하며 역사14전이·전체 의존·게임 회귀·게시는 별도다.

- [정규화 준비 보정 후 제한 수용](./approvals/2026-10-02-canonical-preparation-limited-acceptance-v2.md) · [501개 조사 기계 대조](./verification/2026-10-02-content-survey-astra-mechanical-check.md) — 루나 재호출에 성공했고 준비 보고의 분류·지문 오기를 보정했다. 501개 내용·의존 조사 결과의 독립 검수와 25개 원문 바이트 보존 도구 구현을 진행한다. 과거 한도 대기는 해소됐으며 전체 회귀·게시 수용은 별도다.

- [루나 재호출 성공·후속 재개](./handoffs/2026-10-02-luna-recovery-resume.md) · [출처v2 제한 수용](./approvals/2026-10-02-forward-provenance-v2-limited-acceptance.md) — 실제 `gpt-6-luna`가 미완료 출처 검수를 끝내 P0/P1=0/0을 기록했다. 정규화534·검사기의 독립 검수와501 정식 내용·의존 조사를 재개했다. 아래 한도 대기 문구는 과거 단계이며 전체 회귀·게시 수용은 아직 별도다.

- [필수 입력 선별 대조](./verification/2026-10-02-code-first-input-selection-astra-check.md) · [534개 후보 대조](./verification/2026-10-02-publication-lf534-astra-check.md) · [엄격 검사기 결과](./verification/2026-10-02-canonical-baseline-validator-astra-check.md) — 사용자 위임으로523개를 선별해29개를 추가한534 후보와 새 검사기를 작성했다. 정상2·음성8, 실제505/534 전수 대조가 통과했고2067 경계행·메타7의 전후 지문 차이0이다. [역사 보존·다음 관문](./approvals/2026-10-02-canonical-regression-preserve-history-gate.md)에 따라 새 회귀·독립 수용·게시를 구별한다. 루나 관문은 한도 회복 뒤 별도다.

- [코드·필수 입력 우선 위임 적용](./approvals/2026-10-02-code-first-publication-scope-decision.md) · [523개 선별 계약](./specs/work-contracts/2026-10-02-code-first-input-selection.md) — 사용자가 판단을 위임해 코드·필수 검증 입력부터 선별한다. 실제 참조가 있는 미디어만 개별 후보로 검토하며 통상 범위 선택을 반복 질문하지 않는다. 아래 범위 질문은 위임 전 이력이다.

- [505개 후속 작업 인계](./handoffs/2026-10-02-lf505-publication-checkpoint.md) · [일곱 참조 실제 조회](./verification/2026-10-02-renderer-guid7-loaded-probe-astra-check.md) · [최신 원격·전체 검증 입력 범위](./verification/2026-10-02-lf505-remote-and-v19-scope-precheck.md) — 두 추천안을 적용해 후보505·QA 검사·Unity 가져오기 진단을 마쳤다. 일곱 GUID의 현재 로드 상태는 미해석이며 기존 P1을 유지한다. 과거 전체 검증 입력 중 후보/원격에 없는523개에 대해 코드·필수 입력 우선 선별 또는 미디어 포함 전체 검토 범위를 사용자에게 요청했다. 아래 진행 문구는 당시 이력이다. 게시 승인은 없다.

- [505개 Unity 진단 기계 대조](./verification/2026-10-02-lf505-unity-import-astra-check.md) · [Renderer2D 로드 참조 진단 계약](./specs/work-contracts/2026-10-02-renderer-guid7-loaded-probe.md) — 새 바이트 복사본의 Unity 한 번 실행은 종료코드 0, 자산 가져오기·C# 오류 미검출, 후보 변경 0이다. 라이선스 토큰·컴파일 캐시·종료 통신 오류는 보존한다. 일곱 GUID의 실제 로드 상태는 별도 진단 중이며 독립 수용·게시 승인은 없다. 아래 진행 문구는 각 당시 단계다.

- [게시 바이트·QA 범위 사용자 결정](./approvals/2026-10-02-publication-byte-and-qa-scope-decision.md) · [505개 새 후보 계약](./specs/work-contracts/2026-10-02-publication-lf505-baseline.md) — 2026-10-02 사용자가 두 추천안을 선택했다. 저장소 줄바꿈 기준으로 새 바이트를 검증하고 QA 검사기·직접 입력 네 개를 함께 검토한다. 원문 501개와 과거 검수는 보존하며, 아래 결정 대기 문구는 선택 전 조사 이력이다. 새 후보 작성·검증은 진행 중이다.

- [505개 후보 기계 대조](./verification/2026-10-02-publication-lf505-astra-check.md) · [Unity 한 번 진단 계약](./specs/work-contracts/2026-10-02-lf505-unity-import-diagnostic.md) — 정확 505개·CRLF→LF 18개·나머지 487개 원문 동일을 확인했다. 후보의 QA 일반·자체 검사는 종료코드 0이다. 별도 실행 복사본의 Unity 진단은 진행 중이며 독립 수용·게시 승인은 남아 있다.

- [501파일 첫 Unity 격리 진단](./verification/2026-10-01-initial-code-publication-provisional-501-unity-import-astra-check.md) · [짧은 경로 재진단](./verification/2026-10-01-initial-code-publication-short-path-unity-recheck-astra-check.md) · [Git 줄바꿈 정규화 예비 대조](./verification/2026-10-01-initial-publication-git-canonicalization-precheck.md) — 첫 실행은 패키지 자산 접근 오류를 동반했고, 짧은 경로 재실행은 라이선스·패키지 관리자 연결 실패로 가져오기 전에 종료됐다. 경로 원인은 미판정이다. 501개 중 18개는 현 바이트가 Git 기록 과정에서 줄바꿈 정규화되므로 게시 바이트 기준을 별도 결정해야 한다. 코드 게시·전체 수용은 없다.

- [QA 목록 검사기 후보 밖 입력 예비 대조](./verification/2026-10-01-initial-publication-qa-catalog-dependency-precheck.md) — 후보 안의 검사기가 읽는 네 입력은 모두 후보 밖이며, 필수 차단 ID 원장은 원격에 없다. 다른 두 입력에는 원격과 실제 내용 차이가 있다. 파일별 추가 범위·병합 판단은 미완료다.

- [후속 예비 조사 인계](./handoffs/2026-10-01-forward-content-precheck-checkpoint.md) · [기반 33파일 계약](./specs/work-contracts/2026-10-01-initial-code-publication-foundation-33-precheck.md) · [메타·조립 정의 계약](./specs/work-contracts/2026-10-01-initial-code-publication-meta-asmdef-precheck.md) · [패키지 캐시 소유 계약](./specs/work-contracts/2026-10-01-initial-code-publication-package-owner-precheck.md) · [잔여 식별자 조사 계약](./specs/work-contracts/2026-10-01-initial-code-publication-residual-guid-precheck.md) · [전체 내용·의존성 조사 초안](./specs/work-contracts/2026-10-01-initial-code-publication-forward-content-dependency-draft.md) — 루나 v2 최종 검수는 한도 회복 후 별도로 진행한다. 로컬 501개 현 바이트와 네 예비 조사 지문은 일치했다. 후보 메타에서 소유를 찾지 못한 참조291행 중 현 패키지 캐시에서 265행의 단일 소유를 찾았고, 남은 26행은 네 현 로컬 메타 검색 범위에서 미발견이다. 패키지 요청/잠금 버전 차이 두 건은 [기존 승인 문맥](./verification/2026-10-01-package-version-authority-crosscheck.md)과 함께 보존한다. 전체 계약은 Draft이며 독립 검수·정식 의존 폐쇄·파일별 게시 승인은 아직 없다.

- [최초 코드 게시의 현재 바이트 기준 결정](./adr/0038-forward-provenance-for-initial-code-publication.md) · [읽기 전용 출처 조사 계약](./specs/work-contracts/2026-10-01-initial-code-publication-forward-provenance.md) · [v2 검증 인계](./handoffs/2026-10-01-forward-provenance-v2-verification-checkpoint.md) — 사용자가 새 현재 바이트 기준을 선택했다. 과거 428개 공백과 14개 미입증 전이는 보존한다. v2 원장은 작성됐지만 루나 최종 결과 검토가 사용량 한도로 중단돼 파일별 게시 허용 목록과 코드 게시 승인은 아직 없다.

- [C3·C4 v19 공동 최종 수용](./approvals/2026-10-01-c3-c4-v15-joint-final-integration-acceptance.md) · [9회 실행 독립 검수](./verification/2026-10-01-c3-c4-v15-final-independent-luna-review.md) · [게시 근거 색인](./verification/2026-10-01-c3-c4-publication-evidence-index.md) — 현재 승인 계약 범위에서 C3 잔여 AC007/008과 C4 AC001..010을 수용했다. 1654건의 실행 합계와 C4 188행, C3 내부 377행을 대조했다. 게임 코드의 최초 원격 게시는 별도 파일별 출처·복제 검증 문턱을 유지한다. 아래 진행·미수용 표기는 각 당시 기록이다.

- [C4 발급 후 미반환 경계 증거 개정 — Approved](./specs/work-contracts/2026-09-30-c4-issued-unreturned-boundaries-amendment.md) · [추가 부모·Busy 출처 개정 — Approved](./specs/work-contracts/2026-09-30-c4-unreturned-parent-and-busy-source-amendment.md) — 일곱 실제 실패 경계를 원인·증거 도달·CWT 소비 상태별로 제한하고, 동일 손상 본문의 추가 공개 부모와 직접 읽을 수 없는 C1 Busy의 source 근거를 구별한다. 독립 정적 검수와 한정 구현 승인을 마쳤고 실제 행·Unity·통합 수용은 남아 있다.

- [C4 C1 미호출 실패 증거 개정 — Approved](./specs/work-contracts/2026-09-30-c4-pre-c1-failure-evidence-amendment.md) — 실행 기록 발급 뒤 C1 전 일곱 경계의 소비 전후와 실제 미관측 값을 구별한다. 독립 재검수 후 한정 구현을 승인했으며 실제 실행·수용은 남아 있다.

- [C4 일반 저장 차단 미관측 증거 개정 — Approved](./specs/work-contracts/2026-09-30-c4-ordinary-writer-observation-amendment.md) — 같은 사례의 실제 저장 호출이 없는 경우 쓰기 차단 여부만 미관측으로 기록한다. 별도 실제 저장 관측과 다른 정상 증거 요건은 유지하며 구현·검증은 진행 중이다.

- [C4 발급 후 C2 미호출 실패 증거 개정 — Approved](./specs/work-contracts/2026-09-30-c4-pre-c2-failure-evidence-amendment.md) — 네 실패 경계의 실제 미관측 세대·proof를 정상 권한이나 부재로 오인하지 않는 한정 규칙이다. 독립 설계 검수 후 관련 구현을 승인했으며 최종 증거·실제 실행·수용은 남아 있다.

- [C4 발급 후 결과 미발급 세대 증거 개정 — Approved](./specs/work-contracts/2026-09-30-c4-issued-generation-evidence-amendment.md) — 오류 뒤 실제 실행 원장과 정상 영수증을 부재나 정상 전체 결과로 오인하지 않는 한정 검증 규칙이다. 독립 설계 재검수 후 관련 fixture·검증기 구현을 승인했으며 실제 실행·수용은 남아 있다.

- [C4 내부 인증·생명주기 연결 개정 — Approved](./specs/work-contracts/2026-09-29-c4-fresh-checkpoint-accessibility-amendment.md) · [시험 제어 접근성 개정 — Approved](./specs/work-contracts/2026-09-29-c4-edit-control-accessibility-amendment.md) · [선행 행 출력 개정 — Approved](./specs/work-contracts/2026-09-29-c4-legacy-row-output-amendment.md) · [선행 선택 형식 개정 — Approved](./specs/work-contracts/2026-09-29-c4-predecessor-selection-format-amendment.md) — 기존 승인 범위의 한정 내부 연결과 원본 형식 보존만 개정한다. 독립 정적 설계 검수와 구현 승인을 완료했으며 도구 구현 보정·최종 소스·실제 실행·통합 수용은 진행 중이다. 새 게시 완료를 뜻하지 않는다.

- [C4 r4 실행 연결 — Approved, 구현 진행](./specs/work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md) · [QA 증거 규약 r2 — Approved](./specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md) · [구현 계약 승인](./approvals/2026-09-29-c4-r4-implementation-contract-approval.md) · [연속 작업 기록](./handoffs/2026-09-29-c4-continuous-implementation.md) — 현재 후속 구현의 소유 규격이다. 두 규격의 독립 정적 검수 P0/P1=0/0 후 제한 구현을 승인했으며 실제 실행·통합 수용은 남아 있다. 아래 C4 Review와 이전 승인 대기는 당시 이력이다.

- [C3 R11 실행 직전 단계 — 수용 완료](./approvals/2026-09-29-c3-r11-pre-c4-integration-acceptance.md) — 집중 편집240·실행15와 필수562/51/610을 같은 소스로 실제 검증했다. C3 전체 및 실행 결과 기준 AC007/008은 Approved C4의 실제 결과와 공동 최종 검증까지 미완료다. 변경 요청 #3와 기존 위키에 게시된 과거 범위 이후의 새 문서·구현은 아직 추가 게시하지 않았다.

- [GPT-6 역할 전환](./adr/0037-gpt6-role-model-migration.md) · [적용·병합·위키 확인](./approvals/2026-09-29-gpt6-role-routing-acceptance.md) — 솔·테라 역할은 별도 `gpt-6-sol` 작업자, 루나는 `gpt-6-luna`로 실제 재시도에 응답했다. 현재 도구에 `gpt-6-terra`는 없으며 5.6 새 호출·재시도는 금지한다. 모델 운영 문서의 변경 요청 #2와 위키 게시를 완료했다. 아래 사용량 중단은 이전 모델의 이력이다.

- [병합 이후 C3 구현 인계](./handoffs/2026-09-29-merged-c3-lower-checkpoint.md) — GPT-6으로 하위 보정과 상위 확인·취소·재무장 일관 단위를 구현했다. 초기 상위 검수의 늦은 예약 재사용과 필수 시험 누락을 보완 중이며, 새 소스 실행·필수 회귀·독립 수용은 남아 있다. 아래 인증 차단·한도 중단·초안 상태와 이전 수용 지문은 당시 기록이다.

- [문서 정리 변경 요청 #1 — 원격 병합 완료](https://github.com/AI-Pathfinder-G/AcadeGameMaker/pull/1) · [실제 병합 기록](./verification/2026-09-29-documentation-merge-execution.md) — 사용자 지시로 정확한 원본을 원격 main에 병합했다. 아래 인증 차단·변경 요청 부재는 이전 기록이며 로컬 게임 코드·작업물은 보존했다. 다음 Approved C3 합성 구현을 진행한다.

- [관찰 단위 수용 이후 작업 기준](./handoffs/2026-09-29-c3l-verified-checkpoint.md) — 조회 함수 사용자 승인과 C3L 수용을 완료했다. 실제 변경 요청 인증 갱신, C3 소유자·재무장 구현과 후속 검증이 남아 있다. 이전 인계서의 미답변 승인 대기는 당시 기록이다.

- [C3L 관찰 단위 — Verified](./approvals/2026-09-29-c3l-integration-acceptance.md) · [최종 독립 수용 검수](./verification/2026-09-29-c3l-final-regression-luna-acceptance-review.md) · [분할 실행 대조](./verification/2026-09-29-vd09-m5d7q-c3l-r6-regression-closure.md) — 집중 11개와 같은 소스의 필수 회귀 고유 613개를 검증해 관찰 단위만 수용했다. R4 시간 초과 이력은 보존했으며 C3 전체·소유자·C4는 미수용이다. 아래 회귀 진행·실패·미검증 표기는 당시 기록이다.

- [C3L 필수 회귀 R4 실제 결과](./verification/2026-09-29-vd09-m5d7q-c3l-r4-execution.md) — 예정 562개 중 561개 통과, 기존 알림 검사 1개 시간 초과 실패다. 이름·전후 입력 지문 차이는 0이지만 전체 수용은 미완료다. 별도 프로세스 51개는 실행 중이며, 아래 R4 진행 중 표기는 당시 기록이다.

- [확인·실행 연결 단계적 수용 결정](./adr/0036-staged-confirmation-and-execution-acceptance.md) · [독립 폐쇄 검수](./verification/2026-09-29-vd09-m5d7q-c3-c4-acceptance-cycle-resolution-luna-closure.md) — 아스트라가 C3 선행 검증과 C4 공동 최종 검증 순서만 승인했다. C3 실행 결과 관련 전체 기준은 미검증이며 C4는 Review를 유지한다. 시험 요구·제품 동작·현재 코드 허용 범위는 완화하지 않았다.

- [C3L 집중 실행 11/11](./verification/2026-09-28-vd09-m5d7q-c3l-focused-execution.md) · [독립 검수](./verification/2026-09-28-vd09-m5d7q-c3l-r3-luna-review.md) · [C3 소유자 설계 승인](./approvals/2026-09-28-c3-owner-implementation-design-approval.md) — 집중 시험의 실패·건너뜀·판정보류 0, 시험 이름과 전후 입력 지문 차이 0이다. 필수 편집 모드 회귀는 진행 중이며 C3L 전체 수용과 C3 본체 구현은 아직 완료하지 않았다. 아래 인계서의 중단·승인 대기는 이전 시점의 기록이다.
- [C3 실행 관리자 조회 함수 사용자 승인](./approvals/2026-09-28-c3-readonly-launch-root-accessor-approval.md) · [Q0 후속 지문 승인](./approvals/2026-09-28-c3-adapter-successor-sha-approval.md) — 이번 세션에서 내부 읽기 전용 함수 추가를 명시적으로 승인받고 정확한 소스 독립 검수와 후속 지문 승인을 완료했다.
- [C3L 관찰 잠금 구분 — Approved, 미검증](./specs/work-contracts/2026-09-28-vd09-m5d7q-c3l-observation-lease-provenance.md) · [C4 초기화 실행 연결 — Review](./specs/work-contracts/2026-09-28-vd09-m5d7q-c4-reset-execution-bridge.md) — C3/C3L 새 검증 후 C4 계약 승인과 구현을 진행한다. 이전 인계서의 승인 대기는 당시 기록이며 위 사용자 승인으로 조회 함수 차단만 해소됐다.

- [다음 세션 인계서](./handoffs/2026-09-28-next-session.md) · [핵심 파일 지문](./handoffs/2026-09-28-workspace-checkpoint.json) — 사용자 요청으로 구현을 종료하고 작업물을 보존했다. C2/C2R 수용 결과와 C3L 미검증 변경분을 구분했으며, 다음 세션은 실행 관리자 내부 조회 함수의 미답변 승인부터 확인해야 한다. 이 인계 요청 자체는 해당 변경 승인이 아니다.

- [M5D7Q-C2/C2R 최종 독립 검수 — PASS](./verification/2026-09-28-vd09-m5d7q-c2-c2r-luna-final-independent-acceptance.md) · [R36 실행·입력 증빙](./verification/2026-09-28-vd09-m5d7q-c2r-r36-execution.md) — 사용자 재개 후 C2/C2R을 Verified로 수용했다. 편집 모드 562+51=613건, 실행 모드 536+74=610건으로 당시 동결 소스를 정확히 대조했다. 실패·건너뜀·판정 불가 0, Luna P0/P1=0이다. C3L의 이후 수정은 아직 미검증이며 C3 구현은 승인 대기 상태로 다음 세션에 인계했다. 아래 이전 중단·미완료 기록은 당시 이력이다.

- [Unity 검사 프로세스 대기 도구 — Verified](./specs/work-contracts/2026-09-28-unity-qa-owned-editor-wait.md) · [Luna 실제 실행 검수](./verification/2026-09-28-unity-qa-owned-editor-wait-luna-r33-actual-review.md) — 사용자가 재개를 지시했다. 보완 도구의 자체 검사 104개와 실제 Unity 집중 검사 32/32를 검증해 QA 도구 범위만 수용했다. 게임 C2/C2R 회귀의 대상 선택 문제와 최종 수용은 별도로 남아 있다.

- [현재 작업분 종료·일시 중단 기록](./verification/2026-09-28-current-batch-pause-checkpoint.md) · [Luna R22 실제 결과 검수](./verification/2026-09-28-vd09-m5d7q-c2r-r22-pause-luna-digest.md) — R22는 614개 중 610개 통과, 별도 실행 정보가 필요한 프로세스 테스트 4개 실패로 종료했다. 의도한 제외 필터가 적용되지 않은 검사 범위 문제이며, 전체 통과로 처리하지 않는다. C2/C2R 최종 수용은 미완료다. 사용자 재개 지시 전에는 재실행·C3 구현·C4 승인·새 검사 실행을 하지 않는다.

- [M5D7Q-C2R 실제 재시작 복구 — Verified](./specs/work-contracts/2026-09-28-vd09-m5d7q-c2r-restart-bootstrap.md) · [최종 독립 검수](./verification/2026-09-28-vd09-m5d7q-c2-c2r-luna-final-independent-acceptance.md) · [실제 프로세스 검증](./verification/2026-09-28-vd09-m5d7q-c2r-process-execution.md) — 초기화 후 복구와 표식 삭제 후 종료·재시작을 실제 프로세스로 검증했다. 현재 편집 모드 613건과 실행 모드 610건의 범위별 회귀를 통과하고 최종 수용했다. 이전 592건의 결과를 현재 수용 근거로 재사용하지 않았으며 실제 메뉴·장면은 연결하지 않았다.

- [M5D7Q-C3 새 게임 확인·취소 소유권 — Approved, synthetic-only](./specs/work-contracts/2026-09-28-vd09-m5d7q-c3-confirmation-owner.md) · [Luna 사전 설계 검토](./verification/2026-09-28-vd09-m5d7q-c3-luna-pregate.md) — 최신 프로필 관찰, 확인·취소 단일 승자와 취소 후 새 입력 세대를 규정한다. 구현·실행·실제 확인 UI 연결은 아직 미검증이다.

- [M5D7Q-C1 새 게임 저장 트랜잭션 — Verified, disk-only](./specs/work-contracts/2026-09-28-vd09-m5d7q-c1-reset-disk-transaction.md) · [실행·Astra 수용 기록](./verification/2026-09-28-vd09-m5d7q-c1-continuation.md) · [Luna 최종 독립 검토](./verification/2026-09-28-vd09-m5d7q-c1-final-luna-review.md) — 단위·장애 212/212, 실제 프로세스 종료·복구 51/51, 프로필 회귀 175/175. R13 집계 도구 조기 종료와 실제 Unity 성공을 구분했다. 실제 메뉴·메모리 적용·확인 UI·장면 전환은 연결하지 않았다.
- [M5D7Q-C2 메모리·입력 전환 — Verified](./specs/work-contracts/2026-09-28-vd09-m5d7q-c2-memory-cutover.md) — 원래 프로필 경로·실행 소유자를 재검증하고 입력과 현재 프로필을 안전하게 전환한다. 루나 독립 검증 후 아스트라가 수용했으며 실제 메뉴나 장면 연결은 별도 계약이다.

- [새 게임 단일 프로필 초기화 확인 — 사용자 승인](./approvals/2026-09-28-new-game-single-profile-confirmation-approval.md) · [ADR-0035 수동 복구 전용 이전 진행](./adr/0035-new-game-manual-only-prior-profile.md) · [M5D7Q-C 초기화 계약 — Approved](./specs/work-contracts/2026-09-28-vd09-m5d7q-c-new-game-reset.md) · [잠금·표식 차단 기반](./verification/2026-09-28-vd09-m5d7q-c-lock-barrier-foundation.md) · [게임 시작 전체 잠금 — 부분 검증](./verification/2026-09-28-vd09-m5d7q-c-launch-lease-evidence.md) — 수동 보관·자동 복구 제외를 고정하고 저장 안전장치와 게임 시작 전체 잠금을 검증했다(EditMode 175/175, PlayMode 36/36). 실제 초기화·확인 UI·인게임 거점 전환은 미완료.

- [M5D7Q-B hub intent handoff — Verified, synthetic-only](./specs/work-contracts/2026-09-28-vd09-m5d7q-b-hub-intent-handoff.md) · [Terra implementation and execution evidence](./verification/2026-09-28-vd09-m5d7q-b-preexecution-evidence.md) · [Luna independent postreview](./verification/2026-09-28-vd09-m5d7q-b-luna-postreview.md) — four menu selections transfer as one typed, one-shot request without executing effects. Host-context Unity focused EditMode `5/5`, PlayMode `4/4`, direct HubPresentation EditMode `68/68`, PlayMode `8/8`; Luna `P0/P1=0` and Astra acceptance. No destination scene, profile mutation, or real wardrobe/gameplay connection is implied.

- [2026-09-13 costume pure-core implementation handoff](./verification/2026-09-13-costume-pure-core-implementation-handoff.md) — REQ-COST-001..009/012/014 additive first slice. 36-entry inventory remains production pending; Unity, media processing, and filesystem adapters are deferred.
- [Costume CIO file adapter — Verified](./specs/work-contracts/2026-09-20-costume-cio-file-adapter.md) — 의상 전용 primary/previous/temp 저장·복구를 분리한다. [Terra 구현 증적](./verification/2026-09-20-costume-cio-implementation-evidence.md)과 [Luna 기존 독립 검토](./verification/2026-09-20-costume-cio-luna-independent-review.md)의 정확한 소스가 유지된 상태에서 후속 전체 Unity 스위트 788/788 EditMode·947/947 PlayMode가 통과했고, [AC-CIO-007 closure — PASS](./verification/2026-09-27-costume-cio-ac007-closure-review.md) 후 Astra가 최종 통합했다.
- [Character sprite and wardrobe readiness audit](./verification/2026-09-20-character-sprite-and-wardrobe-readiness-audit.md) — Luna 독립 감사 결과 세령 조합은 Accepted `0/432`이며 현재 자료는 source-only이다. 저장·표시 프레임워크는 별도 승인으로 진행하되 실제 착용/인게임 바인딩은 첫 미디어 패키지 수용 게이트까지 보류한다.
- [Costume CUA Unity presentation adapter — Approved, synthetic-only verified](./specs/work-contracts/2026-09-20-costume-cua-unity-presentation-adapter.md) · [implementation and Astra acceptance evidence](./verification/2026-09-20-costume-cua-implementation-evidence.md) · [R15 full PlayMode](./verification/2026-09-28-costume-cua-r15-full-playmode-evidence.md) · [Luna independent postreview](./verification/2026-09-28-costume-cua-r15-full-playmode-luna-postreview.md) — `AC-CUA-009: PASS`: focused PlayMode `195/195`, full EditMode `800/800`, and unfiltered full PlayMode `1142/1142`, all with zero failed/skipped/inconclusive. The slice remains synthetic-only: the 36 real Seryeong rows are pending, with no scene, live renderer, real-media import, or catalog promotion implied.
- [ADR-0033 — 세령 64px 미리보기와 32px 인게임 파생본](./adr/0033-seryeong-64px-preview-and-32px-gameplay-derivatives.md) · [결정 검토](./proposals/2026-09-20-seryeong-runtime-density-decision.md) — 사용자가 권고안 C를 승인했다. 현 640×360/18 PPU를 유지하고 64px 옷장 미리보기와 수작업 검수된 32px 양방향 인게임 아틀라스를 한 외형 패키지로 사용한다.
- [Seryeong A1/H1 first vertical-demo media slice — offline pending tooling accepted](./specs/work-contracts/2026-09-20-seryeong-a1-h1-first-media-package.md) — 첫 실제 후보를 A1/H1의 64px 옷장 정지 미리보기와 32px idle 6프레임×좌우로 제한했다. [Terra 구현 증적](./verification/2026-09-20-seryeong-a1-h1-implementation-evidence.md)의 결정론적 진단·검증 도구는 [Luna 최종 독립검수](./verification/2026-09-20-seryeong-a1-h1-luna-independent-review.md)에서 P0/P1/P2 0건으로 닫혀 Astra가 bounded offline tooling 범위만 수용했다. 프로젝트 내 사용 권한은 권리 사슬로 고정됐고 ADR-0034가 A1을 향후 초기 기본 의상으로 정했다. 다만 왼쪽 6프레임 별도 저작, 32px 수동 검수와 Unity publication 전까지 결과는 pending·non-wearable·non-addressable이며, APV3는 `0/432`, catalog는 `NoAcceptedDefault`를 유지한다.

- [SPARK-01-R2 Terra recovery — Verified](./verification/2026-09-10-spark01-r2-terra-recovery-luna-review.md) — SelfTest 실패 재현을 Terra가 회수 구현하고 Luna가 정상·실패 모드와 독립 반례를 재검증. HeaderAndGrid 사전검사 범위 최종 수용.
- [Spark 다음 작업 SPARK-01-R1](./specs/work-contracts/2026-09-09-spark01-r1-correctness-fixes.md) — 최초 구현 독립 검토에서 판정/헤더/배열 결함 발견. bounded Approved 수정계약, 원 구현 미수용. [검토 결과](./verification/2026-09-09-spark01-independent-review.md).
- [Spark 독립 구현 명세 — 스프라이트 사전검사](./specs/work-contracts/2026-09-09-spark-sprite-sheet-preflight.md) — SPARK-01 bounded Approved. 사용자 수동 전달용, 구현·실행 전.
- [관계·진엔딩·상징적 희생자 별도 보고](./proposals/2026-09-09-relationship-ending-and-tragedy-report.md) — 연애 필수, 핵심 신뢰 분기, 문오 제안, CH7 재시도. [후속 스펙](./specs/full-game-narrative/01-relationship-ending-and-retry.md)은 Review.
- [최신 8챕터 이야기·진행 재정렬](./proposals/2026-09-09-eight-chapter-synopsis-realignment.md) — Draft. 서하 4장, 무진의 초반 복선, 7장 승인 분기형 보스전, 4→6장 선택 연속성과 8장 연결. 구현 승인 아님.
- [등장 몬스터·크리처 목록](./canon/creature-roster.md) — 공중 크리처 F1~F6 등록 완료. 후속 스토리·맵 배치 인계 기준.
- [8챕터 적·맵·조작 학습 구성안](./proposals/2026-09-08-eight-chapter-enemy-map-curriculum.md) — 사용자 검토용 Draft. 첫 등장·재등장·안전 대응·서사 분기 보완, 보스 상세 설계는 다음 단계.
- [8챕터 보스 디자인 패키지](./proposals/2026-09-08-chapter-boss-design.md) — Draft. 캐릭터·스토리·공격과 시안 제작용 지시. 이미지 제작과 런타임 구현은 제외.
- [최신 보스 배정·채륜 디자인](./proposals/2026-09-08-boss-roster-unique-revision.md) — 보스 8개체, 라겐만 반복. 오르단 재전투 철회, 5장 신규 보스와 시안 인계 Draft.

이 문서는 프로젝트 문서의 단일 진실원천, 충돌 해결 순서, 변경 절차를 정의한다.

## 읽는 순서

1. [CONTEXT.md](../CONTEXT.md) — 프로젝트 고유 용어
2. [게임 디자인 캐논](./canon/game-design.md), [서사 캐논](./canon/narrative-canon.md), [미술 캐논](./canon/art-direction.md) — 현재 프로젝트의 사실
3. [ADR 색인](./adr/README.md) — 중요한 결정과 이유
4. [스펙 색인](./specs/README.md) — 구현 가능한 계약과 검증 기준
5. [에이전트 운영 모델](./agent-operating-model.md) — 역할과 승인 흐름
6. [위키 홈](../wiki/Home.md) — 발행·탐색용 보기

[제안서 색인](./proposals/README.md)은 검토 이력과 비캐논 초안을 구분해 보여 준다.

## 현재 준비도

- [M5D2 활성 런·실패 판정 코어 계약 — Verified](./specs/work-contracts/2026-09-11-vd05-m5d2-run-failure-arbitration-core.md) · [Luna 계약 사전검토 — PASS](./verification/2026-09-11-vd05-m5d2-contract-pregate.md) · [Terra 구현·R2 실행 증적](./verification/2026-09-11-vd05-m5d2-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-12-vd05-m5d2-luna-independent-review.md) — Unity `6000.6.0f1`에서 집중 EditMode `11/11`, 전체 EditMode `494/494`, 전체 PlayMode `576/576` 통과(모두 fail/skip/inconclusive `0`); P0/P1/P2 없이 Astra 최종 수용 완료. 실제 실패 source와 M5A Unity receipt 연결은 후속 범위다.

- [M5D1 오르단 플레이어블 터미널 전환 계약 — Verified](./specs/work-contracts/2026-09-10-vd07-m5d1-ordan-playable-terminal-transition.md) · [Terra 인계](./verification/2026-09-11-vd07-m5d1-terra-handoff.md) · [Luna 독립 검토](./verification/2026-09-11-vd07-m5d1-luna-independent-review.md) — `-211` terminal requester, 실제 InputRouter·고정 카메라 저작 연결, 런타임 prefab clone 및 fail-closed 경계를 통합했다. Unity `6000.6.0f1`에서 집중 `9/9`·`22/22`·`1/1`, 전체 EditMode `483/483`, PlayMode `576/576` 통과; Luna 독립 검토와 Astra 최종 수용 완료.

- [보스전 구성 결정](./canon/boss-encounter-direction.md) — 초반 한 화면·중반 확장·최종 다단계 거대 보스·진보스 근거리 결전. OD-M5D1-001 해소, [설계 기준](./specs/boss-encounter-framework.md) 반영.

- [M5B3 실제 입력 에셋·공식 생성 래퍼 — Verified](./verification/2026-09-08-vd07-m5b3-device-action-evidence.md) — 전체 EditMode 440/440, PlayMode 435/435; 실제 InputRouter 연결은 후속 단계.
- [M5B4 입력 프레임 원자 적용 — Verified](./verification/2026-09-08-vd07-m5b4-input-frame-evidence.md) — 전체 EditMode 440/440, PlayMode 474/474.
- [M5B5 실제 입력 연결·실행 확인·보스 종료 경계 — Verified](./verification/2026-09-08-vd07-m5b5-input-router-evidence.md) — 전체 EditMode 440/440, PlayMode 539/539, 총 979개 통과. 실제 카메라·메뉴·Run 통합과 플레이 가능한 장면은 후속 단계.
- [M5C1 결정론적 카메라 추적 코어 — Verified](./verification/2026-09-08-vd07-m5c1-camera-follow-evidence.md) — 전체 EditMode474 + PlayMode539, 총1013개 통과. Unity 카메라·장면 연결은 포함하지 않는다.
- [M5C2 실제 고정 방 카메라 연결 — Verified](./verification/2026-09-08-vd07-m5c2-camera-driver-evidence.md) — 전체 EditMode480 + PlayMode567, 총1047개 통과. 완료된 이동 위치 기반 카메라·다음 틱 조준 연결과 픽셀 프로필 제작 설정 검증. 플레이 장면·GPU 시각 검수는 후속 범위.

- [2026-09-08 역할 전환 독립 검토와 보스 종료 실패 모드](./verification/2026-09-08-role-transition-and-m4b3c-pregate.md)
- [2026-09-08 작업 재개 기준 검증 — 기존 보스 저작 테스트 19/19](./verification/2026-09-08-resume-baseline.md)

- [ADR-0032 GPT Terra/Luna 서브에이전트 표준](./adr/0032-gpt-terra-luna-subagent-standard.md) — 2026-09-15 승인. 미래 구현·QA 위임은 GPT Terra/Luna만 사용하며 Ollama 및 기타 대체 모델 호출은 금지한다. 과거 기록은 사실 그대로 보존한다.
- [ADR-0027 Astra 총괄·GPT 전용 역할 재배분](./adr/0027-astra-orchestration-and-gpt-only-delivery.md) — 2026-09-08 승인. 이전 Sol 승인은 유효하며 이후 승인·에스컬레이션은 Astra, 구현은 Terra, 독립 검증은 Luna가 맡는다. 현재 외부 모델 라우팅은 ADR-0032가 명확히 한다.

- [2026-08-24 본격 개발 착수 전 준비도 점검](./project-readiness-audit-2026-08-24.md)
- [2026-08-24 환경 준비 및 제품 기준 승인 기록](./approvals/2026-08-24-environment-and-product-baseline.md)
- [2026-08-24 개발 환경 준비 상태](./environment/setup-status-2026-08-24.md)
- [Sol P0 통합 기본안 — Accepted](./proposals/sol-p0-integration-proposal.md)
- [2026-08-24 P0 통합 승인 기록](./approvals/2026-08-24-p0-integration-approval.md)
- [2026-08-24 P1 이동 계약 승인 기록](./approvals/2026-08-24-p1-movement-approval.md)
- [2026-08-24 P1 입력 구조 승인 기록](./approvals/2026-08-24-p1-input-architecture-approval.md)
- [2026-08-24 P1 포인터 조준 조작 승인 기록](./approvals/2026-08-24-p1-pointer-control-approval.md)
- [2026-08-24 P1 포인터·스틱 타겟 판정 승인 기록](./approvals/2026-08-24-p1-targeting-approval.md)
- [2026-08-24 P1 UI·조준 피드백 승인 기록](./approvals/2026-08-24-p1-ui-and-aim-feedback-approval.md)
- [2026-08-24 P1 런타임 입력 재지정 범위 승인 기록](./approvals/2026-08-24-p1-rebinding-scope-approval.md)
- [2026-08-24 P1 입력 재지정 충돌 정책 승인 기록](./approvals/2026-08-24-p1-rebinding-conflict-approval.md)
- [2026-08-24 P1 단일 프로필 저장 스키마 승인 기록](./approvals/2026-08-24-p1-profile-schema-approval.md)
- [2026-08-24 P1 프로필 원자 저장 순서 승인 기록](./approvals/2026-08-24-p1-profile-atomic-write-approval.md)
- [2026-08-24 P1 프로필 로드 복구 승인 기록](./approvals/2026-08-24-p1-profile-load-recovery-approval.md)
- [2026-08-24 OD-PLAT-001 Luna 독립 검토 — PASS](./verification/2026-08-24-od-plat-001-luna-review.md)
- [2026-08-24 P1 18 PPU·2560×1440 출력 기준 승인 기록](./approvals/2026-08-24-p1-art-density-output-approval.md)
- [2026-08-24 P1 640×360 내부 픽셀 캔버스 승인 기록](./approvals/2026-08-24-p1-art-internal-canvas-approval.md)
- [2026-08-24 P1 고정 16:9 화면 프레임 승인 기록](./approvals/2026-08-24-p1-art-aspect-frame-approval.md)
- [2026-08-24 P1 이동 예측형 고정 배율 카메라 승인 기록](./approvals/2026-08-24-p1-art-camera-behavior-approval.md)
- [2026-08-24 P1 카메라 추적 수치 승인 기록](./approvals/2026-08-24-p1-art-camera-values-approval.md)
- [2026-08-24 P1 픽셀 UI·SDF 텍스트 배율 승인 기록](./approvals/2026-08-24-p1-art-ui-scale-approval.md)
- [2026-08-24 P1 32색 역할 분리 팔레트 승인 기록](./approvals/2026-08-24-p1-art-palette-roles-approval.md)
- [2026-08-24 P1 32색 HEX 팔레트 승인 기록](./approvals/2026-08-24-p1-art-palette-hex-approval.md)
- [2026-08-24 P1 픽셀 윤곽선 승인 기록](./approvals/2026-08-24-p1-art-outline-approval.md)
- [2026-08-24 P1 URP 2D 조명 승인 기록](./approvals/2026-08-24-p1-art-lighting-approval.md)
- [2026-08-24 P1 조준기 exact values 승인 기록](./approvals/2026-08-24-p1-art-reticle-values-approval.md)
- [2026-08-24 OD-ART-001 Luna 독립 검토 — PASS](./verification/2026-08-24-od-art-001-luna-review.md)
- [ADR-0020 히로인 유대·사이드킥·진엔딩](./adr/0020-heroine-bond-sidekick-and-true-ending.md)
- [ADR-0021 AimArc·차지 탄도](./adr/0021-character-aim-arc-and-charged-ballistics.md)
- [2026-08-24 인물 중심 8챕터 시놉시스 승인](./approvals/2026-08-24-character-first-eight-chapter-synopsis-approval.md)
- [ADR-0022 인물 중심 8챕터·자기정당화형 광오](./adr/0022-character-first-eight-chapter-narrative.md)
- [ADR-0026 Qwen 제외·세 개의 Ollama 상시 작업 흐름](./adr/0026-three-lane-ollama-workstreams-without-qwen.md)
- [NAR-00 전체 게임 서사 스펙 — Review](./specs/full-game-narrative/00-spec-index.md)
- [2026-08-24 NAR-00 Luna 독립 문서 검토 — CONDITIONAL](./verification/2026-08-24-nar-00-luna-review.md)
- [BGM 방향·생성본 패키지 — 20곡 방향·20곡 청취 후보](./music/README.md)
- [MiniMax H3 티저 방향·생성본 패키지 — 3종](./video/README.md)
- [2026-08-24 NAR 유대도 수치·표시 상태 승인](./approvals/2026-08-24-nar-bond-state-approval.md)
- [2026-08-24 NAR 세령 핵심 약속 승인](./approvals/2026-08-24-nar-core-promises-approval.md)
- [2026-08-24 NAR 세령 이탈·재합류 규칙 승인](./approvals/2026-08-24-nar-heroine-departure-reentry-approval.md)
- [2026-08-24 NAR 광오 5단계·인간성 연속 보존 승인](./approvals/2026-08-24-nar-arrogance-state-approval.md)
- [2026-08-24 NAR 광오 전환·우선순위 승인](./approvals/2026-08-24-nar-arrogance-transition-approval.md)
- [2026-08-25 P1 AimArc 형상·생명주기 승인](./approvals/2026-08-25-p1-aimarc-geometry-approval.md)
- [2026-08-25 P1 수직 데모 성공 장면 흐름 승인](./approvals/2026-08-25-p1-demo-success-scene-flow-approval.md)
- [GLM 첫 구현 마일스톤 QA 보완안과 Sol 심사](./qa/2026-08-25-glm-milestone-zero-qa-supplement.md) — 역사 기록
- [UI·Art·Run Ollama 외주 패킷과 Sol 선별](./proposals/2026-09-01-ollama-ui-art-run-outsourcing.md) — 역사 기록, 현재 외주 아님
- [8챕터 실제 대화 — Sol 최종 검토본](./proposals/dialogue/2026-08-24-sol-final-eight-chapter-dialogue.md) — 역사 기록: Kimi 초안·GLM 1차 검수·Sol 최종 심사, 사용자 승인 대기
- [Kimi·GLM 검수와 Sol 수용 판단](./proposals/dialogue/2026-08-24-kimi-glm-screening-and-sol-decisions.md) — 역사 기록
- [독립 QA 마스터 플랜](./qa/qa-master-plan.md)
- [Unity Licensing Client 재연결 실패 감사 및 재발 방지 런북](./qa/unity-licensing-runbook.md)
- [Pre-Unity QA 아티팩트 작업 계약](./qa/pre-unity-qa-work-contract.md)
- [2026-08-24 VD-11 Luna 독립 검증](./verification/2026-08-24-vd-11-luna-review.md)
- [2026-08-25 수직 데모 구현 게이트 승인](./approvals/2026-08-25-vertical-demo-implementation-gate-approval.md)
- [2026-08-25 수직 데모 계약 준비도 Luna 독립 검토 — PASS](./verification/2026-08-25-vertical-demo-contract-readiness-luna-review.md)
- [2026-08-25 VD-09 Unity bootstrap 구현 증적](./verification/2026-08-25-vd09-bootstrap-implementation-evidence.md)
- [2026-08-25 VD-09 Unity bootstrap Luna 독립 검토 — PASS](./verification/2026-08-25-vd09-bootstrap-luna-review.md)
- [VD-09 Unity bootstrap 작업 계약](./specs/work-contracts/2026-08-25-vd09-unity-bootstrap.md)
- [Unity 6000.6.0f1 기준 갱신 작업 계약](./specs/work-contracts/2026-09-06-unity-6000-6-baseline-update.md)
- [Unity 6000.6.0f1 기준 갱신 검증](./verification/2026-09-06-unity-6000-6-baseline-update.md)
- [VD-01 결정론적 이동 sandbox 작업 계약](./specs/work-contracts/2026-08-25-vd01-movement-sandbox.md)
- [2026-08-26 VD-02 순수 무게 전이 M1 구현·Luna 검증 — PASS](./verification/2026-08-26-vd02-weight-transfer-m1-evidence.md)
- [2026-08-26 VD-02 Unity M2 계약 Luna 사전 게이트 — PASS](./verification/2026-08-26-vd02-weight-transfer-m2-contract-pregate.md)
- [2026-08-26 VD-02 Unity M2A 고정 수학·LOS 끝점 증적 — PASS](./verification/2026-08-26-vd02-weight-transfer-m2a-evidence.md)
- [2026-08-26 VD-02 Unity M2B1 target registry·production LOS 증적 — PASS](./verification/2026-08-26-vd02-weight-transfer-m2b1-evidence.md)
- [2026-08-26 VD-02 Unity M2B2A observation·movement seam 증적 — PASS](./verification/2026-08-26-vd02-weight-transfer-m2b2a-evidence.md)
- [2026-08-27 VD-02 Unity M2B2B session driver·owner sink 증적 — PASS](./verification/2026-08-27-vd02-weight-transfer-m2b2b-evidence.md)
- [VD-03 M1 결정론적 피해 코어 작업 계약 — Approved](./specs/work-contracts/2026-08-27-vd03-combat-m1-damage-core.md)
- [2026-08-27 VD-03 M1 피해 코어 계약 Luna 사전 게이트 — PASS](./verification/2026-08-27-vd03-combat-m1-contract-pregate.md)
- [2026-08-27 VD-03 M1 결정론적 피해·체력 코어 증적 — PASS](./verification/2026-08-27-vd03-combat-m1-evidence.md)
- [VD-03 M2A 결정론적 기본 공격 코어 작업 계약 — Approved](./specs/work-contracts/2026-08-27-vd03-combat-m2a-basic-attack-core.md)
- [2026-08-27 VD-03 M2A 기본 공격 코어 계약 Luna 사전 게이트 — PASS](./verification/2026-08-27-vd03-combat-m2a-contract-pregate.md)
- [2026-08-27 VD-03 M2A 결정론적 기본 공격 코어 증적 — PASS](./verification/2026-08-27-vd03-combat-m2a-evidence.md)
- [VD-03 M2B Unity 전투 어댑터 작업 계약 — Approved](./specs/work-contracts/2026-08-27-vd03-combat-m2b-unity-adapter.md)
- [2026-08-27 VD-03 M2B Unity 전투 어댑터 계약 사전 게이트 — PASS](./verification/2026-08-27-vd03-combat-m2b-contract-pregate.md)
- [2026-08-27 VD-03 M2B1 Unity 전투 저작·관찰 캡처 증적 — PASS](./verification/2026-08-27-vd03-combat-m2b1-evidence.md)
- [2026-08-28 VD-03 M2B2 Unity 전투 고정 페이즈 통합 증적 — PASS](./verification/2026-08-28-vd03-combat-m2b2-evidence.md)
- [VD-03 M3A 일반 적 결정론적 반응 코어 작업 계약 — Verified](./specs/work-contracts/2026-08-28-vd03-combat-m3a-regular-enemy-reaction-core.md)
- [2026-08-28 VD-03 M3A 계약 Luna 사전 게이트 — PASS](./verification/2026-08-28-vd03-combat-m3a-contract-pregate.md)
- [2026-08-28 VD-03 M3A 일반 적 반응 코어 구현 증적 — PASS](./verification/2026-08-28-vd03-combat-m3a-evidence.md)
- [VD-03 M3B1 일반 적 반응 Unity 어댑터 작업 계약 — Verified](./specs/work-contracts/2026-08-29-vd03-combat-m3b1-reaction-unity-adapter.md)
- [2026-08-29 VD-03 M3B1 계약 Luna 사전 게이트 — PASS](./verification/2026-08-29-vd03-combat-m3b1-contract-pregate.md)
- [2026-08-29 VD-03 M3B1 구현 증적 — PASS](./verification/2026-08-29-vd03-combat-m3b1-implementation-evidence.md)
- [VD-03 M3B2A 일반 적 행동 코어 작업 계약 — Approved](./specs/work-contracts/2026-08-29-vd03-combat-m3b2a-regular-enemy-behavior-core.md)
- [2026-08-29 VD-03 M3B2A 계약 Luna 사전 게이트 — PASS](./verification/2026-08-29-vd03-combat-m3b2a-contract-pregate.md)
- [VD-03 M3B2B 일반 적 행동 Unity 브리지 작업 계약 — Approved](./specs/work-contracts/2026-08-29-vd03-combat-m3b2b-behavior-unity-bridge.md)
- [2026-08-29 VD-03 M3B2B 계약 Luna 사전 게이트 — PASS](./verification/2026-08-29-vd03-combat-m3b2b-contract-pregate.md)
- [2026-08-29 VD-03 M3B2B 구현 증적 — PASS](./verification/2026-08-29-vd03-combat-m3b2b-implementation-evidence.md)
- [VD-03 M3C1 결정론적 일반 적 이동 코어 작업 계약 — Approved](./specs/work-contracts/2026-08-30-vd03-combat-m3c1-enemy-locomotion-core.md)
- [2026-08-30 VD-03 M3C1 계약 Luna 사전 게이트 — PASS](./verification/2026-08-30-vd03-combat-m3c1-contract-pregate.md)
- [2026-08-30 VD-03 M3C1 구현 증적 — PASS](./verification/2026-08-30-vd03-combat-m3c1-implementation-evidence.md)
- [VD-03 M3C2 일반 적 이동 Unity 브리지 작업 계약 — Approved](./specs/work-contracts/2026-08-30-vd03-combat-m3c2-enemy-locomotion-unity-bridge.md)
- [2026-08-30 VD-03 M3C2 계약 Luna 사전 게이트 — PASS](./verification/2026-08-30-vd03-combat-m3c2-contract-pregate.md)
- [2026-08-30 VD-03 M3C2 구현 증적 — PASS](./verification/2026-08-30-vd03-combat-m3c2-implementation-evidence.md)
- [VD-03 M3D1 결정론적 일반 적 위협 코어 작업 계약 — Approved (equal-X 호환성 부록)](./specs/work-contracts/2026-08-31-vd03-combat-m3d1-regular-enemy-threat-core.md)
- [2026-08-31 VD-03 M3D1 계약 Luna 사전 게이트 — PASS](./verification/2026-08-31-vd03-combat-m3d1-contract-pregate.md)
- [2026-08-31 VD-03 M3D1 구현 증적 — PASS](./verification/2026-08-31-vd03-combat-m3d1-implementation-evidence.md)
- [VD-03 M3D2 위협 전달·Unity 브리지 작업 계약 — Approved (구현 수정 부록)](./specs/work-contracts/2026-08-31-vd03-combat-m3d2-threat-delivery-unity-bridge.md)
- [2026-08-31 VD-03 M3D2 계약 Luna 사전 게이트 — PASS](./verification/2026-08-31-vd03-combat-m3d2-contract-pregate.md)
- [2026-09-01 VD-03 M3D2 구현 증적 — PASS](./verification/2026-09-01-vd03-combat-m3d2-implementation-evidence.md)
- [VD-03 M3E1 일반 적 조우 장면 조립·표현 handoff 작업 계약 — Approved](./specs/work-contracts/2026-09-01-vd03-combat-m3e1-authored-encounter-composition.md)
- [VD-03 M3E1 계약 사전 게이트 증적 — PASS](./verification/2026-09-01-vd03-combat-m3e1-contract-pregate.md)
- [VD-03 M3E1A 충돌 제외 타당성 증적 — FAIL / STOP](./verification/2026-09-01-vd03-combat-m3e1a-collision-feasibility.md)
- [VD-01 M2 ignored-pair Cast hit 필터 부록 — Approved](./specs/work-contracts/2026-09-01-vd01-movement-m2-ignore-pair-hit-filter-addendum.md)
- [VD-01 M2 ignored-pair Cast hit 필터 계약 사전 게이트 — PASS](./verification/2026-09-01-vd01-movement-m2-ignore-pair-filter-contract-pregate.md)
- [VD-01 M2 ignored-pair Cast hit 필터 구현 증적](./verification/2026-09-01-vd01-movement-m2-ignore-pair-filter-implementation-evidence.md)
- [VD-03 M3E1B0 일반 적 조우 작성 그래프 구현 증적 — PASS](./verification/2026-09-01-vd03-combat-m3e1b0-authored-encounter-evidence.md)
- [VD-01 Movement 작성 테스트 임시경로 격리 증적 — PASS](./verification/2026-09-02-vd01-movement-authoring-test-isolation-evidence.md)
- [VD-03 M3E1B1 lifecycle·표현 handoff 구현 증적 — PASS](./verification/2026-09-05-vd03-combat-m3e1b1-implementation-evidence.md)
- [VD-03 M4A 환수관 오르단 결정론적 보스 코어 작업 계약 — Verified](./specs/work-contracts/2026-09-05-vd03-combat-m4a-ordan-boss-core.md)
- [VD-03 M4A 계약 Luna 사전 게이트 — PASS](./verification/2026-09-05-vd03-combat-m4a-contract-pregate.md)
- [VD-03 M4A 환수관 오르단 보스 코어 구현 증적 — PASS](./verification/2026-09-05-vd03-combat-m4a-implementation-evidence.md)
- [VD-03 M4B1 오르단 Unity Combat 브리지 작업 계약 — Verified](./specs/work-contracts/2026-09-05-vd03-combat-m4b1-ordan-unity-combat-bridge.md)
- [VD-03 M4B1 계약 Luna 사전 게이트 — PASS](./verification/2026-09-05-vd03-combat-m4b1-contract-pregate.md)
- [VD-03 M4B1 오르단 Unity Combat 브리지 구현 증적 — PASS / Verified](./verification/2026-09-05-vd03-combat-m4b1-implementation-evidence.md)
- [VD-03 M4B2 오르단 scripted Transfer 노출 작업 계약 — Verified](./specs/work-contracts/2026-09-06-vd03-combat-m4b2-ordan-transfer-exposure.md)
- [VD-03 M4B2 계약 Luna 사전 게이트 — PASS](./verification/2026-09-06-vd03-combat-m4b2-contract-pregate.md)
- [VD-03 M4B2 오르단 scripted Transfer 노출 구현 증적 — PASS / Verified](./verification/2026-09-06-vd03-combat-m4b2-implementation-evidence.md)
- [VD-03 M4B3A 오르단 저작 보스 그래프 작업 계약 — Verified](./specs/work-contracts/2026-09-06-vd03-combat-m4b3a-ordan-authored-graph.md)
- [VD-03 M4B3A 계약 Luna 사전 게이트 — PASS](./verification/2026-09-06-vd03-combat-m4b3a-contract-pregate.md)
- [VD-03 M4B3A 오르단 저작 보스 그래프 구현 증적 — PASS / Verified](./verification/2026-09-06-vd03-combat-m4b3a-implementation-evidence.md)
- [VD-03 M4B3B1 오르단 Audit forecast·노출 작업 계약 — Verified](./specs/work-contracts/2026-09-06-vd03-combat-m4b3b1-ordan-audit-exposure.md)
- [VD-03 M4B3B1 계약 Luna 사전 게이트 — PASS](./verification/2026-09-06-vd03-combat-m4b3b1-contract-pregate.md)
- [VD-03 M4B3B1 구현 증적 — PASS / Verified](./verification/2026-09-06-vd03-combat-m4b3b1-implementation-evidence.md)
- [VD-03 M4B3B2 오르단 적대 기하·다음 틱 플레이어 피해 작업 계약 — Verified](./specs/work-contracts/2026-09-06-vd03-combat-m4b3b2-ordan-hostile-geometry.md)
- [VD-03 M4B3B2 계약 Luna 사전 게이트 — PASS](./verification/2026-09-07-vd03-combat-m4b3b2-contract-pregate.md)
- [VD-03 M4B3B2 구현·Luna 독립 검증 — PASS](./verification/2026-09-07-vd03-combat-m4b3b2-implementation-evidence.md)
- [VD-03 M4B3B3 오르단 BalanceAudit 결정론적 견인 작업 계약 — Verified](./specs/work-contracts/2026-09-07-vd03-combat-m4b3b3-ordan-audit-pull.md)
- [VD-03 M4B3B3 계약 사전 게이트 — PASS](./verification/2026-09-07-vd03-combat-m4b3b3-contract-pregate.md)
- [VD-03 M4B3B3 구현 증거 — PASS / Verified](./verification/2026-09-07-vd03-combat-m4b3b3-implementation-evidence.md)
- [VD-03 M4B3C 보스 종료 정리 작업 계약 — Verified](./specs/work-contracts/2026-09-08-vd03-combat-m4b3c-ordan-terminal-teardown.md)
- [VD-03 M4B3C 계약 사전 게이트](./verification/2026-09-08-vd03-combat-m4b3c-contract-pregate.md)
- [VD-03 M4B3C 상세 종료 검증 — PASS / Verified](./verification/2026-09-08-vd03-combat-m4b3c-implementation-evidence.md)
- [VD-05 M5A 보스 이후 진행 판단 코어 — Verified](./specs/work-contracts/2026-09-08-vd05-m5a-post-boss-progression-core.md)
- [VD-05 M5A 계약 사전 게이트 — PASS](./verification/2026-09-08-vd05-m5a-contract-pregate.md)
- [VD-05 M5A 구현·독립 검증 — PASS](./verification/2026-09-08-vd05-m5a-implementation-evidence.md)
- [VD-07 M5B1 이동 입력 잠금 기반 — Verified](./specs/work-contracts/2026-09-08-vd07-m5b1-movement-input-gate.md)
- [VD-07 M5B1 구현·독립 검증 — PASS](./verification/2026-09-08-vd07-m5b1-input-gate-evidence.md)
- [VD-07 M5B2 전이·보스 전투 입력 잠금 — Verified](./specs/work-contracts/2026-09-08-vd07-m5b2-transfer-combat-input-gates.md)
- [VD-07 M5B2 구현·독립 검증 — PASS](./verification/2026-09-08-vd07-m5b2-input-gate-evidence.md)
- [VD-09 M5D3 프로필 진행 상태 투영 코어 — Verified](./specs/work-contracts/2026-09-12-vd09-m5d3-profile-progression-projection-core.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-12-vd09-m5d3-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-12-vd09-m5d3-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-12-vd09-m5d3-luna-independent-review.md) — engine-free progression value projection만 검증했다. profile file·codec/hash·persistence·M5A/Unity 연결은 후속 범위다.
- [VD-09 M5D3 계약 Luna 사전 게이트 — PASS](./verification/2026-09-12-vd09-m5d3-contract-pregate.md)
- [VD-09 M5D4 프로필 설정·튜토리얼 값 코어 — Verified](./specs/work-contracts/2026-09-12-vd09-m5d4-profile-settings-tutorial-core.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-12-vd09-m5d4-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-12-vd09-m5d4-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-12-vd09-m5d4-luna-independent-review.md) — engine-free settings/tutorial 값만 검증했다. persisted profile·settings 적용·tutorial event·Unity 연결은 후속 범위다.
- [VD-09 M5D5 binding override RFC 8785 코어 — Verified](./specs/work-contracts/2026-09-12-vd09-m5d5-binding-overrides-jcs-core.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-12-vd09-m5d5-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-12-vd09-m5d5-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-12-vd09-m5d5-luna-independent-review.md) — engine-free inner JSON canonicalization만 검증했다. binding 적용·outer profile/hash·persistence·Unity 연결은 후속 범위다.
- [VD-09 M5D6 프로필 입력 블록·호환성 코어 — Verified](./specs/work-contracts/2026-09-12-vd09-m5d6-profile-input-compatibility-core.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-12-vd09-m5d6-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-12-vd09-m5d6-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-12-vd09-m5d6-luna-independent-review.md) — input metadata compatibility만 검증했다. actual override apply·outer profile recovery·Unity 연결은 후속 범위다.
- [VD-09 M5D7A profile v1 canonical encoder — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7a-profile-v1-canonical-encoder.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-13-vd09-m5d7a-contract-pregate.md) · [Terra 구현·Astra 실행 증적](./verification/2026-09-13-vd09-m5d7a-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-13-vd09-m5d7a-luna-independent-review.md) — 집중 EditMode `8/8`, 전체 EditMode `529/529`, 전체 PlayMode `576/576` 통과(실패·건너뜀·미결정 `0`). canonical encoder와 mismatch recovery 후보 보존만 검증했으며 decoder·atomic persistence는 후속 범위다.
- [VD-09 M5D7B profile v1 canonical decoder — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7b-profile-v1-canonical-decoder.md) · [Luna 계약 사전 게이트 — PASS](./verification/2026-09-13-vd09-m5d7b-contract-pregate.md) · [Terra 구현·Astra 실행 증적](./verification/2026-09-13-vd09-m5d7b-implementation-evidence.md) · [Luna R2 독립 검증 — PASS](./verification/2026-09-13-vd09-m5d7b-luna-independent-review.md) — 최종 집중 EditMode `16/16`, 전체 EditMode `545/545`, 전체 PlayMode `576/576` 통과. exact canonical/hash validator와 metadata·binding input-only recovery projection을 검증했으며 file IO/atomic recovery는 후속 범위다.
- [VD-09 M5D7C profile recovery transformation core — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7c-profile-recovery-transformation-core.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7c-contract-pregate.md) · [구현·실행 증적](./verification/2026-09-13-vd09-m5d7c-implementation-evidence.md) · [Luna R3 독립 검증 — PASS](./verification/2026-09-13-vd09-m5d7c-luna-independent-review.md) — 승인 기본값과 caller-owned default/input repair/previous promotion 변환을 검증. 최종 집중 `10/10`, 전체 EditMode `555/555`, PlayMode `576/576` 통과.
- [VD-09 M5D7D atomic profile save adapter — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7d-atomic-profile-save-adapter.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7d-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7d-implementation-evidence.md) · [Luna 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7d-luna-independent-review.md) — exact temp write-through·flush·재검증, `profile.prev.json` Windows replace/move, 접근 오류 구분, recoverable-only exception containment, ref-counted directory lease를 검증했다. Focused EditMode 29/29, full EditMode 584/584, full PlayMode 576/576.
- [VD-09 M5D7E profile load selection core — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7e-profile-load-selection-core.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7e-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7e-implementation-evidence.md) · [Luna 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7e-luna-independent-review.md) — primary→previous→default 선택, input-only repair, temp source 제외와 역할별 보존 의도를 검증했다. Focused EditMode 10/10, full EditMode 594/594, full PlayMode 576/576.
- [VD-09 M5D7F profile quarantine adapter — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7f-profile-quarantine-adapter.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7f-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7f-implementation-evidence.md) · [Luna R2 독립 검증 — PASS](./verification/2026-09-13-vd09-m5d7f-luna-independent-review.md) — exact recovery identity/nohash proof와 19자 UTC 검증을 보완했다. 최종 집중 `18/18`, 전체 EditMode `612/612`, PlayMode `576/576` 통과.
- [VD-09 M5D7G profile launch observation adapter — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7g-profile-launch-observation-adapter.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7g-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7g-implementation-evidence.md) · [Luna 독립 검증 — PASS](./verification/2026-09-13-vd09-m5d7g-luna-independent-review.md) — exact three-leaf 관찰, typed completion/count, 실제 read fault와 protocol fault 분리, non-atomic 경계를 검증했다. 집중 `10/10`, 전체 EditMode `622/622`, PlayMode `576/576` 통과.
- [VD-09 M5D7H primary input-recovery atomic save extension — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7h-primary-input-recovery-atomic-save.md) · [Luna second pre-gate — PASS](./verification/2026-09-13-vd09-m5d7h-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7h-implementation-evidence.md) · [Luna R2 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7h-luna-postreview.md) — Primary/Decoded typed proof, fresh semantic plan, exact revision proof와 recovery-only atomic replacement를 검증했다. 실제 Input System 적용과 launch coordination은 후속 범위다.
- [VD-09 M5D7I Input System binding-override apply adapter — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7i-input-binding-override-apply-adapter.md) · [Luna R2 pre-gate — PASS](./verification/2026-09-13-vd09-m5d7i-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7i-implementation-evidence.md) · [Luna 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7i-luna-postreview.md) — fresh disabled action candidate의 실제 override 적용과 canonical round-trip 검증을 완료했다. 복구 변환·저장·launch coordination은 후속 범위다.
- [VD-09 M5D7J binding-apply-failure recovery transform — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7j-binding-apply-failure-recovery-transform.md) · [Luna pre-gate — PASS](./verification/2026-09-13-vd09-m5d7j-contract-pregate.md) · [Terra 구현 증적](./verification/2026-09-13-vd09-m5d7j-implementation-evidence.md) · [Luna 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7j-luna-postreview.md) — 실제 적용 실패 뒤 primary/previous source를 중복 revision 증가 없이 current-default input 복구 문서로 바꾸는 순수 변환을 검증했다. 실제 launch 순서는 후속 범위다.
- [VD-09 M5D7K launch preservation sequence executor — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7k-launch-preservation-sequence.md) · [Luna pre-gate — PASS](./verification/2026-09-13-vd09-m5d7k-contract-pregate.md) · [구현 증적](./verification/2026-09-13-vd09-m5d7k-implementation-evidence.md) · [Luna R2 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7k-luna-postreview.md) — 저장 전 stale temp·비정상 primary·previous를 Temp→Primary→Previous 순서로 보존하고 실패 시 `MayPersist`를 금지하는 경계를 검증했다.
- [VD-09 M5D7L profile launch preparation coordinator — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7l-profile-launch-preparation-coordinator.md) · [Luna R2 pre-gate — PASS](./verification/2026-09-13-vd09-m5d7l-contract-pregate.md) · [구현 증적](./verification/2026-09-13-vd09-m5d7l-implementation-evidence.md) · [Luna R2 사후검증 — PASS](./verification/2026-09-13-vd09-m5d7l-luna-postreview.md) — 관찰·선택·보존·격리 입력 적용·필요 복구·저장을 정확한 준비 순서로 통합하고, 입력 객체 소유권과 저장 실패 시 안전한 메모리 상태를 검증했다. 집중 `33/33`, M5D7I `8/8`, 전체 EditMode `663/663`, PlayMode `617/617` 통과.
- [VD-09 M5D7M desktop profile launch adapter — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7m-desktop-profile-launch-adapter.md) · [Luna final post-review — PASS](./verification/2026-09-13-vd09-m5d7m-luna-postreview.md) — 최종 R50 집중 117/117, M5B5 3/3, M5D7L 33/33, 전체 EditMode 663/663, 전체 PlayMode 734/734가 실패·건너뜀·미결정 없이 통과했고 Luna P0=0/P1=0 후 Astra가 수용했다.
- [VD-09 M5D7N hub-entry handoff latch — Verified](./specs/work-contracts/2026-09-13-vd09-m5d7n-hub-entry-handoff-latch.md) · [Terra 구현·실행 증적](./verification/2026-09-13-vd09-m5d7n-implementation-evidence.md) · [Luna 최종 사후검증 — PASS](./verification/2026-09-14-vd09-m5d7n-luna-postreview.md) — M5D7M의 검증된 UIOnly 영수증과 예상 알림을 첫 Update에서 한 번만 검증·이관한다. 최종 집중 `69/69`, M5D7M 직접 회귀 `117/117`, 전체 EditMode `669/669`, 전체 PlayMode `803/803`가 실패·건너뜀·미결정 없이 통과했고 Luna `P0=0`/`P1=0` 후 Astra가 수용했다.
- [VD-09 M5D7O hub-menu presentation controller — Verified](./specs/work-contracts/2026-09-14-vd09-m5d7o-hub-menu-presentation-controller.md) · [Terra 구현·실행 증적](./verification/2026-09-14-vd09-m5d7o-implementation-evidence.md) · [Luna 최종 사후검증 — PASS](./verification/2026-09-14-vd09-m5d7o-luna-postreview.md) — Primary/Previous만 프로필 이어가기로 판정하고 고정 메뉴 순서·초기 포커스·수동 알림 닫기·단일 선택 intent·640×360 UI 의미값을 장면/패키지와 분리했다. 최종 집중 `19/19`, M5D7N 직접 회귀 `69/69`, 전체 EditMode `688/688`, 전체 PlayMode `803/803`가 실패·건너뜀·미결정 없이 통과했고 Luna `P0=0`/`P1=0` 후 Astra가 수용했다.
- [VD-09 M5D7P-A UI semantic frame seam — Verified](./specs/work-contracts/2026-09-20-vd09-m5d7p-a-ui-semantic-frame-seam.md) · [Terra 구현·실행 증적](./verification/2026-09-20-vd09-m5d7p-a-implementation-evidence.md) · [Luna 최종 사후검증 — PASS](./verification/2026-09-20-vd09-m5d7p-a-luna-postreview.md) — 기존 InputRouter 단독 소유권을 유지하면서 UIOnly 입력을 성공 영수증에 결속된 불변 프레임으로 전달한다. 최종 집중 EditMode `5/5`, PlayMode `52/52`, M5B5 `3/3`, M5D7O `19/19`, 전체 EditMode `693/693`, PlayMode `855/855`가 실패·건너뜀·미결정 없이 통과했고 Luna `P0=0`/`P1=0` 후 Astra가 수용했다. 비차단 P2 두 건은 후속 authored-presentation gate로 이관했다.
- [VD-09 M5D7P-B Unity UI package baseline — Verified](./specs/work-contracts/2026-09-20-vd09-m5d7p-b-ui-package-baseline.md) · [Terra 구현 증적](./verification/2026-09-20-vd09-m5d7p-b-implementation-evidence.md) · [Luna 최종 사후검증 — PASS](./verification/2026-09-20-vd09-m5d7p-b-luna-postreview.md) — Unity 6000.6.0f1 내장 uGUI 2.6.0만 직접 선언하고 지원 종료된 TextMeshPro shim은 추가하지 않았다. 동결 기준선의 두 SHA와 정확한 resolver 차분을 독립 재검산했고, 전체 EditMode `688/688`, PlayMode `805/805`가 실패·건너뜀·미결정 없이 통과했다. Luna `P0=0`/`P1=0`/`P2=0` 후 Astra가 수용했다.
- [VD-09 M5D7Q0 Hub-UIOnly router authoring graph — Verified](./specs/work-contracts/2026-09-20-vd09-m5d7q0-hub-ui-only-router-graph.md) · [Terra 구현 증적](./verification/2026-09-20-vd09-m5d7q0-implementation-evidence.md) · [Luna 최종 독립검증 — ACCEPT](./verification/2026-09-23-vd09-m5d7q0-luna-independent-review.md) — 허브 전용 무게임플레이 라우터 그래프와 결정론적 정규 프리팹 복구를 완성했다. 빌더 독립 프로세스 `31/31`, 최종 범위 감사 `4/4`, 전체 EditMode `737/737`, 영향 검토로 재사용 승인된 전체 PlayMode `943/943`가 실패·건너뜀·미결정 없이 통과했고, Luna `P0=0`/`P1=0`/`P2=0` 후 Astra가 수용했다.
- [VD-09 M5D7Q-A authored uGUI hub shell — Verified](./specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md) · [Terra implementation evidence](./verification/2026-09-23-vd09-m5d7q-a-implementation-evidence.md) · [Luna final postreview — PASS](./verification/2026-09-27-vd09-m5d7q-a-final-luna-postreview.md) · [retry-r visual review — PASS](./verification/2026-09-27-vd09-m5d7q-a-resolution-capture-retry-r-visual-review.md) — Astra integrated the final package after independent Luna P0/P1/P2=0 review. The final record preserves the Approved-at-execution SHA and the separate fresh-process capture-equality proof.

## 단일 진실원천

| 질문 | 권위 있는 위치 | 담지 않는 것 |
|---|---|---|
| 이 용어는 무엇을 뜻하는가? | `CONTEXT.md` | 구현 수치, 일정, 테스트 |
| 현재 게임 세계와 제품 방향은 무엇인가? | `docs/canon/` | 결정 과정, 코드 구조 |
| 왜 이 선택을 했는가? | `docs/adr/`의 현재 accepted ADR | 세부 동작, 작업 목록 |
| 무엇을 어떻게 검증해야 하는가? | `docs/specs/`의 Approved 스펙 | 결정의 역사, 홍보 문구 |
| 누가 설계·구현·검수하는가? | `AGENTS.md`, `docs/agent-operating-model.md` | 게임 규칙 |
| 독자는 어디서 내용을 찾는가? | `wiki/` | 새로운 결정이나 요구사항 |

## 충돌 해결 규칙

1. superseded ADR은 현재 판단 근거로 사용하지 않는다.
2. 현재 ADR과 캐논이 충돌하면 구현을 멈추고 Astra가 둘을 함께 정정한다.
3. 스펙이 캐논 또는 현재 ADR과 충돌하면 스펙을 수정한다.
4. 위키가 원본 문서와 다르면 원본을 따르고 위키를 갱신한다.
5. 대화, 작업 메모, 에이전트 초안은 문서에 승인 반영되기 전까지 권위가 없다.

## 변경과 추적성

- 중요한 비가역 결정은 ADR 번호 `ADR-NNNN`으로 기록한다.
- 기능 요구사항은 스펙별 접두사가 있는 `REQ-...` ID를 갖는다.
- 인수 기준은 `AC-...` ID를 갖고 하나 이상의 요구사항을 검증한다.
- 미결정은 `OD-...` ID로 기록하며, 해결 전에는 관련 스펙을 `Approved`로 올리지 않는다.
- 구현 변경은 관련 REQ ID를, 테스트·플레이 검수 결과는 관련 AC ID를 남긴다.
- 위키 페이지는 하단의 `권위 문서` 링크로 원본을 가리킨다.

## 상태 게이트

`Draft → Review → Approved → Implemented → Verified`

- Astra만 스펙을 `Approved`로 전환하고 통합을 승인한다.
- 솔은 Approved 계약 안에서 단위 파트를 설계·구현한다.
- Luna는 AC ID에 따라 독립 검수하고 `Verified` 증적을 남긴다.
- 과거 외부 Ollama 산출물은 현재 작업 흐름에 사용하지 않는다. 새 단위는 솔 구현과 루나 독립 검증을 거치며 아스트라 승인 전에는 상태를 전환하지 못한다.
