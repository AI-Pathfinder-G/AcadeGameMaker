"""Boundary-only magenta-matte decontamination candidate (diagnostic V4)."""
from pathlib import Path
import json, re
from PIL import Image, ImageDraw
try:
 from .component_v3 import ROOT, OUT, CELL, source_rgb_alpha, clean_master, sha256, stable_write, checker_contact, preview_gif
except ImportError:
 from component_v3 import ROOT, OUT, CELL, source_rgb_alpha, clean_master, sha256, stable_write, checker_contact, preview_gif

def spill(p): return min(p[0],p[2])-p[1] >= 45 and max(p[0],p[2]) >= 100
def hue_class(p):
    r,g,b=p; return (r-b, min(r,b)-g)
def edge(alpha,x,y):
    w,h=alpha.size
    return any(not (0<=nx<w and 0<=ny<h) or alpha.getpixel((nx,ny))==0 for nx,ny in ((x-1,y),(x+1,y),(x,y-1),(x,y+1)))
def nearest_interior(rgb,alpha,x,y, *, allow_spill=False):
    w,h=rgb.size
    for radius in range(1,7):
        found=[]
        for ny in range(max(0,y-radius),min(h,y+radius+1)):
            for nx in range(max(0,x-radius),min(w,x+radius+1)):
                if (nx,ny)==(x,y) or not alpha.getpixel((nx,ny)) or edge(alpha,nx,ny): continue
                if allow_spill or not spill(rgb.getpixel((nx,ny))): found.append((nx,ny))
        if found:
            if allow_spill:
                return min(found,key=lambda q:(max(0,min(rgb.getpixel(q)[0],rgb.getpixel(q)[2])-rgb.getpixel(q)[1]),(q[0]-x)**2+(q[1]-y)**2))
            return min(found,key=lambda q:(q[0]-x)**2+(q[1]-y)**2)
    return None
def clean(rgb,alpha, *, strict_key_spill=False, boundary_depth=1):
    if boundary_depth < 1:
        raise ValueError('boundary_depth must be positive')
    def near_boundary(x,y):
        for dy in range(-boundary_depth,boundary_depth+1):
            for dx in range(-boundary_depth,boundary_depth+1):
                if abs(dx)+abs(dy) > boundary_depth: continue
                nx,ny=x+dx,y+dy
                if not (0 <= nx < rgb.width and 0 <= ny < rgb.height) or alpha.getpixel((nx,ny)) == 0:
                    return True
        return False
    out=rgb.copy(); before=rgb.copy(); changed=[]; draw=ImageDraw.Draw(before)
    for y in range(rgb.height):
      for x in range(rgb.width):
       p=rgb.getpixel((x,y))
       if not (alpha.getpixel((x,y)) and near_boundary(x,y) and spill(p)): continue
       q=nearest_interior(rgb,alpha,x,y,allow_spill=strict_key_spill)
       if not q: continue
       fg=rgb.getpixel(q)
       # Same chromatic family is an authored plum/purple candidate, not matte.
       # Pair diagnostics may additionally remove only the bright, strongly-key-like
       # subset.  Muted plum garment cores remain protected.
       # Match the connected source-key chroma rule.  The separate 2-pixel
       # pair boundary limit prevents this stricter path from altering a plum
       # garment's interior.
       bright_key_like = min(p[0], p[2])-p[1] >= 65 and max(p[0], p[2]) >= 90
       if abs(hue_class(p)[0]-hue_class(fg)[0]) < 24 and abs(hue_class(p)[1]-hue_class(fg)[1]) < 36 and not (strict_key_spill and bright_key_like): continue
       # retain black outlines and skin-like colours even if a source pixel is noisy
       if max(p)-min(p)<28 or min(p[0],p[2])<p[1]: continue
       out.putpixel((x,y),fg); changed.append({'x':x,'y':y,'before':list(p),'after':list(fg),'nearestInterior':list(q)})
       draw.point((x,y),fill=(0,255,255))
    return out,changed,before
