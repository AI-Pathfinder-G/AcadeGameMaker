# 1챕터 직접 플레이 준비 인계

작성일은 2026-10-14이며, 최우선 목표는 사용자가 현재 소스의 로컬 빌드에서 본편 1챕터를 직접 체험하고 완주하는 것이다. 이번 인계는 구현 완료 선언이나 새 실행 승인이 아니다. 사용자는 게임이 수신한 지정 입력만 기록하고 직접 플레이하는 방식을 선택했으며, 실제 플레이 시점은 **나중으로 연기**했다.

## 재개 지점

현재 Unity와 관측 관리자는 모두 종료했다. 마지막 관리자는 세션 `83540`, Unity는 PID `80452`였으며, 원래 프로세스 종료와 Dispose를 확인했다. STOP는 원래 핸들에 의한 종료이며 정상 저장 종료로 표현하지 않는다. 현재 살아 있는 세션으로 재사용하면 안 된다.

실제 Unity 시작은 누적 **4회**다. 첫 관리자 컴파일 실패는 Unity 시작 0회였고, 이후 입력 EOF, 제한 환경의 편집기 충돌, 보정 전 정상 로컬 관측, 보정 후 정상 로컬 관측을 구분해 보존했다. 마지막 실행은 첫 실패 없음, 전체 206.425428초, 종료 관측 0.2504809초, 관리자 종료 0이었다.

첫 방 장면 로드·사례 디렉터리 생성·RESERVE·영상 자식·Play·실행 권위 파일 발급은 모두 **0**이다. SourceReady는 false이며 B1 FINAL과 실제 10사례, CH1 완주, 독립 플레이 수용은 미완료다. 이 상태를 구현 진척 퍼센트나 전체 게임 수용으로 환산하지 않는다.

## 먼저 읽을 문서

1. `AGENTS.md`, `docs/README.md`, `CONTEXT.md`, `docs/agent-operating-model.md`.
2. ADR-0039·0041·0042·0043과 Approved인 `docs/specs/work-contracts/2026-10-03-play-first-delivery-strategy.md` 및 `2026-10-03-chapter1-playable-delivery.md`.
3. 동결 G3 `docs/specs/work-contracts/2026-10-08-b1-basic-movement-observation-and-acceptance.md`와 M1 호환 `2026-10-08-m1-bounded-development-compatibility.md`. M1의 현재 끝 조항은 §10.10.25다.
4. `docs/verification/gpt-participation-ledger.md`와 아래 로컬 증거의 지정 항목. 오래된 인계서 전체를 현재 지침으로 복구하지 않는다.

문서 게시본은 저장소의 LF 정책에 따른 열람본일 수 있다. 실제 실행에는 아래 작업공간 원문의 길이·SHA와 정확 입력 판정이 필요하다. 게시 SHA를 역사 실행 핀에 대입하지 않는다.

## 로컬 산출물과 정확 원문

기준 폴더는 `C:\Users\me\Documents\GPT-workspace\AcadeGameMaker`다. 아래 SHA는 이 작업공간의 원문 기준이다.

| 산출물 | 길이 | SHA256 |
|---|---:|---|
| G3 동결 계약 | 62045 | `39C64FF77AA6F8B89EAEAD430C0BAE5B19E780F9842182F37CDA61B29E8E222F` |
| M1 호환 계약 | 246065 | `BFEFA453A99486D6C229CE8DF9DF724934E2ADB806556875C5F25354AB835CD3` |
| `output/chapter1-playable-r1/evidence/b1-final-source-checkpoint-sol61.v14.json` | 125747 | `579F64500359B96F0C8B703711C89BE5F9FF2A85A2DB996C42C336B73067882F` |
| `output/chapter1-playable-r1/tools/b1-window-video-capture.v1.cpp` | 71632 | `C3C12E0EF91E2A9F2F19FCFE8BC82F5179853CDB359E1BB4F3F5BE61C3E55A0E` |
| `output/chapter1-playable-r1/tools/b1-window-video-capture-r4.exe` | 395264 | `1793993246584C6D009C8B636F9C3034E8339EF22B31552827B6B7B7B25357C2` |
| `output/chapter1-playable-r1/project/Library/b1-unity-g3-owned-r4/control.jsonl` | 1421 | `300A546FD5651ABCAD0A7EE93148CD56212B7ACE7A86375A4F84EB0DB5D72856` |

