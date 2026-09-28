# 현재 구현 상태

정리 기준: 2026-09-29. 이 페이지는 발행·탐색용 보기다. 캐논·결정 기록·승인 명세가 동작과 승인 범위를 소유한다.

| 범위 | 현재 상태 | 범위와 남은 작업 |
|---|---|---|
| C1 저장 초기화 | 검증·수용 완료 | 디스크 전용. 실제 메뉴 연결은 별도 |
| C2 메모리·입력 전환 | 검증·수용 완료 | 당시 소스의 독립 객체 구성 범위 |
| C2R 재시작 복구 | 검증·수용 완료 | 실제 별도 프로세스 복구 포함 |
| C3L 관찰 잠금 | 구현, 집중 시험 11/11 | 현재 소스 편집 모드 회귀 562개 진행 중, 별도 프로세스 회귀 51개와 최종 독립 수용 필요 |
| C3 확인·취소 | 승인, 미구현 | 새 결정 세대와 취소 후 입력 세대 구현 필요 |
| C4 초기화 실행 연결 | 검토 중 | C3 선행 단계 독립 수용·소스 동결과 별도 계약 승인 후 구현 |
| 실제 화면·목적지·옷장 | 후속 범위 | 별도 계약 및 실제 미디어 수용 필요 |

C2/C2R 최종 수용 근거는 편집 모드 562+51=613개, 실행 모드 536+74=610개 고유 검사이며 실패·건너뜀·판정 불가가 없다. 이는 이전에 동결된 소스의 결과이며 이후 C3L 변경분이나 미구현 C3의 통과 결과가 아니다. 위키 게시 자체도 새 게임 시험이 아니다.

세령 외형은 실제 수용 조합 0/432를 유지한다. 첫 A1/H1 제작 도구의 수용은 실제 착용·게임 내 표시 수용을 뜻하지 않는다. 의상 표시 어댑터의 합성 자료 검증과 실제 미디어 수용도 구분한다.

후속 작업은 아스트라가 범위를 승인하고 테라가 구현한 뒤 루나가 독립 검증하며 아스트라가 최종 수용한다. C3는 확인·취소와 한 번만 소비하는 실행 허가까지만 다루며 C1 초기화나 C2 전환을 직접 호출하지 않는다.

사용자가 승인한 내부 읽기 전용 경로 조회 함수를 추가했고 정확한 후속 지문을 독립 검수·승인했다. C3L 집중 시험은 실제 종료 코드 0, 실패·건너뜀·판정보류 0이며 시험 이름과 전후 입력 874개의 지문 차이도 0이다. 필수 회귀가 끝나기 전에는 C3L 전체 수용으로 기록하지 않는다.

ADR-0036은 C3 선행 검증과 C4 실제 실행 연결 공동 최종 검증의 순환을 해소한다. C3 실행 결과 관련 두 기준은 부분 증거만 남기고 전체 미검증을 유지한다. C3 전체 검증이나 C4 구현 승인을 뜻하지 않으며 필요한 집중·편집 모드·실행 모드 회귀와 독립 검수는 그대로 적용한다.

권위 문서와 근거는 문서 정리 변경 요청의 정확한 개정에서 확인한다. 기본 브랜치에 병합되기 전에는 변경 요청의 파일 보기를 사용하며, 기본 브랜치 링크만으로 최신 자료를 판단하지 않는다.

- [문서 지도](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/README.md)
- [C3L 집중 실행 기록](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/verification/2026-09-28-vd09-m5d7q-c3l-focused-execution.md)
- [단계적 수용 결정](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/adr/0036-staged-confirmation-and-execution-acceptance.md)
- [다음 세션 인계서](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/handoffs/2026-09-28-next-session.md)
- [C2/C2R 최종 독립 수용 기록](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/verification/2026-09-28-vd09-m5d7q-c2-c2r-luna-final-independent-acceptance.md)
- [C3 계약](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c3-confirmation-owner.md)
- [C3L 계약](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c3l-observation-lease-provenance.md)
- [C4 계약](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c4-reset-execution-bridge.md)
