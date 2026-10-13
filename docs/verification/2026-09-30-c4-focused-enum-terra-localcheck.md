# C4 집중 선택 열거형 파서 테라 국소 검사

- 작성: 2026-09-30, 테라 구현 담당, 실제 `gpt-6-sol`. 자체 검사이며 독립 수용이 아니다.
- 추적: REQ-M5D7QC4-007, AC-M5D7QC4-010.
- 수정 전 `artifacts/c4-build-focused-selection.ps1` SHA `B16362720E9DE2E69FC44990A664EA221E6A928D8A795A5F70C959D785D1015C`; 수정 후 SHA `CAE02BEF76CCEC382AE44AC34E3D552097189F79402589DB91A30230667260CB`.

실제 `TestCase` 인자에서 참조하는 열거형 이름만 먼저 수집한다. 열거형 소스의 모든 경로 지문은 그대로 묶되, 실제 참조 이름의 선언만 기존 정수·중복·별칭 검사를 적용한다. 참조 이름의 누락·암시 값·복합 표현·동적 인자·이름 붙은 인자는 계속 거절한다. 시험 class·메서드·속성·이름 생성의 기존 검사는 유지한다.

실제 Bridge 파일 `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetExecutionBridgeV1.cs` SHA `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`를 사용한 국소 파서 검사 실제 종료값0, 예상 일치10/10이다. 현재 Edit 시험과 해당 Bridge의 주변 private 암시 열거형은 허용됐고, 실제 명시 `ProfileResetExecutionCheckpointV1` 참조는 허용됐다. 실제 `ExecutionEventKind.Consumed`를 시험 인자로 넣으면 거절됐다. 추가 음성은 참조 선언 누락, 중복 선언, 값 별칭, 이름 중복, flags·표현식·named 인자다. 국소 함수만 실행했고 집중 선택 JSON은 생성하지 않았다.

| 파일 | SHA256 |
| --- | --- |
| docs/verification/2026-09-30-c4-focused-enum-terra-localcheck.ps1 | 479CE257442C1F7E657F982375069A6991B90EA8FD67A981F30718099FEC7909 |
| docs/verification/2026-09-30-c4-focused-enum-terra-localcheck.json | F30D74AB190762C55F1299CFAA30EB9AEAD7318A43A57737AA21313499073871 |

새 도구 PowerShell 구문 오류0. Unity·runner·queue·최종 선택 생성은 수행하지 않았고 전체 수용은 남아 있다.
