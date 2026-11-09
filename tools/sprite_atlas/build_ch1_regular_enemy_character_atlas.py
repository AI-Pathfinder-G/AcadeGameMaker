#!/usr/bin/env python3
"""Original, editable Chapter1 regular-enemy art prototypes.

32 individually composed 64px pixel frames, with 2 unique creature designs:
Walker (heavy close-range charging mechanical guard) and Surveyor (tall,
ranged industrial marksman). Four rows: walker facing right and left, surveyor
facing right and left. Eight keyed poses: idle, walkA, walkB, telegraph, execute,
hurt, dead, recover. Coordinates, costumes, forearms and gear are editable
authored geometry, not extracted player sprites and no asset mirroring.

The generated Unity atlas is PROVISIONAL art. No behavior/damage/physics
authority; runtime binds these frames to published regular enemy snapshots.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw

CELL=64
ROWS=4
COLS=8
NAMES=("walker-right","walker-left","surveyor-right","surveyor-left")
POSES=("idle","stride-a","stride-b","telegraph","execute","hurt","dead","recover")

INK=(21,26,35,255)
SHADOW=(35,43,52,255)
STEEL=(76,87,91,255)
LIT=(126,139,128,255)
RUST=(152,81,44,255)
AMBER=(240,167,66,255)
DARK_AMBER=(189,104,46,255)
COAT=(38,61,72,255)
COAT_LIT=(66,98,105,255)
CYAN=(117,222,212,255)
GLOW=(214,251,227,255)
BONE=(157,154,133,255)
DARK=(31,36,44,255)

def paint_walker(facing:int,key:int):
    a=Image.new("RGBA",(CELL,CELL),(0,0,0,0))
    d=ImageDraw.Draw(a)
    side=1 if facing>0 else -1
    standing=(key!=6)
    stride= -3 if key==1 else 3 if key==2 else 0
    lean= 3*side if key==4 else -2*side if key==3 else 0
    lower=5 if key==6 else 0
    def shift_rect(box,fill,outline=None):
        x1,y1,x2,y2=box
        d.rectangle((x1+lean,y1+lower,x2+lean,y2+lower),fill=fill,outline=outline)
    # Bare hydraulic ankles; boot toes remain visually tied to floor.
    if standing:
        for x,delta in ((25,stride),(39,-stride)):
            d.polygon([(x-5,36),(x+4,36),(x+5+delta,48),(x+1+delta,50),
                       (x-4+delta,47)],fill=INK)
            d.polygon([(x-2,39),(x+2,39),(x+3+delta,46),(x-2+delta,46)],
                      fill=STEEL)
            d.rectangle((x-7+delta,48,x+5+delta,51),fill=INK)
            d.rectangle((x-6+delta,49,x+4+delta,50),fill=(87,74,52,255))
            d.point((x+4+delta,50),fill=AMBER)
    else:
        d.polygon([(24,40),(38,44),(44,48),(23,48)],fill=INK)
        d.line((23,48,46,48),fill=DARK_AMBER,width=2)
    # Heavy utility belt, asymmetric back tank.
    shift_rect((23,30,43,38),INK)
    shift_rect((25,31,41,35),STEEL)
    shift_rect((27,32,32,34),RUST)
    backpack_x=19 if facing>0 else 43
    d.rectangle((backpack_x-4+lean,19+lower,backpack_x+3+lean,32+lower),fill=INK)
    d.rectangle((backpack_x-2+lean,22+lower,backpack_x+1+lean,29+lower),fill=RUST)
    d.point((backpack_x,20+lower),fill=AMBER)
    # Guard torso, weight is in the forward-leaning breastplate.
    shift_rect((23,18,43,32),INK)
    shift_rect((25,20,41,30),SHADOW)
    d.polygon([(28+lean,20+lower),(39+lean,21+lower),
               (40+lean,29+lower),(31+lean,30+lower)],fill=STEEL)
    d.line((29+lean,23+lower,37+lean,26+lower),fill=(120,122,98,255),width=2)
    d.rectangle((27+lean,29+lower,37+lean,32+lower),fill=INK)
    d.point((33+lean,30+lower),fill=AMBER)
    # Riveted pauldron and front shield-claw.
    front_x=45 if facing>0 else 19
    rear_x=20 if facing>0 else 44
    shift_rect((rear_x-4,20,rear_x+3,29),INK)
    shift_rect((rear_x-3,22,rear_x+1,27),STEEL)
    shift_rect((front_x-5,20,front_x+3,27),INK)
    shift_rect((front_x-4,21,front_x+1,25),RUST)
    weapon_delta=8 if key==4 else 2 if key==3 else -3 if key==5 else 0
    clenched=(front_x+side*weapon_delta)
    d.line((front_x+lean,26+lower,clenched+lean,35+lower- (3 if key==4 else 0)),
           fill=INK,width=7)
    d.line((front_x+lean,27+lower,clenched+lean,34+lower- (3 if key==4 else 0)),
           fill=STEEL,width=4)
    d.rectangle((clenched-3+lean,32+lower-(3 if key==4 else 0),
                 clenched+4+lean,38+lower-(3 if key==4 else 0)),fill=INK)
    d.rectangle((clenched-1+lean,33+lower-(3 if key==4 else 0),
                 clenched+2+lean,35+lower-(3 if key==4 else 0)),fill=AMBER if key in (3,4) else RUST)
    # Head and ram-like face cage.
    shift_rect((28,10,39,22),INK)
    shift_rect((29,11,38,19),STEEL)
    shift_rect((30,12,37,15),SHADOW)
    vstart=34 if side>0 else 29
    d.rectangle((vstart+lean,16+lower,vstart+3+lean,17+lower),
                fill=AMBER if key in (3,4) else (177,121,59,255))
    d.polygon([(29+lean,11+lower),(26+lean,12+lower),(27+lean,17+lower),
               (30+lean,17+lower)],fill=INK)
    d.rectangle((31+lean,20+lower,36+lean,22+lower),fill=DARK_AMBER)
    # Clear warning/impact cues on the actual published phase only.
    if key==3:
        d.line((48+side*4,12,51+side*3,8),fill=AMBER,width=2)
        d.line((48+side*4,16,52+side*4,16),fill=AMBER,width=1)
    if key==4:
        origin=clenched+side*3
        d.line((origin,25,origin+side*4,23),fill=AMBER,width=2)
        d.point((origin+side*5,21),fill=(255,230,145,255))
    if key==5:
        d.line((17,24,20,28),fill=(249,128,94,255),width=2)
        d.point((16,27),fill=(247,185,98,255))
    if key==6:
        d.line((20,45,48,45),fill=STEEL,width=2)
        d.point((42,40),fill=DARK_AMBER)
    if key==7:
        d.rectangle((30,24,33,25),fill=AMBER)
    return a

def paint_surveyor(facing:int,key:int):
    a=Image.new("RGBA",(CELL,CELL),(0,0,0,0))
    d=ImageDraw.Draw(a)
    side=1 if facing>0 else -1
    stride=2 if key==1 else -2 if key==2 else 0
    crouch=6 if key==6 else 0
    sway= 1 if key==1 else -1 if key==2 else 0
    # Narrow boots, long coat hem and split legs.
    if key!=6:
        for x,delta in ((29,stride),(39,-stride)):
            d.polygon([(x-3,35),(x+3,35),(x+2+delta,47),(x-3+delta,48)],
                      fill=INK)
            d.line((x+delta-2,44,x+delta+1,46),fill=COAT_LIT,width=2)
            d.rectangle((x-5+delta,48,x+4+delta,51),fill=INK)
            d.line((x-4+delta,50,x+2+delta,50),fill=(69,131,130,255),width=1)
    else:
        d.polygon([(24,46),(32,39),(43,46),(47,50),(23,50)],fill=INK)
        d.line((25,50,47,50),fill=CYAN,width=1)
    d.polygon([(25+sway,30+crouch),(42+sway,30+crouch),
               (45+sway,42+crouch),(23+sway,42+crouch)],fill=INK)
    d.polygon([(27+sway,30+crouch),(40+sway,30+crouch),
               (43+sway,39+crouch),(25+sway,39+crouch)],fill=COAT)
    d.line((29+sway,30+crouch,30+sway,39+crouch),fill=COAT_LIT,width=2)
    d.line((38+sway,30+crouch,41+sway,40+crouch),fill=(24,41,52,255),width=2)
    # Face, respirator and high protective hood.
    d.polygon([(27+sway,13+crouch),(30+sway,9+crouch),
               (39+sway,9+crouch),(42+sway,13+crouch),
               (40+sway,22+crouch),(26+sway,22+crouch)],fill=INK)
    d.polygon([(29+sway,13+crouch),(38+sway,11+crouch),
               (39+sway,19+crouch),(29+sway,20+crouch)],fill=COAT_LIT)
    # Direction-specific monocular: the source is drawn separately,
    # not horizontally flipped from a finished image.
    eye=38 if side>0 else 28
    d.rectangle((eye+sway,15+crouch,eye+3+sway,17+crouch),
                fill=GLOW if key in (3,4) else CYAN)
    d.line((31+sway,20+crouch,38+sway,21+crouch),fill=INK,width=2)
    d.point((34+sway,21+crouch),fill=(166,166,144,255))
    # Scarf and asymmetrical shoulder-mounted telemetry antenna.
    d.rectangle((27+sway,23+crouch,41+sway,33+crouch),fill=INK)
    d.polygon([(28+sway,24+crouch),(39+sway,23+crouch),
               (40+sway,32+crouch),(29+sway,31+crouch)],fill=COAT)
    d.line((31+sway,25+crouch,37+sway,29+crouch),fill=COAT_LIT,width=2)
    d.rectangle((33+sway,31+crouch,38+sway,33+crouch),fill=BONE)
    back=24 if side>0 else 44
    d.line((back+sway,20+crouch,back+sway,10+crouch),fill=INK,width=3)
    d.point((back+sway,9+crouch),fill=CYAN)
    d.line((back+sway,22+crouch,back+sway+side*3,27+crouch),fill=COAT_LIT,width=3)
    # Right/left rifle, independent facial straps and barrel placements.
    muzzle=51 if side>0 else 13
    stock=31 if side>0 else 39
    if key==4:
        rifle_y=25
    elif key==3:
        rifle_y=29
    elif key==5:
        rifle_y=35
    elif key==7:
        rifle_y=34  # spent-shot recovery lowers the barrel
    else:
        rifle_y=32
    rifle_y+=crouch
    d.line((stock+sway,rifle_y,muzzle+sway,rifle_y-2),fill=INK,width=5)
    d.line((stock+sway,rifle_y-1,muzzle+sway,rifle_y-3),fill=STEEL,width=2)
    d.rectangle((min(stock,muzzle)+2+sway,rifle_y-5,
                 min(stock,muzzle)+6+sway,rifle_y-3),fill=CYAN if key in (3,4) else COAT_LIT)
    d.point((muzzle+sway,rifle_y-2),fill=GLOW if key==4 else BONE)
    arm=41 if side>0 else 26
    d.line((arm+sway,26+crouch,stock+side*3+sway,rifle_y+2),fill=INK,width=5)
    d.line((arm+sway,27+crouch,stock+side*3+sway,rifle_y+1),fill=COAT_LIT,width=2)
    if key==3:
        for dy in (15,20):
            d.line((muzzle+side*3,dy,muzzle+side*5,dy),fill=CYAN,width=1)
    if key==4:
        d.line((muzzle+side*2,rifle_y-3,muzzle+side*7,rifle_y-5),fill=CYAN,width=2)
        d.point((muzzle+side*8,rifle_y-7),fill=GLOW)
    if key==5:
        d.line((21,27,17,22),fill=(250,148,115,255),width=1)
        d.point((18,20),fill=(232,116,96,255))
    if key==6:
        d.line((25,46,43,46),fill=(32,81,87,255),width=2)
    return a

def frame(row:int,key:int)->Image.Image:
    if not (0<=row<ROWS and 0<=key<COLS):
        raise ValueError("Unknown enemy animation pose")
    facing=1 if row in (0,2) else -1
    return paint_walker(facing,key) if row<2 else paint_surveyor(facing,key)

def preview(atlas:Image.Image)->Image.Image:
    scale=4
    canvas=Image.new("RGBA",(COLS*CELL*scale,ROWS*(CELL*scale+25)),(36,45,53,255))
    d=ImageDraw.Draw(canvas)
    for row in range(ROWS):
        for key in range(COLS):
            tile=atlas.crop((key*CELL,row*CELL,(key+1)*CELL,(row+1)*CELL))
            bg=Image.new("RGBA",(CELL*scale,CELL*scale),(50,59,69,255))
            db=ImageDraw.Draw(bg)
            for y in range(0,CELL*scale,16):
                for x in range(0,CELL*scale,16):
                    if ((x//16+y//16)&1):
                        db.rectangle((x,y,x+15,y+15),fill=(66,76,86,255))
            bg.alpha_composite(tile.resize(bg.size,Image.Resampling.NEAREST))
            x=key*CELL*scale;y=row*(CELL*scale+25)
            canvas.alpha_composite(bg,(x,y+25))
            d.text((x+5,y+6),NAMES[row]+" "+POSES[key],fill=(244,237,222,255))
    return canvas

def build(output:Path):
    if output.exists():
        raise SystemExit("Refuse to overwrite an existing source candidate")
    output.mkdir(parents=True)
    atlas=Image.new("RGBA",(COLS*CELL,ROWS*CELL),(0,0,0,0))
    records=[]
    for row in range(ROWS):
        for key in range(COLS):
            art=frame(row,key)
            bb=art.getbbox()
            if not bb or bb[0]<2 or bb[1]<4 or bb[2]>63 or bb[3]>64:
                raise ValueError(f"Actor sprite clips frame: row{row} key{key} bbox={bb}")
            if key!=6 and (bb[3]<51 or bb[3]>53):
                raise ValueError(f"Standing frame baseline unstable: row{row} frame{key}")
            rgba=np.asarray(art)
            if int(np.count_nonzero(rgba[:,:,3]>0))<80:
                raise ValueError("Unexpectedly empty enemy sprite")
            atlas.alpha_composite(art,(key*CELL,row*CELL))
            records.append(dict(enemy=NAMES[row],pose=POSES[key],row=row,key=key,
                                sourceAlphaBounds=list(bb),
                                opaquePixels=int(np.count_nonzero(rgba[:,:,3]>0)),
                                pixelsHash=hashlib.sha256(rgba.tobytes()).hexdigest()))
    out=output/"Candidate-CH1-RegularEnemies.png"
    atlas.save(out)
    preview(atlas).convert("RGB").save(output/"RegularEnemies-preview-4x.jpg",quality=92)
    record=dict(schema="ch1-regular-enemies-authored-character-pixel-candidate-v1",
                status="provisional-visual-only-not-final-approved",
                atlas=out.name,atlasSha256=hashlib.sha256(out.read_bytes()).hexdigest(),
                cols=COLS,rows=ROWS,cell=[CELL,CELL],ppu=18,
                pivot=[.5,.1875],rowsByEnemy=list(NAMES),
                keyNames=list(POSES),notDerivedFromDoeonOrOtherCopyrightedMedia=True,
                noImageMirroring=True,
                noGameMechanicsOrHitboxChanges=True,
                frames=records,
                limits="Artist-direction approval, manual 1x combat readability, real-world screenshot and future independent QA required.")
    (output/"manifest.json").write_text(
        json.dumps(record,ensure_ascii=False,sort_keys=True,indent=2)+"\n",encoding="utf8")
    print("ENEMY_CANDIDATE",out,"sha256",record["atlasSha256"],
          "sprites",len(records))

if __name__=="__main__":
    ap=argparse.ArgumentParser()
    ap.add_argument("--output",type=Path,required=True)
    build(ap.parse_args().output)