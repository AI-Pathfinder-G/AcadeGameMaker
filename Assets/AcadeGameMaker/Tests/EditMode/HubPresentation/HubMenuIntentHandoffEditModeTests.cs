using System;
using System.Linq;
using System.Reflection;
using AcadeGameMaker.Hub.Authoring.Editor;
using AcadeGameMaker.Hub.Presentation.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.HubPresentation
{
    // REQ-M5D7QB-001..005 / AC-M5D7QB-001/002/004: injected values exercise
    // the same one-way boundary without constructing any product destination.
    internal sealed class HubMenuIntentHandoffEditModeTests
    {
        [Test] public void AC_M5D7QB_001_AllKnownItemsTransferExactlyOnce()
        {
            foreach(var name in new[]{"Continue","NewGame","Settings","Quit"})
            {
                var fixture=Fixture.Create(name); try { Set(fixture.Owner,"_state",HubMenuIntentHandoffOwnerStateV1.AwaitingIntent); Invoke(fixture.Owner,"LateUpdate"); Assert.That(fixture.Owner.State,Is.EqualTo(HubMenuIntentHandoffOwnerStateV1.RequestReady)); HubMenuIntentRequestV1 request; Assert.That(fixture.Owner.TryTakeRequest(out request),Is.True,name); Assert.That(Read(request,"Item").ToString(),Is.EqualTo(name)); Assert.That(Read(request,"Receipt").Equals(fixture.Receipt),Is.True); Assert.That(fixture.Presenter.State,Is.EqualTo(HubPresenterStateV1.IntentRetained)); Assert.That(fixture.Presenter.TryTakeRetainedIntent(fixture.Owner,out request),Is.False); }
                finally { fixture.Dispose(); }
            }
        }

        [Test] public void AC_M5D7QB_002_ForeignAndPreRetentionTransfersCloseThePresenter()
        {
            var fixture=Fixture.Create("NewGame");var foreignRoot=new GameObject("M5D7QB foreign");foreignRoot.SetActive(false);try {var foreign=foreignRoot.AddComponent<HubMenuIntentHandoffOwnerV1>();HubMenuIntentRequestV1 ignored;Assert.Throws<InvalidOperationException>(()=>fixture.Presenter.TryTakeRetainedIntent(foreign,out ignored));Assert.That(fixture.Presenter.State,Is.EqualTo(HubPresenterStateV1.Failed));}finally{UnityEngine.Object.DestroyImmediate(foreignRoot);fixture.Dispose();}
            var pre=Fixture.Create("Quit",false);try {HubMenuIntentRequestV1 ignored;Assert.That(pre.Presenter.TryTakeRetainedIntent(pre.Owner,out ignored),Is.False);}finally{pre.Dispose();}
        }

        [Test] public void AC_M5D7QB_002_UnknownIntentMalformedReceiptAndDuplicateOrUnboundOwnerFailClosed()
        {
            var unknown=Fixture.Create("Continue");try{var intent=ReadField(unknown.Presenter,"_retainedIntent");Assert.That(intent.GetType(),Is.EqualTo(InputType("HubMenuIntentV1")));Set(intent,"_item",Enum.ToObject(InputType("HubMenuItemV1"),999));Set(unknown.Presenter,"_retainedIntent",intent);HubMenuIntentRequestV1 ignored;Assert.Throws<InvalidOperationException>(()=>unknown.Presenter.TryTakeRetainedIntent(unknown.Owner,out ignored));Assert.That(unknown.Presenter.State,Is.EqualTo(HubPresenterStateV1.Failed));}finally{unknown.Dispose();}
            var malformed=Fixture.Create("Continue");try{Set(malformed.Presenter,"_handoffProof",Activator.CreateInstance(InputType("HubEntryHandoffReceiptV1")));HubMenuIntentRequestV1 ignored;Assert.Throws<InvalidOperationException>(()=>malformed.Presenter.TryTakeRetainedIntent(malformed.Owner,out ignored));Assert.That(malformed.Presenter.State,Is.EqualTo(HubPresenterStateV1.Failed));}finally{malformed.Dispose();}
            var authored=AssetDatabase.LoadAssetAtPath<GameObject>(HubPresentationAuthoringBuilder.MenuPrefabPath);Assert.That(authored.GetComponents<HubMenuIntentHandoffOwnerV1>(),Has.Length.EqualTo(1));Assert.That(Attribute.GetCustomAttribute(typeof(HubMenuIntentHandoffOwnerV1),typeof(DisallowMultipleComponent)),Is.Not.Null);Assert.DoesNotThrow(()=>HubPresentationAuthoringValidator.ValidateMenuAsset(authored));
            var unbound=Fixture.Create("Settings");try{Set(unbound.Owner,"_presenter",null);Set(unbound.Owner,"_state",HubMenuIntentHandoffOwnerStateV1.AwaitingIntent);Invoke(unbound.Owner,"LateUpdate");Assert.That(unbound.Owner.State,Is.EqualTo(HubMenuIntentHandoffOwnerStateV1.Failed));}finally{unbound.Dispose();}
        }

        [Test] public void AC_M5D7QB_004_PrefabAndRequestSurfaceAreClosed()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(HubPresentationAuthoringBuilder.MenuPrefabPath);var presenter=root.GetComponent<HubMenuPresenterV1>();var owners=root.GetComponents<HubMenuIntentHandoffOwnerV1>();Assert.That(owners,Has.Length.EqualTo(1));Assert.That(ReferenceEquals(presenter.HandoffOwner,owners[0]),Is.True);Assert.That(ReferenceEquals(owners[0].Presenter,presenter),Is.True);HubPresentationAuthoringValidator.ValidateMenuAsset(root);
            Assert.That(typeof(HubMenuIntentRequestV1).GetFields(BindingFlags.Instance|BindingFlags.NonPublic).All(field=>field.FieldType.Name=="HubMenuItemV1"||field.FieldType.Name=="HubEntryHandoffReceiptV1"),Is.True);
            var ownerSource=HubMenuIntentHandoffC3AuditInputV1.WithExactFrameworkImportExcluded(System.IO.File.ReadAllText("Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs"));foreach(var forbidden in new[]{"SceneManager","Application.Quit","Profile","Run","Settings","Wardrobe","CIO","CUA","InputSystem","UnityEvent","delegate"})Assert.That(ownerSource,Does.Not.Contain(forbidden),forbidden);
            var presenterSource=System.IO.File.ReadAllText("Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs");var transfer=presenterSource.Substring(presenterSource.IndexOf("TryTakeRetainedIntent",StringComparison.Ordinal),presenterSource.IndexOf("private void Awake",StringComparison.Ordinal)-presenterSource.IndexOf("TryTakeRetainedIntent",StringComparison.Ordinal));foreach(var forbidden in new[]{"SceneManager","Application.Quit","new InputRouter","new HubMenuPresentationControllerV1","UnityEvent","delegate"})Assert.That(transfer,Does.Not.Contain(forbidden),forbidden);
        }

        [Test] public void AC_M5D7QB_004_ReflectionCorruptionFailsClosed()
        {
            var request=NewRequest("Settings",ValidReceipt());foreach(var field in typeof(HubMenuIntentRequestV1).GetFields(BindingFlags.Instance|BindingFlags.NonPublic)){object boxed=request;field.SetValue(boxed,field.FieldType.IsEnum?Enum.ToObject(field.FieldType,999):Activator.CreateInstance(field.FieldType));var corrupt=(HubMenuIntentRequestV1)boxed;Assert.Throws<InvalidOperationException>(()=>Invoke(corrupt,"Validate"),field.Name);}
            var fixture=Fixture.Create("Continue");try{Set(fixture.Presenter,"_retainedIntentProof",null);HubMenuIntentRequestV1 ignored;Assert.Throws<InvalidOperationException>(()=>fixture.Presenter.TryTakeRetainedIntent(fixture.Owner,out ignored));Assert.That(fixture.Presenter.State,Is.EqualTo(HubPresenterStateV1.Failed));}finally{fixture.Dispose();}
        }

        internal static Type InputType(string name){return Type.GetType("AcadeGameMaker.Input.Unity."+name+", AcadeGameMaker.Input.Unity",true);}
        internal static object ValidReceipt(){var type=InputType("HubEntryHandoffReceiptV1");object boxed=Activator.CreateInstance(type);var mode=type.GetField("_mode",BindingFlags.Instance|BindingFlags.NonPublic);mode.SetValue(boxed,Enum.Parse(mode.FieldType,"UIOnly"));type.GetField("_ui",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(boxed,true);type.GetField("_accepted",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(boxed,true);return boxed;}
        internal static object MenuItem(string name){return Enum.Parse(InputType("HubMenuItemV1"),name);}
        internal static HubMenuIntentRequestV1 NewRequest(string item,object receipt){var ctor=typeof(HubMenuIntentRequestV1).GetConstructors(BindingFlags.Instance|BindingFlags.NonPublic).Single();return (HubMenuIntentRequestV1)ctor.Invoke(new[]{MenuItem(item),receipt});}
        internal static object Read(object target,string property){return target.GetType().GetProperty(property,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(target,null);}
        internal static object ReadField(object target,string field){return target.GetType().GetField(field,BindingFlags.Instance|BindingFlags.NonPublic).GetValue(target);}
        internal static void Invoke(object target,string method){try{target.GetType().GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(target,null);}catch(TargetInvocationException error){if(error.InnerException!=null)throw error.InnerException;throw;}}
        internal static void Set(object target,string name,object value){target.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(target,value);}

        internal sealed class Fixture:IDisposable
        { internal readonly GameObject Root;internal readonly HubMenuPresenterV1 Presenter;internal readonly HubMenuIntentHandoffOwnerV1 Owner;internal readonly object Receipt;Fixture(GameObject root,HubMenuPresenterV1 presenter,HubMenuIntentHandoffOwnerV1 owner,object receipt){Root=root;Presenter=presenter;Owner=owner;Receipt=receipt;}internal static Fixture Create(string item,bool retained=true){var root=new GameObject("M5D7QB fixture");root.SetActive(false);var presenter=root.AddComponent<HubMenuPresenterV1>();var owner=root.AddComponent<HubMenuIntentHandoffOwnerV1>();presenter.ConfigureHandoffOwnerForAuthoring(owner);owner.ConfigurePresenterForAuthoring(presenter);var receipt=ValidReceipt();if(retained){var intent=Activator.CreateInstance(InputType("HubMenuIntentV1"),BindingFlags.Instance|BindingFlags.NonPublic,null,new[]{MenuItem(item)},null);Set(presenter,"_state",HubPresenterStateV1.IntentRetained);Set(presenter,"_retainedIntent",intent);Set(presenter,"_retainedIntentProof",intent);Set(presenter,"_handoff",receipt);Set(presenter,"_handoffProof",receipt);}else Set(presenter,"_state",HubPresenterStateV1.Ready);return new Fixture(root,presenter,owner,receipt);}public void Dispose(){UnityEngine.Object.DestroyImmediate(Root);}}
    }
}
