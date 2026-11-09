# CH1 자연 보스 검증 — 입력 소비와 FixedUpdate 관측 간격 (2026-11-09)

**현황:** 입력 정책의 한정된 안전 보정·검사 및 새 Windows 빌드 완료. Extraction **인게임 ChapterCompleted / candidate-success 증거 확보**, 그러나 **프로세스 정상 종료·최종 result 성공은 미확인**. 자연 플레이 수동 승인 및 독립 검수 미완료.

## 재현 근거

- 베이스: `attempt-039`의 격리 `branch1.jsonl`: DebtLineA Telegraph의 완료 tick1896에 D+Space 입력 seq1870 발행, 다음 관측 tick1914(동일 게임 프레임 내 약 18개 물리 tick), 위치 x=-7·접지 유지, last consumed seq1869. 입력을 **발행**했지만 다음 Dynamic 소비는 관측하지 못했다. 원인인 Unity 콜백 사이의 실제 hitch 발생 경위는 확정하지 않는다.
- `Chapter1VerificationBossPolicy.Read`: Standalone 소유 점프 후보가 **소비 영수증 없이 8 완료 tick 초과** 시 새로운 입력·물리 미래를 추정하지 않고 `소유 입력 미소비` 상세 이유로 fail-closed. 기존 8 tick 상한/권한/충돌·피해 검사는 완화하지 않았다.
- 기존 리소스가 보스 진입에 처음 동기화되는 부담을 줄이기 위해 `Chapter1OrdanArt.PreloadForHub`로 정적·좌우 후보 Sprite를 플레이 허브 초기 단계에 사전 생성하도록 변경. 보스 시간·시뮬레이션·입력·세이브 권한은 변경하지 않으며, 직접 성능 개선은 별도 관측 대상이다.
- `attempt-042` 신규 격리 실행: 종전 실패를 지나 Recovery age20, x=3.158203, 소유 D+Space 발행 후 다음 FixedUpdate의 동적 입력 소비 전 상태에서 Recover 접근 정책이 조기 거절. 완료 실패 원문 보존.
- `TryRecoveryApproach`: **본인 소유 Standalone**만, 동일 phase/identity·유한 좌표·접지·무위협·보스 몸체 여유 공간·기존 중립 receipt가 유지될 때 회복 시작 이후 최대 **2 완료 tick** 동안 이미 요청한 D+Space를 계속 발행할 수 있게 한정. 입력 소비/실제 상승은 **별도 현재 관측 필수**이며, 3번째 미소비 tick, 위험/충돌/소유권 변경은 반드시 fail-closed. 시뮬레이션이나 입력 이벤트를 직접 조작하지 않는다.

## 코드 및 검사

- 제품 코드: `output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Runtime/Input/Unity/Chapter1VerificationInputPilot.cs`, `HubPresentation/Unity/Chapter1OrdanArt.cs`, `Chapter1ExpeditionPresenter.cs`.
- 회귀: `Chapter1BossVerificationPolicyPlayModeTests.cs`에 18tick 불소비 사례, Recovery 1/2tick pending·정상 소유 receipt·3tick 경계·위협 거절 반례 추가.
- Unity 입력 정책 EditMode 검사: **42/42 통과**, `output/ch1-ordan-art-20261109/boss-gap-audit-20261109/policy-v2-results.xml`.
- 오르단 Unity 아트 검사: **4/4 통과**, 동일 경로 `ordan-art-results.xml`.
- Windows Standalone64: `attempt-042` 및 `attempt-043` 모두 빌드 성공(오류0, 경고2). 빌드 경고는 기존 미사용 필드 CS0414 및 TextMeshPro 셰이더 deprecated 구문; 배포 승인은 별도.

## attempt-043: 달성 범위와 미해결 종료 문제

- 독립 격리 `output/ch1-ordan-art-20261109/boss-gap-audit-20261109/attempt-043-branch1-clean/VerificationEvidence/branch1.jsonl`: 오르단 실제 BasicAttack 피격으로 HP60→27 진행, 이후 보스 격파, Extraction 엔딩 5줄, `ChapterCompleted`, `completion branch=Extraction revision=3 advance=5`, `candidate-success`까지 관측.
- **종료는 실패:** `candidate-success` 이후 정상 `cleanup` 또는 최종 `result` 이벤트가 기록되지 않았고, 외부 소유 런처가 145초에 **자신이 시작한 게임 PID만** 종료(외부 런처 exit124). 따라서 **자동 전체 통과 / 정상 종료 / 독립 수용으로 표시하지 않는다.**
- 완료 후 약 68초까지 비소유 마우스 이벤트가 기록됐으나 종료 지연의 인과 원인은 미확인. 종료 처리의 호출·시간 진행·메시지 루프/권한 경로를 다음 별도 관찰 대상으로 유지하고, 같은 실패 경로를 무의미하게 반복하지 않는다.
- 원본 에셋, 개인 저장 데이터, 다른 프로젝트 프로세스는 변경하지 않았다. 원격 `main` 통합·릴리스 승인은 보류.

**다음 우선순위:** (1) 후보 완료 이후 종료·cleanup 누락 원인 최소 관측, (2) Solidarity 분기·실패 재시도 동일 소스 검증, (3) 보스 아트 1배율 자연 동작 검수, (4) 독립 QA 후 main 통합.
