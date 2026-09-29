# C3 진입·취소·게이트 보정 구현 증거

2026-09-29. 테라 구현 역할의 실제 gpt-6-sol 작업자. 근거는 docs/approvals/2026-09-29-c3-intake-cancel-and-gate-correction-approval.md, 제안 D55936D54119282834909ED4C13C3CA683C981984D9541AAA138290B01BCBE2C 및 루나 설계 검수 결함 0이다. 추적은 REQ/AC-M5D7QC3-001/003/004/005/006/007/009/010의 승인된 단계 범위이며 전체 실행 연결·C4 구현·수용은 아니다.

## 런타임 명세표

| 진입점 | 현재 시작 줄 | 게이트 뒤 전체 종료 처리 밖의 재확인 | 내부 무결성 및 효과 |
| --- | --- | --- | --- |
| AcceptNewGame | owner 146 | 같은 명시 실제 발급의 기존 Q-B 등록 이력 분류 및 정상 종료 상태 거절 | 원본 역할·현재 입력 연결·세대/증표·실제 양쪽 미결 슬롯·기존 Q-B 소비 검사 후 관찰 |
| RetryIntake | owner 166 | 실제 같은 등록 객체와 같은 witness, Closed 거절 | 실제 현재 retry·세대/증표·accepted 발급·예상 상태 및 CAS 검증, Busy 유지 |
| Confirm | owner 222 | 같은 실제 decision witness와 Used 거절 | 현재 decision·display·accepted 발급·세대/결정 세대·상태와 CAS 검증 |
| Cancel | owner 269 | 같은 실제 decision witness와 Used 거절 | 동일 무결성 및 단일 소비 뒤 기존 실제 재무장 권한 발급 |
| OpenFreshDecision | owner 250 | 앞서 잡은 동일 _fresh 참조·세대·epoch 및 종료 상태 확인 | 무결성과 현재 대기 상태 확인 후 기존 새 결정 발급 |
| Rearm | owner 294 | 실제 같은 재무장 witness, Used/Consumed 완료 이력 확인 | 현재 실제 참조·기존 세대/증표·Cancelled·CAS와 기존 정상 준비/커밋 순서 |
| CommitForExecution | owner 322 | 같은 현재 _confirmed 실제 request 참조 및 종료 상태 거절 | 무결성·ConfirmedReady 검사 후 기존 lower 예약→상위 폐쇄→lower 완료 |

모든 Enter 실패는 외부 finally에 들어가지 않는다. 획득에 성공한 호출만 바깥 finally에서 Exit한다. 정상 소비·교체된 호출의 재확인 거절은 내부 Terminal catch 밖에 있으며 미소비 실제 권한의 현재 슬롯/증거 오류는 내부 catch로 전체 폐쇄한다. 정상 Busy 이후 같은 실제 권한의 재시도는 유지했다. owner 354의 Validate는 살아 있는 여덟 상태에만 기존 presenter의 현재 router/latch/adapter 연결을 요구한다. ExecutionCommitted/TerminalFailure/Closed에는 그 현재 살아 있는 연결 요구만 제외하고 기존 원본 역할·상태·세대·증표 무결성은 유지한다.

Q-B 238의 ClassifyNewGameIssuance는 기존 Issuances의 명시된 실제 객체와 원본 issuer/owner/presenter/router/record/handle 및 append 이력과 소비 완료를 읽어 0/1/2 분류만 반환한다. 미결 권한을 자동 검색하거나 증거·권한·등록부를 반환하지 않는다. 등록·발급·소비·역사 변경은 없다. 기존 Consume의 전체 proof와 단일 CAS를 유지했다. 정상 비활성화로 미소비 handle이 종료된 경우도 owner의 유효 종료 상태를 재확인해 반복 Terminal/peer 종료 없이 거절하도록 했다.

## 집중 시험 변경

