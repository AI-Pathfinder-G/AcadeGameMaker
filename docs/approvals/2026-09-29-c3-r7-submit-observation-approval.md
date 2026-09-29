# C3 실제 Submit 최소 관측 승인

2026-09-29. 아스트라 승인. 상태 `Approved`.
REQ-M5D7QC3-001/005/007, AC-M5D7QC3-006/009/010을 추적한다.

[솔 r2 제안](../proposals/2026-09-29-c3-r7-edit-submit-minimal-observation-r2.md) 지문 `7DBDF90C6251718F6730512068132D10D76F6C3DDCB976A55BF801E820675536`와 [루나 r2 설계 검수](../verification/2026-09-29-c3-r7-edit-submit-minimal-observation-luna-r2-review.md) 지문 `DF278B7A7A878F68249F81C123FBEC4DD7846C64118B9A9EC4749F6D58996ABD`, P0/P1=0을 근거로 승인한다.

허용 변경은 기존 `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/NewGameConfirmationOwnerV1Tests.cs`의 단일 AC006 시험과 그 시험만 사용하는 비공개 관측 보조 함수이다. 원래 R7 바이트를 별도 이력으로 먼저 보존한다. 제안의 다섯 시점, 정확한 여섯 Router 원시 필드, 정적 환경 값과 원래 키보드 deviceId/added만 기록한다. 정확 선언 타입·조립·필드 타입·비정적 결속을 검증한다. 진단 오류는 별도 출력으로 남기고 기존 본문과 assertion을 계속 수행한다. 원래 assertion 예외를 잡아 통과시키지 않는다.

입력 이벤트·갱신·발행 프레임·권한·상태 대입·기존 assertion·180초 제한은 변경하지 않는다. 추가 action/asset/영수증 getter, enabled/isPressed 조회, 콜백 직접 호출, 내부 패키지 갱신 호출, 조립·friend·런타임·설정·meta 변경은 금지한다. 377행과 필수 562/51/610 선택을 보존한다.

테라 역할의 별도 `gpt-6-sol`이 구현하고 `gpt-6-luna`가 독립 검수한다. 동결된 새 소스와 정확 단일 이름 선택을 기록한 뒤 같은 단일 시험의 관측 실행 한 번만 수행한다. 이전 실행 증거를 덮어쓰지 않는다. 실제 종료값·이름·전후 지문·진단 오류와 관측값을 보존한다. 관측은 기존 실패를 통과로 전환하지 않으며 원인 단정·제품 수정·C3/C4 수용을 승인하지 않는다. 후속 수정은 실제 결과에 따른 별도 승인이 필요하다.
