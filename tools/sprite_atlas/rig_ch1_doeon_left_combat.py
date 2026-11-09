#!/usr/bin/env python3
"""Provisional left-facing punch and hit-react animation, NOT hand-drawn acceptance.

Uses the separately authored left master and layered sprite geometry; there is
no flipX, right costume source, combat-hit timing, or change to game mechanics.
Four visual rows provide alternate gestures, only attack/hit are bound in Unity.
"""
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path
import numpy as np
import cv2
from PIL import Image, ImageDraw
from rig_ch1_doeon_left_prototype import (
    FRAME,CELL,ROWS,COLS,RAW_FOOT_Y,CENTER_HIP,
    LEFT_SHOULDER,RIGHT_SHOULDER,LEFT_HIP,RIGHT_HIP,
    prepare_master,layers,affine_rotate,composite,checker
)

ANIMATION_ROWS=("basic_attack","hit_recoil","heavy_attack_unbound","guard_recover_unbound")
# Action 0-5: anticipate, extend, full extension, strike, withdraw, ready.
ATTACK={
    "basic_attack": {
        "front":(0,-24,-56,-77,-35,-8),
        "back":(2,12,26,28,17,2),
        "body":(0,4,10,12,6,0),
        "shift":(0,-2,-4,-6,-2,0),
        "bob":(0,-1,-2,-1,0,0),
        "lead_leg":(0,4,8,12,4,0),
        "trail_leg":(0,-2,-6,-7,-3,0),
    },
    "heavy_attack_unbound":{
        "front":(-5,-35,-67,-84,-61,-10),
        "back":(4,17,29,30,17,4),
        "body":(-1,6,13,14,9,0),
        "shift":(0,-2,-5,-8,-4,0),
        "bob":(0,-1,-3,-2,-1,0),
        "lead_leg":(0,5,8,13,9,0),
        "trail_leg":(0,-2,-8,-12,-7,0),
    }
}
HURT={
    "hit_recoil":{
        "front":(-3,10,32,43,18,0),
        "back":(-2,-11,-28,-35,-15,0),
        "body":(0,-4,-11,-15,-6,0),
        "shift":(0,2,4,5,2,0),
        "bob":(0,-2,-4,-2,-1,0),
        "lead_leg":(0,-2,-6,-8,-3,0),
        "trail_leg":(0,4,7,8,3,0),
    },
    "guard_recover_unbound":{
        "front":(-10,-31,-42,-36,-17,-3),
        "back":(8,20,31,25,12,1),
        "body":(0,-3,-7,-5,-2,0),
        "shift":(0,1,2,2,1,0),
        "bob":(0,-1,-2,-2,-1,0),
        "lead_leg":(0,-2,-3,-3,-1,0),
        "trail_leg":(0,3,5,4,2,0),
    }
}

def compose_action(parts, row_name:str, frame:int):
    if row_name in ATTACK:
        d=ATTACK[row_name]
    elif row_name in HURT:
        d=HURT[row_name]
    else:
        raise ValueError(row_name)
    lean=d["body"][frame];shift=(d["shift"][frame],d["bob"][frame])
    base=Image.new("RGBA",(FRAME,FRAME),(0,0,0,0))
    # Legs remain in an authored forward-facing stance, with modest planted
    # foot movement and no digital right-left reflection of the belt/strap.
    back_leg=affine_rotate(parts["right_leg"],RIGHT_HIP,d["trail_leg"][frame],shift)
    front_leg=affine_rotate(parts["left_leg"],LEFT_HIP,d["lead_leg"][frame],shift)
    composite(base,back_leg)
    composite(base,front_leg)
    # Rear arm swings with the torso, preserving shoulder attachment to body.
    rear=affine_rotate(parts["right_arm"],RIGHT_SHOULDER,d["back"][frame])
    rear=affine_rotate(rear,CENTER_HIP,lean,shift)
    composite(base,rear)
    torso=affine_rotate(parts["body"],CENTER_HIP,lean,shift)
    composite(base,torso)
    # Front fist moves to the left (negative angular values in OpenCV's
    # image-space rotation convention), not a mirrored right-handed punch.
    front=affine_rotate(parts["left_arm"],LEFT_SHOULDER,d["front"][frame])
    front=affine_rotate(front,CENTER_HIP,lean,shift)
    composite(base,front)
    pelvis=affine_rotate(parts["pelvis"],CENTER_HIP,lean,shift)
    composite(base,pelvis)
    arr=np.asarray(base).copy()
    yy,xx=np.nonzero(arr[:,:,3]>20)
    if len(yy)<700:
        raise RuntimeError(f"Disconnected or unexpectedly empty sprite: {row_name}/{frame}")
    # For grounded combat, planted boots remain exactly at foot baseline.
    dy=RAW_FOOT_Y-int(yy.max())
    if dy:
        translation=np.float32([[1,0,0],[0,1,dy]])
        arr=cv2.warpAffine(arr,translation,(FRAME,FRAME),
                           flags=cv2.INTER_NEAREST,
                           borderMode=cv2.BORDER_CONSTANT,borderValue=(0,0,0,0))
    return Image.fromarray(arr,"RGBA")

