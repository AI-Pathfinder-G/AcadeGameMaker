// REQ-PLAT-001, REQ-PLAT-002, REQ-PLAT-005, REQ-PLAT-007:
// Editor-only static bootstrap verification. This assembly exposes no runtime API.
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace AcadeGameMaker.Tests.Bootstrap
{
    internal sealed class BootstrapProjectConfigurationTests
    {
        private static string ProjectRoot =>
            Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

        [Test]
        public void ApprovedPlatformBaselineIsConfigured()
        {
            Assert.That(Read("ProjectSettings/ProjectVersion.txt"), Does.Contain("m_EditorVersion: 6000.6.0f1"));

            var manifest = Read("Packages/manifest.json");
            StringAssert.Contains("\"com.unity.inputsystem\": \"1.20.0\"", manifest);
            StringAssert.Contains("\"com.unity.render-pipelines.universal\": \"17.3.0\"", manifest);
            StringAssert.Contains("\"com.unity.test-framework\": \"1.6.0\"", manifest);
            StringAssert.DoesNotContain("com.unity.collab-proxy", manifest);
            StringAssert.DoesNotContain("com.unity.visualscripting", manifest);

            var settings = Read("ProjectSettings/ProjectSettings.asset");
            StringAssert.Contains("productName: AcadeGameMaker", settings);
            StringAssert.Contains("Standalone: com.aipathfinderg.acadegamemaker", settings);
            StringAssert.Contains("defaultScreenWidth: 2560", settings);
            StringAssert.Contains("defaultScreenHeight: 1440", settings);
            StringAssert.Contains("resizableWindow: 1", settings);
            StringAssert.Contains("activeInputHandler: 1", settings);
            StringAssert.Contains("Standalone: 0", settings);

            StringAssert.Contains("Fixed Timestep: 0.016666668", Read("ProjectSettings/TimeManager.asset"));
            StringAssert.Contains("m_Gravity: {x: 0, y: -9.81}", Read("ProjectSettings/Physics2DSettings.asset"));

            var buildSettings = Read("ProjectSettings/EditorBuildSettings.asset");
            StringAssert.Contains("path: Assets/Scenes/Bootstrap.unity", buildSettings);
            StringAssert.DoesNotContain("com.unity.input.settings.actions", buildSettings);
            Assert.That(AssetDatabase.LoadMainAssetAtPath("Assets/Settings/UniversalRP.asset"), Is.Not.Null);
            Assert.That(AssetDatabase.LoadMainAssetAtPath("Assets/Settings/Renderer2D.asset"), Is.Not.Null);
            Assert.That(File.Exists(Path.Combine(ProjectRoot, "Assets", "GameInput.inputactions")), Is.True);
            Assert.That(File.Exists(Path.Combine(ProjectRoot, "Assets", "AcadeGameMaker", "Runtime", "Input", "GameInputActions.cs")), Is.True);
            Assert.That(File.Exists(Path.Combine(ProjectRoot, "Assets", "InputSystem_Actions.inputactions")), Is.False);
        }

        private static string Read(string relativePath) =>
            File.ReadAllText(Path.Combine(ProjectRoot, relativePath));
    }
}
