#!/usr/bin/env python3
"""Experimental layered joint animation from the *authored left-facing* Doeon.

This is not a mirrored right sheet and not an accepted hand-drawn animation.
Keeps original art byte-for-byte, uses explicit limb masks, joint rotation
and a verified common foot baseline. Output stays local/isolated for review.
"""
from __future__ import annotations
import argparse
import hashlib
import json
import math
from pathlib import Path
import cv2
import numpy as np
from PIL import Image, ImageDraw

FRAME=256
CELL=64
ROWS=4
COLS=6
PER_ROW=("idle","walk","run","jump")
RAW_FOOT_Y=208
PIVOT_Y=52
# Manual landmarks from independent left-facing authored source preview.
LEFT_SHOULDER=(109,99)
RIGHT_SHOULDER=(147,97)
LEFT_HIP=(119,154)
RIGHT_HIP=(140,154)
LEFT_KNEE=(116,179)
RIGHT_KNEE=(144,179)
CENTER_HIP=(131,151)

def mask_polygon(points):
    m=Image.new("L",(FRAME,FRAME),0)
    ImageDraw.Draw(m).polygon(points,fill=255)
    return np.asarray(m)>0

def prepare_master(path:Path):
    original=path.read_bytes()
    im=Image.open(path).convert("RGBA")
    arr=np.asarray(im)
    ys,xs=np.nonzero(arr[:,:,3]>128)
    if len(xs)<30000:
        raise ValueError("Source left master too sparse or wrong canvas")
    bounds=(int(xs.min()),int(ys.min()),int(xs.max()+1),int(ys.max()+1))
    # Existing left side master has its own gear/strap arrangement.
    body=im.crop(bounds)
    body=body.resize((72,160),Image.Resampling.LANCZOS)
    canvas=Image.new("RGBA",(FRAME,FRAME),(0,0,0,0))
    canvas.alpha_composite(body,(92,48))
    return np.array(canvas),original,bounds

def isolate(image,mask):
    part=image.copy()
    part[~mask]=0
    return part

def layers(master):
    alpha=master[:,:,3]>12
    left_arm=mask_polygon([
       (102,90),(111,91),(120,104),(118,117),(115,131),
       (111,143),(107,155),(94,158),(94,138),(103,119)])
    right_arm=mask_polygon([
       (136,90),(151,90),(164,104),(164,130),(155,151),
       (139,156),(137,142),(144,121),(141,107)])
    pelvis=mask_polygon([(112,141),(150,141),(151,160),(109,160)])
    # Stable centerline split keeps the two independently colored boots separate.
    left_leg=np.zeros((FRAME,FRAME),dtype=bool)
    right_leg=np.zeros_like(left_leg)
    for y in range(151,FRAME):
        split=int(129+max(0,y-175)*.08)
        left_leg[y,:split]=True
        right_leg[y,split:]=True
    left_leg &= alpha & ~(left_arm|right_arm)
    right_leg &= alpha & ~(left_arm|right_arm)
    arm_l=left_arm&alpha;arm_r=right_arm&alpha
    body=alpha&~(arm_l|arm_r|left_leg|right_leg)
    # Pelvis overlaps the joint heads and covers seams when knees swing.
    layer_masks=dict(left_leg=left_leg,right_leg=right_leg,
                     left_arm=arm_l,right_arm=arm_r,
                     body=body,pelvis=pelvis&alpha)
    return {k:isolate(master,v) for k,v in layer_masks.items()},layer_masks

def affine_rotate(pixels,pivot,angle,shift=(0,0)):
    matrix=cv2.getRotationMatrix2D(pivot,float(angle),1.0)
    matrix[0,2]+=shift[0];matrix[1,2]+=shift[1]
    return cv2.warpAffine(pixels,matrix,(FRAME,FRAME),
                          flags=cv2.INTER_NEAREST,
                          borderMode=cv2.BORDER_CONSTANT,borderValue=(0,0,0,0))

