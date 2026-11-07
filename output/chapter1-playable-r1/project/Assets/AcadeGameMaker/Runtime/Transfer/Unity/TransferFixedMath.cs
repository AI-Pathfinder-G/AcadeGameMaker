using System;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Transfer.Unity
{
    // REQ-WT-001, REQ-WT-003, REQ-WT-004, REQ-WT-007, REQ-WT-008.
    // M2 uses only checked integer/rational calculations; Unity floats never decide a selector key.
    internal static class TransferFixedMath
    {
        internal const int OrthoSizeQ1000 = 10000;
        internal const int RotationQ10 = 0;
        internal const int GridWidth = 1920;
        internal const int GridHeight = 1080;
        internal const long EllipseUnitQ1M = 1000000L;
        internal const long EllipseUnitSquaredQ1M = EllipseUnitQ1M * EllipseUnitQ1M;
        internal const long MouseDirectionMinimumDistanceSquaredQ1000 = 2500L;

        private static readonly int[] CordicMicrodegrees =
        {
            45000000, 26565051, 14036243, 7125016, 3576334, 1789911,
            895174, 447614, 223811, 111906, 55953, 27976,
            13988, 6994, 3497, 1749, 874, 437, 219, 109, 55, 27, 14, 7
        };

        internal static long RoundDivAway(long numerator, long denominator)
        {
            if (denominator <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(denominator));
            }

            var quotient = checked(numerator / denominator);
            var remainder = checked(numerator % denominator);
            if (remainder == 0)
            {
                return quotient;
            }

            var magnitude = remainder < 0 ? checked(-remainder) : remainder;
            // Equivalent to 2 * magnitude >= denominator, without a potentially overflowing multiply.
            if (magnitude >= checked(denominator - magnitude))
            {
                return numerator < 0 ? checked(quotient - 1) : checked(quotient + 1);
            }

            return quotient;
        }

        internal static (int x, int y) ProjectWorldToGrid(
            SimulationCameraPoseSnapshot camera,
            int worldXQ1000,
            int worldYQ1000)
        {
            ValidateVerticalDemoCamera(camera);
            var centerX = checked((long)camera.PositionXQ100 * 10L);
            var centerY = checked((long)camera.PositionYQ100 * 10L);
            var height = (long)camera.OrthoSizeQ1000;
            var xNumerator = checked(checked(checked(9L * checked((long)worldXQ1000 - centerX)) + checked(16L * height)) * 1919L);
            var yNumerator = checked(checked(checked((long)worldYQ1000 - centerY) + height) * 1079L);
            return (
                checked((int)RoundDivAway(xNumerator, checked(32L * height))),
                checked((int)RoundDivAway(yNumerator, checked(2L * height))));
        }

        internal static (int xQ1000, int yQ1000) InverseGridToWorld(
            SimulationCameraPoseSnapshot camera,
            int screenX,
            int screenY)
        {
            ValidateVerticalDemoCamera(camera);
            if (screenX < 0 || screenX >= GridWidth || screenY < 0 || screenY >= GridHeight)
            {
                throw new ArgumentOutOfRangeException();
            }

            var centerX = checked((long)camera.PositionXQ100 * 10L);
            var centerY = checked((long)camera.PositionYQ100 * 10L);
            var height = (long)camera.OrthoSizeQ1000;
            var xOffset = RoundDivAway(
                checked(checked((2L * screenX) - 1919L) * checked(16L * height)),
                checked(9L * 1919L));
            var yOffset = RoundDivAway(
                checked(checked((2L * screenY) - 1079L) * height),
                1079L);
            return (checked((int)checked(centerX + xOffset)), checked((int)checked(centerY + yOffset)));
        }

        internal static int ScreenDistanceSquaredKey(int screenX, int screenY, int projectedAimX, int projectedAimY)
        {
            var dx = checked((long)screenX - projectedAimX);
            var dy = checked((long)screenY - projectedAimY);
            return checked((int)checked(checked(dx * dx) + checked(dy * dy)));
        }

        internal static bool IsInsideAxisAlignedEllipse(
            int pointerXQ1000,
            int pointerYQ1000,
            int centerXQ1000,
            int centerYQ1000,
            int halfExtentXQ1000,
            int halfExtentYQ1000)
        {
            if (halfExtentXQ1000 <= 0 || halfExtentYQ1000 <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var dx = checked((long)pointerXQ1000 - centerXQ1000);
            var dy = checked((long)pointerYQ1000 - centerYQ1000);
            var absoluteX = Absolute(dx);
            var absoluteY = Absolute(dy);
            if (absoluteX > halfExtentXQ1000 || absoluteY > halfExtentYQ1000)
            {
                return false;
            }

            var normalizedX = RoundDivAway(checked(dx * EllipseUnitQ1M), halfExtentXQ1000);
            var normalizedY = RoundDivAway(checked(dy * EllipseUnitQ1M), halfExtentYQ1000);
            var squared = checked(checked(normalizedX * normalizedX) + checked(normalizedY * normalizedY));
            return squared <= EllipseUnitSquaredQ1M;
        }

        internal static bool HasMouseDirection(int deltaXQ1000, int deltaYQ1000)
        {
            var distanceSquared = checked(checked((long)deltaXQ1000 * deltaXQ1000) + checked((long)deltaYQ1000 * deltaYQ1000));
            return distanceSquared >= MouseDirectionMinimumDistanceSquaredQ1000;
        }

        internal static (int xQ4096, int yQ4096) NormalizeMouseDirectionQ4096(int deltaXQ1000, int deltaYQ1000)
        {
            var squaredLength = checked(checked((long)deltaXQ1000 * deltaXQ1000) + checked((long)deltaYQ1000 * deltaYQ1000));
            if (squaredLength < MouseDirectionMinimumDistanceSquaredQ1000)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaXQ1000));
            }

            return (NormalizeComponent(deltaXQ1000, squaredLength), NormalizeComponent(deltaYQ1000, squaredLength));
        }

        internal static int GamepadAngleKey(
            int aimXQ4096,
            int aimYQ4096,
            int targetDeltaXQ1000,
            int targetDeltaYQ1000)
        {
            if (targetDeltaXQ1000 == 0 && targetDeltaYQ1000 == 0)
            {
                return 0;
            }

            var dot = checked(checked((long)aimXQ4096 * targetDeltaXQ1000) + checked((long)aimYQ4096 * targetDeltaYQ1000));
            var cross = checked(checked((long)aimXQ4096 * targetDeltaYQ1000) - checked((long)aimYQ4096 * targetDeltaXQ1000));
            var x = Absolute(dot);
            var y = Absolute(cross);
            long acuteMicrodegrees = 0;
            for (var index = 0; index < CordicMicrodegrees.Length && y != 0; index++)
            {
                var oldX = x;
                var oldY = y;
                if (oldY > 0)
                {
                    x = checked(oldX + (oldY >> index));
                    y = checked(oldY - (oldX >> index));
                    acuteMicrodegrees = checked(acuteMicrodegrees + CordicMicrodegrees[index]);
                }
                else
                {
                    x = checked(oldX - (oldY >> index));
                    y = checked(oldY + (oldX >> index));
                    acuteMicrodegrees = checked(acuteMicrodegrees - CordicMicrodegrees[index]);
                }
            }

            var angle = dot < 0 ? checked(180000000L - acuteMicrodegrees) : acuteMicrodegrees;
            return checked((int)RoundDivAway(angle, 100000L));
        }

        internal static void ValidateVerticalDemoCamera(SimulationCameraPoseSnapshot camera)
        {
            if (camera.OrthoSizeQ1000 != OrthoSizeQ1000 || camera.RotationQ10 != RotationQ10)
            {
                throw new ArgumentException("VD-02 M2 supports only the approved orthographic camera pose.", nameof(camera));
            }
        }

        private static int NormalizeComponent(int component, long squaredLength)
        {
            var absolute = Absolute((long)component);
            var numerator = checked(checked(absolute * absolute) * 4096L * 4096L);
            var low = 0;
            var high = 4096;
            while (low < high)
            {
                var middle = (low + high + 1) / 2;
                var candidate = checked((long)middle * middle * squaredLength);
                if (candidate <= numerator)
                {
                    low = middle;
                }
                else
                {
                    high = middle - 1;
                }
            }

            var q = low;
            var doubledCandidate = checked(checked((long)(2 * q + 1) * (2 * q + 1)) * squaredLength);
            if (doubledCandidate <= checked(4L * numerator))
            {
                q++;
            }

            return component < 0 ? -q : q;
        }

        private static long Absolute(long value)
        {
            if (value == long.MinValue)
            {
                throw new OverflowException();
            }

            return value < 0 ? -value : value;
        }
    }
}
