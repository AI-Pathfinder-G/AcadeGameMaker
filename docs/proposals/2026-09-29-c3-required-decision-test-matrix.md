# C3 확인 분류와 실제 파일 재관찰의 필수 집중 행렬

- 날짜: 2026-09-29
- 상태: 제한 시험 설계 제안 — 구현·실행·수용 아님
- 설계자·실제 모델: 솔, `gpt-6-sol`
- 추적: `REQ-M5D7QC3-002/004`, `AC-M5D7QC3-002/004`.
- 범위: 현재 프로필 스키마와 인코더, 승인 기본값, 실제 소유자 fixture를 읽었다.
  테스트 소스·제품 코드·Unity·public API·assembly는 변경하지 않았다.

## 실제 자료 구성

기본 D는 `ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument`의 실제
정규 바이트와 Snapshot이다. 유효 변형은 public `ProfileSettingsSnapshot`,
`ProfileInputSnapshot`, `ProfileTutorialSnapshot`, `ProfileProgressionSnapshot`,
`ProfileSnapshotV1`으로 만들고 `ProfileCanonicalEncoder.Encode`의
`CanonicalFileBytes`를 쓴다. 각 행은 public decoder로 목표 decode classification을
먼저 확인한다. root·capture·request·decision/receipt/proof는 제조하지 않는다.

세 역할의 정확한 이름은 Primary=`profile.json`, Previous=`profile.prev.json`,
Temp=`profile.tmp.json`이다. `profile.temp.json` 등 다른 이름을 쓰지 않는다.
실제 launch는 기본 파일로 안전하게 완료한 뒤 **intake 또는 Confirm 전에** 시험
임시 root의 실제 세 파일만 바꾼다. launch preparation이 이전/임시 자료를 복구·
보존한 결과를 새 관찰 자료로 잘못 사용하는 일을 피한다. 파일 쓰기/삭제 완료와
handle 닫기를 확인한 뒤 실제 lower capture 또는 실제 owner API를 호출한다.

기존 `NewGameActualCohortFixtureV1.Create`는 실제 임시 환경, 실제 preparation,
latch/presenter/Q-B를 만든다. fixture의 정상 반환물만 사용한다. 본문에는 이름
단독 메서드 조회가 있으므로 새 양성 행에서는 승인된 정확한 선언 형식·전체
매개변수/반환 형식 결속을 적용해야 한다. private 필드는 역사/세대/원본 capture
참조의 **읽기 증거**로만 사용할 수 있고 권한/상태를 대입하지 않는다. 실제 입력
프레임 재무장은 별도 PlayMode 집중 행의 책임이며 이 표로 대체하지 않는다.

## 단일 제품 필드 행 — 세 역할 각각에 배치

아래 각 행은 해당 필드 외 모든 값을 D와 같게 유지한다. 세 역할 r 각각에 그
자료만 두고 나머지를 absent로 하는 행과, 다른 역할에 D를 둔 대표 경쟁 행을
분리한다. 각 field×role 실제 분류 관찰을 생략하지 않는다. M은 의미 있는 확인,
A는 모호한 확인, N은 확인 불필요다.

