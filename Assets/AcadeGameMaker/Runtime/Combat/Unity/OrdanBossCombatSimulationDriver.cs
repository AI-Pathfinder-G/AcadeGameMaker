using System;
using System.Collections.Generic;
using System.Globalization;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-001, REQ-COM-004, REQ-COM-006. Boss-only -190 M2A/M1 owner.
    // It intentionally does not alter the verified regular-enemy driver.
    [DefaultExecutionOrder(-190)]
    [DisallowMultipleComponent]
    internal sealed class OrdanBossCombatSimulationDriver : MonoBehaviour
    {
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private CombatTargetRegistry _registry;
        [SerializeField] private TransferSimulationDriver _transferDriver;
        [SerializeField] private OrdanBossHostileDamageProducer _hostileProducer;
        [SerializeField] private List<MonoBehaviour> _bossCombatOwners = new List<MonoBehaviour>();

        private SortedDictionary<int, OrdanBossCombatInput> _inputs = new SortedDictionary<int, OrdanBossCombatInput>();
        private readonly SortedDictionary<int, OrdanBossDeliveryBatch> _deliveries = new SortedDictionary<int, OrdanBossDeliveryBatch>();
        private readonly object _deliveryOwner = new object();
        private readonly object _hostileAppendOwner = new object();
        private readonly object _terminalDiscardIssuer = new object();
        private IReadOnlyList<FrozenCombatTarget> _frozenTargets;
        private CombatSession _combatSession;
        private BasicAttackSession _attackSession;
        // REQ-COM-004. First -190 work stays private until the -185 bridge
        // has preflighted and committed its M4A/batch pair.
        private CombatSession _provisionalCombatSession;
        private BasicAttackSession _provisionalAttackSession;
        private OrdanBossCombatOutcome _provisionalOutcome;
        private bool _hasProvisionalOutcome;
        private bool _prepared;
        private object _inputGateOwner;
        private readonly object _inputFrameIssuer = new object();
        private bool _inputGateLocked;
        private long _inputGeneration;
        private IInputFrameCommitSource _coordinatedInputSource;
        private InputFrameCommitReceipt? _pendingCoordinatedReceipt;
        private long _lastConsumedCoordinatedOrdinal = -1;
        private long _lastCoordinatedModeEpoch;
        private bool _hasCoordinatedModeEpoch;
        private bool _coordinatedLaneClosed;
        private bool _registered;
        private bool _attempted;
        private SimulationTick _firstSourceTick;
        private bool _hasLatestOutcome;
        private OrdanBossCombatOutcome _latestOutcome;

        // REQ-UX-004, REQ-UX-006, REQ-UX-008. The staged dictionary is opaque
        // and instance-bound so a router cannot replay or cross-wire a frame.
        internal sealed class InputFrameCandidate
        {
            private readonly OrdanBossCombatSimulationDriver _consumer;
            private readonly object _issuer;
            private readonly object _owner;
            private readonly SimulationTick _tick;
            private readonly long _generation;
            private readonly long _nextGeneration;
            private readonly SortedDictionary<int, OrdanBossCombatInput> _inputs;
            private readonly bool _locked;
            private readonly int _strippedCount;
            private readonly InputFrameCommitReceipt? _coordinatedReceipt;
            private bool _committed;

            internal InputFrameCandidate(OrdanBossCombatSimulationDriver consumer, object issuer, object owner, SimulationTick tick,
                long generation, long nextGeneration, SortedDictionary<int, OrdanBossCombatInput> inputs,
                bool locked, int strippedCount, InputFrameCommitReceipt? coordinatedReceipt = null)
            {
                _consumer = consumer; _issuer = issuer; _owner = owner; _tick = tick; _generation = generation;
                _nextGeneration = nextGeneration; _inputs = inputs; _locked = locked; _strippedCount = strippedCount; _coordinatedReceipt = coordinatedReceipt;
            }

            internal bool Matches(OrdanBossCombatSimulationDriver consumer, object issuer, object owner, SimulationTick tick, long generation) =>
                !_committed && ReferenceEquals(_consumer, consumer) && ReferenceEquals(_issuer, issuer) && ReferenceEquals(_owner, owner) && _tick == tick && _generation == generation;
            internal SimulationTick Tick => _tick;
            internal int ApplyToMatchingConsumer(OrdanBossCombatSimulationDriver consumer, object issuer, object owner)
            {
                if (consumer == null)
                    throw new InvalidOperationException("Boss Combat input-frame candidate has no consumer.");
                if (!ReferenceEquals(issuer, consumer._inputFrameIssuer))
                    throw new InvalidOperationException("Boss Combat input-frame candidate issuer is invalid.");
                consumer.ValidateInputFrameCandidate(owner, this);
                consumer.ValidateProposedCoordinatedReceipt(owner, _tick, _locked, _coordinatedReceipt);
                if (!Matches(consumer, consumer._inputFrameIssuer, owner, _tick, consumer._inputGeneration))
                    throw new InvalidOperationException("Boss Combat input-frame candidate is foreign, stale, consumed, or forged.");
                consumer._inputs = _inputs;
                consumer._inputGateLocked = _locked;
                consumer._inputGeneration = _nextGeneration;
                if (_coordinatedReceipt.HasValue) consumer._pendingCoordinatedReceipt = _coordinatedReceipt;
                _committed = true;
                return _strippedCount;
            }
        }

        internal PlayerMovementController BoundPlayer => _player;
        internal TransferSimulationDriver BoundTransferDriver => _transferDriver;
        internal CombatTargetRegistry BoundRegistry => _registry;
        internal OrdanBossHostileDamageProducer BoundHostileProducer => _hostileProducer;
        internal bool HasLatestOutcome => _hasLatestOutcome;
        // REQ-UX-004, REQ-UX-008. The router receives only this completed
        // death fact; it does not infer Run success or inspect combat payloads.
        internal bool HasCompletedBossDeath
        {
            get
            {
                if (!_hasLatestOutcome) return false;
                for (var index = 0; index < _latestOutcome.Combatants.Count; index++)
                {
                    var combatant = _latestOutcome.Combatants[index];
                    if (string.Equals(combatant.TargetId, "ordan", StringComparison.Ordinal)) return combatant.IsDead;
                }
                return false;
            }
        }
        internal bool HasCurrentOutcome => _hasLatestOutcome || _hasProvisionalOutcome;
        internal OrdanBossCombatOutcome LatestOutcome => _hasLatestOutcome ? _latestOutcome : throw new InvalidOperationException("Boss Combat has not published.");
        internal OrdanBossCombatOutcome CurrentOutcomeForBridge => _hasLatestOutcome ? _latestOutcome : _hasProvisionalOutcome ? _provisionalOutcome : throw new InvalidOperationException("Boss Combat has no current staged outcome.");
        internal SimulationTick NextExpectedTick => ActiveCombatSession == null ? _player.NextExpectedTick : ActiveCombatSession.NextExpectedTick;
        internal bool IsInputGateLocked => _inputGateLocked;
        internal bool IsCoordinatedInputLaneClosed => _coordinatedLaneClosed;
        internal InputFrameCommitReceipt? CurrentCoordinatedReceipt => _coordinatedInputSource == null ? (InputFrameCommitReceipt?)null : _coordinatedInputSource.CurrentReceipt;
        internal void ValidateCoordinatedInputRegistration(object owner, IInputFrameCommitSource source, SimulationTick firstTick)
        {
            var combat = ActiveCombatSession;
            var attack = ActiveAttackSession;
            if (owner == null || source == null || source.IsFaulted || !ReferenceEquals(owner, source) || !_prepared || !_registered || _coordinatedLaneClosed || _player == null ||
                combat == null || attack == null || firstTick != _player.NextExpectedTick || firstTick != combat.NextExpectedTick || firstTick != attack.NextExpectedTick)
                throw new InvalidOperationException("Boss Combat coordinated input registration is not ready or identity-bound.");
            if ((_inputGateOwner != null && !ReferenceEquals(_inputGateOwner, owner)) || (_coordinatedInputSource != null && !ReferenceEquals(_coordinatedInputSource, source)))
                throw new InvalidOperationException("Boss Combat coordinated input registration conflicts with an existing owner.");
        }
        internal void RegisterCoordinatedInputOwner(object owner, IInputFrameCommitSource source, SimulationTick firstTick)
        {
            ValidateCoordinatedInputRegistration(owner, source, firstTick); RegisterInputGateOwner(owner); _coordinatedInputSource = source;
        }

        private void FixedUpdate() => AdvanceCombatPhase();

        // REQ-COM-004. Authoring/tests use the same noninitializing seam.
        internal void ConfigureForTests(PlayerMovementController player, CombatTargetRegistry registry, TransferSimulationDriver transferDriver,
            params MonoBehaviour[] owners) => Configure(player, registry, transferDriver, owners);
        private void Configure(PlayerMovementController player, CombatTargetRegistry registry, TransferSimulationDriver transferDriver,
            MonoBehaviour[] owners)
        {
            if (_prepared || _attempted) throw new InvalidOperationException("Boss Combat cannot be reconfigured after preparation.");
            _player = player; _registry = registry; _transferDriver = transferDriver;
            _bossCombatOwners = owners == null ? null : new List<MonoBehaviour>(owners);
        }

        // REQ-COM-001, REQ-COM-004. Editor authoring uses the same assignment-only
        // path as focused fixtures; preparation remains bridge-owned.
        internal void ConfigureForAuthoring(PlayerMovementController player, CombatTargetRegistry registry, TransferSimulationDriver transferDriver,
            params MonoBehaviour[] owners) => Configure(player, registry, transferDriver, owners);
        // REQ-COM-004. Assignment-only authored identity for the one -170 producer.
        internal void ConfigureHostileProducerForAuthoring(OrdanBossHostileDamageProducer producer) { if (_prepared || _attempted) throw new InvalidOperationException(); _hostileProducer = producer; }
        internal void ConfigureHostileProducerForTests(OrdanBossHostileDamageProducer producer) => ConfigureHostileProducerForAuthoring(producer);

        // REQ-COM-004. Called by the -185 bridge Awake before any combat attempt.
        internal IReadOnlyList<FrozenCombatTarget> PrepareBossDeliveryRegistration()
        {
            if (_prepared || _registered || _attempted || _deliveries.Count != 0) throw new InvalidOperationException("Boss delivery preparation requires a pristine Combat owner.");
            ValidateGraph();
            // REQ-COM-004. Read-only authoring validation returns a staged
            // immutable roster; boss ownership retains this staged copy and
            // never mutates the shared registry freeze latch during bootstrap.
            var roster = _registry.ReadValidatedAuthoringRoster();
            ValidateBossRoster(roster);
            var first = _player.NextExpectedTick;
            var combat = new CombatSession(first);
            var attack = new BasicAttackSession(first);
            for (var index = 0; index < roster.Count; index++) combat.Register(roster[index].CombatantRegistration);
            _frozenTargets = roster; _provisionalCombatSession = combat; _provisionalAttackSession = attack; _prepared = true;
            return roster;
        }

        internal void RegisterBossDeliveryLane(SimulationTick firstSourceTick)
        {
            if (!_prepared || _registered || _attempted || _deliveries.Count != 0 || firstSourceTick.Value < 0 ||
                firstSourceTick != _player.NextExpectedTick || firstSourceTick != ActiveCombatSession.NextExpectedTick || firstSourceTick != ActiveAttackSession.NextExpectedTick)
                throw new InvalidOperationException("Boss delivery lane registration must be one-time and tick-aligned.");
            _firstSourceTick = firstSourceTick; _registered = true;
        }

        internal void Submit(OrdanBossCombatInput input)
        {
            var accepted = _inputGateOwner == null
                ? input
                : NormalizeInput(input, _inputGateLocked);
            if (!_prepared || accepted.Tick.Value < _player.NextExpectedTick.Value || _inputs.ContainsKey(accepted.Tick.Value))
                throw new InvalidOperationException("Boss Combat accepts one exact future input.");
            var nextGeneration = checked(_inputGeneration + 1);
            _inputs.Add(accepted.Tick.Value, accepted);
            _inputGeneration = nextGeneration;
        }

        // REQ-UX-004, REQ-UX-008, REQ-COM-001, REQ-COM-004. The boss
        // consumer accepts one future InputMode owner without global state.
        internal void RegisterInputGateOwner(object owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (_inputGateOwner != null && !ReferenceEquals(_inputGateOwner, owner))
            {
                throw new InvalidOperationException("A different Boss Combat input-gate owner is already registered.");
            }

            _inputGateOwner = owner;
        }

        // REQ-UX-004, REQ-UX-008, REQ-COM-001, REQ-COM-004. Staging leaves
        // M1 external damage and delivery lanes intact while removing only input.
        internal int ApplyInputGate(object owner, SimulationTick tick, bool locked)
        {
            if (_inputGateOwner == null || !ReferenceEquals(_inputGateOwner, owner))
            {
                throw new InvalidOperationException("The caller is not the registered Boss Combat input-gate owner.");
            }

            var combat = ActiveCombatSession;
            var attack = ActiveAttackSession;
            if (!_prepared || _player == null || tick.Value < 0 || tick != _player.NextExpectedTick ||
                combat == null || attack == null || combat.NextExpectedTick != tick || attack.NextExpectedTick != tick)
            {
                throw new InvalidOperationException("Boss Combat input-gate application requires a prepared unconsumed exact phase.");
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

            var staged = new SortedDictionary<int, OrdanBossCombatInput>();
            var strippedCount = 0;
            foreach (var entry in _inputs)
            {
                var validated = NormalizeInput(entry.Value, false);
                if (entry.Key != validated.Tick.Value)
                {
                    throw new InvalidOperationException("Queued Boss Combat input dictionary key does not match its tick.");
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

        internal InputFrameCandidate PrepareInputFrame(object owner, SimulationTick tick, bool locked, OrdanBossCombatInput? input) => PrepareInputFrameInternal(owner, tick, locked, input, null);
        internal InputFrameCandidate PrepareInputFrame(object owner, SimulationTick tick, bool locked, OrdanBossCombatInput? input, InputFrameCommitReceipt receipt) => PrepareInputFrameInternal(owner, tick, locked, input, receipt);
        private InputFrameCandidate PrepareInputFrameInternal(object owner, SimulationTick tick, bool locked, OrdanBossCombatInput? input, InputFrameCommitReceipt? receipt)
        {
            EnsureInputFrameOwner(owner, tick);
            ValidateProposedCoordinatedReceipt(owner, tick, locked, receipt);
            if (locked && input.HasValue) throw new InvalidOperationException("A locked Boss Combat input frame cannot carry input.");
            var staged = CopyInputQueue();
            var changed = _inputGateLocked != locked;
            var stripped = 0;
            if (locked && changed)
            {
                var strippedInputs = new SortedDictionary<int, OrdanBossCombatInput>();
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
                if (accepted.Tick != tick || accepted.ExternalDamageRequests.Count != 0)
                    throw new InvalidOperationException("Boss Combat input-frame input may contain only exact interactive fields.");
                if (locked || (staged.TryGetValue(tick.Value, out var existing) && HasInteractiveInput(existing)))
                    throw new InvalidOperationException("Boss Combat input-frame input conflicts with the exact tick.");
                staged[tick.Value] = staged.TryGetValue(tick.Value, out existing)
                    ? new OrdanBossCombatInput(tick, accepted.Camera, accepted.Aim, accepted.Press, existing.ExternalDamageRequests)
                    : accepted;
                changed = true;
            }
            var next = checked(_inputGeneration + 1);
            return new InputFrameCandidate(this, _inputFrameIssuer, owner, tick, _inputGeneration, next, staged, locked, stripped, receipt);
        }

        internal void ValidateInputFrameCandidate(object owner, InputFrameCandidate candidate)
        {
            if (candidate == null)
                throw new InvalidOperationException("Boss Combat input-frame candidate is null.");
            EnsureInputFrameOwner(owner, candidate.Tick);
            if (!candidate.Matches(this, _inputFrameIssuer, owner, candidate.Tick, _inputGeneration))
                throw new InvalidOperationException("Boss Combat input-frame candidate is foreign, stale, consumed, or forged.");
        }

        internal int CommitInputFrame(object owner, InputFrameCandidate candidate)
        {
            ValidateInputFrameCandidate(owner, candidate);
            return candidate.ApplyToMatchingConsumer(this, _inputFrameIssuer, owner);
        }

        private void EnsureInputFrameOwner(object owner, SimulationTick tick)
        {
            if (owner == null || _inputGateOwner == null || !ReferenceEquals(_inputGateOwner, owner))
                throw new InvalidOperationException("The caller is not the registered Boss Combat input-frame owner.");
            var combat = ActiveCombatSession;
            var attack = ActiveAttackSession;
            if (!_prepared || _player == null || tick.Value < 0 || tick != _player.NextExpectedTick || combat == null || attack == null ||
                combat.NextExpectedTick != tick || attack.NextExpectedTick != tick)
                throw new InvalidOperationException("Boss Combat input frame requires a prepared unconsumed exact phase.");
        }

        private SortedDictionary<int, OrdanBossCombatInput> CopyInputQueue()
        {
            var staged = new SortedDictionary<int, OrdanBossCombatInput>();
            foreach (var entry in _inputs)
            {
                var accepted = NormalizeInput(entry.Value, false);
                if (entry.Key != accepted.Tick.Value) throw new InvalidOperationException("Queued Boss Combat input dictionary key does not match its tick.");
                staged.Add(entry.Key, accepted);
            }
            return staged;
        }

        internal OrdanBossDeliveryCandidate PreflightBossDelivery(SimulationTick sourceTick, DamageRequest? request)
        {
            if (!_registered || !HasCurrentOutcome || sourceTick != CurrentOutcomeForBridge.Tick || sourceTick.Value != checked(ActiveCombatSession.NextExpectedTick.Value - 1))
                throw new InvalidOperationException("Boss delivery source must be the completed current Combat tick.");
            var delivery = new SimulationTick(checked(sourceTick.Value + 1));
            if (_deliveries.ContainsKey(delivery.Value)) throw new InvalidOperationException("Boss delivery tick is already occupied.");
            if (request.HasValue) ValidatePayloadRequest(sourceTick, delivery, request.Value);
            return new OrdanBossDeliveryCandidate(_deliveryOwner, sourceTick, delivery, request);
        }

        internal void CommitPreflightedBossDelivery(OrdanBossDeliveryCandidate candidate)
        {
            if (candidate == null || !candidate.IsOwnedBy(_deliveryOwner) || !_registered || !HasCurrentOutcome ||
                candidate.SourceTick != CurrentOutcomeForBridge.Tick || candidate.DeliveryTick != ActiveCombatSession.NextExpectedTick || _deliveries.ContainsKey(candidate.DeliveryTick.Value))
                throw new InvalidOperationException("Boss delivery candidate is forged, stale, or conflicting.");
            if (candidate.Request.HasValue) ValidatePayloadRequest(candidate.SourceTick, candidate.DeliveryTick, candidate.Request.Value);
            _deliveries.Add(candidate.DeliveryTick.Value, new OrdanBossDeliveryBatch(candidate.SourceTick, candidate.DeliveryTick, candidate.Request));
        }

        // REQ-COM-004. Atomic append replaces the exact bridge-owned pending batch only.
        internal OrdanBossHostileDeliveryCandidate PreflightHostileDeliveryAppend(OrdanBossHostileDamageProducer producer, SimulationTick sourceTick, IReadOnlyList<DamageRequest> hostile)
        {
            if (!ReferenceEquals(producer, _hostileProducer) || hostile == null || hostile.Count > 1 || !_deliveries.TryGetValue(checked(sourceTick.Value + 1), out var batch) || batch.SourceTick != sourceTick || batch.HostileAppendCommitted) throw new InvalidOperationException("Hostile append is invalid.");
            var copy = CopyHostile(sourceTick, batch.DeliveryTick, hostile); return new OrdanBossHostileDeliveryCandidate(_hostileAppendOwner, batch, copy);
        }
        internal void CommitPreflightedHostileDeliveryAppend(OrdanBossHostileDeliveryCandidate candidate)
        { if (candidate == null || !candidate.IsOwnedBy(_hostileAppendOwner) || !_deliveries.TryGetValue(candidate.Batch.DeliveryTick.Value, out var batch) || !batch.Equals(candidate.Batch) || batch.HostileAppendCommitted) throw new InvalidOperationException("Hostile candidate is stale."); _deliveries[candidate.Batch.DeliveryTick.Value] = new OrdanBossDeliveryBatch(batch.SourceTick, batch.DeliveryTick, batch.Request, candidate.HostileRequests, true); }
        internal OrdanBossDeliveryBatch ReadPendingDeliveryForTests(SimulationTick deliveryTick)
        {
            if (!_deliveries.TryGetValue(deliveryTick.Value, out var batch)) throw new InvalidOperationException("Pending boss delivery does not exist.");
            return new OrdanBossDeliveryBatch(batch.SourceTick, batch.DeliveryTick, batch.Request, batch.HostileRequests, batch.HostileAppendCommitted);
        }
        // REQ-COM-004. The -195 terminal coordinator can discard only the
        // bridge's exact empty t+1 delivery, after all checks have completed.
        internal OrdanBossTerminalDeliveryDiscardCandidate PreflightTerminalDeliveryDiscard(object owner, SimulationTick deathSourceTick) =>
            PreflightTerminalDeliveryDiscard(owner, deathSourceTick, null);

        internal OrdanBossTerminalDeliveryDiscardCandidate PreflightTerminalDeliveryDiscard(object owner, SimulationTick deathSourceTick, InputFrameCommitReceipt? receipt)
        {
            if (owner == null || deathSourceTick.Value < 0) throw new ArgumentException("Terminal discard owner is invalid.");
            var deliveryTick = new SimulationTick(checked(deathSourceTick.Value + 1));
            if (_deliveries.Count != 1 || !_deliveries.TryGetValue(deliveryTick.Value, out var batch) ||
                batch.SourceTick != deathSourceTick || batch.DeliveryTick != deliveryTick || batch.Request.HasValue ||
                batch.HostileRequests.Count != 0 || batch.HostileAppendCommitted)
                throw new InvalidOperationException("Boss terminal delivery is not the exact empty batch.");
            foreach (var pair in _inputs)
            {
                if (pair.Key > deliveryTick.Value) throw new InvalidOperationException("Boss terminal discard cannot cross a future input.");
                if (pair.Key != deliveryTick.Value || !IsExactEmptyInput(pair.Value, deliveryTick)) throw new InvalidOperationException("Boss terminal discard input is not empty.");
            }
            ValidateTerminalCoordinatedProof(deliveryTick, receipt);
            return new OrdanBossTerminalDeliveryDiscardCandidate(_terminalDiscardIssuer, owner, batch, _inputs.ContainsKey(deliveryTick.Value), receipt);
        }
        internal void CommitPreflightedTerminalDeliveryDiscard(OrdanBossTerminalDeliveryDiscardCandidate candidate, object owner)
        {
            if (candidate == null || owner == null || !candidate.IsIssuedBy(_terminalDiscardIssuer) || !candidate.IsOwnedBy(owner) ||
                !IsExactTerminalDiscardQueue(candidate.Batch, candidate.HadEmptyInput))
                throw new InvalidOperationException("Boss terminal discard candidate is forged or stale.");
            var nextGeneration = candidate.HadEmptyInput ? checked(_inputGeneration + 1) : _inputGeneration;
            ValidateTerminalCoordinatedProof(candidate.Batch.DeliveryTick, candidate.CoordinatedReceipt);
            _deliveries.Remove(candidate.Batch.DeliveryTick.Value);
            if (candidate.HadEmptyInput) { _inputs.Remove(candidate.Batch.DeliveryTick.Value); _inputGeneration = nextGeneration; }
            if (candidate.CoordinatedReceipt.HasValue)
            {
                _lastConsumedCoordinatedOrdinal = candidate.CoordinatedReceipt.Value.FrameOrdinal;
                _lastCoordinatedModeEpoch = candidate.CoordinatedReceipt.Value.ModeEpoch;
                _hasCoordinatedModeEpoch = true;
                _pendingCoordinatedReceipt = null;
                _coordinatedLaneClosed = true;
            }
        }

        private void ValidateTerminalCoordinatedProof(SimulationTick tick, InputFrameCommitReceipt? receipt)
        {
            if (_coordinatedInputSource == null)
            {
                if (receipt.HasValue) throw new InvalidOperationException("Uncoordinated Combat cannot close a coordinated proof.");
                return;
            }
            if (_coordinatedInputSource.IsFaulted || _coordinatedLaneClosed || !receipt.HasValue || !_pendingCoordinatedReceipt.HasValue || !_coordinatedInputSource.CurrentReceipt.HasValue || receipt.Value != _pendingCoordinatedReceipt.Value || receipt.Value != _coordinatedInputSource.CurrentReceipt.Value || receipt.Value.Tick != tick || receipt.Value.FrameOrdinal != checked(_lastConsumedCoordinatedOrdinal + 1) || (!_hasCoordinatedModeEpoch && receipt.Value.ModeEpoch != 0) || (_hasCoordinatedModeEpoch && (receipt.Value.ModeEpoch < _lastCoordinatedModeEpoch || (receipt.Value.ModeEpoch != _lastCoordinatedModeEpoch && (_lastCoordinatedModeEpoch == long.MaxValue || receipt.Value.ModeEpoch != _lastCoordinatedModeEpoch + 1)))) || receipt.Value.Mode == InputMode.GameplayEnabled || !_inputGateLocked)
                throw new InvalidOperationException("Boss Combat terminal coordinated proof is invalid.");
        }
        private static bool IsExactEmptyInput(OrdanBossCombatInput input, SimulationTick tick) => input.Tick == tick && !input.Camera.HasValue && !input.Aim.HasValue && !input.Press.HasValue && input.ExternalDamageRequests.Count == 0;
        private bool IsExactTerminalDiscardQueue(OrdanBossDeliveryBatch expected, bool hadEmptyInput)
        {
            if (_deliveries.Count != 1 || !_deliveries.TryGetValue(expected.DeliveryTick.Value, out var current) || !current.Equals(expected)) return false;
            var sawInput = false;
            foreach (var pair in _inputs)
            {
                if (pair.Key != expected.DeliveryTick.Value || !IsExactEmptyInput(pair.Value, expected.DeliveryTick)) return false;
                sawInput = true;
            }
            return sawInput == hadEmptyInput;
        }
        private static DamageRequest[] CopyHostile(SimulationTick source, SimulationTick delivery, IReadOnlyList<DamageRequest> values)
        { var copy = new DamageRequest[values.Count]; for (var i=0;i<copy.Length;i++) { var v=values[i]; if (v.Tick!=delivery || delivery.Value!=checked(source.Value+1) || v.SourceId!="ordan" || v.TargetId!="player" || v.Amount!=1 || v.Kind!=DamageKind.BossPattern || !IsExactHostileRequestId(v.RequestId, delivery)) throw new ArgumentException(); copy[i]=v; } return copy; }

        private static bool IsExactHostileRequestId(string requestId, SimulationTick delivery)
        {
            if (string.IsNullOrEmpty(requestId) || requestId.Length > 96) return false;
            var parts = requestId.Split('/');
            if ((parts.Length != 4 && parts.Length != 5) || parts[0] != "ordan.hostile" ||
                !IsAsciiDigits(parts[parts.Length - 1], 10) ||
                parts[parts.Length - 1] != delivery.Value.ToString("D10", CultureInfo.InvariantCulture) ||
                !IsAsciiDigits(parts[2], 19) || !long.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out _))
                return false;
            if (parts.Length == 4)
                return parts[1] == "debt-a" || parts[1] == "debt-b" || parts[1] == "audit";
            return parts[1] == "seizure" &&
                (parts[3] == "BossWeight-0" || parts[3] == "BossWeight-1" || parts[3] == "BossWeight-2");
        }

        private static bool IsAsciiDigits(string value, int length)
        {
            if (value == null || value.Length != length) return false;
            for (var index = 0; index < value.Length; index++)
                if (value[index] < '0' || value[index] > '9') return false;
            return true;
        }

        // REQ-COM-004. Only the bridge may publish the first staged -190
        // result; no failed bootstrap exposes M1/M2A sessions or latest output.
        internal void PreflightBootstrapPromotion(SimulationTick sourceTick)
        {
            if (_hasLatestOutcome || !_hasProvisionalOutcome || sourceTick != _provisionalOutcome.Tick ||
                _provisionalCombatSession == null || _provisionalAttackSession == null)
                throw new InvalidOperationException("Boss bootstrap promotion requires its exact provisional outcome.");
        }

        internal void PromoteBootstrap(SimulationTick sourceTick)
        {
            PreflightBootstrapPromotion(sourceTick);
            _combatSession = _provisionalCombatSession; _attackSession = _provisionalAttackSession; _latestOutcome = _provisionalOutcome;
            _provisionalCombatSession = null; _provisionalAttackSession = null; _hasProvisionalOutcome = false; _hasLatestOutcome = true;
        }

        internal void DiscardFailedBootstrap()
        {
            if (_hasLatestOutcome || !_hasProvisionalOutcome) return;
            _provisionalCombatSession = null; _provisionalAttackSession = null; _hasProvisionalOutcome = false;
        }

        internal OrdanBossCombatOutcome AdvanceCombatPhaseForTests() => AdvanceCombatPhase();

        private OrdanBossCombatOutcome AdvanceCombatPhase()
        {
            if (_coordinatedInputSource != null)
            {
                if (_player == null) throw new InvalidOperationException("Coordinated Boss Combat requires a bound player.");
                ConsumeCoordinatedReceipt(_player.NextExpectedTick);
            }
            _attempted = true;
            if (!_registered) throw new InvalidOperationException("Boss Combat requires its bridge-owned delivery lane before the first attempt.");
            var tick = _player.NextExpectedTick;
            var combatSession = ActiveCombatSession; var attackSession = ActiveAttackSession;
            if (combatSession == null || attackSession == null || tick != combatSession.NextExpectedTick || tick != attackSession.NextExpectedTick) throw new InvalidOperationException("Boss Combat ticks are not aligned.");
            var input = ConsumeOrEmpty(tick);
            var delivery = PreflightIncomingDelivery(tick);
            PreflightInputAndReceipt(tick, input);
            var alive = CaptureAttackability();
            var observations = input.Aim.HasValue
                ? new CombatObservationBuilder().Capture(tick, _player.Snapshot, input.Camera.Value, input.Aim.Value, _frozenTargets, alive, _player.transform)
                : Array.Empty<BasicAttackCandidateObservation>();
            var attackCandidate = attackSession.PreviewNext(tick, input.Aim, observations, input.Press);
            var requests = MergeRequests(input.ExternalDamageRequests, delivery, attackCandidate.Outcome.DamageRequest);
            var canonical = CopyAndSort(requests);
            // M2A and M1 are individually preflighted before queue removal. M1 remains
            // the sole canonical ordering, health, dedupe, clamp and death authority.
            var attack = attackSession.CommitPreflighted(attackCandidate);
            var results = combatSession.Process(tick, requests);
            if (delivery.HasValue) _deliveries.Remove(tick.Value);
            var outcome = new OrdanBossCombatOutcome(tick, attack, canonical, results, CaptureSnapshots());
            if (_hasLatestOutcome) { _latestOutcome = outcome; }
            else { _provisionalOutcome = outcome; _hasProvisionalOutcome = true; }
            return outcome;
        }

        private void ValidateProposedCoordinatedReceipt(object owner, SimulationTick tick, bool locked, InputFrameCommitReceipt? receipt)
        {
            if (_coordinatedInputSource == null) { if (receipt.HasValue) throw new InvalidOperationException("Boss Combat is not registered for coordinated input."); return; }
            if (_coordinatedLaneClosed || !receipt.HasValue || !ReferenceEquals(owner, _coordinatedInputSource) || _coordinatedInputSource.IsFaulted || receipt.Value.Tick != tick || receipt.Value.FrameOrdinal != checked(_lastConsumedCoordinatedOrdinal + 1) || (!_hasCoordinatedModeEpoch && receipt.Value.ModeEpoch != 0) || (_hasCoordinatedModeEpoch && (receipt.Value.ModeEpoch < _lastCoordinatedModeEpoch || (receipt.Value.ModeEpoch != _lastCoordinatedModeEpoch && (_lastCoordinatedModeEpoch == long.MaxValue || receipt.Value.ModeEpoch != _lastCoordinatedModeEpoch + 1)))) || locked != (receipt.Value.Mode != InputMode.GameplayEnabled)) throw new InvalidOperationException("Boss Combat coordinated input receipt is invalid.");
        }
        private void ConsumeCoordinatedReceipt(SimulationTick tick)
        {
            if (_coordinatedInputSource == null) return;
            if (_coordinatedLaneClosed || _coordinatedInputSource.IsFaulted || !_pendingCoordinatedReceipt.HasValue || !_coordinatedInputSource.CurrentReceipt.HasValue || _pendingCoordinatedReceipt.Value != _coordinatedInputSource.CurrentReceipt.Value || _pendingCoordinatedReceipt.Value.Tick != tick || _pendingCoordinatedReceipt.Value.FrameOrdinal != checked(_lastConsumedCoordinatedOrdinal + 1) || (!_hasCoordinatedModeEpoch && _pendingCoordinatedReceipt.Value.ModeEpoch != 0) || (_hasCoordinatedModeEpoch && (_pendingCoordinatedReceipt.Value.ModeEpoch < _lastCoordinatedModeEpoch || (_pendingCoordinatedReceipt.Value.ModeEpoch != _lastCoordinatedModeEpoch && (_lastCoordinatedModeEpoch == long.MaxValue || _pendingCoordinatedReceipt.Value.ModeEpoch != _lastCoordinatedModeEpoch + 1)))) || _inputGateLocked != (_pendingCoordinatedReceipt.Value.Mode != InputMode.GameplayEnabled)) throw new InvalidOperationException("Boss Combat coordinated input proof is missing or stale.");
            _lastConsumedCoordinatedOrdinal = _pendingCoordinatedReceipt.Value.FrameOrdinal; _lastCoordinatedModeEpoch = _pendingCoordinatedReceipt.Value.ModeEpoch; _hasCoordinatedModeEpoch = true; _pendingCoordinatedReceipt = null;
        }

        private void ValidateGraph()
        {
            if (_player == null || _registry == null || _transferDriver == null || _bossCombatOwners == null || _bossCombatOwners.Count != 1 || _bossCombatOwners[0] != this)
                throw new InvalidOperationException("Boss Combat requires one exact boss owner, player, registry, and Transfer bindings.");
        }

        private static void ValidateBossRoster(IReadOnlyList<FrozenCombatTarget> roster)
        {
            if (roster == null || roster.Count != 2 || roster[0].TargetId != "ordan" || roster[1].TargetId != "player" ||
                roster[0].MaxHealth != 60 || roster[0].InvulnerabilityTicks != 0 || !roster[0].CanReceivePlayerBasicAttack || roster[0].HasMatchingTransferTarget ||
                roster[1].MaxHealth != 5 || roster[1].InvulnerabilityTicks != 45 || roster[1].CanReceivePlayerBasicAttack)
                throw new InvalidOperationException("Boss Combat roster must be exact ordinal ordan/player authored values.");
        }

        private OrdanBossDeliveryBatch? PreflightIncomingDelivery(SimulationTick tick)
        {
            if (tick == _firstSourceTick)
            {
                if (_deliveries.Count != 0) throw new InvalidOperationException("Boss bootstrap tick cannot receive a delivery.");
                return null;
            }
            if (_deliveries.Count != 1 || !_deliveries.TryGetValue(tick.Value, out var batch) || batch.SourceTick.Value != checked(tick.Value - 1) || batch.DeliveryTick != tick)
                throw new InvalidOperationException("Every post-bootstrap Boss Combat tick requires one exact delivery batch.");
            if (batch.Request.HasValue) ValidatePayloadRequest(batch.SourceTick, batch.DeliveryTick, batch.Request.Value); CopyHostile(batch.SourceTick, batch.DeliveryTick, batch.HostileRequests);
            return batch;
        }

        private void PreflightInputAndReceipt(SimulationTick tick, OrdanBossCombatInput input)
        {
            if (input.Tick != tick) throw new InvalidOperationException("Boss Combat input tick is not exact.");
            if (!input.Aim.HasValue) return;
            var receipt = _transferDriver.LatestCaptureReceipt;
            if (!receipt.HasValue || receipt.Value.SimulationTick != tick || receipt.Value.AimSampleId != input.Aim.Value.SampleId ||
                receipt.Value.AimSampleTick != input.Aim.Value.SampleTick || receipt.Value.CameraPoseTick != input.Camera.Value.CameraPoseTick)
                throw new InvalidOperationException("Boss Combat requires the exact Transfer capture receipt.");
        }

        private Dictionary<string, bool> CaptureAttackability()
        {
            var values = new Dictionary<string, bool>(StringComparer.Ordinal);
            for (var index = 0; index < _frozenTargets.Count; index++)
            {
                var target = _frozenTargets[index];
                values.Add(target.TargetId, target.TargetId == "ordan" && !ActiveCombatSession.Snapshot(target.TargetId).IsDead);
            }
            return values;
        }

        private OrdanBossCombatInput ConsumeOrEmpty(SimulationTick tick)
        {
            if (_inputs.TryGetValue(tick.Value, out var input)) { var nextGeneration = checked(_inputGeneration + 1); _inputs.Remove(tick.Value); _inputGeneration = nextGeneration; return input; }
            return new OrdanBossCombatInput(tick, null, null, null, Array.Empty<DamageRequest>());
        }

        private static OrdanBossCombatInput NormalizeInput(OrdanBossCombatInput input, bool stripInteractive)
        {
            var validated = new OrdanBossCombatInput(
                input.Tick,
                input.Camera,
                input.Aim,
                input.Press,
                input.ExternalDamageRequests);
            return stripInteractive ? StripInteractiveInput(validated) : validated;
        }

        private static OrdanBossCombatInput StripInteractiveInput(OrdanBossCombatInput input)
        {
            return new OrdanBossCombatInput(
                input.Tick,
                null,
                null,
                null,
                input.ExternalDamageRequests);
        }

        private static bool HasInteractiveInput(OrdanBossCombatInput input)
        {
            return input.Camera.HasValue || input.Aim.HasValue || input.Press.HasValue;
        }

        private static DamageRequest[] MergeRequests(IReadOnlyList<DamageRequest> external, OrdanBossDeliveryBatch? delivery, DamageRequest? basic)
        {
            var count = checked(external.Count + (delivery.HasValue && delivery.Value.Request.HasValue ? 1 : 0) + (delivery.HasValue ? delivery.Value.HostileRequests.Count : 0) + (basic.HasValue ? 1 : 0));
            var values = new DamageRequest[count]; var cursor = 0;
            for (var index = 0; index < external.Count; index++) values[cursor++] = external[index];
            if (delivery.HasValue && delivery.Value.Request.HasValue) values[cursor++] = delivery.Value.Request.Value;
            if (delivery.HasValue) for (var i=0;i<delivery.Value.HostileRequests.Count;i++) values[cursor++] = delivery.Value.HostileRequests[i];
            if (basic.HasValue) values[cursor] = basic.Value;
            return values;
        }

        private CombatantSnapshot[] CaptureSnapshots()
        {
            var values = new CombatantSnapshot[_frozenTargets.Count];
            for (var index = 0; index < values.Length; index++) values[index] = ActiveCombatSession.Snapshot(_frozenTargets[index].TargetId);
            return values;
        }

        private static DamageRequest[] CopyAndSort(IReadOnlyList<DamageRequest> requests)
        {
            var copy = new DamageRequest[requests.Count]; for (var index = 0; index < copy.Length; index++) copy[index] = requests[index];
            Array.Sort(copy, CompareRequests); return copy;
        }
        private static int CompareRequests(DamageRequest left, DamageRequest right)
        {
            var value = string.CompareOrdinal(left.RequestId, right.RequestId); if (value != 0) return value;
            value = string.CompareOrdinal(left.SourceId, right.SourceId); if (value != 0) return value;
            value = string.CompareOrdinal(left.TargetId, right.TargetId); if (value != 0) return value;
            value = left.Kind.CompareTo(right.Kind); return value != 0 ? value : left.Amount.CompareTo(right.Amount);
        }
        private static void ValidatePayloadRequest(SimulationTick sourceTick, SimulationTick deliveryTick, DamageRequest request)
        {
            if (request.Tick != deliveryTick || request.TargetId != "ordan" || request.Amount != 8 || request.Kind != DamageKind.BossPayloadTransfer ||
                request.SourceId != "BossWeight-0" && request.SourceId != "BossWeight-1" && request.SourceId != "BossWeight-2" || string.IsNullOrEmpty(request.RequestId))
                throw new ArgumentException("Boss delivery must be M4A's exact payload damage request.", nameof(request));
            const string prefix = "ordan.payload/";
            var suffix = "/" + deliveryTick.Value.ToString("D10", CultureInfo.InvariantCulture);
            if (!request.RequestId.StartsWith(prefix, StringComparison.Ordinal) || !request.RequestId.EndsWith(suffix, StringComparison.Ordinal) ||
                request.RequestId.Length != prefix.Length + 19 + suffix.Length)
                throw new ArgumentException("Boss payload request ID is not M4A's exact ordinal format.", nameof(request));
            long ordinal = 0;
            try
            {
                for (var index = 0; index < 19; index++)
                {
                    var value = request.RequestId[prefix.Length + index];
                    if (value < '0' || value > '9') throw new ArgumentException("Boss payload ordinal must be ASCII digits.", nameof(request));
                    ordinal = checked(ordinal * 10 + value - '0');
                }
            }
            catch (OverflowException) { throw new ArgumentException("Boss payload ordinal overflows Int64.", nameof(request)); }
            if (ordinal % 3 != request.SourceId[request.SourceId.Length - 1] - '0') throw new ArgumentException("Boss payload source handle does not match its ordinal.", nameof(request));
            if (sourceTick.Value != checked(deliveryTick.Value - 1)) throw new ArgumentException("Boss payload horizon is not one tick.", nameof(request));
        }

        private CombatSession ActiveCombatSession => _hasLatestOutcome ? _combatSession : _provisionalCombatSession;
        private BasicAttackSession ActiveAttackSession => _hasLatestOutcome ? _attackSession : _provisionalAttackSession;
    }

    // REQ-COM-004. Exact-tick input is immutable and never retains a caller collection.
    internal readonly struct OrdanBossCombatInput
    {
        internal OrdanBossCombatInput(SimulationTick tick, SimulationCameraPoseSnapshot? camera, AimSample? aim, BasicAttackPressed? press, IReadOnlyList<DamageRequest> externalDamageRequests)
        {
            if (externalDamageRequests == null || aim.HasValue != camera.HasValue || (aim.HasValue && (aim.Value.SampleTick != tick || aim.Value.CameraPoseTick != camera.Value.CameraPoseTick)) ||
                (press.HasValue && (!aim.HasValue || press.Value.Tick != tick))) throw new ArgumentException("Boss Combat aim/camera/press fields must be exact and coupled.");
            var copy = new DamageRequest[externalDamageRequests.Count];
            for (var index = 0; index < copy.Length; index++) { if (externalDamageRequests[index].Tick != tick) throw new ArgumentException("External request tick is not exact."); copy[index] = externalDamageRequests[index]; }
            Tick = tick; Camera = camera; Aim = aim; Press = press; ExternalDamageRequests = Array.AsReadOnly(copy);
        }
        internal SimulationTick Tick { get; } internal SimulationCameraPoseSnapshot? Camera { get; } internal AimSample? Aim { get; }
        internal BasicAttackPressed? Press { get; } internal IReadOnlyList<DamageRequest> ExternalDamageRequests { get; }
    }

    // REQ-COM-004. Private-owner candidate prevents a forged M4A queue write.
    internal sealed class OrdanBossDeliveryCandidate
    {
        private readonly object _owner;
        internal OrdanBossDeliveryCandidate(object owner, SimulationTick sourceTick, SimulationTick deliveryTick, DamageRequest? request) { _owner = owner; SourceTick = sourceTick; DeliveryTick = deliveryTick; Request = request; }
        internal SimulationTick SourceTick { get; } internal SimulationTick DeliveryTick { get; } internal DamageRequest? Request { get; }
        internal bool IsOwnedBy(object owner) => ReferenceEquals(_owner, owner);
    }
    internal sealed class OrdanBossHostileDeliveryCandidate
    { private readonly object _owner; internal OrdanBossHostileDeliveryCandidate(object owner, OrdanBossDeliveryBatch batch, DamageRequest[] hostile) { _owner=owner; Batch=batch; HostileRequests=Array.AsReadOnly(hostile); } internal OrdanBossDeliveryBatch Batch { get; } internal IReadOnlyList<DamageRequest> HostileRequests { get; } internal bool IsOwnedBy(object owner) => ReferenceEquals(_owner,owner); }
    internal readonly struct OrdanBossDeliveryBatch : IEquatable<OrdanBossDeliveryBatch>
    {
        internal OrdanBossDeliveryBatch(SimulationTick sourceTick, SimulationTick deliveryTick, DamageRequest? request) : this(sourceTick, deliveryTick, request, Array.Empty<DamageRequest>(), false) { }
        internal OrdanBossDeliveryBatch(SimulationTick sourceTick, SimulationTick deliveryTick, DamageRequest? request, IReadOnlyList<DamageRequest> hostile) : this(sourceTick, deliveryTick, request, hostile, hostile != null && hostile.Count != 0) { }
        internal OrdanBossDeliveryBatch(SimulationTick sourceTick, SimulationTick deliveryTick, DamageRequest? request, IReadOnlyList<DamageRequest> hostile, bool hostileAppendCommitted) { if(hostile==null||hostile.Count>1||(!hostileAppendCommitted&&hostile.Count!=0)) throw new ArgumentException(); SourceTick=sourceTick; DeliveryTick=deliveryTick; Request=request; HostileAppendCommitted=hostileAppendCommitted; var copy=new DamageRequest[hostile.Count]; for(var i=0;i<copy.Length;i++)copy[i]=hostile[i]; HostileRequests=Array.AsReadOnly(copy); }
        internal SimulationTick SourceTick { get; } internal SimulationTick DeliveryTick { get; } internal DamageRequest? Request { get; } internal IReadOnlyList<DamageRequest> HostileRequests { get; } internal bool HostileAppendCommitted { get; }
        public bool Equals(OrdanBossDeliveryBatch other) => SourceTick==other.SourceTick && DeliveryTick==other.DeliveryTick && Nullable.Equals(Request,other.Request) && HostileAppendCommitted==other.HostileAppendCommitted &&
            HostileRequests != null && other.HostileRequests != null && HostileRequests.Count==other.HostileRequests.Count &&
            (HostileRequests.Count==0 || SameRequest(HostileRequests[0], other.HostileRequests[0]));
        private static bool SameRequest(DamageRequest left, DamageRequest right) => left.RequestId == right.RequestId && left.SourceId == right.SourceId &&
            left.TargetId == right.TargetId && left.Amount == right.Amount && left.Kind == right.Kind && left.Tick == right.Tick;
    }
    internal sealed class OrdanBossTerminalDeliveryDiscardCandidate
    {
        private readonly object _issuer; private readonly object _owner;
        internal OrdanBossTerminalDeliveryDiscardCandidate(object issuer, object owner, OrdanBossDeliveryBatch batch, bool hadEmptyInput, InputFrameCommitReceipt? coordinatedReceipt = null) { _issuer = issuer; _owner = owner; Batch = batch; HadEmptyInput = hadEmptyInput; CoordinatedReceipt = coordinatedReceipt; }
        internal OrdanBossDeliveryBatch Batch { get; }
        internal bool HadEmptyInput { get; }
        internal InputFrameCommitReceipt? CoordinatedReceipt { get; }
        internal bool IsIssuedBy(object issuer) => ReferenceEquals(_issuer, issuer);
        internal bool IsOwnedBy(object owner) => ReferenceEquals(_owner, owner);
    }

    // REQ-COM-004. Immutable M1 publication consumed by the -185 bridge.
    internal readonly struct OrdanBossCombatOutcome
    {
        internal OrdanBossCombatOutcome(SimulationTick tick, BasicAttackTickOutcome attack, IReadOnlyList<DamageRequest> trace, IReadOnlyList<DamageResult> results, IReadOnlyList<CombatantSnapshot> combatants)
        {
            if (trace == null || results == null || combatants == null || trace.Count != results.Count) throw new ArgumentException();
            Tick = tick; Attack = attack; CanonicalRequestTrace = Copy(trace); DamageResults = Copy(results); Combatants = Copy(combatants);
            for (var index = 0; index < trace.Count; index++) if (CanonicalRequestTrace[index].RequestId != DamageResults[index].RequestId || CanonicalRequestTrace[index].TargetId != DamageResults[index].TargetId || CanonicalRequestTrace[index].Tick != tick || DamageResults[index].Tick != tick) throw new ArgumentException("Combat trace/result alignment is exact.");
        }
        internal SimulationTick Tick { get; } internal BasicAttackTickOutcome Attack { get; } internal IReadOnlyList<DamageRequest> CanonicalRequestTrace { get; }
        internal IReadOnlyList<DamageResult> DamageResults { get; } internal IReadOnlyList<CombatantSnapshot> Combatants { get; }
        private static IReadOnlyList<T> Copy<T>(IReadOnlyList<T> values) { var copy = new T[values.Count]; for (var i = 0; i < copy.Length; i++) copy[i] = values[i]; return Array.AsReadOnly(copy); }
    }
}
