using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-001, REQ-COM-002, REQ-COM-004, REQ-COM-005.  The +110 handoff
    // consumes completed fixed-phase publications.  It is explicitly view-only:
    // no gameplay phase, request queue, Transform, renderer, or physics query is
    // driven from the presentation it publishes.
    [DefaultExecutionOrder(110)]
    [DisallowMultipleComponent]
    internal sealed class RegularEnemyEncounterHandoffAdapter : MonoBehaviour
    {
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private TransferSimulationDriver _transfer;
        [SerializeField] private CombatSimulationDriver _combat;
        [SerializeField] private RegularEnemyReactionSimulationDriver _reaction;
        [SerializeField] private RegularEnemyBehaviorSimulationDriver _behavior;
        [SerializeField] private RegularEnemyLocomotionSimulationDriver _locomotion;
        [SerializeField] private RegularEnemyThreatSimulationDriver _threat;
        [SerializeField] private CapsuleCollider2D _playerCollider;
        [SerializeField] private TransferTarget _surveyorTarget;
        [SerializeField] private TransferTarget _walkerTarget;
        [SerializeField] private BoxCollider2D _surveyorCollider;
        [SerializeField] private BoxCollider2D _walkerCollider;

        private RegularEnemyEncounterLifecycleSession _session;
        private RegularEnemyEncounterPresentationView? _latest;
        private bool _surveyorIgnored;
        private bool _walkerIgnored;
        private bool _fatalCollisionCommitFailure;
        private EncounterColliderState? _retainedColliderState;

        // REQ-COM-004. Production consumers receive a copied immutable value
        // view. The adapter retains neither their state nor an event queue.
        internal RegularEnemyEncounterPresentationView? LatestPresentationView => _latest;
        // REQ-COM-001, REQ-COM-004. Narrow immutable lifecycle-state reads for
        // composition verification; they provide no mutation or replay path.
        internal bool HasOwnedLifecycleForEncounterHandoff => _session != null;
        internal bool HasRetainedColliderStateForEncounterHandoff => _retainedColliderState.HasValue;
        internal bool SurveyorPairIgnoredForEncounterHandoff => _surveyorIgnored;
        internal bool WalkerPairIgnoredForEncounterHandoff => _walkerIgnored;

        private void FixedUpdate() => AdvanceHandoff();

        // REQ-COM-001, REQ-COM-004. Editor-only direct composition; no runtime
        // discovery or repair path exists.
        internal void ConfigureForAuthoring(PlayerMovementController player, TransferSimulationDriver transfer,
            CombatSimulationDriver combat, RegularEnemyReactionSimulationDriver reaction,
            RegularEnemyBehaviorSimulationDriver behavior, RegularEnemyLocomotionSimulationDriver locomotion,
            RegularEnemyThreatSimulationDriver threat, CapsuleCollider2D playerCollider,
            TransferTarget surveyorTarget, TransferTarget walkerTarget, BoxCollider2D surveyorCollider, BoxCollider2D walkerCollider)
        {
            if (_session != null) throw new InvalidOperationException("Encounter handoff cannot be reconfigured after use.");
            _player = player; _transfer = transfer; _combat = combat; _reaction = reaction; _behavior = behavior;
            _locomotion = locomotion; _threat = threat; _playerCollider = playerCollider;
            _surveyorTarget = surveyorTarget; _walkerTarget = walkerTarget; _surveyorCollider = surveyorCollider; _walkerCollider = walkerCollider;
        }

        internal void ValidateEncounterGraph(PlayerMovementController player, TransferSimulationDriver transfer,
            CombatSimulationDriver combat, RegularEnemyReactionSimulationDriver reaction,
            RegularEnemyBehaviorSimulationDriver behavior, RegularEnemyLocomotionSimulationDriver locomotion,
            RegularEnemyThreatSimulationDriver threat, CapsuleCollider2D playerCollider,
            TransferTarget surveyorTarget, TransferTarget walkerTarget, BoxCollider2D surveyorCollider, BoxCollider2D walkerCollider)
        {
            if (_player != player || _transfer != transfer || _combat != combat || _reaction != reaction || _behavior != behavior ||
                _locomotion != locomotion || _threat != threat || _playerCollider != playerCollider ||
                _surveyorTarget != surveyorTarget || _walkerTarget != walkerTarget || _surveyorCollider != surveyorCollider || _walkerCollider != walkerCollider)
                throw new InvalidOperationException("Encounter handoff graph references are cross-wired.");
            if (_playerCollider == null || _surveyorTarget == null || _walkerTarget == null || _surveyorCollider == null || _walkerCollider == null || _playerCollider.isTrigger ||
                _surveyorCollider.isTrigger || _walkerCollider.isTrigger)
                throw new InvalidOperationException("Encounter handoff authoring requires exact solid collider bindings.");
        }

        private void AdvanceHandoff()
        {
            if (_fatalCollisionCommitFailure) throw new InvalidOperationException("M3E1 handoff stopped after a collision commit failure.");
            ValidateRequiredBindings();
            ValidateRetainedHandoffState();
            var source = CaptureExactSources();
            var geometry = ValidateFrozenGraph();
            if (Physics2D.GetIgnoreCollision(_playerCollider, _surveyorCollider) != _surveyorIgnored ||
                Physics2D.GetIgnoreCollision(_playerCollider, _walkerCollider) != _walkerIgnored)
                throw new InvalidOperationException("Encounter collision pair state drifted externally.");
            // On first use every rejection-capable capture above completes before
            // a staged lifecycle owner exists or retained adapter state changes.
            var staged = _session ?? new RegularEnemyEncounterLifecycleSession(source.Tick);
            var input = ToLifecycleInput(source);
            var preview = staged.PreviewNext(input);
            geometry = ValidateFrozenGraph();
            ValidateSourcesStillExact(source);
            if (Physics2D.GetIgnoreCollision(_playerCollider, _surveyorCollider) != _surveyorIgnored ||
                Physics2D.GetIgnoreCollision(_playerCollider, _walkerCollider) != _walkerIgnored)
                throw new InvalidOperationException("Encounter collision pair state drifted during final preflight.");
            staged.CommitNext(input, preview);
            try
            {
                ApplyPair(_surveyorCollider, preview.SurveyorIgnoreCollision, ref _surveyorIgnored);
                ApplyPair(_walkerCollider, preview.WalkerIgnoreCollision, ref _walkerIgnored);
            }
            catch
            {
                // REQ-COM-001, REQ-COM-004. A post-core engine pair failure has
                // no safe rollback; retain any verified earlier pair and fail-stop.
                _fatalCollisionCommitFailure = true;
                throw;
            }
            _session = staged;
            _retainedColliderState = geometry;
            _latest = new RegularEnemyEncounterPresentationView(source.Tick, preview.Snapshot.NextExpectedTick,
                source.PlayerAlive, source.Reaction, source.Behavior, source.Threat, preview);
        }

        // REQ-COM-001, REQ-COM-004. Missing serialized references fail closed
        // before any source read, staged owner, or collision-pair operation.
        private void ValidateRequiredBindings()
        {
            if (_player == null || _transfer == null || _combat == null || _reaction == null || _behavior == null || _locomotion == null || _threat == null ||
                _playerCollider == null || _surveyorTarget == null || _walkerTarget == null || _surveyorCollider == null || _walkerCollider == null)
                throw new InvalidOperationException("Encounter handoff requires every serialized graph binding.");
        }

        private void ValidateRetainedHandoffState()
        {
            if (_session == null)
            {
                if (_latest.HasValue || _retainedColliderState.HasValue || _surveyorIgnored || _walkerIgnored)
                    throw new InvalidOperationException("Unowned M3E1 handoff state is not pristine.");
                return;
            }
            if (!_latest.HasValue || !_session.LatestSnapshot.HasValue || _latest.Value.NextExpectedTick != _session.NextExpectedTick ||
                _latest.Value.Tick != _session.LatestSnapshot.Value.Tick)
                throw new InvalidOperationException("M3E1 lifecycle and carried presentation state are not aligned.");
        }

        private EncounterSources CaptureExactSources()
        {
            if (!_combat.HasLatestOutcomeForEncounterHandoff) throw new InvalidOperationException("Encounter handoff requires Combat publication.");
            var playerSnapshot = _player.Snapshot;
            var playerNextExpectedTick = _player.NextExpectedTick;
            var tick = playerSnapshot.Tick;
            if (tick.Value < 0 || playerNextExpectedTick.Value != checked(tick.Value + 1)) throw new InvalidOperationException("Player publication is not exact.");
            var transfer = _transfer.LatestPhasePublication;
            var reaction = _reaction.LatestCarriedViewForEncounterHandoff;
            var behavior = _behavior.LatestCarriedViewForEncounterHandoff;
            var motion = _locomotion.LatestCarriedViewForEncounterHandoff;
            var threat = _threat.LatestCarriedViewForEncounterHandoff;
            if (!transfer.HasValue || transfer.Value.Tick != tick || !reaction.HasValue || reaction.Value.Tick != tick ||
                !behavior.HasValue || behavior.Value.Tick != tick || !motion.HasValue || motion.Value.Tick != tick ||
                !threat.HasValue || threat.Value.Tick != tick)
                throw new InvalidOperationException("M3E1 requires every exact completed upstream publication.");
            _transfer.ValidateEncounterHandoffPublication(_player, _surveyorTarget, _walkerTarget, _surveyorCollider, _walkerCollider, tick);
            var combat = _combat.LatestOutcomeForEncounterHandoff;
            if (combat.Tick != tick) throw new InvalidOperationException("Combat publication is stale or future.");
            if (reaction.Value.Walker.NextExpectedTick.Value != tick.Value + 1 || reaction.Value.Surveyor.NextExpectedTick.Value != tick.Value + 1 ||
                behavior.Value.Snapshot.NextExpectedTick.Value != tick.Value + 1 || motion.Value.Snapshot.NextExpectedTick.Value != tick.Value + 1 ||
                threat.Value.Snapshot.NextExpectedTick.Value != tick.Value + 1)
                throw new InvalidOperationException("M3E1 upstream next ticks are not exact.");
            var roles = CombatRoles(combat);
            var playerAlive = !roles[0].IsDead;
            if (reaction.Value.Walker.IsAlive != !roles[2].IsDead || reaction.Value.Surveyor.IsAlive != !roles[1].IsDead ||
                behavior.Value.Snapshot.Walker.IsAlive != !roles[2].IsDead || behavior.Value.Snapshot.Surveyor.IsAlive != !roles[1].IsDead ||
                motion.Value.Snapshot.Walker.Dead != roles[2].IsDead || motion.Value.Snapshot.Surveyor.Dead != roles[1].IsDead ||
                threat.Value.Snapshot.PlayerDead != roles[0].IsDead || threat.Value.Snapshot.WalkerDead != roles[2].IsDead || threat.Value.Snapshot.SurveyorDead != roles[1].IsDead)
                throw new InvalidOperationException("Combat and regular-enemy phase alive states disagree.");
            if (combat.EncounterReset != IsResetShape(reaction.Value, behavior.Value, motion.Value, threat.Value))
                throw new InvalidOperationException("M3E1 reset discriminators are mixed.");
            return new EncounterSources(tick, playerSnapshot, playerNextExpectedTick, transfer.Value, combat, reaction.Value, behavior.Value, motion.Value, threat.Value, roles, playerAlive);
        }

        private static bool IsResetShape(CarriedReactionView reaction, CarriedBehaviorView behavior,
            CarriedEnemyMotionView motion, CarriedEnemyThreatView threat) =>
            behavior.Snapshot.Walker.Phase == WalkerBehaviorPhase.Reset && behavior.Snapshot.Surveyor.Phase == SurveyorBehaviorPhase.Reset &&
            reaction.Walker.IsAlive && reaction.Surveyor.IsAlive &&
            !motion.Snapshot.Walker.Dead && !motion.Snapshot.Surveyor.Dead &&
            !threat.Snapshot.PlayerDead && !threat.Snapshot.WalkerDead && !threat.Snapshot.SurveyorDead;

        private static RegularEnemyEncounterLifecycleInput ToLifecycleInput(EncounterSources source) =>
            new RegularEnemyEncounterLifecycleInput(source.Tick,
                source.Combat.EncounterReset ? RegularEnemyEncounterLifecycleInputKind.EncounterReset : RegularEnemyEncounterLifecycleInputKind.Gameplay,
                source.Combat.CanonicalRequestTrace, source.Combat.DamageResults, source.Roles);

        private static RegularEnemyEncounterRoleState[] CombatRoles(CombatSimulationOutcome outcome)
        {
            if (outcome.Combatants.Count != 3) throw new InvalidOperationException("Combat roster is not exact.");
            var values = new RegularEnemyEncounterRoleState[3];
            const string player = "player"; const string surveyor = "surveyor"; const string walker = "walker";
            var ids = new[] { player, surveyor, walker };
            for (var role = 0; role < ids.Length; role++)
            {
                CombatantSnapshot? found = null;
                for (var i = 0; i < outcome.Combatants.Count; i++) if (outcome.Combatants[i].TargetId == ids[role])
                { if (found.HasValue) throw new InvalidOperationException("Combat role is duplicated."); found = outcome.Combatants[i]; }
                if (!found.HasValue) throw new InvalidOperationException("Combat role is absent.");
                values[role] = new RegularEnemyEncounterRoleState(ids[role], found.Value.CurrentHealth, found.Value.IsDead);
            }
            return values;
        }

        private EncounterColliderState ValidateFrozenGraph()
        {
            if (_player == null || _transfer == null || _combat == null || _reaction == null || _behavior == null || _locomotion == null || _threat == null ||
                _playerCollider == null || _surveyorTarget == null || _walkerTarget == null || _surveyorCollider == null || _walkerCollider == null || _playerCollider.isTrigger || _surveyorCollider.isTrigger || _walkerCollider.isTrigger ||
                !_playerCollider.enabled || !_surveyorCollider.enabled || !_walkerCollider.enabled)
                throw new InvalidOperationException("Encounter handoff requires exact active bindings and solid colliders.");
            var roster = _combat.FrozenTargetsForEncounterHandoff;
            if (roster.Count != 3 || roster[0].TargetId != "player" || roster[1].TargetId != "surveyor" || roster[2].TargetId != "walker" ||
                roster[0].TargetCollider != _playerCollider || roster[1].TargetCollider != _surveyorCollider || roster[2].TargetCollider != _walkerCollider ||
                roster[0].TargetPoseRoot != _player.transform || _surveyorTarget.TargetCollider != roster[1].TargetCollider ||
                _walkerTarget.TargetCollider != roster[2].TargetCollider || _surveyorTarget.TargetPoseRoot != roster[1].TargetPoseRoot ||
                _walkerTarget.TargetPoseRoot != roster[2].TargetPoseRoot)
                throw new InvalidOperationException("Encounter handoff does not bind the frozen Combat graph.");
            _reaction.ValidateEncounterGraph(_player, _transfer, _combat);
            _behavior.ValidateEncounterGraph(_player, _combat, _reaction);
            _locomotion.ValidateEncounterGraph(_player, _combat, _reaction, _behavior);
            _threat.ValidateEncounterGraph(_player, _combat, _behavior, _locomotion);
            var current = EncounterColliderState.Capture(_playerCollider, _surveyorCollider, _walkerCollider);
            if (_retainedColliderState.HasValue && !_retainedColliderState.Value.Equals(current))
                throw new InvalidOperationException("Encounter handoff retained collider/body geometry drifted.");
            return current;
        }

        private void ValidateSourcesStillExact(EncounterSources expected)
        {
            var current = CaptureExactSources();
            if (current.Tick != expected.Tick || !SamePlayer(current.PlayerSnapshot, expected.PlayerSnapshot) || current.PlayerNextExpectedTick != expected.PlayerNextExpectedTick ||
                !SameTransfer(current.Transfer, expected.Transfer) || !current.Reaction.Equals(expected.Reaction) || !current.Behavior.Equals(expected.Behavior) ||
                !current.Motion.Equals(expected.Motion) || !current.Threat.Equals(expected.Threat) || !SameCombat(current.Combat, expected.Combat) ||
                !SameRoles(current.Roles, expected.Roles))
                throw new InvalidOperationException("M3E1 source changed during preflight.");
        }

        private static bool SameRoles(IReadOnlyList<RegularEnemyEncounterRoleState> left, IReadOnlyList<RegularEnemyEncounterRoleState> right)
        {
            if (left.Count != right.Count) return false;
            for (var i = 0; i < left.Count; i++)
                if (left[i].TargetId != right[i].TargetId || left[i].CurrentHealth != right[i].CurrentHealth || left[i].IsDead != right[i].IsDead) return false;
            return true;
        }

        private static bool SamePlayer(PlayerMotionSnapshot left, PlayerMotionSnapshot right) => left.Tick == right.Tick &&
            left.PositionXQ4096 == right.PositionXQ4096 && left.PositionYQ4096 == right.PositionYQ4096 &&
            left.VelocityXQ4096 == right.VelocityXQ4096 && left.VelocityYQ4096 == right.VelocityYQ4096 &&
            left.IsGrounded == right.IsGrounded && left.WallId == right.WallId && left.WallSide == right.WallSide &&
            left.ActionState == right.ActionState && left.IsAirDashAvailable == right.IsAirDashAvailable;

        private static bool SameTransfer(TransferPhasePublication left, TransferPhasePublication right)
        {
            var a = left.Outcome.Snapshot; var b = right.Outcome.Snapshot;
            var x = left.Input; var y = right.Input;
            return left.Tick == right.Tick && a.Tick == b.Tick && a.HighlightedTargetId == b.HighlightedTargetId && a.ActiveTargetId == b.ActiveTargetId &&
                a.PlayerState == b.PlayerState && a.TransitionLockEndTick == b.TransitionLockEndTick && a.LastFailureReason == b.LastFailureReason && a.Revision == b.Revision &&
                x.HadAim == y.HadAim && x.HadPress == y.HadPress && x.RemovalCount == y.RemovalCount && x.LifecycleReason == y.LifecycleReason &&
                SameTransferAttempt(left.Outcome.RemovalResult, right.Outcome.RemovalResult) &&
                SameTransferAttempt(left.Outcome.PressResult, right.Outcome.PressResult);
        }

        // REQ-COM-001, REQ-COM-004. Transfer's immutable phase publication is
        // a complete source discriminator: nested removal/press results are
        // not a presentation shortcut and must not change during preflight.
        private static bool SameTransferAttempt(TransferAttemptResult? left, TransferAttemptResult? right)
        {
            if (left.HasValue != right.HasValue) return false;
            if (!left.HasValue) return true;
            var a = left.Value; var b = right.Value;
            return a.Tick == b.Tick && a.AimSampleId == b.AimSampleId && a.SelectedTargetId == b.SelectedTargetId &&
                a.Success == b.Success && a.FailureReason == b.FailureReason && SameTransferSession(a.Session, b.Session) &&
                SameTransferStateChanged(a.StateChanged, b.StateChanged) && SameTransferCleared(a.Cleared, b.Cleared);
        }

        private static bool SameTransferSession(TransferSessionSnapshot left, TransferSessionSnapshot right) => left.Tick == right.Tick &&
            left.HighlightedTargetId == right.HighlightedTargetId && left.ActiveTargetId == right.ActiveTargetId &&
            left.PlayerState == right.PlayerState && left.TransitionLockEndTick == right.TransitionLockEndTick &&
            left.LastFailureReason == right.LastFailureReason && left.Revision == right.Revision;

        private static bool SameTransferStateChanged(TransferStateChanged? left, TransferStateChanged? right)
        {
            if (left.HasValue != right.HasValue) return false;
            if (!left.HasValue) return true;
            var a = left.Value; var b = right.Value;
            return a.TargetId == b.TargetId && a.PlayerModifierId == b.PlayerModifierId && a.TargetModifierId == b.TargetModifierId &&
                a.Tick == b.Tick && a.Revision == b.Revision;
        }

        private static bool SameTransferCleared(TransferCleared? left, TransferCleared? right)
        {
            if (left.HasValue != right.HasValue) return false;
            if (!left.HasValue) return true;
            var a = left.Value; var b = right.Value;
            return a.Reason == b.Reason && a.PreviousTargetId == b.PreviousTargetId && a.Tick == b.Tick && a.Revision == b.Revision;
        }

        private static bool SameCombat(CombatSimulationOutcome left, CombatSimulationOutcome right)
        {
            if (left.Tick != right.Tick || left.EncounterReset != right.EncounterReset || left.CanonicalRequestTrace.Count != right.CanonicalRequestTrace.Count ||
                left.DamageResults.Count != right.DamageResults.Count || left.Combatants.Count != right.Combatants.Count) return false;
            for (var i = 0; i < left.CanonicalRequestTrace.Count; i++)
            {
                var a = left.CanonicalRequestTrace[i]; var b = right.CanonicalRequestTrace[i];
                var x = left.DamageResults[i]; var y = right.DamageResults[i];
                if (a.RequestId != b.RequestId || a.SourceId != b.SourceId || a.TargetId != b.TargetId || a.Amount != b.Amount || a.Kind != b.Kind || a.Tick != b.Tick ||
                    x.RequestId != y.RequestId || x.TargetId != y.TargetId || x.AppliedAmount != y.AppliedAmount || x.Result != y.Result || x.Tick != y.Tick) return false;
            }
            for (var i = 0; i < left.Combatants.Count; i++)
            {
                var a = left.Combatants[i]; var b = right.Combatants[i];
                if (a.TargetId != b.TargetId || a.CurrentHealth != b.CurrentHealth || a.IsDead != b.IsDead ||
                    a.InvulnerabilityEndTick != b.InvulnerabilityEndTick || a.AcceptedRequestCount != b.AcceptedRequestCount) return false;
            }
            return true;
        }

        private void ApplyPair(Collider2D enemy, bool desired, ref bool retained)
        {
            if (retained == desired) return;
            Physics2D.IgnoreCollision(_playerCollider, enemy, desired);
            if (Physics2D.GetIgnoreCollision(_playerCollider, enemy) != desired)
                throw new InvalidOperationException("M3E1 collision pair application did not persist.");
            retained = desired;
        }

        private readonly struct EncounterSources
        {
            internal EncounterSources(SimulationTick tick, PlayerMotionSnapshot playerSnapshot, SimulationTick playerNextExpectedTick, TransferPhasePublication transfer, CombatSimulationOutcome combat, CarriedReactionView reaction, CarriedBehaviorView behavior,
                CarriedEnemyMotionView motion, CarriedEnemyThreatView threat, RegularEnemyEncounterRoleState[] roles, bool playerAlive)
            { Tick = tick; PlayerSnapshot = playerSnapshot; PlayerNextExpectedTick = playerNextExpectedTick; Transfer = transfer; Combat = combat; Reaction = reaction; Behavior = behavior; Motion = motion; Threat = threat; Roles = (RegularEnemyEncounterRoleState[])roles.Clone(); PlayerAlive = playerAlive; }
            internal SimulationTick Tick { get; } internal CombatSimulationOutcome Combat { get; } internal CarriedReactionView Reaction { get; }
            internal PlayerMotionSnapshot PlayerSnapshot { get; } internal SimulationTick PlayerNextExpectedTick { get; } internal TransferPhasePublication Transfer { get; }
            internal CarriedBehaviorView Behavior { get; } internal CarriedEnemyMotionView Motion { get; } internal CarriedEnemyThreatView Threat { get; }
            internal RegularEnemyEncounterRoleState[] Roles { get; } internal bool PlayerAlive { get; }
        }

        // REQ-COM-001, REQ-COM-004. Captured once from the exact serialized
        // collider/body objects and revalidated before every lifecycle preview and
        // final commit; no mutable geometry is allowed to become presentation input.
        private readonly struct EncounterColliderState
        {
            private EncounterColliderState(Collider2D player, Collider2D surveyor, Collider2D walker,
                Rigidbody2D playerBody, Rigidbody2D surveyorBody, Rigidbody2D walkerBody)
            {
                Player = player; Surveyor = surveyor; Walker = walker; PlayerBody = playerBody; SurveyorBody = surveyorBody; WalkerBody = walkerBody;
                PlayerTransform = player.transform; SurveyorTransform = surveyor.transform; WalkerTransform = walker.transform;
                PlayerSize = ((CapsuleCollider2D)player).size; SurveyorSize = ((BoxCollider2D)surveyor).size; WalkerSize = ((BoxCollider2D)walker).size;
                PlayerOffset = player.offset; SurveyorOffset = surveyor.offset; WalkerOffset = walker.offset;
                PlayerScale = PlayerTransform.lossyScale; SurveyorScale = SurveyorTransform.lossyScale; WalkerScale = WalkerTransform.lossyScale;
                PlayerRotation = PlayerTransform.rotation; SurveyorRotation = SurveyorTransform.rotation; WalkerRotation = WalkerTransform.rotation;
                PlayerBodyState = new BodyState(playerBody); SurveyorBodyState = new BodyState(surveyorBody); WalkerBodyState = new BodyState(walkerBody);
            }
            private Collider2D Player { get; } private Collider2D Surveyor { get; } private Collider2D Walker { get; }
            private Rigidbody2D PlayerBody { get; } private Rigidbody2D SurveyorBody { get; } private Rigidbody2D WalkerBody { get; }
            private Transform PlayerTransform { get; } private Transform SurveyorTransform { get; } private Transform WalkerTransform { get; }
            private Vector2 PlayerSize { get; } private Vector2 SurveyorSize { get; } private Vector2 WalkerSize { get; }
            private Vector2 PlayerOffset { get; } private Vector2 SurveyorOffset { get; } private Vector2 WalkerOffset { get; }
            private Vector3 PlayerScale { get; } private Vector3 SurveyorScale { get; } private Vector3 WalkerScale { get; }
            private Quaternion PlayerRotation { get; } private Quaternion SurveyorRotation { get; } private Quaternion WalkerRotation { get; }
            private BodyState PlayerBodyState { get; } private BodyState SurveyorBodyState { get; } private BodyState WalkerBodyState { get; }
            internal static EncounterColliderState Capture(CapsuleCollider2D player, BoxCollider2D surveyor, BoxCollider2D walker)
            {
                var playerBody = player.attachedRigidbody; var surveyorBody = surveyor.attachedRigidbody; var walkerBody = walker.attachedRigidbody;
                if (playerBody == null || surveyorBody == null || walkerBody == null || player.direction != CapsuleDirection2D.Vertical ||
                    !player.enabled || !surveyor.enabled || !walker.enabled || player.isTrigger || surveyor.isTrigger || walker.isTrigger)
                    throw new InvalidOperationException("Encounter handoff collider/body profile is invalid.");
                return new EncounterColliderState(player, surveyor, walker, playerBody, surveyorBody, walkerBody);
            }
            public override bool Equals(object value)
            {
                if (!(value is EncounterColliderState other)) return false;
                return ReferenceEquals(Player, other.Player) && ReferenceEquals(Surveyor, other.Surveyor) && ReferenceEquals(Walker, other.Walker) &&
                    ReferenceEquals(PlayerBody, other.PlayerBody) && ReferenceEquals(SurveyorBody, other.SurveyorBody) && ReferenceEquals(WalkerBody, other.WalkerBody) &&
                    ReferenceEquals(PlayerTransform, other.PlayerTransform) && ReferenceEquals(SurveyorTransform, other.SurveyorTransform) && ReferenceEquals(WalkerTransform, other.WalkerTransform) &&
                    PlayerSize == other.PlayerSize && SurveyorSize == other.SurveyorSize && WalkerSize == other.WalkerSize &&
                    PlayerOffset == other.PlayerOffset && SurveyorOffset == other.SurveyorOffset && WalkerOffset == other.WalkerOffset &&
                    PlayerScale == other.PlayerScale && SurveyorScale == other.SurveyorScale && WalkerScale == other.WalkerScale &&
                    PlayerRotation == other.PlayerRotation && SurveyorRotation == other.SurveyorRotation && WalkerRotation == other.WalkerRotation &&
                    PlayerBodyState.Equals(other.PlayerBodyState) && SurveyorBodyState.Equals(other.SurveyorBodyState) && WalkerBodyState.Equals(other.WalkerBodyState);
            }
            public override int GetHashCode() => 0;
            private readonly struct BodyState
            {
                internal BodyState(Rigidbody2D body) { Type = body.bodyType; Simulated = body.simulated; Constraints = body.constraints; Gravity = body.gravityScale; Interpolation = body.interpolation; FullKinematic = body.useFullKinematicContacts; }
                private RigidbodyType2D Type { get; } private bool Simulated { get; } private RigidbodyConstraints2D Constraints { get; } private float Gravity { get; } private RigidbodyInterpolation2D Interpolation { get; } private bool FullKinematic { get; }
                public override bool Equals(object value) => value is BodyState other && Type == other.Type && Simulated == other.Simulated && Constraints == other.Constraints && Gravity == other.Gravity && Interpolation == other.Interpolation && FullKinematic == other.FullKinematic;
                public override int GetHashCode() => 0;
            }
        }
    }

    // REQ-COM-001, REQ-COM-002. Immutable presentation-only M3E1 handoff. It
    // contains no Unity object or mutable source collection.
    internal readonly struct RegularEnemyEncounterPresentationView
    {
        internal RegularEnemyEncounterPresentationView(SimulationTick tick, SimulationTick nextExpectedTick, bool playerAlive,
            CarriedReactionView reaction, CarriedBehaviorView behavior, CarriedEnemyThreatView threat, RegularEnemyEncounterLifecyclePreview lifecycle)
        {
            Tick = tick; NextExpectedTick = nextExpectedTick; PlayerAlive = playerAlive; WalkerAlive = reaction.Walker.IsAlive; SurveyorAlive = reaction.Surveyor.IsAlive;
            WalkerReaction = reaction.Walker; SurveyorReaction = reaction.Surveyor; WalkerBehavior = behavior.Snapshot.Walker; SurveyorBehavior = behavior.Snapshot.Surveyor;
            HasActiveDenialLine = threat.Snapshot.HasActiveLine; DenialLineCenterXQ4096 = threat.Snapshot.LineCenterXQ4096;
            DenialLineExclusiveEndTick = threat.Snapshot.LineExclusiveEndTick; AppliedHitTargetIds = Copy(lifecycle.AppliedHitTargetIds);
            Defeats = Copy(lifecycle.Defeats); Rewards = Copy(lifecycle.Rewards); Cleared = lifecycle.Cleared; Completion = lifecycle.Completion;
        }
        internal SimulationTick Tick { get; } internal SimulationTick NextExpectedTick { get; } internal bool PlayerAlive { get; }
        internal bool WalkerAlive { get; } internal bool SurveyorAlive { get; } internal RegularEnemyReactionSnapshot WalkerReaction { get; }
        internal RegularEnemyReactionSnapshot SurveyorReaction { get; } internal WalkerBehaviorSnapshot WalkerBehavior { get; }
        internal SurveyorBehaviorSnapshot SurveyorBehavior { get; } internal bool HasActiveDenialLine { get; }
        internal int DenialLineCenterXQ4096 { get; } internal int DenialLineExclusiveEndTick { get; }
        internal ReadOnlyCollection<string> AppliedHitTargetIds { get; } internal ReadOnlyCollection<RegularEnemyDefeatedNotice> Defeats { get; }
        internal ReadOnlyCollection<RegularEnemyRewardRequest> Rewards { get; } internal RegularEnemyEncounterClearedNotice? Cleared { get; }
        internal RegularEnemyRoomCompletionRequest? Completion { get; }
        private static ReadOnlyCollection<T> Copy<T>(IReadOnlyList<T> source)
        {
            var copy = new T[source.Count];
            for (var i = 0; i < copy.Length; i++) copy[i] = source[i];
            return new ReadOnlyCollection<T>(copy);
        }
    }
}
