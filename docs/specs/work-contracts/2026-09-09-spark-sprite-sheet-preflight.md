# SPARK-01 — 스프라이트 시트 규격·프레임 사전검사 도구

- Status: Approved — bounded tool implementation only.
- Approved by: Astra, 2026-09-09. 사용자 요청: Spark가 구현할 부분을 분리해 파일로 인계.
- 구현: 사용자가 직접 Spark로 지정한 별도 작업. 이 계약에 한해 기존 Terra 구현 담당의 예외이며 다른 코어 소유권은 변경하지 않는다.
- 독립 검토: Luna 또는 별도 검증 작업. 최종 수용: Astra. 구현자는 자기 결과를 Verified로 승인하지 않는다.
- 현재 상태: 명세만 작성. Spark 실행·구현·검증은 아직 없음.

## Spark에게 전달할 지시

이 파일에 명시된 SPARK-01만 구현하라. 실제 게임용 스프라이트는 다른 작업에서 제작 중이므로 기다리지 말고 자체 테스트 입력으로 도구를 완성하라. 실 에셋이 준비되지 않은 것은 이 작업의 차단 사유가 아니다. 게임 코어·Unity·관계/엔딩/저장 스펙을 구현하거나 기존 파일을 정리하지 마라. 네트워크·외부 모델·추가 에이전트·설치가 필요하지 않다. 아래 허용 파일에만 작성하고 자체 테스트 증적과 짧은 인계 보고를 제출하라. 최종 독립 감사/실 에셋 검사/Unity 임포트는 다른 작업이 수행한다.

## 최소 읽기 범위

저장소 AGENTS.md, docs/README.md, CONTEXT.md, docs/agent-operating-model.md의 현재 지침과 이 계약을 읽는다. 관련 결정은 [ADR-0031](../../adr/0031-bounded-external-audit-and-spark-preference.md), 상위 미술 경계는 [VD-08](../vertical-demo/08-art-and-asset-integration.md)다. 다른 서사 제안서/전체 대화/에셋 원본은 읽을 필요 없다. 기존 qa/tools/Test-QaCatalog.ps1은 PowerShell 스타일 참고만 가능하며 수정하지 않는다.

## 목적과 비범위

향후 인계받을 PNG 스프라이트 시트의 **헤더상 크기, 고정 격자 분할 가능 여부, 애니메이션 프레임 인덱스**를 검사한다. 결과는 사람이 읽는 요약과 JSON이다. 원본은 읽기 전용이다.

이 도구는 PNG 전체 디코딩/CRC·IDAT 무결성/투명도·팔레트·실루엣/애니메이션 자연스러움/라이선스/Unity 임포트 설정/게임 준비 완료를 검증하지 않는다. 통과는 HeaderAndGrid 범위만 의미한다. 검사를 통과했다고 에셋 등록부 상태를 Approved/Imported로 바꾸지 않는다. 불규칙 아틀라스·여백·프레임별 다른 크기·pivot·hitbox·실제 Animator 연결은 범위 밖이다.

## 환경과 허용 파일

PowerShell 7 사용(이 작업에서 7.6.5 확인). 기본 .NET 라이브러리만, Unity/Python/Node/이미지 라이브러리 설치 불필요. 원본 PNG를 변환/분할/재저장하지 않는다.

새로 작성할 수 있는 파일:

1. qa/tools/sprite-preflight/Test-SpriteSheetPreflight.ps1 — 사용자 실행 진입점과 검사 함수.
2. qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1 — 별도 프로세스 기반 자가 테스트.
3. qa/tools/sprite-preflight/README.md — 사용법/지원 범위/출력 예시.
4. qa/tools/sprite-preflight/example.manifest.json — 아래 예제와 같은 설명용 데이터. 실제 이미지 경로 없음.
5. docs/verification/2026-09-09-spark-sprite-preflight-handoff.md — 실제 구현/AC별 자체 테스트 결과, 명령, 남은 문제.

테스트 출력은 새 고유 임시 디렉터리만 사용하며 경로를 보고한다. 자동 재귀 삭제는 하지 않는다. 입력 PNG·manifest의 실행 전후 SHA-256을 테스트에서 비교한다. 기존 허용 경로에 파일이 이미 있으면 읽고 소유권/변경을 확인해 충돌 시 덮어쓰지 말고 보고한다.

금지: Assets/, ProjectSettings/, Packages/, images/, third_party/, 다른 qa 도구·카탈로그, 기존 문서·AGENTS.md 수정. git commit/push/reset/clean, 원격 게시, 다른 작업 조작 금지. 계약 오류를 발견하면 가장 작은 재현과 필요한 결정을 보고하며 조용히 범위를 확대하지 않는다.

## 입력 계약

호출 형식:

```powershell
pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.ps1 -SheetPath <PNG파일> -ManifestPath <JSON파일> -ReportPath <새JSON결과파일>
pwsh -NoProfile -File qa/tools/sprite-preflight/Test-SpriteSheetPreflight.SelfTest.ps1
```

