"""Test actual pixel FX and behavioral-authority scope, not mock FX UI."""
from __future__ import annotations

import importlib.util
import hashlib
import json
import tempfile
import unittest
from pathlib import Path

import numpy as np
from PIL import Image

SRC=Path(__file__).resolve().parents[1]/"build_ch1_regular_enemy_v3_effects.py"
module=importlib.util.spec_from_file_location("ch1_enemy_fx",SRC)
fx=importlib.util.module_from_spec(module)
module.loader.exec_module(fx)


class PressurePrismFXTests(unittest.TestCase):
    def test_only_actual_published_action_keys_have_visible_fx(self):
        count=0
        for row in range(4):
            for key in range(8):
                tile=np.asarray(fx.draw(row,key))
                self.assertEqual(tile.shape,(64,64,4))
                active=bool(np.any(tile[:,:,3]))
                self.assertEqual(active,key in fx.KEYS[row],(row,key))
                if active:
                    count+=1
                    yy,xx=np.nonzero(tile[:,:,3])
                    self.assertGreaterEqual(int(xx.min()),1)
                    self.assertLess(int(xx.max()),63)
                    self.assertGreaterEqual(int(yy.min()),1)
                    self.assertLess(int(yy.max()),55)
        self.assertEqual(count,14)
        with self.assertRaises(ValueError):fx.draw(5,3)
        with self.assertRaises(ValueError):fx.draw(0,8)

    def test_directional_pressure_burst_and_telegraph_art_not_mirrored(self):
        for row in (0,2):
            a=np.asarray(fx.draw(row,3))
            b=np.asarray(fx.draw(row+1,3))
            self.assertFalse(np.array_equal(a,b))
            self.assertGreater(int(np.count_nonzero(np.any(a!=b,axis=2))),14)
            self.assertFalse(np.array_equal(a[:,::-1,:],b),
                             "Independent source direction is not a flipped bitmap")
        for row in range(4):
            warning=np.asarray(fx.draw(row,3))
            attack=np.asarray(fx.draw(row,4))
            self.assertGreater(int(np.count_nonzero(np.any(warning!=attack,axis=2))),30)

    def test_hurt_is_only_published_surveyor_suppressed_optical_glitch(self):
        for row in (0,1):
            self.assertIsNone(fx.draw(row,5).getbbox(),
                "Walker may not claim a separate actual hit response")
        for row in (2,3):
            self.assertIsNotNone(fx.draw(row,5).getbbox(),
                "Surveyor Suppressed has a published phase")
        for row in range(4):
            for key in (0,1,2,6):
                self.assertIsNone(fx.draw(row,key).getbbox(),
                    "Idle/movement/dead cannot emit fabricated combat signals")

    def test_manifest_is_deterministic_and_rgba_atlas_has_32_cells(self):
        with tempfile.TemporaryDirectory() as d:
            dest=Path(d)/"fx"
            fx.build(dest)
            m=json.loads((dest/"manifest.json").read_text("utf8"))
            img=dest/"Candidate-CH1-RegularEnemyVfx.png"
            with Image.open(img) as f:
                self.assertEqual(f.size,(512,256))
            self.assertEqual(m["atlasSha256"],hashlib.sha256(img.read_bytes()).hexdigest())
            self.assertEqual(len(m["frames"]),32)
            self.assertEqual(sum(x["opaqueCount"]>0 for x in m["frames"]),14)
            self.assertEqual(m["schema"],"ch1-regular-enemy-v3-published-phase-vfx-v1")
            self.assertTrue(m["noGameplayOrPhysicsChange"])
            self.assertFalse(m["manualArtAccepted"])
            with self.assertRaises(SystemExit):fx.build(dest)


if __name__=="__main__":
    unittest.main()