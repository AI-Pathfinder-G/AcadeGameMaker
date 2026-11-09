"""Source-backed validation for the left recoil's final 7-tick idle settle.

No changes to original art are allowed. The previous selector used hit key0
for ages9..11 although hit key5 precisely equals independent left idle key1,
creating a gratuitous return-to-recoil flash before idle resumed.
"""
import unittest
from pathlib import Path
import numpy as np
from PIL import Image

PROJECT=Path(__file__).resolve().parents[3]
ART=PROJECT/"output/chapter1-playable-r1/project/Assets/Resources/Chapter1Doeon"


def _frames():
    if not all((ART/f).exists() for f in ("Candidate-left-combat.png",
                                           "Candidate-left-movement.png")):
        raise unittest.SkipTest("Project candidate art not in CI environment")
    combat=np.asarray(Image.open(ART/"Candidate-left-combat.png").convert("RGBA"))
    idle=np.asarray(Image.open(ART/"Candidate-left-movement.png").convert("RGBA"))
    if combat.shape!=(256,384,4) or idle.shape!=(256,384,4):
        raise AssertionError("Unexpected 6x4 source art geometry")
    recoil=tuple(combat[64:128,i*64:(i+1)*64] for i in range(6))
    resting=tuple(idle[:64,i*64:(i+1)*64] for i in range(6))
    return recoil,resting

class SourceBackedRecoilSettleTests(unittest.TestCase):
    def test_last_recoil_frame_equals_original_independent_left_idle_key(self):
        recoil,idle=_frames()
        np.testing.assert_array_equal(recoil[5],idle[1],
            err_msg="Expected pixel-perfect authored idle-like recoil pose")
        self.assertGreater(np.count_nonzero(recoil[0]!=idle[1]),100)
    def test_return_to_start_frame_creates_unnecessary_art_flash(self):
        recoil,_=_frames()
        old=int(np.count_nonzero(np.any(recoil[5]!=recoil[0],axis=2)))
        self.assertGreaterEqual(old,70)
        self.assertEqual(np.count_nonzero(np.any(recoil[5]!=recoil[5],axis=2)),0)
    def test_settled_pose_is_closer_to_all_idle_frames_than_early_recoil(self):
        recoil,idle=_frames()
        diffs=lambda pose:[
            int(np.count_nonzero(np.any(pose!=stand,axis=2)))
            for stand in idle]
        old=diffs(recoil[0]);new=diffs(recoil[5])
        self.assertLess(sum(new),sum(old))
        self.assertLessEqual(sum(new),sum(old)*0.55)
        # No change to art alpha, pivot or attack/hitbox can be inferred
        # or required to improve the visual pose selection.
        for f in (0,5):
            self.assertEqual(recoil[f].shape,(64,64,4))
            self.assertEqual(np.nonzero(recoil[f][:,:,3]>128)[0].max(),51)

if __name__=="__main__":
    unittest.main()