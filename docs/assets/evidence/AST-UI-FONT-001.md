# AST-UI-FONT-001 — Noto Sans CJK KR 2.004

- 확인일: 2026-09-23
- 검증자: Astra
- 사용 범위: VD-09 M5D7Q-A 허브 메뉴의 한국어 우선 정적 TextMeshPro SDF
- 상태: 소스·라이선스와 OTF/OFL 저장소 사본 검증 완료. 실제 1024x1024
  capacity 실패 뒤 2048x2048 정적 atlas 보정은 `Review`이며, Luna 재검토와
  Astra 승인 전에는 생성 구현 권한이 없다. Unity 생성물 해시는 구현 후 추가

## 공식 출처와 취득물

- 공식 프로젝트: <https://github.com/notofonts/noto-cjk/tree/main/Sans>
- 공식 릴리스 태그: <https://github.com/notofonts/noto-cjk/releases/tag/Sans2.004>
- 공식 배포 안내: <https://github.com/notofonts/noto-cjk/blob/main/Sans/README.md>
- 최종 다운로드 URL:
  <https://github.com/googlefonts/noto-cjk/releases/download/Sans2.004/07_NotoSansCJKkr.zip>
- 취득일: 2026-09-23
- 아카이브: `07_NotoSansCJKkr.zip`
- 아카이브 SHA-256:
  `E26FCF98E75176D24984875377AB921DBB46055B88ED4A39454D91D6146C5654`

공식 Sans README가 `Sans2.004`의 언어별 한국어 정적 OTF 묶음으로 위
다운로드를 지정한다. 임시 격리 폴더에서만 내려받아 검사했으며, 이 증적 작성
시점에는 폰트 바이너리를 저장소에 넣지 않았다.

## 선택 파일의 동일성

| 파일 | 바이트 | SHA-256 | 내부 family/subfamily | 내부 버전 | PostScript 이름 |
|---|---:|---|---|---|---|
| `NotoSansCJKkr-Regular.otf` | 16,433,112 | `6BCB2A0703AA137E874FC2DFFA85F6C21BA9A67FA329E81B8C801663AF7E992A` | `Noto Sans CJK KR` / `Regular` | `Version 2.004;hotconv 1.0.118;makeotfexe 2.5.65603` | `NotoSansCJKkr-Regular` |
| `NotoSansCJKkr-Bold.otf` | 16,997,996 | `26D0C6748500A0444844280B308F5B62C7AE92AC6C6AC88148E502DD211EB52A` | `Noto Sans CJK KR` / `Bold` | `Version 2.004;hotconv 1.0.118;makeotfexe 2.5.65603` | `NotoSansCJKkr-Bold` |
| 배포본 `LICENSE` | 4,301 | `6A73F9541C2DE74158C0E7CF6B0A58EF774F5A780BF191F2D7EC9CC53EFE2BF2` | SIL Open Font License 1.1 | 26 February 2007 | 해당 없음 |

OTF name table의 저작권 문자열은 `© 2014-2021 Adobe
(http://www.adobe.com/).`이다. 가변 폰트, 다른 지역 서브셋, 이동하는 latest
URL 또는 다른 버전의 파일로 대체할 수 없다.

## 라이선스 판단과 의무

배포본의 정확한 `LICENSE`는 SIL Open Font License 1.1이다. 원본과 수정본은
소프트웨어에 묶거나 포함하여 재배포할 수 있다. 다음 조건을 적용한다.

- 폰트 또는 개별 구성요소를 단독으로 판매하지 않는다.
- 배포하는 모든 복사본에 저작권 고지와 OFL 1.1 원문을 함께 제공한다.
- 폰트 및 파생 폰트는 OFL 1.1 아래에 유지한다.
- 수정본에 Reserved Font Name을 쓰지 않고, 권리자·저자 이름을 홍보나 보증에
  사용하지 않는다. 이번 단위는 원본 OTF를 수정하지 않는다.
- 생성된 SDF 폰트 자산과 그 내장 아틀라스는 원본 폰트에서 파생된 배포물로
  취급하고 OFL 원문을 프로젝트와 함께 보존한다.

프로젝트 도입 시 배포본의 바이트를 그대로
`Assets/UI/Fonts/Hub/OFL-1.1.txt`에 복사하고 해시 일치를 검증한다.

## 고정 글리프 집합

Regular와 Bold는 같은 138개 코드 포인트만 갖는 정적 집합으로 생성한다.

- `U+0020-U+007E`: 공백을 포함한 인쇄 가능한 ASCII 95자. 테스트, 계측 및
  닫힌 디버그 문자열에 필요한 Latin·숫자·구두점을 포함한다.
- `U+00D7`: 알림 닫기 기호 `×`.
- 아래 42개 한글 음절:

