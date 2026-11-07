#if UNITY_EDITOR
using System;
using System.Collections;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Combat.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class RegularEnemyEncounterAuthoredGraphPlayModeTests
    {
        private const string GraphPath = "Assets/Prefabs/Combat/CombatEncounterGraph.prefab";

        // AC-COM-001, AC-COM-003; AC-WT-003, AC-WT-005 (provisional gate 7).
        // Destroying the committed final B1 graph and instantiating a separate fresh graph
        // restores exact Transfer registrations and both collision pairs before Movement tick 0 publishes.
        [UnityTest]
        public IEnumerator DestroyAndFreshInstantiateRestoresTransferAndCollisionNeutralGraph()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(GraphPath);
            Assert.That(asset, Is.Not.Null, "Run the approved B1 builder before scene verification.");
            var first = UnityEngine.Object.Instantiate(asset);
            var firstRefs = CaptureRefs(first);
            Assert.That(first.GetComponentsInChildren<RegularEnemyEncounterHandoffAdapter>(true).Length, Is.EqualTo(1));

            // AC-COM-001, AC-COM-003; AC-WT-003, AC-WT-005. Production Combat
            // kills both authored enemies at t0 and production Transfer consumes
            // its exact t+1 removal transaction. No registry/session is mutated by the test.
            var tick0 = new SimulationTick(0);
            firstRefs.Combat.SubmitForTests(new CombatSimulationInput(tick0, null, null, null, new[]
            {
                new DamageRequest("b0.gate7/surveyor/0000000000", "b0.gate7", "surveyor", 6, DamageKind.HeavyImpact, tick0),
                new DamageRequest("b0.gate7/walker/0000000000", "b0.gate7", "walker", 9, DamageKind.HeavyImpact, tick0)
            }, false));
            yield return new WaitForFixedUpdate();
            Assert.That(Combatant(firstRefs.Combat, "surveyor").IsDead, Is.True);
            Assert.That(Combatant(firstRefs.Combat, "walker").IsDead, Is.True);
            yield return new WaitForFixedUpdate();
            Assert.That(firstRefs.Transfer.LatestPhasePublication.HasValue, Is.True);
            Assert.That(firstRefs.Transfer.LatestPhasePublication.Value.Tick, Is.EqualTo(new SimulationTick(1)));
            Assert.That(firstRefs.Transfer.LatestPhasePublication.Value.Input.RemovalCount, Is.EqualTo(2));
            Assert.Throws<InvalidOperationException>(() => firstRefs.Transfer.ValidateEncounterGraph(firstRefs.Player,
                firstRefs.Registry, firstRefs.Surveyor, firstRefs.Walker, true),
                "The old encounter must no longer retain the exact two Transfer registrations after t+1 removal.");

            // Test-only post-death pair state: B0 production still owns no pair mutation.
            Physics2D.IgnoreCollision(firstRefs.PlayerCollider, firstRefs.Surveyor.TargetCollider, true);
            Physics2D.IgnoreCollision(firstRefs.PlayerCollider, firstRefs.Walker.TargetCollider, true);
            Assert.That(Physics2D.GetIgnoreCollision(firstRefs.PlayerCollider, firstRefs.Surveyor.TargetCollider), Is.True);
            Assert.That(Physics2D.GetIgnoreCollision(firstRefs.PlayerCollider, firstRefs.Walker.TargetCollider), Is.True);
            UnityEngine.Object.Destroy(first);
            yield return null;
            Assert.That(firstRefs.Player == null, Is.True);

            var fresh = UnityEngine.Object.Instantiate(asset);
            try
            {
                var freshRefs = CaptureRefs(fresh);
                Assert.That(freshRefs.Player, Is.Not.SameAs(firstRefs.Player));
                Assert.That(freshRefs.Transfer, Is.Not.SameAs(firstRefs.Transfer));
                Assert.That(freshRefs.Surveyor, Is.Not.SameAs(firstRefs.Surveyor));
                Assert.That(freshRefs.Walker, Is.Not.SameAs(firstRefs.Walker));
                var probe = fresh.AddComponent<Gate7PreMovementProbe>();
                probe.Configure(freshRefs.Player, freshRefs.PlayerCollider, freshRefs.Transfer, freshRefs.Registry,
                    freshRefs.Surveyor, freshRefs.Walker);
                yield return new WaitForFixedUpdate();
                Assert.That(probe.Captured, Is.True);
                Assert.That(probe.Failure, Is.Null);
                Assert.That(probe.PlayerTickAtProbe, Is.Zero);
                Assert.That(probe.SurveyorPairIgnoredAtProbe, Is.False);
                Assert.That(probe.WalkerPairIgnoredAtProbe, Is.False);
            }
            finally { UnityEngine.Object.Destroy(fresh); }
        }

        private static FreshRefs CaptureRefs(GameObject root)
        {
            var player = Single<PlayerMovementController>(root);
            var transfer = Single<TransferSimulationDriver>(root);
            var combat = Single<CombatSimulationDriver>(root);
            var registry = Single<TransferTargetRegistry>(root);
            var targets = root.GetComponentsInChildren<TransferTarget>(true);
            Assert.That(targets.Length, Is.EqualTo(2));
            TransferTarget surveyor = null, walker = null;
            foreach (var target in targets) { if (target.TargetId == "surveyor") surveyor = target; if (target.TargetId == "walker") walker = target; }
            Assert.That(surveyor, Is.Not.Null);
            Assert.That(walker, Is.Not.Null);
            Assert.That(player.NextExpectedTick.Value, Is.Zero);
            var capsule = player.GetComponent<CapsuleCollider2D>();
            return new FreshRefs(player, capsule, transfer, combat, registry, surveyor, walker);
        }

        private static CombatantSnapshot Combatant(CombatSimulationDriver combat, string targetId)
        {
            var values = combat.LatestOutcomeForTests.Combatants;
            for (var index = 0; index < values.Count; index++)
                if (string.Equals(values[index].TargetId, targetId, StringComparison.Ordinal)) return values[index];
            throw new AssertionException($"Missing Combat role {targetId}.");
        }

        private static T Single<T>(GameObject root) where T : Component
        {
            var values = root.GetComponentsInChildren<T>(true);
            Assert.That(values.Length, Is.EqualTo(1));
            return values[0];
        }

        private readonly struct FreshRefs
        {
            internal FreshRefs(PlayerMovementController player, CapsuleCollider2D playerCollider,
                TransferSimulationDriver transfer, CombatSimulationDriver combat, TransferTargetRegistry registry,
                TransferTarget surveyor, TransferTarget walker)
            { Player = player; PlayerCollider = playerCollider; Transfer = transfer; Combat = combat; Registry = registry; Surveyor = surveyor; Walker = walker; }
            internal PlayerMovementController Player { get; }
            internal CapsuleCollider2D PlayerCollider { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal CombatSimulationDriver Combat { get; }
            internal TransferTargetRegistry Registry { get; }
            internal TransferTarget Surveyor { get; }
            internal TransferTarget Walker { get; }
        }

        // AC-COM-001, AC-COM-003; AC-WT-003, AC-WT-005. This test-only -195
        // observer executes after production Transfer -200 and before Movement 0.
        [DefaultExecutionOrder(-195)]
        private sealed class Gate7PreMovementProbe : MonoBehaviour
        {
            private PlayerMovementController _player;
            private CapsuleCollider2D _playerCollider;
            private TransferSimulationDriver _transfer;
            private TransferTargetRegistry _registry;
            private TransferTarget _surveyor;
            private TransferTarget _walker;

            internal bool Captured { get; private set; }
            internal System.Exception Failure { get; private set; }
            internal int PlayerTickAtProbe { get; private set; }
            internal bool SurveyorPairIgnoredAtProbe { get; private set; }
            internal bool WalkerPairIgnoredAtProbe { get; private set; }

            internal void Configure(PlayerMovementController player, CapsuleCollider2D playerCollider,
                TransferSimulationDriver transfer, TransferTargetRegistry registry, TransferTarget surveyor, TransferTarget walker)
            {
                _player = player; _playerCollider = playerCollider; _transfer = transfer;
                _registry = registry; _surveyor = surveyor; _walker = walker;
            }

            private void FixedUpdate()
            {
                if (Captured) return;
                try
                {
                    _transfer.ValidateEncounterGraph(_player, _registry, _surveyor, _walker, true);
                    PlayerTickAtProbe = _player.NextExpectedTick.Value;
                    SurveyorPairIgnoredAtProbe = Physics2D.GetIgnoreCollision(_playerCollider, _surveyor.TargetCollider);
                    WalkerPairIgnoredAtProbe = Physics2D.GetIgnoreCollision(_playerCollider, _walker.TargetCollider);
                }
                catch (System.Exception exception) { Failure = exception; }
                Captured = true;
            }
        }
    }
}
#endif
