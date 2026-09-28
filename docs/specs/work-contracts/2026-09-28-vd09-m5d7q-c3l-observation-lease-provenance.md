---
status: Verified
---

# C3L — observation-only lease acquisition provenance

Date: 2026-09-28. Owner/approval: Astra; implementation: Terra;
independent verification: Luna. Parent: Approved M5D7Q-C3.
Approved — Astra 2026-09-28; observation-only implementation authority.

## Reason and exclusions

The existing `ProfileRootOperationLockV1.Acquire` in
`ProfileNewGameResetServiceV1.cs` collapses root-directory creation failures,
access/security failures and actual exclusive-lock timeout into Busy without
preserving a typed cause. C1's capture wrapper retains that collapsed exception
but cannot recover its cause. C3 requires only genuine acquisition timeout to
permit retry. Message inspection or Outcome.Busy alone cannot satisfy it.

This technical clarification adds no user product decision. Existing Acquire,
CaptureConfirmationIdentity, Begin, Resume, C1/C2 durable algorithms, public
exception ABI and existing callers must remain unchanged. No UI, action, map,
scene, profile save, barrier operation or general lease API is introduced.

## Bounded observation acquisition

Add one internal observation-only acquisition entry on the existing lock type.
It returns the actual privately constructed `ProfileRootOperationLockV1` lease,
not a copied ownership flag or replacement lock implementation. Only the C3
same-lease observation capture may call it. Existing callers still call Acquire.

Normalize and validate the authenticated launch root through the existing
Profile-owned safety checks before acquisition; recheck containment under the
real held lease before barrier/three-leaf observation. The observation capture
returns identity and decoded projections from the same three reads only.

The new entry may create the existing operation-lock/root infrastructure as
normal capture does, but never a profile leaf, barrier, archive or durable
reset mutation. Directory creation/access/security failures and non-contention
I/O failures are typed Unavailable immediately. Unexpected invariant/programmer
exceptions retain their original identity and terminally close C3; they are not
Busy. On the current Windows baseline, only actual exclusive-lock FileStream
open failures with exact HRESULT 0x80070020 or 0x80070021 are retryable lock
contention. Unknown/platform-unmapped failures fail closed as Unavailable;
never inspect exception text or treat every IOException as contention.

Only continued actual contention reaching the existing five-second acquisition
bound may mint typed Timeout evidence. Use the existing lock's bounded monotonic
wait semantics; no new clock/network/random authority in C3 owner/coordinator.
Evidence must retain the actual exception/cause and distinguish RootCreation,
LockOpen and ContentionTimeout. Default/unknown/forged/reflected-corrupt rows
must reject; caller booleans, diagnostics or throw-only test controls cannot
mint timeout provenance. C3 maps authenticated timeout to its existing Busy
retry state, Unavailable to CaptureUnreadable/terminal, and no other failure to
Busy. A failure never carries a lease or observation identity/projections.

