using System;

namespace AcadeGameMaker.Core
{
    // REQ-UX-004, REQ-UX-006, REQ-UX-008. The sole engine-free identity of a
    // globally committed device-input frame.
    public enum InputMode
    {
        GameplayEnabled,
        UIOnly,
        Cutscene,
        Transition,
        Ended
    }

    public readonly struct InputFrameCommitReceipt : IEquatable<InputFrameCommitReceipt>
    {
        public InputFrameCommitReceipt(SimulationTick tick, long frameOrdinal, long modeEpoch, InputMode mode)
        {
            if (tick.Value < 0) throw new ArgumentOutOfRangeException(nameof(tick));
            if (frameOrdinal < 0) throw new ArgumentOutOfRangeException(nameof(frameOrdinal));
            if (modeEpoch < 0) throw new ArgumentOutOfRangeException(nameof(modeEpoch));
            if (mode < InputMode.GameplayEnabled || mode > InputMode.Ended) throw new ArgumentOutOfRangeException(nameof(mode));
            Tick = tick;
            FrameOrdinal = frameOrdinal;
            ModeEpoch = modeEpoch;
            Mode = mode;
        }

        public SimulationTick Tick { get; }
        public long FrameOrdinal { get; }
        public long ModeEpoch { get; }
        public InputMode Mode { get; }

        public bool Equals(InputFrameCommitReceipt other) =>
            Tick == other.Tick && FrameOrdinal == other.FrameOrdinal && ModeEpoch == other.ModeEpoch && Mode == other.Mode;
        public override bool Equals(object obj) => obj is InputFrameCommitReceipt other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Tick.GetHashCode();
                hash = (hash * 397) ^ FrameOrdinal.GetHashCode();
                hash = (hash * 397) ^ ModeEpoch.GetHashCode();
                return (hash * 397) ^ (int)Mode;
            }
        }
        public static bool operator ==(InputFrameCommitReceipt left, InputFrameCommitReceipt right) => left.Equals(right);
        public static bool operator !=(InputFrameCommitReceipt left, InputFrameCommitReceipt right) => !left.Equals(right);
    }

    public interface IInputFrameCommitSource
    {
        InputFrameCommitReceipt? CurrentReceipt { get; }
        bool IsFaulted { get; }
    }
}
