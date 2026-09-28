# Kimi K3 VD-01 Movement Core Draft — Sol Screening

- Date: 2026-08-25
- Model: `kimi-k3:cloud` through Ollama
- Authority: non-canonical implementation proposal only
- Owning contract: `docs/specs/work-contracts/2026-08-25-vd01-movement-sandbox.md`
- Screening: Sol

## Invocation result

Kimi exhausted the requested 5,000-token response budget on a detailed reasoning draft and returned no final code body. The reasoning was still screened as an implementation proposal; no Kimi output was copied into runtime code.

## Accepted proposal elements

- Use pure integer Q4096 public snapshots and signed 64-bit intermediates.
- Carry `/60` rational remainders for acceleration and position integration so repeated runs do not accumulate frame-dependent floating-point drift.
- Test coyote and buffer at 6/7, cooldown at 35/36, wall lock at 5/6, jump release exactly once, modifier restoration, obstruction cooldown preservation, stable-ground air-dash restoration, and three identical replay streams.
- Keep Unity references and device input outside the pure motor.

## Rejected or corrected elements

- Kimi considered `AcadeGameMaker.Core.Movement`; the frozen namespace remains `AcadeGameMaker.Movement`.
- Kimi questioned or proposed changing `WallId`; the frozen type remains an authored string ID with empty value exactly for `WallSide.None`.
- Kimi counted cooldown from dash start; the approved VD-01 contract starts the 36-tick cooldown after the scheduled 15 active dash ticks.
- Kimi considered ending the dash action immediately on collision; collision stops displacement only and does not shorten the scheduled action or cooldown timeline.
- Kimi treated same-wall locking as possibly wall-jump-only; the approved rule blocks reattachment, wall slide, and another wall jump on the same authored wall until age 6.
- Kimi proposed choosing unapproved wall-slide input and stable-ground heuristics. Sol instead froze direction-free wall slide on valid contact and defined `IsGrounded` as an already validated adapter signal.

## Sol resolutions before implementation

- Snapshot position and velocity fields are signed 32-bit Q4096 values with checked signed 64-bit intermediates.
- Input axes are signed 32-bit Q4096 values clamped to `[-4096,4096]`.
- Duplicate, skipped, and reversed simulation ticks fail fast without state advancement.
- `WallJumping` lasts the impulse tick only.
- Input lock cancels active dash and ignores commands while gravity and collision physics continue.

## Second focused code proposal

Sol asked Kimi again with reasoning disabled and the corrected exact contract. Kimi returned a partial C# motor and boundary-test draft before reaching the 3,500-token limit.

Useful elements retained for Terra's rewrite:

- Separate pure fixed-point helper, motor, and EditMode boundary-test files.
- Signed remainder carry shaped as `sum = velocity + remainder`, followed by `/60` position delta and `%60` retained remainder.
- Test names and intent for coyote/buffer, dash cooldown, same-wall lock, and repeated replay equality.
- Early separation of input lock, modifier selection, dash state, and wall state.

The returned code is not directly admissible. Sol rejected these concrete defects:

- It introduced a new public `FixedQ4096` type and public motor methods outside the frozen API.
- It used nonexistent enum values such as `None` and `Jumping` and fields not present in the approved payloads.
- Critical constants were wrong: gravity was emitted as `-121`, jump as `14390`, wall jump as `9500/13500`, rather than their Q4096 values.
- It did not actually enforce sequential ticks.
- It omitted the required air-dash state and consumption rules.
- It cleared dash cooldown on landing and used wall-side comparison instead of `IsDashPathBlocked`.
- It shortened or miscounted dash/cooldown ticks and produced invalid 6/7 and 35/36 tests.
- It required an unapproved wall-slide input condition and did not implement same-wall reattachment semantics correctly.
- Its snapshot initializer did not match the frozen `PlayerMotionSnapshot` fields.

Terra may use only the retained structural ideas and must rewrite all constants, public shapes, tick accounting, and tests against the Approved work contract. Luna must independently review every Kimi-influenced file.
