using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Hub.Presentation.Unity;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.HubPresentation
{
    // Approved AC-M5D7QC3-009/010 successor: exactly one plain framework
    // import is distinguished; the predecessor forbidden-body checks remain.
    internal static class HubMenuIntentHandoffC3AuditInputV1
    {
        private const string Import="using System.Runtime.CompilerServices;";
        internal static string WithExactFrameworkImportExcluded(string source)
        {
            if (source == null) throw new InvalidOperationException("No audit source.");
            var output=new StringBuilder(); var count=0; var header=true;
            var headerLines=new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            using (var reader=new StringReader(source))
            {
                string line;
                while ((line=reader.ReadLine()) != null)
                {
                    if (header)
                    {
                        if (line == "namespace AcadeGameMaker.Hub.Presentation.Unity") header=false;
                        else if (line == Import) { count++; headerLines.Add(line); continue; }
                        else if (!string.IsNullOrWhiteSpace(line) && line != "using System;" && line != "using System.Threading;" && line != "using AcadeGameMaker.Input.Unity;" && line != "using UnityEngine;") throw new InvalidOperationException("Audit import header is not the exact approved header.");
                        else if (!string.IsNullOrWhiteSpace(line) && !headerLines.Add(line)) throw new InvalidOperationException("Audit import header has a duplicate declaration.");
                    }
                    else if (line == Import) throw new InvalidOperationException("Framework import is outside its header.");
                    output.AppendLine(line);
                }
            }
            if (header || count != 1 || headerLines.Count != 5) throw new InvalidOperationException("Audit requires the exact five-import header with one framework import.");
            return output.ToString();
        }
        internal static void AssertForbiddenBody(string source)
        {
            var body=WithExactFrameworkImportExcluded(source);
            foreach (var forbidden in new[] { "SceneManager","Application.Quit","Profile","Run","Settings","Wardrobe","CIO","CUA","InputSystem","UnityEvent","delegate" })
                Assert.That(body,Does.Not.Contain(forbidden),forbidden);
        }
    }
    internal sealed class HubMenuIntentHandoffC3AuditSuccessorTests
    {
        private const string SourcePath="Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs";
        [Test]
        public void AC004_ExactFrameworkImportHasOneStrictSuccessorException()
        {
            HubMenuIntentHandoffC3AuditInputV1.AssertForbiddenBody(File.ReadAllText(SourcePath));
            foreach (var name in new[] { "Issuances","Successors" })
            {
                var field=typeof(HubMenuIntentHandoffOwnerV1).GetField(name,BindingFlags.Static|BindingFlags.NonPublic);
                Assert.That(field,Is.Not.Null); Assert.That(field.IsPrivate,Is.True);
                Assert.That(field.FieldType.GetGenericTypeDefinition(),Is.EqualTo(typeof(System.Runtime.CompilerServices.ConditionalWeakTable<,>)));
            }
        }
        [TestCase("Run")] [TestCase("RunSession")] [TestCase("Profile")] [TestCase("ProfileResetDiskTransactionV1.Begin")]
        [TestCase("SceneManager")] [TestCase("Application.Quit")] [TestCase("Settings")] [TestCase("Wardrobe")]
        [TestCase("CIO")] [TestCase("CUA")] [TestCase("InputSystem")] [TestCase("UnityEvent")] [TestCase("delegate")]
        [TestCase("System.Runtime.CompilerServices")]
        public void AC004_GameplayAndDurableTokensRemainForbiddenAfterImportExclusion(string token)
        {
            var source=File.ReadAllText(SourcePath)+"\n// "+token+"\n";
            Assert.That(() => HubMenuIntentHandoffC3AuditInputV1.AssertForbiddenBody(source),Throws.TypeOf<AssertionException>());
        }
        [TestCase("duplicate")] [TestCase("alias")] [TestCase("global")] [TestCase("space")] [TestCase("namespace")]
        [TestCase("preprocessor")] [TestCase("codeBefore")] [TestCase("codeAfter")] [TestCase("missing")]
        public void AC004_ImportCopiesAliasesAndNonHeaderLocationsAreRejected(string mutation)
        {
            var source=File.ReadAllText(SourcePath); var import="using System.Runtime.CompilerServices;";
            var replacement=mutation == "duplicate" ? import+"\n"+import : mutation == "alias" ? "using X = System.Runtime.CompilerServices;" : mutation == "global" ? "global "+import : mutation == "space" ? " "+import : mutation == "namespace" ? "" : mutation == "preprocessor" ? "#if UNITY_EDITOR\n"+import+"\n#endif" : mutation == "codeBefore" ? "X(); "+import : mutation == "codeAfter" ? import+" X();" : "";
            var position=source.IndexOf(import,StringComparison.Ordinal);
            source=source.Substring(0,position)+replacement+source.Substring(position+import.Length);
            if (mutation == "namespace") source += "\n"+import+"\n";
            Assert.That(() => HubMenuIntentHandoffC3AuditInputV1.WithExactFrameworkImportExcluded(source),Throws.InvalidOperationException);
        }
        [Test]
        public void AC004_PredecessorAuditBytesAndForbiddenBodyRemainEvidenced()
        {
            var bytes=File.ReadAllBytes("docs/evidence/c3-qb-audit-predecessor/HubMenuIntentHandoffEditModeTests.cs.txt");
            using (var hash=SHA256.Create()) Assert.That(BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-",""),Is.EqualTo("FC7DD9F902C9783036137CE68423D43A28C00DE187DA125FDB8C41CDCF79B421"));
            var predecessor=Encoding.UTF8.GetString(bytes);
            var current=File.ReadAllText("Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffEditModeTests.cs");
            var prefix="HubMenuIntentHandoffC3AuditInputV1.WithExactFrameworkImportExcluded(";
            var original="System.IO.File.ReadAllText(\"Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs\")";
            Assert.That(current.Replace(prefix+original+")",original),Is.EqualTo(predecessor));
            Assert.That(typeof(HubMenuIntentHandoffEditModeTests).GetMethods(BindingFlags.Instance|BindingFlags.Public).Count(m => m.GetCustomAttributes(typeof(TestAttribute),false).Length != 0),Is.EqualTo(5));
        }
    }
}
