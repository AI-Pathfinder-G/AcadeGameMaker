using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using static AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-UX-004/008; AC-M5B5-002/007/008. Root integration probes supplement
    // Luna's independently authored adversarial fixtures, never replace review.
    internal sealed class InputRouterFailureBoundaryPlayModeTests : InputTestFixture
    {
        [UnityEngine.SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _runtimeFixture=new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnityEngine.TestTools.UnitySetUp] public System.Collections.IEnumerator BeginRuntimeCase(){_runtimeFixture.PrepareEnter();yield return new UnityEngine.TestTools.EnterPlayMode();_runtimeFixture.ConfirmEntered();yield return _runtimeFixture.RunBody(_runtimeFixture.BeginCase(false));}
        public override void TearDown(){}
        [UnityEngine.TestTools.UnityTearDown] public System.Collections.IEnumerator CloseRuntimeCase(){try{if(_runtimeFixture.OwnsRuntimeFixture && UnityEngine.Application.isPlaying)yield return _runtimeFixture.RunBody(_runtimeFixture.CloseCase(),true);}finally{_runtimeFixture.CaptureCleanupFailure(()=>base.TearDown());}if(_runtimeFixture.OwnsRuntimeFixture){if(UnityEngine.Application.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();_runtimeFixture.CaptureCleanupFailure(_runtimeFixture.CompleteExit);}_runtimeFixture.ThrowCleanupFailure();}
        

        [TestCase("stale")]
        [TestCase("future")]
        [TestCase("ordinal")]
        [TestCase("epoch")]
        [TestCase("mode")]
        public void ActualRouterSourceMismatchCannotAuthorizeAnyPendingConsumer(string mismatch)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled);
                router.StepForTests(); AdvanceFrame(rig);
                var stale = router.CurrentReceipt.Value;
                router.StepForTests();
                var exact = router.CurrentReceipt.Value;
                var wrong = mismatch == "stale" ? stale : new InputFrameCommitReceipt(
                    new SimulationTick(mismatch == "future" ? 2 : 1), mismatch == "ordinal" ? 9 : 1,
                    mismatch == "epoch" ? 1 : 0, mismatch == "mode" ? InputMode.UIOnly : InputMode.GameplayEnabled);
                typeof(InputRouter).GetField("_currentReceipt", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(router, wrong);
                var sinks = new object[] { rig.Player, rig.Transfer, rig.Combat };
                var generations = new long[3];
                for (var i = 0; i < sinks.Length; i++) generations[i] = GetPrivate<long>(sinks[i], "_inputGeneration");
                Assert.Throws<InvalidOperationException>(() => rig.Transfer.AdvanceCoordinatedInputPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Player.AdvanceOneFixedTickForTests());
                for (var i = 0; i < sinks.Length; i++)
                {
                    Assert.That(GetPrivate<long>(sinks[i], "_inputGeneration"), Is.EqualTo(generations[i]));
                    Assert.That(GetPrivate<InputFrameCommitReceipt?>(sinks[i], "_pendingCoordinatedReceipt"), Is.EqualTo(exact));
                }
                Assert.That(rig.Player.NextExpectedTick.Value, Is.EqualTo(1));
                Assert.That(rig.Combat.NextExpectedTick.Value, Is.EqualTo(1));
                // REQ-M5D7PA-006: a reflected single-field receipt forgery is
                // now a terminal UiCapture boundary.  Restoring the evidence
                // cannot reopen the source or authorize the pending batch.
                Assert.That(router.IsFaulted, Is.True);
                Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.UiCapture));
                typeof(InputRouter).GetField("_currentReceipt", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(router, exact);
                Assert.Throws<InvalidOperationException>(() => AdvanceFrame(rig));
                Assert.That(rig.Player.NextExpectedTick.Value, Is.EqualTo(1));
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void ActualRouterReceiptCannotBeConsumedTwiceByAnyPhase()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled);
                router.StepForTests();
                rig.Transfer.AdvanceCoordinatedInputPhaseForTests();
                Assert.Throws<InvalidOperationException>(() => rig.Transfer.AdvanceCoordinatedInputPhaseForTests());
                rig.Combat.AdvanceCombatPhaseForTests();
                Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
                rig.Bridge.AdvanceBridgePhaseForTests();
                rig.Player.AdvanceOneFixedTickForTests();
                Assert.Throws<InvalidOperationException>(() => rig.Player.AdvanceOneFixedTickForTests());
                Assert.That(rig.Player.NextExpectedTick.Value, Is.EqualTo(1));
                Assert.That(rig.Combat.NextExpectedTick.Value, Is.EqualTo(1));
                Assert.That(router.CurrentReceipt.Value.FrameOrdinal, Is.Zero);
            }
            finally { DestroyAll(objects); }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void ForeignSourceWithMatchingReceiptCannotReplaceRegisteredRouter(int sinkIndex)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled);
                router.StepForTests();
                var foreign = new ForeignReceiptSource(router.CurrentReceipt.Value);
                var tick = rig.Player.NextExpectedTick;
                if (sinkIndex == 0) Assert.Throws<InvalidOperationException>(() => rig.Player.RegisterCoordinatedInputOwner(router, foreign, tick));
                if (sinkIndex == 1) Assert.Throws<InvalidOperationException>(() => rig.Transfer.RegisterCoordinatedInputOwner(router, foreign, tick));
                if (sinkIndex == 2) Assert.Throws<InvalidOperationException>(() => rig.Combat.RegisterCoordinatedInputOwner(router, foreign, tick));
                foreach (var sink in new object[] { rig.Player, rig.Transfer, rig.Combat })
                    Assert.That(GetPrivate<object>(sink, "_coordinatedInputSource"), Is.SameAs(router));
                typeof(InputRouter).GetField("_currentReceipt", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(router, null);
                Assert.That(foreign.CurrentReceipt.HasValue, Is.True);
                Assert.Throws<InvalidOperationException>(() => rig.Transfer.AdvanceCoordinatedInputPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Player.AdvanceOneFixedTickForTests());
            }
            finally { DestroyAll(objects); }
        }

        private sealed class ForeignReceiptSource : IInputFrameCommitSource
        {
            internal ForeignReceiptSource(InputFrameCommitReceipt receipt) { CurrentReceipt = receipt; }
            public InputFrameCommitReceipt? CurrentReceipt { get; }
            public bool IsFaulted => false;
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void ConflictingGateOwnerCannotPartiallyRegisterTheOtherConsumers(int conflictingSink)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var previous = new object();
                if (conflictingSink == 0) rig.Player.RegisterInputGateOwner(previous);
                if (conflictingSink == 1) rig.Transfer.RegisterInputGateOwner(previous);
                if (conflictingSink == 2) rig.Combat.RegisterInputGateOwner(previous);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled, false);
                Assert.Throws<InvalidOperationException>(() => router.InitializeForTests());
                var sinks = new object[] { rig.Player, rig.Transfer, rig.Combat };
                for (var index = 0; index < sinks.Length; index++)
                {
                    Assert.That(GetPrivate<object>(sinks[index], "_inputGateOwner"), Is.SameAs(index == conflictingSink ? previous : null));
                    Assert.That(GetPrivate<object>(sinks[index], "_coordinatedInputSource"), Is.Null);
                }
                Assert.That(router.CurrentReceipt.HasValue, Is.False);
                Assert.That(GetPrivate<AcadeGameMaker.Input.GameInputActions>(router, "_actions").Gameplay.enabled, Is.False);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void FirstReceiptEstablishesAuthoredModeBeforeAnyDifferentModeRequest()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.Transition);
                var requester = new object(); router.RegisterModeRequester(requester);
                Assert.Throws<InvalidOperationException>(() => router.RequestMode(requester, InputMode.GameplayEnabled, "too-early", rig.Player.NextExpectedTick));
                Assert.That(router.CurrentReceipt.HasValue, Is.False);
                Assert.That(router.EffectiveMode, Is.EqualTo(InputMode.Transition));
                router.RequestMode(requester, InputMode.Transition, "bootstrap", rig.Player.NextExpectedTick);
                router.RequestMode(requester, InputMode.Transition, "bootstrap", rig.Player.NextExpectedTick);
                router.StepForTests();
                Assert.That(router.CurrentReceipt.Value.ModeEpoch, Is.Zero);
                Assert.That(router.CurrentReceipt.Value.Mode, Is.EqualTo(InputMode.Transition));
                AdvanceFrame(rig);
                router.RequestMode(requester, InputMode.GameplayEnabled, "ready", rig.Player.NextExpectedTick);
                router.StepForTests();
                Assert.That(router.CurrentReceipt.Value.ModeEpoch, Is.EqualTo(1));
                Assert.That(router.EffectiveMode, Is.EqualTo(InputMode.GameplayEnabled));
            }
            finally { DestroyAll(objects); }
        }

        [TestCase(InputRouterFaultStage.PrepareMovement, false)]
        [TestCase(InputRouterFaultStage.PrepareTransfer, false)]
        [TestCase(InputRouterFaultStage.PrepareCombat, false)]
        [TestCase(InputRouterFaultStage.ValidateMovement, false)]
        [TestCase(InputRouterFaultStage.ValidateTransfer, false)]
        [TestCase(InputRouterFaultStage.ValidateCombat, false)]
        [TestCase(InputRouterFaultStage.AfterMovementCommit, true)]
        [TestCase(InputRouterFaultStage.AfterTransferCommit, true)]
        [TestCase(InputRouterFaultStage.AfterCombatCommit, true)]
        [TestCase(InputRouterFaultStage.MapsDisabled, true)]
        [TestCase(InputRouterFaultStage.TargetMapEnabled, true)]
        public void EveryFailureBoundaryBlocksAllPhasesAndPreservesGlobalProof(InputRouterFaultStage stage, bool terminal)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled);
                router.StepForTests(); AdvanceFrame(rig);
                var before = router.CurrentReceipt;
                var keyboard = InputSystem.AddDevice<Keyboard>();
                Press(keyboard.spaceKey);
                var requester = new object();
                router.RegisterModeRequester(requester);
                router.RequestMode(requester, InputMode.UIOnly, "fault-boundary", rig.Player.NextExpectedTick);
                var mg = GetPrivate<long>(rig.Player, "_inputGeneration");
                var tg = GetPrivate<long>(rig.Transfer, "_inputGeneration");
                var cg = GetPrivate<long>(rig.Combat, "_inputGeneration");
                router.SetFaultStageForTests(stage);
                Assert.Throws<InvalidOperationException>(() => router.StepForTests());
                Assert.That(router.CurrentReceipt, Is.EqualTo(before));
                Assert.That(router.EffectiveMode, Is.EqualTo(InputMode.GameplayEnabled));
                Assert.That(router.IsFaulted, Is.EqualTo(terminal));
                Assert.That(GetPrivate<bool>(router, "_jumpPressed"), Is.True, "Failed frame must retain frozen input.");
                Assert.That(GetPrivate<long>(router, "_nextAimSampleId"), Is.Zero);
                Assert.That(GetPrivate<long>(router, "_nextAttackSequenceId"), Is.Zero);
                Assert.Throws<InvalidOperationException>(() => rig.Transfer.AdvanceCoordinatedInputPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Player.AdvanceOneFixedTickForTests());
                Assert.That(rig.Player.NextExpectedTick.Value, Is.EqualTo(1));
                Assert.That(rig.Combat.NextExpectedTick.Value, Is.EqualTo(1));
                if (terminal)
                {
                    Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(stage));
                    Assert.That(router.LastDiagnostic.GameplayMapEnabled, Is.False);
                    Assert.That(router.LastDiagnostic.UiMapEnabled, Is.False);
                    router.SetFaultStageForTests(null);
                    Assert.Throws<InvalidOperationException>(() => router.StepForTests());
                }
                else
                {
                    Assert.That(GetPrivate<long>(rig.Player, "_inputGeneration"), Is.EqualTo(mg));
                    Assert.That(GetPrivate<long>(rig.Transfer, "_inputGeneration"), Is.EqualTo(tg));
                    Assert.That(GetPrivate<long>(rig.Combat, "_inputGeneration"), Is.EqualTo(cg));
                    router.SetFaultStageForTests(null);
                    Assert.DoesNotThrow(() => router.StepForTests());
                    Assert.That(router.CurrentReceipt.Value.Tick.Value, Is.EqualTo(1));
                    Assert.That(router.CurrentReceipt.Value.ModeEpoch, Is.EqualTo(1));
                    Assert.That(router.LastDiagnostic.GameplayMapEnabled, Is.False);
                    Assert.That(router.LastDiagnostic.UiMapEnabled, Is.True);
                    AdvanceFrame(rig);
                }
            }
            finally { DestroyAll(objects); }
        }

        [TestCase("_frameOrdinal")]
        [TestCase("_modeEpoch")]
        public void ReceiptCounterOverflowOccursBeforeAnyLocalCommit(string counter)
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled);
                router.StepForTests(); AdvanceFrame(rig);
                var previous = router.CurrentReceipt;
                typeof(InputRouter).GetField(counter, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(router, long.MaxValue);
                var requester = new object(); router.RegisterModeRequester(requester);
                router.RequestMode(requester, InputMode.Transition, "overflow", rig.Player.NextExpectedTick);
                var mg = GetPrivate<long>(rig.Player, "_inputGeneration");
                var tg = GetPrivate<long>(rig.Transfer, "_inputGeneration");
                var cg = GetPrivate<long>(rig.Combat, "_inputGeneration");
                Assert.Throws<OverflowException>(() => router.StepForTests());
                Assert.That(router.CurrentReceipt, Is.EqualTo(previous));
                Assert.That(router.IsFaulted, Is.False);
                Assert.That(GetPrivate<long>(rig.Player, "_inputGeneration"), Is.EqualTo(mg));
                Assert.That(GetPrivate<long>(rig.Transfer, "_inputGeneration"), Is.EqualTo(tg));
                Assert.That(GetPrivate<long>(rig.Combat, "_inputGeneration"), Is.EqualTo(cg));
                Assert.That(rig.Player.IsInputGateLocked, Is.False);
                Assert.That(rig.Transfer.IsInputGateLocked, Is.False);
                Assert.That(rig.Combat.IsInputGateLocked, Is.False);
            }
            finally { DestroyAll(objects); }
        }

        [Test]
        public void DisableLatchesFaultWithoutConsumingAlreadyPublishedPendingProof()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled);
                router.StepForTests();
                var receipt = router.CurrentReceipt;
                router.enabled = false;
                Assert.That(router.IsFaulted, Is.True);
                Assert.That(router.CurrentReceipt, Is.EqualTo(receipt));
                Assert.That(router.LastDiagnostic.Fault, Is.EqualTo(InputRouterFaultStage.Disabled));
                Assert.Throws<InvalidOperationException>(() => rig.Transfer.AdvanceCoordinatedInputPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Player.AdvanceOneFixedTickForTests());
                Assert.That(rig.Player.NextExpectedTick.Value, Is.Zero);
                Assert.Throws<InvalidOperationException>(() => router.StepForTests());
                LogAssert.Expect(LogType.Exception, new Regex("InvalidOperationException: An initialized InputRouter cannot be re-enabled"));
                router.enabled = true;
                Assert.That(router.IsFaulted, Is.True);
                Assert.That(router.CurrentReceipt, Is.EqualTo(receipt));
                Assert.Throws<InvalidOperationException>(() => router.StepForTests());
            }
            finally { DestroyAll(objects); }
        }
    }
}
