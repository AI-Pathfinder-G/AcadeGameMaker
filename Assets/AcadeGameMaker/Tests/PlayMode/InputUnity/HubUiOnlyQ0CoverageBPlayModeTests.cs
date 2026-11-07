using System;
using System.Collections;
using System.Reflection;
using AcadeGameMaker.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Input.Unity.PlayMode.Tests
{
    // REQ-M5D7Q0-003/005/008 / AC-M5D7Q0-007,009.  This is the independent
    // Group B matrix.  It intentionally uses the real adapter-owned prepared
    // lifecycle and only reflects corruptions that production code must detect.
    internal sealed class HubUiOnlyQ0CoverageBPlayModeTests
    {
        [TestCase("hub-flag", InputRouterFaultStage.HubUiOnly)]
        [TestCase("gameplay-map", InputRouterFaultStage.HubUiOnly)]
        [TestCase("ui-map", InputRouterFaultStage.HubUiOnly)]
        [TestCase("requester", InputRouterFaultStage.HubUiOnly)]
        [TestCase("pending-request", InputRouterFaultStage.HubUiOnly)]
        [TestCase("receipt", InputRouterFaultStage.UiCapture)]
        [TestCase("frame", InputRouterFaultStage.UiCapture)]
        [TestCase("frame-proof", InputRouterFaultStage.UiCapture)]
        [TestCase("receipt-proof", InputRouterFaultStage.UiCapture)]
        [TestCase("next-tick", InputRouterFaultStage.HubUiOnly)]
        [TestCase("next-ordinal", InputRouterFaultStage.HubUiOnly)]
        [TestCase("owner", InputRouterFaultStage.HubUiOnly)]
        [TestCase("actions", InputRouterFaultStage.HubUiOnly)]
        [TestCase("callbacks", InputRouterFaultStage.HubUiOnly)]
        public void AC007_EachIndependentRuntimeCorruptionRetainsForensicsAndPendingBatch(
            string mutation, InputRouterFaultStage expectedFault)
        {
            using (var context = HubRuntimeContext.Create())
            {
                StartAndPublish(context);
                var forensicReceipt = Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt");
                var forensicFrame = Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame");
                var forensicReceiptProof = Get<InputFrameCommitReceipt?>(context.Router, "_uiPublicationProof");
                var forensicFrameProof = Get<UiSemanticFrameV1?>(context.Router, "_uiFramePublicationProof");
                var actions = Get<GameInputActions>(context.Router, "_actions");
                StageFullPendingUiBatch(context.Router);
                var pendingBatch = CapturePendingUiBatch(context.Router);
                ApplyMutation(context.Router, actions, mutation);
                var containedActions = Get<GameInputActions>(context.Router, "_actions");

                // Use the public source getter as the next invariant boundary.
                // This distinguishes hub-graph faults from UiCapture proof faults
                // before StepForTests reaches its broader prepared-state guard.
                Assert.That(() => { var ignored = context.Router.CurrentReceipt; }, Throws.InvalidOperationException, mutation);
                AssertTerminalCorruption(context.Router, actions, forensicReceipt, forensicFrame, expectedFault, mutation);
                AssertRawPublicationBackingWasNotNormalized(context.Router, mutation, forensicReceipt, forensicFrame, forensicReceiptProof, forensicFrameProof);
                CollectionAssert.AreEqual(pendingBatch, CapturePendingUiBatch(context.Router), mutation + " must not consume the full pending batch");
                Assert.That(() => { var ignored = context.Router.CurrentReceipt; }, Throws.InvalidOperationException, mutation + " cannot retry");
                // A substituted live reference is caller-owned.  Do not dispose
                // the adopted proof manually: normal Hub teardown proves its
                // exact close.  Only the foreign caller-owned wrapper is cleaned
                // after all containment assertions have run.
                if (!ReferenceEquals(actions, containedActions))
                {
                    Assert.That(containedActions.asset, Is.Not.Null, mutation + " foreign wrapper remains undisposed");
                    containedActions.Dispose();
                }
                // AC-M5D7Q0-007. The deliberately corrupted owner remains
                // forensic state, so DestroyImmediate drives both OnDisable
                // and OnDestroy through the exact lifecycle validation error.
                if (mutation == "owner")
                {
                    LogAssert.Expect(LogType.Exception, "InvalidOperationException: Initialized launch state is malformed.");
                    LogAssert.Expect(LogType.Exception, "InvalidOperationException: Initialized launch state is malformed.");
                }
            }
        }

        [TestCase("tick-overflow", InputRouterFaultStage.HubUiOnly)]
        [TestCase("ordinal-overflow", InputRouterFaultStage.HubUiOnly)]
        public void AC007_CheckedHubSuccessorOverflowRetainsForensicsAndPendingBatch(string overflow, InputRouterFaultStage expectedFault)
        {
            using (var context = HubRuntimeContext.Create())
            {
                StartAndPublish(context);
                var forensicReceipt = Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt");
                var forensicFrame = Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame");
                var actions = Get<GameInputActions>(context.Router, "_actions");
                StageFullPendingUiBatch(context.Router);
                var pendingBatch = CapturePendingUiBatch(context.Router);
                if (overflow == "tick-overflow")
                {
                    var max = new SimulationTick(int.MaxValue);
                    Set(context.Router, "_hubNextTick", max);
                    Set(context.Router, "_hubNextTickProof", max);
                }
                else
                {
                    Set(context.Router, "_frameOrdinal", long.MaxValue);
                    Set(context.Router, "_hubFrameOrdinalProof", long.MaxValue);
                }

                Assert.That(() => context.Router.StepForTests(), Throws.InvalidOperationException, overflow);
                AssertTerminalCorruption(context.Router, actions, forensicReceipt, forensicFrame, expectedFault, overflow);
                CollectionAssert.AreEqual(pendingBatch, CapturePendingUiBatch(context.Router), overflow + " must not consume the full pending batch");
            }
        }

        [TestCase("player", "AcadeGameMaker.Movement.PlayerMovementController, AcadeGameMaker.Movement.Unity")]
        [TestCase("transfer", "AcadeGameMaker.Transfer.Unity.TransferSimulationDriver, AcadeGameMaker.Transfer.Unity")]
        [TestCase("combat", "AcadeGameMaker.Combat.Unity.OrdanBossCombatSimulationDriver, AcadeGameMaker.Combat.Unity")]
        [TestCase("terminal", "AcadeGameMaker.Combat.Unity.OrdanBossTerminalTeardown, AcadeGameMaker.Combat.Unity")]
        [TestCase("camera", null)]
        public void AC007_EachFullHubHybridReferenceFaultsWithoutRelabelingPendingBatch(string reference, string typeName)
        {
            using (var context = HubRuntimeContext.Create())
            {
                GameObject foreign = null;
                try
                {
                    StartAndPublish(context);
                    var forensicReceipt = Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt");
                    var forensicFrame = Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame");
                    var actions = Get<GameInputActions>(context.Router, "_actions");
                    StageFullPendingUiBatch(context.Router);
                    var pendingBatch = CapturePendingUiBatch(context.Router);
                    foreign = new GameObject("m5d7q0-hybrid-" + reference);
                    foreign.SetActive(false);
                    // PlayerMovementController requires the abstract Collider2D
                    // base type; provide one concrete fixture collider before
                    // adding it so this test reaches the router hybrid guard.
                    if (reference == "player") foreign.AddComponent<BoxCollider2D>();
                    var component = typeName == null ? (Component)foreign.AddComponent<HubUiOnlyQ0CoverageBMarker>() : foreign.AddComponent(Type.GetType(typeName, true));
                    Set(context.Router, HybridField(reference), component);

                    Assert.That(() => context.Router.StepForTests(), Throws.InvalidOperationException, reference);
                    AssertTerminalCorruption(context.Router, actions, forensicReceipt, forensicFrame, InputRouterFaultStage.HubUiOnly, reference);
                    CollectionAssert.AreEqual(pendingBatch, CapturePendingUiBatch(context.Router), reference + " must not relabel or consume the full pending batch");
                }
                finally { if (foreign != null) UnityEngine.Object.DestroyImmediate(foreign); }
            }
        }

        [TestCase(true, false)]
        [TestCase(true, true)]
        [TestCase(false, false)]
        [TestCase(false, true)]
        public void AC009_ActualControlledAwakeAndEveryLegalStartPermutationPublishesOnce(
            bool routerComponentAddedFirst, bool latchStartsBeforeRouter)
        {
            using (var context = ControlledLifecycleContext.Create(routerComponentAddedFirst))
            {
                // Awake is run by Unity activation, in the component order selected
                // above; no string-labelled pseudo-order is used.
                Assert.That(context.Router.PreparedLaunchAwakeEntered, Is.True);
                Assert.That(Get<bool>(context.Adapter, "_awakeAttempted"), Is.True);
                Assert.That(Get<GameInputActions>(context.Router, "_actions"), Is.Not.Null);
                Assert.That(context.Router.CurrentReceipt, Is.Null);

                Invoke(context.Adapter, "Start");
                if (latchStartsBeforeRouter) Invoke(context.Latch, "Start");
                Invoke(context.Router, "Start");
                if (!latchStartsBeforeRouter) Invoke(context.Latch, "Start");
                context.Router.StepForTests();

                Assert.That(context.Router.CurrentReceipt.Value.Tick.Value, Is.Zero);
                Assert.That(context.Router.CurrentReceipt.Value.FrameOrdinal, Is.Zero);
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.Zero);
            }
        }

        [UnityTest]
        public IEnumerator AC009_ActualDisableDestroyBeforePublicationClosesAdoptedOwnerOnce()
        {
            using (var context = ControlledLifecycleContext.Create(true))
            {
                var actions = Get<GameInputActions>(context.Router, "_actions");
                context.Host.SetActive(false); // actual OnDisable on the cohort
                Assert.That(context.Adapter.CurrentReceipt, Is.Null);
                Assert.That(context.Router.IsFaulted, Is.True);
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                Assert.That(Get<GameInputActions>(context.Router, "_actions"), Is.Null);
                UnityEngine.Object.DestroyImmediate(context.Host); // actual OnDestroy, no double close
                context.MarkDestroyed();
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                // GameInputActions.Dispose schedules destruction of its asset.
                // Observe only after Unity processes the scheduled destroy.
                yield return null;
                Assert.That(actions.asset == null, Is.True);
            }
        }

        [UnityTest]
        public IEnumerator AC009_ActualDisableDestroyAfterReceiptAndFreshSessionDoNotCarryState()
        {
            using (var first = ControlledLifecycleContext.Create(false))
            {
                Invoke(first.Adapter, "Start");
                first.Router.StepForTests();
                var actions = Get<GameInputActions>(first.Router, "_actions");
                first.Host.SetActive(false);
                UnityEngine.Object.DestroyImmediate(first.Host);
                first.MarkDestroyed();
                Assert.That(Get<int>(first.Router, "_closeAttempts"), Is.EqualTo(1));
                // GameInputActions.Dispose is deferred by Unity in PlayMode.
                yield return null;
                Assert.That(actions.asset == null, Is.True);
            }
            using (var fresh = ControlledLifecycleContext.Create(true))
            {
                Invoke(fresh.Adapter, "Start");
                fresh.Router.StepForTests();
                Assert.That(fresh.Router.CurrentReceipt.Value.Tick.Value, Is.Zero);
                Assert.That(fresh.Router.CurrentReceipt.Value.FrameOrdinal, Is.Zero);
                Assert.That(Get<long>(fresh.Router, "_modeEpoch"), Is.Zero);
                Assert.That(Get<GameInputActions>(fresh.Router, "_actions"), Is.Not.Null);
            }
        }

        private static void StartAndPublish(HubRuntimeContext context)
        {
            HubRuntimeContext.Invoke(context.Adapter, "Start");
            context.Router.StepForTests();
        }

        private static void ApplyMutation(InputRouter router, GameInputActions actions, string mutation)
        {
            if (mutation == "hub-flag") { Set(router, "_authoredHubUiOnly", false); return; }
            if (mutation == "gameplay-map") { actions.Gameplay.Get().Enable(); return; }
            if (mutation == "ui-map") { actions.UI.Get().Disable(); return; }
            if (mutation == "requester") { Set(router, "_modeRequester", new object()); return; }
            if (mutation == "pending-request")
            {
                var type = typeof(InputRouter).GetNestedType("PendingModeRequest", BindingFlags.NonPublic);
                var value = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null,
                    new object[] { InputMode.UIOnly, "corrupt", new SimulationTick(0) }, null);
                Set(router, "_pendingModeRequest", value);
                return;
            }
            var receipt = Get<InputFrameCommitReceipt?>(router, "_currentReceipt").Value;
            if (mutation == "receipt") { Set(router, "_currentReceipt", new InputFrameCommitReceipt(new SimulationTick(receipt.Tick.Value + 1), receipt.FrameOrdinal, 0, InputMode.UIOnly)); return; }
            if (mutation == "frame") { Set(router, "_currentUiFrame", UiSemanticFrameV1.Empty(new InputFrameCommitReceipt(receipt.Tick, receipt.FrameOrdinal + 1, 0, InputMode.UIOnly))); return; }
            if (mutation == "frame-proof") { Set(router, "_uiFramePublicationProof", UiSemanticFrameV1.Empty(new InputFrameCommitReceipt(receipt.Tick, receipt.FrameOrdinal + 1, 0, InputMode.UIOnly))); return; }
            if (mutation == "receipt-proof") { Set(router, "_uiPublicationProof", new InputFrameCommitReceipt(new SimulationTick(receipt.Tick.Value + 1), receipt.FrameOrdinal, 0, InputMode.UIOnly)); return; }
            if (mutation == "next-tick") { Set(router, "_hubNextTick", new SimulationTick(99)); return; }
            if (mutation == "next-ordinal") { Set(router, "_frameOrdinal", 99L); return; }
            if (mutation == "owner") { Set(router, "_preparedLaunchOwner", new object()); return; }
            if (mutation == "actions") { Set(router, "_actions", new GameInputActions()); return; }
            if (mutation == "callbacks") { Set(router, "_uiCallbacksRegistered", false); return; }
            throw new ArgumentOutOfRangeException(nameof(mutation));
        }

        private static string HybridField(string reference)
        {
            if (reference == "player") return "_player";
            if (reference == "transfer") return "_transfer";
            if (reference == "combat") return "_combat";
            if (reference == "terminal") return "_terminalTeardown";
            if (reference == "camera") return "_cameraProviderBehaviour";
            throw new ArgumentOutOfRangeException(nameof(reference));
        }

        private static void AssertTerminalCorruption(InputRouter router, GameInputActions actions,
            InputFrameCommitReceipt? receipt, UiSemanticFrameV1? frame, InputRouterFaultStage expectedFault, string label)
        {
            Assert.That(router.IsFaulted, Is.True, label);
            Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(expectedFault), label + " fault stage");
            Assert.That(actions.Gameplay.Get().enabled, Is.False, label + " gameplay map");
            Assert.That(actions.UI.Get().enabled, Is.False, label + " UI map");
            if (label != "receipt") Assert.That(Get<InputFrameCommitReceipt?>(router, "_currentReceipt"), Is.EqualTo(receipt), label + " receipt forensic value");
            if (label != "frame") Assert.That(Get<UiSemanticFrameV1?>(router, "_currentUiFrame"), Is.EqualTo(frame), label + " frame forensic value");
        }

        private static void AssertRawPublicationBackingWasNotNormalized(InputRouter router, string mutation,
            InputFrameCommitReceipt? receipt, UiSemanticFrameV1? frame, InputFrameCommitReceipt? receiptProof, UiSemanticFrameV1? frameProof)
        {
            if (mutation == "receipt")
            {
                Assert.That(Get<InputFrameCommitReceipt?>(router, "_currentReceipt"), Is.Not.EqualTo(receipt), "corrupt raw receipt remains forensic");
                Assert.That(Get<InputFrameCommitReceipt?>(router, "_uiPublicationProof"), Is.EqualTo(receiptProof), "independent receipt proof remains prior valid");
                Assert.That(Get<UiSemanticFrameV1?>(router, "_currentUiFrame"), Is.EqualTo(frame), "frame backing is not normalized");
                Assert.That(Get<UiSemanticFrameV1?>(router, "_uiFramePublicationProof"), Is.EqualTo(frameProof), "independent frame proof remains prior valid");
            }
            else if (mutation == "frame")
            {
                Assert.That(Get<UiSemanticFrameV1?>(router, "_currentUiFrame"), Is.Not.EqualTo(frame), "corrupt raw frame remains forensic");
                Assert.That(Get<UiSemanticFrameV1?>(router, "_uiFramePublicationProof"), Is.EqualTo(frameProof), "independent frame proof remains prior valid");
                Assert.That(Get<InputFrameCommitReceipt?>(router, "_currentReceipt"), Is.EqualTo(receipt), "receipt backing is not normalized");
                Assert.That(Get<InputFrameCommitReceipt?>(router, "_uiPublicationProof"), Is.EqualTo(receiptProof), "independent receipt proof remains prior valid");
            }
        }

        private static void StageFullPendingUiBatch(InputRouter router)
        {
            Set(router, "_uiNavigateXQ4096", 4096); Set(router, "_uiNavigateYQ4096", -4096); Set(router, "_uiNavigateChanged", true);
            Set(router, "_uiHasPoint", true); Set(router, "_uiPointXActualPx", 321); Set(router, "_uiPointYActualPx", 654); Set(router, "_uiPointChanged", true);
            Set(router, "_uiClickPressed", true); Set(router, "_uiHasClickPoint", true); Set(router, "_uiClickPointXActualPx", 321); Set(router, "_uiClickPointYActualPx", 654);
            Set(router, "_uiScrollXQ4096", 15L); Set(router, "_uiScrollYQ4096", -25L); Set(router, "_uiSubmitPressed", true); Set(router, "_uiCancelPressed", true); Set(router, "_callbackOrdinal", 17L);
        }

        private static object[] CapturePendingUiBatch(InputRouter router) => new object[]
        {
            Get<int>(router, "_uiNavigateXQ4096"), Get<int>(router, "_uiNavigateYQ4096"), Get<bool>(router, "_uiNavigateChanged"), Get<bool>(router, "_uiHasPoint"),
            Get<int>(router, "_uiPointXActualPx"), Get<int>(router, "_uiPointYActualPx"), Get<bool>(router, "_uiPointChanged"), Get<bool>(router, "_uiClickPressed"),
            Get<bool>(router, "_uiHasClickPoint"), Get<int>(router, "_uiClickPointXActualPx"), Get<int>(router, "_uiClickPointYActualPx"), Get<long>(router, "_uiScrollXQ4096"),
            Get<long>(router, "_uiScrollYQ4096"), Get<bool>(router, "_uiSubmitPressed"), Get<bool>(router, "_uiCancelPressed"), Get<long>(router, "_callbackOrdinal")
        };

        private static T Get<T>(object target, string field) => (T)target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
        private static void Set(object target, string field, object value) => target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
        private static void Invoke(object target, string method) => target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
    }

    internal sealed class HubUiOnlyQ0CoverageBMarker : MonoBehaviour { }

    internal sealed class ControlledLifecycleContext : IDisposable
    {
        private readonly string _root;
        private bool _destroyed;
        internal GameObject Host { get; }
        internal InputRouter Router { get; }
        internal DesktopProfileLaunchAdapterV1 Adapter { get; }
        internal HubEntryHandoffLatchV1 Latch { get; }

        private ControlledLifecycleContext(GameObject host, string root, InputRouter router, DesktopProfileLaunchAdapterV1 adapter, HubEntryHandoffLatchV1 latch)
        { Host = host; _root = root; Router = router; Adapter = adapter; Latch = latch; }

        internal static ControlledLifecycleContext Create(bool routerComponentAddedFirst)
        {
            var root = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "m5d7q0-coverage-b-" + Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(root);
            var host = new GameObject("m5d7q0-coverage-b-lifecycle");
            host.SetActive(false);
            InputRouter router;
            DesktopProfileLaunchAdapterV1 adapter;
            if (routerComponentAddedFirst)
            {
                router = host.AddComponent<InputRouter>();
                adapter = host.AddComponent<DesktopProfileLaunchAdapterV1>();
            }
            else
            {
                adapter = host.AddComponent<DesktopProfileLaunchAdapterV1>();
                router = host.AddComponent<InputRouter>();
            }
            var latch = host.AddComponent<HubEntryHandoffLatchV1>();
            router.ConfigureHubUiOnlyForAuthoring();
            adapter.ConfigureHubUiOnlyForAuthoring(router);
            adapter.ConfigureForTests(router, new Environment(root), new DesktopProfileLaunchPreparationPortV1());
            latch.ConfigureHubUiOnlyForAuthoring(adapter, router);
            host.SetActive(true);
            return new ControlledLifecycleContext(host, root, router, adapter, latch);
        }

        internal void MarkDestroyed() { _destroyed = true; }
        public void Dispose()
        {
            if (!_destroyed) UnityEngine.Object.DestroyImmediate(Host);
            if (System.IO.Directory.Exists(_root)) System.IO.Directory.Delete(_root, true);
        }

        private sealed class Environment : IDesktopProfileLaunchEnvironmentV1
        {
            private readonly string _root;
            internal Environment(string root) { _root = root; }
            public string GetPersistentDataPath() => _root;
            public DateTimeOffset GetUtcNow() => new DateTimeOffset(2026, 9, 20, 0, 0, 0, TimeSpan.Zero);
        }
    }
}
