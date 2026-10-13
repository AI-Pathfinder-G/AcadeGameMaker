# C4 Play R2 v12 동결 자료 독립 검토

검토 기준은 승인 계약 `docs/specs/work-contracts/2026-09-30-c4-play-r2-current-source-pin-amendment.md` SHA-256 `0DA1251B97D1A2E763B3A878110077C5FB5EB3E43565DA72C99A9D949D72EF1B`이다. Unity 실행 및 원본·결과 변경은 하지 않았다.

## 대조 결과

- v12 소스 원장 `artifacts/c4-frozen-source-manifest-v12.json` SHA-256 `7A18812EF9C32BA7D3328B1ACABE77883CB114A0585793A7AC2F0F47805E8EA8`은 137개 경로를 갖는다. v11의 131개 기존 경로는 상대 순서가 유지되고, SHA 변경은 승인된 두 fixture뿐이다. 추가 6개 증거 경로가 있으며 추가·변경 항목은 `AllowedChanges`에 연결되어 있다. 137개 경로는 모두 존재하고 실제 SHA가 원장과 일치한다. 원장은 자기 파일 및 입력 목록 파일을 자기 `Files` 안에 넣지 않는다.
- Bridge `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`, Owner `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7`, Edit fixture `16DEB90F87C0E66A6DFD8C04E9C0DB6EC91F59F0051690853B61FA5C49489B0C`, Play fixture `F73BCC9EA0493214FC65B3005862DA5666F6EBEDD759C68CB9910D006D2C7B10`의 실물 지문이 원장과 일치한다.
- 입력 목록 `artifacts/c4-frozen-input-paths-v12.json` SHA-256 `132DDADB891DD24629B324DB569BBB30CC8821B798971E8EBA7EB70C9B74E2C6`은 1,024개 고유 경로다. v11의 1,017개 경로와 사유는 모두 같은 상대 순서·값으로 보존되고 7개 경로가 추가됐다. 목록은 대소문자 구분 정렬이며 1,024개 사유 항목도 고유·일대일이다. 모든 입력 경로가 존재한다. source manifest와 rows v7, Play 선택 v3가 포함되고 입력 목록 자체는 제외된다. 선언된 출력 제외 규칙은 그대로 유지된다.
- rows v7 `artifacts/c4-required-checkpoint-rows-v7.json` SHA-256 `E637FC829F1316327866CF0F13305F56A23258341FE0FA4BE80367F853B93C4D`는 188개 행의 값·순서가 v6와 같고 `EnumSources`도 같다. 상위 `SourceFiles`에서 Edit·Play fixture 두 지문만 새 바이트로 갱신됐다. Strict 행과 다른 행 내용은 바뀌지 않았다.
- Play 선택 v3 `artifacts/c4-focused-play-selection-v3.json` SHA-256 `F572205953FCCD7EC50C1D6E586B8F84F036B22D939182B1022AC071C8E8E8EE`는 32개 이름·순서·선택식·부모 파일 지문이 v2와 같다. 다만 아래 P1에 따라 `ParserEvidence.EnumSources`의 Bridge 지문만 v12/rows v7과 일치하지 않는다.

## 발견 사항

P0 0건, P1 1건이다. Play 선택 v3의 `ParserEvidence.EnumSources`에는 Bridge의 과거 SHA `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`가 남아 있다. v12 원장과 rows v7은 현재 Bridge SHA `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`를 가리킨다. 이름·선택식은 같지만 선택 자료의 파서 근거가 동결 소스와 일치하지 않아 독립적으로 완전한 현재 출처 증거로 볼 수 없다. 다음 승인된 증거 개정에서 32개 이름, 순서, 선택식, 부모 SHA를 보존하면서 파서 출처 SHA를 현재 Bridge로 갱신한 새 선택 파일을 발급하고 입력 목록 및 계획에 결속해야 한다. v3 원본은 보존한다.

추가로 manifest v12의 Edit fixture `Files[].ChangeAuthority`는 “현재 원장 v11 결속”이라고 적혀 있지만 현재 SHA는 v12 자기 결속이다. 같은 경로의 `AllowedChanges.Reason`은 승인된 “현재 v12 결속”을 정확히 기록하고 승인 계약도 그 변경을 허용하므로 구현 권한 자체에는 모호함이 없다. 따라서 별도 차단 P1로 세지 않지만, 다음 불변 원장을 만들 때 파일별 `ChangeAuthority` 문구를 현재 근거와 맞추는 편이 감사에 정확하다. 이미 발급된 v12 원장은 수정하지 않는다.

## 실행 범위

이 정적 검토는 원장·입력·행·선택 자료의 바이트 결속을 확인한 결과다. P1의 선택 파서 근거 불일치가 남아 있고 새 실행 계획도 이 검토 대상이 아니므로 Play 재실행 또는 테스트 통과를 판정하지 않는다.
