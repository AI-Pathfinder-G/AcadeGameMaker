using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-M5D7QC2R-001/005/006; AC-M5D7QC2R-005/006 execution fixture.
    // Main supplies isolated Unity processes. This fixture never spawns, kills,
    // cleans, or repairs a process/root; it only uses production C1/C2R paths.
    internal sealed class ProfileResetRestartProcessV1Tests
    {
        [Test]
        public void AC_M5D7QC2R_005_ProcessPrepareWritesDiskPreparedEvidence()
        {
            var context=PhaseContext.Require("Prepare");
            Directory.CreateDirectory(context.Root);var old=NonDefaultOldProgressBytes();File.WriteAllBytes(Path.Combine(context.Root,"profile.json"),old);var oldHash=Hash(old);context.WriteSeed("oldHash="+oldHash+" revision=7 choice=Solidarity skill=CommonReferencePlane branch=Solidarity");
            var result=ProfileResetDiskTransactionV1.Begin(context.Root,ProfileResetDiskTransactionV1.CaptureConfirmationIdentity(context.Root));
            Assert.That(result.Outcome,Is.EqualTo(ProfileResetDiskOutcomeV1.DiskPrepared));
            var archive=Path.Combine(context.Root,"recovery","new-game",result.TransactionId,"primary.bin");Assert.That(File.Exists(archive),Is.True);Assert.That(Hash(File.ReadAllBytes(archive)),Is.EqualTo(oldHash));AssertBarrierPresent(context.Root);context.Write("pid="+Process.GetCurrentProcess().Id+" outcome=DiskPrepared tx="+result.TransactionId+" oldHash="+oldHash);
        }

        [Test]
        public void AC_M5D7QC2R_005_006_ProcessResumeCompletesFreshAuthorGraph()
        {
            var context=PhaseContext.Require("Resume");
            using(var fixture=LaunchFixture.Create(context.Root,null))
            {
                fixture.Start();var bootstrap=fixture.Adapter.RestartBootstrapResult;Assert.That(bootstrap.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.Completed));var receipt=bootstrap.Receipt;receipt.Validate();
                Assert.That(fixture.Adapter.CurrentReceipt,Is.Null);Assert.That(fixture.Preparation.Calls,Is.EqualTo(0));Assert.That(fixture.Environment.UtcCalls,Is.EqualTo(0));fixture.Router.ValidateAllMapsDisabled(fixture.Actions);AssertBarrierAbsent(context.Root);
                var defaults=ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes;var cell=fixture.Adapter.CurrentResetSession;Assert.That(cell.Root,Is.EqualTo(ProfileResetBarrierV1.NormalizeRoot(context.Root)));CollectionAssert.AreEqual(defaults,File.ReadAllBytes(Path.Combine(context.Root,"profile.json")));CollectionAssert.AreEqual(defaults,cell.CopyCanonicalDocument());Assert.That(Hash(defaults),Is.Not.EqualTo(context.ReadSeedHash()));AssertGenerationAgreement(fixture);
                Assert.That((bool)Field(fixture.Router,"_resetOldDisposeAttempted"),Is.False);ProfileLaunchNotificationV1 notification;Assert.That(fixture.Adapter.TryTakeNotification(out notification),Is.False);
                ProfileResetMemoryReceiptV1 taken;Assert.That(fixture.Adapter.TryTakeResetReceipt(out taken),Is.True);Assert.That(taken,Is.SameAs(receipt));Assert.That(fixture.Adapter.TryTakeResetReceipt(out taken),Is.False);
                context.Write("pid="+Process.GetCurrentProcess().Id+" outcome=Completed hash="+receipt.TargetSha256+" maps=disabled history=none receipt=oneshot oldProgress=absent");
            }
        }

        [Test]
        public void AC_M5D7QC2R_006_ProcessDeleteGateSignalsAfterActualDeleteAndWaitsForMain()
        {
            var context=PhaseContext.Require("DeleteGate");
            using(var fixture=LaunchFixture.Create(context.Root,new DeleteGateControl(context)))
            {
                fixture.Start();Assert.Fail("Main must terminate this process after deleted.signal; reaching Start return is a harness failure.");
            }
        }

        [Test]
        public void AC_M5D7QC2R_006_ProcessOrdinaryAfterDeleteHasNoResetReceipt()
        {
            var context=PhaseContext.Require("OrdinaryAfterDelete");
            using(var fixture=LaunchFixture.Create(context.Root,null))
            {
                fixture.Start();Assert.That(fixture.Adapter.RestartBootstrapResult.Outcome,Is.EqualTo(RestartBootstrapOutcomeV1.OrdinaryLaunchRequired));Assert.That(fixture.Adapter.CurrentReceipt.HasValue,Is.True);Assert.That(fixture.Preparation.Calls,Is.EqualTo(1));Assert.That(fixture.Environment.UtcCalls,Is.EqualTo(1));
                Assert.Throws<InvalidOperationException>(()=>{var ignored=fixture.Adapter.CurrentResetSession;});ProfileResetMemoryReceiptV1 ignoredReceipt;Assert.That(fixture.Adapter.TryTakeResetReceipt(out ignoredReceipt),Is.False);AssertBarrierAbsent(context.Root);
                var defaults=ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes;CollectionAssert.AreEqual(defaults,File.ReadAllBytes(Path.Combine(context.Root,"profile.json")));Assert.That(Hash(defaults),Is.Not.EqualTo(context.ReadSeedHash()));
                context.Write("pid="+Process.GetCurrentProcess().Id+" outcome=OrdinaryLaunchRequired preparation=1 utc=1 resetReceipt=none");
            }
        }

        private sealed class DeleteGateControl:IProfileResetMemoryCutoverControlV1
        {
            private readonly PhaseContext _context;private bool _signalled;internal DeleteGateControl(PhaseContext context){_context=context;}
            public void Checkpoint(ProfileResetMemoryAuthorityCheckpointV1 point){}
            public void InvokeOperation(ResetCutoverOperationV1 op,Action exact){exact();}
            public void Checkpoint(ProfileResetMemoryCutoverCheckpointV1 point)
            {
                if(_signalled||point!=ProfileResetMemoryCutoverCheckpointV1.AfterDelete)return;_signalled=true;AssertBarrierAbsent(_context.Root);_context.WriteSignal("pid="+Process.GetCurrentProcess().Id+" phase=DeleteGate deleted=true");
                var watch=Stopwatch.StartNew();while(watch.Elapsed<TimeSpan.FromSeconds(60))Thread.Sleep(100);throw new TimeoutException("Main did not terminate DeleteGate process within 60 seconds.");
            }
        }
        private sealed class PhaseContext
        {
            internal readonly string Base,Root,Phase;private PhaseContext(string b,string p){Base=b;Root=Path.Combine(b,"profile");Phase=p;}
            internal static PhaseContext Require(string expected){var b=Environment.GetEnvironmentVariable("ACADE_C2R_PROCESS_BASE");var p=Environment.GetEnvironmentVariable("ACADE_C2R_PROCESS_PHASE");if(string.IsNullOrWhiteSpace(b)||string.IsNullOrWhiteSpace(p))Assert.Fail("C2R process environment is required; this fixture is never skipped.");if(!string.Equals(p,expected,StringComparison.Ordinal))Assert.Fail("C2R process phase mismatch: expected "+expected+".");var c=new PhaseContext(b,p);c.Validate();return c;}
            internal void Write(string text){WriteNew(Path.Combine(Base,Phase+".evidence.txt"),text);}internal void WriteSeed(string text){WriteNew(Path.Combine(Base,"seed.evidence.txt"),text);}internal string ReadSeedHash(){var line=File.ReadAllText(Path.Combine(Base,"seed.evidence.txt"));var prefix="oldHash=";if(!line.StartsWith(prefix,StringComparison.Ordinal)||line.Length<prefix.Length+64)Assert.Fail("Seed evidence is malformed.");return line.Substring(prefix.Length,64);}internal void WriteSignal(string text){WriteNew(Path.Combine(Base,"deleted.signal"),text);Write(text);}private static void WriteNew(string path,string text){using(var stream=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.None))using(var writer=new StreamWriter(stream,new UTF8Encoding(false))){writer.WriteLine(text);}}
            private void Validate(){var full=Path.GetFullPath(Base).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);var temp=Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);var leaf=Path.GetFileName(full);if(!leaf.StartsWith("AcadeGameMaker-C2R-Process-",StringComparison.Ordinal)||leaf.Length!="AcadeGameMaker-C2R-Process-".Length+32)Assert.Fail("C2R process base name is not the exact owned form.");for(var i="AcadeGameMaker-C2R-Process-".Length;i<leaf.Length;i++)if(!Uri.IsHexDigit(leaf[i]))Assert.Fail("C2R process base nonce is malformed.");if(!full.StartsWith(temp+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)||!Directory.Exists(full))Assert.Fail("C2R process base is not an owned OS-temp descendant.");for(var current=new DirectoryInfo(full);current!=null;current=current.Parent){var attributes=File.GetAttributes(current.FullName);if((attributes&FileAttributes.Directory)==0||(attributes&FileAttributes.ReparsePoint)!=0)Assert.Fail("C2R process base ancestry is unsafe.");if(string.Equals(current.FullName.TrimEnd(Path.DirectorySeparatorChar),temp,StringComparison.OrdinalIgnoreCase))return;}Assert.Fail("C2R process base escaped OS temp.");}
        }
        private sealed class LaunchFixture:IDisposable
        {
            internal readonly string Root;internal readonly GameObject Parent;internal readonly InputRouter Router;internal readonly DesktopProfileLaunchAdapterV1 Adapter;internal readonly CountingEnvironment Environment;internal readonly CountingPreparation Preparation;
            private LaunchFixture(string root,GameObject parent,InputRouter router,DesktopProfileLaunchAdapterV1 adapter,CountingEnvironment env,CountingPreparation prep){Root=root;Parent=parent;Router=router;Adapter=adapter;Environment=env;Preparation=prep;}internal GameInputActions Actions{get{return(GameInputActions)typeof(InputRouter).GetField("_actions",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(Router);}}
            internal static LaunchFixture Create(string root,IProfileResetMemoryCutoverControlV1 control){var parent=new GameObject("c2r-process");parent.SetActive(false);try{var ro=new GameObject("router");ro.transform.SetParent(parent.transform);var router=ro.AddComponent<InputRouter>();router.ConfigureHubUiOnlyForAuthoring();var env=new CountingEnvironment(root);var prep=new CountingPreparation();var ao=new GameObject("adapter");ao.transform.SetParent(parent.transform);var adapter=ao.AddComponent<DesktopProfileLaunchAdapterV1>();adapter.ConfigureForTests(router,env,prep);if(control!=null)adapter.ConfigureRecoveryControlForTests(control);parent.SetActive(true);return new LaunchFixture(root,parent,router,adapter,env,prep);}catch{UnityEngine.Object.DestroyImmediate(parent);throw;}}
            internal void Start(){typeof(DesktopProfileLaunchAdapterV1).GetMethod("Start",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(Adapter,null);}public void Dispose(){UnityEngine.Object.DestroyImmediate(Parent);}
        }
        private sealed class CountingEnvironment:IDesktopProfileLaunchEnvironmentV1{private readonly string _root;internal int UtcCalls;internal CountingEnvironment(string root){_root=root;}public string GetPersistentDataPath(){return _root;}public DateTimeOffset GetUtcNow(){UtcCalls++;return new DateTimeOffset(2026,9,28,0,0,0,TimeSpan.Zero);}}
        private sealed class CountingPreparation:IDesktopProfileLaunchPreparationV1{internal int Calls;public PreparedProfileLaunchV1 Prepare(string root,DateTimeOffset utc){Calls++;return ProfileLaunchPreparationCoordinatorV1.Prepare(root,utc);}}
        private static byte[] NonDefaultOldProgressBytes(){var baseline=ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.Snapshot;var progression=new ProfileProgressionSnapshot(7,202,ProfileChoice.Solidarity,ProfileConsentState.OffersScopedResonance,ProfileSkill.CommonReferencePlane,new[]{ProfileBranch.Solidarity});return ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(baseline.Settings,baseline.Input,baseline.Tutorial,progression)).CanonicalFileBytes;}
        private static string Hash(byte[] bytes){using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(bytes)).Replace("-",String.Empty).ToLowerInvariant();}
        private static void AssertGenerationAgreement(LaunchFixture fixture){var cell=fixture.Adapter.CurrentResetSession;var adapter=(long)Field(fixture.Adapter,"_resetGeneration");var router=(long)Field(fixture.Router,"_resetCutoverGeneration");var current=(long)Field(cell,"_generation");var cellActions=(GameInputActions)Field(cell,"_actions");Assert.That(adapter,Is.GreaterThan(0));Assert.That(router,Is.EqualTo(adapter));Assert.That(current,Is.EqualTo(adapter));Assert.That(cellActions,Is.SameAs(fixture.Actions));}
        private static object Field(object target,string field){return target.GetType().GetField(field,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(target);}
        private static void AssertBarrierPresent(string root){foreach(var name in new[]{ProfileResetBarrierV1.BarrierName}){var a=File.GetAttributes(Path.Combine(root,name));Assert.That((a&FileAttributes.Directory),Is.EqualTo((FileAttributes)0));Assert.That((a&FileAttributes.ReparsePoint),Is.EqualTo((FileAttributes)0));}Assert.Throws<FileNotFoundException>(()=>File.GetAttributes(Path.Combine(root,ProfileResetBarrierV1.BarrierTempName)));}
        private static void AssertBarrierAbsent(string root){foreach(var name in new[]{ProfileResetBarrierV1.BarrierName,ProfileResetBarrierV1.BarrierTempName}){try{File.GetAttributes(Path.Combine(root,name));Assert.Fail("Barrier remains: "+name);}catch(FileNotFoundException){}catch(DirectoryNotFoundException){}}}
    }
}
