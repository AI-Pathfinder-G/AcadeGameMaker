# C4 미반환 부모 fullname r7 한정 재검수

- 검증기: `artifacts/c4-verify-required-rows.ps1` SHA256 `0F4C91B2622F097CEED4DBE23D020C0D26D4F7A68535D3EBD88C3A76CC4E2326`.
- 국소 결과: `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r7.json` SHA256 `9A1DE731AE749C017E200058CDA7596C7624F281014D487C979ECE96C72245B3`.
- 범위: r6에서 남긴 부모 fullname 비교 문제의 국소 verifier 수정만 검토했다.

## 판정

P0=0, P1=0. r6의 잔여 부모 범위 P1은 닫혔다. proof 손상 분기는 두 승인 full name을 `-cnotin`으로 case-sensitive 비교하고, checkpoint 17은 승인된 단일 full name을 `-cne`로 case-sensitive 비교한다. 따라서 namespace, test class, 대소문자 차이를 허용하지 않는다.

새 국소 사례는 proof 부모에 대해 다른 namespace 2건·다른 class 2건·대소문자 변경 2건을, checkpoint 17에 대해 namespace·class·대소문자 변경 각 1건을 거절한다. 기존 다른 부모 및 손상 규칙 음성도 보존되며 184/184 검사가 일치했다.

이 기록은 합성 verifier 검사만 확인한다. 결과 JSON은 `SyntheticLocalOnly=true`, `ActualCwtObserved=false`, `ActualUnityExecuted=false`, `WholeAccepted=false`를 유지한다. Edit fixture의 실제 행 실행, Unity, 컴파일 또는 전체 수용은 이 재검수 범위가 아니다.
