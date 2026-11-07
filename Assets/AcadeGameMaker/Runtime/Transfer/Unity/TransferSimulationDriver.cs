using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using UnityEngine;

namespace AcadeGameMaker.Transfer.Unity
{
    // REQ-WT-001~008, REQ-MOV-004. The -200 transfer phase completes before the default-order movement motor begins its tick.
    [DefaultExecutionOrder(-200)]
    [DisallowMultipleComponent]
    internal sealed class TransferSimulationDriver : MonoBehaviour
    {
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private TransferTargetRegistry _registry;

        private SortedDictionary<int, TransferSimulationInput> _inputs = new SortedDictionary<int, TransferSimulationInput>();
        private readonly HashSet<string> _removedTargetIds = new HashSet<string>(StringComparer.Ordinal);
        private TransferSession _session;
        private bool _initialized;
        private object _inputGateOwner;
        private readonly object _inputFrameIssuer = new object();
        private bool _inputGateLocked;
        private long _inputGeneration;
        private IInputFrameCommitSource _coordinatedInputSource;
        private InputFrameCommitReceipt? _pendingCoordinatedReceipt;
        private long _lastConsumedCoordinatedOrdinal = -1;
        private long _lastCoordinatedModeEpoch;
        private bool _hasCoordinatedModeEpoch;
        private int _syncCaptureCount;
        private int _finalReflectionCount;
        private TransferCaptureReceipt? _latestCaptureReceipt;
        private TransferPhasePublication? _latestPhasePublication;
        private object _exposureEndOwner;
        private bool _exposureEndLaneRegistered;
        private HashSet<string> _exposureEndTargetIds;
        // REQ-COM-004. This is deliberately the completed-input request list,
        // not processed permanent-removal effect evidence: lifecycle-first
        // Transfer phases retain the request but skip its removal processing.
        // It is separate from the published summary to preserve M4B2's shape.
        private IReadOnlyList<string> _latestCompletedInputRemovalIds = Array.AsReadOnly(Array.Empty<string>());
        private int _bossTerminalCleanupTick = -1;
        private BossTerminalTransferCleanupReceipt? _latestBossTerminalCleanupReceipt;

        // REQ-UX-004, REQ-UX-006, REQ-UX-008. Queue copies are private to the
        // candidate and are installed only by the matching consumer instance.
        internal sealed class InputFrameCandidate
        {
            private readonly TransferSimulationDriver _consumer;
            private readonly object _issuer;
            private readonly object _owner;
            private readonly SimulationTick _tick;
            private readonly long _generation;
            private readonly long _nextGeneration;
            private readonly SortedDictionary<int, TransferSimulationInput> _inputs;
            private readonly bool _locked;
            private readonly int _strippedCount;
            private readonly InputFrameCommitReceipt? _coordinatedReceipt;
            private bool _committed;

            internal InputFrameCandidate(TransferSimulationDriver consumer, object issuer, object owner, SimulationTick tick,
                long generation, long nextGeneration, SortedDictionary<int, TransferSimulationInput> inputs,
                bool locked, int strippedCount, InputFrameCommitReceipt? coordinatedReceipt = null)
            {
                _consumer = consumer; _issuer = issuer; _owner = owner; _tick = tick; _generation = generation;
                _nextGeneration = nextGeneration; _inputs = inputs; _locked = locked; _strippedCount = strippedCount; _coordinatedReceipt = coordinatedReceipt;
            }

            internal bool Matches(TransferSimulationDriver consumer, object issuer, object owner, SimulationTick tick, long generation) =>
                !_committed && ReferenceEquals(_consumer, consumer) && ReferenceEquals(_issuer, issuer) && ReferenceEquals(_owner, owner) && _tick == tick && _generation == generation;
            internal SimulationTick Tick => _tick;
            internal int ApplyToMatchingConsumer(TransferSimulationDriver consumer, object issuer, object owner)
            {
                if (consumer == null)
                    throw new InvalidOperationException("Transfer input-frame candidate has no consumer.");
                if (!ReferenceEquals(issuer, consumer._inputFrameIssuer))
                    throw new InvalidOperationException("Transfer input-frame candidate issuer is invalid.");
                consumer.ValidateInputFrameCandidate(owner, this);
                consumer.ValidateProposedCoordinatedReceipt(owner, _tick, _locked, _coordinatedReceipt);
                if (!Matches(consumer, consumer._inputFrameIssuer, owner, _tick, consumer._inputGeneration))
                    throw new InvalidOperationException("Transfer input-frame candidate is foreign, stale, consumed, or forged.");
                consumer._inputs = _inputs;
                consumer._inputGateLocked = _locked;
                consumer._inputGeneration = _nextGeneration;
                if (_coordinatedReceipt.HasValue) consumer._pendingCoordinatedReceipt = _coordinatedReceipt;
                _committed = true;
                return _strippedCount;
            }
        }

        internal int SyncCaptureCountForTests => _syncCaptureCount;
        internal int FinalReflectionCountForTests => _finalReflectionCount;
        internal TransferSessionSnapshot SnapshotForTests => _session == null ? default(TransferSessionSnapshot) : _session.Snapshot(_player.NextExpectedTick);
        internal TransferCaptureReceipt? LatestCaptureReceipt => _latestCaptureReceipt;
        // REQ-WT-001, REQ-WT-005. Read-only boss exposure seams; these never
        // create or re-freeze the registry.
        internal PlayerMovementController BoundPlayer => _player;
        internal TransferTargetRegistry BoundRegistry => _registry;
        // REQ-WT-003, REQ-WT-005, REQ-WT-007. Published only after the VD-02
        // session and final player-modifier reflection have both completed.
        internal TransferPhasePublication? LatestPhasePublication => _latestPhasePublication;
        internal IReadOnlyList<string> LatestCompletedInputRemovalIds
        {
            get
            {
                var copy = new string[_latestCompletedInputRemovalIds.Count];
                for (var index = 0; index < copy.Length; index++) copy[index] = _latestCompletedInputRemovalIds[index];
                return Array.AsReadOnly(copy);
            }
        }
        internal BossTerminalTransferCleanupReceipt? LatestBossTerminalCleanupReceipt => _latestBossTerminalCleanupReceipt;
        internal bool IsInputGateLocked => _inputGateLocked;

