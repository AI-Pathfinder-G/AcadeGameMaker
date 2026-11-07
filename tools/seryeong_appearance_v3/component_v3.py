"""V3 component proposal: expanded ROIs prevent grid-edge hair/boot clipping.

Diagnostic only.  It writes beside v1/v2 and never declares mask or anchors
reviewed.  A component is selected by area plus proximity to its expected
torso; every selection is visualised for human review.
"""
from pathlib import Path
import argparse, json, re, shutil
from PIL import Image, ImageDraw
try:
 from .build_diagnostic import ROOT, OUT, SOURCES, CELL, TARGET_CELL, TARGET_ANCHOR, source_rgb_alpha, clean_master, sha256, stable_write, preview_gif, checker_contact
except ImportError:  # Direct `python component_v3.py` invocation.
 from build_diagnostic import ROOT, OUT, SOURCES, CELL, TARGET_CELL, TARGET_ANCHOR, source_rgb_alpha, clean_master, sha256, stable_write, preview_gif, checker_contact

PAD_X, PAD_Y = 32, 48

CORE_ALPHA = 64

def comps(alpha, roi, threshold=CORE_ALPHA):
    x0,y0,x1,y1=roi; w,h=alpha.size; pix=alpha.load(); seen=set(); out=[]
    for sy in range(max(0,y0),min(h,y1)):
      for sx in range(max(0,x0),min(w,x1)):
       if (sx,sy) in seen or pix[sx,sy] < threshold: continue
       todo=[(sx,sy)]; seen.add((sx,sy)); pts=[]
       while todo:
        x,y=todo.pop(); pts.append((x,y))
        for nx,ny in ((x-1,y),(x,y-1),(x+1,y),(x,y+1),(x-1,y-1),(x+1,y-1),(x-1,y+1),(x+1,y+1)):
         if x0<=nx<x1 and y0<=ny<y1 and 0<=nx<w and 0<=ny<h and (nx,ny) not in seen and pix[nx,ny] >= threshold: seen.add((nx,ny)); todo.append((nx,ny))
       xs=[p[0] for p in pts]; ys=[p[1] for p in pts]; core=[min(xs),min(ys),max(xs)+1,max(ys)+1]
       out.append({"area":len(pts),"bounds":[max(x0,core[0]-2),max(y0,core[1]-2),min(x1,core[2]+2),min(y1,core[3]+2)],"coreBounds":core})
    return out

def choose(items, cx, cy):
    candidates=[x for x in items if x['area']>=20]
    if not candidates: return None
    def score(item):
      x0,y0,x1,y1=item['bounds']; mx=(x0+x1)/2; my=(y0+y1)/2
      return ((mx-cx)**2+(my-cy)**2)**.5 - min(item['area'],10000)*.003
    return min(candidates,key=score)

