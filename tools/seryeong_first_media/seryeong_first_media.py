"""Fail-closed offline staging for the Seryeong A1/H1 first-media slice.

REQ-SM1-001..010: this tool deliberately produces only diagnostics while the
separately authored left-facing idle row, manual 32px correction, and final
independent review remain absent.  Project-use authorization is pinned to its
rights-chain evidence; it is not itself an acceptance shortcut.
It does not create a 32px atlas, a clip map, an accepted package, or Unity
assets.  The one 64px candidate is a byte-stable crop of the hash-pinned,
already-source-only v4 diagnostic; it is not a new matte/downscale operation.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import os
import re
import stat
import tempfile
from dataclasses import dataclass
from pathlib import Path
from typing import Any

from PIL import Image


ROOT = Path(__file__).resolve().parents[2]
DEFAULT_STAGE = ROOT / "images/sprites/seryeong-appearance-v3/production/a1-h1-v1"
DEFAULT_MANIFEST = ROOT / "images/sprites/seryeong-appearance-v3/manifest/a1-h1-package-v1.json"
PENDING = "PendingMissingLeftManual32pxAndLunaReview"
RIGHTS_DISPOSITION = "ProjectUseAuthorized"
STAGE_MARKER_NAME = ".seryeong-first-media-owned-v1"
STAGE_MARKER_VALUE = "seryeong-first-media-owned-stage-v1\n"
PRODUCTION_STAGE_RELATIVE = "images/sprites/seryeong-appearance-v3/production/a1-h1-v1"
STAGE_REQUIREMENT_IDS = [f"REQ-SM1-{number:03d}" for number in range(1, 11)]
DIAGNOSTIC_REQUIREMENT_IDS = ["REQ-SM1-001", "REQ-SM1-003", "REQ-SM1-004", "REQ-SM1-007", "REQ-SM1-008", "REQ-SM1-010"]
PACKAGE_REQUIREMENT_IDS = ["REQ-SM1-001", "REQ-SM1-005", "REQ-SM1-006", "REQ-SM1-009", "REQ-SM1-010"]
STAGE_BLOCKERS = ["six manually corrected idle.left frames", "manual 32px A1/H1 correction and review", "independent Luna review and Astra disposition"]
NOT_PRODUCED = ["384x128 32px two-facing gameplay atlas", "canonical idle left/right clip map", "AcceptedForVerticalDemoMedia disposition", "Unity/importer/catalog/addressable/wearable publication"]
DIAGNOSTIC_OPERATIONS = ["exact 128px crop of existing source_only v4 diagnostic", "nearest-neighbor 4x diagnostic enlargement"]
FORBIDDEN_OPERATIONS = ["blind horizontal mirror", "32px automatic downscale", "new background removal", "frame fabrication", "Unity publication"]
RESOLVED_INITIAL_DEFAULT_COSTUME_ID = "seryeong.costume.a1.v1"
RIGHTS_EVIDENCE = {
    "path": "docs/evidence/2026-09-20-seryeong-a1-h1-rights-chain.md",
    "sha256": "d079325198338e9d171750336add34c69f7130fa9bd81747d82bcfd5ee31236b",
    "status": RIGHTS_DISPOSITION,
}
QUARANTINE_CANDIDATE_HASHES = {
    "left-idle-generated-v1.png": "2b0292ef464f8eb76d3b4a42ebb73e4b57db955bd4e41387d8adeaf1503bde40",
    "left-idle-generated-v2.png": "24565d1656344fa7b3de6cd9de3b2570287b98caa58e30fbc0314626a136e505",
}


@dataclass(frozen=True)
class SourcePin:
    role: str
    path: str
    sha256: str


# REQ-SM1-001/006/008/010: the complete input chain is fixed here, rather
# than accepted from a caller argument.  This prevents source substitution.
SOURCE_PINS = (
    SourcePin("rawMovement", "images/sprites/seryeong-appearance-v3/native/a1/h1/raw/movement-source-v1.png", "a2939b925999aaf9634513862456e75912cb40fb4d638b2fe6584d28d3448dc6"),
    SourcePin("rawProvenance", "images/sprites/seryeong-appearance-v3/native/a1/h1/raw/provenance.json", "a68fce849cd740aaaed246de61c48ad024a780a65134e2b75eb765b1ea824766"),
    SourcePin("a1Reference", "images/seryeong/shoulder-v2/A-shirt-v7.png", "951310f8cb41f126e192ea6de74ab8b297a5f95d18be158d8d98503775da5cc1"),
    SourcePin("h1Reference", "images/seryeong/shoulder-v2/hair-v1/01-loose-and-halfup.png", "da047fa24b06cfcb7eb6c3516a3e6b17145bf3cb954be416c8e2685f8b96b60d"),
    SourcePin("reviewedCalibration", "images/sprites/seryeong-appearance-v3/manifest/a1-hair-movement-reviewed-calibration.json", "414aaac2d134ff86c2ccfa645517ce606722a3b6f13ccfaac49cbb32b60735f9"),
    SourcePin("v4Diagnostic", "images/sprites/seryeong-appearance-v3/native/a1/h1/source-reference-v4-matte/movement/native-atlas-64body-6x4.png", "2564dd72375b2a4e9c4980cb9ac5d36cedd52db3df6509c6e338e43ed9a63b41"),
)
PIN_BY_ROLE = {pin.role: pin for pin in SOURCE_PINS}
OUTPUT_FILES = (
    "pending/right-idle-frame0-preview-diagnostic.png",
    "pending/right-idle-frame0-preview-diagnostic-4x.png",
    "pending/right-idle-row-diagnostic.png",
    "pending/right-idle-diagnostic.json",
)
PENDING_FILES = OUTPUT_FILES + ("pending/stage-manifest.json",)


class ValidationError(ValueError):
    """A condition which must keep the staging package non-publishable."""


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def canonical_json(value: Any) -> bytes:
    return (json.dumps(value, ensure_ascii=False, indent=2, sort_keys=True) + "\n").encode("utf-8")


def write_json(path: Path, value: Any) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(canonical_json(value))


def _absolute(path: Path) -> Path:
    """Normalize lexical `.`/`..` without resolving a possible reparse point."""
    return Path(os.path.abspath(os.fspath(path)))


def _same_path(left: Path, right: Path) -> bool:
    return os.path.normcase(os.fspath(_absolute(left))) == os.path.normcase(os.fspath(_absolute(right)))


def _is_reparse(path: Path) -> bool:
    try:
        details = os.lstat(path)
    except FileNotFoundError:
        return False
    return stat.S_ISLNK(details.st_mode) or bool(getattr(details, "st_file_attributes", 0) & 0x400)


def _assert_no_reparse_chain(path: Path) -> None:
    current = _absolute(path)
    while True:
        if _is_reparse(current):
            raise ValidationError(f"reparse/symlink path is forbidden: {current}")
        if current.parent == current:
            return
        current = current.parent


def _assert_not_root_or_source_ancestor(path: Path) -> None:
    candidate = _absolute(path)
    protected = (ROOT, *(ROOT / pin.path for pin in SOURCE_PINS))
    for item in protected:
        absolute = _absolute(item)
        if _same_path(candidate, absolute) or candidate in absolute.parents:
            raise ValidationError("output path cannot be ROOT, a pinned source, or their ancestor")


def _assert_regular_file(path: Path, require_single_link: bool = False) -> None:
    _assert_no_reparse_chain(path)
    try:
        details = os.lstat(path)
    except FileNotFoundError as error:
        raise ValidationError(f"missing file: {path}") from error
    if not stat.S_ISREG(details.st_mode):
        raise ValidationError(f"expected regular file: {path}")
    if require_single_link and details.st_nlink != 1:
        raise ValidationError(f"hard-linked output is forbidden: {path}")


def _assert_regular_directory(path: Path) -> None:
    _assert_no_reparse_chain(path)
    try:
        details = os.lstat(path)
    except FileNotFoundError as error:
        raise ValidationError(f"missing directory: {path}") from error
    if not stat.S_ISDIR(details.st_mode):
        raise ValidationError(f"expected regular directory: {path}")


def require_pins() -> dict[str, str]:
    hashes: dict[str, str] = {}
    for pin in SOURCE_PINS:
        source = ROOT / pin.path
        _assert_regular_file(source)
        actual = sha256(source)
        if actual != pin.sha256:
            raise ValidationError(f"hash mismatch for {pin.role}: {actual}")
        hashes[pin.role] = actual

    with Image.open(ROOT / PIN_BY_ROLE["rawMovement"].path) as raw:
        if raw.mode != "RGB" or raw.size != (1536, 1024):
            raise ValidationError("raw movement geometry is not the pinned 6x4 RGB sheet")
    provenance = json.loads((ROOT / PIN_BY_ROLE["rawProvenance"].path).read_text(encoding="utf-8"))
    if provenance.get("costumeId") != "seryeong.costume.a1.v1" or provenance.get("hairId") != "h1":
        raise ValidationError("raw provenance does not identify A1/H1")
    if provenance.get("facing") != "right" or provenance.get("accepted") is not False:
        raise ValidationError("raw provenance must remain right-facing and SourceOnly")
    with Image.open(ROOT / PIN_BY_ROLE["v4Diagnostic"].path) as diagnostic:
        if diagnostic.mode != "RGBA" or diagnostic.size != (768, 512):
            raise ValidationError("v4 diagnostic must remain a 6x4 128px RGBA source-only sheet")
    rights_path = ROOT / RIGHTS_EVIDENCE["path"]
    _assert_regular_file(rights_path)
    if sha256(rights_path) != RIGHTS_EVIDENCE["sha256"]:
        raise ValidationError("rights-chain evidence hash mismatch")
    return hashes


def _png(path: Path, image: Image.Image) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    image.save(path, format="PNG", optimize=False, compress_level=9)


def _alpha_bounds(image: Image.Image) -> list[int] | None:
    bounds = image.getchannel("A").getbbox()
    return list(bounds) if bounds else None


def _assert_production_targets(stage: Path, manifest_path: Path) -> tuple[Path, Path]:
    if not _same_path(stage, DEFAULT_STAGE) or not _same_path(manifest_path, DEFAULT_MANIFEST):
        raise ValidationError("production build/validation requires the exact Approved staging and manifest paths")
    stage = _absolute(stage)
    manifest_path = _absolute(manifest_path)
    _assert_not_root_or_source_ancestor(stage)
    _assert_not_root_or_source_ancestor(manifest_path)
    _assert_no_reparse_chain(stage)
    _assert_no_reparse_chain(manifest_path)
    return stage, manifest_path


def _assert_test_targets(stage: Path, manifest_path: Path) -> tuple[Path, Path]:
    """Permit only a new, named leaf below a system-temp unique test root."""
    stage = _absolute(stage)
    manifest_path = _absolute(manifest_path)
    temp_root = _absolute(Path(tempfile.gettempdir()))
    _assert_no_reparse_chain(temp_root)
    _assert_not_root_or_source_ancestor(stage)
    _assert_not_root_or_source_ancestor(manifest_path)
    if stage.name != "stage" or manifest_path.name != "manifest.json" or manifest_path.parent != stage.parent:
        raise ValidationError("test targets must be sibling stage/manifest.json leaves")
    if stage.parent.parent != temp_root or not stage.parent.name.startswith("seryeong-first-media-test-"):
        raise ValidationError("test stage must be below a unique system-temp test root")
    _assert_regular_directory(stage.parent)
    _assert_no_reparse_chain(stage)
    _assert_no_reparse_chain(manifest_path)
    return stage, manifest_path


def _assert_stage_inventory(stage: Path, allow_missing: bool) -> None:
    if not stage.exists():
        if allow_missing:
            return
        raise ValidationError("missing owned staging directory")
    _assert_regular_directory(stage)
    marker = stage / STAGE_MARKER_NAME
    _assert_regular_file(marker, require_single_link=True)
    if marker.read_text(encoding="utf-8") != STAGE_MARKER_VALUE:
        raise ValidationError("staging ownership marker mismatch")
    names = {entry.name for entry in stage.iterdir()}
    expected_stage_names = {STAGE_MARKER_NAME, "pending"}
    quarantine = stage / "quarantine"
    if quarantine.exists():
        expected_stage_names.add("quarantine")
    if names != expected_stage_names:
        raise ValidationError("unknown stage payload or directory")
    pending = stage / "pending"
    _assert_regular_directory(pending)
    expected = {Path(relative).name for relative in PENDING_FILES}
    for entry in pending.iterdir():
        if entry.name not in expected:
            raise ValidationError("unknown pending payload or directory")
        _assert_regular_file(entry, require_single_link=True)
    if quarantine.exists():
        _assert_regular_directory(quarantine)
        if {entry.name for entry in quarantine.iterdir()} != set(QUARANTINE_CANDIDATE_HASHES):
            raise ValidationError("unknown or missing quarantined candidate")
        for name, expected_hash in QUARANTINE_CANDIDATE_HASHES.items():
            candidate = quarantine / name
            _assert_regular_file(candidate, require_single_link=True)
            if sha256(candidate) != expected_hash:
                raise ValidationError(f"quarantined candidate hash mismatch: {name}")


def _assert_destination_parent(path: Path) -> None:
    _assert_not_root_or_source_ancestor(path)
    _assert_no_reparse_chain(path)
    _assert_regular_directory(path.parent)


def _assert_package_static(package: Any, source_hashes: dict[str, str]) -> None:
    if not isinstance(package, dict):
        raise ValidationError("manifest must be a JSON object")
    expected = {
        "schemaVersion": 1,
        "packageId": "seryeong.a1.h1.vertical-demo.v1",
        "producerMarker": STAGE_MARKER_NAME,
        "costumeId": "seryeong.costume.a1.v1",
        "hairId": "h1",
        "portraitSetId": "seryeong.costume.a1.v1.portrait.v1",
        "gameplaySetId": "seryeong.costume.a1.v1.atlas.v1",
        "mediaRevision": 1,
        "disposition": PENDING,
        "nonClaim": "This is a pending diagnostic staging manifest, not a complete media package or AcceptedForVerticalDemoMedia record.",
        "stageRoot": PRODUCTION_STAGE_RELATIVE,
        "rightsDisposition": RIGHTS_DISPOSITION,
        "rightsEvidence": RIGHTS_EVIDENCE,
        "resolvedInitialDefaultCostumeId": RESOLVED_INITIAL_DEFAULT_COSTUME_ID,
        "nonWearable": True,
        "nonAddressable": True,
        "acceptedAppearanceCombinationCount": 0,
        "runtimeCatalogDisposition": "NoAcceptedDefault",
        "runtimeCatalogDefaultGrant": False,
        "completeAtlas": False,
        "clipMap": None,
        "missingLeft": True,
    }
    for key, value in expected.items():
        if not _strict_json_equal(package.get(key), value):
            raise ValidationError(f"manifest canonical field drift: {key}")
    if set(package) != set(expected) | {"requirementIds", "sourceHashes", "stageManifestSha256"}:
        raise ValidationError("manifest contains an unknown or missing schema field")
    if not _strict_json_equal(package.get("requirementIds"), PACKAGE_REQUIREMENT_IDS):
        raise ValidationError("manifest requirement schema drift")
    if not _strict_json_equal(package.get("sourceHashes"), source_hashes):
        raise ValidationError("manifest source hash chain drift")
    stage_hash = package.get("stageManifestSha256")
    if type(stage_hash) is not str or not re.fullmatch(r"[0-9a-f]{64}", stage_hash):
        raise ValidationError("manifest stage-manifest hash schema drift")


def _prepare_stage(stage: Path) -> None:
    """Clear only explicit tool-owned files; never recursively delete a path."""
    if not stage.exists():
        stage.parent.mkdir(parents=True, exist_ok=True)
        _assert_no_reparse_chain(stage.parent)
        stage.mkdir()
        (stage / STAGE_MARKER_NAME).write_text(STAGE_MARKER_VALUE, encoding="utf-8", newline="\n")
        (stage / "pending").mkdir()
        return
    _assert_stage_inventory(stage, allow_missing=False)
    for relative in PENDING_FILES:
        generated = stage / relative
        if generated.exists():
            _assert_regular_file(generated)
            generated.unlink()


def _preflight_stage(stage: Path) -> None:
    """Perform every stage ownership/path check before any unlink or write."""
    _assert_destination_parent(stage)
    _assert_stage_inventory(stage, allow_missing=True)


def _preflight_manifest(manifest_path: Path, source_hashes: dict[str, str]) -> None:
    """Reject unowned, linked, or semantically drifted manifests before stage cleanup."""
    _assert_destination_parent(manifest_path)
    _assert_regular_file(manifest_path, require_single_link=True)
    try:
        existing = json.loads(manifest_path.read_text(encoding="utf-8"))
    except (OSError, ValueError, json.JSONDecodeError) as error:
        raise ValidationError("existing manifest is not a tool-owned JSON payload") from error
    _assert_package_static(existing, source_hashes)


def _read_json_object(path: Path, label: str) -> dict[str, Any]:
    _assert_regular_file(path, require_single_link=True)
    try:
        value = json.loads(path.read_text(encoding="utf-8"))
    except (OSError, ValueError, json.JSONDecodeError) as error:
        raise ValidationError(f"invalid {label} JSON") from error
    if not isinstance(value, dict):
        raise ValidationError(f"{label} must be a JSON object")
    return value


def _assert_exact_object(actual: dict[str, Any], expected: dict[str, Any], label: str) -> None:
    if not _strict_json_equal(actual, expected):
        raise ValidationError(f"{label} canonical semantic drift")


def _strict_json_equal(actual: Any, expected: Any) -> bool:
    """JSON-semantic equality that rejects Python's bool/int/float aliases."""
    if type(actual) is not type(expected):
        return False
    if isinstance(expected, dict):
        return set(actual) == set(expected) and all(_strict_json_equal(actual[key], expected[key]) for key in expected)
    if isinstance(expected, list):
        return len(actual) == len(expected) and all(_strict_json_equal(left, right) for left, right in zip(actual, expected))
    return actual == expected


