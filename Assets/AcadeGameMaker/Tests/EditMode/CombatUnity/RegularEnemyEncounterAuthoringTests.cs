using System;
using System.IO;
using AcadeGameMaker.Combat.Authoring;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    internal sealed class RegularEnemyEncounterAuthoringTests
    {
        // AC-COM-001, AC-COM-003. Verification loads committed B1 assets read-only
        // and passes the caller-selected stage to the root-bounded validator.
        [Test]
        public void CommittedPrefabAndSceneAreExactCollisionNeutralB1Graphs()
        {
            var contents = PrefabUtility.LoadPrefabContents(RegularEnemyEncounterAuthoringBuilder.GraphPrefabPath);
            try
            {
                var registry = contents.GetComponentInChildren<CombatTargetRegistry>(true);
                Assert.Throws<InvalidOperationException>(() => _ = registry.FrozenTargets);
                RegularEnemyEncounterAuthoringValidator.Validate(contents, RegularEnemyEncounterAuthoringStage.B1, false);
                Assert.Throws<InvalidOperationException>(() => _ = registry.FrozenTargets);
            }
            finally { PrefabUtility.UnloadPrefabContents(contents); }

            var scene = EditorSceneManager.OpenScene(RegularEnemyEncounterAuthoringBuilder.ScenePath, OpenSceneMode.Additive);
            try
            {
                var roots = scene.GetRootGameObjects();
                Assert.That(roots.Length, Is.EqualTo(1));
                RegularEnemyEncounterAuthoringValidator.Validate(roots[0], RegularEnemyEncounterAuthoringStage.B1, false);
            }
            finally { EditorSceneManager.CloseScene(scene, true); }
        }

        // AC-COM-001. Exact known MonoBehaviour enumeration proves zero B1 handoff
        // drivers and rejects a foreign graph member without asset repair.
        [Test]
        public void RootBoundedValidatorRejectsForeignBehaviourWithoutSavingAsset()
        {
            var contents = PrefabUtility.LoadPrefabContents(RegularEnemyEncounterAuthoringBuilder.GraphPrefabPath);
            try
            {
                var registry = contents.GetComponentInChildren<CombatTargetRegistry>(true);
                Assert.Throws<InvalidOperationException>(() => _ = registry.FrozenTargets);
                contents.AddComponent<ForeignBehaviour>();
                Assert.Throws<InvalidOperationException>(() =>
                    RegularEnemyEncounterAuthoringValidator.Validate(contents, RegularEnemyEncounterAuthoringStage.B1, false));
                Assert.Throws<InvalidOperationException>(() => _ = registry.FrozenTargets);
            }
            finally { PrefabUtility.UnloadPrefabContents(contents); }
        }

        // AC-COM-001, AC-COM-003. Every structural/reference mutation is rejected
        // read-only; validation never initializes CombatTargetRegistry's runtime cache.
        [TestCaseSource(nameof(ExactGraphMutations))]
        public void ExactB1ValidatorRejectsTopologyGeometryComponentAndCrossWireMutations(GraphMutation mutation)
        {
            var contents = PrefabUtility.LoadPrefabContents(RegularEnemyEncounterAuthoringBuilder.GraphPrefabPath);
            try
            {
                var registry = contents.GetComponentInChildren<CombatTargetRegistry>(true);
                AssertFrozenUninitialized(registry);
                ApplyMutation(contents, mutation);
                Assert.Throws<InvalidOperationException>(() =>
                    RegularEnemyEncounterAuthoringValidator.Validate(contents, RegularEnemyEncounterAuthoringStage.B1, false),
                    mutation.ToString());
                if (registry != null) AssertFrozenUninitialized(registry);
            }
            finally { PrefabUtility.UnloadPrefabContents(contents); }
        }

        // AC-COM-001. Production asset construction uses only production-named
        // authoring seams; this scan guards accidental test-seam regression.
        [Test]
        public void BuilderSourceContainsNoTestConfigurationSeam()
        {
            const string path = "Assets/AcadeGameMaker/Editor/CombatAuthoring/RegularEnemyEncounterAuthoringBuilder.cs";
            var source = File.ReadAllText(path);
            StringAssert.DoesNotContain("ConfigureForTests", source);
            StringAssert.Contains("ConfigureForAuthoring", source);
            StringAssert.DoesNotContain("SceneManager.CreateScene", source);
            StringAssert.DoesNotContain("public ", source);
            var initialSave = source.IndexOf("SaveScene(scene, ScenePath)", StringComparison.Ordinal);
            var buildGraph = source.IndexOf("BuildGraph(playerPrefab", StringComparison.Ordinal);
            var provisionalValidate = source.IndexOf("Validate(graph, stage, false)", StringComparison.Ordinal);
            Assert.That(initialSave, Is.GreaterThanOrEqualTo(0));
            Assert.That(buildGraph, Is.GreaterThan(initialSave));
            Assert.That(provisionalValidate, Is.GreaterThan(buildGraph));
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-003. Unity exposes no
        // controllable post-call failure for Physics2D.IgnoreCollision. A
        // runtime fault hook would alter production semantics, so this source
        // assertion instead locks the required commit -> verified-pair ->
        // fail-stop ordering and rejects a test-only adapter hook.
        [Test]
        public void HandoffSourceKeepsPostCommitPairFailureFailStoppedWithoutATestHook()
        {
            const string path = "Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyEncounterHandoffAdapter.cs";
            var source = File.ReadAllText(path);
            StringAssert.DoesNotContain("ForTests", source);
            var commit = source.IndexOf("staged.CommitNext(input, preview);", StringComparison.Ordinal);
            var apply = source.IndexOf("ApplyPair(_surveyorCollider", StringComparison.Ordinal);
            var verify = source.IndexOf("Physics2D.GetIgnoreCollision(_playerCollider, enemy) != desired", StringComparison.Ordinal);
            var catchBlock = source.IndexOf("catch", commit, StringComparison.Ordinal);
            var failStop = source.IndexOf("_fatalCollisionCommitFailure = true;", commit, StringComparison.Ordinal);
            Assert.That(commit, Is.GreaterThanOrEqualTo(0));
            Assert.That(apply, Is.GreaterThan(commit));
            Assert.That(verify, Is.GreaterThan(apply));
            Assert.That(catchBlock, Is.GreaterThan(commit));
            Assert.That(failStop, Is.GreaterThan(catchBlock));
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-003. The +110 source
        // comparison must retain every immutable Transfer result field, not
        // merely the final Transfer session snapshot or input summary.
        [Test]
        public void HandoffTransferRevalidationSourceComparesEveryNestedResultField()
        {
            const string path = "Assets/AcadeGameMaker/Runtime/Combat/Unity/RegularEnemyEncounterHandoffAdapter.cs";
            var source = File.ReadAllText(path);
            StringAssert.Contains("SameTransferAttempt(left.Outcome.RemovalResult, right.Outcome.RemovalResult)", source);
            StringAssert.Contains("SameTransferAttempt(left.Outcome.PressResult, right.Outcome.PressResult)", source);
            StringAssert.Contains("a.Tick == b.Tick && a.AimSampleId == b.AimSampleId && a.SelectedTargetId == b.SelectedTargetId", source);
            StringAssert.Contains("a.Success == b.Success && a.FailureReason == b.FailureReason && SameTransferSession(a.Session, b.Session)", source);
            StringAssert.Contains("SameTransferStateChanged(a.StateChanged, b.StateChanged) && SameTransferCleared(a.Cleared, b.Cleared)", source);
            StringAssert.Contains("left.HighlightedTargetId == right.HighlightedTargetId && left.ActiveTargetId == right.ActiveTargetId", source);
            StringAssert.Contains("left.PlayerState == right.PlayerState && left.TransitionLockEndTick == right.TransitionLockEndTick", source);
            StringAssert.Contains("left.LastFailureReason == right.LastFailureReason && left.Revision == right.Revision", source);
            StringAssert.Contains("a.TargetId == b.TargetId && a.PlayerModifierId == b.PlayerModifierId && a.TargetModifierId == b.TargetModifierId", source);
            StringAssert.Contains("a.Reason == b.Reason && a.PreviousTargetId == b.PreviousTargetId && a.Tick == b.Tick && a.Revision == b.Revision", source);
        }

        // AC-COM-001, AC-COM-003. Phase order is the runtime type declaration;
        // Player intentionally has no attribute and therefore occupies order 0.
        [Test]
        public void RuntimeDefaultExecutionOrderDeclarationsAreExact()
        {
            AssertDeclaredOrder(typeof(TransferSimulationDriver), -200);
            AssertDeclaredOrder(typeof(CombatSimulationDriver), -190);
            AssertDeclaredOrder(typeof(RegularEnemyReactionSimulationDriver), -180);
            AssertDeclaredOrder(typeof(RegularEnemyBehaviorSimulationDriver), -170);
            AssertDeclaredOrder(typeof(RegularEnemyLocomotionSimulationDriver), -160);
            AssertDeclaredOrder(typeof(PlayerMovementController), 0);
            AssertDeclaredOrder(typeof(RegularEnemyThreatSimulationDriver), 100);
            AssertDeclaredOrder(typeof(RegularEnemyEncounterHandoffAdapter), 110);
        }

        // AC-COM-001, AC-WT-003. Unity serializes each registry through a
        // matching MonoScript asset instead of a secondary-class fileID 0.
        [Test]
        public void RegistryComponentsResolveToMatchingSourceAssets()
        {
            var root = new GameObject("RegistryScriptIdentity");
            try
            {
                var transfer = root.AddComponent<TransferTargetRegistry>();
                var combat = root.AddComponent<CombatTargetRegistry>();
                Assert.That(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(transfer)),
                    Is.EqualTo("Assets/AcadeGameMaker/Runtime/Transfer/Unity/TransferTargetRegistry.cs"));
                Assert.That(AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(combat)),
                    Is.EqualTo("Assets/AcadeGameMaker/Runtime/Combat/Unity/CombatTargetRegistry.cs"));
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
        }

        // AC-COM-001, AC-COM-003. The existing target invariant rejects an
        // unnamed transient Scene; the builder source-order test proves it saves
        // the allowlisted Scene path before constructing or validating the graph.
        [Test]
        public void UnnamedWorkingSceneRejectsAuthoredTargetBeforeBuilderInitialSave()
        {
            var unnamed = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var invalid = CreatePlayerTarget("UnnamedPlayer");
            try
            {
                Assert.That(unnamed.name, Is.Empty);
                Assert.Throws<InvalidOperationException>(() => invalid.Freeze());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(invalid.gameObject);
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
        }

        // AC-COM-001, AC-COM-003. The stage is explicit; the final B1 graph is
        // rejected by B0 validation rather than inferred from its component set.
        [Test]
        public void B1GraphIsRejectedWhenCallerExplicitlySelectsB0()
        {
            var contents = PrefabUtility.LoadPrefabContents(RegularEnemyEncounterAuthoringBuilder.GraphPrefabPath);
            try
            {
                Assert.Throws<InvalidOperationException>(() =>
                    RegularEnemyEncounterAuthoringValidator.Validate(contents, RegularEnemyEncounterAuthoringStage.B0, false));
            }
            finally { PrefabUtility.UnloadPrefabContents(contents); }
        }

        // AC-WT-003, AC-WT-005; affected AC-COM-001. Exact Heavy apply/clear owns no combat state.
        [Test]
        public void ProductionEnemySinkAcceptsOnlyExactMonotonicHeavyTransaction()
        {
            var root = new GameObject("WalkerSink");
            try
            {
                var sink = root.AddComponent<RegularEnemyTransferModifierSink>();
                sink.ConfigureForAuthoring("walker", RegularEnemyTransferModifierSink.RequiredBaseProfileId);
                var exact = Request("walker", 0, 1);
                Assert.That(sink.TryApply(exact), Is.True);
                Assert.That(sink.IsHeavy, Is.True);
                Assert.That(sink.LastAcceptedRevision, Is.EqualTo(1));
                Assert.That(sink.TryApply(exact), Is.False);
                Assert.That(sink.TryApply(Request("surveyor", 1, 2)), Is.False);
                Assert.That(sink.TryApply(new TransferModifierRequest("walker", TransferTargetKind.Enemy, "foreign",
                    RegularEnemyTransferModifierSink.RequiredHeavyModifierId, new SimulationTick(1), 2)), Is.False);
                Assert.That(sink.LastAcceptedRevision, Is.EqualTo(1));
                sink.Clear(TransferClearReason.TargetRemoved, new SimulationTick(1), 2);
                Assert.That(sink.IsHeavy, Is.False);
                Assert.That(sink.LastAcceptedRevision, Is.EqualTo(2));
                Assert.DoesNotThrow(() => sink.Clear(TransferClearReason.TargetRemoved, new SimulationTick(2), 2));
                Assert.That(sink.TryApply(Request("walker", 2, 2)), Is.False);
                Assert.That(sink.TryApply(Request("walker", 2, 3)), Is.True);
                root.SetActive(false);
                Assert.That(sink.IsStillAvailable(), Is.False);
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
        }

        // AC-WT-003, AC-WT-005. Invalid authoring is rejected before any sink mutation.
        [Test]
        public void ProductionEnemySinkRejectsForeignRoleProfileAndInvalidClear()
        {
            var root = new GameObject("Sink");
            try
            {
                var sink = root.AddComponent<RegularEnemyTransferModifierSink>();
                Assert.Throws<ArgumentException>(() => sink.ConfigureForAuthoring("player", RegularEnemyTransferModifierSink.RequiredBaseProfileId));
                Assert.Throws<ArgumentException>(() => sink.ConfigureForAuthoring("walker", "foreign"));
                sink.ConfigureForAuthoring("surveyor", RegularEnemyTransferModifierSink.RequiredBaseProfileId);
                Assert.Throws<ArgumentOutOfRangeException>(() => sink.Clear((TransferClearReason)99, new SimulationTick(0), 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => sink.Clear(TransferClearReason.ManualRecall, new SimulationTick(-1), 1));
                Assert.That(sink.TryApply(new TransferModifierRequest("surveyor", TransferTargetKind.Enemy,
                    RegularEnemyTransferModifierSink.RequiredBaseProfileId, RegularEnemyTransferModifierSink.RequiredHeavyModifierId,
                    new SimulationTick(-1), 1)), Is.False);
                Assert.That(sink.IsHeavy, Is.False);
                Assert.That(sink.LastAcceptedRevision, Is.Zero);
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
        }

        private static TransferModifierRequest Request(string role, int tick, int revision) =>
            new TransferModifierRequest(role, TransferTargetKind.Enemy,
                RegularEnemyTransferModifierSink.RequiredBaseProfileId,
                RegularEnemyTransferModifierSink.RequiredHeavyModifierId, new SimulationTick(tick), revision);

        private static CombatTarget CreatePlayerTarget(string name)
        {
            var root = new GameObject(name);
            var collider = root.AddComponent<CapsuleCollider2D>();
            collider.direction = CapsuleDirection2D.Vertical;
            collider.size = new Vector2(0.8f, 1.6f);
            var target = root.AddComponent<CombatTarget>();
            target.ConfigureForAuthoring("player", 5, 45, false, 0, 0, 0, 0, 400, 800, collider, root.transform);
            return target;
        }

        private static void AssertDeclaredOrder(Type type, int expected)
        {
            var attributes = Attribute.GetCustomAttributes(type, typeof(DefaultExecutionOrder), false);
            if (expected == 0)
            {
                Assert.That(attributes, Is.Empty, $"{type.Name} must use Unity default order 0 by absence.");
                return;
            }
            Assert.That(attributes.Length, Is.EqualTo(1));
            Assert.That(((DefaultExecutionOrder)attributes[0]).order, Is.EqualTo(expected));
        }

        private static readonly GraphMutation[] ExactGraphMutations =
        {
            GraphMutation.MissingPlayer, GraphMutation.MissingWalker, GraphMutation.MissingSurveyor,
            GraphMutation.DuplicateRole, GraphMutation.RoleIdCorrupt, GraphMutation.CombatRosterOrderCorrupt,
            GraphMutation.TransferRosterOrderCorrupt, GraphMutation.MissingTransferRegistry,
            GraphMutation.DuplicateTransferRegistry, GraphMutation.MissingCombatRegistry,
            GraphMutation.DuplicateCombatRegistry, GraphMutation.MissingTransferDriver,
            GraphMutation.CrossWireTransferDriver, GraphMutation.MissingCombatDriver,
            GraphMutation.CrossWireCombatDriver, GraphMutation.MissingReactionDriver,
            GraphMutation.CrossWireReactionDriver, GraphMutation.MissingBehaviorDriver,
            GraphMutation.CrossWireBehaviorDriver, GraphMutation.MissingLocomotionDriver,
            GraphMutation.CrossWireLocomotionDriver, GraphMutation.MissingThreatDriver,
            GraphMutation.CrossWireThreatDriver, GraphMutation.ForeignRootBehaviour,
            GraphMutation.ForeignActorCollider, GraphMutation.ForeignSystemsComponent,
            GraphMutation.ForeignEnvironmentBody, GraphMutation.RootPosition,
            GraphMutation.DirectChildOrder, GraphMutation.RoleName, GraphMutation.RolePosition,
            GraphMutation.RoleRotation, GraphMutation.RoleScale, GraphMutation.RoleChild,
            GraphMutation.SystemsName, GraphMutation.SystemsPosition, GraphMutation.SystemsChild,
            GraphMutation.EnvironmentName, GraphMutation.EnvironmentPosition,
            GraphMutation.EnvironmentOrder, GraphMutation.EnvironmentChildName,
            GraphMutation.EnvironmentChildPosition, GraphMutation.EnvironmentChildRotation,
            GraphMutation.EnvironmentChildScale, GraphMutation.EnvironmentBoxSize,
            GraphMutation.EnvironmentBoxOffset, GraphMutation.EnvironmentBoxEdgeRadius,
            GraphMutation.EnvironmentLayer, GraphMutation.EnvironmentTrigger,
            GraphMutation.PlayerExtraSolid, GraphMutation.PlayerColliderDisabled,
            GraphMutation.PlayerCapsuleDirection, GraphMutation.PlayerCapsuleSize,
            GraphMutation.PlayerCapsuleOffset, GraphMutation.PlayerBodyType,
            GraphMutation.PlayerBodySimulation, GraphMutation.PlayerGravity,
            GraphMutation.EnemyExtraSolid, GraphMutation.EnemyColliderDisabled,
            GraphMutation.EnemyBoxSize, GraphMutation.EnemyBoxOffset,
            GraphMutation.EnemyBoxTrigger, GraphMutation.EnemyBodyType,
            GraphMutation.EnemyBodySimulation, GraphMutation.EnemyGravity,
            GraphMutation.ActorLayer, GraphMutation.RootLayer, GraphMutation.SystemsLayer,
            GraphMutation.InactiveRoot, GraphMutation.InactiveRole,
            GraphMutation.MissingHandoff, GraphMutation.DuplicateHandoff,
            GraphMutation.HandoffPlayerNull, GraphMutation.HandoffTransferNull, GraphMutation.HandoffCombatNull,
            GraphMutation.HandoffReactionNull, GraphMutation.HandoffBehaviorNull, GraphMutation.HandoffLocomotionNull,
            GraphMutation.HandoffThreatNull, GraphMutation.HandoffPlayerColliderNull,
            GraphMutation.HandoffSurveyorTargetNull, GraphMutation.HandoffWalkerTargetNull,
            GraphMutation.HandoffSurveyorColliderNull, GraphMutation.HandoffWalkerColliderNull,
            GraphMutation.HandoffForeignSameIdTarget
        };

        private static void ApplyMutation(GameObject root, GraphMutation mutation)
        {
            var player = root.transform.Find("CombatEncounterPlayer");
            var walker = root.transform.Find("Walker");
            var surveyor = root.transform.Find("Surveyor");
            var systems = root.transform.Find("Systems");
            var environment = root.transform.Find("Environment");
            switch (mutation)
            {
                case GraphMutation.MissingPlayer: UnityEngine.Object.DestroyImmediate(player.gameObject); break;
                case GraphMutation.MissingWalker: UnityEngine.Object.DestroyImmediate(walker.gameObject); break;
                case GraphMutation.MissingSurveyor: UnityEngine.Object.DestroyImmediate(surveyor.gameObject); break;
                case GraphMutation.DuplicateRole:
                    var duplicate = UnityEngine.Object.Instantiate(walker.gameObject, root.transform);
                    duplicate.name = "Walker";
                    break;
                case GraphMutation.RoleIdCorrupt:
                    SetString(walker.GetComponent<TransferTarget>(), "_targetId", "surveyor");
                    break;
                case GraphMutation.CombatRosterOrderCorrupt:
                    SwapArrayElements(systems.GetComponent<CombatTargetRegistry>(), "_targets", 1, 2);
                    break;
                case GraphMutation.TransferRosterOrderCorrupt:
                    SwapArrayElements(systems.GetComponent<TransferTargetRegistry>(), "_targets", 0, 1);
                    break;
                case GraphMutation.MissingTransferRegistry: UnityEngine.Object.DestroyImmediate(systems.GetComponent<TransferTargetRegistry>()); break;
                case GraphMutation.DuplicateTransferRegistry: DuplicateSystems(root, systems); break;
                case GraphMutation.MissingCombatRegistry: UnityEngine.Object.DestroyImmediate(systems.GetComponent<CombatTargetRegistry>()); break;
                case GraphMutation.DuplicateCombatRegistry: DuplicateSystems(root, systems); break;
                case GraphMutation.MissingTransferDriver: UnityEngine.Object.DestroyImmediate(systems.GetComponent<TransferSimulationDriver>()); break;
                case GraphMutation.CrossWireTransferDriver: ClearReference(systems.GetComponent<TransferSimulationDriver>(), "_player"); break;
                case GraphMutation.MissingCombatDriver: UnityEngine.Object.DestroyImmediate(systems.GetComponent<CombatSimulationDriver>()); break;
                case GraphMutation.CrossWireCombatDriver: ClearReference(systems.GetComponent<CombatSimulationDriver>(), "_registry"); break;
                case GraphMutation.MissingReactionDriver: UnityEngine.Object.DestroyImmediate(systems.GetComponent<RegularEnemyReactionSimulationDriver>()); break;
                case GraphMutation.CrossWireReactionDriver: ClearReference(systems.GetComponent<RegularEnemyReactionSimulationDriver>(), "_transferDriver"); break;
                case GraphMutation.MissingBehaviorDriver: UnityEngine.Object.DestroyImmediate(systems.GetComponent<RegularEnemyBehaviorSimulationDriver>()); break;
                case GraphMutation.CrossWireBehaviorDriver: ClearReference(systems.GetComponent<RegularEnemyBehaviorSimulationDriver>(), "_reactionDriver"); break;
                case GraphMutation.MissingLocomotionDriver: UnityEngine.Object.DestroyImmediate(systems.GetComponent<RegularEnemyLocomotionSimulationDriver>()); break;
                case GraphMutation.CrossWireLocomotionDriver: ClearReference(systems.GetComponent<RegularEnemyLocomotionSimulationDriver>(), "_behaviorDriver"); break;
                case GraphMutation.MissingThreatDriver: UnityEngine.Object.DestroyImmediate(systems.GetComponent<RegularEnemyThreatSimulationDriver>()); break;
                case GraphMutation.CrossWireThreatDriver: ClearReference(systems.GetComponent<RegularEnemyThreatSimulationDriver>(), "_locomotionDriver"); break;
                case GraphMutation.ForeignRootBehaviour: root.AddComponent<ForeignBehaviour>(); break;
                case GraphMutation.ForeignActorCollider: walker.gameObject.AddComponent<CircleCollider2D>(); break;
                case GraphMutation.ForeignSystemsComponent: systems.gameObject.AddComponent<AudioSource>(); break;
                case GraphMutation.ForeignEnvironmentBody: environment.GetChild(0).gameObject.AddComponent<Rigidbody2D>(); break;
                case GraphMutation.RootPosition: root.transform.localPosition = Vector3.right; break;
                case GraphMutation.DirectChildOrder: walker.SetSiblingIndex(0); break;
                case GraphMutation.RoleName: walker.name = "WalkerRenamed"; break;
                case GraphMutation.RolePosition: walker.localPosition += Vector3.right; break;
                case GraphMutation.RoleRotation: walker.localRotation = Quaternion.Euler(0f, 0f, 1f); break;
                case GraphMutation.RoleScale: walker.localScale = new Vector3(2f, 1f, 1f); break;
                case GraphMutation.RoleChild: new GameObject("ForeignChild").transform.SetParent(walker, false); break;
                case GraphMutation.SystemsName: systems.name = "SystemsRenamed"; break;
                case GraphMutation.SystemsPosition: systems.localPosition = Vector3.right; break;
                case GraphMutation.SystemsChild: new GameObject("ForeignChild").transform.SetParent(systems, false); break;
                case GraphMutation.EnvironmentName: environment.name = "EnvironmentRenamed"; break;
                case GraphMutation.EnvironmentPosition: environment.localPosition = Vector3.right; break;
                case GraphMutation.EnvironmentOrder: environment.GetChild(1).SetSiblingIndex(0); break;
                case GraphMutation.EnvironmentChildName: environment.GetChild(0).name = "Floor"; break;
                case GraphMutation.EnvironmentChildPosition: environment.GetChild(0).localPosition += Vector3.right; break;
                case GraphMutation.EnvironmentChildRotation: environment.GetChild(0).localRotation = Quaternion.Euler(0f, 0f, 1f); break;
                case GraphMutation.EnvironmentChildScale: environment.GetChild(0).localScale = new Vector3(2f, 1f, 1f); break;
                case GraphMutation.EnvironmentBoxSize: environment.GetChild(0).GetComponent<BoxCollider2D>().size = Vector2.one; break;
                case GraphMutation.EnvironmentBoxOffset: environment.GetChild(0).GetComponent<BoxCollider2D>().offset = Vector2.right; break;
                case GraphMutation.EnvironmentBoxEdgeRadius: environment.GetChild(0).GetComponent<BoxCollider2D>().edgeRadius = 0.1f; break;
                case GraphMutation.EnvironmentLayer: environment.GetChild(0).gameObject.layer = 0; break;
                case GraphMutation.EnvironmentTrigger: environment.GetChild(0).GetComponent<BoxCollider2D>().isTrigger = true; break;
                case GraphMutation.PlayerExtraSolid: player.gameObject.AddComponent<BoxCollider2D>(); break;
                case GraphMutation.PlayerColliderDisabled: player.GetComponent<CapsuleCollider2D>().enabled = false; break;
                case GraphMutation.PlayerCapsuleDirection: player.GetComponent<CapsuleCollider2D>().direction = CapsuleDirection2D.Horizontal; break;
                case GraphMutation.PlayerCapsuleSize: player.GetComponent<CapsuleCollider2D>().size = Vector2.one; break;
                case GraphMutation.PlayerCapsuleOffset: player.GetComponent<CapsuleCollider2D>().offset = Vector2.right; break;
                case GraphMutation.PlayerBodyType: player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; break;
                case GraphMutation.PlayerBodySimulation: player.GetComponent<Rigidbody2D>().simulated = false; break;
                case GraphMutation.PlayerGravity: player.GetComponent<Rigidbody2D>().gravityScale = 0f; break;
                case GraphMutation.EnemyExtraSolid: walker.gameObject.AddComponent<CircleCollider2D>(); break;
                case GraphMutation.EnemyColliderDisabled: walker.GetComponent<BoxCollider2D>().enabled = false; break;
                case GraphMutation.EnemyBoxSize: walker.GetComponent<BoxCollider2D>().size = Vector2.one; break;
                case GraphMutation.EnemyBoxOffset: walker.GetComponent<BoxCollider2D>().offset = Vector2.right; break;
                case GraphMutation.EnemyBoxTrigger: walker.GetComponent<BoxCollider2D>().isTrigger = true; break;
                case GraphMutation.EnemyBodyType: walker.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; break;
                case GraphMutation.EnemyBodySimulation: walker.GetComponent<Rigidbody2D>().simulated = false; break;
                case GraphMutation.EnemyGravity: walker.GetComponent<Rigidbody2D>().gravityScale = 1f; break;
                case GraphMutation.ActorLayer: walker.gameObject.layer = 8; break;
                case GraphMutation.RootLayer: root.layer = 8; break;
                case GraphMutation.SystemsLayer: systems.gameObject.layer = 8; break;
                case GraphMutation.InactiveRoot: root.SetActive(false); break;
                case GraphMutation.InactiveRole: walker.gameObject.SetActive(false); break;
                case GraphMutation.MissingHandoff: UnityEngine.Object.DestroyImmediate(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>()); break;
                case GraphMutation.DuplicateHandoff: root.AddComponent<RegularEnemyEncounterHandoffAdapter>(); break;
                case GraphMutation.HandoffPlayerNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_player"); break;
                case GraphMutation.HandoffTransferNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_transfer"); break;
                case GraphMutation.HandoffCombatNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_combat"); break;
                case GraphMutation.HandoffReactionNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_reaction"); break;
                case GraphMutation.HandoffBehaviorNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_behavior"); break;
                case GraphMutation.HandoffLocomotionNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_locomotion"); break;
                case GraphMutation.HandoffThreatNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_threat"); break;
                case GraphMutation.HandoffPlayerColliderNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_playerCollider"); break;
                case GraphMutation.HandoffSurveyorTargetNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_surveyorTarget"); break;
                case GraphMutation.HandoffWalkerTargetNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_walkerTarget"); break;
                case GraphMutation.HandoffSurveyorColliderNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_surveyorCollider"); break;
                case GraphMutation.HandoffWalkerColliderNull: ClearReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_walkerCollider"); break;
                case GraphMutation.HandoffForeignSameIdTarget:
                {
                    var foreign = UnityEngine.Object.Instantiate(surveyor.gameObject, root.transform);
                    foreign.name = "ForeignSurveyor";
                    SetReference(systems.GetComponent<RegularEnemyEncounterHandoffAdapter>(), "_surveyorTarget", foreign.GetComponent<TransferTarget>());
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(mutation));
            }
        }

        private static void DuplicateSystems(GameObject root, Transform systems)
        {
            var duplicate = UnityEngine.Object.Instantiate(systems.gameObject, root.transform);
            duplicate.name = "Systems";
        }

        private static void ClearReference(UnityEngine.Object target, string propertyName)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(propertyName).objectReferenceValue = null;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(propertyName).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetString(UnityEngine.Object target, string propertyName, string value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(propertyName).stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SwapArrayElements(UnityEngine.Object target, string propertyName, int left, int right)
        {
            var serialized = new SerializedObject(target);
            var array = serialized.FindProperty(propertyName);
            var first = array.GetArrayElementAtIndex(left).objectReferenceValue;
            array.GetArrayElementAtIndex(left).objectReferenceValue = array.GetArrayElementAtIndex(right).objectReferenceValue;
            array.GetArrayElementAtIndex(right).objectReferenceValue = first;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssertFrozenUninitialized(CombatTargetRegistry registry) =>
            Assert.Throws<InvalidOperationException>(() => _ = registry.FrozenTargets);

        internal enum GraphMutation
        {
            MissingPlayer, MissingWalker, MissingSurveyor, DuplicateRole, RoleIdCorrupt,
            CombatRosterOrderCorrupt, TransferRosterOrderCorrupt, MissingTransferRegistry,
            DuplicateTransferRegistry, MissingCombatRegistry, DuplicateCombatRegistry,
            MissingTransferDriver, CrossWireTransferDriver, MissingCombatDriver, CrossWireCombatDriver,
            MissingReactionDriver, CrossWireReactionDriver, MissingBehaviorDriver, CrossWireBehaviorDriver,
            MissingLocomotionDriver, CrossWireLocomotionDriver, MissingThreatDriver, CrossWireThreatDriver,
            ForeignRootBehaviour, ForeignActorCollider, ForeignSystemsComponent, ForeignEnvironmentBody,
            RootPosition, DirectChildOrder, RoleName, RolePosition, RoleRotation, RoleScale, RoleChild,
            SystemsName, SystemsPosition, SystemsChild, EnvironmentName, EnvironmentPosition,
            EnvironmentOrder, EnvironmentChildName, EnvironmentChildPosition, EnvironmentChildRotation,
            EnvironmentChildScale, EnvironmentBoxSize, EnvironmentBoxOffset, EnvironmentBoxEdgeRadius,
            EnvironmentLayer, EnvironmentTrigger, PlayerExtraSolid, PlayerColliderDisabled,
            PlayerCapsuleDirection, PlayerCapsuleSize, PlayerCapsuleOffset, PlayerBodyType,
            PlayerBodySimulation, PlayerGravity, EnemyExtraSolid, EnemyColliderDisabled,
            EnemyBoxSize, EnemyBoxOffset, EnemyBoxTrigger, EnemyBodyType, EnemyBodySimulation,
            EnemyGravity, ActorLayer, RootLayer, SystemsLayer, InactiveRoot, InactiveRole,
            MissingHandoff, DuplicateHandoff, HandoffPlayerNull, HandoffTransferNull, HandoffCombatNull,
            HandoffReactionNull, HandoffBehaviorNull, HandoffLocomotionNull, HandoffThreatNull,
            HandoffPlayerColliderNull, HandoffSurveyorTargetNull, HandoffWalkerTargetNull,
            HandoffSurveyorColliderNull, HandoffWalkerColliderNull, HandoffForeignSameIdTarget
        }

        private sealed class ForeignBehaviour : MonoBehaviour { }
    }
}
