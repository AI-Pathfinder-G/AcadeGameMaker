---
status: Approved
---

# VD-09 M5D7Q-C New Game confirmation and single-profile reset

- Date: 2026-09-28
- Status: Approved — bounded reset transaction; implementation and verification pending
- Product direction: `docs/approvals/2026-09-28-new-game-single-profile-confirmation-approval.md`
- Decision: `docs/adr/0035-new-game-manual-only-prior-profile.md`
- Owner and eventual approval/integration: Astra
- Bounded design: Sol; intended implementation: Terra; independent QA: Luna
- Dependencies: M5D7Q-B, M5D7D/E/K/L/M Verified and VD-09 Approved
- Parent requirements: `REQ-UX-004/009`, `REQ-PLAT-003/006/008/009/010`

## Scope and non-scope

Define the `NewGame` request's confirmation, cancellation, exact default
profile reset, and success/failure boundary. No mutation occurs on mere Q-B
request receipt. The contract must never interpret `Continue` as active-run
restoration.

This Approved contract authorizes only the bounded reset behavior below;
it does not claim implementation or verification. Actual destination scene
transition, expedition start, Settings, Quit, wardrobe, costume media,
additional save slots, and full-demo completion are out of scope. In-world
hub identity, build registration and transition acceptance need a later
contract. No sandbox scene becomes a product destination.

## Candidate behavioral contract

1. An exact Q-B `NewGame` request is consumed once. Existing-progress
   detection compares every freshly observed valid active candidate against
   every approved default field, including settings, bindings, tutorial and
   progression; source name, file existence or revision alone is not the
   decision. Meaningful non-default state in primary **or previous** requires
   confirmation, even if the selected primary is exact default. A readable
   but invalid/unsupported candidate has ambiguous user data and also
   requires confirmation before its archival. A `Default` bootstrap at
   revision `0` does not by itself require confirmation. Any newer meaningful
   or ambiguous state discovered at confirmation reopens the decision gate,
   never overwrites it.
2. Confirmation copy must explain that the current single profile's
   progress, settings, bindings and tutorial acknowledgments will reset.
   Cancel restores menu interaction without changing profile files, backup,
   memory state, input maps, scene or run. Duplicate confirm/cancel and
   teardown are one-shot.
3. After confirmation, obtain exclusive ownership and freshly observe
   `profile.json`, `profile.prev.json`, and `profile.tmp.json`; revalidate
   source identity/revision and approved preservation rules. Any mismatch
   reopens the decision gate rather than using stale launch receipt bytes.
   Never directly delete/truncate a valid primary to perform reset.
4. Reset result uses the exact v1 default settings, empty binding override,
   empty tutorial and initial progression from the approved default planner;
   it does not merge prior state or preserve an active run. It starts a new
   v1 lineage at revision `0`. Old active primary, previous and temp bytes are
   moved into a transaction-unique, manual-only recovery directory outside
   the three automatic candidate names. No old byte is an automatic load or
   recovery candidate after completed reset. An exact-default selected primary
   is not a no-op when any previous/temp leaf exists: the transaction must
   still remove all old automatic candidates. A no-op is allowed only when
   primary is exact default and both previous and temp are absent.
5. Use approved atomic temp write/flush/re-read/commit semantics. Before a
   confirmed, post-commit validated result there is no success event or
   scene cutover. Failure before durable barrier publication leaves old
   active state authoritative. Failure after publication keeps the barrier
   and permits only validated restart/resume, never old automatic recovery.
   An uncertain commit returns a terminal `ReloadRequired` result and makes
   no guessed rollback or second attempt in the same session.
6. The in-memory profile, settings/input state, and durable reset result must
   agree at cutover. A typed, one-shot reset receipt may be produced only
   after exact post-commit validation; a future destination adapter must
   accept that receipt before switching from `UIOnly`.

## Reset cutover protocol under design

This protocol is the confirmed New Game exception to the ordinary VD-09
load/save order. `profile.reset.json` is a durable
transaction barrier, written and validated through a same-directory temp
and no-overwrite atomic move. Its canonical, hashed v1 body identifies the
transaction, the exact default target and the fresh observed presence,
classification, byte length, hash and decoded revision (where valid) of
each active profile leaf. An unreadable present leaf fails before mutation;
an unknown, malformed or unreadable barrier is terminal manual repair and
never falls through to ordinary `Previous` recovery.

