# Seryeong 64px runtime density decision

- Date: 2026-09-20
- Status: **Accepted — option C approved by the user on 2026-09-20; recorded in ADR-0033**
- Design counter-review: Sol
- Final authority after user direction: Astra

## Context

The accepted project baseline is a 640×360 internal gameplay canvas, 18 PPU environment/world density, fixed world framing, and integer upscale to supported outputs. Seryeong's newer production masters use a 64px standing body in 128×128 cells. They are still `source_only`, but choosing how they will eventually appear in play changes the media package that can be accepted.

## Options

| Option | Result | Main cost/risk |
|---|---|---|
| A — character-only 36 PPU on 640×360 | Preserves approximately 1.78 world-unit body size, but two source pixels collapse to one logical pixel | Loses the intended 64px detail and may produce unstable outlines/hair sampling; not recommended |
| B — 1280×720 internal world render with 36 PPU characters | Displays the 64px gameplay body at full source detail while preserving world framing | Broad renderer migration: camera, Pixel Perfect profile, pointer/aim mapping evidence, UI composition, supported-output scaling, and many exact 640×360/18 PPU tests/specs require rebaseline |
| C — keep 640×360/18 PPU and author reviewed 32px gameplay derivatives | Preserves current camera, aim, UI, collision, world framing, and integer pixel behavior; 64px master remains the wardrobe preview/source authority | Each outfit/action needs intentional pixel reduction and manual cleanup; gameplay carries less detail than the master |

## Sol recommendation

Choose **C** for the current vertical-demo architecture. The user accepted this recommendation on 2026-09-20.

- Treat one accepted outfit as an atomic package containing a matching accepted 64px wardrobe preview and a hand-reviewed 32px two-facing gameplay atlas.
- Begin from nearest-neighbor reduction only as a draft. Manually restore face, hair, garment identifiers, bow/hand contacts, silhouette, and sole baseline before acceptance.
- Reject any package whose high-detail preview and gameplay derivative read as different hair, outfit, palette, or proportions.
- Preserve 64px masters so a later high-resolution renderer migration can reuse them.

Option A should not be used because it keeps world size without delivering the authored detail. Option B is visually strongest, but it is a platform/rendering migration rather than a costume-import setting. If the user prioritizes full 64px in-play detail over the current minimum-resolution and integer-scale contracts, B must receive a separate ADR, impact contract, and regression program before media import.

## Decision effects

- Selecting C authorizes Astra to draft the first real-media acceptance/import contract around `64px preview + reviewed 32px gameplay atlas`, while leaving the real catalog pending until visual/license/hash acceptance.
- Selecting B pauses real-media import while the render baseline is redesigned and independently verified.
- No option changes existing source status by itself. Current Seryeong media remains `source_only` and Accepted `0/432`.
