#!/usr/bin/env python3
"""Visual-only right-facing Doeon torso-root alignment.

Source art has inconsistent horizontal torso placement in WALK/RUN keyed
poses. Translate only those 12 RGBA tiles by integer <=2 pixels, preserving
pixel colors, foot height, pose animation, source originals and gameplay.
The keyed original candidate is immutable; this output remains unaccepted art.
"""
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw

CELL, COLS, ROWS = 64, 6, 4
ROW_SPECS = ((0, "walk", 31), (1, "run", 34))
BAND_Y = (30, 36)

def source_anchor(frame: np.ndarray) -> float:
    if frame.shape != (CELL, CELL, 4) or frame.dtype != np.uint8:
        raise ValueError("expected 64x64 RGBA uint8 frame")
    mask = frame[BAND_Y[0]:BAND_Y[1], :, 3] >= 128
    ys, xs = np.nonzero(mask)
    if len(xs) < 8:
        raise ValueError("torso anchor evidence insufficient")
    return float(np.median(xs))

def slide_frame(frame: np.ndarray, dx: int) -> np.ndarray:
    if frame.shape != (CELL, CELL, 4) or frame.dtype != np.uint8:
        raise ValueError("expected 64x64 RGBA uint8")
    if not isinstance(dx, int) or abs(dx) > 2:
        raise ValueError("unexpected torso offset: maximum allowed 2 pixels")
    out = np.zeros_like(frame)
    if dx >= 0:
        if dx and np.any(frame[:, CELL-dx:, 3]):
            raise ValueError("opaque pixels would be clipped on right edge")
        out[:, dx:] = frame[:, :CELL-dx]
    else:
        n = -dx
        if np.any(frame[:, :n, 3]):
            raise ValueError("opaque pixels would be clipped on left edge")
        out[:, :CELL-n] = frame[:, n:]
    # One-to-one reversible translation: never interpolates or erases art.
    if np.count_nonzero(out[:, :, 3]) != np.count_nonzero(frame[:, :, 3]):
        raise RuntimeError("source alpha pixels were lost")
    if not np.array_equal(np.sort(out[out[:, :, 3]>0], axis=0),
                          np.sort(frame[frame[:, :, 3]>0], axis=0)):
        raise RuntimeError("palette/alpha distribution changed")
    return out

