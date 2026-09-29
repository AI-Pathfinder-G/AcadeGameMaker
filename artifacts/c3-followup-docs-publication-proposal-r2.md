# C3 제한 실행 증거와 후속 문서 게시 후보 r2

2026-09-29. 실제 작성자 `gpt-6-sol`. 읽기 전용 문서 PR 검토 자료이다. 원본 원장 `c3-followup-docs-publication-inventory.json` SHA-256 `42668846D498D23FD2D4F97E9A566BDD8EE68ADA9206A961F331A2652A9348B1`과 원본 설명 `c3-followup-docs-publication-proposal.md` SHA-256 `F9FD9DC5BA9A9EDFC232977CA85F113F8EC65ABFD2340787D0E7144997CF5078`은 이전 시점의 역사로 보존했다. 소스·QA·캡처 입력·기존 문서와 HEAD/index/원격을 수정하지 않았다. 네트워크·Unity·컴파일·게시·PR 생성은 수행하지 않았다.

새 원장 `c3-followup-docs-publication-inventory-r2.json`은 `origin/main` `46b14e97be2c5fc6017d92c09e3200899599830a`의 tree/blob 바이트와 현재 문서를 비교한다. 오늘 C3 문서와 부모 계약·참여 원장 104개 중 완료 기록 후보 89개를 선택했다. 신규 87개·변경 2개이고 나머지 15개는 동일 원격 또는 인계/정확 지문 연결 보류 자료다. 각 경로의 현재 SHA-256·원격 blob object ID/내용 SHA-256·상태·연결 목록을 기록했다. 수집/재확인 중 지문 차이는 0이다.

R11의 현재 확정 세 문서를 모두 포함한다. 첫 정적 구현 검수는 원본 후보에도 있었고 이번에 현재 지문을 재조회했다. 실제 결과 검수와 집중 부분 수용 두 문서를 새로 추가했으며 참여 원장은 완료된 최신 R11 절을 반영해 변경 후보로 전환했다.

| 문서 | 현재 SHA-256 |
|---|---|
| `docs/verification/2026-09-29-c3-r11-cancel-type-luna-runtime-review.md` | `B6F24ABCC572A63E2A8D6F724C1111B4F046E0C4DF769D9939945D88F19849B6` |
| `docs/verification/2026-09-29-c3-r11-play-focused-r1-luna-result-review.md` | `FAD61A27107B4E7CBEA42450715F22EE2232AAD4083C07152A077074BACDCE4D` |
| `docs/approvals/2026-09-29-c3-r11-play-focused-partial-acceptance.md` | `1CF32ED23DC6BA1AC38DDF1F04C4F4309F5DDFAC574DEB1BFDA59A081C31B4AD` |

실제 R11 Play 집중은 15/15 통과, native/QA/외부 실행 종료 0, 이름 차이/중복 0, 입력 884개 전후 차이 0이며 루나 결과 검수와 아스트라의 **15개 선택 범위만의 부분 수용**이 완료됐다. 이전 R6–R10 실패·R7 관측·미도달 한계를 보존한다. 최종 Edit 240개 및 377행/91사례의 실제 검증, 필수 562/51/610 회귀와 최종 독립 검수는 남아 있다. C3 전체와 C4는 미수용이다. 실제 임시 cohort의 메모리/실행 객체 부재를 전체 게임 세션 보존으로 확대하지 않는다.

원격에 없는 연결 원시/선택/지문 증거는 **39개**이며 모두 로컬에는 존재한다. 대표적으로 lower 집중 XML/log·상위 frozen manifests·R6–R10 실패 XML·R7 관측 XML·R11 실제 집중 결과·초기 코드 게시 후보/바이트 원장과 행 검증 도구가 있다. 문서 후보 밖의 연결 문서 한 개는 진행 이력 인계 `docs/handoffs/2026-09-29-merged-c3-lower-checkpoint.md`이다. 이번 docs 범위에 원시 증거·코드/자산/설정·Q0/필수 회귀 핀을 자동 추가하지 않는다. 원격 독자가 링크된 실제 증거를 확인할 수 있는 최소 증거 패키지의 허용 범위/크기/원본 관계는 별도로 결정해야 한다. 현 문서 목록을 완전한 증거 폐쇄로 주장하지 않는다.

초기 게임 코드 게시의 489개 후보와 직접 바이트 근거 공백 428개는 그대로 남는다. 이번 문서 검토는 게시 기반 계약을 승인하거나 실행 가능한 코드 PR을 만드는 작업이 아니다. 후보 안의 Approved 표준/승인 기록은 이미 부여된 제한 계약의 사실 기록이며 새 Approved를 만들지 않는다. 다른 날짜 Q0/필수 회귀 핀, Assets/Packages/ProjectSettings/QA 자료는 게시 후보에서 제외한다.

문서 PR 제목 후보는 **C3 제한 Play 집중 수용과 승인·검수·실패 이력 정리**이다. 원장의 `Candidate=true` 문서만 기본 후보로 검토하며, 루나의 독립 게시 범위 검수와 연결 증거 공백 검토 후 실제 PR 범위를 확정한다. 원격에 동일한 문서를 무차별 재게시하거나 최신 작성 중 문서를 포함하지 않는다. 위키 초안과 실제 위키 게시 상태는 이 문서 원장 범위 밖이며 완료로 주장하지 않는다.

새 원장 SHA-256: `E8912A39F5140C81B0D4E6677AE80B29C2A4EC180F510D180B126BC2F1A4EE22`.
