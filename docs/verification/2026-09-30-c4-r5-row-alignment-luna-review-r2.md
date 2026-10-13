# C4 R5 행 정렬 수정안 독립 재검토

- 대상 초안: `docs/specs/work-contracts/2026-09-30-c4-r5-evidence-row-alignment-draft.md` — SHA-256 `7329EF527318EF974C52D38AC0C608AB12AD416F406C0398E77FEAA990A14685`
- 이전 검토: `docs/verification/2026-09-30-c4-r5-row-alignment-luna-review.md` — SHA-256 `A57B9CDA91789F5E00769DDD63BE79B51DA86A2160CB0BEBB61861B93B67D2FC`
- 범위: 초안과 현재 Owner·fixture·부모 시험의 정적 대조. Unity, 컴파일, QA, 소스 수정은 수행하지 않았다.

초안의 다섯 번째 보정은 이전 P1을 닫는다. `Create(false)`는 fixture 생성 때 `_initialCommitHistory`를 null로 잡는다. 해당 부모는 미커밋 실행 거절을 확인한 뒤 같은 실제 `f.Confirmed`를 `CommitForExecution`에 넘기고 반환 참조를 확인한다. Owner는 그 호출에서 실제 하위 예약을 만들고 `CommitHistory`를 구성한 다음 커밋을 완료하며 `_commitHistory`에 기록한다. 기록은 `Previous`, `Confirmed`, `Reservation`을 보유한다. 따라서 커밋 반환 직후, C4 실행 전에 현재 Owner의 기록이 존재하고 `Previous=null`, `Confirmed`가 `f.Confirmed`와 동일하며 `Reservation`이 존재함을 읽고, 그 정확한 기록 참조를 fixture의 기대 기준으로 보관하는 절차가 실행 이력의 의미와 맞는다. 이후 기존 `ObserveActualHistory`의 참조·이력 연결 검사는 실제 사용된 커밋 기록의 보존을 검증하게 된다. 이는 테스트 fixture의 기대 스냅샷만 갱신하고 제품 Owner나 권한 상태를 쓰지 않는다.

초안은 이 추가 기준점에 필요한 범위를 명시했다. 부모 시험의 호출 위치 한 곳, fixture의 읽기 검증 및 fixture 자체 기준 참조 보관, 해당 행의 기존 `ExpectedHistory=Yes` 유지, 선택·행·동결 manifest·입력 목록·계획의 새 지문 결속이 포함된다. 사례명, selector, REQ/AC, 런타임, 실행 순서는 그대로다. 기준점 포착이 확인되지 않으면 재실행 전 중단하도록 하며, 결과를 미리 수용하지 않는다.

나머지 보정도 이전 원자료 판정과 일치한다. C1 checkpoint·terminal 실패 55행은 실제 fault 이력을 반영해 `permanentFaultRecorded=Yes`로 정렬한다. 정상 완료 24행은 실제 최종 marker를 `NoBarrier`로 기록하고, 같은 사례의 일반 Save가 없으므로 writer 차단만 제한된 `Unknown`으로 남긴다. 최종 장벽 6행은 완료 후 상태를 `NoBarrier`로 정렬하되, AC006의 준비 장벽 아래 실제 Save 차단 `Yes`와 callback 증거를 보존한다. 미반환 17행은 결과 발급 전 Bridge 경계의 정확한 동결 포인터를 사용하고 실제 예외·결과 부재·CWT 근거를 계속 요구한다. 이 수정은 실패·부재 증거를 일반적인 Unknown이나 정적 추론으로 바꾸지 않는다.

기존 R5 실행은 139/139 NUnit 통과였지만 필수 행 148개 중 86개가 불일치했고 큐가 fail-stop했다. 이 초안은 그 결과를 보존하고, 수정 뒤에도 비교 결과가 일치하기 전까지 수용으로 간주하지 않는다. 새 fixture/시험 및 출력 지문은 아직 발급되지 않았으며 이 설계 재검토는 구현 적합성이나 Unity 통과를 입증하지 않는다.

판정: P0=0, P1=0. 이전 기준점 P1은 이번 초안에서 필요한 부모·fixture·원장 지문 변경까지 포함해 설계상 폐쇄됐다. 실제 구현·새 자료·후속 실행은 별도 독립 검수가 필요하다. C3 전체 수용이나 C4 진행 허가를 뜻하지 않는다.
