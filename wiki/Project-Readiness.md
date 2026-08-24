# Project Readiness

2026-08-24 갱신 결과, P0와 이동·플랫폼 계약 반영은 끝났지만 **P1 두 항목과 독립 검토가 남아 Unity 프로젝트 생성·에셋 임포트·구현은 아직 시작할 수 없다.**

## 핵심 상태

- 구현 가능한 `Approved` 스펙: 0개
- 열린 P0 미결정: 0개
- 열린 P1 결정: 2개
- Unity Hub와 Unity Editor 6000.3.21f1: 설치·버전 검증 완료
- Git과 Git LFS: 준비됨
- 무료 에셋: CC0 원본 7개 격리 확보, 미임포트
- `main` 브랜치 보호와 Unity용 루트 무시·속성 규칙: 적용 완료

## 완료한 승인 범위

- Unity Hub와 Unity 6.3 LTS 6000.3.21f1 설치·버전 검증
- Unity 계정 로그인 완료 보고
- Ollama Cloud GLM 5.2와 MiniMax M3의 비민감 고정 문장 연결 시험
- Unity 프로젝트 생성 전 저장소 안전장치 적용
- Sol P0 기본안 사용자 승인과 ADR-0018·스펙 반영

IDE, 아트 편집기, 오디오 도구, 추가 플랫폼 모듈은 해당 작업이 시작될 때 설치한다.

## 다음 게이트

Sol이 해결된 `OD-PLAT-001`의 Luna 독립 검토를 마치고 `OD-ART-001`, `OD-SCENE-001`을 통합 결정한 뒤 관련 스펙을 `Approved`로 승격한다. 그 다음에만 Unity 프로젝트를 생성한다.

## 2026-08-24 진행 갱신

- 저장소 안전장치와 `pre-unity-docs-v1` 태그 적용 완료
- Unity Hub 설치 완료
- Unity Editor 6000.3.21f1 설치·버전 검증 완료
- Ollama Cloud GLM 5.2·MiniMax M3 연결 시험 완료
- Unity 계정 로그인 완료를 사용자가 보고함; Personal 라이선스 활성 상태의 기계 검증은 별도 확인
- Sol P0 통합안 사용자 승인 완료, ADR-0018과 규범 스펙에 반영
- P1 균형 정밀 이동 계약 사용자 승인 완료, VD-01·VD-04에 반영
- P1 Input System 1.20.0 단독·InputRouter·Gameplay/UI map 구조 사용자 승인 완료
- P1 포인터 조준형 횡스크롤 조작과 키보드·마우스/XInput 역할 배치 사용자 승인 완료
- P1 마우스 24px 직접 포인터와 gamepad 18° 획득·26° 유지 타겟 판정 사용자 승인 완료
- P1 UI·runtime rebind와 versioned profile 원자 저장·복구 계약 사용자 승인 및 Luna 독립 문서 검토 PASS
- P1 미술 18 PPU, 640×360 내부 canvas와 2560×1440 4× 출력 사용자 승인; aspect·camera·palette·lighting 결정 대기

> 권위 문서: [본격 개발 착수 전 준비도 점검](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/project-readiness-audit-2026-08-24.md)
