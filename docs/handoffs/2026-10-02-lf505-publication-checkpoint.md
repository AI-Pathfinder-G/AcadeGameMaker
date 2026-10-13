# 저장소 바이트 505개 후속 작업 인계

사용자의 2026-10-02 두 추천안 선택을 [결정 기록](../approvals/2026-10-02-publication-byte-and-qa-scope-decision.md)과 ADR-0038에 반영했다. 현재 원본501과 추가 QA 입력4는 보존하고, Git 줄바꿈 기준의 새 후보505를 `output/publication-lf505`에 작성했다. CRLF→LF 전이18·그대로487이며 [기계 대조](../verification/2026-10-02-publication-lf505-astra-check.md)에서 505개 지문/객체 불일치0이다. 후보 QA 일반·자체 검사는 각각 종료코드0이다.

별도 `output/u505-r1`의 [Unity 진단](../verification/2026-10-02-lf505-unity-import-astra-check.md)은 실제 자산 새로고침·패키지 해석에 도달했고 종료코드0·패키지 자산 가져오기/C# 오류 미검출·505개 변경0이었다. 라이선스 토큰·캐시·종료 통신 오류는 보존한다. 최초 긴 경로 오류의 유일 원인을 확정하지 않는다.

[Renderer2D 실제 로드 진단](../verification/2026-10-02-renderer-guid7-loaded-probe-astra-check.md)의 첫 조회 코드 실패는 v1 증거에 보존했다. 승인된 현재 API 보정 뒤 별도 한 번 조회는 종료코드0이며, 일곱 GUID 경로 모두 빈 문자열·여섯 참조 속성은 객체null·마지막 속성은 부재다. 게임 무영향 또는 자산 결함의 전체 판정은 하지 않고 기존 P1을 유지한다. 원본/새 후보/실행본505는 모두 불변이다.

[최신 원격·v19 범위 대조](../verification/2026-10-02-lf505-remote-and-v19-scope-precheck.md)는 HEAD `e95bb5169ad2ed08106bd342915d4637a1241263`와 잘림 없는 트리를 새 UTC 조회에 결속했다. 후보505의 원격 존재10·부재495·저장소 바이트 동일7·내용 차이3이다. 과거 전체 검증 입력1084와 비교하면 후보와 원격 모두에 없는 입력523개가 약410MB다. 모두를 제품 필수로 보거나 자동 포함하지 않는다.

사용자가 후속 판단을 위임하여 [코드·필수 검증 입력 우선 결정](../approvals/2026-10-02-code-first-publication-scope-decision.md)을 적용했다. [Approved 선별 계약](../specs/work-contracts/2026-10-02-code-first-input-selection.md)에 따라 테라 역할의 별도 `gpt-6-sol`이 523개 현재 바이트와 실제 의존을 조사한다. 음원·영상은 실제 필수 읽기 근거가 있는 항목만 검토하고 역사 문서·증거를 일괄 포함하지 않는다. 통상 기술 선택을 사용자에게 다시 묻지 않는다.

[선별 기계 대조](../verification/2026-10-02-code-first-input-selection-astra-check.md)에서523행·37참조 변의 SHA/줄 불일치0을 확인했다. 선별후보29개·보류488개·미입증6개이며, 별도534 후보（`../../artifacts/c4-publication-lf534-baseline-v1.json`；로컬 보존 자료）에 정확505+29를 복사했다. 주 에이전트 대조에서도534집합·현재 원본/후보SHA 불일치0이다. 추가7개 LF 전이는 역사 내부pin을 바꾸지 않는다. v1은 보존하고 [변환수 필드 보완](../specs/work-contracts/2026-10-02-lf534-transition-count-supplement.md)의 새v2와 [엄격 검사기](../specs/work-contracts/2026-10-02-canonical-baseline-validator.md)를 진행한다.

[최종 검사기 기계 대조](../verification/2026-10-02-canonical-baseline-validator-astra-check.md)까지 완료했다.534 v2 원장 SHA는 `89BC3D6EFDD2B1344E8E6F4F74E819382F7AB2F9C67EBFE373B3BF62F4D26F9E`, 검사기 최종SHA는 `80B277304ADA6C06CCEAC2325D258047E60366CB1579F3AE7C635D127225D7B5`다. 정상2·음성8 통과, 최종 도구의505/534 대조 종료0, 실행 전후2067 경계행·메타7 지문 차이0이다. 최초 결과의 실행 전 기록 공백은 그대로 보존하고 별도v2 재검사로 보완했다. 새 검사기는534 후보 집합 밖의 신규 QA 도구이며 자동 게시 목록에 추가하지 않았다.

다음은 [정규화 회귀 보완 초안](../proposals/2026-10-02-canonical-regression-freeze-design-v2.md)과 [역사 보존 관문](../approvals/2026-10-02-canonical-regression-preserve-history-gate.md)이다. 기존 fixture2개 외 감사 시험2개 및 새 핀/QA 결속이 필요하지만 현재 Draft를 임의 구현하지 않는다. 루나의 미완료 독립 관문을 먼저 확인하고 정확 후속 Approved 계약을 발급한다. 사용자에게 같은 기술 선택을 다시 묻거나 한도 오류를 반복 호출하지 않는다. 원격 코드 게시·전체 후보 수용은 여전히 미완료다.

솔 역할의 별도 `gpt-6-sol`은 [Renderer2D 처리 초안](../proposals/2026-10-02-renderer2d-legacy-reference-disposition.md)을 작성했다. 현재 자산을 유지한 격리 검증을 우선하며 정적 공급 경로를 게임 무영향 또는 P1 폐쇄로 확대하지 않는다. C4 시험의 과거 원장·하드코딩 지문과 새 LF 바이트가 충돌하므로 과거 증거를 보존하는 새 엄격 동결 설계를 따로 진행한다. 원격 코드 게시·파일별 허용 목록·깨끗한 복제·제품 시험/빌드·루나 독립 수용은 아직 없다. 루나 검수는 기존 사용자 지시대로 한도 회복 뒤 별도 수행한다.
