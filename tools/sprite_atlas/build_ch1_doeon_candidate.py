#!/usr/bin/env python3
"""Build a bounded, source-hash-provenanced right-facing Doeon candidate atlas.

Never alters the original 1536x1024 concept sources. No accepted left-facing
source exists, therefore no mirroring is performed. Not combat hitbox authority.
"""
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path

import cv2
import numpy as np
from PIL import Image, ImageDraw

ROWS = 4
COLS = 6
CELL = 64
PIVOT_TOP_LEFT = [32, 52]
PPU = 18
SPEC = (
    dict(name="movement", source="09-doeon-movement.png",
         y_edges=[0, 284, 533, 780, 1024],
         animations=["idle_walk", "run", "jump_land", "dash_hit"]),
    dict(name="combat", source="10-doeon-combat.png",
         y_edges=[0, 256, 512, 768, 1024],
         animations=["basic_attack", "weight_transfer", "compression_verdict", "shared_plane"]),
)
SCALE = 0.135  # one scale across *both* source sheets, not a per-frame shrink


def mask_source(src: Image.Image):
    rgb=np.asarray(src.convert("RGB"),dtype=np.uint8)
    r=rgb[:,:,0].astype(np.int16)
    g=rgb[:,:,1].astype(np.int16)
    b=rgb[:,:,2].astype(np.int16)
    # Generated raw source contains near-magenta in a large chroma band.
    # Matte chroma only, preserve dark fabric, brass and skin colors.
    magenta=(r>=130)&(b>=145)&(g<=105)&((r-g)>=80)&((b-g)>=85)
    rgba=np.zeros((rgb.shape[0],rgb.shape[1],4),dtype=np.uint8)
    rgba[:,:,:3]=rgb
    rgba[:,:,3]=np.where(magenta,0,255).astype(np.uint8)
    rgba[magenta,:3]=0
    return rgba


def primary_person_frame(crop:np.ndarray):
    mask=np.uint8(crop[:,:,3]>=128)
    count,labels,stats,centroids=cv2.connectedComponentsWithStats(mask,8)
    if count<2:
        return crop,{"error":"no-opaque-component"}
    n=max(range(1,count),key=lambda i:int(stats[i,cv2.CC_STAT_AREA]))
    person=stats[n,:]
    primary_count=int(person[cv2.CC_STAT_AREA])
    kept=np.zeros_like(mask)
    kept[labels==n]=255
    removed=0
    # Separate distant source-generation specks / motion-line VFX from actor.
    # Connected accessory details very near the main silhouette are kept.
    x0,y0,w,h=[int(v) for v in person[:4]]
    for c in range(1,count):
        if c==n:continue
        x,y,cw,ch,area=[int(v) for v in stats[c,:5]]
        dx=max(x0-(x+cw),x-(x0+w),0)
        dy=max(y0-(y+ch),y-(y0+h),0)
        near=dx<=5 and dy<=5
        if near and area>=10:
            kept[labels==c]=255
        else:
            removed+=area
    transparent=crop.copy()
    transparent[kept==0,:]=0
    yy,xx=np.where(kept>0)
    if len(xx)==0:
        return transparent,{"error":"empty-after-clean"}
    box=[int(xx.min()),int(yy.min()),int(xx.max()+1),int(yy.max()+1)]
    edgeOpaque=(int(np.count_nonzero(kept[0:2,:]))+int(np.count_nonzero(kept[-2:,:]))+
                int(np.count_nonzero(kept[:,0:2]))+int(np.count_nonzero(kept[:,-2:])))
    return transparent,dict(box=box,primaryPixels=primary_count,removedSpeckPixels=removed,
                            edgeOpaque=edgeOpaque,componentCount=count-1)


