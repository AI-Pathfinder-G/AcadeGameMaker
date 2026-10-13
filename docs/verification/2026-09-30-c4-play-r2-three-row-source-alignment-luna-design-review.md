# C4 Play 세 행 근거 정렬 설계 검수

- 검토한 제안: `docs/proposals/2026-09-30-c4-play-r2-three-row-source-alignment-design.md`, SHA-256 `AE721318D3A67A983C08594468A6A11B01A790F07DDBDBB2B1D2E7840608361F`.
- 관련 실패 자료: 행 비교 `artifacts/c4-r2-focused-play-required-row-comparison.json`, SHA-256 `FC5E5356BF1680775478EA55F96C42A7DE87EC96F60AE3CB46F23C91C7061831`; 행 v8 SHA-256 `F7460A26F5ECA8722148F0B5273BE7BB8FCBAB39F2C431AA83D9C798A40830B4`; C4 r4 및 QA 증거 규약 r2.
- 판정 범위: 제안의 변경 허용성·증거 귀속·재생성/동결 경계만 정적으로 검토했다. 구현·Unity 실행·수용은 하지 않았다.

## 판정

P0 0건, P1 0건. 세 행 다섯 leaf의 제한 변경은 실제 비교 실패와 일치한다. 두 AC007 행에서 `ExpectedGuard.permanentFaultRecorded`와 대응 canonical RequiredFact를 모두 `No`에서 `Yes`로 바꾸는 것은 중첩 사실의 정확 일치를 위해 필요하다. 실제 결과는 각각 `ReloadRequired/C2Invoked`, `ManualRepairRequired/C2Invoked`이고, Bridge의 terminal 닫기 경로가 미완료 reset lifecycle을 fault로 기록할 수 있다. 이는 C4 r4의 DiskPrepared 뒤 비완료 terminal 요구와 충돌하지 않는다. 이 변경은 실제 fault를 정상으로 낮추거나 검사기를 완화하지 않는다.

AC008의 `authorityCorrelation.receipt`는 값 `Absent`가 양쪽에서 같으므로 certainty 하나만 `Observed`에서 `SourceEstablished`로 정렬하는 것이 맞다. 승인된 pre-C1 규칙은 결과·receipt 발급 사실을 C1 호출 전의 고정 source 경계로 제한하며, fixture도 실제 session/receipt 필드가 null임을 확인하고 해당 발급 불가 경계에 귀속한다. 이를 `Observed`로 고정하면 실제로 발급된 receipt 객체를 검증했다는 인상을 줄 수 있다. `Absent` 값 자체를 바꾸거나 다른 정상 FullBridge 행의 certainty 규칙을 완화해서는 안 된다.

따라서 정확한 범위는 두 AC007 행의 중복 표현 두 쌍, AC008의 certainty 한 곳, 합계 세 행·다섯 leaf다. 나머지 185행, 35개 Play 행의 식별·순서, 32개 NUnit 이름·행동을 유지한다. R2 실패 자료는 이 정렬의 근거로 보존한다.

## 재생성 및 동결 경계

기존 생성기는 fixture 안의 `ExpectedRowsJson`을 원천으로 사용하므로 JSON v9만 단독 편집하면 안 된다. 제안은 Play fixture의 지정 다섯 leaf와 현재 원장 참조 두 곳만 바꾸고, 일반 저장소 역사 참조 v1은 보존한다. 다른 Play 근거 포인터·NoOp 일곱 토큰의 정확 1회 검사도 유지한다. 기존 Edit fixture와 선택·시험 부모·제품 런타임은 변경 대상이 아니다.

행 v9를 생성기 재실행으로 발급하기 전에 동일 상대 경로의 격리 공간, 실제 복사본의 SHA, 생성기 고정 v1 출력의 사전 부재, 단일 실행, 실제 종료 0 및 stdout/stderr 요약을 증거 JSON에 남기도록 한 절차는 비재귀적이고 검증 가능하다. 다섯 leaf와 Play fixture SHA 외 행 구조 변화가 있거나 저장소 역사 v1이 바뀌면 실패해야 한다. 그 뒤 manifest v17이 최종 fixture·행·생성 증거를 고정하고, input v17 및 plan v13이 실제 경로/SHA를 연결하는 순서도 순환 참조를 만들지 않는다. v17 자기 경로 문자열을 fixture에 넣는 것은 manifest의 fixture SHA를 재귀적으로 포함하지 않는다.

새 stem과 여덟 출력 경로를 사용하고 나머지 일곱 실행도 직전 부재를 확인하는 조건은 기존 결과 덮어쓰기를 막는다. 180초 제한, 선택 이름, 실제 행 수와 의미를 그대로 유지한다. 후속 구현과 실행은 별도 승인이 필요하며 이번 검토는 그것을 승인하지 않는다.
