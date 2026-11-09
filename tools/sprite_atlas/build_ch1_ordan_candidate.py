#!/usr/bin/env python3
"""Build a provisional RIGHT Ordan boss phase atlas from the user's owned 13-ordan-combat master.

Source is strictly read-only. No blind left mirror, no gameplay authority, no source rewriting.
Preserves the 6x4 grid and original pose placement with uniform nearest-neighbor downsample.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw

SOURCE_SIZE=(1536,1024)
SOURCE_CELL=256
CELL=128
COLUMNS=6
ROWS=4
GROUPS=("guard-step","debt-line","seizure-weight","balance-audit")

def build(source:Path,out:Path)->dict:
    if out.exists():
        raise FileExistsError("Refuse to overwrite existing output")
    if not source.is_file() or source.resolve()==out.resolve():
        raise FileNotFoundError("Existing read-only Ordan source required")
    with Image.open(source) as loaded:
        if loaded.size!=SOURCE_SIZE or loaded.mode not in ("RGB","RGBA"):
            raise ValueError("Unexpected source geometry or mode")
        rgb=np.asarray(loaded.convert("RGB"),dtype=np.uint8)
    ch=rgb.astype(np.int16)
    r,g,b=ch[:,:,0],ch[:,:,1],ch[:,:,2]
    magenta=(r>=165)&(b>=165)&(g<=95)&(r-g>=108)&(b-g>=108)
    rgba=np.concatenate((rgb,np.where(magenta,0,255).astype(np.uint8)[:,:,None]),axis=2)
    rgba[magenta,:3]=0
    atlas=Image.new("RGBA",(CELL*COLUMNS,CELL*ROWS),(0,0,0,0))
    frames=[]
    for row in range(ROWS):
        for key in range(COLUMNS):
            tile=rgba[row*SOURCE_CELL:(row+1)*SOURCE_CELL,
                      key*SOURCE_CELL:(key+1)*SOURCE_CELL]
            ys,xs=np.nonzero(tile[:,:,3]>128)
            if len(xs)<1500:
                raise ValueError(f"Ordan key {row}:{key} incomplete or empty")
            edge=bool((tile[0,:,3]>0).any() or (tile[-1,:,3]>0).any()
                      or (tile[:,0,3]>0).any() or (tile[:,-1,3]>0).any())
            if edge:
                raise ValueError(f"Ordan source clips pose {row}:{key}")
            sprite=Image.fromarray(tile,"RGBA").resize((CELL,CELL),Image.Resampling.NEAREST)
            atlas.alpha_composite(sprite,(key*CELL,row*CELL))
            frames.append(dict(row=row,key=key,pattern=GROUPS[row],
                               sourceBox=[int(xs.min()),int(ys.min()),int(xs.max()+1),int(ys.max()+1)],
                               sourceFoot=int(ys.max()),
                               visiblePixels=int(len(xs)),
                               frameSha256=hashlib.sha256(np.asarray(sprite).tobytes()).hexdigest()))
    out.mkdir(parents=True)
    target=out/"Candidate-OrdanAttackRight.png"
    atlas.save(target)
    # Local review preview only. Do not add the preview/contact sheet to Unity.
    background=Image.new("RGB",(CELL*COLUMNS*2,CELL*ROWS*2),(42,47,53))
    background.paste(atlas.resize(background.size,Image.Resampling.NEAREST),
                     (0,0),atlas.resize(background.size,Image.Resampling.NEAREST))
    background.save(out/"ordan-candidate-review-2x.png")
    manifest=dict(schema="ch1-ordan-right-combat-phase-atlas-v1",
                  status="provisional-not-manual-approved",sourceName=source.name,
                  sourceSha256=hashlib.sha256(source.read_bytes()).hexdigest(),
                  atlasSha256=hashlib.sha256(target.read_bytes()).hexdigest(),
                  dimensions=[CELL*COLUMNS,CELL*ROWS],cell=[CELL,CELL],
                  rows=list(GROUPS),keysPerRow=COLUMNS,
                  noLeftMirror=True,manualArtAccepted=False,
                  combatPhysicsModified=False,frames=frames)
    (out/"manifest.json").write_text(json.dumps(manifest,ensure_ascii=False,
        indent=2,sort_keys=True)+"\n",encoding="utf-8")
    print("ORDAN_CANDIDATE",target,"SHA",manifest["atlasSha256"],"frames",len(frames))
    return manifest

if __name__=="__main__":
    parser=argparse.ArgumentParser()
    parser.add_argument("--source",required=True,type=Path)
    parser.add_argument("--output",required=True,type=Path)
    args=parser.parse_args()
    build(args.source,args.output)
