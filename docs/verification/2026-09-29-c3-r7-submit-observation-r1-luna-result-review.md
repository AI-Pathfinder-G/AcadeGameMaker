# C3 R7 Submit 관측 실행 독립 결과 검토

실제 XML SHA-256 `9675F13A7E8E0485CB96072BE76F57A189D260BFF358E390A5FE70AFA65ECDB3`을 원문과 대조했다. 관측 결과는 다섯 지점이 각 1회, 진단 오류 0회다. 모든 지점에서 `LatestUpdateType=Editor`, `IsPlaying=false`, `IsFocused=false`, `UpdateMode=ProcessEventsInDynamicUpdate`, `Mode=UIOnly`, `UiCallbacksRegistered=true`, `CallbackOrdinal=0`, `UiSubmitPressed=false`, 장치 `DeviceId=1`/`Added=true`다. 첫 등록 시점에는 `CaptureSuppressed=true`와 `UiEnableQuarantinePending=true`; released Update 직후부터 이후 시점까지 둘 다 false로 유지됐다.

**관측 기록 P0 0, P1 0; 수용 기준 AC006은 여전히 실패(P1 미해결).** XML은 정확히 선택된 단일 AC006 시험을 기록하며 `Total=1, Passed=0, Failed=1, Skipped=0`, 원래 `real published Submit edge` assertion에서 기대 true/실제 false다. NUnit 실행은 14.5223192초에 실패 종료했고, 별도 비교 JSON도 이름 차이/중복 0, 884개 입력의 전후 차이 0, `Verified=false`, QA 반환 코드 5를 기록한다. 부모 실행 관찰의 Unity 종료 코드 2도 실패 결과와 일치한다. 이번 실행은 원래 실패를 성공으로 바꾸지 않았다.

관측값은 실제 Editor 갱신 종류와 Router Submit callback 카운터/edge 미변화가 함께 나타난 것으로, 설치된 패키지의 Editor action 처리 제외 경로와 **일치한다**. 이는 장치 Enter 값이 상태에 반영됐는지, action 자체가 수행됐는지, 또는 Editor 미집중 상태 중 어느 하나가 단독 원인인지 증명하지 않는다. r2가 의도적으로 장치 값과 action getter를 제외했기 때문이다. 제품 fault를 확정하거나, Play 시험으로 이동하거나, 입력 설정/fixture를 보정할 근거로 쓰면 안 된다.

관측 출력은 XML `<output>`에 원시 JSON 다섯 행으로 실제 보존됐고 `C3SubmitObservationError`는 없다. 현재 확정 가능한 결론은 실패한 Editor 경로에서 Router Submit callback이 관찰되지 않았다는 데까지다. 추가 실행·코드 변경·컴파일은 없었다. C3 전체 및 C4 수용과 제품 원인 판정은 미완료다.
