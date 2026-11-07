using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;

namespace AcadeGameMaker.Camera
{
    // REQ-ART-011, REQ-UX-013. Engine-free GameplayFollow owner.  Pixel18
    // state is authoritative; Q100 is only the approved publication projection.
    internal sealed class SimulationCameraSession
    {
        private const int SettledAge = 12;
        private const int HorizontalHalfExtentPixel18 = 54;
        private const int VerticalHalfExtentPixel18 = 36;
        private const int FollowCapPixel18 = 9;
        private const long Q4096PerPixel18 = 4096L;

        private readonly CameraCenterBounds _bounds;
        private SimulationTick _nextExpectedTick;
        private int _centerX, _centerY;
        private AxisState _x = new AxisState(0, 0, 0, SettledAge);
        private AxisState _y = new AxisState(0, 0, 0, SettledAge);
        private bool _hasLatest;
        private CameraFollowSnapshot _latest;

        internal SimulationCameraSession(SimulationTick firstTick, int initialCenterXPixel18, int initialCenterYPixel18, CameraCenterBounds bounds)
        {
            if (firstTick.Value < 0 || !bounds.Contains(initialCenterXPixel18, initialCenterYPixel18)) throw new ArgumentOutOfRangeException();
            _bounds = bounds; _nextExpectedTick = firstTick; _centerX = initialCenterXPixel18; _centerY = initialCenterYPixel18;
        }

        internal SimulationTick NextExpectedTick => _nextExpectedTick;
        internal bool HasLatestSnapshot => _hasLatest;
        internal CameraFollowSnapshot LatestSnapshot => _hasLatest ? _latest : throw new InvalidOperationException("Camera has no completed snapshot.");

        internal CameraFollowSnapshot Advance(PlayerMotionSnapshot completedPlayer)
        {
            if (completedPlayer.Tick != _nextExpectedTick) throw new InvalidOperationException("Camera requires the exact next completed player tick.");

            // Stage every checked operation before assigning any owner field.
            var nextTick = new SimulationTick(checked(_nextExpectedTick.Value + 1));
            var stagedX = AdvanceAxis(_x, HorizontalTarget(completedPlayer.VelocityXQ4096));
            var stagedY = AdvanceAxis(_y, VerticalTarget(completedPlayer.VelocityYQ4096));
            var centerX = FollowAxis(_centerX, completedPlayer.PositionXQ4096, stagedX.Offset, HorizontalHalfExtentPixel18, _bounds.MinXPixel18, _bounds.MaxXPixel18);
            var centerY = FollowAxis(_centerY, completedPlayer.PositionYQ4096, stagedY.Offset, VerticalHalfExtentPixel18, _bounds.MinYPixel18, _bounds.MaxYPixel18);
            var q100X = CameraCenterBounds.ToQ100(centerX);
            var q100Y = CameraCenterBounds.ToQ100(centerY);
            var snapshot = new CameraFollowSnapshot(completedPlayer.Tick, centerX, centerY, q100X, q100Y,
                stagedX.Offset, stagedY.Offset, stagedX.Target, stagedY.Target, stagedX.Age, stagedY.Age);

            _x = stagedX; _y = stagedY; _centerX = centerX; _centerY = centerY; _latest = snapshot; _hasLatest = true; _nextExpectedTick = nextTick;
            return snapshot;
        }

        private static int HorizontalTarget(int velocity) => velocity <= -8192 ? -54 : velocity >= 8192 ? 54 : 0;
        private static int VerticalTarget(int velocity) => velocity >= 16384 ? 27 : velocity <= -24576 ? -54 : 0;

        private static AxisState AdvanceAxis(AxisState current, int requestedTarget)
        {
            var changed = requestedTarget != current.Target;
            var start = changed ? current.Offset : current.Start;
            var age = changed ? 1 : Math.Min(SettledAge, checked(current.Age + 1));
            var offset = checked((int)checked((long)start + CameraCenterBounds.RoundAway(checked((long)(requestedTarget - start) * age), SettledAge)));
            return new AxisState(start, requestedTarget, offset, age);
        }

        private static int FollowAxis(int centerPixel18, int playerQ4096, int offsetPixel18, int halfExtentPixel18, int minPixel18, int maxPixel18)
        {
            var center = checked((long)centerPixel18 * Q4096PerPixel18);
            var focus = checked(checked((long)playerQ4096 * 18L) + checked((long)offsetPixel18 * Q4096PerPixel18));
            var extent = checked((long)halfExtentPixel18 * Q4096PerPixel18);
            long desired = center;
            if (focus > checked(center + extent)) desired = checked(focus - extent);
            else if (focus < checked(center - extent)) desired = checked(focus + extent);
            var cap = checked((long)FollowCapPixel18 * Q4096PerPixel18);
            var delta = checked(desired - center);
            var moved = delta > cap ? checked(center + cap) : delta < -cap ? checked(center - cap) : desired;
            var min = checked((long)minPixel18 * Q4096PerPixel18);
            var max = checked((long)maxPixel18 * Q4096PerPixel18);
            var clamped = moved < min ? min : moved > max ? max : moved;
            return checked((int)CameraCenterBounds.RoundAway(clamped, Q4096PerPixel18));
        }

        private readonly struct AxisState
        {
            internal AxisState(int start, int target, int offset, int age) { Start = start; Target = target; Offset = offset; Age = age; }
            internal int Start { get; } internal int Target { get; } internal int Offset { get; } internal int Age { get; }
        }
    }
}
