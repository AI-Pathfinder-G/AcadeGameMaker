# VD-02 Unity M2A 고정 수학·LOS 끝점 증적

- 검증일: 2026-08-26
- 구현: Terra, Kimi K3 제한 초안은 Terra가 선별
- 통합 판단: Sol
- 독립 검증: Luna
- 판정: **PASS — M2A Verified**
- 관련 요구사항: `REQ-WT-001`, `REQ-WT-003`, `REQ-WT-004`, `REQ-WT-007`, `REQ-WT-008`
- 부분 증적 인수 기준: `AC-WT-003`, `AC-WT-006`

## 수용 범위

M2A는 다음 기반만 검증한다. target sink, 전체 LOS 필터링·정렬, movement bridge, lifecycle, replay는 M2B 이후에 검증한다.

- checked signed-64 `RoundDivAway`
- 고정 orthographic Q1000 world↔1920×1080 endpoint grid 투영
- integer screen-distance와 axis-aligned ellipse 판정
- mouse 2500 경계와 Q4096 정규화
- 승인된 24-step integer CORDIC angle key
- Unity Linecast의 origin fraction 0, interior, endpoint fraction 1, endpoint 뒤 제외 실증

## 실행 결과

| 시험 | 결과 | SHA256 |
|---|---:|---|
| `TestResults/vd02-m2a-fixed-math-final.xml` | 9/9 PASS | `223573EA4FFD3571C1D23271A1605354D28FA9C9971CF74D608F2DFECA14E839` |
| `TestResults/vd02-m2a-los-spike.xml` | 4/4 PASS | `2F440E1D04687C6750DF91253E5060A7298B446F1F54B99D56215B7B3AADBA28` |
| `TestResults/vd02-m2a-m1-regression.xml` | 17/17 PASS | `0673FCBA85AE14B3202EB112B345728F17C9DED2562C1E107D5AE92E4D9DFC00` |
| `TestResults/vd02-m2a-movement-full-regression.xml` | 22/22 PASS | `54BD77A42F2B5C80FAA0E8502B44FF5436438430CC83B8CD9262EE3A3E174241` |

## 핵심 소스 고정값

- `TransferFixedMath.cs`: `9C0A33A1E4CAC920825218D3627D3F4D36E113F0B5B68DF54CAA865465958E8A`
- `TransferFixedMathTests.cs`: `505C86B8223DEC06D92E421F1A9E4044B2E39077905A431BF1661511D1155D95`
- `TransferLinecastPhysicsSpikeTests.cs`: `564D12E46CCA4F589597A4E036E99B84ACE1BF58C5A864F64BB1A0A5E265F0B8`

## 독립 검토와 교정

Luna의 첫 검토는 overflow-before-mutation 증적 부족과 AC 과대 표기를 P1으로 판정했다. Terra는 production 동작을 바꾸지 않고 extreme valid projection 경계, inverse Q1000 conversion overflow, screen-distance overflow, mouse-distance·normalization overflow와 mutation sentinel을 추가했다. 또한 M2A 표기를 부분 `AC-WT-003`·`AC-WT-006`으로 제한했다. 재실행 9/9 PASS 뒤 Luna는 남은 P0/P1이 없다고 판정했다.

Kimi 초안에서는 doubled remainder overflow를 피하는 비교 주의점만 독립 검증 후 반영했다. 계약식을 받지 않은 추측성 projection 제안과 일반론은 반려했으며 Kimi 코드는 복사하지 않았다.

## 보류 범위

- trigger/self-child/target-child/composite/equal-hit/64-hit saturation LOS
- `TransferTarget`, owner sink, registry와 observation builder
- target mutation·session·movement modifier의 원자적 bridge
- lifecycle/removal/same-tick press와 30/60/144 render-group replay
- `AC-WT-002` 실제 enemy behavior는 VD-03
