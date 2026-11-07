# Unity 정상 실행·앱 접근 복구 인계 — 2026-10-25

최신 상태는 이 문서와 output/unity-official-route-20261025/report.md를 참조한다. 이전 인계와 실패·소진 기록은 당시 사실로 보존한다. 활성 제품은 H:/GPT-workspace/AcadeGameMaker/output/chapter1-playable-r1/project다.

## 해결된 사항

Git는 main4f011ae2e90f117db95cbaa1f58e4b091b405237에 기존 승인99파일을 원문 바이트로 저장했다. 기본 .git 읽기 제한을 제거한 것이 아니며 공식 require_escalated 승인 실행 경로를 사용한다. 저장소 로컬 .git/info/attributes의99경로 원문 보존 규칙이 있으므로 다른 복제 환경에는 자동 전달되지 않는다. 상세 output/git-access-recovery-20261025/report.md.

Unity도 이전에 공식 승인 실행 경로를 누락했다. 기본 제한 문맥의 프로세스3개/Unity0과 달리 정상 승인 읽기 실행은453개와 기존Hub·LicensingClient14120을 보았다. 동일 승인 문맥의 Unity 초기화는 기존 라이선스 채널에 즉시 연결하고 이용 권한·엔진·실제 스크립트 컴파일 및 자산 갱신을 마친 뒤53.666993초에 자연 종료0이다. 제약 실행 문맥에서의 IPC/WMI 실패를 실제 사용자 라이선스 미보유로 오인하지 않는다.

그 성공·회수·제품 불변 확인 후 정상 승인 문맥의 일반 Editor1회를 실행했다. 약관 창이 아닌 정상 작업창에서 CUA getApp2.2517초, 접근성·화면 조회3.0023초 성공을 관측했다. 이전에도 같은 공식 getApp API를 사용했으므로 앱 선택 API 자체를 빠뜨렸다는 결론은 아니다. 과거300초 승인 시간 초과의 유일한 내부 원인을 확정하지 않으며, 현재 실제 정상 작업창의 읽기 접근 복구를 확인한 것이다. 전역 앱 접근 설정 변경0이다.

약관 수락·로그인·라이선스 활성화/반환·키/파일/레지스트리 직접 조작·관리자/UAC·보안 설정 변경·화면 입력은0이다. 일반 Editor는 관측 후 stop.signal로64.2311835초에 강제 종료-1했다. 초기화의 자연 종료와 구분한다. 확인 자손은 초기화46개/일반Editor18개이며 사후 모든 확인 PID 부재와 로그폐쇄, 기존공유7개 신원 보존을 확인했다. 미관측 모든 자손 포획·공개 동기호출 강제 선점·CUA불투명창ID와PID직접결속은 미입증이다.

## 보존·검수

권위 preparation/unity-official-route-20261025/astra-scope-decision.md, 원시는 output/unity-official-route-20261025/의 init/gui, 사전·사후 검사, 정상문맥기준, 공유프로세스 보존, GUI관측 및 최종 식별자 기록이다. CUA 이미지는 도구 대화에 표시됐고 독립 이미지 파일은 없다. Luna 독립검수와 Astra 결과통합은 각 luna-final-review.md, preparation/unity-official-route-20261025/astra-result-decision.md의 실제 완료를 확인한다.

제품630/선별99/기존준비6/Ship·ZIP 차이0, 추가제품0, 현재 Git 추가커밋·병합0이다. 과거 기본실행 실패와 이전실행기 감지오류·보정 자료를 삭제하거나 성공으로 고치지 않는다.

## 다음 개발 경로

Unity의 시작·시험·빌드는 공식 require_escalated 승인 실행 문맥에 유한 소유 실행기를 둔다. 기본 제한 문맥의 조회 결과를 호스트 전체로 해석하지 않는다. 프로세스 동시성은 Hub의 unity 보조를 Editor로 오인하지 않도록 정확 Editor exe/게임 경로로 확인한다. 정상 승인 문맥에서 지원된 CIM/Process로 소유 출생·경로·부모를 확인하고 기존공유Hub/라이선스클라이언트는 종료하지 않는다.

이번 초기화/일반Editor관측 각1회는 소비·종료다. Unity 시험·빌드·게임 실행은0이며 추가 검사로 전용하지 않는다. 복구된 실제 환경과 현재 제품 지문을 결속한 새 한정 실행 범위에서 정확4 PlayMode 검사, 실제 통과 후 새 Windows 빌드, 동일 빌드 실제 화면·조작·CH1 양분기·실패재시도를 이어간다. 이전 실패/소진을 복원하지 않고 사용자에게 반복 Play·키·경로 입력을 요구하지 않는다. 실제 요구되지 않은 약관 동의나 로그인도 선행으로 다시 요구하지 않는다.

licenseInitializationResolved=true, unityAppReadAccessVerified=true. actualWindowAcceptance=false, presentationClearAccepted=false, finalAcceptance=false. 현재 기존exe/ZIP은 보정 소스 재빌드본이 아니며 정상 작업창 접근을 CH1 게임 완주로 표시하지 않는다.

후속 검수·통합 완료: Luna 결과 검수와 Astra 결과 판정 파일이 작성됐다. 초기화·앱 읽기 복구만 한정 수용했고 기존 관측 한계2건을 유지한다. 기본 환경 조회와 최초 승인 조회453개/후속 기준469개는 서로 다른 시점의 자료임을 initial-context-comparison.json에서 구분했다. 다음 개발 때 이미 불필요한 것으로 확인된 약관·로그인 선행을 다시 요구하지 않는다.
