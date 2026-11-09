#!/usr/bin/env python3
"""CH1 Walker / Surveyor v3: new character silhouettes, not cosmetic recolors.

64px independent directional pixel poses, 4 rows x 8 keys.
WALKER: pressure-crushing autonomous ram with unequal pressure cylinders,
boiler core, tiny cage-head, piston-arm and shock hoof.
SURVEYOR: monocular refraction aperture, deployable triangulation halo,
tripod-like surveying legs, lens/projector instead of a conventional rifle.

Pure visual art: preserves the existing 32-key animation contract. Never
modifies gameplay or imports copyrighted/reference sprite pixels.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw

CELL, COLS, ROWS=64,8,4
ROW_NAMES=("walker-right","walker-left","surveyor-right","surveyor-left")
POSES=("idle","stride-a","stride-b","telegraph","execute","hurt","dead","recover")
I=(20,26,34,255)
BLACK=(12,18,26,255)
STEEL=(76,84,89,255)
IRON=(42,49,55,255)
RUST=(124,66,45,255)
RUST_LIGHT=(177,99,55,255)
BRASS=(180,139,77,255)
WARNING=(250,178,59,255)
HOT=(255,229,135,255)
SOOT=(46,39,37,255)
COLD=(26,48,60,255)
MOSS=(49,78,82,255)
CYAN=(81,211,194,255)
PALE=(199,251,228,255)
INDIGO=(72,109,124,255)
CHALK=(155,178,172,255)
HURT=(250,119,99,255)


class Pen:
    """Side-oriented integer-coordinates; independently draws R/L objects."""
    def __init__(self,side:int):
        assert side in (-1,1)
        self.image=Image.new("RGBA",(CELL,CELL),(0,0,0,0))
        self.p=ImageDraw.Draw(self.image)
        self.side=side

    def x(self,dx:int):return 32+dx*self.side
    def pt(self,dx:int,y:int):return (self.x(dx),y)

    def poly(self,points,color,outline=None):
        xy=[self.pt(x,y) for x,y in points]
        self.p.polygon(xy,fill=color,outline=outline)

    def line(self,points,color,width=1):
        self.p.line([self.pt(x,y) for x,y in points],fill=color,width=width,joint="curve")

    def box(self,x1,y1,x2,y2,color):
        a,b=sorted((self.x(x1),self.x(x2)))
        self.p.rectangle((a,y1,b,y2),fill=color)

    def ellipse(self,cx,cy,rx,ry,color):
        a,b=sorted((self.x(cx-rx),self.x(cx+rx)))
        self.p.ellipse((a,cy-ry,b,cy+ry),fill=color)

    def dot(self,x,y,color):self.p.point(self.pt(x,y),fill=color)


def walker(side:int,key:int)->Image.Image:
    if key not in range(8):raise ValueError("invalid Walker key")
    q=Pen(side)
    p=q
    charge=(key==3)
    strike=(key==4)
    damaged=(key==5)
    dead=(key==6)
    recover=(key==7)
    gait=(-3 if key==1 else 3 if key==2 else 0)
    body=(-3 if charge else 2 if strike else -2 if damaged else 0)
    # Distinct dead posture: pressure vessel and ram fall over as a machine.
    if dead:
        p.poly([(-19,44),(-4,39),(13,39),(22,45),(20,50),(-19,50)],I)
        p.poly([(-18,45),(3,41),(17,45),(14,48),(-17,48)],RUST)
        p.ellipse(-10,43,7,6,BLACK)
        p.ellipse(-10,43,4,4,SOOT)
        p.poly([(4,39),(11,35),(23,42),(24,48)],STEEL,I)
        p.line([(14,47),(24,49)],BRASS,2)
        p.box(5,43,10,45,SOOT) # extinguished boiler
        p.box(-17,49,19,51,I)
        return q.image

    # Huge, grounded, unequal hoof/pressure legs.
    rear=(-9+gait)
    front=(10-gait)
    for pos,s in ((rear,-1),(front,1)):
        p.poly([(pos-5,35),(pos+5,35),(pos+5+s,45),
                (pos+4+s,49),(pos-4+s,49),(pos-6,44)],I)
        p.poly([(pos-3,38),(pos+2,37),(pos+3+s,44),
                (pos-3+s,45)],RUST if s==-1 else IRON)
        p.line([(pos-3+s,43),(pos+3+s,45)],BRASS,2)
        p.poly([(pos-8+s,47),(pos+5+s,47),
                (pos+8+s,50),(pos+6+s,51),(pos-8+s,51)],BLACK)
        p.box(pos-6+s,49,pos+4+s,50,STEEL)
    # Non-human silhouette: behind the boiler hang two unequal pressure
    # cylinders. High vent stacks make the character legible at 1x.
    p.poly([(-18,15),(-11,11),(-7,17),(-7,34),(-19,33)],I)
    p.box(-17,18,-9,30,RUST)
    p.box(-16,19,-14,27,RUST_LIGHT)
    p.box(-12,20,-11,27,BRASS)
    p.line([(-15,15),(-15,8)],I,3)
    p.line([(-14,11),(-17,9)],STEEL,2)
    p.ellipse(-16,9,2,2,WARNING if charge else BRASS)
    p.poly([(-22,21),(-18,19),(-17,29),(-22,30)],I)
    p.box(-21,22,-19,27,SOOT)
    # Boiler torso: a squat six-sided pressure vessel rather than armor
    # on a human body. Ram arm is many pixels larger than the face.
    p.poly([(-12+body,22),(-3+body,15),(11+body,17),
            (18+body,26),(14+body,39),(-11+body,37),
            (-17+body,31)],I)
    p.poly([(-10+body,23),(-1+body,18),(9+body,20),
            (14+body,27),(11+body,35),(-9+body,34)],RUST)
    p.poly([(-6+body,23),(7+body,22),(10+body,28),
            (4+body,34),(-8+body,32)],IRON)
    p.line([(-7+body,25),(1+body,22),(9+body,25)],STEEL,2)
    p.line([(-11+body,34),(11+body,35)],BRASS,2)
    # Heavy boiler door, outer dark ring and explicit central flame dial.
    p.ellipse(0+body,28,7,7,BLACK)
    p.ellipse(0+body,28,5,5,BRASS)
    p.ellipse(0+body,28,3,3,
              HOT if strike else WARNING if charge else
              SOOT if recover else RUST_LIGHT)
    p.dot(0+body,28,HOT if charge or strike else BRASS)
    for x,y in ((-10,28),(10,30),(-2,35)):
        p.dot(x+body,y,BRASS)
    # Independent hose curls from tank to boiler, not a generic backpack.
    p.line([(-16,22),(-14,16),(-9,14),(-4+body,20)],BLACK,3)
    p.line([(-16,22),(-14,16),(-9,14),(-4+body,20)],BRASS,1)
    p.line([(-18,30),(-13,38),(-7+body,34)],STEEL,2)
    # Micro cage head/ram crest, deliberately dominated by the pressure arm.
    p.poly([(5+body,12),(11+body,10),(16+body,14),
            (15+body,21),(8+body,22),(4+body,18)],I)
    p.poly([(8+body,13),(13+body,13),(14+body,19),
            (7+body,19)],STEEL)
    p.line([(9+body,16),(14+body,16)],WARNING if charge or strike else BRASS,2)
    p.poly([(11+body,11),(18+body,11),(18+body,14),
            (15+body,15)],BLACK)
    # Recoil side shock absorber, separate from the forward piston arm.
    p.line([(-12+body,23),(-20,35)],I,7)
    p.line([(-12+body,23),(-20,35)],STEEL,4)
    p.ellipse(-20,35,4,4,I)
    p.line([(-22,33),(-23,37)],BRASS,2)
    # Large asymmetric forearm and hydraulic ram head. Substantial visual
    # shift on telegraph/strike; no identical keypose retained.
    piston_tip=4 if charge else 15 if strike else 9 if damaged else 12 if recover else 10
    piston_y=29 if charge else 24 if strike else 35 if damaged else 33
    p.ellipse(11+body,25,5,5,BLACK)
    p.ellipse(11+body,25,3,3,STEEL)
    p.line([(11+body,25),(piston_tip,piston_y)],I,9)
    p.line([(12+body,25),(piston_tip,piston_y)],STEEL,5)
    p.line([(12+body,24),(piston_tip-3,piston_y-1)],BRASS,2)
    if charge:
        p.line([(18,25),(21,29)],WARNING,2)
        p.box(19,27,23,29,HOT)
    elif strike:
        p.line([(21,22),(28,21)],HOT,3)
    claw=piston_tip+3
    p.poly([(claw-4,piston_y-6),(claw+4,piston_y-6),
            (claw+7,piston_y-2),(claw+6,piston_y+5),
            (claw-5,piston_y+5)],BLACK)
    p.poly([(claw-2,piston_y-4),(claw+3,piston_y-4),
            (claw+5,piston_y-1),(claw+4,piston_y+3),
            (claw-3,piston_y+3)],RUST_LIGHT)
    p.box(claw-1,piston_y-3,claw+2,piston_y+3,
          HOT if strike or charge else BRASS)
    p.poly([(claw+5,piston_y-2),(claw+8,piston_y),
            (claw+5,piston_y+2)],I)
    # Distinct phase silhouette/telegraph; effects stay bounded to 64px.
    if charge:
        p.line([(1,13),(1,8)],WARNING,2)
        p.dot(1,6,HOT)
        p.line([(18,14),(21,11)],WARNING,2)
    if strike:
        p.line([(claw+8,19),(claw+10,16)],WARNING,2)
        p.dot(claw+11,15,HOT)
        p.line([(claw+7,28),(claw+10,30)],RUST_LIGHT,2)
    if damaged:
        p.line([(-8,14),(-13,11)],HURT,2)
        p.dot(-12,9,WARNING)
        p.line([(13,32),(17,36)],HURT,1)
    if recover:
        p.line([(-15,13),(-20,10)],STEEL,1)
        p.ellipse(1,28,2,2,SOOT)
    return q.image


def surveyor(side:int,key:int)->Image.Image:
    if key not in range(8):raise ValueError("invalid Surveyor key")
    p=Pen(side)
    charge=key==3
    fire=key==4
    hurt=key==5
    dead=key==6
    recover=key==7
    gait=(-3 if key==1 else 3 if key==2 else 0)
    sway=(1 if key==1 else -1 if key==2 else -2 if hurt else 0)
    if dead:
        # Collapsed measuring mast, fallen triangle-frame and inert lens.
        p.poly([(-17,43),(-10,38),(2,41),(17,47),
                (17,50),(-20,50)],I)
        p.line([(-18,46),(19,48)],MOSS,3)
        p.line([(-13,47),(-6,32),(6,47)],STEEL,2)
        p.ellipse(10,42,5,5,BLACK)
        p.ellipse(10,42,3,3,COLD)
        p.line([(-13,50),(19,50)],I,2)
        p.dot(-17,45,CHALK)
        return p.image

    # Three-legged surveying instrument, NOT a soldier's two trouser legs.
    for pos,dx in ((-8+gait,-6),(9-gait,6)):
        p.line([(pos,36),(pos+dx,48)],I,6)
        p.line([(pos,36),(pos+dx,48)],INDIGO,3)
        p.poly([(pos+dx-5,48),(pos+dx+4,48),
                (pos+dx+6,51),(pos+dx-5,51)],BLACK)
        p.line([(pos+dx-3,49),(pos+dx+3,49)],CYAN,1)
    # Third deployable compensator leg is asymmetrically behind the left.
    p.line([(-5,37),(-15,48)],I,4)
    p.line([(-7,39),(-14,47)],STEEL,2)
    p.box(-19,49,-12,51,BLACK)
    p.dot(-16,49,CYAN)
    # Thin diagnostic mast and exoskeletal reticule support.
    p.poly([(-7+sway,27),(-3+sway,22),(6+sway,25),
            (9+sway,38),(-10+sway,40)],I)
    p.poly([(-4+sway,27),(5+sway,27),
            (6+sway,36),(-8+sway,36)],MOSS)
    p.line([(-5+sway,31),(6+sway,33)],INDIGO,2)
    p.line([(0+sway,28),(0+sway,38)],CHALK,1)
    p.ellipse(1+sway,33,2,3,BLACK)
    p.dot(1+sway,33,CYAN if not hurt else HURT)
    # A triangular halo is the signature of the prism surveyor. It expands
    # in aim mode; different geometry / asymmetrical floating antenna.
    open_step=3 if charge or fire else 0
    if hurt:open_step=-2
    p.poly([(-19-open_step,11),(-4,5),(12+open_step,12),
            (7+open_step,28),(-18-open_step,25)],None,outline=STEEL)
    p.line([(-19-open_step,11),(-4,5),(12+open_step,12)],INDIGO,2)
    p.line([(-18-open_step,25),(-19-open_step,11)],MOSS,2)
    p.line([(12+open_step,12),(7+open_step,28)],MOSS,2)
    p.ellipse(-4,5,2,2,CYAN if charge else CHALK)
    p.dot(-19-open_step,11,PALE if charge else CYAN)
    p.dot(12+open_step,12,PALE if charge else CYAN)
    # Right-facing source scanner has its own back sensor on a single
    # extendable arm: do not mirror other model or its weapon.
    p.line([(-15,21),(-21,18)],STEEL,2)
    p.ellipse(-21,18,3,3,BLACK)
    p.dot(-21,18,CYAN)
    # Enormous single prism eye, much bigger than a human face.
    p.poly([(2+sway,15),(7+sway,9),(16+sway,11),
            (21+sway,17),(16+sway,25),(7+sway,25)],BLACK)
    p.poly([(6+sway,16),(10+sway,12),(16+sway,14),
            (18+sway,18),(15+sway,22),(8+sway,21)],INDIGO)
    p.ellipse(12+sway,18,5,5,BLACK)
    p.ellipse(12+sway,18,3,4,
              PALE if charge or fire else CYAN if not hurt else COLD)
    p.line([(10+sway,17),(15+sway,20)],CHALK,1)
    p.dot(14+sway,16,PALE if charge or fire else INDIGO)
    # Angular triangulation projector with a split nose, not a rifle.
    arm_y=30 if fire else 33 if charge else 36 if hurt else 34
    nose=18 if charge else 18 if fire else 17 if recover else 20
    p.line([(4+sway,28),(nose,arm_y)],BLACK,6)
    p.line([(5+sway,28),(nose,arm_y)],INDIGO,3)
    p.poly([(nose-5,arm_y-4),(nose+2,arm_y-5),
            (nose+5,arm_y),(nose+2,arm_y+4),
            (nose-5,arm_y+3)],BLACK)
    p.poly([(nose-3,arm_y-3),(nose+1,arm_y-3),
            (nose+3,arm_y),(nose+1,arm_y+2),
            (nose-3,arm_y+1)],MOSS)
    p.ellipse(nose+2,arm_y,2,2,
              PALE if fire else CYAN if charge else INDIGO)
    # Distinctive triangulation guides. These marks show the attack
    # phases only; they do not add a gameplay laser/target/hitbox.
    if charge:
        p.line([(nose+5,arm_y-8),(nose+11,arm_y-5)],CYAN,1)
        p.line([(nose+5,arm_y+8),(nose+11,arm_y+5)],CYAN,1)
        p.dot(nose+11,arm_y,PALE)
        p.line([(-5,8),(-5,3)],PALE,1)
    if fire:
        p.line([(nose+5,arm_y),(nose+11,arm_y-2)],PALE,2)
        p.line([(nose+8,arm_y-5),(nose+11,arm_y-7)],CYAN,1)
        p.dot(nose+11,arm_y-2,PALE)
    if hurt:
        p.line([(-9,18),(-14,14)],HURT,2)
        p.dot(-15,12,HURT)
        p.line([(8,20),(12,24)],HURT,1)
    if recover:
        p.dot(-4,5,INDIGO)
        p.line([(nose+2,arm_y),(nose+4,arm_y)],MOSS,1)
    return p.image


def render(row:int,key:int)->Image.Image:
    if not (0<=row<ROWS and 0<=key<COLS):raise ValueError("invalid row/key")
    side=1 if row in (0,2) else -1
    return walker(side,key) if row<2 else surveyor(side,key)


def contact_sheet(sheet:Image.Image,scale=4)->Image.Image:
    panel=Image.new("RGB",(COLS*CELL*scale,ROWS*(CELL*scale+32)),
                    (31,40,50))
    d=ImageDraw.Draw(panel)
    for row in range(ROWS):
        for key in range(COLS):
            x=key*CELL*scale;y=row*(CELL*scale+32)
            tile=sheet.crop((key*CELL,row*CELL,(key+1)*CELL,(row+1)*CELL))
            back=Image.new("RGBA",(CELL*scale,CELL*scale),(52,62,70,255))
            bp=ImageDraw.Draw(back)
            for yy in range(0,CELL*scale,16):
                for xx in range(0,CELL*scale,16):
                    if (xx//16+yy//16)&1:
                        bp.rectangle((xx,yy,xx+15,yy+15),fill=(66,78,87,255))
            back.alpha_composite(tile.resize((CELL*scale,CELL*scale),
                                              Image.Resampling.NEAREST))
            panel.paste(back.convert("RGB"),(x,y+30))
            short=("IDL","ST-A","ST-B","WARN","ACT","HIT","DEAD","REC")
            label=(f"{'W' if row<2 else 'S'}{'<' if row%2 else '>'} {short[key]}"
                   if scale==1 else f"{ROW_NAMES[row]} / {POSES[key]}")
            d.text((x+3,y+8),label,fill=(238,229,204))
    return panel


def build(out:Path):
    if out.exists():raise SystemExit("Refuse to overwrite existing candidate")
    frames=[];atlas=Image.new("RGBA",(CELL*COLS,CELL*ROWS),(0,0,0,0))
    for row in range(ROWS):
        for key in range(COLS):
            sprite=render(row,key)
            rgba=np.asarray(sprite)
            bbox=sprite.getbbox()
            if bbox is None or bbox[0]<1 or bbox[1]<1 or bbox[2]>=64 or bbox[3]>53:
                raise ValueError(f"Sprite clipped / outside 64px margins: {row},{key},{bbox}")
            count=int(np.count_nonzero(rgba[:,:,3]))
            if count<75:raise ValueError(f"Empty or tiny sprite {row},{key}")
            foot=int(np.where(rgba[:,:,3]>40)[0].max())
            if foot not in (50,51):raise ValueError(f"Foot anchor lost {row},{key}: {foot}")
            atlas.alpha_composite(sprite,(key*CELL,row*CELL))
            frames.append(dict(row=row,key=key,enemy=ROW_NAMES[row],
                               action=POSES[key],bbox=list(bbox),
                               opaque=count,footY=foot,
                               sha256=hashlib.sha256(rgba.tobytes()).hexdigest()))
    if len({x["sha256"] for x in frames})!=32:
        raise ValueError("Duplicate authored sprite poses")
    out.mkdir(parents=True)
    png=out/"Candidate-CH1-RegularEnemies-v3.png"
    atlas.save(png)
    contact_sheet(atlas,4).save(out/"RegularEnemies-v3-review-4x.png")
    contact_sheet(atlas,1).save(out/"RegularEnemies-v3-native-1x.png")
    data=dict(schema="ch1-walker-surveyor-silhouette-redesign-v3",
              status="provisional-original-character-art-not-independent-approved",
              atlas=png.name,atlasSha256=hashlib.sha256(png.read_bytes()).hexdigest(),
              pixelCell=[64,64],rows=4,columns=8,pixelsPerUnit=18,
              normalizedPivot=[.5,.1875],
              rowNames=list(ROW_NAMES),actionKeys=list(POSES),
              concept={"walker":"boiler ram exoskeleton with asymmetrical tanks, hoof legs, massive pressure piston",
                       "surveyor":"large single prism eye with deployable survey triangle, asymmetric sensor mast and tripod legs"},
              noOtherCharacterSpriteInputs=True,noMirroredSourceFrames=True,
              noCombatOrSaveLogicChange=True,
              allFrames=frames,manualArtAccepted=False,
              codexIndependentQA=False)
    (out/"manifest.json").write_text(json.dumps(data,indent=2,ensure_ascii=False,
                                                sort_keys=True)+"\n",encoding="utf8")
    print("REDESIGN_V3",out,"SHA",data["atlasSha256"],
          "SOURCES",len(frames),"32 unique")


if __name__=="__main__":
    ap=argparse.ArgumentParser()
    ap.add_argument("--output",required=True,type=Path)
    build(ap.parse_args().output)