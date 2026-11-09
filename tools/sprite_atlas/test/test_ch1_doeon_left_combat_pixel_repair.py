import importlib.util
import unittest
from pathlib import Path

import numpy as np

SCRIPT=Path(__file__).resolve().parents[1]/"repair_ch1_doeon_left_combat_pixels.py"
spec=importlib.util.spec_from_file_location("left_seams", SCRIPT)
module=importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)

class LeftCombatPixelTests(unittest.TestCase):
    def test_enclosed_core_hole_is_filled_with_neighbor_color(self):
        a=np.zeros((64,64,4),dtype=np.uint8)
        a[26:35,28:39]=[55,60,70,255]
        a[30,33]=0
        changed,loose,seams=module.clean_frame(a)
        self.assertEqual(seams,[[33,30]])
        self.assertEqual(loose,[])
        self.assertEqual(changed[30,33].tolist(),[55,60,70,255])
        np.testing.assert_array_equal(changed[26,28],a[26,28])

    def test_only_truly_detached_one_pixel_is_removed(self):
        a=np.zeros((64,64,4),dtype=np.uint8)
        a[25:36,28:38]=[40,50,65,255]
        a[11,9]=[193,100,65,255]
        changed,loose,seams=module.clean_frame(a)
        self.assertEqual(loose,[[9,11]])
        self.assertEqual(seams,[])
        self.assertEqual(int(changed[11,9,3]),0)
        np.testing.assert_array_equal(changed[25:36,28:38],a[25:36,28:38])

    def test_never_closes_open_boundary_gaps_or_costume_rivet(self):
        a=np.zeros((64,64,4),dtype=np.uint8)
        a[25:35,28:39]=[31,45,68,255]
        a[25,33]=0
        a[30,33]=[135,89,41,255]
        changed,loose,seams=module.clean_frame(a)
        np.testing.assert_array_equal(changed,a)
        self.assertEqual(loose,[])
        self.assertEqual(seams,[])

    def test_research_rows_and_other_frames_remain_unchanged(self):
        from PIL import Image
        root=Path(__file__).resolve().parents[3]/"output/ch1-package-A-20261109"
        # Local artifact proof is optional and never required to run CI.
        original=root/"left-combat-prototype-v1/Candidate-left-combat-PROTOTYPE.png"
        cleaned=root/"left-combat-seam-cleanup/Candidate-left-combat-cleaned.png"
        if not original.exists() or not cleaned.exists():
            self.skipTest("Local QA artifact not present")
        a=np.asarray(Image.open(original).convert("RGBA"))
        b=np.asarray(Image.open(cleaned).convert("RGBA"))
        np.testing.assert_array_equal(a[128:],b[128:])
        self.assertEqual(a.shape,b.shape)
        self.assertLessEqual(int(np.count_nonzero(np.any(a!=b,axis=2))),35)

if __name__=="__main__":
    unittest.main()