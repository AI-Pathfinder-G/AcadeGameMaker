#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Run.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.RunUnity
{
    // REQ/AC-AUTOCH1-012. This measures candidates; it never edits their acceptance metadata.
    internal sealed class Chapter1RoomProductionAcceptanceTests:InputTestFixture
    {
        [Serializable] private sealed class Case
        {
            public string roomId,socketId,prefabSha256;
            public int startTick,endTick,sampleCount;
            public bool entered,moveReached,jumpObserved,dashObserved,socketObserved,completed;
            public int unexpectedFailureCount;
            public string evidenceSha256;
        }
        [Serializable] private sealed class Result
        {public int schemaVersion=1;public string runId,inputKind="synthetic-input-system",sourceManifestSha256;public Case[] cases;}
        [Serializable] private sealed class Source
        {public string path;public long byteLength;public string sha256;}
        [Serializable] private sealed class Requirement
        {public string roomId,socketId;public bool movement=true,jump=true,dash=true,socket=true,exit=true;}
        [Serializable] private sealed class Manifest
        {public int schemaVersion=1;public string runId;public Source[] sources;public Requirement[] requirements;}
        private sealed class Port:IChapter1BossProgressionPort
        {public bool BlocksNewFailureRequests=>false;public bool ValidatePendingFailure(Chapter1EncounterLifetimeOwner owner,Chapter1AcceptedFailureToken token)=>false;public void ConsumePendingFailure(Chapter1EncounterLifetimeOwner owner,Chapter1AcceptedFailureToken token){throw new InvalidOperationException("측정은 실패를 정상 진행으로 소비하지 않습니다.");}}
        private Keyboard _keyboard;private Mouse _mouse;private Scene _scene;private GameObject _host;
        public override void TearDown(){Chapter1AuthoredInputTrace.LogStage("rooms.fixture-restore-deferred");}
        [UnityTearDown] public IEnumerator Close()
        {
            try{Chapter1AuthoredInputTrace.LogStage("rooms.close-before");if(_host!=null){_host.SetActive(false);UnityEngine.Object.Destroy(_host);}if(_scene.IsValid()&&_scene.isLoaded){foreach(var root in _scene.GetRootGameObjects())root.SetActive(false);yield return SceneManager.UnloadSceneAsync(_scene);if(_scene.isLoaded)throw new InvalidOperationException("측정 장면 정리가 끝나지 않았습니다.");}Chapter1AuthoredInputTrace.LogStage("rooms.unloaded");}
            finally{Chapter1AuthoredInputTrace.LogStage("rooms.fixture-restore-before");base.TearDown();Chapter1AuthoredInputTrace.LogStage("rooms.fixture-restore-after");}
        }
        [UnityTest] public IEnumerator MeasureAllTenProductionConfigurations()
        {yield return MeasureConfigurations(false);}
        [UnityTest] public IEnumerator MeasureFirstTwoProductionConfigurationsWithNeutralBoundary()
        {yield return MeasureConfigurations(true);}
        private IEnumerator MeasureConfigurations(bool firstTwoOnly)
        {
            Chapter1AuthoredInputTrace.ResetStages();Chapter1AuthoredInputTrace.LogStage("rooms.begin");
            var runId="rooms-"+Guid.NewGuid().ToString("N");DesktopProfileLaunchAdapterV1.ConfigureChapter1TestRun(runId);
            var workspace=Path.GetFullPath(Path.Combine(Application.dataPath,"../../../.."));
            var output=Path.Combine(workspace,"output/chapter1-autonomous-20261021/evidence/room-production",runId);
            RequireNoLinks(output);if(Directory.Exists(output)||File.Exists(output))throw new IOException("측정 실행 경로가 이미 존재합니다.");Directory.CreateDirectory(output);
            var requirements=new List<Requirement>();for(var n=1;n<=6;n++){requirements.Add(new Requirement{roomId="R0"+n,socketId="S0"+n+"-A"});if(n>1&&n<6)requirements.Add(new Requirement{roomId="R0"+n,socketId="S0"+n+"-B"});}
            if(firstTwoOnly)requirements.RemoveRange(2,requirements.Count-2);
            var project=Path.GetDirectoryName(Application.dataPath);var sourcePaths=new List<string>();
            foreach(var extension in new[]{"*.cs","*.asmdef"})foreach(var path in Directory.GetFiles(Path.Combine(Application.dataPath,"AcadeGameMaker/Runtime"),extension,SearchOption.AllDirectories))sourcePaths.Add(path);
            sourcePaths.Add(Path.Combine(Application.dataPath,"AcadeGameMaker/Tests/PlayMode/RunUnity/Chapter1RoomProductionAcceptanceTests.cs"));
            sourcePaths.Add(Path.Combine(Application.dataPath,"AcadeGameMaker/Tests/PlayMode/RunUnity/Chapter1ExpeditionPlayModeTests.cs"));
            for(var n=1;n<=6;n++)foreach(var path in AssetDatabase.GetDependencies("Assets/Prefabs/Chapter1/R0"+n+".prefab",true))if(path.StartsWith("Assets/",StringComparison.Ordinal))sourcePaths.Add(Path.Combine(project,path));
            foreach(var path in new[]{"Packages/manifest.json","Packages/packages-lock.json","ProjectSettings/ProjectVersion.txt"})sourcePaths.Add(Path.Combine(project,path));
            sourcePaths.Sort(StringComparer.Ordinal);var sources=new List<Source>();var seen=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach(var path in sourcePaths){if(!seen.Add(path)||Directory.Exists(path))continue;var bytes=File.ReadAllBytes(path);sources.Add(new Source{path=path.Substring(project.Length+1).Replace('\\','/'),byteLength=bytes.Length,sha256=Sha(bytes)});}
            var manifest=Encoding.UTF8.GetBytes(JsonUtility.ToJson(new Manifest{runId=runId,sources=sources.ToArray(),requirements=requirements.ToArray()}));WriteNew(Path.Combine(output,"source-geometry-manifest.v1.json"),manifest);
            _keyboard=InputSystem.AddDevice<Keyboard>();_mouse=InputSystem.AddDevice<Mouse>();
            _scene=EditorSceneManager.LoadSceneInPlayMode("Assets/Scenes/Chapter1Playable.unity",new LoadSceneParameters(LoadSceneMode.Additive));
            Chapter1AuthoredInputTrace.LogStage("rooms.scene-loaded");
            yield return null;
            foreach(var root in _scene.GetRootGameObjects())root.SetActive(false);
            var results=new List<Case>();
            GameObject precedingGraph=null;
            foreach(var requirement in requirements)
            {
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/"+requirement.roomId+".prefab");Assert.That(prefab,Is.Not.Null);
                var room=prefab.GetComponent<Chapter1RoomInstance>();Assert.That(room,Is.Not.Null);
                var entry=new Case{roomId=requirement.roomId,socketId=requirement.socketId,prefabSha256=Sha(File.ReadAllBytes(Path.Combine(project,AssetDatabase.GetAssetPath(prefab))))};
                var raw=new StringBuilder();var watch=System.Diagnostics.Stopwatch.StartNew();var bodyCompleted=false;
                try
                {
                Assert.That(precedingGraph==null,Is.True,"이전 생존 그래프가 아직 제거되지 않았습니다.");
                var neutral=AwaitOwnedNeutral(requirement.roomId+"-"+requirement.socketId,watch);
                try{while(neutral.MoveNext())yield return neutral.Current;}finally{(neutral as IDisposable)?.Dispose();}
                _host=new GameObject("ProductionRoomMeasurement");SceneManager.MoveGameObjectToScene(_host,_scene);var lifetime=_host.AddComponent<Chapter1EncounterLifetimeOwner>();
                lifetime.BeginChapter1Expedition(prefab,runId+"-"+requirement.socketId,room.Entry,room.Hazard,requirement.socketId,new Port());
                Chapter1AuthoredInputTrace.LogStage("rooms.begin-"+requirement.roomId+"-"+requirement.socketId);
                var graph=lifetime.CurrentRouter.transform.parent;var walker=graph.Find("Walker");var surveyor=graph.Find("Surveyor");
                precedingGraph=graph.gameObject;
                entry.socketObserved=walker!=null&&surveyor!=null&&new Vector2(walker.localPosition.x,walker.localPosition.y)==room.WalkerSocket(requirement.socketId)&&new Vector2(surveyor.localPosition.x,surveyor.localPosition.y)==room.SurveyorSocket(requirement.socketId);
                var last=-1;
                ValidateEntryDashGeometry(room,requirement.socketId);
                var entryDashIssued=false;var entryDashReleased=false;
                var pilot=new Chapter1ProductionRoomPilot(lifetime.CurrentRouter);
                for(var ordinal=0;ordinal<3000;ordinal++)
                {
                    if(watch.Elapsed.TotalSeconds>=120)break;
                    var router=lifetime.CurrentRouter;if(router==null||lifetime.IsFailed){entry.unexpectedFailureCount++;break;}
                    var command=pilot.Read(ordinal,true);
                    var issuedKeys=command.Keys;var issuedAttack=command.Attack;var entryDashThisTick=false;
                    if(!entryDashIssued)
                    {
                        issuedKeys=Array.Empty<Key>();issuedAttack=false;
                        if(last>=0)
                        {
                            var before=router.BoundPlayerForAuthoring.Snapshot;var receipt=router.CurrentReceipt;
                            var ready=receipt.HasValue && receipt.Value.Mode==AcadeGameMaker.Core.InputMode.GameplayEnabled && receipt.Value.Tick.Value==last;
                            Assert.That(Math.Abs(before.PositionXQ4096/4096f),Is.LessThanOrEqualTo(.1f));
                            if(ShouldIssueEntryDash(ready,before.IsGrounded,!_keyboard.leftShiftKey.isPressed))
                            {
                                issuedKeys=new[]{Key.D,Key.LeftShift};entryDashIssued=true;entryDashThisTick=true;
                            }
                            else Assert.That(last,Is.LessThan(30),"입구의 실제 지면 접촉을 얻지 못했습니다.");
                        }
                    }
                    else if(!entryDashReleased){issuedKeys=Array.Empty<Key>();issuedAttack=false;entryDashReleased=true;}
                    InputSystem.QueueStateEvent(_keyboard,new KeyboardState(issuedKeys));var state=new MouseState{position=command.Pointer};if(issuedAttack)state=state.WithButton(MouseButton.Left);InputSystem.QueueStateEvent(_mouse,state);InputSystem.Update();
                    yield return new WaitForFixedUpdate();
                    if(ordinal==0)Chapter1AuthoredInputTrace.LogStage("rooms.first-fixed");
                    if(!lifetime.TryGetCompletedEvidence(out var view)||view.Tick.Value==last)continue;
                    Assert.That(view.Tick.Value,Is.EqualTo(last+1));last=view.Tick.Value;if(!entry.entered){entry.startTick=last;entry.entered=true;}
                    entry.endTick=last;entry.sampleCount++;var movement=router.BoundPlayerForAuthoring.Snapshot;
                    var actualCombat=graph.GetComponentInChildren<AcadeGameMaker.Combat.Unity.CombatSimulationDriver>(true).LatestOutcomeForEncounterHandoff;
                    Assert.That(actualCombat.Tick.Value,Is.EqualTo(last));
                    entry.moveReached|=view.Position.x>=2;entry.jumpObserved|=!movement.IsGrounded&&movement.VelocityYQ4096>0;entry.dashObserved|=movement.ActionState==MovementActionState.Dashing;
                    raw.Append(last).Append(',').Append(movement.PositionXQ4096).Append(',').Append(movement.PositionYQ4096).Append(',').Append(view.Health).Append(',').Append((int)movement.ActionState).Append(',').Append(view.AliveEnemyCount)
                        .Append(',').Append(string.Join("+",issuedKeys)).Append(',').Append(issuedAttack?1:0).Append(",0,")
                        .Append(command.Pointer.x.ToString("R",System.Globalization.CultureInfo.InvariantCulture)).Append(',').Append(command.Pointer.y.ToString("R",System.Globalization.CultureInfo.InvariantCulture))
                        .Append(',').Append(command.ObservedTick).Append(',').Append(command.Target).Append(',').Append(command.TargetHealth).Append(',').Append(command.AttackCode)
                        .Append(',').Append(actualCombat.Tick.Value).Append(',').Append(actualCombat.Attack.Attempt.HasValue?actualCombat.Attack.Attempt.Value.Code.ToString():"none")
                        .Append(',').Append(actualCombat.Attack.Selection.TargetId).Append(',').Append(entryDashThisTick?1:0).Append('\n');
                    if(view.Health<=0||view.Position.y< -8){entry.unexpectedFailureCount++;break;}
                    if(lifetime.CurrentHazards.TryReadPublishedHazardsForAuthoring(out var kill,out var crush)&&(kill.Triggered||crush.Triggered)){entry.unexpectedFailureCount++;break;}
                    if(room.Exit.Contains(view.Position)&&view.AliveEnemyCount==0){entry.completed=true;break;}
                }
                bodyCompleted=true;
                }
                finally
                {
                    try{var bytes=Encoding.UTF8.GetBytes(raw.ToString());entry.evidenceSha256=Sha(bytes);WriteNew(Path.Combine(output,requirement.roomId+"-"+requirement.socketId+".samples.csv"),bytes);}
                    catch(Exception recordError){if(bodyCompleted)throw;Debug.Log("[CH1CaseRecordFailure] primary exception retained; "+recordError.GetType().Name+":"+recordError.Message);}
                }
                results.Add(entry);
                InputSystem.QueueStateEvent(_keyboard,new KeyboardState());InputSystem.QueueStateEvent(_mouse,new MouseState());InputSystem.Update();
                UnityEngine.Object.Destroy(_host);_host=null;yield return null;yield return new WaitForFixedUpdate();
                Debug.Log("방 생산 측정: "+JsonUtility.ToJson(entry));
                Assert.That(entry.unexpectedFailureCount,Is.Zero);Assert.That(entry.entered&&entry.moveReached&&entry.jumpObserved&&entry.dashObserved&&entry.socketObserved&&entry.completed,Is.True,requirement.roomId+"/"+requirement.socketId);
            }
            Assert.That(results.Count,Is.EqualTo(firstTwoOnly?2:10));
            WriteNew(Path.Combine(output,firstTwoOnly?"chapter1-room-minimal-observation.v1.json":"chapter1-room-production-acceptance.v1.json"),Encoding.UTF8.GetBytes(JsonUtility.ToJson(new Result{runId=runId,sourceManifestSha256=Sha(manifest),cases=results.ToArray()})));
        }
        private IEnumerator AwaitOwnedNeutral(string caseId,System.Diagnostics.Stopwatch caseWatch)
        {
            var keyboard=_keyboard;var mouse=_mouse;var keyboardId=keyboard.deviceId;var mouseId=mouse.deviceId;
            var before=InputState.updateCount;var neutralWatch=System.Diagnostics.Stopwatch.StartNew();
            var keyboardEvent=false;var mouseEvent=false;var accepted=false;var eventCount=0;var dynamicCount=0;var frames=0;
            var leftRaw=float.NaN;var rightRaw=float.NaN;var leftPressed=true;var rightPressed=true;
            var after=before;var eventType="none";var lastType="none";var readFailure="";
            Action<InputEventPtr,InputDevice> observeEvent=(eventPtr,device)=>
            {
                if(eventPtr.type!=StateEvent.Type)return;
                var dynamic=InputState.currentUpdateType==InputUpdateType.Dynamic;
                if(ReferenceEquals(device,keyboard)&&device.deviceId==keyboardId)
                {
                    eventCount++;eventType=InputState.currentUpdateType.ToString();
                    keyboardEvent=dynamic&&keyboard.leftShiftKey.ReadValueFromEvent(eventPtr,out var left)&&left==0&&keyboard.rightShiftKey.ReadValueFromEvent(eventPtr,out var right)&&right==0;
                }
                if(ReferenceEquals(device,mouse)&&device.deviceId==mouseId)
                {eventCount++;mouseEvent=dynamic&&mouse.leftButton.ReadValueFromEvent(eventPtr,out var left)&&left==0&&mouse.rightButton.ReadValueFromEvent(eventPtr,out var right)&&right==0;}
            };
            Action observeAfter=()=>
            {
                lastType=InputState.currentUpdateType.ToString();after=InputState.updateCount;
                if(InputState.currentUpdateType!=InputUpdateType.Dynamic)return;dynamicCount++;
                if(neutralWatch.Elapsed.TotalSeconds>=5||caseWatch.Elapsed.TotalSeconds>=120||frames>=30)return;
                try
                {
                    if(!keyboard.added||!mouse.added)return;
                    leftRaw=keyboard.leftShiftKey.ReadValue();rightRaw=keyboard.rightShiftKey.ReadValue();leftPressed=keyboard.leftShiftKey.isPressed;rightPressed=keyboard.rightShiftKey.isPressed;
                    var same=ReferenceEquals(_keyboard,keyboard)&&ReferenceEquals(_mouse,mouse)&&keyboard.deviceId==keyboardId&&mouse.deviceId==mouseId;
                    var mouseClear=mouse.leftButton.ReadValue()==0&&mouse.rightButton.ReadValue()==0&&!mouse.leftButton.isPressed&&!mouse.rightButton.isPressed;
                    accepted=NeutralObservationAccepts(keyboardEvent,mouseEvent,true,before,after,same,keyboard.added&&mouse.added,keyboard.enabled&&mouse.enabled,leftRaw,rightRaw,leftPressed,rightPressed,mouseClear);
                }
                catch(Exception error){readFailure=error.GetType().Name+":"+error.Message;accepted=false;}
            };
            InputSystem.onEvent+=observeEvent;InputSystem.onAfterUpdate+=observeAfter;
            Debug.Log("[CH1Neutral] begin="+caseId+" keyboard="+keyboardId+" mouse="+mouseId+" before="+before+" type="+InputState.currentUpdateType);
            try
            {
                InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.QueueStateEvent(mouse,new MouseState());
                while(!accepted&&frames<30&&neutralWatch.Elapsed.TotalSeconds<5&&caseWatch.Elapsed.TotalSeconds<120){frames++;yield return null;}
                accepted=accepted&&ReferenceEquals(_keyboard,keyboard)&&ReferenceEquals(_mouse,mouse)&&keyboard.added&&mouse.added&&keyboard.enabled&&mouse.enabled;
                var detail="case="+caseId+" accepted="+accepted+" keyboard="+keyboardId+" mouse="+mouseId+" added="+(keyboard.added&&mouse.added)+" enabled="+(keyboard.enabled&&mouse.enabled)+" eventType="+eventType+" lastType="+lastType+" before="+before+" after="+after+" events="+eventCount+" dynamic="+dynamicCount+" keyEvent="+keyboardEvent+" mouseEvent="+mouseEvent+" leftRaw="+leftRaw.ToString("R")+" rightRaw="+rightRaw.ToString("R")+" leftPressed="+leftPressed+" rightPressed="+rightPressed+" frames="+frames+" seconds="+neutralWatch.Elapsed.TotalSeconds.ToString("R")+" readFailure="+readFailure;
                while(Encoding.UTF8.GetByteCount(detail)>3500)detail=detail.Substring(0,detail.Length/2);
                Debug.Log("[CH1Neutral] "+detail);
                if(!accepted)throw new InvalidOperationException("실제 동일 장치 중립 소비를 확인하지 못했습니다. "+detail);
            }
            finally{InputSystem.onEvent-=observeEvent;InputSystem.onAfterUpdate-=observeAfter;}
        }
        private static bool NeutralObservationAccepts(bool keyboardEvent,bool mouseEvent,bool dynamic,uint before,uint after,bool same,bool added,bool enabled,float leftRaw,float rightRaw,bool leftPressed,bool rightPressed,bool mouseClear)
        {return keyboardEvent&&mouseEvent&&dynamic&&after>before&&same&&added&&enabled&&leftRaw==0&&rightRaw==0&&!leftPressed&&!rightPressed&&mouseClear;}
        private static void ValidateEntryDashGeometry(Chapter1RoomInstance room,string socket)
        {
            if(room.Entry!=new Vector2(0,.81f) || !room.SupportsSocketPlan(socket) ||
                Math.Min(room.WalkerSocket(socket).x,room.SurveyorSocket(socket).x)<5)
                throw new InvalidOperationException("입구 대시 원본 입구·적 배치가 다릅니다.");
            var supported=false;
            foreach(var floor in room.Floors)if(SupportsEntryDashFloor(floor.xMin,floor.xMax,floor.yMax))supported=true;
            foreach(var terrain in room.Terrain)if(terrain.Overlaps(Rect.MinMaxRect(-.5f,-.1f,5.5f,1.8f)))supported=false;
            if(!supported)throw new InvalidOperationException("입구 대시 5m의 원본 평지·지형 경계가 다릅니다.");
        }
        private static bool ShouldIssueEntryDash(bool ready,bool grounded,bool released)
        {
            if(!ready)throw new InvalidOperationException("입구 대시의 실제 발행 준비가 다릅니다.");
            if(!grounded)return false;
            if(!released)throw new InvalidOperationException("입구 대시에 새 누름 경계가 없습니다.");
            return true;
        }
        private static bool SupportsEntryDashFloor(float min,float max,float top)=>min<=-.5f&&max>=5.5f&&top==0;
        private static string Sha(byte[] bytes){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-",string.Empty);}
        private static void RequireNoLinks(string path){for(var current=path;!string.IsNullOrEmpty(current);current=Path.GetDirectoryName(current))if(Directory.Exists(current)&&(File.GetAttributes(current)&FileAttributes.ReparsePoint)!=0)throw new IOException("측정 경로 링크를 거절합니다.");}
        private static void WriteNew(string path,byte[] bytes){using(var file=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.None)){file.Write(bytes,0,bytes.Length);file.Flush(true);}}
    }
}
#endif
