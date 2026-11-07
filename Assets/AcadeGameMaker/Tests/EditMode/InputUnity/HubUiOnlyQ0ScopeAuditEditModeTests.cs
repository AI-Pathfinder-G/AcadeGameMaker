using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.InputUnity
{
    // REQ-M5D7Q0-001/009/010 / AC-M5D7Q0-011.  This is deliberately a static
    // audit: it never builds a prefab, invokes an authoring menu, or starts Unity
    // lifecycle code.  The allowlists make metadata scope explicit rather than
    // treating every Unity-generated folder meta as harmless.
    internal sealed class HubUiOnlyQ0ScopeAuditEditModeTests
    {
        private static readonly HashSet<string> AllowedAmended = Set(
            "Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs",
            "Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouterDiagnostic.cs",
            "Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs",
            "Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs",
            "Assets/AcadeGameMaker/Runtime/Input/Unity/AssemblyInfo.cs",
            "Assets/AcadeGameMaker/Tests/EditMode/InputUnity/AcadeGameMaker.Input.Unity.EditMode.Tests.asmdef");

        private static readonly HashSet<string> AllowedNew = Set(
            "Assets/AcadeGameMaker/Editor/HubAuthoring/AcadeGameMaker.Hub.Authoring.Editor.asmdef",
            "Assets/AcadeGameMaker/Editor/HubAuthoring/AssemblyInfo.cs",
            "Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringBuilder.cs",
            "Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringValidator.cs",
            "Assets/Prefabs/Hub/HubRuntimeRoot.prefab",
            "Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs",
            "Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubRuntimeAuthoringBuilderEditModeTests.cs",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyInputRouterPlayModeTests.cs",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0CoverageBPlayModeTests.cs",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0HandoffMatrixPlayModeTests.cs",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0RemainingRuntimePlayModeTests.cs",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0TestSupport.cs");

        private static readonly HashSet<string> AllowedMeta = Set(
            "Assets/AcadeGameMaker/Editor/HubAuthoring.meta",
            "Assets/AcadeGameMaker/Editor/HubAuthoring/AcadeGameMaker.Hub.Authoring.Editor.asmdef.meta",
            "Assets/AcadeGameMaker/Editor/HubAuthoring/AssemblyInfo.cs.meta",
            "Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringBuilder.cs.meta",
            "Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringValidator.cs.meta",
            "Assets/Prefabs/Hub.meta",
            "Assets/Prefabs/Hub/HubRuntimeRoot.prefab.meta",
            "Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs.meta",
            "Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubRuntimeAuthoringBuilderEditModeTests.cs.meta",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyInputRouterPlayModeTests.cs.meta",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0CoverageBPlayModeTests.cs.meta",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0HandoffMatrixPlayModeTests.cs.meta",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0RemainingRuntimePlayModeTests.cs.meta",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0TestSupport.cs.meta");

        private static readonly HashSet<string> AllowedDocumentation = Set(
            "docs/proposals/2026-09-20-vd09-m5d7q0-hub-ui-only-router-graph.md",
            "docs/specs/work-contracts/2026-09-20-vd09-m5d7q0-hub-ui-only-router-graph.md",
            "docs/verification/2026-09-20-vd09-m5d7q0-luna-pregate.md",
            "docs/verification/2026-09-20-vd09-m5d7q0-implementation-evidence.md",
            "docs/verification/2026-09-22-vd09-m5d7q0-luna-independent-review.md",
            "docs/verification/2026-09-23-vd09-m5d7q0-luna-independent-review.md");

        [Test]
        public void AC011_ActualRepositoryManifestClassifiesTheExactQ0DeltaWithoutFolderInference()
        {
            // Read Git's actual changed + untracked inventory.  Unlike the
            // retired disk-name heuristic, there is no Hub folder scan, Q0
            // filename prefix, documentation glob, or synthetic before/after
            // sample here.  Q0 attribution is only an exact declared path.
            var root = Directory.GetParent(Application.dataPath).FullName;
            var actual = CurrentRepositoryChangedOrUntrackedPaths(root).ToArray();
            var q0 = actual.Where(DeclaredQ0Paths.Contains).OrderBy(path => path, StringComparer.Ordinal).ToArray();
            var externallyDeclaredPreExisting = actual.Where(path => !DeclaredQ0Paths.Contains(path)).OrderBy(path => path, StringComparer.Ordinal).ToArray();

            foreach (var path in q0) TestContext.WriteLine("Q0_EXACT\t" + path);
            foreach (var path in externallyDeclaredPreExisting) TestContext.WriteLine("PREEXISTING_DECLARED_EXTERNALLY_OR_UNATTRIBUTABLE\t" + path);

            Assert.That(q0, Is.Not.Empty, "the actual repository manifest must expose the staged Q0 delta");
            foreach (var required in RequiredCurrentQ0Paths)
                Assert.That(q0, Does.Contain(required), "required exact Q0 path is absent from the actual repository manifest: " + required);

            // The captured pre-run workspace was already dirty. These non-Q0
            // rows are intentionally emitted as an exact current inventory,
            // never as a clean before/after claim. A new unknown path cannot be
            // relabeled Q0 because Q0 has no wildcard admission route.
            Assert.That(q0.All(DeclaredQ0Paths.Contains), Is.True);
        }

        [Test]
        public void AC011_EvidenceBackedHistoricalSourcesAndPinnedC2C2RSuccessorsMatchTheirSeparateManifests()
        {
            var root = Directory.GetParent(Application.dataPath).FullName;
            var evidence = File.ReadAllText(Path.Combine(root, "docs", "verification", "2026-09-20-vd09-m5d7q0-implementation-evidence.md"));
            // Q0AC011: the nine historical evidence rows remain immutable.
            // C2RAC008 permits only the two named, independently evidenced
            // successors below; it is not an old-or-new hash fallback.
            foreach (var entry in EvidenceHashedSources)
            {
                var path = Path.Combine(root, entry.Path.Replace('/', Path.DirectorySeparatorChar));
                Assert.That(File.Exists(path), Is.True, entry.Path);
                StringAssert.Contains("| `" + entry.Path + "` | `" + entry.Hash + "` |", evidence, entry.Path + " evidence row");
            }

            Assert.That(HistoricalCurrentHashedSources.Length, Is.EqualTo(7));
            foreach (var entry in HistoricalCurrentHashedSources)
            {
                var historical = EvidenceHashedSources.Single(candidate => candidate.Path == entry.Path);
                Assert.That(entry.Hash, Is.EqualTo(historical.Hash), entry.Path + " historical source hash");
                var path = Path.Combine(root, entry.Path.Replace('/', Path.DirectorySeparatorChar));
                Assert.That(Hash(path), Is.EqualTo(entry.Hash), entry.Path + " current hash");
            }

            Assert.That(CurrentSuccessorHashedSources.Length, Is.EqualTo(2));
            CollectionAssert.AreEquivalent(new[]
            {
                "Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs",
                "Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs"
            }, CurrentSuccessorHashedSources.Select(entry => entry.Path).ToArray());

            var r25Digest = File.ReadAllText(Path.Combine(root, "docs", "verification", "2026-09-28-vd09-m5d7q-c2-r25-luna-digest.md"));
            var processExecution = File.ReadAllText(Path.Combine(root, "docs", "verification", "2026-09-28-vd09-m5d7q-c2r-process-execution.md"));
            var adapterApproval = File.ReadAllText(Path.Combine(root, "docs", "approvals", "2026-09-28-c3-adapter-successor-sha-approval.md"));
            AssertC4CurrentSuccessorPins(root);
            foreach (var entry in CurrentSuccessorHashedSources)
            {
                var path = Path.Combine(root, entry.Path.Replace('/', Path.DirectorySeparatorChar));
                Assert.That(File.Exists(path), Is.True, entry.Path);
                AssertSuccessorProvenance(entry, r25Digest, processExecution, adapterApproval);
            }
        }

        [Test]
        public void AC011_StaticProductionAuditFindsOneGeneratedSubscriberAndOneSemanticSource()
        {
            var root = Directory.GetParent(Application.dataPath).FullName;
            var runtime = Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Input", "Unity");
            var sources = Directory.GetFiles(runtime, "*.cs", SearchOption.TopDirectoryOnly)
                .Select(File.ReadAllText).ToArray();
            var router = File.ReadAllText(Path.Combine(runtime, "InputRouter.cs"));
            const string routerContract = @"internal\s+sealed\s+class\s+InputRouter\s*:\s*MonoBehaviour\s*,\s*IUiSemanticFrameSourceV1\s*,\s*GameInputActions\.IGameplayActions\s*,\s*GameInputActions\.IUIActions";
            Assert.That(Regex.Matches(router, routerContract).Count, Is.EqualTo(1), "exactly one generated UI callback subscriber and semantic frame source");
            Assert.That(sources.Any(source => source.Contains("InputSystemUIInputModule") || Regex.IsMatch(source, @"new\s+InputRouter\s*\(") || source.Contains("Mouse.current")), Is.False);
        }

        [Test]
        public void AC011_ActualRepositoryManifestPermitsOnlyIndividuallyNamedQ0Metas()
        {
            // The changed/untracked manifest is the only scope inventory.
            // Do not reintroduce a Hub-folder enumeration: that would make an
            // arbitrary file appear Q0 merely because of its location.
            var root = Directory.GetParent(Application.dataPath).FullName;
            var candidates = CurrentRepositoryChangedOrUntrackedPaths(root)
                .Where(path => DeclaredQ0Paths.Contains(path) && path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                .ToArray();
            foreach (var path in candidates)
                Assert.That(AllowedMeta.Contains(path), Is.True, path + " is not an exact Q0 metadata allowlist entry.");
            Assert.That(AllowedMeta.Count, Is.EqualTo(14));
        }

        private static readonly HashSet<string> DeclaredQ0Paths = new HashSet<string>(
            AllowedAmended.Concat(AllowedNew).Concat(AllowedMeta).Concat(AllowedDocumentation), StringComparer.Ordinal);

        private static readonly string[] RequiredCurrentQ0Paths =
        {
            "Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs",
            "Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringValidator.cs",
            "Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubRuntimeAuthoringBuilderEditModeTests.cs",
            "Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs",
            "Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0RemainingRuntimePlayModeTests.cs",
            "docs/verification/2026-09-20-vd09-m5d7q0-implementation-evidence.md"
        };

        private static IEnumerable<string> CurrentRepositoryChangedOrUntrackedPaths(string root)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "status --porcelain=v1 -z --untracked-files=all",
                WorkingDirectory = root,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using (var process = Process.Start(startInfo))
            {
                var standardOutput = process.StandardOutput.ReadToEnd();
                var standardError = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new InvalidOperationException("AC011 could not read the actual Git manifest: " + standardError);

                var records = standardOutput.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
                for (var index = 0; index < records.Length; index++)
                {
                    var record = records[index];
                    if (record.Length < 4) throw new InvalidOperationException("AC011 Git manifest record is malformed.");
                    yield return Normalize(record.Substring(3));
                    // Porcelain -z sends a second path for rename/copy rows.
                    if ((record[0] == 'R' || record[0] == 'C' || record[1] == 'R' || record[1] == 'C') && ++index < records.Length)
                        yield return Normalize(records[index]);
                }
            }
        }

        private static readonly EvidenceHash[] EvidenceHashedSources =
        {
            new EvidenceHash("Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs", "6C1BBAA1F9AB4000F492233E96AC69D6B93DD5B32378F58C9EC92F13C4EA8341"),
            new EvidenceHash("Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouterDiagnostic.cs", "D1768821D6B635AD1B515FCEB326BC58C2F92E1551D640B2BAA268F9983181F1"),
            new EvidenceHash("Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs", "3A3110407949A42FA172EBF0A14F4508364FF1C1D6B739B3EF62246912F08A3D"),
            new EvidenceHash("Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs", "1201240D6E874E596E00D36EFD37330D3C37AEF0591EDA38BAF30D635E4C267C"),
            new EvidenceHash("Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringValidator.cs", "C88C27BC316192925F652017F4251E13EED86AE5F200358B48FF8A4714BE6067"),
            new EvidenceHash("Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubRuntimeAuthoringBuilderEditModeTests.cs", "E853B2A777B86E9F70AF4B06FED7427DB7A9DF0710A1BD3F3B311EAD3F1D122D"),
            new EvidenceHash("Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyInputRouterPlayModeTests.cs", "2CCBF5CE34A541E6012CCB48086353356D71D766C5BED68C43018AA1BE0FFE1F"),
            new EvidenceHash("Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0CoverageBPlayModeTests.cs", "8EEDFA700F94B0101A1C4AF89A5D26905C5A8F95B42781935D6A6BD3832328A5"),
            new EvidenceHash("Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0RemainingRuntimePlayModeTests.cs", "6899830A32D452F8C82BC18697E1B9744C2C050EAC7A89A856DE6240EB9C5D1F")
        };

        private static readonly EvidenceHash[] HistoricalCurrentHashedSources =
        {
            new EvidenceHash("Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouterDiagnostic.cs", "D1768821D6B635AD1B515FCEB326BC58C2F92E1551D640B2BAA268F9983181F1"),
            new EvidenceHash("Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs", "1201240D6E874E596E00D36EFD37330D3C37AEF0591EDA38BAF30D635E4C267C"),
            new EvidenceHash("Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringValidator.cs", "C88C27BC316192925F652017F4251E13EED86AE5F200358B48FF8A4714BE6067"),
            new EvidenceHash("Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubRuntimeAuthoringBuilderEditModeTests.cs", "E853B2A777B86E9F70AF4B06FED7427DB7A9DF0710A1BD3F3B311EAD3F1D122D"),
            new EvidenceHash("Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyInputRouterPlayModeTests.cs", "2CCBF5CE34A541E6012CCB48086353356D71D766C5BED68C43018AA1BE0FFE1F"),
            new EvidenceHash("Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0CoverageBPlayModeTests.cs", "8EEDFA700F94B0101A1C4AF89A5D26905C5A8F95B42781935D6A6BD3832328A5"),
            new EvidenceHash("Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/HubUiOnlyQ0RemainingRuntimePlayModeTests.cs", "6899830A32D452F8C82BC18697E1B9744C2C050EAC7A89A856DE6240EB9C5D1F")
        };

        private static readonly EvidenceHash[] CurrentSuccessorHashedSources =
        {
            new EvidenceHash("Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs", "66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB"),
            new EvidenceHash("Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs", "1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB")
        };

        private static void AssertSuccessorProvenance(EvidenceHash entry, string r25Digest, string processExecution, string adapterApproval)
        {
            if (entry.Path.EndsWith("InputRouter.cs", StringComparison.Ordinal))
            {
                StringAssert.Contains("Router\n`" + entry.Hash + "`", r25Digest, entry.Path + " R25 provenance");
                StringAssert.Contains("router `" + entry.Hash + "`", processExecution, entry.Path + " process provenance");
                return;
            }

            if (entry.Path.EndsWith("DesktopProfileLaunchAdapterV1.cs", StringComparison.Ordinal))
            {
                StringAssert.Contains("승인 SHA-256: `" + entry.Hash + "`", adapterApproval, entry.Path + " C3 successor approval provenance");
                StringAssert.Contains("REQ-M5D7QC3-001/007", adapterApproval, entry.Path + " C3 contract provenance");
                return;
            }

            Assert.Fail("Unknown current successor source: " + entry.Path);
        }

        // REQ-M5D7QC4-007 / AC-M5D7QC4-009/010: 승인된 실제 자원만 현재 두 파일을 정한다.
        private const string C4CurrentSuccessorPinPath = "docs/evidence/c4-audit-current-source-pins-v2.json";
        private const string C4CurrentSuccessorPinSha256 = "5DD70E7A7BD8E79A997F10362EF2DBAD148A913BD3221E20CF5F0DF8D9F33AF5";
        [Serializable] private sealed class C4PinBinding { public string Path; public string Sha256; }
        [Serializable] private sealed class C4RuntimePin
        { public string Path; public string Sha256; public C4PinBinding Predecessor; public C4PinBinding ChangeApproval; public C4PinBinding StaticReview; }
        [Serializable] private sealed class C4PinResource { public int SchemaVersion; public string[] Trace; public C4RuntimePin[] RuntimePins; }
        [Serializable] private sealed class C4PredecessorRow { public string Sha256; public string SourcePath; public string PreservedPath; }
        [Serializable] private sealed class C4PredecessorResource { public C4PredecessorRow[] Files; }
        private static void AssertC4CurrentSuccessorPins(string root)
        {
            var resourcePath=Path.Combine(root,C4CurrentSuccessorPinPath.Replace('/',Path.DirectorySeparatorChar));
            Assert.That(Hash(resourcePath),Is.EqualTo(C4CurrentSuccessorPinSha256),"실제 승인 자원의 바이트 결속");
            // 고정 SHA 검증이 해석보다 먼저이므로 다른 키·중복 키·값은 이 분기에 들어오지 못한다.
            var resource=JsonUtility.FromJson<C4PinResource>(File.ReadAllText(resourcePath));
            Assert.That(resource,Is.Not.Null); Assert.That(resource.SchemaVersion,Is.EqualTo(1));
            Assert.That(resource.Trace,Is.EqualTo(new[] {"REQ-M5D7QC4-002","REQ-M5D7QC4-004","REQ-M5D7QC4-006","REQ-M5D7QC4-007","AC-M5D7QC4-002","AC-M5D7QC4-004","AC-M5D7QC4-006","AC-M5D7QC4-008","AC-M5D7QC4-009","AC-M5D7QC4-010"}));
            Assert.That(resource.RuntimePins,Has.Length.EqualTo(2));
            var predecessorPath=Path.Combine(root,"docs/evidence/c4-runtime-predecessor-manifest.json");
            Assert.That(Hash(predecessorPath),Is.EqualTo("C8FD2B74EF842F45B8D202B1DE3DC408F09F20F69C1BD030FE589E43B8DDD7F7"));
            var predecessor=JsonUtility.FromJson<C4PredecessorResource>(File.ReadAllText(predecessorPath));
            Assert.That(predecessor.Files,Has.Length.EqualTo(9));
            var seen=new HashSet<string>(StringComparer.Ordinal);
            foreach (var pin in resource.RuntimePins)
            {
                var original=CurrentSuccessorHashedSources.Single(entry=>entry.Path==pin.Path);
                Assert.That(seen.Add(pin.Path),Is.True,"현재 경로 중복 금지");
                Assert.That(Hash(Path.Combine(root,pin.Path.Replace('/',Path.DirectorySeparatorChar))),Is.EqualTo(pin.Sha256));
                Assert.That(pin.Predecessor,Is.Not.Null);
                Assert.That(pin.Predecessor.Path,Is.EqualTo("docs/evidence/c4-runtime-predecessor/"+Path.GetFileName(original.Path)+".txt"));
                Assert.That(pin.Predecessor.Sha256,Is.EqualTo(original.Hash));
                var preserved=predecessor.Files.Single(entry=>entry.SourcePath==original.Path);
                Assert.That(preserved.PreservedPath,Is.EqualTo(pin.Predecessor.Path)); Assert.That(preserved.Sha256,Is.EqualTo(original.Hash));
                AssertC4PinBinding(root,pin.Predecessor,preserved.PreservedPath,original.Hash);
                if (pin.Path == "Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs")
                    AssertC4PinBinding(root,pin.ChangeApproval,"docs/specs/work-contracts/2026-09-30-c4-editmode-disposal-pointer-recovery-amendment.md","01D638189329EEE39C52FA663D1EF18242EB321E60875156F7204DCA1FD45791");
                else
                    AssertC4PinBinding(root,pin.ChangeApproval,"docs/specs/work-contracts/2026-09-29-c4-r4-exact-implementation-amendment.md","7FE7E65B8E90D01C89465A77910A8BF6DBE09E6C1224867EDCC6A601CFF2289D");
                StringAssert.Contains("Approved",File.ReadAllText(Path.Combine(root,pin.ChangeApproval.Path)));
                if (pin.Path == "Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs")
                    AssertC4PinBinding(root,pin.StaticReview,"docs/verification/2026-09-30-c4-editmode-disposal-pointer-recovery-luna-implementation-review.md","03AC446E15FBD950CEA90E8D50644B9E5DE1F299E2932DE8CEFCAD2D9A017863");
                else
                    AssertC4PinBinding(root,pin.StaticReview,"docs/verification/2026-09-30-c4-adapter-router-pin-luna-review.md","06D5CE50E6B217B62F0AF3D6CFA102C5FDE2F25764A0F53A6D5CCA8C50BE9F9F");
                StringAssert.Contains(pin.Sha256,File.ReadAllText(Path.Combine(root,pin.StaticReview.Path)));
            }
            Assert.That(seen.SetEquals(CurrentSuccessorHashedSources.Select(entry=>entry.Path)),Is.True);
        }
        private static void AssertC4PinBinding(string root,C4PinBinding binding,string exactPath,string exactSha256)
        {
            Assert.That(binding,Is.Not.Null); Assert.That(binding.Path,Is.EqualTo(exactPath)); Assert.That(binding.Sha256,Is.EqualTo(exactSha256));
            Assert.That(Hash(Path.Combine(root,binding.Path.Replace('/',Path.DirectorySeparatorChar))),Is.EqualTo(binding.Sha256));
        }

        private static string Hash(string path)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-", string.Empty);
        }

        private readonly struct EvidenceHash
        {
            internal EvidenceHash(string path, string hash) { Path = path; Hash = hash; }
            internal string Path { get; }
            internal string Hash { get; }
        }

        private static HashSet<string> Set(params string[] values) => new HashSet<string>(values, StringComparer.Ordinal);
        private static string Normalize(string path) => path.Replace('\\', '/').TrimStart('/');
    }
}
