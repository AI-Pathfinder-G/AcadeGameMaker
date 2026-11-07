// AC-COM-003, AC-WT-003, AC-WT-005, AC-WT-006, AC-UX-007. M2B2 fixed-phase integration evidence.
using System;
using System.Collections.Generic;
using System.Reflection;
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
    internal sealed class CombatSimulationDriverPlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private SimulationMode2D _previousSimulationMode;

        private enum ResetBusyKind { FailedPress, Lifecycle, Removal }

        [SetUp] public void SetUp() { _previousSimulationMode = Physics2D.simulationMode; Physics2D.simulationMode = SimulationMode2D.Script; }
        [TearDown] public void TearDown() { for (var i = _objects.Count - 1; i >= 0; i--) UnityEngine.Object.DestroyImmediate(_objects[i]); _objects.Clear(); Physics2D.simulationMode = _previousSimulationMode; Physics2D.SyncTransforms(); }

        // AC-COM-003, AC-WT-006. Reflection pins transfer=-200, combat=-190 and movement's default order; combat consumes the one transfer sync without adding another.
        [Test]
        public void ExecutionOrderAndSingleSharedAimSynchronizationAreExact()
        {
            Assert.That(Order<TransferSimulationDriver>(), Is.EqualTo(-200));
            Assert.That(Order<CombatSimulationDriver>(), Is.EqualTo(-190));
            Assert.That(Order<RegularEnemyReactionSimulationDriver>(), Is.EqualTo(-180));
            Assert.That(Order<RegularEnemyBehaviorSimulationDriver>(), Is.EqualTo(-170));
            Assert.That(Attribute.GetCustomAttribute(typeof(PlayerMovementController), typeof(DefaultExecutionOrder)), Is.Null);
            var rig = CreateRig(); CompleteEmptyTick(rig);
            SubmitSharedAim(rig, 1, 7, new BasicAttackPressed(0, 7, new SimulationTick(1)));
            rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.That(rig.Transfer.SyncCaptureCountForTests, Is.EqualTo(1));
            var outcome = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(rig.Transfer.SyncCaptureCountForTests, Is.EqualTo(1));
            Assert.That(outcome.Attack.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredMiss));
            Assert.That(Snapshot(outcome, "walker").CurrentHealth, Is.EqualTo(9));
        }

        // REQ-COM-004; AC-COM-003, AC-WT-005. The synchronous test seam proves
        // that -190 publishes combat without implicitly publishing the separate -180 phase.
        [Test]
        public void CombatOnlySeamLeavesReactionUnpublishedUntilExplicitMinus180Advance()
        {
            var rig = CreateRig();
            rig.Transfer.AdvanceTransferPhaseForTests();
            var combat = rig.Combat.AdvanceCombatOnlyForTests();
            Assert.That(combat.Tick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(rig.Reaction.CarriedViewForTests.HasValue, Is.False);

            var reaction = rig.Reaction.AdvanceReactionPhaseForTests();
            Assert.That(reaction.Tick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(rig.Reaction.CarriedViewForTests.HasValue, Is.True);
        }

        // REQ-COM-001, REQ-COM-004; AC-COM-001, AC-COM-003: -170 consumes only the
        // exact current M1/M3A publications, uses the bootstrap pose at t0, and adds no sync.
        [Test]
        public void BehaviorBridgePublishesExactPairAfterMinus180WithoutAdditionalSync()
        {
            var rig = CreateRig();
            AdvanceBehaviorFixed(rig);
            var carried = rig.Behavior.LatestCarriedViewForTests;
            Assert.That(carried.HasValue, Is.True);
            Assert.That(carried.Value.Tick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(carried.Value.Snapshot.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.DashTelegraph));
            Assert.That(carried.Value.Snapshot.Surveyor.Phase, Is.EqualTo(SurveyorBehaviorPhase.Relocate));
            Assert.That(rig.Transfer.SyncCaptureCountForTests, Is.EqualTo(0));
            rig.Player.AdvanceOneFixedTickForTests();
            AdvanceBehaviorFixed(rig);
            Assert.That(rig.Behavior.LatestCarriedViewForTests.Value.Tick, Is.EqualTo(new SimulationTick(1)));
        }

        // REQ-COM-004; AC-COM-003: missing initial M1 publication cannot be confused with a default tick-0 outcome.
        [Test]
        public void BehaviorBridgeRejectsMissingCombatPublicationBeforePairMutation()
        {
            var rig = CreateRig();
            Assert.Throws<InvalidOperationException>(() => rig.Behavior.AdvanceBehaviorPhaseForTests());
            Assert.That(rig.Behavior.LatestCarriedViewForTests.HasValue, Is.False);
            Assert.That(rig.Behavior.HasSessionForTests, Is.False);
            Assert.That(rig.Behavior.NextExpectedTickForTests, Is.EqualTo(new SimulationTick(0)));
            Assert.That(rig.Reaction.QueuedRecoveryCountForTests, Is.EqualTo(0));
        }

        // REQ-COM-004; AC-COM-003: a -190 result without its required -180 M3A publication cannot advance the behavior pair.
        [Test]
        public void BehaviorBridgeRejectsMissingReactionPublicationBeforePairMutation()
        {
            var rig = CreateRig();
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.AdvanceCombatOnlyForTests();
            Assert.Throws<InvalidOperationException>(() => rig.Behavior.AdvanceBehaviorPhaseForTests());
            Assert.That(rig.Behavior.LatestCarriedViewForTests.HasValue, Is.False);
            Assert.That(rig.Behavior.NextExpectedTickForTests, Is.EqualTo(new SimulationTick(0)));
        }

        // REQ-COM-004, REQ-COM-005; AC-COM-003: the M3B2B-only preflight rejects a non-t+1 conversion without queue mutation.
        [Test]
        public void BehaviorRecoveryPreflightRejectsNonNextTickWithoutQueueMutation()
        {
            var rig = CreateRig();
            Assert.Throws<InvalidOperationException>(() => rig.Reaction.PreflightRecoveryForBehaviorBridge(new AttackRecoveryStarted(9, new SimulationTick(2))));
            Assert.That(rig.Reaction.QueuedRecoveryCountForTests, Is.EqualTo(0));
        }

        // REQ-COM-001, REQ-COM-005; AC-COM-001, AC-COM-003: the final dash publication
        // preflights and queues one t+1 recovery before default movement advances the clock.
        [Test]
        public void BehaviorBridgeQueuesFinalDashRecoveryBeforeMovementAndPublishesOnce()
        {
            var rig = CreateRig();
            rig.WalkerCombat.transform.position = new Vector2(1f, 0f);
            for (var tick = 0; tick <= 29; tick++)
            {
                AdvanceBehaviorFixed(rig);
                if (tick < 29) rig.Player.AdvanceOneFixedTickForTests();
            }
            var finalDash = rig.Behavior.LatestCarriedViewForTests.Value.Snapshot;
            Assert.That(finalDash.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.DashActive));
            Assert.That(finalDash.WalkerRecoveryIntent.Value.ScheduledTick, Is.EqualTo(new SimulationTick(30)));
            Assert.That(rig.Reaction.QueuedRecoveryCountForTests, Is.EqualTo(1));
            Assert.That(rig.Player.NextExpectedTick, Is.EqualTo(new SimulationTick(29)));
            rig.Player.AdvanceOneFixedTickForTests();
            AdvanceBehaviorFixed(rig);
            Assert.That(rig.Reaction.QueuedRecoveryCountForTests, Is.EqualTo(0));
            Assert.That(rig.Behavior.LatestCarriedViewForTests.Value.Snapshot.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.Recovery));
        }

        // REQ-COM-004, REQ-COM-005; AC-COM-003: a completed behavior pair survives stale M1 and future M3A publication rejection without core, queue or carried-view mutation.
        [Test]
        public void BehaviorBridgePreservesPriorPublicationOnPostSuccessStaleAndFutureUpstream()
        {
            var rig = CreateRig();
            AdvanceBehaviorFixed(rig); // t0 success
            rig.Player.AdvanceOneFixedTickForTests();
            Assert.Throws<InvalidOperationException>(() => rig.Behavior.AdvanceBehaviorPhaseForTests()); // stale M1=t0 at expected t1
            Assert.That(rig.Behavior.NextExpectedTickForTests, Is.EqualTo(new SimulationTick(1)));
            Assert.That(rig.Behavior.LatestCarriedViewForTests.Value.Tick, Is.EqualTo(new SimulationTick(0)));
            Assert.That(rig.Reaction.QueuedRecoveryCountForTests, Is.EqualTo(0));

            AdvanceBehaviorFixed(rig); // t1 success
            rig.Player.AdvanceOneFixedTickForTests();
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.AdvanceCombatOnlyForTests(); // exact M1=t2, intentionally no -180
            var prior = rig.Reaction.LatestCarriedViewForBehaviorBridge.Value;
            var futureTick = new SimulationTick(3);
            var future = new CarriedReactionView(futureTick, ReTickReaction(prior.Walker, "walker", futureTick), ReTickReaction(prior.Surveyor, "surveyor", futureTick));
            typeof(RegularEnemyReactionSimulationDriver).GetField("_carried", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(rig.Reaction, (CarriedReactionView?)future);
            Assert.Throws<InvalidOperationException>(() => rig.Behavior.AdvanceBehaviorPhaseForTests());
            Assert.That(rig.Behavior.NextExpectedTickForTests, Is.EqualTo(new SimulationTick(2)));
            Assert.That(rig.Behavior.LatestCarriedViewForTests.Value.Tick, Is.EqualTo(new SimulationTick(1)));
            Assert.That(rig.Reaction.QueuedRecoveryCountForTests, Is.EqualTo(0));
        }

        // REQ-COM-004; AC-COM-003: exact reset flows through all four fixed phases and clears behavior state without an intent before ordinal-zero gameplay resumes.
        [Test]
        public void BehaviorBridgeCarriesExactResetThenRestartsOrdinalZero()
        {
            var rig = CreateRig();
            AdvanceBehaviorFixed(rig);
            rig.Player.AdvanceOneFixedTickForTests();
            var resetTick = rig.Player.NextExpectedTick;
            rig.Combat.SubmitForTests(new CombatSimulationInput(resetTick, null, null, null, Array.Empty<DamageRequest>(), true));
            AdvanceBehaviorFixed(rig);
            var reset = rig.Behavior.LatestCarriedViewForTests.Value.Snapshot;
            Assert.That(reset.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.Reset));
            Assert.That(reset.Surveyor.Phase, Is.EqualTo(SurveyorBehaviorPhase.Reset));
            Assert.That(reset.WalkerRecoveryIntent.HasValue || reset.SurveyorShotIntent.HasValue, Is.False);
            Assert.That(reset.NextExpectedTick, Is.EqualTo(new SimulationTick(2)));
            rig.Player.AdvanceOneFixedTickForTests();
            AdvanceBehaviorFixed(rig);
            var restarted = rig.Behavior.LatestCarriedViewForTests.Value.Snapshot;
            Assert.That(restarted.Walker.AttackCycleOrdinal, Is.EqualTo(0));
            Assert.That(restarted.Surveyor.ShotOrdinal, Is.EqualTo(0));
        }

        // REQ-WT-003, REQ-COM-001, REQ-COM-004; AC-WT-005, AC-COM-003: an active shared aim is the only sync, while layer-8 self roots are excluded and a third-party layer-8 blocker closes LOS.
        [Test]
        public void BehaviorBridgeUsesSingleSharedAimSyncAndAuthoredLosFiltering()
        {
            var open = CreateRig();
            open.WalkerCombat.transform.position = new Vector2(10f, 0f);
            AdvanceBehaviorFixed(open);
            open.Player.AdvanceOneFixedTickForTests();
            open.WalkerCombat.transform.position = new Vector2(1f, 0f);
            open.WalkerCombat.gameObject.layer = TransferLineOfSight.Layer;
            open.Player.gameObject.layer = TransferLineOfSight.Layer;
            SubmitSharedAim(open, 1, 1, new BasicAttackPressed(1, 1, new SimulationTick(1)));
            AdvanceBehaviorFixed(open);
            Assert.That(open.Transfer.SyncCaptureCountForTests, Is.EqualTo(1));
            Assert.That(open.Behavior.LatestCarriedViewForTests.Value.Snapshot.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.DashTelegraph));

            var blocked = CreateRig();
            blocked.WalkerCombat.transform.position = new Vector2(10f, 0f);
            AdvanceBehaviorFixed(blocked);
            blocked.Player.AdvanceOneFixedTickForTests();
            blocked.WalkerCombat.transform.position = new Vector2(1f, 0f);
            var blocker = Track(new GameObject("BehaviorLosBlocker")); blocker.layer = TransferLineOfSight.Layer; blocker.transform.position = new Vector2(0.5f, 0f); blocker.AddComponent<BoxCollider2D>();
            SubmitSharedAim(blocked, 1, 1, new BasicAttackPressed(1, 1, new SimulationTick(1)));
            AdvanceBehaviorFixed(blocked);
            Assert.That(blocked.Transfer.SyncCaptureCountForTests, Is.EqualTo(1));
            Assert.That(blocked.Behavior.LatestCarriedViewForTests.Value.Snapshot.Walker.Phase, Is.EqualTo(WalkerBehaviorPhase.Approach));
        }

        // REQ-COM-001, REQ-COM-005; AC-COM-001, AC-COM-003. The first combat tick
        // is closed, then a t=1 recovery publication becomes attackable at t=2 only.
        [Test]
        public void WalkerUsesOnlyTheCarriedTMinusOneReactionView()
        {
            var closedRig = CreateRig();
            closedRig.Reaction.SubmitRecoveryForTests(new AttackRecoveryStarted(7, new SimulationTick(1)));
            AdvanceEmptyFixed(closedRig);
            SubmitSharedAim(closedRig, 1, 1, new BasicAttackPressed(1, 1, new SimulationTick(1)));
            closedRig.Transfer.AdvanceTransferPhaseForTests();
            var closed = closedRig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(closed.Attack.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredMiss));
            var openRig = CreateRig();
            openRig.Reaction.SubmitRecoveryForTests(new AttackRecoveryStarted(8, new SimulationTick(1)));
            AdvanceEmptyFixed(openRig); AdvanceEmptyFixed(openRig);
            SubmitSharedAim(openRig, 2, 2, new BasicAttackPressed(2, 2, new SimulationTick(2)));
            openRig.Transfer.AdvanceTransferPhaseForTests();
            var open = openRig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(open.Attack.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredHit));
        }

        // REQ-COM-004; AC-COM-003. A recovery cycle is encounter-unique even when
        // callers try to queue it for two distinct future reaction ticks.
        [Test]
        public void RecoveryCandidatesAreUniqueByCycleIdAcrossFutureTicks()
        {
            var rig = CreateRig();
            rig.Reaction.SubmitRecoveryForTests(new AttackRecoveryStarted(55, new SimulationTick(1)));
            Assert.Throws<InvalidOperationException>(() => rig.Reaction.SubmitRecoveryForTests(new AttackRecoveryStarted(55, new SimulationTick(2))));
        }

        // REQ-COM-001, REQ-COM-005; AC-COM-001, AC-COM-003. A recovery opened at
        // reaction tick 1 is visible to exactly combat ticks 2..31, then closes at 32.
        [Test]
        public void WalkerRecoveryWindowProvidesExactlyThirtyCarriedCombatPhases()
        {
            var rig = CreateRig();
            rig.Reaction.SubmitRecoveryForTests(new AttackRecoveryStarted(30, new SimulationTick(1)));
            AdvanceEmptyFixed(rig); // reaction 0
            AdvanceEmptyFixed(rig); // reaction 1 opens [1, 31)
            for (var tick = 2; tick <= 31; tick++)
            {
                Assert.That(rig.Reaction.CarriedViewForTests.Value.Walker.ReactionAllowsBasicAttack, Is.True, "combat tick " + tick);
                AdvanceEmptyFixed(rig);
            }
            Assert.That(rig.Reaction.CarriedViewForTests.Value.Walker.ReactionAllowsBasicAttack, Is.False, "combat tick 32");
        }

        // REQ-COM-005; AC-COM-001, AC-COM-003. Only canonical Applied HeavyImpact
        // opens the 90-tick carried walker window.
        [Test]
        public void WalkerHeavyImpactWindowProvidesExactlyNinetyCarriedCombatPhases()
        {
            var rig = CreateRig();
            AdvanceEmptyFixed(rig);
            var tick = new SimulationTick(1);
            var impact = new DamageRequest("impact", "hazard", "walker", 1, DamageKind.HeavyImpact, tick);
            rig.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null, new[] { impact }, false));
            AdvanceEmptyFixed(rig); // reaction 1 opens [1, 91)
            for (var current = 2; current <= 91; current++)
            {
                Assert.That(rig.Reaction.CarriedViewForTests.Value.Walker.ReactionAllowsBasicAttack, Is.True, "combat tick " + current);
                AdvanceEmptyFixed(rig);
            }
            Assert.That(rig.Reaction.CarriedViewForTests.Value.Walker.ReactionAllowsBasicAttack, Is.False, "combat tick 92");
        }

        // REQ-COM-002, REQ-COM-005; AC-COM-001, AC-WT-002. Surveyor directives
        // are independent of basic-attack eligibility and its residual lock is exact.
        [Test]
        public void SurveyorFireLockDoesNotGateBasicAttackAndRecallPreservesExactLock()
        {
            var rig = CreateRig();
            rig.SurveyorCombat.transform.position = new Vector2(1f, 0f);
            AdvanceEmptyFixed(rig);
            var applyTick = new SimulationTick(1);
            rig.Transfer.SubmitForTests(TransferInput(1, 1, new TransferPressed(1, applyTick)));
            AdvanceEmptyFixed(rig);
            var heavy = rig.Reaction.CarriedViewForTests.Value.Surveyor;
            Assert.That(heavy.IsHeavy, Is.True); Assert.That(heavy.ForcedDescent, Is.True); Assert.That(heavy.CanFire, Is.False); Assert.That(heavy.FireLockEndTick, Is.EqualTo(76));
            SubmitSharedAim(rig, rig.Player.NextExpectedTick.Value, 2, new BasicAttackPressed(2, 2, rig.Player.NextExpectedTick));
            var attack = AdvanceEmptyFixed(rig);
            Assert.That(attack.Attack.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredHit));
            Assert.That(attack.Attack.Selection.TargetId, Is.EqualTo("surveyor"));

            // VD-02's original apply cooldown remains authoritative. Recall after it
            // expires so this test verifies the residual M3A lock, not a failed press.
            AdvanceThrough(rig, 22);
            var recallTick = rig.Player.NextExpectedTick;
            rig.Transfer.SubmitForTests(new TransferSimulationInput(recallTick, null, null, new TransferPressed(null, recallTick), Array.Empty<TransferTargetRemoved>(), null));
            AdvanceEmptyFixed(rig);
            var recalled = rig.Reaction.CarriedViewForTests.Value.Surveyor;
            Assert.That(recalled.ForcedDescent, Is.False); Assert.That(recalled.CanFire, Is.False); Assert.That(recalled.FireLockEndTick, Is.EqualTo(76));
            while (rig.Player.NextExpectedTick.Value <= 76) AdvanceEmptyFixed(rig);
            Assert.That(rig.Reaction.CarriedViewForTests.Value.Surveyor.CanFire, Is.True);
        }

        // REQ-COM-004, REQ-COM-005; AC-COM-003. Full-tuple canonical ordering is
        // retained beside results, so duplicate request IDs are never ID-looked-up.
        [Test]
        public void CanonicalDuplicateIdTraceArbitratesAppliedHeavyImpactByExactIndex()
        {
            var rig = CreateRig();
            rig.Reaction.SubmitRecoveryForTests(new AttackRecoveryStarted(77, new SimulationTick(1)));
            AdvanceEmptyFixed(rig);
            var tick = new SimulationTick(1);
            var applied = new DamageRequest("same", "a", "walker", 1, DamageKind.HeavyImpact, tick);
            var duplicate = new DamageRequest("same", "b", "surveyor", 1, DamageKind.HeavyImpact, tick);
            rig.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null, new[] { duplicate, applied }, false));
            var outcome = AdvanceEmptyFixed(rig);
            Assert.That(outcome.CanonicalRequestTrace[0].SourceId, Is.EqualTo("a"));
            Assert.That(outcome.CanonicalRequestTrace[1].SourceId, Is.EqualTo("b"));
            Assert.That(outcome.DamageResults[0].Result, Is.EqualTo(DamageResultCode.Applied));
            Assert.That(outcome.DamageResults[1].Result, Is.EqualTo(DamageResultCode.Duplicate));
            Assert.That(rig.Reaction.CarriedViewForTests.Value.Walker.ShieldWindowEndTick, Is.EqualTo(91));
            Assert.Throws<InvalidOperationException>(() => rig.Reaction.SubmitRecoveryForTests(new AttackRecoveryStarted(77, new SimulationTick(3))));

            var ignored = CreateRig(); AdvanceEmptyFixed(ignored);
            var ignoredTick = new SimulationTick(1);
            var basic = new DamageRequest("ignored", "a", "walker", 1, DamageKind.BasicAttack, ignoredTick);
            var heavyDuplicate = new DamageRequest("ignored", "b", "walker", 1, DamageKind.HeavyImpact, ignoredTick);
            ignored.Combat.SubmitForTests(new CombatSimulationInput(ignoredTick, null, null, null, new[] { heavyDuplicate, basic }, false));
            AdvanceEmptyFixed(ignored);
            Assert.That(ignored.Reaction.CarriedViewForTests.Value.Walker.ReactionAllowsBasicAttack, Is.False);
        }

        // REQ-COM-004, REQ-WT-005; AC-COM-003, AC-WT-005. Reset is a current-tick
        // three-phase transaction: after a completed baseline tick, only idle transfer
        // intent plus the exact current combat reset marker may reset both sessions.
        [Test]
        public void EncounterResetRequiresExactIdleCurrentTickAndPublishesEmptyCarriedView()
        {
            var rig = CreateRig();
            AdvanceEmptyFixed(rig);
            var resetTick = rig.Player.NextExpectedTick;
            rig.Combat.SubmitForTests(new CombatSimulationInput(resetTick, null, null, null, Array.Empty<DamageRequest>(), true));
            AdvanceEmptyFixed(rig);
            var carried = rig.Reaction.CarriedViewForTests.Value;
            Assert.That(carried.Tick, Is.EqualTo(resetTick));
            Assert.That(carried.Walker.LastTransferRevision, Is.EqualTo(-1));
            Assert.That(carried.Surveyor.LastTransferRevision, Is.EqualTo(-1));
            Assert.That(carried.Walker.ReactionAllowsBasicAttack, Is.False);

            var busy = CreateRig(); AdvanceEmptyFixed(busy);
            var busyTick = busy.Player.NextExpectedTick;
            busy.Transfer.SubmitForTests(TransferInput(busyTick.Value, 1, null)); // aim-only is non-idle
            busy.Combat.SubmitForTests(new CombatSimulationInput(busyTick, null, null, null, Array.Empty<DamageRequest>(), true));
            busy.Transfer.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => busy.Combat.AdvanceCombatPhaseForTests());
        }

        // REQ-WT-005, REQ-COM-004; AC-WT-005, AC-COM-003. One transfer tick may
        // clear walker then apply surveyor; both literal M3A sessions mirror once.
        [Test]
        public void SameTickWalkerRemovalThenSurveyorApplyProjectsIncreasingRevisions()
        {
            var rig = CreateRig();
            AdvanceEmptyFixed(rig);
            var tick1 = new SimulationTick(1);
            rig.Transfer.SubmitForTests(TransferInput(1, 1, new TransferPressed(1, tick1)));
            AdvanceEmptyFixed(rig);
            // A TargetRemoved clear preserves VD-02's original apply lock. Advance
            // to its approved expiry, then prove the same transfer tick may clear
            // walker and apply surveyor with r2 then r3.
            AdvanceThrough(rig, 22);
            rig.SurveyorCombat.transform.position = new Vector2(1f, 0f);
            var tick2 = rig.Player.NextExpectedTick;
            var removal = new TransferTargetRemoved("walker", tick2);
            rig.Transfer.SubmitForTests(new TransferSimulationInput(tick2, Camera(tick2.Value - 1), Aim(tick2.Value, 2), new TransferPressed(2, tick2), new[] { removal }, null));
            var transfer = rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.That(transfer.RemovalResult.Value.Cleared.Value.Revision, Is.EqualTo(2));
            Assert.That(transfer.PressResult.Value.StateChanged.Value.TargetId, Is.EqualTo("surveyor"));
            Assert.That(transfer.PressResult.Value.StateChanged.Value.Revision, Is.EqualTo(3));
            rig.Combat.AdvanceCombatPhaseForTests(); rig.Player.AdvanceOneFixedTickForTests();
            var carried = rig.Reaction.CarriedViewForTests.Value;
            Assert.That(carried.Walker.LastTransferRevision, Is.EqualTo(2)); Assert.That(carried.Walker.IsHeavy, Is.False);
            Assert.That(carried.Surveyor.LastTransferRevision, Is.EqualTo(3)); Assert.That(carried.Surveyor.IsHeavy, Is.True);
        }

        // REQ-COM-004, REQ-WT-005; AC-COM-003. All non-idle transfer summaries
        // reject the current combat reset before M2A/M1 state can advance.
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void ResetRejectsEveryNonIdleTransferIntentBeforeCombatMutation(int kindValue)
        {
            var kind = (ResetBusyKind)kindValue;
            var rig = CreateRig(); AdvanceEmptyFixed(rig);
            var tick = rig.Player.NextExpectedTick;
            switch (kind)
            {
                case ResetBusyKind.FailedPress:
                    rig.Transfer.SubmitForTests(new TransferSimulationInput(tick, null, null, new TransferPressed(null, tick), Array.Empty<TransferTargetRemoved>(), null)); break;
                case ResetBusyKind.Lifecycle:
                    rig.Transfer.SubmitForTests(new TransferSimulationInput(tick, null, null, null, Array.Empty<TransferTargetRemoved>(), TransferClearReason.RoomLeaving)); break;
                default:
                    rig.Transfer.SubmitForTests(new TransferSimulationInput(tick, null, null, null, new[] { new TransferTargetRemoved("walker", tick) }, null)); break;
            }
            rig.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null, Array.Empty<DamageRequest>(), true));
            rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
            Assert.That(rig.Combat.NextCombatTickForTests, Is.EqualTo(tick));
        }

        // REQ-COM-004; AC-COM-003. Binding/roster validation is explicit and fails
        // before the first M1 processing call, never by dynamic scene discovery.
        [Test]
        public void MissingReactionBindingAndMalformedReactionRosterFailBeforeM1()
        {
            var valid = CreateRig();
            var unbound = Track(new GameObject("UnboundCombat")).AddComponent<CombatSimulationDriver>();
            var player = (PlayerMovementController)typeof(CombatSimulationDriver).GetField("_player", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(valid.Combat);
            var registry = (CombatTargetRegistry)typeof(CombatSimulationDriver).GetField("_registry", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(valid.Combat);
            unbound.ConfigureForTests(player, registry, valid.Transfer, null);
            Assert.Throws<InvalidOperationException>(() => unbound.InitializeForTests());

            var missing = CreateRig(includeSurveyor: false); missing.Transfer.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => missing.Combat.AdvanceCombatPhaseForTests());
            Assert.That(missing.Combat.NextCombatTickForTests.Value, Is.EqualTo(0));
            var malformed = CreateRig(includeSurveyor: true, surveyorHealth: 5); malformed.Transfer.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => malformed.Combat.AdvanceCombatPhaseForTests());
            Assert.That(malformed.Combat.NextCombatTickForTests.Value, Is.EqualTo(0));
        }

        // REQ-COM-004, REQ-WT-005; AC-COM-003. A current recovery candidate and
        // mirrored raw Heavy both block reset; a retained old combat outcome cannot
        // be replayed as a later reaction reset.
        [Test]
        public void ResetRejectsCurrentRecoveryAndRawHeavyAndStaleCombatOutcome()
        {
            var recovery = CreateRig(); recovery.Reaction.SubmitRecoveryForTests(new AttackRecoveryStarted(88, new SimulationTick(1)));
            AdvanceEmptyFixed(recovery);
            var recoveryTick = recovery.Player.NextExpectedTick;
            recovery.Combat.SubmitForTests(new CombatSimulationInput(recoveryTick, null, null, null, Array.Empty<DamageRequest>(), true));
            recovery.Transfer.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => recovery.Combat.AdvanceCombatPhaseForTests());

            var heavy = CreateRig(); AdvanceEmptyFixed(heavy);
            var applyTick = heavy.Player.NextExpectedTick;
            heavy.Transfer.SubmitForTests(TransferInput(applyTick.Value, 1, new TransferPressed(1, applyTick)));
            AdvanceEmptyFixed(heavy);
            var heavyTick = heavy.Player.NextExpectedTick;
            heavy.Combat.SubmitForTests(new CombatSimulationInput(heavyTick, null, null, null, Array.Empty<DamageRequest>(), true));
            heavy.Transfer.AdvanceTransferPhaseForTests();
            Assert.Throws<InvalidOperationException>(() => heavy.Combat.AdvanceCombatPhaseForTests());

            var stale = CreateRig(); AdvanceEmptyFixed(stale);
            Assert.Throws<InvalidOperationException>(() => stale.Reaction.AdvanceReactionPhaseForTests());
        }

        // REQ-COM-004, REQ-COM-005; AC-COM-003. M1 may publish at a reachable
        // high tick, but an exact live 90/75 reaction horizon overflow then preserves
        // both reaction sessions/carried state and fails only the -180 phase.
        [Test]
        public void PostM1LiveHeavyImpactAndSurveyorHeavyOverflowArePhaseLocal()
        {
            AssertReactionOverflowPreservesUpstream(int.MaxValue - 89, false);
            AssertReactionOverflowPreservesUpstream(int.MaxValue - 74, true);
        }

        // REQ-COM-004; AC-COM-003. At the highest tick admitted by combat's common
        // 45-tick horizon, a duplicate HeavyImpact is ignored and triggers no 90-tick
        // reaction arithmetic.
        [Test]
        public void IgnoredHeavyImpactAtReachableHighTickDoesNotPerformWindowArithmetic()
        {
            var rig = CreateRig(int.MaxValue - 45);
            var tick = rig.Player.NextExpectedTick;
            var basic = new DamageRequest("high", "a", "walker", 1, DamageKind.BasicAttack, tick);
            var duplicateHeavy = new DamageRequest("high", "b", "walker", 1, DamageKind.HeavyImpact, tick);
            rig.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null, new[] { duplicateHeavy, basic }, false));
            rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.DoesNotThrow(() => rig.Combat.AdvanceCombatPhaseForTests());
            Assert.That(rig.Combat.LatestOutcomeForTests.DamageResults[1].Result, Is.EqualTo(DamageResultCode.Duplicate));
            Assert.That(rig.Reaction.CarriedViewForTests.Value.Walker.ShieldWindowEndTick, Is.EqualTo(-1));
        }

        // REQ-COM-004, REQ-COM-005; AC-COM-003. Suppressed/dead paths at admitted
        // high ticks advance M3A without proposing 90/75 horizons.
        [Test]
        public void HighTickInvulnerableTargetDeadAndLethalHeavyPathsSkipReactionWindows()
        {
            var invulnerable = CreateRig(int.MaxValue - 91);
            var first = invulnerable.Player.NextExpectedTick;
            invulnerable.Combat.SubmitForTests(new CombatSimulationInput(first, null, null, null, new[] { new DamageRequest("hurt", "hazard", "player", 1, DamageKind.EnemyContact, first) }, false));
            AdvanceEmptyFixed(invulnerable);
            var second = invulnerable.Player.NextExpectedTick;
            invulnerable.Combat.SubmitForTests(new CombatSimulationInput(second, null, null, null, new[] { new DamageRequest("invuln", "hazard", "player", 1, DamageKind.EnemyContact, second) }, false));
            Assert.DoesNotThrow(() => AdvanceEmptyFixed(invulnerable));
            Assert.That(invulnerable.Combat.LatestOutcomeForTests.DamageResults[0].Result, Is.EqualTo(DamageResultCode.Invulnerable));

            var targetDead = CreateRig(int.MaxValue - 46);
            var killTick = targetDead.Player.NextExpectedTick;
            targetDead.Combat.SubmitForTests(new CombatSimulationInput(killTick, null, null, null, new[] { new DamageRequest("kill", "hazard", "walker", 9, DamageKind.HeavyImpact, killTick) }, false));
            AdvanceEmptyFixed(targetDead);
            var deadTick = targetDead.Player.NextExpectedTick;
            targetDead.Combat.SubmitForTests(new CombatSimulationInput(deadTick, null, null, null, new[] { new DamageRequest("dead", "hazard", "walker", 1, DamageKind.HeavyImpact, deadTick) }, false));
            Assert.DoesNotThrow(() => AdvanceEmptyFixed(targetDead));
            Assert.That(targetDead.Combat.LatestOutcomeForTests.DamageResults[0].Result, Is.EqualTo(DamageResultCode.TargetDead));
            Assert.That(targetDead.Reaction.CarriedViewForTests.Value.Walker.ShieldWindowEndTick, Is.EqualTo(-1));

            var lethalWalker = CreateRig(int.MaxValue - 45);
            var walkerTick = lethalWalker.Player.NextExpectedTick;
            lethalWalker.Combat.SubmitForTests(new CombatSimulationInput(walkerTick, null, null, null, new[] { new DamageRequest("lethal-walker", "hazard", "walker", 9, DamageKind.HeavyImpact, walkerTick) }, false));
            Assert.DoesNotThrow(() => AdvanceEmptyFixed(lethalWalker));
            var walker = lethalWalker.Reaction.CarriedViewForTests.Value.Walker;
            Assert.That(walker.IsAlive, Is.False); Assert.That(walker.ReactionAllowsBasicAttack, Is.False); Assert.That(walker.ShieldWindowEndTick, Is.EqualTo(-1));

            var lethalSurveyor = CreateRig(int.MaxValue - 45);
            lethalSurveyor.SurveyorCombat.transform.position = new Vector2(1f, 0f);
            var surveyorTick = lethalSurveyor.Player.NextExpectedTick;
            lethalSurveyor.Transfer.SubmitForTests(TransferInput(surveyorTick.Value, 1, new TransferPressed(1, surveyorTick)));
            lethalSurveyor.Combat.SubmitForTests(new CombatSimulationInput(surveyorTick, null, null, null, new[] { new DamageRequest("lethal-surveyor", "hazard", "surveyor", 6, DamageKind.HeavyImpact, surveyorTick) }, false));
            Assert.DoesNotThrow(() => AdvanceEmptyFixed(lethalSurveyor));
            var surveyor = lethalSurveyor.Reaction.CarriedViewForTests.Value.Surveyor;
            Assert.That(surveyor.IsAlive, Is.False); Assert.That(surveyor.IsHeavy, Is.True); Assert.That(surveyor.ForcedDescent, Is.False); Assert.That(surveyor.CanFire, Is.False); Assert.That(surveyor.FireLockEndTick, Is.EqualTo(-1));
        }

        // AC-WT-003, AC-WT-006. Missing or mismatched receipts fail before either deterministic combat clock publishes.
        [Test]
        public void MissingAndMismatchedCaptureReceiptsAreFailStopAndConsumed()
        {
            var missing = CreateRig(); CompleteEmptyTick(missing);
            var missingCombatBefore = CombatTrace(missing.Combat.LatestOutcomeForTests);
            var missingAttackBefore = AttackStateTrace(missing.Combat); var missingHealthBefore = HealthStateTrace(missing.Combat, "player", "walker");
            missing.Transfer.AdvanceTransferPhaseForTests();
            var missingTransferBefore = TransferSnapshotTrace(missing.Transfer.SnapshotForTests);
            missing.Combat.SubmitForTests(CombatInput(1, 1, new BasicAttackPressed(0, 1, new SimulationTick(1))));
            Assert.Throws<InvalidOperationException>(() => missing.Combat.AdvanceCombatPhaseForTests());
            Assert.That(missing.Combat.NextCombatTickForTests.Value, Is.EqualTo(1));
            Assert.That(CombatTrace(missing.Combat.LatestOutcomeForTests), Is.EqualTo(missingCombatBefore));
            Assert.That(AttackStateTrace(missing.Combat), Is.EqualTo(missingAttackBefore)); Assert.That(HealthStateTrace(missing.Combat, "player", "walker"), Is.EqualTo(missingHealthBefore));
            Assert.That(TransferSnapshotTrace(missing.Transfer.SnapshotForTests), Is.EqualTo(missingTransferBefore));

            var mismatch = CreateRig(); CompleteEmptyTick(mismatch);
            var mismatchCombatBefore = CombatTrace(mismatch.Combat.LatestOutcomeForTests);
            var mismatchAttackBefore = AttackStateTrace(mismatch.Combat); var mismatchHealthBefore = HealthStateTrace(mismatch.Combat, "player", "walker");
            mismatch.Transfer.SubmitForTests(TransferInput(1, 2, null));
            mismatch.Combat.SubmitForTests(CombatInput(1, 3, new BasicAttackPressed(0, 3, new SimulationTick(1))));
            mismatch.Transfer.AdvanceTransferPhaseForTests(); var mismatchTransferBefore = TransferSnapshotTrace(mismatch.Transfer.SnapshotForTests);
            Assert.Throws<InvalidOperationException>(() => mismatch.Combat.AdvanceCombatPhaseForTests());
            Assert.That(mismatch.Combat.NextCombatTickForTests.Value, Is.EqualTo(1));
            Assert.That(CombatTrace(mismatch.Combat.LatestOutcomeForTests), Is.EqualTo(mismatchCombatBefore));
            Assert.That(AttackStateTrace(mismatch.Combat), Is.EqualTo(mismatchAttackBefore)); Assert.That(HealthStateTrace(mismatch.Combat, "player", "walker"), Is.EqualTo(mismatchHealthBefore));
            Assert.That(TransferSnapshotTrace(mismatch.Transfer.SnapshotForTests), Is.EqualTo(mismatchTransferBefore));
        }

        // AC-WT-003, AC-WT-006. A prior aim receipt remains diagnostic state only and cannot authorize capture after an intervening no-aim transfer tick.
        [Test]
        public void StaleReceiptCannotBeReusedAfterEmptyTransferTick()
        {
            var rig = CreateRig(); CompleteEmptyTick(rig);
            SubmitSharedAim(rig, 1, 1, null);
            rig.Transfer.AdvanceTransferPhaseForTests(); rig.Combat.AdvanceCombatPhaseForTests(); rig.Player.AdvanceOneFixedTickForTests();
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.SubmitForTests(CombatInput(2, 2, null));
            Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
            Assert.That(rig.Combat.NextCombatTickForTests.Value, Is.EqualTo(2));
        }

        // AC-COM-003. The conservative horizon covers 21, authored 45, and authored 600 at exact pass/overflow boundaries.
        [TestCase(0, 21)]
        [TestCase(45, 45)]
        [TestCase(600, 600)]
        public void HorizonPreflightPinsActualAuthoredMaximums(int authoredMaximum, int effectiveHorizon)
        {
            Assert.DoesNotThrow(() => CombatSimulationDriver.PreflightHorizon(new SimulationTick(int.MaxValue - effectiveHorizon), authoredMaximum));
            Assert.Throws<OverflowException>(() => CombatSimulationDriver.PreflightHorizon(new SimulationTick(int.MaxValue - effectiveHorizon + 1), authoredMaximum));
        }

        // AC-COM-003, AC-WT-003. The actual driver consumes the overflowing tick, then rejects before M2A, M1, health, or prior transfer publication changes.
        [Test]
        public void DriverHorizonOverflowLeavesCompletePublishedStateAtPriorBoundary()
        {
            var start = int.MaxValue - 599; var rig = CreateRig(start, true);
            var attackBefore = AttackStateTrace(rig.Combat); var healthBefore = HealthStateTrace(rig.Combat, "player", "walker", "zz600");
            var latestBefore = rig.Combat.LatestOutcomeForTests;
            rig.Transfer.AdvanceTransferPhaseForTests(); var transferBefore = TransferSnapshotTrace(rig.Transfer.SnapshotForTests);
            Assert.Throws<OverflowException>(() => rig.Combat.AdvanceCombatPhaseForTests());
            Assert.That(rig.Combat.NextCombatTickForTests.Value, Is.EqualTo(start));
            Assert.That(AttackStateTrace(rig.Combat), Is.EqualTo(attackBefore));
            Assert.That(HealthStateTrace(rig.Combat, "player", "walker", "zz600"), Is.EqualTo(healthBefore));
            Assert.That(rig.Combat.LatestOutcomeForTests.Equals(latestBefore), Is.True);
            Assert.That(TransferSnapshotTrace(rig.Transfer.SnapshotForTests), Is.EqualTo(transferBefore));
        }

        // AC-COM-003. Reset exclusivity, exact request ticks, defensive copying, and unique future buffering reject ambiguity before session work.
        [Test]
        public void CombatInputAndBufferEnforceExactExclusiveDefensiveOwnership()
        {
            var tick = new SimulationTick(1); var requests = new List<DamageRequest> { new DamageRequest("a", "hazard", "walker", 1, DamageKind.HeavyImpact, tick) };
            var input = new CombatSimulationInput(tick, null, null, null, requests, false); requests.Clear();
            Assert.That(input.ExternalDamageRequests.Count, Is.EqualTo(1));
            Assert.Throws<ArgumentException>(() => new CombatSimulationInput(tick, Camera(0), Aim(1, 1), null, Array.Empty<DamageRequest>(), true));
            Assert.Throws<ArgumentException>(() => new CombatSimulationInput(tick, null, null, null, new[] { new DamageRequest("late", "hazard", "walker", 1, DamageKind.HeavyImpact, new SimulationTick(0)) }, false));
            var rig = CreateRig(); rig.Combat.SubmitForTests(input);
            Assert.Throws<InvalidOperationException>(() => rig.Combat.SubmitForTests(input));
        }

        // AC-COM-003, AC-UX-007. M2A appends to the unsorted producer batch once; M1 alone returns canonical ID order and preserves cooldown edges.
        [Test]
        public void AttackBatchCanonicalizationAndCooldownEdgesRemainOwnedByM1AndM2A()
        {
            var rig = CreateRig(); OpenWalkerForBasicAttack(rig);
            var tick1 = new SimulationTick(2);
            var external = new[]
            {
                new DamageRequest("z", "hazard", "walker", 1, DamageKind.HeavyImpact, tick1),
                new DamageRequest("a", "hazard", "walker", 1, DamageKind.HeavyImpact, tick1)
            };
            rig.Transfer.SubmitForTests(TransferInput(2, 1, null));
            rig.Combat.SubmitForTests(new CombatSimulationInput(tick1, Camera(1), Aim(2, 1), new BasicAttackPressed(0, 1, tick1), external, false));
            rig.Transfer.AdvanceTransferPhaseForTests();
            var first = rig.Combat.AdvanceCombatPhaseForTests();
            CollectionAssert.AreEqual(new[] { "Player.BasicAttack/0", "a", "z" }, ResultIds(first));
            Assert.That(Snapshot(first, "walker").CurrentHealth, Is.EqualTo(4));
            rig.Player.AdvanceOneFixedTickForTests();

            AdvanceThrough(rig, 22);
            SubmitSharedAim(rig, 22, 22, new BasicAttackPressed(22, 22, new SimulationTick(22)));
            rig.Transfer.AdvanceTransferPhaseForTests(); var blocked = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(blocked.Attack.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.Cooldown)); Assert.That(blocked.DamageResults.Count, Is.EqualTo(0));
            rig.Player.AdvanceOneFixedTickForTests();
            SubmitSharedAim(rig, 23, 23, new BasicAttackPressed(23, 23, new SimulationTick(23)));
            rig.Transfer.AdvanceTransferPhaseForTests(); var allowed = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(allowed.Attack.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredHit)); Assert.That(Snapshot(allowed, "walker").CurrentHealth, Is.EqualTo(1));
        }

        // AC-COM-003, AC-WT-005. A killed co-authored target stays transferred through combat tick t, then one merged t+1 removal clears it exactly once.
        [Test]
        public void DeathSchedulesOneNextTickTransferRemovalWithoutRetroactiveClear()
        {
            var rig = CreateRig(); CompleteEmptyTick(rig);
            rig.Transfer.SubmitForTests(TransferInput(1, 1, new TransferPressed(1, new SimulationTick(1))));
            rig.Transfer.SubmitForTests(new TransferSimulationInput(new SimulationTick(2), null, null, new TransferPressed(null, new SimulationTick(2)), Array.Empty<TransferTargetRemoved>(), null));
            var lethal = new DamageRequest("kill", "hazard", "walker", 9, DamageKind.HeavyImpact, new SimulationTick(1));
            rig.Combat.SubmitForTests(new CombatSimulationInput(new SimulationTick(1), Camera(0), Aim(1, 1), null, new[] { lethal }, false));
            var transferAtDeathTick = rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.That(transferAtDeathTick.Snapshot.ActiveTargetId, Is.EqualTo("walker"));
            var death = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(Snapshot(death, "walker").IsDead, Is.True);
            Assert.That(rig.Sink.IsAppliedForTests, Is.True, "combat must not retroactively clear the completed transfer phase");
            rig.Player.AdvanceOneFixedTickForTests();

            var removal = rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.That(removal.RemovalResult.Value.Cleared.Value.Reason, Is.EqualTo(TransferClearReason.TargetRemoved));
            Assert.That(removal.Snapshot.ActiveTargetId, Is.Empty); Assert.That(rig.Sink.IsAppliedForTests, Is.False);
            Assert.That(removal.PressResult.Value.FailureReason, Is.EqualTo(TransferFailureReason.Cooldown), "the pre-existing t+1 press survives removal merging");
        }

        // AC-COM-003, AC-WT-005. Duplicate and later TargetDead results cannot enqueue a second removal after the sole killing transition.
        [Test]
        public void DuplicateAndRepeatedTargetDeadResultsScheduleNoSecondRemoval()
        {
            var rig = CreateRig(); CompleteEmptyTick(rig); var tick1 = new SimulationTick(1);
            rig.Transfer.SubmitForTests(TransferInput(1, 1, new TransferPressed(1, tick1)));
            var first = new DamageRequest("kill", "hazard", "walker", 9, DamageKind.HeavyImpact, tick1);
            var duplicate = new DamageRequest("kill", "other", "walker", 1, DamageKind.EnemyContact, tick1);
            rig.Combat.SubmitForTests(new CombatSimulationInput(tick1, Camera(0), Aim(1, 1), null, new[] { duplicate, first }, false));
            rig.Transfer.AdvanceTransferPhaseForTests(); var death = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(death.DamageResults[0].Result, Is.EqualTo(DamageResultCode.Applied)); Assert.That(death.DamageResults[1].Result, Is.EqualTo(DamageResultCode.Duplicate)); rig.Player.AdvanceOneFixedTickForTests();
            var removal = rig.Transfer.AdvanceTransferPhaseForTests(); Assert.That(removal.RemovalResult.HasValue, Is.True);
            var deadRequest = new DamageRequest("after", "hazard", "walker", 1, DamageKind.HeavyImpact, new SimulationTick(2));
            rig.Combat.SubmitForTests(new CombatSimulationInput(new SimulationTick(2), null, null, null, new[] { deadRequest }, false));
            var dead = rig.Combat.AdvanceCombatPhaseForTests(); Assert.That(dead.DamageResults[0].Result, Is.EqualTo(DamageResultCode.TargetDead)); rig.Player.AdvanceOneFixedTickForTests();
            var tick3 = rig.Transfer.AdvanceTransferPhaseForTests(); Assert.That(tick3.RemovalResult.HasValue, Is.False);
        }

        // AC-COM-003, AC-WT-005. Death and invulnerability on combatants without TransferTarget authoring never create transfer removals.
        [Test]
        public void NoTransferTargetDeathAndInvulnerabilityProduceNoRemoval()
        {
            var rig = CreateRig(); CompleteEmptyTick(rig); var tick1 = new SimulationTick(1);
            var hit = new DamageRequest("a-hit", "hazard", "player", 1, DamageKind.EnemyContact, tick1);
            var invulnerable = new DamageRequest("b-invulnerable", "hazard", "player", 1, DamageKind.EnemyContact, tick1);
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.SubmitForTests(new CombatSimulationInput(tick1, null, null, null, new[] { invulnerable, hit }, false));
            var first = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(first.DamageResults[0].Result, Is.EqualTo(DamageResultCode.Applied)); Assert.That(first.DamageResults[1].Result, Is.EqualTo(DamageResultCode.Invulnerable)); rig.Player.AdvanceOneFixedTickForTests();
            rig.Transfer.AdvanceTransferPhaseForTests();
            var lethal = new DamageRequest("kill-player", "hazard", "player", 9, DamageKind.HeavyImpact, new SimulationTick(2));
            rig.Combat.SubmitForTests(new CombatSimulationInput(new SimulationTick(2), null, null, null, new[] { lethal }, false));
            var stillInvulnerable = rig.Combat.AdvanceCombatPhaseForTests(); Assert.That(stillInvulnerable.DamageResults[0].Result, Is.EqualTo(DamageResultCode.Invulnerable)); rig.Player.AdvanceOneFixedTickForTests();
            AdvanceThrough(rig, 46);
            rig.Transfer.AdvanceTransferPhaseForTests();
            var finalHit = new DamageRequest("kill-player-final", "hazard", "player", 9, DamageKind.HeavyImpact, new SimulationTick(46));
            rig.Combat.SubmitForTests(new CombatSimulationInput(new SimulationTick(46), null, null, null, new[] { finalHit }, false));
            var death = rig.Combat.AdvanceCombatPhaseForTests(); Assert.That(death.DamageResults[0].Result, Is.EqualTo(DamageResultCode.Applied)); Assert.That(Snapshot(death, "player").IsDead, Is.True); rig.Player.AdvanceOneFixedTickForTests();
            var tick47 = rig.Transfer.AdvanceTransferPhaseForTests(); Assert.That(tick47.RemovalResult.HasValue, Is.False);
        }

        // AC-COM-003. Explicit reset retains global t+1 while restoring frozen health and clearing attack sequence/cooldown plus M1 tombstones.
        [Test]
        public void EncounterResetRestoresFrozenStateWithoutRewindingGlobalTick()
        {
            var rig = CreateRig(); CompleteEmptyTick(rig);
            var tick1 = new SimulationTick(1); var stable = new DamageRequest("stable", "hazard", "walker", 1, DamageKind.HeavyImpact, tick1);
            rig.Transfer.SubmitForTests(TransferInput(1, 1, null));
            rig.Combat.SubmitForTests(new CombatSimulationInput(tick1, Camera(0), Aim(1, 1), new BasicAttackPressed(0, 1, tick1), new[] { stable }, false));
            rig.Transfer.AdvanceTransferPhaseForTests(); var damaged = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(Snapshot(damaged, "walker").CurrentHealth, Is.EqualTo(8)); rig.Player.AdvanceOneFixedTickForTests();

            // Mutable authoring is deliberately corrupted after initialization; reset must use only the previously frozen copies.
            rig.WalkerCombat.ConfigureForTests("walker", 999, 600, false, 0, 0, 0, 0, 100, 100, rig.WalkerCollider, rig.WalkerCombat.transform, rig.WalkerTransfer);

            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.SubmitForTests(new CombatSimulationInput(new SimulationTick(2), null, null, null, Array.Empty<DamageRequest>(), true));
            var reset = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(Snapshot(reset, "walker").CurrentHealth, Is.EqualTo(9));
            Assert.That(reset.Attack.Snapshot.CooldownEndTick, Is.EqualTo(-1)); Assert.That(reset.Attack.Snapshot.ConsumedSequenceCount, Is.EqualTo(0));
            Assert.That(reset.Attack.Snapshot.NextExpectedTick.Value, Is.EqualTo(3)); rig.Player.AdvanceOneFixedTickForTests();

            var tick3 = new SimulationTick(3); var reusedRequestId = new DamageRequest("stable", "hazard", "walker", 1, DamageKind.HeavyImpact, tick3);
            rig.Transfer.SubmitForTests(TransferInput(3, 3, null));
            rig.Combat.SubmitForTests(new CombatSimulationInput(tick3, Camera(2), Aim(3, 3), new BasicAttackPressed(0, 3, tick3), new[] { reusedRequestId }, false));
            rig.Transfer.AdvanceTransferPhaseForTests(); var reusedSequence = rig.Combat.AdvanceCombatPhaseForTests();
            Assert.That(reusedSequence.Attack.Attempt.Value.Code, Is.EqualTo(BasicAttackAttemptCode.FiredMiss));
            Assert.That(reusedSequence.DamageResults.Count, Is.EqualTo(1));
            Assert.That(reusedSequence.DamageResults[0].RequestId, Is.EqualTo("stable")); Assert.That(reusedSequence.DamageResults[0].Result, Is.EqualTo(DamageResultCode.Applied));
            Assert.That(Snapshot(reusedSequence, "walker").CurrentHealth, Is.EqualTo(8));
        }

        // AC-COM-003, AC-WT-005, AC-WT-006, AC-UX-007. Render grouping cannot alter any normalized transfer, attack, damage, health, death-handoff, or next-tick scalar.
        [Test]
        public void ThirtySixtyAndOneFortyFourRenderGroupsProduceIdenticalCompletePhaseTrace()
        {
            CollectionAssert.AreEqual(ReplayAtRenderRate(30), ReplayAtRenderRate(60));
            CollectionAssert.AreEqual(ReplayAtRenderRate(30), ReplayAtRenderRate(144));
        }

        // AC-WT-003, AC-WT-005. Identical duplicates collapse idempotently; a mixed-tick conflict rejects without replacing the queued transfer payload.
        [Test]
        public void RemovalMergeIsIdempotentAndMixedTickConflictDoesNotReplaceQueuedInput()
        {
            var rig = CreateRig(); CompleteEmptyTick(rig);
            var removal = new TransferTargetRemoved("walker", new SimulationTick(2));
            rig.Transfer.SubmitForTests(new TransferSimulationInput(new SimulationTick(2), null, null, new TransferPressed(null, new SimulationTick(2)), new[] { removal }, null));
            Assert.DoesNotThrow(() => rig.Transfer.MergeRemovals(new[] { removal, removal }));
            Assert.Throws<InvalidOperationException>(() => rig.Transfer.MergeRemovals(new[] { removal, new TransferTargetRemoved("other", new SimulationTick(3)) }));
            rig.Transfer.AdvanceTransferPhaseForTests(); rig.Combat.AdvanceCombatPhaseForTests(); rig.Player.AdvanceOneFixedTickForTests();
            var retained = rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.That(retained.PressResult.HasValue, Is.True);
        }

        private Rig CreateRig(int firstTick = 0, bool includeInvulnerability600 = false, bool includeSurveyor = true, int surveyorHealth = 6)
        {
            var playerObject = Track(new GameObject("player")); var body = playerObject.AddComponent<Rigidbody2D>(); body.bodyType = RigidbodyType2D.Kinematic; var playerCollider = playerObject.AddComponent<CapsuleCollider2D>(); var player = playerObject.AddComponent<PlayerMovementController>();
            if (firstTick != 0)
            {
                // REQ-COM-004, AC-COM-003. High-tick capture still consumes the
                // completed t-1 pose; seed it through the existing motor test seam.
                typeof(PlayerMovementController).GetField("_motor", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(player, new PlayerMovementMotor(new SimulationTick(checked(firstTick - 1))));
                player.AdvanceOneFixedTickForTests();
            }
            var playerCombat = playerObject.AddComponent<CombatTarget>(); playerCombat.ConfigureForTests("player", 5, 45, false, 0, 0, 0, 0, 100, 100, playerCollider, playerObject.transform);

            var walkerObject = Track(new GameObject("walker")); walkerObject.transform.position = new Vector2(1f, 0f); var walkerCollider = walkerObject.AddComponent<BoxCollider2D>();
            var sink = walkerObject.AddComponent<TransferSandboxOwnerStub>(); sink.ConfigureForTests("walker", "Target.Enemy.Heavy.v1");
            var transferTarget = walkerObject.AddComponent<TransferTarget>(); transferTarget.ConfigureForTests("walker", TransferTargetKind.Enemy, 0, 0, 0, 0, 100, 100, "base", walkerCollider, walkerObject.transform, sink);
            var walkerCombat = walkerObject.AddComponent<CombatTarget>(); walkerCombat.ConfigureForTests("walker", 9, 0, true, 0, 0, 0, 0, 100, 100, walkerCollider, walkerObject.transform, transferTarget);

            CombatTarget surveyorCombat = null; TransferTarget surveyorTransfer = null;
            if (includeSurveyor)
            {
                var surveyorObject = Track(new GameObject("surveyor")); surveyorObject.transform.position = new Vector2(100f, 0f); var surveyorCollider = surveyorObject.AddComponent<BoxCollider2D>();
                var surveyorSink = surveyorObject.AddComponent<TransferSandboxOwnerStub>(); surveyorSink.ConfigureForTests("surveyor", "Target.Enemy.Heavy.v1");
                surveyorTransfer = surveyorObject.AddComponent<TransferTarget>(); surveyorTransfer.ConfigureForTests("surveyor", TransferTargetKind.Enemy, 0, 0, 0, 0, 100, 100, "base", surveyorCollider, surveyorObject.transform, surveyorSink);
                surveyorCombat = surveyorObject.AddComponent<CombatTarget>(); surveyorCombat.ConfigureForTests("surveyor", surveyorHealth, 0, true, 0, 0, 0, 0, 100, 100, surveyorCollider, surveyorObject.transform, surveyorTransfer);
            }

            var transferRegistry = Track(new GameObject("TransferRegistry")).AddComponent<TransferTargetRegistry>();
            if (includeSurveyor) transferRegistry.ConfigureForTests(surveyorTransfer, transferTarget); else transferRegistry.ConfigureForTests(transferTarget);
            var transferDriver = Track(new GameObject("TransferDriver")).AddComponent<TransferSimulationDriver>(); transferDriver.ConfigureForTests(player, transferRegistry); transferDriver.InitializeForTests();
            CombatTarget overflowTarget = null;
            if (includeInvulnerability600) { var overflowObject = Track(new GameObject("zz600")); var overflowCollider = overflowObject.AddComponent<BoxCollider2D>(); overflowTarget = overflowObject.AddComponent<CombatTarget>(); overflowTarget.ConfigureForTests("zz600", 1, 600, false, 0, 0, 0, 0, 100, 100, overflowCollider, overflowObject.transform); }
            var combatRegistry = Track(new GameObject("CombatRegistry")).AddComponent<CombatTargetRegistry>();
            if (!includeSurveyor) combatRegistry.ConfigureForTests(playerCombat, walkerCombat);
            else if (overflowTarget == null) combatRegistry.ConfigureForTests(playerCombat, surveyorCombat, walkerCombat); else combatRegistry.ConfigureForTests(playerCombat, surveyorCombat, walkerCombat, overflowTarget);
            var combatDriver = Track(new GameObject("CombatDriver")).AddComponent<CombatSimulationDriver>();
            var reactionDriver = Track(new GameObject("ReactionDriver")).AddComponent<RegularEnemyReactionSimulationDriver>();
            reactionDriver.ConfigureForTests(player, transferDriver, combatDriver);
            combatDriver.ConfigureForTests(player, combatRegistry, transferDriver, reactionDriver); combatDriver.InitializeForTests();
            var behaviorDriver = Track(new GameObject("BehaviorDriver")).AddComponent<RegularEnemyBehaviorSimulationDriver>();
            behaviorDriver.ConfigureForTests(player, combatDriver, reactionDriver);
            return new Rig(player, transferDriver, combatDriver, reactionDriver, behaviorDriver, sink, walkerCombat, surveyorCombat, transferTarget, walkerCollider);
        }

        private List<string> ReplayAtRenderRate(int renderFramesPerSecond)
        {
            var rig = CreateRig(); var trace = new List<string>(); var allowance = 0d;
            while (trace.Count < 24)
            {
                allowance += 60d / renderFramesPerSecond;
                while (allowance >= 1d && trace.Count < 24)
                {
                    var tick = trace.Count; var simulationTick = new SimulationTick(tick);
                    if (tick == 1)
                    {
                        rig.Transfer.SubmitForTests(TransferInput(tick, 1, new TransferPressed(1, simulationTick)));
                        rig.Combat.SubmitForTests(CombatInput(tick, 1, new BasicAttackPressed(0, 1, simulationTick)));
                    }
                    else if (tick == 21)
                    {
                        SubmitSharedAim(rig, tick, 21, new BasicAttackPressed(21, 21, simulationTick));
                    }
                    else if (tick == 22)
                    {
                        rig.Transfer.SubmitForTests(TransferInput(tick, 22, null));
                        var lethal = new DamageRequest("kill", "hazard", "walker", 9, DamageKind.HeavyImpact, simulationTick);
                        rig.Combat.SubmitForTests(new CombatSimulationInput(simulationTick, Camera(21), Aim(22, 22), new BasicAttackPressed(22, 22, simulationTick), new[] { lethal }, false));
                    }

                    var transfer = rig.Transfer.AdvanceTransferPhaseForTests();
                    var combat = rig.Combat.AdvanceCombatPhaseForTests();
                    var behavior = rig.Behavior.AdvanceBehaviorPhaseForTests();
                    trace.Add(TransferTrace(transfer) + "|" + CombatTrace(combat) + "|" + PhaseClockAndReceiptTrace(rig) + "|" + BehaviorTrace(behavior));
                    rig.Player.AdvanceOneFixedTickForTests(); allowance -= 1d;
                }
            }

            return trace;
        }

        private static void CompleteEmptyTick(Rig rig) { rig.Transfer.AdvanceTransferPhaseForTests(); rig.Combat.AdvanceCombatPhaseForTests(); rig.Player.AdvanceOneFixedTickForTests(); }
        private static CombatSimulationOutcome AdvanceEmptyFixed(Rig rig)
        {
            rig.Transfer.AdvanceTransferPhaseForTests();
            var outcome = rig.Combat.AdvanceCombatPhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();
            return outcome;
        }
        private static void AdvanceBehaviorFixed(Rig rig)
        {
            rig.Transfer.AdvanceTransferPhaseForTests();
            rig.Combat.AdvanceCombatPhaseForTests();
            rig.Behavior.AdvanceBehaviorPhaseForTests();
        }
        private static void OpenWalkerForBasicAttack(Rig rig)
        {
            var current = rig.Player.NextExpectedTick;
            rig.Reaction.SubmitRecoveryForTests(new AttackRecoveryStarted(1000 + current.Value, new SimulationTick(checked(current.Value + 1))));
            CompleteEmptyTick(rig);
            CompleteEmptyTick(rig);
        }
        private void AssertReactionOverflowPreservesUpstream(int tickValue, bool surveyorHeavy)
        {
            var rig = CreateRig(tickValue);
            if (surveyorHeavy)
            {
                rig.SurveyorCombat.transform.position = new Vector2(1f, 0f);
                var tick = rig.Player.NextExpectedTick;
                rig.Transfer.SubmitForTests(TransferInput(tick.Value, 1, new TransferPressed(1, tick)));
            }
            else
            {
                var tick = rig.Player.NextExpectedTick;
                rig.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null,
                    new[] { new DamageRequest("overflow-impact", "hazard", "walker", 1, DamageKind.HeavyImpact, tick) }, false));
            }
            var expected = rig.Player.NextExpectedTick;
            rig.Transfer.AdvanceTransferPhaseForTests();
            Assert.Throws<OverflowException>(() => rig.Combat.AdvanceCombatPhaseForTests());
            Assert.That(rig.Combat.LatestOutcomeForTests.Tick, Is.EqualTo(expected));
            Assert.That(rig.Transfer.LatestPhasePublication.Value.Tick, Is.EqualTo(expected));
            Assert.That(rig.Reaction.CarriedViewForTests.HasValue, Is.False);
            Assert.That(rig.Reaction.WalkerNextExpectedTickForTests, Is.EqualTo(expected));
            Assert.That(rig.Reaction.SurveyorNextExpectedTickForTests, Is.EqualTo(expected));
            Assert.That(rig.Reaction.QueuedRecoveryCountForTests, Is.EqualTo(0));
        }
        private static void AdvanceThrough(Rig rig, int exclusiveTick) { while (rig.Player.NextExpectedTick.Value < exclusiveTick) { rig.Transfer.AdvanceTransferPhaseForTests(); rig.Combat.AdvanceCombatPhaseForTests(); rig.Player.AdvanceOneFixedTickForTests(); } }
        private static void SubmitSharedAim(Rig rig, int tick, long sampleId, BasicAttackPressed? press) { rig.Transfer.SubmitForTests(TransferInput(tick, sampleId, null)); rig.Combat.SubmitForTests(CombatInput(tick, sampleId, press)); }
        private static TransferSimulationInput TransferInput(int tick, long sampleId, TransferPressed? press) => new TransferSimulationInput(new SimulationTick(tick), Camera(tick - 1), Aim(tick, sampleId), press, Array.Empty<TransferTargetRemoved>(), null);
        private static CombatSimulationInput CombatInput(int tick, long sampleId, BasicAttackPressed? press) => new CombatSimulationInput(new SimulationTick(tick), Camera(tick - 1), Aim(tick, sampleId), press, Array.Empty<DamageRequest>(), false);
        private static SimulationCameraPoseSnapshot Camera(int tick) => new SimulationCameraPoseSnapshot(new SimulationTick(tick), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
        private static AimSample Aim(int tick, long sampleId) => new AimSample(sampleId, AimSource.MousePointer, 1014, 540, true, 4096, 0, new SimulationTick(tick - 1), new SimulationTick(tick));
        private static int Order<T>() => ((DefaultExecutionOrder)Attribute.GetCustomAttribute(typeof(T), typeof(DefaultExecutionOrder))).order;
        private static CombatantSnapshot Snapshot(CombatSimulationOutcome outcome, string targetId) { for (var i = 0; i < outcome.Combatants.Count; i++) if (outcome.Combatants[i].TargetId == targetId) return outcome.Combatants[i]; throw new AssertionException("Missing combatant snapshot: " + targetId); }
        private static string[] ResultIds(CombatSimulationOutcome outcome) { var values = new string[outcome.DamageResults.Count]; for (var i = 0; i < values.Length; i++) values[i] = outcome.DamageResults[i].RequestId; return values; }
        private static string TransferTrace(TransferTickOutcome value) =>
            "transfer={" + TransferSnapshotTrace(value.Snapshot) + "},removal={" + TransferAttemptTrace(value.RemovalResult) + "},press={" + TransferAttemptTrace(value.PressResult) + "}";
        private static string TransferAttemptTrace(TransferAttemptResult? result)
        {
            if (!result.HasValue) return "none";
            var value = result.Value;
            return "tick=" + value.Tick.Value + ",aim=" + (value.AimSampleId.HasValue ? value.AimSampleId.Value.ToString() : "null") + ",selected=" + value.SelectedTargetId + ",success=" + value.Success + ",failure=" + value.FailureReason +
                ",session={" + TransferSnapshotTrace(value.Session) + "},changed=" + (value.StateChanged.HasValue ? value.StateChanged.Value.TargetId + ":" + value.StateChanged.Value.PlayerModifierId + ":" + value.StateChanged.Value.TargetModifierId + ":" + value.StateChanged.Value.Tick.Value + ":" + value.StateChanged.Value.Revision : "none") +
                ",cleared=" + (value.Cleared.HasValue ? value.Cleared.Value.Reason + ":" + value.Cleared.Value.PreviousTargetId + ":" + value.Cleared.Value.Tick.Value + ":" + value.Cleared.Value.Revision : "none");
        }
        private static string TransferSnapshotTrace(TransferSessionSnapshot value) => value.Tick.Value + ":" + value.HighlightedTargetId + ":" + value.ActiveTargetId + ":" + value.PlayerState + ":" + value.TransitionLockEndTick + ":" + value.LastFailureReason + ":" + value.Revision;
        private static string CombatTrace(CombatSimulationOutcome value)
        {
            var builder = new StringBuilder(); var attack = value.Attack;
            builder.Append("combat=").Append(value.Tick.Value).Append(",selection=").Append(attack.Selection.Tick.Value).Append(':').Append(attack.Selection.TargetId);
            builder.Append(",attempt=").Append(attack.Attempt.HasValue ? attack.Attempt.Value.AttackSequenceId + ":" + attack.Attempt.Value.Tick.Value + ":" + attack.Attempt.Value.Code + ":" + attack.Attempt.Value.SelectedTargetId + ":" + attack.Attempt.Value.Fired : "none");
            builder.Append(",request=").Append(attack.DamageRequest.HasValue ? attack.DamageRequest.Value.RequestId + ":" + attack.DamageRequest.Value.SourceId + ":" + attack.DamageRequest.Value.TargetId + ":" + attack.DamageRequest.Value.Amount + ":" + attack.DamageRequest.Value.Kind + ":" + attack.DamageRequest.Value.Tick.Value : "none");
            builder.Append(",attackSnapshot=").Append(attack.Snapshot.Tick.Value).Append(':').Append(attack.Snapshot.HighlightedTargetId).Append(':').Append(attack.Snapshot.CooldownEndTick).Append(':').Append(attack.Snapshot.ConsumedSequenceCount).Append(':').Append(attack.Snapshot.NextExpectedTick.Value);
            builder.Append(",results="); for (var i = 0; i < value.DamageResults.Count; i++) { var result = value.DamageResults[i]; builder.Append('[').Append(result.RequestId).Append(':').Append(result.TargetId).Append(':').Append(result.AppliedAmount).Append(':').Append(result.Result).Append(':').Append(result.Tick.Value).Append(']'); }
            builder.Append(",combatants="); for (var i = 0; i < value.Combatants.Count; i++) { var target = value.Combatants[i]; builder.Append('[').Append(target.TargetId).Append(':').Append(target.CurrentHealth).Append(':').Append(target.IsDead).Append(':').Append(target.InvulnerabilityEndTick).Append(':').Append(target.AcceptedRequestCount).Append(']'); }
            return builder.ToString();
        }
        private static string PhaseClockAndReceiptTrace(Rig rig)
        {
            var receipt = rig.Transfer.LatestCaptureReceipt;
            var receiptTrace = receipt.HasValue
                ? receipt.Value.SimulationTick.Value + ":" + receipt.Value.AimSampleId + ":" + receipt.Value.AimSampleTick.Value + ":" + receipt.Value.CameraPoseTick.Value
                : "none";
            var carried = rig.Reaction.CarriedViewForTests;
            var reactionTrace = carried.HasValue
                ? carried.Value.Tick.Value + ":w=" + ReactionSnapshotTrace(carried.Value.Walker) + ":s=" + ReactionSnapshotTrace(carried.Value.Surveyor)
                : "none";
            return "clocks=combat:" + rig.Combat.NextCombatTickForTests.Value + ",movement:" + rig.Player.NextExpectedTick.Value + ",syncCount:" + rig.Transfer.SyncCaptureCountForTests + ",receipt:" + receiptTrace + ",reaction:" + reactionTrace;
        }
        private static string ReactionSnapshotTrace(RegularEnemyReactionSnapshot value) =>
            value.Tick.Value + ":" + value.IsAlive + ":" + value.IsHeavy + ":" + value.LastTransferRevision + ":" + value.ShieldWindowEndTick + ":" + value.FireLockEndTick + ":" + value.ReactionAllowsBasicAttack + ":" + value.RecoveryMultiplierQ1000 + ":" + value.ForcedDescent + ":" + value.CanFire + ":" + value.NextExpectedTick.Value;
        private static string BehaviorTrace(CarriedBehaviorView value)
        {
            var snapshot = value.Snapshot;
            return "behavior=" + value.Tick.Value + ":w=" + snapshot.Walker.Phase + ":" + snapshot.Walker.ExclusiveEndTick + ":" + snapshot.Walker.AttackCycleOrdinal +
                ":s=" + snapshot.Surveyor.Phase + ":" + snapshot.Surveyor.ExclusiveEndTick + ":" + snapshot.Surveyor.ShotOrdinal +
                ":recovery=" + (snapshot.WalkerRecoveryIntent.HasValue ? snapshot.WalkerRecoveryIntent.Value.AttackCycleOrdinal + ":" + snapshot.WalkerRecoveryIntent.Value.ScheduledTick.Value : "none") +
                ":shot=" + (snapshot.SurveyorShotIntent.HasValue ? snapshot.SurveyorShotIntent.Value.ShotOrdinal + ":" + snapshot.SurveyorShotIntent.Value.TargetXQ1000 : "none") + ":next=" + snapshot.NextExpectedTick.Value;
        }
        private static RegularEnemyReactionSnapshot ReTickReaction(RegularEnemyReactionSnapshot source, string targetId, SimulationTick tick) =>
            new RegularEnemyReactionSnapshot(tick, targetId, source.IsAlive, source.IsHeavy, source.LastTransferRevision, source.ShieldWindowEndTick, source.FireLockEndTick,
                source.ReactionAllowsBasicAttack, source.RecoveryMultiplierQ1000, source.ForcedDescent, source.CanFire, new SimulationTick(checked(tick.Value + 1)));
        private static string AttackStateTrace(CombatSimulationDriver driver)
        {
            var session = (BasicAttackSession)typeof(CombatSimulationDriver).GetField("_attackSession", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(driver);
            var value = session.Snapshot(session.NextExpectedTick); return value.Tick.Value + ":" + value.HighlightedTargetId + ":" + value.CooldownEndTick + ":" + value.ConsumedSequenceCount + ":" + value.NextExpectedTick.Value;
        }
        private static string HealthStateTrace(CombatSimulationDriver driver, params string[] ids)
        {
            var session = (CombatSession)typeof(CombatSimulationDriver).GetField("_combatSession", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(driver); var builder = new StringBuilder();
            for (var i = 0; i < ids.Length; i++) { var value = session.Snapshot(ids[i]); builder.Append(value.TargetId).Append(':').Append(value.CurrentHealth).Append(':').Append(value.IsDead).Append(':').Append(value.InvulnerabilityEndTick).Append(':').Append(value.AcceptedRequestCount).Append('|'); }
            return builder.ToString();
        }
        private GameObject Track(GameObject value) { _objects.Add(value); return value; }

        private readonly struct Rig
        {
            internal Rig(PlayerMovementController player, TransferSimulationDriver transfer, CombatSimulationDriver combat, RegularEnemyReactionSimulationDriver reaction, RegularEnemyBehaviorSimulationDriver behavior, TransferSandboxOwnerStub sink, CombatTarget walkerCombat, CombatTarget surveyorCombat, TransferTarget walkerTransfer, Collider2D walkerCollider) { Player = player; Transfer = transfer; Combat = combat; Reaction = reaction; Behavior = behavior; Sink = sink; WalkerCombat = walkerCombat; SurveyorCombat = surveyorCombat; WalkerTransfer = walkerTransfer; WalkerCollider = walkerCollider; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal CombatSimulationDriver Combat { get; }
            internal RegularEnemyReactionSimulationDriver Reaction { get; }
            internal RegularEnemyBehaviorSimulationDriver Behavior { get; }
            internal TransferSandboxOwnerStub Sink { get; }
            internal CombatTarget WalkerCombat { get; }
            internal CombatTarget SurveyorCombat { get; }
            internal TransferTarget WalkerTransfer { get; }
            internal Collider2D WalkerCollider { get; }
        }
    }
}
