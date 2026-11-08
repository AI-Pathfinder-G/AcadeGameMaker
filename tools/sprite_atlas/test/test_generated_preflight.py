"""Regression tests for offline, non-accepting sprite source diagnostics."""
from __future__ import annotations

import hashlib
import importlib.util
from pathlib import Path
from tempfile import TemporaryDirectory
import unittest

from PIL import Image, ImageDraw

SCRIPT = Path(__file__).resolve().parents[1] / "inspect_generated_keyposes.py"
spec = importlib.util.spec_from_file_location("ch1_source_keypose_preflight", SCRIPT)
assert spec is not None and spec.loader is not None
keyposes = importlib.util.module_from_spec(spec)
spec.loader.exec_module(keyposes)


class SourceKeyposePreflightTests(unittest.TestCase):
    @staticmethod
    def source_at(path: Path, rows: int):
        image = Image.new("RGB", (1536, 1024), (251, 3, 251))
        draw = ImageDraw.Draw(image)
        for row in range(rows):
            y0 = (1024*row)//rows
            for col in range(6):
                x0 = 256*col
                draw.rectangle((x0+75,y0+75,x0+137,y0+170),fill=(50,92,112))
        image.save(path)

    def test_chroma_not_exact_ff00ff_and_non_chroma_remains_opaque(self):
        with TemporaryDirectory() as tmp:
            path = Path(tmp)/"synthetic.png"
            self.source_at(path,3)
            data,rgba,bg=keyposes.source_image(path)
            self.assertEqual(rgba.shape,(1024,1536,4))
            self.assertTrue(bg[0,0],"near-magenta supplied by image generator must be transparent")
            self.assertEqual(int(rgba[0,0,3]),0)
            self.assertFalse(bg[100,100],"painted actor is not the chroma background")
            self.assertEqual(int(rgba[100,100,3]),255)
            self.assertEqual(hashlib.sha256(data).hexdigest(),hashlib.sha256(path.read_bytes()).hexdigest())

    def test_three_row_plan_uses_exact_floor_edges_not_four_rows(self):
        with TemporaryDirectory() as tmp:
            src=Path(tmp)/"source.png"
            dst=Path(tmp)/"output"
            self.source_at(src,3)
            data,rgba,bg=keyposes.source_image(src)
            result=keyposes.analysis("synthetic-roles",data,rgba,bg,dst,3)
            self.assertEqual(result["nonemptyCells"],18)
            self.assertEqual(result["edgeTouchCells"],0)
            self.assertEqual(len(result["rowBoundaryReview"]),2)
            with Image.open(dst/"uniform-scale-preview-atlas.png") as atlas:
                self.assertEqual(atlas.size,(384,192))
            import json
            details=json.loads((dst/"preflight.json").read_text("utf8"))
            self.assertEqual(details["defaultYEdges"],[0,341,682,1024])
            self.assertEqual(details["emptyCells"],0)
            self.assertEqual(details["disposition"],"source_only_needs_visual_review")

    def test_wrong_source_size_fails_closed(self):
        with TemporaryDirectory() as tmp:
            path=Path(tmp)/"tiny.png"
            Image.new("RGB",(32,32),(251,3,251)).save(path)
            with self.assertRaisesRegex(ValueError,"wrong size"):
                keyposes.source_image(path)

    def test_diagnostic_cannot_accept_or_modify_source(self):
        with TemporaryDirectory() as tmp:
            src=Path(tmp)/"source.png"; dst=Path(tmp)/"output"
            self.source_at(src,4)
            before=hashlib.sha256(src.read_bytes()).hexdigest()
            data,rgba,bg=keyposes.source_image(src)
            result=keyposes.analysis("synthetic-actor",data,rgba,bg,dst,4)
            self.assertEqual(result["nonemptyCells"],24)
            self.assertEqual(hashlib.sha256(src.read_bytes()).hexdigest(),before)
            self.assertEqual(result["disposition"],"source_only_needs_visual_review")
            self.assertTrue((dst/"checker-preview-4x.png").is_file())


if __name__=="__main__":
    unittest.main()