using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-001, REQ-COM-002, REQ-COM-004, REQ-COM-005; affected REQ-WT-003, REQ-WT-005, REQ-WT-007.
    // The -160 bridge owns explicit-position environment casts and the M3C1 Unity handoff only.
    [DefaultExecutionOrder(-160)]
    [DisallowMultipleComponent]
    internal sealed class RegularEnemyLocomotionSimulationDriver : MonoBehaviour
    {
        private const string WalkerId = "walker";
        private const string SurveyorId = "surveyor";
        private const int SkinQ4096 = 82;
        private const int SupportProbeQ4096 = 205;
        private const int BlockingNormalQ4096 = 2867;
        private const int HitBufferCapacity = 64;
        private const float UnitsPerWorldUnit = 4096f;

        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private CombatSimulationDriver _combatDriver;
        [SerializeField] private RegularEnemyReactionSimulationDriver _reactionDriver;
        [SerializeField] private RegularEnemyBehaviorSimulationDriver _behaviorDriver;

        private readonly RaycastHit2D[] _hits = new RaycastHit2D[HitBufferCapacity];
        private RegularEnemyLocomotionSession _session;
        private CarriedEnemyMotionView? _carried;
        private FrozenLocomotionRole _walker;
        private FrozenLocomotionRole _surveyor;
        private bool _initialized;
        private string _mutationRoleForTests;
        private LocomotionHitMutationForTests _hitMutationForTests;
        private int _queryCountForTests;
        private int _walkerQueryCountForTests;
        private int _surveyorQueryCountForTests;
        private int _walkerMoveCountForTests;
        private int _surveyorMoveCountForTests;
        private PositionQ4096 _lastWalkerVerticalOriginForTests;
        private string _lastSelectedColliderKeyForTests;

        internal CarriedEnemyMotionView? LatestCarriedViewForTests => _carried;
        internal CarriedEnemyMotionView? LatestCarriedViewForM3D => _carried;
        // REQ-COM-001, REQ-COM-004. Immutable completed -160 phase read for M3E1.
        internal CarriedEnemyMotionView? LatestCarriedViewForEncounterHandoff => _carried;
        internal SimulationTick NextExpectedTickForTests => _session == null ? default : _session.NextExpectedTick;
        internal bool HasSessionForTests => _session != null;
        internal int QueryCountForTests => _queryCountForTests;
        internal int WalkerQueryCountForTests => _walkerQueryCountForTests;
        internal int SurveyorQueryCountForTests => _surveyorQueryCountForTests;
        internal int WalkerMoveCountForTests => _walkerMoveCountForTests;
        internal int SurveyorMoveCountForTests => _surveyorMoveCountForTests;
        internal PositionQ4096 LastWalkerVerticalOriginForTests => _lastWalkerVerticalOriginForTests;
        internal string LastSelectedColliderKeyForTests => _lastSelectedColliderKeyForTests ?? string.Empty;

        private void FixedUpdate() => AdvanceLocomotionPhase();

        // REQ-COM-001, REQ-COM-004. Noninitializing exact authored graph binding.
        internal void ConfigureForAuthoring(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyReactionSimulationDriver reactionDriver, RegularEnemyBehaviorSimulationDriver behaviorDriver) =>
            Configure(player, combatDriver, reactionDriver, behaviorDriver);
        internal void ConfigureForTests(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyReactionSimulationDriver reactionDriver, RegularEnemyBehaviorSimulationDriver behaviorDriver) =>
            Configure(player, combatDriver, reactionDriver, behaviorDriver);

        // REQ-COM-001, REQ-COM-004. Exact read-only graph seam for the +110
        // handoff; it does not query, move a body, or initialize locomotion.
        internal void ValidateEncounterGraph(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyReactionSimulationDriver reactionDriver, RegularEnemyBehaviorSimulationDriver behaviorDriver)
        {
            if (_player != player || _combatDriver != combatDriver || _reactionDriver != reactionDriver || _behaviorDriver != behaviorDriver)
                throw new InvalidOperationException("Locomotion encounter graph references are cross-wired.");
        }

        private void Configure(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyReactionSimulationDriver reactionDriver, RegularEnemyBehaviorSimulationDriver behaviorDriver)
        {
            if (_initialized) throw new InvalidOperationException("Locomotion driver cannot be reconfigured after initialization.");
            _player = player;
            _combatDriver = combatDriver;
            _reactionDriver = reactionDriver;
            _behaviorDriver = behaviorDriver;
        }

        internal CarriedEnemyMotionView AdvanceLocomotionPhaseForTests() => AdvanceLocomotionPhase();

        internal void ValidateThreatGraph(PlayerMovementController player, CombatSimulationDriver combatDriver,
            RegularEnemyBehaviorSimulationDriver behaviorDriver)
        {
            if (_player != player || _combatDriver != combatDriver || _behaviorDriver != behaviorDriver)
                throw new InvalidOperationException("Locomotion threat graph references are cross-wired.");
            behaviorDriver.ValidateThreatGraph(player, combatDriver, _reactionDriver);
        }
        internal RegularEnemyCollisionPairResolution ResolvePairForTests(RegularEnemyMotionPairPrediction prediction)
        {
            if (!_initialized) throw new InvalidOperationException("Synthetic collision resolution requires an initialized locomotion bridge.");
            return ResolvePair(prediction, new FrozenLocomotionRoles(_walker, _surveyor));
        }
        internal void MutateNextQueriesForTests(string roleId, LocomotionHitMutationForTests mutation)
        {
            if (roleId != WalkerId && roleId != SurveyorId) throw new ArgumentOutOfRangeException(nameof(roleId));
            _mutationRoleForTests = roleId;
            _hitMutationForTests = mutation;
        }
        internal static int QuantizePositiveSizeForTests(float value) => QuantizePositiveSize(value);
        internal static void ValidateZeroAngularVelocityForTests(float value) => ValidateZeroAngularVelocity(value);

        private CarriedEnemyMotionView AdvanceLocomotionPhase()
        {
            EnsureBindings();
            var tick = _player.NextExpectedTick;
            var roles = ValidateAndFreezeRoles(_combatDriver.FrozenTargetsForLocomotionBridge);
            var behavior = ExactBehaviorView(tick);
            var reaction = ExactReactionView(tick);
            ValidateUpstreamPair(behavior, reaction);
            var input = SelectInput(tick, behavior, reaction);

            if (_initialized)
            {
                ValidateRetainedRoles(roles);
                ValidatePriorAlignment(tick);
                return input.Kind == RegularEnemyLocomotionInputKind.EncounterReset
                    ? CommitReset(tick, input)
                    : CommitGameplay(tick, input);
            }

            // AC-COM-003, AC-WT-005: first use is staged entirely in locals. A failed seed
            // probe, preview, role resolution, or candidate validation leaves no owned session/view.
            if (input.Kind != RegularEnemyLocomotionInputKind.Gameplay)
                throw new InvalidOperationException("The first locomotion publication must be gameplay, not reset.");
            var walkerGrounded = ProbeSupport(roles.Walker, roles);
            var surveyorGrounded = ProbeSupport(roles.Surveyor, roles);
            var stagedSession = new RegularEnemyLocomotionSession(tick, roles.Walker.Position, walkerGrounded,
                roles.Surveyor.Position, surveyorGrounded);
            var prediction = stagedSession.PreviewNext(input);
            var resolution = ResolvePair(prediction, roles);
            var candidate = stagedSession.ValidateResolvedNext(input, resolution);
            RevalidateBeforeCommit(roles, null);
            var committed = stagedSession.CommitResolvedNext(input, resolution, candidate);

            // No ordinary rejection remains beyond this point. A Unity exception is fail-stop.
            MoveCommittedBodies(tick, committed, roles);
            _walker = roles.Walker;
            _surveyor = roles.Surveyor;
            _session = stagedSession;
            _initialized = true;
            _carried = new CarriedEnemyMotionView(tick, committed);
            return _carried.Value;
        }

        private CarriedEnemyMotionView CommitGameplay(SimulationTick tick, RegularEnemyLocomotionPairInput input)
        {
            var prediction = _session.PreviewNext(input);
            var resolution = ResolvePair(prediction, new FrozenLocomotionRoles(_walker, _surveyor));
            var candidate = _session.ValidateResolvedNext(input, resolution);
            RevalidateBeforeCommit(new FrozenLocomotionRoles(_walker, _surveyor), _session.LatestSnapshot);
            var committed = _session.CommitResolvedNext(input, resolution, candidate);
            MoveCommittedBodies(tick, committed, new FrozenLocomotionRoles(_walker, _surveyor));
            _carried = new CarriedEnemyMotionView(tick, committed);
            return _carried.Value;
        }

        private CarriedEnemyMotionView CommitReset(SimulationTick tick, RegularEnemyLocomotionPairInput input)
        {
            // REQ-COM-004: reset is query-free and restores the seeds frozen at first adoption.
            var candidate = _session.PreflightReset(input);
            RevalidateBeforeCommit(new FrozenLocomotionRoles(_walker, _surveyor), _session.LatestSnapshot);
            var committed = _session.CommitReset(input, candidate);
            MoveCommittedBodies(tick, committed, new FrozenLocomotionRoles(_walker, _surveyor));
            _carried = new CarriedEnemyMotionView(tick, committed);
            return _carried.Value;
        }

        private void EnsureBindings()
        {
            if (_player == null || _combatDriver == null || _reactionDriver == null || _behaviorDriver == null)
                throw new InvalidOperationException("Locomotion driver requires explicit player, combat, reaction, and behavior bindings.");
        }

        private CarriedBehaviorView ExactBehaviorView(SimulationTick tick)
        {
            var view = _behaviorDriver.LatestCarriedViewForLocomotionBridge;
            if (!view.HasValue || view.Value.Tick != tick || view.Value.Snapshot.Tick != tick ||
                view.Value.Snapshot.NextExpectedTick.Value != checked(tick.Value + 1))
                throw new InvalidOperationException("Locomotion requires the exact current behavior publication.");
            return view.Value;
        }

        private CarriedReactionView ExactReactionView(SimulationTick tick)
        {
            var view = _reactionDriver.LatestCarriedViewForBehaviorBridge;
            if (!view.HasValue || view.Value.Tick != tick || view.Value.Walker.Tick != tick || view.Value.Surveyor.Tick != tick ||
                view.Value.Walker.NextExpectedTick.Value != checked(tick.Value + 1) ||
                view.Value.Surveyor.NextExpectedTick.Value != checked(tick.Value + 1))
                throw new InvalidOperationException("Locomotion requires the exact current reaction publication.");
            return view.Value;
        }

        private static RegularEnemyLocomotionPairInput SelectInput(SimulationTick tick, CarriedBehaviorView behavior, CarriedReactionView reaction)
        {
            var walkerReset = behavior.Snapshot.Walker.Phase == WalkerBehaviorPhase.Reset;
            var surveyorReset = behavior.Snapshot.Surveyor.Phase == SurveyorBehaviorPhase.Reset;
            if (walkerReset != surveyorReset) throw new InvalidOperationException("Locomotion requires paired Reset phases.");
            return walkerReset
                ? RegularEnemyLocomotionPairInput.EncounterReset(tick, behavior.Snapshot, reaction.Walker, reaction.Surveyor)
                : RegularEnemyLocomotionPairInput.Gameplay(tick, behavior.Snapshot, reaction.Walker, reaction.Surveyor);
        }

        private static void ValidateUpstreamPair(CarriedBehaviorView behavior, CarriedReactionView reaction)
        {
            var walker = behavior.Snapshot.Walker;
            var surveyor = behavior.Snapshot.Surveyor;
            if (!string.Equals(reaction.Walker.TargetId, WalkerId, StringComparison.Ordinal) ||
                !string.Equals(reaction.Surveyor.TargetId, SurveyorId, StringComparison.Ordinal) ||
                walker.IsAlive != reaction.Walker.IsAlive || surveyor.IsAlive != reaction.Surveyor.IsAlive ||
                walker.Phase < WalkerBehaviorPhase.Approach || walker.Phase > WalkerBehaviorPhase.Reset ||
                surveyor.Phase < SurveyorBehaviorPhase.Relocate || surveyor.Phase > SurveyorBehaviorPhase.Reset ||
                walker.Movement < RegularEnemyMovementIntent.None || walker.Movement > RegularEnemyMovementIntent.Downward ||
                surveyor.Movement < RegularEnemyMovementIntent.None || surveyor.Movement > RegularEnemyMovementIntent.Downward)
                throw new InvalidOperationException("Locomotion requires role-aligned, valid-phase upstream publications before querying.");
        }

        private FrozenLocomotionRoles ValidateAndFreezeRoles(IReadOnlyList<FrozenCombatTarget> roster)
        {
            if (roster == null) throw new ArgumentNullException(nameof(roster));
            FrozenCombatTarget? walker = null;
            FrozenCombatTarget? surveyor = null;
            string previous = null;
            for (var index = 0; index < roster.Count; index++)
            {
                var target = roster[index];
                if (previous != null && string.CompareOrdinal(previous, target.TargetId) >= 0)
                    throw new InvalidOperationException("Locomotion requires the Combat roster to remain ordinal sorted.");
                previous = target.TargetId;
                if (target.TargetId == WalkerId) { if (walker.HasValue) throw new InvalidOperationException("Duplicate walker role."); walker = target; }
                if (target.TargetId == SurveyorId) { if (surveyor.HasValue) throw new InvalidOperationException("Duplicate surveyor role."); surveyor = target; }
            }
            if (!walker.HasValue || !surveyor.HasValue) throw new InvalidOperationException("Locomotion requires literal walker and surveyor roles.");
            return new FrozenLocomotionRoles(ValidateRole(walker.Value, WalkerId), ValidateRole(surveyor.Value, SurveyorId));
        }

        private static FrozenLocomotionRole ValidateRole(FrozenCombatTarget target, string expectedId)
        {
            if (!string.Equals(target.TargetId, expectedId, StringComparison.Ordinal) ||
                (expectedId == WalkerId && (target.MaxHealth != 9 || target.InvulnerabilityTicks != 0 || !target.CanReceivePlayerBasicAttack || !target.HasMatchingTransferTarget)) ||
                (expectedId == SurveyorId && (target.MaxHealth != 6 || target.InvulnerabilityTicks != 0 || !target.CanReceivePlayerBasicAttack || !target.HasMatchingTransferTarget)))
                throw new InvalidOperationException("Locomotion role disagrees with the frozen combat identity.");
            if (!(target.TargetCollider is BoxCollider2D box) || target.TargetPoseRoot == null)
                throw new InvalidOperationException("Locomotion roles require a frozen BoxCollider2D and pose root.");
            var root = target.TargetPoseRoot;
            var body = box.attachedRigidbody;
            if (body == null || body.transform != root || box.transform != root || !root.gameObject.activeInHierarchy)
                throw new InvalidOperationException("Locomotion body, box, and frozen pose root must be one active Transform.");
            ValidateBodyAndBox(root, body, box);
            var enabledSolids = root.GetComponentsInChildren<Collider2D>(true);
            var solidCount = 0;
            for (var index = 0; index < enabledSolids.Length; index++)
            {
                if (!enabledSolids[index].enabled || enabledSolids[index].isTrigger) continue;
                solidCount++;
                if (enabledSolids[index] != box) throw new InvalidOperationException("Locomotion role has an extra enabled solid collider.");
            }
            if (solidCount != 1) throw new InvalidOperationException("Locomotion role requires exactly one enabled solid collider.");
            return new FrozenLocomotionRole(expectedId, root, body, box, QuantizeBodyPosition(body.position),
                new PositionQ4096(QuantizePositiveSize(box.size.x), QuantizePositiveSize(box.size.y)));
        }

        private static void ValidateBodyAndBox(Transform root, Rigidbody2D body, BoxCollider2D box)
        {
            if (!box.enabled || box.isTrigger || !body.simulated || body.bodyType != RigidbodyType2D.Kinematic ||
                body.constraints != RigidbodyConstraints2D.FreezeRotation || body.interpolation != RigidbodyInterpolation2D.None ||
                body.useFullKinematicContacts || body.rotation != 0f || body.linearVelocity != Vector2.zero ||
                root.lossyScale.x != 1f || root.lossyScale.y != 1f || root.eulerAngles.z != 0f || box.offset != Vector2.zero || box.edgeRadius != 0f)
                throw new InvalidOperationException("Locomotion Rigidbody2D/BoxCollider2D authoring is not the frozen exact form.");
            ValidateZeroAngularVelocity(body.angularVelocity);
            _ = QuantizeBodyPosition(body.position);
            _ = QuantizePositiveSize(box.size.x);
            _ = QuantizePositiveSize(box.size.y);
        }

        private void ValidateRetainedRoles(FrozenLocomotionRoles roles)
        {
            if (!RoleBindingEquals(_walker, roles.Walker) || !RoleBindingEquals(_surveyor, roles.Surveyor))
                throw new InvalidOperationException("Locomotion frozen role bindings changed after initialization.");
        }

        private void ValidatePriorAlignment(SimulationTick tick)
        {
            if (_session.NextExpectedTick != tick || !_session.LatestSnapshot.HasValue || !_carried.HasValue ||
                _carried.Value.Tick != _session.LatestSnapshot.Value.Tick || !PairEquals(_carried.Value.Snapshot, _session.LatestSnapshot.Value))
                throw new InvalidOperationException("Locomotion session and carried view are not exactly aligned.");
            var prior = _session.LatestSnapshot.Value;
            if (!PositionEquals(QuantizeBodyPosition(_walker.Body.position), prior.Walker.Position) ||
                !PositionEquals(QuantizeBodyPosition(_surveyor.Body.position), prior.Surveyor.Position))
                throw new InvalidOperationException("Physics bodies did not consume the prior committed locomotion positions.");
        }

        private void RevalidateBeforeCommit(FrozenLocomotionRoles roles, RegularEnemyMotionPairSnapshot? expectedPrior)
        {
            var walker = ValidateRoleFromRetained(roles.Walker);
            var surveyor = ValidateRoleFromRetained(roles.Surveyor);
            var expectedWalker = expectedPrior.HasValue ? expectedPrior.Value.Walker.Position : roles.Walker.Position;
            var expectedSurveyor = expectedPrior.HasValue ? expectedPrior.Value.Surveyor.Position : roles.Surveyor.Position;
            if (!PositionEquals(walker.Position, expectedWalker) || !PositionEquals(surveyor.Position, expectedSurveyor))
                throw new InvalidOperationException("A locomotion body drifted during pair resolution.");
        }

        private static FrozenLocomotionRole ValidateRoleFromRetained(FrozenLocomotionRole role)
        {
            ValidateBodyAndBox(role.Root, role.Body, role.Box);
            if (role.Body.transform != role.Root || role.Box.transform != role.Root || role.Box.attachedRigidbody != role.Body || !role.Root.gameObject.activeInHierarchy ||
                QuantizePositiveSize(role.Box.size.x) != role.Size.X || QuantizePositiveSize(role.Box.size.y) != role.Size.Y)
                throw new InvalidOperationException("Locomotion frozen role geometry changed during the tick.");
            var colliders = role.Root.GetComponentsInChildren<Collider2D>(true);
            var solidCount = 0;
            for (var index = 0; index < colliders.Length; index++) if (colliders[index].enabled && !colliders[index].isTrigger) { solidCount++; if (colliders[index] != role.Box) throw new InvalidOperationException("Locomotion role gained an extra solid collider."); }
            if (solidCount != 1) throw new InvalidOperationException("Locomotion role lost its sole solid collider.");
            return new FrozenLocomotionRole(role.RoleId, role.Root, role.Body, role.Box,
                QuantizeBodyPosition(role.Body.position), role.Size);
        }

        private RegularEnemyCollisionPairResolution ResolvePair(RegularEnemyMotionPairPrediction prediction, FrozenLocomotionRoles roles)
        {
            var walker = ResolveRole(prediction.Walker, roles.Walker, roles);
            var surveyor = ResolveRole(prediction.Surveyor, roles.Surveyor, roles);
            return new RegularEnemyCollisionPairResolution(walker, surveyor);
        }

        private RegularEnemyCollisionResolution ResolveRole(RegularEnemyMotionPrediction prediction, FrozenLocomotionRole role, FrozenLocomotionRoles roles)
        {
            if (!string.Equals(prediction.RoleId, role.RoleId, StringComparison.Ordinal))
                throw new InvalidOperationException("Locomotion prediction role and frozen role disagree.");
            if (!prediction.Alive)
                return new RegularEnemyCollisionResolution(prediction.Tick, prediction.RoleId, prediction.StartPosition,
                    prediction.PredictedPosition, prediction.StartPosition, false, false, false, prediction.PriorGrounded);

            var x = ResolveAxis(prediction.StartPosition.X, prediction.PredictedPosition.X, prediction.StartPosition,
                true, role, roles);
            var xResolvedOrigin = new PositionQ4096(x.Position, prediction.StartPosition.Y);
            var y = ResolveAxis(prediction.StartPosition.Y, prediction.PredictedPosition.Y, xResolvedOrigin,
                false, role, roles);
            var resolved = new PositionQ4096(x.Position, y.Position);
            var support = false;
            if (prediction.PriorGrounded && prediction.PredictedPosition.Y == prediction.StartPosition.Y && !y.Blocked)
                support = ProbeSupportAt(role, roles, resolved);
            var grounded = (prediction.PredictedPosition.Y < prediction.StartPosition.Y && y.Blocked) || support;
            return new RegularEnemyCollisionResolution(prediction.Tick, prediction.RoleId, prediction.StartPosition,
                prediction.PredictedPosition, resolved, x.Blocked, y.Blocked, support, grounded);
        }

        private AxisResolution ResolveAxis(int start, int predicted, PositionQ4096 origin, bool horizontal,
            FrozenLocomotionRole role, FrozenLocomotionRoles roles)
        {
            var delta = checked((long)predicted - start);
            if (delta == 0) return new AxisResolution(predicted, false);
            var requested = Abs(delta);
            var direction = horizontal
                ? delta < 0 ? Vector2.left : Vector2.right
                : delta < 0 ? Vector2.down : Vector2.up;
            var hit = FindBestHit(role.RoleId, origin, role.Size, direction, checked(requested + SkinQ4096), roles, true);
            if (!hit.HasValue) return new AxisResolution(predicted, false);
            var allowed = Math.Max(0L, hit.Value.DistanceQ4096 - SkinQ4096);
            if (allowed >= requested) return new AxisResolution(predicted, false);
            var signed = delta < 0 ? -allowed : allowed;
            return new AxisResolution(checked((int)(start + signed)), true);
        }

        private bool ProbeSupport(FrozenLocomotionRole role, FrozenLocomotionRoles roles) =>
            ProbeSupportAt(role, roles, role.Position);

        private bool ProbeSupportAt(FrozenLocomotionRole role, FrozenLocomotionRoles roles, PositionQ4096 origin) =>
            FindBestHit(role.RoleId, origin, role.Size, Vector2.down, SupportProbeQ4096, roles, true).HasValue;

        private CollisionHit? FindBestHit(string roleId, PositionQ4096 origin, PositionQ4096 size, Vector2 direction,
            long distanceQ4096, FrozenLocomotionRoles roles, bool requireOpposingNormal)
        {
            _queryCountForTests++;
            if (roleId == WalkerId) _walkerQueryCountForTests++; else if (roleId == SurveyorId) _surveyorQueryCountForTests++;
            if (roleId == WalkerId && (direction == Vector2.up || direction == Vector2.down)) _lastWalkerVerticalOriginForTests = origin;
            if (_mutationRoleForTests == roleId && _hitMutationForTests == LocomotionHitMutationForTests.ForcedFailure)
                throw new InvalidOperationException("Injected locomotion query failure for atomicity verification.");
            var distance = distanceQ4096 / UnitsPerWorldUnit;
            if (float.IsNaN(distance) || float.IsInfinity(distance) || distance < 0f)
                throw new OverflowException("Locomotion cast distance is not finite and nonnegative.");
            var filter = ContactFilter2D.noFilter;
            filter.useLayerMask = true;
            filter.layerMask = Physics2D.DefaultRaycastLayers;
            filter.useTriggers = false;
            var count = Physics2D.BoxCast(ToWorld(origin), ToWorld(size), 0f, direction, filter, _hits, distance);
            if (_mutationRoleForTests == roleId && _hitMutationForTests == LocomotionHitMutationForTests.ForcedSaturation)
                count = HitBufferCapacity;
            if (count >= HitBufferCapacity) throw new InvalidOperationException("Locomotion BoxCast saturated its fixed hit buffer.");

            var keys = new HashSet<string>(StringComparer.Ordinal);
            CollisionHit? best = null;
            var mutationApplied = false;
            for (var index = 0; index < count; index++)
            {
                var hit = _hits[index];
                var collider = hit.collider;
                if (collider == null || collider.isTrigger || IsActorCollider(collider.transform, roles)) continue;
                var hitDistanceValue = hit.distance;
                var hitFractionValue = hit.fraction;
                var hitNormal = hit.normal;
                if (_mutationRoleForTests == roleId && !mutationApplied)
                {
                    if (_hitMutationForTests == LocomotionHitMutationForTests.DistanceNaN) hitDistanceValue = float.NaN;
                    if (_hitMutationForTests == LocomotionHitMutationForTests.DistancePositiveInfinity) hitDistanceValue = float.PositiveInfinity;
                    if (_hitMutationForTests == LocomotionHitMutationForTests.NormalNaN) hitNormal.x = float.NaN;
                    mutationApplied = _hitMutationForTests == LocomotionHitMutationForTests.DistanceNaN ||
                        _hitMutationForTests == LocomotionHitMutationForTests.DistancePositiveInfinity ||
                        _hitMutationForTests == LocomotionHitMutationForTests.NormalNaN;
                }
                if (!IsFinite(hitDistanceValue) || !IsFinite(hitFractionValue) || !IsFinite(hitNormal.x) || !IsFinite(hitNormal.y))
                    throw new InvalidOperationException("Locomotion cast returned nonfinite hit data.");
                var identity = ColliderIdentity.Create(collider);
                if (!keys.Add(identity.CanonicalKey)) throw new InvalidOperationException("Locomotion cast returned a duplicate stable collider key.");
                var component = OpposingNormal(direction, hitNormal);
                var normalQ4096 = checked((long)Math.Floor(component * UnitsPerWorldUnit));
                if (requireOpposingNormal && normalQ4096 < BlockingNormalQ4096) continue;
                var hitDistance = Math.Max(0L, checked((long)Math.Floor(hitDistanceValue * UnitsPerWorldUnit)));
                var candidate = new CollisionHit(hitDistance, identity);
                if (!best.HasValue || CollisionHit.Compare(candidate, best.Value) < 0) best = candidate;
            }
            _lastSelectedColliderKeyForTests = best.HasValue ? best.Value.Identity.CanonicalKey : string.Empty;
            return best;
        }

        private bool IsActorCollider(Transform value, FrozenLocomotionRoles roles) =>
            IsRootOrDescendant(value, _player.transform) || IsRootOrDescendant(value, roles.Walker.Root) || IsRootOrDescendant(value, roles.Surveyor.Root);

        private static bool IsRootOrDescendant(Transform value, Transform root) => value == root || value.IsChildOf(root);

        private static float OpposingNormal(Vector2 direction, Vector2 normal)
        {
            if (direction == Vector2.right) return -normal.x;
            if (direction == Vector2.left) return normal.x;
            if (direction == Vector2.up) return -normal.y;
            if (direction == Vector2.down) return normal.y;
            throw new ArgumentException("Locomotion casts require an exact cardinal direction.", nameof(direction));
        }

        private void MoveCommittedBodies(SimulationTick tick, RegularEnemyMotionPairSnapshot snapshot, FrozenLocomotionRoles roles)
        {
            try
            {
                if (!snapshot.Walker.Dead) { roles.Walker.Body.MovePosition(ToWorld(snapshot.Walker.Position)); _walkerMoveCountForTests++; }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Fail-stop MovePosition exception for walker at SimulationTick {tick.Value}.", exception);
            }
            try
            {
                if (!snapshot.Surveyor.Dead) { roles.Surveyor.Body.MovePosition(ToWorld(snapshot.Surveyor.Position)); _surveyorMoveCountForTests++; }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"Fail-stop MovePosition exception for surveyor at SimulationTick {tick.Value}.", exception);
            }
        }

        private static bool RoleBindingEquals(FrozenLocomotionRole left, FrozenLocomotionRole right) =>
            left.RoleId == right.RoleId && left.Root == right.Root && left.Body == right.Body && left.Box == right.Box && PositionEquals(left.Size, right.Size);
        private static bool PairEquals(RegularEnemyMotionPairSnapshot a, RegularEnemyMotionPairSnapshot b) =>
            a.Tick == b.Tick && a.NextExpectedTick == b.NextExpectedTick && MotionEquals(a.Walker, b.Walker) && MotionEquals(a.Surveyor, b.Surveyor);
        private static bool MotionEquals(RegularEnemyMotionSnapshot a, RegularEnemyMotionSnapshot b) =>
            a.RoleId == b.RoleId && PositionEquals(a.Position, b.Position) && PositionEquals(a.Velocity, b.Velocity) &&
            a.XPositionRemainder == b.XPositionRemainder && a.YPositionRemainder == b.YPositionRemainder &&
            a.GravityRemainder == b.GravityRemainder && a.Grounded == b.Grounded && a.Dead == b.Dead;
        private static bool PositionEquals(PositionQ4096 left, PositionQ4096 right) => left.X == right.X && left.Y == right.Y;
        private static PositionQ4096 QuantizeBodyPosition(Vector2 position) => new PositionQ4096(QuantizeRound(position.x), QuantizeRound(position.y));
        private static int QuantizePositiveSize(float value)
        {
            var quantized = QuantizeRound(value);
            if (quantized <= 0) throw new InvalidOperationException("Locomotion box size must be finite and quantize positive.");
            return quantized;
        }
        private static int QuantizeRound(float value) => !IsFinite(value)
            ? throw new ArgumentOutOfRangeException(nameof(value))
            : checked((int)Math.Round(value * UnitsPerWorldUnit, MidpointRounding.AwayFromZero));
        private static void ValidateZeroAngularVelocity(float value)
        {
            if (!IsFinite(value) || value != 0f) throw new InvalidOperationException("Locomotion angular velocity must be finite zero.");
        }
        private static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        private static long Abs(long value) => value < 0 ? checked(-value) : value;
        private static Vector2 ToWorld(PositionQ4096 value) => new Vector2(value.X / UnitsPerWorldUnit, value.Y / UnitsPerWorldUnit);

        private readonly struct AxisResolution
        {
            internal AxisResolution(int position, bool blocked) { Position = position; Blocked = blocked; }
            internal int Position { get; }
            internal bool Blocked { get; }
        }

        private readonly struct CollisionHit
        {
            internal CollisionHit(long distanceQ4096, ColliderIdentity identity) { DistanceQ4096 = distanceQ4096; Identity = identity; }
            internal long DistanceQ4096 { get; }
            internal ColliderIdentity Identity { get; }
            internal static int Compare(CollisionHit left, CollisionHit right)
            {
                var distance = left.DistanceQ4096.CompareTo(right.DistanceQ4096);
                if (distance != 0) return distance;
                var scene = string.CompareOrdinal(left.Identity.SceneName, right.Identity.SceneName);
                if (scene != 0) return scene;
                var hierarchy = ColliderIdentity.CompareHierarchy(left.Identity.HierarchySegments, right.Identity.HierarchySegments);
                if (hierarchy != 0) return hierarchy;
                var type = string.CompareOrdinal(left.Identity.ColliderTypeFullName, right.Identity.ColliderTypeFullName);
                return type != 0 ? type : left.Identity.ColliderComponentOrdinal.CompareTo(right.Identity.ColliderComponentOrdinal);
            }
        }

        private readonly struct FrozenLocomotionRole
        {
            internal FrozenLocomotionRole(string roleId, Transform root, Rigidbody2D body, BoxCollider2D box, PositionQ4096 position, PositionQ4096 size)
            { RoleId = roleId; Root = root; Body = body; Box = box; Position = position; Size = size; }
            internal string RoleId { get; }
            internal Transform Root { get; }
            internal Rigidbody2D Body { get; }
            internal BoxCollider2D Box { get; }
            internal PositionQ4096 Position { get; }
            internal PositionQ4096 Size { get; }
        }

        private readonly struct FrozenLocomotionRoles
        {
            internal FrozenLocomotionRoles(FrozenLocomotionRole walker, FrozenLocomotionRole surveyor) { Walker = walker; Surveyor = surveyor; }
            internal FrozenLocomotionRole Walker { get; }
            internal FrozenLocomotionRole Surveyor { get; }
        }
    }

    internal enum LocomotionHitMutationForTests
    {
        None = 0,
        ForcedFailure = 1,
        ForcedSaturation = 2,
        DistanceNaN = 3,
        DistancePositiveInfinity = 4,
        NormalNaN = 5
    }

    // REQ-COM-004: immutable value-only -160 publication; no Unity object or mutable collection escapes.
    internal readonly struct CarriedEnemyMotionView
    {
        internal CarriedEnemyMotionView(SimulationTick tick, RegularEnemyMotionPairSnapshot snapshot)
        {
            if (snapshot.Tick != tick || snapshot.NextExpectedTick.Value != checked(tick.Value + 1))
                throw new ArgumentException("Enemy motion view must be exact and consecutive.");
            Tick = tick;
            Snapshot = snapshot;
        }
        internal SimulationTick Tick { get; }
        internal RegularEnemyMotionPairSnapshot Snapshot { get; }
    }
}
