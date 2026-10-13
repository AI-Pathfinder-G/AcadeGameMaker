# 501파일 격리 Unity 가져오기 진단 — 테라 보고

- 담당: 테라 구현 역할 (gpt-6-sol), 독립 검수 아님
- 계약: REQ-PLAT-032..034 / AC-PLAT-030..032
- 원본 사전/사후 501/501, 501/501; 복사본 실행 전/후 501/501, 501/501
- 초기 후보 밖 복사 0개; Unity 생성 18873개, 지문 오류 0개
- Unity 실행 1회, 종료코드 0, 로그 SHA-256 9DEA1F0C823E9A9441987333C3959F65A0B9810349105DF5B6619BEF26F8B99A
- 상태: 종료코드 0·C# 컴파일 오류 미검출·패키지 자산 가져오기 실패 진단 동반
- 컴파일 오류 진단 0개, 경고 줄 10개. 7521~7542줄 패키지 TraceRenderingLayerMask.urtshader 가져오기 오류와 7624줄 Curl error 42를 별도 보존한다.
- Renderer2D 일곱 GUID의 로그 문자열 부재는 해석 성공 증거가 아니다.
- 상세: artifacts/c4-initial-publication-provisional-501-unity-import-v1.json, 같은 이름 .md, .log

## 자체 확인

- AC-PLAT-030: 충족
- AC-PLAT-031: 충족
- AC-PLAT-032: 충족

## 미완료 게이트

파일별 게시 결정, 깨끗한 Git 복제, 독립 검수와 전체 수용은 남았다. PublicationApproved=false; DependencyClosure=false; WholeAccepted=false; LunaReviewed=false; CleanCloneExecuted=false.

## 패키지 자산 오류의 해석 경계

- DirectoryNotFoundException 40줄, Asset import failed 13줄, IOException 6줄을 로그 원문 위치와 함께 기록했다. 3562줄과 7207줄이 대표 사례다.
- 깊은 격리 경로의 길이·위치 영향 가능성이 있으나 짧은 경로 재현을 수행하지 않아 원본 프로젝트 결함으로 단정하지 않는다. 종료코드 0으로 가져오기 오류를 소거하지 않는다.
