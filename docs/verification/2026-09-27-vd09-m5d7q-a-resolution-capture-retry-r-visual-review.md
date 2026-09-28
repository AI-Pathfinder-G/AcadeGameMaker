# VD09 M5D7Q-A resolution capture retry-r visual review

Date: 2026-09-27 (Asia/Seoul)  
Reviewer: Luna (independent visual QA)  
Scope: direct pixel inspection of all ten retry-r PNGs; no image modification and no Unity execution.

## Cross-check

The reviewed directory contains exactly ten PNGs and `manifest.json`. The manifest reports the following dimensions and the PNG IHDR dimensions independently match each row:

| File | Dimensions | Visual result |
|---|---:|---|
| `640x360.png` | 640×360 | Logical frame fills the output; four Korean labels are legible, distinct, and contained. Continue focus rail is visible at the left edge. |
| `1280x720.png` | 1280×720 | Clean 2× presentation; text edges remain crisp, button spacing and focus rail are consistent. |
| `1920x1080.png` | 1920×1080 | Clean 3× presentation; no clipping, overlap, broken glyphs, or overflow. |
| `2560x1440.png` | 2560×1440 | Clean 4× presentation; safe-frame content is stable and centered, with consistent scale and spacing. |
| `1366x768.png` | 1366×768 | Expected centered 1280×720 safe frame with visible side/top-bottom margins; menu remains fully inside the frame. |
| `3440x1440-ultrawide.png` | 3440×1440 | Expected centered 2560×1440 safe frame with equal vertical side letterbox regions; no stretching or clipping. |
| `720x1280-narrow.png` | 720×1280 | Expected centered 640×360 safe frame with top/bottom letterbox regions; Korean labels and rail remain intact. |
| `639x359-below-minimum.png` | 639×359 | Expected below-minimum/suppressed capture; the authoring menu remains visually coherent and no glyph is visibly truncated or corrupted. |
| `resize-quarantine-frame0.png` | 1280×720 | Expected quarantine frame with `pointClickSuppressed=true`; authoring visual is coherent and the non-color focus rail remains visible. |
| `resize-quarantine-frame1.png` | 1280×720 | Expected quarantine frame with `pointClickSuppressed=false`; same authoring visual state, with no visible corruption or layout jump. |

## Visual findings

- Background/backdrop is uniform and inert-looking in every image; safe-frame margins and letterboxing are symmetric for the non-16:9 targets.
- All four labels — `계속하기`, `새 게임`, `설정`, `종료` — render as readable Korean glyphs with no tofu, missing strokes, clipping, overlap, or visible overflow.
- The four button rectangles preserve their vertical rhythm and consistent width/height. The Continue focus rail is a clear non-color cue; no disabled strike or notification panel is visibly active. The empty notification state is consistent with the authoring contract.
- Scaling is proportional across 1×/2×/3×/4× outputs. Ultrawide and narrow captures show centered safe content rather than stretched UI. The below-minimum and quarantine cases retain the expected authoring view; pointer suppression is a manifest/input-state fact and is not expected to add a visual marker.
- `resize-quarantine-frame0.png` and `resize-quarantine-frame1.png` differ in their expected state digest/PNG identity but show no user-visible corruption; this is consistent with the contract’s separation of pointer suppression from authoring visuals.

## Verdict

**PASS — visual P0=0, P1=0, P2=0.** No visual blocker was found. This is a visual-only post-review of retry-r; fresh-process byte identity and final integration remain separate gates.

