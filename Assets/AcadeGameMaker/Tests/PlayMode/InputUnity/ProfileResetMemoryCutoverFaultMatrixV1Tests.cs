using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-M5D7QC2-002..006/007; AC-M5D7QC2-002..006/008.  Every row owns a
    // new real hub/C1 root.  The controls only throw at production boundaries
    // or wrap the exact object supplied by production.
    internal sealed class ProfileResetMemoryCutoverFaultMatrixV1Tests
    {
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeStaging)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterCandidateCreate)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeBindingApply)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterBindingApply)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeTerminalGate)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterTerminalGate)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeTransfer)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterTransfer)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeOldDispose)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterOldDispose)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeCell)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterCell)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforePair)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterPair)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeDelete)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterDelete)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeFinalProbe)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterFinalProbe)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeReceiptStage)]
        public void AC_M5D7QC2_002_Through006_EachCutoverCheckpointClosesFreshCohort(ProfileResetMemoryCutoverCheckpointV1 point)
        {
            using (var fixture = Fixture.Create("checkpoint-" + point))
            {
                var history = fixture.Adapter.CurrentReceipt;
                var disk = fixture.PrepareDisk();
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, new ThrowAt(point));
                AssertTerminalFailure(fixture, result, history, DeletedBefore(point));
            }
        }

        [TestCase(ResetCutoverOperationV1.CancelUiEnableQuarantine, false)]
        [TestCase(ResetCutoverOperationV1.CancelUiEnableQuarantine, true)]
        [TestCase(ResetCutoverOperationV1.DisableOldGameplay, false)]
        [TestCase(ResetCutoverOperationV1.DisableOldGameplay, true)]
        [TestCase(ResetCutoverOperationV1.DisableOldUi, false)]
        [TestCase(ResetCutoverOperationV1.DisableOldUi, true)]
        [TestCase(ResetCutoverOperationV1.RemoveOldGameplayCallbacks, false)]
        [TestCase(ResetCutoverOperationV1.RemoveOldGameplayCallbacks, true)]
        [TestCase(ResetCutoverOperationV1.RemoveOldUiCallbacks, false)]
        [TestCase(ResetCutoverOperationV1.RemoveOldUiCallbacks, true)]
        [TestCase(ResetCutoverOperationV1.DisposeOld, false)]
        [TestCase(ResetCutoverOperationV1.DisposeOld, true)]
        [TestCase(ResetCutoverOperationV1.DisableNewGameplay, false)]
        [TestCase(ResetCutoverOperationV1.DisableNewGameplay, true)]
        [TestCase(ResetCutoverOperationV1.DisableNewUi, false)]
        [TestCase(ResetCutoverOperationV1.DisableNewUi, true)]
        [TestCase(ResetCutoverOperationV1.RemoveNewGameplayCallbacks, false)]
        [TestCase(ResetCutoverOperationV1.RemoveNewGameplayCallbacks, true)]
        [TestCase(ResetCutoverOperationV1.RemoveNewUiCallbacks, false)]
        [TestCase(ResetCutoverOperationV1.RemoveNewUiCallbacks, true)]
        public void AC_M5D7QC2_003_004_EachExactOwnedOperationFaultClosesFreshCohort(ResetCutoverOperationV1 operation, bool after)
        {
            using (var fixture = Fixture.Create("operation-" + operation + "-" + after))
            {
                var history = fixture.Adapter.CurrentReceipt;
                var disk = fixture.PrepareDisk(); var control = new OperationFault(operation, after, false);
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, control);
                AssertTerminalFailure(fixture, result, history, false);
                Assert.That(control.WrapperCalls, Is.EqualTo(1));
                Assert.That(control.ExactCalls, Is.EqualTo(after ? 1 : 0));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void AC_M5D7QC2_002_004_DisposeNewCleanupFaultIsNotAReplacementOperation(bool after)
        {
            using (var fixture = Fixture.Create("dispose-new-" + after))
            {
                var history = fixture.Adapter.CurrentReceipt;
                var disk = fixture.PrepareDisk(); var control = new OperationFault(ResetCutoverOperationV1.DisposeNew, after, true);
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, control);
                AssertTerminalFailure(fixture, result, history, false);
                Assert.That(control.WrapperCalls, Is.EqualTo(1));
                Assert.That(control.ExactCalls, Is.EqualTo(after ? 1 : 0));
            }
        }

        [Test]
        public void AC_M5D7QC2_004_ReflectedStagedDisposeDiagnosticTrueCannotSuppressPrivateCloseAttempt()
        {
            using (var fixture = Fixture.Create("staged-dispose-diagnostic-true"))
            {
                var history = fixture.Adapter.CurrentReceipt; var disk = fixture.PrepareDisk(); var control = new StagedDisposeDiagnosticFault(fixture.Adapter, true);
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, control);
                AssertTerminalFailure(fixture, result, history, false);
                Assert.That(control.DisposeNewCalls, Is.EqualTo(1), "the private candidate witness, not a reflected diagnostic bit, authorizes its one close attempt");
                Assert.That(control.ExactDisposeCalls, Is.EqualTo(1));
            }
        }

        [Test]
        public void AC_M5D7QC2_004_ReflectedStagedDisposeDiagnosticFalseCannotReplayOwnedClose()
        {
            using (var fixture = Fixture.Create("staged-dispose-diagnostic-false"))
            {
                var history = fixture.Adapter.CurrentReceipt; var disk = fixture.PrepareDisk(); var control = new StagedDisposeDiagnosticFault(fixture.Adapter, false);
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, control);
                AssertTerminalFailure(fixture, result, history, false);
                Assert.That(control.DisposeNewCalls, Is.EqualTo(1)); Assert.That(control.ExactDisposeCalls, Is.EqualTo(1));
                typeof(DesktopProfileLaunchAdapterV1).GetField("_untransferredCandidateDisposeAttempted",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(fixture.Adapter,false);
                Assert.Throws<InvalidOperationException>(() => fixture.Adapter.DisposeUntransferredResetCandidate(control.Candidate, control));
                Assert.That(control.DisposeNewCalls, Is.EqualTo(1)); Assert.That(control.ExactDisposeCalls, Is.EqualTo(1));
            }
        }

        [TestCase(ProfileResetMemoryAuthorityCheckpointV1.BeforeProofCas, false)]
        [TestCase(ProfileResetMemoryAuthorityCheckpointV1.AfterProofCas, false)]
        [TestCase(ProfileResetMemoryAuthorityCheckpointV1.BeforeBarrierDelete, false)]
        [TestCase(ProfileResetMemoryAuthorityCheckpointV1.AfterBarrierDelete, true)]
        [TestCase(ProfileResetMemoryAuthorityCheckpointV1.BeforeBarrierAbsenceProbe, true)]
        [TestCase(ProfileResetMemoryAuthorityCheckpointV1.AfterBarrierAbsenceProbe, true)]
        [TestCase(ProfileResetMemoryAuthorityCheckpointV1.BeforeFinalDiskProbe, true)]
        [TestCase(ProfileResetMemoryAuthorityCheckpointV1.AfterFinalDiskProbe, true)]
        public void AC_M5D7QC2_005_006_ProfileAuthorityBoundaryFaultsKeepCorrectBarrierFact(ProfileResetMemoryAuthorityCheckpointV1 point, bool deleted)
        {
            using (var fixture = Fixture.Create("authority-" + point))
            {
                var history = fixture.Adapter.CurrentReceipt; var disk = fixture.PrepareDisk();
                ProfileResetMemoryResultV1 result=null;
                try { result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, new AuthorityThrow(point)); }
                catch (InvalidOperationException) { }
                AssertTerminalFailure(fixture, result, history, deleted);
            }
        }

        [Test]
        public void AC_M5D7QC2_005_ActualLockedMarkerDeleteReturnsReloadAndRetainsBarrier()
        {
            using (var fixture = Fixture.Create("locked-marker"))
            {
                var history = fixture.Adapter.CurrentReceipt; var disk = fixture.PrepareDisk();
                using (var control = new LockMarkerAtDelete(fixture.Root))
                {
                    var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, control);
                    AssertTerminalFailure(fixture, result, history, false);
                    Assert.That(control.Opened, Is.True);
                }
            }
        }

        [Test]
        public void AC_M5D7QC2_004_006_PostDeleteForeignActionMutationFailsPairWithoutBarrierRecreation()
        {
            using (var fixture = Fixture.Create("post-delete-memory-original"))
            using (var foreign = Fixture.Create("post-delete-memory-foreign"))
            {
                var history = fixture.Adapter.CurrentReceipt; var disk = fixture.PrepareDisk();
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, new PostDeleteActionCorruption(fixture.Router, foreign.Actions));
                AssertTerminalFailure(fixture, result, history, true);
                Assert.That(foreign.Actions.UI.Get().enabled, Is.True, "hostile original metadata cannot contain a foreign actual cohort");
            }
        }

        [TestCase("primary")]
        [TestCase("previous")]
        [TestCase("temp")]
        [TestCase("barrier")]
        [TestCase("barrier-temp")]
        [TestCase("archive-changed")]
        [TestCase("archive-missing")]
        public void AC_M5D7QC2_006_ActualPostDeleteDiskMutationCannotPublishReceipt(string mutation)
        {
            using (var fixture = Fixture.Create("final-disk-" + mutation))
            {
                var history=fixture.Adapter.CurrentReceipt; var disk=fixture.PrepareDisk();
                var control=new PostDeleteDiskMutation(fixture.Root,disk.TransactionId,mutation);
                var result=ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root,disk.PreparedProof,fixture.Adapter,fixture.Router,control);
                AssertTerminalFailure(fixture,result,history,mutation!="barrier",true,mutation=="barrier-temp");
                if(mutation=="barrier") Assert.That(File.Exists(Path.Combine(fixture.Root,ProfileResetBarrierV1.BarrierName)),Is.True);
                if(mutation=="barrier-temp") Assert.That(File.Exists(Path.Combine(fixture.Root,ProfileResetBarrierV1.BarrierTempName)),Is.True);
            }
        }

        [TestCase("changed")]
        [TestCase("missing")]
        [TestCase("directory")]
        public void AC_M5D7QC2_005_ActualPreDeleteMarkerMutationClosesWithoutFalseAbsence(string mutation)
        {
            using (var fixture=Fixture.Create("marker-"+mutation))
            {
                var history=fixture.Adapter.CurrentReceipt; var disk=fixture.PrepareDisk(); var marker=Path.Combine(fixture.Root,ProfileResetBarrierV1.BarrierName);
                if(mutation=="changed") File.WriteAllBytes(marker,new byte[]{0x41});
                else if(mutation=="missing") File.Delete(marker);
                else { File.Delete(marker); Directory.CreateDirectory(marker); }
                var result=ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root,disk.PreparedProof,fixture.Adapter,fixture.Router);
                Assert.That(result.Outcome,Is.Not.EqualTo(ProfileResetMemoryOutcomeV1.Completed)); Assert.That(result.Receipt,Is.Null); Assert.That(fixture.Adapter.IsResetCutoverTerminal,Is.True);
                Assert.That(fixture.Adapter.CurrentReceipt,Is.EqualTo(history));
                if(mutation=="missing")Assert.That(File.Exists(marker),Is.False,"the coordinator must not restore a missing pre-delete marker");
                else Assert.That(mutation=="directory" ? Directory.Exists(marker) : File.Exists(marker),Is.True,"the coordinator never invents a different marker state before delete");
            }
        }

        [TestCase("changed")]
        [TestCase("missing")]
        [TestCase("directory")]
        public void AC_M5D7QC2_005_AuthorizedPairBeforeDeleteMarkerMutationFailsWithoutRecreation(string mutation)
        {
            using(var fixture=Fixture.Create("before-delete-marker-"+mutation))
            {
                var history=fixture.Adapter.CurrentReceipt;var disk=fixture.PrepareDisk();var marker=Path.Combine(fixture.Root,ProfileResetBarrierV1.BarrierName);
                var result=ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root,disk.PreparedProof,fixture.Adapter,fixture.Router,new BeforeDeleteMarkerMutation(marker,mutation));
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetMemoryOutcomeV1.ReloadRequired));Assert.That(result.Receipt,Is.Null);Assert.That(fixture.Adapter.IsResetCutoverTerminal,Is.True);Assert.That(fixture.Adapter.CurrentReceipt,Is.EqualTo(history));
                if(mutation=="missing")Assert.That(File.Exists(marker),Is.False);else Assert.That(mutation=="directory"?Directory.Exists(marker):File.Exists(marker),Is.True);
            }
        }

        [Test]
        public void AC_M5D7QC2_005_ActualBarrierJunctionClosesOriginalCohortWithoutTargetMutation()
        {
            using (var native=new NativeJunctionRoot())
            {
                var fixture=Fixture.CreateAt(native.Root);
                try
                {
                    var disk=fixture.PrepareDisk(); var marker=Path.Combine(native.Root,ProfileResetBarrierV1.BarrierName); File.Delete(marker); native.CreateJunction(marker,native.Target);
                    var result=ProfileResetMemoryCutoverV1.FinalizeReset(native.Root,disk.PreparedProof,fixture.Adapter,fixture.Router);
                    Assert.That(result.Outcome,Is.Not.EqualTo(ProfileResetMemoryOutcomeV1.Completed)); Assert.That(result.Receipt,Is.Null); Assert.That(fixture.Adapter.IsResetCutoverTerminal,Is.True);
                    native.AssertJunction(marker); native.AssertSentinel();
                }
                finally { fixture.DestroyRuntimeOnly(); }
            }
        }

        [Test]
        public void AC_M5D7QC2_005_AuthorizedPairBeforeDeleteBarrierJunctionFailsWithoutTargetMutation()
        {
            using(var native=new NativeJunctionRoot())
            {
                var fixture=Fixture.CreateAt(native.Root);
                try
                {
                    var history=fixture.Adapter.CurrentReceipt;var disk=fixture.PrepareDisk();var marker=Path.Combine(native.Root,ProfileResetBarrierV1.BarrierName);
                    var result=ProfileResetMemoryCutoverV1.FinalizeReset(native.Root,disk.PreparedProof,fixture.Adapter,fixture.Router,new BeforeDeleteJunction(marker,native));
                    Assert.That(result.Outcome,Is.EqualTo(ProfileResetMemoryOutcomeV1.ReloadRequired));Assert.That(result.Receipt,Is.Null);Assert.That(fixture.Adapter.IsResetCutoverTerminal,Is.True);Assert.That(fixture.Adapter.CurrentReceipt,Is.EqualTo(history));native.AssertJunction(marker);native.AssertSentinel();
                }
                finally{fixture.DestroyRuntimeOnly();}
            }
        }

        [Test]
        public void AC_M5D7QC2_009_OrdinaryFixtureCleanupRefusesNestedJunctionBeforeNativeOwnerRemovesIt()
        {
            var native=new NativeJunctionRoot();
            try
            {
                var nested=Path.Combine(native.Root,"nested");Directory.CreateDirectory(nested);var link=Path.Combine(nested,"link");native.CreateJunction(link,native.Target);
                var cleanup=typeof(Fixture).GetMethod("CleanupOwnedRoot",BindingFlags.Static|BindingFlags.NonPublic);
                var error=Assert.Throws<TargetInvocationException>(()=>cleanup.Invoke(null,new object[]{native.Root}));
                Assert.That(error.InnerException,Is.TypeOf<InvalidOperationException>());native.AssertJunction(link);native.AssertSentinel();
            }
            finally { native.Dispose(); }
            Assert.That(Directory.Exists(native.Base),Is.False,"native owner removes known links before deleting its owned base");
        }

        [TestCase("root")]
        [TestCase("root-b")]
        [TestCase("generation")]
        [TestCase("action")]
        [TestCase("cell")]
        public void AC_M5D7QC2_004_006_AfterFinalProbePairMetadataMismatchCannotStageReceipt(string mutation)
        {
            using(var fixture=Fixture.Create("final-pair-"+mutation))
            using(var foreign=Fixture.Create("final-pair-foreign-"+mutation))
            {
                var history=fixture.Adapter.CurrentReceipt;var disk=fixture.PrepareDisk();var control=new AfterFinalPairMismatch(fixture.Adapter,fixture.Router,foreign.Actions,mutation);
                var result=ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root,disk.PreparedProof,fixture.Adapter,fixture.Router,control);
                AssertTerminalFailure(fixture,result,history,true);
                Assert.That(control.Cell,Is.Not.Null);Assert.Throws<InvalidOperationException>(()=>{var ignored=control.Cell.Root;});
                Assert.That(foreign.Actions.UI.Get().enabled,Is.True);
            }
        }

        private static bool DeletedBefore(ProfileResetMemoryCutoverCheckpointV1 point)
        {
            return point == ProfileResetMemoryCutoverCheckpointV1.AfterDelete || point == ProfileResetMemoryCutoverCheckpointV1.BeforeFinalProbe || point == ProfileResetMemoryCutoverCheckpointV1.AfterFinalProbe || point == ProfileResetMemoryCutoverCheckpointV1.BeforeReceiptStage;
        }

        private static void AssertTerminalFailure(Fixture fixture, ProfileResetMemoryResultV1 result, DesktopProfileLaunchReceiptV1? history, bool barrierDeleted, bool historyReadable=true, bool barrierTempPresent=false)
        {
            if(result!=null) { Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.ReloadRequired)); Assert.That(result.Receipt, Is.Null); }
            Assert.That(fixture.Adapter.IsResetCutoverTerminal, Is.True);
            if(historyReadable)Assert.That(fixture.Adapter.CurrentReceipt, Is.EqualTo(history));else Assert.Throws<InvalidOperationException>(()=>{var ignored=fixture.Adapter.CurrentReceipt;});
            Assert.Throws<InvalidOperationException>(() => { var ignored = fixture.Adapter.CurrentResetSession; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = fixture.Router.CurrentUiFrame; });
            ProfileLaunchNotificationV1 notification; Assert.That(fixture.Adapter.TryTakeNotification(out notification), Is.False);
            Assert.That(File.Exists(Path.Combine(fixture.Root, ProfileResetBarrierV1.BarrierName)), Is.EqualTo(!barrierDeleted));
            Assert.That(File.Exists(Path.Combine(fixture.Root, ProfileResetBarrierV1.BarrierTempName)), Is.EqualTo(barrierTempPresent));
        }

        private sealed class ThrowAt : IProfileResetMemoryCutoverControlV1
        {
            private readonly ProfileResetMemoryCutoverCheckpointV1 _point; internal ThrowAt(ProfileResetMemoryCutoverCheckpointV1 point) { _point=point; }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point) { if (point==_point) throw new InvalidOperationException("Injected cutover checkpoint."); }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point) { }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exact) { exact(); }
        }
        private sealed class OperationFault : IProfileResetMemoryCutoverControlV1
        {
            private readonly ResetCutoverOperationV1 _operation; private readonly bool _after, _forceCleanup; internal int WrapperCalls, ExactCalls;
            internal OperationFault(ResetCutoverOperationV1 operation, bool after, bool forceCleanup) { _operation=operation; _after=after; _forceCleanup=forceCleanup; }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point) { }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point) { if (_forceCleanup && point==ProfileResetMemoryCutoverCheckpointV1.AfterTerminalGate) throw new InvalidOperationException("Force real candidate cleanup."); }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exact)
            {
                if(operation!=_operation) { exact(); return; }
                WrapperCalls++; if(!_after) throw new InvalidOperationException("Before exact owned operation."); ExactCalls++; exact(); throw new InvalidOperationException("After exact owned operation.");
            }
        }
        private sealed class AuthorityThrow : IProfileResetMemoryCutoverControlV1
        {
            private readonly ProfileResetMemoryAuthorityCheckpointV1 _point; internal AuthorityThrow(ProfileResetMemoryAuthorityCheckpointV1 point) { _point=point; }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point) { }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point) { if(point==_point) throw new InvalidOperationException("Injected Profile authority checkpoint."); }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exact) { exact(); }
        }
        private sealed class StagedDisposeDiagnosticFault : IProfileResetMemoryCutoverControlV1
        {
            private readonly DesktopProfileLaunchAdapterV1 _owner; private readonly bool _setTrue; private bool _fired;
            internal GameInputActions Candidate; internal int DisposeNewCalls, ExactDisposeCalls;
            internal StagedDisposeDiagnosticFault(DesktopProfileLaunchAdapterV1 owner, bool setTrue) { _owner=owner; _setTrue=setTrue; }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point) { }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point)
            {
                if(_fired || point!=ProfileResetMemoryCutoverCheckpointV1.AfterCandidateCreate) return;
                _fired=true; Candidate=(GameInputActions)typeof(DesktopProfileLaunchAdapterV1).GetField("_untransferredCandidate",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(_owner);
                if(_setTrue) typeof(DesktopProfileLaunchAdapterV1).GetField("_untransferredCandidateDisposeAttempted",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(_owner,true);
                throw new InvalidOperationException("Force real staged candidate cleanup.");
            }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exact)
            {
                if(operation==ResetCutoverOperationV1.DisposeNew) { DisposeNewCalls++; ExactDisposeCalls++; }
                exact();
            }
        }
        private sealed class LockMarkerAtDelete : IProfileResetMemoryCutoverControlV1, IDisposable
        {
            private readonly string _marker; private FileStream _handle; internal bool Opened;
            internal LockMarkerAtDelete(string root) { _marker=Path.Combine(root,ProfileResetBarrierV1.BarrierName); }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point) { }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point)
            {
                if(point!=ProfileResetMemoryAuthorityCheckpointV1.BeforeBarrierDelete) return;
                _handle=new FileStream(_marker,FileMode.Open,FileAccess.Read,FileShare.Read); Opened=true;
            }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exact) { exact(); }
            public void Dispose() { if(_handle!=null) _handle.Dispose(); }
        }
        private sealed class PostDeleteActionCorruption : IProfileResetMemoryCutoverControlV1
        {
            private readonly InputRouter _router; private readonly GameInputActions _foreign; private bool _done;
            internal PostDeleteActionCorruption(InputRouter router, GameInputActions foreign) { _router=router; _foreign=foreign; }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point) { }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point)
            {
                if(_done || point!=ProfileResetMemoryAuthorityCheckpointV1.AfterBarrierDelete) return;
                _done=true; typeof(InputRouter).GetField("_actions",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(_router,_foreign);
            }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exact) { exact(); }
        }
        private sealed class PostDeleteDiskMutation : IProfileResetMemoryCutoverControlV1
        {
            private readonly string _root,_id,_mutation; private bool _done;
            internal PostDeleteDiskMutation(string root,string id,string mutation){_root=root;_id=id;_mutation=mutation;}
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point) { }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point)
            {
                if(_done||point!=ProfileResetMemoryAuthorityCheckpointV1.AfterBarrierDelete)return; _done=true;
                if(_mutation=="primary")File.WriteAllBytes(Path.Combine(_root,"profile.json"),new byte[]{1});
                else if(_mutation=="previous")File.WriteAllBytes(Path.Combine(_root,"profile.prev.json"),new byte[]{1});
                else if(_mutation=="temp")File.WriteAllBytes(Path.Combine(_root,"profile.tmp.json"),new byte[]{1});
                else if(_mutation=="barrier")File.WriteAllBytes(Path.Combine(_root,ProfileResetBarrierV1.BarrierName),new byte[]{1});
                else if(_mutation=="barrier-temp")File.WriteAllBytes(Path.Combine(_root,ProfileResetBarrierV1.BarrierTempName),new byte[]{1});
                else { var archive=Path.Combine(_root,"recovery","new-game",_id,"primary.bin"); if(_mutation=="archive-changed")File.WriteAllBytes(archive,new byte[]{1});else File.Delete(archive); }
            }
            public void InvokeOperation(ResetCutoverOperationV1 operation,Action exact){exact();}
        }
        private sealed class BeforeDeleteMarkerMutation : IProfileResetMemoryCutoverControlV1
        {
            private readonly string _marker,_kind;private bool _done;internal BeforeDeleteMarkerMutation(string marker,string kind){_marker=marker;_kind=kind;}
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point){}public void InvokeOperation(ResetCutoverOperationV1 operation,Action exact){exact();}
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point){if(_done||point!=ProfileResetMemoryCutoverCheckpointV1.BeforeDelete)return;_done=true;if(_kind=="changed")File.WriteAllBytes(_marker,new byte[]{0x41});else if(_kind=="missing")File.Delete(_marker);else{File.Delete(_marker);Directory.CreateDirectory(_marker);}}
        }
        private sealed class BeforeDeleteJunction : IProfileResetMemoryCutoverControlV1
        {
            private readonly string _marker;private readonly NativeJunctionRoot _native;private bool _done;internal BeforeDeleteJunction(string marker,NativeJunctionRoot native){_marker=marker;_native=native;}
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point){}public void InvokeOperation(ResetCutoverOperationV1 operation,Action exact){exact();}
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point){if(_done||point!=ProfileResetMemoryCutoverCheckpointV1.BeforeDelete)return;_done=true;File.Delete(_marker);_native.CreateJunction(_marker,_native.Target);}
        }
        private sealed class AfterFinalPairMismatch : IProfileResetMemoryCutoverControlV1
        {
            private readonly DesktopProfileLaunchAdapterV1 _adapter;private readonly InputRouter _router;private readonly GameInputActions _foreign;private readonly string _kind;private bool _done;internal CurrentSessionProfileV1 Cell;
            internal AfterFinalPairMismatch(DesktopProfileLaunchAdapterV1 adapter,InputRouter router,GameInputActions foreign,string kind){_adapter=adapter;_router=router;_foreign=foreign;_kind=kind;}
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point){}
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point)
            {
                if(_done||point!=ProfileResetMemoryAuthorityCheckpointV1.AfterFinalDiskProbe)return;_done=true;Cell=_adapter.CurrentResetSession;
                if(_kind=="root")typeof(DesktopProfileLaunchAdapterV1).GetField("_launchRootWitnessA",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(_adapter,"C:\\foreign-root");
                else if(_kind=="root-b")typeof(DesktopProfileLaunchAdapterV1).GetField("_launchRootWitnessB",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(_adapter,"C:\\foreign-root");
                else if(_kind=="generation")typeof(InputRouter).GetField("_resetCutoverGeneration",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(_router,0L);
                else if(_kind=="action")typeof(InputRouter).GetField("_actions",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(_router,_foreign);
                else typeof(CurrentSessionProfileV1).GetField("_generation",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(Cell,0L);
            }
            public void InvokeOperation(ResetCutoverOperationV1 operation,Action exact){exact();}
        }
        // Test-only NTFS junction producer.  It is confined to one owned temp
        // base and never implements or calls an alternate profile algorithm.
        private sealed class NativeJunctionRoot : IDisposable
        {
            private const uint Write=0x40000000,Open=3,Backup=0x02000000,OpenReparse=0x00200000,Set=0x000900A4,Get=0x000900A8,Tag=0xA0000003;
            private readonly string _base; private readonly byte[] _sentinel={0x43,0x32,0x4a,0x58}; internal readonly string Root,Target;internal string Base{get{return _base;}}
            internal NativeJunctionRoot(){_base=Path.Combine(Path.GetTempPath(),"AcadeGameMaker-C2-matrix-junction-"+Guid.NewGuid().ToString("N"));Root=Path.Combine(_base,"root");Target=Path.Combine(_base,"target");Directory.CreateDirectory(Root);Directory.CreateDirectory(Target);File.WriteAllBytes(Path.Combine(Target,"sentinel.bin"),_sentinel);Verify(_base);}
            internal void CreateJunction(string link,string target)
            {
                Verify(link);Verify(target);if(Directory.Exists(link)||File.Exists(link))throw new InvalidOperationException("Junction link exists.");Directory.CreateDirectory(link);
                var sub=System.Text.Encoding.Unicode.GetBytes("\\??\\"+Path.GetFullPath(target).TrimEnd(Path.DirectorySeparatorChar)+"\0");var print=System.Text.Encoding.Unicode.GetBytes(Path.GetFullPath(target).TrimEnd(Path.DirectorySeparatorChar)+"\0");var data=new byte[16+sub.Length+print.Length];U32(data,0,Tag);U16(data,4,(ushort)(data.Length-8));U16(data,10,(ushort)(sub.Length-2));U16(data,12,(ushort)sub.Length);U16(data,14,(ushort)(print.Length-2));Buffer.BlockCopy(sub,0,data,16,sub.Length);Buffer.BlockCopy(print,0,data,16+sub.Length,print.Length);
                var h=CreateFile(link,Write,0,IntPtr.Zero,Open,Backup|OpenReparse,IntPtr.Zero);if(h==new IntPtr(-1))throw new IOException("Junction CreateFile failed: "+Marshal.GetLastWin32Error());try{uint n;if(!DeviceIoControl(h,Set,data,(uint)data.Length,null,0,out n,IntPtr.Zero))throw new IOException("Junction set failed: "+Marshal.GetLastWin32Error());}finally{CloseHandle(h);}AssertJunction(link);
            }
            internal void AssertJunction(string link){Verify(link);var h=CreateFile(link,0,0,IntPtr.Zero,Open,Backup|OpenReparse,IntPtr.Zero);if(h==new IntPtr(-1))throw new IOException("Junction verify open failed.");try{var d=new byte[16384];uint n;if(!DeviceIoControl(h,Get,null,0,d,(uint)d.Length,out n,IntPtr.Zero)||n<8||R32(d,0)!=Tag)throw new IOException("Junction tag invalid.");}finally{CloseHandle(h);}}
            internal void AssertSentinel(){Verify(Target);var actual=File.ReadAllBytes(Path.Combine(Target,"sentinel.bin"));if(actual.Length!=_sentinel.Length)throw new InvalidOperationException("Target sentinel changed.");for(var i=0;i<actual.Length;i++)if(actual[i]!=_sentinel[i])throw new InvalidOperationException("Target sentinel changed.");if(File.Exists(Path.Combine(Target,"profile.operation.lock"))||File.Exists(Path.Combine(Target,ProfileResetBarrierV1.BarrierName)))throw new InvalidOperationException("Junction target was mutated.");}
            public void Dispose(){RemoveLinks(_base);Verify(_base);if(Directory.Exists(_base))Directory.Delete(_base,true);}
            private void RemoveLinks(string path){Verify(path);foreach(var child in Directory.GetDirectories(path)){if((File.GetAttributes(child)&FileAttributes.ReparsePoint)!=0){AssertJunction(child);if(!RemoveDirectory(child))throw new IOException("Junction removal failed.");}else RemoveLinks(child);}}
            private void Verify(string path){var b=Path.GetFullPath(_base).TrimEnd(Path.DirectorySeparatorChar);var full=Path.GetFullPath(path);var temp=Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar);if(!b.StartsWith(temp+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)||(!string.Equals(full.TrimEnd(Path.DirectorySeparatorChar),b,StringComparison.OrdinalIgnoreCase)&&!full.StartsWith(b+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)))throw new InvalidOperationException("Junction path escapes owned temp base.");if(!Directory.Exists(b)||(File.GetAttributes(b)&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Junction base is unsafe.");for(var current=new DirectoryInfo(b);current!=null;current=current.Parent){var a=File.GetAttributes(current.FullName);if((a&FileAttributes.Directory)==0||(a&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Junction ancestor is unsafe.");if(string.Equals(current.FullName.TrimEnd(Path.DirectorySeparatorChar),temp,StringComparison.OrdinalIgnoreCase))return;}throw new InvalidOperationException("Junction base escaped OS temp.");}
            private static void U16(byte[]b,int i,ushort v){b[i]=(byte)v;b[i+1]=(byte)(v>>8);}private static void U32(byte[]b,int i,uint v){b[i]=(byte)v;b[i+1]=(byte)(v>>8);b[i+2]=(byte)(v>>16);b[i+3]=(byte)(v>>24);}private static uint R32(byte[]b,int i){return(uint)(b[i]|(b[i+1]<<8)|(b[i+2]<<16)|(b[i+3]<<24));}
            [DllImport("kernel32.dll",SetLastError=true,CharSet=CharSet.Unicode)]private static extern IntPtr CreateFile(string n,uint a,uint s,IntPtr x,uint c,uint f,IntPtr t);[DllImport("kernel32.dll",SetLastError=true)]private static extern bool DeviceIoControl(IntPtr h,uint c,byte[]i,uint il,byte[]o,uint ol,out uint r,IntPtr x);[DllImport("kernel32.dll",SetLastError=true)]private static extern bool CloseHandle(IntPtr h);[DllImport("kernel32.dll",SetLastError=true,CharSet=CharSet.Unicode)]private static extern bool RemoveDirectory(string p);
        }

        private sealed class Fixture : IDisposable
        {
            internal readonly string Root; internal readonly GameObject Parent; internal readonly InputRouter Router; internal readonly DesktopProfileLaunchAdapterV1 Adapter;
            private Fixture(string root, GameObject parent, InputRouter router, DesktopProfileLaunchAdapterV1 adapter) { Root=root; Parent=parent; Router=router; Adapter=adapter; }
            internal GameInputActions Actions { get { return (GameInputActions)typeof(InputRouter).GetField("_actions",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(Router); } }
            internal static Fixture Create(string suffix)
            {
                return CreateAt(Path.Combine(Path.GetTempPath(),"AcadeGameMaker-C2-matrix-"+suffix+"-"+Guid.NewGuid().ToString("N")),true);
            }
            internal static Fixture CreateAt(string root,bool cleanupRootOnFailure=false)
            {
                var parent=new GameObject("c2-matrix"); parent.SetActive(false);
                try { var ro=new GameObject("router"); ro.transform.SetParent(parent.transform); var router=ro.AddComponent<InputRouter>(); router.ConfigureHubUiOnlyForAuthoring();
                    var ao=new GameObject("adapter"); ao.transform.SetParent(parent.transform); var adapter=ao.AddComponent<DesktopProfileLaunchAdapterV1>(); adapter.ConfigureForTests(router,new EnvironmentPort(root),new PreparationPort());
                    parent.SetActive(true); typeof(DesktopProfileLaunchAdapterV1).GetMethod("Start",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(adapter,null); Assert.That(adapter.CurrentReceipt.HasValue,Is.True);
                    return new Fixture(root,parent,router,adapter); }
                catch { UnityEngine.Object.DestroyImmediate(parent); if(cleanupRootOnFailure)CleanupOwnedRoot(root); throw; }
            }
            internal ProfileResetDiskResultV1 PrepareDisk() { var disk=ProfileResetDiskTransactionV1.Begin(Root,ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(Root)); Assert.That(disk.Outcome,Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared)); return disk; }
            internal void DestroyRuntimeOnly(){UnityEngine.Object.DestroyImmediate(Parent);}
            public void Dispose() { DestroyRuntimeOnly();CleanupOwnedRoot(Root); }
            private static void CleanupOwnedRoot(string root){var full=Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);var temp=Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);if(!full.StartsWith(temp+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Cleanup escaped OS temp.");if(!Directory.Exists(full))return;VerifyOwnedTree(full,temp);Directory.Delete(full,true);}
            private static void VerifyOwnedTree(string directory,string temp){var root=Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);if((File.GetAttributes(root)&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Cleanup root is reparse.");for(var ancestor=new DirectoryInfo(root);ancestor!=null;ancestor=ancestor.Parent){var attributes=File.GetAttributes(ancestor.FullName);if((attributes&FileAttributes.Directory)==0||(attributes&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Cleanup ancestor is unsafe.");if(string.Equals(ancestor.FullName.TrimEnd(Path.DirectorySeparatorChar),temp,StringComparison.OrdinalIgnoreCase))break;}foreach(var entry in Directory.GetFileSystemEntries(root)){var full=Path.GetFullPath(entry);if(!full.StartsWith(root+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)||!full.StartsWith(temp+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Cleanup child escaped owned root.");var attributes=File.GetAttributes(full);if((attributes&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Cleanup refuses reparse descendant.");if((attributes&FileAttributes.Directory)!=0)VerifyOwnedTreeChild(full,root,temp);}}
            private static void VerifyOwnedTreeChild(string directory,string root,string temp){foreach(var entry in Directory.GetFileSystemEntries(directory)){var full=Path.GetFullPath(entry);if(!full.StartsWith(root+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)||!full.StartsWith(temp+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Cleanup child escaped owned root.");var attributes=File.GetAttributes(full);if((attributes&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Cleanup refuses reparse descendant.");if((attributes&FileAttributes.Directory)!=0)VerifyOwnedTreeChild(full,root,temp);}}
        }
        private sealed class EnvironmentPort : IDesktopProfileLaunchEnvironmentV1 { private readonly string _root; internal EnvironmentPort(string root){_root=root;} public string GetPersistentDataPath(){return _root;} public DateTimeOffset GetUtcNow(){return new DateTimeOffset(2026,9,28,0,0,0,TimeSpan.Zero);} }
        private sealed class PreparationPort : IDesktopProfileLaunchPreparationV1 { public PreparedProfileLaunchV1 Prepare(string root,DateTimeOffset utc){return ProfileLaunchPreparationCoordinatorV1.Prepare(root,utc);} }
    }
}
