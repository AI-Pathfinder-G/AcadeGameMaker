using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-001, REQ-COM-002, REQ-COM-004, REQ-COM-005; affected REQ-MOV-001, REQ-WT-005.
    // The +100 phase reads completed logical publications only and delivers M3D1 requests at t+1.
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    internal sealed class RegularEnemyThreatSimulationDriver : MonoBehaviour
    {
        private const string PlayerId = "player";
        private const string WalkerId = "walker";
        private const string SurveyorId = "surveyor";
        private const int DenialLineHalfWidthQ4096 = 1024;

        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private CombatSimulationDriver _combatDriver;
        [SerializeField] private RegularEnemyBehaviorSimulationDriver _behaviorDriver;
        [SerializeField] private RegularEnemyLocomotionSimulationDriver _locomotionDriver;

        private RegularEnemyThreatSession _session;
        private CarriedEnemyThreatView? _carried;
        private FrozenThreatGeometry _geometry;
        private bool _registered;

        internal CarriedEnemyThreatView? LatestCarriedViewForTests => _carried;
        // REQ-COM-001, REQ-COM-004. Immutable completed +100 phase read for M3E1.
        internal CarriedEnemyThreatView? LatestCarriedViewForEncounterHandoff => _carried;
        internal SimulationTick NextExpectedTickForTests => _session == null ? default : _session.NextExpectedTick;
        internal bool HasSessionForTests => _session != null;

        private void Awake() => InitializeAndRegister();
        private void FixedUpdate() => AdvanceThreatPhase();

        internal void ConfigureForTests(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyBehaviorSimulationDriver behaviorDriver, RegularEnemyLocomotionSimulationDriver locomotionDriver)
        {
            if (_registered) throw new InvalidOperationException("Threat driver cannot be reconfigured after registration.");
            _player = player;
            _combatDriver = combatDriver;
            _behaviorDriver = behaviorDriver;
            _locomotionDriver = locomotionDriver;
            InitializeAndRegister();
        }

        internal CarriedEnemyThreatView AdvanceThreatPhaseForTests() => AdvanceThreatPhase();

        // REQ-COM-001, REQ-COM-004. B0 authoring validates the exact explicit graph
        // without registering the delivery lane or creating the M3D1 session.
        internal void ValidateEncounterGraph(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyBehaviorSimulationDriver behaviorDriver, RegularEnemyLocomotionSimulationDriver locomotionDriver)
        {
            if (_player != player || _combatDriver != combatDriver || _behaviorDriver != behaviorDriver ||
                _locomotionDriver != locomotionDriver)
                throw new InvalidOperationException("Threat encounter graph references are cross-wired.");
            locomotionDriver.ValidateThreatGraph(player, combatDriver, behaviorDriver);
        }

        // REQ-COM-001. Editor-only configuration intentionally does not initialize or
        // register the threat lane; Awake remains the sole production registration point.
        internal void ConfigureForAuthoring(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyBehaviorSimulationDriver behaviorDriver, RegularEnemyLocomotionSimulationDriver locomotionDriver)
        {
            if (_registered) throw new InvalidOperationException("Threat driver cannot be authored after registration.");
            _player = player;
            _combatDriver = combatDriver;
            _behaviorDriver = behaviorDriver;
            _locomotionDriver = locomotionDriver;
        }

        private void InitializeAndRegister()
        {
            if (_registered) return;
            ValidateBindings();
            _locomotionDriver.ValidateThreatGraph(_player, _combatDriver, _behaviorDriver);
            var firstSourceTick = _player.NextExpectedTick;
            var roster = _combatDriver.PrepareThreatDeliveryRegistration();
            var geometry = ValidateAndFreezeGeometry(roster);
            _combatDriver.RegisterThreatDeliveryLane(firstSourceTick);
            _geometry = geometry;
            _registered = true;
        }

        private CarriedEnemyThreatView AdvanceThreatPhase()
        {
            if (!_registered) throw new InvalidOperationException("Threat driver must register its Combat lane in Awake.");
            ValidateBindings();
            var tick = _player.Snapshot.Tick;
            if (_player.NextExpectedTick.Value != checked(tick.Value + 1))
                throw new InvalidOperationException("Threat phase requires the exact completed player movement tick.");

            var sources = ValidateAndCaptureSources(tick);
            RevalidateGeometry(_geometry);
            var input = SelectInput(tick, sources);

            if (_session == null)
            {
                var staged = new RegularEnemyThreatSession(tick, _geometry.PlayerHalfWidth, _geometry.PlayerHalfHeight,
                    _geometry.WalkerHalfWidth, _geometry.WalkerHalfHeight, DenialLineHalfWidthQ4096);
                var candidate = staged.PreviewNext(input);
                var delivery = _combatDriver.PreflightThreatDelivery(tick, candidate.CopyRequests());
                RevalidateTransaction(tick, sources);
                var committed = staged.CommitNext(input, candidate);
                _combatDriver.CommitPreflightedThreatDelivery(delivery);
                _session = staged;
                _carried = new CarriedEnemyThreatView(tick, committed);
                return _carried.Value;
            }

            if (_session.NextExpectedTick != tick || !_session.LatestSnapshot.HasValue || !_carried.HasValue ||
                _carried.Value.Tick != _session.LatestSnapshot.Value.Tick || !_carried.Value.Snapshot.Equals(_session.LatestSnapshot.Value))
                throw new InvalidOperationException("Threat session and carried view must remain exactly aligned.");
            var nextCandidate = _session.PreviewNext(input);
            var nextDelivery = _combatDriver.PreflightThreatDelivery(tick, nextCandidate.CopyRequests());
            RevalidateTransaction(tick, sources);
            var nextCommitted = _session.CommitNext(input, nextCandidate);
            _combatDriver.CommitPreflightedThreatDelivery(nextDelivery);
            _carried = new CarriedEnemyThreatView(tick, nextCommitted);
            return _carried.Value;
        }

        private ThreatSources ValidateAndCaptureSources(SimulationTick tick)
        {
            if (_player.Snapshot.Tick != tick || _player.NextExpectedTick.Value != checked(tick.Value + 1))
                throw new InvalidOperationException("Threat phase requires the exact completed player movement publication.");
            if (!_combatDriver.HasLatestOutcomeForThreatBridge)
                throw new InvalidOperationException("Threat phase requires a successful current Combat publication.");
            var combat = _combatDriver.LatestOutcomeForThreatBridge;
            var behaviorValue = _behaviorDriver.LatestCarriedViewForThreatBridge;
            var motionValue = _locomotionDriver.LatestCarriedViewForM3D;
            if (combat.Tick != tick || !behaviorValue.HasValue || behaviorValue.Value.Tick != tick ||
                behaviorValue.Value.Snapshot.Tick != tick || behaviorValue.Value.Snapshot.NextExpectedTick.Value != checked(tick.Value + 1) ||
                !motionValue.HasValue || motionValue.Value.Tick != tick || motionValue.Value.Snapshot.Tick != tick ||
                motionValue.Value.Snapshot.NextExpectedTick.Value != checked(tick.Value + 1))
                throw new InvalidOperationException("Threat phase requires exact current Combat, Behavior, and Motion publications.");

            var player = ExactCombatant(combat, PlayerId);
            var walker = ExactCombatant(combat, WalkerId);
            var surveyor = ExactCombatant(combat, SurveyorId);
            var behavior = behaviorValue.Value.Snapshot;
            var motion = motionValue.Value.Snapshot;
            if ((!walker.IsDead) != behavior.Walker.IsAlive || (!surveyor.IsDead) != behavior.Surveyor.IsAlive ||
                motion.Walker.Dead == behavior.Walker.IsAlive || motion.Surveyor.Dead == behavior.Surveyor.IsAlive)
                throw new InvalidOperationException("Combat, Behavior, and Motion alive states disagree.");
            return new ThreatSources(_player.Snapshot, combat, behaviorValue.Value, motionValue.Value,
                !player.IsDead, !walker.IsDead, !surveyor.IsDead);
        }

        private void RevalidateTransaction(SimulationTick tick, ThreatSources expected)
        {
            ValidateBindings();
            _locomotionDriver.ValidateThreatGraph(_player, _combatDriver, _behaviorDriver);
            RevalidateGeometry(_geometry);
            var actual = ValidateAndCaptureSources(tick);
            if (!SourcesEqual(actual, expected))
                throw new InvalidOperationException("Threat upstream publication changed during pair preflight.");
        }

        private static bool SourcesEqual(ThreatSources left, ThreatSources right) =>
            left.PlayerMotion.Equals(right.PlayerMotion) && left.Combat.Equals(right.Combat) &&
            left.Behavior.Equals(right.Behavior) && left.Motion.Equals(right.Motion) &&
            left.PlayerAlive == right.PlayerAlive && left.WalkerAlive == right.WalkerAlive &&
            left.SurveyorAlive == right.SurveyorAlive;

        private static RegularEnemyThreatInput SelectInput(SimulationTick tick, ThreatSources sources)
        {
            var behavior = sources.Behavior.Snapshot;
            var motion = sources.Motion.Snapshot;
            var walkerReset = behavior.Walker.Phase == WalkerBehaviorPhase.Reset;
            var surveyorReset = behavior.Surveyor.Phase == SurveyorBehaviorPhase.Reset;
            var anyReset = sources.Combat.EncounterReset || walkerReset || surveyorReset;
            if (anyReset && !(sources.Combat.EncounterReset && walkerReset && surveyorReset))
                throw new InvalidOperationException("Threat reset requires exact paired reset publications.");
            return anyReset
                ? RegularEnemyThreatInput.EncounterReset(tick, sources.PlayerAlive, behavior, motion)
                : RegularEnemyThreatInput.Gameplay(tick, sources.PlayerAlive,
                    new PositionQ4096(sources.PlayerMotion.PositionXQ4096, sources.PlayerMotion.PositionYQ4096), behavior, motion);
        }

        private void ValidateBindings()
        {
            if (_player == null || _combatDriver == null || _behaviorDriver == null || _locomotionDriver == null)
                throw new InvalidOperationException("Threat driver requires explicit player, Combat, Behavior, and Locomotion bindings.");
        }

        private FrozenThreatGeometry ValidateAndFreezeGeometry(IReadOnlyList<FrozenCombatTarget> roster)
        {
            FrozenCombatTarget? player = null;
            FrozenCombatTarget? walker = null;
            FrozenCombatTarget? surveyor = null;
            string previous = null;
            for (var index = 0; index < roster.Count; index++)
            {
                var target = roster[index];
                if (previous != null && string.CompareOrdinal(previous, target.TargetId) >= 0)
                    throw new InvalidOperationException("Threat bridge requires the Combat roster to remain ordinal sorted.");
                previous = target.TargetId;
                if (target.TargetId == PlayerId) { if (player.HasValue) throw new InvalidOperationException("Duplicate player role."); player = target; }
                if (target.TargetId == WalkerId) { if (walker.HasValue) throw new InvalidOperationException("Duplicate walker role."); walker = target; }
                if (target.TargetId == SurveyorId) { if (surveyor.HasValue) throw new InvalidOperationException("Duplicate surveyor role."); surveyor = target; }
            }
            if (!player.HasValue || !walker.HasValue || !surveyor.HasValue)
                throw new InvalidOperationException("Threat bridge requires literal player, walker, and surveyor roles.");
            if (player.Value.MaxHealth != 5 || player.Value.InvulnerabilityTicks != 45 || player.Value.CanReceivePlayerBasicAttack || player.Value.HasMatchingTransferTarget ||
                walker.Value.MaxHealth != 9 || walker.Value.InvulnerabilityTicks != 0 || !walker.Value.CanReceivePlayerBasicAttack || !walker.Value.HasMatchingTransferTarget ||
                surveyor.Value.MaxHealth != 6 || surveyor.Value.InvulnerabilityTicks != 0 || !surveyor.Value.CanReceivePlayerBasicAttack || !surveyor.Value.HasMatchingTransferTarget ||
                surveyor.Value.TargetCollider == null || surveyor.Value.TargetPoseRoot == null)
                throw new InvalidOperationException("Threat roster scalar roles disagree with the frozen Combat identities.");

            var playerShape = ValidatePlayerGeometry(player.Value);
            var walkerShape = ValidateWalkerGeometry(walker.Value);
            return new FrozenThreatGeometry(player.Value, walker.Value, surveyor.Value, playerShape.collider, playerShape.body,
                walkerShape.collider, walkerShape.body, playerShape.halfWidth, playerShape.halfHeight,
                walkerShape.halfWidth, walkerShape.halfHeight);
        }

        private (CapsuleCollider2D collider, Rigidbody2D body, int halfWidth, int halfHeight) ValidatePlayerGeometry(FrozenCombatTarget target)
        {
            if (!(target.TargetCollider is CapsuleCollider2D capsule) || target.TargetPoseRoot == null ||
                target.TargetPoseRoot != _player.transform || capsule.transform != _player.transform || capsule.attachedRigidbody == null ||
                capsule.attachedRigidbody.transform != _player.transform || !target.TargetPoseRoot.gameObject.activeInHierarchy)
                throw new InvalidOperationException("Player controller, frozen root, body, and capsule must be one active Transform.");
            ValidatePlayerShape(capsule, capsule.attachedRigidbody, target.TargetPoseRoot);
            ValidateSoleSolid(target.TargetPoseRoot, capsule);
            return (capsule, capsule.attachedRigidbody, QuantizePositiveHalf(capsule.size.x), QuantizePositiveHalf(capsule.size.y));
        }

        private static (BoxCollider2D collider, Rigidbody2D body, int halfWidth, int halfHeight) ValidateWalkerGeometry(FrozenCombatTarget target)
        {
            if (!(target.TargetCollider is BoxCollider2D box) || target.TargetPoseRoot == null || box.attachedRigidbody == null ||
                box.transform != target.TargetPoseRoot || box.attachedRigidbody.transform != target.TargetPoseRoot ||
                !target.TargetPoseRoot.gameObject.activeInHierarchy)
                throw new InvalidOperationException("Walker frozen root, body, and box must be one active Transform.");
            ValidateWalkerShape(box, box.attachedRigidbody, target.TargetPoseRoot);
            ValidateSoleSolid(target.TargetPoseRoot, box);
            return (box, box.attachedRigidbody, QuantizePositiveHalf(box.size.x), QuantizePositiveHalf(box.size.y));
        }

        private static void ValidatePlayerShape(CapsuleCollider2D capsule, Rigidbody2D body, Transform root)
        {
            if (!capsule.enabled || capsule.isTrigger || capsule.direction != CapsuleDirection2D.Vertical || capsule.offset != Vector2.zero ||
                !body.simulated || body.bodyType != RigidbodyType2D.Kinematic || root.lossyScale.x != 1f || root.lossyScale.y != 1f ||
                root.eulerAngles.z != 0f || !FinitePositive(capsule.size.x) || !FinitePositive(capsule.size.y))
                throw new InvalidOperationException("Player threat capsule authoring is not the frozen exact form.");
        }

        private static void ValidateWalkerShape(BoxCollider2D box, Rigidbody2D body, Transform root)
        {
            if (!box.enabled || box.isTrigger || !body.simulated || body.bodyType != RigidbodyType2D.Kinematic ||
                body.constraints != RigidbodyConstraints2D.FreezeRotation || body.interpolation != RigidbodyInterpolation2D.None ||
                body.useFullKinematicContacts || body.rotation != 0f || body.angularVelocity != 0f || body.linearVelocity != Vector2.zero ||
                root.lossyScale.x != 1f || root.lossyScale.y != 1f || root.eulerAngles.z != 0f || box.offset != Vector2.zero ||
                box.edgeRadius != 0f || !FinitePositive(box.size.x) || !FinitePositive(box.size.y))
                throw new InvalidOperationException("Walker threat box authoring is not the frozen M3C2 form.");
        }

        private static void ValidateSoleSolid(Transform root, Collider2D expected)
        {
            var values = root.GetComponentsInChildren<Collider2D>(true);
            var count = 0;
            for (var index = 0; index < values.Length; index++)
            {
                if (!values[index].enabled || values[index].isTrigger) continue;
                count++;
                if (values[index] != expected) throw new InvalidOperationException("Threat geometry has an extra enabled solid collider.");
            }
            if (count != 1) throw new InvalidOperationException("Threat geometry requires exactly one enabled solid collider.");
        }

        private void RevalidateGeometry(FrozenThreatGeometry value)
        {
            var current = ValidateAndFreezeGeometry(_combatDriver.FrozenTargetsForThreatBridge);
            if (!GeometryBindingsEqual(value, current))
                throw new InvalidOperationException("Threat frozen geometry drifted after registration.");
        }

        private static bool GeometryBindingsEqual(FrozenThreatGeometry left, FrozenThreatGeometry right) =>
            left.Player.TargetId == right.Player.TargetId && left.Player.MaxHealth == right.Player.MaxHealth &&
            left.Player.InvulnerabilityTicks == right.Player.InvulnerabilityTicks && left.Player.CanReceivePlayerBasicAttack == right.Player.CanReceivePlayerBasicAttack &&
            left.Player.HasMatchingTransferTarget == right.Player.HasMatchingTransferTarget &&
            left.Walker.TargetId == right.Walker.TargetId && left.Walker.MaxHealth == right.Walker.MaxHealth &&
            left.Walker.InvulnerabilityTicks == right.Walker.InvulnerabilityTicks && left.Walker.CanReceivePlayerBasicAttack == right.Walker.CanReceivePlayerBasicAttack &&
            left.Walker.HasMatchingTransferTarget == right.Walker.HasMatchingTransferTarget &&
            left.Surveyor.TargetId == right.Surveyor.TargetId && left.Surveyor.MaxHealth == right.Surveyor.MaxHealth &&
            left.Surveyor.InvulnerabilityTicks == right.Surveyor.InvulnerabilityTicks && left.Surveyor.CanReceivePlayerBasicAttack == right.Surveyor.CanReceivePlayerBasicAttack &&
            left.Surveyor.HasMatchingTransferTarget == right.Surveyor.HasMatchingTransferTarget &&
            left.Surveyor.TargetCollider == right.Surveyor.TargetCollider && left.Surveyor.TargetPoseRoot == right.Surveyor.TargetPoseRoot &&
            left.PlayerCollider == right.PlayerCollider && left.PlayerBody == right.PlayerBody &&
            left.WalkerCollider == right.WalkerCollider && left.WalkerBody == right.WalkerBody &&
            left.PlayerHalfWidth == right.PlayerHalfWidth && left.PlayerHalfHeight == right.PlayerHalfHeight &&
            left.WalkerHalfWidth == right.WalkerHalfWidth && left.WalkerHalfHeight == right.WalkerHalfHeight;

        private static CombatantSnapshot ExactCombatant(CombatSimulationOutcome outcome, string id)
        {
            CombatantSnapshot? found = null;
            for (var index = 0; index < outcome.Combatants.Count; index++)
            {
                if (!string.Equals(outcome.Combatants[index].TargetId, id, StringComparison.Ordinal)) continue;
                if (found.HasValue) throw new InvalidOperationException("Combat publication has a duplicate threat role.");
                found = outcome.Combatants[index];
            }
            return found ?? throw new InvalidOperationException("Combat publication omits a threat role.");
        }

        private static bool FinitePositive(float value) => !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f;
        private static int QuantizePositiveHalf(float size)
        {
            if (!FinitePositive(size)) throw new ArgumentOutOfRangeException(nameof(size));
            var value = checked((int)Math.Round(size * 2048d, MidpointRounding.AwayFromZero));
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            return value;
        }

        private readonly struct ThreatSources
        {
            internal ThreatSources(PlayerMotionSnapshot playerMotion, CombatSimulationOutcome combat, CarriedBehaviorView behavior,
                CarriedEnemyMotionView motion, bool playerAlive, bool walkerAlive, bool surveyorAlive)
            { PlayerMotion = playerMotion; Combat = combat; Behavior = behavior; Motion = motion; PlayerAlive = playerAlive; WalkerAlive = walkerAlive; SurveyorAlive = surveyorAlive; }
            internal PlayerMotionSnapshot PlayerMotion { get; }
            internal CombatSimulationOutcome Combat { get; }
            internal CarriedBehaviorView Behavior { get; }
            internal CarriedEnemyMotionView Motion { get; }
            internal bool PlayerAlive { get; }
            internal bool WalkerAlive { get; }
            internal bool SurveyorAlive { get; }
        }

        private readonly struct FrozenThreatGeometry
        {
            internal FrozenThreatGeometry(FrozenCombatTarget player, FrozenCombatTarget walker, FrozenCombatTarget surveyor,
                CapsuleCollider2D playerCollider, Rigidbody2D playerBody, BoxCollider2D walkerCollider, Rigidbody2D walkerBody,
                int playerHalfWidth, int playerHalfHeight, int walkerHalfWidth, int walkerHalfHeight)
            { Player = player; Walker = walker; Surveyor = surveyor; PlayerCollider = playerCollider; PlayerBody = playerBody; WalkerCollider = walkerCollider; WalkerBody = walkerBody; PlayerHalfWidth = playerHalfWidth; PlayerHalfHeight = playerHalfHeight; WalkerHalfWidth = walkerHalfWidth; WalkerHalfHeight = walkerHalfHeight; }
            internal FrozenCombatTarget Player { get; }
            internal FrozenCombatTarget Walker { get; }
            internal FrozenCombatTarget Surveyor { get; }
            internal CapsuleCollider2D PlayerCollider { get; }
            internal Rigidbody2D PlayerBody { get; }
            internal BoxCollider2D WalkerCollider { get; }
            internal Rigidbody2D WalkerBody { get; }
            internal int PlayerHalfWidth { get; }
            internal int PlayerHalfHeight { get; }
            internal int WalkerHalfWidth { get; }
            internal int WalkerHalfHeight { get; }
        }
    }

    // REQ-COM-004: immutable value-only +100 publication; no Unity or request-queue object escapes.
    internal readonly struct CarriedEnemyThreatView
    {
        internal CarriedEnemyThreatView(SimulationTick tick, RegularEnemyThreatSnapshot snapshot)
        {
            if (snapshot.Tick != tick || snapshot.NextExpectedTick.Value != checked(tick.Value + 1))
                throw new ArgumentException("Enemy threat view must be exact and consecutive.");
            Tick = tick;
            Snapshot = snapshot;
        }
        internal SimulationTick Tick { get; }
        internal RegularEnemyThreatSnapshot Snapshot { get; }
    }
}