def _assert_same_rgba(actual_path: Path, expected: Image.Image, label: str) -> None:
    _assert_regular_file(actual_path, require_single_link=True)
    with Image.open(actual_path) as actual:
        if actual.mode != "RGBA" or actual.size != expected.size or actual.tobytes() != expected.tobytes():
            raise ValidationError(f"{label} pixels drift from deterministic source diagnostic")


def _expected_diagnostic_images() -> tuple[Image.Image, Image.Image, Image.Image]:
    with Image.open(ROOT / PIN_BY_ROLE["v4Diagnostic"].path) as source:
        preview = source.crop((0, 0, 128, 128)).copy()
        right_row = source.crop((0, 0, 768, 128)).copy()
    return preview, preview.resize((512, 512), Image.Resampling.NEAREST), right_row


def _assert_inner_semantics(stage: Path, manifest_path: Path, source_hashes: dict[str, str]) -> None:
    """Bind every pending document and diagnostic image to its fixed meaning."""
    pending = stage / "pending"
    stage_manifest_path = pending / "stage-manifest.json"
    diagnostic_path = pending / "right-idle-diagnostic.json"
    stage_manifest = _read_json_object(stage_manifest_path, "stage manifest")
    diagnostic = _read_json_object(diagnostic_path, "right-idle diagnostic")
    package = _read_json_object(manifest_path, "package manifest")

    preview, preview_4x, right_row = _expected_diagnostic_images()
    preview_path = pending / "right-idle-frame0-preview-diagnostic.png"
    preview_4x_path = pending / "right-idle-frame0-preview-diagnostic-4x.png"
    row_path = pending / "right-idle-row-diagnostic.png"
    _assert_same_rgba(preview_path, preview, "preview diagnostic")
    _assert_same_rgba(preview_4x_path, preview_4x, "4x preview diagnostic")
    _assert_same_rgba(row_path, right_row, "right-idle row diagnostic")
    frame_bounds = [_alpha_bounds(right_row.crop((column * 128, 0, (column + 1) * 128, 128))) for column in range(6)]
    generated_images = {relative: sha256(stage / relative) for relative in OUTPUT_FILES[:3]}
    expected_diagnostic = {
        "schemaVersion": 1,
        "requirementIds": DIAGNOSTIC_REQUIREMENT_IDS,
        "disposition": PENDING,
        "diagnosticOnly": True,
        "source": {"role": "v4Diagnostic", "path": PIN_BY_ROLE["v4Diagnostic"].path, "sha256": source_hashes["v4Diagnostic"]},
        "exactSourceFrame": {"row": 0, "column": 0, "sourceFrameIndex": 0},
        "rightIdle": {"facing": "right", "frameCount": 6, "sourceRow": 0, "frameAlphaBounds": frame_bounds},
        "preview": {"path": "pending/right-idle-frame0-preview-diagnostic.png", "geometry": [128, 128], "sourceTopLeftBaseline": 104, "sourceTopLeftPivot": [64, 104], "normalizedPivot": [0.5, 0.1875], "bodyTarget": 64, "actualAlphaBounds": _alpha_bounds(preview), "reviewState": "manual 64px landmark/body measurement still required"},
        "missingLeft": {"facing": "left", "requiredFrames": 6, "availableFrames": 0, "status": "FailClosedMissingProductionInput", "reason": "Only right-facing raw source is pinned. Horizontal mirror and fabricated/duplicated frames are forbidden."},
        "notProduced": NOT_PRODUCED,
        "operations": DIAGNOSTIC_OPERATIONS,
        "forbiddenOperationsNotUsed": FORBIDDEN_OPERATIONS,
        "generated": generated_images,
    }
    _assert_exact_object(diagnostic, expected_diagnostic, "right-idle diagnostic")

    generated_stage = dict(generated_images)
    generated_stage["pending/right-idle-diagnostic.json"] = sha256(diagnostic_path)
    expected_stage = {
        "schemaVersion": 1,
        "requirementIds": STAGE_REQUIREMENT_IDS,
        "costumeId": "seryeong.costume.a1.v1",
        "hairId": "h1",
        "portraitSetId": "seryeong.costume.a1.v1.portrait.v1",
        "gameplaySetId": "seryeong.costume.a1.v1.atlas.v1",
        "mediaRevision": 1,
        "disposition": PENDING,
        "nonWearable": True,
        "nonAddressable": True,
        "acceptedAppearanceCombinationCount": 0,
        "runtimeCatalogDisposition": "NoAcceptedDefault",
        "runtimeCatalogDefaultGrant": False,
        "rightsDisposition": RIGHTS_DISPOSITION,
        "rightsEvidence": RIGHTS_EVIDENCE,
        "resolvedInitialDefaultCostumeId": RESOLVED_INITIAL_DEFAULT_COSTUME_ID,
        "sourceHashes": source_hashes,
        "payload": {"previewDiagnostic": "pending/right-idle-frame0-preview-diagnostic.png", "completeAtlas": False, "clipMap": None, "leftFacingFrames": 0, "rightFacingFrames": 6},
        "blockers": STAGE_BLOCKERS,
        "generated": generated_stage,
    }
    _assert_exact_object(stage_manifest, expected_stage, "stage manifest")
    _assert_package_static(package, source_hashes)
    if package["stageManifestSha256"] != sha256(stage_manifest_path):
        raise ValidationError("package does not bind the exact stage manifest")


