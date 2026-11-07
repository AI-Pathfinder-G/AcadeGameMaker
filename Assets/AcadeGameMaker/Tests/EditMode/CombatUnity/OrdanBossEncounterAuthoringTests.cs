// AC-COM-002, AC-COM-003, AC-WT-003, AC-WT-005.
using System;
using System.IO;
using System.Reflection;
using AcadeGameMaker.Camera.Unity;
using AcadeGameMaker.Combat.Authoring;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    internal sealed class OrdanBossEncounterAuthoringTests
    {
        public enum StructuralMutation
        {
            Missing, Duplicate, Reordered, Renamed, CrossWired, Foreign,
            WrongLayer, WrongShape, WrongProfile, WrongOwner, WrongComponentOrder, LegacySerializedMode,
            MissingInputRouter, RouterCameraCrossWire, WrongCameraProfile
        }

        [Test]
        public void CommittedBossPrefabAndSceneValidateWithoutInitializingOwners()
        {
            var prefab = PrefabUtility.LoadPrefabContents(OrdanBossEncounterAuthoringBuilder.GraphPrefabPath);
            try { OrdanBossEncounterAuthoringValidator.Validate(prefab, false); }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
            var scene = EditorSceneManager.OpenScene(OrdanBossEncounterAuthoringBuilder.ScenePath, OpenSceneMode.Additive);
            try { var roots = scene.GetRootGameObjects(); Assert.That(roots.Length, Is.EqualTo(1)); OrdanBossEncounterAuthoringValidator.Validate(roots[0], true); }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }

        // AC-COM-003. The production choice is an explicit serialized typed
        // latch, exposed only through a getter so validation never infers mode
        // from nullable bindings or registry contents.
        [Test]
        public void SchedulerProductionModeReadSeamIsExactAndImmutable()
        {
            var prefab = PrefabUtility.LoadPrefabContents(OrdanBossEncounterAuthoringBuilder.GraphPrefabPath);
            try
            {
                var scheduler = prefab.GetComponentInChildren<OrdanBossTransferExposureScheduler>();
                Assert.That(scheduler.ConfigurationModeForAuthoring, Is.EqualTo(OrdanBossTransferExposureConfigurationMode.ProductionAuditAndPayload));
                var seam = typeof(OrdanBossTransferExposureScheduler).GetProperty("ConfigurationModeForAuthoring", BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.That(seam, Is.Not.Null); Assert.That(seam.CanWrite, Is.False);
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
        }

        [Test]
        public void ValidatorRejectsCrossWiredBalanceAuditPullWithoutRepair()
        {
            var prefab = PrefabUtility.LoadPrefabContents(OrdanBossEncounterAuthoringBuilder.GraphPrefabPath);
            try
            {
                var pull = prefab.GetComponentInChildren<OrdanBossBalanceAuditPullProducer>();
                SetPrivate(pull, "_bridge", null);
                Assert.Throws<InvalidOperationException>(() => OrdanBossEncounterAuthoringValidator.Validate(prefab, false));
                Assert.That(GetPrivate(pull, "_bridge"), Is.Null);
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
        }

        [Test]
        public void AuditSinkRejectsForgedRequestsAndKeepsHiddenStateRevisionSafe()
        {
            var value = new GameObject("BossAuditBox-0");
            try
            {
                var sink = value.AddComponent<OrdanBossAuditTransferModifierSink>(); sink.ConfigureForTests("BossAuditBox-0");
                var exact = new TransferModifierRequest("BossAuditBox-0", TransferTargetKind.Box, OrdanBossAuditTransferModifierSink.RequiredBaseProfileId,
                    OrdanBossAuditTransferModifierSink.RequiredModifierId, new SimulationTick(0), 1);
                Assert.That(sink.TryApply(exact), Is.False); Assert.That(sink.IsStillAvailable(), Is.False);
                Assert.That(sink.TryApply(new TransferModifierRequest("BossAuditBox-0", TransferTargetKind.Box, "foreign", OrdanBossAuditTransferModifierSink.RequiredModifierId, new SimulationTick(0), 2)), Is.False);
                sink.Clear(TransferClearReason.TargetRemoved, new SimulationTick(1), 1);
                Assert.That(sink.LastAcceptedRevision, Is.EqualTo(1)); Assert.That(sink.IsApplied, Is.False);
                Assert.Throws<ArgumentOutOfRangeException>(() => sink.Clear(TransferClearReason.TargetRemoved, new SimulationTick(2), 1));
            }
            finally { UnityEngine.Object.DestroyImmediate(value); }
        }

        // AC-COM-002, AC-COM-003, AC-COM-004. Each mutation is independent:
        // validation is fail-closed and never repairs authored graph state.
        [TestCase(StructuralMutation.Missing)]
        [TestCase(StructuralMutation.Duplicate)]
        [TestCase(StructuralMutation.Reordered)]
        [TestCase(StructuralMutation.Renamed)]
        [TestCase(StructuralMutation.CrossWired)]
        [TestCase(StructuralMutation.Foreign)]
        [TestCase(StructuralMutation.WrongLayer)]
        [TestCase(StructuralMutation.WrongShape)]
        [TestCase(StructuralMutation.WrongProfile)]
        [TestCase(StructuralMutation.WrongOwner)]
        [TestCase(StructuralMutation.WrongComponentOrder)]
        [TestCase(StructuralMutation.LegacySerializedMode)]
        [TestCase(StructuralMutation.MissingInputRouter)]
        [TestCase(StructuralMutation.RouterCameraCrossWire)]
        [TestCase(StructuralMutation.WrongCameraProfile)]
        public void ValidatorRejectsEveryIndependentStructuralMutationWithoutRepair(StructuralMutation mutation)
        {
            var prefab = PrefabUtility.LoadPrefabContents(OrdanBossEncounterAuthoringBuilder.GraphPrefabPath);
            try
            {
                ApplyMutation(prefab, mutation);
                Assert.Throws<InvalidOperationException>(() => OrdanBossEncounterAuthoringValidator.Validate(prefab, false));
                AssertMutationRemains(prefab, mutation);
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
        }

        // AC-COM-003, AC-WT-005. The audit sink is dormant until its future
        // owner exposes it, rejects forged identity/revision, and accepts a
        // Transfer-owned clear revision even while hidden.
        [Test]
        public void AuditSinkMatrixPreservesHiddenAndAppliedRevisionSemantics()
        {
            var value = new GameObject("BossAuditBox-0");
            try
            {
                var sink = value.AddComponent<OrdanBossAuditTransferModifierSink>();
                sink.ConfigureForTests(OrdanBossAuditTransferModifierSink.RequiredTargetId);
                var exact = new TransferModifierRequest("BossAuditBox-0", TransferTargetKind.Box,
                    OrdanBossAuditTransferModifierSink.RequiredBaseProfileId, OrdanBossAuditTransferModifierSink.RequiredModifierId, new SimulationTick(0), 1);
                Assert.That(sink.TryApply(exact), Is.False, "hidden audit must not become a target");
                sink.CommitExposureForM4B3B(true);
                Assert.That(sink.TryApply(exact), Is.True);
                Assert.That(sink.TryApply(exact), Is.False, "duplicate revision is rejected");
                Assert.That(sink.TryApply(new TransferModifierRequest("BossAuditBox-0", TransferTargetKind.Box, "foreign",
                    OrdanBossAuditTransferModifierSink.RequiredModifierId, new SimulationTick(1), 2)), Is.False);
                Assert.That(sink.TryApply(new TransferModifierRequest("foreign", TransferTargetKind.Box,
                    OrdanBossAuditTransferModifierSink.RequiredBaseProfileId, OrdanBossAuditTransferModifierSink.RequiredModifierId, new SimulationTick(1), 2)), Is.False);
                Assert.That(sink.TryApply(new TransferModifierRequest("BossAuditBox-0", TransferTargetKind.Enemy,
                    OrdanBossAuditTransferModifierSink.RequiredBaseProfileId, "Target.Enemy.Heavy.v1", new SimulationTick(1), 2)), Is.False);
                sink.Clear(TransferClearReason.TargetRemoved, new SimulationTick(1), 2);
                Assert.That(sink.IsApplied, Is.False); Assert.That(sink.IsStillAvailable(), Is.True);
                sink.CommitExposureForM4B3B(false);
                sink.Clear(TransferClearReason.TargetRemoved, new SimulationTick(2), 3);
                Assert.That(sink.LastAcceptedRevision, Is.EqualTo(3)); Assert.That(sink.IsStillAvailable(), Is.False);
            }
            finally { UnityEngine.Object.DestroyImmediate(value); }
        }

        // AC-COM-002, AC-COM-003. The approved nested player placement is the
        // only legal prefab override; a foreign Rigidbody value is fail-closed.
        [Test]
        public void ValidatorAcceptsExactPlayerPlacementOverridesAndRejectsForeignOnes()
        {
            var scene = EditorSceneManager.OpenScene(OrdanBossEncounterAuthoringBuilder.ScenePath, OpenSceneMode.Additive);
            try
            {
                var root = scene.GetRootGameObjects()[0];
                Assert.DoesNotThrow(() => OrdanBossEncounterAuthoringValidator.Validate(root, true));
                var body = root.transform.GetChild(0).GetComponent<Rigidbody2D>();
                body.gravityScale = 9f;
                PrefabUtility.RecordPrefabInstancePropertyModifications(body);
                Assert.Throws<InvalidOperationException>(() => OrdanBossEncounterAuthoringValidator.Validate(root, true));
            }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }

        private static void ApplyMutation(GameObject root, StructuralMutation mutation)
        {
            var payload = Find(root.transform, "BossWeight-0");
            switch (mutation)
            {
                case StructuralMutation.Missing: UnityEngine.Object.DestroyImmediate(payload.gameObject); break;
                case StructuralMutation.Duplicate: UnityEngine.Object.Instantiate(payload.gameObject, root.transform); break;
                case StructuralMutation.Reordered: payload.SetSiblingIndex(0); break;
                case StructuralMutation.Renamed: payload.name = "WrongWeight"; break;
                case StructuralMutation.CrossWired:
                    SetPrivate(root.GetComponentInChildren<OrdanBossTransferExposureScheduler>(), "_bridge", null); break;
                case StructuralMutation.Foreign: root.AddComponent<Rigidbody2D>(); break;
                case StructuralMutation.WrongLayer: payload.gameObject.layer = TransferLineOfSight.Layer; break;
                case StructuralMutation.WrongShape: payload.GetComponent<BoxCollider2D>().size = new Vector2(99f, 99f); break;
                case StructuralMutation.WrongProfile: SetPrivate(payload.GetComponent<TransferTarget>(), "_baseModifierProfileId", "foreign"); break;
                case StructuralMutation.WrongOwner: SetPrivate(payload.GetComponent<TransferTarget>(), "_targetOwnerSink", root.GetComponentInChildren<OrdanBossAuditTransferModifierSink>()); break;
                case StructuralMutation.WrongComponentOrder: ComponentUtility.MoveComponentUp(payload.GetComponent<TransferTarget>()); break;
                case StructuralMutation.LegacySerializedMode:
                    SetPrivate(root.GetComponentInChildren<OrdanBossTransferExposureScheduler>(), "_configurationMode", OrdanBossTransferExposureConfigurationMode.LegacyPayloadOnly); break;
                case StructuralMutation.MissingInputRouter:
                    UnityEngine.Object.DestroyImmediate(root.GetComponentInChildren<InputRouter>()); break;
                case StructuralMutation.RouterCameraCrossWire:
                    SetPrivate(root.GetComponentInChildren<InputRouter>(), "_cameraProviderBehaviour", root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>()); break;
                case StructuralMutation.WrongCameraProfile:
                    root.GetComponentInChildren<UnityEngine.Camera>().orthographicSize = 9f; break;
                default: throw new ArgumentOutOfRangeException(nameof(mutation));
            }
        }

        // AC-COM-003, AC-COM-004, AC-WT-005. Every M4B3A authoring seam is
        // assignment-only while the graph is inactive, and the new source has
        // no discovery, render-time, physics-query, input, or scene authority.
        [Test]
        public void AuthoringSeamsStayPristineAndSourcesContainNoForbiddenAuthority()
        {
            var root = PrefabUtility.LoadPrefabContents(OrdanBossEncounterAuthoringBuilder.GraphPrefabPath);
            try
            {
                root.SetActive(false);
                var player = root.transform.GetChild(0).GetComponent<PlayerMovementController>();
                var ordan = root.transform.GetChild(1).GetComponent<CombatTarget>();
                var auditTarget = root.transform.GetChild(2).GetComponent<TransferTarget>();
                var auditSink = root.transform.GetChild(2).GetComponent<OrdanBossAuditTransferModifierSink>();
                var payloadTargets = new TransferTarget[3];
                var payloadSinks = new OrdanBossPayloadTransferModifierSink[3];
                var payloadColliders = new Collider2D[3];
                for (var index = 0; index < 3; index++)
                {
                    var payload = root.transform.GetChild(3 + index);
                    payloadTargets[index] = payload.GetComponent<TransferTarget>();
                    payloadSinks[index] = payload.GetComponent<OrdanBossPayloadTransferModifierSink>();
                    payloadColliders[index] = payload.GetComponent<BoxCollider2D>();
                    payloadSinks[index].ConfigureForAuthoring("BossWeight-" + index);
                }
                auditSink.ConfigureForAuthoring(OrdanBossAuditTransferModifierSink.RequiredTargetId,
                    OrdanBossAuditTransferModifierSink.RequiredBaseProfileId);
                var systems = root.transform.GetChild(6);
                var transfer = systems.GetComponent<TransferSimulationDriver>();
                var combat = systems.GetComponent<OrdanBossCombatSimulationDriver>();
                var bridge = systems.GetComponent<OrdanBossSimulationDriver>();
                var scheduler = systems.GetComponent<OrdanBossTransferExposureScheduler>();
                var handoff = systems.GetComponent<OrdanBossEncounterHandoffAdapter>();
                combat.ConfigureForAuthoring(player, combat.BoundRegistry, transfer, combat);
                bridge.ConfigureForAuthoring(player, transfer, combat);
                scheduler.ConfigureForAuthoring(bridge, transfer, payloadTargets, payloadSinks);
                handoff.ConfigureForAuthoring(player, transfer, combat, bridge, scheduler, ordan,
                    auditTarget, auditSink, payloadTargets, payloadSinks, payloadColliders);

                Assert.That((bool)GetPrivate(combat, "_prepared"), Is.False);
                Assert.That((bool)GetPrivate(combat, "_attempted"), Is.False);
                Assert.That((bool)GetPrivate(bridge, "_prepared"), Is.False);
                Assert.That((bool)GetPrivate(bridge, "_initialized"), Is.False);
                Assert.That((bool)GetPrivate(scheduler, "_initialized"), Is.False);
                Assert.That(scheduler.HasLatestOutcome, Is.False);
                Assert.That(handoff.LatestPresentationView.HasValue, Is.False);
                Assert.That(auditSink.IsStillAvailable(), Is.False);
                Assert.That(auditSink.LastAcceptedRevision, Is.Zero);
                foreach (var sink in payloadSinks)
                    Assert.That(!sink.IsStillAvailable() && !sink.IsApplied && sink.LastAcceptedRevision == 0, Is.True);

                AssertSourceExcludes("Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringBuilder.cs",
                    "ConfigureForTests", "FindObject", "Resources.Load", "[Test", "public static class", "public class");
                var authoritySources = new[]
                {
                    "Assets/AcadeGameMaker/Editor/CombatAuthoring/OrdanBossEncounterAuthoringValidator.cs",
                    "Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossEncounterHandoffAdapter.cs",
                    "Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossAuditTransferModifierSink.cs",
                    "Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossCombatSimulationDriver.cs",
                    "Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossSimulationDriver.cs",
                    "Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTransferExposureScheduler.cs",
                    "Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossBalanceAuditPullProducer.cs",
                    "Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossTerminalTeardown.cs",
                    "Assets/AcadeGameMaker/Runtime/Combat/Unity/OrdanBossPayloadTransferModifierSink.cs"
                };
                foreach (var path in authoritySources)
                    AssertSourceExcludes(path, "FindObject", "FindAnyObject", "FindFirstObject", "Resources.Load",
                        "UnityEngine.Random", "Random.", "Time.deltaTime", "StartCoroutine", "IEnumerator", "OnCollision", "OnTrigger",
                        "Physics2D.", "Raycast", "Overlap", "SceneManager.LoadScene", "LoadScene(", "UnityEngine.Input.", "Input.Get", "GetKey", "GetMouse",
                        "public sealed class", "public class", "public readonly struct", "public struct", "public interface", "public enum",
                        "public void ConfigureForAuthoring");
                foreach (var path in new[] { authoritySources[0], authoritySources[1], authoritySources[2] })
                    AssertSourceExcludes(path, "AddComponent<", "Destroy(", "SetParent(", "SetActive(", "SaveScene(", "SaveAssets(");
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        private static void AssertMutationRemains(GameObject root, StructuralMutation mutation)
        {
            if (mutation == StructuralMutation.Missing) { Assert.That(root.transform.childCount, Is.EqualTo(8)); return; }
            if (mutation == StructuralMutation.Duplicate) { Assert.That(root.transform.childCount, Is.EqualTo(10)); return; }
            if (mutation == StructuralMutation.Renamed) { Assert.That(Find(root.transform, "WrongWeight"), Is.Not.Null); return; }
            if (mutation == StructuralMutation.Reordered) { Assert.That(root.transform.GetChild(0).name, Is.EqualTo("BossWeight-0")); return; }
            if (mutation == StructuralMutation.Foreign) { Assert.That(root.GetComponent<Rigidbody2D>(), Is.Not.Null); return; }
            if (mutation == StructuralMutation.WrongLayer) { Assert.That(Find(root.transform, "BossWeight-0").gameObject.layer, Is.EqualTo(TransferLineOfSight.Layer)); return; }
            if (mutation == StructuralMutation.WrongShape) { Assert.That(Find(root.transform, "BossWeight-0").GetComponent<BoxCollider2D>().size, Is.EqualTo(new Vector2(99f, 99f))); return; }
            if (mutation == StructuralMutation.WrongProfile) { Assert.That((string)GetPrivate(Find(root.transform, "BossWeight-0").GetComponent<TransferTarget>(), "_baseModifierProfileId"), Is.EqualTo("foreign")); return; }
            if (mutation == StructuralMutation.WrongOwner) { Assert.That(GetPrivate(Find(root.transform, "BossWeight-0").GetComponent<TransferTarget>(), "_targetOwnerSink"), Is.EqualTo(root.GetComponentInChildren<OrdanBossAuditTransferModifierSink>())); return; }
            if (mutation == StructuralMutation.CrossWired) Assert.That(GetPrivate(root.GetComponentInChildren<OrdanBossTransferExposureScheduler>(), "_bridge"), Is.Null);
            if (mutation == StructuralMutation.WrongComponentOrder)
            {
                var components = Find(root.transform, "BossWeight-0").GetComponents<Component>();
                Assert.That(Array.IndexOf(components, Find(root.transform, "BossWeight-0").GetComponent<TransferTarget>()),
                    Is.LessThan(Array.IndexOf(components, Find(root.transform, "BossWeight-0").GetComponent<OrdanBossPayloadTransferModifierSink>())));
            }
            if (mutation == StructuralMutation.LegacySerializedMode)
                Assert.That(root.GetComponentInChildren<OrdanBossTransferExposureScheduler>().ConfigurationModeForAuthoring,
                    Is.EqualTo(OrdanBossTransferExposureConfigurationMode.LegacyPayloadOnly));
            if (mutation == StructuralMutation.MissingInputRouter) Assert.That(root.GetComponentInChildren<InputRouter>(), Is.Null);
            if (mutation == StructuralMutation.RouterCameraCrossWire) Assert.That(GetPrivate(root.GetComponentInChildren<InputRouter>(), "_cameraProviderBehaviour"), Is.EqualTo(root.GetComponentInChildren<OrdanBossEncounterHandoffAdapter>()));
            if (mutation == StructuralMutation.WrongCameraProfile) Assert.That(root.GetComponentInChildren<UnityEngine.Camera>().orthographicSize, Is.EqualTo(9f));
        }

        private static Transform Find(Transform root, string name)
        {
            foreach (var child in root.GetComponentsInChildren<Transform>(true)) if (child.name == name) return child;
            return null;
        }
        private static void SetPrivate(object target, string field, object value) => target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
        private static object GetPrivate(object target, string field) => target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
        private static void AssertSourceExcludes(string path, params string[] forbidden)
        {
            var source = File.ReadAllText(path);
            foreach (var token in forbidden) Assert.That(source, Does.Not.Contain(token), path + " contains forbidden authority token " + token);
        }
    }
}
