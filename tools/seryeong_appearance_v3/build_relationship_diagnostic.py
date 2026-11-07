"""Build source-only group diagnostics for the supplied A1/H1 interaction sheets.

The builder preserves each source cell as a group (solo, pair, or triplet).
It is deliberately not a compositor, actor splitter, or acceptance tool.
"""
from __future__ import annotations

import hashlib
import json
from pathlib import Path

from PIL import Image, ImageDraw

from build_diagnostic import ROOT, clean_master, flood_mask


RAW = ROOT / "images/sprites/seryeong-appearance-v3/native/a1/h1/raw"
OUTPUT = ROOT / "images/sprites/seryeong-appearance-v3/native/a1/h1/relationship-diagnostic-v2"
GRID_COLUMNS = 6
ATLAS_CELL = (256, 128)

SHEETS = (
    ("relationship", "relationship-source-v1.png", ("arm_link", "cheek_kiss_mutual_blush", "handhold"), ("pair", "pair", "pair"), ((0, 378, 45, 371), (378, 735, 402, 727), (735, 1024, 760, 1022))),
    ("shop", "shop-source-v2.png", ("lovers_shop_glare", "shop_row2_changed_pose_not_arm_link", "lovers_shop_glare"), ("triplet", "triplet", "triplet"), ((0, 346, 45, 338), (346, 677, 370, 660), (677, 1024, 700, 1000))),
    ("reactions", "reactions-source-v1.png", ("prelove_jealous", "prelove_confused", "lovers_shop_pull_doeon_outside"), ("solo", "solo", "triplet"), ((0, 342, 40, 322), (342, 682, 374, 652), (682, 1024, 725, 974))),
)
GROUP_WIDTH = {"solo": 128, "pair": 192, "triplet": 256}


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def bounds(alpha: Image.Image, rect: tuple[int, int, int, int]) -> list[int] | None:
    box = alpha.crop(rect).getbbox()
    return None if box is None else [rect[0] + box[0], rect[1] + box[1], rect[0] + box[2], rect[1] + box[3]]


def selected_group_tile(master: Image.Image, alpha: Image.Image, rect: tuple[int, int, int, int]) -> tuple[Image.Image, dict]:
    """Keep qualifying actual components, never an expanded rectangular crop.

    A cell's largest component establishes the local action-group scale. Every
    component at least 10% of that area is retained, which keeps separately
    connected partners while excluding the thin adjacent-cell fragments found
    in the v2 relationship diagnostic.
    """
    x0, y0, x1, y1 = rect
    px = alpha.load(); seen: set[tuple[int, int]] = set(); components = []
    for sy in range(y0, y1):
        for sx in range(x0, x1):
            if (sx, sy) in seen or not px[sx, sy]:
                continue
            todo = [(sx, sy)]; seen.add((sx, sy)); points = []
            while todo:
                x, y = todo.pop(); points.append((x, y))
                for nx, ny in ((x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1), (x - 1, y - 1), (x + 1, y - 1), (x - 1, y + 1), (x + 1, y + 1)):
                    if x0 <= nx < x1 and y0 <= ny < y1 and (nx, ny) not in seen and px[nx, ny]:
                        seen.add((nx, ny)); todo.append((nx, ny))
            components.append(points)
    if not components:
        raise ValueError(f"empty source cell: {rect}")
    largest = max(len(component) for component in components)
    retained = [component for component in components if len(component) >= largest * 0.10]
    tile = Image.new("RGBA", (x1 - x0, y1 - y0), (0, 0, 0, 0)); source = master.load(); target = tile.load()
    xs: list[int] = []; ys: list[int] = []
    for component in retained:
        for x, y in component:
            target[x - x0, y - y0] = source[x, y]; xs.append(x); ys.append(y)
    selected_bounds = [min(xs), min(ys), max(xs) + 1, max(ys) + 1]
    return tile, {"componentCount": len(components), "retainedComponentCount": len(retained), "largestComponentArea": largest, "minimumRetainedArea": largest * 0.10, "selectedBounds": selected_bounds}


