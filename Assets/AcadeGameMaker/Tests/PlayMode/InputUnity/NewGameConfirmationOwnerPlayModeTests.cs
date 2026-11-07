using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using AcadeGameMaker.Input;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // AC-M5D7QC3-001/005/006/008/009: actual temporary-root launch and actual
    // virtual UI input. Reflection binds normal APIs by complete signatures.
    internal sealed class NewGameConfirmationOwnerPlayModeTests : InputTestFixture
    {
        [Test]
        public void AC001_NormalDisableAfterActualTakeRejectsLateIntakeWithoutChangingClosedHistory()
        {
            using (var f=Fixture.Create(false))
            {
                f.SelectInitialNewGame(); var actual=f.Take(); ((Behaviour)f.Owner).enabled=false;
                Assert.That(f.OwnerState,Is.EqualTo("Closed")); Assert.That(f.HandoffState,Is.EqualTo("Closed"));
                Assert.That(f.Field(f.Presenter,"_confirmationInteractionClosed"),Is.True);
                var before=f.IntakeSnapshot(); var operation=f.Field(f.Owner,"_operation");
                Assert.That(() => f.Accept(actual),Throws.InvalidOperationException);
                Assert.That(f.IntakeSnapshot(),Is.EqualTo(before)); Assert.That(f.Field(f.Owner,"_operation"),Is.EqualTo(operation));
                Assert.That(f.OwnerState,Is.EqualTo("Closed")); Assert.That(f.HandoffState,Is.EqualTo("Closed"));
            }
        }
        [TestCase(false)] [TestCase(true)]
        public void AC005_SameActualCancelPreservesCompleteObservedFixtureSnapshot(bool tempPresent)
        {
            using (var f=Fixture.Create(false))
            {
                f.SelectInitialNewGame();
                var d=ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.Snapshot;
                var meaningful=ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.BorderlessFullscreen,999,700,800,false,false),d.Input,d.Tutorial,d.Progression));
                File.WriteAllBytes(Path.Combine(f.Root,"profile.json"),meaningful.CanonicalFileBytes);
                if (tempPresent) File.WriteAllBytes(Path.Combine(f.Root,"profile.tmp.json"),Encoding.UTF8.GetBytes("{"));
                var actual=f.Take(); var decision=f.Decision(f.Accept(actual));
                var generation=f.Field(f.Owner,"_generation"); var epoch=f.Field(f.Owner,"_epoch");
                var before=f.CancelSnapshot();
                var cancelled=f.Cancel(decision);
                var after=f.CancelSnapshot();
                Assert.That(after.Keys,Is.EquivalentTo(before.Keys)); foreach (var pair in before) Assert.That(after[pair.Key],Is.EqualTo(pair.Value),pair.Key);
                Assert.That(f.OwnerState,Is.EqualTo("Cancelled")); Assert.That(f.Field(f.Owner,"_decision"),Is.Null); Assert.That(f.Field(f.Owner,"_rearm"),Is.SameAs(f.RearmCapability(cancelled)));
                Assert.That(f.Field(f.Owner,"_epoch"),Is.EqualTo(epoch)); Assert.That(f.Field(f.Owner,"_generation"),Is.EqualTo(generation));
                TestContext.Out.WriteLine("{\"id\":\"C5-same-cancel-"+(tempPresent?"present":"absent")+"\",\"status\":\"passed\",\"observedFields\":"+before.Count+",\"currentMemoryAbsent\":"+(before["adapter._currentSession"]==null?"true":"false")+",\"wholeGameplaySessionClaimed\":false}");
                f.Rearm(f.RearmCapability(cancelled)); Assert.That(f.Field(f.Owner,"_epoch"),Is.EqualTo((long)epoch+1));
            }
        }
        [Test]
        public void AC005_ActualInputCancelRearmPreservesInitialTakenHistory()
        {
            using (var f=Fixture.Create(false))
            {
                f.SelectInitialNewGame();
                var first=f.Take(); var decision=f.Decision(f.Accept(first));
                var intent=f.Field(f.Presenter,"_retainedIntent"); var taken=f.Field(f.HandoffOwner,"_takenRequest"); var cursor=f.Field(f.Presenter,"_cursor");
                var receipt=f.Router.CurrentReceipt; var current=f.Adapter.CurrentReceipt;
                var bytes=File.ReadAllBytes(Path.Combine(f.Root,"profile.json"));
                var rearm=f.RearmCapability(f.Cancel(decision)); f.Rearm(rearm);
                Assert.That(f.Field(f.Presenter,"_retainedIntent"),Is.EqualTo(intent));
                Assert.That(f.Field(f.HandoffOwner,"_takenRequest"),Is.EqualTo(taken));
                Assert.That(f.Field(f.Presenter,"_cursor"),Is.SameAs(cursor));
                Assert.That(f.Router.CurrentReceipt,Is.EqualTo(receipt)); Assert.That(f.Adapter.CurrentReceipt,Is.EqualTo(current));
                Assert.That(File.ReadAllBytes(Path.Combine(f.Root,"profile.json")),Is.EqualTo(bytes));
                Assert.That(f.OwnerState,Is.EqualTo("AwaitingRequest"));
                Assert.That(() => f.Rearm(rearm),Throws.InvalidOperationException);
            }
        }
        [Test]
        public void AC006_ImmediateReadyFactoryDiscardsActualSubmitBeforeNewTake()
        {
            using (var f=Fixture.Create(false))
            {
                f.SelectInitialNewGame();
                var beforeTake=f.InitialHistory();
                var decision=f.Decision(f.Accept(f.Take()));
                var oldTaken=f.InitialHistory()[2];
                f.Rearm(f.RearmCapability(f.Cancel(decision)));
                var slot=f.Field(f.Presenter,"_successor");
                Assert.That(((UiSemanticFrameCursorV1)f.Field(slot,"Cursor")).State,Is.EqualTo(UiSemanticFrameCursorStateV1.Ready));
                f.PresenterUpdate(); f.PresenterFixed();
                Assert.That(f.Field(slot,"BaselinePending"),Is.True);
                f.Publish(Key.Enter);
                Assert.That(f.Router.CurrentUiFrame.Value.SubmitPressed,Is.True);
                f.PresenterFixed(); f.Late();
                Assert.That(f.Field(slot,"BaselinePending"),Is.False);
                Assert.That(f.Field(slot,"Phase").ToString(),Is.EqualTo("Ready"));
                Assert.That(f.Field(slot,"Retained"),Is.Null,"discarded true frame cannot activate");
                var afterDiscard=f.InitialHistory();
                Assert.That(afterDiscard[0],Is.SameAs(beforeTake[0]));
                Assert.That(afterDiscard[1],Is.EqualTo(beforeTake[1]));
                Assert.That(afterDiscard[2],Is.EqualTo(oldTaken));
                Assert.That(f.HandoffState,Is.EqualTo("RequestTaken"));
                Assert.That(f.TryTake(out var ignored),Is.False);
                Assert.That(ignored,Is.Null);
                f.Publish(); f.PresenterFixed(); f.Late();
                f.Publish(Key.Enter);
                Assert.That(f.Router.CurrentUiFrame.Value.SubmitPressed,Is.True);
                f.PresenterFixed(); f.Late();
                var next=f.Take(); var result=f.Accept(next);
                Assert.That(f.Outcome(result),Is.EqualTo("DecisionRequired"));
                var nextDecision=f.Decision(result);
                Assert.That(nextDecision,Is.Not.SameAs(decision));
            }
        }
        [Test]
        public void AC005_RepeatedActualInputCancellationAppendsThreeEpochs()
        {
            using (var f=Fixture.Create(false))
            {
                f.SelectInitialNewGame(); object previousHistory=null; object initialTaken=null;
                for (var epoch=1;epoch<=3;epoch++)
                {
                    var decision=f.Decision(f.Accept(f.Take()));
                    if (initialTaken == null) initialTaken=f.Field(f.HandoffOwner,"_takenRequest");
                    f.Rearm(f.RearmCapability(f.Cancel(decision)));
                    var history=f.Field(f.Owner,"_history");
                    Assert.That(f.Field(history,"Previous"),Is.SameAs(previousHistory)); previousHistory=history;
                    Assert.That(f.Field(f.Owner,"_epoch"),Is.EqualTo((long)epoch+1));
                    Assert.That(f.Field(f.HandoffOwner,"_takenRequest"),Is.EqualTo(initialTaken));
                    Assert.That(f.Field(f.Owner,"_pendingIssued"),Is.Null); Assert.That(f.Field(f.Owner,"_acceptedIssued"),Is.Null);
                    f.Publish(); f.PresenterFixed(); // first true frame is discarded
                    f.Publish(Key.Enter); f.PresenterFixed(); f.Late();
                }
            }
        }
        [Test]
        public void AC006_ActualSkippedSuccessorPublicationClosesAllParticipants()
        {
            using (var f=Fixture.Create(false))
            {
                f.SelectInitialNewGame(); var decision=f.Decision(f.Accept(f.Take())); f.Rearm(f.RearmCapability(f.Cancel(decision)));
                f.Publish(); f.Publish(); f.PresenterFixed();
                Assert.That(f.OwnerState,Is.EqualTo("TerminalFailure"));
                Assert.That(f.HandoffState,Is.EqualTo("Closed"));
                Assert.That(f.Field(f.Presenter,"_confirmationInteractionClosed"),Is.True);
            }
        }
        [Test]
        public void AC006_DismissedActualLaunchNoticeSurvivesSuccessorWithoutRetake()
        {
            using (var f=Fixture.Create(true))
            {
                f.SelectInitialNewGame();
                var controller=(HubMenuPresentationControllerV1)f.Field(f.Presenter,"_controller");
                Assert.That(controller.CurrentView.NotificationState,Is.EqualTo(HubNoticeStateV1.Dismissed));
                var notificationHistory=f.Field(f.Presenter,"_notification");
                var decision=f.Decision(f.Accept(f.Take())); f.Rearm(f.RearmCapability(f.Cancel(decision)));
                var slot=f.Field(f.Presenter,"_successor");
                var successor=(HubMenuPresentationControllerV1)f.Field(slot,"Controller");
                Assert.That(successor.CurrentView.NotificationState,Is.EqualTo(HubNoticeStateV1.Dismissed));
                Assert.That(f.Field(f.Presenter,"_notification"),Is.EqualTo(notificationHistory));
                Assert.That(f.Latch.TryTakeLaunchNotification(out var ignored),Is.False);
                f.Publish(Key.Enter); f.PresenterFixed(); f.Late();
                Assert.That(f.Field(slot,"Retained"),Is.Null);
                Assert.That(successor.CurrentView.NotificationState,Is.EqualTo(HubNoticeStateV1.Dismissed));
            }
        }
        [Test]
        public void AC008_ActualConfirmedRequestCommitsOnlyAfterUpperClosure()
        {
            using (var f=Fixture.Create(false))
            {
                f.SelectInitialNewGame(); var decision=f.Decision(f.Accept(f.Take()));
                var confirmed=f.Confirmed(f.Confirm(decision));
                var committed=f.Commit(confirmed);
                Assert.That(committed,Is.SameAs(confirmed)); Assert.That(f.OwnerState,Is.EqualTo("ExecutionCommitted"));
                Assert.That(f.HandoffState,Is.EqualTo("Closed")); Assert.That(f.Field(f.Presenter,"_confirmationInteractionClosed"),Is.True);
                Assert.That(() => f.Cancel(decision),Throws.InvalidOperationException);
                Assert.That(() => f.Commit(confirmed),Throws.InvalidOperationException);
                Assert.That(f.Router.CurrentReceipt.Value.Mode.ToString(),Is.EqualTo("UIOnly"));
            }
        }
        [Test]
        public void AC008_DisablingActualOwnerCannotManufactureCancelOrRearm()
        {
            using (var f=Fixture.Create(false))
            {
                f.SelectInitialNewGame(); var decision=f.Decision(f.Accept(f.Take()));
                ((Behaviour)f.Owner).enabled=false;
                Assert.That(f.OwnerState,Is.EqualTo("Closed")); Assert.That(f.Field(f.Owner,"_rearm"),Is.Null);
                Assert.That(() => f.Cancel(decision),Throws.InvalidOperationException);
            }
        }

        [TestCase("AwaitingBaseline")]
        [TestCase("Ready")]
        [TestCase("Transferred")]
        [TestCase("AwaitingDecision")]
        public void AC006_LateActualReservationPreservesInputAndDecisionAuthority(string phase)
        {
            using (var f=Fixture.Create(false))
            {
                f.SelectInitialNewGame(); var first=f.Decision(f.Accept(f.Take())); f.Rearm(f.RearmCapability(f.Cancel(first)));
                var slot=f.Field(f.Presenter,"_successor"); var cursor=f.Field(slot,"Cursor");
                var reservation=f.Field(slot,"Reservation"); var token=f.Field(slot,"Token"); var epoch=(long)f.Field(slot,"Epoch");
                if (phase != "AwaitingBaseline") { f.Publish(); f.PresenterFixed(); }
                object handle=null,decision=null;
                if (phase == "Transferred" || phase == "AwaitingDecision")
                {
                    f.Publish(Key.Enter); f.PresenterFixed(); f.Late(); handle=f.Take();
                    if (phase == "AwaitingDecision") decision=f.Decision(f.Accept(handle));
                }
                var requestSlot=f.Field(f.HandoffOwner,"_successor"); var pending=f.Field(f.Owner,"_pendingIssued"); var pendingHandoff=f.Field(f.HandoffOwner,"_pendingIssued"); var ownerState=f.OwnerState;
                Assert.That(f.MatchesHistoricalReservation(reservation,token,epoch),Is.True);
                Assert.That(() => f.PrepareAgain(reservation,token,epoch),Throws.InvalidOperationException);
                Assert.That(() => f.CommitAgain(reservation,token,epoch),Throws.InvalidOperationException);
                Assert.That(f.Field(f.Presenter,"_successor"),Is.SameAs(slot)); Assert.That(f.Field(slot,"Cursor"),Is.SameAs(cursor));
                Assert.That(f.Field(f.HandoffOwner,"_successor"),Is.SameAs(requestSlot)); Assert.That(f.Field(f.Owner,"_pendingIssued"),Is.SameAs(pending)); Assert.That(f.OwnerState,Is.EqualTo(ownerState));
                Assert.That(f.Field(f.HandoffOwner,"_pendingIssued"),Is.SameAs(pendingHandoff));
                if (phase == "AwaitingBaseline") { f.Publish(); f.PresenterFixed(); }
                if (handle == null) { f.Publish(Key.Enter); f.PresenterFixed(); f.Late(); handle=f.Take(); }
                if (decision == null) decision=f.Decision(f.Accept(handle));
                Assert.That(f.Field(f.Owner,"_decision"),Is.SameAs(decision));
                f.Rearm(f.RearmCapability(f.Cancel(decision)));
                Assert.That(f.Field(f.Owner,"_epoch"),Is.EqualTo(3L));
            }
        }

        [Test]
        public void AC003_RearmGraphicCallbackRejectsSameThreadStaleConfirmCancelAndRearm()
        {
            TestContext.Progress.WriteLine("{\"id\":\"C3-rearm-render-stale-reentry\",\"status\":\"planned\"}");
            using (var f=Fixture.Create(false))
            {
                f.SelectInitialNewGame(); var old=f.Decision(f.Accept(f.Take())); var rearm=f.RearmCapability(f.Cancel(old));
                var thread=Thread.CurrentThread.ManagedThreadId; var epoch=(long)f.Field(f.Owner,"_epoch"); var history=f.Field(f.Owner,"_history");
                var textType=Type.GetType("TMPro.TMP_Text, Unity.TextMeshPro",true);
                Assert.That(textType.FullName,Is.EqualTo("TMPro.TMP_Text")); Assert.That(textType.Assembly.GetName().Name,Is.EqualTo("Unity.TextMeshPro"));
                var labels=((Component)f.Presenter).GetComponentsInChildren(textType,true); Graphic label=null;
                foreach (var candidate in labels) if (((Graphic)candidate).isActiveAndEnabled && candidate.transform.parent.GetComponent(Type.GetType("AcadeGameMaker.Hub.Presentation.Unity.HubMenuButtonViewV1, AcadeGameMaker.Hub.Presentation.Unity",true))!=null) { label=(Graphic)candidate; break; }
                Assert.That(label,Is.Not.Null,"normal rendered button graphic is required");
                var setter=textType.GetMethod("set_text",BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly,null,new[] {typeof(string)},null);
                Assert.That(setter,Is.Not.Null); Assert.That(setter.DeclaringType,Is.EqualTo(textType)); Assert.That(setter.ReturnType,Is.EqualTo(typeof(void))); Assert.That(setter.IsGenericMethod,Is.False);
                Assert.That(textType.IsInstanceOfType(label),Is.True); Assert.That(setter.GetParameters()[0].IsOptional,Is.False);
                setter.Invoke(label,new object[] {"재진입 시험 준비"});
                var entered=0; Exception callbackError=null;
                UnityEngine.Events.UnityAction callback=() =>
                {
                    entered++;
                    try
                    {
                        Assert.That(Thread.CurrentThread.ManagedThreadId,Is.EqualTo(thread)); Assert.That(f.OwnerState,Is.EqualTo("Rearming"));
                        var before=f.ReentrySnapshot(); var files=f.FileFingerprint();
                        Assert.That(() => f.Confirm(old),Throws.InvalidOperationException);
                        Assert.That(() => f.Cancel(old),Throws.InvalidOperationException);
                        Assert.That(() => f.Rearm(rearm),Throws.InvalidOperationException);
                        Assert.That(f.ReentrySnapshot(),Is.EqualTo(before)); Assert.That(f.FileFingerprint(),Is.EqualTo(files));
                    }
                    catch (Exception error) { callbackError=error; }
                };
                label.RegisterDirtyVerticesCallback(callback);
                try { f.Rearm(rearm); }
                finally { label.UnregisterDirtyVerticesCallback(callback); }
                Assert.That(entered,Is.GreaterThan(0),"callback must originate from actual Rearm Render"); Assert.That(callbackError,Is.Null);
                Assert.That(f.Field(f.Owner,"_epoch"),Is.EqualTo(epoch+1)); Assert.That(f.Field(f.Field(f.Owner,"_history"),"Previous"),Is.SameAs(history));
                var slot=f.Field(f.Presenter,"_successor"); Assert.That(f.Field(slot,"BaselinePending"),Is.True);
                f.Publish(Key.Enter); f.PresenterFixed(); f.Late(); Assert.That(f.Field(slot,"Retained"),Is.Null); Assert.That(f.TryTake(out var ignored),Is.False);
                f.Publish(); f.PresenterFixed(); f.Publish(Key.Enter); f.PresenterFixed(); f.Late();
                var next=f.Decision(f.Accept(f.Take())); Assert.That(next,Is.Not.SameAs(old));
                TestContext.Progress.WriteLine("{\"id\":\"C3-rearm-render-stale-reentry\",\"status\":\"passed\",\"sameThread\":true,\"liveConfirmReentry\":false}");
            }
        }

        private sealed class Fixture : IDisposable
        {
            private const string HubAssembly="AcadeGameMaker.Hub.Presentation.Unity";
            private static Type Hub(string name)
            {
                var full="AcadeGameMaker.Hub.Presentation.Unity."+name;
                var type=Type.GetType(full+", "+HubAssembly,true);
                if (type.FullName != full || type.Assembly.GetName().Name != HubAssembly) throw new InvalidOperationException("Hub type binding is not exact.");
                return type;
            }
            private static readonly Type PresenterType=Hub("HubMenuPresenterV1"), HandoffType=Hub("HubMenuIntentHandoffOwnerV1"), OwnerType=Hub("NewGameConfirmationOwnerV1"), IssuedType=Hub("IssuedNewGameRequestV1"), ResultType=Hub("NewGameConfirmationStartResultV1"), DecisionType=Hub("NewGameDecisionCapabilityV1"), RearmType=Hub("HubNewGameRearmCapabilityV1");
            private static object Invoke(object target,Type declaration,string method,Type returnType,Type[] parameters,params object[] arguments)
            {
                if (target == null || target.GetType() != declaration) throw new InvalidOperationException("Normal API receiver does not match its declaration.");
                var bound=declaration.GetMethod(method,BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.NonPublic,null,parameters,null);
                if (bound == null || bound.DeclaringType != declaration || bound.ReturnType != returnType || bound.IsGenericMethod || bound.GetParameters().Length != parameters.Length) throw new MissingMethodException("Exact normal API signature is missing: "+method);
                var actualParameters=bound.GetParameters();
                for (var i=0;i<parameters.Length;i++) if (actualParameters[i].ParameterType != parameters[i] || actualParameters[i].IsOptional) throw new MissingMethodException("Normal API parameter binding is not exact.");
                try { return bound.Invoke(target,arguments); }
                catch (TargetInvocationException error) { if (error.InnerException != null) throw error.InnerException; throw; }
            }
            private static object Property(object target,Type declaration,string name,Type expected)
            { return Invoke(target,declaration,"get_"+name,expected,Type.EmptyTypes); }
            private static object Opaque(object value,Type expected)
            { if (value == null || value.GetType() != expected) throw new InvalidOperationException("Actual opaque return type is not exact."); return value; }
            internal string Root;
            internal object Owner, Presenter, HandoffOwner;
            internal InputRouter Router;
            internal DesktopProfileLaunchAdapterV1 Adapter;
            internal HubEntryHandoffLatchV1 Latch;
            private GameObject _host;
            private Keyboard _keyboard;
            private bool _recovered;
            internal string OwnerState { get { return Property(Owner,OwnerType,"State",Hub("NewGameConfirmationOwnerStateV1")).ToString(); } }
            internal string HandoffState { get { return Property(HandoffOwner,HandoffType,"State",Hub("HubMenuIntentHandoffOwnerStateV1")).ToString(); } }
            internal static Fixture Create(bool recovered)
            {
                var f=new Fixture { Root=Path.Combine(Path.GetTempPath(),"AcadeGameMaker-C3-play-"+Guid.NewGuid().ToString("N")), _recovered=recovered };
                Directory.CreateDirectory(f.Root);
                if (!recovered) File.WriteAllBytes(Path.Combine(f.Root,"profile.json"),ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                f._host=new GameObject("actual-c3-play-cohort"); f._host.SetActive(false);
                try
                {
                    var runtime=new GameObject("launch"); runtime.transform.SetParent(f._host.transform,false);
                    f.Router=runtime.AddComponent<InputRouter>(); f.Adapter=runtime.AddComponent<DesktopProfileLaunchAdapterV1>(); f.Latch=runtime.AddComponent<HubEntryHandoffLatchV1>();
                    f.Router.ConfigureHubUiOnlyForAuthoring(); f.Adapter.ConfigureHubUiOnlyForAuthoring(f.Router);
                    f.Adapter.ConfigureForTests(f.Router,new EnvironmentPort(f.Root),new DesktopProfileLaunchPreparationPortV1()); f.Latch.ConfigureHubUiOnlyForAuthoring(f.Adapter,f.Router);
                    var prefab=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Hub/HubMenuRoot.prefab"); Assert.That(prefab,Is.Not.Null);
                    var menu=UnityEngine.Object.Instantiate(prefab,f._host.transform,false);
                    f.Presenter=menu.GetComponent(PresenterType); f.HandoffOwner=menu.GetComponent(HandoffType);
                    var buttonType=Hub("HubMenuButtonViewV1"); var components=menu.GetComponentsInChildren(buttonType,true); var buttons=Array.CreateInstance(buttonType,components.Length);
                    for (var i=0;i<components.Length;i++) buttons.SetValue(components[i],i);
                    var safeType=Hub("HubSafeFrameScalerV1");
                    var eventType=ExactType("UnityEngine.EventSystems.EventSystem","UnityEngine.UI"); var textType=ExactType("TMPro.TMP_Text","Unity.TextMeshPro");
                    var notice=menu.transform.Find("SafeFrame/Notification"); var dismiss=notice.Find("Dismiss");
                    Invoke(f.Presenter,PresenterType,"ConfigureForAuthoring",typeof(void),new[] { typeof(InputRouter),typeof(HubEntryHandoffLatchV1),safeType,eventType,buttonType.MakeArrayType(),typeof(RectTransform),typeof(GameObject),textType,textType },f.Router,f.Latch,menu.GetComponent(safeType),menu.GetComponentInChildren(eventType),buttons,dismiss.GetComponent<RectTransform>(),notice.gameObject,notice.Find("Message").GetComponent(textType),dismiss.GetComponent(textType));
                    f.Owner=menu.AddComponent(OwnerType);
                    Invoke(f.Owner,OwnerType,"ConfigureForAuthoring",typeof(void),new[] { PresenterType,HandoffType,typeof(DesktopProfileLaunchAdapterV1),typeof(InputRouter) },f.Presenter,f.HandoffOwner,f.Adapter,f.Router);
                    f._keyboard=InputSystem.AddDevice<Keyboard>(); f._host.SetActive(true);
                    Assert.That(f.Router.PreparedLaunchAwakeEntered,Is.True,"actual runtime Awake");
                    Invoke(f.Adapter,typeof(DesktopProfileLaunchAdapterV1),"Start",typeof(void),Type.EmptyTypes);
                    Invoke(f.Latch,typeof(HubEntryHandoffLatchV1),"Start",typeof(void),Type.EmptyTypes);
                    Invoke(f.Latch,typeof(HubEntryHandoffLatchV1),"Update",typeof(void),Type.EmptyTypes);
                    f.Router.StepForTests(); f.PresenterUpdate();
                    Assert.That(Property(f.Presenter,PresenterType,"State",Hub("HubPresenterStateV1")).ToString(),Is.EqualTo("Ready"));
                    File.WriteAllBytes(Path.Combine(f.Root,"profile.prev.json"),ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                    return f;
                }
                catch { f.Dispose(); throw; }
            }
            private static Type ExactType(string full,string assembly)
            { var type=Type.GetType(full+", "+assembly,true); if (type.FullName != full || type.Assembly.GetName().Name != assembly) throw new InvalidOperationException("UI type binding is not exact."); return type; }
            internal void Publish(params Key[] keys) { InputSystem.QueueStateEvent(_keyboard,new KeyboardState(keys)); InputSystem.Update(); Router.StepForTests(); }
            internal void PresenterUpdate() { Invoke(Presenter,PresenterType,"Update",typeof(void),Type.EmptyTypes); }
            internal void PresenterFixed() { Invoke(Presenter,PresenterType,"FixedUpdate",typeof(void),Type.EmptyTypes); }
            internal void Late() { Invoke(HandoffOwner,HandoffType,"LateUpdate",typeof(void),Type.EmptyTypes); }
            internal bool MatchesHistoricalReservation(object reservation,object token,long epoch)
            { return (bool)Invoke(HandoffOwner,HandoffType,"MatchesSuccessorReservation",typeof(bool),new[] { Hub("HubNewGameSuccessorReservationV1"),OwnerType,PresenterType,typeof(InputRouter),typeof(object),typeof(long) },reservation,Owner,Presenter,Router,token,epoch); }
            internal void PrepareAgain(object reservation,object token,long epoch)
            { Invoke(Presenter,PresenterType,"PrepareNewGameSuccessor",typeof(void),new[] { OwnerType,Hub("HubNewGameSuccessorReservationV1"),typeof(object),typeof(long) },Owner,reservation,token,epoch); }
            internal void CommitAgain(object reservation,object token,long epoch)
            { Invoke(HandoffOwner,HandoffType,"CommitNewGameSuccessor",typeof(void),new[] { Hub("HubNewGameSuccessorReservationV1"),OwnerType,PresenterType,typeof(InputRouter),typeof(object),typeof(long) },reservation,Owner,Presenter,Router,token,epoch); }
            internal object[] ReentrySnapshot()
            { return new[] { Field(Owner,"_epoch"),Field(Owner,"_generation"),Field(Owner,"_history"),Field(Owner,"_pendingIssued"),Field(Owner,"_acceptedIssued"),Field(Owner,"_decision"),Field(Owner,"_rearm"),Field(Owner,"_operation"),Field(HandoffOwner,"_pendingIssued"),Field(HandoffOwner,"_pendingSuccessor"),Field(HandoffOwner,"_successor"),Field(Presenter,"_successor") }; }
            internal object[] IntakeSnapshot()
            { return new[] { Field(HandoffOwner,"_request"),Field(HandoffOwner,"_requestProof"),Field(HandoffOwner,"_takenRequest"),Field(HandoffOwner,"_takenRequestProof"),Field(HandoffOwner,"_epochs"),Field(HandoffOwner,"_pendingIssued"),Field(Owner,"_pendingIssued"),Field(Owner,"_acceptedIssued"),Field(Owner,"_display"),Field(Owner,"_epoch"),Field(Owner,"_history") }; }
            internal string FileFingerprint()
            {
                var result=new StringBuilder(); foreach (var name in new[] {"profile.json","profile.prev.json","profile.tmp.json"}) { var path=Path.Combine(Root,name); result.Append(name).Append('='); if (File.Exists(path)) { var bytes=File.ReadAllBytes(path); using (var sha=SHA256.Create()) result.Append(bytes.Length).Append(':').Append(BitConverter.ToString(sha.ComputeHash(bytes))); } else result.Append("absent"); result.Append(';'); } return result.ToString();
            }
            private static object SnapshotField(object target,Type declaration,string name,Type expected)
            {
                if (target==null || target.GetType()!=declaration) throw new InvalidOperationException("Snapshot receiver must match its exact declaration.");
                var field=declaration.GetField(name,BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.DeclaredOnly);
                if (field==null || field.DeclaringType!=declaration || field.FieldType!=expected) throw new MissingFieldException(name);
                return field.GetValue(target);
            }
            internal Dictionary<string,object> CancelSnapshot()
            {
                var result=new Dictionary<string,object>();
                Action<string,object,Type,string,Type> add=(prefix,target,declaration,name,type) => result.Add(prefix+"."+name,SnapshotField(target,declaration,name,type));
                foreach (var name in new[] {"profile.json","profile.prev.json","profile.tmp.json"}) { var path=Path.Combine(Root,name); result.Add("file."+name,File.Exists(path)?File.ReadAllBytes(path):null); }
                result.Add("files.fingerprint",FileFingerprint());
                foreach (var name in new[] {"_resetCutoverTerminal"}) add("adapter",Adapter,typeof(DesktopProfileLaunchAdapterV1),name,typeof(bool));
                add("adapter",Adapter,typeof(DesktopProfileLaunchAdapterV1),"_currentSession",typeof(CurrentSessionProfileV1));
                add("adapter",Adapter,typeof(DesktopProfileLaunchAdapterV1),"_resetLifecycle",ExactType(typeof(DesktopProfileLaunchAdapterV1).FullName+"+ResetAdapterLifecycleV1","AcadeGameMaker.Input.Unity"));
                add("adapter",Adapter,typeof(DesktopProfileLaunchAdapterV1),"_resetGeneration",typeof(long));
                add("adapter",Adapter,typeof(DesktopProfileLaunchAdapterV1),"_resetReceipt",typeof(ProfileResetMemoryReceiptV1));
                add("adapter",Adapter,typeof(DesktopProfileLaunchAdapterV1),"_untransferredCandidate",typeof(GameInputActions));
                add("adapter",Adapter,typeof(DesktopProfileLaunchAdapterV1),"_restartBootstrap",ExactType(typeof(DesktopProfileLaunchAdapterV1).FullName+"+RestartBootstrapStateV1","AcadeGameMaker.Input.Unity"));
                add("adapter",Adapter,typeof(DesktopProfileLaunchAdapterV1),"_restartResult",typeof(RestartBootstrapResultV1));
                foreach (var name in new[] {"_launchRootWitnessA","_launchRootWitnessB"}) add("adapter",Adapter,typeof(DesktopProfileLaunchAdapterV1),name,typeof(string));
                add("adapter",Adapter,typeof(DesktopProfileLaunchAdapterV1),"_launchReceiptWitness",typeof(DesktopProfileLaunchReceiptV1?));
                result.Add("adapter.receipt",Adapter.CurrentReceipt);
                foreach (var name in new[] {"_actions","_resetOldActions","_resetNewActions"}) add("router",Router,typeof(InputRouter),name,typeof(GameInputActions));
                foreach (var name in new[] {"_resetCutoverTerminal","_captureSuppressed","_faulted","_disposed","_actionsClosed"}) add("router",Router,typeof(InputRouter),name,typeof(bool));
                foreach (var name in new[] {"_resetCutoverGeneration","_modeEpoch","_frameOrdinal"}) add("router",Router,typeof(InputRouter),name,typeof(long));
                add("router",Router,typeof(InputRouter),"_resetCutoverLifecycle",ExactType(typeof(InputRouter).FullName+"+ResetCutoverLifecycleV1","AcadeGameMaker.Input.Unity"));
                add("router",Router,typeof(InputRouter),"_mode",typeof(AcadeGameMaker.Core.InputMode));
                result.Add("router.receipt",Router.CurrentReceipt); result.Add("router.uiFrame",Router.CurrentUiFrame);
                result.Add("latch.handoff",Latch.CurrentHandoff);
                foreach (var name in new[] {"_notification","_retainedFailureNotification"}) add("latch",Latch,typeof(HubEntryHandoffLatchV1),name,typeof(ProfileLaunchNotificationV1?));
                foreach (var name in new[] {"_handoff","_handoffProof"}) add("presenter",Presenter,PresenterType,name,typeof(HubEntryHandoffReceiptV1?));
                add("presenter",Presenter,PresenterType,"_notification",typeof(ProfileLaunchNotificationV1?));
                add("presenter",Presenter,PresenterType,"_retainedIntent",typeof(HubMenuIntentV1?));
                add("handoff",HandoffOwner,HandoffType,"_takenRequest",typeof(Nullable<>).MakeGenericType(Hub("HubMenuIntentRequestV1")));
                add("handoff",HandoffOwner,HandoffType,"_epochs",Hub("HubMenuIntentHandoffOwnerV1+TakenEpochRecord"));
                add("owner",Owner,OwnerType,"_display",typeof(ProfileNewGameCaptureResultV1));
                add("owner",Owner,OwnerType,"_acceptedIssued",IssuedType); add("owner",Owner,OwnerType,"_history",Hub("NewGameConfirmationOwnerV1+EpochHistory"));
                add("owner",Owner,OwnerType,"_acceptedRequest",typeof(Nullable<>).MakeGenericType(Hub("HubMenuIntentRequestV1"))); add("owner",Owner,OwnerType,"_epochToken",typeof(object));
                var actions=(GameInputActions)result["router._actions"]; result.Add("input.asset",actions.asset); result.Add("input.asset.json",actions.asset.ToJson()); result.Add("input.asset.mask",actions.asset.bindingMask);
                foreach (var map in new[] {actions.Gameplay.Get(),actions.UI.Get()})
                {
                    var prefix="input."+map.name; result.Add(prefix+".map",map); result.Add(prefix+".enabled",map.enabled); result.Add(prefix+".mask",map.bindingMask);
                    foreach (var action in map.actions)
                    { var key=prefix+"."+action.name; result.Add(key+".action",action); result.Add(key+".enabled",action.enabled); result.Add(key+".mask",action.bindingMask); for (var i=0;i<action.bindings.Count;i++) { var binding=action.bindings[i]; result.Add(key+".binding."+i,binding.id+"|"+binding.path+"|"+binding.effectivePath+"|"+binding.overridePath+"|"+binding.overrideInteractions+"|"+binding.overrideProcessors); } }
                }
                var session=(CurrentSessionProfileV1)result["adapter._currentSession"];
                result.Add("memory.actualCellAbsent",session==null);
                if (session!=null) { result.Add("memory.generation",session.Generation); result.Add("memory.canonical",session.CopyCanonicalDocument()); result.Add("memory.settings",session.Settings); result.Add("memory.input",session.Input); result.Add("memory.tutorial",session.Tutorial); result.Add("memory.progression",session.Progression); }
                result.Add("scene.count",SceneManager.sceneCount); result.Add("scene.active",SceneManager.GetActiveScene().handle);
                for (var i=0;i<SceneManager.sceneCount;i++) { var scene=SceneManager.GetSceneAt(i); result.Add("scene."+i,scene.handle+"|"+scene.path+"|"+scene.name+"|"+scene.isLoaded); }
                foreach (var entry in new[] {Owner,Presenter,HandoffOwner,(object)Router,Adapter,Latch}) { var component=(Component)entry; var key="object."+entry.GetType().FullName; result.Add(key,entry); result.Add(key+".active",component.gameObject.activeInHierarchy); result.Add(key+".scene",component.gameObject.scene.handle); result.Add(key+".enabled",((Behaviour)component).enabled); }
                foreach (var item in new[] {new[] {"_player","AcadeGameMaker.Movement.PlayerMovementController, AcadeGameMaker.Movement.Unity"},new[] {"_transfer","AcadeGameMaker.Transfer.Unity.TransferSimulationDriver, AcadeGameMaker.Transfer.Unity"},new[] {"_combat","AcadeGameMaker.Combat.Unity.OrdanBossCombatSimulationDriver, AcadeGameMaker.Combat.Unity"},new[] {"_terminalTeardown","AcadeGameMaker.Combat.Unity.OrdanBossTerminalTeardown, AcadeGameMaker.Combat.Unity"}})
                { var type=Type.GetType(item[1],true); add("execution",Router,typeof(InputRouter),item[0],type); var value=result["execution."+item[0]]; result.Add("execution."+item[0]+".active",value!=null && ((Component)value).gameObject.activeInHierarchy); }
                add("execution",Router,typeof(InputRouter),"_cameraProviderBehaviour",typeof(MonoBehaviour));
                var camera=(MonoBehaviour)result["execution._cameraProviderBehaviour"]; result.Add("execution.camera.active",camera!=null && camera.isActiveAndEnabled);
                return result;
            }
            internal void SelectInitialNewGame()
            {
                Publish();
                PresenterFixed();
                if (_recovered) { Publish(Key.Enter); PresenterFixed(); Publish(); PresenterFixed(); }
                else
                {
                    Publish(Key.DownArrow);
                    Assert.That(Router.CurrentUiFrame.Value.NavigateChanged,Is.True);
                    Assert.That(Router.CurrentUiFrame.Value.NavigateYQ4096,Is.LessThan(0));
                    PresenterFixed(); Publish(); PresenterFixed();
                }
                Publish(Key.Enter); Assert.That(Router.CurrentUiFrame.Value.SubmitPressed,Is.True); PresenterFixed(); Late();
                Assert.That(HandoffState,Is.EqualTo("RequestReady"),"actual UI selection reaches Q-B");
                var requestType=Hub("HubMenuIntentRequestV1");
                var actualRequest=Opaque(SnapshotField(HandoffOwner,HandoffType,"_request",typeof(Nullable<>).MakeGenericType(requestType)),requestType);
                Assert.That((HubMenuItemV1)SnapshotField(actualRequest,requestType,"_item",typeof(HubMenuItemV1)),Is.EqualTo(HubMenuItemV1.NewGame));
                Publish(); PresenterFixed();
            }
            internal bool TryTake(out object handle)
            {
                var args=new object[] { Owner,Presenter,Router,null };
                var taken=(bool)Invoke(HandoffOwner,HandoffType,"TryTakeNewGameForConfirmation",typeof(bool),new[] { OwnerType,PresenterType,typeof(InputRouter),IssuedType.MakeByRefType() },args);
                handle=args[3]; if (taken) Opaque(handle,IssuedType); else Assert.That(handle,Is.Null);
                return taken;
            }
            internal object Take() { Assert.That(TryTake(out var handle),Is.True); return Opaque(handle,IssuedType); }
            internal object Accept(object handle)
            { var actual=Opaque(handle,IssuedType); Assert.That(ReferenceEquals(actual,handle),Is.True); return Opaque(Invoke(Owner,OwnerType,"AcceptNewGame",ResultType,new[] { IssuedType,typeof(DesktopProfileLaunchAdapterV1),typeof(InputRouter) },actual,Adapter,Router),ResultType); }
            internal object Decision(object result) { return Opaque(Property(Opaque(result,ResultType),ResultType,"DecisionCapability",DecisionType),DecisionType); }
            internal string Outcome(object result) { return Property(Opaque(result,ResultType),ResultType,"Outcome",Hub("NewGameConfirmationOutcomeV1")).ToString(); }
            internal object[] InitialHistory()
            {
                return new[] {
                    SnapshotField(Presenter,PresenterType,"_cursor",typeof(UiSemanticFrameCursorV1)),
                    SnapshotField(Presenter,PresenterType,"_retainedIntent",typeof(HubMenuIntentV1?)),
                    SnapshotField(HandoffOwner,HandoffType,"_takenRequest",typeof(Nullable<>).MakeGenericType(Hub("HubMenuIntentRequestV1")))
                };
            }
            internal object RearmCapability(object result) { return Opaque(Property(Opaque(result,ResultType),ResultType,"RearmCapability",RearmType),RearmType); }
            internal object Confirmed(object result) { return Opaque(Property(Opaque(result,ResultType),ResultType,"ConfirmedRequest",typeof(ConfirmedProfileResetRequestV1)),typeof(ConfirmedProfileResetRequestV1)); }
            internal object Confirm(object decision) { return Opaque(Invoke(Owner,OwnerType,"Confirm",ResultType,new[] { DecisionType },Opaque(decision,DecisionType)),ResultType); }
            internal object Cancel(object decision) { return Opaque(Invoke(Owner,OwnerType,"Cancel",ResultType,new[] { DecisionType },Opaque(decision,DecisionType)),ResultType); }
            internal void Rearm(object capability) { Invoke(Owner,OwnerType,"Rearm",typeof(void),new[] { RearmType },Opaque(capability,RearmType)); }
            internal object Commit(object request) { return Invoke(Owner,OwnerType,"CommitForExecution",typeof(ConfirmedProfileResetRequestV1),new[] { typeof(ConfirmedProfileResetRequestV1) },Opaque(request,typeof(ConfirmedProfileResetRequestV1))); }
            internal object Field(object target,string name)
            {
                // Read-only forensic assertions; no field assignment or proof manufacture.
                var field=target.GetType().GetField(name,BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.NonPublic);
                if (field == null || field.DeclaringType != target.GetType()) throw new MissingFieldException(name);
                return field.GetValue(target);
            }
            public void Dispose()
            {
                if (_host != null) UnityEngine.Object.DestroyImmediate(_host);
                if (_keyboard != null && _keyboard.added) InputSystem.RemoveDevice(_keyboard);
                var root=Path.GetFullPath(Root); var temp=Path.GetFullPath(Path.GetTempPath());
                Assert.That(root.StartsWith(temp,StringComparison.OrdinalIgnoreCase) && Path.GetFileName(root).StartsWith("AcadeGameMaker-C3-play-",StringComparison.Ordinal) && !File.GetAttributes(root).HasFlag(FileAttributes.ReparsePoint),Is.True);
                if (Directory.Exists(root)) Directory.Delete(root,true);
            }
            private sealed class EnvironmentPort : IDesktopProfileLaunchEnvironmentV1
            { private readonly string _root; internal EnvironmentPort(string root) { _root=root; } public string GetPersistentDataPath() { return _root; } public DateTimeOffset GetUtcNow() { return new DateTimeOffset(2026,9,29,0,0,0,TimeSpan.Zero); } }
        }
    }
}
