using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D7D-001..006; AC-M5D7D-001..010. Real isolated directories plus the internal deterministic fault seam.
    public sealed class ProfileAtomicSaveServiceV1Tests
    {
        [Test]
        public void AC001_FirstSaveWritesExactBytesAndLeavesNoTemp()
        {
            WithDirectory(directory =>
            {
                var document = Document(1); var result = ProfileAtomicSaveServiceV1.Save(directory, document);
                Assert.That(result.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.CommittedFirst));
                Assert.That(result.FailureStage, Is.EqualTo(ProfileAtomicSaveStage.None));
                Assert.That(result.CommittedRevision, Is.EqualTo(1));
                CollectionAssert.AreEqual(document.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(directory, "profile.json")));
                Assert.That(File.Exists(Path.Combine(directory, "profile.tmp.json")), Is.False);
                Assert.That(result.PreviousState, Is.EqualTo(ProfileStoredFileState.Missing));
            });
        }

        [Test]
        public void AC002_ReplacementPreservesExactOldPrimary()
        {
            WithDirectory(directory =>
            {
                var first = Document(1); var second = Document(2);
                ProfileAtomicSaveServiceV1.Save(directory, first);
                var result = ProfileAtomicSaveServiceV1.Save(directory, second);
                Assert.That(result.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.CommittedReplacement));
                CollectionAssert.AreEqual(second.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(directory, "profile.json")));
                CollectionAssert.AreEqual(first.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(directory, "profile.prev.json")));
                Assert.That(File.Exists(Path.Combine(directory, "profile.tmp.json")), Is.False);
                AssertExactLeafSet(directory, "profile.json", "profile.prev.json");
            });
        }

        [TestCase("ensure", ProfileAtomicSaveStage.DirectoryPreparation)]
        [TestCase("write", ProfileAtomicSaveStage.TempWriteFlushClose)]
        [TestCase("flush", ProfileAtomicSaveStage.TempWriteFlushClose)]
        [TestCase("close", ProfileAtomicSaveStage.TempWriteFlushClose)]
        [TestCase("temp-read", ProfileAtomicSaveStage.TempReadValidation)]
        [TestCase("temp-corrupt", ProfileAtomicSaveStage.TempReadValidation)]
        public void AC003_InjectedPrecommitFailuresNeverCommit(string fault, ProfileAtomicSaveStage stage)
        {
            WithDirectory(directory =>
            {
                var original = Document(1); ProfileAtomicSaveServiceV1.Save(directory, original);
                var port = NewPort(fault); var result = InvokeInternal(directory, Document(2), port);
                Assert.That(result.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.FailedBeforeCommit));
                Assert.That(result.FailureStage, Is.EqualTo(stage));
                CollectionAssert.AreEqual(original.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(directory, "profile.json")));
                Assert.That(port.Calls.Contains("replace") || port.Calls.Contains("move"), Is.False);
            });
        }

        [TestCase("replace", ProfileAtomicSaveStage.CommitCall)]
        [TestCase("move", ProfileAtomicSaveStage.CommitCall)]
        [TestCase("post-corrupt", ProfileAtomicSaveStage.PostCommitValidation)]
        [TestCase("post-missing", ProfileAtomicSaveStage.PostCommitValidation)]
        [TestCase("post-old-primary", ProfileAtomicSaveStage.PostCommitValidation)]
        [TestCase("post-temp-remains", ProfileAtomicSaveStage.PostCommitValidation)]
        [TestCase("post-previous-mismatch", ProfileAtomicSaveStage.PostCommitValidation)]
        public void AC004_CommitAndPostCommitFaultsAreUncertainWithoutFallback(string fault, ProfileAtomicSaveStage stage)
        {
            WithDirectory(directory =>
            {
                if (fault != "move") ProfileAtomicSaveServiceV1.Save(directory, Document(1));
                var port = NewPort(fault); var result = InvokeInternal(directory, Document(2), port);
                Assert.That(result.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.CommitOutcomeUncertain));
                Assert.That(result.FailureStage, Is.EqualTo(stage));
                Assert.That(result.CommittedRevision, Is.EqualTo(-1));
                Assert.That(result.PrimaryState, Is.Not.EqualTo(ProfileStoredFileState.NotProbed));
                Assert.That(result.PreviousState, Is.Not.EqualTo(ProfileStoredFileState.NotProbed));
                Assert.That(result.TempState, Is.Not.EqualTo(ProfileStoredFileState.NotProbed));
                Assert.That(port.Calls.FindAll(x => x == "replace" || x == "move").Count, Is.EqualTo(1));
            });
        }

        [Test]
        public void AC005_InvalidExistingPrimaryAndStaleTempStopBeforeReplace()
        {
            WithDirectory(directory =>
            {
                File.WriteAllText(Path.Combine(directory, "profile.json"), "not a profile");
                File.WriteAllText(Path.Combine(directory, "profile.tmp.json"), "stale");
                var port = NewPort(null); var result = InvokeInternal(directory, Document(2), port);
                Assert.That(result.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.FailedBeforeCommit));
                Assert.That(result.FailureStage, Is.EqualTo(ProfileAtomicSaveStage.ExistingPrimaryValidation));
                Assert.That(port.Calls.Contains("replace"), Is.False);
                Assert.That(File.ReadAllText(Path.Combine(directory, "profile.json")), Is.EqualTo("not a profile"));
            });
        }

        [TestCase("unsupported")]
        [TestCase("input-recovery")]
        [TestCase("unreadable")]
        public void AC005_EachNonCurrentExistingPrimaryBlocksCommit(string kind)
        {
            WithDirectory(directory =>
            {
                var primary = Path.Combine(directory, "profile.json");
                if (kind == "unsupported") File.WriteAllText(primary, "{\"schemaVersion\":0}");
                else if (kind == "input-recovery") File.WriteAllBytes(primary, IncompatibleDocument(1).CanonicalFileBytes);
                else File.WriteAllBytes(primary, Document(1).CanonicalFileBytes);
                var port = NewPort(kind == "unreadable" ? "primary-read" : null); var result = InvokeInternal(directory, Document(2), port);
                Assert.That(result.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.FailedBeforeCommit));
                Assert.That(result.FailureStage, Is.EqualTo(ProfileAtomicSaveStage.ExistingPrimaryValidation));
                Assert.That(port.Calls.Contains("replace"), Is.False);
                Assert.That(port.Calls.Contains("move"), Is.False);
            });
        }

        [Test]
        public void AC005_PresenceAccessErrorIsUnreadableAndNeverCommits()
        {
            WithDirectory(directory =>
            {
                File.WriteAllBytes(Path.Combine(directory, "profile.json"), Document(1).CanonicalFileBytes);
                var port = NewPort("presence-error"); var result = InvokeInternal(directory, Document(2), port);
                Assert.That(result.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.FailedBeforeCommit));
                Assert.That(result.FailureStage, Is.EqualTo(ProfileAtomicSaveStage.ExistingPrimaryValidation));
                Assert.That(result.PrimaryState, Is.EqualTo(ProfileStoredFileState.Unreadable));
                Assert.That(port.Calls.Contains("replace"), Is.False);
                Assert.That(port.Calls.Contains("move"), Is.False);
            });
        }

        [TestCase("unexpected-open", false)]
        [TestCase("unexpected-presence", true)]
        public void AC003_AC005_UnexpectedFaultEscapesAndReleasesLease(string fault, bool createPrimary)
        {
            WithDirectory(directory =>
            {
                if (createPrimary) File.WriteAllBytes(Path.Combine(directory, "profile.json"), Document(1).CanonicalFileBytes);
                Assert.Throws<InvalidOperationException>(() => InvokeInternal(directory, Document(2), NewPort(fault)));
                Assert.That(RegistryCount(), Is.EqualTo(0));
            });
        }

        [Test]
        public void AC003_FatalFaultEscapesAndReleasesLease()
        {
            WithDirectory(directory =>
            {
                Assert.Throws<OutOfMemoryException>(() => InvokeInternal(directory, Document(2), NewPort("fatal-open")));
                Assert.That(RegistryCount(), Is.EqualTo(0));
            });
        }

        [Test]
        public void AC002_InjectedPortPreservesRequiredCommitPrefixOrder()
        {
            WithDirectory(directory =>
            {
                ProfileAtomicSaveServiceV1.Save(directory, Document(1));
                var port = NewPort(null); var result = InvokeInternal(directory, Document(2), port);
                Assert.That(result.Outcome, Is.EqualTo(ProfileAtomicSaveOutcome.CommittedReplacement));
                CollectionAssert.AreEqual(new[] { "ensure", "open", "write", "flush", "close", "read", "presence", "read", "replace" }, port.Calls.GetRange(0, 9));
            });
        }

        [Test]
        public void AC006_ArgumentsRejectBeforeAnyPortIo()
        {
            var port = NewPort(null);
            Assert.Throws<ArgumentException>(() => InvokeInternal("relative", Document(1), port));
            Assert.That(port.Calls.Count, Is.EqualTo(0));
            Assert.Throws<ArgumentException>(() => InvokeInternal(string.Empty, Document(1), port));
            Assert.Throws<ArgumentException>(() => InvokeInternal(null, Document(1), port));
            Assert.Throws<ArgumentException>(() => ProfileAtomicSaveServiceV1.Save(Path.GetPathRoot(Path.GetTempPath()), Document(1)));
            Assert.Throws<ArgumentException>(() => ProfileAtomicSaveServiceV1.Save(Path.GetTempPath(), default(ProfileCanonicalDocumentV1)));
            Assert.Throws<ArgumentException>(() => InvokeInternal(Path.GetTempPath(), IncompatibleDocument(1), port));
            Assert.That(port.Calls.Count, Is.EqualTo(0));
        }

        [Test]
        public void AC007_AliasesAndWaitersSerializeAndFinalLeaseIsRemoved()
        {
            WithDirectory(directory =>
            {
                var aliases = new[] { directory, directory + Path.DirectorySeparatorChar, directory.ToUpperInvariant() };
                var started = new ManualResetEventSlim(false); var release = new ManualResetEventSlim(false);
                var first = NewPort("block-flush"); first.BlockStarted = started; first.BlockRelease = release;
                var firstTask = Task.Run(() => InvokeInternal(aliases[0], Document(1), first));
                var waiters = new List<Task<ProfileAtomicSaveResultV1>>();
                try
                {
                    Assert.That(started.Wait(TimeSpan.FromSeconds(5)), Is.True);
                    for (var index = 2; index < 7; index++) { var revision = index; waiters.Add(Task.Run(() => ProfileAtomicSaveServiceV1.Save(aliases[revision % aliases.Length], Document(revision)))); }
                }
                finally { release.Set(); }
                Task.WaitAll(waiters.ToArray()); firstTask.Wait();
                var final = ProfileCanonicalDecoderV1.Decode(File.ReadAllBytes(Path.Combine(directory, "profile.json")));
                Assert.That(final.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidCurrentInput));
                Assert.That(RegistryCount(), Is.EqualTo(0));
            });
        }

        [Test]
        public void AC008_ResultInvariantsAndSourceByteOwnershipFailClosed()
        {
            WithDirectory(directory =>
            {
                var document = Document(3); var bytes = document.CanonicalFileBytes; bytes[0] = (byte)'!';
                ProfileAtomicSaveServiceV1.Save(directory, document);
                Assert.That(File.ReadAllBytes(Path.Combine(directory, "profile.json"))[0], Is.Not.EqualTo((byte)'!'));
                var result = ProfileAtomicSaveServiceV1.Save(directory, Document(4));
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_outcome", (ProfileAtomicSaveOutcome)99).Validate());
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_failureStage", ProfileAtomicSaveStage.CommitCall).Validate());
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_committedRevision", -1L).Validate());
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_primaryState", (ProfileStoredFileState)99).Validate());
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_previousState", ProfileStoredFileState.Missing).Validate());
                var failed = Corrupt(default(ProfileAtomicSaveResultV1), "_outcome", ProfileAtomicSaveOutcome.FailedBeforeCommit);
                failed = Corrupt(failed, "_failureStage", ProfileAtomicSaveStage.None);
                failed = Corrupt(failed, "_primaryState", ProfileStoredFileState.Missing);
                failed = Corrupt(failed, "_previousState", ProfileStoredFileState.Missing);
                failed = Corrupt(failed, "_tempState", ProfileStoredFileState.Missing);
                failed = Corrupt(failed, "_committedRevision", -1L);
                Assert.Throws<InvalidOperationException>(() => failed.Validate());
                Assert.Throws<InvalidOperationException>(() => default(ProfileAtomicSaveResultV1).Validate());
            });
        }

        [Test]
        public void AC009_ProductionSourceUsesRequiredBclTransactionWithoutFallback()
        {
            var source = File.ReadAllText(Path.Combine(FindRoot(), "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileAtomicSaveServiceV1.cs"));
            foreach (var required in new[] { "FileMode.Create", "FileShare.None", "FileOptions.WriteThrough", "Flush(true)", "File.Replace(tempPath, primaryPath, previousPath, true)", "File.Move(tempPath, primaryPath)" }) StringAssert.Contains(required, source);
            foreach (var leaf in new[] { "PrimaryName = \"profile.json\"", "PreviousName = \"profile.prev.json\"", "TempName = \"profile.tmp.json\"" }) StringAssert.Contains(leaf, source);
            StringAssert.DoesNotContain("profile.previous.json", source);
            foreach (var forbidden in new[] { "File.Delete", "Copy(", "persistentDataPath", "UnityEngine", "DateTime", "Random", "System.Net" }) StringAssert.DoesNotContain(forbidden, source);
        }

        [Test]
        public void AC010_DifferentDirectoriesProceedWithoutSharedDirectoryLock()
        {
            var left = CreateDirectory(); var right = CreateDirectory();
            try
            {
                var gate = new ManualResetEventSlim(false); var release = new ManualResetEventSlim(false); var port = NewPort("block-flush"); port.BlockStarted = gate; port.BlockRelease = release;
                var blocked = Task.Run(() => InvokeInternal(left, Document(1), port));
                try
                {
                    Assert.That(gate.Wait(TimeSpan.FromSeconds(5)), Is.True);
                    var other = Task.Run(() => ProfileAtomicSaveServiceV1.Save(right, Document(2)));
                    Assert.That(other.Wait(TimeSpan.FromSeconds(5)), Is.True);
                }
                finally { release.Set(); }
                blocked.Wait();
                Assert.That(RegistryCount(), Is.EqualTo(0));
            }
            finally { DeleteDirectory(left); DeleteDirectory(right); }
        }

        private static ProfileCanonicalDocumentV1 Document(long revision)
        {
            var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false),
                new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty)),
                new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            return ProfileCanonicalEncoder.Encode(snapshot);
        }
        private static ProfileCanonicalDocumentV1 IncompatibleDocument(long revision)
        {
            var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false),
                new ProfileInputSnapshot("obsolete-asset", 1, ProfileBindingOverridesJson.Parse(string.Empty)),
                new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            return ProfileCanonicalEncoder.Encode(snapshot);
        }
        private static void WithDirectory(Action<string> action) { var directory = CreateDirectory(); try { action(directory); } finally { DeleteDirectory(directory); } }
        private static string CreateDirectory() { var directory = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-M5D7D-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory); return directory; }
        private static void DeleteDirectory(string directory) { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
        private static void AssertExactLeafSet(string directory, params string[] expected)
        {
            // REQ-M5D7QC-007: the durable root lock is required metadata, not an
            // active profile candidate. This still proves no extra data leaf
            // (especially temp) survived the atomic replacement.
            var actual = Directory.GetFiles(directory);
            var names = new string[actual.Length];
            for (var index = 0; index < actual.Length; index++) names[index] = Path.GetFileName(actual[index]);
            var expectedWithLock = new string[expected.Length + 1];
            Array.Copy(expected, expectedWithLock, expected.Length);
            expectedWithLock[expected.Length] = "profile.operation.lock";
            CollectionAssert.AreEquivalent(expectedWithLock, names);
        }
        private static ProfileAtomicSaveResultV1 InvokeInternal(string directory, ProfileCanonicalDocumentV1 document, FilePortProxy port)
        {
            MethodInfo save = null; foreach (var candidate in typeof(ProfileAtomicSaveServiceV1).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)) if (candidate.Name == "Save" && candidate.GetParameters().Length == 3) { save = candidate; break; }
            var interfaceType = save.GetParameters()[2].ParameterType; var proxy = CreateProxy(interfaceType, typeof(FilePortProxy));
            ((FilePortProxy)proxy).CopyFrom(port);
            try { return (ProfileAtomicSaveResultV1)save.Invoke(null, new object[] { directory, document, proxy }); }
            catch (TargetInvocationException exception) when (exception.InnerException != null) { ExceptionDispatchInfo.Capture(exception.InnerException).Throw(); throw; }
        }
        private static FilePortProxy NewPort(string fault) { var value = new FilePortProxy(); value.Fault = fault; return value; }
        private static object CreateProxy(Type contract, Type proxy) { MethodInfo create = null; foreach (var candidate in typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)) if (candidate.Name == "Create" && candidate.IsGenericMethodDefinition && candidate.GetParameters().Length == 0) { create = candidate; break; } return create.MakeGenericMethod(contract, proxy).Invoke(null, null); }
        private static int RegistryCount() { return (int)((System.Collections.IDictionary)typeof(ProfileAtomicSaveServiceV1).GetField("Leases", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).Count; }
        private static ProfileAtomicSaveResultV1 Corrupt(ProfileAtomicSaveResultV1 source, string field, object value) { object boxed = source; typeof(ProfileAtomicSaveResultV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value); return (ProfileAtomicSaveResultV1)boxed; }
        private static string FindRoot() { var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (File.Exists(Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new DirectoryNotFoundException(); }

        public class FilePortProxy : DispatchProxy
        {
            internal string Fault; internal List<string> Calls = new List<string>(); internal ManualResetEventSlim BlockStarted; internal ManualResetEventSlim BlockRelease;
            internal void CopyFrom(FilePortProxy source) { Fault = source.Fault; Calls = source.Calls; BlockStarted = source.BlockStarted; BlockRelease = source.BlockRelease; }
            protected override object Invoke(MethodInfo method, object[] args)
            {
                var name = method.Name;
                if (name == "EnsureDirectory") { Calls.Add("ensure"); Throw("ensure"); Directory.CreateDirectory((string)args[0]); return null; }
                if (name == "OpenTempExclusive") { Calls.Add("open"); if (Fault == "unexpected-open") throw new InvalidOperationException("injected unexpected open"); if (Fault == "fatal-open") throw new OutOfMemoryException("injected fatal open"); var proxy = CreateProxy(method.ReturnType, typeof(TempWriterProxy)); var writer = (TempWriterProxy)proxy; writer.Initialize((string)args[0], Fault, Calls, BlockStarted, BlockRelease); return proxy; }
                if (name == "ProbePresence")
                {
                    var path = (string)args[0]; Calls.Add("presence");
                    if ((Fault == "presence-error" || Fault == "unexpected-presence") && path.EndsWith("profile.json", StringComparison.Ordinal))
                    {
                        if (Fault == "presence-error") throw new UnauthorizedAccessException("injected presence access error");
                        throw new InvalidOperationException("injected unexpected presence error");
                    }
                    try { File.GetAttributes(path); return Enum.ToObject(method.ReturnType, 2); }
                    catch (FileNotFoundException) { return Enum.ToObject(method.ReturnType, 1); }
                    catch (DirectoryNotFoundException) { return Enum.ToObject(method.ReturnType, 1); }
                }
                if (name == "ReadAllExclusive") { var path = (string)args[0]; Calls.Add("read"); if ((Fault == "temp-read" && path.EndsWith("profile.tmp.json", StringComparison.Ordinal)) || (Fault == "primary-read" && path.EndsWith("profile.json", StringComparison.Ordinal))) throw new IOException("injected read"); return File.ReadAllBytes(path); }
                if (name == "Replace") { Calls.Add("replace"); Throw("replace"); File.Replace((string)args[0], (string)args[1], (string)args[2], true); AfterCommit((string)args[1]); return null; }
                if (name == "Move") { Calls.Add("move"); Throw("move"); File.Move((string)args[0], (string)args[1]); AfterCommit((string)args[1]); return null; }
                throw new MissingMethodException(name);
            }
            private void Throw(string point) { if (Fault == point) throw new IOException("injected " + point); }
            private void AfterCommit(string primary)
            {
                if (Fault == "post-corrupt") File.WriteAllText(primary, "corrupt");
                else if (Fault == "post-missing") File.Delete(primary);
                else if (Fault == "post-old-primary") File.Copy(Path.Combine(Path.GetDirectoryName(primary), "profile.prev.json"), primary, true);
                else if (Fault == "post-temp-remains") File.WriteAllText(Path.Combine(Path.GetDirectoryName(primary), "profile.tmp.json"), "{}");
                else if (Fault == "post-previous-mismatch") File.WriteAllText(Path.Combine(Path.GetDirectoryName(primary), "profile.prev.json"), "{}");
            }
        }
        public class TempWriterProxy : DispatchProxy
        {
            private FileStream _stream; private string _path; private string _fault; private List<string> _calls; private ManualResetEventSlim _started; private ManualResetEventSlim _release;
            internal void Initialize(string path, string fault, List<string> calls, ManualResetEventSlim started, ManualResetEventSlim release) { _stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None); _path = path; _fault = fault; _calls = calls; _started = started; _release = release; }
            protected override object Invoke(MethodInfo method, object[] args)
            {
                if (method.Name == "WriteAll") { _calls.Add("write"); if (_fault == "write") throw new IOException("injected write"); var bytes = (byte[])args[0]; _stream.Write(bytes, 0, bytes.Length); return null; }
                if (method.Name == "FlushToStorage") { _calls.Add("flush"); if (_fault == "block-flush") { _started.Set(); _release.Wait(TimeSpan.FromSeconds(10)); } if (_fault == "flush") throw new IOException("injected flush"); _stream.Flush(true); return null; }
                if (method.Name == "Close") { _calls.Add("close"); _stream.Dispose(); if (_fault == "temp-corrupt") File.WriteAllText(_path, "corrupt"); if (_fault == "close") throw new IOException("injected close"); return null; }
                throw new MissingMethodException(method.Name);
            }
        }
    }
}
