using System;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Camera
{
    // REQ-ART-010/011, REQ-UX-013. Authored inclusive camera-center lattice bounds.
    internal readonly struct CameraCenterBounds
    {
        internal CameraCenterBounds(int minXPixel18, int maxXPixel18, int minYPixel18, int maxYPixel18)
        {
            if (minXPixel18 > maxXPixel18 || minYPixel18 > maxYPixel18) throw new ArgumentOutOfRangeException();
            // Validate the approved existing Q100 publication boundary now, before session state exists.
            ToQ100(minXPixel18); ToQ100(maxXPixel18); ToQ100(minYPixel18); ToQ100(maxYPixel18);
            MinXPixel18 = minXPixel18; MaxXPixel18 = maxXPixel18; MinYPixel18 = minYPixel18; MaxYPixel18 = maxYPixel18;
        }

        internal int MinXPixel18 { get; } internal int MaxXPixel18 { get; }
        internal int MinYPixel18 { get; } internal int MaxYPixel18 { get; }

        internal bool Contains(int xPixel18, int yPixel18) => xPixel18 >= MinXPixel18 && xPixel18 <= MaxXPixel18 && yPixel18 >= MinYPixel18 && yPixel18 <= MaxYPixel18;
        internal static int ToQ100(int pixel18) => checked((int)RoundAway(checked((long)pixel18 * 100L), 18L));
        internal static long RoundAway(long numerator, long denominator)
        {
            if (denominator <= 0) throw new ArgumentOutOfRangeException(nameof(denominator));
            var quotient = checked(numerator / denominator); var remainder = checked(numerator % denominator);
            if (remainder == 0) return quotient;
            var magnitude = remainder < 0 ? checked(-remainder) : remainder;
            return magnitude >= checked(denominator - magnitude) ? (numerator < 0 ? checked(quotient - 1) : checked(quotient + 1)) : quotient;
        }
    }

    internal readonly struct CameraFollowSnapshot
    {
        internal CameraFollowSnapshot(SimulationTick tick, int centerXPixel18, int centerYPixel18, int centerXQ100, int centerYQ100,
            int offsetXPixel18, int offsetYPixel18, int targetXPixel18, int targetYPixel18, int transitionAgeX, int transitionAgeY)
        {
            if (tick.Value < 0 || transitionAgeX < 0 || transitionAgeX > 12 || transitionAgeY < 0 || transitionAgeY > 12) throw new ArgumentOutOfRangeException();
            Tick = tick; CenterXPixel18 = centerXPixel18; CenterYPixel18 = centerYPixel18; CenterXQ100 = centerXQ100; CenterYQ100 = centerYQ100;
            OffsetXPixel18 = offsetXPixel18; OffsetYPixel18 = offsetYPixel18; TargetXPixel18 = targetXPixel18; TargetYPixel18 = targetYPixel18;
            TransitionAgeX = transitionAgeX; TransitionAgeY = transitionAgeY;
        }
        internal SimulationTick Tick { get; } internal int CenterXPixel18 { get; } internal int CenterYPixel18 { get; }
        internal int CenterXQ100 { get; } internal int CenterYQ100 { get; }
        internal int OffsetXPixel18 { get; } internal int OffsetYPixel18 { get; }
        internal int TargetXPixel18 { get; } internal int TargetYPixel18 { get; }
        internal int TransitionAgeX { get; } internal int TransitionAgeY { get; }
    }
}
