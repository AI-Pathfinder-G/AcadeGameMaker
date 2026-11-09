#!/usr/bin/env python3
"""An honest left-facing *motion experiment* from independently authored left art.

No mirroring, no invented left combat key poses, no Unity import. Not a complete
animation atlas. Only diagnostic movement contour variants for art review.
"""
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw

CELL=64
WIDTH=384
HEIGHT=256
PPU=18
FRAME_COUNT=6
BOOT_Y=43
HIP_Y=34
SHOULDER_Y=17
BASELINE=52

def original_to_cell(source:Image.Image):
    img=source.convert('RGBA')
    pixels=np.array(img)
    yy,xx=np.where(pixels[:,:,3]>128)
    if len(xx)==0:raise ValueError('Left sprite is transparent')
    bbox=(int(xx.min()),int(yy.min()),int(xx.max()+1),int(yy.max()+1))
    art=img.crop(bbox)
    width=int(round(art.width*32/art.height))
    scaled=art.resize((max(1,width),32),Image.Resampling.NEAREST)
    out=Image.new('RGBA',(CELL,CELL),(0,0,0,0))
    x=(CELL-scaled.width)//2
    out.alpha_composite(scaled,(x,BASELINE-32))
    return np.asarray(out),bbox

def warp(base,mode,frame):
    phase=(-2,-1,0,1,2,1)[frame]
    torso_shift={'idle':0,'walk':1,'run':2,'jump':1}.get(mode,0)*phase//2
    bob={'idle':(0,0,-1,0,0,-1),
         'walk':(0,-1,0,-1,0,-1),
         'run':(-1,0,-2,0,-1,0),
         'jump':(2,0,-1,-2,0,2)}[mode][frame]
    stride={'idle':0,'walk':1,'run':2,'jump':1}[mode]
    result=np.zeros_like(base)
    for y in range(14,53):
        t=(y-SHOULDER_Y)/(BASELINE-SHOULDER_Y)
        dx=int(round(torso_shift*max(0,1-t)))
        if y>=HIP_Y:
            # Opposing boot shifts; no mirroring of asymmetric garments.
            inner=np.asarray(base[y])
            for x in range(CELL):
                if inner[x,3]==0:continue
                leg_dir=-1 if x<CELL//2 else 1
                stride_phase=leg_dir*phase*stride//2
                target_x=x+dx+stride_phase
                target_y=y+bob
                if 0<=target_x<CELL and 0<=target_y<CELL:
                    result[target_y,target_x]=inner[x]
        else:
            target_y=y+bob
            if not 0<=target_y<CELL:continue
            line=base[y]
            xfrom=max(0,-dx);xto=min(CELL,CELL-dx)
            result[target_y,xfrom+dx:xto+dx,:]=line[xfrom:xto,:]
    # Incomplete left motion expression remains an unapproved prototype.
    return Image.fromarray(result,'RGBA')

def render(input_path:Path,dest:Path):
    if dest.exists():raise SystemExit('Refusing overwrite of prototype output')
    raw=input_path.read_bytes();original=Image.open(input_path)
    base,bounds=original_to_cell(original)
    dest.mkdir(parents=True)
    sheet=Image.new('RGBA',(WIDTH,HEIGHT),(0,0,0,0))
    modes=['idle','walk','run','jump']
    for row,mode in enumerate(modes):
        for frame in range(FRAME_COUNT):
            part=warp(base,mode,frame)
            sheet.alpha_composite(part,(frame*CELL,row*CELL))
    sheet.save(dest/'left-movement-PROTOTYPE.png')
    preview=sheet.resize((WIDTH*4,HEIGHT*4),Image.Resampling.NEAREST)
    bg=Image.new('RGBA',preview.size,(50,58,67,255))
    d=ImageDraw.Draw(bg)
    for y in range(0,preview.height,16):
        for x in range(0,preview.width,16):
            if (x//16+y//16)%2:d.rectangle((x,y,x+15,y+15),fill=(73,82,91,255))
    Image.alpha_composite(bg,preview).save(dest/'left-movement-PROTOTYPE-4x.png')
    rec=dict(schema='ch1-doeon-left-reconstruction-experiment-v1',sourceSha256=hashlib.sha256(raw).hexdigest(),
             sourcePath=str(input_path),sourceBounds=bounds,
             resultingAtlasSha256=hashlib.sha256((dest/'left-movement-PROTOTYPE.png').read_bytes()).hexdigest(),
             direction='left',sourceType='authored-left-static',
             originalWasMirrored=False,containsHandAuthoredLeftMotion=False,
             facing='left-procedural-prototype-only',frameCount=24,cellSize=[64,64],PPU=PPU,
             modes=modes,disposition='REJECT_FOR_LIVE_USE_UNTIL_VISUAL_ACCEPTANCE',
             limitations=['Not original keyed motion','Legs repositioned algorithmically',
                          'No attack/hit left sprite authored','Do not publish as completed/accepted character art'])
    (dest/'prototype-manifest.json').write_text(json.dumps(rec,ensure_ascii=False,indent=2,sort_keys=True)+'\n',encoding='utf8')
    print('LEFT_PROTOTYPE',dest,'ATLAS_SHA',rec['resultingAtlasSha256'])
def main():
    p=argparse.ArgumentParser()
    p.add_argument('--source',type=Path,required=True)
    p.add_argument('--output',type=Path,required=True)
    args=p.parse_args()
    render(args.source,args.output)
if __name__=='__main__':main()