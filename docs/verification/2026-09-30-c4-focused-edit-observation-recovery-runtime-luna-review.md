# C4 집중 Edit 관측 복구 구현 정적 검토

- 승인 계약: `docs/specs/work-contracts/2026-09-30-c4-focused-edit-observation-recovery-amendment.md`
- 승인 계약 SHA-256: `7CDBEE47862C02C81D859F8902F5A8A3458157350E10D9CE7C825A8553B8CBD7`
- 검토 범위: 지정된 Edit fixture, Play fixture, Edit test, 큐의 정적 구현만. Unity/컴파일/큐 실행은 하지 않았다.

## 판정

P0 0건, P1 0건이다. 계약에서 요구한 관측 보정과 결과 의미 보존이 현재 바이트에 반영됐다. 새 선택·행·동결 원장·입력 목록·계획은 아직 발급되지 않았으므로 실행 전 결속 검토와 실제 재실행은 남아 있다.

## 확인한 보정

- 현재 시험 파일을 검사하는 두 Edit fixture 참조와 한 Play fixture 참조는 manifest v8을 읽는다. 선행 제품 소스를 확인하는 나머지 다섯 v1 참조는 보존됐다.
- C1의 51개 사례 중 40, 41, 44–49는 callback 방문을 계수하고 최초 방문에서만 예외와 행 도달을 기록한다. 각 지정 사례는 두 방문, 그 밖은 한 방문을 단언한다. 기존 outcome, marker, barrier, guard 및 terminal 거절 단언이 유지됐다.
- 여섯 fresh handback 사례의 공통 경로는 `ExecuteOwner()` 뒤 중복 handback을 하지 않으며, 다음 세대 확인 및 과거 commit/take/cursor 보존 검사는 남아 있다.
- 체크포인트 19는 lower 예약 생성·등록 전, 20은 lower 예약 CWT 등록 뒤지만 lower 함수 반환 전이다. fixture는 실제 Owner operation/capability를 확인하고 두 시점 모두 Owner capability CWT 부재를 검사한다. lower 기록은 19에서 미등록, 20에서 등록이어야 하며, 21 이후에는 Owner CWT의 실제 동일 참조를 검사한다. 체크포인트별 lower reservation 상태를 합치지 않는다.
- AC003의 중복 호출 검사는 첫 `FinalizeReset` 전체 호출 문자열 뒤부터 검색한다. 기존 validator 순서와 proof 관측 검사는 그대로다.
- Edit/Play fixture literal은 각각 승인 행 원장의 148/148 및 35/35와 일치했다. 나머지 5 strict 행을 포함한 승인 원장 전체는 188행이다. 시험 메서드·케이스 선언과 행 의미를 바꾸는 허용 변경은 확인되지 않았다.
- 큐는 결과 경로를 `c4-final-validation-queue-result-v5.json`으로 고정하며 기존 파일을 거부한다. 기존 fail-stop, 자식 표준오류·종료값 관찰, 계획 결속, 잠금 및 결과 `CreateNew` 경로가 유지됐다.

## 정확 지문

- Edit fixture: `8FE0785955DD786D394FF5E8057AB352D8ED94CA4BFC0EAC2AD64628E77B2214`
- Play fixture: `30330B3276F61F9686D9743CD7BFCA20B5CE6A5FFDC2E0994F8847949FAB1A0E`
- Edit tests: `5F6E3C8D3ADC0EA3C5A2C616356043B8BA450F5BEBBFFCC39962A6A40B3E7F71`
- 큐 도구: `25B09F07FB78DD5289B2078BE8600A41850E331CD62B38D1B3E188F274343361`

이 정적 판정은 아직 발급되지 않은 v3 선택·행 자료, manifest v8, input v8, plan v6의 정확 바이트 결속이나 Unity 실행 통과를 증명하지 않는다. 승인된 계약의 제한 범위에서만 구현 검토를 마쳤다.
