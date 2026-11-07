// AC-WT-003, AC-WT-006. M2A integer-adapter evidence; transfer, lifecycle, and full LOS acceptance remain deferred.
using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.TransferUnity
{
    internal sealed class TransferFixedMathTests
    {
        // AC-WT-006. Signed exact halves round away from zero without a doubled-remainder overflow.
        [Test]
        public void RoundDivAwayPinsSignedHalfAndNearHalfBoundaries()
        {
            Assert.That(TransferFixedMath.RoundDivAway(8, 2), Is.EqualTo(4));
            Assert.That(TransferFixedMath.RoundDivAway(9, 2), Is.EqualTo(5));
            Assert.That(TransferFixedMath.RoundDivAway(-9, 2), Is.EqualTo(-5));
            Assert.That(TransferFixedMath.RoundDivAway(10, 3), Is.EqualTo(3));
            Assert.That(TransferFixedMath.RoundDivAway(11, 3), Is.EqualTo(4));
            Assert.That(TransferFixedMath.RoundDivAway(-10, 4), Is.EqualTo(-3));
            Assert.That(TransferFixedMath.RoundDivAway(long.MaxValue, long.MaxValue), Is.EqualTo(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => TransferFixedMath.RoundDivAway(1, 0));
        }

        // AC-WT-006. The fixed orthographic camera has endpoint-sampled grid boundaries.
        [Test]
        public void ProjectionAndInversePinCenterAndEndpointGrid()
        {
            var camera = Camera(0, 0);
            Assert.That(TransferFixedMath.ProjectWorldToGrid(camera, 0, 0), Is.EqualTo((960, 540)));
            Assert.That(TransferFixedMath.InverseGridToWorld(camera, 0, 0), Is.EqualTo((-17778, -10000)));
            Assert.That(TransferFixedMath.InverseGridToWorld(camera, 1919, 1079), Is.EqualTo((17778, 10000)));
            Assert.That(TransferFixedMath.ProjectWorldToGrid(camera, -17778, -10000), Is.EqualTo((0, 0)));
            Assert.That(TransferFixedMath.ProjectWorldToGrid(camera, 17778, 10000), Is.EqualTo((1919, 1079)));
            Assert.That(TransferFixedMath.InverseGridToWorld(Camera(123, -456), 960, 540), Is.EqualTo((1239, -4551)));
        }

        // AC-WT-006. Camera pose outside the frozen vertical-demo profile fails before an observation can be constructed.
        [Test]
        public void UnsupportedCameraProfileAndGridInputFailFast()
        {
            Assert.Throws<ArgumentException>(() => TransferFixedMath.ProjectWorldToGrid(Camera(0, 0, 9999), 0, 0));
            Assert.Throws<ArgumentException>(() => TransferFixedMath.ProjectWorldToGrid(Camera(0, 0, 10000, 1), 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => TransferFixedMath.InverseGridToWorld(Camera(0, 0), 1920, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => TransferFixedMath.InverseGridToWorld(Camera(0, 0), 0, -1));
        }

        // AC-WT-006. The selection key is exact squared integer screen distance, with no square root.
        [Test]
        public void ScreenDistanceAndEllipseBoundariesAreExact()
        {
            Assert.That(TransferFixedMath.ScreenDistanceSquaredKey(960, 540, 960, 540), Is.EqualTo(0));
            Assert.That(TransferFixedMath.ScreenDistanceSquaredKey(963, 536, 960, 540), Is.EqualTo(25));
            Assert.That(TransferFixedMath.IsInsideAxisAlignedEllipse(1000, 0, 0, 0, 1000, 2000), Is.True);
            Assert.That(TransferFixedMath.IsInsideAxisAlignedEllipse(1001, 0, 0, 0, 1000, 2000), Is.False);
            Assert.That(TransferFixedMath.IsInsideAxisAlignedEllipse(0, 2000, 0, 0, 1000, 2000), Is.True);
            Assert.That(TransferFixedMath.IsInsideAxisAlignedEllipse(0, 2001, 0, 0, 1000, 2000), Is.False);
        }

        // AC-WT-006. 0.0025 inclusive mouse direction and Q4096 component rounding are deterministic.
        [Test]
        public void MouseThresholdAndQ4096NormalizationAreExact()
        {
            Assert.That(TransferFixedMath.HasMouseDirection(49, 0), Is.False);
            Assert.That(TransferFixedMath.HasMouseDirection(50, 0), Is.True);
            Assert.That(TransferFixedMath.NormalizeMouseDirectionQ4096(50, 0), Is.EqualTo((4096, 0)));
            Assert.That(TransferFixedMath.NormalizeMouseDirectionQ4096(30, 40), Is.EqualTo((2458, 3277)));
            Assert.That(TransferFixedMath.NormalizeMouseDirectionQ4096(-30, 40), Is.EqualTo((-2458, 3277)));
            Assert.Throws<ArgumentOutOfRangeException>(() => TransferFixedMath.NormalizeMouseDirectionQ4096(49, 0));
        }

        // AC-WT-006. The frozen 24-step CORDIC table produces selector tenths-degree boundaries.
        [Test]
        public void CordicPinsCardinalAndSelectionBoundaries()
        {
            Assert.That(TransferFixedMath.GamepadAngleKey(4096, 0, 0, 0), Is.EqualTo(0));
            Assert.That(TransferFixedMath.GamepadAngleKey(4096, 0, 4096, 0), Is.EqualTo(0));
            Assert.That(TransferFixedMath.GamepadAngleKey(4096, 0, 0, 4096), Is.EqualTo(900));
            Assert.That(TransferFixedMath.GamepadAngleKey(4096, 0, -4096, 0), Is.EqualTo(1800));
            Assert.That(TransferFixedMath.GamepadAngleKey(4096, 0, 4096, 1324), Is.EqualTo(179));
            Assert.That(TransferFixedMath.GamepadAngleKey(4096, 0, 4096, 1331), Is.EqualTo(180));
            Assert.That(TransferFixedMath.GamepadAngleKey(4096, 0, 4096, 1339), Is.EqualTo(181));
            Assert.That(TransferFixedMath.GamepadAngleKey(4096, 0, 4096, 1991), Is.EqualTo(259));
            Assert.That(TransferFixedMath.GamepadAngleKey(4096, 0, 4096, 1998), Is.EqualTo(260));
            Assert.That(TransferFixedMath.GamepadAngleKey(4096, 0, 4096, 2006), Is.EqualTo(261));
        }

        // AC-WT-003. Projection's public int inputs remain within Int64; inverse conversion back to Q1000 fails checked.
        [Test]
        public void ProjectionPublicBoundaryIsSafeAndInverseQ1000ConversionOverflows()
        {
            var extremeCamera = Camera(int.MaxValue, int.MinValue);
            Assert.DoesNotThrow(() => TransferFixedMath.ProjectWorldToGrid(extremeCamera, int.MinValue, int.MaxValue));
            Assert.Throws<OverflowException>(() => TransferFixedMath.InverseGridToWorld(extremeCamera, 960, 540));
        }

        // AC-WT-003. A distance-key overflow occurs before a hypothetical observation/session publication mutates either sentinel.
        [Test]
        public void ScreenDistanceOverflowPrecedesObservationAndSessionMutation()
        {
            var sentinel = new ObservationSessionSentinel();
            Assert.Throws<OverflowException>(() => sentinel.ConstructObservationThenPublish(
                () => TransferFixedMath.ScreenDistanceSquaredKey(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue)));
            Assert.That(sentinel.ObservationConstructed, Is.False);
            Assert.That(sentinel.SessionRevision, Is.EqualTo(0));
        }

        // AC-WT-003. Both mouse squared-length paths throw before the same observation/session sentinel can mutate.
        [Test]
        public void MouseDistanceAndNormalizationOverflowPrecedeMutation()
        {
            var thresholdSentinel = new ObservationSessionSentinel();
            Assert.Throws<OverflowException>(() => thresholdSentinel.ConstructObservationThenPublish(
                () => TransferFixedMath.HasMouseDirection(int.MinValue, int.MinValue) ? 1 : 0));
            Assert.That(thresholdSentinel.ObservationConstructed, Is.False);
            Assert.That(thresholdSentinel.SessionRevision, Is.EqualTo(0));

            var normalizationSentinel = new ObservationSessionSentinel();
            Assert.Throws<OverflowException>(() => normalizationSentinel.ConstructObservationThenPublish(
                () => TransferFixedMath.NormalizeMouseDirectionQ4096(int.MinValue, int.MinValue).xQ4096));
            Assert.That(normalizationSentinel.ObservationConstructed, Is.False);
            Assert.That(normalizationSentinel.SessionRevision, Is.EqualTo(0));
        }

        private static SimulationCameraPoseSnapshot Camera(int xQ100, int yQ100, int orthoSizeQ1000 = 10000, int rotationQ10 = 0) =>
            new SimulationCameraPoseSnapshot(
                new SimulationTick(0), xQ100, yQ100, orthoSizeQ1000, rotationQ10,
                2560, 1440, 0, 0, 2560, 1440, 4);

        private sealed class ObservationSessionSentinel
        {
            internal bool ObservationConstructed { get; private set; }
            internal int SessionRevision { get; private set; }

            internal void ConstructObservationThenPublish(Func<int> keyFactory)
            {
                var key = keyFactory();
                if (key < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(key));
                }

                ObservationConstructed = true;
                SessionRevision++;
            }
        }
    }
}
