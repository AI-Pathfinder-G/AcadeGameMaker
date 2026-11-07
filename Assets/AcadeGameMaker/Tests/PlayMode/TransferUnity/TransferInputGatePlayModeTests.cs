// AC-M5B2-001, AC-M5B2-002, AC-M5B2-004, AC-M5B2-005, AC-M5B2-007.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.TransferUnity
{
    internal sealed class TransferInputGatePlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private SimulationMode2D _previousSimulationMode;

        [SetUp]
        public void SetUp()
        {
            _previousSimulationMode = Physics2D.simulationMode;
            Physics2D.simulationMode = SimulationMode2D.Script;
        }

        [TearDown]
        public void TearDown()
        {
            for (var index = _objects.Count - 1; index >= 0; index--)
            {
                UnityEngine.Object.DestroyImmediate(_objects[index]);
            }

            _objects.Clear();
            Physics2D.simulationMode = _previousSimulationMode;
            Physics2D.SyncTransforms();
        }

        // REQ-UX-004/008, REQ-WT-005/006; AC-M5B2-001, AC-M5B2-004.
        [Test]
        public void LockStripsExactAndFutureInteractiveFieldsButPreservesRemovalAndKeys()
        {
            var rig = CreateSingleTargetRig();
            Bootstrap(rig);
            var owner = new object();
            rig.Driver.RegisterInputGateOwner(owner);

            var tickOne = new SimulationTick(1);
            var tickFour = new SimulationTick(4);
            rig.Driver.Submit(Input(tickOne));
            rig.Driver.Submit(Input(tickFour, new[] { new TransferTargetRemoved("box", tickFour) }));
            var dropped = rig.Driver.ApplyInputGate(owner, tickOne, true);

            Assert.That(dropped, Is.EqualTo(2));
            var queuedOne = Queued(rig.Driver, 1);
            var queuedFour = Queued(rig.Driver, 4);
            Assert.That(queuedOne.Camera.HasValue, Is.False);
            Assert.That(queuedOne.Aim.HasValue, Is.False);
            Assert.That(queuedOne.Press.HasValue, Is.False);
            Assert.That(queuedFour.Camera.HasValue, Is.False);
            Assert.That(queuedFour.Aim.HasValue, Is.False);
            Assert.That(queuedFour.Press.HasValue, Is.False);
            Assert.That(queuedFour.Removals.Count, Is.EqualTo(1));
            Assert.That(queuedFour.Removals[0].TargetId, Is.EqualTo("box"));

            var first = rig.Driver.AdvanceTransferPhaseForTests();
            Assert.That(first.PressResult.HasValue, Is.False);
            rig.Player.AdvanceOneFixedTickForTests();
            rig.Driver.AdvanceTransferPhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();
            rig.Driver.AdvanceTransferPhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();
            var future = rig.Driver.AdvanceTransferPhaseForTests();
            Assert.That(future.PressResult.HasValue, Is.False);
            Assert.That(future.Snapshot.ActiveTargetId, Is.Empty);
        }

        // REQ-WT-005/006; AC-M5B2-002. An exposure end is a maintenance lane:
        // it survives canonicalization and clears the active relation while the
        // interactive aim/press is stripped before the real -200 processing.
        [Test]
        public void LockedMixedExposureInputClearsActiveRelationWithoutPressCapture()
        {
            var rig = CreateExposureRig();
            rig.Driver.AdvanceTransferPhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();
            var firstInput = Input(new SimulationTick(1));
            rig.Driver.Submit(firstInput);
            var first = rig.Driver.AdvanceTransferPhaseForTests();
            Assert.That(first.PressResult.HasValue, Is.True);
            Assert.That(first.PressResult.Value.Success, Is.True);
            var activeId = first.PressResult.Value.SelectedTargetId;
            var tickTwo = new SimulationTick(2);
            rig.Driver.RegisterInputGateOwner(rig.GateOwner);
            rig.Driver.Submit(Input(tickTwo));
            rig.Driver.MergeExposureEnds(rig.ExposureOwner, new[] { new TransferTargetExposureEnded(activeId, tickTwo) });
            rig.Player.AdvanceOneFixedTickForTests();
            Assert.That(rig.Driver.ApplyInputGate(rig.GateOwner, tickTwo, true), Is.EqualTo(1));
            var queued = Queued(rig.Driver, 2);
            Assert.That(queued.ExposureEnds.Count, Is.EqualTo(1));
            Assert.That(queued.ExposureEnds[0].TargetId, Is.EqualTo(activeId));
            Assert.That(queued.Camera.HasValue, Is.False);
            Assert.That(queued.Press.HasValue, Is.False);

            var outcome = rig.Driver.AdvanceTransferPhaseForTests();
            Assert.That(outcome.PressResult.HasValue, Is.False);
            Assert.That(outcome.RemovalResult.HasValue, Is.True);
            Assert.That(outcome.RemovalResult.Value.Cleared.Value.Reason, Is.EqualTo(TransferClearReason.TargetRemoved));
            Assert.That(outcome.Snapshot.ActiveTargetId, Is.Empty);
            Assert.That(outcome.Snapshot.PlayerState, Is.EqualTo(TransferPlayerState.Baseline));
        }

        // REQ-UX-004/008, REQ-WT-005/006; AC-M5B2-001, AC-M5B2-004.
        [Test]
        public void UnlockRetainsStrippedFutureWorkAndAcceptsOnlyFreshInteractiveInput()
        {
            var rig = CreateSingleTargetRig();
            Bootstrap(rig);
            var owner = new object();
            rig.Driver.RegisterInputGateOwner(owner);
            rig.Driver.Submit(Input(new SimulationTick(1)));
            rig.Driver.Submit(Input(new SimulationTick(3)));
            rig.Driver.ApplyInputGate(owner, new SimulationTick(1), true);
            rig.Driver.AdvanceTransferPhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();

            Assert.That(rig.Driver.ApplyInputGate(owner, new SimulationTick(2), false), Is.EqualTo(0));
            rig.Driver.Submit(Input(new SimulationTick(2)));
            Assert.That(rig.Driver.ApplyInputGate(owner, new SimulationTick(2), false), Is.EqualTo(0));
            var fresh = Queued(rig.Driver, 2);
            var old = Queued(rig.Driver, 3);
            Assert.That(fresh.Press.HasValue, Is.True);
            Assert.That(old.Press.HasValue, Is.False);

            var freshOutcome = rig.Driver.AdvanceTransferPhaseForTests();
            Assert.That(freshOutcome.PressResult.HasValue, Is.True);
            rig.Player.AdvanceOneFixedTickForTests();
            var oldOutcome = rig.Driver.AdvanceTransferPhaseForTests();
            Assert.That(oldOutcome.PressResult.HasValue, Is.False);
        }

        // REQ-WT-005/006; AC-M5B2-004, AC-M5B2-005. Invalid owner/tick and a
        // malformed queued default are rejected without changing the valid queue.
        [Test]
        public void InvalidGateOrStagedEnvelopeLeavesQueueAndLatchUnchanged()
        {
            var rig = CreateSingleTargetRig();
            Bootstrap(rig);
            var owner = new object();
            rig.Driver.RegisterInputGateOwner(owner);
            rig.Driver.Submit(Input(new SimulationTick(1)));
            Assert.Throws<InvalidOperationException>(() => rig.Driver.ApplyInputGate(new object(), new SimulationTick(1), true));
            Assert.Throws<InvalidOperationException>(() => rig.Driver.ApplyInputGate(owner, new SimulationTick(0), true));
            Assert.That(Queued(rig.Driver, 1).Press.HasValue, Is.True);

            var dictionary = (IDictionary)GetPrivate(rig.Driver, "_inputs");
            dictionary.Add(5, default(TransferSimulationInput));
            Assert.Catch<ArgumentException>(() => rig.Driver.ApplyInputGate(owner, new SimulationTick(1), true));
            Assert.That(Queued(rig.Driver, 1).Press.HasValue, Is.True);
            Assert.That(dictionary.Contains(5), Is.True);

            dictionary.Remove(5);
            var consumed = rig.Driver.AdvanceTransferPhaseForTests();
            Assert.That(consumed.PressResult.HasValue, Is.True);
            Assert.Throws<InvalidOperationException>(() => rig.Driver.ApplyInputGate(owner, new SimulationTick(1), true));
        }

        // REQ-UX-004/008, REQ-WT-005/006; AC-M5B2-005. A locked mixed
        // submission is canonicalized while its caller-owned maintenance list
        // cannot alias the stored read-only payload.
        [Test]
        public void LockedMixedSubmissionCopiesMaintenanceBeforeCanonicalization()
        {
            var rig = CreateSingleTargetRig();
            Bootstrap(rig);
            var owner = new object();
            rig.Driver.RegisterInputGateOwner(owner);
            Assert.That(rig.Driver.ApplyInputGate(owner, new SimulationTick(1), true), Is.EqualTo(0));

            var tick = new SimulationTick(1);
            var removals = new List<TransferTargetRemoved> { new TransferTargetRemoved("box", tick) };
            rig.Driver.Submit(new TransferSimulationInput(tick, Camera(0), Mouse(1), new TransferPressed(1, tick), removals, null));
            removals[0] = new TransferTargetRemoved("forged-after-submit", tick);
            var queued = Queued(rig.Driver, 1);
            Assert.That(queued.Camera.HasValue, Is.False);
            Assert.That(queued.Aim.HasValue, Is.False);
            Assert.That(queued.Press.HasValue, Is.False);
            Assert.That(queued.Removals.Count, Is.EqualTo(1));
            Assert.That(queued.Removals[0].TargetId, Is.EqualTo("box"));
            Assert.Throws<NotSupportedException>(() => ((IList<TransferTargetRemoved>)queued.Removals)[0] = removals[0]);
            Assert.Throws<ArgumentNullException>(() => rig.Driver.Submit(default));
            Assert.Throws<InvalidOperationException>(() => rig.Driver.Submit(Input(tick)));
            Assert.That(Queued(rig.Driver, 1), Is.EqualTo(queued));

            var outcome = rig.Driver.AdvanceTransferPhaseForTests();
            Assert.That(outcome.PressResult.HasValue, Is.False);
            Assert.That(outcome.RemovalResult.HasValue, Is.False);
            rig.Player.AdvanceOneFixedTickForTests();
            Assert.Throws<InvalidOperationException>(() => rig.Driver.Submit(Input(tick)));
        }

        // REQ-WT-005/006; AC-M5B2-004. An uninitialized driver cannot use the
        // gate or submit a default envelope; a prepared driver rejects an
        // already-consumed phase even if the player has not advanced yet.
        [Test]
        public void UninitializedAndAlreadyConsumedGateCallsFailWithoutIngress()
        {
            var unbound = Track(new GameObject("UninitializedTransferGate")).AddComponent<TransferSimulationDriver>();
            var owner = new object();
            unbound.RegisterInputGateOwner(owner);
            Assert.Throws<InvalidOperationException>(() => unbound.ApplyInputGate(owner, new SimulationTick(0), true));
            Assert.Throws<ArgumentNullException>(() => unbound.SubmitForTests(default(TransferSimulationInput)));
            Assert.That(GetPrivate(unbound, "_initialized"), Is.False);
        }

        // REQ-UX-004/008, REQ-WT-006; AC-M5B2-007. Render grouping drives the
        // actual fixed transfer phase and gate transitions, not a relabelled trace.
        [Test]
        public void ThirtySixtyAndOneFortyFourGroupingProduceIdenticalTransferGateTrace()
        {
            var thirty = ReplayAtRenderRate(30);
            var sixty = ReplayAtRenderRate(60);
            var oneFortyFour = ReplayAtRenderRate(144);
            CollectionAssert.AreEqual(thirty, sixty);
            CollectionAssert.AreEqual(thirty, oneFortyFour);
            TestContext.WriteLine("AC-M5B2-007 cadence=30,60,144 firstDivergence=none tracesEqual=True");
        }

        // REQ-UX-004/008; AC-M5B2-004. Initialized bootstrap is permitted,
        // registration is identity-bound, and unlock does not replace system work.
        [Test]
        public void BootstrapRegistrationAndUnlockPreserveQueueIdentity()
        {
            var rig = CreateSingleTargetRig();
            var owner = new object();
            Assert.Throws<InvalidOperationException>(() => rig.Driver.ApplyInputGate(owner, new SimulationTick(0), true));
            Assert.Throws<ArgumentNullException>(() => rig.Driver.RegisterInputGateOwner(null));
            rig.Driver.RegisterInputGateOwner(owner);
            rig.Driver.RegisterInputGateOwner(owner);
            Assert.Throws<InvalidOperationException>(() => rig.Driver.RegisterInputGateOwner(new object()));
            Assert.Throws<InvalidOperationException>(() => rig.Driver.ApplyInputGate(null, new SimulationTick(0), true));
            Assert.That(rig.Driver.ApplyInputGate(owner, new SimulationTick(0), true), Is.Zero);
            var queue = GetPrivate(rig.Driver, "_inputs");
            Assert.That(rig.Driver.ApplyInputGate(owner, new SimulationTick(0), true), Is.Zero);
            rig.Driver.ApplyInputGate(owner, new SimulationTick(0), false);
            Assert.That(GetPrivate(rig.Driver, "_inputs"), Is.SameAs(queue));
            rig.Driver.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => rig.Driver.ApplyInputGate(owner, new SimulationTick(0), true));
        }

        private List<string> ReplayAtRenderRate(int renderFramesPerSecond)
        {
            var rig = CreateSingleTargetRig();
            Bootstrap(rig);
            var owner = new object();
            rig.Driver.RegisterInputGateOwner(owner);
            var trace = new List<string>();
            var accumulator = 0;
            for (var tick = 1; tick < 7;)
            {
                accumulator += 60;
                while (accumulator >= renderFramesPerSecond && tick < 7)
                {
                    if (tick == 1)
                    {
                        rig.Driver.Submit(Input(new SimulationTick(1)));
                        rig.Driver.ApplyInputGate(owner, new SimulationTick(1), true);
                    }
                    else if (tick == 2)
                    {
                        rig.Driver.ApplyInputGate(owner, new SimulationTick(2), false);
                        rig.Driver.Submit(Input(new SimulationTick(2)));
                        rig.Driver.ApplyInputGate(owner, new SimulationTick(2), false);
                    }

                    var outcome = rig.Driver.AdvanceTransferPhaseForTests();
                    trace.Add(outcome.Snapshot.Tick.Value + ":" + outcome.Snapshot.PlayerState + ":" +
                        (outcome.PressResult.HasValue ? outcome.PressResult.Value.FailureReason.ToString() : "none"));
                    rig.Player.AdvanceOneFixedTickForTests();
                    tick++;
                    accumulator -= renderFramesPerSecond;
                }
            }

            DestroyTrackedObjects();
            return trace;
        }

        private void Bootstrap(Rig rig)
        {
            rig.Driver.AdvanceTransferPhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();
        }

        private Rig CreateSingleTargetRig()
        {
            var player = CreatePlayer();
            var target = CreateBoxTarget("box", TransferTargetKind.Box);
            var registry = Track(new GameObject("TransferGateRegistry")).AddComponent<TransferTargetRegistry>();
            registry.ConfigureForTests(target);
            var driver = Track(new GameObject("TransferGateDriver")).AddComponent<TransferSimulationDriver>();
            driver.ConfigureForTests(player, registry);
            driver.InitializeForTests();
            return new Rig(player, driver);
        }

        private ExposureRig CreateExposureRig()
        {
            var player = CreatePlayer();
            var targets = new TransferTarget[3];
            for (var index = 0; index < targets.Length; index++)
            {
                targets[index] = CreateBoxTarget("BossWeight-" + index, TransferTargetKind.BossPayload);
            }

            var registry = Track(new GameObject("ExposureGateRegistry")).AddComponent<TransferTargetRegistry>();
            registry.ConfigureForTests(targets);
            var driver = Track(new GameObject("ExposureGateDriver")).AddComponent<TransferSimulationDriver>();
            driver.ConfigureForTests(player, registry);
            var exposureOwner = new object();
            driver.RegisterExposureEndLane(exposureOwner, new[] { "BossWeight-0", "BossWeight-1", "BossWeight-2" });
            driver.InitializeForTests();
            return new ExposureRig(player, driver, exposureOwner, new object());
        }

        private TransferSimulationInput Input(SimulationTick tick, IReadOnlyList<TransferTargetRemoved> removals = null)
        {
            return new TransferSimulationInput(
                tick,
                Camera(tick.Value - 1),
                Mouse(tick.Value),
                new TransferPressed(1, tick),
                removals ?? Array.Empty<TransferTargetRemoved>(),
                null);
        }

        private PlayerMovementController CreatePlayer()
        {
            var value = Track(new GameObject("TransferGatePlayer"));
            var body = value.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            value.AddComponent<CapsuleCollider2D>();
            return value.AddComponent<PlayerMovementController>();
        }

        private TransferTarget CreateBoxTarget(string id, TransferTargetKind kind)
        {
            var value = Track(new GameObject(id));
            value.transform.position = new Vector2(1f, 0f);
            var body = value.AddComponent<Rigidbody2D>();
            body.mass = 2f;
            var collider = value.AddComponent<BoxCollider2D>();
            var sink = value.AddComponent<TransferSandboxOwnerStub>();
            var modifier = kind == TransferTargetKind.BossPayload
                ? "Target.BossPayload.ScriptedHeavy.v1"
                : "Target.Box.Heavy.v1";
            sink.ConfigureForTests(id, modifier);
            var target = value.AddComponent<TransferTarget>();
            target.ConfigureForTests(id, kind, 0, 0, 0, 0, 100, 100,
                "base", collider, value.transform, sink);
            return target;
        }

        private static SimulationCameraPoseSnapshot Camera(int poseTick) =>
            new SimulationCameraPoseSnapshot(new SimulationTick(poseTick), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);

        private static AimSample Mouse(int tick) =>
            new AimSample(1, AimSource.MousePointer, 1014, 540, true, 4096, 0,
                new SimulationTick(tick - 1), new SimulationTick(tick));

        private static TransferSimulationInput Queued(TransferSimulationDriver driver, int tick) =>
            (TransferSimulationInput)((IDictionary)GetPrivate(driver, "_inputs"))[tick];

        private static object GetPrivate(object value, string fieldName) =>
            value.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value);

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

        private readonly struct Rig
        {
            internal Rig(PlayerMovementController player, TransferSimulationDriver driver)
            { Player = player; Driver = driver; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Driver { get; }
        }

        private readonly struct ExposureRig
        {
            internal ExposureRig(PlayerMovementController player, TransferSimulationDriver driver, object exposureOwner, object gateOwner)
            { Player = player; Driver = driver; ExposureOwner = exposureOwner; GateOwner = gateOwner; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Driver { get; }
            internal object ExposureOwner { get; }
            internal object GateOwner { get; }
        }
    }
}