새 AC001은 두 실제 임시 실행 묶음에서 owner/presenter/router의 take 전 불일치 세 행과 owner/router/다른 root adapter의 take 후 불일치 세 행을 추가했다. 거절 전후 실제 미결·request/proof·taken/history·display를 비교하고 원래 실제 권한과 다른 묶음의 실제 권한이 정상 성공해야 한다. 실제 Cancel/Rearm·첫 프레임 폐기 뒤 이전 handle 거절 및 새 실제 handle 성공, 기존 정상 ConfigureForAuthoring으로 현재 연결을 바꾼 뒤 관찰 전 전체 종료, 정상 비활성화 뒤 같은 실제 handle 거절의 종료 이력 비변경도 추가했다.

음성 주입은 정확히 Q-B _takenRequestProof, owner _epochProof/_stateProof, adapter _launchRootWitnessB 네 필드만 각각 한 번 손상한다. 원래 필드 값을 finally에서 복구하지만 원래 handle의 재사용은 계속 거절돼야 한다. 이를 정상 권한 발급으로 사용하지 않는다. 손상된 epoch proof를 보존한 종료 상태의 검증 getter는 거절할 수 있으므로 그 즉시 종료 증거는 exact readonly _state 읽기로 확인하고, 일반 정상 종료 getter의 별도 시험은 유지했다. 같은 파일의 NegativeEvidenceField는 네 필드와 선언 형식·정확 FieldType만 허용한다.

새 AC005는 M/D/부재와 M/D/잘못된 임시 자료 두 경우에서 같은 실제 Cancel의 직전/직후, 중간 프레임 진행이나 재무장 없이 비교한다. 세 파일 정확 bytes/존재/길이/SHA, 관찰·실제 발급·taken/history·epoch/token, adapter의 실제 메모리/reset 대상 또는 부재, router의 reset/입력 자산·맵·action/binding·mask·enabled, 양측 영수증·latch와 presenter 이력, 실제 loaded/active 장면, 시험 객체·입력 및 실제로 보유한 실행 컴포넌트 참조/활성을 대조한다. 실제 current cell이 있으면 기존 canonical 문서와 설정·입력·튜토리얼·진행·세대를 복사하며, 없으면 부재를 기록한다. 가짜 메모리 셀이나 실제 게임 세션을 만들지 않았으며 실제 게임 전체 보존을 주장하지 않는다. 취소 이후 재무장은 별도 단계에서 epoch 증가를 확인한다.

소유자 편집 시험은 [Test] 19개와 [TestCase] 27행, 실행 모드 시험은 [Test] 8개와 [TestCase] 6행이다. 아스트라가 최종 집중 선택을 독립 생성한다. 기존 필수 회귀 이름과 377개 내부 분류/전이의 전체 행 ID·내용·순서는 유지했다. 이전 예상 원장은 c3-required-decision-expected-rows-r4.json으로 보존했고 현재 원장의 SourceSHA256만 새 시험 지문으로 갱신하여 Rows 문자열 동등 true를 확인했다.

## 동결·컴파일·한계

작업자 원장은 c3-upper-r5-terra-frozen-source-manifest.json이며 아스트라의 독립 통합 원장과 구분한다. 변경 파일은 Q-B, 소유자, 신규 편집 시험, 신규 실행 시험 네 개다. presenter·lower ABD4/F80·adapter/router·기존 감사·조립 정의·기존 회귀 이름은 보존했다. 변경된 런타임, 입력 편집 시험, 입력 실행 시험의 독립 컴파일은 각각 실제 종료 0이며 c3-upper-local-compile-r5/commands.json에 원본/변경 rsp·입력 지문·전체 명령·로그·출력 지문을 보존했다. 최종 읽기 증거 보정 뒤 편집 시험만 재컴파일한 종료 0은 같은 폴더 input-editmode-final-command.json 및 별도 로그에 기록했다. 이전 로그를 덮어쓰지 않았다.

기존 Bee 참조 어셈블리를 사용한 독립 컴파일의 한계가 있으며 Unity 새 전체 빌드·실제 시험 실행·XML 증거 확인을 대신하지 않는다. 앞선 인증과 Enter 사이의 정확 지연 경쟁 순서를 실행 재현하지 않았다. 기존 정상 동시/늦은/중복/Busy 시험과 일곱 진입점의 소스 구조 검수로 구분한다. Unity 실행과 독립 검수·필수 회귀·최종 수용은 남아 있다. GPT 5.6 호출은 없으며 이후 소스 개선을 중지한다.
