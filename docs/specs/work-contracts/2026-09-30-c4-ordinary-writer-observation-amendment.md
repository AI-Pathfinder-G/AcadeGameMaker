# C4 일반 저장 차단의 미관측 증거 개정

- 상태: **Approved — 일반 저장 미관측의 한정 증거 규칙 구현 승인, 실제 실행·통합 수용 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 승인 근거: [독립 검수](../../verification/2026-09-30-c4-ordinary-writer-observation-luna-review.md) SHA `84BEB74F72B825846D280278C161A97FAF9135B60B1E9753B486FE1228C82AF5`, 정적 규범 P0/P1=0/0. 검수 초안 SHA `9D24687E2BA0A1C301AE42BCCB920F8DB16152E9454C39D0AEE92AA994A0E8EB`을 `2026-09-30-c4-ordinary-writer-observation-reviewed-draft.md`에 그대로 보존했다. 본문 승인 전 문장은 당시 이력이다. 별도 실제 실행 배분은 남아 있다.
- 작성·기술 선택: 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-002/005/006/007`, `AC-M5D7QC4-002/005/006/007/008/009/010`.
- 근거: Approved [QA 증거 규약 r2](2026-09-29-c4-qa-evidence-protocol.md) 및 [C4 r4](2026-09-29-c4-r4-exact-implementation-amendment.md), [운영 도구 독립 검수](../../verification/2026-09-30-c4-operational-tools-luna-review.md) SHA `717F7F7943EB8EBC21A262BB41E952CF9BA474CBFB5335D1A96E3BB447AF29EF`의 추가 증거 판단.

## 해소 대상

`barrier.state=NoBarrier` 또는 marker·root lease의 현재 부재는 같은 사례에서 공용 `ProfileAtomicSaveServiceV1.Save` 호출이 차단되지 않았다는 관측이 아니다. 일반 저장은 인자·canonical document·다른 파일 상태에 의해 실패할 수 있다. 특히 C2 뒤 marker가 사라진 네 장애 경계와 Busy/Stale 뒤 실제 fresh cursor를 계속 검증하는 세 사례에서 별도 Save를 호출하면 원래 사후 파일/revision을 바꿔 후속 결과를 오염시킨다. 현재 helper가 두 reset 게이트 부재만으로 `ordinaryWriterBlocked=No`를 발행하면 사실 범위를 넘는다.

## 한정된 미관측 값

정상 FullBridge/ActualCallback/ActualWorker의 기존 `Unknown` 금지에서 **오직** `barrier.ordinaryWriterBlocked` 한 leaf에 한정해 다음 조건을 모두 만족하는 경우 정확 `Unknown`·canonical certainty=Unknown을 허용한다. 다른 정상 authority/guard/barrier.state/history/cleanup이나 호출 수의 Unknown 면제는 없다. 이 값은 차단되지 않았음·저장 성공·쓰기 허가를 뜻하지 않는다.

1. 실제 해당 사례는 같은 root에 대한 별도 public `Save` 호출을 수행하지 않는다. 정확 부모 시험 본문과 호출된 fixture/도우미 source가 그 사실을 보여야 한다. 새 observer·writer 호출·제품 경로를 추가해 값을 채우지 않는다. 실제 Save를 한 행에는 이 면제를 금지하고 그 실제 결과/예외·무변경 bytes를 기존 `ActualOrdinaryWriterObservation` 및 정확 행 pointer로 검증한다.
2. 실제 root·marker·원본 lease의 상태를 별도 관측하여 `barrier.state`와 정확하게 기록한다. 임시 writer가 다른 원인을 만나지 않는다는 추정으로 `ordinaryWriterBlocked=No`를 기록하지 않는다. positive `Yes`는 같은 root의 실제 writer 차단 관측, 또는 유효 인자·source 지배와 실제 활성 reset barrier/lease 원본을 함께 입증한 경우에만 기존 규칙에 따라 사용한다. root 자체의 장애·표식·lease를 Unknown으로 버리지 않는다.
3. 예상 원장의 정확 해당 행은 `ExpectedBarrier.ordinaryWriterBlocked="Unknown"`, 대응 RequiredFact `Name="barrier.ordinaryWriterBlocked", ExpectedValue="Unknown", RequiredCertainty="Unknown"`을 함께 사전 선언한다. terminal after와 nested actual leaf 둘 다 exact Unknown이어야 한다. `diagnostic.ordinaryWriterObservation=NoSameCaseSaveObservation`·SourceEstablished를 RequiredFacts에 정확 추가하고, 해당 시험 본문 및 호출 helper의 동결 파일·양수 행 번호를 actual Fact.evidenceReference로 결속한다. 단순 source 경로 문자열이나 실행 후 사유 변경은 실패다.
4. 해당 행의 `barrier.evidenceReference`는 여전히 기존 Present/Absent 규칙을 따른다. FullBridge에서 실제 장벽 상태나 root/marker/lease 검사 근거가 필요하면 Present이며, `diagnostic.barrierEvidenceKind=FrozenSourceBoundary`와 실제 읽기·source 지배 및 동결 파일의 정확 pointer를 결속한다. 이 Unknown을 barrier 전체의 부재나 정상 proof로 사용하지 않는다.
5. 같은 행이 `AC-M5D7QC4-006`의 실제 ordinary writer 차단·bytes 불변을 직접 요구하거나 실제 Save를 호출하면 이 면제를 적용하지 않는다. 해당 별도 실제 writer 시험은 기존 Approved의 Observed 결과를 보존한다. C4 실행 자체의 C1/C2 정상 검증과 하위 reset-only writer는 별도이며 public ordinary Save의 실제 결과라고 보고하지 않는다.

두 값의 의미를 분리한다. `barrier.state`는 실제 reset marker/장벽 상태이고 `barrier.ordinaryWriterBlocked`는 **같은 사례의 일반 저장 시도 차단 여부**다. `No`는 안전한 별도 실제 Save 시도가 차단되지 않았음을 관측한 경우에만 발급한다. `No`만으로 영속 파일의 모든 내용이 유효하거나 차후 Save가 항상 성공한다는 일반 명제를 만들지 않는다. `Yes`도 관측/입증한 reset 차단 근거를 넘지 않는다.

## 구현·독립 검수 경계

독립 검수와 아스트라 승인 후 변경 허용은 기존 배분된 두 fixture/shared emitter·ExpectedRows 및 `c4-verify-required-rows.ps1`의 위 단일 leaf의 정확 조건 처리다. 실제 parent 시험이 진단을 연결하는 범위는 이미 배분된 신규 Edit/Play 파일에 한정한다. 런타임8, C1/Profile writer, 새 제품 API, 다른 증거 필드·스키마 키, 기존 회귀 시험, 새 일반 저장 호출은 바꾸지 않는다. SourceManifest/InputPathList에는 실제 읽은 writer·barrier·시험·도우미 source를 결속한다. 실제 실행·최종 수용은 별도 freeze/검수/배분 뒤다.

국소 음성 검사는 정상 행에서 다른 authority/guard/barrier.state/history의 Unknown, writer 호출이 있는 행, 실제 writer 관측 필수 행, SourceEstablished 사유/참조 누락·오경로, nested/canonical 값·certainty 불일치, 빈 root/marker/lease 근거를 거절한다. 미관측 행의 실제 시험 본문·호출 그래프와 동결 source의 일치성은 독립 검수한다. 합성 도구 검사는 Unity 실행이나 실제 Save를 대체하지 않는다.