        internal void InitializeForInputRouter() => EnsureInitialized();
        internal void ValidateCoordinatedInputRegistration(object owner, IInputFrameCommitSource source, SimulationTick firstTick)
        {
            if (owner == null || source == null || source.IsFaulted || !ReferenceEquals(owner, source) || !_initialized || _player == null || firstTick != _player.NextExpectedTick ||
                (_latestPhasePublication.HasValue && _latestPhasePublication.Value.Tick.Value >= firstTick.Value))
                throw new InvalidOperationException("Transfer coordinated input registration is not ready or identity-bound.");
            if ((_inputGateOwner != null && !ReferenceEquals(_inputGateOwner, owner)) || (_coordinatedInputSource != null && !ReferenceEquals(_coordinatedInputSource, source)))
                throw new InvalidOperationException("Transfer coordinated input registration conflicts with an existing owner.");
        }
        internal void RegisterCoordinatedInputOwner(object owner, IInputFrameCommitSource source, SimulationTick firstTick)
        {
            ValidateCoordinatedInputRegistration(owner, source, firstTick); RegisterInputGateOwner(owner); _coordinatedInputSource = source;
        }

        private void FixedUpdate() => AdvanceTransferPhase();

        // REQ-WT-003, REQ-WT-005; affected REQ-COM-001. Noninitializing authored binding.
        internal void ConfigureForAuthoring(PlayerMovementController player, TransferTargetRegistry registry) => Configure(player, registry);
        internal void ConfigureForTests(PlayerMovementController player, TransferTargetRegistry registry) => Configure(player, registry);

        private void Configure(PlayerMovementController player, TransferTargetRegistry registry)
        {
            if (_initialized) throw new InvalidOperationException("TransferSimulationDriver cannot be reconfigured after initialization.");
            _player = player;
            _registry = registry;
        }

        internal void InitializeForTests() => EnsureInitialized();

        // REQ-WT-003, REQ-WT-005, REQ-WT-007; affected REQ-COM-001. Exact B0
        // graph/registration validation is read-only and never replays a Transfer phase.
        internal void ValidateEncounterGraph(PlayerMovementController player, TransferTargetRegistry registry,
            TransferTarget surveyor, TransferTarget walker, bool requireInitialized)
        {
            if (_player != player || _registry != registry)
                throw new InvalidOperationException("Transfer encounter graph references are cross-wired.");
            registry.ValidateForAuthoring();
            if (registry.Targets.Count != 2 || registry.Targets[0] != surveyor || registry.Targets[1] != walker ||
                !string.Equals(surveyor.TargetId, "surveyor", StringComparison.Ordinal) ||
                !string.Equals(walker.TargetId, "walker", StringComparison.Ordinal))
                throw new InvalidOperationException("Transfer encounter registry must be exact surveyor, walker order.");
            if (requireInitialized && (!_initialized || _session == null ||
                !_session.HasExactEncounterRegistrations("surveyor", "walker")))
                throw new InvalidOperationException("Fresh encounter requires complete Transfer registrations.");
        }

        // REQ-COM-001, REQ-WT-003, REQ-WT-005. Read-only +110 handoff seam:
        // proves the completed -200 publication still belongs to the exact
        // authored player/surveyor/walker registry without replaying Transfer.
        internal void ValidateEncounterHandoffPublication(PlayerMovementController player, TransferTarget surveyor,
            TransferTarget walker, Collider2D surveyorCollider, Collider2D walkerCollider, SimulationTick tick)
        {
            if (_player != player || !_initialized || !_latestPhasePublication.HasValue || _latestPhasePublication.Value.Tick != tick)
                throw new InvalidOperationException("Encounter handoff requires the exact completed Transfer publication.");
            _registry.ValidateForAuthoring();
            if (_registry.Targets.Count != 2 || _registry.Targets[0] != surveyor || _registry.Targets[1] != walker ||
                surveyor == null || walker == null || surveyor.TargetCollider != surveyorCollider || walker.TargetCollider != walkerCollider ||
                !string.Equals(surveyor.TargetId, "surveyor", StringComparison.Ordinal) || !string.Equals(walker.TargetId, "walker", StringComparison.Ordinal))
                throw new InvalidOperationException("Encounter handoff requires exact frozen surveyor, walker Transfer object identity.");
        }

        // REQ-WT-001, REQ-WT-006. Authoritative internal production seam for exact tick-indexed transfer inputs.
        internal void Submit(TransferSimulationInput input)
        {
            var accepted = _inputGateOwner == null
                ? input
                : NormalizeInput(input, _inputGateLocked);
            EnsureInitialized();
            var expected = _player.NextExpectedTick.Value;
            if (accepted.Tick.Value < expected || _inputs.ContainsKey(accepted.Tick.Value))
            {
                throw new InvalidOperationException("Transfer inputs are exact, future, and unique by SimulationTick.");
            }

            var nextGeneration = checked(_inputGeneration + 1);
            _inputs.Add(accepted.Tick.Value, accepted);
            _inputGeneration = nextGeneration;
        }

        internal void SubmitForTests(TransferSimulationInput input) => Submit(input);

        // REQ-UX-004, REQ-UX-008, REQ-WT-005, REQ-WT-006. This consumer has
        // one identity-bound future InputMode owner; registration changes no input.
        internal void RegisterInputGateOwner(object owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (_inputGateOwner != null && !ReferenceEquals(_inputGateOwner, owner))
            {
                throw new InvalidOperationException("A different Transfer input-gate owner is already registered.");
            }

            _inputGateOwner = owner;
        }

