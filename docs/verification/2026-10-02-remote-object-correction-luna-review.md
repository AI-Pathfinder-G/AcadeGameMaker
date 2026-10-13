# 원격 객체 지문 보완 독립 검수

대상 승인 계약: `docs/specs/work-contracts/2026-10-02-remote-object-correction-independent-review.md`. 조사 v1 SHA-256 `AE4DE92037B0A345ADB9AA42710112C07EB54A9BD512C1A6309444EFB02A4F4A`, v1 Luna 보고 SHA-256 `ADAA4547349D195002144C0041968A47EB7FEC320E6D0B2CCCC8FC7D68CB7419`, 보완 JSON SHA-256 `9B4FF061C3F4D8FB2FA80C6F24B63EB6E65D43F1B7A4D0650C658D6B86E23A11`, 테라 보고 SHA-256 `E5FFE85E92639434E47E7BFDA3F5768EF44BEE7D396D528082ECCE5AF900A7CA`를 재확인했다.

## 판정

- **P0: 0건. P1: 0건(새 v2 원격 결속 범위).** 과거 v1의 `TreeSha=HEAD` 오류는 역사 기록으로 그대로 남지만 새 관측의 정확 트리 식별·재현으로 보완됐다. 보완 증거는 v1을 수정하거나 오류를 소급해 없애지 않는다.
- **AC-PLAT-016 — 새 관측에서 충족.** GitHub 연결 GET을 `2026-10-02 03:02:28–03:02:32 UTC`에 직접 수행했다. `main` 시작/종료 HEAD는 모두 `e95bb5169ad2ed08106bd342915d4637a1241263`. 고정 커밋 응답의 `tree.sha`와 고정 트리 재귀 응답 `sha`는 모두 `93c4e1fabbaddabd4c237e5964db8dd735965f95`; 전체 항목 1042, 잘림 `false`(blob 1009)다. 계약 경로 501개를 응답 트리와 직접 대조해 존재 7·부재 494를 재산출했고 보완 원장의 7개 존재 집합과 일치했다. 이 결론은 해당 관측 시각에 한정된다.
- **AC-PLAT-017 — 제한 증거 충족.** 보완 JSON의 7개 `ResponseBase64`를 로컬에서 복원해 본문 SHA-256, 바이트 수, `blob <길이>\0` 접두를 포함한 Git blob SHA-1을 다시 계산했다. 본문·크기·OID 불일치 0이며 각각 실제 트리 항목과 일치했다. 501 원문 경로의 로컬 시작/종료 지문 불일치도 0이다. 충돌을 원격 본문과 현재 원문/정규화 후보 지문 차이로 재계산한 결과 원문 2개(`artifacts/c3-required-decision-expected-rows.json`, `qa/tools/Test-QaCatalog.ps1`), 정규화 1개(`qa/tools/Test-QaCatalog.ps1`)로 JSON 기록과 일치한다.
- 보완 JSON의 `RemoteConditionAccepted`, `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`, `CleanCloneExecuted`는 모두 거짓이다. 이 독립 검수는 해당 플래그를 갱신하지 않는다.

새 객체 결속은 원격 트리·501 경로 분류·7개 기록 blob의 재현 오류를 해결한다. 전체 501 내용 적합, 의미상 간접 의존 폐쇄, 렌더러, Q0, 깨끗한 복제, 게임 회귀 또는 게시 권한은 다루지 않았다.