def _preflight_pair(stage: Path, manifest_path: Path, source_hashes: dict[str, str]) -> None:
    """Require a fresh pair or two owned members before any state transition."""
    _preflight_stage(stage)
    stage_exists = stage.exists()
    manifest_exists = manifest_path.exists()
    if stage_exists != manifest_exists:
        raise ValidationError("stage and manifest must be created together or both be owned existing outputs")
    if manifest_exists:
        _preflight_manifest(manifest_path, source_hashes)
        _assert_inner_semantics(stage, manifest_path, source_hashes)


def _build(stage: Path, manifest_path: Path) -> dict[str, Any]:
    """Create pending diagnostics only.  No mirror, alpha processing, or scale-down occurs."""
    source_hashes = require_pins()
    # REQ-SM1-006/010: all rejectable ownership, path, link, and schema checks
    # occur before `_prepare_stage` can unlink even one known output file.
    _preflight_pair(stage, manifest_path, source_hashes)
    _prepare_stage(stage)
    source_path = ROOT / PIN_BY_ROLE["v4Diagnostic"].path
    with Image.open(source_path) as source:
        # Exact source row 1/column 1 (zero-based index 0) per REQ-SM1-001.
        preview = source.crop((0, 0, 128, 128)).copy()
        right_row = source.crop((0, 0, 768, 128)).copy()
    if preview.getchannel("A").getbbox() is None:
        raise ValidationError("pinned diagnostic preview has no transparent foreground")

    pending = stage / "pending"
    preview_path = pending / "right-idle-frame0-preview-diagnostic.png"
    preview_4x_path = pending / "right-idle-frame0-preview-diagnostic-4x.png"
    row_path = pending / "right-idle-row-diagnostic.png"
    _png(preview_path, preview)
    _png(preview_4x_path, preview.resize((512, 512), Image.Resampling.NEAREST))
    _png(row_path, right_row)

    generated = {relative: sha256(stage / relative) for relative in OUTPUT_FILES[:3]}
    frame_bounds = []
    for column in range(6):
        frame_bounds.append(_alpha_bounds(right_row.crop((column * 128, 0, (column + 1) * 128, 128))))
    diagnostic = {
        "schemaVersion": 1,
        "requirementIds": DIAGNOSTIC_REQUIREMENT_IDS,
        "disposition": PENDING,
        "diagnosticOnly": True,
        "source": {"role": "v4Diagnostic", "path": PIN_BY_ROLE["v4Diagnostic"].path, "sha256": source_hashes["v4Diagnostic"]},
        "exactSourceFrame": {"row": 0, "column": 0, "sourceFrameIndex": 0},
        "rightIdle": {"facing": "right", "frameCount": 6, "sourceRow": 0, "frameAlphaBounds": frame_bounds},
        "preview": {"path": "pending/right-idle-frame0-preview-diagnostic.png", "geometry": [128, 128], "sourceTopLeftBaseline": 104, "sourceTopLeftPivot": [64, 104], "normalizedPivot": [0.5, 0.1875], "bodyTarget": 64, "actualAlphaBounds": _alpha_bounds(preview), "reviewState": "manual 64px landmark/body measurement still required"},
        "missingLeft": {"facing": "left", "requiredFrames": 6, "availableFrames": 0, "status": "FailClosedMissingProductionInput", "reason": "Only right-facing raw source is pinned. Horizontal mirror and fabricated/duplicated frames are forbidden."},
        "notProduced": NOT_PRODUCED,
        "operations": DIAGNOSTIC_OPERATIONS,
        "forbiddenOperationsNotUsed": FORBIDDEN_OPERATIONS,
        "generated": generated,
    }
    write_json(pending / "right-idle-diagnostic.json", diagnostic)
    generated["pending/right-idle-diagnostic.json"] = sha256(pending / "right-idle-diagnostic.json")
    stage_manifest = {
        "schemaVersion": 1,
        "requirementIds": STAGE_REQUIREMENT_IDS,
        "costumeId": "seryeong.costume.a1.v1",
        "hairId": "h1",
        "portraitSetId": "seryeong.costume.a1.v1.portrait.v1",
        "gameplaySetId": "seryeong.costume.a1.v1.atlas.v1",
        "mediaRevision": 1,
        "disposition": PENDING,
        "nonWearable": True,
        "nonAddressable": True,
        "acceptedAppearanceCombinationCount": 0,
        "runtimeCatalogDisposition": "NoAcceptedDefault",
        "runtimeCatalogDefaultGrant": False,
        "rightsDisposition": RIGHTS_DISPOSITION,
        "rightsEvidence": RIGHTS_EVIDENCE,
        "resolvedInitialDefaultCostumeId": RESOLVED_INITIAL_DEFAULT_COSTUME_ID,
        "sourceHashes": source_hashes,
        "payload": {"previewDiagnostic": "pending/right-idle-frame0-preview-diagnostic.png", "completeAtlas": False, "clipMap": None, "leftFacingFrames": 0, "rightFacingFrames": 6},
        "blockers": STAGE_BLOCKERS,
        "generated": generated,
    }
    write_json(pending / "stage-manifest.json", stage_manifest)
    package_manifest = {
        "schemaVersion": 1,
        "requirementIds": PACKAGE_REQUIREMENT_IDS,
        "packageId": "seryeong.a1.h1.vertical-demo.v1",
        "producerMarker": STAGE_MARKER_NAME,
        "costumeId": "seryeong.costume.a1.v1",
        "hairId": "h1",
        "portraitSetId": "seryeong.costume.a1.v1.portrait.v1",
        "gameplaySetId": "seryeong.costume.a1.v1.atlas.v1",
        "mediaRevision": 1,
        "disposition": PENDING,
        "nonClaim": "This is a pending diagnostic staging manifest, not a complete media package or AcceptedForVerticalDemoMedia record.",
        "stageRoot": "images/sprites/seryeong-appearance-v3/production/a1-h1-v1",
        "stageManifestSha256": sha256(pending / "stage-manifest.json"),
        "sourceHashes": source_hashes,
        "rightsDisposition": RIGHTS_DISPOSITION,
        "rightsEvidence": RIGHTS_EVIDENCE,
        "resolvedInitialDefaultCostumeId": RESOLVED_INITIAL_DEFAULT_COSTUME_ID,
        "nonWearable": True,
        "nonAddressable": True,
        "acceptedAppearanceCombinationCount": 0,
        "runtimeCatalogDisposition": "NoAcceptedDefault",
        "runtimeCatalogDefaultGrant": False,
        "completeAtlas": False,
        "clipMap": None,
        "missingLeft": True,
    }
    write_json(manifest_path, package_manifest)
    _validate(stage, manifest_path)
    return {"stage": stage_manifest, "package": package_manifest}


