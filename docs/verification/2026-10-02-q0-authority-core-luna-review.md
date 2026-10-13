# Q0 서명 검증 핵심 독립 검수

대상 Approved 계약 `docs/specs/work-contracts/2026-10-02-q0-authority-core-independent-review.md`; 구현 계약 `docs/specs/work-contracts/2026-10-02-q0-signed-authority-core-implementation.md`. 운영 진입점, 핵심 모듈, 자기시험 소스 지문은 각각 `A35D7AE6E21FFFB20F80837D201EF338BD13486A2383BDF3F9873928598795A7`, `CC589ADCDD1AE072ACF7E00165BCDFC8A3DADFF150E6D75A5B3084D816982738`, `9A030B9B48BD6B536E929E89088074D76D65855906BABAEAAE736FACC763F876`로 승인 입력과 일치했다.

## 판정

- **AC-PLAT-068 핵심 범위: 충족, P0 0 / P1 0.** 세 소스를 바이트 동일하게 `output/q0-authority-luna-r1`에 복사해 격리 자기시험을 실행했다. 15/15 통과, 종료 0; 결과 SHA-256 `EBE9061C0A0152D06A132A90D7CA064B6F7810ECC4C9FBBF017EC2DA83EC6FFB`로 기존 원본과 동일했다. 기존 소스·원본 시험 결과·테라 보고는 수정하거나 덮어쓰지 않았고 원래 다섯 입력 지문도 그대로였다.
- 정상 합성, 계획/계약/권위/서명 변조, 다른 키, 과거 문맥, 누락/중복/미지 필드, 비정규 Base64·길이, 잘못된 시간·경로를 각각 실제 거절 이유와 대조했다. caller 계획 SHA만 맞춘 운영 호출은 `exit=2`로 거부됐다. 격리 별도 입력의 잘못된 UTF-8도 엄격 디코더에서 JSON 파싱 전에 거부됐다(실제 관측은 .NET UTF-8 디코더 예외).
- **외부 권위 P1은 열린 상태다.** 이 시험은 합성 키만 사용했다. 실제 고정 공개키·외부 인증 승인 루트·현재 Authority SHA/개정이 발급되지 않았고 운영 스크립트는 그 부재를 명시하며 거절한다. 따라서 AC-PLAT-066/067의 실사용 권위·키 동결·실제 계획 서명·비순환 동결은 수용하지 않는다. 합성 정상이나 15/15 결과를 운영 승인으로 승격하지 않는다.
- 전체 의존 폐쇄·게시 승인·전체 수용 플래그는 false로 유지한다. Unity·네트워크·Git·제품/QA 실행은 하지 않았다.
