using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace AcadeGameMaker.Profile.Tests
{
    // REQ-M5D7QC1-002..007; AC-M5D7QC1-003/006/008/009. Each case kills a
    // real child executing the compiled Profile DLL, distinct from in-process seams.
    public sealed class ProfileResetDiskProcessV1Tests
    {
        private static string _workerArtifacts;
        private static string _workerDll;

        private static IEnumerable AllCheckpointNames
        {
            get
            {
                foreach (ProfileResetDiskCheckpointV1 point in Enum.GetValues(typeof(ProfileResetDiskCheckpointV1))) yield return point.ToString();
            }
        }

        [OneTimeSetUp]
        public void BuildWorkerOnce()
        {
            _workerArtifacts = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C1-worker-" + Guid.NewGuid().ToString("N"));
            _workerDll = BuildWorker(_workerArtifacts);
        }

        [OneTimeTearDown]
        public void RemoveWorkerArtifacts()
        {
            if (!string.IsNullOrEmpty(_workerArtifacts) && Directory.Exists(_workerArtifacts) && IsOwnedTemp(_workerArtifacts)) Directory.Delete(_workerArtifacts, true);
        }

        [TestCaseSource(nameof(AllCheckpointNames))]
        public void AC003_AC006_AC008_ActualWorkerCrashAtEveryCheckpointRestartsSafely(string checkpointName)
        {
            var checkpoint = (ProfileResetDiskCheckpointV1)Enum.Parse(typeof(ProfileResetDiskCheckpointV1), checkpointName, false);
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C1-process-" + Guid.NewGuid().ToString("N"));
            var signal = root + ".profile-reset-crash-ready.signal";
            Process worker = null;
            Process resume = null;
            try
            {
                Directory.CreateDirectory(root);
                byte[] primary, previous, temp;
                SeedCandidates(root, out primary, out previous, out temp);
                byte[] interrupted = null;
                string mode;
                if ((int)checkpoint >= 25 && (int)checkpoint <= 32)
                {
                    Assert.That(ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root)).Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
                    VerifyArchive(root, primary, previous, temp);
                    interrupted = (int)checkpoint <= 28 ? new byte[] { 81, 9, 1 } : new byte[] { 82, 9, 2 };
                    File.WriteAllBytes(Path.Combine(root, (int)checkpoint <= 28 ? "profile.tmp.json" : "profile.json"), interrupted);
                    mode = "resume";
                }
                else if ((int)checkpoint >= 13 && (int)checkpoint <= 24) { PublishMarker(root); mode = "resume"; }
                else mode = "begin";

                worker = StartWorker(_workerDll, root, checkpointName, signal, mode);
                WaitForReady(signal, worker.Id, checkpointName);
                AssertBusyAndUnchanged(root, checkpoint);
                worker.Kill();
                Assert.That(worker.WaitForExit(5000), Is.True, "exact worker child must exit after kill");
                worker.Dispose(); worker = null;

                if ((int)checkpoint <= 9)
                {
                    Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.False);
                    AssertActiveOldBytes(root, primary, previous, temp);
                    AssertNoArchiveFiles(root);
                }
                else Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.True);

                DeleteOwnedSignal(signal, root);
                resume = StartWorker(_workerDll, root, null, signal, "resume");
                Assert.That(resume.WaitForExit(15000), Is.True, "normal restart worker timed out");
                var output = resume.StandardOutput.ReadToEnd();
                var error = resume.StandardError.ReadToEnd();
                Assert.That(resume.ExitCode, Is.EqualTo(0), error);
                Assert.That(output, Does.Contain("OUTCOME=" + ((int)checkpoint <= 9 ? "ManualRepairRequired" : "DiskPrepared")), error);
                if ((int)checkpoint <= 9)
                {
                    AssertActiveOldBytes(root, primary, previous, temp);
                    AssertNoArchiveFiles(root);
                }
                else
                {
                    VerifyPrepared(root, primary, previous, temp);
                    if (interrupted != null) VerifyInterruptedPreserved(root, interrupted, (int)checkpoint <= 28);
                }
            }
            finally
            {
                var cleanupFailures = new List<string>();
                TryKillOwned(resume, cleanupFailures); TryKillOwned(worker, cleanupFailures);
                try { DeleteOwnedSignal(signal, root); } catch (Exception e) { cleanupFailures.Add("signal cleanup: " + e.Message); }
                try { if (Directory.Exists(root) && IsOwnedTemp(root)) Directory.Delete(root, true); } catch (Exception e) { cleanupFailures.Add("root cleanup: " + e.Message); }
                if (cleanupFailures.Count != 0) Assert.Fail(string.Join(" | ", cleanupFailures));
            }
        }

        private static void PublishMarker(string root)
        {
            var result = ProfileResetDiskTransactionV1.Begin(root, ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root), new StopAfterPublishedMarker());
            Assert.That(result.Outcome, Is.EqualTo(ProfileResetDiskOutcomeV1.ManualRepairRequired));
            Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.True);
        }

        private static string BuildWorker(string artifacts)
        {
            Directory.CreateDirectory(artifacts);
            var project = Path.GetFullPath(Path.Combine("qa", "fixtures", "ProfileResetCrashWorker", "ProfileResetCrashWorker.csproj"));
            var obj = Path.Combine(artifacts, "obj").Replace('\\', '/') + "/";
            var bin = Path.Combine(artifacts, "bin").Replace('\\', '/') + "/";
            var arguments = "build \"" + project + "\" --nologo -p:RestoreSources= -p:BaseIntermediateOutputPath=\"" + obj + "\" -p:MSBuildProjectExtensionsPath=\"" + obj + "\" -p:BaseOutputPath=\"" + bin + "\"";
            var diagnostics = new StringBuilder(); var diagnosticLock = new object();
            Process compiler = null;
            try
            {
                compiler = Process.Start(new ProcessStartInfo(@"C:\Program Files\dotnet\dotnet.exe", arguments) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardError = true, RedirectStandardOutput = true });
                compiler.OutputDataReceived += (sender, e) => { if (e.Data != null) lock (diagnosticLock) diagnostics.AppendLine(e.Data); };
                compiler.ErrorDataReceived += (sender, e) => { if (e.Data != null) lock (diagnosticLock) diagnostics.AppendLine(e.Data); };
                compiler.BeginOutputReadLine(); compiler.BeginErrorReadLine();
                var exited = compiler.WaitForExit(30000);
                if (!exited) { compiler.Kill(); Assert.That(compiler.WaitForExit(5000), Is.True, "exact worker compiler did not exit after timeout kill"); }
                compiler.WaitForExit();
                Assert.That(exited, Is.True, "worker compiler timed out\n" + diagnostics);
                lock (diagnosticLock) Assert.That(compiler.ExitCode, Is.EqualTo(0), diagnostics.ToString());
            }
            finally
            {
                if (compiler != null)
                {
                    if (!compiler.HasExited) { compiler.Kill(); Assert.That(compiler.WaitForExit(5000), Is.True, "exact worker compiler did not exit during cleanup"); }
                    compiler.Dispose();
                }
            }
            var result = Path.Combine(artifacts, "bin", "Debug", "net10.0", "ProfileResetCrashWorker.dll");
            Assert.That(File.Exists(result), Is.True, "worker build did not produce the expected DLL");
            return result;
        }

        private static Process StartWorker(string worker, string root, string checkpoint, string signal, string mode)
        {
            var assembly = Path.GetFullPath(Path.Combine("Library", "ScriptAssemblies", "AcadeGameMaker.Profile.dll"));
            var arguments = "\"" + worker + "\" --assembly \"" + assembly + "\" --root \"" + root + "\" --mode " + mode + " --signal \"" + signal + "\"" + (checkpoint == null ? "" : " --checkpoint " + checkpoint);
            return Process.Start(new ProcessStartInfo(@"C:\Program Files\dotnet\dotnet.exe", arguments) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true });
        }

        private static void WaitForReady(string signal, int pid, string checkpoint)
        {
            var until = DateTime.UtcNow.AddSeconds(10); while (!File.Exists(signal) && DateTime.UtcNow < until) System.Threading.Thread.Sleep(25);
            Assert.That(File.Exists(signal), Is.True, "worker signal timeout");
            string text = null;
            while (DateTime.UtcNow < until) { try { text = File.ReadAllText(signal); break; } catch (IOException) { System.Threading.Thread.Sleep(25); } }
            Assert.That(text, Is.EqualTo(pid + "|" + checkpoint), "ready signal must bind the exact child and checkpoint");
        }

        private static void AssertBusyAndUnchanged(string root, ProfileResetDiskCheckpointV1 checkpoint)
        {
            var before = Snapshot(root, checkpoint); var watch = Stopwatch.StartNew();
            Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileRootOperationLockV1.Acquire(root));
            Assert.That(watch.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(4.75)).And.LessThanOrEqualTo(TimeSpan.FromSeconds(5.75)));
            CollectionAssert.AreEquivalent(before, Snapshot(root, checkpoint));
        }

        private static void SeedCandidates(string root, out byte[] primary, out byte[] previous, out byte[] temp)
        {
            primary = new byte[] { 0, 255, 7 }; previous = Encoding.UTF8.GetBytes("{\"schemaVersion\":2}"); temp = ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes;
            File.WriteAllBytes(Path.Combine(root, "profile.json"), primary); File.WriteAllBytes(Path.Combine(root, "profile.prev.json"), previous); File.WriteAllBytes(Path.Combine(root, "profile.tmp.json"), temp);
        }
        private static void AssertActiveOldBytes(string root, byte[] primary, byte[] previous, byte[] temp)
        {
            CollectionAssert.AreEqual(primary, File.ReadAllBytes(Path.Combine(root, "profile.json"))); CollectionAssert.AreEqual(previous, File.ReadAllBytes(Path.Combine(root, "profile.prev.json"))); CollectionAssert.AreEqual(temp, File.ReadAllBytes(Path.Combine(root, "profile.tmp.json")));
        }
        private static void VerifyPrepared(string root, byte[] primary, byte[] previous, byte[] temp)
        {
            Assert.That(File.Exists(Path.Combine(root, "profile.reset.json")), Is.True, "prepared state retains its barrier");
            CollectionAssert.AreEqual(ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes, File.ReadAllBytes(Path.Combine(root, "profile.json")));
            Assert.That(File.Exists(Path.Combine(root, "profile.prev.json")), Is.False); Assert.That(File.Exists(Path.Combine(root, "profile.tmp.json")), Is.False); VerifyArchive(root, primary, previous, temp);
        }
        private static void VerifyArchive(string root, byte[] primary, byte[] previous, byte[] temp)
        {
            var directories = Directory.GetDirectories(Path.Combine(root, "recovery", "new-game")); Assert.That(directories, Has.Length.EqualTo(1));
            CollectionAssert.AreEqual(primary, File.ReadAllBytes(Path.Combine(directories[0], "primary.bin"))); CollectionAssert.AreEqual(previous, File.ReadAllBytes(Path.Combine(directories[0], "previous.bin"))); CollectionAssert.AreEqual(temp, File.ReadAllBytes(Path.Combine(directories[0], "temp.bin")));
        }
        private static void VerifyInterruptedPreserved(string root, byte[] bytes, bool temp)
        {
            var name = Hash(bytes).ToLowerInvariant() + "-" + bytes.LongLength + ".bin";
            var transaction = Directory.GetDirectories(Path.Combine(root, "recovery", "new-game")); Assert.That(transaction, Has.Length.EqualTo(1));
            CollectionAssert.AreEqual(bytes, File.ReadAllBytes(Path.Combine(transaction[0], temp ? "interrupted-default-temp" : "interrupted-default-primary", name)));
        }
        private static void AssertNoArchiveFiles(string root)
        {
            var archive = Path.Combine(root, "recovery", "new-game"); if (Directory.Exists(archive)) Assert.That(Directory.GetFiles(archive, "*.bin", SearchOption.AllDirectories), Is.Empty);
        }

        private static string[] Snapshot(string root, ProfileResetDiskCheckpointV1 checkpoint)
        {
            var files = Directory.GetFiles(root, "*", SearchOption.AllDirectories); Array.Sort(files, StringComparer.Ordinal); var rows = new List<string>();
            foreach (var file in files)
            {
                var name = Path.GetFileName(file); var relative = file.Substring(root.Length);
                if (string.Equals(name, "profile.operation.lock", StringComparison.Ordinal)) { rows.Add(relative + ":LOCK"); continue; }
                try { rows.Add(relative + ":" + Hash(File.ReadAllBytes(file))); }
                catch (IOException) when (CanBeExclusivelyOpen(checkpoint, name)) { rows.Add(relative + ":LOCKED:" + new FileInfo(file).Length); }
            }
            return rows.ToArray();
        }
        private static bool CanBeExclusivelyOpen(ProfileResetDiskCheckpointV1 checkpoint, string name)
        {
            return (string.Equals(name, "profile.reset.tmp.json", StringComparison.Ordinal) && (int)checkpoint >= 1 && (int)checkpoint <= 9) || (string.Equals(name, "profile.tmp.json", StringComparison.Ordinal) && (((int)checkpoint >= 34 && (int)checkpoint <= 41) || ((int)checkpoint >= 50 && (int)checkpoint <= 51)));
        }
        private static string Hash(byte[] bytes) { using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", ""); }
        private static bool IsOwnedTemp(string path)
        {
            var temp = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var full = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return full.StartsWith(temp, StringComparison.OrdinalIgnoreCase) && !new DirectoryInfo(full).Attributes.HasFlag(FileAttributes.ReparsePoint);
        }
        private static void DeleteOwnedSignal(string signal, string root)
        {
            if (!string.Equals(signal, root + ".profile-reset-crash-ready.signal", StringComparison.OrdinalIgnoreCase) || !IsOwnedTemp(root)) throw new InvalidOperationException("Unexpected fixture signal.");
            if (File.Exists(signal)) File.Delete(signal);
        }
        private static void TryKillOwned(Process process, List<string> failures)
        {
            if (process == null) return;
            try
            {
                if (!process.HasExited) { process.Kill(); if (!process.WaitForExit(5000)) failures.Add("owned child did not exit"); }
            }
            catch (Exception e) { failures.Add("owned child cleanup: " + e.Message); }
            finally { process.Dispose(); }
        }
        private sealed class StopAfterPublishedMarker : IProfileResetDiskTestControlV1
        {
            public void Checkpoint(ProfileResetDiskCheckpointV1 checkpoint) { if (checkpoint == ProfileResetDiskCheckpointV1.AfterMarkerCommittedReopen) throw new IOException("Stop after actual marker publication."); }
        }
    }
}
