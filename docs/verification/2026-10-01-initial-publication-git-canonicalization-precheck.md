# 최초 코드 게시의 Git 바이트 정규화 예비 대조

- 범위: 현재 v2 후보 501개를 읽기 전용으로 대조했다. 입력 v2 원장 SHA-256 `23F3060352AD230EE86FE480560A0591D72EC41ABCA18C421610257AEBF206FE`; 기계 판독 결과（`../../artifacts/c4-initial-publication-git-canonicalization-precheck-v1.json`；로컬 보존 자료） SHA-256 `1B20D797C61CBD13073F0E1CB9E186C61C8F437CC4D8F4784C74F6AFA8082251`.
- 방법: 각 실물 파일에 `git hash-object --no-filters`와 저장소 속성을 적용하는 `git hash-object`를 같은 시점에 실행하고, 다른 18개는 원문에서 CRLF만 LF로 치환한 바이트의 Git 객체 지문이 후자와 일치하는지 확인했다. 18개 모두 일치했다. 글꼴 등 이진 파일의 우연한 CRLF 바이트는 차이로 세지 않았다.
- 판정: 501개 중 **18개는 현 실물 바이트와 Git에 기록될 바이트가 다르다.** 모두 줄바꿈 정규화만으로 설명되지만, 현재 바이트를 새 기준으로 삼은 [ADR-0038](../adr/0038-forward-provenance-for-initial-code-publication.md)의 정확 바이트 결속과 자동으로 같아지지 않는다. 원본 파일이나 `.gitattributes`는 변경하지 않았다.

현재 [원격 `main` 개정](https://github.com/AI-Pathfinder-G/AcadeGameMaker/commit/e95bb5169ad2ed08106bd342915d4637a1241263)은 변경 요청 #6 병합 상태였다. 연결된 GitHub 읽기 도구에서 기존 충돌 두 경로의 원격 객체를 다시 읽었다.

| 경로 | 원격 Git 객체 | 현재 파일을 Git에 넣을 때의 객체 | 현재 원문 SHA-256 | 의미 |
| --- | --- | --- | --- | --- |
| `artifacts/c3-required-decision-expected-rows.json` | `439059572cba0dc55cb6c590769c0e1901a70280` | 동일 | `EEA9CAFD13AAE95852DAB176FFEEC54AB85DD33902C4078553BDD10597B11364` | 현재 원문의 CRLF 10,629쌍을 LF로 바꾼 지문 `E036962DE66278DE04E356F1C8BEAF8FE16934ECE20C7A05CEDC1F3D48110CD1`이 v2의 원격 바이트 지문과 일치한다. 따라서 의미 있는 내용 차이를 이 근거만으로 주장하지 않는다. 원문 바이트와 원격 바이트는 다르다. |
| `qa/tools/Test-QaCatalog.ps1` | `e2123ed07ff9f9d3d4697756ed5031c9ab86304b` | `59e003b360be6c949d38ae805a7f341cfe737830` | `E13E91B6967520D6ADC357E03C7A32D3B6A07F061A1FF0CA1F2A85228FF1B835` | 실제 내용 차이: 원격 대비 32행 추가·7행 삭제. 차단 목록 입력·검증 경로가 달라 별도 내용/의존 검수와 병합 판정이 필요하다. |

첫 경로는 v2의 **원문 바이트 충돌** 기록을 지우지 않는다. 새 대조는 그 원인이 Git 줄바꿈 정규화임을 좁힌 것이다. 18개 전체의 경로·원문/정규화 SHA-256·Git 객체 지문·CRLF 수는 JSON에 있다. 어떤 경로도 게시 허용하지 않았고, 두 번째 실제 차이를 자동 병합하지 않았다. 이 예비 대조는 루나의 v2 독립 검수, 501개 내용·의존 폐쇄, 깨끗한 복제, Unity 검증을 대신하지 않는다. 정규화된 바이트를 새 기준으로 다시 결속할지, 현 원문 바이트 보존을 위해 저장소 속성을 바꿀지는 게시 전 별도 결정이 필요하다.
