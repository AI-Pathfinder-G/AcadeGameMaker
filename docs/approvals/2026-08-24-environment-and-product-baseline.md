# 환경 준비 및 제품 기준 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Scope: A1, A3, 권고 제품 기준, 저장소 안전장치

## 승인 내용

- Unity Hub 설치
- Unity Editor 6000.3.21f1 설치
- Unity 계정 로그인 및 Personal 라이선스 활성화 준비
- Ollama Cloud GLM 5.2와 MiniMax M3 비민감 연결 시험
- Windows 10/11 x64 비출시 수직 데모
- 키보드와 XInput 게임패드 지원, 마우스는 UI 및 조준 입력에 사용 가능
- 16:9, 2560×1440 출력 기준, 창 모드/전체 화면 전환 — 2026-08-24 후속 사용자 승인으로 기존 1920×1080 기준 대체
- 60 FPS, 물리 고정 간격 1/60초
- Unity Analytics, Ads, Cloud, Diagnostics 기본 비활성
- 기준 태그, Unity `.gitignore`, `.gitattributes`, LFS 정책, main 강제 푸시·삭제 금지

## 보류 내용

- IDE
- 아트 편집 도구
- 오디오 편집 도구
- Android, iOS, WebGL, 서버 및 Windows IL2CPP 추가 모듈
- Unity 프로젝트 생성과 에셋 임포트

## 적용 상태

- `pre-unity-docs-v1` 기준 태그 생성 완료
- Unity 루트 무시 규칙과 YAML/LFS 속성 규칙 적용 완료
- `main` 강제 푸시·삭제 금지 적용 완료
- Unity Hub 설치 완료
- Unity Editor 6000.3.21f1 설치와 제품 버전 검증 완료
- Unity 계정 인증은 별도 설치 기록에서 완료 여부를 남긴다.

이 기록은 사용자 승인 출처를 보존한다. 실제 Unity 프로젝트 설정과 저장 스키마·복구의 규범 요구사항은 해결된 `OD-PLAT-001`에 따라 VD-07·VD-09에 반영됐다.