def checker_pair(original: np.ndarray, aligned: np.ndarray) -> Image.Image:
    # Top panels are pre-alignment, bottom panels are after alignment.
    scale, pad = 4, 10
    width, height = COLS*(CELL*scale+pad), 4*(CELL*scale+40)
    canvas = Image.new("RGBA", (width, height), (28, 36, 43, 255))
    painter = ImageDraw.Draw(canvas)
    for r, title, buffer in ((0, "WALK before", original),
                             (1, "WALK after", aligned),
                             (2, "RUN before", original),
                             (3, "RUN after", aligned)):
        source_row = r//2
        panel_top = r*(CELL*scale+40)
        for col in range(COLS):
            x0=col*CELL
            panel = Image.new("RGBA",(CELL*scale,CELL*scale),(51,61,70,255))
            d=ImageDraw.Draw(panel)
            for y in range(0,CELL*scale,16):
                for x in range(0,CELL*scale,16):
                    if (x//16+y//16)%2:
                        d.rectangle((x,y,x+15,y+15),fill=(73,83,92,255))
            art=Image.fromarray(buffer[source_row*CELL:(source_row+1)*CELL,
                                          x0:(col+1)*CELL],"RGBA")
            panel.alpha_composite(art.resize(panel.size,Image.Resampling.NEAREST))
            dx=col*(CELL*scale+pad)
            canvas.alpha_composite(panel,(dx,panel_top+28))
            painter.line((dx+31*scale,panel_top+28,dx+31*scale,
                          panel_top+28+CELL*scale),fill=(242,198,90,150),width=1)
            painter.text((dx+4,panel_top+6),f"{title} {col}",fill=(240,237,225,255))
    return canvas

def build(src: Path, dest: Path, expected_sha: str | None = None):
    if dest.exists():
        raise SystemExit("Output already exists; cannot overwrite candidate")
    data=src.read_bytes()
    sha=hashlib.sha256(data).hexdigest().lower()
    if expected_sha and expected_sha.lower()!=sha:
        raise ValueError("Source hash differs from review-approved candidate")
    original=np.asarray(Image.open(src).convert("RGBA"))
    if original.shape!=(ROWS*CELL,COLS*CELL,4):
        raise ValueError("Invalid source sheet dimensions")
    result=original.copy()
    receipts=[]
    for row,label,target in ROW_SPECS:
        for col in range(COLS):
            y0=row*CELL;x0=col*CELL
            before=original[y0:y0+CELL,x0:x0+CELL]
            median=source_anchor(before)
            dx=int(round(target-median))
            if abs(dx)>2:
                raise ValueError(f"Frame {row}/{col} needs >2px adjustment")
            after=slide_frame(before,dx)
            aligned=source_anchor(after)
            if abs(aligned-target)>0.5:
                raise ValueError(f"Root remains offset in {label}/{col}")
            orig_y,orig_x=np.nonzero(before[:,:,3]>=128)
            dest_y,dest_x=np.nonzero(after[:,:,3]>=128)
            if len(orig_y)<50 or not np.array_equal(np.sort(orig_y),np.sort(dest_y)):
                raise ValueError("Grounding heights changed")
            if int(orig_y.max())!=51 or int(dest_y.max())!=51:
                raise ValueError("Feet ground baseline changed")
            result[y0:y0+CELL,x0:x0+CELL]=after
            receipts.append(dict(row=row,clip=label,column=col,
                                 beforeAnchorPx=median,shiftXpx=dx,
                                 afterAnchorPx=aligned,groundLastY=51,
                                 opaquePixelCount=int(np.count_nonzero(before[:,:,3]))))
    if not np.array_equal(result[2*CELL:],original[2*CELL:]):
        raise RuntimeError("Jump/dash/hurt rows must remain byte-for-byte unchanged")
    for row,_,target in ROW_SPECS:
        alphas=[source_anchor(result[row*CELL:(row+1)*CELL,col*CELL:(col+1)*CELL])
                for col in range(COLS)]
        if max(alphas)-min(alphas)>0.5:
            raise RuntimeError("Torso root still unstable after correction")
    dest.mkdir(parents=True)
    target=dest/"Candidate-right-movement-stabilized.png"
    Image.fromarray(result,"RGBA").save(target)
    checker_pair(original,result).save(dest/"right-walk-run-before-after-4x.png")
    record=dict(schema="ch1-doeon-right-torso-visual-alignment-v1",
                sourceSha256=sha,
                outputSha256=hashlib.sha256(target.read_bytes()).hexdigest(),
                sourceAsset=str(src),candidateOnly=True,
                originalArchiveNotModified=True,mechanicsUnchanged=True,
                correctedRows=["walk","run"],
                unchangedRows=["jump_land","dash_hit"],
                pixelRangeAllowed=[-2,2],baselineUnchanged=True,
                torsoBandY=list(BAND_Y),frameReceipts=receipts,
                warning="Manual graphics acceptance still needed. Root alignment is a reversible horizontal keypose correction, not new source art.")
    (dest/"manifest.json").write_text(
        json.dumps(record,ensure_ascii=False,sort_keys=True,indent=2)+"\n",
        encoding="utf8")
    print("RIGHT_TORSO_STABILIZED frames",len(receipts),
          "shifts",[(v["clip"],v["column"],v["shiftXpx"]) for v in receipts])
    print("BEFORE_SHA",sha,"AFTER_SHA",record["outputSha256"])

if __name__=="__main__":
    parser=argparse.ArgumentParser()
    parser.add_argument("--input",type=Path,required=True)
    parser.add_argument("--output",type=Path,required=True)
    parser.add_argument("--expected-sha")
    opts=parser.parse_args()
    build(opts.input,opts.output,opts.expected_sha)