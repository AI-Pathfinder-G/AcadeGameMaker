# C3 intake·취소 증거 보완 설계 독립 검수

- 검수자·실제 모델: 루나, `gpt-6-luna`
- 대상: `docs/proposals/2026-09-29-c3-intake-and-cancel-required-evidence.md`
- SHA-256: `D55936D54119282834909ED4C13C3CA683C981984D9541AAA138290B01BCBE2C` (일치)
- 기준: Approved C3 REQ/AC-M5D7QC3-001/005/009/010 및 2026-09-29 제한 증거 보정 승인, owner/Q-B 현재 지문
- 범위: 제안 설계만. 소스·시험·Unity 수정 또는 실행 없음.

## 판정

**P0=0, P1=0 (설계 범위).** 제안은 실제 발급된 두 cohort 참조만 쓰며, AC001의 clean foreign 거절과 실제 한 필드 proof 손상 폐쇄를 구분한다. AC005는 실제 같은 Cancel 전후 관찰을 한 cohort에서 수행하고, synthetic fixture에 존재하지 않는 `CurrentSession`/gameplay 상태를 만들어 내지 않고 부재와 C3의 비호출 경계를 증거 한계로 기록한다. 이는 C3가 C1/C2 실행 전 synthetic-only라는 범위와 부합하며 실제 게임 세션의 메모리 보존 증거로 확대하지 않는다.

## 주요 설계 확인

- Root·receipt·adapter, owner/presenter/router 문맥, 최초·후속 epoch, proof 손상 행은 모두 실제 발급 handle 및 실제 두 fixture를 이용한다. 증명 객체/registry를 정상 발급 상태로 조립하지 않는다. 허용한 네 개 단일 손상(`_launchRootWitnessB`, Q-B `_takenRequestProof`, owner `_epochProof`, `_stateProof`)만 각각 실제 intake에 전달하고, 정상 actual 미소모 proof 손상은 전체 terminal containment로 귀결되게 설계했다. foreign adapter 거절을 동일 adapter root-witness 손상 근거로 합산하지 않는다.
- `AcceptNewGame`의 현 precheck `HasPendingIssued`는 `_state == _stateProof`를 요구해 stateProof 단일 손상을 외부 foreign 거절로 처리할 수 있다. 이를 정확한 CWT 발급/현재 실제 참조 인증과 내부 lifecycle/proof `Validate`로 분리하고, 인증된 현재 actual 손상은 Enter 후 전체 폐쇄로 보내는 보정은 AC001에 필요하고 충분한 방향이다. 외부 잘못된 후보는 소비·폐쇄하지 않고, 뒤이어 원본 정상 handle을 성공시켜 비소모를 증명해야 한다.
- post-Enter 재인증은 사전 인증 객체와 같은 실제 witness 참조를 다시 확인하고, 완료/소비된 정상 이전 권한의 지연 호출만 terminal catch 바깥에서 비변경 거부하는 구분이 타당하다. 아직 미소모인 실제 현재 권한의 pending slot/proof 불일치는 내부 Validate의 전체 폐쇄를 유지한다. Busy로 살아 있는 실제 retry/decision의 허용된 재시도는 계속 가능해야 한다. `ExecutionCommitted`, `TerminalFailure`, `Closed`에서 현재 live presenter 결속만 예외로 두되 기존 원본 cohort 연결과 state/stateProof, epoch/token proof 유효성 검사를 보존하라는 제한도 맞다.
- 일곱 진입점(Confirm, Cancel, RetryIntake, Rearm, AcceptNewGame, OpenFreshDecision, CommitForExecution)을 모두 목록화하고, 재인증·완료 이전 권한의 비변경 거부·내부 불변조건 검증·finally Exit의 순서를 요구한다. Q-B CWT의 readonly issuance record와 Consumed 상태를 확인하는 내부 read-only 보조는 Acceptance/terminal와 completed replay를 나누는 데 한정하고 새 issuance/helper authority를 발급하지 않는다.
- 동시 경합은 동일 실제 capability/handle의 정상 loser 및 완료 후 늦은 재호출을 실행 행으로 나누고, Busy 후 정상 재시도를 보존한다. 사전 인증과 Enter 사이의 정확 지연 창을 재현할 seam은 제안하지 않으므로 해당 interleaving은 실행했다고 주장하지 않고, 모든 실제 진입점의 현재 소스 지문·줄·재검증·catch 배치·Exit 흐름을 구조 보장으로 별도 검수하겠다고 정확히 한정했다.
- 같은 Cancel 호출의 직전/직후에 세 leaf의 존재/bytes/hash, actual decision/display/issuance/history, adapter·router reset 및 input 상태, router maps/actions·binding, 양측 receipts, 실제 loaded/active scene과 해당 fixture 실행 상태를 한 cohort에서 대조한다. 이후 Rearm의 epoch/cursor 차이는 Cancel snapshot과 분리한다. null인 current-session/runtime 대상은 부재로 기록하고 실제 게임플레이·global memory 전수 보존이라 주장하지 않는다.

## 구현 전 승인·증거 경계

시험 설계는 직접 제한 구현 권한을 만들지 않는다. 특히 Accept의 인증 분리, Validate cohort 보정과 post-Enter 재인증은 기존 Q-B/owner 소스를 바꾸는 일이므로 아스트라의 별도 정확 범위 승인 및 새 지문 루나 검수가 선행되어야 한다. 새 issuance/token/CWT 쓰기 경로, 제품 callback/delay seam, presenter/asmdef/friend/public API, C1/C2/scene/gameplay 동작은 이 문서에서 허용되지 않는다. 단일 proof 손상행이 실제 단일 필드만 바꾸는지, 정상 current actual의 성공 비소모와 손상 후 closure를 확인하는지, 일곱 진입점 전부 같은 규칙을 적용하는지 최종 소스에서 재검수해야 한다.

Unity 실제 실행 및 runner/XML 증거는 아직 없다. 180초 제한과 선택/예상 이름/실패·건너뜀·판정보류·중복·누락·전후 지문 대조를 유지해야 한다. AC001/005의 설계 공백에 대한 제안 검수는 가능하지만 구현·AC010 실행 수용이나 C3 전체 Verified는 아니다.
