# C3 하위 시험 컴파일 접근 보정 독립 검수

- 검수자·실제 모델: 루나, `gpt-6-luna`
- 런타임 SHA-256: `ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1` (일치)
- 시험 SHA-256: `F80D57376F3845E16BB3D347CFB3834ACE8FCCB53F539376D880A844A90F9581` (일치)
- 범위: 두 줄 컴파일 접근 보정의 정적 검수. 런타임·시험 수정 및 Unity 실행 없음.

## 판정

P0=0, P1=0이다. 변경 시험에서 접근 불가한 내부형 `ProfileResetConfirmationCaptureV1` 직접 참조가 0건이고, 복제된 capture의 검증은 기존 `Invoke` 경로로 수행한다. 해당 경로는 `TargetInvocationException`을 감싸는 호출 결과에서 `InnerException`이 실제 `InvalidOperationException`인지 검사하므로 clone 거절의 의미를 유지한다. 정상 발급은 원래 capture를 사용하며, 새 발급·등록·조립 friend 또는 런타임 변경은 보이지 않는다.

`[Test]` 18개와 `[TestCase]` 55개, 총 73개 예정 행은 보존됐다. 실제 runtime 지문은 지정값과 일치한다.

## 증거 한계

테라의 standalone C# 컴파일 종료 0 보고는 해당 컴파일 구성의 증거로만 취급한다. Unity 컴파일·선택자 열거·실행 및 전체 회귀 결과는 이 검수에서 확인하지 않았으며, 새 지문의 수용 증거가 아니다.
