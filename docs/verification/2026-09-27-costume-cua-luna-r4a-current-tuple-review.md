# Costume CUA R4-A current-tuple corruption narrow review

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Scope: narrow closure review of R3-P1-001 only; Unity was not run and no implementation file was modified.
- Approved amendment contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Prior R3 review SHA-256: `40B605DF2D6A440BD024F58E528E58C185EF0BE041DE216DBA2E39FB4146E008`
- Terra evidence reviewed: `docs/verification/2026-09-20-costume-cua-implementation-evidence.md`, SHA-256 `D60EA0014884E4036C9E93B6D5BC01A7D3ACEEEF6651649E566F0F71F9C87F11`

## Exact inputs

| Path | SHA-256 | Lines |
|---|---|---:|
| `Assets/AcadeGameMaker/Runtime/Presentation/Unity/CostumeUnityPresentationAdapterV1.cs` | `6C5DF9CA1BA9EE9A53D5A091D4AE31DAA8FA46449E5CA60B738CF4F09038E2FB` | 95 |
| `Assets/AcadeGameMaker/Tests/PlayMode/PresentationUnity/CostumeUnityPresentationAdapterV1Tests.cs` | `FF8853A945113AA7D0A972E71B7C2D77F435F3AB5DA14BFDA6A8A04F954842BE` | 392 |

## R4-A verification

`CurrentTupleCorruptions` contains exactly 23 rows:

1. binding actor
2. media actor
3. snapshot actor
4. absent state current
5. different state current
6. binding costume
7. portrait set
8. gameplay atlas set
9. catalog revision
10. presentation revision
11–13. portrait/atlas/clip-map hash
14–15. missing/extra action vocabulary
16–23. stored frame X/Y/width/height/pivot X/pivot Y/baseline/duration

Each row is executed through two independent `TestCaseSource`s (`FF8853...:16–38`): a fresh valid-first-publication fixture followed by `TrySelect`, and a fresh valid-first-publication fixture followed by a higher-tick `CompletedSnapshots.Observe`. This produces 46 exact corruption cases rather than relying on test names alone.

For each case, `Corrupt` creates the specified malformed stored tuple (`:295–322`) and replaces only the adapter's private `_published` reference. The assertion helper (`:275–294`) verifies:

- the valid control path first publishes successfully;
- the pre-corruption highlight remains a nonterminal valid operation;
- the selected operation returns `ReloadRequired`;
- persistence status becomes `ReloadRequired`;
- the malformed `Published` object remains the exact same reference;
- move/replace/projection counts do not change;
- current ID and transient highlight remain unchanged;
- subsequent `TrySelect`, higher-tick `Observe`, and `Highlight` are all terminal `ReloadRequired` no-ops.

The adapter source predicate at `CostumeUnityPresentationAdapterV1.cs:83–88` checks the corresponding actor, state/current, observed snapshot, clip/frame and package-validation invariants before either selection or later observation publication.

## Decision

**PASS for R3-P1-001 only — P0=0, P1=0, P2=0 within this narrow scope.** The 23-row × two-entry-point corruption requirement is statically closed, with a valid control and terminal/no-mutation assertions. This does not close the known R3-P1-002 (package mutation matrix), R3-P1-003 (durable replacement/projection boundary), or R3-P1-004 (complete mechanics probe); the overall CUA execution gate remains blocked until those findings receive separate closure reviews.

