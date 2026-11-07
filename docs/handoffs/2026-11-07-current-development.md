# 현재 개발 경로와 비Unity 구현 — 2026-11-07

PR #11은 `a11d75270fc87c60220dcee1e04b16dbd37af6f4`로 원격 main에 병합됐다. 현재 개발 작업공간은 `H:/GPT-workspace/AcadeGameMaker-integration-20261107`, 활성 제품 상대 경로는 `output/chapter1-playable-r1/project`다. 이전 `H:/GPT-workspace/AcadeGameMaker`는 기존 main·dirty 수정·미게시 80커밋 이력·원시 자료의 보존본이다. 원본을 force/reset하거나 새 제품 수정을 역복사하지 않는다.

산사 자원을 우선하여 Unity Editor·Hub·batchmode·Player·빌드는 실행 보류다. 설계·코드 구현과 가벼운 비Unity 검증은 계속한다. 이 단계는 기존 소유 점프 fallback의 빠진 안전 조건을 보완한다. 일반/미소유 판정, 생산 HP·좌표·시간·전투와 기존 승인·실패·소진 원문은 바꾸지 않는다.

DebtLine은 Execute age0 단발이며 hostile 접촉은 그 tick의 직전 완료 pose로 평가하고 피해만 t+1에 배송한다. 포락선은 이 실제 접촉 시점과 추가 보수 샘플의 하한을 검사한다. 최대 상승·느린 중력/낙하 상한으로 계산한 착지, 벽·보스 몸체, Stage3 최단 실행18+복구33tick·다음 예고60tick·직전 pose1tick과 입력 지연을 함께 제한한다. 실제 WallSide/WallId 관측도 소유 계속을 철회한다. 신원·lease·sequence·focus·phase/slot·접촉/예약/당김 거절은 유지한다. E2 y/vy는 순수 계산 반례이며 과거 실행의 현재 소유 자격이나 실제 전투 성공을 입증하지 않는다.

재현은 저장소 루트에서 `powershell.exe -NoProfile -File qa/tools/Test-Ch1VerificationPolicy.ps1`이다. 설치된 .NET SDK/reference pack만 사용하고 다운로드나 Unity 어셈블리 로드는 없다. 실제 소스 policy/상수/시기/저장 메서드를 추출해 계산·상태·파일시스템을 검사한다. 값 어댑터는 enum 이름과 항상 비활성인 단독 세션 형식만 제공하며 Unity 물리 모사가 아니다. 양성 소유 검사는 기존 fixture lease 값 입력이고 실제 단독 Driver 권한 양성의 증거가 아니다.

현재 Unity NUnit 실행, 실제 물리·충돌·동일 배포본 보스 전투·전체 회귀·새 빌드·실제 CH1 완주는 미검증이다. 다음 실행 재개 시 현재 활성 경로와 새 커밋을 사용하고 과거 Source 핀이나 보존 원본에서 Unity를 시작하지 않는다. CH1 패키지 성숙도와 최종 수용 수치는 이전 감사의 별도 지표이며 이번 정적 보정을 전체 게임 완료율로 환산하지 않는다.
