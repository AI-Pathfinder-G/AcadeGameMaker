using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-001, REQ-COM-004, REQ-WT-006, REQ-UX-007, REQ-UX-008. Unity pose capture translates only into M2A's internal observations.
    internal sealed class CombatObservationBuilder
    {
        internal IReadOnlyList<BasicAttackCandidateObservation> Capture(
            SimulationTick tick, PlayerMotionSnapshot completedPlayer, SimulationCameraPoseSnapshot camera, AimSample sample,
            IReadOnlyList<FrozenCombatTarget> frozenTargets, IReadOnlyDictionary<string, bool> attackabilityBeforeTick, Transform playerRoot)
        {
            if (frozenTargets == null) throw new ArgumentNullException(nameof(frozenTargets));
            if (attackabilityBeforeTick == null) throw new ArgumentNullException(nameof(attackabilityBeforeTick));
            if (playerRoot == null) throw new ArgumentNullException(nameof(playerRoot));
            ValidateTickInputs(tick, completedPlayer, camera, sample);

            var playerXQ1000 = Q4096ToQ1000(completedPlayer.PositionXQ4096);
            var playerYQ1000 = Q4096ToQ1000(completedPlayer.PositionYQ4096);
            var pointer = sample.Source == AimSource.MousePointer
                ? TransferFixedMath.InverseGridToWorld(camera, sample.ScreenPixelX.Value, sample.ScreenPixelY.Value)
                : (xQ1000: 0, yQ1000: 0);
            var ids = new HashSet<string>(StringComparer.Ordinal);
            var staged = new List<BasicAttackCandidateObservation>(frozenTargets.Count);
            for (var index = 0; index < frozenTargets.Count; index++)
            {
                var target = frozenTargets[index];
                if (!ids.Add(target.TargetId)) throw new ArgumentException("Frozen combat registry contains duplicate TargetId.", nameof(frozenTargets));
                if (!attackabilityBeforeTick.TryGetValue(target.TargetId, out var isAttackable)) throw new ArgumentException("Combat attackability snapshot must cover every frozen TargetId.", nameof(attackabilityBeforeTick));
                // The player is a registered M1 combatant but is never an M2A attack candidate (REQ-COM-004).
                if (string.Equals(target.TargetId, "player", StringComparison.Ordinal)) continue;
                var pose = target.TargetPoseRoot.position;
                var poseXQ1000 = QuantizePoseQ1000(pose.x);
                var poseYQ1000 = QuantizePoseQ1000(pose.y);
                var aimXQ1000 = checked(poseXQ1000 + target.LocalAimPointXQ1000);
                var aimYQ1000 = checked(poseYQ1000 + target.LocalAimPointYQ1000);
                var shapeXQ1000 = checked(poseXQ1000 + target.LocalAimShapeCenterXQ1000);
                var shapeYQ1000 = checked(poseYQ1000 + target.LocalAimShapeCenterYQ1000);
                var projected = TransferFixedMath.ProjectWorldToGrid(camera, aimXQ1000, aimYQ1000);
                var mouseInside = sample.Source == AimSource.MousePointer && TransferFixedMath.IsInsideAxisAlignedEllipse(pointer.xQ1000, pointer.yQ1000, shapeXQ1000, shapeYQ1000, target.LocalAimShapeHalfExtentXQ1000, target.LocalAimShapeHalfExtentYQ1000);
                var screenDistance = sample.Source == AimSource.MousePointer ? TransferFixedMath.ScreenDistanceSquaredKey(sample.ScreenPixelX.Value, sample.ScreenPixelY.Value, projected.x, projected.y) : 0;
                var angle = sample.Source == AimSource.GamepadStick ? TransferFixedMath.GamepadAngleKey(sample.AimVectorXQ4096, sample.AimVectorYQ4096, checked(aimXQ1000 - playerXQ1000), checked(aimYQ1000 - playerYQ1000)) : 0;
                var los = TransferLineOfSight.Evaluate(playerXQ1000, playerYQ1000, aimXQ1000, aimYQ1000, playerRoot, target.TargetPoseRoot, target.TargetId, tick);
                staged.Add(new BasicAttackCandidateObservation(target.TargetId, tick, target.CanReceivePlayerBasicAttack && isAttackable, los.IsOpen,
                    TransferSelector.PlayerDistanceKey(playerXQ1000, playerYQ1000, aimXQ1000, aimYQ1000), mouseInside, screenDistance, angle));
            }

            return staged.ToArray();
        }

        internal static int Q4096ToQ1000(int valueQ4096) => checked((int)TransferFixedMath.RoundDivAway(checked((long)valueQ4096 * 1000L), 4096L));
        internal static int QuantizePoseQ1000(float value) => float.IsNaN(value) || float.IsInfinity(value)
            ? throw new ArgumentOutOfRangeException(nameof(value))
            : checked((int)Math.Round(checked((double)value * 1000d), MidpointRounding.AwayFromZero));

        private static void ValidateTickInputs(SimulationTick tick, PlayerMotionSnapshot player, SimulationCameraPoseSnapshot camera, AimSample sample)
        {
            var completedTick = new SimulationTick(checked(tick.Value - 1));
            if (player.Tick != completedTick || camera.CameraPoseTick != completedTick || sample.CameraPoseTick != completedTick || sample.SampleTick != tick)
            {
                throw new ArgumentException("Combat observation capture requires completed t-1 poses and an exact t aim sample.");
            }
            TransferFixedMath.ValidateVerticalDemoCamera(camera);
        }
    }
}
