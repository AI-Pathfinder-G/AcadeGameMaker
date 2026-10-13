# Unity 진입 전 캡처 복구 제안 문구 정정 재검토

검토 대상: `docs/proposals/2026-09-30-c4-final-queue-pre-unity-capture-recovery-design.md`\
정확 SHA-256: `15E770FBF6231849CC41C89E7E02BFCDA51FF5D6831F3CEA6A3E0462CCF52ACD`\
판정: **앞선 P1 폐쇄 확인. 이 한정 정정 기준 P0 0건, P1 0건. 제안 상태이며 구현·실행 승인은 아니다.**

정정된 설명을 v14 원장과 입력에 대조했다. 두 builder 로그는 v14 manifest의 `Files`에 각각 `Kind=Evidence`와 정확 SHA로 한 행씩 있고, `AllowedChanges`에도 한 행씩 있다. v14 input `ReasonMap`의 두 행은 각각 `Disposition=Added`이다. 실제 로그 바이트도 각 기록 SHA와 일치한다. 따라서 “Evidence에 보존”이라는 기존 표현을 “Files의 Evidence 항목”으로 고친 설명은 정확하다. v14 manifest와 v14 input 자체를 v15 `Evidence` 및 직접 입력으로 결속하면, 로그가 v15 실행 캡처에서 제외되어도 이전 로그 지문과 추가 사유의 원본 기록을 보존한다.

로그를 v15 실행용 `Files`·`AllowedChanges`와 `Paths`·`ReasonMap`에서 함께 제외하고 캡처/검증 도구의 제외 규칙을 유지하는 설계는 현재 동작과 일치한다. 로그 둘은 고정 선행 캡처에 없으므로 제외를 `Removed`로 기록하지 않는 설명도 맞다. 결과 경로를 v9로만 갱신하고 v8 실패 결과 및 v10 계획을 보존하며, 9개 실행·사례·행·종료 관측·실패 중단 조건을 바꾸지 않는 제한도 적절하다.

본 재검토는 앞 문서 `docs/verification/2026-09-30-c4-pre-unity-capture-recovery-luna-design-review.md`의 P1 중 로그 분류 문구만 대상으로 했다. 새 v15 산출물·큐 변경·실행은 검수하지 않았으며 Unity/큐를 실행하지 않았다.
