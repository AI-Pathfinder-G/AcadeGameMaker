# 1챕터 직접 플레이 재개 기록 — 2026-10-15

## 목표와 현재 경계

최우선 목표는 사용자가 현재 소스의 로컬 빌드에서 본편 1챕터를 직접 플레이하고 완주하는 것이다. 사용자는 직접 플레이를 지금 진행할 수 있다고 답했다. 실제 게임 입력은 사용자가 하며 운영체제 키 후킹·자동 유지/해제·입력 주입은 하지 않는다. 준비가 닫히기 전 Play·사례 시계·영상 촬영은 시작하지 않는다.

현재 SourceReady=false, B1 FINAL·실제 10사례·CH1 완주 미완료다. Unity 누적 시작은 역사 기록대로 7회이며 이번 기록에서 Unity·컴파일·게임 입력·촬영 시작은 0이다. 기존 소비 권위와 동적 신원은 재사용하지 않는다. 구 인계 `2026-10-14-chapter1-session-handoff.md`의 당시 시작4회는 역사이며 현재값이 아니다.

## M1 현재 런타임 능력 관측

`docs/specs/work-contracts/2026-10-08-m1-bounded-development-compatibility.md` §10.10.38·39는 Approved다. 기존 `capabilityCurrentRuntimeExecutionRecord1`의 정상·지연 사례와 물리 폐쇄는 부분 증거지만, Root의 직접 봉인 QPC가 재판독보다 앞서 능력 수용으로 올리지 않았다. 이전 단회 권위는 소비됐다.

이번 세션에는 새 준비2의 래퍼가 없는 필드 이름을 읽는 P1을 루나가 발견했다. Root는 준비2를 보존하고 `capabilityCurrentRuntimeWrapperCorrection2`를 끝 추가했으며 루나의 정적 재검토 P0/P1=0이다. 다음 전체 호출문 후보에서는 외부 권위/원장 핀 미대조, Root와 래퍼의 두 시작 시계, 실패 메모리 원문 누락의 P1 세 개를 아스트라·루나가 확인했다. 해당 후보는 실행하지 않았다.

§39에 맞춘 새 `capabilityCurrentRuntimePreparation3`은 정상1·지연1, 하나의 첫 QPC, 별도 외부 핀 인수, 원시1MiB/영수증32KiB 분리 운송을 정적으로 설계했다. 루나는 네 본문과 원장 원문의 바이트 일치, 구문 오류0, 새 P0/P1=0을 확인했다. 그러나 `transportLimitVerified=false`, `executionReady=false`, 정확 실행 권위0이며 실제 관측은 0이다.

Root가 `exec_command`의 합성 1,081,856바이트 출력을 `max_output_tokens=400000`으로 두 번 확인했을 때 둘 다 중간 33,280바이트가 생략됐다. 이는 §39의 최대 원시 반환을 무잘림 운송한다는 선행을 충족하지 못한다. 실제 관측을 실행하지 말고, 별도 승인된 유한 운송 방식과 잘림 검증을 먼저 닫는다. 이 합성 확인은 능력 부모·감독기·Unity·컴파일을 시작하지 않았다.

주 원장은 `output/chapter1-playable-r1/evidence/b1-basic-movement-minimal-compile-work-ledger-root.v1.json`이다. 이번 종료 지점의 전체 길이/지문은 **27,768,625바이트 / `8E58E5D62AE333ECBDA584C9F5965B540351A0059E60FA6D153ACBCEA5ECBC71`**이다. 원장에는 준비·정정·독립 판독·운송 한계 원시·아래 격리 구현 결과를 새 고유 항목으로만 추가했다. 원장 전체를 재직렬화하지 않는다.

## 충돌 없는 C1-C 격리 저작

`docs/specs/work-contracts/2026-10-03-c1c-bounded-closure-candidate.md` §8이 격리 Closing 참조와 프리팹 저작을 Approved로 승인했다. Root가 기준 v5의 4원문과 부모 메타8, 신규 프리팹 GUID, 대상 v6 부재를 `c1cClosingIsolatedV6OwnershipPreparation1`에 고정했다. 솔은 `output/chapter1-playable-r1/staging/c1c-sol61-v6/`에 정확6파일과 부모 메타8만 작성했다.

주요 결과는 `Chapter1ClosingPresenter.cs` **20,336바이트 / `E4852557C6E43C642C1D30EE3595C497AA77B034FBE427B17C791A69C06E9815`**, `Closing.prefab` **13,222바이트 / `0FE72348D082F9C0AB6B11685A538ABFE0F32094735371D851E300241F66CB3C`**, 기존 대사 JSON **3,657바이트 / `FAA385CCF3CD30B652FDCE4D8A6F81F2234E337D973D157E6F05D57CB5A3AE3E`**이다. 전체 14파일의 지문은 원장 `c1cClosingIsolatedV6Implementation1`에 있다.

새 소스는 정적 UI·대사 참조와 두 입구만 추가하고 기존 실제 호스트·영수증·60틱·양분기 동작을 보존했다. 프리팹은 비활성 루트, 빈 초기 문구, 실제 폰트·대사·스케일러 참조를 갖는다. 루나가 AC-C1C-CLOSEAUTH-001/002의 정적 범위를 P0/P1=0으로 판독했고 아스트라가 이 **격리 정적 산출물만** 한정 수용했다. 해당 판정은 `c1cClosingIsolatedV6AstraLimitedAcceptance1`에 있다.

Unity 가져오기·글리프 표시·컴파일·실제 두 분기·공유 프로젝트 통합은 아직 0이다. B1 독립 FINAL 전 C1-C 공유 통합을 하지 않는다. 실제 `ClosingHostEntry` 발급, 프리팹 복제/비활성 결속/활성화/파기 호출자는 후속 정확 계약이 필요하다.

## 다음 작업 순서

1. §39의 잘림 없는 최대 원시 운송을 충족하는 유한 방식을 설계하고 별도 승인·독립 판독한다. 원래 부모·감독기/두 사례/예산·기존 실패 원문은 보존한다. 현재 준비3을 실행 권위로 해석하지 않는다.
2. 실제 외부 원장·권위 핀과 한 시계·원시 반환을 닫은 뒤에만 아스트라 정확 단회 실행 권위를 받아 능력 관측을 시행한다. 성공하더라도 r16 소비 호환과 별도 컴파일·Unity 권위가 필요하다.
3. B1 FINAL의 필수 실제 사례와 독립 수용을 진행한 다음 C1-C 공유 연결을 판단한다. 사용자가 직접 조작할 준비는 확인됐지만 기술·권위 선행이 닫힐 때까지 Play/촬영을 시작하지 않는다.

결정이 필요한 미닫힌 항목은 보류로 둔다. Q0 인간 승인·키/세대·필수9회·원격 게임 코드 게시 관문과 기존 동결 원문·실패 기록은 유지한다. 일일 진척 감사는 별도 감사 대화에서 하루 한 번만 수행한다.
