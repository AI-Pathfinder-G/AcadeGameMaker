using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Combat.Authoring
{
    // REQ-AUTOCH1-001/006, AC-AUTOCH1-006: 명시적인 CH1 릴리스 빌드만 발급한다.
    public static class Chapter1AutonomousBuild
    {
        public static void BuildWindows()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("플레이 실행 중에는 빌드하지 않습니다.");
            var scene=EditorSceneManager.OpenScene(Chapter1ExpeditionAuthoringBuilder.ScenePath,OpenSceneMode.Single);
            Chapter1ExpeditionAuthoringValidator.ValidateScene(scene);
            AcadeGameMaker.Run.Unity.Chapter1RoomCatalog admitted=null;
            foreach(var root in scene.GetRootGameObjects())foreach(var catalog in root.GetComponentsInChildren<AcadeGameMaker.Run.Unity.Chapter1RoomCatalog>(true))
            {if(admitted!=null)throw new InvalidOperationException("편입 카탈로그가 중복입니다.");admitted=catalog;}
            if(admitted==null)throw new InvalidOperationException("실제 편입 카탈로그가 없습니다.");admitted.Validate(true);
            var workspace=Path.GetFullPath(Path.Combine(Application.dataPath,"../../../.."));
            var expectedAssets=Path.GetFullPath(Path.Combine(workspace,"output/chapter1-playable-r1/project/Assets"));
            if(!string.Equals(expectedAssets,Path.GetFullPath(Application.dataPath),StringComparison.OrdinalIgnoreCase) || !File.Exists(Path.Combine(workspace,"docs/README.md")))throw new InvalidOperationException("승인된 프로젝트 작업공간과 일치하지 않습니다.");
            var baseOutput=Path.GetFullPath(Path.Combine(workspace,"output/chapter1-autonomous-20261021/build/windows"));
            VerifyNoLinks(baseOutput);
            var output=baseOutput;
            if(Directory.Exists(output) && Directory.EnumerateFileSystemEntries(output).Any())
            {var attempt=1;do{output=Path.Combine(baseOutput,"attempt-"+(attempt++).ToString("D3"));}while(Directory.Exists(output));}
            output=Path.GetFullPath(output);
            if(!string.Equals(output,baseOutput,StringComparison.OrdinalIgnoreCase) && !string.Equals(Path.GetDirectoryName(output),baseOutput,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("빌드 출력이 승인 경계를 벗어났습니다.");
            VerifyNoLinks(output);Directory.CreateDirectory(output);VerifyNoLinks(output);
            var executable=Path.Combine(output,"AcadeGameMakerChapter1.exe");
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{Chapter1ExpeditionAuthoringBuilder.ScenePath},locationPathName=executable,target=BuildTarget.StandaloneWindows64,options=BuildOptions.None});
            var summary=report.summary;
            File.WriteAllText(Path.Combine(output,"build-report.txt"),"result="+summary.result+"\nplatform="+summary.platform+"\nerrors="+summary.totalErrors+"\nwarnings="+summary.totalWarnings+"\nbytes="+summary.totalSize+"\nduration="+summary.totalTime+"\n",new UTF8Encoding(false));
            if(summary.result!=BuildResult.Succeeded || summary.totalErrors!=0 || !File.Exists(executable))throw new InvalidOperationException("Windows 빌드가 성공하지 못했습니다. 결과 원문을 확인하세요.");
            var data=Path.Combine(output,"AcadeGameMakerChapter1_Data");
            if(new FileInfo(executable).Length==0 || !File.Exists(Path.Combine(output,"UnityPlayer.dll")) ||
                new FileInfo(Path.Combine(output,"UnityPlayer.dll")).Length==0 || !Directory.Exists(data) ||
                !File.Exists(Path.Combine(data,"globalgamemanagers")) || !Directory.Exists(Path.Combine(data,"Managed")))throw new InvalidOperationException("실행에 필요한 Windows 동반 파일이 없습니다.");
            var lines=new StringBuilder();
            foreach(var file in Directory.GetFiles(output,"*",SearchOption.AllDirectories).OrderBy(v=>v,StringComparer.Ordinal))
            {using(var hash=SHA256.Create())using(var stream=File.OpenRead(file))lines.Append(BitConverter.ToString(hash.ComputeHash(stream)).Replace("-",string.Empty)).Append("  ").Append(new FileInfo(file).Length).Append("  ").Append(file.Substring(output.Length+1).Replace('\\','/')).Append('\n');}
            File.WriteAllText(Path.Combine(output,"SHA256SUMS.txt"),lines.ToString(),new UTF8Encoding(false));
            Debug.Log("CH1 Windows 빌드 완료: "+executable);
        }
        private static void VerifyNoLinks(string path)
        {
            for(var current=Path.GetFullPath(path);!string.IsNullOrEmpty(current);current=Path.GetDirectoryName(current))
            {
                if(!Directory.Exists(current) && !File.Exists(current))continue;
                var attributes=File.GetAttributes(current);
                if((attributes&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("빌드 경로의 링크는 허용하지 않습니다: "+current);
                if(!Directory.Exists(current))throw new InvalidOperationException("빌드 폴더 경로에 파일이 있습니다: "+current);
            }
        }
    }
}