주 원장은 `output/chapter1-playable-r1/evidence/b1-basic-movement-minimal-compile-work-ledger-root.v1.json`이다. 원장 전체 지문을 실행 권위로 삼지 않는다. 기존 원시 접두사를 보존하는 끝 추가 방식과 공유 파일 단일 쓰기 소유를 유지한다. 64비트 정수가 들어 있는 역사 전체를 자바스크립트 숫자로 재직렬화하지 않는다.

| 원장 항목 | 의미 |
|---|---|
| `unityVisibleOwnedStartPreparation5` | 현재 관리자 코드와 계획의 실행 없는 보정 준비 |
| `unityVisibleOwnedManager5CompileObservation` | 구문 1회 오류 0, 분리 C# 메모리 컴파일 1회 성공, 0.8628192초 |
| `unityVisibleOwnedManager5IndependentRead` | 루나의 전체 역치환 대조, 새 P0/P1 없음 |
| `unityVisibleOwnedStartExactAuthority5` | 이미 소비하고 종료한 §10.10.25 단회 시작 권위 |
| `unityVisibleOwnedStartActualExecution5` | 원문 기록 가시성 및 원래 창 결속의 당시 관측 |
| `unityBasicMovementWindowRead2` | 당시 GameView와 미무장 관측창의 식별값 |
| `unityR01AIntegratedAuthorityPreparation3` | 종료된 세션의 R01-A 준비 틀. 미래 동적 값 재사용 금지 |
| `unityOwnedManager5DeferredPlayClosure` | 사용자 플레이 연기와 실제 정리·폐쇄 제어 원문 |

관리자 준비5의 코드 원문은 **47083 / `FF58FA1EEE6AB7EE4AE81BF8417BC5EACC45607E3E14A48221D68E3D60CF8840`**, 계획은 **22793 / `6096195F624C9EA78688837DA7FD40F50F13E23E178F0DDE797F73FC59C6E2AC`**다. 분리 C#은 **13387 / `FCFB183EA2011A646C7E5AD148093879C4C79190341531E6E5B09023D34E3BAC`**다. 검사 통과를 실제 네이티브 API나 영상 실행의 수용으로 확대하지 않는다.

## 이번에 닫은 문제와 남은 제한

관리자 Pump가 짧은 준비 메시지를 파일 버퍼에 남겨 두면 부모와 자식이 서로 기다릴 가능성을 실제 자식 실행 전에 발견했다. 실제 쓰기 직후 `Flush(false)`를 추가하고 flush 실패에도 Dispose를 시도하도록 한정 보정했다. 실제 영상 자식의 descriptor 가시성은 아직 관측하지 않았다.

터미널 반환은 화면 접기와 제어문자를 포함하므로 권위 JSON 원문으로 복원해 사용하지 않는다. 동일 UTF-8 JSON과 LF 바이트를 고정 `control.jsonl`에 CreateNew 단독 writer로 기록한다. 행 32KiB·전체 1MiB를 쓰기 전에 검사하고 파일과 터미널 어느 쪽의 실패도 숨기지 않는다. 마지막 실행에서 시작·조회·결속·종료의 완결 4행을 읽었으며, 관리자가 종료한 뒤 위 지문을 계산했다. 동기 flush의 강제 시간 상한이나 저장장치 내구성은 보장하지 않는다.

r4 컴파일의 전체 120.3668366초 실패는 보존한다. 객체의 후속 링크 입력 사용만 한정 수용됐으며, 별도 링크는 41.4278684초에 통과했다. 객체·실행 파일 존재를 전체 컴파일 관문 성공이나 영상 촬영 성공으로 바꾸지 않는다. 재분석 시험 3개 미수행도 전체 9개 통과로 쓰지 않는다.

## 다음 세션 순서

