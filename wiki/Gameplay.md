# Gameplay

## Core verb

플레이어는 자신의 무게를 상자와 적에게 넘기고 되찾는다. 주인공은 가벼워져 공중 기동이 달라지고, 대상은 무거워져 낙하·충돌·환경 반응이 달라진다. 수직 데모에서는 중력 방향 변경을 다루지 않는다.

## Expedition structure

지형을 임의 생성하지 않는다. 전체 방 풀은 입출구와 핵심 동선을 검증한 수작업 방 정확히 6개로 구성하고 한 원정에는 4개를 사용한다. 시작 `R01`과 수렴 `R06`은 고정하며 가운데 두 방과 소켓 변주는 지원 시드 101, 202, 303, 404의 검증 snapshot으로 결정한다.

## Progression choice

수탈은 활성 전이 대상에 피해와 경직을 주는 `압착 판결`, 연대는 플레이어만 밟는 임시 발판을 만드는 `공동 기준면`을 제공한다. 두 기술은 같은 봉쇄선을 통과할 수 있지만 타인의 주체성을 다루는 방식이 다르다.

**권위 문서:** [게임 디자인 캐논](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/canon/game-design.md), [무게 전이 스펙](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/02-weight-transfer.md), [방/원정 스펙](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/04-authored-rooms-and-expedition.md)
