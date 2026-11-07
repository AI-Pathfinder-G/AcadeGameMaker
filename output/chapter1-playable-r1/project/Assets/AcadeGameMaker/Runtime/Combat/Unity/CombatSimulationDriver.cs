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
    // REQ-COM-001, REQ-COM-004, REQ-WT-005, REQ-WT-006. The -190 phase is the sole Unity owner of M2A-to-M1 combat publication.
    [DefaultExecutionOrder(-190)]
    [DisallowMultipleComponent]
    internal sealed class CombatSimulationDriver : MonoBehaviour
    {
        private SimulationTick? _chapterCarryTick;
        private int _chapterCarryHealth,_chapterCarryInvulnerability;
        internal SimulationTick? Chapter1ConfiguredCarryTick => _chapterCarryTick;
        // REQ-C1B-HP-001: store live carry before session-producing Awake.
        internal void ConfigureChapter1CarriedPlayerForAuthoring(SimulationTick firstTick,int currentHealth,int invulnerabilityEndTick)
        {
            if(_chapterCarryTick.HasValue || _initialized || _combatSession!=null || _attackSession!=null || _combatPhaseAttempted || _threatDeliveryLaneRegistered || gameObject.activeInHierarchy || firstTick.Value<0 ||
                firstTick.Value>int.MaxValue-600 || currentHealth<1 || currentHealth>5 || invulnerabilityEndTick< -1)
                throw new InvalidOperationException("Carried Combat entry must be pristine, inactive, and live.");
            _chapterCarryTick=firstTick;_chapterCarryHealth=currentHealth;_chapterCarryInvulnerability=invulnerabilityEndTick;
        }
        // REQ/AC-AUTOCH1-CARRYBASELINE-001: read only the original inactive entry assignment; never create a session.
        internal int ReadEncounterInitialPlayerHealth(SimulationTick firstTick)
        {
            if(!_chapterCarryTick.HasValue)return 5;
            if(_chapterCarryTick.Value!=firstTick || _chapterCarryHealth<1 || _chapterCarryHealth>5)
                throw new InvalidOperationException("Lifecycle entry must match the original carried Combat baseline.");
            return _chapterCarryHealth;
        }
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private CombatTargetRegistry _registry;
        [SerializeField] private TransferSimulationDriver _transferDriver;
        [SerializeField] private RegularEnemyReactionSimulationDriver _reactionDriver;

        private SortedDictionary<int, CombatSimulationInput> _inputs = new SortedDictionary<int, CombatSimulationInput>();
        private readonly SortedDictionary<int, RegularEnemyThreatDeliveryBatch> _threatDeliveries = new SortedDictionary<int, RegularEnemyThreatDeliveryBatch>();
        private readonly object _threatDeliveryOwner = new object();
        private readonly HashSet<string> _scheduledTransferRemovalIds = new HashSet<string>(StringComparer.Ordinal);
        private IReadOnlyList<FrozenCombatTarget> _frozenTargets;
        private CombatSession _combatSession;
        private BasicAttackSession _attackSession;
        private int _maxRegisteredInvulnerabilityTicks;
        private bool _initialized;
        private bool _hasLatestOutcome;
        private CombatSimulationOutcome _latestOutcome;
        private bool _threatRegistrationPrepared;
        private bool _threatDeliveryLaneRegistered;
        private bool _combatPhaseAttempted;
        private SimulationTick _firstThreatSourceTick;
        private SimulationTick _firstThreatDeliveryTick;

        internal CombatSimulationOutcome LatestOutcomeForTests => _latestOutcome;
        // REQ-COM-004, AC-COM-003: default tick 0 is never evidence of an M1 publication.
        internal bool HasLatestOutcomeForBehaviorBridge => _hasLatestOutcome;
        internal CombatSimulationOutcome LatestOutcomeForBehaviorBridge => _hasLatestOutcome
            ? _latestOutcome
            : throw new InvalidOperationException("Behavior bridge requires a successful combat publication.");
        internal IReadOnlyList<FrozenCombatTarget> FrozenTargetsForBehaviorBridge => _initialized
            ? _frozenTargets
            : throw new InvalidOperationException("Behavior bridge requires the combat-owned frozen roster.");
        // REQ-COM-004, REQ-WT-007: M3C2 aliases the same frozen Combat-owned roster; it never owns a second registry.
        internal IReadOnlyList<FrozenCombatTarget> FrozenTargetsForLocomotionBridge => _initialized
            ? _frozenTargets
            : throw new InvalidOperationException("Locomotion bridge requires the combat-owned frozen roster.");
        // REQ-COM-002, REQ-COM-004: M3D aliases Combat's exact frozen roster and successful publication.
        internal IReadOnlyList<FrozenCombatTarget> FrozenTargetsForThreatBridge => _initialized
            ? _frozenTargets
            : throw new InvalidOperationException("Threat bridge requires the combat-owned frozen roster.");
        internal bool HasLatestOutcomeForThreatBridge => _hasLatestOutcome;
        internal CombatSimulationOutcome LatestOutcomeForThreatBridge => _hasLatestOutcome
            ? _latestOutcome
            : throw new InvalidOperationException("Threat bridge requires a successful combat publication.");
        // REQ-COM-001, REQ-COM-004. Read-only +110 M3E1 handoff seam; it does
        // not expose queues or permit a second Combat consumption.
        internal bool HasLatestOutcomeForEncounterHandoff => _hasLatestOutcome;
        internal CombatSimulationOutcome LatestOutcomeForEncounterHandoff => _hasLatestOutcome
            ? _latestOutcome
            : throw new InvalidOperationException("Encounter handoff requires a successful Combat publication.");
        internal IReadOnlyList<FrozenCombatTarget> FrozenTargetsForEncounterHandoff => _initialized
            ? _frozenTargets
            : throw new InvalidOperationException("Encounter handoff requires the frozen Combat roster.");
        internal SimulationTick NextCombatTickForTests => _combatSession == null ? default(SimulationTick) : _combatSession.NextExpectedTick;
        internal int ThreatDeliveryCountForTests => _threatDeliveries.Count;
        internal bool ThreatDeliveryLaneRegisteredForTests => _threatDeliveryLaneRegistered;

        private object _coordinatedOwner,_coordinatedToken;
        private IInputFrameCommitSource _coordinatedSource;
        private string _coordinatedRun;
        private long _coordinatedGraph,_inputGeneration;
        private bool _inputLocked;
        private InputFrameCommitReceipt? _pendingInputReceipt;
        internal PlayerMovementController BoundPlayer => _player;
        internal TransferSimulationDriver BoundTransferDriver => _transferDriver;
        internal bool IsInputGateLocked => _inputLocked;
        // REQ-C1B-INPUT-001: actual router identity and its one registration token.
        internal void ValidateCoordinatedInputRegistration(object owner,IInputFrameCommitSource source,object token,string runKey,long graph,SimulationTick firstTick)
        {
            if(owner==null || source==null || source.IsFaulted || !ReferenceEquals(owner,source) || token==null || string.IsNullOrEmpty(runKey) || graph<1 ||
                !_initialized || !_threatDeliveryLaneRegistered || _player==null || _combatSession.NextExpectedTick!=firstTick || _attackSession.NextExpectedTick!=firstTick ||
                _player.NextExpectedTick!=firstTick || _inputs.Count!=0 || _pendingInputReceipt.HasValue ||
                (_coordinatedOwner!=null && (!ReferenceEquals(owner,_coordinatedOwner) || !ReferenceEquals(token,_coordinatedToken) || runKey!=_coordinatedRun || graph!=_coordinatedGraph)))
                throw new InvalidOperationException("Regular coordinated registration is not pristine or exact.");
        }
        internal void RegisterCoordinatedInputOwner(object owner,IInputFrameCommitSource source,object token,string runKey,long graph,SimulationTick firstTick)
        {
            ValidateCoordinatedInputRegistration(owner,source,token,runKey,graph,firstTick);
            _coordinatedOwner=owner;_coordinatedSource=source;_coordinatedToken=token;_coordinatedRun=runKey;_coordinatedGraph=graph;
        }
        internal sealed class InputFrameCandidate
        {
            private readonly CombatSimulationDriver _driver;
            private readonly object _owner,_token;
            private readonly string _run;
            private readonly long _graph,_generation;
            private readonly InputFrameCommitReceipt _receipt;
            private readonly SortedDictionary<int,CombatSimulationInput> _inputs;
            private readonly bool _locked;
            private bool _committed;
            internal InputFrameCandidate(CombatSimulationDriver driver,object owner,object token,string run,long graph,long generation,
                InputFrameCommitReceipt receipt,SortedDictionary<int,CombatSimulationInput> inputs,bool locked)
            { _driver=driver;_owner=owner;_token=token;_run=run;_graph=graph;_generation=generation;_receipt=receipt;_inputs=inputs;_locked=locked; }
            internal void Validate(CombatSimulationDriver driver,object owner,object token)
            {
                if(_committed || !ReferenceEquals(_driver,driver) || !ReferenceEquals(_owner,owner) || !ReferenceEquals(_token,token) ||
                    !ReferenceEquals(owner,driver._coordinatedOwner) || !ReferenceEquals(token,driver._coordinatedToken) ||
                    _run!=driver._coordinatedRun || _graph!=driver._coordinatedGraph || _generation!=driver._inputGeneration)
                    throw new InvalidOperationException("Regular candidate is foreign, stale, or consumed.");
                driver.ValidateProposedReceipt(owner,token,_receipt,_locked);
            }
            internal void Apply(CombatSimulationDriver driver,object owner,object token)
            {
                Validate(driver,owner,token);var next=checked(driver._inputGeneration+1);
                driver._inputs=_inputs;driver._pendingInputReceipt=_receipt;driver._inputLocked=_locked;driver._inputGeneration=next;_committed=true;
            }
        }
        private void ValidateProposedReceipt(object owner,object token,InputFrameCommitReceipt receipt,bool locked)
        {
            if(!ReferenceEquals(owner,_coordinatedOwner) || !ReferenceEquals(token,_coordinatedToken) || _coordinatedSource==null || _coordinatedSource.IsFaulted ||
                _player==null || receipt.Tick!=_player.NextExpectedTick || receipt.Tick!=_combatSession.NextExpectedTick || receipt.Tick!=_attackSession.NextExpectedTick ||
                locked!=(receipt.Mode!=InputMode.GameplayEnabled) || _pendingInputReceipt.HasValue)
                throw new InvalidOperationException("Regular pending receipt is not exact.");
            var previous=_coordinatedSource.CurrentReceipt;
            if(previous.HasValue && (receipt.FrameOrdinal!=checked(previous.Value.FrameOrdinal+1) || receipt.Tick.Value!=checked(previous.Value.Tick.Value+1) ||
                receipt.ModeEpoch!=previous.Value.ModeEpoch+(receipt.Mode==previous.Value.Mode?0:1)))
                throw new InvalidOperationException("Regular global receipt successor mismatch.");
            if(!previous.HasValue && (receipt.FrameOrdinal!=0 || receipt.ModeEpoch!=0))throw new InvalidOperationException("Regular receipt bootstrap mismatch.");
        }
        internal InputFrameCandidate PrepareInputFrame(object owner,object token,SimulationTick tick,bool locked,CombatSimulationInput? input,InputFrameCommitReceipt receipt)
        {
            ValidateProposedReceipt(owner,token,receipt,locked);
            if(tick!=receipt.Tick || (locked && input.HasValue))throw new InvalidOperationException("Regular candidate input tick/mode mismatch.");
            var staged=new SortedDictionary<int,CombatSimulationInput>(_inputs);
            if(staged.ContainsKey(tick.Value))throw new InvalidOperationException("Regular input slot is already occupied.");
            var value=input ?? new CombatSimulationInput(tick,null,null,null,Array.Empty<DamageRequest>(),false);
            if(value.Tick!=tick || value.EncounterReset || value.ExternalDamageRequests.Count!=0)
                throw new InvalidOperationException("Coordinated regular input cannot inject reset or damage.");
            staged.Add(tick.Value,value);
            return new InputFrameCandidate(this,owner,token,_coordinatedRun,_coordinatedGraph,_inputGeneration,receipt,staged,locked);
        }
        internal void ValidateInputFrameCandidate(object owner,object token,InputFrameCandidate candidate)
        { if(candidate==null)throw new InvalidOperationException("Missing regular candidate.");candidate.Validate(this,owner,token); }
        internal void CommitInputFrame(object owner,object token,InputFrameCandidate candidate)
        { ValidateInputFrameCandidate(owner,token,candidate);candidate.Apply(this,owner,token); }
        private void ValidateCommittedInputPhase(SimulationTick tick)
        {
            if(_coordinatedSource==null)return;
            var global=_coordinatedSource.CurrentReceipt;
            if(_coordinatedSource.IsFaulted || !_pendingInputReceipt.HasValue || !global.HasValue || !_pendingInputReceipt.Value.Equals(global.Value) ||
                global.Value.Tick!=tick || !_inputs.ContainsKey(tick.Value))
                throw new InvalidOperationException("Regular phase has no published global receipt.");
        }

        private void FixedUpdate() => AdvanceCombatPhase();

        // REQ-COM-001, REQ-COM-004. Noninitializing exact authored graph binding.
        internal void ConfigureForAuthoring(PlayerMovementController player, CombatTargetRegistry registry,
            TransferSimulationDriver transferDriver, RegularEnemyReactionSimulationDriver reactionDriver) =>
            Configure(player, registry, transferDriver, reactionDriver);

        internal void ConfigureForTests(PlayerMovementController player, CombatTargetRegistry registry,
            TransferSimulationDriver transferDriver, RegularEnemyReactionSimulationDriver reactionDriver) =>
            Configure(player, registry, transferDriver, reactionDriver);

        private void Configure(PlayerMovementController player, CombatTargetRegistry registry,
            TransferSimulationDriver transferDriver, RegularEnemyReactionSimulationDriver reactionDriver)
        {
            if (_initialized) throw new InvalidOperationException("CombatSimulationDriver cannot be reconfigured after initialization.");
            _player = player;
            _registry = registry;
            _transferDriver = transferDriver;
            _reactionDriver = reactionDriver;
        }

        internal void InitializeForTests() => EnsureInitialized();

        // REQ-COM-001, REQ-COM-004: preparation may initialize Combat but leaves the M3D lane inactive and empty.
        internal IReadOnlyList<FrozenCombatTarget> PrepareThreatDeliveryRegistration()
        {
            if (_threatDeliveryLaneRegistered || _combatPhaseAttempted || _hasLatestOutcome || _threatDeliveries.Count != 0)
                throw new InvalidOperationException("Threat delivery preparation requires pristine inactive Combat state.");
            EnsureInitialized();
            _threatRegistrationPrepared = true;
            return _frozenTargets;
        }

        // REQ-COM-001, REQ-COM-004: opt-in registration preserves legacy isolated M2B behavior.
        internal void RegisterThreatDeliveryLane(SimulationTick firstSourceTick)
        {
            if (!_threatRegistrationPrepared || !_initialized || _threatDeliveryLaneRegistered || _combatPhaseAttempted || _hasLatestOutcome || _threatDeliveries.Count != 0)
                throw new InvalidOperationException("Threat delivery lane registration must be pristine and unique.");
            var horizon = Math.Max(21, _maxRegisteredInvulnerabilityTicks);
            if (firstSourceTick.Value < 0 || firstSourceTick.Value > int.MaxValue - horizon - 1 ||
                firstSourceTick != _combatSession.NextExpectedTick || firstSourceTick != _attackSession.NextExpectedTick ||
                firstSourceTick != _player.NextExpectedTick)
                throw new InvalidOperationException("Threat delivery lane requires exact aligned, horizon-safe first source tick.");
            _firstThreatSourceTick = firstSourceTick;
            _firstThreatDeliveryTick = new SimulationTick(checked(firstSourceTick.Value + 1));
            _threatDeliveryLaneRegistered = true;
        }

        internal void ValidateThreatGraph(PlayerMovementController player, RegularEnemyReactionSimulationDriver reactionDriver)
        {
            if (_player != player || _reactionDriver != reactionDriver)
                throw new InvalidOperationException("Combat threat graph references are cross-wired.");
        }

        // REQ-COM-001, REQ-COM-004, REQ-WT-005. B0 authoring compares the exact
        // explicit graph; it neither initializes nor exposes the mutable registry.
        internal void ValidateEncounterGraph(PlayerMovementController player, CombatTargetRegistry registry,
            TransferSimulationDriver transferDriver, RegularEnemyReactionSimulationDriver reactionDriver)
        {
            if (_player != player || _registry != registry || _transferDriver != transferDriver ||
                _reactionDriver != reactionDriver)
                throw new InvalidOperationException("Combat encounter graph references are cross-wired.");
        }

        internal RegularEnemyThreatDeliveryCandidate PreflightThreatDelivery(SimulationTick sourceTick,
            IReadOnlyList<DamageRequest> requests)
        {
            if (!_threatDeliveryLaneRegistered || !_hasLatestOutcome || sourceTick != _latestOutcome.Tick ||
                sourceTick.Value != checked(_combatSession.NextExpectedTick.Value - 1))
                throw new InvalidOperationException("Threat delivery source must be the exact completed Combat tick.");
            var deliveryTick = new SimulationTick(checked(sourceTick.Value + 1));
            if (deliveryTick.Value > int.MaxValue - 1) throw new OverflowException("Threat delivery tick is exhausted.");
            PreflightHorizon(deliveryTick, _maxRegisteredInvulnerabilityTicks);
            if (_threatDeliveries.ContainsKey(deliveryTick.Value))
                throw new InvalidOperationException("A threat batch already exists for the delivery tick.");
            ValidateThreatRequests(sourceTick, deliveryTick, requests);
            return new RegularEnemyThreatDeliveryCandidate(_threatDeliveryOwner, sourceTick, deliveryTick, requests);
        }

        internal void CommitPreflightedThreatDelivery(RegularEnemyThreatDeliveryCandidate candidate)
        {
            if (candidate == null || !candidate.IsOwnedBy(_threatDeliveryOwner))
                throw new InvalidOperationException("Threat delivery candidate was not issued by this Combat driver.");
            if (!_threatDeliveryLaneRegistered || !_hasLatestOutcome || candidate.SourceTick != _latestOutcome.Tick ||
                candidate.SourceTick.Value != checked(_combatSession.NextExpectedTick.Value - 1) ||
                candidate.DeliveryTick.Value != checked(candidate.SourceTick.Value + 1) ||
                candidate.DeliveryTick != _combatSession.NextExpectedTick || _threatDeliveries.ContainsKey(candidate.DeliveryTick.Value))
                throw new InvalidOperationException("Threat delivery candidate became stale or conflicts with the queue.");
            var requests = candidate.CopyRequests();
            ValidateThreatRequests(candidate.SourceTick, candidate.DeliveryTick, requests);
            _threatDeliveries.Add(candidate.DeliveryTick.Value,
                new RegularEnemyThreatDeliveryBatch(candidate.SourceTick, candidate.DeliveryTick, requests));
        }

        // REQ-COM-001, REQ-COM-004. Inputs are exact future ticks and are consumed at most once.
        internal void Submit(CombatSimulationInput input)
        {
            EnsureInitialized();
            if(_coordinatedSource!=null)throw new InvalidOperationException("Registered regular Combat forbids Submit bypass.");
            var expected = _player.NextExpectedTick.Value;
            if (input.Tick.Value < expected || _inputs.ContainsKey(input.Tick.Value))
            {
                throw new InvalidOperationException("Combat inputs are exact, future, and unique by SimulationTick.");
            }

            _inputs.Add(input.Tick.Value, input);
        }

        internal void SubmitForTests(CombatSimulationInput input) => Submit(input);
        internal CombatSimulationOutcome AdvanceCombatOnlyForTests() => AdvanceCombatPhase();
        // Test-only helper preserves the complete fixed pipeline for legacy M2B
        // scripts; production still runs the reaction component at -180.
        internal CombatSimulationOutcome AdvanceCombatPhaseForTests()
        {
            var outcome = AdvanceCombatOnlyForTests();
            _reactionDriver.AdvanceReactionPhaseForTests();
            return outcome;
        }

        private CombatSimulationOutcome AdvanceCombatPhase()
        {
            if (_player == null) throw new InvalidOperationException("CombatSimulationDriver requires an explicit PlayerMovementController.");
            var tick = _player.NextExpectedTick;
            EnsureInitialized();ValidateCommittedInputPhase(tick);
            _combatPhaseAttempted = true;
            var input = ConsumeOrEmpty(tick);

            var threatBatch = PreflightThreatBatchForCombat(tick);
            // All validation that can reject in-contract session work happens before either deterministic session publishes this tick.
            Preflight(tick, input);
            var aliveBeforeTick = CaptureAliveSnapshot();
            var effectiveAttackability = _reactionDriver.BuildEffectiveAttackability(tick, _frozenTargets, aliveBeforeTick);
            IReadOnlyList<BasicAttackCandidateObservation> observations = Array.Empty<BasicAttackCandidateObservation>();
            if (input.Aim.HasValue)
            {
                observations = new CombatObservationBuilder().Capture(
                    tick,
                    _player.Snapshot,
                    input.Camera.Value,
                    input.Aim.Value,
                    _frozenTargets,
                    effectiveAttackability,
                    _player.transform);
            }
            var stagedAttackSession = input.EncounterReset ? new BasicAttackSession(tick) : _attackSession;
            var attackCandidate = stagedAttackSession.PreviewNext(tick, input.Aim, observations, input.Press);

            // AC-COM-003: the distinct threat queue is consumed only after every rejection-capable preflight.
            if (threatBatch.HasValue) _threatDeliveries.Remove(tick.Value);
            var attackOutcome = stagedAttackSession.CommitPreflighted(attackCandidate);
            if (input.EncounterReset)
            {
                _combatSession.ResetEncounter();
                RegisterFrozenCombatants();
                _attackSession = stagedAttackSession;
                _scheduledTransferRemovalIds.Clear();
            }

            var deliveredThreats = input.EncounterReset || !threatBatch.HasValue
                ? Array.Empty<DamageRequest>()
                : threatBatch.Value.CopyRequests();
            var batch = new DamageRequest[input.ExternalDamageRequests.Count + deliveredThreats.Length + (attackOutcome.DamageRequest.HasValue ? 1 : 0)];
            for (var index = 0; index < input.ExternalDamageRequests.Count; index++) batch[index] = input.ExternalDamageRequests[index];
            for (var index = 0; index < deliveredThreats.Length; index++) batch[input.ExternalDamageRequests.Count + index] = deliveredThreats[index];
            if (attackOutcome.DamageRequest.HasValue) batch[batch.Length - 1] = attackOutcome.DamageRequest.Value;

            var canonicalTrace = CopyAndSortCanonicalTrace(batch);
            // M1 alone sorts, deduplicates, mutates health, and establishes death.
            var damageResults = _combatSession.Process(tick, batch);
            var snapshots = CaptureCombatSnapshots();
            ScheduleNextTickTransferRemovals(tick, damageResults, snapshots);
            _latestOutcome = new CombatSimulationOutcome(tick, input.EncounterReset, attackOutcome, canonicalTrace, damageResults, snapshots);
            _hasLatestOutcome = true;
            if(_coordinatedSource!=null)_pendingInputReceipt=null;
            return _latestOutcome;
        }

        private void EnsureInitialized()
        {
            if (_initialized) return;
            if (_player == null || _registry == null || _transferDriver == null || _reactionDriver == null) throw new InvalidOperationException("CombatSimulationDriver requires explicit player, combat registry, transfer-driver, and reaction-driver bindings.");
            _registry.ValidateAndFreeze();
            _frozenTargets = _registry.FrozenTargets;
            var firstTick = _player.NextExpectedTick;
            _combatSession = new CombatSession(firstTick);
            _attackSession = new BasicAttackSession(firstTick);
            RegisterFrozenCombatants();
            for (var index = 0; index < _frozenTargets.Count; index++)
            {
                _maxRegisteredInvulnerabilityTicks = Math.Max(_maxRegisteredInvulnerabilityTicks, _frozenTargets[index].InvulnerabilityTicks);
            }

            _initialized = true;
        }

        private void RegisterFrozenCombatants()
        {
            if(_chapterCarryTick.HasValue && _combatSession.NextExpectedTick!=_chapterCarryTick.Value)throw new InvalidOperationException("Carried regular tick mismatch.");
            for(var index=0;index<_frozenTargets.Count;index++)
            {
                var registration=_frozenTargets[index].CombatantRegistration;
                if(_chapterCarryTick.HasValue && registration.TargetId=="player")_combatSession.RegisterCarriedPlayer(registration,_chapterCarryHealth,_chapterCarryInvulnerability);
                else _combatSession.Register(registration);
            }
        }

        private CombatSimulationInput ConsumeOrEmpty(SimulationTick tick)
        {
            if (_inputs.TryGetValue(tick.Value, out var input))
            {
                // A malformed input is fail-stop after consumption and is intentionally not retryable.
                _inputs.Remove(tick.Value);
                return input;
            }

            return new CombatSimulationInput(tick, null, null, null, Array.Empty<DamageRequest>(), false);
        }

        private RegularEnemyThreatDeliveryBatch? PreflightThreatBatchForCombat(SimulationTick tick)
        {
            if (!_threatDeliveryLaneRegistered) return null;
            if (tick == _firstThreatSourceTick)
            {
                if (_threatDeliveries.Count != 0) throw new InvalidOperationException("Threat bootstrap tick cannot have a delivery batch.");
                return null;
            }
            if (tick.Value < _firstThreatDeliveryTick.Value || _threatDeliveries.Count != 1 ||
                !_threatDeliveries.TryGetValue(tick.Value, out var batch) || batch.DeliveryTick != tick ||
                batch.SourceTick.Value != checked(tick.Value - 1))
                throw new InvalidOperationException("Combat requires exactly one current threat delivery batch.");
            ValidateThreatRequests(batch.SourceTick, batch.DeliveryTick, batch.CopyRequests());
            return batch;
        }

        private static void ValidateThreatRequests(SimulationTick sourceTick, SimulationTick deliveryTick,
            IReadOnlyList<DamageRequest> requests)
        {
            if (requests == null) throw new ArgumentNullException(nameof(requests));
            if (requests.Count < 0 || requests.Count > 2) throw new ArgumentOutOfRangeException(nameof(requests));
            var sawWalker = false;
            var sawLine = false;
            for (var index = 0; index < requests.Count; index++)
            {
                var request = requests[index];
                if (request.Tick != deliveryTick || request.Amount != 1 || !string.Equals(request.TargetId, "player", StringComparison.Ordinal))
                    throw new ArgumentException("Threat requests require exact target, amount, and delivery tick.", nameof(requests));
                if (request.Kind == DamageKind.EnemyContact)
                {
                    if (sawWalker || sawLine || !string.Equals(request.SourceId, "walker", StringComparison.Ordinal) ||
                        !string.Equals(request.RequestId, "enemy.contact/walker/" + sourceTick.Value.ToString("D10", CultureInfo.InvariantCulture), StringComparison.Ordinal))
                        throw new ArgumentException("Walker contact request is malformed or out of order.", nameof(requests));
                    sawWalker = true;
                }
                else if (request.Kind == DamageKind.EnemyRanged)
                {
                    const string prefix = "enemy.line/surveyor/";
                    var suffix = "/" + sourceTick.Value.ToString("D10", CultureInfo.InvariantCulture);
                    if (sawLine || !string.Equals(request.SourceId, "surveyor", StringComparison.Ordinal) || request.RequestId == null ||
                        request.RequestId.Length != prefix.Length + 19 + suffix.Length ||
                        !request.RequestId.StartsWith(prefix, StringComparison.Ordinal) || !request.RequestId.EndsWith(suffix, StringComparison.Ordinal) ||
                        !IsNonnegativeInt64Ascii(request.RequestId, prefix.Length, 19))
                        throw new ArgumentException("Surveyor line request is malformed or out of order.", nameof(requests));
                    sawLine = true;
                }
                else throw new ArgumentException("Threat request kind is not owned by M3D.", nameof(requests));
            }
        }

        private static bool IsNonnegativeInt64Ascii(string value, int offset, int count)
        {
            long parsed = 0;
            try
            {
                for (var index = 0; index < count; index++)
                {
                    var c = value[offset + index];
                    if (c < '0' || c > '9') return false;
                    parsed = checked(parsed * 10 + c - '0');
                }
            }
            catch (OverflowException) { return false; }
            return parsed >= 0;
        }

        private void Preflight(SimulationTick tick, CombatSimulationInput input)
        {
            if (input.Tick != tick || _combatSession.NextExpectedTick != tick || _attackSession.NextExpectedTick != tick)
            {
                throw new InvalidOperationException("Movement, M2A, M1, and combat input ticks must be aligned before combat publication.");
            }

            PreflightHorizon(tick, _maxRegisteredInvulnerabilityTicks);
            for (var index = 0; index < _frozenTargets.Count; index++) _ = _combatSession.Snapshot(_frozenTargets[index].TargetId);
            // REQ-COM-004: ResetEncounter is deliberately committed only after the staged M2A candidate.
            // Prove its otherwise-rejecting lifecycle prerequisite before that first commit.
            if (input.EncounterReset && !_hasLatestOutcome)
                throw new InvalidOperationException("Encounter reset requires a prior successful Combat publication.");
            _reactionDriver.PreflightForCombatPhase(tick, input, _frozenTargets);
            if (!input.Aim.HasValue) return;

            var receipt = _transferDriver.LatestCaptureReceipt;
            if (!receipt.HasValue || receipt.Value.SimulationTick != tick || receipt.Value.AimSampleId != input.Aim.Value.SampleId ||
                receipt.Value.AimSampleTick != input.Aim.Value.SampleTick || receipt.Value.CameraPoseTick != input.Camera.Value.CameraPoseTick)
            {
                throw new InvalidOperationException("Combat capture requires the exact transfer-phase synchronization receipt.");
            }
        }

        internal static void PreflightHorizon(SimulationTick tick, int maxRegisteredInvulnerabilityTicks)
        {
            if (maxRegisteredInvulnerabilityTicks < 0 || maxRegisteredInvulnerabilityTicks > 600) throw new ArgumentOutOfRangeException(nameof(maxRegisteredInvulnerabilityTicks));
            _ = checked(tick.Value + Math.Max(21, maxRegisteredInvulnerabilityTicks));
        }

        private Dictionary<string, bool> CaptureAliveSnapshot()
        {
            var values = new Dictionary<string, bool>(StringComparer.Ordinal);
            for (var index = 0; index < _frozenTargets.Count; index++)
            {
                var target = _frozenTargets[index];
                values.Add(target.TargetId, !_combatSession.Snapshot(target.TargetId).IsDead);
            }

            return values;
        }

        private static DamageRequest[] CopyAndSortCanonicalTrace(IReadOnlyList<DamageRequest> batch)
        {
            var copy = new DamageRequest[batch.Count];
            for (var index = 0; index < copy.Length; index++) copy[index] = batch[index];
            Array.Sort(copy, CompareRequests);
            return copy;
        }

        private static int CompareRequests(DamageRequest left, DamageRequest right)
        {
            var requestId = string.CompareOrdinal(left.RequestId, right.RequestId);
            if (requestId != 0) return requestId;
            var sourceId = string.CompareOrdinal(left.SourceId, right.SourceId);
            if (sourceId != 0) return sourceId;
            var targetId = string.CompareOrdinal(left.TargetId, right.TargetId);
            if (targetId != 0) return targetId;
            var kind = left.Kind.CompareTo(right.Kind);
            return kind != 0 ? kind : left.Amount.CompareTo(right.Amount);
        }

        private CombatantSnapshot[] CaptureCombatSnapshots()
        {
            var values = new CombatantSnapshot[_frozenTargets.Count];
            for (var index = 0; index < values.Length; index++) values[index] = _combatSession.Snapshot(_frozenTargets[index].TargetId);
            return values;
        }

        private void ScheduleNextTickTransferRemovals(SimulationTick tick, IReadOnlyList<DamageResult> results, IReadOnlyList<CombatantSnapshot> snapshots)
        {
            var deadNow = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < results.Count; index++)
            {
                if (results[index].Result == DamageResultCode.Applied) deadNow.Add(results[index].TargetId);
            }

            var removals = new List<TransferTargetRemoved>();
            var nextTick = new SimulationTick(checked(tick.Value + 1));
            for (var index = 0; index < _frozenTargets.Count; index++)
            {
                var target = _frozenTargets[index];
                if (!target.HasMatchingTransferTarget || _scheduledTransferRemovalIds.Contains(target.TargetId) || !deadNow.Contains(target.TargetId) || !snapshots[index].IsDead) continue;
                removals.Add(new TransferTargetRemoved(target.TargetId, nextTick));
            }

            if (removals.Count == 0) return;
            _transferDriver.MergeRemovals(removals);
            for (var index = 0; index < removals.Count; index++) _scheduledTransferRemovalIds.Add(removals[index].TargetId);
        }
    }

    // REQ-COM-001, REQ-COM-004. Internal exact-tick lifecycle input; no device identity or mutable caller collection is retained.
    internal readonly struct CombatSimulationInput
    {
        internal CombatSimulationInput(SimulationTick tick, SimulationCameraPoseSnapshot? camera, AimSample? aim, BasicAttackPressed? press, IReadOnlyList<DamageRequest> externalDamageRequests, bool encounterReset)
        {
            if (externalDamageRequests == null) throw new ArgumentNullException(nameof(externalDamageRequests));
            if (aim.HasValue != camera.HasValue || (aim.HasValue && (aim.Value.SampleTick != tick || camera.Value.CameraPoseTick != aim.Value.CameraPoseTick)) || (press.HasValue && press.Value.Tick != tick) || (press.HasValue && !aim.HasValue))
                throw new ArgumentException("Combat aim/camera/press payloads must be coupled and exact-tick.");
            if (encounterReset && (aim.HasValue || press.HasValue || externalDamageRequests.Count != 0)) throw new ArgumentException("Encounter reset is exclusive with combat capture and damage.");
            var copy = new DamageRequest[externalDamageRequests.Count];
            for (var index = 0; index < copy.Length; index++)
            {
                if (externalDamageRequests[index].Tick != tick) throw new ArgumentException("Every external damage request must use the combat input tick.");
                copy[index] = externalDamageRequests[index];
            }

            Tick = tick;
            Camera = camera;
            Aim = aim;
            Press = press;
            ExternalDamageRequests = copy;
            EncounterReset = encounterReset;
        }

        internal SimulationTick Tick { get; }
        internal SimulationCameraPoseSnapshot? Camera { get; }
        internal AimSample? Aim { get; }
        internal BasicAttackPressed? Press { get; }
        internal IReadOnlyList<DamageRequest> ExternalDamageRequests { get; }
        internal bool EncounterReset { get; }
    }

    // REQ-COM-004. M2A intent, canonical M1 results, and final combat snapshots publish as one internal phase result.
    internal readonly struct CombatSimulationOutcome
    {
        internal CombatSimulationOutcome(SimulationTick tick, bool encounterReset, BasicAttackTickOutcome attack, IReadOnlyList<DamageRequest> canonicalRequestTrace, IReadOnlyList<DamageResult> damageResults, IReadOnlyList<CombatantSnapshot> combatants)
        {
            if (canonicalRequestTrace == null || damageResults == null || canonicalRequestTrace.Count != damageResults.Count) throw new ArgumentException("Combat request trace and M1 results must remain index aligned.");
            Tick = tick;
            EncounterReset = encounterReset;
            Attack = attack;
            var traceCopy = new DamageRequest[canonicalRequestTrace.Count];
            var resultCopy = new DamageResult[damageResults.Count];
            for (var index = 0; index < resultCopy.Length; index++)
            {
                var request = canonicalRequestTrace[index];
                var result = damageResults[index];
                if (request.RequestId != result.RequestId || request.TargetId != result.TargetId || request.Tick != result.Tick || request.Tick != tick) throw new ArgumentException("Combat request trace and M1 results must have exact identity alignment.");
                traceCopy[index] = request;
                resultCopy[index] = result;
            }
            var combatantCopy = new CombatantSnapshot[combatants.Count];
            for (var index = 0; index < combatantCopy.Length; index++) combatantCopy[index] = combatants[index];
            CanonicalRequestTrace = traceCopy;
            DamageResults = resultCopy;
            Combatants = combatantCopy;
        }

        internal SimulationTick Tick { get; }
        internal bool EncounterReset { get; }
        internal BasicAttackTickOutcome Attack { get; }
        internal IReadOnlyList<DamageRequest> CanonicalRequestTrace { get; }
        internal IReadOnlyList<DamageResult> DamageResults { get; }
        internal IReadOnlyList<CombatantSnapshot> Combatants { get; }
    }
}
