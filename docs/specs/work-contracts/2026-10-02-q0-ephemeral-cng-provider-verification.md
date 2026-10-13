# Q0 비영속 서명 제공자 실제 검증

상태: **Approved**, 2026-10-02 통합 책임자 승인. `REQ-PLAT-069/070`, `AC-PLAT-067/068` 중 설치 제공자의 비영속·비내보내기와 서명 호환 부분만 대상으로 한다. 외부 권위 발급 초안 SHA `64C2011B4C1D1C348EBCEDDE4A468DC6D5C59E5126880CC7622DD66F4E807E9D`의 정적 API를 실제 실행으로 확인한다. 실사용 권위/키/서명 발급이나 원격 게시를 허용하지 않는다.

테라 `gpt-6-sol` 허용 새 출력은 `qa/tools/Test-Q0EphemeralCngKey.ps1`, `artifacts/c4-q0-ephemeral-cng-provider-tests-v1.json`, `docs/verification/2026-10-02-q0-ephemeral-cng-provider-terra-report.md` 세 파일이다. 단명 격리 CNG RSA3072 키를 이름 없이 생성하고 `ExportPolicy=None`, Length3072를 지정한다. 실제 `IsEphemeral`, `KeySize`, export policy, 제공자 타입을 검사한다. 공개 매개변수만 정상 추출하며 개인키 내보내기는 예상 거절 시험으로 한 번 시도하되 반환되면 민감 값을 출력하거나 기록하지 않고 즉시 실패·키 폐기한다. private 배열을 직렬화하거나 로그/파일/환경/명령행/메시지에 쓰는 코드는 금지한다.

고정 합성 payload의 SHA256/PKCS1 서명과 공개키 독립 검증, 변조 payload 거절, disposal 이후 사용 거절을 확인한다. finally에서 RSA와 CNG 객체를 폐기하고 예외도 정리한다. 실사용 계획/실사용 키와 연결하지 않는다. 시험 결과는 실제 관측과 실패 이유만 기록하고 원래 핵심 도구·15개 증거는 보존한다. 보호 원문534/후보534/505·캐시·설정·Git·원격은 불변이다. 제공자 검증 성공은 권위 루트 P1이나 전체 게시 승인을 닫지 않는다. 독립 루나 수용은 별도이다.
