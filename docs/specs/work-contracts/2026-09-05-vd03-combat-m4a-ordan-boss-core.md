# Work Contract: VD-03 M4A Deterministic Ordan Boss Core

- Status: Verified
- Approved by: Sol, 2026-09-05 after Luna pre-gate PASS (`P0=0`, `P1=0`, `P2=0`)
- Verified by: Luna, 2026-09-05 (`P0=0`, `P1=0`, `P2=0`); integrated by Sol
- Drafted by: Sol, 2026-09-05
- Unit design and implementation: Terra
- Independent verification: Luna
- Owning specs: VD-03, with affected VD-02, VD-04, VD-05 and VD-07 boundaries
- Requirements: `REQ-COM-001`, `REQ-COM-003`, `REQ-COM-004`, `REQ-COM-006`; affected `REQ-WT-001`, `REQ-WT-005`, `REQ-WT-006`
- Partial acceptance evidence only: `AC-COM-002`, `AC-COM-003`, `AC-COM-004`; affected `AC-WT-005`

## Purpose and truthful milestone boundary

M4A adds one engine-free, internal deterministic owner for 환수관 오르단's pattern cursor, phase clock, health-stage timing selection, relevant transfer-effect latches, vulnerability windows and one-shot future-consumer handoffs. It consumes immutable projections of already completed Combat and Transfer authority and emits immutable pattern intents. It never applies health damage, owns active transfer, observes Unity physics, grants rewards, completes a room, handles device input or presents UI.

M4A cannot close `AC-COM-002` or `AC-COM-004`. Those require the later M4B Unity bridge, authored boss encounter, actual VD-07 input and Luna's paired play protocol. M4A supplies only exact trace and lifecycle substrate evidence. M4B must use this contract without changing its state semantics.

## Sol decisions frozen for M4A

### Identity and authority

- The boss Combat target ID is literal ordinal `ordan`, maximum health is `60`, invulnerability is `0`, and all boss-authored player damage intents have amount `1`.
- Ordan's body is never a Transfer target. Only the current scripted `BossWeight-0`, `BossWeight-1` or `BossWeight-2` handle and the exact authored `BossAuditBox-0` may create a boss-relevant transfer effect.
- Combat remains the sole owner of health, damage ordering, dedupe and death. Transfer remains the sole owner of active transfer, target modifiers, revision and clear behavior.
- M4A may construct immutable future Combat requests but never applies or predicts their result. A later M4B queue must deliver those requests to Combat on their exact declared tick.

### Pattern and handle order

The infinite alive pattern sequence is four stable slots:

1. `DebtLineA`
2. `SeizureWeight`
3. `DebtLineB`
4. `BalanceAudit`

Both DebtLine slots share the same timing and intent rules but remain distinct cursor values. After `BalanceAudit` Recovery, the cursor returns to `DebtLineA`. The encounter-local checked `long` payload ordinal is initialized to `-1` before the first payload; entry into each new `SeizureWeight` Telegraph increments it once, so the first active value is `0`. The active handle is `BossWeight-(payloadOrdinal mod 3)`; therefore successive payload patterns expose `0,1,2,0...` independent of Unity creation order or render grouping. Exactly one handle is externally exposed as a Transfer target and only during that Telegraph. The same stable ID is retained internally through SeizureWeight Execute for trajectory correlation and the later payload request source, but it is not re-exposed or transferable there. It is cleared on entry to Vulnerable or Recovery.

### Health stages and frozen timing

- Stage 1: health `42..60`.
- Stage 2: health `21..41`, entered when completed Combat health first becomes strictly less than `42`.
- Stage 3: health `1..20`, entered when completed Combat health first becomes strictly less than `21`.
- Health `0` is Defeated and never a stage.

The session records the highest reached stage monotonically after consuming the completed current-tick Combat state. A stage crossed during a phase is pending only: the current pattern slot retains every duration frozen when its Telegraph began. The pending stage becomes active when the next pattern Telegraph begins. One hit may move Stage 1 directly to pending Stage 3. Death supersedes pending promotion.

