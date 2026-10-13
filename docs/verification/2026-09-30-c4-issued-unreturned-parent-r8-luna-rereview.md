# C4 미반환 부모 fullname r8 독립 재검수

- 검증기: `artifacts/c4-verify-required-rows.ps1` SHA256 `20418B63A488C6ED8D3EF2088BE8FAE719E7E142315B92EE029CFA3AB9C3CD69`.
- 국소 결과: `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r8.json` SHA256 `A984FC6858A567E3D8D7C0315511819B9985E8DF94310728CAA1B8627B2CD1AC`.
- 검토 범위: projection, lifecycle, proof checkpoint, 추가 proof 부모의 exact fullname 제한만 재검토했다.

## 판정

P0=0, P1=0. r6/r7에서 남았던 모든 미반환 부모 접미부 허용은 r8에서 닫혔다. projection 두 부모와 Play lifecycle 부모, proof 손상 두 부모, C4 checkpoint 12·13·17은 각각 정확한 전체 NUnit fullname으로 제한되며 비교는 대소문자를 구분한다. 부모·사건 분류는 변경된 계측이나 별도 권한 경로를 만들지 않는다.

국소 JSON은 승인된 부모들 외에 잘못된 namespace, class 또는 대소문자를 가진 사례를 포함해 190/190 일치한다. `SyntheticLocalOnly=true`, `ActualCwtObserved=false`, `ActualUnityExecuted=false`, `WholeAccepted=false`가 유지된다.

이 결과는 verifier 분기의 정적/합성 검수만 의미한다. 작성 중인 Edit 부모·fixture의 최종 source freeze, checkpoint 14–27 행별 결과, Unity 실행 및 전체 C4 수용은 확인하지 않았다.
