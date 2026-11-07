// REQ-COM-004, REQ-WT-003, REQ-WT-005; AC-M4B3C-001, AC-M4B3C-004,
// AC-M4B3C-006, AC-M4B3C-008. Independent Luna contract tests; no Unity
// phase execution is used to claim runtime or PlayMode verification here.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    internal sealed class OrdanBossTerminalTeardownContractTests
    {
        private readonly List<UnityEngine.Object> _objects = new List<UnityEngine.Object>();
        private const string TemporaryScenePath = "Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/__TempOrdanBossTerminalScene.unity";

        [SetUp]
        public void SetUp()
        {
            AssetDatabase.DeleteAsset(TemporaryScenePath);
            Assert.That(EditorSceneManager.SaveScene(EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single), TemporaryScenePath), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            for (var index = _objects.Count - 1; index >= 0; index--)
                if (_objects[index] != null) UnityEngine.Object.DestroyImmediate(_objects[index]);
            _objects.Clear();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AssetDatabase.DeleteAsset(TemporaryScenePath);
            AssetDatabase.Refresh();
        }

        // REQ-COM-004; AC-M4B3C-001, AC-M4B3C-004. The terminal seam is a
        // single -195 owner and the empty delivery fingerprint includes the
        // append-commit bit, not merely an empty hostile-request list.
        [Test]
        public void TerminalOrderAndEmptyDeliveryFingerprintAreExact()
        {
            var attribute = (DefaultExecutionOrder)Attribute.GetCustomAttribute(
                typeof(OrdanBossTerminalTeardown), typeof(DefaultExecutionOrder));
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute.order, Is.EqualTo(-195));

            var empty = new OrdanBossDeliveryBatch(new SimulationTick(0), new SimulationTick(1), null,
                Array.Empty<DamageRequest>(), false);
            var appendCommittedEmpty = new OrdanBossDeliveryBatch(new SimulationTick(0), new SimulationTick(1), null,
                Array.Empty<DamageRequest>(), true);
            Assert.That(empty.HostileRequests, Has.Count.EqualTo(0));
            Assert.That(empty.HostileAppendCommitted, Is.False);
            Assert.That(empty.Equals(appendCommittedEmpty), Is.False);

            var cleanup = ExactCleanupReceipt();
            var handoffs = new[] { OrdanBossHandoffKind.Defeated, OrdanBossHandoffKind.RewardRequest, OrdanBossHandoffKind.RoomCompletionRequest };
            var components = new[] { "combat", "bridge", "scheduler", "hostile", "pull", "handoff", "ordan" };
            var colliders = new[] { "ordan", "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" };
            var receipt = new OrdanBossTerminalTeardownReceipt(cleanup, empty, handoffs, components, colliders);

            Assert.That(receipt.DeathSourceTick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(receipt.CleanupTick, Is.EqualTo(new SimulationTick(1)));
            Assert.That(receipt.DiscardedDelivery.HostileAppendCommitted, Is.False);
            CollectionAssert.AreEqual(handoffs, receipt.HandoffKinds);
            CollectionAssert.AreEqual(components, receipt.DisabledComponents);
            CollectionAssert.AreEqual(colliders, receipt.DisabledColliders);
            Assert.That(receipt.PlayerPreserved, Is.True);
            Assert.That(receipt.Terminal, Is.True);

            Assert.Throws<ArgumentException>(() => new OrdanBossTerminalTeardownReceipt(cleanup,
                appendCommittedEmpty, handoffs, components, colliders));

            var cleanupIds = new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" };
            var defensiveCleanup = new BossTerminalTransferCleanupReceipt(new SimulationTick(0), new SimulationTick(1), null,
                cleanupIds, string.Empty, string.Empty, TransferPlayerState.Baseline, 0, null, true);
            cleanupIds[0] = "forged";
            Assert.That(defensiveCleanup.RequestedIds[0], Is.EqualTo("BossAuditBox-0"));

            var sourceHandoffs = new[] { OrdanBossHandoffKind.Defeated, OrdanBossHandoffKind.RewardRequest, OrdanBossHandoffKind.RoomCompletionRequest };
            var sourceComponents = new[] { "combat", "bridge", "scheduler" };
            var sourceColliders = new[] { "ordan", "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" };
            var defensiveReceipt = new OrdanBossTerminalTeardownReceipt(defensiveCleanup, empty,
                sourceHandoffs, sourceComponents, sourceColliders);
            sourceHandoffs[0] = OrdanBossHandoffKind.RoomCompletionRequest;
            sourceComponents[0] = "forged";
            sourceColliders[0] = "forged";
            Assert.That(defensiveReceipt.HandoffKinds[0], Is.EqualTo(OrdanBossHandoffKind.Defeated));
            Assert.That(defensiveReceipt.DisabledComponents[0], Is.EqualTo("combat"));
            Assert.That(defensiveReceipt.DisabledColliders[0], Is.EqualTo("ordan"));

            var hostileSource = new[] { HostileRequest(1) };
            var defensiveBatch = new OrdanBossDeliveryBatch(new SimulationTick(0), new SimulationTick(1), null,
                hostileSource, true);
            hostileSource[0] = ExternalRequest(1);
            Assert.That(defensiveBatch.HostileRequests[0].RequestId, Is.EqualTo("ordan.hostile/debt-a/0000000000000000000/0000000001"));
        }

        // REQ-COM-004; AC-M4B3C-004, AC-M4B3C-008. A future input is rejected
        // before the exact t+1 delivery is consumed, preserving both queues.
        [Test]
        public void TerminalDiscardRejectsFutureInputWithoutConsumingQueues()
        {
            var rig = CreateCombatRig();
            BootstrapEmptyDelivery(rig);
            var deliveryBefore = rig.Combat.ReadPendingDeliveryForTests(new SimulationTick(1));
            var inputs = PrivateDictionary(rig.Combat, "_inputs");
            rig.Combat.Submit(new OrdanBossCombatInput(new SimulationTick(2), null, null, null, Array.Empty<DamageRequest>()));

            Assert.Throws<InvalidOperationException>(() => rig.Combat.PreflightTerminalDeliveryDiscard(new object(), new SimulationTick(0)));
            Assert.That(rig.Combat.ReadPendingDeliveryForTests(new SimulationTick(1)), Is.EqualTo(deliveryBefore));
            Assert.That(inputs.Contains(2), Is.True);
        }

        // REQ-COM-004; AC-M4B3C-004. The only optional t+1 Combat input
        // accepted by the terminal seam is the exact empty shape; successful
        // commit removes that input and its matching empty delivery together,
        // while the already-published Combat outcome remains unchanged. The
        // authored death-snapshot claim remains covered by the PlayMode path.
        [Test]
        public void TerminalDiscardCommitsExactEmptyInputAndDelivery()
        {
            var rig = CreateCombatRig();
            BootstrapEmptyDelivery(rig);
            var beforeOutcome = rig.Combat.LatestOutcome;
            var inputs = PrivateDictionary(rig.Combat, "_inputs");
            var deliveries = PrivateDictionary(rig.Combat, "_deliveries");
            var tick = new SimulationTick(1);
            rig.Combat.Submit(new OrdanBossCombatInput(tick, null, null, null, Array.Empty<DamageRequest>()));

            var owner = new object();
            var candidate = rig.Combat.PreflightTerminalDeliveryDiscard(owner, new SimulationTick(0));
            Assert.That(candidate.HadEmptyInput, Is.True);
            rig.Combat.CommitPreflightedTerminalDeliveryDiscard(candidate, owner);

            Assert.That(inputs.Contains(tick.Value), Is.False);
            Assert.That(deliveries.Contains(tick.Value), Is.False);
            Assert.That(rig.Combat.LatestOutcome, Is.EqualTo(beforeOutcome));
        }

        // REQ-COM-004; AC-M4B3C-004, AC-M4B3C-008. Forged, cross-graph and
        // stale candidates cannot consume another coordinator's queue.
        [Test]
        public void TerminalDiscardRejectsForgedCrossGraphAndStaleCandidates()
        {
            var first = CreateCombatRig();
            var second = CreateCombatRig();
            BootstrapEmptyDelivery(first);
            BootstrapEmptyDelivery(second);
            var owner = new object();
            var candidate = first.Combat.PreflightTerminalDeliveryDiscard(owner, new SimulationTick(0));

            var forged = new OrdanBossTerminalDeliveryDiscardCandidate(new object(), owner, candidate.Batch, candidate.HadEmptyInput);
            Assert.Throws<InvalidOperationException>(() => first.Combat.CommitPreflightedTerminalDeliveryDiscard(forged, owner));
            Assert.Throws<InvalidOperationException>(() => first.Combat.CommitPreflightedTerminalDeliveryDiscard(null, owner));
            Assert.Throws<InvalidOperationException>(() => first.Combat.CommitPreflightedTerminalDeliveryDiscard(candidate, new object()));
            Assert.That(first.Combat.ReadPendingDeliveryForTests(new SimulationTick(1)).HostileAppendCommitted, Is.False);

            Assert.Throws<InvalidOperationException>(() => second.Combat.CommitPreflightedTerminalDeliveryDiscard(candidate, owner));
            Assert.That(second.Combat.ReadPendingDeliveryForTests(new SimulationTick(1)).HostileAppendCommitted, Is.False);

            first.Combat.CommitPreflightedTerminalDeliveryDiscard(candidate, owner);
            Assert.Throws<InvalidOperationException>(() => first.Combat.ReadPendingDeliveryForTests(new SimulationTick(1)));
            Assert.Throws<InvalidOperationException>(() => first.Combat.CommitPreflightedTerminalDeliveryDiscard(candidate, owner));
        }

        // REQ-COM-004; AC-M4B3C-004, AC-M4B3C-008. HostileAppendCommitted=true
        // and any extra future delivery are invalid terminal candidates; the
        // exact empty delivery remains untouched on rejection.
        [Test]
        public void TerminalDiscardRejectsCommittedEmptyBitAndFutureDelivery()
        {
            var rig = CreateCombatRig();
            BootstrapEmptyDelivery(rig);
            var deliveries = PrivateDictionary(rig.Combat, "_deliveries");
            var exact = rig.Combat.ReadPendingDeliveryForTests(new SimulationTick(1));
            deliveries[1] = new OrdanBossDeliveryBatch(new SimulationTick(0), new SimulationTick(1), null,
                Array.Empty<DamageRequest>(), true);
            Assert.Throws<InvalidOperationException>(() => rig.Combat.PreflightTerminalDeliveryDiscard(new object(), new SimulationTick(0)));
            Assert.That(rig.Combat.ReadPendingDeliveryForTests(new SimulationTick(1)).HostileAppendCommitted, Is.True);

            deliveries[1] = exact;
            deliveries[2] = new OrdanBossDeliveryBatch(new SimulationTick(1), new SimulationTick(2), null);
            Assert.Throws<InvalidOperationException>(() => rig.Combat.PreflightTerminalDeliveryDiscard(new object(), new SimulationTick(0)));
            Assert.That(rig.Combat.ReadPendingDeliveryForTests(new SimulationTick(1)), Is.EqualTo(exact));
            Assert.That(deliveries.Contains(2), Is.True);
        }

        // REQ-COM-004; AC-M4B3C-004, AC-M4B3C-008. Payload, hostile, camera,
        // aim, press and external input make the terminal queue non-empty and
        // must be rejected without consuming any queued state.
        [Test]
        public void TerminalDiscardRejectsActualNonEmptyDeliveryAndInput()
        {
            var rig = CreateCombatRig();
            BootstrapEmptyDelivery(rig);
            var deliveries = PrivateDictionary(rig.Combat, "_deliveries");
            var inputs = PrivateDictionary(rig.Combat, "_inputs");
            var exact = rig.Combat.ReadPendingDeliveryForTests(new SimulationTick(1));

            var payload = new OrdanBossDeliveryBatch(new SimulationTick(0), new SimulationTick(1), PayloadRequest(1));
            deliveries[1] = payload;
            Assert.Throws<InvalidOperationException>(() => rig.Combat.PreflightTerminalDeliveryDiscard(new object(), new SimulationTick(0)));
            Assert.That(rig.Combat.ReadPendingDeliveryForTests(new SimulationTick(1)), Is.EqualTo(payload));

            var hostile = new OrdanBossDeliveryBatch(new SimulationTick(0), new SimulationTick(1), null,
                new[] { HostileRequest(1) }, true);
            deliveries[1] = hostile;
            Assert.Throws<InvalidOperationException>(() => rig.Combat.PreflightTerminalDeliveryDiscard(new object(), new SimulationTick(0)));
            Assert.That(rig.Combat.ReadPendingDeliveryForTests(new SimulationTick(1)), Is.EqualTo(hostile));

            deliveries[1] = exact;
            var tick = new SimulationTick(1);
            var camera = new SimulationCameraPoseSnapshot(new SimulationTick(0), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
            var aim = new AimSample(1, AimSource.GamepadStick, null, null, null, 4096, 0, new SimulationTick(0), tick);
            rig.Combat.Submit(new OrdanBossCombatInput(tick, camera, aim, new BasicAttackPressed(1, 1, tick), Array.Empty<DamageRequest>()));
            Assert.Throws<InvalidOperationException>(() => rig.Combat.PreflightTerminalDeliveryDiscard(new object(), new SimulationTick(0)));
            Assert.That(inputs.Contains(1), Is.True);
            Assert.That(rig.Combat.ReadPendingDeliveryForTests(tick), Is.EqualTo(exact));

            inputs.Remove(1);
            rig.Combat.Submit(new OrdanBossCombatInput(tick, null, null, null, new[] { ExternalRequest(1) }));
            Assert.Throws<InvalidOperationException>(() => rig.Combat.PreflightTerminalDeliveryDiscard(new object(), new SimulationTick(0)));
            Assert.That(inputs.Contains(1), Is.True);
            Assert.That(rig.Combat.ReadPendingDeliveryForTests(tick), Is.EqualTo(exact));
        }

        // REQ-COM-004; AC-M4B3C-004, AC-M4B3C-008. A preflight candidate is
        // single-use and cannot commit after the delivery or input fingerprint
        // changes, including removal of the expected queue entry.
        [Test]
        public void TerminalDiscardCommitRejectsMissingAndStaleFingerprints()
        {
            var missing = CreateCombatRig();
            BootstrapEmptyDelivery(missing);
            var owner = new object();
            var missingCandidate = missing.Combat.PreflightTerminalDeliveryDiscard(owner, new SimulationTick(0));
            PrivateDictionary(missing.Combat, "_deliveries").Remove(1);
            Assert.Throws<InvalidOperationException>(() => missing.Combat.CommitPreflightedTerminalDeliveryDiscard(missingCandidate, owner));

            var stale = CreateCombatRig();
            BootstrapEmptyDelivery(stale);
            var staleOwner = new object();
            var staleCandidate = stale.Combat.PreflightTerminalDeliveryDiscard(staleOwner, new SimulationTick(0));
            var stalePayload = new OrdanBossDeliveryBatch(new SimulationTick(0), new SimulationTick(1), PayloadRequest(1));
            PrivateDictionary(stale.Combat, "_deliveries")[1] = stalePayload;
            Assert.Throws<InvalidOperationException>(() => stale.Combat.CommitPreflightedTerminalDeliveryDiscard(staleCandidate, staleOwner));
            Assert.That(stale.Combat.ReadPendingDeliveryForTests(new SimulationTick(1)), Is.EqualTo(stalePayload));

            var inputChanged = CreateCombatRig();
            BootstrapEmptyDelivery(inputChanged);
            var inputOwner = new object();
            var inputCandidate = inputChanged.Combat.PreflightTerminalDeliveryDiscard(inputOwner, new SimulationTick(0));
            PrivateDictionary(inputChanged.Combat, "_inputs")[1] = new OrdanBossCombatInput(new SimulationTick(1), null, null, null,
                new[] { ExternalRequest(1) });
            Assert.Throws<InvalidOperationException>(() => inputChanged.Combat.CommitPreflightedTerminalDeliveryDiscard(inputCandidate, inputOwner));
            Assert.That(PrivateDictionary(inputChanged.Combat, "_inputs").Contains(1), Is.True);
            Assert.That(inputChanged.Combat.ReadPendingDeliveryForTests(new SimulationTick(1)).HostileAppendCommitted, Is.False);
        }

        // REQ-COM-004; AC-M4B3C-003, AC-M4B3C-008. The terminal Transfer
        // envelope is rejected before ConsumeOrEmpty/TransferSession.Process;
        // the malformed input remains queued and no receipt is published.
        [Test]
        public void MalformedTerminalTransferEnvelopeIsPreservedBeforeConsume()
        {
            var player = CreatePlayer("terminal-transfer-player");
            var registryObject = Track(new GameObject("terminal-transfer-registry"));
            var registry = registryObject.AddComponent<TransferTargetRegistry>();
            var driverObject = Track(new GameObject("terminal-transfer-driver"));
            var driver = driverObject.AddComponent<TransferSimulationDriver>();
            driver.ConfigureForTests(player, registry);
            driver.InitializeForTests();
            InvokePhase(driver, "AdvanceTransferPhaseForTests");
            InvokePhase(player, "AdvanceOneFixedTickForTests");

            var tick = new SimulationTick(1);
            var inputs = PrivateDictionary(driver, "_inputs");
            // The six-argument constructor keeps ExposureEnds empty while the
            // four removal IDs are deliberately malformed; this remains a
            // public-type-only fixture for the Combat.Unity test assembly.
            inputs[1] = CreateMalformedTransferInput(tick);
            SetPrivate(driver, "_bossTerminalCleanupTick", 1);
            var before = driver.SnapshotForTests;

            Assert.Throws<InvalidOperationException>(() => InvokePhase(driver, "AdvanceTransferPhaseForTests"));
            Assert.That(inputs.Contains(1), Is.True);
            Assert.That(driver.LatestBossTerminalCleanupReceipt.HasValue, Is.False);
            var after = driver.SnapshotForTests;
            Assert.That(after.ActiveTargetId, Is.EqualTo(before.ActiveTargetId));
            Assert.That(after.Revision, Is.EqualTo(before.Revision));
            Assert.That(after.PlayerState, Is.EqualTo(before.PlayerState));
        }

        // REQ-COM-004, AC-M4B3C-001. The authored prefab is driven through
        // the real death-source order: -200, -190, -185, -180, -170, -165,
        // Movement, then +110. This proves the exact terminal triplet,
        // four-ID t+1 reservation, terminal producer suppression, and final
        // source-t handoff view without relying on PlayMode scheduling.
        [Test]
        public void AuthoredGraphDeathChainPublishesExactTerminalEvidence()
        {
            var rig = CreateAuthoredRig();
            try
            {
                AdvanceAuthoredTick(rig, null);
                var deathTick = NextExpectedTick(rig.Player);
                var trace = new List<string>();

                InvokePhase(rig.Transfer, "AdvanceTransferPhaseForTests");
                trace.Add("-200");
                rig.Combat.Submit(new OrdanBossCombatInput(deathTick, null, null, null,
                    new[] { new DamageRequest("test/authored/terminal", "test", "ordan", 60, DamageKind.BossPattern, deathTick) }));
                Assert.That(rig.Combat.AdvanceCombatPhaseForTests().Tick, Is.EqualTo(deathTick));
                trace.Add("-190");
                var bridge = rig.Bridge.AdvanceBridgePhaseForTests();
                Assert.That(bridge.SourceTick, Is.EqualTo(deathTick));
                trace.Add("-185");

                var scheduler = rig.Scheduler.AdvanceSchedulerPhaseForTests();
                Assert.That(scheduler.SourceTick, Is.EqualTo(deathTick));
                Assert.That(scheduler.IsTerminal, Is.True);
                CollectionAssert.AreEqual(new[] { "BossWeight-0", "BossWeight-1", "BossWeight-2" }, scheduler.PermanentlyRemovedTargetIds);
                Assert.That(rig.Scheduler.LatestAuditOutcome.IsTerminal, Is.True);
                CollectionAssert.AreEqual(new[] { "BossAuditBox-0" }, rig.Scheduler.LatestAuditOutcome.PermanentlyRemovedTargetIds);
                trace.Add("-180");

                var hostile = rig.Hostile.AdvanceForTests();
                Assert.That(hostile.Digest.Disposition, Is.EqualTo(OrdanBossHostileDisposition.SuppressedBossDead));
                Assert.That(hostile.Digest.Terminal, Is.True);
                trace.Add("-170");
                var pull = rig.AuditPull.AdvanceForTests();
                Assert.That(pull.Disposition, Is.EqualTo(OrdanBossBalanceAuditPullDisposition.SuppressedBossDead));
                Assert.That(pull.Terminal, Is.True);
                trace.Add("-165");

                InvokePhase(rig.Player, "AdvanceOneFixedTickForTests");
                Assert.That(NextExpectedTick(rig.Player), Is.EqualTo(new SimulationTick(deathTick.Value + 1)));
                trace.Add("Movement");
                InvokeHandoff(rig.Handoff);
                var view = rig.Handoff.LatestPresentationView.Value;
                Assert.That(view.Tick, Is.EqualTo(deathTick));
                Assert.That(view.IsTerminal, Is.True);
                Assert.That(view.HostileDamageDigest.Disposition, Is.EqualTo(OrdanBossHostileDisposition.SuppressedBossDead));
                Assert.That(view.BalanceAuditPullDigest.Disposition, Is.EqualTo(OrdanBossBalanceAuditPullDisposition.SuppressedBossDead));
                Assert.That(view.Handoffs.Count, Is.EqualTo(3));
                Assert.That(view.Handoffs[0].Kind, Is.EqualTo(OrdanBossHandoffKind.Defeated));
                Assert.That(view.Handoffs[1].Kind, Is.EqualTo(OrdanBossHandoffKind.RewardRequest));
                Assert.That(view.Handoffs[2].Kind, Is.EqualTo(OrdanBossHandoffKind.RoomCompletionRequest));
                for (var index = 0; index < view.Handoffs.Count; index++)
                    Assert.That(view.Handoffs[index].Tick, Is.EqualTo(deathTick));
                trace.Add("+110");

                var queuedInputs = PrivateDictionary(rig.Transfer, "_inputs");
                var cleanupTick = new SimulationTick(deathTick.Value + 1);
                Assert.That(queuedInputs.Contains(cleanupTick.Value), Is.True);
                var queued = (TransferSimulationInput)queuedInputs[cleanupTick.Value];
                Assert.That(queued.Tick, Is.EqualTo(cleanupTick));
                Assert.That(((ICollection)queued.ExposureEnds).Count, Is.EqualTo(0));
                Assert.That(queued.Removals.Count, Is.EqualTo(4));
                CollectionAssert.AreEqual(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" },
                    new[] { queued.Removals[0].TargetId, queued.Removals[1].TargetId, queued.Removals[2].TargetId, queued.Removals[3].TargetId });
                for (var index = 0; index < queued.Removals.Count; index++)
                    Assert.That(queued.Removals[index].Tick, Is.EqualTo(cleanupTick));

                var deliveries = PrivateDictionary(rig.Combat, "_deliveries");
                Assert.That(deliveries.Count, Is.EqualTo(1));
                var delivery = rig.Combat.ReadPendingDeliveryForTests(cleanupTick);
                Assert.That(delivery.SourceTick, Is.EqualTo(deathTick));
                Assert.That(delivery.DeliveryTick, Is.EqualTo(cleanupTick));
                Assert.That(delivery.Request.HasValue, Is.False);
                Assert.That(delivery.HostileRequests.Count, Is.EqualTo(0));
                Assert.That(delivery.HostileAppendCommitted, Is.False);
                CollectionAssert.AreEqual(new[] { "-200", "-190", "-185", "-180", "-170", "-165", "Movement", "+110" }, trace);
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
        }

        // REQ-COM-004, AC-M4B3C-003. Helper-level negative only: after a
        // valid terminal -200 commit has published its receipt/publication,
        // a mismatched expectation is rejected without rollback. This does
        // not claim a whole-phase re-execution or production callback path.
        [Test]
        public void ValidTerminalCleanupPublicationSurvivesReceiptOnlyMismatch()
        {
            var rig = CreateAuthoredRig();
            try
            {
                AdvanceAuthoredTick(rig, null);
                var deathTick = NextExpectedTick(rig.Player);
                InvokePhase(rig.Transfer, "AdvanceTransferPhaseForTests");
                rig.Combat.Submit(new OrdanBossCombatInput(deathTick, null, null, null,
                    new[] { new DamageRequest("test/authored/postcommit", "test", "ordan", 60, DamageKind.BossPattern, deathTick) }));
                rig.Combat.AdvanceCombatPhaseForTests();
                rig.Bridge.AdvanceBridgePhaseForTests();
                rig.Scheduler.AdvanceSchedulerPhaseForTests();
                rig.Hostile.AdvanceForTests();
                rig.AuditPull.AdvanceForTests();
                InvokePhase(rig.Player, "AdvanceOneFixedTickForTests");
                InvokeHandoff(rig.Handoff);

                var cleanupTick = NextExpectedTick(rig.Player);
                var validOutcome = InvokePhase(rig.Transfer, "AdvanceTransferPhaseForTests");
                var beforeReceipt = rig.Transfer.LatestBossTerminalCleanupReceipt.Value;
                var beforePublication = rig.Transfer.LatestPhasePublication.Value;
                var beforeQueueCount = PrivateDictionary(rig.Transfer, "_inputs").Count;
                var expectationType = typeof(BossTerminalCleanupExpectation);
                var constructor = expectationType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                    new[] { typeof(SimulationTick), typeof(SimulationTick), typeof(TransferClearReason?), typeof(string), typeof(int) }, null);
                Assert.That(constructor, Is.Not.Null);
                var mismatchedExpectation = constructor.Invoke(new object[] { deathTick, cleanupTick, null,
                    beforeReceipt.ActiveTargetBefore, beforeReceipt.Revision + 1 });
                var build = typeof(TransferSimulationDriver).GetMethod("BuildBossTerminalCleanupReceipt",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.That(build, Is.Not.Null);
                Assert.Throws<TargetInvocationException>(() => build.Invoke(rig.Transfer,
                    new object[] { mismatchedExpectation, ExactTerminalTransferInput(cleanupTick), validOutcome }));

                var afterReceipt = rig.Transfer.LatestBossTerminalCleanupReceipt.Value;
                var afterPublication = rig.Transfer.LatestPhasePublication.Value;
                Assert.That(afterReceipt.DeathSourceTick, Is.EqualTo(beforeReceipt.DeathSourceTick));
                Assert.That(afterReceipt.CleanupTick, Is.EqualTo(beforeReceipt.CleanupTick));
                Assert.That(afterReceipt.Revision, Is.EqualTo(beforeReceipt.Revision));
                Assert.That(afterReceipt.DurableRemovalCommitted, Is.EqualTo(beforeReceipt.DurableRemovalCommitted));
                Assert.That(afterPublication.Tick, Is.EqualTo(beforePublication.Tick));
                Assert.That(PrivateDictionary(rig.Transfer, "_inputs").Count, Is.EqualTo(beforeQueueCount));
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
        }

        // REQ-COM-004, AC-M4B3C-003. TransferSimulationInput is immutable
        // and rejects a removal whose tick is not the input phase tick before
        // any queue or session consumer can observe it.
        [Test]
        public void TransferInputRejectsWrongRemovalTickBeforeQueueUse()
        {
            Assert.Throws<ArgumentException>(() => new TransferSimulationInput(new SimulationTick(1), null, null, null,
                new[] { new TransferTargetRemoved("BossAuditBox-0", new SimulationTick(2)) }, null));
        }

        // REQ-COM-004; AC-M4B3C-006. Static destructive-action guard scope is
        // limited to the new teardown runtime file; authoring rebuild code is
        // intentionally outside this test's scan.
        [Test]
        public void TeardownRuntimeContainsNoDestructiveGraphOperation()
        {
            var path = Path.Combine(Application.dataPath, "AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTerminalTeardown.cs");
            Assert.That(File.Exists(path), Is.True, path);
            var source = File.ReadAllText(path);
            Assert.That(source, Does.Not.Contain("SetActive("));
            Assert.That(source, Does.Not.Contain("Destroy("));
            Assert.That(source, Does.Not.Contain("DestroyImmediate("));
            Assert.That(source, Does.Not.Contain("SceneManager.Load"));
            StringAssert.Contains("BossAuditBox-0", source);
            StringAssert.Contains("BossWeight-0", source);
            StringAssert.Contains("BossWeight-1", source);
            StringAssert.Contains("BossWeight-2", source);
        }

        private CombatRig CreateCombatRig()
        {
            var player = CreatePlayer("combat-player");
            var playerCollider = player.GetComponent<Collider2D>();
            var playerTarget = player.gameObject.AddComponent<CombatTarget>();
            playerTarget.ConfigureForTests("player", 5, 45, false, 0, 0, 0, 0, 100, 100, playerCollider, player.transform, null);
            var ordanObject = Track(new GameObject("ordan"));
            var ordanCollider = ordanObject.AddComponent<BoxCollider2D>();
            var ordan = ordanObject.AddComponent<CombatTarget>();
            ordan.ConfigureForTests("ordan", 60, 0, true, 0, 0, 0, 0, 100, 100, ordanCollider, ordanObject.transform, null);
            var registry = Track(new GameObject("combat-registry")).AddComponent<CombatTargetRegistry>();
            registry.ConfigureForTests(ordan, playerTarget);
            var transferRegistry = Track(new GameObject("transfer-registry")).AddComponent<TransferTargetRegistry>();
            var transfer = Track(new GameObject("transfer")).AddComponent<TransferSimulationDriver>();
            transfer.ConfigureForTests(player, transferRegistry);
            var combat = Track(new GameObject("combat")).AddComponent<OrdanBossCombatSimulationDriver>();
            combat.ConfigureForTests(player, registry, transfer, combat);
            var bridge = Track(new GameObject("bridge")).AddComponent<OrdanBossSimulationDriver>();
            bridge.ConfigureForTests(player, transfer, combat);
            bridge.InitializeForTests();
            return new CombatRig(player, transfer, combat, bridge);
        }

        private static void BootstrapEmptyDelivery(CombatRig rig)
        {
            InvokePhase(rig.Transfer, "AdvanceTransferPhaseForTests");
            rig.Combat.AdvanceCombatPhaseForTests();
            rig.Bridge.AdvanceBridgePhaseForTests();
        }

        private AuthoredRig CreateAuthoredRig()
        {
            const string path = "Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab";
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.That(asset, Is.Not.Null, path);
            var root = Track(UnityEngine.Object.Instantiate(asset));
            Assert.That(root.scene.name, Is.EqualTo("__TempOrdanBossTerminalScene"));
            var player = root.GetComponentInChildren<PlayerMovementController>(true);
            var transfer = root.GetComponentInChildren<TransferSimulationDriver>(true);
            var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>(true);
            var bridge = root.GetComponentInChildren<OrdanBossSimulationDriver>(true);
            var scheduler = root.GetComponentInChildren<OrdanBossTransferExposureScheduler>(true);
            var hostile = root.GetComponentInChildren<OrdanBossHostileDamageProducer>(true);
            var auditPull = root.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>(true);
            var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>(true);
            var teardown = root.GetComponentInChildren<OrdanBossTerminalTeardown>(true);
            Assert.That(player, Is.Not.Null); Assert.That(transfer, Is.Not.Null); Assert.That(combat, Is.Not.Null);
            Assert.That(bridge, Is.Not.Null); Assert.That(scheduler, Is.Not.Null); Assert.That(hostile, Is.Not.Null);
            Assert.That(auditPull, Is.Not.Null); Assert.That(handoff, Is.Not.Null); Assert.That(teardown, Is.Not.Null);
            // EditMode prefab instantiation does not provide the PlayMode Awake
            // ordering that registers the bridge-owned combat delivery lane.
            // Initialize before disabling authored components to mirror that
            // production bootstrap ordering; the prefab supplies the bindings.
            bridge.InitializeForTests();
            // PlayMode Awake also registers the scheduler's private exposure-end
            // lane and the BalanceAudit pull producer before the first -200
            // Transfer phase. Reproduce that ordering while Transfer remains
            // pristine so the authored EditMode fixture has the same owners.
            scheduler.InitializeForTests();
            auditPull.InitializeForTests();
            player.enabled = false; transfer.enabled = false; combat.enabled = false; bridge.enabled = false;
            scheduler.enabled = false; hostile.enabled = false; auditPull.enabled = false; handoff.enabled = false; teardown.enabled = false;
            return new AuthoredRig(root, player, transfer, combat, bridge, scheduler, hostile, auditPull, handoff);
        }

        private static void AdvanceAuthoredTick(AuthoredRig rig, OrdanBossCombatInput? input)
        {
            var expected = NextExpectedTick(rig.Player);
            InvokePhase(rig.Transfer, "AdvanceTransferPhaseForTests");
            if (input.HasValue) rig.Combat.Submit(input.Value);
            Assert.That(rig.Combat.AdvanceCombatPhaseForTests().Tick, Is.EqualTo(expected));
            Assert.That(rig.Bridge.AdvanceBridgePhaseForTests().SourceTick, Is.EqualTo(expected));
            Assert.That(rig.Scheduler.AdvanceSchedulerPhaseForTests().SourceTick, Is.EqualTo(expected));
            Assert.That(rig.Hostile.AdvanceForTests().Tick, Is.EqualTo(expected));
            Assert.That(rig.AuditPull.AdvanceForTests().SourceTick, Is.EqualTo(expected));
            InvokePhase(rig.Player, "AdvanceOneFixedTickForTests");
            Assert.That(NextExpectedTick(rig.Player), Is.EqualTo(new SimulationTick(expected.Value + 1)));
            InvokeHandoff(rig.Handoff);
        }

        private static SimulationTick NextExpectedTick(PlayerMovementController player)
        {
            var property = typeof(PlayerMovementController).GetProperty("NextExpectedTick", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(property, Is.Not.Null);
            return (SimulationTick)property.GetValue(player, null);
        }

        private static void InvokeHandoff(OrdanBossEncounterHandoffAdapter handoff)
        {
            var method = typeof(OrdanBossEncounterHandoffAdapter).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(handoff, null);
        }

        private static TransferSimulationInput ExactTerminalTransferInput(SimulationTick cleanupTick)
        {
            return new TransferSimulationInput(cleanupTick, null, null, null,
                new[]
                {
                    new TransferTargetRemoved("BossAuditBox-0", cleanupTick),
                    new TransferTargetRemoved("BossWeight-0", cleanupTick),
                    new TransferTargetRemoved("BossWeight-1", cleanupTick),
                    new TransferTargetRemoved("BossWeight-2", cleanupTick)
                }, null);
        }

        private PlayerMovementController CreatePlayer(string name)
        {
            var value = Track(new GameObject(name));
            value.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            value.AddComponent<BoxCollider2D>();
            return value.AddComponent<PlayerMovementController>();
        }

        private GameObject Track(GameObject value)
        {
            _objects.Add(value);
            return value;
        }

        private static BossTerminalTransferCleanupReceipt ExactCleanupReceipt()
        {
            return new BossTerminalTransferCleanupReceipt(new SimulationTick(0), new SimulationTick(1), null,
                new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" },
                string.Empty, string.Empty, TransferPlayerState.Baseline, 0, null, true);
        }

        private static DamageRequest PayloadRequest(int tick)
        {
            return new DamageRequest("ordan.payload/0000000000000000000/0000000001", "BossWeight-0", "ordan", 8,
                DamageKind.BossPayloadTransfer, new SimulationTick(tick));
        }

        private static DamageRequest HostileRequest(int tick)
        {
            return new DamageRequest("ordan.hostile/debt-a/0000000000000000000/0000000001", "ordan", "player", 1,
                DamageKind.BossPattern, new SimulationTick(tick));
        }

        private static DamageRequest ExternalRequest(int tick)
        {
            return new DamageRequest("external/terminal-negative", "external", "ordan", 1,
                DamageKind.BossPattern, new SimulationTick(tick));
        }

        private static IDictionary PrivateDictionary(object instance, string fieldName)
        {
            var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, fieldName);
            return (IDictionary)field.GetValue(instance);
        }

        private static void SetPrivate(object instance, string fieldName, object value)
        {
            var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, fieldName);
            field.SetValue(instance, value);
        }

        private static object CreateMalformedTransferInput(SimulationTick tick)
        {
            var type = typeof(TransferSimulationDriver).Assembly.GetType("AcadeGameMaker.Transfer.Unity.TransferSimulationInput");
            Assert.That(type, Is.Not.Null);
            var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { typeof(SimulationTick), typeof(SimulationCameraPoseSnapshot?), typeof(AimSample?), typeof(TransferPressed?),
                    typeof(IReadOnlyList<TransferTargetRemoved>), typeof(TransferClearReason?) }, null);
            Assert.That(constructor, Is.Not.Null);
            var removals = new[]
            {
                new TransferTargetRemoved("foreign", tick),
                new TransferTargetRemoved("BossWeight-0", tick),
                new TransferTargetRemoved("BossWeight-1", tick),
                new TransferTargetRemoved("BossWeight-2", tick)
            };
            return constructor.Invoke(new object[] { tick, null, null, null, removals, null });
        }

        private static object InvokePhase(object instance, string methodName)
        {
            var method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null, methodName);
            try { return method.Invoke(instance, null); }
            catch (TargetInvocationException exception) when (exception.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                throw;
            }
        }

        private sealed class CombatRig
        {
            internal CombatRig(PlayerMovementController player, TransferSimulationDriver transfer,
                OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge)
            { Player = player; Transfer = transfer; Combat = combat; Bridge = bridge; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal OrdanBossCombatSimulationDriver Combat { get; }
            internal OrdanBossSimulationDriver Bridge { get; }
        }

        private sealed class AuthoredRig
        {
            internal AuthoredRig(GameObject root, PlayerMovementController player, TransferSimulationDriver transfer,
                OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge,
                OrdanBossTransferExposureScheduler scheduler, OrdanBossHostileDamageProducer hostile,
                OrdanBossBalanceAuditPullProducer auditPull, OrdanBossEncounterHandoffAdapter handoff)
            { Root = root; Player = player; Transfer = transfer; Combat = combat; Bridge = bridge; Scheduler = scheduler; Hostile = hostile; AuditPull = auditPull; Handoff = handoff; }
            internal GameObject Root { get; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal OrdanBossCombatSimulationDriver Combat { get; }
            internal OrdanBossSimulationDriver Bridge { get; }
            internal OrdanBossTransferExposureScheduler Scheduler { get; }
            internal OrdanBossHostileDamageProducer Hostile { get; }
            internal OrdanBossBalanceAuditPullProducer AuditPull { get; }
            internal OrdanBossEncounterHandoffAdapter Handoff { get; }
        }
    }
}