def composite(out,layer):
    # PIL's over handles alpha and preserves original palette in each part.
    Image.Image.alpha_composite(out,Image.fromarray(layer,'RGBA'))

def angles(mode,idx):
    # Six distinct, loopable gait keys. Never use identical forward phases.
    cycles=(-.5,.2,.9,.5,-.3,-.9)
    sine=cycles[idx]
    if mode=="idle":
        return dict(left_leg=0,right_leg=0,left_arm=2*sine,
                    right_arm=-2*sine,body=0,bob=0,lk=0,rk=0,shift=0)
    if mode=="walk":
        return dict(left_leg=19*sine,right_leg=-19*sine,
                    left_arm=-19*sine,right_arm=16*sine,
                    body=-2,bob=-(idx%2)*1,lk=8*max(0,sine),
                    rk=8*max(0,-sine),shift=0)
    if mode=="run":
        return dict(left_leg=32*sine,right_leg=-32*sine,
                    left_arm=-30*sine,right_arm=25*sine,
                    body=-6,bob=-(idx%2)*3,lk=19*max(0,sine),
                    rk=19*max(0,-sine),shift=-2)
    # Jump frames represent takeoff/rising/peak/falling/landing, not
    # a constant walking loop. No attack or damage authority.
    leg=(-25,-31,-36,-24,-12,0)[idx]
    return dict(left_leg=leg,right_leg=-leg*.7,
                left_arm=(8,25,36,25,9,0)[idx],
                right_arm=(-4,-13,-18,-12,-5,0)[idx],
                body=(-2,-5,-7,-5,-2,0)[idx],
                bob=(0,-5,-8,-6,-3,0)[idx],
                lk=(5,15,23,14,6,0)[idx],
                rk=(12,19,16,12,5,0)[idx],shift=0)

def pose(base_layers,mode,idx):
    a=angles(mode,idx)
    shift=(a['shift'],a['bob'])
    out=Image.new("RGBA",(FRAME,FRAME),(0,0,0,0))
    # Camera-side leg is drawn later; all parts originate in left-authored image.
    for name,hip,knee,knee_key in (
        ("right_leg",RIGHT_HIP,RIGHT_KNEE,"rk"),
        ("left_leg",LEFT_HIP,LEFT_KNEE,"lk")):
        leg=base_layers[name]
        # Subtle bend below knee, then full segment swing at hip.
        whole=affine_rotate(leg,knee,a[knee_key]/2)
        whole=affine_rotate(whole,hip,a[name],shift)
        composite(out,whole)
    # The torso remains visually coherent; a narrow pelvis overlay caps the
    # rotating hip origins. Hair and asymmetrical straps are not mirrored.
    torso=affine_rotate(base_layers["body"],CENTER_HIP,a["body"],shift)
    composite(out,torso)
    for name,pivot in (("right_arm",RIGHT_SHOULDER),("left_arm",LEFT_SHOULDER)):
        arm=affine_rotate(base_layers[name],pivot,a[name],shift)
        composite(out,arm)
    pelvis=affine_rotate(base_layers["pelvis"],CENTER_HIP,a["body"],shift)
    composite(out,pelvis)
    rgba=np.array(out)
    ys,xs=np.nonzero(rgba[:,:,3]>32)
    if len(ys):
        # A walk/run's one foot must meet the unchanged ground baseline.
        if mode in ("idle","walk","run"):
            delta=RAW_FOOT_Y-int(ys.max())
            if delta:
                trans=np.float32([[1,0,0],[0,1,delta]])
                rgba=cv2.warpAffine(rgba,trans,(FRAME,FRAME),
                                    flags=cv2.INTER_NEAREST,
                                    borderMode=cv2.BORDER_CONSTANT,
                                    borderValue=(0,0,0,0))
    return Image.fromarray(rgba,'RGBA')

