#!/usr/bin/env python3
"""Conservative pixel-art cleanup, only for provisional Doeon LEFT attack/hurt.

The independently authored left master is never opened for writing; no
mirroring, gameplay changes, interpolation, or blanket morphological closing.
Only 1px detached alpha specks are removed and completely enclosed 1px
transparent torso/hip seam holes are filled. Other VFX/holes remain for manual
art review. This does not grant final visual approval.
"""
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path

import cv2
import numpy as np
from PIL import Image, ImageDraw

ROWS, COLS, CELL = 4, 6, 64
LIVE_ROWS = (0, 1)
SEAM_X = range(23, 44)
SEAM_Y = range(26, 47)

def clean_frame(frame: np.ndarray):
    if frame.shape != (CELL, CELL, 4) or frame.dtype != np.uint8:
        raise ValueError("Expected a single 64x64 RGBA frame")
    before=frame.copy()
    result=frame.copy()
    opaque=(before[:,:,3]>64).astype(np.uint8)
    n, labels, sizes, _=cv2.connectedComponentsWithStats(opaque, 8)
    removed=[]
    for component in range(1,n):
        if int(sizes[component,cv2.CC_STAT_AREA]) != 1:
            continue
        yy,xx=np.where(labels==component)
        y,x=int(yy[0]),int(xx[0])
        result[y,x]=0
        removed.append([x,y])
    filled=[]
    # Source snapshot drives the decisions. Do not cascade one filled pixel
    # into another seam and do not repaint open silhouette boundaries.
    alpha=before[:,:,3]
    for y in SEAM_Y:
        for x in SEAM_X:
            if alpha[y,x] != 0:
                continue
            coords=((y-1,x),(y+1,x),(y,x-1),(y,x+1))
            if any(int(alpha[yy,xx])<128 for yy,xx in coords):
                continue
            rgb=np.stack([before[yy,xx,:3] for yy,xx in coords])
            result[y,x,:3]=np.median(rgb,axis=0).astype(np.uint8)
            result[y,x,3]=255
            filled.append([x,y])
    return result,removed,filled

def checker(atlas:Image.Image):
    big=atlas.resize((atlas.width*4,atlas.height*4),Image.Resampling.NEAREST)
    bg=Image.new("RGBA",big.size,(51,59,68,255))
    d=ImageDraw.Draw(bg)
    for y in range(0,bg.height,16):
        for x in range(0,bg.width,16):
            if (x//16+y//16)%2:
                d.rectangle((x,y,x+15,y+15),fill=(73,82,90,255))
    return Image.alpha_composite(bg,big)

def main():
    arg=argparse.ArgumentParser()
    arg.add_argument("--input",required=True,type=Path)
    arg.add_argument("--output",required=True,type=Path)
    opts=arg.parse_args()
    if opts.output.exists():
        raise SystemExit("Refuse to overwrite a prior QA candidate")
    if opts.input.resolve().parent == opts.output.resolve():
        raise SystemExit("Refuse to use the original source as an output")
    image=Image.open(opts.input).convert("RGBA")
    if image.size != (COLS*CELL,ROWS*CELL):
        raise ValueError(f"Unexpected candidate atlas dimension {image.size}")
    before=np.asarray(image)
    after=before.copy()
    stats=[]
    for row in LIVE_ROWS:
        for col in range(COLS):
            y0,x0=row*CELL,col*CELL
            crop=before[y0:y0+CELL,x0:x0+CELL]
            repaired,removed,filled=clean_frame(crop)
            after[y0:y0+CELL,x0:x0+CELL]=repaired
            stats.append({"row":row,"column":col,
                          "removedDetachedSinglePixels":removed,
                          "filledEnclosedTorsoPixels":filled})
    if not np.array_equal(before[128:],after[128:]):
        raise RuntimeError("Unbound research rows must remain byte-for-byte unchanged")
    if not np.array_equal(before[:128,:,:3][
        (before[:128,:,3]>0)&(after[:128,:,3]>0)
    ],after[:128,:,:3][
        (before[:128,:,3]>0)&(after[:128,:,3]>0)
    ]):
        raise RuntimeError("Existing opaque RGB colors changed unexpectedly")
    changed=int(np.count_nonzero(np.any(before!=after,axis=2)))
    removed=sum(len(s["removedDetachedSinglePixels"]) for s in stats)
    filled=sum(len(s["filledEnclosedTorsoPixels"]) for s in stats)
    if changed!=removed+filled or changed>35:
        raise RuntimeError(f"Excessive edit count: changed={changed}, removed={removed}, filled={filled}")
    opts.output.mkdir(parents=True)
    target=opts.output/"Candidate-left-combat-cleaned.png"
    atlas=Image.fromarray(after,"RGBA")
    atlas.save(target)
    checker(atlas).save(opts.output/"Candidate-left-combat-cleaned-review-4x.png")
    record={"schema":"ch1-doeon-left-pixel-seam-cleanup-v1",
            "disposition":"visual-candidate-not-accepted",
            "inputSha256":hashlib.sha256(opts.input.read_bytes()).hexdigest(),
            "outputSha256":hashlib.sha256(target.read_bytes()).hexdigest(),
            "unchangedUnboundRows":True,
            "opaqueRgbColorsUnchanged":True,
            "sourceMasterUnmodified":True,
            "pixelChanges":changed,"isolatedPixelsRemoved":removed,
            "torsoEnclosedPixelsFilled":filled,"frames":stats,
            "warning":"Only reversible pixel cleanup of procedural left combat candidate; manual 1x/4x pose review and authored left art remain incomplete."}
    (opts.output/"manifest.json").write_text(
        json.dumps(record,ensure_ascii=False,sort_keys=True,indent=2)+"\n",
        encoding="utf8")
    print("CH1_DOEON_PIXEL_CLEANUP",record["pixelChanges"],
          "removed",removed,"filled",filled,"SHA256",record["outputSha256"])

if __name__=="__main__":
    main()