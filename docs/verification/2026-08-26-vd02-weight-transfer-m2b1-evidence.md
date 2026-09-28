# VD-02 Unity M2B1 target registry·production LOS 증적

- 검증일: 2026-08-26
- 구현: Terra, Kimi K3 제한 테스트 초안은 Terra가 선별
- 통합 판단: Sol
- 독립 검증: Luna
- 판정: **PASS — M2B1 Verified**
- 관련 요구사항: `REQ-WT-003`, `REQ-WT-005`, `REQ-WT-007`
- 부분 증적 인수 기준: `AC-WT-003`, `AC-WT-006`

## 수용 범위

- user layer slot 8의 정확한 `TransferLineOfSight`, mask 256
- 유일한 새 public Unity component `TransferTarget`과 private serialized schema
- engine-free `ITransferTargetModifierSink` binding authoring 검증
- 명시적 serialized·ordinal-sorted target registry와 missing/duplicate 거부
- trigger 제외, player/target root·child 제외, composite 차단
- fraction 동률의 scene·hierarchy·type·numeric component ordinal 정렬
- delimiter 충돌 없는 length-prefixed canonical collider identity
- 64-hit nonalloc buffer 포화의 target/tick 포함 fail-closed 오류

## 실행 결과

| 시험 | 결과 | SHA256 |
|---|---:|---|
| `TestResults/vd02-m2b1-authoring-final4.xml` | 6/6 PASS | `CE6DB2766587637AB77E3B5E62DD7CF5D5957EBED2EB4A8A2A8032CE84F987DD` |
| `TestResults/vd02-m2b1-los.xml` | 5/5 PASS | `E9209F81B4059E6FB3A330D95E267996E17CC7E4FB2926EFBAE9CEB669F4A082` |
| `TestResults/vd02-m2b1-transferunity-editmode.xml` | 15/15 PASS | `60327469CC3D51A36B63AB2FFEB7D8775F18E1D95F47B3781AC3670DE2B50922` |
| `TestResults/vd02-m2b1-transferunity-playmode.xml` | 9/9 PASS | `C3E3422832ECEB38427C2FB1A5DB77811690BBCAC0ECC02B660C7CDF2800CCC9` |

## 핵심 소스 고정값

- `TransferTarget.cs`: `EFB616201B59984094E6EBE48CDE6DEE20FFEB23AAA859992860E4EF88CD2188`
- `TransferLineOfSight.cs`: `9A74F5887ADB7F27E2DE57E10A31D678553492D8CD1BB35479368E710825B772`
- `TransferTargetAuthoringTests.cs`: `47D1A5C66A5F83FE8814E77FE2E94D52A205CAD9F7781B220D11D622D8987247`
- `TransferLineOfSightTests.cs`: `1FBD606ECF94D6FBB3467A44B0EE5A934EA36CEA7DEE77873904053868159081`

## 검토 중 교정

- PlayMode 테스트 어셈블리에 필요한 Core 참조 누락을 보완했다.
- 이름 없는 EditMode scene을 valid authoring으로 취급하던 테스트를 실제 임시 named scene asset 생성·즉시 삭제 방식으로 바꿨다. production의 named-scene 검증은 유지했다.
- delimiter 문자열 하나로 합친 collider key가 충돌하고 tuple 순서를 왜곡할 수 있어, 내부 identity를 scene·hierarchy segments·type·numeric ordinal로 분리하고 canonical display/duplicate key만 length-prefix로 구성했다.
- Unity가 생성한 `SceneTemplateSettings.json`은 범위 밖 부산물이라 증적 수집 후 제거했다.

Luna는 재현 결과와 소스를 독립 검토해 M2B1 범위에 남은 P0/P1이 없다고 판정했다. Kimi production 코드는 채택하지 않았고 반복 LOS/authoring 테스트 분해만 Terra 검토 후 활용했다.

## 보류 범위

- target pose observation builder와 한 tick당 단일 `Physics2D.SyncTransforms`
- TransferSession과 target sink apply/clear
- movement preflight·non-rejecting final modifier reflection
- lifecycle/removal/same-tick press와 30/60/144 render-group replay
- `AC-WT-002` 실제 enemy behavior는 VD-03
