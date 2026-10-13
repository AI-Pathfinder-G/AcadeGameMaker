# Renderer2D 현재 사용 경로 v3 테라 보고

Approved 미리보기 장면 보정 계약에 따라 v2에서 NewPreviewScene/ClosePreviewScene 및 camera.scene 설정만 기능 변경하고 Unity를 한 번 실행했다. 실제 Renderer2D 카메라와 임시 2D 조명 장면, 지원되는 단일 카메라 요청의 제출, 콜백 각 1회 및 비검정 픽셀 16개를 관측했다. 다만 falloff 전역 값은 UnityBlack이므로 실제 lookup 소비나 2D 조명 효과를 단정할 수 없다(AC-PLAT-064 부분 관측). APV 여섯 객체는 로드됐지만 조건부 디버그 소비는 미관측이다(AC-PLAT-063 부분 관측). 원문534·후보534·후보505·실행본505·패키지 및 v1/v2 증거의 전후 SHA는 동일하다(AC-PLAT-065 보존 확인).

추가 실행·패치는 하지 않았다. 기존 Renderer7 P1, 게시·전체수용·의존 폐쇄 및 루나 독립 수용은 미완료다.
