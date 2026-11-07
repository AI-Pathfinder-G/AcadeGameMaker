"""Engine-free tests for REQ-SM1 and AC-SM1 fail-closed staging."""
from __future__ import annotations

import json
import os
import sys
import tempfile
import unittest
from pathlib import Path
from unittest import mock

from PIL import Image

HERE = Path(__file__).resolve().parent
sys.path.insert(0, str(HERE))
import seryeong_first_media as media  # noqa: E402


class SeryeongFirstMediaTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temporary = tempfile.TemporaryDirectory(prefix="seryeong-first-media-test-")
        self.root = Path(self.temporary.name)
        self.stage = self.root / "stage"
        self.manifest = self.root / "manifest.json"

    def tearDown(self) -> None:
        self.temporary.cleanup()

    @staticmethod
    def stage_bytes(stage: Path) -> dict[str, bytes]:
        return {path.relative_to(stage).as_posix(): path.read_bytes() for path in stage.rglob("*") if path.is_file()}

    @staticmethod
    def write_json(path: Path, value: object) -> None:
        path.write_text(json.dumps(value, ensure_ascii=False, indent=2, sort_keys=True) + "\n", encoding="utf-8")

    @staticmethod
    def rebind_package_stage_hash(stage: Path, manifest: Path) -> None:
        package = json.loads(manifest.read_text(encoding="utf-8"))
        package["stageManifestSha256"] = media.sha256(stage / "pending/stage-manifest.json")
        SeryeongFirstMediaTests.write_json(manifest, package)

    def test_build_is_byte_identical_and_preserves_sources(self) -> None:
        # AC-SM1-006 / REQ-SM1-010: two independent runs are deterministic;
        # source pins are checked both before and after writing diagnostics.
        before = media.require_pins()
        media.build_for_test(self.stage, self.manifest)
        first = {path.relative_to(self.root).as_posix(): media.sha256(path) for path in self.root.rglob("*") if path.is_file()}
        with tempfile.TemporaryDirectory(prefix="seryeong-first-media-test-") as second_root_text:
            second_root = Path(second_root_text)
            second_stage = second_root / "stage"
            second_manifest = second_root / "manifest.json"
            media.build_for_test(second_stage, second_manifest)
            second = {path.relative_to(second_root).as_posix(): media.sha256(path) for path in second_root.rglob("*") if path.is_file()}
        self.assertEqual(first, second)
        self.assertEqual(before, media.require_pins())

    def test_pending_manifest_cannot_claim_complete_atlas(self) -> None:
        # AC-SM1-003/008: the absent left-facing source is represented as an
        # explicit blocker rather than duplicated, mirrored, or fabricated.
        media.build_for_test(self.stage, self.manifest)
        stage = json.loads((self.stage / "pending/stage-manifest.json").read_text(encoding="utf-8"))
        diagnostic = json.loads((self.stage / "pending/right-idle-diagnostic.json").read_text(encoding="utf-8"))
        self.assertEqual(media.PENDING, stage["disposition"])
        self.assertEqual(media.RIGHTS_DISPOSITION, stage["rightsDisposition"])
        self.assertEqual(media.RIGHTS_EVIDENCE, stage["rightsEvidence"])
        self.assertEqual(media.RESOLVED_INITIAL_DEFAULT_COSTUME_ID, stage["resolvedInitialDefaultCostumeId"])
        self.assertFalse(stage["runtimeCatalogDefaultGrant"])
        self.assertEqual("NoAcceptedDefault", stage["runtimeCatalogDisposition"])
        self.assertFalse(stage["payload"]["completeAtlas"])
        self.assertEqual(0, stage["payload"]["leftFacingFrames"])
        self.assertEqual(6, stage["payload"]["rightFacingFrames"])
        self.assertEqual("FailClosedMissingProductionInput", diagnostic["missingLeft"]["status"])
        self.assertFalse(any("atlas" in path.name.lower() for path in self.stage.rglob("*") if path.is_file()))
        with Image.open(self.stage / "pending/right-idle-frame0-preview-diagnostic.png") as preview:
            self.assertEqual("RGBA", preview.mode)
            self.assertEqual((128, 128), preview.size)
            self.assertIsNotNone(preview.getchannel("A").getbbox())

    def test_schema_and_payload_corruption_rejects(self) -> None:
        # AC-SM1-006: corruption cannot turn a pending diagnostic into a
        # package and is rejected before any non-pending disposition exists.
        media.build_for_test(self.stage, self.manifest)
        package = json.loads(self.manifest.read_text(encoding="utf-8"))
        package["completeAtlas"] = True
        self.manifest.write_text(json.dumps(package), encoding="utf-8")
        before = self.stage_bytes(self.stage)
        with self.assertRaises(media.ValidationError):
            media.validate_for_test(self.stage, self.manifest)
        with self.assertRaises(media.ValidationError):
            media.build_for_test(self.stage, self.manifest)
        self.assertEqual(before, self.stage_bytes(self.stage))

    def test_exact_package_identity_fields_reject_without_stage_mutation(self) -> None:
        # AC-SM1-005/006: package identity, revision, sets, path and pending
        # claims cannot be changed even if an attacker recalculates JSON bytes.
        cases = {
            "schemaVersion": 2,
            "mediaRevision": 2,
            "portraitSetId": "other.portrait",
            "stageRoot": "outside/stage",
            "rightsDisposition": "PendingUserAttestation",
            "resolvedInitialDefaultCostumeId": "other.costume",
            "acceptedAppearanceCombinationCount": 1,
            "runtimeCatalogDisposition": "Accepted",
            "runtimeCatalogDefaultGrant": True,
            "nonClaim": "accepted",
            "unexpected": "payload",
        }
        for key, value in cases.items():
            with self.subTest(key=key), tempfile.TemporaryDirectory(prefix="seryeong-first-media-test-") as text:
                root = Path(text)
                stage = root / "stage"
                manifest = root / "manifest.json"
                media.build_for_test(stage, manifest)
                package = json.loads(manifest.read_text(encoding="utf-8"))
                package[key] = value
                manifest.write_text(json.dumps(package), encoding="utf-8")
                before = self.stage_bytes(stage)
                with self.assertRaises(media.ValidationError):
                    media.validate_for_test(stage, manifest)
                with self.assertRaises(media.ValidationError):
                    media.build_for_test(stage, manifest)
                self.assertEqual(before, self.stage_bytes(stage))

    def test_manifest_marker_and_hardlink_reject_before_stage_cleanup(self) -> None:
        # AC-SM1-006/010: package ownership/link checks run before any known
        # output unlink, so an adversarial manifest cannot empty the stage.
        media.build_for_test(self.stage, self.manifest)
        package = json.loads(self.manifest.read_text(encoding="utf-8"))
        package["producerMarker"] = "poisoned"
        self.manifest.write_text(json.dumps(package), encoding="utf-8")
        before = self.stage_bytes(self.stage)
        with self.assertRaises(media.ValidationError):
            media.build_for_test(self.stage, self.manifest)
        self.assertEqual(before, self.stage_bytes(self.stage))

        with tempfile.TemporaryDirectory(prefix="seryeong-first-media-test-") as text:
            root = Path(text)
            stage = root / "stage"
            manifest = root / "manifest.json"
            peer = root / "manifest-peer.json"
            media.build_for_test(stage, manifest)
            os.link(manifest, peer)
            before = self.stage_bytes(stage)
            peer_before = peer.read_bytes()
            with self.assertRaises(media.ValidationError):
                media.build_for_test(stage, manifest)
            self.assertEqual(before, self.stage_bytes(stage))
            self.assertEqual(peer_before, peer.read_bytes())

    def test_generated_payload_corruption_rejects(self) -> None:
        media.build_for_test(self.stage, self.manifest)
        preview = self.stage / "pending/right-idle-frame0-preview-diagnostic.png"
        preview.write_bytes(preview.read_bytes() + b"corruption")
        with self.assertRaises(media.ValidationError):
            media.validate_for_test(self.stage, self.manifest)

    def test_inner_manifest_semantics_reject_even_after_hash_rebinding(self) -> None:
        # AC-SM1-005/006: changing an inner document and recomputing every
        # outer hash must not bypass its canonical semantic contract.
        stage_cases = {
            "schemaVersion": lambda value: value.__setitem__("schemaVersion", 2),
            "requirementIds": lambda value: value.__setitem__("requirementIds", []),
            "mediaRevision": lambda value: value.__setitem__("mediaRevision", 2),
            "portraitSetId": lambda value: value.__setitem__("portraitSetId", "other.portrait"),
            "gameplaySetId": lambda value: value.__setitem__("gameplaySetId", "other.gameplay"),
            "acceptedAppearanceCombinationCount": lambda value: value.__setitem__("acceptedAppearanceCombinationCount", 1),
            "runtimeCatalogDisposition": lambda value: value.__setitem__("runtimeCatalogDisposition", "Accepted"),
            "payload.clipMap": lambda value: value["payload"].__setitem__("clipMap", "fake.json"),
            "blockers": lambda value: value.__setitem__("blockers", []),
        }
        diagnostic_cases = {
            "schemaVersion": lambda value: value.__setitem__("schemaVersion", 2),
            "requirementIds": lambda value: value.__setitem__("requirementIds", []),
            "source": lambda value: value["source"].__setitem__("role", "other"),
            "exactSourceFrame": lambda value: value["exactSourceFrame"].__setitem__("column", 1),
            "rightIdle": lambda value: value["rightIdle"].__setitem__("frameCount", 5),
            "preview": lambda value: value["preview"].__setitem__("path", "other.png"),
            "missingLeft": lambda value: value["missingLeft"].__setitem__("availableFrames", 6),
            "notProduced": lambda value: value.__setitem__("notProduced", []),
            "operations": lambda value: value.__setitem__("operations", ["mirror"]),
            "forbiddenOperationsNotUsed": lambda value: value.__setitem__("forbiddenOperationsNotUsed", []),
            "generated": lambda value: value["generated"].__setitem__("pending/right-idle-row-diagnostic.png", "0" * 64),
        }
        for kind, cases in (("stage", stage_cases), ("diagnostic", diagnostic_cases)):
            for name, mutate in cases.items():
                with self.subTest(kind=kind, name=name), tempfile.TemporaryDirectory(prefix="seryeong-first-media-test-") as text:
                    root = Path(text)
                    stage = root / "stage"
                    manifest = root / "manifest.json"
                    media.build_for_test(stage, manifest)
                    target = stage / "pending" / ("stage-manifest.json" if kind == "stage" else "right-idle-diagnostic.json")
                    value = json.loads(target.read_text(encoding="utf-8"))
                    mutate(value)
                    self.write_json(target, value)
                    if kind == "diagnostic":
                        stage_value = json.loads((stage / "pending/stage-manifest.json").read_text(encoding="utf-8"))
                        stage_value["generated"]["pending/right-idle-diagnostic.json"] = media.sha256(target)
                        self.write_json(stage / "pending/stage-manifest.json", stage_value)
                    self.rebind_package_stage_hash(stage, manifest)
                    before = self.stage_bytes(stage)
                    with self.assertRaises(media.ValidationError):
                        media.validate_for_test(stage, manifest)
                    with self.assertRaises(media.ValidationError):
                        media.build_for_test(stage, manifest)
                    self.assertEqual(before, self.stage_bytes(stage))

    def test_json_type_smuggling_rejects_even_after_hash_rebinding(self) -> None:
        # AC-SM1-005/006: Python considers True == 1 and 0 == False, but the
        # JSON contract is type exact. Rebinding outer hashes cannot bypass it.
        cases = (
            ("package-mediaRevision-bool", "package", lambda value: value.__setitem__("mediaRevision", True)),
            ("package-accepted-float", "package", lambda value: value.__setitem__("acceptedAppearanceCombinationCount", 0.0)),
            ("stage-schema-bool", "stage", lambda value: value.__setitem__("schemaVersion", True)),
            ("stage-revision-float", "stage", lambda value: value.__setitem__("mediaRevision", 1.0)),
            ("stage-accepted-bool", "stage", lambda value: value.__setitem__("acceptedAppearanceCombinationCount", False)),
            ("diagnostic-schema-bool", "diagnostic", lambda value: value.__setitem__("schemaVersion", True)),
            ("diagnostic-frame-float", "diagnostic", lambda value: value["rightIdle"].__setitem__("frameCount", 6.0)),
        )
        for name, kind, mutate in cases:
            with self.subTest(name=name), tempfile.TemporaryDirectory(prefix="seryeong-first-media-test-") as text:
                root = Path(text)
                stage = root / "stage"
                manifest = root / "manifest.json"
                media.build_for_test(stage, manifest)
                target = manifest if kind == "package" else stage / "pending" / ("stage-manifest.json" if kind == "stage" else "right-idle-diagnostic.json")
                value = json.loads(target.read_text(encoding="utf-8"))
                mutate(value)
                self.write_json(target, value)
                if kind == "diagnostic":
                    stage_value = json.loads((stage / "pending/stage-manifest.json").read_text(encoding="utf-8"))
                    stage_value["generated"]["pending/right-idle-diagnostic.json"] = media.sha256(target)
                    self.write_json(stage / "pending/stage-manifest.json", stage_value)
                if kind != "package":
                    self.rebind_package_stage_hash(stage, manifest)
                before = self.stage_bytes(stage)
                with self.assertRaises(media.ValidationError):
                    media.validate_for_test(stage, manifest)
                with self.assertRaises(media.ValidationError):
                    media.build_for_test(stage, manifest)
                self.assertEqual(before, self.stage_bytes(stage))

    def test_path_escape_unknown_payload_and_reparse_reject_without_cleanup(self) -> None:
        # AC-SM1-006/010: the public production API is pathless; test targets
        # must be a unique temp leaf and unowned payloads remain untouched.
        with self.assertRaises(TypeError):
            media.build(self.stage, self.manifest)  # type: ignore[call-arg]
        with self.assertRaises(media.ValidationError):
            media.build_for_test(Path(tempfile.gettempdir()), Path(tempfile.gettempdir()) / "manifest.json")
        with self.assertRaises(media.ValidationError):
            media.build_for_test(self.stage, self.root / "other.json")

        media.build_for_test(self.stage, self.manifest)
        unknown = self.stage / "untrusted.txt"
        unknown.write_text("do not delete", encoding="utf-8")
        with self.assertRaises(media.ValidationError):
            media.build_for_test(self.stage, self.manifest)
        self.assertEqual("do not delete", unknown.read_text(encoding="utf-8"))
        self.assertNotIn("rmtree", Path(media.__file__).read_text(encoding="utf-8"))

        unknown.unlink()
        preview = self.stage / "pending/right-idle-frame0-preview-diagnostic.png"
        with mock.patch.object(media, "_is_reparse", side_effect=lambda path: Path(path).name == preview.name):
            with self.assertRaises(media.ValidationError):
                media.validate_for_test(self.stage, self.manifest)

    def test_quarantine_candidate_hash_drift_rejects_without_stage_mutation(self) -> None:
        # REQ-SM1-006/010, AC-SM1-006/008: a known rejected candidate may be
        # retained for audit, but it cannot become an unpinned stage bypass.
        media.build_for_test(self.stage, self.manifest)
        quarantine = self.stage / "quarantine"
        quarantine.mkdir()
        for name in media.QUARANTINE_CANDIDATE_HASHES:
            (quarantine / name).write_bytes(b"not-the-production-candidate")
        before = self.stage_bytes(self.stage)
        with self.assertRaises(media.ValidationError):
            media.validate_for_test(self.stage, self.manifest)
        with self.assertRaises(media.ValidationError):
            media.build_for_test(self.stage, self.manifest)
        self.assertEqual(before, self.stage_bytes(self.stage))


if __name__ == "__main__":
    unittest.main()
