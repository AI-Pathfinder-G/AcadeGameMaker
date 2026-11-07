using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D7F-001..005; AC-M5D7F-001..010. Real isolated moves plus deterministic internal-operation faults.
    public sealed class ProfileQuarantineServiceV1Tests
    {
        private static readonly DateTimeOffset Utc = new DateTimeOffset(2026, 9, 13, 10, 11, 12, 345, TimeSpan.Zero);

        [Test]
        public void AC001_InvalidPrimaryMovesExactBytesToExactUtcHashName()
        {
            WithDirectory(root =>
            {
                var bytes = Utf8("{}"); File.WriteAllBytes(Path.Combine(root, "profile.json"), bytes);
                var result = ProfileQuarantineServiceV1.Preserve(root, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, Utc);
                Assert.That(result.Outcome, Is.EqualTo(ProfileQuarantineOutcome.Preserved)); Assert.That(result.FailureStage, Is.EqualTo(ProfileQuarantineStage.None));
                Assert.That("20260913T101112345Z".Length, Is.EqualTo(19));
                var expected = Path.Combine(root, "recovery", "profile-invalid-primary-20260913T101112345Z-" + Hash8(bytes) + ".json");
                Assert.That(result.DestinationPath, Is.EqualTo(expected)); Assert.That(result.HashSuffix, Is.EqualTo(Hash8(bytes))); Assert.That(result.CollisionOrdinal, Is.Zero);
                Assert.That(File.Exists(Path.Combine(root, "profile.json")), Is.False); CollectionAssert.AreEqual(bytes, File.ReadAllBytes(expected)); result.Validate();
            });
        }

        [Test]
        public void AC002_AllReasonFamiliesAndNoHashNamesAreExact()
        {
            WithDirectory(root =>
            {
                var unsupported = Utf8("{\"schemaVersion\":0}"); File.WriteAllBytes(Path.Combine(root, "profile.prev.json"), unsupported);
                var unsupportedCandidate = Candidate(ProfileLoadFileRole.Previous, unsupported);
                var unsupportedResult = ProfileQuarantineServiceV1.Preserve(root, unsupportedCandidate, ProfileFilePreservationReason.UnsupportedPrevious, Utc);
                StringAssert.EndsWith("profile-unsupported-v0-20260913T101112345Z-" + Hash8(unsupported) + ".json", unsupportedResult.DestinationPath);
                File.WriteAllText(Path.Combine(root, "profile.tmp.json"), "stale");
                var temp = Invoke(root, ProfileLoadCandidateV1.Unreadable(ProfileLoadFileRole.Temp), ProfileFilePreservationReason.StaleTemp, NewPort("read-fault"));
                Assert.That(temp.Outcome, Is.EqualTo(ProfileQuarantineOutcome.Preserved)); StringAssert.EndsWith("profile-stale-temp-20260913T101112345Z-nohash.json", temp.DestinationPath); Assert.That(temp.HashSuffix, Is.EqualTo("nohash"));
            });
        }

        [TestCase(ProfileLoadFileRole.Primary, ProfileFilePreservationReason.UnreadablePrimary, "profile.json", "primary")]
        [TestCase(ProfileLoadFileRole.Previous, ProfileFilePreservationReason.UnreadablePrevious, "profile.prev.json", "previous")]
        public void AC003_UnreadableRolesUseNoHashOnlyWithUnreadableProof(ProfileLoadFileRole role, ProfileFilePreservationReason reason, string sourceLeaf, string label)
        {
            WithDirectory(root =>
            {
                File.WriteAllText(Path.Combine(root, sourceLeaf), "not-read");
                var result = Invoke(root, ProfileLoadCandidateV1.Unreadable(role), reason, NewPort("read-fault"));
                Assert.That(result.Outcome, Is.EqualTo(ProfileQuarantineOutcome.Preserved)); Assert.That(result.HashSuffix, Is.EqualTo("nohash"));
                StringAssert.EndsWith("profile-unreadable-" + label + "-20260913T101112345Z-nohash.json", result.DestinationPath); result.Validate();
            });
        }

        [Test]
        public void AC003_CollisionAllocationUsesSmallestOrdinalAndCheckedFailureBeforeMove()
        {
            WithDirectory(root =>
            {
                var bytes = Utf8("{}"); var stem = "profile-invalid-primary-20260913T101112345Z-" + Hash8(bytes); var recovery = Path.Combine(root, "recovery"); Directory.CreateDirectory(recovery);
                File.WriteAllText(Path.Combine(recovery, stem + ".json"), "occupied"); File.WriteAllText(Path.Combine(recovery, stem + "-1.json"), "occupied"); File.WriteAllBytes(Path.Combine(root, "profile.json"), bytes);
                var result = ProfileQuarantineServiceV1.Preserve(root, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, Utc);
                Assert.That(result.CollisionOrdinal, Is.EqualTo(2)); StringAssert.EndsWith(stem + "-2.json", result.DestinationPath);
            });
        }

        [Test]
        public void AC004_ClassificationOnlyRevalidationRejectsChangedOrMismatchedSource()
        {
            WithDirectory(root =>
            {
                var invalid = Utf8("{}"); File.WriteAllBytes(Path.Combine(root, "profile.json"), invalid);
                var unsupportedCandidate = Candidate(ProfileLoadFileRole.Primary, Utf8("{\"schemaVersion\":0}"));
                var mismatch = ProfileQuarantineServiceV1.Preserve(root, unsupportedCandidate, ProfileFilePreservationReason.UnsupportedPrimary, Utc);
                Assert.That(mismatch.Outcome, Is.EqualTo(ProfileQuarantineOutcome.FailedBeforeMove)); Assert.That(mismatch.FailureStage, Is.EqualTo(ProfileQuarantineStage.SourceRevalidation));
                var changed = Invoke(root, Candidate(ProfileLoadFileRole.Primary, invalid), ProfileFilePreservationReason.InvalidPrimary, NewPort("change-current"));
                Assert.That(changed.Outcome, Is.EqualTo(ProfileQuarantineOutcome.FailedBeforeMove)); Assert.That(changed.FailureStage, Is.EqualTo(ProfileQuarantineStage.SourceRevalidation)); Assert.That(File.Exists(Path.Combine(root, "profile.json")), Is.True);
            });
        }

        [TestCase("missing", ProfileQuarantineOutcome.SourceMissing, ProfileQuarantineStage.SourceRevalidation)]
        [TestCase("presence-access", ProfileQuarantineOutcome.FailedBeforeMove, ProfileQuarantineStage.SourceRevalidation)]
        [TestCase("ensure", ProfileQuarantineOutcome.FailedBeforeMove, ProfileQuarantineStage.RecoveryDirectoryPreparation)]
        [TestCase("allocate", ProfileQuarantineOutcome.FailedBeforeMove, ProfileQuarantineStage.DestinationAllocation)]
        [TestCase("move", ProfileQuarantineOutcome.MoveOutcomeUncertain, ProfileQuarantineStage.MoveCall)]
        [TestCase("post", ProfileQuarantineOutcome.MoveOutcomeUncertain, ProfileQuarantineStage.PostMoveValidation)]
        [TestCase("hash-io", ProfileQuarantineOutcome.FailedBeforeMove, ProfileQuarantineStage.DestinationAllocation)]
        public void AC005_AC006_ClosedFailureMatrixMapsRecoverableFaults(string fault, ProfileQuarantineOutcome outcome, ProfileQuarantineStage stage)
        {
            WithDirectory(root =>
            {
                var bytes = Utf8("{}"); if (fault != "missing") File.WriteAllBytes(Path.Combine(root, "profile.json"), bytes);
                var port = NewPort(fault); var result = Invoke(root, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, port);
                Assert.That(result.Outcome, Is.EqualTo(outcome)); Assert.That(result.FailureStage, Is.EqualTo(stage)); result.Validate();
                Assert.That(port.MoveCount, Is.LessThanOrEqualTo(1));
            });
        }

        [Test]
        public void AC007_ArgumentsAndMalformedResultFailBeforeIo()
        {
            var port = NewPort(null); var invalid = Candidate(ProfileLoadFileRole.Primary, Utf8("{}"));
            Assert.Throws<ArgumentException>(() => Invoke("relative", invalid, ProfileFilePreservationReason.InvalidPrimary, port));
            Assert.Throws<ArgumentException>(() => Invoke(Path.GetPathRoot(Path.GetTempPath()), invalid, ProfileFilePreservationReason.InvalidPrimary, port));
            Assert.Throws<ArgumentException>(() => Invoke(Path.GetTempPath(), invalid, ProfileFilePreservationReason.InvalidPrevious, port)); Assert.That(port.Calls.Count, Is.Zero);
            Assert.Throws<InvalidOperationException>(() => default(ProfileQuarantineResultV1).Validate());
            var corrupt = Corrupt(default(ProfileQuarantineResultV1), "_outcome", ProfileQuarantineOutcome.Preserved);
            Assert.Throws<InvalidOperationException>(() => corrupt.Validate());
        }

        [Test]
        public void AC008_ResultStatesAndPostDigestDetectTampering()
        {
            WithDirectory(root =>
            {
                var bytes = Utf8("{}"); File.WriteAllBytes(Path.Combine(root, "profile.json"), bytes);
                var result = Invoke(root, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, NewPort("tamper"));
                Assert.That(result.Outcome, Is.EqualTo(ProfileQuarantineOutcome.MoveOutcomeUncertain)); Assert.That(result.FailureStage, Is.EqualTo(ProfileQuarantineStage.PostMoveValidation));
                Assert.That(result.SourceState, Is.EqualTo(ProfileQuarantineFileState.Missing)); Assert.That(result.DestinationState, Is.EqualTo(ProfileQuarantineFileState.Present));
            });
        }

        [Test]
        public void AC009_ResultProofRejectsEveryForgedDestinationIdentityField()
        {
            WithDirectory(root =>
            {
                var bytes = Utf8("{}"); File.WriteAllBytes(Path.Combine(root, "profile.json"), bytes);
                var result = ProfileQuarantineServiceV1.Preserve(root, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, Utc);
                result.Validate();
                Assert.That(result.SourceRole, Is.EqualTo(ProfileLoadFileRole.Primary)); Assert.That(result.Reason, Is.EqualTo(ProfileFilePreservationReason.InvalidPrimary)); Assert.That(result.SourceState, Is.EqualTo(ProfileQuarantineFileState.Missing)); Assert.That(result.DestinationState, Is.EqualTo(ProfileQuarantineFileState.Present));
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_destinationPath", Path.Combine(root, "recovery", "profile-evil.json")).Validate());
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_utcToken", "20260913T101112346Z").Validate());
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_hashSuffix", "12345678").Validate());
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_collisionOrdinal", 1).Validate());
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_root", root + "-other").Validate());
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_hashWasProven", false).Validate());
            });
            WithDirectory(root =>
            {
                var bytes = Utf8("{\"schemaVersion\":0}"); File.WriteAllBytes(Path.Combine(root, "profile.json"), bytes);
                var result = ProfileQuarantineServiceV1.Preserve(root, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.UnsupportedPrimary, Utc);
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_unsupportedVersion", 9).Validate());
            });
            WithDirectory(root =>
            {
                File.WriteAllText(Path.Combine(root, "profile.tmp.json"), "readable stale");
                var result = ProfileQuarantineServiceV1.Preserve(root, Candidate(ProfileLoadFileRole.Temp, Utf8("{}")), ProfileFilePreservationReason.StaleTemp, Utc);
                Assert.Throws<InvalidOperationException>(() => Corrupt(result, "_hashSuffix", "nohash").Validate());
            });
        }

        [Test]
        public void AC009_UnexpectedAndFatalFaultsEscapeAndReleaseLease()
        {
            WithDirectory(root =>
            {
                var bytes = Utf8("{}"); File.WriteAllBytes(Path.Combine(root, "profile.json"), bytes);
                Assert.Throws<InvalidOperationException>(() => Invoke(root, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, NewPort("unexpected")));
                Assert.That(RegistryCount(), Is.Zero);
                Assert.Throws<InvalidOperationException>(() => Invoke(root, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, NewPort("hash-malformed")));
                Assert.That(RegistryCount(), Is.Zero);
                Assert.Throws<OutOfMemoryException>(() => Invoke(root, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, NewPort("fatal")));
                Assert.That(RegistryCount(), Is.Zero);
            });
        }

        [Test]
        public void AC010_AliasesSerializeWhileDifferentRootsProceedAndSourceHasNoEngineAuthority()
        {
            var left = CreateDirectory(); var right = CreateDirectory();
            try
            {
                var bytes = Utf8("{}"); File.WriteAllBytes(Path.Combine(left, "profile.json"), bytes); var started = new ManualResetEventSlim(false); var release = new ManualResetEventSlim(false); var port = NewPort("block"); port.Started = started; port.Release = release;
                var first = Task.Run(() => Invoke(left, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, port));
                Task<ProfileQuarantineResultV1> same = null;
                try
                {
                    Assert.That(started.Wait(TimeSpan.FromSeconds(5)), Is.True);
                    same = Task.Run(() => ProfileQuarantineServiceV1.Preserve(left.ToUpperInvariant() + Path.DirectorySeparatorChar, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, Utc));
                    Assert.That(SpinWait.SpinUntil(() => LeaseReferenceCount() == 2, TimeSpan.FromSeconds(5)), Is.True);
                    Assert.That(same.Wait(TimeSpan.FromMilliseconds(100)), Is.False);
                    var other = Task.Run(() => ProfileQuarantineServiceV1.Preserve(right, Candidate(ProfileLoadFileRole.Primary, bytes), ProfileFilePreservationReason.InvalidPrimary, Utc)); Assert.That(other.Wait(TimeSpan.FromSeconds(5)), Is.True);
                }
                finally { release.Set(); }
                Assert.That(first.Wait(TimeSpan.FromSeconds(5)), Is.True); Assert.That(same.Wait(TimeSpan.FromSeconds(5)), Is.True); Assert.That(RegistryCount(), Is.Zero);
                var source = File.ReadAllText(Path.Combine(FindRoot(), "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileQuarantineServiceV1.cs"));
                foreach (var required in new[] { "File.GetAttributes", "File.Move", "SHA256", "profile.prev.json", "profile.tmp.json" }) StringAssert.Contains(required, source);
                foreach (var forbidden in new[] { "File.Delete", "File.Copy", "UnityEngine", "persistentDataPath", "DateTime.Now", "Random", "profile.previous.json" }) StringAssert.DoesNotContain(forbidden, source);
            }
            finally { DeleteDirectory(left); DeleteDirectory(right); }
        }

        private static ProfileLoadCandidateV1 Candidate(ProfileLoadFileRole role, byte[] bytes) => ProfileLoadCandidateV1.Decoded(role, ProfileCanonicalDecoderV1.Decode(bytes));
        private static byte[] Utf8(string value) => new UTF8Encoding(false).GetBytes(value);
        private static string Hash8(byte[] value) { using (var sha = SHA256.Create()) { var all = sha.ComputeHash(value); var text = new StringBuilder(); foreach (var b in all) text.Append(b.ToString("x2", CultureInfo.InvariantCulture)); return text.ToString(0, 8); } }
        private static void WithDirectory(Action<string> action) { var root = CreateDirectory(); try { action(root); } finally { DeleteDirectory(root); } }
        private static string CreateDirectory() { var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-M5D7F-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root); return root; }
        private static void DeleteDirectory(string root) { if (Directory.Exists(root)) Directory.Delete(root, true); }
        private static FilePortProxy NewPort(string fault) { return new FilePortProxy { Fault = fault }; }
        private static ProfileQuarantineResultV1 Invoke(string root, ProfileLoadCandidateV1 candidate, ProfileFilePreservationReason reason, FilePortProxy source)
        {
            MethodInfo method = null; foreach (var item in typeof(ProfileQuarantineServiceV1).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)) if (item.Name == "Preserve" && item.GetParameters().Length == 5) { method = item; break; }
            var proxy = (FilePortProxy)CreateProxy(method.GetParameters()[4].ParameterType, typeof(FilePortProxy)); proxy.CopyFrom(source);
            try { return (ProfileQuarantineResultV1)method.Invoke(null, new object[] { root, candidate, reason, Utc, proxy }); }
            catch (TargetInvocationException exception) when (exception.InnerException != null) { ExceptionDispatchInfo.Capture(exception.InnerException).Throw(); throw; }
        }
        private static object CreateProxy(Type contract, Type proxy) { MethodInfo create = null; foreach (var item in typeof(DispatchProxy).GetMethods(BindingFlags.Public | BindingFlags.Static)) if (item.Name == "Create" && item.IsGenericMethodDefinition && item.GetParameters().Length == 0) { create = item; break; } return create.MakeGenericMethod(contract, proxy).Invoke(null, null); }
        private static int RegistryCount() => ((IDictionary)typeof(ProfileQuarantineServiceV1).GetField("Leases", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null)).Count;
        private static int LeaseReferenceCount()
        {
            var leases = (IDictionary)typeof(ProfileQuarantineServiceV1).GetField("Leases", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            if (leases.Count != 1) return -1;
            foreach (DictionaryEntry entry in leases) return (int)entry.Value.GetType().GetField("Count", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(entry.Value);
            return -1;
        }
        private static ProfileQuarantineResultV1 Corrupt(ProfileQuarantineResultV1 value, string field, object replacement) { object boxed = value; typeof(ProfileQuarantineResultV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, replacement); return (ProfileQuarantineResultV1)boxed; }
        private static string FindRoot() { var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (File.Exists(Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new DirectoryNotFoundException(); }

        public class FilePortProxy : DispatchProxy
        {
            internal string Fault; internal List<string> Calls = new List<string>(); internal int MoveCount; internal ManualResetEventSlim Started; internal ManualResetEventSlim Release;
            internal void CopyFrom(FilePortProxy other) { Fault = other.Fault; Calls = other.Calls; MoveCount = other.MoveCount; Started = other.Started; Release = other.Release; }
            protected override object Invoke(MethodInfo method, object[] args)
            {
                var path = args != null && args.Length > 0 ? args[0] as string : null; Calls.Add(method.Name);
                if (method.Name == "ProbePresence") { if (Fault == "missing" && path.EndsWith("profile.json", StringComparison.Ordinal)) return Enum.ToObject(method.ReturnType, 1); if (Fault == "presence-access" && path.EndsWith("profile.json", StringComparison.Ordinal)) throw new UnauthorizedAccessException("presence"); if (Fault == "allocate" && path.IndexOf("recovery", StringComparison.OrdinalIgnoreCase) >= 0) throw new IOException("allocation"); try { File.GetAttributes(path); return Enum.ToObject(method.ReturnType, 2); } catch (FileNotFoundException) { return Enum.ToObject(method.ReturnType, 1); } catch (DirectoryNotFoundException) { return Enum.ToObject(method.ReturnType, 1); } }
                if (method.Name == "ReadAllExclusive") { if (Fault == "unexpected") throw new InvalidOperationException("unexpected"); if (Fault == "fatal") throw new OutOfMemoryException("fatal"); if (Fault == "read-fault") throw new IOException("read"); if (Fault == "change-current") return Utf8("{\"schemaVersion\":0}"); if (Fault == "post" && path.IndexOf("recovery", StringComparison.OrdinalIgnoreCase) >= 0) throw new IOException("post"); return File.ReadAllBytes(path); }
                if (method.Name == "EnsureDirectory") { if (Fault == "ensure") throw new UnauthorizedAccessException("ensure"); Directory.CreateDirectory(path); return null; }
                if (method.Name == "Hash") { if (Fault == "hash-io") throw new IOException("hash"); if (Fault == "hash-malformed") return "BAD"; using (var sha = SHA256.Create()) { var all = sha.ComputeHash((byte[])args[0]); var text = new StringBuilder(); foreach (var b in all) text.Append(b.ToString("x2", CultureInfo.InvariantCulture)); return text.ToString(); } }
                if (method.Name == "Move") { MoveCount++; if (Fault == "move") throw new IOException("move"); if (Fault == "block") { Started.Set(); Release.Wait(TimeSpan.FromSeconds(10)); } File.Move((string)args[0], (string)args[1]); if (Fault == "tamper") File.WriteAllText((string)args[1], "tampered"); return null; }
                throw new MissingMethodException(method.Name);
            }
        }
    }
}
