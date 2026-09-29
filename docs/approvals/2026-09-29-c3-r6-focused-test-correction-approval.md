# C3 집중 시험 실행 구성의 제한 보정 승인

2026-09-29. 아스트라 승인, 상태 `Approved`.
`REQ-M5D7QC3-001/002/004/005/007`,
`AC-M5D7QC3-001/002/004/006/009/010`을 추적한다.

[솔 보정안 r2](../proposals/2026-09-29-c3-r6-focused-test-correction-design-r2.md)
지문 `DE6A88E4041114A4116720ED8639F3C17EA0C6D5AF27A4DE586863722DDEDE43`와
[루나 독립 설계 검수](../verification/2026-09-29-c3-r6-focused-test-correction-luna-design-review.md)
지문 `E230CF7C22B4C636EB4E8CFE862022C213655E4E99AB8A6CC58550D6E59494A0`,
정적 P0/P1=0을 근거로 아래 시험 구성 변경만 승인한다.

실제 R6 실행은 155개 중 149개 통과·6개 실패, 실제 Unity 종료값 2다.
이름 누락·중복과 전후 884개 지문 변화는 0이다. 내부 377개 행의 계획·관측
출력은 존재하지만 상위 네 NUnit 시험이 180초 제한 초과로 실패해 수용 행은
0개다. [정정된 독립 결과 검토](../verification/2026-09-29-c3-r6-focused-edit-r1-luna-result-review-r3.md)를
보존하며 현재 실행의 생명주기·제출 입력·시간 초과 세 P1 묶음을 닫지 않는다.

## 허용 경로와 보정

- `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs`
- `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs`
- `artifacts/c3-build-focused-selection.ps1`: 필요할 때 정확 선언 재집계만.
  이미 지원하는 복수 정수 인자 처리를 불필요하게 일반화하지 않는다.
- `artifacts/c3-build-required-decision-expected-rows.ps1`: 기존 377개 행에
  정확 NUnit 사례·원래 인덱스·분할 묶음을 결속하는 부분만.
- `artifacts/c3-verify-decision-rows.ps1`: 기존 단일 사례 추정을 정확 행별
  사례 매핑으로 교체하며 성공 판정 조건은 유지한다.
- 새 버전 원장·선택·정적 증거 파일. 이전 소스와 실행 원장은 별도로 보존한다.

분류 173개 행을 8개씩 22개 고정 사례로, 세 역할별 전이 68개 행을 3개씩
23개 사례로 나눈다. 합계 91개다. 각 분할은 원래 행 배열의 연속 부분이며,
이를 원장 순서로 연결하면 원래 배열과 정확히 같아야 한다. ID·내용·역할·
순서·잠금 역할·기대 결과·권한·세대·원본 참조·파일 전후 동등성 검증은
변경하지 않는다. 사례별 계획/관측은 해당 부분만 출력하고, 그 정확 사례가
`Passed`인 경우에만 행 증거를 수용한다. 377개 각각 계획/통과 출력 1개,
실패·중복·미지 행·미지 상태 0을 요구한다. 180초 제한은 그대로 유지한다.

정상 비활성화 사례는 기존 허용 실행 모드 시험 파일로 옮겨 정상 구성·활성화·
실제 입력 선택·실제 권한 take 이후 `Behaviour.enabled=false`로 실제
수명주기 콜백을 거친다. 필요하면 정상 한 프레임 양보를 사용한다. `Closed`,
양측 폐쇄, 늦은 intake 거부와 역사/스냅샷 불변 검증을 유지한다. 콜백 직접
호출, `ExecuteAlways` 추가, 제품 수명주기 변경과 기대 조건 완화는 금지한다.

즉시 준비 커서 시험은 키보드 추가 후 새 successor 생성 **이전**에 실제
released 상태 이벤트와 정상 입력 갱신을 수행한다. 새 successor 이후의 첫
실제 발행은 기존 Enter 입력과 정상 router 단계로 `SubmitPressed=true`여야
한다. 그 실제 프레임을 버리고 권한·역사를 유지한 뒤 다음 released/Enter
입력에서 새 정상 선택·take 성공을 확인한다. 새 세대에서 빈 발행을 먼저
소비하거나 억제 필드·가짜 프레임·직접 action 콜백을 주입하지 않는다.

## 유지 조건과 검증

구현자는 테라 역할의 별도 `gpt-6-sol`, 독립 검수자는 `gpt-6-luna`, 최종
통합은 아스트라다. 구현자는 자기 구현을 독립 수용하지 않는다. 두 시험의
원래 지문은 각각 `647BEB1256A18EA90A3F73BB3D81B3219A248EBCCE8A7DBB9EA26D176CCB897A`,
`2D3403E910E0DBA349854A194F7140DE65759190DF53920F2A989C6D0D07127C`다.
원본 보존 후 변경 지문·정확 선택 이름과 행 매핑을 고정한다. 잠정 편집
241개·실행 15개는 선언 재집계와 실제 XML 대조 전 실행 증거가 아니다.

런타임·조립·friend·공개 API·meta·자산·설정·Q0 pin·기존 필수 562/51/610
회귀 원장은 변경하지 않는다. 실제 실행 종료값·누락·중복·건너뜀·불확정·
전후 지문을 새 최종 소스에서 검증하고 독립 검수를 받아야 한다. 이전 실패
XML/로그/원시 집계와 대기 반환 기록은 보존한다. 시간 초과가 다시 생기면
그 실패를 남기며 제한을 늘리거나 행을 삭제하지 않는다.

이 승인은 시험 보정 구현에 한정한다. 새 실행 통과, 선행 단계 수용, 전체
C3 AC007/008 및 Review C4 수용을 승인하지 않는다.
