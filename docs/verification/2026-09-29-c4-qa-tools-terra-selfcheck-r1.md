# C4 검증 도구 테라 자체 점검 r1

- 작성: 2026-09-29, 테라 구현 역할, 실제 `gpt-6-sol`.
- 상태: 자체 지역 점검 기록. 독립 검수·아스트라 수용·실제 C4 실행 증거가 아니다.
- 추적: `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-001..010`, `AC-M5D7QC3-002/004/007/008/010`.
- 구현 근거: Approved C4 r4, QA r2 및 선행 377행 출력 개정. 원본 선행 선택 형식의 별도 개정은 현재 Draft이므로 아래 한정 분기는 추가 승인 대기이며 최종 수용을 주장하지 않는다.
- Unity·실행기·큐 실행, 신규 선택/행/소스/입력/계획 원장 발급은 하지 않았다. 지역 함수 검사는 메모리 안의 시험 객체를 사용했고 최종 원장으로 저장하지 않았다. 기존 377행의 검사는 과거 XML을 읽기만 했으며 새 실행 통과를 뜻하지 않는다.

## 선행 선택의 정확 격리

`C4-RunSelection`은 다음 여섯 정확 경로가 사적 고정 지문 표에 있을 때만 선행 형식을 고른다. 먼저 실제 file binding을 검증하고 지정 SHA와 비교한다. 이어 같은 계획의 선행 선택 지도 Parts에서 Path/SHA/PartId가 모두 같은 단일 부분을 찾고 OrderedNames를 원본 ExpectedQualifiedNames와 정확 비교한다. 경로만 같고 SHA가 다른 경우 즉시 실패하며 신규 형식으로 fallback하지 않는다. 현재 플랫폼 결속의 추가 보정은 아래 잔여에 기록한다.

| 정확 경로 | SHA-256 | 정확 최상위 키 형식 |
| --- | --- | --- |
| `artifacts/c3-r11-edit-matrix-selection.json` | `B04192B8627B3BC03F934B102ADA35D3CD095757C88DC4ED2AD9FD476F2ACBED` | 6키 형식 |
| `artifacts/c3-r11-edit-remaining-selection.json` | `F8CCD04044B67A76E3033CD3D6A5A169B98BA5B25107801B26C667520B125EAF` | 6키 형식 |
| `artifacts/c3-r11-playmode-focused-selection.json` | `C63B2946E83FF4F4862197E8C5EDFDC25B488C3AACC5C0628F612553A485CA01` | 6키 형식 |
| `artifacts/c3l-edit-regression-selection.json` | `FBD1B5A6FE6AD6E96CA5CDAD4103C9DFA129B3F5F622859E11D0B43FD4D649F8` | 4키 형식 |
| `artifacts/c3l-play-regression-selection.json` | `FFFF443666F58B8518A6E89C206AF8BF23A0EF559888AAC8D528E3111591FB9E` | 4키 형식 |
| `artifacts/c2-r36-selection-preflight.json` | `B2CF18E155D98BA7ECAC1E8CABE5EAF90CF6C48CC0419D1ECC800758220120E3` | 11키 형식 |

6키는 `Trace,Platform,Selector,ExpectedCount,ExpectedQualifiedNames,SourceFiles`다. 4키는 `Trace,Selector,ExpectedCount,ExpectedQualifiedNames`다. 11키는 `Selector,ExpectedCount,ExpectedQualifiedNames,R35RetainedCount,SelectedOtherCount,MatchesFixtureParent,BuildConfigScanRoots,CurrentObservedRepositoryBuildConfigs,ObservedSDK,RequiredWorkerInputs,FreshBeforeAfterRequired`다. 추가·누락 키를 거절한다. 원본 메타데이터와 문자열 Trace를 신규 Trace 배열인 척 변환하지 않는다.

이 여섯 경로 밖의 선택은 `C4-Selection`의 QA r2 신규 스키마만 허용한다. 신규 focused 세 파일은 SchemaVersion/Trace 배열/SelectionKind/ParserEvidence/WholeAccepted=false 및 exact fullname selector를 요구한다. 선행 6/4/11키 객체를 신규 focused로 받지 않으며 기존 selector·이름·순서·원본 소스를 변경하지 않는다. 여섯 객체의 빈 또는 누락 SchemaVersion을 일반적인 신규 스키마 완화로 사용하지 않는다.

## 실제 지역 검사 방법과 원시 결과

System.Text.Json으로 중복 키와 정수 token을 확인한 뒤 배열·객체·문자열·boolean·null을 보존한다. 숫자 token은 Int64로 읽는다. 문자열 숫자·부동소수·지수·Int64 범위 초과를 정수로 보충하지 않는다. 실제 함수 본문만 읽어 Windows 인코딩/역파싱을 호출했으며 프로세스 시작 함수는 호출하지 않았다.

