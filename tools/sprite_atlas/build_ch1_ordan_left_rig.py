#!/usr/bin/env python3
"""Provisional Ordan LEFT combat animation from the distinct authored LEFT master.

Articulated cutout rig and original procedural left-facing accents only.
Never mirror/transform RIGHT combat pixels, mutate the left master, or
change gameplay. This is not hand-drawn final art acceptance.
"""
from __future__ import annotations
import argparse
import hashlib
import json
import math
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw

CELL=128
COLS=6
ROWS=4
POSE_GROUPS=("guard-step","debt-line","seizure-weight","balance-audit")
INK=(18,20,22,255)
STEEL=(59,61,64,255)
GOLD=(212,156,64,255)
HIGHLIGHT=(255,206,96,255)
RUST=(119,52,43,255)
PALE=(255,237,171,255)
STATES=(
    (0,-5,3,-6,5,0),
    (0,-26,-46,-70,-53,-14),
    (0,-32,-64,-116,-97,-18),
    (0,-19,-38,-67,-57,-12),
)

def digest(data:bytes)->str:
    return hashlib.sha256(data).hexdigest()

def frame_sprite(master:Image.Image,row:int,key:int)->Image.Image:
    if not 0<=row<ROWS or not 0<=key<COLS:
        raise ValueError("invalid left phase")
    angle=STATES[row][key]
    canvas=master.copy()
    # Isolate the independently drawn left gauntlet along its actual contour.
    # The parent torso/cloth remains; cutout seams are obscured by a small
    # shadowed red sleeve underlay at the original elbow.
    contour=[(57,49),(64,52),(66,58),(63,65),(59,72),
             (52,74),(47,72),(45,68),(47,62),(50,55)]
    region=Image.new("L",(CELL,CELL),0)
    ImageDraw.Draw(region).polygon(contour,fill=255)
    arm=Image.new("RGBA",(CELL,CELL),(0,0,0,0))
    arm.paste(master,(0,0),region)
    canvas.paste((0,0,0,0),(0,0,CELL,CELL),region)
    under=ImageDraw.Draw(canvas)
    under.polygon([(54,51),(63,50),(64,66),(56,70),(48,66)],
                  fill=RUST)
    under.line([(53,53),(61,51),(62,59)],fill=INK,width=3)
    under.line([(54,54),(60,54)],fill=GOLD,width=1)
    # PIL rotation uses original left-layer pixels only, no right pose.
    turned=arm.rotate(angle,resample=Image.Resampling.NEAREST,
                      center=(61,51),expand=False)
    canvas.alpha_composite(turned)
    d=ImageDraw.Draw(canvas)
    d.ellipse((57,47,64,54),fill=INK)
    d.ellipse((59,49,62,52),fill=GOLD)
    # Correct position of the mechanical glove after pivot rotation.
    a=math.radians(angle)
    dx,dy=-8,17
    contact=(int(round(61+dx*math.cos(a)+dy*math.sin(a))),
             int(round(51-dx*math.sin(a)+dy*math.cos(a))))
    x,y=contact
    if row==0 and key in (1,2,3,4):
        d.line([(x,y-2),(x-2,y+1)],fill=GOLD,width=2)
        d.point((x-3,y+2),fill=HIGHLIGHT)
    if row==1:  # DebtLine: arm-mounted hook warning, not a projectile.
        if key in (1,2):
            d.line([(x-8,y-2),(x-3,y-2)],fill=HIGHLIGHT,width=1)
        elif key in (3,4):
            d.line([(x-2,y),(x-11,y-1)],fill=STEEL,width=3)
            d.line([(x-10,y-1),(x-15,y-5),(x-17,y-2)],fill=GOLD,width=2)
            d.point((x-13,y-7),fill=PALE)
        elif key==5:
            d.point((x-4,y-2),fill=GOLD)
    elif row==2: # SeizureWeight: visibly raise the counterweight.
        if key in (1,2,3,4):
            d.line([(x,y-2),(x-5,y-8)],fill=INK,width=3)
            d.rectangle((x-15,y-19,x-4,y-8),fill=INK)
            d.rectangle((x-13,y-17,x-6,y-10),fill=STEEL)
            d.line([(x-13,y-15),(x-6,y-15)],fill=GOLD,width=2)
            for cx,cy in ((x-12,y-16),(x-7,y-11)):
                d.point((cx,cy),fill=HIGHLIGHT)
            if key in (3,4):
                d.line([(x-16,y-7),(x-19,y-3)],fill=GOLD,width=2)
    elif row==3: # BalanceAudit: an instrumentation/stamp forearm cue.
        if key in (1,2,3,4):
            d.polygon([(x-3,y-4),(x-13,y-5),(x-17,y+1),
                       (x-13,y+5),(x-3,y+4)],fill=INK)
            d.polygon([(x-5,y-2),(x-12,y-3),(x-14,y+1),
                       (x-12,y+3),(x-5,y+2)],fill=STEEL)
            d.line([(x-11,y-3),(x-11,y+3)],fill=GOLD,width=1)
            if key in (3,4):
                d.line([(x-18,y-1),(x-21,y-5)],fill=HIGHLIGHT,width=2)
    return canvas

