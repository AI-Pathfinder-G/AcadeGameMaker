# Renderer2D 현재 사용 경로 v3 독립 검수

대상 승인 계약 `docs/specs/work-contracts/2026-10-02-renderer-current-use-v3-independent-review.md`. v3 JSON SHA-256 `E356B16DB0AC5D77B6512D35C7CB078741ABCEABEAB0BCA09A67FDB0DD1C48F1`, probe 원문 SHA-256 `E65766FE371838D78B0B04347C87A681EE6B35BC1B619ADB7FB427DBA88708E7`, 테라 보고 SHA-256 `0682C7BD3FBEE27CBEC99DF9E13689CB2F24615B2DB12B1EF7CC8F4622B2B705`를 확인했다. 읽기·해시 대조만 수행했고 Unity/시험/Git/네트워크 실행은 하지 않았다.

## 판정

- **P0: 0. P1: 기존 Renderer GUID7/falloff 의존 P1 유지, 종결 아님.** 요청 제출 및 비검정 픽셀은 성공했지만 `_FalloffLookup`은 카메라 콜백 때와 요청 후 모두 `UnityEngine.Texture2D:UnityBlack`이다. 실제 lookup 소비, 조명 효과 또는 GUID7의 무영향을 결론내릴 근거가 없다. 결과의 `RendererGuid7P1Closed`, 게시/의존 폐쇄/전체 수용 플래그도 모두 거짓이다.
- **AC-PLAT-063 — 부분 관측.** 활성 URP 전역 설정과 APV 여섯 객체의 로드 경로·GUID·파일ID가 기록됐다. 조건부 APV 디버그 경로의 실제 소비는 확인되지 않았다.
- **AC-PLAT-064 — 부분 관측.** 활성 파이프라인/단일 Renderer2D와 카메라 Renderer2D, 미리보기 장면, 전역·점 Light2D 및 Sprite 생성, 단일 카메라 요청 지원·제출, 시작/종료 콜백 각 1회, 32×32 출력의 비검정 픽셀 16개·색 2개를 원시 관측에 대조했다. falloff는 UnityBlack이므로 실제 조명 lookup 사용은 미입증이다.
- **AC-PLAT-065 — 제한 범위 충족.** 허용 v2→v3 소스 차이는 probe 클래스명·결과 접미사 변경, `NewPreviewScene`/`ClosePreviewScene`, `camera.scene=scene`만이다. 이 네 변경을 역적용한 v3 소스는 v2 바이트와 일치했다. 보호 파일·패키지·v1/v2 입력·v3 소스 사본/실행본/meta·로그 등 35 지문을 실제 파일과 대조해 불일치 0, 사전/사후 보호 맵 차이 0을 확인했다. 실행은 Unity 6000.6.0f1 단 1회, 종료 0, 프로브 오류/경고/실패 없음이며 원시 관측 SHA와 로그 SHA도 기록값과 일치한다. 로그에는 별도 라이선스 토큰 갱신 오류 진단이 있으나 실행 종료 결과는 0이다.

## 다음 진단에서 읽어 확인할 원인 후보

1. `Renderer2DRendergraph.cs:929-934`는 `layerBatches[i].lightStats.useLights`를 합쳐 `GlobalPropertiesPass.Setup`에 넘긴다. `GlobalPropertiesPass.cs:28-35`는 `useLights`일 때만 falloff 전역 텍스처를 렌더그래프 pass 뒤에 설정한다. 현재 기록은 해당 batch의 `useLights` 값을 담지 않았다. 실제 light culling/batch와 이 조건의 참·거짓을 관측해야 한다.
2. probe `PublicationRendererCurrentUseProbeV3.cs:170-176`은 `SpriteRenderer`에 Sprite만 넣고 머티리얼은 지정하지 않는다. `GraphicsSettings.asset:39`의 기본 Sprite 머티리얼, `Renderer2D.asset:65-68`의 Lit 기본 유형/머티리얼, 실제 선택된 Sprite 셰이더를 대조해야 16개 픽셀이 조명 적용 출력인지 구별할 수 있다.
3. probe는 `:65` 카메라 시작 콜백과 `:214` 요청 뒤에 `Shader.GetGlobalTexture`를 읽는다. URP `GlobalPropertiesPass.cs:31-35`는 RenderGraph pass 종료 시 전역 텍스처를 설정하므로 이 두 외부 읽기만으로 pass 내부 소비나 복원 이후 상태를 판정할 수 없다. 실제 pass 소비 시점의 바인딩과 Sprite-Lit 셰이더 입력을 별도 증거로 확인해야 한다.

이 세 항목은 소스 경로에서 나온 후속 진단 후보이며 현재 원인의 확정이 아니다. 이 보고는 전체 의존 폐쇄·게임 회귀·깨끗한 복제·게시 권한을 수용하지 않는다.
