using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using UnityEngine;

namespace AcadeGameMaker.Transfer.Unity
{
    // REQ-WT-001, REQ-WT-003, REQ-WT-004, REQ-WT-006, REQ-WT-008.
    // The caller owns the one capture-phase SyncTransforms; this builder reads only that captured physics/transform state.
    internal sealed class TransferObservationBuilder
    {
        internal IReadOnlyList<TransferCandidateObservation> Capture(
            SimulationTick tick,
            PlayerMotionSnapshot completedPlayer,
            SimulationCameraPoseSnapshot camera,
            AimSample sample,
            IReadOnlyList<TransferTarget> sortedTargets,
            IReadOnlyList<string> removedTargetIds,
            Transform playerRoot)
        {
            if (sortedTargets == null) throw new ArgumentNullException(nameof(sortedTargets));
            if (removedTargetIds == null) throw new ArgumentNullException(nameof(removedTargetIds));
            if (playerRoot == null) throw new ArgumentNullException(nameof(playerRoot));

            ValidateTickInputs(tick, completedPlayer, camera, sample);
            ValidateRemovedIds(removedTargetIds);
            var playerXQ1000 = Q4096ToQ1000(completedPlayer.PositionXQ4096);
            var playerYQ1000 = Q4096ToQ1000(completedPlayer.PositionYQ4096);
            var pointer = sample.Source == AimSource.MousePointer
                ? TransferFixedMath.InverseGridToWorld(camera, sample.ScreenPixelX.Value, sample.ScreenPixelY.Value)
                : (xQ1000: 0, yQ1000: 0);

            // Publication is deliberately deferred until every checked conversion, descriptor addition, and LOS query succeeds.
            var observations = new List<TransferCandidateObservation>(sortedTargets.Count);
            for (var index = 0; index < sortedTargets.Count; index++)
            {
                var target = sortedTargets[index] ?? throw new ArgumentException("Transfer target registry cannot contain null entries.", nameof(sortedTargets));
                if (ContainsRemovedId(removedTargetIds, target.TargetId))
                {
                    continue;
                }

                var descriptor = target.BuildDescriptorFromValidatedAuthoring();
                var pose = target.TargetPoseRoot.position;
                var poseXQ1000 = QuantizePoseQ1000(pose.x);
                var poseYQ1000 = QuantizePoseQ1000(pose.y);
                var worldAimPointXQ1000 = checked(poseXQ1000 + descriptor.AimPointXQ1000);
                var worldAimPointYQ1000 = checked(poseYQ1000 + descriptor.AimPointYQ1000);
                var worldAimShapeCenterXQ1000 = checked(poseXQ1000 + descriptor.AimShapeCenterXQ1000);
                var worldAimShapeCenterYQ1000 = checked(poseYQ1000 + descriptor.AimShapeCenterYQ1000);
                var projectedAim = TransferFixedMath.ProjectWorldToGrid(camera, worldAimPointXQ1000, worldAimPointYQ1000);
                var mouseInsideShape = sample.Source == AimSource.MousePointer &&
                    TransferFixedMath.IsInsideAxisAlignedEllipse(
                        pointer.xQ1000,
                        pointer.yQ1000,
                        worldAimShapeCenterXQ1000,
                        worldAimShapeCenterYQ1000,
                        descriptor.AimShapeHalfExtentXQ1000,
                        descriptor.AimShapeHalfExtentYQ1000);
                var screenDistanceSquaredKey = sample.Source == AimSource.MousePointer
                    ? TransferFixedMath.ScreenDistanceSquaredKey(sample.ScreenPixelX.Value, sample.ScreenPixelY.Value, projectedAim.x, projectedAim.y)
                    : 0;
                var gamepadAngleKey = sample.Source == AimSource.GamepadStick
                    ? TransferFixedMath.GamepadAngleKey(sample.AimVectorXQ4096, sample.AimVectorYQ4096, checked(worldAimPointXQ1000 - playerXQ1000), checked(worldAimPointYQ1000 - playerYQ1000))
                    : 0;
                var lineOfSight = TransferLineOfSight.Evaluate(
                    playerXQ1000,
                    playerYQ1000,
                    worldAimPointXQ1000,
                    worldAimPointYQ1000,
                    playerRoot,
                    target.TargetPoseRoot,
                    descriptor.TargetId,
                    tick);

                observations.Add(new TransferCandidateObservation(
                    descriptor,
                    tick,
                    worldAimPointXQ1000,
                    worldAimPointYQ1000,
                    worldAimShapeCenterXQ1000,
                    worldAimShapeCenterYQ1000,
                    descriptor.AimShapeHalfExtentXQ1000,
                    descriptor.AimShapeHalfExtentYQ1000,
                    target.TargetCollider.isTrigger,
                    lineOfSight.IsOpen,
                    TransferSelector.PlayerDistanceKey(playerXQ1000, playerYQ1000, worldAimPointXQ1000, worldAimPointYQ1000),
                    mouseInsideShape,
                    screenDistanceSquaredKey,
                    gamepadAngleKey));
            }

            return observations.ToArray();
        }

        internal static int Q4096ToQ1000(int valueQ4096) => checked((int)TransferFixedMath.RoundDivAway(checked((long)valueQ4096 * 1000L), 4096L));

        internal static int QuantizePoseQ1000(float worldCoordinate)
        {
            if (float.IsNaN(worldCoordinate) || float.IsInfinity(worldCoordinate))
            {
                throw new ArgumentOutOfRangeException(nameof(worldCoordinate));
            }

            return checked((int)Math.Round(checked((double)worldCoordinate * 1000d), MidpointRounding.AwayFromZero));
        }

        private static void ValidateTickInputs(SimulationTick tick, PlayerMotionSnapshot completedPlayer, SimulationCameraPoseSnapshot camera, AimSample sample)
        {
            var completedTick = new SimulationTick(checked(tick.Value - 1));
            if (completedPlayer.Tick != completedTick || camera.CameraPoseTick != completedTick || sample.CameraPoseTick != completedTick || sample.SampleTick != tick)
            {
                throw new ArgumentException("Transfer observation capture requires completed t-1 poses and an exact t aim sample.");
            }

            TransferFixedMath.ValidateVerticalDemoCamera(camera);
        }

        private static void ValidateRemovedIds(IReadOnlyList<string> removedTargetIds)
        {
            for (var index = 0; index < removedTargetIds.Count; index++)
            {
                TransferValidation.TargetId(removedTargetIds[index]);
            }
        }

        private static bool ContainsRemovedId(IReadOnlyList<string> removedTargetIds, string targetId)
        {
            for (var index = 0; index < removedTargetIds.Count; index++)
            {
                if (string.Equals(removedTargetIds[index], targetId, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