| 행 이름 | 기본값 → 명시 변형 | 실제 decode | 기대 C3 분류 |
| --- | --- | --- | --- |
| F01_WindowMode | BorderlessFullscreen → Windowed | ValidCurrentInput | M |
| F02_MasterVolume | 1000 → 999 | ValidCurrentInput | M |
| F03_MusicVolume | 700 → 699 | ValidCurrentInput | M |
| F04_SfxVolume | 800 → 799 | ValidCurrentInput | M |
| F05_AimInvertX | false → true | ValidCurrentInput | M |
| F06_AimInvertY | false → true | ValidCurrentInput | M |
| F07_BindingOverrides | 빈 문자열 `""` → `ProfileBindingOverridesJson.Parse("{}")` | ValidCurrentInput | M |
| F08_TutorialIds | 빈 배열 → `new[]{"tutorial.jump"}` | ValidCurrentInput | M |
| F09_LastOfferedSeed | null → 101 | ValidCurrentInput | M |
| F10_Consent | RefusesOwnershipTransfer → OffersScopedResonance | ValidCurrentInput | M |
| F11_CompletedBranches | 빈 배열 → `new[]{ProfileBranch.Extraction}` | ValidCurrentInput | M |
| F12_ChoiceSkillExtraction | null/null → Extraction/CompressionVerdict | ValidCurrentInput | M |
| F13_ChoiceSkillSolidarity | null/null → Solidarity/CommonReferencePlane | ValidCurrentInput | M |
| I01_InputAssetId | 현재 상수 → `"other-input-asset"` | ValidInputMetadataRecoveryRequired | A |
| I02_BindingSchemaVersion | 1 → 2 | ValidInputMetadataRecoveryRequired | A |
| R01_RevisionOnly | 0 → 1, 제품 값 전부 D | ValidCurrentInput | Primary 단독 N, Previous/Temp 존재 A |

F12/F13은 committedChoice와 grantedSkill 각각의 비기본 비교를 모두 추적한다.
현 스키마의 public 생성자는 두 값을 동시에 null 또는 승인된 정확한 쌍으로만
허용한다. 한 필드만 null/비null인 자료는 유효한 단일 필드 변형이 아니므로 권한
반사 제조로 만들지 않는다. 대신 두 정상 쌍 행과 별도 불일치 쌍 invalid 바이트
행으로 두 필드의 요구를 빠짐없이 추적한다. I01/I02는 입력 호환 복구가 필요하므로
의미 있는 정상 입력 프로필 M으로 잘못 기대하지 않고 A를 검증한다. 모든 스칼라
제품 필드가 이 표에 있으며 bookkeeping revision만 의미 판정에서 제외한다.

## 세 역할 배치·분류 행렬

각 역할의 자료 종류는 `Ø`=absent, D=정확 기본, M=F02 대표 의미 변경,
I=I01/I02 또는 정상 파일의 binding recovery, X=invalid, S=unsupported,
U=실제 unreadable이다. 아래에서 r, s는 서로 다른 실제 역할이다.

| 대표 행/전개 | 기대 | 필수 의미 |
| --- | --- | --- |
| (Ø,Ø,Ø) | N | 파일 없음도 실제 관찰 |
| (D,Ø,Ø) 및 revision-only Primary | N | 실제 기본 모든 제품 필드 동일 |
| D가 있는 나머지 6개 비어 있지 않은 부분집합 | A | Previous/Temp 존재만으로 기본이어도 확인 필요; (D,D,D) 포함 |
| 각 r에 F01..F13 단독, 나머지 Ø | M | 각 단일 제품 필드×각 역할; 이름/선택원본으로 누락 금지 |
| 각 r에 I01/I02 단독, 나머지 Ø | A | 각 입력 메타데이터 필드×각 역할 |
| 각 r에 M, 각 s에 D, 나머지 Ø | M | 순서 있는 6개 배치; default Primary가 선택돼도 다른 leaf 의미 변경 우선 |
| 각 r에 I/X/S, 나머지 Ø | A | I에는 metadata와 binding 두 복구 종류를 각각 별도 행으로 포함 |
| 각 r에 I/X/S, 다른 역할 전부 D | A | 모호한 leaf를 정상/default leaf로 숨기지 않음 |
| 각 r에 M, 각 s에 I/X/S, 나머지 D | M | 각 모호 종류별 순서 있는 6개 배치; 모호 증거도 세 leaf capture에 보존 |
| 각 r에 U, 나머지 D | CaptureUnreadable | 분류 enum으로 바꾸지 않고 현재 epoch 종료 |
| 각 r에 U, 한 다른 역할 M, 나머지 I/X/S 대표 | CaptureUnreadable | unreadable 우선; 다른 역할 실제 배치를 순환 |

