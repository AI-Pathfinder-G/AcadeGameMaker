"""Expand the approved Seryeong appearance-v3 source-only inventory.

This module intentionally records plans and source provenance only.  It never
creates images, atlas frames, output hashes, or acceptance claims.
"""

from __future__ import annotations

import hashlib
import json
from pathlib import Path
from typing import Any, Iterable

from PIL import Image, ImageDraw


REPOSITORY_ROOT = Path(__file__).resolve().parents[2]
CONTRACT_ID = "2026-09-13-seryeong-appearance-combinations-v3"
FIRST_SLICE_COSTUMES = ("a1", "a2", "c2", "d3")
FACING_IDS = ("left", "right")

# The ordered list is contractual.  Do not merge draw/release or either
# recovery state: coverage and later review are per named action.
ACTIONS = (
    ("idle", "normal", 6, {"scene": "gameplay", "partnerState": "none"}),
    ("walk", "normal", 6, {"scene": "gameplay", "partnerState": "none"}),
    ("run", "normal", 6, {"scene": "gameplay", "partnerState": "none"}),
    ("jump", "normal", 6, {"scene": "gameplay", "partnerState": "none"}),
    ("land", "normal", 6, {"scene": "gameplay", "partnerState": "none"}),
    ("evade_dash", "normal", 6, {"scene": "gameplay", "partnerState": "none"}),
    ("locomotion_recover", "normal", 6, {"scene": "gameplay", "partnerState": "none"}),
    ("bow_aim", "combat", 6, {"scene": "combat", "partnerState": "none"}),
    ("bow_draw_hold", "combat", 6, {"scene": "combat", "partnerState": "none"}),
    ("bow_release", "combat", 6, {"scene": "combat", "partnerState": "none"}),
    ("bow_recover", "combat", 6, {"scene": "combat", "partnerState": "none"}),
    ("hit_recover", "combat", 6, {"scene": "combat", "partnerState": "none"}),
    ("forward_stretch", "village_solo", 6, {"scene": "village", "partnerState": "solo"}),
    ("folded_arm_gaze_doeon", "village_solo", 6, {"scene": "village", "partnerState": "doeon_staged"}),
    ("writing", "village_solo", 6, {"scene": "village", "partnerState": "solo"}),
    ("supply_gesture", "village_solo", 6, {"scene": "village", "partnerState": "solo"}),
    ("arm_link", "doeon_relationship", 8, {"scene": "relationship", "partnerState": "doeon_required"}),
    ("cheek_kiss_mutual_blush", "doeon_relationship", 8, {"scene": "relationship", "partnerState": "doeon_required", "beats": ["approach", "contact", "mutual_reaction", "recovery"]}),
    ("handhold", "doeon_relationship", 8, {"scene": "relationship", "partnerState": "doeon_required"}),
    ("prelove_jealous", "doeon_relationship", 8, {"scene": "seoha_shop", "partnerState": "doeon_and_seoha_required", "relationship": "not_lovers", "preloveAccompaniedShop": True, "seryeongPresent": True}),
    ("prelove_confused", "doeon_relationship", 8, {"scene": "seoha_shop", "partnerState": "doeon_and_seoha_required", "relationship": "not_lovers", "preloveAccompaniedShop": True, "seryeongPresent": True}),
    ("lovers_shop_glare", "lovers_shop", 8, {"scene": "seoha_shop", "partnerState": "doeon_and_seoha_required", "relationship": "lovers"}),
    ("lovers_shop_block_flirt", "lovers_shop", 8, {"scene": "seoha_shop", "partnerState": "doeon_and_seoha_required", "relationship": "lovers"}),
    ("lovers_shop_pull_doeon_outside", "lovers_shop", 8, {"scene": "seoha_shop", "partnerState": "doeon_and_seoha_required", "relationship": "lovers"}),
    ("non_lovers_seryeong_absence", "non_lovers_shop", 0, {"scene": "seoha_shop_external", "partnerState": "doeon_and_seoha_or_required_actors_staged", "relationship": "not_lovers", "seryeongPresent": False, "doeonSoloVisit": True, "absenceCondition": "external_scene_only"}),
)

HAIR_LABELS = (
    "long_straight_side_part", "voluminous_long_waves", "layered_shoulder_chest_waves_curtain_bangs",
    "low_half_up_clip_straight_long", "high_half_ponytail_waves", "straight_blunt_fringe_long",
    "high_full_ponytail", "low_side_ponytail_image_left", "single_side_braid_image_left",
    "braided_roots_high_ponytail", "low_braided_bun", "high_bun_hairpin_tendrils",
)
HAIR_SHEETS = (
    "images/seryeong/shoulder-v2/hair-v1/01-loose-and-halfup.png",
    "images/seryeong/shoulder-v2/hair-v1/02-tied-and-braided.png",
)


def _sha256(relative_path: str) -> str:
    return hashlib.sha256((REPOSITORY_ROOT / relative_path).read_bytes()).hexdigest()


def _load_costumes() -> list[dict[str, Any]]:
    data = json.loads((REPOSITORY_ROOT / "images/sprites/costumes-v2/costume-inventory.json").read_text(encoding="utf-8"))
    return data["costumes"]


