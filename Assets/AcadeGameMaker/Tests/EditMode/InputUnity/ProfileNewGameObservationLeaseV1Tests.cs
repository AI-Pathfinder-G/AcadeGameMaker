using System;
using System.IO;
using System.Reflection;
using System.Threading;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.InputUnity
{
    // AC-M5D7QC3L-002..004 / AC-M5D7QC3-001: every row enters the real lower
    // C3 capture through an actually launched adapter/router cohort.
    internal sealed class ProfileNewGameObservationLeaseV1Tests
    {
        [Test]
        public void AC002_ActualHeldLockTimesOutOnlyThroughLowerBusy()
        {
            using (var fixture = Fixture.Create())
            using (new FileStream(Path.Combine(fixture.Root, "profile.operation.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var started = Environment.TickCount;
                var row = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, new object(), new object());
                Assert.That(row.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Busy));
                Assert.That(unchecked(Environment.TickCount - started), Is.GreaterThanOrEqualTo(5000));
            }
        }

        [Test]
        public void AC002_ActualHeldLockReleasedEarlyCapturesBeforeDeadline()
        {
            using (var fixture = Fixture.Create())
            using (var acquired = new ManualResetEventSlim(false))
            using (var release = new ManualResetEventSlim(false))
            {
                Exception workerError = null;
                var worker = new Thread(() => { try { using (new FileStream(Path.Combine(fixture.Root, "profile.operation.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) { acquired.Set(); release.Wait(); } } catch (Exception e) { workerError = e; acquired.Set(); } }) { IsBackground = true };
                worker.Start(); Assert.That(acquired.Wait(1000), Is.True, "worker acquired actual lock"); Assert.That(workerError, Is.Null);
                var releaser = new Thread(() => { Thread.Sleep(150); release.Set(); }) { IsBackground = true }; releaser.Start();
                try { var started = Environment.TickCount; var row = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, new object(), new object()); Assert.That(row.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Captured)); Assert.That(unchecked(Environment.TickCount - started), Is.LessThan(5000)); }
                finally { release.Set(); Assert.That(worker.Join(1000), Is.True, "worker joined"); Assert.That(releaser.Join(1000), Is.True, "releaser joined"); }
                Assert.That(workerError, Is.Null);
            }
        }

        [Test]
        public void AC003_OnlyActualObservationTimeoutIssuerAuthenticatesProvenance()
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C3L-timeout-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            try
            {
                var lockPath = Path.Combine(root, "profile.operation.lock");
                using (new FileStream(lockPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    var assembly = typeof(ProfileRecoveryPlannerV1).Assembly;
                    var lockType = assembly.GetType("AcadeGameMaker.Profile.ProfileRootOperationLockV1", true);
                    var acquire = lockType.GetMethod("AcquireForObservation", BindingFlags.Static | BindingFlags.NonPublic);
                    var verify = lockType.GetMethod("IsAuthenticatedObservationContentionTimeout", BindingFlags.Static | BindingFlags.NonPublic);
                    var thrown = Assert.Throws<TargetInvocationException>(() => acquire.Invoke(null, new object[] { root })).InnerException as IOException;
                    Assert.That(thrown, Is.Not.Null); Assert.That((bool)verify.Invoke(null, new object[] { thrown }), Is.True);
                    var cause = (IOException)thrown.InnerException; var causeHash = cause.HResult; var wrapperHash = thrown.HResult;
                    SetHResult(cause, causeHash == unchecked((int)0x80070020) ? unchecked((int)0x80070021) : unchecked((int)0x80070020)); Assert.That((bool)verify.Invoke(null, new object[] { thrown }), Is.False); SetHResult(cause, causeHash); Assert.That((bool)verify.Invoke(null, new object[] { thrown }), Is.True);
                    SetHResult(thrown, wrapperHash + 1); Assert.That((bool)verify.Invoke(null, new object[] { thrown }), Is.False); SetHResult(thrown, wrapperHash); Assert.That((bool)verify.Invoke(null, new object[] { thrown }), Is.True);
                    var fake = new IOException("fake"); SetHResult(fake, unchecked((int)0x80070020)); Assert.That((bool)verify.Invoke(null, new object[] { fake }), Is.False); Assert.That((bool)verify.Invoke(null, new object[] { null }), Is.False);
                    var nested = lockType.GetNestedType("ObservationContentionTimeoutException", BindingFlags.NonPublic);
                    var forged = (IOException)Activator.CreateInstance(nested, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { fake }, null);
                    Assert.That((bool)verify.Invoke(null, new object[] { forged }), Is.False);
                }
            }
            finally { DeleteOwnedTemporaryRoot(root); }
        }

        [Test]
        public void AC003_ObservationRejectsRelativeAndFilesystemRootWithoutWrites()
        {
            var assembly = typeof(ProfileRecoveryPlannerV1).Assembly;
            var transaction = assembly.GetType("AcadeGameMaker.Profile.ProfileResetDiskTransactionV1", true);
            var capture = transaction.GetMethod("CaptureConfirmationObservation", BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var path in new[] { "relative-profile-root", Path.GetPathRoot(Path.GetTempPath()) })
            {
                var error = Assert.Throws<TargetInvocationException>(() => capture.Invoke(null, new object[] { path }));
                Assert.That(error.InnerException, Is.TypeOf<ArgumentException>().Or.InstanceOf<IOException>());
            }
            Assert.That(Directory.Exists(Path.Combine(ProjectRoot(), "relative-profile-root")), Is.False);
        }

        [Test]
        public void AC003_RootFileAndLockDirectoryAreUnreadableWithoutCaptureEvidence()
        {
            using (var fixture = Fixture.Create())
            {
                Directory.Delete(fixture.Root, true); File.WriteAllText(fixture.Root, "root-file");
                var rootFile = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, new object(), new object());
                Assert.That(rootFile.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Unreadable));
            }
            using (var fixture = Fixture.Create())
            {
                var lockPath = Path.Combine(fixture.Root, "profile.operation.lock");
                Assert.That(File.Exists(lockPath), Is.True, "fixture launch lock file"); File.Delete(lockPath); Assert.That(File.Exists(lockPath), Is.False, "no held lock before directory fixture");
                Directory.CreateDirectory(lockPath);
                var lockDirectory = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, new object(), new object());
                Assert.That(lockDirectory.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Unreadable));
            }
        }

        [Test]
        public void AC003_BarrierFileAndDirectoryAreUnreadableWithoutMutation()
        {
            using (var fixture = Fixture.Create())
            {
                var path = Path.Combine(fixture.Root, "profile.reset.json"); File.WriteAllText(path, "barrier"); var before = File.ReadAllBytes(path);
                Assert.That(ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, new object(), new object()).Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Unreadable));
                CollectionAssert.AreEqual(before, File.ReadAllBytes(path));
            }
            using (var fixture = Fixture.Create())
            {
                var path = Path.Combine(fixture.Root, "profile.reset.json"); Directory.CreateDirectory(path);
                Assert.That(ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, new object(), new object()).Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Unreadable));
                Assert.That(Directory.Exists(path), Is.True);
            }
        }

        [Test]
        public void AC003_HeldPrimaryReadFailureIsUnreadableAndPreservesBytes()
        {
            using (var fixture = Fixture.Create())
            {
                var path = Path.Combine(fixture.Root, "profile.json"); File.WriteAllBytes(path, ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes); var before = File.ReadAllBytes(path);
                using (new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    var row = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, new object(), new object());
                    Assert.That(row.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Unreadable));
                }
                CollectionAssert.AreEqual(before, File.ReadAllBytes(path));
            }
        }

        [Test]
        public void AC004_ActualCaptureIsIssuedAndReflectionCorruptionFailsClosed()
        {
            using (var fixture = Fixture.Create())
            {
                var owner = new object(); var epoch = new object();
                var row = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                Assert.That(row.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Captured));
                typeof(ProfileNewGameCaptureResultV1).GetField("_classification", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(row, NewGameObservationClassificationV1.ConfirmationRequiredMeaningful);
                Assert.That(() => { var ignored = row.Outcome; }, Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC004_CaptureLeafAndProjectionReflectionCorruptionFailsClosed()
        {
            using (var fixture = Fixture.Create())
            {
                var primary = Path.Combine(fixture.Root, "profile.json"); File.WriteAllBytes(primary, ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes); var bytes = File.ReadAllBytes(primary);
                var row = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, new object(), new object()); Assert.That(row.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Captured));
                var capture = Field<object>(row, "_capture"); var identity = Field<object>(capture, "Identity"); var leaves = (Array)Field<object>(identity, "Leaves"); var a=leaves.GetValue(0); var b=leaves.GetValue(1);
                try { leaves.SetValue(b,0); leaves.SetValue(a,1); Assert.That(() => { var x=row.Outcome; }, Throws.InvalidOperationException); } finally { leaves.SetValue(a,0); leaves.SetValue(b,1); }
                Assert.That(row.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Captured)); var hash=Field<string>(a,"Hash");
                try { SetField(a,"Hash",new string('0',64)); Assert.That(() => { var x=row.Outcome; }, Throws.InvalidOperationException); } finally { SetField(a,"Hash",hash); }
                Assert.That(row.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Captured)); var projection=Field<object>(capture,"Primary");
                try { SetField(capture,"Primary",null); Assert.That(() => { var x=row.Outcome; }, Throws.InvalidOperationException); } finally { SetField(capture,"Primary",projection); }
                Assert.That(row.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Captured)); CollectionAssert.AreEqual(bytes,File.ReadAllBytes(primary));
            }
        }

        [Test]
        public void AC001_ForeignRealReceiptAndActionsCannotReturnOriginalRoot()
        {
            using (var original = Fixture.Create())
            using (var foreign = Fixture.Create())
            {
                var current = Field<DesktopProfileLaunchReceiptV1?>(original.Adapter, "_currentReceipt");
                var launch = Field<DesktopProfileLaunchReceiptV1?>(original.Adapter, "_launchReceiptWitness");
                try
                {
                    SetField(original.Adapter, "_currentReceipt", foreign.Adapter.CurrentReceipt);
                    SetField(original.Adapter, "_launchReceiptWitness", foreign.Adapter.CurrentReceipt);
                    Assert.That(() => original.Adapter.GetAuthenticatedLaunchRootForNewGame(original.Router), Throws.InvalidOperationException);
                }
                finally { SetField(original.Adapter, "_currentReceipt", current); SetField(original.Adapter, "_launchReceiptWitness", launch); }

                var originalActions = Field<GameInputActions>(original.Router, "_actions");
                var foreignActions = Field<GameInputActions>(foreign.Router, "_actions");
                var foreignUi = foreignActions.UI.Get().enabled;
                try
                {
                    SetField(original.Router, "_actions", foreignActions);
                    Assert.That(() => original.Adapter.GetAuthenticatedLaunchRootForNewGame(original.Router), Throws.InvalidOperationException);
                    Assert.That(foreignActions.UI.Get().enabled, Is.EqualTo(foreignUi), "foreign UI map remains unchanged");
                }
                finally { SetField(original.Router, "_actions", originalActions); }
            }
        }

        [Test]
        public void AC001_PredecessorBodiesReconstructToAcceptedR36Fingerprints()
        {
            const string service = "2E1DCE184495D4AABAC135AFC6910121A94D69498A85669A7D7EA1BD5E554098";
            const string disk = "168D1539EAF359C3242ECABADAC0776956370E52105007D9224566F7F9C3FDC4";
            var root = ProjectRoot();
            var s = File.ReadAllText(Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileNewGameResetServiceV1.cs"));
            s = s.Replace("using System.Runtime.CompilerServices;\n", string.Empty);
            s = System.Text.RegularExpressions.Regex.Replace(s, "(?s)    // REQ-M5D7QC3L-002:.*?(?=    internal sealed class ProfileRootOperationLockV1)", string.Empty);
            s = System.Text.RegularExpressions.Regex.Replace(s, "(?s)        private sealed class ObservationContentionTimeoutException.*?(?=        private ProfileRootOperationLockV1)", string.Empty);
            s = System.Text.RegularExpressions.Regex.Replace(s, "(?s)        // REQ-M5D7QC3L-001\\.\\.003:.*?(?=        // A held-root operation)", string.Empty);
            var d = File.ReadAllText(Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileResetDiskTransactionV1.cs"));
            d = System.Text.RegularExpressions.Regex.Replace(d, "(?s)    // REQ-M5D7QC3-002/004 and REQ-M5D7QC3L-003:.*?(?=    internal sealed class Leaf)", string.Empty);
            d = System.Text.RegularExpressions.Regex.Replace(d, "(?s)        private sealed class ConfirmationCaptureWitness.*?(?=        private static readonly string\\[\\] Archives)", string.Empty);
            d = System.Text.RegularExpressions.Regex.Replace(d, "(?s)        // C3L observation-only counterpart\\..*?(?=        // REQ-M5D7QC1-001\\.\\.006)", string.Empty);
            Assert.That(HashUtf8(s), Is.EqualTo(service)); Assert.That(HashUtf8(d), Is.EqualTo(disk));
        }

        private static string ProjectRoot() { return Directory.GetParent(Application.dataPath).FullName; }
        private static string HashUtf8(string text) { using (var sha = System.Security.Cryptography.SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text))).Replace("-", string.Empty); }
        private static void SetHResult(Exception value, int hresult) { typeof(Exception).GetField("_HResult", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(value, hresult); }
        private static void Invoke(object target, string name) { target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null); }
        private static T Field<T>(object target, string name) { return (T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target); }
        private static void SetField(object target, string name, object value) { target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value); }
        private sealed class Fixture : IDisposable
        {
            internal readonly string Root; internal readonly GameObject Host; internal readonly InputRouter Router; internal readonly DesktopProfileLaunchAdapterV1 Adapter;
            private Fixture(string root, GameObject host, InputRouter router, DesktopProfileLaunchAdapterV1 adapter) { Root = root; Host = host; Router = router; Adapter = adapter; }
            internal static Fixture Create()
            {
                var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C3L-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
                var host = new GameObject("c3l-observation"); host.SetActive(false);
                var routerObject = new GameObject("router"); routerObject.transform.SetParent(host.transform);
                var adapterObject = new GameObject("adapter"); adapterObject.transform.SetParent(host.transform);
                var router = routerObject.AddComponent<InputRouter>(); router.ConfigureHubUiOnlyForAuthoring();
                var adapter = adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>(); adapter.ConfigureForTests(router, new EnvironmentPort(root), new DesktopProfileLaunchPreparationPortV1());
                host.SetActive(true);
                if (!router.PreparedLaunchAwakeEntered) Invoke(router, "Awake");
                var awakeAttempted = (bool)typeof(DesktopProfileLaunchAdapterV1).GetField("_awakeAttempted", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(adapter);
                if (!awakeAttempted) Invoke(adapter, "Awake");
                Invoke(adapter, "Start");
                Assert.That(adapter.CurrentReceipt.HasValue, Is.True, "actual launch receipt");
                return new Fixture(root, host, router, adapter);
            }
            public void Dispose() { UnityEngine.Object.DestroyImmediate(Host); DeleteOwnedTemporaryRoot(Root); }
        }
        private static void DeleteOwnedTemporaryRoot(string candidate) { var temp=Path.GetFullPath(Path.GetTempPath()); var root=Path.GetFullPath(candidate); Assert.That(root.StartsWith(temp,StringComparison.OrdinalIgnoreCase) && Path.GetFileName(root).StartsWith("AcadeGameMaker-C3L-",StringComparison.Ordinal) && !File.GetAttributes(root).HasFlag(FileAttributes.ReparsePoint), Is.True, "owned non-reparse temporary root"); if (File.Exists(root)) File.Delete(root); else if (Directory.Exists(root)) Directory.Delete(root, true); }
        private sealed class EnvironmentPort : IDesktopProfileLaunchEnvironmentV1
        {
            private readonly string _root; internal EnvironmentPort(string root) { _root = root; }
            public string GetPersistentDataPath() { return _root; } public DateTimeOffset GetUtcNow() { return DateTimeOffset.UtcNow; }
        }
    }
}
