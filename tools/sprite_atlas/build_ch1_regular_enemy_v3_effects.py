#!/usr/bin/env python3
"""Original Walker pressure vent / Surveyor optical ranging accents.

Visual-only 4x8 64px RGBA overlay. Same 18PPU/foot anchor as V3 character:
rows Walker R/L and Surveyor R/L; keys match completed enemy behavior.
Blank keys for idle, movement, dead. VFX cannot hit, damage, or target.

Walker telegraph pressure-warning, execute piston burst, recover steam.
Surveyor telegraph optic triangulation, execute lens discharge, suppressed
optical glitch, cooldown reticle collapse. No false hit sparks on misses.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw

CELL,COLS,ROWS=64,8,4
AMBER=(250,172,57,235)
HOT=(255,232,157,255)
FADE=(179,118,68,155)
FUME=(164,178,175,145)
CYAN=(94,229,213,242)
PALE=(209,254,237,255)
DEEP=(60,143,151,175)
HURT=(235,110,110,190)
KEYS={0:(3,4,7),1:(3,4,7),2:(3,4,5,7),3:(3,4,5,7)}
TITLES=("walker-right","walker-left","surveyor-right","surveyor-left")
STATES=("idle","step-a","step-b","warning","active","suppressed","dead","recover")

class Paint:
    def __init__(self,side):
        self.side=side
        self.image=Image.new("RGBA",(64,64),(0,0,0,0))
        self.p=ImageDraw.Draw(self.image)
    def point(self,x,y,col):
        self.p.point((32+x*self.side,y),fill=col)
    def line(self,pts,col,w=1):
        self.p.line([(32+x*self.side,y) for x,y in pts],fill=col,width=w)
    def rect(self,x1,y1,x2,y2,col):
        xx=sorted((32+x1*self.side,32+x2*self.side))
        self.p.rectangle((xx[0],y1,xx[1],y2),fill=col)
    def diamond(self,x,y,size,col):
        self.p.polygon([(32+x*self.side,y-size),
                        (32+(x+size)*self.side,y),
                        (32+x*self.side,y+size),
                        (32+(x-size)*self.side,y)],fill=col)

def draw(row,key):
    if row not in range(ROWS) or key not in range(COLS):
        raise ValueError("Unknown enemy VFX key")
    side=1 if row%2==0 else -1
    z=Paint(side)
    if key not in KEYS[row]:return z.image
    if row<2:
        if key==3:
            # Only valid during published DashTelegraph.
            z.line([(-17,18),(-18,11),(-14,6)],FUME,2)
            z.line([(-14,13),(-11,8),(-10,5)],FADE,1)
            z.point(-16,5,AMBER)
            z.line([(-6,24),(-10,27)],AMBER,2)
            z.line([(6,21),(10,18)],HOT,2)
            z.diamond(18,30,3,AMBER)
            z.line([(21,26),(22,22)],HOT,1)
            z.point(23,34,FADE)
        elif key==4:
            # A bounded non-collider pressure burst, not an enemy hit.
            z.line([(18,18),(22,12)],HOT,2)
            z.line([(22,35),(27,41)],AMBER,2)
            z.line([(21,22),(25,20),(28,21)],HOT,2)
            z.line([(20,31),(25,34),(28,33)],FADE,2)
            z.diamond(26,27,3,HOT)
            z.point(28,17,AMBER)
            z.line([(12,39),(16,42)],FUME,1)
        elif key==7:
            z.line([(-16,18),(-18,13),(-15,9)],FUME,1)
            z.line([(-12,16),(-10,13)],FADE,1)
            z.point(-17,8,FADE)
            z.point(6,24,AMBER)
    else:
        if key==3:
            # Suspended open triangle and prismatic range solution;
            # graphics do not extend to a target collider.
            z.line([(-6,8),(16,8),(27,22),(-6,8)],CYAN,1)
            z.line([(16,8),(24,34),(-6,8)],DEEP,1)
            z.diamond(16,18,4,PALE)
            z.line([(21,29),(27,29)],CYAN,1)
            z.point(27,22,PALE)
            z.point(-6,8,PALE)
        elif key==4:
            # A short muzzle glint aligned to published Fire phase.
            z.line([(19,29),(24,27),(30,27)],PALE,2)
            z.line([(20,31),(24,35),(30,36)],CYAN,1)
            z.diamond(26,29,4,PALE)
            z.point(30,23,CYAN)
            z.point(25,20,DEEP)
            z.line([(16,15),(19,12)],CYAN,1)
        elif key==5:
            # Suppressed is optical instability, not fabricated HP damage.
            z.line([(6,16),(11,13)],HURT,2)
            z.line([(14,22),(19,20)],DEEP,1)
            z.point(13,11,CYAN)
            z.line([(-4,26),(0,29)],DEEP,1)
        elif key==7:
            # Cooldown/muting reticle shrinks, with no shot fired.
            z.line([(9,11),(16,15)],DEEP,1)
            z.line([(16,24),(21,24)],DEEP,1)
            z.point(18,30,CYAN)
            z.point(-7,9,DEEP)
    return z.image

def preview(atlas):
    scale=4
    canvas=Image.new("RGBA",(COLS*CELL*scale,ROWS*(CELL*scale+26)),
                     (30,39,49,255))
    d=ImageDraw.Draw(canvas)
    for row in range(ROWS):
        for key in range(COLS):
            x=key*CELL*scale;y=row*(CELL*scale+26)
            crop=atlas.crop((key*CELL,row*CELL,(key+1)*CELL,(row+1)*CELL))
            bg=Image.new("RGBA",(CELL*scale,CELL*scale),(52,60,70,255))
            bg.alpha_composite(crop.resize(bg.size,Image.Resampling.NEAREST))
            canvas.alpha_composite(bg,(x,y+25))
            d.text((x+5,y+6),f"{TITLES[row]} {STATES[key]}",fill=(238,227,202,255))
    return canvas

def build(out):
    if out.exists():raise SystemExit("Refuse to overwrite existing V3 VFX candidate")
    atlas=Image.new("RGBA",(COLS*CELL,ROWS*CELL),(0,0,0,0))
    records=[]
    for row in range(ROWS):
        for key in range(COLS):
            part=draw(row,key)
            bounds=part.getbbox()
            if bounds is not None:
                if bounds[0]<1 or bounds[1]<1 or bounds[2]>=64 or bounds[3]>=55:
                    raise RuntimeError(f"Effect clips cell: {row}/{key}/{bounds}")
            data=np.asarray(part)
            count=int(np.count_nonzero(data[:,:,3]))
            if (key in KEYS[row]) != (count>0):
                raise RuntimeError(f"Published-only effect contract violated: {row}/{key}")
            atlas.alpha_composite(part,(key*CELL,row*CELL))
            records.append(dict(row=row,key=key,kind=TITLES[row],
                state=STATES[key],opaqueCount=count,
                rgbaSha256=hashlib.sha256(data.tobytes()).hexdigest(),
                bbox=list(bounds) if bounds else None))
    out.mkdir(parents=True)
    png=out/"Candidate-CH1-RegularEnemyVfx.png"
    atlas.save(png)
    preview(atlas).convert('RGB').save(out/"RegularEnemy-v3-ActionVfx-4x.png")
    profile=dict(schema="ch1-regular-enemy-v3-published-phase-vfx-v1",
        state="visual-only-candidate-not-artist-approved",
        resource="Chapter1Enemies/Candidate-RegularEnemyVfx",
        atlasSha256=hashlib.sha256(png.read_bytes()).hexdigest(),
        cell=[CELL,CELL],rows=ROWS,columns=COLS,pixelsPerUnit=18,
        normalizedPivot=[.5,.1875],
        allowedKeysByRow={str(row):list(keys) for row,keys in KEYS.items()},
        noPredictionOrHitDamage=True,noGameplayOrPhysicsChange=True,
        description="Walker pressure ram warning/burst/steam, Surveyor triangulation/optic fire/suppression/cooldown. Only completed published behavior phases.",
        frames=records,manualArtAccepted=False,CodexIndependentQA=False)
    (out/"manifest.json").write_text(json.dumps(profile,ensure_ascii=False,
        indent=2,sort_keys=True)+"\n",encoding='utf8')
    print('V3_ENEMY_ACTION_FX_CREATED',png,
          'SHA',profile['atlasSha256'],
          'activeFrames',sum(x['opaqueCount']>0 for x in records),'cells',len(records))

if __name__=="__main__":
    arg=argparse.ArgumentParser()
    arg.add_argument("--output",required=True,type=Path)
    build(arg.parse_args().output)