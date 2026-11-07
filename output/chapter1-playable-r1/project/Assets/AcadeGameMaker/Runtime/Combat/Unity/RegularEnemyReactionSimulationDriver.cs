using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-001, REQ-COM-002, REQ-COM-004, REQ-COM-005; affected REQ-WT-003, REQ-WT-005, REQ-WT-007.
    // The -180 phase is an adapter only: M3A owns reaction state, M1 owns life and VD-02 owns transfer state/revisions.
    [DefaultExecutionOrder(-180)]
    [DisallowMultipleComponent]
    internal sealed class RegularEnemyReactionSimulationDriver : MonoBehaviour
    {
        private const string Walker = "walker";
        private const string Surveyor = "surveyor";
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private TransferSimulationDriver _transferDriver;
        [SerializeField] private CombatSimulationDriver _combatDriver;

        private readonly SortedDictionary<int, AttackRecoveryStarted> _recoveryByTick = new SortedDictionary<int, AttackRecoveryStarted>();
        private readonly HashSet<long> _queuedCycleIds = new HashSet<long>();
        private readonly HashSet<long> _seenCycleIds = new HashSet<long>();
        private readonly HashSet<long> _tombstonedCycleIds = new HashSet<long>();
        private RegularEnemyReactionSession _walker;
        private RegularEnemyReactionSession _surveyor;
        private CarriedReactionView? _carried;
        private SimulationTick _initialReactionTick;
        private bool _initialized;

        internal CarriedReactionView? CarriedViewForTests => _carried;
        // REQ-COM-004, AC-COM-003: production read-only M3A publication seam for the following -170 phase.
        internal CarriedReactionView? LatestCarriedViewForBehaviorBridge => _carried;
        // REQ-COM-001, REQ-COM-004. Immutable completed -180 phase read for M3E1.
        internal CarriedReactionView? LatestCarriedViewForEncounterHandoff => _carried;
        internal SimulationTick WalkerNextExpectedTickForTests => _walker == null ? default(SimulationTick) : _walker.NextExpectedTick;
        internal SimulationTick SurveyorNextExpectedTickForTests => _surveyor == null ? default(SimulationTick) : _surveyor.NextExpectedTick;
        internal int QueuedRecoveryCountForTests => _recoveryByTick.Count;

        private void FixedUpdate() => AdvanceReactionPhase();

        // REQ-COM-001, REQ-COM-004. Noninitializing exact authored graph binding.
        internal void ConfigureForAuthoring(PlayerMovementController player, TransferSimulationDriver transferDriver,
            CombatSimulationDriver combatDriver) => Configure(player, transferDriver, combatDriver);
        internal void ConfigureForTests(PlayerMovementController player, TransferSimulationDriver transferDriver,
            CombatSimulationDriver combatDriver) => Configure(player, transferDriver, combatDriver);

        private void Configure(PlayerMovementController player, TransferSimulationDriver transferDriver, CombatSimulationDriver combatDriver)
        {
            if (_initialized) throw new InvalidOperationException("Reaction driver cannot be reconfigured after initialization.");
            _player = player;
            _transferDriver = transferDriver;
            _combatDriver = combatDriver;
        }

        // REQ-COM-001, REQ-COM-004, REQ-WT-005. Exact editor/runtime graph identity
        // check only; reaction state is not created or advanced.
        internal void ValidateEncounterGraph(PlayerMovementController player,
            TransferSimulationDriver transferDriver, CombatSimulationDriver combatDriver)
        {
            if (_player != player || _transferDriver != transferDriver || _combatDriver != combatDriver)
                throw new InvalidOperationException("Reaction encounter graph references are cross-wired.");
        }

        internal void SubmitRecoveryForTests(AttackRecoveryStarted value) => SubmitRecovery(value);

        // M3B2 may use this exact seam only while the clock is still before the candidate tick.
        internal void SubmitRecovery(AttackRecoveryStarted value)
        {
            ValidateRecoverySubmission(value);
            CommitPreflightedRecoveryForBehaviorBridge(value);
        }

        // REQ-COM-004, REQ-COM-005: M3B2B calls this before its M3B2A Process. Its
        // exact t+1 restriction prevents delayed/early conversion from becoming queue history.
        internal void PreflightRecoveryForBehaviorBridge(AttackRecoveryStarted value)
        {
            ValidateRecoverySubmission(value);
            if (value.Tick.Value != checked(_player.NextExpectedTick.Value + 1))
                throw new InvalidOperationException("Behavior recovery conversion must target exactly the next global tick.");
        }

        // The fixed main-thread phase has just discharged every ordinary rejection in PreflightRecoveryForBehaviorBridge.
        // This intentionally has no second validation branch: a violation is a fail-stop invariant, never a partial publish.
        internal void CommitPreflightedRecoveryForBehaviorBridge(AttackRecoveryStarted value)
        {
            _recoveryByTick.Add(value.Tick.Value, value);
            _queuedCycleIds.Add(value.AttackCycleId);
            _seenCycleIds.Add(value.AttackCycleId);
        }

        internal void PreflightForCombatPhase(SimulationTick tick, CombatSimulationInput input, IReadOnlyList<FrozenCombatTarget> roster)
        {
            EnsureInitialized(roster);
            if (_walker.NextExpectedTick != tick || _surveyor.NextExpectedTick != tick)
                throw new InvalidOperationException("Both reaction sessions must expect the exact combat tick before M1.");
            if (!_carried.HasValue)
            {
                if (tick != _initialReactionTick) throw new InvalidOperationException("Only the first combat tick may lack a carried reaction view.");
            }
            else if (_carried.Value.Tick.Value != checked(tick.Value - 1) || _carried.Value.Walker.NextExpectedTick != tick || _carried.Value.Surveyor.NextExpectedTick != tick)
            {
                throw new InvalidOperationException("Combat requires the exact immutable reaction view from t-1.");
            }
            var publication = ExactTransferPublication(tick);
            var edges = ProjectEdges(publication);
            ValidateSessionEdge(_walker, edges.Walker);
            ValidateSessionEdge(_surveyor, edges.Surveyor);
            if (_recoveryByTick.TryGetValue(tick.Value, out var recovery) && (recovery.Tick != tick || _tombstonedCycleIds.Contains(recovery.AttackCycleId)))
                throw new InvalidOperationException("Recovery queue contains a stale or tombstoned exact-tick candidate.");
            if (!input.EncounterReset) return;
            if (!publication.Input.IsIdle || _recoveryByTick.ContainsKey(tick.Value) || (_walker.LatestSnapshot.HasValue && _walker.LatestSnapshot.Value.IsHeavy) || (_surveyor.LatestSnapshot.HasValue && _surveyor.LatestSnapshot.Value.IsHeavy))
                throw new InvalidOperationException("Encounter reset requires an idle transfer tick, no recovery candidate, and previously mirrored baseline state.");
        }

        internal Dictionary<string, bool> BuildEffectiveAttackability(SimulationTick tick, IReadOnlyList<FrozenCombatTarget> roster, IReadOnlyDictionary<string, bool> aliveBeforeM1)
        {
            if (aliveBeforeM1 == null) throw new ArgumentNullException(nameof(aliveBeforeM1));
            var values = new Dictionary<string, bool>(StringComparer.Ordinal);
            for (var index = 0; index < roster.Count; index++)
            {
                var target = roster[index];
                if (!aliveBeforeM1.TryGetValue(target.TargetId, out var alive)) throw new ArgumentException("Combat alive snapshot is incomplete.", nameof(aliveBeforeM1));
                var allowed = target.CanReceivePlayerBasicAttack && alive;
                if (target.TargetId == Walker) allowed = allowed && _carried.HasValue && _carried.Value.Walker.ReactionAllowsBasicAttack;
                values.Add(target.TargetId, allowed);
            }
            return values;
        }

        internal CarriedReactionView AdvanceReactionPhaseForTests() => AdvanceReactionPhase();

        private CarriedReactionView AdvanceReactionPhase()
        {
            EnsureBindings();
            var tick = _player.NextExpectedTick;
            var outcome = _combatDriver.LatestOutcomeForTests;
            if (outcome.Tick != tick) throw new InvalidOperationException("Reaction publication requires the exact successful combat outcome.");
            EnsureInitializedFromCombat();
            var transfer = ExactTransferPublication(tick);
            var edges = ProjectEdges(transfer);
            var walkerSnapshot = Combatant(outcome, Walker);
            var surveyorSnapshot = Combatant(outcome, Surveyor);
            var reset = outcome.EncounterReset;
            var candidate = _recoveryByTick.TryGetValue(tick.Value, out var queued) ? (AttackRecoveryStarted?)queued : null;
            var walkerEvent = reset || walkerSnapshot.IsDead ? WalkerReactionEvent.None : SelectWalkerEvent(outcome, candidate);
            var walkerInput = reset
                ? RegularEnemyReactionInput.EncounterReset(tick)
                : RegularEnemyReactionInput.Gameplay(tick, Walker, !walkerSnapshot.IsDead, edges.Walker, walkerEvent);
            var surveyorInput = reset
                ? RegularEnemyReactionInput.EncounterReset(tick)
                : RegularEnemyReactionInput.Gameplay(tick, Surveyor, !surveyorSnapshot.IsDead, edges.Surveyor, WalkerReactionEvent.None);

            // AC-COM-003, AC-WT-005: no session, carried view or queue mutation occurs
            // until both post-M1 inputs have passed exactly the M3A validation path.
            _walker.ValidateNext(walkerInput);
            _surveyor.ValidateNext(surveyorInput);
            var nextWalker = _walker.Process(walkerInput);
            var nextSurveyor = _surveyor.Process(surveyorInput);
            if (candidate.HasValue)
            {
                _recoveryByTick.Remove(tick.Value);
                _queuedCycleIds.Remove(candidate.Value.AttackCycleId);
                _tombstonedCycleIds.Add(candidate.Value.AttackCycleId);
            }
            if (reset)
            {
                _recoveryByTick.Clear();
                _queuedCycleIds.Clear();
                _seenCycleIds.Clear();
                _tombstonedCycleIds.Clear();
            }
            _carried = new CarriedReactionView(tick, nextWalker, nextSurveyor);
            return _carried.Value;
        }

        private void EnsureInitializedFromCombat()
        {
            if (_initialized) return;
            throw new InvalidOperationException("Combat must invoke reaction structural preflight before M1 publication.");
        }

        private void EnsureInitialized(IReadOnlyList<FrozenCombatTarget> roster)
        {
            EnsureBindings();
            if (_initialized) return;
            FrozenCombatTarget? walker = null;
            FrozenCombatTarget? surveyor = null;
            for (var index = 0; index < roster.Count; index++)
            {
                var target = roster[index];
                if (target.TargetId == Walker) { if (walker.HasValue) throw new InvalidOperationException("Reaction roster has duplicate walker."); walker = target; }
                if (target.TargetId == Surveyor) { if (surveyor.HasValue) throw new InvalidOperationException("Reaction roster has duplicate surveyor."); surveyor = target; }
            }
            if (!walker.HasValue || !surveyor.HasValue || walker.Value.MaxHealth != 9 || walker.Value.InvulnerabilityTicks != 0 || !walker.Value.CanReceivePlayerBasicAttack || !walker.Value.HasMatchingTransferTarget ||
                surveyor.Value.MaxHealth != 6 || surveyor.Value.InvulnerabilityTicks != 0 || !surveyor.Value.CanReceivePlayerBasicAttack || !surveyor.Value.HasMatchingTransferTarget)
                throw new InvalidOperationException("Reaction adapter requires the frozen co-authored walker 9/0/true and surveyor 6/0/true roster.");
            var first = _player.NextExpectedTick;
            _walker = new RegularEnemyReactionSession(Walker, first);
            _surveyor = new RegularEnemyReactionSession(Surveyor, first);
            _initialReactionTick = first;
            _initialized = true;
        }

        private void EnsureBindings()
        {
            if (_player == null || _transferDriver == null || _combatDriver == null) throw new InvalidOperationException("Reaction driver requires explicit player, transfer, and combat bindings.");
        }

        private void ValidateRecoverySubmission(AttackRecoveryStarted value)
        {
            EnsureBindings();
            if (value.Tick.Value <= _player.NextExpectedTick.Value || _recoveryByTick.ContainsKey(value.Tick.Value) || _queuedCycleIds.Contains(value.AttackCycleId) || _seenCycleIds.Contains(value.AttackCycleId) || _tombstonedCycleIds.Contains(value.AttackCycleId))
                throw new InvalidOperationException("Recovery candidates must be unique, untombstoned, and submitted for a strictly future tick.");
        }

        private TransferPhasePublication ExactTransferPublication(SimulationTick tick)
        {
            var publication = _transferDriver.LatestPhasePublication;
            if (!publication.HasValue || publication.Value.Tick != tick) throw new InvalidOperationException("Reaction phase requires the exact successful transfer publication.");
            return publication.Value;
        }

        private static CombatantSnapshot Combatant(CombatSimulationOutcome outcome, string id)
        {
            for (var index = 0; index < outcome.Combatants.Count; index++) if (outcome.Combatants[index].TargetId == id) return outcome.Combatants[index];
            throw new InvalidOperationException("Combat outcome omits a reaction roster target.");
        }

        private static WalkerReactionEvent SelectWalkerEvent(CombatSimulationOutcome outcome, AttackRecoveryStarted? candidate)
        {
            for (var index = 0; index < outcome.DamageResults.Count; index++)
            {
                var result = outcome.DamageResults[index];
                var request = outcome.CanonicalRequestTrace[index];
                if (result.TargetId == Walker && result.Result == DamageResultCode.Applied && request.Kind == DamageKind.HeavyImpact)
                    return WalkerReactionEvent.HeavyImpactApplied(request.RequestId);
            }
            return candidate.HasValue ? WalkerReactionEvent.AttackRecoveryStarted(candidate.Value.AttackCycleId) : WalkerReactionEvent.None;
        }

        private static void ValidateSessionEdge(RegularEnemyReactionSession session, RegularEnemyTransferTransition? edge)
        {
            if (!edge.HasValue) return;
            var snapshot = session.LatestSnapshot;
            if (snapshot.HasValue && edge.Value.Revision <= snapshot.Value.LastTransferRevision) throw new InvalidOperationException("Transfer revisions must increase across reaction publications.");
        }

        private static ReactionEdges ProjectEdges(TransferPhasePublication publication)
        {
            var projected = new List<RegularEnemyTransferTransition>();
            Project(publication.Outcome.RemovalResult, projected);
            Project(publication.Outcome.PressResult, projected);
            projected.Sort((left, right) => left.Revision.CompareTo(right.Revision));
            for (var index = 1; index < projected.Count; index++) if (projected[index - 1].Revision >= projected[index].Revision) throw new InvalidOperationException("Projected reaction edges require strictly increasing transfer revisions.");
            RegularEnemyTransferTransition? walker = null;
            RegularEnemyTransferTransition? surveyor = null;
            for (var index = 0; index < projected.Count; index++)
            {
                var edge = projected[index];
                if (edge.TargetId == Walker) { if (walker.HasValue) throw new InvalidOperationException("One transfer tick cannot emit two walker reaction edges."); walker = edge; }
                if (edge.TargetId == Surveyor) { if (surveyor.HasValue) throw new InvalidOperationException("One transfer tick cannot emit two surveyor reaction edges."); surveyor = edge; }
            }
            if (projected.Count > 0) ValidateFinalTransferSnapshot(publication.Outcome.Snapshot, projected[projected.Count - 1]);
            return new ReactionEdges(walker, surveyor);
        }

        private static void Project(TransferAttemptResult? result, List<RegularEnemyTransferTransition> values)
        {
            if (!result.HasValue || !result.Value.Success) return;
            var value = result.Value;
            if (value.StateChanged.HasValue)
            {
                var changed = value.StateChanged.Value;
                if (changed.TargetId == Walker || changed.TargetId == Surveyor)
                {
                    if (changed.Tick != value.Tick || changed.Revision != value.Session.Revision || value.Session.ActiveTargetId != changed.TargetId)
                        throw new InvalidOperationException("Reaction transfer apply edge is stale or disagrees with its exact result session.");
                    values.Add(new RegularEnemyTransferTransition(changed.TargetId, RegularEnemyTransferState.Heavy, changed.Revision, changed.Tick));
                }
                return;
            }
            if (value.Cleared.HasValue)
            {
                var cleared = value.Cleared.Value;
                if (cleared.PreviousTargetId == Walker || cleared.PreviousTargetId == Surveyor)
                {
                    if (cleared.Tick != value.Tick || cleared.Revision != value.Session.Revision || value.Session.ActiveTargetId.Length != 0)
                        throw new InvalidOperationException("Reaction transfer clear edge is stale or disagrees with its exact result session.");
                    values.Add(new RegularEnemyTransferTransition(cleared.PreviousTargetId, RegularEnemyTransferState.Baseline, cleared.Revision, cleared.Tick));
                }
            }
        }

        private static void ValidateFinalTransferSnapshot(TransferSessionSnapshot snapshot, RegularEnemyTransferTransition edge)
        {
            if (snapshot.Tick != edge.Tick || snapshot.Revision < edge.Revision) throw new InvalidOperationException("Projected reaction edge disagrees with final transfer snapshot.");
            // A later same-tick box or boss-payload transition is intentionally outside
            // M3A.  When this literal reaction edge is also the final transition, the
            // transfer snapshot must still prove its exact resulting state.
            if (snapshot.Revision > edge.Revision) return;
            if (edge.State == RegularEnemyTransferState.Heavy && snapshot.ActiveTargetId != edge.TargetId) throw new InvalidOperationException("Heavy reaction edge disagrees with final transfer target.");
            if (edge.State == RegularEnemyTransferState.Baseline && snapshot.ActiveTargetId.Length != 0) throw new InvalidOperationException("Baseline reaction edge disagrees with final transfer target.");
        }
    }

    internal readonly struct AttackRecoveryStarted
    {
        internal AttackRecoveryStarted(long attackCycleId, SimulationTick tick)
        {
            if (attackCycleId < 0 || tick.Value < 0) throw new ArgumentOutOfRangeException();
            AttackCycleId = attackCycleId;
            Tick = tick;
        }
        internal long AttackCycleId { get; }
        internal SimulationTick Tick { get; }
    }

    internal readonly struct CarriedReactionView
    {
        internal CarriedReactionView(SimulationTick tick, RegularEnemyReactionSnapshot walker, RegularEnemyReactionSnapshot surveyor)
        {
            if (walker.Tick != tick || surveyor.Tick != tick || walker.TargetId != "walker" || surveyor.TargetId != "surveyor") throw new ArgumentException("Carried reaction view must be exact and role aligned.");
            Tick = tick; Walker = walker; Surveyor = surveyor;
        }
        internal SimulationTick Tick { get; }
        internal RegularEnemyReactionSnapshot Walker { get; }
        internal RegularEnemyReactionSnapshot Surveyor { get; }
    }

    internal readonly struct ReactionEdges
    {
        internal ReactionEdges(RegularEnemyTransferTransition? walker, RegularEnemyTransferTransition? surveyor) { Walker = walker; Surveyor = surveyor; }
        internal RegularEnemyTransferTransition? Walker { get; }
        internal RegularEnemyTransferTransition? Surveyor { get; }
    }
}