def hair_inventory() -> list[dict[str, Any]]:
    """Return H1..H12 with source-sheet hashes and label-free figure rectangles."""
    inventory = []
    for index, label in enumerate(HAIR_LABELS):
        sheet = HAIR_SHEETS[index // 6]
        figure = index % 6
        column, row = figure % 3, figure // 3
        inventory.append({
            "id": f"h{index + 1}",
            "label": label,
            "reference": sheet,
            "sourceSha256": _sha256(sheet),
            "figureIndex": figure,
            "figureRect": [column * 512, row * 512, 512, 460],
            "sourceComposition": "labeled_figure_reference",
            "notHairMask": True,
        })
    return inventory


def appearance_id(costume: dict[str, Any], hair: dict[str, Any]) -> str:
    return f"seryeong.appearance.{costume['option']}.{hair['id']}.v3"


def _slice_source_pointers(costume: dict[str, Any], hair: dict[str, Any]) -> dict[str, Any] | None:
    if costume["option"] not in FIRST_SLICE_COSTUMES:
        return None
    return {
        "costumeReference": {"path": costume["reference"], "sha256": costume["sourceSha256"], "figureIndex": costume["referenceFigureIndex"]},
        "hairReference": {"path": hair["reference"], "sha256": hair["sourceSha256"], "figureIndex": hair["figureIndex"], "figureRect": hair["figureRect"]},
        "hairMappingStatus": "unmapped",
        "note": "Diagnostic source pointer only; it is not an extraction, composition, or accepted appearance mapping.",
    }


def appearance_inventory() -> dict[str, Any]:
    costumes = _load_costumes()
    hairs = hair_inventory()
    pairs = []
    for costume in costumes:
        for hair in hairs:
            item = {
                "id": appearance_id(costume, hair),
                "costumeId": costume["option"],
                "costumeRecordId": costume["id"],
                "hairId": hair["id"],
                "readiness": "production_pending",
                "reviewState": "source_only",
                "referenceHairSource": {
                    "status": "unmapped_from_costumes_v2_inventory",
                    "mapping": "not_asserted",
                    "note": "The existing costume inventory does not identify a separate original hair asset. This field preserves that unresolved provenance without inventing an H mapping.",
                },
            }
            pointers = _slice_source_pointers(costume, hair)
            if pointers is not None:
                item["diagnosticSourcePointers"] = pointers
            pairs.append(item)
    return {
        "schemaVersion": 1,
        "contractId": CONTRACT_ID,
        "readiness": "source_only",
        "acceptedCombinationCount": 0,
        "costumes": costumes,
        "hairs": hairs,
        "appearances": pairs,
        "firstSlice": {"costumeIds": list(FIRST_SLICE_COSTUMES), "combinationCount": 48, "readiness": "production_pending"},
    }


def coverage_inventory(appearances: Iterable[dict[str, Any]] | None = None) -> dict[str, Any]:
    """Create coverage rows without allocating output files or native frame records."""
    appearances = list(appearances if appearances is not None else appearance_inventory()["appearances"])
    rows = []
    for appearance in appearances:
        for action_id, context, required_frames, gate in ACTIONS:
            absence = action_id == "non_lovers_seryeong_absence"
            facings = ("declarative_absence",) if absence else FACING_IDS
            for facing in facings:
                row = {
                    "appearanceId": appearance["id"],
                    "actionId": action_id,
                    "context": context,
                    "sceneGate": gate,
                    "facing": facing,
                    "requiredFrames": required_frames,
                    "readiness": "production_pending",
                    "reviewState": "source_only",
                    "output": None,
                }
                if absence:
                    row.update({
                        "presentation": "authored_solo_absence",
                        "absenceIsNotSprite": True,
                        "nativeFrameOutputs": [],
                        "note": "Declarative staging coverage only. No transparent PNG, atlas frame, or facing substitute is permitted.",
                    })
                else:
                    row.update({
                        "presentation": "authored_sprite_required",
                        "absenceIsNotSprite": False,
                    })
                rows.append(row)
    return {
        "schemaVersion": 1,
        "contractId": CONTRACT_ID,
        "readiness": "source_only",
        "acceptedCombinationCount": 0,
        "actionDefinitions": [
            {"id": action_id, "context": context, "requiredFrames": frames, "sceneGate": gate}
            for action_id, context, frames, gate in ACTIONS
        ],
        "coverage": rows,
    }


def _relative(path: Path) -> str:
    return path.relative_to(REPOSITORY_ROOT).as_posix()


def _existing_media(*paths: Path) -> list[dict[str, str]]:
    return [{"path": _relative(path), "kind": path.suffix.lstrip(".")} for path in paths if path.is_file()]


def _preferred_movement_paths(native: Path, costume: str, hair: str) -> tuple[Path, Path, str, list[Path]]:
    """Return the reviewed source revision and its matching V4 folder.

    Revision selection affects gallery routing only.  It never changes the
    source-only acceptance gate or removes earlier raw files from disk.
    """
    raw_root = native / costume / hair / "raw"
    corrected = (costume, hair) in {("a2", "h12"), ("d3", "h8"), ("d3", "h9")}
    raw = raw_root / ("movement-source-v2.png" if corrected else "movement-source-v1.png")
    historical = [raw_root / "movement-source-v1.png"] if corrected else []
    v4_kind = "movement-v2" if (costume, hair) in {("d3", "h8"), ("d3", "h9")} else "movement"
    return raw, native / costume / hair / "source-reference-v4-matte" / v4_kind, v4_kind, historical


def write_first_slice_idle_contact(output_dir: Path) -> Path | None:
    """Create a compact review sheet only when all 48 actual V4 atlases exist."""
    native = REPOSITORY_ROOT / "images/sprites/seryeong-appearance-v3/native"
    cells: list[tuple[str, str, Image.Image]] = []
    for costume in FIRST_SLICE_COSTUMES:
        for hair_number in range(1, 13):
            hair = f"h{hair_number}"
            _, folder, _, _ = _preferred_movement_paths(native, costume, hair)
            atlas_path = folder / "native-atlas-64body-6x4.png"
            if not atlas_path.is_file():
                return None
            with Image.open(atlas_path) as atlas:
                if atlas.size != (768, 512):
                    return None
                cells.append((costume, hair, atlas.convert("RGBA").crop((0, 0, 128, 128))))
    canvas = Image.new("RGBA", (12 * 128, 24 + 4 * 148), (28, 29, 34, 255))
    draw = ImageDraw.Draw(canvas)
    for column in range(12):
        draw.text((column * 128 + 4, 4), f"H{column + 1}", fill=(235, 235, 240, 255))
    for row, costume in enumerate(FIRST_SLICE_COSTUMES):
        y = 24 + row * 148
        draw.text((4, y + 130), costume.upper(), fill=(235, 235, 240, 255))
        for column in range(12):
            _, _, cell = cells[row * 12 + column]
            canvas.alpha_composite(cell, (column * 128, y))
    destination = output_dir / "first-slice-idle-64body-contact.png"
    canvas.save(destination)
    return destination


def candidate_media_inventory() -> dict[str, Any]:
    """Index actual diagnostic candidates without promoting them into coverage output.

    This scan is intentionally limited to the first-slice diagnostics and the
    twelve supplied A1 hair sources.  It records only existing local files;
    planned shop/prelove work is a pending-root marker, never a broken link.
    """
    native = REPOSITORY_ROOT / "images/sprites/seryeong-appearance-v3/native"
    candidates: list[dict[str, Any]] = []
    for costume in FIRST_SLICE_COSTUMES:
        for context in ("movement", "combat", "village"):
            v4 = native / costume / "source-reference-v4-matte" / context
            v3 = native / costume / "source-reference-v3-components" / context
            folder = v4 if (v4 / "native-atlas-64body-6x4.png").is_file() else v3
            diagnostic_version = "v4_matte" if folder == v4 else "v3_components"
            candidates.append({
                "id": f"diagnostic.{costume}.{diagnostic_version}.{context}",
                "costumeId": costume,
                "hairId": None,
                "context": {"movement": "normal", "combat": "combat", "village": "village_solo"}[context],
                "mediaContext": context,
                "status": "source_only_candidate",
                "acceptanceState": "source_only",
                "note": f"{diagnostic_version} diagnostic candidate. It has no H mapping and is not a completed appearance combination.",
                "media": _existing_media(
                    folder / "native-atlas-64body-6x4.png", folder / "native-atlas-32body-6x4.png", folder / "contact-4x.png", folder / "density-study-64body-6x4.png",
                    *(folder / f"density-row-{row}-{scale}.gif" for row in range(1, 5) for scale in ("native", "4x")),
                    *(folder / f"row-{row}-{scale}.gif" for row in range(1, 5) for scale in ("native", "4x")),
                ),
                "densityStudy": {"atlas32": _relative(folder / "native-atlas-32body-6x4.png"), "atlas64Contact": _relative(folder / "native-atlas-64body-6x4.png"), "has64MotionGif": (folder / "density-row-1-native.gif").is_file()},
            })
    for hair in tuple(f"h{i}" for i in range(1, 13)):
        raw = native / "a1" / hair / "raw/movement-source-v1.png"
        v4 = native / "a1" / hair / "source-reference-v4-matte/movement"
        v3 = native / "a1" / hair / "source-reference-v3-components/movement"
        processed = v4 if (v4 / "native-atlas-64body-6x4.png").is_file() else v3
        diagnostic_version = "v4_matte" if processed == v4 else "v3_components"
        candidates.append({
            "id": f"candidate.a1.{hair}.movement", "costumeId": "a1", "hairId": hair, "context": "normal", "mediaContext": "movement",
            "status": "source_only_candidate", "acceptanceState": "source_only",
            "note": f"Supplied raw hair source; preferred {diagnostic_version} diagnostic is listed only when available. Full action/facing matrix remains pending.",
            "media": _existing_media(raw, processed / "native-atlas-64body-6x4.png", processed / "native-atlas-32body-6x4.png", processed / "contact-4x.png", *(processed / f"density-row-{row}-{scale}.gif" for row in range(1, 5) for scale in ("native", "4x")), *(processed / f"row-{row}-{scale}.gif" for row in range(1, 5) for scale in ("native", "4x"))),
            "densityStudy": {"atlas32": _relative(processed / "native-atlas-32body-6x4.png"), "atlas64Contact": _relative(processed / "native-atlas-64body-6x4.png"), "has64MotionGif": (processed / "density-row-1-native.gif").is_file()},
        })
    h1raw = native / "a1/h1/raw"
    candidates.extend((
        {"id": "candidate.a1.h1.combat", "costumeId": "a1", "hairId": "h1", "context": "combat", "mediaContext": "combat", "status": "source_only_candidate", "acceptanceState": "source_only", "note": "Hair-corrected 6×4 raw combat source only; no native review, facing matrix, or acceptance credit exists.", "media": _existing_media(h1raw / "combat-source-v1.png")},
        {"id": "candidate.a1.h1.relationship", "costumeId": "a1", "hairId": "h1", "context": "doeon_relationship", "mediaContext": "relationship", "status": "source_only_candidate", "acceptanceState": "source_only", "note": "Raw relationship source only; native processing, full coverage and review are pending.", "media": _existing_media(h1raw / "relationship-source-v1.png")},
        {"id": "candidate.a1.h1.shop", "costumeId": "a1", "hairId": "h1", "context": "lovers_shop", "mediaContext": "shop", "status": "rejected", "acceptanceState": "rejected", "rejectionReason": "RejectedHairRow2: row 2 retains old tied/ponytail hair and fails H1 loose-straight consistency.", "note": "Shown for audit only. It must not be promoted into an appearance or coverage output.", "media": _existing_media(h1raw / "shop-source-v1.png")},
    ))
    # First-slice raw movement sources and their V4 diagnostics are gallery
    # evidence only.  They do not create native-frame records or acceptance.
    for costume in ("a2", "c2", "d3"):
        for hair in tuple(f"h{i}" for i in range(1, 13)):
            raw, v4, v4_kind, historical = _preferred_movement_paths(native, costume, hair)
            if raw.is_file():
                media = _existing_media(
                    raw, v4 / "native-atlas-64body-6x4.png", v4 / "contact-checker-4x.png",
                    *(v4 / f"row-{row}-{scale}.gif" for row in range(1, 5) for scale in ("native", "4x")),
                )
                candidates.append({
                    "id": f"candidate.{costume}.{hair}.movement", "costumeId": costume, "hairId": hair,
                    "context": "normal", "mediaContext": "movement", "status": "source_only_candidate",
                    "acceptanceState": "source_only", "preferredSourceRevision": raw.name,
                    "preferredV4Kind": v4_kind,
                    "note": "Preferred raw revision and any actual V4 diagnostic are source-only review evidence; both facings, full matrix review, and acceptance remain pending.",
                    "media": media,
                    "history": [{"path": _relative(path), "status": "rejected_history"} for path in historical if path.is_file()],
                })
    group_root = native / "a1/h1/relationship-diagnostic-v2"
    for context, coverage_context in (("relationship", "doeon_relationship"), ("shop", "lovers_shop"), ("reactions", "doeon_relationship")):
        folder = group_root / context
        candidates.append({
            "id": f"candidate.a1.h1.group_diagnostic.{context}", "costumeId": "a1", "hairId": "h1", "context": coverage_context, "mediaContext": context,
            "status": "source_only_candidate", "acceptanceState": "source_only", "note": "Group-preserving 64-body diagnostic candidate; it is not a completed combination or native acceptance.",
            "media": _existing_media(*(folder / f"native-atlas-64body-6x1-row-{row}.png" for row in range(1, 4)), *(folder / f"contact-checker-native-row-{row}.png" for row in range(1, 4)), *(folder / f"row-{row}-{scale}.gif" for scale in ("native", "4x") for row in range(1, 4))),
        })
    v1_history = native / "a1/h1/relationship-diagnostic-v1/relationship"
    candidates.append({
        "id": "candidate.a1.h1.group_diagnostic.v1_history", "costumeId": "a1", "hairId": "h1", "context": "doeon_relationship", "mediaContext": "relationship",
        "status": "rejected", "acceptanceState": "rejected", "rejectionReason": "RejectedScale: v1 fitted scale to full row width and is retained only as diagnostic history.",
        "note": "Historical rejected diagnostic; relationship-diagnostic-v2 is the preferred group candidate and remains source-only.",
        "media": _existing_media(v1_history / "native-atlas-64body-6x4.png", v1_history / "row-1-native.gif"),
    })
    return {
        "schemaVersion": 1, "contractId": CONTRACT_ID, "acceptedCombinationCount": 0, "readiness": "source_only",
        "candidates": candidates,
        "pendingRoots": [
            {"id": "a1_h1_shop_v2", "root": _relative(h1raw), "context": "shop", "status": "pending_not_present", "note": "A replacement shop v2 result is pending; the rejected v1 source remains rejected."},
            {"id": "a1_h1_prelove", "root": _relative(h1raw), "context": "prelove", "status": "pending_not_present", "note": "A prelove result is pending; no source media exists yet."},
        ],
    }


def validate(inventory: dict[str, Any], coverage: dict[str, Any]) -> None:
    costumes, hairs, appearances = inventory["costumes"], inventory["hairs"], inventory["appearances"]
    if len(costumes) != 36 or len(hairs) != 12 or len(appearances) != 432:
        raise ValueError("expected exactly 36 costumes, 12 hairs, and 432 appearances")
    if len({c["option"] for c in costumes}) != 36 or len({h["id"] for h in hairs}) != 12:
        raise ValueError("duplicate costume or hair identifier")
    if len({p["id"] for p in appearances}) != 432:
        raise ValueError("duplicate appearance identifier")
    if len(ACTIONS) != 25 or len({action[0] for action in ACTIONS}) != 25:
        raise ValueError("actions must be the 25 unique contractual actions")
    if sum(p["costumeId"] in FIRST_SLICE_COSTUMES for p in appearances) != 48:
        raise ValueError("first slice must contain exactly 48 appearances")
    for source in [*costumes, *hairs]:
        path, digest = source["reference"], source["sourceSha256"]
        if _sha256(path) != digest:
            raise ValueError(f"source hash mismatch: {path}")
    expected_keys = {
        (appearance["id"], action_id, facing)
        for appearance in appearances
        for action_id, _, _, _ in ACTIONS
        for facing in (("declarative_absence",) if action_id == "non_lovers_seryeong_absence" else FACING_IDS)
    }
    actual_keys = {(row["appearanceId"], row["actionId"], row["facing"]) for row in coverage["coverage"]}
    if len(coverage["coverage"]) != len(expected_keys) or actual_keys != expected_keys:
        raise ValueError("coverage action matrix mismatch")
    for row in coverage["coverage"]:
        if row["actionId"] == "non_lovers_seryeong_absence":
            if row["facing"] != "declarative_absence" or not row["absenceIsNotSprite"] or row["output"] is not None:
                raise ValueError("non-lovers absence must remain a declarative non-sprite record")
        elif row["facing"] not in FACING_IDS or row["output"] is not None:
            raise ValueError("sprite coverage must have an authored facing and no pre-generated output")


def write_manifests(output_dir: Path | None = None) -> tuple[Path, Path]:
    output_dir = output_dir or REPOSITORY_ROOT / "images/sprites/seryeong-appearance-v3/manifest"
    inventory = appearance_inventory()
    coverage = coverage_inventory(inventory["appearances"])
    validate(inventory, coverage)
    output_dir.mkdir(parents=True, exist_ok=True)
    inventory_path, coverage_path = output_dir / "inventory.json", output_dir / "coverage.json"
    inventory_path.write_text(json.dumps(inventory, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    coverage_path.write_text(json.dumps(coverage, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    candidates = candidate_media_inventory()
    candidate_path = output_dir / "candidate-media.json"
    candidate_path.write_text(json.dumps(candidates, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    write_first_slice_idle_contact(output_dir)
    write_viewer(output_dir / "viewer.html", inventory, candidates)
    return inventory_path, coverage_path


def write_viewer(path: Path, inventory: dict[str, Any], candidates: dict[str, Any]) -> None:
    """Write a standalone local-file viewer with embedded source-only metadata."""
    viewer_metadata = {
        "costumes": [{"id": c["option"], "name": c["name"], "reference": c["reference"], "figureIndex": c["referenceFigureIndex"]} for c in inventory["costumes"]],
        "hairs": inventory["hairs"],
        "appearances": inventory["appearances"],
        "actions": [{"id": a[0], "context": a[1], "requiredFrames": a[2]} for a in ACTIONS], "candidateMedia": candidates,
    }
    metadata = json.dumps(viewer_metadata, ensure_ascii=False).replace("</", "<\\/")
    path.write_text(f"""<!doctype html>
<html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">
<title>Seryeong appearance v3 source inventory</title>
<style>
body{{font:14px system-ui,sans-serif;margin:24px;background:#17151b;color:#eee}} main{{max-width:1080px;margin:auto}} label{{display:inline-grid;gap:4px;margin:0 12px 12px 0}} select{{padding:6px}} #summary{{color:#f0c674}} #result,#candidates{{display:grid;grid-template-columns:repeat(auto-fit,minmax(270px,1fr));gap:16px}} article{{background:#292530;padding:12px;border:1px solid #4e4758;border-radius:8px}} .hairCrop{{width:256px;height:230px;background-repeat:no-repeat;background-size:768px 1024px;border:1px solid #625b6b;background-color:#0d0c10}} img{{width:100%;max-height:180px;object-fit:contain;background:#0d0c10}} small{{display:block;color:#bcb4c6;margin-top:8px}} a{{color:#93c5fd}} .rejected{{border-color:#ef4444}} .media{{display:flex;flex-wrap:wrap;gap:8px;margin-top:8px}}
</style></head><body><main>
<h1>Seryeong appearance v3 — source-only reference inventory</h1>
<p>This view shows a selected <em>reference pair</em>, never a composited final portrait. All combinations remain production pending; source references and CSS crops are review aids only.</p>
<label>Costume <select id=\"costume\"></select></label><label>Hair <select id=\"hair\"></select></label><label>Context <select id=\"context\"></select></label>
<p id=\"summary\"></p><section id=\"result\"></section><h2>Actual diagnostic candidates</h2><p>These local media files are separate from the pending coverage matrix. Source presence is not completion or acceptance.</p><section id=\"candidates\"></section>
<script id=\"appearance-metadata\" type=\"application/json\">{metadata}</script>
<script>
const data=JSON.parse(document.getElementById('appearance-metadata').textContent), $=id=>document.getElementById(id);
const rel=p=>'../../../../'+p;
function fill(id,items,label){{$(id).innerHTML='<option value=\"\">All</option>'+items.map(x=>`<option value="${{x.id}}">${{label(x)}}</option>`).join('')}}
fill('costume',data.costumes,x=>`${{x.id}} — ${{x.name}}`); fill('hair',data.hairs,x=>`${{x.id}} — ${{x.label}}`); fill('context',[...new Map(data.actions.map(x=>[x.context,x])).values()],x=>x.context);
function render(){{const c=$('costume').value,h=$('hair').value,ctx=$('context').value; const pairs=data.appearances.filter(p=>(!c||p.costumeId===c)&&(!h||p.hairId===h)); const actions=data.actions.filter(a=>!ctx||a.context===ctx); const absent=actions.filter(a=>a.id==='non_lovers_seryeong_absence').length; const missing=pairs.length*(actions.length-absent)*2+pairs.length*absent; $('summary').textContent=`${{pairs.length}} reference pair(s); ${{missing}} required coverage record(s) are pending, 0 accepted. The non-lovers absence is conditional external-scene staging only, never a transparent sprite or a rule to hide Seryeong during accompanied prelove shop scenes.`; $('result').innerHTML=pairs.slice(0,48).map(p=>{{const costume=data.costumes.find(x=>x.id===p.costumeId),hair=data.hairs.find(x=>x.id===p.hairId),[x,y,w,hh]=hair.figureRect; return `<article><strong>${{p.id}}</strong><div class="hairCrop" title="${{hair.reference}} figure ${{hair.figureIndex}}" style="background-image:url('${{rel(hair.reference)}}');background-position:-${{x/2}}px -${{y/2}}px"></div><small>Hair reference crop: ${{hair.id}}, label-free ${{w}}×${{hh}} source region.</small><img alt="Costume source reference for ${{costume.id}}" src="${{rel(costume.reference)}}"><small>Costume source: ${{costume.reference}} · figure ${{costume.figureIndex}} · ${{p.readiness}}</small></article>`}}).join('')||'<p>No matching reference pair.</p>'; const candidates=data.candidateMedia.candidates.filter(m=>(!c||m.costumeId===c)&&(!h||m.hairId===h)&&(!ctx||m.context===ctx)); $('candidates').innerHTML=candidates.map(m=>{{const gifs=m.media.filter(x=>x.kind==='gif').slice(0,2), links=m.media.map(x=>`<a href="${{rel(x.path)}}" target="_blank">${{x.path.split('/').pop()}}</a>`).join('<br>'); const motion=gifs.map(x=>`<img alt="${{m.id}} motion" src="${{rel(x.path)}}">`).join(''); const study=m.densityStudy ? (m.densityStudy.has64MotionGif ? '<small>64-body motion GIF is available.</small>' : `<small>32/64 comparison: <a href="${{rel(m.densityStudy.atlas32)}}" target="_blank">32 atlas</a> · <a href="${{rel(m.densityStudy.atlas64Contact)}}" target="_blank">64 contact atlas</a>; no 64 motion GIF exists, so no study toggle is shown.</small>`) : ''; return `<article class="${{m.status==='rejected'?'rejected':''}}"><strong>${{m.id}}</strong><small>${{m.status}} · ${{m.note}}</small>${{m.rejectionReason?`<small>${{m.rejectionReason}}</small>`:''}}<div class="media">${{motion}}</div>${{study}}<small>${{links}}</small></article>`}}).join('')||'<p>No actual candidate media matches these filters.</p>'}}
['costume','hair','context'].forEach(id=>$(id).addEventListener('change',render)); render();
</script></main></body></html>\n""", encoding="utf-8")


def v8_inventory() -> dict[str, Any]:
    """Bounded V8 source ledger: 16 outfits × 12 hairs, all still pending."""
    reference_files = {
        "v": "01-bold-explorer-v8.png", "t": "02-tailored-formal-v8.png",
        "l": "03-elegant-lady-v8.png", "b": "04-bikini-v8.png",
    }
    outfits = []
    for prefix, filename in reference_files.items():
        reference = f"images/seryeong/shoulder-v2/v8/{filename}"
        for index in range(1, 5):
            outfits.append({"id": f"{prefix}{index}", "label": f"{prefix.upper()}{index}", "reference": reference, "referenceFigureIndex": index - 1, "sourceSha256": _sha256(reference)})
    hairs = hair_inventory()
    native = REPOSITORY_ROOT / "images/sprites/seryeong-appearance-v3/native"
    # These source revisions were explicitly reviewed.  Never select a later
    # filename merely because it sorts last: only this routing may replace V1.
    preferred_raw_revisions = {
        ("v1", "h1", "movement"): "movement-source-v3.png",
        ("v3", "h1", "movement"): "movement-source-v3.png",
        ("b2", "h1", "movement"): "movement-source-v2.png",
        ("t4", "h1", "movement"): "movement-source-v2.png",
        ("l1", "h1", "movement"): "movement-source-v2.png",
        ("l2", "h1", "movement"): "movement-source-v2.png",
        ("l4", "h1", "movement"): "movement-source-v2.png",
        ("t1", "h1", "combat"): "combat-source-v2.png",
        ("t2", "h1", "combat"): "combat-source-v2.png",
    }
    sources = []
    for outfit in outfits:
        for hair in hairs:
            raw_root = native / outfit["id"] / hair["id"] / "raw"
            for kind, label in (("movement", "이동"), ("combat", "전투"), ("village", "마을"), ("relationship", "관계")):
                v4 = native / outfit["id"] / hair["id"] / "source-reference-v4-matte" / kind
                atlas = v4 / "native-atlas-64body-6x4.png"
                media = [_relative(path) for path in (atlas, *(v4 / f"row-{row}-{scale}.gif" for row in range(1,5) for scale in ("native","4x"))) if path.is_file()]
                raw_name = preferred_raw_revisions.get(
                    (outfit["id"], hair["id"], kind), f"{kind}-source-v1.png"
                )
                raw = raw_root / raw_name
                if media or raw.is_file():
                    history = [
                        {"path": _relative(candidate), "status": "rejected_history"}
                        for candidate in sorted(raw_root.glob(f"{kind}-source-v*.png"))
                        if candidate != raw
                    ]
                    sources.append({"costumeId":outfit["id"],"hairId":hair["id"],"kind":kind,"kindLabel":label,"status":"source_only","path":_relative(raw if raw.is_file() else atlas),"sha256":_sha256(_relative(raw)) if raw.is_file() else None,"preferredSourceRevision":raw.name if raw.is_file() else None,"nativeV4":media,"rawPath":_relative(raw) if raw.is_file() else None,"history":history})
            diagnostic_root = native / outfit["id"] / hair["id"] / "relationship-diagnostic-v8"
            diagnostic_atlas = diagnostic_root / "native-atlas-pair-192x128-4x4.png"
            if diagnostic_atlas.is_file():
                diagnostic_media = [_relative(path) for path in (diagnostic_atlas, *(diagnostic_root / f"row-{row}-{scale}.gif" for row in range(1, 5) for scale in ("native", "4x"))) if path.is_file()]
                relationship = next((item for item in reversed(sources) if item["costumeId"] == outfit["id"] and item["hairId"] == hair["id"] and item["kind"] == "relationship"), None)
                if relationship is None:
                    sources.append({"costumeId":outfit["id"],"hairId":hair["id"],"kind":"relationship","kindLabel":"관계","status":"source_only","path":_relative(diagnostic_atlas),"sha256":None,"preferredSourceRevision":None,"nativeV4":diagnostic_media,"rawPath":None,"history":[]})
                else:
                    relationship["nativeV4"] = diagnostic_media + relationship["nativeV4"]
    return {
        "schemaVersion": 1, "contractId": "2026-09-14-seryeong-v8-production",
        "acceptedCombinationCount": 0, "outfits": outfits, "hairs": hairs,
        "appearances": [{"id": f"seryeong.v8.{outfit['id']}.{hair['id']}", "costumeId": outfit["id"], "hairId": hair["id"], "readiness": "production_pending", "acceptanceState": "pending"} for outfit in outfits for hair in hairs],
        "sourceOnlyMedia": sources,
        "note": "Actual source-only candidates only. Both facings and the full action matrix remain pending; no candidate completes an appearance or acceptance.",
    }


def write_v8_viewer(path: Path, ledger: dict[str, Any]) -> None:
    """Write the V8 local viewer from the files that actually exist in the ledger."""
    metadata = json.dumps(ledger, ensure_ascii=False).replace("</", "<\\/")
    manifest_dir = path.parent
    overview_links = (
        ("이동 idle", "v8-h1-movement-overview.png"),
        ("이동 row 2", "v8-h1-walk-overview.gif"),
        ("전투 idle", "v8-h1-combat-overview.png"),
        ("전투 row 2", "v8-h1-combat-row-2-overview.gif"),
        ("마을 idle", "v8-h1-village-overview.png"),
        ("마을 row 2", "v8-h1-village-row-2-overview.gif"),
    )
    links_html = " · ".join(
        f'<a href="{filename}">{label}</a>'
        for label, filename in overview_links
        if (manifest_dir / filename).is_file()
    )
    path.write_text(f'''<!doctype html><meta charset="utf-8"><title>Seryeong V8 candidates</title>
<style>body{{font:14px system-ui,sans-serif;margin:24px;background:#17151b;color:#eee}}main{{max-width:1180px;margin:auto}}select{{padding:6px;margin:0 12px 12px 0}}#items{{display:grid;grid-template-columns:repeat(auto-fit,minmax(250px,1fr));gap:14px}}article{{background:#292530;border:1px solid #4e4758;border-radius:8px;padding:10px}}img{{width:100%;max-height:190px;object-fit:contain;background:#0d0c10}}small{{display:block;color:#c7c0cf;margin-top:7px}}a{{color:#93c5fd}}</style>
<main><h1>세령 V8 실제 후보</h1><p>실제 파일이 있는 후보만 표시합니다. 수용 조합은 0개이며, 양방향과 전체 동작 행렬은 보류 상태입니다.</p><p>{links_html}</p><label>의상 <select id="outfit"></select></label><label>헤어 <select id="hair"></select></label><label>동작 <select id="kind"></select></label><p id="summary"></p><section id="items"></section><script id="v8-metadata" type="application/json">{metadata}</script><script>
const d=JSON.parse(document.getElementById('v8-metadata').textContent),q=id=>document.getElementById(id),rel=p=>'../../../../'+p;
function fill(id,items,key){{q(id).innerHTML='<option value="">전체</option>'+items.map(x=>`<option value="${{x.id}}">${{x[key]}}</option>`).join('')}}
fill('outfit',d.outfits,'label');fill('hair',d.hairs,'label');
const kinds=[...new Map(d.sourceOnlyMedia.map(x=>[x.kind,x])).values()];q('kind').innerHTML='<option value="">전체</option>'+kinds.map(x=>`<option value="${{x.kind}}">${{x.kindLabel||x.kind}}</option>`).join('');
function render(){{const o=q('outfit').value,h=q('hair').value,k=q('kind').value,media=d.sourceOnlyMedia.filter(x=>(!o||x.costumeId===o)&&(!h||x.hairId===h)&&(!k||x.kind===k));q('summary').textContent=`실제 후보 ${{media.length}}개 · 수용 0개 · 양방향과 전체 동작 행렬 보류`;q('items').innerHTML=media.map(m=>{{const outfit=d.outfits.find(x=>x.id===m.costumeId),hair=d.hairs.find(x=>x.id===m.hairId),native=m.nativeV4||[],preview=native.find(p=>p.endsWith('row-1-native.gif'))||m.path,links=[m.path,...native].filter((p,i,a)=>a.indexOf(p)===i).map(p=>`<a href="${{rel(p)}}" target="_blank">${{p.split('/').pop()}}</a>`).join(' · ');return `<article><strong>${{outfit.label}} · ${{hair?hair.label:m.hairId.toUpperCase()}} · ${{m.kindLabel||m.kind}}</strong><img src="${{rel(preview)}}" alt="${{outfit.label}} ${{m.hairId}} ${{m.kind}} 후보"><small>source_only · ${{links}}</small></article>`}}).join('')||'<p>이 필터에 맞는 실제 후보 파일이 없습니다.</p>'}}
['outfit','hair','kind'].forEach(id=>q(id).addEventListener('change',render));render();</script></main>\n''', encoding="utf-8")

def write_v8_manifests(output_dir: Path | None = None) -> Path:
    output_dir = output_dir or REPOSITORY_ROOT / "images/sprites/seryeong-appearance-v3/manifest"
    output_dir.mkdir(parents=True, exist_ok=True)
    ledger = v8_inventory()
    if len(ledger["outfits"]) != 16 or len(ledger["hairs"]) != 12 or len(ledger["appearances"]) != 192:
        raise ValueError("V8 ledger must preserve the 16 outfit by 12 hair pending matrix")
    manifest = output_dir / "v8-inventory.json"
    manifest.write_text(json.dumps(ledger, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    write_v8_h1_overviews(output_dir)
    write_v8_h1_kind_overviews(output_dir, "combat")
    write_v8_h1_kind_overviews(output_dir, "village")
    write_v8_viewer(output_dir / "v8-viewer.html", ledger)
    return manifest


def write_v8_h1_overviews(output_dir: Path) -> tuple[Path, Path] | None:
    """Build review-only 4×4 H1 sheets from actual V4 64-body atlas cells."""
    outfits = ("v1", "v2", "v3", "v4", "t1", "t2", "t3", "t4", "l1", "l2", "l3", "l4", "b1", "b2", "b3", "b4")
    native = REPOSITORY_ROOT / "images/sprites/seryeong-appearance-v3/native"
    atlases: list[Image.Image] = []
    for outfit in outfits:
        atlas_path = native / outfit / "h1/source-reference-v4-matte/movement/native-atlas-64body-6x4.png"
        if not atlas_path.is_file():
            return None
        with Image.open(atlas_path) as atlas:
            if atlas.size != (768, 512):
                return None
            atlases.append(atlas.convert("RGBA"))
    scale, cell, heading = 2, 128, 24
    size = cell * scale
    overview = Image.new("RGBA", (4 * size, heading + 4 * (size + 18)), (28, 29, 34, 255))
    draw = ImageDraw.Draw(overview)
    for index, (outfit, atlas) in enumerate(zip(outfits, atlases)):
        column, row = index % 4, index // 4
        x, y = column * size, heading + row * (size + 18)
        overview.alpha_composite(atlas.crop((0, 0, cell, cell)).resize((size, size), Image.Resampling.NEAREST), (x, y))
        draw.text((x + 4, y + size), f"{outfit.upper()} · H1 idle", fill=(235, 235, 240, 255))
    draw.text((4, 4), "V8 H1 movement candidates — source-only; all other hair/action/facing work pending", fill=(235, 235, 240, 255))
    overview_path = output_dir / "v8-h1-movement-overview.png"
    overview.save(overview_path)
    frames: list[Image.Image] = []
    for frame_index in range(6):
        frame = Image.new("RGBA", (4 * size, 4 * size), (28, 29, 34, 255))
        for index, atlas in enumerate(atlases):
            column, row = index % 4, index // 4
            # The second 6-cell source row is the walk cycle.
            frame.alpha_composite(atlas.crop((frame_index * cell, cell, (frame_index + 1) * cell, 2 * cell)).resize((size, size), Image.Resampling.NEAREST), (column * size, row * size))
        frames.append(frame)
    walk_path = output_dir / "v8-h1-walk-overview.gif"
    frames[0].save(walk_path, save_all=True, append_images=frames[1:], duration=120, loop=0, disposal=2)
    return overview_path, walk_path


def write_v8_h1_kind_overviews(output_dir: Path, kind: str) -> tuple[Path, Path] | None:
    """Build a review-only H1 contact sheet and row-two animation for one V8 action family."""
    if kind not in {"combat", "village"}:
        raise ValueError(f"unsupported V8 overview kind: {kind}")
    outfits = ("v1", "v2", "v3", "v4", "t1", "t2", "t3", "t4", "l1", "l2", "l3", "l4", "b1", "b2", "b3", "b4")
    native = REPOSITORY_ROOT / "images/sprites/seryeong-appearance-v3/native"
    atlases: list[Image.Image] = []
    for outfit in outfits:
        atlas_path = native / outfit / "h1/source-reference-v4-matte" / kind / "native-atlas-64body-6x4.png"
        if not atlas_path.is_file():
            return None
        with Image.open(atlas_path) as atlas:
            if atlas.size != (768, 512):
                return None
            atlases.append(atlas.convert("RGBA"))
    scale, cell, heading = 2, 128, 24
    size = cell * scale
    overview = Image.new("RGBA", (4 * size, heading + 4 * (size + 18)), (28, 29, 34, 255))
    draw = ImageDraw.Draw(overview)
    label = {"combat": "combat", "village": "village"}[kind]
    for index, (outfit, atlas) in enumerate(zip(outfits, atlases)):
        column, row = index % 4, index // 4
        x, y = column * size, heading + row * (size + 18)
        overview.alpha_composite(atlas.crop((0, 0, cell, cell)).resize((size, size), Image.Resampling.NEAREST), (x, y))
        draw.text((x + 4, y + size), f"{outfit.upper()} · H1 idle", fill=(235, 235, 240, 255))
    draw.text((4, 4), f"V8 H1 {label} candidates — source-only; full coverage and acceptance pending", fill=(235, 235, 240, 255))
    overview_path = output_dir / f"v8-h1-{kind}-overview.png"
    overview.save(overview_path)
    frames: list[Image.Image] = []
    for frame_index in range(6):
        frame = Image.new("RGBA", (4 * size, 4 * size), (28, 29, 34, 255))
        for index, atlas in enumerate(atlases):
            column, row = index % 4, index // 4
            frame.alpha_composite(atlas.crop((frame_index * cell, cell, (frame_index + 1) * cell, 2 * cell)).resize((size, size), Image.Resampling.NEAREST), (column * size, row * size))
        frames.append(frame)
    row_two_path = output_dir / f"v8-h1-{kind}-row-2-overview.gif"
    frames[0].save(row_two_path, save_all=True, append_images=frames[1:], duration=120, loop=0, disposal=2)
    return overview_path, row_two_path


if __name__ == "__main__":
    write_manifests()
    write_v8_manifests()

