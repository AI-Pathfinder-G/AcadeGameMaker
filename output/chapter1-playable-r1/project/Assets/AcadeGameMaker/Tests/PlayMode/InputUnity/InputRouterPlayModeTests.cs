using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-UX-004/006/007/008/013; AC-M5B5-001/002/003/007/008.
    // Initial checkpoint: authored graph bootstrap and one real Keyboard event.
    // This fixture does not claim camera publication, terminal retirement, or
    // 30/60/144 cadence equivalence until those paths have independent tests.
    internal sealed class InputRouterPlayModeTests : InputTestFixture
    {
        [UnityEngine.SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _runtimeFixture=new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnityEngine.TestTools.UnitySetUp] public System.Collections.IEnumerator BeginRuntimeCase(){_runtimeFixture.PrepareEnter();yield return new UnityEngine.TestTools.EnterPlayMode();_runtimeFixture.ConfirmEntered();yield return _runtimeFixture.RunBody(_runtimeFixture.BeginCase(false));}
        public override void TearDown(){}
        [UnityEngine.TestTools.UnityTearDown] public System.Collections.IEnumerator CloseRuntimeCase(){try{if(_runtimeFixture.OwnsRuntimeFixture && UnityEngine.Application.isPlaying)yield return _runtimeFixture.RunBody(_runtimeFixture.CloseCase(),true);}finally{_runtimeFixture.CaptureCleanupFailure(()=>base.TearDown());}if(_runtimeFixture.OwnsRuntimeFixture){if(UnityEngine.Application.isPlaying)yield return new UnityEngine.TestTools.ExitPlayMode();_runtimeFixture.CaptureCleanupFailure(_runtimeFixture.CompleteExit);}_runtimeFixture.ThrowCleanupFailure();}
        

        [Test]
        public void GameplayBootstrapPublishesOneReceiptAndExclusiveMap()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled);
                router.StepForTests();

                Assert.That(router.IsFaulted, Is.False);
                Assert.That(router.CurrentReceipt.HasValue, Is.True);
                Assert.That(router.CurrentReceipt.Value.Tick, Is.EqualTo(new SimulationTick(0)));
                Assert.That(router.CurrentReceipt.Value.FrameOrdinal, Is.EqualTo(0));
                Assert.That(router.CurrentReceipt.Value.ModeEpoch, Is.EqualTo(0));
                Assert.That(router.CurrentReceipt.Value.Mode, Is.EqualTo(InputMode.GameplayEnabled));
                Assert.That(rig.Player.IsInputGateLocked, Is.False);
                Assert.That(rig.Transfer.IsInputGateLocked, Is.False);
                Assert.That(rig.Combat.IsInputGateLocked, Is.False);

                var actions = GetPrivate<GameInputActions>(router, "_actions");
                Assert.That(actions.Gameplay.enabled, Is.True);
                Assert.That(actions.UI.enabled, Is.False);
            }
            finally
            {
                DestroyAll(objects);
            }
        }

        [Test]
        public void KeyboardMoveIsBufferedAndConsumedOnTheNextFixedTick()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var router = CreateRouter(objects, rig, InputMode.GameplayEnabled);
                router.StepForTests();
                AdvanceFrame(rig);
                var before = rig.Player.Snapshot;
                var keyboard = InputSystem.AddDevice<Keyboard>();

                Press(keyboard.dKey);
                Assert.That(GetPrivate<Vector2>(router, "_move").x, Is.EqualTo(1f));
                Assert.That(rig.Player.Snapshot.PositionXQ4096, Is.EqualTo(before.PositionXQ4096),
                    "Device callbacks must buffer, not move the player.");
                router.StepForTests();
                AdvanceFrame(rig);

                Assert.That(router.IsFaulted, Is.False);
                Assert.That(router.CurrentReceipt.HasValue, Is.True);
                Assert.That(router.CurrentReceipt.Value.Tick, Is.EqualTo(new SimulationTick(1)));
                Assert.That(router.CurrentReceipt.Value.FrameOrdinal, Is.EqualTo(1));
                Assert.That(rig.Player.Snapshot.Tick, Is.EqualTo(new SimulationTick(1)));
                Assert.That(rig.Player.Snapshot.PositionXQ4096, Is.GreaterThan(before.PositionXQ4096),
                    "The generated Gameplay/Move callback must reach Movement at the next fixed tick.");
                Release(keyboard.dKey);
            }
            finally
            {
                DestroyAll(objects);
            }
        }

        // REQ-UX-004/008; AC-M5B5-007/008. The global receipt is staged in
        // each consumer before publication, then all three phases reject it
        // without consuming local state until the exact source receipt exists.
        [Test]
        public void CoordinatedReceiptIsRequiredBeforeAnyConsumerPhaseAndThenConsumedOnce()
        {
            var objects = new List<GameObject>();
            try
            {
                var rig = CreateRig(objects);
                var source = new MutableCommitSource();
                var foreign = new MutableCommitSource();
                var tick = new SimulationTick(0);

                Assert.Throws<InvalidOperationException>(() => rig.Player.ValidateCoordinatedInputRegistration(source, foreign, tick));
                RegisterCoordinated(rig, source, tick);

                var receipt = new InputFrameCommitReceipt(tick, 0, 0, InputMode.Transition);
                var movementCandidate = rig.Player.PrepareInputFrame(source, tick, true, null, receipt);
                var transferCandidate = rig.Transfer.PrepareInputFrame(source, tick, true, null, receipt);
                var combatCandidate = rig.Combat.PrepareInputFrame(source, tick, true, null, receipt);
                rig.Player.CommitInputFrame(source, movementCandidate);
                rig.Transfer.CommitInputFrame(source, transferCandidate);
                rig.Combat.CommitInputFrame(source, combatCandidate);

                var playerBefore = rig.Player.Snapshot;
                var transferBefore = rig.Transfer.SnapshotForTests;
                var movementQueue = GetPrivate<object>(rig.Player, "_commands");
                var transferQueue = GetPrivate<object>(rig.Transfer, "_inputs");
                var combatQueue = GetPrivate<object>(rig.Combat, "_inputs");
                var combatAttempted = GetPrivate<bool>(rig.Combat, "_attempted");

                Assert.Throws<InvalidOperationException>(() => rig.Transfer.AdvanceCoordinatedInputPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Player.AdvanceOneFixedTickForTests());
                Assert.That(rig.Player.Snapshot.Tick, Is.EqualTo(playerBefore.Tick));
                Assert.That(rig.Player.Snapshot.PositionXQ4096, Is.EqualTo(playerBefore.PositionXQ4096));
                Assert.That(rig.Player.Snapshot.PositionYQ4096, Is.EqualTo(playerBefore.PositionYQ4096));
                Assert.That(rig.Transfer.SnapshotForTests.Tick, Is.EqualTo(transferBefore.Tick));
                Assert.That(GetPrivate<object>(rig.Player, "_commands"), Is.SameAs(movementQueue));
                Assert.That(GetPrivate<object>(rig.Transfer, "_inputs"), Is.SameAs(transferQueue));
                Assert.That(GetPrivate<object>(rig.Combat, "_inputs"), Is.SameAs(combatQueue));
                Assert.That(GetPrivate<bool>(rig.Combat, "_attempted"), Is.EqualTo(combatAttempted));

                source.CurrentReceipt = receipt;
                Assert.DoesNotThrow(() => rig.Transfer.AdvanceCoordinatedInputPhaseForTests());
                Assert.DoesNotThrow(() => rig.Combat.AdvanceCombatPhaseForTests());
                Assert.DoesNotThrow(() => rig.Player.AdvanceOneFixedTickForTests());
                Assert.That(rig.Player.NextExpectedTick, Is.EqualTo(new SimulationTick(1)));

                source.IsFaulted = true;
                Assert.Throws<InvalidOperationException>(() => rig.Transfer.AdvanceCoordinatedInputPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Combat.AdvanceCombatPhaseForTests());
                Assert.Throws<InvalidOperationException>(() => rig.Player.AdvanceOneFixedTickForTests());
            }
            finally
            {
                DestroyAll(objects);
            }
        }

        internal static InputRouter CreateRouter(ICollection<GameObject> objects, Rig rig, InputMode mode, bool initialize = true)
        {
            var cameraObject = Track(objects, "input-router-camera-provider");
            var camera = cameraObject.AddComponent<TestCameraProvider>();
            camera.Player = rig.Player;

            var routerObject = Track(objects, "input-router");
            routerObject.SetActive(false);
            var router = routerObject.AddComponent<InputRouter>();
            router.ConfigureForAuthoring(rig.Player, rig.Transfer, rig.Combat, camera, mode);
            routerObject.SetActive(true);
            if (initialize) router.InitializeForTests();
            return router;
        }

        internal static void AdvanceFrame(Rig rig)
        {
            rig.Transfer.AdvanceCoordinatedInputPhaseForTests();
            rig.Combat.AdvanceCombatPhaseForTests();
            rig.Bridge.AdvanceBridgePhaseForTests();
            rig.Player.AdvanceOneFixedTickForTests();
        }

        internal static Rig CreateRig(ICollection<GameObject> objects)
        {
            var playerObject = Track(objects, "input-router-player");
            var body = playerObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            var playerCollider = playerObject.AddComponent<BoxCollider2D>();
            var player = playerObject.AddComponent<PlayerMovementController>();

            var ordanObject = Track(objects, "input-router-ordan");
            // Keep the target outside the initial movement collision envelope.
            ordanObject.transform.position = Vector3.right * 8f;
            var ordanCollider = ordanObject.AddComponent<BoxCollider2D>();
            var ordan = ordanObject.AddComponent<CombatTarget>();
            ordan.ConfigureForTests("ordan", 60, 0, true, 0, 0, 0, 0, 100, 100, ordanCollider, ordanObject.transform);
            var playerTarget = playerObject.AddComponent<CombatTarget>();
            playerTarget.ConfigureForTests("player", 5, 45, false, 0, 0, 0, 0, 100, 100, playerCollider, playerObject.transform);

            var transferRegistry = Track(objects, "input-router-transfer-registry").AddComponent<TransferTargetRegistry>();
            transferRegistry.ConfigureForTests();
            var transfer = Track(objects, "input-router-transfer").AddComponent<TransferSimulationDriver>();
            transfer.ConfigureForTests(player, transferRegistry);
            transfer.InitializeForTests();

            var combatRegistry = Track(objects, "input-router-combat-registry").AddComponent<CombatTargetRegistry>();
            combatRegistry.ConfigureForTests(ordan, playerTarget);
            var combatObject = Track(objects, "input-router-combat");
            combatObject.SetActive(false);
            var combat = combatObject.AddComponent<OrdanBossCombatSimulationDriver>();
            combat.ConfigureForTests(player, combatRegistry, transfer, combat);
            combatObject.SetActive(true);

            var bridgeObject = Track(objects, "input-router-bridge");
            bridgeObject.SetActive(false);
            var bridge = bridgeObject.AddComponent<OrdanBossSimulationDriver>();
            bridge.ConfigureForTests(player, transfer, combat);
            bridge.InitializeForTests();
            bridgeObject.SetActive(true);
            return new Rig(player, transfer, combat, bridge);
        }

        internal static GameObject Track(ICollection<GameObject> objects, string name)
        {
            var value = new GameObject(name);
            objects.Add(value);
            return value;
        }

        internal static void DestroyAll(IReadOnlyList<GameObject> objects)
        {
            for (var index = objects.Count - 1; index >= 0; index--)
            {
                if (objects[index] != null) UnityEngine.Object.DestroyImmediate(objects[index]);
            }
        }

        internal static T GetPrivate<T>(object instance, string fieldName)
        {
            return (T)instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
        }

        private static void RegisterCoordinated(Rig rig, MutableCommitSource source, SimulationTick firstTick)
        {
            rig.Player.ValidateCoordinatedInputRegistration(source, source, firstTick);
            rig.Transfer.ValidateCoordinatedInputRegistration(source, source, firstTick);
            rig.Combat.ValidateCoordinatedInputRegistration(source, source, firstTick);
            rig.Player.RegisterCoordinatedInputOwner(source, source, firstTick);
            rig.Transfer.RegisterCoordinatedInputOwner(source, source, firstTick);
            rig.Combat.RegisterCoordinatedInputOwner(source, source, firstTick);
        }

        private sealed class TestCameraProvider : MonoBehaviour, IInputCameraSnapshotProvider
        {
            internal PlayerMovementController Player { get; set; }
            public PlayerMovementController BoundPlayer => Player;
            public bool TryGetCompletedCamera(SimulationTick expectedPoseTick, out SimulationCameraPoseSnapshot camera)
            {
                camera = default(SimulationCameraPoseSnapshot);
                return false;
            }
        }

        private sealed class MutableCommitSource : IInputFrameCommitSource
        {
            public InputFrameCommitReceipt? CurrentReceipt { get; set; }
            public bool IsFaulted { get; set; }
        }

        internal readonly struct Rig
        {
            internal Rig(PlayerMovementController player, TransferSimulationDriver transfer, OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge)
            { Player = player; Transfer = transfer; Combat = combat; Bridge = bridge; }
            internal PlayerMovementController Player { get; }
            internal TransferSimulationDriver Transfer { get; }
            internal OrdanBossCombatSimulationDriver Combat { get; }
            internal OrdanBossSimulationDriver Bridge { get; }
        }
    }
}
