import importlib.util
import json
import tempfile
import unittest
from pathlib import Path

import numpy as np
from PIL import Image

TOOL = Path(__file__).resolve().parents[1]/"stabilize_ch1_doeon_right_torso.py"
spec = importlib.util.spec_from_file_location("torso_align", TOOL)
aligner = importlib.util.module_from_spec(spec)
spec.loader.exec_module(aligner)


class RightTorsoRootTests(unittest.TestCase):
    @staticmethod
    def tile(center, color=(53, 67, 76, 255)):
        tile = np.zeros((64, 64, 4), dtype=np.uint8)
        tile[24:41, center-2:center+3] = color
        tile[45:52, center-4:center+4] = (75, 65, 57, 255)
        return tile

    def test_source_anchor_and_reversible_pixel_translation(self):
        for offset in (-2, -1, 0, 1, 2):
            source = self.tile(31-offset)
            self.assertEqual(aligner.source_anchor(source), float(31-offset))
            after = aligner.slide_frame(source, offset)
            self.assertEqual(aligner.source_anchor(after), 31.0)
            np.testing.assert_array_equal(
                aligner.slide_frame(after, -offset), source)
            self.assertEqual(np.count_nonzero(source[:, :, 3]),
                             np.count_nonzero(after[:, :, 3]))
            self.assertEqual(int(np.nonzero(source[:, :, 3])[0].max()),
                             int(np.nonzero(after[:, :, 3])[0].max()))

    def test_source_refuses_invalid_geometry_and_pixel_clipping(self):
        with self.assertRaises(ValueError):
            aligner.slide_frame(self.tile(31), 3)
        with self.assertRaises(ValueError):
            aligner.slide_frame(np.zeros((32,32,4), dtype=np.uint8), 0)
        left = self.tile(31)
        left[15,0] = [255,44,35,255]
        with self.assertRaises(ValueError):
            aligner.slide_frame(left,-1)
        right = self.tile(31)
        right[15,63] = [255,44,35,255]
        with self.assertRaises(ValueError):
            aligner.slide_frame(right,1)

    def test_full_sheet_only_corrects_walk_and_run(self):
        original=np.zeros((256,384,4),dtype=np.uint8)
        # Both moving rows begin with deliberate authored torso jitter.
        offsets=((-1,0,1,1,2,-1), (0,0,-1,0,1,-1))
        for row,target in ((0,31),(1,34)):
            for col in range(6):
                original[row*64:(row+1)*64,col*64:(col+1)*64] = \
                    self.tile(target-offsets[row][col])
        # Other rows deliberately use unaligned unique art, never touched.
        for row in (2,3):
            for col in range(6):
                original[row*64+15:row*64+32,col*64+10:col*64+44] = \
                    [20+row+col,35+row+col,45+row+col,255]
        with tempfile.TemporaryDirectory() as tmp:
            source=Path(tmp)/"source.png"
            source_image=Image.fromarray(original, "RGBA")
            source_image.save(source)
            output=Path(tmp)/"candidate"
            aligner.build(source,output)
            changed=np.asarray(Image.open(
                output/"Candidate-right-movement-stabilized.png").convert("RGBA"))
            np.testing.assert_array_equal(original[128:],changed[128:])
            ledger=json.loads((output/"manifest.json").read_text("utf8"))
            self.assertEqual(ledger["schema"],
                             "ch1-doeon-right-torso-visual-alignment-v1")
            self.assertEqual(len(ledger["frameReceipts"]),12)
            self.assertEqual(len(ledger["unchangedRows"]),2)
            self.assertTrue(ledger["baselineUnchanged"])
            for item in ledger["frameReceipts"]:
                self.assertLessEqual(abs(item["shiftXpx"]),2)
                self.assertEqual(item["groundLastY"],51)
                self.assertLessEqual(abs(item["afterAnchorPx"]-
                                         (31 if item["clip"]=="walk" else 34)),.5)
            with self.assertRaises(ValueError):
                aligner.build(source, Path(tmp)/"wrong-hash", "0"*64)
            with self.assertRaises(SystemExit):
                aligner.build(source,output)

if __name__=="__main__":
    unittest.main()