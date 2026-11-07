using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Globalization;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Profile;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using RouterTests = AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests;

namespace AcadeGameMaker.Input.Unity.PlayMode.Tests
{
    // AC-M5D7M-003/005/008: local, engine-free proof and reservation guards.
    // Lifecycle integration remains a PlayMode execution item in the evidence.
    internal sealed class DesktopProfileLaunchAdapterV1Tests
    {
        [Test]
        public void PreparedHubProof_ValidatesClosedUiOnlyMatrix()
        {
            var owner = new object();
            var proof = new PreparedHubInitializationProofV1(owner, InputMode.UIOnly, false, true, true, true);
            Assert.That(proof.IsOwnedBy(owner), Is.True);
            Assert.That(proof.Mode, Is.EqualTo(InputMode.UIOnly));
            Assert.That(proof.GameplayEnabled, Is.False);
            Assert.That(proof.UiEnabled, Is.True);
            Assert.That(proof.CallbacksRegistered && proof.Initialized && proof.PendingPublication, Is.True);
        }

        [Test]
        public void PreparedHubProof_ReflectionCorruptionFailsClosed()
        {
            var proof = new PreparedHubInitializationProofV1(new object(), InputMode.UIOnly, false, true, true, true);
            var boxed = (object)proof;
            typeof(PreparedHubInitializationProofV1).GetField("_ui", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, false);
            Assert.That(() => ((PreparedHubInitializationProofV1)boxed).Validate(), Throws.InvalidOperationException);
        }

        [Test]
        public void Reservation_RejectsForeignOwnerWithoutMutatingWinner()
        {
            var go = new GameObject("m5d7m-router");
            try
            {
                go.SetActive(false);
                var router = go.AddComponent<InputRouter>();
                var winner = new object();
                router.ReservePreparedHubLaunch(winner);
                Assert.That(() => router.ReservePreparedHubLaunch(new object()), Throws.InvalidOperationException);
                Assert.That(() => router.FailPreparedHubLaunch(new object()), Throws.InvalidOperationException);
                router.FailPreparedHubLaunch(winner);
            }
            finally { NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>()); UnityEngine.Object.DestroyImmediate(go); }
        }

