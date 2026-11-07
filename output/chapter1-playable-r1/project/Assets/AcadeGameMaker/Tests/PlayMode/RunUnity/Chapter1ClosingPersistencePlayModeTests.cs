using System;
using System.Collections;
using System.IO;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using AcadeGameMaker.Run.Unity;
using AcadeGameMaker.Tests.PlayMode.RunUnity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Run.Unity.PlayMode.Tests
{
    // AC-CHOICE-001/002/003, AC-CH1-003/004/006/007. 합성 실제 장치·저장·표시 경로의 검사다.
    public sealed class Chapter1ClosingPersistencePlayModeTests
    {
        private Chapter1AuthoredInputTrace _trace;
        private string _runId;
        // AC-AUTOCH1-SAMEBUILD-004: native public-input fixture, no mock runtime replacement.
        private InputSettings _settings;
        private InputSettings.EditorInputBehaviorInPlayMode _previousEditorInput;
        private InputSettings.BackgroundBehavior _previousBackground;
        private InputDevice[] _priorDevices;
        [SetUp] public void IsolateProfile()
        {
            _trace=null;_runId="closing-"+Guid.NewGuid().ToString("N");
            DesktopProfileLaunchAdapterV1.ConfigureChapter1TestRun(_runId);
            _settings=InputSystem.settings;_previousEditorInput=_settings.editorInputBehaviorInPlayMode;_previousBackground=_settings.backgroundBehavior;
            _priorDevices=InputSystem.devices.ToArray();
            _settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            _settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        }
        public void TearDown(){Chapter1AuthoredInputTrace.LogStage("postboss.native-close-deferred");}
        [UnityTearDown] public IEnumerator Close()
        {
            try{if(_trace!=null)yield return _trace.CloseForReopen();}
            finally
            {
                _trace=null;
                _settings.editorInputBehaviorInPlayMode=_previousEditorInput;_settings.backgroundBehavior=_previousBackground;
                Assert.That(ReferenceEquals(InputSystem.settings,_settings),Is.True);
                Assert.That(InputSystem.settings.editorInputBehaviorInPlayMode,Is.EqualTo(_previousEditorInput));
                Assert.That(InputSystem.settings.backgroundBehavior,Is.EqualTo(_previousBackground));
                Assert.That(InputSystem.devices.Count,Is.EqualTo(_priorDevices.Length));
                foreach(var device in _priorDevices)Assert.That(InputSystem.GetDeviceById(device.deviceId),Is.SameAs(device));
                Chapter1AuthoredInputTrace.LogStage("postboss.native-settings-devices-restored");
            }
        }
        private IEnumerator OpenChoice(){_trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();yield return _trace.PlayToChoice();}
        [UnityTest] public IEnumerator UiSubmitClickHeldCancelAndDuplicate()
        {
            yield return OpenChoice();var adapter=_trace.Flow.Lifetime.CurrentChoiceAdapter;
            var before=Chapter1AuthoredInputTrace.ReadCurrentTestProfile();
            yield return _trace.ChoiceClick(0,true,true);for(var i=0;i<8;i++)yield return new WaitForFixedUpdate();
            Assert.That(adapter.UiPhase,Is.EqualTo(Chapter1ChoiceUiPhase.ExtractionProposed));
            Assert.That(Chapter1AuthoredInputTrace.ReadCurrentTestProfile().ProfileRevision,Is.EqualTo(before.ProfileRevision));
            _trace.Keyboard();_trace.Pointer(Vector2.zero,false);yield return _trace.ChoiceClick(3);
            Assert.That(adapter.UiPhase,Is.EqualTo(Chapter1ChoiceUiPhase.Required));
            yield return _trace.ChoiceClick(1);Assert.That(adapter.UiPhase,Is.EqualTo(Chapter1ChoiceUiPhase.SolidarityProposed));
            var primary=Path.Combine(Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot),"profile.json");
            using(var held=new FileStream(primary,FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                yield return _trace.ChoiceClick(2,false,true);for(var i=0;i<8;i++)yield return new WaitForFixedUpdate();
                RecordSaveDiagnostic("failed-return",before.ProfileRevision);
                Assert.That(adapter.UiPhase,Is.EqualTo(Chapter1ChoiceUiPhase.SaveFailed));Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.ChoiceRequired));
                Assert.That(_trace.Flow.CurrentChapter1CompletionReceipt,Is.Null);
            }
            // 잠금 해제·held 자체는 저장 재시도나 기술 획득을 만들지 않는다.
            for(var i=0;i<6;i++)yield return new WaitForFixedUpdate();Assert.That(adapter.UiPhase,Is.EqualTo(Chapter1ChoiceUiPhase.SaveFailed));
            _trace.Keyboard();_trace.Pointer(Vector2.zero,false);RecordSaveDiagnostic("retry-before",before.ProfileRevision);
            try{yield return _trace.ChoiceClick(4);yield return _trace.WaitPhase(Chapter1RunPhase.AtBarrier);}
            finally{RecordSaveDiagnostic("retry-result-or-failure",before.ProfileRevision);}
            var after=Chapter1AuthoredInputTrace.ReadCurrentTestProfile();Assert.That(after.Progression.CommittedChoice,Is.EqualTo(ProfileChoice.Solidarity));
            Assert.That(after.ProfileRevision,Is.EqualTo(before.ProfileRevision+1));Assert.That(after.Progression.CompletedBranches,Is.Empty);
            Assert.That(_trace.Flow.LastSaveResult.HasValue,Is.True);Assert.That(_trace.Flow.LastSaveResult.Value.Outcome,Is.EqualTo(ProfileAtomicSaveOutcome.CommittedReplacement));Assert.That(_trace.Flow.LastSaveResult.Value.CommittedRevision,Is.EqualTo(before.ProfileRevision+1));
            Assert.That(_trace.Flow.LastChoiceRetryStatus,Does.StartWith("Saved:"));
            var pending=ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(before.Settings,before.Input,before.Tutorial,new ProfileProgressionSnapshot(before.ProfileRevision+1,before.Progression.LastOfferedSeed,ProfileChoice.Solidarity,(ProfileConsentState)3,(ProfileSkill)2,before.Progression.CompletedBranches)));
            var recovered=Directory.GetFiles(Path.Combine(Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot),"recovery"));Assert.That(recovered.Length,Is.EqualTo(1));Assert.That(File.ReadAllBytes(recovered[0]),Is.EqualTo(pending.CanonicalFileBytes));
        }
        private void RecordSaveDiagnostic(string stage,long beforeRevision)
        {
            _trace.RecordC1cDiagnostic(stage,()=>
            {
                var text="runId="+_runId+" beforeRevision="+beforeRevision+" privateProfileReadiness=unobserved-no-supported-access";
                text+=" flow="+Chapter1AuthoredInputTrace.C1cRead(()=>_trace.Flow.View.Phase+"/error="+_trace.Flow.View.Error)+" ui="+Chapter1AuthoredInputTrace.C1cRead(()=>_trace.Flow.Lifetime.CurrentChoiceAdapter.UiPhase.ToString());
                text+=" save="+Chapter1AuthoredInputTrace.C1cRead(()=>{var result=_trace.Flow.LastSaveResult;if(!result.HasValue)return "unpublished";var value=result.Value;return value.Outcome+"/stage="+value.FailureStage+"/revision="+value.CommittedRevision+"/primary="+value.PrimaryState+"/previous="+value.PreviousState+"/temp="+value.TempState;});
                foreach(var leaf in new[]{"profile.json","profile.prev.json","profile.tmp.json"})text+=" "+leaf+"="+Chapter1AuthoredInputTrace.C1cRead(()=>
                {
                    var root=Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot);var path=Path.Combine(root,leaf);
                    if(!File.Exists(path))return "absent";
                    using(var file=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.None))
                    {var length=file.Length;if(length>1024*1024)return "exists/length="+length+"/hash=unobserved-size-limit";using(var sha=System.Security.Cryptography.SHA256.Create()){var hash=BitConverter.ToString(sha.ComputeHash(file)).Replace("-","");return "exists/length="+length+"/closedHandleSHA256="+hash;}}
                });
                return text;
            });
        }
        [UnityTest] public IEnumerator AuthoredBarrierStartsAboveFloorWithOpenSight([ValueSource(nameof(Branches))] int branch)
        {
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.OpenBarrier(branch);
            var graph=_trace.BarrierGraph;var router=_trace.Flow.Lifetime.CurrentRouter;var player=router.BoundPlayerForAuthoring;var body=player.GetComponent<CapsuleCollider2D>();
            var floor=graph.transform.Find("LeftFloor").GetComponent<BoxCollider2D>();var target=graph.transform.Find("Counterweight");Assert.That(body,Is.Not.Null);Assert.That(floor,Is.Not.Null);Assert.That(target,Is.Not.Null);
            Assert.That(_trace.Flow.Lifetime.TryGetCompletedEvidence(out var completed),Is.True);
            Assert.That(body.bounds.min.y,Is.EqualTo(floor.bounds.max.y).Within(.02f));Assert.That(graph.transform.InverseTransformPoint(body.bounds.center).x,Is.EqualTo(0).Within(.02f));
            Assert.That(completed.Position.y,Is.GreaterThan(floor.bounds.max.y));Assert.That(completed.Position.x,Is.EqualTo(player.transform.position.x).Within(.001f));Assert.That(completed.Position.y,Is.EqualTo(player.transform.position.y).Within(.001f));
            var filter=new ContactFilter2D();filter.NoFilter();filter.useLayerMask=true;filter.layerMask=1<<8;filter.useTriggers=false;var hits=new RaycastHit2D[64];
            var count=Physics2D.Linecast(completed.Position,target.position,filter,hits);Assert.That(count,Is.LessThan(hits.Length));
            for(var i=0;i<count;i++){var collider=hits[i].collider;if(collider==null||collider.isTrigger||collider.transform==player.transform||collider.transform.IsChildOf(player.transform)||collider.transform==target||collider.transform.IsChildOf(target))continue;Assert.Fail("저작 입구의 실제 시야가 막혔습니다: "+collider.name);}
            yield return _trace.TransferToCounterweight();Assert.That(_trace.Flow.Lifetime.TryGetCompletedEvidence(out var transferred),Is.True);Assert.That(transferred.TransferTarget,Is.EqualTo("BarrierCounterweight"));
        }
        private static readonly int[] Branches={1,2};
        [UnityTest] public IEnumerator BothBranchesCloseOnlyAfterRealPassageAndSave([ValueSource(nameof(Branches))] int branch)
        {
                _trace=new Chapter1AuthoredInputTrace();yield return _trace.OpenBarrier(branch);yield return _trace.UseBarrierSkill();
                Assert.That(_trace.Flow.CurrentChapter1CompletionReceipt,Is.Null);Assert.That(_trace.Flow.RunEndCommittedRevision,Is.Zero);
                yield return _trace.PassBarrier();Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));
                Assert.That(_trace.Flow.CurrentChapter1CompletionReceipt,Is.Null);
                var saved=Chapter1AuthoredInputTrace.ReadCurrentTestProfile();Assert.That(saved.Progression.CompletedBranches,Contains.Item((ProfileBranch)branch));
                // 최초 줄의 60틱 미만 edge 및 그 뒤 held는 줄을 넘기지 못한다.
                var first=_trace.ClosingStatus();yield return _trace.ClosingEdge(false,true);
                for(var i=0;i<65;i++)yield return new WaitForFixedUpdate();Assert.That(_trace.ClosingStatus(),Is.EqualTo(first));
                Assert.That(_trace.Flow.CurrentChapter1CompletionReceipt,Is.Null);_trace.Keyboard();yield return _trace.FinishClosing(branch);
                var receipt=_trace.Flow.CurrentChapter1CompletionReceipt;for(var i=0;i<6;i++)yield return new WaitForFixedUpdate();
                Assert.That(_trace.Flow.CurrentChapter1CompletionReceipt,Is.SameAs(receipt));Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));
        }
        private static readonly bool[] CompletionCases={false,true};
        [UnityTest] public IEnumerator RejectForgedIncompleteOrUncertainCompletion([ValueSource(nameof(CompletionCases))] bool uncertain)
        {
            yield return OpenChoice();Assert.Throws<InvalidOperationException>(()=>_trace.Flow.TryCommitChapterCompletion(null,null,null));
            if(uncertain)
            {
                yield return _trace.ChoiceClick(0);var adapter=_trace.Flow.Lifetime.CurrentChoiceAdapter;
                var previous=Path.Combine(Chapter1DevelopmentProfile.ValidateExactRoot(Chapter1DevelopmentProfile.ExactRoot),"profile.prev.json");
                Assert.That(File.Exists(previous),Is.True);
                using(var held=new FileStream(previous,FileMode.Open,FileAccess.Read,FileShare.Read))yield return _trace.ChoiceClick(2,false,true);
                Assert.That(_trace.Flow.LastSaveResult.HasValue,Is.True);Assert.That(_trace.Flow.LastSaveResult.Value.Outcome,Is.EqualTo(ProfileAtomicSaveOutcome.CommitOutcomeUncertain));
                Assert.That(adapter.UiPhase,Is.EqualTo(Chapter1ChoiceUiPhase.SaveUncertain));Assert.That(_trace.Flow.CurrentChapter1CompletionReceipt,Is.Null);
                for(var i=0;i<8;i++)yield return new WaitForFixedUpdate();Assert.That(adapter.UiPhase,Is.EqualTo(Chapter1ChoiceUiPhase.SaveUncertain));
                Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.ChoiceRequired));Assert.That(_trace.Flow.RunEndCommittedRevision,Is.Zero);yield break;
            }
            yield return _trace.SelectBranch(1);yield return _trace.UseBarrierSkill();yield return _trace.PassBarrier();
            var flow=_trace.Flow;var entry=flow.Lifetime.CurrentChoiceAdapter.ClosingHostEntry;Assert.That(entry,Is.Not.Null);
            var demo=(Chapter1DemoEndReceipt)entry.Demo;var save=(Chapter1BranchSaveReceipt)entry.BranchSave;
            // 실제 DemoCompleted와 저장만으로는 CH1 완료 권위를 만들지 않는다.
            Assert.That(flow.ValidatePendingChapter1SuccessfulRun(demo,save),Is.True);Assert.That(flow.CurrentChapter1CompletionReceipt,Is.Null);
            var fakeDemo=new Chapter1DemoEndReceipt(flow,demo.RunKey,demo.Generation,demo.Branch,demo.CommittedRunEndRevision);
            var wrongGeneration=new Chapter1DemoEndReceipt(flow,demo.RunKey,demo.Generation+1,demo.Branch,demo.CommittedRunEndRevision);
            var wrongBranch=new Chapter1DemoEndReceipt(flow,demo.RunKey,demo.Generation,ProfileBranch.Solidarity,demo.CommittedRunEndRevision);
            foreach(var forged in new[]{fakeDemo,wrongGeneration,wrongBranch})
            {Assert.That(flow.ValidatePendingChapter1SuccessfulRun(forged,save),Is.False);Assert.Throws<InvalidOperationException>(()=>flow.TryCommitChapterCompletion(forged,null,save));}
            var fakeClosing=new Chapter1ClosingReceipt(new object(),demo,entry,new string('A',64),1,1);
            Assert.Throws<InvalidOperationException>(()=>flow.TryCommitChapterCompletion(demo,fakeClosing,save));Assert.That(flow.CurrentChapter1CompletionReceipt,Is.Null);
            Assert.Throws<InvalidOperationException>(()=>new Chapter1BranchSaveReceipt(flow,fakeDemo,default(ProfileAtomicSaveResultV1),save.ExpectedRevision));
            yield return _trace.FinishClosing(1);var actual=flow.CurrentChapter1CompletionReceipt;
            Assert.Throws<InvalidOperationException>(()=>flow.TryCommitChapterCompletion(actual.Demo,actual.Closing,actual.Save));Assert.That(flow.CurrentChapter1CompletionReceipt,Is.SameAs(actual));
        }
        [UnityTest] public IEnumerator RestartPreservesV1PairButNotChapterReceipt()
        {
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.CompleteBranch(1);var before=Chapter1AuthoredInputTrace.ReadCurrentTestProfile();
            Assert.That(_trace.Flow.CurrentChapter1CompletionReceipt,Is.Not.Null);yield return _trace.CloseForReopen();_trace=null;
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();yield return _trace.WaitReady();
            var after=new Chapter1DevelopmentProfile(_trace.Flow.CurrentHub).Snapshot;
            Assert.That(after.Progression.CommittedChoice,Is.EqualTo(before.Progression.CommittedChoice));Assert.That(after.Progression.GrantedSkill,Is.EqualTo(before.Progression.GrantedSkill));
            Assert.That(after.Progression.ConsentState,Is.EqualTo(before.Progression.ConsentState));Assert.That(after.Progression.CompletedBranches,Is.EqualTo(before.Progression.CompletedBranches));
            Assert.That(_trace.Flow.CurrentChapter1CompletionReceipt,Is.Null);Assert.That(_trace.Flow.RunStartedRevision,Is.Zero);Assert.That(_trace.Flow.View.RunKey,Is.Null);
            yield return _trace.PlayToChoice();yield return _trace.WaitPhase(Chapter1RunPhase.AtBarrier);Assert.That(_trace.Flow.CurrentChapter1CompletionReceipt,Is.Null);
            Assert.That(Chapter1AuthoredInputTrace.ReadCurrentTestProfile().Progression.CommittedChoice,Is.EqualTo(ProfileChoice.Extraction));
        }
    }
}
