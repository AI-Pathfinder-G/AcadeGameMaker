// AC-COM-003, AC-UX-007, AC-UX-012, AC-WT-006. M2B1 observes exact captured poses and delegates spatial math to Transfer.Unity.
using System;
using System.Collections.Generic;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    internal sealed class CombatObservationBuilderTests
    {
        private readonly List<GameObject> _objects = new List<GameObject>();
        private const string TemporaryScenePath = "Assets/AcadeGameMaker/Tests/EditMode/CombatUnity/__TempCombatObservationScene.unity";
        [SetUp] public void SetUp() { AssetDatabase.DeleteAsset(TemporaryScenePath); var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); Assert.That(EditorSceneManager.SaveScene(scene, TemporaryScenePath), Is.True); }
        [TearDown] public void TearDown() { for (var i = _objects.Count - 1; i >= 0; i--) UnityEngine.Object.DestroyImmediate(_objects[i]); _objects.Clear(); EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); AssetDatabase.DeleteAsset(TemporaryScenePath); AssetDatabase.Refresh(); }

        // AC-COM-003. 5.999/6.000/6.001 u convert to the shared 35,988/36,000/36,012 keys before M2A selection.
        [Test]
        public void CapturePinsPriorTickQ1000DistanceBoundariesAndCombatOwnedAvailability()
        {
            var registry = Track(new GameObject("Registry")).AddComponent<CombatTargetRegistry>();
            var near = Create("near", 5.999f, true);
            var edge = Create("edge", 6f, true);
            var far = Create("far", 6.001f, true);
            registry.ConfigureForTests(near, far, edge);
            Assert.Throws<InvalidOperationException>(() => registry.ValidateAndFreeze());
            registry.ConfigureForTests(edge, far, near);
            registry.ValidateAndFreeze();
            var root = Track(new GameObject("Player"));
            var observations = new CombatObservationBuilder().Capture(new SimulationTick(1), Player(0), Camera(0), Gamepad(1), registry.FrozenTargets,
                new Dictionary<string, bool> { ["edge"] = true, ["near"] = false, ["far"] = true }, root.transform);
            Assert.That(observations[0].PlayerDistanceKey, Is.EqualTo(36000));
            Assert.That(observations[1].PlayerDistanceKey, Is.EqualTo(36012));
            Assert.That(observations[2].PlayerDistanceKey, Is.EqualTo(35988));
            Assert.That(observations[2].IsAttackable, Is.False, "Combat-owned alive snapshot excludes a dead target without Transfer state.");
        }

        // AC-UX-007, AC-UX-012. Exact M2A accepts the delegated keys, preserving mouse 23/24/25 and gamepad angular boundaries.
        [Test]
        public void CaptureUsesSharedProjectionShapeAndAngleKeysWithoutFloatRecomputation()
        {
            var registry = Track(new GameObject("Registry")).AddComponent<CombatTargetRegistry>();
            var target = Create("target", 0f, true, 0, 0, 1000, 1000);
            registry.ConfigureForTests(target); registry.ValidateAndFreeze();
            var root = Track(new GameObject("Player"));
            var builder = new CombatObservationBuilder();
            var mouse = new AimSample(1, AimSource.MousePointer, 983, 540, true, 4096, 0, new SimulationTick(0), new SimulationTick(1));
            var observation = builder.Capture(new SimulationTick(1), Player(0), Camera(0), mouse, registry.FrozenTargets, Alive("target"), root.transform)[0];
            Assert.That(observation.ScreenDistanceSquaredKey, Is.EqualTo(529));
            mouse = new AimSample(2, AimSource.MousePointer, 984, 540, true, 4096, 0, new SimulationTick(0), new SimulationTick(1));
            observation = builder.Capture(new SimulationTick(1), Player(0), Camera(0), mouse, registry.FrozenTargets, Alive("target"), root.transform)[0];
            Assert.That(observation.ScreenDistanceSquaredKey, Is.EqualTo(576));
            mouse = new AimSample(3, AimSource.MousePointer, 985, 540, true, 4096, 0, new SimulationTick(0), new SimulationTick(1));
            observation = builder.Capture(new SimulationTick(1), Player(0), Camera(0), mouse, registry.FrozenTargets, Alive("target"), root.transform)[0];
            Assert.That(observation.ScreenDistanceSquaredKey, Is.EqualTo(625));
            Assert.That(observation.MouseInsideShape, Is.True);
            var angledRegistry = Track(new GameObject("AngledRegistry")).AddComponent<CombatTargetRegistry>();
            var angled = Create("angled", 4.096f, true, 0, 1331, 1000, 1000);
            angledRegistry.ConfigureForTests(angled); angledRegistry.ValidateAndFreeze();
            var gamepad = new AimSample(4, AimSource.GamepadStick, null, null, null, 4096, 0, new SimulationTick(0), new SimulationTick(1));
            observation = builder.Capture(new SimulationTick(1), Player(0), Camera(0), gamepad, angledRegistry.FrozenTargets, Alive("angled"), root.transform)[0];
            Assert.That(observation.GamepadAngleKey, Is.EqualTo(180));

            var boundaryRegistry = Track(new GameObject("BoundaryRegistry")).AddComponent<CombatTargetRegistry>();
            var acquire179 = Create("a179", 4.096f, true, 0, 1324); var acquire181 = Create("a181", 4.096f, true, 0, 1339);
            var retain259 = Create("r259", 4.096f, true, 0, 1991); var retain260 = Create("r260", 4.096f, true, 0, 1998); var retain261 = Create("r261", 4.096f, true, 0, 2006);
            boundaryRegistry.ConfigureForTests(acquire179, acquire181, retain259, retain260, retain261); boundaryRegistry.ValidateAndFreeze();
            var boundary = builder.Capture(new SimulationTick(1), Player(0), Camera(0), gamepad, boundaryRegistry.FrozenTargets,
                new Dictionary<string, bool> { ["a179"] = true, ["a181"] = true, ["r259"] = true, ["r260"] = true, ["r261"] = true }, root.transform);
            Assert.That(boundary[0].GamepadAngleKey, Is.EqualTo(179)); Assert.That(boundary[1].GamepadAngleKey, Is.EqualTo(181));
            Assert.That(boundary[2].GamepadAngleKey, Is.EqualTo(259)); Assert.That(boundary[3].GamepadAngleKey, Is.EqualTo(260)); Assert.That(boundary[4].GamepadAngleKey, Is.EqualTo(261));
        }

        // AC-COM-003. Checked conversion failure occurs before this call can publish a result collection.
        [Test]
        public void CaptureRejectsWrongTickOrOverflowBeforePublication()
        {
            var root = Track(new GameObject("Player"));
            IReadOnlyList<BasicAttackCandidateObservation> published = null;
            var builder = new CombatObservationBuilder();
            Assert.Throws<ArgumentException>(() => published = builder.Capture(new SimulationTick(1), Player(1), Camera(0), Gamepad(1), Array.Empty<FrozenCombatTarget>(), new Dictionary<string, bool>(), root.transform));
            var overflowing = new SimulationCameraPoseSnapshot(new SimulationTick(0), int.MaxValue, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
            Assert.Throws<OverflowException>(() => published = builder.Capture(new SimulationTick(1), Player(0), overflowing, new AimSample(1, AimSource.MousePointer, 1919, 540, true, 4096, 0, new SimulationTick(0), new SimulationTick(1)), Array.Empty<FrozenCombatTarget>(), new Dictionary<string, bool>(), root.transform));
            Assert.That(published, Is.Null);
        }

        private CombatTarget Create(string id, float x, bool attackable, int localX = 0, int localY = 0, int halfX = 100, int halfY = 100)
        {
            var go = Track(new GameObject(id)); go.transform.position = new Vector3(x, 0f, 9f);
            var collider = go.AddComponent<BoxCollider2D>(); var target = go.AddComponent<CombatTarget>();
            target.ConfigureForTests(id, 9, 0, attackable, localX, localY, localX, localY, halfX, halfY, collider, go.transform);
            return target;
        }
        private GameObject Track(GameObject value) { _objects.Add(value); return value; }
        private static IReadOnlyDictionary<string, bool> Alive(string id) => new Dictionary<string, bool> { [id] = true };
        private static PlayerMotionSnapshot Player(int tick) => new PlayerMotionSnapshot(new SimulationTick(tick), 0, 0, 0, 0, false, string.Empty, WallSide.None, MovementActionState.Normal, true);
        private static SimulationCameraPoseSnapshot Camera(int tick) => new SimulationCameraPoseSnapshot(new SimulationTick(tick), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
        private static AimSample Gamepad(int tick) => new AimSample(1, AimSource.GamepadStick, null, null, null, 4096, 0, new SimulationTick(tick - 1), new SimulationTick(tick));
    }
}
