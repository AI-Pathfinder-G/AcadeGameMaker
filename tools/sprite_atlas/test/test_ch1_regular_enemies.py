import hashlib
import importlib.util
import json
import tempfile
import unittest
from pathlib import Path

import numpy as np
from PIL import Image

MODULE=Path(__file__).resolve().parents[1]/"build_ch1_regular_enemy_character_atlas.py"
spec=importlib.util.spec_from_file_location("regular_enemy_art",MODULE)
artist=importlib.util.module_from_spec(spec)
spec.loader.exec_module(artist)

class RegularEnemySourceArtTests(unittest.TestCase):
    def test_32_frames_have_solid_nonempty_silhouettes(self):
        for row in range(4):
            keys=set()
            for key in range(8):
                art=np.asarray(artist.frame(row,key))
                self.assertEqual(art.shape,(64,64,4))
                self.assertGreater(int(np.count_nonzero(art[:,:,3]>0)),80)
                self.assertTrue(np.all(art[0,:,3]==0))
                self.assertTrue(np.all(art[:,0,3]==0))
                if key!=6:
                    self.assertGreaterEqual(int(np.nonzero(art[:,:,3]>0)[0].max()),50)
                    self.assertLessEqual(int(np.nonzero(art[:,:,3]>0)[0].max()),52)
                keys.add(hashlib.sha256(art.tobytes()).hexdigest())
            self.assertEqual(len(keys),8,"Each combat/idle pose must differ in authored pixels")

    def test_walker_guard_and_surveyor_are_distinct_facing_designs(self):
        for key in (0,2,3,4,5,6):
            w=np.asarray(artist.frame(0,key))
            s=np.asarray(artist.frame(2,key))
            self.assertGreater(int(np.count_nonzero(w!=s)),220)
            # These are independently built directional sources, not copies.
            a=np.asarray(artist.frame(0,key))
            b=np.asarray(artist.frame(1,key))
            self.assertFalse(np.array_equal(a[:,::-1],b),
                "Asymmetric armor and gear should not use mirrored whole sprites")
            c=np.asarray(artist.frame(2,key))
            d=np.asarray(artist.frame(3,key))
            self.assertFalse(np.array_equal(c[:,::-1],d),
                "Surveyor has separately composed left-side equipment")

    def test_defeat_and_telegraph_are_not_reused_idle_art(self):
        for row in range(4):
            idle=np.asarray(artist.frame(row,0))
            prepare=np.asarray(artist.frame(row,3))
            active=np.asarray(artist.frame(row,4))
            defeated=np.asarray(artist.frame(row,6))
            self.assertGreater(np.count_nonzero(idle!=prepare),45)
            self.assertGreater(np.count_nonzero(prepare!=active),40)
            self.assertGreater(np.count_nonzero(idle!=defeated),90)

    def test_generation_and_manifest_do_not_overwrite(self):
        with tempfile.TemporaryDirectory() as d:
            output=Path(d)/"art"
            artist.build(output)
            atlas=Image.open(output/"Candidate-CH1-RegularEnemies.png").convert("RGBA")
            self.assertEqual(atlas.size,(512,256))
            m=json.loads((output/"manifest.json").read_text("utf8"))
            self.assertEqual(m["schema"],
                "ch1-regular-enemies-authored-character-pixel-candidate-v1")
            self.assertEqual(m["status"],"provisional-visual-only-not-final-approved")
            self.assertEqual(len(m["frames"]),32)
            self.assertEqual(m["rowsByEnemy"],list(artist.NAMES))
            self.assertEqual(len(set(x["pixelsHash"] for x in m["frames"])),32)
            self.assertEqual(m["atlasSha256"],hashlib.sha256(
                (output/"Candidate-CH1-RegularEnemies.png").read_bytes()).hexdigest())
            with self.assertRaises(SystemExit):
                artist.build(output)

if __name__=="__main__":
    unittest.main()