        // REQ-UX-004, REQ-UX-008, REQ-WT-005, REQ-WT-006. Gate application
        // preserves all maintenance fields and strips only interactive capture.
        internal int ApplyInputGate(object owner, SimulationTick tick, bool locked)
        {
            if (_inputGateOwner == null || !ReferenceEquals(_inputGateOwner, owner))
            {
                throw new InvalidOperationException("The caller is not the registered Transfer input-gate owner.");
            }

            if (!_initialized || _player == null || tick.Value < 0 || tick != _player.NextExpectedTick ||
                (_latestPhasePublication.HasValue && _latestPhasePublication.Value.Tick.Value >= tick.Value))
            {
                throw new InvalidOperationException("Transfer input-gate application requires an initialized unconsumed exact phase.");
            }

            if (_inputGateLocked == locked)
            {
                return 0;
            }

            if (!locked)
            {
                var nextGeneration = checked(_inputGeneration + 1);
                _inputGateLocked = false;
                _inputGeneration = nextGeneration;
                return 0;
            }

            var staged = new SortedDictionary<int, TransferSimulationInput>();
            var strippedCount = 0;
            foreach (var entry in _inputs)
            {
                var validated = NormalizeInput(entry.Value, false);
                if (entry.Key != validated.Tick.Value)
                {
                    throw new InvalidOperationException("Queued Transfer input dictionary key does not match its tick.");
                }

                if (HasInteractiveInput(validated))
                {
                    strippedCount++;
                }

                staged.Add(entry.Key, StripInteractiveInput(validated));
            }

            var changedGeneration = checked(_inputGeneration + 1);
            _inputs = staged;
            _inputGateLocked = true;
            _inputGeneration = changedGeneration;
            return strippedCount;
        }

        internal InputFrameCandidate PrepareInputFrame(object owner, SimulationTick tick, bool locked, TransferSimulationInput? input) => PrepareInputFrameInternal(owner, tick, locked, input, null);
        internal InputFrameCandidate PrepareInputFrame(object owner, SimulationTick tick, bool locked, TransferSimulationInput? input, InputFrameCommitReceipt receipt) => PrepareInputFrameInternal(owner, tick, locked, input, receipt);
        private InputFrameCandidate PrepareInputFrameInternal(object owner, SimulationTick tick, bool locked, TransferSimulationInput? input, InputFrameCommitReceipt? receipt)
        {
            EnsureInputFrameOwner(owner, tick);
            ValidateProposedCoordinatedReceipt(owner, tick, locked, receipt);
            if (locked && input.HasValue) throw new InvalidOperationException("A locked Transfer input frame cannot carry input.");
            var staged = CopyInputQueue();
            var changed = _inputGateLocked != locked;
            var stripped = 0;
            if (locked && changed)
            {
                var strippedInputs = new SortedDictionary<int, TransferSimulationInput>();
                foreach (var entry in staged)
                {
                    if (HasInteractiveInput(entry.Value)) stripped++;
                    strippedInputs.Add(entry.Key, StripInteractiveInput(entry.Value));
                }
                staged = strippedInputs;
            }
            if (input.HasValue)
            {
                var accepted = NormalizeInput(input.Value, false);
                if (accepted.Tick != tick || accepted.Removals.Count != 0 || accepted.ExposureEnds.Count != 0 || accepted.Lifecycle.HasValue)
                    throw new InvalidOperationException("Transfer input-frame input may contain only exact interactive fields.");
                if (locked || (staged.TryGetValue(tick.Value, out var existing) && HasInteractiveInput(existing)))
                    throw new InvalidOperationException("Transfer input-frame input conflicts with the exact tick.");
                staged[tick.Value] = staged.TryGetValue(tick.Value, out existing)
                    ? new TransferSimulationInput(tick, accepted.Camera, accepted.Aim, accepted.Press, existing.Removals, existing.ExposureEnds, existing.Lifecycle)
                    : accepted;
                changed = true;
            }
            var next = checked(_inputGeneration + 1);
            return new InputFrameCandidate(this, _inputFrameIssuer, owner, tick, _inputGeneration, next, staged, locked, stripped, receipt);
        }

        internal void ValidateInputFrameCandidate(object owner, InputFrameCandidate candidate)
        {
            if (candidate == null)
                throw new InvalidOperationException("Transfer input-frame candidate is null.");
            EnsureInputFrameOwner(owner, candidate.Tick);
            if (!candidate.Matches(this, _inputFrameIssuer, owner, candidate.Tick, _inputGeneration))
                throw new InvalidOperationException("Transfer input-frame candidate is foreign, stale, consumed, or forged.");
        }

        internal int CommitInputFrame(object owner, InputFrameCandidate candidate)
        {
            ValidateInputFrameCandidate(owner, candidate);
            return candidate.ApplyToMatchingConsumer(this, _inputFrameIssuer, owner);
        }

        private void EnsureInputFrameOwner(object owner, SimulationTick tick)
        {
            if (owner == null || _inputGateOwner == null || !ReferenceEquals(_inputGateOwner, owner))
                throw new InvalidOperationException("The caller is not the registered Transfer input-frame owner.");
            if (!_initialized || _player == null || tick.Value < 0 || tick != _player.NextExpectedTick ||
                (_latestPhasePublication.HasValue && _latestPhasePublication.Value.Tick.Value >= tick.Value))
                throw new InvalidOperationException("Transfer input frame requires an initialized unconsumed exact phase.");
        }

