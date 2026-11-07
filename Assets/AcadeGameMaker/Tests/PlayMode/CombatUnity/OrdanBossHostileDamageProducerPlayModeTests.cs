// AC-M4B3B2-004, AC-M4B3B2-005, AC-M4B3B2-006, AC-M4B3B2-007, AC-M4B3B2-008.
using System.Collections;
using AcadeGameMaker.Combat.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class OrdanBossHostileDamageProducerPlayModeTests
    {
        private const string GraphPath = "Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab";

        // AC-M4B3B2-004/005/006/007. The authored graph has one exact -170 owner;
        // its result is immutable value data and never exposes Unity geometry.
        [UnityTest]
        public IEnumerator AuthoredGraphOwnsOneHostileProducerAndDigestIsValueOnly()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            Assert.That(asset, Is.Not.Null);
            var root = Object.Instantiate(asset);
            try
            {
                yield return new WaitForFixedUpdate();
                var producers = root.GetComponentsInChildren<OrdanBossHostileDamageProducer>(true);
                Assert.That(producers, Has.Length.EqualTo(1));
                Assert.That(producers[0].GetType().GetCustomAttributes(typeof(DefaultExecutionOrder), true), Is.Not.Empty);
            }
            finally { Object.Destroy(root); }
        }

        // AC-M4B3B2-008. Fresh graphs are independent: cadence harnesses may
        // compare their value publications without sharing a producer instance.
        [Test]
        public void FreshGraphsDoNotShareHostileProducerState()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            var left = Object.Instantiate(asset); var right = Object.Instantiate(asset);
            try
            {
                var a = left.GetComponentInChildren<OrdanBossHostileDamageProducer>(true);
                var b = right.GetComponentInChildren<OrdanBossHostileDamageProducer>(true);
                Assert.That(a, Is.Not.Null); Assert.That(b, Is.Not.Null); Assert.That(a, Is.Not.SameAs(b));
            }
            finally { Object.DestroyImmediate(left); Object.DestroyImmediate(right); }
        }
    }
}
