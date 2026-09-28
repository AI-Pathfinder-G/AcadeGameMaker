# VD-09 M5D7Q C3L 회귀 범위 독립 해석

- 검수자: Luna
- 일자: 2026-09-28
- 기준: Approved C3L 계약 AC-M5D7QC3L-005, Approved C3 AC-M5D7QC3-010 및 현 변경 범위

## 판정

제안한 단계적 실행은 계약상 허용된다.

1. 현재 C3L 단계에서는 focused C3L 11개와 새 runtime 변경에 영향을 받는 C1/C3 EditMode required 회귀 562+51을 새 소스로 실행한다. 이 묶음은 C3L의 `focused and required C3/C1 regressions`에 해당하며, AC-M5D7QC3L-005 판단에 필요한 failed/skipped/inconclusive 0 및 Luna P0/P1 검토를 제공한다.
2. C3 owner/rearm 구현 이후에는 새 C3 focused 시험과 함께 Input/Hub/C2 PlayMode required 610 및 필요한 EditMode 집합을 현재 최종 소스에서 다시 실행한다. 과거 PlayMode 결과는 successor 소스 통과로 재사용하지 않는다.

AC-M5D7QC3L-005 본문에는 PlayMode 610이 명시되어 있지 않다. 따라서 C3L만의 acceptance를 위해 610을 선행 필수로 볼 근거는 없다. 반면 C3L은 parent `AC-M5D7QC3-010`을 지원하고, C3 AC010은 focused 및 required regression suites를 요구하므로 owner/rearm 통합 후 610을 포함한 최종 회귀는 필수다. 이 610을 C3L 실행으로 소급하거나 C3 전체 수용으로 기록해서는 안 된다.

현재 legacy service/disk 본문 재구성·Adapter getter·lower observation만 변경된 단계에서는 C3 owner lifecycle 자체를 검증할 수 없다. C3L focused와 새 EditMode 회귀가 통과해도 결과 범위는 C3L 및 해당 C1/C3 영향 회귀로 제한하며, owner/rearm 구현 뒤 최종 통합 실행을 별도로 요구한다.