def make(source:Path,output:Path):
    if output.exists():
        raise SystemExit("Output already exists; never overwrite art")
    source_bytes=source.read_bytes()
    master,data,bbox=prepare_master(source)
    if data!=source_bytes:
        raise RuntimeError("Source proof changed during processing")
    parts,_=layers(master)
    output.mkdir(parents=True)
    atlas=Image.new("RGBA",(CELL*COLS,CELL*ROWS),(0,0,0,0))
    records=[]
    for row,name in enumerate(ANIMATION_ROWS):
        unique=set()
        for frame in range(COLS):
            pose=compose_action(parts,name,frame)
            tile=pose.resize((CELL,CELL),Image.Resampling.NEAREST)
            rgba=np.asarray(tile)
            ay,ax=np.nonzero(rgba[:,:,3]>32)
            if len(ax)==0:raise RuntimeError("Empty frame")
            bb=[int(ax.min()),int(ay.min()),int(ax.max()+1),int(ay.max()+1)]
            if bb[0]<1 or bb[1]<1 or bb[2]>CELL-1 or bb[3]>CELL:
                raise RuntimeError(f"Clipped frame {row}/{frame}: {bb}")
            if abs(bb[3]-52)>1:
                raise RuntimeError(f"Foot baseline drift {row}/{frame}: {bb[3]}")
            digest=hashlib.sha256(rgba.tobytes()).hexdigest()
            unique.add(digest)
            records.append(dict(row=row,frame=frame,clip=name,
                                bbox=bb,nontransparent=int(len(ax)),
                                spritePixelsHash=digest))
            atlas.alpha_composite(tile,(frame*CELL,row*CELL))
        if len(unique)<5:
            raise RuntimeError(f"Row not sufficiently distinct: {name}={len(unique)}")
    texture=output/"Candidate-left-combat-PROTOTYPE.png"
    atlas.save(texture)
    checker(atlas).save(output/"Candidate-left-combat-review-4x.png")
    rec=dict(schema="doeon-left-jointed-combat-candidate-v1",
             state="PROVISIONAL_NOT_HAND_DRAWN_OR_ART_ACCEPTED",
             sourceType="authored-independent-left-static",
             sourcePath=str(source),sourceSha256=hashlib.sha256(source_bytes).hexdigest(),
             originalAlphaBounds=list(bbox),
             atlasFile=texture.name,atlasSha256=hashlib.sha256(texture.read_bytes()).hexdigest(),
             stageUse={"basicAttack":"row0","hitReact":"row1"},
             unboundRows=["heavy_attack_unbound","guard_recover_unbound"],
             rows=list(ANIMATION_ROWS),columns=6,cellSize=[64,64],ppu=18,
             pivotNormalized=[.5,.1875],fullLeftCombatHandDrawn=False,
             mirroredRightArtwork=False,changesCombatHitbox=False,
             outputNote="Art/animation experiment only. Resolve elbow/weapon contacts, attack timeline, impact effects, and style before approval.",
             frames=records)
    (output/"manifest.json").write_text(
        json.dumps(rec,sort_keys=True,indent=2,ensure_ascii=False)+"\n",encoding="utf8")
    print("CANDIDATE",output,"frames",len(records),"sha",rec["atlasSha256"])

if __name__=="__main__":
    ap=argparse.ArgumentParser()
    ap.add_argument("--source",type=Path,required=True)
    ap.add_argument("--output",type=Path,required=True)
    a=ap.parse_args()
    make(a.source,a.output)