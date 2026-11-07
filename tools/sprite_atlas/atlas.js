#!/usr/bin/env node
'use strict';

// REQ-SPR-001/006/007/010. Offline-only; no Unity file is ever addressed here.
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const { ROOT, PRODUCTION, PLAN_SIZE, sha256File, readPlan, rowId, rawPath, defaultEdges, stableJson, validatePlan, loadReviewConfig, reviewedGrid } = require('./lib/plan');
const png = require('./lib/png');
const command = process.argv[2];
const has = flag => process.argv.includes(flag);
const value = flag => { const index = process.argv.indexOf(flag); return index < 0 ? null : process.argv[index + 1]; };
const label = 'offline draft — pending visual acceptance';
const defects = JSON.parse(fs.readFileSync(path.join(__dirname, 'known-defects.json'), 'utf8')).notes;
const known = id => defects.find(note => note.jobId === id);
function writeJson(file, value) { fs.mkdirSync(path.dirname(file), { recursive: true }); fs.writeFileSync(file, stableJson(value), 'utf8'); }
function inspect() {
  const plan = readPlan(); const errors = validatePlan(plan); const sheets = plan.jobs.map(job => {
    const raw = rawPath(job); const defect = known(job.id);
    if (!fs.existsSync(raw)) return { id: job.id, raw: path.relative(ROOT, raw).replaceAll('\\','/'), disposition: 'pending', reason: 'raw source has not arrived', expected: { width: PLAN_SIZE.width, height: PLAN_SIZE.height, columns: job.cols, rows: job.rowCount, sourcePosesPerRow: 6 } };
    const sourceSha256 = sha256File(raw); let dimensions;
    try { const image = png.decode(fs.readFileSync(raw)); dimensions = { width: image.width, height: image.height, colorModel: 'RGBA after decode' }; } catch (error) { return { id:job.id, raw:path.relative(ROOT,raw).replaceAll('\\','/'), sourceSha256, disposition:'needs-review', reason:`unreadable raw PNG: ${error.message}` }; }
    const wrongCanvas = dimensions.width !== PLAN_SIZE.width || dimensions.height !== PLAN_SIZE.height;
    return { id: job.id, raw: path.relative(ROOT, raw).replaceAll('\\','/'), sourceSha256, dimensions, defaultFloorEdges: { xEdges: defaultEdges(PLAN_SIZE.width, job.cols), yEdges: defaultEdges(PLAN_SIZE.height, job.rowCount) }, rows: job.rows.map(rowId), disposition: defect || wrongCanvas ? 'needs-review' : 'pending_review', reason: defect?.reason || (wrongCanvas ? 'wrong raw canvas' : 'requires reviewed extraction bounds and visual acceptance') };
  });
  const report = { schemaVersion: 1, label, requirementIds: ['REQ-SPR-001','REQ-SPR-006','REQ-SPR-007','REQ-SPR-010'], planErrors: errors, expectedSheetCount: PLAN_SIZE.sheets, sheets };
  const file = value('--report') || path.join(PRODUCTION, 'raw-inspection.json'); writeJson(file, report); process.stdout.write(`${file}\n`);
  return report;
}
function cellProfile(job) {
  if (job.id === '04-lovers-interactions') return { width:96,height:64,bodyHeight:32,baseline:52,pivot:[48,52],group:true };
  if (job.id === '05-lovers-shop') return { width:128,height:64,bodyHeight:32,baseline:52,pivot:[64,52],group:true };
  if (job.id === '18-boss-creatures') return { width:192,height:128,bodyHeight:88,baseline:104,pivot:[96,104],group:false };
  if (['12-seoha-boss','13-ordan-combat','14-taegan-combat','15-chaeryun-combat','16-mujin-combat','17-ragen-combat'].includes(job.id)) return { width:128,height:128,bodyHeight:64,baseline:104,pivot:[64,104],group:false };
  return { width:64,height:64,bodyHeight:32,baseline:52,pivot:[32,52],group:false };
}
function sourceCitation(job) {
  const combat = /combat|movement|boss|creatures/.test(job.id);
  if (!combat) return { authority:'role/interaction plan', citation:'generation-plan.json', status:'offline-draft' };
  if (job.id === '10-doeon-combat') return { authority:'Approved mechanic', citation:'docs/specs/vertical-demo/02-weight-transfer.md; docs/specs/vertical-demo/03-combat-and-enemies.md', status:'offline-draft' };
  if (job.id === '13-ordan-combat') return { authority:'Approved mechanic', citation:'docs/specs/work-contracts/2026-09-05-vd03-combat-m4a-ordan-boss-core.md', status:'offline-draft' };
  return { authority:'visual proposal only', citation:'docs/proposals/2026-09-09-eight-chapter-synopsis-realignment.md', status:'combat_pending' };
}
// Preview provenance only. These values do not specify hit frames, hurt boxes,
// collision, target geometry, runtime playback, or any gameplay behavior.
function timingFor(job, animationId) {
  const visual = { class: 'visual-proposal', note: 'Integer preview ticks only; not runtime timing or hit geometry.', frames: [6,6,6,6,6,6] };
  if (job.id === '10-doeon-combat') {
    if (animationId === 'basic_attack') return { class:'Approved-mechanic reference', citation:'docs/specs/vertical-demo/03-combat-and-enemies.md REQ-COM-001', note:'21 ticks cited for offline preview provenance; no hit geometry is declared.', declaredActionTicks:21 };
    if (animationId === 'compression_verdict') return { class:'Approved-mechanic reference', citation:'docs/specs/vertical-demo/06-humanity-choice-and-narrative.md REQ-CHOICE-006', note:'36-tick delay cited for offline preview provenance; no hit geometry is declared.', declaredDelayTicks:36 };
    if (animationId === 'shared_plane') return { class:'Approved-mechanic reference', citation:'docs/specs/vertical-demo/06-humanity-choice-and-narrative.md REQ-CHOICE-007', note:'Contract citation for offline preview provenance; no hit geometry is declared.' };
  }
  if (job.id === '13-ordan-combat') {
    const source='docs/specs/work-contracts/2026-09-05-vd03-combat-m4a-ordan-boss-core.md REQ-COM-006';
    if (animationId === 'debt_line') return { class:'Approved-mechanic reference', citation:source, note:'Telegraph/execute/recovery preview phase ticks; no hit geometry is declared.', phases:[48,21,39] };
    if (animationId === 'seizure_weight') return { class:'Approved-mechanic reference', citation:source, note:'Telegraph/execute/recovery preview phase ticks; execute is up to 72 ticks; no hit geometry is declared.', phases:[60,'up to 72',48] };
    if (animationId === 'balance_audit') return { class:'Approved-mechanic reference', citation:source, note:'Telegraph/execute/recovery preview phase ticks; no hit geometry is declared.', phases:[60,90,48] };
  }
  return visual;
}
function reject(job, raw, sourceSha256, reason, quarantineRoot) {
  const date = new Date().toISOString().slice(0,10); const folder = path.join(quarantineRoot, date, job.id); fs.mkdirSync(folder,{recursive:true}); const destination=path.join(folder,path.basename(raw));
  if (!fs.existsSync(destination)) fs.copyFileSync(raw,destination);
  writeJson(path.join(folder,'disposition.json'), { label, jobId:job.id, source:path.relative(ROOT,raw).replaceAll('\\','/'), sourceSha256, disposition:'quarantined', reason });
  return { id:job.id, disposition:'quarantined', reason, sourceSha256 };
}
function paste(destination, source, x, y) { for(let row=0;row<source.height;row++) source.pixels.copy(destination.pixels,((y+row)*destination.width+x)*4,row*source.width*4,(row+1)*source.width*4); }
function pack() {
  if (!has('--confirm-user-image-processing')) throw new Error('Image processing is blocked pending explicit user permission. Re-run only after approval with --confirm-user-image-processing.');
  const reviewPath=value('--review-config'); if (!reviewPath) throw new Error('A reviewed extraction configuration is required.');
  const review=loadReviewConfig(reviewPath), plan=readPlan(), output=value('--output') || path.join(PRODUCTION,'offline-draft'), quarantine=value('--quarantine') || path.join(PRODUCTION,'quarantine');
  if (fs.existsSync(output)) throw new Error(`Refusing to overwrite existing production output: ${output}`);
  const accepted=[], rejected=[];
  for(const job of plan.jobs){ const raw=rawPath(job); if(!fs.existsSync(raw)){rejected.push({id:job.id,disposition:'pending',reason:'raw source has not arrived'});continue;} const hash=sha256File(raw), defect=known(job.id), grid=reviewedGrid(job,hash,review);
    if(defect || !grid.ok){rejected.push(reject(job,raw,hash,defect?.reason || grid.reason,quarantine));continue;}
    const image=png.magentaToAlpha(png.decode(fs.readFileSync(raw))); if(image.width!==PLAN_SIZE.width||image.height!==PLAN_SIZE.height){rejected.push(reject(job,raw,hash,'wrong raw canvas',quarantine));continue;}
    const profile=cellProfile(job), sheet={width:profile.width*job.cols,height:profile.height*job.rowCount,pixels:Buffer.alloc(profile.width*job.cols*profile.height*job.rowCount)}, frames=[]; let failed=null;
    for(let row=0;row<job.rowCount&&!failed;row++) for(let col=0;col<job.cols;col++){ const sourceRect={x:grid.record.xEdges[col],y:grid.record.yEdges[row],width:grid.record.xEdges[col+1]-grid.record.xEdges[col],height:grid.record.yEdges[row+1]-grid.record.yEdges[row]}; const cropped=png.crop(image,sourceRect), bounds=png.alphaBounds(cropped);
      if(!bounds){failed=`empty source rectangle at row ${row}, frame ${col}`;break;} const art=png.crop(cropped,bounds), scale=profile.bodyHeight/art.height;
      if(!Number.isFinite(scale)||scale<=0){failed='invalid uniform scale';break;} const scaled=png.nearest(art,Math.max(1,Math.round(art.width*scale)),profile.bodyHeight); if(scaled.width>profile.width||scaled.height>profile.height){failed=`normalized sprite exceeds ${profile.width}x${profile.height} cell`;break;}
      const dx=Math.floor((profile.width-scaled.width)/2), dy=profile.baseline-scaled.height; if(dy<0){failed='normalized sprite exceeds baseline cell';break;} paste(sheet,scaled,col*profile.width+dx,row*profile.height+dy);
      frames.push({rowIndex:row,animationId:rowId(job.rows[row]),frameIndex:col,sourceRect,rect:{x:col*profile.width,y:row*profile.height,width:profile.width,height:profile.height},baseline:profile.baseline,pivot:profile.pivot,sourceArtBounds:bounds,uniformScale:scale,previewDurationTicks:6,loop:false});
    }
    if(failed){rejected.push(reject(job,raw,hash,failed,quarantine));continue;} const rel=`sheets/${job.id}.png`, target=path.join(output,rel);fs.mkdirSync(path.dirname(target),{recursive:true});const bytes=png.encode(sheet);fs.writeFileSync(target,bytes);
    accepted.push({id:job.id,label,disposition:'offline-draft',raw:path.relative(ROOT,raw).replaceAll('\\','/'),sourceSha256:hash,review:grid.record,profile,rows:job.rows.map(action => ({ animationId:rowId(action), sourcePoseCount:6, timingProvenance:timingFor(job,rowId(action)) })),frames,citation:sourceCitation(job),output:rel,outputSha256:crypto.createHash('sha256').update(bytes).digest('hex')});
  }
  const manifest={schemaVersion:1,label,requirementIds:['REQ-SPR-001','REQ-SPR-002','REQ-SPR-006','REQ-SPR-007','REQ-SPR-009','REQ-SPR-010'],generator:{name:'offline-sprite-atlas',version:'1.0.0',nearestNeighbor:true,magentaKey:'RGB(255,0,255)'},plannedSheets:PLAN_SIZE.sheets,accepted,rejected}; writeJson(path.join(output,'atlas-manifest.json'),manifest);process.stdout.write(`${path.join(output,'atlas-manifest.json')}\n`);
}
function validate() { const output=value('--output');if(!output)throw new Error('--output is required');const manifest=JSON.parse(fs.readFileSync(path.join(output,'atlas-manifest.json'),'utf8'));const failures=[];if(manifest.plannedSheets!==18)failures.push('manifest does not report 18 planned sheets');for(const sheet of manifest.accepted||[]){const bytes=fs.readFileSync(path.join(output,sheet.output));if(sha256File(path.join(output,sheet.output))!==sheet.outputSha256)failures.push(`${sheet.id}: output hash mismatch`);const image=png.decode(bytes);for(const frame of sheet.frames){if(frame.rect.x<0||frame.rect.y<0||frame.rect.x+frame.rect.width>image.width||frame.rect.y+frame.rect.height>image.height)failures.push(`${sheet.id}: out-of-grid frame`);}} const result={label,acceptanceCriteria:['AC-SPR-001','AC-SPR-002','AC-SPR-004','AC-SPR-006'],valid:failures.length===0,failures};writeJson(path.join(output,'validation.json'),result);process.stdout.write(`${result.valid?'valid':'invalid'}\n`);if(!result.valid)process.exitCode=1; }
try { if(command==='inspect') inspect(); else if(command==='validate-plan'){const errors=validatePlan(readPlan());process.stdout.write(`${errors.length?'invalid':'valid'}\n`);if(errors.length){process.stdout.write(errors.join('\n'));process.exitCode=1;}} else if(command==='pack') pack(); else if(command==='validate') validate(); else throw new Error('usage: atlas.js inspect|validate-plan|pack|validate'); } catch(error){process.stderr.write(`${error.message}\n`);process.exitCode=1;}
