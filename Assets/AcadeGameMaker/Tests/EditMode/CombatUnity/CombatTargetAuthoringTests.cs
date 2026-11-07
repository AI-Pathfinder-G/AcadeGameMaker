// AC-COM-003, AC-WT-003, AC-WT-005, AC-WT-006. M2B1 authoring is explicit, frozen, and independent of Unity discovery order.
using System;
using System.Collections.Generic;
using System.Linq;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    internal sealed class CombatTargetAuthoringTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private const string TemporaryScenePath = "Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/__TempCombatTargetAuthoringScene.unity";

        [SetUp]
        public void SetUp()
        {
            AssetDatabase.DeleteAsset(TemporaryScenePath);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Assert.That(EditorSceneManager.SaveScene(scene, TemporaryScenePath), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            for (var index = _objects.Count - 1; index >= 0; index--) UnityEngine.Object.DestroyImmediate(_objects[index]);
            _objects.Clear();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AssetDatabase.DeleteAsset(TemporaryScenePath);
            AssetDatabase.Refresh();
        }

        // AC-COM-003. The exact 1..96 ASCII grammar is enforced at the component boundary, including each rejected non-ASCII/control character.
        [Test]
        public void TargetIdUsesExactAsciiGrammarAndPinnedRegistrationBounds()
        {
            var probe = Create("probe");
            var probeCollider = probe.GetComponent<Collider2D>();
            for (var value = 0; value <= 127; value++)
            {
                var id = "a" + (char)value;
                probe.ConfigureForTests(id, 9, 0, true, 0, 0, 0, 0, 100, 100, probeCollider, probe.transform);
                var allowed = (value >= 'A' && value <= 'Z') || (value >= 'a' && value <= 'z') || (value >= '0' && value <= '9') || "._:/-".IndexOf((char)value) >= 0;
                if (allowed) Assert.DoesNotThrow(() => probe.ValidateForAuthoring(), "ASCII " + value);
                else Assert.Throws<ArgumentException>(() => probe.ValidateForAuthoring(), "ASCII " + value);
            }

            Assert.DoesNotThrow(() => Create("player", 5, 45, false).ValidateForAuthoring());
            Assert.DoesNotThrow(() => Create("walker", 9, 0, true).ValidateForAuthoring());
            Assert.DoesNotThrow(() => Create("surveyor", 6, 0, true).ValidateForAuthoring());
            Assert.DoesNotThrow(() => Create("ordan", 60, 0, true).ValidateForAuthoring());
            Assert.Throws<ArgumentOutOfRangeException>(() => Create("bad-health", 0, 0, true).ValidateForAuthoring());
            Assert.Throws<ArgumentOutOfRangeException>(() => Create("bad-invulnerability", 1, 601, true).ValidateForAuthoring());
        }

        // AC-COM-003. This assembly exposes the contractually sole public Unity gameplay type.
        [Test]
        public void CombatUnityAssemblyExportsOnlyCombatTarget()
        {
            var exported = typeof(CombatTarget).Assembly.GetExportedTypes().Select(type => type.FullName).OrderBy(value => value, StringComparer.Ordinal).ToArray();
            CollectionAssert.AreEqual(new[] { typeof(CombatTarget).FullName }, exported);
        }

        // AC-WT-005. Co-authoring compares every shared scalar/reference, while transfer kind/profile/sink are deliberately irrelevant.
        [Test]
        public void CoauthoredTransferTargetMustMatchAllIdentityAndGeometryAndOrdanIsNeverTransferable()
        {
            var joint = CreateCoauthored("walker");
            Assert.DoesNotThrow(() => joint.combat.ValidateForAuthoring());
            joint.combat.ConfigureForTests("walker", 9, 0, true, 11, 20, 30, 40, 50, 60, joint.collider, joint.go.transform, joint.transfer);
            Assert.Throws<InvalidOperationException>(() => joint.combat.ValidateForAuthoring());

            var ordan = CreateCoauthored("ordan");
            Assert.Throws<InvalidOperationException>(() => ordan.combat.ValidateForAuthoring());
        }

        // AC-COM-003. Combat has no position-Z contract; only local Z rotation and XY scale are frozen.
        [Test]
        public void PoseRootAllowsArbitraryPositionZButRejectsRotationOrNonUnitXYScale()
        {
            var target = Create("z-pose");
            target.transform.position = new Vector3(0f, 0f, 37f);
            Assert.DoesNotThrow(() => target.ValidateForAuthoring());
            target.transform.localEulerAngles = new Vector3(0f, 0f, 1f);
            Assert.Throws<InvalidOperationException>(() => target.ValidateForAuthoring());
            target.transform.localEulerAngles = Vector3.zero;
            target.transform.localScale = new Vector3(2f, 1f, 1f);
            Assert.Throws<InvalidOperationException>(() => target.ValidateForAuthoring());
        }

        // AC-COM-003. The serialized registry rejects input errors before replacing its immutable prior frozen copy.
        [Test]
        public void RegistryRejectsUnsortedNullDuplicateIdAndColliderAndNeverPublishesPartialFrozenRoster()
        {
            var registry = Track(new GameObject("Registry")).AddComponent<CombatTargetRegistry>();
            var alpha = Create("alpha");
            var bravo = Create("bravo");
            registry.ConfigureForTests(alpha, bravo);
            registry.ValidateAndFreeze();
            var frozen = registry.FrozenTargets;
            Assert.That(frozen, Is.Not.TypeOf<FrozenCombatTarget[]>());
            Assert.That(frozen.Count, Is.EqualTo(2));
            Assert.That(frozen[0].MaxHealth, Is.EqualTo(9));

            alpha.ConfigureForTests("alpha", 999, 0, true, 0, 0, 0, 0, 100, 100, alpha.GetComponent<Collider2D>(), alpha.transform);
            Assert.That(frozen[0].MaxHealth, Is.EqualTo(9));
            registry.ConfigureForTests(bravo, alpha);
            Assert.Throws<InvalidOperationException>(() => registry.ValidateAndFreeze());
            Assert.That(registry.FrozenTargets, Is.SameAs(frozen));

            registry.ConfigureForTests(alpha, null);
            Assert.Throws<InvalidOperationException>(() => registry.ValidateAndFreeze());
            registry.ConfigureForTests(alpha, alpha);
            Assert.Throws<InvalidOperationException>(() => registry.ValidateAndFreeze());
            var duplicateCollider = Create("charlie");
            duplicateCollider.ConfigureForTests("charlie", 9, 0, true, 0, 0, 0, 0, 100, 100, alpha.GetComponent<Collider2D>(), duplicateCollider.transform);
            registry.ConfigureForTests(alpha, duplicateCollider);
            Assert.Throws<InvalidOperationException>(() => registry.ValidateAndFreeze());
        }

        private CombatTarget Create(string id, int health = 9, int invulnerability = 0, bool attackable = true)
        {
            var go = Track(new GameObject(id));
            var collider = go.AddComponent<BoxCollider2D>();
            var combat = go.AddComponent<CombatTarget>();
            combat.ConfigureForTests(id, health, invulnerability, attackable, 0, 0, 0, 0, 100, 100, collider, go.transform);
            return combat;
        }

        private (GameObject go, Collider2D collider, CombatTarget combat, TransferTarget transfer) CreateCoauthored(string id)
        {
            var go = Track(new GameObject(id));
            var collider = go.AddComponent<BoxCollider2D>();
            var sink = go.AddComponent<Sink>(); sink.Id = id;
            var transfer = go.AddComponent<TransferTarget>();
            transfer.ConfigureForTests(id, TransferTargetKind.Enemy, 10, 20, 30, 40, 50, 60, "profile", collider, go.transform, sink);
            var combat = go.AddComponent<CombatTarget>();
            combat.ConfigureForTests(id, 9, 0, true, 10, 20, 30, 40, 50, 60, collider, go.transform, transfer);
            return (go, collider, combat, transfer);
        }

        private GameObject Track(GameObject value) { _objects.Add(value); return value; }
        private sealed class Sink : MonoBehaviour, ITransferTargetModifierSink
        {
            internal string Id;
            public string BoundTargetId => Id;
            public bool IsStillAvailable() => false;
            public bool TryApply(TransferModifierRequest request) => false;
            public void Clear(TransferClearReason reason, SimulationTick tick, int revision) { }
        }
    }
}
