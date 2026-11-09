import importlib.util
import json
import tempfile
import unittest
from pathlib import Path
import numpy as np
from PIL import Image

TOOL=Path(__file__).resolve().parents[1]/"build_ch1_doeon_action_feedback.py"
spec=importlib.util.spec_from_file_location("doeon_feedback",TOOL)
media=importlib.util.module_from_spec(spec)
spec.loader.exec_module(media)

class DoeonActionFeedbackArtTests(unittest.TestCase):
    def test_four_distinct_directional_events_have_finite_lifetimes(self):
        for row in range(4):
            for frame in range(6):
                tile=media.make_frame(row,frame)
                self.assertEqual(tile.size,(64,64))
                self.assertEqual(tile.mode,"RGBA")
                a=np.asarray(tile)[:,:,3]
                expected=frame<(5 if row%2 else 4)
                self.assertEqual(bool(np.any(a)),expected,(row,frame))
                if expected:
                    self.assertEqual(int(np.count_nonzero(a[0,:])),0)
                    self.assertEqual(int(np.count_nonzero(a[-1,:])),0)
                    self.assertEqual(int(np.count_nonzero(a[:,0])),0)
                    self.assertEqual(int(np.count_nonzero(a[:,-1])),0)

    def test_swing_only_not_a_fake_hit_spark(self):
        for row in (0,2):
            f=np.asarray(media.make_frame(row,0).convert("RGBA"))
            opaque=f[:,:,3]>0
            self.assertTrue(np.any(opaque))
            # Swing uses gold/ivory; no red-damage shard is fabricated
            self.assertGreater(float(f[:,:,1][opaque].mean()),100.)
            self.assertGreater(float(f[:,:,2][opaque].mean()),65.)
            self.assertLess(int(np.count_nonzero(f[45:,:,3])),3)
        for row in (1,3):
            f=np.asarray(media.make_frame(row,0).convert("RGBA"))
            colors=f[f[:,:,3]>0,:3]
            self.assertTrue(any(int(rgb[0])>235 and int(rgb[1])<140
                                for rgb in colors))

    def test_left_and_right_effects_are_independent_art(self):
        for row in (0,1):
            left=np.asarray(media.make_frame(row,0))
            right=np.asarray(media.make_frame(row+2,0))
            self.assertFalse(np.array_equal(left,right))
            x0=np.nonzero(left[:,:,3]>0)[1]
            x1=np.nonzero(right[:,:,3]>0)[1]
            if row==0: # left swing vs right swing
                self.assertLess(float(x0.mean()),32.)
                self.assertGreater(float(x1.mean()),32.)
            else: # left shoulder recoil and right shoulder recoil
                self.assertGreater(float(x0.mean()),32.)
                self.assertLess(float(x1.mean()),32.)

    def test_full_atlas_manifest_lifecycle_and_no_overwrite(self):
        with tempfile.TemporaryDirectory() as root:
            dest=Path(root)/"sample"
            media.make(dest)
            img=np.asarray(Image.open(dest/"Candidate-Doeon-action-feedback.png"))
            self.assertEqual(img.shape,(256,384,4))
            meta=json.loads((dest/"manifest.json").read_text("utf8"))
            self.assertEqual(len(meta["frames"]),24)
            self.assertEqual(meta["status"],"candidate-art-pending-manual-approval")
            self.assertEqual(sum(x["nontransparent"]>0 for x in meta["frames"]),18)
            np.testing.assert_array_equal(img[0:64,320:384],np.zeros((64,64,4),dtype=np.uint8))
            np.testing.assert_array_equal(img[128:192,320:384],np.zeros((64,64,4),dtype=np.uint8))
            with self.assertRaises(SystemExit):
                media.make(dest)

if __name__=="__main__":
    unittest.main()