Telegraph durations are never stage-scaled. The Stage 1 bases are DebtLine Execute/Recovery `21/39`, SeizureWeight normal Execute/Recovery `72/48`, SeizureWeight accelerated Execute/Recovery `24/48`, and BalanceAudit Execute/Recovery `90/48`. Stage 2 applies checked integer `ceil(baseTicks*100/110)` independently to each base, including the accelerated base `24`; Stage 3 applies `ceil(baseTicks*100/120)`. Stage 1 uses the base unchanged. Overflow or a result below one rejects before mutation. The table below is the normative expansion of that formula; an implementation must match both the formula and every value.

| Pattern | Stage | Telegraph | Execute normal | Execute accelerated | Recovery |
|---|---:|---:|---:|---:|---:|
| DebtLine A/B | 1 | 48 | 21 | n/a | 39 |
| DebtLine A/B | 2 | 48 | 20 | n/a | 36 |
| DebtLine A/B | 3 | 48 | 18 | n/a | 33 |
| SeizureWeight | 1 | 60 | 72 | 24 | 48 |
| SeizureWeight | 2 | 60 | 66 | 22 | 44 |
| SeizureWeight | 3 | 60 | 60 | 20 | 40 |
| BalanceAudit | 1 | 60 | 90 | n/a | 48 |
| BalanceAudit | 2 | 60 | 82 | n/a | 44 |
| BalanceAudit | 3 | 60 | 75 | n/a | 40 |

`SeizureWeight`'s formerly ambiguous “up to 72 ticks” is resolved as follows: an unmodified payload uses the exact normal Execute duration; a valid Heavy edge during Telegraph freezes the accelerated Execute duration for that pattern. No spatial callback, RNG or early collision ends the M4A clock.

### Tick and phase convention

- The authoritative clock is a consecutive nonnegative `SimulationTick`.
- Construction accepts `firstExpectedTick` only in `0..int.MaxValue-120`. Before every phase, vulnerability and future-request transition, the candidate checks `phaseStart+duration`, `tick+1` and the largest new exclusive end with checked arithmetic. A later horizon that cannot fit rejects the complete input without mutation.
- The first accepted input has outer tick equal to Combat-state tick, exact target `ordan`, alive full health `60`, `pendingPayloadDamageResult=null` and no relevant transfer edge. It produces the first `DebtLineA` Telegraph snapshot at phase age `0`.
- A duration of `N` owns exactly `N` snapshots with ages `0..N-1`. Its exclusive end is `phaseStartTick+N`; input on that tick belongs to the next phase at age `0`.
- At each tick, M4A validates every input and pending horizon, resolves an aligned pending Combat result, gives current Combat death priority, performs an exclusive-end phase transition, applies the one permitted relevant transfer effect, emits intents, and only then adopts the candidate snapshot.
- `PreviewNext` is mutation-free. `CommitNext` recomputes the candidate from the exact input and compares it field-for-field before replacing all owner state atomically.

Normal phase flow is `Telegraph -> Execute -> Recovery -> next Telegraph`. Vulnerability replaces the remainder of the triggering path and suspends pattern progression. It owns exactly `N` snapshots at ages `0..N-1`, emits no attack intent, and then enters the triggering pattern's Recovery at age `0`.

### DebtLine

- `DebtLineA` and `DebtLineB` emit exactly one immutable `DebtLineIntent` on Execute age `0`.
- Each pattern entry owns a checked nonnegative `long attackOrdinal`, beginning at `0` for the first `DebtLineA` and incremented exactly once when the next Telegraph begins. The DebtLine intent ID is exact ordinal string `ordan.debt/{attackOrdinal:D19}`. M4A records pattern slot, attack ordinal, source tick and damage amount `1`. M4B later owns authored line geometry, player intersection and any `DamageRequest` production.
- No transfer target or transfer effect changes DebtLine.

### SeizureWeight