def one(outfit,kind,file,root,source_override=None,prior_override=None,dest_override=None,calibration=None):
    source=source_override or ROOT/'images/sprites/costumes-v2/raw'/file; rgb,alpha,diag=source_rgb_alpha(source, broad=True); master=clean_master(rgb,alpha)
    dest=dest_override or root/outfit/'source-reference-v3-components'/kind; dest.mkdir(parents=True,exist_ok=True); master.save(dest/'transparent-clean-master.png')
    if calibration:
        scale,base=calibration['scale32'],calibration['sourceAnchor']
    else:
        prior=prior_override or json.loads((root/outfit/'source-reference-v2-broad-fringe'/kind/'diagnostic.json').read_text(encoding='utf-8'))
        scale,base=prior['sheetUniformScale'],prior['anchorProposal']['source']
    atlas=Image.new('RGBA',(384,256)); overlay=rgb.convert('RGBA'); draw=ImageDraw.Draw(overlay); records=[]
    for row in range(4):
      for col in range(6):
       x,y=col*CELL,row*CELL; roi=(max(0,x-PAD_X),max(0,y-PAD_Y),min(1536,x+CELL+PAD_X),min(1024,y+CELL+PAD_Y)); items=comps(alpha,roi); selected=choose(items,x+128,y+120)
       if not selected: raise ValueError(f'no torso-associated component {source} {row},{col}')
       bx0,by0,bx1,by1=selected['bounds']; tile=master.crop((bx0,by0,bx1,by1)).resize((round((bx1-bx0)*scale),round((by1-by0)*scale)),Image.Resampling.NEAREST)
       registered=[x+base[0],y+base[1]]
       atlas.alpha_composite(tile,(round(col*64+TARGET_ANCHOR[0]+(bx0-registered[0])*scale),round(row*64+TARGET_ANCHOR[1]+(by0-registered[1])*scale)))
       draw.rectangle(selected['bounds'],outline=(0,255,255,255),width=2); draw.rectangle(roi,outline=(255,220,0,255),width=1)
       records.append({'row':row,'column':col,'expandedRoi':list(roi),'components':items,'selectedComponent':selected,'registeredSourceAnchor':registered,'targetAnchor':[32,52],'scale':scale})
    overlay.save(dest/'component-overlay.png'); atlas.save(dest/'native-atlas-32body-6x4.png'); atlas.resize((1536,1024),Image.Resampling.NEAREST).save(dest/'contact-4x.png'); checker_contact(atlas,dest/'contact-checker-4x.png')
    for row in range(4): preview_gif(atlas,row,1,dest/f'row-{row+1}-native.gif',64); preview_gif(atlas,row,4,dest/f'row-{row+1}-4x.gif',64)
    density=Image.new('RGBA',(768,512))
    for frame in records:
       row,col=frame['row'],frame['column']; bx0,by0,bx1,by1=frame['selectedComponent']['bounds']; registered=frame['registeredSourceAnchor']; scale64=scale*2
       tile=master.crop((bx0,by0,bx1,by1)).resize((round((bx1-bx0)*scale64),round((by1-by0)*scale64)),Image.Resampling.NEAREST)
       density.alpha_composite(tile,(round(col*128+64+(bx0-registered[0])*scale64),round(row*128+104+(by0-registered[1])*scale64)))
    density.save(dest/'native-atlas-64body-6x4.png')
    density.save(dest/'density-study-64body-6x4.png')
    checker_contact(density,dest/'density-study-64body-checker-2x.png')
    for row in range(4): preview_gif(density,row,1,dest/f'density-row-{row+1}-native.gif',128); preview_gif(density,row,4,dest/f'density-row-{row+1}-4x.gif',128)
    comparison=Image.new('RGBA',(3072,1024),(72,78,88,255)); left=Image.open(dest/'contact-checker-4x.png'); right=Image.open(dest/'density-study-64body-checker-2x.png'); comparison.alpha_composite(left,(0,0)); comparison.alpha_composite(right,(1536,0)); comparison.save(dest/'density-comparison-checker.png')
    stable_write(dest/'diagnostic.json',{'label':'diagnostic candidate v3 — component proposal — not Accepted','acceptanceState':'source_only','source':str(source.relative_to(ROOT)).replace('\\','/'),'sourceSha256':sha256(source),'diagnostic':diag,'componentSelection':'expanded ROI, 8-connected components, torso proximity/area proposal; manual review required','frames':records,'standingCalibration':calibration or {'status':'unreviewed_alpha_bbox_candidate'},'preferredOfflineProfile':{'bodyHeight':64,'cell':[128,128],'baseline':104,'authority':'user choice 2026-09-13: detail expression prioritised','status':'offline candidate only; manual scalp-to-sole reference remains pending'},'densityStudy':{'status':'retained_32px_comparison','bodyHeight':32,'cell':64,'doesNotAdopt':['18 PPU','gameplay cell','camera','collider','runtime pivot']},'outputs':{'comparisonAtlas32Sha256':sha256(dest/'native-atlas-32body-6x4.png'),'preferredAtlas64Sha256':sha256(dest/'native-atlas-64body-6x4.png'),'overlaySha256':sha256(dest/'component-overlay.png')}})

def load_calibration(path, root):
    if path is None:
        return None
    data=json.loads(path.read_text(encoding='utf-8'))
    required=('approved','reviewer','sourceScalpY','sourceSoleY','sourceAnchorX','sourceAnchorY')
    if not all(key in data for key in required) or data['approved'] is not True:
        raise ValueError('reviewed calibration must be approved and provide reviewer, scalp/sole and source anchor coordinates')
    height=data['sourceSoleY']-data['sourceScalpY']
    if height <= 0:
        raise ValueError('sourceSoleY must be greater than sourceScalpY')
    return {'status':'reviewed_manual_scalp_to_sole','reviewer':data['reviewer'],'sourceScalpY':data['sourceScalpY'],'sourceSoleY':data['sourceSoleY'],'sourceAnchor':[data['sourceAnchorX'],data['sourceAnchorY']],'scale32':32.0/height}

