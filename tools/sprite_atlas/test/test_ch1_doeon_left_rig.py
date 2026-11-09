import importlib.util
import hashlib
import unittest
from pathlib import Path
import numpy as np
from PIL import Image

TOOL = Path(__file__).resolve().parents[1]/"rig_ch1_doeon_left_prototype.py"
spec=importlib.util.spec_from_file_location("left_rig",TOOL)
rig=importlib.util.module_from_spec(spec)
spec.loader.exec_module(rig)

class LeftArticulatedCandidateTests(unittest.TestCase):
    def synthetic_actor(self):
        frame=np.zeros((256,256,4),dtype=np.uint8)
        # Artificial left-facing silhouette: central jacket and head,
        # separately colored sleeves and boots for left-right provenance.
        frame[57:95,118:141,:]=[160,135,98,255]
        frame[95:155,113:147,:]=[35,44,60,255]
        frame[94:152,100:113,:]=[77,108,147,255]
        frame[94:152,149:163,:]=[77,105,140,255]
        frame[150:205,112:129,:]=[55,59,68,255]
        frame[151:205,134:155,:]=[51,59,74,255]
        frame[199:209,97:131,:]=[108,76,40,255]
        frame[199:209,134:164,:]=[121,80,35,255]
        return frame

    def test_24_frame_geometry_and_alpha_are_bounded(self):
        actor=self.synthetic_actor()
        parts,masks=rig.layers(actor)
        self.assertEqual(len(parts),6)
        for mode in rig.PER_ROW:
            fingerprints=set()
            for i in range(6):
                pose=rig.pose(parts,mode,i)
                im=np.asarray(pose)
                self.assertEqual(im.shape,(256,256,4))
                self.assertGreater(np.count_nonzero(im[:,:,3]),30)
                self.assertEqual(np.count_nonzero(im[0,:,3]),0)
                self.assertEqual(np.count_nonzero(im[:,0,3]),0)
                fingerprints.add(hashlib.sha256(im.tobytes()).hexdigest())
            self.assertGreaterEqual(len(fingerprints),4,mode)

    def test_independent_left_skeleton_not_horizontal_image_flip(self):
        self.assertEqual(rig.LEFT_HIP,(119,154))
        self.assertEqual(rig.RIGHT_HIP,(140,154))
        self.assertLess(rig.LEFT_SHOULDER[0],rig.RIGHT_SHOULDER[0])
        self.assertEqual(rig.ROWS,4)
        self.assertEqual(rig.COLS,6)
        self.assertEqual(rig.CELL,64)

    def test_distinct_walk_run_keys(self):
        for mode in ("walk","run"):
            values=[(round(rig.angles(mode,i)["left_leg"],3),
                     round(rig.angles(mode,i)["right_leg"],3)) for i in range(6)]
            self.assertEqual(len(set(values)),6)

if __name__=="__main__":
    unittest.main()