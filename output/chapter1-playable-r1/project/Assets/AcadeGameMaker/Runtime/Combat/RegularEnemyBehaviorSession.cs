using System;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat
{
    // REQ-COM-001, REQ-COM-002, REQ-COM-004, REQ-COM-005; affected REQ-WT-003, REQ-WT-005, REQ-WT-007.
    // This is deliberately an engine-free, pair-atomic planner. M1, M3A, M3B1 and VD-02 remain external owners.
    internal enum RegularEnemyBehaviorInputKind { Gameplay = 0, EncounterReset = 1 }
    internal enum WalkerBehaviorPhase { Approach = 0, DashTelegraph = 1, DashActive = 2, Recovery = 3, Dead = 4, Reset = 5 }
    internal enum SurveyorBehaviorPhase { Relocate = 0, FireTelegraph = 1, Fire = 2, Cooldown = 3, Suppressed = 4, Dead = 5, Reset = 6 }
    internal enum RegularEnemyMovementIntent { None = 0, Horizontal = 1, DashHorizontal = 2, Downward = 3 }

    internal readonly struct WalkerRecoveryIntent
    {
        internal WalkerRecoveryIntent(string targetId, long attackCycleOrdinal, SimulationTick scheduledTick)
        {
            if (!string.Equals(targetId, "walker", StringComparison.Ordinal)) throw new ArgumentException("Recovery intent target must be walker.", nameof(targetId));
            if (attackCycleOrdinal < 0 || scheduledTick.Value < 0) throw new ArgumentOutOfRangeException();
            TargetId = targetId;
            AttackCycleOrdinal = attackCycleOrdinal;
            ScheduledTick = scheduledTick;
        }

        internal string TargetId { get; }
        internal long AttackCycleOrdinal { get; }
        internal SimulationTick ScheduledTick { get; }
    }

    internal readonly struct SurveyorShotIntent
    {
        internal SurveyorShotIntent(string targetId, long shotOrdinal, int targetXQ1000)
        {
            if (!string.Equals(targetId, "surveyor", StringComparison.Ordinal)) throw new ArgumentException("Shot intent target must be surveyor.", nameof(targetId));
            if (shotOrdinal < 0) throw new ArgumentOutOfRangeException(nameof(shotOrdinal));
            TargetId = targetId;
            ShotOrdinal = shotOrdinal;
            TargetXQ1000 = targetXQ1000;
        }

        internal string TargetId { get; }
        internal long ShotOrdinal { get; }
        internal int TargetXQ1000 { get; }
    }

    internal readonly struct RegularEnemyBehaviorPairInput
    {
        private RegularEnemyBehaviorPairInput(
            RegularEnemyBehaviorInputKind kind,
            SimulationTick tick,
            bool walkerFinalAlive,
            bool surveyorFinalAlive,
            RegularEnemyReactionSnapshot walkerReaction,
            RegularEnemyReactionSnapshot surveyorReaction,
            int playerXQ1000,
            int walkerXQ1000,
            int surveyorXQ1000,
            bool walkerHasLineOfSight,
            bool surveyorHasLineOfSight)
        {
            if (kind != RegularEnemyBehaviorInputKind.Gameplay && kind != RegularEnemyBehaviorInputKind.EncounterReset) throw new ArgumentOutOfRangeException(nameof(kind));
            if (tick.Value < 0) throw new ArgumentOutOfRangeException(nameof(tick));
            Kind = kind;
            Tick = tick;
            WalkerFinalAlive = walkerFinalAlive;
            SurveyorFinalAlive = surveyorFinalAlive;
            WalkerReaction = walkerReaction;
            SurveyorReaction = surveyorReaction;
            PlayerXQ1000 = playerXQ1000;
            WalkerXQ1000 = walkerXQ1000;
            SurveyorXQ1000 = surveyorXQ1000;
            WalkerHasLineOfSight = walkerHasLineOfSight;
            SurveyorHasLineOfSight = surveyorHasLineOfSight;
        }

        internal static RegularEnemyBehaviorPairInput Gameplay(
            SimulationTick tick, bool walkerFinalAlive, bool surveyorFinalAlive,
            RegularEnemyReactionSnapshot walkerReaction, RegularEnemyReactionSnapshot surveyorReaction,
            int playerXQ1000, int walkerXQ1000, int surveyorXQ1000,
            bool walkerHasLineOfSight, bool surveyorHasLineOfSight) =>
            new RegularEnemyBehaviorPairInput(RegularEnemyBehaviorInputKind.Gameplay, tick, walkerFinalAlive, surveyorFinalAlive,
                walkerReaction, surveyorReaction, playerXQ1000, walkerXQ1000, surveyorXQ1000, walkerHasLineOfSight, surveyorHasLineOfSight);

        internal static RegularEnemyBehaviorPairInput EncounterReset(
            SimulationTick tick, RegularEnemyReactionSnapshot walkerReaction, RegularEnemyReactionSnapshot surveyorReaction) =>
            new RegularEnemyBehaviorPairInput(RegularEnemyBehaviorInputKind.EncounterReset, tick, false, false,
                walkerReaction, surveyorReaction, 0, 0, 0, false, false);

        // REQ-COM-004: keeps invalid discriminant rejection observable without widening the runtime input surface.
        internal static void AssertInvalidDiscriminantRejectedForTests() =>
            _ = new RegularEnemyBehaviorPairInput((RegularEnemyBehaviorInputKind)int.MaxValue, new SimulationTick(0), false, false, default, default, 0, 0, 0, false, false);

        internal RegularEnemyBehaviorInputKind Kind { get; }
        internal SimulationTick Tick { get; }
        internal bool WalkerFinalAlive { get; }
        internal bool SurveyorFinalAlive { get; }
        internal RegularEnemyReactionSnapshot WalkerReaction { get; }
        internal RegularEnemyReactionSnapshot SurveyorReaction { get; }
        internal int PlayerXQ1000 { get; }
        internal int WalkerXQ1000 { get; }
        internal int SurveyorXQ1000 { get; }
        internal bool WalkerHasLineOfSight { get; }
        internal bool SurveyorHasLineOfSight { get; }
    }

    internal readonly struct WalkerBehaviorSnapshot
    {
        internal WalkerBehaviorSnapshot(WalkerBehaviorPhase phase, int exclusiveEndTick, bool isAlive, RegularEnemyMovementIntent movement, int direction, bool guardRaised, bool telegraph, int latchedDashDirection, long attackCycleOrdinal)
        {
            Phase = phase; ExclusiveEndTick = exclusiveEndTick; IsAlive = isAlive; Movement = movement; Direction = direction;
            GuardRaised = guardRaised; Telegraph = telegraph; LatchedDashDirection = latchedDashDirection; AttackCycleOrdinal = attackCycleOrdinal;
        }
        internal WalkerBehaviorPhase Phase { get; }
        internal int ExclusiveEndTick { get; }
        internal bool IsAlive { get; }
        internal RegularEnemyMovementIntent Movement { get; }
        internal int Direction { get; }
        internal bool GuardRaised { get; }
        internal bool Telegraph { get; }
        internal int LatchedDashDirection { get; }
        internal long AttackCycleOrdinal { get; }
    }

    internal readonly struct SurveyorBehaviorSnapshot
    {
        internal SurveyorBehaviorSnapshot(SurveyorBehaviorPhase phase, int exclusiveEndTick, bool isAlive, RegularEnemyMovementIntent movement, int direction, bool telegraph, bool hasLatchedTarget, int latchedTargetXQ1000, long shotOrdinal)
        {
            Phase = phase; ExclusiveEndTick = exclusiveEndTick; IsAlive = isAlive; Movement = movement; Direction = direction;
            Telegraph = telegraph; HasLatchedTarget = hasLatchedTarget; LatchedTargetXQ1000 = latchedTargetXQ1000; ShotOrdinal = shotOrdinal;
        }
        internal SurveyorBehaviorPhase Phase { get; }
        internal int ExclusiveEndTick { get; }
        internal bool IsAlive { get; }
        internal RegularEnemyMovementIntent Movement { get; }
        internal int Direction { get; }
        internal bool Telegraph { get; }
        internal bool HasLatchedTarget { get; }
        internal int LatchedTargetXQ1000 { get; }
        internal long ShotOrdinal { get; }
    }

    internal readonly struct RegularEnemyBehaviorPairSnapshot
    {
        internal RegularEnemyBehaviorPairSnapshot(SimulationTick tick, WalkerBehaviorSnapshot walker, SurveyorBehaviorSnapshot surveyor, WalkerRecoveryIntent? walkerRecoveryIntent, SurveyorShotIntent? surveyorShotIntent, SimulationTick nextExpectedTick)
        {
            Tick = tick; Walker = walker; Surveyor = surveyor; WalkerRecoveryIntent = walkerRecoveryIntent; SurveyorShotIntent = surveyorShotIntent; NextExpectedTick = nextExpectedTick;
        }
        internal SimulationTick Tick { get; }
        internal WalkerBehaviorSnapshot Walker { get; }
        internal SurveyorBehaviorSnapshot Surveyor { get; }
        internal WalkerRecoveryIntent? WalkerRecoveryIntent { get; }
        internal SurveyorShotIntent? SurveyorShotIntent { get; }
        internal SimulationTick NextExpectedTick { get; }
    }

    internal sealed class RegularEnemyBehaviorSession
    {
        private const string WalkerId = "walker";
        private const string SurveyorId = "surveyor";
        private SimulationTick _nextExpectedTick;
        private bool _hasCompletedGameplay;
        private bool _walkerDead;
        private bool _surveyorDead;
        private WalkerBehaviorPhase _walkerPhase = WalkerBehaviorPhase.Approach;
        private SurveyorBehaviorPhase _surveyorPhase = SurveyorBehaviorPhase.Relocate;
        private int _walkerEndTick = -1;
        private int _surveyorEndTick = -1;
        private int _walkerDashDirection;
        private bool _surveyorHasLatchedTarget;
        private int _surveyorLatchedTargetXQ1000;
        private long _walkerAttackCycleOrdinal;
        private long _surveyorShotOrdinal;
        private RegularEnemyBehaviorPairSnapshot? _latestSnapshot;

        internal RegularEnemyBehaviorSession(SimulationTick firstExpectedTick)
        {
            if (firstExpectedTick.Value < 0) throw new ArgumentOutOfRangeException(nameof(firstExpectedTick));
            _nextExpectedTick = firstExpectedTick;
        }

        internal SimulationTick NextExpectedTick => _nextExpectedTick;
        internal RegularEnemyBehaviorPairSnapshot? LatestSnapshot => _latestSnapshot;

        // REQ-COM-004, REQ-COM-005: validate both staged roles through the same checked code path before either owner field commits.
        internal void ValidateNext(RegularEnemyBehaviorPairInput input)
        {
            _ = PreviewNext(input);
        }

        // REQ-COM-004, REQ-COM-005: M3B2B previews the exact pair before it commits either its
        // behavior session or M3B1's future recovery queue. The cloned session retains no mutation.
        internal RegularEnemyBehaviorPairSnapshot PreviewNext(RegularEnemyBehaviorPairInput input)
        {
            var staged = new RegularEnemyBehaviorSession(_nextExpectedTick)
            {
                _hasCompletedGameplay = _hasCompletedGameplay,
                _walkerDead = _walkerDead,
                _surveyorDead = _surveyorDead,
                _walkerPhase = _walkerPhase,
                _surveyorPhase = _surveyorPhase,
                _walkerEndTick = _walkerEndTick,
                _surveyorEndTick = _surveyorEndTick,
                _walkerDashDirection = _walkerDashDirection,
                _surveyorHasLatchedTarget = _surveyorHasLatchedTarget,
                _surveyorLatchedTargetXQ1000 = _surveyorLatchedTargetXQ1000,
                _walkerAttackCycleOrdinal = _walkerAttackCycleOrdinal,
                _surveyorShotOrdinal = _surveyorShotOrdinal,
                _latestSnapshot = _latestSnapshot
            };
            return staged.Process(input);
        }

        internal RegularEnemyBehaviorPairSnapshot Process(RegularEnemyBehaviorPairInput input)
        {
            if (input.Tick != _nextExpectedTick) throw new InvalidOperationException("Behavior input must use the exact next global simulation tick.");
            if (input.Kind != RegularEnemyBehaviorInputKind.Gameplay && input.Kind != RegularEnemyBehaviorInputKind.EncounterReset) throw new ArgumentOutOfRangeException(nameof(input));
            return input.Kind == RegularEnemyBehaviorInputKind.EncounterReset ? ProcessReset(input) : ProcessGameplay(input);
        }

        // Internal test seam only; no public ABI. REQ-COM-004: makes checked ordinal-boundary atomicity observable without a synthetic long-running encounter.
        internal void SetOrdinalsForTests(long walkerAttackCycleOrdinal, long surveyorShotOrdinal)
        {
            if (walkerAttackCycleOrdinal < 0 || surveyorShotOrdinal < 0) throw new ArgumentOutOfRangeException();
            _walkerAttackCycleOrdinal = walkerAttackCycleOrdinal;
            _surveyorShotOrdinal = surveyorShotOrdinal;
        }

        private RegularEnemyBehaviorPairSnapshot ProcessGameplay(RegularEnemyBehaviorPairInput input)
        {
            var nextTick = new SimulationTick(checked(input.Tick.Value + 1));
            ValidateGameplayInput(input, nextTick);

            var walker = StageWalker(input, nextTick, out var walkerPhase, out var walkerEnd, out var walkerDead, out var walkerDashDirection, out var walkerOrdinal, out var recovery);
            var surveyor = StageSurveyor(input, out var surveyorPhase, out var surveyorEnd, out var surveyorDead, out var hasLatchedTarget, out var latchedTargetX, out var surveyorOrdinal, out var shot);
            var snapshot = new RegularEnemyBehaviorPairSnapshot(input.Tick, walker, surveyor, recovery, shot, nextTick);

            _hasCompletedGameplay = true;
            _walkerDead = walkerDead;
            _surveyorDead = surveyorDead;
            _walkerPhase = walkerPhase;
            _surveyorPhase = surveyorPhase;
            _walkerEndTick = walkerEnd;
            _surveyorEndTick = surveyorEnd;
            _walkerDashDirection = walkerDashDirection;
            _surveyorHasLatchedTarget = hasLatchedTarget;
            _surveyorLatchedTargetXQ1000 = latchedTargetX;
            _walkerAttackCycleOrdinal = walkerOrdinal;
            _surveyorShotOrdinal = surveyorOrdinal;
            _latestSnapshot = snapshot;
            _nextExpectedTick = nextTick;
            return snapshot;
        }

        private RegularEnemyBehaviorPairSnapshot ProcessReset(RegularEnemyBehaviorPairInput input)
        {
            if (!_hasCompletedGameplay) throw new InvalidOperationException("Encounter reset is forbidden at the first expected tick.");
            var nextTick = new SimulationTick(checked(input.Tick.Value + 1));
            ValidateResetReaction(input.WalkerReaction, WalkerId, input.Tick, nextTick);
            ValidateResetReaction(input.SurveyorReaction, SurveyorId, input.Tick, nextTick);
            var walker = new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Reset, -1, true, RegularEnemyMovementIntent.None, 0, false, false, 0, 0);
            var surveyor = new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Reset, -1, true, RegularEnemyMovementIntent.None, 0, false, false, 0, 0);
            var snapshot = new RegularEnemyBehaviorPairSnapshot(input.Tick, walker, surveyor, null, null, nextTick);

            _hasCompletedGameplay = false;
            _walkerDead = false;
            _surveyorDead = false;
            _walkerPhase = WalkerBehaviorPhase.Reset;
            _surveyorPhase = SurveyorBehaviorPhase.Reset;
            _walkerEndTick = -1;
            _surveyorEndTick = -1;
            _walkerDashDirection = 0;
            _surveyorHasLatchedTarget = false;
            _surveyorLatchedTargetXQ1000 = 0;
            _walkerAttackCycleOrdinal = 0;
            _surveyorShotOrdinal = 0;
            _latestSnapshot = snapshot;
            _nextExpectedTick = nextTick;
            return snapshot;
        }

        private void ValidateGameplayInput(RegularEnemyBehaviorPairInput input, SimulationTick nextTick)
        {
            ValidateGameplayReaction(input.WalkerReaction, WalkerId, input.Tick, nextTick, true);
            ValidateGameplayReaction(input.SurveyorReaction, SurveyorId, input.Tick, nextTick, false);
            if (input.WalkerFinalAlive != input.WalkerReaction.IsAlive || input.SurveyorFinalAlive != input.SurveyorReaction.IsAlive)
                throw new InvalidOperationException("M1 final-alive values must agree with exact M3A snapshots.");
            if (_walkerDead && input.WalkerFinalAlive) throw new InvalidOperationException("Walker resurrection requires encounter reset.");
            if (_surveyorDead && input.SurveyorFinalAlive) throw new InvalidOperationException("Surveyor resurrection requires encounter reset.");

            // REQ-COM-004: explicitly exercise the Q1000 long-delta path before either role can commit.
            var walkerDx = (long)input.PlayerXQ1000 - input.WalkerXQ1000;
            var surveyorDx = (long)input.PlayerXQ1000 - input.SurveyorXQ1000;
            _ = AbsoluteQ1000(walkerDx);
            _ = AbsoluteQ1000(surveyorDx);
        }

        private WalkerBehaviorSnapshot StageWalker(RegularEnemyBehaviorPairInput input, SimulationTick nextTick, out WalkerBehaviorPhase phase, out int endTick, out bool isDead, out int dashDirection, out long ordinal, out WalkerRecoveryIntent? recovery)
        {
            phase = _walkerPhase;
            endTick = _walkerEndTick;
            isDead = _walkerDead || !input.WalkerFinalAlive;
            dashDirection = _walkerDashDirection;
            ordinal = _walkerAttackCycleOrdinal;
            recovery = null;
            var direction = Direction(input.PlayerXQ1000, input.WalkerXQ1000);
            var guard = input.WalkerFinalAlive && !input.WalkerReaction.ReactionAllowsBasicAttack;

            if (isDead)
            {
                phase = WalkerBehaviorPhase.Dead; endTick = -1; dashDirection = 0;
                return new WalkerBehaviorSnapshot(phase, endTick, false, RegularEnemyMovementIntent.None, 0, false, false, 0, ordinal);
            }

            if (phase == WalkerBehaviorPhase.Reset) phase = WalkerBehaviorPhase.Approach;
            if (phase == WalkerBehaviorPhase.Recovery && input.Tick.Value >= endTick) phase = WalkerBehaviorPhase.Approach;
            if (phase == WalkerBehaviorPhase.DashTelegraph && input.Tick.Value >= endTick)
            {
                phase = WalkerBehaviorPhase.DashActive;
                endTick = checked(input.Tick.Value + 12);
            }
            else if (phase == WalkerBehaviorPhase.DashActive && input.Tick.Value >= endTick)
            {
                phase = WalkerBehaviorPhase.Recovery;
                endTick = checked(input.Tick.Value + RecoveryDuration(input.WalkerReaction.RecoveryMultiplierQ1000));
            }

            if (phase == WalkerBehaviorPhase.Approach)
            {
                if (input.WalkerHasLineOfSight && AbsoluteQ1000((long)input.PlayerXQ1000 - input.WalkerXQ1000) <= 4000L)
                {
                    phase = WalkerBehaviorPhase.DashTelegraph;
                    endTick = checked(input.Tick.Value + 18);
                    dashDirection = direction;
                    return new WalkerBehaviorSnapshot(phase, endTick, true, RegularEnemyMovementIntent.None, 0, guard, true, dashDirection, ordinal);
                }
                endTick = -1;
                return new WalkerBehaviorSnapshot(phase, endTick, true, RegularEnemyMovementIntent.Horizontal, direction, guard, false, 0, ordinal);
            }

            if (phase == WalkerBehaviorPhase.DashTelegraph)
                return new WalkerBehaviorSnapshot(phase, endTick, true, RegularEnemyMovementIntent.None, 0, guard, true, dashDirection, ordinal);

            if (phase == WalkerBehaviorPhase.DashActive)
            {
                if (checked(input.Tick.Value + 1) == endTick)
                {
                    var nextOrdinal = checked(ordinal + 1);
                    recovery = new WalkerRecoveryIntent(WalkerId, ordinal, nextTick);
                    ordinal = nextOrdinal;
                }
                return new WalkerBehaviorSnapshot(phase, endTick, true, RegularEnemyMovementIntent.DashHorizontal, dashDirection, guard, false, dashDirection, ordinal);
            }

            return new WalkerBehaviorSnapshot(WalkerBehaviorPhase.Recovery, endTick, true, RegularEnemyMovementIntent.None, 0, guard, false, 0, ordinal);
        }

        private SurveyorBehaviorSnapshot StageSurveyor(RegularEnemyBehaviorPairInput input, out SurveyorBehaviorPhase phase, out int endTick, out bool isDead, out bool hasLatchedTarget, out int latchedTargetX, out long ordinal, out SurveyorShotIntent? shot)
        {
            phase = _surveyorPhase;
            endTick = _surveyorEndTick;
            isDead = _surveyorDead || !input.SurveyorFinalAlive;
            hasLatchedTarget = _surveyorHasLatchedTarget;
            latchedTargetX = _surveyorLatchedTargetXQ1000;
            ordinal = _surveyorShotOrdinal;
            shot = null;

            if (isDead)
            {
                phase = SurveyorBehaviorPhase.Dead; endTick = -1; hasLatchedTarget = false; latchedTargetX = 0;
                return new SurveyorBehaviorSnapshot(phase, endTick, false, RegularEnemyMovementIntent.None, 0, false, false, 0, ordinal);
            }

            if (input.SurveyorReaction.ForcedDescent || !input.SurveyorReaction.CanFire)
            {
                phase = SurveyorBehaviorPhase.Suppressed; endTick = -1; hasLatchedTarget = false; latchedTargetX = 0;
                return new SurveyorBehaviorSnapshot(phase, endTick, true, input.SurveyorReaction.ForcedDescent ? RegularEnemyMovementIntent.Downward : RegularEnemyMovementIntent.None, 0, false, false, 0, ordinal);
            }

            if (phase == SurveyorBehaviorPhase.Reset || phase == SurveyorBehaviorPhase.Suppressed)
            {
                phase = SurveyorBehaviorPhase.Relocate;
                endTick = checked(input.Tick.Value + 48);
            }
            else if (phase == SurveyorBehaviorPhase.Relocate && endTick < 0)
            {
                // REQ-COM-002: the constructor begins at the first Relocate entry, not at its completion boundary.
                endTick = checked(input.Tick.Value + 48);
            }
            else if (phase == SurveyorBehaviorPhase.Relocate && input.Tick.Value >= endTick)
            {
                phase = SurveyorBehaviorPhase.FireTelegraph;
                endTick = checked(input.Tick.Value + 30);
                hasLatchedTarget = true;
                latchedTargetX = input.PlayerXQ1000;
            }
            else if (phase == SurveyorBehaviorPhase.FireTelegraph && input.Tick.Value >= endTick)
            {
                phase = SurveyorBehaviorPhase.Fire;
                endTick = checked(input.Tick.Value + 1);
            }
            else if (phase == SurveyorBehaviorPhase.Fire && input.Tick.Value >= endTick)
            {
                phase = SurveyorBehaviorPhase.Cooldown;
                endTick = checked(input.Tick.Value + 60);
                hasLatchedTarget = false;
                latchedTargetX = 0;
            }
            else if (phase == SurveyorBehaviorPhase.Cooldown && input.Tick.Value >= endTick)
            {
                phase = SurveyorBehaviorPhase.Relocate;
                endTick = checked(input.Tick.Value + 48);
            }

            if (phase == SurveyorBehaviorPhase.Relocate)
                return new SurveyorBehaviorSnapshot(phase, endTick, true, RegularEnemyMovementIntent.Horizontal, (ordinal & 1L) == 0 ? -1 : 1, false, false, 0, ordinal);
            if (phase == SurveyorBehaviorPhase.FireTelegraph)
                return new SurveyorBehaviorSnapshot(phase, endTick, true, RegularEnemyMovementIntent.None, 0, true, hasLatchedTarget, latchedTargetX, ordinal);
            if (phase == SurveyorBehaviorPhase.Fire)
            {
                if (!hasLatchedTarget) throw new InvalidOperationException("Fire requires a previously latched target.");
                var nextOrdinal = checked(ordinal + 1);
                shot = new SurveyorShotIntent(SurveyorId, ordinal, latchedTargetX);
                ordinal = nextOrdinal;
                return new SurveyorBehaviorSnapshot(phase, endTick, true, RegularEnemyMovementIntent.None, 0, false, true, latchedTargetX, ordinal);
            }
            return new SurveyorBehaviorSnapshot(SurveyorBehaviorPhase.Cooldown, endTick, true, RegularEnemyMovementIntent.None, 0, false, false, 0, ordinal);
        }

        private static void ValidateGameplayReaction(RegularEnemyReactionSnapshot value, string targetId, SimulationTick tick, SimulationTick nextTick, bool walker)
        {
            if (!string.Equals(value.TargetId, targetId, StringComparison.Ordinal) || value.Tick != tick || value.NextExpectedTick != nextTick)
                throw new ArgumentException("M3A snapshot must have the literal role and exact tick horizon.");
            if (value.LastTransferRevision < -1 || value.LastTransferRevision == 0 || value.ShieldWindowEndTick < -1 || value.FireLockEndTick < -1)
                throw new ArgumentOutOfRangeException(nameof(value));
            if (value.IsHeavy && value.LastTransferRevision < 1) throw new InvalidOperationException("Heavy requires a positive transfer revision.");
            if (walker)
            {
                if (value.RecoveryMultiplierQ1000 != 1000 && value.RecoveryMultiplierQ1000 != 1500) throw new ArgumentOutOfRangeException(nameof(value));
                if (!value.IsAlive && (value.ReactionAllowsBasicAttack || value.RecoveryMultiplierQ1000 != 1000)) throw new InvalidOperationException("Dead walker reaction fields are malformed.");
                if (value.IsAlive && value.RecoveryMultiplierQ1000 != (value.IsHeavy ? 1500 : 1000))
                    throw new InvalidOperationException("Live walker recovery multiplier must derive exactly from Heavy.");
                var expectedAttackability = value.IsAlive && value.ShieldWindowEndTick >= 0 && tick.Value < value.ShieldWindowEndTick;
                if (value.ReactionAllowsBasicAttack != expectedAttackability) throw new InvalidOperationException("Walker attackability must derive from its exclusive shield end.");
            }
            else
            {
                if (value.ReactionAllowsBasicAttack || value.RecoveryMultiplierQ1000 != 1000) throw new InvalidOperationException("Surveyor cannot own walker reaction fields.");
                if (!value.IsAlive && (value.ForcedDescent || value.CanFire)) throw new InvalidOperationException("Dead surveyor reaction fields are malformed.");
                if (value.IsAlive && (value.ForcedDescent != value.IsHeavy || value.CanFire != (!value.IsHeavy && !(value.FireLockEndTick >= 0 && tick.Value < value.FireLockEndTick))))
                    throw new InvalidOperationException("Surveyor fire fields must derive from Heavy and the exclusive fire-lock end.");
            }
        }

        private static void ValidateResetReaction(RegularEnemyReactionSnapshot value, string targetId, SimulationTick tick, SimulationTick nextTick)
        {
            if (!string.Equals(value.TargetId, targetId, StringComparison.Ordinal) || value.Tick != tick || value.NextExpectedTick != nextTick ||
                !value.IsAlive || value.IsHeavy || value.LastTransferRevision != -1 || value.ShieldWindowEndTick != -1 || value.FireLockEndTick != -1 ||
                value.ReactionAllowsBasicAttack || value.RecoveryMultiplierQ1000 != 1000 || value.ForcedDescent || value.CanFire)
                throw new InvalidOperationException("Encounter reset requires exact empty M3A reset publications.");
        }

        private static int RecoveryDuration(int multiplierQ1000)
        {
            if (multiplierQ1000 != 1000 && multiplierQ1000 != 1500) throw new ArgumentOutOfRangeException(nameof(multiplierQ1000));
            return checked((40 * multiplierQ1000 + 999) / 1000);
        }

        private static long AbsoluteQ1000(long value) => value < 0 ? checked(-value) : value;
        private static int Direction(int playerXQ1000, int enemyXQ1000) => playerXQ1000 > enemyXQ1000 ? 1 : playerXQ1000 < enemyXQ1000 ? -1 : 0;
    }
}
