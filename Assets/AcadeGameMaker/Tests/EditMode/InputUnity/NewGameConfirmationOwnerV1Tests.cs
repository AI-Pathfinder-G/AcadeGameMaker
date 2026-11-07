using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace AcadeGameMaker.Tests.EditMode.InputUnity
{
    // AC-M5D7QC3-001..009: positive authority comes from real launch, actual
    // Q-A controller activation, Q-B LateUpdate and its registered reference.
    internal sealed class NewGameConfirmationOwnerV1Tests
    {
        [TestCase("OwnerBeforeTake")] [TestCase("PresenterBeforeTake")] [TestCase("RouterBeforeTake")]
        [TestCase("OwnerAfterTake")] [TestCase("RouterAfterTake")] [TestCase("RootAdapterAfterTake")]
        public void AC001_TwoActualLaunchCohortsRejectForeignContextWithoutConsumption(string row)
        {
            using (var a=NewGameActualCohortFixtureV1.Create(true))
            using (var b=NewGameActualCohortFixtureV1.Create(true))
            {
                var taken=row.EndsWith("AfterTake",StringComparison.Ordinal); object handle=taken?a.Take():null;
                var snapshot=a.IntakeSnapshot(); var other=b.IntakeSnapshot();
                if (!taken)
                {
                    var args=new[] {row=="OwnerBeforeTake"?b.Owner:a.Owner,row=="PresenterBeforeTake"?b.Presenter:a.Presenter,row=="RouterBeforeTake"?b.Router:a.Router,null};
                    Assert.That(NewGameActualCohortFixtureV1.Call(a.HandoffOwner,"TryTakeNewGameForConfirmation",args),Is.False);
                }
                else Assert.That(() => NewGameActualCohortFixtureV1.Call(row=="OwnerAfterTake"?b.Owner:a.Owner,"AcceptNewGame",handle,row=="RootAdapterAfterTake"?b.Adapter:a.Adapter,row=="RouterAfterTake"?b.Router:a.Router),Throws.InvalidOperationException);
                Assert.That(a.IntakeSnapshot(),Is.EqualTo(snapshot)); Assert.That(b.IntakeSnapshot(),Is.EqualTo(other));
                if (!taken) handle=a.Take();
                Assert.That(NewGameActualCohortFixtureV1.Read(a.Accept(handle),"Outcome").ToString(),Is.EqualTo("DecisionRequired"));
                Assert.That(NewGameActualCohortFixtureV1.Read(b.Accept(b.Take()),"Outcome").ToString(),Is.EqualTo("DecisionRequired"));
            }
        }
        [TestCase("TakenProofCorruption")] [TestCase("EpochProofCorruption")] [TestCase("StateProofCorruption")] [TestCase("RootWitnessCorruption")]
        public void AC001_ApprovedSingleEvidenceCorruptionClosesActualIntakeAndCannotRevive(string row)
        {
            using (var a=NewGameActualCohortFixtureV1.Create(true))
            using (var b=NewGameActualCohortFixtureV1.Create(true))
            {
                var handle=a.Take(); var taken=NewGameActualCohortFixtureV1.Field(a.HandoffOwner,"_takenRequest"); var history=NewGameActualCohortFixtureV1.Field(a.HandoffOwner,"_epochs");
                var target=row=="TakenProofCorruption"?a.HandoffOwner:row=="RootWitnessCorruption"?a.Adapter:a.Owner;
                var name=row=="TakenProofCorruption"?"_takenRequestProof":row=="EpochProofCorruption"?"_epochProof":row=="StateProofCorruption"?"_stateProof":"_launchRootWitnessB";
                object bad=row=="TakenProofCorruption"?null:row=="EpochProofCorruption"?(object)2L:row=="StateProofCorruption"?Enum.Parse(NewGameActualCohortFixtureV1.HubType("NewGameConfirmationOwnerStateV1"),"Inspecting"):b.Root;
                var original=NewGameActualCohortFixtureV1.NegativeEvidenceField(target,name).GetValue(target);
                NewGameActualCohortFixtureV1.NegativeEvidenceField(target,name).SetValue(target,bad);
                try
                {
                    Assert.That(() => a.Accept(handle),Throws.InvalidOperationException);
                    Assert.That(NewGameActualCohortFixtureV1.Field(a.Owner,"_state").ToString(),Is.EqualTo("TerminalFailure"));
                    Assert.That(NewGameActualCohortFixtureV1.Read(a.HandoffOwner,"State").ToString(),Is.EqualTo("Closed"));
                    Assert.That(NewGameActualCohortFixtureV1.Field(a.Presenter,"_confirmationInteractionClosed"),Is.True);
                    Assert.That(NewGameActualCohortFixtureV1.Field(a.Owner,"_display"),Is.Null); Assert.That(NewGameActualCohortFixtureV1.Field(a.Owner,"_confirmed"),Is.Null);
                    Assert.That(NewGameActualCohortFixtureV1.Field(a.HandoffOwner,"_takenRequest"),Is.EqualTo(taken)); Assert.That(NewGameActualCohortFixtureV1.Field(a.HandoffOwner,"_epochs"),Is.SameAs(history));
                }
                finally { NewGameActualCohortFixtureV1.NegativeEvidenceField(target,name).SetValue(target,original); }
                Assert.That(() => a.Accept(handle),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Field(a.Owner,"_display"),Is.Null);
                Assert.That(NewGameActualCohortFixtureV1.Read(b.Accept(b.Take()),"Outcome").ToString(),Is.EqualTo("DecisionRequired"));
            }
        }
        [Test]
        public void AC001_NormalAuthoringConnectionChangeAfterActualTakeClosesBeforeObservation()
        {
            using (var a=NewGameActualCohortFixtureV1.Create(true))
            using (var b=NewGameActualCohortFixtureV1.Create(true))
            {
                var handle=a.Take(); var p=a.Presenter;
                NewGameActualCohortFixtureV1.Call(p,"ConfigureForAuthoring",b.Router,NewGameActualCohortFixtureV1.Field(b.Presenter,"_latch"),NewGameActualCohortFixtureV1.Field(p,"_safeFrame"),NewGameActualCohortFixtureV1.Field(p,"_eventSystem"),NewGameActualCohortFixtureV1.Field(p,"_buttons"),NewGameActualCohortFixtureV1.Field(p,"_dismissRect"),NewGameActualCohortFixtureV1.Field(p,"_notice"),NewGameActualCohortFixtureV1.Field(p,"_noticeText"),NewGameActualCohortFixtureV1.Field(p,"_dismissText"));
                Assert.That(() => a.Accept(handle),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Read(a.Owner,"State").ToString(),Is.EqualTo("TerminalFailure"));
                Assert.That(NewGameActualCohortFixtureV1.Field(a.Owner,"_display"),Is.Null);
                Assert.That(NewGameActualCohortFixtureV1.Read(a.HandoffOwner,"State").ToString(),Is.EqualTo("Closed"));
            }
        }
        [Test]
        public void AC001_OldActualHandleCannotConsumeNewEpochPendingHandle()
        {
            using (var a=NewGameActualCohortFixtureV1.Create(true))
            {
                var old=a.Take(); var decision=NewGameActualCohortFixtureV1.Read(a.Accept(old),"DecisionCapability");
                var initialTaken=NewGameActualCohortFixtureV1.Field(a.HandoffOwner,"_takenRequest"); var initialIntent=NewGameActualCohortFixtureV1.Field(a.Presenter,"_retainedIntent");
                var cap=NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(a.Owner,"Cancel",decision),"RearmCapability"); NewGameActualCohortFixtureV1.Call(a.Owner,"Rearm",cap);
                a.Step(); NewGameActualCohortFixtureV1.Call(a.Presenter,"FixedUpdate"); NewGameActualCohortFixtureV1.Call(a.Presenter,"ActivateSuccessor",HubMenuItemV1.NewGame); NewGameActualCohortFixtureV1.Call(a.HandoffOwner,"LateUpdate");
                var current=a.Take(); var snapshot=a.IntakeSnapshot(); Assert.That(() => a.Accept(old),Throws.InvalidOperationException); Assert.That(a.IntakeSnapshot(),Is.EqualTo(snapshot));
                Assert.That(NewGameActualCohortFixtureV1.Read(a.Accept(current),"Outcome").ToString(),Is.EqualTo("DecisionRequired"));
                Assert.That(NewGameActualCohortFixtureV1.Field(a.HandoffOwner,"_takenRequest"),Is.EqualTo(initialTaken)); Assert.That(NewGameActualCohortFixtureV1.Field(a.Presenter,"_retainedIntent"),Is.EqualTo(initialIntent));
            }
        }
        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(7)] [TestCase(8)] [TestCase(9)] [TestCase(10)] [TestCase(11)] [TestCase(12)] [TestCase(13)] [TestCase(14)] [TestCase(15)] [TestCase(16)] [TestCase(17)] [TestCase(18)] [TestCase(19)] [TestCase(20)] [TestCase(21)]
        public void AC002_ActualFreshCaptureAllRolesFieldsAndPriorityMatrix(int batch)
        {
            var rows=C3DiskMatrixV1.Batch(C3DiskMatrixV1.ClassificationRows(),batch,8);
            foreach (var row in rows) C3DiskMatrixV1.Log(row.Id,"planned",null,null,null,0,0,false);
            using (var f=NewGameActualCohortFixtureV1.Create(false))
            {
                var epoch=new object();
                foreach (var row in rows)
                {
                    C3DiskMatrixV1.Apply(f.Root,row.Before); var before=C3DiskMatrixV1.Fingerprint(f.Root);
                    try
                    {
                        ProfileNewGameCaptureResultV1 capture;
                        using (C3DiskMatrixV1.Lock(f.Root,row.LockRole)) capture=ProfileNewGameConfirmationV1.Capture((DesktopProfileLaunchAdapterV1)f.Adapter,(InputRouter)f.Router,f.Owner,epoch);
                        var actual=capture.Outcome == ProfileNewGameCaptureOutcomeV1.Captured ? capture.Classification.ToString() : capture.Outcome.ToString();
                        Assert.That(actual,Is.EqualTo(row.Expected),row.Id);
                        var after=C3DiskMatrixV1.Fingerprint(f.Root); Assert.That(after,Is.EqualTo(before),row.Id);
                        C3DiskMatrixV1.Log(row.Id,"passed",before,after,actual,0,0,false);
                    }
                    catch { C3DiskMatrixV1.Log(row.Id,"failed",before,C3DiskMatrixV1.Fingerprint(f.Root),null,0,0,false); throw; }
                }
            }
        }
        [TestCase(0,0)] [TestCase(0,1)] [TestCase(0,2)] [TestCase(0,3)] [TestCase(0,4)] [TestCase(0,5)] [TestCase(0,6)] [TestCase(0,7)] [TestCase(0,8)] [TestCase(0,9)] [TestCase(0,10)] [TestCase(0,11)] [TestCase(0,12)] [TestCase(0,13)] [TestCase(0,14)] [TestCase(0,15)] [TestCase(0,16)] [TestCase(0,17)] [TestCase(0,18)] [TestCase(0,19)] [TestCase(0,20)] [TestCase(0,21)] [TestCase(0,22)]
        [TestCase(1,0)] [TestCase(1,1)] [TestCase(1,2)] [TestCase(1,3)] [TestCase(1,4)] [TestCase(1,5)] [TestCase(1,6)] [TestCase(1,7)] [TestCase(1,8)] [TestCase(1,9)] [TestCase(1,10)] [TestCase(1,11)] [TestCase(1,12)] [TestCase(1,13)] [TestCase(1,14)] [TestCase(1,15)] [TestCase(1,16)] [TestCase(1,17)] [TestCase(1,18)] [TestCase(1,19)] [TestCase(1,20)] [TestCase(1,21)] [TestCase(1,22)]
        [TestCase(2,0)] [TestCase(2,1)] [TestCase(2,2)] [TestCase(2,3)] [TestCase(2,4)] [TestCase(2,5)] [TestCase(2,6)] [TestCase(2,7)] [TestCase(2,8)] [TestCase(2,9)] [TestCase(2,10)] [TestCase(2,11)] [TestCase(2,12)] [TestCase(2,13)] [TestCase(2,14)] [TestCase(2,15)] [TestCase(2,16)] [TestCase(2,17)] [TestCase(2,18)] [TestCase(2,19)] [TestCase(2,20)] [TestCase(2,21)] [TestCase(2,22)]
        public void AC004_ActualFileTransitionMatrixClosesOldDecisionAndBindsFreshGeneration(int role,int batch)
        {
            var rows=C3DiskMatrixV1.Batch(C3DiskMatrixV1.TransitionRows(role),batch,3);
            foreach (var row in rows) C3DiskMatrixV1.Log(row.Id,"planned",null,null,null,0,0,false);
            foreach (var row in rows)
            using (var f=NewGameActualCohortFixtureV1.Create(false))
            {
                C3DiskMatrixV1.Apply(f.Root,row.Before); var displayHashes=C3DiskMatrixV1.Fingerprint(f.Root);
                var first=f.Accept(f.Take()); var old=NewGameActualCohortFixtureV1.Read(first,"DecisionCapability");
                var display=NewGameActualCohortFixtureV1.Field(f.Owner,"_display");
                var generation=(long)NewGameActualCohortFixtureV1.Field(f.Owner,"_generation");
                C3DiskMatrixV1.Apply(f.Root,row.After); var before=C3DiskMatrixV1.Fingerprint(f.Root);
                try
                {
                    object result;
                    object next=null,confirmedSource=null;
                    using (C3DiskMatrixV1.Lock(f.Root,row.LockRole)) result=NewGameActualCohortFixtureV1.Call(f.Owner,"Confirm",old);
                    var outcome=NewGameActualCohortFixtureV1.Read(result,"Outcome").ToString();
                    Assert.That(outcome,Is.EqualTo(row.Expected),row.Id);
                    Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"Confirm",old),Throws.InvalidOperationException,row.Id);
                    Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",old),Throws.InvalidOperationException,row.Id);
                    if (outcome == "FreshDecisionRequired")
                    {
                        Assert.That(() => NewGameActualCohortFixtureV1.Read(result,"ConfirmedRequest"),Throws.InvalidOperationException);
                        var fresh=NewGameActualCohortFixtureV1.Call(f.Owner,"OpenFreshDecision");
                        next=NewGameActualCohortFixtureV1.Read(fresh,"DecisionCapability");
                        Assert.That(next,Is.Not.SameAs(old)); Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_generation"),Is.EqualTo(generation+1));
                        Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_display"),Is.Not.SameAs(display));
                        Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"OpenFreshDecision"),Throws.InvalidOperationException);
                        Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"Confirm",old),Throws.InvalidOperationException);
                        Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",old),Throws.InvalidOperationException);
                        Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_decision"),Is.SameAs(next));
                        Assert.That(NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(f.Owner,"Confirm",next),"Outcome").ToString(),Is.EqualTo("Confirmed"));
                    }
                    else if (outcome == "Confirmed")
                    {
                        Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_generation"),Is.EqualTo(generation));
                        var request=(ConfirmedProfileResetRequestV1)NewGameActualCohortFixtureV1.Read(result,"ConfirmedRequest");
                        Assert.That(request,Is.SameAs(NewGameActualCohortFixtureV1.Field(f.Owner,"_confirmed")));
                        var source=C3DiskMatrixV1.ReadActualConfirmedSource(request);
                        confirmedSource=source;
                        Assert.That(source,Is.Not.SameAs(display));
                        var token=NewGameActualCohortFixtureV1.Field(f.Owner,"_epochToken");
                        var fresh=ProfileNewGameConfirmationV1.Capture((DesktopProfileLaunchAdapterV1)f.Adapter,(InputRouter)f.Router,f.Owner,token);
                        Assert.That(ProfileNewGameConfirmationV1.MatchesRecapturedIdentity((DesktopProfileLaunchAdapterV1)f.Adapter,(InputRouter)f.Router,f.Owner,token,source,fresh),Is.True);
                        Assert.That(ProfileNewGameConfirmationV1.MatchesRecapturedIdentity((DesktopProfileLaunchAdapterV1)f.Adapter,(InputRouter)f.Router,f.Owner,token,(ProfileNewGameCaptureResultV1)display,source),Is.EqualTo(displayHashes==before));
                    }
                    else { Assert.That(NewGameActualCohortFixtureV1.Read(f.Owner,"State").ToString(),Is.EqualTo("TerminalFailure")); Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_decision"),Is.Null); }
                    var after=C3DiskMatrixV1.Fingerprint(f.Root); Assert.That(after,Is.EqualTo(before),row.Id);
                    C3DiskMatrixV1.Log(row.Id,"passed",displayHashes+" -> "+before,after,outcome,generation,(long)NewGameActualCohortFixtureV1.Field(f.Owner,"_generation"),true,"oldDecision="+C3DiskMatrixV1.ReferenceId(old)+";newDecision="+C3DiskMatrixV1.ReferenceId(next)+";display="+C3DiskMatrixV1.ReferenceId(display)+";currentDisplay="+C3DiskMatrixV1.ReferenceId(NewGameActualCohortFixtureV1.Field(f.Owner,"_display"))+";confirmedSource="+C3DiskMatrixV1.ReferenceId(confirmedSource));
                }
                catch { C3DiskMatrixV1.Log(row.Id,"failed",displayHashes+" -> "+before,C3DiskMatrixV1.Fingerprint(f.Root),null,generation,(long)NewGameActualCohortFixtureV1.Field(f.Owner,"_generation"),false); throw; }
            }
        }
        [TestCase("Confirm")] [TestCase("Cancel")]
        public void AC003_ActualDecisionRejectsNullUnregisteredCloneForeignAndOldGenerationWithoutConsumption(string operation)
        {
            using (var f=NewGameActualCohortFixtureV1.Create(true))
            using (var foreign=NewGameActualCohortFixtureV1.Create(true))
            {
                var actual=NewGameActualCohortFixtureV1.Read(f.Accept(f.Take()),"DecisionCapability");
                var other=NewGameActualCohortFixtureV1.Read(foreign.Accept(foreign.Take()),"DecisionCapability");
                var clone=typeof(object).GetMethod("MemberwiseClone",BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.DeclaredOnly, null,Type.EmptyTypes,null).Invoke(actual,null);
                foreach (var candidate in new object[] { null,Activator.CreateInstance(actual.GetType(),true),clone,other })
                {
                    Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,operation,candidate),Throws.InvalidOperationException);
                    Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_decision"),Is.SameAs(actual));
                    Assert.That(NewGameActualCohortFixtureV1.Read(f.Owner,"State").ToString(),Is.EqualTo("AwaitingDecision"));
                }
                File.WriteAllBytes(Path.Combine(f.Root,"profile.tmp.json"),Encoding.UTF8.GetBytes("{"));
                Assert.That(NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(f.Owner,"Confirm",actual),"Outcome").ToString(),Is.EqualTo("FreshDecisionRequired"));
                var next=NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(f.Owner,"OpenFreshDecision"),"DecisionCapability");
                var generation=NewGameActualCohortFixtureV1.Field(f.Owner,"_generation");
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,operation,actual),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_generation"),Is.EqualTo(generation));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_decision"),Is.SameAs(next));
                Assert.That(NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(f.Owner,operation,next),"Outcome").ToString(),Is.EqualTo(operation == "Confirm" ? "Confirmed" : "Cancelled"));
                Assert.That(NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(foreign.Owner,operation,other),"Outcome").ToString(),Is.EqualTo(operation == "Confirm" ? "Confirmed" : "Cancelled"));
            }
        }
        [Test]
        public void AC001_ActualQBTakeOpaqueHandleIsConsumedOnce()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(false))
            {
                var handle=f.Take(); var result=f.Accept(handle);
                Assert.That(NewGameActualCohortFixtureV1.Read(result,"Outcome").ToString(),Is.EqualTo("Confirmed"));
                Assert.That(() => f.Accept(handle),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Read(f.HandoffOwner,"State").ToString(),Is.EqualTo("RequestTaken"));
            }
        }
        [Test]
        public void AC001_UnregisteredCandidateCannotConsumePendingActualHandle()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(false))
            {
                var handle=f.Take();
                var candidate=Activator.CreateInstance(handle.GetType(),true);
                Assert.That(() => f.Accept(candidate),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Accept(handle),"Outcome").ToString(),Is.EqualTo("Confirmed"));
            }
        }
        [Test]
        public void AC001_CopiedEqualRequestCannotConsumePendingActualHandle()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(false))
            {
                var handle=f.Take();
                var actual=NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_takenRequest");
                var copy=Activator.CreateInstance(actual.GetType(),BindingFlags.Instance|BindingFlags.NonPublic,null,new[] { NewGameActualCohortFixtureV1.Read(actual,"Item"),NewGameActualCohortFixtureV1.Read(actual,"Receipt") },null);
                var intake=NewGameActualCohortFixtureV1.Method(f.Owner,"AcceptNewGame");
                Assert.That(() => intake.Invoke(f.Owner,new[] { copy,f.Adapter,f.Router }),Throws.TypeOf<ArgumentException>());
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Accept(handle),"Outcome").ToString(),Is.EqualTo("Confirmed"));
            }
        }
        [TestCase("Settings")] [TestCase("Quit")] [TestCase("Continue")]
        public void AC001_NonNewGameCleanRejectionPreservesActualLiveRequest(string item)
        {
            using (var f=NewGameActualCohortFixtureV1.Create(false,item))
            {
                var live=NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_request");
                var args=new object[] { f.Owner,f.Presenter,f.Router,null };
                Assert.That(NewGameActualCohortFixtureV1.Call(f.HandoffOwner,"TryTakeNewGameForConfirmation",args),Is.False);
                Assert.That(NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_request"),Is.EqualTo(live));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_takenRequest"),Is.Null);
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_display"),Is.Null);
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Owner,"State").ToString(),Is.EqualTo("AwaitingRequest"));
            }
        }
        [Test]
        public void AC001_ForeignCohortRejectionPreservesPendingActualHandle()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(false))
            using (var foreign=NewGameActualCohortFixtureV1.Create(false))
            {
                var args=new object[] { foreign.Owner,f.Presenter,f.Router,null };
                Assert.That(NewGameActualCohortFixtureV1.Call(f.HandoffOwner,"TryTakeNewGameForConfirmation",args),Is.False);
                var handle=f.Take();
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"AcceptNewGame",handle,foreign.Adapter,f.Router),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Accept(handle),"Outcome").ToString(),Is.EqualTo("Confirmed"));
            }
        }
        [Test]
        public void AC002_InitialBusyRetryRetainsOneIssuanceWithoutRetake()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(false))
            {
                var handle=f.Take(); object busy;
                using (f.HoldRootLock()) busy=f.Accept(handle);
                Assert.That(NewGameActualCohortFixtureV1.Read(busy,"Outcome").ToString(),Is.EqualTo("Busy"));
                var retry=NewGameActualCohortFixtureV1.Read(busy,"RetryCapability");
                Assert.That(() => f.Accept(handle),Throws.InvalidOperationException);
                var result=NewGameActualCohortFixtureV1.Call(f.Owner,"RetryIntake",retry);
                Assert.That(NewGameActualCohortFixtureV1.Read(result,"Outcome").ToString(),Is.EqualTo("Confirmed"));
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"RetryIntake",retry),Throws.InvalidOperationException);
            }
        }
        [Test]
        public void AC003_ExactKoreanCopyAndOneShotCancelPreserveBytesAndMaps()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(true))
            {
                var result=f.Accept(f.Take()); var decision=NewGameActualCohortFixtureV1.Read(result,"DecisionCapability");
                Assert.That(NewGameActualCohortFixtureV1.Read(result,"Prompt"),Is.EqualTo("새 게임을 시작하면 현재 단일 프로필의 진행 상황, 설정, 입력 설정, 튜토리얼 확인 상태가 초기화됩니다. 기존 프로필 파일은 자동 복구에 사용되지 않는 수동 복구용 자료로 계속 보관됩니다. 의상 및 외형 자료는 별도 저장 영역이므로 변경되지 않습니다. 계속하시겠습니까?"));
                Assert.That(NewGameActualCohortFixtureV1.Read(result,"ConfirmLabel"),Is.EqualTo("초기화하고 새 게임"));
                Assert.That(NewGameActualCohortFixtureV1.Read(result,"CancelLabel"),Is.EqualTo("취소"));
                var before=File.ReadAllBytes(Path.Combine(f.Root,"profile.json")); var previous=File.ReadAllBytes(Path.Combine(f.Root,"profile.prev.json"));
                var receipt=((InputRouter)f.Router).CurrentReceipt;
                var cancelled=NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",decision);
                Assert.That(NewGameActualCohortFixtureV1.Read(cancelled,"Outcome").ToString(),Is.EqualTo("Cancelled"));
                Assert.That(File.ReadAllBytes(Path.Combine(f.Root,"profile.json")),Is.EqualTo(before));
                Assert.That(File.ReadAllBytes(Path.Combine(f.Root,"profile.prev.json")),Is.EqualTo(previous));
                Assert.That(((InputRouter)f.Router).CurrentReceipt,Is.EqualTo(receipt));
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"Confirm",decision),Throws.InvalidOperationException);
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",decision),Throws.InvalidOperationException);
            }
        }
        [Test]
        public void AC003_ConfirmCancelRaceHasExactlyOneWinner()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(true))
            using (var acquired=new ManualResetEventSlim(false))
            {
                var decision=NewGameActualCohortFixtureV1.Read(f.Accept(f.Take()),"DecisionCapability");
                Exception workerError=null; Exception loserError=null;
                var worker=new Thread(() =>
                {
                    try
                    {
                        using (f.HoldRootLock())
                        {
                            acquired.Set();
                            if (!SpinWait.SpinUntil(() => (int)NewGameActualCohortFixtureV1.Field(f.Owner,"_operation") == 1,TimeSpan.FromSeconds(10))) throw new TimeoutException("Confirm did not reserve inspection.");
                            try { NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",decision); } catch (Exception e) { loserError=e; }
                        }
                    }
                    catch (Exception e) { workerError=e; acquired.Set(); }
                }) { IsBackground=true };
                worker.Start();
                object confirmed=null;
                try { Assert.That(acquired.Wait(TimeSpan.FromSeconds(10)),Is.True); confirmed=NewGameActualCohortFixtureV1.Call(f.Owner,"Confirm",decision); }
                finally { Assert.That(worker.Join(TimeSpan.FromSeconds(15)),Is.True,"race worker collected"); }
                Assert.That(workerError,Is.Null); Assert.That(loserError,Is.TypeOf<InvalidOperationException>());
                Assert.That(NewGameActualCohortFixtureV1.Read(confirmed,"Outcome").ToString(),Is.EqualTo("Confirmed"));
            }
        }
        [TestCase("equal")] [TestCase("default")] [TestCase("ambiguous")]
        public void AC004_ConfirmRecaptureEqualChangedDefaultAndChangedPromptRows(string change)
        {
            using (var f=NewGameActualCohortFixtureV1.Create(true))
            {
                var first=f.Accept(f.Take()); var decision=NewGameActualCohortFixtureV1.Read(first,"DecisionCapability");
                if (change == "default") File.Delete(Path.Combine(f.Root,"profile.prev.json"));
                if (change == "ambiguous") File.WriteAllBytes(Path.Combine(f.Root,"profile.tmp.json"),new byte[] { 0xFF });
                var result=NewGameActualCohortFixtureV1.Call(f.Owner,"Confirm",decision);
                Assert.That(NewGameActualCohortFixtureV1.Read(result,"Outcome").ToString(),Is.EqualTo(change == "ambiguous" ? "FreshDecisionRequired" : "Confirmed"));
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",decision),Throws.InvalidOperationException);
                if (change == "ambiguous")
                {
                    var fresh=NewGameActualCohortFixtureV1.Call(f.Owner,"OpenFreshDecision");
                    Assert.That(NewGameActualCohortFixtureV1.Read(fresh,"DecisionCapability"),Is.Not.SameAs(decision));
                    Assert.That(NewGameActualCohortFixtureV1.Read(fresh,"Outcome").ToString(),Is.EqualTo("DecisionRequired"));
                }
            }
        }
        [Test]
        public void AC007_ConfirmBusyPreservesSameLiveDecision()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(true))
            {
                var decision=NewGameActualCohortFixtureV1.Read(f.Accept(f.Take()),"DecisionCapability"); object busy;
                using (f.HoldRootLock()) busy=NewGameActualCohortFixtureV1.Call(f.Owner,"Confirm",decision);
                Assert.That(NewGameActualCohortFixtureV1.Read(busy,"Outcome").ToString(),Is.EqualTo("Busy"));
                Assert.That(NewGameActualCohortFixtureV1.Read(busy,"DecisionCapability"),Is.SameAs(decision));
                Assert.That(() => NewGameActualCohortFixtureV1.Read(busy,"RetryCapability"),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(f.Owner,"Confirm",decision),"Outcome").ToString(),Is.EqualTo("Confirmed"));
            }
        }
        [Test]
        public void AC008_CommitClosesAllAuthorityBeforeReturningOpaqueRequest()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(false))
            {
                var request=NewGameActualCohortFixtureV1.Read(f.Accept(f.Take()),"ConfirmedRequest");
                Assert.That(NewGameActualCohortFixtureV1.Call(f.Owner,"CommitForExecution",request),Is.SameAs(request));
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Owner,"State").ToString(),Is.EqualTo("ExecutionCommitted"));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Presenter,"_confirmationInteractionClosed"),Is.True);
                Assert.That(NewGameActualCohortFixtureV1.Read(f.HandoffOwner,"State").ToString(),Is.EqualTo("Closed"));
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"CommitForExecution",request),Throws.InvalidOperationException);
                Assert.That(File.Exists(Path.Combine(f.Root,"profile.reset.barrier.json")),Is.False);
            }
        }
        [TestCase("AfterLowerReserve")] [TestCase("AfterUpperClosure")]
        public void AC008_CommitFaultNeverRestoresConfirmedRequest(string fault)
        {
            using (var f=NewGameActualCohortFixtureV1.Create(false))
            {
                var request=NewGameActualCohortFixtureV1.Read(f.Accept(f.Take()),"ConfirmedRequest");
                NewGameActualCohortFixtureV1.Call(f.Owner,"SetFaultForTests",fault);
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"CommitForExecution",request),Throws.InvalidOperationException);
                NewGameActualCohortFixtureV1.Call(f.Owner,"SetFaultForTests",new object[] { null });
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"CommitForExecution",request),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Owner,"State").ToString(),Is.EqualTo("TerminalFailure"));
            }
        }
        [Test]
        public void AC006_PartialSuccessorPrepareFailureClosesAllParticipants()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(true))
            {
                var decision=NewGameActualCohortFixtureV1.Read(f.Accept(f.Take()),"DecisionCapability");
                var rearm=NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",decision),"RearmCapability");
                NewGameActualCohortFixtureV1.Call(f.Owner,"SetFaultForTests","AfterSuccessorPrepare");
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"Rearm",rearm),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Owner,"State").ToString(),Is.EqualTo("TerminalFailure"));
                Assert.That(NewGameActualCohortFixtureV1.Read(f.HandoffOwner,"State").ToString(),Is.EqualTo("Closed"));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Presenter,"_confirmationInteractionClosed"),Is.True);
            }
        }
        [Test]
        public void AC005_RepeatedCancelAppendsEpochsWithoutRewritingInitialHistory()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(true))
            {
                var oldIntent=NewGameActualCohortFixtureV1.Field(f.Presenter,"_retainedIntent");
                object oldTaken=null; object previousHistory=null;
                for (var epoch=1;epoch<=3;epoch++)
                {
                    var decision=NewGameActualCohortFixtureV1.Read(f.Accept(f.Take()),"DecisionCapability");
                    if (oldTaken == null) oldTaken=NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_takenRequest");
                    var cancelled=NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",decision);
                    var rearm=NewGameActualCohortFixtureV1.Read(cancelled,"RearmCapability");
                    NewGameActualCohortFixtureV1.Call(f.Owner,"Rearm",rearm);
                    Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"Rearm",rearm),Throws.InvalidOperationException);
                    var history=NewGameActualCohortFixtureV1.Field(f.Owner,"_history");
                    Assert.That(NewGameActualCohortFixtureV1.Field(history,"Previous"),Is.SameAs(previousHistory)); previousHistory=history;
                    Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_epoch"),Is.EqualTo((long)epoch+1));
                    Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_pendingIssued"),Is.Null);
                    Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_acceptedIssued"),Is.Null);
                    Assert.That(NewGameActualCohortFixtureV1.Field(f.Presenter,"_retainedIntent"),Is.EqualTo(oldIntent));
                    Assert.That(NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_takenRequest"),Is.EqualTo(oldTaken));
                    f.Step(); NewGameActualCohortFixtureV1.Call(f.Presenter,"FixedUpdate");
                    NewGameActualCohortFixtureV1.Call(f.Presenter,"ActivateSuccessor",HubMenuItemV1.NewGame);
                    NewGameActualCohortFixtureV1.Call(f.HandoffOwner,"LateUpdate");
                }
            }
        }
        [Test]
        public void AC006_SkippedActualSuccessorFrameClosesAllOwners()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(true))
            {
                var decision=NewGameActualCohortFixtureV1.Read(f.Accept(f.Take()),"DecisionCapability");
                var rearm=NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",decision),"RearmCapability");
                NewGameActualCohortFixtureV1.Call(f.Owner,"Rearm",rearm);
                f.Step(); f.Step(); NewGameActualCohortFixtureV1.Call(f.Presenter,"FixedUpdate");
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Presenter,"_confirmationInteractionClosed"),Is.True);
                Assert.That(NewGameActualCohortFixtureV1.Read(f.HandoffOwner,"State").ToString(),Is.EqualTo("Closed"));
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Owner,"State").ToString(),Is.EqualTo("TerminalFailure"));
            }
        }
        [TestCase("AwaitingBaseline")]
        [TestCase("Ready")]
        [TestCase("Transferred")]
        [TestCase("AwaitingDecision")]
        public void AC006_LateActualSuccessorReservationCannotReplaceCurrentCursor(string phase)
        {
            using (var f=NewGameActualCohortFixtureV1.Create(true))
            {
                var first=NewGameActualCohortFixtureV1.Read(f.Accept(f.Take()),"DecisionCapability");
                var rearm=NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",first),"RearmCapability");
                NewGameActualCohortFixtureV1.Call(f.Owner,"Rearm",rearm);
                var slot=NewGameActualCohortFixtureV1.Field(f.Presenter,"_successor");
                var cursor=NewGameActualCohortFixtureV1.Field(slot,"Cursor");
                var reservation=NewGameActualCohortFixtureV1.Field(slot,"Reservation");
                var token=NewGameActualCohortFixtureV1.Field(slot,"Token");
                var epoch=NewGameActualCohortFixtureV1.Field(slot,"Epoch");
                if (phase != "AwaitingBaseline") { f.Step(); NewGameActualCohortFixtureV1.Call(f.Presenter,"FixedUpdate"); }
                object handle=null, decision=null;
                if (phase == "Transferred" || phase == "AwaitingDecision")
                {
                    NewGameActualCohortFixtureV1.Call(f.Presenter,"ActivateSuccessor",HubMenuItemV1.NewGame);
                    NewGameActualCohortFixtureV1.Call(f.HandoffOwner,"LateUpdate"); handle=f.Take();
                    if (phase == "AwaitingDecision") decision=NewGameActualCohortFixtureV1.Read(f.Accept(handle),"DecisionCapability");
                }
                var currentRequestSlot=NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_successor");
                var pending=NewGameActualCohortFixtureV1.Field(f.Owner,"_pendingIssued");
                var pendingHandoff=NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_pendingIssued");
                var ownerState=NewGameActualCohortFixtureV1.Read(f.Owner,"State");
                Assert.That(NewGameActualCohortFixtureV1.Call(f.HandoffOwner,"MatchesSuccessorReservation",reservation,f.Owner,f.Presenter,f.Router,token,epoch),Is.True,"history stays queryable");
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Presenter,"PrepareNewGameSuccessor",f.Owner,reservation,token,epoch),Throws.InvalidOperationException);
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.HandoffOwner,"CommitNewGameSuccessor",reservation,f.Owner,f.Presenter,f.Router,token,epoch),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Presenter,"_successor"),Is.SameAs(slot));
                Assert.That(NewGameActualCohortFixtureV1.Field(slot,"Cursor"),Is.SameAs(cursor));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_successor"),Is.SameAs(currentRequestSlot));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_pendingIssued"),Is.SameAs(pending));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_pendingIssued"),Is.SameAs(pendingHandoff));
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Owner,"State"),Is.EqualTo(ownerState));
                if (phase == "AwaitingBaseline") { f.Step(); NewGameActualCohortFixtureV1.Call(f.Presenter,"FixedUpdate"); }
                if (handle == null) { NewGameActualCohortFixtureV1.Call(f.Presenter,"ActivateSuccessor",HubMenuItemV1.NewGame); NewGameActualCohortFixtureV1.Call(f.HandoffOwner,"LateUpdate"); handle=f.Take(); }
                if (decision == null) decision=NewGameActualCohortFixtureV1.Read(f.Accept(handle),"DecisionCapability");
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_decision"),Is.SameAs(decision));
                var next=NewGameActualCohortFixtureV1.Read(NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",decision),"RearmCapability");
                NewGameActualCohortFixtureV1.Call(f.Owner,"Rearm",next);
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_epoch"),Is.EqualTo(3L));
            }
        }
        [Test]
        public void AC008_TeardownClosesPendingDecisionWithoutCancelOrRearm()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(true))
            {
                var decision=NewGameActualCohortFixtureV1.Read(f.Accept(f.Take()),"DecisionCapability");
                NewGameActualCohortFixtureV1.Call(f.Owner,"OnDisable");
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Owner,"State").ToString(),Is.EqualTo("Closed"));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_rearm"),Is.Null);
                Assert.That(() => NewGameActualCohortFixtureV1.Call(f.Owner,"Cancel",decision),Throws.InvalidOperationException);
            }
        }
        [Test]
        public void AC001_FirstAwaitingRequestRequiresNoHistoryAndAfterTakeFailureRetainsHistory()
        {
            using (var f=NewGameActualCohortFixtureV1.Create(false))
            {
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_epoch"),Is.EqualTo(1L));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_history"),Is.Null);
                Assert.That(NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_epochs"),Is.Null);
                Assert.That(NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_takenRequest"),Is.Null);
                var handle=f.Take(); var taken=NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_takenRequest"); var record=NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_epochs");
                NewGameActualCohortFixtureV1.Call(f.Owner,"SetFaultForTests","BeforeCapture");
                Assert.That(() => f.Accept(handle),Throws.InvalidOperationException);
                Assert.That(NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_takenRequest"),Is.EqualTo(taken));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.HandoffOwner,"_epochs"),Is.SameAs(record));
                Assert.That(NewGameActualCohortFixtureV1.Field(f.Owner,"_display"),Is.Null);
                Assert.That(NewGameActualCohortFixtureV1.Read(f.Owner,"State").ToString(),Is.EqualTo("TerminalFailure"));
                Assert.That(() => f.Accept(handle),Throws.InvalidOperationException);
            }
        }
    }

    internal static class C3DiskMatrixV1
    {
        internal sealed class Row
        {
            internal string Id, Expected;
            internal string[] Before, After;
            internal int LockRole=-1;
            internal Row(string id,string[] before,string expected,string[] after=null,int lockRole=-1) { Id=id; Before=before; Expected=expected; After=after; LockRole=lockRole; }
        }
        private static readonly string[] Names={"profile.json","profile.prev.json","profile.tmp.json"};
        internal static readonly string[] Fields={"F01","F02","F03","F04","F05","F06","F07","F08","F09","F10","F11","F12","F13"};
        private static readonly string[] Ambiguous={"I01","I02","B","Xparse","Xhash","S","Xchoice","Xskill"};
        internal static List<Row> Batch(List<Row> rows,int batch,int size)
        {
            if (batch<0 || size<=0 || batch*size>=rows.Count) throw new ArgumentOutOfRangeException(nameof(batch));
            return rows.GetRange(batch*size,Math.Min(size,rows.Count-batch*size));
        }
        internal static List<Row> ClassificationRows()
        {
            var rows=new List<Row>();
            for (var mask=0;mask<8;mask++) rows.Add(new Row("C2-default-"+mask,new[] {(mask&1)!=0?"D":null,(mask&2)!=0?"D":null,(mask&4)!=0?"D":null},mask<2?"NoConfirmationRequired":"ConfirmationRequiredAmbiguous"));
            for (var r=0;r<3;r++)
            {
                foreach (var field in Fields) rows.Add(new Row("C2-"+r+"-"+field,Place(r,field),"ConfirmationRequiredMeaningful"));
                rows.Add(new Row("C2-"+r+"-revision",Place(r,"R"),r==0?"NoConfirmationRequired":"ConfirmationRequiredAmbiguous"));
                foreach (var a in Ambiguous)
                {
                    rows.Add(new Row("C2-"+r+"-"+a+"-alone",Place(r,a),"ConfirmationRequiredAmbiguous"));
                    var together=new[] {"D","D","D"}; together[r]=a;
                    rows.Add(new Row("C2-"+r+"-"+a+"-default",together,"ConfirmationRequiredAmbiguous"));
                    for (var s=0;s<3;s++) if (s!=r) { var pair=new[] {"D","D","D"}; pair[r]=a; pair[s]="F02"; rows.Add(new Row("C2-"+r+"-"+a+"-meaningful-"+s,pair,"ConfirmationRequiredMeaningful")); }
                }
                for (var s=0;s<3;s++) if (s!=r)
                {
                    var pair=Place(r,"F02"); pair[s]="D"; rows.Add(new Row("C2-"+r+"-meaningful-default-"+s,pair,"ConfirmationRequiredMeaningful"));
                    var revision=Place(r,"R"); revision[s]="D"; rows.Add(new Row("C2-"+r+"-revision-default-"+s,revision,"ConfirmationRequiredAmbiguous"));
                }
                rows.Add(new Row("C2-"+r+"-unreadable-default",new[] {"D","D","D"},"Unreadable",lockRole:r));
                foreach (var a in new[] {"I01","B","Xparse","S"})
                { var unreadable=new[] {"D","D","D"}; unreadable[(r+1)%3]="F02"; unreadable[(r+2)%3]=a; rows.Add(new Row("C2-"+r+"-unreadable-priority-"+a,unreadable,"Unreadable",lockRole:r)); }
            }
            return rows;
        }
        private static string[] Place(int r,string kind) { var result=new string[3]; result[r]=kind; return result; }
        internal static List<Row> TransitionRows(int r)
        {
            var rows=new List<Row>(); var sentinel=(r+1)%3;
            Action<string,string,string> add=(id,from,to) => { var before=Place(r,from); before[sentinel]="F02"; var after=(string[])before.Clone(); after[r]=to; rows.Add(new Row("C4-"+r+"-"+id,before,"FreshDecisionRequired",after)); };
            add("add-default",null,"D"); add("add-meaningful",null,"F02"); add("remove-default","D",null); add("remove-meaningful","F02",null);
            foreach (var field in Fields) add("default-to-"+field,"D",field);
            add("meaningful-byte-change","F02","F03"); add("revision-byte-change","D","R"); add("invalid-byte-change","Xparse","Xhash");
            foreach (var a in Ambiguous)
            {
                foreach (var from in new string[] {null,"D","F02"}) add("to-"+a+"-from-"+(from??"absent"),from,a);
                add(a+"-to-default",a,"D"); add(a+"-remove",a,null);
            }
            var unreadable=Place(r,"D"); unreadable[sentinel]="F02"; rows.Add(new Row("C4-"+r+"-unreadable",unreadable,"CaptureUnreadable",(string[])unreadable.Clone(),r));
            foreach (var kind in new[] {"F02","Xparse","I01","B","S"})
            { var equal=Place(r,kind); rows.Add(new Row("C4-"+r+"-equal-"+kind,equal,"Confirmed",(string[])equal.Clone())); }
            var exact=Place(r,"F02"); if (r!=0) exact[0]="D"; var clean=new string[3]; clean[0]="D";
            rows.Add(new Row("C4-"+r+"-exact-default",exact,"Confirmed",clean));
            rows.Add(new Row("C4-"+r+"-delete-all",Place(r,"F02"),"Confirmed",new string[3]));
            return rows;
        }
        internal static byte[] Bytes(string kind)
        {
            if (kind == null) return null;
            var d=ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument;
            byte[] bytes;
            var expected=ProfileCanonicalDecodeClassification.ValidCurrentInput;
            if (kind == "Xparse") { bytes=Encoding.UTF8.GetBytes("{"); expected=ProfileCanonicalDecodeClassification.Invalid; }
            else if (kind == "S") { bytes=Encoding.UTF8.GetBytes("{\"schemaVersion\":2}"); expected=ProfileCanonicalDecodeClassification.UnsupportedProfileSchema; }
            else if (kind == "Xhash") { bytes=Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(d.CanonicalFileBytes).Replace(d.PayloadSha256,new string('0',64))); expected=ProfileCanonicalDecodeClassification.Invalid; }
            else if (kind == "B" || kind == "Xchoice" || kind == "Xskill")
            {
                var payload=Encoding.UTF8.GetString(d.CanonicalPayloadBytes);
                if (kind == "B") { payload=payload.Replace("\"bindingOverridesJson\":\"\"","\"bindingOverridesJson\":\"{\""); expected=ProfileCanonicalDecodeClassification.ValidBindingRecoveryRequired; }
                else { payload=payload.Replace(kind == "Xchoice" ? "\"committedChoice\":null" : "\"grantedSkill\":null",kind == "Xchoice" ? "\"committedChoice\":\"Extraction\"" : "\"grantedSkill\":\"CompressionVerdict\""); expected=ProfileCanonicalDecodeClassification.Invalid; }
                var hash=Hash(Encoding.UTF8.GetBytes(payload)); bytes=Encoding.UTF8.GetBytes(payload.Substring(0,payload.Length-1)+",\"integrity\":{\"payloadSha256\":\""+hash+"\"}}");
            }
            else
            {
                var settings=new ProfileSettingsSnapshot(kind=="F01"?ProfileWindowMode.Windowed:ProfileWindowMode.BorderlessFullscreen,kind=="F02"?999:1000,kind=="F03"?699:700,kind=="F04"?799:800,kind=="F05",kind=="F06");
                var input=new ProfileInputSnapshot(kind=="I01"?"other-input-asset":ProfileInputContract.CurrentInputActionsAssetId,kind=="I02"?2:1,kind=="F07"?ProfileBindingOverridesJson.Parse("{}"):d.Snapshot.Input.BindingOverrides);
                var tutorial=kind=="F08"?new ProfileTutorialSnapshot(new[] {"tutorial.jump"}):d.Snapshot.Tutorial;
                var progression=new ProfileProgressionSnapshot(kind=="R"?1:0,kind=="F09"?(int?)101:null,kind=="F12"?(ProfileChoice?)ProfileChoice.Extraction:kind=="F13"?(ProfileChoice?)ProfileChoice.Solidarity:null,kind=="F10"?ProfileConsentState.OffersScopedResonance:ProfileConsentState.RefusesOwnershipTransfer,kind=="F12"?(ProfileSkill?)ProfileSkill.CompressionVerdict:kind=="F13"?(ProfileSkill?)ProfileSkill.CommonReferencePlane:null,kind=="F11"?new[] {ProfileBranch.Extraction}:new ProfileBranch[0]);
                bytes=ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(settings,input,tutorial,progression)).CanonicalFileBytes;
                if (kind=="I01" || kind=="I02") expected=ProfileCanonicalDecodeClassification.ValidInputMetadataRecoveryRequired;
            }
            Assert.That(ProfileCanonicalDecoderV1.Decode(bytes).Classification,Is.EqualTo(expected),kind); return bytes;
        }
        internal static void Apply(string root,string[] kinds) { for (var i=0;i<3;i++) { var path=Path.Combine(root,Names[i]); if (File.Exists(path)) File.Delete(path); if (kinds[i]!=null) File.WriteAllBytes(path,Bytes(kinds[i])); } }
        internal static ProfileNewGameCaptureResultV1 ReadActualConfirmedSource(ConfirmedProfileResetRequestV1 request)
        {
            var lower=typeof(ProfileNewGameConfirmationV1);
            var witness=Type.GetType("AcadeGameMaker.Input.Unity.ProfileNewGameConfirmationV1+ConfirmedRequestWitness, AcadeGameMaker.Input.Unity",true);
            Assert.That(witness.Assembly,Is.SameAs(lower.Assembly)); Assert.That(witness.FullName,Is.EqualTo(lower.FullName+"+ConfirmedRequestWitness"));
            var tableType=typeof(System.Runtime.CompilerServices.ConditionalWeakTable<,>).MakeGenericType(typeof(ConfirmedProfileResetRequestV1),witness);
            var field=lower.GetField("IssuedConfirmedRequests",BindingFlags.DeclaredOnly|BindingFlags.Static|BindingFlags.NonPublic);
            Assert.That(field,Is.Not.Null); Assert.That(field.DeclaringType,Is.EqualTo(lower)); Assert.That(field.FieldType,Is.EqualTo(tableType)); Assert.That(field.IsInitOnly,Is.True);
            var lookup=tableType.GetMethod("TryGetValue",BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.Public,null,new[] {typeof(ConfirmedProfileResetRequestV1),witness.MakeByRefType()},null);
            Assert.That(lookup,Is.Not.Null); Assert.That(lookup.ReturnType,Is.EqualTo(typeof(bool))); Assert.That(lookup.GetParameters()[1].IsOut,Is.True);
            var args=new object[] {request,null}; Assert.That(lookup.Invoke(field.GetValue(null),args),Is.True);
            var source=witness.GetField("SourceResult",BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.NonPublic);
            Assert.That(source,Is.Not.Null); Assert.That(source.DeclaringType,Is.EqualTo(witness)); Assert.That(source.FieldType,Is.EqualTo(typeof(ProfileNewGameCaptureResultV1))); Assert.That(source.IsInitOnly,Is.True);
            return (ProfileNewGameCaptureResultV1)source.GetValue(args[1]);
        }
        internal static IDisposable Lock(string root,int role) { return role<0?null:new FileStream(Path.Combine(root,Names[role]),FileMode.Open,FileAccess.Read,FileShare.None); }
        internal static string Hash(byte[] bytes) { using (var sha=SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant(); }
        internal static string Fingerprint(string root) { var text=new StringBuilder(); for (var i=0;i<3;i++) { var path=Path.Combine(root,Names[i]); text.Append(Names[i]).Append('='); if (File.Exists(path)) { var bytes=File.ReadAllBytes(path); text.Append(bytes.Length).Append(':').Append(Hash(bytes)).Append(':').Append(ProfileCanonicalDecoderV1.Decode(bytes).Classification); } else text.Append("absent"); text.Append(';'); } return text.ToString(); }
        internal static string ReferenceId(object value) { return value==null?"null":System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value).ToString(System.Globalization.CultureInfo.InvariantCulture); }
        [Serializable] private sealed class Evidence { public string id,status,before,after,outcome,authorityReferences; public long generationBefore,generationAfter; public bool authorityChecked; }
        internal static void Log(string id,string status,string before,string after,string outcome,long generationBefore,long generationAfter,bool authorityChecked,string authority=null)
        { TestContext.Out.WriteLine(JsonUtility.ToJson(new Evidence {id=id,status=status,before=before,after=after,outcome=outcome,generationBefore=generationBefore,generationAfter=generationAfter,authorityChecked=authorityChecked,authorityReferences=authority})); }
    }

    // Shared test-only actual-cohort fixture. PlayMode tests use reflection to
    // call these normal operations without a new friend or runtime factory.
    public sealed class NewGameActualCohortFixtureV1 : IDisposable
    {
        public string Root { get; private set; }
        public object Owner { get; private set; }
        public object Presenter { get; private set; }
        public object HandoffOwner { get; private set; }
        public object Adapter { get; private set; }
        public object Router { get; private set; }
        private GameObject _host;
        public static NewGameActualCohortFixtureV1 Create(bool prompt,string item="NewGame")
        {
            var f=new NewGameActualCohortFixtureV1();
            f.Root=Path.Combine(Path.GetTempPath(),"AcadeGameMaker-C3-owner-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(f.Root);
            File.WriteAllBytes(Path.Combine(f.Root,"profile.json"),ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
            f._host=new GameObject("actual-c3-cohort"); f._host.SetActive(false);
            try
            {
                var runtime=new GameObject("launch"); runtime.transform.SetParent(f._host.transform,false);
                var router=runtime.AddComponent<InputRouter>(); var adapter=runtime.AddComponent<DesktopProfileLaunchAdapterV1>(); var latch=runtime.AddComponent<HubEntryHandoffLatchV1>();
                router.ConfigureHubUiOnlyForAuthoring(); adapter.ConfigureHubUiOnlyForAuthoring(router); adapter.ConfigureForTests(router,new EnvironmentPort(f.Root),new DesktopProfileLaunchPreparationPortV1()); latch.ConfigureHubUiOnlyForAuthoring(adapter,router);
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hub/HubMenuRoot.prefab"); Assert.That(prefab,Is.Not.Null);
                var menu=UnityEngine.Object.Instantiate(prefab,f._host.transform,false);
                var presenter=menu.GetComponent(HubType("HubMenuPresenterV1")); var handoffOwner=menu.GetComponent(HubType("HubMenuIntentHandoffOwnerV1"));
                var buttons=menu.GetComponentsInChildren(HubType("HubMenuButtonViewV1"),true);
                var authoredButtons=Array.CreateInstance(HubType("HubMenuButtonViewV1"),buttons.Length); for (var i=0;i<buttons.Length;i++) authoredButtons.SetValue(buttons[i],i);
                var notice=menu.transform.Find("SafeFrame/Notification"); var dismiss=notice.Find("Dismiss");
                Call(presenter,"ConfigureForAuthoring",router,latch,menu.GetComponent(HubType("HubSafeFrameScalerV1")),menu.GetComponentInChildren(Type.GetType("UnityEngine.EventSystems.EventSystem, UnityEngine.UI",true)),authoredButtons,dismiss.GetComponent<RectTransform>(),notice.gameObject,notice.Find("Message").GetComponent(Type.GetType("TMPro.TMP_Text, Unity.TextMeshPro",true)),dismiss.GetComponent(Type.GetType("TMPro.TMP_Text, Unity.TextMeshPro",true)));
                var owner=menu.AddComponent(HubType("NewGameConfirmationOwnerV1")); Call(owner,"ConfigureForAuthoring",presenter,handoffOwner,adapter,router);
                f.Owner=owner; f.Presenter=presenter; f.HandoffOwner=handoffOwner; f.Adapter=adapter; f.Router=router;
                f._host.SetActive(true);
                if (!router.PreparedLaunchAwakeEntered) Call(router,"Awake");
                if (!(bool)Field(adapter,"_awakeAttempted")) Call(adapter,"Awake");
                if (Field(latch,"_state").ToString() == "Pristine") Call(latch,"Awake");
                if (Convert.ToInt32(Read(presenter,"State")) == 0) Call(presenter,"Awake");
                if (Convert.ToInt32(Read(handoffOwner,"State")) == 0) Call(handoffOwner,"Awake");
                Call(adapter,"Start"); Call(latch,"Start"); Call(latch,"Update"); router.StepForTests(); Call(presenter,"Update");
                Assert.That(Read(presenter,"State").ToString(),Is.EqualTo("Ready"));
                if (prompt) File.WriteAllBytes(Path.Combine(f.Root,"profile.prev.json"),ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                Call(presenter,"Activate",Enum.Parse(typeof(HubMenuItemV1),item)); Call(handoffOwner,"LateUpdate");
                Assert.That(Read(handoffOwner,"State").ToString(),Is.EqualTo("RequestReady"));
                return f;
            }
            catch { f.Dispose(); throw; }
        }
        public object Take()
        { var args=new object[] { Owner,Presenter,Router,null }; Assert.That(Call(HandoffOwner,"TryTakeNewGameForConfirmation",args),Is.True); Assert.That(args[3],Is.Not.Null); return args[3]; }
        public object Accept(object handle) { return Call(Owner,"AcceptNewGame",handle,Adapter,Router); }
        public object[] IntakeSnapshot() { return new[] {Field(HandoffOwner,"_request"),Field(HandoffOwner,"_requestProof"),Field(HandoffOwner,"_takenRequest"),Field(HandoffOwner,"_takenRequestProof"),Field(HandoffOwner,"_epochs"),Field(HandoffOwner,"_pendingIssued"),Field(Owner,"_pendingIssued"),Field(Owner,"_acceptedIssued"),Field(Owner,"_display"),Field(Owner,"_epoch"),Field(Owner,"_history")}; }
        public static FieldInfo NegativeEvidenceField(object target,string name)
        {
            var alias=Alias(target); var expected=alias=="Q" && name=="_takenRequestProof"?T("Request?"):alias=="O" && name=="_epochProof"?typeof(long):alias=="O" && name=="_stateProof"?T("OState"):alias=="A" && name=="_launchRootWitnessB"?typeof(string):null;
            if (expected==null) throw new InvalidOperationException("Only four approved negative evidence fields are allowed.");
            var field=target.GetType().GetField(name,BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.NonPublic); if (field==null || field.FieldType!=expected || field.DeclaringType!=target.GetType()) throw new MissingFieldException(name); return field;
        }
        public IDisposable HoldRootLock() { return new FileStream(Path.Combine(Root,"profile.operation.lock"),FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None); }
        public void Step() { ((InputRouter)Router).StepForTests(); }
        public static Type HubType(string name) { return ExactType("AcadeGameMaker.Hub.Presentation.Unity."+name,"AcadeGameMaker.Hub.Presentation.Unity"); }
        private static Type ExactType(string name,string assembly) { var t=Type.GetType(name+", "+assembly,true); if (t.FullName!=name || t.Assembly.GetName().Name!=assembly) throw new InvalidOperationException("Exact type binding failed."); return t; }
        private static Type T(string name)
        {
            switch (name)
            {
                case "void":return typeof(void); case "bool":return typeof(bool); case "int":return typeof(int); case "long":return typeof(long); case "string":return typeof(string); case "object":return typeof(object);
                case "R":return typeof(InputRouter); case "A":return typeof(DesktopProfileLaunchAdapterV1); case "L":return typeof(HubEntryHandoffLatchV1);
                case "Cursor":return typeof(UiSemanticFrameCursorV1); case "CursorState":return typeof(UiSemanticFrameCursorStateV1); case "Capture":return typeof(ProfileNewGameCaptureResultV1); case "C":return typeof(ConfirmedProfileResetRequestV1); case "LatchState":return typeof(HubEntryHandoffStateV1);
                case "Rect":return typeof(RectTransform); case "GameObject":return typeof(GameObject); case "EventSystem":return ExactType("UnityEngine.EventSystems.EventSystem","UnityEngine.UI"); case "TMP":return ExactType("TMPro.TMP_Text","Unity.TextMeshPro");
                case "P":return HubType("HubMenuPresenterV1"); case "Q":return HubType("HubMenuIntentHandoffOwnerV1"); case "O":return HubType("NewGameConfirmationOwnerV1"); case "H":return HubType("IssuedNewGameRequestV1"); case "Z":return HubType("NewGameConfirmationStartResultV1");
                case "D":return HubType("NewGameDecisionCapabilityV1"); case "Retry":return HubType("NewGameCaptureRetryCapabilityV1"); case "E":return HubType("HubNewGameRearmCapabilityV1"); case "S":return HubType("HubNewGameSuccessorReservationV1");
                case "Item":return typeof(HubMenuItemV1); case "Receipt":return typeof(HubEntryHandoffReceiptV1); case "Intent":return typeof(HubMenuIntentV1); case "Request":return HubType("HubMenuIntentRequestV1");
                case "PState":return HubType("HubPresenterStateV1"); case "QState":return HubType("HubMenuIntentHandoffOwnerStateV1"); case "OState":return HubType("NewGameConfirmationOwnerStateV1"); case "Outcome":return HubType("NewGameConfirmationOutcomeV1"); case "Classification":return typeof(NewGameObservationClassificationV1);
                case "Scaler":return HubType("HubSafeFrameScalerV1"); case "Buttons":return HubType("HubMenuButtonViewV1").MakeArrayType();
                case "PSlot":return HubType("HubMenuPresenterV1+SuccessorEpochSlot"); case "QSlot":return HubType("HubMenuIntentHandoffOwnerV1+SuccessorRequestSlot"); case "History":return HubType("NewGameConfirmationOwnerV1+EpochHistory"); case "Record":return HubType("HubMenuIntentHandoffOwnerV1+TakenEpochRecord");
                case "Request?":return typeof(Nullable<>).MakeGenericType(T("Request")); case "Intent?":return typeof(Nullable<>).MakeGenericType(T("Intent"));
                case "H&":return T("H").MakeByRefType();
                default:throw new InvalidOperationException("Unknown exact type alias.");
            }
        }
        private static readonly Dictionary<string,string> Calls=new Dictionary<string,string>
        {
            {"P.ConfigureForAuthoring","void R L Scaler EventSystem Buttons Rect GameObject TMP TMP"},{"O.ConfigureForAuthoring","void P Q A R"},
            {"R.Awake","void"},{"A.Awake","void"},{"L.Awake","void"},{"P.Awake","void"},{"Q.Awake","void"},{"A.Start","void"},{"L.Start","void"},{"L.Update","void"},{"P.Update","void"},{"P.FixedUpdate","void"},{"Q.LateUpdate","void"},
            {"P.Activate","void Item"},{"P.ActivateSuccessor","void Item"},{"Q.TryTakeNewGameForConfirmation","bool O P R H&"},{"O.AcceptNewGame","Z H A R"},{"O.RetryIntake","Z Retry"},{"O.Confirm","Z D"},{"O.Cancel","Z D"},{"O.OpenFreshDecision","Z"},{"O.Rearm","void E"},{"O.SetFaultForTests","void string"},{"O.OnDisable","void"},{"O.CommitForExecution","C C"},
            {"Q.MatchesSuccessorReservation","bool S O P R object long"},{"Q.CommitNewGameSuccessor","void S O P R object long"},{"P.PrepareNewGameSuccessor","void O S object long"}
        };
        private static readonly Dictionary<string,string> Properties=new Dictionary<string,string>
        {
            {"Z.Outcome","Outcome"},{"Z.Classification","Classification"},{"Z.DecisionCapability","D"},{"Z.RetryCapability","Retry"},{"Z.RearmCapability","E"},{"Z.ConfirmedRequest","C"},{"Z.Prompt","string"},{"Z.ConfirmLabel","string"},{"Z.CancelLabel","string"},
            {"O.State","OState"},{"Q.State","QState"},{"P.State","PState"},{"Request.Item","Item"},{"Request.Receipt","Receipt"},{"Cursor.State","CursorState"}
        };
        private static readonly Dictionary<string,string> Fields=new Dictionary<string,string>
        {
            {"A._awakeAttempted","bool"},{"L._state","LatchState"},{"O._state","OState"},{"O._operation","int"},{"O._epoch","long"},{"O._generation","long"},{"O._epochToken","object"},{"O._history","History"},{"O._pendingIssued","H"},{"O._acceptedIssued","H"},{"O._decision","D"},{"O._rearm","E"},{"O._display","Capture"},{"O._confirmed","C"},
            {"Q._request","Request?"},{"Q._requestProof","Request?"},{"Q._takenRequest","Request?"},{"Q._takenRequestProof","Request?"},{"Q._epochs","Record"},{"Q._successor","QSlot"},{"Q._pendingIssued","H"},{"P._cursor","Cursor"},{"P._retainedIntent","Intent?"},{"P._successor","PSlot"},{"P._confirmationInteractionClosed","bool"},
            {"P._latch","L"},{"P._safeFrame","Scaler"},{"P._eventSystem","EventSystem"},{"P._buttons","Buttons"},{"P._dismissRect","Rect"},{"P._notice","GameObject"},{"P._noticeText","TMP"},{"P._dismissText","TMP"},
            {"PSlot.Cursor","Cursor"},{"PSlot.BaselinePending","bool"},{"PSlot.Phase","PState"},{"PSlot.Retained","Intent?"},{"PSlot.Reservation","S"},{"PSlot.Token","object"},{"PSlot.Epoch","long"},{"History.Previous","History"}
        };
        private static string Alias(object target)
        { if (target==null) throw new InvalidOperationException("Null exact receiver."); foreach (var name in new[] {"R","A","L","P","Q","O","Z","Request","Cursor","PSlot","QSlot","History","Record"}) if (target.GetType()==T(name)) return name; throw new InvalidOperationException("Unknown exact receiver."); }
        public static MethodInfo Method(object target,string name)
        {
            var key=Alias(target)+"."+name; if (!Calls.TryGetValue(key,out var signature)) throw new MissingMethodException(key);
            var parts=signature.Split(' '); var parameters=new Type[parts.Length-1]; for (var i=1;i<parts.Length;i++) parameters[i-1]=T(parts[i]);
            var declaring=target.GetType(); var method=declaring.GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.DeclaredOnly,null,parameters,null);
            if (method==null || method.DeclaringType!=declaring || method.ReturnType!=T(parts[0]) || method.IsGenericMethod || method.GetParameters().Length!=parameters.Length) throw new MissingMethodException(key);
            var actual=method.GetParameters(); for (var i=0;i<actual.Length;i++) if (actual[i].ParameterType!=parameters[i] || actual[i].IsOptional || actual[i].IsOut != (parts[i+1]=="H&")) throw new MissingMethodException(key);
            return method;
        }
        public static object Call(object target,string name,params object[] args)
        { try { return Method(target,name).Invoke(target,args); } catch (TargetInvocationException e) { if (e.InnerException != null) throw e.InnerException; throw; } }
        public static object Read(object target,string name)
        {
            var key=Alias(target)+"."+name; if (!Properties.TryGetValue(key,out var result)) throw new MissingMemberException(key);
            var declaration=target.GetType(); var getter=declaration.GetMethod("get_"+name,BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.DeclaredOnly,null,Type.EmptyTypes,null);
            if (getter==null || getter.DeclaringType!=declaration || getter.ReturnType!=T(result) || getter.IsGenericMethod) throw new MissingMemberException(key);
            try { return getter.Invoke(target,null); } catch (TargetInvocationException e) { if (e.InnerException!=null) throw e.InnerException; throw; }
        }
        public static object Field(object target,string name)
        {
            var key=Alias(target)+"."+name; if (!Fields.TryGetValue(key,out var result)) throw new MissingFieldException(key);
            var declaration=target.GetType(); var field=declaration.GetField(name,BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.DeclaredOnly);
            if (field==null || field.DeclaringType!=declaration || field.FieldType!=T(result) || field.IsStatic) throw new MissingFieldException(key);
            var value=field.GetValue(target); var expected=Nullable.GetUnderlyingType(field.FieldType)??field.FieldType;
            if (value!=null && !expected.IsInstanceOfType(value)) throw new InvalidOperationException("Exact field value differs."); return value;
        }
        public void Dispose()
        {
            if (_host != null) UnityEngine.Object.DestroyImmediate(_host);
            var root=Path.GetFullPath(Root); var temp=Path.GetFullPath(Path.GetTempPath());
            Assert.That(root.StartsWith(temp,StringComparison.OrdinalIgnoreCase) && Path.GetFileName(root).StartsWith("AcadeGameMaker-C3-owner-",StringComparison.Ordinal) && !File.GetAttributes(root).HasFlag(FileAttributes.ReparsePoint),Is.True);
            if (Directory.Exists(root)) Directory.Delete(root,true);
        }
        private sealed class EnvironmentPort : IDesktopProfileLaunchEnvironmentV1
        { private readonly string _root; internal EnvironmentPort(string root) { _root=root; } public string GetPersistentDataPath() { return _root; } public DateTimeOffset GetUtcNow() { return DateTimeOffset.UtcNow; } }
    }
}
