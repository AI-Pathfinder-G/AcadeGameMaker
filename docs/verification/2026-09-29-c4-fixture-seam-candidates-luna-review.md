# C4 A/B/C 시험 seam 후보 독립 검토

검토 대상 `docs/proposals/2026-09-29-c4-fixture-seam-decision-candidates.md`의 확인 SHA-256은 `F0B6897D9481D73107B94D70592A2F091F5557C7DE6AC805FB90E9EDA2D5DB33`이다. 원본 C4 계약은 `Review`, 직전 구현 계약 초안은 SHA-256 `8339895BC58985997AD55A9577045F99D2EF3C74B1821CAA3E450EFCA6C9E9BE`이며 보존된다. 이 후보안도 승인·구현·실행이 아니며, 앞선 P1 A/B/C를 폐쇄하지 않는다.

**판정: P0 0건, P1 3건 유지.** 후보는 현재 throw-only checkpoint 허용보다 넓은 정지, observer/음성 손상, worker 진입 순서를 각각 별도 승인 대상으로 분리하고 실제 결과와 합성 결과를 구분한다. 다만 A/B/C를 허용할 정확한 계약 문구, 예상 NUnit 행 및 실행 증거는 승인 전 결정되어야 한다. C3 전체 선행 수용 및 필수 Play610 실행도 미완료 상태다.

A의 `DiskPrepared` 검증 뒤·C2 진입 전 정지점은 실제 C1/C2 호출을 그대로 보존하고, 별도 actor가 실제 임시 root lease를 얻은 다음 기존 C2 Acquire가 Busy를 내도록 하는 구성이어서 가짜 Busy 반환보다 계약에 맞다. root의 실제 lock 경로를 의도적으로 불능화해 C2의 ManualRepair 경로를 유발하는 안도 결과를 직접 만들지 않는다. 하지만 둘 다 C4 계약의 기존 throw-only 통제를 넘어서는 새 정지·worker·임시 filesystem 조작이다. 정확 phase, one-shot, timeout, 준비실패 시 C2 미호출, lease/worker 정리와 native 결과 판정까지 포함한 별도 Approved amendment 없이는 허용할 수 없다. 별도 BCL worker 결과를 별도 프로세스 검증으로 부풀리지 않는 제한은 적절하다. 이 실행이 입증할 수 있는 범위는 C1 DiskPrepared 이후 C2의 실제 Busy 또는 ManualRepair 처리뿐이며 C1 Busy/Stale handback과 혼동해서는 안 된다.

B는 C1 결과가 `readonly struct`라는 점을 반영해 by-value observer나 boxed copy 변조를 executor의 실제 저장 결과 손상이라고 부르지 않는다. 실제 proof 객체 한 필드 음성과 실제 C2 결과 참조 음성은 각 결과 생성 뒤의 getter/composition 거절로 한정하며, 호출 이전 손상으로 과장하지 않는다. 반면 제안한 공통 순수 validator의 boxed-copy 음성은 top-level 실제 local 결과의 변조 실행이 아니라 validator와 production 분기 지배 구조의 증거다. 이 경계만으로 AC003의 결과-corruption 행이 충분한지 판정이 필요하다. 실제 holder 대안은 production이 소비하는 저장 projection을 훼손할 수 있지만 새 holder/불변 원본/등록 상태와 negative-only 저장 손상 권한을 추가하므로 단순 관측으로 볼 수 없다. 두 대안 모두 C1 `Begin` 결과의 대체, 정상 issuer/권한 제조, 결과 재등록·복구를 허용하지 않는다는 점은 분명하다. 어떤 대안을 채택할지, AC003/005에서 어떤 행이 실행 증거이고 어떤 행이 구조 근거인지 정확한 amendment 전까지 P1 B는 열린다.

C 후보는 이미 실제 소비된 opaque 요청을 늦게 호출한 worker가 adapter/router의 Unity 상태를 읽기 전에 비변경 거절되도록 제안한다. 이는 정상 소비 loser의 보존을 증명할 수 있는 구체 행이며, 최초 유효 호출을 임의 worker에 허용하지 않고 consumed-history 확인을 Unity 경계보다 먼저 두는 조건도 타당하다. 하지만 이는 소비 후 늦은 호출에 한정된다. 사전 인증 뒤 gate까지의 모든 경합 순서나 callback 없는 동기 구간의 same-thread 재진입 증거가 아니다. 실제 호출 순서, 관리 참조 비교, 정상 승자의 단일 완료 및 history 보존을 정확 시험으로 고정하고 나머지는 구조 근거로 남겨야 한다. 시험 callback이나 대기 gate로 실행 가능성을 인위적으로 만들면 안 된다. AC008의 이 증거 조합을 허용하는 기술 판정 전까지 P1 C는 열린다.

제안은 각 새 seam의 계약 amendment가 필요하다고 명시하고, R11 runtime/선택을 이미 변경한 것처럼 취급하지 않는다. 변경 전·후 C1/C2 알고리즘, Hub 역참조, asmdef/friend/public ABI, 기존 선택·역사 보존과 새 focused 선택/원장 분리는 구현 단계의 필수 추적 조건으로 유지해야 한다. 현재 자료에서 세 후보의 실행성이나 C4 수용 결과는 검증하지 않았다. C4 Review, C3 선행 전체 수용 미완료, Play610 진행 중을 유지한다.
