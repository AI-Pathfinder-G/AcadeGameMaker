using System;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using AcadeGameMaker.Profile;
using AcadeGameMaker.Input.Unity;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.InputUnity
{
    // AC-M5D7QC4-001/003/005/007/008: 실제 정상 발급과 독립 관리 문맥을 검증한다.
    public sealed class ProfileResetExecutionBridgeV1Tests
    {
        private readonly List<C4ActualExecutionEditFixtureV1.EvidenceRow> _evidenceRows=new List<C4ActualExecutionEditFixtureV1.EvidenceRow>();
        [TearDown]
        public void FinishActualEvidenceRows()
        {
            foreach (var row in _evidenceRows) row.Finish();
            _evidenceRows.Clear();
        }
        private C4ActualExecutionEditFixtureV1.EvidenceRow BeginEvidence(string id,string kind,string family,string checkpoint,int? value,string[] requirements,string[] acceptance)
        {
            var row=new C4ActualExecutionEditFixtureV1.EvidenceRow(id,requirements,acceptance,kind,family,checkpoint,value);
            _evidenceRows.Add(row); return row;
        }
        private static void ObserveUnreturnedNoNative(C4ActualExecutionEditFixtureV1.EvidenceRow row,string exactParent)
        {
            Assert.That(TestContext.CurrentContext.Test.MethodName,Is.EqualTo(exactParent));
            const string testPath="Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs";
            var lines=File.ReadAllLines(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath),testPath));var declarationLine=0;
            for(var i=0;i<lines.Length;i++)if(lines[i].TrimStart().StartsWith("public void "+exactParent+"(",StringComparison.Ordinal)){Assert.That(declarationLine,Is.Zero);declarationLine=i+1;}
            Assert.That(declarationLine,Is.GreaterThan(0));
            C4ActualExecutionEditFixtureV1.ObserveNoNativeObserverRequired(row,"source:"+testPath+":"+declarationLine);
        }
        private static void ObserveTypedResult(C4ActualExecutionEditFixtureV1 f,C4ActualExecutionEditFixtureV1.EvidenceRow row,ProfileResetExecutionResultV1 result,ExecutionCounts counts)
        {
            f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result);
            row.Observe("callCounts.c1Begin",counts.C1); row.Observe("callCounts.c2Finalize",counts.C2);
            row.Observe("callCounts.preparedObserver",counts.PreparedObserver); row.Observe("callCounts.c2Observer",counts.C2Observer);
            row.Observe("callCounts.freshReserve",counts.FreshReserve);
            f.ObserveAbsentFreshCompletion(row,result);
            const string testPath="Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs";
            var lines=File.ReadAllLines(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath),testPath));
            var declaration="public void "+TestContext.CurrentContext.Test.MethodName+"("; var declarationLine=0;
            for (var i=0;i<lines.Length;i++) if (lines[i].TrimStart().StartsWith(declaration,StringComparison.Ordinal))
            { Assert.That(declarationLine,Is.Zero); declarationLine=i+1; }
            Assert.That(declarationLine,Is.GreaterThan(0));
            C4ActualExecutionEditFixtureV1.ObserveNoNativeObserverRequired(row,"source:"+testPath+":"+declarationLine);
            f.Dispose(); f.ObserveActualCleanup(row);
        }
        private static void ObserveStructureScope(C4ActualExecutionEditFixtureV1.EvidenceRow row,bool pureValidator=false)
        {
            foreach (var key in new[]{"confirmed","owner","adapter","router","root","proof","c2Result","receipt","nextEpoch"}) row.Observe("authorityCorrelation."+key,"Unknown","Unknown");
            row.Observe("authorityCorrelation.executionGeneration",null,"Unknown"); row.Observe("authorityCorrelation.memoryGeneration",null,"Unknown");
            row.Observe("authorityCorrelation.checked",false,"SourceEstablished");
            foreach (var key in new[]{"adapter","router","permanentFaultRecorded"}) row.Observe("guard."+key,"Unknown","Unknown");
            row.Observe("barrier.state","Unknown","Unknown"); row.Observe("barrier.ordinaryWriterBlocked","Unknown","Unknown"); row.Observe("barrier.evidenceReference",null,"SourceEstablished");
            foreach (var key in new[]{"originalCommitPreserved","originalEpochPreserved","oldTakePreserved","oldCursorPreserved","receiptReferencePreserved"}) row.Observe("history."+key,"Unknown","Unknown");
            row.Observe("cleanup.required",false,"SourceEstablished");
            foreach (var key in new[]{"releaseSignaled","workerLeaseReleased","workerJoined","originalThreadCleanup"}) row.Observe("cleanup."+key,"NotApplicable","SourceEstablished");
            row.Observe("cleanup.exception",null,"SourceEstablished");
            foreach (var key in new[]{"c1Begin","c2Finalize","freshReserve","freshComplete","preparedObserver","c2Observer","nativeCallback"}) row.Observe("callCounts."+key,0,"SourceEstablished");
            row.Observe("diagnostic.generationUnobservedReason",pureValidator ? "PureValidatorGenerationOutsideScope" : "StructureGenerationOutsideScope","SourceEstablished");
            row.Observe("diagnostic.barrierEvidenceAbsentReason","NoBarrierEvidenceRequiredForThisPreEntryOrPureRow","SourceEstablished");
            row.Observe("diagnostic.cleanupNotRequiredReason",pureValidator ? "PureValidatorHasNoRuntimeCleanup" : "StructureAuditHasNoRuntimeCleanup","SourceEstablished");
            if(!pureValidator) row.Observe("diagnostic.strictAuditCompleted","Yes","SourceEstablished");
        }
        [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(7)] [TestCase(8)] [TestCase(9)] [TestCase(10)]
        [TestCase(11)] [TestCase(12)] [TestCase(13)] [TestCase(14)] [TestCase(15)] [TestCase(16)] [TestCase(17)] [TestCase(18)] [TestCase(19)] [TestCase(20)]
        [TestCase(21)] [TestCase(22)] [TestCase(23)] [TestCase(24)] [TestCase(25)] [TestCase(26)] [TestCase(27)] [TestCase(28)] [TestCase(29)] [TestCase(30)]
        [TestCase(31)] [TestCase(32)] [TestCase(33)] [TestCase(34)] [TestCase(35)] [TestCase(36)] [TestCase(37)] [TestCase(38)] [TestCase(39)] [TestCase(40)]
        [TestCase(41)] [TestCase(42)] [TestCase(43)] [TestCase(44)] [TestCase(45)] [TestCase(46)] [TestCase(47)] [TestCase(48)] [TestCase(49)] [TestCase(50)] [TestCase(51)] [Timeout(180000)]
        public void AC006_ActualC1CheckpointThrowPreservesBarrierAndGuard(int diskCheckpoint)
        {
            var enumType=C4ActualExecutionEditFixtureV1.ProfileType("ProfileResetDiskCheckpointV1");
            Assert.That(Enum.IsDefined(enumType,diskCheckpoint),Is.True);
            var evidence=BeginEvidence("edit_AC006_ActualC1CheckpointThrowPreservesBarrierAndGuard_"+diskCheckpoint,"FullBridge","C1",Enum.GetName(enumType,diskCheckpoint),diskCheckpoint,new[]{"REQ-M5D7QC4-002","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-006"});
            using (var f=C4ActualExecutionEditFixtureV1.Create(true,true))
            {
                var reached=0; var visits=0; var c1=0; var c2=0; var prepared=0; var memory=0; var reserve=0;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                { if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1++; if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2++; if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) reserve++; },()=>{},proof=>prepared++,actualMemory=>memory++);
                var diskControl=C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>
                {
                    // 원래 세 leaf의 실제 보관 완료 뒤 새 중단 파일을 만들어 해당 정상 보존 분기를 통과시킨다.
                    if (value == 24 && diskCheckpoint >= 25 && diskCheckpoint <= 32)
                        File.WriteAllText(Path.Combine(f.Root,diskCheckpoint <= 28 ? "profile.tmp.json" : "profile.json"),"실제 중단 파일");
                    if (value == diskCheckpoint)
                    {
                        visits++;
                        if (reached == 0) { reached++; evidence.Reached(); throw new IOException("실제 C1 지점 종료 시험: "+name); }
                    }
                });
                var result=f.Execute(control,diskControl);
                Assert.That(reached,Is.EqualTo(1),"계획한 실제 C1 지점에 도달하지 않았습니다.");
                var recoveryRevisits=diskCheckpoint == 40 || diskCheckpoint == 41 || diskCheckpoint >= 44 && diskCheckpoint <= 49;
                Assert.That(visits,Is.EqualTo(recoveryRevisits ? 2 : 1),"실제 C1 복구 조회 횟수가 다릅니다.");
                Assert.That(c1,Is.EqualTo(1)); Assert.That(c2,Is.Zero);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.ReloadRequired).Or.EqualTo(ProfileResetExecutionOutcomeV1.ManualRepairRequired));
                Assert.That(File.Exists(Path.Combine(f.Root,"profile.reset.json")),Is.EqualTo(diskCheckpoint >= 10));
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router),Throws.InvalidOperationException);
                result.Validate(); Assert.That(reached,Is.EqualTo(1));
                f.ObserveActualResult(evidence,result);
                f.ObserveActualBarrier(evidence,result);
                evidence.Observe("callCounts.c1Begin",c1); evidence.Observe("callCounts.c2Finalize",c2);
                evidence.Observe("callCounts.preparedObserver",prepared); evidence.Observe("callCounts.c2Observer",memory);
                evidence.Observe("callCounts.freshReserve",reserve);
                f.ObserveAbsentFreshCompletion(evidence,result);
                ObserveUnreturnedNoNative(evidence,"AC006_ActualC1CheckpointThrowPreservesBarrierAndGuard");
                f.Dispose(); f.ObserveActualCleanup(evidence);
            }
        }
        [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)] [TestCase(5)] [TestCase(6)] [TestCase(7)] [TestCase(8)] [TestCase(9)]
        [TestCase(10)] [TestCase(11)] [TestCase(12)] [TestCase(13)] [TestCase(14)] [TestCase(15)] [TestCase(16)] [TestCase(17)] [TestCase(18)]
        [TestCase(19)] [TestCase(20)] [TestCase(21)] [TestCase(22)] [TestCase(23)] [TestCase(24)] [TestCase(25)] [TestCase(26)] [TestCase(27)] [Timeout(180000)]
        public void AC008_ActualC4CheckpointFailureNeverRestoresAuthority(int checkpoint)
        {
            var preC1=checkpoint >= 1 && checkpoint <= 7;
            var preC2=checkpoint >= 8 && checkpoint <= 11;
            var observedProofCheckpoint=checkpoint == 12 || checkpoint == 13;
            var freshPublishCheckpoint=checkpoint == 17;
            var freshFailureCheckpoint=checkpoint >= 19 && checkpoint <= 25;
            var issuedResultCheckpoint=checkpoint == 18 || checkpoint == 27;
            var receiptWithoutResultCheckpoint=(checkpoint >= 14 && checkpoint <= 16) || checkpoint == 26;
            var checkpointName=Enum.GetName(typeof(ProfileResetExecutionCheckpointV1),checkpoint);
            Assert.That(checkpointName,Is.Not.Null);
            var row=preC1 || preC2 || observedProofCheckpoint || freshPublishCheckpoint || freshFailureCheckpoint || issuedResultCheckpoint || receiptWithoutResultCheckpoint ? BeginEvidence("edit_AC008_ActualC4CheckpointFailureNeverRestoresAuthority_"+checkpoint,(freshPublishCheckpoint || freshFailureCheckpoint || checkpoint == 18) ? "ActualWorker" : "FullBridge","C4",checkpointName,checkpoint,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-003","REQ-M5D7QC4-005"},new[]{"AC-M5D7QC4-003","AC-M5D7QC4-005","AC-M5D7QC4-008"}) : null;
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var reached=0; var c1Returns=0; var c2Calls=0; var preparedObservers=0; var c2Observers=0; var freshReserves=0; object observedProof=null; object observedMemory=null;
                Exception callbackFailure=null; Exception caughtFailure=null; C4ActualExecutionEditFixtureV1.RootLeaseWorker actualLease=null; var heldAtCheckpoint=false; ProfileResetExecutionResultV1 freshFailureResult=null;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(actual=>
                {
                    if (actual == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1Returns++;
                    if (actual == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2Calls++;
                    if (actual == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) freshReserves++;
                    if ((int)actual == checkpoint) { reached++; if(preC1 || preC2) row.Reached(); if(freshPublishCheckpoint) heldAtCheckpoint=actualLease.HoldsRootAtCheckpoint(f.Root); callbackFailure=new InvalidOperationException("실제 C4 지점 종료 시험"); throw callbackFailure; }
                },()=>{},proof=>{preparedObservers++;observedProof=proof;},memory=>{c2Observers++;observedMemory=memory;});
                var disk=C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{});
                if (checkpoint >= 17 && checkpoint <= 25)
                {
                    ProfileResetExecutionResultV1 result=null;
                    if (freshPublishCheckpoint)
                    {
                        using (actualLease=new C4ActualExecutionEditFixtureV1.RootLeaseWorker(f.Root)) caughtFailure=Assert.Throws<InvalidOperationException>(()=>f.Execute(control,disk));
                    }
                    else if (checkpoint <= 18)
                    {
                        using (actualLease=new C4ActualExecutionEditFixtureV1.RootLeaseWorker(f.Root)) caughtFailure=Assert.Throws<InvalidOperationException>(()=>f.Execute(control,disk));
                    }
                    else
                    {
                        using (actualLease=new C4ActualExecutionEditFixtureV1.RootLeaseWorker(f.Root)) result=f.Execute(control,disk);
                        Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Busy));
                        caughtFailure=Assert.Throws<InvalidOperationException>(() => f.AcceptFresh(result));
                        freshFailureResult=result;
                    }
                }
                else if (preC1 || preC2 || observedProofCheckpoint || receiptWithoutResultCheckpoint) caughtFailure=Assert.Throws<InvalidOperationException>(()=>f.Execute(control,disk));
                else if (checkpoint == 27) caughtFailure=Assert.Throws<InvalidOperationException>(()=>f.Execute(control,disk));
                else Assert.That(() => f.Execute(control,disk),Throws.InvalidOperationException);
                Assert.That(reached,Is.EqualTo(1));
                if (preC1 || preC2)
                {
                    if(preC1) f.ObservePreC1Failure(row,caughtFailure,callbackFailure,(ProfileResetExecutionCheckpointV1)checkpoint,reached,c1Returns,c2Calls,preparedObservers,c2Observers,freshReserves);
                    else f.ObservePreC2Failure(row,caughtFailure,callbackFailure,(ProfileResetExecutionCheckpointV1)checkpoint,reached,c1Returns,c2Calls,preparedObservers,c2Observers,freshReserves);
                    const string testPath="Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs";
                    var lines=File.ReadAllLines(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath),testPath)); var declarationLine=0;
                    for(var i=0;i<lines.Length;i++) if(lines[i].TrimStart().StartsWith("public void AC008_ActualC4CheckpointFailureNeverRestoresAuthority(",StringComparison.Ordinal)){Assert.That(declarationLine,Is.Zero);declarationLine=i+1;}
                    Assert.That(declarationLine,Is.GreaterThan(0));
                    C4ActualExecutionEditFixtureV1.ObserveNoNativeObserverRequired(row,"source:"+testPath+":"+declarationLine);
                }
                else if (observedProofCheckpoint)
                {
                    f.ObserveUnreturnedObservedProofCheckpoint(row,caughtFailure,callbackFailure,(ProfileResetExecutionCheckpointV1)checkpoint,observedProof,reached,c1Returns,c2Calls,preparedObservers,c2Observers,freshReserves);
                    row.Reached(); ObserveUnreturnedNoNative(row,"AC008_ActualC4CheckpointFailureNeverRestoresAuthority");
                }
                else if (freshPublishCheckpoint)
                {
                    f.ObserveUnreturnedFreshPublishCheckpoint(row,caughtFailure,callbackFailure,actualLease,heldAtCheckpoint,reached,c1Returns,c2Calls,preparedObservers,c2Observers,freshReserves);
                    row.Reached(); ObserveUnreturnedNoNative(row,"AC008_ActualC4CheckpointFailureNeverRestoresAuthority");
                }
                else if (freshFailureCheckpoint)
                {
                    Assert.That(caughtFailure,Is.SameAs(callbackFailure)); row.Reached();
                    row.Observe("diagnostic.freshPartialCallbackExceptionSame","Yes");
                    f.ObserveActualResult(row,freshFailureResult); f.ObserveActualBarrier(row,freshFailureResult,actualLease);
                    row.Observe("callCounts.c1Begin",c1Returns); row.Observe("callCounts.c2Finalize",c2Calls);
                    row.Observe("callCounts.preparedObserver",preparedObservers); row.Observe("callCounts.c2Observer",c2Observers);
                    f.ObserveActualFreshFailureCounts(row,freshFailureResult,checkpoint);
                    ObserveUnreturnedNoNative(row,"AC008_ActualC4CheckpointFailureNeverRestoresAuthority");
                }
                else if (issuedResultCheckpoint)
                {
                    Assert.That(caughtFailure,Is.SameAs(callbackFailure));
                    var issuedResult=f.ReadIssuedActualResult(); issuedResult.Validate(); row.Reached();
                    row.Observe("diagnostic.issuedResultCheckpointExceptionSame","Yes");
                    f.ObserveActualResult(row,issuedResult); f.ObserveActualBarrier(row,issuedResult,actualLease);
                    row.Observe("callCounts.c1Begin",c1Returns); row.Observe("callCounts.c2Finalize",c2Calls);
                    row.Observe("callCounts.preparedObserver",preparedObservers); row.Observe("callCounts.c2Observer",c2Observers);
                    row.Observe("callCounts.freshReserve",freshReserves); f.ObserveAbsentFreshCompletion(row,issuedResult);
                    ObserveUnreturnedNoNative(row,"AC008_ActualC4CheckpointFailureNeverRestoresAuthority");
                }
                else if (receiptWithoutResultCheckpoint)
                {
                    Assert.That(caughtFailure,Is.SameAs(callbackFailure));
                    f.ObserveIssuedExecutionWithoutResult(row,caughtFailure,true);
                    f.ObserveReceiptWithoutExecutionResult(row,observedMemory,c2Observers,checkpoint == 14 ? "NotInspected" : "Validated");
                    f.ObserveNoResultBarrierGuardHistory(row,observedProof,observedMemory);
                    row.Reached(); row.Observe("diagnostic.receiptCheckpointExceptionSame","Yes");
                    row.Observe("callCounts.c1Begin",c1Returns); row.Observe("callCounts.c2Finalize",c2Calls);
                    row.Observe("callCounts.preparedObserver",preparedObservers); row.Observe("callCounts.c2Observer",c2Observers);
                    row.Observe("callCounts.freshReserve",freshReserves);
                    ObserveUnreturnedNoNative(row,"AC008_ActualC4CheckpointFailureNeverRestoresAuthority");
                }
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router),Throws.InvalidOperationException);
                Assert.That(reached,Is.EqualTo(1));
                if (preC1 || preC2 || observedProofCheckpoint || freshPublishCheckpoint || freshFailureCheckpoint || issuedResultCheckpoint || receiptWithoutResultCheckpoint)
                {
                    f.Dispose(); f.ObserveActualCleanup(row);
                    if(freshFailureCheckpoint || checkpoint == 18 || freshPublishCheckpoint)
                    {
                        row.Observe("cleanup.releaseSignaled",actualLease.ReleaseSignaled ? "Yes" : "No");
                        row.Observe("cleanup.workerLeaseReleased",actualLease.LeaseReleased ? "Yes" : "No");
                        row.Observe("cleanup.workerJoined",actualLease.Joined ? "Yes" : "No");
                    }
                }
            }
        }
        [Test] [Timeout(180000)]
        public void AC001_ConsumedAndLateLoserCannotCloseWinner()
        {
            var row=BeginEvidence("edit_AC001_ConsumedAndLateLoserCannotCloseWinner","FullBridge","None",null,null,new[]{"REQ-M5D7QC4-001"},new[]{"AC-M5D7QC4-001"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var result=f.ExecuteOwner(); var generation=result.ExecutionGeneration;
                row.Reached();
                Assert.That(() => f.ExecuteOwner(),Throws.InvalidOperationException);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(result.ExecutionGeneration,Is.EqualTo(generation));
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                Assert.That(C4ActualExecutionEditFixtureV1.Read(f.Owner,C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerV1"),"State",C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerStateV1")).ToString(),Is.EqualTo("ExecutionCommitted"));
                f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result); f.ObserveNoOpCallCounts(row,result);
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        [Test] [Timeout(180000)]
        public void AC002_ActualBothGuardHalvesBlockOldPublication()
        {
            var row=BeginEvidence("edit_AC002_ActualBothGuardHalvesBlockOldPublication","FullBridge","C4","AfterRouterGuard",6,
                new[]{"REQ-M5D7QC4-002"},new[]{"AC-M5D7QC4-002"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var guarded=0; var before=f.Router.CurrentReceipt.Value; var counts=new ExecutionCounts();
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                {
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) counts.C1++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) counts.C2++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) counts.FreshReserve++;
                    if (checkpoint != ProfileResetExecutionCheckpointV1.AfterRouterGuard) return;
                    guarded++; row.Reached();
                    Assert.That(ProfileNewGameConfirmationV1.IsExecutionGuardedPair(f.Adapter,f.Router),Is.True);
                    Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                    Assert.That(f.Router.CurrentReceipt.Value,Is.EqualTo(before));
                    Assert.That(f.Adapter.TryTakeNotification(out _),Is.False);
                },()=>{},proof=>counts.PreparedObserver++,memory=>counts.C2Observer++);
                var result=f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                Assert.That(guarded,Is.EqualTo(1)); Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(counts.C1,Is.EqualTo(1)); Assert.That(counts.C2,Is.EqualTo(1));
                ObserveTypedResult(f,row,result,counts);
            }
        }
        [TestCase(3)] [TestCase(4)] [TestCase(5)] [TestCase(6)] [Timeout(180000)]
        public void AC002_ThrowAtGuardBoundaryClosesOriginalPair(int checkpoint)
        {
            var checkpointName=Enum.GetName(typeof(ProfileResetExecutionCheckpointV1),checkpoint);
            Assert.That(checkpointName,Is.Not.Null);
            var row=BeginEvidence("edit_AC002_ThrowAtGuardBoundaryClosesOriginalPair_"+checkpoint,"FullBridge","C4",checkpointName,checkpoint,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-002"},new[]{"AC-M5D7QC4-002"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var reached=0; var c1=0; var c2=0; var prepared=0; var memory=0; var reserve=0;
                Exception callbackFailure=null;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(actual=>
                {
                    if (actual == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1++;
                    if (actual == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2++;
                    if (actual == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) reserve++;
                    if ((int)actual == checkpoint) { reached++; row.Reached(); callbackFailure=new InvalidOperationException("실제 가드 경계 종료 시험"); throw callbackFailure; }
                },()=>{},proof=>prepared++,actualMemory=>memory++);
                var failure=Assert.Throws<InvalidOperationException>(()=>f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{})));
                Assert.That(reached,Is.EqualTo(1)); Assert.That(c1,Is.Zero); Assert.That(c2,Is.Zero);
                f.ObservePreC1Failure(row,failure,callbackFailure,(ProfileResetExecutionCheckpointV1)checkpoint,reached,c1,c2,prepared,memory,reserve);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router),Throws.InvalidOperationException);
                Assert.That(reached,Is.EqualTo(1));
                const string testPath="Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs";
                var lines=File.ReadAllLines(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath),testPath)); var declarationLine=0;
                for(var i=0;i<lines.Length;i++) if(lines[i].TrimStart().StartsWith("public void AC002_ThrowAtGuardBoundaryClosesOriginalPair(",StringComparison.Ordinal)){Assert.That(declarationLine,Is.Zero);declarationLine=i+1;}
                Assert.That(declarationLine,Is.GreaterThan(0));
                C4ActualExecutionEditFixtureV1.ObserveNoNativeObserverRequired(row,"source:"+testPath+":"+declarationLine);
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        [Test] [Timeout(180000)]
        public void AC003_ActualBeginUsesOriginalIdentityOnce()
        {
            var row=BeginEvidence("edit_AC003_ActualBeginUsesOriginalIdentityOnce","FullBridge","C4","AfterC1Begin",8,new[]{"REQ-M5D7QC4-003"},new[]{"AC-M5D7QC4-003"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                object actualProof=null,actualMemory=null; var counts=new ExecutionCounts();
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                { if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) { counts.C1++; row.Reached(); } if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) counts.C2++; if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) counts.FreshReserve++; },
                ()=>{},proof=>{ counts.PreparedObserver++; Assert.That(actualProof,Is.Null); actualProof=proof; },memory=>{ counts.C2Observer++; Assert.That(actualMemory,Is.Null); actualMemory=memory; });
                var result=f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                Assert.That(actualProof,Is.Not.Null); Assert.That(actualMemory,Is.Not.Null);
                Assert.That(counts.C1,Is.EqualTo(1)); Assert.That(counts.C2,Is.EqualTo(1));
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                ObserveTypedResult(f,row,result,counts);
            }
        }
        [Test] [Timeout(180000)]
        public void AC005_ActualDiskPreparedPassesSameProofRootPairOnce()
        {
            var row=BeginEvidence("edit_AC005_ActualDiskPreparedPassesSameProofRootPairOnce","FullBridge","None",null,null,new[]{"REQ-M5D7QC4-005"},new[]{"AC-M5D7QC4-005"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var observed=0; var c2=0; object originalProof=null; var counts=new ExecutionCounts();
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                { if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) counts.C1++; if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) { c2++; counts.C2++; } if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) counts.FreshReserve++; },()=>{},proof=>
                { observed++; counts.PreparedObserver++; row.Reached(); originalProof=proof; Assert.That(proof.GetType(),Is.EqualTo(C4ActualExecutionEditFixtureV1.ProfileType("ProfileResetDiskPreparedProofV1"))); },memory=>counts.C2Observer++);
                var result=f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                Assert.That(originalProof,Is.Not.Null); Assert.That(observed,Is.EqualTo(1)); Assert.That(c2,Is.EqualTo(1));
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(f.Adapter.CurrentResetSession.Generation,Is.GreaterThan(0));
                ObserveTypedResult(f,row,result,counts);
            }
        }
        [Test] [Timeout(180000)]
        public void AC004_ActualC1BusyIssuesOneFreshHandback()
        {
            var row=BeginEvidence("edit_AC004_ActualC1BusyIssuesOneFreshHandback","ActualWorker","None",null,null,
                new[]{"REQ-M5D7QC4-004"},new[]{"AC-M5D7QC4-004"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                ProfileResetExecutionResultV1 result;
                var worker=new C4ActualExecutionEditFixtureV1.RootLeaseWorker(f.Root);
                using (worker) result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router);
                Assert.That(worker.ReleaseSignaled,Is.True); Assert.That(worker.LeaseReleased,Is.True); Assert.That(worker.Joined,Is.True);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Busy));
                Assert.That(result.Phase,Is.EqualTo(ProfileResetExecutionPhaseV1.C1Returned));
                f.AcceptFresh(result);
                row.Reached();
                Assert.That(C4ActualExecutionEditFixtureV1.Read(f.Owner,C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerV1"),"State",C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerStateV1")).ToString(),Is.EqualTo("AwaitingRequest"));
                Assert.That(() => f.AcceptFresh(result),Throws.InvalidOperationException);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Busy));
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result,worker); f.ObserveNoOpCallCounts(row,result);
                f.Dispose(); f.ObserveActualCleanup(row);
                row.Observe("cleanup.releaseSignaled",worker.ReleaseSignaled ? "Yes" : "No");
                row.Observe("cleanup.workerLeaseReleased",worker.LeaseReleased ? "Yes" : "No");
                row.Observe("cleanup.workerJoined",worker.Joined ? "Yes" : "No");
            }
        }
        [Test] [Timeout(180000)]
        public void AC004_ActualC1StaleIssuesOneFreshHandback()
        {
            var row=BeginEvidence("edit_AC004_ActualC1StaleIssuesOneFreshHandback","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-004"},new[]{"AC-M5D7QC4-004"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                File.AppendAllText(Path.Combine(f.Root,"profile.json"),"\n");
                var result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.ConfirmationStale));
                f.AcceptFresh(result);
                row.Reached();
                Assert.That(() => f.AcceptFresh(result),Throws.InvalidOperationException);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.ConfirmationStale));
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result); f.ObserveNoOpCallCounts(row,result);
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        [TestCase(19)] [TestCase(20)] [TestCase(21)] [TestCase(22)] [TestCase(23)] [TestCase(24)] [TestCase(25)] [Timeout(180000)]
        public void AC004_FreshPartialFailureClosesBothSlots(int checkpoint)
        {
            var row=BeginEvidence("edit_AC004_FreshPartialFailureClosesBothSlots_"+checkpoint,"FullBridge","C4",Enum.GetName(typeof(ProfileResetExecutionCheckpointV1),checkpoint),checkpoint,
                new[]{"REQ-M5D7QC4-004","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-004","AC-M5D7QC4-008"});
            AssertActualFreshPartialFailure(row,checkpoint);
        }
        private static void AssertActualFreshPartialFailure(C4ActualExecutionEditFixtureV1.EvidenceRow row,int checkpoint)
        {
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var reached=0; var c1=0; var c2=0; var prepared=0; var memory=0; Exception callbackFailure=null;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(actual=>
                {
                    if (actual == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1++;
                    if (actual == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2++;
                    if ((int)actual == checkpoint) { reached++; callbackFailure=new InvalidOperationException("실제 새 슬롯 부분 종료 시험"); throw callbackFailure; }
                },()=>{},proof=>prepared++,actualMemory=>memory++);
                ProfileResetExecutionResultV1 result;
                var lease=new C4ActualExecutionEditFixtureV1.RootLeaseWorker(f.Root);
                using (lease) result=f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                Assert.That(lease.ReleaseSignaled,Is.True); Assert.That(lease.LeaseReleased,Is.True); Assert.That(lease.Joined,Is.True);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Busy));
                var caughtFailure=Assert.Throws<InvalidOperationException>(() => f.AcceptFresh(result));
                Assert.That(caughtFailure,Is.SameAs(callbackFailure));
                Assert.That(reached,Is.EqualTo(1)); Assert.That(c1,Is.EqualTo(1)); Assert.That(c2,Is.Zero);
                Assert.That(prepared,Is.Zero); Assert.That(memory,Is.Zero);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                Assert.That(C4ActualExecutionEditFixtureV1.Read(f.Owner,C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerV1"),"State",C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerStateV1")).ToString(),Is.EqualTo("TerminalFailure"));
                result.Validate();
                row.Reached(); row.Observe("diagnostic.freshPartialCallbackExceptionSame","Yes");
                f.ObserveActualResult(row,result); f.ObserveActualGuard(row); f.ObserveActualHistory(row);
                f.ObserveActualBarrier(row,result,lease);
                row.Observe("callCounts.c1Begin",c1); row.Observe("callCounts.c2Finalize",c2);
                row.Observe("callCounts.preparedObserver",prepared); row.Observe("callCounts.c2Observer",memory);
                f.ObserveActualFreshFailureCounts(row,result,checkpoint);
                ObserveUnreturnedNoNative(row,TestContext.CurrentContext.Test.MethodName);
                f.Dispose(); f.ObserveActualCleanup(row);
                row.Observe("cleanup.releaseSignaled",lease.ReleaseSignaled ? "Yes" : "No");
                row.Observe("cleanup.workerLeaseReleased",lease.LeaseReleased ? "Yes" : "No");
                row.Observe("cleanup.workerJoined",lease.Joined ? "Yes" : "No");
            }
        }
        [Test] [Timeout(180000)]
        public void AC004_FreshCloseNeverFallsBackToOldOrCancelSlot()
        {
            var row=BeginEvidence("edit_AC004_FreshCloseNeverFallsBackToOldOrCancelSlot","FullBridge","C4","AfterFreshPrepare",21,
                new[]{"REQ-M5D7QC4-004","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-004","AC-M5D7QC4-008"});
            AssertActualFreshPartialFailure(row,21);
        }
        [TestCase(21)] [TestCase(22)] [TestCase(23)] [TestCase(24)] [TestCase(25)] [Timeout(180000)]
        public void AC008_FreshOwnerPartialPublicationFailureClosesBothCohorts(int checkpoint)
        {
            var row=BeginEvidence("edit_AC008_FreshOwnerPartialPublicationFailureClosesBothCohorts_"+checkpoint,"FullBridge","C4",Enum.GetName(typeof(ProfileResetExecutionCheckpointV1),checkpoint),checkpoint,
                new[]{"REQ-M5D7QC4-004","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-008"});
            AssertActualFreshPartialFailure(row,checkpoint);
        }
        [Test] [Timeout(180000)]
        public void AC004_ConsumedOrForeignHandbackCannotRebind()
        {
            var originalRow=BeginEvidence("edit_AC004_ConsumedOrForeignHandbackCannotRebind_original","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-004"},new[]{"AC-M5D7QC4-004"});
            var otherRootRow=BeginEvidence("edit_AC004_ConsumedOrForeignHandbackCannotRebind_otherRoot","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-004"},new[]{"AC-M5D7QC4-004"});
            using (var a=C4ActualExecutionEditFixtureV1.Create())
            using (var b=C4ActualExecutionEditFixtureV1.Create())
            {
                File.AppendAllText(Path.Combine(a.Root,"profile.json"),"\n");
                var result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(a.Confirmed,a.Adapter,a.Router);
                Assert.That(() => b.AcceptFresh(result),Throws.InvalidOperationException);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionAvailable(b.Confirmed,b.Adapter,b.Router,b.Owner),Is.True);
                a.AcceptFresh(result); originalRow.Reached();
                Assert.That(() => a.AcceptFresh(result),Throws.InvalidOperationException);
                var otherResult=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(b.Confirmed,b.Adapter,b.Router);
                Assert.That(otherResult.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed)); otherRootRow.Reached();
                originalRow.Observe("diagnostic.foreignHandbackRejected","Yes");
                originalRow.Observe("diagnostic.consumedHandbackReplayRejected","Yes");
                a.ObserveActualResult(originalRow,result); a.ObserveActualBarrier(originalRow,result); a.ObserveNoOpCallCounts(originalRow,result);
                b.ObserveActualResult(otherRootRow,otherResult); b.ObserveActualBarrier(otherRootRow,otherResult); b.ObserveNoOpCallCounts(otherRootRow,otherResult);
                a.Dispose(); a.ObserveActualCleanup(originalRow);
                b.Dispose(); b.ObserveActualCleanup(otherRootRow);
            }
        }
        [Test] [Timeout(180000)]
        public void AC001_ActualCommittedOpaqueStartsExactlyOnce()
        {
            var row=BeginEvidence("edit_AC001_ActualCommittedOpaqueStartsExactlyOnce","FullBridge","None",null,null,new[]{"REQ-M5D7QC4-001"},new[]{"AC-M5D7QC4-001"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var counts=new ExecutionCounts();
                var result=f.Execute(counts.Control,C4ActualExecutionEditFixtureV1.DiskControl((name,value) => { }));
                row.Reached();
                result.Validate();
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(result.Phase,Is.EqualTo(ProfileResetExecutionPhaseV1.Completed));
                Assert.That(result.ExecutionGeneration,Is.GreaterThan(0));
                Assert.That(counts.C1,Is.EqualTo(1)); Assert.That(counts.C2,Is.EqualTo(1));
                Assert.That(counts.PreparedObserver,Is.EqualTo(1)); Assert.That(counts.C2Observer,Is.EqualTo(1));
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router),Throws.InvalidOperationException);
                Assert.That(counts.C1,Is.EqualTo(1)); Assert.That(counts.C2,Is.EqualTo(1));
                result.Validate(); Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                ObserveTypedResult(f,row,result,counts);
            }
        }
        [Test] [Timeout(180000)]
        public void AC001_UnregisteredCandidateCannotStart()
        {
            var row=BeginEvidence("edit_AC001_UnregisteredCandidateCannotStart","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-001"},new[]{"AC-M5D7QC4-001"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(new ConfirmedProfileResetRequestV1(),f.Adapter,f.Router),Throws.InvalidOperationException);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionAvailable(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                var result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                row.Reached(); row.Observe("diagnostic.unregisteredCandidateRejected","Yes");
                f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result); f.ObserveNoOpCallCounts(row,result);
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        [Test] [Timeout(180000)]
        public void AC001_UncommittedActualRequestCannotStart()
        {
            var row=BeginEvidence("edit_AC001_UncommittedActualRequestCannotStart","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-001"},new[]{"AC-M5D7QC4-001"});
            using (var f=C4ActualExecutionEditFixtureV1.Create(false))
            {
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router),Throws.InvalidOperationException);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                var committed=C4ActualExecutionEditFixtureV1.Invoke(f.Owner,C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerV1"),"CommitForExecution",typeof(ConfirmedProfileResetRequestV1),new[] {typeof(ConfirmedProfileResetRequestV1)},f.Confirmed);
                Assert.That(committed,Is.SameAs(f.Confirmed));
                f.CaptureActualCommitBaselineAfterUncommittedStart();
                var result=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                row.Reached(); row.Observe("diagnostic.uncommittedRequestRejected","Yes");
                f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result); f.ObserveNoOpCallCounts(row,result);
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        [Test] [Timeout(180000)]
        public void AC001_ActualForeignRootPairGenerationCannotStart()
        {
            var aRow=BeginEvidence("edit_AC001_ActualForeignRootPairGenerationCannotStart_original","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-001"},new[]{"AC-M5D7QC4-001"});
            var bRow=BeginEvidence("edit_AC001_ActualForeignRootPairGenerationCannotStart_otherRoot","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-001"},new[]{"AC-M5D7QC4-001"});
            using (var a=C4ActualExecutionEditFixtureV1.Create())
            using (var b=C4ActualExecutionEditFixtureV1.Create())
            {
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(a.Confirmed,b.Adapter,a.Router),Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.ExecuteConfirmedReset(a.Confirmed,a.Adapter,b.Router),Throws.InvalidOperationException);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionAvailable(a.Confirmed,a.Adapter,a.Router,b.Owner),Is.False);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionAvailable(a.Confirmed,a.Adapter,a.Router,null),Is.False);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionAvailable(a.Confirmed,a.Adapter,a.Router,a.Owner),Is.True);
                var aResult=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(a.Confirmed,a.Adapter,a.Router);
                var bResult=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(b.Confirmed,b.Adapter,b.Router);
                Assert.That(aResult.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(bResult.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                aRow.Reached(); bRow.Reached();
                aRow.Observe("diagnostic.foreignAdapterRejected","Yes"); aRow.Observe("diagnostic.foreignRouterRejected","Yes");
                aRow.Observe("diagnostic.foreignOwnerRejected","Yes"); aRow.Observe("diagnostic.missingOwnerRejected","Yes");
                a.ObserveActualResult(aRow,aResult); a.ObserveActualBarrier(aRow,aResult); a.ObserveNoOpCallCounts(aRow,aResult);
                b.ObserveActualResult(bRow,bResult); b.ObserveActualBarrier(bRow,bResult); b.ObserveNoOpCallCounts(bRow,bResult);
                a.Dispose(); a.ObserveActualCleanup(aRow); b.Dispose(); b.ObserveActualCleanup(bRow);
            }
        }
        [Test] [Timeout(180000)]
        public void AC001_ActualWrongThreadFirstCallRejectsBeforeUnityAndConsume()
        {
            var row=BeginEvidence("edit_AC001_ActualWrongThreadFirstCallRejectsBeforeUnityAndConsume","ActualWorker","None",null,null,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-001","AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var lifecycle=f.ReadRequestLifecycle(); var issue=f.ReadOriginalRequestIssue();
                var originalHead=C4ActualExecutionEditFixtureV1.Field(lifecycle,lifecycle.GetType(),"Head",issue.GetType());
                Exception failure=null; Exception rejectedFailure=null; var rejected=false; var joined=false;
                var worker=new Thread(() =>
                {
                    try
                    {
                        Assert.That(ProfileNewGameConfirmationV1.IsExecutionAvailable(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                        try { ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router); }
                        catch (InvalidOperationException error) { rejected=true; rejectedFailure=error; }
                    }
                    catch (Exception error) { failure=error; }
                });
                worker.IsBackground=true;
                try { worker.Start(); joined=worker.Join(TimeSpan.FromSeconds(10)); Assert.That(joined,Is.True,"관리 문맥 시험 작업자가 종료되지 않았습니다."); }
                finally { if (worker.IsAlive) { joined=worker.Join(TimeSpan.FromSeconds(10)); Assert.That(joined,Is.True,"작업자 정리에 실패했습니다."); } }
                Assert.That(failure,Is.Null); Assert.That(rejected,Is.True);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionAvailable(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                f.ObservePreEntryProtocolReject(row,rejectedFailure,originalHead,"WrongThread",worker);
                row.Reached(); row.Observe("diagnostic.workerRejectJoined",joined ? "Yes" : "No");
                f.Dispose(); f.ObserveActualCleanup(row); row.Observe("cleanup.workerJoined",joined ? "Yes" : "No");
            }
        }
        [Test] [Timeout(180000)]
        public void AC001_OriginalBirthThreadExecutesSameRequestAfterWrongThreadReject()
        {
            var row=BeginEvidence("edit_AC001_OriginalBirthThreadExecutesSameRequestAfterWrongThreadReject","ActualWorker","None",null,null,
                new[]{"REQ-M5D7QC4-001"},new[]{"AC-M5D7QC4-001"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                Exception failure=null; var rejected=false; var joined=false;
                var worker=new Thread(() => { try { ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router); } catch (InvalidOperationException) { rejected=true; } catch (Exception error) { failure=error; } });
                worker.IsBackground=true;
                try { worker.Start(); joined=worker.Join(TimeSpan.FromSeconds(10)); Assert.That(joined,Is.True); }
                finally { if (worker.IsAlive) { joined=worker.Join(TimeSpan.FromSeconds(10)); Assert.That(joined,Is.True); } }
                Assert.That(failure,Is.Null); Assert.That(rejected,Is.True); Assert.That(joined,Is.True);
                var counts=new ExecutionCounts(); var result=f.Execute(counts.Control,C4ActualExecutionEditFixtureV1.DiskControl((name,value) => { }));
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed)); Assert.That(counts.C1,Is.EqualTo(1)); Assert.That(counts.C2,Is.EqualTo(1));
                row.Reached(); ObserveTypedResult(f,row,result,counts);
                row.Observe("cleanup.workerJoined",joined ? "Yes" : "No");
            }
        }
        // REQ-M5D7QC4-001/006 / AC-M5D7QC4-001/008: 원래 발급 객체의 한 투영만 손상한다.
        [Test] [Timeout(180000)]
        public void AC001_SingleRegisteredWitnessDamageClosesWithoutBegin()
        {
            var row=BeginEvidence("edit_AC001_SingleRegisteredWitnessDamageClosesWithoutBegin","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-001","AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var witness=f.ReadConfirmedWitness(); var counts=new ExecutionCounts();
                C4ActualExecutionEditFixtureV1.DamageOriginalField(witness,"Root",typeof(string),null);
                var failure=Assert.Throws<InvalidOperationException>(()=>f.Execute(counts.Control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{})));
                Assert.That(counts.C1,Is.Zero); Assert.That(counts.C2,Is.Zero);
                Assert.That(f.IsOriginalExecutionRegistered(),Is.True);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                f.ObserveUnreturnedProjectionFailure(row,failure,"Root",counts.C1,counts.C2,counts.PreparedObserver,counts.C2Observer,counts.FreshReserve);
                row.Reached();
                ObserveUnreturnedNoNative(row,"AC001_SingleRegisteredWitnessDamageClosesWithoutBegin");
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        [Test] [Timeout(180000)]
        public void AC001_ActualBirthProjectionDamageClosesOnOriginalThread()
        {
            var row=BeginEvidence("edit_AC001_ActualBirthProjectionDamageClosesOnOriginalThread","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-001","AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                C4ActualExecutionEditFixtureV1.DamageOriginalField(f.ReadConfirmedWitness(),"BirthThreadProjection",typeof(Thread),null);
                var counts=new ExecutionCounts();
                var failure=Assert.Throws<InvalidOperationException>(()=>f.Execute(counts.Control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{})));
                Assert.That(counts.C1,Is.Zero); Assert.That(counts.C2,Is.Zero);
                Assert.That(f.IsOriginalExecutionRegistered(),Is.True);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                f.ObserveUnreturnedProjectionFailure(row,failure,"BirthThreadProjection",counts.C1,counts.C2,counts.PreparedObserver,counts.C2Observer,counts.FreshReserve);
                row.Reached();
                ObserveUnreturnedNoNative(row,"AC001_ActualBirthProjectionDamageClosesOnOriginalThread");
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        [Test] [Timeout(180000)]
        public void AC001_ActualIssuerThreadAnchorDamageRejectsWithoutConsume()
        {
            var row=BeginEvidence("edit_AC001_ActualIssuerThreadAnchorDamageRejectsWithoutConsume","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-001","AC-M5D7QC4-008"});
            AssertTrustedThreadDamage(row,false);
        }
        [Test] [Timeout(180000)]
        public void AC001_ActualIssuerThreadEventDamageRejectsWithoutConsume()
        {
            var row=BeginEvidence("edit_AC001_ActualIssuerThreadEventDamageRejectsWithoutConsume","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-001","AC-M5D7QC4-008"});
            AssertTrustedThreadDamage(row,true);
        }
        private static void AssertTrustedThreadDamage(C4ActualExecutionEditFixtureV1.EvidenceRow row,bool eventDamage)
        {
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var lifecycle=f.ReadRequestLifecycle(); var issued=f.ReadOriginalRequestIssue();
                var head=C4ActualExecutionEditFixtureV1.Field(lifecycle,lifecycle.GetType(),"Head",issued.GetType());
                Assert.That(f.IsOriginalExecutionRegistered(),Is.False);
                C4ActualExecutionEditFixtureV1.DamageOriginalField(eventDamage ? issued : lifecycle,
                    eventDamage ? "IssuerThreadAtIssue" : "IssuerThreadAnchor",typeof(Thread),null);
                var counts=new ExecutionCounts();
                var failure=Assert.Throws<InvalidOperationException>(() => f.Execute(counts.Control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{})));
                Assert.That(counts.C1,Is.Zero); Assert.That(counts.C2,Is.Zero);
                Assert.That(f.IsOriginalExecutionRegistered(),Is.False);
                Assert.That(C4ActualExecutionEditFixtureV1.Field(lifecycle,lifecycle.GetType(),"Head",issued.GetType()),Is.SameAs(head));
                f.ObservePreEntryProtocolReject(row,failure,head,eventDamage ? "IssuerThreadAtIssue" : "IssuerThreadAnchor");
                row.Reached(); f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        [Test] [Timeout(180000)]
        public void AC008_ActualConsumedWorkerRejectsBeforeUnityAccess()
        {
            var row=BeginEvidence("edit_AC008_ActualConsumedWorkerRejectsBeforeUnityAccess","ActualWorker","None",null,null,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var result=f.ExecuteOwner(); var generation=result.ExecutionGeneration;
                Exception failure=null; var rejected=false; var joined=false;
                var worker=new Thread(()=> { try { ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router); }
                    catch (InvalidOperationException) { rejected=true; } catch (Exception error) { failure=error; } });
                worker.IsBackground=true;
                try { worker.Start(); joined=worker.Join(TimeSpan.FromSeconds(10)); Assert.That(joined,Is.True); }
                finally { if (worker.IsAlive) { joined=worker.Join(TimeSpan.FromSeconds(10)); Assert.That(joined,Is.True); } }
                Assert.That(failure,Is.Null); Assert.That(rejected,Is.True);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(result.ExecutionGeneration,Is.EqualTo(generation));
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                Assert.That(joined,Is.True); row.Reached();
                f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result); f.ObserveNoOpCallCounts(row,result);
                f.Dispose(); f.ObserveActualCleanup(row); row.Observe("cleanup.workerJoined",joined ? "Yes" : "No");
            }
        }
        // REQ-M5D7QC4-005 / AC-M5D7QC4-005: 실제 getter 뒤 같은 proof 한 필드만 손상한다.
        [Test] [Timeout(180000)]
        public void AC005_ActualNestedPreparedProofRootNullStopsBeforeFinalize()
        {
            var row=BeginEvidence("edit_AC005_ActualNestedPreparedProofRootNullStopsBeforeFinalize","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-003","REQ-M5D7QC4-005"},new[]{"AC-M5D7QC4-005"});
            AssertActualPreparedProofRootDamage(row,"AC005_ActualNestedPreparedProofRootNullStopsBeforeFinalize");
        }
        private static void AssertActualPreparedProofRootDamage(C4ActualExecutionEditFixtureV1.EvidenceRow row,string exactParent)
        {
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var observed=0; var c1=0; var c2Boundary=0; var c2Observers=0; var freshReserves=0; object observedProof=null; object damagedProof=null;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                { if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1++; if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeC2Finalize) c2Boundary++; if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) freshReserves++; },
                    ()=>{},proof=> { observed++; observedProof=proof; damagedProof=proof; DamageActualField(damagedProof,C4ActualExecutionEditFixtureV1.ProfileType("ProfileResetDiskPreparedProofV1"),"_root",typeof(string),null,true); },memory=>c2Observers++);
                var failure=Assert.Throws<InvalidOperationException>(()=>f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{})));
                Assert.That(observedProof,Is.Not.Null); Assert.That(observed,Is.EqualTo(1)); Assert.That(c1,Is.EqualTo(1)); Assert.That(c2Boundary,Is.Zero);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                if(row!=null)
                {
                    f.ObserveUnreturnedRejectedProof(row,failure,observedProof,damagedProof,observed,c1,c2Boundary,c2Observers,freshReserves);
                    row.Reached(); ObserveUnreturnedNoNative(row,exactParent);
                    f.Dispose(); f.ObserveActualCleanup(row);
                }
            }
        }
        // REQ-M5D7QC4-005 / AC-M5D7QC4-007: 실제 C2 반환 객체를 관측한 뒤 한 outcome을 default로 손상한다.
        [Test] [Timeout(180000)]
        public void AC007_ActualC2OutcomeDefaultRejectsComposition()
        {
            var row=BeginEvidence("edit_AC007_ActualC2OutcomeDefaultRejectsComposition","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-005","REQ-M5D7QC4-007"},new[]{"AC-M5D7QC4-007"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var c1=0; var c2=0; var observed=0; var prepared=0; var freshReserve=0; object actual=null; object observedProof=null;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                { if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1++; if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2++; if(checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) freshReserve++; },
                    ()=>{},proof=>{prepared++;observedProof=proof;},memory=> { observed++; actual=memory;
                        DamageActualField(memory,typeof(ProfileResetMemoryResultV1),"_outcome",typeof(ProfileResetMemoryOutcomeV1),default(ProfileResetMemoryOutcomeV1),true); });
                var failure=Assert.Throws<ArgumentException>(() => f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{})));
                Assert.That(actual,Is.Not.Null); Assert.That(c1,Is.EqualTo(1)); Assert.That(c2,Is.EqualTo(1)); Assert.That(observed,Is.EqualTo(1));
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                Assert.That(() => f.ExecuteOwner(),Throws.InvalidOperationException);
                f.ObserveIssuedExecutionWithoutResult(row,failure,true);
                f.ObserveReceiptWithoutExecutionResult(row,actual,observed,"Rejected");
                f.ObserveNoResultBarrierGuardHistory(row,observedProof,actual);
                row.Reached(); row.Observe("diagnostic.c2DefaultOutcomeRejected","Yes");
                row.Observe("callCounts.c1Begin",c1); row.Observe("callCounts.c2Finalize",c2);
                row.Observe("callCounts.preparedObserver",prepared); row.Observe("callCounts.c2Observer",observed);
                row.Observe("callCounts.freshReserve",freshReserve);
                ObserveUnreturnedNoNative(row,"AC007_ActualC2OutcomeDefaultRejectsComposition");
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        private static void DamageActualField(object actual,Type declaring,string name,Type expected,object negative,bool readOnly)
        {
            Assert.That(actual,Is.Not.Null); Assert.That(actual.GetType(),Is.EqualTo(declaring));
            var field=declaring.GetField(name,BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.NonPublic);
            Assert.That(field,Is.Not.Null); Assert.That(field.DeclaringType,Is.EqualTo(declaring));
            Assert.That(field.FieldType,Is.EqualTo(expected)); Assert.That(field.IsInitOnly,Is.EqualTo(readOnly));
            field.SetValue(actual,negative);
        }
        // REQ-M5D7QC4-003 / AC-M5D7QC4-003: 실제 Begin 구조체의 boxed copy만 순수 음성으로 사용한다.
        [Test] [Timeout(180000)]
        public void AC003_ActualBeginBoxedCopyClosedRowValidatorRejectsDefaultUnknown()
        {
            var resultRow=BeginEvidence("edit_AC003_ActualBeginBoxedCopyClosedRowValidatorRejectsDefaultUnknown_result","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-003"},new[]{"AC-M5D7QC4-003"});
            var validatorRow=BeginEvidence("edit_AC003_ActualBeginBoxedCopyClosedRowValidatorRejectsDefaultUnknown_validator","PureValidator","None",null,null,
                new[]{"REQ-M5D7QC4-003"},new[]{"AC-M5D7QC4-003"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var executed=f.ExecuteOwner(); Assert.That(executed.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                var rejectedCopies=0;
                var originalRecord=f.ReadActualResultRecord(executed);
                var rowType=C4ActualExecutionEditFixtureV1.ProfileType("ProfileResetDiskResultV1");
                var actual=C4ActualExecutionEditFixtureV1.Field(originalRecord,originalRecord.GetType(),"Disk",rowType);
                ValidateActualDiskRow(actual,rowType);
                foreach (var fieldName in new[] {"_outcome","_state"})
                {
                    var field=rowType.GetField(fieldName,BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.NonPublic);
                    Assert.That(field,Is.Not.Null); Assert.That(field.IsInitOnly,Is.True); Assert.That(field.FieldType.IsEnum,Is.True);
                    foreach (var value in new object[] {Enum.ToObject(field.FieldType,0),Enum.ToObject(field.FieldType,999),
                        Enum.Parse(field.FieldType,fieldName == "_outcome" ? "Busy" : "NoBarrier")})
                    {
                        var boxed=RuntimeHelpers.GetObjectValue(actual); Assert.That(boxed,Is.Not.SameAs(actual));
                        field.SetValue(boxed,value);
                        Assert.That(() => ValidateActualDiskRow(boxed,rowType),Throws.InvalidOperationException);
                        rejectedCopies++;
                        ValidateActualDiskRow(actual,rowType);
                    }
                }
                executed.Validate(); Assert.That(executed.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(rejectedCopies,Is.EqualTo(6)); resultRow.Reached(); validatorRow.Reached();
                validatorRow.Observe("diagnostic.boxedValidatorRejections",rejectedCopies); ObserveStructureScope(validatorRow,true);
                f.ObserveActualResult(resultRow,executed); f.ObserveActualBarrier(resultRow,executed); f.ObserveNoOpCallCounts(resultRow,executed);
                f.Dispose(); f.ObserveActualCleanup(resultRow);
            }
        }
        private static void ValidateActualDiskRow(object actual,Type actualType)
        {
            Assert.That(actual.GetType(),Is.EqualTo(actualType));
            InvokeStatic(typeof(ProfileNewGameConfirmationV1),"ValidateExecutionDiskRow",typeof(void),new[] {actualType},actual);
        }
        private static object InvokeStatic(Type declaring,string name,Type result,Type[] parameters,params object[] arguments)
        {
            var method=declaring.GetMethod(name,BindingFlags.DeclaredOnly|BindingFlags.Static|BindingFlags.NonPublic,null,parameters,null);
            Assert.That(method,Is.Not.Null); Assert.That(method.DeclaringType,Is.EqualTo(declaring));
            Assert.That(method.ReturnType,Is.EqualTo(result)); Assert.That(method.IsGenericMethod,Is.False);
            var signature=method.GetParameters(); Assert.That(signature.Length,Is.EqualTo(parameters.Length));
            for (var i=0;i<signature.Length;i++) { Assert.That(signature[i].ParameterType,Is.EqualTo(parameters[i])); Assert.That(signature[i].IsOptional,Is.False); }
            try { return method.Invoke(null,arguments); }
            catch (TargetInvocationException error) { if (error.InnerException != null) throw error.InnerException; throw; }
        }
        // REQ-M5D7QC4-003/004 / AC-M5D7QC4-003/004: 실제 C1의 두 terminal 행은 Finalize/fresh로 진행하지 않는다.
        [TestCase("Manual")] [TestCase("Reload")] [Timeout(180000)]
        public void AC003_ActualTerminalDiskRowNeverCallsFinalize(string terminal)
        { AssertActualTerminalDiskRow(terminal); }
        [TestCase("Manual")] [TestCase("Reload")] [Timeout(180000)]
        public void AC004_TerminalDiskRowsNeverIssueHandback(string terminal)
        { AssertActualTerminalDiskRow(terminal); }
        private void AssertActualTerminalDiskRow(string terminal)
        {
            var target=terminal == "Manual" ? 10 : 33;
            var name=Enum.GetName(C4ActualExecutionEditFixtureV1.ProfileType("ProfileResetDiskCheckpointV1"),target);
            var parent=TestContext.CurrentContext.Test.MethodName;
            var row=BeginEvidence("edit_"+parent+"_"+terminal,"FullBridge","C1",name,target,
                new[]{"REQ-M5D7QC4-003","REQ-M5D7QC4-004"},new[]{"AC-M5D7QC4-003","AC-M5D7QC4-004"});
            using (var f=C4ActualExecutionEditFixtureV1.Create(true,true))
            {
                var hit=0; var counts=new ExecutionCounts();
                var result=f.Execute(counts.Control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>
                { if (value == target) { hit++; row.Reached(); throw new IOException("실제 C1 terminal 종료 지점"); } }));
                Assert.That(hit,Is.EqualTo(1)); Assert.That(counts.C1,Is.EqualTo(1)); Assert.That(counts.C2,Is.Zero);
                Assert.That(counts.PreparedObserver,Is.Zero); Assert.That(counts.C2Observer,Is.Zero);
                Assert.That(result.Outcome,Is.EqualTo(terminal == "Manual" ? ProfileResetExecutionOutcomeV1.ManualRepairRequired : ProfileResetExecutionOutcomeV1.ReloadRequired));
                Assert.That(result.Phase,Is.EqualTo(ProfileResetExecutionPhaseV1.C1Returned));
                Assert.That(() => f.AcceptFresh(result),Throws.InvalidOperationException);
                Assert.That(() => f.ExecuteOwner(),Throws.InvalidOperationException);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                result.Validate();
                f.ObserveActualResult(row,result);f.ObserveActualBarrier(row,result);
                row.Observe("callCounts.c1Begin",counts.C1);row.Observe("callCounts.c2Finalize",counts.C2);
                row.Observe("callCounts.preparedObserver",counts.PreparedObserver);row.Observe("callCounts.c2Observer",counts.C2Observer);row.Observe("callCounts.freshReserve",counts.FreshReserve);
                f.ObserveAbsentFreshCompletion(row,result);
                const string testPath="Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs";
                var lines=File.ReadAllLines(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath),testPath));var declaration="public void "+parent+"(";var declarationLine=0;
                for(var i=0;i<lines.Length;i++)if(lines[i].TrimStart().StartsWith(declaration,StringComparison.Ordinal)){Assert.That(declarationLine,Is.Zero);declarationLine=i+1;}
                Assert.That(declarationLine,Is.GreaterThan(0));
                C4ActualExecutionEditFixtureV1.ObserveNoNativeObserverRequired(row,"source:"+testPath+":"+declarationLine);
                f.Dispose();f.ObserveActualCleanup(row);
            }
        }
        // REQ-M5D7QC4-002 / AC-M5D7QC4-002/006: 실제 after-update가 pending guard를 풀지 못한다.
        [Test] [Timeout(180000)]
        public void AC002_QuarantineReleaseCannotReopenPendingGuard()
        {
            var row=BeginEvidence("edit_AC002_QuarantineReleaseCannotReopenPendingGuard","FullBridge","C4","AfterRouterGuard",6,
                new[]{"REQ-M5D7QC4-002","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-002","AC-M5D7QC4-006"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var reached=0; var counts=new ExecutionCounts();
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                {
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) counts.C1++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) counts.C2++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) counts.FreshReserve++;
                    if (checkpoint != ProfileResetExecutionCheckpointV1.AfterRouterGuard) return;
                    reached++; row.Reached();
                    Assert.That(ProfileNewGameConfirmationV1.IsExecutionGuardedPair(f.Adapter,f.Router),Is.True);
                    var receipt=f.Router.CurrentReceipt; var frame=f.Router.CurrentUiFrame;
                    UnityEngine.InputSystem.InputSystem.Update();
                    Assert.That(ProfileNewGameConfirmationV1.IsExecutionGuardedPair(f.Adapter,f.Router),Is.True);
                    Assert.That(f.Router.CurrentReceipt,Is.EqualTo(receipt)); Assert.That(f.Router.CurrentUiFrame,Is.EqualTo(frame));
                    Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                    Assert.That(f.Adapter.TryTakeNotification(out _),Is.False);
                },()=>{},proof=>counts.PreparedObserver++,memory=>counts.C2Observer++);
                var result=f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(reached,Is.EqualTo(1));
                Assert.That(counts.C1,Is.EqualTo(1)); Assert.That(counts.C2,Is.EqualTo(1));
                ObserveTypedResult(f,row,result,counts);
            }
        }
        [Test] [Timeout(180000)]
        public void AC006_ThrowAfterRealBeginBeforeFinalizeKeepsPairClosed()
        {
            var row=BeginEvidence("edit_AC006_ThrowAfterRealBeginBeforeFinalizeKeepsPairClosed","FullBridge","C4","AfterC1Begin",8,
                new[]{"REQ-M5D7QC4-002","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-006"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var returned=0; var finalize=0; Exception callbackFailure=null;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                {
                    if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeC2Finalize) finalize++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) { returned++; callbackFailure=new IOException("실제 Begin 반환 직후 종료"); throw callbackFailure; }
                },()=>{},proof=>{},memory=>{});
                var failure=Assert.Throws<IOException>(() => f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{})));
                Assert.That(returned,Is.EqualTo(1)); Assert.That(finalize,Is.Zero);
                Assert.That(File.Exists(Path.Combine(f.Root,"profile.reset.json")),Is.True);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                Assert.That(() => f.Router.StepForTests(),Throws.InvalidOperationException);
                Assert.That(() => f.ExecuteOwner(),Throws.InvalidOperationException);
                f.ObservePreC2Failure(row,failure,callbackFailure,ProfileResetExecutionCheckpointV1.AfterC1Begin,returned,returned,finalize,0,0,0);
                row.Reached(); ObserveUnreturnedNoNative(row,"AC006_ThrowAfterRealBeginBeforeFinalizeKeepsPairClosed");
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        // REQ-M5D7QC4-001/006 / AC-M5D7QC4-008: 실제 원래 thread만 승리하고 worker는 native 이전 거절된다.
        [Test] [Timeout(180000)]
        public void AC008_ConcurrentActualExecutorHasOneWinner()
        {
            var row=BeginEvidence("edit_AC008_ConcurrentActualExecutorHasOneWinner","ActualWorker","C4","AfterConfirmedConsume",2,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var losers=0; var c1=0; var c2=0; var prepared=0; var memory=0; var reserve=0; var joined=false; Exception unexpected=null;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                {
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) reserve++;
                    if (checkpoint != ProfileResetExecutionCheckpointV1.AfterConfirmedConsume) return;
                    var worker=new Thread(()=> { try { ProfileNewGameConfirmationV1.ExecuteConfirmedReset(f.Confirmed,f.Adapter,f.Router); }
                        catch (InvalidOperationException) { Interlocked.Increment(ref losers); } catch (Exception error) { unexpected=error; } });
                    worker.IsBackground=true;
                    try { worker.Start(); joined=worker.Join(TimeSpan.FromSeconds(10)); Assert.That(joined,Is.True); }
                    finally { if (worker.IsAlive) { joined=worker.Join(TimeSpan.FromSeconds(10)); Assert.That(joined,Is.True); } }
                    row.Reached();
                },()=>{},proof=>prepared++,actualMemory=>memory++);
                var result=f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                Assert.That(unexpected,Is.Null); Assert.That(losers,Is.EqualTo(1));
                Assert.That(c1,Is.EqualTo(1)); Assert.That(c2,Is.EqualTo(1)); Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                Assert.That(joined,Is.True);
                f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result);
                row.Observe("callCounts.c1Begin",c1); row.Observe("callCounts.c2Finalize",c2);
                row.Observe("callCounts.preparedObserver",prepared); row.Observe("callCounts.c2Observer",memory); row.Observe("callCounts.freshReserve",reserve);
                f.ObserveAbsentFreshCompletion(row,result);
                const string testPath="Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs";
                var lines=File.ReadAllLines(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath),testPath));var declarationLine=0;
                for(var i=0;i<lines.Length;i++)if(lines[i].TrimStart().StartsWith("public void AC008_ConcurrentActualExecutorHasOneWinner(",StringComparison.Ordinal)){Assert.That(declarationLine,Is.Zero);declarationLine=i+1;}
                Assert.That(declarationLine,Is.GreaterThan(0));
                C4ActualExecutionEditFixtureV1.ObserveNoNativeObserverRequired(row,"source:"+testPath+":"+declarationLine);
                f.Dispose(); f.ObserveActualCleanup(row); row.Observe("cleanup.workerJoined",joined ? "Yes" : "No");
            }
        }
        [Test] [Timeout(180000)]
        public void AC008_ConcurrentFreshReserveHasOneWinner()
        {
            var row=BeginEvidence("edit_AC008_ConcurrentFreshReserveHasOneWinner","ActualWorker","C4","BeforeFreshReserve",19,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-004"},new[]{"AC-M5D7QC4-008"});
            AssertActualConcurrentFreshOwner(row);
        }
        [Test] [Timeout(180000)]
        public void AC008_ConcurrentSameHandbackHasOneOwnerTransition()
        {
            var row=BeginEvidence("edit_AC008_ConcurrentSameHandbackHasOneOwnerTransition","ActualWorker","C4","BeforeFreshReserve",19,
                new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-004"},new[]{"AC-M5D7QC4-008"});
            AssertActualConcurrentFreshOwner(row);
        }
        private static void AssertActualConcurrentFreshOwner(C4ActualExecutionEditFixtureV1.EvidenceRow row)
        {
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var losers=0; var consumed=0; var reserve=0; var c1=0; var c2=0; var prepared=0; var memory=0; var joined=false;
                Exception unexpected=null;
                ProfileResetExecutionResultV1 result=null;
                ProfileResetExecutionResultV1 workerHandback=null;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                {
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterFreshConsume) consumed++;
                    if (checkpoint != ProfileResetExecutionCheckpointV1.BeforeFreshReserve) return;
                    reserve++; workerHandback=result; Assert.That(workerHandback,Is.Not.Null);
                    var worker=new Thread(()=> { try { f.AcceptFresh(workerHandback); }
                        catch (InvalidOperationException) { Interlocked.Increment(ref losers); } catch (Exception error) { unexpected=error; } });
                    worker.IsBackground=true;
                    try { worker.Start(); joined=worker.Join(TimeSpan.FromSeconds(10)); Assert.That(joined,Is.True); }
                    finally { if (worker.IsAlive) { joined=worker.Join(TimeSpan.FromSeconds(10)); Assert.That(joined,Is.True); } }
                    row.Reached();
                },()=>{},proof=>prepared++,actualMemory=>memory++);
                var lease=new C4ActualExecutionEditFixtureV1.RootLeaseWorker(f.Root);
                using (lease)
                    result=f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Busy));
                f.AcceptFresh(result);
                Assert.That(workerHandback,Is.SameAs(result)); Assert.That(joined,Is.True);
                Assert.That(unexpected,Is.Null); Assert.That(losers,Is.EqualTo(1)); Assert.That(consumed,Is.EqualTo(1));
                var ownerType=C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerV1");
                var winnerEpoch=C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epoch",typeof(long));
                var winnerToken=C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epochToken",typeof(object));
                Assert.That(() => f.AcceptFresh(result),Throws.InvalidOperationException);
                Assert.That(C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epoch",typeof(long)),Is.EqualTo(winnerEpoch));
                Assert.That(C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epochToken",typeof(object)),Is.SameAs(winnerToken));
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                Assert.That(c1,Is.EqualTo(1)); Assert.That(c2,Is.Zero);
                f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result,lease);
                row.Observe("callCounts.c1Begin",c1); row.Observe("callCounts.c2Finalize",c2);
                row.Observe("callCounts.freshReserve",reserve); row.Observe("callCounts.freshComplete",consumed);
                row.Observe("callCounts.preparedObserver",prepared); row.Observe("callCounts.c2Observer",memory);
                row.Observe("diagnostic.sameHandbackReference",ReferenceEquals(workerHandback,result) ? "SameActual" : "Different");
                const string testPath="Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs";
                var lines=File.ReadAllLines(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath),testPath));var declaration="public void "+TestContext.CurrentContext.Test.MethodName+"(";var declarationLine=0;
                for(var i=0;i<lines.Length;i++)if(lines[i].TrimStart().StartsWith(declaration,StringComparison.Ordinal)){Assert.That(declarationLine,Is.Zero);declarationLine=i+1;}
                Assert.That(declarationLine,Is.GreaterThan(0));
                C4ActualExecutionEditFixtureV1.ObserveNoNativeObserverRequired(row,"source:"+testPath+":"+declarationLine);
                f.Dispose(); f.ObserveActualCleanup(row);
                row.Observe("cleanup.releaseSignaled",lease.ReleaseSignaled ? "Yes" : "No");
                row.Observe("cleanup.workerLeaseReleased",lease.LeaseReleased ? "Yes" : "No");
                row.Observe("cleanup.workerJoined",joined && lease.Joined ? "Yes" : "No");
            }
        }
        [Test] [Timeout(180000)]
        public void AC008_CompletedHistoryCannotAuthorizeSecondExecution()
        {
            var row=BeginEvidence("edit_AC008_CompletedHistoryCannotAuthorizeSecondExecution","FullBridge","None",null,null,new[]{"REQ-M5D7QC4-001","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var counts=new ExecutionCounts(); var result=f.Execute(counts.Control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                row.Reached();
                var original=f.ReadActualResultRecord(result); var generation=result.ExecutionGeneration;
                Assert.That(() => f.ExecuteOwner(),Throws.InvalidOperationException);
                Assert.That(f.ReadActualResultRecord(result),Is.SameAs(original));
                Assert.That(result.ExecutionGeneration,Is.EqualTo(generation));
                Assert.That(counts.C1,Is.EqualTo(1)); Assert.That(counts.C2,Is.EqualTo(1));
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.False);
                ObserveTypedResult(f,row,result,counts);
            }
        }
        // REQ-M5D7QC4-005 / AC-M5D7QC4-007: 같은 실제 C2 결과·receipt의 CWT 결속을 보존한다.
        [Test] [Timeout(180000)]
        public void AC007_ActualC2CompletedCarriesExactReceipt()
        {
            var evidence=BeginEvidence("edit_AC007_ActualC2CompletedCarriesExactReceipt","FullBridge","None",null,null,new[]{"REQ-M5D7QC4-005"},new[]{"AC-M5D7QC4-007"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                ProfileResetMemoryResultV1 actual=null; var observed=0; var c2=0; var counts=new ExecutionCounts();
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                { if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) counts.C1++; if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) { c2++; counts.C2++; } if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) counts.FreshReserve++; },()=>{},proof=>counts.PreparedObserver++,memory=>
                { observed++; counts.C2Observer++; evidence.Reached(); actual=(ProfileResetMemoryResultV1)memory; actual.Validate(); });
                var result=f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                var row=f.ReadActualResultRecord(result);
                Assert.That(C4ActualExecutionEditFixtureV1.Field(row,row.GetType(),"Memory",typeof(ProfileResetMemoryResultV1)),Is.SameAs(actual));
                var receipt=(ProfileResetMemoryReceiptV1)C4ActualExecutionEditFixtureV1.Field(row,row.GetType(),"Receipt",typeof(ProfileResetMemoryReceiptV1));
                Assert.That(receipt,Is.SameAs(actual.Receipt)); receipt.Validate();
                Assert.That(receipt.Root,Is.EqualTo(f.Root)); Assert.That(receipt.Generation,Is.EqualTo(f.Adapter.CurrentResetSession.Generation));
                Assert.That(receipt.UiOnlyBlocked,Is.True); Assert.That(observed,Is.EqualTo(1)); Assert.That(c2,Is.EqualTo(1));
                Assert.That(() => f.ExecuteOwner(),Throws.InvalidOperationException);
                Assert.That(f.ReadActualResultRecord(result),Is.SameAs(row)); Assert.That(actual.Receipt,Is.SameAs(receipt));
                ObserveTypedResult(f,evidence,result,counts);
            }
        }
        // REQ-M5D7QC4-005 / AC-M5D7QC4-005: 승인된 실제 getter 뒤의 동일 단일 root 손상 범위만 사용한다.
        [Test] [Timeout(180000)]
        public void AC005_ActualProofPairRootDamageStopsBeforeCutover()
        {
            var row=BeginEvidence("edit_AC005_ActualProofPairRootDamageStopsBeforeCutover","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-003","REQ-M5D7QC4-005"},new[]{"AC-M5D7QC4-005"});
            AssertActualPreparedProofRootDamage(row,"AC005_ActualProofPairRootDamageStopsBeforeCutover");
        }
        // REQ-M5D7QC4-001/006 / AC-M5D7QC4-008: 실제 등록 뒤 checkpoint 예외는 원래 cohort를 영구 폐쇄한다.
        [Test] [Timeout(180000)]
        public void AC008_PartialRegistrationExceptionClosesOriginalPair()
        {
            var row=BeginEvidence("edit_AC008_PartialRegistrationExceptionClosesOriginalPair","FullBridge","C4","AfterConfirmedConsume",2,
                new[] {"REQ-M5D7QC4-001","REQ-M5D7QC4-006"},new[] {"AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var reached=0; var c1=0; var c2=0; var reserve=0; var complete=0; var prepared=0; var memory=0; Exception failure=null; Exception callbackFailure=null;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                {
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) c1++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) c2++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) reserve++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterFreshConsume) complete++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterConfirmedConsume)
                    { reached++; Assert.That(f.IsOriginalExecutionRegistered(),Is.True); row.Reached(); callbackFailure=new IOException("실제 C4 등록 직후 종료 시험"); throw callbackFailure; }
                },()=>{},proof=>{prepared++;},actualMemory=>{memory++;});
                Assert.That(()=>{try {f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));}catch(Exception error){failure=error;throw;}},Throws.TypeOf<IOException>());
                Assert.That(reached,Is.EqualTo(1)); Assert.That(c1,Is.Zero); Assert.That(c2,Is.Zero);
                Assert.That(f.IsOriginalExecutionRegistered(),Is.True);
                Assert.That(ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner),Is.True);
                Assert.That(f.Adapter.IsResetCutoverTerminal,Is.True);
                Assert.That(()=>f.ExecuteOwner(),Throws.InvalidOperationException);
                f.ObservePreC1Failure(row,failure,callbackFailure,ProfileResetExecutionCheckpointV1.AfterConfirmedConsume,reached,c1,c2,prepared,memory,reserve);
                row.Observe("guard.permanentFaultRecorded",ProfileNewGameConfirmationV1.IsExecutionFaulted(f.Confirmed,f.Adapter,f.Router,f.Owner) ? "Yes" : "No");
                row.Observe("callCounts.c1Begin",c1); row.Observe("callCounts.c2Finalize",c2); row.Observe("callCounts.freshReserve",reserve); row.Observe("callCounts.freshComplete",complete);
                row.Observe("callCounts.preparedObserver",prepared); row.Observe("callCounts.c2Observer",memory);
                const string testPath="Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs";
                var lines=File.ReadAllLines(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath),testPath));var declarationLine=0;
                for(var i=0;i<lines.Length;i++)if(lines[i].TrimStart().StartsWith("public void AC008_PartialRegistrationExceptionClosesOriginalPair(",StringComparison.Ordinal)){Assert.That(declarationLine,Is.Zero);declarationLine=i+1;}
                Assert.That(declarationLine,Is.GreaterThan(0));
                C4ActualExecutionEditFixtureV1.ObserveNoNativeObserverRequired(row,"source:"+testPath+":"+declarationLine);
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        // REQ-M5D7QC4-002/006 / AC-M5D7QC4-006: 실제 prepared barrier에서 정상 public writer를 시도한다.
        [Test] [Timeout(180000)]
        public void AC006_ActualOrdinaryWriterCannotCrossPreparedBarrier()
        {
            const string evidenceId="edit_AC006_ActualOrdinaryWriterCannotCrossPreparedBarrier";
            var row=BeginEvidence(evidenceId,"ActualCallback","None",null,null,new[]{"REQ-M5D7QC4-002","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-006"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var attempted=0; var c1=0; var c2=0; var prepared=0; var memory=0; var reserve=0;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                { if(checkpoint==ProfileResetExecutionCheckpointV1.AfterC1Begin)c1++;if(checkpoint==ProfileResetExecutionCheckpointV1.AfterC2Finalize)c2++;if(checkpoint==ProfileResetExecutionCheckpointV1.BeforeFreshReserve)reserve++; },()=>
                {
                    attempted++;
                    var marker=Path.Combine(f.Root,"profile.reset.json");
                    var primary=Path.Combine(f.Root,"profile.json");
                    Assert.That(File.Exists(marker),Is.True);
                    var markerBytes=File.ReadAllBytes(marker); var primaryBytes=File.ReadAllBytes(primary);
                    Assert.That(()=>ProfileAtomicSaveServiceV1.Save(f.Root,ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument),
                        Throws.TypeOf<ProfileRootOperationBlockedExceptionV1>());
                    CollectionAssert.AreEqual(markerBytes,File.ReadAllBytes(marker));
                    CollectionAssert.AreEqual(primaryBytes,File.ReadAllBytes(primary));
                    Assert.That(File.Exists(Path.Combine(f.Root,"profile.tmp.json")),Is.False);
                    Assert.That(ProfileNewGameConfirmationV1.IsExecutionGuardedPair(f.Adapter,f.Router),Is.True);
                    row.Reached();
                    row.Observe("diagnostic.actualOrdinaryWriterBlocked","Yes");
                },proof=>prepared++,actualMemory=>memory++);
                var result=f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                Assert.That(attempted,Is.EqualTo(1)); Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result);
                var pointer="case:"+TestContext.CurrentContext.Test.FullName+"#row:"+evidenceId+"#fact:diagnostic.actualOrdinaryWriterBlocked";
                row.Observe("barrier.ordinaryWriterBlocked","Yes"); row.Observe("barrier.evidenceReference",pointer);
                row.Observe("diagnostic.barrierEvidenceKind","ActualOrdinaryWriterObservation");
                row.Observe("callCounts.c1Begin",c1);row.Observe("callCounts.c2Finalize",c2);
                row.Observe("callCounts.preparedObserver",prepared);row.Observe("callCounts.c2Observer",memory);row.Observe("callCounts.freshReserve",reserve);
                f.ObserveAbsentFreshCompletion(row,result);
                const string testPath="Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileResetExecutionBridgeV1Tests.cs";
                var lines=File.ReadAllLines(Path.Combine(Path.GetDirectoryName(UnityEngine.Application.dataPath),testPath));var declarationLine=0;
                for(var i=0;i<lines.Length;i++)if(lines[i].TrimStart().StartsWith("public void AC006_ActualOrdinaryWriterCannotCrossPreparedBarrier(",StringComparison.Ordinal)){Assert.That(declarationLine,Is.Zero);declarationLine=i+1;}
                Assert.That(declarationLine,Is.GreaterThan(0));
                C4ActualExecutionEditFixtureV1.ObserveNoNativeObserverRequired(row,"source:"+testPath+":"+declarationLine);
                f.Dispose(); f.ObserveActualCleanup(row);
            }
        }
        // REQ-M5D7QC4-003/005 / AC-M5D7QC4-003: 현재 제품 본문의 두 검증 위치와 단일 Finalize 분기를 직접 대조한다.
        [Test] [Timeout(180000)]
        public void AC003_ProductionValidatorDominatesEveryProofAndFinalizeBranch()
        {
            var row=BeginEvidence("edit_AC003_ProductionValidatorDominatesEveryProofAndFinalizeBranch","Structure","None",null,null,new[]{"REQ-M5D7QC4-003","REQ-M5D7QC4-005"},new[]{"AC-M5D7QC4-003"});
            var path=Path.Combine(UnityEngine.Application.dataPath,"AcadeGameMaker","Runtime","Input","Unity","ProfileResetExecutionBridgeV1.cs");
            var source=File.ReadAllText(path);
            var start=source.IndexOf("internal static ProfileResetExecutionResultV1 ExecuteConfirmedReset(ConfirmedProfileResetRequestV1 confirmed,DesktopProfileLaunchAdapterV1 adapter,InputRouter router,",StringComparison.Ordinal);
            var end=source.IndexOf("internal static void ValidateExecutionDiskRow(ProfileResetDiskResultV1 actual)",StringComparison.Ordinal);
            Assert.That(start,Is.GreaterThanOrEqualTo(0)); Assert.That(end,Is.GreaterThan(start));
            var body=source.Substring(start,end-start);
            var first=body.IndexOf("ValidateExecutionDiskRow(disk);",StringComparison.Ordinal);
            var branch=body.IndexOf("if (disk.Outcome == ProfileResetDiskOutcomeV1.DiskPrepared)",StringComparison.Ordinal);
            var getter=body.IndexOf("var proof=disk.PreparedProof;",StringComparison.Ordinal);
            var observer=body.IndexOf("control.ObserveActualPreparedProofBeforeRowValidation(proof);",StringComparison.Ordinal);
            var second=body.IndexOf("ValidateExecutionDiskRow(disk);",first+1,StringComparison.Ordinal);
            var prepared=body.IndexOf("control.PostDiskPreparedValidatedPreC2();",StringComparison.Ordinal);
            var finalize=body.IndexOf("memory=ProfileResetMemoryCutoverV1.FinalizeReset(execution.Root,proof,adapter,router,memoryControl);",StringComparison.Ordinal);
            Assert.That(first,Is.GreaterThanOrEqualTo(0)); Assert.That(branch,Is.GreaterThan(first));
            Assert.That(getter,Is.GreaterThan(branch)); Assert.That(observer,Is.GreaterThan(getter));
            Assert.That(second,Is.GreaterThan(observer)); Assert.That(prepared,Is.GreaterThan(second)); Assert.That(finalize,Is.GreaterThan(prepared));
            Assert.That(body.IndexOf("ValidateExecutionDiskRow(disk);",second+1,StringComparison.Ordinal),Is.EqualTo(-1));
            Assert.That(body.IndexOf(".PreparedProof",getter+"var proof=disk.PreparedProof;".Length,StringComparison.Ordinal),Is.EqualTo(-1));
            Assert.That(body.IndexOf("ProfileResetMemoryCutoverV1.FinalizeReset(",finalize+"memory=ProfileResetMemoryCutoverV1.FinalizeReset(execution.Root,proof,adapter,router,memoryControl);".Length,StringComparison.Ordinal),Is.EqualTo(-1));
            StringAssert.Contains("internal static void ValidateExecutionDiskRow(ProfileResetDiskResultV1 actual) { actual.Validate(); }",source);
            row.Reached(); ObserveStructureScope(row);
        }
        // REQ-M5D7QC4-004 / AC-M5D7QC4-004: 같은 실제 owner가 Busy/Stale 이후 새 epoch에서 정상 발급한다.
        [Test] [Timeout(180000)]
        public void AC004_ActualBusySameOwnerPublishesPristineNextEpoch()
        { AssertActualFreshOwner(false); }
        [Test] [Timeout(180000)]
        public void AC004_ActualStaleSameOwnerPublishesPristineNextEpoch()
        { AssertActualFreshOwner(true); }
        [Test] [Timeout(180000)]
        public void AC004_FreshSlotUsesActualNewIntentWithoutLegacyTakeReset()
        { AssertActualFreshOwner(false); }
        [Test] [Timeout(180000)]
        public void AC004_OldCommittedHandleRejectsAfterFreshWhileNewHandleSucceeds()
        { AssertActualFreshOwner(false); }
        [Test] [Timeout(180000)]
        public void AC004_NewActualHandleConfirmsWithNextDecisionGeneration()
        { AssertActualFreshOwner(false); }
        [Test] [Timeout(180000)]
        public void AC008_FreshOwnerHistoryPreservesOldCommitAndAllCapabilityReferences()
        { AssertActualFreshOwner(false); }
        private void AssertActualFreshOwner(bool stale)
        {
            var parent=TestContext.CurrentContext.Test.MethodName;
            var originalId="edit_"+parent+"_original";
            var originalRow=BeginEvidence(originalId,stale ? "FullBridge" : "ActualWorker","None",null,null,
                new[]{"REQ-M5D7QC4-004","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-004","AC-M5D7QC4-008"});
            var successorRow=BeginEvidence("edit_"+parent+"_successor","FullBridge","None",null,null,
                new[]{"REQ-M5D7QC4-004","REQ-M5D7QC4-006"},new[]{"AC-M5D7QC4-004","AC-M5D7QC4-008"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var ownerType=C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerV1");
                var epoch=(long)C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epoch",typeof(long));
                var token=C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epochToken",typeof(object));
                var generation=(long)C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_generation",typeof(long));
                var history=C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_commitHistory",ownerType.GetNestedType("CommitHistory",BindingFlags.NonPublic));
                var witness=f.ReadConfirmedWitness();
                var oldCapture=C4ActualExecutionEditFixtureV1.Field(witness,witness.GetType(),"Capture",C4ActualExecutionEditFixtureV1.ProfileType("ProfileResetConfirmationCaptureV1"));
                var oldResult=C4ActualExecutionEditFixtureV1.Field(witness,witness.GetType(),"SourceResult",typeof(ProfileNewGameCaptureResultV1));
                var oldSourceWitness=C4ActualExecutionEditFixtureV1.Field(witness,witness.GetType(),"SourceWitness",typeof(ProfileNewGameConfirmationV1).GetNestedType("ResultWitness",BindingFlags.NonPublic));
                var handoffType=C4ActualExecutionEditFixtureV1.Hub("HubMenuIntentHandoffOwnerV1");
                var takenType=typeof(Nullable<>).MakeGenericType(C4ActualExecutionEditFixtureV1.Hub("HubMenuIntentRequestV1"));
                var oldTaken=C4ActualExecutionEditFixtureV1.Field(f.HandoffOwner,handoffType,"_takenRequest",takenType);
                var presenterType=C4ActualExecutionEditFixtureV1.Hub("HubMenuPresenterV1");
                var oldCursor=C4ActualExecutionEditFixtureV1.Field(f.Presenter,presenterType,"_cursor",typeof(UiSemanticFrameCursorV1));
                ProfileResetExecutionResultV1 result; C4ActualExecutionEditFixtureV1.RootLeaseWorker actualWorker=null;
                if (stale) { File.AppendAllText(Path.Combine(f.Root,"profile.json"),"\n"); result=f.ExecuteOwner(); }
                else { using (actualWorker=new C4ActualExecutionEditFixtureV1.RootLeaseWorker(f.Root)) result=f.ExecuteOwner(); }
                Assert.That(result.Outcome,Is.EqualTo(stale ? ProfileResetExecutionOutcomeV1.ConfirmationStale : ProfileResetExecutionOutcomeV1.Busy));
                Assert.That((long)C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epoch",typeof(long)),Is.EqualTo(checked(epoch+1)));
                Assert.That(C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epochToken",typeof(object)),Is.Not.SameAs(token));
                Assert.That((long)C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_generation",typeof(long)),Is.EqualTo(generation));
                Assert.That(C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_commitHistory",ownerType.GetNestedType("CommitHistory",BindingFlags.NonPublic)),Is.SameAs(history));
                Assert.That(C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_confirmed",typeof(ConfirmedProfileResetRequestV1)),Is.Null);
                Assert.That(C4ActualExecutionEditFixtureV1.Field(f.HandoffOwner,handoffType,"_takenRequest",takenType),Is.EqualTo(oldTaken));
                Assert.That(C4ActualExecutionEditFixtureV1.Field(f.Presenter,presenterType,"_cursor",typeof(UiSemanticFrameCursorV1)),Is.SameAs(oldCursor));
                Assert.That(f.ReadConfirmedWitness(),Is.SameAs(witness));
                Assert.That(C4ActualExecutionEditFixtureV1.Field(witness,witness.GetType(),"Capture",C4ActualExecutionEditFixtureV1.ProfileType("ProfileResetConfirmationCaptureV1")),Is.SameAs(oldCapture));
                Assert.That(C4ActualExecutionEditFixtureV1.Field(witness,witness.GetType(),"SourceResult",typeof(ProfileNewGameCaptureResultV1)),Is.SameAs(oldResult));
                Assert.That(C4ActualExecutionEditFixtureV1.Field(witness,witness.GetType(),"SourceWitness",oldSourceWitness.GetType()),Is.SameAs(oldSourceWitness));
                Assert.That(()=>f.ExecuteOwner(),Throws.InvalidOperationException);
                originalRow.Reached();
                f.ObserveActualResult(originalRow,result); f.ObserveActualBarrier(originalRow,result,actualWorker);
                f.ObserveNoOpCallCounts(originalRow,result);
                f.Router.StepForTests(); f.PresenterFixed(); f.ActivateNewGame(); f.LateTakePoll();
                var next=f.Take(); Assert.That(next,Is.Not.Null);
                var confirmed=f.ConfirmActual(f.Intake(next)); Assert.That(confirmed,Is.Not.SameAs(f.Confirmed));
                Assert.That((long)C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_generation",typeof(long)),Is.EqualTo(checked(generation+1)));
                Assert.That(C4ActualExecutionEditFixtureV1.Invoke(f.Owner,ownerType,"CommitForExecution",typeof(ConfirmedProfileResetRequestV1),new[] {typeof(ConfirmedProfileResetRequestV1)},confirmed),Is.SameAs(confirmed));
                var nextResult=ProfileNewGameConfirmationV1.ExecuteConfirmedReset(confirmed,f.Adapter,f.Router);
                Assert.That(nextResult.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Completed));
                result.Validate(); Assert.That(result.Outcome,Is.EqualTo(stale ? ProfileResetExecutionOutcomeV1.ConfirmationStale : ProfileResetExecutionOutcomeV1.Busy));
                successorRow.Reached();
                f.ObserveActualResult(successorRow,nextResult,confirmed); f.ObserveActualBarrier(successorRow,nextResult,null,confirmed);
                f.ObserveNoOpCallCounts(successorRow,nextResult,false,confirmed);
                f.ObserveActualSuccessorLink(successorRow,originalId,result,confirmed,nextResult);
                f.Dispose(); f.ObserveActualCleanup(originalRow); f.ObserveActualCleanup(successorRow);
                if(actualWorker!=null)
                {
                    originalRow.Observe("cleanup.releaseSignaled",actualWorker.ReleaseSignaled ? "Yes" : "No");
                    originalRow.Observe("cleanup.workerLeaseReleased",actualWorker.LeaseReleased ? "Yes" : "No");
                    originalRow.Observe("cleanup.workerJoined",actualWorker.Joined ? "Yes" : "No");
                }
            }
        }
        // REQ-M5D7QC4-004 / AC-M5D7QC4-004: 준비 중 재진입은 정상 intake에도 권한을 발급하지 않는다.
        [Test] [Timeout(180000)]
        public void AC004_PreparingOwnerCannotBindOrIssueUntilReciprocalComplete()
        {
            var row=BeginEvidence("edit_AC004_PreparingOwnerCannotBindOrIssueUntilReciprocalComplete","ActualWorker","C4","AfterFreshPrepare",21,
                new[]{"REQ-M5D7QC4-004"},new[]{"AC-M5D7QC4-004"});
            using (var f=C4ActualExecutionEditFixtureV1.Create())
            {
                var ownerType=C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerV1");
                var oldEpoch=(long)C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epoch",typeof(long));
                var issued=C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_acceptedIssued",C4ActualExecutionEditFixtureV1.Hub("IssuedNewGameRequestV1"));
                var reached=0; var c1=0; var c2=0; var prepared=0; var memory=0; var reserve=0; var complete=0;
                var control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint=>
                {
                    if(checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin)c1++;
                    if(checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize)c2++;
                    if(checkpoint == ProfileResetExecutionCheckpointV1.AfterFreshReserve)reserve++;
                    if(checkpoint == ProfileResetExecutionCheckpointV1.AfterFreshConsume)complete++;
                    if (checkpoint != ProfileResetExecutionCheckpointV1.AfterFreshPrepare) return;
                    reached++; row.Reached();
                    Assert.That(C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_state",C4ActualExecutionEditFixtureV1.Hub("NewGameConfirmationOwnerStateV1")).ToString(),Is.EqualTo("ExecutionFreshPreparing"));
                    Assert.That((long)C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epoch",typeof(long)),Is.EqualTo(oldEpoch));
                    Assert.That(()=>f.Intake(issued),Throws.InvalidOperationException);
                    Assert.That(C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_confirmed",typeof(ConfirmedProfileResetRequestV1)),Is.SameAs(f.Confirmed));
                },()=>{},proof=>prepared++,actualMemory=>memory++);
                ProfileResetExecutionResultV1 result;
                var lease=new C4ActualExecutionEditFixtureV1.RootLeaseWorker(f.Root);
                using (lease) result=f.Execute(control,C4ActualExecutionEditFixtureV1.DiskControl((name,value)=>{}));
                f.AcceptFresh(result); Assert.That(reached,Is.EqualTo(1));
                Assert.That((long)C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_epoch",typeof(long)),Is.EqualTo(checked(oldEpoch+1)));
                Assert.That(C4ActualExecutionEditFixtureV1.Field(f.Owner,ownerType,"_confirmed",typeof(ConfirmedProfileResetRequestV1)),Is.Null);
                Assert.That(result.Outcome,Is.EqualTo(ProfileResetExecutionOutcomeV1.Busy));
                f.ObserveActualResult(row,result); f.ObserveActualBarrier(row,result,lease);
                row.Observe("callCounts.c1Begin",c1); row.Observe("callCounts.c2Finalize",c2);
                row.Observe("callCounts.preparedObserver",prepared); row.Observe("callCounts.c2Observer",memory);
                row.Observe("callCounts.freshReserve",reserve); row.Observe("callCounts.freshComplete",complete);
                ObserveUnreturnedNoNative(row,"AC004_PreparingOwnerCannotBindOrIssueUntilReciprocalComplete");
                f.Dispose(); f.ObserveActualCleanup(row);
                row.Observe("cleanup.releaseSignaled",lease.ReleaseSignaled ? "Yes" : "No");
                row.Observe("cleanup.workerLeaseReleased",lease.LeaseReleased ? "Yes" : "No");
                row.Observe("cleanup.workerJoined",lease.Joined ? "Yes" : "No");
            }
        }
        // REQ-M5D7QC4-007 / AC-M5D7QC4-009: 보존된 원문에서 기존 transfer의 전체 본문을 비교한다.
        [Test] [Timeout(180000)]
        public void AC009_ExecutionFreshTransferKeepsLegacyBodiesExact()
        {
            var row=BeginEvidence("edit_AC009_ExecutionFreshTransferKeepsLegacyBodiesExact","Structure","None",null,null,new[]{"REQ-M5D7QC4-007"},new[]{"AC-M5D7QC4-009"});
            var project=Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath,".."));
            foreach (var name in new[] {"HubMenuPresenterV1","HubMenuIntentHandoffOwnerV1"})
            {
                var original=File.ReadAllText(Path.Combine(project,"docs","evidence","c4-runtime-predecessor",name+".cs.txt"));
                var current=File.ReadAllText(Path.Combine(UnityEngine.Application.dataPath,"AcadeGameMaker","Runtime","HubPresentation","Unity",name+".cs"));
                var signature=name == "HubMenuPresenterV1" ? "internal bool TryTakeRetainedIntent(" : "internal bool TryTakeRequest(";
                Assert.That(ReadExactMethodBody(current,signature),Is.EqualTo(ReadExactMethodBody(original,signature)));
            }
            row.Reached(); ObserveStructureScope(row);
        }
        private static string ReadExactMethodBody(string source,string signature)
        {
            var start=source.IndexOf(signature,StringComparison.Ordinal); Assert.That(start,Is.GreaterThanOrEqualTo(0));
            Assert.That(source.IndexOf(signature,start+signature.Length,StringComparison.Ordinal),Is.EqualTo(-1));
            var open=source.IndexOf('{',start); Assert.That(open,Is.GreaterThan(start));
            var depth=0; var state=0;
            for (var i=open;i<source.Length;i++)
            {
                var c=source[i]; var next=i+1<source.Length ? source[i+1] : '\0';
                if (state == 1) { if (c == '\n') state=0; continue; }
                if (state == 2) { if (c == '*' && next == '/') { state=0; i++; } continue; }
                if (state == 3 || state == 4) { if (c == '\\') { i++; continue; } if (c == (state == 3 ? '"' : '\'')) state=0; continue; }
                if (state == 5) { if (c == '"') { if (next == '"') i++; else state=0; } continue; }
                if (c == '/' && next == '/') { state=1; i++; continue; }
                if (c == '/' && next == '*') { state=2; i++; continue; }
                if (c == '@' && next == '"') { state=5; i++; continue; }
                if (c == '"') { state=3; continue; } if (c == '\'') { state=4; continue; }
                if (c == '{') depth++; if (c == '}' && --depth == 0) return source.Substring(start,i-start+1);
            }
            Assert.Fail("감사 본문이 닫히지 않았습니다."); return null;
        }
        internal sealed class ExecutionCounts
        {
            internal int C1,C2,PreparedObserver,C2Observer,FreshReserve;
            internal readonly IProfileResetExecutionTestControlV1 Control;
            internal ExecutionCounts()
            {
                Control=ProfileResetExecutionTestControlFactoryV1.Create(checkpoint =>
                {
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC1Begin) C1++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.AfterC2Finalize) C2++;
                    if (checkpoint == ProfileResetExecutionCheckpointV1.BeforeFreshReserve) FreshReserve++;
                },() => { },actual => { PreparedObserver++; },actual => { C2Observer++; });
            }
        }
    }
}
