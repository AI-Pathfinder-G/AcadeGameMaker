# VD-09 M5D7P-B Unity UI package baseline — Luna 사후검수

- 검수일: 2026-09-20
- 검수자: Luna (`gpt-5.6-luna`), 독립 사후검수
- 구현자: Terra (`gpt-5.6-terra`)
- 계약: `docs/specs/work-contracts/2026-09-20-vd09-m5d7p-b-ui-package-baseline.md`
- 기준: `AGENTS.md`, ADR-0032, `docs/agent-operating-model.md`, 승인 계약과 개정 사전검토, 구현 증거, Unity 6000.6.0f1 설치 패키지 메타데이터 및 등록·컴파일 로그

## 판정

**PASS — P0=0, P1=0, P2=0.** AC-M5D7PB-001부터 004까지 통과한다. 승인된 uGUI 직접 의존성, 해당 패키지가 요구하는 정확한 lock 그래프 변경, 내장 TMP 어셈블리 등록·컴파일, resolver smoke와 전체 회귀 결과를 독립 확인했다. 동결 기준선과 최종 패키지 파일의 SHA-256도 기록값과 일치한다.

## 기준선·최종 파일의 독립 비교

구현 기록이 가리키는 임시 스냅샷을 직접 읽고 SHA-256을 재계산했다. 기준 파일은 `C:\Users\me\AppData\Local\Temp\acade-m5d7p-b-20260920\manifest.json.baseline` (205 bytes), `packages-lock.json.baseline` (4,804 bytes)에 있다.

| 파일 | 동결 기준 SHA-256 | 최종 SHA-256 | 독립 의미 비교 |
|---|---|---|---|
| `Packages/manifest.json` | `7EE76CE89F845F718D861C1015E07C02F4C9A469E5ECCA71E396E5363D97DD25` | `AD3A5D2329D5A8FD71BE89873C75BA2CAA98B4391558A69D83D81CADFE684E1E` | `com.unity.ugui: 2.6.0` 단일 추가. 기존 직접 의존성은 그대로이며 TMP shim 항목 없음. |
| `Packages/packages-lock.json` | `C0DA832620B8DB22C1E607D3E2638680C29186A871F55FA6FA5B453DD27432A9` | `B2B9B1DE4381F3C289837ACD1BDE69013C1F7581511E26861DFDF4F5AE00E550` | uGUI 및 `audio` 추가, 기존 `physics`·`ui` 깊이만 변경. 제거 또는 그 밖의 package entry 변경 없음. |

Lock 차이는 정확히 다음과 같다.

- 추가: `com.unity.ugui` — `2.6.0`, `builtin`, depth `0`; 종속성은 `com.unity.modules.ui`, `imgui`, `audio`, `physics2d`, `physics` 각각 `1.0.0`.
- 추가: `com.unity.modules.audio` — `1.0.0`, `builtin`, depth `1`.
- 변경: `com.unity.modules.ui`와 `com.unity.modules.physics`만 depth `2`에서 `1`로 변경.
- `imgui`는 depth `1`, 기존 직접 `physics2d`는 depth `0` 유지. 제거 항목 및 나머지 package 차이 없음.

이 결과는 승인 계약의 frozen dirty working-tree 기준선에 대한 의미 비교다. 기존 lockfile의 Git HEAD 차이를 이 작업의 변경으로 돌리지 않았다.

## AC별 결과

| 인수 기준 | 결과 | 독립 증거 |
|---|---|---|
| `AC-M5D7PB-001` | **PASS** | 기준선과 최종 manifest를 비교해 uGUI `2.6.0` 한 항목만 추가된 것을 확인. `com.unity.textmeshpro`는 manifest에 없다. |
| `AC-M5D7PB-002` | **PASS** | 기준선·최종 lock을 구조적으로 비교. uGUI는 내장 `2.6.0`, depth `0`이며 설치 메타데이터와 종속성 5개가 정확히 같다. shim lock 항목은 없다. |
| `AC-M5D7PB-003` | **PASS** | 설치된 Editor `6000.6.0f1`의 BuiltInPackages 메타데이터, smoke의 package resolve/registration 기록 및 `Unity.TextMeshPro`·`Unity.TextMeshPro.Editor`의 Csc 성공을 확인. |
| `AC-M5D7PB-004` | **PASS** | 허용된 package 의미 차이만 존재. package smoke 성공, R01 전체 EditMode 및 R04 전체 PlayMode가 실패·skip·inconclusive 없이 완료. R02/R03는 비수용 이력으로 분리되어 있다. |

### 설치 패키지와 resolver 확인

