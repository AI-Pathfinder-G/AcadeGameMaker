# GLM 첫 구현 마일스톤 QA 보완안과 Sol 심사

- Date: 2026-08-25
- Status: Screened draft; not verification evidence
- Draft: Ollama Cloud GLM 5.2
- Acceptance owner: Sol
- Independent verifier: Luna

## 제공 범위

GLM에는 저장소 원문이나 비밀을 제공하지 않고 Unity bootstrap과 VD-01 이동 수치, 기존 `AC-PLAT-001/005`, `AC-MOV-001~006`, 현재 QA catalog 요약만 제공했다.

## 수용한 보완점

- 경계값은 통과 쪽과 실패 쪽을 한 쌍으로 검증한다: coyote·buffer 6/7 tick, dash cooldown 35/36 tick, same-wall lock 5/6 tick.
- 가벼운 modifier의 진입·회수에서 gravity·air acceleration·fall cap이 기준값으로 정확히 복구되고 dash·wall-jump 불변값이 오염되지 않는지 검사한다.
- 순수 상태 모델의 EditMode 테스트와 Rigidbody2D adapter의 PlayMode 테스트를 분리한다.
- PlayMode는 wall clock이나 render frame이 아니라 명시적 60Hz simulation tick으로 진행하고 동일 입력 기록을 반복한다.
- OS 입력을 직접 폴링하지 않고 의미 명령 또는 Input System test event를 주입한다.

## 반려·교정한 항목

- GLM이 일부 테스트에 잘못 연결한 AC ID는 수용하지 않는다. deadzone·coyote·buffer는 `AC-MOV-004`, dash·wall은 `AC-MOV-005`, 가벼운 상태는 `AC-MOV-006`을 따른다.
- Unity Project Settings에 존재하지 않는 “최소 해상도 설정” 정적 검사는 반려한다. 640×360은 실행 창·표시 검증 fixture다.
- `1e-5` 같은 새 허용오차는 반려한다. 각 AC에 이미 승인된 허용오차를 그대로 사용한다.
- Maximum Allowed Timestep 변경 제안은 제품 계약 밖이므로 반려한다. 테스트 runner가 tick을 명시적으로 구동해야 한다.
- GLM이 요청 범위를 넘어 인용한 `AC-PLAT-002`는 첫 bootstrap 작업 계약에 포함하지 않고 후속 Windows smoke 계약에서 다룬다.

## 첫 마일스톤 적용

- static: Unity version, URP 2D, Input System 1.20.0, fixed 60Hz와 Player Settings를 `AC-PLAT-001/005` 범위에서 검사한다.
- EditMode: deadzone, buffer/coyote, dash cooldown, wall lock, modifier swap을 `AC-MOV-004~006`에 연결한다.
- PlayMode: run·jump·release·dash·wall slide/jump·light modifier를 `AC-MOV-001~006`의 승인 허용오차로 3회 재생한다.
- manual: 640×360과 2560×1440 sandbox capture는 구현 증적이 아니라 Luna 검수 입력으로 보존한다.

이 문서는 GLM의 보완 의견을 Sol이 선별한 QA 설계 입력이며 PASS 또는 Approved를 의미하지 않는다.

