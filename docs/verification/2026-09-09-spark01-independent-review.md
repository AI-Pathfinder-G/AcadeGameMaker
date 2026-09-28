# SPARK-01 독립 검토 — 수정 후 재검토 필요

- 2026-09-09. 실행/수용 판정 Astra, 독립 정적 코드 검토 Luna.
- 판정: Changes required, 미수용. Spark의 자체 AC 전부 PASS 보고는 아래 범위에서 재현되지 않는다. 게임/실 에셋/Unity 검증은 수행하지 않았다.
- 기준: [SPARK-01](../specs/work-contracts/2026-09-09-spark-sprite-sheet-preflight.md).

## 실행 증거

기존 SelfTest 재실행은 종료0, PASS13/SKIPPED0이었다. 증거: C:/Users/me/AppData/Local/Temp/sprite-preflight-selftest-35675e0e0b084feda0b2109354d49609.

Astra의 별도 [재현 스크립트](../../qa/reviews/2026-09-09-spark01-repro.ps1)는 8건의 기대 종료코드 중7건 불일치, 입력 원본 해시 변경0건. 실제 도구 프로세스를 호출했다. 증거: C:/Users/me/AppData/Local/Temp/spark01-independent-eb21aadf685d479aadcbe5d1cd9814e8/findings.json. 이 디렉터리는 삭제하지 않았다.

| 사례 | 기대→실제 종료 | 결과 |
|---|---|---|
| 정상64x96 | 0→0 | 종료코드 일치, 아래 출력 스키마 결함 별도 |
| animation 추가 필드 | 1→0 | 잘못된 데이터 통과 |
| frames=[0,"bad"] | 1→0 | 잘못된 프레임이 제거되고 통과 |
| animation id 키 중복 | 1→0 | 중복 키 통과 |
| 정상 JSON 뒤 {} 추가 | 1→0 | 후행 JSON 통과 |
| frameWidth=32.0 | 0→1 | 명세가 허용한 정수값 표기 거절 |
| PNG width=256 | 0→1 | 정상 헤더 거절 |
| PNG width=32768 | 0→1 | 허용 상한 헤더 거절 |

추가 관찰: 정상 결과에서 animations는 단일 객체, errors는 null이다. 계약은 각각 배열과 []를 요구하므로 AC-SPRPF-005 미충족.

## 수정 요구

- F01 / P1 / AC002: ParseAnimation의 state.Issues가 ParseManifest 최종 issues로 합쳐지지 않는다(도구103,120,311~419행). 루트와 하위 객체의 모든 오류를 판정에 반영할 것.
- F02 / P1 / AC002: 루트 객체가 닫힌 뒤 reader EOF를 확인하지 않는다. JSON 전체 입력을 소비하고 후행 값/토큰을 거절할 것.
- F03 / P1 / AC002: TryGetInt32만 사용해32.0을 거절한다(436행). 정수값인 숫자 표현을 허용하되 소수·문자열·범위 초과를 엄격히 구분할 것.
- F04 / P1 / AC003: byte 값의 shift/OR 조합이 상위 바이트를 정확히 복원하지 않는다(608,617~618행). 명시적 넓은 정수 연산/표준 big-endian 읽기로 수정할 것.
- F05 / P1 / AC005: PowerShell 컬렉션 열거로 배열이 객체/null로 바뀐다(Build-Result). 0/1/여러 원소에서 JSON 타입을 보존할 것.
- F06 / P1 / AC006: 정적 검토상 직계 부모만 reparse 검사한다(519행). 모든 조상을 검사해야 한다. 이 항목은 이번 반례 스크립트에서 실행하지 않았음.
- F07 / P1 / AC007: 필수 테스트가 대부분 빠져 있고, 링크 검사 예외를 SKIPPED로 처리한 뒤 exit0한다. 실제 assert 실패도 환경 skip으로 삼킬 수 있다. 링크 생성과 검증 실패를 분리하고 전체 테스트의 의도적 실패 시 nonzero를 검증할 것. 현재 missing-file 도구 호출은 테스트 하네스 실패 검증이 아니다.

Luna는 F01/F02를 P0로 제기했다. Astra는 데이터 검사기의 오판으로 인한 수용 차단 P1로 통일하며 수정 필요성은 수용했다. AC001은 해당64x96 계산만 확인, AC002/003/005 실패, AC004/006/007 전체 완료 근거 부족. 단위 사례 통과를 AC 전체 통과로 올리지 않는다.

검토 시 도구 SHA256 CF7C2E3839BC5C26CB716F7F4007FB784983229CAC9195AB6870D22A9CF5D54F; SelfTest SHA256 BB61A0AAF2E7A043053BBC042FFB535AAA3B753FCFF13FB6E462663ADD567926. 원 구현 파일은 수정하지 않았다.
