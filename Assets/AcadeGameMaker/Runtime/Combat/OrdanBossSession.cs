using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat
{
    // REQ-COM-003, REQ-COM-004, REQ-COM-006. M4A is an engine-free, internal
    // projection owner; Combat and Transfer retain their existing authority.
    internal enum OrdanPatternSlot { None = 0, DebtLineA = 1, SeizureWeight = 2, DebtLineB = 3, BalanceAudit = 4 }
    internal enum OrdanBossPhase { Telegraph = 0, Execute = 1, Recovery = 2, Vulnerable = 3, Defeated = 4 }
    internal enum OrdanHealthStage { None = 0, Stage1 = 1, Stage2 = 2, Stage3 = 3 }
    internal enum OrdanTransferTargetKind { BossPayload = 0, Box = 1 }
    internal enum OrdanTransferState { Baseline = 0, Heavy = 1 }
    internal enum OrdanVulnerabilitySource { None = 0, SeizureWeightTransfer = 1, BalanceAuditInterrupt = 2 }
    internal enum OrdanBossIntentKind { DebtLine = 0, PayloadAcceleration = 1, PayloadDamageRequest = 2, BalanceAuditPull = 3, BalanceAuditShockwave = 4, BalanceAuditInterrupt = 5 }
    internal enum OrdanBossHandoffKind { Defeated = 0, RewardRequest = 1, RoomCompletionRequest = 2 }

    internal readonly struct OrdanCombatState
    {
        internal OrdanCombatState(SimulationTick tick, string targetId, int currentHealth, bool isDead, DamageResult? pendingPayloadDamageResult)
        {
            if (tick.Value < 0 || currentHealth < 0 || currentHealth > 60 || isDead != (currentHealth == 0)) throw new ArgumentException(nameof(currentHealth));
            DamageValidation.Id(targetId, nameof(targetId));
            Tick = tick; TargetId = targetId; CurrentHealth = currentHealth; IsDead = isDead; PendingPayloadDamageResult = pendingPayloadDamageResult;
        }
        internal SimulationTick Tick { get; }
        internal string TargetId { get; }
        internal int CurrentHealth { get; }
        internal bool IsDead { get; }
        internal DamageResult? PendingPayloadDamageResult { get; }
    }

    internal readonly struct OrdanRelevantTransferEdge
    {
        internal OrdanRelevantTransferEdge(SimulationTick tick, string targetId, OrdanTransferTargetKind targetKind, OrdanTransferState previousState, OrdanTransferState nextState, int revision)
        {
            if (tick.Value < 0 || revision < 0 || targetKind < OrdanTransferTargetKind.BossPayload || targetKind > OrdanTransferTargetKind.Box ||
                previousState < OrdanTransferState.Baseline || previousState > OrdanTransferState.Heavy || nextState < OrdanTransferState.Baseline || nextState > OrdanTransferState.Heavy)
                throw new ArgumentOutOfRangeException(nameof(revision));
            DamageValidation.Id(targetId, nameof(targetId));
            Tick = tick; TargetId = targetId; TargetKind = targetKind; PreviousState = previousState; NextState = nextState; Revision = revision;
        }
        internal SimulationTick Tick { get; }
        internal string TargetId { get; }
        internal OrdanTransferTargetKind TargetKind { get; }
        internal OrdanTransferState PreviousState { get; }
        internal OrdanTransferState NextState { get; }
        internal int Revision { get; }
    }

    internal readonly struct OrdanBossTickInput
    {
        internal OrdanBossTickInput(SimulationTick tick, OrdanCombatState combatState, OrdanRelevantTransferEdge? relevantTransferEdge)
        {
            if (tick.Value < 0 || combatState.Tick != tick || (relevantTransferEdge.HasValue && relevantTransferEdge.Value.Tick != tick)) throw new ArgumentException(nameof(tick));
            Tick = tick; CombatState = combatState; RelevantTransferEdge = relevantTransferEdge;
        }
        internal SimulationTick Tick { get; }
        internal OrdanCombatState CombatState { get; }
        internal OrdanRelevantTransferEdge? RelevantTransferEdge { get; }
    }

    internal readonly struct OrdanBossSnapshot : IEquatable<OrdanBossSnapshot>
    {
        internal OrdanBossSnapshot(SimulationTick tick, OrdanPatternSlot patternSlot, OrdanBossPhase phase, int phaseAge, OrdanHealthStage activeStage,
            OrdanHealthStage pendingStage, string activeHandleId, bool isAccelerated, bool isVulnerable, OrdanVulnerabilitySource vulnerabilitySource, SimulationTick nextExpectedTick)
        {
            if (tick.Value < 0 || nextExpectedTick.Value < 0 || phaseAge < 0 || patternSlot < OrdanPatternSlot.None || patternSlot > OrdanPatternSlot.BalanceAudit ||
                phase < OrdanBossPhase.Telegraph || phase > OrdanBossPhase.Defeated || activeStage < OrdanHealthStage.None || activeStage > OrdanHealthStage.Stage3 ||
                pendingStage < OrdanHealthStage.None || pendingStage > OrdanHealthStage.Stage3 || vulnerabilitySource < OrdanVulnerabilitySource.None || vulnerabilitySource > OrdanVulnerabilitySource.BalanceAuditInterrupt)
                throw new ArgumentOutOfRangeException(nameof(phase));
            Tick = tick; PatternSlot = patternSlot; Phase = phase; PhaseAge = phaseAge; ActiveStage = activeStage; PendingStage = pendingStage;
            ActiveHandleId = activeHandleId ?? string.Empty; IsAccelerated = isAccelerated; IsVulnerable = isVulnerable; VulnerabilitySource = vulnerabilitySource; NextExpectedTick = nextExpectedTick;
        }
        internal SimulationTick Tick { get; }
        internal OrdanPatternSlot PatternSlot { get; }
        internal OrdanBossPhase Phase { get; }
        internal int PhaseAge { get; }
        internal OrdanHealthStage ActiveStage { get; }
        internal OrdanHealthStage PendingStage { get; }
        internal string ActiveHandleId { get; }
        internal bool IsAccelerated { get; }
        internal bool IsVulnerable { get; }
        internal OrdanVulnerabilitySource VulnerabilitySource { get; }
        internal SimulationTick NextExpectedTick { get; }
        public bool Equals(OrdanBossSnapshot other) => Tick == other.Tick && PatternSlot == other.PatternSlot && Phase == other.Phase && PhaseAge == other.PhaseAge && ActiveStage == other.ActiveStage && PendingStage == other.PendingStage &&
            ActiveHandleId == other.ActiveHandleId && IsAccelerated == other.IsAccelerated && IsVulnerable == other.IsVulnerable && VulnerabilitySource == other.VulnerabilitySource && NextExpectedTick == other.NextExpectedTick;
        public override bool Equals(object obj) => obj is OrdanBossSnapshot && Equals((OrdanBossSnapshot)obj);
        public override int GetHashCode() => Tick.Value;
    }

    internal readonly struct OrdanBossIntent : IEquatable<OrdanBossIntent>
    {
        internal OrdanBossIntent(OrdanBossIntentKind kind, string id, OrdanPatternSlot patternSlot, SimulationTick tick, int damageAmount, SimulationTick? deliveryTick, DamageRequest? futureDamageRequest)
        {
            if (kind < OrdanBossIntentKind.DebtLine || kind > OrdanBossIntentKind.BalanceAuditInterrupt || tick.Value < 0 || damageAmount < 0) throw new ArgumentOutOfRangeException(nameof(kind));
            DamageValidation.Id(id, nameof(id));
            Kind = kind; Id = id; PatternSlot = patternSlot; Tick = tick; DamageAmount = damageAmount; DeliveryTick = deliveryTick; FutureDamageRequest = futureDamageRequest;
        }
        internal OrdanBossIntentKind Kind { get; }
        internal string Id { get; }
        internal OrdanPatternSlot PatternSlot { get; }
        internal SimulationTick Tick { get; }
        internal int DamageAmount { get; }
        internal SimulationTick? DeliveryTick { get; }
        internal DamageRequest? FutureDamageRequest { get; }
        public bool Equals(OrdanBossIntent other) => Kind == other.Kind && Id == other.Id && PatternSlot == other.PatternSlot && Tick == other.Tick && DamageAmount == other.DamageAmount && DeliveryTick == other.DeliveryTick && Nullable.Equals(FutureDamageRequest, other.FutureDamageRequest);
        public override bool Equals(object obj) => obj is OrdanBossIntent && Equals((OrdanBossIntent)obj);
        public override int GetHashCode() => Id.GetHashCode();
    }

    internal readonly struct OrdanBossHandoff : IEquatable<OrdanBossHandoff>
    {
        internal OrdanBossHandoff(OrdanBossHandoffKind kind, SimulationTick tick)
        {
            if (kind < OrdanBossHandoffKind.Defeated || kind > OrdanBossHandoffKind.RoomCompletionRequest || tick.Value < 0) throw new ArgumentOutOfRangeException(nameof(kind));
            Kind = kind; Tick = tick;
        }
        internal OrdanBossHandoffKind Kind { get; }
        internal SimulationTick Tick { get; }
        public bool Equals(OrdanBossHandoff other) => Kind == other.Kind && Tick == other.Tick;
        public override bool Equals(object obj) => obj is OrdanBossHandoff && Equals((OrdanBossHandoff)obj);
        public override int GetHashCode() => ((int)Kind * 397) ^ Tick.Value;
    }

    // REQ-COM-003, REQ-WT-005. M4A alone owns the one-tick Transfer exposure
    // forecast.  Unity consumers may forward it but must never reconstruct a
    // handle ordinal or phase duration from a snapshot.
    internal readonly struct OrdanTransferExposureForecast : IEquatable<OrdanTransferExposureForecast>
    {
        internal OrdanTransferExposureForecast(SimulationTick sourceTick, SimulationTick transferTick, string targetId)
        {
            if (sourceTick.Value < 0 || transferTick.Value != checked(sourceTick.Value + 1)) throw new ArgumentOutOfRangeException(nameof(transferTick));
            targetId = targetId ?? string.Empty;
            if (targetId.Length != 0 && targetId != "BossWeight-0" && targetId != "BossWeight-1" && targetId != "BossWeight-2")
                throw new ArgumentException("Exposure forecast must name one scripted payload or be empty.", nameof(targetId));
            SourceTick = sourceTick; TransferTick = transferTick; TargetId = targetId;
        }

        internal SimulationTick SourceTick { get; }
        internal SimulationTick TransferTick { get; }
        internal string TargetId { get; }
        internal bool IsExposed => TargetId.Length != 0;
        public bool Equals(OrdanTransferExposureForecast other) => SourceTick == other.SourceTick && TransferTick == other.TransferTick && TargetId == other.TargetId;
        public override bool Equals(object obj) => obj is OrdanTransferExposureForecast && Equals((OrdanTransferExposureForecast)obj);
        public override int GetHashCode() => ((SourceTick.Value * 397) ^ TransferTick.Value) ^ TargetId.GetHashCode();
    }

    // REQ-COM-003. M4B3B1's audit window is forecast only by M4A candidate state.
    internal readonly struct OrdanAuditExposureForecast : IEquatable<OrdanAuditExposureForecast>
    {
        internal OrdanAuditExposureForecast(SimulationTick sourceTick, SimulationTick transferTick, string targetId)
        {
            if (sourceTick.Value < 0 || transferTick.Value != checked(sourceTick.Value + 1)) throw new ArgumentOutOfRangeException(nameof(transferTick));
            targetId = targetId ?? string.Empty;
            if (targetId.Length != 0 && targetId != "BossAuditBox-0") throw new ArgumentException(nameof(targetId));
            SourceTick = sourceTick; TransferTick = transferTick; TargetId = targetId;
        }
        internal SimulationTick SourceTick { get; } internal SimulationTick TransferTick { get; } internal string TargetId { get; }
        internal bool IsExposed => TargetId.Length != 0;
        public bool Equals(OrdanAuditExposureForecast other) => SourceTick == other.SourceTick && TransferTick == other.TransferTick && TargetId == other.TargetId;
        public override bool Equals(object obj) => obj is OrdanAuditExposureForecast other && Equals(other);
        public override int GetHashCode() => ((SourceTick.Value * 397) ^ TransferTick.Value) ^ TargetId.GetHashCode();
    }

    // REQ-COM-002, REQ-COM-004. M4A's complete Execute-time trajectory
    // projection; Unity consumers must not reconstruct phase age or duration.
    internal readonly struct SeizureTrajectoryProjection : IEquatable<SeizureTrajectoryProjection>
    {
        internal SeizureTrajectoryProjection(SimulationTick sourceTick, string payloadId, long attackOrdinal, int elapsedTick, int frozenDuration, bool accelerated)
        {
            if (sourceTick.Value < 0 || (payloadId != "BossWeight-0" && payloadId != "BossWeight-1" && payloadId != "BossWeight-2") || attackOrdinal < 0 || elapsedTick < 0 || elapsedTick >= frozenDuration ||
                (accelerated ? frozenDuration != 24 && frozenDuration != 22 && frozenDuration != 20 : frozenDuration != 72 && frozenDuration != 66 && frozenDuration != 60)) throw new ArgumentException();
            SourceTick = sourceTick; PayloadId = payloadId; AttackOrdinal = attackOrdinal; ElapsedTick = elapsedTick; FrozenDuration = frozenDuration; Accelerated = accelerated;
        }
        internal SimulationTick SourceTick { get; } internal string PayloadId { get; } internal long AttackOrdinal { get; }
        internal int ElapsedTick { get; } internal int FrozenDuration { get; } internal bool Accelerated { get; }
        public bool Equals(SeizureTrajectoryProjection other) => SourceTick == other.SourceTick && PayloadId == other.PayloadId && AttackOrdinal == other.AttackOrdinal && ElapsedTick == other.ElapsedTick && FrozenDuration == other.FrozenDuration && Accelerated == other.Accelerated;
        public override bool Equals(object obj) => obj is SeizureTrajectoryProjection other && Equals(other);
        public override int GetHashCode() => SourceTick.Value ^ AttackOrdinal.GetHashCode() ^ ElapsedTick ^ FrozenDuration;
    }

    // REQ-COM-003, REQ-COM-004. M4A owns the full active BalanceAudit pull window.
    internal readonly struct BalanceAuditPullProjection : IEquatable<BalanceAuditPullProjection>
    {
        internal BalanceAuditPullProjection(
            SimulationTick sourceTick,
            long attackOrdinal,
            int elapsedTick,
            int frozenDuration)
        {
            if (sourceTick.Value < 0 || attackOrdinal < 0 || elapsedTick < 0 || elapsedTick >= frozenDuration ||
                (frozenDuration != 90 && frozenDuration != 82 && frozenDuration != 75))
            {
                throw new ArgumentException("BalanceAudit pull projection is invalid.");
            }

            SourceTick = sourceTick;
            AttackOrdinal = attackOrdinal;
            ElapsedTick = elapsedTick;
            FrozenDuration = frozenDuration;
        }

        internal SimulationTick SourceTick { get; }
        internal long AttackOrdinal { get; }
        internal int ElapsedTick { get; }
        internal int FrozenDuration { get; }

        public bool Equals(BalanceAuditPullProjection other) =>
            SourceTick == other.SourceTick &&
            AttackOrdinal == other.AttackOrdinal &&
            ElapsedTick == other.ElapsedTick &&
            FrozenDuration == other.FrozenDuration;

        public override bool Equals(object obj) => obj is BalanceAuditPullProjection other && Equals(other);
        public override int GetHashCode() => SourceTick.Value ^ AttackOrdinal.GetHashCode() ^ ElapsedTick ^ FrozenDuration;
    }

    internal readonly struct OrdanBossTickOutput : IEquatable<OrdanBossTickOutput>
    {
        internal OrdanBossTickOutput(OrdanBossSnapshot snapshot, IReadOnlyList<OrdanBossIntent> intents, IReadOnlyList<OrdanBossHandoff> handoffs,
            OrdanTransferExposureForecast transferExposureForecast)
            : this(snapshot, intents, handoffs, transferExposureForecast, new OrdanAuditExposureForecast(snapshot.Tick, snapshot.NextExpectedTick, string.Empty)) { }
        internal OrdanBossTickOutput(OrdanBossSnapshot snapshot, IReadOnlyList<OrdanBossIntent> intents, IReadOnlyList<OrdanBossHandoff> handoffs,
            OrdanTransferExposureForecast transferExposureForecast, OrdanAuditExposureForecast auditExposureForecast)
            : this(snapshot, intents, handoffs, transferExposureForecast, auditExposureForecast, null) { }
        internal OrdanBossTickOutput(OrdanBossSnapshot snapshot, IReadOnlyList<OrdanBossIntent> intents, IReadOnlyList<OrdanBossHandoff> handoffs,
            OrdanTransferExposureForecast transferExposureForecast, OrdanAuditExposureForecast auditExposureForecast, SeizureTrajectoryProjection? seizureTrajectoryProjection)
            : this(snapshot,intents,handoffs,transferExposureForecast,auditExposureForecast,seizureTrajectoryProjection,null) { }
        internal OrdanBossTickOutput(
            OrdanBossSnapshot snapshot,
            IReadOnlyList<OrdanBossIntent> intents,
            IReadOnlyList<OrdanBossHandoff> handoffs,
            OrdanTransferExposureForecast transferExposureForecast,
            OrdanAuditExposureForecast auditExposureForecast,
            SeizureTrajectoryProjection? seizureTrajectoryProjection,
            BalanceAuditPullProjection? balanceAuditPullProjection)
        {
            if (intents == null || handoffs == null) throw new ArgumentNullException();
            if (transferExposureForecast.SourceTick != snapshot.Tick || transferExposureForecast.TransferTick != snapshot.NextExpectedTick ||
                auditExposureForecast.SourceTick != snapshot.Tick || auditExposureForecast.TransferTick != snapshot.NextExpectedTick ||
                (transferExposureForecast.IsExposed && auditExposureForecast.IsExposed))
                throw new ArgumentException("Exposure forecast horizon must equal the output horizon.", nameof(transferExposureForecast));
            var active = snapshot.PatternSlot == OrdanPatternSlot.SeizureWeight && snapshot.Phase == OrdanBossPhase.Execute;
            if (seizureTrajectoryProjection.HasValue != active || (active && (seizureTrajectoryProjection.Value.SourceTick != snapshot.Tick || seizureTrajectoryProjection.Value.PayloadId != snapshot.ActiveHandleId || seizureTrajectoryProjection.Value.AttackOrdinal < 0 || seizureTrajectoryProjection.Value.ElapsedTick != snapshot.PhaseAge || seizureTrajectoryProjection.Value.Accelerated != snapshot.IsAccelerated || seizureTrajectoryProjection.Value.FrozenDuration != ExpectedSeizureDuration(snapshot.ActiveStage, snapshot.IsAccelerated)))) throw new ArgumentException("Seizure projection does not match snapshot.");
            var auditActive = snapshot.PatternSlot == OrdanPatternSlot.BalanceAudit && snapshot.Phase == OrdanBossPhase.Execute;
            if (balanceAuditPullProjection.HasValue != auditActive ||
                (auditActive &&
                    (balanceAuditPullProjection.Value.SourceTick != snapshot.Tick ||
                     balanceAuditPullProjection.Value.AttackOrdinal < 0 ||
                     balanceAuditPullProjection.Value.ElapsedTick != snapshot.PhaseAge ||
                     balanceAuditPullProjection.Value.FrozenDuration != ExpectedBalanceAuditDuration(snapshot.ActiveStage))))
            {
                throw new ArgumentException("BalanceAudit pull projection does not match snapshot.");
            }

            Snapshot = snapshot;
            Intents = Copy(intents);
            Handoffs = Copy(handoffs);
            TransferExposureForecast = transferExposureForecast;
            AuditExposureForecast = auditExposureForecast;
            SeizureTrajectoryProjection = seizureTrajectoryProjection;
            BalanceAuditPullProjection = balanceAuditPullProjection;
        }
        internal OrdanBossSnapshot Snapshot { get; }
        internal IReadOnlyList<OrdanBossIntent> Intents { get; }
        internal IReadOnlyList<OrdanBossHandoff> Handoffs { get; }
        internal OrdanTransferExposureForecast TransferExposureForecast { get; }
        internal OrdanAuditExposureForecast AuditExposureForecast { get; }
        internal SeizureTrajectoryProjection? SeizureTrajectoryProjection { get; }
        internal BalanceAuditPullProjection? BalanceAuditPullProjection { get; }
        public bool Equals(OrdanBossTickOutput other) =>
            Snapshot.Equals(other.Snapshot) &&
            TransferExposureForecast.Equals(other.TransferExposureForecast) &&
            AuditExposureForecast.Equals(other.AuditExposureForecast) &&
            Nullable.Equals(SeizureTrajectoryProjection, other.SeizureTrajectoryProjection) &&
            Nullable.Equals(BalanceAuditPullProjection, other.BalanceAuditPullProjection) &&
            Same(Intents, other.Intents) &&
            Same(Handoffs, other.Handoffs);
        public override bool Equals(object obj) => obj is OrdanBossTickOutput && Equals((OrdanBossTickOutput)obj);
        public override int GetHashCode() => Snapshot.GetHashCode();
        private static IReadOnlyList<T> Copy<T>(IReadOnlyList<T> values) { var copy = new T[values.Count]; for (var i = 0; i < copy.Length; i++) copy[i] = values[i]; return Array.AsReadOnly(copy); }
        private static bool Same<T>(IReadOnlyList<T> left, IReadOnlyList<T> right) where T : IEquatable<T> { if (left == null || right == null) return left == right; if (left.Count != right.Count) return false; for (var i = 0; i < left.Count; i++) if (!left[i].Equals(right[i])) return false; return true; }
        private static int ExpectedSeizureDuration(OrdanHealthStage stage, bool accelerated)
        {
            if (stage == OrdanHealthStage.Stage1) return accelerated ? 24 : 72;
            if (stage == OrdanHealthStage.Stage2) return accelerated ? 22 : 66;
            if (stage == OrdanHealthStage.Stage3) return accelerated ? 20 : 60;
            throw new ArgumentException("Active Seizure requires an exact health stage.");
        }

        private static int ExpectedBalanceAuditDuration(OrdanHealthStage stage)
        {
            if (stage == OrdanHealthStage.Stage1) return 90;
            if (stage == OrdanHealthStage.Stage2) return 82;
            if (stage == OrdanHealthStage.Stage3) return 75;
            throw new ArgumentException("Active BalanceAudit requires an exact health stage.");
        }
    }

    internal readonly struct OrdanBossTickCandidate
    {
        // The candidate exposes only the immutable projection.  The mutable
        // staged owner state never escapes preview/commit.
        internal OrdanBossTickCandidate(OrdanBossTickOutput output) { Output = output; }
        internal OrdanBossTickOutput Output { get; }
    }

    // REQ-COM-001, REQ-COM-003, REQ-COM-004, REQ-COM-006; affected REQ-WT-001, REQ-WT-005, REQ-WT-006.
    internal sealed class OrdanBossSession
    {
        private OrdanBossState _state;
        internal OrdanBossSession(SimulationTick firstExpectedTick)
        {
            if (firstExpectedTick.Value < 0 || firstExpectedTick.Value > int.MaxValue - 120) throw new ArgumentOutOfRangeException(nameof(firstExpectedTick));
            _state = OrdanBossState.Initial(firstExpectedTick);
        }
        internal OrdanBossTickOutput LatestOutput => _state.HasOutput ? _state.LatestOutput : throw new InvalidOperationException("No Ordan tick has committed.");
        internal SimulationTick NextExpectedTick => _state.NextExpectedTick;
        internal OrdanBossTickCandidate PreviewNext(OrdanBossTickInput input) => Calculate(input, _state).Candidate;
        internal OrdanBossTickOutput CommitNext(OrdanBossTickCandidate candidate, OrdanBossTickInput input)
        {
            var exact = Calculate(input, _state);
            if (!exact.Candidate.Output.Equals(candidate.Output)) throw new InvalidOperationException("Ordan preview is forged or stale.");
            _state = exact.State;
            return exact.Candidate.Output;
        }
        internal OrdanBossTickOutput Step(OrdanBossTickInput input)
        {
            var candidate = PreviewNext(input);
            return CommitNext(candidate, input);
        }

        private static OrdanBossCalculation Calculate(OrdanBossTickInput input, OrdanBossState prior)
        {
            ValidateInput(input, prior);
            var next = prior.Copy();
            var intents = new List<OrdanBossIntent>();
            var handoffs = new List<OrdanBossHandoff>();
            var tick = input.Tick;
            var nextTick = new SimulationTick(checked(tick.Value + 1));

            if (!next.HasOutput) StartFirstPattern(next, tick);
            ValidateHealth(input.CombatState, next);
            ResolvePendingPayload(input.CombatState, next, tick);

            if (input.CombatState.IsDead)
            {
                if (!next.IsDefeated) { next.IsDefeated = true; NormalizeDefeated(next, tick, nextTick); handoffs.Add(new OrdanBossHandoff(OrdanBossHandoffKind.Defeated, tick)); handoffs.Add(new OrdanBossHandoff(OrdanBossHandoffKind.RewardRequest, tick)); handoffs.Add(new OrdanBossHandoff(OrdanBossHandoffKind.RoomCompletionRequest, tick)); }
                else NormalizeDefeated(next, tick, nextTick);
                var deadSnapshot = next.Snapshot(tick, nextTick);
                var deadOutput = new OrdanBossTickOutput(deadSnapshot, intents, handoffs, new OrdanTransferExposureForecast(tick, nextTick, string.Empty), new OrdanAuditExposureForecast(tick, nextTick, string.Empty)); next.Adopt(deadOutput, nextTick, input.CombatState); return new OrdanBossCalculation(new OrdanBossTickCandidate(deadOutput), next);
            }

            next.PendingStage = MaxStage(next.PendingStage, StageForHealth(input.CombatState.CurrentHealth));
            if (next.Phase != OrdanBossPhase.Defeated && tick.Value == next.PhaseEndTick)
                TransitionExclusiveEnd(next, tick);

            if (input.RelevantTransferEdge.HasValue) ApplyTransfer(next, input.RelevantTransferEdge.Value, tick, intents);

            EmitPhaseIntents(next, tick, intents);
            var snapshot = next.Snapshot(tick, nextTick);
            SeizureTrajectoryProjection? trajectory = null;
            if (snapshot.PatternSlot == OrdanPatternSlot.SeizureWeight && snapshot.Phase == OrdanBossPhase.Execute)
                trajectory = new SeizureTrajectoryProjection(tick, snapshot.ActiveHandleId, next.AttackOrdinal, snapshot.PhaseAge, Duration(next.Slot, next.ActiveStage, next.Phase, next.IsAccelerated), next.IsAccelerated);
            BalanceAuditPullProjection? pull = null;
            if (snapshot.PatternSlot == OrdanPatternSlot.BalanceAudit && snapshot.Phase == OrdanBossPhase.Execute)
            {
                pull = new BalanceAuditPullProjection(
                    tick,
                    next.AttackOrdinal,
                    snapshot.PhaseAge,
                    Duration(next.Slot, next.ActiveStage, next.Phase, false));
            }

            var output = new OrdanBossTickOutput(
                snapshot,
                intents,
                handoffs,
                BuildNextTransferExposureForecast(next, tick, nextTick),
                BuildNextAuditExposureForecast(next, tick, nextTick),
                trajectory,
                pull);
            next.Adopt(output, nextTick, input.CombatState);
            return new OrdanBossCalculation(new OrdanBossTickCandidate(output), next);
        }

        private readonly struct OrdanBossCalculation
        {
            internal OrdanBossCalculation(OrdanBossTickCandidate candidate, OrdanBossState state)
            {
                Candidate = candidate;
                State = state ?? throw new ArgumentNullException(nameof(state));
            }

            internal OrdanBossTickCandidate Candidate { get; }
            internal OrdanBossState State { get; }
        }

        private static void ValidateInput(OrdanBossTickInput input, OrdanBossState state)
        {
            if (input.Tick != state.NextExpectedTick || input.CombatState.TargetId != "ordan") throw new InvalidOperationException("Ordan input must be consecutive and literal.");
            if (state.HasOutput && state.LastHealth < input.CombatState.CurrentHealth) throw new InvalidOperationException("Ordan health cannot increase.");
            if (state.IsDefeated && (!input.CombatState.IsDead || input.CombatState.PendingPayloadDamageResult.HasValue || input.RelevantTransferEdge.HasValue)) throw new InvalidOperationException("Defeated Ordan accepts only normalized dead inputs.");
            if (state.PendingPayload.HasValue != input.CombatState.PendingPayloadDamageResult.HasValue) throw new InvalidOperationException("Pending payload result presence is exact.");
            if (!state.PendingPayload.HasValue && input.CombatState.PendingPayloadDamageResult.HasValue) throw new InvalidOperationException("Unexpected payload result.");
        }
        private static void ValidateHealth(OrdanCombatState combat, OrdanBossState state)
        {
            if (!state.HasOutput && (combat.CurrentHealth != 60 || combat.IsDead)) throw new InvalidOperationException("First Ordan input must be alive at full health.");
            if (state.HasOutput && state.LastDead && !combat.IsDead) throw new InvalidOperationException("Ordan cannot resurrect.");
        }
        private static void StartFirstPattern(OrdanBossState state, SimulationTick tick)
        {
            state.Slot = OrdanPatternSlot.DebtLineA; state.Phase = OrdanBossPhase.Telegraph; state.PhaseStartTick = tick.Value; state.ActiveStage = OrdanHealthStage.Stage1; state.PendingStage = OrdanHealthStage.Stage1; state.AttackOrdinal = 0; state.PayloadOrdinal = -1; state.PhaseEndTick = checked(tick.Value + TelegraphDuration(state.Slot));
        }
        private static void ResolvePendingPayload(OrdanCombatState combat, OrdanBossState state, SimulationTick tick)
        {
            if (!state.PendingPayload.HasValue) return;
            var pending = state.PendingPayload.Value; var result = combat.PendingPayloadDamageResult.Value;
            if (result.RequestId != pending.RequestId || result.TargetId != "ordan" || result.Tick != pending.Tick || result.Tick != tick)
                throw new InvalidOperationException("Payload result is not aligned to its pending request.");
            var validAlive = result.Result == DamageResultCode.Applied && result.AppliedAmount == 8;
            var validDead = combat.IsDead && ((result.Result == DamageResultCode.Applied && result.AppliedAmount >= 1 && result.AppliedAmount <= 8) || (result.Result == DamageResultCode.TargetDead && result.AppliedAmount == 0));
            if ((!combat.IsDead && !validAlive) || (combat.IsDead && !validDead)) throw new InvalidOperationException("Payload result is not contract-valid.");
            state.PendingPayload = null;
            if (!combat.IsDead) StartVulnerability(state, tick, 120, OrdanVulnerabilitySource.SeizureWeightTransfer);
        }
        private static void ApplyTransfer(OrdanBossState state, OrdanRelevantTransferEdge edge, SimulationTick tick, List<OrdanBossIntent> intents)
        {
            if (edge.PreviousState != OrdanTransferState.Baseline || edge.NextState != OrdanTransferState.Heavy || edge.Revision <= state.LastRelevantRevision || edge.Revision <= state.LastRevision(edge.TargetId))
                throw new InvalidOperationException("Transfer edge must be a fresh Baseline-to-Heavy edge.");
            var payload = edge.TargetKind == OrdanTransferTargetKind.BossPayload && state.Slot == OrdanPatternSlot.SeizureWeight && state.Phase == OrdanBossPhase.Telegraph && edge.TargetId == state.ActiveHandleId && !state.IsAccelerated;
            var audit = edge.TargetKind == OrdanTransferTargetKind.Box && state.Slot == OrdanPatternSlot.BalanceAudit && state.Phase == OrdanBossPhase.Execute && edge.TargetId == "BossAuditBox-0";
            if (!payload && !audit) throw new InvalidOperationException("Transfer edge is not relevant in this phase.");
            state.LastRelevantRevision = edge.Revision; state.SetRevision(edge.TargetId, edge.Revision);
            if (payload)
            {
                state.IsAccelerated = true;
                intents.Add(new OrdanBossIntent(OrdanBossIntentKind.PayloadAcceleration, Format("ordan.payload-accelerate/{0:D19}", state.PayloadOrdinal), state.Slot, tick, 0, null, null));
            }
            else
            {
                // AC-COM-002: Execute age zero always announces the pull, even
                // when the same-tick box edge immediately wins the interruption.
                if (checked(tick.Value - state.PhaseStartTick) == 0)
                    intents.Add(new OrdanBossIntent(OrdanBossIntentKind.BalanceAuditPull, Format("ordan.audit-pull/{0:D19}", state.AttackOrdinal), state.Slot, tick, 0, null, null));
                intents.Add(new OrdanBossIntent(OrdanBossIntentKind.BalanceAuditInterrupt, Format("ordan.audit-interrupt/{0:D19}", state.AttackOrdinal), state.Slot, tick, 0, null, null));
                StartVulnerability(state, tick, 90, OrdanVulnerabilitySource.BalanceAuditInterrupt);
            }
        }
        private static void EmitPhaseIntents(OrdanBossState state, SimulationTick tick, List<OrdanBossIntent> intents)
        {
            if (state.Phase == OrdanBossPhase.Vulnerable || state.Phase == OrdanBossPhase.Defeated) return;
            var age = checked(tick.Value - state.PhaseStartTick);
            if (state.Phase == OrdanBossPhase.Execute && age == 0 && (state.Slot == OrdanPatternSlot.DebtLineA || state.Slot == OrdanPatternSlot.DebtLineB))
                intents.Add(new OrdanBossIntent(OrdanBossIntentKind.DebtLine, Format("ordan.debt/{0:D19}", state.AttackOrdinal), state.Slot, tick, 1, null, null));
            if (state.Slot == OrdanPatternSlot.BalanceAudit && state.Phase == OrdanBossPhase.Execute)
            {
                if (age == 0) intents.Add(new OrdanBossIntent(OrdanBossIntentKind.BalanceAuditPull, Format("ordan.audit-pull/{0:D19}", state.AttackOrdinal), state.Slot, tick, 0, null, null));
                if (age == Duration(state.Slot, state.ActiveStage, state.Phase, false) - 1)
                    intents.Add(new OrdanBossIntent(OrdanBossIntentKind.BalanceAuditShockwave, Format("ordan.audit-shockwave/{0:D19}", state.AttackOrdinal), state.Slot, tick, 1, null, null));
            }
            if (state.Slot == OrdanPatternSlot.SeizureWeight && state.Phase == OrdanBossPhase.Execute && state.IsAccelerated && age == Duration(state.Slot, state.ActiveStage, state.Phase, true) - 1)
            {
                var delivery = new SimulationTick(checked(tick.Value + 1));
                var id = Format("ordan.payload/{0:D19}/{1:D10}", state.PayloadOrdinal, delivery.Value);
                var request = new DamageRequest(id, state.ActiveHandleId, "ordan", 8, DamageKind.BossPayloadTransfer, delivery);
                state.PendingPayload = request;
                intents.Add(new OrdanBossIntent(OrdanBossIntentKind.PayloadDamageRequest, id, state.Slot, tick, 8, delivery, request));
            }
        }
        private static void TransitionExclusiveEnd(OrdanBossState state, SimulationTick tick)
        {
            if (state.Phase == OrdanBossPhase.Telegraph) BeginPhase(state, OrdanBossPhase.Execute, tick);
            else if (state.Phase == OrdanBossPhase.Execute) BeginPhase(state, OrdanBossPhase.Recovery, tick);
            else if (state.Phase == OrdanBossPhase.Recovery) BeginNextPattern(state, tick);
            else if (state.Phase == OrdanBossPhase.Vulnerable) BeginPhase(state, OrdanBossPhase.Recovery, tick);
            else throw new InvalidOperationException("Defeated phase cannot transition.");
        }
        private static OrdanTransferExposureForecast BuildNextTransferExposureForecast(OrdanBossState committed, SimulationTick sourceTick, SimulationTick transferTick)
        {
            if (committed.IsDefeated) return new OrdanTransferExposureForecast(sourceTick, transferTick, string.Empty);
            // REQ-COM-003, REQ-WT-005. Forecast only the exposure seam. Do not
            // execute the next gameplay transition here: doing so would move a
            // next-tick ordinal/phase overflow into the already-valid current
            // tick and break M4A's verified atomic boundary.
            var targetId = committed.Slot == OrdanPatternSlot.SeizureWeight && committed.Phase == OrdanBossPhase.Telegraph &&
                transferTick.Value != committed.PhaseEndTick ? committed.ActiveHandleId : string.Empty;
            if (committed.Phase == OrdanBossPhase.Recovery && transferTick.Value == committed.PhaseEndTick &&
                NextSlot(committed.Slot) == OrdanPatternSlot.SeizureWeight)
            {
                // REQ-COM-003. A non-representable candidate horizon rejects
                // as a whole; it must never be softened into an empty forecast.
                _ = checked(committed.AttackOrdinal + 1);
                var payloadOrdinal = checked(committed.PayloadOrdinal + 1);
                targetId = "BossWeight-" + (payloadOrdinal % 3);
            }
            return new OrdanTransferExposureForecast(sourceTick, transferTick, targetId);
        }
        private static OrdanAuditExposureForecast BuildNextAuditExposureForecast(OrdanBossState committed, SimulationTick sourceTick, SimulationTick transferTick)
        {
            var exposed = !committed.IsDefeated && committed.Slot == OrdanPatternSlot.BalanceAudit &&
                ((committed.Phase == OrdanBossPhase.Telegraph && transferTick.Value == committed.PhaseEndTick) ||
                 (committed.Phase == OrdanBossPhase.Execute && transferTick.Value < committed.PhaseEndTick));
            return new OrdanAuditExposureForecast(sourceTick, transferTick, exposed ? "BossAuditBox-0" : string.Empty);
        }
        private static void BeginNextPattern(OrdanBossState state, SimulationTick tick)
        {
            state.Slot = NextSlot(state.Slot); state.ActiveStage = state.PendingStage; state.AttackOrdinal = checked(state.AttackOrdinal + 1); state.IsAccelerated = false; state.ActiveHandleId = string.Empty;
            if (state.Slot == OrdanPatternSlot.SeizureWeight) { state.PayloadOrdinal = checked(state.PayloadOrdinal + 1); state.ActiveHandleId = "BossWeight-" + (state.PayloadOrdinal % 3); }
            BeginPhase(state, OrdanBossPhase.Telegraph, tick);
        }
        private static void BeginPhase(OrdanBossState state, OrdanBossPhase phase, SimulationTick tick)
        {
            state.Phase = phase; state.PhaseStartTick = tick.Value; var duration = Duration(state.Slot, state.ActiveStage, phase, state.IsAccelerated); state.PhaseEndTick = checked(tick.Value + duration);
            if (phase == OrdanBossPhase.Recovery || phase == OrdanBossPhase.Vulnerable) { state.ActiveHandleId = string.Empty; state.IsAccelerated = false; }
            if (phase != OrdanBossPhase.Vulnerable) state.VulnerabilitySource = OrdanVulnerabilitySource.None;
        }
        private static void StartVulnerability(OrdanBossState state, SimulationTick tick, int duration, OrdanVulnerabilitySource source)
        {
            if (duration < 1 || source == OrdanVulnerabilitySource.None) throw new ArgumentOutOfRangeException(nameof(duration));
            state.Phase = OrdanBossPhase.Vulnerable; state.PhaseStartTick = tick.Value; state.PhaseEndTick = checked(tick.Value + duration); state.VulnerabilitySource = source; state.ActiveHandleId = string.Empty; state.IsAccelerated = false;
        }
        private static void NormalizeDefeated(OrdanBossState state, SimulationTick tick, SimulationTick nextTick)
        {
            state.Slot = OrdanPatternSlot.None; state.Phase = OrdanBossPhase.Defeated; state.PhaseStartTick = tick.Value; state.PhaseEndTick = tick.Value; state.ActiveStage = OrdanHealthStage.None; state.PendingStage = OrdanHealthStage.None; state.ActiveHandleId = string.Empty; state.IsAccelerated = false; state.VulnerabilitySource = OrdanVulnerabilitySource.None; state.PendingPayload = null; state.NextExpectedTick = nextTick;
        }
        private static OrdanPatternSlot NextSlot(OrdanPatternSlot slot) { if (slot == OrdanPatternSlot.DebtLineA) return OrdanPatternSlot.SeizureWeight; if (slot == OrdanPatternSlot.SeizureWeight) return OrdanPatternSlot.DebtLineB; if (slot == OrdanPatternSlot.DebtLineB) return OrdanPatternSlot.BalanceAudit; if (slot == OrdanPatternSlot.BalanceAudit) return OrdanPatternSlot.DebtLineA; throw new InvalidOperationException("Invalid Ordan pattern slot."); }
        private static OrdanHealthStage StageForHealth(int health) => health < 21 ? OrdanHealthStage.Stage3 : health < 42 ? OrdanHealthStage.Stage2 : OrdanHealthStage.Stage1;
        private static OrdanHealthStage MaxStage(OrdanHealthStage left, OrdanHealthStage right) => left >= right ? left : right;
        private static int TelegraphDuration(OrdanPatternSlot slot) => slot == OrdanPatternSlot.DebtLineA || slot == OrdanPatternSlot.DebtLineB ? 48 : 60;
        private static int Duration(OrdanPatternSlot slot, OrdanHealthStage stage, OrdanBossPhase phase, bool accelerated)
        {
            if (phase == OrdanBossPhase.Telegraph) return TelegraphDuration(slot);
            if (phase == OrdanBossPhase.Vulnerable) throw new InvalidOperationException("Vulnerability uses its triggering duration.");
            int baseTicks;
            if (slot == OrdanPatternSlot.DebtLineA || slot == OrdanPatternSlot.DebtLineB) baseTicks = phase == OrdanBossPhase.Execute ? 21 : phase == OrdanBossPhase.Recovery ? 39 : throw new InvalidOperationException();
            else if (slot == OrdanPatternSlot.SeizureWeight) baseTicks = phase == OrdanBossPhase.Execute ? accelerated ? 24 : 72 : phase == OrdanBossPhase.Recovery ? 48 : throw new InvalidOperationException();
            else if (slot == OrdanPatternSlot.BalanceAudit) baseTicks = phase == OrdanBossPhase.Execute ? 90 : phase == OrdanBossPhase.Recovery ? 48 : throw new InvalidOperationException();
            else throw new InvalidOperationException();
            if (stage == OrdanHealthStage.Stage1) return baseTicks;
            var divisor = stage == OrdanHealthStage.Stage2 ? 110 : stage == OrdanHealthStage.Stage3 ? 120 : throw new InvalidOperationException();
            return checked((checked(baseTicks * 100) + divisor - 1) / divisor);
        }
        private static string Format(string format, long value) { if (value < 0) throw new OverflowException("Ordan ordinal is negative."); return string.Format(System.Globalization.CultureInfo.InvariantCulture, format, value); }
        private static string Format(string format, long ordinal, int tick) { if (ordinal < 0 || tick < 0) throw new OverflowException("Ordan identifier component is negative."); return string.Format(System.Globalization.CultureInfo.InvariantCulture, format, ordinal, tick); }
    }

    internal sealed class OrdanBossState : IEquatable<OrdanBossState>
    {
        internal bool HasOutput; internal bool IsDefeated; internal int LastHealth; internal bool LastDead; internal SimulationTick NextExpectedTick;
        internal OrdanPatternSlot Slot; internal OrdanBossPhase Phase; internal int PhaseStartTick; internal int PhaseEndTick; internal OrdanHealthStage ActiveStage; internal OrdanHealthStage PendingStage;
        internal string ActiveHandleId; internal bool IsAccelerated; internal OrdanVulnerabilitySource VulnerabilitySource; internal long PayloadOrdinal; internal long AttackOrdinal; internal int LastRelevantRevision;
        internal int[] TargetRevisions; internal DamageRequest? PendingPayload; internal OrdanBossTickOutput LatestOutput;
        internal static OrdanBossState Initial(SimulationTick first) => new OrdanBossState { NextExpectedTick = first, ActiveHandleId = string.Empty, PayloadOrdinal = -1, AttackOrdinal = -1, LastRelevantRevision = -1, TargetRevisions = new[] { -1, -1, -1, -1 } };
        internal OrdanBossState Copy() => new OrdanBossState { HasOutput = HasOutput, IsDefeated = IsDefeated, LastHealth = LastHealth, LastDead = LastDead, NextExpectedTick = NextExpectedTick, Slot = Slot, Phase = Phase, PhaseStartTick = PhaseStartTick, PhaseEndTick = PhaseEndTick, ActiveStage = ActiveStage, PendingStage = PendingStage, ActiveHandleId = ActiveHandleId, IsAccelerated = IsAccelerated, VulnerabilitySource = VulnerabilitySource, PayloadOrdinal = PayloadOrdinal, AttackOrdinal = AttackOrdinal, LastRelevantRevision = LastRelevantRevision, TargetRevisions = (int[])TargetRevisions.Clone(), PendingPayload = PendingPayload, LatestOutput = LatestOutput };
        internal int LastRevision(string id) { var index = RevisionIndex(id); return TargetRevisions[index]; }
        internal void SetRevision(string id, int revision) { TargetRevisions[RevisionIndex(id)] = revision; }
        internal OrdanBossSnapshot Snapshot(SimulationTick tick, SimulationTick next) => new OrdanBossSnapshot(tick, Slot, Phase, Phase == OrdanBossPhase.Defeated ? 0 : checked(tick.Value - PhaseStartTick), ActiveStage, PendingStage, ActiveHandleId, IsAccelerated, Phase == OrdanBossPhase.Vulnerable, VulnerabilitySource, next);
        internal void Adopt(OrdanBossTickOutput output, SimulationTick next, OrdanCombatState combat) { HasOutput = true; LastHealth = combat.CurrentHealth; LastDead = combat.IsDead; LatestOutput = output; NextExpectedTick = next; }
        private static int RevisionIndex(string id) { if (id == "BossWeight-0") return 0; if (id == "BossWeight-1") return 1; if (id == "BossWeight-2") return 2; if (id == "BossAuditBox-0") return 3; throw new InvalidOperationException("Unknown Ordan transfer target."); }
        public bool Equals(OrdanBossState other) { if (other == null || HasOutput != other.HasOutput || IsDefeated != other.IsDefeated || LastHealth != other.LastHealth || LastDead != other.LastDead || NextExpectedTick != other.NextExpectedTick || Slot != other.Slot || Phase != other.Phase || PhaseStartTick != other.PhaseStartTick || PhaseEndTick != other.PhaseEndTick || ActiveStage != other.ActiveStage || PendingStage != other.PendingStage || ActiveHandleId != other.ActiveHandleId || IsAccelerated != other.IsAccelerated || VulnerabilitySource != other.VulnerabilitySource || PayloadOrdinal != other.PayloadOrdinal || AttackOrdinal != other.AttackOrdinal || LastRelevantRevision != other.LastRelevantRevision || !Nullable.Equals(PendingPayload, other.PendingPayload) || (HasOutput && !LatestOutput.Equals(other.LatestOutput))) return false; for (var i = 0; i < 4; i++) if (TargetRevisions[i] != other.TargetRevisions[i]) return false; return true; }
    }
}
