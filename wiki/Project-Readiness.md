# Project Readiness

## 2026-09-29 최신 상태

현재 준비도는 [[현재 구현 상태|Implementation-Status]]와 각 권위 계약·검증 기록을 따른다. 새 게임 C1·C2·C2R과 C3L 관찰 단위는 범위를 제한해 수용됐고, C3 소유자·취소·재무장은 후속 구현이며 C4는 검토 중이다. 실제 확인 화면·거점 이동·옷장 연결과 세령 미디어의 수용은 완료되지 않았다. 아래 2026-09-06 및 그 이전 내용은 당시 기록이다.

2026-09-06 감사 결과, **수직 데모는 계약 단위로 구현 중이며 M4B1·M4B2까지 검증되었고 M4B3A는 fresh Graph PlayMode 증적 대기 상태다.**

현재 저장소는 전체 게임 완성본이 아니라 검증된 수직 데모 기반부다. Unity 프로젝트 기준은 `6000.6.0f1 (f7f8ed4d1e24)`로 갱신됐고 새 Editor에서 패키지·배치 검증이 진행 중이다. BGM은 20곡 방향 중 MP3 생성본 3개가 있고, MiniMax H3 티저는 MP4 생성본 3개가 있다. 모두 최종 채택·편집·게임 임포트 검증 전의 제작 산출물이다.

아래 2026-08-27 진행 문단은 당시의 역사적 기록이며 현재 상태의 기준은 위 감사 요약과 각 작업 계약·검증 증적이다.

## 핵심 상태

- gameplay 구현 가능한 `Approved` 스펙: `VD-00~10` 전체 (`VD-11` pre-Unity QA infrastructure는 Verified)
- 열린 P0 미결정: 0개
- 열린 P0·P1 결정: 0개
- Unity Hub와 Unity Editor 6000.6.0f1: 설치·버전 검증 완료; 새 Editor bootstrap/배치 검증 대기
- Git과 Git LFS: 준비됨
- 무료 에셋: CC0 원본 7개 격리 확보, 미임포트
- `main` 브랜치 보호와 Unity용 루트 무시·속성 규칙: 적용 완료

## 완료한 승인 범위

- Unity Hub와 Unity 6000.6.0f1 설치·버전 검증 및 프로젝트 기준 갱신
- Unity Hub 계정 로그인·프로필 반영과 Personal 라이선스 활성 표시 확인
- Ollama Cloud GLM 5.2와 MiniMax M3의 비민감 고정 문장 연결 시험 (역사 기록; 현재 사용하지 않음)
- Unity 프로젝트 생성 전 저장소 안전장치 적용
- Sol P0 기본안 사용자 승인과 ADR-0018·스펙 반영

IDE, 아트 편집기, 오디오 도구, 추가 플랫폼 모듈은 해당 작업이 시작될 때 설치한다.

## 다음 게이트

Unity 라이선스 초기화가 정상화되면 M4B3A fresh Graph PlayMode를 재실행하고 `AC-COM-002`, `AC-COM-003`, `AC-COM-004`, `AC-WT-005`에 연결된 구현 증적을 작성한다. 이후 M4B3B와 room/reward/run 소비자를 진행한다. 실제 consumer 증적이 생길 때까지 `AC-WT-002`와 `AC-WT-005`의 deferred 범위는 완료로 표시하지 않는다.

## 2026-08-25 구현 게이트

- P0·P1 열린 결정 0개
- Luna 최종 계약 검토 PASS
- QA catalog 13 scenario, 68/68 AC coverage, blocked scenario 0
- Sol 수직 데모 구현 게이트 승인
- Unity backend는 현재 설치 모듈에 맞춘 Windows x64 Mono로 고정
- 저장소 루트를 Unity 프로젝트 루트로 사용
- 외부 에셋 import와 gameplay 구현은 각각의 승인된 작업 계약 경계 안에서만 수행

## 2026-08-24 진행 갱신

- 저장소 안전장치와 `pre-unity-docs-v1` 태그 적용 완료
- Unity Hub 설치 완료
- Unity Editor 6000.3.21f1 설치·버전 검증 완료
- Ollama Cloud GLM 5.2·MiniMax M3 연결 시험 완료 (역사 기록; 현재 사용하지 않음)
- Unity Hub 계정 로그인·프로필 반영과 Personal 라이선스 활성 표시 확인; 저장소·Unity Cloud 프로젝트는 미연결
- Sol P0 통합안 사용자 승인 완료, ADR-0018과 규범 스펙에 반영
- P1 균형 정밀 이동 계약 사용자 승인 완료, VD-01·VD-04에 반영
- P1 Input System 1.20.0 단독·InputRouter·Gameplay/UI map 구조 사용자 승인 완료
- P1 포인터 조준형 횡스크롤 조작과 키보드·마우스/XInput 역할 배치 사용자 승인 완료
- P1 마우스 24px 직접 포인터와 gamepad 18° 획득·26° 유지 타겟 판정 사용자 승인 완료
- P1 UI·runtime rebind와 versioned profile 원자 저장·복구 계약 사용자 승인 및 Luna 독립 문서 검토 PASS
- P1 미술 18 PPU, 640×360 고정 16:9 frame, 최소 640×360 창과 2560×1440 4× 출력 사용자 승인
- P1 이동 예측형 fixed-zoom camera와 dead-zone·look-ahead·transition 수치 사용자 승인
- P1 640×360 pixel UI·final-output SDF text 혼합 배율과 font·hit-area·margin minimum 사용자 승인
- P1 world 21색·semantic 11색 역할 분리와 32개 exact sRGB HEX 사용자 승인
- P1 logical pixel 윤곽선의 정상·조준 포착·활성 전이 색·두께·우선순위 사용자 승인
- P1 URP 2D world/actor layer, 장면 preset, local light·shadow cap와 semantic unlit 규칙 사용자 승인
- P1 reticle 9×9/13×13px, 획득/해제 3/6 tick, 1px halo·상태 ring 사용자 승인으로 OD-ART-001 해결
- GLM QA 계획 초안과 MiniMax 부분 구현안을 GPT가 계약 교정하고 Luna가 독립 변조 검증해 VD-11을 Verified로 전환; 13 scenario가 VD-00~09의 68 AC를 전수 연결 (역사 기록; 현재 외부 모델 호출 없음)

> 권위 문서: [본격 개발 착수 전 준비도 점검](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/codex/documentation-checkpoint-20260928/docs/project-readiness-audit-2026-08-24.md)
