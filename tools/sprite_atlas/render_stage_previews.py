#!/usr/bin/env python3
"""Deterministic 1x/4x and GIF previews for source-only magenta sprite diagnostics.

This script never modifies original art or Unity Assets; it does not accept
sprites or infer an animation's gameplay timing/hitbox.
"""
import argparse
import hashlib
import json
from pathlib import Path

from PIL import Image, ImageDraw

CHOICES=("01-doeon-idle","03-seoha-npc-idle","10-doeon-combat","13-ordan-combat")

def sha256(path:Path)->str:
    return hashlib.sha256(path.read_bytes()).hexdigest()

def checker(width,height,size=12):
    pic=Image.new("RGBA",(width,height),(53,62,70,255))
    draw=ImageDraw.Draw(pic)
    for y in range(0,height,size):
        for x in range(0,width,size):
            if (x//size+y//size)%2==0:
                draw.rectangle((x,y,x+size-1,y+size-1),fill=(70,78,87,255))
    return pic

def frame_on_checker(img,scale:int=4):
    scaled=img.resize((img.width*scale,img.height*scale),Image.Resampling.NEAREST)
    return Image.alpha_composite(checker(scaled.width,scaled.height),scaled)

def stage():
    # Intentional draft set dressing; not a captured gameplay scene.
    pic=Image.new("RGBA",(640,360),(16,24,32,255))
    draw=ImageDraw.Draw(pic)
    draw.rectangle((0,280,639,359),fill=(35,52,60,255))
    draw.rectangle((0,278,639,281),fill=(93,112,112,255))
    for x in range(12,640,52):
        draw.rectangle((x,280,x+3,359),fill=(49,69,77,255))
        draw.rectangle((x-5,278,x+10,280),fill=(87,105,104,255))
    for x in range(18,640,40):
        h=68+(x*17)%95
        draw.rectangle((x,278-h,x+26,275),fill=(25,38,46,255))
        for yy in range(278-h+10,267,17):
            draw.rectangle((x+7,yy,x+9,yy+2),fill=(89,75,49,255))
    return pic

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument("--preflight",type=Path,required=True)
    ap.add_argument("--generation-plan",type=Path,required=True)
    ap.add_argument("--output-dir",type=Path,required=True)
    ap.add_argument("--sources",nargs="*",default=list(CHOICES))
    args=ap.parse_args()
    if args.output_dir.exists():raise SystemExit("Refusing to overwrite preview output")
    plan={j["id"]:j for j in json.loads(args.generation_plan.read_text("utf8"))["jobs"]}
    summaries=[]
    args.output_dir.mkdir(parents=True)
    for name in args.sources:
        base=args.preflight/name
        preflight=json.loads((base/"preflight.json").read_text("utf8"))
        atlas=Image.open(base/"uniform-scale-preview-atlas.png").convert("RGBA")
        grid=preflight["previewCell"]
        assert atlas.size==(grid[0]*6,grid[1]*preflight["rowCount"])
        if name not in plan:raise RuntimeError("Missing art plan for "+name)
        rows=[]
        for row, desc in enumerate(plan[name]["rows"]):
            label=desc.split(":",1)[0].strip()
            frames=[]
            for col in range(6):
                part=atlas.crop((col*grid[0],row*grid[1],(col+1)*grid[0],(row+1)*grid[1]))
                frames.append(frame_on_checker(part))
            dest=args.output_dir/f"{name}-row{row}-{label}.gif"
            frames[0].save(dest,save_all=True,append_images=frames[1:],duration=[240,120,120,120,150,270],loop=0,optimize=False)
            rows.append({"row":row,"name":label,"frames":6,"gif":dest.name,"sha256":sha256(dest)})
        summaries.append({"source":name,"preflightSourceSha256":preflight["sourceSha256"],
                          "frameCount":preflight["nonemptyCells"],"edgeTouchCells":preflight["edgeTouchCells"],
                          "previewRows":rows,"status":"source_only_diagnostic"})
        print(name, "GIFs",len(rows))
    canvas=stage()
    locations=(("01-doeon-idle",178),("13-ordan-combat",327),("03-seoha-npc-idle",465))
    for name,x in locations:
        info=json.loads((args.preflight/name/"preflight.json").read_text("utf8"))
        cell=info["previewCell"];baseline=info["previewBaselineTopLeft"]
        atlas=Image.open(args.preflight/name/"uniform-scale-preview-atlas.png").convert("RGBA")
        frame=atlas.crop((0,0,cell[0],cell[1]))
        canvas.alpha_composite(frame,(x-cell[0]//2,280-baseline))
    stage1=args.output_dir/"stage-reference-1x.png"
    stage4=args.output_dir/"stage-reference-4x.png"
    canvas.save(stage1)
    canvas.resize((2560,1440),Image.Resampling.NEAREST).save(stage4)
    manifest={"schema":"chapter1-stage-keypose-preview-v1","stageIsNotGameplay":True,"originalArtUnchanged":True,
              "status":"source_only_not_independently_reviewed","sources":summaries,
              "stage1x":{"path":stage1.name,"sha256":sha256(stage1)},
              "stage4x":{"path":stage4.name,"sha256":sha256(stage4)}}
    (args.output_dir/"manifest.json").write_text(json.dumps(manifest,ensure_ascii=False,sort_keys=True,indent=2)+"\n",encoding="utf8")
    print(args.output_dir/"manifest.json")

if __name__=="__main__":
    main()