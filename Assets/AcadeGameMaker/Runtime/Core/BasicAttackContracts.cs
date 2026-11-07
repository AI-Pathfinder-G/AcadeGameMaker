using System;

namespace AcadeGameMaker.Core
{
    // REQ-COM-001, REQ-UX-007, REQ-UX-008. VD-07 will publish this immutable semantic press; it is never a device or render-frame identity.
    public readonly struct BasicAttackPressed
    {
        public BasicAttackPressed(long attackSequenceId, long aimSampleId, SimulationTick tick)
        {
            if (attackSequenceId < 0) throw new ArgumentOutOfRangeException(nameof(attackSequenceId));
            if (aimSampleId < 0) throw new ArgumentOutOfRangeException(nameof(aimSampleId));
            AttackSequenceId = attackSequenceId;
            AimSampleId = aimSampleId;
            Tick = tick;
        }

        public long AttackSequenceId { get; }
        public long AimSampleId { get; }
        public SimulationTick Tick { get; }
    }
}
