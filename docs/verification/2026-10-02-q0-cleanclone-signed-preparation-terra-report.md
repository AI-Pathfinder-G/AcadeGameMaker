# Q0 깨끗한 복제·서명 문맥 격리 준비 — 테라 보고

- 승인 계약: `docs/specs/work-contracts/2026-10-02-q0-cleanclone-signed-preparation.md` (`Approved`), 소유 `REQ-PLAT-061/062`, 부분 검증 `AC-PLAT-059/060`. 채택 초안 SHA-256은 `7EFC91A494B60512ACE4056E7B3356AF34E5A988ED110C45D09EB92AC6721B39`로 대조했다.
- 부재였던 `output/q0-cleanclone-signed-prep-r1`에만 네 C# 시험과 각 메타를 원시 복사하고, 다섯 QA 원본은 각각 `c4-canonical-*` 후속 이름으로 복사했다. 복사 직후 13개 파일의 원본·복사 SHA 일치를 확인했다. 이후 네 C#과 다섯 후속 도구만 편집하고, 허용된 보조 세 파일을 새로 만들었다. 자체 시험 저장소를 제외한 복제 안 파일은 정확 16개다. 실제 파일별 원본·LF534 전임·편집 후 SHA와 추가/삭제 행수, 메타 동일성, 후속 도구 SHA는 `artifacts/c4-q0-cleanclone-signed-preparation-v1.json`에 기록했다.
- 네 C#의 원본→LF534 전임 SHA를 별도 열에 두었다. `HubUiOnlyQ0ScopeAuditEditModeTests.cs`는 `20F5E1...`→`AE3FBA...`→편집 후 `A5B037...`, 편집 fixture는 `DEDF38...`→`C92F2E...`→`0FF191...`, 실행 fixture는 `2237DA...`→`47CBC5...`→`E72422...` 순서다. 엄격 감사의 원본과 LF534 전임은 동일한 `6EEFEA...`, 편집 후는 `EEF283...`이다. 앞의 LF 전이와 뒤의 현재 문맥 의미 변경은 다른 단계이며, 편집 바이트를 원문 25개의 줄바꿈 변환 결과로 부르지 않는다.

## 구현 및 실제 음성 실행

복제 C#의 Q0 세 진입점, 엄격 감사와 fixture의 관련 진입점에 계획 경로 환경변수 하나만 읽는 현재 문맥 조기 관문을 추가했다. 역사 본문의 메서드명·원시 핀·금지 토큰 단언과 fixture 본문은 삭제하지 않았다. 현재 운영 공개키와 외부 인증 루트가 미발급이므로 `c4-canonical-verify-context.ps1`은 `Verified=false`와 종료코드 2를 반환한다. C#은 그 실패를 수용하지 않으며, 예상 밖 종료코드 0도 현재 준비 단계에서 승인으로 취급하지 않는다. 다섯 `c4-canonical-*` 후속 도구는 원래 판정 본문 전에 같은 미발급 관문을 호출한다. 실제 호출 다섯 건이 모두 종료코드 1과 미발급 사유로 조기 거절됐고 Unity·제품 입력은 읽지 않았다.

읽기 전용 `c4-canonical-repository-binding.psm1`은 실제 `git rev-parse --show-toplevel`, `HEAD`, tree, `status --porcelain=v1 -z --untracked-files=all`, `ls-files -z`, 정확 tracked 목록과 파일 원시 SHA, 메타 GUID, 경로 정규화와 reparse 이탈을 확인한다. 계획 속 Git 관측값을 쓰지 않는다. 실제 Git 초기화·커밋·수정은 허용된 `selftest-repositories` 아래에서만 했고 원격은 추가하지 않았다. 최종 자체 시험은 **13/13 통과**: 청결 정상, unstaged·staged·삭제·이름변경·untracked, HEAD·tree, tracked 누락·추가, 파일 SHA, 메타 GUID, junction 이탈 음성이다. 이 시험의 실행 루트와 개별 거절 사유도 결과 JSON에 남겼다. 준비 결과 파일은 시험 저장소 밖에 썼다.

복제 C# 네 파일은 설치된 Roslyn 구문 파서에서 오류 0, 준비 PowerShell 여덟 파일은 PowerShell 파서에서 오류 0이었다. Unity 컴파일·전체 회귀는 실행하지 않았다. 모든 결과 플래그 `DependencyClosure=false`, `LunaReviewed=false`, `WholeAccepted=false`, `PublicationApproved=false`다. 결과 JSON SHA-256은 `4B2C0F3D0B5D9F0F5465AEB22324792566A2F3D1CA7877C5FD94C9AEBAB69342`다.

## 남은 결속 범위

현재 문맥의 성공 경로는 아직 없다. 외부 인증 루트·고정 공개키·최종 서명 계획이 없으므로 복제 C#의 실제 현재 핀/Git 양성 분기와 후속 큐의 9선택·188/377행을 실행하거나 수용했다고 주장하지 않는다. 특히 기존 엄격 감사가 편집된 Q0 메서드의 역사 비교를 어떻게 확인할지는 Unity 실행 및 독립 검수에서 다시 판정해야 한다. 새 의미 바이트의 최종 소스 원장과 전체 직접 입력 폐쇄도 후속 계약이 필요하다. `AC-PLAT-059/060` 전체 충족, 깨끗한 복제의 전체 수용, 게시 승인은 여전히 열려 있다.
