# Q0 분리 서명 검증 핵심 구현

상태: **Approved**, 2026-10-02 통합 책임자 승인. 소유 `REQ-PLAT-068/069/070`, `AC-PLAT-066/067/068`. 근거는 `docs/proposals/2026-10-02-q0-signed-authority-and-freeze-design.md` SHA `155A43134BACC29BFD3627E99E42E827E671E770AD29BE537429106F2D23BC89`와 Q0 구현 전 독립 검수이다.

이번 구현은 검증 핵심과 격리 시험이다. 기존 Q0/엄격 감사/C4 fixture/실행 진입점을 변경하지 않는다. 실제 승인 루트와 최종 공개키가 아직 발급되지 않았으므로 운영 검증은 반드시 거절한다. 합성 시험을 실제 승인으로 계산하지 않는다. `AC-PLAT-066/067`의 외부 인증과 실제 동결·서명 부분은 열린 상태로 유지한다. `AC-PLAT-068`의 핵심 형식·변조 거절·우회 부재만 독립 검수 대상으로 삼는다.

테라 역할 `gpt-6-sol`의 허용 새 출력은 `qa/tools/Test-Q0SignedPlanAuthority.ps1`, `qa/tools/Test-Q0SignedPlanAuthority.Core.psm1`, `qa/tools/Test-Q0SignedPlanAuthority.SelfTest.ps1`, `artifacts/c4-q0-signed-authority-core-tests-v1.json`, `docs/verification/2026-10-02-q0-signed-authority-core-terra-report.md`이다. 운영 스크립트에는 공개키 교체·서명 생략·합성 시험 매개변수가 없어야 하며, 아직 발급되지 않은 고정 공개키나 외부 인증을 꾸미지 않는다. 준비 상태에서는 명확한 비정상 종료를 반환한다. 순수 핵심 함수의 키 매개변수는 모듈 내부 시험 범위일 뿐 운영 승인 수단이 아니다.

정확 JSON 스키마와 중복/미지 필드 거절, 대문자 SHA 64자리와 nonce 32자리, 표준 Base64 384바이트 서명, 현재 문맥, UTC 시각과 절대 계획 경로, 계획 원시 SHA, ASCII/LF payload 및 RSA-3072/SHA-256/PKCS1 검증을 구현한다. 알려진 설계의 필드와 순서를 그대로 사용한다. JSON 검수는 재직렬화 전 중복 키를 검출해야 한다. 공개키 modulus 384바이트·exponent AQAB와 공개키 지문 형식도 검사한다. 키 생성은 격리 시험의 단명 RSA 객체만 허용하고 개인키를 내보내거나 기록하지 않으며 finally에서 폐기한다. 실사용 서명 키는 생성하지 않는다.

독립적인 격리 시험에는 합성 정상, 계획/계약/권위/서명 각각 변조, 다른 키, 과거 문맥, 누락/중복/미지 필드, 비정규 Base64, 잘못된 길이, 잘못된 시각/경로와 caller SHA만 일치하는 운영 호출 거절이 포함된다. 조작 입력이 서명 검사 이전 형식 오류로 거절됐는지 실제 이유도 기록한다. 실제 PowerShell/.NET 버전과 키 제공자 타입, 소스 SHA·각 시험의 관측 결과를 적고 정적 추측으로 실행 성공을 주장하지 않는다. 결과의 `DependencyClosure`, `LunaReviewed`, `WholeAccepted`, `PublicationApproved`는 모두 false이다. 기존 원문/534후보/캐시/역사증거/Git/원격은 불변이다. 후속 실제 키 동결과 실행 연결은 별도 Approved 개정이 필요하다.
