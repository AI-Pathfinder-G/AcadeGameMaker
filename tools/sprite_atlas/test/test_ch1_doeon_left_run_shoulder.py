import importlib.util
import unittest
from pathlib import Path

import cv2
import numpy as np

MODULE=Path(__file__).resolve().parents[1]/"repair_ch1_doeon_left_run_shoulder.py"
spec=importlib.util.spec_from_file_location("jump_root",MODULE)
repair=importlib.util.module_from_spec(spec)
spec.loader.exec_module(repair)

class RunShoulderJointTests(unittest.TestCase):
    @staticmethod
    def sample(two_pixels_apart=True):
        a=np.zeros((64,64,4),dtype=np.uint8)
        a[11:46,29:39,:]=[43,51,61,255] # main 350px
        if two_pixels_apart:
            a[22:28,20:28,:]=[88,74,58,255] # known arm root, gap x28
        else:
            a[22:28,17:25,:]=[88,74,58,255] # more than 1px gap
        return a

    def test_bridges_exactly_one_shoulder_pixel(self):
        a=self.sample()
        fixed,proof=repair.connect_shoulder(a)
        self.assertEqual(proof["beforeMainArea"],350)
        self.assertEqual(proof["beforeDetachedArmArea"],48)
        self.assertEqual(proof["afterSignificantComponents"],1)
        self.assertEqual(len(np.where(np.any(fixed!=a,axis=2))[0]),1)
        x,y=proof["pixelTopLeft"]
        self.assertEqual(x,28)
        self.assertGreaterEqual(y,20)
        self.assertLessEqual(y,27)
        self.assertEqual(int(fixed[y,x,3]),255)
        self.assertEqual(fixed[y,x,:3].tolist(),[88,74,58])

    def test_disallows_large_unknown_gap(self):
        a=self.sample(False)
        with self.assertRaises(ValueError):
            repair.connect_shoulder(a)

    def test_disallows_wrong_art_shape_or_component_count(self):
        a=np.zeros((64,64,4),dtype=np.uint8)
        a[11:46,29:39,:]=[43,51,61,255]
        with self.assertRaises(ValueError):
            repair.connect_shoulder(a)
        with self.assertRaises(ValueError):
            repair.connect_shoulder(np.zeros((32,32,4),dtype=np.uint8))

    def test_only_two_run_frames_may_change(self):
        atlas=np.zeros((256,384,4),dtype=np.uint8)
        for col in (2,3):
            x0=col*64
            tile=self.sample()
            atlas[128:192,x0:x0+64,:]=tile
        before=atlas.copy()
        for col in (2,3):
            x0=col*64
            fixed,proof=repair.connect_shoulder(
                atlas[128:192,x0:x0+64])
            atlas[128:192,x0:x0+64]=fixed
            self.assertEqual(proof["afterSignificantComponents"],1)
        self.assertEqual(int(np.count_nonzero(np.any(before!=atlas,axis=2))),2)
        np.testing.assert_array_equal(before[:128],atlas[:128])
        np.testing.assert_array_equal(before[192:],atlas[192:])
        for col in (0,1,4,5):
            x0=col*64
            np.testing.assert_array_equal(before[128:192,x0:x0+64],
                                         atlas[128:192,x0:x0+64])
if __name__=="__main__":
    unittest.main()