#!/usr/bin/env python3
"""Read-only, non-accepting audit for previously generated magenta-key sprite sources.

Creates only OUTPUT diagnostic previews and a source SHA manifest. Never writes
the original archive or Assets/, never calls Unity and never approves sprites.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

import numpy as np
from PIL import Image

SCHEMA = "chapter1-source-keypose-preflight-v1"
DEFAULT_SOURCES = (
    "06-ain-yudam-idle", "07-village-roles",
    "09-doeon-movement", "10-doeon-combat",
    "11-seryeong-movement-combat", "12-seoha-boss", "13-ordan-combat"
)
BOSS_IDS = frozenset(("12-seoha-boss", "13-ordan-combat", "14-taegan-combat", "15-chaeryun-combat", "16-mujin-combat", "17-ragen-combat", "18-boss-creatures"))
MIXED_IDS = frozenset(("04-lovers-interactions", "05-lovers-shop", "06-ain-yudam-idle", "07-village-roles", "18-boss-creatures"))
COLS = 6
WIDTH, HEIGHT = 1536, 1024

def transparent_checker(image: Image.Image, scale: int = 4) -> Image.Image:
    expanded = image.resize((image.width*scale, image.height*scale), Image.Resampling.NEAREST)
    arr = np.empty((expanded.height, expanded.width, 3), dtype=np.uint8)
    y, x = np.indices((expanded.height, expanded.width))
    tone = np.where(((x//16 + y//16) % 2) == 0, 52, 70)
    arr[:,:,:] = tone[:,:,None]
    bg = Image.fromarray(arr, "RGB").convert("RGBA")
    return Image.alpha_composite(bg, expanded)

def source_image(src: Path):
    data = src.read_bytes()
    im = Image.open(src)
    im.load()
    if im.size != (WIDTH, HEIGHT):
        raise ValueError(f"{src.name}: wrong size {im.size}; expected {WIDTH}x{HEIGHT}")
    if im.mode not in ("RGB", "RGBA"):
        raise ValueError(f"{src.name}: unsupported mode {im.mode}")
    rgb = np.asarray(im.convert("RGB"), dtype=np.uint8)
    # The generated "pure magenta" background was in fact compressed/speckled.
    # A hard-coded exact 255,0,255 comparison is demonstrably insufficient.
    r, g, b = (rgb[:,:,i].astype(np.int16) for i in range(3))
    chroma = (r>=165)&(b>=165)&(g<=95)&(r-g>=108)&(b-g>=108)
    rgba = np.empty((HEIGHT, WIDTH, 4), dtype=np.uint8)
    rgba[:,:,:3] = rgb
    rgba[:,:,3] = np.where(chroma, 0, 255).astype(np.uint8)
    rgba[chroma,:3] = 0
    return data, rgba, chroma

def crop_bbox(a: np.ndarray):
    yy,xx = np.nonzero(a[:,:,3] >= 128)
    if len(xx) == 0:
        return None
    return [int(xx.min()), int(yy.min()), int(xx.max())+1, int(yy.max())+1]

def analysis(name: str, data: bytes, rgba: np.ndarray, chroma: np.ndarray, dest: Path, row_count: int):
    foreground = ~chroma
    cell_w = WIDTH//COLS
    yedges = [(HEIGHT*i)//row_count for i in range(row_count+1)]
    row_projection = np.count_nonzero(foreground, axis=1)
    col_projection = np.count_nonzero(foreground, axis=0)
    row_candidates=[]
    for ideal in yedges[1:-1]:
        ys=np.arange(max(0,ideal-72), min(HEIGHT,ideal+73))
        best=int(ys[int(np.argmin(row_projection[ys]))])
        row_candidates.append({"ideal":ideal,"minimumAt":best,
                               "foregroundCountAtIdeal":int(row_projection[ideal]),
                               "foregroundCountAtMin":int(row_projection[best])})
    frame_records=[];crops=[];bbox_heights=[]
    for row in range(row_count):
        y0, y1 = yedges[row], yedges[row+1]
        for col in range(COLS):
            crop=rgba[y0:y1, col*cell_w:(col+1)*cell_w]
            box=crop_bbox(crop)
            if box:
                if row==0: bbox_heights.append(box[3]-box[1])
                edge = sum(int(np.count_nonzero(crop[:,x,3])) for x in (0,1,cell_w-2,cell_w-1))
                edge += sum(int(np.count_nonzero(crop[y,:,3])) for y in (0,1,crop.shape[0]-2,crop.shape[0]-1))
            else:
                edge=0
            frame_records.append({"row":row,"column":col,"sourceRect":[col*cell_w,y0,cell_w,y1-y0],
                                  "foregroundBBoxTopLeftXYXY":box,"edgeOpaquePixelCount":edge})
            crops.append((crop,box))
    large = name in BOSS_IDS
    cell_size=(192,128) if name=="18-boss-creatures" else (
        (128,64) if name=="05-lovers-shop" else (
        (96,64) if name=="04-lovers-interactions" else (
        (128,128) if large else (64,64))))
    baseline=104 if cell_size[1]==128 else 52
    body_height=88 if name=="18-boss-creatures" else (64 if cell_size[1]==128 else 32)
    median_height=float(np.median(bbox_heights)) if bbox_heights else 0
    if median_height<=0:
        raise ValueError(f"{name}: all top row cells are empty")
    uniform_scale=body_height / median_height
    atlas=Image.new("RGBA",(cell_size[0]*COLS,cell_size[1]*row_count),(0,0,0,0))
    overflows=[];nonempty=0
    for i,(crop,box) in enumerate(crops):
        if box is None:continue
        nonempty+=1
        col=i%COLS;row=i//COLS
        art=Image.fromarray(crop[box[1]:box[3],box[0]:box[2]],"RGBA")
        dst_w=max(1,int(round(art.width*uniform_scale)))
        dst_h=max(1,int(round(art.height*uniform_scale)))
        # No per-frame rescale: one uniform scale for the entire source.
        if dst_w>cell_size[0] or dst_h>baseline:
            overflows.append({"row":row,"column":col,"dstSize":[dst_w,dst_h]})
            continue
        art=art.resize((dst_w,dst_h),Image.Resampling.NEAREST)
        dx=col*cell_size[0]+(cell_size[0]-dst_w)//2
        dy=row*cell_size[1]+baseline-dst_h
        atlas.alpha_composite(art,(dx,dy))
    # Diagnostic only: hard matte leaves some colored edge spill, source poses
    # may be misregistered, and row boundaries are unreviewed.
    dest.mkdir(parents=True,exist_ok=False)
    Image.fromarray(rgba,"RGBA").save(dest/"source-hardmatte.png")
    atlas.save(dest/"uniform-scale-preview-atlas.png")
    transparent_checker(atlas).save(dest/"checker-preview-4x.png")
    diagnostics={
        "schema":SCHEMA,
        "sourceSha256":hashlib.sha256(data).hexdigest(),
        "sourceName":name,
        "canvas":[WIDTH,HEIGHT],
        "rowCount":row_count,"columnCount":COLS,"reviewedYEdges":None,"defaultYEdges":yedges,
        "chromaDetectedFraction":round(float(chroma.mean()),6),
        "nonemptyCells":nonempty,
        "emptyCells":COLS*row_count-nonempty,
        "edgeTouchCells":sum(fr["edgeOpaquePixelCount"]>0 for fr in frame_records),
        "rowBoundaryReview":row_candidates,
        "uniformPreviewScale":round(uniform_scale,6),
        "medianFirstRowBoundsHeight":median_height,
        "previewCell":cell_size,
        "previewBodyHeight":body_height,
        "previewBaselineTopLeft":baseline,
        "previewOverflow":overflows,
        "mixedActorRows":name in MIXED_IDS,
        "notAcceptance":"Hard-matte/nearest preview is NOT an approved keyed atlas, playable sprite or animation. Manual cell boundaries and pose/feet/halo review required.",
        "disposition":"source_only_needs_visual_review",
        "frames":frame_records
    }
    (dest/"preflight.json").write_text(json.dumps(diagnostics,ensure_ascii=False,sort_keys=True,indent=2)+"\n",encoding="utf8")
    return {key:diagnostics[key] for key in
            ("sourceName","sourceSha256","chromaDetectedFraction","nonemptyCells","edgeTouchCells",
             "previewOverflow","uniformPreviewScale","mixedActorRows","rowBoundaryReview","disposition")}

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument("--source-root",type=Path,required=True)
    ap.add_argument("--output-dir",type=Path,required=True)
    ap.add_argument("--sources",nargs="*",default=None)
    args=ap.parse_args()
    plan_file=args.source_root.parent/"generation-plan.json"
    plan=json.loads(plan_file.read_text("utf8"))
    jobs={job["id"]:job for job in plan["jobs"]}
    requested=args.sources if args.sources is not None else sorted(jobs)
    if args.output_dir.exists():raise SystemExit("Refusing to overwrite output directory")
    # Refuse any attempt to use the original archive as destination.
    source=args.source_root.resolve();output=args.output_dir.resolve()
    if output==source or source in output.parents:
        raise SystemExit("Refusing output inside original source tree")
    args.output_dir.mkdir(parents=True)
    records=[]
    for key in requested:
        if key not in jobs:raise SystemExit(f"Unknown job id {key}")
        row_count=jobs[key]["rowCount"]
        if row_count not in (3,4):raise SystemExit(f"Unsupported rows {key}={row_count}")
        path=args.source_root/f"{key}-v2.png"
        if not path.exists():path=args.source_root/f"{key}.png"
        if not path.is_file():raise SystemExit(f"Missing source for {key}")
        data,rgba,chroma=source_image(path)
        rec=analysis(key,data,rgba,chroma,args.output_dir/key,row_count)
        rec["actualSourcePath"]=str(path)
        records.append(rec)
        print(f"{key}: cells={rec['nonemptyCells']}/{COLS*row_count} chroma={rec['chromaDetectedFraction']:.1%}"
              f" edgeTouch={rec['edgeTouchCells']} overflows={len(rec['previewOverflow'])}")
    manifest={"schema":SCHEMA,"sourceRoot":str(source),"status":"source_only_diagnostic",
              "originalsReadOnly":True,"unityAssetsModified":False,"records":records}
    (args.output_dir/"manifest.json").write_text(
        json.dumps(manifest,ensure_ascii=False,sort_keys=True,indent=2)+"\n",encoding="utf8")
    print(args.output_dir/"manifest.json")

if __name__=="__main__":
    main()