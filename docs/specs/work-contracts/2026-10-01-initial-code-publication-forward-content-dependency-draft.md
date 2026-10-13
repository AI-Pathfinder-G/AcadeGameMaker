# 최초 코드 게시 501파일의 현재 내용·의존 폐쇄 조사 계약 초안

- 상태: **Draft.** 이 문서는 읽기 전용 파일별 조사와 새 증거 작성의 다음 한정 계약 후보다. 게임 코드·설정·자산·QA·Git·원격을 바꾸거나 Unity·빌드·게시·클린 복제를 실행할 권한이 아니다.
- 선행 게이트: [현재 출처 기준 Approved v1 계약](2026-10-01-initial-code-publication-forward-provenance.md)과 v1 원장（`../../../artifacts/c4-initial-publication-forward-provenance-v1.json`；로컬 보존 자료） SHA-256 `C5D9C98C1F44A9D5A68F8F5C573787CF539E6069D2CE5EB7F0F35BB36B753EBE`, [루나 결과](../../verification/2026-10-01-initial-code-publication-forward-provenance-result-luna-review.md) SHA-256 `D964D98D5C8D01FC08CE9FFE820DD9D9E30D0B60EAB71A4150090B658D3C1E56`의 P1 두 건을 보존한다. [v2 보정 Approved 계약](2026-10-01-initial-code-publication-forward-provenance-v2-correction.md) 아래 v2 501행 원장（`../../../artifacts/c4-initial-publication-forward-provenance-v2.json`；로컬 보존 자료） SHA-256 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`와 [테라 결과 보고](../../verification/2026-10-01-initial-code-publication-forward-provenance-v2-terra-report.md)는 작성됐다. **루나의 v2 최종 독립 결과 검수와 아스트라 제한 수용이 확인되기 전에는 이 계약의 501파일 정식 조사에 착수하지 않는다.** 사용자는 루나 검수를 한도 회복 뒤 별도로 진행하고 다음 작업을 계속하도록 지시했다. 이에 따라 [예비 읽기 전용 대조](../../verification/2026-10-01-post-pr6-preliminary-readonly-inventory.md), [별도 Approved 기반 33파일 예비 조사](2026-10-01-initial-code-publication-foundation-33-precheck.md), [메타 254·조립 정의 20 예비 조사](2026-10-01-initial-code-publication-meta-asmdef-precheck.md)만 선행한다. 이 결과는 이 501파일 계약의 승인이나 내용·의존성 폐쇄가 아니다. v1 원격 숫자와 세 잘못 결속된 문서 인용을 성공 근거로 선취하지 않는다.
- 추적: `REQ-M5D7QC3-001/005/006/007`, `REQ-M5D7QC4-001..007`, `AC-M5D7QC3-009/010`, `AC-M5D7QC4-009/010`. 개별 파일에는 해당 Approved 규격의 정확 REQ/AC와 금지 동작을 추가로 결속한다. 대응 Approved 요구가 없으면 문서 이름 언급을 승인으로 만들지 않고 미입증 또는 제외 후보로 둔다.
- 별도 [현 패키지 캐시 예비 조사](2026-10-01-initial-code-publication-package-owner-precheck.md)는 위 메타 후보에서 소유를 찾지 못한 291개 GUID 참조 행 중 현 캐시 소유265·미발견26을 나눴다. 직접 패키지 요청/잠금 두 버전 차이도 기록했다. 이는 현 로컬 캐시의 사실이며 이 계약의 501파일 정식 조사, 깨끗한 복제 재현 또는 게시 허용을 대신하지 않는다.
- 별도 [잔여 26참조 현 환경 예비 조사](2026-10-01-initial-code-publication-residual-guid-precheck.md)는 네 로컬 메타 검색 범위에서 26행/9값의 정확 소유를 찾지 못했다. 기존 Renderer2D 일곱 값의 독립 검수 P1 차단은 유지한다. [기존 패키지 버전 승인 문맥](../../verification/2026-10-01-package-version-authority-crosscheck.md)은 현재 요청/잠금 차이를 새 변경이나 즉시 수정 근거로 다루지 않는다. 두 결과 모두 실제 Unity 가져오기 또는 이 계약의 승인 근거가 아니다.
- 별도 [첫 501파일 격리 Unity 진단](2026-10-01-initial-code-publication-provisional-501-unity-import.md)은 패키지 자산 접근·가져오기 오류를 남겼다. [짧은 경로 재진단](2026-10-01-initial-code-publication-short-path-unity-recheck.md)은 라이선스·패키지 관리자 연결 실패로 가져오기 전에 종료되어 첫 오류의 경로 원인을 판정하지 못했다. [Git 줄바꿈 정규화 예비 대조](../../verification/2026-10-01-initial-publication-git-canonicalization-precheck.md)는 501개 중 18개의 현 원문 바이트가 Git 기록 바이트와 다름을 확인했다. 이 결과들은 내용·의존성 폐쇄나 게시 승인 근거가 아니다.

## 제안 요구와 인수 기준 — Draft 전용

다음 ID는 이 초안의 검토 대상이며 `Approved` 요구나 구현 권한이 아니다. 기존 제품 계약의 REQ/AC를 대체하지 않는다.

| 제안 요구 | 측정할 인수 기준 |
| --- | --- |
| `REQ-PLAT-015` — 501개 경로와 현재 바이트의 조사 시작·끝 기준을 보존한다. | `AC-PLAT-013`: 501개 고유 경로의 기준·시작·끝 SHA가 501/501 일치하고 누락·중복·변경이 0이며, 하나라도 다르면 포함 판정을 발급하지 않는다. |
| `REQ-PLAT-016` — 각 파일의 실제 내용을 적용 가능한 Approved 요구·금지 동작·독립 근거의 정확 바이트와 결속한다. | `AC-PLAT-014`: 501개 행 모두 내용 판독 방식과 요구 근거 또는 명시적 미입증 사유를 가지며, 단순 이름 언급·폴더 소속·시험 통과만으로 내용 적합을 판정한 행은 0이다. |
| `REQ-PLAT-017` — 게시 후보의 직접·간접 의존과 GUID/조립/패키지/설정 참조를 실제 경로와 바이트로 연결한다. | `AC-PLAT-015`: 발견한 모든 참조 변에 출발·도착·종류·원문 위치가 있고, GUID 중복·누락·범위 밖 필수 파일·미해석 참조는 각각 수량과 영향 행을 명시한다. 미해석 참조가 도달하는 행의 의존 폐쇄 값은 거짓이다. |
| `REQ-PLAT-018` — 원격 충돌과 Q0·복제 조건을 포함 후보와 분리해 실패 폐쇄한다. | `AC-PLAT-016`: 최신 원격 HEAD·트리·존재 파일 blob을 조회 시각에 결속하고, 두 기존 충돌 경로를 포함해 미결정 충돌·Q0 조건 불일치가 있는 행에 게시 허용을 주지 않는다. |
| `REQ-PLAT-019` — 조사 자체의 결과와 후속 승인 경계를 기계 판독 가능하게 남긴다. | `AC-PLAT-017`: JSON에 501개 고유 행과 참조 변·누락 목록·포함/제외/미입증 사유가 있으며 모든 행의 `PublicationApproved=false`다. 독립 루나 검수와 아스트라의 별도 파일별 허용 결정 전에는 코드 게시를 하지 않는다. |

## 조사 단위와 증거 기준

대상은 최초 501파일 원장（`../../../artifacts/c4-initial-publication-byte-closure-v1.json`；로컬 보존 자료） SHA-256 `E43A4268ABC87A56AD4F4A198D96893135706EB1364ABB33D70BA27741765B17`의 고유 `Path`·`CurrentSha256` 501쌍이다. 초기 후보489와 C4 신규12를 구별하고, v2에서 별도로 확정될 현재 기준의 같은 501쌍·UTC 시작/종료·원격 결속을 착수 때 재확인한다. 실물·v2 기준·501 원장 중 어느 경로 또는 SHA가 다르면 새 바이트로 조용히 대체하지 않고 중단한다. 조사 전후 전체 501 실물 지문도 기록해 읽기 중 변경을 검출한다. 원래 직접 근거61/공백428/현재 일치60, 기존 변경10의 14전이·14열린 간격·닫힘0/10은 **역사 축**으로 불변 보존한다.

각 파일 행에는 정확 경로·기준/실물 SHA, 자료 종류와 실제 내용 판독 방식, 소유 조립/패키지/자산 역할, 적용 Approved 계약의 상태·SHA·정확 줄·허용 범위·REQ/AC, 해당 바이트에서 확인한 요구 충족 근거와 금지 동작 검사 범위, 독립 검수의 정확 파일/SHA/줄, 직접·간접 의존의 `From/To/Kind/GuidOrReference/Evidence`, 누락/모호 참조, 현재 원격 상태, `ContentEvidence`, `DependencyClosure`, `InclusionCandidate`, `ExclusionReason`, `PublicationApproved=false`를 둔다. 정적 텍스트 검색은 호출 결과나 실제 실행의 부재를 증명하지 않으므로 소스는 실제 호출 경로와 권한 경계를 읽고, 실행으로만 알 수 있는 부분은 `미검증`으로 남긴다. 문서에 이름만 나온 파일, 같은 폴더나 조립에 속한 파일, 메타의 짝 소스 승인만으로 현재 바이트의 승인을 추론하지 않는다.

읽기 전용 의존 그래프는 실제 게시 후보의 프로젝트 설정·장면·프리팹·입력 자산·렌더러, C3/C4 런타임과 시험 선택/QA 입력에서 시작한다. YAML/직렬화 GUID, meta 소유자와 짝 asset, 장면·프리팹 컴포넌트, shader/include/font/렌더러, C# 형식·`asmdef` 참조·친구 조립, `Packages` 요청/잠금 및 `ProjectSettings` 값, QA 스크립트의 직접 읽기 경로와 선택 원장을 각각 근거 줄·필드로 연결한다. GUID의 단일 소유·참조 대상의 정확 바이트·중복·범위 밖 의존을 검사한다. 후보 501 밖의 필수 파일이 나오면 자동 포함하지 않고 새 범위 승인 후보로 기록한다. 제외 후보가 필수 경로에서 도달되면 폐쇄 실패다.

## 우선순위 있는 조사 묶음

| 순서 | 실제 파일별 조사 | 실패 시 처리 |
| --- | --- | --- |
| 0. 기준과 차단 확인 | 독립 검수와 제한 수용을 마친 v2의 501경로/현재 SHA 및 이 조사의 전후 지문을 확인한다. 변경 요청 #6 이후 원격 `main`이 `e95bb5169ad2ed08106bd342915d4637a1241263`으로 이동했으므로 새 HEAD·트리·blob을 정확 재조회한다. v2의 상태는 현 SHA만 확인471, 문서 근거 있으나 의존 미입증22, 신규 meta 제한 검수6, 원격 동일 경로 충돌2이며 전체 허용 목록은 비어 있다. | v2 P1, 원격 충돌2, 기준 변경, 원격 시각·blob 미결속이면 해당 행을 미입증으로 두고 포함 판정은 닫는다. |
| 1. 프로젝트 기반·실행 자산 | `Packages` 2와 바이트 기반1, `ProjectSettings` 10, 실행 자산20의 실제 내용·역할·요구를 판독한다. 입력 자산·장면2·프리팹4·렌더러·폰트·shader와 패키지/설정 참조를 연결한다. | 미해석 `Renderer2D` GUID, 버전·직렬화·자산 참조 불일치 또는 출처 미확인은 실패 폐쇄한다. 엔진 내장 가능성만으로 성공 처리하지 않는다. |
| 2. 메타·조립 그래프 | 기존 파일 meta208·폴더 meta40·신규 C4 meta6의 GUID, 짝 자산, 단일 소유와 참조처를 검증한다. 조립파일188·신규 C4 소스6 가운데 `.asmdef` 20의 참조·친구 조립, 실제 형식/패키지 방향을 연결한다. | meta SHA/소유권, GUID 중복·실종, 조립 또는 범위 밖 필수 파일은 미폐쇄로 남긴다. 신규 meta6의 제한 수용은 전체 의존 폐쇄가 아니다. |
| 3. 제품 내용과 권한 | C3/C4 변경10·신규 source/test6을 포함한 각 소스의 실제 구현, 허용 파일·행동, 금지된 권한/효과·호출 경계를 Approved REQ/AC와 비교한다. 나머지 조립 소스도 실제 사용·요구·참조를 파일별 판정한다. | 현재 SHA 검수 또는 기존 Unity 통과만으로 내용 적합·게시 적합을 발급하지 않는다. 금지 동작/미확인 호출은 별도 결함·미입증이다. |
| 4. 검증·게시 입력 | QA 도구·선택 원장13, Q0 직접 읽기 문서4, 외부 작업자2, 감사 원문1 및 기타 후보를 실제 스크립트 읽기 경로·선택/행 원장·원본 핀과 결속한다. | Q0의 Git 변경·미추적 작업 상태 요구와 깨끗한 복제의 충돌은 별도 판정 전까지 실패 폐쇄한다. QA 경로를 시험 결과가 있다는 이유로 임의 제외하지 않는다. |

위 종류 집계는 501 **전체 후보**의 원장 분류이며 원래 428 역사 공백 분류와 섞지 않는다. 묶음 사이의 의존은 앞 단계로 되돌아가 재검증한다. 특정 묶음의 결론을 얻지 못해도 다른 행의 읽기 전용 조사 사실은 기록할 수 있으나, 파일별 `DependencyClosure=true` 또는 전체 포함 허용으로 전파하지 않는다.

## 출력·독립 검수·후속 게이트

승인 후 테라가 새로 작성할 후보는 `artifacts/c4-initial-publication-content-dependency-v1.json`, `artifacts/c4-initial-publication-content-dependency-v1.md`, `docs/verification/2026-10-01-initial-code-publication-content-dependency-terra-report.md` 세 파일이다. 루나 독립 결과 후보는 `docs/verification/2026-10-01-initial-code-publication-content-dependency-luna-review.md` 한 파일이다. 이 Draft와 네 출력은 작성 전에 부재했고 v19 입력1084·source193 경로 밖이었다. 기존 501/v1/v2 원장과 증거·승인·실행 결과는 보존한다. JSON은 501 고유 경로·정확 SHA/REQ/AC·의존 변·GUID 단일성·누락/모호 참조·포함/제외/미입증을 완전 출력하고, 정상 행과 음성 행 모두 추적 가능하게 한다. 테라의 자체 판정은 수용이 아니다. 루나는 실제 바이트, Approved 요구/금지, 그래프 폐쇄와 미해석 항목, 원격·Q0 차단을 독립 재대조해 P0/P1과 파일별 한계를 보고한다. 아스트라만 그 뒤 읽기 전용 근거 분류를 제한 수용할 수 있다.

원격 동일 경로 충돌2, Q0 깨끗한 복제 조건, 미해석 `Renderer2D` GUID는 근거 없이 정상화하거나 우회하지 않는다. 현 단계의 `PublicationApproved=false`와 빈 허용 목록은 유지한다. 파일별 내용·의존 근거가 닫혀도 정확 포함/제외 목록은 별도 아스트라 승인, 최신 원격 재조회와 깨끗한 복제의 Unity `6000.6.0f1` 가져오기·컴파일·C3/C4 선택/행/종료값 검증은 또 다른 계약과 실제 실행·루나 독립 검수가 필요하다. 이 Draft는 그 승인이나 게시를 대신하지 않는다.
