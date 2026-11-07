using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AcadeGameMaker.Input.Unity.PlayMode.Tests
{
    // REQ-M5D7Q0-001..008 / AC-M5D7Q0-001,003,004,005,009,012.  This is an
    // additive Q0-only receipt/lifecycle matrix; M5D7M/N fixtures stay intact.
    internal sealed class HubUiOnlyQ0RemainingRuntimePlayModeTests : InputTestFixture
    {
        [Test]
        public void AC001_MissingDefaultFalseHubFlagsRetainTheLegacyFullSimulationInitializationAndCommit()
        {
            var objects = new List<GameObject>();
            try
            {
                // A fresh component is the missing-serialized-field equivalent:
                // Unity supplies the declared false default before any authoring
                // seam is applied.  Exercise the historical full graph through
                // its real initialization and first coordinated commit.
                var rig = global::AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests.CreateRig(objects);
                var router = global::AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests.CreateRouter(
                    objects, rig, InputMode.GameplayEnabled);
                Assert.That(HubRuntimeContext.Get<bool>(router, "_authoredHubUiOnly"), Is.False);
                Assert.That(HubRuntimeContext.Get<bool>(router, "_hubUiOnlyIntentProof"), Is.False);
                Assert.That(router.EffectiveMode, Is.EqualTo(InputMode.GameplayEnabled));

                router.StepForTests();
                global::AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests.AdvanceFrame(rig);

                Assert.That(router.IsFaulted, Is.False);
                Assert.That(router.CurrentReceipt.Value.Tick, Is.EqualTo(new SimulationTick(0)));
                Assert.That(router.CurrentReceipt.Value.Mode, Is.EqualTo(InputMode.GameplayEnabled));
                Assert.That(rig.Player.NextExpectedTick, Is.EqualTo(new SimulationTick(1)));
            }
            finally
            {
                global::AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests.DestroyAll(objects);
            }
        }

        [Test]
        public void AC001_LegacySerializedFullSimulationPrefabOmitsHubFieldsAndUsesFalseDefaults()
        {
            // This is deliberately a direct YAML assertion, not a Unity
            // materialization: the pre-Q0 authored full graph must retain its
            // missing-field serialization and therefore deserialize the newly
            // appended discriminator fields to their declared false defaults.
            var root = Directory.GetParent(Application.dataPath).FullName;
            var prefabPath = Path.Combine(root, "Assets", "Prefabs", "Combat", "OrdanBossEncounterGraph.prefab");
            var yaml = File.ReadAllText(prefabPath);
            StringAssert.Contains("AcadeGameMaker.Input.Unity::AcadeGameMaker.Input.Unity.InputRouter", yaml);
            StringAssert.DoesNotContain("_authoredHubUiOnly:", yaml);
            StringAssert.DoesNotContain("_hubUiOnlyIntentProof:", yaml);

            var host = new GameObject("m5d7q0-legacy-default-false");
            host.SetActive(false);
            try
            {
                var router = host.AddComponent<InputRouter>();
                Assert.That(HubRuntimeContext.Get<bool>(router, "_authoredHubUiOnly"), Is.False);
                Assert.That(HubRuntimeContext.Get<bool>(router, "_hubUiOnlyIntentProof"), Is.False);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        [TestCase("player")]
        [TestCase("transfer")]
        [TestCase("combat")]
        [TestCase("terminal")]
        [TestCase("camera")]
        [TestCase("initial-mode")]
        public void AC001_EveryReflectedFullHubReferenceHybridFaultsBeforeAnyHubCommitMutation(string hybrid)
        {
            using (var context = HubRuntimeContext.Create())
            {
                GameObject foreign = null;
                try
                {
                    HubRuntimeContext.Invoke(context.Adapter, "Start");
                    context.Router.StepForTests();
                    StagePendingUiBatch(context.Router);
                    var publication = context.Router.CurrentReceipt;
                    var frame = context.Router.CurrentUiFrame;
                    var pending = CapturePendingUiBatch(context.Router);
                    var nextTick = HubRuntimeContext.Get<SimulationTick>(context.Router, "_hubNextTick");
                    var ordinal = HubRuntimeContext.Get<long>(context.Router, "_frameOrdinal");
                    var epoch = HubRuntimeContext.Get<long>(context.Router, "_modeEpoch");

                    foreign = new GameObject("m5d7q0-hybrid-" + hybrid);
                    foreign.SetActive(false);
                    switch (hybrid)
                    {
                        case "player":
                            foreign.AddComponent<Rigidbody2D>();
                            foreign.AddComponent<BoxCollider2D>();
                            HubRuntimeContext.Set(context.Router, "_player", foreign.AddComponent<PlayerMovementController>());
                            break;
                        case "transfer": HubRuntimeContext.Set(context.Router, "_transfer", foreign.AddComponent<AcadeGameMaker.Transfer.Unity.TransferSimulationDriver>()); break;
                        case "combat": HubRuntimeContext.Set(context.Router, "_combat", foreign.AddComponent<OrdanBossCombatSimulationDriver>()); break;
                        case "terminal": HubRuntimeContext.Set(context.Router, "_terminalTeardown", foreign.AddComponent<OrdanBossTerminalTeardown>()); break;
                        case "camera": HubRuntimeContext.Set(context.Router, "_cameraProviderBehaviour", context.Adapter); break;
                        case "initial-mode": HubRuntimeContext.Set(context.Router, "_initialMode", InputMode.GameplayEnabled); break;
                        default: throw new ArgumentOutOfRangeException(nameof(hybrid));
                    }

                    Assert.That(() => { var ignored = context.Router.CurrentReceipt; }, Throws.InvalidOperationException, hybrid);
                    Assert.That(context.Router.IsFaulted, Is.True, hybrid);
                    Assert.That(context.Router.EffectiveMode, Is.EqualTo(InputMode.UIOnly), hybrid + " mode");
                    Assert.That(HubRuntimeContext.Get<long>(context.Router, "_modeEpoch"), Is.EqualTo(epoch), hybrid + " epoch");
                    Assert.That(HubRuntimeContext.Get<SimulationTick>(context.Router, "_hubNextTick"), Is.EqualTo(nextTick), hybrid + " tick");
                    Assert.That(HubRuntimeContext.Get<long>(context.Router, "_frameOrdinal"), Is.EqualTo(ordinal), hybrid + " ordinal");
                    Assert.That(HubRuntimeContext.Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt"), Is.EqualTo(publication), hybrid + " receipt");
                    Assert.That(HubRuntimeContext.Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame"), Is.EqualTo(frame), hybrid + " frame");
                    CollectionAssert.AreEqual(pending, CapturePendingUiBatch(context.Router), hybrid + " pending batch");
                    AssertBothMapsDisabled(context.Router, hybrid);
                }
                finally
                {
                    if (foreign != null) UnityEngine.Object.DestroyImmediate(foreign);
                }
            }
        }

        [Test]
        public void AC001_ClosedFullAndHubAuthoringSeamsRejectWithoutChangingTheirExistingGraphs()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = global::AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests.CreateRig(objects);
                var full = global::AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests.CreateRouter(
                    objects, rig, InputMode.GameplayEnabled, false);
                var player = full.BoundPlayerForAuthoring;
                var transfer = full.BoundTransferForAuthoring;
                var combat = full.BoundCombatForAuthoring;
                var camera = full.BoundCameraProviderForAuthoring;

                Assert.That(() => full.ConfigureHubUiOnlyForAuthoring(), Throws.InvalidOperationException);
                Assert.That(full.BoundPlayerForAuthoring, Is.SameAs(player));
                Assert.That(full.BoundTransferForAuthoring, Is.SameAs(transfer));
                Assert.That(full.BoundCombatForAuthoring, Is.SameAs(combat));
                Assert.That(full.BoundCameraProviderForAuthoring, Is.SameAs(camera));
                Assert.That(HubRuntimeContext.Get<bool>(full, "_authoredHubUiOnly"), Is.False);
                Assert.That(HubRuntimeContext.Get<bool>(full, "_hubUiOnlyIntentProof"), Is.False);

                using (var hub = HubRuntimeContext.Create())
                {
                    Assert.That(() => hub.Router.ConfigureForAuthoring(player, transfer, combat, camera, InputMode.GameplayEnabled), Throws.InvalidOperationException);
                    Assert.That(() => hub.Router.ConfigureTerminalTeardownForAuthoring(null), Throws.InvalidOperationException);
                    Assert.That(hub.Router.AuthoredHubUiOnly, Is.True);
                    Assert.That(hub.Router.HubUiOnlyIntentProofForAuthoring, Is.True);
                    Assert.That(hub.Router.AuthoredInitialMode, Is.EqualTo(InputMode.UIOnly));
                    Assert.That(hub.Router.BoundPlayerForAuthoring, Is.Null);
                    Assert.That(hub.Router.BoundTransferForAuthoring, Is.Null);
                    Assert.That(hub.Router.BoundCombatForAuthoring, Is.Null);
                    Assert.That(hub.Router.BoundTerminalTeardownForAuthoring, Is.Null);
                    Assert.That(hub.Router.BoundCameraProviderForAuthoring, Is.Null);
                }
            }
            finally
            {
                global::AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests.DestroyAll(objects);
            }
        }

        private readonly struct ReceiptFramePair : IEquatable<ReceiptFramePair>
        {
            internal ReceiptFramePair(InputFrameCommitReceipt receipt, UiSemanticFrameV1 frame) { Receipt = receipt; Frame = frame; }
            internal InputFrameCommitReceipt Receipt { get; }
            internal UiSemanticFrameV1 Frame { get; }
            public bool Equals(ReceiptFramePair other) => Receipt.Equals(other.Receipt) && Frame.Equals(other.Frame);
            public override bool Equals(object obj) => obj is ReceiptFramePair other && Equals(other);
            public override int GetHashCode() => Receipt.GetHashCode() ^ Frame.GetHashCode();
        }

        [Test]
        public void AC004_RuntimeGraphHasOnlyTheOneRouterAndNoGameplayCameraRequesterOrSecondSubscriber()
        {
            using (var context = HubRuntimeContext.Create())
            {
                var components = context.Host.GetComponents<Component>();
                Assert.That(components, Has.Length.EqualTo(4), "Transform + router + adapter + latch only");
                Assert.That(context.Router.BoundPlayerForAuthoring, Is.Null);
                Assert.That(context.Router.BoundTransferForAuthoring, Is.Null);
                Assert.That(context.Router.BoundCombatForAuthoring, Is.Null);
                Assert.That(context.Router.BoundTerminalTeardownForAuthoring, Is.Null);
                Assert.That(context.Router.BoundCameraProviderForAuthoring, Is.Null);
                Assert.That(HubRuntimeContext.Get<object>(context.Router, "_modeRequester"), Is.Null);
                Assert.That(HubRuntimeContext.Get<object>(context.Router, "_cameraProvider"), Is.Null);
                var semanticSourceCount = 0;
                foreach (var contract in typeof(InputRouter).GetInterfaces())
                    if (contract == typeof(IUiSemanticFrameSourceV1)) semanticSourceCount++;
                Assert.That(semanticSourceCount, Is.EqualTo(1), "one semantic-frame source implementation");

                HubRuntimeContext.Invoke(context.Adapter, "Start");
                var actions = HubRuntimeContext.Get<GameInputActions>(context.Router, "_actions");
                Assert.That(actions, Is.Not.Null, "one adopted action owner");
                Assert.That(actions.Gameplay.Get().enabled, Is.False);
                Assert.That(actions.UI.Get().enabled, Is.True);
                Assert.That(HubRuntimeContext.Get<bool>(context.Router, "_gameplayCallbacksRegistered"), Is.True);
                Assert.That(HubRuntimeContext.Get<bool>(context.Router, "_uiCallbacksRegistered"), Is.True);
                Assert.That(HubRuntimeContext.Get<int>(context.Router, "_closeAttempts"), Is.Zero);
                Assert.That(HubRuntimeContext.Get<long>(context.Router, "_nextAimSampleId"), Is.Zero);
                Assert.That(HubRuntimeContext.Get<long>(context.Router, "_nextAttackSequenceId"), Is.Zero);
                Assert.That(HubRuntimeContext.Get<bool>(context.Router, "_committedAim"), Is.False);
                Assert.That(HubRuntimeContext.Get<bool>(context.Router, "_committedAttack"), Is.False);
                Assert.That(HubRuntimeContext.Get<object>(context.Router, "_cameraProvider"), Is.Null);

                // The hub transaction receives a non-empty UI batch, commits it,
                // and leaves every gameplay/camera counter at its exact zero
                // baseline.  These are existing router counters, not a Q0 seam.
                StagePendingUiBatch(context.Router);
                context.Router.StepForTests();
                Assert.That(context.Router.CurrentReceipt.Value.Mode, Is.EqualTo(InputMode.UIOnly));
                Assert.That(context.Router.CurrentUiFrame.HasValue, Is.True);
                Assert.That(HubRuntimeContext.Get<long>(context.Router, "_nextAimSampleId"), Is.Zero);
                Assert.That(HubRuntimeContext.Get<long>(context.Router, "_nextAttackSequenceId"), Is.Zero);
                Assert.That(HubRuntimeContext.Get<bool>(context.Router, "_committedAim"), Is.False);
                Assert.That(HubRuntimeContext.Get<bool>(context.Router, "_committedAttack"), Is.False);
                Assert.That(HubRuntimeContext.Get<object>(context.Router, "_modeRequester"), Is.Null);
                Assert.That(HubRuntimeContext.Get<object>(context.Router, "_cameraProvider"), Is.Null);
            }
        }

        [Test]
        public void AC004_StaticHubCommitHasNoGameplayCameraRequesterOrSecondSourcePath()
        {
            var sourcePath = Path.GetFullPath(Path.Combine("Assets", "AcadeGameMaker", "Runtime", "Input", "Unity", "InputRouter.cs"));
            Assert.That(File.Exists(sourcePath), Is.True, "InputRouter source");
            var source = File.ReadAllText(sourcePath);
            var hubCommit = ExtractMethod(source, "private void CommitHubUiOnlyFrame()");

            foreach (var forbidden in new[]
            {
                "BuildMovement", "BuildTransfer", "BuildCombat", "BuildAim", "PrepareInputFrame",
                "ValidateInputFrameCandidate", "CommitInputFrame", "TryGetCompletedCamera", "_player",
                "_transfer", "_combat", "_cameraProvider", "_nextAimSampleId", "_nextAttackSequenceId",
                "RegisterModeRequester", "RequestMode", "AddCallbacks"
            })
                Assert.That(hubCommit.Contains(forbidden), Is.False, forbidden + " must not be reachable from the hub commit transaction");

            Assert.That(Count(source, "IUiSemanticFrameSourceV1"), Is.EqualTo(1), "one frame source contract");
            Assert.That(Count(source, ".AddCallbacks(this)"), Is.EqualTo(2), "one router owns the two generated maps");
            Assert.That(Count(source, "InputSystem.onAfterUpdate +="), Is.EqualTo(1), "one map-enable quarantine hook");
            Assert.That(source.Contains("InputSystemUIInputModule"), Is.False, "no UI module subscriber");
            Assert.That(source.Contains("new InputRouter("), Is.False, "no second router allocation");
        }

        [Test]
        public void AC005_HealthyPreAndPostInitializationCommandsDoNotMutateMapsOrMode()
        {
            using (var context = HubRuntimeContext.Create())
            {
                AssertRejectedCommandLeavesHealthyHubUnchanged(context, "pre-initialization");
                HubRuntimeContext.Invoke(context.Adapter, "Start");
                AssertRejectedCommandLeavesHealthyHubUnchanged(context, "post-initialization");
            }
        }

        [Test]
        public void AC003_OneCallbackDrivenFixedBoundarySchedulerMatchesCompleteReceiptAndFrameSequencesAtAllRenderRates()
        {
            // This is a rational timeline, not a modeled counter.  Every
            // render opportunity executes a real InputSystem.Update before
            // the fixed boundary.  The same non-empty generated-action script
            // is then injected and dispatched immediately before boundaries
            // 0, 1 and 2, irrespective of render rate.
            var at30 = RunCallbackDrivenFixedScheduler(30);
            var at60 = RunCallbackDrivenFixedScheduler(60);
            var at144 = RunCallbackDrivenFixedScheduler(144);

            Assert.That(at30.RenderInputSystemUpdates, Is.EqualTo(3), "30 FPS rational opportunities");
            Assert.That(at60.RenderInputSystemUpdates, Is.EqualTo(6), "60 FPS rational opportunities");
            Assert.That(at144.RenderInputSystemUpdates, Is.EqualTo(14), "144 FPS rational opportunities");
            Assert.That(at30.RenderInputSystemUpdates, Is.Not.EqualTo(at60.RenderInputSystemUpdates));
            Assert.That(at60.RenderInputSystemUpdates, Is.Not.EqualTo(at144.RenderInputSystemUpdates));
            Assert.That(at30.CallbackDispatchUpdates, Is.EqualTo(3), "one non-empty callback dispatch per fixed boundary");
            Assert.That(at60.CallbackDispatchUpdates, Is.EqualTo(3), "same callback script at 60 FPS");
            Assert.That(at144.CallbackDispatchUpdates, Is.EqualTo(3), "same callback script at 144 FPS");
            Assert.That(at30.Sequence, Is.Not.Empty, "30 FPS receipt/frame proof");
            Assert.That(at60.Sequence, Is.Not.Empty, "60 FPS receipt/frame proof");
            Assert.That(at144.Sequence, Is.Not.Empty, "144 FPS receipt/frame proof");
            Assert.That(at30.Sequence[0].Frame.HasPoint, Is.True, "the real callback script captured its point");
            Assert.That(at30.Sequence[0].Frame.PointChanged, Is.True, "the real callback script changed its point");
            Assert.That(at30.Sequence[0].Frame.ClickPressed, Is.True, "the real callback script captured its click");
            Assert.That(at30.Sequence[0].Frame.SubmitPressed, Is.True, "the real callback script captured submit");
            Assert.That(at30.Sequence[0].Frame.CancelPressed, Is.True, "the real callback script captured cancel");
            Assert.That(at30.Sequence[0].Frame.ScrollXQ4096, Is.Not.EqualTo(0L), "the real callback script captured scroll");
            CollectionAssert.AreEqual(at30.Sequence, at60.Sequence, "complete receipt/frame sequence at 60 FPS");
            CollectionAssert.AreEqual(at30.Sequence, at144.Sequence, "complete receipt/frame sequence at 144 FPS");
        }

        [Test]
        public void AC005_ReflectedRequesterCorruptionFaultsAtTheNextBoundaryAndClosesMaps()
        {
            using (var context = HubRuntimeContext.Create())
            {
                HubRuntimeContext.Invoke(context.Adapter, "Start");
                context.Router.StepForTests();
                var receipt = context.Router.CurrentReceipt;
                var frame = context.Router.CurrentUiFrame;
                var actions = HubRuntimeContext.Get<GameInputActions>(context.Router, "_actions");
                HubRuntimeContext.Set(context.Router, "_modeRequester", new object());

                Assert.That(() => { var ignored = context.Router.CurrentReceipt; }, Throws.InvalidOperationException);
                Assert.That(context.Router.IsFaulted, Is.True);
                Assert.That(actions.Gameplay.Get().enabled, Is.False);
                Assert.That(actions.UI.Get().enabled, Is.False);
                Assert.That(HubRuntimeContext.Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt"), Is.EqualTo(receipt));
                Assert.That(HubRuntimeContext.Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame"), Is.EqualTo(frame));
            }
        }

        [Test]
        public void AC005_ReflectedPendingModeRequestCorruptionAtNextCommitPreservesPublicationAndBatchThenClosesMaps()
        {
            using (var context = HubRuntimeContext.Create())
            {
                HubRuntimeContext.Invoke(context.Adapter, "Start");
                context.Router.StepForTests();
                StagePendingUiBatch(context.Router);
                var receipt = context.Router.CurrentReceipt;
                var frame = context.Router.CurrentUiFrame;
                var pendingBatch = CapturePendingUiBatch(context.Router);
                SetPendingModeRequestCorruption(context.Router);

                Assert.That(() => context.Router.StepForTests(), Throws.InvalidOperationException);
                Assert.That(context.Router.IsFaulted, Is.True);
                Assert.That(HubRuntimeContext.Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt"), Is.EqualTo(receipt));
                Assert.That(HubRuntimeContext.Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame"), Is.EqualTo(frame));
                CollectionAssert.AreEqual(pendingBatch, CapturePendingUiBatch(context.Router));
                AssertBothMapsDisabled(context.Router, "pending-mode corruption");
                Assert.That(() => context.Router.StepForTests(), Throws.InvalidOperationException, "terminal hub cannot consume the retained batch");
            }
        }

        [TestCase("router-first")]
        [TestCase("adapter-first")]
        public void AC009_AwakeStartOrderingPublishesOnlyAtAdapterThenFreshSessionStartsFromZero(string ordering)
        {
            using (var first = HubRuntimeContext.Create())
            {
                // Router-first is an Awake/cohort ordering only: its Start cannot
                // precede adapter preparation on this closed hub branch.
                Assert.That(first.Adapter.CurrentReceipt, Is.Null, ordering + " before adapter Start");
                HubRuntimeContext.Invoke(first.Adapter, "Start");
                if (ordering == "router-first") HubRuntimeContext.Invoke(first.Router, "Start");
                first.Router.StepForTests();
                Assert.That(first.Router.CurrentReceipt.Value.Tick.Value, Is.Zero, ordering);
                HubRuntimeContext.Invoke(first.Router, "OnDisable");
                HubRuntimeContext.Invoke(first.Router, "OnDestroy");
                Assert.That(HubRuntimeContext.Get<int>(first.Router, "_closeAttempts"), Is.EqualTo(1), ordering);
            }
            using (var reloaded = HubRuntimeContext.Create())
            {
                HubRuntimeContext.Invoke(reloaded.Adapter, "Start");
                reloaded.Router.StepForTests();
                Assert.That(reloaded.Router.CurrentReceipt.Value.Tick.Value, Is.Zero, "fresh reload session");
                Assert.That(reloaded.Router.CurrentReceipt.Value.FrameOrdinal, Is.Zero, "fresh reload ordinal");
            }
        }

        [Test]
        public void AC012_PostPublicationFaultStopsConsumptionAndRepeatedTeardownClosesOnce()
        {
            using (var context = HubRuntimeContext.Create())
            {
                HubRuntimeContext.Invoke(context.Adapter, "Start");
                context.Router.StepForTests();
                var receipt = context.Router.CurrentReceipt;
                var frame = context.Router.CurrentUiFrame;
                var actions = HubRuntimeContext.Get<GameInputActions>(context.Router, "_actions");
                var preparedState = HubRuntimeContext.Get<object>(context.Router, "_preparedLaunchState").ToString();
                Assert.That(preparedState, Is.EqualTo("Initialized"), "existing M5D7M state before the runtime fault");
                HubRuntimeContext.Set(context.Router, "_hubFrameOrdinalProof", 99L);

                Assert.That(() => context.Router.StepForTests(), Throws.InvalidOperationException);
                Assert.That(context.Router.IsFaulted, Is.True);
                Assert.That(HubRuntimeContext.Get<GameInputActions>(context.Router, "_actions"), Is.SameAs(actions), "adopted action stays owned until teardown");
                Assert.That(HubRuntimeContext.Get<object>(context.Router, "_preparedLaunchState").ToString(), Is.EqualTo(preparedState), "fault does not create a state row");
                Assert.That(actions.Gameplay.Get().enabled, Is.False);
                Assert.That(actions.UI.Get().enabled, Is.False);
                Assert.That(HubRuntimeContext.Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt"), Is.EqualTo(receipt));
                Assert.That(HubRuntimeContext.Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame"), Is.EqualTo(frame));

                HubRuntimeContext.Invoke(context.Router, "OnDisable");
                HubRuntimeContext.Invoke(context.Router, "OnDestroy");
                HubRuntimeContext.Invoke(context.Router, "OnDestroy");
                Assert.That(HubRuntimeContext.Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                Assert.That(HubRuntimeContext.Get<int>(context.Router, "_disposeAttempts"), Is.EqualTo(1));
                Assert.That(HubRuntimeContext.Get<GameInputActions>(context.Router, "_actions"), Is.Null);
                Assert.That(HubRuntimeContext.Get<object>(context.Router, "_preparedLaunchState").ToString(), Is.EqualTo(preparedState), "closure retains the existing Initialized row");
            }
        }

        private static void AssertRejectedCommandLeavesHealthyHubUnchanged(HubRuntimeContext context, string phase)
        {
            var actions = HubRuntimeContext.Get<GameInputActions>(context.Router, "_actions");
            var gameplayEnabled = actions != null && actions.Gameplay.Get().enabled;
            var uiEnabled = actions != null && actions.UI.Get().enabled;
            var mode = context.Router.EffectiveMode;
            var epoch = HubRuntimeContext.Get<long>(context.Router, "_modeEpoch");
            var receipt = HubRuntimeContext.Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt");
            var frame = HubRuntimeContext.Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame");
            StagePendingUiBatch(context.Router);
            var pending = CapturePendingUiBatch(context.Router);
            Assert.That(() => context.Router.RegisterModeRequester(new object()), Throws.InvalidOperationException, phase + " requester");
            Assert.That(() => context.Router.RequestMode(new object(), InputMode.GameplayEnabled, "forbidden", new SimulationTick(0)), Throws.InvalidOperationException, phase + " mode");
            Assert.That(context.Router.EffectiveMode, Is.EqualTo(mode), phase + " mode unchanged");
            Assert.That(HubRuntimeContext.Get<long>(context.Router, "_modeEpoch"), Is.EqualTo(epoch), phase + " epoch unchanged");
            Assert.That(HubRuntimeContext.Get<object>(context.Router, "_modeRequester"), Is.Null, phase + " requester unchanged");
            Assert.That(HubRuntimeContext.Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt"), Is.EqualTo(receipt), phase + " receipt unchanged");
            Assert.That(HubRuntimeContext.Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame"), Is.EqualTo(frame), phase + " frame unchanged");
            CollectionAssert.AreEqual(pending, CapturePendingUiBatch(context.Router), phase + " pending UI batch unchanged");
            Assert.That(context.Router.IsFaulted, Is.False, phase + " remains healthy");
            if (actions != null)
            {
                Assert.That(actions.Gameplay.Get().enabled, Is.EqualTo(gameplayEnabled), phase + " gameplay map unchanged");
                Assert.That(actions.UI.Get().enabled, Is.EqualTo(uiEnabled), phase + " UI map unchanged");
            }
        }

        private CallbackFixedSchedulerResult RunCallbackDrivenFixedScheduler(int renderFps)
        {
            const int fixedHz = 30;
            const int fixedBoundaryCount = 3;
            using (var context = HubRuntimeContext.Create())
            {
                HubRuntimeContext.Invoke(context.Adapter, "Start");
                var keyboard = InputSystem.AddDevice<Keyboard>();
                var mouse = InputSystem.AddDevice<Mouse>();
                try
                {
                    var sequence = new ReceiptFramePair[fixedBoundaryCount];
                    var renderInputSystemUpdates = 0;
                    var nextRenderNumerator = 1;
                    for (var fixedBoundary = 0; fixedBoundary < sequence.Length; fixedBoundary++)
                    {
                        // The exact comparison nextRenderNumerator / renderFps
                        // <= (fixedBoundary + 1) / fixedHz avoids float
                        // scheduling drift.  These are actual update calls,
                        // so the varying opportunity total is observable.
                        var fixedTimeNumerator = (fixedBoundary + 1) * renderFps;
                        while (nextRenderNumerator * fixedHz <= fixedTimeNumerator)
                        {
                            InputSystem.Update();
                            renderInputSystemUpdates++;
                            nextRenderNumerator++;
                        }
                        InjectSameUiCallbackScriptBeforeFixedBoundary(keyboard, mouse, fixedBoundary);
                        context.Router.StepForTests();
                        sequence[fixedBoundary] = new ReceiptFramePair(context.Router.CurrentReceipt.Value, context.Router.CurrentUiFrame.Value);
                    }
                    return new CallbackFixedSchedulerResult(sequence, renderInputSystemUpdates, fixedBoundaryCount);
                }
                finally
                {
                    InputSystem.RemoveDevice(keyboard);
                    InputSystem.RemoveDevice(mouse);
                }
            }
        }

        private void InjectSameUiCallbackScriptBeforeFixedBoundary(Keyboard keyboard, Mouse mouse, int fixedBoundary)
        {
            if (fixedBoundary == 0)
            {
                Set(mouse.position, new Vector2(31.5f, -7.5f));
                Press(keyboard.dKey);
                Press(mouse.leftButton);
                Press(keyboard.enterKey);
                Press(keyboard.escapeKey);
                InputSystem.QueueDeltaStateEvent(mouse.scroll, new Vector2(1.5f, -2.5f));
            }
            else if (fixedBoundary == 1)
            {
                Release(keyboard.dKey);
                Release(mouse.leftButton);
                Release(keyboard.enterKey);
                Release(keyboard.escapeKey);
                Set(mouse.position, new Vector2(-17.5f, 2048.5f));
                InputSystem.QueueDeltaStateEvent(mouse.scroll, new Vector2(-1f, 3f));
            }
            else
            {
                Press(keyboard.aKey);
                Set(mouse.position, new Vector2(8.5f, 9.5f));
                Press(mouse.leftButton);
                Press(keyboard.enterKey);
                InputSystem.QueueDeltaStateEvent(mouse.scroll, new Vector2(2f, 0f));
            }
            InputSystem.Update();
        }

        private readonly struct CallbackFixedSchedulerResult
        {
            internal CallbackFixedSchedulerResult(ReceiptFramePair[] sequence, int renderInputSystemUpdates, int callbackDispatchUpdates)
            {
                Sequence = sequence;
                RenderInputSystemUpdates = renderInputSystemUpdates;
                CallbackDispatchUpdates = callbackDispatchUpdates;
            }

            internal ReceiptFramePair[] Sequence { get; }
            internal int RenderInputSystemUpdates { get; }
            internal int CallbackDispatchUpdates { get; }
        }

        private static void StagePendingUiBatch(InputRouter router)
        {
            HubRuntimeContext.Set(router, "_uiNavigateXQ4096", 4096);
            HubRuntimeContext.Set(router, "_uiNavigateYQ4096", -4096);
            HubRuntimeContext.Set(router, "_uiNavigateChanged", true);
            HubRuntimeContext.Set(router, "_uiHasPoint", true);
            HubRuntimeContext.Set(router, "_uiPointXActualPx", 321);
            HubRuntimeContext.Set(router, "_uiPointYActualPx", 654);
            HubRuntimeContext.Set(router, "_uiPointChanged", true);
            HubRuntimeContext.Set(router, "_uiClickPressed", true);
            HubRuntimeContext.Set(router, "_uiHasClickPoint", true);
            HubRuntimeContext.Set(router, "_uiClickPointXActualPx", 321);
            HubRuntimeContext.Set(router, "_uiClickPointYActualPx", 654);
            HubRuntimeContext.Set(router, "_uiScrollXQ4096", 15L);
            HubRuntimeContext.Set(router, "_uiScrollYQ4096", -25L);
            HubRuntimeContext.Set(router, "_uiSubmitPressed", true);
            HubRuntimeContext.Set(router, "_uiCancelPressed", true);
            HubRuntimeContext.Set(router, "_callbackOrdinal", 17L);
        }

        private static object[] CapturePendingUiBatch(InputRouter router)
        {
            return new object[]
            {
                HubRuntimeContext.Get<int>(router, "_uiNavigateXQ4096"), HubRuntimeContext.Get<int>(router, "_uiNavigateYQ4096"),
                HubRuntimeContext.Get<bool>(router, "_uiNavigateChanged"), HubRuntimeContext.Get<bool>(router, "_uiHasPoint"),
                HubRuntimeContext.Get<int>(router, "_uiPointXActualPx"), HubRuntimeContext.Get<int>(router, "_uiPointYActualPx"),
                HubRuntimeContext.Get<bool>(router, "_uiPointChanged"), HubRuntimeContext.Get<bool>(router, "_uiClickPressed"),
                HubRuntimeContext.Get<bool>(router, "_uiHasClickPoint"), HubRuntimeContext.Get<int>(router, "_uiClickPointXActualPx"),
                HubRuntimeContext.Get<int>(router, "_uiClickPointYActualPx"), HubRuntimeContext.Get<long>(router, "_uiScrollXQ4096"),
                HubRuntimeContext.Get<long>(router, "_uiScrollYQ4096"), HubRuntimeContext.Get<bool>(router, "_uiSubmitPressed"),
                HubRuntimeContext.Get<bool>(router, "_uiCancelPressed"), HubRuntimeContext.Get<long>(router, "_callbackOrdinal")
            };
        }

        private static void SetPendingModeRequestCorruption(InputRouter router)
        {
            var pendingType = typeof(InputRouter).GetNestedType("PendingModeRequest", BindingFlags.NonPublic);
            var constructor = pendingType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { typeof(InputMode), typeof(string), typeof(SimulationTick) }, null);
            var request = constructor.Invoke(new object[] { InputMode.GameplayEnabled, "reflected-corruption", new SimulationTick(1) });
            var field = typeof(InputRouter).GetField("_pendingModeRequest", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(router, Activator.CreateInstance(field.FieldType, request));
        }

        private static void AssertBothMapsDisabled(InputRouter router, string message)
        {
            var actions = HubRuntimeContext.Get<GameInputActions>(router, "_actions");
            Assert.That(actions.Gameplay.Get().enabled, Is.False, message + " Gameplay map");
            Assert.That(actions.UI.Get().enabled, Is.False, message + " UI map");
        }

        private static string ExtractMethod(string source, string signature)
        {
            var start = source.IndexOf(signature, StringComparison.Ordinal);
            Assert.That(start, Is.GreaterThanOrEqualTo(0), signature);
            var open = source.IndexOf('{', start);
            var depth = 0;
            for (var index = open; index < source.Length; index++)
            {
                if (source[index] == '{') depth++;
                else if (source[index] == '}' && --depth == 0) return source.Substring(start, index - start + 1);
            }
            throw new InvalidOperationException("Unclosed source method: " + signature);
        }

        private static int Count(string source, string value)
        {
            var count = 0;
            for (var start = 0; (start = source.IndexOf(value, start, StringComparison.Ordinal)) >= 0; start += value.Length) count++;
            return count;
        }
    }

    internal sealed class HubRuntimeContext : IDisposable
    {
        private readonly string _root;
        internal GameObject Host { get; }
        internal InputRouter Router { get; }
        internal DesktopProfileLaunchAdapterV1 Adapter { get; }
        internal HubEntryHandoffLatchV1 Latch { get; }

        private HubRuntimeContext(GameObject host, string root, InputRouter router, DesktopProfileLaunchAdapterV1 adapter, HubEntryHandoffLatchV1 latch)
        { Host = host; _root = root; Router = router; Adapter = adapter; Latch = latch; }

        internal static HubRuntimeContext Create()
        {
            var root = Path.Combine(Path.GetTempPath(), "m5d7q0-remaining-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            var host = new GameObject("m5d7q0-remaining-hub");
            host.SetActive(false);
            var router = host.AddComponent<InputRouter>();
            var adapter = host.AddComponent<DesktopProfileLaunchAdapterV1>();
            var latch = host.AddComponent<HubEntryHandoffLatchV1>();
            router.ConfigureHubUiOnlyForAuthoring();
            adapter.ConfigureHubUiOnlyForAuthoring(router);
            adapter.ConfigureForTests(router, new Environment(root), new DesktopProfileLaunchPreparationPortV1());
            latch.ConfigureHubUiOnlyForAuthoring(adapter, router);
            host.SetActive(true);
            return new HubRuntimeContext(host, root, router, adapter, latch);
        }

        internal static T Get<T>(object target, string field) => (T)target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
        internal static void Set(object target, string field, object value) => target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
        internal static void Invoke(object target, string method) => target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);

        public void Dispose()
        {
            UnityEngine.Object.DestroyImmediate(Host);
            if (Directory.Exists(_root)) Directory.Delete(_root, true);
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