세 경로는 필수이며 와일드카드로 확장하지 않는다. SheetPath/ManifestPath는 호출자가 직접 지정한 로컬 일반 파일이고 내부 JSON에서 파일을 찾지 않는다. 폴더 탐색/URL 요청/UNC 네트워크 경로/심볼릭 링크 및 reparse point 입력은 지원하지 않으며 거절한다. 입력 경로의 부모도 reparse point 여부를 검사한다. ReportPath는 입력과 달라야 하며 부모가 존재하는 로컬 일반 디렉터리여야 한다. 부모 경로 reparse point/UNC도 거절한다. 기존 파일을 덮어쓰지 않으며 FileMode.CreateNew 등으로 경합 시에도 보호한다. 부모 폴더는 자동 생성하지 않는다.

manifest v1(UTF-8 JSON):

```json
{
  "schemaVersion": 1,
  "pixelsPerUnit": 18,
  "frameWidth": 32,
  "frameHeight": 48,
  "animations": [
    { "id": "idle", "frames": [0, 1] },
    { "id": "walk", "frames": [2, 3, 2] }
  ]
}
```

未知/추가 필드는 무시하지 않고 INVALID_MANIFEST로 거절한다. 객체 키는 위 철자를 정확히 따르고 중복 JSON 속성도 거절한다(System.Text.Json 순회 등 기본 라이브러리로 검사). 문자열 숫자/boolean/null을 숫자로 강제 변환하지 않는다. schemaVersion과 PPU는 각각 숫자 정수1/18, frameWidth/Height는 숫자 정수1..32768이다. 소수점 표현은 값이 정수인 경우 허용하며 문자열은 불허한다. animations는 1..128개 배열, 각 id는 ^[a-z][a-z0-9_-]{0,31}$이며 중복 불가. frames는 1..4096개 숫자 정수 배열이다. 한 애니메이션에서 같은 프레임 반복은 합법적이며 순서를 보존한다. manifest 크기는 최대1 MiB, JSON 최대 깊이16으로 제한한다.

## 요구사항

- REQ-SPRPF-001: 입력/출력 경로 경계와 읽기 전용 원본을 지킨다. 네트워크/Unity/임포트/이미지 변환 없음. 설정/환경/등록부를 수정하지 않는다.
- REQ-SPRPF-002: manifest를 위 계약 그대로 검증하며 필드 누락·추가·중복 키·잘못된 타입/범위·중복 animation id를 명확히 거절한다. 형식 오류를 암묵적으로 보정하지 않는다.
- REQ-SPRPF-003: PNG는 스트림으로 앞33바이트를 읽는다. 8바이트 PNG signature, 첫 chunk의 big-endian length=13, type=IHDR, width/height 각각1..32768을 확인한다. 파일 길이가33 미만이면 거절. CRC 등 나머지 PNG 무결성 검사는 하지 않으며 보고서에도 그 한계를 명시한다. 큰 파일 전체를 메모리에 로드하지 않는다. 해시는 스트리밍 SHA-256이다.
- REQ-SPRPF-004: sheetWidth % frameWidth == 0 및 sheetHeight % frameHeight == 0이어야 한다. columns=width/frameWidth, rows=height/frameHeight, frameCount=columns*rows. 충분한 정수 범위로 계산한다. 인덱스는 좌상단0에서 행 우선, 0<=index<frameCount. 음수/소수 인덱스 거절, 모든 프레임 사용 의무는 없다. 32x48 프레임도 합법적이며 18 PPU를 ‘프레임은18픽셀 배수’로 오해하지 않는다.
- REQ-SPRPF-005: 기계용 결과는 schemaVersion1, scope="HeaderAndGrid", passed(boolean), checksNotPerformed=["ImageDecode","PixelArtQuality","License","UnityImport","Gameplay"], sheetWidth/Height, columns/rows/frameCount, sheetSha256/manifestSha256, animations(입력 순서), errors(배열)로 고정한다. 계산 불가 값은 null. SHA-256은 소문자64자리. 각 error는 code/message 문자열. 오류 없음은 []이고 passed=true. 시각·무작위ID·절대경로는 JSON에 넣지 않는다. 입력 동일하면 JSON 바이트도 동일해야 한다(UTF-8 BOM 없음, 순서 고정, 개행LF). 출력 경로 자체의 문제로 파일을 만들 수 없으면 stderr에 오류 코드, 기존 파일 보존.
- REQ-SPRPF-006: 종료코드0=범위 내 검사 통과, 1=입력 내용 검증 실패, 2=경로/읽기/출력 I/O 또는 호출 실패. 0을 PNG 전체 무결성이나 게임 사용 승인으로 표현하지 않는다. 실패는 예외 stack 대신 짧은 코드/사유. 불필요한 진행 로그 없음. stdout은 요약, JSON은 ReportPath.
- REQ-SPRPF-007: SelfTest는 성공/음성/경계/안전 검사를 독립된 도구 프로세스로 실행하고 예상 종료코드·JSON·파일 보존·실행 결과를 검증한다. 통과 건수와 실패 목록, 증거 디렉터리를 출력하며 하나라도 실패하면 nonzero. 테스트 환경 문제도 PASS로 처리하지 않는다.