- A relevant transfer edge contains exact `previousState`, `nextState` and revision. Only `Baseline -> Heavy` is a rising edge; `Baseline -> Baseline`, `Heavy -> Heavy`, `Heavy -> Baseline` and unknown states reject. Only a rising `BossPayload` edge for the exact currently exposed handle, with exact tick, is permitted during Telegraph ages `0..59`.
- The session initializes a global `lastAcceptedRelevantRevision=-1` and one per-target last revision of `-1` for `BossWeight-0`, `BossWeight-1`, `BossWeight-2` and `BossAuditBox-0`. An accepted edge revision must be nonnegative and greater than both the global value and that exact target's value. Only successful commit advances both values. Any rejected, stale, duplicate or malformed edge consumes neither revision.
- The first valid edge latches the current payload as accelerated and emits one immutable acceleration intent. Duplicate, stale, wrong-ID, wrong-kind, Baseline, early, late and second relevant edges reject the complete M4A input without mutation.
- External Transfer-target exposure and transfer eligibility exist only during Telegraph. During Execute the same ID is retained internally for trajectory and request correlation but is not re-exposed as a Transfer target. It is removed on entry to Vulnerable or Recovery. The later M4B owner must preserve this distinction and clear/retire the target without asking M4A to own Transfer state.
- An accelerated Execute lasts the exact accelerated duration in the table. The acceleration intent ID is `ordan.payload-accelerate/{payloadOrdinal:D19}`. On its last active Execute tick M4A emits one future Combat `DamageRequest` candidate for delivery at `t+1`: exact request ID `ordan.payload/{payloadOrdinal:D19}/{deliveryTick:D10}`, source ID equal to the active handle, target `ordan`, amount `8`, kind `BossPayloadTransfer`. Payload ordinal is a checked nonnegative `long`; failure to increment or format a representable ordinal rejects before mutation.
- The next M4A input must contain the exact aligned completed `DamageResult`: request ID must equal the pending ID, target must be `ordan`, and result tick must equal both the pending delivery tick and the input/Combat-state tick. If Combat state remains alive, only `Applied` with `AppliedAmount=8` starts a `120`-tick vulnerability on that result tick. If Combat state is exact dead at health `0`, `Applied` with `AppliedAmount=1..8` is accepted as M1's legal lethal clamp and enters Defeated; `TargetDead` with amount `0` is also accepted when an earlier canonical request killed Ordan before this request. `Applied(0)`, partial `Applied(1..7)` while alive, `TargetDead` while alive, `Invulnerable`, `Duplicate`, `Invalid`, missing or misaligned results reject without mutation. M4A does not invent a fallback vulnerability.
- A normal Execute emits no self-damage request and transitions to Recovery at its exclusive end.

### BalanceAudit

- Execute age `0` emits one immutable `BalanceAuditPullIntent` with ID `ordan.audit-pull/{attackOrdinal:D19}`; it causes no direct damage in M4A.
- Only the exact rising `Baseline -> Heavy` `Box` edge for literal `BossAuditBox-0`, with exact tick and revision satisfying the global and per-target rules above, may interrupt Execute ages `0..duration-1`.
- A valid edge wins over the same tick's final shockwave, discards all remaining Execute time, emits one notice with ID `ordan.audit-interrupt/{attackOrdinal:D19}` and starts a `90`-tick vulnerability at age `0` on that same tick.
- If uninterrupted, the last active Execute tick emits exactly one immutable `BalanceAuditShockwaveIntent` with ID `ordan.audit-shockwave/{attackOrdinal:D19}` and damage amount `1`. M4B later owns authored occlusion, geometry and any player `DamageRequest`.
- Early, late, duplicate, stale, wrong-ID, wrong-kind and Baseline relevant edges reject without mutation.

### Vulnerability effect

Vulnerability is an exact safe attack window, not a damage multiplier. While vulnerable, Ordan emits no pattern attack intent, the pattern clock is suspended and the immutable view marks the already damageable body as exposed for M4B presentation and authored pose holding. Basic attacks remain valid during every alive phase, including Telegraph, Execute and Recovery; M4A never gates or changes their damage. The explicit tactical advantage is the `8` payload damage, skipped or shortened hostile execution and uninterrupted `120`/`90`-tick attack window. Any damage multiplier requires a new Sol contract and M1/M2B impact review.

### Combat, death and lifecycle precedence

