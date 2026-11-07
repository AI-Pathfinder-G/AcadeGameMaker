using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Profile;
using AcadeGameMaker.Transfer;
using NUnit.Framework;
using UnityEngine;
using RouterTests = AcadeGameMaker.Tests.PlayMode.InputUnity.InputRouterPlayModeTests;

namespace AcadeGameMaker.Input.Unity.PlayMode.Tests
{
    // AC-M5D7N-001..008. Authored pre-lifecycle same-GameObject cohort only;
    // no late AddComponent history/provenance claim is made (Luna P2-001).
    internal sealed class HubEntryHandoffLatchV1Tests
    {
        [Test]
        public void DefaultReceipt_FirstUpdatePublishesOneUiOnlyHandoffAndOneNotification()
        {
            var context=Create(false);
            try
            {
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");Assert.That(State(context.Latch),Is.EqualTo(2));Invoke(context.Latch,"Update");
                var handoff=context.Latch.CurrentHandoff;Assert.That(handoff.HasValue,Is.True);Assert.That(handoff.Value.HubEntryAccepted,Is.True);Assert.That(handoff.Value.InputMode,Is.EqualTo(InputMode.UIOnly));Assert.That(handoff.Value.GameplayEnabled,Is.False);Assert.That(handoff.Value.UiEnabled,Is.True);
                ProfileLaunchNotificationV1 notification;Assert.That(context.Latch.TryTakeLaunchNotification(out notification),Is.True);notification.Validate();Assert.That(context.Latch.TryTakeLaunchNotification(out notification),Is.False);Assert.That(State(context.Latch),Is.EqualTo(5));Invoke(context.Latch,"Update");Assert.That(context.Latch.CurrentHandoff.HasValue,Is.True);
            }
            finally{context.Dispose();}
        }

        [Test]
        public void CleanPrimary_ExpectedNonePairsWithFalseDefaultAndPublishes()
        {
            var context=Create(true);
            try
            {
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");Invoke(context.Latch,"Update");ProfileLaunchNotificationV1 ignored;
                Assert.That(context.Latch.CurrentHandoff.HasValue,Is.True);Assert.That(context.Latch.TryTakeLaunchNotification(out ignored),Is.False);Assert.That(State(context.Latch),Is.EqualTo(3));
            }
            finally{context.Dispose();}
        }

        [Test]
        public void PriorConsumedExpectedNotification_FailsFirstUpdateWithoutHandoffOrRetry()
        {
            var context=Create(false);
            try
            {
                Invoke(context.Adapter,"Start");ProfileLaunchNotificationV1 source;Assert.That(context.Adapter.TryTakeNotification(out source),Is.True);Invoke(context.Latch,"Start");
                Assert.That(()=>Invoke(context.Latch,"Update"),Throws.InstanceOf<TargetInvocationException>());Assert.That(context.Latch.CurrentHandoff,Is.Null);Assert.That(State(context.Latch),Is.EqualTo(6));Invoke(context.Latch,"Update");Assert.That(context.Latch.CurrentHandoff,Is.Null);
            }
            finally{context.Dispose();}
        }

        [Test]
        public void PrePublicationDisableConsumesNoSourceNotification_PostPublicationTeardownRetainsHandoff()
        {
            var before=Create(false);var after=Create(false);
            try
            {
                Invoke(before.Adapter,"Start");Invoke(before.Latch,"Start");Invoke(before.Latch,"OnDisable");ProfileLaunchNotificationV1 source;Assert.That(before.Adapter.TryTakeNotification(out source),Is.True);Assert.That(before.Latch.CurrentHandoff,Is.Null);Assert.That(State(before.Latch),Is.EqualTo(7));
                Invoke(after.Adapter,"Start");Invoke(after.Latch,"Start");Invoke(after.Latch,"Update");var handoff=after.Latch.CurrentHandoff;Invoke(after.Latch,"OnDestroy");Assert.That(after.Latch.CurrentHandoff,Is.EqualTo(handoff));Assert.That(after.Router.IsFaulted,Is.False);
            }
            finally{before.Dispose();after.Dispose();}
        }

        // AC-M5D7N-008: one hostile immutable-receipt field per fixture keeps
        // reflection evidence bounded and prevents a single NUnit case from
        // accumulating all six validation paths before teardown.
        [TestCase("_receipt")]
        [TestCase("_correlation")]
        [TestCase("_mode")]
        [TestCase("_gameplay")]
        [TestCase("_ui")]
        [TestCase("_accepted")]
        public void HandoffReceiptReflectionMutation_FailsClosed(string fieldName)
        {
            var context=Create(false);var prepared=false;
            try
            {
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");Invoke(context.Latch,"Update");prepared=true;var handoff=(object)ReadPublishedHandoffDirectly(context.Latch);
                var field=typeof(HubEntryHandoffReceiptV1).GetField(fieldName,BindingFlags.Instance|BindingFlags.NonPublic);field.SetValue(handoff,HandoffReceiptBadValue(fieldName));
                SetPrivate(context.Latch,"_handoff",handoff);
                Assert.That(()=>{var ignored=context.Latch.CurrentHandoff;},Throws.InvalidOperationException,fieldName+" getter");
                ProfileLaunchNotificationV1 ignoredNotification;
                Assert.That(()=>context.Latch.TryTakeLaunchNotification(out ignoredNotification),Throws.InvalidOperationException,fieldName+" take");
                Assert.That(()=>Invoke(context.Latch,"Update"),Throws.InstanceOf<TargetInvocationException>(),fieldName+" update");
            }
            finally{if(prepared)NormalizeLatchForCleanup(context.Latch);context.Dispose();}
        }

        [Test]
        public void HandoffIllegalState_RejectsUpdateAndGetterThenRestoresForTeardown()
        {
            var context=Create(false);var prepared=false;
            try
            {
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");Invoke(context.Latch,"Update");prepared=true;
                SetPrivate(context.Latch,"_state",99);Assert.That(()=>Invoke(context.Latch,"Update"),Throws.InstanceOf<TargetInvocationException>());Assert.That(()=>{var ignored=context.Latch.CurrentHandoff;},Throws.InvalidOperationException);
            }
            finally{if(prepared)NormalizeLatchForCleanup(context.Latch);context.Dispose();}
        }

