using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using AcadeGameMaker.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Input.Unity.PlayMode.Tests
{
    // REQ-M5D7Q0-001..008 / AC-M5D7Q0-003,005,007,012. These fixtures use
    // the actual adapter-owned prepared lifecycle; no test-only router init.
    internal sealed class HubUiOnlyInputRouterPlayModeTests
    {
        [Test]
        public void AC003_HealthyPreparedHubPublishesLocalUiOnlyTickZeroThenExactSuccessors()
        {
            var context = Create();
            try
            {
                Invoke(context.Adapter, "Start");
                Assert.That(context.Router.CurrentReceipt, Is.Null);
                Assert.That(context.Router.CurrentUiFrame, Is.Null);
                Assert.That(Get<SimulationTick>(context.Router, "_hubNextTick").Value, Is.Zero);
                Assert.That(Get<SimulationTick>(context.Router, "_hubNextTickProof").Value, Is.Zero);
                Assert.That(Get<long>(context.Router, "_frameOrdinal"), Is.Zero);
                Assert.That(Get<long>(context.Router, "_hubFrameOrdinalProof"), Is.Zero);

                context.Router.StepForTests();

                var receipt = context.Router.CurrentReceipt;
                var frame = context.Router.CurrentUiFrame;
                Assert.That(receipt.HasValue && frame.HasValue, Is.True);
                Assert.That(receipt.Value.Tick.Value, Is.Zero);
                Assert.That(receipt.Value.FrameOrdinal, Is.Zero);
                Assert.That(receipt.Value.ModeEpoch, Is.Zero);
                Assert.That(receipt.Value.Mode, Is.EqualTo(InputMode.UIOnly));
                Assert.That(frame.Value.Receipt, Is.EqualTo(receipt.Value));
                Assert.That(Get<InputFrameCommitReceipt?>(context.Router, "_uiPublicationProof"), Is.EqualTo(receipt));
                Assert.That(Get<SimulationTick>(context.Router, "_hubNextTick").Value, Is.EqualTo(1));
                Assert.That(Get<SimulationTick>(context.Router, "_hubNextTickProof").Value, Is.EqualTo(1));
                Assert.That(Get<long>(context.Router, "_frameOrdinal"), Is.EqualTo(1));
                Assert.That(Get<long>(context.Router, "_hubFrameOrdinalProof"), Is.EqualTo(1));
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void AC005_HubRejectsRequesterAndModeBeforeAnyGameplayDereferenceOrMutation()
        {
            var context = Create();
            try
            {
                Invoke(context.Adapter, "Start");
                var requester = new object();
                Assert.That(() => context.Router.RegisterModeRequester(requester), Throws.InvalidOperationException);
                Assert.That(() => context.Router.RequestMode(requester, InputMode.GameplayEnabled, "illegal", new SimulationTick(0)), Throws.InvalidOperationException);
                Assert.That(context.Router.EffectiveMode, Is.EqualTo(InputMode.UIOnly));
                Assert.That(Get<long>(context.Router, "_modeEpoch"), Is.Zero);
                Assert.That(Get<object>(context.Router, "_modeRequester"), Is.Null);
                Assert.That(Get<object>(context.Router, "_player"), Is.Null);
                Assert.That(context.Router.IsFaulted, Is.False);
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void AC007_ReflectedHubSuccessorCorruptionPreservesForensicsAndInvalidatesSource()
        {
            var context = Create();
            try
            {
                Invoke(context.Adapter, "Start");
                context.Router.StepForTests();
                var receipt = Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt");
                var frame = Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame");
                var actions = Get<GameInputActions>(context.Router, "_actions");
                Set(context.Router, "_hubNextTick", new SimulationTick(99));

                Assert.That(() => { var ignored = context.Router.CurrentReceipt; }, Throws.InvalidOperationException);
                Assert.That(context.Router.IsFaulted, Is.True);
                Assert.That(context.Router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.HubUiOnly));
                Assert.That(Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt"), Is.EqualTo(receipt));
                Assert.That(Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame"), Is.EqualTo(frame));
                Assert.That(Get<GameInputActions>(context.Router, "_actions"), Is.SameAs(actions));
                Assert.That(actions.Gameplay.Get().enabled, Is.False);
                Assert.That(actions.UI.Get().enabled, Is.False);
                Assert.That(() => context.Router.StepForTests(), Throws.InvalidOperationException);
            }
            finally { context.Dispose(); }
        }

        [TestCase("owner")]
        [TestCase("actions")]
        public void AC007_IndependentPreparedIdentityProofRejectsOneReflectedReplacement(string target)
        {
            var context = Create();
            GameInputActions foreignActions = null;
            object originalProof = null;
            try
            {
                Invoke(context.Adapter, "Start");
                var field = target == "owner" ? "_preparedLaunchOwnerProof" : "_preparedActionsProof";
                originalProof = Get<object>(context.Router, field);
                if (target == "owner") Set(context.Router, field, new object());
                else
                {
                    foreignActions = new GameInputActions();
                    Set(context.Router, field, foreignActions);
                }

                Assert.That(() => { var ignored = context.Router.CurrentReceipt; }, Throws.InvalidOperationException, target);
                Assert.That(context.Router.IsFaulted, Is.True, target);
                Assert.That(context.Router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.HubUiOnly), target);
                Set(context.Router, field, originalProof); // cleanup only; fault evidence was already asserted.
            }
            finally
            {
                HubUiOnlyQ0TestSupport.DisposeCallerOwnedActions(foreignActions);
                context.Dispose();
            }
        }

        [Test]
        public void AC007_PreInitializationHybridMutationFailsThroughPreparedClosureWithoutPublication()
        {
            var context = Create();
            try
            {
                Assert.That(Get<object>(context.Router, "_player"), Is.Null, "fixture adopted before Start");
                Set(context.Router, "_modeRequester", new object());
                Assert.That(() => { var ignored = context.Router.CurrentReceipt; }, Throws.InvalidOperationException);
                Assert.That(context.Router.IsFaulted, Is.True);
                Assert.That(Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt"), Is.Null);
                Assert.That(Get<GameInputActions>(context.Router, "_actions"), Is.Null);
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void AC007_PreReservationHubCorruptionOccupiesClosedActionFreeTerminalRow()
        {
            var host = new GameObject("m5d7q0-unreserved-corruption");
            host.SetActive(false);
            try
            {
                var router = host.AddComponent<InputRouter>();
                router.ConfigureHubUiOnlyForAuthoring();
                Set(router, "_modeRequester", new object());

                Assert.That(() => { var ignored = router.CurrentReceipt; }, Throws.InvalidOperationException);
                Assert.That(router.IsFaulted, Is.True);
                Assert.That(Get<GameInputActions>(router, "_actions"), Is.Null);
                Assert.That(Get<bool>(router, "_actionsClosed"), Is.True);
                Assert.That(Get<int>(router, "_closeAttempts"), Is.Zero);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        [Test]
        public void AC007_PreInitializationHubMarkerClearCannotRestoreLegacyOrRequesterPath()
        {
            var host = new GameObject("m5d7q0-preinit-marker-clear");
            host.SetActive(false);
            try
            {
                var router = host.AddComponent<InputRouter>();
                router.ConfigureHubUiOnlyForAuthoring();
                Set(router, "_authoredHubUiOnly", false);

                Assert.That(() => router.RegisterModeRequester(new object()), Throws.InvalidOperationException);
                Assert.That(router.IsFaulted, Is.True);
                Assert.That(Get<GameInputActions>(router, "_actions"), Is.Null);
                Assert.That(Get<bool>(router, "_actionsClosed"), Is.True);
                Assert.That(Get<InputFrameCommitReceipt?>(router, "_currentReceipt"), Is.Null);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        [Test]
        public void AC002_MarkerMismatchBeforeReserveUsesTheLegalActionFreeClosedRow()
        {
            var host = new GameObject("m5d7q0-reserve-marker-mismatch");
            host.SetActive(false);
            try
            {
                var router = host.AddComponent<InputRouter>();
                router.ConfigureHubUiOnlyForAuthoring();
                Set(router, "_authoredHubUiOnly", false);
                host.SetActive(true);

                Assert.That(() => router.ReservePreparedHubLaunch(new object()), Throws.InvalidOperationException);
                Assert.That(Get<object>(router, "_preparedLaunchState").ToString(), Is.EqualTo("Unreserved"));
                Assert.That(router.PreparedLaunchFaultStageForTests.ToString(), Is.EqualTo("None"));
                Assert.That(router.IsFaulted, Is.True);
                Assert.That(Get<GameInputActions>(router, "_actions"), Is.Null);
                Assert.That(Get<bool>(router, "_actionsClosed"), Is.True);
                Assert.That(Get<int>(router, "_closeAttempts"), Is.Zero);
                Assert.That(Get<InputFrameCommitReceipt?>(router, "_currentReceipt"), Is.Null);
                Assert.That(Get<UiSemanticFrameV1?>(router, "_currentUiFrame"), Is.Null);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        [TestCase("missing")]
        [TestCase("disabled")]
        public void AC002_MissingOrDisabledAdapterCannotAllocateTheHubLegacyFallback(string adapterState)
        {
            var host = new GameObject("m5d7q0-" + adapterState + "-adapter");
            host.SetActive(false);
            try
            {
                var router = host.AddComponent<InputRouter>();
                router.ConfigureHubUiOnlyForAuthoring();
                if (adapterState == "disabled")
                {
                    var adapter = host.AddComponent<DesktopProfileLaunchAdapterV1>();
                    adapter.ConfigureHubUiOnlyForAuthoring(router);
                    adapter.enabled = false;
                    LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape("InvalidOperationException: Desktop launch adapter/router cohort is not exact.")));
                }

                host.SetActive(true);
                Invoke(router, "Start");

                Assert.That(router.IsFaulted, Is.True, adapterState);
                Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.HubUiOnly), adapterState);
                Assert.That(Get<GameInputActions>(router, "_actions"), Is.Null, adapterState + " must not allocate legacy actions");
                Assert.That(Get<int>(router, "_closeAttempts"), Is.Zero, adapterState + " has no adopted candidate to close");
                Assert.That(Get<InputFrameCommitReceipt?>(router, "_currentReceipt"), Is.Null, adapterState);
                Assert.That(Get<UiSemanticFrameV1?>(router, "_currentUiFrame"), Is.Null, adapterState);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        [Test]
        public void AC002_MarkerMismatchAfterReserveBeforeAdoptUsesOwnerBoundFailedWithoutClosingCallerCandidate()
        {
            var host = new GameObject("m5d7q0-adopt-marker-mismatch");
            host.SetActive(false);
            var candidate = new GameInputActions();
            try
            {
                var router = host.AddComponent<InputRouter>();
                router.ConfigureHubUiOnlyForAuthoring();
                host.SetActive(true);
                var owner = new object();
                router.ReservePreparedHubLaunch(owner);
                Set(router, "_authoredHubUiOnly", false);

                Assert.That(() => router.AdoptPreparedHubActions(owner, candidate), Throws.InvalidOperationException);
                Assert.That(Get<object>(router, "_preparedLaunchState").ToString(), Is.EqualTo("Failed"));
                Assert.That(router.PreparedLaunchFaultStageForTests.ToString(), Is.EqualTo("Closed"));
                Assert.That(Get<object>(router, "_preparedLaunchOwner"), Is.SameAs(owner));
                Assert.That(Get<object>(router, "_preparedLaunchOwnerProof"), Is.SameAs(owner));
                Assert.That(Get<GameInputActions>(router, "_actions"), Is.Null);
                Assert.That(Get<int>(router, "_closeAttempts"), Is.Zero);
                Assert.That(candidate.Gameplay.Get().enabled, Is.False);
                Assert.That(candidate.UI.Get().enabled, Is.False);
                Assert.That(Get<InputFrameCommitReceipt?>(router, "_currentReceipt"), Is.Null);
                Assert.That(Get<UiSemanticFrameV1?>(router, "_currentUiFrame"), Is.Null);
            }
            finally { HubUiOnlyQ0TestSupport.DisposeCallerOwnedActions(candidate); UnityEngine.Object.DestroyImmediate(host); }
        }

        [Test]
        public void AC002_MarkerMismatchAfterAdoptBeforeInitializeClosesExactOwnedCandidateOnce()
        {
            var host = new GameObject("m5d7q0-initialize-marker-mismatch");
            host.SetActive(false);
            try
            {
                var router = host.AddComponent<InputRouter>();
                router.ConfigureHubUiOnlyForAuthoring();
                host.SetActive(true);
                var owner = new object();
                var candidate = new GameInputActions();
                router.ReservePreparedHubLaunch(owner);
                router.AdoptPreparedHubActions(owner, candidate);
                Set(router, "_authoredHubUiOnly", false);

                Assert.That(() => router.InitializePreparedHub(owner), Throws.InvalidOperationException);
                Assert.That(Get<object>(router, "_preparedLaunchState").ToString(), Is.EqualTo("Failed"));
                Assert.That(router.PreparedLaunchFaultStageForTests.ToString(), Is.EqualTo("Closed"));
                Assert.That(Get<object>(router, "_preparedLaunchOwner"), Is.SameAs(owner));
                Assert.That(Get<GameInputActions>(router, "_lastClosedActions"), Is.SameAs(candidate));
                Assert.That(Get<GameInputActions>(router, "_actions"), Is.Null);
                Assert.That(Get<int>(router, "_closeAttempts"), Is.EqualTo(1));
                Assert.That(Get<int>(router, "_disposeAttempts"), Is.EqualTo(1));
                Assert.That(candidate.Gameplay.Get().enabled, Is.False);
                Assert.That(candidate.UI.Get().enabled, Is.False);
                Assert.That(Get<InputFrameCommitReceipt?>(router, "_currentReceipt"), Is.Null);
                Assert.That(Get<UiSemanticFrameV1?>(router, "_currentUiFrame"), Is.Null);
                Assert.That(Get<bool>(router, "_actionsClosed"), Is.True);
            }
            finally { UnityEngine.Object.DestroyImmediate(host); }
        }

        [Test]
        public void AC007_PostPublicationHubFlagMutationCannotExposeForensicSource()
        {
            var context = Create();
            try
            {
                Invoke(context.Adapter, "Start");
                context.Router.StepForTests();
                var receipt = Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt");
                var frame = Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame");
                Set(context.Router, "_authoredHubUiOnly", false);

                Assert.That(() => { var ignored = context.Router.CurrentReceipt; }, Throws.InvalidOperationException);
                Assert.That(context.Router.IsFaulted, Is.True);
                Assert.That(Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt"), Is.EqualTo(receipt));
                Assert.That(Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame"), Is.EqualTo(frame));
                Assert.That(Get<GameInputActions>(context.Router, "_actions").UI.Get().enabled, Is.False);
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void AC012_PostPublicationHubFaultDefersThenDisableClosesExactAdoptedActionsOnce()
        {
            var context = Create();
            try
            {
                Invoke(context.Adapter, "Start");
                context.Router.StepForTests();
                var actions = Get<GameInputActions>(context.Router, "_actions");
                Set(context.Router, "_hubFrameOrdinalProof", 99L);

                Assert.That(() => { var ignored = context.Router.CurrentUiFrame; }, Throws.InvalidOperationException);
                Assert.That(Get<GameInputActions>(context.Router, "_actions"), Is.SameAs(actions));
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.Zero);
                Invoke(context.Router, "OnDisable");
                Assert.That(Get<GameInputActions>(context.Router, "_actions"), Is.Null);
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                Assert.That(Get<int>(context.Router, "_disposeAttempts"), Is.EqualTo(1));
                Invoke(context.Router, "OnDestroy");
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void AC012_PostPublicationActionSubstitutionDisablesAndClosesOnlyTheAdoptedProof()
        {
            var context = Create();
            var foreign = new GameInputActions();
            try
            {
                Invoke(context.Adapter, "Start");
                context.Router.StepForTests();
                var adopted = Get<GameInputActions>(context.Router, "_preparedActionsProof");
                foreign.UI.Enable();
                Set(context.Router, "_actions", foreign);

                // Step reaches the strict prepared invariant first; a
                // post-publication substitution must still defer exact wrapper
                // closure until the subsequent lifecycle teardown.
                Assert.That(() => context.Router.StepForTests(), Throws.InvalidOperationException);
                Assert.That(adopted.Gameplay.Get().enabled, Is.False, "adopted Gameplay is contained");
                Assert.That(adopted.UI.Get().enabled, Is.False, "adopted UI is contained");
                Assert.That(foreign.UI.Get().enabled, Is.True, "foreign caller-owned UI is untouched by LatchFault");
                Assert.That(foreign.asset, Is.Not.Null, "foreign wrapper remains caller-owned");

                Invoke(context.Router, "OnDisable");
                Invoke(context.Router, "OnDestroy");
                Assert.That(Get<GameInputActions>(context.Router, "_lastClosedActions"), Is.SameAs(adopted));
                Assert.That(Get<GameInputActions>(context.Router, "_preparedActionsProof"), Is.SameAs(adopted), "forensic adopted proof is retained");
                Assert.That(Get<GameInputActions>(context.Router, "_preparedActionsContainmentProof"), Is.SameAs(adopted), "containment proof is retained");
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                Assert.That(Get<int>(context.Router, "_disposeAttempts"), Is.EqualTo(1));
                Assert.That(foreign.UI.Get().enabled, Is.True, "teardown does not close foreign wrapper");
                Assert.That(foreign.asset, Is.Not.Null, "foreign wrapper is not disposed");
            }
            finally { HubUiOnlyQ0TestSupport.DisposeCallerOwnedActions(foreign); context.Dispose(); }
        }

        [TestCase("_preparedActionsProof")]
        [TestCase("_preparedActionsContainmentProof")]
        public void AC012_OneAdoptedActionProofSubstitutionUsesTheTwoOfThreeOriginalForContainment(string corruptedProof)
        {
            var context = Create();
            var foreign = new GameInputActions();
            try
            {
                Invoke(context.Adapter, "Start");
                context.Router.StepForTests();
                var genuine = Get<GameInputActions>(context.Router, "_actions");
                foreign.UI.Enable();
                Set(context.Router, corruptedProof, foreign);

                Assert.That(() => context.Router.StepForTests(), Throws.InvalidOperationException, corruptedProof);
                Assert.That(genuine.Gameplay.Get().enabled, Is.False, corruptedProof + " genuine Gameplay contained");
                Assert.That(genuine.UI.Get().enabled, Is.False, corruptedProof + " genuine UI contained");
                Assert.That(foreign.UI.Get().enabled, Is.True, corruptedProof + " foreign UI untouched");
                Assert.That(foreign.asset, Is.Not.Null, corruptedProof + " foreign wrapper remains caller-owned");

                Invoke(context.Router, "OnDisable");
                Invoke(context.Router, "OnDestroy");
                Assert.That(Get<GameInputActions>(context.Router, "_lastClosedActions"), Is.SameAs(genuine), corruptedProof + " closes genuine wrapper");
                Assert.That(Get<GameInputActions>(context.Router, "_preparedActionsProof"), Is.SameAs(genuine), corruptedProof + " restored action proof");
                Assert.That(Get<GameInputActions>(context.Router, "_preparedActionsContainmentProof"), Is.SameAs(genuine), corruptedProof + " restored containment proof");
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1), corruptedProof + " exact-once closure");
                Assert.That(foreign.UI.Get().enabled, Is.True, corruptedProof + " foreign teardown untouched");
                Assert.That(foreign.asset, Is.Not.Null, corruptedProof + " foreign teardown undisposed");
            }
            finally { HubUiOnlyQ0TestSupport.DisposeCallerOwnedActions(foreign); context.Dispose(); }
        }

        [TestCase("_gameplayCallbacksProof")]
        [TestCase("_uiCallbacksProof")]
        public void AC009_OneCallbackProofSubstitutionStillUnsubscribesTheGenuineWrapperExactlyOnce(string corruptedProof)
        {
            var context = Create();
            var foreign = new GameInputActions();
            try
            {
                Invoke(context.Adapter, "Start");
                var genuine = Get<GameInputActions>(context.Router, "_actions");
                foreign.UI.Enable();
                Set(context.Router, corruptedProof, foreign);

                Assert.That(() => context.Router.StepForTests(), Throws.InvalidOperationException, corruptedProof);
                Assert.That(genuine.Gameplay.Get().enabled, Is.False, corruptedProof + " genuine Gameplay contained");
                Assert.That(genuine.UI.Get().enabled, Is.False, corruptedProof + " genuine UI contained");
                Assert.That(foreign.UI.Get().enabled, Is.True, corruptedProof + " foreign callback proof untouched");

                Invoke(context.Router, "OnDisable");
                Invoke(context.Router, "OnDestroy");
                Assert.That(Get<GameInputActions>(context.Router, "_lastClosedActions"), Is.SameAs(genuine), corruptedProof + " closes genuine wrapper");
                Assert.That(Get<int>(context.Router, "_gameplayUnsubscribeAttempts"), Is.EqualTo(1), corruptedProof + " gameplay exact-once");
                Assert.That(Get<int>(context.Router, "_uiUnsubscribeAttempts"), Is.EqualTo(1), corruptedProof + " UI exact-once");
                Assert.That(Get<GameInputActions>(context.Router, "_gameplayCallbacksProof"), Is.Null, corruptedProof + " gameplay proof cleared");
                Assert.That(Get<GameInputActions>(context.Router, "_uiCallbacksProof"), Is.Null, corruptedProof + " UI proof cleared");
                Assert.That(foreign.UI.Get().enabled, Is.True, corruptedProof + " foreign wrapper remains untouched");
                Assert.That(foreign.asset, Is.Not.Null, corruptedProof + " foreign wrapper undisposed");
            }
            finally { HubUiOnlyQ0TestSupport.DisposeCallerOwnedActions(foreign); context.Dispose(); }
        }

        [TestCase("_gameplayCallbacksRegistered")]
        [TestCase("_uiCallbacksRegistered")]
        public void AC009_CorruptOneCallbackBitStillUnsubscribesBothExactRegisteredMapsOnce(string clearedBit)
        {
            var context = Create();
            try
            {
                Invoke(context.Adapter, "Start");
                Set(context.Router, clearedBit, false);
                Set(context.Router, "_hubFrameOrdinalProof", 77L);

                Assert.That(() => { var ignored = context.Router.CurrentUiFrame; }, Throws.InvalidOperationException, clearedBit);
                Invoke(context.Router, "OnDisable");
                Invoke(context.Router, "OnDestroy");

                Assert.That(Get<int>(context.Router, "_gameplayUnsubscribeAttempts"), Is.EqualTo(1), clearedBit + " gameplay exact-once");
                Assert.That(Get<int>(context.Router, "_uiUnsubscribeAttempts"), Is.EqualTo(1), clearedBit + " UI exact-once");
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1), clearedBit + " close exact-once");
                Assert.That(Get<GameInputActions>(context.Router, "_gameplayCallbacksProof"), Is.Null, clearedBit + " gameplay proof cleared");
                Assert.That(Get<GameInputActions>(context.Router, "_uiCallbacksProof"), Is.Null, clearedBit + " UI proof cleared");
            }
            finally { context.Dispose(); }
        }

        [TestCase("adopted")]
        [TestCase("pending")]
        public void AC009_PrePublicationActionSubstitutionClosesOnlyAdoptedCandidateAndPublishesNothing(string phase)
        {
            var context = Create();
            var foreign = new GameInputActions();
            try
            {
                var adopted = Get<GameInputActions>(context.Router, "_preparedActionsProof");
                if (phase == "pending")
                {
                    typeof(InputRouter).GetMethod("InitializePreparedHub", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(context.Router, new object[] { context.Adapter });
                    Assert.That(Get<object>(context.Router, "_preparedLaunchState").ToString(), Is.EqualTo("InitializedPendingPublication"));
                }
                foreign.UI.Enable();
                Set(context.Router, "_actions", foreign);

                if (phase == "adopted")
                    Assert.That(() => Invoke(context.Adapter, "Start"), Throws.InstanceOf<TargetInvocationException>());
                else
                    Assert.That(() => typeof(InputRouter).GetMethod("ConfirmPreparedHubPublication", BindingFlags.Instance | BindingFlags.NonPublic)
                        .Invoke(context.Router, new object[] { context.Adapter }), Throws.InstanceOf<TargetInvocationException>());

                AssertNoPreConfirmationPublication(context);
                Assert.That(Get<object>(context.Router, "_preparedLaunchState").ToString(), Is.EqualTo("Failed"), phase + " uses legal existing failure row");
                Assert.That(Get<GameInputActions>(context.Router, "_lastClosedActions"), Is.SameAs(adopted), phase + " closes adopted proof");
                Assert.That(Get<GameInputActions>(context.Router, "_preparedActionsProof"), Is.SameAs(adopted), phase + " retains forensic proof");
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1), phase + " close exact-once");
                Assert.That(foreign.UI.Get().enabled, Is.True, phase + " foreign UI untouched");
                Assert.That(foreign.asset, Is.Not.Null, phase + " foreign wrapper undisposed");
                Invoke(context.Router, "OnDisable");
                Invoke(context.Router, "OnDestroy");
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1), phase + " disable/destroy remains idempotent");
            }
            finally { HubUiOnlyQ0TestSupport.DisposeCallerOwnedActions(foreign); context.Dispose(); }
        }

        [Test]
        public void AC006_RealDefaultProfileAdapterAndLatchPublishOneExactUiOnlyHandoff()
        {
            var context = Create();
            try
            {
                Invoke(context.Adapter, "Start");
                Invoke(context.Latch, "Start");
                Invoke(context.Latch, "Update");
                var handoff = context.Latch.CurrentHandoff;
                Assert.That(handoff.HasValue, Is.True);
                Assert.That(handoff.Value.InputMode, Is.EqualTo(InputMode.UIOnly));
                Assert.That(handoff.Value.GameplayEnabled, Is.False);
                Assert.That(handoff.Value.UiEnabled, Is.True);
            }
            finally { context.Dispose(); }
        }

        [TestCase("environment")]
        [TestCase("prepare")]
        [TestCase("take")]
        public void AC002_EnvironmentPrepareAndTakeFailuresLeaveRouterActionFree(string stage)
        {
            var recording = new HubUiOnlyQ0TestSupport.RecordingCoordinatorPreparationV1 { PreConsumeTake = stage == "take" };
            var preparation = stage == "take" ? (IDesktopProfileLaunchPreparationV1)recording : new ThrowingPreparation();
            var environment = stage == "environment" ? (IDesktopProfileLaunchEnvironmentV1)new ThrowingEnvironment() : new EnvironmentPort(Path.Combine(Path.GetTempPath(), "m5d7q0-failure-" + Guid.NewGuid().ToString("N")));
            if (stage == "environment") LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape("InvalidOperationException: environment")));
            if (stage == "prepare") LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape("InvalidOperationException: prepare")));
            if (stage == "take") LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape("InvalidOperationException: Prepared actions are no longer owned.")));
            LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape("InvalidOperationException: Hub handoff authored cohort is not exact.")));
            var context = Create(environment, preparation);
            try
            {
                AssertNoPreConfirmationPublication(context);
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.Zero, stage);
                Assert.That(recording.Created.Count, Is.EqualTo(stage == "take" ? 1 : 0), stage);
                Assert.That(recording.Applied.Count, Is.EqualTo(stage == "take" ? 1 : 0), stage);
                Assert.That(recording.Disposed.Count, Is.EqualTo(stage == "take" ? 1 : 0), stage);
                if (stage == "take")
                {
                    Assert.That(recording.Created[0], Is.SameAs(recording.Applied[0]));
                    Assert.That(recording.Created[0], Is.SameAs(recording.Disposed[0]));
                    AssertMapsDisabled(recording.Created[0]);
                }
            }
            finally { context.Dispose(); }
        }

        [TestCase(InputRouter.PreparedLaunchFaultInjectionStage.AfterGameplayCallbackRegistration)]
        [TestCase(InputRouter.PreparedLaunchFaultInjectionStage.BeforeUiCallbackRegistration)]
        [TestCase(InputRouter.PreparedLaunchFaultInjectionStage.AfterUiCallbackRegistration)]
        public void AC002_CallbackRegistrationFailureClosesAdoptedCandidateWithoutPublication(InputRouter.PreparedLaunchFaultInjectionStage stage)
        {
            var preparation = new HubUiOnlyQ0TestSupport.RecordingCoordinatorPreparationV1();
            var context = Create(new EnvironmentPort(Path.Combine(Path.GetTempPath(), "m5d7q0-callback-" + Guid.NewGuid().ToString("N")), true), preparation);
            try
            {
                var candidate = preparation.Actions.Created[0];
                context.Router.SetPreparedLaunchFaultStageForTests(stage);
                Assert.That(() => Invoke(context.Adapter, "Start"), Throws.InstanceOf<TargetInvocationException>());
                AssertNoPreConfirmationPublication(context);
                Assert.That(Get<GameInputActions>(context.Router, "_lastClosedActions"), Is.SameAs(candidate));
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                var expectedGameplayUnsubscribes = 1;
                var expectedUiUnsubscribes = stage == InputRouter.PreparedLaunchFaultInjectionStage.AfterUiCallbackRegistration ? 1 : 0;
                Assert.That(Get<int>(context.Router, "_gameplayUnsubscribeAttempts"), Is.EqualTo(expectedGameplayUnsubscribes), stage + " gameplay callback removal");
                Assert.That(Get<int>(context.Router, "_uiUnsubscribeAttempts"), Is.EqualTo(expectedUiUnsubscribes), stage + " UI callback removal");
                Assert.That(Get<GameInputActions>(context.Router, "_gameplayCallbacksProof"), Is.Null, stage + " gameplay callback proof cleared");
                Assert.That(Get<GameInputActions>(context.Router, "_uiCallbacksProof"), Is.Null, stage + " UI callback proof cleared");
                Assert.That(Get<GameInputActions>(context.Router, "_preparedActionsProof"), Is.SameAs(candidate), stage + " adopted proof retained for forensics");
                Invoke(context.Router, "OnDisable");
                Invoke(context.Router, "OnDestroy");
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1), stage + " teardown is idempotent");
                AssertMapsDisabled(candidate);
            }
            finally { context.Dispose(); }
        }

        [UnityTest]
        public IEnumerator AC002_AdoptRejectionLeavesCandidateDisposalWithAdapterExactlyOnce()
        {
            var preparation = new HubUiOnlyQ0TestSupport.RecordingCoordinatorPreparationV1 { RejectAdopt = true };
            LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape("InvalidOperationException: Prepared hub adoption is not authorized.")));
            LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape("InvalidOperationException: Hub handoff authored cohort is not exact.")));
            var context = Create(new EnvironmentPort(Path.Combine(Path.GetTempPath(), "m5d7q0-adopt-" + Guid.NewGuid().ToString("N")), true), preparation);
            try
            {
                var candidate = preparation.Actions.Created[0];
                AssertMapsDisabled(candidate);
                // GameInputActions.Dispose schedules Unity asset destruction.
                // Observe after that scheduled destruction, not synchronously.
                yield return null;
                preparation.Actions.ObserveAdapterDisposal(candidate);
                Assert.That(preparation.Actions.Created.Count, Is.EqualTo(1));
                Assert.That(preparation.Actions.Applied.Count, Is.EqualTo(1));
                Assert.That(preparation.Actions.Disposed.Count, Is.EqualTo(1));
                Assert.That(preparation.Actions.Created[0], Is.SameAs(preparation.Actions.Applied[0]));
                Assert.That(preparation.Actions.Created[0], Is.SameAs(preparation.Actions.Disposed[0]));
                AssertNoPreConfirmationPublication(context);
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(0));
            }
            finally { context.Dispose(); }
        }

        // AC-M5D7Q0-002 boundary ledger. The remaining adapter stages are
        // parameterized above; this table keeps the focused source auditable.
        [TestCase(DesktopProfileLaunchAdapterFaultStageV1.AfterInitializeProof)]
        [TestCase(DesktopProfileLaunchAdapterFaultStageV1.AfterReceiptStaging)]
        [TestCase(DesktopProfileLaunchAdapterFaultStageV1.AfterNotificationStaging)]
        [TestCase(DesktopProfileLaunchAdapterFaultStageV1.BeforeConfirm)]
        public void AC002_InitializationProofReceiptNotificationAndConfirmFailureHaveNoPublication(DesktopProfileLaunchAdapterFaultStageV1 stage)
        {
            var preparation = new HubUiOnlyQ0TestSupport.RecordingCoordinatorPreparationV1();
            var context = Create(new EnvironmentPort(Path.Combine(Path.GetTempPath(), "m5d7q0-stage-" + Guid.NewGuid().ToString("N")), true), preparation);
            try
            {
                var candidate = preparation.Actions.Created[0];
                context.Adapter.SetFaultStageForTests(stage);
                Assert.That(() => Invoke(context.Adapter, "Start"), Throws.InstanceOf<TargetInvocationException>());
                AssertNoPreConfirmationPublication(context);
                Assert.That(Get<GameInputActions>(context.Router, "_lastClosedActions"), Is.SameAs(candidate));
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                AssertMapsDisabled(candidate);
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void AC002_InitializationMatrixFailureClosesTheAdoptedCandidateBeforePublication()
        {
            var preparation = new HubUiOnlyQ0TestSupport.RecordingCoordinatorPreparationV1();
            var context = Create(new EnvironmentPort(Path.Combine(Path.GetTempPath(), "m5d7q0-initialize-" + Guid.NewGuid().ToString("N")), true), preparation);
            try
            {
                var candidate = preparation.Actions.Created[0];
                Set(context.Router, "_initialMode", InputMode.GameplayEnabled);
                Assert.That(() => Invoke(context.Adapter, "Start"), Throws.InstanceOf<TargetInvocationException>());
                AssertNoPreConfirmationPublication(context);
                Assert.That(Get<GameInputActions>(context.Router, "_lastClosedActions"), Is.SameAs(candidate));
                Assert.That(Get<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                AssertMapsDisabled(candidate);
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void AC002_DuplicateForeignAdapterReservationCannotCloseTheLegitimateOwnersCandidate()
        {
            var root = Path.Combine(Path.GetTempPath(), "m5d7q0-duplicate-" + Guid.NewGuid().ToString("N"));
            var host = new GameObject("m5d7q0-duplicate-root");
            host.SetActive(false);
            var firstHost = new GameObject("legitimate-adapter");
            var secondHost = new GameObject("foreign-adapter");
            firstHost.transform.SetParent(host.transform);
            secondHost.transform.SetParent(host.transform);
            try
            {
                var router = host.AddComponent<InputRouter>();
                var legitimate = firstHost.AddComponent<DesktopProfileLaunchAdapterV1>();
                var foreign = secondHost.AddComponent<DesktopProfileLaunchAdapterV1>();
                var winner = new HubUiOnlyQ0TestSupport.RecordingCoordinatorPreparationV1();
                var loser = new HubUiOnlyQ0TestSupport.RecordingCoordinatorPreparationV1();
                router.ConfigureHubUiOnlyForAuthoring();
                legitimate.ConfigureHubUiOnlyForAuthoring(router);
                foreign.ConfigureHubUiOnlyForAuthoring(router);
                legitimate.ConfigureForTests(router, new EnvironmentPort(root), winner);
                foreign.ConfigureForTests(router, new EnvironmentPort(root), loser);
                LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape("InvalidOperationException: Prepared hub reservation is not available.")));
                host.SetActive(true);

                var owner = Get<object>(router, "_preparedLaunchOwner");
                var winningAdapter = ReferenceEquals(owner, legitimate) ? legitimate : foreign;
                var winningPreparation = ReferenceEquals(owner, legitimate) ? winner : loser;
                var losingAdapter = ReferenceEquals(owner, legitimate) ? foreign : legitimate;
                var losingPreparation = ReferenceEquals(owner, legitimate) ? loser : winner;
                var candidate = winningPreparation.Actions.Created[0];
                Assert.That(winningPreparation.Actions.Created.Count, Is.EqualTo(1));
                Assert.That(losingPreparation.Actions.Created, Is.Empty);
                Assert.That(Get<GameInputActions>(router, "_actions"), Is.SameAs(candidate));
                AssertMapsDisabled(candidate);
                Invoke(losingAdapter, "OnDisable");
                Invoke(losingAdapter, "OnDestroy");
                Assert.That(Get<GameInputActions>(router, "_actions"), Is.SameAs(candidate));
                Assert.That(Get<int>(router, "_closeAttempts"), Is.Zero);

                Invoke(winningAdapter, "Start");
                Assert.That(winningAdapter.CurrentReceipt.HasValue, Is.True);
                Assert.That(Get<GameInputActions>(router, "_actions"), Is.SameAs(candidate));
                Assert.That(Get<int>(router, "_closeAttempts"), Is.Zero);
                Assert.That(candidate.Gameplay.Get().enabled, Is.False);
                Assert.That(candidate.UI.Get().enabled, Is.True);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(host);
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }

        private static void AssertNoPreConfirmationPublication(Context context)
        {
            ProfileLaunchNotificationV1 notification;
            Assert.That(context.Adapter.CurrentReceipt, Is.Null);
            Assert.That(context.Adapter.TryTakeNotification(out notification), Is.False);
            Assert.That(Get<InputFrameCommitReceipt?>(context.Router, "_currentReceipt"), Is.Null);
            Assert.That(Get<UiSemanticFrameV1?>(context.Router, "_currentUiFrame"), Is.Null);
            Assert.That(Get<GameInputActions>(context.Router, "_actions"), Is.Null);
            Assert.That(Get<bool>(context.Router, "_actionsClosed"), Is.True);
        }

        private static void AssertMapsDisabled(GameInputActions actions)
        {
            Assert.That(actions.Gameplay.Get().enabled, Is.False, "Gameplay map");
            Assert.That(actions.UI.Get().enabled, Is.False, "UI map");
        }

        private static Context Create() => Create(new EnvironmentPort(Path.Combine(Path.GetTempPath(), "m5d7q0-" + Guid.NewGuid().ToString("N")), true), new DesktopProfileLaunchPreparationPortV1());
        private static Context Create(IDesktopProfileLaunchEnvironmentV1 environment, IDesktopProfileLaunchPreparationV1 preparation)
        {
            var root = environment is EnvironmentPort port ? port.Root : Path.Combine(Path.GetTempPath(), "m5d7q0-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            var host = new GameObject("m5d7q0-hub-runtime");
            host.SetActive(false);
            var router = host.AddComponent<InputRouter>();
            var adapter = host.AddComponent<DesktopProfileLaunchAdapterV1>();
            var latch = host.AddComponent<HubEntryHandoffLatchV1>();
            router.ConfigureHubUiOnlyForAuthoring();
            adapter.ConfigureHubUiOnlyForAuthoring(router);
            adapter.ConfigureForTests(router, environment, preparation);
            if (preparation is StableAdoptFailurePreparation stable) { stable.Router = router; stable.Owner = adapter; }
            if (preparation is HubUiOnlyQ0TestSupport.RecordingCoordinatorPreparationV1 recording) { recording.Router = router; recording.Owner = adapter; }
            latch.ConfigureHubUiOnlyForAuthoring(adapter, router);
            host.SetActive(true);
            return new Context(host, root, router, adapter, latch);
        }

        private static void Invoke(object target, string name) => target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
        private static T Get<T>(object target, string name) => (T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
        private static void Set(object target, string name, object value) => target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);

        private sealed class EnvironmentPort : IDesktopProfileLaunchEnvironmentV1
        {
            private readonly string _root;
            internal string Root => _root;
            internal EnvironmentPort(string root, bool unused = false) { _root = root; }
            public string GetPersistentDataPath() => _root;
            public DateTimeOffset GetUtcNow() => new DateTimeOffset(2026, 9, 20, 0, 0, 0, TimeSpan.Zero);
        }

        private sealed class ThrowingEnvironment : IDesktopProfileLaunchEnvironmentV1
        { public string GetPersistentDataPath() { throw new InvalidOperationException("environment"); } public DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow; }
        private sealed class ThrowingPreparation : IDesktopProfileLaunchPreparationV1
        { public PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc) { throw new InvalidOperationException("prepare"); } }
        private sealed class PreConsumedPreparation : IDesktopProfileLaunchPreparationV1
        {
            private readonly DesktopProfileLaunchPreparationPortV1 _inner = new DesktopProfileLaunchPreparationPortV1();
            public PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc)
            { var token = _inner.Prepare(root, utc); var actions = token.TakeActions(); HubUiOnlyQ0TestSupport.DisposeCallerOwnedActions(actions); return token; }
        }
        private sealed class StableAdoptFailurePreparation : IDesktopProfileLaunchPreparationV1
        {
            private readonly DesktopProfileLaunchPreparationPortV1 _inner = new DesktopProfileLaunchPreparationPortV1();
            internal InputRouter Router; internal object Owner;
            public PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc)
            {
                var token = _inner.Prepare(root, utc);
                if (Router != null && Owner != null) Router.FailPreparedHubLaunch(Owner);
                return token;
            }
        }

        private sealed class Context : IDisposable
        {
            private readonly GameObject _host;
            private readonly string _root;
            internal Context(GameObject host, string root, InputRouter router, DesktopProfileLaunchAdapterV1 adapter, HubEntryHandoffLatchV1 latch)
            { _host = host; _root = root; Router = router; Adapter = adapter; Latch = latch; }
            internal InputRouter Router { get; }
            internal DesktopProfileLaunchAdapterV1 Adapter { get; }
            internal HubEntryHandoffLatchV1 Latch { get; }
            public void Dispose()
            {
                if (_host != null) UnityEngine.Object.DestroyImmediate(_host);
                if (Directory.Exists(_root)) Directory.Delete(_root, true);
            }
        }
    }
}
