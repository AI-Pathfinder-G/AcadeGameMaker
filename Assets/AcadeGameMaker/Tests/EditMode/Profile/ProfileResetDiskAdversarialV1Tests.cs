using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Profile.Tests
{
    // REQ-M5D7QC1-001/002/003/005/006/007.  AC-001/002/004/007/009 use
    // only real files and the production decoder; no reset validation seam is bypassed.
    public sealed class ProfileResetDiskAdversarialV1Tests
    {
        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void AC001_FreshRoleMutationIsStaleAndLeavesEveryFileUntouched(int changedRole)
        {
            WithRoot(root =>
            {
                WriteLeaves(root, Document(3).CanonicalFileBytes, Document(4).CanonicalFileBytes, new byte[] { 1, 2, 3 });
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                var names = new[] { "profile.json", "profile.prev.json", "profile.tmp.json" };
                var replacement = changedRole == 0 ? Document(8).CanonicalFileBytes : changedRole == 1 ? Unsupported() : new byte[] { 9, 7, 5, 3 };
                File.WriteAllBytes(Path.Combine(root, names[changedRole]), replacement);
                var before = Snapshot(root);
                AssertResult(ProfileResetDiskTransactionV1.Begin(root, identity), ProfileResetDiskOutcomeV1.ConfirmationStale);
                AssertSnapshot(root, before);
                Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.False);
            });
        }

        [Test]
        public void AC004_AllRoleClassificationsArchiveExactlyBeforeDefaultAppears()
        {
            WithRoot(root =>
            {
                var primary = ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes;
                var previous = Unsupported(); var temp = new byte[] { 0, 255, 4, 8 };
                WriteLeaves(root, primary, previous, temp);
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                AssertResult(result, ProfileResetDiskOutcomeV1.DiskPrepared);
                var tx = Path.Combine(root, "recovery", "new-game", result.TransactionId);
                CollectionAssert.AreEqual(primary, File.ReadAllBytes(Path.Combine(tx, "primary.bin")));
                CollectionAssert.AreEqual(previous, File.ReadAllBytes(Path.Combine(tx, "previous.bin")));
                CollectionAssert.AreEqual(temp, File.ReadAllBytes(Path.Combine(tx, "temp.bin")));
                CollectionAssert.AreEqual(primary, File.ReadAllBytes(Path.Combine(root, "profile.json")));
                Assert.That(File.Exists(Path.Combine(root, "profile.prev.json")), Is.False);
                Assert.That(File.Exists(Path.Combine(root, "profile.tmp.json")), Is.False);
            });
        }

        [Test]
        public void AC002_GoldenMarkerSingleFieldOrderTypeCaseHashAndVersionMutationsFailClosed()
        {
            foreach (var mutation in new Func<string, string>[]
            {
                text => Rehash(text.Replace("\"format\":\"AcadeGameMaker.ProfileResetBarrier\"", "\"format\":\"Wrong\"")),
                text => Rehash(text.Replace("\"version\":1", "\"version\":2")),
                text => Rehash(text.Replace("\"profileRevision\":0", "\"profileRevision\":0.0")),
                text => Rehash(text.Replace("\"role\":\"Primary\"", "\"role\":\"primary\"")),
                text => Rehash(text.Replace("\"sha256\":\"", "\"sha256\":123,\"ignored\":\"")),
                text => Rehash(MoveVersionAfterTransaction(text)),
                text => Rehash(text.Replace(",\"bodySha256\"", ",\"unknown\":null,\"bodySha256\"")),
                text => text.Substring(0, text.Length - 2) + "00\"}"
            })
            {
                WithGoldenMarker((root, marker) =>
                {
                    File.WriteAllText(Path.Combine(root, "profile.reset.json"), mutation(marker), new UTF8Encoding(false));
                    AssertResult(ProfileResetDiskTransactionV1.Resume(root), ProfileResetDiskOutcomeV1.ManualRepairRequired);
                    AssertOrdinaryRoutesBlocked(root);
                });
            }
        }

        [Test]
        public void AC002_OversizeDirectoryAndExclusiveLockedMarkerAreTerminal()
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.reset.json"), new byte[8193]);
                AssertResult(ProfileResetDiskTransactionV1.Resume(root), ProfileResetDiskOutcomeV1.ManualRepairRequired);
            });
            WithRoot(root =>
            {
                Directory.CreateDirectory(Path.Combine(root, "profile.reset.json"));
                AssertResult(ProfileResetDiskTransactionV1.Resume(root), ProfileResetDiskOutcomeV1.ManualRepairRequired);
            });
            WithGoldenMarker((root, marker) =>
            {
                using (new FileStream(Path.Combine(root, "profile.reset.json"), FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    AssertResult(ProfileResetDiskTransactionV1.Resume(root), ProfileResetDiskOutcomeV1.ManualRepairRequired);
            });
        }

        [Test]
        public void AC007_ArchiveBothNeitherAndMismatchRemainManualRepair()
        {
            foreach (var adversary in new Action<string, string>[]
            {
                (root, tx) => File.Copy(Path.Combine(root, "profile.json"), Path.Combine(tx, "primary.bin")),
                (root, tx) => { File.Delete(Path.Combine(root, "profile.json")); },
                (root, tx) => File.WriteAllBytes(Path.Combine(tx, "primary.bin"), new byte[] { 4, 4 })
            })
            {
                WithReconstructedPendingOldState((root, tx) =>
                {
                    adversary(root, tx);
                    AssertResult(ProfileResetDiskTransactionV1.Resume(root), ProfileResetDiskOutcomeV1.ManualRepairRequired);
                });
            }
        }

        [Test]
        public void AC007_CorruptNewPrimaryIsPreservedThenExactDefaultIsRebuilt()
        {
            WithGoldenMarker((root, marker) =>
            {
                var corrupt = new byte[] { 11, 12, 13 }; File.WriteAllBytes(Path.Combine(root, "profile.json"), corrupt);
                var prepared = ProfileResetDiskTransactionV1.Resume(root);
                AssertResult(prepared, ProfileResetDiskOutcomeV1.DiskPrepared);
                var folder = Path.Combine(root, "recovery", "new-game", prepared.TransactionId, "interrupted-default-primary");
                CollectionAssert.AreEqual(corrupt, File.ReadAllBytes(Directory.GetFiles(folder)[0]));
                CollectionAssert.AreEqual(ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(root, "profile.json")));
            });
        }

        [Test]
        public void AC007_IdenticalInterruptedTempCollisionDoesNotOverwriteEitherCopy()
        {
            WithGoldenMarker((root, marker) =>
            {
                var bytes = new byte[] { 3, 1, 4, 1, 5 }; File.WriteAllBytes(Path.Combine(root, "profile.tmp.json"), bytes);
                var tx = Directory.GetDirectories(Path.Combine(root, "recovery", "new-game"))[0];
                var destination = Path.Combine(tx, "interrupted-default-temp", Hash(bytes) + "-5.bin"); Directory.CreateDirectory(Path.GetDirectoryName(destination)); File.WriteAllBytes(destination, bytes);
                AssertResult(ProfileResetDiskTransactionV1.Resume(root), ProfileResetDiskOutcomeV1.ManualRepairRequired);
                CollectionAssert.AreEqual(bytes, File.ReadAllBytes(Path.Combine(root, "profile.tmp.json")));
                CollectionAssert.AreEqual(bytes, File.ReadAllBytes(destination));
            });
        }

        [Test]
        public void AC002_AC007_LoneBarrierTempIsInertAndBeginCollisionDoesNotMutateActiveLeaves()
        {
            WithRoot(root =>
            {
                var old = Document(12).CanonicalFileBytes; File.WriteAllBytes(Path.Combine(root, "profile.json"), old);
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                File.WriteAllText(Path.Combine(root, "profile.reset.tmp.json"), "inert");
                AssertResult(ProfileResetDiskTransactionV1.Resume(root), ProfileResetDiskOutcomeV1.ManualRepairRequired);
                AssertResult(ProfileResetDiskTransactionV1.Begin(root, identity), ProfileResetDiskOutcomeV1.ManualRepairRequired);
                CollectionAssert.AreEqual(old, File.ReadAllBytes(Path.Combine(root, "profile.json")));
                Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.False);
            });
        }

        [Test]
        public void AC009_OutputsValidateAndBarrierBlocksObserveSaveAndInputRepairWithoutActiveMutation()
        {
            WithGoldenMarker((root, marker) =>
            {
                var before = Snapshot(root);
                AssertOrdinaryRoutesBlocked(root);
                AssertSnapshot(root, before);
                var result = ProfileResetDiskTransactionV1.Resume(root); result.Validate();
                Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
            });
        }

        // REQ-M5D7QC1-001/002/003/004/007.  These cases intentionally use the
        // decoder only to prove each fixture class before Begin; the marker oracle
        // below is literal UTF-8 text and its own System SHA-256 calculation.
        [TestCaseSource(nameof(EachRoleClassCases))]
        public void AC004_EachRoleClassArchivesExactlyAndMarkerRecordsActualClass(int changedRole, FixtureKind kind)
        {
            WithRoot(root =>
            {
                var fixtures = new[] { Fixture.Missing(), Fixture.Missing(), Fixture.Missing() };
                fixtures[changedRole] = Fixture.Create(kind, 31 + changedRole);
                AssertFixtureClasses(fixtures);
                WriteFixtureLeaves(root, fixtures);

                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                AssertResult(result, ProfileResetDiskOutcomeV1.DiskPrepared);
                AssertLiteralMarker(root, result.TransactionId, fixtures);
                AssertArchivedAndPrepared(root, result.TransactionId, fixtures);
            });
        }

        [TestCaseSource(nameof(MixedRoleClassCases))]
        public void AC004_MixedThreeRoleClassesArchiveExactlyAndMarkerRecordsActualClasses(FixtureKind primary, FixtureKind previous, FixtureKind temp)
        {
            WithRoot(root =>
            {
                var fixtures = new[] { Fixture.Create(primary, 41), Fixture.Create(previous, 42), Fixture.Create(temp, 43) };
                AssertFixtureClasses(fixtures);
                WriteFixtureLeaves(root, fixtures);

                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                AssertResult(result, ProfileResetDiskOutcomeV1.DiskPrepared);
                AssertLiteralMarker(root, result.TransactionId, fixtures);
                AssertArchivedAndPrepared(root, result.TransactionId, fixtures);
            });
        }

        [Test]
        public void AC002_LiteralMarkerOracleUsesInvariantLengthsUnderHostileCulture()
        {
            WithRoot(root =>
            {
                var original = CultureInfo.CurrentCulture;
                try
                {
                    CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
                    var fixtures = new[] { Fixture.Invalid(new byte[1201]), Fixture.Missing(), Fixture.Missing() };
                    AssertFixtureClasses(fixtures);
                    WriteFixtureLeaves(root, fixtures);
                    var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                    AssertResult(result, ProfileResetDiskOutcomeV1.DiskPrepared);
                    AssertLiteralMarker(root, result.TransactionId, fixtures);
                    StringAssert.Contains("\"byteLength\":1201", File.ReadAllText(Path.Combine(root, "profile.reset.json"), Encoding.UTF8));
                }
                finally { CultureInfo.CurrentCulture = original; }
            });
        }

        [TestCaseSource(nameof(MarkerMutationKinds))]
        public void AC002_RehashedSingleMarkerMutationsAreRejectedByDecoderBeforeArchive(string kind)
        {
            WithRoot(root =>
            {
                var fixtures = new[] { Fixture.Current(61), Fixture.Missing(), Fixture.Missing() };
                WriteFixtureLeaves(root, fixtures);
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));
                AssertResult(result, ProfileResetDiskOutcomeV1.DiskPrepared);
                var marker = File.ReadAllText(Path.Combine(root, "profile.reset.json"), Encoding.UTF8);
                var mutated = MutateMarker(marker, kind, result.TransactionId);
                File.WriteAllText(Path.Combine(root, "profile.reset.json"), mutated, new UTF8Encoding(false));
                AssertResult(ProfileResetDiskTransactionV1.Resume(root), ProfileResetDiskOutcomeV1.ManualRepairRequired);
            });
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void AC001_UnreadableExclusiveLeafBeforeBeginDoesNotChangeAnyActiveLeaf(int role)
        {
            WithRoot(root =>
            {
                var fixtures = new[] { Fixture.Current(71), Fixture.RecoveryMetadata(72), Fixture.Invalid(new byte[] { 7, 1, 7 }) };
                WriteFixtureLeaves(root, fixtures);
                var identity = ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root);
                var before = Snapshot(root);
                using (new FileStream(Path.Combine(root, Names[role]), FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    AssertResult(ProfileResetDiskTransactionV1.Begin(root, identity), ProfileResetDiskOutcomeV1.ManualRepairRequired);
                AssertSnapshot(root, before);
                Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.False);
            });
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void AC007_UnreadablePendingOldSourceFailsClosedWithoutMoves(int role)
        {
            WithReconstructedPendingOldState((root, tx) =>
            {
                var before = Snapshot(root);
                using (new FileStream(Path.Combine(root, Names[role]), FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    AssertResult(ProfileResetDiskTransactionV1.Resume(root), ProfileResetDiskOutcomeV1.ManualRepairRequired);
                AssertSnapshot(root, before);
            });
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void AC007_UnreadableArchiveDestinationAndCollisionFailClosedForEachRole(int role)
        {
            WithReconstructedPendingOldState((root, tx) =>
            {
                var destination = Path.Combine(tx, ArchiveNames[role]);
                File.WriteAllBytes(destination, new byte[] { 3, 3, (byte)role });
                var before = Snapshot(root);
                using (new FileStream(destination, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    AssertResult(ProfileResetDiskTransactionV1.Resume(root), ProfileResetDiskOutcomeV1.ManualRepairRequired);
                AssertSnapshot(root, before);
                CollectionAssert.AreEqual(new byte[] { 3, 3, (byte)role }, File.ReadAllBytes(destination));
            });
        }

        // A real Begin produces the canonical marker.  We then move its exact archived
        // fixture bytes back under only this test root to model a published, pre-move crash.
        private static void WithReconstructedPendingOldState(Action<string, string> test)
        {
            WithRoot(root =>
            {
                WriteLeaves(root, ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes, Document(6).CanonicalFileBytes, new byte[] { 7, 7, 7 });
                var prepared = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root)); AssertResult(prepared, ProfileResetDiskOutcomeV1.DiskPrepared);
                var tx = Path.Combine(root, "recovery", "new-game", prepared.TransactionId);
                File.Delete(Path.Combine(root, "profile.json"));
                File.Move(Path.Combine(tx, "primary.bin"), Path.Combine(root, "profile.json"));
                File.Move(Path.Combine(tx, "previous.bin"), Path.Combine(root, "profile.prev.json"));
                File.Move(Path.Combine(tx, "temp.bin"), Path.Combine(root, "profile.tmp.json"));
                test(root, tx);
            });
        }

        private static void WithGoldenMarker(Action<string, string> test)
        {
            WithRoot(root =>
            {
                var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root)); AssertResult(result, ProfileResetDiskOutcomeV1.DiskPrepared);
                test(root, File.ReadAllText(Path.Combine(root, "profile.reset.json"), Encoding.UTF8));
            });
        }

        private static void AssertOrdinaryRoutesBlocked(string root)
        {
            Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileLaunchObservationAdapterV1.Observe(root));
            Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileAtomicSaveServiceV1.Save(root, Document(0)));
            var recovery = ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Primary, ProfileCanonicalDecoderV1.Decode(RecoveryDocument(2).CanonicalFileBytes));
            Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileAtomicSaveServiceV1.SaveInputRecovery(root, recovery));
        }

        private static readonly string[] Names = { "profile.json", "profile.prev.json", "profile.tmp.json" };
        private static readonly string[] ArchiveNames = { "primary.bin", "previous.bin", "temp.bin" };

        private static IEnumerable EachRoleClassCases()
        {
            foreach (var kind in new[] { FixtureKind.Missing, FixtureKind.ValidCurrent, FixtureKind.ValidRecoveryMetadata, FixtureKind.ValidRecoveryBinding, FixtureKind.Unsupported, FixtureKind.Invalid })
                for (var role = 0; role < 3; role++)
                    yield return new TestCaseData(role, kind).SetName("AC004_" + Names[role] + "_" + kind);
        }

        private static IEnumerable MixedRoleClassCases()
        {
            yield return new TestCaseData(FixtureKind.ValidCurrent, FixtureKind.ValidRecoveryMetadata, FixtureKind.Unsupported);
            yield return new TestCaseData(FixtureKind.ValidRecoveryBinding, FixtureKind.Invalid, FixtureKind.Missing);
            yield return new TestCaseData(FixtureKind.Unsupported, FixtureKind.ValidCurrent, FixtureKind.Invalid);
            yield return new TestCaseData(FixtureKind.Missing, FixtureKind.ValidRecoveryBinding, FixtureKind.ValidRecoveryMetadata);
            yield return new TestCaseData(FixtureKind.Invalid, FixtureKind.Unsupported, FixtureKind.ValidCurrent);
        }

        private static IEnumerable MarkerMutationKinds()
        {
            yield return new TestCaseData("upper-id");
            yield return new TestCaseData("path-extra");
            yield return new TestCaseData("duplicate");
            yield return new TestCaseData("missing-nonnull");
            yield return new TestCaseData("present-missing-revision");
            yield return new TestCaseData("unknown-classification");
        }

        private static string MutateMarker(string marker, string kind, string id)
        {
            string changed;
            switch (kind)
            {
                case "upper-id": changed = marker.Replace(id, id.ToUpperInvariant()); break;
                case "path-extra": changed = marker.Replace("\"fileName\":\"profile.json\"", "\"fileName\":\"profile.json.extra\""); break;
                case "duplicate": changed = marker.Replace("\"version\":1", "\"version\":1,\"version\":1"); break;
                case "missing-nonnull": changed = marker.Replace("\"presence\":\"Missing\",\"classification\":null", "\"presence\":\"Missing\",\"classification\":\"Invalid\""); break;
                case "present-missing-revision": changed = marker.Replace(",\"decodedRevision\":61}", "}"); break;
                case "unknown-classification": changed = marker.Replace("\"classification\":\"ValidCurrentInput\"", "\"classification\":\"UnknownClass\""); break;
                default: throw new ArgumentOutOfRangeException(nameof(kind));
            }
            return Rehash(changed);
        }

        private static void AssertFixtureClasses(Fixture[] fixtures)
        {
            for (var i = 0; i < fixtures.Length; i++)
            {
                if (fixtures[i].Kind == FixtureKind.Missing) continue;
                var decoded = ProfileCanonicalDecoderV1.Decode(fixtures[i].Bytes);
                Assert.That(decoded.Classification, Is.EqualTo(fixtures[i].ExpectedDecoderClass), "fixture decoder class for " + Names[i]);
            }
        }

        private static void WriteFixtureLeaves(string root, Fixture[] fixtures)
        {
            for (var i = 0; i < fixtures.Length; i++)
                if (fixtures[i].Kind != FixtureKind.Missing) File.WriteAllBytes(Path.Combine(root, Names[i]), fixtures[i].Bytes);
        }

        // This deliberately does not call MarkerText, the barrier parser, or a
        // production hash helper.  It is a fixed-order wire-format oracle.
        private static void AssertLiteralMarker(string root, string transactionId, Fixture[] fixtures)
        {
            var expected = ExpectedMarkerText(transactionId, fixtures);
            var actualBytes = File.ReadAllBytes(Path.Combine(root, "profile.reset.json"));
            CollectionAssert.AreEqual(new UTF8Encoding(false).GetBytes(expected), actualBytes);
            var bodyEnd = expected.LastIndexOf(",\"bodySha256\"", StringComparison.Ordinal);
            var body = expected.Substring(0, bodyEnd) + "}";
            var actualHashStart = expected.IndexOf("\"bodySha256\":\"", StringComparison.Ordinal) + "\"bodySha256\":\"".Length;
            Assert.That(expected.Substring(actualHashStart, 64), Is.EqualTo(Hash(new UTF8Encoding(false).GetBytes(body))));
        }

        private static string ExpectedMarkerText(string transactionId, Fixture[] fixtures)
        {
            var target = ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes;
            var body = "{\"format\":\"AcadeGameMaker.ProfileResetBarrier\",\"version\":1,\"transactionId\":\"" + transactionId + "\",\"target\":{\"profileRevision\":0,\"byteLength\":" + target.LongLength.ToString(CultureInfo.InvariantCulture) + ",\"sha256\":\"" + Hash(target) + "\"},\"oldLeaves\":[" + ExpectedLeaf("Primary", "profile.json", fixtures[0]) + "," + ExpectedLeaf("Previous", "profile.prev.json", fixtures[1]) + "," + ExpectedLeaf("Temp", "profile.tmp.json", fixtures[2]) + "]}";
            return body.Substring(0, body.Length - 1) + ",\"bodySha256\":\"" + Hash(new UTF8Encoding(false).GetBytes(body)) + "\"}";
        }

        private static string ExpectedLeaf(string role, string name, Fixture fixture)
        {
            if (fixture.Kind == FixtureKind.Missing)
                return "{\"role\":\"" + role + "\",\"fileName\":\"" + name + "\",\"presence\":\"Missing\",\"classification\":null,\"byteLength\":null,\"sha256\":null,\"decodedRevision\":null}";
            var revision = fixture.ExpectedLeafClass == "ValidCurrentInput" || fixture.ExpectedLeafClass == "ValidInputRecoveryRequired" ? fixture.Revision.ToString(CultureInfo.InvariantCulture) : "null";
            return "{\"role\":\"" + role + "\",\"fileName\":\"" + name + "\",\"presence\":\"Present\",\"classification\":\"" + fixture.ExpectedLeafClass + "\",\"byteLength\":" + fixture.Bytes.LongLength.ToString(CultureInfo.InvariantCulture) + ",\"sha256\":\"" + Hash(fixture.Bytes) + "\",\"decodedRevision\":" + revision + "}";
        }

        private static void AssertArchivedAndPrepared(string root, string id, Fixture[] fixtures)
        {
            var tx = Path.Combine(root, "recovery", "new-game", id);
            for (var i = 0; i < fixtures.Length; i++)
            {
                var archive = Path.Combine(tx, ArchiveNames[i]);
                if (fixtures[i].Kind == FixtureKind.Missing) Assert.That(File.Exists(archive), Is.False);
                else CollectionAssert.AreEqual(fixtures[i].Bytes, File.ReadAllBytes(archive));
            }
            CollectionAssert.AreEqual(ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(root, Names[0])));
            Assert.That(File.Exists(Path.Combine(root, Names[1])), Is.False);
            Assert.That(File.Exists(Path.Combine(root, Names[2])), Is.False);
        }

        public enum FixtureKind { Missing, ValidCurrent, ValidRecoveryMetadata, ValidRecoveryBinding, Unsupported, Invalid }

        private sealed class Fixture
        {
            internal readonly FixtureKind Kind;
            internal readonly byte[] Bytes;
            internal readonly long Revision;
            internal readonly ProfileCanonicalDecodeClassification ExpectedDecoderClass;
            internal readonly string ExpectedLeafClass;
            private Fixture(FixtureKind kind, byte[] bytes, long revision, ProfileCanonicalDecodeClassification decoderClass, string leafClass) { Kind = kind; Bytes = bytes; Revision = revision; ExpectedDecoderClass = decoderClass; ExpectedLeafClass = leafClass; }
            internal static Fixture Missing() { return new Fixture(FixtureKind.Missing, null, -1, default(ProfileCanonicalDecodeClassification), null); }
            internal static Fixture Current(long revision) { return new Fixture(FixtureKind.ValidCurrent, Document(revision).CanonicalFileBytes, revision, ProfileCanonicalDecodeClassification.ValidCurrentInput, "ValidCurrentInput"); }
            internal static Fixture RecoveryMetadata(long revision) { return new Fixture(FixtureKind.ValidRecoveryMetadata, RecoveryDocument(revision).CanonicalFileBytes, revision, ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired, "ValidInputRecoveryRequired"); }
            internal static Fixture RecoveryBinding(long revision) { return new Fixture(FixtureKind.ValidRecoveryBinding, MalformedBindingDocument(revision), revision, ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired, "ValidInputRecoveryRequired"); }
            internal static Fixture Unsupported() { return new Fixture(FixtureKind.Unsupported, ProfileResetDiskAdversarialV1Tests.Unsupported(), -1, ProfileCanonicalDecodeClassification.UnsupportedProfileSchema, "UnsupportedSchema"); }
            internal static Fixture Invalid(byte[] bytes) { return new Fixture(FixtureKind.Invalid, bytes, -1, ProfileCanonicalDecodeClassification.Invalid, "Invalid"); }
            internal static Fixture Create(FixtureKind kind, long revision)
            {
                switch (kind)
                {
                    case FixtureKind.Missing: return Missing();
                    case FixtureKind.ValidCurrent: return Current(revision);
                    case FixtureKind.ValidRecoveryMetadata: return RecoveryMetadata(revision);
                    case FixtureKind.ValidRecoveryBinding: return RecoveryBinding(revision);
                    case FixtureKind.Unsupported: return Unsupported();
                    case FixtureKind.Invalid: return Invalid(new byte[] { 0, 255, (byte)revision, 4 });
                    default: throw new ArgumentOutOfRangeException(nameof(kind));
                }
            }
        }

        private static void AssertResult(ProfileResetDiskResultV1 result, ProfileResetDiskOutcomeV1 outcome) { result.Validate(); Assert.That(result.Outcome, Is.EqualTo(outcome)); }
        private static byte[][] Snapshot(string root) { return new[] { Bytes(root, "profile.json"), Bytes(root, "profile.prev.json"), Bytes(root, "profile.tmp.json") }; }
        private static void AssertSnapshot(string root, byte[][] snapshot) { CollectionAssert.AreEqual(snapshot[0], Bytes(root, "profile.json")); CollectionAssert.AreEqual(snapshot[1], Bytes(root, "profile.prev.json")); CollectionAssert.AreEqual(snapshot[2], Bytes(root, "profile.tmp.json")); }
        private static byte[] Bytes(string root, string name) { var path = Path.Combine(root, name); return File.Exists(path) ? File.ReadAllBytes(path) : null; }
        private static void WriteLeaves(string root, byte[] primary, byte[] previous, byte[] temp) { File.WriteAllBytes(Path.Combine(root, "profile.json"), primary); File.WriteAllBytes(Path.Combine(root, "profile.prev.json"), previous); File.WriteAllBytes(Path.Combine(root, "profile.tmp.json"), temp); }
        private static byte[] Unsupported() { return Encoding.UTF8.GetBytes("{\"schemaVersion\":2}"); }
        private static string MoveVersionAfterTransaction(string text)
        {
            var withoutVersion = text.Replace("\"format\":\"AcadeGameMaker.ProfileResetBarrier\",\"version\":1,\"transactionId\":\"", "\"format\":\"AcadeGameMaker.ProfileResetBarrier\",\"transactionId\":\"");
            var end = withoutVersion.IndexOf("\",\"target\"", StringComparison.Ordinal);
            return end < 0 ? withoutVersion : withoutVersion.Insert(end + 1, ",\"version\":1");
        }
        private static string Rehash(string text) { var cut = text.LastIndexOf(",\"bodySha256\"", StringComparison.Ordinal); if (cut < 0) return text; var body = text.Substring(0, cut) + "}"; return text.Substring(0, cut) + ",\"bodySha256\":\"" + Hash(Encoding.UTF8.GetBytes(body)) + "\"}"; }
        private static string Hash(byte[] bytes) { using (var sha = SHA256.Create()) { var hash = sha.ComputeHash(bytes); var b = new StringBuilder(64); foreach (var x in hash) b.Append(x.ToString("x2")); return b.ToString(); } }
        private static ProfileCanonicalDocumentV1 Document(long revision) { return Encode(ProfileInputContract.CurrentInputActionsAssetId, revision); }
        private static ProfileCanonicalDocumentV1 RecoveryDocument(long revision) { return Encode("obsolete-input", revision); }
        private static byte[] MalformedBindingDocument(long revision)
        {
            var source = Document(revision); var payload = Encoding.UTF8.GetString(source.CanonicalPayloadBytes).Replace("\"bindingOverridesJson\":\"\"", "\"bindingOverridesJson\":\"{bad}\"");
            var payloadBytes = new UTF8Encoding(false).GetBytes(payload); var hash = Hash(payloadBytes);
            return new UTF8Encoding(false).GetBytes(payload.Substring(0, payload.Length - 1) + ",\"integrity\":{\"payloadSha256\":\"" + hash + "\"}}");
        }
        private static ProfileCanonicalDocumentV1 Encode(string inputId, long revision)
        {
            var settings = new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false);
            var input = new ProfileInputSnapshot(inputId, ProfileInputContract.CurrentBindingSchemaVersion, ProfileBindingOverridesJson.Parse(string.Empty));
            var snapshot = new ProfileSnapshotV1(settings, input, new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            return ProfileCanonicalEncoder.Encode(snapshot);
        }
        private static void WithRoot(Action<string> test)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C1-adversary-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            try { test(root); }
            finally { if (Directory.Exists(root)) { Directory.Delete(root, true); Assert.That(Directory.Exists(root), Is.False, "temporary root cleanup must be exact"); } }
        }
    }
}
