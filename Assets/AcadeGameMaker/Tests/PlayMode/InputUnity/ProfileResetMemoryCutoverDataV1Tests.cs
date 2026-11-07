using System;
using System.Reflection;
using System.IO;
using NUnit.Framework;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Input;
using AcadeGameMaker.Profile;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-M5D7QC2-001..006; AC-M5D7QC2-001/002/003/004/006/009. This focused
    // fixture uses the real Hub/C1/C2 paths for bounded coordinator faults.
    internal sealed class ProfileResetMemoryCutoverDataV1Tests
    {
        [Test]
        public void AC_M5D7QC2_004_CapabilityAndReceiptCannotBeConstructedByCallers()
        {
            Assert.That(typeof(ResetCutoverCapabilityV1).GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length, Is.EqualTo(0));
            Assert.That(typeof(ProfileResetMemoryReceiptV1).GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length, Is.EqualTo(0));
            Assert.That(typeof(ProfileResetMemoryReceiptV1).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { typeof(string), typeof(string), typeof(string), typeof(long), typeof(long), typeof(string), typeof(long) }, null), Is.Null);
        }

        [Test]
        public void AC_M5D7QC2_003_CheckpointSurfaceIsClosedAndNoOp()
        {
            Assert.That(Enum.GetValues(typeof(ProfileResetMemoryCutoverCheckpointV1)).Length, Is.EqualTo(19));
            var control = ProfileResetMemoryCutoverNoOpControlV1.Instance;
            foreach (ProfileResetMemoryCutoverCheckpointV1 checkpoint in Enum.GetValues(typeof(ProfileResetMemoryCutoverCheckpointV1)))
                Assert.That(() => control.Checkpoint(checkpoint), Throws.Nothing, checkpoint.ToString());
            Assert.That(Enum.IsDefined(typeof(ProfileResetMemoryCutoverCheckpointV1), "AfterPublication"), Is.False);
        }

        [Test]
        public void AC_M5D7QC2_009_ResultRejectsClosedEnumAndSuccessWithoutFinalEvidence()
        {
            Assert.That(() => new ProfileResetMemoryResultV1(default(ProfileResetMemoryOutcomeV1)), Throws.ArgumentException);
            Assert.That(() => new ProfileResetMemoryResultV1(ProfileResetMemoryOutcomeV1.Completed), Throws.ArgumentException);
            var result = new ProfileResetMemoryResultV1(ProfileResetMemoryOutcomeV1.ReloadRequired);
            typeof(ProfileResetMemoryResultV1).GetField("_outcome", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(result, 99);
            Assert.That(() => { var ignored = result.Outcome; }, Throws.ArgumentException);
            Assert.That(() => { var ignored = result.Receipt; }, Throws.ArgumentException);
        }

        [Test]
        public void AC_M5D7QC2_001_ActualHeldRootLeaseReturnsBusyWithoutMemoryOrProofMutation()
        {
            using (var fixture = new LiveFixture())
            {
                var disk = fixture.PrepareDisk(); var before = fixture.Actions;
                using (ProfileRootOperationLockV1.Acquire(fixture.Root))
                {
                    var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router);
                    Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.Busy));
                }
                Assert.That(fixture.Actions, Is.SameAs(before));
                Assert.That(disk.PreparedProof.TryConsume(), Is.True, "Busy must not consume the C1 proof.");
            }
        }

        [Test]
        public void AC_M5D7QC2_003_InjectedBusyAfterLeaseIsReloadNotBusy()
        {
            using (var fixture = new LiveFixture())
            {
                var disk = fixture.PrepareDisk(); var control = new BusyAt(ProfileResetMemoryCutoverCheckpointV1.BeforeStaging);
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, control);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.ReloadRequired));
                Assert.That(fixture.ResetTerminal, Is.True);
            }
        }

        [Test]
        public void AC_M5D7QC2_004_CellGetterRejectsRetainedCellAfterAfterCellFault()
        {
            using (var fixture = new LiveFixture())
            {
                var disk = fixture.PrepareDisk(); var control = new FaultAt(fixture.Adapter, ProfileResetMemoryCutoverCheckpointV1.AfterCell); var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, control);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.ReloadRequired));
                Assert.That(control.Retained, Is.Not.Null);
                var retained = control.Retained;
                Assert.That(() => { var ignored = retained.Root; }, Throws.InvalidOperationException);
                Assert.That(() => { var ignored = retained.Document; }, Throws.InvalidOperationException);
                Assert.That(() => { var ignored = retained.Settings; }, Throws.InvalidOperationException);
                Assert.That(() => { var ignored = retained.Input; }, Throws.InvalidOperationException);
                Assert.That(() => { var ignored = retained.Tutorial; }, Throws.InvalidOperationException);
                Assert.That(() => { var ignored = retained.Progression; }, Throws.InvalidOperationException);
                Assert.That(() => { var ignored = retained.Generation; }, Throws.InvalidOperationException);
                Assert.That(() => retained.CopyCanonicalDocument(), Throws.InvalidOperationException);
                Assert.That(() => retained.IsFor(fixture.Actions, 1, fixture.Root), Throws.InvalidOperationException);
            }
        }

        [TestCase("_root")]
        [TestCase("_canonical")]
        [TestCase("_generation")]
        [TestCase("_actions")]
        [TestCase("_owner")]
        [TestCase("_document")]
        [TestCase("_settings")]
        [TestCase("_input")]
        [TestCase("_tutorial")]
        [TestCase("_progression")]
        public void AC_M5D7QC2_004_CellGetterRejectsEachReflectedPairFact(string fieldName)
        {
            using (var fixture = new LiveFixture())
            {
                var disk = fixture.PrepareDisk(); var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.Completed));
                var copy = result.Receipt.CopyTargetBytes(); copy[0] ^= 1; Assert.That(result.Receipt.CopyTargetBytes()[0], Is.Not.EqualTo(copy[0]), "receipt returns a defensive target copy after lease release");
                var cell = fixture.Adapter.CurrentResetSession; var field = typeof(CurrentSessionProfileV1).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic); GameInputActions foreign = null;
                if (fieldName == "_root") field.SetValue(cell, fixture.Root + "-foreign");
                else if (fieldName == "_canonical") { var bytes = (byte[])field.GetValue(cell); bytes[0] ^= 1; }
                else if (fieldName == "_generation") field.SetValue(cell, 0L);
                else if (fieldName == "_actions") { foreign = new GameInputActions(); field.SetValue(cell, foreign); }
                else if (fieldName == "_owner") field.SetValue(cell, null);
                else if (fieldName == "_document") field.SetValue(cell, default(ProfileCanonicalDocumentV1));
                else if (fieldName == "_settings") field.SetValue(cell, default(ProfileSettingsSnapshot));
                else if (fieldName == "_input") field.SetValue(cell, default(ProfileInputSnapshot));
                else if (fieldName == "_tutorial") field.SetValue(cell, default(ProfileTutorialSnapshot));
                else if (fieldName == "_progression") field.SetValue(cell, default(ProfileProgressionSnapshot));
                try { Assert.That(() => { var ignored = cell.Root; }, Throws.InvalidOperationException); }
                finally { if (foreign != null) foreign.Dispose(); }
            }
        }

        [TestCase("_launchRootWitnessA")]
        [TestCase("_launchRootWitnessB")]
        public void AC_M5D7QC2_003_004_006_008_CompletedRootWitnessDriftInvalidatesLiveCellWithoutRewritingHistory(string witnessField)
        {
            using (var fixture = new LiveFixture())
            {
                var history = fixture.Adapter.CurrentReceipt; Assert.That(history.HasValue, Is.True);
                var disk = fixture.PrepareDisk(); var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router); Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.Completed));var retainedCell=fixture.Adapter.CurrentResetSession;
                typeof(DesktopProfileLaunchAdapterV1).GetField(witnessField, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(fixture.Adapter, fixture.Root + "-foreign");
                Assert.That(() => { var ignored = fixture.Adapter.CurrentResetSession; }, Throws.InvalidOperationException);
                Assert.That(() => { var ignored = retainedCell.Root; }, Throws.InvalidOperationException);Assert.That(() => { var ignored=retainedCell.Document;},Throws.InvalidOperationException);Assert.That(() => retainedCell.CopyCanonicalDocument(),Throws.InvalidOperationException);Assert.That(() => { var ignored=retainedCell.Generation;},Throws.InvalidOperationException);
                Assert.That(fixture.Adapter.CurrentReceipt, Is.EqualTo(history));
                var retainedHistory = (DesktopProfileLaunchReceiptV1)typeof(DesktopProfileLaunchAdapterV1).GetField("_currentReceipt", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fixture.Adapter);var retainedLaunchWitness=(DesktopProfileLaunchReceiptV1)typeof(DesktopProfileLaunchAdapterV1).GetField("_launchReceiptWitness",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(fixture.Adapter);Assert.That(retainedHistory, Is.EqualTo(history.Value));Assert.That(retainedLaunchWitness,Is.EqualTo(history.Value));
                Assert.That(() => result.Receipt.Validate(), Throws.Nothing, "detached final evidence remains forensic evidence");fixture.Router.ValidateAllMapsDisabled(fixture.Actions);ProfileLaunchNotificationV1 notification;Assert.That(fixture.Adapter.TryTakeNotification(out notification), Is.False);
            }
        }

        [Test]
        public void AC_M5D7QC2_004_PreTransferCapabilityCannotMintReceiptWithRealFinalDiskProof()
        {
            using (var fixture = new LiveFixture())
            {
                var disk = fixture.PrepareDisk(); var document = ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument;
                using (var lease = ProfileRootOperationLockV1.Acquire(fixture.Root))
                {
                    var authority = ProfileResetDiskTransactionV1.ReauthenticatePreparedUnderHeldRoot(lease, fixture.Root, disk.PreparedProof);
                    var staged = new GameInputActions();
                    try
                    {
                        var binding = ProfileBindingOverrideApplyAdapterV1.Apply(document.Snapshot.Input, staged); fixture.Router.ValidateAllMapsDisabled(staged);
                        fixture.Adapter.RegisterStagedCandidate(staged, ProfileResetMemoryCutoverNoOpControlV1.Instance);
                        var cap = fixture.Adapter.EnterResetCutoverTerminal(fixture.Root, authority, ProfileResetMemoryCutoverNoOpControlV1.Instance);
                        ProfileResetDiskTransactionV1.RemoveAuthenticatedBarrierUnderHeldRoot(lease, authority);
                        var finalProof = ProfileResetDiskTransactionV1.CaptureFinalDiskProofUnderHeldRoot(lease, authority);
                        var callerCell = new CurrentSessionProfileV1(fixture.Adapter, fixture.Root, document, binding, staged, cap.Generation);
                        Assert.That(() => ProfileResetMemoryReceiptV1.CreateFinal(finalProof, callerCell, binding, cap), Throws.InvalidOperationException);
                    }
                    finally { fixture.Adapter.LatchResetCutoverFailure(); }
                }
            }
        }

        [Test]
        public void AC_M5D7QC2_006_CompletedReceiptRejectsEverySingleFieldForgeryAndRetainsDefensiveEvidence()
        {
            using (var fixture = new LiveFixture())
            {
                var disk = fixture.PrepareDisk(); var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router); Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.Completed));
                var receipt = result.Receipt;
                foreach (var field in typeof(ProfileResetMemoryReceiptV1).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    var original = field.GetValue(receipt); field.SetValue(receipt, ForgedValue(field, original));
                    try { AssertReceiptGettersReject(receipt, field.Name); }
                    finally { field.SetValue(receipt, original); }
                    Assert.That(() => receipt.Validate(), Throws.Nothing, field.Name + " restore");
                }
            }
        }

        [Test]
        public void AC_M5D7QC2_003_ActualUnsafeLockPathIsManualAndTerminal()
        {
            using (var fixture = new LiveFixture())
            {
                var disk = fixture.PrepareDisk(); var lockPath = Path.Combine(fixture.Root, "profile.operation.lock");
                Assert.That(File.Exists(lockPath), Is.True); File.Delete(lockPath); Directory.CreateDirectory(lockPath);
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.ManualRepairRequired));
                Assert.That(fixture.ResetTerminal, Is.True);
            }
        }

        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void AC_M5D7QC2_002_StagedDisposeFaultIsReloadAndInvokedOnce(bool afterRealDispose, bool ioFailure)
        {
            using (var fixture = new LiveFixture())
            {
                var disk = fixture.PrepareDisk(); var control = new DisposeFault(afterRealDispose, ioFailure);
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, control);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.ReloadRequired));
                Assert.That(control.DisposeNewCalls, Is.EqualTo(1));
                Assert.That((bool)typeof(DesktopProfileLaunchAdapterV1).GetField("_untransferredCandidateDisposeAttempted", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fixture.Adapter), Is.True);
                Assert.That(fixture.ResetTerminal, Is.True);
            }
        }

        private sealed class BusyAt : IProfileResetMemoryCutoverControlV1
        {
            private readonly ProfileResetMemoryCutoverCheckpointV1 _target;
            internal BusyAt(ProfileResetMemoryCutoverCheckpointV1 target) { _target = target; }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 checkpoint) { if (checkpoint == _target) throw new ProfileRootOperationBlockedExceptionV1(ProfileRootOperationOutcomeV1.Busy, "Injected after lease."); }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exactOwnedOperation) { exactOwnedOperation(); }
        }
        private static object ForgedValue(FieldInfo field, object original)
        {
            if (field.FieldType == typeof(string)) return ((string)original) + "x";
            if (field.FieldType == typeof(byte[])) { var copy = (byte[])((byte[])original).Clone(); copy[0] ^= 1; return copy; }
            if (field.FieldType == typeof(long)) return ((long)original) + 1;
            if (field.FieldType == typeof(bool)) return !((bool)original);
            if (field.FieldType.IsEnum) return Enum.ToObject(field.FieldType, 99);
            // The detached final-proof reference is deliberately nullable only
            // for hostile reflection; receipt validation must reject it.
            if (!field.FieldType.IsValueType) return null;
            return Activator.CreateInstance(field.FieldType);
        }
        private static void AssertReceiptGettersReject(ProfileResetMemoryReceiptV1 receipt, string fieldName)
        {
            Assert.That(() => { var ignored = receipt.Generation; }, Throws.InvalidOperationException, fieldName + ": generation");
            Assert.That(() => { var ignored = receipt.Root; }, Throws.InvalidOperationException, fieldName + ": root");
            Assert.That(() => { var ignored = receipt.TransactionId; }, Throws.InvalidOperationException, fieldName + ": transaction");
            Assert.That(() => { var ignored = receipt.TargetSha256; }, Throws.InvalidOperationException, fieldName + ": hash");
            Assert.That(() => { var ignored = receipt.TargetByteLength; }, Throws.InvalidOperationException, fieldName + ": length");
            Assert.That(() => { var ignored = receipt.TargetRevision; }, Throws.InvalidOperationException, fieldName + ": revision");
            Assert.That(() => { var ignored = receipt.ArchiveProofDigest; }, Throws.InvalidOperationException, fieldName + ": archive");
            Assert.That(() => receipt.CopyTargetBytes(), Throws.InvalidOperationException, fieldName + ": target bytes");
            Assert.That(() => { var ignored = receipt.UiOnlyBlocked; }, Throws.InvalidOperationException, fieldName + ": disposition");
        }
        private sealed class FaultAt : IProfileResetMemoryCutoverControlV1
        {
            private readonly DesktopProfileLaunchAdapterV1 _owner; private readonly ProfileResetMemoryCutoverCheckpointV1 _target;
            internal CurrentSessionProfileV1 Retained { get; private set; }
            internal FaultAt(DesktopProfileLaunchAdapterV1 owner, ProfileResetMemoryCutoverCheckpointV1 target) { _owner = owner; _target = target; }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 checkpoint) { if (checkpoint == _target) { Retained = _owner.CurrentResetSession; throw new InvalidOperationException("Injected post-cell fault."); } }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exactOwnedOperation) { exactOwnedOperation(); }
        }
        private sealed class DisposeFault : IProfileResetMemoryCutoverControlV1
        {
            private readonly bool _after, _io; internal int DisposeNewCalls;
            internal DisposeFault(bool after, bool io) { _after = after; _io = io; }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 checkpoint) { if (checkpoint == ProfileResetMemoryCutoverCheckpointV1.AfterCandidateCreate) throw new InvalidOperationException("Force staged cleanup."); }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exactOwnedOperation)
            {
                if (operation != ResetCutoverOperationV1.DisposeNew) { exactOwnedOperation(); return; }
                DisposeNewCalls++; if (!_after) Throw(); exactOwnedOperation(); Throw();
            }
            private void Throw() { if (_io) throw new IOException("Injected staged dispose IO failure."); throw new InvalidOperationException("Injected staged dispose failure."); }
        }
        private sealed class LiveFixture : IDisposable
        {
            private readonly GameObject _parent; internal readonly string Root; internal readonly InputRouter Router; internal readonly DesktopProfileLaunchAdapterV1 Adapter;
            internal LiveFixture()
            {
                Root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C2-data-" + Guid.NewGuid().ToString("N")); _parent = new GameObject("c2-data-parent"); _parent.SetActive(false);
                var routerObject = new GameObject("router"); routerObject.transform.SetParent(_parent.transform); Router = routerObject.AddComponent<InputRouter>(); Router.ConfigureHubUiOnlyForAuthoring();
                var adapterObject = new GameObject("adapter"); adapterObject.transform.SetParent(_parent.transform); Adapter = adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>(); Adapter.ConfigureForTests(Router, new EnvironmentPort(Root), new PreparationPort()); _parent.SetActive(true);
                typeof(DesktopProfileLaunchAdapterV1).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(Adapter, null); Assert.That(Adapter.CurrentReceipt.HasValue, Is.True);
            }
            internal GameInputActions Actions { get { return (GameInputActions)typeof(InputRouter).GetField("_actions", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Router); } }
            internal bool ResetTerminal { get { return (bool)typeof(InputRouter).GetField("_resetCutoverTerminal", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Router); } }
            internal ProfileResetDiskResultV1 PrepareDisk() { var result=ProfileResetDiskTransactionV1.Begin(Root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(Root)); Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared)); return result; }
            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(_parent);
                var temp=Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar;
                var full=Path.GetFullPath(Root); Assert.That(full.StartsWith(temp,StringComparison.OrdinalIgnoreCase),Is.True,"cleanup is confined to this owned temp descendant");
                if(Directory.Exists(full) && (File.GetAttributes(full)&FileAttributes.ReparsePoint)==0) Directory.Delete(full,true);
            }
        }
        private sealed class EnvironmentPort : IDesktopProfileLaunchEnvironmentV1 { private readonly string _root; internal EnvironmentPort(string root){_root=root;} public string GetPersistentDataPath(){return _root;}public DateTimeOffset GetUtcNow(){return new DateTimeOffset(2026,9,28,0,0,0,TimeSpan.Zero);} }
        private sealed class PreparationPort : IDesktopProfileLaunchPreparationV1 { public PreparedProfileLaunchV1 Prepare(string root,DateTimeOffset utc){return ProfileLaunchPreparationCoordinatorV1.Prepare(root,utc);} }
    }
}
