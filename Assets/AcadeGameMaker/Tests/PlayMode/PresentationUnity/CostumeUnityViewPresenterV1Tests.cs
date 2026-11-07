using System.Reflection;
using AcadeGameMaker.Presentation.Unity;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.PlayMode.PresentationUnity
{
    // AC-CUA-005/007: static guard for explicit-port-only presentation ownership.
    public sealed class CostumeUnityViewPresenterV1Tests
    {
        [Test]
        public void AC_CUA_007_AdapterHasOnlyInjectedPortsAndNoStaticOwner()
        {
            var type = typeof(CostumeUnityPresentationAdapterV1);
            Assert.That(type.GetField("_projection", BindingFlags.Instance | BindingFlags.NonPublic), Is.Not.Null);
            Assert.That(type.GetProperty("CompletedSnapshots"), Is.Not.Null);
            Assert.That(type.GetProperty("CompletedSnapshots").PropertyType, Is.EqualTo(typeof(ICostumeUnityCompletedSnapshotPortV1)));
            var select = type.GetMethod("TrySelect");
            Assert.That(select, Is.Not.Null);
            Assert.That(select.GetParameters().Length, Is.EqualTo(1));
            Assert.That(select.GetParameters()[0].ParameterType, Is.EqualTo(typeof(CostumeUnityMediaPackageV1)));
            Assert.That(type.GetMethod("ObserveCompletedSnapshot"), Is.Null);
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) Assert.That(field.IsLiteral, Is.True, field.Name);
            Assert.That(typeof(CostumeUnityViewPresenterV1).GetConstructors().Length, Is.EqualTo(1));
        }
    }
}
