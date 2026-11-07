using System;

namespace AcadeGameMaker.Core
{
    public readonly struct SimulationTick : IEquatable<SimulationTick>, IComparable<SimulationTick>
    {
        public SimulationTick(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public int CompareTo(SimulationTick other) => Value.CompareTo(other.Value);
        public bool Equals(SimulationTick other) => Value == other.Value;
        public override bool Equals(object obj) => obj is SimulationTick other && Equals(other);
        public override int GetHashCode() => Value;
        public static bool operator ==(SimulationTick left, SimulationTick right) => left.Equals(right);
        public static bool operator !=(SimulationTick left, SimulationTick right) => !left.Equals(right);
    }
}
