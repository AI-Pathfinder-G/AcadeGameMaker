using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-M5D7QC2R-001..006; AC-M5D7QC2R-001/002/003/004/007/008 partial. Same-process fresh
    // object harness only: C1's on-disk barrier is resumed by Start; no caller
    // prepared proof is ever supplied to the recovery adapter.
    internal sealed class ProfileResetRestartBootstrapV1Tests
    {
        [Test]
        public void AC_M5D7QC2R_001_003_004_008_SameProcessRestartResumesC1AndCompletesWithoutOrdinaryLaunch()
        {
            using(var fixture=RecoveryFixture.Create("completed",null))
            {
                fixture.Start();var result=fixture.Adapter.RestartBootstrapResult;
                Assert.That(result.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.Completed));Assert.That(result.Receipt,Is.Not.Null);result.Receipt.Validate();
                Assert.That(fixture.Adapter.CurrentReceipt,Is.Null);Assert.That(fixture.Preparation.Calls,Is.EqualTo(0));Assert.That(fixture.Environment.UtcCalls,Is.EqualTo(0));
                var cell=fixture.Adapter.CurrentResetSession;Assert.That(cell.Root,Is.EqualTo(ProfileResetBarrierV1.NormalizeRoot(fixture.Root)));fixture.Router.ValidateAllMapsDisabled(fixture.Actions);
                CollectionAssert.AreEqual(ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes,File.ReadAllBytes(Path.Combine(fixture.Root,"profile.json")));AssertBarrierAbsent(fixture.Root);
                var adapterGeneration=(long)Field(fixture.Adapter,"_resetGeneration");var routerGeneration=(long)Field(fixture.Router,"_resetCutoverGeneration");var cellGeneration=(long)Field(cell,"_generation");Assert.That(adapterGeneration,Is.GreaterThan(0));Assert.That(routerGeneration,Is.EqualTo(adapterGeneration));Assert.That(cellGeneration,Is.EqualTo(adapterGeneration));
                Assert.That((bool)Field(fixture.Router,"_resetOldDisposeAttempted"),Is.False);ProfileLaunchNotificationV1 notification;Assert.That(fixture.Adapter.TryTakeNotification(out notification),Is.False);
            }
        }

        [Test]
        public void AC_M5D7QC2R_001_OrdinaryAbsentRootUsesOrdinaryPreparationOnce()
        {
            using(var fixture=RecoveryFixture.CreateOrdinary("ordinary"))
            {
                fixture.Start();Assert.That(fixture.Adapter.RestartBootstrapResult.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.OrdinaryLaunchRequired));Assert.That(fixture.Preparation.Calls,Is.EqualTo(1));Assert.That(fixture.Environment.UtcCalls,Is.EqualTo(1));Assert.That(fixture.Adapter.CurrentReceipt.HasValue,Is.True);
            }
        }

        [Test]
        public void AC_M5D7QC2R_003_008_DuplicateAndReflectedStartAttemptCannotReopenCompletedRecovery()
        {
            using(var fixture=RecoveryFixture.Create("duplicate",null))
            {
                fixture.Start();var completed=fixture.Adapter.RestartBootstrapResult;Assert.That(completed.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.Completed));
                fixture.Start();Assert.That(fixture.Adapter.RestartBootstrapResult,Is.SameAs(completed));
                typeof(DesktopProfileLaunchAdapterV1).GetField("_startAttempted",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(fixture.Adapter,false);
                fixture.Start();Assert.That(fixture.Adapter.RestartBootstrapResult,Is.SameAs(completed));Assert.That(fixture.Adapter.CurrentReceipt,Is.Null);Assert.That(fixture.Preparation.Calls,Is.EqualTo(0));
                ProfileLaunchNotificationV1 notification;Assert.That(fixture.Adapter.TryTakeNotification(out notification),Is.False);
            }
        }

        [Test]
        public void AC_M5D7QC2R_001_ActualHeldRootLeaseReturnsBusyBlockedWithoutCandidate()
        {
            var root=RecoveryFixture.NewRoot("busy");var first=ProfileResetDiskTransactionV1.Begin(root,ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));Assert.That(first.Outcome,Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
            try { using(var lease=ProfileRootOperationLockV1.Acquire(root)){var fixture=RecoveryFixture.CreateAt(root,null);try{fixture.Start();Assert.That(fixture.Adapter.RestartBootstrapResult.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.BusyBlocked));Assert.That(Field(fixture.Adapter,"_untransferredCandidate"),Is.Null);Assert.That(fixture.Adapter.CurrentReceipt,Is.Null);}finally{UnityEngine.Object.DestroyImmediate(fixture.Parent);}} }
            finally { RecoveryFixture.Cleanup(root); }
        }

        [TestCase("temp")]
        [TestCase("barrier-directory")]
        [TestCase("root-unsafe")]
        public void AC_M5D7QC2R_001_InvalidRecoverySelectionIsManualWithoutOrdinaryPreparation(string kind)
        {
            var root=RecoveryFixture.NewRoot(kind);var first=ProfileResetDiskTransactionV1.Begin(root,ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));Assert.That(first.Outcome,Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
            try { if(kind=="temp")File.WriteAllBytes(Path.Combine(root,ProfileResetBarrierV1.BarrierTempName),new byte[]{1});else if(kind=="barrier-directory"){File.Delete(Path.Combine(root,ProfileResetBarrierV1.BarrierName));Directory.CreateDirectory(Path.Combine(root,ProfileResetBarrierV1.BarrierName));}else{var lockPath=Path.Combine(root,"profile.operation.lock");if(File.Exists(lockPath))File.Delete(lockPath);Directory.CreateDirectory(lockPath);}
                using(var fixture=RecoveryFixture.CreateAt(root,null)){fixture.Start();Assert.That(fixture.Adapter.RestartBootstrapResult.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.ManualRepairRequired));Assert.That(fixture.Preparation.Calls,Is.EqualTo(0));Assert.That(fixture.Adapter.CurrentReceipt,Is.Null);} }
            finally { RecoveryFixture.Cleanup(root); }
        }

        [Test]
        public void AC_M5D7QC2R_001_AbsentSelectionReservesBeforeOneRootReadAndUsesOnlyTheFirstRoot()
        {
            var first=RecoveryFixture.NewRoot("single-root-first");var second=RecoveryFixture.NewRoot("single-root-second");Directory.CreateDirectory(second);Directory.CreateDirectory(Path.Combine(second,ProfileResetBarrierV1.BarrierName));
            var environment=new ScriptedRootEnvironment(first,second);var preparation=new TracePreparation(environment.Events);
            try
            {
                using(var fixture=RecoveryFixture.CreateWithEnvironment(first,environment,preparation))
                {
                    Assert.That(environment.RootCalls,Is.EqualTo(1));Assert.That(environment.SecondRootWasQueried,Is.False);Assert.That(environment.ReservedAtFirstRoot,Is.True);
                    CollectionAssert.AreEqual(new[]{"root","utc","prepare"},environment.Events);Assert.That(preparation.Root,Is.EqualTo(ProfileResetBarrierV1.NormalizeRoot(first)));Assert.That(preparation.Calls,Is.EqualTo(1));
                    Assert.That(fixture.Adapter.RestartBootstrapResult.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.OrdinaryLaunchRequired));
                }
            }
            finally { RecoveryFixture.Cleanup(second); }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void AC_M5D7QC2R_001_PrivatePreparedReservationPromotesActionlesslyBeforeOrAfterRouterAwake(bool afterRouterAwake)
        {
            var root=RecoveryFixture.NewRoot("promotion-"+afterRouterAwake);
            using(var fixture=RecoveryFixture.CreatePristine(root))
            {
                fixture.Router.ReservePreparedHubLaunch(fixture.Adapter);if(afterRouterAwake)Invoke(fixture.Router,"Awake");
                fixture.Router.PromotePreparedReservationToRecovery(fixture.Adapter,ProfileResetBarrierV1.NormalizeRoot(root),false);
                AssertActionlessRecovery(fixture);Assert.That(Field(fixture.Router,"_preparedLaunchState").ToString(),Is.EqualTo("Failed"));Assert.That(Field(fixture.Router,"_recoveryReservation").ToString(),Is.EqualTo("Reserved"));
                Assert.That(()=>fixture.Router.InitializePreparedHub(fixture.Adapter),Throws.InvalidOperationException);Assert.That(fixture.Adapter.HasExactResetLaunchCohort(),Is.False);
            }
        }

        [Test]
        public void AC_M5D7QC2R_002_ForeignDuplicateAndCorruptPromotionLeaveOtherReservedCohortUntouched()
        {
            var primaryRoot=RecoveryFixture.NewRoot("promotion-primary");var otherRoot=RecoveryFixture.NewRoot("promotion-other");
            using(var primary=RecoveryFixture.CreatePristine(primaryRoot))using(var other=RecoveryFixture.CreatePristine(otherRoot))
            {
                primary.Router.ReservePreparedHubLaunch(primary.Adapter);other.Router.ReservePreparedHubLaunch(other.Adapter);
                Assert.That(()=>primary.Router.PromotePreparedReservationToRecovery(other.Adapter,ProfileResetBarrierV1.NormalizeRoot(primaryRoot),false),Throws.InvalidOperationException);
                AssertPreparedReservationUntouched(primary);AssertPreparedReservationUntouched(other);
                primary.Router.PromotePreparedReservationToRecovery(primary.Adapter,ProfileResetBarrierV1.NormalizeRoot(primaryRoot),false);
                Assert.That(()=>primary.Router.PromotePreparedReservationToRecovery(primary.Adapter,ProfileResetBarrierV1.NormalizeRoot(primaryRoot),false),Throws.InvalidOperationException);AssertActionlessRecovery(primary);AssertPreparedReservationUntouched(other);
                FieldInfo proof=primary.Router.GetType().GetField("_preparedLaunchOwnerProof",BindingFlags.Instance|BindingFlags.NonPublic);proof.SetValue(other.Router,new object());
                Assert.That(()=>other.Router.PromotePreparedReservationToRecovery(other.Adapter,ProfileResetBarrierV1.NormalizeRoot(otherRoot),false),Throws.InvalidOperationException);AssertPreparedReservationUntouched(other);Assert.That(primary.Adapter.HasExactResetLaunchCohort(),Is.False);
            }
        }

        [Test]
        public void AC_M5D7QC2R_003_005_PreResumeDisableAndReflectedDiagnosticsCannotReopenRecoveryOrFallback()
        {
            using(var reserved=RecoveryFixture.Create("reserved-disable",null))
            {
                reserved.Parent.SetActive(false);reserved.Start();Assert.That(reserved.Preparation.Calls,Is.EqualTo(0));Assert.That(reserved.Environment.UtcCalls,Is.EqualTo(0));Assert.That(reserved.Adapter.CurrentReceipt,Is.Null);Assert.That(reserved.Actions,Is.Null);
            }
            var root=RecoveryFixture.NewRoot("repair-reflection");var barrier=Path.Combine(root,ProfileResetBarrierV1.BarrierName);
            try
            {
                var first=ProfileResetDiskTransactionV1.Begin(root,ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));Assert.That(first.Outcome,Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));File.Delete(barrier);Directory.CreateDirectory(barrier);
                using(var repair=RecoveryFixture.CreateAt(root,null))
                {
                    Assert.That(repair.Adapter.RestartBootstrapResult.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.ManualRepairRequired));AssertActionlessRecovery(repair);repair.Parent.SetActive(false);
                    SetField(repair.Adapter,"_restartBootstrap",0);SetField(repair.Adapter,"_startAttempted",false);SetField(repair.Router,"_recoveryReservation",0);repair.Start();var routerFailure=Assert.Throws<TargetInvocationException>(()=>Invoke(repair.Router,"Start"));Assert.That(routerFailure.InnerException,Is.TypeOf<InvalidOperationException>());Assert.That(routerFailure.InnerException.Message,Is.EqualTo("Failed launch state is malformed."));
                    Assert.That(repair.Adapter.RestartBootstrapResult.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.ManualRepairRequired));Assert.That(repair.Preparation.Calls,Is.EqualTo(0));Assert.That(repair.Environment.UtcCalls,Is.EqualTo(0));Assert.That(repair.Adapter.CurrentReceipt,Is.Null);Assert.That(repair.Actions,Is.Null);
                    // Deliberately reflected recovery ownership remains malformed through
                    // teardown; this exact fail-closed invariant log is not suppression.
                    LogAssert.Expect(LogType.Exception,"InvalidOperationException: Failed launch state is malformed.");
                }
            }
            finally { RecoveryFixture.Cleanup(root); }
        }

        [TestCase("null")]
        [TestCase("whitespace")]
        [TestCase("relative")]
        [TestCase("filesystem-root")]
        public void AC_M5D7QC2R_001_005_HubUiOnlyReturnedUnsafeRootClosesReadableManualWithoutOrdinaryFallback(string kind)
        {
            var cleanupRoot=RecoveryFixture.NewRoot("returned-unsafe-"+kind);var environment=new UnsafeReturnedRootEnvironment(kind,cleanupRoot);var preparation=new CountingPreparation();
            using(var fixture=RecoveryFixture.CreateWithUnsafeSelection(cleanupRoot,environment,preparation))
            {
                Assert.That(environment.RootCalls,Is.EqualTo(1));Assert.That(environment.UtcCalls,Is.EqualTo(0));Assert.That(preparation.Calls,Is.EqualTo(0));Assert.That(fixture.Adapter.RestartBootstrapResult.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.ManualRepairRequired));
                Assert.That(Field(fixture.Adapter,"_pending"),Is.Null);Assert.That((bool)Field(fixture.Adapter,"_adopted"),Is.False);AssertActionlessRecovery(fixture);
                Assert.DoesNotThrow(()=>Invoke(fixture.Router,"Start"));Assert.DoesNotThrow(()=>Invoke(fixture.Router,"FixedUpdate"));fixture.Parent.SetActive(false);Assert.DoesNotThrow(()=>Invoke(fixture.Router,"FixedUpdate"));
                Assert.That(environment.UtcCalls,Is.EqualTo(0));Assert.That(preparation.Calls,Is.EqualTo(0));Assert.That(fixture.Adapter.CurrentReceipt,Is.Null);Assert.That(fixture.Actions,Is.Null);
            }
        }

        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeStaging)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterCandidateCreate)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeBindingApply)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterBindingApply)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeTerminalGate)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterTerminalGate)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeTransfer)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterTransfer)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeCell)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterCell)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforePair)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterPair)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeDelete)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterDelete)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeFinalProbe)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.AfterFinalProbe)]
        [TestCase(ProfileResetMemoryCutoverCheckpointV1.BeforeReceiptStage)]
        public void AC_M5D7QC2R_002_Through006_RestartApplicableCheckpointClosesFreshRecovery(ProfileResetMemoryCutoverCheckpointV1 point)
        {
            using(var fixture=RecoveryFixture.Create("checkpoint-"+point,new ThrowControl(point)))
            {
                fixture.Start();var result=fixture.Adapter.RestartBootstrapResult;Assert.That(result.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.ReloadRequired));Assert.That(fixture.Adapter.CurrentReceipt,Is.Null);Assert.That(fixture.Preparation.Calls,Is.EqualTo(0));Assert.That(fixture.Environment.UtcCalls,Is.EqualTo(0));
                AssertBarrierState(fixture.Root,(int)point>=(int)ProfileResetMemoryCutoverCheckpointV1.AfterDelete);
            }
        }

        [TestCase(ResetCutoverOperationV1.DisableNewGameplay,false)]
        [TestCase(ResetCutoverOperationV1.DisableNewGameplay,true)]
        [TestCase(ResetCutoverOperationV1.DisposeNew,false)]
        [TestCase(ResetCutoverOperationV1.DisposeNew,true)]
        public void AC_M5D7QC2R_003_004_SelectedRestartOwnedOperationFaultsCloseFreshRecovery(ResetCutoverOperationV1 operation,bool after)
        {
            using(var fixture=RecoveryFixture.Create("operation-"+operation+after,new OperationControl(operation,after)))
            {fixture.Start();Assert.That(fixture.Adapter.RestartBootstrapResult.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.ReloadRequired));Assert.That(fixture.Adapter.CurrentReceipt,Is.Null);}
        }

        private static object Field(object target,string name){return target.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(target);}
        private static void SetField(object target,string name,object value){var field=target.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic);field.SetValue(target,field.FieldType.IsEnum&&value is int?Enum.ToObject(field.FieldType,value):value);}
        private static void Invoke(object target,string name){target.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(target,null);}
        private static void AssertPreparedReservationUntouched(RecoveryFixture fixture){Assert.That(Field(fixture.Router,"_preparedLaunchState").ToString(),Is.EqualTo("Reserved"));Assert.That(Field(fixture.Router,"_actions"),Is.Null);Assert.That((bool)Field(fixture.Router,"_gameplayCallbacksRegistered"),Is.False);Assert.That((bool)Field(fixture.Router,"_uiCallbacksRegistered"),Is.False);Assert.That(fixture.Adapter.CurrentReceipt,Is.Null);}
        private static void AssertActionlessRecovery(RecoveryFixture fixture){Assert.That(Field(fixture.Router,"_actions"),Is.Null);Assert.That((bool)Field(fixture.Router,"_gameplayCallbacksRegistered"),Is.False);Assert.That((bool)Field(fixture.Router,"_uiCallbacksRegistered"),Is.False);Assert.That(fixture.Adapter.CurrentReceipt,Is.Null);Assert.That(fixture.Adapter.HasExactResetLaunchCohort(),Is.False);}
        private static void AssertBarrierState(string root,bool absent){if(absent){AssertBarrierAbsent(root);return;}var path=Path.Combine(root,ProfileResetBarrierV1.BarrierName);var attributes=File.GetAttributes(path);Assert.That((attributes&FileAttributes.Directory),Is.EqualTo((FileAttributes)0));Assert.That((attributes&FileAttributes.ReparsePoint),Is.EqualTo((FileAttributes)0));Assert.Throws<FileNotFoundException>(()=>File.GetAttributes(Path.Combine(root,ProfileResetBarrierV1.BarrierTempName)));}
        private static void AssertBarrierAbsent(string root){foreach(var name in new[]{ProfileResetBarrierV1.BarrierName,ProfileResetBarrierV1.BarrierTempName}){var path=Path.Combine(root,name);try{var attributes=File.GetAttributes(path);if((attributes&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Barrier absence cannot follow a reparse observation.");throw new InvalidOperationException("Barrier name remains present: "+name);}catch(FileNotFoundException){}catch(DirectoryNotFoundException){}}}
        private sealed class ThrowControl:IProfileResetMemoryCutoverControlV1
        {private readonly ProfileResetMemoryCutoverCheckpointV1 _point;internal ThrowControl(ProfileResetMemoryCutoverCheckpointV1 point){_point=point;}public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point){if(point==_point)throw new InvalidOperationException("Injected restart boundary.");}public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point){}public void InvokeOperation(ResetCutoverOperationV1 op,Action exact){exact();}}
        private sealed class OperationControl:IProfileResetMemoryCutoverControlV1
        {private readonly ResetCutoverOperationV1 _op;private readonly bool _after;internal OperationControl(ResetCutoverOperationV1 op,bool after){_op=op;_after=after;}public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 p){}public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 p){if(_op==ResetCutoverOperationV1.DisposeNew&&p==ProfileResetMemoryCutoverCheckpointV1.AfterTerminalGate)throw new InvalidOperationException();}public void InvokeOperation(ResetCutoverOperationV1 op,Action exact){if(op!=_op){exact();return;}if(!_after)throw new InvalidOperationException();exact();throw new InvalidOperationException();}}
        private sealed class CountingEnvironment:IDesktopProfileLaunchEnvironmentV1
        {private readonly string _root;internal int UtcCalls;internal CountingEnvironment(string root){_root=root;}public string GetPersistentDataPath(){return _root;}public DateTimeOffset GetUtcNow(){UtcCalls++;return new DateTimeOffset(2026,9,28,0,0,0,TimeSpan.Zero);}}
        private sealed class CountingPreparation:IDesktopProfileLaunchPreparationV1
        {internal int Calls;public PreparedProfileLaunchV1 Prepare(string root,DateTimeOffset utc){Calls++;return ProfileLaunchPreparationCoordinatorV1.Prepare(root,utc);}}
        private sealed class ScriptedRootEnvironment:IDesktopProfileLaunchEnvironmentV1
        {private readonly string _first,_second;internal readonly List<string> Events=new List<string>();internal int RootCalls;internal bool SecondRootWasQueried,ReservedAtFirstRoot;internal Func<bool> ReservationAtRoot;internal ScriptedRootEnvironment(string first,string second){_first=first;_second=second;}public string GetPersistentDataPath(){RootCalls++;Events.Add("root");if(RootCalls==1){ReservedAtFirstRoot=ReservationAtRoot!=null&&ReservationAtRoot();return _first;}SecondRootWasQueried=true;return _second;}public DateTimeOffset GetUtcNow(){Events.Add("utc");return new DateTimeOffset(2026,9,28,0,0,0,TimeSpan.Zero);}}
        private sealed class TracePreparation:IDesktopProfileLaunchPreparationV1
        {private readonly List<string> _events;internal int Calls;internal string Root;internal TracePreparation(List<string> events){_events=events;}public PreparedProfileLaunchV1 Prepare(string root,DateTimeOffset utc){Calls++;Root=root;_events.Add("prepare");return ProfileLaunchPreparationCoordinatorV1.Prepare(root,utc);}}
        private sealed class UnsafeReturnedRootEnvironment:IDesktopProfileLaunchEnvironmentV1
        {private readonly string _kind,_safeRoot;internal int RootCalls,UtcCalls;internal UnsafeReturnedRootEnvironment(string kind,string safeRoot){_kind=kind;_safeRoot=safeRoot;}public string GetPersistentDataPath(){RootCalls++;if(_kind=="null")return null;if(_kind=="whitespace")return " \t";if(_kind=="relative")return "relative-profile";if(_kind=="filesystem-root")return Path.GetPathRoot(_safeRoot);throw new InvalidOperationException("Unknown unsafe-root case.");}public DateTimeOffset GetUtcNow(){UtcCalls++;return new DateTimeOffset(2026,9,28,0,0,0,TimeSpan.Zero);}}
        private sealed class RecoveryFixture:IDisposable
        {
            private static readonly object IssuedRootsGate=new object();private static readonly HashSet<string> IssuedRoots=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            internal readonly string Root;internal readonly GameObject Parent;internal readonly InputRouter Router;internal readonly DesktopProfileLaunchAdapterV1 Adapter;internal readonly CountingEnvironment Environment;internal readonly CountingPreparation Preparation;
            private RecoveryFixture(string root,GameObject parent,InputRouter router,DesktopProfileLaunchAdapterV1 adapter,CountingEnvironment env,CountingPreparation prep){Root=root;Parent=parent;Router=router;Adapter=adapter;Environment=env;Preparation=prep;}
            internal GameInputActions Actions{get{return(GameInputActions)Field(Router,"_actions");}}
            internal static string NewRoot(string name){var root=Path.Combine(Path.GetTempPath(),"AcadeGameMaker-C2R-"+name+"-"+Guid.NewGuid().ToString("N"));lock(IssuedRootsGate)IssuedRoots.Add(Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar));return root;}
            internal static RecoveryFixture Create(string name,IProfileResetMemoryCutoverControlV1 control){var root=NewRoot(name);try{var first=ProfileResetDiskTransactionV1.Begin(root,ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(root));Assert.That(first.Outcome,Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));return CreateAt(root,control,true);}catch{Cleanup(root);throw;}}
            internal static RecoveryFixture CreateOrdinary(string name){return CreateAt(NewRoot(name),null,true);}
            internal static RecoveryFixture CreateWithEnvironment(string root,ScriptedRootEnvironment environment,TracePreparation preparation)
            {var parent=new GameObject("c2r-trace");parent.SetActive(false);try{var ro=new GameObject("router");ro.transform.SetParent(parent.transform);var router=ro.AddComponent<InputRouter>();router.ConfigureHubUiOnlyForAuthoring();environment.ReservationAtRoot=()=>string.Equals(Field(router,"_preparedLaunchState").ToString(),"Reserved",StringComparison.Ordinal);var ao=new GameObject("adapter");ao.transform.SetParent(parent.transform);var adapter=ao.AddComponent<DesktopProfileLaunchAdapterV1>();adapter.ConfigureForTests(router,environment,preparation);parent.SetActive(true);return new RecoveryFixture(root,parent,router,adapter,null,null);}catch{UnityEngine.Object.DestroyImmediate(parent);Cleanup(root);throw;}}
            internal static RecoveryFixture CreateWithUnsafeSelection(string root,UnsafeReturnedRootEnvironment environment,CountingPreparation preparation)
            {var parent=new GameObject("c2r-unsafe-root");parent.SetActive(false);try{var ro=new GameObject("router");ro.transform.SetParent(parent.transform);var router=ro.AddComponent<InputRouter>();router.ConfigureHubUiOnlyForAuthoring();var ao=new GameObject("adapter");ao.transform.SetParent(parent.transform);var adapter=ao.AddComponent<DesktopProfileLaunchAdapterV1>();adapter.ConfigureForTests(router,environment,preparation);parent.SetActive(true);return new RecoveryFixture(root,parent,router,adapter,null,null);}catch{UnityEngine.Object.DestroyImmediate(parent);Cleanup(root);throw;}}
            internal static RecoveryFixture CreatePristine(string root)
            {var parent=new GameObject("c2r-promotion");parent.SetActive(false);try{var ro=new GameObject("router");ro.transform.SetParent(parent.transform);var router=ro.AddComponent<InputRouter>();router.ConfigureHubUiOnlyForAuthoring();var ao=new GameObject("adapter");ao.transform.SetParent(parent.transform);var adapter=ao.AddComponent<DesktopProfileLaunchAdapterV1>();adapter.ConfigureForTests(router,new CountingEnvironment(root),new CountingPreparation());return new RecoveryFixture(root,parent,router,adapter,null,null);}catch{UnityEngine.Object.DestroyImmediate(parent);Cleanup(root);throw;}}
            internal static RecoveryFixture CreateAt(string root,IProfileResetMemoryCutoverControlV1 control,bool cleanup=false)
            {var parent=new GameObject("c2r");parent.SetActive(false);try{var ro=new GameObject("router");ro.transform.SetParent(parent.transform);var router=ro.AddComponent<InputRouter>();router.ConfigureHubUiOnlyForAuthoring();var env=new CountingEnvironment(root);var prep=new CountingPreparation();var ao=new GameObject("adapter");ao.transform.SetParent(parent.transform);var adapter=ao.AddComponent<DesktopProfileLaunchAdapterV1>();adapter.ConfigureForTests(router,env,prep);if(control!=null)adapter.ConfigureRecoveryControlForTests(control);parent.SetActive(true);return new RecoveryFixture(root,parent,router,adapter,env,prep);}catch{UnityEngine.Object.DestroyImmediate(parent);if(cleanup)Cleanup(root);throw;}}
            internal void Start(){typeof(DesktopProfileLaunchAdapterV1).GetMethod("Start",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(Adapter,null);}
            public void Dispose(){UnityEngine.Object.DestroyImmediate(Parent);Cleanup(Root);}internal static void Cleanup(string root){var full=Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);lock(IssuedRootsGate)if(!IssuedRoots.Contains(full))throw new InvalidOperationException("Cleanup root was not issued by this fixture.");var temp=Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);ValidateCleanupLocation(full,temp);if(!Directory.Exists(full))return;VerifyTree(full,temp);Directory.Delete(full,true);}private static void ValidateCleanupLocation(string full,string temp){var leaf=Path.GetFileName(full);var prefix="AcadeGameMaker-C2R-";if(!leaf.StartsWith(prefix,StringComparison.Ordinal)||leaf.Length<=prefix.Length+32)throw new InvalidOperationException("Cleanup root has no issued fixture name.");var nonce=leaf.Substring(leaf.Length-32);for(var i=0;i<nonce.Length;i++)if(!Uri.IsHexDigit(nonce[i]))throw new InvalidOperationException("Cleanup root nonce is malformed.");if(!full.StartsWith(temp+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Cleanup escaped OS temp.");var parent=new DirectoryInfo(full).Parent;var reachedTemp=false;for(var ancestor=parent;ancestor!=null;ancestor=ancestor.Parent){var attributes=File.GetAttributes(ancestor.FullName);if((attributes&FileAttributes.Directory)==0||(attributes&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Cleanup ancestor is unsafe.");if(string.Equals(ancestor.FullName.TrimEnd(Path.DirectorySeparatorChar),temp,StringComparison.OrdinalIgnoreCase)){reachedTemp=true;break;}}if(!reachedTemp)throw new InvalidOperationException("Cleanup ancestry escaped OS temp.");}private static void VerifyTree(string root,string temp){if((File.GetAttributes(root)&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Cleanup root is reparse.");var reachedTemp=false;for(var ancestor=new DirectoryInfo(root);ancestor!=null;ancestor=ancestor.Parent){var attributes=File.GetAttributes(ancestor.FullName);if((attributes&FileAttributes.Directory)==0||(attributes&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Cleanup ancestor is unsafe.");if(string.Equals(ancestor.FullName.TrimEnd(Path.DirectorySeparatorChar),temp,StringComparison.OrdinalIgnoreCase)){reachedTemp=true;break;}}if(!reachedTemp)throw new InvalidOperationException("Cleanup ancestry escaped OS temp.");VerifyChildren(root,root,temp);}private static void VerifyChildren(string directory,string root,string temp){foreach(var entry in Directory.GetFileSystemEntries(directory)){var full=Path.GetFullPath(entry);if(!full.StartsWith(root+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)||!full.StartsWith(temp+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Cleanup child escaped owned root.");var attributes=File.GetAttributes(full);if((attributes&FileAttributes.ReparsePoint)!=0)throw new InvalidOperationException("Cleanup refuses reparse descendant.");if((attributes&FileAttributes.Directory)!=0)VerifyChildren(full,root,temp);}}
        }
    }
}
