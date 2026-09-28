# M5D7M R62 runner-boundary observer evidence

- Date: 2026-09-28 (Asia/Seoul)
- Contract: `2026-09-27-m5d7m-duplicate-liveness-recovery.md`, R62
- Traceability: `REQ-M5D7M-R62-001/002`; `AC-M5D7M-R62-002/003`

## Result

The one authorized Unity 6000.6.0f1 PlayMode run used the exact R60 selection,
PID `18032`, no `-quit`, no version probe, and no retry. It exited naturally
with exit code `0` at `03:42:42 KST`; no watchdog stop occurred.

`m5d7m-r62-run-boundary.xml` reports `990/990` passed, with `0` failed,
skipped, and inconclusive; duration `1293.7224278s`. XML SHA-256:
`31D3552DF1B972F0F215C9E4120E907DAF3DD757B8FD8B478ABC111A2412E57B`.
The XML contains the expected 990-test selected fixture inventory. The editor
log is `393233` bytes, SHA-256
`507CF4021054211ACEA28A38FBD069F1AACA03E0F73CEFA73B7A24A2B11DD44C`.

## Boundary observation

The observer recorded D4 `started`, the expected reservation exception, then
D4 `finished Passed`. It then recorded Handoff fixture `started`, five case
starts, five case `finished Passed` events, and Handoff fixture `finished
Passed`. This run therefore observed the previously missing runner boundary
and subsequent selected fixture completion.

The observer source/meta hashes were respectively
`5F4539E497083D0481329ECCFD8703FC531E9880BE68991306549EE472ABF72E` and
`0D3236308F51C4FB60E3F496AF7E938C386D4DBE172271BC87E92A3633A55A3C`.
The four CUA baseline hashes remained unchanged. No new R62
`InitTestScene` pair was observed; the prior R61 pair remains preserved.

## Boundary

The evidence supports a timing-sensitive runner-boundary observation only; it
does not identify a production or fixture defect. `AC-CUA-009` remains open.
The temporary observer remains present pending Astra's removal instruction.