- Each input carries the exact completed current-tick Ordan Combat state: target ID, health `0..60`, dead iff health is `0`, and any exact result required by M4A's pending request. Health may not increase and a dead boss may not return alive.
- If current Combat reports first death, death wins over phase boundary, transfer effect, payload result vulnerability and attack intent. The session enters `Defeated`, emits exactly one internal `OrdanDefeatedNotice`, `OrdanRewardRequest` and `OrdanRoomCompletionRequest`, and emits no further pattern output.
- Repeated dead inputs are accepted only at the exact consecutive tick with matching outer/Combat ticks, target `ordan`, health `0`, dead true, no pending result and no relevant transfer edge. Each advances snapshot tick and checked `nextExpectedTick`, retains the complete normalized Defeated shape, and publishes immutable empty intent and handoff collections. No post-defeat alive gameplay or reset is accepted.
- M4A has no same-instance reset. `BossStarted`, room/run re-entry and retry construct a fresh session after the external lifecycle has removed the old scene graph and cleared Transfer. Failure, room exit or destruction produces no synthetic completion handoff.
- The three completion values are frozen future-consumer requests only. They do not grant inventory, mutate a room, change run phase, transition a scene or imply persistence.

## Immutable input and output boundary

The implementation uses internal engine-free values in `AcadeGameMaker.Combat`:

- `OrdanCombatState(tick,targetId,currentHealth,isDead,pendingPayloadDamageResult|null)`;
- `OrdanRelevantTransferEdge(tick,targetId,targetKind,previousState,nextState,revision)|null` using M4A-local discriminants rather than a Transfer assembly dependency;
- `OrdanBossTickInput(tick,combatState,relevantTransferEdge|null)`;
- `OrdanBossSnapshot(tick,patternSlot,phase,phaseAge,activeStage,pendingStage,activeHandleId,isAccelerated,isVulnerable,vulnerabilitySource,nextExpectedTick)`;
- immutable intent and handoff lists ordered by fixed semantic ordinal, never collection enumeration.

Constructors defensively copy collections. No caller-owned array, list or mutable object escapes. Inputs contain no Unity type, Transform, collider, body, registry, callback, wall clock, frame duration or random source.

Frozen discriminants are `PatternSlot=None,DebtLineA,SeizureWeight,DebtLineB,BalanceAudit`, `Phase=Telegraph,Execute,Recovery,Vulnerable,Defeated`, `Stage=None,Stage1,Stage2,Stage3`, `TransferTargetKind=BossPayload,Box`, `TransferState=Baseline,Heavy`, and `Vulnerability=None,SeizureWeightTransfer,BalanceAuditInterrupt`; unknown values reject. Transfer revision is a signed 32-bit integer with accepted values `0..int.MaxValue`. Alive snapshots never use `None` pattern/stage. During SeizureWeight Telegraph, `activeHandleId` names the single externally exposed Transfer target; during Execute it preserves only internal trajectory/request correlation and must not imply availability. Outside those two phases it is empty. Nonaccelerated patterns and all Recovery snapshots have `isAccelerated=false`. Vulnerable snapshots keep the triggering pattern slot, use empty handle, `isAccelerated=false`, `isVulnerable=true` and the exact non-None source. Defeated snapshots normalize to `patternSlot=None`, `phase=Defeated`, `phaseAge=0`, both stages `None`, empty handle, both booleans false and vulnerability source `None`; they retain only exact tick and checked `nextExpectedTick` plus one-shot handoffs on the first death tick. Empty intent/handoff collections are immutable, not null.

## M4B contract obligations reserved now

M4B must separately freeze and review:

- exact Unity execution order relative to `-200` Transfer, `-190` Combat and M3 regular-enemy phases;
- next-tick delivery/consumption of M4A boss and player damage intents without a second M1 owner;
- one authored kinematic trajectory for normal `72` and accelerated `24` base-tick payload paths, stage scaling and exact retained handle geometry;
- DebtLine, pull, box occlusion and shockwave geometry;
- exact Combat/Transfer graph identity and first-use validation;
- `BossDefeated` mapping, Transfer clear ordering and later VD-04 room-completion consumption;
- presentation-only view, authored boss scene and actual play timing evidence.