        private SortedDictionary<int, TransferSimulationInput> CopyInputQueue()
        {
            var staged = new SortedDictionary<int, TransferSimulationInput>();
            foreach (var entry in _inputs)
            {
                var accepted = NormalizeInput(entry.Value, false);
                if (entry.Key != accepted.Tick.Value) throw new InvalidOperationException("Queued Transfer input dictionary key does not match its tick.");
                staged.Add(entry.Key, accepted);
            }
            return staged;
        }

        // REQ-WT-001, REQ-WT-005. Exactly one boss scheduler owns the
        // temporary-withdrawal lane. Registration is deliberately possible
        // before the first -200 phase but after serialized binding exists.
        internal void RegisterExposureEndLane(object owner, IReadOnlyList<string> targetIds)
        {
            if (owner == null || targetIds == null || (targetIds.Count != 3 && targetIds.Count != 4) || _exposureEndLaneRegistered || _initialized || _latestPhasePublication.HasValue || _player == null || _registry == null)
                throw new InvalidOperationException("Exposure-end lane requires one pristine configured Transfer driver.");
            var accepted = new HashSet<string>(StringComparer.Ordinal);
            var first = targetIds.Count == 4 ? 1 : 0;
            if (targetIds.Count == 4 && (!string.Equals(targetIds[0], "BossAuditBox-0", StringComparison.Ordinal) || !RegistryContainsTargetId("BossAuditBox-0")))
                throw new InvalidOperationException("Exposure-end lane requires the exact audit target before payload IDs.");
            for (var index = 0; index < 3; index++)
            {
                var expected = "BossWeight-" + index;
                if (!string.Equals(targetIds[index + first], expected, StringComparison.Ordinal) || !RegistryContainsTargetId(expected) || !accepted.Add(expected))
                    throw new InvalidOperationException("Exposure-end lane requires the exact registered boss payload IDs in ordinal order.");
            }
            if (targetIds.Count == 4) accepted.Add("BossAuditBox-0");
            _exposureEndOwner = owner;
            _exposureEndTargetIds = accepted;
            _exposureEndLaneRegistered = true;
        }

        // REQ-WT-005, REQ-WT-006, REQ-COM-004. Combat may atomically append only future target removals without replacing queued transfer intent or lifecycle data.
        internal void MergeRemovals(IReadOnlyList<TransferTargetRemoved> removals)
        {
            MergeRemovals(removals, false);
        }

        // REQ-COM-004. The production death batch is not an extensible merge:
        // any pre-existing removal would make the requested four-ID proof
        // ambiguous, so reject before replacing the queue or touching sinks.
        internal void MergeExactBossTerminalRemovals(IReadOnlyList<TransferTargetRemoved> removals)
        {
            if (_bossTerminalCleanupTick >= 0 || _latestBossTerminalCleanupReceipt.HasValue)
                throw new InvalidOperationException("Boss terminal cleanup may be reserved once.");
            MergeRemovals(removals, true);
        }

        private void MergeRemovals(IReadOnlyList<TransferTargetRemoved> removals, bool requireExactBossTerminalBatch)
        {
            EnsureInitialized();
            if (removals == null) throw new ArgumentNullException(nameof(removals));
            if (removals.Count == 0) return;

            if (requireExactBossTerminalBatch) ValidateExactBossTerminalRemovalBatch(removals);

            var tick = removals[0].Tick;
            if (tick.Value <= _player.NextExpectedTick.Value)
            {
                throw new InvalidOperationException("Merged transfer removals must target a phase strictly after the current movement tick.");
            }

            _inputs.TryGetValue(tick.Value, out var queued);
            if (requireExactBossTerminalBatch && queued.Removals != null && queued.Removals.Count != 0)
                throw new InvalidOperationException("Boss terminal removal batch cannot merge with pre-existing removals.");
            var merged = new List<TransferTargetRemoved>(queued.Removals == null ? removals.Count : queued.Removals.Count + removals.Count);
            var ids = new HashSet<string>(StringComparer.Ordinal);
            if (queued.Removals != null)
            {
                for (var index = 0; index < queued.Removals.Count; index++)
                {
                    var existing = queued.Removals[index];
                    if (existing.Tick != tick) throw new InvalidOperationException("Queued transfer removals violate the exact-tick merge contract.");
                    if (ids.Add(existing.TargetId)) merged.Add(existing);
                }
            }

            for (var index = 0; index < removals.Count; index++)
            {
                var removal = removals[index];
                if (removal.Tick != tick) throw new InvalidOperationException("Transfer removal merge contains a mismatched-tick target.");
                if (ids.Add(removal.TargetId)) merged.Add(removal);
            }

            merged.Sort((left, right) => string.CompareOrdinal(left.TargetId, right.TargetId));
            var exposureEnds = queued.ExposureEnds == null ? Array.Empty<TransferTargetExposureEnded>() : queued.ExposureEnds;
            var replacement = ReplaceInput(tick, queued, merged, exposureEnds, queued.Removals == null);
            var nextGeneration = checked(_inputGeneration + 1);
            _inputs[tick.Value] = replacement;
            _inputGeneration = nextGeneration;
            if (requireExactBossTerminalBatch) _bossTerminalCleanupTick = tick.Value;
        }

        private static void ValidateExactBossTerminalRemovalBatch(IReadOnlyList<TransferTargetRemoved> removals)
        {
            if (removals.Count != 4) throw new InvalidOperationException("Boss terminal removal batch requires four exact IDs.");
            var tick = removals[0].Tick;
            var expected = new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" };
            for (var index = 0; index < expected.Length; index++)
                if (removals[index].Tick != tick || !string.Equals(removals[index].TargetId, expected[index], StringComparison.Ordinal))
                    throw new InvalidOperationException("Boss terminal removal batch is not exact ordinal audit/payload order.");
        }