선행 분기 검사는 정확 원본 파일 여섯 개와 실제 SHA를 읽고, 동일 원본 이름을 가진 메모리 전용 Parts 지도를 제공했다. 이 지도는 최종 실행 계획이나 원장이 아니다. 자체 검사에서 실행 Count를 문자열로 바꾸었을 때 처음에는 통과하는 규약 불일치를 발견했다. 그 실패 사실을 보존한다.

```json
{
  "Check": "원본선택분기_정합수정전",
  "OriginalSelectionNormalPassed": 6,
  "WrongBindingRejected": true,
  "UnboundPartRejected": true,
  "StringCountAcceptedMismatchCandidate": true
}
```

아스트라가 기존 Approved 규약의 정합 수정을 지시한 뒤, 모든 JSON 정수의 정확 Int64 검사 선행·실행 Count 검사·Tools 전체 행 수와 고유 수·경로 대소문자 무시 비교·최종 기록까지 잠금 유지로 보정했다. 다음은 보정 뒤 실제 결과다. 잠금 관련 true는 원문 구조 검사이며 실제 mutex 실행 통과가 아니다.

```json
{
  "Check": "정합수정후",
  "OriginalSelectionNormalPassed": 6,
  "Expected": 6,
  "StringRunCountRejected": true,
  "OrdinalIgnoreCaseStaticPresent": true,
  "ToolsCountAndUniqueStaticPresent": true,
  "FinalWriteBeforeMutexReleaseStatic": true,
  "ActualMutexOrProcessExecuted": false
}
```

```json
{
  "IntegerStringRejected": 8,
  "Expected": 8,
  "ActualType": "System.Int64"
}
```

위 정수 음성 대상은 `SchemaVersion,Count,ExpectedCount,ObservationSeconds,CommandLineLength,ProcessId,OuterExitCode,c1Begin`에 각각 문자열 `"1"`을 넣은 경우다.

```json
{
  "Check": "파서및인코딩",
  "JsonNegativePassed": 6,
  "JsonNegativeExpected": 6,
  "WindowsRoundTripPassed": 5,
  "WindowsRoundTripExpected": 5,
  "BooleanStringDistinct": true,
  "IntegerStringDistinct": true,
  "NullZeroDistinct": true
}
```

JSON 음성은 최상위 중복, 중첩 중복, `1.0`, `1e0`, `9223372036854775808`, JSON 뒤 비JSON 문자열이다. 인코딩 왕복은 일반 문자열, 공백 있는 Windows 경로, 내부 quote, 말단 backslash, 큰따옴표와 괄호가 있는 exact selector다. 지원되지 않는 TestCase escaping을 허용한 결과가 아니라 Windows 인자 인코딩 함수 검사다.

정규 사실 36개를 가진 메모리 전용 PureValidator 행의 정상 대조가 통과했고 checked/generation/facts 순서/추가 키/call 숫자 형식 손상 다섯 개를 거절했다. 이는 실제 게임 실행이나 fixture의 관측 정확성 증거가 아니다.

```json
{
  "Baseline": "Passed",
  "CanonicalFacts": 36,
  "NestedNegativePassed": 5,
  "NestedNegativeExpected": 5
}
```

기존 `c3-r11-edit-matrix-r1.xml`, 기존 source-before 및 기존 decision-row-comparison을 읽고, 원본 예상 377행/754개 raw 행/부모 NUnit/원시 내용/지문을 다시 대조했다. 기존 출력은 수정하지 않았다. 비교 결과 누락 키, 376으로 손상한 개수, Matched=false, 부모 CaseResult=Failed, 잘못된 XML SHA의 음성을 실제로 거절했다.

```json
{
  "Check": "보존된377행읽기전용대조",
  "BaselinePassed": true,
  "NegativePassed": 5,
  "NegativeExpected": 5
}
```

실제 배열 대조 결과도 보존한다. 각 원본 순서를 유지하고 서로소 집합 합집합을 검사하며 joined sequence를 전체240 순서로 오인하지 않는다.

```json
{
  "OriginalCount": 240,
  "Matrix": 91,
  "Remaining": 149,
  "JoinedOrderSame": false,
  "UnionSame": true,
  "Duplicated": 0
}
```

일곱 파일의 PowerShell Parser 검사에서 SyntaxErrors는 각각0이다. UTC 정상 문자열은 통과하고 `+09:00` 시각은 거절했다. source 정의를 실제 생성기에 넣어 선택/행 개수를 발급하는 검사는 아직 하지 않았다.

## 프로세스·잠금·생성 경계 자체 점검