D의 부분집합 7행과 전체 Ø를 모두 명시하므로 absent/default 배치에는 축약이 없다.
기타 7종류 전체 Cartesian 곱은 필요하지 않다. 역할별 단독 자료, 정상/default와
경쟁, 의미+모호 우선순위, unreadable 우선순위를 모든 역할로 이동해 관찰과
우선순위 동치류를 폐쇄한다. 임의 한 역할만 검사하고 다른 역할을 동치로 생략하지
않는다. revision-only는 세 역할 단독과 default 경쟁 배치로 이름/선택/개정 독립을
증명하며 M을 판정하는 근거로 쓰지 않는다.

X는 UTF-8 `"{"` 및 정상 envelope integrity 불일치 두 실제 바이트 행을 사용한다.
S는 UTF-8 `{"schemaVersion":2}`를 public decoder에서 UnsupportedProfileSchema로
확인한 뒤 쓴다. binding recovery는 정상 D canonical payload의
`bindingOverridesJson`을 malformed inner JSON `"{"`로 바꾸고 **실제 payload
SHA-256을 다시 계산**한 실제 envelope로 구성하여 decoder가
ValidBindingRecoveryRequired임을 확인한다. schema/invalid/recovery 자료는 정상
스키마 encoder가 만들 수 없는 의도적 디스크 입력이며 발급 proof 위조가 아니다.
불일치 choice/skill 파일은 동일 envelope 절차로 실제 decoder Invalid를 확인한다.
U는 해당 임시 leaf를 FileShare.None으로 열린 실제 FileStream으로 잠그고 root
operation lock은 잡지 않는다. capture 읽기 실패가 Unreadable인지 확인하고 해제한다.
root lock Busy를 U로 집계하지 않는다. 플랫폼에서 기대 장애가 재현되지 않으면
성공/건너뜀으로 처리하지 말고 실제 시험 실패와 원인을 기록한다.

## Confirm 전후 각 역할의 실제 변경 행렬

모든 행은 실제 owner intake로 prompt/display를 먼저 발급받고 g라는 현재 decision
generation과 actual capability를 보관한다. 각 역할 r=Primary/Previous/Temp에
동일 변형을 전개한다. 다른 역할의 M sentinel은 fresh 결과를 계속 prompt로 유지할
때만 사용한다. sentinel 값은 F02이며 다른 파일은 완전히 같은 바이트를 유지한다.

| 역할 r의 display 전 → Confirm 전 변경 | 다른 역할 조건 | Confirm 결과/새 세대 |
| --- | --- | --- |
| Ø → D 추가 | 다른 역할 M sentinel | FreshDecisionRequired, OpenFreshDecision 후 g+1 |
| Ø → M 추가 | 다른 역할 D 또는 모호 prompt 근거 | FreshDecisionRequired, OpenFreshDecision 후 g+1 |
| D → Ø 삭제 | 다른 역할 M sentinel | FreshDecisionRequired, OpenFreshDecision 후 g+1 |
| M → Ø 삭제 | 다른 역할 M sentinel | FreshDecisionRequired, OpenFreshDecision 후 g+1 |
| D → F01..F13 각각 교체 | 다른 역할 prompt 근거 유지 | FreshDecisionRequired, 각 행 OpenFreshDecision 후 g+1 |
| M → 다른 M 바이트 | 다른 역할 Ø 또는 D | FreshDecisionRequired, OpenFreshDecision 후 g+1 |
| D → revision-only 1 | 다른 역할 M sentinel | 제품 값 동일해도 fresh identity 다름; OpenFreshDecision 후 g+1 |
| D/M/Ø → X, S, I 각각 교체/추가 | display는 실제 prompt | FreshDecisionRequired, OpenFreshDecision 후 g+1 |
| X → 다른 X 바이트 | 다른 역할 Ø 또는 D | 분류 같아도 실제 hash/length 변화, OpenFreshDecision 후 g+1 |
| I/X/S → D | 다른 역할 M sentinel | 의미/모호 자료가 default로 바뀌어도 새 identity, OpenFreshDecision 후 g+1 |
| I/X/S → Ø | 다른 역할 M sentinel | 모호 자료 제거, OpenFreshDecision 후 g+1 |
| present → U | 다른 역할 변경 없음 | CaptureUnreadable/TerminalFailure, 새 decision 없음 |
| 바이트 동일 유지 | 실제 M 또는 A prompt | ConfirmedReady, g 유지, 동일 display identity와 새 actual recapture 일치 |

