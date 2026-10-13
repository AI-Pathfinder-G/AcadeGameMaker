# C4 집중 Edit v8 동결·계획 독립 검토

- 기준 계약: Approved C4 집중 Edit 관측 복구 계약 SHA-256 `7CDBEE47862C02C81D859F8902F5A8A3458157350E10D9CE7C825A8553B8CBD7`.
- 범위: 동결 원장, 입력 목록, 계획 및 참조 결속의 읽기 전용 정적 대조. Unity/큐 실행은 하지 않았다.

## 판정

P0 0건, P1 0건이다. 요청한 세 자료는 지정 지문과 일치하며 승인된 변경만 포함한다. 재실행 전 자료 검토를 통과했지만 실행 승인이나 실행 결과 수용을 뜻하지 않는다.

## 확인 사항

- `artifacts/c4-frozen-source-manifest-v8.json` SHA-256 `2DA66963AAA2E4CB4DF927E34E31521552C3CFB1507AC8287B30FF75EE893645`는 107개 파일, 31개 허용 변경을 담고 `WholeAccepted=false`다. v7의 105개 파일 중 네 기존 파일 지문만 바뀌고 Edit 선택 v3 및 행 v3 두 자료가 추가됐다. 107개 현재 경로가 모두 존재하고 실제 SHA가 원장과 일치한다.
- `artifacts/c4-frozen-input-paths-v8.json` SHA-256 `0FB98CDF4B13E92A6C7164B70071D821F199755737239EB3C7B63A19950B7047`는 990개 고유 경로다. v7의 985개를 모두 보존하고 승인 계약, 정적 검토 기록, Edit 선택 v3, 행 v3, manifest v8 다섯 경로를 더했다. 이유표도 990개이며 기존 이유는 바뀌지 않았다. 목록은 `Sort-Object -CaseSensitive` 순서와 일치한다. 큐 계획·실행 출력 및 자기참조는 포함되지 않았다.
- `artifacts/c4-final-validation-queue-plan-v6.json` SHA-256 `94B7A64FC4D41EC490A872A248F3FE8CEFF8CFEE7D7A522BEDC37A8A2E4B14E6`은 9개 실행을 유지한다. v5 대비 최상위 결속은 원장·입력·행·도구뿐이며, 첫 실행의 stem, 선택 결속 및 출력 경로만 바뀌었다. 첫 실행은 `c4-r4-focused-edit`, 139개 이름, Windows 명령행 19,566자다. 나머지 8개 실행의 이름·순서·선택은 보존됐다. 모든 선택 파일의 SHA, selector, 기대 이름 및 개수가 계획과 일치하고 모든 계획 명령행 길이는 32,767자 미만이다.
- 계획은 총 72개 출력 경로를 선언하며 현재 모두 없다. `c4-final-validation-queue-result-v5.json`도 없어 기존 실패 증거와 충돌하지 않는다. 새 계획은 행 v3(188행), 선택 v3 및 유지된 Play 선택 v2/Hub 선택 v1에 정확히 결속된다.

요청된 지문은 manifest `2DA66963AAA2E4CB4DF927E34E31521552C3CFB1507AC8287B30FF75EE893645`, input `0FB98CDF4B13E92A6C7164B70071D821F199755737239EB3C7B63A19950B7047`, plan `94B7A64FC4D41EC490A872A248F3FE8CEFF8CFEE7D7A522BEDC37A8A2E4B14E6`이다. 정적 자료 결속만 검증했으며 실제 Unity 실행, XML, native 종료 및 QA 결과는 아직 없다.