def build(source:Path,out:Path)->dict:
    if out.exists():
        raise FileExistsError("Refuse to overwrite source/evidence output")
    if not source.is_file():
        raise FileNotFoundError("Left original master missing")
    source_bytes=source.read_bytes()
    with Image.open(source) as f:
        if f.size!=(1215,1295) or f.mode!="RGBA":
            raise ValueError("Wrong independent left source geometry/mode")
        master=f.resize((CELL,CELL),Image.Resampling.NEAREST)
    atlas=Image.new("RGBA",(CELL*COLS,CELL*ROWS),(0,0,0,0))
    entries=[]
    for row in range(ROWS):
        for key in range(COLS):
            frame=frame_sprite(master,row,key)
            rgba=np.asarray(frame)
            box=frame.getbbox()
            if box is None or box[0]<1 or box[2]>=CELL or box[1]<1 or box[3]>127:
                raise ValueError(f"clipped left frame: {row}/{key} {box}")
            if int(np.count_nonzero(rgba[:,:,3]))<900:
                raise ValueError(f"empty left frame: {row}/{key}")
            atlas.alpha_composite(frame,(key*CELL,row*CELL))
            entries.append(dict(row=row,key=key,group=POSE_GROUPS[row],
                                angle=STATES[row][key],bounds=list(box),
                                sha256=digest(rgba.tobytes())))
    # Avoid presenting a static source copy as 24 independent animations.
    if len({x["sha256"] for x in entries})<15:
        raise ValueError("insufficient distinct left action frames")
    out.mkdir(parents=True)
    png=out/"Candidate-OrdanAttackLeft.png"
    atlas.save(png)
    review=Image.new("RGBA",atlas.size,(40,48,57,255))
    review.alpha_composite(atlas)
    review.resize((atlas.width*2,atlas.height*2),
                  Image.Resampling.NEAREST).convert("RGB").save(
                      out/"left-phase-candidate-2x.png")
    manifest=dict(schema="ch1-ordan-left-rig-from-owned-left-static-v1",
                  status="provisional-cutout-rig-not-final-hand-authored",
                  sourceFile=source.name,sourceSha256=digest(source_bytes),
                  atlasSha256=digest(png.read_bytes()),
                  size=[atlas.width,atlas.height],cellSize=CELL,
                  rightSourceUsed=False,rightFramesMirrored=False,
                  originalLeftMasterUnmodified=True,
                  manualArtAccepted=False,independentQA=False,
                  frames=entries)
    (out/"manifest.json").write_text(json.dumps(manifest,ensure_ascii=False,
        indent=2,sort_keys=True)+"\n",encoding="utf-8")
    print("LEFT_CANDIDATE",png,"frames",len(entries),
          "unique",len({x["sha256"] for x in entries}),
          "sha256",manifest["atlasSha256"])
    return manifest

if __name__=="__main__":
    ap=argparse.ArgumentParser()
    ap.add_argument("--source",required=True,type=Path)
    ap.add_argument("--output",required=True,type=Path)
    a=ap.parse_args()
    build(a.source,a.output)
