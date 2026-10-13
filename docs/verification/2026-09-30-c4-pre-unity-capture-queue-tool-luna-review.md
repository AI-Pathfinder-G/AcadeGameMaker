# C4 실행 전 캡처 복구 큐 도구 독립 정적 검토

승인 계약: `docs/specs/work-contracts/2026-09-30-c4-pre-unity-capture-recovery-amendment.md`\
승인 계약 SHA-256: `17F4694236C5F7EC561EFC3E86BF88C4A50F2ABC455FD0450420ACC28213168B`\
검토 대상: `artifacts/c4-final-validation-queue.ps1`\
현재 SHA-256: `C233CFF842B30087B0220FBB19267236FBA936280E739E42BBA66A031F79B509`\
판정: **P0 0건, P1 0건. 승인된 한 줄 변경과 일치한다. v15 발급·Unity 실행은 검토하지 않았다.**

현 큐 도구에서 `artifacts/c4-final-validation-queue-result-v9.json` 토큰은 결과 경로 비교에 정확히 한 번 나온다. 해당 토큰만 v8로 되돌린 바이트를 메모리에서 해시한 값은 `6D3F435E2655A8420252B9E8B2D8A550106E6CCB9792659ECA7DB7A4FF928279`로, 승인 계약의 기존 도구 지문과 일치한다. 현재 도구의 PowerShell 구문 분석 오류는 0건이다. 따라서 허용된 변경은 결과 경로 문자열 한 곳뿐이며 다른 도구 로직은 바뀌지 않았다.

큐 결과 v8은 그대로 존재하고 SHA-256 `573480005473F747DC41283E1D9F14C0A4F650E786737A81F87F692962F7031E`이다. 새 v9 결과 경로는 아직 없다. 변경 도구는 기존 결과를 덮어쓰지 않고 v9만 허용하며, 결과 생성·실패 중단·프로젝트 잠금·실제 종료 기록·전후 입력 및 행 검증 경로를 유지한다. 원시 종료, QA 반환, 외부 종료 관측 구조도 변경하지 않았다.

이 검토는 큐 도구의 승인된 한 줄 변경만 다룬다. source manifest v15, input v15, plan v11, 선택·출력 결속 및 새 큐 실행은 확인하거나 수용하지 않았다.
