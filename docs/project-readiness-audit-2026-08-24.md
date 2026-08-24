# 본격 개발 착수 전 준비도 점검

- 점검일: 2026-08-24
- 점검 범위: 설계 게이트, 개발 환경, 저장소 안전장치, 모델 운영, 에셋·라이선스
- 판정: **P0와 이동·플랫폼 P1 반영 완료 / P1 두 항목과 스펙 승인 전까지 Unity 프로젝트 생성·임포트·구현 보류**

## 1. 착수 게이트 판정

| 항목 | 현재 상태 | 판정 |
|---|---|---|
| 권위 문서와 위키 | 캐논·ADR·스펙·위키 구조 존재 | 준비됨 |
| 구현 승인 스펙 | `Approved` 0개 | 차단 |
| 미결정 | P0 0개, P1 2개 | 차단 |
| Unity 프로젝트 | 아직 생성하지 않음 | 정상 |
| 무료 에셋 | Kenney CC0 원본 7개 격리 확보, 검증됨, 미임포트 | 준비됨 |
| 저장소 | 공개 저장소, Unity 안전장치와 기준 태그 적용 | 준비됨 |
| 브랜치 보호 | `main` 강제 푸시·삭제 금지 | 준비됨 |
| Unity용 `.gitignore`/`.gitattributes` | 루트 적용 완료 | 준비됨 |

구현은 스펙 상태가 `Approved`가 된 뒤에만 시작한다. 현재 허용 범위는 도구 설치, 계정 인증, 저장소 안전장치 마련, 설계안 작성과 검토다.

## 2. 확인된 장비와 설치 상태

| 분류 | 확인 결과 | 판단 |
|---|---|---|
| OS | Windows 11 64-bit | Unity 6 요구 조건 충족 |
| CPU/RAM | Unity 6 요구 조건 충족 | 충분 |
| GPU | Unity 6 요구 조건 충족 | 충분 |
| 저장 공간 | 설치 가능 | 에디터·캐시·빌드 공간 관리 필요 |
| Git | 설치 및 동작 확인 | 준비됨 |
| Git LFS | 설치 및 동작 확인 | 준비됨 |
| Unity Hub/Editor | Hub와 Editor 6000.3.21f1 설치·버전 검증 완료 | 준비됨 |
| IDE | Visual Studio/VS Code/Rider 미설치 | 선택 설치 |
| .NET SDK | 미설치 | Unity 시작에는 별도 설치 불필요 |
| 2D 편집 도구 | Aseprite/Krita/GIMP 미설치 | 아트 파이프라인 결정 후 설치 |
| 오디오 도구 | Audacity/FFmpeg 미설치 | 오디오 작업 시작 때 설치 |

## 3. 지금 요청할 설치·인증 승인

### A1. 필수 권고 묶음

1. Unity Hub 설치
2. Unity Editor **6000.3.21f1 (Unity 6.3 LTS)** 고정 설치
3. Unity 계정 로그인과 Unity Personal 라이선스 활성화

프로젝트가 비출시 수직 데모이므로 Android, iOS, WebGL, 서버 빌드 모듈과 Unity 클라우드 서비스는 설치·연결하지 않는다. Windows IL2CPP 모듈도 실제 배포 빌드 요구가 생길 때까지 보류한다.

### A2. 선택 권고 묶음

- IDE: 기본 권고는 **Visual Studio Community 2022 + Game development with Unity**다. 사용자가 코드를 직접 열지 않는다면 초기 설치를 보류해도 된다.
- 로컬 2D 도구: 아트 규격 승인 후 설치한다. 유료 Aseprite 구매는 별도 승인 대상으로 남긴다. 무료 대안은 Krita 또는 LibreSprite다.
- 오디오 도구: 첫 오디오 제작/편집 작업 때 Audacity와 FFmpeg를 설치한다.

### A3. Ollama Cloud 인증

- 로컬 Ollama 0.32.15와 `qwen3.8:latest`는 준비되어 있다.
- `glm-5.2:cloud`, `minimax-m3:cloud`는 아직 계정 인증과 실호출을 검증하지 않았다.
- 사용자가 Ollama 계정 로그인을 승인한 뒤, 비밀정보나 로컬 민감 자료를 포함하지 않은 고정 테스트 문장으로 각 모델을 1회 검증한다.
- 설치되어 있는 다른 로컬 모델은 삭제할 필요가 없지만 이 프로젝트 작업에는 배정하지 않는다.

GPT-5.6 Sol/Terra/Luna는 Codex가 제공하는 모델을 사용하므로 별도 로컬 설치가 필요 없다. 프로젝트의 현재 역할 분담은 OpenAI의 공식 모델 포지셔닝과도 일치한다.

## 4. 프로젝트 생성 전에 사용자가 결정할 항목

### 즉시 결정이 필요한 제품 기준

