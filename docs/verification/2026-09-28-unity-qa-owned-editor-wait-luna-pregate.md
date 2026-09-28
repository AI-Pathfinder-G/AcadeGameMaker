# Owned-Editor wait contract pre-gate — Luna

Date: 2026-09-28  
Reviewed contract: `docs/specs/work-contracts/2026-09-28-unity-qa-owned-editor-wait.md` (Review; no implementation authority)  
Reviewed design: `docs/verification/2026-09-28-unity-qa-launcher-wait-design.md`  
Scope: read-only runner/design review while the R22 Editor is active; no Unity, Assets, license, dependency, process, or runner execution.

## Verdict

P0: none. P1: two bounded design clarifications are required before approval/implementation:

1. The design’s “existing test-counter success policy” should repeat the
   contract’s closed rule in the outcome table and deterministic tests:
   `test-run` total must be nonzero, passed must account for the run, and
   failed/skipped/inconclusive must all be zero. In particular, zero-total or
   zero-XML cases cannot pass merely because an exit code is zero. A bounded
   `AwaitSeconds=0` test may exercise immediate classification, but it must
   still classify zero/absent evidence as incomplete, never success.
2. Exact parsed-argument matching is required, but the design does not state
   an argv-safe `Start-Process` construction/round-trip for paths containing
   spaces, quotes, separators, or flag-like values. Require a deterministic
   quoting test and a single argument-vector construction that preserves the
   exact `-projectPath`, `-testResults`, `-logFile`, and `-runTests` tokens.

These are implementation-clarity P1s, not permission to broaden the allowlist.
The historical `REQ-PLAT-012..014` / `AC-PLAT-010..012` comments in the design
remain historical trace only; current normative trace is the contract’s
`REQ-PLAT-004/005`, `AC-PLAT-002`, and `AC-UQW-001..007`.

## AC map

- **AC-UQW-001/002:** exact parsed path/flag matching, pinned executable,
  `-runTests`, duplicate/worker exclusion, retained PID + creation time/handle,
  and launcher/Editor attach race are covered; no selection by prefix or PID
  alone.
- **AC-UQW-003:** 30-second discovery grace, monotonic bounded wait,
  compact 30-second progress, one launch, and no kill/relaunch/schedule are
  specified. The attach-before-exit race must remain incomplete evidence.
- **AC-UQW-004:** valid nonzero NUnit XML, all-zero failure/skip/inconclusive,
  actual exit 0, malformed/inconsistent/zero counters, and zero discovery are
  closed outcomes; apply P1-1’s explicit wording.
- **AC-UQW-005:** licensing/entitlement failure, transient channel refusal,
  missing XML after actual exit, unreadable XML, unknown exit, and live-editor
  timeout are distinct; valid completed XML must not be overridden by a
  transient refusal.
- **AC-UQW-006:** deterministic PowerShell tests remain Unity/license-free,
  with no production bypass, dependency, or process termination.
- **AC-UQW-007:** implementation must provide exact runner/test hashes and a
  later real invocation; this review is not acceptance.

No implementation or acceptance claim is made. R22 evidence remains owned by
the currently running actual Editor and is unaffected by this review.

## Amended-contract closure review — Luna

Re-read after Astra’s amendments. Contract SHA-256:
`97956C258ABE23D08A5F83FDEE423A85E21EDFCB63DCEB8614FBE59EC6D51AD8`.
Design SHA-256:
`D0033AB6D8E26C01D229C67DA1FC3452ADCB2638AE87B5ABCDA2F0795CC667D0`.

Both prior P1s are closed: the contract and design now require nonzero,
consistent XML totals with every row passed and zero failed/skipped/
inconclusive, explicitly reject public `AwaitSeconds=0`, and constrain any
internal immediate-deadline test from becoming success. They also require one
validated Windows-safe encoded argument line, reject malformed/unsupported
values before launch, cover spaces/trailing backslashes/quotes/flag-like
values/duplicates with golden round trips, and never print the encoded line.
Legacy platform identifiers are explicitly non-normative trace.

Remaining P0/P1 findings: none. All seven AC-UQW mappings above remain valid;
this is still a Review-only pre-gate, not approval or implementation
acceptance. The active R22 Editor remains untouched.