M4A must not anticipate these with Unity or public runtime code.

## Required evidence

Focused EditMode tests must cover at least:

1. exact four-slot cursor and `BossWeight-0,1,2,0` sequence;
2. all Stage 1/2/3 timing-table values and phase exclusive ends;
3. health `43->42`, `42->41`, `22->21`, `21->20`, Stage 1 direct to Stage 3 and death, with next-pattern-only stage promotion;
4. DebtLine one-shot intent on both slots and no transfer response;
5. payload Heavy edges at Telegraph ages `0` and `59`, rejection at Execute age `0`, accelerated/normal exact durations, one `t+1` amount-8 request and exact aligned result;
6. `120`-tick vulnerability start/end, no intents inside it and Recovery at the exclusive end;
7. BalanceAudit interrupt at first/last Execute tick, interrupt priority over final shockwave, exact `90`-tick vulnerability and uninterrupted one-shot shockwave;
8. source/static proof that M4A has no basic-attack gate, amount rewrite or damage-multiplier field, plus unchanged existing M1/M2A tests;
9. same-tick death precedence, one-shot defeated/reward/completion values and no resurrection/reset;
10. wrong/stale/duplicate transfer edge, wrong/missing/partial pending result, skipped/repeated tick and forged preview rejection with complete state preservation;
11. checked tick, request-ID and phase-horizon overflow before mutation;
12. caller-mutation resistance and exact equality across repeated executions of the same consecutive tick script; render grouping remains an M4B PlayMode responsibility.

Every test cites partial `AC-COM-002`, `AC-COM-003` or `AC-COM-004` and affected `AC-WT-005` as appropriate. Final evidence records total/passed/failed/skipped and XML SHA-256. Full existing EditMode and PlayMode regressions must remain green even though M4A itself is engine-free.

## Allowlist

- `Assets/AcadeGameMaker/Runtime/Combat/OrdanBossSession.cs` and `.meta`;
- `Assets/AcadeGameMaker/Tests/EditMode/Combat/OrdanBossSessionTests.cs` and `.meta`;
- this work contract, its Luna pre-gate and implementation evidence, plus documentation index links.

No asmdef or `AssemblyInfo` change is expected.

## Denylist and stop conditions

- no modification of Core, Transfer, Movement, existing Combat M1/M2/M3 runtime semantics, any Unity runtime, scene, prefab, Editor authoring, package or project setting;
- no public type/member, new assembly dependency, runtime discovery, physics query/sync, callback authority, wall clock, delta time, RNG or Unity identity;
- no actual reward, room/run state, persistence, input, camera, UI, VFX, audio, final art or narrative change;
- no damage multiplier, boss-only failure, instant kill, time limit, dynamic joint or boss-body Transfer target;
- stop and return to Sol if pure implementation requires M1 changes, a Transfer reference, Unity state, a second Combat owner, ambiguous same-tick rollback or a public cross-assembly API.

## Ollama utilization record

| Lane | Outcome | Sol screening disposition |
|---|---|---|
| Kimi K3 | used and accepted in part | Retained typed phase/value decomposition, immutable tick output, preview/commit and one-shot test emphasis. Rejected guessed `40/20` thresholds, basic-attack damage ownership, public fields, mutable preview state and silent enum fallback. |
| GLM 5.2 | used and accepted in part | Retained findings on ambiguous `up-to-72`, stage promotion, transfer-window bounds, death precedence, one-shot latches and vulnerability flow. Rejected a nonexistent DebtLine transfer-vulnerability premise and any suggestion that Transfer directly applies damage. |
| MiniMax M3 | failed and replaced | The bounded fixture request returned an empty final response without quota/rate/auth/model/network error. Terra's local impact pass supplies the fixture matrix; Luna must independently review it. |

No cloud model received repository text, local paths, credentials, personal data, secrets or approval authority.

## Approval gate

Luna independently reported `P0=0`, `P1=0`, `P2=0` after the contract corrections. Sol approves M4A implementation within this exact allowlist and denylist. Approval does not claim implementation evidence or close any full acceptance criterion.
