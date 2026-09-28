# SPARK-01-R2 — 정확한 숫자·테스트 실패·입력 상한 수정

- Status: Verified, Astra 2026-09-10. Terra recovery 구현과 Luna 독립 검증 통과. [최종 검증](../../verification/2026-09-10-spark01-r2-terra-recovery-luna-review.md).
- 구현 Spark 수동 전달, 독립 검토 Luna/Astra, 최종 수용 Astra.
- 먼저 [R1 독립 검토](../../verification/2026-09-09-spark01-r1-independent-review.md)를 읽는다. 기존 SPARK-01/R1의 범위·요구사항·AC 유지.

## 수정 범위

qa/tools/sprite-preflight/Test-SpriteSheetPreflight.ps1, Test-SpriteSheetPreflight.SelfTest.ps1, README.md 및 신규 docs/verification/2026-09-09-spark01-r2-handoff.md만 수정한다. 예전 보고서는 보존. 독립 재현 스크립트/계약 수정, Unity/원본 미디어/다른 기능 변경 금지. 외주·설치·추가 에이전트 불필요.

## 세 수정과 확인 기준

1. REQ-SPRPF-R2-001 / AC-SPRPF-R2-001 → 기존 R1-001: double/float/decimal 변환에서 반올림된 값으로 정수성을 결정하지 않는다. JSON 숫자 원문에서 부호·가수 숫자·소수 자리·지수를 해석해 정확히 정수이고 Int32 범위일 때만 허용한다. 지수만큼 거대한 문자열/BigInteger를 할당하지 않는다. 32/32.0/3.2e1/1e0/0e-400은 합법, 32.0000000000000001/1e-400/0.99999999999999999999/Int32 초과는 해당 내용 오류. 프레임/PPU 등 필드의 추가 범위도 적용한다. 각 사례를 독립 assert하고 숫자 재현2건 모두 종료1인지 확인한다.
2. REQ-SPRPF-R2-002 / AC-SPRPF-R2-002 → 기존 R1-005: 링크 생성만 별도 try/catch, 실제 도구 호출/assert 실패는 FAIL로 전파한다. 필수 미검증/SKIPPED>0이면 전체 nonzero, PASS 전체 완료로 표현 금지. 정상 전체0, 기존 -InjectAssertionFailure는nonzero, 링크 assert 실패 및 링크 생성 불가를 각각 테스트 전용 임시 복사/주입으로 재현해 nonzero를 입증한다. 작업계약상 환경 skip과 실제 assert 실패를 보고에서 구별한다. 최종 실패 경로는 증거 위치·누적 PASS/FAIL/SKIP을 출력하고 정확한 종료값으로 끝낸다.
3. REQ-SPRPF-R2-003 / AC-SPRPF-R2-003 → 기존 R1 상한 읽기: manifest 파일을 전체 ReadAllBytes로 읽기 전에 크기를 제한한다. 스트림에서 최대1MiB+1만 읽는 등 성장/경합에도 제한 메모리를 보장한다. 1MiB 이하/정확히1MiB의 유효 JSON(공백 패딩 가능)은 허용,1MiB+1은 INVALID_MANIFEST/1. 큰 입력 전체를 로드하지 않는 것을 코드와 테스트로 증명한다. 대규모 OOM 실험은 불필요.

테스트용 실패 주입은 SelfTest 내부 정상 검증 경로를 실제 실패시켜야 하며 단순 스위치→exit1은 검증으로 인정하지 않는다. 제품 도구에 실패 주입 플래그 추가 금지.

## 완료 확인

기존 SelfTest, qa/reviews/2026-09-09-spark01-repro.ps1(8건), qa/reviews/2026-09-09-spark01-r1-numeric-repro.ps1(2건)을 실행한다. 독립 스크립트의 기대값을 변경하지 않는다. 위3개 새AC와 원 계약의 미검증 사례를 각각 표시하고 실행하지 않은 항목은 그대로 미검증이라고 보고한다.

인계에 실제 명령·PowerShell버전·실행 결과·증거 위치·세 수정 코드 위치·수정 전후 SHA256(64자리, 알고리즘 명시)을 남긴다. Git 미추적 파일도 hash 계산 가능하다. 이번 수정 전 해시는 R1 독립 보고에서 대조한다. 구현 완료/독립 검토 대기까지만 선언한다. rollback은 이 작업의 허용 파일 변경만 대상이며 실제 삭제/되돌리기는 수행하지 않는다.
