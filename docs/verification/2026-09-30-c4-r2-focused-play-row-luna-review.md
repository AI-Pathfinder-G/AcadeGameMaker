# C4 focused Play 행 검수

- 판정 범위: plan v12의 focused Play 단일 실행과 필수 행 비교. Unity를 다시 실행하지 않았다. 이 결과로 C4 전체 수용을 주장하지 않는다.
- plan: `artifacts/c4-final-validation-queue-plan-v12.json`, SHA `4DAD0A629965E588FCD444EC27039B20A78A0A0D3592F1CCAB4235F97D27B940`.
- 실행 증거: XML `artifacts/c4-r2-focused-play.xml`, SHA `1C715DFEE791E960C4FFB18E0DB9F46556BD167A31703DD182FF409461DBB699`; 행 비교 `artifacts/c4-r2-focused-play-required-row-comparison.json`, SHA `FC5E5356BF1680775478EA55F96C42A7DE87EC96F60AE3CB46F23C91C7061831`; native 반환 `artifacts/c4-r2-focused-play-native-exit-observation.json`, SHA `F32AD8FC7FBE85F9FBF0BE5B0B9FD6ABA6DAFD33D89D6FADCE4FC96D75DBF2A4`; QA 반환 `artifacts/c4-r2-focused-play-qa-tool-return.json`, SHA `71CE5E0B082DB63ACFA71A78E9E3D8A64522A52FADAA18F7885C35BECD9B8582`; 큐 결과 `artifacts/c4-final-validation-queue-result-v10.json`, SHA `741A087D1FD38A2AAB03E9C9A3F608116C72A6623014EC974A8F92E8CC87BB56`.
- 바인딩: Play 선택 v4 SHA `032376266E3217B859BDC9F01A98385AD275E25DF0443865196BE7782F97DF13`; 행 v8 SHA `F7460A26F5ECA8722148F0B5273BE7BB8FCBAB39F2C431AA83D9C798A40830B4`; 입력 v16 SHA `708EFBB467698A82636A4D138C4C349175B11F2345A25301494EA84BF1A4400F`. before/after 캡처는 각각 1052개 경로를 담고 경로별 파일 SHA 차이는 0이다.

## 실제 실행 결과

XML에는 선택된 32개 정식 이름이 각각 한 번씩 있고 모두 통과했다. 실패·건너뜀·중복 이름은 없다. native 종료, QA 도구 반환, 외부 반환은 각각 0이다. 그러나 원장 비교는 35행 중 35행 도달·실행을 확인했으면서도 세 행의 필수 증거가 예상값과 일치하지 않아 `EvidenceMatched=false`로 종료됐다. 큐는 앞선 Edit 한 묶음만 보존하고 Play에서 중단했으며 `WholeAccepted=false`다. 따라서 NUnit 통과와 필수 증거 수용을 구분한다.

## 차단 행

두 AC007 행은 각각 실제 `ReloadRequired/C2Invoked`와 `ManualRepairRequired/C2Invoked`를 반환했고 receipt 없음, 검사 완료, 장벽 증거도 행 요구와 맞는다. 실제 guard 관측은 `permanentFaultRecorded=Yes`인데 v8 행은 `No`를 요구한다. 동결된 C4 경로에서 terminal 결과는 `CloseOriginalExecution`으로 pair를 닫고, 각 half의 `Close`가 reset/cohort가 이미 완료되지 않은 경우 lifecycle fault를 기록할 수 있다. 이는 AC007의 “DiskPrepared 이후 비완료 terminal” 요구와 양립할 수 있으며, 현재 XML만으로 제품 결함이라고 단정할 수 없다. 다만 `Yes`와 `No`는 엄격 비교에서 불일치하므로 이 두 행은 수용되지 않았다. 규범적 의도와 행의 기대값을 해결하고 다시 실행해야 한다.

AC008의 실제 `AfterRouterGuard` 실패는 6인자 callback에서 같은 예외와 도달을 확인했고, C1 미호출·결과 미발급·fault 및 native 폐쇄를 기록했다. 실제 receipt는 없으며 fixture는 `_currentSession`과 `_resetReceipt`의 정확 형식 필드를 읽어 null을 확인한 뒤 `Absent/SourceEstablished`를 기록한다. v8 행은 같은 부재값을 `Observed`로 요구한다. 값은 같지만 certainty가 달라 행 비교가 거부했다. pre-C1 승인 규칙의 source 경계와 실제 필드 관측을 반영해 행/관측 certainty의 한쪽을 승인된 방식으로 정렬해야 하며, `Absent`를 `Unknown`이나 미검증 성공으로 바꾸면 안 된다.

## 결론

P0 0건. P1은 두 종류의 행 증거 불일치다: AC007 두 행의 fault 기대값, AC008 한 행의 receipt certainty. 이는 현재 증거 비교를 통과시키지 못하는 차단사항이며, 그 자체가 제품 런타임 결함의 증명은 아니다. 수정 승인 후에는 기존 이름·행 의미를 유지한 정확 보정과 새 실행 증거가 필요하다. Unity 실행은 XML 32개 행에 한정되고, 전체 9회 큐·C3 전체 수용·C4 수용은 확인되지 않았다.
