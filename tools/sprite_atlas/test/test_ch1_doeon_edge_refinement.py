import importlib.util
import json
import unittest
from pathlib import Path
import numpy as np

MODULE=Path(__file__).resolve().parents[1]/"refine_ch1_doeon_edges.py"
spec=importlib.util.spec_from_file_location("edge_refiner",MODULE)
refiner=importlib.util.module_from_spec(spec)
spec.loader.exec_module(refiner)

class CandidateEdgeTests(unittest.TestCase):
    def test_edge_magenta_is_recolored_without_altering_alpha(self):
        pixels=np.zeros((5,5,4),dtype=np.uint8)
        pixels[2,2]=[35,39,41,255]
        pixels[2,3]=[175,10,181,255]
        revised,stats=refiner.refine(pixels)
        self.assertEqual(stats["repaired"],1)
        np.testing.assert_array_equal(revised[:,:,3],pixels[:,:,3])
        self.assertEqual(revised[2,3].tolist(),[35,39,41,255])
    def test_inner_purple_does_not_rewrite_intentional_palette(self):
        pixels=np.zeros((9,9,4),dtype=np.uint8)
        pixels[1:8,1:8,:]=[50,52,54,255]
        pixels[4,4]=[149,52,142,255]
        revised,stats=refiner.refine(pixels)
        self.assertEqual(stats["repaired"],0)
        np.testing.assert_array_equal(revised,pixels)
    def test_no_opaque_neighbor_retains_prototype_detail(self):
        pixels=np.zeros((5,5,4),dtype=np.uint8)
        pixels[2,2]=[175,10,181,255]
        revised,stats=refiner.refine(pixels)
        self.assertEqual(stats["retainedForReview"],1)
        np.testing.assert_array_equal(revised,pixels)
    def test_all_24_frames_preserve_geometry_and_alpha(self):
        original=np.zeros((256,384,4),dtype=np.uint8)
        for row in range(4):
            for col in range(6):
                y=row*64+31
                x=col*64+31
                original[y,x]=[35,39,41,255]
                original[y,x+1]=[175,10,181,255]
        output,stats=refiner.refine(original)
        self.assertEqual(stats["repaired"],24)
        np.testing.assert_array_equal(original[:,:,3],output[:,:,3])
        self.assertEqual(output.shape,original.shape)
        for row in range(4):
            for col in range(6):
                y=row*64+31
                x=col*64+32
                self.assertEqual(output[y,x].tolist(),[35,39,41,255])
if __name__=="__main__":
    unittest.main()