직접 exact-default 경로는 별도 세 역할 행으로 한다. Primary의 M→D이고 두 보조
leaf가 Ø인 행, Previous의 M→Ø이고 Primary D/Temp Ø인 행, Temp의 M→Ø이고
Primary D/Previous Ø인 행이다. 각 행은 변경을 감지하되 새 실제 capture로
NoConfirmationRequired/ConfirmedReady를 반환한다. 새 prompt가 없으므로 g+1
decision을 제조하지 않고 g를 유지하며, confirmed request가 **fresh actual capture**에
결속됐는지 읽기 증거로 대조한다. 모든 세 leaf 삭제도 별도 실제 prompt→전체 Ø
행으로 검증한다. exact-default bypass는 C1 실행을 뜻하지 않는다.

AC004의 새 decision generation은 의미/모호 변경으로 새 prompt를 여는 경로에서
g+1을 요구한다. Approved 본문의 새 exact-default 직접 경로에는 prompt 발급이
없음을 분리한 것이며, 이 해석은 아스트라가 확인해야 한다. 구현이 달리 동작하면
기대값을 실행 결과에 맞춰 바꾸지 말고 계약 충돌로 보고한다.

FreshDecisionRequired 상태에서는 confirmed request가 없고 이전 Confirm/Cancel
capability는 모두 거부돼야 한다. `OpenFreshDecision` 한 번만 새 capability를 만들고
g+1이며 old capture를 제자리 덮어쓰지 않는다. 새 capability Confirm은 파일을
그대로 둔 상태에서 정상 1회 성공해야 한다. source request/receipt/epoch/history는
실제 발급 원본을 보존한다. 각 실제 add/remove/change는 Confirm 전에 세 파일의
존재/길이/hash를 기록하고 Confirm 후에도 같은 파일 상태여야 한다. C1 Begin을
호출하지 않고 stale capture를 실행 경계에 넘기지 않는다.

## 비용과 실행 원장

분류 전용 관찰 행은 동일 actual launch cohort에서 실제 세 파일을 매 행 완전히
재설정하고 **새 실제 Capture**로 순회할 수 있다. 각 행은 원래 capture를 재사용하거나
clone하지 않고 actual registry 발급 결과의 classification을 검증한다. owner intake는
한 번뿐이므로 owner 상태를 반사로 초기화해 여러 intake를 한 cohort에 얹지 않는다.
initial owner 결과와 Confirm 전후 전이는 독립 case/cohort 또는 정상 Cancel/Rearm으로
진행한다. 정상 rearm을 사용하면 실제 첫 프레임 폐기까지 완료한 뒤 새 실제 take를
받으며, 이것이 더 비싸면 독립 cohort가 더 작은 선택이다.

각 field×role/배치/전이 행에는 고정 행 ID와 기대값을 원장에 저장한다. 여러 관찰을
하나의 NUnit case에 순회하면 그 case의 내부 행 원장과 NUnit case 수를 구분하고
모든 내부 행 성공·실패를 기록한다. 단일 실패 뒤 나머지를 수행하지 않았다면
미실행 행은 통과로 집계하지 않는다. 원장 대조는 실제 runner의 정확한 names,
내부 행 ID, 역할·파일 before/after hash, actual owner 결과와 generation/capability
참조 증거를 포함한다. 별도 집중 selector를 고정하고 기존 lower 73/C3L 11 및
Profile codec 회귀는 보조 이력으로만 남긴다. 그 시험들은 현재 실제 분류/owner
전이 행을 대체하지 않는다. 새 지문 동결·실행 전후 대조·actual exit·실패/건너뜀/
판정보류/누락/중복 집계와 루나 독립 검수 후에만 해당 AC 증거를 판정한다.
