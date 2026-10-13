# C4 일반 저장 미관측 규칙 테라 자체 검사

- 작성: 2026-09-30, 테라 구현 담당, 실제 `gpt-6-sol`. 자체 검사이며 독립 수용이 아니다.
- 근거: Approved `2026-09-30-c4-ordinary-writer-observation-amendment.md` SHA `B886A65AD69DCED7230CA8E1E766E23483F93EFD047073FECF8C89F8D61A7EDE`.
- 추적: REQ-M5D7QC4-002/005/006/007, AC-M5D7QC4-002/005/006/007/008/009/010.

정상 FullBridge/ActualCallback/ActualWorker에서 `barrier.ordinaryWriterBlocked`만 정확 Unknown과 canonical Unknown certainty로 허용한다. 사전 예상값·`NoSameCaseSaveObservation` SourceEstablished 진단을 요구하고, 실제 부모 공개 선언행 및 fixture의 marker 읽기행 pointer를 동결된 파일 SHA와 대조한다. 장벽 상태 Unknown, 장벽 근거 부재, 실제 Save 호출 AC006 행은 거절한다. 정상 결과의 다른 authority·guard·history·barrier 상태 Unknown은 기존 규칙대로 거절한다. Structure/PureValidator의 기존 미관측 범위는 유지한다.

최종 국소 검사 실제 종료값0, 판정 일치23/23(정상1, 음성22)이다. 음성은 다른 증거 종류·결과·장벽 상태, 실제 writer 값 No, nested/canonical 값·certainty, 진단 누락·값·certainty, 부모·장벽 pointer 누락/오행/다른 파일, 실제 Save 부모, source pin 누락, 구조행 기존 범위 및 예상 장벽근거 오류를 포함한다. 처음20개 및 보강23개 원시 자료를 보존했고, fixture가 C1 helper를 추가한 뒤 변경된 marker 읽기행으로 최종23개를 재검사했다.

합성 조건 검사는 부모·fixture의 실제 호출 그래프와 같은 사례의 Save 미호출 여부를 독립적으로 입증하지 않는다. 실제 fixture의 root/marker/lease 읽기·바이트 지배, 행 전체 및 Unity 실행은 독립 검수·최종 수용 대상이다. Unity·runner·queue·원장 발급은 하지 않았다. WholeAccepted=false다.

| 파일 | SHA256 |
| --- | --- |
| artifacts/c4-verify-required-rows.ps1 | DEF2391963186E331EE1EA3C04BDF63E21D8DE3F4E271222CC15C2E865F6065B |
| docs/verification/2026-09-30-c4-writer-unknown-terra-localcheck.ps1 | 10F7A3E14D533E404E971F9F8520B967EF5A03F443F9BD622B5F40109E906D8D |
| docs/verification/2026-09-30-c4-writer-unknown-terra-localcheck-r3.json | 45BD757796C7358DAE85DDA073A4B787D96EFD009D30E84FA652E3B8CC0CBEF0 |

도구와 국소 스크립트의 PowerShell 구문 오류는 각각0이다. 실제 동결·행 전체·부모 NUnit·독립 수용은 남아 있다.
