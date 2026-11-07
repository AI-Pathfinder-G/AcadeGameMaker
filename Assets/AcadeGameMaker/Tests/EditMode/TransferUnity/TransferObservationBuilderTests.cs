// AC-WT-003, AC-WT-006. M2B2A validates fixed-tick capture boundaries before any observation is published.
using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.TransferUnity
{
    internal sealed class TransferObservationBuilderTests
    {
        // AC-WT-003, AC-WT-006. Capture accepts only completed t-1 player/camera poses and exact t sample.
        [Test]
        public void CaptureRequiresCompletedPriorTickBeforeOutputPublication()
        {
            var builder = new TransferObservationBuilder();
            var root = new GameObject("ObservationBuilderRoot");
            try
            {
                var result = builder.Capture(new SimulationTick(1), Snapshot(0), Camera(0), Mouse(1), Array.Empty<TransferTarget>(), Array.Empty<string>(), root.transform);
                Assert.That(result, Is.Empty);

                Assert.Throws<ArgumentException>(() => builder.Capture(new SimulationTick(1), Snapshot(1), Camera(0), Mouse(1), Array.Empty<TransferTarget>(), Array.Empty<string>(), root.transform));
                Assert.Throws<ArgumentException>(() => builder.Capture(new SimulationTick(1), Snapshot(0), Camera(1), Mouse(1), Array.Empty<TransferTarget>(), Array.Empty<string>(), root.transform));
                var unsupportedCamera = new SimulationCameraPoseSnapshot(new SimulationTick(0), 0, 0, 10001, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
                Assert.Throws<ArgumentException>(() => builder.Capture(new SimulationTick(1), Snapshot(0), unsupportedCamera, Mouse(1), Array.Empty<TransferTarget>(), Array.Empty<string>(), root.transform));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        // AC-WT-003. Target pose coordinates quantize independently once with the frozen AwayFromZero rule.
        [Test]
        public void TargetPoseQuantizationUsesAwayFromZeroAtSignedHalfUnits()
        {
            Assert.That(TransferObservationBuilder.QuantizePoseQ1000(0.0005f), Is.EqualTo(1));
            Assert.That(TransferObservationBuilder.QuantizePoseQ1000(-0.0005f), Is.EqualTo(-1));
        }

        // AC-WT-003. A checked inverse-projection overflow fails before the local publication sentinel changes.
        [Test]
        public void CaptureOverflowDoesNotPublishObservationSentinel()
        {
            var builder = new TransferObservationBuilder();
            var root = new GameObject("ObservationBuilderOverflowRoot");
            IReadOnlyList<AcadeGameMaker.Transfer.TransferCandidateObservation> published = null;
            try
            {
                var overflowingCamera = new SimulationCameraPoseSnapshot(new SimulationTick(0), int.MaxValue, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
                Assert.Throws<OverflowException>(() => published = builder.Capture(new SimulationTick(1), Snapshot(0), overflowingCamera, Mouse(1, 1919, 540), Array.Empty<TransferTarget>(), Array.Empty<string>(), root.transform));
                Assert.That(published, Is.Null);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static PlayerMotionSnapshot Snapshot(int tick) => new PlayerMotionSnapshot(new SimulationTick(tick), 0, 0, 0, 0, false, string.Empty, WallSide.None, MovementActionState.Normal, true);
        private static SimulationCameraPoseSnapshot Camera(int tick) => new SimulationCameraPoseSnapshot(new SimulationTick(tick), 0, 0, 10000, 0, 1920, 1080, 0, 0, 1920, 1080, 3);
        private static AimSample Mouse(int tick, int x = 960, int y = 540) => new AimSample(1, AimSource.MousePointer, x, y, true, 4096, 0, new SimulationTick(tick - 1), new SimulationTick(tick));
    }
}