The reset coordinator and all normal profile writers/loaders require one
cross-process exclusive root ownership mechanism. The exact same-root path
is `profile.operation.lock`, opened with `FileMode.OpenOrCreate`,
`FileAccess.ReadWrite`, `FileShare.None` and held through fresh observation,
all disk mutation and memory cutover. The file may persist after process
exit: its existence alone is not ownership. Acquisition waits at most five
seconds, then returns a non-mutating Busy/ReloadRequired result. After a
crashed owner's handle releases, the next owner rechecks barrier and leaves
from disk rather than trusting an earlier snapshot. A held-root internal save
entry point avoids reacquiring the lock during reset; a public ordinary save
acquires it independently. Every ordinary load/save checks the barrier under
ownership and refuses to proceed if it exists, unless entered by the reset
resume coordinator. A process-local `lock` is insufficient. Under that ownership, confirm
revalidates the observation, creates a unique non-overwriting
`recovery/new-game/<transaction-id>/` archive, then durably publishes the
barrier before moving any active leaf. Marker publication failure leaves
normal active state authoritative; an empty archive may remain but is not a
reset signal. Once the barrier exists, startup intercepts it before normal
load recovery and resumes this transaction only.

The profile root must already be prepared before opening the lock file;
directory preparation failure or access denial returns a non-mutating
terminal result, not a fallback to process-local locking. A stale
`profile.reset.tmp.json` without a committed barrier is never promoted or
interpreted as authorization to reset: normal startup may load the untouched
active profile, but a later reset refuses the temp-name collision pending
explicit preservation/repair. A successful barrier move consumes its temp;
completed reset requires both barrier names absent. Crash safety here means
process termination/restart, not an unproven power-loss guarantee for
directory metadata. Archive destinations are reopened and verified after
same-volume moves before proceeding.

The coordinator moves the observed temp, primary and previous, if present,
to their respective archive roles. Each step accepts exactly one matching
fingerprint at source or destination, and verifies the destination after
move. Both copies, neither copy, mismatch, collision, or unreadable bytes
cause terminal uncertainty/manual repair, with no deletion or guessed retry.
Only when all originally present bytes are archived and all three active
leaves are absent may the approved first-save path create the exact default
revision `0` primary. The active previous and temp must remain absent.

After exact default primary and archive post-validation, stage and apply the
default profile/settings/input to memory while the barrier and root ownership
still block every other writer. A memory apply failure or partial apply keeps
the barrier and enters terminal `ReloadRequired`; the session cannot issue a
normal save, start gameplay or return to the menu. After successful memory
cutover, the barrier may be removed. Failure or uncertainty removing it yields
`ReloadRequired` and no success receipt; restart with the barrier validates
the completed durable state and finishes removal. Only after barrier absence
and active-state reprobe may the coordinator emit one receipt and release
ownership. Failure or uncertainty in that reprobe is terminal
`ReloadRequired`, with normal saves/menu/gameplay disabled for this session;
ownership is released only after the session is gated, never while old
in-memory state could be saved. A later normal save may create a previous snapshot only from this
new lineage; completed reset leaves no old automatic candidate.

While the barrier is present, a crash at any point resumes the transaction
from validated fingerprints; a corrupt default primary is rebuilt from the
exact target only after proving all old leaves are archived. A malformed
barrier or ambiguous move/commit blocks automatic launch. A pre-barrier
crash leaves the old profile authoritative. No in-session retry follows an
uncertain commit. Archive files are retained indefinitely unless a later
user-approved retention policy changes this; no automatic restore, cleanup
or restore UI is in scope.

The old-leaf resume matrix is strict until the complete archive is proved:
matching source only means move is pending; matching destination only means
move has completed; both old copies, neither copy, or any mismatched/unreadable
old bytes mean terminal manual repair. Once every old destination is proved,
an active primary matching the new default target is a reset product, not an
old-leaf duplicate. An active temp created by an interrupted default first
save is never a load candidate: on restart preserve its bytes under this
transaction archive before retrying the exact first save; an unreadable or
colliding temp blocks automatic resume. For every present old leaf, the destination remains inside the
validated transaction directory, is reopened and fingerprinted after move,
and is never overwritten. The transaction ID is fixed-format, path-separator
free, and the archive path must resolve beneath the profile root without a
reparse/symlink traversal. A readable but invalid profile leaf is still
archived byte-for-byte; an unreadable leaf prevents the transaction from
beginning. A marker-present restart that finds complete archive and exact
default primary completes validation; if default primary is corrupt it may
recreate exact `r0` only after proving every old leaf is archived and absent
from the active names.

