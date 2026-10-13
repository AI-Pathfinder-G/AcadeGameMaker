# C4 Play R2 현재 소스 결속 구현 정적 검토

검토 기준은 승인 계약 `docs/specs/work-contracts/2026-09-30-c4-play-r2-current-source-pin-amendment.md` SHA-256 `0DA1251B97D1A2E763B3A878110077C5FB5EB3E43565DA72C99A9D949D72EF1B`이다. Unity 실행 및 소스·원장 수정은 하지 않았다.

Play fixture `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs`의 SHA-256은 `F73BCC9EA0493214FC65B3005862DA5666F6EBEDD759C68CB9910D006D2C7B10`이다. `AssertFrozenNoOpSingleCallSource`는 v12 원장을 읽고 Bridge와 Owner의 정확한 Runtime 경로, 고정 SHA 및 실제 파일 바이트 SHA를 각각 대조한다. 현재 기대 지문은 Bridge `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`, Owner `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7`이다. 경로별 원장 중복 항목도 거부한다.

no-op 실행 표현식이 그대로 있는지 확인하고, Bridge의 네 토큰과 Owner의 세 토큰이 각 정확히 한 번 있는지 검사한다. 현재 Play helper에 이 일곱 검사가 모두 남아 있다. 일반 저장소 검증은 별도 `AssertFrozenOrdinaryWriterSource`에서 v1 원장을 계속 사용하며 이번 수정으로 이동되거나 완화되지 않았다. Play fixture의 부모와 기대 행·행동은 계약상 변경 대상이 아니며, 이번 검토에서는 제공된 소스 SHA가 지정된 지문과 일치함을 확인했다.

Edit fixture `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs`의 SHA-256은 `16DEB90F87C0E66A6DFD8C04E9C0DB6EC91F59F0051690853B61FA5C49489B0C`이다. 변경은 계약에서 허용한 현재 fixture 자기 결속을 v12로 갱신한 범위와 부합한다. Edit fixture의 역사적 `AssertFrozenNoOpSingleCallSource`와 일반 저장소 검증은 이 계약의 Play 보정 대상으로 바뀌지 않았다.

## 판정

P0 0건, P1 0건이다. 구현 소스는 승인된 한정 범위와 정합하며 단일 호출 검증과 v1 일반 저장소 검증을 보존한다. 다만 v12 원장은 아직 발급되지 않아 v12의 실제 바이트·fixture 자기 지문·입력 목록 결속을 대조할 수 없고, 실행 결과도 없다. 따라서 이 문서는 정적 구현 검토이며 실행 준비 완료나 테스트 통과 판정이 아니다. 후속 검토는 발급된 v12 및 입력·선택·행 자료의 정확 지문을 대상으로 해야 한다.
