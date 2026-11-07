// AC-COM-003, AC-UX-007, AC-UX-012, AC-WT-006. Isolated M2B1 physics capture exercises the verified Transfer.Unity LOS path.
using System;
using System.Collections.Generic;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.CombatUnity
{
    internal sealed class CombatObservationBuilderPlayModeTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private SimulationMode2D _previousSimulationMode;

        [SetUp] public void SetUp() { _previousSimulationMode = Physics2D.simulationMode; Physics2D.simulationMode = SimulationMode2D.Script; }
        [TearDown] public void TearDown() { for (var i = _objects.Count - 1; i >= 0; i--) UnityEngine.Object.DestroyImmediate(_objects[i]); _objects.Clear(); Physics2D.simulationMode = _previousSimulationMode; Physics2D.SyncTransforms(); }

        // AC-WT-006. This test owns the external capture sync; CombatObservationBuilder itself has no synchronization authority.
        [Test]
        public void CaptureUsesMask256AndIgnoresTriggerAndTargetRootButBlocksOtherGeometry()
        {
            var player = Track(new GameObject("PlayerRoot"));
            var registry = Track(new GameObject("Registry")).AddComponent<CombatTargetRegistry>();
            var target = Create("target", 4f, true);
            registry.ConfigureForTests(target); registry.ValidateAndFreeze();
            Physics2D.SyncTransforms();
            Assert.That(Capture(registry, player.transform)[0].IsLineOfSightOpen, Is.True);

            var trigger = Blocker("Trigger", 1f, true);
            Physics2D.SyncTransforms();
            Assert.That(Capture(registry, player.transform)[0].IsLineOfSightOpen, Is.True);
            UnityEngine.Object.DestroyImmediate(trigger);

            var targetRootCollider = target.gameObject.GetComponent<Collider2D>();
            targetRootCollider.gameObject.layer = 8;
            Physics2D.SyncTransforms();
            Assert.That(Capture(registry, player.transform)[0].IsLineOfSightOpen, Is.True);
            targetRootCollider.gameObject.layer = 0;
            var blocker = Blocker("Blocker", 2f, false);
            Physics2D.SyncTransforms();
            Assert.That(Capture(registry, player.transform)[0].IsLineOfSightOpen, Is.False);
            Assert.That(blocker.layer, Is.EqualTo(8));
        }

        // AC-COM-003. Player/non-attackable/dead entries are excluded through combat-owned input, not Transfer availability.
        [Test]
        public void CaptureExcludesPlayerAndRepresentsOnlyCombatOwnedAttackability()
        {
            var playerRoot = Track(new GameObject("PlayerRoot"));
            var registry = Track(new GameObject("Registry")).AddComponent<CombatTargetRegistry>();
            var player = Create("player", 1f, false);
            var disabled = Create("disabled", 2f, false);
            var dead = Create("dead", 3f, true);
            var live = Create("live", 4f, true);
            registry.ConfigureForTests(player, dead, disabled, live);
            Assert.Throws<InvalidOperationException>(() => registry.ValidateAndFreeze());
            registry.ConfigureForTests(dead, disabled, live, player);
            // 'dead' < 'disabled' < 'live' < 'player' is the authored ordinal roster.
            registry.ValidateAndFreeze();
            Physics2D.SyncTransforms();
            var observations = new CombatObservationBuilder().Capture(new SimulationTick(1), Player(0), Camera(0), Aim(1), registry.FrozenTargets,
                new Dictionary<string, bool> { ["dead"] = false, ["disabled"] = true, ["live"] = true, ["player"] = true }, playerRoot.transform);
            Assert.That(observations.Count, Is.EqualTo(3));
            Assert.That(observations[0].TargetId, Is.EqualTo("dead")); Assert.That(observations[0].IsAttackable, Is.False);
            Assert.That(observations[1].TargetId, Is.EqualTo("disabled")); Assert.That(observations[1].IsAttackable, Is.False);
            Assert.That(observations[2].TargetId, Is.EqualTo("live")); Assert.That(observations[2].IsAttackable, Is.True);
        }

        private IReadOnlyList<AcadeGameMaker.Combat.BasicAttackCandidateObservation> Capture(CombatTargetRegistry registry, Transform playerRoot) =>
            new CombatObservationBuilder().Capture(new SimulationTick(1), Player(0), Camera(0), Aim(1), registry.FrozenTargets, new Dictionary<string, bool> { ["target"] = true }, playerRoot);
        private CombatTarget Create(string id, float x, bool attackable) { var go = Track(new GameObject(id)); go.transform.position = new Vector3(x, 0f, 11f); var collider = go.AddComponent<BoxCollider2D>(); var target = go.AddComponent<CombatTarget>(); target.ConfigureForTests(id, id == "player" ? 5 : 9, id == "player" ? 45 : 0, attackable, 0, 0, 0, 0, 100, 100, collider, go.transform); return target; }
        private GameObject Blocker(string name, float x, bool trigger) { var value = Track(new GameObject(name)); value.layer = 8; value.transform.position = new Vector2(x, 0f); value.AddComponent<BoxCollider2D>().isTrigger = trigger; return value; }
        private GameObject Track(GameObject value) { _objects.Add(value); return value; }
        private static PlayerMotionSnapshot Player(int tick) => new PlayerMotionSnapshot(new SimulationTick(tick), 0, 0, 0, 0, false, string.Empty, WallSide.None, MovementActionState.Normal, true);
        private static SimulationCameraPoseSnapshot Camera(int tick) => new SimulationCameraPoseSnapshot(new SimulationTick(tick), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
        private static AimSample Aim(int tick) => new AimSample(1, AimSource.GamepadStick, null, null, null, 4096, 0, new SimulationTick(tick - 1), new SimulationTick(tick));
    }
}