## Mutation boundary and parent-spec amendment gate

The only persistent paths this transaction may create or change beneath the
profile root are `profile.operation.lock`, `profile.reset.tmp.json`,
`profile.reset.json`, the three existing active profile leaves, and files
inside its unique `recovery/new-game/<transaction-id>/` directory. It may
not scan that archive as a load candidate, overwrite an existing archive
name, alter a different save slot or touch costume/media storage. Before
barrier publication, abort leaves active state authoritative; an empty
transaction directory is inert. After durable barrier publication, rollback
to old active filenames is forbidden: resume to the exact default or stop
for manual repair. There is no automatic erasure or cleanup of old bytes.

The VD-09 v1 amendment applies narrowly: when the reset barrier is
present, it takes precedence over ordinary primary/previous/temp selection;
the first save after all old leaves are archived starts a new lineage at
revision `0` with no previous. After barrier removal, ordinary VD-09 rules
resume unchanged. This exception is scoped only to confirmed `NewGame` reset
and must be cited by its implementation and verification evidence.

## Candidate requirements and acceptance criteria

- **REQ-M5D7QC-001:** Q-B `NewGame` request is exact, one-shot and has no
  effect before its confirmation policy is resolved.
- **REQ-M5D7QC-002:** Existing-progress confirmation is explicit, and Cancel
  is byte-for-byte and in-memory mutation-free.
- **REQ-M5D7QC-003:** Confirm re-observes current durable candidates and
  refuses stale or conflicting state before mutation.
- **REQ-M5D7QC-004:** Reset constructs an exact approved-default profile and
  commits only through atomic save and the chosen old-profile disposition.
- **REQ-M5D7QC-005:** Failure/uncertain commit has no success or cutover;
  successful cutover has a validated one-shot receipt and matching memory.
- **REQ-M5D7QC-006:** Reset recovery must honor the selected `OD-NG-001`
  policy, including primary corruption and crash windows.
- **REQ-M5D7QC-007:** A durable reset barrier intercepts every loader/writer
  before ordinary recovery and old active leaves are archived outside all
  automatic candidates under cross-process exclusive ownership.

- **AC-M5D7QC-001:** Primary/Previous/Default and meaningful-change cases
  display the correct confirmation decision; cancel/duplicate input leaves
  every file, memory snapshot, map, scene and run unchanged.
- **AC-M5D7QC-002:** A matrix of valid/invalid/unreadable primary, previous,
  stale temp, and changed file generation proves fresh observation,
  preservation order and no silent overwrite.
- **AC-M5D7QC-003:** The committed document matches exact defaults, hash and
  revision/generation policy; old-profile fate matches `OD-NG-001`; no old
  progress is silently merged.
- **AC-M5D7QC-004:** Fault injection at observation, preservation, write,
  flush, temp validation, commit and post-validation proves safe failure and
  terminal uncertainty without false success or automatic retry.
- **AC-M5D7QC-005:** Crash/restart before, during and after commit, including
  corruption of the new primary, follows the chosen recovery policy and
  yields consistent in-memory/durable state.
- **AC-M5D7QC-006:** No scene/gameplay effect occurs without a validated
  reset receipt; a rejected destination cannot retroactively claim the
  profile was not reset.
- **AC-M5D7QC-007:** With a valid, invalid or unreadable reset barrier,
  startup never chooses old `Previous`; with no barrier after completion,
  no old active leaf exists. Marker, each archive move, first-save and marker
  removal fault/crash points, plus a second process attempting load/save at
  every phase (wait at most five seconds, then non-mutating failure), preserve
  this invariant without false success. Memory-apply fault injection proves
  no ordinary save can republish old state after durable reset. Stale barrier
  temp without a committed barrier never initiates reset, and completed reset
  leaves both barrier names absent.

## Traceability and open decision

`REQ-M5D7QC-001/002 -> AC-M5D7QC-001`; `003 -> 002`; `004 -> 003/004`;
`005 -> 004/006`; `006 -> 005/007`; `007 -> 007`.

`OD-NG-001` is resolved by the user: manual-only archive, never automatic
recovery. Astra approved this bounded contract on 2026-09-28 after Sol's
design and Luna's independent pre-review found no remaining P0. The VD-09
amendment above covers the loader barrier and new-lineage revision `0`.
Implementation still requires Terra ownership, REQ citations, deterministic
fault/crash tests and Luna's independent AC verification before integration.
