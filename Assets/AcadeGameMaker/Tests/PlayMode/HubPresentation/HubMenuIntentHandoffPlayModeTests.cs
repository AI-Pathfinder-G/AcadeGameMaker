using System;
using System.Reflection;
using AcadeGameMaker.Hub.Presentation.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.HubPresentation
{
    // REQ-M5D7QB-002/004 / AC-M5D7QB-002/003: injected non-effectful values cover consumption and terminal rows.
    internal sealed class HubMenuIntentHandoffPlayModeTests
    {
        [Test] public void AC_M5D7QB_003_RequestIsTakenOnceThenUnavailable()
        { var fixture=OwnerFixture.Create(HubMenuIntentHandoffOwnerStateV1.RequestReady);try{HubMenuIntentRequestV1 request;Assert.That(fixture.Owner.TryTakeRequest(out request),Is.True);Assert.That(Read(request,"Item").ToString(),Is.EqualTo("Settings"));Assert.That(fixture.Owner.State,Is.EqualTo(HubMenuIntentHandoffOwnerStateV1.RequestTaken));Assert.That(fixture.Owner.TryTakeRequest(out request),Is.False);}finally{fixture.Dispose();} }

        [Test] public void AC_M5D7QB_003_AllLifecycleTeardownsCloseWithoutReissue()
        { foreach(var state in new[]{HubMenuIntentHandoffOwnerStateV1.AwaitingIntent,HubMenuIntentHandoffOwnerStateV1.RequestReady,HubMenuIntentHandoffOwnerStateV1.RequestTaken}){var fixture=OwnerFixture.Create(state);try{Invoke(fixture.Owner,"Close");HubMenuIntentRequestV1 request;Assert.That(fixture.Owner.State,Is.EqualTo(HubMenuIntentHandoffOwnerStateV1.Closed),state.ToString());Assert.That(fixture.Owner.TryTakeRequest(out request),Is.False);}finally{fixture.Dispose();}} }

        [Test] public void AC_M5D7QB_002_ReflectionRequestProofMismatchLatchesFailed()
        { var fixture=OwnerFixture.Create(HubMenuIntentHandoffOwnerStateV1.RequestReady);try{Set(fixture.Owner,"_requestProof",null);HubMenuIntentRequestV1 ignored;Assert.Throws<InvalidOperationException>(()=>fixture.Owner.TryTakeRequest(out ignored));Assert.That(fixture.Owner.State,Is.EqualTo(HubMenuIntentHandoffOwnerStateV1.Failed));}finally{fixture.Dispose();} }

        [Test] public void AC_M5D7QB_004_OwnerExecutionOrderFollowsPresenter()
        {var attribute=(DefaultExecutionOrder)Attribute.GetCustomAttribute(typeof(HubMenuIntentHandoffOwnerV1),typeof(DefaultExecutionOrder));Assert.That(attribute,Is.Not.Null);Assert.That(attribute.order,Is.EqualTo(-170));}

        static Type InputType(string name){return Type.GetType("AcadeGameMaker.Input.Unity."+name+", AcadeGameMaker.Input.Unity",true);}static object ValidReceipt(){var type=InputType("HubEntryHandoffReceiptV1");object boxed=Activator.CreateInstance(type);var mode=type.GetField("_mode",BindingFlags.Instance|BindingFlags.NonPublic);mode.SetValue(boxed,Enum.Parse(mode.FieldType,"UIOnly"));type.GetField("_ui",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(boxed,true);type.GetField("_accepted",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(boxed,true);return boxed;}static HubMenuIntentRequestV1 Request(){var type=typeof(HubMenuIntentRequestV1);return (HubMenuIntentRequestV1)type.GetConstructors(BindingFlags.Instance|BindingFlags.NonPublic)[0].Invoke(new[]{Enum.Parse(InputType("HubMenuItemV1"),"Settings"),ValidReceipt()});}static object Read(object value,string name){return value.GetType().GetProperty(name,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(value,null);}static void Set(object target,string name,object value){target.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(target,value);}static void Invoke(object target,string name){try{target.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(target,null);}catch(TargetInvocationException error){if(error.InnerException!=null)throw error.InnerException;throw;}}
        sealed class OwnerFixture:IDisposable{internal readonly GameObject Root;internal readonly HubMenuIntentHandoffOwnerV1 Owner;OwnerFixture(GameObject root,HubMenuIntentHandoffOwnerV1 owner){Root=root;Owner=owner;}internal static OwnerFixture Create(HubMenuIntentHandoffOwnerStateV1 state){var root=new GameObject("M5D7QB owner fixture");root.SetActive(false);var owner=root.AddComponent<HubMenuIntentHandoffOwnerV1>();var request=Request();Set(owner,"_state",state);if(state==HubMenuIntentHandoffOwnerStateV1.RequestReady){Set(owner,"_request",request);Set(owner,"_requestProof",request);}if(state==HubMenuIntentHandoffOwnerStateV1.RequestTaken){Set(owner,"_takenRequest",request);Set(owner,"_takenRequestProof",request);}return new OwnerFixture(root,owner);}public void Dispose(){UnityEngine.Object.DestroyImmediate(Root);}}
    }
}
