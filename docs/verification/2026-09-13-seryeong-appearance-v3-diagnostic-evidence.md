# Seryeong appearance v3 source-reference diagnostic evidence

Status: diagnostic implementation evidence only; all appearance combinations remain `0/432 Accepted`.

Traceability: `REQ-APV3-004/006/007/008/010`; `AC-APV3-004/007/008/009`.

The offline builder at `tools/seryeong_appearance_v3/build_diagnostic.py` produced
the twelve first-four source-reference candidates under
`images/sprites/seryeong-appearance-v3/native/{a1,a2,c2,d3}/`. Each source has a
hash-bound transparent clean master, mask, 32px-body/64px-cell candidate atlas,
comparison-only 64px-body/128px-cell density study, row GIFs at native and
nearest 4x, checker contact, and per-frame bounds/anchor proposal record.

The original `source-reference` and broad-fringe `source-reference-v2-broad-fringe`
outputs are retained. V2 records residual key-colour and opaque-edge chroma spill
counts instead of treating a narrow classifier result as clean-alpha evidence.
It does not globally erase plum garment colours.

The v3 component diagnostic retains the former records and writes separate
`source-reference-v3-components` candidates. It selects an 8-connected foreground
component from an expanded per-frame ROI, renders `component-overlay.png`, and
records every proposed component/bounds/anchor. This is specifically a review aid
for cross-grid boots and hair; selection, scalp-to-sole reference height, and
anchors are unreviewed.

`test_diagnostic.py` rebuilds the broad-fringe diagnostic twice in clean temporary
roots and compares every output hash. It also verifies twelve records, 24 frames
per source, 32px native atlas dimensions/alpha mode, and the comparison study
dimensions. The test does not constitute visual acceptance.

No Unity, runtime, existing media, inventory, or appearance-composition files were
changed by this diagnostic slice. No hair mapping, layering, mirroring, completed
action matrix, palette quantization, or combination acceptance is claimed.
