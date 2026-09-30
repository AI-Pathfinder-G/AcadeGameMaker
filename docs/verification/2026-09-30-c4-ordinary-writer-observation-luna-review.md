# C4 일반 저장 차단의 미관측 증거 개정 검토

검토 대상 Draft SHA-256: `9D24687E2BA0A1C301AE42BCCB920F8DB16152E9454C39D0AEE92AA994A0E8EB`.

대조 규범은 Approved QA r2 `EAC95DAAAFE32388C2B2C72E5CC917C222081B7FF3B1FA68E334438C5A599B31` 및 C4 r4 `7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D`다. 운영 도구 검토 `717F7F7943EB8EBC21A262BB41E952CF9BA474CBFB5335D1A96E3BB447AF29EF`의 결론과도 일치한다.

정적 판정: P0 0, P1 0. 예외를 `barrier.ordinaryWriterBlocked` 한 leaf에만 한정하고, 같은 사례에서 public `Save`를 호출하지 않은 정확한 행에 `Unknown`을 허용한다. 실제 Save를 요구하는 행이나 실제 Save가 실행된 행에는 예외를 적용하지 않는다. `barrier.state`, root/marker/lease 관측, 가드, 이력, 정리, 호출 수의 Unknown 면제는 없다. AC-M5D7QC4-006의 durable barrier 및 기존 실제 writer 증거 요건을 면제하지 않는 구분도 명시되어 있다.

`No`는 reset barrier가 같은 사례의 일반 저장 시도를 막지 않았음을 실제 안전한 Save 관측으로 확인했을 때만 쓸 수 있고, 저장 전체 성공을 뜻하지 않는다. marker/lease 부재만으로 `No`를 발급하지 않는다는 기준은 타당하다. 실제 사례에서 Save를 호출하지 않았다면 canonical leaf와 nested 기대값을 함께 `Unknown`으로 고정하고, 미호출 근거와 실제 barrier 상태를 별도로 결속해야 한다. 이는 QA r2의 FullBridge 정상 Unknown 금지에 대한 한 leaf·한 문맥의 명시적 개정으로 충분히 좁다.

남은 확인은 승인 이후 각 대상 행의 실제 test/helper 본문이 public Save를 호출하지 않는지, RequiredFacts와 terminal after/nested 값이 모두 동일하게 연결되는지다. 이 정적 설계 검토는 그 구현 사실이나 Unity 실행을 대신하지 않는다. 문서는 Draft이며 구현·실행 승인이 아니다.
