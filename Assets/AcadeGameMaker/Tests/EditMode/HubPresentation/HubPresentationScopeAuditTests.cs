using System.IO;
using AcadeGameMaker.Hub.Authoring.Editor;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.HubPresentation
{
    // AC-M5D7QA-002/009: the builder must treat source-controlled TMP settings as immutable input.
    internal sealed class HubPresentationScopeAuditTests
    {
        [Test]
        public void AC_M5D7QA_002_TmpSettingsBytesAreUnchangedByBuilder()
        {
            const string path = "Assets/UI/Resources/TMP Settings.asset";
            var before = File.ReadAllBytes(path);
            HubPresentationAuthoringBuilder.Build();
            CollectionAssert.AreEqual(before, File.ReadAllBytes(path));
        }

        // REQ-M5D7QA-009; AC-M5D7QA-002/010. Once the exact four-label
        // profile is canonical, another invocation must not reserialize it.
        [Test]
        public void AC_M5D7QA_002_And_010_CanonicalLabelProfileIsBuilderByteNoOp()
        {
            HubPresentationAuthoringBuilder.Build();
            var prefab=File.ReadAllBytes(HubPresentationAuthoringBuilder.MenuPrefabPath); var scene=File.ReadAllBytes(HubPresentationAuthoringBuilder.ScenePath);
            HubPresentationAuthoringBuilder.Build();
            CollectionAssert.AreEqual(prefab,File.ReadAllBytes(HubPresentationAuthoringBuilder.MenuPrefabPath)); CollectionAssert.AreEqual(scene,File.ReadAllBytes(HubPresentationAuthoringBuilder.ScenePath));
        }
    }
}
