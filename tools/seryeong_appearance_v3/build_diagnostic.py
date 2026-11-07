"""Offline diagnostic builder for REQ-APV3-004/006/007/008/010.

This is deliberately not an appearance compositor or acceptance tool.  It only
turns the twelve hash-recorded first-four source sheets into mask/anchor
proposals and native candidate previews for human review.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import shutil
from collections import deque
from pathlib import Path

from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[2]
RAW = ROOT / "images/sprites/costumes-v2/raw"
OUT = ROOT / "images/sprites/seryeong-appearance-v3/native"
MANIFEST = ROOT / "images/sprites/seryeong-appearance-v3/manifest"
SOURCES = {
    "a1": (("movement", "a1-movement-v2.png"), ("combat", "a1-combat.png"), ("village", "a1-village.png")),
    "a2": (("movement", "a2-movement-v2.png"), ("combat", "a2-combat.png"), ("village", "a2-village.png")),
    "c2": (("movement", "c2-movement-v2.png"), ("combat", "c2-combat.png"), ("village", "c2-village.png")),
    "d3": (("movement", "d3-movement-v3.png"), ("combat", "d3-combat.png"), ("village", "d3-village.png")),
}
CELL = 256
TARGET_CELL = 64
TARGET_ANCHOR = (32, 52)
LABEL = "diagnostic candidate — source-reference only — palette pending — not Accepted"


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def stable_write(path: Path, value: object) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2, sort_keys=True) + "\n", encoding="utf-8")


def near_magenta(pixel: tuple[int, int, int], background: tuple[int, int, int]) -> bool:
    """Conservative key: hue/chroma-like range around border-derived magenta.

    It is used only after a perimeter flood fill, so a disconnected plum
    garment is preserved even if its colour approaches the key family.
    """
    r, g, b = pixel
    br, bg, bb = background
    return r >= 210 and b >= 210 and g <= 55 and max(abs(r - br), abs(g - bg), abs(b - bb)) <= 46


def border_background(rgb: Image.Image) -> tuple[int, int, int]:
    values = []
    w, h = rgb.size
    for x in range(w):
        values.extend((rgb.getpixel((x, 0)), rgb.getpixel((x, h - 1))))
    for y in range(1, h - 1):
        values.extend((rgb.getpixel((0, y)), rgb.getpixel((w - 1, y))))
    return tuple(sorted(v[i] for v in values)[len(values) // 2] for i in range(3))


def flood_mask(rgb: Image.Image, broad: bool = False) -> tuple[Image.Image, dict]:
    """Make alpha by removing only near-key pixels connected to sheet edges."""
    w, h = rgb.size
    bg = border_background(rgb)
    px = rgb.load()
    seen = bytearray(w * h)
    queue: deque[tuple[int, int]] = deque()
    for x in range(w):
        queue.extend(((x, 0), (x, h - 1)))
    for y in range(1, h - 1):
        queue.extend(((0, y), (w - 1, y)))
    removed = 0
    while queue:
        x, y = queue.popleft()
        index = y * w + x
        p = px[x, y]
        broad_key = min(p[0], p[2]) - p[1] >= 65 and max(p[0], p[2]) >= 90
        if seen[index] or not (near_magenta(p, bg) or (broad and broad_key)):
            continue
        seen[index] = 1
        removed += 1
        if x: queue.append((x - 1, y))
        if x + 1 < w: queue.append((x + 1, y))
        if y: queue.append((x, y - 1))
        if y + 1 < h: queue.append((x, y + 1))
    # Flood filling protects disconnected plum pixels.  A second, stricter
    # source-key pass removes enclosed antialiased key islands, which cannot be
    # fringe-cleaned by connectivity alone.  It is intentionally narrower than
    # the outer classifier and is reported for review.
    strict_removed = 0
    alpha_values = []
    for y in range(h):
        for x in range(w):
            p = px[x, y]
            strict_key = p[0] >= 225 and p[2] >= 225 and p[1] <= 45 and max(abs(p[i] - bg[i]) for i in range(3)) <= 30
            if seen[y * w + x] or strict_key:
                alpha_values.append(0)
                strict_removed += int(strict_key and not seen[y * w + x])
            else:
                alpha_values.append(255)
    alpha = Image.new("L", (w, h), 255)
    alpha.putdata(alpha_values)
    residual = sum(1 for y in range(h) for x in range(w) if alpha.getpixel((x, y)) and near_magenta(px[x, y], bg))
    spill = 0
    for y in range(h):
        for x in range(w):
            if not alpha.getpixel((x, y)):
                continue
            p = px[x, y]
            chroma = min(p[0], p[2]) - p[1] >= 65 and max(p[0], p[2]) >= 90
            if chroma and any(0 <= nx < w and 0 <= ny < h and not alpha.getpixel((nx, ny)) for nx, ny in ((x-1,y),(x+1,y),(x,y-1),(x,y+1))):
                spill += 1
    return alpha, {"borderBackgroundRgb": list(bg), "classifier": {"rMin": 210, "bMin": 210, "gMax": 55, "maxChannelDelta": 46, "broadConnectedChroma": broad, "broadFormula": "min(R,B)-G >= 65 and max(R,B) >= 90", "method": "edge-connected-near-magenta flood fill; optional broad connected fringe variant; strict enclosed-key cleanup"}, "removedConnectedKeyPixels": removed, "removedStrictEnclosedKeyPixels": strict_removed, "residualNearKeyOpaquePixels": residual, "opaqueEdgeChromaSpillPixels": spill}


def source_rgb_alpha(path: Path, broad: bool = False) -> tuple[Image.Image, Image.Image, dict]:
    """Prefer authored alpha; only key RGB sheets that have no transparency."""
    source = Image.open(path)
    rgb = source.convert("RGB")
    if "A" in source.getbands():
        alpha = source.getchannel("A")
        extrema = alpha.getextrema()
        if extrema[0] < 255:
            return rgb, alpha, {
                "alphaSource": "preserved_authored_alpha",
                "alphaExtrema": list(extrema),
                "hiddenRgbIgnored": True,
                "classifier": {"method": "not run; source alpha is authoritative"},
            }
    alpha, diagnostic = flood_mask(rgb, broad=broad)
    diagnostic["alphaSource"] = "derived_magenta_key"
    return rgb, alpha, diagnostic


def bounds(alpha: Image.Image, rect: tuple[int, int, int, int]) -> list[int] | None:
    crop = alpha.crop(rect)
    box = crop.getbbox()
    if box is None:
        return None
    return [rect[0] + box[0], rect[1] + box[1], rect[0] + box[2], rect[1] + box[3]]


def clean_master(rgb: Image.Image, alpha: Image.Image) -> Image.Image:
    image = rgb.convert("RGBA")
    image.putalpha(alpha)
    # Avoid invisible key-colour fringe through future alpha-unaware tooling.
    data = [(0, 0, 0, 0) if a == 0 else (r, g, b, a) for r, g, b, a in image.getdata()]
    image.putdata(data)
    return image


def native_atlas(master: Image.Image, scale: float, anchor: tuple[int, int], cell: int, target_anchor: tuple[int, int]) -> Image.Image:
    scaled_size = (round(master.width * scale), round(master.height * scale))
    reduced = master.resize(scaled_size, Image.Resampling.NEAREST)
    atlas = Image.new("RGBA", (6 * cell, 4 * cell), (0, 0, 0, 0))
    # One sheet-wide source anchor / scale; per-frame source locations are retained.
    source_anchor_x, source_anchor_y = anchor
    dx = round(target_anchor[0] - source_anchor_x * scale)
    dy = round(target_anchor[1] - source_anchor_y * scale)
    for row in range(4):
        for column in range(6):
            tile = reduced.crop((round(column * CELL * scale), round(row * CELL * scale), round((column + 1) * CELL * scale), round((row + 1) * CELL * scale)))
            atlas.alpha_composite(tile, (column * cell + dx, row * cell + dy))
    return atlas


def preview_gif(atlas: Image.Image, row: int, multiplier: int, path: Path, cell: int) -> None:
    frames = []
    for col in range(6):
        frame = atlas.crop((col * cell, row * cell, (col + 1) * cell, (row + 1) * cell))
        if multiplier != 1:
            frame = frame.resize((cell * multiplier, cell * multiplier), Image.Resampling.NEAREST)
        # GIF is only a diagnostic playback format, so its palette is intentionally pending.
        frames.append(frame.convert("P", palette=Image.Palette.ADAPTIVE, colors=256))
    frames[0].save(path, save_all=True, append_images=frames[1:], duration=167, loop=0, disposal=2, transparency=0)

def checker_contact(atlas: Image.Image, path: Path) -> None:
    scaled = atlas.resize((atlas.width * 4, atlas.height * 4), Image.Resampling.NEAREST)
    background = Image.new("RGBA", scaled.size, (72, 78, 88, 255))
    draw = ImageDraw.Draw(background)
    for y in range(0, scaled.height, 16):
        for x in range(0, scaled.width, 16):
            if (x // 16 + y // 16) % 2:
                draw.rectangle((x, y, x + 15, y + 15), fill=(112, 118, 128, 255))
    background.alpha_composite(scaled)
    background.save(path)


def build_one(outfit: str, kind: str, filename: str, root: Path, source_path: Path | None = None, calibration: dict | None = None, proposal_dir: Path | None = None, broad_fringe: bool = False) -> dict:
    source = source_path or RAW / filename
    rgb, alpha, diagnostic = source_rgb_alpha(source, broad=broad_fringe)
    if rgb.size != (1536, 1024):
        raise ValueError(f"{source}: expected 1536x1024, got {rgb.size}")
    master = clean_master(rgb, alpha)
    proposal_dir = proposal_dir or root / outfit / "source-reference" / kind
    proposal_dir.mkdir(parents=True, exist_ok=True)
    master_path = proposal_dir / "transparent-clean-master.png"
    master.save(master_path)
    mask = Image.new("RGBA", rgb.size, (255, 255, 255, 255))
    mask.putalpha(alpha)
    mask.save(proposal_dir / "diagnostic-mask.png")
    reference = bounds(alpha, (0, 0, CELL, CELL))
    if reference is None:
        raise ValueError(f"{source}: empty standing reference")
    ref_height = calibration["standingBodyReference"]["height"] if calibration else reference[3] - reference[1]
    if ref_height <= 0:
        raise ValueError(f"{source}: invalid standing reference")
    scale = 32.0 / ref_height
    # Proposed, hash-bound source anchor: neutral row-0/cell-0 sole and centre.
    anchor = tuple(calibration["anchorProposal"]["source"]) if calibration else ((reference[0] + reference[2]) // 2, reference[3])
    atlas = native_atlas(master, scale, anchor, TARGET_CELL, TARGET_ANCHOR)
    atlas_path = proposal_dir / "native-atlas-32body-6x4.png"
    atlas.save(atlas_path)
    # Comparison-only study.  It cannot change 18 PPU, gameplay cells, pivots,
    # colliders or any accepted profile; it makes compression visible to review.
    density_scale = 64.0 / ref_height
    density_atlas = native_atlas(master, density_scale, anchor, 128, (64, 104))
    density_path = proposal_dir / "density-study-64body-6x4.png"
    density_atlas.save(density_path)
    frames = []
    for row in range(4):
        for column in range(6):
            rect = (column * CELL, row * CELL, (column + 1) * CELL, (row + 1) * CELL)
            body = bounds(alpha, rect)
            frames.append({"row": row, "column": column, "sourceRect": list(rect), "foregroundBounds": body, "proposedSourceAnchor": list(anchor), "targetAnchor": list(TARGET_ANCHOR), "scale": scale})
        preview_gif(atlas, row, 1, proposal_dir / f"row-{row + 1}-native.gif", TARGET_CELL)
        preview_gif(atlas, row, 4, proposal_dir / f"row-{row + 1}-4x.gif", TARGET_CELL)
    contact = atlas.resize((atlas.width * 4, atlas.height * 4), Image.Resampling.NEAREST)
    contact.save(proposal_dir / "contact-4x.png")
    checker_contact(atlas, proposal_dir / "contact-checker-4x.png")
    record = {"label": LABEL, "requirementIds": ["REQ-APV3-004", "REQ-APV3-006", "REQ-APV3-007", "REQ-APV3-008", "REQ-APV3-010"], "acceptanceState": "source_only", "source": str(source.relative_to(ROOT)).replace("\\", "/"), "sourceSha256": sha256(source), "sourceSize": list(rgb.size), "cellGrid": {"columns": 6, "rows": 4, "sourceCell": CELL, "nativeCell": TARGET_CELL}, "standingBodyReference": {"frame": [0, 0], "bounds": reference, "height": ref_height, "calibration": "a1 movement source reference" if calibration else "unreviewed alpha bbox (may include hair); manual scalp-to-sole review required"}, "sheetUniformScale": scale, "anchorProposal": {"source": list(anchor), "target": list(TARGET_ANCHOR), "status": "unreviewed"}, "diagnostic": diagnostic, "frames": frames, "outputs": {"cleanMasterSha256": sha256(master_path), "maskSha256": sha256(proposal_dir / "diagnostic-mask.png"), "atlas32Sha256": sha256(atlas_path), "densityStudy64Sha256": sha256(density_path)}, "densityStudy": {"status": "comparison_only", "bodyHeight": 64, "cell": 128, "scale": density_scale, "targetAnchor": [64, 104], "doesNotAdopt": ["18 PPU", "gameplay cell", "camera", "collider", "runtime pivot"]}, "palette": {"status": "pending", "reason": "No world/hero palette quantization is approved for this diagnostic candidate."}}
    stable_write(proposal_dir / "diagnostic.json", record)
    return record


def build(root: Path, fringe_v2: bool = False) -> dict:
    reference_name = "source-reference-v2-broad-fringe" if fringe_v2 else "source-reference"
    for outfit in SOURCES:
        source_reference = root / outfit / reference_name
        if source_reference.exists():
            shutil.rmtree(source_reference)
    records = [build_one(outfit, kind, file, root, proposal_dir=root / outfit / reference_name / kind, broad_fringe=fringe_v2) for outfit, entries in SOURCES.items() for kind, file in entries]
    receipt = {"label": LABEL, "schemaVersion": 1, "tool": "seryeong_appearance_v3/build_diagnostic.py", "pillow": Image.__version__, "records": [{"outfit": r["source"].split("/")[-1].split("-")[0], "source": r["source"], "sourceSha256": r["sourceSha256"], "atlas32Sha256": r["outputs"]["atlas32Sha256"], "densityStudy64Sha256": r["outputs"]["densityStudy64Sha256"]} for r in records], "acceptedAppearanceCount": 0, "note": "Twelve source-reference diagnostics only; no H mapping, composition, mirror, or acceptance."}
    stable_write(root / "build-receipt.json", receipt)
    return receipt


def build_hair_sources(root: Path) -> None:
    calibration = json.loads((root / "a1/source-reference/movement/diagnostic.json").read_text(encoding="utf-8"))
    for hair in ("h1", "h6", "h11"):
        source = root / "a1" / hair / "raw/movement-source-v1.png"
        if source.exists():
            build_one("a1", "movement", source.name, root, source_path=source, calibration=calibration, proposal_dir=root / "a1" / hair / "source-reference/movement")


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--output", type=Path, default=OUT)
    parser.add_argument("--process-hair-sources", action="store_true", help="process only supplied A1 hair source candidates using A1 movement calibration")
    parser.add_argument("--fringe-v2", action="store_true", help="write a separate broad edge-fringe diagnostic variant")
    args = parser.parse_args()
    if args.process_hair_sources:
        build_hair_sources(args.output)
    else:
        build(args.output, fringe_v2=args.fringe_v2)


if __name__ == "__main__":
    main()