        // REQ-WT-001, REQ-WT-005. Appends a private scheduler-owned temporary
        // end without touching permanent removals or the frozen registry.
        internal void MergeExposureEnds(object owner, IReadOnlyList<TransferTargetExposureEnded> ends)
        {
            if (!_initialized || !_exposureEndLaneRegistered || !ReferenceEquals(owner, _exposureEndOwner) || ends == null || !_latestPhasePublication.HasValue ||
                _latestPhasePublication.Value.Tick != _player.NextExpectedTick)
                throw new InvalidOperationException("Exposure-end merge requires the registered owner and exact completed Transfer publication.");
            if (ends.Count == 0) return;
            var tick = ends[0].Tick;
            if (tick.Value <= _player.NextExpectedTick.Value) throw new InvalidOperationException("Exposure ends must target a strictly future Transfer phase.");
            _inputs.TryGetValue(tick.Value, out var queued);
            var values = new List<TransferTargetExposureEnded>((queued.ExposureEnds == null ? 0 : queued.ExposureEnds.Count) + ends.Count);
            var ids = new HashSet<string>(StringComparer.Ordinal);
            if (queued.ExposureEnds != null) for (var index = 0; index < queued.ExposureEnds.Count; index++)
            {
                var existing = queued.ExposureEnds[index];
                if (existing.Tick != tick) throw new InvalidOperationException("Queued exposure end has a mismatched tick.");
                if (ids.Add(existing.TargetId)) values.Add(existing);
            }
            for (var index = 0; index < ends.Count; index++)
            {
                var value = ends[index];
                if (value.Tick != tick) throw new InvalidOperationException("Exposure-end merge contains a mismatched tick.");
                if (!_exposureEndTargetIds.Contains(value.TargetId)) throw new InvalidOperationException("Exposure-end merge names a target outside the registered boss payload lane.");
                if (ids.Add(value.TargetId)) values.Add(value);
            }
            values.Sort((left, right) => string.CompareOrdinal(left.TargetId, right.TargetId));
            var permanent = queued.Removals == null ? Array.Empty<TransferTargetRemoved>() : queued.Removals;
            var nextGeneration = checked(_inputGeneration + 1);
            _inputs[tick.Value] = ReplaceInput(tick, queued, permanent, values, queued.Removals == null);
            _inputGeneration = nextGeneration;
        }

        private bool RegistryContainsTargetId(string targetId)
        {
            for (var index = 0; index < _registry.Targets.Count; index++)
                if (_registry.Targets[index] != null && string.Equals(_registry.Targets[index].TargetId, targetId, StringComparison.Ordinal)) return true;
            return false;
        }

        private static TransferSimulationInput ReplaceInput(SimulationTick tick, TransferSimulationInput queued,
            IReadOnlyList<TransferTargetRemoved> removals, IReadOnlyList<TransferTargetExposureEnded> exposureEnds, bool isEmpty)
        {
            return isEmpty
                ? new TransferSimulationInput(tick, null, null, null, removals, exposureEnds, null)
                : new TransferSimulationInput(tick, queued.Camera, queued.Aim, queued.Press, removals, exposureEnds, queued.Lifecycle);
        }

        internal TransferTickOutcome AdvanceTransferPhaseForTests() => AdvanceTransferPhase();
        // AC-M5B5-001/007. Input integration tests advance the real phase without
        // widening access to Transfer's internal outcome payload assembly.
        internal void AdvanceCoordinatedInputPhaseForTests() => AdvanceTransferPhase();

        private TransferTickOutcome AdvanceTransferPhase()
        {
            if (_player == null) throw new InvalidOperationException("TransferSimulationDriver requires an explicit PlayerMovementController.");
            var tick = _player.NextExpectedTick;
            ConsumeCoordinatedReceipt(tick);
            _player.PreflightTransferModifier(tick); // Must happen before registry/sink/session work.
            EnsureInitialized();
            var terminalExpectation = PreflightBossTerminalCleanup(tick);
            var input = ConsumeOrEmpty(tick);
            var removals = input.Removals;
            IReadOnlyList<TransferCandidateObservation> observations = Array.Empty<TransferCandidateObservation>();
            if (!input.Lifecycle.HasValue && input.Aim.HasValue)
            {
                // REQ-WT-003, REQ-WT-006. Exactly one external synchronization precedes all registry poses and LOS casts for this capture.
                Physics2D.SyncTransforms();
                _syncCaptureCount++;
                _latestCaptureReceipt = new TransferCaptureReceipt(tick, input.Aim.Value.SampleId, input.Aim.Value.SampleTick, input.Camera.Value.CameraPoseTick);
                observations = new TransferObservationBuilder().Capture(
                    tick,
                    _player.Snapshot,
                    input.Camera.Value,
                    input.Aim.Value,
                    _registry.Targets,
                    RemovedIdsForCapture(removals),
                    _player.transform);
            }

            var outcome = _session.Process(tick, input.Lifecycle, input.ExposureEnds, removals, input.Aim, observations, input.Press);
            // REQ-COM-004. Lifecycle has verified precedence inside
            // TransferSession; requested removals remain observable below but
            // must not become durable descriptor exclusions on that path.
            if (!input.Lifecycle.HasValue)
                for (var index = 0; index < removals.Count; index++) _removedTargetIds.Add(removals[index].TargetId);
            ReflectFinalPlayerState(outcome.Snapshot, tick);
            _latestCompletedInputRemovalIds = CopyRemovalIds(removals);
            _latestPhasePublication = new TransferPhasePublication(outcome, new TransferPhaseInputSummary(input));
            if (terminalExpectation.HasValue)
            {
                _latestBossTerminalCleanupReceipt = BuildBossTerminalCleanupReceipt(terminalExpectation.Value, input, outcome);
                _bossTerminalCleanupTick = -1;
            }
            return outcome;
        }

