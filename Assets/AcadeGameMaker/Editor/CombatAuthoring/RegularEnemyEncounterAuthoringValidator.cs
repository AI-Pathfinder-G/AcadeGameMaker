using System;
using System.Collections.Generic;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Combat.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Authoring
{
    internal static class RegularEnemyEncounterAuthoringValidator
    {
        // REQ-COM-001, REQ-COM-004, REQ-WT-003, REQ-WT-005. Validation is
        // root-bounded, read-only with respect to assets, and never discovers role ownership.
        internal static void Validate(GameObject root, RegularEnemyEncounterAuthoringStage stage, bool requireInitialized)
        {
            if (stage != RegularEnemyEncounterAuthoringStage.B0 && stage != RegularEnemyEncounterAuthoringStage.B1)
                throw new ArgumentOutOfRangeException(nameof(stage));
            if (root == null || root.name != "CombatEncounterGraph" || !root.activeInHierarchy)
                throw new InvalidOperationException("Encounter graph requires one active literal CombatEncounterGraph root.");

            ValidateExactAuthoredTopology(root, stage);

            var players = Exact<PlayerMovementController>(root, 1);
            var transfers = Exact<TransferTarget>(root, 2);
            var combatTargets = Exact<CombatTarget>(root, 3);
            var sinks = Exact<RegularEnemyTransferModifierSink>(root, 2);
            var transferRegistries = Exact<TransferTargetRegistry>(root, 1);
            var combatRegistries = Exact<CombatTargetRegistry>(root, 1);
            var transferDrivers = Exact<TransferSimulationDriver>(root, 1);
            var combatDrivers = Exact<CombatSimulationDriver>(root, 1);
            var reactions = Exact<RegularEnemyReactionSimulationDriver>(root, 1);
            var behaviors = Exact<RegularEnemyBehaviorSimulationDriver>(root, 1);
            var locomotions = Exact<RegularEnemyLocomotionSimulationDriver>(root, 1);
            var threats = Exact<RegularEnemyThreatSimulationDriver>(root, 1);
            var handoffs = root.GetComponentsInChildren<RegularEnemyEncounterHandoffAdapter>(true);
            if (handoffs.Length != (stage == RegularEnemyEncounterAuthoringStage.B1 ? 1 : 0))
                throw new InvalidOperationException("Encounter Handoff driver count does not match the explicit authoring stage.");

            var player = players[0];
            var surveyorTransfer = transfers[0].TargetId == "surveyor" ? transfers[0] : transfers[1];
            var walkerTransfer = transfers[0].TargetId == "walker" ? transfers[0] : transfers[1];
            if (surveyorTransfer.TargetId != "surveyor" || walkerTransfer.TargetId != "walker")
                throw new InvalidOperationException("Encounter graph requires literal surveyor and walker Transfer roles.");
            var playerCombat = Select(combatTargets, "player");
            var surveyorCombat = Select(combatTargets, "surveyor");
            var walkerCombat = Select(combatTargets, "walker");
            ValidatePlayer(player, playerCombat);
            ValidateEnemy(surveyorTransfer, surveyorCombat, SelectSink(sinks, "surveyor"), 6);
            ValidateEnemy(walkerTransfer, walkerCombat, SelectSink(sinks, "walker"), 9);

            var transferRegistry = transferRegistries[0];
            var combatRegistry = combatRegistries[0];
            var transferDriver = transferDrivers[0];
            var combatDriver = combatDrivers[0];
            var reaction = reactions[0];
            var behavior = behaviors[0];
            var locomotion = locomotions[0];
            var threat = threats[0];
            transferDriver.ValidateEncounterGraph(player, transferRegistry, surveyorTransfer, walkerTransfer, requireInitialized);
            ValidateCombatRoster(combatRegistry.ReadValidatedAuthoringRoster(), playerCombat, surveyorCombat, walkerCombat);
            combatDriver.ValidateEncounterGraph(player, combatRegistry, transferDriver, reaction);
            reaction.ValidateEncounterGraph(player, transferDriver, combatDriver);
            behavior.ValidateEncounterGraph(player, combatDriver, reaction);
            locomotion.ValidateEncounterGraph(player, combatDriver, reaction, behavior);
            threat.ValidateEncounterGraph(player, combatDriver, behavior, locomotion);
            if (stage == RegularEnemyEncounterAuthoringStage.B1)
                handoffs[0].ValidateEncounterGraph(player, transferDriver, combatDriver, reaction, behavior, locomotion, threat,
                    player.GetComponent<CapsuleCollider2D>(), surveyorTransfer, walkerTransfer,
                    surveyorTransfer.GetComponent<BoxCollider2D>(), walkerTransfer.GetComponent<BoxCollider2D>());

            AssertOrder(transferDriver, -200);
            AssertOrder(combatDriver, -190);
            AssertOrder(reaction, -180);
            AssertOrder(behavior, -170);
            AssertOrder(locomotion, -160);
            AssertOrder(player, 0);
            AssertOrder(threat, 100);
            if (stage == RegularEnemyEncounterAuthoringStage.B1) AssertOrder(handoffs[0], 110);
            ValidateEnvironment(root);
            ValidateCollisionNeutral(player, surveyorTransfer, walkerTransfer);
            ValidateNoAdditionalBehaviours(root, stage);
        }

        private static void ValidateExactAuthoredTopology(GameObject root, RegularEnemyEncounterAuthoringStage stage)
        {
            // REQ-COM-001, REQ-COM-004. Each explicit stage is an exact authored fixture, not a
            // discovery surface: hierarchy, sibling order, transforms and component
            // membership are all part of the frozen composition contract.
            AssertTransform(root.transform, Vector3.zero);
            AssertExactComponents(root, typeof(Transform));
            if (root.layer != 0 || root.transform.childCount != 5)
                throw new InvalidOperationException("Encounter graph root requires exactly five direct children.");

            var player = AssertDirectChild(root.transform, 0, "CombatEncounterPlayer", new Vector3(0f, 0.81f, 0f));
            var walker = AssertDirectChild(root.transform, 1, "Walker", new Vector3(3f, 0.81f, 0f));
            var surveyor = AssertDirectChild(root.transform, 2, "Surveyor", new Vector3(6f, 0.81f, 0f));
            var systems = AssertDirectChild(root.transform, 3, "Systems", Vector3.zero);
            var environment = AssertDirectChild(root.transform, 4, "Environment", Vector3.zero);

            AssertLeaf(player);
            AssertExactComponents(player.gameObject, typeof(Transform), typeof(Rigidbody2D), typeof(CapsuleCollider2D),
                typeof(PlayerMovementController), typeof(CombatTarget));
            AssertLeaf(walker);
            AssertExactEnemyComponents(walker.gameObject);
            AssertLeaf(surveyor);
            AssertExactEnemyComponents(surveyor.gameObject);

            AssertLeaf(systems);
            if (systems.gameObject.layer != 0 || environment.gameObject.layer != 0)
                throw new InvalidOperationException("Encounter graph containers require the default layer.");
            if (stage == RegularEnemyEncounterAuthoringStage.B0)
                AssertExactComponents(systems.gameObject, typeof(Transform), typeof(TransferTargetRegistry),
                    typeof(CombatTargetRegistry), typeof(TransferSimulationDriver), typeof(CombatSimulationDriver),
                    typeof(RegularEnemyReactionSimulationDriver), typeof(RegularEnemyBehaviorSimulationDriver),
                    typeof(RegularEnemyLocomotionSimulationDriver), typeof(RegularEnemyThreatSimulationDriver));
            else
                AssertExactComponents(systems.gameObject, typeof(Transform), typeof(TransferTargetRegistry),
                    typeof(CombatTargetRegistry), typeof(TransferSimulationDriver), typeof(CombatSimulationDriver),
                    typeof(RegularEnemyReactionSimulationDriver), typeof(RegularEnemyBehaviorSimulationDriver),
                    typeof(RegularEnemyLocomotionSimulationDriver), typeof(RegularEnemyThreatSimulationDriver),
                    typeof(RegularEnemyEncounterHandoffAdapter));
            AssertExactComponents(environment.gameObject, typeof(Transform));
        }

        private static void AssertExactEnemyComponents(GameObject value) =>
            AssertExactComponents(value, typeof(Transform), typeof(Rigidbody2D), typeof(BoxCollider2D),
                typeof(RegularEnemyTransferModifierSink), typeof(TransferTarget), typeof(CombatTarget));

        private static Transform AssertDirectChild(Transform parent, int index, string name, Vector3 localPosition)
        {
            var child = parent.GetChild(index);
            if (!string.Equals(child.name, name, StringComparison.Ordinal) || !child.gameObject.activeSelf)
                throw new InvalidOperationException($"Encounter graph direct child {index} must be literal {name}.");
            AssertTransform(child, localPosition);
            return child;
        }

        private static void AssertTransform(Transform value, Vector3 localPosition)
        {
            if (value.localPosition != localPosition || value.localRotation != Quaternion.identity || value.localScale != Vector3.one)
                throw new InvalidOperationException($"Encounter graph transform {value.name} is not exact.");
        }

        private static void AssertLeaf(Transform value)
        {
            if (value.childCount != 0) throw new InvalidOperationException($"Encounter graph role/container {value.name} must be a leaf.");
        }

        private static void AssertExactComponents(GameObject value, params Type[] expected)
        {
            var actual = value.GetComponents<Component>();
            if (actual.Length != expected.Length)
                throw new InvalidOperationException($"Encounter graph object {value.name} has a missing or foreign component.");
            var remaining = new List<Type>(expected);
            for (var index = 0; index < actual.Length; index++)
            {
                if (actual[index] == null || !remaining.Remove(actual[index].GetType()))
                    throw new InvalidOperationException($"Encounter graph object {value.name} component membership is not exact.");
            }
            if (remaining.Count != 0) throw new InvalidOperationException($"Encounter graph object {value.name} is missing an exact component.");
        }

        private static void ValidatePlayer(PlayerMovementController player, CombatTarget combat)
        {
            if (player.gameObject != combat.gameObject || player.gameObject.name != "CombatEncounterPlayer" ||
                player.gameObject.layer != 0 || player.GetComponent<TransferTarget>() != null)
                throw new InvalidOperationException("Encounter graph player role/root is not exact.");
            var body = player.GetComponent<Rigidbody2D>();
            var capsule = player.GetComponent<CapsuleCollider2D>();
            var frozen = combat.Freeze();
            if (body == null || capsule == null || frozen.TargetId != "player" || frozen.MaxHealth != 5 ||
                frozen.InvulnerabilityTicks != 45 || frozen.CanReceivePlayerBasicAttack || frozen.HasMatchingTransferTarget ||
                frozen.TargetCollider != capsule || frozen.TargetPoseRoot != player.transform ||
                body.bodyType != RigidbodyType2D.Kinematic || body.constraints != RigidbodyConstraints2D.FreezeRotation ||
                !body.simulated || body.interpolation != RigidbodyInterpolation2D.None || body.useFullKinematicContacts ||
                body.rotation != 0f || body.linearVelocity != Vector2.zero || body.angularVelocity != 0f ||
                body.gravityScale != 3.1315f || capsule.direction != CapsuleDirection2D.Vertical ||
                capsule.size != new Vector2(0.8f, 1.6f) || capsule.offset != Vector2.zero || capsule.isTrigger || !capsule.enabled)
                throw new InvalidOperationException("Encounter graph player scalar/body/geometry profile is not exact.");
            var solids = player.transform.GetComponentsInChildren<Collider2D>(true);
            if (solids.Length != 1 || solids[0] != capsule)
                throw new InvalidOperationException("Encounter graph player requires exactly one enabled solid CapsuleCollider2D.");
        }

        private static void ValidateEnemy(TransferTarget transfer, CombatTarget combat,
            RegularEnemyTransferModifierSink sink, int health)
        {
            transfer.ValidateForAuthoring();
            var frozen = combat.Freeze();
            var body = transfer.GetComponent<Rigidbody2D>();
            var box = transfer.GetComponent<BoxCollider2D>();
            if (transfer.gameObject != combat.gameObject || transfer.gameObject != sink.gameObject ||
                transfer.gameObject.layer != 0 || frozen.TargetId != transfer.TargetId || frozen.MaxHealth != health ||
                frozen.InvulnerabilityTicks != 0 || !frozen.CanReceivePlayerBasicAttack || !frozen.HasMatchingTransferTarget ||
                sink.BoundTargetId != transfer.TargetId || sink.BaseModifierProfileId != RegularEnemyTransferModifierSink.RequiredBaseProfileId ||
                body == null || box == null || body.bodyType != RigidbodyType2D.Kinematic || !body.simulated ||
                body.constraints != RigidbodyConstraints2D.FreezeRotation || body.interpolation != RigidbodyInterpolation2D.None ||
                body.useFullKinematicContacts || body.gravityScale != 0f || body.rotation != 0f || body.linearVelocity != Vector2.zero ||
                body.angularVelocity != 0f ||
                box.size != new Vector2(0.8f, 1.6f) || box.offset != Vector2.zero || box.edgeRadius != 0f || box.isTrigger)
                throw new InvalidOperationException("Encounter graph regular-enemy scalar/body/geometry/sink profile is not exact.");
            var solids = transfer.transform.GetComponentsInChildren<Collider2D>(true);
            if (solids.Length != 1 || solids[0] != box || !box.enabled)
                throw new InvalidOperationException("Encounter graph enemy requires exactly one enabled solid BoxCollider2D.");
        }

        private static void ValidateCombatRoster(IReadOnlyList<FrozenCombatTarget> frozen,
            CombatTarget player, CombatTarget surveyor, CombatTarget walker)
        {
            if (frozen.Count != 3 || frozen[0].TargetId != "player" || frozen[1].TargetId != "surveyor" ||
                frozen[2].TargetId != "walker" || frozen[0].TargetPoseRoot != player.transform ||
                frozen[1].TargetPoseRoot != surveyor.transform || frozen[2].TargetPoseRoot != walker.transform)
                throw new InvalidOperationException("Combat roster must be exact player, surveyor, walker order.");
        }

        private static void ValidateEnvironment(GameObject root)
        {
            var environment = root.transform.Find("Environment");
            if (environment == null || environment.childCount != 3) throw new InvalidOperationException("Encounter graph environment is incomplete.");
            ValidateEnvironmentBox(environment, 0, "Ground", new Vector3(0f, -0.5f, 0f), new Vector2(20f, 1f));
            ValidateEnvironmentBox(environment, 1, "LeftBound", new Vector3(-10f, 4f, 0f), new Vector2(1f, 10f));
            ValidateEnvironmentBox(environment, 2, "RightBound", new Vector3(10f, 4f, 0f), new Vector2(1f, 10f));
        }

        private static void ValidateEnvironmentBox(Transform parent, int index, string name, Vector3 position, Vector2 size)
        {
            var child = AssertDirectChild(parent, index, name, position);
            AssertLeaf(child);
            AssertExactComponents(child.gameObject, typeof(Transform), typeof(BoxCollider2D));
            var box = child.GetComponent<BoxCollider2D>();
            if (child.gameObject.layer != 8 || !child.gameObject.activeSelf || !box.enabled || box.isTrigger ||
                box.size != size || box.offset != Vector2.zero || box.edgeRadius != 0f)
                throw new InvalidOperationException("Encounter graph environment/layer-8 authoring is not exact.");
        }

        private static void ValidateCollisionNeutral(PlayerMovementController player, TransferTarget surveyor, TransferTarget walker)
        {
            var playerCollider = player.GetComponent<CapsuleCollider2D>();
            if (Physics2D.GetIgnoreCollision(playerCollider, surveyor.TargetCollider) ||
                Physics2D.GetIgnoreCollision(playerCollider, walker.TargetCollider))
                throw new InvalidOperationException("M3E1 encounter graph must begin collision-neutral.");
        }

        private static void ValidateNoAdditionalBehaviours(GameObject root, RegularEnemyEncounterAuthoringStage stage)
        {
            // Exact known sets prove the selected stage's handoff count without
            // discovery or inference from the asset contents.
            var values = root.GetComponentsInChildren<MonoBehaviour>(true);
            if (values.Length != (stage == RegularEnemyEncounterAuthoringStage.B0 ? 16 : 17)) throw new InvalidOperationException("Encounter graph contains a missing or foreign MonoBehaviour.");
            foreach (var value in values)
            {
                if (!(value is PlayerMovementController) && !(value is TransferTarget) && !(value is CombatTarget) &&
                    !(value is RegularEnemyTransferModifierSink) && !(value is TransferTargetRegistry) &&
                    !(value is CombatTargetRegistry) && !(value is TransferSimulationDriver) &&
                    !(value is CombatSimulationDriver) && !(value is RegularEnemyReactionSimulationDriver) &&
                    !(value is RegularEnemyBehaviorSimulationDriver) && !(value is RegularEnemyLocomotionSimulationDriver) &&
                    !(value is RegularEnemyThreatSimulationDriver) &&
                    !(stage == RegularEnemyEncounterAuthoringStage.B1 && value is RegularEnemyEncounterHandoffAdapter))
                    throw new InvalidOperationException("Encounter graph contains a foreign MonoBehaviour.");
            }
        }

        private static T[] Exact<T>(GameObject root, int count) where T : Component
        {
            var values = root.GetComponentsInChildren<T>(true);
            if (values.Length != count) throw new InvalidOperationException($"Encounter graph requires exactly {count} {typeof(T).Name} component(s).");
            return values;
        }

        private static CombatTarget Select(CombatTarget[] values, string role)
        {
            CombatTarget result = null;
            foreach (var value in values) if (value.TargetId == role) { if (result != null) throw new InvalidOperationException(); result = value; }
            return result ?? throw new InvalidOperationException($"Missing literal Combat role {role}.");
        }

        private static RegularEnemyTransferModifierSink SelectSink(RegularEnemyTransferModifierSink[] values, string role)
        {
            foreach (var value in values) if (value.BoundTargetId == role) return value;
            throw new InvalidOperationException($"Missing literal Transfer sink {role}.");
        }

        private static void AssertOrder(MonoBehaviour value, int expected)
        {
            // REQ-COM-001, REQ-COM-004. Runtime phase order is declared by the
            // component type's DefaultExecutionOrder attribute. A type without
            // that attribute has Unity's default order 0; importer override state
            // is neither the declaration nor an authored graph reference.
            var attributes = Attribute.GetCustomAttributes(value.GetType(), typeof(DefaultExecutionOrder), false);
            if ((expected == 0 && attributes.Length != 0) ||
                (expected != 0 && (attributes.Length != 1 || ((DefaultExecutionOrder)attributes[0]).order != expected)))
                throw new InvalidOperationException($"{value.GetType().Name} execution order must be {expected}.");
        }
    }
}
