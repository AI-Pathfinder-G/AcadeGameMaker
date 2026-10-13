# Wiki Publishing Status

## 2026-10-14 문서 정리와 세션 인계

최신 로컬 상태는 [1챕터 직접 플레이 준비 인계](../docs/handoffs/2026-10-14-chapter1-session-handoff.md)와 [새 세션 프롬프트](../docs/handoffs/2026-10-14-next-session-prompt.md)에 정리했다. 기록 관리자의 한정 보정·검사·원문 전달을 확인했으며, 사용자의 플레이 연기 선택에 따라 Unity와 관리자를 종료했다. 실제 플레이·B1 FINAL·CH1 완주는 미완료다.

이번 정리는 문서 전용 병합을 준비하는 기록이다. 게임 코드·실행 도구·바이너리·대형 원시 증거는 로컬에 보존한다. 저장소 문서 병합 결과는 게시 보고서에 따로 기록하며, 이 문구만으로 원격 병합이나 별도 GitHub 위키 게시 완료를 주장하지 않는다. 게시 LF 열람본과 실행용 로컬 원문은 바이트 대조표로 구분한다.

## 2026-10-03 Q0 문서 병합과 회귀 복구 준비

[변경 요청 7번](https://github.com/AI-Pathfinder-G/AcadeGameMaker/pull/7)은 Q0 기본 실행 계약과 R1 공개권위를, [변경 요청 8번](https://github.com/AI-Pathfinder-G/AcadeGameMaker/pull/8)은 R2 복구 계약과 새 공개권위를 각각 두 문서로 원격 병합했다. R2 병합 개정은 `428a7fd710123dad19775e904108440afab1046e`다. [기본 계약](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/428a7fd710123dad19775e904108440afab1046e/docs/specs/work-contracts/2026-10-02-q0-trusted-child-execution.md)과 [R2 복구 계약](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/428a7fd710123dad19775e904108440afab1046e/docs/specs/work-contracts/2026-10-03-q0-r2-schema-and-issuance-recovery.md)의 권위를 유지한다.

R1 서명 전 실패와 R2 서명 후 큐 만료는 각각 보존한다. R2는 서명·독립 검수까지 완료했지만 계획 소비·아홉 회 회귀 시험은 수행하지 않았다. 사용자의 시험 실행 승인은 기존 큐 기한 이후에 도착했다. 같은 시험 범위의 R3 복구는 로컬 준비이며 새 운영키 승인·새 실행·새 공개권위 게시를 완료한 것이 아니다. 이 위키 갱신은 탐색 보기의 상태 보고이며 게임 코드·원시 시험 자료·로컬 R3 명세를 원격에 게시하지 않는다.

아래 항목은 각 작성 당시의 로컬 정리·게시 이력이다.

## 2026-10-02 입력 원장 확정과 후속 구현 개시

입력 분류1110행의 루나 독립 검수는 한정 범위 P0/P1=0/0이다. 실제 아스트라의 [인계서 개시 판정](../docs/handoffs/2026-10-02-luna-recovery-resume.md)에 따라 `input-disposition-v3.json`을 검수된 초안과 같은2072465바이트로 새로 발급했다. SHA는 `04AE96DA1BF8F18BCC4C9028FF5AEFBDBD4650F5D3560E80C55AF30C686EDF4A`이고 전수 바이트 차이는0이다.

준비 복제에는 승인된 원시 입력565개를 복사했고 구현 담당자와 주 조정자의 지문 대조가 모두 일치했다. 외부 역사 참조6개와 미래 생성5개는 현재 복사 수에 포함하지 않았다. [Unity 등록 보완 계약](../docs/specs/work-contracts/2026-10-02-q0-unity-launch-registration.md)에 따라 자식 통신·실제 프로세스 등록·후속 QA 도구를 구현한다. 통신 시험·Unity 컴파일·새 운영 발급·아홉 선택·원격 게시 완료를 뜻하지 않으며 이 항목은 로컬 기록이다.

## 2026-10-02 첫 발급 실패 보존과 후속 준비 계속

외부 권위 v3 준비는 테라와 루나의 독립 45/45 검증을 마쳤으나 첫 실제 발급은 연결 도구의 원격 쓰기 권한 오류로 중단됐다. 권위 결속·서명·변경 요청 생성·병합은 수행하지 않았다. 실패 기록은 `output/q0-external-authority-evidence-r1/issuance.json` SHA `5AE04E8AA043385C5C10E7EC2059E00CEAFC695270822DC8258AF16BA6AF6635`에 보존한다. 정확히 소유한 서명 자식의 종료를 관측했으며 관리 정리 성공이나 물리적 메모리 삭제를 주장하지 않는다.

정상 네트워크 환경의 명령줄 로그인과 저장소 조회는 유효했다. 제한 환경의 로그인 확인 실패와 연결 도구의 쓰기 권한 오류는 별개다. 재인증을 사용자에게 요청하지 않고, 승인된 신뢰 자식 계약의 입력 원장 확정·키 없는 통신 구현·컴파일 준비를 계속한다. 실패 세대의 재사용과 새 키 자동 발급은 금지하며 이 항목은 로컬 기록이다.

## 2026-10-02 서명 문맥 준비 검수와 외부 권위 구현

[Q0 준비 독립 검수](../docs/verification/2026-10-02-q0-cleanclone-preparation-luna-review.md)는 별도 저장소의 실제 Git 정상·음성 13/13, 준비 파일16개와 네 시험의 원본→LF 전임→의미 변경 계보를 대조했다. [실제 아스트라의 외부 권위 발급 계약](../docs/specs/work-contracts/2026-10-02-q0-external-authority-issuance.md)에 따라 다음 다섯 소스를 별도 복제에서 준비한다. 운영키 생성 전 독립 준비 검수를 마쳐야 하며 일반 명령행과 기존 C# 호출의 권위 거절은 유지한다.

내장19참조 객체 식별, 정상 갱신 뒤 점 조명16픽셀 차이, 서명 핵심15/15·비영속 제공자10/10·두 단계 발급27/27의 한정 수용을 완료했다. [Renderer7 독립 검수](../docs/verification/2026-10-02-renderer7-current-scope-luna-review.md)는 현재 정적 경로를 대조했으며 범용 재로드·마이그레이션과 전체 소비 쟁점은 열려 있다. 새 운영키·권위 문서 원격 게시·전체9선택·게임 게시를 완료했다고 표시하지 않는다. 이 항목은 로컬 기록이다.

## 2026-10-02 연속 후속 작업

[미해석 참조 분류](../docs/approvals/2026-10-02-reference-classification-limited-acceptance.md)는 테라 구현과 루나 독립 검수를 완료했다. [렌더 진단 v3 제한 수용](../docs/approvals/2026-10-02-renderer-current-use-v3-limited-acceptance.md)은 실제 렌더 관측과 입력 보존을 기록하되 조명 감쇠 자원/APV 사용과 기존 렌더 GUID 쟁점을 열린 상태로 유지한다. 내장 참조 후보19행의 실제 객체 조회와 Q0 서명 핵심의 독립 검수를 진행한다. 서명 핵심의 합성15개 통과는 실사용 승인 루트나 코드 게시 승인으로 해석하지 않는다. 이 문구는 로컬 후속 기록이며 새 원격 게시 완료가 아니다.

## 2026-10-02 한도 회복 후 로컬 후속 증거

실제 `gpt-6-luna` 재호출에 성공해 출처v2와 정규화 준비 증거의 독립 검수를 진행했다. 원문 보존 도구는 최종 합성 정상1·음성4와 후보 안 출력 거절, 실제534 대조·25개 원문 복원·전후 변경0을 확인하고 기능 범위만 제한 수용했다. [원문 도구 수용](../docs/approvals/2026-10-02-raw-lineage-limited-acceptance.md)과 [재개 인계](../docs/handoffs/2026-10-02-luna-recovery-resume.md)가 현재 로컬 증거다.

501개 조사에서 발견된 커밋/트리 객체 지문 혼동은 원본 기록을 보존한 채 새 원격 관측으로 보완한다. Q0 깨끗한 복제 구현 초안은 Draft이며 Renderer2D 참조·내용 및 간접 의존 폐쇄·새 전체 회귀·깨끗한 복제·최초 코드 게시 수용은 별도다. 이 로컬 위키 문구 작성은 새 원격 게시나 변경 요청 병합 완료를 뜻하지 않는다. 아래 원격 게시 이력은 각 당시 기록이다.

새 원격 보완의 루나 직접 재조회까지 완료해 [조사 증거를 제한 수용](../docs/approvals/2026-10-02-content-survey-evidence-limited-acceptance.md)했다. 정확501 존재7/부재494·7본문 지문 일치·원문충돌2/정규화충돌1을 확인한 것이며 충돌 해결·파일별 게시 허용은 아니다. 원문 보존 기능 수용과 이 새 원격 증거 수용은 전체 게임 회귀 또는 위키 원격 게시 완료와 구분한다.

## 2026-09-29 문서 변경 요청 원격 병합 완료

사용자 지시로 [문서 정리 변경 요청 #1](https://github.com/AI-Pathfinder-G/AcadeGameMaker/pull/1)을 원격 main에 병합했다. 실제 병합 개정은 `9b07ec59c6ffc201e7f218857c553ae56610af6a`이다. 아래 초안 생성·인증 차단 상태는 당시 이력이다. 문서 병합은 로컬 게임 코드 전체의 통합이나 C3 전체 수용을 뜻하지 않는다.

## 2026-09-29 변경 요청 생성 완료

사용자가 공식 기기 인증을 직접 승인한 뒤 [문서 정리 변경 요청 #1](https://github.com/AI-Pathfinder-G/AcadeGameMaker/pull/1)을 초안으로 생성했다. 대상은 main이며 문서 브랜치와 위키 게시 상태를 검토할 수 있다. 변경 요청을 이 채팅에 첨부했고 병합은 수행하지 않았다. 아래 인증 갱신 대기·변경 요청 부재는 이전 시점의 기록이다.

## 2026-09-29 후속 관찰 구현 검증 반영

문서 브랜치와 위키에 사용자 승인 조회 함수의 정확한 후속 지문 승인, C3L 집중 11개와 동일 소스 필수 회귀 고유 613개, 최종 독립 검수·관찰 단위 수용 및 ADR-0036 단계적 수용 순서를 반영한다. R4 시간 초과 이력과 R6 현재 재실행을 구분한다. C3 전체 검증이나 Review C4 구현 완료로 표시하지 않는다. 변경 요청은 여전히 생성하지 못했으며 인증 갱신을 기다린다. 아래 개정 값은 이전 게시 이력이다.

중간 보완 문서 개정 `9c4014e1d448f398de9603fd59754fbad26434a9`와 위키 개정 `06cb91542b4699a8ec7e4654306169122326d20e`는 원격 게시를 완료했다. 최종 수용 상태는 별도 후속 개정으로 게시한다.

## 2026-09-28 게시 준비

이번 정리본은 `codex/documentation-checkpoint-20260928` 문서 브랜치를 원본으로 사용한다. 문서 개정 `4d3fd80079392368f9726e232467f7dd20b36f2a`는 원격 게시됐으며 변경 요청 생성은 인증 갱신 대기 상태다. 아직 병합 전이므로 계약 링크는 해당 브랜치로 연결한다. 아래 2026-08-24 게시 완료·개정 기록은 당시 이력이다.

위키 본문 게시 개정은 `8ea7ee9332779992ae3bf6b5f02d89cd260c7f61`이며 2026-09-28에 원격 게시 성공과 실제 최신 구현 상태 페이지를 확인했다. 독자 페이지 14개와 탐색 파일 1개를 게시했고 위키 내부 연결의 누락은 0개다. 본 기록과 권위 문서 링크 정정은 그 뒤 별도 게시 개정으로 남긴다. 위키 게시 자체는 게임 구현·시험 수용 증거가 아니다.

## Repository publication

- Repository: [AI-Pathfinder-G/AcadeGameMaker](https://github.com/AI-Pathfinder-G/AcadeGameMaker)
- Visibility: Public
- Default branch: `main`
- Initial documentation revision: `5b8d090`
- Latest narrative documentation revision: `7fb211f`
- Wiki package: `wiki/`
- Entry page: `Home.md`
- Navigation: `_Sidebar.md`
- Repository package status: Published
- Checked: 2026-08-24 (Asia/Seoul)

## Native GitHub Wiki status

- Wiki: [AcadeGameMaker Wiki](https://github.com/AI-Pathfinder-G/AcadeGameMaker/wiki)
- Status: Published
- Initial full-package revision: `ca9953376a47d6aa3690d8675004440f96573e63`
- Character-first narrative package revision: `1a0c76b48dde2e2c94b4a88b81cd6ae03a178d6e`
- Published pages: 13 reader pages and `_Sidebar.md`
- Verified: Git push success, 13 reader-page file count, local Markdown links, Wiki-style page targets, and authoritative source links
- Checked: 2026-08-24 (Asia/Seoul)

Wiki는 권위 문서가 아니라 발행·탐색 계층이다. 충돌 시 `CONTEXT.md`, `docs/canon/`, 현재 ADR, Approved 스펙 순으로 원본을 따른다.

## Republish procedure

1. 권위 문서 변경을 먼저 `main`에 반영한다.
2. 대응하는 `wiki/` 요약과 원본 링크를 갱신한다.
3. `wiki/` 전체를 Wiki 전용 Git 저장소에 게시한다.
4. Home, Sidebar, 모든 권위 문서 링크를 검수한다.
5. 게시 revision과 검수일을 이 페이지에 기록한다.
