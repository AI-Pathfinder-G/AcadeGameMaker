# C3 재진입 증거의 제한 범위

- 날짜: 2026-09-29
- 상태: 시험·해석 제안 — 실행·P1 폐쇄·통합 수용 아님
- 설계자·실제 모델: 솔, `gpt-6-sol`
- 추적: `REQ-M5D7QC3-003/005`, `AC-M5D7QC3-003/005/006/009/010`
- 제품 delegate/callback seam, 상태/권한 반사 대입, 코드 수정 및 Unity 실행 없음.

## 실제 시험과 구조 증거의 조합

현재 Confirm은 실제 발급 decision 인증→operation guard→decision CAS→실제 lower
Capture/재관찰→기존 발급 결과 검증으로 진행한다. 정상 경로에는 UI 렌더나 외부
delegate 호출이 없다. 따라서 live ConfirmInspecting 구간에서 같은 스레드의 정상
외부 콜백이 Confirm/Cancel을 다시 호출했다는 실행 증거를 현 구조로 만들 수 없다.
private `_operation`/decision 상태를 대입하거나 제품 콜백 seam을 추가해 통과한
시험을 실제 정상 재진입으로 보고하지 않는다.

실행 가능한 재진입 반례는 실제 Cancel로 발급받은 rearm capability를 사용한
Rearm 내부의 Q-A 렌더 단계다. Unity UI `Graphic.RegisterDirtyVerticesCallback`으로
실제 그래픽의 dirty 콜백을 등록하고 **실제 렌더 호출 중 같은 스레드**에서
Confirm(oldDecision), Cancel(oldDecision), Rearm(actualRearm)을 각각 시도한다.
참조는 모두 실제 정상 반환 객체이며 callback은 시험 그래픽에만 등록한다.

외부 Rearm 중 owner는 Rearming, 이전 decision은 이미 소비되고 rearm의 소비가
예약돼 있다. 중첩 Confirm/Cancel은 정상 stale decision 인증에서, 중첩 Rearm은
정상 Cancelled-state 검사에서 거부된다. 이 실행은 **재진입 시도의 비변경 거부**를
증명하지만 live decision CAS나 operation guard 내부의 재진입 경로를 실행한
증거가 아니다. 이름과 결과 원장에 이 구분을 명시한다.

## 최소 정상 fixture 순서

1. 승인된 실제 임시 root/cohort와 실제 NewGame take/intake로 decision을 발급한다.
   실제 Cancel을 호출해 oldDecision과 actualRearm을 보관한다.
2. 실제 `Graphic`/TMP 그래픽 한 개를 선택한다. rearm Render가 실제 dirty 전환을
   만들도록 **시험 UI 표시값**만 준비할 수 있다. 예를 들어 등록 전 label의 public
   text를 다른 시험 문자열로 설정하고 rearm이 정상 copy를 다시 Render하게 한다.
   owner·epoch·receipt·proof·capability 및 private 제품 상태는 바꾸지 않는다.
3. 그래픽에 dirty 콜백을 등록하고 actualRearm으로 정상 Rearm을 한 번 호출한다.
   callback 안에서 원래 호출 스레드 ID와 같은지 확인하고 nested 세 호출 각각의
   정상 거부를 포착한다. callback을 직접 호출하거나 public SetVerticesDirty를
   별도로 불러 실제 Rearm 도중 콜백인 것처럼 대체하지 않는다.
4. 콜백 진입을 실제로 관찰해야 한다. 콜백이 없거나 경로가 달라졌으면 이 행은
   실패이며 성공/건너뜀으로 처리하지 않는다. 중첩 호출 직전/직후 읽기 snapshot의
   generation·capability 참조·history·pending 상태와 파일 hash가 같음을 확인한다.
   외부 정상 Rearm의 세대 전진과 중첩 호출의 비변경을 별도로 대조한다.
5. 외부 Rearm이 정상 완료한 뒤 checked epoch 증가 1회, append-only 역사 1회,
   pending baseline을 확인한다. 실제 cursor 첫 frame 폐기와 다음 정상 새 선택이
   성공해야 중첩 시도가 합법 외부 작업을 종료하지 않았음이 증명된다. callback은
   finally에서 해제하고 해당 시험 UI/cohort만 정리한다.

시험 이름 예시는 `AC003_RearmGraphicCallbackRejectsSameThreadStaleConfirmCancelAndRearm`이다.
관련 구조 행은 `AC003_LiveConfirmHasNoExternalCallbackBeforeDecisionClosure`로
분리한다. 후자는 실제 실행 재진입 사례 수에 넣지 않는다.

## 필수 구조 대조와 승인 해석

동결 소스의 Confirm/Cancel/Retry/Rearm 진입 검사, `Enter`의 CAS, decision
Pending→ConfirmInspecting/Consumed 전이, Busy에만 Pending 복귀하는 경계를 실제
줄 번호와 지문으로 기록한다. Confirm의 실제 호출 사슬에 렌더·delegate·UnityEvent·
사용자 코드 callback이 없음을 확인한다. 실제 동시 Confirm/Cancel 단일 승자,
중복·late·foreign·stale capability 실행 행은 별도로 유지하며 이번 재진입 구조
자료로 그 실행 행을 대체하지 않는다. 결론은 "도달 가능한 재진입 시도는 실제
거부, live Confirm 같은 스레드 callback 경로는 현 구조에서 도달 불가"로 한정한다.

솔의 권고는 현행 AC003의 reentrant 항목을 이 실제 시험과 구조 대조의 조합으로
판정하는 것이다. 아스트라가 이 해석을 명시적으로 확인해야 하며, 필요하면 다음
문구를 Approved 계약의 제한 보정안으로 기록한다.

> 재진입은 제품의 실제 외부 콜백 경계에서 발생하는 중첩 호출의 비변경 거부를
> 실행으로 확인한다. 정상 Confirm의 예약 중 외부 콜백 경계가 없는 경우 정확한
> 동결 소스의 도달 불가 구조 증거를 함께 기록하며, 합성 callback seam이나 상태
> 대입으로 live 예약 중 재진입 실행 증거를 제조하지 않는다. 실제 동시·late·중복
> 호출과 결정 단일 승자 요구는 그대로 유지한다.

이는 시험 접근/증거 범위의 솔·아스트라 판단이며 제품 동작 변경이나 사용자 제품
결정을 요구하지 않는다. 루나가 새 정확한 증거를 검수하고 아스트라가 승인하기
전에는 AC003 전체 통과나 재진입 실행 완료를 선언하지 않는다.
