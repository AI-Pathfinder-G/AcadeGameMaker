"""Fail-closed V8 relationship-pair diagnostic (4x4 unequal source rows)."""
from pathlib import Path
import argparse, hashlib, json
from PIL import Image
import math
from .build_diagnostic import ROOT, source_rgb_alpha, clean_master, stable_write, preview_gif, checker_contact
from .matte_v4 import clean as despill

CELL_W, CELL_H, CORE = 384, 128, 64

def digest(path): return hashlib.sha256(path.read_bytes()).hexdigest()

def validate(table, source):
    if not table.get("approved") or not table.get("reviewer"):
        raise ValueError("approved reviewer calibration is required")
    with Image.open(source) as image:
        if image.size != (1536,1024): raise ValueError("expected 1536x1024 source")
    if table.get("kind") != "relationship" or table.get("sourceSha256") != digest(source):
        raise ValueError("kind or source hash mismatch")
    bands=table.get("rowBands")
    if not isinstance(bands,list) or len(bands)!=4 or bands[0][0]!=0 or bands[-1][1]!=1024:
        raise ValueError("exact four row bands covering source height required")
    if any(a>=b for a,b in bands) or any(bands[i][1]!=bands[i+1][0] for i in range(3)):
        raise ValueError("row bands overlap or leave a gap")
    rows=table.get("rows")
    if not isinstance(rows,list) or len(rows)!=4: raise ValueError("four row calibrations required")
    for row in rows:
        if not all(k in row for k in ("scale","seryeongAnchor","doeonAnchor")):
            raise ValueError("each row requires fixed scale and both actor anchors")
        if not math.isfinite(row["scale"]) or row["scale"]<=0 or len(row["seryeongAnchor"])!=2 or len(row["doeonAnchor"])!=2:
            raise ValueError("invalid pair anchor calibration")
    return bands,rows

def build(source, table_path, dest):
    table=json.loads(table_path.read_text(encoding="utf-8")); bands,rows=validate(table,source)
    rgb,alpha,diag=source_rgb_alpha(source,broad=True)
    rgb,changed,_=despill(rgb,alpha,strict_key_spill=True,boundary_depth=2); diag["boundaryMatteChangedPixels"]=len(changed)
    master=clean_master(rgb,alpha)
    atlas=Image.new("RGBA",(4*192,4*128)); records=[]
    def gif(row,multiplier,path):
        frames=[]
        for c in range(4):
            frame=atlas.crop((c*192,row*128,(c+1)*192,(row+1)*128)).resize((192*multiplier,128*multiplier),Image.Resampling.NEAREST)
            checker=Image.new("RGBA",frame.size,(72,78,88,255)); checker.alpha_composite(frame); frames.append(checker.convert("P",palette=Image.Palette.ADAPTIVE,colors=256))
        frames[0].save(path,save_all=True,append_images=frames[1:],duration=167,loop=0,disposal=2,optimize=False)
    for row,(y0,y1) in enumerate(bands):
        ser,doe=rows[row]["seryeongAnchor"],rows[row]["doeonAnchor"]
        for col in range(4):
            x0,x1=col*CELL_W,(col+1)*CELL_W; crop=master.crop((x0,y0,x1,y1))
            # Anchors are local to a source column; each actor must appear in
            # each column, preventing one intact pair from masking a solo hole.
            def actor_present(anchor):
                ax,ay=x0+anchor[0],anchor[1]
                return any(alpha.getpixel((x,y))>=CORE for y in range(max(y0,ay-48),min(y1,ay+49)) for x in range(max(x0,ax-48),min(x1,ax+49)))
            if not actor_present(ser) or not actor_present(doe): raise ValueError(f"missing partner core evidence row {row}, column {col}")
            core=alpha.crop((x0,y0,x1,y1)).point(lambda a:255 if a>=CORE else 0).getbbox()
            if core is None: raise ValueError(f"missing partner/group core at row {row}, column {col}")
            bx0,by0,bx1,by1=core; group=crop.crop(core)
            scale=rows[row]["scale"]; tile=group.resize((round(group.width*scale),round(group.height*scale)),Image.Resampling.NEAREST)
            px=round(128+(bx0-ser[0])*scale); py=round(104+(y0+by0-ser[1])*scale)
            if px<0 or py<0 or px+tile.width>192 or py+tile.height>128: raise ValueError(f"pair placement exceeds native cell row {row}, column {col}")
            atlas.alpha_composite(tile,(col*192+px,row*128+py))
            doe_target=[round(128+(doe[0]-ser[0])*scale),round(104+(doe[1]-ser[1])*scale)]
            records.append({"row":row,"column":col,"sourceRect":[x0,y0,x1,y1],"coreBounds":[bx0,by0,bx1,by1],"seryeongAnchor":ser,"doeonAnchor":doe,"doeonTargetAnchor":doe_target,"fixedRowScale":scale,"placement":[px,py]})
    dest.mkdir(parents=True,exist_ok=True); atlas.save(dest/"native-atlas-pair-192x128-4x4.png"); checker_contact(atlas,dest/"contact-checker-4x.png")
    for row in range(4):
        gif(row,1,dest/f"row-{row+1}-native.gif"); gif(row,4,dest/f"row-{row+1}-4x.gif")
    try: source_name=str(source.relative_to(ROOT)).replace("\\","/")
    except ValueError: source_name=str(source)
    stable_write(dest/"diagnostic.json",{"label":"V8 pair diagnostic — source_only — not Accepted","acceptanceState":"source_only","source":source_name,"sourceSha256":digest(source),"kind":"relationship","grid":[4,4],"rowBands":bands,"rows":records,"reviewer":table["reviewer"],"diagnostic":diag,"actors":["seryeong","doeon"],"outputs":{"atlas":digest(dest/"native-atlas-pair-192x128-4x4.png")}})

def main():
 p=argparse.ArgumentParser();p.add_argument("--source",type=Path,required=True);p.add_argument("--calibration",type=Path,required=True);p.add_argument("--output",type=Path,required=True);a=p.parse_args();build(a.source,a.calibration,a.output)
if __name__=="__main__": main()
