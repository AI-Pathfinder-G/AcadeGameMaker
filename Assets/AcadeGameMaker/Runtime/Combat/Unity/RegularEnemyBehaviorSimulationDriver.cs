using System;
using System.Collections.Generic;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-001, REQ-COM-002, REQ-COM-004, REQ-COM-005; affected REQ-WT-003, REQ-WT-005, REQ-WT-007.
    // The -170 bridge owns only immutable M3B2A publications. It never moves bodies, creates damage, or owns M3A/M3B1 state.
    [DefaultExecutionOrder(-170)]
    [DisallowMultipleComponent]
    internal sealed class RegularEnemyBehaviorSimulationDriver : MonoBehaviour
    {
        private const string Walker = "walker";
        private const string Surveyor = "surveyor";
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private CombatSimulationDriver _combatDriver;
        [SerializeField] private RegularEnemyReactionSimulationDriver _reactionDriver;

        private RegularEnemyBehaviorSession _session;
        private CarriedBehaviorView? _carried;
        private SimulationTick _initialBehaviorTick;
        private bool _initialized;

        internal CarriedBehaviorView? LatestCarriedViewForTests => _carried;
        // REQ-COM-004: immutable exact-tick production seam for the following -160 locomotion phase.
        internal CarriedBehaviorView? LatestCarriedViewForLocomotionBridge => _carried;
        // REQ-COM-002, REQ-COM-004: exact immutable source for the +100 threat phase.
        internal CarriedBehaviorView? LatestCarriedViewForThreatBridge => _carried;
        // REQ-COM-001, REQ-COM-004. Immutable completed -170 phase read for M3E1.
        internal CarriedBehaviorView? LatestCarriedViewForEncounterHandoff => _carried;
        internal SimulationTick NextExpectedTickForTests => _session == null ? default(SimulationTick) : _session.NextExpectedTick;
        internal bool HasSessionForTests => _session != null;

        private void FixedUpdate() => AdvanceBehaviorPhase();

        // REQ-COM-001, REQ-COM-004. Noninitializing exact authored graph binding.
        internal void ConfigureForAuthoring(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyReactionSimulationDriver reactionDriver) => Configure(player, combatDriver, reactionDriver);
        internal void ConfigureForTests(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyReactionSimulationDriver reactionDriver) => Configure(player, combatDriver, reactionDriver);

        // REQ-COM-001, REQ-COM-004. Exact read-only graph seam for the +110
        // handoff; it creates no session and does not publish behavior.
        internal void ValidateEncounterGraph(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyReactionSimulationDriver reactionDriver)
        {
            if (_player != player || _combatDriver != combatDriver || _reactionDriver != reactionDriver)
                throw new InvalidOperationException("Behavior encounter graph references are cross-wired.");
        }

        private void Configure(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyReactionSimulationDriver reactionDriver)
        {
            if (_initialized) throw new InvalidOperationException("Behavior driver cannot be reconfigured after initialization.");
            _player = player;
            _combatDriver = combatDriver;
            _reactionDriver = reactionDriver;
        }

        internal CarriedBehaviorView AdvanceBehaviorPhaseForTests() => AdvanceBehaviorPhase();

        internal void ValidateThreatGraph(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyReactionSimulationDriver reactionDriver)
        {
            if (_player != player || _combatDriver != combatDriver || _reactionDriver != reactionDriver)
                throw new InvalidOperationException("Behavior threat graph references are cross-wired.");
            combatDriver.ValidateThreatGraph(player, reactionDriver);
        }

        private CarriedBehaviorView AdvanceBehaviorPhase()
        {
            EnsureBindings();
            var tick = _player.NextExpectedTick;
            var roster = _combatDriver.FrozenTargetsForBehaviorBridge;
            var roles = ValidateAndSelectRoles(roster);
            var outcome = ExactCombatOutcome(tick);
            var reaction = ExactReactionView(tick);
            var player = _player.Snapshot;
            ValidatePlayerSnapshot(tick, player, _initialized ? _initialBehaviorTick : tick);
            var observation = StageObservation(tick, player, roles, outcome, reaction);
            // REQ-COM-004: every result-dependent source above must be exact before a new
            // behavior session is even created; a failed first-tick preflight remains wholly absent.
            EnsureInitialized(tick);
            var input = outcome.EncounterReset
                ? RegularEnemyBehaviorPairInput.EncounterReset(tick, reaction.Walker, reaction.Surveyor)
                : RegularEnemyBehaviorPairInput.Gameplay(tick, observation.WalkerAlive, observation.SurveyorAlive,
                    reaction.Walker, reaction.Surveyor, observation.PlayerXQ1000, observation.WalkerXQ1000, observation.SurveyorXQ1000,
                    observation.WalkerLos, observation.SurveyorLos);

            // AC-COM-003: clone preview validates both exact M3A views and every reachable core horizon without changing behavior state.
            var preview = _session.PreviewNext(input);
            _session.ValidateNext(input);
            AttackRecoveryStarted? recovery = null;
            if (preview.WalkerRecoveryIntent.HasValue)
            {
                var intent = preview.WalkerRecoveryIntent.Value;
                if (!string.Equals(intent.TargetId, Walker, StringComparison.Ordinal) || intent.ScheduledTick.Value != checked(tick.Value + 1))
                    throw new InvalidOperationException("Behavior recovery intent must be the literal walker at t+1.");
                recovery = new AttackRecoveryStarted(intent.AttackCycleOrdinal, intent.ScheduledTick);
                _reactionDriver.PreflightRecoveryForBehaviorBridge(recovery.Value);
            }

            var committed = _session.Process(input);
            if (!PairEquals(preview, committed)) throw new InvalidOperationException("Behavior commit disagreed with its mutation-free preview.");
            if (recovery.HasValue) _reactionDriver.CommitPreflightedRecoveryForBehaviorBridge(recovery.Value);
            _carried = new CarriedBehaviorView(tick, committed);
            return _carried.Value;
        }

        private void EnsureInitialized(SimulationTick firstTick)
        {
            if (_initialized) return;
            _ = ValidateAndSelectRoles(_combatDriver.FrozenTargetsForBehaviorBridge);
            _initialBehaviorTick = firstTick;
            _session = new RegularEnemyBehaviorSession(_initialBehaviorTick);
            _initialized = true;
        }

        private void EnsureBindings()
        {
            if (_player == null || _combatDriver == null || _reactionDriver == null)
                throw new InvalidOperationException("Behavior driver requires explicit player, combat, and reaction bindings.");
        }

        private CombatSimulationOutcome ExactCombatOutcome(SimulationTick tick)
        {
            if (!_combatDriver.HasLatestOutcomeForBehaviorBridge) throw new InvalidOperationException("Behavior phase requires a successful combat publication.");
            var outcome = _combatDriver.LatestOutcomeForBehaviorBridge;
            if (outcome.Tick != tick) throw new InvalidOperationException("Behavior phase requires the exact current combat publication.");
            return outcome;
        }

        private CarriedReactionView ExactReactionView(SimulationTick tick)
        {
            var view = _reactionDriver.LatestCarriedViewForBehaviorBridge;
            if (!view.HasValue || view.Value.Tick != tick || view.Value.Walker.NextExpectedTick.Value != checked(tick.Value + 1) || view.Value.Surveyor.NextExpectedTick.Value != checked(tick.Value + 1))
                throw new InvalidOperationException("Behavior phase requires the exact current reaction publication.");
            return view.Value;
        }

        private static void ValidatePlayerSnapshot(SimulationTick tick, PlayerMotionSnapshot snapshot, SimulationTick initialBehaviorTick)
        {
            var expected = tick == initialBehaviorTick ? initialBehaviorTick : new SimulationTick(checked(tick.Value - 1));
            if (snapshot.Tick != expected) throw new InvalidOperationException("Behavior spatial input requires the approved bootstrap or completed player snapshot.");
        }

        private BehaviorRoles ValidateAndSelectRoles(IReadOnlyList<FrozenCombatTarget> roster)
        {
            if (roster == null) throw new ArgumentNullException(nameof(roster));
            FrozenCombatTarget? walker = null;
            FrozenCombatTarget? surveyor = null;
            string previousId = null;
            for (var index = 0; index < roster.Count; index++)
            {
                var target = roster[index];
                if (previousId != null && string.CompareOrdinal(previousId, target.TargetId) >= 0)
                    throw new InvalidOperationException("Behavior bridge requires the combat-owned frozen roster to remain ordinal sorted.");
                previousId = target.TargetId;
                if (target.TargetId == Walker) { if (walker.HasValue) throw new InvalidOperationException("Behavior roster has duplicate walker."); walker = target; }
                if (target.TargetId == Surveyor) { if (surveyor.HasValue) throw new InvalidOperationException("Behavior roster has duplicate surveyor."); surveyor = target; }
            }
            if (!walker.HasValue || !surveyor.HasValue ||
                walker.Value.MaxHealth != 9 || walker.Value.InvulnerabilityTicks != 0 || !walker.Value.CanReceivePlayerBasicAttack || !walker.Value.HasMatchingTransferTarget ||
                surveyor.Value.MaxHealth != 6 || surveyor.Value.InvulnerabilityTicks != 0 || !surveyor.Value.CanReceivePlayerBasicAttack || !surveyor.Value.HasMatchingTransferTarget ||
                walker.Value.TargetPoseRoot == null || surveyor.Value.TargetPoseRoot == null)
                throw new InvalidOperationException("Behavior bridge requires the frozen M3B1 walker and surveyor combat identities.");
            return new BehaviorRoles(walker.Value, surveyor.Value);
        }

        private BehaviorObservation StageObservation(SimulationTick tick, PlayerMotionSnapshot player, BehaviorRoles roles, CombatSimulationOutcome outcome, CarriedReactionView reaction)
        {
            var playerX = CombatObservationBuilder.Q4096ToQ1000(player.PositionXQ4096);
            var playerY = CombatObservationBuilder.Q4096ToQ1000(player.PositionYQ4096);
            var walkerPose = roles.Walker.TargetPoseRoot.position;
            var surveyorPose = roles.Surveyor.TargetPoseRoot.position;
            var walkerX = CombatObservationBuilder.QuantizePoseQ1000(walkerPose.x);
            var walkerY = CombatObservationBuilder.QuantizePoseQ1000(walkerPose.y);
            var surveyorX = CombatObservationBuilder.QuantizePoseQ1000(surveyorPose.x);
            var surveyorY = CombatObservationBuilder.QuantizePoseQ1000(surveyorPose.y);
            var walkerAimX = checked(walkerX + roles.Walker.LocalAimPointXQ1000);
            var walkerAimY = checked(walkerY + roles.Walker.LocalAimPointYQ1000);
            var surveyorAimX = checked(surveyorX + roles.Surveyor.LocalAimPointXQ1000);
            var surveyorAimY = checked(surveyorY + roles.Surveyor.LocalAimPointYQ1000);
            var walkerLos = TransferLineOfSight.Evaluate(playerX, playerY, walkerAimX, walkerAimY, _player.transform, roles.Walker.TargetPoseRoot, Walker, tick).IsOpen;
            var surveyorLos = TransferLineOfSight.Evaluate(playerX, playerY, surveyorAimX, surveyorAimY, _player.transform, roles.Surveyor.TargetPoseRoot, Surveyor, tick).IsOpen;
            var walkerAlive = !Combatant(outcome, Walker).IsDead;
            var surveyorAlive = !Combatant(outcome, Surveyor).IsDead;
            if (walkerAlive != reaction.Walker.IsAlive || surveyorAlive != reaction.Surveyor.IsAlive)
                throw new InvalidOperationException("Exact M1 final alive values must agree with the current M3A publication.");
            return new BehaviorObservation(playerX, playerY, walkerX, walkerY, surveyorX, surveyorY, walkerLos, surveyorLos, walkerAlive, surveyorAlive);
        }

        private static CombatantSnapshot Combatant(CombatSimulationOutcome outcome, string id)
        {
            CombatantSnapshot? value = null;
            for (var index = 0; index < outcome.Combatants.Count; index++)
            {
                if (outcome.Combatants[index].TargetId != id) continue;
                if (value.HasValue) throw new InvalidOperationException("Combat publication has a duplicate behavior role.");
                value = outcome.Combatants[index];
            }
            return value ?? throw new InvalidOperationException("Combat publication omits a behavior role.");
        }

        private static bool PairEquals(RegularEnemyBehaviorPairSnapshot left, RegularEnemyBehaviorPairSnapshot right) =>
            left.Tick == right.Tick && left.NextExpectedTick == right.NextExpectedTick &&
            WalkerEquals(left.Walker, right.Walker) && SurveyorEquals(left.Surveyor, right.Surveyor) &&
            RecoveryEquals(left.WalkerRecoveryIntent, right.WalkerRecoveryIntent) && ShotEquals(left.SurveyorShotIntent, right.SurveyorShotIntent);

        private static bool WalkerEquals(WalkerBehaviorSnapshot left, WalkerBehaviorSnapshot right) => left.Phase == right.Phase && left.ExclusiveEndTick == right.ExclusiveEndTick && left.IsAlive == right.IsAlive && left.Movement == right.Movement && left.Direction == right.Direction && left.GuardRaised == right.GuardRaised && left.Telegraph == right.Telegraph && left.LatchedDashDirection == right.LatchedDashDirection && left.AttackCycleOrdinal == right.AttackCycleOrdinal;
        private static bool SurveyorEquals(SurveyorBehaviorSnapshot left, SurveyorBehaviorSnapshot right) => left.Phase == right.Phase && left.ExclusiveEndTick == right.ExclusiveEndTick && left.IsAlive == right.IsAlive && left.Movement == right.Movement && left.Direction == right.Direction && left.Telegraph == right.Telegraph && left.HasLatchedTarget == right.HasLatchedTarget && left.LatchedTargetXQ1000 == right.LatchedTargetXQ1000 && left.ShotOrdinal == right.ShotOrdinal;
        private static bool RecoveryEquals(WalkerRecoveryIntent? left, WalkerRecoveryIntent? right) => !left.HasValue ? !right.HasValue : right.HasValue && left.Value.TargetId == right.Value.TargetId && left.Value.AttackCycleOrdinal == right.Value.AttackCycleOrdinal && left.Value.ScheduledTick == right.Value.ScheduledTick;
        private static bool ShotEquals(SurveyorShotIntent? left, SurveyorShotIntent? right) => !left.HasValue ? !right.HasValue : right.HasValue && left.Value.TargetId == right.Value.TargetId && left.Value.ShotOrdinal == right.Value.ShotOrdinal && left.Value.TargetXQ1000 == right.Value.TargetXQ1000;
    }

    // REQ-COM-004: immutable value-only -170 publication; no transform, collider, request or queue reference escapes the phase.
    internal readonly struct CarriedBehaviorView
    {
        internal CarriedBehaviorView(SimulationTick tick, RegularEnemyBehaviorPairSnapshot snapshot)
        {
            if (snapshot.Tick != tick || snapshot.NextExpectedTick.Value != checked(tick.Value + 1)) throw new ArgumentException("Behavior view must be exact and consecutive.");
            Tick = tick;
            Snapshot = snapshot;
        }
        internal SimulationTick Tick { get; }
        internal RegularEnemyBehaviorPairSnapshot Snapshot { get; }
    }

    internal readonly struct BehaviorRoles
    {
        internal BehaviorRoles(FrozenCombatTarget walker, FrozenCombatTarget surveyor) { Walker = walker; Surveyor = surveyor; }
        internal FrozenCombatTarget Walker { get; }
        internal FrozenCombatTarget Surveyor { get; }
    }

    internal readonly struct BehaviorObservation
    {
        internal BehaviorObservation(int playerXQ1000, int playerYQ1000, int walkerXQ1000, int walkerYQ1000, int surveyorXQ1000, int surveyorYQ1000, bool walkerLos, bool surveyorLos, bool walkerAlive, bool surveyorAlive)
        {
            PlayerXQ1000 = playerXQ1000; PlayerYQ1000 = playerYQ1000; WalkerXQ1000 = walkerXQ1000; WalkerYQ1000 = walkerYQ1000; SurveyorXQ1000 = surveyorXQ1000; SurveyorYQ1000 = surveyorYQ1000;
            WalkerLos = walkerLos; SurveyorLos = surveyorLos; WalkerAlive = walkerAlive; SurveyorAlive = surveyorAlive;
        }
        internal int PlayerXQ1000 { get; } internal int PlayerYQ1000 { get; }
        internal int WalkerXQ1000 { get; } internal int WalkerYQ1000 { get; }
        internal int SurveyorXQ1000 { get; } internal int SurveyorYQ1000 { get; }
        internal bool WalkerLos { get; } internal bool SurveyorLos { get; }
        internal bool WalkerAlive { get; } internal bool SurveyorAlive { get; }
    }
}
