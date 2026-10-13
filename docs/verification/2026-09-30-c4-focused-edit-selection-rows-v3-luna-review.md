# C4 Edit 선택·행 v3 독립 정적 검토

- 선택 자료 SHA-256: `FCA708318949FB6E1D02B50E142F04CE4F1BD5059C3E0D31E1A048748ABD6B47`
- 행 자료 SHA-256: `EC1C28562735B6B5E71B9C8E6E7D98AE0C67414A4A812F0AC85E1265D446AB70`
- 기준 계약: Approved C4 집중 Edit 관측 복구 계약 SHA-256 `7CDBEE47862C02C81D859F8902F5A8A3458157350E10D9CE7C825A8553B8CBD7`.
- 범위: 자료·선언·공유 행의 정적 대조만 수행했다. Unity/컴파일/큐는 실행하지 않았다.

## 판정

P0 0건, P1 0건이다. 두 v3 자료는 각 v2와 비교해 `SourceFiles` 지문만 바뀌었다. 선택 의미, 행 값 및 열거형 원장은 그대로다.

## 확인 사항

- Edit 선택 v3은 139개의 이름을 유지하며, 현재 Edit 시험 파일의 실제 메서드 및 매개변수화된 시험 사례와 모두 일치했다. 그 `SourceFiles`는 현재 `ProfileResetExecutionBridgeV1Tests.cs` SHA-256 `5F6E3C8D3ADC0EA3C5A2C616356043B8BA450F5BEBBFFCC39962A6A40B3E7F71`과 일치한다.
- 행 v3은 188행 전체를 유지한다. v2와의 필드 대조에서 행 자료 외 속성 차이는 없었다. Edit fixture의 148행과 Play fixture의 35행은 각각 공유 승인 행 원장의 해당 구간과 순서·값까지 일치했고, 나머지 5행은 기존 StrictAudit 구간으로 보존됐다. `WholeAccepted`는 false다.
- 행 자료의 세 `SourceFiles` 및 세 `EnumSources` 경로는 현재 파일 바이트 SHA와 모두 일치한다. 세 파일은 Edit fixture `8FE0785955DD786D394FF5E8057AB352D8ED94CA4BFC0EAC2AD64628E77B2214`, Play fixture `30330B3276F61F9686D9743CD7BFCA20B5CE6A5FFDC2E0994F8847949FAB1A0E`, StrictAudit `1089AFEC1CD19AD231F1EBC7EB0A7E272A17C7B90705F195D916B45AB8860772`다. 열거형 원본인 Bridge, disk transaction, memory cutover도 자료 지문과 일치한다.
- Play 선택 v2와 Hub 선택 v1은 각각 SHA-256 `D7A625CC34DFA8E00908A7BBC2157D153C762B64ADA73D848097938D9500E1B4`, `747630026CDD35A0B54BABB19130F2838F2D8FDD5C5AD881E366502502DEB6D8`로 유지된다.

본 검토는 자료 간 정적 결속만 확인했다. 아직 발급되지 않은 manifest/input/plan 결속이나 실제 Unity 실행 결과에 대한 수용 판정은 포함하지 않는다.
