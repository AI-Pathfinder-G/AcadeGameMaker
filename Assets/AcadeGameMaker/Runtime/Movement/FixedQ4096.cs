using System;

namespace AcadeGameMaker.Movement
{
    // Internal only: REQ-MOV-001~004, REQ-MOV-006~010 use Q4096 public payloads.
    internal static class FixedQ4096
    {
        internal const int Units = 4096;
        internal const int TicksPerSecond = 60;
        internal const int Deadzone = 819; // round(0.20 * 4096), away from zero at an exact half.
        internal const int RunSpeed = 32768; // 8.0 u/s
        internal const int GroundAcceleration = 286720; // 70.0 u/s²
        internal const int GroundDeceleration = 368640; // 90.0 u/s²
        internal const int AirAcceleration = 163840; // 40.0 u/s²
        internal const int AirDeceleration = 81920; // 20.0 u/s²
        internal const int LightweightAirAcceleration = 204800; // 50.0 u/s²
        internal const int Gravity = 125829; // round(30.72 * 4096)
        internal const int JumpVelocity = 58941; // round(14.39 * 4096)
        internal const int MaxFallSpeed = 90112; // 22.0 u/s
        internal const int LightweightMaxFallSpeed = 63078; // round(15.4 * 4096)
        internal const int DashVelocity = 81920; // 20.0 u/s
        internal const int WallSlideFallSpeed = 24576; // 6.0 u/s
        internal const int WallJumpX = 38912; // 9.5 u/s
        internal const int WallJumpY = 55296; // 13.5 u/s

        internal static int DivideRoundAwayFromZero(long numerator, int denominator)
        {
            var absolute = Math.Abs(numerator);
            var rounded = (absolute + denominator / 2) / denominator;
            return checked((int)(numerator < 0 ? -rounded : rounded));
        }

        internal static int Integrate(ref int value, int ratePerSecond, ref long remainder)
        {
            var sum = checked((long)ratePerSecond + remainder);
            var delta = sum / TicksPerSecond;
            remainder = sum % TicksPerSecond;
            value = checked(value + checked((int)delta));
            return value;
        }

        internal static int ApplyPerTick(ref int value, int signedRatePerSecond, ref long remainder, int multiplierNumerator, int multiplierDenominator)
        {
            var denominator = checked(TicksPerSecond * multiplierDenominator);
            var sum = checked((long)signedRatePerSecond * multiplierNumerator + remainder);
            var delta = sum / denominator;
            remainder = sum % denominator;
            value = checked(value + checked((int)delta));
            return value;
        }
    }
}
