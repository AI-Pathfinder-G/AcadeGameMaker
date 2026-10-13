# 원격 객체 지문 보완의 독립 검수

상태: **Approved**, 2026-10-02 아스트라 역할 승인. 별도 `gpt-6-luna`가 `REQ-PLAT-018/019`, `AC-PLAT-016/017`의 원격 객체 결속만 검수한다.

정확 입력은 `artifacts/c4-content-survey-remote-objects-v2.json` SHA-256 `9B4FF061C3F4D8FB2FA80C6F24B63EB6E65D43F1B7A4D0650C658D6B86E23A11`, 테라 보고 `docs/verification/2026-10-02-content-survey-remote-correction-terra-report.md` SHA-256 `E5FFE85E92639434E47E7BFDA3F5768EF44BEE7D396D528082ECCE5AF900A7CA`, 원본 조사 SHA `AE4DE92037B0A345ADB9AA42710112C07EB54A9BD512C1A6309444EFB02A4F4A`, 원본 독립 결과 SHA `ADAA4547349D195002144C0041968A47EB7FEC320E6D0B2CCCC8FC7D68CB7419`다. 소유 보완 계약은 `2026-10-02-content-survey-remote-object-correction.md`다.

새 관측 시각·HEAD→Git커밋 tree.sha→실제 트리응답sha의 동일성, 완전1042항목·잘림false, 정확501 집합의 존재/부재, 보관7blob Base64 실제복원 크기·SHA256·GitblobSHA1과 트리항목 결속, 원문/정규화 충돌 별도재계산, 원문501 전후와 원본v1 보존, 모든 거짓 승인 플래그를 읽기 전용 대조한다. 필요하면 연결GitHub GET으로 고정HEAD의 Git커밋/트리만 재조회할 수 있으나 최신원격 판정으로 소급하지 않는다. 로컬Git 읽기도 허용하되 객체생성·인덱스·설정·쓰기·Unity·시험 실행은 금지한다.

허용 새 출력은 `docs/verification/2026-10-02-remote-object-correction-luna-review.md` 하나다. 원본P1이 새 결속 증거로 보완됐는지 정확AC/P0/P1으로 판단한다. 의미상 내용 적합·의존 폐쇄·렌더러·Q0·깨끗한복제·게임회귀·게시 승인 범위는 제외한다. 오류 호출은 반복하지 않는다.
