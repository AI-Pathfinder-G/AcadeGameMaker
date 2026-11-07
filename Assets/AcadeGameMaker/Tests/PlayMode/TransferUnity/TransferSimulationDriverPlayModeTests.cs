// AC-WT-001, AC-WT-003, AC-WT-006; partial AC-WT-002/AC-WT-005 owner-sink evidence only. AC-MOV-004.
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.TransferUnity
{
    internal sealed class TransferSimulationDriverPlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private SimulationMode2D _previousSimulationMode;

        [SetUp] public void SetUp() { _previousSimulationMode = Physics2D.simulationMode; Physics2D.simulationMode = SimulationMode2D.Script; }
        [TearDown] public void TearDown() { for (var i = _objects.Count - 1; i >= 0; i--) UnityEngine.Object.DestroyImmediate(_objects[i]); _objects.Clear(); Physics2D.simulationMode = _previousSimulationMode; Physics2D.SyncTransforms(); }

        // AC-WT-001. The transfer bridge is explicitly scheduled before default-order movement.
        [Test]
        public void DriverHasFrozenMinusTwoHundredExecutionOrder()
        {
            var attribute = (DefaultExecutionOrder)Attribute.GetCustomAttribute(typeof(TransferSimulationDriver), typeof(DefaultExecutionOrder));
            Assert.That(attribute, Is.Not.Null); Assert.That(attribute.order, Is.EqualTo(-200));
        }

        // AC-WT-001, AC-WT-003, AC-WT-006; AC-MOV-004. The -200 bridge captures once, commits once, and reflects the final Lightweight state before movement tick 1.
        [Test]
        public void DriverCapturesOnceAndReflectsLightweightExactlyOnce()
        {
            var player = CreatePlayer();
            var box = CreateBoxTarget("box", out var sink);
            var driver = CreateDriver(player, box);
            driver.AdvanceTransferPhaseForTests();
            player.AdvanceOneFixedTickForTests();
            driver.Submit(Input(1, new TransferPressed(1, new SimulationTick(1))));

            var outcome = driver.AdvanceTransferPhaseForTests();
            Assert.That(outcome.PressResult.Value.Success, Is.True);
            Assert.That(driver.SyncCaptureCountForTests, Is.EqualTo(1));
            Assert.That(driver.FinalReflectionCountForTests, Is.EqualTo(2));
            Assert.That(player.GetComponent<Rigidbody2D>().gravityScale, Is.EqualTo(3.1315f * 0.65f).Within(0.00001f));
            Assert.That(sink.IsAppliedForTests, Is.True);
        }

        // Partial AC-WT-002, AC-WT-005. Box ownership alone restores its captured base mass/gravity and reversible impact multiplier.
        [Test]
        public void BoxOwnerSinkAppliesAndClearsOnlyItsOwnReversiblePhysics()
        {
            var target = Track(new GameObject("BoxOwner"));
            var body = target.AddComponent<Rigidbody2D>(); body.mass = 2f; body.gravityScale = 1.5f;
            var sink = target.AddComponent<TransferSandboxBoxOwnerSink>(); sink.ConfigureForTests("box", body);
            var request = new TransferModifierRequest("box", TransferTargetKind.Box, "base", "Target.Box.Heavy.v1", new SimulationTick(4), 1);
            Assert.That(sink.ImpactDamageMultiplierQ1000, Is.EqualTo(1000));
            Assert.That(sink.TryApply(request), Is.True);
            Assert.That(body.mass, Is.EqualTo(6f)); Assert.That(body.gravityScale, Is.EqualTo(3f)); Assert.That(sink.ImpactDamageMultiplierQ1000, Is.EqualTo(1500));
            sink.Clear(TransferClearReason.ManualRecall, new SimulationTick(25), 2);
            Assert.That(body.mass, Is.EqualTo(2f)); Assert.That(body.gravityScale, Is.EqualTo(1.5f)); Assert.That(sink.ImpactDamageMultiplierQ1000, Is.EqualTo(1000)); Assert.That(sink.IsAppliedForTests, Is.False);
        }

        // Partial AC-WT-002, AC-WT-005. The three frozen target modifier IDs remain isolated owner decisions.
        [Test]
        public void OwnerSinksAcceptOnlyTheirFrozenModifierIds()
        {
            var enemy = Track(new GameObject("Enemy")).AddComponent<TransferSandboxOwnerStub>(); enemy.ConfigureForTests("enemy", "Target.Enemy.Heavy.v1");
            var boss = Track(new GameObject("Boss")).AddComponent<TransferSandboxOwnerStub>(); boss.ConfigureForTests("boss", "Target.BossPayload.ScriptedHeavy.v1");
            Assert.That(enemy.TryApply(new TransferModifierRequest("enemy", TransferTargetKind.Enemy, "base", "Target.Enemy.Heavy.v1", new SimulationTick(0), 1)), Is.True);
            Assert.That(boss.TryApply(new TransferModifierRequest("boss", TransferTargetKind.BossPayload, "base", "Target.BossPayload.ScriptedHeavy.v1", new SimulationTick(0), 1)), Is.True);
            Assert.That(enemy.IsAppliedForTests, Is.True); Assert.That(boss.IsAppliedForTests, Is.True);
        }

        // AC-WT-001, AC-WT-005. A false target apply reports ModifierRejected without changing player gravity, target body, or revision.
        [Test]
        public void RejectedTargetApplyLeavesSessionMovementAndTargetUnchanged()
        {
            var player = CreatePlayer();
            var target = CreateBoxTarget("box", out var sink);
            var driver = CreateDriver(player, target);
            driver.AdvanceTransferPhaseForTests(); player.AdvanceOneFixedTickForTests();
            sink.Clear(TransferClearReason.RoomLeaving, new SimulationTick(0), 0); // retained base state; next request is explicitly rejected by duplicate installed handle.
            sink.TryApply(new TransferModifierRequest("box", TransferTargetKind.Box, "base", "Target.Box.Heavy.v1", new SimulationTick(0), 1));
            var mass = target.GetComponent<Rigidbody2D>().mass; var gravity = player.GetComponent<Rigidbody2D>().gravityScale;
            driver.Submit(Input(1, new TransferPressed(1, new SimulationTick(1))));
            var outcome = driver.AdvanceTransferPhaseForTests();
            Assert.That(outcome.PressResult.Value.FailureReason, Is.EqualTo(TransferFailureReason.ModifierRejected));
            Assert.That(outcome.Snapshot.Revision, Is.EqualTo(0)); Assert.That(player.GetComponent<Rigidbody2D>().gravityScale, Is.EqualTo(gravity)); Assert.That(target.GetComponent<Rigidbody2D>().mass, Is.EqualTo(mass));
        }

        // AC-WT-001, AC-WT-005, AC-WT-006; AC-MOV-004. Active removal clears its owner, preserves the prior apply lock, and leaves the same-tick press visible as Cooldown.
        [Test]
        public void ActiveRemovalClearsThenPreservesSameTickPressCooldown()
        {
            var player = CreatePlayer(); var target = CreateBoxTarget("box", out var sink); var driver = CreateDriver(player, target);
            driver.AdvanceTransferPhaseForTests(); player.AdvanceOneFixedTickForTests();
            driver.Submit(Input(1, new TransferPressed(1, new SimulationTick(1)))); driver.AdvanceTransferPhaseForTests(); player.AdvanceOneFixedTickForTests();
            var removal = new TransferTargetRemoved("box", new SimulationTick(2));
            driver.Submit(new TransferSimulationInput(new SimulationTick(2), null, null, new TransferPressed(1, new SimulationTick(2)), new[] { removal }, null));
            var outcome = driver.AdvanceTransferPhaseForTests();
            Assert.That(outcome.RemovalResult.Value.Cleared.Value.Reason, Is.EqualTo(TransferClearReason.TargetRemoved));
            Assert.That(outcome.PressResult.Value.FailureReason, Is.EqualTo(TransferFailureReason.Cooldown));
            Assert.That(outcome.Snapshot.ActiveTargetId, Is.Empty); Assert.That(sink.IsAppliedForTests, Is.False);
            Assert.That(player.GetComponent<Rigidbody2D>().gravityScale, Is.EqualTo(3.1315f).Within(0.00001f));
        }

        // AC-WT-001, AC-WT-005. Lifecycle consumes input, clears the active owner, and publishes the actual Baseline result.
        [Test]
        public void LifecycleClearsActiveOwnerAndConsumesSameTickPress()
        {
            var player = CreatePlayer(); var target = CreateBoxTarget("box", out var sink); var driver = CreateDriver(player, target);
            driver.AdvanceTransferPhaseForTests(); player.AdvanceOneFixedTickForTests();
            driver.Submit(Input(1, new TransferPressed(1, new SimulationTick(1)))); driver.AdvanceTransferPhaseForTests(); player.AdvanceOneFixedTickForTests();
            driver.Submit(new TransferSimulationInput(new SimulationTick(2), null, null, new TransferPressed(1, new SimulationTick(2)), Array.Empty<TransferTargetRemoved>(), TransferClearReason.RoomLeaving));
            var outcome = driver.AdvanceTransferPhaseForTests();
            Assert.That(outcome.RemovalResult.Value.Cleared.Value.Reason, Is.EqualTo(TransferClearReason.RoomLeaving)); Assert.That(outcome.PressResult.HasValue, Is.False);
            Assert.That(outcome.Snapshot.PlayerState, Is.EqualTo(TransferPlayerState.Baseline)); Assert.That(sink.IsAppliedForTests, Is.False);
        }

        // AC-WT-003, AC-WT-005. The private scripted-exposure lane is
        // registered before bootstrap, merges only after an exact completed
        // publication, and does not permanently remove the descriptor.
        [Test]
        public void ExposureEndLaneUsesRegisteredOwnerAndFutureTick()
        {
            var player = CreatePlayer(); var targets = CreateLaneTargets();
            var registry = Track(new GameObject("ExposureRegistry")).AddComponent<TransferTargetRegistry>(); registry.ConfigureForTests(targets);
            var driver = Track(new GameObject("ExposureDriver")).AddComponent<TransferSimulationDriver>(); driver.ConfigureForTests(player, registry);
            var owner = new object(); driver.RegisterExposureEndLane(owner, LaneIds());
            driver.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => driver.MergeExposureEnds(new object(), new[] { new TransferTargetExposureEnded("BossWeight-0", new SimulationTick(1)) }));
            Assert.Throws<InvalidOperationException>(() => driver.MergeExposureEnds(owner, new[] { new TransferTargetExposureEnded("not-registered", new SimulationTick(1)) }));
            driver.MergeExposureEnds(owner, new[] { new TransferTargetExposureEnded("BossWeight-0", new SimulationTick(1)), new TransferTargetExposureEnded("BossWeight-0", new SimulationTick(1)) });
            player.AdvanceOneFixedTickForTests();
            Assert.DoesNotThrow(() => driver.AdvanceTransferPhaseForTests());
        }

        // AC-WT-003, AC-WT-005. Both future merge directions retain every
        // existing input field and defensively copy/dedupe the temporary end.
        [Test]
        public void ExposureAndPermanentFutureMergesAreCommutativeAndPreserveQueuedInput()
        {
            var player = CreatePlayer(); var targets = CreateLaneTargets(); var registry = Track(new GameObject("MergeRegistry")).AddComponent<TransferTargetRegistry>(); registry.ConfigureForTests(targets);
            var driver = Track(new GameObject("MergeDriver")).AddComponent<TransferSimulationDriver>(); driver.ConfigureForTests(player, registry); var owner = new object(); driver.RegisterExposureEndLane(owner, LaneIds());
            var tickOne = new SimulationTick(1); var original = new TransferSimulationInput(tickOne, Camera(0), Mouse(1), new TransferPressed(1, tickOne), Array.Empty<TransferTargetRemoved>(), TransferClearReason.RoomLeaving);
            driver.Submit(original); driver.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => driver.MergeExposureEnds(owner, new[] { new TransferTargetExposureEnded("BossWeight-0", new SimulationTick(0)) }));
            var ends = new[] { new TransferTargetExposureEnded("BossWeight-0", tickOne), new TransferTargetExposureEnded("BossWeight-0", tickOne) };
            driver.MergeExposureEnds(owner, ends); driver.MergeRemovals(new[] { new TransferTargetRemoved("BossWeight-0", tickOne) }); ends[0] = new TransferTargetExposureEnded("altered", tickOne);
            var queued = Queued(driver, 1);
            Assert.That(queued.Camera, Is.EqualTo(original.Camera)); Assert.That(queued.Aim, Is.EqualTo(original.Aim)); Assert.That(queued.Press, Is.EqualTo(original.Press)); Assert.That(queued.Lifecycle, Is.EqualTo(TransferClearReason.RoomLeaving));
            Assert.That(queued.ExposureEnds.Count, Is.EqualTo(1)); Assert.That(queued.ExposureEnds[0].TargetId, Is.EqualTo("BossWeight-0")); Assert.That(queued.Removals.Count, Is.EqualTo(1));
            var playerReverse = CreatePlayer(); var reverseTargets = CreateLaneTargets(); var reverseRegistry = Track(new GameObject("MergeReverseRegistry")).AddComponent<TransferTargetRegistry>(); reverseRegistry.ConfigureForTests(reverseTargets);
            var reverse = Track(new GameObject("MergeReverseDriver")).AddComponent<TransferSimulationDriver>(); reverse.ConfigureForTests(playerReverse, reverseRegistry); var reverseOwner = new object(); reverse.RegisterExposureEndLane(reverseOwner, LaneIds()); reverse.Submit(original); reverse.AdvanceTransferPhaseForTests();
            reverse.MergeRemovals(new[] { new TransferTargetRemoved("BossWeight-0", tickOne) }); reverse.MergeExposureEnds(reverseOwner, new[] { new TransferTargetExposureEnded("BossWeight-0", tickOne) }); var reverseQueued = Queued(reverse, 1);
            Assert.That(reverseQueued.Removals.Count, Is.EqualTo(queued.Removals.Count)); Assert.That(reverseQueued.Removals[0].TargetId, Is.EqualTo(queued.Removals[0].TargetId)); Assert.That(reverseQueued.ExposureEnds.Count, Is.EqualTo(queued.ExposureEnds.Count)); Assert.That(reverseQueued.ExposureEnds[0].TargetId, Is.EqualTo(queued.ExposureEnds[0].TargetId)); Assert.That(reverseQueued.Camera, Is.EqualTo(queued.Camera)); Assert.That(reverseQueued.Lifecycle, Is.EqualTo(queued.Lifecycle));
            Assert.That(reverseQueued.Aim, Is.EqualTo(queued.Aim)); Assert.That(reverseQueued.Press, Is.EqualTo(queued.Press));
        }

        // AC-WT-001, AC-WT-006. Render grouping only batches the same 60 Hz driver phases; snapshots and state events replay identically.
        [Test]
        public void ThirtySixtyAndOneFortyFourRenderGroupsReplayIdenticalDriverOutcomes()
        {
            var thirty = ReplayAtRenderRate(30); var sixty = ReplayAtRenderRate(60); var oneFortyFour = ReplayAtRenderRate(144);
            CollectionAssert.AreEqual(thirty, sixty); CollectionAssert.AreEqual(thirty, oneFortyFour);
        }

        // AC-WT-003, AC-WT-006; AC-MOV-004. Checked capture overflow happens before session publication, target application, and final movement reflection.
        [Test]
        public void CaptureOverflowLeavesSessionMovementAndTargetUnchanged()
        {
            var player = CreatePlayer(); var target = CreateBoxTarget("box", out var sink); var driver = CreateDriver(player, target);
            driver.AdvanceTransferPhaseForTests(); player.AdvanceOneFixedTickForTests();
            var before = driver.SnapshotForTests; var gravity = player.GetComponent<Rigidbody2D>().gravityScale; var mass = target.GetComponent<Rigidbody2D>().mass;
            var camera = new SimulationCameraPoseSnapshot(new SimulationTick(0), int.MaxValue, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
            driver.Submit(new TransferSimulationInput(new SimulationTick(1), camera, Mouse(1, 1919), new TransferPressed(1, new SimulationTick(1)), Array.Empty<TransferTargetRemoved>(), null));
            Assert.Throws<OverflowException>(() => driver.AdvanceTransferPhaseForTests());
            var after = driver.SnapshotForTests;
            Assert.That(after.Revision, Is.EqualTo(before.Revision)); Assert.That(after.ActiveTargetId, Is.EqualTo(before.ActiveTargetId));
            Assert.That(player.GetComponent<Rigidbody2D>().gravityScale, Is.EqualTo(gravity)); Assert.That(target.GetComponent<Rigidbody2D>().mass, Is.EqualTo(mass)); Assert.That(sink.IsAppliedForTests, Is.False);
        }

        private TransferSimulationDriver CreateDriver(PlayerMovementController player, TransferTarget target)
        {
            var registry = Track(new GameObject("Registry")).AddComponent<TransferTargetRegistry>(); registry.ConfigureForTests(target);
            var driver = Track(new GameObject("TransferDriver")).AddComponent<TransferSimulationDriver>(); driver.ConfigureForTests(player, registry); driver.InitializeForTests(); return driver;
        }
        private PlayerMovementController CreatePlayer()
        {
            var value = Track(new GameObject("Player")); var body = value.AddComponent<Rigidbody2D>(); body.bodyType = RigidbodyType2D.Kinematic; value.AddComponent<CapsuleCollider2D>(); return value.AddComponent<PlayerMovementController>();
        }
        private TransferTarget CreateBoxTarget(string id, out TransferSandboxBoxOwnerSink sink)
        {
            var value = Track(new GameObject(id)); value.transform.position = new Vector2(1f, 0f); var body = value.AddComponent<Rigidbody2D>(); body.mass = 2f; body.gravityScale = 1f; var collider = value.AddComponent<BoxCollider2D>();
            sink = value.AddComponent<TransferSandboxBoxOwnerSink>(); sink.ConfigureForTests(id, body); var target = value.AddComponent<TransferTarget>(); target.ConfigureForTests(id, TransferTargetKind.Box, 0, 0, 0, 0, 100, 100, "base", collider, value.transform, sink); return target;
        }
        private TransferTarget[] CreateLaneTargets()
        {
            var targets=new TransferTarget[3];for(var index=0;index<3;index++)targets[index]=CreateBoxTarget("BossWeight-"+index,out _);return targets;
        }
        private static string[] LaneIds() => new[] { "BossWeight-0", "BossWeight-1", "BossWeight-2" };
        private static TransferSimulationInput Input(int tick, TransferPressed press) => new TransferSimulationInput(new SimulationTick(tick), Camera(tick - 1), Mouse(tick), press, Array.Empty<TransferTargetRemoved>(), null);
        private List<string> ReplayAtRenderRate(int renderFramesPerSecond)
        {
            var player = CreatePlayer(); var target = CreateBoxTarget("replay-box", out _); var driver = CreateDriver(player, target); var results = new List<string>(); var allowance = 0d;
            while (results.Count < 23)
            {
                allowance += 60d / renderFramesPerSecond;
                while (allowance >= 1d && results.Count < 23)
                {
                    var tick = results.Count;
                    if (tick == 1) driver.Submit(Input(tick, new TransferPressed(1, new SimulationTick(tick))));
                    if (tick == 22) driver.Submit(new TransferSimulationInput(new SimulationTick(tick), null, null, new TransferPressed(1, new SimulationTick(tick)), Array.Empty<TransferTargetRemoved>(), null));
                    var outcome = driver.AdvanceTransferPhaseForTests();
                    results.Add(SnapshotTrace(outcome.Snapshot) + "|removal=" + EventTrace(outcome.RemovalResult) + "|press=" + EventTrace(outcome.PressResult));
                    player.AdvanceOneFixedTickForTests(); allowance -= 1d;
                }
            }

            return results;
        }
        private static SimulationCameraPoseSnapshot Camera(int tick) => new SimulationCameraPoseSnapshot(new SimulationTick(tick), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
        // Camera() maps the target at world X=1 to screen X=1014 for the frozen 1920x1080, half-height-10 projection.
        private static AimSample Mouse(int tick, int screenX = 1014) => new AimSample(1, AimSource.MousePointer, screenX, 540, true, 4096, 0, new SimulationTick(tick - 1), new SimulationTick(tick));
        private static string EventTrace(TransferAttemptResult? result)
        {
            if (!result.HasValue) return "none";
            var value = result.Value;
            var changed = value.StateChanged.HasValue ? StateChangedTrace(value.StateChanged.Value) : "none";
            var cleared = value.Cleared.HasValue ? ClearedTrace(value.Cleared.Value) : "none";
            return "tick=" + value.Tick.Value +
                ",aim=" + (value.AimSampleId.HasValue ? value.AimSampleId.Value.ToString() : "null") +
                ",selected=" + value.SelectedTargetId +
                ",success=" + value.Success +
                ",failure=" + value.FailureReason +
                ",session={" + SnapshotTrace(value.Session) +
                "},changed={" + changed +
                "},cleared={" + cleared + "}";
        }
        private static string SnapshotTrace(TransferSessionSnapshot value) =>
            "tick=" + value.Tick.Value +
            ",highlighted=" + value.HighlightedTargetId +
            ",active=" + value.ActiveTargetId +
            ",player=" + value.PlayerState +
            ",lockEnd=" + value.TransitionLockEndTick +
            ",lastFailure=" + value.LastFailureReason +
            ",revision=" + value.Revision;
        private static string StateChangedTrace(TransferStateChanged value) =>
            "target=" + value.TargetId +
            ",playerModifier=" + value.PlayerModifierId +
            ",targetModifier=" + value.TargetModifierId +
            ",tick=" + value.Tick.Value +
            ",revision=" + value.Revision;
        private static string ClearedTrace(TransferCleared value) =>
            "reason=" + value.Reason +
            ",previous=" + value.PreviousTargetId +
            ",tick=" + value.Tick.Value +
                ",revision=" + value.Revision;
        private static TransferSimulationInput Queued(TransferSimulationDriver driver, int tick) =>
            (TransferSimulationInput)((IDictionary)typeof(TransferSimulationDriver).GetField("_inputs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(driver))[tick];
        private GameObject Track(GameObject value) { _objects.Add(value); return value; }
    }
}
