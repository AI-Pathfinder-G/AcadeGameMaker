# CH1 오르단: 오른쪽 공격 패턴 애니메이션 후보 v1

**상태:** 개발용 원화 후보 구현·Unity 컴파일 및 집중 검사 통과. **최종 수동 아트 승인과 독립 QA는 미완료.**

## 근거 및 제품 변경
- 원본: 사용자가 제공한 로컬 H 드라이브 `13-ordan-combat.png` (1536×1024, 가로 6 × 세로 4); 원본 바이트는 변경하지 않음.
- 새 생성 도구 `tools/sprite_atlas/build_ch1_ordan_candidate.py`에서 감지된 마젠타 배경만 투명화하고, 256px 원본 정렬을 유지한 채 128px 후보 키로 **일괄 1/2배 nearest-neighbor 리샘플링**함. 임의 프레임 크기 보정·좌우 반전 없음.
- 오른쪽 공격 후보 아틀라스: `Assets/Resources/Chapter1Ordan/Candidate-OrdanAttackRight.png` (768×512, 24 프레임); SHA-256 `da080a408258835faf991683acb6860b45fc8fc355eca5ff3abd8a4cc0ae7382`.
- 원본 시트의 4개 행: guard-step / debt-line / seizure-weight / balance-audit. 실제 보스 슬롯 DebtLineA/B는 동일 debt-line 행을 사용하고 SeizureWeight·BalanceAudit는 고유 행을 사용.
- `Chapter1OrdanVisualEvidence`는 현재 소유 graph의 **기존 완료된 +110 보스 handoff publication**만 읽음. 기록이 없거나 종료된 장면이면 정적 원본으로 돌아감.
- `Chapter1OrdanArt.SelectRightPatternCell`에서 **Telegraph/Execute/Recovery의 실제 발행된 단계**만 프레임 선택, Vulnerable/Defeated·미지의 슬롯에서는 원본 정적 표시를 보존.
- GUI용 프레임 렌더는 전체 텍스처가 아니라 6×4의 실제 선택 Sprite UV만 그림.
- **왼쪽 방향 기존 `Chapter1Ordan/Left.png`는 손대지 않았으며, 편의상 반전하거나 오른쪽 공격 이미지를 가짜 왼쪽 이미지로 사용하지 않음.** 독립 원화가 확보되기 전까지 왼쪽은 정적.
- 전투 AI, 보스 패턴 시간, 데미지·히트박스, 플레이어 입력, 저장, 원정·종결 권위에 변경 없음. 원본과 출력 무결성 증거는 로컬 `output/ch1-ordan-art-20261109/right-phase-atlas-v1`에 보관.

## 실제 검사와 한계
- 24 프레임 변환, 원본 보존, 잘림 거부를 확인한 Python 생성 검사 **3/3** 및 전체 스프라이트 회귀 **54/54** 통과.
- Unity **EditMode** `Chapter1OrdanPhaseArtPlayModeTests` **3/3** 통과. 처음 두 번의 PlayMode 테스트 결과 0/0은 테스트 어셈블리 플랫폼 선택 오류로, 통과로 계산하지 않음. 초기 메타파일 YAML 실패도 수정함.
- Windows64 빌드 `attempt-040`: 성공, 컴파일 오류 0건 / Unity 빌드 경고 1건.
- **오른쪽 공격 실제 자연 보스 플레이 1배율 동작 및 신규 빌드 전체 분기 완주는 미검증.** 앞선 `attempt-039`의 봇 자연 보스 검증은 DebtLineA Telegraph 진입 단계에서 입력 경로 안전 검증 불가로 실패한 원문을 보존함.
- 다음 우선순위: 사용자 고유 왼쪽 보스 공격 동작 제작, 패턴 전환과 예고/피격/기술 효과의 1배율 플레이 가독성 검증, 보스 입력 경로 최소 재현과 별도 수정.

## 2026-11-09 후속 — 왼쪽 공격 아틀라스 컷아웃 리그 후보

- 사용자 소유 원본 `Chapter1Ordan/Left.png`(1215×1295 RGBA)만 사용하여 128px, 6×4, 24키 왼쪽 방향 아틀라스를 제작. 오른쪽 24포즈를 반전하거나 픽셀 입력으로 사용하지 않았으며 원본 왼쪽 마스터 SHA-256은 이전과 동일함.
- `tools/sprite_atlas/build_ch1_ordan_left_rig.py`: 왼쪽 원본 전완부를 원래 팔 위치에서 분리한 뒤 회전시키는 제한적인 컷아웃 리그, 패턴별 갈고리·압수 중량 블록·계량 장치 효과. 24키 중 20개는 RGBA가 구별되지만 손수 그린 완성 프레임이 아니고 하반신 보행은 정적임.
- Unity `Candidate-OrdanAttackLeft.png` SHA-256 `1f1410522eebc50f09aea0aec2947fd568859a800248547fdfc251c880472b51`. 왼쪽 캐릭터 47.4 PPU와 원본 보행 피벗을 유지하며 `Chapter1OrdanArt`가 이미 발행된 동일 보스 패턴·단계에서 해당 방향 컷만 선택함. 게임 시뮬레이션/보스 시간표/공격 판정/저장 로직 변경 없음.
- Python 왼쪽 전용 생성 테스트 3/3, 전체 Python 그래픽 회귀 57/57. Unity EditMode의 `Chapter1OrdanPhaseArtPlayModeTests` 4/4 통과, Windows64 빌드 `attempt-041` 성공(오류0/경고1), 격리 실행 9초 생존 및 미처리 예외 패턴 0건. 수동 자연 플레이·보스 전 구간·최종 미적 승인·독립 검수는 미달.
- 기존 자동 보스 증거 `v4-gait-branch1-clean-2`의 중단은 입력 소유자 시퀀스가 DebtLineA 2틱 부근에서 D+Space를 발행한 뒤 약 18틱 관측 공백을 거쳐 20틱째 x=-7 정지 상태로 재관측한 안전 인증 불가다. 이 사실을 게임 성공으로 고치거나 동작 권위를 바꾸지 않았고, 동일 조건 재실행은 보류.
