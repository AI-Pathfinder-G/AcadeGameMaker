# C3 필수 결정 증거 보완 승인

아스트라는 사용자 후속 구현 지시, Approved C3 및 부모 명세의 2026-09-29 분류·새 결정·재진입 증거 해석을 근거로 제한 집중 시험 보완을 승인한다. 솔의 행렬 지문 `B213890A1204B72BDBE9B6263154341A6668F95C7BBC3D224EBAB8D15D41599D`에 대한 루나의 유일한 설계 P1은 exact-default 세대 문구 해석이며 부모 명세에 명시했다. 재진입 제안 지문은 `DFF389EBE18B29EC5B6DC790FD8D73060A505CBEA3E99FCE17CA5161D62A3E79`다. 독립 설계 해석 확인과 새 구현 검수는 별도로 남긴다.

테라 역할의 실제 모델은 별도 `gpt-6-sol`이다. 기존 신규 `Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs`와 `Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs` 내부에 행렬·실제 결정 음성·도달 가능한 재진입 시험을 추가한다. 기존 하위 ABD4/F80 및 관련 정상 코드·회귀 이름을 보존한다. 양성 호출은 정확한 조립·선언 형식·전체 매개변수/by-ref·반환 형식으로 연결하고 불투명 실제 반환 참조를 유지한다. 같은 fixture의 name-only 호출 도우미를 정확한 허용 서명으로 좁히는 시험 보정은 승인한다. 비공개 필드는 읽기 증거만 허용하며 정상 권한/상태 대입은 금지한다.

분류 행은 같은 실제 cohort에서 매번 세 파일을 완전히 재설정하고 새 Capture를 발급받아 순회할 수 있다. owner 재확인 행은 정상 독립 cohort 또는 실제 취소·재무장으로 진행한다. 고정 행 ID·역할·실제 바이트 지문·decode/owner 결과·세대·권한 참조 증거를 기록하며 NUnit case 수와 내부 행 수를 구분한다. 실패 뒤 미실행 행을 통과로 집계하지 않는다. 실제 파일 잠금 Unreadable과 root-lock Busy를 구분한다.

추적은 REQ-M5D7QC3-002/003/004/005/007 및 AC-M5D7QC3-002/003/004/005/006/009/010이다. 실제 같은 스레드 렌더 콜백에서 stale 결정·소비 중 rearm 거부를 검증하고 구조상 callback-free Confirm을 별도 기록한다. live gate 재진입 실행 주장, 새 제품 delegate 통로, assembly/friend/public ABI·live UI·장면·자산·설정 수정과 C4 구현은 승인하지 않는다. 새 정확 지문·독립 검수·최종 집중과 필수 회귀 전에는 통과·수용으로 기록하지 않는다.

정확 기본값 직접 확인의 fresh 결속을 검증하는 읽기 증거로, 실제 Confirm 반환 원본 request를 키로 기존 lower `IssuedConfirmedRequests` 비공개 등록부의 정확한 형식과 `TryGetValue` 서명에 결속해 witness를 조회할 수 있다. 실제 발급 witness의 readonly `SourceResult`를 이전 display 및 새 actual capture·세 파일 지문과 대조한다. 등록부 Add/Remove/GetValue 생성 함수·private mint·상태 대입·C4 identity 추출은 허용하지 않는다. 이는 이미 승인된 비공개 필드 읽기 증거를 구체화하며 제품 API나 새 권한을 만들지 않는다.

실행 전 실제 시험 패키지의 `TestListenerWrapper.TestOutput` 본문이 비어 있음을 확인했다. 진행 채널 `TestContext.Progress`에만 쓴 내부 행은 증거로 보존되지 않을 수 있으므로, 시험 도우미의 출력 한 줄을 `TestContext.Out.WriteLine`으로 변경해 각 NUnit 시험 결과 출력에 남기는 제한 보정을 승인한다. 패키지·시험 실행기·제품 소스·180초 제한은 변경하지 않는다. 이전 동결·컴파일 자료를 보존하고 수정한 시험의 새 지문 및 편집 모드 컴파일을 별도 기록한다. 실제 XML에 377개 고유 내부 행의 계획·완료·실패·미실행 상태가 남는지 실행 후 대조해야 하며 출력이 없으면 통과로 집계하지 않는다.
