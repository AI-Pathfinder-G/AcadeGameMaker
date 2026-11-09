#!/usr/bin/env python3
"""Conservative pixel art edge-decontamination of a candidate atlas.

Removes obvious magenta source-backdrop fringes on transparent boundaries.
Does not touch authored original art, change animation/collider authority,
mirror sprites, or create a new missing facing.
"""
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path
import numpy as np
import cv2
from PIL import Image, ImageDraw

def refine(pixels):
    result=pixels.copy()
    a=pixels[:,:,3]>0
    r,g,b=[pixels[:,:,i].astype('int16') for i in range(3)]
    chroma=(r>=90)&(b>=100)&(r>g+45)&(b>g+45)&(np.abs(r-b)<90)&a
    empty=np.uint8(~a)
    near_empty=cv2.dilate(empty,np.ones((3,3),dtype='uint8'),iterations=1)>0
    suspected=chroma&near_empty
    kept=a&~chroma
    # Only fringe pixels whose non-purple neighbors can substitute
    # tonal edge pixels are repaired. Isolated true VFX are not faked.
    count=0;unresolved=0
    for y,x in np.argwhere(suspected):
        candidates=[]
        for dy in (-1,0,1):
            for dx in (-1,0,1):
                yy=int(y+dy);xx=int(x+dx)
                if yy<0 or xx<0 or yy>=pixels.shape[0] or xx>=pixels.shape[1]:
                    continue
                if kept[yy,xx]:
                    candidates.append((abs(dx)+abs(dy),pixels[yy,xx,:3]))
        if candidates:
            candidates.sort(key=lambda c:c[0])
            selected=candidates[0][1]
            result[y,x,:3]=selected
            count+=1
        else:
            # Keep the pixel unchanged rather than silently deleting
            # isolated effect or unique costume details.
            unresolved+=1
    return result,dict(opaque=int(a.sum()),chroma=int(chroma.sum()),
                       edgeChroma=int(suspected.sum()),
                       repaired=count,retainedForReview=unresolved)

def checker(image:Image.Image):
    s=4
    big=image.resize((image.width*s,image.height*s),Image.Resampling.NEAREST)
    bg=Image.new('RGBA',big.size,(52,60,68,255))
    d=ImageDraw.Draw(bg)
    for y in range(0,bg.height,16):
        for x in range(0,bg.width,16):
            if (x//16+y//16)%2:
                d.rectangle((x,y,x+15,y+15),fill=(73,82,90,255))
    return Image.alpha_composite(bg,big)

def main():
    a=argparse.ArgumentParser()
    a.add_argument('--candidate-root',required=True,type=Path)
    a.add_argument('--output',required=True,type=Path)
    args=a.parse_args()
    if args.output.exists():raise SystemExit('Refusing overwrite')
    args.output.mkdir(parents=True)
    summary=[]
    for name in ('movement','combat'):
        inp=args.candidate_root/(name+'.png')
        im=Image.open(inp).convert('RGBA')
        if im.size!=(384,256):raise SystemExit('Unexpected candidate dimensions')
        data=np.array(im)
        out,stats=refine(data)
        refined=Image.fromarray(out,'RGBA')
        target=args.output/(name+'.png')
        refined.save(target)
        checker(refined).save(args.output/(name+'-refined-4x.png'))
        stats['inputSha256']=hashlib.sha256(inp.read_bytes()).hexdigest()
        stats['outputSha256']=hashlib.sha256(target.read_bytes()).hexdigest()
        stats['name']=name
        summary.append(stats)
        print(name,stats)
    (args.output/'manifest.json').write_text(json.dumps({'schema':'doeon-right-edge-prototype-v1',
        'status':'pending-visual-review','sourceAtlasesUnmodified':True,
        'rights':'derived solely from previously authored local user project sprite sources',
        'results':summary},indent=2,sort_keys=True)+'\n')
if __name__=='__main__':main()