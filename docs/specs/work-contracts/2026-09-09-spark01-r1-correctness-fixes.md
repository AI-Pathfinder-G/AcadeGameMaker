# SPARK-01-R1 — 입력 판정·PNG 크기·테스트 보완

- Status: Approved — Astra, 2026-09-09. 사용자 요청의 다음 Spark 작업.
- 구현: 사용자가 직접 Spark에 전달. 독립 검토 Luna/Astra, 최종 수용 Astra.
- 현재 상태: 명세 작성, 수정 구현 전. 기존 SPARK-01 미수용.
- 선행 명세: [SPARK-01](./2026-09-09-spark-sprite-sheet-preflight.md)의 REQ/AC 전부 유지.
- 발견/재현: [독립 검토 보고](../../verification/2026-09-09-spark01-independent-review.md), qa/reviews/2026-09-09-spark01-repro.ps1.

## 전달 지시

SPARK-01-R1만 구현한다. 기존 기능 추가 없이 독립 검토의 F01~F07을 수정하고 원 계약의 누락 테스트를 완성한다. AGENTS.md와 원 계약의 읽기/환경/금지 범위를 따른다. 두 번째 도구를 새로 만들지 않고 기존 구현을 수정한다. 이전 자체 PASS13은 원 계약 전체 통과 근거가 아니므로 그대로 반복 보고하지 않는다. 실 에셋이나 Unity는 필요 없다.

## 수정 허용 범위

- qa/tools/sprite-preflight/Test-SpriteSheetPreflight.ps1
- qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1
- qa/tools/sprite-preflight/README.md
- docs/verification/2026-09-09-spark01-r1-handoff.md (신규)

원 계약·독립 검토·독립 재현 스크립트·옛 인계 보고·그 외 파일 수정 금지. 루트가 추가한 독립 검사를 약화하거나 삭제하지 않는다. 기존 무관 변경 보존, git commit/push/clean/reset·Unity·외주·설치 금지. 테스트는 새로운 고유 임시 디렉터리만 사용하며 자동 삭제하지 않는다. 이 작업의 수정 전 파일 해시를 인계에 남긴다. 되돌릴 필요가 생기면 이 네 파일의 이번 변경만 대상으로 하며 공유 작업을 덮어쓰지 않는다.

## 수정 요구와 인수 기준

| 새 REQ | 원 REQ / AC | 해야 할 일 | 새 AC |
|---|---|---|---|
| REQ-SPRPF-R1-001 | 002 / AC002 | 하위 객체 오류 통합, EOF 검사, 수학적으로 정수인 숫자 허용. 소수/문자열/누락/추가/중복은 거절 | AC-SPRPF-R1-001: 독립 재현의 nested-extra/bad-frame-type/duplicate-nested-key/trailing-json은1, integer-decimal은0. 개별 오류 fixture 추가 |
| REQ-SPRPF-R1-002 | 003 / AC003 | IHDR length/width/height big-endian 정확성. uint32 범위를 먼저 읽고 명세 범위로 검증하여 상위비트 값도 헤더 오류1로 처리 | AC-SPRPF-R1-002: width256/32768 정상0. width/height 각각0/32769/0xffffffff는 헤더 오류1. IHDR length257/269 등 상위바이트가 있는 잘못된 길이는 거절 |
| REQ-SPRPF-R1-003 | 005 / AC005 | animations/errors의 JSON 배열 타입을 원소수와 무관하게 보존. unavailable 해시는 null(빈 문자열 아님). 동일 입력 결과바이트 결정론 유지 | AC-SPRPF-R1-003: 정상 errors=[], 단일 animation도 배열, 다중 오류도 배열. 손상 manifest의 계산 불가 필드 null. BOM없음/동일바이트 직접 검사 |
| REQ-SPRPF-R1-004 | 001 / AC006 | 경로 조상 전체 reparse 검사. 모든 I/O 오류를 계약 코드2로 정리, 보고에 절대경로 삽입 금지. 읽기/출력 실패에서 stack 유출 없음 | AC-SPRPF-R1-004: 바로 아래 및 두 단계 아래 junction 경로 거절, 기존 입력/출력 불변. 실제 검사 미실행은 PASS 금지 |
| REQ-SPRPF-R1-005 | 007 / AC007 및 전체 | 오류 하나당 fixture로 각 필수 기대를 분리. 여러 오류를 한 JSON에 넣고 하나 잡혔다고 전체 통과 처리 금지. 링크 생성 실패와 assert 실패의 catch 분리. 필수 skip/환경 미검증 시 전체nonzero, 명시적 미검증 보고 | AC-SPRPF-R1-005: 정상 SelfTest 전체통과0, 고의 assert 실패 주입 시nonzero. 도구가 음성입력에서nonzero인 것만으로 하네스 실패 검증을 대체하지 않음 |

32.0뿐 아니라1e0처럼 값이 정수인 유효 JSON 숫자도 원 계약상 허용한다. 문자열"32"는 거절한다. 부동소수점 반올림으로 실제 소수를 정수로 바꾸지 않는다. manifest는 크기 제한을 확인해 제한된 바이트만 읽고, 파서 내부 예외는 INTERNAL_ERROR/2로 분리한다. 알려진 잘못된 입력은 내용 오류1이다. 경로 검사 실패 시 출력할 JSON에 실제 로컬 경로를 넣지 않는다. 사람이 읽는 stderr에는 역할/코드 위주로 요약한다.

고의 SelfTest 실패를 검증할 수 있도록 selftest에 테스트 전용 -InjectAssertionFailure 스위치를 추가해도 된다. 이 스위치는 실제 검증 assert를 실패시키고 정상 실행 경로와 동일한 실패 처리를 거쳐야 한다. 단순 지정 시 exit1만 하는 가짜 시험은 안 된다. 운영 도구에 실패 주입 스위치를 추가하지 않는다.

## 실행과 완료 보고

```powershell
pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1
pwsh -NoProfile -File qa/reviews/2026-09-09-spark01-repro.ps1
```

두 번째 명령은 독립 재현8건 모두 기대코드와 일치해야 종료0이다. 이8건 통과만으로 모든 AC 완료를 선언하지 않는다. 원 SPARK-01 AC001~007의 모든 필수 사례와 R1 AC001~005를 대응시켜 실제 실행/미실행을 보고한다.

인계 파일에는 수정 전후 해시, F01~F07 해결 위치, 개별 사례와 기대/실제, selftest 고의 실패의 실제 종료코드, 원본 보존/출력타입/경계 결과, 정확한 실행 명령·버전·임시증거경로를 남긴다. 실제 수행하지 않은 검사에 PASS를 붙이지 않는다. 완료 후 수정파일과 새 인계 보고 경로만 짧게 전달하고 독립 검토 대기한다.
