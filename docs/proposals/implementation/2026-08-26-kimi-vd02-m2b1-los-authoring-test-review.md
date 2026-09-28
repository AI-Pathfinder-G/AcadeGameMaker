# Kimi K3 VD-02 M2B1 LOS and authoring test screening

- Draft source: Ollama Cloud `kimi-k3:cloud`, bounded non-secret LOS/authoring test checklist
- Scope supplied: layer-8 filter, triggers, hierarchy ignores, composite, equal fractions, 64-hit saturation, and explicit registry validation
- Reviewer: Terra
- Status: screened; no Kimi production code copied

## Accepted test decomposition

- Separate behavior cases for trigger exclusion, player/target descendant ignores, unrelated blocker control, composite blocking, equal-fraction ordering, and fail-closed nonalloc saturation.
- The checklist correctly emphasized a saturation arrangement in which raw ignored hits can otherwise hide a later blocker. M2B1 instead treats any full 64-entry result buffer as the frozen fail-closed contract error before per-hit filtering.
- The checklist retained explicit serialized registry cases for null entries, duplicate IDs, invalid bindings, and ordinal order.

## Rejected or corrected input

- The draft speculated that equal fractions could use instance ID or unspecified ordering. This is rejected: the Approved contract requires fraction key then ordinal scene/hierarchy path, collider type name, and same-GameObject component ordinal; instance IDs and discovery order are forbidden.
- It suggested normalization/deduplication or skipping invalid registry references. This is rejected: authoring validation fails fast; no implicit repair or unordered discovery is introduced.
- It proposed optional unspecified degenerate LOS cases. They are outside M2B1 and were not added.

## Terra implementation decision

`TransferTarget` remains the only new public runtime component. Registry, LOS diagnostics, and test seams remain internal; all behavior is implemented directly from the Approved M2 addendum.
