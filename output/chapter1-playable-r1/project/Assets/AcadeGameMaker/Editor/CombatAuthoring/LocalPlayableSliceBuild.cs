using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Combat.Authoring
{
    // REQ-LPS-010/012, AC-LPS-007: explicit current-source single-scene build.
    internal static class LocalPlayableSliceBuild
    {
        [Serializable] private sealed class FileRow { public string path, sha256; public long length; }
        [Serializable] private sealed class BuildLedger
        {
            public string contractSha256="493EA6167158C723D498A33388D8DF4C42873E0E9D56F436CAF08D2DB1087D3E";
            public string parentApprovalSha256="9150DDA73EF79E3CE1C597D8A4946D77899DFA266B288DD6CC0302E41CD67401";
            public string requirement="REQ-LPS-010/012", acceptance="AC-LPS-007", participant="Terra/gpt-6.1-sol";
            public string startedUtc, finishedUtc, result, outputPath, target="StandaloneWindows64", backend="Mono";
            public FileRow[] inputs, outputs;
        }
        public static void BuildWindows()
        {
            var project=Directory.GetParent(Application.dataPath).FullName;
            var bounded=Directory.GetParent(project).FullName;
            var evidence=Path.Combine(bounded,"evidence"); var build=Path.Combine(bounded,"build");
            if(!project.Replace('\\','/').EndsWith("output/local-playable-slice-r1/project",StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Build requires the exact isolated project root.");
            if(PlayerSettings.GetScriptingBackend(NamedBuildTarget.Standalone)!=ScriptingImplementation.Mono2x)
                throw new InvalidOperationException("Existing standalone backend is not Mono; configuration is not repaired.");
            Directory.CreateDirectory(evidence);
            var inputLedger=Path.Combine(evidence,"windows-build-inputs.v1.json");
            var outputLedger=Path.Combine(evidence,"windows-build-output.v1.json");
            if(File.Exists(inputLedger)||File.Exists(outputLedger)||Directory.Exists(build))
                throw new InvalidOperationException("Build evidence/output already exists; automatic overwrite is forbidden.");
            var scene=EditorSceneManager.OpenScene(LocalPlayableSliceAuthoringBuilder.ScenePath);
            LocalPlayableSliceAuthoringValidator.ValidateScene(scene);
            var ledger=new BuildLedger { startedUtc=DateTime.UtcNow.ToString("O"),
                outputPath=Path.Combine(build,"LocalPlayableSlice.exe"),
                inputs=new[]{"Assets","Packages","ProjectSettings"}.SelectMany(folder=>Directory.GetFiles(Path.Combine(project,folder),"*",SearchOption.AllDirectories))
                    .OrderBy(path=>path,StringComparer.Ordinal).Select(path=>ReadRow(project,path)).ToArray() };
            CreateNew(inputLedger,JsonUtility.ToJson(ledger,true));
            Directory.CreateDirectory(build);
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes=new[]{LocalPlayableSliceAuthoringBuilder.ScenePath},
                locationPathName=ledger.outputPath,target=BuildTarget.StandaloneWindows64,options=BuildOptions.StrictMode });
            ledger.finishedUtc=DateTime.UtcNow.ToString("O");ledger.result=report.summary.result.ToString();
            ledger.outputs=Directory.GetFiles(build,"*",SearchOption.AllDirectories).OrderBy(path=>path,StringComparer.Ordinal).Select(path=>ReadRow(build,path)).ToArray();
            CreateNew(outputLedger,JsonUtility.ToJson(ledger,true));
            if(report.summary.result!=BuildResult.Succeeded)throw new InvalidOperationException("Local build failed; raw build evidence retained.");
        }
        private static FileRow ReadRow(string root,string path)
        {
            var bytes=File.ReadAllBytes(path); using(var sha=SHA256.Create())
                return new FileRow { path=path.Substring(root.Length+1).Replace('\\','/'),length=bytes.LongLength,
                    sha256=BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","") };
        }
        private static void CreateNew(string path,string text)
        { using(var stream=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.None))
          using(var writer=new StreamWriter(stream,new System.Text.UTF8Encoding(false)))writer.Write(text); }
    }
}
