// REQ-PLAT-001, REQ-PLAT-005:
// Explicit Windows x64 Mono development-build entrypoint for Unity 6.3 batch mode.
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace AcadeGameMaker.Tests.Bootstrap
{
    public static class BootstrapBuild
    {
        public static void BuildWindows64Development()
        {
            var outputPath = GetRequiredArgument("-bootstrapBuildPath");
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                throw new BuildFailedException("Bootstrap build requires at least one enabled scene.");
            }

            var outputDirectory = Path.GetDirectoryName(outputPath);
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                throw new BuildFailedException("The -bootstrapBuildPath argument must include an output directory.");
            }

            Directory.CreateDirectory(outputDirectory);
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);

            var report = BuildPipeline.BuildPlayer(
                scenes,
                outputPath,
                BuildTarget.StandaloneWindows64,
                BuildOptions.Development | BuildOptions.StrictMode);

            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(
                    $"Bootstrap Windows x64 development build failed: {report.summary.result}.");
            }
        }

        private static string GetRequiredArgument(string name)
        {
            var arguments = Environment.GetCommandLineArgs();
            for (var index = 0; index < arguments.Length - 1; index++)
            {
                if (string.Equals(arguments[index], name, StringComparison.Ordinal))
                {
                    return arguments[index + 1];
                }
            }

            throw new BuildFailedException($"Missing required command argument: {name}");
        }
    }
}