def checker(im,scale=4):
    enlarged=im.resize((im.width*scale,im.height*scale),Image.Resampling.NEAREST)
    bg=Image.new('RGBA',enlarged.size,(52,59,66,255))
    d=ImageDraw.Draw(bg)
    for y in range(0,bg.height,16):
        for x in range(0,bg.width,16):
            if (x//16+y//16)%2:
                d.rectangle((x,y,x+15,y+15),fill=(73,81,90,255))
    return Image.alpha_composite(bg,enlarged)

def build(src,dest):
    if dest.exists():
        raise SystemExit("Refusing to overwrite experiment")
    master,data,bounds=prepare_master(src)
    parts,masks=layers(master)
    if int(sum(np.count_nonzero(v) for v in masks.values()))<2500:
        raise RuntimeError('Suspiciously empty layers')
    dest.mkdir(parents=True)
    # Color-coded source decomposition audit image.
    colored=np.zeros((FRAME,FRAME,4),dtype=np.uint8)
    colors={'body':[207,207,207],'pelvis':[255,155,45],
            'left_arm':[40,196,250],'right_arm':[245,82,180],
            'left_leg':[94,226,93],'right_leg':[243,217,53]}
    for key in ("body","left_leg","right_leg","left_arm","right_arm"):
        colored[masks[key],:3]=colors[key]
        colored[masks[key],3]=255
    Image.fromarray(colored).save(dest/'limb-mask-audit.png')
    contact=Image.new('RGBA',(CELL*COLS,CELL*ROWS),(0,0,0,0))
    evidence=[]
    for row,mode in enumerate(PER_ROW):
        for col in range(COLS):
            im=pose(parts,mode,col)
            # 4x pose downsample; edges remain crisp and canvas is deterministic.
            tile=im.resize((CELL,CELL),Image.Resampling.NEAREST)
            contact.alpha_composite(tile,(CELL*col,CELL*row))
            a=np.array(tile)[:,:,3]
            ys,xs=np.nonzero(a>64)
            if len(xs)==0:raise RuntimeError(f'Empty pose {mode}/{col}')
            bbox=[int(xs.min()),int(ys.min()),int(xs.max())+1,int(ys.max())+1]
            if bbox[0]<=1 or bbox[2]>=CELL-1 or bbox[1]<1 or bbox[3]>CELL:
                raise RuntimeError(f'Frame overflow {mode}/{col}: {bbox}')
            evidence.append(dict(mode=mode,frame=col,bbox=bbox,
                                 opaque=int(np.count_nonzero(a>64)),
                                 feetToFixedBaseline=52-bbox[3]))
    contact.save(dest/'left-joint-motion-EXPERIMENT.png')
    checker(contact).save(dest/'left-joint-motion-EXPERIMENT-4x.png')
    record=dict(schema='left-authored-art-joint-motion-experiment-v2',
                sourceSha256=hashlib.sha256(data).hexdigest(),
                source=str(src),sourceAlphaBounds=list(bounds),
                originalMirrored=False,leftArtOriginalUnchanged=True,
                atlasSha256=hashlib.sha256((dest/'left-joint-motion-EXPERIMENT.png').read_bytes()).hexdigest(),
                status='QUARANTINE_NOT_ACCEPTED_FOR_RUNTIME',modes=list(PER_ROW),
                cellSize=[CELL,CELL],ppu=18,frames=evidence,
                disclaimer='Cutout-rig experiment is not separately hand-drawn left action art, with no attack frame source. Review seams, boots, hip pixels and stage quality before any Unity use.')
    (dest/'manifest.json').write_text(json.dumps(record,sort_keys=True,ensure_ascii=False,indent=2)+'\n',encoding='utf8')
    print('EXPERIMENT',dest,'frames',len(evidence),'hash',record['atlasSha256'])

if __name__=='__main__':
    p=argparse.ArgumentParser()
    p.add_argument('--source',type=Path,required=True)
    p.add_argument('--output',type=Path,required=True)
    args=p.parse_args()
    build(args.source,args.output)