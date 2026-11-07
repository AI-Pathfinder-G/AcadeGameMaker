using System;
using System.Collections.Generic;
using AcadeGameMaker.Camera;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Camera
{
    // REQ-ART-010/011, REQ-UX-013; AC-M5C1-001..005.
    // Pure arithmetic tests intentionally use PlayerMotionSnapshot values and
    // never Unity objects, render timing, or a camera Transform.
    internal sealed class CameraFollowSessionTests
    {
        [Test]
        public void SessionStartsWithoutASeedSnapshotAndUsesFirstCompletedTick()
        {
            var session = NewSession(5);

            Assert.That(session.HasLatestSnapshot, Is.False);
            Assert.That(session.NextExpectedTick, Is.EqualTo(new SimulationTick(5)));
            Assert.Throws<InvalidOperationException>(() => { var ignored = session.LatestSnapshot; });

            var result = session.Advance(Player(5));
            Assert.That(result.Tick, Is.EqualTo(new SimulationTick(5)));
            Assert.That(session.NextExpectedTick, Is.EqualTo(new SimulationTick(6)));
            Assert.That(session.HasLatestSnapshot, Is.True);
        }

        [TestCase(-8193, -54)]
        [TestCase(-8192, -54)]
        [TestCase(-8191, 0)]
        [TestCase(8191, 0)]
        [TestCase(8192, 54)]
        [TestCase(8193, 54)]
        public void HorizontalVelocityThresholdsAreInclusive(int velocity, int target)
        {
            var result = NewSession().Advance(Player(0, velocityX: velocity));

            Assert.That(result.TargetXPixel18, Is.EqualTo(target));
            Assert.That(result.TransitionAgeX, Is.EqualTo(target == 0 ? 12 : 1));
        }

        [TestCase(16383, 0)]
        [TestCase(16384, 27)]
        [TestCase(16385, 27)]
        [TestCase(-24575, 0)]
        [TestCase(-24576, -54)]
        [TestCase(-24577, -54)]
        public void VerticalVelocityThresholdsAreInclusiveAndSymmetric(int velocity, int target)
        {
            var result = NewSession().Advance(Player(0, velocityY: velocity));

            Assert.That(result.TargetYPixel18, Is.EqualTo(target));
            Assert.That(result.TransitionAgeY, Is.EqualTo(target == 0 ? 12 : 1));
        }

        [Test]
        public void HorizontalTransitionUsesAgeOneSixAndTwelveWithAwayRounding()
        {
            var session = NewSession();
            var expected = new[] { 5, 9, 14, 18, 23, 27, 32, 36, 41, 45, 50, 54 };
            for (var tick = 0; tick < expected.Length; tick++)
            {
                var result = session.Advance(Player(tick, velocityX: 8192));
                Assert.That(result.TransitionAgeX, Is.EqualTo(tick + 1));
                Assert.That(result.OffsetXPixel18, Is.EqualTo(expected[tick]));
            }
        }

        [Test]
        public void RetargetStartsFromLastCommittedOffsetAtAgeOne()
        {
            var session = NewSession();
            Assert.That(session.Advance(Player(0, velocityX: 8192)).OffsetXPixel18, Is.EqualTo(5));
            Assert.That(session.Advance(Player(1, velocityX: 8192)).OffsetXPixel18, Is.EqualTo(9));
            Assert.That(session.Advance(Player(2, velocityX: 8192)).OffsetXPixel18, Is.EqualTo(14));

            var retarget = session.Advance(Player(3, velocityX: -8192));
            Assert.That(retarget.TargetXPixel18, Is.EqualTo(-54));
            Assert.That(retarget.TransitionAgeX, Is.EqualTo(1));
            Assert.That(retarget.OffsetXPixel18, Is.EqualTo(8));

            CameraFollowSnapshot final = retarget;
            for (var tick = 4; tick <= 14; tick++) final = session.Advance(Player(tick, velocityX: -8192));
            Assert.That(final.TransitionAgeX, Is.EqualTo(12));
            Assert.That(final.OffsetXPixel18, Is.EqualTo(-54));
        }

        [Test]
        public void VerticalTransitionUsesSymmetricAwayRounding()
        {
            var up = NewSession();
            var upExpected = new[] { 2, 5, 7, 9, 11, 14, 16, 18, 20, 23, 25, 27 };
            for (var tick = 0; tick < upExpected.Length; tick++)
                Assert.That(up.Advance(Player(tick, velocityY: 16384)).OffsetYPixel18, Is.EqualTo(upExpected[tick]));

            var down = NewSession();
            var downExpected = new[] { -5, -9, -14, -18, -23, -27, -32, -36, -41, -45, -50, -54 };
            for (var tick = 0; tick < downExpected.Length; tick++)
                Assert.That(down.Advance(Player(tick, velocityY: -24576)).OffsetYPixel18, Is.EqualTo(downExpected[tick]));
        }

        [TestCase(12288, 0)]
        [TestCase(-12288, 0)]
        [TestCase(8192, 0)]
        [TestCase(-8192, 0)]
        public void InclusiveDeadZoneEdgesPreserveCenter(int position, int expected)
        {
            var result = NewSession().Advance(Player(0, positionX: position, positionY: position == 12288 || position == -12288 ? 0 : position));

            if (Math.Abs(position) == 12288) Assert.That(result.CenterXPixel18, Is.EqualTo(expected));
            else Assert.That(result.CenterYPixel18, Is.EqualTo(expected));
        }

        [TestCase(113, 0)]
        [TestCase(114, 1)]
        [TestCase(-113, 0)]
        [TestCase(-114, -1)]
        public void FractionalDeadZoneCrossingIsSnappedOnlyAtTheFinalStep(int deltaQ4096, int expectedCenter)
        {
            var position = deltaQ4096 >= 0 ? 12288 + deltaQ4096 : -12288 + deltaQ4096;
            var result = NewSession().Advance(Player(0, positionX: position));
            Assert.That(result.CenterXPixel18, Is.EqualTo(expectedCenter));
        }

        [Test]
        public void AxesUseIndependentCapsWithoutEarlyPositionQuantization()
        {
            var result = NewSession().Advance(Player(0, positionX: 4 * 4096, positionY: 3 * 4096));

            Assert.That(result.CenterXPixel18, Is.EqualTo(9));
            Assert.That(result.CenterYPixel18, Is.EqualTo(9));
        }

        [Test]
        public void BoundsClampAfterFollowCapAndEqualBoundsAreValid()
        {
            var result = NewSession(bounds: Bounds(-4, 4, -100, 100)).Advance(Player(0, positionX: 4 * 4096));
            Assert.That(result.CenterXPixel18, Is.EqualTo(4));

            var fixedSession = NewSession(bounds: Bounds(2, 2, 3, 3), centerX: 2, centerY: 3);
            var fixedResult = fixedSession.Advance(Player(0, positionX: int.MaxValue, positionY: int.MinValue));
            Assert.That(fixedResult.CenterXPixel18, Is.EqualTo(2));
            Assert.That(fixedResult.CenterYPixel18, Is.EqualTo(3));
        }

        [Test]
        public void InvalidBoundsAndInitialCenterAreRejectedBeforeSessionCreation()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CameraCenterBounds(2, 1, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => NewSession(bounds: Bounds(0, 1, 0, 1), centerX: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => NewSession(bounds: Bounds(0, 1, 0, 1), centerY: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => NewSession(firstTick: -1));
            Assert.Catch<Exception>(() => new CameraCenterBounds(int.MaxValue, int.MaxValue, 0, 0));
        }

        [TestCase(-1, -6)]
        [TestCase(1, 6)]
        [TestCase(9, 50)]
        public void Pixel18ProjectionUsesSignedAwayFromZeroQ100(int pixel18, int expectedQ100)
        {
            var result = NewSession(centerX: pixel18).Advance(Player(0));
            Assert.That(result.CenterXPixel18, Is.EqualTo(pixel18));
            Assert.That(result.CenterXQ100, Is.EqualTo(expectedQ100));
            Assert.That(Math.Abs((long)result.CenterXQ100 * 18L - pixel18 * 100L), Is.LessThan(9));
        }

        [Test]
        public void EverySmallSignedPixel18ProjectionStaysWithinStrictQ100ErrorBound()
        {
            for (var pixel18 = -100; pixel18 <= 100; pixel18++)
            {
                var result = NewSession(centerX: pixel18, bounds: Bounds(pixel18, pixel18, -100, 100)).Advance(Player(0));
                Assert.That(Math.Abs((long)result.CenterXQ100 * 18L - pixel18 * 100L), Is.LessThan(9), pixel18.ToString());
            }
        }

        [Test]
        public void RepeatedAndSkippedTicksPreservePriorSnapshotAndNextTick()
        {
            var session = NewSession();
            var committed = session.Advance(Player(0));
            Assert.Throws<InvalidOperationException>(() => session.Advance(Player(0)));
            Assert.Throws<InvalidOperationException>(() => session.Advance(Player(2)));
            AssertSnapshotEqual(committed, session.LatestSnapshot);
            Assert.That(session.NextExpectedTick, Is.EqualTo(new SimulationTick(1)));
        }

        [Test]
        public void TickSuccessorOverflowIsAtomicBeforeFirstPublication()
        {
            var session = NewSession(firstTick: int.MaxValue);
            Assert.Throws<OverflowException>(() => session.Advance(Player(int.MaxValue)));
            Assert.That(session.HasLatestSnapshot, Is.False);
            Assert.That(session.NextExpectedTick, Is.EqualTo(new SimulationTick(int.MaxValue)));
            Assert.Throws<InvalidOperationException>(() => { var ignored = session.LatestSnapshot; });
        }

        [Test]
        public void ScriptedThirtySixtyAndOneFortyFourRenderGroupingReplaysExactly()
        {
            var baseline = Replay(60);
            CollectionAssert.AreEqual(baseline, Replay(30));
            CollectionAssert.AreEqual(baseline, Replay(144));
        }

        private static List<string> Replay(int renderRate)
        {
            var session = NewSession();
            var values = new List<string>();
            var accumulator = 0;
            while (values.Count < 30)
            {
                accumulator += 60;
                while (accumulator >= renderRate && values.Count < 30)
                {
                    accumulator -= renderRate;
                    var tick = values.Count;
                    var result = session.Advance(Player(tick, positionX: tick * 123, positionY: -tick * 77,
                        velocityX: tick % 4 == 0 ? 8192 : 0, velocityY: tick % 5 == 0 ? -24576 : 0));
                    values.Add(string.Join("/", result.Tick.Value, result.CenterXPixel18, result.CenterYPixel18,
                        result.OffsetXPixel18, result.OffsetYPixel18, result.TargetXPixel18, result.TargetYPixel18,
                        result.TransitionAgeX, result.TransitionAgeY, result.CenterXQ100, result.CenterYQ100));
                }
            }
            return values;
        }

        private static SimulationCameraSession NewSession(int firstTick = 0, int centerX = 0, int centerY = 0,
            CameraCenterBounds? bounds = null) =>
            new SimulationCameraSession(new SimulationTick(firstTick), centerX, centerY,
                bounds ?? Bounds(-1000, 1000, -1000, 1000));

        private static CameraCenterBounds Bounds(int minX, int maxX, int minY, int maxY) =>
            new CameraCenterBounds(minX, maxX, minY, maxY);

        private static PlayerMotionSnapshot Player(int tick, int positionX = 0, int positionY = 0,
            int velocityX = 0, int velocityY = 0) =>
            new PlayerMotionSnapshot(new SimulationTick(tick), positionX, positionY, velocityX, velocityY,
                false, string.Empty, WallSide.None, MovementActionState.Normal, true);

        private static void AssertSnapshotEqual(CameraFollowSnapshot expected, CameraFollowSnapshot actual)
        {
            Assert.That(actual.Tick, Is.EqualTo(expected.Tick));
            Assert.That(actual.CenterXPixel18, Is.EqualTo(expected.CenterXPixel18));
            Assert.That(actual.CenterYPixel18, Is.EqualTo(expected.CenterYPixel18));
            Assert.That(actual.CenterXQ100, Is.EqualTo(expected.CenterXQ100));
            Assert.That(actual.CenterYQ100, Is.EqualTo(expected.CenterYQ100));
            Assert.That(actual.OffsetXPixel18, Is.EqualTo(expected.OffsetXPixel18));
            Assert.That(actual.OffsetYPixel18, Is.EqualTo(expected.OffsetYPixel18));
            Assert.That(actual.TargetXPixel18, Is.EqualTo(expected.TargetXPixel18));
            Assert.That(actual.TargetYPixel18, Is.EqualTo(expected.TargetYPixel18));
            Assert.That(actual.TransitionAgeX, Is.EqualTo(expected.TransitionAgeX));
            Assert.That(actual.TransitionAgeY, Is.EqualTo(expected.TransitionAgeY));
        }
    }
}