- runner는 exact 큐 부모의 `-File` 경로를 확인하며 QA 한 번만 숨김 하위 프로세스로 시작하도록 작성돼 있다. 실제 실행 확인은 없다. QA stdout/stderr는 별도 버퍼이고 runner stdout은 partial JSON 한 객체다. 실제 QA/native/outer 종료값은 서로 대체하지 않는다.
- native 관찰은 Unity binary/project/results/log exact 경로, runTests1/assetWorker0, 후보1, PID+creation UTC, 종료 전 handle 확보를 요구한다. 불완전 inventory/빠른 종료/복수 후보/미관측은 실패다. raw commandline은 해시만 저장하고 원문을 출력하지 않는다. process attach/creation/PID 재사용/빠른 종료 경합은 실제 미실시다.
- mutex는 큐만 획득하고 abandoned를 실패로 기록한다. 최종 queue-result CreateNew까지 보유하도록 정합 수정했다. 실제 동시 큐·abandoned·프로세스 종료 경합은 미실시다.
- 공통 writer는 FileMode.CreateNew와 FileShare.None이다. final QaReturn 작성자는 큐뿐이며 actual runner exit 이후 한 번 작성한다. 변경하지 않은 기존377 비교기만 사전 출력 부재 검사 뒤 원래 Set-Content를 호출한다. 새 C4 결과를 같은 파일에 덮어쓰지 않는다.

이 새 기록 파일이 이미 존재하는 상태에서 공통 writer로 같은 경로를 다시 생성하려는 지역 음성을 실행했다. 생성은 거절됐고 당시 파일 바이트는 동일했다. 아래 지문은 이 단락을 덧붙이기 전 검사 당시의 바이트이며 최종 문서 자신의 지문이나 실행 입력 지문이 아니다.

```json
{
  "ExistingFileCreateNewRejected": true,
  "ExistingBytesUnchanged": true,
  "BeforeSha256": "4D9A6DC17B739F5E8C8DBA6AD9D7B87428BE3B28A39BE60AA44F1C1D22EF13AE",
  "AfterSha256": "4D9A6DC17B739F5E8C8DBA6AD9D7B87428BE3B28A39BE60AA44F1C1D22EF13AE"
}
```

## 현재 바이트와 잔여

| 도구 | 점검 시 SHA-256 |
| --- | --- |
| `c4-build-focused-selection.ps1` | `B16362720E9DE2E69FC44990A664EA221E6A928D8A795A5F70C959D785D1015C` |
| `c4-build-required-checkpoint-rows.ps1` | `D1B91A300016EA55FC8A5F69395DCFE0D63E36EA222BE3B4F62E5CC4ACEF8BBD` |
| `c4-capture-inputs.ps1` | `5552B87255EF601E8D9A7FB6076DEE892F2082E85F01E2F05AF93637E91C74DB` |
| `c4-final-selected-run.ps1` | `ED2BF1FA01A41559189A63296A7C749EFDD09BEF6408729038280422BDB5A1E4` |
| `c4-final-validation-queue.ps1` | `B1E9BA59BE887667A9F1764FACEA673015F9D08D43A37A278711A8101E46B9F4` |
| `c4-verify-required-rows.ps1` | `7C413284E2819D866BC237E05EBCD68C04EE8680CC2EF94AD131B10B1A013E51` |
| `c4-verify-run.ps1` | `FA610EABEAAA3FD6DB36B32F7BFA3B7B96E4F1F22791B1F4283231507B9EB414` |

1. 여섯 선행 객체의 서로 다른 key/Trace/Platform 형식은 별도 정확 개정의 승인 대기다. 특히 Platform이 없는 Worker51/Edit562/Play610는 현재 map·실행 계획으로 연결하지만 사적 고정 플랫폼 표의 추가 엄격 결속은 해당 개정의 최종 승인 후 대조해야 한다. Worker51 원본은11키이며 원래 실행은 EditMode다.
2. 실제 최종 source·fixture/Hub 공유 행 정의·enum 파싱·expectedCase와 전체 nested facts의 source 대조는 남아 있다. source 미완성 상태에서 임의 개수·빈 원장을 만들지 않는다.
3. 실제 native/mutex/출력 경합·프로세스 관찰과 final queue는 미실시다. 현재 자체 결과가 이 경계를 독립 수용하거나 실제 성공을 증명하지 않는다.
4. runner는 부모 큐를 `pwsh -File <정확 큐 경로>`로 실행하는 형태를 요구한다. 다른 실행 호스트 방식은 성공으로 추정하지 않으며 실제 배분 전 실행 형태를 확인해야 한다.

이 기록에 사후 승인 또는 새 SHA를 소급 귀속하지 않는다. 이후 보정은 후속 바이트와 검사 결과로 분리한다.
