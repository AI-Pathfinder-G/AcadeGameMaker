using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Run.Unity;
using AcadeGameMaker.Hub.Presentation.Unity;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Camera;
using AcadeGameMaker.Camera.Unity;
using AcadeGameMaker.Movement;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AcadeGameMaker.Combat.Authoring
{
    internal static class Chapter1ExpeditionAuthoringBuilder
    {
        [Serializable] private sealed class AutonomousAdmission
        {
            public int schemaVersion;public string measurementPath,measurementSha256,sourceManifestPath,sourceManifestSha256,lunaDecisionPath,lunaDecisionSha256,astraApprovalSection;
            public AutonomousAdmissionUpdate[] updates;
        }
        [Serializable] private sealed class AutonomousAdmissionUpdate{public string roomId,prefabPath,beforeSha256,fieldName;}
        [Serializable] private sealed class AutonomousMeasurement
        {public int schemaVersion;public string runId,inputKind,sourceManifestSha256;public AutonomousMeasurementCase[] cases;}
        [Serializable] private sealed class AutonomousMeasurementCase
        {public string roomId,socketId,prefabSha256;public int startTick,endTick,sampleCount;public bool entered,moveReached,jumpObserved,dashObserved,socketObserved,completed;public int unexpectedFailureCount;public string evidenceSha256;}
        [Serializable] private sealed class AutonomousSources{public int schemaVersion;public string runId;public AutonomousSource[] sources;public AutonomousRequirement[] requirements;}
        [Serializable] private sealed class AutonomousSource{public string path;public long byteLength;public string sha256;}
        [Serializable] private sealed class AutonomousRequirement{public string roomId,socketId;public bool movement,jump,dash,socket,exit;}
        // REQ-AUTOCH1-012. Separate intake: the measuring test cannot invoke
        // this with its own result as approval or alter the catalog directly.
        public static void ApplyAutonomousRoomAcceptance()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("측정 실행 중에는 편입하지 않습니다.");
            var workspace=Path.GetFullPath(Path.Combine(Application.dataPath,"../../../.."));
            var admissionPath=Path.Combine(workspace,"preparation/autonomous-ch1-20261021/room-admission.v1.json");
            var admissionBytes=ReadAutonomousInput(admissionPath,workspace,32768);var admission=ReadClosed<AutonomousAdmission>(admissionBytes);
            if(admission.schemaVersion!=1||admission.astraApprovalSection!="REQ/AC-AUTOCH1-012"||admission.updates==null||admission.updates.Length!=6)throw new InvalidOperationException("독립 수용 명세가 유효하지 않습니다.");
            var evidenceRoot=Path.Combine(workspace,"output/chapter1-autonomous-20261021/evidence/room-production");
            var measurementPath=Path.GetFullPath(Path.Combine(workspace,admission.measurementPath));
            var manifestPath=Path.GetFullPath(Path.Combine(workspace,admission.sourceManifestPath));
            if(Path.GetFileName(measurementPath)!="chapter1-room-production-acceptance.v1.json"||Path.GetDirectoryName(manifestPath)!=Path.GetDirectoryName(measurementPath)||Path.GetFileName(manifestPath)!="source-geometry-manifest.v1.json")throw new InvalidOperationException("측정·원문 명세 경로가 다릅니다.");
            var measurementBytes=ReadAutonomousInput(measurementPath,evidenceRoot,1048576);var manifestBytes=ReadAutonomousInput(manifestPath,evidenceRoot,1048576);
            var decisionPath=Path.GetFullPath(Path.Combine(workspace,admission.lunaDecisionPath));var decision=ReadAutonomousInput(decisionPath,Path.Combine(workspace,"docs/verification/autonomous-ch1-20261021"),262144);
            if(ShaBytes(measurementBytes)!=admission.measurementSha256||ShaBytes(manifestBytes)!=admission.sourceManifestSha256||ShaBytes(decision)!=admission.lunaDecisionSha256)throw new InvalidOperationException("독립 검수·측정 원문 지문이 다릅니다.");
            var measurement=ReadClosed<AutonomousMeasurement>(measurementBytes);var manifest=ReadClosed<AutonomousSources>(manifestBytes);
            if(measurement.schemaVersion!=1||manifest.schemaVersion!=1||measurement.inputKind!="synthetic-input-system"||measurement.runId!=manifest.runId||measurement.sourceManifestSha256!=admission.sourceManifestSha256||measurement.cases==null||measurement.cases.Length!=10||manifest.requirements==null||manifest.requirements.Length!=10||manifest.sources==null||manifest.sources.Length==0)throw new InvalidOperationException("측정 명세가 닫힌 열 구성이 아닙니다.");
            var project=Path.GetDirectoryName(Application.dataPath);
            foreach(var source in manifest.sources){var bytes=ReadAutonomousInput(Path.GetFullPath(Path.Combine(project,source.path)),project,16777216);if(bytes.LongLength!=source.byteLength||ShaBytes(bytes)!=source.sha256)throw new InvalidOperationException("측정 이후 생산 원문이 바뀌었습니다: "+source.path);}
            var expected=new HashSet<string>(StringComparer.Ordinal);for(var n=1;n<=6;n++){expected.Add("R0"+n+"/S0"+n+"-A");if(n>1&&n<6)expected.Add("R0"+n+"/S0"+n+"-B");}
            foreach(var item in measurement.cases)
            {
                var key=item.roomId+"/"+item.socketId;if(!expected.Remove(key)||item.startTick<0||item.endTick<item.startTick||item.sampleCount!=checked(item.endTick-item.startTick+1)||!item.entered||!item.moveReached||!item.jumpObserved||!item.dashObserved||!item.socketObserved||!item.completed||item.unexpectedFailureCount!=0)throw new InvalidOperationException("미완료·중복·외부 방 구성은 편입하지 않습니다.");
                var matched=0;foreach(var required in manifest.requirements)if(required.roomId==item.roomId&&required.socketId==item.socketId){if(!required.movement||!required.jump||!required.dash||!required.socket||!required.exit)throw new InvalidOperationException("요구 행동을 축소할 수 없습니다.");matched++;}if(matched!=1)throw new InvalidOperationException("측정 요구 행동이 유일하지 않습니다.");
                var raw=ReadAutonomousInput(Path.Combine(Path.GetDirectoryName(measurementPath),item.roomId+"-"+item.socketId+".samples.csv"),evidenceRoot,1048576);if(ShaBytes(raw)!=item.evidenceSha256)throw new InvalidOperationException("실제 표본 원문이 다릅니다.");
            }
            if(expected.Count!=0)throw new InvalidOperationException("필수 방 구성이 누락됐습니다.");
            var updates=new HashSet<string>(StringComparer.Ordinal);
            foreach(var update in admission.updates)
            {
                if(!updates.Add(update.roomId)||update.roomId==null||!Regex.IsMatch(update.roomId,"^R0[1-6]$")||update.prefabPath!="Assets/Prefabs/Chapter1/"+update.roomId+".prefab"||update.fieldName!="_basicMovementAcceptanceSha")throw new InvalidOperationException("허용 메타데이터 변경 목록이 아닙니다.");
                var bytes=ReadAutonomousInput(Path.Combine(project,update.prefabPath),project,16777216);if(ShaBytes(bytes)!=update.beforeSha256)throw new InvalidOperationException("편입 전 프리팹이 측정 대상과 다릅니다.");
                foreach(var item in measurement.cases)if(item.roomId==update.roomId&&item.prefabSha256!=update.beforeSha256)throw new InvalidOperationException("방 측정 프리팹 지문이 다릅니다.");
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(update.prefabPath);if(prefab==null||prefab.GetComponent<Chapter1RoomInstance>()==null||prefab.GetComponent<Chapter1RoomInstance>().IsBasicMovementAccepted)throw new InvalidOperationException("새 편입 대상이 아닌 프리팹입니다.");
                foreach(var suffix in new[]{"",".meta"})
                {
                    var original=File.ReadAllBytes(Path.Combine(project,update.prefabPath+suffix));var preserved=Path.Combine(workspace,"preparation/autonomous-ch1-20261021/before",update.prefabPath+suffix);
                    if(File.Exists(preserved)){if(ShaBytes(File.ReadAllBytes(preserved))!=ShaBytes(original))throw new InvalidOperationException("보존 원문이 다릅니다.");}
                    else{Directory.CreateDirectory(Path.GetDirectoryName(preserved));using(var file=new FileStream(preserved,FileMode.CreateNew,FileAccess.Write,FileShare.None)){file.Write(original,0,original.Length);file.Flush(true);}}
                }
            }
            var proof=ShaBytes(admissionBytes);
            foreach(var update in admission.updates)
            {
                var root=PrefabUtility.LoadPrefabContents(update.prefabPath);
                try{root.GetComponent<Chapter1RoomInstance>().AcceptBasicMovementForAuthoring(proof);if(PrefabUtility.SaveAsPrefabAsset(root,update.prefabPath)==null)throw new IOException("편입 메타데이터 저장에 실패했습니다.");}
                finally{PrefabUtility.UnloadPrefabContents(root);}
            }
        }
        private static T ReadClosed<T>(byte[] bytes)
        {
            var text=new UTF8Encoding(false,true).GetString(bytes);var value=JsonUtility.FromJson<T>(text);
            if(value==null||JsonUtility.ToJson(value)!=text)throw new InvalidOperationException("정확 필드·순서의 닫힌 JSON이 아닙니다.");return value;
        }
        private static byte[] ReadAutonomousInput(string path,string boundary,int maximum)
        {
            path=Path.GetFullPath(path);boundary=Path.GetFullPath(boundary).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);
            if(!path.StartsWith(boundary+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new IOException("허용 입력 경계를 벗어났습니다.");
            for(var current=path;!string.IsNullOrEmpty(current);current=Path.GetDirectoryName(current))if((File.GetAttributes(current)&FileAttributes.ReparsePoint)!=0)throw new IOException("입력 링크를 거절합니다.");
            using(var file=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read)){if(file.Length<1||file.Length>maximum)throw new IOException("입력 크기 상한을 벗어났습니다.");var bytes=new byte[(int)file.Length];var read=0;while(read<bytes.Length){var count=file.Read(bytes,read,bytes.Length-read);if(count==0)throw new EndOfStreamException();read+=count;}return bytes;}
        }
        private static string ShaBytes(byte[] bytes){using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-",string.Empty);}
        internal const string ScenePath="Assets/Scenes/Chapter1Playable.unity";
        internal const string HubPath="Assets/Prefabs/Chapter1/Hub.prefab";
        internal const string OrdanPath="Assets/Prefabs/Chapter1/OrdanEncounter.prefab";
        private const string PreservedGraph="Assets/Prefabs/Chapter1/Chapter1RegularEncounter.prefab";
        private const string PreservedGraphSha="B16B013217C342805509DCF5C34BFD1B43A3EBA1F16DBDCE386BA465AFA9A2DC";
        private const string PreservedMetaSha="C2F76DD8A3E5B58D599FF9B8B2C8C0A7D3C22958446B46A8A9C7A0F05E679DAF";
        [MenuItem("AcadeGameMaker/Chapter1/Review Basic Movement/R01")] public static void ReviewR01()=>OpenBasicMovementReview("R01");
        [MenuItem("AcadeGameMaker/Chapter1/Review Basic Movement/R02")] public static void ReviewR02()=>OpenBasicMovementReview("R02");
        [MenuItem("AcadeGameMaker/Chapter1/Review Basic Movement/R03")] public static void ReviewR03()=>OpenBasicMovementReview("R03");
        [MenuItem("AcadeGameMaker/Chapter1/Review Basic Movement/R04")] public static void ReviewR04()=>OpenBasicMovementReview("R04");
        [MenuItem("AcadeGameMaker/Chapter1/Review Basic Movement/R05")] public static void ReviewR05()=>OpenBasicMovementReview("R05");
        [MenuItem("AcadeGameMaker/Chapter1/Review Basic Movement/R06")] public static void ReviewR06()=>OpenBasicMovementReview("R06");
        [MenuItem("AcadeGameMaker/Chapter1/Review Basic Movement/R02-B")] public static void ReviewR02B()=>OpenBasicMovementReview("R02","S02-B");
        [MenuItem("AcadeGameMaker/Chapter1/Review Basic Movement/R03-B")] public static void ReviewR03B()=>OpenBasicMovementReview("R03","S03-B");
        [MenuItem("AcadeGameMaker/Chapter1/Review Basic Movement/R04-B")] public static void ReviewR04B()=>OpenBasicMovementReview("R04","S04-B");
        [MenuItem("AcadeGameMaker/Chapter1/Review Basic Movement/R05-B")] public static void ReviewR05B()=>OpenBasicMovementReview("R05","S05-B");
        private static void OpenBasicMovementReview(string id,string requestedSocket=null)
        {
            // §11.5의 저작 검수 모드. 파일을 저장하지 않는 임시 scene이며 RunStarted/방풀 수용을 만들지 않는다.
            VerifyPreserved();var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/"+id+".prefab");
            Chapter1ExpeditionAuthoringValidator.ValidateRoom(prefab,false);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);scene.name="Chapter1BasicMovementReview"+id;
            var host=new GameObject("Chapter1RoomReviewHost");host.SetActive(false);var room=prefab.GetComponent<Chapter1RoomInstance>();
            var socket=requestedSocket??"S"+id.Substring(1)+"-A";
            if(!room.SupportsSocketPlan(socket))throw new InvalidOperationException("승인 기본 이동 소켓이 아닙니다.");
            host.AddComponent<Chapter1EncounterLifetimeOwner>().ConfigureChapter1RoomReviewForAuthoring(prefab,room.Entry,room.Hazard,socket);
            host.SetActive(true);
            Debug.Log("기본 이동 검수 후보 "+id+" · 원문 "+Sha(AssetDatabase.GetAssetPath(prefab))+" · 실제 입력/검수 기록 후 별도 승인 필요");
        }
        internal static void AcceptBasicMovementReviewForAuthoring(string id,string expectedPrefabSha,string evidenceFile,string expectedEvidenceSha)
        {
            // 호출자는 실제 독립 수동 trace와 room 원문을 먼저 검수한다. 해시 존재만으로 검수를 수행했다고 주장하지 않는다.
            var path="Assets/Prefabs/Chapter1/"+id+".prefab";var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Chapter1ExpeditionAuthoringValidator.ValidateRoom(prefab,false);
            if(Sha(path)!=expectedPrefabSha)throw new InvalidOperationException("실제 검수한 방 원문과 현재 후보가 다릅니다.");
            var evidenceRoot=Path.GetFullPath(Path.Combine(Application.dataPath,"..","..","evidence"));
            var evidence=Path.GetFullPath(evidenceFile);
            if(!evidence.StartsWith(evidenceRoot+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase) ||
                !File.Exists(evidence) || (File.GetAttributes(evidence)&FileAttributes.ReparsePoint)!=0 || Sha(evidence)!=expectedEvidenceSha)
                throw new InvalidOperationException("별도 독립 수동 검수 원문 결속이 없습니다.");
            var contents=PrefabUtility.LoadPrefabContents(path);
            try{contents.GetComponent<Chapter1RoomInstance>().AcceptBasicMovementForAuthoring(expectedEvidenceSha);
                if(PrefabUtility.SaveAsPrefabAsset(contents,path)==null)throw new InvalidOperationException("검수 지문 메타 기록에 실패했습니다.");}
            finally{PrefabUtility.UnloadPrefabContents(contents);}
        }
        // REQ-C1B-ROOM-001/HUB-001: 정확 신규 산출만 저장한다. 전체 SaveAssets는 호출하지 않는다.
        [MenuItem("AcadeGameMaker/Chapter1/Build Expedition Candidates")]
        public static void Build()
        {
            VerifyPreserved();
            foreach(var path in Outputs())if(File.Exists(path) || File.Exists(path+".meta"))throw new InvalidOperationException("기존 원정 산출은 덮어쓰지 않습니다: "+path);
            var regular=AssetDatabase.LoadAssetAtPath<GameObject>(PreservedGraph);
            var ordan=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab");
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset");
            if(regular==null || ordan==null || font==null)throw new InvalidOperationException("승인 regular·Ordan·한글 폰트 입력이 없습니다.");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);scene.name="Chapter1ExpeditionAuthoring";
            Chapter1RegularEncounterAuthoringValidator.Validate(regular);
            var plans=new TextAsset[4];var seeds=new[]{101,202,303,404};
            for(var i=0;i<4;i++){plans[i]=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Chapter1/RoomPlans/"+seeds[i]+".json");if(plans[i]==null)throw new InvalidOperationException("정수 계획 원문 입력이 없습니다.");}
            var roomPrefabs=new GameObject[6];
            for(var i=0;i<6;i++)roomPrefabs[i]=BuildRoom(regular,"R0"+(i+1));
            var bossStage=new GameObject("InactiveOrdanAuthoringStage");bossStage.SetActive(false);
            var boss=UnityEngine.Object.Instantiate(ordan,bossStage.transform,false);boss.SetActive(false);boss.name="OrdanEncounter";
            try{Chapter1ExpeditionAuthoringValidator.ValidateBoss(boss);if(PrefabUtility.SaveAsPrefabAsset(boss,OrdanPath)==null)throw new InvalidOperationException("Ordan 신규 prefab 저장 실패입니다.");}finally{UnityEngine.Object.DestroyImmediate(bossStage);}
            var hub=new GameObject("Chapter1Hub");hub.SetActive(false);
            try
            {
                var router=hub.AddComponent<InputRouter>();router.ConfigureHubUiOnlyForAuthoring();
                var adapter=hub.AddComponent<DesktopProfileLaunchAdapterV1>();adapter.ConfigureChapter1DevelopmentRootForAuthoring(router,Chapter1DevelopmentProfile.ExactRoot);
                hub.AddComponent<Chapter1HubLifetimeOwner>().ConfigureForAuthoring(router,adapter,1);
                Chapter1ExpeditionAuthoringValidator.ValidateHub(hub);
                if(PrefabUtility.SaveAsPrefabAsset(hub,HubPath)==null)throw new InvalidOperationException("거점 신규 prefab 저장 실패입니다.");
            }
            finally{UnityEngine.Object.DestroyImmediate(hub);}
            var root=new GameObject("Chapter1Expedition");root.SetActive(false);
            var catalog=root.AddComponent<Chapter1RoomCatalog>();catalog.ConfigureForAuthoring(roomPrefabs,plans);
            var lifetime=root.AddComponent<Chapter1EncounterLifetimeOwner>();var flow=root.AddComponent<Chapter1PlayableFlowOwner>();
            flow.ConfigureForAuthoring(AssetDatabase.LoadAssetAtPath<GameObject>(HubPath),AssetDatabase.LoadAssetAtPath<GameObject>(OrdanPath),catalog,lifetime);
            BuildPresentation(root,flow,font);
            root.SetActive(true);
            if(!EditorSceneManager.SaveScene(scene,ScenePath))throw new InvalidOperationException("신규 원정 scene 저장 실패입니다.");
            Chapter1ExpeditionAuthoringValidator.ValidateScene(scene);VerifyPreserved();
            // 검수 전 방은 후보다. 실제 출정의 catalog.Select는 기본 이동 수용을 별도로 요구한다.
        }
        internal const string ChoicePath="Assets/Prefabs/Chapter1/Choice.prefab";
        internal const string BarrierPath="Assets/Prefabs/Chapter1/Barrier.prefab";
        internal const string ClosingPath="Assets/Prefabs/Chapter1/Closing.prefab";
        // REQ/AC-AUTOCH1-009/011: 기존 후보 생성과 분리된 현재 CH1 생산 저작 입구.
        public static void BuildAutonomousChapter1()
        {
            VerifyPreserved();
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular-SDF.asset");
            var closing=AssetDatabase.LoadAssetAtPath<GameObject>(ClosingPath);
            if(font==null || closing==null || closing.GetComponent<Chapter1ClosingPresenter>()==null || closing.activeSelf)
                throw new InvalidOperationException("승인된 한글 폰트·정적 종결 프리팹이 없습니다.");
            UpdateChapter1Font(font);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);scene.name="Chapter1Playable";
            var choice=BuildChoice(font);var barrier=BuildBarrier();
            var rooms=new GameObject[6];for(var i=0;i<6;i++)
            {rooms[i]=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/R0"+(i+1)+".prefab");Chapter1ExpeditionAuthoringValidator.ValidateRoom(rooms[i],false);}
            var plans=new TextAsset[4];var seeds=new[]{101,202,303,404};
            for(var i=0;i<4;i++){plans[i]=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Chapter1/RoomPlans/"+seeds[i]+".json");if(plans[i]==null)throw new InvalidOperationException("승인 방 계획이 없습니다.");}
            var hub=AssetDatabase.LoadAssetAtPath<GameObject>(HubPath);var boss=AssetDatabase.LoadAssetAtPath<GameObject>(OrdanPath);
            Chapter1ExpeditionAuthoringValidator.ValidateHub(hub);Chapter1ExpeditionAuthoringValidator.ValidateBoss(boss);
            var root=new GameObject("Chapter1Expedition");root.SetActive(false);
            var catalog=root.AddComponent<Chapter1RoomCatalog>();catalog.ConfigureForAuthoring(rooms,plans);
            var lifetime=root.AddComponent<Chapter1EncounterLifetimeOwner>();var flow=root.AddComponent<Chapter1PlayableFlowOwner>();
            flow.ConfigureForAuthoring(hub,boss,catalog,lifetime);
            flow.ConfigureChapter1PostBossForAuthoring(choice,barrier,closing);
            BuildPresentation(root,flow,font);root.SetActive(true);
            if(!EditorSceneManager.SaveScene(scene,ScenePath))throw new InvalidOperationException("현재 CH1 장면 저장에 실패했습니다.");
            var buildScenes=new List<EditorBuildSettingsScene>();var found=false;
            foreach(var item in EditorBuildSettings.scenes){if(item.path==ScenePath){if(!found)buildScenes.Add(new EditorBuildSettingsScene(ScenePath,true));found=true;}else buildScenes.Add(item);}
            if(!found)buildScenes.Add(new EditorBuildSettingsScene(ScenePath,true));EditorBuildSettings.scenes=buildScenes.ToArray();
            Chapter1ExpeditionAuthoringValidator.ValidateScene(scene);VerifyPreserved();
        }
        private static void UpdateChapter1Font(TMP_FontAsset font)
        {
            // REQ/AC-AUTOCH1-011: 주석을 제외한 현재 생산 문자열과 승인 서사 원문만 글리프 입력으로 삼는다.
            var characters=new SortedSet<char>();
            var paths=new[]{"HubPresentation/Unity/Chapter1ExpeditionPresenter.cs","HubPresentation/Unity/Chapter1ChoicePresenter.cs","HubPresentation/Unity/Chapter1ClosingPresenter.cs","Run/Unity/Chapter1PlayableFlowOwner.cs","Input/Unity/DesktopProfileLaunchAdapterV1.cs","Input/Unity/InputRouter.cs"};
            var lexer=new Regex(@"//[^\r\n]*|/\*[\s\S]*?\*/|@""(?:""""|[^""])*""|""(?:\\.|[^""\\])*""");
            foreach(var path in paths)
            {
                var source=File.ReadAllText("Assets/AcadeGameMaker/Runtime/"+path);
                foreach(Match token in lexer.Matches(source))
                {
                    if(token.Value.StartsWith("//",StringComparison.Ordinal)||token.Value.StartsWith("/*",StringComparison.Ordinal))continue;
                    var text=token.Value.StartsWith("@",StringComparison.Ordinal)?token.Value.Substring(2,token.Value.Length-3).Replace("\"\"","\""):Regex.Unescape(token.Value.Substring(1,token.Value.Length-2));
                    foreach(var c in text)if(!char.IsControl(c))characters.Add(c);
                }
            }
            foreach(var c in File.ReadAllText("Assets/Chapter1/Narrative/Chapter1Closing.json"))if(!char.IsControl(c))characters.Add(c);
            foreach(var c in "수탈연대선택확정취소저장다시시도▶")characters.Add(c);
            var copy=new StringBuilder();foreach(var c in characters)if(!HasChapter1Glyph(font,c))copy.Append(c);
            if(copy.Length==0)
            {
                Debug.Log("CH1 실제 문자열 글리프 집합: "+characters.Count+"자, 기존 글리프 모두 확인");
                return;
            }
            var sourceFont=AssetDatabase.LoadAssetAtPath<Font>("Assets/UI/Fonts/Hub/NotoSansCJKkr-Regular.otf");
            if(sourceFont==null)throw new InvalidOperationException("기존 한글 원본 폰트가 없습니다.");
            var previousMode=font.atlasPopulationMode;
            try
            {
                font.atlasPopulationMode=AtlasPopulationMode.Dynamic;
                var serializedFont=new SerializedObject(font);serializedFont.FindProperty("m_SourceFontFile").objectReferenceValue=sourceFont;serializedFont.ApplyModifiedPropertiesWithoutUndo();
                font.TryAddCharacters(copy.ToString(),out var missing);
                  if(!string.IsNullOrEmpty(missing))throw new InvalidOperationException("CH1 글리프를 모두 저작하지 못했습니다: "+missing);
                  foreach(var c in characters)if(!HasChapter1Glyph(font,c))throw new InvalidOperationException("CH1 최종 글리프 검증에 실패했습니다: "+c);
            }
            finally{font.atlasPopulationMode=previousMode;EditorUtility.SetDirty(font);AssetDatabase.SaveAssetIfDirty(font);}
            foreach(var atlas in font.atlasTextures){EditorUtility.SetDirty(atlas);AssetDatabase.SaveAssetIfDirty(atlas);}
            Debug.Log("CH1 실제 문자열 글리프 집합: "+characters.Count+"자");
        }
          private static bool HasChapter1Glyph(TMP_FontAsset font,char character)
          {
              if(!font.HasCharacter(character,false,false))return false;
              var entry=font.characterLookupTable[character];var glyph=entry.glyph;
              if(glyph==null || !font.glyphLookupTable.TryGetValue(entry.glyphIndex,out var registered) || !ReferenceEquals(glyph,registered))
                  throw new InvalidOperationException("CH1 글리프 원본 결속이 손상됐습니다: "+character);
              var atlases=font.atlasTextures;var index=glyph.atlasIndex;
              if(atlases==null || index<0 || index>=atlases.Length || atlases[index]==null)
                  throw new InvalidOperationException("CH1 글리프 아틀라스가 없습니다: "+character);
              var rect=glyph.glyphRect;var atlas=atlases[index];
              if(!Chapter1GlyphRectangleFits(rect.x,rect.y,rect.width,rect.height,atlas.width,atlas.height,char.IsWhiteSpace(character)))
                  throw new InvalidOperationException("CH1 글리프 아틀라스 범위가 다릅니다: "+character);
              return true;
          }
          private static bool Chapter1GlyphRectangleFits(int x,int y,int width,int height,int atlasWidth,int atlasHeight,bool whitespace)
          {
              return x>=0 && y>=0 && width>=0 && height>=0 && atlasWidth>0 && atlasHeight>0 &&
                  (whitespace || (width>0 && height>0)) && (long)x+width<=atlasWidth && (long)y+height<=atlasHeight;
          }
        private static GameObject BuildChoice(TMP_FontAsset font)
        {
            var root=new GameObject("Choice");root.SetActive(false);
            try
            {
                var canvas=new GameObject("Canvas",typeof(RectTransform),typeof(Canvas));canvas.transform.SetParent(root.transform,false);canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
                var safe=new GameObject("SafeFrame",typeof(RectTransform));safe.transform.SetParent(canvas.transform,false);
                var rect=safe.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.sizeDelta=new Vector2(640,360);
                var scaler=root.AddComponent<HubSafeFrameScalerV1>();scaler.ConfigureForAuthoring(rect);
                var status=Text(safe.transform,"Status",new Vector2(0,75),new Vector2(600,180),font,15);status.alignment=TextAlignmentOptions.TopLeft;
                var labels=new TMP_Text[5];var buttons=new RectTransform[5];var copy=new[]{"수탈","연대","선택 확정","취소","저장 다시 시도"};
                for(var i=0;i<5;i++){labels[i]=Text(safe.transform,"Choice"+i,new Vector2(0,-35-i*29),new Vector2(260,28),font,14);labels[i].text=copy[i];buttons[i]=labels[i].rectTransform;}
                root.AddComponent<Chapter1ChoicePresenter>().ConfigureStaticForAuthoring(status,labels,buttons,scaler);
                Chapter1ExpeditionAuthoringValidator.ValidateChoice(root);
                var saved=PrefabUtility.SaveAsPrefabAsset(root,ChoicePath);if(saved==null)throw new InvalidOperationException("선택 프리팹 저장 실패입니다.");return saved;
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        private static GameObject BuildBarrier()
        {
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(PreservedGraph);var root=new GameObject("Barrier");root.SetActive(false);
            try
            {
                // 생산 일반방의 적·피해·입력 소유자는 복제하지 않는다. 플레이어와 카메라 정적 구성만 재사용한다.
                var sourcePlayer=source.GetComponentInChildren<PlayerMovementController>(true);
                var playerObject=UnityEngine.Object.Instantiate(sourcePlayer.gameObject,root.transform,false);playerObject.name="Player";playerObject.transform.localPosition=new Vector3(0,.81f,0);
                var player=playerObject.GetComponent<PlayerMovementController>();
                var cameraObject=UnityEngine.Object.Instantiate(source.GetComponentInChildren<SimulationCameraDriver>(true).gameObject,root.transform,false);cameraObject.name="Camera";
                var camera=cameraObject.GetComponent<SimulationCameraDriver>();camera.ConfigureForAuthoring(player,cameraObject.GetComponent<UnityEngine.Camera>(),cameraObject.GetComponent<UnityEngine.Rendering.Universal.PixelPerfectCamera>(),0,81,new CameraCenterBounds(0,0,81,81));
                var systems=new GameObject("Systems");systems.transform.SetParent(root.transform,false);
                systems.AddComponent<InputRouter>();var transfer=systems.AddComponent<TransferSimulationDriver>();var targets=systems.AddComponent<TransferTargetRegistry>();
                var registry=systems.AddComponent<CombatTargetRegistry>();systems.AddComponent<BarrierCombatSimulationDriver>();systems.AddComponent<Chapter1ChoiceEnvironmentOwner>();
                Box(root.transform,"LeftFloor",new Rect(-3,-1,4,1));Box(root.transform,"RightFloor",new Rect(4.5f,-1,4.5f,1));Box(root.transform,"CounterweightSupport",new Rect(4,-1,1,1));
                var weight=BarrierBox(root.transform,"Counterweight",new Vector2(4.5f,.5f),Vector2.one,9);
                var body=weight.gameObject.AddComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Dynamic;body.mass=1;body.gravityScale=1;body.constraints=RigidbodyConstraints2D.FreezeRotation;
                var sink=weight.gameObject.AddComponent<Chapter1CounterweightOwnerSink>();sink.ConfigureForAuthoring("BarrierCounterweight",body);
                var target=weight.gameObject.AddComponent<TransferTarget>();target.ConfigureForAuthoring("BarrierCounterweight",TransferTargetKind.Box,0,0,0,0,500,500,"Box.Base.v1",weight,weight.transform,sink);
                var boxCombat=weight.gameObject.AddComponent<CombatTarget>();boxCombat.ConfigureForAuthoring("BarrierCounterweight",8,0,false,0,0,0,0,500,500,weight,weight.transform,target);
                var gate=BarrierBox(root.transform,"Gate",new Vector2(5.1f,1.5f),new Vector2(.2f,3),8);
                var gateCombat=gate.gameObject.AddComponent<CombatTarget>();gateCombat.ConfigureForAuthoring("BarrierGate-Fractured",2,0,false,0,0,0,0,100,1500,gate,gate.transform);
                var bridge=BarrierBox(root.transform,"Bridge",new Vector2(2.75f,-.125f),new Vector2(3.5f,.25f),8);bridge.enabled=false;
                var platform=BarrierBox(root.transform,"Platform",new Vector2(2.25f,-.125f),new Vector2(4.5f,.25f),8);platform.enabled=false;platform.usedByEffector=true;
                var effector=platform.gameObject.AddComponent<PlatformEffector2D>();effector.useOneWay=true;effector.useColliderMask=true;effector.colliderMask=1<<playerObject.layer;
                // 레일은 상자 가장자리 바깥이며 LOS 전용 8번 계층에서 제외한다.
                BarrierBox(root.transform,"CounterweightRailLeft",new Vector2(3.95f,1.5f),new Vector2(.1f,3),9).excludeLayers=1<<playerObject.layer;
                BarrierBox(root.transform,"CounterweightRailRight",new Vector2(5.05f,1.5f),new Vector2(.1f,3),9).excludeLayers=1<<playerObject.layer;
                var exit=BarrierBox(root.transform,"Exit",new Vector2(7,1.5f),new Vector2(1,3),9);exit.isTrigger=true;
                var safety=BarrierBox(root.transform,"Safety",new Vector2(3,-3),new Vector2(12,2),9);safety.isTrigger=true;
                targets.ConfigureForAuthoring(target);transfer.ConfigureForAuthoring(player,targets);
                var playerCombat=playerObject.GetComponent<CombatTarget>();if(playerCombat==null)throw new InvalidOperationException("기존 플레이어 피해 등록이 없습니다.");
                registry.ConfigureForAuthoring(boxCombat,gateCombat,playerCombat);
                Chapter1ExpeditionAuthoringValidator.ValidateBarrier(root);
                var saved=PrefabUtility.SaveAsPrefabAsset(root,BarrierPath);if(saved==null)throw new InvalidOperationException("봉쇄선 프리팹 저장 실패입니다.");return saved;
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        private static BoxCollider2D BarrierBox(Transform parent,string name,Vector2 center,Vector2 size,int layer)
        {var value=new GameObject(name){layer=layer};value.transform.SetParent(parent,false);value.transform.localPosition=center;var box=value.AddComponent<BoxCollider2D>();box.size=size;return box;}
        private static GameObject BuildRoom(GameObject preserved,string id)
        {
            var graph=UnityEngine.Object.Instantiate(preserved);graph.SetActive(false);graph.name=id;
            try
            {
                var meta=graph.AddComponent<Chapter1RoomInstance>();meta.ConfigureForAuthoring(id);
                var environment=graph.transform.Find("Environment");if(environment==null)throw new InvalidOperationException("기존 환경 결속이 없습니다.");
                while(environment.childCount!=0)UnityEngine.Object.DestroyImmediate(environment.GetChild(0).gameObject);
                var ordinal=0;foreach(var rect in meta.Floors)Box(environment,"Floor"+ordinal++,rect);
                ordinal=0;foreach(var rect in meta.Terrain)Box(environment,"Terrain"+ordinal++,rect);
                Box(environment,"LeftBound",meta.LeftBound);Box(environment,"RightBound",meta.RightBound);
                var socket="S"+id.Substring(1)+"-A";
                graph.transform.Find("Walker").localPosition=meta.WalkerSocket(socket);
                graph.transform.Find("Surveyor").localPosition=meta.SurveyorSocket(socket);
                var camera=graph.GetComponentInChildren<SimulationCameraDriver>(true);
                var unityCamera=graph.GetComponentInChildren<UnityEngine.Camera>(true);
                var pixel=graph.GetComponentInChildren<UnityEngine.Rendering.Universal.PixelPerfectCamera>(true);
                // 640×360/PPU18 viewport의 반폭·반높이를 정확 room 영역에서 제외한 center 격자다.
                camera.ConfigureForAuthoring(graph.GetComponentInChildren<PlayerMovementController>(true),unityCamera,pixel,248,0,new CameraCenterBounds(248,328,0,72));
                Chapter1ExpeditionAuthoringValidator.ValidateRoom(graph,false);
                var path="Assets/Prefabs/Chapter1/"+id+".prefab";
                var prefab=PrefabUtility.SaveAsPrefabAsset(graph,path);if(prefab==null)throw new InvalidOperationException("방 후보 저장 실패입니다.");return prefab;
            }
            finally{UnityEngine.Object.DestroyImmediate(graph);}
        }
        private static void Box(Transform parent,string name,Rect rect)
        {var value=new GameObject(name){layer=8};value.transform.SetParent(parent,false);value.transform.localPosition=rect.center;value.AddComponent<BoxCollider2D>().size=rect.size;}
        private static void BuildPresentation(GameObject root,Chapter1PlayableFlowOwner flow,TMP_FontAsset font)
        {
            var canvas=new GameObject("Canvas",typeof(RectTransform),typeof(Canvas));canvas.transform.SetParent(root.transform,false);canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
            var safe=new GameObject("SafeFrame",typeof(RectTransform));safe.transform.SetParent(canvas.transform,false);
            var rect=safe.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.sizeDelta=new Vector2(640,360);
            var scaler=root.AddComponent<HubSafeFrameScalerV1>();scaler.ConfigureForAuthoring(rect);
            var status=Text(safe.transform,"Status",new Vector2(0,115),new Vector2(620,90),font,16);
            var buttons=new RectTransform[6];var labels=new TMP_Text[6];
            for(var i=0;i<6;i++){labels[i]=Text(safe.transform,"Intent"+i,new Vector2(0,45-i*30),new Vector2(260,28),font,14);buttons[i]=labels[i].rectTransform;}
            root.AddComponent<Chapter1ExpeditionPresenter>().ConfigureForAuthoring(flow,status,labels,buttons,scaler);
        }
        private static TMP_Text Text(Transform parent,string name,Vector2 position,Vector2 size,TMP_FontAsset font,float points)
        {var value=new GameObject(name,typeof(RectTransform));value.transform.SetParent(parent,false);var text=value.AddComponent<TextMeshProUGUI>();text.font=font;text.fontSize=points;text.color=Color.white;text.alignment=TextAlignmentOptions.Center;text.raycastTarget=false;
            var rect=text.rectTransform;rect.anchorMin=rect.anchorMax=new Vector2(.5f,.5f);rect.anchoredPosition=position;rect.sizeDelta=size;return text;}
        private static string[] Outputs()=>new[]{ScenePath,HubPath,OrdanPath,"Assets/Prefabs/Chapter1/R01.prefab","Assets/Prefabs/Chapter1/R02.prefab","Assets/Prefabs/Chapter1/R03.prefab","Assets/Prefabs/Chapter1/R04.prefab","Assets/Prefabs/Chapter1/R05.prefab","Assets/Prefabs/Chapter1/R06.prefab"};
        private static void VerifyPreserved(){if(Sha(PreservedGraph)!=PreservedGraphSha || Sha(PreservedGraph+".meta")!=PreservedMetaSha)throw new InvalidOperationException("보존 graph/meta 지문이 다릅니다.");}
        private static string Sha(string path){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(File.ReadAllBytes(path))).Replace("-","");}
    }
}
