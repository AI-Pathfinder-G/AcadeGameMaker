# C3 R8 Submit 이전 구현 독립 검토

대조한 기준은 승인 `docs/approvals/2026-09-29-c3-submit-playmode-transfer-approval.md` SHA-256 `27764BD15B96C6C3746EB9E4C5556C9258B8ADC0C90040EF99820A8EBBEF9CFA`, R8 동결 원장 `artifacts/c3-upper-r8-frozen-source-manifest.json` SHA-256 `200C8A3D1932DA3297A9FA9D431ACC4665C821E4FBDB937E83AC55161AD627F3`, 구현 증거 `artifacts/c3-r8-submit-transfer-terra-evidence.json` SHA-256 `5FB100DE6CBC05D6DDAC3C3C3F5D72844ECEAEE5A991772B15F6285ACC3216D4`다. 새 Edit SHA `A0A1928CE83A22D324F785476B38DCDE595B71E53122139F10535C5B3AF6F9D6`, Play SHA `FC431157BEBF276EDA2C36ACF7682E2C3144A999AF39A18375D492B1D0D116A2`다.

**P0 0, P1 0.** 구현은 승인한 Edit AC006 및 전용 진단 helper 제거와 기존 Play AC006 강화로 한정된다. 동결 원장의 14개 경로를 재해시한 결과 전부 일치했고, 변경은 허용된 두 시험 소스에 한정된다. R8 기대 행렬은 377행이며 R7 기대 행렬과 실제 직렬화 대조에서 모든 행이 같고, 91개 고정 행렬 이름과 기존 generator 두 파일도 바이트가 유지됐다. 과거 Edit 실패와 관측 원장은 이력으로 보존되며 실행 성공으로 재분류되지 않았다.

강화된 Play 시험은 기존 InputTestFixture 기반 실제 키 이벤트 흐름을 유지한다. 재예약 후 추가 빈 발행 없이 첫 실제 Enter 제출을 확인하고, 첫 프레임 폐기 뒤 successor baseline 해제·Ready 상태·retained 없음·다음 take 거절을 검증한다. 원래 cursor 동일성, retained intent 동일성, 이미 take한 Q-B 요청 동일성 및 `RequestTaken` 이력을 확인한 다음 release와 두 번째 Enter를 거쳐 새 actual take/Accept 결과 `DecisionRequired`와 새 decision 참조를 검사한다. actual handle은 기존 Take/Accept 정상 경로에서 전달되며 새 권한이나 프레임을 직접 제조하지 않는다. Outcome은 기존 정확 형식/선언에 결속한 getter로 읽는다. Edit 쪽의 사용하지 않는 단일 실패 시험과 전용 SubmitObservation helper가 제거되어 중복 진단도 남지 않았다.

R8 집중 선택 240 Edit/15 Play와 377행 원장은 실행 전 원장으로 확인된다. 별도 분할 계획의 네 정확 선택은 Edit 91+149, Play 2+13이며 각 selector가 선택 파일의 모든 정규 이름과 일치한다. 두 플랫폼별 분할 합집합은 기존 선택과 같고 중복/누락 0이다. 예상 행렬 JSON은 377행·91개 고유 case이며, 계획상 인자 길이는 최대 24,168자로 Windows 32,767자 한도보다 작다. 원본 선택·generator는 보존된다. 분할 실행은 각각 별도 실행 증거로 기록해야 하며, 그 결과를 하나의 동시 실행이었다고 표현해서는 안 된다. 테스트 간 교차 실행 상태가 AC 요구로 별도 확인될 경우에는 분할 결과만으로 그 상호작용을 주장하지 않는다.

실행 전 검토이므로 첫 실제 Enter frame 폐기와 다음 정상 선택은 아직 런타임 증거가 아니다. Unity/컴파일 실행은 없고 통과, 제품 원인, C3 전체 또는 C4 수용을 주장하지 않는다. 실제 실행 후에는 정확 이름 집합, 각 native 종료 결과, XML, 최종 14개 소스 지문, 실제 377행 case 결과와 전후 입력 지문을 각 부분별로 대조해야 한다.
