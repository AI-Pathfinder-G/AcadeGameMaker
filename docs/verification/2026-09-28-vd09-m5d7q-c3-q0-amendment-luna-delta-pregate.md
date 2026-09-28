# C3 Q0 successor amendment — Luna delta pre-gate

Date: 2026-09-28. Read-only contract delta review; no Q0/source/test/Unity
changes and no implementation acceptance.

## Reviewed boundary

The current approved C3 contract SHA-256 is
`8B17E87DB947F08095EBE8525E9F50D7C5D6F1187804441F99BAF4AE551D9BD2`.
The Astra amendment adds only a conditional Q0 evidence-maintenance lane under
`AC-M5D7QC3-009/010`: after C2/C2R acceptance on unchanged source, Terra may
implement the allowlisted C3 Adapter accessor; Luna must then review the exact
resulting Adapter source; Astra must separately approve its exact SHA-256 and
C3 provenance; only then may Q0 replace the current Adapter assertion with one
strict C3 successor row.

The delta matches the existing C3 forward review. All nine historical rows,
seven strict legacy-current rows, the accepted C2/C2R Adapter hash
`0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`, and the
sole strict current Router hash
`66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB` remain
immutable/strict. There is no old-or-new fallback, hash range, alias, history
rewrite, runtime-authority change, new assembly, or product change. The C3
surface remains synthetic-only: it observes C1 and cannot call C1 Begin or
execute C2.

## Implementation failure checks

Before any Q0 edit, Luna must verify at least these five failure boundaries:

1. The accessor changes only the allowlisted Adapter getter; no C1 Begin,
   C2 call, barrier removal, destination/menu/scene/gameplay/map authority, or
   durable save mutation is introduced.
2. The exact resulting Adapter file is independently hashed and mapped to C3
   provenance; no predecessor hash is accepted as a compatibility fallback.
3. Q0 retains every historical and legacy row and adds exactly one C3 Adapter
   successor, with Router remaining the sole strict current Router assertion.
4. Test selection uses the actual source namespace
   `AcadeGameMaker.Tests.EditMode.InputUnity`; a passing predecessor suite must
   not stand in for an unexecuted C3 successor.
5. C3 negative paths remain terminal/closed: foreign or skipped ticks,
   corrupt/partial successor witnesses, stale confirmation, rearm misuse, and
   any path attempting C1 before the execution boundary cannot promote or
   retry through an alternate authority.

## Verdict

**Delta pre-gate: PASS, P0=0, P1=0 for the amendment’s boundary only.** C3
implementation is not yet accepted and no runtime or Q0 pass is claimed.
The required order remains: C2/C2R accepted → Terra Adapter getter → Luna
exact source review → Astra exact-hash approval → one strict Q0 pin update.
