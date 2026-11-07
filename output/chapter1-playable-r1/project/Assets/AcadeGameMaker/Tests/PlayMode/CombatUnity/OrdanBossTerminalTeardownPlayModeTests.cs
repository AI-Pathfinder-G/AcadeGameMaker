// REQ-COM-004, REQ-WT-003, REQ-WT-005; AC-M4B3C-005..008.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Transfer;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class OrdanBossTerminalTeardownPlayModeTests
    {
        [UnityEngine.SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _runtimeFixture=new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnityEngine.TestTools.UnitySetUp] public System.Collections.IEnumerator BeginRuntimeCase(){_runtimeFixture.PrepareEnter();yield return new UnityEngine.TestTools.EnterPlayMode();_runtimeFixture.ConfirmEntered();yield return _runtimeFixture.RunBody(_runtimeFixture.BeginCase(false));}
        [UnityEngine.TestTools.UnityTearDown] public System.Collections.IEnumerator CloseRuntimeCase(){try{if(_runtimeFixture.OwnsRuntimeFixture && UnityEngine.Application.isPlaying)yield return _runtimeFixture.RunBody(_runtimeFixture.CloseCase(),true);}finally{}if(_runtimeFixture.OwnsRuntimeFixture){if(UnityEngine.Application.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();_runtimeFixture.CaptureCleanupFailure(_runtimeFixture.CompleteExit);}_runtimeFixture.ThrowCleanupFailure();}
        

        private const string GraphPath = "Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab";

        // AC-M4B3C-005. Calling the -195 seam before terminal scheduler state
        // is a strict no-op: no receipt and no component state is changed.
        [UnityTest]
        public IEnumerator PreterminalTeardownCallbackIsMutationFree()
        {yield return _runtimeFixture.RunBody(PreterminalTeardownCallbackIsMutationFreeOriginalBody());}
        private IEnumerator PreterminalTeardownCallbackIsMutationFreeOriginalBody(){
            var root = InstantiateGraph();
            try
            {
                var teardown = root.GetComponentInChildren<OrdanBossTerminalTeardown>();
                var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
                var transfer = root.GetComponentInChildren<TransferSimulationDriver>();
                Assert.That(teardown, Is.Not.Null); Assert.That(combat.enabled && transfer.enabled, Is.True);
                // Bootstrap proof: no fixed phase has run yet.
                InvokeFixed(teardown);
                Assert.That(teardown.LatestReceipt.HasValue, Is.False);
                Assert.That(teardown.enabled && combat.enabled && transfer.enabled && root.activeInHierarchy, Is.True);
                yield return new WaitForFixedUpdate();
                // Repeating the preterminal callback after bootstrap remains a no-op.
                InvokeFixed(teardown);
                Assert.That(teardown.LatestReceipt.HasValue, Is.False);
            }
            finally { UnityEngine.Object.Destroy(root); }
        }

        // AC-M4B3C-005, AC-M4B3C-006, AC-M4B3C-007. The automatic t+1
        // coordinator freezes the death-t boss snapshot, preserves exact player
        // and registry references, then permits two further Transfer/Movement ticks.
        [UnityTest]
        public IEnumerator AutomaticTerminalTeardownPreservesFrozenReferencesAndPlayerContinuation()
        {yield return _runtimeFixture.RunBody(AutomaticTerminalTeardownPreservesFrozenReferencesAndPlayerContinuationOriginalBody());}
        private IEnumerator AutomaticTerminalTeardownPreservesFrozenReferencesAndPlayerContinuationOriginalBody(){
            var root = InstantiateGraph();
            try
            {
                yield return new WaitForFixedUpdate();
                var player = root.GetComponentInChildren<PlayerMovementController>();
                var body = player.GetComponent<Rigidbody2D>(); var playerCollider = player.GetComponent<Collider2D>();
                var transfer = root.GetComponentInChildren<TransferSimulationDriver>();
                var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
                var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                var teardown = root.GetComponentInChildren<OrdanBossTerminalTeardown>();
                var registry = transfer.BoundRegistry; var combatRegistry = combat.BoundRegistry;
                var tick = player.NextExpectedTick;
                combat.Submit(new OrdanBossCombatInput(tick, null, null, null, new[]
                {
                    new DamageRequest("terminal-playmode-lethal", "test", "ordan", 60, DamageKind.BossPattern, tick)
                }));
                yield return new WaitForFixedUpdate();
                var deathView = handoff.LatestPresentationView.Value;
                var deathCombat = combat.LatestOutcome;
                Assert.That(deathView.IsTerminal, Is.True);
                yield return new WaitForFixedUpdate();

                Assert.That(teardown.LatestReceipt.HasValue, Is.True);
                Assert.That(teardown.LatestReceipt.Value.DeathSourceTick, Is.EqualTo(tick));
                Assert.That(teardown.LatestReceipt.Value.CleanupTick, Is.EqualTo(new SimulationTick(tick.Value + 1)));
                Assert.That(root.activeInHierarchy && player.enabled && transfer.enabled && registry.enabled && combatRegistry.enabled, Is.True);
                Assert.That(player.GetComponent<Rigidbody2D>(), Is.SameAs(body)); Assert.That(player.GetComponent<Collider2D>(), Is.SameAs(playerCollider));
                Assert.That(combat.enabled, Is.False); Assert.That(handoff.enabled, Is.False);
                Assert.That(combat.LatestOutcome, Is.EqualTo(deathCombat));
                Assert.That(handoff.LatestPresentationView.Value, Is.EqualTo(deathView));
                foreach (var target in registry.Targets) Assert.That(target, Is.Not.Null);

                transfer.Submit(PressInput(player.NextExpectedTick.Value));
                yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
                Assert.That(player.Snapshot.Tick.Value, Is.EqualTo(tick.Value + 3));
                Assert.That(transfer.SnapshotForTests.PlayerState, Is.EqualTo(AcadeGameMaker.Transfer.TransferPlayerState.Baseline));
                Assert.That(transfer.SnapshotForTests.ActiveTargetId, Is.Empty, "removed boss targets must not be reacquired by a later press");
                Assert.That(teardown.LatestReceipt.Value.Cleanup.RequestedIds, Is.EqualTo(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
                Assert.That(SessionHasNoRegistrations(transfer, "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2"), Is.True);
                foreach (var target in registry.Targets) Assert.That(target.TargetCollider.enabled, Is.False);
                Assert.That(player.GetComponent<Rigidbody2D>(), Is.SameAs(body)); Assert.That(player.GetComponent<Collider2D>(), Is.SameAs(playerCollider));
                Assert.That(combat.LatestOutcome, Is.EqualTo(deathCombat));
                Assert.That(handoff.LatestPresentationView.Value, Is.EqualTo(deathView));
            }
            finally { UnityEngine.Object.Destroy(root); }
        }

        // AC-M4B3C-005, AC-M4B3C-008. Coordinator latches are instance-local:
        // two authored graphs may consume the same source tick, while a second
        // direct advance of either completed instance fail-stops without revival.
        [UnityTest]
        public IEnumerator TerminalTeardownIsInstanceLocalAndOneShot()
        {yield return _runtimeFixture.RunBody(TerminalTeardownIsInstanceLocalAndOneShotOriginalBody());}
        private IEnumerator TerminalTeardownIsInstanceLocalAndOneShotOriginalBody(){
            var first = InstantiateGraph(); var second = InstantiateGraph();
            try
            {
                yield return new WaitForFixedUpdate();
                var firstCombat = first.GetComponentInChildren<OrdanBossCombatSimulationDriver>(); var secondCombat = second.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
                var firstPlayer = first.GetComponentInChildren<PlayerMovementController>(); var secondPlayer = second.GetComponentInChildren<PlayerMovementController>();
                var firstTeardown = first.GetComponentInChildren<OrdanBossTerminalTeardown>(); var secondTeardown = second.GetComponentInChildren<OrdanBossTerminalTeardown>();
                var tick = firstPlayer.NextExpectedTick; Assert.That(secondPlayer.NextExpectedTick, Is.EqualTo(tick));
                firstCombat.Submit(Lethal(tick, "instance-one")); secondCombat.Submit(Lethal(tick, "instance-two"));
                yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
                Assert.That(firstTeardown.LatestReceipt.HasValue && secondTeardown.LatestReceipt.HasValue, Is.True);
                Assert.That(firstTeardown.LatestReceipt.Value.DeathSourceTick, Is.EqualTo(secondTeardown.LatestReceipt.Value.DeathSourceTick));
                Assert.That(firstCombat.enabled || secondCombat.enabled, Is.False);
                Assert.Throws<TargetInvocationException>(() => InvokeFixed(firstTeardown));
                Assert.That(firstTeardown.enabled && firstCombat.enabled, Is.False);
            }
            finally { UnityEngine.Object.Destroy(first); UnityEngine.Object.Destroy(second); }
        }

        // AC-M4B3C-008. Every case starts from the same fully prepared normal
        // terminal state; exactly one proof/binding/enabled value is changed.
        // The failed -195 callback must preserve the empty delivery, all boss
        // enabled states, final handoff, and absence of a teardown receipt.
        [TestCase("missing-handoff")]
        [TestCase("input-only")]
        [TestCase("partial-disabled")]
        [TestCase("foreign-same-root")]
        [TestCase("wrong-handoff-order")]
        [TestCase("wrong-handoff-tick")]
        [TestCase("wrong-collider")]
        [TestCase("target-enabled")]
        [TestCase("sink-drift")]
        public void PreparedTerminalNegativesFailStopWithoutMutation(string failure)
        {
            var rig = PrepareManualTerminal();
            try
            {
                var delivery = rig.Combat.ReadPendingDeliveryForTests(rig.CleanupTick);
                var handoff = rig.Handoff.LatestPresentationView;
                if (failure == "missing-handoff") SetField(rig.Handoff, "_latest", null);
                else if (failure == "input-only") SetField(rig.Transfer, "_latestBossTerminalCleanupReceipt",
                    new BossTerminalTransferCleanupReceipt(rig.DeathTick, rig.CleanupTick, null,
                        new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }, string.Empty, string.Empty,
                        TransferPlayerState.Baseline, 0, null, false));
                else if (failure == "partial-disabled") rig.Combat.enabled = false;
                else if (failure == "wrong-handoff-order") SetField(rig.Handoff, "_latest", CopyView(handoff.Value,
                    new[] { handoff.Value.Handoffs[1], handoff.Value.Handoffs[0], handoff.Value.Handoffs[2] }));
                else if (failure == "wrong-handoff-tick") SetField(rig.Handoff, "_latest", CopyView(handoff.Value,
                    new[] { new OrdanBossHandoff(OrdanBossHandoffKind.Defeated, new SimulationTick(rig.DeathTick.Value + 1)), handoff.Value.Handoffs[1], handoff.Value.Handoffs[2] }));
                else if (failure == "wrong-collider")
                {
                    var foreign = new GameObject("same-root-wrong-collider"); foreign.transform.SetParent(rig.Root.transform);
                    var colliders = (List<Collider2D>)rig.Teardown.GetType().GetField("_targetColliders", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(rig.Teardown);
                    colliders[0] = foreign.AddComponent<BoxCollider2D>();
                }
                else if (failure == "target-enabled") rig.Transfer.BoundRegistry.Targets[0].TargetCollider.enabled = true;
                else if (failure == "sink-drift") ((OrdanBossAuditTransferModifierSink)rig.Transfer.BoundRegistry.Targets[0].TargetOwnerSink).CommitExposureForM4B3B(true);
                else
                {
                    var foreignObject = new GameObject("foreign-hostile"); foreignObject.transform.SetParent(rig.Root.transform);
                    SetField(rig.Teardown, "_hostile", foreignObject.AddComponent<OrdanBossHostileDamageProducer>());
                }
                Assert.Throws<TargetInvocationException>(() => InvokeFixed(rig.Teardown));
                Assert.That(rig.Teardown.LatestReceipt.HasValue, Is.False);
                Assert.That(rig.Combat.ReadPendingDeliveryForTests(rig.CleanupTick), Is.EqualTo(delivery));
                Assert.That(rig.Bridge.enabled && rig.Scheduler.enabled && rig.Hostile.enabled && rig.Pull.enabled && rig.Handoff.enabled, Is.True);
                if (failure != "partial-disabled") Assert.That(rig.Combat.enabled, Is.True);
                if (failure != "missing-handoff" && failure != "wrong-handoff-order" && failure != "wrong-handoff-tick")
                    Assert.That(rig.Handoff.LatestPresentationView, Is.EqualTo(handoff));
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
        }

        // AC-M4B3C-003. Every malformed terminal lane fails before the normal
        // -200 consume/session path, retaining its queued input and baseline.
        [TestCase("exposure")]
        [TestCase("wrong-order")]
        [TestCase("duplicate")]
        public void MalformedTerminalEnvelopeIsPreservedBeforeConsume(string shape)
        {
            var rig = CreateTransferRig(null);
            try
            {
                rig.Driver.AdvanceTransferPhaseForTests(); rig.Player.AdvanceOneFixedTickForTests();
                var tick = new SimulationTick(1); var removals = Removals(1);
                if (shape == "wrong-order") { var swap = removals[0]; removals[0] = removals[1]; removals[1] = swap; }
                if (shape == "duplicate") removals[3] = new TransferTargetRemoved("BossWeight-1", tick);
                var ends = shape == "exposure" ? new[] { new TransferTargetExposureEnded("BossAuditBox-0", tick) } : Array.Empty<TransferTargetExposureEnded>();
                Inputs(rig.Driver)[1] = new TransferSimulationInput(tick, null, null, null, removals, ends, null);
                SetField(rig.Driver, "_bossTerminalCleanupTick", 1);
                var before = rig.Driver.SnapshotForTests;
                Assert.Throws<InvalidOperationException>(() => rig.Driver.AdvanceTransferPhaseForTests());
                Assert.That(Inputs(rig.Driver).Contains(1), Is.True);
                Assert.That(rig.Driver.LatestBossTerminalCleanupReceipt.HasValue, Is.False);
                Assert.That(rig.Driver.SnapshotForTests.Revision, Is.EqualTo(before.Revision));
            }
            finally { DestroyTransferRig(rig); }
        }

        // AC-M4B3C-003. Repeated reservation and a missed terminal due tick
        // are both fail-stop; neither is allowed to become generic input work.
        [Test]
        public void DuplicateAndStaleTerminalReservationsPreserveTransferState()
        {
            var duplicate = CreateTransferRig(null);
            try
            {
                duplicate.Driver.MergeExactBossTerminalRemovals(Removals(1));
                var queued = Inputs(duplicate.Driver)[1];
                Assert.Throws<InvalidOperationException>(() => duplicate.Driver.MergeExactBossTerminalRemovals(Removals(1)));
                Assert.That(Inputs(duplicate.Driver)[1], Is.EqualTo(queued));
                Assert.That(duplicate.Driver.LatestBossTerminalCleanupReceipt.HasValue, Is.False);
            }
            finally { DestroyTransferRig(duplicate); }

            var stale = CreateTransferRig(null);
            try
            {
                stale.Driver.AdvanceTransferPhaseForTests(); stale.Player.AdvanceOneFixedTickForTests();
                SetField(stale.Driver, "_bossTerminalCleanupTick", 0);
                var before = stale.Driver.SnapshotForTests;
                Assert.Throws<InvalidOperationException>(() => stale.Driver.AdvanceTransferPhaseForTests());
                Assert.That(stale.Driver.SnapshotForTests.Revision, Is.EqualTo(before.Revision));
                Assert.That(stale.Driver.LatestBossTerminalCleanupReceipt.HasValue, Is.False);
            }
            finally { DestroyTransferRig(stale); }
        }

        // AC-M4B3C-002. The -180 lane is reserved while the death-source
        // phase is still pending, so active audit and payload paths clear at
        // the following -200 with the exact TargetRemoved revision.
        [TestCase("BossAuditBox-0")]
        [TestCase("BossWeight-0")]
        public void ActiveTerminalCleanupClearsExactOwnerAtNextTick(string activeId)
        {
            var rig = CreateTransferRig(activeId);
            try
            {
                rig.Driver.AdvanceTransferPhaseForTests();
                rig.Driver.MergeExactBossTerminalRemovals(Removals(2));
                rig.Player.AdvanceOneFixedTickForTests();
                rig.Driver.Submit(PressInput(1));
                Assert.That(rig.Driver.AdvanceTransferPhaseForTests().Snapshot.ActiveTargetId, Is.EqualTo(activeId));
                rig.Player.AdvanceOneFixedTickForTests();
                var outcome = rig.Driver.AdvanceTransferPhaseForTests();
                var receipt = rig.Driver.LatestBossTerminalCleanupReceipt.Value;
                Assert.That(outcome.RemovalResult.Value.Cleared.Value.Reason, Is.EqualTo(TransferClearReason.TargetRemoved));
                Assert.That(outcome.RemovalResult.Value.Cleared.Value.PreviousTargetId, Is.EqualTo(activeId));
                Assert.That(receipt.DurableRemovalCommitted, Is.True);
                Assert.That(receipt.Clear.Value.Revision, Is.EqualTo(2));
                Assert.That(receipt.Clear.Value.Tick, Is.EqualTo(new SimulationTick(2)));
                Assert.That(outcome.Snapshot.PlayerState, Is.EqualTo(TransferPlayerState.Baseline));
                Assert.That(SessionHasNoRegistrations(rig.Driver, "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2"), Is.True);
                Assert.That(HasExactRemovedIds(rig.Driver), Is.True);
            }
            finally { DestroyTransferRig(rig); }
        }

        // REQ/AC-AUTOCH1-BOSSTERMINALMERGE-001: opposite order, using existing candidate and completed-publication evidence only.
        [TestCase(false)] [TestCase(true)]
        public void BossFailureMergesAfterTerminalReservation(bool activePayload)
        {
            var rig=CreateTransferRig(activePayload?"BossWeight-0":null);var owner=new object();
            try
            {
                rig.Driver.RegisterInputGateOwner(owner);rig.Driver.AdvanceTransferPhaseForTests();
                var cleanup=1;
                if(activePayload){rig.Player.AdvanceOneFixedTickForTests();rig.Driver.Submit(PressInput(1));rig.Driver.AdvanceTransferPhaseForTests();cleanup=2;}
                var queued=PressInput(cleanup);rig.Driver.Submit(queued);rig.Driver.MergeExactBossTerminalRemovals(Removals(cleanup));rig.Player.AdvanceOneFixedTickForTests();
                var tick=new SimulationTick(cleanup);var before=rig.Driver.PrepareInputFrame(owner,tick,false,null);
                rig.Driver.MergeExactBossTerminalRunFailed(owner,tick);
                Assert.Throws<InvalidOperationException>(()=>rig.Driver.ValidateInputFrameCandidate(owner,before));
                var after=rig.Driver.PrepareInputFrame(owner,tick,false,null);rig.Driver.ValidateInputFrameCandidate(owner,after);
                Assert.Throws<InvalidOperationException>(()=>rig.Driver.Submit(new TransferSimulationInput(tick,null,null,null,Array.Empty<TransferTargetRemoved>(),null)));
                rig.Driver.ValidateInputFrameCandidate(owner,after);
                var outcome=rig.Driver.AdvanceTransferPhaseForTests();var actual=rig.Driver.LatestPhasePublication.Value.Input;
                Assert.That(actual.HadAim,Is.EqualTo(queued.Aim.HasValue));Assert.That(actual.HadPress,Is.EqualTo(queued.Press.HasValue));Assert.That(actual.LifecycleReason,Is.EqualTo(TransferClearReason.RunFailed));
                Assert.That(actual.RemovalCount,Is.EqualTo(4));var ids=rig.Driver.LatestCompletedInputRemovalIds;Assert.That(ids.Count,Is.EqualTo(4));for(var i=0;i<4;i++)Assert.That(ids[i],Is.EqualTo(Removals(cleanup)[i].TargetId));
                var receipt=rig.Driver.LatestBossTerminalCleanupReceipt.Value;Assert.That(receipt.CleanupTick,Is.EqualTo(tick));Assert.That(receipt.DeathSourceTick.Value,Is.EqualTo(cleanup-1));Assert.That(receipt.IsLifecycleSuperseded,Is.True);Assert.That(receipt.LifecycleReason,Is.EqualTo(TransferClearReason.RunFailed));Assert.That(receipt.DurableRemovalCommitted,Is.False);Assert.That(receipt.PlayerState,Is.EqualTo(TransferPlayerState.Baseline));Assert.That(receipt.ActiveTargetAfter,Is.Empty);
                if(activePayload){Assert.That(outcome.RemovalResult.Value.Cleared.Value.Reason,Is.EqualTo(TransferClearReason.RunFailed));Assert.That(outcome.RemovalResult.Value.Cleared.Value.PreviousTargetId,Is.EqualTo("BossWeight-0"));}else Assert.That(receipt.Clear.HasValue,Is.False);
            }
            finally{DestroyTransferRig(rig);}
        }
        [TestCase("owner")] [TestCase("pastTick")] [TestCase("futureTick")] [TestCase("noReservation")]
        [TestCase("wrongFour")] [TestCase("existingLifecycle")] [TestCase("duplicate")] [TestCase("consumed")]
        public void BossFailureMergeRejectsInvalidEnvelopeWithoutMutation(string invalidCase)
        {
            var rig=CreateTransferRig(null);var owner=new object();
            try
            {
                rig.Driver.RegisterInputGateOwner(owner);rig.Driver.AdvanceTransferPhaseForTests();var tick=new SimulationTick(1);
                var expectedLifecycle=invalidCase=="existingLifecycle"?(TransferClearReason?)TransferClearReason.RoomLeaving:null;
                var queued=new TransferSimulationInput(tick,null,null,null,Array.Empty<TransferTargetRemoved>(),expectedLifecycle);rig.Driver.Submit(queued);
                var exact=invalidCase!="noReservation"&&invalidCase!="wrongFour";
                if(exact)rig.Driver.MergeExactBossTerminalRemovals(Removals(1));
                else if(invalidCase=="wrongFour")rig.Driver.MergeRemovals(new[]{Removals(1)[0],Removals(1)[1],Removals(1)[2]}); // no terminal reservation: this is not an internal-four-ID corruption probe.
                rig.Player.AdvanceOneFixedTickForTests();
                if(invalidCase=="duplicate"){rig.Driver.MergeExactBossTerminalRunFailed(owner,tick);expectedLifecycle=TransferClearReason.RunFailed;}
                if(invalidCase=="consumed"){rig.Driver.AdvanceTransferPhaseForTests();rig.Player.AdvanceOneFixedTickForTests();}
                var current=rig.Player.NextExpectedTick;var before=rig.Driver.PrepareInputFrame(owner,current,false,null);
                var rejectedTick=invalidCase=="pastTick"?new SimulationTick(0):invalidCase=="futureTick"?new SimulationTick(2):tick;
                Assert.Throws<InvalidOperationException>(()=>rig.Driver.MergeExactBossTerminalRunFailed(invalidCase=="owner"?new object():owner,rejectedTick));
                rig.Driver.ValidateInputFrameCandidate(owner,before);
                rig.Driver.AdvanceTransferPhaseForTests();var actual=rig.Driver.LatestPhasePublication.Value.Input;
                Assert.That(rig.Driver.LatestPhasePublication.Value.Tick,Is.EqualTo(current));Assert.That(actual.LifecycleReason,Is.EqualTo(invalidCase=="consumed"?null:expectedLifecycle));Assert.That(actual.HadAim||actual.HadPress,Is.False);
                var count=invalidCase=="consumed"||invalidCase=="noReservation"?0:invalidCase=="wrongFour"?3:4;Assert.That(actual.RemovalCount,Is.EqualTo(count));var ids=rig.Driver.LatestCompletedInputRemovalIds;Assert.That(ids.Count,Is.EqualTo(count));for(var i=0;i<count;i++)Assert.That(ids[i],Is.EqualTo(Removals(1)[i].TargetId));
                Assert.That(rig.Driver.LatestBossTerminalCleanupReceipt.HasValue,Is.EqualTo(exact));
                if(exact){var receipt=rig.Driver.LatestBossTerminalCleanupReceipt.Value;Assert.That(receipt.LifecycleReason,Is.EqualTo(expectedLifecycle));Assert.That(receipt.DurableRemovalCommitted,Is.EqualTo(!expectedLifecycle.HasValue));}
            }
            finally{DestroyTransferRig(rig);}
        }

        // AC-M4B3C-002, AC-M4B3C-003. Inactive and lifecycle-first terminal
        // cleanup retain the distinction between no clear, durable removal,
        // and lifecycle supersession for all permitted reasons.
        [TestCase(TransferClearReason.RoomLeaving)]
        [TestCase(TransferClearReason.RunFailed)]
        [TestCase(TransferClearReason.Cutscene)]
        [TestCase(TransferClearReason.DemoCompleted)]
        public void InactiveTerminalCleanupSeparatesNormalAndLifecycle(TransferClearReason? lifecycle)
        {
            var rig = CreateTransferRig(null);
            try
            {
                rig.Driver.AdvanceTransferPhaseForTests();
                if (lifecycle.HasValue) rig.Driver.Submit(new TransferSimulationInput(new SimulationTick(1), null, null, null, Array.Empty<TransferTargetRemoved>(), lifecycle));
                rig.Driver.MergeExactBossTerminalRemovals(Removals(1));
                rig.Player.AdvanceOneFixedTickForTests();
                var outcome = rig.Driver.AdvanceTransferPhaseForTests();
                var receipt = rig.Driver.LatestBossTerminalCleanupReceipt.Value;
                Assert.That(outcome.RemovalResult.HasValue, Is.False);
                Assert.That(receipt.Clear.HasValue, Is.False);
                Assert.That(receipt.IsLifecycleSuperseded, Is.EqualTo(lifecycle.HasValue));
                Assert.That(receipt.DurableRemovalCommitted, Is.EqualTo(!lifecycle.HasValue));
                Assert.That(receipt.LifecycleReason, Is.EqualTo(lifecycle));
                Assert.That(receipt.Revision, Is.EqualTo(0));
                Assert.That(SessionHasNoRegistrations(rig.Driver, "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2"), Is.True);
                Assert.That(HasExactRemovedIds(rig.Driver), Is.EqualTo(!lifecycle.HasValue));
            }
            finally { DestroyTransferRig(rig); }
        }

        [Test]
        public void InactiveTerminalCleanupHasNoClearAndCommitsDurableRemoval() =>
            InactiveTerminalCleanupSeparatesNormalAndLifecycle(null);

        // AC-M4B3C-003. Lifecycle-first cleanup clears an active payload by
        // its own reason and revision, while never turning the requested IDs
        // into a durable removal claim.
        [TestCase(TransferClearReason.RoomLeaving)]
        [TestCase(TransferClearReason.RunFailed)]
        [TestCase(TransferClearReason.Cutscene)]
        [TestCase(TransferClearReason.DemoCompleted)]
        public void ActiveTerminalLifecycleUsesExactReasonWithoutDurableClaim(TransferClearReason reason)
        {
            var rig = CreateTransferRig("BossWeight-0");
            try
            {
                rig.Driver.AdvanceTransferPhaseForTests();
                rig.Driver.Submit(new TransferSimulationInput(new SimulationTick(2), null, null, null, Array.Empty<TransferTargetRemoved>(), reason));
                rig.Driver.MergeExactBossTerminalRemovals(Removals(2));
                rig.Player.AdvanceOneFixedTickForTests();
                rig.Driver.Submit(PressInput(1)); rig.Driver.AdvanceTransferPhaseForTests(); rig.Player.AdvanceOneFixedTickForTests();
                var outcome = rig.Driver.AdvanceTransferPhaseForTests();
                var receipt = rig.Driver.LatestBossTerminalCleanupReceipt.Value;
                Assert.That(outcome.RemovalResult.Value.Cleared.Value.Reason, Is.EqualTo(reason));
                Assert.That(outcome.RemovalResult.Value.Cleared.Value.PreviousTargetId, Is.EqualTo("BossWeight-0"));
                Assert.That(outcome.RemovalResult.Value.Cleared.Value.Revision, Is.EqualTo(2));
                Assert.That(receipt.IsLifecycleSuperseded && !receipt.DurableRemovalCommitted, Is.True);
                Assert.That(receipt.LifecycleReason, Is.EqualTo(reason));
                Assert.That(SessionHasNoRegistrations(rig.Driver, "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2"), Is.True);
                Assert.That(HasExactRemovedIds(rig.Driver), Is.False);
            }
            finally { DestroyTransferRig(rig); }
        }

        // REQ-UX-004/008, REQ-COM-004; AC-M5B2-006. Gate stripping is separate
        // from terminal discard: retained damage and future keys still fail closed.
        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void GatedTerminalPreservesExactEmptyDiscardBoundary(bool externalDamage, bool futureKey)
        {
            var rig = PrepareManualTerminal(true, externalDamage, futureKey);
            try
            {
                var delivery = rig.Combat.ReadPendingDeliveryForTests(rig.CleanupTick);
                var outcome = rig.Combat.LatestOutcome;
                var finalView = rig.Handoff.LatestPresentationView;
                var queue = (IDictionary)typeof(OrdanBossCombatSimulationDriver)
                    .GetField("_inputs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(rig.Combat);
                var pending = (OrdanBossCombatInput)queue[rig.CleanupTick.Value];
                Assert.That(pending.Camera.HasValue || pending.Aim.HasValue || pending.Press.HasValue, Is.False);
                Assert.That(pending.ExternalDamageRequests.Count, Is.EqualTo(externalDamage ? 1 : 0));
                Assert.That(rig.Transfer.LatestBossTerminalCleanupReceipt.Value.DurableRemovalCommitted, Is.True);

                if (externalDamage || futureKey)
                {
                    Assert.Throws<TargetInvocationException>(() => InvokeFixed(rig.Teardown));
                    Assert.That(rig.Teardown.LatestReceipt.HasValue, Is.False);
                    Assert.That(rig.Combat.enabled && rig.Bridge.enabled && rig.Scheduler.enabled, Is.True);
                    Assert.That(queue.Count, Is.EqualTo(futureKey ? 2 : 1));
                    Assert.That(queue[rig.CleanupTick.Value], Is.EqualTo(pending));
                    Assert.That(rig.Combat.ReadPendingDeliveryForTests(rig.CleanupTick), Is.EqualTo(delivery));
                }
                else
                {
                    InvokeFixed(rig.Teardown);
                    Assert.That(rig.Teardown.LatestReceipt.HasValue, Is.True);
                    Assert.That(rig.Combat.enabled, Is.False);
                    Assert.That(queue.Count, Is.Zero);
                }

                Assert.That(rig.Combat.LatestOutcome, Is.EqualTo(outcome));
                Assert.That(rig.Handoff.LatestPresentationView, Is.EqualTo(finalView));
            }
            finally { UnityEngine.Object.DestroyImmediate(rig.Root); }
        }

        // REQ-WT-005/006, REQ-UX-008; AC-M5B2-002. The active transfer is
        // cleared by the original maintenance cause, never by an input-gate event.
        [TestCase(null)]
        [TestCase(TransferClearReason.RoomLeaving)]
        [TestCase(TransferClearReason.RunFailed)]
        [TestCase(TransferClearReason.Cutscene)]
        [TestCase(TransferClearReason.DemoCompleted)]
        public void GatePreservesActiveTerminalMaintenanceAndLifecycle(TransferClearReason? lifecycle)
        {
            var rig = CreateTransferRig("BossWeight-0");
            try
            {
                rig.Driver.AdvanceTransferPhaseForTests();
                var queued = PressInput(2);
                rig.Driver.Submit(new TransferSimulationInput(queued.Tick, queued.Camera, queued.Aim,
                    queued.Press, queued.Removals, lifecycle));
                rig.Driver.MergeExactBossTerminalRemovals(Removals(2));
                rig.Player.AdvanceOneFixedTickForTests();
                rig.Driver.Submit(PressInput(1));
                Assert.That(rig.Driver.AdvanceTransferPhaseForTests().Snapshot.ActiveTargetId, Is.EqualTo("BossWeight-0"));
                rig.Player.AdvanceOneFixedTickForTests();
                var owner = new object();
                rig.Driver.RegisterInputGateOwner(owner);
                var captures = rig.Driver.SyncCaptureCountForTests;
                Assert.That(rig.Driver.ApplyInputGate(owner, new SimulationTick(2), true), Is.EqualTo(1));
                var result = rig.Driver.AdvanceTransferPhaseForTests();
                var receipt = rig.Driver.LatestBossTerminalCleanupReceipt.Value;
                Assert.That(result.PressResult.HasValue, Is.False);
                Assert.That(rig.Driver.SyncCaptureCountForTests, Is.EqualTo(captures));
                Assert.That(receipt.Clear.Value.Reason, Is.EqualTo(lifecycle ?? TransferClearReason.TargetRemoved));
                Assert.That(receipt.Clear.Value.Revision, Is.EqualTo(2));
                Assert.That(receipt.IsLifecycleSuperseded, Is.EqualTo(lifecycle.HasValue));
                Assert.That(receipt.DurableRemovalCommitted, Is.EqualTo(!lifecycle.HasValue));
                Assert.That(receipt.PlayerState, Is.EqualTo(TransferPlayerState.Baseline));
                Assert.That(HasExactRemovedIds(rig.Driver), Is.EqualTo(!lifecycle.HasValue));
                Assert.That(receipt.RequestedIds, Is.EqualTo(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
            }
            finally { DestroyTransferRig(rig); }
        }

        private static GameObject InstantiateGraph()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            Assert.That(asset, Is.Not.Null);
            return UnityEngine.Object.Instantiate(asset);
        }

        private static TransferSimulationInput PressInput(int tick)
        {
            var value = new SimulationTick(tick);
            var camera = new SimulationCameraPoseSnapshot(new SimulationTick(tick - 1), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
            var aim = new AimSample(1, AimSource.MousePointer, 1014, 540, true, 4096, 0, camera.CameraPoseTick, value);
            return new TransferSimulationInput(value, camera, aim, new TransferPressed(1, value), Array.Empty<TransferTargetRemoved>(), null);
        }

        private static OrdanBossCombatInput Lethal(SimulationTick tick, string id) => new OrdanBossCombatInput(tick, null, null, null,
            new[] { new DamageRequest(id, "test", "ordan", 60, DamageKind.BossPattern, tick) });

        private static PreparedManualTerminal PrepareManualTerminal(bool gateInputs = false, bool externalDamage = false, bool futureKey = false)
        {
            var fixture = typeof(OrdanBossEncounterAuthoredGraphPlayModeTests);
            var rig = fixture.GetMethod("CreateManualRig", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            var root = RigValue<GameObject>(rig, "Root"); var player = RigValue<PlayerMovementController>(rig, "Player");
            var transfer = RigValue<TransferSimulationDriver>(rig, "Transfer"); var combat = RigValue<OrdanBossCombatSimulationDriver>(rig, "Combat");
            var bridge = RigValue<OrdanBossSimulationDriver>(rig, "Bridge"); var scheduler = RigValue<OrdanBossTransferExposureScheduler>(rig, "Scheduler");
            var hostile = RigValue<OrdanBossHostileDamageProducer>(rig, "Hostile"); var pull = RigValue<OrdanBossBalanceAuditPullProducer>(rig, "AuditPull"); var handoff = RigValue<OrdanBossEncounterHandoffAdapter>(rig, "Handoff");
            fixture.GetMethod("ManualFixedStep", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new[] { rig });
            var death = player.NextExpectedTick;
            if (gateInputs)
            {
                var afterDeath = new SimulationTick(death.Value + 1);
                transfer.Submit(PressInput(afterDeath.Value));
                combat.Submit(GatedCombatPress(afterDeath, externalDamage));
                if (futureKey) combat.Submit(GatedCombatPress(new SimulationTick(afterDeath.Value + 1), false));
            }
            transfer.AdvanceTransferPhaseForTests(); combat.Submit(Lethal(death, "prepared-terminal")); combat.AdvanceCombatPhaseForTests(); bridge.AdvanceBridgePhaseForTests(); scheduler.AdvanceSchedulerPhaseForTests(); hostile.AdvanceForTests(); pull.AdvanceForTests(); player.AdvanceOneFixedTickForTests();
            InvokeHandoff(handoff);
            var cleanup = player.NextExpectedTick;
            if (gateInputs)
            {
                var owner = new object();
                transfer.RegisterInputGateOwner(owner);
                combat.RegisterInputGateOwner(owner);
                Assert.That(transfer.ApplyInputGate(owner, cleanup, true), Is.EqualTo(1));
                Assert.That(combat.ApplyInputGate(owner, cleanup, true), Is.EqualTo(futureKey ? 2 : 1));
            }
            transfer.AdvanceTransferPhaseForTests();
            var teardown = root.GetComponentInChildren<OrdanBossTerminalTeardown>();
            player.enabled = true; transfer.enabled = true; combat.enabled = true; bridge.enabled = true; scheduler.enabled = true; hostile.enabled = true; pull.enabled = true; handoff.enabled = true; teardown.enabled = true;
            foreach (var target in root.GetComponentsInChildren<CombatTarget>(true)) if (target.Freeze().TargetId == "ordan") { target.enabled = true; target.Freeze().TargetCollider.enabled = true; }
            return new PreparedManualTerminal(root, transfer, combat, bridge, scheduler, hostile, pull, handoff, teardown, death, cleanup);
        }

        private static T RigValue<T>(object rig, string name) => (T)rig.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(rig);
        private static OrdanBossCombatInput GatedCombatPress(SimulationTick tick, bool externalDamage)
        {
            var aimInput = PressInput(tick.Value);
            var requests = externalDamage
                ? new[] { new DamageRequest("gated-retained-damage", "test", "player", 1, DamageKind.BossPattern, tick) }
                : Array.Empty<DamageRequest>();
            return new OrdanBossCombatInput(tick, aimInput.Camera, aimInput.Aim,
                new BasicAttackPressed(1, 1, tick), requests);
        }
        private static void SetField(object instance, string name, object value) => instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        private static bool SessionHasNoRegistrations(TransferSimulationDriver driver, params string[] ids)
        {
            var session = driver.GetType().GetField("_session", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(driver);
            return (bool)session.GetType().GetMethod("HasNoRegistrations", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(session, new object[] { ids });
        }
        private static IDictionary Inputs(TransferSimulationDriver driver) => (IDictionary)driver.GetType().GetField("_inputs", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(driver);
        private static bool HasExactRemovedIds(TransferSimulationDriver driver)
        {
            var removed = (IEnumerable)driver.GetType().GetField("_removedTargetIds", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(driver);
            var values = new List<string>();
            foreach (var value in removed) values.Add(value as string);
            if (values.Count != 4) return false;
            foreach (var id in new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" })
            {
                var found = false;
                foreach (var value in values) if (string.Equals(value, id, StringComparison.Ordinal)) { found = true; break; }
                if (!found) return false;
            }
            return true;
        }
        private static void InvokeHandoff(OrdanBossEncounterHandoffAdapter handoff) => handoff.GetType().GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(handoff, null);
        private static OrdanBossEncounterPresentationView CopyView(OrdanBossEncounterPresentationView value, IReadOnlyList<OrdanBossHandoff> handoffs) =>
            new OrdanBossEncounterPresentationView(value.Tick, value.NextExpectedTick, value.Snapshot, value.Intents, handoffs, value.Combatants,
                value.ActiveTransferTargetId, value.TransferPlayerState, value.ForecastPayloadId, value.ExposedPayloadId, value.AuditForecastId,
                value.AuditExposedId, value.AuditTemporaryEndedTargetIds, value.AuditPermanentlyRemovedTargetIds, value.AuditAvailable,
                value.AuditApplied, value.PayloadAvailable, value.IsTerminal, value.StableIds, value.HostileDamageDigest, value.BalanceAuditPullDigest);

        private static TransferTargetRemoved[] Removals(int tick) => new[]
        {
            new TransferTargetRemoved("BossAuditBox-0", new SimulationTick(tick)), new TransferTargetRemoved("BossWeight-0", new SimulationTick(tick)),
            new TransferTargetRemoved("BossWeight-1", new SimulationTick(tick)), new TransferTargetRemoved("BossWeight-2", new SimulationTick(tick))
        };

        private static TransferRig CreateTransferRig(string exposedId)
        {
            var root = new GameObject("terminal-transfer-rig");
            var playerObject = new GameObject("player"); playerObject.transform.SetParent(root.transform); playerObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; playerObject.AddComponent<CapsuleCollider2D>();
            var player = playerObject.AddComponent<PlayerMovementController>(); var targets = new TransferTarget[4];
            for (var index = 0; index < targets.Length; index++)
            {
                var id = index == 0 ? "BossAuditBox-0" : "BossWeight-" + (index - 1);
                var value = new GameObject(id); value.transform.SetParent(root.transform); value.transform.position = new Vector2(1f, 0f); var collider = value.AddComponent<BoxCollider2D>();
                MonoBehaviour sink;
                TransferTargetKind kind; string profile;
                if (index == 0) { var audit = value.AddComponent<OrdanBossAuditTransferModifierSink>(); audit.ConfigureForTests(id); audit.CommitExposureForM4B3B(exposedId == id); sink = audit; kind = TransferTargetKind.Box; profile = OrdanBossAuditTransferModifierSink.RequiredBaseProfileId; }
                else { var payload = value.AddComponent<OrdanBossPayloadTransferModifierSink>(); payload.ConfigureForTests(id); payload.CommitExposure(exposedId == id); sink = payload; kind = TransferTargetKind.BossPayload; profile = OrdanBossPayloadTransferModifierSink.RequiredBaseProfileId; }
                targets[index] = value.AddComponent<TransferTarget>(); targets[index].ConfigureForTests(id, kind, 0, 0, 0, 0, 100, 100, profile, collider, value.transform, sink);
            }
            var registry = root.AddComponent<TransferTargetRegistry>(); registry.ConfigureForTests(targets);
            var driver = root.AddComponent<TransferSimulationDriver>(); driver.ConfigureForTests(player, registry); driver.InitializeForTests();
            return new TransferRig(root, player, driver);
        }

        private static void DestroyTransferRig(TransferRig rig) => UnityEngine.Object.DestroyImmediate(rig.Root);

        private static void InvokeFixed(OrdanBossTerminalTeardown teardown)
        {
            var method = typeof(OrdanBossTerminalTeardown).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);
            method.Invoke(teardown, null);
        }

        private readonly struct TransferRig
        {
            internal TransferRig(GameObject root, PlayerMovementController player, TransferSimulationDriver driver) { Root = root; Player = player; Driver = driver; }
            internal GameObject Root { get; } internal PlayerMovementController Player { get; } internal TransferSimulationDriver Driver { get; }
        }
        private readonly struct PreparedManualTerminal
        {
            internal PreparedManualTerminal(GameObject root, TransferSimulationDriver transfer, OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge, OrdanBossTransferExposureScheduler scheduler, OrdanBossHostileDamageProducer hostile, OrdanBossBalanceAuditPullProducer pull, OrdanBossEncounterHandoffAdapter handoff, OrdanBossTerminalTeardown teardown, SimulationTick deathTick, SimulationTick cleanupTick)
            { Root = root; Transfer = transfer; Combat = combat; Bridge = bridge; Scheduler = scheduler; Hostile = hostile; Pull = pull; Handoff = handoff; Teardown = teardown; DeathTick = deathTick; CleanupTick = cleanupTick; }
            internal GameObject Root { get; } internal TransferSimulationDriver Transfer { get; } internal OrdanBossCombatSimulationDriver Combat { get; } internal OrdanBossSimulationDriver Bridge { get; } internal OrdanBossTransferExposureScheduler Scheduler { get; } internal OrdanBossHostileDamageProducer Hostile { get; } internal OrdanBossBalanceAuditPullProducer Pull { get; } internal OrdanBossEncounterHandoffAdapter Handoff { get; } internal OrdanBossTerminalTeardown Teardown { get; } internal SimulationTick DeathTick { get; } internal SimulationTick CleanupTick { get; }
        }
    }
}