        private void ValidateProposedCoordinatedReceipt(object owner, SimulationTick tick, bool locked, InputFrameCommitReceipt? receipt)
        {
            if (_coordinatedInputSource == null) { if (receipt.HasValue) throw new InvalidOperationException("Transfer is not registered for coordinated input."); return; }
            if (!receipt.HasValue || !ReferenceEquals(owner, _coordinatedInputSource) || _coordinatedInputSource.IsFaulted || receipt.Value.Tick != tick || receipt.Value.FrameOrdinal != checked(_lastConsumedCoordinatedOrdinal + 1) || (!_hasCoordinatedModeEpoch && receipt.Value.ModeEpoch != 0) || (_hasCoordinatedModeEpoch && (receipt.Value.ModeEpoch < _lastCoordinatedModeEpoch || (receipt.Value.ModeEpoch != _lastCoordinatedModeEpoch && (_lastCoordinatedModeEpoch == long.MaxValue || receipt.Value.ModeEpoch != _lastCoordinatedModeEpoch + 1)))) || locked != (receipt.Value.Mode != InputMode.GameplayEnabled))
                throw new InvalidOperationException("Transfer coordinated input receipt is invalid.");
        }

        private void ConsumeCoordinatedReceipt(SimulationTick tick)
        {
            if (_coordinatedInputSource == null) return;
            if (_coordinatedInputSource.IsFaulted || !_pendingCoordinatedReceipt.HasValue || !_coordinatedInputSource.CurrentReceipt.HasValue || _pendingCoordinatedReceipt.Value != _coordinatedInputSource.CurrentReceipt.Value || _pendingCoordinatedReceipt.Value.Tick != tick || _pendingCoordinatedReceipt.Value.FrameOrdinal != checked(_lastConsumedCoordinatedOrdinal + 1) || (!_hasCoordinatedModeEpoch && _pendingCoordinatedReceipt.Value.ModeEpoch != 0) || (_hasCoordinatedModeEpoch && (_pendingCoordinatedReceipt.Value.ModeEpoch < _lastCoordinatedModeEpoch || (_pendingCoordinatedReceipt.Value.ModeEpoch != _lastCoordinatedModeEpoch && (_lastCoordinatedModeEpoch == long.MaxValue || _pendingCoordinatedReceipt.Value.ModeEpoch != _lastCoordinatedModeEpoch + 1)))) || _inputGateLocked != (_pendingCoordinatedReceipt.Value.Mode != InputMode.GameplayEnabled))
                throw new InvalidOperationException("Transfer coordinated input proof is missing or stale.");
            _lastConsumedCoordinatedOrdinal = _pendingCoordinatedReceipt.Value.FrameOrdinal; _lastCoordinatedModeEpoch = _pendingCoordinatedReceipt.Value.ModeEpoch; _hasCoordinatedModeEpoch = true; _pendingCoordinatedReceipt = null;
        }

        private BossTerminalCleanupExpectation? PreflightBossTerminalCleanup(SimulationTick tick)
        {
            if (_bossTerminalCleanupTick < 0 || _bossTerminalCleanupTick > tick.Value) return null;
            if (_bossTerminalCleanupTick < tick.Value)
                throw new InvalidOperationException("Boss terminal cleanup reservation is stale.");
            if (!_inputs.TryGetValue(tick.Value, out var input) || input.Tick != tick || input.Removals == null || input.ExposureEnds == null || input.ExposureEnds.Count != 0 ||
                input.Removals.Count != 4 || input.Lifecycle.HasValue && !IsLifecycle(input.Lifecycle.Value))
                throw new InvalidOperationException("Boss terminal cleanup envelope is malformed.");
            var ids = BossTerminalIds;
            for (var index = 0; index < ids.Length; index++)
                if (input.Removals[index].Tick != tick || !string.Equals(input.Removals[index].TargetId, ids[index], StringComparison.Ordinal))
                    throw new InvalidOperationException("Boss terminal cleanup removals are not exact.");
            return new BossTerminalCleanupExpectation(new SimulationTick(checked(tick.Value - 1)), tick, input.Lifecycle, _session.Snapshot(tick).ActiveTargetId, _session.Snapshot(tick).Revision);
        }

        private BossTerminalTransferCleanupReceipt BuildBossTerminalCleanupReceipt(BossTerminalCleanupExpectation expected, TransferSimulationInput input, TransferTickOutcome outcome)
        {
            var ids = BossTerminalIds;
            var lifecycle = expected.Lifecycle;
            var durable = true;
            for (var index = 0; index < ids.Length; index++) durable &= _removedTargetIds.Contains(ids[index]);
            var removedFromSession = _session.HasNoRegistrations(ids);
            if (!removedFromSession) throw new InvalidOperationException("Boss terminal cleanup did not remove every requested registration.");
            if (!lifecycle.HasValue && !durable) throw new InvalidOperationException("Boss terminal cleanup effect did not commit.");
            if (lifecycle.HasValue)
                for (var index = 0; index < ids.Length; index++) if (_removedTargetIds.Contains(ids[index])) throw new InvalidOperationException("Lifecycle-superseded cleanup cannot claim durable removals.");
            var clear = outcome.RemovalResult.HasValue ? outcome.RemovalResult.Value.Cleared : null;
            var expectedReason = lifecycle ?? TransferClearReason.TargetRemoved;
            if (expected.ActiveTargetId.Length != 0 && (!clear.HasValue || clear.Value.Reason != expectedReason || clear.Value.PreviousTargetId != expected.ActiveTargetId))
                throw new InvalidOperationException("Boss terminal active clear is not exact.");
            if (expected.ActiveTargetId.Length != 0 && (clear.Value.Tick != expected.CleanupTick || clear.Value.Revision != checked(expected.Revision + 1)))
                throw new InvalidOperationException("Boss terminal active clear revision is not exact.");
            if (expected.ActiveTargetId.Length == 0 && (clear.HasValue || outcome.Snapshot.Revision != expected.Revision))
                throw new InvalidOperationException("Boss terminal inactive cleanup changed revision.");
            return new BossTerminalTransferCleanupReceipt(expected.DeathSourceTick, expected.CleanupTick, lifecycle, ids, expected.ActiveTargetId,
                outcome.Snapshot.ActiveTargetId, outcome.Snapshot.PlayerState, outcome.Snapshot.Revision, clear, !lifecycle.HasValue && durable && removedFromSession);
        }

