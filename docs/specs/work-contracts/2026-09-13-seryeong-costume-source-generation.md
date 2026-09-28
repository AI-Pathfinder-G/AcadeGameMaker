# Seryeong costume source generation

- Status: Approved
- Authority: Astra, 2026-09-13; user requested all existing outfits and gameplay sheets.
- Parent: REQ-COST-001/010/011, REQ-SPR-001..010.
- Scope: source inventory, built-in image generation, saved prompts/references, and source review only. No simulation or scene change. Mechanical image processing remains subject to the pending explicit user answer.

The stable actor ID is `seryeong`. The scoped wardrobe has 36 entries: story (default outfit); A1..A3, C1..C3, D1..D3 from shoulder-v2; V1..V4, T1..T4, L1..L4, B1..B4 from shoulder-v2/v8; X1..X4, S1..S3 from bold-adult-v1; R1..R3 from string-bikini-v2. Front/back views are one outfit; hair-only H variants are not additional costumes. Preserve all existing media and identities. Similar outfits remain individually mapped to their source IDs.

Each outfit targets three source sheets, six columns by four rows, 24 independently drawn key poses per sheet. Movement: idle, walk, run, jump/land. Combat: evade/dash, bow shot, held bow draw/release, hit/recover. Village: forward stretch, folded-arm gaze, writing, supply gesture. A source pose sheet is not itself a verified animation atlas. Bow hold is a visual action variation and adds no charged-attack mechanic. Dash art adds no invulnerability. Full-body cloth and secondary settling follow action acceleration and preserve coverage; no separate anatomy-focused oscillation clip. Appropriate cloth support and stiffness differ by garment.

Use the adopted same adult face, long half-up black hair and narrow shoulders. References designate the exact single outfit, not the whole multi-character reference sheet. Neutral orthographic side/three-quarter facing right, stable scale, generous gutters, no text, no baked FX. Request actual alpha; where unavailable, solid magenta source is acceptable but remains explicitly unprocessed. Final accepted native scale and palette remain governed by the parent contracts; never silently change collider/camera/art density.

Priority order: A1, A2, C2, D3, then story (default outfit) and remaining entries in ordinal order. Do not begin NPC/enemy/boss image production before Seryeong production is completed or the user changes sequence. Concurrent read-only roster research and skill specification drafting is permitted.

AC-COST-001 source evidence: account for every entry, source path/hash and explicit option position. AC-COST-008 source evidence: inspect action-driven settling and coverage, while native motion measurement remains pending packing/playback. AC-SPR-006: wrong frame count, merged figures, crossed cells, missing limbs, background or captions prevent clean-atlas acceptance. Save failed sources separately and retry targeted defects. Image generation does not approve any gameplay mechanic.

Allowlist: new images/sprites/costumes-v2/**, new docs/verification/2026-09-13-costume-source-*.md. Astra handles image-tool orchestration; Terra may maintain manifest/tooling under a separately approved child contract; Luna reviews source/native evidence independently.