설치된 `com.unity.ugui` 메타데이터는 버전 `2.6.0`이며 위 다섯 모듈만 선언한다. `Unity.TextMeshPro.asmdef`는 `autoReferenced: true`; 포함된 소스는 `namespace TMPro`를 제공한다. 반대로 설치된 `com.unity.textmeshpro`는 `5.0.0`, `type: shim`, unsupported 안내가 있고 uGUI만을 향한다. 프로젝트에서는 이 shim을 직접 선언하지 않는다.

Smoke 로그에서 package manager는 해석을 완료하고 25개 package를 등록했으며 lock 수정 뒤 내장 `com.unity.ugui@2.6.0` 및 필요한 built-in module들을 표시한다. 같은 로그에서 `Unity.TextMeshPro.dll`과 Editor 어셈블리의 Csc 단계가 성공적으로 끝난다. package resolve 실패나 uGUI 버전 대체는 기록되지 않는다. 이 package-only 계약은 게임 소스의 `TMPro` 소비자 컴파일을 요구하지 않으며, 그 검증은 후속 authored-presentation 계약에 남는다.

### 실행 증거 재계산

| 실행 | 독립 XML 결과 | 시간 | SHA-256 |
|---|---|---:|---|
| package-resolution smoke | 성공적으로 종료; resolver가 uGUI 2.6.0을 등록하고 TMP 어셈블리를 컴파일 | — | log `7A9B151C77632B4CFAF196FD157BAEEC6ED0015A0F1E000DFA4EC1BE9155ADE4` |
| R01 전체 EditMode | 688/688 pass, fail/skip/inconclusive `0/0/0` | `875.9722615s` | XML `503E27174523960D5EAE7423F7C3C0D0B6971385D34010DDB08981FCC5453B9E`; log `2F8D05AE37D03A9B16615FA1EBF80152C025BF05E300020AEB5FA5E09E4B1C5C` |
| R04 최종 전체 PlayMode | 805/805 pass, fail/skip/inconclusive `0/0/0` | `3303.2216857s` | XML `EBAD69083000D2C67299D0C87A985CD60856460377E3EE1071BDA72B25159CCD`; log `5FE051B4DC63914FB2FCBD3B472CC269D9C0D1AB9A52091CD399EC8A3FE007A0` |

Unity package metadata 재계산 해시: uGUI `package.json` `817628B5AD7FD255E49B12D168872BA0904FCEE1CF6D5E58ED78B1C0E9ACDF6E`; TMP shim `package.json` `31F8D2CDB225BED17AA0F24EF6C4D0C075B94D19A93A7D72D8680C03DF5125DC`; uGUI `Runtime/TMP/Unity.TextMeshPro.asmdef` `1630B20F8F0030AF5AC996A6F60793076C8285BEDFFABC9BCBBFE70A3D5811CA`.

## 비수용 실행 이력

- R02에는 XML이 없다. 로그에서 M5D7P-A 입력 테스트 assembly의 `AcadeGameMaker.Core` 참조 누락으로 인한 C# compile errors를 확인했으며, 이 run은 인수 증거에서 제외한다. R02 log SHA-256: `FA573E7B94C0D0452144EBB74B79E9688794B6374FD49B148CB9A779E270CA4D`.
- R03는 805 중 799 pass, 6 fail로 완료했으며 인수 증거가 아니다. 실패 사례는 M5D7P-A 입력 소스 경계와 기존 terminal overflow 테스트였다. 후속 수정 뒤 R04가 805/805로 통과했다. R03 XML SHA-256: `FEEB5F7B9F38322945A3A9B3EC12F3EE9795FC1E5033DE2F4BEFA04809C3D138`; log SHA-256: `368BE3093C1AB7A915CC1184E1725238304FD9A56690E6CDF4FC8A7733A18CEF`.

이 두 기록은 원래 실패·컴파일 차단의 이력으로 남고, 최종 수용을 위해 재사용하지 않았다.

## 범위·독립성

독립 검수 범위에서 M5D7P-B의 package 변경은 계약이 허용한 manifest/lock 의미 차이와 문서·실행 증거로 제한된다. 이 검수는 runtime, test, scene, prefab, asset, ProjectSettings 또는 generated input을 편집하지 않았다. 작업 트리에 함께 보이는 M5D7P-A 및 기존 사용자 변경은 P-B의 package 기준선 차이로 귀속하지 않았다. Terra의 구현 판단을 수용 근거로 복사하지 않고 기준 스냅샷, 설치 패키지 메타데이터, Unity 로그와 XML에서 판정을 다시 산출했다.

## 심각도

- P0: 0
- P1: 0
- P2: 0

## 최종 판정

**PASS — M5D7P-B의 AC-M5D7PB-001..004가 충족됐다. Luna 독립 검수는 P0=0/P1=0이다. Astra의 최종 통합 수용과 상태 전환은 별도 권한으로 남는다.**
