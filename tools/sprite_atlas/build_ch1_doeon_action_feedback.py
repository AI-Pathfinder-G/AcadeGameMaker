#!/usr/bin/env python3
"""Author visual-only pixel-art action feedback from finished game evidence.

Four original facing/action rows: left swing, left hurt, right swing, right
hurt. No copied/mirrored character media. Movement, damage, hitboxes, input
ownership and combat effects are NOT changed; every cue is merely a visible
response to an actual *completed* attack/fire or HP drop. Candidate art only.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path
from PIL import Image, ImageDraw

CELL,COLS,ROWS = 64,6,4
ROWS_DETAIL=("left-swing","left-hurt","right-swing","right-hurt")

def make_frame(row:int,frame:int)->Image.Image:
    if not (0<=row<ROWS and 0<=frame<COLS):
        raise ValueError("Invalid visual-only feedback frame")
    out=Image.new("RGBA",(CELL,CELL),(0,0,0,0))
    pen=ImageDraw.Draw(out)
    is_hurt=(row%2)==1
    is_left=row<2
    if not is_hurt and frame<4:
        # A swoosh is not a hit: use for FiredMiss as well as FiredHit.
        warm=[(249,226,145,230),(234,184,91,183),
              (201,135,69,125),(170,112,65,74)][frame]
        lit=(255,244,189,min(255,warm[3]+18))
        if is_left:
            pen.arc((4,12,36,43),108,246,fill=warm,width=2 if frame<2 else 1)
            pen.arc((7,16,35,41),116,235,fill=lit,width=1)
            x,y=(8+frame*2,29)
            pen.line((x,y,x+4,y-2),fill=lit,width=1)
            pen.point((x+2,y+4),fill=warm)
        else:
            pen.arc((28,12,60,43),-66,66,fill=warm,width=2 if frame<2 else 1)
            pen.arc((29,16,57,41),-56,58,fill=lit,width=1)
            x,y=(53-frame*2,29)
            pen.line((x,y,x-4,y-2),fill=lit,width=1)
            pen.point((x-2,y+4),fill=warm)
        # Small directional streak aligned with the hand / source fist.
        for k in range(max(0,3-frame)):
            if is_left:
                px,py=(11-k*2,22+k*5)
                pen.line((px,py,px-2,py+2),fill=warm,width=1)
            else:
                px,py=(53+k*2,22+k*5)
                pen.line((px,py,px+2,py+2),fill=warm,width=1)
    elif is_hurt and frame<5:
        # The only criterion for hurt feedback is real completed HP loss,
        # not an attack input or predicted enemy overlap.
        alpha=(225,193,155,105,58)[frame]
        red=(252,104,91,alpha)
        coral=(255,190,136,max(32,alpha-10))
        cx,cy=(39,25) if is_left else (25,25)
        # A segmented, restrained recoil spark near the upper torso,
        # not a looping particle system or a new projectile.
        pen.line((cx-3,cy-3,cx-5-frame//2,cy-5),fill=red,width=1)
        pen.line((cx+2,cy-4,cx+4+frame//3,cy-6),fill=coral,width=1)
        pen.line((cx+5,cy+1,cx+7,cy+2+frame//3),fill=red,width=1)
        if frame<3:
            pen.line((cx-1,cy+4,cx-2,cy+7),fill=coral,width=1)
            pen.point((cx+2,cy+1),fill=red)
    return out

def checker_preview(sheet:Image.Image)->Image.Image:
    four=sheet.resize((sheet.width*4,sheet.height*4),Image.Resampling.NEAREST)
    bg=Image.new("RGBA",four.size,(49,57,66,255))
    d=ImageDraw.Draw(bg)
    for y in range(0,four.height,16):
        for x in range(0,four.width,16):
            if ((x//16+y//16)&1):
                d.rectangle((x,y,x+15,y+15),fill=(71,80,90,255))
    return Image.alpha_composite(bg,four)

def make(output:Path):
    if output.exists():
        raise SystemExit("Output already exists; refuse to overwrite")
    output.mkdir(parents=True)
    sheet=Image.new("RGBA",(COLS*CELL,ROWS*CELL),(0,0,0,0))
    frames=[]
    for row in range(ROWS):
        for col in range(COLS):
            tile=make_frame(row,col)
            sheet.alpha_composite(tile,(col*CELL,row*CELL))
            assert tile.getbbox() is None or (
                tile.getbbox()[0]>=3 and tile.getbbox()[1]>=10
                and tile.getbbox()[2]<=61 and tile.getbbox()[3]<=46), (row,col)
            frames.append(dict(row=row,column=col,kind=ROWS_DETAIL[row],
                               nontransparent=sum(1 for a in tile.getchannel("A").tobytes() if a)))
    target=output/"Candidate-Doeon-action-feedback.png"
    sheet.save(target)
    checker_preview(sheet).save(output/"action-feedback-review-4x.png")
    report=dict(schema="chapter1-doeon-action-feedback-candidate-v1",
                status="candidate-art-pending-manual-approval",
                atlas=target.name,atlasSha256=hashlib.sha256(target.read_bytes()).hexdigest(),
                cellSize=[CELL,CELL],columns=COLS,rows=ROWS,
                pixelsPerUnit=18,pivotNormalized=[.5,.1875],
                rowNames=list(ROWS_DETAIL),frames=frames,
                nonGameplay=True,normalHitVsMissSemantics="swing for either fired hit/miss; hurt only for completed HP decrease",
                sourceCharacterArtMirrored=False,sourceCharacterArtUnmodified=True,
                usage="only first 4 fired-attack ticks and first 5 actual-hurt ticks; other clip states transparent",
                limitation="Pixel-art accent is a visual candidate, not approved final effects or independent QA")
    (output/"manifest.json").write_text(json.dumps(report,sort_keys=True,indent=2,ensure_ascii=False)+"\n",encoding="utf8")
    print("ACTION_FEEDBACK_ATLAS",target,"SHA256",report["atlasSha256"])
    print("OPAQUE_FRAMES",sum(frame["nontransparent"]>0 for frame in frames),"TOTAL",len(frames))
def main():
    p=argparse.ArgumentParser()
    p.add_argument("--output",type=Path,required=True)
    make(p.parse_args().output)
if __name__=="__main__":
    main()