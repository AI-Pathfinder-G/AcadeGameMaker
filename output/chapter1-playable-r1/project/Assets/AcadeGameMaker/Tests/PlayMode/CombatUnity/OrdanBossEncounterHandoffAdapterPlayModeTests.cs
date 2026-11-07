// AC-COM-002, AC-COM-003, AC-COM-004, AC-WT-005.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class OrdanBossEncounterHandoffAdapterPlayModeTests
    {
        [UnityEngine.SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _runtimeFixture=new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnityEngine.TestTools.UnitySetUp] public System.Collections.IEnumerator BeginRuntimeCase(){_runtimeFixture.PrepareEnter();yield return new UnityEngine.TestTools.EnterPlayMode();_runtimeFixture.ConfirmEntered();yield return _runtimeFixture.RunBody(_runtimeFixture.BeginCase(false));}
        [UnityEngine.TestTools.UnityTearDown] public System.Collections.IEnumerator CloseRuntimeCase(){try{if(_runtimeFixture.OwnsRuntimeFixture && UnityEngine.Application.isPlaying)yield return _runtimeFixture.RunBody(_runtimeFixture.CloseCase(),true);}finally{}if(_runtimeFixture.OwnsRuntimeFixture){if(UnityEngine.Application.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();_runtimeFixture.CaptureCleanupFailure(_runtimeFixture.CompleteExit);}_runtimeFixture.ThrowCleanupFailure();}
        

        private const string GraphPath = "Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab";

        [UnityTest]
        public IEnumerator PublishedViewIsImmutableAndLaterTicksKeepAuditDormant()
        {yield return _runtimeFixture.RunBody(PublishedViewIsImmutableAndLaterTicksKeepAuditDormantOriginalBody());}
        private IEnumerator PublishedViewIsImmutableAndLaterTicksKeepAuditDormantOriginalBody(){
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            Assert.That(asset, Is.Not.Null);
            var root = UnityEngine.Object.Instantiate(asset);
            try
            {
                yield return new WaitForFixedUpdate(); var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                var first = handoff.LatestPresentationView.Value;
                Assert.Throws<NotSupportedException>(() => ((System.Collections.IList)first.StableIds).Add("foreign"));
                Assert.Throws<NotSupportedException>(() => ((System.Collections.IList)first.Intents).Add(default(OrdanBossIntent)));
                Assert.Throws<NotSupportedException>(() => ((System.Collections.IList)first.Handoffs).Add(default(OrdanBossHandoff)));
                Assert.Throws<NotSupportedException>(() => ((System.Collections.IList)first.Combatants).Add(default(CombatantSnapshot)));
                Assert.Throws<NotSupportedException>(() => ((System.Collections.IList)first.PayloadAvailable).Add(true));
                Assert.Throws<NotSupportedException>(() => ((System.Collections.IList)first.AuditTemporaryEndedTargetIds).Add("forged"));
                Assert.Throws<NotSupportedException>(() => ((System.Collections.IList)first.AuditPermanentlyRemovedTargetIds).Add("forged"));
                yield return new WaitForFixedUpdate(); var second = handoff.LatestPresentationView.Value;
                Assert.That(second.Tick.Value, Is.GreaterThan(first.Tick.Value)); Assert.That(second.AuditAvailable, Is.False); Assert.That(second.AuditApplied, Is.False);
            }
            finally { UnityEngine.Object.Destroy(root); }
        }

        // AC-COM-003, AC-COM-004. Cross-wire, foreign-root, duplicate,
        // skipped, and failed-first +110 publications all fail atomically.
        [UnityTest]
        public IEnumerator InvalidHandoffPublicationsAreAtomic()
        {yield return _runtimeFixture.RunBody(InvalidHandoffPublicationsAreAtomicOriginalBody());}
        private IEnumerator InvalidHandoffPublicationsAreAtomicOriginalBody(){
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            Assert.That(asset, Is.Not.Null);
            GameObject root = null, firstFailureRoot = null, foreignRoot = null, duplicateRoot = null, skippedRoot = null;
            try
            {
                root = UnityEngine.Object.Instantiate(asset);
                firstFailureRoot = UnityEngine.Object.Instantiate(asset);
                foreignRoot = UnityEngine.Object.Instantiate(asset);
                duplicateRoot = UnityEngine.Object.Instantiate(asset);
                skippedRoot = UnityEngine.Object.Instantiate(asset);
                var first = firstFailureRoot.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                first.enabled = false;
                SetPrivate(first, "_bridge", null);
                var foreign = foreignRoot.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                foreign.enabled = false;
                ((List<TransferTarget>)GetPrivate(foreign, "_payloadTargets"))[0] = PayloadTarget(root, "BossWeight-0");
                yield return new WaitForFixedUpdate();
                var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                var published = handoff.LatestPresentationView.Value;
                handoff.enabled = false;
                SetPrivate(handoff, "_bridge", null);
                Assert.Throws<TargetInvocationException>(() => InvokePrivate(handoff, "FixedUpdate"));
                Assert.That(handoff.LatestPresentationView.Value.Tick, Is.EqualTo(published.Tick));

                Assert.That(first.LatestPresentationView.HasValue, Is.False);
                Assert.Throws<TargetInvocationException>(() => InvokePrivate(first, "FixedUpdate"));
                Assert.That(first.LatestPresentationView.HasValue, Is.False);
                Assert.That(foreign.LatestPresentationView.HasValue, Is.False);
                Assert.Throws<TargetInvocationException>(() => InvokePrivate(foreign, "FixedUpdate"));
                Assert.That(foreign.LatestPresentationView.HasValue, Is.False);

                yield return new WaitForFixedUpdate();
                var duplicate = duplicateRoot.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                var duplicatePublished = duplicate.LatestPresentationView.Value;
                duplicate.enabled = false;
                Assert.Throws<TargetInvocationException>(() => InvokePrivate(duplicate, "FixedUpdate"));
                Assert.That(duplicate.LatestPresentationView.Value.Tick, Is.EqualTo(duplicatePublished.Tick));

                yield return new WaitForFixedUpdate();
                var skipped = skippedRoot.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                var skippedPublished = skipped.LatestPresentationView.Value;
                skipped.enabled = false;
                yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
                Assert.Throws<TargetInvocationException>(() => InvokePrivate(skipped, "FixedUpdate"));
                Assert.That(skipped.LatestPresentationView.Value.Tick, Is.EqualTo(skippedPublished.Tick));

            }
            finally
            {
                if (root != null) { root.SetActive(false); UnityEngine.Object.DestroyImmediate(root); }
                if (firstFailureRoot != null) { firstFailureRoot.SetActive(false); UnityEngine.Object.DestroyImmediate(firstFailureRoot); }
                if (foreignRoot != null) { foreignRoot.SetActive(false); UnityEngine.Object.DestroyImmediate(foreignRoot); }
                if (duplicateRoot != null) { duplicateRoot.SetActive(false); UnityEngine.Object.DestroyImmediate(duplicateRoot); }
                if (skippedRoot != null) { skippedRoot.SetActive(false); UnityEngine.Object.DestroyImmediate(skippedRoot); }
            }
        }

        // AC-COM-003, AC-COM-004, AC-WT-005. The defeat tick has exactly the
        // ordered terminal triplet and all three permanent payload removals.
        [UnityTest]
        public IEnumerator BossDeathPublishesExactTerminalTripletAndPayloadRemovals()
        {yield return _runtimeFixture.RunBody(BossDeathPublishesExactTerminalTripletAndPayloadRemovalsOriginalBody());}
        private IEnumerator BossDeathPublishesExactTerminalTripletAndPayloadRemovalsOriginalBody(){
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            Assert.That(asset, Is.Not.Null);
            var root = UnityEngine.Object.Instantiate(asset);
            try
            {
                yield return new WaitForFixedUpdate();
                var combat = root.GetComponentInChildren<OrdanBossCombatSimulationDriver>();
                var handoff = root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>();
                var scheduler = root.GetComponentInChildren<OrdanBossTransferExposureScheduler>();
                var tick = combat.BoundPlayer.NextExpectedTick;
                combat.Submit(new OrdanBossCombatInput(tick, null, null, null, new[]
                {
                    new DamageRequest("ordan-lethal", "test", "ordan", 60, DamageKind.BossPattern, tick)
                }));
                yield return new WaitForFixedUpdate();
                var view = handoff.LatestPresentationView.Value;
                Assert.That(view.IsTerminal, Is.True); Assert.That(view.PayloadAvailable, Is.EqualTo(new[] { false, false, false }));
                Assert.That(view.Handoffs, Has.Count.EqualTo(3));
                Assert.That(view.Handoffs[0].Kind, Is.EqualTo(OrdanBossHandoffKind.Defeated));
                Assert.That(view.Handoffs[1].Kind, Is.EqualTo(OrdanBossHandoffKind.RewardRequest));
                Assert.That(view.Handoffs[2].Kind, Is.EqualTo(OrdanBossHandoffKind.RoomCompletionRequest));
                Assert.That(view.Handoffs[0].Tick, Is.EqualTo(tick)); Assert.That(view.Handoffs[1].Tick, Is.EqualTo(tick)); Assert.That(view.Handoffs[2].Tick, Is.EqualTo(tick));
                Assert.That(scheduler.LatestOutcome.PermanentlyRemovedTargetIds, Is.EqualTo(new[] { "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
                Assert.That(scheduler.LatestOutcome.TemporarilyEndedTargetIds, Is.Empty);
                Assert.That(view.AuditPermanentlyRemovedTargetIds, Is.EqualTo(new[] { "BossAuditBox-0" }));
                Assert.That(view.AuditTemporaryEndedTargetIds, Is.Empty);
                yield return new WaitForFixedUpdate();
                var transfer = root.GetComponentInChildren<TransferSimulationDriver>();
                var teardown = root.GetComponentInChildren<OrdanBossTerminalTeardown>();
                Assert.That(teardown, Is.Not.Null);
                Assert.That(transfer.LatestCompletedInputRemovalIds, Is.EqualTo(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
                Assert.That(teardown.LatestReceipt.HasValue, Is.True);
                Assert.That(teardown.LatestReceipt.Value.DeathSourceTick, Is.EqualTo(tick));
                Assert.That(teardown.LatestReceipt.Value.CleanupTick, Is.EqualTo(new SimulationTick(tick.Value + 1)));
                Assert.That(teardown.LatestReceipt.Value.DisabledComponents,
                    Is.EqualTo(new[] { "combat", "bridge", "scheduler", "hostile", "pull", "handoff", "ordan" }));
                Assert.That(teardown.LatestReceipt.Value.DisabledColliders,
                    Is.EqualTo(new[] { "ordan", "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }));
                Assert.Throws<InvalidOperationException>(() => scheduler.AdvanceSchedulerPhaseForTests());
                handoff.enabled = false;
                Assert.Throws<TargetInvocationException>(() => InvokePrivate(handoff, "FixedUpdate"));
                var preserved = handoff.LatestPresentationView.Value;
                Assert.That(preserved.Tick, Is.EqualTo(view.Tick));
                Assert.That(preserved.Handoffs, Is.EqualTo(view.Handoffs));
            }
            finally { UnityEngine.Object.Destroy(root); }
        }

        private static void SetPrivate(object target, string field, object value) =>
            target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
        private static object GetPrivate(object target, string field) =>
            target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
        private static void InvokePrivate(object target, string method) =>
            target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
        private static TransferTarget PayloadTarget(GameObject root, string targetId)
        {
            foreach (var target in root.GetComponentsInChildren<TransferTarget>(true)) if (target.TargetId == targetId) return target;
            Assert.Fail("Missing required payload target: " + targetId); return null;
        }
    }
}
