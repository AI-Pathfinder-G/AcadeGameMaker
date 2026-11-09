import importlib.util
import json
import tempfile
import unittest
from pathlib import Path
import numpy as np
from PIL import Image

MODULE=Path(__file__).resolve().parents[1]/"align_ch1_doeon_right_swing_to_hand.py"
spec=importlib.util.spec_from_file_location("right_hand",MODULE)
align=importlib.util.module_from_spec(spec)
spec.loader.exec_module(align)

class RightAttackContactTests(unittest.TestCase):
    def test_exact_rightward_art_moves_left_3_without_pixel_loss(self):
        tile=np.zeros((64,64,4),dtype=np.uint8)
        tile[18:40,48:59]=[214,177,91,230]
        tile[30,52]=[249,226,145,255]
        out=align.shift_lossless(tile,-3)
        np.testing.assert_array_equal(out[:,45:56],tile[:,48:59])
        self.assertEqual(np.count_nonzero(tile[:,:,3]),
                         np.count_nonzero(out[:,:,3]))
        np.testing.assert_array_equal(out[:,:,3]>0,
                                      np.roll(tile[:,:,3]>0,-3,axis=1))

    def test_refuses_unknown_or_clipping_edits(self):
        tile=np.zeros((64,64,4),dtype=np.uint8)
        tile[10,1]=[244,201,90,255]
        with self.assertRaises(ValueError):
            align.shift_lossless(tile,-3)
        with self.assertRaises(ValueError):
            align.shift_lossless(tile,-4)
        with self.assertRaises(ValueError):
            align.shift_lossless(np.zeros((32,32,4),dtype=np.uint8),-3)

    def test_full_atlas_updates_only_four_right_swing_frames(self):
        atlas=np.zeros((256,384,4),dtype=np.uint8)
        for row in range(4):
            for col in range(6):
                atlas[row*64+18:row*64+38,col*64+50:col*64+57] = \
                    [30+row*40,53+col,94,180]
        with tempfile.TemporaryDirectory() as tmp:
            inp=Path(tmp)/"before.png"
            out=Path(tmp)/"corrected"
            Image.fromarray(atlas,'RGBA').save(inp)
            align.refine(inp,out)
            after=np.asarray(Image.open(
                out/"Candidate-action-feedback-hand-aligned.png").convert("RGBA"))
            ledger=json.loads((out/"manifest.json").read_text('utf8'))
            self.assertEqual(ledger["schema"],
                "doeon-action-feedback-right-hand-contact-v2")
            self.assertEqual(len(ledger["correctedRightSwingFrames"]),4)
            self.assertTrue(ledger["candidateOnly"])
            for row in range(4):
                for col in range(6):
                    before=atlas[row*64:(row+1)*64,col*64:(col+1)*64]
                    updated=after[row*64:(row+1)*64,col*64:(col+1)*64]
                    if row==2 and col<4:
                        np.testing.assert_array_equal(updated[:,47:54],
                                                      before[:,50:57])
                    else:
                        np.testing.assert_array_equal(updated,before)
            with self.assertRaises(SystemExit):
                align.refine(inp,out)

    def test_current_user_authored_right_pose_contact_improves(self):
        # Environment-dependent direct asset proof is optional in generic CI.
        import cv2
        root=Path(__file__).resolve().parents[3]
        source=root/"output/chapter1-playable-r1/project/Assets/Resources/Chapter1Doeon"
        prior=root/"output/ch1-package-A-20261109/doeon-action-feedback-v1/Candidate-Doeon-action-feedback.png"
        fixed=root/"output/ch1-package-A-20261109/doeon-action-feedback-hand-contact-v2/Candidate-action-feedback-hand-aligned.png"
        combat=source/"Candidate-combat.png"
        if not(all(f.exists() for f in (prior,fixed,combat))):
            self.skipTest("Local project art not present")
        a=np.asarray(Image.open(prior).convert("RGBA"))
        b=np.asarray(Image.open(fixed).convert("RGBA"))
        c=np.asarray(Image.open(combat).convert("RGBA"))
        # First two swing phases have the authored extended hand on row0 col3.
        actor=c[:64,3*64:4*64,3]>64
        distance=cv2.distanceTransform((~actor).astype('uint8'),cv2.DIST_L2,5)
        before=a[2*64:3*64,:64,3]>64
        after=b[2*64:3*64,:64,3]>64
        self.assertGreater(float(distance[before].min()),4.)
        self.assertLessEqual(float(distance[after].min()),2.)
        np.testing.assert_array_equal(a[:128],b[:128])
        np.testing.assert_array_equal(a[192:],b[192:])
if __name__=="__main__":
    unittest.main()