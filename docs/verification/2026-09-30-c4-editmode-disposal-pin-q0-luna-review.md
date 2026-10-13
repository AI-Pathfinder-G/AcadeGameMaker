# C4 현재 소스 pin 및 감사 시험 정적 검토

- pin v2 SHA-256: `5DD70E7A7BD8E79A997F10362EF2DBAD148A913BD3221E20CF5F0DF8D9F33AF5`.
- Q0 감사 시험 SHA-256: `20F5E1C963B6F969649DDE58FA0B48338B2446049AB56949DE3BA60EB3812567`.
- Strict 감사 시험 SHA-256: `6EEFEA85B5E1A63BC47B6CF2B8341BAC71B2455BBAD9BEA462DB22679DFCC713`.
- 이전 pin SHA-256: `818AA68759C0BCBA3FC393C9E5DDF5AAEE23C23E076EBE8697CC19F3BE04F6B0`.
- 판정: P0 0건, P1 0건. 정적 검토만 수행했으며 Unity 실행은 하지 않았다.

새 pin v2는 두 runtime 경로만 포함한다. Adapter 행은 이전 pin과 바이트 단위로 동일하다. Router 행 SHA는 현재 `InputRouter.cs`와 일치하고, 승인 binding은 승인된 복구 계약의 정확 경로/SHA, 검수 binding은 독립 구현 검수 문서의 정확 경로/SHA를 가리킨다. Router predecessor는 보존된 선행 입력의 정확 경로/SHA다. 이전 pin 파일은 보존되어 있다.

Q0 시험은 v2 pin 경로와 SHA를 고정하고, Router 행에서만 새 승인/검수 binding을 요구한다. Adapter 행은 기존 r4 승인과 기존 검수 binding을 유지한다. 이전 successor 및 predecessor 역사 검사는 별도 값으로 남아 있으며 새 Router SHA로 덮어쓰지 않았다. Strict 시험은 동일 v2 경로를 읽고 두 행의 경로·중복·현재 파일 SHA·predecessor와 binding을 검증하며, Q0 helper가 정확 pin 경계 및 원래 감사 본문 보존 조건을 수행하는지 확인한다.

따라서 이 세 자료의 pin 결속과 감사 범위에는 정적 P0/P1 차이가 없다. Q0/Strict 시험의 실제 통과나 다음 동결·실행 자료의 정확성은 이번 검토 범위가 아니다.
