using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D7G-001..006; AC-M5D7G-001..010. Isolated BCL fixtures and deterministic internal observation-port protocol tests.
    public sealed class ProfileLaunchObservationAdapterV1Tests
    {
        [Test]
        public void AC001_ThreeMissingLeavesProduceRoleExactMissingWithoutReads()
        {
            WithDirectory(root => { var port = NewPort(null); var batch = Invoke(root, port); Assert.That(batch.Primary.Kind, Is.EqualTo(ProfileLoadCandidateKind.Missing)); Assert.That(batch.Previous.Kind, Is.EqualTo(ProfileLoadCandidateKind.Missing)); Assert.That(batch.Temp.Kind, Is.EqualTo(ProfileLoadCandidateKind.Missing)); Assert.That(port.Reads, Is.Zero); CollectionAssert.AreEqual(new[] { "probe:profile.json", "probe:profile.prev.json", "probe:profile.tmp.json" }, port.Calls); });
        }

        [Test]
        public void AC002_AllFiveDecoderClassificationsArePreservedForEveryRole()
        {
            WithDirectory(root =>
            {
                var cases = new[] { Document(1, "current"), Document(2, "obsolete"), Binding(Document(3, "current")), Utf8("{\"schemaVersion\":0}"), Utf8("{}") };
                var expected = new[] { ProfileCanonicalDecodeClassification.ValidCurrentInput, ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired, ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired, ProfileCanonicalDecodeClassification.UnsupportedProfileSchema, ProfileCanonicalDecodeClassification.Invalid };
                var leaves = new[] { "profile.json", "profile.prev.json", "profile.tmp.json" };
                for (var classification = 0; classification < cases.Length; classification++) for (var role = 0; role < 3; role++)
                {
                    foreach (var leaf in leaves) if (File.Exists(Path.Combine(root, leaf))) File.Delete(Path.Combine(root, leaf));
                    File.WriteAllBytes(Path.Combine(root, leaves[role]), cases[classification]); var batch = ProfileLaunchObservationAdapterV1.Observe(root); var candidate = role == 0 ? batch.Primary : role == 1 ? batch.Previous : batch.Temp;
                    Assert.That(candidate.Kind, Is.EqualTo(ProfileLoadCandidateKind.Decoded)); Assert.That(candidate.DecodeResult.Classification, Is.EqualTo(expected[classification]));
                }
            });
        }

        [Test]
        public void AC003_RealMixedFixtureAndPortOrderRemainObservationOnly()
        {
            WithDirectory(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), Document(1, "current")); File.WriteAllBytes(Path.Combine(root, "profile.tmp.json"), Document(99, "current"));
                var port = NewPort(null); var batch = Invoke(root, port);
                Assert.That(batch.Primary.DecodeResult.Document.Snapshot.ProfileRevision, Is.EqualTo(1)); Assert.That(batch.Temp.DecodeResult.Document.Snapshot.ProfileRevision, Is.EqualTo(99)); Assert.That(batch.Previous.Kind, Is.EqualTo(ProfileLoadCandidateKind.Missing));
                CollectionAssert.AreEqual(new[] { "probe:profile.json", "open/read:profile.json", "close:profile.json", "return:profile.json", "probe:profile.prev.json", "probe:profile.tmp.json", "open/read:profile.tmp.json", "close:profile.tmp.json", "return:profile.tmp.json" }, port.Calls);
            });
        }

        [TestCase("profile.json", ProfileLoadFileRole.Primary)]
        [TestCase("profile.prev.json", ProfileLoadFileRole.Previous)]
        [TestCase("profile.tmp.json", ProfileLoadFileRole.Temp)]
        public void AC004_ProbeAndReadRecoverableFaultsArePerRole(string leaf, ProfileLoadFileRole role)
        {
            WithDirectory(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), Document(1, "current")); File.WriteAllBytes(Path.Combine(root, "profile.prev.json"), Document(2, "current")); File.WriteAllBytes(Path.Combine(root, "profile.tmp.json"), Document(3, "current"));
                foreach (var fault in new[] { "probe:" + leaf, "read:" + leaf, "disappear:" + leaf, "eof:" + leaf })
                {
                    var batch = Invoke(root, NewPort(fault)); var candidate = role == ProfileLoadFileRole.Primary ? batch.Primary : role == ProfileLoadFileRole.Previous ? batch.Previous : batch.Temp;
                    Assert.That(candidate.Kind, Is.EqualTo(ProfileLoadCandidateKind.Unreadable));
                    if (role != ProfileLoadFileRole.Primary) Assert.That(batch.Primary.Kind, Is.EqualTo(ProfileLoadCandidateKind.Decoded));
                    if (role != ProfileLoadFileRole.Previous) Assert.That(batch.Previous.Kind, Is.EqualTo(ProfileLoadCandidateKind.Decoded));
                    if (role != ProfileLoadFileRole.Temp) Assert.That(batch.Temp.Kind, Is.EqualTo(ProfileLoadCandidateKind.Decoded));
                }
            });
        }

        [Test]
        public void AC005_EmptyIsDecodedInvalidAndProtocolViolationsEscapeBeforeBatch()
        {
            WithDirectory(root => { File.WriteAllBytes(Path.Combine(root, "profile.json"), Array.Empty<byte>()); var batch = ProfileLaunchObservationAdapterV1.Observe(root); Assert.That(batch.Primary.DecodeResult.Classification, Is.EqualTo(ProfileCanonicalDecodeClassification.Invalid)); });
            WithDirectory(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), Utf8("{}"));
                foreach (var fault in new[] { "default", "not-closed", "null", "mismatch", "overflow", "negative", "unknown-completion", "short", "zero-progress" }) { var port = NewPort(fault); Assert.Throws<InvalidOperationException>(() => Invoke(root, port)); Assert.That(port.Calls, Does.Not.Contain("probe:profile.prev.json")); }
            });
        }

        [Test]
        public void AC006_ReturnedCandidatesDoNotExposeInjectedByteMutation()
        {
            WithDirectory(root => { var bytes = Document(7, "current"); File.WriteAllBytes(Path.Combine(root, "profile.json"), bytes); var port = NewPort(null); var batch = Invoke(root, port); port.State.LastBytes[0] ^= 1; Assert.That(batch.Primary.DecodeResult.Document.Snapshot.ProfileRevision, Is.EqualTo(7)); Assert.That(batch.Primary.DecodeResult.Document.Snapshot.ProfileRevision, Is.EqualTo(7)); });
        }

        [Test]
        public void AC007_AC008_PathBatchAndUnexpectedFailuresFailClosed()
        {
            var port = NewPort(null); Assert.Throws<ArgumentException>(() => Invoke(null, port)); Assert.Throws<ArgumentException>(() => Invoke(string.Empty, port)); Assert.Throws<ArgumentException>(() => Invoke("relative", port)); Assert.Throws<ArgumentException>(() => Invoke(Path.GetPathRoot(Path.GetTempPath()), port)); Assert.That(port.Calls.Count, Is.Zero);
            Assert.Throws<InvalidOperationException>(() => default(ProfileLaunchObservationBatchV1).Validate());
            var malformed = Corrupt(default(ProfileLaunchObservationBatchV1), "_primary", ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous)); Assert.Throws<InvalidOperationException>(() => malformed.Validate());
            var valid = Corrupt(default(ProfileLaunchObservationBatchV1), "_primary", ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary)); valid = Corrupt(valid, "_previous", ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous)); valid = Corrupt(valid, "_temp", ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)); valid.Validate();
            var duplicate = Corrupt(valid, "_previous", ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary)); Assert.Throws<InvalidOperationException>(() => { var ignored = duplicate.Primary; });
            var unknown = Corrupt(default(ProfileLaunchObservationBatchV1), "_primary", CorruptCandidate(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary), "_kind", (ProfileLoadCandidateKind)99)); Assert.Throws<InvalidOperationException>(() => unknown.Validate());
            WithDirectory(root => { var invalidPort = NewPort("presence-invalid"); Assert.Throws<InvalidOperationException>(() => Invoke(root, invalidPort)); Assert.That(invalidPort.Calls, Is.EqualTo(new[] { "probe:profile.json" })); File.WriteAllBytes(Path.Combine(root, "profile.json"), Utf8("{}")); Assert.Throws<InvalidOperationException>(() => Invoke(root, NewPort("unexpected")), "unexpected read fault must escape after a present probe"); Assert.Throws<OutOfMemoryException>(() => Invoke(root, NewPort("fatal")), "fatal read fault must escape after a present probe"); });
        }

        [Test]
        public void AC009_AC010_SourceIsReadOnlyFixedLeafAndHasNoLeaseAuthority()
        {
            var source = File.ReadAllText(Path.Combine(FindRoot(), "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileLaunchObservationAdapterV1.cs"));
            foreach (var required in new[] { "profile.json", "profile.prev.json", "profile.tmp.json", "File.GetAttributes", "FileAccess.Read", "FileShare.Read", "CompletedAndClosed" }) StringAssert.Contains(required, source);
            foreach (var forbidden in new[] { "File.Delete", "File.Copy", "File.Move", "File.Write", "UnityEngine", "persistentDataPath", "DateTime.Now", "Random", "Dictionary<", "lock (" }) StringAssert.DoesNotContain(forbidden, source);
        }

        private static byte[] Document(long revision, string asset) { var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), new ProfileInputSnapshot(asset == "current" ? ProfileInputContract.CurrentInputActionsAssetId : "obsolete", 1, ProfileBindingOverridesJson.Parse(string.Empty)), new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>())); return ProfileCanonicalEncoder.Encode(snapshot).CanonicalFileBytes; }
        private static byte[] Binding(byte[] source)
        {
            var document = ProfileCanonicalDecoderV1.Decode(source).Document; var payload = Encoding.UTF8.GetString(document.CanonicalPayloadBytes).Replace("\"bindingOverridesJson\":\"\"", "\"bindingOverridesJson\":\"{bad}\""); var payloadBytes = Utf8(payload); var hash = Hash(payloadBytes);
            return Utf8(payload.Substring(0, payload.Length - 1) + ",\"integrity\":{\"payloadSha256\":\"" + hash + "\"}}");
        }
        private static string Hash(byte[] value) { using (var sha = System.Security.Cryptography.SHA256.Create()) { var bytes = sha.ComputeHash(value); var text = new StringBuilder(bytes.Length * 2); foreach (var item in bytes) text.Append(item.ToString("x2", System.Globalization.CultureInfo.InvariantCulture)); return text.ToString(); } }
        private static byte[] Utf8(string value) => new UTF8Encoding(false).GetBytes(value);
        private static void WithDirectory(Action<string> action) { var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-M5D7G-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root); try { action(root); } finally { if (Directory.Exists(root)) Directory.Delete(root, true); } }
        private static FilePortProxy NewPort(string fault) { return new FilePortProxy { Fault = fault }; }
        private static ProfileLaunchObservationBatchV1 Invoke(string root, FilePortProxy source)
        {
            MethodInfo method = null; foreach (var item in typeof(ProfileLaunchObservationAdapterV1).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)) if (item.Name == "Observe" && item.GetParameters().Length == 2) { method = item; break; }
            var proxy = (FilePortProxy)CreateProxy(method.GetParameters()[1].ParameterType, typeof(FilePortProxy)); proxy.CopyFrom(source);
            try { return (ProfileLaunchObservationBatchV1)method.Invoke(null, new object[] { root, proxy }); } catch (TargetInvocationException error) when (error.InnerException != null) { ExceptionDispatchInfo.Capture(error.InnerException).Throw(); throw; }
        }
        private static object CreateProxy(Type contract, Type proxy) { MethodInfo create = null; foreach (var item in typeof(DispatchProxy).GetMethods(BindingFlags.Static | BindingFlags.Public)) if (item.Name == "Create" && item.IsGenericMethodDefinition && item.GetParameters().Length == 0) { create = item; break; } return create.MakeGenericMethod(contract, proxy).Invoke(null, null); }
        private static ProfileLaunchObservationBatchV1 Corrupt(ProfileLaunchObservationBatchV1 value, string field, object replacement) { object boxed = value; typeof(ProfileLaunchObservationBatchV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, replacement); return (ProfileLaunchObservationBatchV1)boxed; }
        private static ProfileLoadCandidateV1 CorruptCandidate(ProfileLoadCandidateV1 value, string field, object replacement) { object boxed = value; typeof(ProfileLoadCandidateV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, replacement); return (ProfileLoadCandidateV1)boxed; }
        private static string FindRoot() { var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (File.Exists(Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new DirectoryNotFoundException(); }

        public class FilePortProxy : DispatchProxy
        {
            internal sealed class SharedState { internal byte[] LastBytes; }
            internal string Fault; internal List<string> Calls = new List<string>(); internal int Reads; internal SharedState State = new SharedState();
            internal void CopyFrom(FilePortProxy source) { Fault = source.Fault; Calls = source.Calls; Reads = source.Reads; State = source.State; }
            protected override object Invoke(MethodInfo method, object[] args)
            {
                var path = (string)args[0]; var leaf = Path.GetFileName(path);
                if (method.Name == "ProbePresence") { Calls.Add("probe:" + leaf); if (Fault == "presence-invalid") return Enum.ToObject(method.ReturnType, 99); if (Fault == "probe:" + leaf) throw new UnauthorizedAccessException(); try { File.GetAttributes(path); return Enum.ToObject(method.ReturnType, 2); } catch (FileNotFoundException) { return Enum.ToObject(method.ReturnType, 1); } catch (DirectoryNotFoundException) { return Enum.ToObject(method.ReturnType, 1); } }
                if (method.Name == "ReadAllClosed")
                {
                    Calls.Add("open/read:" + leaf); Reads++; if (Fault == "unexpected") throw new InvalidOperationException(); if (Fault == "fatal") throw new OutOfMemoryException(); if (Fault == "read:" + leaf || Fault == "disappear:" + leaf) throw new IOException(); if (Fault == "eof:" + leaf) throw new EndOfStreamException();
                    if (Fault == "default") return Activator.CreateInstance(method.ReturnType); var bytes = Fault == "null" ? null : (File.Exists(path) ? File.ReadAllBytes(path) : Array.Empty<byte>()); State.LastBytes = bytes; Calls.Add("close:" + leaf); Calls.Add("return:" + leaf);
                    if (Fault == "not-closed") return ReadResult(method.ReturnType, 0, bytes == null ? 0 : bytes.Length, bytes == null ? 0 : bytes.Length, bytes);
                    if (Fault == "mismatch") return ReadResult(method.ReturnType, 1, 3, 2, bytes ?? Array.Empty<byte>());
                    if (Fault == "overflow") return ReadResult(method.ReturnType, 1, (long)int.MaxValue + 1, (long)int.MaxValue + 1, bytes ?? Array.Empty<byte>());
                    if (Fault == "negative") return ReadResult(method.ReturnType, 1, -1, -1, bytes ?? Array.Empty<byte>());
                    if (Fault == "unknown-completion") return ReadResult(method.ReturnType, 99, bytes == null ? 0 : bytes.Length, bytes == null ? 0 : bytes.Length, bytes);
                    if (Fault == "short") return ReadResult(method.ReturnType, 1, 2, 1, bytes ?? Array.Empty<byte>());
                    if (Fault == "zero-progress") return ReadResult(method.ReturnType, 1, 1, 0, bytes ?? Array.Empty<byte>());
                    return ReadResult(method.ReturnType, 1, bytes == null ? 0 : bytes.Length, bytes == null ? 0 : bytes.Length, bytes);
                }
                throw new MissingMethodException(method.Name);
            }
            private static object ReadResult(Type type, int completion, long expected, long read, byte[] bytes) { object boxed = Activator.CreateInstance(type); type.GetField("Completion", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(boxed, Enum.ToObject(type.GetField("Completion", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).FieldType, completion)); type.GetField("ExpectedLength", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(boxed, expected); type.GetField("BytesRead", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(boxed, read); type.GetField("Bytes", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(boxed, bytes); return boxed; }
        }
    }
}
