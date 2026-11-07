using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-003. Internal-only typed registration state; authoring validation
    // reads it but runtime never infers it from bindings or forecast content.
    internal enum OrdanBossTransferExposureConfigurationMode
    {
        Unconfigured = 0,
        LegacyPayloadOnly = 1,
        ProductionAuditAndPayload = 2
    }

    // REQ-COM-003, REQ-COM-004, REQ-WT-001, REQ-WT-005. Applies only the
    // immutable M4A next-tick forecast in the inter-tick -180 seam.
    [DefaultExecutionOrder(-180)]
    [DisallowMultipleComponent]
    internal sealed class OrdanBossTransferExposureScheduler : MonoBehaviour
    {
        [SerializeField] private OrdanBossSimulationDriver _bridge;
        [SerializeField] private TransferSimulationDriver _transferDriver;
        [SerializeField] private List<TransferTarget> _payloadTargets = new List<TransferTarget>();
        [SerializeField] private List<OrdanBossPayloadTransferModifierSink> _payloadSinks = new List<OrdanBossPayloadTransferModifierSink>();
        [SerializeField] private TransferTarget _auditTarget;
        [SerializeField] private OrdanBossAuditTransferModifierSink _auditSink;
        [SerializeField] private BoxCollider2D _auditCollider;

        private readonly object _laneOwner = new object();
        // REQ-COM-003. The serialized enum is an explicit authoring-time
        // registration latch; runtime never derives a mode from nulls, IDs, or
        // forecasts after the graph has been saved.
        [SerializeField] private OrdanBossTransferExposureConfigurationMode _configurationMode;
        private bool _initialized;
        private bool _terminal;
        private bool _hasLatest;
        private SimulationTick _nextSourceTick;
        private string _previousExposedTargetId = string.Empty;
        private OrdanBossTransferExposureOutcome _latest;
        private bool _hasLatestAudit;
        private OrdanBossAuditExposureOutcome _latestAudit;

        internal bool HasLatestOutcome => _hasLatest;
        internal OrdanBossTransferExposureOutcome LatestOutcome => _hasLatest ? _latest : throw new InvalidOperationException("Exposure scheduler has not published.");
        internal bool HasLatestAuditOutcome => _hasLatestAudit;
        internal OrdanBossAuditExposureOutcome LatestAuditOutcome => _hasLatestAudit ? _latestAudit : throw new InvalidOperationException("Audit exposure scheduler has not published.");
        internal bool IsTerminal => _terminal;
        // REQ-COM-003. Immutable authoring/read seam for exact typed-mode
        // verification; it has no setter, discovery, or runtime transition.
        internal OrdanBossTransferExposureConfigurationMode ConfigurationModeForAuthoring => _configurationMode;
        // REQ-COM-004. Read-only identities used by the authored graph handoff.
        internal OrdanBossSimulationDriver BoundBridge => _bridge;
        internal TransferSimulationDriver BoundTransferDriver => _transferDriver;
        internal TransferTarget BoundAuditTarget => _auditTarget;
        internal OrdanBossAuditTransferModifierSink BoundAuditSink => _auditSink;
        internal BoxCollider2D BoundAuditCollider => _auditCollider;

        private void Awake()
        {
            if (_bridge != null || _transferDriver != null || _payloadTargets.Count != 0 || _payloadSinks.Count != 0) Initialize();
        }
        private void FixedUpdate() => AdvanceSchedulerPhase();

        internal void ConfigureForTests(OrdanBossSimulationDriver bridge, TransferSimulationDriver transferDriver,
            TransferTarget[] targets, OrdanBossPayloadTransferModifierSink[] sinks) => Configure(bridge, transferDriver, targets, sinks);
        private void Configure(OrdanBossSimulationDriver bridge, TransferSimulationDriver transferDriver,
            TransferTarget[] targets, OrdanBossPayloadTransferModifierSink[] sinks)
        {
            if (_initialized) throw new InvalidOperationException("Exposure scheduler cannot be reconfigured after Awake.");
            _bridge = bridge; _transferDriver = transferDriver;
            _payloadTargets = targets == null ? null : new List<TransferTarget>(targets);
            _payloadSinks = sinks == null ? null : new List<OrdanBossPayloadTransferModifierSink>(sinks); _configurationMode = OrdanBossTransferExposureConfigurationMode.LegacyPayloadOnly;
        }
        // REQ-COM-001, REQ-COM-004. This is assignment-only and must run while
        // the inactive authored graph is pristine.
        internal void ConfigureForAuthoring(OrdanBossSimulationDriver bridge, TransferSimulationDriver transferDriver,
            TransferTarget[] targets, OrdanBossPayloadTransferModifierSink[] sinks) =>
            Configure(bridge, transferDriver, targets, sinks);
        internal void ConfigureForAuthoring(OrdanBossSimulationDriver bridge, TransferSimulationDriver transferDriver,
            TransferTarget[] targets, OrdanBossPayloadTransferModifierSink[] sinks, TransferTarget auditTarget, OrdanBossAuditTransferModifierSink auditSink, BoxCollider2D auditCollider)
        { Configure(bridge, transferDriver, targets, sinks); _auditTarget = auditTarget; _auditSink = auditSink; _auditCollider = auditCollider; _configurationMode = OrdanBossTransferExposureConfigurationMode.ProductionAuditAndPayload; }
        internal void InitializeForTests() => Initialize();
        internal OrdanBossTransferExposureOutcome AdvanceSchedulerPhaseForTests() => AdvanceSchedulerPhase();

        private void Initialize()
        {
            if (_initialized) return;
            ValidateBindingsAndPairs();
            _transferDriver.RegisterExposureEndLane(_laneOwner, IsProduction ? new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" } : new[] { "BossWeight-0", "BossWeight-1", "BossWeight-2" });
            CommitExposure(string.Empty, string.Empty);
            _nextSourceTick = _transferDriver.BoundPlayer.NextExpectedTick;
            _initialized = true;
        }

        private OrdanBossTransferExposureOutcome AdvanceSchedulerPhase()
        {
            Initialize();
            if (_terminal) throw new InvalidOperationException("Terminal exposure scheduler accepts no later advance.");
            var player = _transferDriver.BoundPlayer;
            var tick = player.NextExpectedTick;
            if (tick != _nextSourceTick || !_bridge.HasLatestOutcome || _bridge.BoundTransferDriver != _transferDriver ||
                !_transferDriver.LatestPhasePublication.HasValue || _transferDriver.LatestPhasePublication.Value.Tick != tick)
                throw new InvalidOperationException("Exposure scheduler requires exact current bridge and Transfer publications.");
            var bridge = _bridge.LatestOutcome;
            var forecast = bridge.Forecast; var auditForecast = bridge.AuditForecast;
            if (bridge.SourceTick != tick || forecast.SourceTick != tick || forecast.TransferTick.Value != checked(tick.Value + 1) || auditForecast.SourceTick != tick || auditForecast.TransferTick != forecast.TransferTick || (forecast.IsExposed && auditForecast.IsExposed))
                throw new InvalidOperationException("Exposure forecast horizon is not exact.");
            ValidateBindingsAndPairs();

            var terminal = IsExactDefeat(bridge.Handoffs, tick);
            var desired = terminal ? string.Empty : forecast.TargetId;
            var auditDesired = terminal || !IsProduction ? string.Empty : auditForecast.TargetId;
            if (!IsPayloadOrEmpty(desired) || (auditDesired.Length != 0 && auditDesired != "BossAuditBox-0")) throw new InvalidOperationException("Exposure forecast names an invalid target.");
            var ends = new List<TransferTargetExposureEnded>();
            var removals = new List<TransferTargetRemoved>();
            var transferTick = forecast.TransferTick;
            if (terminal)
            {
                if (IsProduction) removals.Add(new TransferTargetRemoved("BossAuditBox-0", transferTick));
                for (var index = 0; index < _payloadTargets.Count; index++) removals.Add(new TransferTargetRemoved(_payloadTargets[index].TargetId, transferTick));
            }
            else if (!string.Equals(_previousExposedTargetId, desired.Length != 0 ? desired : auditDesired, StringComparison.Ordinal) && _previousExposedTargetId.Length != 0)
            {
                ends.Add(new TransferTargetExposureEnded(_previousExposedTargetId, transferTick));
            }

            // Both Transfer queue mutations fully preflight/copy before their
            // replacement write. Temporary ends precede permanent terminal
            // removals by contract and are never combined for terminal state.
            if (ends.Count != 0) _transferDriver.MergeExposureEnds(_laneOwner, ends);
            if (removals.Count != 0)
            {
                if (IsProduction) _transferDriver.MergeExactBossTerminalRemovals(removals);
                else _transferDriver.MergeRemovals(removals);
            }
            CommitExposure(desired, auditDesired);
            var payloadRemovals = terminal ? removals.FindAll(value => value.TargetId.StartsWith("BossWeight-", StringComparison.Ordinal)) : removals;
            var payloadEnds = ends.FindAll(value => value.TargetId.StartsWith("BossWeight-", StringComparison.Ordinal));
            var auditEnds = ends.FindAll(value => value.TargetId == "BossAuditBox-0");
            var auditRemovals = removals.FindAll(value => value.TargetId == "BossAuditBox-0");
            var outcome = new OrdanBossTransferExposureOutcome(tick, transferTick, _previousExposedTargetId.StartsWith("BossWeight-", StringComparison.Ordinal) ? _previousExposedTargetId : string.Empty, desired, payloadEnds, payloadRemovals, terminal);
            if (IsProduction) { _latestAudit = new OrdanBossAuditExposureOutcome(tick, transferTick, _previousExposedTargetId == "BossAuditBox-0" ? _previousExposedTargetId : string.Empty, auditDesired, auditEnds.ConvertAll(value => value.TargetId), auditRemovals.ConvertAll(value => value.TargetId), terminal); _hasLatestAudit = true; }
            _previousExposedTargetId = desired.Length != 0 ? desired : auditDesired; _nextSourceTick = transferTick; _terminal = terminal; _latest = outcome; _hasLatest = true;
            return outcome;
        }

        private void ValidateBindingsAndPairs()
        {
            if (_configurationMode == OrdanBossTransferExposureConfigurationMode.Unconfigured || _bridge == null || _transferDriver == null || _bridge.BoundTransferDriver != _transferDriver ||
                _transferDriver.BoundPlayer == null || _transferDriver.BoundRegistry == null || _payloadTargets == null || _payloadSinks == null ||
                _payloadTargets.Count != 3 || _payloadSinks.Count != 3) throw new InvalidOperationException("Exposure scheduler requires exact bindings and three payload pairs.");
            var registry = _transferDriver.BoundRegistry;
            var payloadRegistryCount = 0;
            for (var registryIndex = 0; registryIndex < registry.Targets.Count; registryIndex++)
            {
                var registered = registry.Targets[registryIndex];
                if (registered == null) throw new InvalidOperationException("Payload registry cannot contain a missing target.");
                if (registered.TargetId.StartsWith("BossWeight-", StringComparison.Ordinal))
                {
                    if (registered.TargetId != "BossWeight-0" && registered.TargetId != "BossWeight-1" && registered.TargetId != "BossWeight-2")
                        throw new InvalidOperationException("Payload registry contains an additional scripted weight ID.");
                    payloadRegistryCount++;
                }
            }
            if (payloadRegistryCount != 3) throw new InvalidOperationException("Payload registry must contain each exact scripted weight once.");
            if (!IsProduction && (_auditTarget != null || _auditSink != null || _auditCollider != null))
                throw new InvalidOperationException("Legacy exposure configuration cannot infer or retain audit bindings.");
            if (IsProduction)
            {
                if (_auditTarget == null || _auditSink == null || _auditCollider == null || _auditTarget.TargetId != "BossAuditBox-0" ||
                    !ReferenceEquals(_auditTarget.TargetOwnerSink, _auditSink) || _auditTarget.TargetCollider != _auditCollider || !RegistryContains(registry, _auditTarget))
                    throw new InvalidOperationException("Audit target/sink bindings are not exact.");
                var auditDescriptor = _auditTarget.BuildDescriptorFromValidatedAuthoring();
                if (auditDescriptor.Kind != TransferTargetKind.Box ||
                    auditDescriptor.BaseModifierProfileId != OrdanBossAuditTransferModifierSink.RequiredBaseProfileId ||
                    _auditSink.BaseModifierProfileId != OrdanBossAuditTransferModifierSink.RequiredBaseProfileId)
                    throw new InvalidOperationException("Audit target authoring values are not exact.");
            }
            for (var index = 0; index < 3; index++)
            {
                var target = _payloadTargets[index]; var sink = _payloadSinks[index]; var expected = "BossWeight-" + index;
                if (target == null || sink == null || target.TargetId != expected || sink.BoundTargetId != expected || !ReferenceEquals(target.TargetOwnerSink, sink) ||
                    target.TargetCollider == null || !RegistryContains(registry, target))
                    throw new InvalidOperationException("Payload target/sink bindings are not exact.");
                var descriptor = target.BuildDescriptorFromValidatedAuthoring();
                if (descriptor.Kind != TransferTargetKind.BossPayload || descriptor.BaseModifierProfileId != OrdanBossPayloadTransferModifierSink.RequiredBaseProfileId ||
                    sink.BaseModifierProfileId != OrdanBossPayloadTransferModifierSink.RequiredBaseProfileId)
                    throw new InvalidOperationException("Payload target authoring values are not exact.");
            }
        }

        private bool IsProduction => _configurationMode == OrdanBossTransferExposureConfigurationMode.ProductionAuditAndPayload;
        private void CommitExposure(string desired, string auditDesired)
        {
            for (var index = 0; index < _payloadTargets.Count; index++)
            {
                var exposed = string.Equals(_payloadTargets[index].TargetId, desired, StringComparison.Ordinal);
                _payloadSinks[index].CommitExposure(exposed);
                _payloadTargets[index].TargetCollider.enabled = exposed;
            }
            if (IsProduction) { var exposed = auditDesired == "BossAuditBox-0"; _auditSink.CommitExposureForM4B3B(exposed); _auditCollider.enabled = exposed; }
        }
        private static bool RegistryContains(TransferTargetRegistry registry, TransferTarget target)
        {
            for (var index = 0; index < registry.Targets.Count; index++) if (registry.Targets[index] == target) return true;
            return false;
        }
        private static bool IsPayloadOrEmpty(string id) => string.IsNullOrEmpty(id) || id == "BossWeight-0" || id == "BossWeight-1" || id == "BossWeight-2";
        private static bool IsExactDefeat(IReadOnlyList<OrdanBossHandoff> handoffs, SimulationTick tick) => handoffs != null && handoffs.Count == 3 &&
            handoffs[0].Kind == OrdanBossHandoffKind.Defeated && handoffs[0].Tick == tick &&
            handoffs[1].Kind == OrdanBossHandoffKind.RewardRequest && handoffs[1].Tick == tick &&
            handoffs[2].Kind == OrdanBossHandoffKind.RoomCompletionRequest && handoffs[2].Tick == tick;
    }

    // REQ-COM-003, REQ-WT-005. Presentation-neutral immutable result of one
    // successful -180 adoption; it never retains Unity object references.
    internal readonly struct OrdanBossTransferExposureOutcome
    {
        internal OrdanBossTransferExposureOutcome(SimulationTick sourceTick, SimulationTick transferTick, string previousTargetId, string exposedTargetId,
            IReadOnlyList<TransferTargetExposureEnded> temporaryEnds, IReadOnlyList<TransferTargetRemoved> permanentRemovals, bool isTerminal)
        {
            if (transferTick.Value != checked(sourceTick.Value + 1) || !IsPayloadOrEmpty(previousTargetId) || !IsPayloadOrEmpty(exposedTargetId) || temporaryEnds == null || permanentRemovals == null)
                throw new ArgumentException("Exposure outcome is malformed.");
            previousTargetId = previousTargetId ?? string.Empty; exposedTargetId = exposedTargetId ?? string.Empty;
            ValidateShape(transferTick, previousTargetId, exposedTargetId, temporaryEnds, permanentRemovals, isTerminal);
            SourceTick = sourceTick; TransferTick = transferTick; PreviousTargetId = previousTargetId; ExposedTargetId = exposedTargetId; IsTerminal = isTerminal;
            TemporarilyEndedTargetIds = CopyEnds(temporaryEnds); PermanentlyRemovedTargetIds = CopyRemovals(permanentRemovals);
        }
        internal SimulationTick SourceTick { get; } internal SimulationTick TransferTick { get; }
        internal string PreviousTargetId { get; } internal string ExposedTargetId { get; } internal bool IsTerminal { get; }
        internal IReadOnlyList<string> TemporarilyEndedTargetIds { get; } internal IReadOnlyList<string> PermanentlyRemovedTargetIds { get; }
        private static IReadOnlyList<string> CopyEnds(IReadOnlyList<TransferTargetExposureEnded> values) { var copy = new string[values.Count]; for (var i = 0; i < copy.Length; i++) copy[i] = values[i].TargetId; return Array.AsReadOnly(copy); }
        private static IReadOnlyList<string> CopyRemovals(IReadOnlyList<TransferTargetRemoved> values) { var copy = new string[values.Count]; for (var i = 0; i < copy.Length; i++) copy[i] = values[i].TargetId; return Array.AsReadOnly(copy); }
        private static bool IsPayloadOrEmpty(string id) => string.IsNullOrEmpty(id) || id == "BossWeight-0" || id == "BossWeight-1" || id == "BossWeight-2";
        private static void ValidateShape(SimulationTick tick, string previous, string exposed, IReadOnlyList<TransferTargetExposureEnded> ends, IReadOnlyList<TransferTargetRemoved> removals, bool terminal)
        {
            if (terminal)
            {
                if (exposed.Length != 0 || ends.Count != 0 || removals.Count != 3) throw new ArgumentException("Terminal exposure outcome has an invalid shape.");
                for (var index = 0; index < 3; index++) if (removals[index].Tick != tick || removals[index].TargetId != "BossWeight-" + index) throw new ArgumentException("Terminal removals must be exact ordinal payload IDs.");
                return;
            }
            if (removals.Count != 0) throw new ArgumentException("Nonterminal exposure outcome cannot permanently remove a payload.");
            var changed = previous.Length != 0 && previous != exposed;
            if (ends.Count != (changed ? 1 : 0) || (changed && (ends[0].Tick != tick || ends[0].TargetId != previous)))
                throw new ArgumentException("Temporary exposure end does not match the prior payload.");
        }
    }

    // REQ-COM-003. Audit projection is separate so verified M4B2 payload evidence remains byte-for-byte shaped.
    internal readonly struct OrdanBossAuditExposureOutcome : IEquatable<OrdanBossAuditExposureOutcome>
    {
        internal OrdanBossAuditExposureOutcome(SimulationTick sourceTick, SimulationTick transferTick, string previousTargetId, string exposedTargetId,
            IReadOnlyList<string> ends, IReadOnlyList<string> removals, bool terminal)
        {
            previousTargetId = previousTargetId ?? string.Empty; exposedTargetId = exposedTargetId ?? string.Empty;
            if (transferTick.Value != checked(sourceTick.Value + 1) || (previousTargetId.Length != 0 && previousTargetId != "BossAuditBox-0") ||
                (exposedTargetId.Length != 0 && exposedTargetId != "BossAuditBox-0") || ends == null || removals == null ||
                (terminal && (exposedTargetId.Length != 0 || ends.Count != 0 || removals.Count != 1 || removals[0] != "BossAuditBox-0")) ||
                (!terminal && (removals.Count != 0 || ends.Count != (previousTargetId.Length != 0 && previousTargetId != exposedTargetId ? 1 : 0)))) throw new ArgumentException("Audit exposure outcome is malformed.");
            SourceTick = sourceTick; TransferTick = transferTick; PreviousTargetId = previousTargetId; ExposedTargetId = exposedTargetId; IsTerminal = terminal;
            var endCopy = new string[ends.Count]; for (var i = 0; i < endCopy.Length; i++) { if (ends[i] != "BossAuditBox-0") throw new ArgumentException(); endCopy[i] = ends[i]; }
            var removalCopy = new string[removals.Count]; for (var i = 0; i < removalCopy.Length; i++) { if (removals[i] != "BossAuditBox-0") throw new ArgumentException(); removalCopy[i] = removals[i]; }
            TemporarilyEndedTargetIds = Array.AsReadOnly(endCopy); PermanentlyRemovedTargetIds = Array.AsReadOnly(removalCopy);
        }
        internal SimulationTick SourceTick { get; } internal SimulationTick TransferTick { get; } internal string PreviousTargetId { get; } internal string ExposedTargetId { get; } internal bool IsTerminal { get; }
        internal IReadOnlyList<string> TemporarilyEndedTargetIds { get; } internal IReadOnlyList<string> PermanentlyRemovedTargetIds { get; }
        public bool Equals(OrdanBossAuditExposureOutcome other) =>
            SourceTick == other.SourceTick && TransferTick == other.TransferTick &&
            string.Equals(PreviousTargetId, other.PreviousTargetId, StringComparison.Ordinal) &&
            string.Equals(ExposedTargetId, other.ExposedTargetId, StringComparison.Ordinal) && IsTerminal == other.IsTerminal &&
            SameIds(TemporarilyEndedTargetIds, other.TemporarilyEndedTargetIds) &&
            SameIds(PermanentlyRemovedTargetIds, other.PermanentlyRemovedTargetIds);
        public override bool Equals(object obj) => obj is OrdanBossAuditExposureOutcome other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = SourceTick.GetHashCode(); hash = (hash * 397) ^ TransferTick.GetHashCode();
                hash = (hash * 397) ^ (PreviousTargetId == null ? 0 : PreviousTargetId.GetHashCode());
                hash = (hash * 397) ^ (ExposedTargetId == null ? 0 : ExposedTargetId.GetHashCode()); hash = (hash * 397) ^ IsTerminal.GetHashCode();
                hash = AddIds(hash, TemporarilyEndedTargetIds); return AddIds(hash, PermanentlyRemovedTargetIds);
            }
        }
        public static bool operator ==(OrdanBossAuditExposureOutcome left, OrdanBossAuditExposureOutcome right) => left.Equals(right);
        public static bool operator !=(OrdanBossAuditExposureOutcome left, OrdanBossAuditExposureOutcome right) => !left.Equals(right);
        private static bool SameIds(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null || left.Count != right.Count) return false;
            for (var index = 0; index < left.Count; index++) if (!string.Equals(left[index], right[index], StringComparison.Ordinal)) return false;
            return true;
        }
        private static int AddIds(int hash, IReadOnlyList<string> values)
        {
            if (values == null) return hash;
            for (var index = 0; index < values.Count; index++) hash = (hash * 397) ^ (values[index] == null ? 0 : values[index].GetHashCode());
            return hash;
        }
    }
}
