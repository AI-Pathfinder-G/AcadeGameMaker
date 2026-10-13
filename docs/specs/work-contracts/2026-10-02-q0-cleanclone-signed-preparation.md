# Q0 서명 문맥의 격리 구현 준비

상태: **Approved**, 2026-10-02 통합 책임자 승인. 소유 `REQ-PLAT-061/062`, `AC-PLAT-059/060`. 채택 초안은 `docs/proposals/2026-10-02-q0-cleanclone-signed-integration-contract-draft.md` SHA `7EFC91A494B60512ACE4056E7B3356AF34E5A988ED110C45D09EB92AC6721B39`이다. 이번 승인은 격리 준비와 음성 검증이며 현재 깨끗한 복제의 전체 수용/게시·실사용 키 발급은 아니다.

테라 `gpt-6-sol`은 부재를 확인한 `output/q0-cleanclone-signed-prep-r1` 안에서만 작업한다. 초안의 정확 네 C# 시험 파일과 메타를 원시 바이트로 복사·지문 확인하고 그 네 시험 소스만 편집한다. 초안의 다섯 원본 QA 스크립트는 그대로 보존하며 같은 상대 artifacts 경로에 제안된 다섯 c4-canonical 후속 파일을 생성한다. 추가 허용 보조는 이 복제의 `artifacts/c4-canonical-verify-context.ps1`, `artifacts/c4-canonical-repository-binding.psm1`, `artifacts/c4-canonical-preparation-selftest.ps1`이다. 새 결과 `artifacts/c4-q0-cleanclone-signed-preparation-v1.json`과 `docs/verification/2026-10-02-q0-cleanclone-signed-preparation-terra-report.md`는 원래 작업공간에 생성한다. 그 밖 새 C#/메타/asmdef/friend/제품/자산/설정은 허용하지 않는다.

네 시험에는 계획 경로 하나만 문맥 신호로 읽는 현재 분기를 추가한다. 공개키/계획 SHA/빈 Git 출력 등 환경 신호를 권위로 쓰지 않는다. 역사 메서드·선택·원시 핀·금지 토큰/구조 단언은 유지한다. 현재 분기는 동결된 문맥 검증기의 실제 종료와 엄격 결과를 재검사한 뒤에만 현재 핀/실제 Git 검사를 사용한다. 현재 실사용 검증기는 미발급이므로 준비 도구의 운영 경로는 서명/외부 고정이 준비되지 않았다는 명확한 비정상 종료를 반환하며 C# 현재 분기도 실패해야 한다. 임시 합성 키나 caller SHA로 양성 운영 경로를 만들지 않는다. 미래 키/검증기를 결속하려면 별도 개정으로 최종 소스를 다시 동결한다.

후속 큐/포획/선택/행 판정은 서명 문맥을 재확인하는 실패 우선 경계를 포함하고 기존9선택·188/377행·180초·종료/누락/중복 판정을 보존한다. 순수 저장소 보조는 실제 git status/HEAD/tree/ls-files와 정규화 절대 루트·정확 tracked 집합·메타/GUID/원시 SHA를 읽어 검증한다. 계획 속 Git 관측값을 실행 결과로 쓰지 않는다. 운영 경로에서 검증된 계획 없이 이 보조를 권위로 사용하지 않는다.

격리 자기시험만을 위해 `output/q0-cleanclone-signed-prep-r1/selftest-repositories` 아래에 최소 실제 Git 저장소를 초기화·커밋·수정/스테이징/삭제/이름변경/untracked할 수 있다. 원래 .git과 다른 저장소는 변경하지 않는다. 각 대상의 절대 경로가 이 허용 루트 안인지 확인한다. 네트워크/원격은 사용하지 않고 사실 값을 Git에 주입하지 않는다. 독립적인 저장소 검사 양성과 위 음성·누락/추가/파일 SHA/HEAD/tree/링크 이탈 및 운영 미발급 거절을 실제 관측한다. 준비 출력은 각 시험 저장소 밖에 기록해 청결을 오염시키지 않는다. 시간 낭비를 막기 위해 같은 실패를 반복하지 않고 정확 원인을 보고한다.

원문25개는 기존 CurrentRawBaseline와 LF534 전임 바이트로 별도 보존한다. 이번 네 C#의 의미 변경은 CRLF→LF 전이와 다르다. 전임 LF534 후보 SHA와 준비 후 의미 변경 SHA를 단계별로 기록하고 새 의미 바이트를 원문25개의 변환 결과라고 주장하지 않는다. 최종 현재 핀은 편집이 끝난 소스 원장이 나중에 결속한다. 미래 계획/검수 SHA를 자기 소스에 넣지 않는다.

실제 Unity/전체 회귀/운영 키/권위 기록/원격 게시 실행은 금지한다. 보호 원문534·LF534·LF505·설정/패키지/기존 QA/증거와 원래 Git은 불변이다. 정확 허용 파일 목록·복사 전후/편집 후 SHA·차이·실제 시험/오류를 결과에 기록한다. 모든 전체 수용/게시/루나 수용 플래그는 false이고 AC059/060 전체 충족으로 승격하지 않는다. 독립 검수는 별도다.