1. 사용자에게 실제 플레이 가능한 시점을 확인한다. 현재 연기 선택을 즉시 실행 승인으로 해석하지 않는다. 기다리는 동안 충돌 없는 기승인 준비·개발은 진행할 수 있다.
2. 필요한 경우 기존 오류·소진·누적 4회를 보존한 별도 정확 단회 시작 판정을 받는다. 준비5와 같은 코드를 사용하더라도 새 출력 접두사·소유·예산·정확 권위를 닫아야 하며 이미 소비한 권위를 반복하지 않는다.
3. 새 원래 프로세스의 PID·UTC 7자리 시작 시각·QPC·HWND·thread와 실제 GameView/관측창 EntityId를 다시 관측한다. UTC는 `ParseExact`와 UTC 스타일로 처리하고 `ConvertFrom-Json -DateKind String`을 사용한다. 이전 동적 값이나 중립 관측은 재사용하지 않는다.
4. 정확 메뉴 `AcadeGameMaker/Chapter1/Review Basic Movement/R01`의 장면 생성 부작용, prefab/meta 핀, host 유일성을 본문과 대조한다. 저장·수집기·Play 없는 장면 준비와 주창 최대화, 아래 두 디렉터리 생성, RESERVE의 실제 단계 권위를 먼저 닫는다. 최대화나 화면 크기는 실제 WGC 크기·짝수 조건의 증거가 아니다.
5. 사례 디렉터리는 `output/chapter1-playable-r1/evidence/b1-unity-g3-owned-r1` 및 그 아래 `R01-A`의 고정 2개다. 기존 부모 안전성·부재·생성 방식·부분 실패 보존을 확인한다. 디렉터리를 파일의 CreateNew와 혼동하지 않는다. RESERVE는 디렉터리를 생성하지 않는다.
6. 승인된 경우 RESERVE의 실제 원래 핸들 3개·FILETIME·창 크기·사례 시계와 정확 준비 입력을 결속한다. 같은 자식·같은 WGC item의 원문 descriptor 길이/SHA를 확인한 뒤 최종 계획을 발급한다. 새 자식으로 바꾸거나 임의 크기를 대입하지 않는다.
7. 실제 영상 시작·정확 외부 권위·중립과 사용자 준비가 닫힌 후 수집기 1회와 첫 Play 1회를 진행한다. R01의 등록·reload·첫 tick 0·200→210 실행 순서는 같은 첫 Play에서 관측한다. 예비 Play를 추가하지 않는다. 진단 230은 양성 40+음성 190이며 230 tick이나 실행 순서가 아니다.
8. 실제 Play와 Pause의 진단 1회, 사용자 직접 조작, Stop·seal·영상 END·폐쇄를 관측한다. Root의 ACCEPT는 진행 영수증이며 독립 최종 수용이 아니다. G3의 실제 10사례와 6개 방 묶음 독립 검수, B1 FINAL 이후 통합 제한을 유지한다.

전체 18000초에는 시작·가져오기·기다림·Pause·UI·정리가 포함된다. 사례는 1800초이며 종료·회수 예산과 첫 실패 중단·자동 재시도 0을 정확 계약대로 적용한다. 실패나 한도·인증·모델 오류를 반복 호출로 해결하지 않는다.

## 유지할 경계

- 사용자 직접 플레이와 Unity가 실제 수신한 지정 입력만 기록한다. 운영체제 키 후킹, 자동 키 유지·해제, 게임 조작 주입은 하지 않는다.
- Windows UI는 적용 가능한 컴퓨터 조작 스킬과 실제 노출 API를 읽은 뒤 조작한다. 실제 앱 인증·보안·개인정보 동의는 사용자가 처리한다.
- 아스트라 `gpt-6-astra`, 솔 `gpt-6.1-sol`, 루나 `gpt-6-luna`를 실제 도구 가용 목록과 AGENTS 기준으로 배정한다. 필요한 수만 쓰고 추론 상향을 강제하지 않는다. 테라·5.6·외부 모델과 별도 유료 API는 사용하지 않는다.
- `automation-2`는 삭제 상태를 유지한다. 일일 오전 8시 감사는 별도 대화에서 수행하며 이번 인계를 진척 감사로 추가하지 않는다.
- Q0 키·세대·인간 승인·필수 9회·게임 코드 게시 관문과 동결 계약/소스/실패 원문을 보존한다. M2/C1C 공유 통합을 B1 FINAL 이전에 수용하지 않는다.
- 원격 저장소의 문서 병합은 게임 코드·도구·바이너리·원시 증거의 게시나 CH1 빌드 전달 완료를 뜻하지 않는다. 로컬 작업 폴더의 미게시 파일과 기존 Git 상태를 보존한다.
