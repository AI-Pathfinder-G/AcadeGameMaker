# Q0 단명 발급 세션 핵심 구현

상태: **Approved**, 2026-10-02 통합 책임자 승인. `REQ-PLAT-068..070`, `AC-PLAT-067/068`의 세션 핵심 구현이다. 비영속 제공자 v2는 실제 null/IsEphemeral true를 관측했으나 독립 검수는 별도 진행 중이다. 이번 구현은 합성 세션만 실행하며 실제 운영 키·권위 기록·계획 서명·원격 게시를 발급하지 않는다.

테라 `gpt-6-sol`의 새 허용 출력은 `qa/tools/Q0EphemeralIssuer.Core.psm1`, `qa/tools/Test-Q0EphemeralIssuerCore.ps1`, `artifacts/c4-q0-ephemeral-issuer-core-tests-v1.json`, `docs/verification/2026-10-02-q0-ephemeral-issuer-core-terra-report.md`이다. CNG의 실제 null 이름·RSA3072·ExportPolicyNone·IsEphemeral true를 검사하는 세션 생성/공개 정보 읽기/합성 승인 payload 한 번 서명/폐기 함수를 구현한다. 개인 매개변수 내보내기·키/PEM 저장·복원·로그는 금지한다. 세션은 프로세스 메모리만 사용하고 finally에서 폐기한다.

세션 생성 때 허용 계약 SHA와 권위 SHA, CurrentPublicationCleanClone 문맥을 고정한다. 미래 승인 루트의 진위를 이 모듈이 스스로 인증한다고 주장하지 않는다. 64자리 대문자 SHA·32자리 nonce·정확 현재 문맥을 검사하고 기존 AGM-Q0-PLAN-V1 ASCII/LF payload를 사용한다. 외부 검토된 계획 SHA/nonce를 전달하는 주체는 후속 실제 발급 계약에서 통합 책임자로 제한해야 한다. 여기의 합성 인수는 실사용 승인 근거가 아니다.

한 세션에서 서명은 한 번만 성공하며 재사용은 거절한다. TTL은 단조 시간으로 관리하고 기본45분·정수 유효 범위를 제한한다. 시험의 만료 관측은 TTL0으로 즉시 만료시켜 기다리지 않을 수 있다. 폐기·만료 뒤 서명과 개인키 복구를 거절하고 실패한 필드 검사로 서명 성공을 기록하지 않는다. 유효한 합성 서명은 이미 검수된 Q0 핵심 모듈을 이용한 독립 공개 검증과 대조한다. 다른 계약/권위·과거 문맥·비정규 SHA/nonce·만료·재사용·폐기·실제 제공자 관문을 각각 검사한다. 오류는 실제 사유를 기록한다.

기존 Q0 핵심/제공자/정리 도구와 증거는 보존한다. 실사용 진입점은 종료2를 유지한다. 보호 원문/534/505/캐시/설정/Git/원격은 불변이다. 결과의 전체 의존·게시·전체 수용·루나 수용 플래그는 false이다. 핵심 구현의 독립 검수와 운영 발급 승인은 별도이다.
