# C3 하위 확인 요청 보정본 독립 검수

- 검수자·실제 호출 모델: Luna, `gpt-6-luna`
- 날짜: 2026-09-29
- 범위: 지정된 고정 하위 런타임·시험 소스 정적 검수와 실제 집중 실행 증거 대조. 소스·시험·Unity 실행은 변경하지 않음.
- 런타임 SHA-256: `DD53C5E52649063F8858D971FCF2DC7D653EAEA382E89F916AE031AA80869A7E` — 지정 지문과 일치
- 시험 SHA-256: `93C1C78611BFEE915E118886599BA8A82FFF0C340006D44454EAFD441647D54B` — 지정 지문과 일치

## 판정

P0: 0. 최초 검수 P1 두 건의 **구현 보정은 코드에서 확인**했다. 예약 CAS 전에 보관 capture를 검증하고 현재 인증 root를 재확인한다. 예약 후 완료 검증/등록 예외는 예약 상태를 `Closed`로 보내며 원래 요청을 재예약할 수 없다. 인증 전 foreign owner 호출은 상태를 소모하지 않고, 동일 요청·owner의 후속 완료가 가능하다. `IdentityFor`는 프로젝트 소스에서 더 이상 발견되지 않는다.

그러나 최신 소스에서 추가된 경계 결함과 새 음성 시험의 공백 때문에 보정된 보안 경계를 수용할 수 없다. **P1 두 건**을 남긴다. 따라서 하위 단위의 독립 수용은 보류하며 AC-M5D7QC3-010의 Luna P0/P1=0 조건은 충족하지 않는다.

## P1 — 발급 witness의 변조·복제 음성 행 미완결

`AC001_ReflectedCaptureResultFieldsOrCloneCannotAuthenticateRecapture`는 `_owner`, `_root`, `_outcome` 변조를 검사한다. 시험명에 clone이 있으나 실제 복제 객체 생성·거부 검증은 없다. `_capture` 및 `_classification` 반사 변조도 이 시험에서 직접 확인하지 않는다. 다른 시험은 projection 참조 제거와 세 leaf 존재 여부를 검사하지만 발급 결과의 위 두 필드와는 별개다. 결과 등록표가 발급 결과 객체 자체와 모든 witness 필드를 함께 비교하므로 코드상 방어 구조는 보이나, 보정 승인 및 최초 검수의 명시된 반사 witness 요구를 실제 시험이 입증하지 못한다.

추가로 `ConfirmedRequestWitness`는 요청 생성에 사용된 `ResultWitness` 또는 원본 실제 capture-result 참조를 보존하지 않고 `Capture`만 보관한다. 따라서 같은 owner/epoch/adapter/router/root에서 발급된 다른 실제 capture를 witness의 `Capture`에 반사 대입하면 개별 capture 자체 검증과 root 검사가 통과할 수 있고, 요청과 원래 mint capture의 동일성은 증명되지 않는다. 원래 실제 발급 결과와 mint capture를 witness에 결합하거나 동등한 변경 불가능한 발급 증명을 유지하고, 이 대체 시도를 거부하는 음성 행이 필요하다. 이는 AC-M5D7QC3-001/004/008의 요청 출처·동일 identity 보장에 대한 구현 P1이다.

## P1 — 예약 CAS 직전 실패와 비소모성 입증 부족

런타임 `ReserveExecutionCommit`은 요청 cohort를 인증한 뒤 CAS 전에 `ValidateRequestAtActiveRoot`를 호출한다. 이 호출은 `try`/terminal-close 처리 밖에 있다. 따라서 projection/root 검증 실패는 상태 변경 전에 예외가 나고 요청은 `Issued`(상태 0)에 남아 복구 후 재예약할 수 있다. 승인 보정은 인증 전 foreign-owner 불일치는 비소모로 두되, 인증된 요청 이후 실패는 닫도록 구별했다. 현재 `AC008_ReservedProjectionOrRootCorruptionClosesTheOriginalRequest`는 예약 **후** projection/root를 훼손하여 완료 실패가 terminal close되는지를 확인할 뿐, 인증된 요청의 예약 전 검증 실패를 닫는지 확인하지 않는다. 이는 시험 공백에 그치지 않고 승인된 terminality와 어긋나는 구현 P1이다.

필요한 수정·시험: 외부 owner/epoch가 요청 인증에서 거절되면 원래 권한이 보존되는지 유지한다. 요청 인증이 성공한 뒤 CAS 전 검증 실패 또는 예외가 나면 원래 요청을 terminal `Closed` 처리하고, root/projection 복구 뒤에도 재예약·완료할 수 없음을 시험한다. 추적 대상은 REQ-M5D7QC3-006/007, AC-M5D7QC3-006/008 하위 부분이다. AC-007/008의 전체 기준은 ADR-0036에 따라 계속 Open/Not Verified다.

## 실제 실행 증거와 수용 범위

실행 기록과 `artifacts/c3-lower-r1-verification.json`은 총 21, 통과 21, 실패·건너뜀·판정 불가 0, 실제 runner 종료 코드 0, 입력 876개의 전후 차이 0을 보고한다. 지정 런타임·시험 지문이 결과 기록과 일치하며 결과 XML SHA-256 `28BD256C3DC28F24EFC3DF5B7AC0DA353C7176E08E41FCB90B9E1C36CB81AC7C`도 재계산해 일치했다. 시험명 21개도 XML과 결과 원장에서 일치한다. 이는 실제 집중 실행 사실을 뒷받침하지만 위 두 공백을 메우거나 독립 수용을 대신하지 않는다.

부분 실행 범위는 AC-M5D7QC3-001/004/008의 일부 하위 경계와 AC-M5D7QC3-009의 하위 API 검토에 국한한다. 실제 Q-B 발급 출처, 상위 owner의 Confirm/Cancel 경쟁, 취소 후 새 cursor 재무장, C3 전체 선행 기준 AC-M5D7QC3-010의 필수 회귀는 입증되지 않았다. AC-M5D7QC3-007/008 전체, C3 전체 및 C4는 수용하지 않는다. 실행 기록에는 앞선 모델들의 사용량 중단은 적혀 있지만, 각 모델의 실제 오류 원문과 전후 완료 보고는 이 증거 묶음에서 확인할 수 없으므로 한도 오류의 개별 귀속·시점을 독립 판정하지 않는다.

## 결과

- P0: 0
- P1: 2 — 실제 capture-result 출처/동일성 witness 결손 및 반사·복제 음성 행 불충분, 인증된 예약 CAS 직전 실패의 미종료 상태와 관련 음성 행 부재
- 코드 수정: 없음
- 독립 수용: 보류
- Astra 통합 승인: 이 검수는 승인하지 않음