Microsoft documents Windows sharing/lock error codes 32/33:
[System Error Codes (0–499)](https://learn.microsoft.com/en-us/windows/win32/debug/system-error-codes--0-499-).
The HRESULT-to-contention interpretation here is a bounded implementation
decision requiring actual FileStream contention evidence, not documentation
substituting for execution.

## Requirements and acceptance

- REQ-M5D7QC3L-001: Add only the internal observation acquisition lane and
  preserve all legacy/durable acquisition behavior byte-for-byte in its bodies.
- REQ-M5D7QC3L-002: Distinguish authenticated actual contention timeout from
  unavailable/access/security/root/non-contention failures without text inference.
- REQ-M5D7QC3L-003: Return only a real same-root lease for same-three-read C3
  capture, and grant no additional effect or public/assembly authority.

- AC-M5D7QC3L-001: Source/API diff proves original Acquire, original capture,
  Begin/Resume and C1/C2 durable behavior unchanged; no new public ABI/friends.
- AC-M5D7QC3L-002: Real retained exclusive FileStream forces genuine bounded
  timeout; releasing it before timeout permits one real acquisition. These
  are real filesystem fixtures, not fabricated timeout results.
- AC-M5D7QC3L-003: Real root-as-file, directory-valued/unsafe lock, unreadable
  path and non-contention failure fixtures are terminal Unavailable, never
  Busy; throw-only boundary errors preserve their original identity. Unknown,
  forged/default/reflection-corrupt provenance fails closed.
- AC-M5D7QC3L-004: Same-lease capture still performs exactly three leaf reads,
  preserves full identity/projection agreement, rejects unsafe roots/barriers,
  and has no profile/barrier/archive/map/scene mutation or failure success row.
- AC-M5D7QC3L-005: Focused and required C3/C1 regressions have failed/skipped/
  inconclusive 0; Luna reports P0=0/P1=0 before Astra acceptance. Prior C1/C2
  source snapshots remain factual predecessor evidence, not successor passes.

Trace: 001 -> AC-001/005; 002 -> AC-002/003; 003 -> AC-004/005.
This supports parent AC-M5D7QC3-002/004/007/009/010 without changing criteria.

## Exact conditional allowlist

After Approved, add only the new internal observation acquisition entry and
its closed provenance types in
`Assets/AcadeGameMaker/Runtime/Profile/ProfileNewGameResetServiceV1.cs`.
Use it only in C3's already-allowlisted observation addition to
`ProfileResetDiskTransactionV1.cs` and lower `ProfileNewGameConfirmationV1.cs`.
Focused existing-assembly Input.Unity EditMode C3 tests may cover real fixtures
and lower Profile boundary through existing visibility. No new test assembly,
friend, configuration, public ABI, scene or asset is allowed. All local edits
use apply_patch and preserve unrelated dirty work.

Stop for Astra if correct classification requires altering legacy Acquire,
adding an alternate durable algorithm, faking successful lease provenance,
changing C3 Busy/user copy, mutating an old receipt or widening assembly/API.
Terra cannot independently accept its own implementation. Main alone executes
Unity regression; Luna independently verifies; Astra integrates.

## Astra approval — 2026-09-28

Luna independently reviewed the full Review draft SHA-256
`9FE666C476D9B3650721DD52FA0A5D61240A6F3B54192BDC56CEAD654A17B4ED`
in `2026-09-28-vd09-m5d7q-c3l-luna-pregate.md` and found P0=0/P1=0
by design for AC-M5D7QC3L-001 through -005. Terra independently confirmed
the exact collapse and real fixture feasibility. Astra approves only the
conditional allowlist and existing behavior exclusions above.

For AC-M5D7QC3L-003, access/security and non-contention branches may additionally
use named throw-only boundary controls preserving the exact original exception.
Such controls can cause only terminal Unavailable and are structurally unable
to mint timeout evidence, a lease or identity. Distinguish these injection rows
from real OS filesystem failures in the report; do not claim real permission
evidence for them. No ACL, account, global permission or security-policy changes
are authorized. Real contention/root-file/lock-directory fixtures remain
mandatory; no Skip/Ignore or invented successful lease is permitted.

Implementation remains unverified. C3 synthetic-only restrictions and Q0
successor exact-hash approval gate remain binding. This is a technical
correctness clarification, not a new user product decision or permission to
modify legacy acquisition/durable behavior.

## 아스트라 통합 수용 — Verified, 2026-09-29

[통합 수용 기록](../../approvals/2026-09-29-c3l-integration-acceptance.md)에 따라 AC-M5D7QC3L-001..005를 수용했다. 루나 최종 독립 검수 P0=0/P1=0, 실제 집중 11개와 현재 필수 회귀 고유 613개의 증거를 확인했다. R4 시간 초과 실패는 보존하며 동일 소스·동일 제한의 R6 한 행과 R5 실제 별도 프로세스 결과를 정확한 이름 및 출처로 대조했다. 입력 874개는 모든 실행 전후 동일하다.

위 승인 시점의 미검증 표기는 당시 이력이다. 현재 수용은 관찰 단위에 한정하며 C3 전체, C3 owner/rearm 회귀, 실행 모드 또는 Review C4 실제 실행 연결 통과를 뜻하지 않는다. 이후 source 변경은 새 검증이 필요하다.
