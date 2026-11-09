"""Source-backed tests of the wholly reworked pressure-ram/prism characters.

Covers production 32-frame geometry, distinctive silhouette, phase pose
differences, original palette and deterministic, nonoverwriting generation.
"""
from __future__ import annotations
import hashlib
import importlib.util
import json
import tempfile
import unittest
from pathlib import Path

import numpy as np
from PIL import Image

MODULE=Path(__file__).resolve().parents[1]/"build_ch1_regular_enemy_v3.py"
spec=importlib.util.spec_from_file_location("ch1_enemy_redesign_v3",MODULE)
artist=importlib.util.module_from_spec(spec)
spec.loader.exec_module(artist)


class Ch1MonsterV3Tests(unittest.TestCase):
    def test_32_independent_keyed_pixel_assets_have_valid_anchor_and_bounds(self):
        frames=set()
        for row in range(4):
            for key in range(8):
                sprite=artist.render(row,key)
                rgba=np.asarray(sprite)
                self.assertEqual(rgba.shape,(64,64,4))
                bounds=sprite.getbbox()
                self.assertGreaterEqual(bounds[0],1)
                self.assertGreaterEqual(bounds[1],1)
                self.assertLess(bounds[2],64)
                self.assertLessEqual(bounds[3],53)
                self.assertIn(int(np.where(rgba[:,:,3]>40)[0].max()),(50,51))
                self.assertGreater(int(np.count_nonzero(rgba[:,:,3])),75)
                frames.add(hashlib.sha256(rgba.tobytes()).hexdigest())
        self.assertEqual(len(frames),32,"All 32 player-facing action keys must be unique")

    def test_distinctive_ram_boiler_versus_monocular_triangle_and_colors(self):
        for side in (-1,1):
            walk=np.asarray(artist.walker(side,0))
            survey=np.asarray(artist.surveyor(side,0))
            pressure=(walk[:,:,:3]==artist.RUST_LIGHT[:3]).all(axis=2)
            prism=(survey[:,:,:3]==artist.CYAN[:3]).all(axis=2)
            self.assertGreater(int(pressure.sum()),20)
            self.assertGreater(int(prism.sum()),15)
            walk_alpha=walk[:,:,3]>64
            prism_alpha=survey[:,:,3]>64
            # A ram has broad boiler mass, the prism is a visibly lighter
            # surveying tripod with asymmetric suspended aperture.
            self.assertGreater(int(walk_alpha.sum()),int(prism_alpha.sum())+200)
            self.assertGreater(int(prism_alpha[:15].sum()),15,
                               "surveyor has a distinctive upper measuring halo")
            self.assertGreater(int(walk_alpha[18:39].sum()),450,
                               "walker has dominant pressure vessel torso")
            # No mirroring of pre-authored pixel sprites is used.
            other=np.asarray(artist.walker(-side,0))
            self.assertFalse(np.array_equal(walk[:,::-1,:],other))

    def test_strong_telegraph_attack_death_and_recoil_silhouette(self):
        for row in range(4):
            idle=np.asarray(artist.render(row,0))
            warn=np.asarray(artist.render(row,3))
            strike=np.asarray(artist.render(row,4))
            recoil=np.asarray(artist.render(row,5))
            dead=np.asarray(artist.render(row,6))
            after=np.asarray(artist.render(row,7))
            self.assertGreater(int(np.count_nonzero(np.any(idle!=warn,axis=2))),55)
            self.assertGreater(int(np.count_nonzero(np.any(warn!=strike,axis=2))),85)
            self.assertGreater(int(np.count_nonzero(np.any(idle!=dead,axis=2))),300)
            self.assertGreater(int(np.count_nonzero(np.any(recoil!=after,axis=2))),80)
            # Dropped mechanical bodies stay near floor, with no upright
            # "death" placeholder pretending to be the living idle.
            self.assertGreaterEqual(int(np.where(dead[:,:,3]>20)[0].min()),31)
            self.assertIn(int(np.where(dead[:,:,3]>20)[0].max()),(50,51))
            for idx in (1,2,3,4,5,7):
                self.assertFalse(np.array_equal(np.asarray(artist.render(row,idx)),dead))

    def test_source_version_is_material_silhouette_redesign_not_tint(self):
        # The known v2 art is only used as comparison; the v3 authoring
        # generator does NOT take any previous source pixels as input.
        root=Path(__file__).resolve().parents[3]
        path=root/"output/ch1-package-A-20261109/regular-enemy-authored-v2/Candidate-CH1-RegularEnemies.png"
        if not path.exists():self.skipTest("Local v2 baseline not present")
        original=np.asarray(Image.open(path).convert("RGBA"))
        v3=np.array([np.asarray(artist.render(row,0)) for row in range(4)])
        for row in range(4):
            old=original[row*64:(row+1)*64,:64,3]>64
            new=v3[row,:,:,3]>64
            jaccard=np.logical_and(old,new).sum()/np.logical_or(old,new).sum()
            self.assertLess(float(jaccard),.75,
                            "Silhouette must change, not just repaint: row "+str(row))

    def test_artifact_is_reproducible_and_has_native_1x_review(self):
        with tempfile.TemporaryDirectory() as temp:
            dest=Path(temp)/"regular-enemies-v3"
            artist.build(dest)
            manifest=json.loads((dest/"manifest.json").read_text("utf8"))
            atlas=dest/"Candidate-CH1-RegularEnemies-v3.png"
            self.assertEqual(manifest["schema"],
                             "ch1-walker-surveyor-silhouette-redesign-v3")
            self.assertEqual(manifest["status"],
                             "provisional-original-character-art-not-independent-approved")
            self.assertEqual(manifest["atlasSha256"],
                             hashlib.sha256(atlas.read_bytes()).hexdigest())
            self.assertEqual(len(manifest["allFrames"]),32)
            self.assertTrue((dest/"RegularEnemies-v3-review-4x.png").is_file())
            self.assertTrue((dest/"RegularEnemies-v3-native-1x.png").is_file())
            with Image.open(atlas) as im:
                self.assertEqual(im.size,(512,256))
            self.assertFalse(manifest["manualArtAccepted"])
            self.assertTrue(manifest["noCombatOrSaveLogicChange"])
            with self.assertRaises(SystemExit):artist.build(dest)


if __name__=="__main__":
    unittest.main()