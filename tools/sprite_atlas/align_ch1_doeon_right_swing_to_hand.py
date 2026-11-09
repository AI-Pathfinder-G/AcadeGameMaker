#!/usr/bin/env python3
"""Right-facing sword/punch swoosh contact correction, VFX ONLY.

Source right swing trails were 4-8px distant from the actual posed fist.
Translate only active 4 right-swing frames 3px toward the hand. No scaling,
interpolation, new attacks, hit particles on misses or changes to sprites/
combat/physics/saves. Preserve the v1 sheet as immutable input evidence.
"""
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw

CELL,COLS,ROWS=64,6,4
RIGHT_SWING_ROW=2
ACTIVE_FRAMES=tuple(range(4))
CONTACT_X=-3

def shift_lossless(tile:np.ndarray, dx:int=CONTACT_X)->np.ndarray:
    if tile.shape!=(CELL,CELL,4) or tile.dtype!=np.uint8:
        raise ValueError("Expected a 64x64 RGBA uint8 tile")
    if not isinstance(dx,int) or dx!=CONTACT_X:
        raise ValueError("Only the reviewed -3px right-contact shift is supported")
    result=np.zeros_like(tile)
    delta=-dx
    if np.any(tile[:,:delta,3]):
        raise ValueError("Left edge contains alpha and would be clipped")
    result[:,:CELL-delta]=tile[:,delta:]
    a=tile[:,:,3]>0
    b=result[:,:,3]>0
    if np.count_nonzero(a)!=np.count_nonzero(b):
        raise RuntimeError("Lost or gained pixel alpha while translating")
    if not np.array_equal(np.sort(result[b],axis=0),np.sort(tile[a],axis=0)):
        raise RuntimeError("RGBA palette shifted nonreversibly")
    return result

def preview(original:np.ndarray,refined:np.ndarray)->Image.Image:
    width=4*(64*4+12);height=2*(64*4+30)
    canvas=Image.new("RGBA",(width,height),(35,43,52,255))
    painter=ImageDraw.Draw(canvas)
    for f in ACTIVE_FRAMES:
        for stage,image in enumerate((original,refined)):
            tile=Image.fromarray(image[2*CELL:3*CELL,
                                      f*CELL:(f+1)*CELL],"RGBA")
            bg=Image.new("RGBA",(CELL*4,CELL*4),(53,62,72,255))
            bg.alpha_composite(tile.resize(bg.size,Image.Resampling.NEAREST))
            x=f*(CELL*4+12);y=stage*(CELL*4+30)
            canvas.alpha_composite(bg,(x,y+27))
            painter.text((x+6,y+6),f"RIGHT SWING {f} "+("BEFORE" if stage==0 else "AFTER"),
                         fill=(244,237,220,255))
    return canvas

def refine(source:Path, dest:Path):
    if dest.exists():
        raise SystemExit("Destination exists; refuse overwrite")
    raw=source.read_bytes()
    rgba=np.asarray(Image.open(source).convert("RGBA"))
    if rgba.shape!=(ROWS*CELL,COLS*CELL,4):
        raise ValueError("Expected 384x256 RGBA atlas")
    result=rgba.copy()
    frames=[]
    for col in ACTIVE_FRAMES:
        x0=col*CELL;y0=RIGHT_SWING_ROW*CELL
        before=rgba[y0:y0+CELL,x0:x0+CELL,:]
        after=shift_lossless(before)
        result[y0:y0+CELL,x0:x0+CELL,:]=after
        y,x=np.nonzero(before[:,:,3]>0)
        ny,nx=np.nonzero(after[:,:,3]>0)
        if len(x)<20 or int(nx.min())!=int(x.min())+CONTACT_X or int(nx.max())!=int(x.max())+CONTACT_X:
            raise RuntimeError("Unexpected art shift geometry")
        if not np.array_equal(np.sort(y),np.sort(ny)):
            raise RuntimeError("Vertical positioning changed")
        frames.append(dict(row=RIGHT_SWING_ROW,column=col,
                           shiftX=CONTACT_X,
                           oldMinX=int(x.min()),newMinX=int(nx.min()),
                           oldMaxX=int(x.max()),newMaxX=int(nx.max()),
                           nontransparent=len(x)))
    if not np.array_equal(rgba[:2*CELL],result[:2*CELL]) or not np.array_equal(
        rgba[3*CELL:],result[3*CELL:]):
        raise RuntimeError("Left direction or right recoil unexpectedly changed")
    for col in (4,5):
        x0=col*CELL
        if not np.array_equal(rgba[2*CELL:3*CELL,x0:x0+CELL],
                              result[2*CELL:3*CELL,x0:x0+CELL]):
            raise RuntimeError("Transparent right swing tail altered")
    dest.mkdir(parents=True)
    target=dest/"Candidate-action-feedback-hand-aligned.png"
    Image.fromarray(result,"RGBA").save(target)
    preview(rgba,result).save(dest/"right-swing-before-after-4x.png")
    resulthash=hashlib.sha256(target.read_bytes()).hexdigest()
    evidence=dict(schema="doeon-action-feedback-right-hand-contact-v2",
                  status="visual-candidate-not-final-accepted",
                  originalAtlasSha256=hashlib.sha256(raw).hexdigest(),
                  correctedAtlasSha256=resulthash,
                  correctedRightSwingFrames=frames,
                  originalRowsUnchanged=["left-swing","left-hurt","right-hurt"],
                  candidateOnly=True,noGameplayChanges=True,sourceCharacterArtUnmodified=True,
                  rightHandPoseDistanceBeforePx=4.39,
                  rightHandPoseDistanceAfterApproxPx=2.0,
                  remainingConcern="Fading frames may be farther from the recovering hand; check real moving visual timing and 1x scale before accepting artwork.")
    (dest/"manifest.json").write_text(json.dumps(evidence,sort_keys=True,indent=2)+"\n",encoding="utf8")
    print("RIGHT_SWING_CONTACT",len(frames),"frames, x",CONTACT_X,
          "RGBA_SHA256",resulthash)
    print("SOURCE_SHA256",evidence["originalAtlasSha256"])

if __name__=="__main__":
    p=argparse.ArgumentParser()
    p.add_argument("--input",type=Path,required=True)
    p.add_argument("--output",type=Path,required=True)
    v=p.parse_args()
    refine(v.input,v.output)