def make_checker(size: tuple[int, int]) -> Image.Image:
    image = Image.new("RGBA", size, (76, 80, 90, 255))
    draw = ImageDraw.Draw(image)
    for y in range(0, size[1], 16):
        for x in range(0, size[0], 16):
            if (x // 16 + y // 16) % 2:
                draw.rectangle((x, y, x + 15, y + 15), fill=(118, 122, 132, 255))
    return image


def save_motion(atlas: Image.Image, destination: Path, multiplier: int) -> None:
    frames = []
    for column in range(GRID_COLUMNS):
        frame = atlas.crop((column * 256, 0, (column + 1) * 256, 128))
        if multiplier != 1:
            frame = frame.resize((frame.width * multiplier, frame.height * multiplier), Image.Resampling.NEAREST)
        frames.append(frame.convert("P", palette=Image.Palette.ADAPTIVE, colors=256))
    frames[0].save(destination, save_all=True, append_images=frames[1:], duration=167, loop=0, disposal=2, transparency=0)


def row_frame(master: Image.Image, alpha: Image.Image, row: int, group: str, calibration: tuple[int, int, int, int]) -> tuple[Image.Image, dict]:
    y1, y2, crown, sole = calibration
    row_rect = (0, y1, master.width, y2)
    source_bounds = bounds(alpha, row_rect)
    if source_bounds is None:
        raise ValueError(f"row {row + 1} has no foreground")
    source_height = sole - crown
    # The approved 64-body profile uses only the manually proposed Seryeong
    # crown-to-sole height.  Full-row/group width must never reduce this scale.
    scale = 64 / source_height
    frame = Image.new("RGBA", (GRID_COLUMNS * ATLAS_CELL[0], ATLAS_CELL[1]), (0, 0, 0, 0))
    clipping = source_bounds[0] == 0 or source_bounds[2] == master.width or source_bounds[1] == 0 or source_bounds[3] == master.height
    width_overflow = False
    cells = []
    for column in range(GRID_COLUMNS):
        source_rect = (column * 256, y1, (column + 1) * 256, y2)
        selected, selection = selected_group_tile(master, alpha, source_rect)
        group_image = selected.resize((round(256 * scale), round((y2 - y1) * scale)), Image.Resampling.NEAREST)
        # The source cell carries all people and anchors together.  No actor is
        # picked, detached, or re-anchored independently.
        target = Image.new("RGBA", ATLAS_CELL, (0, 0, 0, 0))
        x = (256 - group_image.width) // 2
        y = round(128 - (sole - y1) * scale)
        target.alpha_composite(group_image, (x, y))
        frame.alpha_composite(target, (column * 256, 0))
        cell_bounds = selection["selectedBounds"]
        cell_overflow = round((cell_bounds[2] - cell_bounds[0]) * scale) > GROUP_WIDTH[group]
        width_overflow = width_overflow or cell_overflow
        cells.append({"column": column, "sourceRect": list(source_rect), "foregroundBounds": cell_bounds, "groupPreserved": True, "componentSelection": selection, "widthOverflowFailsClosed": cell_overflow})
    record = {
        "row": row, "sourceRowBounds": [y1, y2], "groupType": group, "targetGroupBounds": [GROUP_WIDTH[group], 128],
        "sourceCrownSoleApproximation": {"crown": crown, "sole": sole, "height": source_height, "status": "UNREVIEWED", "method": "manual Seryeong crown-to-sole proposal"},
        "fixedScaleForRow": scale, "worldAndPartnerAnchors": "preserve_group_within_frame", "cutBodyAtCanvasBorder": clipping,
        "warnings": ["manual_review_required"] + (["foreground_touches_canvas_border"] if clipping else []) + (["group_width_exceeds_target_profile_fail_clip"] if width_overflow else []), "widthOverflowFailsClosed": width_overflow, "cells": cells,
    }
    return frame, record


def build_sheet(name: str, filename: str, action_ids: tuple[str, ...], groups: tuple[str, ...], calibrations: tuple[tuple[int, int, int, int], ...], output: Path) -> dict:
    source = RAW / filename
    rgb = Image.open(source).convert("RGB")
    if rgb.size != (1536, 1024):
        raise ValueError(f"{source}: expected 1536x1024")
    alpha, mask_diagnostic = flood_mask(rgb, broad=True)
    master = clean_master(rgb, alpha)
    folder = output / name
    folder.mkdir(parents=True, exist_ok=True)
    master.save(folder / "transparent-clean-master.png")
    mask = Image.new("RGBA", rgb.size, (255, 255, 255, 255)); mask.putalpha(alpha); mask.save(folder / "diagnostic-mask.png")
    rows = []
    for row, (action_id, group, calibration) in enumerate(zip(action_ids, groups, calibrations)):
        rendered, row_record = row_frame(master, alpha, row, group, calibration)
        atlas_path = folder / f"native-atlas-64body-6x1-row-{row + 1}.png"
        rendered.save(atlas_path)
        checker = make_checker(rendered.size); checker.alpha_composite(rendered); checker.save(folder / f"contact-checker-native-row-{row + 1}.png")
        save_motion(rendered, folder / f"row-{row + 1}-native.gif", 1)
        save_motion(rendered, folder / f"row-{row + 1}-4x.gif", 4)
        status = "rejected" if action_id == "shop_row2_changed_pose_not_arm_link" or row_record["widthOverflowFailsClosed"] else "source_only"
        row_record.update({"actionId": action_id, "status": status, "outputs": {"nativeAtlas": atlas_path.name, "nativeAtlasSha256": sha256(atlas_path)}})
        rows.append(row_record)
    record = {
        "schemaVersion": 1, "label": "group diagnostic candidate — source-only — not Accepted", "acceptanceState": "source_only", "acceptedCombinationCount": 0,
        "source": source.relative_to(ROOT).as_posix(), "sourceSha256": sha256(source), "sourceSize": list(rgb.size),
        "maskDiagnostic": mask_diagnostic, "rowCalibration": "mixed action-family rows allowed; one fixed scale per row, never per pose", "rows": rows,
        "outputs": {"rowAtlases": [row["outputs"] for row in rows]}, "supersedes": {"path": "native/a1/h1/relationship-diagnostic-v1", "status": "RejectedScale", "reason": "v1 fitted scale to full row width and did not satisfy the 64-body profile"},
        "shopConstraint": "shop row 2 changed pose is not Accepted arm_link" if name == "shop" else None,
    }
    (folder / "diagnostic.json").write_text(json.dumps(record, ensure_ascii=False, indent=2, sort_keys=True) + "\n", encoding="utf-8")
    return record


def build(output: Path = OUTPUT) -> list[dict]:
    return [build_sheet(name, filename, actions, groups, calibrations, output) for name, filename, actions, groups, calibrations in SHEETS]


if __name__ == "__main__":
    build()