        private static bool IsLifecycle(TransferClearReason value) => value == TransferClearReason.RoomLeaving || value == TransferClearReason.RunFailed || value == TransferClearReason.Cutscene || value == TransferClearReason.DemoCompleted;
        private static readonly string[] BossTerminalIds = { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" };

        private void EnsureInitialized()
        {
            if (_initialized) return;
            if (_player == null || _registry == null) throw new InvalidOperationException("TransferSimulationDriver requires explicit serialized player and registry bindings.");
            _registry.ValidateForAuthoring();
            _session = new TransferSession();
            foreach (var target in _registry.Targets)
            {
                _session.Register(target.BuildDescriptorFromValidatedAuthoring(), target.TargetOwnerSink);
            }

            _initialized = true;
        }

        private TransferSimulationInput ConsumeOrEmpty(SimulationTick tick)
        {
            if (_inputs.TryGetValue(tick.Value, out var input))
            {
                // A malformed capture is a fail-stop contract error; its consumed input is intentionally not retryable.
                var nextGeneration = checked(_inputGeneration + 1);
                _inputs.Remove(tick.Value);
                _inputGeneration = nextGeneration;
                return input;
            }

            return new TransferSimulationInput(tick, null, null, null, Array.Empty<TransferTargetRemoved>(), Array.Empty<TransferTargetExposureEnded>(), null);
        }

        private static TransferSimulationInput NormalizeInput(TransferSimulationInput input, bool stripInteractive)
        {
            var validated = new TransferSimulationInput(
                input.Tick,
                input.Camera,
                input.Aim,
                input.Press,
                input.Removals,
                input.ExposureEnds,
                input.Lifecycle);
            return stripInteractive ? StripInteractiveInput(validated) : validated;
        }

        private static TransferSimulationInput StripInteractiveInput(TransferSimulationInput input)
        {
            return new TransferSimulationInput(
                input.Tick,
                null,
                null,
                null,
                input.Removals,
                input.ExposureEnds,
                input.Lifecycle);
        }

        private static bool HasInteractiveInput(TransferSimulationInput input)
        {
            return input.Camera.HasValue || input.Aim.HasValue || input.Press.HasValue;
        }

        private void ReflectFinalPlayerState(TransferSessionSnapshot snapshot, SimulationTick tick)
        {
            var modifier = snapshot.PlayerState == TransferPlayerState.Lightweight
                ? PlayerMovementModifierKind.Lightweight
                : PlayerMovementModifierKind.Baseline;
            _player.ApplyPreflightedTransferModifier(modifier, tick);
            _finalReflectionCount++;
        }

        private IReadOnlyList<string> RemovedIdsForCapture(IReadOnlyList<TransferTargetRemoved> removals)
        {
            var ids = new List<string>(_removedTargetIds);
            for (var index = 0; index < removals.Count; index++) ids.Add(removals[index].TargetId);
            ids.Sort(StringComparer.Ordinal);
            return ids;
        }

        private static IReadOnlyList<string> CopyRemovalIds(IReadOnlyList<TransferTargetRemoved> removals)
        {
            var copy = new string[removals.Count];
            for (var index = 0; index < copy.Length; index++) copy[index] = removals[index].TargetId;
            return Array.AsReadOnly(copy);
        }
    }

    // REQ-WT-006, REQ-COM-001. Minimal immutable proof that the authoritative -200 phase synchronized the exact shared aim/camera pair.
    internal readonly struct TransferCaptureReceipt
    {
        internal TransferCaptureReceipt(SimulationTick simulationTick, long aimSampleId, SimulationTick aimSampleTick, SimulationTick cameraPoseTick)
        {
            if (aimSampleId < 0 || aimSampleTick != simulationTick || cameraPoseTick.Value != checked(simulationTick.Value - 1)) throw new ArgumentException("Capture receipt must describe the exact processing aim/camera pair.");
            SimulationTick = simulationTick;
            AimSampleId = aimSampleId;
            AimSampleTick = aimSampleTick;
            CameraPoseTick = cameraPoseTick;
        }

        internal SimulationTick SimulationTick { get; }
        internal long AimSampleId { get; }
        internal SimulationTick AimSampleTick { get; }
        internal SimulationTick CameraPoseTick { get; }
    }

