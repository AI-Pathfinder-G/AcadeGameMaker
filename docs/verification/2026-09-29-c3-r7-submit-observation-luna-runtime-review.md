# C3 R7 Submit 관측 구현 독립 정적 검토

승인 문서 SHA-256 `4351CA9C817B706597B26898C383ECCADDAB0011C24485B6D45E3A4D20363190`, 대상 Edit 시험 SHA-256 `DF9FE80D362457ED8DEBB03556C644F8D669ADF03C20CBE74BFD5A695B691B3C`, 단일 선택 SHA-256 `FE38051F501253B5D866F6BC76080924DE89E52DC48ADD7563520D780FE04481`, 구현 증거 JSON SHA-256 `5E1B121A58B7D97D9B42ADFF0C026408894739524C6011AB20B95DB68989B09E`를 대조했다.

**P0 0, P1 0.** 정적 구현은 승인 범위와 일치한다. 기존 AC006 본문에는 관측 5회만 추가됐고 새 시험 선언은 없다. 관측 지점은 키보드 등록 직후, 원래 released Update 직후, Rearm 직후, Enter Update 직후 Step 전, Step 직후로 제안의 순서를 따른다. 기존 입력 갱신·Take/Cancel/Rearm·Step·원래 assertion과 후속 기존 시험 동작은 보존됐고, 입력/권한/제품 상태를 쓰거나 새 프레임을 발행하는 관측 호출은 없다.

보조 함수는 시험 본문에 진입하기 전 정확한 Router/`InputMode` 선언 이름과 조립, 여섯 필드의 선언형·이름·타입·비정적 여부를 결속한다. 필드 목록은 승인된 `_callbackOrdinal`, `_uiSubmitPressed`, `_captureSuppressed`, `_uiEnableQuarantinePending`, `_uiCallbacksRegistered`, `_mode`뿐이며 읽기는 `GetValue`로 한정된다. 입력 액션·바인딩·장치 enabled/isPressed·추가 제품 getter는 없다. 진단 실패는 별도 NUnit 출력으로 기록하고 삼키며 기존 시험 assertion은 진단 처리 밖에 남아 있다. 선택 원장은 기존 AC006 정규 이름 1개를 정확히 지정한다.

R7 보존 manifest의 14개 경로를 현재 바이트와 대조했다. 승인된 Edit 시험 파일 1개만 지문이 바뀌었고 다른 13개는 이전 지문과 일치한다. 구현 증거의 기존 텍스트 복원 및 377행 보존 주장은 참고했고, 런타임 의미 검증이나 테스트 통과로 확대하지 않는다.

정적 검토상 단일 관측 실행 전 차단 결함은 없다. 실제 NUnit XML에 로그가 남는지, 정확한 runtime 관측값과 원래 Submit assertion 결과는 아직 실행되지 않아 확인되지 않았다. 이번 검토는 코드 정적 대조만 수행했으며 Unity/컴파일은 실행하지 않았다.
