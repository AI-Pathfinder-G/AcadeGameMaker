// REQ-MOV-001, REQ-MOV-004, REQ-MOV-006; affected REQ-COM-001, REQ-COM-002,
// REQ-COM-004, REQ-COM-005, REQ-WT-003, REQ-WT-005, REQ-WT-007. M3E1A feasibility evidence only: production
// collision-pair lifecycle remains forbidden until this spike is independently accepted.
using System;
using System.Collections.Generic;
using System.Text;
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
    internal sealed class RegularEnemyCollisionExclusionFeasibilityPlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private SimulationMode2D _priorSimulationMode;

        [SetUp]
        public void UseScriptPhysics()
        {
            _priorSimulationMode = Physics2D.simulationMode;
            Physics2D.simulationMode = SimulationMode2D.Script;
        }

        [TearDown]
        public void RestorePhysics()
        {
            DestroyTrackedObjects();
            Physics2D.simulationMode = _priorSimulationMode;
            Physics2D.SyncTransforms();
        }

        // AC-COM-001, AC-COM-003 (partial); M3E1A gates 1-3 and 6. Each role gets a
        // fresh rig. The direct cast is a bounded canonical oracle around the same NoFilter
        // call, while PlayerMovementController proves the production path changes too.
        [TestCase("walker")]
        [TestCase("surveyor")]
        public void ExactPairIgnoreRemovesOnlySelectedAliveSolidFromFreshProductionCast(string selectedRole)
        {
            var cast = CreateCastRig(selectedRole);
            var selected = selectedRole == "walker" ? cast.Walker : cast.Surveyor;
            var nonignored = selectedRole == "walker" ? cast.Surveyor : cast.Walker;
            var selectedDirection = selectedRole == "walker" ? Vector2.right : Vector2.left;
            var nonignoredDirection = selectedRole == "walker" ? Vector2.left : Vector2.right;
            var selectedId = PlayerMovementController.GetStableAuthoredId(selected);
            var nonignoredId = PlayerMovementController.GetStableAuthoredId(nonignored);
            var walkerId = PlayerMovementController.GetStableAuthoredId(cast.Walker);
            var surveyorId = PlayerMovementController.GetStableAuthoredId(cast.Surveyor);

            Assert.That(cast.PlayerCollider.Distance(cast.Walker).isOverlapped, Is.False);
            Assert.That(cast.PlayerCollider.Distance(cast.Surveyor).isOverlapped, Is.False);
            Assert.That(CaptureCanonicalHorizontalHits(cast.PlayerCollider, selectedDirection), Does.Contain(selectedId));
            Assert.That(CaptureCanonicalHorizontalHits(cast.PlayerCollider, nonignoredDirection), Does.Contain(nonignoredId));

            StepPlayer(cast.Player);
            Assert.That(cast.Player.Snapshot.IsGrounded, Is.True, "The authored ground must remain in the production cast.");
            Assert.That(cast.Player.Snapshot.WallId, Is.EqualTo(selectedId), "The closer selected alive role must own the production horizontal hit.");

            Physics2D.IgnoreCollision(cast.PlayerCollider, selected, true);
            Assert.That(Physics2D.GetIgnoreCollision(cast.PlayerCollider, selected), Is.True, "Apply must be reflected immediately.");
            Assert.That(Physics2D.GetIgnoreCollision(cast.PlayerCollider, nonignored), Is.False);
            Assert.That(CaptureCanonicalHorizontalHits(cast.PlayerCollider, selectedDirection), Does.Contain(selectedId),
                "Unity raw Cast retains ignored pairs; the production Movement loop owns the filter.");
            Assert.That(CaptureCanonicalHorizontalHits(cast.PlayerCollider, nonignoredDirection), Does.Contain(nonignoredId));
            StepPlayer(cast.Player);
            Assert.That(cast.Player.Snapshot.IsGrounded, Is.True);
            Assert.That(cast.Player.Snapshot.WallId, Is.EqualTo(nonignoredId), "Only the ignored role may disappear from the production cast.");
            Assert.That(cast.Player.Snapshot.WallId, Is.EqualTo(selectedRole == "walker" ? surveyorId : walkerId));

            Physics2D.IgnoreCollision(cast.PlayerCollider, selected, false);
            Assert.That(Physics2D.GetIgnoreCollision(cast.PlayerCollider, selected), Is.False);
            StepPlayer(cast.Player);
            Assert.That(cast.Player.Snapshot.WallId, Is.EqualTo(selectedId),
                "Returning the exact pair false must restore eligibility on the next Movement query.");
        }

        // AC-COM-001, AC-COM-003 (partial); M3E1A gate 4. Environment remains
        // authoritative after both exact actor pairs are excluded.
        [Test]
        public void GroundAndEnvironmentWallRemainBlockingAfterBothPairIgnores()
        {
            var cast = CreateCastRig("walker");
            Physics2D.IgnoreCollision(cast.PlayerCollider, cast.Walker, true);
            Physics2D.IgnoreCollision(cast.PlayerCollider, cast.Surveyor, true);
            Assert.That(Physics2D.GetIgnoreCollision(cast.PlayerCollider, cast.Walker), Is.True);
            Assert.That(Physics2D.GetIgnoreCollision(cast.PlayerCollider, cast.Surveyor), Is.True);
            StepPlayer(cast.Player);
            Assert.That(cast.Player.Snapshot.IsGrounded, Is.True);
            MoveRightUntilEnvironmentBlocks(cast.Player);
            StepPlayer(cast.Player);
            Assert.That(cast.Player.Snapshot.IsGrounded, Is.True, "Pair ignores must not remove ground blocking.");
            Assert.That(cast.Player.Snapshot.VelocityXQ4096, Is.EqualTo(0));
            Assert.That(cast.Player.Snapshot.WallId,
                Is.EqualTo(PlayerMovementController.GetStableAuthoredId(cast.EnvironmentWall)));
            Assert.That(cast.PlayerCollider.Distance(cast.EnvironmentWall).isOverlapped, Is.False,
                "The nonignored environment wall must remain blocking after both actor-pair ignores.");
        }

        // AC-COM-001, AC-COM-003 (partial); M3E1A gate 5. IgnoreCollision must not
        // invalidate the already frozen M3C2 box or M3D2 capsule/box geometry.
        [Test]
        public void IgnoredPairsLeaveM3C2AndM3D2ImmutableViewsEqualToControl()
        {
            var control = CreateEncounterRig();
            Assert.DoesNotThrow(() => AdvanceFullSourceTick(control));
            Assert.DoesNotThrow(() => AdvanceFullSourceTick(control));
            var controlMotion = control.Locomotion.LatestCarriedViewForM3D.Value.Snapshot;
            var controlThreat = control.Threat.LatestCarriedViewForTests.Value.Snapshot;

            DestroyTrackedObjects();
            var ignored = CreateEncounterRig();
            Physics2D.IgnoreCollision(ignored.PlayerCollider, ignored.Walker.Collider, true);
            Physics2D.IgnoreCollision(ignored.PlayerCollider, ignored.Surveyor.Collider, true);

            Assert.DoesNotThrow(() => AdvanceFullSourceTick(ignored));
            Assert.DoesNotThrow(() => AdvanceFullSourceTick(ignored));
            Assert.That(ignored.Locomotion.LatestCarriedViewForM3D.Value.Snapshot, Is.EqualTo(controlMotion));
            Assert.That(ignored.Threat.LatestCarriedViewForTests.Value.Snapshot, Is.EqualTo(controlThreat));
            Assert.That(Physics2D.GetIgnoreCollision(ignored.PlayerCollider, ignored.Walker.Collider), Is.True);
            Assert.That(Physics2D.GetIgnoreCollision(ignored.PlayerCollider, ignored.Surveyor.Collider), Is.True);
        }

        // AC-COM-001, AC-COM-003 (partial); M3E1A gate 6 only. Gate 7 is deferred to
        // M3E1B0's committed authored graph and is intentionally not claimed here.
        [Test]
        public void DestroyedFixtureFreshEncounterUsesNewReferencesAndFalsePairState()
        {
            var first = CreateEncounterRig();
            var oldPlayer = first.PlayerCollider;
            var oldWalker = first.Walker.Collider;
            var oldSurveyor = first.Surveyor.Collider;
            Physics2D.IgnoreCollision(first.PlayerCollider, first.Walker.Collider, true);
            Physics2D.IgnoreCollision(first.PlayerCollider, first.Surveyor.Collider, true);
            Assert.That(Physics2D.GetIgnoreCollision(first.PlayerCollider, first.Walker.Collider), Is.True);
            Assert.That(Physics2D.GetIgnoreCollision(first.PlayerCollider, first.Surveyor.Collider), Is.True);

            DestroyTrackedObjects();
            Assert.That(oldPlayer == null, Is.True);
            Assert.That(oldWalker == null, Is.True);
            Assert.That(oldSurveyor == null, Is.True);
            var fresh = CreateEncounterRig();
            Assert.That(ReferenceEquals(fresh.PlayerCollider, oldPlayer), Is.False);
            Assert.That(ReferenceEquals(fresh.Walker.Collider, oldWalker), Is.False);
            Assert.That(ReferenceEquals(fresh.Surveyor.Collider, oldSurveyor), Is.False);
            Assert.That(Physics2D.GetIgnoreCollision(fresh.PlayerCollider, fresh.Walker.Collider), Is.False);
            Assert.That(Physics2D.GetIgnoreCollision(fresh.PlayerCollider, fresh.Surveyor.Collider), Is.False);
            Assert.That(fresh.Player.NextExpectedTick.Value, Is.EqualTo(0), "Fresh pair state is checked before the first movement publication.");
        }

        // AC-COM-001, AC-COM-003 (partial); M3E1A gate 8. Render cadence groups
        // the same 60 Hz phases and cannot alter pair state or the logical publications.
        [Test]
        public void ThirtySixtyAndOneFortyFourRenderGroupingProduceIdenticalLogicalTraces()
        {
            var thirty = RunGroupedTrace(30);
            var sixty = RunGroupedTrace(60);
            var oneFortyFour = RunGroupedTrace(144);
            Assert.That(sixty, Is.EqualTo(thirty));
            Assert.That(oneFortyFour, Is.EqualTo(thirty));
        }

        private CastRig CreateCastRig(string selectedRole)
        {
            if (selectedRole != "walker" && selectedRole != "surveyor") throw new ArgumentOutOfRangeException(nameof(selectedRole));
            var playerObject = Track(new GameObject("player"));
            playerObject.transform.position = new Vector2(0f, 0.81f);
            var playerBody = ConfigureKinematicBody(playerObject.AddComponent<Rigidbody2D>());
            var playerCollider = playerObject.AddComponent<CapsuleCollider2D>();
            playerCollider.direction = CapsuleDirection2D.Vertical;
            playerCollider.offset = Vector2.zero;
            playerCollider.size = new Vector2(0.8f, 1.6f);
            var player = playerObject.AddComponent<PlayerMovementController>();

            var ground = CreateEnvironmentBox("Ground", new Vector2(0f, -0.5f), new Vector2(12f, 1f));
            var wall = CreateEnvironmentBox("EnvironmentWall", new Vector2(2.5f, 1f), new Vector2(0.5f, 4f));
            var walkerX = selectedRole == "walker" ? 0.83f : 0.84f;
            var surveyorX = selectedRole == "surveyor" ? -0.83f : -0.84f;
            var walker = CreateSolidEnemyBox("walker", new Vector2(walkerX, 0.81f));
            var surveyor = CreateSolidEnemyBox("surveyor", new Vector2(surveyorX, 0.81f));
            _ = playerBody;
            _ = ground;
            Physics2D.SyncTransforms();
            return new CastRig(player, playerCollider, walker, surveyor, wall);
        }

        private EncounterRig CreateEncounterRig()
        {
            var playerObject = Track(new GameObject("player"));
            playerObject.transform.position = new Vector2(0f, 0.81f);
            ConfigureKinematicBody(playerObject.AddComponent<Rigidbody2D>());
            var playerCollider = playerObject.AddComponent<CapsuleCollider2D>();
            playerCollider.direction = CapsuleDirection2D.Vertical;
            playerCollider.offset = Vector2.zero;
            playerCollider.size = new Vector2(0.8f, 1.6f);
            var player = playerObject.AddComponent<PlayerMovementController>();
            var playerCombat = playerObject.AddComponent<CombatTarget>();
            playerCombat.ConfigureForTests("player", 5, 45, false, 0, 0, 0, 0, 100, 100, playerCollider, playerObject.transform);

            var walker = CreateEnemy("walker", new Vector2(3f, 0.81f), 9);
            var surveyor = CreateEnemy("surveyor", new Vector2(10f, 0.81f), 6);
            CreateEnvironmentBox("Ground", new Vector2(5f, -0.5f), new Vector2(32f, 1f));
            CreateEnvironmentBox("Bounds", new Vector2(16f, 2f), new Vector2(0.5f, 8f));

            var transferRegistry = Track(new GameObject("TransferRegistry")).AddComponent<TransferTargetRegistry>();
            transferRegistry.ConfigureForTests(surveyor.Transfer, walker.Transfer);
            var transfer = Track(new GameObject("TransferDriver")).AddComponent<TransferSimulationDriver>();
            transfer.ConfigureForTests(player, transferRegistry);
            transfer.InitializeForTests();

            var combatRegistry = Track(new GameObject("CombatRegistry")).AddComponent<CombatTargetRegistry>();
            combatRegistry.ConfigureForTests(playerCombat, surveyor.Combat, walker.Combat);
            var combat = Track(new GameObject("CombatDriver")).AddComponent<CombatSimulationDriver>();
            var reaction = Track(new GameObject("ReactionDriver")).AddComponent<RegularEnemyReactionSimulationDriver>();
            reaction.ConfigureForTests(player, transfer, combat);
            combat.ConfigureForTests(player, combatRegistry, transfer, reaction);
            var behavior = Track(new GameObject("BehaviorDriver")).AddComponent<RegularEnemyBehaviorSimulationDriver>();
            behavior.ConfigureForTests(player, combat, reaction);
            var locomotion = Track(new GameObject("LocomotionDriver")).AddComponent<RegularEnemyLocomotionSimulationDriver>();
            locomotion.ConfigureForTests(player, combat, reaction, behavior);
            var threatObject = Track(new GameObject("ThreatDriver"));
            threatObject.SetActive(false);
            var threat = threatObject.AddComponent<RegularEnemyThreatSimulationDriver>();
            threat.ConfigureForTests(player, combat, behavior, locomotion);
            Physics2D.SyncTransforms();
            return new EncounterRig(player, playerCollider, transferRegistry, transfer, combat, behavior, locomotion, threat, walker, surveyor);
        }

        private Enemy CreateEnemy(string id, Vector2 position, int health)
        {
            var value = Track(new GameObject(id));
            value.transform.position = position;
            var body = ConfigureKinematicBody(value.AddComponent<Rigidbody2D>());
            var collider = value.AddComponent<BoxCollider2D>();
            collider.offset = Vector2.zero;
            collider.size = new Vector2(0.8f, 1.6f);
            var sink = value.AddComponent<TransferSandboxOwnerStub>();
            sink.ConfigureForTests(id, "Target.Enemy.Heavy.v1");
            var transfer = value.AddComponent<TransferTarget>();
            transfer.ConfigureForTests(id, TransferTargetKind.Enemy, 0, 0, 0, 0, 100, 100,
                "Enemy.Regular.Baseline.v1", collider, value.transform, sink);
            var combat = value.AddComponent<CombatTarget>();
            combat.ConfigureForTests(id, health, 0, true, 0, 0, 0, 0, 100, 100,
                collider, value.transform, transfer);
            return new Enemy(body, collider, transfer, combat);
        }

        private BoxCollider2D CreateSolidEnemyBox(string name, Vector2 position)
        {
            var value = Track(new GameObject(name));
            value.transform.position = position;
            ConfigureKinematicBody(value.AddComponent<Rigidbody2D>());
            var collider = value.AddComponent<BoxCollider2D>();
            collider.offset = Vector2.zero;
            collider.size = new Vector2(0.8f, 1.6f);
            return collider;
        }

        private BoxCollider2D CreateEnvironmentBox(string name, Vector2 position, Vector2 size)
        {
            var value = Track(new GameObject(name));
            value.transform.position = position;
            var collider = value.AddComponent<BoxCollider2D>();
            collider.offset = Vector2.zero;
            collider.size = size;
            return collider;
        }

        private static Rigidbody2D ConfigureKinematicBody(Rigidbody2D body)
        {
            body.bodyType = RigidbodyType2D.Kinematic;
            body.simulated = true;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.interpolation = RigidbodyInterpolation2D.None;
            body.useFullKinematicContacts = false;
            body.rotation = 0f;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            return body;
        }

        private static void StepPlayer(PlayerMovementController player)
        {
            player.AdvanceOneFixedTickForTests();
            Physics2D.Simulate(1f / 60f);
        }

        private static void MoveRightUntilEnvironmentBlocks(PlayerMovementController player)
        {
            for (var index = 0; index < 120; index++)
            {
                player.SubmitCommand(new MovementCommand(player.NextExpectedTick, 4096, 0, false, false, false));
                StepPlayer(player);
                if (index > 0 && player.Snapshot.VelocityXQ4096 == 0) return;
            }

            Assert.Fail("The production movement cast did not reach the nonignored environment wall.");
        }

        private static IReadOnlyList<string> CaptureCanonicalHorizontalHits(CapsuleCollider2D playerCollider, Vector2 direction)
        {
            var filter = new ContactFilter2D();
            filter.NoFilter();
            filter.useTriggers = false;
            var buffer = new RaycastHit2D[32];
            var count = playerCollider.Cast(direction, filter, buffer, 410f / 4096f);
            Assert.That(count, Is.LessThan(buffer.Length), "A saturated cast cannot prove a complete canonical hit set.");
            var ids = new List<string>();
            for (var index = 0; index < count; index++)
            {
                var hit = buffer[index];
                if (hit.collider == null || hit.collider == playerCollider || hit.collider.isTrigger) continue;
                ids.Add(PlayerMovementController.GetStableAuthoredId(hit.collider));
            }

            ids.Sort(StringComparer.Ordinal);
            for (var index = 1; index < ids.Count; index++)
                Assert.That(ids[index], Is.Not.EqualTo(ids[index - 1]), "Canonical cast hits must be unique by stable authored ID.");
            return ids;
        }

        private static void AdvanceFullSourceTick(EncounterRig rig)
        {
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.AdvanceCombatPhaseForTests();
            rig.Behavior.AdvanceBehaviorPhaseForTests();
            var motion = rig.Locomotion.AdvanceLocomotionPhaseForTests();
            rig.Walker.Body.position = ToWorld(motion.Snapshot.Walker.Position);
            rig.Surveyor.Body.position = ToWorld(motion.Snapshot.Surveyor.Position);
            rig.Player.AdvanceOneFixedTickForTests();
            Physics2D.Simulate(1f / 60f);
            rig.Threat.AdvanceThreatPhaseForTests();
        }

        private string RunGroupedTrace(int renderRate)
        {
            var rig = CreateEncounterRig();
            var trace = new StringBuilder();
            var logicalStates = new HashSet<string>(StringComparer.Ordinal);
            var observedPairTransition = false;
            var completed = 0;
            var accumulator = 0;
            while (completed < 24)
            {
                accumulator = checked(accumulator + 60);
                while (accumulator >= renderRate && completed < 24)
                {
                    accumulator -= renderRate;
                    if (completed == 6)
                    {
                        Assert.That(rig.Player.NextExpectedTick.Value, Is.EqualTo(6));
                        Physics2D.IgnoreCollision(rig.PlayerCollider, rig.Walker.Collider, true);
                        Physics2D.IgnoreCollision(rig.PlayerCollider, rig.Surveyor.Collider, true);
                        observedPairTransition = Physics2D.GetIgnoreCollision(rig.PlayerCollider, rig.Walker.Collider) &&
                            Physics2D.GetIgnoreCollision(rig.PlayerCollider, rig.Surveyor.Collider);
                    }

                    AdvanceFullSourceTick(rig);
                    var motion = rig.Locomotion.LatestCarriedViewForM3D.Value.Snapshot;
                    var threat = rig.Threat.LatestCarriedViewForTests.Value.Snapshot;
                    logicalStates.Add(rig.Player.Snapshot.PositionXQ4096 + ":" + rig.Player.Snapshot.PositionYQ4096 + ":" +
                        motion.Walker.Position.X + ":" + motion.Walker.Position.Y + ":" +
                        motion.Surveyor.Position.X + ":" + motion.Surveyor.Position.Y + ":" +
                        threat.ExpectedShotOrdinal + ":" + threat.HasActiveLine);
                    trace.Append(rig.Player.Snapshot.Tick.Value).Append(':')
                        .Append(rig.Player.Snapshot.PositionXQ4096).Append(':')
                        .Append(rig.Player.Snapshot.PositionYQ4096).Append(':')
                        .Append(rig.Player.Snapshot.IsGrounded).Append(':')
                        .Append(motion.Walker.Position.X).Append(':').Append(motion.Walker.Position.Y).Append(':')
                        .Append(motion.Surveyor.Position.X).Append(':').Append(motion.Surveyor.Position.Y).Append(':')
                        .Append(threat.ExpectedShotOrdinal).Append(':').Append(threat.HasActiveLine).Append(':')
                        .Append(Physics2D.GetIgnoreCollision(rig.PlayerCollider, rig.Walker.Collider)).Append(':')
                        .Append(Physics2D.GetIgnoreCollision(rig.PlayerCollider, rig.Surveyor.Collider)).Append('|');
                    completed++;
                }
            }

            Assert.That(observedPairTransition, Is.True, "The pair flags must change at exact logical tick 6 in every cadence replay.");
            Assert.That(logicalStates.Count, Is.GreaterThan(1), "Cadence evidence must contain changing gameplay state, not only an empty/static loop.");
            var value = trace.ToString();
            DestroyTrackedObjects();
            return value;
        }

        private static Vector2 ToWorld(PositionQ4096 value) => new Vector2(value.X / 4096f, value.Y / 4096f);

        private GameObject Track(GameObject value)
        {
            _objects.Add(value);
            return value;
        }

        private void DestroyTrackedObjects()
        {
            for (var index = _objects.Count - 1; index >= 0; index--)
            {
                UnityEngine.Object.DestroyImmediate(_objects[index]);
            }

            _objects.Clear();
            Physics2D.SyncTransforms();
        }

        private readonly struct CastRig
        {
            internal CastRig(PlayerMovementController player, CapsuleCollider2D playerCollider,
                BoxCollider2D walker, BoxCollider2D surveyor, BoxCollider2D environmentWall)
            {
                Player = player;
                PlayerCollider = playerCollider;
                Walker = walker;
                Surveyor = surveyor;
                EnvironmentWall = environmentWall;
            }

            internal PlayerMovementController Player { get; }
            internal CapsuleCollider2D PlayerCollider { get; }
            internal BoxCollider2D Walker { get; }
            internal BoxCollider2D Surveyor { get; }
            internal BoxCollider2D EnvironmentWall { get; }
        }

        private readonly struct Enemy
        {
            internal Enemy(Rigidbody2D body, BoxCollider2D collider, TransferTarget transfer, CombatTarget combat)
            {
                Body = body;
                Collider = collider;
                Transfer = transfer;
                Combat = combat;
            }

            internal Rigidbody2D Body { get; }
            internal BoxCollider2D Collider { get; }
            internal TransferTarget Transfer { get; }
            internal CombatTarget Combat { get; }
        }

        private readonly struct EncounterRig
        {
            internal EncounterRig(PlayerMovementController player, CapsuleCollider2D playerCollider,
                TransferTargetRegistry transferRegistry, TransferSimulationDriver transfer, CombatSimulationDriver combat,
                RegularEnemyBehaviorSimulationDriver behavior, RegularEnemyLocomotionSimulationDriver locomotion,
                RegularEnemyThreatSimulationDriver threat, Enemy walker, Enemy surveyor)
            {
                Player = player;
                PlayerCollider = playerCollider;
                TransferRegistry = transferRegistry;
                Transfer = transfer;
                Combat = combat;
                Behavior = behavior;
                Locomotion = locomotion;
                Threat = threat;
                Walker = walker;
                Surveyor = surveyor;
            }

            internal PlayerMovementController Player { get; }
            internal CapsuleCollider2D PlayerCollider { get; }
            internal TransferTargetRegistry TransferRegistry { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal CombatSimulationDriver Combat { get; }
            internal RegularEnemyBehaviorSimulationDriver Behavior { get; }
            internal RegularEnemyLocomotionSimulationDriver Locomotion { get; }
            internal RegularEnemyThreatSimulationDriver Threat { get; }
            internal Enemy Walker { get; }
            internal Enemy Surveyor { get; }
        }
    }
}
