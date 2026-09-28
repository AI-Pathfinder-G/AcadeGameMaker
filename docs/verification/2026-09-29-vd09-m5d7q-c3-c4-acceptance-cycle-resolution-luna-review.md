# C3·C4 단계적 수용 순환 해소 독립 검수

- 검수자: Luna
- 검수일: 2026-09-29
- 대상 제안서 현재 SHA-256: `F2F821E975A5CB0BB41048452B9A657E7BAA6D5CE420626EC46D7AF3BFA8A61B`
- 대상 ADR 현재 SHA-256: `4ECC0C8141C3A69E41FB0944C9F5EB7346840DD2D5CBDFABDAD22C86FB24D467`
- 대조 기준: Approved C3 `AC-M5D7QC3-001..010`, Review C4 `AC-M5D7QC4-001..010`

## 판정

P0는 발견하지 않았다. 제안서와 ADR은 실행 권한을 넓히지 않고 수용 순서만 분리한다. C3 선행 단계는 C1 `Begin`이나 C4 typed report 발급을 제조하지 않으며, reflection mint·scalar seam·caller boolean·C1 결과 복제를 양성 증거로 인정하지 않는다. C4는 계속 `Review`이고, 실제 C1/C2 연결은 C4가 Astra에 의해 `Approved`로 바뀐 뒤에만 가능하다. 최종 공동 게이트에서만 남은 C3 실행 결과 기준과 C4 기준을 함께 닫고 C3 전체 `Verified`를 기록하도록 한 점은 C3·C4 원문과 일치한다.

AC-M5D7QC3-010은 단계1과 공동 최종 게이트에 모두 명시되어 있다. 단계1의 집중 시험과 필요한 회귀는 실패·건너뜀·판정보류 0 및 Luna P0/P1=0 뒤 Astra 수용을 요구하고, 새 C3 owner/rearm 변경에 필요한 EditMode·PlayMode 회귀를 C3L의 EditMode 562개·worker 51개 결과로 대체하지 않는다. 해당 562+51 관찰은 현재 R4가 실행 중이므로 완료된 수용 증거로 간주할 수 없다. 최종 단계는 `AC-M5D7QC3-010`과 `AC-M5D7QC4-010`을 각각 현재 소스로 다시 충족하도록 한다.

## P1 — 부분 행을 AC 완료로 기록할 수 있는 표기가 남아 있음

단계1이 `AC-M5D7QC3-007`의 관찰 Busy 행과 `AC-M5D7QC3-008`의 C3 커밋 구간을 “수용”한다고 쓰면서, 두 전체 AC가 부분 증거로 `PASS` 또는 독립 `Verified`로 기록되지 않도록 상태명이 고정되어 있지 않다. 원문 AC-007은 실제 C1 Busy/Stale 및 가능한 장벽 이후 terminal 분기를 포함하고, AC-008은 C1 장벽 전 권한 폐쇄를 전체 실행 경계로 요구한다. 따라서 단계1의 결과는 `pre-C4 evidence only`로만 기록하고 AC-007/008은 `Open/Not Verified`로 유지해야 한다. 공동 최종 게이트에서 실제 C1 결과와 C4 회귀를 확인한 뒤에만 두 AC를 닫을 수 있다.

이 보정은 동작이나 기준을 강화하거나 완화하지 않고 기록 분류만 명확히 한다. ADR의 “C3 전체 Verified로 기록하지 않는다” 문구와 함께 단계1의 보고 양식에도 같은 제한을 명시하면 P1이 닫힌다.

## 권한 및 수용 경계 확인

- C3의 승인된 합성 범위, exact owner/epoch/generation, one-shot consumption, rearm 및 실행 직전 권한 폐쇄는 유지된다.
- C4의 C1 `Begin`, guard, typed executor report issuer/intake, C2 `FinalizeReset`은 C4 승인 전 구현 대상이 아니다.
- 최종 공동 게이트는 C4 `AC-M5D7QC4-001..010` 전체를 포함한다. 특히 `AC-M5D7QC4-009`의 정적/API·권한 경계 검수와 `AC-M5D7QC4-010`의 현재 소스 집중·필수 회귀를 `001..008` 또는 과거 결과로 갈음할 수 없다.
- C4 선행 조건의 변경은 제안일 뿐이며 Astra 승인 전에는 계약 변경으로 취급할 수 없다.
- `AC-M5D7QC3-010`과 `AC-M5D7QC4-010`의 현재 소스 집중·필수 회귀와 독립 P0/P1 검수는 생략되거나 과거 결과로 대체되지 않는다.

현재 판단은 **P0=0, P1=1(부분 AC 표기 명확화 필요)**이며, 소스·시험·현재 계약은 수정하지 않았다.
