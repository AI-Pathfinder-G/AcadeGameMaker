# 개발 환경 준비 상태

- Date: 2026-08-24
- Scope: 승인된 A1, A3

## 완료

- Unity Hub 3.21.0 공식 패키지 설치
- Unity Editor 6000.3.21f1 설치 및 `ProductVersion=6000.3.21f1_c02631ffc030` 검증
- Unity Editor 6000.5.9f1 추가 설치 확인. 프로젝트 생성 기준은 ADR-0010의 6000.3.21f1을 유지
- Ollama 계정 인증 상태 확인
- `glm-5.2:cloud` 비민감 고정 문장 호출 성공
- `minimax-m3:cloud` 비민감 고정 문장 호출 성공
- `kimi-k3:cloud` 모델 등록과 비민감 고정 문장 `KIMI_K3_OK` 호출 성공
- 로컬 `qwen3.8:latest` 준비 상태 유지
- Unity Hub 프로필 반영과 Unity Personal 라이선스 활성 표시를 UI에서 확인

## 확인 대기

- Hub 프로젝트 0개로, 저장소 Unity 프로젝트와 Unity Cloud 프로젝트는 아직 연결하지 않음
- Approved 스펙과 수직 데모 프로젝트 생성 승인

## 금지 상태 유지

- Unity 프로젝트를 생성하지 않음
- 에셋을 Unity에 임포트하지 않음
- IDE·아트·오디오 도구와 추가 플랫폼 모듈을 설치하지 않음
- Unity Analytics, Ads, Cloud 프로젝트를 연결하지 않음
