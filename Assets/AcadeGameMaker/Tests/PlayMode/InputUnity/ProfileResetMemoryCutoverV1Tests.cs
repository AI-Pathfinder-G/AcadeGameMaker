using NUnit.Framework;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Input;
using System.Reflection;
using System.IO;
using System;
using System.Threading;
using AcadeGameMaker.Profile;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-M5D7QC2-003..007; AC-M5D7QC2-003/004/006/008/009. These focused
    // hostile rows complement the Profile disk matrix; they do not claim the
    // full C2 end-to-end fault matrix has been executed.
    internal sealed class ProfileResetMemoryCutoverV1Tests
    {
        [Test]
        public void AC_M5D7QC2_009_ResultRejectsDefaultAndSuccessWithoutReceipt()
        {
            Assert.Throws<System.ArgumentException>(() => new ProfileResetMemoryResultV1(default(ProfileResetMemoryOutcomeV1)));
            Assert.Throws<System.ArgumentException>(() => new ProfileResetMemoryResultV1(ProfileResetMemoryOutcomeV1.Completed));
            Assert.That(new ProfileResetMemoryResultV1(ProfileResetMemoryOutcomeV1.ReloadRequired).Receipt, Is.Null);
        }

        [Test]
        public void AC_M5D7QC2_004_ResultGetterRevalidatesReflectedOutcome()
        {
            var result = new ProfileResetMemoryResultV1(ProfileResetMemoryOutcomeV1.ReloadRequired);
            typeof(ProfileResetMemoryResultV1).GetField("_outcome", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(result, default(ProfileResetMemoryOutcomeV1));
            Assert.That(() => { var ignored = result.Outcome; }, Throws.ArgumentException);
            Assert.That(() => { var ignored = result.Receipt; }, Throws.ArgumentException);
        }

        [Test]
        public void AC_M5D7QC2_006_ReceiptCannotBeConstructedFromArbitraryScalars()
        {
            Assert.That(typeof(ProfileResetMemoryReceiptV1).GetConstructors(BindingFlags.Instance | BindingFlags.Public), Is.Empty);
            Assert.That(typeof(ProfileResetMemoryReceiptV1).GetMethod("CreateFinal", BindingFlags.Static | BindingFlags.NonPublic), Is.Not.Null,
                "Only the final disk proof + exact pair factory may mint a completed receipt.");
        }

        [Test]
        public void AC_M5D7QC2_009_ReceiptDoesNotRetainLiveLeaseOrCutoverAuthority()
        {
            var source = File.ReadAllText(Path.Combine(FindProjectRoot(), "Assets", "AcadeGameMaker", "Runtime", "Input", "Unity", "ProfileResetMemoryCutoverV1.cs"));
            StringAssert.DoesNotContain("private readonly ResetCutoverCapabilityV1 _capability", source);
            StringAssert.DoesNotContain("private readonly ValidatedResetMemoryAuthorityV1", source);
            StringAssert.DoesNotContain("private readonly ProfileRootOperationLockV1", source);
        }

        [Test]
        public void AC_M5D7QC2_001_Through006_ActualHubLaunchC1AndC2HappyPath()
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C2-happy-" + Guid.NewGuid().ToString("N"));
            var parent = new GameObject("c2-happy-parent"); parent.SetActive(false);
            try
            {
                var routerObject = new GameObject("router"); routerObject.transform.SetParent(parent.transform); var router = routerObject.AddComponent<InputRouter>(); router.ConfigureHubUiOnlyForAuthoring();
                var adapterObject = new GameObject("adapter"); adapterObject.transform.SetParent(parent.transform); var adapter = adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>();
                adapter.ConfigureForTests(router, new FixedEnvironment(root), new RealPreparation());
                parent.SetActive(true);
                typeof(DesktopProfileLaunchAdapterV1).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(adapter, null);
                var historical = adapter.CurrentReceipt; Assert.That(historical.HasValue, Is.True);
                var forensicReceipt = typeof(InputRouter).GetField("_currentReceipt", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router);
                var forensicFrame = typeof(InputRouter).GetField("_currentUiFrame", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router);
                var oldActions = (GameInputActions)typeof(InputRouter).GetField("_actions", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router);
                var disk = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root)); Assert.That(disk.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(root, disk.PreparedProof, adapter, router);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.Completed)); Assert.That(result.Receipt, Is.Not.Null);
                var current = adapter.CurrentResetSession; Assert.That(current.Root, Is.EqualTo(ProfileResetBarrierV1.NormalizeRoot(root))); CollectionAssert.AreEqual(ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes, current.CopyCanonicalDocument());
                var newActions = (GameInputActions)typeof(InputRouter).GetField("_actions", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router);
                Assert.That(newActions, Is.Not.SameAs(oldActions)); router.ValidateAllMapsDisabled(newActions); Assert.That((bool)typeof(InputRouter).GetField("_resetOldDisposeAttempted", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router), Is.True);
                CollectionAssert.AreEqual(ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(root, "profile.json"))); Assert.That(File.Exists(Path.Combine(root, "profile.prev.json")), Is.False); Assert.That(File.Exists(Path.Combine(root, "profile.tmp.json")), Is.False); Assert.That(File.Exists(Path.Combine(root, ProfileResetBarrierV1.BarrierName)), Is.False); Assert.That(File.Exists(Path.Combine(root, ProfileResetBarrierV1.BarrierTempName)), Is.False);
                Assert.That(adapter.CurrentReceipt.Value.FinalRevision, Is.EqualTo(historical.Value.FinalRevision)); CollectionAssert.AreEqual(adapter.CurrentReceipt.Value.FinalDocument.CanonicalFileBytes, historical.Value.FinalDocument.CanonicalFileBytes);
                Assert.That(typeof(InputRouter).GetField("_currentReceipt", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router), Is.EqualTo(forensicReceipt));
                Assert.That(typeof(InputRouter).GetField("_currentUiFrame", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router), Is.EqualTo(forensicFrame));
                ProfileResetMemoryReceiptV1 taken; Assert.That(adapter.TryTakeResetReceipt(out taken), Is.True); Assert.That(taken, Is.SameAs(result.Receipt)); Assert.That(adapter.TryTakeResetReceipt(out taken), Is.False);
                Assert.Throws<InvalidOperationException>(() => { var ignored = router.CurrentReceipt; }); Assert.Throws<InvalidOperationException>(() => { var ignored = router.CurrentUiFrame; }); result.Receipt.Validate();
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(parent);
                var temp = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                Assert.That(Path.GetFullPath(root).StartsWith(temp, StringComparison.OrdinalIgnoreCase), Is.True, "cleanup may only delete this owned OS-temp descendant");
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }

        // REQ-M5D7QC2-003/004/007; AC-M5D7QC2-003/004/008.  These are real
        // HubUIOnly/C1/C2 rows.  The closed control can only throw immediately
        // before or after the exact production-owned operation it is given.
        [TestCase(ResetCutoverOperationV1.CancelUiEnableQuarantine, false)]
        [TestCase(ResetCutoverOperationV1.CancelUiEnableQuarantine, true)]
        [TestCase(ResetCutoverOperationV1.DisposeOld, false)]
        [TestCase(ResetCutoverOperationV1.DisposeOld, true)]
        public void AC_M5D7QC2_003_004_008_ActualOwnedOperationFaultsFailStopAndRetainHistory(
            ResetCutoverOperationV1 faultOperation, bool throwAfterExactOwnedOperation)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C2-operation-" + Guid.NewGuid().ToString("N"));
            var parent = new GameObject("c2-operation-parent"); parent.SetActive(false);
            var control = new ExactOwnedOperationFaultControl(faultOperation, throwAfterExactOwnedOperation);
            try
            {
                var routerObject = new GameObject("router"); routerObject.transform.SetParent(parent.transform); var router = routerObject.AddComponent<InputRouter>(); router.ConfigureHubUiOnlyForAuthoring();
                var adapterObject = new GameObject("adapter"); adapterObject.transform.SetParent(parent.transform); var adapter = adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>();
                adapter.ConfigureForTests(router, new FixedEnvironment(root), new RealPreparation());
                parent.SetActive(true);
                typeof(DesktopProfileLaunchAdapterV1).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(adapter, null);

                var historicalLaunch = adapter.CurrentReceipt; Assert.That(historicalLaunch.HasValue, Is.True);
                var historicalRouterReceipt = typeof(InputRouter).GetField("_currentReceipt", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router);
                var historicalRouterFrame = typeof(InputRouter).GetField("_currentUiFrame", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router);
                var disk = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                Assert.That(disk.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));

                var result = ProfileResetMemoryCutoverV1.FinalizeReset(root, disk.PreparedProof, adapter, router, control);

                Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.ReloadRequired));
                Assert.That(result.Receipt, Is.Null);
                Assert.That(control.OperationInvocations(faultOperation), Is.EqualTo(1));
                Assert.That(control.ExactOwnedInvocations(faultOperation), Is.EqualTo(throwAfterExactOwnedOperation ? 1 : 0));
                foreach (ResetCutoverOperationV1 operation in Enum.GetValues(typeof(ResetCutoverOperationV1)))
                    Assert.That(control.OperationInvocations(operation), Is.LessThanOrEqualTo(1), "production must not repeat " + operation);
                Assert.That(control.OperationInvocations(ResetCutoverOperationV1.DisposeOld), Is.LessThanOrEqualTo(1));
                Assert.That(control.OperationInvocations(ResetCutoverOperationV1.DisposeNew), Is.LessThanOrEqualTo(1));

                Assert.That(File.Exists(Path.Combine(root, ProfileResetBarrierV1.BarrierName)), Is.True, "the pre-delete barrier must remain after a terminal operation fault");
                Assert.That(File.Exists(Path.Combine(root, ProfileResetBarrierV1.BarrierTempName)), Is.False);
                Assert.That(adapter.CurrentReceipt.Value.FinalRevision, Is.EqualTo(historicalLaunch.Value.FinalRevision));
                CollectionAssert.AreEqual(adapter.CurrentReceipt.Value.FinalDocument.CanonicalFileBytes, historicalLaunch.Value.FinalDocument.CanonicalFileBytes);
                Assert.That(typeof(InputRouter).GetField("_currentReceipt", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router), Is.EqualTo(historicalRouterReceipt));
                Assert.That(typeof(InputRouter).GetField("_currentUiFrame", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router), Is.EqualTo(historicalRouterFrame));
                Assert.Throws<InvalidOperationException>(() => { var ignored = router.CurrentReceipt; });
                Assert.Throws<InvalidOperationException>(() => { var ignored = router.CurrentUiFrame; });

                var oldDisposeAttempted = ResetDisposeAttempted(router, "_resetOldDisposeAttempted");
                var newDisposeAttempted = ResetDisposeAttempted(router, "_resetNewDisposeAttempted");
                Assert.That(oldDisposeAttempted, Is.True);
                Assert.That(newDisposeAttempted, Is.True);
                parent.SetActive(false); // Exercises the terminal OnDisable containment path.
                Assert.That(ResetDisposeAttempted(router, "_resetOldDisposeAttempted"), Is.EqualTo(oldDisposeAttempted));
                Assert.That(ResetDisposeAttempted(router, "_resetNewDisposeAttempted"), Is.EqualTo(newDisposeAttempted));
                var operationCountsBeforeDestroy = control.TotalOperationInvocations;
                UnityEngine.Object.DestroyImmediate(parent); parent = null; // Exercises terminal OnDestroy without a second owned-operation wrapper call.
                Assert.That(control.TotalOperationInvocations, Is.EqualTo(operationCountsBeforeDestroy));
            }
            finally
            {
                if (parent != null) UnityEngine.Object.DestroyImmediate(parent);
                DeleteOwnedNonReparseTempRoot(root);
            }
        }

        private sealed class FixedEnvironment : IDesktopProfileLaunchEnvironmentV1
        {
            private readonly string _root; internal FixedEnvironment(string root) { _root = root; }
            public string GetPersistentDataPath() { return _root; } public DateTimeOffset GetUtcNow() { return new DateTimeOffset(2026, 9, 28, 0, 0, 0, TimeSpan.Zero); }
        }
        private sealed class RealPreparation : IDesktopProfileLaunchPreparationV1
        { public PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc) { return ProfileLaunchPreparationCoordinatorV1.Prepare(root, utc); } }

        // REQ-M5D7QC2-003/004/007; AC-M5D7QC2-003/004/008/009.  These use the
        // real Hub launch and C1 proof path; their foreign wrappers have an
        // enabled UI map so containment touching them is observable.
        [Test]
        public void AC_M5D7QC2_003_CleanForeignRouterAndRootRejectWithoutConsumingProof()
        {
            using (var fixture = ActualLaunchFixture.Create("clean-foreign-original"))
            using (var foreign = ActualLaunchFixture.Create("clean-foreign-second"))
            {
                var disk = PrepareDisk(fixture.Root);
                Assert.Throws<InvalidOperationException>(() => ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, foreign.Router));
                Assert.Throws<InvalidOperationException>(() => ProfileResetMemoryCutoverV1.FinalizeReset(foreign.Root, disk.PreparedProof, fixture.Adapter, fixture.Router));
                Assert.That(fixture.Adapter.IsResetCutoverTerminal, Is.False, "clean protocol rejection must not enter the original gate");
                Assert.That(foreign.Actions.UI.Get().enabled, Is.True, "clean foreign input must not be contained");

                // The same actual C1 proof proves neither clean rejection consumed it.
                var complete = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router);
                Assert.That(complete.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.Completed));
                Assert.That(foreign.Actions.UI.Get().enabled, Is.True);
            }
        }

        [TestCase("router")]
        [TestCase("root")]
        [TestCase("actions")]
        public void AC_M5D7QC2_003_CoordinatedLaunchWitnessCorruptionContainsOnlyOriginalCohort(string corruption)
        {
            using (var fixture = ActualLaunchFixture.Create("cohort-corrupt-original"))
            using (var foreign = ActualLaunchFixture.Create("cohort-corrupt-second"))
            {
                var historicalReceipt = fixture.Adapter.CurrentReceipt;
                var historicalRouterReceipt = PrivateField(fixture.Router, "_currentReceipt");
                var historicalFrame = PrivateField(fixture.Router, "_currentUiFrame");
                if (corruption == "router")
                {
                    SetPrivateField(fixture.Adapter, "_router", foreign.Router);
                    SetPrivateField(fixture.Adapter, "_launchRouterWitness", foreign.Router);
                    SetPrivateField(fixture.Adapter, "_launchRouterWitnessB", foreign.Router);
                    SetPrivateField(fixture.Adapter, "_launchRootWitnessA", foreign.Root);
                    SetPrivateField(fixture.Adapter, "_launchRootWitnessB", foreign.Root);
                    SetPrivateField(fixture.Adapter, "_currentReceipt", foreign.Adapter.CurrentReceipt);
                    SetPrivateField(fixture.Adapter, "_launchReceiptWitness", foreign.Adapter.CurrentReceipt);
                }
                else if (corruption == "root")
                {
                    SetPrivateField(fixture.Adapter, "_launchRootWitnessA", foreign.Root);
                    SetPrivateField(fixture.Adapter, "_launchRootWitnessB", foreign.Root);
                }
                else
                {
                    SetPrivateField(fixture.Router, "_actions", foreign.Actions);
                    SetPrivateField(fixture.Router, "_preparedActionsProof", foreign.Actions);
                    SetPrivateField(fixture.Router, "_preparedActionsContainmentProof", foreign.Actions);
                }

                var disk = PrepareDisk(fixture.Root);
                AssertOriginalFaultOrClosedFailure(fixture, foreign, disk.PreparedProof, historicalReceipt, historicalRouterReceipt, historicalFrame, null, corruption != "router");
            }
        }

        [Test]
        public void AC_M5D7QC2_003_AfterTerminalGateAdapterCohortSubstitutionContainsOnlyOriginal()
        {
            using (var fixture = ActualLaunchFixture.Create("after-gate-adapter-original"))
            using (var foreign = ActualLaunchFixture.Create("after-gate-adapter-foreign"))
            {
                var control = new AfterTerminalGateAdapterCorruptionControl(fixture.Adapter, foreign);
                var disk = PrepareDisk(fixture.Root);
                AssertOriginalFaultOrClosedFailure(fixture, foreign, disk.PreparedProof, fixture.Adapter.CurrentReceipt, PrivateField(fixture.Router, "_currentReceipt"), PrivateField(fixture.Router, "_currentUiFrame"), control, false);
                fixture.DestroyForTerminalAssertion();
                Assert.That(control.DisposeOldCalls, Is.LessThanOrEqualTo(1)); Assert.That(control.DisposeNewCalls, Is.LessThanOrEqualTo(1));
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void AC_M5D7QC2_003_AfterTerminalGateRouterDiagnosticCorruptionContainsOnlyOriginal(bool attempted)
        {
            using (var fixture = ActualLaunchFixture.Create("after-gate-router-original"))
            using (var foreign = ActualLaunchFixture.Create("after-gate-router-foreign"))
            {
                var control = new AfterTerminalGateRouterCorruptionControl(fixture.Router, foreign.Actions, attempted);
                var disk = PrepareDisk(fixture.Root);
                AssertOriginalFaultOrClosedFailure(fixture, foreign, disk.PreparedProof, fixture.Adapter.CurrentReceipt, PrivateField(fixture.Router, "_currentReceipt"), PrivateField(fixture.Router, "_currentUiFrame"), control);
                fixture.DestroyForTerminalAssertion();
                Assert.That(control.DisposeOldCalls, Is.LessThanOrEqualTo(1)); Assert.That(control.DisposeNewCalls, Is.LessThanOrEqualTo(1));
            }
        }

        [Test]
        public void AC_M5D7QC2_004_OriginalOldCloseCwtGateHasOneConcurrentWinnerAfterRealTransfer()
        {
            using (var fixture = ActualLaunchFixture.Create("atomic-old-close-original"))
            using (var foreign = ActualLaunchFixture.Create("atomic-old-close-foreign"))
            {
                var old = fixture.Actions;
                var control = new ConcurrentOldCloseControl(fixture.Router, old);
                var disk = PrepareDisk(fixture.Root);
                var result = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router, control);
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.ReloadRequired));
                Assert.That(result.Receipt, Is.Null); Assert.That(fixture.Adapter.IsResetCutoverTerminal, Is.True);
                Assert.That(control.Winners, Is.EqualTo(1));
                Assert.That(control.MainThreadDisposeCalls, Is.EqualTo(1));
                Assert.That(foreign.Actions.UI.Get().enabled, Is.True, "the original CWT close gate must never claim a foreign cohort");
                Assert.That(File.Exists(Path.Combine(fixture.Root, ProfileResetBarrierV1.BarrierName)), Is.True);
            }
        }

        [Test]
        public void AC_M5D7QC2_003_004_008_CompletedQuietTickThenReflectedTerminalReplayCannotReopenAuthority()
        {
            using (var fixture = ActualLaunchFixture.Create("completed-replay"))
            {
                var historicalReceipt = fixture.Adapter.CurrentReceipt;
                var historicalRouterReceipt = PrivateField(fixture.Router, "_currentReceipt");
                var historicalFrame = PrivateField(fixture.Router, "_currentUiFrame");
                var launchNotification = PrivateField(fixture.Adapter, "_notification");
                var disk = PrepareDisk(fixture.Root);
                var completed = ProfileResetMemoryCutoverV1.FinalizeReset(fixture.Root, disk.PreparedProof, fixture.Adapter, fixture.Router);
                Assert.That(completed.Outcome, Is.EqualTo(ProfileResetMemoryOutcomeV1.Completed));
                var retained = fixture.Adapter.CurrentResetSession;

                typeof(InputRouter).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(fixture.Router, null);
                Assert.DoesNotThrow(() => { var root = retained.Root; });
                Assert.That(retained.Root, Is.EqualTo(ProfileResetBarrierV1.NormalizeRoot(fixture.Root)));

                SetPrivateField(fixture.Adapter, "_resetCutoverTerminal", false);
                var lifecycle = typeof(DesktopProfileLaunchAdapterV1).GetField("_resetLifecycle", BindingFlags.Instance | BindingFlags.NonPublic);
                lifecycle.SetValue(fixture.Adapter, Enum.ToObject(lifecycle.FieldType, 0));
                SetPrivateField(fixture.Adapter, "_notification", launchNotification);

                Assert.Throws<InvalidOperationException>(() => { var ignored = fixture.Adapter.CurrentResetSession; });
                Assert.Throws<InvalidOperationException>(() => { var ignored = retained.Root; });
                Assert.Throws<InvalidOperationException>(() => { var ignored = fixture.Router.CurrentUiFrame; });
                ProfileLaunchNotificationV1 replayed;
                Assert.That(fixture.Adapter.TryTakeNotification(out replayed), Is.False, "the immutable cohort terminal phase blocks reflected notification replay");
                Assert.That(fixture.Adapter.CurrentReceipt, Is.EqualTo(historicalReceipt));
                Assert.That(PrivateField(fixture.Router, "_currentReceipt"), Is.EqualTo(historicalRouterReceipt));
                Assert.That(PrivateField(fixture.Router, "_currentUiFrame"), Is.EqualTo(historicalFrame));
                completed.Receipt.Validate();
            }
        }

        private sealed class ActualLaunchFixture : IDisposable
        {
            internal readonly string Root; internal readonly GameObject Parent; internal readonly InputRouter Router; internal readonly DesktopProfileLaunchAdapterV1 Adapter; private bool _destroyed;
            private ActualLaunchFixture(string root, GameObject parent, InputRouter router, DesktopProfileLaunchAdapterV1 adapter) { Root=root; Parent=parent; Router=router; Adapter=adapter; }
            internal GameInputActions Actions { get { return (GameInputActions)PrivateField(Router, "_actions"); } }
            internal static ActualLaunchFixture Create(string suffix)
            {
                var root=Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C2-" + suffix + "-" + Guid.NewGuid().ToString("N")); var parent=new GameObject("c2-fixture"); parent.SetActive(false);
                var ro=new GameObject("router"); ro.transform.SetParent(parent.transform); var router=ro.AddComponent<InputRouter>(); router.ConfigureHubUiOnlyForAuthoring();
                var ao=new GameObject("adapter"); ao.transform.SetParent(parent.transform); var adapter=ao.AddComponent<DesktopProfileLaunchAdapterV1>(); adapter.ConfigureForTests(router,new FixedEnvironment(root),new RealPreparation());
                parent.SetActive(true); typeof(DesktopProfileLaunchAdapterV1).GetMethod("Start",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(adapter,null);
                Assert.That(adapter.CurrentReceipt.HasValue, Is.True); return new ActualLaunchFixture(root,parent,router,adapter);
            }
            internal void DestroyForTerminalAssertion() { if (_destroyed) return; UnityEngine.Object.DestroyImmediate(Parent); _destroyed=true; }
            public void Dispose() { if (!_destroyed) UnityEngine.Object.DestroyImmediate(Parent); DeleteOwnedNonReparseTempRoot(Root); }
        }

        private static ProfileResetDiskResultV1 PrepareDisk(string root)
        {
            var disk = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
            Assert.That(disk.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
            return disk;
        }

        private static void AssertOriginalFaultOrClosedFailure(
            ActualLaunchFixture original, ActualLaunchFixture foreign, ProfileResetDiskPreparedProofV1 proof,
            DesktopProfileLaunchReceiptV1? historicalReceipt, object historicalRouterReceipt, object historicalFrame,
            IProfileResetMemoryCutoverControlV1 control = null, bool adapterHistoryReadable = true)
        {
            ProfileResetMemoryResultV1 result = null;
            Exception thrown = null;
            try { result = control == null ? ProfileResetMemoryCutoverV1.FinalizeReset(original.Root, proof, original.Adapter, original.Router) : ProfileResetMemoryCutoverV1.FinalizeReset(original.Root, proof, original.Adapter, original.Router, control); }
            catch (InvalidOperationException e) { thrown = e; }
            Assert.That(thrown != null || (result != null && result.Outcome != ProfileResetMemoryOutcomeV1.Completed), Is.True, "cohort corruption cannot produce a completed row");
            Assert.That(original.Adapter.IsResetCutoverTerminal, Is.True, "only the original private cohort must close");
            Assert.That(foreign.Adapter.IsResetCutoverTerminal, Is.False);
            Assert.That(foreign.Actions.UI.Get().enabled, Is.True, "foreign actions remain live and untouched");
            Assert.That(File.Exists(Path.Combine(original.Root, ProfileResetBarrierV1.BarrierName)), Is.True, "a corrupt memory row retains C1's barrier");
            if (adapterHistoryReadable) Assert.That(original.Adapter.CurrentReceipt, Is.EqualTo(historicalReceipt), "launch history remains forensic evidence");
            else Assert.Throws<InvalidOperationException>(() => { var ignored = original.Adapter.CurrentReceipt; }, "corrupt mutable receipt fields must not replace original CWT history");
            Assert.That(PrivateField(original.Router, "_currentReceipt"), Is.EqualTo(historicalRouterReceipt));
            Assert.That(PrivateField(original.Router, "_currentUiFrame"), Is.EqualTo(historicalFrame));
        }

        private sealed class AfterTerminalGateAdapterCorruptionControl : IProfileResetMemoryCutoverControlV1
        {
            private readonly DesktopProfileLaunchAdapterV1 _original; private readonly ActualLaunchFixture _foreign; private bool _fired; private int _disposeOld, _disposeNew;
            internal AfterTerminalGateAdapterCorruptionControl(DesktopProfileLaunchAdapterV1 original, ActualLaunchFixture foreign) { _original=original; _foreign=foreign; }
            internal int DisposeOldCalls { get { return _disposeOld; } } internal int DisposeNewCalls { get { return _disposeNew; } }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 checkpoint)
            {
                if (_fired || checkpoint != ProfileResetMemoryCutoverCheckpointV1.AfterTerminalGate) return;
                _fired=true;
                SetPrivateField(_original, "_router", _foreign.Router);
                SetPrivateField(_original, "_launchRouterWitness", _foreign.Router);
                SetPrivateField(_original, "_launchRouterWitnessB", _foreign.Router);
                SetPrivateField(_original, "_launchRootWitnessA", _foreign.Root);
                SetPrivateField(_original, "_launchRootWitnessB", _foreign.Root);
                SetPrivateField(_original, "_currentReceipt", _foreign.Adapter.CurrentReceipt);
                SetPrivateField(_original, "_launchReceiptWitness", _foreign.Adapter.CurrentReceipt);
                throw new InvalidOperationException("Injected after real terminal gate.");
            }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exactOwnedOperation) { if(operation==ResetCutoverOperationV1.DisposeOld)_disposeOld++; if(operation==ResetCutoverOperationV1.DisposeNew)_disposeNew++; exactOwnedOperation(); }
        }

        private sealed class AfterTerminalGateRouterCorruptionControl : IProfileResetMemoryCutoverControlV1
        {
            private readonly InputRouter _original; private readonly GameInputActions _foreign; private readonly bool _attempted; private bool _fired; private int _disposeOld, _disposeNew;
            internal AfterTerminalGateRouterCorruptionControl(InputRouter original, GameInputActions foreign, bool attempted) { _original=original; _foreign=foreign; _attempted=attempted; }
            internal int DisposeOldCalls { get { return _disposeOld; } } internal int DisposeNewCalls { get { return _disposeNew; } }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 checkpoint)
            {
                if (_fired || checkpoint != ProfileResetMemoryCutoverCheckpointV1.AfterTerminalGate) return;
                _fired=true;
                SetPrivateField(_original, "_resetOldActions", _foreign);
                SetPrivateField(_original, "_resetNewActions", _foreign);
                SetPrivateField(_original, "_actions", _foreign);
                SetPrivateField(_original, "_resetOldDisposeAttempted", _attempted);
                SetPrivateField(_original, "_resetNewDisposeAttempted", _attempted);
                throw new InvalidOperationException("Injected after real terminal gate.");
            }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exactOwnedOperation) { if(operation==ResetCutoverOperationV1.DisposeOld)_disposeOld++; if(operation==ResetCutoverOperationV1.DisposeNew)_disposeNew++; exactOwnedOperation(); }
        }

        private sealed class ConcurrentOldCloseControl : IProfileResetMemoryCutoverControlV1
        {
            private readonly InputRouter _router; private readonly GameInputActions _old; private int _winners; private int _mainThreadDisposeCalls; private bool _fired;
            internal ConcurrentOldCloseControl(InputRouter router, GameInputActions old) { _router=router; _old=old; }
            internal int Winners { get { return Volatile.Read(ref _winners); } }
            internal int MainThreadDisposeCalls { get { return Volatile.Read(ref _mainThreadDisposeCalls); } }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 checkpoint)
            {
                if (_fired || checkpoint != ProfileResetMemoryCutoverCheckpointV1.BeforeOldDispose) return;
                _fired=true;
                var ready = new CountdownEvent(2); var release = new ManualResetEventSlim(false);
                ThreadStart claim = () => { ready.Signal(); release.Wait(); if (DesktopProfileLaunchAdapterV1.TryBeginOriginalDispose(_router, false)) Interlocked.Increment(ref _winners); };
                var one = new Thread(claim); var two = new Thread(claim); one.Start(); two.Start();
                Assert.That(ready.Wait(TimeSpan.FromSeconds(5)), Is.True);
                release.Set(); Assert.That(one.Join(5000), Is.True); Assert.That(two.Join(5000), Is.True);
                Assert.That(Winners, Is.EqualTo(1), "the private original-old close claim is one-shot under concurrent callers");
                _old.Dispose(); Interlocked.Increment(ref _mainThreadDisposeCalls);
            }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exactOwnedOperation) { exactOwnedOperation(); }
        }

        private sealed class ExactOwnedOperationFaultControl : IProfileResetMemoryCutoverControlV1
        {
            private readonly ResetCutoverOperationV1 _faultOperation; private readonly bool _throwAfter;
            private readonly int[] _operationCounts = new int[12]; private readonly int[] _exactOwnedCounts = new int[12];
            internal ExactOwnedOperationFaultControl(ResetCutoverOperationV1 faultOperation, bool throwAfter) { _faultOperation = faultOperation; _throwAfter = throwAfter; }
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 checkpoint) { }
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 checkpoint) { }
            public void InvokeOperation(ResetCutoverOperationV1 operation, Action exactOwnedOperation)
            {
                Assert.That(exactOwnedOperation, Is.Not.Null); var index = (int)operation; _operationCounts[index]++;
                if (operation == _faultOperation && !_throwAfter) throw new InvalidOperationException("Injected before exact owned operation.");
                _exactOwnedCounts[index]++; exactOwnedOperation();
                if (operation == _faultOperation && _throwAfter) throw new InvalidOperationException("Injected after exact owned operation.");
            }
            internal int OperationInvocations(ResetCutoverOperationV1 operation) { return _operationCounts[(int)operation]; }
            internal int ExactOwnedInvocations(ResetCutoverOperationV1 operation) { return _exactOwnedCounts[(int)operation]; }
            internal int TotalOperationInvocations { get { var total = 0; for (var i = 1; i < _operationCounts.Length; i++) total += _operationCounts[i]; return total; } }
        }

        private static bool ResetDisposeAttempted(InputRouter router, string fieldName)
        {
            return (bool)typeof(InputRouter).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(router);
        }

        private static object PrivateField(object target, string fieldName)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, "required private diagnostic field is absent: " + fieldName);
            return field.GetValue(target);
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null, "required private diagnostic field is absent: " + fieldName);
            field.SetValue(target, value);
        }

        private static void DeleteOwnedNonReparseTempRoot(string root)
        {
            var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var fullTemp = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            Assert.That(fullRoot.StartsWith(fullTemp, StringComparison.OrdinalIgnoreCase), Is.True, "cleanup may only delete this owned OS-temp descendant");
            if (!Directory.Exists(fullRoot)) return;
            Assert.That((File.GetAttributes(fullRoot) & FileAttributes.ReparsePoint) == 0, Is.True, "cleanup refuses a reparse root");
            Directory.Delete(fullRoot, true);
        }

        private static string FindProjectRoot()
        {
            var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "Assets", "AcadeGameMaker", "Runtime", "Input", "Unity", "ProfileResetMemoryCutoverV1.cs"))) return directory.FullName;
                directory = directory.Parent;
            }
            Assert.Fail("Project root was not found."); return null;
        }
    }
}