def component_points(alpha, roi, wanted, threshold=64):
    x0,y0,x1,y1=roi; pix=alpha.load(); seen=set()
    for sy in range(y0,y1):
      for sx in range(x0,x1):
       if (sx,sy) in seen or pix[sx,sy] < threshold: continue
       todo=[(sx,sy)]; seen.add((sx,sy)); pts=[]
       while todo:
        x,y=todo.pop(); pts.append((x,y))
        for nx,ny in ((x-1,y),(x,y-1),(x+1,y),(x,y+1),(x-1,y-1),(x+1,y-1),(x-1,y+1),(x+1,y+1)):
         if x0<=nx<x1 and y0<=ny<y1 and (nx,ny) not in seen and pix[nx,ny] >= threshold: seen.add((nx,ny)); todo.append((nx,ny))
       xs=[p[0] for p in pts]; ys=[p[1] for p in pts]; bounds=[min(xs),min(ys),max(xs)+1,max(ys)+1]
       if bounds==wanted: return pts
    raise ValueError(f'V3 selected component {wanted} cannot be reconstructed')
def tile_from_component(master, alpha, frame, scale):
    x0,y0,x1,y1=frame['selectedComponent']['bounds']; points=component_points(alpha,frame['expandedRoi'],frame['selectedComponent'].get('coreBounds',frame['selectedComponent']['bounds']))
    point_set=set(points)
    tile=master.crop((x0,y0,x1,y1)); mask=Image.new('L',tile.size,0)
    halo={(px+dx,py+dy) for px,py in point_set for dx in range(-2,3) for dy in range(-2,3)}
    def kept(gx,gy):
        if alpha.getpixel((gx,gy)) == 0: return 0
        return alpha.getpixel((gx,gy)) if (gx,gy) in halo else 0
    mask.putdata([kept(x0+(i%tile.width),y0+(i//tile.width)) for i in range(tile.width*tile.height)])
    tile.putalpha(mask); return tile.resize((round(tile.width*scale),round(tile.height*scale)),Image.Resampling.NEAREST)
def reviewed_hair_calibration(source, table_path, outfit, hair, kind):
    table=json.loads(table_path.read_text(encoding='utf-8'))
    actual=sha256(source)
    return next((x for x in table['entries'] if x['sourceSha256']==actual and x.get('costumeId',outfit)==outfit and x.get('hairId',hair)==hair and x.get('kind',kind)==kind),None)
def source_filename(entry, kind):
    name=entry.get('sourceFileName',f'{kind}-source-v1.png')
    if Path(name).name != name or not re.fullmatch(rf'{re.escape(kind)}-source-v[1-9][0-9]*\.png',name):
        raise ValueError(f'unsafe reviewed source file name: {name!r}')
    return name
def build(outfit,kind,filename,source_override=None, calibration_table=None, hair=None):
    source=source_override or ROOT/'images/sprites/costumes-v2/raw'/filename; raw,alpha,_=source_rgb_alpha(source,broad=True); rgb,changes,overlay=clean(raw,alpha); master=clean_master(rgb,alpha)
    hair_root=OUT/outfit/source.parts[-3] if source_override is not None else None
    if source_override is not None and hair is None:
        hair=source.parts[-3]
    v3=(OUT/outfit/'source-reference-v3-components'/kind) if hair_root is None else (hair_root/'source-reference-v3-components'/kind)
    record=json.loads((v3/'diagnostic.json').read_text(encoding='utf-8')); dest=(OUT/outfit/'source-reference-v4-matte'/kind) if hair_root is None else (hair_root/'source-reference-v4-matte'/kind); dest.mkdir(parents=True,exist_ok=True)
    master.save(dest/'transparent-clean-master.png'); overlay.save(dest/'matte-changed-pixel-overlay.png')
    calibration=reviewed_hair_calibration(source, calibration_table, outfit, hair, kind) if source_override is not None else None
    if source_override is not None and calibration is None:
        raise ValueError(f'no reviewed hash-bound calibration for {source}')
    if calibration and calibration.get('identityDisposition','') == 'rejected_wrong_hair':
        return
    scale=64.0/(calibration['sourceSoleY']-calibration['sourceScalpY']) if calibration else record['frames'][0]['scale']*2
    atlas=Image.new('RGBA',(768,512))
    for f in record['frames']:
      x0,y0,x1,y1=f['selectedComponent']['bounds']; tile=tile_from_component(master,alpha,f,scale); col,row=f['column'],f['row']; ax,ay=([col*CELL+calibration['sourceAnchor'][0],row*CELL+calibration['sourceAnchor'][1]] if calibration else f['registeredSourceAnchor']); atlas.alpha_composite(tile,(round(col*128+64+(x0-ax)*scale),round(row*128+104+(y0-ay)*scale)))
    atlas.save(dest/'native-atlas-64body-6x4.png'); checker_contact(atlas,dest/'contact-checker-4x.png')
    occupied=[sum(1 for pixel in atlas.crop((c*128,r*128,(c+1)*128,(r+1)*128)).getdata() if pixel[3]) for r in range(4) for c in range(6)]
    if any(count == 0 for count in occupied): raise ValueError(f'{source}: an output cell is empty')
    for row in range(4): preview_gif(atlas,row,1,dest/f'row-{row+1}-native.gif',128); preview_gif(atlas,row,4,dest/f'row-{row+1}-4x.gif',128)
    stable_write(dest/'diagnostic.json',{'label':'diagnostic candidate v4 — boundary matte decontamination — not Accepted','acceptanceState':'source_only','source':str(source.relative_to(ROOT)).replace('\\','/'),'sourceSha256':sha256(source),'method':'only alpha-boundary chromatic spill; nearest non-spill interior RGB replacement; authored same-hue plum, black outlines and skin-like pixels skipped','reviewedHairCalibration':calibration,'sheetScale64':scale,'outputCellOpaquePixelCounts':occupied,'changedPixelCount':len(changes),'changedPixels':changes,'outputs':{'masterSha256':sha256(dest/'transparent-clean-master.png'),'atlas64Sha256':sha256(dest/'native-atlas-64body-6x4.png')}})
def main():
 import argparse; p=argparse.ArgumentParser(); p.add_argument('--process-hair-sources',action='store_true'); p.add_argument('--outfit',choices=('a1','a2','c2','d3','v1','v2','v3','v4','t1','t2','t3','t4','l1','l2','l3','l4','b1','b2','b3','b4')); p.add_argument('--hair',action='append',choices=tuple(f'h{i}' for i in range(1,13))); p.add_argument('--kind',choices=('movement','combat','village'),default='movement'); p.add_argument('--calibration-table',type=Path); a=p.parse_args()
 if a.process_hair_sources:
  if not a.outfit or not a.calibration_table:
   raise ValueError('--process-hair-sources requires --outfit and --calibration-table')
  table=json.loads(a.calibration_table.read_text(encoding='utf-8'))
  for hair in (a.hair or tuple(f'h{i}' for i in range(1,13))):
   entry=next((item for item in table.get('entries',[]) if item.get('hairId')==hair and item.get('costumeId',a.outfit)==a.outfit and item.get('kind',a.kind)==a.kind),None)
   if entry is None: raise ValueError(f'no reviewed calibration entry for {a.outfit}/{hair}')
   source=OUT/a.outfit/hair/'raw'/source_filename(entry,a.kind)
   if source.exists(): build(a.outfit,a.kind,source.name,source,a.calibration_table,hair)
  return
 sources={'a1':(('movement','a1-movement-v2.png'),('combat','a1-combat.png'),('village','a1-village.png')),'a2':(('movement','a2-movement-v2.png'),('combat','a2-combat.png'),('village','a2-village.png')),'c2':(('movement','c2-movement-v2.png'),('combat','c2-combat.png'),('village','c2-village.png')),'d3':(('movement','d3-movement-v3.png'),('combat','d3-combat.png'),('village','d3-village.png'))}
 for outfit,entries in sources.items():
  for kind,file in entries: build(outfit,kind,file)
if __name__=='__main__': main()