오류 코드: INVALID_MANIFEST(내용/타입), UNSUPPORTED_VERSION(schemaVersion!=1), INVALID_PNG_HEADER, NON_DIVISIBLE_GRID, FRAME_OUT_OF_RANGE, INVALID_PATH, INPUT_IO_ERROR, REPORT_IO_ERROR, INTERNAL_ERROR. 단일 실패를 우선순위 경로→manifest→PNG→격자→프레임으로 반환해도 된다. 같은 단계에서 여러 오류를 반환하면 입력 순서 등 결정론적 순서를 유지한다. 종료코드2 사유와 내용 오류를 혼동하지 않는다. 미존재/잘못된 경로는 INVALID_PATH(2), 읽기 권한 실패는 INPUT_IO_ERROR(2), 예상 못한 내부 예외는 INTERNAL_ERROR(2)다.

## 인수 기준

| AC | REQ | 필수 검사와 기대 결과 |
|---|---|---|
| AC-SPRPF-001 | 001/003/004/005/006 | 64x96 헤더와 예제 manifest → 2열2행4프레임, 통과0. 32x48 프레임 허용. 원본 해시 불변 |
| AC-SPRPF-002 | 002/006 | JSON문법 오류, 필드 누락/추가/중복 키, 문자열 숫자, null, 0/음수 frame 크기, PPU!=18, 중복 id →1 및 INVALID_MANIFEST; 버전2→1/UNSUPPORTED_VERSION |
| AC-SPRPF-003 | 003/006 | PNG signature 오류,33바이트 미만, IHDR length/type 오류, 0 또는32769 크기 →1/INVALID_PNG_HEADER. 경계32768은 헤더 범위 내 허용 |
| AC-SPRPF-004 | 004/006 | 65x96/32x48→1/NON_DIVISIBLE_GRID. 4프레임에서0/3유효, -1/4불가. 소수 index 불가. 반복[2,3,2]와 미사용 프레임 허용 |
| AC-SPRPF-005 | 005/006 | 같은 입력으로 서로 다른 새 결과 경로 두 번 실행→JSON바이트/해시 동일. checksNotPerformed와 scope 명시. 헤더 뒤 픽셀 데이터 손상은 본 검사 범위 밖임을 README와 fixture로 보여 줌 |
| AC-SPRPF-006 | 001/006 | 기존 결과 파일, 결과=입력, 미존재/UNC/링크 경로 거절→2, 원본/기존 결과 불변. 공백·한글·대괄호가 포함된 합법 로컬 경로는 literal로 처리. 링크 테스트 생성 권한이 없으면 그 항목 미검증으로 보고하고 전체 검증 완료 주장 금지 |
| AC-SPRPF-007 | 007 | SelfTest가 프로세스 종료코드와 결과를 실제 assert. 테스트를 의도적으로 실패시킨 확인(임시 복사/주입 등)에서 nonzero임을 증명. 예외를 잡아 성공으로 바꾸지 않음 |

## 테스트 자료와 결과 인계

테스트는 자체 생성한 최소 헤더 fixture를 쓴다. 이는 완전한 PNG가 아니라 헤더 검사 전용 자료임을 파일명/보고에 명시한다. 실제 아트 파일의 수정을 피한다. 자체 테스트용 데이터를 생성하는 코드의 파일 쓰기는 도구의 정상 테스트 출력이며 원본 미디어 편집이 아니다. 셀 크기·행/열·인덱스 오류는 이미지 생성 AI나 Unity 없이 재현 가능하다.

인계 파일에는 (1) 수정 파일, (2) REQ별 구현 요약, (3) 실행한 정확한 명령/PowerShell 버전, (4) AC별 PASS/FAIL/미실행과 실제 건수, (5) 남긴 임시 증거 경로, (6) 검증하지 않은 범위, (7) 다음 검토자에게 필요한 사항을 담는다. ‘구현 완료/독립 검토 대기’까지만 보고하며 Verified 전환·실 에셋 승인·전체 QA 완료를 선언하지 않는다.

rollback: 이 작업의 신규 파일만 격리/되돌릴 수 있어야 하며 기존 파일과 공유 설정을 수정하지 않아 rollback에 영향이 없어야 한다. 실제 되돌리기/삭제는 여기서 수행하지 않는다. 변경 증적은 범위 내 diff와 해시로 남긴다.

## 완료 후 짧은 답변 형식

구현한 도구 위치 / 실행 예시 / 자체 테스트 결과 / 미검증 사항 / 인계 보고 위치만 요약한다. 전체 소스나 장문의 프로젝트 설명을 채팅에 다시 붙이지 않는다. 다른 기능을 추가하지 않고 독립 검토를 기다린다.