| ID | 결정 | 권고 기본값 |
|---|---|---|
| D-PLATFORM | 대상 플랫폼 | Windows 10/11 x64 전용 비출시 수직 데모 |
| D-INPUT | 수직 데모 입력 | 키보드·마우스 포인터 조준과 XInput twin-stick 의미 대응 |
| D-DISPLAY | 화면 기준 | 16:9, 2560×1440 출력 기준, 창 모드/전체 화면 전환 |
| D-FRAME | 프레임 기준 | 60 FPS, 물리 고정 간격 1/60초 |
| D-PRIVACY | Unity 온라인 기능 | Analytics, Ads, Cloud, Diagnostics 비활성 기본 |
| D-INSTALL | 설치 위치 | Hub는 기본 위치, Editor는 여유 공간이 큰 드라이브, 프로젝트·Library는 현재 작업공간 유지 |

### 해결된 P0 창작·게임플레이 결정

P0 여섯 항목은 2026-08-24 사용자 승인과 [ADR-0018](./adr/0018-vertical-demo-p0-integration.md)로 해결됐다.

1. `OD-WT-001` — 무게 전이 사거리, 조준, 단일/복수 대상, 회수, 쿨다운, 상자·적 배율
2. `OD-ROOM-001` — 6개 방 풀에서 런당 방 수, 샘플 시드, 반복 제한
3. `OD-RUN-001` — 실패 조건과 유지/소실 표, 세션 재시작 경계
4. `OD-CHOICE-001` — 추출/연대 선택의 대상 인간, 시점, 장면 순서
5. `OD-CHOICE-002` — 두 선택이 주는 구체 스킬, 입력, 데모 수치
6. `OD-COM-001` — 두 적 역할과 미니보스의 정체·패턴·성공/실패 조건

이 중 사용자가 직접 정할 핵심은 `OD-CHOICE-001`, 두 스킬의 판타지 방향, 적·보스의 정체다. 수치·쿨다운·반복 규칙은 Sol이 기본안을 만들고 사용자에게 묶음 승인받는 방식이 적합하다.

P1 가운데 `OD-MOV-001`과 `OD-PLAT-001`은 해결됐고 `OD-ART-001`의 18 PPU·2560×1440 출력 기준은 부분 확정됐다. 다음 두 항목은 스펙 Approved 전환 전에 마저 정리한다.

- `OD-ART-001` — 픽셀 밀도·팔레트·카메라·조명
- `OD-SCENE-001` — 보스→선택→히로인 장면 흐름과 실패 시 노출 범위

## 5. Unity 프로젝트 생성 전 저장소 안전장치

다음 항목을 먼저 적용하는 것을 권고한다.

1. 현재 문서 상태에 `pre-unity-docs-v1` 기준 태그 생성
2. Unity 표준 루트 `.gitignore` 추가
3. Unity YAML 텍스트 직렬화와 병합 기준을 담은 `.gitattributes` 추가
4. 런타임 PNG/OGG는 일반 Git, 편집 원본 PSD/KRA/ASEPRITE와 대형 WAV만 Git LFS로 관리
5. `main` 브랜치 강제 푸시·삭제 금지 보호 설정
6. 프로젝트 생성 뒤 첫 커밋 전에 Packages와 ProjectSettings 차이를 Sol이 검토

공개 저장소이므로 Unity 계정 토큰, Ollama 인증정보, 개인 경로가 포함된 설정 파일은 절대 커밋하지 않는다.

## 6. 권고 착수 순서

1. A1, A3, 제품 기준 승인 — 완료.
2. 저장소 안전장치와 기준 태그 — 완료.
3. Unity Hub와 6.3 LTS 설치·버전 검증 — 완료.
4. P0 기본안 사용자 승인과 규범 스펙 반영 — 완료.
5. 남은 P1 두 항목 결정, 해결된 플랫폼 계약의 Luna 독립 검토, Sol의 Approved 전환 — 다음 게이트.
6. 그 다음에만 Unity 프로젝트 생성, 패키지 고정, 에셋 임포트, Terra 단위 구현을 시작한다.

## 7. 승인 문구 권고

다음처럼 한 번에 승인할 수 있다.

> A1과 A3, 권고 제품 기준 전체, 저장소 안전장치 적용을 승인한다. IDE·아트·오디오 도구는 보류한다. P0는 Sol이 기본안을 먼저 제출하라.

## 근거 문서

- [문서 지도](./README.md)
- [에이전트 운영 모델](./agent-operating-model.md)
- [수직 데모 스펙 색인](./specs/vertical-demo/00-spec-index.md)
- [미결정 목록](./specs/vertical-demo/OPEN-DECISIONS.md)
- [시스템 계약](./specs/vertical-demo/SYSTEM-CONTRACTS.md)
- [에셋 등록부](./assets/asset-register.md)
- [Unity 6000.3.21f1 릴리스](https://unity.com/releases/editor/whats-new/6000.3.21f1)
- [Unity 6 지원 일정](https://unity.com/releases/unity-6/support)
- [Unity Hub 설치 안내](https://docs.unity3d.com/es/current/Manual/GettingStartedInstallingHub.html)
- [OpenAI 모델 목록](https://developers.openai.com/api/docs/models)
- [OpenAI 모델 선택 가이드](https://developers.openai.com/api/docs/guides/latest-model)