def build(source_root:Path, output:Path):
    if output.exists():
        raise SystemExit(f"Output exists; refusing overwrite: {output}")
    source_root=source_root.resolve();output=output.resolve()
    if output==source_root or source_root in output.parents:
        raise SystemExit("Output must not lie inside source archive")
    # Verify all inputs are readable *before* output creation.
    imgs={}
    for cfg in SPEC:
        path=source_root/cfg["source"]
        if not path.is_file():raise SystemExit(f"Missing source {path}")
        src=Image.open(path);src.load()
        if src.size!=(1536,1024) or src.mode not in ("RGB","RGBA"):
            raise SystemExit(f"Unexpected canvas/mode for {path}: {src.size}/{src.mode}")
        imgs[cfg["name"]]=(path,src.copy(),hashlib.sha256(path.read_bytes()).hexdigest())
    output.mkdir(parents=True)
    records=[]
    for cfg in SPEC:
        path,img,source_sha=imgs[cfg["name"]]
        rgba=mask_source(img)
        atlas=Image.new("RGBA",(CELL*COLS,CELL*ROWS),(0,0,0,0))
        frames=[]
        problems=[]
        for row in range(ROWS):
            for col in range(COLS):
                x0=col*256;y0=cfg["y_edges"][row]
                crop=rgba[y0:cfg["y_edges"][row+1],x0:(col+1)*256,:]
                cropped,details=primary_person_frame(crop)
                if "error" in details:
                    problems.append(dict(row=row,column=col,**details));continue
                bx0,by0,bx1,by1=details["box"]
                piece=Image.fromarray(cropped[by0:by1,bx0:bx1,:],"RGBA")
                dw=max(1,round(piece.width*SCALE));dh=max(1,round(piece.height*SCALE))
                if dw>CELL or dh>PIVOT_TOP_LEFT[1]:
                    problems.append(dict(row=row,column=col,reason="cell_overflow",size=[dw,dh]))
                    continue
                small=piece.resize((dw,dh),Image.Resampling.NEAREST)
                dx=(CELL-dw)//2;dy=PIVOT_TOP_LEFT[1]-dh
                atlas.alpha_composite(small,(col*CELL+dx,row*CELL+dy))
                frames.append(dict(row=row,column=col,clip=cfg["animations"][row],
                                   rectTopLeft=[col*CELL,row*CELL,CELL,CELL],
                                   baselineTopLeft=PIVOT_TOP_LEFT[1],pivotTopLeft=PIVOT_TOP_LEFT,
                                   sourceRect=[x0,y0,256,crop.shape[0]],sourceOpaqueBounds=details["box"],
                                   scaledSize=[dw,dh],removedSpeckPixels=details["removedSpeckPixels"],
                                   sourceBorderOpaque=details["edgeOpaque"]))
        if problems or len(frames)!=24:
            raise RuntimeError(f"Cannot publish {cfg['name']}: issues={problems}")
        target=output/(cfg["name"]+".png")
        atlas.save(target)
        with Image.open(target) as decoded:
            if decoded.size!=(384,256) or decoded.mode!="RGBA":
                raise RuntimeError(f"Unexpected atlas {target}")
        checksum=hashlib.sha256(target.read_bytes()).hexdigest()
        records.append(dict(kind=cfg["name"],source=cfg["source"],sourceSha256=source_sha,
                            sourceRowEdges=cfg["y_edges"],sourceXEdges=[i*256 for i in range(COLS+1)],
                            atlas=target.name,atlasSha256=checksum,cellSize=[CELL,CELL],
                            spritePivotNormalized=[.5,(CELL-PIVOT_TOP_LEFT[1])/CELL],pixelsPerUnit=PPU,
                            scale=SCALE,columns=COLS,rows=ROWS,
                            rowIds=cfg["animations"],frames=frames))
        # Full sheet checkerboard preview (nearest neighbor only).
        big=atlas.resize((1536,1024),Image.Resampling.NEAREST)
        checker=Image.new("RGBA",big.size,(52,61,67,255))
        draw=ImageDraw.Draw(checker)
        for cy in range(0,big.height,16):
            for cx in range(0,big.width,16):
                if (cx//16+cy//16)%2:
                    draw.rectangle((cx,cy,cx+15,cy+15),fill=(74,82,90,255))
        preview=Image.alpha_composite(checker,big)
        preview.save(output/(cfg["name"]+"-review-4x.png"))
    manifest=dict(schema="ch1-doeon-right-candidate-v1",status="candidate_pending_visual_and_gameplay_review",
                  originalArchiveUnchanged=True,notFinalLeftFacing=True,explicitNoMirror=True,
                  rightSideOnly=True,mechanicsUnchanged=True,sourceRoot=str(source_root),
                  leftAnimation="existing individually authored static Left sprite fallback",
                  tools="Python/Pillow/OpenCV; deterministic nearest-neighbor, source chroma key",
                  issue="Raw keyposes are concept material with hand variation and art overhang; review each frame. No new motion/hitbox authority.",
                  assets=records)
    (output/"manifest.json").write_text(json.dumps(manifest,ensure_ascii=False,sort_keys=True,indent=2)+"\n",encoding="utf8")
    print("SOURCE_SHA",*(f"{r['kind']}={r['sourceSha256']}" for r in records))
    print("RESULT",output,"ASSETS",len(records),"FRAMES",sum(len(r["frames"]) for r in records))
    print("BORDER_TOUCH",*(f"{r['kind']}={sum(f['sourceBorderOpaque']>0 for f in r['frames'])}" for r in records))


def main():
    ap=argparse.ArgumentParser()
    ap.add_argument("--source-root",type=Path,required=True)
    ap.add_argument("--output",type=Path,required=True)
    args=ap.parse_args()
    build(args.source_root,args.output)

if __name__=="__main__":
    main()