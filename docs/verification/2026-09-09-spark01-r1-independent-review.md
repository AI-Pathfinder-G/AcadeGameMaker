# SPARK-01-R1 독립 재검토

- 판정: Changes required / 미수용. 2026-09-09 Astra 실행 검토, Luna 독립 정적 검토.
- 원 도구/자체테스트 수정 없음. 신규 독립 재현 자료만 작성.

## 확인된 개선

- 자체테스트 재실행: PASS26/SKIPPED0, 종료0. 증거 C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-6c932917aa764beb8bcb77480ffd4404.
- 기존 독립 반례8건: 기대 종료코드8/8 일치, 원본 해시 불변. 증거 C:/Users/me/AppData/Local/Temp/spark01-independent-7a440a9328f24a3b83ac97d9854cac17.
- 정상 JSON의 animations 배열/errors=[] 직접 확인. 중첩 오류/후행 JSON/32.0/256·32768 헤더의 기존 재현은 개선됐다.
- -InjectAssertionFailure 실행: 실제 assert 오류로 종료1 확인. 다만 오류 처리의 Write-Error가 종료하여 뒤쪽 증거경로 출력은 누락된다.

## 남은 수정 사항

1. P1 / AC-SPRPF-R1-001: TryGetDouble→Math.Truncate(도구489~522행)는 비정수를 반올림한다. 별도 [숫자 재현](../../qa/reviews/2026-09-09-spark01-r1-numeric-repro.ps1)에서 frameWidth=32.0000000000000001, frame index=1e-400 모두 예상 종료1이나 실제0/통과. 증거 C:/Users/me/AppData/Local/Temp/spark01-r1-numeric-3d2be3b1ebb24156b2576df30e17c1e6/findings.json. 숫자 원문을 정확하게 판정해야 한다.
2. P1 / AC-SPRPF-R1-005: SelfTest419~461행의 try가 링크 생성과 실제 assert를 함께 감싼다. 검사 불일치도 SKIPPED가 되고476행은 조건 없이 exit0. 이번 정상 환경에서는 두 링크 검사가 통과했지만 실패 경로의 코드는 원 계약과 다르다. 이번에는 실패 링크 환경을 실제 주입하지 않았고 정적으로 확인했다.
3. P1 / REQ-SPRPF-R1-005의 제한 읽기: 도구835행 ReadAllBytes 후852행에서1MiB를 검사한다. 크기 확인/제한 읽기 요구 미반영, 해당 테스트도 없다. 메모리 소진을 실제 유발하지 않고 코드로 확인했다.

Luna의 별도 읽기 전용 검토도 위3건을 확인했다. 자체 PASS26과 기존8반례 통과를 전체 AC 통과로 인정하지 않는다. 입력 판정·테스트 하네스·상한 읽기가 해결되기 전 최종 수용 보류.

인계 증적 보완: 기존 보고서의 변경 전 hash N/A는 부정확하다. git 미추적 여부와 파일 hash 계산은 별개이며 최초 독립 보고에 SHA256이 남아 있다. 새 보고에는 알고리즘과 실제64자리 SHA256을 적어야 한다. 이번 검토 SHA256: 도구 A81D36CC2D2A82283240F5101D2686451A5B575D4FB71B1F097DD60D35BED4BC, SelfTest E11F80C9F61E8F42260107CCC3FF50CA0C2FF42FF974AB21C2D769DB0B737DF8.

기준 [R1 계약](../specs/work-contracts/2026-09-09-spark01-r1-correctness-fixes.md). 실제 게임/PNG 디코딩/임포트 검증 없음.
