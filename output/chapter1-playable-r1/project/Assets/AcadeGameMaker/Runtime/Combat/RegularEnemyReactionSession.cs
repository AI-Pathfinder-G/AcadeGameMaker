using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat
{
    internal enum RegularEnemyTransferState
    {
        Baseline = 0,
        Heavy = 1
    }

    internal enum WalkerReactionEventKind
    {
        None = 0,
        AttackRecoveryStarted = 1,
        HeavyImpactApplied = 2
    }

    internal enum RegularEnemyReactionInputKind
    {
        Gameplay = 0,
        EncounterReset = 1
    }

    internal readonly struct RegularEnemyTransferTransition
    {
        internal RegularEnemyTransferTransition(string targetId, RegularEnemyTransferState state, int revision, SimulationTick tick)
        {
            DamageValidation.Id(targetId, nameof(targetId));
            if (state != RegularEnemyTransferState.Baseline && state != RegularEnemyTransferState.Heavy) throw new ArgumentOutOfRangeException(nameof(state));
            if (revision < 1) throw new ArgumentOutOfRangeException(nameof(revision));
            if (tick.Value < 0) throw new ArgumentOutOfRangeException(nameof(tick));
            TargetId = targetId;
            State = state;
            Revision = revision;
            Tick = tick;
        }

        internal string TargetId { get; }
        internal RegularEnemyTransferState State { get; }
        internal int Revision { get; }
        internal SimulationTick Tick { get; }
    }

    internal readonly struct WalkerReactionEvent
    {
        internal WalkerReactionEvent(WalkerReactionEventKind kind, long? attackCycleId, string requestId)
        {
            if (kind < WalkerReactionEventKind.None || kind > WalkerReactionEventKind.HeavyImpactApplied) throw new ArgumentOutOfRangeException(nameof(kind));
            switch (kind)
            {
                case WalkerReactionEventKind.None:
                    if (attackCycleId.HasValue || requestId != null) throw new ArgumentException("A None walker event cannot carry an ID.");
                    break;
                case WalkerReactionEventKind.AttackRecoveryStarted:
                    if (!attackCycleId.HasValue || attackCycleId.Value < 0 || requestId != null) throw new ArgumentException("Attack recovery requires only a nonnegative cycle ID.");
                    break;
                case WalkerReactionEventKind.HeavyImpactApplied:
                    if (attackCycleId.HasValue) throw new ArgumentException("Heavy impact cannot carry an attack-cycle ID.");
                    DamageValidation.Id(requestId, nameof(requestId));
                    break;
            }

            Kind = kind;
            AttackCycleId = attackCycleId;
            RequestId = requestId ?? string.Empty;
        }

        internal static WalkerReactionEvent None => new WalkerReactionEvent(WalkerReactionEventKind.None, null, null);
        internal static WalkerReactionEvent AttackRecoveryStarted(long attackCycleId) => new WalkerReactionEvent(WalkerReactionEventKind.AttackRecoveryStarted, attackCycleId, null);
        internal static WalkerReactionEvent HeavyImpactApplied(string requestId) => new WalkerReactionEvent(WalkerReactionEventKind.HeavyImpactApplied, null, requestId);

        internal WalkerReactionEventKind Kind { get; }
        internal long? AttackCycleId { get; }
        internal string RequestId { get; }
    }

    internal readonly struct RegularEnemyReactionInput
    {
        private RegularEnemyReactionInput(
            RegularEnemyReactionInputKind kind,
            SimulationTick tick,
            string targetId,
            bool? finalAliveAfterM1,
            RegularEnemyTransferTransition? transferTransition,
            WalkerReactionEvent walkerEvent)
        {
            if (kind != RegularEnemyReactionInputKind.Gameplay && kind != RegularEnemyReactionInputKind.EncounterReset) throw new ArgumentOutOfRangeException(nameof(kind));
            if (tick.Value < 0) throw new ArgumentOutOfRangeException(nameof(tick));
            if (kind == RegularEnemyReactionInputKind.Gameplay)
            {
                DamageValidation.Id(targetId, nameof(targetId));
                if (!finalAliveAfterM1.HasValue) throw new ArgumentException("Gameplay input requires the final M1 alive state.");
                if (transferTransition.HasValue && (transferTransition.Value.Tick != tick || !string.Equals(transferTransition.Value.TargetId, targetId, StringComparison.Ordinal)))
                    throw new ArgumentException("Transfer transition must match the gameplay target and tick.");
                ValidateWalkerEvent(walkerEvent);
            }
            else
            {
                if (targetId != null || finalAliveAfterM1.HasValue || transferTransition.HasValue || walkerEvent.Kind != WalkerReactionEventKind.None || walkerEvent.AttackCycleId.HasValue || !string.IsNullOrEmpty(walkerEvent.RequestId))
                    throw new ArgumentException("Encounter reset cannot carry gameplay payload.");
            }

            Kind = kind;
            Tick = tick;
            TargetId = targetId ?? string.Empty;
            FinalAliveAfterM1 = finalAliveAfterM1;
            TransferTransition = transferTransition;
            WalkerEvent = walkerEvent;
        }

        internal static RegularEnemyReactionInput Gameplay(SimulationTick tick, string targetId, bool finalAliveAfterM1, RegularEnemyTransferTransition? transferTransition, WalkerReactionEvent walkerEvent) =>
            new RegularEnemyReactionInput(RegularEnemyReactionInputKind.Gameplay, tick, targetId, finalAliveAfterM1, transferTransition, walkerEvent);

        internal static RegularEnemyReactionInput EncounterReset(SimulationTick tick) =>
            new RegularEnemyReactionInput(RegularEnemyReactionInputKind.EncounterReset, tick, null, null, null, WalkerReactionEvent.None);

        internal RegularEnemyReactionInputKind Kind { get; }
        internal SimulationTick Tick { get; }
        internal string TargetId { get; }
        internal bool? FinalAliveAfterM1 { get; }
        internal RegularEnemyTransferTransition? TransferTransition { get; }
        internal WalkerReactionEvent WalkerEvent { get; }

        private static void ValidateWalkerEvent(WalkerReactionEvent value)
        {
            _ = new WalkerReactionEvent(value.Kind, value.AttackCycleId, value.Kind == WalkerReactionEventKind.HeavyImpactApplied ? value.RequestId : null);
        }
    }

    internal readonly struct RegularEnemyReactionSnapshot
    {
        internal RegularEnemyReactionSnapshot(
            SimulationTick tick,
            string targetId,
            bool isAlive,
            bool isHeavy,
            int lastTransferRevision,
            int shieldWindowEndTick,
            int fireLockEndTick,
            bool reactionAllowsBasicAttack,
            int recoveryMultiplierQ1000,
            bool forcedDescent,
            bool canFire,
            SimulationTick nextExpectedTick)
        {
            DamageValidation.Id(targetId, nameof(targetId));
            if (tick.Value < 0 || nextExpectedTick.Value != checked(tick.Value + 1)) throw new ArgumentException("Snapshot ticks must be consecutive and nonnegative.");
            if (lastTransferRevision < -1 || shieldWindowEndTick < -1 || fireLockEndTick < -1) throw new ArgumentOutOfRangeException();
            if (recoveryMultiplierQ1000 != 1000 && recoveryMultiplierQ1000 != 1500) throw new ArgumentOutOfRangeException(nameof(recoveryMultiplierQ1000));
            Tick = tick;
            TargetId = targetId;
            IsAlive = isAlive;
            IsHeavy = isHeavy;
            LastTransferRevision = lastTransferRevision;
            ShieldWindowEndTick = shieldWindowEndTick;
            FireLockEndTick = fireLockEndTick;
            ReactionAllowsBasicAttack = reactionAllowsBasicAttack;
            RecoveryMultiplierQ1000 = recoveryMultiplierQ1000;
            ForcedDescent = forcedDescent;
            CanFire = canFire;
            NextExpectedTick = nextExpectedTick;
        }

        internal SimulationTick Tick { get; }
        internal string TargetId { get; }
        internal bool IsAlive { get; }
        internal bool IsHeavy { get; }
        internal int LastTransferRevision { get; }
        internal int ShieldWindowEndTick { get; }
        internal int FireLockEndTick { get; }
        internal bool ReactionAllowsBasicAttack { get; }
        internal int RecoveryMultiplierQ1000 { get; }
        internal bool ForcedDescent { get; }
        internal bool CanFire { get; }
        internal SimulationTick NextExpectedTick { get; }
    }

    // REQ-COM-001, REQ-COM-002, REQ-COM-004, REQ-COM-005; affected REQ-WT-003, REQ-WT-005, REQ-WT-007.
    // Engine-free owner of regular-enemy reaction windows only; health and active transfer remain external authorities.
    internal sealed class RegularEnemyReactionSession
    {
        private const string WalkerTargetId = "walker";
        private const string SurveyorTargetId = "surveyor";
        private readonly string _targetId;
        private readonly bool _isWalker;
        private SimulationTick _nextExpectedTick;
        private bool _isDead;
        private bool _isHeavy;
        private bool _hasCompletedGameplay;
        private int _lastTransferRevision = -1;
        private int _shieldWindowEndTick = -1;
        private int _fireLockEndTick = -1;
        private HashSet<string> _consumedImpactRequestIds = new HashSet<string>(StringComparer.Ordinal);
        private HashSet<long> _consumedAttackCycleIds = new HashSet<long>();
        private RegularEnemyReactionSnapshot? _latestSnapshot;

        internal RegularEnemyReactionSession(string targetId, SimulationTick firstExpectedTick)
        {
            if (!string.Equals(targetId, WalkerTargetId, StringComparison.Ordinal) && !string.Equals(targetId, SurveyorTargetId, StringComparison.Ordinal))
                throw new ArgumentException("Regular enemy reaction target must be the literal walker or surveyor ID.", nameof(targetId));
            if (firstExpectedTick.Value < 0) throw new ArgumentOutOfRangeException(nameof(firstExpectedTick));
            _targetId = targetId;
            _isWalker = string.Equals(targetId, WalkerTargetId, StringComparison.Ordinal);
            _nextExpectedTick = firstExpectedTick;
        }

        internal SimulationTick NextExpectedTick => _nextExpectedTick;
        internal RegularEnemyReactionSnapshot? LatestSnapshot => _latestSnapshot;

        // REQ-COM-004, REQ-COM-005. M3B validates both reaction inputs after M1 and
        // before either session commits.  The clone deliberately exercises the exact
        // same checked arithmetic, discriminants, revision guards and tombstones as
        // Process while preserving every observable owner field on this session.
        internal void ValidateNext(RegularEnemyReactionInput input)
        {
            var staged = new RegularEnemyReactionSession(_targetId, _nextExpectedTick)
            {
                _isDead = _isDead,
                _isHeavy = _isHeavy,
                _hasCompletedGameplay = _hasCompletedGameplay,
                _lastTransferRevision = _lastTransferRevision,
                _shieldWindowEndTick = _shieldWindowEndTick,
                _fireLockEndTick = _fireLockEndTick,
                _consumedImpactRequestIds = new HashSet<string>(_consumedImpactRequestIds, StringComparer.Ordinal),
                _consumedAttackCycleIds = new HashSet<long>(_consumedAttackCycleIds),
                _latestSnapshot = _latestSnapshot
            };
            _ = staged.Process(input);
        }

        internal RegularEnemyReactionSnapshot Process(RegularEnemyReactionInput input)
        {
            if (input.Tick != _nextExpectedTick) throw new InvalidOperationException("Reaction input must use the exact next global simulation tick.");
            if (input.Kind != RegularEnemyReactionInputKind.Gameplay && input.Kind != RegularEnemyReactionInputKind.EncounterReset)
                throw new ArgumentOutOfRangeException(nameof(input), "Reaction input kind is outside the frozen discriminants.");
            return input.Kind == RegularEnemyReactionInputKind.EncounterReset ? ProcessReset(input) : ProcessGameplay(input);
        }

        private RegularEnemyReactionSnapshot ProcessReset(RegularEnemyReactionInput input)
        {
            if (!_hasCompletedGameplay) throw new InvalidOperationException("Encounter reset requires a completed gameplay tick in this encounter.");
            if (_isHeavy) throw new InvalidOperationException("Encounter reset requires the mirrored transfer state to be baseline.");
            var nextTick = new SimulationTick(checked(input.Tick.Value + 1));
            var snapshot = new RegularEnemyReactionSnapshot(input.Tick, _targetId, true, false, -1, -1, -1, false, 1000, false, false, nextTick);
            var emptyImpactIds = new HashSet<string>(StringComparer.Ordinal);
            var emptyCycleIds = new HashSet<long>();

            _isDead = false;
            _isHeavy = false;
            _hasCompletedGameplay = false;
            _lastTransferRevision = -1;
            _shieldWindowEndTick = -1;
            _fireLockEndTick = -1;
            _consumedImpactRequestIds = emptyImpactIds;
            _consumedAttackCycleIds = emptyCycleIds;
            _latestSnapshot = snapshot;
            _nextExpectedTick = nextTick;
            return snapshot;
        }

        private RegularEnemyReactionSnapshot ProcessGameplay(RegularEnemyReactionInput input)
        {
            if (!string.Equals(input.TargetId, _targetId, StringComparison.Ordinal)) throw new ArgumentException("Reaction input target does not match its session.", nameof(input));
            var finalAlive = input.FinalAliveAfterM1.Value;
            if (_isDead && finalAlive) throw new InvalidOperationException("A dead enemy cannot become alive without encounter reset.");
            if (!_isWalker && input.WalkerEvent.Kind != WalkerReactionEventKind.None) throw new ArgumentException("Surveyor input cannot carry a walker event.", nameof(input));
            if (!finalAlive && input.WalkerEvent.Kind != WalkerReactionEventKind.None) throw new ArgumentException("Dead-tick walker events are forbidden.", nameof(input));

            var transition = input.TransferTransition;
            PreflightTransition(transition, finalAlive);
            var nextTick = new SimulationTick(checked(input.Tick.Value + 1));

            var proposedShieldEnd = -1;
            var proposedFireLockEnd = -1;
            if (finalAlive && _isWalker)
            {
                if (input.WalkerEvent.Kind == WalkerReactionEventKind.AttackRecoveryStarted) proposedShieldEnd = checked(input.Tick.Value + 30);
                if (input.WalkerEvent.Kind == WalkerReactionEventKind.HeavyImpactApplied) proposedShieldEnd = checked(input.Tick.Value + 90);
            }
            else if (finalAlive && !_isWalker && transition.HasValue && transition.Value.State == RegularEnemyTransferState.Heavy)
            {
                proposedFireLockEnd = checked(input.Tick.Value + 75);
            }

            PreflightEventDedupe(input.WalkerEvent);

            var stagedHeavy = _isHeavy;
            var stagedRevision = _lastTransferRevision;
            if (transition.HasValue)
            {
                stagedHeavy = transition.Value.State == RegularEnemyTransferState.Heavy;
                stagedRevision = transition.Value.Revision;
            }

            var stagedShieldEnd = proposedShieldEnd < 0 ? _shieldWindowEndTick : Math.Max(_shieldWindowEndTick, proposedShieldEnd);
            var stagedFireLockEnd = proposedFireLockEnd < 0 ? _fireLockEndTick : Math.Max(_fireLockEndTick, proposedFireLockEnd);
            var stagedDead = _isDead || !finalAlive;
            var stagedImpactIds = _consumedImpactRequestIds;
            var stagedCycleIds = _consumedAttackCycleIds;
            if (input.WalkerEvent.Kind == WalkerReactionEventKind.HeavyImpactApplied)
            {
                stagedImpactIds = new HashSet<string>(_consumedImpactRequestIds, StringComparer.Ordinal) { input.WalkerEvent.RequestId };
            }
            else if (input.WalkerEvent.Kind == WalkerReactionEventKind.AttackRecoveryStarted)
            {
                stagedCycleIds = new HashSet<long>(_consumedAttackCycleIds) { input.WalkerEvent.AttackCycleId.Value };
            }

            var effectiveAlive = !stagedDead;
            var allowsBasicAttack = _isWalker && effectiveAlive && stagedShieldEnd >= 0 && input.Tick.Value < stagedShieldEnd;
            var recoveryMultiplier = _isWalker && effectiveAlive && stagedHeavy ? 1500 : 1000;
            var forcedDescent = !_isWalker && effectiveAlive && stagedHeavy;
            // REQ-COM-005, AC-COM-001. Baseline recall clears forced descent only;
            // the historical exclusive overheat end remains effective through tick end-1.
            var residualFireLockActive = stagedFireLockEnd >= 0 && input.Tick.Value < stagedFireLockEnd;
            var canFire = !_isWalker && effectiveAlive && !stagedHeavy && !residualFireLockActive;
            var snapshot = new RegularEnemyReactionSnapshot(
                input.Tick, _targetId, effectiveAlive, stagedHeavy, stagedRevision, stagedShieldEnd, stagedFireLockEnd,
                allowsBasicAttack, recoveryMultiplier, forcedDescent, canFire, nextTick);

            _isDead = stagedDead;
            _isHeavy = stagedHeavy;
            _hasCompletedGameplay = true;
            _lastTransferRevision = stagedRevision;
            _shieldWindowEndTick = stagedShieldEnd;
            _fireLockEndTick = stagedFireLockEnd;
            _consumedImpactRequestIds = stagedImpactIds;
            _consumedAttackCycleIds = stagedCycleIds;
            _latestSnapshot = snapshot;
            _nextExpectedTick = nextTick;
            return snapshot;
        }

        private void PreflightTransition(RegularEnemyTransferTransition? transition, bool finalAlive)
        {
            if (!transition.HasValue) return;
            var value = transition.Value;
            if (value.Tick != _nextExpectedTick || !string.Equals(value.TargetId, _targetId, StringComparison.Ordinal)) throw new ArgumentException("Transfer transition target and tick must match the session.", nameof(transition));
            if (value.Revision <= _lastTransferRevision) throw new InvalidOperationException("Transfer revisions must increase strictly within an encounter.");
            var proposedHeavy = value.State == RegularEnemyTransferState.Heavy;
            if (proposedHeavy == _isHeavy) throw new InvalidOperationException("Transfer transition must be an actual Baseline/Heavy edge.");
            if (_isDead && (finalAlive || value.State != RegularEnemyTransferState.Baseline || !_isHeavy))
                throw new InvalidOperationException("A dead session accepts only the expected higher-revision Baseline cleanup.");
        }

        private void PreflightEventDedupe(WalkerReactionEvent walkerEvent)
        {
            if (walkerEvent.Kind == WalkerReactionEventKind.HeavyImpactApplied && _consumedImpactRequestIds.Contains(walkerEvent.RequestId))
                throw new InvalidOperationException("Heavy-impact request ID was already consumed in this encounter.");
            if (walkerEvent.Kind == WalkerReactionEventKind.AttackRecoveryStarted && _consumedAttackCycleIds.Contains(walkerEvent.AttackCycleId.Value))
                throw new InvalidOperationException("Attack-cycle ID was already consumed in this encounter.");
        }
    }
}
