# C4 국소 검증 규칙 독립 검수

검수 범위는 승인된 C1 미호출 및 일반 저장 미관측 규칙의 두 행 도구와 그 구현자 국소 검사뿐이다. Unity, 컴파일, 큐 실행은 하지 않았다.

## 정확 지문

- `artifacts/c4-build-required-checkpoint-rows.ps1`: `DBD8BE999330657B81489342BB58586B0952583AF0867C591DB5E39949A9C071`
- `artifacts/c4-verify-required-rows.ps1`: `DEF2391963186E331EE1EA3C04BDF63E21D8DE3F4E271222CC15C2E865F6065B`
- Approved C1 미호출 개정: `8B312BF1374B9B8A0164D4937A9D84ECAF31C69B8058DB7D4CEB64B1D6339F1D`
- Approved 일반 저장 미관측 개정: `B886A65AD69DCED7230CA8E1E766E23483F93EFD047073FECF8C89F8D61A7EDE`
- 입력 규약: QA r2 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31`, 발급 세대 `FF88BFC0471FB48C2828765A1C6FBD0F3913F0DD123CDEB16BCBADD4F8F2DCB6`, C2 미호출 `7F1BAA3821580E7382258B7C8BD0F045818FF51B9334AD0C905EB7B9A6388356`, C4 r4 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`.
- C1 미호출 국소 JSON: `6B6731DEBF1AFA93392A1F52DED41299CB4469970199C672DB315D0C41973F9D`
- writer Unknown 국소 JSON: `45BD757796C7358DAE85DDA073A4B787D96EFD009D30E84FA652E3B8CC0CBEF0`

## 판정

P0=0, P1=0. 두 도구의 현재 규칙은 각각 승인된 제한을 지키며, 검토 범위에서 정정이 필요한 모순을 찾지 못했다.

C1 미호출은 `IssuedWithoutResult`와 함께 C4 경계 1–7 및 정확 checkpoint 값을 강제한다. 경계 1만 소비 전, 2–7은 소비 후이며 원본 발급·스레드·실패 이력 검사를 유지한다. 결과/단계/검사완료는 null·false, memory/proof는 승인된 Unknown, C1/C2·observer·fresh 호출 수는 모두 0이어야 한다. source 규칙과 canonical 값·certainty 및 여섯 진단의 존재도 대조한다. 다른 경계, 미도달, C1/C2 발급, checked=true 또는 잘못된 소비 상태는 통과하지 않는다.

저장 미관측은 FullBridge·실제 callback/worker 행의 `ordinaryWriterBlocked` 한 leaf에만 Unknown을 허용한다. barrier 상태 자체가 Unknown이면 거절하고, AC006 실제 저장 시험에는 예외를 적용하지 않는다. `NoSameCaseSaveObservation` 및 `FrozenSourceBoundary` 진단, 부모 시험과 fixture의 정확 source 포인터·동결 지문을 요구한다. fixture는 실제 marker 읽기를 지목하며 부모 시험 본문에서 저장 호출이 없는지 검사한다. 현 동결 소스에서 public Save 호출은 별도 AC006 시험에만 있으며, 검수 대상 사례에서는 호출되지 않는다. 따라서 Unknown은 저장 성공이나 차단 부재를 뜻하지 않는다.

## 국소 검사와 한계

구현자 국소 검사 기록은 C1 76/76, writer Unknown 23/23으로 기재되어 있다. 각 JSON은 `SyntheticLocalOnly=true`, 실제 CWT 관측·Unity 실행·전체 수용이 모두 false라고 명시한다. 이는 행 규칙의 합성 정상·거절 조건 확인으로만 인정한다. 실제 NUnit/XML, 원본 CWT/receipt, Unity 실행, 전체 canonical 행 수용의 증거가 아니다. source 포인터와 파일 지문은 고정 입력을 결속하지만, 이번 판정은 실제 실행 결과를 수용하지 않는다.