        // AC-M5D7N-004/006/008: every retained publication proof participates in
        // the same fail-closed boundary used by getter, take and Update.
        [TestCase("_handoff")]
        [TestCase("_handoffProof")]
        [TestCase("_notification")]
        [TestCase("_ownedNotificationProof")]
        [TestCase("_expectedNotification")]
        [TestCase("_sourceNotificationProbed")]
        [TestCase("_sourceNotificationTaken")]
        [TestCase("_localNotificationDelivered")]
        [TestCase("_retainedFailureNotification")]
        public void PublicationHistoryReflectionMutation_IsRejectedByGetterTakeAndUpdate(string fieldName)
        {
            var context=Create(false);var prepared=false;
            try
            {
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");Invoke(context.Latch,"Update");prepared=true;
                var field=typeof(HubEntryHandoffLatchV1).GetField(fieldName,BindingFlags.Instance|BindingFlags.NonPublic);
                object changed;
                if(fieldName=="_handoff"||fieldName=="_handoffProof"||fieldName=="_notification"||fieldName=="_ownedNotificationProof") changed=null;
                else if(fieldName=="_retainedFailureNotification") changed=typeof(HubEntryHandoffLatchV1).GetField("_notification",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(context.Latch);
                else changed=!(bool)field.GetValue(context.Latch);
                field.SetValue(context.Latch,changed);
                Assert.That(()=>{var ignored=context.Latch.CurrentHandoff;},Throws.InvalidOperationException,fieldName+" getter");
                ProfileLaunchNotificationV1 ignoredNotification;
                Assert.That(()=>context.Latch.TryTakeLaunchNotification(out ignoredNotification),Throws.InvalidOperationException,fieldName+" take");
                Assert.That(()=>Invoke(context.Latch,"Update"),Throws.InstanceOf<TargetInvocationException>(),fieldName+" update");
            }
            finally{if(prepared)NormalizeLatchForCleanup(context.Latch);context.Dispose();}
        }

        [Test]
        public void UpdateBeforeStart_FailsTerminallyWithoutSourceRead()
        {
            var context=Create(false);
            try
            {
                Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.AwaitingStart));
                Assert.That(()=>Invoke(context.Latch,"Update"),Throws.InstanceOf<TargetInvocationException>());
                Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.Failed));
                Assert.That(context.Latch.CurrentHandoff,Is.Null);
                Assert.That(()=>Invoke(context.Latch,"Update"),Throws.Nothing);
            }
            finally{context.Dispose();}
        }

        [TestCase("external")]
        [TestCase("disabled")]
        [TestCase("inactive")]
        public void AuthoredTopologyIsRecheckedAtStartAndFirstUpdate(string mutation)
        {
            var context=Create(false);
            GameObject external=null;
            try
            {
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");
                if(mutation=="external")
                {
                    external=new GameObject("m5d7n-foreign-adapter");external.SetActive(false);
                    SetPrivate(context.Latch,"_launchAdapter",external.AddComponent<DesktopProfileLaunchAdapterV1>());
                }
                else if(mutation=="disabled") context.Adapter.enabled=false;
                else context.Adapter.gameObject.SetActive(false);
                if(mutation=="inactive")
                {
                    // The adapter and latch share an authored host. Deactivation
                    // invokes the real prepublication OnDisable first, so AC-006
                    // requires a terminal close/no retry rather than a fault.
                    Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.ClosedBeforePublication),mutation);
                    Assert.That(()=>Invoke(context.Latch,"Update"),Throws.Nothing,mutation+" terminal no retry");
                    Assert.That(context.Latch.CurrentHandoff,Is.Null,mutation);
                }
                else
                {
                    Assert.That(()=>Invoke(context.Latch,"Update"),Throws.InstanceOf<TargetInvocationException>(),mutation);
                    Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.Failed),mutation);
                    Assert.That(context.Latch.CurrentHandoff,Is.Null,mutation);
                }
            }
            finally
            {
                if(external!=null) UnityEngine.Object.DestroyImmediate(external);
                context.Dispose();
            }
        }

        [Test]
        public void AuthoredTopologyReplacementBeforeStart_FailsBeforeAnyLatchSourceRead()
        {
            var context=Create(false);var foreign=new GameObject("m5d7n-foreign-before-start");foreign.SetActive(false);
            try
            {
                Invoke(context.Adapter,"Start");
                SetPrivate(context.Latch,"_launchAdapter",foreign.AddComponent<DesktopProfileLaunchAdapterV1>());
                Assert.That(()=>Invoke(context.Latch,"Start"),Throws.InstanceOf<TargetInvocationException>());
                Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.Failed));
                Assert.That(context.Latch.CurrentHandoff,Is.Null);
            }
            finally{UnityEngine.Object.DestroyImmediate(foreign);context.Dispose();}
        }

        // AC-M5D7N-005: the latch itself is part of the authored active cohort;
        // private lifecycle invocation makes each boundary deterministic without
        // claiming a late-AddComponent lifecycle history.
        [TestCase("Awake")]
        [TestCase("Start")]
        [TestCase("Update")]
        public void DisabledLatch_IsRejectedAtEveryPrePublicationBoundary(string boundary)
        {
            var context=Create(false);
            try
            {
                if(boundary=="Awake") SetPrivate(context.Latch,"_state",HubEntryHandoffStateV1.Pristine);
                else if(boundary=="Update") { Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start"); }
                context.Latch.enabled=false;
                // Disabling invokes OnDisable synchronously; restore only the legal
                // pre-publication baseline so this row isolates the self-enabled
                // authored-topology fact rather than the closed-state guard.
                SetPrivate(context.Latch,"_state",boundary=="Awake" ? HubEntryHandoffStateV1.Pristine : boundary=="Start" ? HubEntryHandoffStateV1.AwaitingStart : HubEntryHandoffStateV1.AwaitingFirstUpdate);
                Assert.That(()=>Invoke(context.Latch,boundary),Throws.InstanceOf<TargetInvocationException>(),boundary);
                Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.Failed),boundary);
                Assert.That(context.Latch.CurrentHandoff,Is.Null,boundary);
            }
            finally{context.Dispose();}
        }

        [Test]
        public void InactiveLatchHost_IsRejectedAtFirstUpdateBoundary()
        {
            var context=Create(false);
            try
            {
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");
                context.Latch.gameObject.SetActive(false); // Unity closes it first.
                SetPrivate(context.Latch,"_state",HubEntryHandoffStateV1.AwaitingFirstUpdate);
                Assert.That(()=>Invoke(context.Latch,"Update"),Throws.InstanceOf<TargetInvocationException>());
                Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.Failed));
                Assert.That(context.Latch.CurrentHandoff,Is.Null);
            }
            finally{context.Dispose();}
        }

        [TestCase("Awake")]
        [TestCase("Start")]
        public void InactiveLatchHost_IsRejectedBeforeFirstUpdate(string boundary)
        {
            var context=Create(false);
            try
            {
                context.Latch.gameObject.SetActive(false);
                SetPrivate(context.Latch,"_state",boundary=="Awake"?HubEntryHandoffStateV1.Pristine:HubEntryHandoffStateV1.AwaitingStart);
                Assert.That(()=>Invoke(context.Latch,boundary),Throws.InstanceOf<TargetInvocationException>(),boundary);
                Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.Failed),boundary);
                Assert.That(context.Latch.CurrentHandoff,Is.Null,boundary);
            }
            finally{context.Dispose();}
        }

        [TestCase("_receipt")]
        [TestCase("_correlation")]
        [TestCase("_kind")]
        public void NotificationReflectionMutation_IsRejectedAtGetterTakeAndUpdate(string fieldName)
        {
            var context=Create(false);var prepared=false;
            try
            {
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");Invoke(context.Latch,"Update");prepared=true;
                var ownedProof=typeof(HubEntryHandoffLatchV1).GetField("_ownedNotificationProof",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(context.Latch);
                var pending=(ProfileLaunchNotificationV1)typeof(HubEntryHandoffLatchV1).GetField("_notification",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(context.Latch);
                var boxed=(object)pending;
                var field=typeof(ProfileLaunchNotificationV1).GetField(fieldName,BindingFlags.Instance|BindingFlags.NonPublic);
                field.SetValue(boxed,field.FieldType.IsEnum?Enum.ToObject(field.FieldType,999):default(DesktopProfileLaunchReceiptV1));SetPrivate(context.Latch,"_notification",boxed);
                Assert.That(typeof(HubEntryHandoffLatchV1).GetField("_ownedNotificationProof",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(context.Latch),Is.EqualTo(ownedProof),fieldName+" proof unchanged");
                Assert.That(()=>{var ignored=context.Latch.CurrentHandoff;},Throws.InvalidOperationException,fieldName+" getter");
                ProfileLaunchNotificationV1 ignoredNotification;Assert.That(()=>context.Latch.TryTakeLaunchNotification(out ignoredNotification),Throws.InvalidOperationException,fieldName+" take");
                Assert.That(()=>Invoke(context.Latch,"Update"),Throws.InstanceOf<TargetInvocationException>(),fieldName+" update");
            }
            finally{if(prepared)NormalizeLatchForCleanup(context.Latch);context.Dispose();}
        }

        // AC-M5D7N-001..004: the adapter path is driven with real M5D7L Prepare;
        // only filesystem observation/save side effects are replaced by narrow
        // deterministic instance ports.
        [TestCaseSource(nameof(ScenarioTable))]
        public void RealCoordinatorScenario_AwakeStartFirstUpdate_PublishesExactTypedOutcome(LaunchScenario scenario)
        {
            var context=CreateScenario(scenario);
            try
            {
                Assert.That(scenario.Preparation.Calls,Is.EqualTo(1),scenario.Name+" awake preparation");
                Assert.That(scenario.Preparation.Root,Is.EqualTo(context.Root),scenario.Name+" normalized root");
                Assert.That(scenario.Preparation.Utc,Is.EqualTo(Utc),scenario.Name+" UTC");
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");Invoke(context.Latch,"Update");
                var handoff=context.Latch.CurrentHandoff;
                Assert.That(handoff.HasValue,Is.True,scenario.Name);
                handoff.Value.Validate();
                Assert.That(handoff.Value.HubEntryAccepted,Is.True,scenario.Name+" accepted");
                Assert.That(handoff.Value.GameplayEnabled,Is.False,scenario.Name+" gameplay disabled");
                Assert.That(handoff.Value.UiEnabled,Is.True,scenario.Name+" UI enabled");
                Assert.That(handoff.Value.LaunchReceipt.Source,Is.EqualTo(scenario.Source),scenario.Name+" source");
                Assert.That(handoff.Value.LaunchReceipt.SourceRevision,Is.EqualTo(scenario.SourceRevision),scenario.Name+" source revision");
                Assert.That(handoff.Value.LaunchReceipt.FinalRevision,Is.EqualTo(scenario.ExpectedFinalRevision),scenario.Name+" final revision");
                Assert.That(handoff.Value.LaunchReceipt.PersistenceOutcome,Is.EqualTo(scenario.Outcome),scenario.Name+" save outcome");
                Assert.That(handoff.Value.LaunchReceipt.PersistenceStage,Is.EqualTo(scenario.Save.Stage),scenario.Name+" save stage");
                Assert.That(handoff.Value.LaunchReceipt.PersistencePending,Is.EqualTo(scenario.ExpectedPending),scenario.Name+" pending");
                Assert.That(handoff.Value.LaunchReceipt.MayPersist,Is.EqualTo(scenario.ExpectedMayPersist),scenario.Name+" may persist");
                Assert.That(handoff.Value.LaunchReceipt.PreservationStoppedRole,Is.EqualTo(scenario.ExpectedStoppedRole),scenario.Name+" stopped role");
                if(scenario.Preservation.HasValue)
                {
                    var expectedPreservation=scenario.Preservation.Value;var actualPreservation=handoff.Value.LaunchReceipt.Preservation;
                    Assert.That(actualPreservation.TempState,Is.EqualTo(expectedPreservation.TempState),scenario.Name+" temp preservation");
                    Assert.That(actualPreservation.PrimaryState,Is.EqualTo(expectedPreservation.PrimaryState),scenario.Name+" primary preservation");
                    Assert.That(actualPreservation.PreviousState,Is.EqualTo(expectedPreservation.PreviousState),scenario.Name+" previous preservation");
                }
                Assert.That(handoff.Value.InputMode,Is.EqualTo(InputMode.UIOnly),scenario.Name+" mode");
                Assert.That(context.Router.EffectiveMode,Is.EqualTo(InputMode.UIOnly),scenario.Name+" router mode");
                Assert.That(scenario.Actions.Created.Count,Is.EqualTo(scenario.BindingRepair?2:1),scenario.Name+" candidates");
                Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.SameAs(scenario.Actions.Created[scenario.Actions.Created.Count-1]),scenario.Name+" adopted candidate");
                ProfileLaunchNotificationV1 notification;var hasNotification=context.Latch.TryTakeLaunchNotification(out notification);
                Assert.That(hasNotification,Is.EqualTo(scenario.Notification),scenario.Name+" notification");
                if(hasNotification) { Assert.That(notification.Kind,Is.EqualTo(scenario.NotificationKind),scenario.Name+" notification kind");Assert.That(HubEntryHandoffReceiptV1.SameReceipt(notification.Receipt,handoff.Value.LaunchReceipt),Is.True,scenario.Name+" notification receipt correlation"); }
                Assert.That(context.Latch.TryTakeLaunchNotification(out notification),Is.False,scenario.Name+" one local consumption");
            }
            finally{context.Dispose();}
        }

        // AC-M5D7N-003: source consumption is single-shot; all four adversarial
        // notification shapes terminate before publication and cannot be reread.
        [TestCase("PreConsumed")]
        [TestCase("Unexpected")]
        [TestCase("CorruptKind")]
        [TestCase("CorruptCorrelation")]
        public void NotificationAdversary_TerminalizesWithoutHandoffOrRetry(string mode)
        {
            var context=Create(mode=="Unexpected");Context donor=null;
            try
            {
                Invoke(context.Adapter,"Start");
                if(mode=="PreConsumed") { ProfileLaunchNotificationV1 taken;Assert.That(context.Adapter.TryTakeNotification(out taken),Is.True); }
                else if(mode=="Unexpected")
                {
                    // Expected-none must receive a valid, transferred source payload:
                    // a default-launch donor supplies a real RecoveryCompleted proof.
                    donor=Create(false);Invoke(donor.Adapter,"Start");ProfileLaunchNotificationV1 taken;
                    Assert.That(donor.Adapter.TryTakeNotification(out taken),Is.True);
                    taken.Validate();SetPrivate(context.Adapter,"_notification",taken);
                }
                else
                {
                    var value=(ProfileLaunchNotificationV1)typeof(DesktopProfileLaunchAdapterV1).GetField("_notification",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(context.Adapter);var boxed=(object)value;
                    var field=typeof(ProfileLaunchNotificationV1).GetField(mode=="CorruptKind"?"_kind":"_correlation",BindingFlags.Instance|BindingFlags.NonPublic);
                    field.SetValue(boxed,mode=="CorruptKind"?(object)(ProfileLaunchNotificationKindV1)999:default(DesktopProfileLaunchReceiptV1));SetPrivate(context.Adapter,"_notification",boxed);
                }
                Invoke(context.Latch,"Start");Assert.That(()=>Invoke(context.Latch,"Update"),Throws.InstanceOf<TargetInvocationException>(),mode);
                Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.Failed),mode);Assert.That(context.Latch.CurrentHandoff,Is.Null,mode);Assert.That(()=>Invoke(context.Latch,"Update"),Throws.Nothing,mode);
            }
            finally{if(donor!=null)donor.Dispose();context.Dispose();}
        }

        [Test]
        public void PublishedLatch_DoesNotRereadMutatedSourceReceipt()
        {
            var context=Create(true);var receiptField=typeof(DesktopProfileLaunchAdapterV1).GetField("_currentReceipt",BindingFlags.Instance|BindingFlags.NonPublic);object originalReceipt=null;
            try
            {
                Invoke(context.Adapter,"Start");originalReceipt=receiptField.GetValue(context.Adapter);Invoke(context.Latch,"Start");Invoke(context.Latch,"Update");var published=context.Latch.CurrentHandoff;
                SetPrivate(context.Adapter,"_currentReceipt",default(DesktopProfileLaunchReceiptV1));
                Assert.That(()=>Invoke(context.Latch,"Update"),Throws.Nothing);
                Assert.That(context.Latch.CurrentHandoff,Is.EqualTo(published));
            }
            finally{if(originalReceipt!=null)receiptField.SetValue(context.Adapter,originalReceipt);context.Dispose();}
        }

        [Test]
        public void LatchHasNoPreparationPersistenceOrActionOwnershipAuthority()
        {
            foreach(var field in typeof(HubEntryHandoffLatchV1).GetFields(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public))
                Assert.That(field.FieldType,Is.Not.EqualTo(typeof(GameInputActions)),field.Name);
            foreach(var method in typeof(HubEntryHandoffLatchV1).GetMethods(BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.DeclaredOnly))
            {
                Assert.That(method.Name,Does.Not.Contain("Prepare"),method.Name);
                Assert.That(method.Name,Does.Not.Contain("Save"),method.Name);
                Assert.That(method.Name,Does.Not.Contain("Preserve"),method.Name);
            }
        }

        [Test]
        public void LatchSourceHasNoForbiddenScenePersistenceRouterOrExternalAuthority()
        {
            var sourcePath=Path.Combine(Directory.GetParent(Application.dataPath).FullName,"Assets","AcadeGameMaker","Runtime","Input","Unity","HubEntryHandoffLatchV1.cs");
            var source=File.ReadAllText(sourcePath);
            // The receipt/notification types below are the narrow immutable proof
            // boundary.  These checks deliberately distinguish them from owning
            // profile preparation, scene/UI, router-mode, IO, network or RNG APIs.
            Assert.That(source,Does.Contain("DesktopProfileLaunchReceiptV1"));
            Assert.That(source,Does.Contain("ProfileLaunchNotificationV1"));
            foreach(var forbidden in new[]{"UnityEngine.SceneManagement","TMPro","EventSystem","GameObject.Find","FindObject","LoadScene","RequestMode","ProfileLaunchPreparationCoordinatorV1","System.IO","File.","Directory.","Debug.Log","System.Net","UnityEngine.Random","ProfileAtomicSave",".Save(",".Preserve(","UnityWebRequest","HttpClient","GameInputActions","SetMode","ConfigureFor"})
                Assert.That(source,Does.Not.Contain(forbidden),forbidden);
        }

        [Test]
        public void ReceiptCorrelationUsesValueTypeBackingFieldEqualityAndSharedProofIdentity()
        {
            // A custom Equals could silently widen correlation semantics; all three
            // proof layers rely on ValueType backing-field equality, including
            // the shared preparation-proof identity inside the source receipt.
            Assert.That(typeof(DesktopProfileLaunchReceiptV1).GetMethod("Equals",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly),Is.Null);
            Assert.That(typeof(ProfileLaunchNotificationV1).GetMethod("Equals",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly),Is.Null);
            Assert.That(typeof(HubEntryHandoffReceiptV1).GetMethod("Equals",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.DeclaredOnly),Is.Null);
            var first=Create(false);var second=Create(false);
            try
            {
                Invoke(first.Adapter,"Start");Invoke(second.Adapter,"Start");
                var exact=first.Adapter.CurrentReceipt.Value;var copied=exact;var independent=second.Adapter.CurrentReceipt.Value;
                Assert.That(HubEntryHandoffReceiptV1.SameReceipt(exact,copied),Is.True,"exact copied receipt");
                Assert.That(HubEntryHandoffReceiptV1.SameReceipt(exact,independent),Is.False,"independent valid proof");
                var handoff=new HubEntryHandoffReceiptV1(exact);var copiedHandoff=handoff;var independentHandoff=new HubEntryHandoffReceiptV1(independent);
                Assert.That(handoff.Equals(copiedHandoff),Is.True,"exact copied handoff");Assert.That(handoff.Equals(independentHandoff),Is.False,"independent handoff proof");
                var mutatedHandoff=(object)handoff;typeof(HubEntryHandoffReceiptV1).GetField("_accepted",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(mutatedHandoff,false);
                Assert.That(((HubEntryHandoffReceiptV1)mutatedHandoff).Equals(handoff),Is.False,"mutated handoff proof");
                var notification=(ProfileLaunchNotificationV1)typeof(DesktopProfileLaunchAdapterV1).GetField("_notification",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(first.Adapter);
                var copiedNotification=notification;var independentNotification=(ProfileLaunchNotificationV1)typeof(DesktopProfileLaunchAdapterV1).GetField("_notification",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(second.Adapter);
                Assert.That(notification.Equals(copiedNotification),Is.True,"exact copied notification");Assert.That(notification.Equals(independentNotification),Is.False,"independent notification proof");
                var mutatedNotification=(object)notification;typeof(ProfileLaunchNotificationV1).GetField("_kind",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(mutatedNotification,(ProfileLaunchNotificationKindV1)999);
                Assert.That(((ProfileLaunchNotificationV1)mutatedNotification).Equals(notification),Is.False,"mutated notification proof");
            }
            finally{first.Dispose();second.Dispose();}
        }

        [TestCase("OtherAdapterGameObject")]
        [TestCase("ForeignSerializedRouter")]
        [TestCase("DisabledAdapter")]
        [TestCase("DisabledRouter")]
        [TestCase("InactiveRouter")]
        [TestCase("FaultedRouter")]
        [TestCase("NonUiRouter")]
        [TestCase("MissingReceipt")]
        [TestCase("MalformedReceipt")]
        public void InvalidEntryCase_TerminalizesWithoutPublicationOrRetry(string kind)
        {
            var context=kind=="InactiveRouter"?CreateOrdered("SeparateRouterFirst"):Create(false);GameObject foreign=null;
            var originalFaulted=GetPrivate<bool>(context.Router,"_faulted");var originalMode=GetPrivate<InputMode>(context.Router,"_mode");
            var receiptField=typeof(DesktopProfileLaunchAdapterV1).GetField("_currentReceipt",BindingFlags.Instance|BindingFlags.NonPublic);object originalReceipt=null;
            try
            {
                Invoke(context.Adapter,"Start");originalReceipt=receiptField.GetValue(context.Adapter);var original=GetPrivate<GameInputActions>(context.Router,"_actions");
                if(kind=="OtherAdapterGameObject") { foreign=new GameObject("m5d7n-other-adapter");foreign.SetActive(false);SetPrivate(context.Latch,"_launchAdapter",foreign.AddComponent<DesktopProfileLaunchAdapterV1>()); }
                else if(kind=="ForeignSerializedRouter") { foreign=new GameObject("m5d7n-foreign-router");foreign.SetActive(false);SetPrivate(context.Latch,"_router",foreign.AddComponent<InputRouter>()); }
                else if(kind=="InactiveRouter") context.Router.gameObject.SetActive(false);
                else if(kind=="DisabledAdapter") context.Adapter.enabled=false;
                else if(kind=="DisabledRouter") context.Router.enabled=false;
                else if(kind=="FaultedRouter") SetPrivate(context.Router,"_faulted",true);
                else if(kind=="NonUiRouter") SetPrivate(context.Router,"_mode",InputMode.GameplayEnabled);
                else if(kind=="MissingReceipt") SetPrivate(context.Adapter,"_currentReceipt",null);
                else if(kind=="MalformedReceipt") SetPrivate(context.Adapter,"_currentReceipt",default(DesktopProfileLaunchReceiptV1));
                var updateOnly=kind=="FaultedRouter"||kind=="NonUiRouter"||kind=="MissingReceipt"||kind=="MalformedReceipt";
                if(!updateOnly) Assert.That(()=>Invoke(context.Latch,"Start"),Throws.InstanceOf<TargetInvocationException>(),kind);
                else { Invoke(context.Latch,"Start");Assert.That(()=>Invoke(context.Latch,"Update"),Throws.InstanceOf<TargetInvocationException>(),kind); }
                Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.Failed),kind);Assert.That(context.Latch.CurrentHandoff,Is.Null,kind);
                Assert.That(GetPrivate<GameInputActions>(context.Router,"_actions"),Is.SameAs(original),kind+" router action identity");
                Assert.That(()=>Invoke(context.Latch,"Update"),Throws.Nothing,kind+" terminal no retry");
            }
            finally
            {
                // Restore hostile reflected fields only after the terminal/no-retry
                // assertions, so normal Unity teardown validates without stray logs.
                if(kind=="FaultedRouter") SetPrivate(context.Router,"_faulted",originalFaulted);
                if(kind=="NonUiRouter") SetPrivate(context.Router,"_mode",originalMode);
                if(kind=="MissingReceipt"||kind=="MalformedReceipt") receiptField.SetValue(context.Adapter,originalReceipt);
                if(foreign!=null)UnityEngine.Object.DestroyImmediate(foreign);context.Dispose();
            }
        }

        [TestCase("Before","Disable")]
        [TestCase("Before","Destroy")]
        [TestCase("After","Disable")]
        [TestCase("After","Destroy")]
        public void TeardownMomentAndKind_CloseOnlyBeforePublication_AndNeverRetract(string moment,string kind)
        {
            var context=Create(false);
            try
            {
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");
                HubEntryHandoffReceiptV1? published=null;
                if(moment=="After") { Invoke(context.Latch,"Update");published=context.Latch.CurrentHandoff;Assert.That(published.HasValue,Is.True); }
                Invoke(context.Latch,kind=="Disable"?"OnDisable":"OnDestroy");
                Assert.That(context.Latch.CurrentHandoff,Is.EqualTo(published),moment+kind);
                Assert.That(()=>Invoke(context.Latch,"Update"),Throws.Nothing,moment+kind+" no retry");
                if(moment=="Before") Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.ClosedBeforePublication),moment+kind);
            }
            finally{context.Dispose();}
        }

        [Test]
        public void ReactivationAndDuplicateUpdate_DoNotRepublishOrReconsume()
        {
            var context=Create(false);
            try
            {
                Invoke(context.Adapter,"Start");Invoke(context.Latch,"Start");Invoke(context.Latch,"Update");var receipt=context.Latch.CurrentHandoff;
                var publishedState=State(context.Latch);
                context.Latch.enabled=false; // actual Unity OnDisable dispatch
                context.Latch.enabled=true;  // OnEnable has no source authority; Start is not replayed
                Assert.That(State(context.Latch),Is.EqualTo(publishedState),"reactivation terminal state");
                Invoke(context.Latch,"Update");Invoke(context.Latch,"Update");
                Assert.That(context.Latch.CurrentHandoff,Is.EqualTo(receipt));
                ProfileLaunchNotificationV1 notification;Assert.That(context.Latch.TryTakeLaunchNotification(out notification),Is.True);
                Assert.That(context.Latch.TryTakeLaunchNotification(out notification),Is.False);
            }
            finally{context.Dispose();}
        }

        [TestCase("SameRouterFirst")]
        [TestCase("SameAdapterFirst")]
        [TestCase("SeparateRouterFirst")]
        [TestCase("SeparateAdapterFirst")]
        public void AuthoredLifecycleCreationOrders_ConvergeOnlyAtFirstUpdate(string order)
        {
            var context=CreateOrdered(order);
            try
            {
                Assert.That(State(context.Latch),Is.EqualTo((int)HubEntryHandoffStateV1.AwaitingStart),order+" Awake only");
                Assert.That(context.Latch.CurrentHandoff,Is.Null,order+" no Awake handoff");
                Invoke(context.Adapter,"Start");Assert.That(context.Latch.CurrentHandoff,Is.Null,order+" no adapter Start handoff");
                Invoke(context.Latch,"Start");Assert.That(context.Latch.CurrentHandoff,Is.Null,order+" no latch Start handoff");
                Invoke(context.Latch,"Update");Assert.That(context.Latch.CurrentHandoff.HasValue,Is.True,order+" first Update handoff");
            }
            finally{context.Dispose();}
        }

        private static int State(HubEntryHandoffLatchV1 latch) { return Convert.ToInt32(typeof(HubEntryHandoffLatchV1).GetField("_state",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(latch)); }
        private static void SetPrivate(object target,string name,object value) { var field=target.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic);field.SetValue(target,field.FieldType.IsEnum?Enum.ToObject(field.FieldType,value):value); }
        private static T GetPrivate<T>(object target,string name) { return (T)target.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(target); }
        private static void Invoke(object target,string name) { target.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(target,null); }
        private static object HandoffReceiptBadValue(string field) { return field=="_mode"?(object)InputMode.GameplayEnabled:field=="_gameplay"?(object)true:field=="_ui"?(object)false:field=="_accepted"?(object)false:default(DesktopProfileLaunchReceiptV1); }

        private static HubEntryHandoffReceiptV1 ReadPublishedHandoffDirectly(HubEntryHandoffLatchV1 latch)
        {
            var handoff=typeof(HubEntryHandoffLatchV1).GetField("_handoff",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(latch);
            Assert.That(handoff,Is.Not.Null,"coherent published fixture handoff");
            return (HubEntryHandoffReceiptV1)handoff;
        }

        // REQ-M5D7N-006/AC-M5D7N-008: hostile mutations remain observable through
        // getter/take/Update first.  Afterwards use a cheap, contract-valid
        // prepublication state so Unity's real OnDisable/OnDestroy path does not
        // revalidate the intentionally exercised published receipt during cleanup.
        // Dedicated teardown-retention rows deliberately do not call this helper.
        private static void NormalizeLatchForCleanup(HubEntryHandoffLatchV1 latch)
        {
            SetPrivate(latch,"_handoff",null);SetPrivate(latch,"_handoffProof",null);SetPrivate(latch,"_notification",null);SetPrivate(latch,"_ownedNotificationProof",null);SetPrivate(latch,"_expectedNotification",false);
            SetPrivate(latch,"_sourceNotificationProbed",false);SetPrivate(latch,"_sourceNotificationTaken",false);SetPrivate(latch,"_localNotificationDelivered",false);
            SetPrivate(latch,"_retainedFailureNotification",null);SetPrivate(latch,"_state",HubEntryHandoffStateV1.Pristine);
        }

        private static readonly DateTimeOffset Utc=new DateTimeOffset(2026,9,13,11,12,13,TimeSpan.Zero);
        private static IEnumerable<TestCaseData> ScenarioTable
        {
            get
            {
                yield return new TestCaseData(LaunchScenario.CleanPrimary()).SetName("CleanPrimary");
                yield return new TestCaseData(LaunchScenario.Default()).SetName("Default");
                yield return new TestCaseData(LaunchScenario.Previous()).SetName("Previous");
                yield return new TestCaseData(LaunchScenario.DecodeRepair()).SetName("DecodeRepair");
                yield return new TestCaseData(LaunchScenario.BindingApplyRepair()).SetName("BindingApplyRepair");
                yield return new TestCaseData(LaunchScenario.PreservationDenied()).SetName("PreservationDenied");
                yield return new TestCaseData(LaunchScenario.SaveFailed()).SetName("SaveFailed");
                yield return new TestCaseData(LaunchScenario.CommitUncertain()).SetName("CommitUncertain");
                yield return new TestCaseData(LaunchScenario.PreservationOnly()).SetName("PreservationOnly");
            }
        }

        private static Context Create(bool cleanPrimary)
        {
            var objects=new List<GameObject>();var root=Path.Combine(Path.GetTempPath(),"m5d7n-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);if(cleanPrimary)File.WriteAllBytes(Path.Combine(root,"profile.json"),Document(4).CanonicalFileBytes);
            var rig=RouterTests.CreateRig(objects);var cameraObject=new GameObject("m5d7n-camera");objects.Add(cameraObject);var camera=cameraObject.AddComponent<CameraPort>();camera.Player=rig.Player;
            var go=new GameObject("m5d7n-authored-cohort");objects.Add(go);go.SetActive(false);var router=go.AddComponent<InputRouter>();var adapter=go.AddComponent<DesktopProfileLaunchAdapterV1>();var latch=go.AddComponent<HubEntryHandoffLatchV1>();router.ConfigureForAuthoring(rig.Player,rig.Transfer,rig.Combat,camera,InputMode.UIOnly);SetPrivate(adapter,"_router",router);SetPrivate(adapter,"_environment",new EnvironmentPort(root,Utc));SetPrivate(adapter,"_preparation",new DesktopProfileLaunchPreparationPortV1());SetPrivate(latch,"_launchAdapter",adapter);SetPrivate(latch,"_router",router);go.SetActive(true);return new Context(objects,root,router,adapter,latch);
        }

        private static Context CreateScenario(LaunchScenario scenario)
        {
            var objects=new List<GameObject>();var root=Path.Combine(Path.GetTempPath(),"m5d7n-scenario-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);
            var rig=RouterTests.CreateRig(objects);var cameraObject=new GameObject("m5d7n-scenario-camera");objects.Add(cameraObject);var camera=cameraObject.AddComponent<CameraPort>();camera.Player=rig.Player;
            var go=new GameObject("m5d7n-authored-scenario");objects.Add(go);go.SetActive(false);var router=go.AddComponent<InputRouter>();var adapter=go.AddComponent<DesktopProfileLaunchAdapterV1>();var latch=go.AddComponent<HubEntryHandoffLatchV1>();
            router.ConfigureForAuthoring(rig.Player,rig.Transfer,rig.Combat,camera,InputMode.UIOnly);
            scenario.Preparation=new ScenarioPreparation(scenario);
            SetPrivate(adapter,"_router",router);SetPrivate(adapter,"_environment",new EnvironmentPort(root,Utc));SetPrivate(adapter,"_preparation",scenario.Preparation);
            SetPrivate(latch,"_launchAdapter",adapter);SetPrivate(latch,"_router",router);go.SetActive(true);return new Context(objects,root,router,adapter,latch);
        }
        private static Context CreateOrdered(string order)
        {
            var objects=new List<GameObject>();var root=Path.Combine(Path.GetTempPath(),"m5d7n-order-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);var rig=RouterTests.CreateRig(objects);var cameraObject=new GameObject("m5d7n-order-camera");objects.Add(cameraObject);var camera=cameraObject.AddComponent<CameraPort>();camera.Player=rig.Player;
            var separate=order.StartsWith("Separate",StringComparison.Ordinal);var routerFirst=order.EndsWith("RouterFirst",StringComparison.Ordinal);var host=new GameObject("m5d7n-order-host");objects.Add(host);host.SetActive(false);var routerHost=separate?new GameObject("m5d7n-order-router"):host;if(separate){objects.Add(routerHost);routerHost.SetActive(false);}InputRouter router=null;DesktopProfileLaunchAdapterV1 adapter=null;HubEntryHandoffLatchV1 latch=null;
            if(routerFirst) router=routerHost.AddComponent<InputRouter>();adapter=host.AddComponent<DesktopProfileLaunchAdapterV1>();if(!routerFirst)router=routerHost.AddComponent<InputRouter>();latch=host.AddComponent<HubEntryHandoffLatchV1>();
            router.ConfigureForAuthoring(rig.Player,rig.Transfer,rig.Combat,camera,InputMode.UIOnly);SetPrivate(adapter,"_router",router);SetPrivate(adapter,"_environment",new EnvironmentPort(root,Utc));SetPrivate(adapter,"_preparation",new DesktopProfileLaunchPreparationPortV1());SetPrivate(latch,"_launchAdapter",adapter);SetPrivate(latch,"_router",router);if(separate)routerHost.SetActive(true);host.SetActive(true);return new Context(objects,root,router,adapter,latch);
        }
        private static ProfileCanonicalDocumentV1 Document(long revision)
        {
            var input=new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId,ProfileInputContract.CurrentBindingSchemaVersion,ProfileBindingOverridesJson.Parse(string.Empty));
            return ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed,500,500,500,false,false),input,new ProfileTutorialSnapshot(Array.Empty<string>()),new ProfileProgressionSnapshot(revision,null,null,ProfileConsentState.RefusesOwnershipTransfer,null,Array.Empty<ProfileBranch>())));
        }
        private static ProfileCanonicalDocumentV1 Document(long revision,string asset,string overrides)
        {
            var input=new ProfileInputSnapshot(asset,ProfileInputContract.CurrentBindingSchemaVersion,ProfileBindingOverridesJson.Parse(overrides));
            return ProfileCanonicalEncoder.Encode(new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed,500,500,500,false,false),input,new ProfileTutorialSnapshot(Array.Empty<string>()),new ProfileProgressionSnapshot(revision,null,null,ProfileConsentState.RefusesOwnershipTransfer,null,Array.Empty<ProfileBranch>())));
        }
        private static ProfileLoadCandidateV1 Candidate(ProfileLoadFileRole role,ProfileCanonicalDocumentV1 document) { return ProfileLoadCandidateV1.Decoded(role,ProfileCanonicalDecoderV1.Decode(document.CanonicalFileBytes)); }
        private static ProfileLaunchObservationProjectionV1 EmptyObservation() { return new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)); }
        internal sealed class LaunchScenario
        {
            internal string Name; internal ProfileLaunchObservationProjectionV1 Observation; internal ProfileLaunchPreservationProjectionV1? Preservation; internal ProfileLaunchSaveProjectionV1 Save;
            internal ProfileLaunchSource Source; internal long SourceRevision; internal ProfileLaunchPersistenceOutcomeV1 Outcome; internal bool Notification; internal ProfileLaunchNotificationKindV1 NotificationKind; internal bool BindingRepair; internal TrackingActions Actions; internal ScenarioPreparation Preparation;
            internal long ExpectedFinalRevision { get { if(Name=="DecodeRepair")return 9;if(Name=="BindingApplyRepair")return 3;if(Name=="Previous")return 5;if(Name=="Default"||Name=="PreservationDenied"||Name=="SaveFailed"||Name=="CommitUncertain")return 0;return SourceRevision; } }
            internal bool ExpectedPending { get { return Outcome==ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation||Outcome==ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit||Outcome==ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain; } }
            internal bool ExpectedMayPersist { get { return !Preservation.HasValue||Preservation.Value.MayPersist; } }
            internal ProfileLoadFileRole? ExpectedStoppedRole { get { return Preservation.HasValue?Preservation.Value.StoppedRole:null; } }
            private LaunchScenario(string name,ProfileLaunchObservationProjectionV1 observation,ProfileLaunchPreservationProjectionV1? preservation,ProfileLaunchSaveProjectionV1 save,ProfileLaunchSource source,long sourceRevision,bool notification,ProfileLaunchNotificationKindV1 kind,bool bindingRepair=false)
            { Name=name;Observation=observation;Preservation=preservation;Save=save;Source=source;SourceRevision=sourceRevision;Outcome=save.Outcome;Notification=notification;NotificationKind=kind;BindingRepair=bindingRepair;Actions=new TrackingActions(bindingRepair); }
            internal static LaunchScenario CleanPrimary() { var d=Document(3);return new LaunchScenario("CleanPrimary",new ProfileLaunchObservationProjectionV1(Candidate(ProfileLoadFileRole.Primary,d),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)),null,new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.None,ProfileLaunchPersistenceOutcomeV1.NotRequired,ProfileAtomicSaveStage.None,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,null),ProfileLaunchSource.Primary,3,false,0); }
            internal static LaunchScenario Default() { return new LaunchScenario("Default",EmptyObservation(),null,Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst,0),ProfileLaunchSource.Default,-1,true,ProfileLaunchNotificationKindV1.RecoveryCompleted); }
            internal static LaunchScenario Previous() { var d=Document(4);return new LaunchScenario("Previous",new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary),Candidate(ProfileLoadFileRole.Previous,d),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)),null,Ordinary(ProfileLaunchPersistenceOutcomeV1.CommittedFirst,5),ProfileLaunchSource.Previous,4,true,ProfileLaunchNotificationKindV1.RecoveryCompleted); }
            internal static LaunchScenario DecodeRepair() { var d=Document(8,"obsolete",string.Empty);return new LaunchScenario("DecodeRepair",new ProfileLaunchObservationProjectionV1(Candidate(ProfileLoadFileRole.Primary,d),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)),null,Recovery(ProfileLaunchPersistenceOutcomeV1.CommittedPrimaryInputRecovery,9,ProfileAtomicSaveStage.None),ProfileLaunchSource.Primary,8,true,ProfileLaunchNotificationKindV1.RecoveryCompleted); }
            internal static LaunchScenario BindingApplyRepair() { var d=Document(2,ProfileInputContract.CurrentInputActionsAssetId,"[]");return new LaunchScenario("BindingApplyRepair",new ProfileLaunchObservationProjectionV1(Candidate(ProfileLoadFileRole.Primary,d),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)),null,new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,ProfileLaunchPersistenceOutcomeV1.CommittedReplacement,ProfileAtomicSaveStage.None,ProfileStoredFileState.ValidCurrentInput,ProfileStoredFileState.ValidCurrentInput,ProfileStoredFileState.Missing,3),ProfileLaunchSource.Primary,2,true,ProfileLaunchNotificationKindV1.RecoveryCompleted,true); }
            internal static LaunchScenario PreservationDenied() { var p=new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired,ProfileLaunchPreservationStateV1.FailedBeforeMove,ProfileLaunchPreservationStateV1.NotRequired,false,ProfileLoadFileRole.Primary,ProfileFilePreservationReason.None,ProfileFilePreservationReason.UnreadablePrimary,ProfileFilePreservationReason.None,1);return new LaunchScenario("PreservationDenied",new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Unreadable(ProfileLoadFileRole.Primary),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)),p,new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation,ProfileAtomicSaveStage.None,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,null),ProfileLaunchSource.Default,-1,true,ProfileLaunchNotificationKindV1.PersistenceDeferred); }
            internal static LaunchScenario SaveFailed() { return new LaunchScenario("SaveFailed",EmptyObservation(),null,new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit,ProfileAtomicSaveStage.DirectoryPreparation,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,null),ProfileLaunchSource.Default,-1,true,ProfileLaunchNotificationKindV1.PersistenceDeferred); }
            internal static LaunchScenario CommitUncertain() { return new LaunchScenario("CommitUncertain",EmptyObservation(),null,new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain,ProfileAtomicSaveStage.CommitCall,ProfileStoredFileState.Missing,ProfileStoredFileState.Missing,ProfileStoredFileState.Missing,null),ProfileLaunchSource.Default,-1,true,ProfileLaunchNotificationKindV1.PersistenceDeferred); }
            internal static LaunchScenario PreservationOnly() { var d=Document(7);var bad=ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Temp,ProfileCanonicalDecoderV1.Decode(System.Text.Encoding.UTF8.GetBytes("{}")));var p=new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.MoveOutcomeUncertain,ProfileLaunchPreservationStateV1.NotRequired,ProfileLaunchPreservationStateV1.NotRequired,false,ProfileLoadFileRole.Temp,ProfileFilePreservationReason.StaleTemp,ProfileFilePreservationReason.None,ProfileFilePreservationReason.None,1);return new LaunchScenario("PreservationOnly",new ProfileLaunchObservationProjectionV1(Candidate(ProfileLoadFileRole.Primary,d),ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous),bad),p,new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.None,ProfileLaunchPersistenceOutcomeV1.NotRequired,ProfileAtomicSaveStage.None,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,ProfileStoredFileState.NotProbed,null),ProfileLaunchSource.Primary,7,true,ProfileLaunchNotificationKindV1.RecoveryArtifactPreservationFailed); }
            private static ProfileLaunchSaveProjectionV1 Ordinary(ProfileLaunchPersistenceOutcomeV1 outcome,long revision) { return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary,outcome,ProfileAtomicSaveStage.None,ProfileStoredFileState.ValidCurrentInput,ProfileStoredFileState.Missing,ProfileStoredFileState.Missing,revision); }
            private static ProfileLaunchSaveProjectionV1 Recovery(ProfileLaunchPersistenceOutcomeV1 outcome,long revision,ProfileAtomicSaveStage stage) { return new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.PrimaryInputRecovery,outcome,stage,ProfileStoredFileState.ValidCurrentInput,ProfileStoredFileState.ValidInputRecoveryRequired,ProfileStoredFileState.Missing,revision); }
        }
        internal sealed class ScenarioPreparation : IDesktopProfileLaunchPreparationV1
        {
            private readonly LaunchScenario _scenario; internal int Calls;internal string Root;internal DateTimeOffset Utc;
            internal ScenarioPreparation(LaunchScenario scenario) { _scenario=scenario; }
            public PreparedProfileLaunchV1 Prepare(string root,DateTimeOffset utc) { Calls++;Root=root;Utc=utc;return ProfileLaunchPreparationCoordinatorV1.Prepare(root,utc,new ScenarioProfilePort(_scenario),_scenario.Actions); }
        }
        private sealed class ScenarioProfilePort : IProfileLaunchPreparationProfilePortV1
        {
            private readonly LaunchScenario _scenario;internal ScenarioProfilePort(LaunchScenario scenario){_scenario=scenario;}
            public ProfileLaunchObservationProjectionV1 Observe(string root){return _scenario.Observation;}
            public ProfileLaunchPreservationProjectionV1 Preserve(string root,ProfileLaunchObservationProjectionV1 observation,ProfileLoadSelectionPlanV1 selection,DateTimeOffset utc){return _scenario.Preservation??new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired,ProfileLaunchPreservationStateV1.NotRequired,ProfileLaunchPreservationStateV1.NotRequired,true,null,selection.TempPreservation,selection.PrimaryPreservation,selection.PreviousPreservation,0);}
            public ProfileLaunchSaveProjectionV1 Save(string root,ProfileCanonicalDocumentV1 document){return _scenario.Save;}
            public ProfileLaunchSaveProjectionV1 SaveInputRecovery(string root,ProfileLoadCandidateV1 primary){return _scenario.Save;}
        }
        internal sealed class TrackingActions : IProfileLaunchPreparationActionsPortV1
        {
            internal readonly List<GameInputActions> Created=new List<GameInputActions>();private readonly bool _bindingRepair;private int _apply;
            internal TrackingActions(bool bindingRepair){_bindingRepair=bindingRepair;}
            public GameInputActions Create(){var value=new GameInputActions();Created.Add(value);return value;}
            public ProfileBindingOverrideApplyResultV1 Apply(ProfileInputSnapshot input,GameInputActions candidate){_apply++;if(_apply==1&&_bindingRepair)return new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired,ProfileBindingOverrideApplyFailureV1.LoadFailed,ProfileBindingOverrideCandidateDispositionV1.Discard,"[]",null);return new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady,ProfileBindingOverrideApplyFailureV1.None,ProfileBindingOverrideCandidateDispositionV1.Retain,string.Empty,string.Empty);}
            public void Dispose(GameInputActions candidate){candidate.Dispose();}
        }
        private sealed class EnvironmentPort : IDesktopProfileLaunchEnvironmentV1
        { private readonly string _root;private readonly DateTimeOffset _utc;internal EnvironmentPort(string root,DateTimeOffset utc){_root=root;_utc=utc;}public string GetPersistentDataPath(){return _root;}public DateTimeOffset GetUtcNow(){return _utc;} }
        private sealed class CameraPort : MonoBehaviour,IInputCameraSnapshotProvider
        { internal PlayerMovementController Player;public PlayerMovementController BoundPlayer=>Player;public bool TryGetCompletedCamera(SimulationTick tick,out SimulationCameraPoseSnapshot snapshot){snapshot=default(SimulationCameraPoseSnapshot);return false;} }
        private sealed class Context : IDisposable
        { private readonly List<GameObject> _objects;private readonly string _root;internal Context(List<GameObject> objects,string root,InputRouter router,DesktopProfileLaunchAdapterV1 adapter,HubEntryHandoffLatchV1 latch){_objects=objects;_root=root;Router=router;Adapter=adapter;Latch=latch;}internal string Root=>_root;internal InputRouter Router{get;}internal DesktopProfileLaunchAdapterV1 Adapter{get;}internal HubEntryHandoffLatchV1 Latch{get;}public void Dispose(){RouterTests.DestroyAll(_objects);if(Directory.Exists(_root))Directory.Delete(_root,true);} }
    }
}
