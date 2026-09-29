# C3 실제 PlayMode fixture 배치안 독립 설계 검수

- 검수자·실제 모델: 루나, `gpt-6-luna`
- 대상: `docs/proposals/2026-09-29-c3-playmode-fixture-placement.md`
- 대상 SHA-256: `8F6B2F907E54D76A16D72B372E249C32329309E4E92BA26DF3EF9EB9834CBB50` (일치)
- 범위: 설계 문서 정적 검수. 소스·시험 수정 및 실행 없음.

## 판정

P0는 0건이다. 제안은 현재 Hub PlayMode friend가 없어 임시 root 환경을 직접 주입할 수 없다는 제약에 맞춰 기존 `AcadeGameMaker.Input.Unity.PlayMode.Tests` 조립에 신규 시험 파일과 `.meta`만 추가한다. fixture가 실제 preparation을 호출하고 실제 임시 저장 루트를 쓰며, Hub API의 실제 반환 객체를 opaque handle 그대로 전달하는 경로는 승인 C3의 synthetic 단계와 일치한다. private 상태·proof·receipt 제조, 다른 시험 private factory, asmdef/friend/runtime/public API 변경, 실제 persistent 경로 사용을 배제한 경계도 적절하다.

## P1 보완 조건

1. 참조 조회 호출은 시험이 의도한 제품 메서드만 실행하도록 assembly-qualified 형식명, 선언 형식, 정확한 매개변수 형식·반환 형식으로 결속해야 한다. `GetMethod` 이름 단독 검색이나 비슷한 overload로의 대체를 허용하지 말고, 해석 실패·호출 예외는 시험 실패로 드러내야 한다. opaque 반환물은 값 복사·재구성 없이 실제 반환 객체와의 동일성 확인 후 다음 API에 전달해야 한다.
2. 별도 PlayMode focused selector와 정확한 예상 이름 원장을 확정하고, 기존 536개 및 fault 74개 이름을 보존해 신규 시험과 분리 집계해야 한다. 새 fixture는 아직 실행되지 않았으므로 설계 통과를 컴파일·runner 포함·실행 증거로 표시할 수 없다.

## 승인 경계

이 배치는 현 Approved C3 시험 허용 목록에 없는 위치 변경이다. 아스트라가 신규 시험 파일 두 개 경로만 허용하는 제한 보정을 승인하기 전 구현하면 안 된다. 실행 가능한 실제 UI 구성, inactive cohort의 환경 주입 순서, 가상 입력과 `LateUpdate` 게시 순서는 구현 후 정확한 selector 및 필수 회귀에서 입증해야 한다. 자료가 없으면 private 상태 조작으로 대체하지 말고 누락 경계로 보고한다.

## 후속 보완 검수 — 2026-09-29

- 후속 제안 지문: `BAB0264267FA79583C57AD121FC83E28DEE28E2C99DBCA2C7183C9C1868EAF99` (일치)
- 후속 검수자·실제 모델: 루나, `gpt-6-luna`

새 지문은 기존 P1 두 조건을 구체화해 폐쇄한다. reflection 형식 해석은 assembly-qualified 이름과 실제 assembly/full name을 대조하고, 메서드는 `DeclaredOnly` 및 선언 형식·호출 종류·전체 매개변수(by-ref 포함)·반환 형식으로 결속한다. 이름 단독 또는 유사 overload fallback을 금지하고 해석·호출 오류를 시험 실패로 남긴다. opaque 권한 객체는 정확한 형식과 최초 반환 참조 동일성을 검증한 뒤 그대로 전달하도록 명시했다. 회귀는 기존 536개와 fault 74개 원장을 신규 집중 원장과 분리하고, 실제 selector 인수·runner 전체 이름 XML·입력 전후 지문·결과·종료값을 기록해 누락/초과/중복 및 비성공 결과를 차단한다.

후속 설계 정적 판정은 P0=0, P1=0이다. 최초 검수의 P1=2는 당시 지문에 대한 이력으로 남긴다. 이번 폐쇄는 시험 파일 추가 승인이나 컴파일·runner 열거·실행 증거가 아니며, 아스트라의 별도 Approved C3 경로 보정 전에는 구현할 수 없다.
