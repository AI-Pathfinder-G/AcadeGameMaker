# C4 Play R2 생성기 재현 증거 후속 독립 검토

이 문서는 앞선 `docs/verification/2026-09-30-c4-play-r2-successor-artifacts-luna-review.md`의 P1을 후속 증거로 재검토한다. 재현 기록 `artifacts/c4-play-v4-builder-reproduction-r1.json`의 SHA-256은 `DE7325F5D6AF24C6586AEE68019E289884D5A2D0E2DE2FCA93CF3AA2841F8727`, 실행 로그 `artifacts/c4-play-v4-builder-reproduction-r1.log`은 `1236401BB2EC0B4FB7555DA3B865399919E1D5AE4949B439276E044FCCF40B08`이다. 승인 계약은 `docs/specs/work-contracts/2026-09-30-c4-play-r2-freeze-binding-successor-amendment.md` SHA-256 `965E7FD32298C2BD60CB8A744D00A50DAC0F29FD87B3AD2E87D81826FF88188C`이다.

재현 기록의 생성기와 검증기, Play 부모 및 세 열거형 입력의 복사본 지문을 현재 원본과 대조했다. 여섯 지문이 모두 일치한다. 기록은 임시 고정 v1 출력이 실행 전에 없었고, 격리된 작업 경로에서 `FocusedPlay` 생성기를 호출해 종료 코드 0을 받았다고 남긴다. 생성 출력 SHA-256은 `032376266E3217B859BDC9F01A98385AD275E25DF0443865196BE7782F97DF13`이며 현재 선택 v4 파일의 SHA와 일치한다. 저장소의 기존 v1 파일은 실행 전후 `08F3755DCCE22B49D6B337776D09F1AB60A28199EDEDC8B1C3871B4F1823A998`로 같고, 현재 파일과도 일치한다. 기록된 로그는 동일한 격리 호출을 포함하며 로그 지문도 실제 파일과 맞는다.

독립 구조 비교에서 선택 v3 `F572205953FCCD7EC50C1D6E586B8F84F036B22D939182B1022AC071C8E8E8EE`와 v4 사이의 유일한 값 차이는 `ParserEvidence.EnumSources[0].Sha256`이다. v4의 해당 지문은 Bridge의 현재 실제 SHA-256 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`와 일치한다. 32개 정식 이름·순서, 선택식, 부모 소스, 나머지 선택 필드는 동일하다. 원장 v13의 142개 파일 지문과 입력 목록 v13의 1,029개 경로·사유에 대한 앞선 독립 대조도 변경되지 않았다.

## 판정

앞선 P1은 닫혔다. P0 0건, P1 0건이다. 임시 생성 파일 자체는 현재 격리 경로에 남아 있지 않지만, 실행 기록에 담긴 생성 출력 SHA가 v4 전체 파일 SHA와 같고, 원본 소스·인자·종료 코드·기존 v1 전후 지문이 함께 연결되어 있어 승인 계약의 재생성 증거를 충족한다. Unity 실행이나 C4 전체 수용을 의미하지 않는다. 이 재검토에서는 생성기, Unity, 컴파일러, 검증 큐를 실행하지 않았다.
