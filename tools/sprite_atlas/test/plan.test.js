'use strict';
const test = require('node:test'); const assert = require('node:assert/strict');
const { readPlan, validatePlan, defaultEdges, validateEdges, reviewedGrid } = require('../lib/plan');
test('AC-SPR-002: generation plan declares exactly eighteen six-pose sheet jobs', () => { const plan=readPlan(); assert.deepEqual(validatePlan(plan),[]); assert.equal(plan.jobs.length,18); for(const job of plan.jobs){assert.equal(job.cols,6);assert.equal(job.rows.length,job.rowCount);} });
test('AC-SPR-001: floor-derived edges are complete monotonic integer bounds', () => { const edges=defaultEdges(1024,3);assert.deepEqual(edges,[0,341,682,1024]);assert.equal(validateEdges(edges,1024,'rows'),null); });
test('AC-SPR-006: unreviewed or hash-mismatched grids fail closed', () => { const job={id:'01-doeon-idle',cols:6,rowCount:3};assert.equal(reviewedGrid(job,'a'.repeat(64),{sheets:[]}).ok,false);assert.equal(reviewedGrid(job,'a'.repeat(64),{sheets:[{jobId:job.id,sourceSha256:'b'.repeat(64),approved:true,reviewer:'r',reason:'x',xEdges:[0,256,512,768,1024,1280,1536],yEdges:[0,341,682,1024]}]}).ok,false); });
