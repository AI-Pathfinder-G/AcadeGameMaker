// AC-M5B2-001, AC-M5B2-003, AC-M5B2-004, AC-M5B2-005, AC-M5B2-007.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    internal sealed class OrdanBossCombatInputGatePlayModeTests
    {
        // REQ-COM-001/004, REQ-UX-004/008; AC-M5B2-001, AC-M5B2-003.
        [Test]
        public void LockStripsExactAndFutureAttackFieldsButPreservesExternalDamageAndDeliveryHorizon()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var owner = new object();
                rig.Combat.RegisterInputGateOwner(owner);
                var tickZero = new SimulationTick(0);
                var tickTwo = new SimulationTick(2);
                var zeroRequest = Damage(tickZero, "gate.external.zero");
                var twoRequest = Damage(tickTwo, "gate.external.two");
                rig.Combat.Submit(Input(tickZero, true, zeroRequest));
                rig.Combat.Submit(Input(tickTwo, true, twoRequest));

                Assert.That(rig.Combat.ApplyInputGate(owner, tickZero, true), Is.EqualTo(2));
                var queuedZero = Queued(rig.Combat, 0);
                var queuedTwo = Queued(rig.Combat, 2);
                Assert.That(queuedZero.Camera.HasValue, Is.False);
                Assert.That(queuedZero.Aim.HasValue, Is.False);
                Assert.That(queuedZero.Press.HasValue, Is.False);
                Assert.That(queuedZero.ExternalDamageRequests.Count, Is.EqualTo(1));
                Assert.That(queuedTwo.Camera.HasValue, Is.False);
                Assert.That(queuedTwo.Aim.HasValue, Is.False);
                Assert.That(queuedTwo.Press.HasValue, Is.False);
                Assert.That(queuedTwo.ExternalDamageRequests.Count, Is.EqualTo(1));

                var zero = Advance(rig);
                Assert.That(zero.Attack.DamageRequest.HasValue, Is.False);
                Assert.That(ContainsRequest(zero.CanonicalRequestTrace, zeroRequest.RequestId), Is.True);
                Assert.That(Health(zero, "player"), Is.EqualTo(4));

                var tickOne = new SimulationTick(1);
                rig.Combat.ApplyInputGate(owner, tickOne, false);
                rig.Transfer.Submit(new TransferSimulationInput(tickOne, CameraFor(tickOne), AimFor(tickOne), null,
                    Array.Empty<TransferTargetRemoved>(), null));
                rig.Combat.Submit(Input(tickOne, true, null));
                Assert.That(rig.Combat.ApplyInputGate(owner, tickOne, false), Is.EqualTo(0));
                var fresh = Queued(rig.Combat, 1);
                Assert.That(fresh.Camera.HasValue, Is.True);
                Assert.That(fresh.Press.HasValue, Is.True);
                var freshOutcome = Advance(rig);
                Assert.That(freshOutcome.Attack.DamageRequest.HasValue, Is.True);
                Assert.That(Health(freshOutcome, "ordan"), Is.EqualTo(57));

                var two = Advance(rig);
                Assert.That(two.Attack.DamageRequest.HasValue, Is.False);
                Assert.That(ContainsRequest(two.CanonicalRequestTrace, twoRequest.RequestId), Is.True);
            }
            finally
            {
                DestroyAll(objects);
            }
        }

        // REQ-COM-001/004, REQ-UX-004/008; AC-M5B2-003, AC-M5B2-005.
        [Test]
        public void LockedMixedSubmissionCanonicalizesToMaintenanceOnlyAndStillProcessesDamage()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var owner = new object();
                rig.Combat.RegisterInputGateOwner(owner);
                rig.Combat.ApplyInputGate(owner, new SimulationTick(0), true);
                var request = Damage(new SimulationTick(0), "gate.locked.mixed");
                rig.Combat.Submit(Input(new SimulationTick(0), true, request));
                var queued = Queued(rig.Combat, 0);
                Assert.That(queued.Camera.HasValue, Is.False);
                Assert.That(queued.Aim.HasValue, Is.False);
                Assert.That(queued.Press.HasValue, Is.False);
                Assert.That(queued.ExternalDamageRequests.Count, Is.EqualTo(1));

                var outcome = Advance(rig);
                Assert.That(outcome.Attack.DamageRequest.HasValue, Is.False);
                Assert.That(ContainsRequest(outcome.CanonicalRequestTrace, request.RequestId), Is.True);
                Assert.That(Health(outcome, "player"), Is.EqualTo(4));
            }
            finally
            {
                DestroyAll(objects);
            }
        }

        // REQ-COM-004, REQ-UX-004/008; AC-M5B2-004, AC-M5B2-005.
        [Test]
        public void InvalidOwnerTickAndMalformedStagingPreserveCombatQueueAndLatch()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var owner = new object();
                rig.Combat.RegisterInputGateOwner(owner);
                rig.Combat.Submit(Input(new SimulationTick(0), true, null));
                rig.Combat.Submit(Input(new SimulationTick(2), true, null));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.ApplyInputGate(new object(), new SimulationTick(0), true));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.ApplyInputGate(owner, new SimulationTick(1), true));
                Assert.That(Queued(rig.Combat, 0).Press.HasValue, Is.True);

                var dictionary = (IDictionary)GetPrivate(rig.Combat, "_inputs");
                dictionary.Add(4, default(OrdanBossCombatInput));
                Assert.Catch<ArgumentException>(() => rig.Combat.ApplyInputGate(owner, new SimulationTick(0), true));
                Assert.That(Queued(rig.Combat, 0).Press.HasValue, Is.True);
                Assert.That(dictionary.Contains(4), Is.True);

                dictionary.Remove(4);
                Assert.That(rig.Combat.ApplyInputGate(owner, new SimulationTick(0), true), Is.EqualTo(2));
                Assert.That(rig.Combat.ApplyInputGate(owner, new SimulationTick(0), true), Is.EqualTo(0));
            }
            finally
            {
                DestroyAll(objects);
            }
        }

        // REQ-COM-001/004, REQ-UX-004/008; AC-M5B2-007. Each cadence drives
        // the actual Transfer -> Combat -> Bridge fixed phases.
        [Test]
        public void ThirtySixtyAndOneFortyFourGroupingProduceIdenticalCombatGateTrace()
        {
            var thirty = ReplayAtRenderRate(30);
            var sixty = ReplayAtRenderRate(60);
            var oneFortyFour = ReplayAtRenderRate(144);
            CollectionAssert.AreEqual(thirty, sixty);
            CollectionAssert.AreEqual(thirty, oneFortyFour);
            TestContext.WriteLine("AC-M5B2-007 cadence=30,60,144 firstDivergence=none tracesEqual=True");
        }

        // REQ-UX-004/008; AC-M5B2-004. Validation is not allowed to prepare a
        // missing owner or claim a lock after the consumer has already run.
        [Test]
        public void GateRejectsMissingOwnerPreparationAndConsumedCombatPhase()
        {
            var objects = new List<GameObject>();
            try
            {
                var owner = new object();
                var unprepared = Track(objects, "unprepared-gate").AddComponent<OrdanBossCombatSimulationDriver>();
                unprepared.RegisterInputGateOwner(owner);
                Assert.Throws<InvalidOperationException>(() => unprepared.ApplyInputGate(owner, new SimulationTick(0), true));
                Assert.That(GetPrivate(unprepared, "_prepared"), Is.False);

                var rig = CreateRig(objects);
                Assert.Throws<InvalidOperationException>(() => rig.Combat.ApplyInputGate(owner, new SimulationTick(0), true));
                Assert.Throws<ArgumentNullException>(() => rig.Combat.RegisterInputGateOwner(null));
                rig.Combat.RegisterInputGateOwner(owner);
                rig.Combat.RegisterInputGateOwner(owner);
                Assert.Throws<InvalidOperationException>(() => rig.Combat.RegisterInputGateOwner(new object()));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.ApplyInputGate(null, new SimulationTick(0), true));
                Assert.That(rig.Combat.ApplyInputGate(owner, new SimulationTick(0), true), Is.Zero);
                var queue = GetPrivate(rig.Combat, "_inputs");
                rig.Combat.ApplyInputGate(owner, new SimulationTick(0), false);
                Assert.That(GetPrivate(rig.Combat, "_inputs"), Is.SameAs(queue));
                rig.Transfer.AdvanceTransferPhaseForTests();
                rig.Combat.AdvanceCombatPhaseForTests();
                Assert.Throws<InvalidOperationException>(() => rig.Combat.ApplyInputGate(owner, new SimulationTick(0), true));
                Assert.That(GetPrivate(rig.Combat, "_inputGateLocked"), Is.False);
            }
            finally { DestroyAll(objects); }
        }

        // REQ-COM-004, REQ-UX-008; AC-M5B2-005. Source collections and exposed
        // DTO lists cannot mutate retained damage; invalid ingress stays atomic.
        [Test]
        public void DamageCollectionsAreReadOnlyAndInvalidIngressPreservesQueuedWork()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var owner = new object();
                rig.Combat.RegisterInputGateOwner(owner);
                rig.Combat.ApplyInputGate(owner, new SimulationTick(0), true);
                var tick = new SimulationTick(0);
                var request = Damage(tick, "read-only-damage");
                var source = new[] { request };
                var input = new OrdanBossCombatInput(tick, CameraFor(tick), AimFor(tick), new BasicAttackPressed(7, 700, tick), source);
                source[0] = Damage(tick, "changed-source");
                Assert.Throws<NotSupportedException>(() => ((IList<DamageRequest>)input.ExternalDamageRequests)[0] = source[0]);
                rig.Combat.Submit(input);
                var before = Queued(rig.Combat, 0);
                Assert.Throws<NotSupportedException>(() => ((IList<DamageRequest>)before.ExternalDamageRequests)[0] = source[0]);
                Assert.That(before.ExternalDamageRequests[0].RequestId, Is.EqualTo(request.RequestId));
                Assert.Throws<ArgumentException>(() => rig.Combat.Submit(default));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.Submit(input));
                Assert.That(Queued(rig.Combat, 0), Is.EqualTo(before));
                var outcome = Advance(rig);
                Assert.That(Health(outcome, "player"), Is.EqualTo(4));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.Submit(input));
            }
            finally { DestroyAll(objects); }
        }

        // REQ-UX-004/006/008, REQ-COM-001/004; AC-M5B4-001/003/004.
        [Test]
        public void CombatLockCandidateIsMutationFreeAndPreservesExternalDamage()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var owner = new object();
                rig.Combat.RegisterInputGateOwner(owner);
                var tick = new SimulationTick(0);
                var future = new SimulationTick(2);
                var request = Damage(tick, "candidate.damage.zero");
                var futureRequest = Damage(future, "candidate.damage.future");
                rig.Combat.Submit(Input(tick, true, request));
                rig.Combat.Submit(Input(future, true, futureRequest));
                var queue = GetPrivate(rig.Combat, "_inputs");

                var fakeIssuer = new object();
                var forged = new OrdanBossCombatSimulationDriver.InputFrameCandidate(
                    rig.Combat, fakeIssuer, owner, tick,
                    (long)GetPrivate(rig.Combat, "_inputGeneration"),
                    (long)GetPrivate(rig.Combat, "_inputGeneration"),
                    new SortedDictionary<int, OrdanBossCombatInput>(), false, 0);
                Assert.Throws<InvalidOperationException>(() => forged.ApplyToMatchingConsumer(rig.Combat, fakeIssuer, owner));
                Assert.That(GetPrivate(rig.Combat, "_inputs"), Is.SameAs(queue));

                var candidate = rig.Combat.PrepareInputFrame(owner, tick, true, null);
                Assert.That(GetPrivate(rig.Combat, "_inputs"), Is.SameAs(queue));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.ValidateInputFrameCandidate(new object(), candidate));
                Assert.That(GetPrivate(rig.Combat, "_inputGateLocked"), Is.False);
                Assert.That(rig.Combat.CommitInputFrame(owner, candidate), Is.EqualTo(2));
                Assert.That(GetPrivate(rig.Combat, "_inputGateLocked"), Is.True);

                var zero = Queued(rig.Combat, 0);
                var futureInput = Queued(rig.Combat, 2);
                Assert.That(zero.Camera.HasValue, Is.False);
                Assert.That(zero.Aim.HasValue, Is.False);
                Assert.That(zero.Press.HasValue, Is.False);
                Assert.That(zero.ExternalDamageRequests.Count, Is.EqualTo(1));
                Assert.That(futureInput.ExternalDamageRequests.Count, Is.EqualTo(1));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.CommitInputFrame(owner, candidate));

                var afterCommit = GetPrivate(rig.Combat, "_inputs");
                var afterCommitCount = ((IDictionary)afterCommit).Count;
                SetPrivate(rig.Combat, "_inputGeneration", long.MaxValue);
                Assert.Throws<OverflowException>(() => rig.Combat.Submit(Input(new SimulationTick(4), false, null)));
                Assert.That(GetPrivate(rig.Combat, "_inputs"), Is.SameAs(afterCommit));
                Assert.That(((IDictionary)GetPrivate(rig.Combat, "_inputs")).Count, Is.EqualTo(afterCommitCount));
            }
            finally { DestroyAll(objects); }
        }

        // REQ-UX-004/006/008; AC-M5B4-001/005. All three sinks validate before
        // any commit; the failed third validation leaves every latch untouched.
        [Test]
        public void ThreeConsumerFrameValidatesAsOneAtomicBoundary()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var movementOwner = new object();
                var transferOwner = new object();
                var combatOwner = new object();
                rig.Player.RegisterInputGateOwner(movementOwner);
                rig.Transfer.RegisterInputGateOwner(transferOwner);
                rig.Combat.RegisterInputGateOwner(combatOwner);
                var tick = new SimulationTick(0);
                var movementCandidate = rig.Player.PrepareInputFrame(movementOwner, tick, true, null);
                var transferCandidate = rig.Transfer.PrepareInputFrame(transferOwner, tick, true, null);
                var combatCandidate = rig.Combat.PrepareInputFrame(combatOwner, tick, true, null);

                rig.Player.ValidateInputFrameCandidate(movementOwner, movementCandidate);
                rig.Transfer.ValidateInputFrameCandidate(transferOwner, transferCandidate);
                Assert.Throws<InvalidOperationException>(() => rig.Combat.ValidateInputFrameCandidate(new object(), combatCandidate));
                Assert.That(GetPrivate(rig.Player, "_inputLocked"), Is.False);
                Assert.That(GetPrivate(rig.Transfer, "_inputGateLocked"), Is.False);
                Assert.That(GetPrivate(rig.Combat, "_inputGateLocked"), Is.False);

                rig.Combat.ValidateInputFrameCandidate(combatOwner, combatCandidate);
                rig.Player.CommitInputFrame(movementOwner, movementCandidate);
                rig.Transfer.CommitInputFrame(transferOwner, transferCandidate);
                rig.Combat.CommitInputFrame(combatOwner, combatCandidate);
                Assert.That(GetPrivate(rig.Player, "_inputLocked"), Is.True);
                Assert.That(GetPrivate(rig.Transfer, "_inputGateLocked"), Is.True);
                Assert.That(GetPrivate(rig.Combat, "_inputGateLocked"), Is.True);
            }
            finally { DestroyAll(objects); }
        }

        // REQ-UX-004/006/008; AC-M5B4-001/004/005. Every rejection position
        // preserves queue identity/content, latch, generation and motor state.
        [TestCase(0, false)] [TestCase(1, false)] [TestCase(2, false)]
        [TestCase(0, true)] [TestCase(1, true)] [TestCase(2, true)]
        public void TrioRejectsEveryPrepareAndValidationPositionUnchanged(int position, bool duringValidation)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var owner = new object();
                RegisterFrameOwner(rig, owner);
                var tick = new SimulationTick(0);
                rig.Player.SubmitCommand(new MovementCommand(tick, 4096, 0, true, false, true));
                rig.Transfer.Submit(new TransferSimulationInput(tick, null, null, new TransferPressed(null, tick), Array.Empty<TransferTargetRemoved>(), null));
                rig.Combat.Submit(Input(tick, true, null));
                var consumers = FrameConsumers(rig);
                var queues = consumers.Select((value, i) => GetPrivate(value, i == 0 ? "_commands" : "_inputs")).ToArray();
                var generations = consumers.Select(value => GetPrivate(value, "_inputGeneration")).ToArray();
                var snapshot = rig.Player.Snapshot;
                PlayerMovementController.InputFrameCandidate movement = null;
                TransferSimulationDriver.InputFrameCandidate transfer = null;
                OrdanBossCombatSimulationDriver.InputFrameCandidate combat = null;
                var prepares = new Action[]
                {
                    () => movement = rig.Player.PrepareInputFrame(!duringValidation && position == 0 ? new object() : owner, tick, true, null),
                    () => transfer = rig.Transfer.PrepareInputFrame(!duringValidation && position == 1 ? new object() : owner, tick, true, null),
                    () => combat = rig.Combat.PrepareInputFrame(!duringValidation && position == 2 ? new object() : owner, tick, true, null)
                };
                if (duringValidation)
                {
                    foreach (var prepare in prepares) prepare();
                    var validations = new Action[]
                    {
                        () => rig.Player.ValidateInputFrameCandidate(position == 0 ? new object() : owner, movement),
                        () => rig.Transfer.ValidateInputFrameCandidate(position == 1 ? new object() : owner, transfer),
                        () => rig.Combat.ValidateInputFrameCandidate(position == 2 ? new object() : owner, combat)
                    };
                    Assert.Throws<InvalidOperationException>(() => { foreach (var validate in validations) validate(); });
                }
                else Assert.Throws<InvalidOperationException>(() => { foreach (var prepare in prepares) prepare(); });
                for (var i = 0; i < consumers.Length; i++)
                {
                    var queue = GetPrivate(consumers[i], i == 0 ? "_commands" : "_inputs");
                    Assert.That(queue, Is.SameAs(queues[i]));
                    Assert.That(((IDictionary)queue).Count, Is.EqualTo(1));
                    Assert.That(((IDictionary)queue).Contains(0), Is.True);
                    Assert.That(GetPrivate(consumers[i], "_inputGeneration"), Is.EqualTo(generations[i]));
                    Assert.That(GetPrivate(consumers[i], i == 0 ? "_inputLocked" : "_inputGateLocked"), Is.False);
                }
                Assert.That(rig.Player.Snapshot, Is.EqualTo(snapshot));
            }
            finally { DestroyAll(objects); }
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void NoOpFrameStillInvalidatesParallelCandidate(int index)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects); var owner = new object(); RegisterFrameOwner(rig, owner);
                var consumer = FrameConsumers(rig)[index]; var tick = new SimulationTick(0);
                var before = (long)GetPrivate(consumer, "_inputGeneration");
                if (index == 0)
                {
                    var a = rig.Player.PrepareInputFrame(owner, tick, false, null);
                    var b = rig.Player.PrepareInputFrame(owner, tick, false, null);
                    rig.Player.CommitInputFrame(owner, a);
                    Assert.Throws<InvalidOperationException>(() => rig.Player.CommitInputFrame(owner, b));
                }
                else if (index == 1)
                {
                    var a = rig.Transfer.PrepareInputFrame(owner, tick, false, null);
                    var b = rig.Transfer.PrepareInputFrame(owner, tick, false, null);
                    rig.Transfer.CommitInputFrame(owner, a);
                    Assert.Throws<InvalidOperationException>(() => rig.Transfer.CommitInputFrame(owner, b));
                }
                else
                {
                    var a = rig.Combat.PrepareInputFrame(owner, tick, false, null);
                    var b = rig.Combat.PrepareInputFrame(owner, tick, false, null);
                    rig.Combat.CommitInputFrame(owner, a);
                    Assert.Throws<InvalidOperationException>(() => rig.Combat.CommitInputFrame(owner, b));
                }
                Assert.That((long)GetPrivate(consumer, "_inputGeneration"), Is.EqualTo(before + 1));
            }
            finally { DestroyAll(objects); }
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void CandidateAndLegacyGateOverflowPreserveAllInputState(int index)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects); var owner = new object(); RegisterFrameOwner(rig, owner);
                var consumer = FrameConsumers(rig)[index]; var tick = new SimulationTick(0);
                var queue = (IDictionary)GetPrivate(consumer, index == 0 ? "_commands" : "_inputs");
                SetPrivate(consumer, "_inputGeneration", long.MaxValue);
                Action prepare = index == 0 ? (Action)(() => rig.Player.PrepareInputFrame(owner, tick, true, null))
                    : index == 1 ? () => rig.Transfer.PrepareInputFrame(owner, tick, true, null)
                    : () => rig.Combat.PrepareInputFrame(owner, tick, true, null);
                Action apply = index == 0 ? (Action)(() => rig.Player.ApplyInputGate(owner, tick, true))
                    : index == 1 ? () => rig.Transfer.ApplyInputGate(owner, tick, true)
                    : () => rig.Combat.ApplyInputGate(owner, tick, true);
                Assert.Throws<OverflowException>(() => prepare());
                Assert.Throws<OverflowException>(() => apply());
                Assert.That(GetPrivate(consumer, index == 0 ? "_commands" : "_inputs"), Is.SameAs(queue));
                Assert.That(queue.Count, Is.Zero);
                Assert.That(GetPrivate(consumer, index == 0 ? "_inputLocked" : "_inputGateLocked"), Is.False);
                Assert.That((long)GetPrivate(consumer, "_inputGeneration"), Is.EqualTo(long.MaxValue));
            }
            finally { DestroyAll(objects); }
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void InvalidTailQueueKeyRejectsWithoutPartialNormalization(int index)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects); var owner = new object(); RegisterFrameOwner(rig, owner);
                var consumer = FrameConsumers(rig)[index]; var tick = new SimulationTick(0);
                var queue = (IDictionary)GetPrivate(consumer, index == 0 ? "_commands" : "_inputs");
                if (index == 0)
                {
                    queue.Add(0, new MovementCommand(tick, 4096, 0, true, false, true));
                    queue.Add(2, new MovementCommand(new SimulationTick(3), 0, 0, false, false, false));
                }
                else if (index == 1)
                {
                    queue.Add(0, new TransferSimulationInput(tick, null, null, new TransferPressed(null, tick), Array.Empty<TransferTargetRemoved>(), null));
                    queue.Add(2, new TransferSimulationInput(new SimulationTick(3), null, null, null, Array.Empty<TransferTargetRemoved>(), null));
                }
                else { queue.Add(0, Input(tick, true, null)); queue.Add(2, Input(new SimulationTick(3), false, null)); }
                var first = queue[0]; var generation = GetPrivate(consumer, "_inputGeneration");
                Action prepare = index == 0 ? (Action)(() => rig.Player.PrepareInputFrame(owner, tick, true, null))
                    : index == 1 ? () => rig.Transfer.PrepareInputFrame(owner, tick, true, null)
                    : () => rig.Combat.PrepareInputFrame(owner, tick, true, null);
                Assert.Throws<InvalidOperationException>(() => prepare());
                Assert.That(GetPrivate(consumer, index == 0 ? "_commands" : "_inputs"), Is.SameAs(queue));
                Assert.That(queue.Count, Is.EqualTo(2)); Assert.That(queue[0], Is.EqualTo(first));
                Assert.That(GetPrivate(consumer, "_inputGeneration"), Is.EqualTo(generation));
                Assert.That(GetPrivate(consumer, index == 0 ? "_inputLocked" : "_inputGateLocked"), Is.False);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void FrameCompositionRetainsSystemFieldsAndRejectsInteractiveOverwrite()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects); var owner = new object(); RegisterFrameOwner(rig, owner);
                var tick = new SimulationTick(0);
                var removals = new[] { new TransferTargetRemoved("system.target", tick) };
                rig.Transfer.Submit(new TransferSimulationInput(tick, null, null, null, removals, TransferClearReason.RoomLeaving));
                var damage = Damage(tick, "frame.external");
                rig.Combat.Submit(Input(tick, false, damage));
                var transferInput = new TransferSimulationInput(tick, null, null, new TransferPressed(null, tick), Array.Empty<TransferTargetRemoved>(), null);
                rig.Transfer.CommitInputFrame(owner, rig.Transfer.PrepareInputFrame(owner, tick, false, transferInput));
                rig.Combat.CommitInputFrame(owner, rig.Combat.PrepareInputFrame(owner, tick, false, Input(tick, true, null)));
                var transfer = (TransferSimulationInput)((IDictionary)GetPrivate(rig.Transfer, "_inputs"))[0];
                Assert.That(transfer.Removals[0].TargetId, Is.EqualTo("system.target"));
                Assert.That(transfer.Lifecycle, Is.EqualTo(TransferClearReason.RoomLeaving));
                Assert.That(transfer.Press.HasValue, Is.True);
                Assert.That(Queued(rig.Combat, 0).ExternalDamageRequests[0].RequestId, Is.EqualTo("frame.external"));
                Assert.That(Queued(rig.Combat, 0).Press.HasValue, Is.True);
                Assert.Throws<InvalidOperationException>(() => rig.Transfer.PrepareInputFrame(owner, tick, false, transferInput));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.PrepareInputFrame(owner, tick, false, Input(tick, true, null)));
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void TerminalDiscardGenerationOverflowPreservesDeliveryAndInput()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                rig.Transfer.AdvanceTransferPhaseForTests();
                rig.Combat.AdvanceCombatPhaseForTests();
                rig.Bridge.AdvanceBridgePhaseForTests();
                rig.Player.AdvanceOneFixedTickForTests();
                rig.Combat.Submit(Input(new SimulationTick(1), false, null));
                var owner = new object();
                var candidate = rig.Combat.PreflightTerminalDeliveryDiscard(owner, new SimulationTick(0));
                var deliveries = (IDictionary)GetPrivate(rig.Combat, "_deliveries");
                var inputs = (IDictionary)GetPrivate(rig.Combat, "_inputs");
                var delivery = deliveries[1]; var input = inputs[1];
                SetPrivate(rig.Combat, "_inputGeneration", long.MaxValue);
                Assert.Throws<OverflowException>(() => rig.Combat.CommitPreflightedTerminalDeliveryDiscard(candidate, owner));
                Assert.That(deliveries.Count, Is.EqualTo(1)); Assert.That(deliveries[1], Is.EqualTo(delivery));
                Assert.That(inputs.Count, Is.EqualTo(1)); Assert.That(inputs[1], Is.EqualTo(input));
            }
            finally { DestroyAll(objects); }
        }

        // AC-M5B4-001: exact tick, locked interactive injection, initialization
        // and registration are independent rejection boundaries.
        [TestCase(0, 0)] [TestCase(1, 0)] [TestCase(2, 0)]
        [TestCase(0, 1)] [TestCase(1, 1)] [TestCase(2, 1)]
        [TestCase(0, 2)] [TestCase(1, 2)] [TestCase(2, 2)]
        [TestCase(0, 3)] [TestCase(1, 3)] [TestCase(2, 3)]
        public void FramePreconditionsRejectWithoutInitializationOrMutation(int index, int rejection)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects); var owner = new object();
                if (rejection != 3) RegisterFrameOwner(rig, owner);
                var consumer = FrameConsumers(rig)[index];
                var queue = GetPrivate(consumer, index == 0 ? "_commands" : "_inputs");
                var generation = GetPrivate(consumer, "_inputGeneration");
                var field = index == 0 ? "_motor" : index == 1 ? "_initialized" : "_prepared";
                var priorInitialization = GetPrivate(consumer, field);
                if (rejection == 2) SetPrivate(consumer, field, index == 0 ? null : (object)false);
                var tick = new SimulationTick(rejection == 0 ? 1 : 0);
                var locked = rejection == 1;
                Action prepare = index == 0
                    ? (Action)(() => rig.Player.PrepareInputFrame(owner, tick, locked, new MovementCommand(tick, 0, 0, true, false, false)))
                    : index == 1
                        ? () => rig.Transfer.PrepareInputFrame(owner, tick, locked, new TransferSimulationInput(tick, null, null, new TransferPressed(null, tick), Array.Empty<TransferTargetRemoved>(), null))
                        : () => rig.Combat.PrepareInputFrame(owner, tick, locked, Input(tick, true, null));
                Assert.Throws<InvalidOperationException>(() => prepare());
                Assert.That(GetPrivate(consumer, index == 0 ? "_commands" : "_inputs"), Is.SameAs(queue));
                Assert.That(((IDictionary)queue).Count, Is.Zero);
                Assert.That(GetPrivate(consumer, "_inputGeneration"), Is.EqualTo(generation));
                if (rejection == 2)
                {
                    Assert.That(GetPrivate(consumer, field), Is.EqualTo(index == 0 ? null : (object)false));
                    SetPrivate(consumer, field, priorInitialization);
                }
            }
            finally { DestroyAll(objects); }
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void EmptyPhaseAdvanceStalesCandidateWithoutQueueGenerationChange(int index)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects); var owner = new object(); RegisterFrameOwner(rig, owner);
                var consumer = FrameConsumers(rig)[index]; var tick = new SimulationTick(0);
                var generation = GetPrivate(consumer, "_inputGeneration");
                Action validate;
                if (index == 0)
                {
                    var candidate = rig.Player.PrepareInputFrame(owner, tick, false, null);
                    rig.Player.AdvanceOneFixedTickForTests();
                    validate = () => rig.Player.ValidateInputFrameCandidate(owner, candidate);
                }
                else if (index == 1)
                {
                    var candidate = rig.Transfer.PrepareInputFrame(owner, tick, false, null);
                    rig.Transfer.AdvanceTransferPhaseForTests();
                    validate = () => rig.Transfer.ValidateInputFrameCandidate(owner, candidate);
                    Assert.That(rig.Player.NextExpectedTick, Is.EqualTo(tick));
                }
                else
                {
                    var candidate = rig.Combat.PrepareInputFrame(owner, tick, false, null);
                    rig.Transfer.AdvanceTransferPhaseForTests();
                    rig.Combat.AdvanceCombatPhaseForTests();
                    validate = () => rig.Combat.ValidateInputFrameCandidate(owner, candidate);
                    Assert.That(rig.Player.NextExpectedTick, Is.EqualTo(tick));
                }
                Assert.That(GetPrivate(consumer, "_inputGeneration"), Is.EqualTo(generation));
                Assert.Throws<InvalidOperationException>(() => validate());
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void DefaultAndSystemAuthorityInputCannotEnterInteractiveLane()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects); var owner = new object(); RegisterFrameOwner(rig, owner);
                var tick = new SimulationTick(0);
                Assert.Catch<ArgumentException>(() => rig.Transfer.PrepareInputFrame(owner, tick, false, default(TransferSimulationInput)));
                Assert.Catch<ArgumentException>(() => rig.Combat.PrepareInputFrame(owner, tick, false, default(OrdanBossCombatInput)));
                Assert.Throws<InvalidOperationException>(() => rig.Transfer.PrepareInputFrame(owner, tick, false,
                    new TransferSimulationInput(tick, null, null, null, Array.Empty<TransferTargetRemoved>(), TransferClearReason.RoomLeaving)));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.PrepareInputFrame(owner, tick, false, Input(tick, false, Damage(tick, "forbidden.external"))));
                Assert.That(((IDictionary)GetPrivate(rig.Transfer, "_inputs")).Count, Is.Zero);
                Assert.That(((IDictionary)GetPrivate(rig.Combat, "_inputs")).Count, Is.Zero);
            }
            finally { DestroyAll(objects); }
        }

        private static object[] FrameConsumers(BossRig rig) => new object[] { rig.Player, rig.Transfer, rig.Combat };
        private static void RegisterFrameOwner(BossRig rig, object owner)
        {
            rig.Player.RegisterInputGateOwner(owner); rig.Transfer.RegisterInputGateOwner(owner); rig.Combat.RegisterInputGateOwner(owner);
        }

        private List<string> ReplayAtRenderRate(int renderFramesPerSecond)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var owner = new object();
                rig.Combat.RegisterInputGateOwner(owner);
                var tickZero = new SimulationTick(0);
                rig.Combat.Submit(Input(tickZero, true, Damage(tickZero, "gate.cadence")));
                rig.Combat.ApplyInputGate(owner, tickZero, true);
                var trace = new List<string>();
                var accumulator = 0;
                for (var tick = 0; tick < 6;)
                {
                    accumulator += 60;
                    while (accumulator >= renderFramesPerSecond && tick < 6)
                    {
                        if (tick == 1)
                        {
                            rig.Combat.ApplyInputGate(owner, new SimulationTick(1), false);
                        }

                        var outcome = Advance(rig);
                        trace.Add(outcome.Tick.Value + ":" + outcome.CanonicalRequestTrace.Count + ":" +
                            (outcome.Attack.DamageRequest.HasValue ? outcome.Attack.DamageRequest.Value.RequestId : "none") + ":" +
                            Health(outcome, "ordan") + ":" + Health(outcome, "player"));
                        tick++;
                        accumulator -= renderFramesPerSecond;
                    }
                }

                return trace;
            }
            finally
            {
                DestroyAll(objects);
            }
        }

        private static OrdanBossCombatOutcome Advance(BossRig rig)
        {
            rig.Transfer.AdvanceTransferPhaseForTests();
            var outcome = rig.Combat.AdvanceCombatPhaseForTests();
            rig.Bridge.AdvanceBridgePhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();
            return outcome;
        }

        private static OrdanBossCombatInput Input(SimulationTick tick, bool interactive, DamageRequest? request)
        {
            var damage = request.HasValue ? new[] { request.Value } : Array.Empty<DamageRequest>();
            if (!interactive)
            {
                return new OrdanBossCombatInput(tick, null, null, null, damage);
            }

            return new OrdanBossCombatInput(
                tick,
                CameraFor(tick),
                AimFor(tick),
                new BasicAttackPressed(7, 700, tick),
                damage);
        }

        private static DamageRequest Damage(SimulationTick tick, string id) =>
            // Keep Ordan's mandatory full-health bootstrap intact while proving
            // that locking player input is not an invulnerability feature.
            new DamageRequest(id, "gate", "player", 1, DamageKind.BossPattern, tick);

        private static int Health(OrdanBossCombatOutcome outcome, string id)
        {
            foreach (var combatant in outcome.Combatants)
                if (combatant.TargetId == id) return combatant.CurrentHealth;
            throw new InvalidOperationException("Missing expected combatant: " + id);
        }

        private static SimulationCameraPoseSnapshot CameraFor(SimulationTick tick) =>
            new SimulationCameraPoseSnapshot(new SimulationTick(checked(tick.Value - 1)), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);

        private static AimSample AimFor(SimulationTick tick) =>
            new AimSample(700, AimSource.GamepadStick, null, null, null, 4096, 0,
                new SimulationTick(tick.Value - 1), tick);

        private static bool ContainsRequest(IReadOnlyList<DamageRequest> values, string requestId)
        {
            for (var index = 0; index < values.Count; index++)
            {
                if (values[index].RequestId == requestId) return true;
            }

            return false;
        }

        private static OrdanBossCombatInput Queued(OrdanBossCombatSimulationDriver driver, int tick) =>
            (OrdanBossCombatInput)((IDictionary)GetPrivate(driver, "_inputs"))[tick];

        private static object GetPrivate(object value, string fieldName) =>
            value.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value);

        private static void SetPrivate(object value, string fieldName, object fieldValue) =>
            value.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(value, fieldValue);

        private static BossRig CreateRig(ICollection<GameObject> objects)
        {
            var playerObject = Track(objects, "combat-gate-player");
            var playerBody = playerObject.AddComponent<Rigidbody2D>();
            playerBody.bodyType = RigidbodyType2D.Kinematic;
            var playerCollider = playerObject.AddComponent<BoxCollider2D>();
            var player = playerObject.AddComponent<PlayerMovementController>();

            var ordanObject = Track(objects, "combat-gate-ordan");
            ordanObject.transform.position = Vector3.right;
            var ordanCollider = ordanObject.AddComponent<BoxCollider2D>();
            var ordan = ordanObject.AddComponent<CombatTarget>();
            ordan.ConfigureForTests("ordan", 60, 0, true, 0, 0, 0, 0, 100, 100, ordanCollider, ordanObject.transform);
            var playerTarget = playerObject.AddComponent<CombatTarget>();
            playerTarget.ConfigureForTests("player", 5, 45, false, 0, 0, 0, 0, 100, 100, playerCollider, playerObject.transform);

            var transferRegistry = Track(objects, "combat-gate-transfer-registry").AddComponent<TransferTargetRegistry>();
            transferRegistry.ConfigureForTests();
            var transfer = Track(objects, "combat-gate-transfer").AddComponent<TransferSimulationDriver>();
            transfer.ConfigureForTests(player, transferRegistry);
            transfer.InitializeForTests();

            var combatRegistry = Track(objects, "combat-gate-combat-registry").AddComponent<CombatTargetRegistry>();
            combatRegistry.ConfigureForTests(ordan, playerTarget);
            var combatObject = Track(objects, "combat-gate-combat");
            combatObject.SetActive(false);
            var combat = combatObject.AddComponent<OrdanBossCombatSimulationDriver>();
            combat.ConfigureForTests(player, combatRegistry, transfer, combat);
            combatObject.SetActive(true);

            var bridgeObject = Track(objects, "combat-gate-bridge");
            bridgeObject.SetActive(false);
            var bridge = bridgeObject.AddComponent<OrdanBossSimulationDriver>();
            bridge.ConfigureForTests(player, transfer, combat);
            bridge.InitializeForTests();
            bridgeObject.SetActive(true);
            return new BossRig(player, transfer, combat, bridge);
        }

        private static GameObject Track(ICollection<GameObject> objects, string name)
        {
            var value = new GameObject(name);
            objects.Add(value);
            return value;
        }

        private static void DestroyAll(IReadOnlyList<GameObject> objects)
        {
            for (var index = objects.Count - 1; index >= 0; index--)
            {
                UnityEngine.Object.DestroyImmediate(objects[index]);
            }
        }

        private readonly struct BossRig
        {
            internal BossRig(PlayerMovementController player, TransferSimulationDriver transfer,
                OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge)
            { Player = player; Transfer = transfer; Combat = combat; Bridge = bridge; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal OrdanBossCombatSimulationDriver Combat { get; }
            internal OrdanBossSimulationDriver Bridge { get; }
        }
    }
}