def build() -> dict[str, Any]:
    stage, manifest_path = _assert_production_targets(DEFAULT_STAGE, DEFAULT_MANIFEST)
    return _build(stage, manifest_path)


def build_for_test(stage: Path, manifest_path: Path) -> dict[str, Any]:
    """Test-only API: only accepts a fresh `seryeong-first-media-test-*` temp leaf."""
    stage, manifest_path = _assert_test_targets(stage, manifest_path)
    return _build(stage, manifest_path)


def _validate(stage: Path, manifest_path: Path) -> None:
    """Validate the expected incomplete state; this never upgrades it to acceptance.

    AC-SM1-003/006/008: an absent left row is a successful fail-closed outcome,
    while any fake full atlas, hash drift, or status escalation is rejected.
    """
    expected_sources = require_pins()
    _preflight_pair(stage, manifest_path, expected_sources)


def validate() -> None:
    stage, manifest_path = _assert_production_targets(DEFAULT_STAGE, DEFAULT_MANIFEST)
    _validate(stage, manifest_path)


def validate_for_test(stage: Path, manifest_path: Path) -> None:
    stage, manifest_path = _assert_test_targets(stage, manifest_path)
    _validate(stage, manifest_path)


def main() -> None:
    parser = argparse.ArgumentParser(description="Build or validate fail-closed A1/H1 media diagnostics.")
    parser.add_argument("command", choices=("build", "validate"))
    args = parser.parse_args()
    if args.command == "build":
        build()
        print("PENDING_FAIL_CLOSED: right-facing diagnostics staged; missing idle.left, manual 32px review, and independent review remain blockers.")
    else:
        validate()
        print("PENDING_FAIL_CLOSED: staging is valid but intentionally non-acceptable.")


if __name__ == "__main__":
    main()
