# 내장 참조 객체 조회 독립 검수

승인 계약 `docs/specs/work-contracts/2026-10-02-builtin-reference-independent-review.md`. JSON SHA-256 `F1D4B9F2D5DDDB779187A1C9DD97FF728CD949CF8D0D37CE447EAB149895E418`, 테라 보고 SHA-256 `4A9B985BFA980EFE9B99B9D80DFC4982F5B44104DD052E3A73169305008E86C8`, probe 소스 SHA-256 `8596C5B51F508C165E4E5ADECD14BC1A908DCE2213A32EE9C20F353BC30A070A`, 로그 SHA-256 `F6E5BCBB4E9B11C497C1C7999ECE561BB00A5DB64E0989AC9D00816AB46E5799`가 계약값과 일치한다.

## 판정

- **P0: 0. 새 P1: 0.** 기존 Renderer7 P1 및 전체 게시/폐쇄 관문은 그대로 열린다.
- **AC-PLAT-069 — 부분 충족.** 현재 원문 19개 행의 원문 행·값 불일치 0. 두 내장 경로 `Library/unity default resources`(517객체), `Resources/unity_builtin_extra`(257객체)의 실제 관측 목록과 대조해 GUID/long fileID 조합 19/19가 각각 정확히 객체 하나에 유일 일치했다. Hub 장면 `Assets/Scenes/Hub.unity:29` skybox와 `:96` lightingDataAsset은 공개 필드 직접 조회가 확인됐다. 나머지 17행은 객체 튜플 조회만 성공했으며 원문 필드의 사용은 미입증이다.
- **AC-PLAT-070 — 부분 충족/복원 실패 보존.** 전후 보호 파일 지문 45개를 직접 재해시해 불일치 0, 보호 집합 사전/사후 차이 0을 확인했다. probe 소스와 사본 지문은 같고 19행 원문 위치도 실제 파일 행과 전부 일치한다. 실행 1회 종료코드는 0이며 원시 관측 SHA와 로그 SHA가 보존됐다. 그러나 시작 시 열린 장면은 0개였고 `RestoreSceneManagerSetup`은 “No loaded scene found / One active scene is required”로 실패했다. 실패 기록을 성공으로 바꾸지 않는다. 로그에는 별도 라이선스 토큰 갱신 오류와 `Curl error 42: Callback aborted`도 기록돼 있다.
- **사용 한계.** 객체 lookup의 성공은 직접 필드 접근 2행을 넘는 원문 소비를 증명하지 않는다. 17개 튜플 전용 참조의 실제 사용, 저장/복원 성공, 렌더링 소비, 모든 참조 의존 폐쇄는 미입증이다. 전체 회귀·깨끗한 복제·이식성·게시 승인은 이 검수에서 다루지 않았다.

Unity·시험·네트워크·Git 실행 및 기존 파일 수정은 하지 않았다.
