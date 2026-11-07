using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AcadeGameMaker.Costumes;
using AcadeGameMaker.Costumes.IO;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.CostumesIO
{
    // REQ-CIO-001..008; AC-CIO-001..006. Deterministic port traces cover CIO without starting Unity.
    public sealed class CostumeFileAdapterV1Tests
    {
        [Test]
        public void AC_CIO_001_InvalidRootsDriveRelativeFormsAndOwnedNamesFailBeforeAnyPortOperation()
        {
            foreach (var invalid in new[] { null, "relative", "C:relative", "C:.", "C:folder\\child", Path.GetPathRoot(Path.GetTempPath()), Path.Combine(Path.GetTempPath(), "..", "other") })
            {
                var port = new RecordingPort(); Assert.Throws<ArgumentException>(() => new CostumeFileAdapterV1(invalid, port)); Assert.That(port.Events, Is.Empty);
            }

            var root = Path.Combine(Path.GetTempPath(), "cio-test-root"); var valid = new RecordingPort(); NewAdapter(valid, root).Load(PendingCatalog());
            CollectionAssert.AreEquivalent(new[] { CostumeFileAdapterV1.PrimaryFileName, CostumeFileAdapterV1.PreviousFileName, CostumeFileAdapterV1.TempFileName }, valid.ReadLeaves);
            AssertAllPathsOwned(valid, root);
        }

        [Test]
        public void AC_CIO_002_ObservationsAreSingleReadDefensiveAndPathRejectedUsesNotObserved()
        {
            var port = new RecordingPort(); var bytes = CostumeCanonicalCodecV1.Encode(ValidState(3)); port.Reads[CostumeFileAdapterV1.PrimaryFileName] = Read(bytes); port.Reads[CostumeFileAdapterV1.PreviousFileName] = Missing(); port.Reads[CostumeFileAdapterV1.TempFileName] = Missing();
            var result = NewAdapter(port).Load(AcceptedCatalog());
            Assert.That(result.Outcome, Is.EqualTo(CostumeFileLoadOutcomeV1.Recovered)); Assert.That(result.RecoveryPlan.State.Revision, Is.EqualTo(3));
            Assert.That(port.ReadCount(CostumeFileAdapterV1.PrimaryFileName), Is.EqualTo(1)); Assert.That(port.ReadCount(CostumeFileAdapterV1.PreviousFileName), Is.EqualTo(1)); Assert.That(port.ReadCount(CostumeFileAdapterV1.TempFileName), Is.EqualTo(1));
            var returned = result.Primary.Bytes; returned[0] ^= 1; Assert.That(result.Primary.Bytes, Is.EqualTo(bytes));

            var rejectedPort = new RecordingPort { RootIsSafe = false }; var rejected = NewAdapter(rejectedPort).Load(AcceptedCatalog());
            Assert.That(rejected.Outcome, Is.EqualTo(CostumeFileLoadOutcomeV1.PathRejected)); Assert.That(rejected.Primary.Disposition, Is.EqualTo(CostumeFileReadDispositionV1.NotObserved)); Assert.That(rejected.Previous.Disposition, Is.EqualTo(CostumeFileReadDispositionV1.NotObserved)); Assert.That(rejected.Temp.Disposition, Is.EqualTo(CostumeFileReadDispositionV1.NotObserved)); Assert.That(rejectedPort.ReadLeaves, Is.Empty);
        }

        [Test]
        public void AC_CIO_002_CorruptAndUnreadPermutationsPreserveSelectorPrecedenceWithoutMerge()
        {
            var corruptPrimary = new RecordingPort(); corruptPrimary.Reads[CostumeFileAdapterV1.PrimaryFileName] = Read(new byte[] { 1, 2, 3 }); corruptPrimary.Reads[CostumeFileAdapterV1.PreviousFileName] = Read(CostumeCanonicalCodecV1.Encode(ValidState(8))); corruptPrimary.Reads[CostumeFileAdapterV1.TempFileName] = Read(CostumeCanonicalCodecV1.Encode(ValidState(99)));
            var selected = NewAdapter(corruptPrimary).Load(AcceptedCatalog()); Assert.That(selected.Outcome, Is.EqualTo(CostumeFileLoadOutcomeV1.Recovered)); Assert.That(selected.RecoveryPlan.State.Revision, Is.EqualTo(8));

            var unreadPrimary = new RecordingPort(); unreadPrimary.Reads[CostumeFileAdapterV1.PrimaryFileName] = Failed(); unreadPrimary.Reads[CostumeFileAdapterV1.PreviousFileName] = Read(CostumeCanonicalCodecV1.Encode(ValidState(8))); unreadPrimary.Reads[CostumeFileAdapterV1.TempFileName] = Missing();
            Assert.That(NewAdapter(unreadPrimary).Load(AcceptedCatalog()).Outcome, Is.EqualTo(CostumeFileLoadOutcomeV1.ReadFailure));
            var validPrimary = new RecordingPort(); validPrimary.Reads[CostumeFileAdapterV1.PrimaryFileName] = Read(CostumeCanonicalCodecV1.Encode(ValidState(8))); validPrimary.Reads[CostumeFileAdapterV1.PreviousFileName] = Failed(); validPrimary.Reads[CostumeFileAdapterV1.TempFileName] = Failed();
            Assert.That(NewAdapter(validPrimary).Load(AcceptedCatalog()).Outcome, Is.EqualTo(CostumeFileLoadOutcomeV1.Recovered));
        }

        [Test]
        public void AC_CIO_002_FaultyCustomPortReadProtocolCannotBootstrapOrPublishNotObserved()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.NotObserved, null, 0));
            foreach (var raw in new[] { default(CostumeFilePortReadResultV1), CorruptPortDisposition(CostumeFileReadDispositionV1.NotObserved), CorruptPortDisposition((CostumeFileReadDispositionV1)99) })
            {
                var port = new RecordingPort { HasRawOverride = true, RawOverride = raw }; var result = NewAdapter(port).Load(AcceptedCatalog());
                Assert.That(result.Outcome, Is.EqualTo(CostumeFileLoadOutcomeV1.ReadFailure)); Assert.That(result.HasRecoveryPlan, Is.False); Assert.That(result.Primary.Disposition, Is.EqualTo(CostumeFileReadDispositionV1.ReadFailed)); Assert.That(port.ReadCount(CostumeFileAdapterV1.PrimaryFileName), Is.EqualTo(1));
            }
        }

        [Test]
        public void AC_CIO_003_RecordingPortProvesExactFirstAndReplacementTransactions()
        {
            var firstPort = new RecordingPort(); var first = NewAdapter(firstPort).Save(ValidState(5));
            Assert.That(first.Outcome, Is.EqualTo(CostumeFileSaveOutcomeV1.CommittedFirst)); Assert.That(first.ExpectedRevision, Is.EqualTo(5));
            AssertTrace(firstPort, "root", "stage:Validation", "inspect:costume-state-v1.json", "inspect:costume-state-v1.previous.json", "inspect:costume-state-v1.temp.json", "stage:TempWrite", "open:costume-state-v1.temp.json", "write", "stage:ManagedFlush", "managed", "stage:DurableFlush", "durable", "stage:TempClose", "close", "stage:AtomicMove", "move", "stage:PrimaryReopen", "read:costume-state-v1.json", "stage:PrimaryEquality", "stage:PrimaryDecode");
            AssertAllPathsOwned(firstPort, Path.Combine(Path.GetTempPath(), "cio-test-root"));

            var replacementPort = new RecordingPort { PrimaryExists = true }; var replacement = NewAdapter(replacementPort).Save(ValidState(6));
            Assert.That(replacement.Outcome, Is.EqualTo(CostumeFileSaveOutcomeV1.CommittedReplacement)); Assert.That(replacement.ExpectedRevision, Is.EqualTo(6));
            AssertTrace(replacementPort, "root", "stage:Validation", "inspect:costume-state-v1.json", "inspect:costume-state-v1.previous.json", "inspect:costume-state-v1.temp.json", "stage:TempWrite", "open:costume-state-v1.temp.json", "write", "stage:ManagedFlush", "managed", "stage:DurableFlush", "durable", "stage:TempClose", "close", "stage:AtomicReplace", "replace", "stage:PrimaryReopen", "read:costume-state-v1.json", "stage:PrimaryEquality", "stage:PrimaryDecode");
            AssertAllPathsOwned(replacementPort, Path.Combine(Path.GetTempPath(), "cio-test-root"));
        }

        [Test]
        public void AC_CIO_004_FullFaultMatrixHasTypedStageNoRetryNoCleanupAndExactOwnedPaths()
        {
            AssertFault("inspect:costume-state-v1.json", false, CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.Validation);
            AssertFault("inspect:costume-state-v1.previous.json", false, CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.Validation);
            AssertFault("inspect:costume-state-v1.temp.json", false, CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.Validation);
            AssertFault("write", false, CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.TempWrite);
            AssertFault("managed", false, CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.ManagedFlush);
            AssertFault("durable", false, CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.DurableFlush);
            AssertFault("close", false, CostumeFileSaveOutcomeV1.FailedBeforeCommit, CostumeFileSaveStageV1.TempClose);
            AssertFault("move", false, CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.AtomicMove);
            AssertFault("replace", true, CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.AtomicReplace);
            AssertFault("reopen", false, CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.PrimaryReopen);
            AssertFault("equality", false, CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.PrimaryEquality);
            AssertFault("decode", false, CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.PrimaryDecode);
        }

        [Test]
        public void AC_CIO_005_AllPendingMissingCorruptAndValidEmptyStatesNeverFabricateBindingOrSelection()
        {
            var corrupt = new RecordingPort(); corrupt.Reads[CostumeFileAdapterV1.PrimaryFileName] = Missing(); corrupt.Reads[CostumeFileAdapterV1.PreviousFileName] = Read(new byte[] { 8 }); corrupt.Reads[CostumeFileAdapterV1.TempFileName] = Missing(); AssertNoFabrication(NewAdapter(corrupt).Load(PendingCatalog()), corrupt);
            var empty = new RecordingPort(); empty.Reads[CostumeFileAdapterV1.PrimaryFileName] = Read(CostumeCanonicalCodecV1.Encode(new CostumeStateV1(7, new string[0], new KeyValuePair<string, string>[0]))); empty.Reads[CostumeFileAdapterV1.PreviousFileName] = Missing(); empty.Reads[CostumeFileAdapterV1.TempFileName] = Missing(); AssertNoFabrication(NewAdapter(empty).Load(PendingCatalog()), empty);
        }

        [Test]
        public void AC_CIO_003_ActualLocalNtfsSmokeKeepsPrimaryAndPreviousCanonical()
        {
            var root = Path.Combine(Path.GetTempPath(), "acade-costume-cio-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            try
            {
                var adapter = new CostumeFileAdapterV1(root); var first = adapter.Save(ValidState(5)); var replacement = adapter.Save(ValidState(6));
                Assert.That(first.Outcome, Is.EqualTo(CostumeFileSaveOutcomeV1.CommittedFirst)); Assert.That(replacement.Outcome, Is.EqualTo(CostumeFileSaveOutcomeV1.CommittedReplacement)); CostumeStateV1 primary; CostumeStateV1 previous;
                Assert.That(CostumeCanonicalCodecV1.TryDecode(File.ReadAllBytes(Path.Combine(root, CostumeFileAdapterV1.PrimaryFileName)), out primary), Is.True); Assert.That(CostumeCanonicalCodecV1.TryDecode(File.ReadAllBytes(Path.Combine(root, CostumeFileAdapterV1.PreviousFileName)), out previous), Is.True); Assert.That(primary.Revision, Is.EqualTo(6)); Assert.That(previous.Revision, Is.EqualTo(5));
            }
            finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }

        [Test]
        public void AC_CIO_006_StaticBoundaryContainsNoForbiddenAuthority()
        {
            var root = FindRoot(); var source = File.ReadAllText(Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Costumes", "IO", "CostumeFileAdapterV1.cs")); StringAssert.Contains("REQ-CIO-001..008", source);
            foreach (var forbidden in new[] { "UnityEngine", "persistentDataPath", "System.Net", "DateTime", "Random", "Quarantine", "Profile", "AcadeGameMaker.Run", "File.Delete", "Directory.Delete" }) StringAssert.DoesNotContain(forbidden, source);
        }

        private static void AssertFault(string fault, bool replacement, CostumeFileSaveOutcomeV1 outcome, CostumeFileSaveStageV1 stage)
        {
            var port = new RecordingPort { Fault = fault, PrimaryExists = replacement }; var result = NewAdapter(port).Save(ValidState(1));
            Assert.That(result.Outcome, Is.EqualTo(outcome), fault); Assert.That(result.Stage, Is.EqualTo(stage), fault); Assert.That(port.AtomicCalls, Is.EqualTo(outcome == CostumeFileSaveOutcomeV1.FailedBeforeCommit ? 0 : 1), fault); Assert.That(port.DeleteCalls, Is.EqualTo(0), fault); Assert.That(port.OpenWriteCalls, Is.EqualTo(fault.StartsWith("inspect:", StringComparison.Ordinal) ? 0 : 1), fault); AssertAllPathsOwned(port, Path.Combine(Path.GetTempPath(), "cio-test-root"));
        }

        private static void AssertNoFabrication(CostumeFileLoadResultV1 result, RecordingPort port)
        {
            Assert.That(result.Outcome, Is.EqualTo(CostumeFileLoadOutcomeV1.Recovered)); Assert.That(result.RecoveryPlan.Reason, Is.EqualTo(CostumeRecoveryReason.NoAcceptedDefault)); Assert.That(result.RecoveryPlan.HasBinding, Is.False); Assert.That(result.RecoveryPlan.State.UnlockedCostumeIds, Is.Empty); Assert.That(result.RecoveryPlan.State.CurrentByActor, Is.Empty); Assert.That(port.OpenWriteCalls, Is.EqualTo(0)); Assert.That(port.AtomicCalls, Is.EqualTo(0)); Assert.That(port.DeleteCalls, Is.EqualTo(0));
        }

        private static void AssertTrace(RecordingPort port, params string[] expected) { CollectionAssert.AreEqual(expected, port.Events); }
        private static void AssertAllPathsOwned(RecordingPort port, string root) { var fullRoot = Path.GetFullPath(root); foreach (var path in port.Paths) { Assert.That(StringComparer.OrdinalIgnoreCase.Equals(Path.GetDirectoryName(path), fullRoot), Is.True, path); CollectionAssert.Contains(new[] { CostumeFileAdapterV1.PrimaryFileName, CostumeFileAdapterV1.PreviousFileName, CostumeFileAdapterV1.TempFileName }, Path.GetFileName(path)); } }
        private static CostumeFileAdapterV1 NewAdapter(RecordingPort port) => NewAdapter(port, Path.Combine(Path.GetTempPath(), "cio-test-root"));
        private static CostumeFileAdapterV1 NewAdapter(RecordingPort port, string root) => new CostumeFileAdapterV1(root, port);
        private static CostumeFilePortReadResultV1 Missing() => new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.Missing, null, 0);
        private static CostumeFilePortReadResultV1 Failed() => new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.ReadFailed, null, 0);
        private static CostumeFilePortReadResultV1 Read(byte[] bytes) => new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.Read, bytes, 0);
        private static CostumeCatalogV1 PendingCatalog() => CostumeCatalogV1.SeryeongInventoryPending();
        private static CostumeCatalogV1 AcceptedCatalog() => new CostumeCatalogV1(1, new[] { new CostumeDefinitionV1("seryeong", "seryeong.costume.story.v1", "story", Hash(), "hair", CostumeDisposition.Accepted, true, "portrait", "gameplay", Hash(), Hash(), Hash(), 1) });
        private static CostumeStateV1 ValidState(long revision) => new CostumeStateV1(revision, new[] { "seryeong.costume.story.v1" }, new[] { new KeyValuePair<string, string>("seryeong", "seryeong.costume.story.v1") });
        private static string Hash() => new string('a', 64);
        private static CostumeFilePortReadResultV1 CorruptPortDisposition(CostumeFileReadDispositionV1 disposition) { object boxed = Missing(); typeof(CostumeFilePortReadResultV1).GetField("<Disposition>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, disposition); return (CostumeFilePortReadResultV1)boxed; }
        private static string FindRoot() { var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory); while (current != null) { if (File.Exists(Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; } throw new DirectoryNotFoundException(); }

        private sealed class RecordingPort : ICostumeFilePortV1, ICostumeFileTransactionObserverV1
        {
            internal readonly Dictionary<string, CostumeFilePortReadResultV1> Reads = new Dictionary<string, CostumeFilePortReadResultV1>(StringComparer.OrdinalIgnoreCase);
            internal readonly List<string> Events = new List<string>(); internal readonly List<string> Paths = new List<string>(); internal readonly List<string> ReadLeaves = new List<string>(); internal bool RootIsSafe = true; internal bool PrimaryExists; internal string Fault; internal bool HasRawOverride; internal CostumeFilePortReadResultV1 RawOverride; internal int OpenWriteCalls; internal int AtomicCalls; internal int DeleteCalls = 0; private byte[] _written; private byte[] _primary;

            public bool IsExistingLocalNtfsDirectoryWithoutReparseAncestor(string normalizedDirectory) { Events.Add("root"); return RootIsSafe; }
            public void RecordStage(CostumeFileSaveStageV1 stage) { Events.Add("stage:" + stage); if (Fault == "decode" && stage == CostumeFileSaveStageV1.PrimaryDecode) throw new IOException("decode fault"); }
            public CostumeFilePortReadResultV1 ReadOnce(string fullPath)
            {
                Paths.Add(fullPath); var leaf = Path.GetFileName(fullPath); ReadLeaves.Add(leaf); Events.Add("read:" + leaf);
                if (Fault == "reopen" && AtomicCalls > 0 && leaf == CostumeFileAdapterV1.PrimaryFileName) throw new IOException("reopen fault");
                if (AtomicCalls > 0 && leaf == CostumeFileAdapterV1.PrimaryFileName) return Fault == "equality" ? Read(new byte[] { 1 }) : Read(_primary);
                if (HasRawOverride) return RawOverride;
                CostumeFilePortReadResultV1 result; return Reads.TryGetValue(leaf, out result) ? result : Missing();
            }
            public CostumeFilePathInfoV1 Inspect(string fullPath)
            {
                Paths.Add(fullPath); var leaf = Path.GetFileName(fullPath); Events.Add("inspect:" + leaf); if (Fault == "inspect:" + leaf) throw new ArgumentException("inspection fault");
                return new CostumeFilePathInfoV1(leaf == CostumeFileAdapterV1.PrimaryFileName && PrimaryExists ? CostumeFilePathKindV1.File : CostumeFilePathKindV1.Missing, 0);
            }
            public ICostumeFileWriteSessionV1 OpenTempForWrite(string fullPath) { Paths.Add(fullPath); Events.Add("open:" + Path.GetFileName(fullPath)); OpenWriteCalls++; return new RecordingSession(this); }
            public void MoveNoOverwrite(string sourcePath, string destinationPath) { Paths.Add(sourcePath); Paths.Add(destinationPath); Events.Add("move"); AtomicCalls++; if (Fault == "move") throw new IOException("move fault"); _primary = Copy(_written); PrimaryExists = true; }
            public void Replace(string sourcePath, string destinationPath, string backupPath) { Paths.Add(sourcePath); Paths.Add(destinationPath); Paths.Add(backupPath); Events.Add("replace"); AtomicCalls++; if (Fault == "replace") throw new IOException("replace fault"); _primary = Copy(_written); PrimaryExists = true; }
            internal void Write(byte[] bytes) { Events.Add("write"); if (Fault == "write") throw new IOException("write fault"); _written = Copy(bytes); }
            internal void FlushManaged() { Events.Add("managed"); if (Fault == "managed") throw new IOException("managed fault"); }
            internal void FlushDurable() { Events.Add("durable"); if (Fault == "durable") throw new IOException("durable fault"); }
            internal void Close() { Events.Add("close"); if (Fault == "close") throw new IOException("close fault"); }
            internal int ReadCount(string leaf) { var count = 0; foreach (var item in ReadLeaves) if (item == leaf) count++; return count; }
            private static byte[] Copy(byte[] bytes) { return bytes == null ? null : (byte[])bytes.Clone(); }

            private sealed class RecordingSession : ICostumeFileWriteSessionV1
            {
                private readonly RecordingPort _owner; internal RecordingSession(RecordingPort owner) { _owner = owner; }
                public void WriteExact(byte[] bytes) { _owner.Write(bytes); }
                public void FlushManaged() { _owner.FlushManaged(); }
                public void FlushDurable() { _owner.FlushDurable(); }
                public void Dispose() { _owner.Close(); }
            }
        }
    }
}
