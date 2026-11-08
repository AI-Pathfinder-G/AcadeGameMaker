using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine.InputSystem.LowLevel;
using System.Text;
using AcadeGameMaker.Input.Unity;
using UnityEngine.InputSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    [Serializable]
    public sealed class OwnedBatchRuntimeFixtureScope
    {
        // REQ-AUTOCH1: 공개 시험 lifecycle로만 재생에 진입하고 장면 파일은 저장하지 않는다.
        [SerializeField] private string[] _savedScenePaths;
        [SerializeField] private bool[] _savedSceneLoaded;
        [SerializeField] private bool[] _savedSceneActive;
        [SerializeField] private bool _ownsRuntimeFixture;
        [SerializeField] private bool _savedEmptyBatchScene;
        [SerializeField] private bool _ownedBatchFixture;
        [SerializeField] private string _ownedBatchRunId;
        private Scene _ownedCaseScene;
        private Scene _previousCaseScene;
        private bool _allowLocalSliceSingleReplacement;
        private static void LogSceneSetupBeforeRejection(SceneSetup[] setup)
        {
            // 기존 거절에 영향을 주지 않는 읽기 전용 원시.
            try
            {
                const int maximumBytes=16384;var output=new StringBuilder();var used=0;var truncatedTotal=false;var truncatedStrings=0;var readExceptions=0;
                string Text(string value)
                {if(value==null)return "<null>";value=value.Replace("\r","\\r").Replace("\n","\\n");if(value.Length>1024){truncatedStrings++;return value.Substring(0,1024);}return value;}
                string Read(Func<string> read)
                {try{return Text(read());}catch(Exception error){readExceptions++;return "readException="+Text(error.GetType().FullName+":"+error.Message);}}
                void Append(string line)
                {var bytes=Encoding.UTF8.GetByteCount(line)+1;if(used+bytes>maximumBytes-256){truncatedTotal=true;return;}output.AppendLine(line);used+=bytes;}
                var count=0;try{count=SceneManager.sceneCount;}catch(Exception error){readExceptions++;Append("sceneCountReadException="+Text(error.GetType().FullName+":"+error.Message));}
                var active=SceneManager.GetActiveScene();
                Append("[CH1SetupBeforeGuard] setupCount="+(setup==null?-1:setup.Length)+" sceneCount="+count+" activeHandle="+Read(()=>active.handle.ToString())+" activeName="+Read(()=>active.name)+" activePath="+Read(()=>active.path)+" activeValid="+Read(()=>active.IsValid().ToString())+" activeLoaded="+Read(()=>active.isLoaded.ToString())+" activeDirty="+Read(()=>active.isDirty.ToString())+" activeRootCount="+Read(()=>active.rootCount.ToString()));
                for(var i=0;i<Math.Min(8,count);i++)
                {
                    try
                    {var scene=SceneManager.GetSceneAt(i);Append("sceneIndex="+i+" name="+Read(()=>scene.name)+" path="+Read(()=>scene.path)+" valid="+Read(()=>scene.IsValid().ToString())+" loaded="+Read(()=>scene.isLoaded.ToString())+" active="+Read(()=>(scene==active).ToString())+" dirty="+Read(()=>scene.isDirty.ToString())+" rootCount="+Read(()=>scene.rootCount.ToString()));}
                    catch(Exception error){readExceptions++;Append("sceneIndex="+i+" readException="+Text(error.GetType().FullName+":"+error.Message));}
                }
                if(setup!=null)for(var i=0;i<Math.Min(8,setup.Length);i++)
                {var item=setup[i];Append("setupIndex="+i+" path="+Read(()=>item.path)+" isLoaded="+Read(()=>item.isLoaded.ToString())+" isActive="+Read(()=>item.isActive.ToString()));}
                output.Append("diagnosticEnd sceneEnumerationTruncated=").Append(count>8).Append(" setupEnumerationTruncated=").Append(setup!=null&&setup.Length>8).Append(" stringItemsTruncated=").Append(truncatedStrings).Append(" totalBytesTruncated=").Append(truncatedTotal).Append(" readExceptions=").Append(readExceptions);
                Debug.Log(output.ToString());
            }
            catch(Exception error){try{Debug.Log("[CH1SetupBeforeGuard] diagnosticFailed="+error.GetType().FullName+" originalGuardUnchanged=true");}catch{}}
        }
        [SerializeField] private string _capturedInputCase;
        private OwnedInputLease _inputLease;
        private Func<double> _isolatedClock;private long _inputEpoch;private bool _isolationActive;private Chapter1VerificationBossBudget _inputBudget;
        private const string OwnedInputCase="AcadeGameMaker.Tests.PlayMode.InputUnity.Chapter1BossVerificationPolicyPlayModeTests.IsolatedApiBossPolicyFromAuthoredFlow";
                private static bool AllowedInputCase(string name){return name==OwnedInputCase || name=="AcadeGameMaker.Tests.PlayMode.InputUnity.Chapter1BossVerificationPolicyPlayModeTests.OwnedInputIsolationRestoresWithAdvancingFixtureClock" || name=="AcadeGameMaker.Tests.PlayMode.InputUnity.Chapter1BossVerificationPolicyPlayModeTests.OwnedInputIsolationRestoresWithTimestampMutationRejected" || name=="AcadeGameMaker.Tests.PlayMode.InputUnity.Chapter1BossVerificationPolicyPlayModeTests.OwnedInputIsolationRestoresWithUnexpectedQueuedIdRejected" || name=="AcadeGameMaker.Tests.PlayMode.InputUnity.Chapter1BossVerificationPolicyPlayModeTests.OwnedInputIsolationRestoresWithDeviceMutationRejected" || name=="AcadeGameMaker.Tests.PlayMode.InputUnity.Chapter1BossVerificationPolicyPlayModeTests.OwnedInputIsolationRestoresWithReplayedPairRejected" || name=="AcadeGameMaker.Tests.PlayMode.InputUnity.Chapter1BossVerificationPolicyPlayModeTests.OwnedInputIsolationRestoresAfterNormalBody" || name=="AcadeGameMaker.Tests.PlayMode.InputUnity.Chapter1BossVerificationPolicyPlayModeTests.OwnedInputIsolationRestoresAfterBodyFailure";}
        internal OwnedBatchInputIsolation BeginOwnedInputIsolation(IChapter1OwnedBatchInputContext context){if(_isolationActive)throw new InvalidOperationException("중첩 격리입니다.");return new OwnedBatchInputIsolation(context,value=>_isolationActive=value,provider=>_isolatedClock=provider);}
        private const string OwnedInputRoot="H:/GPT-workspace/AcadeGameMaker-integration-20261107/output/ch1-standalone-owner-platform-20261104";
        [Serializable] private sealed class InputIdentity {public int pid;public string birth,path;}
        [Serializable] private sealed class InputStarted {public InputIdentity identity;}
        [Serializable] private sealed class InputPlan {public string phase,exe,sourceSha,scopeSha,gateSha,parent,selectorSha,evidenceId,gatePath;public string[] arguments;}
        [Serializable] private sealed class InputGate {public string Schema,Owner,Model,ScopeSha,Parent,SourceSha;public bool Approved;}
        [Serializable] private sealed class InputSelectors {public string[] editorSelectors,smokeSelectors,receiptTimeSelectors;}
        private static string InputArg(string[] args,string name,bool flag=false)
        {
            var found=-1;
            for(var i=0;i<args.Length;i++)if(string.Equals(args[i],name,StringComparison.OrdinalIgnoreCase)||args[i].StartsWith(name+"=",StringComparison.OrdinalIgnoreCase))
            {if(found>=0 || args[i]!=name)throw new InvalidOperationException("중복/비정규 배치 인수: "+name);found=i;}
            if(found<0)throw new InvalidOperationException("배치 인수 부재: "+name);
            if(flag)return "present";
            if(found+1>=args.Length || string.IsNullOrWhiteSpace(args[found+1]) || args[found+1].StartsWith("-",StringComparison.Ordinal))throw new InvalidOperationException("배치 인수 값 부재: "+name);
            return args[found+1];
        }
        // REQ/AC-PRACE-002: one fixed safety root; evidence selection never grants new input policy.
        internal static string EvidenceRoot(string[] args)
        {
            var id=InputArg(args,"-autonomousOwnedBatchEvidenceId");
            if(!System.Text.RegularExpressions.Regex.IsMatch(id,"^[a-z0-9][a-z0-9-]{0,47}$") || System.Text.RegularExpressions.Regex.IsMatch(id,"^(con|prn|aux|nul|com[1-9]|lpt[1-9])$",System.Text.RegularExpressions.RegexOptions.IgnoreCase))throw new InvalidOperationException("증거 식별자 형식입니다.");
            return InputPath(Path.Combine(OwnedInputRoot,"attempts",id));
        }
        internal static string ValidateEvidenceContract(string[] args,string[] planned,string evidenceId,string gatePath,string planGateSha,string actualGateSha,string sourceSha,string manifestSha,string phase)
        {
            var root=EvidenceRoot(args);var id=InputArg(args,"-autonomousOwnedBatchEvidenceId");
            foreach(var name in new[]{"-autonomousOwnedBatchEvidenceId","-autonomousOwnedBatchGate","-autonomousOwnedBatchGateSha","-autonomousOwnedBatchExecutionReceipt","-autonomousOwnedBatchPlan","-autonomousOwnedBatchSourceManifest"})
                if(InputArg(args,name)!=InputArg(planned,name))throw new InvalidOperationException("증거 실제 인수와 계획이 다릅니다.");
            var selected=InputPath(InputArg(args,"-autonomousOwnedBatchGate"));var expected=InputPath(Path.Combine(root,"luna-tools-review.json"));
            var sha=InputArg(args,"-autonomousOwnedBatchGateSha");
            if(id!=evidenceId || !string.Equals(selected,expected,StringComparison.OrdinalIgnoreCase) || !string.Equals(InputPath(gatePath),expected,StringComparison.OrdinalIgnoreCase) || !System.Text.RegularExpressions.Regex.IsMatch(sha,"^[0-9A-F]{64}$") || sha!=planGateSha || sha!=actualGateSha || !System.Text.RegularExpressions.Regex.IsMatch(sourceSha??"","^[0-9A-F]{64}$") || sourceSha!=manifestSha)throw new InvalidOperationException("증거 시도/게이트/소스가 다릅니다.");
            foreach(var pair in new[]{new[]{"-autonomousOwnedBatchExecutionReceipt","started.json"},new[]{"-autonomousOwnedBatchPlan","plan.json"},new[]{"-autonomousOwnedBatchSourceManifest","source-manifest.json"}})
                if(!string.Equals(InputPath(InputArg(args,pair[0])),InputPath(Path.Combine(root,"execution",phase,pair[1])),StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("증거 영수증 경로가 다릅니다.");
            return root;
        }
        private static string InputPath(string path)
        {
            if(string.IsNullOrEmpty(path) || path.Length<3 || path[1]!=':' || (path[2]!='\\' && path[2]!='/'))throw new InvalidOperationException("절대 배치 경로가 아닙니다.");
            var full=Path.GetFullPath(path);
            for(var part=full;!string.IsNullOrEmpty(part);part=Path.GetDirectoryName(part))
                if((File.Exists(part)||Directory.Exists(part)) && (File.GetAttributes(part)&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("배치 영수증 링크 경로입니다.");
            return full;
        }
        private static string InputText(string path,int maximum)
        {path=InputPath(path);var info=new FileInfo(path);if(!info.Exists||info.Length>maximum)throw new InvalidOperationException("배치 영수증 부재/상한입니다.");return File.ReadAllText(path,Encoding.UTF8);}
        private static string InputSha(string path)
        {path=InputPath(path);using(var sha=System.Security.Cryptography.SHA256.Create())using(var stream=File.OpenRead(path))return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","");}
        private void RevokeOwnedInputContext(){if(_inputLease!=null){_inputLease.Dispose();_inputLease=null;}}
        internal IChapter1OwnedBatchInputContext AcquireOwnedInputContext()
        {
            if(!Application.isEditor || !Application.isBatchMode || !Application.isPlaying || !_ownedBatchFixture || !_ownsRuntimeFixture || !AllowedInputCase(_capturedInputCase) || _inputLease!=null || !_ownedCaseScene.IsValid() || !_ownedCaseScene.isLoaded || !string.IsNullOrEmpty(_ownedCaseScene.path) || !_ownedCaseScene.name.StartsWith("CH1-proof-case-",StringComparison.Ordinal) || SceneManager.GetActiveScene()!=_ownedCaseScene)throw new InvalidOperationException("정확 소유 사례 수명 밖입니다.");
            var project=InputPath(Path.Combine(Application.dataPath,".."));if(!string.Equals(project,InputPath("H:/GPT-workspace/AcadeGameMaker-integration-20261107/output/chapter1-playable-r1/project"),StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("정확 배치 프로젝트가 아닙니다.");
            var args=Environment.GetCommandLineArgs();InputArg(args,"-autonomousOwnedBatchFixture",true);var runId=InputArg(args,"-autonomousOwnedBatchRunId");
            var phase=runId=="owned-batch-input-receipt-time-contract"?"receipt-time-contract":runId=="owned-batch-input-isolation-smoke"?"isolation-smoke":runId=="owned-batch-input-diagnostic-natural"?"diagnostic-natural":runId=="owned-batch-input-regression-editor"?"regression-editor":null;
            if(phase==null || runId!=_ownedBatchRunId)throw new InvalidOperationException("인가하지 않은 배치 runId입니다.");
            var root=EvidenceRoot(args);var output=InputPath(Path.Combine(root,"execution",phase));
            var receipt=InputArg(args,"-autonomousOwnedBatchExecutionReceipt");var planPath=InputArg(args,"-autonomousOwnedBatchPlan");var manifest=InputArg(args,"-autonomousOwnedBatchSourceManifest");
            foreach(var pair in new[]{new[]{receipt,"started.json"},new[]{planPath,"plan.json"},new[]{manifest,"source-manifest.json"}})
                if(!string.Equals(InputPath(pair[0]),InputPath(Path.Combine(output,pair[1])),StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("배치 영수증 정확 경로 불일치입니다.");
            var started=JsonUtility.FromJson<InputStarted>(InputText(receipt,16384));var plan=JsonUtility.FromJson<InputPlan>(InputText(planPath,65536));var gatePath=Path.Combine(root,"luna-tools-review.json");var gate=JsonUtility.FromJson<InputGate>(InputText(gatePath,16384));ValidateEvidenceContract(args,plan.arguments,plan.evidenceId,plan.gatePath,plan.gateSha,InputSha(gatePath),plan.sourceSha,InputSha(manifest),phase);
            if(started?.identity==null || started.identity.path!="C:\\Program Files\\Unity\\Hub\\Editor\\6000.6.0f1\\Editor\\Unity.exe" || plan?.arguments==null || gate==null || !gate.Approved || gate.Schema!="ch1-boss-dependency-gate-v1" || gate.Owner!="Luna" || gate.Model!="gpt-6-luna" || gate.ScopeSha!="A32977E678D393772AD94A73AED3761AA4E66D9BE1FCC625F1052E081332A8B6" || plan.scopeSha!=gate.ScopeSha || plan.phase!=phase || plan.gateSha!=InputSha(gatePath) || plan.sourceSha!=InputSha(manifest) || gate.SourceSha!=plan.sourceSha || plan.parent!=gate.Parent || plan.parent!="814ac33b5887a58ccc4ae711884e7eb492a45082")throw new InvalidOperationException("배치 plan/gate/Source 결속 불일치입니다.");
            foreach(var name in new[]{"-autonomousOwnedBatchFixture","-autonomousOwnedBatchRunId","-autonomousOwnedBatchExecutionReceipt","-autonomousOwnedBatchPlan","-autonomousOwnedBatchSourceManifest","-projectPath","-testPlatform","-testFilter"})
                if(InputArg(args,name,name=="-autonomousOwnedBatchFixture")!=InputArg(plan.arguments,name,name=="-autonomousOwnedBatchFixture"))throw new InvalidOperationException("실제 CLI와 plan 불일치입니다.");
            var known=new[]{"-autonomousOwnedBatchFixture","-autonomousOwnedBatchRunId","-autonomousOwnedBatchExecutionReceipt","-autonomousOwnedBatchPlan","-autonomousOwnedBatchSourceManifest","-autonomousOwnedBatchEvidenceId","-autonomousOwnedBatchGate","-autonomousOwnedBatchGateSha"};
            foreach(var arg in args)if(arg.StartsWith("-autonomousOwnedBatch",StringComparison.OrdinalIgnoreCase) && Array.IndexOf(known,arg)<0)throw new InvalidOperationException("미등록 배치 인수입니다.");
            var selectorPath=Path.Combine(root,"selectors.json");
            if(InputSha(selectorPath)!=plan.selectorSha)throw new InvalidOperationException("고정 선택자 지문 불일치입니다.");
            var expected=phase=="receipt-time-contract"?string.Join(";",JsonUtility.FromJson<InputSelectors>(InputText(selectorPath,65536)).receiptTimeSelectors):phase=="diagnostic-natural"?OwnedInputCase:phase=="isolation-smoke"?string.Join(";",JsonUtility.FromJson<InputSelectors>(InputText(selectorPath,65536)).smokeSelectors):string.Join(";",JsonUtility.FromJson<InputSelectors>(InputText(selectorPath,65536)).editorSelectors);
            if(InputArg(args,"-testFilter")!=expected || InputArg(args,"-testPlatform")!="EditMode" || !string.Equals(InputPath(InputArg(args,"-projectPath")),project,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("정확 사례/단계/프로젝트 불일치입니다.");
            using(var self=System.Diagnostics.Process.GetCurrentProcess())
                if(self.Id!=started.identity.pid || self.StartTime.ToFileTimeUtc().ToString()!=started.identity.birth || self.MainModule.FileName!=started.identity.path || plan.exe!=started.identity.path)throw new InvalidOperationException("자기 실행 영수증 불일치입니다.");
            _inputBudget=_inputBudget??new Chapter1VerificationBossBudget(phase=="isolation-smoke"?65536:131072,true);_inputLease=new OwnedInputLease(this,new Chapter1OwnedBatchInputEvidence{Epoch=++_inputEpoch,CaseName=_capturedInputCase,RunId=runId,Phase=phase,SourceSha=plan.sourceSha,OwnerPid=started.identity.pid,OwnerBirth=started.identity.birth});return _inputLease;
        }
        private sealed class OwnedInputLease:IChapter1OwnedBatchInputContext
        {
            private readonly OwnedBatchRuntimeFixtureScope _owner;private readonly Chapter1OwnedBatchInputEvidence _evidence;private bool _closed;
            internal OwnedInputLease(OwnedBatchRuntimeFixtureScope owner,Chapter1OwnedBatchInputEvidence evidence){_owner=owner;_evidence=evidence;}
            public Chapter1VerificationBossBudget Budget=>_owner._inputBudget;
            public bool TryObserve(out Chapter1OwnedBatchInputEvidence evidence)
            {evidence=_evidence;evidence.IsolatedApi=_owner._isolationActive;return !_closed && ReferenceEquals(_owner._inputLease,this) && _owner._ownedBatchFixture && _owner._ownsRuntimeFixture && AllowedInputCase(_owner._capturedInputCase) && _owner._ownedCaseScene.IsValid() && _owner._ownedCaseScene.isLoaded && Application.isEditor && Application.isBatchMode && Application.isPlaying;}
            public bool TryReadIsolatedTime(out double value)
            {value=0;Chapter1OwnedBatchInputEvidence current;if(!TryObserve(out current) || !current.IsolatedApi || _owner._isolatedClock==null || current.Epoch!=_evidence.Epoch || current.CaseName!=_owner._capturedInputCase || current.SourceSha!=_evidence.SourceSha || current.OwnerPid!=_evidence.OwnerPid || current.OwnerBirth!=_evidence.OwnerBirth)return false;value=_owner._isolatedClock();return !double.IsNaN(value)&&!double.IsInfinity(value);}
            public void Dispose(){_owner._isolatedClock=null;_closed=true;}
        }
        private bool ValidateOwnedBatchStart(SceneSetup[] setup)
        {
            if(setup.Length!=0)return false;
            var args=Environment.GetCommandLineArgs();var marked=Array.IndexOf(args,"-autonomousOwnedBatchFixture")>=0;
            if(!marked)return false;
            Assert.That(Application.isBatchMode,Is.True,"소유 배치 표식은 대화형 편집기에 적용하지 않습니다.");
            var index=Array.IndexOf(args,"-autonomousOwnedBatchRunId");Assert.That(index>=0 && index+1<args.Length,Is.True);_ownedBatchRunId=args[index+1];
            Assert.That(_ownedBatchRunId.Length,Is.InRange(1,128));foreach(var character in _ownedBatchRunId)Assert.That(char.IsLetterOrDigit(character)||character=='-'||character=='_',Is.True);
            var project=Path.GetFullPath(Path.Combine(Application.dataPath,"..")).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);
            var expected=Path.GetFullPath("H:/GPT-workspace/AcadeGameMaker-integration-20261107/output/chapter1-playable-r1/project").TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);
            Assert.That(string.Equals(project,expected,StringComparison.OrdinalIgnoreCase),Is.True,"정확 승인 프로젝트가 아닙니다.");
            using(var process=System.Diagnostics.Process.GetCurrentProcess())Debug.Log("[CH1Fixture] branch=owned-batch-before runId="+_ownedBatchRunId+" marked="+marked+" batch="+Application.isBatchMode+" project="+project+" pid="+process.Id+" birthFileTime="+process.StartTime.ToFileTimeUtc().ToString(System.Globalization.CultureInfo.InvariantCulture)+" setupCount="+setup.Length+" sceneCount="+SceneManager.sceneCount);
            Assert.That(SceneManager.sceneCount,Is.EqualTo(1));var scene=SceneManager.GetSceneAt(0);
            Debug.Log("[CH1Fixture] owned-start name="+scene.name+" path="+scene.path+" valid="+scene.IsValid()+" loaded="+scene.isLoaded+" active="+(SceneManager.GetActiveScene()==scene)+" dirty="+scene.isDirty+" roots="+scene.rootCount);
            Assert.That(scene.IsValid() && scene.isLoaded && SceneManager.GetActiveScene()==scene && string.IsNullOrEmpty(scene.path) && !scene.isDirty,Is.True);
            var roots=scene.GetRootGameObjects();Assert.That(roots.Length,Is.EqualTo(2));
            foreach(var root in roots)
            {
                var components=root.GetComponents<Component>();Assert.That(components.Length,Is.LessThanOrEqualTo(8));var raw=new StringBuilder();
                raw.Append("[CH1Fixture] owned-root name=").Append(root.name.Length>1024?root.name.Substring(0,1024):root.name).Append(" nameTruncated=").Append(root.name.Length>1024).Append(" hideFlags=").Append(root.hideFlags).Append(" childCount=").Append(root.transform.childCount).Append(" componentCount=").Append(components.Length);
                foreach(var component in components)raw.Append(" type=").Append(component==null?"<missing>":component.GetType().FullName);
                Assert.That(Encoding.UTF8.GetByteCount(raw.ToString()),Is.LessThanOrEqualTo(8192));Debug.Log(raw.ToString());
                Assert.That(root.transform.childCount,Is.Zero);
                foreach(var component in components)
                {Assert.That(component,Is.Not.Null);var type=component.GetType();Assert.That(component is MonoBehaviour,Is.False);Assert.That(type==typeof(Transform)||type==typeof(UnityEngine.Camera)||type==typeof(AudioListener)||type==typeof(Light),Is.True,"기본 시작 구성 외 컴포넌트입니다.");}
            }
            return true;
        }
        private int _receiptWitnessRows;private long _receiptWitnessOrdinal;
        internal void RecordCaseInputWitness(string stage,bool entered,bool completed,bool expected)
        {
            if(!_ownedBatchFixture || !_ownsRuntimeFixture || !AllowedInputCase(_capturedInputCase))return;
            if(_receiptWitnessRows>=16)throw new InvalidOperationException("사례 증거16행 상한입니다.");
            var output=Path.Combine(EvidenceRoot(Environment.GetCommandLineArgs()),"execution",_ownedBatchRunId.Substring("owned-batch-input-".Length));var started=JsonUtility.FromJson<InputStarted>(InputText(Path.Combine(output,"started.json"),16384));var plan=JsonUtility.FromJson<InputPlan>(InputText(Path.Combine(output,"plan.json"),65536));
            var row=new Chapter1CaseInputWitness{Stage=stage,CaseName=_capturedInputCase,RunId=_ownedBatchRunId,Phase=plan.phase,SourceSha=plan.sourceSha,OwnerPid=started.identity.pid,OwnerBirth=started.identity.birth,Ordinal=++_receiptWitnessOrdinal,BodyEntered=entered,BodyCompleted=completed,ExpectedFailureReached=expected};
            var json=JsonUtility.ToJson(row);if(_inputBudget==null)_inputBudget=new Chapter1VerificationBossBudget(plan.phase=="isolation-smoke"?65536:131072,true);if(stage=="teardown")_inputBudget.AddCleanup("case-witness "+json);else _inputBudget.Add("case-witness "+json);Debug.Log("[BVP] case-witness "+json);_receiptWitnessRows++;
        }
        public void PrepareEnter(string observedCaseFullName=null)
        {
            _capturedInputCase=observedCaseFullName;
            Assert.That(Application.isPlaying,Is.False,"이미 재생 중인 환경은 이 fixture 소유가 아닙니다.");
            var setup=EditorSceneManager.GetSceneManagerSetup();LogSceneSetupBeforeRejection(setup);_ownedBatchFixture=ValidateOwnedBatchStart(setup);if(!_ownedBatchFixture)Assert.That(setup.Length,Is.GreaterThan(0));
            _savedScenePaths=new string[setup.Length];_savedSceneLoaded=new bool[setup.Length];_savedSceneActive=new bool[setup.Length];
            _savedEmptyBatchScene=setup.Length==1 && string.IsNullOrEmpty(setup[0].path);
            if(_savedEmptyBatchScene)
            {
                Assert.That(Application.isBatchMode,Is.True);Assert.That(SceneManager.sceneCount,Is.EqualTo(1));var empty=SceneManager.GetSceneAt(0);
                Debug.Log("[CH1Fixture] branch=empty-before name="+empty.name+" count="+SceneManager.sceneCount+" valid="+empty.IsValid()+" loaded="+empty.isLoaded+" active="+(SceneManager.GetActiveScene()==empty)+" path="+empty.path+" dirty="+empty.isDirty+" roots="+empty.rootCount+" setupLoaded="+setup[0].isLoaded+" setupActive="+setup[0].isActive);
                Assert.That(empty.IsValid() && empty.isLoaded && SceneManager.GetActiveScene()==empty && string.IsNullOrEmpty(empty.path) && !empty.isDirty && empty.rootCount==0,Is.True,"미저장/내용 있는 장면은 빈 배치 예외가 아닙니다.");
            }
            for(var i=0;i<setup.Length && !_savedEmptyBatchScene;i++)
            {
                Assert.That(string.IsNullOrEmpty(setup[i].path),Is.False,"복원 가능한 저장 장면 경로가 필요합니다.");
                Assert.That(setup[i].path.StartsWith("Assets/",StringComparison.Ordinal) && setup[i].path.EndsWith(".unity",StringComparison.OrdinalIgnoreCase),Is.True,"현재 프로젝트의 저장 장면만 복원합니다.");
                Assert.That(UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(setup[i].path),Is.Not.Null,"복원할 SceneAsset가 존재해야 합니다.");
                var scene=SceneManager.GetSceneByPath(setup[i].path);Debug.Log("[CH1Fixture] branch=saved-before index="+i+" count="+setup.Length+" name="+(scene.IsValid()?scene.name:"unloaded")+" path="+setup[i].path+" valid="+scene.IsValid()+" loaded="+scene.isLoaded+" active="+(SceneManager.GetActiveScene()==scene)+" dirty="+(scene.IsValid()?scene.isDirty.ToString():"unobserved")+" roots="+(scene.IsValid()&&scene.isLoaded?scene.rootCount.ToString():"unobserved")+" setupLoaded="+setup[i].isLoaded+" setupActive="+setup[i].isActive);if(scene.IsValid())Assert.That(scene.isDirty,Is.False,"미저장 장면 변경은 폐기하지 않습니다.");
                _savedScenePaths[i]=setup[i].path;_savedSceneLoaded[i]=setup[i].isLoaded;_savedSceneActive[i]=setup[i].isActive;
            }
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);_ownsRuntimeFixture=true;
            }
        public void ConfirmEntered()
        {
            Assert.That(_ownsRuntimeFixture,Is.True);Assert.That(Application.isPlaying,Is.True);_caseWatch=System.Diagnostics.Stopwatch.StartNew();
            _receiptWitnessRows=0;_receiptWitnessOrdinal=0;_inputBudget=null;Debug.Log("[CH1Fixture] runtime-entered production-scenes-isolated");
        }
        public IEnumerator BeginCase(bool allowLocalSliceSingleReplacement=false)
        {
            _allowLocalSliceSingleReplacement=allowLocalSliceSingleReplacement;
            Assert.That(Application.isPlaying && _ownsRuntimeFixture,Is.True);
            _previousCaseScene=SceneManager.GetActiveScene();
            _ownedCaseScene=SceneManager.CreateScene("CH1-proof-case-"+Guid.NewGuid().ToString("N"));
            Assert.That(_ownedCaseScene.IsValid() && _ownedCaseScene.isLoaded && !string.IsNullOrEmpty(_ownedCaseScene.name),Is.True);
            Assert.That(SceneManager.SetActiveScene(_ownedCaseScene),Is.True);yield return null;
            Assert.That(SceneManager.GetActiveScene(),Is.EqualTo(_ownedCaseScene));Debug.Log("[CH1Fixture] named-case="+_ownedCaseScene.name);
        }
        public IEnumerator CloseCase()
        {
            RevokeOwnedInputContext();
            Assert.That(Application.isPlaying,Is.True,"원래 Dispose는 재생 상태에서 완료되어야 합니다.");yield return null;
            if(!_ownedCaseScene.IsValid() || !_ownedCaseScene.isLoaded)
            {
                Assert.That(_allowLocalSliceSingleReplacement,Is.True,"의도하지 않은 사례 장면 교체입니다.");
                Assert.That(SceneManager.sceneCount,Is.EqualTo(1));var current=SceneManager.GetActiveScene();
                var production=current.path=="Assets/Scenes/LocalPlayableSlice.unity";
                var exit=string.IsNullOrEmpty(current.path)&&(current.name=="LocalSliceLifecycleExit"||current.name=="LocalInputTestExit");
                Assert.That(current.IsValid() && current.isLoaded && (production||exit),Is.True,"승인된 단일 교체 장면이 아닙니다.");
                if(production)
                {
                    foreach(var root in current.GetRootGameObjects())root.SetActive(false);
                    var blank=SceneManager.CreateScene("CH1-proof-single-close-"+Guid.NewGuid().ToString("N"));Assert.That(SceneManager.SetActiveScene(blank),Is.True);
                    yield return SceneManager.UnloadSceneAsync(current);Assert.That(current.isLoaded,Is.False);yield return null;
                }
                var remaining=SceneManager.GetActiveScene();Assert.That(SceneManager.sceneCount==1 && remaining.IsValid() && remaining.isLoaded && string.IsNullOrEmpty(remaining.path) && remaining.rootCount==0,Is.True);
                Debug.Log("[CH1Fixture] single-replacement-cleaned previousHandleRestored=false");yield break;
            }
            if(_previousCaseScene.IsValid() && _previousCaseScene.isLoaded)Assert.That(SceneManager.SetActiveScene(_previousCaseScene),Is.True);
            if(_ownedCaseScene.IsValid() && _ownedCaseScene.isLoaded){var operation=SceneManager.UnloadSceneAsync(_ownedCaseScene);Assert.That(operation,Is.Not.Null);yield return operation;Assert.That(_ownedCaseScene.isLoaded,Is.False);}
            yield return null;Debug.Log("[CH1Fixture] named-case-unloaded delayed-destroy-observed");
        }

        // REQ-AUTOCH1-G5BRIDGE-PREP-001: 미적용 후보. NUnit 사례 문맥은 바깥 실행기가 소유한다.
        private System.Runtime.ExceptionServices.ExceptionDispatchInfo _cleanupFailure;
        private System.Diagnostics.Stopwatch _caseWatch;
        public void CaptureCleanupFailure(Action cleanup)
        {
            try{cleanup();}catch(Exception error)
            {if(_cleanupFailure==null)_cleanupFailure=System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error);else try{_cleanupFailure.SourceException.Data["CH1AdditionalCleanupFailure"]=error;}catch{}}
        }
        public void ThrowCleanupFailure(){var failure=_cleanupFailure;_cleanupFailure=null;if(failure!=null)failure.Throw();}
        public IEnumerator RunBody(IEnumerator body,bool deferCleanupFailure=false)
        {return RunBodyCore(body,deferCleanupFailure,null,null);}
        // 준비 경계시험만 사용하는 짧은 실제 기한. 정상 wrapper는 이 입구를 호출하지 않는다.
        internal IEnumerator RunBodyForProbe(IEnumerator body,double? timeoutSeconds,Action<OwnedBatchRuntimeCoroutineHost> started)
        {
            if(timeoutSeconds.HasValue && (double.IsNaN(timeoutSeconds.Value) || double.IsInfinity(timeoutSeconds.Value) || timeoutSeconds.Value<0.25 || timeoutSeconds.Value>2))
                throw new ArgumentOutOfRangeException(nameof(timeoutSeconds));
            return RunBodyCore(body,false,timeoutSeconds,started);
        }
        private IEnumerator RunBodyCore(IEnumerator body,bool deferCleanupFailure,double? probeTimeoutSeconds,Action<OwnedBatchRuntimeCoroutineHost> started)
        {
            Assert.That(Application.isPlaying && _ownsRuntimeFixture,Is.True);
            var go=new GameObject("CH1-owned-runtime-coroutine");
            UnityEngine.Object.DontDestroyOnLoad(go);
            var host=go.AddComponent<OwnedBatchRuntimeCoroutineHost>();
            var testId=TestContext.CurrentContext.Test.ID;
            var watch=System.Diagnostics.Stopwatch.StartNew();
            if(_caseWatch==null)_caseWatch=System.Diagnostics.Stopwatch.StartNew();
            var reloading=false;
            System.Runtime.ExceptionServices.ExceptionDispatchInfo externalFailure=null;
            UnityEditor.AssemblyReloadEvents.AssemblyReloadCallback reload=()=>{reloading=true;if(host!=null)host.Abort(new InvalidOperationException("시험 본문 도중 도메인 재적재가 요청됐습니다."));};
            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload+=reload;
            try
            {
                host.Begin(body,testId);started?.Invoke(host);
                // 설치 기본180초를 늘리지 않는다. 별도 승인 뒤 실제 경계검사 필요.
                while(!host.IsDone)
                {
                    if(host==null || !Application.isPlaying || reloading)
                    {externalFailure=System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(new InvalidOperationException("소유 시험 실행 개체 또는 재생 수명이 소실됐습니다."));break;}
                    if((probeTimeoutSeconds.HasValue && watch.Elapsed.TotalSeconds>=probeTimeoutSeconds.Value) || _caseWatch.Elapsed.TotalSeconds>=(deferCleanupFailure?170:160) || (deferCleanupFailure && watch.Elapsed.TotalSeconds>=10))
                    {host.Abort(new TimeoutException("시험 본문의 명시적 단조기한을 초과했습니다."));break;}
                    yield return null;
                }
                if(externalFailure!=null){if(deferCleanupFailure)CaptureCleanupFailure(()=>externalFailure.Throw());else externalFailure.Throw();}
                else if(deferCleanupFailure)CaptureCleanupFailure(host.ThrowFirstFailure);else host.ThrowFirstFailure();
            }
            finally
            {
                UnityEditor.AssemblyReloadEvents.beforeAssemblyReload-=reload;
                if(host!=null)host.Abort(null);
                if(go!=null)UnityEngine.Object.DestroyImmediate(go);
            }
        }
        public bool OwnsRuntimeFixture=>_ownsRuntimeFixture;
        public void CompleteExit()
        {
            RevokeOwnedInputContext();
            Assert.That(_ownsRuntimeFixture,Is.True);Assert.That(Application.isPlaying,Is.False);
            Assert.That(_savedScenePaths,Is.Not.Null);if(!_ownedBatchFixture)Assert.That(_savedScenePaths.Length,Is.GreaterThan(0));
            var setup=new SceneSetup[_savedScenePaths.Length];for(var i=0;i<setup.Length;i++)setup[i]=new SceneSetup{path=_savedScenePaths[i],isLoaded=_savedSceneLoaded[i],isActive=_savedSceneActive[i]};
            if(_ownedBatchFixture)
            {
                var empty=SceneManager.GetActiveScene();Debug.Log("[CH1Fixture] branch=owned-batch-exited runId="+_ownedBatchRunId+" sceneCount="+SceneManager.sceneCount+" name="+empty.name+" path="+empty.path+" valid="+empty.IsValid()+" loaded="+empty.isLoaded+" active="+(SceneManager.GetActiveScene()==empty)+" dirty="+empty.isDirty+" roots="+empty.rootCount+" originalRootZeroRestored=false");
                Assert.That(SceneManager.sceneCount==1 && empty.IsValid() && empty.isLoaded && string.IsNullOrEmpty(empty.path) && !empty.isDirty && empty.rootCount==0,Is.True,"소유 시험의 빈 종료 장면이 아닙니다.");
                Debug.Log("[CH1Fixture] owned-batch-default-reconstruction-begin runId="+_ownedBatchRunId);
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects,NewSceneMode.Single);
                var reconstructed=EditorSceneManager.GetSceneManagerSetup();Assert.That(reconstructed.Length,Is.Zero);
                Assert.That(ValidateOwnedBatchStart(reconstructed),Is.True,"다음 fixture용 기본 장면 재구성 검증이 실패했습니다.");
                Debug.Log("[CH1Fixture] owned-batch-default-reconstruction-verified runId="+_ownedBatchRunId+" historicalSceneIdentityRestored=false");
            }
            else if(_savedEmptyBatchScene)
            {
                var empty=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                Debug.Log("[CH1Fixture] branch=empty-restored name="+empty.name+" count="+SceneManager.sceneCount+" valid="+empty.IsValid()+" loaded="+empty.isLoaded+" active="+(SceneManager.GetActiveScene()==empty)+" path="+empty.path+" dirty="+empty.isDirty+" roots="+empty.rootCount+" setupLoaded="+EditorSceneManager.GetSceneManagerSetup()[0].isLoaded+" setupActive="+EditorSceneManager.GetSceneManagerSetup()[0].isActive);
                Assert.That(SceneManager.sceneCount==1 && empty.IsValid() && empty.isLoaded && SceneManager.GetActiveScene()==empty && string.IsNullOrEmpty(empty.path) && !empty.isDirty && empty.rootCount==0,Is.True);
            }
            else
            {
                EditorSceneManager.RestoreSceneManagerSetup(setup);var actual=EditorSceneManager.GetSceneManagerSetup();Assert.That(actual.Length,Is.EqualTo(setup.Length));
                for(var i=0;i<setup.Length;i++){var restored=SceneManager.GetSceneByPath(actual[i].path);Debug.Log("[CH1Fixture] branch=saved-restored index="+i+" count="+actual.Length+" name="+(restored.IsValid()?restored.name:"unloaded")+" path="+actual[i].path+" valid="+restored.IsValid()+" loaded="+restored.isLoaded+" active="+(SceneManager.GetActiveScene()==restored)+" dirty="+(restored.IsValid()?restored.isDirty.ToString():"unobserved")+" roots="+(restored.IsValid()&&restored.isLoaded?restored.rootCount.ToString():"unobserved")+" setupLoaded="+actual[i].isLoaded+" setupActive="+actual[i].isActive);Assert.That(actual[i].path,Is.EqualTo(setup[i].path));Assert.That(actual[i].isLoaded,Is.EqualTo(setup[i].isLoaded));Assert.That(actual[i].isActive,Is.EqualTo(setup[i].isActive));}
            }
            _ownsRuntimeFixture=false;Debug.Log("[CH1Fixture] runtime-exited ownedBatch="+_ownedBatchFixture+" originalSceneSetupRestored="+(!_ownedBatchFixture));
        }

    }
    // 시험 조립체 전용. 실제 제품이나 배포 실행에는 연결하지 않는다.
    // REQ/AC-IEC-D: composition only. No inherited NUnit Setup and no direct Update.
    internal sealed class OwnedBatchInputIsolation
    {
        private static OwnedBatchInputIsolation _active;
        private readonly Action<Func<double>> _clock;private bool _clockActive,_clockReadRejected;private readonly InputTestFixture _api=new InputTestFixture();private readonly IChapter1OwnedBatchInputContext _lease;private readonly Action<bool> _mark;private readonly Chapter1OwnedBatchInputEvidence _identity;
        private readonly InputSettings _settings;private readonly string _settingsJson,_settingsName,_assetPath,_assetSha;private readonly HideFlags _settingsFlags;private readonly bool _persistent;
        private readonly InputDevice[] _devices,_currents;private readonly int[] _ids,_currentIds;private readonly bool[] _added,_enabled;private readonly string[] _layouts;
        private readonly List<UnityEngine.InputSystem.InputControl> _controls=new List<UnityEngine.InputSystem.InputControl>();private readonly List<byte[]> _values=new List<byte[]>();private readonly string _valueHash,_types;
        private Keyboard _keyboard;private Mouse _mouse;private bool _setupAttempted,_setupComplete,_restored,_finished,_registered;private int _originalTicks,_originalAtSetup,_originalAtRestore,_testTicks,_testAtRestore;private double _restoreTime;
        internal double StartedReal{get;private set;}internal string Error{get;private set;}internal bool Restored=>_finished && Error==null;internal Action EmergencyCleanup;
        [Serializable] private sealed class Record
        {public string stage,caseName,phase,sourceSha,ownerBirth,verificationKind="isolated-api-integration",controlHash,types,error;public int ownerPid,devices,controls,originalTicks,testTicks;public int[] baselineDeviceIds,currentIds;public bool[] currentNulls;public bool clockProviderInvalidated,clockReadRejected;public bool nativeNaturalInput=false,packagePlayerLoopDrivenUpdate=true,persistent,originalReferenceRestored,valueEquivalentRestored,currentRestored;}
        internal static byte[] CanonicalValue(object value)
        {
            if(value==null)throw new InvalidOperationException("공개 control null 타입입니다.");var bytes=new List<byte>();bytes.AddRange(Encoding.UTF8.GetBytes(value.GetType().FullName+"|"));
            if(value is float f)bytes.AddRange(BitConverter.GetBytes(f));else if(value is double d)bytes.AddRange(BitConverter.GetBytes(d));
            else if(value is Vector2 v2){bytes.AddRange(BitConverter.GetBytes(v2.x));bytes.AddRange(BitConverter.GetBytes(v2.y));}
            else if(value is Vector3 v3){bytes.AddRange(BitConverter.GetBytes(v3.x));bytes.AddRange(BitConverter.GetBytes(v3.y));bytes.AddRange(BitConverter.GetBytes(v3.z));}
            else if(value is Vector4 v4){bytes.AddRange(BitConverter.GetBytes(v4.x));bytes.AddRange(BitConverter.GetBytes(v4.y));bytes.AddRange(BitConverter.GetBytes(v4.z));bytes.AddRange(BitConverter.GetBytes(v4.w));}
            else if(value is Quaternion q){bytes.AddRange(BitConverter.GetBytes(q.x));bytes.AddRange(BitConverter.GetBytes(q.y));bytes.AddRange(BitConverter.GetBytes(q.z));bytes.AddRange(BitConverter.GetBytes(q.w));}
            else if(value is bool || value is byte || value is sbyte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong)bytes.AddRange(Encoding.UTF8.GetBytes(Convert.ToString(value,CultureInfo.InvariantCulture)));
            else throw new InvalidOperationException("미지원 공개 control 타입입니다: "+value.GetType().FullName);
            return bytes.ToArray();
        }
        internal static bool SameBytes(byte[] left,byte[] right){if(left==null||right==null||left.Length!=right.Length)return false;for(var i=0;i<left.Length;i++)if(left[i]!=right[i])return false;return true;}
        private static string Hash(byte[] bytes){using(var hash=System.Security.Cryptography.SHA256.Create())return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-","");}
        private static string AssetSha(string path)
        {if(string.IsNullOrEmpty(path))return "";var full=Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Application.dataPath),path));for(var part=full;!string.IsNullOrEmpty(part);part=Path.GetDirectoryName(part))if((File.Exists(part)||Directory.Exists(part))&&(File.GetAttributes(part)&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("설정 asset 링크입니다.");return Hash(File.ReadAllBytes(full));}
        private static InputDevice[] CurrentDevices(){return new InputDevice[]{Keyboard.current,Mouse.current,Pointer.current};}
        internal OwnedBatchInputIsolation(IChapter1OwnedBatchInputContext lease,Action<bool> mark,Action<Func<double>> clock)
        {
            if(!Application.isEditor||!Application.isBatchMode||!Application.isPlaying||_active!=null||lease==null||!lease.TryObserve(out _identity)||_identity.IsolatedApi)throw new InvalidOperationException("격리 Setup 선행입니다.");
            _clock=clock;StartedReal=Time.realtimeSinceStartupAsDouble;_lease=lease;_mark=mark;_settings=InputSystem.settings;if(_settings==null)throw new InvalidOperationException("원 설정 부재입니다.");_settingsJson=JsonUtility.ToJson(_settings);_settingsName=_settings.name;_settingsFlags=_settings.hideFlags;_persistent=EditorUtility.IsPersistent(_settings);_assetPath=AssetDatabase.GetAssetPath(_settings);_assetSha=AssetSha(_assetPath);
            if(!_persistent && (_settingsFlags!=HideFlags.HideAndDontSave || !string.IsNullOrEmpty(_assetPath)))throw new InvalidOperationException("예상하지 않은 임시 설정입니다.");
            _devices=new List<InputDevice>(InputSystem.devices).ToArray();if(_devices.Length>16)throw new InvalidOperationException("장치 기준선 상한입니다.");_currents=CurrentDevices();_currentIds=new int[3];for(var c=0;c<3;c++)_currentIds[c]=_currents[c]==null?0:_currents[c].deviceId;_ids=new int[_devices.Length];_added=new bool[_devices.Length];_enabled=new bool[_devices.Length];_layouts=new string[_devices.Length];var all=new List<byte>();var types=new SortedDictionary<string,int>();
            for(var i=0;i<_devices.Length;i++){var device=_devices[i];_ids[i]=device.deviceId;_added[i]=device.added;_enabled[i]=device.enabled;_layouts[i]=device.layout;foreach(var control in device.allControls){if(_controls.Count>=1024)throw new InvalidOperationException("control 기준선 상한입니다.");var value=control.ReadValueAsObject();var raw=CanonicalValue(value);_controls.Add(control);_values.Add(raw);var type=value.GetType().FullName;types[type]=types.ContainsKey(type)?types[type]+1:1;all.AddRange(Encoding.UTF8.GetBytes(device.deviceId+"|"+control.path+"|"));all.AddRange(raw);}}
            _valueHash=Hash(all.ToArray());var text=new StringBuilder();foreach(var pair in types)text.Append(pair.Key).Append(':').Append(pair.Value).Append(';');_types=text.ToString();Log("baseline",false,false,false);
            InputSystem.onBeforeUpdate+=OriginalProbe;AssemblyReloadEvents.beforeAssemblyReload+=Emergency;EditorApplication.playModeStateChanged+=OnMode;_registered=true;_active=this;
            try{_setupAttempted=true;_originalAtSetup=_originalTicks;_api.Setup();_setupComplete=true;if(InputSystem.devices.Count!=0)throw new InvalidOperationException("빈 시험계가 아닙니다.");_mark(true);_clockActive=true;_clock(ReadClock);InputSystem.onBeforeUpdate+=TestProbe;Log("setup",false,false,false);}
            catch(Exception error){Fatal("Setup 불완전: "+error.GetType().Name);throw;}
        }
        private double ReadClock()
        {Chapter1OwnedBatchInputEvidence current;if(!_clockActive || !_setupComplete || _restored || !ReferenceEquals(_active,this) || !_lease.TryObserve(out current) || current.Epoch!=_identity.Epoch || current.CaseName!=_identity.CaseName || current.RunId!=_identity.RunId || current.Phase!=_identity.Phase || current.SourceSha!=_identity.SourceSha || current.OwnerPid!=_identity.OwnerPid || current.OwnerBirth!=_identity.OwnerBirth)throw new InvalidOperationException("정확 격리 시계 수명 밖입니다.");return _api.currentTime;}
        internal void BindOwnedDevices(Chapter1VerificationInputBoundarySnapshot snapshot)
        {if(!_setupComplete||_restored||InputSystem.devices.Count!=2)throw new InvalidOperationException("정확 소유2장치가 아닙니다.");_keyboard=InputSystem.GetDeviceById(snapshot.KeyboardId) as Keyboard;_mouse=InputSystem.GetDeviceById(snapshot.MouseId) as Mouse;if(_keyboard==null||_mouse==null||ReferenceEquals(_keyboard,_mouse))throw new InvalidOperationException("소유 장치 영수증 불일치입니다.");}
        private void OriginalProbe(){if(InputState.currentUpdateType!=InputUpdateType.Dynamic)return;_originalTicks++;if(_setupComplete&&!_restored)Fatal("격리 중 원 probe 호출입니다.");}
        private void TestProbe(){if(InputState.currentUpdateType!=InputUpdateType.Dynamic)return;_testTicks++;if(_restored){Fatal("복귀 뒤 시험 probe 호출입니다.");return;}if(_keyboard!=null && (InputSystem.devices.Count!=2||!_keyboard.added||!_mouse.added||!ReferenceEquals(InputSystem.GetDeviceById(_keyboard.deviceId),_keyboard)||!ReferenceEquals(InputSystem.GetDeviceById(_mouse.deviceId),_mouse)))Fatal("시험 장치 집합 변경입니다.");}
        private void Log(string stage,bool reference,bool values,bool currents)
        {var current=CurrentDevices();var currentIds=new int[3];var nulls=new bool[3];for(var c=0;c<3;c++){nulls[c]=current[c]==null;currentIds[c]=nulls[c]?0:current[c].deviceId;}var row=new Record{clockProviderInvalidated=!_clockActive,clockReadRejected=_clockReadRejected,stage=stage,baselineDeviceIds=_ids,currentIds=currentIds,currentNulls=nulls,caseName=_identity.CaseName,phase=_identity.Phase,sourceSha=_identity.SourceSha,ownerPid=_identity.OwnerPid,ownerBirth=_identity.OwnerBirth,devices=_devices.Length,controls=_controls.Count,controlHash=_valueHash,types=_types,persistent=_persistent,originalReferenceRestored=reference,valueEquivalentRestored=values,currentRestored=currents,originalTicks=_originalTicks,testTicks=_testTicks,error=Error};var json=JsonUtility.ToJson(row);if(stage=="restored" || stage=="probe-restored")_lease.Budget.AddCleanup("isolation "+json);else _lease.Budget.Add("isolation "+json);Debug.Log("[DISO] "+json);}
        private void OnMode(PlayModeStateChange state){if(state==PlayModeStateChange.ExitingPlayMode && !_finished)Emergency();}
        private void Emergency(){_clockActive=false;_clock(null);if(_finished)return;Fatal("종료/reload 예고 정리 성공 미입증입니다.");try{EmergencyCleanup?.Invoke();}catch(Exception error){Fatal("예고 소유정리: "+error.GetType().Name);}try{Restore();}catch(Exception error){Fatal("예고 복귀: "+error.GetType().Name);}}
        private void Fatal(string reason){Error=reason;Debug.LogError("[DISO_FATAL] "+reason);}
        internal void MarkExpectedBodyFailure(){Log("expected-body-failure",false,false,false);}
        internal void Restore()
        {
            _clockActive=false;_clock(null);if(_restored)return;_lease.Dispose();_mark(false);if(!_setupAttempted||!_setupComplete){Fatal("부분 Setup 복귀 미입증입니다.");return;}
            try{InputSystem.onBeforeUpdate-=TestProbe;_testAtRestore=_testTicks;_api.TearDown();_restored=true;
                if(_persistent){if(InputSystem.settings.name!=_settingsName||InputSystem.settings.hideFlags!=_settingsFlags||AssetDatabase.GetAssetPath(InputSystem.settings)!=_assetPath||!ReferenceEquals(InputSystem.settings,_settings)||JsonUtility.ToJson(InputSystem.settings)!=_settingsJson||AssetSha(_assetPath)!=_assetSha)throw new InvalidOperationException("영속 설정 복귀 불일치입니다.");}
                else{var copy=ScriptableObject.CreateInstance<InputSettings>();JsonUtility.FromJsonOverwrite(_settingsJson,copy);copy.name=_settingsName;copy.hideFlags=_settingsFlags;InputSystem.settings=copy;if(EditorUtility.IsPersistent(copy)||!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(copy))||JsonUtility.ToJson(copy)!=_settingsJson||copy.name!=_settingsName||copy.hideFlags!=_settingsFlags)throw new InvalidOperationException("임시 설정 값동등 복귀 불일치입니다.");}
                if(InputSystem.devices.Count!=_devices.Length)throw new InvalidOperationException("원 장치수 복귀 불일치입니다.");for(var i=0;i<_devices.Length;i++){var d=_devices[i];if(!ReferenceEquals(InputSystem.devices[i],d)||d.deviceId!=_ids[i]||InputSystem.GetDeviceById(d.deviceId)!=d||d.added!=_added[i]||d.enabled!=_enabled[i]||d.layout!=_layouts[i])throw new InvalidOperationException("원 장치 복귀 불일치입니다.");}
                var currents=CurrentDevices();for(var i=0;i<3;i++)if(!ReferenceEquals(currents[i],_currents[i])||(currents[i]==null?0:currents[i].deviceId)!=_currentIds[i])throw new InvalidOperationException("현재 장치 선택 복귀 불일치입니다.");for(var i=0;i<_controls.Count;i++)if(!SameBytes(_values[i],CanonicalValue(_controls[i].ReadValueAsObject())))throw new InvalidOperationException("공개 control 정확값 복귀 불일치입니다.");
                if(_originalTicks!=_originalAtSetup)throw new InvalidOperationException("격리 중 원갱신이 관측됐습니다.");_originalAtRestore=_originalTicks;_restoreTime=Time.realtimeSinceStartupAsDouble;Log("restored",_persistent,true,true);
            }catch(Exception error){Fatal("복귀 불명: "+error.GetType().Name);Detach();throw;}
        }
        internal IEnumerator ConfirmRestored()
        {
            try{if(!_restored)throw new InvalidOperationException("복귀되지 않은 격리입니다.");while(_originalTicks-_originalAtRestore<2){if(Error!=null||Time.realtimeSinceStartupAsDouble-_restoreTime>=2||_originalTicks-_originalAtRestore>=4)throw new InvalidOperationException("원 자연 Dynamic 재개 미입증입니다.");yield return null;}if(_testTicks!=_testAtRestore)throw new InvalidOperationException("시험 구독 누수입니다.");Chapter1OwnedBatchInputEvidence e;double rejectedTime;if(_lease.TryReadIsolatedTime(out rejectedTime))throw new InvalidOperationException("복귀 뒤 시계 읽기 허용입니다.");_clockReadRejected=true;if(_lease.TryObserve(out e))throw new InvalidOperationException("해제 뒤 lease 허용입니다.");if(_identity.CaseName.Contains(".OwnedInputIsolationRestores") && Time.realtimeSinceStartupAsDouble-StartedReal>=20)throw new InvalidOperationException("격리 smoke 내부20초 상한입니다.");Log("probe-restored",_persistent,true,true);_finished=true;}
            finally{Detach();}
        }
        private void Detach(){if(_registered){InputSystem.onBeforeUpdate-=OriginalProbe;AssemblyReloadEvents.beforeAssemblyReload-=Emergency;EditorApplication.playModeStateChanged-=OnMode;_registered=false;}if(ReferenceEquals(_active,this))_active=null;}
    }
    public sealed class OwnedBatchRuntimeCoroutineHost : MonoBehaviour
    {
        private readonly System.Collections.Generic.Stack<IEnumerator> _stack=new System.Collections.Generic.Stack<IEnumerator>();
        private readonly System.Collections.Generic.List<IEnumerator> _disposed=new System.Collections.Generic.List<IEnumerator>();
        private readonly System.Collections.Generic.List<System.Runtime.ExceptionServices.ExceptionDispatchInfo> _secondary=new System.Collections.Generic.List<System.Runtime.ExceptionServices.ExceptionDispatchInfo>();
        private System.Runtime.ExceptionServices.ExceptionDispatchInfo _first;
        private Coroutine _routine;
        private string _testId;
        public bool IsDone {get;private set;}
        internal int PendingEnumeratorCount=>_stack.Count;
        internal int DisposedEnumeratorCount=>_disposed.Count;
        public void Begin(IEnumerator body,string testId)
        {
            if(body==null || _stack.Count!=0 || _routine!=null)throw new InvalidOperationException("잘못된 시험 본문 또는 중복 시작입니다.");
            _testId=testId;_stack.Push(body);_routine=StartCoroutine(Drive());
        }
        private void Remember(Exception error)
        {
            if(error==null)return;
            var captured=System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error);
            if(_first==null)_first=captured;else _secondary.Add(captured);
        }
        private void DisposeOnce(IEnumerator iterator)
        {
            foreach(var disposed in _disposed)if(ReferenceEquals(disposed,iterator))return;
            _disposed.Add(iterator);
            try{(iterator as IDisposable)?.Dispose();}catch(Exception error){Remember(error);}
        }
        private IEnumerator Drive()
        {
            try
            {
                while(_stack.Count>0 && _first==null)
                {
                    object yielded=null;var advanced=false;
                    try
                    {
                        if(TestContext.CurrentContext.Test.ID!=_testId)throw new InvalidOperationException("NUnit 사례 문맥이 변경됐습니다.");
                        var iterator=_stack.Peek();advanced=iterator.MoveNext();
                        if(advanced)yielded=iterator.Current;
                    }
                    catch(Exception error){Remember(error);}
                    if(_first!=null)break;
                    if(!advanced){DisposeOnce(_stack.Pop());continue;}
                    // CustomYieldInstruction도 IEnumerator다. keepWaiting/Current 예외를 같은 경계에서 포착한다.
                    if(yielded is IEnumerator nested)
                    {
                        var cycle=false;foreach(var active in _stack)if(ReferenceEquals(active,nested)){cycle=true;break;}
                        if(cycle || _stack.Count>=64){Remember(new InvalidOperationException("시험 열거자 순환 또는 중첩64 초과입니다."));break;}
                        _stack.Push(nested);continue;
                    }
                    if(yielded==null || yielded is WaitForFixedUpdate || yielded is WaitForSeconds || yielded is WaitForEndOfFrame || yielded is AsyncOperation)
                        yield return yielded;
                    else {Remember(new NotSupportedException("지원하지 않는 시험 yield: "+yielded.GetType().FullName));break;}
                }
            }
            finally{while(_stack.Count>0)DisposeOnce(_stack.Pop());IsDone=true;}
        }
        public void Abort(Exception reason)
        {
            Remember(reason);
            if(_routine!=null){StopCoroutine(_routine);_routine=null;}
            while(_stack.Count>0)DisposeOnce(_stack.Pop());
            IsDone=true;
        }
        public void ThrowFirstFailure()
        {
            if(_first==null)return;
            // 후속 정리 오류는 첫 예외를 대체하지 않고 그 원문 객체를 보유한다. 로그로 재발행하지 않는다.
            if(_secondary.Count>0)try{_first.SourceException.Data["CH1SecondaryCleanupFailures"]=_secondary.ToArray();}catch{}
            _first.Throw();
        }
        private void OnDestroy(){if(!IsDone)Abort(new InvalidOperationException("시험 실행 개체가 예기치 않게 파괴됐습니다."));}
    }

    // AC-AUTOCH1-G5BRIDGE-PREP-001: 고정44와 별도 발견하는 합성 실행기 경계 fixture.
    internal sealed class OwnedBatchRuntimeFixtureScopeBridgeTests
    {
        [SerializeField] private OwnedBatchRuntimeFixtureScope _fixture=new OwnedBatchRuntimeFixtureScope();
        [UnitySetUp] public IEnumerator EnterCase()
        {_fixture.PrepareEnter();yield return new EnterPlayMode();_fixture.ConfirmEntered();yield return _fixture.RunBody(_fixture.BeginCase());}
        [UnityTearDown] public IEnumerator ExitCase()
        {
            if(_fixture.OwnsRuntimeFixture && Application.isPlaying)yield return _fixture.RunBody(_fixture.CloseCase(),true);
            if(_fixture.OwnsRuntimeFixture)
            {if(Application.isPlaying)yield return new ExitPlayMode();_fixture.CaptureCleanupFailure(_fixture.CompleteExit);}
            _fixture.ThrowCleanupFailure();
        }
        private sealed class CaptureFailure : IEnumerator,IDisposable
        {
            private readonly IEnumerator _iterator;
            public Exception Failure {get;private set;}
            public CaptureFailure(IEnumerator iterator){_iterator=iterator;}
            public object Current=>_iterator.Current;
            public bool MoveNext(){try{return _iterator.MoveNext();}catch(Exception error){Failure=error;return false;}}
            public void Reset(){throw new NotSupportedException();}
            public void Dispose(){(_iterator as IDisposable)?.Dispose();}
        }
        private sealed class FaultingEnumerator : IEnumerator,IDisposable
        {
            private readonly string _stage;
            private readonly System.Collections.Generic.List<string> _events;
            private bool _moved;
            public Exception Original {get;private set;}
            public FaultingEnumerator(string stage,System.Collections.Generic.List<string> events){_stage=stage;_events=events;}
            private void ThrowOriginal()
            {
                try
                {if(_stage=="MoveNext")Assert.Fail("CH1_BRIDGE_ORIGINAL_ASSERT");throw new InvalidOperationException("CH1_BRIDGE_ORIGINAL_"+_stage);}
                catch(Exception error){Original=error;throw;}
            }
            public bool MoveNext(){if(_stage=="MoveNext")ThrowOriginal();if(_moved)return false;_moved=true;return true;}
            public object Current {get{if(_stage=="Current")ThrowOriginal();return null;}}
            public void Reset(){throw new NotSupportedException();}
            public void Dispose(){_events.Add("inner-dispose");if(_stage=="Dispose")ThrowOriginal();else throw new InvalidOperationException("CH1_BRIDGE_SECONDARY_DISPOSE");}
        }
        private static IEnumerator WithOuterFinally(IEnumerator inner,System.Collections.Generic.List<string> events)
        {try{yield return inner;}finally{events.Add("outer-finally");}}
        [UnityTest]
        public IEnumerator NestedFailuresPreserveFirstStackAndFinally([Values("MoveNext","Current","Dispose")] string stage)
        {
            var events=new System.Collections.Generic.List<string>();var inner=new FaultingEnumerator(stage,events);
            OwnedBatchRuntimeCoroutineHost host=null;
            var capture=new CaptureFailure(_fixture.RunBodyForProbe(WithOuterFinally(inner,events),null,value=>host=value));
            yield return capture;
            Assert.That(capture.Failure,Is.SameAs(inner.Original));Assert.That(capture.Failure.StackTrace,Does.Contain("ThrowOriginal"));
            if(stage=="MoveNext")Assert.That(capture.Failure,Is.InstanceOf<AssertionException>());
            if(stage!="Dispose")Assert.That(capture.Failure.Data.Contains("CH1SecondaryCleanupFailures"),Is.True);
            Assert.That(events,Is.EqualTo(new[]{"inner-dispose","outer-finally"}));
            Assert.That(host.IsDone,Is.True);Assert.That(host.PendingEnumeratorCount,Is.Zero);Assert.That(host.DisposedEnumeratorCount,Is.EqualTo(2));Assert.That(host==null,Is.True);
        }
        private sealed class CountedYield : CustomYieldInstruction
        {
            public int Polls;private readonly Exception _failure;
            public CountedYield(Exception failure=null){_failure=failure;}
            public override bool keepWaiting {get{Polls++;if(_failure!=null)throw _failure;return Polls<3;}}
        }
        private static IEnumerator YieldValue(object value){yield return value;}
        [UnityTest] public IEnumerator CustomYieldAndUnsupportedValueKeepFailureMeaning()
        {
            var wait=new CountedYield();yield return _fixture.RunBodyForProbe(YieldValue(wait),null,null);Assert.That(wait.Polls,Is.EqualTo(3));
            var original=new InvalidOperationException("CH1_BRIDGE_CUSTOM_YIELD");
            var customFailure=new CaptureFailure(_fixture.RunBodyForProbe(YieldValue(new CountedYield(original)),null,null));yield return customFailure;
            Assert.That(customFailure.Failure,Is.SameAs(original));Assert.That(original.StackTrace,Does.Contain("get_keepWaiting"));
            var unsupported=new CaptureFailure(_fixture.RunBodyForProbe(YieldValue(new object()),null,null));yield return unsupported;
            Assert.That(unsupported.Failure,Is.TypeOf<NotSupportedException>());
        }
        private static IEnumerator ExpectedLogBody(string testId)
        {
            Assert.That(TestContext.CurrentContext.Test.ID,Is.EqualTo(testId));
            LogAssert.Expect(LogType.Error,"CH1_BRIDGE_EXPECTED_LOG_ONCE");Debug.LogError("CH1_BRIDGE_EXPECTED_LOG_ONCE");
            yield return new WaitForFixedUpdate();Assert.That(TestContext.CurrentContext.Test.ID,Is.EqualTo(testId));
        }
        [UnityTest] public IEnumerator ExpectedLogIsConsumedOnceInOriginalCase()
        {
            var id=TestContext.CurrentContext.Test.ID;yield return _fixture.RunBodyForProbe(ExpectedLogBody(id),null,null);
            Assert.That(TestContext.CurrentContext.Test.ID,Is.EqualTo(id));LogAssert.NoUnexpectedReceived();
        }
        private static IEnumerator UnexpectedLogBody(){Debug.LogError("CH1_BRIDGE_UNEXPECTED_LOG_PROBE");yield return null;}
        // 실행기 Failed가 정확 기대값인 별도 탐침. 정상 통과 집계에 합산하지 않는다.
        [UnityTest] public IEnumerator UnexpectedLogRemainsRunnerFailure()
        {yield return _fixture.RunBodyForProbe(UnexpectedLogBody(),null,null);}
        private static IEnumerator EndlessBody(Action disposed)
        {try{while(true)yield return null;}finally{disposed();}}
        [UnityTest] public IEnumerator OwnedHostLossIsFailureAndDisposesOnce()
        {
            var disposals=0;OwnedBatchRuntimeCoroutineHost host=null;
            var capture=new CaptureFailure(_fixture.RunBodyForProbe(EndlessBody(()=>disposals++),null,value=>{host=value;UnityEngine.Object.Destroy(value.gameObject);}));
            yield return capture;
            Assert.That(capture.Failure,Is.TypeOf<InvalidOperationException>());Assert.That(capture.Failure.StackTrace,Is.Not.Null.And.Not.Empty);
            Assert.That(disposals,Is.EqualTo(1));Assert.That(host.IsDone,Is.True);Assert.That(host.PendingEnumeratorCount,Is.Zero);Assert.That(host==null,Is.True);
        }
        [UnityTest] public IEnumerator OneRealTimeoutStopsAndDisposesOwnedBody()
        {
            var disposals=0;OwnedBatchRuntimeCoroutineHost host=null;var watch=System.Diagnostics.Stopwatch.StartNew();
            var capture=new CaptureFailure(_fixture.RunBodyForProbe(EndlessBody(()=>disposals++),0.25,value=>host=value));
            yield return capture;
            Assert.That(capture.Failure,Is.TypeOf<TimeoutException>());Assert.That(watch.Elapsed.TotalSeconds,Is.GreaterThanOrEqualTo(0.25));Assert.That(watch.Elapsed.TotalSeconds,Is.LessThanOrEqualTo(2));
            Assert.That(disposals,Is.EqualTo(1));Assert.That(host.IsDone,Is.True);Assert.That(host.PendingEnumeratorCount,Is.Zero);Assert.That(host.DisposedEnumeratorCount,Is.EqualTo(1));Assert.That(host==null,Is.True);
        }
    }

    // REQ/AC-PRACE-004: pure evidence contract; no PlayMode entry or input runtime setup.
    public sealed class BootstrapEvidenceContractTests
    {
        private static string[] Args(string id="parent-race-contract-01")
        {
            var root=Path.Combine("H:/GPT-workspace/AcadeGameMaker-integration-20261107/output/ch1-standalone-owner-platform-20261104","attempts",id);
            return new[]{"-autonomousOwnedBatchEvidenceId",id,"-autonomousOwnedBatchGate",Path.Combine(root,"luna-tools-review.json"),"-autonomousOwnedBatchGateSha",new string('A',64),"-autonomousOwnedBatchExecutionReceipt",Path.Combine(root,"execution/bootstrap-contract/started.json"),"-autonomousOwnedBatchPlan",Path.Combine(root,"execution/bootstrap-contract/plan.json"),"-autonomousOwnedBatchSourceManifest",Path.Combine(root,"execution/bootstrap-contract/source-manifest.json")};
        }
        private static void Check(string[] args,string evidenceId="parent-race-contract-01",string gateSha=null,string sourceSha=null)
        {var plan=Args();OwnedBatchRuntimeFixtureScope.ValidateEvidenceContract(args,plan,evidenceId,plan[3],gateSha??new string('A',64),new string('A',64),sourceSha??new string('B',64),new string('B',64),"bootstrap-contract");}
        [Test,Timeout(20000)] public void ValidEvidenceSelection(){Assert.DoesNotThrow(()=>Check(Args()));}
        [Test,Timeout(20000)] public void RejectsDuplicateEvidenceArguments(){var values=new List<string>(Args());values.AddRange(new[]{"-autonomousOwnedBatchEvidenceId","parent-race-contract-01"});Assert.Throws<InvalidOperationException>(()=>Check(values.ToArray()));}
        [Test,Timeout(20000)] public void RejectsMixedEvidenceId(){Assert.Throws<InvalidOperationException>(()=>Check(Args(),"parent-race-smoke-01"));}
        [Test,Timeout(20000)] public void RejectsGateShaMismatch(){Assert.Throws<InvalidOperationException>(()=>Check(Args(),gateSha:new string('C',64)));}
        [Test,Timeout(20000)] public void RejectsManifestSourceMismatch(){Assert.Throws<InvalidOperationException>(()=>Check(Args(),sourceSha:new string('C',64)));}
        [Test,Timeout(20000)] public void RejectsRootEscape(){Assert.Throws<InvalidOperationException>(()=>Check(Args("../escape")));}
    }}
