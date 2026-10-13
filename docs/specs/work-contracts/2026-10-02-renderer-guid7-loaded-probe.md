# Renderer2D 일곱 참조의 로드 상태 진단

- 상태: **Approved**, 2026-10-02 아스트라 역할 승인. [505개 Unity 진단](2026-10-02-lf505-unity-import-diagnostic.md)이 실제 가져오기·컴파일에 도달했고 후보 505개가 변하지 않은 경우에만 실행한다. 기존 일곱 GUID 미해석 P1과 [독립 검수](../../verification/2026-09-29-c3-renderer2d-guid7-luna-review.md)는 그대로 보존한다.
- 범위: 원본은 읽기 전용. 이미 격리된 `output/u505-r1`의 `Assets/Settings/Renderer2D.asset`에서 YAML 20~25·64줄의 일곱 GUID/fileID와 현재 Unity가 로드한 자산의 직렬화 속성을 조회한다. 새 제품 구현·자산 수정·빌드·원격 게시는 아니다.

## 계약

테라는 실행본 안에만 `Assets/Editor/PublicationRendererGuidProbe.cs`와 Unity가 생성할 해당 폴더/소스 `.meta`를 진단용으로 추가할 수 있다. 진단 소스는 새 원장에 SHA와 전체 내용 또는 정확 파일 참조를 보존하고 후보 505개와 구별한다. 제품 소스·설정·렌더러·패키지 파일을 변경하거나 과거 증거를 덮어쓰지 않는다.

진단 메서드는 `AssetDatabase.GUIDToAssetPath`로 일곱 원문 GUID를 조회하고, 원문 asset을 `LoadMainAssetAtPath`로 로드해 실제 형식·경로·GUID/localFileID를 기록한다. `SerializedObject.FindProperty`로 `probeVolumeResources.probeVolumeDebugShader`, `probeVolumeResources.probeVolumeFragmentationDebugShader`, `probeVolumeResources.probeVolumeOffsetDebugShader`, `probeVolumeResources.probeVolumeSamplingDebugShader`, `probeVolumeResources.probeSamplingDebugMesh`, `probeVolumeResources.probeSamplingDebugTexture`, `m_FallOffLookup`를 조회한다. 속성 부재·타입·ObjectReference 실제 객체/null·instanceID·대상 경로/형식/GUID/localFileID를 구별한다. 참조 속성이 아니면 해당 사실만 기록한다. 현재 loaded asset의 `AssetDatabase.GetDependencies`도 기록하되 YAML 참조 해석이나 사용 증명으로 확대하지 않는다.

`SaveAssets`, `ApplyModifiedProperties`, 재직렬화, 강제 가져오기, 필드 대입, 장면/렌더러 생성, 플레이, 빌드, 패키지 변경은 금지한다. Unity `6000.6.0f1`를 기존 격리 실행본에서 `-batchmode -nographics -quit -executeMethod PublicationRendererGuidProbe.Run`으로 한 번만 실행하고 실제 진단 JSON·로그·버전·UTC·종료값을 남긴다. 필요한 실행 권한은 `require_escalated` 심사를 이용하며 거절·실행 실패 시 우회/재시도하지 않는다. 10분 시간 초과 시 시작한 Unity만 종료한다. 일곱 속성 부재나 GUID 빈 경로는 무영향·자산 누락·게시 허용을 뜻하지 않는다.

허용 출력은 격리 실행본의 위 진단 소스/meta와 Unity 생성물, `artifacts/c4-renderer-guid7-loaded-probe-v1.json`, 같은 이름 `.md`·`.log`, `docs/verification/2026-10-02-renderer-guid7-loaded-probe-terra-report.md`다. 원본 505·정규화 후보 505·실행본의 후보 505 지문을 전후 검사한다. 모든 결과 행/전체의 게시·폐쇄·전체/루나·깨끗한 복제 플래그는 거짓이다. 루나 검수는 사용자 지시대로 한도 회복 뒤 별도다.

| 요구 | 인수 기준 |
| --- | --- |
| `REQ-PLAT-044`: 정확 일곱 YAML 참조와 실제 로드 상태를 구별한다. | `AC-PLAT-042`: 일곱 고유 행에 원문 위치/GUID/fileID, AssetDatabase 경로, 속성 존재/타입, 실제 참조 대상 또는 미관측 사유가 있다. 빈 경로와 없는 속성을 정상 해석으로 쓰지 않는다. |
| `REQ-PLAT-045`: 조회 진단만 실행하고 입력을 보존한다. | `AC-PLAT-043`: 원본/정규화/실행본 후보 505 지문 전후 차이 0, 진단 소스·생성물은 별도 분류, 실제 한 번 실행·UTC·버전·종료·로그 SHA와 오류를 기록한다. |
| `REQ-PLAT-046`: 관측과 수용 경계를 유지한다. | `AC-PLAT-044`: 코드·JSON·보고의 미관측/실패 구분이 일치하고 게시·제품·독립 수용은 모두 미완료다. 기존 P1 차단은 아스트라/루나의 별도 판정 전까지 유지한다. |

테라 역할 `gpt-6-sol`이 진단 소스와 자체 보고를 작성하고 아스트라가 기계 대조한다.
