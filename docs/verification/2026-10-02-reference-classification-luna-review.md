# 미해석 참조 소유 분류 독립 검수

검수 계약 `docs/specs/work-contracts/2026-10-02-reference-classification-independent-review.md` SHA-256 `AF434E6CEC0CE071FD579BE099FC61D89D51FCC80CF74726159354B4CB30FC3D`; 소유 규격 SHA-256 `5BAAAC79F141226C27E748DD8DBC8EBF8C9B31F93156BC86943A5D3726E6C21A`. 입력 분류 JSON SHA-256 `79ABC17E2A8E4B6BBAE8447BCE6838CC01C43967026DA68DE17A2584B4E61262`, 테라 보고 SHA-256 `29031B41237218D39D75741337DC4D8F80088148E3E44D39ED9E4C3A613E44E3`, 원본 조사 SHA-256 `AE4DE92037B0A345ADB9AA42710112C07EB54A9BD512C1A6309444EFB02A4F4A`, 534 원장 SHA-256 `89BC3D6EFDD2B1344E8E6F4F74E819382F7AB2F9C67EBFE373B3BF62F4D26F9E`가 계약과 일치했다.

## 판정

- 분류 산출물 자체: P0 0, P1 0. AC-PLAT-061 및 AC-PLAT-062는 **현재 소유 분류·534 포함 및 원장 지문 범위에서 충족**한다. 이는 전체 의존 폐쇄나 게시 수용 판정이 아니다.
- 기존 미수용 P1은 유지한다. Renderer2D의 미해결 GUID 7개, 두 패키지 요청/잠금 버전 차이, 깨끗한 복제·이식성, 과거 14전이 및 원격 충돌은 이 보고로 해소되지 않는다.

## 독립 대조 결과

1. **AC-PLAT-061 — 388/68 집합과 원문 위치.** 388 참조 행은 중복 없는 1–388 순번, 37개 원문 소스 경로로 구성됐다. 실제 원문 전 행 SHA가 분류 기록과 일치했고, 기록된 행 번호마다 원문 `RawValue`를 재확인했다(값 불일치 0). `SourceLineText`는 긴 행 14건에서 원문 접두 발췌로 짧지만 정확 행 번호와 값은 원문에 존재한다. 이를 완전한 원문 행 인용으로 확대 해석하지 않는다. 68 범위 밖 경로는 전부 고유하며 중복 0이다.
2. **소유 경로와 GUID.** 실제 현재 메타 파일 8,425개를 읽어 대조했다. 캐시 단일 메타 소유 266건의 GUID, 메타 SHA, 짝 자산 SHA 및 소유자 연결 불일치 0; 중복 소유 0이다. 실제 메타 스캔에서 소유되지 않은 참조 26건은 고유 GUID 9개이며 해당 GUID는 Assets/Packages/PackageCache 어느 검색 루트에도 발견되지 않았다. asmdef는 실제 183개(기록 183개)에서 대조했고 단일 소유 13건의 asmdef·짝 meta·패키지 선언 SHA와 원문 이름 값 불일치 0이다.
3. **패키지 결속.** 네 소유 패키지의 실제 `package.json` 지문·이름·캐시 버전이 JSON과 일치했다. 현재 manifest/lock도 읽어 두 기록 차이를 재현했다: `com.unity.render-pipelines.universal` 요청 17.3.0/잠금 17.6.0, `com.unity.test-framework` 요청 1.6.0/잠금 1.8.0. 캐시 소유는 현재 관측이며 이식 가능한 공급 증명이 아니다.
4. **AC-PLAT-062 — 534 및 TMP.** 534 원장은 534/534 정확 경로, 저장된 사전·사후 차이 경로 0, 원문 534행 SHA는 현 파일과 534/534 일치한다. 후보 SHA와 같은 509행은 원문/후보가 동일하고, 나머지 25개는 현 원문과 정규화 후보가 구분되어 있다. 범위 밖 68개 중 후보 포함 4개는 `Assets/UI/Resources/TMP Settings.asset`, `docs/evidence/c4-audit-current-source-pins-v2.json`, `docs/evidence/c4-runtime-predecessor-manifest.json`, `docs/verification/2026-09-30-c4-editmode-disposal-pointer-recovery-luna-implementation-review.md`; 각 후보 지문이 534와 일치한다. `TMP` 부분 경로는 `HubPresentationScopeAuditTests.cs:13`의 공백 포함 문자열로 복구했고 실제 파일/후보 지문 `71A6F2477A384F5D12D27AC22F6C30449ED87AA21FD0906FB119442FEAEECBD4`가 일치했다. 참조행 기준 후보 대상 9건도 각각 534 대상 경로와 후보 지문이 일치한다.
5. **음성·미입증 경계.** 문서/기록 인용, 동적·부분 문자열, 정규식, 시험 생성물 및 실제 미발견을 분류별로 구별했다. TMP 생성 fixture, 템플릿 기본 장면은 필요성이 미입증이다. Renderer2D의 27행 중 미발견 GUID 행 7개는 여전히 기존 P1이며 엔진 내장/결함으로 추정하지 않았다. 실제 실행, 간접 호출 폐쇄, 전체 501 내용 적합은 이 검수 범위 밖이다.
6. JSON의 `DependencyClosure`, `PublicationApproved`, `WholeAccepted`, `LunaReviewed`, `CleanCloneExecuted`는 거짓으로 유지한다. 전체 의존 폐쇄·이식성·제품 동작·전체 회귀·게시 권한은 미수용이다. 네트워크·Git·Unity·제품/QA 시험 실행은 하지 않았다.
