'use strict';

const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');

const ROOT = path.resolve(__dirname, '../../..');
const PRODUCTION = path.join(ROOT, 'images', 'sprites', 'production-v1');
const PLAN_PATH = path.join(PRODUCTION, 'generation-plan.json');
const PLAN_SIZE = Object.freeze({ width: 1536, height: 1024, columns: 6, sheets: 18 });

function sha256File(file) { return crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex'); }
function readPlan(planPath = PLAN_PATH) { return JSON.parse(fs.readFileSync(planPath, 'utf8')); }
function rowId(row) { return String(row).split(':', 1)[0].trim(); }
function rawPath(job, production = PRODUCTION) {
  // A regenerated v2 remains beside the immutable first attempt. Prefer it only
  // when it is present; its own SHA-256 still binds any later review record.
  const revised = path.join(production, 'raw', `${job.id}-v2.png`);
  return fs.existsSync(revised) ? revised : path.join(production, 'raw', `${job.id}.png`);
}
function defaultEdges(size, count) { return Array.from({ length: count + 1 }, (_, i) => Math.floor(i * size / count)); }
function sortObject(value) {
  if (Array.isArray(value)) return value.map(sortObject);
  if (value && typeof value === 'object') return Object.fromEntries(Object.keys(value).sort().map(k => [k, sortObject(value[k])]));
  return value;
}
function stableJson(value) { return `${JSON.stringify(sortObject(value), null, 2)}\n`; }
function validateEdges(edges, expectedLast, label) {
  if (!Array.isArray(edges) || edges.length < 2 || !edges.every(Number.isInteger) || edges[0] !== 0 || edges.at(-1) !== expectedLast)
    return `${label} must be complete integer edges from 0 through ${expectedLast}`;
  if (edges.some((value, index) => index > 0 && value <= edges[index - 1])) return `${label} must be strictly increasing`;
  return null;
}
function validatePlan(plan) {
  const errors = [];
  if (!plan || !Array.isArray(plan.jobs) || plan.jobs.length !== PLAN_SIZE.sheets) errors.push(`expected exactly ${PLAN_SIZE.sheets} jobs`);
  const ids = new Set();
  for (const job of plan.jobs || []) {
    if (!/^\d{2}-[a-z0-9-]+$/.test(job.id || '')) errors.push(`invalid job id: ${job.id}`);
    if (ids.has(job.id)) errors.push(`duplicate job id: ${job.id}`); ids.add(job.id);
    if (job.cols !== PLAN_SIZE.columns) errors.push(`${job.id}: expected six columns`);
    if (!Array.isArray(job.rows) || job.rows.length !== job.rowCount) errors.push(`${job.id}: rows and rowCount disagree`);
    for (const row of job.rows || []) if (!rowId(row)) errors.push(`${job.id}: row lacks action id`);
  }
  return errors;
}
function loadReviewConfig(file) {
  if (!file || !fs.existsSync(file)) return { version: 1, sheets: [] };
  const config = JSON.parse(fs.readFileSync(file, 'utf8'));
  return { ...config, sheets: Array.isArray(config.sheets) ? config.sheets : [] };
}
function reviewedGrid(job, sourceHash, config) {
  const record = config.sheets.find(item => item.jobId === job.id && item.sourceSha256 === sourceHash);
  if (!record || record.approved !== true || !record.reviewer || !record.reason) return { ok: false, reason: 'no reviewed extraction record bound to this source hash' };
  const xError = validateEdges(record.xEdges, PLAN_SIZE.width, `${job.id}.xEdges`);
  const yError = validateEdges(record.yEdges, PLAN_SIZE.height, `${job.id}.yEdges`);
  if (xError || yError || record.xEdges.length !== job.cols + 1 || record.yEdges.length !== job.rowCount + 1)
    return { ok: false, reason: xError || yError || 'edge count does not match declared row/column count' };
  return { ok: true, record };
}
module.exports = { ROOT, PRODUCTION, PLAN_PATH, PLAN_SIZE, sha256File, readPlan, rowId, rawPath, defaultEdges, stableJson, validateEdges, validatePlan, loadReviewConfig, reviewedGrid };
