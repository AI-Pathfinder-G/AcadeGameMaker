"""Tests the safe deterministic extraction of user-authored Ordan source poses."""
from __future__ import annotations

import hashlib
import importlib.util
import tempfile
import unittest
from pathlib import Path

from PIL import Image, ImageDraw

FILE=Path(__file__).resolve().parents[1]/"build_ch1_ordan_candidate.py"
spec=importlib.util.spec_from_file_location("ch1_ordan_candidate",FILE)
mod=importlib.util.module_from_spec(spec)
spec.loader.exec_module(mod)


class OrdanCandidateTests(unittest.TestCase):
    def make_source(self,path:Path)->None:
        img=Image.new("RGB",(1536,1024),(251,3,251))
        draw=ImageDraw.Draw(img)
        for row in range(4):
            for col in range(6):
                x=col*256
                y=row*256
                draw.rectangle((x+42,y+15,x+180,y+247),
                               fill=(40+row*12,60+col*5,35+row*14))
                draw.ellipse((x+105,y+45,x+145,y+80),fill=(195,167,82))
        img.save(path)

    def test_alpha_sizing_provenance_and_original_preservation(self):
        with tempfile.TemporaryDirectory() as tmp:
            parent=Path(tmp)
            raw=parent/"13-ordan-combat.png"
            self.make_source(raw)
            before=hashlib.sha256(raw.read_bytes()).hexdigest()
            out=parent/"candidate"
            result=mod.build(raw,out)
            self.assertEqual(result["sourceSha256"],before)
            self.assertEqual(len(result["frames"]),24)
            self.assertTrue(result["noLeftMirror"])
            self.assertFalse(result["manualArtAccepted"])
            self.assertEqual(hashlib.sha256(raw.read_bytes()).hexdigest(),before)
            with Image.open(out/"Candidate-OrdanAttackRight.png") as atlas:
                self.assertEqual(atlas.size,(768,512))
                self.assertEqual(atlas.mode,"RGBA")
                self.assertEqual(atlas.getpixel((0,0))[3],0)
                self.assertEqual(atlas.getpixel((80,80))[3],255)
            with self.assertRaises(FileExistsError):
                mod.build(raw,out)

    def test_rejects_clipped_source_pose(self):
        with tempfile.TemporaryDirectory() as tmp:
            parent=Path(tmp)
            raw=parent/"13-ordan-combat.png"
            self.make_source(raw)
            with Image.open(raw) as src:
                src=src.copy()
                src.putpixel((0,0),(22,18,25))
                src.save(raw)
            with self.assertRaisesRegex(ValueError,"clips"):
                mod.build(raw,parent/"wrong")

    def test_rejects_wrong_geometry(self):
        with tempfile.TemporaryDirectory() as tmp:
            parent=Path(tmp)
            raw=parent/"13-ordan-combat.png"
            Image.new("RGB",(100,100),(251,3,251)).save(raw)
            with self.assertRaisesRegex(ValueError,"geometry"):
                mod.build(raw,parent/"bad")


if __name__=="__main__":
    unittest.main()