```text
U+AC8C U+ACBD U+ACC4 U+AD6C U+AE30 U+B2C8 U+B2E4 U+B7EC
U+B85C U+B8CC U+B97C U+B9CC U+BABB U+BCC0 U+BCF4 U+BCF5
U+BD88 U+C0AC U+C0C1 U+C0C8 U+C124 U+C18D U+C190 U+C2B5
U+C644 U+C654 U+C740 U+C744 U+C77C U+C784 U+C7A5 U+C800
U+C815 U+C874 U+C885 U+C9C0 U+D30C U+D504 U+D544 U+D558
U+D56D U+D588
```

사람이 읽을 수 있는 동일 집합은 `게경계구기니다러로료를만못변보복불사상새설속손습완왔은을일임장저정존종지파프필하항했`이다.
집합은 정확한 네 메뉴 문자열, 세 알림 문자열, 닫기 기호와 ASCII 범위의
합집합에서 코드 포인트 오름차순으로 산출한다. 구현 테스트는 원문 문자열을
다시 순회해 누락 글리프가 없고 전체 코드 포인트 집합이 정확히 138개임을
증명한다.

## 고정 Unity/TMP 생성 설정

아래 값은 현재 `Review` 중인 capacity amendment의 제안 profile이다. Astra가
승인하기 전에는 두 SDF 자산에 적용하거나 생성하지 않는다. 승인되면 두 face에
동일하게 적용한다.

| 설정 | 고정값 |
|---|---|
| source font | 위에서 검증한 정확한 Regular 또는 Bold OTF |
| atlas population mode | `Static` |
| sampling point size | `90` |
| atlas padding | `9` |
| glyph render mode | `SDFAA` |
| atlas width / height | proposed `2048 / 2048` |
| multi-atlas support | `false` |
| character population | 위 138개 코드 포인트의 정렬된 고정 집합 |
| fallback font assets | 빈 목록 |
| runtime glyph addition | 금지 |
| atlas/material storage | 각 TMP font `.asset`의 결정적 sub-asset |

생성기는 누락·중복·대체 글리프, 둘 이상의 아틀라스, 동적 population,
fallback, 다른 source GUID, 다른 설정을 실패로 처리한다. 같은 Unity 프로세스와
새 Unity 프로세스에서 재생성한 직렬화 바이트와 모든 기존 GUID가 같아야 한다.

### 정적 atlas dimension 정책과 historical capacity evidence

프로젝트가 source control로 관리하는 정적 TMP atlas dimension은 모든 face에
동일한 정사각형 2의 거듭제곱만 허용한다. 이 제한은 플랫폼별 texture import,
serialization, compression과 packing profile을 하나의 결정적 검증면으로
고정하기 위한 프로젝트 정책이다. uGUI가 NPOT 또는 직사각형 texture를
기술적으로 지원하지 않는다는 주장이 아니다.

기존 승인 profile `1024x1024 / point 90 / padding 9 / SDFAA / single /
no-multi`는 보존된 Unity 6000.6.0f1·uGUI 2.6.0 실제 run에서 exact sorted 138
population 중 정확히 `하항했`을 누락했다. 이 실패와
`artifacts/unity-results/m5d7qa-20260923/builder-pass-b.log`는 historical
evidence로 유지한다. 실패한 1024x1024보다 큰 square-POT 후보 중 다음이자
가장 작은 허용 profile이 proposed 2048x2048이다. 이는 임의 직사각형 중
수학적으로 가장 작은 면적이라는 주장이 아니며, 실제 Regular/Bold 성공은
승인 후 positive fixture로 증명해야 한다.

Proposed 2048x2048 Alpha8/no-mip raw level-zero payload는 face당
4,194,304 bytes, 두 face 합계 8,388,608 bytes이며 이전 1024x1024 pair보다
6,291,456 bytes 증가한다. 이는 총 resident/build size가 아니다.

## 보정 승인 뒤 생성 경계와 목적지

- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular.otf`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold.otf`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset`
- `Assets/UI/Fonts/Hub/NotoSansCJKkr-Bold-SDF.asset`
- `Assets/UI/Fonts/Hub/OFL-1.1.txt`

원 계약에 따라 OTF/OFL source prerequisites는 이미 도입됐지만 두 SDF 목적지는
1024 capacity stop 뒤 존재하지 않는다. Astra가 2048 보정을 승인하기 전에는
두 SDF 경로를 생성하지 않는다. 승인 뒤 Terra 구현은 이 문서에 OTF·OFL의
저장소 사본 해시와 Unity `.meta` GUID를 보존하고, 두 SDF `.asset`의 SHA-256,
내장 atlas/material의 구조·식별·설정 증거를 추가한다. Luna가 이를 독립
검증한다. 이 문서 보정 자체는 Unity 자산을 생성하거나 승인하지 않는다.

## 계약 연결

- `REQ-M5D7QA-007`, `REQ-M5D7QA-009`
- `AC-M5D7QA-008`, `AC-M5D7QA-010`
- `docs/specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md`
