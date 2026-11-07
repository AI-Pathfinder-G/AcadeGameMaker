import copy
import hashlib
import json
import sys
import unittest
from pathlib import Path
from PIL import Image

sys.path.insert(0, str(Path(__file__).resolve().parent))
import inventory


class AppearanceInventoryTests(unittest.TestCase):
    def setUp(self):
        self.inventory = inventory.appearance_inventory()
        self.coverage = inventory.coverage_inventory(self.inventory["appearances"])

    def test_contract_counts_and_first_slice(self):
        inventory.validate(self.inventory, self.coverage)
        self.assertEqual(36, len(self.inventory["costumes"]))
        self.assertEqual(12, len(self.inventory["hairs"]))
        self.assertEqual(432, len(self.inventory["appearances"]))
        self.assertEqual(48, sum(item["costumeId"] in inventory.FIRST_SLICE_COSTUMES for item in self.inventory["appearances"]))

    def test_actions_are_unique_and_absence_is_not_a_sprite(self):
        self.assertEqual(25, len(inventory.ACTIONS))
        self.assertEqual(25, len({action[0] for action in inventory.ACTIONS}))
        absence = [row for row in self.coverage["coverage"] if row["actionId"] == "non_lovers_seryeong_absence"]
        self.assertEqual(432, len(absence))
        self.assertTrue(all(row["absenceIsNotSprite"] and row["output"] is None for row in absence))

    def test_non_lovers_absence_is_external_only_and_prelove_stays_present(self):
        gates = {action[0]: action[3] for action in inventory.ACTIONS}
        self.assertEqual("seoha_shop_external", gates["non_lovers_seryeong_absence"]["scene"])
        self.assertFalse(gates["non_lovers_seryeong_absence"]["seryeongPresent"])
        self.assertTrue(gates["non_lovers_seryeong_absence"]["doeonSoloVisit"])
        for action_id in ("prelove_jealous", "prelove_confused"):
            self.assertTrue(gates[action_id]["preloveAccompaniedShop"])
            self.assertTrue(gates[action_id]["seryeongPresent"])

    def test_candidate_media_links_resolve_and_never_count_as_accepted(self):
        candidates = inventory.candidate_media_inventory()
        self.assertEqual(0, candidates["acceptedCombinationCount"])
        self.assertEqual("rejected", next(item for item in candidates["candidates"] if item["id"] == "candidate.a1.h1.shop")["status"])
        for item in candidates["candidates"]:
            for media in item["media"]:
                self.assertTrue((inventory.REPOSITORY_ROOT / media["path"]).is_file(), media["path"])
        hair_candidates = [item for item in candidates["candidates"] if item["id"].startswith("candidate.a1.h") and item["id"].endswith(".movement")]
        self.assertEqual({f"h{i}" for i in range(1, 13)}, {item["hairId"] for item in hair_candidates})
        self.assertTrue(all(any(media["path"].endswith("movement-source-v1.png") for media in item["media"]) for item in hair_candidates))
        self.assertEqual("rejected", next(item for item in candidates["candidates"] if item["id"] == "candidate.a1.h1.group_diagnostic.v1_history")["status"])
        combat = next(item for item in candidates["candidates"] if item["id"] == "candidate.a1.h1.combat")
        self.assertEqual("source_only_candidate", combat["status"])
        self.assertTrue(any(media["path"].endswith("a1/h1/raw/combat-source-v1.png") for media in combat["media"]))

    def test_corrected_source_revisions_route_without_promoting_history(self):
        candidates = {item["id"]: item for item in inventory.candidate_media_inventory()["candidates"]}
        a2h12 = candidates["candidate.a2.h12.movement"]
        self.assertEqual("movement-source-v2.png", a2h12["preferredSourceRevision"])
        self.assertEqual("rejected_history", a2h12["history"][0]["status"])
        for hair in ("h8", "h9"):
            candidate = candidates[f"candidate.d3.{hair}.movement"]
            self.assertEqual("movement-source-v2.png", candidate["preferredSourceRevision"])
            self.assertEqual("movement-v2", candidate["preferredV4Kind"])
            self.assertTrue(any(media["path"].endswith("source-reference-v4-matte/movement-v2/native-atlas-64body-6x4.png") for media in candidate["media"]))
            self.assertEqual("source_only", candidate["acceptanceState"])

    def test_relationship_group_diagnostics_preserve_source_only_gates(self):
        root = inventory.REPOSITORY_ROOT / "images/sprites/seryeong-appearance-v3/native/a1/h1/relationship-diagnostic-v2"
        for name in ("relationship", "shop", "reactions"):
            record = json.loads((root / name / "diagnostic.json").read_text(encoding="utf-8"))
            source = inventory.REPOSITORY_ROOT / record["source"]
            self.assertEqual(hashlib.sha256(source.read_bytes()).hexdigest(), record["sourceSha256"])
            self.assertEqual("source_only", record["acceptanceState"])
            self.assertEqual(0, record["acceptedCombinationCount"])
            expected_bounds = {
                "relationship": [[0, 378], [378, 735], [735, 1024]],
                "shop": [[0, 346], [346, 677], [677, 1024]],
                "reactions": [[0, 342], [342, 682], [682, 1024]],
            }
            self.assertEqual(expected_bounds[name], [row["sourceRowBounds"] for row in record["rows"]])
            self.assertTrue(all(row["worldAndPartnerAnchors"] == "preserve_group_within_frame" for row in record["rows"]))
            self.assertEqual("RejectedScale", record["supersedes"]["status"])
            for row in record["rows"]:
                self.assertEqual(64 / row["sourceCrownSoleApproximation"]["height"], row["fixedScaleForRow"])
                with Image.open(root / name / row["outputs"]["nativeAtlas"]) as atlas:
                    self.assertEqual((1536, 128), atlas.size)
            for gif in root.joinpath(name).glob("*.gif"):
                with Image.open(gif) as motion:
                    self.assertLessEqual(motion.width, 1024)
        shop = json.loads((root / "shop/diagnostic.json").read_text(encoding="utf-8"))
        self.assertEqual("shop_row2_changed_pose_not_arm_link", shop["rows"][1]["actionId"])
        self.assertEqual("rejected", shop["rows"][1]["status"])
        self.assertTrue(all(row["groupType"] == "triplet" for row in shop["rows"]))
        reactions = json.loads((root / "reactions/diagnostic.json").read_text(encoding="utf-8"))
        self.assertEqual(["solo", "solo", "triplet"], [row["groupType"] for row in reactions["rows"]])

    def test_source_hashes_are_valid(self):
        inventory.validate(self.inventory, self.coverage)

    def test_duplicate_hair_fails_closed(self):
        malformed = copy.deepcopy(self.inventory)
        malformed["hairs"][1]["id"] = malformed["hairs"][0]["id"]
        with self.assertRaisesRegex(ValueError, "duplicate costume or hair"):
            inventory.validate(malformed, self.coverage)

    def test_missing_action_fails_closed(self):
        malformed = copy.deepcopy(self.coverage)
        malformed["coverage"] = [row for row in malformed["coverage"] if row["actionId"] != "bow_release"]
        with self.assertRaisesRegex(ValueError, "coverage action matrix mismatch"):
            inventory.validate(self.inventory, malformed)

    def test_written_manifests_match_generated_counts(self):
        root = Path(__file__).resolve().parents[2]
        actual_inventory = json.loads((root / "images/sprites/seryeong-appearance-v3/manifest/inventory.json").read_text(encoding="utf-8"))
        actual_coverage = json.loads((root / "images/sprites/seryeong-appearance-v3/manifest/coverage.json").read_text(encoding="utf-8"))
        actual_candidates = json.loads((root / "images/sprites/seryeong-appearance-v3/manifest/candidate-media.json").read_text(encoding="utf-8"))
        inventory.validate(actual_inventory, actual_coverage)
        self.assertEqual(0, actual_candidates["acceptedCombinationCount"])
        for item in actual_candidates["candidates"]:
            for media in item["media"]:
                self.assertTrue((root / media["path"]).is_file(), media["path"])

    def test_v8_source_only_ledger_preserves_pending_16_by_12_matrix(self):
        root = Path(__file__).resolve().parents[2]
        ledger = json.loads((root / "images/sprites/seryeong-appearance-v3/manifest/v8-inventory.json").read_text(encoding="utf-8"))
        self.assertEqual((16, 12, 192), (len(ledger["outfits"]), len(ledger["hairs"]), len(ledger["appearances"])))
        self.assertEqual(0, ledger["acceptedCombinationCount"])
        for costume_id in ("v1", "v2", "v3", "v4"):
            media = next(item for item in ledger["sourceOnlyMedia"] if item["costumeId"] == costume_id and item["hairId"] == "h1")
            source = root / media["path"]
            self.assertTrue(source.is_file())
            self.assertEqual(hashlib.sha256(source.read_bytes()).hexdigest(), media["sha256"])
            with Image.open(source) as sheet:
                self.assertEqual((1536, 1024), sheet.size)
        self.assertTrue((root / "images/sprites/seryeong-appearance-v3/manifest/v8-viewer.html").is_file())

    def test_v8_preferred_revisions_keep_historical_sources_out_of_acceptance(self):
        root = Path(__file__).resolve().parents[2]
        ledger = json.loads((root / "images/sprites/seryeong-appearance-v3/manifest/v8-inventory.json").read_text(encoding="utf-8"))
        sources = {entry["costumeId"]: entry for entry in ledger["sourceOnlyMedia"] if entry["hairId"] == "h1" and entry.get("kind") == "movement"}
        self.assertEqual("movement-source-v3.png", sources["v1"]["preferredSourceRevision"])
        self.assertEqual("movement-source-v3.png", sources["v3"]["preferredSourceRevision"])
        self.assertEqual("movement-source-v2.png", sources["b2"]["preferredSourceRevision"])
        for costume_id in ("t4", "l1", "l2", "l4"):
            self.assertEqual("movement-source-v2.png", sources[costume_id]["preferredSourceRevision"])
        combat = {entry["costumeId"]: entry for entry in ledger["sourceOnlyMedia"] if entry["hairId"] == "h1" and entry.get("kind") == "combat"}
        self.assertEqual("combat-source-v2.png", combat["t1"]["preferredSourceRevision"])
        self.assertEqual("combat-source-v2.png", combat["t2"]["preferredSourceRevision"])
        self.assertGreaterEqual(len(sources["v1"]["history"]), 2)
        self.assertGreaterEqual(len(sources["v3"]["history"]), 2)
        self.assertEqual(0, ledger["acceptedCombinationCount"])

    def test_v8_h1_overviews_use_all_sixteen_actual_v4_atlases(self):
        root = Path(__file__).resolve().parents[2]
        manifest = root / "images/sprites/seryeong-appearance-v3/manifest"
        with Image.open(manifest / "v8-h1-movement-overview.png") as overview:
            self.assertEqual((1024, 1120), overview.size)
        with Image.open(manifest / "v8-h1-walk-overview.gif") as motion:
            self.assertEqual((1024, 1024), motion.size)
            self.assertEqual(6, motion.n_frames)

    def test_v8_viewer_uses_actual_kind_and_hair_metadata_without_quarantine_links(self):
        root = Path(__file__).resolve().parents[2]
        manifest = root / "images/sprites/seryeong-appearance-v3/manifest"
        viewer = (manifest / "v8-viewer.html").read_text(encoding="utf-8")
        ledger = json.loads((manifest / "v8-inventory.json").read_text(encoding="utf-8"))
        self.assertIn('id="kind"', viewer)
        self.assertIn('hair?hair.label:m.hairId.toUpperCase()', viewer)
        self.assertIn('m.kindLabel||m.kind', viewer)
        self.assertNotIn("quarantine", viewer.lower())
        self.assertNotIn("rejected-alpha-bridge", viewer.lower())
        self.assertIn("rel=p=>'../../../../'+p;", viewer)
        self.assertNotIn("replace(/^images", viewer)
        actual = [item for item in ledger["sourceOnlyMedia"] if item["kind"] in {"combat", "village"} and item["hairId"] == "h1"]
        self.assertEqual(32, len(actual))
        self.assertTrue(all((root / item["path"]).is_file() for item in actual))
        for item in ledger["sourceOnlyMedia"]:
            for media_path in [item["path"], *item.get("nativeV4", [])]:
                self.assertTrue((manifest / "../../../../" / media_path).resolve().is_file(), media_path)
        relationship = next(item for item in ledger["sourceOnlyMedia"] if item["costumeId"] == "v1" and item["hairId"] == "h1" and item["kind"] == "relationship")
        self.assertTrue(any("relationship-diagnostic-v8" in path for path in relationship["nativeV4"]))

    def test_viewer_relative_media_paths_resolve_under_images(self):
        root = Path(__file__).resolve().parents[2]
        manifest = root / "images/sprites/seryeong-appearance-v3/manifest"
        for viewer_name in ("viewer.html", "v8-viewer.html"):
            viewer = (manifest / viewer_name).read_text(encoding="utf-8")
            self.assertIn("rel=p=>'../../../../'+p;", viewer)
            self.assertNotIn("p.replace(/^images", viewer)
        ledger = json.loads((manifest / "v8-inventory.json").read_text(encoding="utf-8"))
        for record in ledger["sourceOnlyMedia"]:
            for media_path in [record["path"], *record.get("nativeV4", [])]:
                resolved = (manifest / "../../../../" / media_path).resolve()
                self.assertTrue(resolved.is_file(), media_path)
                self.assertTrue(resolved.is_relative_to(root / "images"), resolved)

    def test_v8_reviewed_source_routing_and_relationship_discovery_are_exact(self):
        root = Path(__file__).resolve().parents[2]
        manifest = root / "images/sprites/seryeong-appearance-v3/manifest"
        ledger = json.loads((manifest / "v8-inventory.json").read_text(encoding="utf-8"))
        by_key = {(item["costumeId"], item["hairId"], item["kind"]): item for item in ledger["sourceOnlyMedia"]}
        expected = {
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
        self.assertEqual(expected, {key: by_key[key]["preferredSourceRevision"] for key in expected})
        diagnostic_atlases = list((root / "images/sprites/seryeong-appearance-v3/native").glob("*/h*/relationship-diagnostic-v8/native-atlas-pair-192x128-4x4.png"))
        self.assertTrue(diagnostic_atlases)
        expected_diagnostics = {str(path.relative_to(root)).replace("\\", "/") for path in diagnostic_atlases}
        discovered_diagnostics = {
            media_path
            for item in ledger["sourceOnlyMedia"] if item["kind"] == "relationship"
            for media_path in item["nativeV4"] if "relationship-diagnostic-v8" in media_path
        }
        self.assertEqual(expected_diagnostics, discovered_diagnostics)
        for item in ledger["sourceOnlyMedia"]:
            preferred_paths = [item["path"], item.get("rawPath"), *item.get("nativeV4", [])]
            self.assertTrue(all("quarantine" not in path.lower() and "rejected-alpha-bridge" not in path.lower() for path in preferred_paths if path))

    def test_v8_combat_and_village_overviews_use_all_sixteen_actual_v4_atlases(self):
        root = Path(__file__).resolve().parents[2]
        manifest = root / "images/sprites/seryeong-appearance-v3/manifest"
        for kind in ("combat", "village"):
            with Image.open(manifest / f"v8-h1-{kind}-overview.png") as overview:
                self.assertEqual((1024, 1120), overview.size)
            with Image.open(manifest / f"v8-h1-{kind}-row-2-overview.gif") as motion:
                self.assertEqual((1024, 1024), motion.size)
                self.assertEqual(6, motion.n_frames)


if __name__ == "__main__":
    unittest.main()
