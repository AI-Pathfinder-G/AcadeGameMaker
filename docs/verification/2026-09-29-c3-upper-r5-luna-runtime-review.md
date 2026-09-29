# C3 상위 R5 동결본 독립 정적 검수

2026-09-29. 실제 독립 검수자 `gpt-6-luna`. 검토는 지정된 R5 동결 원장과 그 원장의 14개 파일을 읽고 SHA-256을 재계산한 정적 검수다. 런타임·시험·Unity 파일을 수정하거나 컴파일·Unity 시험을 실행하지 않았다.

## 지문과 범위

- Astra 원장 `artifacts/c3-upper-r5-frozen-source-manifest.json`: `B9DC34D615ECE99775C927FB434D88BB53F0B2F9E356BF8FECB2EB2D4483C9EE`. 지정 14개 파일 모두 개별 SHA-256이 원장과 일치했다.
- Q-B `HubMenuIntentHandoffOwnerV1.cs`: `F47C61A043DAF4695CC53E41C6BE9BB67CA147C722F7B830459AA6304B45B3B5`.
- 소유자 `NewGameConfirmationOwnerV1.cs`: `CAEBD7B5DA99AF94A25C5C300789AA48CA87908CA6D8894B99AE49C2391D7043`.
- 편집 시험 `NewGameConfirmationOwnerV1Tests.cs`: `647BEB1256A18EA90A3F73BB3D81B3219A248EBCCE8A7DBB9EA26D176CCB897A`.
- 실행 시험 `NewGameConfirmationOwnerPlayModeTests.cs`: `2D3403E910E0DBA349854A194F7140DE65759190DF53920F2A989C6D0D07127C`.
- 예상 원장 현재 파일 `artifacts/c3-required-decision-expected-rows.json`: `FB4745C4296BD17716328592106171054D1F807E7BEF3FFB663BCF7DB39A17F1`. SourceSHA256은 편집 시험 지문과 일치하고 377개 행이 모두 실행 전 `planned`다. R4 원장(`600CA6CEE3CB5D2C94FF044600E8D2385BEE4BE49AD4EE25D03DBEB1E445EBB8`)의 377개 행 내용과 현재 행 내용은 직렬화 비교에서 일치했다. 이는 실행 결과가 아니다.

승인된 진입·취소·게이트 보정과 결정 증거 범위를 Approved C3 및 `docs/approvals/2026-09-29-c3-intake-cancel-and-gate-correction-approval.md`, `docs/approvals/2026-09-29-c3-required-decision-evidence-implementation-approval.md`에 대조했다. Q-B 발급 기록 읽기 보조 외 새 등록부·토큰·외부 권한 통로·공개 API·assembly 권한을 추가하지 않았고, 하위 ABD4/F80 및 기존 Q-B 시험 원본을 보존했다.

## 정적 판정

**P0 0건, P1 0건.** 이전 예비 검수의 미소모 발급 handle과 정상 비활성화 후 재호출 우려는 닫혔다. `AcceptNewGame`의 게이트 전·후에 `IsNormallyEnded()`와 같은 실제 Q-B issuance 이력 재확인이 있다. 유효한 종료 상태는 inner terminal catch 밖에서 거절된다. live pending issuance의 proof·슬롯 불일치는 여전히 inner `Validate()`/pending 검증으로 들어가 폐쇄된다. 추가 편집 시험은 실제 take 후 owner 비활성화, late Accept 거절, owner/Q-B 종료 상태와 intake snapshot 불변을 확인한다.