        [Test]
        public void Reservation_BlocksEveryLegacyInitializationAndConfigurationSeam()
        {
            var go = new GameObject("m5d7m-reserved-router");
            try
            {
                go.SetActive(false);
                var router = go.AddComponent<InputRouter>();
                router.ReservePreparedHubLaunch(new object());
                Assert.That(() => router.InitializeForTests(), Throws.InvalidOperationException);
                Assert.That(() => router.ConfigureForAuthoring(null, null, null, null, InputMode.UIOnly), Throws.InvalidOperationException);
                Assert.That(() => router.ConfigureTerminalTeardownForAuthoring(null), Throws.InvalidOperationException);
                Assert.That(router.EffectiveMode, Is.EqualTo(InputMode.Transition));
            }
            finally { NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>()); UnityEngine.Object.DestroyImmediate(go); }
        }

        [Test]
        public void PreparedHubProof_EachClosedFactIsRequired()
        {
            var names = new[] { "_owner", "_mode", "_gameplay", "_ui", "_callbacks", "_initialized", "_pendingPublication" };
            foreach (var name in names)
            {
                var boxed = (object)new PreparedHubInitializationProofV1(new object(), InputMode.UIOnly, false, true, true, true);
                var field = typeof(PreparedHubInitializationProofV1).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
                field.SetValue(boxed, name == "_owner" ? null : name == "_mode" ? (object)InputMode.GameplayEnabled : name == "_gameplay" ? true : false);
                Assert.That(() => ((PreparedHubInitializationProofV1)boxed).Validate(), Throws.InvalidOperationException, name);
            }
        }

        [TestCase(DesktopProfileLaunchAdapterFaultStageV1.AfterInitializeProof)]
        [TestCase(DesktopProfileLaunchAdapterFaultStageV1.AfterReceiptStaging)]
        [TestCase(DesktopProfileLaunchAdapterFaultStageV1.AfterNotificationStaging)]
        [TestCase(DesktopProfileLaunchAdapterFaultStageV1.BeforeConfirm)]
        public void AdapterFaultStages_CloseThePendingPublicationRouter(DesktopProfileLaunchAdapterFaultStageV1 stage)
        {
            var context = CreateLiveAdapter();
            try
            {
                var candidate = GetPrivate<GameInputActions>(context.Router, "_actions");
                context.Adapter.SetFaultStageForTests(stage);
                var error = Assert.Throws<TargetInvocationException>(() => Invoke(context.Adapter, "Start"));
                Assert.That(error.InnerException, Is.TypeOf<InvalidOperationException>());
                Assert.That(error.InnerException.Message, Is.EqualTo("Injected desktop launch adapter fault."));
                Assert.That(context.Adapter.CurrentReceipt, Is.Null);
                ProfileLaunchNotificationV1 ignored;
                Assert.That(context.Adapter.TryTakeNotification(out ignored), Is.False);
                Assert.That(GetPrivate<GameInputActions>(context.Router, "_actions"), Is.Null);
                Assert.That(GetPrivate<GameInputActions>(context.Router, "_lastClosedActions"), Is.SameAs(candidate));
                Assert.That(Convert.ToInt32(GetPrivate<object>(context.Router, "_preparedLaunchState")), Is.EqualTo(5));
                Assert.That(GetPrivate<bool>(context.Router, "_faulted"), Is.True);
                Assert.That(GetPrivate<bool>(context.Router, "_actionsClosed"), Is.True);
                Assert.That(candidate.Gameplay.Get().enabled, Is.False);Assert.That(candidate.UI.Get().enabled, Is.False);
                Assert.That(GetPrivate<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                Assert.That(GetPrivate<int>(context.Router, "_disposeAttempts"), Is.EqualTo(1));
                Assert.That(GetPrivate<int>(context.Router, "_gameplayUnsubscribeAttempts"), Is.EqualTo(1));
                Assert.That(GetPrivate<int>(context.Router, "_uiUnsubscribeAttempts"), Is.EqualTo(1));
                Invoke(context.Router,"OnDisable");Invoke(context.Router,"OnDestroy");
                Assert.That(GetPrivate<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                Assert.That(GetPrivate<int>(context.Router, "_disposeAttempts"), Is.EqualTo(1));
            }
            finally { context.Dispose(); }
        }

        [TestCase(InputRouter.PreparedLaunchFaultInjectionStage.AfterGameplayCallbackRegistration)]
        [TestCase(InputRouter.PreparedLaunchFaultInjectionStage.BeforeUiCallbackRegistration)]
        [TestCase(InputRouter.PreparedLaunchFaultInjectionStage.AfterUiCallbackRegistration)]
        public void RouterCallbackFaultStages_CloseTheAdoptedCandidate(InputRouter.PreparedLaunchFaultInjectionStage stage)
        {
            var context = CreateLiveAdapter();
            try
            {
                var candidate = GetPrivate<GameInputActions>(context.Router, "_actions");
                context.Router.SetPreparedLaunchFaultStageForTests(stage);
                var error = Assert.Throws<TargetInvocationException>(() => Invoke(context.Adapter, "Start"));
                Assert.That(error.InnerException, Is.TypeOf<InvalidOperationException>());
                Assert.That(error.InnerException.Message, Is.EqualTo("Injected prepared launch callback fault."));
                Assert.That(context.Adapter.CurrentReceipt, Is.Null);
                ProfileLaunchNotificationV1 ignored; Assert.That(context.Adapter.TryTakeNotification(out ignored), Is.False);
                Assert.That(GetPrivate<GameInputActions>(context.Router, "_actions"), Is.Null);
                Assert.That(GetPrivate<GameInputActions>(context.Router, "_lastClosedActions"), Is.SameAs(candidate));
                Assert.That(context.Router.PreparedLaunchFaultStageForTests, Is.EqualTo(InputRouter.PreparedLaunchFaultStage.CallbackRegistration));
                Assert.That(Convert.ToInt32(GetPrivate<object>(context.Router, "_preparedLaunchState")), Is.EqualTo(5));
                Assert.That(GetPrivate<bool>(context.Router, "_faulted"), Is.True);
                Assert.That(GetPrivate<bool>(context.Router, "_actionsClosed"), Is.True);
                Assert.That(GetPrivate<bool>(context.Router, "_gameplayCallbacksRegistered"), Is.False);
                Assert.That(GetPrivate<bool>(context.Router, "_uiCallbacksRegistered"), Is.False);
                Assert.That(candidate.Gameplay.Get().enabled, Is.False);
                Assert.That(candidate.UI.Get().enabled, Is.False);
                Assert.That(GetPrivate<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                Assert.That(GetPrivate<int>(context.Router, "_disposeAttempts"), Is.EqualTo(1));
                Assert.That(GetPrivate<int>(context.Router, "_gameplayUnsubscribeAttempts"), Is.EqualTo(1));
                Assert.That(GetPrivate<int>(context.Router, "_uiUnsubscribeAttempts"), Is.EqualTo(stage == InputRouter.PreparedLaunchFaultInjectionStage.AfterGameplayCallbackRegistration ? 0 : 1));
                Invoke(context.Router, "OnDisable"); Invoke(context.Router, "OnDestroy");
                Assert.That(GetPrivate<int>(context.Router, "_closeAttempts"), Is.EqualTo(1));
                Assert.That(GetPrivate<int>(context.Router, "_disposeAttempts"), Is.EqualTo(1));
                AssertIndependentLegacyRouterStarts();
            }
            finally { context.Dispose(); }
        }

        [TestCase(InputMode.GameplayEnabled)]
        [TestCase(InputMode.UIOnly)]
        [TestCase(InputMode.Cutscene)]
        [TestCase(InputMode.Transition)]
        [TestCase(InputMode.Ended)]
        public void PreparedHubProof_RejectsEveryNonUiOnlyMode(InputMode mode)
        {
            if (mode == InputMode.UIOnly)
                Assert.That(() => new PreparedHubInitializationProofV1(new object(), mode, false, true, true, true), Throws.Nothing);
            else
                Assert.That(() => new PreparedHubInitializationProofV1(new object(), mode, false, true, true, true), Throws.InvalidOperationException);
        }

        [Test]
        public void AdapterAwake_UsesOneInjectedEnvironmentAndPreparationInExactOrder()
        {
            var root = Path.Combine(Path.GetTempPath(), "m5d7m-adapter-" + Guid.NewGuid().ToString("N"));
            var go = new GameObject("m5d7m-adapter-order-root");
            try
            {
                go.SetActive(false);
                var routerObject = new GameObject("m5d7m-adapter-order-router"); routerObject.transform.SetParent(go.transform);
                var adapterObject = new GameObject("m5d7m-adapter-order-adapter"); adapterObject.transform.SetParent(go.transform);
                var router = routerObject.AddComponent<InputRouter>();
                router.ConfigureForAuthoring(null, null, null, null, InputMode.UIOnly);
                var adapter = adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>();
                var environment = new RecordingEnvironment(root, new DateTimeOffset(2026, 9, 13, 0, 0, 0, TimeSpan.Zero));
                adapter.ConfigureForTests(router, environment, new RecordingPreparation(environment));
                go.SetActive(true);
                Assert.That(environment.Calls, Is.EqualTo("root,utc,prepare"));
                Assert.That(environment.PreparationRoot, Is.EqualTo(Path.GetFullPath(root)));
                Assert.That(environment.PreparationUtc, Is.EqualTo(environment.Utc));
            }
            finally { UnityEngine.Object.DestroyImmediate(go); if (Directory.Exists(root)) Directory.Delete(root, true); }
        }

        [Test]
        public void SameCohort_DefaultBootstrap_PublishesUiOnlyReceiptAndOneRecoveryNotification()
        {
            var context = CreateLiveAdapter();
            try
            {
                Invoke(context.Adapter, "Start"); Invoke(context.Router, "Start");
                Assert.That(context.Adapter.CurrentReceipt.HasValue, Is.True);
                var receipt = context.Adapter.CurrentReceipt.Value;
                Assert.That(receipt.Source, Is.EqualTo(AcadeGameMaker.Profile.ProfileLaunchSource.Default));
                Assert.That(receipt.InputMode, Is.EqualTo(InputMode.UIOnly));
                var actions = GetPrivate<GameInputActions>(context.Router, "_actions");
                Assert.That(actions.UI.Get().enabled, Is.True); Assert.That(actions.Gameplay.Get().enabled, Is.False);
                ProfileLaunchNotificationV1 notification;
                Assert.That(context.Adapter.TryTakeNotification(out notification), Is.True);
                Assert.That(notification.Kind, Is.EqualTo(ProfileLaunchNotificationKindV1.RecoveryCompleted));
                Assert.That(context.Adapter.TryTakeNotification(out notification), Is.False);
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void RouterStartBeforeAdapterStart_IsInertThenAdapterInitializesSuccessfully()
        {
            var context = CreateLiveAdapter();
            try
            {
                Invoke(context.Router, "Start");
                Assert.That(context.Adapter.CurrentReceipt, Is.Null);
                Assert.That(GetPrivate<GameInputActions>(context.Router, "_actions"), Is.Not.Null);
                Invoke(context.Adapter, "Start");
                Assert.That(context.Adapter.CurrentReceipt.HasValue, Is.True);
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void LegacyUnreservedRouter_AwakeDoesNotAllocateActions()
        {
            var go = new GameObject("m5d7m-legacy-router");
            try
            {
                go.SetActive(false); var router = go.AddComponent<InputRouter>(); go.SetActive(true);
                Assert.That(GetPrivate<GameInputActions>(router, "_actions"), Is.Null);
                Invoke(router, "OnDestroy");
                Assert.That(GetPrivate<GameInputActions>(router, "_actions"), Is.Null);
            }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }

        [Test]
        public void LegacyUnreservedRouter_StartAllocatesAndInitializesTheAuthoredGraph()
        {
            var context = CreateLegacyRouter();
            try
            {
                Assert.That(GetPrivate<GameInputActions>(context.Router, "_actions"), Is.Null);
                Invoke(context.Router, "Start");
                Assert.That(GetPrivate<GameInputActions>(context.Router, "_actions"), Is.Not.Null);
                Assert.That(context.Router.EffectiveMode, Is.EqualTo(InputMode.UIOnly));
            }
            finally { context.Dispose(); }
        }

        [TestCase(true, true)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public void LifecyclePermutations_PublishAfterRouterAndAdapterAwake(bool routerCreatedFirst, bool sameObject)
        {
            var context = CreateLiveAdapter(routerCreatedFirst, sameObject);
            try
            {
                Invoke(context.Adapter, "Start"); Invoke(context.Router, "Start");
                Assert.That(context.Adapter.CurrentReceipt.HasValue, Is.True);
                Assert.That(context.Router.EffectiveMode, Is.EqualTo(InputMode.UIOnly));
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void InitializeForTests_PreAwakeAndReserveAfterLegacyStartReject()
        {
            var inactive = new GameObject("m5d7m-preawake");
            try { inactive.SetActive(false); var router=inactive.AddComponent<InputRouter>(); Assert.That(() => router.InitializeForTests(), Throws.InvalidOperationException); }
            finally { UnityEngine.Object.DestroyImmediate(inactive); }
            var legacy=CreateLegacyRouter();
            try { Invoke(legacy.Router,"Start"); Assert.That(() => legacy.Router.ReservePreparedHubLaunch(new object()), Throws.InvalidOperationException); }
            finally { legacy.Dispose(); }
        }

        [Test]
        public void InitializeForTests_IsIdempotentOnlyForHealthyInitializedLegacyRouter()
        {
            var healthy=CreateLegacyRouter();
            try
            {
                healthy.Router.InitializeForTests();var actions=GetPrivate<GameInputActions>(healthy.Router,"_actions");
                healthy.Router.InitializeForTests();Assert.That(GetPrivate<GameInputActions>(healthy.Router,"_actions"),Is.SameAs(actions));
                Invoke(healthy.Router,"OnDisable");Assert.That(()=>healthy.Router.InitializeForTests(),Throws.InvalidOperationException);
            }
            finally{healthy.Dispose();}
            var disposed=CreateLegacyRouter();
            try
            {
                disposed.Router.InitializeForTests();Invoke(disposed.Router,"OnDestroy");
                Assert.That(()=>disposed.Router.InitializeForTests(),Throws.InvalidOperationException);
            }
            finally{disposed.Dispose();}
        }

        [Test]
        public void InitializedPendingPublication_DisableClosesTheExactAdoptedActions()
        {
            var context=CreateLiveAdapter();
            try { context.Router.InitializePreparedHub(context.Adapter); Invoke(context.Router,"OnDisable"); Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"), Is.Null); Assert.That(GetPrivate<int>(context.Router,"_closeAttempts"),Is.EqualTo(1)); Assert.That(GetPrivate<int>(context.Router,"_disposeAttempts"),Is.EqualTo(1)); Assert.That(GetPrivate<int>(context.Router,"_gameplayUnsubscribeAttempts"),Is.EqualTo(1)); Assert.That(GetPrivate<int>(context.Router,"_uiUnsubscribeAttempts"),Is.EqualTo(1)); }
            finally { context.Dispose(); }
        }

        [Test]
        public void PublishedReceipt_RejectsReflectionMutationOfEveryPrivateEvidenceField()
        {
            var context = CreateLiveAdapter();
            try
            {
                Invoke(context.Adapter, "Start");
                var fields = typeof(DesktopProfileLaunchReceiptV1).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    var boxed = (object)context.Adapter.CurrentReceipt.Value;
                    field.SetValue(boxed, CorruptReceiptField(field.Name));
                    Assert.That(() => ((DesktopProfileLaunchReceiptV1)boxed).Validate(), Throws.InvalidOperationException, field.Name);
                }
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void PublishedNotification_RejectsReceiptCorrelationAndKindMutation()
        {
            var context = CreateLiveAdapter();
            try
            {
                Invoke(context.Adapter, "Start"); ProfileLaunchNotificationV1 notification;
                Assert.That(context.Adapter.TryTakeNotification(out notification), Is.True);
                foreach (var field in typeof(ProfileLaunchNotificationV1).GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    var boxed = (object)notification;
                    field.SetValue(boxed, field.Name == "_kind" ? (object)ProfileLaunchNotificationKindV1.PersistenceDeferred : default(DesktopProfileLaunchReceiptV1));
                    Assert.That(() => ((ProfileLaunchNotificationV1)boxed).Validate(), Throws.InvalidOperationException, field.Name);
                }
            }
            finally { context.Dispose(); }
        }

        [Test]
        public void PublishedNotification_RejectsEveryNestedCorrelationReceiptMutation()
        {
            var context=CreateLiveAdapter();
            try
            {
                Invoke(context.Adapter,"Start");ProfileLaunchNotificationV1 notification;Assert.That(context.Adapter.TryTakeNotification(out notification),Is.True);
                foreach(var field in typeof(DesktopProfileLaunchReceiptV1).GetFields(BindingFlags.Instance|BindingFlags.NonPublic))
                {
                    var boxedNotification=(object)notification;
                    var correlation=(object)typeof(ProfileLaunchNotificationV1).GetField("_correlation",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(boxedNotification);
                    var replacement=CorruptReceiptField(field.Name);field.SetValue(correlation,replacement);
                    typeof(ProfileLaunchNotificationV1).GetField("_correlation",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(boxedNotification,(DesktopProfileLaunchReceiptV1)correlation);
                    Assert.That(()=>((ProfileLaunchNotificationV1)boxedNotification).Validate(),Throws.InvalidOperationException,"correlation."+field.Name);
                }
            }
            finally{context.Dispose();}
        }

        [Test]
        public void CurrentReceipt_GetterRevalidatesItsPublishedBackingValue()
        {
            var context=CreateLiveAdapter();
            DesktopProfileLaunchReceiptV1? original=null;
            try
            {
                Invoke(context.Adapter,"Start");
                original=context.Adapter.CurrentReceipt;var boxed=(object)original.Value;
                typeof(DesktopProfileLaunchReceiptV1).GetField("_hubProof",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(boxed,false);
                typeof(DesktopProfileLaunchAdapterV1).GetField("_currentReceipt",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(context.Adapter,(DesktopProfileLaunchReceiptV1?)((DesktopProfileLaunchReceiptV1)boxed));
                Assert.That(() => { var ignored=context.Adapter.CurrentReceipt; }, Throws.InvalidOperationException);
            }
            finally
            {
                if(original.HasValue)typeof(DesktopProfileLaunchAdapterV1).GetField("_currentReceipt",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(context.Adapter,original);
                context.Dispose();
            }
        }

        [Test]
        public void RouterPreparedState_UnknownBeforeAwakeAndAfterAwakeFailsClosed()
        {
            var go=new GameObject("m5d7m-state-corruption");
            try
            {
                go.SetActive(false); var router=go.AddComponent<InputRouter>();
                typeof(InputRouter).GetField("_preparedLaunchState",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(router,99);
                Assert.That(() => Invoke(router,"Awake"), Throws.InstanceOf<TargetInvocationException>());
                Assert.That(GetPrivate<GameInputActions>(router,"_actions"), Is.Null);
            }
            finally { NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>()); UnityEngine.Object.DestroyImmediate(go); }
        }

        [Test]
        public void AdoptionBoundary_RejectsNullEnabledAndRetakeWithoutTakingCallerOwnership()
        {
            var go=new GameObject("m5d7m-adoption-boundary");
            var enabled=new GameInputActions();
            try
            {
                go.SetActive(false);var router=go.AddComponent<InputRouter>();var owner=new object();router.ReservePreparedHubLaunch(owner);
                Assert.That(()=>router.AdoptPreparedHubActions(owner,null),Throws.InvalidOperationException);
                enabled.Gameplay.Enable();Assert.That(()=>router.AdoptPreparedHubActions(owner,enabled),Throws.InvalidOperationException);Assert.That(enabled.Gameplay.Get().enabled,Is.True);enabled.Gameplay.Disable();
                router.AdoptPreparedHubActions(owner,enabled);Assert.That(()=>router.AdoptPreparedHubActions(owner,enabled),Throws.InvalidOperationException);Assert.That(GetPrivate<GameInputActions>(router,"_actions"),Is.SameAs(enabled));
            }
            finally{UnityEngine.Object.DestroyImmediate(go);}
        }

        [Test]
        public void AdapterWrongInitialMode_RejectsBeforePreparationPortRuns()
        {
            var root=Path.Combine(Path.GetTempPath(),"m5d7m-mode-"+Guid.NewGuid().ToString("N"));var parent=new GameObject("m5d7m-mode-parent");
            try
            {
                parent.SetActive(false);var routerObject=new GameObject("router");routerObject.transform.SetParent(parent.transform);var adapterObject=new GameObject("adapter");adapterObject.transform.SetParent(parent.transform);var router=routerObject.AddComponent<InputRouter>();router.ConfigureForAuthoring(null,null,null,null,InputMode.Transition);var adapter=adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>();var environment=new RecordingEnvironment(root,new DateTimeOffset(2026,9,13,0,0,0,TimeSpan.Zero));adapter.ConfigureForTests(router,environment,new ThrowingPreparation());
                Assert.That(()=>Invoke(adapter,"Awake"),Throws.InstanceOf<TargetInvocationException>());Assert.That(environment.Calls,Is.Null.Or.Empty);
            }
            finally{UnityEngine.Object.DestroyImmediate(parent);}
        }

        [TestCase(LaunchFailureKindV1.RootThrow)]
        [TestCase(LaunchFailureKindV1.RootThrowIOException)]
        [TestCase(LaunchFailureKindV1.RootThrowUnauthorized)]
        [TestCase(LaunchFailureKindV1.RootThrowArgument)]
        [TestCase(LaunchFailureKindV1.RootNull)]
        [TestCase(LaunchFailureKindV1.RootRelative)]
        [TestCase(LaunchFailureKindV1.RootWhitespace)]
        [TestCase(LaunchFailureKindV1.RootFilesystemRoot)]
        [TestCase(LaunchFailureKindV1.UtcThrow)]
        [TestCase(LaunchFailureKindV1.UtcNonzero)]
        [TestCase(LaunchFailureKindV1.PreparationThrow)]
        [TestCase(LaunchFailureKindV1.PreparationNull)]
        public void PhaseD2_EnvironmentAndPreparationFailureStopsBeforeAdoption(LaunchFailureKindV1 kind)
        {
            var environment=ScriptedEnvironmentV1.For(kind);var preparation=new ScriptedFailurePreparationV1(kind);
            LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape(ExpectedAwakeFailure(kind))));
            var context=CreateFailureAdapter(environment,preparation);
            try
            {
            Assert.That(context.Error,Is.Null,"Unity logs MonoBehaviour Awake failures rather than returning them from SetActive.");
            Assert.That(preparation.Calls,Is.LessThanOrEqualTo(1));
            if(kind==LaunchFailureKindV1.RootThrow||kind==LaunchFailureKindV1.RootThrowIOException||kind==LaunchFailureKindV1.RootThrowUnauthorized||kind==LaunchFailureKindV1.RootThrowArgument||kind==LaunchFailureKindV1.RootNull||kind==LaunchFailureKindV1.RootRelative||kind==LaunchFailureKindV1.RootWhitespace||kind==LaunchFailureKindV1.RootFilesystemRoot)Assert.That(environment.UtcCalls,Is.EqualTo(0));
            if(kind==LaunchFailureKindV1.UtcThrow||kind==LaunchFailureKindV1.UtcNonzero)Assert.That(preparation.Calls,Is.EqualTo(0));
            Assert.That(context.Adapter.CurrentReceipt,Is.Null);ProfileLaunchNotificationV1 ignored;Assert.That(context.Adapter.TryTakeNotification(out ignored),Is.False);Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.Null);Assert.That(GetPrivate<bool>(context.Router,"_faulted"),Is.True);Assert.That(GetPrivate<bool>(context.Router,"_actionsClosed"),Is.True);Assert.That(GetPrivate<int>(context.Router,"_closeAttempts"),Is.EqualTo(0));Assert.That(GetPrivate<int>(context.Router,"_disposeAttempts"),Is.EqualTo(0));Assert.That(GetPrivate<int>(context.Router,"_gameplayUnsubscribeAttempts"),Is.EqualTo(0));Assert.That(GetPrivate<int>(context.Router,"_uiUnsubscribeAttempts"),Is.EqualTo(0));
            }
            finally{context.Dispose();}
        }

        [TestCase(TokenFailureKindV1.Malformed)]
        [TestCase(TokenFailureKindV1.PreConsumed)]
        [TestCase(TokenFailureKindV1.AdoptFailed)]
        public void PhaseD3_TokenAndAdoptionFailures_PreserveFailureAndCloseWithoutFallback(TokenFailureKindV1 kind)
        {
            LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape(ExpectedAwakeFailure(kind))));
            var context=CreateTokenFailureAdapter(kind);
            try
            {
                Assert.That(context.Error,Is.Null,"Unity logs MonoBehaviour Awake failures rather than returning them from SetActive.");Assert.That(context.Adapter.CurrentReceipt,Is.Null);ProfileLaunchNotificationV1 ignored;Assert.That(context.Adapter.TryTakeNotification(out ignored),Is.False);Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.Null);Assert.That(GetPrivate<bool>(context.Router,"_faulted"),Is.True);Assert.That(GetPrivate<bool>(context.Router,"_actionsClosed"),Is.True);Assert.That(GetPrivate<int>(context.Router,"_closeAttempts"),Is.EqualTo(0));
                Assert.That(context.Scenario.Actions.Created.Count,Is.EqualTo(1));
                if(kind==TokenFailureKindV1.Malformed)Assert.That(context.Scenario.Actions.Disposed.Count,Is.EqualTo(1));
                if(kind==TokenFailureKindV1.PreConsumed)Assert.That(context.PreConsumed,Is.Not.Null);
            }
            finally{context.Dispose();}
        }

        [TestCase("OnDisable")]
        [TestCase("OnDestroy")]
        public void PhaseD4_PreReceiptAdapterTeardown_ClosesAdoptedActionsExactlyOnce(string teardown)
        {
            var context=CreateLiveAdapter();
            try
            {
                var candidate=GetPrivate<GameInputActions>(context.Router,"_actions");Assert.That(candidate,Is.Not.Null);
                Invoke(context.Adapter,teardown);Invoke(context.Adapter,teardown);
                Assert.That(context.Adapter.CurrentReceipt,Is.Null);ProfileLaunchNotificationV1 ignored;Assert.That(context.Adapter.TryTakeNotification(out ignored),Is.False);Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.Null);Assert.That(GetPrivate<bool>(context.Router,"_faulted"),Is.True);Assert.That(GetPrivate<bool>(context.Router,"_actionsClosed"),Is.True);Assert.That(GetPrivate<int>(context.Router,"_closeAttempts"),Is.EqualTo(1));Assert.That(GetPrivate<int>(context.Router,"_disposeAttempts"),Is.EqualTo(1));Assert.That(GetPrivate<int>(context.Router,"_gameplayUnsubscribeAttempts"),Is.EqualTo(0));Assert.That(GetPrivate<int>(context.Router,"_uiUnsubscribeAttempts"),Is.EqualTo(0));
            }
            finally{context.Dispose();}
        }

        [Test]
        public void PhaseD4_DuplicateAdapters_LoserDoesNoPreparationAndCannotCloseWinner()
        {
            LogAssert.Expect(LogType.Exception, new Regex(Regex.Escape("InvalidOperationException: Prepared hub reservation is not available.")));
            var context=CreateDuplicateAdapter();
            try
            {
                var owner=GetPrivate<object>(context.Router,"_preparedLaunchOwner");var winningAdapter=ReferenceEquals(owner,context.First)?context.First:context.Second;Invoke(winningAdapter,"Start");
                var first=context.First.CurrentReceipt;var second=context.Second.CurrentReceipt;Assert.That(first.HasValue||second.HasValue,Is.True);Assert.That(first.HasValue&&second.HasValue,Is.False);
                var winner=first.HasValue?context.FirstScenario:context.SecondScenario;var loser=first.HasValue?context.SecondScenario:context.FirstScenario;Assert.That(loser.Actions.Created,Is.Empty);Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.SameAs(winner.Actions.Created[0]));
            }
            finally{context.Dispose();}
        }

        [Test]
        public void PhaseE1_CorruptClosedFlagCannotSuppressHeldActionCleanup()
        {
            var go=new GameObject("m5d7m-invariant-close");var actions=new GameInputActions();
            try
            {
                go.SetActive(false);var router=go.AddComponent<InputRouter>();typeof(InputRouter).GetField("_actions",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(router,actions);typeof(InputRouter).GetField("_actionsClosed",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(router,true);typeof(InputRouter).GetField("_preparedLaunchState",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(router,Enum.ToObject(typeof(InputRouter).GetField("_preparedLaunchState",BindingFlags.Instance|BindingFlags.NonPublic).FieldType,1));
                Assert.That(()=>Invoke(router,"Awake"),Throws.InstanceOf<TargetInvocationException>());Assert.That(GetPrivate<GameInputActions>(router,"_actions"),Is.Null);Assert.That(GetPrivate<int>(router,"_closeAttempts"),Is.EqualTo(1));
            }
            finally{NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>());UnityEngine.Object.DestroyImmediate(go);}
        }

        [TestCase("_preparedLaunchOwner",null)] [TestCase("_actions",true)]
        [TestCase("_gameplayCallbacksRegistered",true)] [TestCase("_uiCallbacksRegistered",true)]
        [TestCase("_initialized",true)] [TestCase("_startEntered",true)]
        [TestCase("_disposed",true)] [TestCase("_faulted",true)]
        [TestCase("_actionsClosed",true)] [TestCase("_preparedLaunchFaultStage",99)]
        public void PhaseE1b_LegalReservedBaseline_SingleFieldCorruptionIsRejected(string field,object replacement)
        {
            var go=new GameObject("m5d7m-reserved-baseline");
            try
            {
                go.SetActive(false);var router=go.AddComponent<InputRouter>();var owner=new object();router.ReservePreparedHubLaunch(owner);Invoke(router,"Awake");
                if(field=="_actions") replacement=new GameInputActions();
                SetPrivate(router,field,replacement);
                Assert.That(()=>router.StepForTests(),Throws.InvalidOperationException,field);
                Assert.That(GetPrivate<object>(router,"_preparedLaunchOwner"),Is.SameAs(field=="_preparedLaunchOwner"?null:owner),field);
                Assert.That(GetPrivate<GameInputActions>(router,"_actions"),Is.Null,field);
                Assert.That(GetPrivate<int>(router,"_closeAttempts"),Is.EqualTo(field=="_actions"?1:0),field);
            }finally{NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>());UnityEngine.Object.DestroyImmediate(go);}
        }

        [TestCase("_preparedLaunchOwner",null)] [TestCase("_actions",null)]
        [TestCase("_gameplayCallbacksRegistered",true)] [TestCase("_uiCallbacksRegistered",true)]
        [TestCase("_initialized",true)] [TestCase("_disposed",true)]
        [TestCase("_faulted",true)] [TestCase("_actionsClosed",true)]
        [TestCase("_preparedLaunchFaultStage",99)]
        public void PhaseE1b_LegalAdoptedBaseline_SingleFieldCorruptionIsRejected(string field,object replacement)
        {
            var go=new GameObject("m5d7m-adopted-baseline");var actions=new GameInputActions();
            try
            {
                go.SetActive(false);var router=go.AddComponent<InputRouter>();var owner=new object();router.ReservePreparedHubLaunch(owner);router.AdoptPreparedHubActions(owner,actions);Invoke(router,"Awake");SetPrivate(router,field,replacement);
                Assert.That(()=>router.StepForTests(),Throws.InvalidOperationException,field);Assert.That(GetPrivate<GameInputActions>(router,"_actions"),Is.Null,field);Assert.That(GetPrivate<int>(router,"_closeAttempts"),Is.EqualTo(field=="_actions"?0:1),field);
            }
            finally{if(GetPrivate<GameInputActions>(go.GetComponent<InputRouter>(),"_actions")==null&&field=="_actions")actions.Dispose();NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>());UnityEngine.Object.DestroyImmediate(go);}
        }

        [TestCase("gameplay")] [TestCase("ui")]
        public void PhaseE1b_LegalAdoptedBaseline_EnabledMapCorruptionIsRejected(string map)
        {
            var go=new GameObject("m5d7m-adopted-map-baseline");var actions=new GameInputActions();
            try{go.SetActive(false);var router=go.AddComponent<InputRouter>();var owner=new object();router.ReservePreparedHubLaunch(owner);router.AdoptPreparedHubActions(owner,actions);Invoke(router,"Awake");if(map=="gameplay")actions.Gameplay.Enable();else actions.UI.Enable();Assert.That(()=>router.StepForTests(),Throws.InvalidOperationException,map);Assert.That(GetPrivate<GameInputActions>(router,"_actions"),Is.Null);Assert.That(GetPrivate<int>(router,"_closeAttempts"),Is.EqualTo(1));}finally{NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>());UnityEngine.Object.DestroyImmediate(go);}
        }

        [TestCase("_preparedLaunchOwner", null)] [TestCase("_actions", null)]
        [TestCase("_gameplayCallbacksRegistered", false)] [TestCase("_uiCallbacksRegistered", false)]
        [TestCase("_initialized", false)] [TestCase("_awakeEntered", false)]
        [TestCase("_faulted", true)] [TestCase("_actionsClosed", true)]
        [TestCase("_preparedLaunchFaultStage", 99)] [TestCase("_mode", InputMode.GameplayEnabled)]
        public void PhaseE1b_LegalPendingBaseline_SingleFieldCorruptionIsRejected(string field,object replacement)
        {
            var context=CreateLiveAdapter();
            GameInputActions detached=null;GameInputActions held=null;
            try
            {
                context.Router.InitializePreparedHub(context.Adapter);
                held=GetPrivate<GameInputActions>(context.Router,"_actions");
                if(field=="_actions")detached=GetPrivate<GameInputActions>(context.Router,"_actions");
                SetPrivate(context.Router,field,replacement);
                Assert.That(()=>context.Router.StepForTests(),Throws.InvalidOperationException,field);
                Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.Null,field);
                Assert.That(context.Adapter.CurrentReceipt,Is.Null,field);
            }
            finally{if(detached!=null)DisposeActionsForTest(detached);if(held!=null&&!ReferenceEquals(held,detached))DisposeActionsForTest(held);NormalizeDisposedLegacyForCleanup(context.Router);context.Dispose();}
        }

        [TestCase("gameplay")] [TestCase("ui")]
        public void PhaseE1b_LegalPendingBaseline_EnabledMapCorruptionIsRejected(string map)
        {
            var context=CreateLiveAdapter();
            try
            {
                context.Router.InitializePreparedHub(context.Adapter);
                var actions=GetPrivate<GameInputActions>(context.Router,"_actions");
                if(map=="gameplay") actions.Gameplay.Enable(); else actions.UI.Disable();
                Assert.That(()=>context.Router.StepForTests(),Throws.InvalidOperationException,map);
                Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.Null,map);
                Assert.That(GetPrivate<int>(context.Router,"_closeAttempts"),Is.EqualTo(1),map);
            }
            finally{NormalizeDisposedLegacyForCleanup(context.Router);context.Dispose();}
        }

        [TestCase("_preparedLaunchOwner",null)] [TestCase("_faulted",false)]
        [TestCase("_actionsClosed",false)] [TestCase("_preparedLaunchFaultStage",0)]
        [TestCase("_actions",true)] [TestCase("_gameplayCallbacksRegistered",true)]
        [TestCase("_uiCallbacksRegistered",true)]
        public void PhaseE1b_LegalFailedBaseline_SingleFieldCorruptionIsRejected(string field,object replacement)
        {
            var go=new GameObject("m5d7m-failed-baseline");
            try
            {
                go.SetActive(false);var router=go.AddComponent<InputRouter>();var owner=new object();router.ReservePreparedHubLaunch(owner);router.FailPreparedHubLaunch(owner);
                if(field=="_actions") replacement=new GameInputActions();SetPrivate(router,field,replacement);
                Assert.That(()=>router.StepForTests(),Throws.InvalidOperationException,field);
                Assert.That(GetPrivate<GameInputActions>(router,"_actions"),Is.Null,field);
                Assert.That(GetPrivate<int>(router,"_closeAttempts"),Is.EqualTo(field=="_actions"?1:0),field);
            }
            finally{NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>());UnityEngine.Object.DestroyImmediate(go);}
        }

        [Test]
        public void PhaseE1b_DisposedHeldActions_AreContainedBeforeDestroyIdempotence()
        {
            var go=new GameObject("m5d7m-disposed-held");var actions=new GameInputActions();
            try
            {
                go.SetActive(false);var router=go.AddComponent<InputRouter>();Invoke(router,"Awake");
                SetPrivate(router,"_disposed",true);SetPrivate(router,"_actionsClosed",true);SetPrivate(router,"_actions",actions);
                Assert.That(()=>Invoke(router,"OnDestroy"),Throws.InstanceOf<TargetInvocationException>());
                Assert.That(GetPrivate<GameInputActions>(router,"_actions"),Is.Null);Assert.That(GetPrivate<int>(router,"_closeAttempts"),Is.EqualTo(1));Assert.That(GetPrivate<int>(router,"_disposeAttempts"),Is.EqualTo(1));
            }
            finally{NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>());UnityEngine.Object.DestroyImmediate(go);}
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PhaseE1b_UnreservedClosedFaultRows_CannotReinitializeOrReconfigure(bool startEntered)
        {
            var go=new GameObject("m5d7m-unreserved-guard");
            try
            {
                go.SetActive(false);var router=go.AddComponent<InputRouter>();Invoke(router,"Awake");SetPrivate(router,"_faulted",true);SetPrivate(router,"_actionsClosed",true);SetPrivate(router,"_startEntered",startEntered);
                Assert.That(()=>router.InitializeForTests(),Throws.InvalidOperationException);
                Assert.That(()=>router.ConfigureForAuthoring(null,null,null,null,InputMode.UIOnly),Throws.InvalidOperationException);
                Assert.That(()=>router.ConfigureTerminalTeardownForAuthoring(null),Throws.InvalidOperationException);
                Assert.That(GetPrivate<GameInputActions>(router,"_actions"),Is.Null);Assert.That(GetPrivate<int>(router,"_closeAttempts"),Is.EqualTo(0));
            }
            finally{NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>());UnityEngine.Object.DestroyImmediate(go);}
        }

        [TestCase("faulted-action-free-open")]
        [TestCase("live-uninitialized-no-callbacks")]
        [TestCase("live-nonfaulted-uninitialized")]
        [TestCase("closed-nonfaulted-action-free")]
        public void PhaseE1b_UnreservedLooseCombinations_AreRejectedBeforeFallback(string row)
        {
            var go=new GameObject("m5d7m-unreserved-illegal");
            try
            {
                go.SetActive(false);var router=go.AddComponent<InputRouter>();Invoke(router,"Awake");
                if(row=="faulted-action-free-open")SetPrivate(router,"_faulted",true);
                else if(row=="closed-nonfaulted-action-free")SetPrivate(router,"_actionsClosed",true);
                else { SetPrivate(router,"_actions",new GameInputActions());if(row=="live-uninitialized-no-callbacks")SetPrivate(router,"_faulted",true); }
                Assert.That(()=>router.StepForTests(),Throws.InvalidOperationException,row);Assert.That(GetPrivate<GameInputActions>(router,"_actions"),Is.Null,row);
            }
            finally{NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>());UnityEngine.Object.DestroyImmediate(go);}
        }

        [Test]
        public void PhaseE1c_PostAwakeUnknownState_StartAndStepRejectWithoutFallback()
        {
            var go=new GameObject("m5d7m-post-awake");
            try
            {
                go.SetActive(false);var router=go.AddComponent<InputRouter>();Invoke(router,"Awake");var stateField=typeof(InputRouter).GetField("_preparedLaunchState",BindingFlags.Instance|BindingFlags.NonPublic);stateField.SetValue(router,99);Assert.That(()=>Invoke(router,"Start"),Throws.InstanceOf<TargetInvocationException>());Assert.That(Convert.ToInt32(stateField.GetValue(router)),Is.EqualTo(99));Assert.That(GetPrivate<GameInputActions>(router,"_actions"),Is.Null);Assert.That(()=>router.StepForTests(),Throws.InvalidOperationException);
            }
            finally{NormalizeDisposedLegacyForCleanup(go.GetComponent<InputRouter>());UnityEngine.Object.DestroyImmediate(go);}
        }

        [Test]
        public void Ac001_RecordedPreparationTraceAndTransferredTokenOwnershipAreExact()
        {
            var trace=new System.Collections.Generic.List<string>();var scenario=LaunchScenarioV1.Create(LaunchScenarioKindV1.DefaultBootstrap,trace);
            var root=Path.Combine(Path.GetTempPath(),"m5d7m-trace-"+Guid.NewGuid().ToString("N"));var utc=new DateTimeOffset(2026,9,13,0,0,0,TimeSpan.Zero);var environment=new TraceEnvironmentV1(root,utc,trace);var preparation=new ScenarioPreparationV1(scenario,trace);
            var context=CreateLiveAdapter(preparation:preparation,environment:environment,beforeActivate:(ignored,router,adapter)=>environment.Bind(router,adapter));
            try
            {
                Assert.That(scenario.Actions.Created,Has.Count.EqualTo(1));Assert.That(scenario.Actions.Applied,Has.Count.EqualTo(1));Assert.That(scenario.Actions.Disposed,Is.Empty);
                Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.SameAs(scenario.Actions.Created[0]));
                Assert.That(preparation.Prepared,Is.Not.Null);Assert.That(()=>preparation.Prepared.TakeActions(),Throws.InvalidOperationException);
                Assert.That(GetPrivate<int>(preparation.Prepared,"_ownership"),Is.EqualTo(1));Assert.That(GetPrivate<GameInputActions>(preparation.Prepared,"_actions"),Is.Null);
                CollectionAssert.AreEqual(new[]{"reserve","root","utc","prepare","create","apply"},trace);
                Assert.That(environment.RootWasReserved,Is.True);Assert.That(environment.UtcWasReserved,Is.True);
                Assert.That(scenario.PreparationRoot,Is.EqualTo(Path.GetFullPath(root)));Assert.That(scenario.PreparationUtc,Is.EqualTo(utc));
                Invoke(context.Adapter,"Start");Assert.That(GetPrivate<int>(context.Router,"_closeAttempts"),Is.EqualTo(0));
            }
            finally{context.Dispose();}
        }

        [TestCase("current",ProfileLaunchSource.Primary,6L,false)]
        [TestCase("previous",ProfileLaunchSource.Previous,5L,true)]
        [TestCase("metadata",ProfileLaunchSource.Primary,7L,true)]
        [TestCase("binding",ProfileLaunchSource.Primary,8L,true)]
        public void Ac001_ProductionFilesystemPreparation_UsesRealCoordinatorRows(string row,ProfileLaunchSource source,long finalRevision,bool repaired)
        {
            var root=Path.Combine(Path.GetTempPath(),"m5d7m-production-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);var utc=new DateTimeOffset(2026,9,13,0,0,0,TimeSpan.Zero);
            if(row=="current")File.WriteAllBytes(Path.Combine(root,"profile.json"),Document(6).CanonicalFileBytes);
            else if(row=="previous")File.WriteAllBytes(Path.Combine(root,"profile.prev.json"),Document(4).CanonicalFileBytes);
            else if(row=="metadata")File.WriteAllBytes(Path.Combine(root,"profile.json"),Document(6,"obsolete",string.Empty).CanonicalFileBytes);
            else File.WriteAllBytes(Path.Combine(root,"profile.json"),RebuildRawBinding(Document(7),"{bad}"));
            var context=CreateLiveAdapter(environment:new RecordingEnvironment(root,utc));
            try
            {
                Invoke(context.Adapter,"Start");var receipt=context.Adapter.CurrentReceipt.Value;
                Assert.That(receipt.Source,Is.EqualTo(source));Assert.That(receipt.FinalRevision,Is.EqualTo(finalRevision));Assert.That(receipt.FinalRecoveryReasons!=(ProfileRecoveryPlanReason)0,Is.EqualTo(repaired));Assert.That(receipt.InputMode,Is.EqualTo(InputMode.UIOnly));
            }
            finally{context.Dispose();if(Directory.Exists(root))Directory.Delete(root,true);}
        }

        private static void SetPrivate(object value,string field,object replacement){var item=value.GetType().GetField(field,BindingFlags.Instance|BindingFlags.NonPublic);item.SetValue(value,item.FieldType.IsEnum?Enum.ToObject(item.FieldType,replacement):replacement);}
        // Test teardown only: leave Unity with a coherent action-free disposed
        // legacy row after a test deliberately retained corrupt forensic fields.
        private static void NormalizeDisposedLegacyForCleanup(InputRouter router)
        {
            if(router==null)return;
            var held=GetPrivate<GameInputActions>(router,"_actions");if(held!=null)DisposeActionsForTest(held);
            SetPrivate(router,"_preparedLaunchState",0);SetPrivate(router,"_preparedLaunchOwner",null);SetPrivate(router,"_actions",null);
            SetPrivate(router,"_gameplayCallbacksRegistered",false);SetPrivate(router,"_uiCallbacksRegistered",false);SetPrivate(router,"_initialized",false);
            SetPrivate(router,"_awakeEntered",true);SetPrivate(router,"_startEntered",false);SetPrivate(router,"_faulted",false);SetPrivate(router,"_disposed",true);SetPrivate(router,"_actionsClosed",true);SetPrivate(router,"_preparedLaunchFaultStage",0);
        }

        private static string ExpectedAwakeFailure(LaunchFailureKindV1 kind)
        {
            switch(kind)
            {
                case LaunchFailureKindV1.RootThrow: return "InvalidOperationException: root";
                case LaunchFailureKindV1.RootThrowIOException: return "IOException: root-io";
                case LaunchFailureKindV1.RootThrowUnauthorized: return "UnauthorizedAccessException: root-unauthorized";
                case LaunchFailureKindV1.RootThrowArgument: return "ArgumentException: root-argument";
                case LaunchFailureKindV1.RootNull:
                case LaunchFailureKindV1.RootRelative:
                case LaunchFailureKindV1.RootWhitespace: return "ArgumentException: Desktop profile root is malformed.";
                case LaunchFailureKindV1.RootFilesystemRoot: return "ArgumentException: Desktop profile root cannot be a filesystem root.";
                case LaunchFailureKindV1.UtcThrow: return "NotSupportedException: utc";
                case LaunchFailureKindV1.UtcNonzero: return "InvalidOperationException: Desktop launch UTC must have zero offset.";
                case LaunchFailureKindV1.PreparationThrow: return "InvalidOperationException: preparation";
                case LaunchFailureKindV1.PreparationNull: return "InvalidOperationException: Preparation returned no token.";
                default: throw new InvalidOperationException("Unknown launch failure kind.");
            }
        }

        private static string ExpectedAwakeFailure(TokenFailureKindV1 kind)
        {
            switch(kind)
            {
                case TokenFailureKindV1.Malformed: return "InvalidOperationException: Default launch evidence is malformed.";
                case TokenFailureKindV1.PreConsumed: return "InvalidOperationException: Prepared actions are no longer owned.";
                case TokenFailureKindV1.AdoptFailed: return "InvalidOperationException: Prepared hub adoption is not authorized.";
                default: throw new InvalidOperationException("Unknown token failure kind.");
            }
        }
        private static void DisposeActionsForTest(GameInputActions actions)
        {
            if(actions==null)return;try{actions.Gameplay.Disable();}catch{}try{actions.UI.Disable();}catch{}actions.Dispose();
        }

        [TestCase(LaunchScenarioKindV1.CleanCurrent, ProfileLaunchSource.Primary, 3L, ProfileLaunchPersistenceOutcomeV1.NotRequired, false)]
        [TestCase(LaunchScenarioKindV1.DefaultBootstrap, ProfileLaunchSource.Default, 0L, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, true)]
        [TestCase(LaunchScenarioKindV1.PreviousPromotion, ProfileLaunchSource.Previous, 5L, ProfileLaunchPersistenceOutcomeV1.CommittedReplacement, true)]
        public void PhaseA_RealCoordinatorScenario_TransfersExactPreparedCandidate(LaunchScenarioKindV1 kind, ProfileLaunchSource source, long revision, ProfileLaunchPersistenceOutcomeV1 persistence, bool notification)
        {
            var scenario=LaunchScenarioV1.Create(kind); var context=CreateLiveAdapter(preparation:new ScenarioPreparationV1(scenario));
            try
            {
                Invoke(context.Adapter,"Start");
                var receipt=context.Adapter.CurrentReceipt.Value;
                Assert.That(receipt.Source,Is.EqualTo(source)); Assert.That(receipt.FinalRevision,Is.EqualTo(revision)); Assert.That(receipt.PersistenceOutcome,Is.EqualTo(persistence));
                Assert.That(scenario.PreparationRoot,Is.EqualTo(Path.GetFullPath(context.Root))); Assert.That(scenario.PreparationUtc,Is.EqualTo(context.Utc));
                Assert.That(scenario.Actions.Created.Count,Is.EqualTo(1)); Assert.That(scenario.Actions.Applied[0],Is.SameAs(scenario.Actions.Created[0])); Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.SameAs(scenario.Actions.Created[0]));
                ProfileLaunchNotificationV1 item; Assert.That(context.Adapter.TryTakeNotification(out item),Is.EqualTo(notification));
            }
            finally { context.Dispose(); }
        }

        [TestCase(LaunchScenarioKindV1.MetadataDecodeRepair,6L,7L,ProfileLaunchSaveKindV1.PrimaryInputRecovery)]
        [TestCase(LaunchScenarioKindV1.BindingDecodeRepair,7L,8L,ProfileLaunchSaveKindV1.PrimaryInputRecovery)]
        [TestCase(LaunchScenarioKindV1.BindingApplyRepair,8L,9L,ProfileLaunchSaveKindV1.Ordinary)]
        public void PhaseB_RepairScenarios_KeepExactRecoveryCandidateOwnership(LaunchScenarioKindV1 kind,long sourceRevision,long finalRevision,ProfileLaunchSaveKindV1 saveKind)
        {
            var scenario=LaunchScenarioV1.Create(kind);
            // The first candidate is deliberately poisoned/discarded. Exercise
            // that coordinator ownership row without a second full authored
            // scene lifecycle, which makes this focused assertion bounded.
            if(kind==LaunchScenarioKindV1.BindingApplyRepair)
            {
                var root=Path.Combine(Path.GetTempPath(),"m5d7m-binding-apply-"+Guid.NewGuid().ToString("N"));
                using(var prepared=ProfileLaunchPreparationCoordinatorV1.Prepare(root,new DateTimeOffset(2026,9,13,0,0,0,TimeSpan.Zero),scenario.Profile,scenario.Actions))
                {
                    Assert.That(prepared.Source,Is.EqualTo(ProfileLaunchSource.Primary));Assert.That(prepared.SourceRevision,Is.EqualTo(sourceRevision));Assert.That(prepared.FinalRevision,Is.EqualTo(finalRevision));Assert.That(prepared.FinalRecoveryReasons,Is.EqualTo(ProfileRecoveryPlanReason.InputRepair));Assert.That(prepared.SaveKind,Is.EqualTo(saveKind));
                    Assert.That(scenario.Actions.Created.Count,Is.EqualTo(2));Assert.That(scenario.Actions.Disposed[0],Is.SameAs(scenario.Actions.Created[0]));var retained=prepared.TakeActions();Assert.That(retained,Is.SameAs(scenario.Actions.Created[1]));DisposeActionsForTest(retained);
                }
                return;
            }
            var context=CreateLiveAdapter(preparation:new ScenarioPreparationV1(scenario));
            try
            {
                Invoke(context.Adapter,"Start");var receipt=context.Adapter.CurrentReceipt.Value;
                Assert.That(receipt.Source,Is.EqualTo(ProfileLaunchSource.Primary));Assert.That(receipt.SourceRevision,Is.EqualTo(sourceRevision));Assert.That(receipt.FinalRevision,Is.EqualTo(finalRevision));Assert.That(receipt.FinalRecoveryReasons,Is.EqualTo(ProfileRecoveryPlanReason.InputRepair));Assert.That(receipt.SaveKind,Is.EqualTo(saveKind));
                Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.SameAs(scenario.Actions.Created[scenario.Actions.Created.Count-1])); Assert.That(scenario.Actions.Applied[scenario.Actions.Applied.Count-1],Is.SameAs(GetPrivate<GameInputActions>(context.Router,"_actions")));
                ProfileLaunchNotificationV1 notification;Assert.That(context.Adapter.TryTakeNotification(out notification),Is.True);Assert.That(notification.Kind,Is.EqualTo(ProfileLaunchNotificationKindV1.RecoveryCompleted));
            }
            finally{context.Dispose();}
        }

        [TestCase(LaunchScenarioKindV1.PreservationDenied,ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation,ProfileAtomicSaveStage.None)]
        [TestCase(LaunchScenarioKindV1.SaveFailedBeforeCommit,ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit,ProfileAtomicSaveStage.DirectoryPreparation)]
        [TestCase(LaunchScenarioKindV1.SaveCommitUncertain,ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain,ProfileAtomicSaveStage.CommitCall)]
        public void PhaseC_TypedPersistenceRows_StayUiOnlyAndNotifyDeferred(LaunchScenarioKindV1 kind,ProfileLaunchPersistenceOutcomeV1 outcome,ProfileAtomicSaveStage stage)
        {
            var scenario=LaunchScenarioV1.Create(kind);var context=CreateLiveAdapter(preparation:new ScenarioPreparationV1(scenario));
            try
            {
                Invoke(context.Adapter,"Start");var receipt=context.Adapter.CurrentReceipt.Value;
                Assert.That(receipt.PersistenceOutcome,Is.EqualTo(outcome));Assert.That(receipt.PersistenceStage,Is.EqualTo(stage));Assert.That(receipt.PersistencePending,Is.True);Assert.That(receipt.CommittedRevision,Is.Null);Assert.That(receipt.InputMode,Is.EqualTo(InputMode.UIOnly));Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.SameAs(scenario.Actions.Created[0]));
                if(kind==LaunchScenarioKindV1.PreservationDenied){Assert.That(receipt.MayPersist,Is.False);Assert.That(receipt.PreservationStoppedRole,Is.EqualTo(ProfileLoadFileRole.Primary));Assert.That(receipt.Preservation.PrimaryState,Is.EqualTo(ProfileLaunchPreservationStateV1.FailedBeforeMove));}
                ProfileLaunchNotificationV1 notification;Assert.That(context.Adapter.TryTakeNotification(out notification),Is.True);Assert.That(notification.Kind,Is.EqualTo(ProfileLaunchNotificationKindV1.PersistenceDeferred));Assert.That(context.Adapter.TryTakeNotification(out notification),Is.False);
            }
            finally{context.Dispose();}
        }

        [TestCase(LaunchScenarioKindV1.SaveFailedBeforeCommit,ProfileLaunchNotificationKindV1.PersistenceDeferred)]
        [TestCase(LaunchScenarioKindV1.PreservationFailureNoSave,ProfileLaunchNotificationKindV1.RecoveryArtifactPreservationFailed)]
        [TestCase(LaunchScenarioKindV1.MetadataDecodeRepair,ProfileLaunchNotificationKindV1.RecoveryCompleted)]
        public void PhaseC_NotificationPriority_UsesExactFirstApplicableKind(LaunchScenarioKindV1 kind,ProfileLaunchNotificationKindV1 expected)
        {
            var scenario=LaunchScenarioV1.Create(kind);var context=CreateLiveAdapter(preparation:new ScenarioPreparationV1(scenario));
            try{Invoke(context.Adapter,"Start");ProfileLaunchNotificationV1 value;Assert.That(context.Adapter.TryTakeNotification(out value),Is.True);Assert.That(value.Kind,Is.EqualTo(expected));Assert.That(context.Adapter.TryTakeNotification(out value),Is.False);}finally{context.Dispose();}
        }

        private static object CorruptReceiptField(string name)
        {
            switch (name)
            {
                case "_source": return (AcadeGameMaker.Profile.ProfileLaunchSource)0;
                case "_sourceRevision": return -2L;
                case "_finalRevision": return -1L;
                case "_document": return default(AcadeGameMaker.Profile.ProfileCanonicalDocumentV1);
                case "_reasons": return (AcadeGameMaker.Profile.ProfileRecoveryPlanReason)0;
                case "_initialOutcome": case "_finalOutcome": return ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired;
                case "_initialFailure": case "_finalFailure": return ProfileBindingOverrideApplyFailureV1.RoundTripFailed;
                case "_initialDisposition": case "_finalDisposition": return ProfileBindingOverrideCandidateDispositionV1.Discard;
                case "_preservation": return default(ProfileLaunchPreservationProjectionV1);
                case "_save": return default(ProfileLaunchSaveProjectionV1);
                case "_proof": return null;
                case "_hubProof": return false;
                default: throw new InvalidOperationException("Unknown receipt field " + name);
            }
        }

        private static LiveContext CreateLiveAdapter(bool routerCreatedFirst = true, bool sameObject = false, IDesktopProfileLaunchPreparationV1 preparation = null, IDesktopProfileLaunchEnvironmentV1 environment = null, Action<string,InputRouter,DesktopProfileLaunchAdapterV1> beforeActivate = null)
        {
            var objects = new System.Collections.Generic.List<GameObject>();
            var root = Path.Combine(Path.GetTempPath(), "m5d7m-live-" + Guid.NewGuid().ToString("N"));
            var rig = RouterTests.CreateRig(objects);
            var cameraObject = new GameObject("m5d7m-camera"); objects.Add(cameraObject);
            var camera = cameraObject.AddComponent<AdapterCamera>(); camera.Player = rig.Player;
            var launchRoot = new GameObject("m5d7m-launch-root"); objects.Add(launchRoot); launchRoot.SetActive(false);
            var routerObject = new GameObject("m5d7m-router"); objects.Add(routerObject); routerObject.transform.SetParent(launchRoot.transform);
            var adapterObject = sameObject ? routerObject : new GameObject("m5d7m-adapter"); if (!sameObject) { objects.Add(adapterObject); adapterObject.transform.SetParent(launchRoot.transform); }
            InputRouter router; DesktopProfileLaunchAdapterV1 adapter;
            if (routerCreatedFirst) { router = routerObject.AddComponent<InputRouter>(); adapter = adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>(); }
            else { adapter = adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>(); router = routerObject.AddComponent<InputRouter>(); }
            router.ConfigureForAuthoring(rig.Player, rig.Transfer, rig.Combat, camera, InputMode.UIOnly);
            var resolvedEnvironment = environment ?? new RecordingEnvironment(root, new DateTimeOffset(2026, 9, 13, 0, 0, 0, TimeSpan.Zero));
            adapter.ConfigureForTests(router, resolvedEnvironment, preparation ?? new RecordingPreparation((RecordingEnvironment)resolvedEnvironment));
            beforeActivate?.Invoke(root,router,adapter);
            launchRoot.SetActive(true);
            return new LiveContext(objects, root, new DateTimeOffset(2026,9,13,0,0,0,TimeSpan.Zero), router, adapter);
        }

        private static LegacyContext CreateLegacyRouter()
        {
            var objects = new System.Collections.Generic.List<GameObject>();
            var rig = RouterTests.CreateRig(objects);
            var cameraObject = new GameObject("m5d7m-legacy-camera"); objects.Add(cameraObject);
            var camera = cameraObject.AddComponent<AdapterCamera>(); camera.Player=rig.Player;
            var routerObject = new GameObject("m5d7m-legacy-authored"); objects.Add(routerObject); routerObject.SetActive(false);
            var router=routerObject.AddComponent<InputRouter>(); router.ConfigureForAuthoring(rig.Player,rig.Transfer,rig.Combat,camera,InputMode.UIOnly); routerObject.SetActive(true);
            return new LegacyContext(objects, router);
        }

        private static void AssertIndependentLegacyRouterStarts()
        {
            var legacy=CreateLegacyRouter();
            try{legacy.Router.InitializeForTests();Assert.That(legacy.Router.IsFaulted,Is.False);Assert.That(GetPrivate<GameInputActions>(legacy.Router,"_actions"),Is.Not.Null);}
            finally{legacy.Dispose();}
        }
        private static FailureContext CreateFailureAdapter(IDesktopProfileLaunchEnvironmentV1 environment, IDesktopProfileLaunchPreparationV1 preparation)
        {
            var objects=new System.Collections.Generic.List<GameObject>();var parent=new GameObject("m5d7m-failure-root");objects.Add(parent);parent.SetActive(false);
            var routerObject=new GameObject("router");objects.Add(routerObject);routerObject.transform.SetParent(parent.transform);var adapterObject=new GameObject("adapter");objects.Add(adapterObject);adapterObject.transform.SetParent(parent.transform);
            var router=routerObject.AddComponent<InputRouter>();router.ConfigureForAuthoring(null,null,null,null,InputMode.UIOnly);var adapter=adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>();adapter.ConfigureForTests(router,environment,preparation);Exception error=null;try{parent.SetActive(true);}catch(Exception e){error=e;}return new FailureContext(objects,router,adapter,error);
        }
        private static TokenFailureContext CreateTokenFailureAdapter(TokenFailureKindV1 kind)
        {
            var objects=new System.Collections.Generic.List<GameObject>();var root=Path.Combine(Path.GetTempPath(),"m5d7m-token-"+Guid.NewGuid().ToString("N"));var parent=new GameObject("m5d7m-token-root");objects.Add(parent);parent.SetActive(false);
            var routerObject=new GameObject("router");objects.Add(routerObject);routerObject.transform.SetParent(parent.transform);var adapterObject=new GameObject("adapter");objects.Add(adapterObject);adapterObject.transform.SetParent(parent.transform);var router=routerObject.AddComponent<InputRouter>();router.ConfigureForAuthoring(null,null,null,null,InputMode.UIOnly);var adapter=adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>();var scenario=LaunchScenarioV1.Create(LaunchScenarioKindV1.DefaultBootstrap);IDesktopProfileLaunchPreparationV1 preparation;PreConsumedTakePreparationV1 consumed=null;
            if(kind==TokenFailureKindV1.Malformed)preparation=new MalformedTokenPreparationV1(scenario);else if(kind==TokenFailureKindV1.PreConsumed){consumed=new PreConsumedTakePreparationV1(scenario);preparation=consumed;}else{var failed=new StableAdoptFailurePreparationV1(scenario,router);failed.Owner=adapter;preparation=failed;}
            adapter.ConfigureForTests(router,new RecordingEnvironment(root,new DateTimeOffset(2026,9,13,0,0,0,TimeSpan.Zero)),preparation);Exception error=null;try{parent.SetActive(true);}catch(Exception e){error=e;}return new TokenFailureContext(objects,root,router,adapter,scenario,error,consumed==null?null:consumed.Consumed);
        }
        private static DuplicateContext CreateDuplicateAdapter()
        {
            var objects=new System.Collections.Generic.List<GameObject>();var root=Path.Combine(Path.GetTempPath(),"m5d7m-duplicate-"+Guid.NewGuid().ToString("N"));var rig=RouterTests.CreateRig(objects);var cameraObject=new GameObject("m5d7m-duplicate-camera");objects.Add(cameraObject);var camera=cameraObject.AddComponent<AdapterCamera>();camera.Player=rig.Player;var parent=new GameObject("m5d7m-duplicate-root");objects.Add(parent);parent.SetActive(false);var routerObject=new GameObject("router");objects.Add(routerObject);routerObject.transform.SetParent(parent.transform);var router=routerObject.AddComponent<InputRouter>();router.ConfigureForAuthoring(rig.Player,rig.Transfer,rig.Combat,camera,InputMode.UIOnly);
            var firstObject=new GameObject("first");objects.Add(firstObject);firstObject.transform.SetParent(parent.transform);var secondObject=new GameObject("second");objects.Add(secondObject);secondObject.transform.SetParent(parent.transform);var first=firstObject.AddComponent<DesktopProfileLaunchAdapterV1>();var second=secondObject.AddComponent<DesktopProfileLaunchAdapterV1>();var one=LaunchScenarioV1.Create(LaunchScenarioKindV1.DefaultBootstrap);var two=LaunchScenarioV1.Create(LaunchScenarioKindV1.DefaultBootstrap);var utc=new DateTimeOffset(2026,9,13,0,0,0,TimeSpan.Zero);first.ConfigureForTests(router,new RecordingEnvironment(root,utc),new ScenarioPreparationV1(one));second.ConfigureForTests(router,new RecordingEnvironment(root,utc),new ScenarioPreparationV1(two));try{parent.SetActive(true);}catch{}return new DuplicateContext(objects,root,router,first,second,one,two);
        }

        private static void Invoke(object target, string name) { target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null); }
        private static T GetPrivate<T>(object value, string name) { return (T)value.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(value); }
        private sealed class AdapterCamera : MonoBehaviour, IInputCameraSnapshotProvider
        {
            internal PlayerMovementController Player { get; set; }
            public PlayerMovementController BoundPlayer => Player;
            public bool TryGetCompletedCamera(SimulationTick expected, out SimulationCameraPoseSnapshot camera) { camera=default(SimulationCameraPoseSnapshot); return false; }
        }
        private sealed class LiveContext : IDisposable
        {
            private readonly System.Collections.Generic.List<GameObject> _objects; private readonly string _root;
            internal LiveContext(System.Collections.Generic.List<GameObject> objects, string root, DateTimeOffset utc, InputRouter router, DesktopProfileLaunchAdapterV1 adapter) { _objects=objects;_root=root;Utc=utc;Router=router;Adapter=adapter; }
            internal InputRouter Router { get; } internal DesktopProfileLaunchAdapterV1 Adapter { get; }
            internal string Root => _root; internal DateTimeOffset Utc { get; }
            public void Dispose() { RouterTests.DestroyAll(_objects); if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        }
        private sealed class LegacyContext : IDisposable
        {
            private readonly System.Collections.Generic.List<GameObject> _objects;
            internal LegacyContext(System.Collections.Generic.List<GameObject> objects, InputRouter router) { _objects=objects;Router=router; }
            internal InputRouter Router { get; }
            public void Dispose() { RouterTests.DestroyAll(_objects); }
        }
        private sealed class FailureContext : IDisposable
        {
            private readonly System.Collections.Generic.List<GameObject> _objects;internal FailureContext(System.Collections.Generic.List<GameObject> objects,InputRouter router,DesktopProfileLaunchAdapterV1 adapter,Exception error){_objects=objects;Router=router;Adapter=adapter;Error=error;}internal InputRouter Router{get;}internal DesktopProfileLaunchAdapterV1 Adapter{get;}internal Exception Error{get;}public void Dispose(){RouterTests.DestroyAll(_objects);}
        }
        internal enum TokenFailureKindV1 { Malformed, PreConsumed, AdoptFailed }
        private sealed class TokenFailureContext : IDisposable
        {
            private readonly System.Collections.Generic.List<GameObject> _objects;private readonly string _root;internal TokenFailureContext(System.Collections.Generic.List<GameObject> objects,string root,InputRouter router,DesktopProfileLaunchAdapterV1 adapter,LaunchScenarioV1 scenario,Exception error,GameInputActions consumed){_objects=objects;_root=root;Router=router;Adapter=adapter;Scenario=scenario;Error=error;PreConsumed=consumed;}internal InputRouter Router{get;}internal DesktopProfileLaunchAdapterV1 Adapter{get;}internal LaunchScenarioV1 Scenario{get;}internal Exception Error{get;}internal GameInputActions PreConsumed{get;}public void Dispose(){RouterTests.DestroyAll(_objects);if(Directory.Exists(_root))Directory.Delete(_root,true);}
        }
        private sealed class DuplicateContext : IDisposable
        {
            private readonly System.Collections.Generic.List<GameObject> _objects;private readonly string _root;internal DuplicateContext(System.Collections.Generic.List<GameObject> objects,string root,InputRouter router,DesktopProfileLaunchAdapterV1 first,DesktopProfileLaunchAdapterV1 second,LaunchScenarioV1 one,LaunchScenarioV1 two){_objects=objects;_root=root;Router=router;First=first;Second=second;FirstScenario=one;SecondScenario=two;}internal InputRouter Router{get;}internal DesktopProfileLaunchAdapterV1 First{get;}internal DesktopProfileLaunchAdapterV1 Second{get;}internal LaunchScenarioV1 FirstScenario{get;}internal LaunchScenarioV1 SecondScenario{get;}public void Dispose(){RouterTests.DestroyAll(_objects);if(Directory.Exists(_root))Directory.Delete(_root,true);}
        }

        internal enum LaunchScenarioKindV1 { CleanCurrent, DefaultBootstrap, PreviousPromotion, MetadataDecodeRepair, BindingDecodeRepair, BindingApplyRepair, PreservationDenied, SaveFailedBeforeCommit, SaveCommitUncertain, PreservationFailureNoSave }
        private sealed class LaunchScenarioV1
        {
            internal readonly ScenarioProfilePortV1 Profile; internal readonly TrackingActionsPortV1 Actions; internal string PreparationRoot; internal DateTimeOffset PreparationUtc;
            private LaunchScenarioV1(ProfileLaunchObservationProjectionV1 observation, ProfileLaunchSaveProjectionV1 save, ProfileLaunchPreservationProjectionV1? preservation=null, System.Collections.Generic.List<string> trace=null) { Profile=new ScenarioProfilePortV1(observation,save,preservation); Actions=new TrackingActionsPortV1(trace); }
            internal static LaunchScenarioV1 Create(LaunchScenarioKindV1 kind, System.Collections.Generic.List<string> trace=null)
            {
                var missing=ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp);
                if(kind==LaunchScenarioKindV1.CleanCurrent) { var current=Candidate(ProfileLoadFileRole.Primary,Document(3)); return new LaunchScenarioV1(new ProfileLaunchObservationProjectionV1(current,ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),missing),NotRequired(),null,trace); }
                if(kind==LaunchScenarioKindV1.PreviousPromotion) { var previous=Candidate(ProfileLoadFileRole.Previous,Document(4)); return new LaunchScenarioV1(new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary),previous,missing),new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,ProfileLaunchPersistenceOutcomeV1.CommittedReplacement,ProfileAtomicSaveStage.None,ProfileStoredFileState.ValidCurrentInput,ProfileStoredFileState.ValidCurrentInput,ProfileStoredFileState.Missing,5)); }
                if(kind==LaunchScenarioKindV1.MetadataDecodeRepair) { var primary=Candidate(ProfileLoadFileRole.Primary,Document(6,"obsolete",string.Empty)); return new LaunchScenarioV1(new ProfileLaunchObservationProjectionV1(primary,ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),missing),RecoverySave(7)); }
                if(kind==LaunchScenarioKindV1.BindingDecodeRepair) { var source=Document(7); var primary=ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Primary,ProfileCanonicalDecoderV1.Decode(RebuildRawBinding(source,"{bad}"))); return new LaunchScenarioV1(new ProfileLaunchObservationProjectionV1(primary,ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),missing),RecoverySave(8)); }
                if(kind==LaunchScenarioKindV1.BindingApplyRepair) { var stale=StaleBindingOverride(); var primary=Candidate(ProfileLoadFileRole.Primary,Document(8,ProfileInputContract.CurrentInputActionsAssetId,stale)); var row=new LaunchScenarioV1(new ProfileLaunchObservationProjectionV1(primary,ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),missing),new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,ProfileLaunchPersistenceOutcomeV1.CommittedReplacement,ProfileAtomicSaveStage.None,ProfileStoredFileState.ValidCurrentInput,ProfileStoredFileState.ValidCurrentInput,ProfileStoredFileState.Missing,9)); row.Actions.First=new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired,ProfileBindingOverrideApplyFailureV1.LoadFailed,ProfileBindingOverrideCandidateDispositionV1.Discard,stale,null); return row; }
                if(kind==LaunchScenarioKindV1.PreservationDenied) { var primary=ProfileLoadCandidateV1.Unreadable(ProfileLoadFileRole.Primary);var preservation=new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired,ProfileLaunchPreservationStateV1.FailedBeforeMove,ProfileLaunchPreservationStateV1.NotRequired,false,ProfileLoadFileRole.Primary,ProfileFilePreservationReason.None,ProfileFilePreservationReason.UnreadablePrimary,ProfileFilePreservationReason.None,1);return new LaunchScenarioV1(new ProfileLaunchObservationProjectionV1(primary,ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),missing),new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation,ProfileAtomicSaveStage.None,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,null),preservation);}
                if(kind==LaunchScenarioKindV1.SaveFailedBeforeCommit)return new LaunchScenarioV1(new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),missing),new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit,ProfileAtomicSaveStage.DirectoryPreparation,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,null));
                if(kind==LaunchScenarioKindV1.SaveCommitUncertain)return new LaunchScenarioV1(new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),missing),new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain,ProfileAtomicSaveStage.CommitCall,ProfileStoredFileState.Missing,ProfileStoredFileState.Missing,ProfileStoredFileState.Missing,null));
                if(kind==LaunchScenarioKindV1.PreservationFailureNoSave){var current=Candidate(ProfileLoadFileRole.Primary,Document(10));var temp=ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Temp,ProfileCanonicalDecoderV1.Decode(Encoding.UTF8.GetBytes("{}")));var preservation=new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.FailedBeforeMove,ProfileLaunchPreservationStateV1.NotRequired,ProfileLaunchPreservationStateV1.NotRequired,false,ProfileLoadFileRole.Temp,ProfileFilePreservationReason.StaleTemp,ProfileFilePreservationReason.None,ProfileFilePreservationReason.None,1);return new LaunchScenarioV1(new ProfileLaunchObservationProjectionV1(current,ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),temp),NotRequired(),preservation);}
                return new LaunchScenarioV1(new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),missing),new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,ProfileLaunchPersistenceOutcomeV1.CommittedFirst,ProfileAtomicSaveStage.None,ProfileStoredFileState.ValidCurrentInput,ProfileStoredFileState.Missing,ProfileStoredFileState.Missing,0),null,trace);
            }
        }
        private sealed class ScenarioPreparationV1 : IDesktopProfileLaunchPreparationV1
        {
            private readonly LaunchScenarioV1 _scenario; private readonly System.Collections.Generic.List<string> _trace; internal ScenarioPreparationV1(LaunchScenarioV1 scenario, System.Collections.Generic.List<string> trace=null) { _scenario=scenario;_trace=trace; }
            internal PreparedProfileLaunchV1 Prepared { get; private set; }
            public PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc) { if(_trace!=null)_trace.Add("prepare");_scenario.PreparationRoot=root;_scenario.PreparationUtc=utc;Prepared=ProfileLaunchPreparationCoordinatorV1.Prepare(root,utc,_scenario.Profile,_scenario.Actions);return Prepared; }
        }
        private sealed class ScenarioProfilePortV1 : IProfileLaunchPreparationProfilePortV1
        {
            private readonly ProfileLaunchObservationProjectionV1 _observation; private readonly ProfileLaunchSaveProjectionV1 _save; private readonly ProfileLaunchPreservationProjectionV1? _preservation;
            internal ScenarioProfilePortV1(ProfileLaunchObservationProjectionV1 observation,ProfileLaunchSaveProjectionV1 save,ProfileLaunchPreservationProjectionV1? preservation=null){_observation=observation;_save=save;_preservation=preservation;}
            public ProfileLaunchObservationProjectionV1 Observe(string root){return _observation;}
            public ProfileLaunchPreservationProjectionV1 Preserve(string root,ProfileLaunchObservationProjectionV1 observation,ProfileLoadSelectionPlanV1 selection,DateTimeOffset utc){return _preservation??new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired,ProfileLaunchPreservationStateV1.NotRequired,ProfileLaunchPreservationStateV1.NotRequired,true,null,selection.TempPreservation,selection.PrimaryPreservation,selection.PreviousPreservation,0);}
            public ProfileLaunchSaveProjectionV1 Save(string root,ProfileCanonicalDocumentV1 document){return _save;}
            public ProfileLaunchSaveProjectionV1 SaveInputRecovery(string root,ProfileLoadCandidateV1 primary){return _save;}
        }
        private sealed class TrackingActionsPortV1 : IProfileLaunchPreparationActionsPortV1
        {
            private readonly System.Collections.Generic.List<string> _trace; internal readonly System.Collections.Generic.List<GameInputActions> Created=new System.Collections.Generic.List<GameInputActions>(); internal readonly System.Collections.Generic.List<GameInputActions> Applied=new System.Collections.Generic.List<GameInputActions>(); internal readonly System.Collections.Generic.List<GameInputActions> Disposed=new System.Collections.Generic.List<GameInputActions>(); internal ProfileBindingOverrideApplyResultV1? First;
            internal TrackingActionsPortV1(System.Collections.Generic.List<string> trace=null){_trace=trace;}
            public GameInputActions Create(){if(_trace!=null)_trace.Add("create");var x=new GameInputActions();Created.Add(x);return x;}
            public ProfileBindingOverrideApplyResultV1 Apply(ProfileInputSnapshot input,GameInputActions candidate){if(_trace!=null)_trace.Add("apply");Applied.Add(candidate);if(Applied.Count==1&&First.HasValue)return First.Value;return new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady,ProfileBindingOverrideApplyFailureV1.None,ProfileBindingOverrideCandidateDispositionV1.Retain,string.Empty,string.Empty);}
            public void Dispose(GameInputActions candidate){if(_trace!=null)_trace.Add("dispose");Disposed.Add(candidate);DisposeActionsForTest(candidate);}
        }
        private static ProfileCanonicalDocumentV1 Document(long revision,string asset=null,string overrides="")
        {
            var input=new ProfileInputSnapshot(asset??ProfileInputContract.CurrentInputActionsAssetId,ProfileInputContract.CurrentBindingSchemaVersion,ProfileBindingOverridesJson.Parse(overrides));
            return ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed,500,500,500,false,false),input,new ProfileTutorialSnapshot(Array.Empty<string>()),new ProfileProgressionSnapshot(revision,null,null,ProfileConsentState.RefusesOwnershipTransfer,null,Array.Empty<ProfileBranch>())));
        }
        private static ProfileLoadCandidateV1 Candidate(ProfileLoadFileRole role,ProfileCanonicalDocumentV1 document){return ProfileLoadCandidateV1.Decoded(role,ProfileCanonicalDecoderV1.Decode(document.CanonicalFileBytes));}
        private static ProfileLaunchSaveProjectionV1 NotRequired(){return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.None,ProfileLaunchPersistenceOutcomeV1.NotRequired,ProfileAtomicSaveStage.None,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,null);}
        private static ProfileLaunchSaveProjectionV1 RecoverySave(long revision){return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.PrimaryInputRecovery,ProfileLaunchPersistenceOutcomeV1.CommittedPrimaryInputRecovery,ProfileAtomicSaveStage.None,ProfileStoredFileState.ValidCurrentInput,ProfileStoredFileState.ValidInputRecoveryRequired,ProfileStoredFileState.Missing,revision);}
        private static string StaleBindingOverride(){using(var source=new GameInputActions()){source.Gameplay.Jump.ApplyBindingOverride("<Keyboard>/k");var raw=source.asset.SaveBindingOverridesAsJson();var marker=raw.IndexOf("\"id\":\"",StringComparison.Ordinal);var start=marker+6;var end=raw.IndexOf('"',start);return ProfileBindingOverridesJson.Parse(raw.Substring(0,start)+"11111111-2222-4333-8444-555555555555"+raw.Substring(end)).CanonicalText;}}
        private static byte[] RebuildRawBinding(ProfileCanonicalDocumentV1 source,string raw){var payload=Encoding.UTF8.GetString(source.CanonicalPayloadBytes).Replace("\"bindingOverridesJson\":\"\"","\"bindingOverridesJson\":\""+raw+"\"");var bytes=Encoding.UTF8.GetBytes(payload);using(var sha=SHA256.Create()){var hash=sha.ComputeHash(bytes);var text=new StringBuilder(64);foreach(var item in hash)text.Append(item.ToString("x2",CultureInfo.InvariantCulture));return Encoding.UTF8.GetBytes(payload.Substring(0,payload.Length-1)+",\"integrity\":{\"payloadSha256\":\""+text+"\"}}");}}

        private sealed class TraceEnvironmentV1 : IDesktopProfileLaunchEnvironmentV1
        {
            private readonly string _root;private readonly DateTimeOffset _utc;private readonly System.Collections.Generic.List<string> _trace;private InputRouter _router;private DesktopProfileLaunchAdapterV1 _adapter;
            internal TraceEnvironmentV1(string root,DateTimeOffset utc,System.Collections.Generic.List<string> trace){_root=root;_utc=utc;_trace=trace;}
            internal bool RootWasReserved{get;private set;}internal bool UtcWasReserved{get;private set;}
            internal void Bind(InputRouter router,DesktopProfileLaunchAdapterV1 adapter){_router=router;_adapter=adapter;}
            public string GetPersistentDataPath(){RootWasReserved=Convert.ToInt32(GetPrivate<object>(_router,"_preparedLaunchState"))==1&&ReferenceEquals(GetPrivate<object>(_router,"_preparedLaunchOwner"),_adapter);if(RootWasReserved)_trace.Add("reserve");_trace.Add("root");return _root;}
            public DateTimeOffset GetUtcNow(){UtcWasReserved=Convert.ToInt32(GetPrivate<object>(_router,"_preparedLaunchState"))==1&&ReferenceEquals(GetPrivate<object>(_router,"_preparedLaunchOwner"),_adapter);_trace.Add("utc");return _utc;}
        }

        private sealed class RecordingEnvironment : IDesktopProfileLaunchEnvironmentV1
        {
            private readonly string _root; private readonly DateTimeOffset _utc;
            internal RecordingEnvironment(string root, DateTimeOffset utc) { _root=root;_utc=utc; }
            internal string Calls { get; private set; }
            internal string PreparationRoot { get; private set; }
            internal DateTimeOffset PreparationUtc { get; private set; }
            internal DateTimeOffset Utc => _utc;
            public string GetPersistentDataPath() { Calls="root"; return _root; }
            public DateTimeOffset GetUtcNow() { Calls += ",utc"; return _utc; }
            internal void Prepared(string root, DateTimeOffset utc) { Calls += ",prepare"; PreparationRoot=root; PreparationUtc=utc; }
        }
        private sealed class RecordingPreparation : IDesktopProfileLaunchPreparationV1
        {
            private readonly RecordingEnvironment _environment;
            internal RecordingPreparation(RecordingEnvironment environment) { _environment=environment; }
            public PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc) { _environment.Prepared(root, utc); return ProfileLaunchPreparationCoordinatorV1.Prepare(root, utc); }
        }
        private sealed class ThrowingPreparation : IDesktopProfileLaunchPreparationV1
        {
            public PreparedProfileLaunchV1 Prepare(string root, DateTimeOffset utc) { throw new InvalidOperationException("preparation should not run"); }
        }
        internal enum LaunchFailureKindV1 { RootThrow, RootThrowIOException, RootThrowUnauthorized, RootThrowArgument, RootNull, RootRelative, RootWhitespace, RootFilesystemRoot, UtcThrow, UtcNonzero, PreparationThrow, PreparationNull }
        private sealed class ScriptedEnvironmentV1 : IDesktopProfileLaunchEnvironmentV1
        {
            private readonly LaunchFailureKindV1 _kind; internal int RootCalls, UtcCalls;
            private ScriptedEnvironmentV1(LaunchFailureKindV1 kind){_kind=kind;} internal static ScriptedEnvironmentV1 For(LaunchFailureKindV1 kind){return new ScriptedEnvironmentV1(kind);}
            public string GetPersistentDataPath(){RootCalls++;if(_kind==LaunchFailureKindV1.RootThrow)throw new InvalidOperationException("root");if(_kind==LaunchFailureKindV1.RootThrowIOException)throw new IOException("root-io");if(_kind==LaunchFailureKindV1.RootThrowUnauthorized)throw new UnauthorizedAccessException("root-unauthorized");if(_kind==LaunchFailureKindV1.RootThrowArgument)throw new ArgumentException("root-argument");if(_kind==LaunchFailureKindV1.RootNull)return null;if(_kind==LaunchFailureKindV1.RootRelative)return "relative";if(_kind==LaunchFailureKindV1.RootWhitespace)return " ";if(_kind==LaunchFailureKindV1.RootFilesystemRoot)return Path.GetPathRoot(Path.GetTempPath());return Path.Combine(Path.GetTempPath(),"m5d7m-failure-"+Guid.NewGuid().ToString("N"));}
            public DateTimeOffset GetUtcNow(){UtcCalls++;if(_kind==LaunchFailureKindV1.UtcThrow)throw new NotSupportedException("utc");return _kind==LaunchFailureKindV1.UtcNonzero?new DateTimeOffset(2026,9,13,0,0,0,TimeSpan.FromHours(9)):new DateTimeOffset(2026,9,13,0,0,0,TimeSpan.Zero);}
        }
        private sealed class ScriptedFailurePreparationV1 : IDesktopProfileLaunchPreparationV1
        {
            private readonly LaunchFailureKindV1 _kind; internal int Calls; internal ScriptedFailurePreparationV1(LaunchFailureKindV1 kind){_kind=kind;}
            public PreparedProfileLaunchV1 Prepare(string root,DateTimeOffset utc){Calls++;if(_kind==LaunchFailureKindV1.PreparationNull)return null;throw new InvalidOperationException("preparation");}
        }
        private sealed class MalformedTokenPreparationV1 : IDesktopProfileLaunchPreparationV1
        {
            private readonly ScenarioPreparationV1 _inner; internal MalformedTokenPreparationV1(LaunchScenarioV1 scenario){_inner=new ScenarioPreparationV1(scenario);}
            public PreparedProfileLaunchV1 Prepare(string root,DateTimeOffset utc){var token=_inner.Prepare(root,utc);typeof(PreparedProfileLaunchV1).GetField("_sourceRevision",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(token,-2L);return token;}
        }
        private sealed class PreConsumedTakePreparationV1 : IDesktopProfileLaunchPreparationV1
        {
            private readonly ScenarioPreparationV1 _inner; internal GameInputActions Consumed; internal PreConsumedTakePreparationV1(LaunchScenarioV1 scenario){_inner=new ScenarioPreparationV1(scenario);}
            public PreparedProfileLaunchV1 Prepare(string root,DateTimeOffset utc){var token=_inner.Prepare(root,utc);Consumed=token.TakeActions();DisposeActionsForTest(Consumed);return token;}
        }
        private sealed class StableAdoptFailurePreparationV1 : IDesktopProfileLaunchPreparationV1
        {
            private readonly ScenarioPreparationV1 _inner; private readonly InputRouter _router; internal object Owner;
            internal StableAdoptFailurePreparationV1(LaunchScenarioV1 scenario,InputRouter router){_inner=new ScenarioPreparationV1(scenario);_router=router;}
            public PreparedProfileLaunchV1 Prepare(string root,DateTimeOffset utc){var token=_inner.Prepare(root,utc);_router.FailPreparedHubLaunch(Owner);return token;}
        }
    }
}
