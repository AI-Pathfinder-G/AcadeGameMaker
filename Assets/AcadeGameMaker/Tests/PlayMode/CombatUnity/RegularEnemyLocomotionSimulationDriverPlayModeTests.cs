// AC-COM-001, AC-COM-003; affected AC-WT-002, AC-WT-005. VD-03 M3C2 focused Unity bridge evidence.
using System;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class RegularEnemyLocomotionSimulationDriverPlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private SimulationMode2D _previousSimulationMode;
        public enum AuthoringFault { InactiveRoot, MissingBody, Trigger, SimulatedFalse, DynamicBody, PositionConstraint, Interpolation, FullKinematicContacts, LinearVelocity, AngularVelocity, Rotation, Scale, Offset, EdgeRadius }
        public enum PublicationFault { BehaviorMissing, BehaviorStale, BehaviorFuture, ReactionMissing, ReactionStale, ReactionFuture }

        [SetUp]
        public void SetUp()
        {
            _previousSimulationMode = Physics2D.simulationMode;
            Physics2D.simulationMode = SimulationMode2D.Script;
        }

        [TearDown]
        public void TearDown()
        {
            for (var index = _objects.Count - 1; index >= 0; index--) UnityEngine.Object.DestroyImmediate(_objects[index]);
            _objects.Clear();
            Physics2D.simulationMode = _previousSimulationMode;
        }

        [Test]
        public void AC_COM_001_AC_WT_002_ExecutionOrderAndImmutableBootstrapAreExact()
        {
            Assert.That(Order<TransferSimulationDriver>(), Is.EqualTo(-200));
            Assert.That(Order<CombatSimulationDriver>(), Is.EqualTo(-190));
            Assert.That(Order<RegularEnemyReactionSimulationDriver>(), Is.EqualTo(-180));
            Assert.That(Order<RegularEnemyBehaviorSimulationDriver>(), Is.EqualTo(-170));
            Assert.That(Order<RegularEnemyLocomotionSimulationDriver>(), Is.EqualTo(-160));
            Assert.That(Attribute.GetCustomAttribute(typeof(PlayerMovementController), typeof(DefaultExecutionOrder)), Is.Null);

            var rig = CreateRig();
            AdvanceUpstream(rig);
            var view = rig.Locomotion.AdvanceLocomotionPhaseForTests();

            Assert.That(view.Tick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(view.Snapshot.Tick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(view.Snapshot.NextExpectedTick, Is.EqualTo(new SimulationTick(1)));
            Assert.That(view.Snapshot.Walker.RoleId, Is.EqualTo("walker"));
            Assert.That(view.Snapshot.Surveyor.RoleId, Is.EqualTo("surveyor"));
            Assert.That(rig.Locomotion.HasSessionForTests, Is.True);
            Assert.That(rig.Locomotion.LatestCarriedViewForTests.Value.Snapshot.Walker.Position.X,
                Is.EqualTo(view.Snapshot.Walker.Position.X));
        }

        [Test]
        public void AC_COM_003_NonzeroBootstrapUsesExactGlobalTick()
        {
            var rig = CreateRig(firstTick: 7, walkerPosition: new Vector2(5f, 0f));
            AdvanceUpstream(rig);

            var view = rig.Locomotion.AdvanceLocomotionPhaseForTests();

            Assert.That(view.Tick, Is.EqualTo(new SimulationTick(7)));
            Assert.That(view.Snapshot.NextExpectedTick, Is.EqualTo(new SimulationTick(8)));
            Assert.That(rig.Locomotion.NextExpectedTickForTests, Is.EqualTo(new SimulationTick(8)));
        }

        [Test]
        public void AC_COM_003_MissingBindingsAndColliderRootMismatchRejectBeforeSession()
        {
            var unbound = Track(new GameObject("unbound-locomotion")).AddComponent<RegularEnemyLocomotionSimulationDriver>();
            Assert.Throws<InvalidOperationException>(() => unbound.AdvanceLocomotionPhaseForTests());
            Assert.That(unbound.HasSessionForTests, Is.False);

            var mismatch = CreateRig(walkerColliderOnChild: true);
            AdvanceUpstream(mismatch);
            Assert.Throws<InvalidOperationException>(() => mismatch.Locomotion.AdvanceLocomotionPhaseForTests());
            Assert.That(mismatch.Locomotion.HasSessionForTests, Is.False);
            Assert.That(mismatch.Locomotion.QueryCountForTests, Is.Zero);
        }

        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NegativeInfinity)]
        [TestCase(0f)]
        [TestCase(-1f)]
        public void AC_COM_003_NonfiniteOrNonpositiveBoxGeometryRejects(float value)
        {
            Assert.Catch<Exception>(() => RegularEnemyLocomotionSimulationDriver.QuantizePositiveSizeForTests(value));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void AC_COM_003_MissingOrUnsortedCombatRosterRejectsBeforeQuery(bool unsorted)
        {
            var rig = CreateRig();
            AdvanceUpstream(rig);
            var source = rig.Combat.FrozenTargetsForLocomotionBridge;
            FrozenCombatTarget player = default, walker = default, surveyor = default;
            for (var index = 0; index < source.Count; index++)
            {
                if (source[index].TargetId == "player") player = source[index];
                if (source[index].TargetId == "walker") walker = source[index];
                if (source[index].TargetId == "surveyor") surveyor = source[index];
            }
            IReadOnlyList<FrozenCombatTarget> mutation = unsorted
                ? new[] { walker, surveyor, player }
                : new[] { player, walker };
            typeof(CombatSimulationDriver).GetField("_frozenTargets", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(rig.Combat, mutation);

            Assert.Throws<InvalidOperationException>(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
            Assert.That(rig.Locomotion.HasSessionForTests, Is.False);
            Assert.That(rig.Locomotion.QueryCountForTests, Is.Zero);
        }

        [Test]
        public void AC_COM_003_AC_WT_005_FirstTickMalformedBodyLeavesSessionAndViewAbsent()
        {
            var rig = CreateRig();
            AdvanceUpstream(rig);
            rig.SurveyorBody.bodyType = RigidbodyType2D.Dynamic;

            Assert.Throws<InvalidOperationException>(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
            Assert.That(rig.Locomotion.HasSessionForTests, Is.False);
            Assert.That(rig.Locomotion.LatestCarriedViewForTests.HasValue, Is.False);
            Assert.That(rig.WalkerBody.position, Is.EqualTo(new Vector2(1f, 0f)));
            Assert.That(rig.SurveyorBody.position, Is.EqualTo(new Vector2(4f, 0f)));
        }

        [Test]
        public void AC_COM_001_AC_COM_003_FloorDoesNotBlockX_WhileWallBlocksAndSupportGrounds()
        {
            var rig = CreateRig();
            CreateSolid("floor", new Vector2(2.5f, -1f), new Vector2(8f, 1f));
            CreateSolid("left-wall", new Vector2(0f, 0f), new Vector2(1f, 4f));
            AdvanceUpstream(rig);

            var view = rig.Locomotion.AdvanceLocomotionPhaseForTests();

            Assert.That(view.Snapshot.Walker.Position.X, Is.EqualTo(4096), "touching wall must block the leftward Approach sweep");
            Assert.That(view.Snapshot.Walker.Grounded, Is.True);
            Assert.That(view.Snapshot.Surveyor.Grounded, Is.True);
        }

        [Test]
        public void AC_COM_001_AC_COM_003_TriggersAndActorDescendantsAreIgnored_WhileEnvironmentControlBlocks()
        {
            // Five world units keeps the walker in Approach instead of entering the
            // <=4-unit DashTelegraph, so this tick has an exact nonzero X prediction.
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            var trigger = Track(new GameObject("trigger"));
            trigger.transform.position = new Vector2(4f, 0f);
            var triggerCollider = trigger.AddComponent<BoxCollider2D>();
            triggerCollider.size = new Vector2(1f, 4f);
            triggerCollider.isTrigger = true;
            var playerChild = Track(new GameObject("player-child-solid"));
            playerChild.transform.SetParent(rig.Player.transform, false);
            playerChild.transform.localPosition = new Vector2(4.5f, 0f);
            playerChild.AddComponent<BoxCollider2D>().size = new Vector2(1f, 4f);
            var walkerChild = Track(new GameObject("walker-child-trigger"));
            walkerChild.transform.SetParent(rig.WalkerBody.transform, false);
            walkerChild.AddComponent<BoxCollider2D>().isTrigger = true;
            var surveyorChild = Track(new GameObject("surveyor-child-trigger"));
            surveyorChild.transform.SetParent(rig.SurveyorBody.transform, false);
            surveyorChild.AddComponent<BoxCollider2D>().isTrigger = true;
            AdvanceUpstream(rig);

            var ignored = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            Assert.That(ignored.Snapshot.Walker.Position.X, Is.LessThan(5 * 4096),
                "ignored trigger/player descendants must not truncate a nonzero Approach prediction");

            ApplyCommittedBodyPositions(rig, ignored.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();
            var walkerX = ignored.Snapshot.Walker.Position.X / 4096f;
            CreateSolid("environment-control-wall", new Vector2(walkerX - 1f, 0f), new Vector2(1f, 4f));
            AdvanceUpstream(rig);
            var blocked = rig.Locomotion.AdvanceLocomotionPhaseForTests();

            Assert.That(blocked.Snapshot.Walker.Position.X, Is.EqualTo(ignored.Snapshot.Walker.Position.X),
                "an otherwise identical solid environment wall must truncate the next Approach sweep");
        }

        [Test]
        public void AC_COM_003_AC_WT_005_PostBootstrapDriftPreservesPairAndCarriedView()
        {
            var rig = CreateRig();
            AdvanceUpstream(rig);
            var first = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            ApplyCommittedBodyPositions(rig, first.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();
            AdvanceUpstream(rig);
            var prior = rig.Locomotion.LatestCarriedViewForTests.Value;
            rig.SurveyorBody.position += Vector2.right;

            Assert.Throws<InvalidOperationException>(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
            var retained = rig.Locomotion.LatestCarriedViewForTests.Value;
            Assert.That(retained.Tick, Is.EqualTo(prior.Tick));
            Assert.That(retained.Snapshot.NextExpectedTick, Is.EqualTo(prior.Snapshot.NextExpectedTick));
            Assert.That(rig.Locomotion.NextExpectedTickForTests, Is.EqualTo(new SimulationTick(1)));
        }

        [Test]
        public void AC_COM_003_AC_WT_005_ExtraSolidColliderRejectsBeforeFirstAdoption()
        {
            var rig = CreateRig();
            rig.WalkerBody.gameObject.AddComponent<CircleCollider2D>();
            AdvanceUpstream(rig);

            Assert.Throws<InvalidOperationException>(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
            Assert.That(rig.Locomotion.HasSessionForTests, Is.False);
            Assert.That(rig.Locomotion.LatestCarriedViewForTests.HasValue, Is.False);
        }

        [TestCase(AuthoringFault.InactiveRoot)]
        [TestCase(AuthoringFault.MissingBody)]
        [TestCase(AuthoringFault.Trigger)]
        [TestCase(AuthoringFault.SimulatedFalse)]
        [TestCase(AuthoringFault.DynamicBody)]
        [TestCase(AuthoringFault.PositionConstraint)]
        [TestCase(AuthoringFault.Interpolation)]
        [TestCase(AuthoringFault.FullKinematicContacts)]
        [TestCase(AuthoringFault.LinearVelocity)]
        [TestCase(AuthoringFault.Rotation)]
        [TestCase(AuthoringFault.Scale)]
        [TestCase(AuthoringFault.Offset)]
        [TestCase(AuthoringFault.EdgeRadius)]
        public void AC_COM_003_AuthoringVariantsRejectBeforeLocomotionMutation(AuthoringFault fault)
        {
            var rig = CreateRig();
            AdvanceUpstream(rig);
            ApplyAuthoringFault(rig, fault);

            Assert.Throws<InvalidOperationException>(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
            Assert.That(rig.Locomotion.HasSessionForTests, Is.False);
            Assert.That(rig.Locomotion.LatestCarriedViewForTests.HasValue, Is.False);
            Assert.That(rig.Locomotion.QueryCountForTests, Is.Zero);
            Assert.That(rig.Locomotion.WalkerMoveCountForTests, Is.Zero);
            Assert.That(rig.Locomotion.SurveyorMoveCountForTests, Is.Zero);
        }

        [TestCase(1f)]
        [TestCase(-1f)]
        [TestCase(float.NaN)]
        [TestCase(float.PositiveInfinity)]
        public void AC_COM_003_NonzeroOrNonfiniteAngularVelocityInvariantRejectsDeterministically(float value)
        {
            Assert.Throws<InvalidOperationException>(() => RegularEnemyLocomotionSimulationDriver.ValidateZeroAngularVelocityForTests(value));
        }

        [TestCase(LocomotionHitMutationForTests.ForcedSaturation)]
        [TestCase(LocomotionHitMutationForTests.DistanceNaN)]
        [TestCase(LocomotionHitMutationForTests.DistancePositiveInfinity)]
        [TestCase(LocomotionHitMutationForTests.NormalNaN)]
        public void AC_COM_003_HitMutationsPreserveFirstTickAbsence(LocomotionHitMutationForTests mutation)
        {
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            CreateSolid("mutation-floor", new Vector2(3f, -1f), new Vector2(10f, 1f));
            AdvanceUpstream(rig);
            rig.Locomotion.MutateNextQueriesForTests("walker", mutation);

            Assert.Throws<InvalidOperationException>(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
            Assert.That(rig.Locomotion.HasSessionForTests, Is.False);
            Assert.That(rig.Locomotion.LatestCarriedViewForTests.HasValue, Is.False);
            Assert.That(rig.Locomotion.WalkerMoveCountForTests, Is.Zero);
            Assert.That(rig.Locomotion.SurveyorMoveCountForTests, Is.Zero);
        }

        [Test]
        public void AC_COM_003_DuplicateStableEnvironmentKeyRejectsBeforeAdoption()
        {
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            CreateSolid("duplicate", new Vector2(4f, 0f), new Vector2(1f, 4f));
            CreateSolid("duplicate", new Vector2(4f, 0f), new Vector2(1f, 4f));
            AdvanceUpstream(rig);

            Assert.Throws<InvalidOperationException>(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
            Assert.That(rig.Locomotion.HasSessionForTests, Is.False);
            Assert.That(rig.Locomotion.WalkerMoveCountForTests, Is.Zero);
        }

        [Test]
        public void AC_COM_003_EqualDistanceHitsUseCanonicalColliderOrdering()
        {
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            AdvanceUpstream(rig);
            var first = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            var start = first.Snapshot.Walker.Position;
            CreateSolid("b-wall", new Vector2(start.X / 4096f - 1f, start.Y / 4096f), new Vector2(1f, 4f));
            CreateSolid("a-wall", new Vector2(start.X / 4096f - 1f, start.Y / 4096f), new Vector2(1f, 4f));

            var resolution = rig.Locomotion.ResolvePairForTests(SyntheticPair(first.Snapshot, -200, 0, false));

            Assert.That(resolution.Walker.XBlocked, Is.True);
            StringAssert.Contains("a-wall", rig.Locomotion.LastSelectedColliderKeyForTests);
        }

        [Test]
        public void AC_COM_003_SurveyorQueryFailureAfterWalkerResolutionIsPairAtomic()
        {
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            AdvanceUpstream(rig);
            var first = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            ApplyCommittedBodyPositions(rig, first.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();
            AdvanceUpstream(rig);
            var queryBefore = rig.Locomotion.QueryCountForTests;
            var walkerMovesBefore = rig.Locomotion.WalkerMoveCountForTests;
            var surveyorMovesBefore = rig.Locomotion.SurveyorMoveCountForTests;
            rig.Locomotion.MutateNextQueriesForTests("surveyor", LocomotionHitMutationForTests.ForcedFailure);

            Assert.Throws<InvalidOperationException>(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
            Assert.That(rig.Locomotion.QueryCountForTests, Is.GreaterThanOrEqualTo(queryBefore + 2));
            Assert.That(rig.Locomotion.WalkerMoveCountForTests, Is.EqualTo(walkerMovesBefore));
            Assert.That(rig.Locomotion.SurveyorMoveCountForTests, Is.EqualTo(surveyorMovesBefore));
            Assert.That(rig.Locomotion.LatestCarriedViewForTests.Value.Tick, Is.EqualTo(first.Tick));
            Assert.That(rig.Locomotion.NextExpectedTickForTests, Is.EqualTo(new SimulationTick(1)));
        }

        [TestCase(PublicationFault.BehaviorMissing)]
        [TestCase(PublicationFault.BehaviorStale)]
        [TestCase(PublicationFault.BehaviorFuture)]
        [TestCase(PublicationFault.ReactionMissing)]
        [TestCase(PublicationFault.ReactionStale)]
        [TestCase(PublicationFault.ReactionFuture)]
        public void AC_COM_003_MissingStaleFutureUpstreamPublicationPreservesCommittedPair(PublicationFault fault)
        {
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            AdvanceUpstream(rig);
            var first = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            ApplyCommittedBodyPositions(rig, first.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();
            AdvanceUpstream(rig);
            var queries = rig.Locomotion.QueryCountForTests;
            var walkerMoves = rig.Locomotion.WalkerMoveCountForTests;
            var surveyorMoves = rig.Locomotion.SurveyorMoveCountForTests;
            MutatePublication(rig, fault);

            Assert.Throws<InvalidOperationException>(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
            Assert.That(rig.Locomotion.LatestCarriedViewForTests.Value.Tick, Is.EqualTo(first.Tick));
            Assert.That(rig.Locomotion.NextExpectedTickForTests, Is.EqualTo(new SimulationTick(1)));
            Assert.That(rig.Locomotion.QueryCountForTests, Is.EqualTo(queries));
            Assert.That(rig.Locomotion.WalkerMoveCountForTests, Is.EqualTo(walkerMoves));
            Assert.That(rig.Locomotion.SurveyorMoveCountForTests, Is.EqualTo(surveyorMoves));
        }

        [Test]
        public void AC_COM_003_PhysicsStepConsumesMoveBeforeNextMinus160AlignmentCheck()
        {
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            AdvanceUpstream(rig);
            var first = rig.Locomotion.AdvanceLocomotionPhaseForTests();

            Assert.That(Physics2D.Simulate(1f / 60f), Is.True);
            AssertBodyMatches(rig.WalkerBody, first.Snapshot.Walker.Position);
            AssertBodyMatches(rig.SurveyorBody, first.Snapshot.Surveyor.Position);
            rig.Player.AdvanceOneFixedTickForTests();
            AdvanceUpstream(rig);
            Assert.DoesNotThrow(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
        }

        [Test]
        public void AC_COM_001_LongApproachAndRelocateTracesAreIdenticalAt30_60_144Grouping()
        {
            var at30 = ReplayGrouped(30, 60, new Vector2(-100f, 0f), new Vector2(5f, 0f));
            DestroyTracked();
            var at60 = ReplayGrouped(60, 60, new Vector2(-100f, 0f), new Vector2(5f, 0f));
            DestroyTracked();
            var at144 = ReplayGrouped(144, 60, new Vector2(-100f, 0f), new Vector2(5f, 0f));

            CollectionAssert.AreEqual(at30, at60);
            CollectionAssert.AreEqual(at60, at144);
            for (var tick = 0; tick < 60; tick++) StringAssert.Contains("walker=Approach", at60[tick]);
            for (var tick = 0; tick < 48; tick++) StringAssert.Contains("surveyor=Relocate", at60[tick]);
            StringAssert.Contains("surveyor=FireTelegraph", at60[48]);
        }

        [Test]
        public void AC_COM_001_DashActiveIsExactlyTwelveBridgeTicks()
        {
            var rig = CreateRig(walkerPosition: new Vector2(1f, 0f));
            var dashTicks = 0;
            for (var tick = 0; tick < 30; tick++)
            {
                AdvanceUpstream(rig);
                if (rig.Behavior.LatestCarriedViewForTests.Value.Snapshot.Walker.Phase == WalkerBehaviorPhase.DashActive) dashTicks++;
                var view = rig.Locomotion.AdvanceLocomotionPhaseForTests();
                ApplyCommittedBodyPositions(rig, view.Snapshot);
                rig.Player.AdvanceOneFixedTickForTests();
            }
            Assert.That(dashTicks, Is.EqualTo(12));
        }

        [Test]
        public void AC_COM_003_DeathRepeatedDeadAndResetHaveExactQueryAndMoveLifecycle()
        {
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            AdvanceUpstream(rig);
            var first = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            ApplyCommittedBodyPositions(rig, first.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();

            var deathTick = rig.Player.NextExpectedTick;
            rig.Combat.SubmitForTests(new CombatSimulationInput(deathTick, null, null, null,
                new[] { new DamageRequest("walker-lethal", "hazard", "walker", 9, DamageKind.EnemyContact, deathTick) }, false));
            AdvanceUpstream(rig);
            var walkerQueries = rig.Locomotion.WalkerQueryCountForTests;
            var walkerMoves = rig.Locomotion.WalkerMoveCountForTests;
            var death = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            Assert.That(death.Snapshot.Walker.Dead, Is.True);
            Assert.That(rig.Locomotion.WalkerQueryCountForTests, Is.EqualTo(walkerQueries));
            Assert.That(rig.Locomotion.WalkerMoveCountForTests, Is.EqualTo(walkerMoves));

            ApplyCommittedBodyPositions(rig, death.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();
            AdvanceUpstream(rig);
            walkerQueries = rig.Locomotion.WalkerQueryCountForTests;
            walkerMoves = rig.Locomotion.WalkerMoveCountForTests;
            var repeated = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            Assert.That(repeated.Snapshot.Walker.Dead, Is.True);
            Assert.That(rig.Locomotion.WalkerQueryCountForTests, Is.EqualTo(walkerQueries));
            Assert.That(rig.Locomotion.WalkerMoveCountForTests, Is.EqualTo(walkerMoves));

            ApplyCommittedBodyPositions(rig, repeated.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();
            var resetTick = rig.Player.NextExpectedTick;
            rig.Combat.SubmitForTests(new CombatSimulationInput(resetTick, null, null, null, Array.Empty<DamageRequest>(), true));
            AdvanceUpstream(rig);
            var allQueries = rig.Locomotion.QueryCountForTests;
            var reset = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            Assert.That(rig.Locomotion.QueryCountForTests, Is.EqualTo(allQueries));
            Assert.That(reset.Snapshot.Walker.Dead, Is.False);
            Assert.That(reset.Snapshot.Walker.Position.X, Is.EqualTo(5 * 4096));
            Assert.That(reset.Snapshot.Surveyor.Position.X, Is.EqualTo(4 * 4096));
            Assert.That(rig.Locomotion.WalkerMoveCountForTests, Is.EqualTo(walkerMoves + 1));
        }

        [Test]
        public void AC_COM_003_TerminalMinusOneCommitThenTerminalPublicationRejectPreservesState()
        {
            var firstTick = int.MaxValue - 1;
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f), firstTick: firstTick);
            PublishSyntheticUpstream(rig, new SimulationTick(firstTick));

            var terminalCommit = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            Assert.That(terminalCommit.Tick.Value, Is.EqualTo(firstTick));
            Assert.That(terminalCommit.Snapshot.NextExpectedTick.Value, Is.EqualTo(int.MaxValue));
            ApplyCommittedBodyPositions(rig, terminalCommit.Snapshot);
            rig.Player.AdvanceOneFixedTickForTests();
            var queries = rig.Locomotion.QueryCountForTests;
            var moves = rig.Locomotion.WalkerMoveCountForTests;

            Assert.Throws<InvalidOperationException>(() => rig.Locomotion.AdvanceLocomotionPhaseForTests());
            Assert.That(rig.Locomotion.LatestCarriedViewForTests.Value.Tick.Value, Is.EqualTo(firstTick));
            Assert.That(rig.Locomotion.NextExpectedTickForTests.Value, Is.EqualTo(int.MaxValue));
            Assert.That(rig.Locomotion.QueryCountForTests, Is.EqualTo(queries));
            Assert.That(rig.Locomotion.WalkerMoveCountForTests, Is.EqualTo(moves));
        }

        [TestCase(-1)]
        [TestCase(1)]
        public void AC_COM_001_LeftAndRightEnvironmentWallsBlockSyntheticBridgeSweep(int direction)
        {
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            AdvanceUpstream(rig);
            var first = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            var start = first.Snapshot.Walker.Position;
            CreateSolid(direction < 0 ? "left-control" : "right-control",
                new Vector2(start.X / 4096f + direction, start.Y / 4096f), new Vector2(1f, 4f));

            var resolved = rig.Locomotion.ResolvePairForTests(SyntheticPair(first.Snapshot, direction * 200, 0, false));

            Assert.That(resolved.Walker.XBlocked, Is.True);
            Assert.That(resolved.Walker.ResolvedPosition.X, Is.EqualTo(start.X));
        }

        [Test]
        public void AC_COM_001_CeilingLandingLedgeAndXResolvedYOriginAreExact()
        {
            var ceilingRig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            AdvanceUpstream(ceilingRig);
            var ceilingFirst = ceilingRig.Locomotion.AdvanceLocomotionPhaseForTests();
            var start = ceilingFirst.Snapshot.Walker.Position;
            CreateSolid("ceiling", new Vector2(start.X / 4096f, start.Y / 4096f + 1f), new Vector2(4f, 1f));
            var ceiling = ceilingRig.Locomotion.ResolvePairForTests(SyntheticPair(ceilingFirst.Snapshot, 0, 200, false));
            Assert.That(ceiling.Walker.YBlocked, Is.True);
            Assert.That(ceiling.Walker.Grounded, Is.False);
            DestroyTracked();

            var floorRig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            AdvanceUpstream(floorRig);
            var floorFirst = floorRig.Locomotion.AdvanceLocomotionPhaseForTests();
            start = floorFirst.Snapshot.Walker.Position;
            CreateSolid("landing-floor", new Vector2(start.X / 4096f, start.Y / 4096f - 1f), new Vector2(4f, 1f));
            var landing = floorRig.Locomotion.ResolvePairForTests(SyntheticPair(floorFirst.Snapshot, -200, -200, false));
            Assert.That(landing.Walker.YBlocked, Is.True);
            Assert.That(landing.Walker.Grounded, Is.True);
            Assert.That(floorRig.Locomotion.LastWalkerVerticalOriginForTests.X, Is.EqualTo(start.X - 200),
                "the Y sweep must begin at the X-resolved candidate, not the old center");
            DestroyTracked();

            var ledgeRig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            AdvanceUpstream(ledgeRig);
            var ledgeFirst = ledgeRig.Locomotion.AdvanceLocomotionPhaseForTests();
            var ledge = ledgeRig.Locomotion.ResolvePairForTests(SyntheticPair(ledgeFirst.Snapshot, -200, 0, true));
            Assert.That(ledge.Walker.StationarySupportConfirmed, Is.False);
            Assert.That(ledge.Walker.Grounded, Is.False);
        }

        [Test]
        public void AC_COM_001_HeavyGroundedSeedsRemainSupportedThroughBridge()
        {
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            CreateSolid("heavy-floor", new Vector2(3f, -1f), new Vector2(10f, 1f));
            PublishHeavyUpstream(rig, new SimulationTick(0));

            var view = rig.Locomotion.AdvanceLocomotionPhaseForTests();

            Assert.That(view.Snapshot.Walker.Grounded, Is.True);
            Assert.That(view.Snapshot.Walker.Velocity.Y, Is.Zero);
            Assert.That(view.Snapshot.Surveyor.Grounded, Is.True);
            Assert.That(view.Snapshot.Surveyor.Velocity.Y, Is.Zero);
        }

        [Test]
        public void AC_COM_001_HeavyAirborneForcedDescentReachesExactFallClampAndBodyTrace()
        {
            var rig = CreateRig(walkerPosition: new Vector2(5f, 0f));
            CarriedEnemyMotionView view = default;
            for (var tick = 0; tick < 24; tick++)
            {
                PublishHeavyUpstream(rig, new SimulationTick(tick));
                view = rig.Locomotion.AdvanceLocomotionPhaseForTests();
                Assert.That(view.Snapshot.Walker.Grounded, Is.False);
                Assert.That(view.Snapshot.Surveyor.Grounded, Is.False);
                ApplyCommittedBodyPositions(rig, view.Snapshot);
                AssertBodyMatches(rig.WalkerBody, view.Snapshot.Walker.Position);
                AssertBodyMatches(rig.SurveyorBody, view.Snapshot.Surveyor.Position);
                rig.Player.AdvanceOneFixedTickForTests();
            }
            Assert.That(view.Snapshot.Surveyor.Velocity.Y, Is.EqualTo(-49152));
            Assert.That(view.Snapshot.Surveyor.GravityRemainder, Is.Zero);
        }

        private Rig CreateRig(Vector2? walkerPosition = null, Vector2? playerPosition = null, int firstTick = 0, bool walkerColliderOnChild = false)
        {
            var playerObject = Track(new GameObject("player"));
            playerObject.transform.position = playerPosition ?? Vector2.zero;
            var playerBody = playerObject.AddComponent<Rigidbody2D>();
            playerBody.bodyType = RigidbodyType2D.Kinematic;
            var playerCollider = playerObject.AddComponent<CapsuleCollider2D>();
            var player = playerObject.AddComponent<PlayerMovementController>();
            if (firstTick != 0)
            {
                var motor = new PlayerMovementMotor(new SimulationTick(firstTick));
                motor.SeedAuthoritativePosition(checked((int)Math.Round(playerObject.transform.position.x * 4096d, MidpointRounding.AwayFromZero)),
                    checked((int)Math.Round(playerObject.transform.position.y * 4096d, MidpointRounding.AwayFromZero)));
                typeof(PlayerMovementController).GetField("_motor", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(player, motor);
            }
            var playerCombat = playerObject.AddComponent<CombatTarget>();
            playerCombat.ConfigureForTests("player", 5, 45, false, 0, 0, 0, 0, 100, 100, playerCollider, playerObject.transform);

            var walker = CreateEnemy("walker", walkerPosition ?? new Vector2(1f, 0f), 9, walkerColliderOnChild);
            var surveyor = CreateEnemy("surveyor", new Vector2(4f, 0f), 6);

            var transferRegistry = Track(new GameObject("TransferRegistry")).AddComponent<TransferTargetRegistry>();
            transferRegistry.ConfigureForTests(surveyor.Transfer, walker.Transfer);
            var transferDriver = Track(new GameObject("TransferDriver")).AddComponent<TransferSimulationDriver>();
            transferDriver.ConfigureForTests(player, transferRegistry);
            transferDriver.InitializeForTests();

            var combatRegistry = Track(new GameObject("CombatRegistry")).AddComponent<CombatTargetRegistry>();
            combatRegistry.ConfigureForTests(playerCombat, surveyor.Combat, walker.Combat);
            var combatDriver = Track(new GameObject("CombatDriver")).AddComponent<CombatSimulationDriver>();
            var reactionDriver = Track(new GameObject("ReactionDriver")).AddComponent<RegularEnemyReactionSimulationDriver>();
            reactionDriver.ConfigureForTests(player, transferDriver, combatDriver);
            combatDriver.ConfigureForTests(player, combatRegistry, transferDriver, reactionDriver);
            combatDriver.InitializeForTests();
            var behaviorDriver = Track(new GameObject("BehaviorDriver")).AddComponent<RegularEnemyBehaviorSimulationDriver>();
            behaviorDriver.ConfigureForTests(player, combatDriver, reactionDriver);
            var locomotionDriver = Track(new GameObject("LocomotionDriver")).AddComponent<RegularEnemyLocomotionSimulationDriver>();
            locomotionDriver.ConfigureForTests(player, combatDriver, reactionDriver, behaviorDriver);
            return new Rig(player, transferDriver, combatDriver, reactionDriver, behaviorDriver, locomotionDriver,
                walker.Body, surveyor.Body, walker.Box, surveyor.Box);
        }

        private Enemy CreateEnemy(string id, Vector2 position, int health, bool colliderOnChild = false)
        {
            var gameObject = Track(new GameObject(id));
            gameObject.transform.position = position;
            var body = gameObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.interpolation = RigidbodyInterpolation2D.None;
            body.useFullKinematicContacts = false;
            var colliderObject = gameObject;
            if (colliderOnChild)
            {
                colliderObject = Track(new GameObject(id + "-collider"));
                colliderObject.transform.SetParent(gameObject.transform, false);
            }
            var box = colliderObject.AddComponent<BoxCollider2D>();
            var sink = gameObject.AddComponent<TransferSandboxOwnerStub>();
            sink.ConfigureForTests(id, "Target.Enemy.Heavy.v1");
            var transfer = gameObject.AddComponent<TransferTarget>();
            transfer.ConfigureForTests(id, TransferTargetKind.Enemy, 0, 0, 0, 0, 100, 100, "base", box, gameObject.transform, sink);
            var combat = gameObject.AddComponent<CombatTarget>();
            combat.ConfigureForTests(id, health, 0, true, 0, 0, 0, 0, 100, 100, box, gameObject.transform, transfer);
            return new Enemy(body, box, transfer, combat);
        }

        private void CreateSolid(string name, Vector2 position, Vector2 size)
        {
            var value = Track(new GameObject(name));
            value.transform.position = position;
            value.AddComponent<BoxCollider2D>().size = size;
        }

        private string[] ReplayGrouped(int renderFramesPerSecond, int ticks, Vector2 playerPosition, Vector2 walkerPosition)
        {
            var rig = CreateRig(walkerPosition, playerPosition);
            var trace = new List<string>();
            var allowance = 0d;
            while (trace.Count < ticks)
            {
                allowance += 60d / renderFramesPerSecond;
                while (allowance >= 1d && trace.Count < ticks)
                {
                    AdvanceUpstream(rig);
                    var behavior = rig.Behavior.LatestCarriedViewForTests.Value.Snapshot;
                    var motion = rig.Locomotion.AdvanceLocomotionPhaseForTests();
                    trace.Add("tick=" + motion.Tick.Value + ",walker=" + behavior.Walker.Phase + ":" + motion.Snapshot.Walker.Position.X + ":" + motion.Snapshot.Walker.Position.Y +
                        ",surveyor=" + behavior.Surveyor.Phase + ":" + motion.Snapshot.Surveyor.Position.X + ":" + motion.Snapshot.Surveyor.Position.Y);
                    ApplyCommittedBodyPositions(rig, motion.Snapshot);
                    rig.Player.AdvanceOneFixedTickForTests();
                    allowance -= 1d;
                }
            }
            return trace.ToArray();
        }

        private void DestroyTracked()
        {
            for (var index = _objects.Count - 1; index >= 0; index--) UnityEngine.Object.DestroyImmediate(_objects[index]);
            _objects.Clear();
        }

        private static void AdvanceUpstream(Rig rig)
        {
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.AdvanceCombatPhaseForTests();
            rig.Behavior.AdvanceBehaviorPhaseForTests();
        }

        private static void ApplyCommittedBodyPositions(Rig rig, RegularEnemyMotionPairSnapshot snapshot)
        {
            rig.WalkerBody.position = new Vector2(snapshot.Walker.Position.X / 4096f, snapshot.Walker.Position.Y / 4096f);
            rig.SurveyorBody.position = new Vector2(snapshot.Surveyor.Position.X / 4096f, snapshot.Surveyor.Position.Y / 4096f);
        }

        private static void AssertBodyMatches(Rigidbody2D body, PositionQ4096 position)
        {
            Assert.That(body.position.x, Is.EqualTo(position.X / 4096f).Within(0.000001f));
            Assert.That(body.position.y, Is.EqualTo(position.Y / 4096f).Within(0.000001f));
        }

        private static void ApplyAuthoringFault(Rig rig, AuthoringFault fault)
        {
            switch (fault)
            {
                case AuthoringFault.InactiveRoot: rig.WalkerBody.gameObject.SetActive(false); break;
                case AuthoringFault.MissingBody: UnityEngine.Object.DestroyImmediate(rig.WalkerBody); break;
                case AuthoringFault.Trigger: rig.WalkerBox.isTrigger = true; break;
                case AuthoringFault.SimulatedFalse: rig.WalkerBody.simulated = false; break;
                case AuthoringFault.DynamicBody: rig.WalkerBody.bodyType = RigidbodyType2D.Dynamic; break;
                case AuthoringFault.PositionConstraint: rig.WalkerBody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX; break;
                case AuthoringFault.Interpolation: rig.WalkerBody.interpolation = RigidbodyInterpolation2D.Interpolate; break;
                case AuthoringFault.FullKinematicContacts: rig.WalkerBody.useFullKinematicContacts = true; break;
                case AuthoringFault.LinearVelocity: rig.WalkerBody.linearVelocity = Vector2.right; break;
                case AuthoringFault.AngularVelocity: rig.WalkerBody.angularVelocity = 1f; break;
                case AuthoringFault.Rotation: rig.WalkerBody.rotation = 1f; break;
                case AuthoringFault.Scale: rig.WalkerBody.transform.localScale = new Vector3(2f, 1f, 1f); break;
                case AuthoringFault.Offset: rig.WalkerBox.offset = Vector2.right; break;
                case AuthoringFault.EdgeRadius: rig.WalkerBox.edgeRadius = 0.1f; break;
                default: throw new ArgumentOutOfRangeException(nameof(fault));
            }
        }

        private static void MutatePublication(Rig rig, PublicationFault fault)
        {
            var behaviorField = typeof(RegularEnemyBehaviorSimulationDriver).GetField("_carried", BindingFlags.Instance | BindingFlags.NonPublic);
            var reactionField = typeof(RegularEnemyReactionSimulationDriver).GetField("_carried", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fault == PublicationFault.BehaviorMissing) { behaviorField.SetValue(rig.Behavior, null); return; }
            if (fault == PublicationFault.ReactionMissing) { reactionField.SetValue(rig.Reaction, null); return; }
            var tick = fault == PublicationFault.BehaviorStale || fault == PublicationFault.ReactionStale ? new SimulationTick(0) : new SimulationTick(2);
            if (fault == PublicationFault.BehaviorStale || fault == PublicationFault.BehaviorFuture)
            {
                var source = rig.Behavior.LatestCarriedViewForTests.Value.Snapshot;
                var snapshot = new RegularEnemyBehaviorPairSnapshot(tick, source.Walker, source.Surveyor,
                    source.WalkerRecoveryIntent, source.SurveyorShotIntent, new SimulationTick(checked(tick.Value + 1)));
                behaviorField.SetValue(rig.Behavior, new CarriedBehaviorView(tick, snapshot));
                return;
            }
            var reaction = rig.Reaction.CarriedViewForTests.Value;
            reactionField.SetValue(rig.Reaction, new CarriedReactionView(tick,
                ReTick(reaction.Walker, tick), ReTick(reaction.Surveyor, tick)));
        }

        private static RegularEnemyReactionSnapshot ReTick(RegularEnemyReactionSnapshot source, SimulationTick tick) =>
            new RegularEnemyReactionSnapshot(tick, source.TargetId, source.IsAlive, source.IsHeavy, source.LastTransferRevision,
                source.ShieldWindowEndTick, source.FireLockEndTick, source.ReactionAllowsBasicAttack,
                source.RecoveryMultiplierQ1000, source.ForcedDescent, source.CanFire, new SimulationTick(checked(tick.Value + 1)));

        private static void PublishSyntheticUpstream(Rig rig, SimulationTick tick)
        {
            var next = new SimulationTick(checked(tick.Value + 1));
            var walkerReaction = new RegularEnemyReactionSnapshot(tick, "walker", true, false, -1, -1, -1,
                false, 1000, false, false, next);
            var surveyorReaction = new RegularEnemyReactionSnapshot(tick, "surveyor", true, false, -1, -1, -1,
                false, 1000, false, true, next);
            var behavior = new RegularEnemyBehaviorPairSnapshot(tick,
                new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Approach, -1, true, RegularEnemyMovementIntent.Horizontal, -1, true, false, 0, 0),
                new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Relocate, int.MaxValue, true, RegularEnemyMovementIntent.Horizontal, -1, false, false, 0, 0),
                null, null, next);
            typeof(RegularEnemyReactionSimulationDriver).GetField("_carried", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(rig.Reaction, new CarriedReactionView(tick, walkerReaction, surveyorReaction));
            typeof(RegularEnemyBehaviorSimulationDriver).GetField("_carried", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(rig.Behavior, new CarriedBehaviorView(tick, behavior));
        }

        private static void PublishHeavyUpstream(Rig rig, SimulationTick tick)
        {
            var next = new SimulationTick(checked(tick.Value + 1));
            var walkerReaction = new RegularEnemyReactionSnapshot(tick, "walker", true, true, 1, -1, -1,
                false, 1500, false, false, next);
            var surveyorReaction = new RegularEnemyReactionSnapshot(tick, "surveyor", true, true, 1, -1, -1,
                false, 1000, true, false, next);
            var behavior = new RegularEnemyBehaviorPairSnapshot(tick,
                new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Approach, -1, true, RegularEnemyMovementIntent.Horizontal, -1, true, false, 0, 0),
                new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Suppressed, -1, true, RegularEnemyMovementIntent.Downward, 0, false, false, 0, 0),
                null, null, next);
            typeof(RegularEnemyReactionSimulationDriver).GetField("_carried", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(rig.Reaction, new CarriedReactionView(tick, walkerReaction, surveyorReaction));
            typeof(RegularEnemyBehaviorSimulationDriver).GetField("_carried", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(rig.Behavior, new CarriedBehaviorView(tick, behavior));
        }

        private static RegularEnemyMotionPairPrediction SyntheticPair(RegularEnemyMotionPairSnapshot prior, int walkerDeltaX, int walkerDeltaY, bool priorGrounded)
        {
            var tick = prior.NextExpectedTick;
            var walkerTarget = new PositionQ4096(checked(prior.Walker.Position.X + walkerDeltaX), checked(prior.Walker.Position.Y + walkerDeltaY));
            var walker = new RegularEnemyMotionPrediction(tick, "walker", true, prior.Walker.Position, prior.Walker.Velocity,
                new PositionQ4096(walkerDeltaX * 60, walkerDeltaY * 60), walkerTarget, 0, 0, 0, priorGrounded);
            var surveyor = new RegularEnemyMotionPrediction(tick, "surveyor", false, prior.Surveyor.Position, prior.Surveyor.Velocity,
                new PositionQ4096(0, 0), prior.Surveyor.Position, 0, 0, 0, prior.Surveyor.Grounded);
            return new RegularEnemyMotionPairPrediction(tick, walker, surveyor);
        }

        private static int Order<T>() => ((DefaultExecutionOrder)Attribute.GetCustomAttribute(typeof(T), typeof(DefaultExecutionOrder))).order;
        private GameObject Track(GameObject value) { _objects.Add(value); return value; }

        private readonly struct Enemy
        {
            internal Enemy(Rigidbody2D body, BoxCollider2D box, TransferTarget transfer, CombatTarget combat) { Body = body; Box = box; Transfer = transfer; Combat = combat; }
            internal Rigidbody2D Body { get; }
            internal BoxCollider2D Box { get; }
            internal TransferTarget Transfer { get; }
            internal CombatTarget Combat { get; }
        }

        private readonly struct Rig
        {
            internal Rig(PlayerMovementController player, TransferSimulationDriver transfer, CombatSimulationDriver combat,
                RegularEnemyReactionSimulationDriver reaction, RegularEnemyBehaviorSimulationDriver behavior,
                RegularEnemyLocomotionSimulationDriver locomotion, Rigidbody2D walkerBody, Rigidbody2D surveyorBody,
                BoxCollider2D walkerBox, BoxCollider2D surveyorBox)
            { Player = player; Transfer = transfer; Combat = combat; Reaction = reaction; Behavior = behavior; Locomotion = locomotion; WalkerBody = walkerBody; SurveyorBody = surveyorBody; WalkerBox = walkerBox; SurveyorBox = surveyorBox; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal CombatSimulationDriver Combat { get; }
            internal RegularEnemyReactionSimulationDriver Reaction { get; }
            internal RegularEnemyBehaviorSimulationDriver Behavior { get; }
            internal RegularEnemyLocomotionSimulationDriver Locomotion { get; }
            internal Rigidbody2D WalkerBody { get; }
            internal Rigidbody2D SurveyorBody { get; }
            internal BoxCollider2D WalkerBox { get; }
            internal BoxCollider2D SurveyorBox { get; }
        }
    }
}
