// AC-COM-002, AC-COM-003, AC-COM-004, AC-WT-005.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AcadeGameMaker.Camera.Unity;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class OrdanBossEncounterAuthoredGraphPlayModeTests
    {
        private const string GraphPath = "Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab";

        [UnityTest]
        public IEnumerator FreshGraphPublishesTheExactBossPhaseOrderWithHiddenAudit()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            Assert.That(asset, Is.Not.Null);
            var root = UnityEngine.Object.Instantiate(asset);
            try
            {
                yield return new WaitForFixedUpdate();
                var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                var transfer = root.GetComponentInChildren<TransferSimulationDriver>();
                var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
                var bridge = root.GetComponentInChildren<OrdanBossSimulationDriver>();
                var scheduler = root.GetComponentInChildren<OrdanBossTransferExposureScheduler>();
                var auditPull = root.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>();
                var teardown = root.GetComponentInChildren<OrdanBossTerminalTeardown>();
                Assert.That(asset.name, Is.EqualTo("OrdanBossEncounterGraph"));
                Assert.That(root.name, Is.EqualTo("OrdanBossEncounterGraph(Clone)")); Assert.That(root.transform.childCount, Is.EqualTo(9));
                Assert.That(root.GetComponentsInChildren<OrdanBossCombatSimulationDriver>(true), Has.Length.EqualTo(1));
                Assert.That(root.GetComponentsInChildren<OrdanBossTransferExposureScheduler>(true), Has.Length.EqualTo(1));
                Assert.That(root.GetComponentsInChildren<OrdanBossBalanceAuditPullProducer>(true), Has.Length.EqualTo(1));
                Assert.That(root.GetComponentsInChildren<OrdanBossTerminalTeardown>(true), Has.Length.EqualTo(1));
                Assert.That(root.GetComponentsInChildren<InputRouter>(true), Has.Length.EqualTo(1));
                Assert.That(root.GetComponentsInChildren<OrdanBossTerminalTransitionRequester>(true), Has.Length.EqualTo(1));
                Assert.That(root.GetComponentsInChildren<SimulationCameraDriver>(true), Has.Length.EqualTo(1));
                Assert.That(handoff.LatestPresentationView.HasValue, Is.True);
                var view = handoff.LatestPresentationView.Value;
                Assert.That(view.Tick, Is.EqualTo(transfer.LatestPhasePublication.Value.Tick));
                Assert.That(combat.LatestOutcome.Tick, Is.EqualTo(view.Tick));
                Assert.That(bridge.LatestOutcome.SourceTick, Is.EqualTo(view.Tick));
                Assert.That(scheduler.LatestOutcome.SourceTick, Is.EqualTo(view.Tick));
                Assert.That(ExecutionOrder<TransferSimulationDriver>(), Is.EqualTo(-200));
                Assert.That(ExecutionOrder<OrdanBossTerminalTeardown>(), Is.EqualTo(-195));
                Assert.That(ExecutionOrder<OrdanBossCombatSimulationDriver>(), Is.EqualTo(-190));
                Assert.That(ExecutionOrder<OrdanBossSimulationDriver>(), Is.EqualTo(-185));
                Assert.That(ExecutionOrder<OrdanBossTransferExposureScheduler>(), Is.EqualTo(-180));
                Assert.That(ExecutionOrder<OrdanBossHostileDamageProducer>(), Is.EqualTo(-170));
                Assert.That(ExecutionOrder<OrdanBossBalanceAuditPullProducer>(), Is.EqualTo(-165));
                Assert.That(ExecutionOrder<OrdanBossEncounterHandoffAdapter>(), Is.EqualTo(110));
                Assert.That(auditPull.LatestOutcome.SourceTick, Is.EqualTo(view.Tick));
                Assert.That(teardown.LatestReceipt.HasValue, Is.False);
                Assert.That(view.BalanceAuditPullDigest.Tick, Is.EqualTo(view.Tick));
                Assert.That(view.AuditAvailable, Is.False); Assert.That(view.PayloadAvailable, Is.EqualTo(new[] { false, false, false }));
                Assert.That(view.StableIds, Is.EqualTo(new[] { "ordan", "player", "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
            }
            finally { UnityEngine.Object.Destroy(root); }
        }

        // AC-COM-003, AC-COM-004, AC-WT-005. This follows the authored
        // -200/-190/-185/-180/+110 chain from DebtLine through the first
        // SeizureWeight Telegraph, without deriving a handle from geometry.
        [UnityTest]
        public IEnumerator DebtLineForecastMakesOnlyBossWeightZeroTargetableAtFirstSeizureTelegraph()
        {
            var root = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath));
            try
            {
                Assert.That(root, Is.Not.Null);
                var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                var payloadSinks = root.GetComponentsInChildren<OrdanBossPayloadTransferModifierSink>(true);
                var payloadTargets = root.GetComponentsInChildren<TransferTarget>(true);
                Assert.That(payloadSinks, Has.Length.EqualTo(3));
                foreach (var sink in payloadSinks) Assert.That(sink.IsStillAvailable(), Is.False);

                OrdanBossEncounterPresentationView firstSeizure = default;
                var found = false;
                for (var index = 0; index < 110; index++)
                {
                    yield return new WaitForFixedUpdate();
                    var view = handoff.LatestPresentationView.Value;
                    Assert.That(view.AuditAvailable, Is.False); Assert.That(view.AuditApplied, Is.False);
                    if (view.Snapshot.PatternSlot == OrdanPatternSlot.SeizureWeight && view.Snapshot.Phase == OrdanBossPhase.Telegraph && view.Snapshot.PhaseAge == 0)
                    { firstSeizure = view; found = true; break; }
                }
                Assert.That(found, Is.True, "the first SeizureWeight Telegraph must be published");
                Assert.That(firstSeizure.Snapshot.Tick.Value, Is.EqualTo(108));
                Assert.That(firstSeizure.ForecastPayloadId, Is.EqualTo("BossWeight-0"));
                Assert.That(firstSeizure.ExposedPayloadId, Is.EqualTo("BossWeight-0"));
                Assert.That(firstSeizure.PayloadAvailable, Is.EqualTo(new[] { true, false, false }));
                foreach (var target in payloadTargets)
                    if (target.TargetId.StartsWith("BossWeight-", StringComparison.Ordinal))
                        Assert.That(target.TargetCollider.enabled, Is.EqualTo(target.TargetId == "BossWeight-0"));
            }
            finally { UnityEngine.Object.Destroy(root); }
        }

        // AC-COM-003, AC-COM-004. Each cadence uses an independent authored
        // graph with automatic FixedUpdate disabled and manually runs the
        // production -200/-190/-185/-180/player/+110 phase sequence.
        [UnityTest]
        public IEnumerator FullPresentationTraceIsEquivalentForIndependentThirtySixtyAndOneFortyFourHzCadences()
        {
            // 500 fixed ticks includes the entire BalanceAudit Execute window,
            // so cadence evidence carries audit forecast/exposure fields too.
            var atThirty = RunCadenceTrace(new[] { 2 }, 500);
            var atSixty = RunCadenceTrace(new[] { 1 }, 500);
            // 60/144 = 5/12: five fixed steps across twelve render frames.
            var atOneFortyFour = RunCadenceTrace(new[] { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 }, 500);
            Assert.That(atThirty.Count, Is.EqualTo(250));
            Assert.That(atSixty.Count, Is.EqualTo(500));
            Assert.That(atOneFortyFour.Count, Is.EqualTo(500));
            AssertMatchingObservations(atThirty, atSixty);
            AssertMatchingObservations(atThirty, atOneFortyFour);
            AssertMatchingObservations(atSixty, atOneFortyFour);
            yield break;
        }

        // AC-COM-003, AC-COM-004. A tick-zero manual trace proves the exact
        // authored ordering and -180 fails before it can publish out of order.
        [UnityTest]
        public IEnumerator FirstManualTickHasExactPhaseTraceAndRejectsOutOfOrderScheduler()
        {
            var rig = CreateManualRig();
            try
            {
                Assert.That(rig.Scheduler.HasLatestOutcome, Is.False);
                Assert.Throws<InvalidOperationException>(() => rig.Scheduler.AdvanceSchedulerPhaseForTests());
                Assert.That(rig.Scheduler.HasLatestOutcome, Is.False, "out-of-order -180 must not publish");

                var trace = new List<string>();
                var transfer = rig.Transfer.AdvanceTransferPhaseForTests(); trace.Add("-200");
                Assert.That(transfer.Snapshot.Tick.Value, Is.EqualTo(0));
                var combat = rig.Combat.AdvanceCombatPhaseForTests(); trace.Add("-190");
                Assert.That(combat.Tick.Value, Is.EqualTo(0));
                var bridge = rig.Bridge.AdvanceBridgePhaseForTests(); trace.Add("-185");
                Assert.That(bridge.SourceTick.Value, Is.EqualTo(0));
                var scheduler = rig.Scheduler.AdvanceSchedulerPhaseForTests(); trace.Add("-180");
                Assert.That(scheduler.SourceTick.Value, Is.EqualTo(0));
                Assert.That(rig.Hostile.AdvanceForTests().Tick.Value, Is.EqualTo(0)); trace.Add("-170");
                var pull = rig.AuditPull.AdvanceForTests();
                Assert.That(pull.SourceTick.Value, Is.EqualTo(0)); trace.Add("-165");
                Assert.Throws<InvalidOperationException>(() => rig.AuditPull.AdvanceForTests());
                Assert.That(rig.AuditPull.LatestOutcome.SourceTick.Value, Is.EqualTo(0));
                Assert.That(rig.Player.AdvanceOneFixedTickForTests().Tick.Value, Is.EqualTo(0));
                InvokeHandoff(rig.Handoff); trace.Add("+110");
                Assert.That(rig.Handoff.LatestPresentationView.Value.Tick.Value, Is.EqualTo(0));
                CollectionAssert.AreEqual(new[] { "-200", "-190", "-185", "-180", "-170", "-165", "+110" }, trace);
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
            yield break;
        }

        [UnityTest]
        public IEnumerator BalanceAuditPullRejectsSkippedBootstrapWithoutPublishing()
        {
            var rig = CreateManualRig();
            try
            {
                rig.Transfer.AdvanceTransferPhaseForTests();
                rig.Combat.AdvanceCombatPhaseForTests();
                rig.Bridge.AdvanceBridgePhaseForTests();
                rig.Scheduler.AdvanceSchedulerPhaseForTests();
                rig.Hostile.AdvanceForTests();
                rig.Player.AdvanceOneFixedTickForTests();

                rig.Transfer.AdvanceTransferPhaseForTests();
                rig.Combat.AdvanceCombatPhaseForTests();
                rig.Bridge.AdvanceBridgePhaseForTests();
                rig.Scheduler.AdvanceSchedulerPhaseForTests();
                rig.Hostile.AdvanceForTests();
                Assert.Throws<InvalidOperationException>(() => rig.AuditPull.AdvanceForTests());
                Assert.That(rig.AuditPull.HasLatestOutcome, Is.False);
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
            yield break;
        }

        // AC-M4B3B2-004, AC-M4B3B2-005, AC-M4B3B2-007. DebtLine A contact
        // publishes only a t+1 request/digest; the following Combat tick alone
        // applies the damage and removes the pending delivery.
        [UnityTest]
        public IEnumerator DebtContactQueuesThenNextCombatTickAppliesExactlyOnce()
        {
            var rig = CreateManualRig();
            try
            {
                SeedPlayerPosition(rig.Player, 0, 4915);
                OrdanBossHostileDamageDigest queued = default;
                var found = false;
                for (var index = 0; index < 60; index++)
                {
                    ManualFixedStep(rig);
                    var digest = rig.Handoff.LatestPresentationView.Value.HostileDamageDigest;
                    if (digest.Disposition != OrdanBossHostileDisposition.Queued) continue;
                    queued = digest; found = true; break;
                }
                Assert.That(found, Is.True, "DebtLine A must contact the seeded in-strip player");
                Assert.That(queued.GeometryKey, Is.EqualTo("DebtLineA"));
                Assert.That(queued.DeliveryTick.Value.Value, Is.EqualTo(queued.Tick.Value + 1));
                var pending = rig.Combat.ReadPendingDeliveryForTests(queued.DeliveryTick.Value);
                Assert.That(pending.HostileAppendCommitted, Is.True);
                Assert.That(pending.HostileRequests, Has.Count.EqualTo(1));
                Assert.That(pending.HostileRequests[0].RequestId, Is.EqualTo(queued.RequestId));
                var healthBefore = PlayerHealth(rig.Handoff.LatestPresentationView.Value);

                ManualFixedStep(rig);

                Assert.That(PlayerHealth(rig.Handoff.LatestPresentationView.Value), Is.EqualTo(healthBefore - 1));
                Assert.That(rig.Combat.LatestOutcome.DamageResults, Has.Count.EqualTo(1));
                Assert.That(rig.Combat.LatestOutcome.DamageResults[0].RequestId, Is.EqualTo(queued.RequestId));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.ReadPendingDeliveryForTests(queued.DeliveryTick.Value));
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
            yield break;
        }

        // AC-M4B3B2-004. A seeded player beyond the DebtLine emission edge
        // remains outside the segment and therefore produces no hostile request.
        [UnityTest]
        public IEnumerator AuthoredNoContactPathQueuesNoHostileRequest()
        {
            var rig = CreateManualRig();
            try
            {
                SeedPlayerPosition(rig.Player, 28672, 4915);
                for (var index = 0; index < 60; index++)
                {
                    ManualFixedStep(rig);
                    Assert.That(rig.Handoff.LatestPresentationView.Value.HostileDamageDigest.Disposition,
                        Is.Not.EqualTo(OrdanBossHostileDisposition.Queued));
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
            yield break;
        }

        // AC-M4B3B2-004. Both normal and Transfer-accelerated authored
        // Seizure paths sweep the seeded ground position and queue once.
        [UnityTest]
        public IEnumerator NormalAndAcceleratedSeizurePathsContactInAuthoredGraph()
        {
            foreach (var accelerated in new[] { false, true })
            {
                var rig = CreateManualRig();
                try
                {
                    SeedPlayerPosition(rig.Player, accelerated ? 0 : -20480, 4915);
                    var found = false;
                    while (rig.Player.NextExpectedTick.Value < 250)
                    {
                        var steppingTransfer = accelerated && rig.Player.NextExpectedTick.Value == 108;
                        if (steppingTransfer)
                            rig.Transfer.SubmitForTests(PayloadTransferPress(rig.Player.NextExpectedTick));
                        if (accelerated && rig.Player.NextExpectedTick.Value > 108 &&
                            rig.Player.Snapshot.PositionXQ4096 > -5 * 4096)
                            rig.Player.SubmitCommand(new MovementCommand(rig.Player.NextExpectedTick,
                                -4096, 0, false, false, false));
                        ManualFixedStep(rig);
                        if (steppingTransfer)
                        {
                            var press = rig.Transfer.LatestPhasePublication.Value.Outcome.PressResult.Value;
                            Assert.That(press.Success, Is.True, "payload press failed: " + press.FailureReason + "/" + press.SelectedTargetId);
                        }
                        var digest = rig.Handoff.LatestPresentationView.Value.HostileDamageDigest;
                        if (digest.Disposition == OrdanBossHostileDisposition.Queued && digest.GeometryKey == "Seizure")
                        {
                            Assert.That(digest.PayloadId, Is.EqualTo("BossWeight-0"));
                            Assert.That(rig.Bridge.LatestOutcome.SeizureTrajectoryProjection.Value.Accelerated, Is.EqualTo(accelerated));
                            found = true; break;
                        }
                    }
                    Assert.That(found, Is.True, accelerated ? "accelerated Seizure contact missing" : "normal Seizure contact missing");
                }
                finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
            }
            yield break;
        }

        // AC-M4B3B2-004. The uninterrupted final Audit Execute tick emits and
        // contacts its closed annulus; a same-tick Transfer interrupt wins.
        [UnityTest]
        public IEnumerator AuditShockwaveContactsUnlessFinalTickIsInterrupted()
        {
            var uninterrupted = CreateManualRig();
            try
            {
                SeedPlayerPosition(uninterrupted.Player, -20480, 4915);
                while (uninterrupted.Player.NextExpectedTick.Value <= 545)
                {
                    // Countersteer the new BalanceAudit pull so this older
                    // shockwave fixture remains in the closed annulus.
                    if (uninterrupted.Player.Snapshot.PositionXQ4096 > -8 * 4096)
                        uninterrupted.Player.SubmitCommand(new MovementCommand(
                            uninterrupted.Player.NextExpectedTick, -4096, 0, false, false, false));
                    ManualFixedStep(uninterrupted);
                }
                var digest = uninterrupted.Handoff.LatestPresentationView.Value.HostileDamageDigest;
                Assert.That(digest.GeometryKey, Is.EqualTo("Audit"));
                Assert.That(digest.Disposition, Is.EqualTo(OrdanBossHostileDisposition.Queued));
            }
            finally { UnityEngine.Object.DestroyImmediate(uninterrupted.Root); }

            var interrupted = CreateManualRig();
            try
            {
                while (interrupted.Player.NextExpectedTick.Value < 545)
                {
                    if (interrupted.Player.Snapshot.PositionXQ4096 < -5 * 4096)
                        interrupted.Player.SubmitCommand(new MovementCommand(interrupted.Player.NextExpectedTick,
                            4096, 0, false, false, false));
                    ManualFixedStep(interrupted);
                }
                interrupted.Transfer.SubmitForTests(TransferPress(interrupted.Player.NextExpectedTick));
                ManualFixedStep(interrupted);
                Assert.That(interrupted.Transfer.LatestPhasePublication.Value.Outcome.PressResult.Value.Success, Is.True);
                Assert.That(interrupted.Handoff.LatestPresentationView.Value.HostileDamageDigest.GeometryKey, Is.Empty);
                foreach (var intent in interrupted.Bridge.LatestOutcome.Output.Intents)
                    Assert.That(intent.Kind, Is.Not.EqualTo(OrdanBossIntentKind.BalanceAuditShockwave));
            }
            finally { UnityEngine.Object.DestroyImmediate(interrupted.Root); }
            yield break;
        }

        // AC-M4B3B3-007, AC-M4B3B3-008. Age zero queues only t+1; the next
        // tick consumes that exact identity, while a same-tick Transfer edge
        // cancels the pending current directive before Movement can consume it.
        [UnityTest]
        public IEnumerator BalanceAuditPullSeparatesQueueConsumeAndInterruptCancellation()
        {
            var rig = CreateManualRig();
            try
            {
                OrdanBossBalanceAuditPullDigest ageZero = default;
                var found = false;
                while (rig.Player.NextExpectedTick.Value < 600)
                {
                    if (rig.Player.Snapshot.PositionXQ4096 < -5 * 4096)
                        rig.Player.SubmitCommand(new MovementCommand(
                            rig.Player.NextExpectedTick, 4096, 0, false, false, false));
                    ManualFixedStep(rig);
                    var view = rig.Handoff.LatestPresentationView.Value;
                    if (view.Snapshot.PatternSlot != OrdanPatternSlot.BalanceAudit ||
                        view.Snapshot.Phase != OrdanBossPhase.Execute || view.Snapshot.PhaseAge != 0)
                        continue;
                    ageZero = view.BalanceAuditPullDigest;
                    found = true;
                    break;
                }

                Assert.That(found, Is.True);
                Assert.That(ageZero.Disposition, Is.EqualTo(OrdanBossBalanceAuditPullDisposition.Queued));
                Assert.That(ageZero.ConsumedCurrentDirectiveId, Is.Empty);
                Assert.That(ageZero.QueuedNextMovementTick.Value,
                    Is.EqualTo(new SimulationTick(ageZero.Tick.Value + 1)));

                var firstQueuedId = ageZero.QueuedNextDirectiveId;
                ManualFixedStep(rig);
                var consumed = rig.Handoff.LatestPresentationView.Value.BalanceAuditPullDigest;
                Assert.That(consumed.ConsumedCurrentDirectiveId, Is.EqualTo(firstQueuedId));
                Assert.That((long)consumed.AppliedDeltaXQ4096 * consumed.AppliedDeltaXQ4096 +
                    (long)consumed.AppliedDeltaYQ4096 * consumed.AppliedDeltaYQ4096,
                    Is.LessThanOrEqualTo(410L * 410L));

                var pendingCurrentId = consumed.QueuedNextDirectiveId;
                var committedPositionX = rig.Player.Snapshot.PositionXQ4096;
                rig.Transfer.SubmitForTests(TransferPress(rig.Player.NextExpectedTick));
                ManualFixedStep(rig);
                Assert.That(rig.Transfer.LatestPhasePublication.Value.Outcome.PressResult.Value.Success, Is.True);
                var interrupted = rig.Handoff.LatestPresentationView.Value.BalanceAuditPullDigest;
                Assert.That(interrupted.Disposition, Is.EqualTo(OrdanBossBalanceAuditPullDisposition.Interrupted));
                Assert.That(interrupted.CancelledCurrentDirectiveId, Is.EqualTo(pendingCurrentId));
                Assert.That(interrupted.ConsumedCurrentDirectiveId, Is.Empty);
                Assert.That(interrupted.QueuedNextDirectiveId, Is.Empty);
                Assert.That(interrupted.AppliedDeltaXQ4096, Is.Zero);
                Assert.That(interrupted.AppliedDeltaYQ4096, Is.Zero);
                Assert.That(rig.Player.Snapshot.PositionXQ4096, Is.EqualTo(committedPositionX),
                    "same-tick cancellation must not roll back the prior committed pull");
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
            yield break;
        }

        [UnityTest]
        public IEnumerator UninterruptedBalanceAuditPullConsumesExactlyAgesOneThroughFinal()
        {
            var rig = CreateManualRig();
            try
            {
                var executeTicks = 0;
                var consumedTicks = 0;
                var reachedFinal = false;
                while (rig.Player.NextExpectedTick.Value < 650)
                {
                    ManualFixedStep(rig);
                    var view = rig.Handoff.LatestPresentationView.Value;
                    if (view.Snapshot.PatternSlot != OrdanPatternSlot.BalanceAudit ||
                        view.Snapshot.Phase != OrdanBossPhase.Execute)
                    {
                        if (reachedFinal) break;
                        continue;
                    }

                    executeTicks++;
                    var digest = view.BalanceAuditPullDigest;
                    if (view.Snapshot.PhaseAge == 0)
                    {
                        Assert.That(digest.ConsumedCurrentDirectiveId, Is.Empty);
                        Assert.That(digest.QueuedNextDirectiveId, Is.Not.Empty);
                        continue;
                    }

                    consumedTicks++;
                    Assert.That(digest.ConsumedCurrentDirectiveId, Is.Not.Empty);
                    if (view.Snapshot.PhaseAge == 89)
                    {
                        reachedFinal = true;
                        Assert.That(digest.Disposition, Is.EqualTo(OrdanBossBalanceAuditPullDisposition.FinalExecuteNoQueue));
                        Assert.That(digest.QueuedNextDirectiveId, Is.Empty);
                    }
                    else
                    {
                        Assert.That(digest.Disposition, Is.EqualTo(OrdanBossBalanceAuditPullDisposition.Queued));
                        Assert.That(digest.QueuedNextDirectiveId, Is.Not.Empty);
                    }
                }

                Assert.That(reachedFinal, Is.True);
                Assert.That(executeTicks, Is.EqualTo(90));
                Assert.That(consumedTicks, Is.EqualTo(89));
                var after = rig.Handoff.LatestPresentationView.Value.BalanceAuditPullDigest;
                Assert.That(after.Disposition, Is.EqualTo(OrdanBossBalanceAuditPullDisposition.NoProjection));
                Assert.That(after.ConsumedCurrentDirectiveId, Is.Empty);
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
            yield break;
        }

        // AC-M4B3B2-006. Current Combat death and Transfer lifecycle suppress
        // new requests, boss death terminal-latches, and four-ID removal stays scheduler-owned.
        [UnityTest]
        public IEnumerator DeathLifecycleAndTerminalSuppressionRemainOwnerExact()
        {
            var boss = CreateManualRig();
            try
            {
                ManualFixedStep(boss);
                var tick = boss.Player.NextExpectedTick;
                boss.Transfer.AdvanceTransferPhaseForTests();
                boss.Combat.Submit(new OrdanBossCombatInput(tick, null, null, null,
                    new[] { new DamageRequest("test/hostile/boss-death", "test", "ordan", 60, DamageKind.BossPattern, tick) }));
                boss.Combat.AdvanceCombatPhaseForTests(); boss.Bridge.AdvanceBridgePhaseForTests(); boss.Scheduler.AdvanceSchedulerPhaseForTests();
                var outcome = boss.Hostile.AdvanceForTests();
                Assert.That(outcome.Digest.Disposition, Is.EqualTo(OrdanBossHostileDisposition.SuppressedBossDead));
                Assert.That(outcome.Digest.Terminal, Is.True);
                var pull = boss.AuditPull.AdvanceForTests();
                Assert.That(pull.Disposition, Is.EqualTo(OrdanBossBalanceAuditPullDisposition.SuppressedBossDead));
                Assert.That(pull.Terminal, Is.True);
                Assert.Throws<InvalidOperationException>(() => boss.Hostile.AdvanceForTests());
                Assert.Throws<InvalidOperationException>(() => boss.AuditPull.AdvanceForTests());
                Assert.That(boss.Scheduler.LatestOutcome.PermanentlyRemovedTargetIds,
                    Is.EqualTo(new[] { "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
                Assert.That(boss.Scheduler.LatestAuditOutcome.PermanentlyRemovedTargetIds, Is.EqualTo(new[] { "BossAuditBox-0" }));
            }
            finally { UnityEngine.Object.DestroyImmediate(boss.Root); }

            var player = CreateManualRig();
            try
            {
                var tick = player.Player.NextExpectedTick;
                player.Transfer.AdvanceTransferPhaseForTests();
                player.Combat.Submit(new OrdanBossCombatInput(tick, null, null, null,
                    new[] { new DamageRequest("test/hostile/player-death", "test", "player", 5, DamageKind.BossPattern, tick) }));
                player.Combat.AdvanceCombatPhaseForTests(); player.Bridge.AdvanceBridgePhaseForTests(); player.Scheduler.AdvanceSchedulerPhaseForTests();
                Assert.That(player.Hostile.AdvanceForTests().Digest.Disposition, Is.EqualTo(OrdanBossHostileDisposition.SuppressedPlayerDead));
                Assert.That(player.AuditPull.AdvanceForTests().Disposition,
                    Is.EqualTo(OrdanBossBalanceAuditPullDisposition.SuppressedPlayerDead));
            }
            finally { UnityEngine.Object.DestroyImmediate(player.Root); }

            var lifecycle = CreateManualRig();
            try
            {
                var tick = lifecycle.Player.NextExpectedTick;
                lifecycle.Transfer.SubmitForTests(new TransferSimulationInput(tick, null, null, null,
                    Array.Empty<TransferTargetRemoved>(), TransferClearReason.RoomLeaving));
                lifecycle.Transfer.AdvanceTransferPhaseForTests(); lifecycle.Combat.AdvanceCombatPhaseForTests();
                lifecycle.Bridge.AdvanceBridgePhaseForTests(); lifecycle.Scheduler.AdvanceSchedulerPhaseForTests();
                Assert.That(lifecycle.Hostile.AdvanceForTests().Digest.Disposition, Is.EqualTo(OrdanBossHostileDisposition.SuppressedLifecycle));
                Assert.That(lifecycle.AuditPull.AdvanceForTests().Disposition,
                    Is.EqualTo(OrdanBossBalanceAuditPullDisposition.SuppressedLifecycle));
            }
            finally { UnityEngine.Object.DestroyImmediate(lifecycle.Root); }
            yield break;
        }

        // AC-M4B3C-001, AC-M4B3C-003, AC-M4B3C-005, AC-M4B3C-008. The
        // authored death tick queues its t+1 cleanup; -195 then consumes only
        // the exact empty Combat delivery and leaves Movement/Transfer able to
        // advance on the following two ticks.
        [Test]
        public void TerminalDeathCleanupDisablesOnlyBossGraphAndPreservesNextTicks()
        {
            var rig = CreateManualRig();
            try
            {
                var teardown = rig.Root.GetComponentInChildren<OrdanBossTerminalTeardown>();
                Assert.That(teardown, Is.Not.Null);
                ManualFixedStep(rig);

                var deathTick = rig.Player.NextExpectedTick;
                rig.Transfer.AdvanceTransferPhaseForTests();
                rig.Combat.Submit(new OrdanBossCombatInput(deathTick, null, null, null,
                    new[] { new DamageRequest("test/hostile/terminal-teardown", "test", "ordan", 60, DamageKind.BossPattern, deathTick) }));
                rig.Combat.AdvanceCombatPhaseForTests(); rig.Bridge.AdvanceBridgePhaseForTests(); rig.Scheduler.AdvanceSchedulerPhaseForTests();
                rig.Hostile.AdvanceForTests(); rig.AuditPull.AdvanceForTests();
                rig.Player.AdvanceOneFixedTickForTests();
                InvokeHandoff(rig.Handoff);
                Assert.That(rig.Handoff.LatestPresentationView.Value.IsTerminal, Is.True);
                Assert.That(rig.Handoff.LatestPresentationView.Value.Tick, Is.EqualTo(deathTick));

                var cleanupTick = rig.Player.NextExpectedTick;
                rig.Transfer.AdvanceTransferPhaseForTests();
                Assert.That(rig.Transfer.LatestBossTerminalCleanupReceipt.Value.CleanupTick, Is.EqualTo(cleanupTick));

                rig.Player.enabled = true; rig.Transfer.enabled = true; rig.Combat.enabled = true; rig.Bridge.enabled = true;
                rig.Scheduler.enabled = true; rig.Hostile.enabled = true; rig.AuditPull.enabled = true; rig.Handoff.enabled = true; teardown.enabled = true;
                var ordan = Array.Find(rig.Root.GetComponentsInChildren<CombatTarget>(true), target => target.Freeze().TargetId == "ordan");
                Assert.That(ordan, Is.Not.Null);
                ordan.enabled = true; ordan.Freeze().TargetCollider.enabled = true;
                InvokeTeardown(teardown);

                Assert.That(teardown.LatestReceipt.HasValue, Is.True);
                Assert.That(teardown.LatestReceipt.Value.DeathSourceTick, Is.EqualTo(deathTick));
                Assert.That(teardown.LatestReceipt.Value.CleanupTick, Is.EqualTo(cleanupTick));
                Assert.That(teardown.LatestReceipt.Value.DiscardedDelivery.HostileAppendCommitted, Is.False);
                Assert.That(rig.Combat.enabled || rig.Bridge.enabled || rig.Scheduler.enabled || rig.Hostile.enabled || rig.AuditPull.enabled || rig.Handoff.enabled || ordan.enabled, Is.False);
                Assert.That(rig.Player.enabled && rig.Transfer.enabled && rig.Root.activeInHierarchy, Is.True);
                Assert.That(ordan.Freeze().TargetCollider.enabled, Is.False);
                foreach (var target in rig.Transfer.BoundRegistry.Targets) Assert.That(target.TargetCollider.enabled, Is.False);

                Assert.That(rig.Player.AdvanceOneFixedTickForTests().Tick, Is.EqualTo(cleanupTick));
                var followingTick = rig.Player.NextExpectedTick;
                Assert.That(rig.Transfer.AdvanceTransferPhaseForTests().Snapshot.Tick, Is.EqualTo(followingTick));
                Assert.That(rig.Player.AdvanceOneFixedTickForTests().Tick, Is.EqualTo(followingTick));
                var secondFollowingTick = rig.Player.NextExpectedTick;
                Assert.That(rig.Transfer.AdvanceTransferPhaseForTests().Snapshot.Tick, Is.EqualTo(secondFollowingTick));
                Assert.That(rig.Player.AdvanceOneFixedTickForTests().Tick, Is.EqualTo(secondFollowingTick));
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
        }

        // AC-M4B3C-009, REQ-COM-004, REQ-WT-003, REQ-WT-005. Independent
        // authored graphs receive the lethal request at the same source tick;
        // synthetic render cadence may change observation grouping, never the
        // terminal handoff, t+1 cleanup receipt, teardown set, or surviving
        // player/Transfer ticks.
        [Test]
        public void TerminalTraceIsEquivalentForIndependentThirtySixtyAndOneFortyFourHzCadences()
        {
            var atThirty = RunTerminalCadenceTrace(new[] { 2 });
            var atSixty = RunTerminalCadenceTrace(new[] { 1 });
            // 60/144 = 5/12: five fixed steps across twelve render frames.
            var atOneFortyFour = RunTerminalCadenceTrace(new[] { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0 });

            AssertEquivalentTerminalTrace("30/60", atThirty, atSixty);
            AssertEquivalentTerminalTrace("30/144", atThirty, atOneFortyFour);
            TestContext.WriteLine("AC-M4B3C-009 terminal cadence: handoff=" + atThirty.FinalHandoffHash +
                " cleanup/disabled/post=" + string.Join(" | ", atThirty.Entries));
        }

        // AC-M4B3B2-003. The bridge-owned pending batch accepts one exact
        // producer-private replacement and rejects malformed, foreign, forged,
        // stale second-owner work without changing the pristine batch.
        [UnityTest]
        public IEnumerator HostileAppendOwnerAndRequestValidationAreAtomic()
        {
            var rig = CreateManualRig();
            var foreignObject = new GameObject("foreign-hostile-producer");
            try
            {
                var tick = rig.Player.NextExpectedTick;
                rig.Transfer.AdvanceTransferPhaseForTests(); rig.Combat.AdvanceCombatPhaseForTests();
                rig.Bridge.AdvanceBridgePhaseForTests(); rig.Scheduler.AdvanceSchedulerPhaseForTests();
                var delivery = new SimulationTick(tick.Value + 1);
                var pristine = rig.Combat.ReadPendingDeliveryForTests(delivery);
                Assert.That(pristine.HostileAppendCommitted, Is.False);

                var malformed = new DamageRequest("ordan.hostile/debt-a/9/0000000001", "ordan", "player", 1,
                    DamageKind.BossPattern, delivery);
                Assert.Throws<ArgumentException>(() => rig.Combat.PreflightHostileDeliveryAppend(rig.Hostile, tick,
                    new[] { malformed }));
                var wrongHorizon = new DamageRequest("ordan.hostile/debt-a/0000000000000000000/0000000002", "ordan",
                    "player", 1, DamageKind.BossPattern, new SimulationTick(2));
                Assert.Throws<ArgumentException>(() => rig.Combat.PreflightHostileDeliveryAppend(rig.Hostile, tick,
                    new[] { wrongHorizon }));
                var foreign = foreignObject.AddComponent<OrdanBossHostileDamageProducer>();
                Assert.Throws<InvalidOperationException>(() => rig.Combat.PreflightHostileDeliveryAppend(foreign, tick,
                    Array.Empty<DamageRequest>()));
                Assert.That(rig.Combat.ReadPendingDeliveryForTests(delivery), Is.EqualTo(pristine));

                var exact = new DamageRequest("ordan.hostile/debt-a/0000000000000000000/0000000001", "ordan", "player",
                    1, DamageKind.BossPattern, delivery);
                var caller = new[] { exact };
                var candidate = rig.Combat.PreflightHostileDeliveryAppend(rig.Hostile, tick, caller);
                caller[0] = malformed;
                rig.Combat.CommitPreflightedHostileDeliveryAppend(candidate);
                var committed = rig.Combat.ReadPendingDeliveryForTests(delivery);
                Assert.That(committed.HostileAppendCommitted, Is.True);
                Assert.That(committed.HostileRequests[0].RequestId, Is.EqualTo(exact.RequestId));
                Assert.Throws<InvalidOperationException>(() => rig.Combat.PreflightHostileDeliveryAppend(rig.Hostile,
                    tick, Array.Empty<DamageRequest>()));
                var forged = new OrdanBossHostileDeliveryCandidate(new object(), committed, Array.Empty<DamageRequest>());
                Assert.Throws<InvalidOperationException>(() => rig.Combat.CommitPreflightedHostileDeliveryAppend(forged));
                Assert.That(rig.Combat.ReadPendingDeliveryForTests(delivery), Is.EqualTo(committed));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(foreignObject);
                UnityEngine.Object.DestroyImmediate(rig.Root);
            }
            yield break;
        }

        private static IReadOnlyDictionary<int, string> RunCadenceTrace(IReadOnlyList<int> renderFrameFixedStepCounts, int targetTicks)
        {
            var rig = CreateManualRig();
            try
            {
                var observations = new Dictionary<int, string>(); var frame = 0; var completedTicks = 0;
                while (completedTicks < targetTicks)
                {
                    var stepCount = renderFrameFixedStepCounts[frame++ % renderFrameFixedStepCounts.Count];
                    for (var step = 0; step < stepCount && completedTicks < targetTicks; step++)
                    {
                        ManualFixedStep(rig);
                        completedTicks++;
                    }
                    // One presentation observation per synthetic render frame.
                    // A 144 Hz zero-step frame therefore observes the unchanged
                    // last view, while a 30 Hz frame observes after two ticks.
                    if (!rig.Handoff.LatestPresentationView.HasValue) continue;
                    var view = rig.Handoff.LatestPresentationView.Value;
                    var signature = FullViewSignature(view, rig.Player.Snapshot, rig.Hostile, rig.Combat);
                    if (observations.TryGetValue(view.Tick.Value, out var prior)) Assert.That(signature, Is.EqualTo(prior));
                    else observations.Add(view.Tick.Value, signature);
                }
                return observations;
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
        }

        private static TerminalCadenceTrace RunTerminalCadenceTrace(IReadOnlyList<int> renderFrameFixedStepCounts)
        {
            const int deathSourceTick = 4;
            const int finalPostTick = 7;
            var rig = CreateManualRig();
            try
            {
                var entries = new List<string>(); var frame = 0; string finalHandoffSignature = null;
                while (rig.Player.NextExpectedTick.Value <= finalPostTick)
                {
                    var steps = renderFrameFixedStepCounts[frame++ % renderFrameFixedStepCounts.Count];
                    for (var step = 0; step < steps && rig.Player.NextExpectedTick.Value <= finalPostTick; step++)
                    {
                        var tick = rig.Player.NextExpectedTick;
                        if (tick.Value < deathSourceTick) ManualFixedStep(rig);
                        else if (tick.Value == deathSourceTick)
                            finalHandoffSignature = AdvanceTerminalDeathTick(rig, tick, entries);
                        else if (tick.Value == deathSourceTick + 1)
                            AdvanceTerminalCleanupTick(rig, tick, entries);
                        else
                            AdvanceTerminalPostTick(rig, tick, entries);
                    }
                }
                Assert.That(finalHandoffSignature, Is.Not.Null);
                return new TerminalCadenceTrace(entries, finalHandoffSignature, StableTerminalHash(finalHandoffSignature));
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
        }

        private static string AdvanceTerminalDeathTick(ManualRig rig, SimulationTick deathTick, List<string> entries)
        {
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.Submit(new OrdanBossCombatInput(deathTick, null, null, null,
                new[] { new DamageRequest("test/cadence/terminal", "test", "ordan", 60, DamageKind.BossPattern, deathTick) }));
            rig.Combat.AdvanceCombatPhaseForTests(); rig.Bridge.AdvanceBridgePhaseForTests(); rig.Scheduler.AdvanceSchedulerPhaseForTests();
            rig.Hostile.AdvanceForTests(); rig.AuditPull.AdvanceForTests(); rig.Player.AdvanceOneFixedTickForTests(); InvokeHandoff(rig.Handoff);
            var finalView = rig.Handoff.LatestPresentationView.Value;
            Assert.That(finalView.IsTerminal && finalView.Tick == deathTick, Is.True);
            var signature = FullViewSignature(finalView, rig.Player.Snapshot, rig.Hostile, rig.Combat);
            entries.Add("tick=" + deathTick.Value + ":death:handoff=" + StableTerminalHash(signature));
            return signature;
        }

        private static void AdvanceTerminalCleanupTick(ManualRig rig, SimulationTick cleanupTick, List<string> entries)
        {
            rig.Transfer.AdvanceTransferPhaseForTests(); var cleanup = rig.Transfer.LatestBossTerminalCleanupReceipt.Value;
            Assert.That(cleanup.DeathSourceTick, Is.EqualTo(new SimulationTick(cleanupTick.Value - 1)));
            Assert.That(cleanup.CleanupTick, Is.EqualTo(cleanupTick)); Assert.That(cleanup.IsLifecycleSuperseded, Is.False);
            Assert.That(cleanup.DurableRemovalCommitted, Is.True); Assert.That(cleanup.Clear.HasValue, Is.False);
            Assert.That(cleanup.RequestedIds, Is.EqualTo(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
            Assert.That(cleanup.PlayerState, Is.EqualTo(TransferPlayerState.Baseline)); Assert.That(cleanup.ActiveTargetAfter, Is.Empty);
            var teardown = rig.Root.GetComponentInChildren<OrdanBossTerminalTeardown>();
            var ordan = Array.Find(rig.Root.GetComponentsInChildren<CombatTarget>(true), target => target.Freeze().TargetId == "ordan");
            Assert.That(teardown, Is.Not.Null); Assert.That(ordan, Is.Not.Null);
            rig.Player.enabled = true; rig.Transfer.enabled = true; rig.Combat.enabled = true; rig.Bridge.enabled = true;
            rig.Scheduler.enabled = true; rig.Hostile.enabled = true; rig.AuditPull.enabled = true; rig.Handoff.enabled = true; teardown.enabled = true;
            ordan.enabled = true; ordan.Freeze().TargetCollider.enabled = true;
            InvokeTeardown(teardown);
            var receipt = teardown.LatestReceipt.Value;
            Assert.That(receipt.DeathSourceTick, Is.EqualTo(cleanup.DeathSourceTick)); Assert.That(receipt.CleanupTick, Is.EqualTo(cleanupTick));
            Assert.That(receipt.DiscardedDelivery.SourceTick, Is.EqualTo(cleanup.DeathSourceTick)); Assert.That(receipt.DiscardedDelivery.DeliveryTick, Is.EqualTo(cleanupTick));
            Assert.That(receipt.DiscardedDelivery.Request.HasValue || receipt.DiscardedDelivery.HostileRequests.Count != 0 || receipt.DiscardedDelivery.HostileAppendCommitted, Is.False);
            Assert.That(receipt.DisabledComponents, Is.EqualTo(new[] { "combat", "bridge", "scheduler", "hostile", "pull", "handoff", "ordan" }));
            Assert.That(receipt.DisabledColliders, Is.EqualTo(new[] { "ordan", "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
            Assert.That(rig.Combat.enabled || rig.Bridge.enabled || rig.Scheduler.enabled || rig.Hostile.enabled || rig.AuditPull.enabled || rig.Handoff.enabled || ordan.enabled || teardown.enabled, Is.False);
            Assert.That(rig.Player.enabled && rig.Transfer.enabled && rig.Root.activeInHierarchy, Is.True);
            Assert.That(rig.Player.AdvanceOneFixedTickForTests().Tick, Is.EqualTo(cleanupTick));
            entries.Add("tick=" + cleanupTick.Value + ":cleanup:" + CleanupReceiptSignature(cleanup, receipt) + ":player=" + rig.Player.Snapshot.Tick.Value);
        }

        private static void AdvanceTerminalPostTick(ManualRig rig, SimulationTick tick, List<string> entries)
        {
            var transferTick = rig.Transfer.AdvanceTransferPhaseForTests().Snapshot.Tick;
            Assert.That(transferTick, Is.EqualTo(tick)); Assert.That(rig.Player.AdvanceOneFixedTickForTests().Tick, Is.EqualTo(tick));
            entries.Add("tick=" + tick.Value + ":post:player=" + rig.Player.Snapshot.Tick.Value + ":transfer=" + transferTick.Value);
        }

        private static void AssertEquivalentTerminalTrace(string label, TerminalCadenceTrace expected, TerminalCadenceTrace actual)
        {
            Assert.That(actual.FinalHandoffSignature, Is.EqualTo(expected.FinalHandoffSignature), label + " final handoff raw signature diverged.");
            Assert.That(actual.FinalHandoffHash, Is.EqualTo(expected.FinalHandoffHash), label + " final handoff hash diverged.");
            Assert.That(actual.Entries.Count, Is.EqualTo(expected.Entries.Count), label + " terminal trace entry count diverged.");
            for (var index = 0; index < expected.Entries.Count; index++)
                Assert.That(actual.Entries[index], Is.EqualTo(expected.Entries[index]), label + " first divergent terminal tick: " + expected.Entries[index]);
        }

        private static string CleanupReceiptSignature(BossTerminalTransferCleanupReceipt cleanup, OrdanBossTerminalTeardownReceipt receipt)
        {
            var builder = new StringBuilder();
            builder.Append(cleanup.DeathSourceTick.Value).Append(':').Append(cleanup.CleanupTick.Value).Append(':')
                .Append(cleanup.IsLifecycleSuperseded).Append(':').Append(cleanup.DurableRemovalCommitted).Append(':')
                .Append(cleanup.ActiveTargetBefore).Append(':').Append(cleanup.ActiveTargetAfter).Append(':')
                .Append((int)cleanup.PlayerState).Append(':').Append(cleanup.Revision).Append(':')
                .Append(receipt.EncounterKey).Append(':').Append(receipt.PlayerPreserved).Append(':').Append(receipt.Terminal);
            foreach (var id in cleanup.RequestedIds) builder.Append(":request=").Append(id);
            foreach (var id in receipt.DisabledComponents) builder.Append(":component=").Append(id);
            foreach (var id in receipt.DisabledColliders) builder.Append(":collider=").Append(id);
            return builder.ToString();
        }

        private static string StableTerminalHash(string value)
        {
            unchecked
            {
                uint hash = 2166136261;
                for (var index = 0; index < value.Length; index++) { hash ^= value[index]; hash *= 16777619; }
                return hash.ToString("X8");
            }
        }

        private static void AssertMatchingObservations(IReadOnlyDictionary<int, string> expected,
            IReadOnlyDictionary<int, string> actual)
        {
            foreach (var pair in expected)
            {
                Assert.That(actual.ContainsKey(pair.Key), Is.True, "missing matching simulation tick " + pair.Key);
                Assert.That(actual[pair.Key], Is.EqualTo(pair.Value), "presentation drift at simulation tick " + pair.Key);
            }
        }

        private static ManualRig CreateManualRig()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            Assert.That(asset, Is.Not.Null);
            var root = UnityEngine.Object.Instantiate(asset);
            var player = root.GetComponentInChildren<PlayerMovementController>();
            var transfer = root.GetComponentInChildren<TransferSimulationDriver>();
            var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
            var bridge = root.GetComponentInChildren<OrdanBossSimulationDriver>();
            var scheduler = root.GetComponentInChildren<OrdanBossTransferExposureScheduler>();
            var hostile = root.GetComponentInChildren<OrdanBossHostileDamageProducer>();
            var auditPull = root.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>();
            var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
            Assert.That(player, Is.Not.Null); Assert.That(transfer, Is.Not.Null); Assert.That(combat, Is.Not.Null);
            Assert.That(bridge, Is.Not.Null); Assert.That(scheduler, Is.Not.Null); Assert.That(hostile, Is.Not.Null); Assert.That(auditPull, Is.Not.Null); Assert.That(handoff, Is.Not.Null);
            player.enabled = false; transfer.enabled = false; combat.enabled = false; bridge.enabled = false; scheduler.enabled = false; hostile.enabled = false; auditPull.enabled = false; handoff.enabled = false;
            return new ManualRig(root, player, transfer, combat, bridge, scheduler, hostile, auditPull, handoff);
        }

        private static void ManualFixedStep(ManualRig rig)
        {
            var expected = rig.Player.NextExpectedTick;
            Assert.That(rig.Transfer.AdvanceTransferPhaseForTests().Snapshot.Tick, Is.EqualTo(expected));
            Assert.That(rig.Combat.AdvanceCombatPhaseForTests().Tick, Is.EqualTo(expected));
            Assert.That(rig.Bridge.AdvanceBridgePhaseForTests().SourceTick, Is.EqualTo(expected));
            Assert.That(rig.Scheduler.AdvanceSchedulerPhaseForTests().SourceTick, Is.EqualTo(expected));
            Assert.That(rig.Hostile.AdvanceForTests().Tick, Is.EqualTo(expected));
            Assert.That(rig.AuditPull.AdvanceForTests().SourceTick, Is.EqualTo(expected));
            Assert.That(rig.Player.AdvanceOneFixedTickForTests().Tick, Is.EqualTo(expected));
            InvokeHandoff(rig.Handoff);
            Assert.That(rig.Handoff.LatestPresentationView.Value.Tick, Is.EqualTo(expected));
        }

        private static void InvokeHandoff(OrdanBossEncounterHandoffAdapter handoff)
        {
            var method = typeof(OrdanBossEncounterHandoffAdapter).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(handoff, null);
        }

        private static void InvokeTeardown(OrdanBossTerminalTeardown teardown)
        {
            var method = typeof(OrdanBossTerminalTeardown).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(teardown, null);
        }

        private static TransferSimulationInput TransferPress(SimulationTick tick)
        {
            var camera = new SimulationCameraPoseSnapshot(new SimulationTick(tick.Value - 1), 0, 0, 10000, 0,
                1920, 1080, 0, 0, 1920, 1080, 3);
            var aim = new AimSample(9901, AimSource.GamepadStick, null, null, null, 4096, 0,
                camera.CameraPoseTick, tick);
            return new TransferSimulationInput(tick, camera, aim, new TransferPressed(9901, tick),
                Array.Empty<TransferTargetRemoved>(), null);
        }

        private static TransferSimulationInput PayloadTransferPress(SimulationTick tick)
        {
            var camera = new SimulationCameraPoseSnapshot(new SimulationTick(tick.Value - 1), 0, 0, 10000, 0,
                1920, 1080, 0, 0, 1920, 1080, 3);
            var aim = new AimSample(9902, AimSource.GamepadStick, null, null, null, 2170, 3476,
                camera.CameraPoseTick, tick);
            return new TransferSimulationInput(tick, camera, aim, new TransferPressed(9902, tick),
                Array.Empty<TransferTargetRemoved>(), null);
        }

        private static void SeedPlayerPosition(PlayerMovementController player, int xQ4096, int yQ4096)
        {
            var body = player.GetComponent<Rigidbody2D>();
            body.position = new Vector2(xQ4096 / 4096f, yQ4096 / 4096f);
            player.transform.position = body.position;
            var field = typeof(PlayerMovementController).GetField("_motor", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            var motor = (PlayerMovementMotor)field.GetValue(player);
            Assert.That(motor, Is.Not.Null);
            motor.SeedAuthoritativePosition(xQ4096, yQ4096);
        }

        private static int PlayerHealth(OrdanBossEncounterPresentationView view)
        {
            foreach (var combatant in view.Combatants)
                if (combatant.TargetId == "player") return combatant.CurrentHealth;
            throw new AssertionException("player combatant missing");
        }

        private readonly struct ManualRig
        {
            internal ManualRig(GameObject root, PlayerMovementController player, TransferSimulationDriver transfer, OrdanBossCombatSimulationDriver combat,
                OrdanBossSimulationDriver bridge, OrdanBossTransferExposureScheduler scheduler, OrdanBossHostileDamageProducer hostile,
                OrdanBossBalanceAuditPullProducer auditPull, OrdanBossEncounterHandoffAdapter handoff)
            { Root = root; Player = player; Transfer = transfer; Combat = combat; Bridge = bridge; Scheduler = scheduler; Hostile = hostile; AuditPull = auditPull; Handoff = handoff; }
            internal GameObject Root { get; } internal PlayerMovementController Player { get; } internal TransferSimulationDriver Transfer { get; }
            internal OrdanBossCombatSimulationDriver Combat { get; } internal OrdanBossSimulationDriver Bridge { get; }
            internal OrdanBossTransferExposureScheduler Scheduler { get; } internal OrdanBossHostileDamageProducer Hostile { get; }
            internal OrdanBossBalanceAuditPullProducer AuditPull { get; } internal OrdanBossEncounterHandoffAdapter Handoff { get; }
        }

        private readonly struct TerminalCadenceTrace
        {
            internal TerminalCadenceTrace(List<string> entries, string finalHandoffSignature, string finalHandoffHash)
            {
                Entries = Array.AsReadOnly(entries.ToArray());
                FinalHandoffSignature = finalHandoffSignature;
                FinalHandoffHash = finalHandoffHash;
            }

            internal IReadOnlyList<string> Entries { get; }
            internal string FinalHandoffSignature { get; }
            internal string FinalHandoffHash { get; }
        }

        private static string FullViewSignature(OrdanBossEncounterPresentationView value, PlayerMotionSnapshot movement,
            OrdanBossHostileDamageProducer producer, OrdanBossCombatSimulationDriver combat)
        {
            var snapshot = value.Snapshot;
            var builder = new StringBuilder();
            builder.Append(value.Tick.Value).Append('|').Append(value.NextExpectedTick.Value).Append('|')
                .Append((int)snapshot.PatternSlot).Append('|').Append((int)snapshot.Phase).Append('|').Append(snapshot.PhaseAge).Append('|')
                .Append((int)snapshot.ActiveStage).Append('|').Append((int)snapshot.PendingStage).Append('|').Append(snapshot.ActiveHandleId).Append('|')
                .Append(snapshot.IsAccelerated).Append('|').Append(snapshot.IsVulnerable).Append('|').Append((int)snapshot.VulnerabilitySource).Append('|')
                .Append(snapshot.NextExpectedTick.Value).Append('|')
                .Append(value.ActiveTransferTargetId).Append('|').Append((int)value.TransferPlayerState).Append('|')
                .Append(value.ForecastPayloadId).Append('|').Append(value.ExposedPayloadId).Append('|')
                .Append(value.AuditForecastId).Append('|').Append(value.AuditExposedId).Append('|')
                .Append(value.AuditAvailable).Append('|').Append(value.AuditApplied).Append('|').Append(value.IsTerminal);
            builder.Append("|m:").Append(movement.Tick.Value).Append(':').Append(movement.PositionXQ4096).Append(':')
                .Append(movement.PositionYQ4096).Append(':').Append(movement.VelocityXQ4096).Append(':')
                .Append(movement.VelocityYQ4096).Append(':').Append(movement.IsGrounded).Append(':')
                .Append(movement.WallId).Append(':').Append((int)movement.WallSide).Append(':')
                .Append((int)movement.ActionState).Append(':').Append(movement.IsAirDashAvailable);
            var hostile = value.HostileDamageDigest;
            builder.Append("|d:").Append(hostile.Tick.Value).Append(':').Append(hostile.Terminal).Append(':')
                .Append((int)hostile.Disposition).Append(':').Append(hostile.GeometryKey).Append(':').Append(hostile.AttackOrdinal).Append(':')
                .Append(hostile.PayloadId).Append(':').Append(hostile.RequestId).Append(':')
                .Append(hostile.DeliveryTick.HasValue ? hostile.DeliveryTick.Value.Value : -1);
            var geometry = producer.LatestOutcome.Observation;
            builder.Append("|g:").Append(geometry.GeometryKey).Append(':').Append(geometry.Contact).Append(':')
                .Append(geometry.Occluded).Append(':').Append(geometry.StartX).Append(':').Append(geometry.StartY).Append(':')
                .Append(geometry.EndX).Append(':').Append(geometry.EndY).Append(':').Append(geometry.OccluderIndex);
            var pull = value.BalanceAuditPullDigest;
            builder.Append("|pull:").Append(pull.Tick.Value).Append(':').Append((int)pull.Disposition).Append(':')
                .Append(pull.ConsumedCurrentDirectiveId).Append(':').Append(pull.AppliedDeltaXQ4096).Append(':')
                .Append(pull.AppliedDeltaYQ4096).Append(':').Append(pull.CancelledCurrentDirectiveId).Append(':')
                .Append(pull.QueuedNextDirectiveId).Append(':')
                .Append(pull.QueuedNextMovementTick.HasValue ? pull.QueuedNextMovementTick.Value.Value : -1).Append(':')
                .Append(pull.Terminal);
            var pending = combat.ReadPendingDeliveryForTests(new SimulationTick(value.Tick.Value + 1));
            builder.Append("|q:").Append(pending.SourceTick.Value).Append(':').Append(pending.DeliveryTick.Value).Append(':')
                .Append(pending.HostileAppendCommitted).Append(':').Append(pending.HostileRequests.Count);
            if (pending.Request.HasValue) builder.Append(":p:").Append(pending.Request.Value.RequestId);
            if (pending.HostileRequests.Count != 0) builder.Append(":h:").Append(pending.HostileRequests[0].RequestId);
            foreach (var item in value.Intents)
            {
                builder.Append("|i:").Append((int)item.Kind).Append(':').Append(item.Id).Append(':').Append((int)item.PatternSlot).Append(':').Append(item.Tick.Value).Append(':').Append(item.DamageAmount).Append(':').Append(item.DeliveryTick.HasValue ? item.DeliveryTick.Value.Value : -1);
                if (item.FutureDamageRequest.HasValue)
                {
                    var request = item.FutureDamageRequest.Value;
                    builder.Append(":r:").Append(request.RequestId).Append(':').Append(request.SourceId).Append(':').Append(request.TargetId).Append(':').Append(request.Amount).Append(':').Append((int)request.Kind).Append(':').Append(request.Tick.Value);
                }
            }
            foreach (var item in value.Handoffs) builder.Append("|h:").Append((int)item.Kind).Append(':').Append(item.Tick.Value);
            foreach (var item in value.Combatants) builder.Append("|c:").Append(item.TargetId).Append(':').Append(item.CurrentHealth).Append(':').Append(item.IsDead).Append(':').Append(item.InvulnerabilityEndTick).Append(':').Append(item.AcceptedRequestCount);
            foreach (var item in value.PayloadAvailable) builder.Append("|p:").Append(item);
            foreach (var item in value.AuditTemporaryEndedTargetIds) builder.Append("|ae:").Append(item);
            foreach (var item in value.AuditPermanentlyRemovedTargetIds) builder.Append("|ar:").Append(item);
            foreach (var item in value.StableIds) builder.Append("|s:").Append(item);
            return builder.ToString();
        }

        private static int ExecutionOrder<T>() =>
            ((DefaultExecutionOrder)Attribute.GetCustomAttribute(typeof(T), typeof(DefaultExecutionOrder))).order;
    }
}
