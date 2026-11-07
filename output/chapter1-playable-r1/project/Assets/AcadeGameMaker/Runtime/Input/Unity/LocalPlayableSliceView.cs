using System;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-LPS-003/004/005: immutable completed facts; no gameplay authority.
    internal enum LocalPlayableSliceResult { Playing = 0, Succeeded = 1, Failed = 2 }

    internal readonly struct LocalPlayableSliceView
    {
        internal LocalPlayableSliceView(long generation, SimulationTick tick, int playerHealth, int bossHealth,
            string transferTarget, bool lightweight, LocalPlayableSliceResult result, SimulationTick? uiOnlyTick)
        {
            if (generation < 1 || tick.Value < 0 || playerHealth < 0 || playerHealth > 5 || bossHealth < 0 || bossHealth > 60 ||
                !Enum.IsDefined(typeof(LocalPlayableSliceResult), result) ||
                (result == LocalPlayableSliceResult.Playing && uiOnlyTick.HasValue) ||
                (uiOnlyTick.HasValue && uiOnlyTick.Value.Value <= tick.Value))
                throw new ArgumentException("Local playable completed view is malformed.");
            Generation = generation; Tick = tick; PlayerHealth = playerHealth; BossHealth = bossHealth;
            TransferTarget = transferTarget ?? string.Empty; Lightweight = lightweight; Result = result; UiOnlyTick = uiOnlyTick;
        }
        internal long Generation { get; }
        internal SimulationTick Tick { get; }
        internal int PlayerHealth { get; }
        internal int PlayerMaxHealth => 5;
        internal int BossHealth { get; }
        internal int BossMaxHealth => 60;
        internal string TransferTarget { get; }
        internal bool Lightweight { get; }
        internal LocalPlayableSliceResult Result { get; }
        internal SimulationTick? UiOnlyTick { get; }
        internal LocalPlayableSliceView WithGeneration(long generation) =>
            new LocalPlayableSliceView(generation, Tick, PlayerHealth, BossHealth, TransferTarget, Lightweight, Result, UiOnlyTick);
        internal LocalPlayableSliceView WithUiOnlyTick(SimulationTick tick) =>
            new LocalPlayableSliceView(Generation, Tick, PlayerHealth, BossHealth, TransferTarget, Lightweight, Result, tick);
    }

    // REQ-LPS-007: retained handle reads a managed ownership witness after Unity destruction.
    // It has no external setter and contains no scene-object reference.
    internal sealed class LocalPlayableInputLifetime
    {
        private readonly Func<bool> _readClosed;
        internal LocalPlayableInputLifetime(Func<bool> readClosed) { _readClosed = readClosed ?? throw new ArgumentNullException(nameof(readClosed)); }
        internal bool IsClosed => _readClosed();
    }
}
