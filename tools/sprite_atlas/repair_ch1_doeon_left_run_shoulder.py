#!/usr/bin/env python3
"""Bridge detached, left-authored run shoulder joint with ONE pixel per frame.

Only affects Candidate-left-movement RUN row 2, columns 2 & 3 if strict
prerequisites match the observed defect. Never edits the original authored
Left.png, right sprites, combat frames, collision or gameplay state.
"""
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path

import cv2
import numpy as np
from PIL import Image,ImageDraw

COLS,ROWS,CELL = 6,4,64
TARGET_ROW=2
TARGET_COLUMNS=(2,3)

def components(frame:np.ndarray):
    binary=(frame[:,:,3]>64).astype(np.uint8)
    n, labels, stats, _=cv2.connectedComponentsWithStats(binary,8)
    significant=sorted(
        (k for k in range(1,n) if int(stats[k,cv2.CC_STAT_AREA])>=8),
        key=lambda k:int(stats[k,cv2.CC_STAT_AREA]),reverse=True)
    return labels,stats,significant

def connect_shoulder(frame:np.ndarray):
    if frame.shape!=(CELL,CELL,4) or frame.dtype!=np.uint8:
        raise ValueError('Expected 64x64 RGBA sprite')
    labels,stats,groups=components(frame)
    if len(groups)!=2:
        raise ValueError(f'Expected exactly two major components, found {len(groups)}')
    main,arm=groups
    if not (300 <= stats[main,cv2.CC_STAT_AREA] <= 450 and
            35 <= stats[arm,cv2.CC_STAT_AREA] <= 80):
        raise ValueError('Unexpected silhouette areas: do not patch unknown sprite')
    if not (20 <= stats[arm,cv2.CC_STAT_LEFT] <= 22 and
            22 <= stats[arm,cv2.CC_STAT_TOP] <= 24):
        raise ValueError('Detached component is not the known upper-left arm')
    missing=[]
    for y in range(18,30):
        for x in range(24,32):
            if frame[y,x,3] != 0:continue
            neighborhood=labels[y-1:y+2,x-1:x+2]
            if main in neighborhood and arm in neighborhood:
                missing.append((y,x))
    if not missing:
        raise ValueError('Shoulder cannot be repaired with a single bounded pixel')
    # Prefer upper shoulder at one pixel above the arm, near torso garment.
    y,x=min(missing,key=lambda pos:(abs(pos[0]-21),abs(pos[1]-28),pos[0],pos[1]))
    out=frame.copy()
    # Use actual near-shoulder costume color; never introduce a new palette.
    # Pick the nearest opaque arm pixel in a 3x3 neighborhood to join the limb.
    choices=[(yy,xx) for yy in range(y-1,y+2) for xx in range(x-1,x+2)
             if 0<=yy<CELL and 0<=xx<CELL and labels[yy,xx]==arm]
    if not choices:
        raise ValueError('Cannot identify arm root palette')
    yy,xx=min(choices,key=lambda pos:(abs(pos[0]-y)+abs(pos[1]-x),pos[0],pos[1]))
    out[y,x,:]=frame[yy,xx,:]
    out[y,x,3]=255
    next_labels,next_stats,next_groups=components(out)
    if len(next_groups)!=1:
        raise RuntimeError('Two major components were not connected')
    delta=(out!=frame).any(axis=2)
    if np.count_nonzero(delta)!=1 or not delta[y,x]:
        raise RuntimeError('Strict one-pixel modification guarantee failed')
    return out,dict(pixelTopLeft=[x,y],sourceArmTopLeft=[int(xx),int(yy)],
                    beforeMainArea=int(stats[main,cv2.CC_STAT_AREA]),
                    beforeDetachedArmArea=int(stats[arm,cv2.CC_STAT_AREA]),
                    afterSignificantComponents=len(next_groups))

def checker(img:Image.Image):
    large=img.resize((img.width*4,img.height*4),Image.Resampling.NEAREST)
    bg=Image.new('RGBA',large.size,(51,60,69,255))
    d=ImageDraw.Draw(bg)
    for y in range(0,bg.height,16):
        for x in range(0,bg.width,16):
            if (x//16+y//16)%2:
                d.rectangle((x,y,x+15,y+15),fill=(74,83,92,255))
    return Image.alpha_composite(bg,large)

def main():
    parser=argparse.ArgumentParser()
    parser.add_argument('--input',required=True,type=Path)
    parser.add_argument('--output',required=True,type=Path)
    args=parser.parse_args()
    if args.output.exists():raise SystemExit('Refusing overwrite')
    inp=Image.open(args.input).convert('RGBA')
    if inp.size!=(COLS*CELL,ROWS*CELL):
        raise ValueError('Unexpected movement atlas')
    before=np.asarray(inp)
    after=before.copy()
    changes=[]
    for col in TARGET_COLUMNS:
        x0=col*CELL
        y0=TARGET_ROW*CELL
        modified,proof=connect_shoulder(before[y0:y0+CELL,x0:x0+CELL])
        after[y0:y0+CELL,x0:x0+CELL]=modified
        changes.append(dict(row=TARGET_ROW,column=col,**proof))
    changed=int(np.count_nonzero(np.any(before!=after,axis=2)))
    if changed!=2:
        raise RuntimeError(f'Unexpected pixel change count: {changed}')
    if not np.array_equal(before[:128],after[:128]) or not np.array_equal(before[192:],after[192:]):
        raise RuntimeError('Non-run rows modified')
    for col in (0,1,4,5):
        c=col*CELL
        if not np.array_equal(before[128:192,c:c+CELL],after[128:192,c:c+CELL]):
            raise RuntimeError('Unrelated run frame altered')
    args.output.mkdir(parents=True)
    file=args.output/'Candidate-left-movement-run-shoulder-fixed.png'
    amended=Image.fromarray(after,'RGBA')
    amended.save(file)
    checker(amended).save(args.output/'left-run-shoulder-fixed-review-4x.png')
    receipt=dict(schema='ch1-doeon-left-run-shoulder-root-v1',
                 status='candidate-awaiting-human-visual-acceptance',
                 sourceSha256=hashlib.sha256(args.input.read_bytes()).hexdigest(),
                 amendedSha256=hashlib.sha256(file.read_bytes()).hexdigest(),
                 originalMasterUntouched=True,
                 noMirroredRightSource=True,
                 combatAndPhysicsUnchanged=True,
                 changedPixels=changed,
                 targets=changes,
                 limitations='Only two detached shoulder contact pixels repaired; remaining joint animation requires manual QA')
    (args.output/'manifest.json').write_text(
        json.dumps(receipt,ensure_ascii=False,sort_keys=True,indent=2)+'\n',
        encoding='utf8')
    print('REPAIRED',changed,'pixels',receipt['targets'])
    print('SHA256',receipt['amendedSha256'])

if __name__=='__main__':
    main()