일곱 진입점 모두 성공적으로 게이트를 잡은 호출만 외부 `finally`에서 해제하며 `Enter()` 실패는 다른 호출의 게이트를 건드리지 않는다. `AcceptNewGame`, `RetryIntake`, `Confirm`, `Cancel`, `OpenFreshDecision`, `Rearm`, `CommitForExecution`은 게이트 뒤 같은 실제 issuance/witness/slot 또는 generation을 다시 확인한다. 소비·완료·후속 세대의 정상 늦은 호출은 효과 처리 catch 밖에서 거절한다. 아직 미소모인 실제 권한의 상태·proof·slot 불일치는 catch 안에서 전체 종료한다. 후속 정상 상태를 늦은 loser의 catch가 종료하지 않는지 기존 늦은/동시 시험과 분기 구조를 확인했다. 이 특정한 인증-게이트 지연 순서를 별도로 실행 재현했다고 주장하지 않는다.

`Validate()`는 여덟 live 상태에서 현재 presenter/router/latch/adapter 연결을 검사한다. `ExecutionCommitted`, `TerminalFailure`, `Closed`에서는 그 live 연결 조건만 면제하고 cohort 원본·상태/proof·세대/token 검증은 유지해 종료 상태 getter 동작을 보존한다. 종료 후 getter에 대한 정적 경로와 손상된 상태에서 허용된 네 필드 음성행의 조회 예외 분기도 승인 범위와 일치한다.

시험 본문에서 AC001의 실제 두 cohort 전·후 문맥 거절, 승인된 네 단일 증거 손상과 비복구, 정상 구성 변경, 비활성화, 이전 세대 handle 거절 및 새로운 실제 handle 성공을 확인했다. AC002 분류 행렬 173개와 AC004 실제 파일 전이 행렬 204개는 각 실제 fixture/capture와 전체 세 leaf fingerprint를 사용하며 exact-default는 새 prompt·generation 없이 fresh capture에 직접 결속한다. AC003은 actual authority의 null/미등록/복제/foreign/old-generation 거절과 기존 동시 경합을 포함한다. 실제 같은 스레드 `Rearm` 렌더 그래픽 callback 시험은 중첩 stale Confirm/Cancel/Rearm 거절·snapshot 불변·바깥 재무장 완료를 검사하고, 실제 Confirm 경로의 callback-free 호출 사슬은 별개 구조 증거로 취급한다. AC005는 같은 실제 Cancel 전후의 세 파일 bytes/부재, adapter/reset 및 실제 메모리 대상/부재, router input/action/map/binding, 영수증·history, 장면 및 보유한 실행 컴포넌트 상태를 비교한다. 메모리 셀 부재는 합성 fixture의 관찰이며 실제 게임 세션 전체 보존으로 확장하지 않는다. AC006은 재무장 partial failure, 첫 실제 입력 프레임 폐기, 늦은 successor 및 단조 세대 증거를 포함한다.

AC007/008은 C4 실행 연결 전 부분 범위다. R5는 실제 C1 barrier 결과와 모든 post-barrier outcome, 실제 실행 보고의 전체 기준을 입증하지 않는다. AC009는 이 범위의 정적/API 및 authority 제한을 확인했지만, 전체 C3 수용이나 C4 승인을 뜻하지 않는다. AC010은 아직 통과 판정이 없다.

## 남은 검증

독립 컴파일 기록은 Unity 전체 빌드가 아니다. R5 최종 편집 모드 컴파일 기록의 추가 증거는 최초 DLL 바이트가 없고 최종 재컴파일이 같은 출력 경로를 덮어쓴 한계를 명시한다. 실행 모드 및 런타임 편집 컴파일 기록은 남아 있으나 새 Unity 시험 실행과 XML 결과를 대체하지 않는다.

따라서 이 문서는 **정적 P0/P1 검수 통과만** 기록한다. Astra가 생성할 신규 집중 Edit 155개/Play 14개와 각 원장·XML·종료 코드, 377개 내부 행의 실제 계획/완료/실패/미실행 출력, 기존 필수 회귀 562+51 Edit 및 536+74 Play를 정확한 실행 소스와 대조해야 한다. 이 실행·회귀·결과 검수 전에는 구현 수용 또는 전체 AC-M5D7QC3-010 통과로 표시하지 않는다. C3 전체 AC-007/008은 Open/Not Verified이고 C4는 Review로 유지한다.