def calibration_table(path):
    table=json.loads(path.read_text(encoding='utf-8'))
    entries=table.get('entries')
    if not isinstance(entries,list):
        raise ValueError('reviewed calibration table must contain entries')
    required={'hairId','sourceSha256','sourceScalpY','sourceSoleY','sourceAnchor'}
    if any(not required.issubset(entry) for entry in entries):
        raise ValueError('each reviewed calibration entry requires hairId, source hash, scalp/sole and sourceAnchor')
    return entries

def reviewed_calibration(source, entries, outfit, hair, kind):
    actual=sha256(source)
    entry=next((item for item in entries if item['sourceSha256']==actual and item.get('costumeId',outfit)==outfit and item.get('hairId',hair)==hair and item.get('kind',kind)==kind),None)
    if entry is None:
        raise ValueError(f'no reviewed hash-bound calibration for {source}')
    height=entry['sourceSoleY']-entry['sourceScalpY']
    if height <= 0 or len(entry['sourceAnchor']) != 2:
        raise ValueError(f'invalid reviewed calibration for {source}')
    return {**entry,'status':'reviewed_manual_scalp_to_sole','reviewer':entry.get('reviewer','table reviewer'),'scale32':32.0/height}

def source_filename(entry, kind):
    name=entry.get('sourceFileName',f'{kind}-source-v1.png')
    if Path(name).name != name or not re.fullmatch(rf'{re.escape(kind)}-source-v[1-9][0-9]*\.png',name):
        raise ValueError(f'unsafe reviewed source file name: {name!r}')
    return name

def process_hair_sources(root, outfit, entries, hairs=None, kind='movement'):
    prior_path=root/outfit/f'source-reference-v2-broad-fringe/{kind}/diagnostic.json'
    prior=json.loads(prior_path.read_text(encoding='utf-8')) if prior_path.exists() else None
    for hair in (hairs or tuple(f'h{i}' for i in range(1,13))):
        entry=next((item for item in entries if item['hairId']==hair and item.get('costumeId',outfit)==outfit and item.get('kind',kind)==kind),None)
        if entry is None:
            raise ValueError(f'no reviewed calibration entry for {outfit}/{hair}')
        source=root/outfit/hair/'raw'/source_filename(entry,kind)
        if source.exists():
            calibration=reviewed_calibration(source,entries,outfit,hair,kind)
            if calibration.get('identityDisposition','') == 'rejected_wrong_hair':
                continue
            one(outfit,kind,source.name,root,source_override=source,prior_override=prior,dest_override=root/outfit/hair/f'source-reference-v3-components/{kind}',calibration=calibration)

def main():
 p=argparse.ArgumentParser();p.add_argument('--output',type=Path,default=OUT);p.add_argument('--process-hair-sources',action='store_true');p.add_argument('--outfit',choices=('a1','a2','c2','d3','v1','v2','v3','v4','t1','t2','t3','t4','l1','l2','l3','l4','b1','b2','b3','b4'));p.add_argument('--hair',action='append',choices=tuple(f'h{i}' for i in range(1,13)));p.add_argument('--kind',choices=('movement','combat','village'),default='movement');p.add_argument('--calibration-table',type=Path);p.add_argument('--reviewed-calibration',type=Path);a=p.parse_args(); calibration=load_calibration(a.reviewed_calibration,a.output)
 if a.process_hair_sources:
  if not a.outfit or not a.calibration_table:
   raise ValueError('--process-hair-sources requires --outfit and --calibration-table')
  process_hair_sources(a.output,a.outfit,calibration_table(a.calibration_table),a.hair,a.kind); return
 for outfit,entries in SOURCES.items():
  target=a.output/outfit/'source-reference-v3-components'
  if target.exists(): shutil.rmtree(target)
  for kind,file in entries: one(outfit,kind,file,a.output,calibration=calibration)
if __name__=='__main__': main()