    // REQ-COM-004, REQ-WT-003, REQ-WT-005. Internal post-commit proof for
    // the exact boss terminal lane; it is not a public Transfer snapshot.
    internal readonly struct BossTerminalTransferCleanupReceipt
    {
        internal BossTerminalTransferCleanupReceipt(SimulationTick deathSourceTick, SimulationTick cleanupTick,
            TransferClearReason? lifecycleReason, IReadOnlyList<string> requestedIds, string activeBefore,
            string activeAfter, TransferPlayerState playerState, int revision, TransferCleared? clear, bool durableRemovalCommitted)
        {
            if (cleanupTick.Value != checked(deathSourceTick.Value + 1) || requestedIds == null || requestedIds.Count != 4 ||
                (lifecycleReason.HasValue && !IsLifecycle(lifecycleReason.Value)) ||
                (playerState != TransferPlayerState.Baseline || !string.IsNullOrEmpty(activeAfter))) throw new ArgumentException("Boss cleanup receipt is malformed.");
            for (var index = 0; index < BossIds.Length; index++) if (!string.Equals(requestedIds[index], BossIds[index], StringComparison.Ordinal)) throw new ArgumentException("Boss cleanup IDs are not exact.");
            DeathSourceTick = deathSourceTick; CleanupTick = cleanupTick; LifecycleReason = lifecycleReason;
            ActiveTargetBefore = activeBefore ?? string.Empty; ActiveTargetAfter = activeAfter ?? string.Empty;
            PlayerState = playerState; Revision = revision; Clear = clear; DurableRemovalCommitted = durableRemovalCommitted;
            RequestedIds = Array.AsReadOnly((string[])BossIds.Clone());
        }
        internal SimulationTick DeathSourceTick { get; }
        internal SimulationTick CleanupTick { get; }
        internal TransferClearReason? LifecycleReason { get; }
        internal IReadOnlyList<string> RequestedIds { get; }
        internal string ActiveTargetBefore { get; }
        internal string ActiveTargetAfter { get; }
        internal TransferPlayerState PlayerState { get; }
        internal int Revision { get; }
        internal TransferCleared? Clear { get; }
        internal bool DurableRemovalCommitted { get; }
        internal bool IsLifecycleSuperseded => LifecycleReason.HasValue;
        private static readonly string[] BossIds = { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" };
        private static bool IsLifecycle(TransferClearReason value) => value == TransferClearReason.RoomLeaving || value == TransferClearReason.RunFailed || value == TransferClearReason.Cutscene || value == TransferClearReason.DemoCompleted;
    }

    internal readonly struct BossTerminalCleanupExpectation
    {
        internal BossTerminalCleanupExpectation(SimulationTick deathSourceTick, SimulationTick cleanupTick, TransferClearReason? lifecycle, string activeTargetId, int revision)
        { DeathSourceTick = deathSourceTick; CleanupTick = cleanupTick; Lifecycle = lifecycle; ActiveTargetId = activeTargetId ?? string.Empty; Revision = revision; }
        internal SimulationTick DeathSourceTick { get; }
        internal SimulationTick CleanupTick { get; }
        internal TransferClearReason? Lifecycle { get; }
        internal string ActiveTargetId { get; }
        internal int Revision { get; }
    }

    // REQ-WT-003, REQ-WT-005, REQ-WT-007. Immutable consumed-intent proof for
    // downstream fixed phases; it intentionally carries no device, camera or scene data.
    internal readonly struct TransferPhaseInputSummary
    {
        internal TransferPhaseInputSummary(TransferSimulationInput input)
        {
            HadAim = input.Aim.HasValue;
            HadPress = input.Press.HasValue;
            RemovalCount = input.Removals.Count;
            LifecycleReason = input.Lifecycle;
        }

        internal bool HadAim { get; }
        internal bool HadPress { get; }
        internal int RemovalCount { get; }
        internal TransferClearReason? LifecycleReason { get; }
        internal bool IsIdle => !HadAim && !HadPress && RemovalCount == 0 && !LifecycleReason.HasValue;
    }

    // REQ-WT-003, REQ-WT-005, REQ-WT-007. The exact successful -200 result seam.
    internal readonly struct TransferPhasePublication
    {
        internal TransferPhasePublication(TransferTickOutcome outcome, TransferPhaseInputSummary input)
        {
            Outcome = outcome;
            Input = input;
        }

        internal TransferTickOutcome Outcome { get; }
        internal TransferPhaseInputSummary Input { get; }
        internal SimulationTick Tick => Outcome.Snapshot.Tick;
    }

    // REQ-WT-006, REQ-WT-008. Internal tick-indexed adapter input; no device or public provider participates.
    internal readonly struct TransferSimulationInput
    {
        internal TransferSimulationInput(SimulationTick tick, SimulationCameraPoseSnapshot? camera, AimSample? aim, TransferPressed? press, IReadOnlyList<TransferTargetRemoved> removals, TransferClearReason? lifecycle)
            : this(tick, camera, aim, press, removals, Array.Empty<TransferTargetExposureEnded>(), lifecycle) { }
        internal TransferSimulationInput(SimulationTick tick, SimulationCameraPoseSnapshot? camera, AimSample? aim, TransferPressed? press, IReadOnlyList<TransferTargetRemoved> removals, IReadOnlyList<TransferTargetExposureEnded> exposureEnds, TransferClearReason? lifecycle)
        {
            if (removals == null || exposureEnds == null) throw new ArgumentNullException(nameof(removals));
            if (aim.HasValue != camera.HasValue || (aim.HasValue && aim.Value.SampleTick != tick) || (press.HasValue && press.Value.Tick != tick)) throw new ArgumentException("Transfer input payloads must be exact-tick and camera/aim coupled.");
            for (var index = 0; index < removals.Count; index++) if (removals[index].Tick != tick) throw new ArgumentException("Removal tick must match transfer phase tick.");
            for (var index = 0; index < exposureEnds.Count; index++) if (exposureEnds[index].Tick != tick) throw new ArgumentException("Exposure-end tick must match transfer phase tick.");
            Tick = tick; Camera = camera; Aim = aim; Press = press; Lifecycle = lifecycle;
            var copy = new TransferTargetRemoved[removals.Count];
            for (var index = 0; index < copy.Length; index++) copy[index] = removals[index];
            Removals = Array.AsReadOnly(copy);
            var endCopy = new TransferTargetExposureEnded[exposureEnds.Count];
            for (var index = 0; index < endCopy.Length; index++) endCopy[index] = exposureEnds[index];
            ExposureEnds = Array.AsReadOnly(endCopy);
        }
        internal SimulationTick Tick { get; }
        internal SimulationCameraPoseSnapshot? Camera { get; }
        internal AimSample? Aim { get; }
        internal TransferPressed? Press { get; }
        internal IReadOnlyList<TransferTargetRemoved> Removals { get; }
        internal IReadOnlyList<TransferTargetExposureEnded> ExposureEnds { get; }
        internal TransferClearReason? Lifecycle { get; }
    }
}
