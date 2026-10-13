# 테라 미해석 참조·범위 밖 경로 분류 보고

승인 계약 `docs/specs/work-contracts/2026-10-02-unresolved-reference-owner-classification.md` (SHA-256 `5BAAAC79F141226C27E748DD8DBC8EBF8C9B31F93156BC86943A5D3726E6C21A`), `REQ-PLAT-063/064`, `AC-PLAT-061/062`. 조사 v1의 미해석 388참조·영향 37원본 경로와 범위 밖 고유 68경로를 정확 입력으로 분류했다. 예비 패키지 조사 291행은 새 집합에 합치지 않았다.

현 `Assets`, `Packages`, `Library/PackageCache`의 메타 8,425개와 조립 정의 183개를 읽었다. 미해석 GUID 참조 292건 가운데 캐시 메타·짝 파일 소유 266건, 검색 루트에서 미발견 26건/9값이다. 미해석 조립 정의 13건은 현 캐시의 실제 asmdef·meta·패키지 선언에 연결됐다. 발견 소유 279참조의 메타/자산/조립 정의 지문, 원문 위치, 패키지 선언 지문·캐시 버전·요청/잠금 버전을 JSON 각 행에 보존했다. 중복 소유 0건이며 캐시 소유는 이식 가능 증거가 아니다.

문자 경로 참조 83건과 범위 밖 고유 경로 68건은 실제 파일·디렉터리, 기록 인용, 동적/부분 문자열, 시험 생성물, 설정 기본값, 정규식 조각으로 구분했다. 공백 때문에 잘린 `Assets/UI/Resources/TMP`는 실제 `Assets/UI/Resources/TMP Settings.asset`로 복구해 지문과 534 포함을 확인했다. `Assets/Prefabs/Hub/__m5d7q0-override-base.prefab`는 시험 182행에서 생성·200행에서 삭제하는 생성 시험물이다. `Assets/Scenes/SampleScene.unity`는 현재 부재한 `templateDefaultScene` 값이며 실제 필수성은 미입증이다.

범위 밖 고유 경로 68개 중 정확히 534 후보에 이미 들어 있는 대상은 4개다. 참조 발생 기준 후보 대상은 9행이다. 필요성은 직접 읽기 구문이 같은 행에 있는 9참조도 실행 전까지 미입증으로 남겼다. 534 원문·후보 각각 시작·끝·최종 534행 지문이 동일하고 불일치 0이다.

`com.unity.render-pipelines.universal` 요청 `17.3.0`/잠금·캐시 `17.6.0`, `com.unity.test-framework` 요청 `1.6.0`/잠금 `1.8.0` 차이는 남아 있다. `Renderer2D` 일곱 참조의 기존 P1, Q0/깨끗한 복제 조건, 과거 14전이, 원격 충돌도 별도 관문이다. 미발견 9 GUID를 엔진 내장 또는 결함으로 판정하지 않았다.

자체 판정 `AC-PLAT-061=true`, `AC-PLAT-062=true`는 현재 소유 분류와 입력 지문 보존에 한정된다. 모든 참조 행의 `DependencyClosure`·`PublicationApproved`와 전체 수용·루나 검토·깨끗한 복제·이식 가능 증거는 거짓이다.
