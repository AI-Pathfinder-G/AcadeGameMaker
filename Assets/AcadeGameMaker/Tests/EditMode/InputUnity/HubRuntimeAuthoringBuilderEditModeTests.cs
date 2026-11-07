using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using AcadeGameMaker.Hub.Authoring.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.InputUnity
{
    // REQ-M5D7Q0-009 / AC-M5D7Q0-008. The builder is the only writer for the
    // canonical Q0 prefab; these checks deliberately do not repair drift.
    internal sealed class HubRuntimeAuthoringBuilderEditModeTests
    {
        [Test]
        public void AC008_BuilderCreatesTheExactActiveCanonicalPrefab()
        {
            var root = HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();

            Assert.That(root.name, Is.EqualTo("HubRuntimeRoot"));
            Assert.That(root.activeSelf, Is.True);
            Assert.That(AssetDatabase.GetAssetPath(root), Is.EqualTo(HubRuntimeAuthoringBuilder.PrefabPath));
            Assert.That(PrefabUtility.GetPrefabAssetType(root), Is.EqualTo(PrefabAssetType.Regular));
            Assert.That(root.transform.childCount, Is.Zero);
            Assert.That(root.GetComponents<Component>(), Has.Length.EqualTo(4));
            Assert.That(() => HubRuntimeAuthoringValidator.ValidateCanonicalAsset(root), Throws.Nothing);
        }

        [Test]
        public void AC008_SecondBuilderRunPreservesCanonicalBytesAndMetaGuids()
        {
            HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            var folderGuid = AssetDatabase.AssetPathToGUID(HubRuntimeAuthoringBuilder.FolderPath);
            var prefabGuid = AssetDatabase.AssetPathToGUID(HubRuntimeAuthoringBuilder.PrefabPath);
            var before = File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath);

            var root = HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();

            CollectionAssert.AreEqual(before, File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath));
            Assert.That(AssetDatabase.AssetPathToGUID(HubRuntimeAuthoringBuilder.FolderPath), Is.EqualTo(folderGuid));
            Assert.That(AssetDatabase.AssetPathToGUID(HubRuntimeAuthoringBuilder.PrefabPath), Is.EqualTo(prefabGuid));
            Assert.That(() => HubRuntimeAuthoringValidator.ValidateCanonicalAsset(root), Throws.Nothing);
        }

        [Test]
        public void AC008_ValidCanonicalAssetUsesNoOpBuilderPathWithoutByteOrMetaWrite()
        {
            var first = HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            var beforeBytes = File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath);
            var beforeMeta = File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath + ".meta");

            var second = HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();

            Assert.That(second, Is.SameAs(first));
            CollectionAssert.AreEqual(beforeBytes, File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath));
            CollectionAssert.AreEqual(beforeMeta, File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath + ".meta"));
        }

        [Test]
        public void AC008_RepairRestoresExactCanonicalBytesAndMeta()
        {
            HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            var before = File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath);
            var meta = File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath + ".meta");
            var contents = PrefabUtility.LoadPrefabContents(HubRuntimeAuthoringBuilder.PrefabPath);
            try { new GameObject("repair-drift").transform.SetParent(contents.transform, false); PrefabUtility.SaveAsPrefabAsset(contents, HubRuntimeAuthoringBuilder.PrefabPath); }
            finally { PrefabUtility.UnloadPrefabContents(contents); }
            var repaired = HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            CollectionAssert.AreEqual(before, File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath));
            CollectionAssert.AreEqual(meta, File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath + ".meta"));
            Assert.That(() => HubRuntimeAuthoringValidator.ValidateCanonicalAsset(repaired), Throws.Nothing);
        }

        [Test]
        public void AC008_ValidatorRejectsForbiddenChildWithoutRepairingCandidate()
        {
            HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            var contents = PrefabUtility.LoadPrefabContents(HubRuntimeAuthoringBuilder.PrefabPath);
            try
            {
                new GameObject("forbidden-child").transform.SetParent(contents.transform, false);
                PrefabUtility.SaveAsPrefabAsset(contents, HubRuntimeAuthoringBuilder.PrefabPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(contents); }

            var drifted = AssetDatabase.LoadAssetAtPath<GameObject>(HubRuntimeAuthoringBuilder.PrefabPath);
            try
            {
                Assert.That(() => HubRuntimeAuthoringValidator.ValidateCanonicalAsset(drifted), Throws.InvalidOperationException);
                Assert.That(drifted.transform.childCount, Is.EqualTo(1));
            }
            finally { HubRuntimeAuthoringBuilder.BuildCanonicalPrefab(); }
        }

        // REQ-M5D7Q0-009 / AC-M5D7Q0-008. Each row persists one drift into the
        // canonical asset, proves the observational validator does not rewrite
        // its bytes, then permits only the builder to restore the graph.
        [TestCase("flag", "discriminator")]
        [TestCase("intent", "discriminator")]
        [TestCase("mode", "mode")]
        [TestCase("adapter-router", "sibling identity")]
        [TestCase("latch-router", "sibling identity")]
        [TestCase("latch-adapter", "sibling identity")]
        [TestCase("router-disabled", "enabled matrix")]
        [TestCase("adapter-disabled", "enabled matrix")]
        [TestCase("latch-disabled", "enabled matrix")]
        public void AC008_ValidatorRejectsEachReachableCanonicalMutationWithoutRepair(string mutation, string failure)
        {
            HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            var contents = PrefabUtility.LoadPrefabContents(HubRuntimeAuthoringBuilder.PrefabPath);
            try
            {
                var router = contents.GetComponent<AcadeGameMaker.Input.Unity.InputRouter>();
                var adapter = contents.GetComponent<AcadeGameMaker.Input.Unity.DesktopProfileLaunchAdapterV1>();
                var latch = contents.GetComponent<AcadeGameMaker.Input.Unity.HubEntryHandoffLatchV1>();
                if (mutation == "router-disabled") router.enabled = false;
                else if (mutation == "adapter-disabled") adapter.enabled = false;
                else if (mutation == "latch-disabled") latch.enabled = false;
                else
                {
                    var identity = mutation == "adapter-router" ? (UnityEngine.Object)adapter : mutation == "latch-router" || mutation == "latch-adapter" ? latch : router;
                    var serialized = new SerializedObject(identity);
                    var property = serialized.FindProperty(mutation == "flag" ? "_authoredHubUiOnly" : mutation == "intent" ? "_hubUiOnlyIntentProof" : mutation == "mode" ? "_initialMode" : mutation == "adapter-router" || mutation == "latch-router" ? "_router" : mutation == "latch-adapter" ? "_launchAdapter" : "_cameraProviderBehaviour");
                    if (mutation == "flag" || mutation == "intent") property.boolValue = false;
                    else if (mutation == "mode") property.intValue = (int)AcadeGameMaker.Core.InputMode.GameplayEnabled;
                    else property.objectReferenceValue = null;
                    serialized.ApplyModifiedPropertiesWithoutUndo();
                }
                PrefabUtility.SaveAsPrefabAsset(contents, HubRuntimeAuthoringBuilder.PrefabPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(contents); }

            try
            {
                var before = File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath);
                var drifted = AssetDatabase.LoadAssetAtPath<GameObject>(HubRuntimeAuthoringBuilder.PrefabPath);
                var thrown = Assert.Throws<InvalidOperationException>(() => HubRuntimeAuthoringValidator.ValidateCanonicalAsset(drifted));
                StringAssert.Contains(failure, thrown.Message);
                CollectionAssert.AreEqual(before, File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath));
            }
            finally { HubRuntimeAuthoringBuilder.BuildCanonicalPrefab(); }
        }

        [Test]
        public void AC008_ValidatorRejectsCanonicalMissingScriptWithoutRepair()
        {
            HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            try
            {
                // Unity refuses to persist a missing script through the prefab
                // save API. Corrupt only the latch's exact YAML script GUID,
                // import synchronously, then prove the validator observes it.
                var latchGuid = AssetDatabase.AssetPathToGUID("Assets/AcadeGameMaker/Runtime/Input/Unity/HubEntryHandoffLatchV1.cs");
                Assert.That(latchGuid, Is.Not.Empty, "latch script GUID");
                var beforeMutation = File.ReadAllText(HubRuntimeAuthoringBuilder.PrefabPath);
                var pattern = "(m_Script:\\s*\\{fileID:\\s*11500000,\\s*guid:\\s*)" + Regex.Escape(latchGuid) + "(,\\s*type:\\s*3\\})";
                const string missingScriptGuid = "0123456789abcdef0123456789abcdef";
                Assert.That(AssetDatabase.GUIDToAssetPath(missingScriptGuid), Is.Empty, "fixture GUID must remain unassigned");
                var corrupted = new Regex(pattern).Replace(beforeMutation, "${1}" + missingScriptGuid + "${2}", 1);
                Assert.That(corrupted, Is.Not.EqualTo(beforeMutation), "fixture must replace the latch script GUID exactly once");
                File.WriteAllText(HubRuntimeAuthoringBuilder.PrefabPath, corrupted);
                AssetDatabase.ImportAsset(HubRuntimeAuthoringBuilder.PrefabPath, ImportAssetOptions.ForceSynchronousImport);
                var drifted = AssetDatabase.LoadAssetAtPath<GameObject>(HubRuntimeAuthoringBuilder.PrefabPath);
                Assert.That(Array.Exists(drifted.GetComponents<Component>(), component => component == null), Is.True,
                    "fixture must deserialize as a missing script");
                AssertNoRepairAndMessage("missing script");
            }
            finally { HubRuntimeAuthoringBuilder.BuildCanonicalPrefab(); }
        }

        [Test]
        public void AC008_ValidatorRejectsCanonicalPrefabVariantOverrideWithoutRepair()
        {
            const string basePath = "Assets/Prefabs/Hub/__m5d7q0-override-base.prefab";
            HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            GameObject baseContents = null;
            GameObject instance = null;
            try
            {
                baseContents = PrefabUtility.LoadPrefabContents(HubRuntimeAuthoringBuilder.PrefabPath);
                PrefabUtility.SaveAsPrefabAsset(baseContents, basePath);
                PrefabUtility.UnloadPrefabContents(baseContents); baseContents = null;
                instance = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(basePath)) as GameObject;
                instance.tag = "Player"; // valid tag; creates an otherwise-unobserved variant override.
                PrefabUtility.SaveAsPrefabAsset(instance, HubRuntimeAuthoringBuilder.PrefabPath);
                UnityEngine.Object.DestroyImmediate(instance); instance = null;
                var variant = AssetDatabase.LoadAssetAtPath<GameObject>(HubRuntimeAuthoringBuilder.PrefabPath);
                Assert.That(PrefabUtility.GetPrefabAssetType(variant), Is.EqualTo(PrefabAssetType.Variant), "fixture must be a real prefab variant");
                AssertNoRepairAndMessage("regular prefab");
                // Replace the variant while its parent still exists; removing
                // the parent first imports a deliberately broken asset and
                // creates an unrelated Unity error-log failure.
                HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            }
            finally
            {
                if (instance != null) UnityEngine.Object.DestroyImmediate(instance);
                if (baseContents != null) PrefabUtility.UnloadPrefabContents(baseContents);
                if (AssetDatabase.LoadAssetAtPath<GameObject>(basePath) != null) AssetDatabase.DeleteAsset(basePath);
                HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            }
        }

        [Test]
        public void AC008_ValidatorRejectsCanonicalComponentOrderWithoutRepair()
        {
            HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            var contents = PrefabUtility.LoadPrefabContents(HubRuntimeAuthoringBuilder.PrefabPath);
            try
            {
                var latch = contents.GetComponent<AcadeGameMaker.Input.Unity.HubEntryHandoffLatchV1>();
                Assert.That(UnityEditorInternal.ComponentUtility.MoveComponentUp(latch), Is.True);
                PrefabUtility.SaveAsPrefabAsset(contents, HubRuntimeAuthoringBuilder.PrefabPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(contents); }

            try { AssertNoRepairAndMessage("component order"); }
            finally { HubRuntimeAuthoringBuilder.BuildCanonicalPrefab(); }
        }

        // REQ-M5D7Q0-009 / AC-M5D7Q0-008. These are persisted production/UI
        // drifts. TypeCache resolves the actual loaded type by full name and
        // avoids fragile partial assembly-name lookups.
        [TestCase("camera", "UnityEngine.Camera")]
        [TestCase("presentation-canvas", "UnityEngine.Canvas")]
        [TestCase("event-system", "UnityEngine.EventSystems.EventSystem")]
        [TestCase("presentation-ui", "UnityEngine.UI.Image")]
        [TestCase("gameplay-component", "AcadeGameMaker.Transfer.Unity.TransferSimulationDriver")]
        [TestCase("transition-executor", "AcadeGameMaker.Input.Unity.OrdanBossTerminalTransitionRequester")]
        public void AC008_ValidatorRejectsEachForbiddenExtraComponentWithoutRepair(string category, string typeName)
        {
            AssertForbiddenExtraComponentWithoutRepair(category, ResolveComponentType(typeName, category));
        }

        [Test]
        public void AC008_ValidatorRejectsEachAvailableUiOrTmpExtraComponentWithoutRepair()
        {
            const string fullTypeName = "TMPro.TextMeshProUGUI";
            var type = FindComponentType(fullTypeName);
            if (type == null)
            {
                // TMP is the only optional package here. Explicitly prove
                // that TypeCache cannot see it rather than silently skip.
                var matches = TypeCache.GetTypesDerivedFrom<Component>()
                    .Where(candidate => candidate.FullName == fullTypeName).ToArray();
                Assert.That(matches, Is.Empty, "optional TMP absence must be explicit");
                TestContext.WriteLine("OPTIONAL_UI_TMP_COMPONENT_UNAVAILABLE\t" + fullTypeName);
                return;
            }
            AssertForbiddenExtraComponentWithoutRepair(type.Name, EnsureComponentType(type, fullTypeName));
        }

        [Test]
        public void AC008_EngineFreeHubMenuPresentationControllerIsNotAComponentWhenAccessible()
        {
            const string fullTypeName = "AcadeGameMaker.Input.Unity.HubMenuPresentationControllerV1";
            var presentationController = TypeCache.GetTypesDerivedFrom<object>()
                .FirstOrDefault(type => type.FullName == fullTypeName);
            if (presentationController == null)
            {
                TestContext.WriteLine("ENGINE_FREE_PRESENTATION_CONTROLLER_UNAVAILABLE\t" + fullTypeName);
                return;
            }
            Assert.That(typeof(Component).IsAssignableFrom(presentationController), Is.False,
                "the engine-free presentation controller must not become a persisted Unity component");
        }

        [TestCase("_player", "AcadeGameMaker.Movement.PlayerMovementController")]
        [TestCase("_transfer", "AcadeGameMaker.Transfer.Unity.TransferSimulationDriver")]
        [TestCase("_combat", "AcadeGameMaker.Combat.Unity.OrdanBossCombatSimulationDriver")]
        [TestCase("_terminalTeardown", "AcadeGameMaker.Combat.Unity.OrdanBossTerminalTeardown")]
        [TestCase("_cameraProviderBehaviour", "AcadeGameMaker.Input.Unity.OrdanBossTerminalTransitionRequester")]
        public void AC008_ValidatorRejectsEachExternalForbiddenReferenceWithoutRepair(string field, string typeName)
        {
            HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            GameObject contents = null;
            try
            {
                contents = PrefabUtility.LoadPrefabContents(HubRuntimeAuthoringBuilder.PrefabPath);
                if (typeName == "AcadeGameMaker.Movement.PlayerMovementController")
                {
                    contents.AddComponent<Rigidbody2D>();
                    contents.AddComponent<BoxCollider2D>();
                }
                var referencedComponent = contents.AddComponent(ResolveComponentType(typeName, field));
                Assert.That(referencedComponent, Is.Not.Null, field + " fixture component");
                var serialized = new SerializedObject(contents.GetComponent<AcadeGameMaker.Input.Unity.InputRouter>());
                serialized.FindProperty(field).objectReferenceValue = referencedComponent;
                serialized.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(contents, HubRuntimeAuthoringBuilder.PrefabPath);
                PrefabUtility.UnloadPrefabContents(contents); contents = null;
                AssertNoRepairAndMessage("forbidden-reference");
            }
            finally
            {
                if (contents != null) PrefabUtility.UnloadPrefabContents(contents);
                HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            }
        }

        private static void AssertForbiddenExtraComponentWithoutRepair(string category, Type componentType)
        {
            HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
            GameObject contents = null;
            try
            {
                contents = PrefabUtility.LoadPrefabContents(HubRuntimeAuthoringBuilder.PrefabPath);
                var added = contents.AddComponent(componentType);
                Assert.That(added, Is.Not.Null, category + " fixture component");
                PrefabUtility.SaveAsPrefabAsset(contents, HubRuntimeAuthoringBuilder.PrefabPath);
                PrefabUtility.UnloadPrefabContents(contents); contents = null;

                var before = File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath);
                var drifted = AssetDatabase.LoadAssetAtPath<GameObject>(HubRuntimeAuthoringBuilder.PrefabPath);
                Assert.That(drifted.GetComponent(componentType), Is.Not.Null, category + " persists into the canonical asset");
                var thrown = Assert.Throws<InvalidOperationException>(() => HubRuntimeAuthoringValidator.ValidateCanonicalAsset(drifted));
                StringAssert.Contains("forbidden component", thrown.Message, category);
                CollectionAssert.AreEqual(before, File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath), category + " validator does not repair");
            }
            finally
            {
                if (contents != null) PrefabUtility.UnloadPrefabContents(contents);
                var rebuilt = HubRuntimeAuthoringBuilder.BuildCanonicalPrefab();
                Assert.That(rebuilt.GetComponent(componentType), Is.Null, category + " only the builder restores the canonical graph");
                Assert.That(() => HubRuntimeAuthoringValidator.ValidateCanonicalAsset(rebuilt), Throws.Nothing, category + " builder restore");
            }
        }

        private static Type ResolveComponentType(string typeName, string category)
        {
            var type = FindComponentType(typeName);
            Assert.That(type, Is.Not.Null, category + " type must resolve; the fixture may not silently skip it.");
            return EnsureComponentType(type, typeName);
        }

        private static Type FindComponentType(string fullTypeName)
        {
            return TypeCache.GetTypesDerivedFrom<Component>()
                .SingleOrDefault(type => type.FullName == fullTypeName);
        }

        private static Type EnsureComponentType(Type type, string typeName)
        {
            Assert.That(typeof(Component).IsAssignableFrom(type), Is.True, typeName + " must be a Unity component.");
            return type;
        }


        [Test]
        public void AC011_HubAuthoringStaticScopeHasNoSecondUiOwnerOrParentFolderWrite()
        {
            var root = Directory.GetParent(Application.dataPath).FullName;
            var router = File.ReadAllText(Path.Combine(root, "Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs"));
            var builder = File.ReadAllText(Path.Combine(root, "Assets/AcadeGameMaker/Editor/HubAuthoring/HubRuntimeAuthoringBuilder.cs"));
            StringAssert.DoesNotContain("InputSystemUIInputModule", router);
            StringAssert.DoesNotContain("Mouse.current", router);
            StringAssert.DoesNotMatch(@"new\s+InputRouter\s*\(", router);
            StringAssert.DoesNotContain("CreateFolder(\"Assets\", \"Prefabs\")", builder);
            StringAssert.Contains("CanonicalYaml", builder);
            StringAssert.Contains("ForceSynchronousImport", builder);
            StringAssert.Contains("ValidateCanonicalAsset", builder);
            StringAssert.DoesNotContain("SaveAsPrefabAsset", builder);
            StringAssert.DoesNotContain("LoadPrefabContents", builder);
        }

        private static void AssertNoRepairAndMessage(string message)
        {
            var before = File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath);
            var drifted = AssetDatabase.LoadAssetAtPath<GameObject>(HubRuntimeAuthoringBuilder.PrefabPath);
            var thrown = Assert.Throws<InvalidOperationException>(() => HubRuntimeAuthoringValidator.ValidateCanonicalAsset(drifted));
            StringAssert.Contains(message, thrown.Message);
            CollectionAssert.AreEqual(before, File.ReadAllBytes(HubRuntimeAuthoringBuilder.PrefabPath));
        }

    }

}
