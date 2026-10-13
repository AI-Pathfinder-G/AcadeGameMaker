# Renderer2D 조명 대조 v5 테라 보고

승인된 SceneHandle 보정 계약에 따라 v4 소스·메타를 바이트 그대로 지정 history 경로로 옮겼고, v5에서는 클래스·결과 이름, 장면 핸들 GetRawData 다섯 호출과 실제 UInt64 반환에 맞는 DTO 선언만 바꿨다. Unity 6000.6.0f1을 한 번 실행했다(종료 0).

Sprite-Lit-Default 재질 GUID/fileID와 shader GUID/fileID가 설치 패키지 및 실제 SpriteRenderer.sharedMaterial에 정확히 결속됐다. Universal2D 패스 태그를 패키지 원문에서 확인했다. 카메라·Sprite·두 조명이 같은 미리보기 장면에 있고, 읽기 전용 목록에 조명 두 개가 등록됐으며 컬링 마스크에 포함된다(AC-PLAT-071 해당 관측). 점 조명 켜짐·꺼짐 요청은 각 한 번 제출되고 카메라 콜백은 각 프레임마다 관측됐지만 RGBA SHA가 동일하고 차이 픽셀은 0이다. 따라서 점 조명 영향, useLights, falloff lookup 소비는 미입증이다(AC-PLAT-072 미충족). 후속 참고 전역 텍스처 UnityBlack은 내부 패스의 결론이 아니다.

원문534·후보534·후보505·실행본505, 캐시·설정, 기존 결과, 이동한 v4 바이트 전후 SHA 일치: True. 추가 패치·재실행은 없다. Renderer GUID7 P1, 전체 의존 폐쇄·게시·전체 수용·루나 독립 수용은 미완료다.
