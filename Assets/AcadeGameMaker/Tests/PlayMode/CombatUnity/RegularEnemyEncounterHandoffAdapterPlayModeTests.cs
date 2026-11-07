#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    // Positive cases use the real -200..+110 graph. Rejection cases call that
    // same private production callback only while disabled; no test API exists.
    internal sealed class RegularEnemyEncounterHandoffAdapterPlayModeTests
    {
        private const string GraphPath = "Assets/Prefabs/Combat/CombatEncounterGraph.prefab";

        // REQ-COM-001, REQ-COM-004; partial AC-COM-001, AC-COM-003.
        [UnityTest]
        public IEnumerator NeutralProductionBootOwnsOneViewAndLeavesPairsFalse()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                AssertPristine(refs.Handoff);
                yield return new WaitForFixedUpdate();
                var view = View(refs.Handoff);
                Assert.That(view.Tick.Value, Is.Zero);
                Assert.That(view.NextExpectedTick.Value, Is.EqualTo(1));
                Assert.That(view.AppliedHitTargetIds, Is.Empty);
                AssertPairs(refs, false, false);
            }
            finally { Object.Destroy(root); }
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-001, AC-COM-003. The
        // committed authored graph runs sixty real fixed phases without a
        // handoff discontinuity, skipped tick, or unsolicited lifecycle event.
        [UnityTest]
        public IEnumerator AuthoredGraphAdvancesSixtyNeutralFixedTicksConsecutively()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                for (var tick = 0; tick < 60; tick++)
                {
                    yield return new WaitForFixedUpdate();
                    var view = View(refs.Handoff);
                    Assert.That(view.Tick.Value, Is.EqualTo(tick));
                    Assert.That(view.NextExpectedTick.Value, Is.EqualTo(tick + 1));
                    Assert.That(view.Defeats, Is.Empty);
                    Assert.That(view.Rewards, Is.Empty);
                    Assert.That(view.Cleared.HasValue, Is.False);
                    AssertPairs(refs, false, false);
                }
            }
            finally { Object.Destroy(root); }
        }

        // REQ-COM-001, REQ-COM-002, REQ-COM-004; partial AC-COM-001,
        // AC-COM-003. The real Relocate(48) -> FireTelegraph(30) -> Fire
        // cycle publishes a line, keeps it after Surveyor death, then expires
        // it exactly at the immutable exclusive end tick.
        [UnityTest]
        public IEnumerator SurveyorLinePresentationMatchesThreatAndSurvivesDeathUntilExclusiveExpiry()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                var sawWalkerContact = false;
                RegularEnemyEncounterPresentationView spawned = default;
                var exclusiveEnd = -1;
                for (var step = 0; step < 100; step++)
                {
                    yield return new WaitForFixedUpdate();
                    var view = View(refs.Handoff);
                    sawWalkerContact |= ContainsKind(refs.Combat.LatestOutcomeForEncounterHandoff, DamageKind.EnemyContact);
                    var threat = refs.Threat.LatestCarriedViewForEncounterHandoff;
                    Assert.That(threat.HasValue, Is.True);
                    if (!view.HasActiveDenialLine) continue;
                    Assert.That(view.SurveyorBehavior.Phase, Is.EqualTo(SurveyorBehaviorPhase.Fire));
                    Assert.That(view.SurveyorBehavior.HasLatchedTarget, Is.True);
                    Assert.That(view.DenialLineCenterXQ4096, Is.EqualTo(threat.Value.Snapshot.LineCenterXQ4096));
                    Assert.That(view.DenialLineExclusiveEndTick, Is.EqualTo(threat.Value.Snapshot.LineExclusiveEndTick));
                    Assert.That(view.Tick, Is.EqualTo(threat.Value.Tick));
                    spawned = view;
                    exclusiveEnd = view.DenialLineExclusiveEndTick;
                    break;
                }
                Assert.That(spawned.Tick.Value, Is.EqualTo(78), "Relocate(48), FireTelegraph(30), then Fire must be exact.");
                Assert.That(exclusiveEnd, Is.GreaterThan(spawned.Tick.Value));
                Assert.That(exclusiveEnd, Is.EqualTo(spawned.Tick.Value + 30));
                // The B1 view deliberately has no M3D request-kind field. Its
                // Surveyor Fire/line fields distinguish the denial-line source;
                // the executable contact authority remains M3D's
                // AC_COM_001_StationaryThenDashCrossingContactDeliversOnlyNextTick.
                Assert.That(sawWalkerContact, Is.True);

                yield return new WaitForFixedUpdate();
                var deliveredLine = View(refs.Handoff);
                Assert.That(deliveredLine.HasActiveDenialLine, Is.True);
                Assert.That(ContainsKind(refs.Combat.LatestOutcomeForEncounterHandoff, DamageKind.EnemyRanged), Is.True);
                Assert.That(deliveredLine.SurveyorBehavior.Phase, Is.EqualTo(SurveyorBehaviorPhase.Cooldown));
                Assert.That(deliveredLine.WalkerBehavior.Phase, Is.Not.EqualTo(WalkerBehaviorPhase.Dead));

                SubmitDamage(refs, "surveyor", 6);
                yield return new WaitForFixedUpdate();
                var afterDeath = View(refs.Handoff);
                Assert.That(afterDeath.SurveyorAlive, Is.False);
                Assert.That(afterDeath.HasActiveDenialLine, Is.True);
                Assert.That(afterDeath.DenialLineCenterXQ4096, Is.EqualTo(spawned.DenialLineCenterXQ4096));
                Assert.That(afterDeath.DenialLineExclusiveEndTick, Is.EqualTo(exclusiveEnd));

                while (View(refs.Handoff).Tick.Value < exclusiveEnd - 1) yield return new WaitForFixedUpdate();
                Assert.That(View(refs.Handoff).Tick.Value, Is.EqualTo(exclusiveEnd - 1));
                Assert.That(View(refs.Handoff).HasActiveDenialLine, Is.True);
                yield return new WaitForFixedUpdate();
                Assert.That(View(refs.Handoff).Tick.Value, Is.EqualTo(exclusiveEnd));
                Assert.That(View(refs.Handoff).HasActiveDenialLine, Is.False);
                Assert.That(View(refs.Handoff).DenialLineCenterXQ4096, Is.Zero);
                Assert.That(View(refs.Handoff).DenialLineExclusiveEndTick, Is.EqualTo(-1));
            }
            finally { Object.Destroy(root); }
        }

        // REQ-COM-001, REQ-COM-002, REQ-COM-004; partial AC-COM-001, AC-COM-003.
        [UnityTest]
        public IEnumerator SequentialDeathsLatchPairsAndClearOnlyOnceAcrossTPlusOne()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                yield return new WaitForFixedUpdate();
                SubmitDamage(refs, "surveyor", 6);
                yield return new WaitForFixedUpdate();
                var surveyorDeath = View(refs.Handoff);
                Assert.That(surveyorDeath.Defeats.Count, Is.EqualTo(1));
                Assert.That(surveyorDeath.Defeats[0].TargetId, Is.EqualTo("surveyor"));
                Assert.That(surveyorDeath.Rewards.Count, Is.EqualTo(1));
                Assert.That(surveyorDeath.Cleared.HasValue, Is.False);
                AssertPairs(refs, true, false);

                SubmitDamage(refs, "walker", 9);
                yield return new WaitForFixedUpdate();
                var walkerDeath = View(refs.Handoff);
                Assert.That(walkerDeath.Defeats.Count, Is.EqualTo(1));
                Assert.That(walkerDeath.Defeats[0].TargetId, Is.EqualTo("walker"));
                Assert.That(walkerDeath.Cleared.HasValue, Is.True);
                Assert.That(walkerDeath.Completion.HasValue, Is.True);
                AssertPairs(refs, true, true);

                yield return new WaitForFixedUpdate();
                var continuation = View(refs.Handoff);
                Assert.That(continuation.Defeats, Is.Empty);
                Assert.That(continuation.Rewards, Is.Empty);
                Assert.That(continuation.Cleared.HasValue, Is.False);
                Assert.That(continuation.Completion.HasValue, Is.False);
                AssertPairs(refs, true, true);
            }
            finally { Object.Destroy(root); }
        }

        // REQ-COM-001, REQ-COM-002; partial AC-COM-001.
        [UnityTest]
        public IEnumerator SimultaneousDeathsAreRoleOrderedAndRemainOneShot()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                yield return new WaitForFixedUpdate();
                var tick = refs.Player.NextExpectedTick;
                refs.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null, new[]
                {
                    Damage("simultaneous/surveyor/" + tick.Value, "surveyor", 6, tick),
                    Damage("simultaneous/walker/" + tick.Value, "walker", 9, tick)
                }, false));
                yield return new WaitForFixedUpdate();
                var view = View(refs.Handoff);
                Assert.That(view.Defeats.Count, Is.EqualTo(2));
                Assert.That(view.Defeats[0].TargetId, Is.EqualTo("surveyor"));
                Assert.That(view.Defeats[1].TargetId, Is.EqualTo("walker"));
                Assert.That(view.Cleared.HasValue, Is.True);
                AssertPairs(refs, true, true);
                yield return new WaitForFixedUpdate();
                Assert.That(View(refs.Handoff).Defeats, Is.Empty);
                Assert.That(View(refs.Handoff).Cleared.HasValue, Is.False);
            }
            finally { Object.Destroy(root); }
        }

        // REQ-COM-002, REQ-COM-004; partial AC-COM-003.
        [UnityTest]
        public IEnumerator PlayerDeathDoesNotCompleteTheRegularEnemyEncounter()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                yield return new WaitForFixedUpdate();
                SubmitDamage(refs, "player", 5);
                yield return new WaitForFixedUpdate();
                var view = View(refs.Handoff);
                Assert.That(view.PlayerAlive, Is.False);
                Assert.That(view.Defeats, Is.Empty);
                Assert.That(view.Rewards, Is.Empty);
                Assert.That(view.Cleared.HasValue, Is.False);
                Assert.That(view.Completion.HasValue, Is.False);
                AssertPairs(refs, false, false);
            }
            finally { Object.Destroy(root); }
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003.
        [UnityTest]
        public IEnumerator PreDeathResetPublishesAnEmptyResetView()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                yield return new WaitForFixedUpdate();
                var tick = refs.Player.NextExpectedTick;
                refs.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null, Array.Empty<DamageRequest>(), true));
                yield return new WaitForFixedUpdate();
                var view = View(refs.Handoff);
                Assert.That(view.Tick, Is.EqualTo(tick));
                Assert.That(view.AppliedHitTargetIds, Is.Empty);
                Assert.That(view.Defeats, Is.Empty);
                Assert.That(view.Rewards, Is.Empty);
                Assert.That(view.Cleared.HasValue, Is.False);
                AssertPairs(refs, false, false);
            }
            finally { Object.Destroy(root); }
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003.
        [UnityTest]
        public IEnumerator PostDeathResetRejectsAndPreservesThePriorHandoff()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                yield return new WaitForFixedUpdate();
                SubmitDamage(refs, "surveyor", 6);
                yield return new WaitForFixedUpdate();
                var before = View(refs.Handoff);
                refs.Handoff.enabled = false;
                var tick = refs.Player.NextExpectedTick;
                refs.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null, Array.Empty<DamageRequest>(), true));
                LogAssert.Expect(LogType.Exception, new Regex("InvalidOperationException: Encounter reset requires an idle transfer tick"));
                LogAssert.Expect(LogType.Exception, new Regex("InvalidOperationException: Reaction publication requires the exact successful combat outcome"));
                LogAssert.Expect(LogType.Exception, new Regex("InvalidOperationException: Behavior phase requires the exact current combat publication"));
                LogAssert.Expect(LogType.Exception, new Regex("InvalidOperationException: Locomotion requires the exact current behavior publication"));
                LogAssert.Expect(LogType.Exception, new Regex("InvalidOperationException: Threat phase requires exact current Combat, Behavior, and Motion publications"));
                yield return new WaitForFixedUpdate();
                AssertProductionFixedRejects(refs.Handoff);
                Assert.That(View(refs.Handoff).Tick, Is.EqualTo(before.Tick));
                AssertPairs(refs, true, false);
            }
            finally { Object.Destroy(root); }
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-003.
        [UnityTest]
        public IEnumerator ExternalPairDriftRejectsBeforeCoreMutation()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                yield return new WaitForFixedUpdate();
                var before = View(refs.Handoff);
                refs.Handoff.enabled = false;
                yield return new WaitForFixedUpdate();
                Physics2D.IgnoreCollision(refs.PlayerCollider, refs.SurveyorCollider, true);
                AssertProductionFixedRejects(refs.Handoff);
                Assert.That(View(refs.Handoff).Tick, Is.EqualTo(before.Tick));
                Assert.That(refs.Handoff.SurveyorPairIgnoredForEncounterHandoff, Is.False);
                Assert.That(Physics2D.GetIgnoreCollision(refs.PlayerCollider, refs.SurveyorCollider), Is.True);
                Physics2D.IgnoreCollision(refs.PlayerCollider, refs.SurveyorCollider, false);
            }
            finally { Object.Destroy(root); }
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-003.
        [Test]
        public void MissingFirstUseSourceLeavesEveryAdapterStatePristine()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                refs.Handoff.enabled = false;
                AssertProductionFixedRejects(refs.Handoff);
                AssertPristine(refs.Handoff);
                AssertPairs(refs, false, false);
            }
            finally { Object.DestroyImmediate(root); }
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-003. A malformed
        // serialized graph fails as a controlled adapter validation before it
        // can attempt to read an upstream publication or retain any state.
        [Test]
        public void MissingSerializedBindingFailsClosedBeforeSourceCapture()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                refs.Handoff.enabled = false;
                var field = typeof(RegularEnemyEncounterHandoffAdapter).GetField("_player", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.That(field, Is.Not.Null);
                field.SetValue(refs.Handoff, null);
                AssertProductionFixedRejects(refs.Handoff);
                AssertPristine(refs.Handoff);
                AssertPairs(refs, false, false);
            }
            finally { Object.DestroyImmediate(root); }
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003.
        [UnityTest]
        public IEnumerator PresentationCollectionsAreNotCastMutable()
        {
            var root = InstantiateGraph();
            try
            {
                var refs = Capture(root);
                yield return new WaitForFixedUpdate();
                var view = View(refs.Handoff);
                Assert.Throws<NotSupportedException>(() => ((IList)view.AppliedHitTargetIds).Add("walker"));
                Assert.Throws<NotSupportedException>(() => ((IList)view.Defeats).Add(default(RegularEnemyDefeatedNotice)));
                Assert.That(View(refs.Handoff).AppliedHitTargetIds, Is.Empty);
            }
            finally { Object.Destroy(root); }
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-003. Every nested Transfer
        // attempt/result branch is an immutable preflight source. These pure
        // mutations invoke the adapter comparator directly, so rejection leaves
        // no adapter/session/view/pair state to mutate.
        [Test]
        public void TransferRemovalAndPressPublicationMutationsAreRejectedWithoutAdapterState()
        {
            var baseline = CompleteTransferPublication();
            var removal = baseline.Outcome.RemovalResult.Value;
            var press = baseline.Outcome.PressResult.Value;

            AssertTransferChanged(baseline, WithOutcome(baseline, null, press));
            AssertTransferChanged(baseline, WithOutcome(baseline, removal, null));
            AssertTransferChanged(baseline, WithOutcome(baseline,
                new TransferAttemptResult(removal.Tick, 1, removal.SelectedTargetId, removal.Success, removal.FailureReason,
                    removal.Session, removal.StateChanged, removal.Cleared), press));
            AssertTransferChanged(baseline, WithOutcome(baseline,
                new TransferAttemptResult(removal.Tick, removal.AimSampleId, "walker", removal.Success, removal.FailureReason,
                    removal.Session, removal.StateChanged, removal.Cleared), press));
            var changedRemovalSession = new TransferSessionSnapshot(removal.Session.Tick, "walker", removal.Session.ActiveTargetId,
                removal.Session.PlayerState, removal.Session.TransitionLockEndTick, removal.Session.LastFailureReason, removal.Session.Revision);
            AssertTransferChanged(baseline, WithOutcome(baseline,
                new TransferAttemptResult(removal.Tick, removal.AimSampleId, removal.SelectedTargetId, removal.Success, removal.FailureReason,
                    changedRemovalSession, removal.StateChanged, removal.Cleared), press));
            var changedClear = new TransferCleared(TransferClearReason.RoomLeaving, removal.Cleared.Value.PreviousTargetId,
                removal.Cleared.Value.Tick, removal.Cleared.Value.Revision);
            AssertTransferChanged(baseline, WithOutcome(baseline,
                new TransferAttemptResult(removal.Tick, removal.AimSampleId, removal.SelectedTargetId, removal.Success, removal.FailureReason,
                    removal.Session, removal.StateChanged, changedClear), press));

            AssertTransferChanged(baseline, WithOutcome(baseline, removal,
                new TransferAttemptResult(press.Tick, 2, press.SelectedTargetId, press.Success, press.FailureReason,
                    press.Session, press.StateChanged, press.Cleared)));
            AssertTransferChanged(baseline, WithOutcome(baseline, removal,
                new TransferAttemptResult(press.Tick, press.AimSampleId, "surveyor", press.Success, press.FailureReason,
                    press.Session, press.StateChanged, press.Cleared)));
            var changedPressSession = new TransferSessionSnapshot(press.Session.Tick, press.Session.HighlightedTargetId, press.Session.ActiveTargetId,
                press.Session.PlayerState, press.Session.TransitionLockEndTick + 1, press.Session.LastFailureReason, press.Session.Revision);
            AssertTransferChanged(baseline, WithOutcome(baseline, removal,
                new TransferAttemptResult(press.Tick, press.AimSampleId, press.SelectedTargetId, press.Success, press.FailureReason,
                    changedPressSession, press.StateChanged, press.Cleared)));
            var changedState = new TransferStateChanged(press.StateChanged.Value.TargetId, press.StateChanged.Value.PlayerModifierId,
                "Target.Box.Heavy.v1", press.StateChanged.Value.Tick, press.StateChanged.Value.Revision);
            AssertTransferChanged(baseline, WithOutcome(baseline, removal,
                new TransferAttemptResult(press.Tick, press.AimSampleId, press.SelectedTargetId, press.Success, press.FailureReason,
                    press.Session, changedState, press.Cleared)));
        }

        private static GameObject InstantiateGraph()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            Assert.That(asset, Is.Not.Null, "Run the approved B1 authoring builder before this final-graph test.");
            return Object.Instantiate(asset);
        }

        private static TransferPhasePublication CompleteTransferPublication()
        {
            var tick = new SimulationTick(10);
            var removalSession = new TransferSessionSnapshot(tick, string.Empty, string.Empty, TransferPlayerState.Baseline, -1, TransferFailureReason.None, 2);
            var cleared = new TransferCleared(TransferClearReason.TargetRemoved, "surveyor", tick, 2);
            var removal = new TransferAttemptResult(tick, null, string.Empty, true, TransferFailureReason.None, removalSession, null, cleared);
            var pressSession = new TransferSessionSnapshot(tick, "walker", "walker", TransferPlayerState.Lightweight, 31, TransferFailureReason.None, 3);
            var stateChanged = new TransferStateChanged("walker", "Player.Lightweight.v1", "Target.Enemy.Heavy.v1", tick, 3);
            var press = new TransferAttemptResult(tick, 7, "walker", true, TransferFailureReason.None, pressSession, stateChanged, null);
            var outcome = new TransferTickOutcome(removal, press, pressSession);
            var input = new TransferSimulationInput(tick, null, null, null, new[] { new TransferTargetRemoved("surveyor", tick) }, null);
            return new TransferPhasePublication(outcome, new TransferPhaseInputSummary(input));
        }

        private static TransferPhasePublication WithOutcome(TransferPhasePublication source, TransferAttemptResult? removal, TransferAttemptResult? press) =>
            new TransferPhasePublication(new TransferTickOutcome(removal, press, source.Outcome.Snapshot), source.Input);

        private static void AssertTransferChanged(TransferPhasePublication baseline, TransferPhasePublication mutated)
        {
            var comparator = typeof(RegularEnemyEncounterHandoffAdapter).GetMethod("SameTransfer", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(comparator, Is.Not.Null);
            Assert.That((bool)comparator.Invoke(null, new object[] { baseline, mutated }), Is.False);
        }

        private static GraphRefs Capture(GameObject root)
        {
            var player = Single<PlayerMovementController>(root);
            var combat = Single<CombatSimulationDriver>(root);
            var handoff = Single<RegularEnemyEncounterHandoffAdapter>(root);
            var threat = Single<RegularEnemyThreatSimulationDriver>(root);
            var playerCollider = player.GetComponent<CapsuleCollider2D>();
            TransferTarget surveyor = null, walker = null;
            foreach (var target in root.GetComponentsInChildren<TransferTarget>(true))
            { if (target.TargetId == "surveyor") surveyor = target; if (target.TargetId == "walker") walker = target; }
            Assert.That(playerCollider, Is.Not.Null); Assert.That(surveyor, Is.Not.Null); Assert.That(walker, Is.Not.Null);
            return new GraphRefs(player, combat, threat, handoff, playerCollider, surveyor.GetComponent<BoxCollider2D>(), walker.GetComponent<BoxCollider2D>());
        }

        private static void SubmitDamage(GraphRefs refs, string targetId, int amount)
        {
            var tick = refs.Player.NextExpectedTick;
            refs.Combat.SubmitForTests(new CombatSimulationInput(tick, null, null, null,
                new[] { Damage("handoff/" + targetId + "/" + tick.Value, targetId, amount, tick) }, false));
        }

        private static DamageRequest Damage(string requestId, string targetId, int amount, SimulationTick tick) =>
            new DamageRequest(requestId, "player", targetId, amount, DamageKind.HeavyImpact, tick);

        private static bool ContainsKind(CombatSimulationOutcome outcome, DamageKind kind)
        {
            for (var index = 0; index < outcome.CanonicalRequestTrace.Count; index++)
                if (outcome.CanonicalRequestTrace[index].Kind == kind) return true;
            return false;
        }

        private static RegularEnemyEncounterPresentationView View(RegularEnemyEncounterHandoffAdapter handoff)
        { Assert.That(handoff.LatestPresentationView.HasValue, Is.True); return handoff.LatestPresentationView.Value; }

        private static void AssertPristine(RegularEnemyEncounterHandoffAdapter handoff)
        {
            Assert.That(handoff.HasOwnedLifecycleForEncounterHandoff, Is.False); Assert.That(handoff.HasRetainedColliderStateForEncounterHandoff, Is.False);
            Assert.That(handoff.LatestPresentationView.HasValue, Is.False); Assert.That(handoff.SurveyorPairIgnoredForEncounterHandoff, Is.False);
            Assert.That(handoff.WalkerPairIgnoredForEncounterHandoff, Is.False);
        }

        private static void AssertPairs(GraphRefs refs, bool surveyor, bool walker)
        {
            Assert.That(refs.Handoff.SurveyorPairIgnoredForEncounterHandoff, Is.EqualTo(surveyor)); Assert.That(refs.Handoff.WalkerPairIgnoredForEncounterHandoff, Is.EqualTo(walker));
            Assert.That(Physics2D.GetIgnoreCollision(refs.PlayerCollider, refs.SurveyorCollider), Is.EqualTo(surveyor));
            Assert.That(Physics2D.GetIgnoreCollision(refs.PlayerCollider, refs.WalkerCollider), Is.EqualTo(walker));
        }

        private static void AssertProductionFixedRejects(RegularEnemyEncounterHandoffAdapter handoff)
        {
            var fixedUpdate = typeof(RegularEnemyEncounterHandoffAdapter).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(fixedUpdate, Is.Not.Null);
            var exception = Assert.Throws<TargetInvocationException>(() => fixedUpdate.Invoke(handoff, null));
            Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
        }

        private static T Single<T>(GameObject root) where T : Component
        { var values = root.GetComponentsInChildren<T>(true); Assert.That(values.Length, Is.EqualTo(1)); return values[0]; }

        private readonly struct GraphRefs
        {
            internal GraphRefs(PlayerMovementController player, CombatSimulationDriver combat, RegularEnemyThreatSimulationDriver threat, RegularEnemyEncounterHandoffAdapter handoff,
                CapsuleCollider2D playerCollider, BoxCollider2D surveyorCollider, BoxCollider2D walkerCollider)
            { Player = player; Combat = combat; Threat = threat; Handoff = handoff; PlayerCollider = playerCollider; SurveyorCollider = surveyorCollider; WalkerCollider = walkerCollider; }
            internal PlayerMovementController Player { get; } internal CombatSimulationDriver Combat { get; } internal RegularEnemyThreatSimulationDriver Threat { get; } internal RegularEnemyEncounterHandoffAdapter Handoff { get; }
            internal CapsuleCollider2D PlayerCollider { get; } internal BoxCollider2D SurveyorCollider { get; } internal BoxCollider2D WalkerCollider { get; }
        }
    }
}
#endif
