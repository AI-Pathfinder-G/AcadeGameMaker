using System;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D7H-001..007; AC-M5D7H-001..010. Internal recovery entry exercised without changing M5D7D public ABI.
    public sealed class ProfileInputRecoveryAtomicSaveServiceV1Tests
    {
        [Test]
        public void AC001_MetadataRecoveryReplacesPrimaryAndPreservesExactPrevious()
        {
            WithDirectory(root =>
            {
                var source = Document(4, "obsolete", string.Empty); File.WriteAllBytes(Path.Combine(root, "profile.json"), source); var result = Invoke(root, Candidate(source));
                Assert.That(Get(result, "Outcome").ToString(), Is.EqualTo("CommittedRecoveryReplacement")); Assert.That((long)Get(result, "CommittedRevision"), Is.EqualTo(5));
                var primary = File.ReadAllBytes(Path.Combine(root, "profile.json")); var previous = File.ReadAllBytes(Path.Combine(root, "profile.prev.json")); CollectionAssert.AreEqual(source, previous); Assert.That(ProfileCanonicalDecoderV1.Decode(primary).Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.ValidCurrentInput)); Assert.That(File.Exists(Path.Combine(root, "profile.tmp.json")), Is.False);
            });
        }
        [Test]
        public void AC002_BindingRecoveryUsesRepairWithoutUnavailableCompatibilityGetter()
        {
            WithDirectory(root =>
            {
                var source = Binding(Document(7, "current", string.Empty)); File.WriteAllBytes(Path.Combine(root, "profile.json"), source); var result = Invoke(root, Candidate(source));
                Assert.That(Get(result, "Outcome").ToString(), Is.EqualTo("CommittedRecoveryReplacement")); Assert.That((long)Get(result, "CommittedRevision"), Is.EqualTo(8)); CollectionAssert.AreEqual(source, File.ReadAllBytes(Path.Combine(root, "profile.prev.json")));
            });
        }
        [Test]
        public void AC003_AC004_InvalidProofAndChangedPrimaryFailBeforeReplacement()
        {
            WithDirectory(root =>
            {
                var source = Document(1, "obsolete", string.Empty); File.WriteAllBytes(Path.Combine(root, "profile.json"), source);
                Assert.Throws<ArgumentException>(() => Invoke(root, ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary))); Assert.Throws<ArgumentException>(() => Invoke(root, ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Previous, ProfileCanonicalDecoderV1.Decode(source))));
                var current = Document(1, "current", string.Empty); File.WriteAllBytes(Path.Combine(root, "profile.json"), current); var result = Invoke(root, Candidate(source)); Assert.That(Get(result, "FailureStage").ToString(), Is.EqualTo("ExistingPrimaryValidation")); CollectionAssert.AreEqual(current, File.ReadAllBytes(Path.Combine(root, "profile.json"))); Assert.That(File.Exists(Path.Combine(root, "profile.prev.json")), Is.False);
            });
        }
        [TestCase("missing")]
        [TestCase("current")]
        [TestCase("invalid")]
        [TestCase("unsupported")]
        [TestCase("classification")]
        [TestCase("revision")]
        [TestCase("overflow")]
        public void AC004_FreshPrimaryMismatchTableNeverReplacesOrMoves(string change)
        {
            WithDirectory(root =>
            {
                var observed = Document(3, "obsolete", string.Empty); File.WriteAllBytes(Path.Combine(root, "profile.json"), observed);
                if (change == "missing") File.Delete(Path.Combine(root, "profile.json"));
                else if (change == "current") File.WriteAllBytes(Path.Combine(root, "profile.json"), Document(3, "current", string.Empty));
                else if (change == "invalid") File.WriteAllText(Path.Combine(root, "profile.json"), "{}");
                else if (change == "unsupported") File.WriteAllText(Path.Combine(root, "profile.json"), "{\"schemaVersion\":0}");
                else if (change == "classification") File.WriteAllBytes(Path.Combine(root, "profile.json"), Binding(Document(3, "current", string.Empty)));
                else if (change == "revision") File.WriteAllBytes(Path.Combine(root, "profile.json"), Document(4, "obsolete", string.Empty));
                else File.WriteAllBytes(Path.Combine(root, "profile.json"), Document(long.MaxValue, "obsolete", string.Empty));
                var port = NewPort(null); var result = InvokePort(root, Candidate(observed), port); Assert.That(Get(result, "FailureStage").ToString(), Is.EqualTo("ExistingPrimaryValidation")); Assert.That(port.Calls, Does.Not.Contain("replace")); Assert.That(port.Calls, Does.Not.Contain("move"));
            });
        }
        [Test]
        public void AC009_ResultAndSourceBoundariesFailClosed()
        {
            WithDirectory(root =>
            {
                var source = Document(2, "obsolete", string.Empty); File.WriteAllBytes(Path.Combine(root, "profile.json"), source); var result = Invoke(root, Candidate(source));
                var type = result.GetType(); var validate = type.GetMethod("Validate", BindingFlags.Instance | BindingFlags.NonPublic); Assert.DoesNotThrow(() => validate.Invoke(result, null));
                object boxed = result; type.GetField("_revision", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, -1L); Assert.Throws<TargetInvocationException>(() => validate.Invoke(boxed, null));
                boxed = result; type.GetField("_revision", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, 999L); Assert.Throws<TargetInvocationException>(() => validate.Invoke(boxed, null)); Assert.Throws<TargetInvocationException>(() => type.GetProperty("CommittedRevision", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(boxed, null));
                AssertInvalidResultField(result, "_outcome", Enum.ToObject(type.GetField("_outcome", BindingFlags.Instance | BindingFlags.NonPublic).FieldType, 99)); AssertInvalidResultField(result, "_stage", (ProfileAtomicSaveStage)99); AssertInvalidResultField(result, "_primary", ProfileStoredFileState.NotProbed); AssertInvalidResultField(result, "_previous", ProfileStoredFileState.Missing); AssertInvalidResultField(result, "_temp", ProfileStoredFileState.ValidCurrentInput);
            });
        }
        [TestCase("ensure", "DirectoryPreparation")]
        [TestCase("write", "TempWriteFlushClose")]
        [TestCase("flush", "TempWriteFlushClose")]
        [TestCase("close", "TempWriteFlushClose")]
        [TestCase("temp-read", "TempReadValidation")]
        [TestCase("source-read", "ExistingPrimaryValidation")]
        public void AC005_RecoveryPrecommitFaultsNeverReplaceOrMove(string fault, string stage)
        {
            WithDirectory(root =>
            {
                var source = Document(3, "obsolete", string.Empty); File.WriteAllBytes(Path.Combine(root, "profile.json"), source); var port = NewPort(fault); var result = InvokePort(root, Candidate(source), port);
                Assert.That(Get(result, "Outcome").ToString(), Is.EqualTo("FailedBeforeCommit")); Assert.That(Get(result, "FailureStage").ToString(), Is.EqualTo(stage)); Assert.That(port.Calls, Does.Not.Contain("replace")); Assert.That(port.Calls, Does.Not.Contain("move"));
            });
        }
        [TestCase("replace", "CommitCall")]
        [TestCase("post-primary", "PostCommitValidation")]
        [TestCase("post-previous", "PostCommitValidation")]
        [TestCase("post-temp", "PostCommitValidation")]
        public void AC006_RecoveryCommitAndPostFaultsAreUncertainWithoutFallback(string fault, string stage)
        {
            WithDirectory(root =>
            {
                var source = Document(3, "obsolete", string.Empty); File.WriteAllBytes(Path.Combine(root, "profile.json"), source); var port = NewPort(fault); var result = InvokePort(root, Candidate(source), port);
                Assert.That(Get(result, "Outcome").ToString(), Is.EqualTo("CommitOutcomeUncertain")); Assert.That(Get(result, "FailureStage").ToString(), Is.EqualTo(stage)); Assert.That(port.Calls.FindAll(value => value == "replace").Count, Is.EqualTo(1)); Assert.That(port.Calls, Does.Not.Contain("move")); Assert.That(Get(result, "PrimaryState").ToString(), Is.Not.EqualTo("NotProbed"));
            });
        }
        [Test]
        public void AC010_StaticBoundaryRetainsOrdinarySaveAndOneSharedPortRegistry()
        {
            var source = File.ReadAllText(Path.Combine(FindRoot(), "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileAtomicSaveServiceV1.cs"));
            foreach (var required in new[] { "SaveInputRecovery", "File.Replace(tempPath, primaryPath, previousPath, true)", "private static readonly Dictionary<string, LeaseEntry> Leases", "ProfileInputRecoverySaveOutcomeV1" }) StringAssert.Contains(required, source);
            foreach (var forbidden in new[] { "SaveInputRecoveryPublic", "File.Delete", "File.Copy", "Application.persistentDataPath", "UnityEngine", "DateTime.Now", "Random" }) StringAssert.DoesNotContain(forbidden, source);
        }
        [Test]
        public void AC008_OrdinaryAndRecoveryShareAliasLeaseWithRegisteredWaiters()
        {
            var left = CreateDirectory(); var right = CreateDirectory();
            try
            {
                var source = Document(4, "obsolete", string.Empty); File.WriteAllBytes(Path.Combine(left, "profile.json"), source); var started = new ManualResetEventSlim(false); var release = new ManualResetEventSlim(false); var port = NewPort("block-flush"); port.Started = started; port.Release = release;
                var ordinaryDocument = CurrentDocument(9);
                var otherDocument = CurrentDocument(1);
                var ordinary = Task.Run(() => InvokeOrdinaryPort(left, ordinaryDocument, port));
                Task<object> recovery = null;
                try
                {
                    Assert.That(started.Wait(TimeSpan.FromSeconds(5)), Is.True); recovery = Task.Run(() => InvokePort(left.ToUpperInvariant() + Path.DirectorySeparatorChar, Candidate(source), NewPort(null))); Assert.That(SpinWait.SpinUntil(() => LeaseReferences() == 2, TimeSpan.FromSeconds(5)), Is.True);
                    var other = Task.Run(() => ProfileAtomicSaveServiceV1.Save(right, otherDocument)); Assert.That(other.Wait(TimeSpan.FromSeconds(5)), Is.True);
                }
                finally { release.Set(); }
                Assert.That(ordinary.Wait(TimeSpan.FromSeconds(5)), Is.True); Assert.That(recovery.Wait(TimeSpan.FromSeconds(5)), Is.True); Assert.That(RegistryCount(), Is.Zero);
            }
            finally { DeleteDirectory(left); DeleteDirectory(right); }
        }
        [Test]
        public void AC009_InvalidPresenceUnexpectedAndFatalEscapeAndReleaseSharedLease()
        {
            WithDirectory(root =>
            {
                var source = Document(2, "obsolete", string.Empty); File.WriteAllBytes(Path.Combine(root, "profile.json"), source);
                foreach (var fault in new[] { "invalid-presence", "unexpected", "fatal" }) { if (!File.Exists(Path.Combine(root, "profile.json"))) File.WriteAllBytes(Path.Combine(root, "profile.json"), source); Assert.That(() => InvokePort(root, Candidate(source), NewPort(fault)), fault == "fatal" ? Throws.TypeOf<OutOfMemoryException>() : Throws.TypeOf<InvalidOperationException>()); Assert.That(RegistryCount(), Is.Zero); }
                Assert.That(Get(Invoke(root, Candidate(source)), "Outcome").ToString(), Is.EqualTo("CommittedRecoveryReplacement")); Assert.That(RegistryCount(), Is.Zero);
            });
        }
        [Test]
        public void AC007_SuccessPortOrderUsesOneReplaceAndPreservesCapturedPrimary()
        {
            WithDirectory(root =>
            {
                var source = Document(5, "obsolete", string.Empty); File.WriteAllBytes(Path.Combine(root, "profile.json"), source); var port = NewPort(null); var result = InvokePort(root, Candidate(source), port);
                Assert.That(Get(result, "Outcome").ToString(), Is.EqualTo("CommittedRecoveryReplacement")); Assert.That(port.Calls.FindAll(value => value == "replace").Count, Is.EqualTo(1)); Assert.That(port.Calls, Does.Not.Contain("move"));
                AssertOrdered(port.Calls, "ensure", "open", "write", "flush", "close", "read:profile.tmp.json", "probe:profile.json", "read:profile.json", "replace", "read:profile.json", "read:profile.prev.json", "probe:profile.json", "probe:profile.prev.json", "probe:profile.tmp.json"); CollectionAssert.AreEqual(source, File.ReadAllBytes(Path.Combine(root, "profile.prev.json")));
            });
        }

        private static ProfileLoadCandidateV1 Candidate(byte[] bytes) => ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Primary, ProfileCanonicalDecoderV1.Decode(bytes));
        private static FilePortProxy NewPort(string fault) { return new FilePortProxy { Fault = fault }; }
        private static object Invoke(string root, ProfileLoadCandidateV1 candidate)
        {
            MethodInfo method = null; foreach (var item in typeof(ProfileAtomicSaveServiceV1).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)) if (item.Name == "SaveInputRecovery" && item.GetParameters().Length == 2) { method = item; break; }
            try { return method.Invoke(null, new object[] { root, candidate }); } catch (TargetInvocationException exception) when (exception.InnerException != null) { ExceptionDispatchInfo.Capture(exception.InnerException).Throw(); throw; }
        }
        private static object InvokePort(string root, ProfileLoadCandidateV1 candidate, FilePortProxy source)
        {
            MethodInfo method = null; foreach (var item in typeof(ProfileAtomicSaveServiceV1).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)) if (item.Name == "SaveInputRecovery" && item.GetParameters().Length == 3) { method = item; break; }
            var proxy = (FilePortProxy)CreateProxy(method.GetParameters()[2].ParameterType, typeof(FilePortProxy)); proxy.CopyFrom(source);
            try { return method.Invoke(null, new object[] { root, candidate, proxy }); } catch (TargetInvocationException exception) when (exception.InnerException != null) { ExceptionDispatchInfo.Capture(exception.InnerException).Throw(); throw; }
        }
        private static object InvokeOrdinaryPort(string root, ProfileCanonicalDocumentV1 document, FilePortProxy source) { MethodInfo method = null; foreach (var item in typeof(ProfileAtomicSaveServiceV1).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)) if (item.Name == "Save" && item.GetParameters().Length == 3) { method = item; break; } var proxy = (FilePortProxy)CreateProxy(method.GetParameters()[2].ParameterType, typeof(FilePortProxy)); proxy.CopyFrom(source); try { return method.Invoke(null, new object[] { root, document, proxy }); } catch (TargetInvocationException exception) when (exception.InnerException != null) { ExceptionDispatchInfo.Capture(exception.InnerException).Throw(); throw; } }
        private static object CreateProxy(Type contract, Type proxy) { MethodInfo create = null; foreach (var item in typeof(System.Reflection.DispatchProxy).GetMethods(BindingFlags.Static | BindingFlags.Public)) if (item.Name == "Create" && item.IsGenericMethodDefinition && item.GetParameters().Length == 0) { create = item; break; } return create.MakeGenericMethod(contract, proxy).Invoke(null, null); }
        private static object Get(object result, string property) => result.GetType().GetProperty(property, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(result, null);
        private static void AssertInvalidResultField(object source, string field, object replacement) { var type = source.GetType(); object boxed = source; type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, replacement); Assert.Throws<TargetInvocationException>(() => type.GetMethod("Validate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(boxed, null)); }
        private static void AssertOrdered(IList<string> calls, params string[] expected) { var cursor = 0; foreach (var item in expected) { while (cursor < calls.Count && calls[cursor] != item) cursor++; Assert.That(cursor, Is.LessThan(calls.Count), "Missing ordered call " + item); cursor++; } }
        private static byte[] Document(long revision, string asset, string binding)
        {
            var input = new ProfileInputSnapshot(asset == "current" ? ProfileInputContract.CurrentInputActionsAssetId : "obsolete", 1, ProfileBindingOverridesJson.Parse(binding)); var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), input, new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>())); return ProfileCanonicalEncoder.Encode(snapshot).CanonicalFileBytes;
        }
        private static ProfileCanonicalDocumentV1 CurrentDocument(long revision)
        {
            var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, 1, ProfileBindingOverridesJson.Parse(string.Empty)), new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            return ProfileCanonicalEncoder.Encode(snapshot);
        }
        private static byte[] Binding(byte[] source) { var document = ProfileCanonicalDecoderV1.Decode(source).Document; var payload = Encoding.UTF8.GetString(document.CanonicalPayloadBytes).Replace("\"bindingOverridesJson\":\"\"", "\"bindingOverridesJson\":\"{bad}\""); var hash = Hash(Utf8(payload)); return Utf8(payload.Substring(0, payload.Length - 1) + ",\"integrity\":{\"payloadSha256\":\"" + hash + "\"}}"); }
        private static byte[] Utf8(string text) => new UTF8Encoding(false).GetBytes(text);
        private static string Hash(byte[] bytes) { using (var sha = SHA256.Create()) { var digest = sha.ComputeHash(bytes); var text = new StringBuilder(); foreach (var item in digest) text.Append(item.ToString("x2")); return text.ToString(); } }
        private static void WithDirectory(Action<string> action) { var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-M5D7H-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root); try { action(root); } finally { if (Directory.Exists(root)) Directory.Delete(root, true); } }
        private static string CreateDirectory() { var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-M5D7H-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root); return root; }
        private static void DeleteDirectory(string root) { if (Directory.Exists(root)) Directory.Delete(root, true); }
        private static int RegistryCount() => ((System.Collections.IDictionary)typeof(ProfileAtomicSaveServiceV1).GetField("Leases", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).Count;
        private static int LeaseReferences() { var values = (System.Collections.IDictionary)typeof(ProfileAtomicSaveServiceV1).GetField("Leases", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null); foreach (System.Collections.DictionaryEntry item in values) return (int)item.Value.GetType().GetField("RefCount", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(item.Value); return 0; }
        private static string FindRoot() { var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (File.Exists(Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new DirectoryNotFoundException(); }

        public class FilePortProxy : System.Reflection.DispatchProxy
        {
            internal string Fault; internal List<string> Calls = new List<string>(); internal int ReplaceCalls; internal int MoveCalls; internal ManualResetEventSlim Started; internal ManualResetEventSlim Release;
            internal void CopyFrom(FilePortProxy value) { Fault = value.Fault; Calls = value.Calls; ReplaceCalls = value.ReplaceCalls; MoveCalls = value.MoveCalls; Started = value.Started; Release = value.Release; }
            protected override object Invoke(MethodInfo method, object[] args)
            {
                if (method.Name == "EnsureDirectory") { Calls.Add("ensure"); if (Fault == "ensure") throw new IOException(); Directory.CreateDirectory((string)args[0]); return null; }
                if (method.Name == "OpenTempExclusive") { Calls.Add("open"); if (Fault == "open") throw new IOException(); var writer = (WriterProxy)CreateProxy(method.ReturnType, typeof(WriterProxy)); writer.Initialize((string)args[0], Fault, Calls, Started, Release); return writer; }
                if (method.Name == "ProbePresence") { var path = (string)args[0]; Calls.Add("probe:" + Path.GetFileName(path)); if ((Fault == "invalid-presence" || Fault == "unexpected") && path.EndsWith("profile.json", StringComparison.Ordinal)) { if (Fault == "invalid-presence") return Enum.ToObject(method.ReturnType, 99); throw new InvalidOperationException(); } if (Fault == "fatal" && path.EndsWith("profile.json", StringComparison.Ordinal)) throw new OutOfMemoryException(); try { File.GetAttributes(path); return Enum.ToObject(method.ReturnType, 2); } catch (FileNotFoundException) { return Enum.ToObject(method.ReturnType, 1); } catch (DirectoryNotFoundException) { return Enum.ToObject(method.ReturnType, 1); } }
                if (method.Name == "ReadAllExclusive") { var path = (string)args[0]; Calls.Add("read:" + Path.GetFileName(path)); if (Fault == "temp-read" && path.EndsWith("profile.tmp.json", StringComparison.Ordinal)) throw new IOException(); if (Fault == "source-read" && path.EndsWith("profile.json", StringComparison.Ordinal)) throw new IOException(); return File.ReadAllBytes(path); }
                if (method.Name == "Replace") { ReplaceCalls++; Calls.Add("replace"); if (Fault == "replace") throw new IOException(); File.Replace((string)args[0], (string)args[1], (string)args[2], true); if (Fault == "post-primary") File.WriteAllText((string)args[1], "bad"); if (Fault == "post-previous") File.WriteAllText((string)args[2], "bad"); if (Fault == "post-temp") File.WriteAllText((string)args[0], "bad"); return null; }
                if (method.Name == "Move") { MoveCalls++; throw new InvalidOperationException("Recovery must not move."); }
                throw new MissingMethodException(method.Name);
            }
        }
        public class WriterProxy : System.Reflection.DispatchProxy
        {
            private FileStream _stream; private string _fault; private List<string> _calls; private ManualResetEventSlim _started; private ManualResetEventSlim _release;
            internal void Initialize(string path, string fault, List<string> calls, ManualResetEventSlim started, ManualResetEventSlim release) { _stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None); _fault = fault; _calls = calls; _started = started; _release = release; }
            protected override object Invoke(MethodInfo method, object[] args) { if (method.Name == "WriteAll") { _calls.Add("write"); if (_fault == "write") throw new IOException(); var bytes = (byte[])args[0]; _stream.Write(bytes, 0, bytes.Length); return null; } if (method.Name == "FlushToStorage") { _calls.Add("flush"); if (_fault == "block-flush") { _started.Set(); _release.Wait(TimeSpan.FromSeconds(10)); } if (_fault == "flush") throw new IOException(); _stream.Flush(true); return null; } if (method.Name == "Close") { _calls.Add("close"); _stream.Dispose(); if (_fault == "close") throw new IOException(); return null; } throw new MissingMethodException(method.Name); }
        }
    }
}
