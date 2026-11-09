"""Safety and articulation regressions for Ordan's independent LEFT master rig."""
from __future__ import annotations

import hashlib
import importlib.util
import tempfile
import unittest
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw

SOURCE=Path(__file__).resolve().parents[1]/"build_ch1_ordan_left_rig.py"
spec=importlib.util.spec_from_file_location("ordan_left_rig",SOURCE)
artist=importlib.util.module_from_spec(spec)
spec.loader.exec_module(artist)


class OrdanLeftRigTests(unittest.TestCase):
    def test_left_source_is_not_right_mirror_and_original_unchanged(self):
        root=Path(__file__).resolve().parents[3]
        src=root/"output/chapter1-playable-r1/project/Assets/Resources/Chapter1Ordan/Left.png"
        right=root/"output/chapter1-playable-r1/project/Assets/Resources/Chapter1Ordan/Right.png"
        if not src.is_file() or not right.is_file():
            self.skipTest("User-owned source reference is only available in local worktree")
        original=hashlib.sha256(src.read_bytes()).hexdigest()
        rightHash=hashlib.sha256(right.read_bytes()).hexdigest()
        with tempfile.TemporaryDirectory() as tmp:
            dest=Path(tmp)/"rig"
            info=artist.build(src,dest)
            self.assertEqual(info["sourceSha256"],original)
            self.assertEqual(hashlib.sha256(src.read_bytes()).hexdigest(),original)
            self.assertNotEqual(info["sourceSha256"],rightHash)
            self.assertTrue(info["originalLeftMasterUnmodified"])
            self.assertFalse(info["rightFramesMirrored"])
            self.assertFalse(info["rightSourceUsed"])
            self.assertFalse(info["manualArtAccepted"])
            self.assertEqual(len(info["frames"]),24)
            self.assertGreaterEqual(len({f["sha256"] for f in info["frames"]}),15)
            with Image.open(dest/"Candidate-OrdanAttackLeft.png") as sheet:
                self.assertEqual(sheet.size,(768,512))
                self.assertEqual(sheet.mode,"RGBA")
            with self.assertRaises(FileExistsError):
                artist.build(src,dest)

    def test_in_place_left_pose_rig_preserves_foot_anchor_and_unique_actions(self):
        root=Path(__file__).resolve().parents[3]
        src=root/"output/chapter1-playable-r1/project/Assets/Resources/Chapter1Ordan/Left.png"
        with Image.open(src) as original:
            master=original.convert("RGBA").resize((128,128),Image.Resampling.NEAREST)
        for row in range(4):
            frames=[np.asarray(artist.frame_sprite(master,row,key)) for key in range(6)]
            for frame in frames:
                self.assertEqual(frame.shape,(128,128,4))
                self.assertGreater(np.count_nonzero(frame[:,:,3]),900)
                self.assertTrue(np.array_equal(frame[113:125,35:80],
                                               np.asarray(master)[113:125,35:80]),
                                "feet and world-facing pivot must never move")
            self.assertFalse(np.array_equal(frames[1],frames[3]))
            self.assertFalse(np.array_equal(frames[3],frames[5]))
        with self.assertRaises(ValueError):
            artist.frame_sprite(master,4,0)

    def test_wrong_geometry_fails_before_creating_outputs(self):
        with tempfile.TemporaryDirectory() as tmp:
            base=Path(tmp)
            src=base/"invalid-left.png"
            Image.new("RGBA",(40,40),(0,0,0,0)).save(src)
            output=base/"out"
            with self.assertRaises(ValueError):
                artist.build(src,output)
            self.assertFalse(output.exists())


if __name__=="__main__":
    unittest.main()
