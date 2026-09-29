# C3 감사 시험 식별 정보 제한 보정 승인

아스트라는 실제 Unity r5 집중 편집 첫 실행의 종료 1과 XML 부재를 확인했다. 로그에서 신규 감사 시험 `.meta`의 33자리 식별값 때문에 자산을 무시했고, 이에 따라 기존 감사가 참조한 `HubMenuIntentHandoffC3AuditInputV1`을 찾지 못했다. 전후 입력 자료에는 지문 차이가 없다. 이 실패를 보존하며 정적 검수·기존 참조 컴파일을 새 Unity 통과로 주장하지 않는다.

허용 수정은 `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffC3AuditSuccessorTests.cs.meta`의 식별값 `50b803be06dd4777ac62bd70ca5368f5e`에서 마지막 한 글자를 제거한 32자리 `50b803be06dd4777ac62bd70ca5368f5`다. 438개 메타 파일을 읽기 전용으로 확인해 잘못된 형식은 이 한 건이며 유효한 식별값의 중복은 0, 새 값의 기존 사용도 0이다. 코드·검사·기존 선택 이름·조립·씬·설정은 변경하지 않는다. 추적은 REQ-M5D7QC3-007 및 AC-M5D7QC3-009/010이다.

테라가 이 한 항목을 보정하고 루나가 원본·새 지문과 형식·중복을 독립 확인한다. 아스트라는 이전 원장을 보존한 새 원장과 실패 기록을 작성한 뒤 실제 집중 시험을 재실행한다. C3 전체와 C4의 미수용 범위는 유지한다.
