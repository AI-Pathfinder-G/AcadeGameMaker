using System;
using System.Collections.Generic;
using System.Globalization;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat
{
    // REQ-COM-001, REQ-UX-007, REQ-UX-008, REQ-UX-013. M2A's engine-free frozen attack observation; no Transfer state or Unity reference participates.
    internal readonly struct BasicAttackCandidateObservation
    {
        internal BasicAttackCandidateObservation(
            string targetId,
            SimulationTick observationTick,
            bool isAttackable,
            bool isLineOfSightOpen,
            int playerDistanceKey,
            bool mouseInsideShape,
            int screenDistanceSquaredKey,
            int gamepadAngleKey)
        {
            DamageValidation.Id(targetId, nameof(targetId));
            if (playerDistanceKey < 0) throw new ArgumentOutOfRangeException(nameof(playerDistanceKey));
            if (screenDistanceSquaredKey < 0) throw new ArgumentOutOfRangeException(nameof(screenDistanceSquaredKey));
            if (gamepadAngleKey < 0 || gamepadAngleKey > 1800) throw new ArgumentOutOfRangeException(nameof(gamepadAngleKey));
            TargetId = targetId;
            ObservationTick = observationTick;
            IsAttackable = isAttackable;
            IsLineOfSightOpen = isLineOfSightOpen;
            PlayerDistanceKey = playerDistanceKey;
            MouseInsideShape = mouseInsideShape;
            ScreenDistanceSquaredKey = screenDistanceSquaredKey;
            GamepadAngleKey = gamepadAngleKey;
        }

        internal string TargetId { get; }
        internal SimulationTick ObservationTick { get; }
        internal bool IsAttackable { get; }
        internal bool IsLineOfSightOpen { get; }
        internal int PlayerDistanceKey { get; }
        internal bool MouseInsideShape { get; }
        internal int ScreenDistanceSquaredKey { get; }
        internal int GamepadAngleKey { get; }
    }

    internal readonly struct BasicAttackSelectionSnapshot
    {
        internal BasicAttackSelectionSnapshot(SimulationTick tick, string targetId)
        {
            Tick = tick;
            TargetId = targetId ?? string.Empty;
        }

        internal SimulationTick Tick { get; }
        internal string TargetId { get; }
        internal bool HasTarget => TargetId.Length != 0;
    }

    internal enum BasicAttackAttemptCode
    {
        StaleAim = 0,
        InvalidAim = 1,
        Cooldown = 2,
        FiredMiss = 3,
        FiredHit = 4
    }

    internal readonly struct BasicAttackAttempt
    {
        internal BasicAttackAttempt(long attackSequenceId, SimulationTick tick, BasicAttackAttemptCode code, string selectedTargetId)
        {
            AttackSequenceId = attackSequenceId;
            Tick = tick;
            Code = code;
            SelectedTargetId = selectedTargetId ?? string.Empty;
        }

        internal long AttackSequenceId { get; }
        internal SimulationTick Tick { get; }
        internal BasicAttackAttemptCode Code { get; }
        internal string SelectedTargetId { get; }
        internal bool Fired => Code == BasicAttackAttemptCode.FiredMiss || Code == BasicAttackAttemptCode.FiredHit;
    }

    internal readonly struct BasicAttackSessionSnapshot
    {
        internal BasicAttackSessionSnapshot(SimulationTick tick, string highlightedTargetId, int cooldownEndTick, int consumedSequenceCount, SimulationTick nextExpectedTick)
        {
            Tick = tick;
            HighlightedTargetId = highlightedTargetId ?? string.Empty;
            CooldownEndTick = cooldownEndTick;
            ConsumedSequenceCount = consumedSequenceCount;
            NextExpectedTick = nextExpectedTick;
        }

        internal SimulationTick Tick { get; }
        internal string HighlightedTargetId { get; }
        internal int CooldownEndTick { get; }
        internal int ConsumedSequenceCount { get; }
        internal SimulationTick NextExpectedTick { get; }
    }

    internal readonly struct BasicAttackTickOutcome
    {
        internal BasicAttackTickOutcome(BasicAttackSelectionSnapshot selection, BasicAttackAttempt? attempt, DamageRequest? damageRequest, BasicAttackSessionSnapshot snapshot)
        {
            Selection = selection;
            Attempt = attempt;
            DamageRequest = damageRequest;
            Snapshot = snapshot;
        }

        internal BasicAttackSelectionSnapshot Selection { get; }
        internal BasicAttackAttempt? Attempt { get; }
        internal DamageRequest? DamageRequest { get; }
        internal BasicAttackSessionSnapshot Snapshot { get; }
    }

    // REQ-COM-001, REQ-COM-004: immutable M2A-owned preview; no Combat shadow state participates.
    internal sealed class BasicAttackCandidate
    {
        private readonly BasicAttackCandidateObservation[] _observations;
        private readonly object _owner;

        internal BasicAttackCandidate(object owner, SimulationTick tick, AimSample? sample,
            IReadOnlyList<BasicAttackCandidateObservation> observations, BasicAttackPressed? press,
            BasicAttackTickOutcome outcome, string stagedHighlight, int stagedCooldownEndTick)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Tick = tick; Sample = sample; Press = press; Outcome = outcome;
            StagedHighlight = stagedHighlight; StagedCooldownEndTick = stagedCooldownEndTick;
            _observations = new BasicAttackCandidateObservation[observations.Count];
            for (var index = 0; index < _observations.Length; index++) _observations[index] = observations[index];
        }

        internal SimulationTick Tick { get; }
        internal AimSample? Sample { get; }
        internal BasicAttackPressed? Press { get; }
        internal BasicAttackTickOutcome Outcome { get; }
        internal string StagedHighlight { get; }
        internal int StagedCooldownEndTick { get; }
        internal BasicAttackCandidateObservation[] CopyObservations() => (BasicAttackCandidateObservation[])_observations.Clone();
        internal bool IsOwnedBy(object owner) => ReferenceEquals(_owner, owner);
    }

    // REQ-COM-001, REQ-COM-004, REQ-UX-007, REQ-UX-008, REQ-UX-013. The M2A session owns only deterministic attack intent, acquisition, cooldown, and request construction.
    internal sealed class BasicAttackSession
    {
        private readonly HashSet<long> _consumedSequenceIds = new HashSet<long>();
        private readonly object _candidateOwner = new object();
        private SimulationTick _nextExpectedTick;
        private string _highlightedTargetId = string.Empty;
        private int _cooldownEndTick = -1;

        internal BasicAttackSession(SimulationTick firstExpectedTick)
        {
            if (firstExpectedTick.Value < 0 || firstExpectedTick.Value > int.MaxValue - 1) throw new ArgumentOutOfRangeException(nameof(firstExpectedTick));
            _nextExpectedTick = firstExpectedTick;
        }

        internal SimulationTick NextExpectedTick => _nextExpectedTick;

        internal BasicAttackSessionSnapshot Snapshot(SimulationTick tick) =>
            new BasicAttackSessionSnapshot(tick, _highlightedTargetId, _cooldownEndTick, _consumedSequenceIds.Count, _nextExpectedTick);

        internal BasicAttackTickOutcome Process(
            SimulationTick processingTick,
            AimSample? sample,
            IReadOnlyList<BasicAttackCandidateObservation> observations,
            BasicAttackPressed? press)
        {
            var candidate = PreviewNext(processingTick, sample, observations, press);
            return CommitPreflighted(candidate);
        }

        internal BasicAttackCandidate PreviewNext(SimulationTick processingTick, AimSample? sample,
            IReadOnlyList<BasicAttackCandidateObservation> observations, BasicAttackPressed? press)
        {
            if (processingTick.Value < 0 || processingTick != _nextExpectedTick) throw new InvalidOperationException("Attack ticks must be exactly the next nonnegative global simulation tick.");
            var nextExpected = new SimulationTick(checked(processingTick.Value + 1));
            var copiedObservations = CopyAndValidateObservations(processingTick, observations);
            ValidateSample(processingTick, sample);
            ValidatePress(processingTick, press);
            if (press.HasValue && _consumedSequenceIds.Contains(press.Value.AttackSequenceId)) throw new InvalidOperationException("Attack sequence IDs are encounter-lifetime unique.");

            var selection = sample.HasValue
                ? Select(processingTick, sample.Value, copiedObservations, _highlightedTargetId)
                : new BasicAttackSelectionSnapshot(processingTick, _highlightedTargetId);
            BasicAttackAttempt? attempt = null;
            DamageRequest? damageRequest = null;
            var stagedCooldownEndTick = _cooldownEndTick;

            if (press.HasValue)
            {
                var command = press.Value;
                var code = ResolveAttempt(processingTick, sample, command, selection, _cooldownEndTick);
                attempt = new BasicAttackAttempt(command.AttackSequenceId, processingTick, code, selection.TargetId);
                if (code == BasicAttackAttemptCode.FiredHit)
                {
                    damageRequest = BuildDamageRequest(command.AttackSequenceId, selection.TargetId, processingTick);
                    stagedCooldownEndTick = checked(processingTick.Value + 21);
                }
                else if (code == BasicAttackAttemptCode.FiredMiss)
                {
                    stagedCooldownEndTick = checked(processingTick.Value + 21);
                }
            }

            var stagedHighlight = sample.HasValue ? selection.TargetId : _highlightedTargetId;
            var stagedConsumedCount = checked(_consumedSequenceIds.Count + (press.HasValue ? 1 : 0));
            var snapshot = new BasicAttackSessionSnapshot(processingTick, stagedHighlight, stagedCooldownEndTick,
                stagedConsumedCount, nextExpected);
            var outcome = new BasicAttackTickOutcome(selection, attempt, damageRequest, snapshot);
            return new BasicAttackCandidate(_candidateOwner, processingTick, sample, copiedObservations, press,
                outcome, stagedHighlight, stagedCooldownEndTick);
        }

        internal BasicAttackTickOutcome CommitPreflighted(BasicAttackCandidate candidate)
        {
            if (candidate == null || !candidate.IsOwnedBy(_candidateOwner))
                throw new InvalidOperationException("Basic-attack candidate was not issued by this session.");
            var recomputed = PreviewNext(candidate.Tick, candidate.Sample, candidate.CopyObservations(), candidate.Press);
            if (!CandidateEquals(candidate, recomputed))
                throw new InvalidOperationException("Basic-attack candidate disagrees with exact recomputation.");

            // All checked construction and validation completed above. This is the sole publication point for the tick.
            _highlightedTargetId = candidate.StagedHighlight;
            if (candidate.Press.HasValue) _consumedSequenceIds.Add(candidate.Press.Value.AttackSequenceId);
            _cooldownEndTick = candidate.StagedCooldownEndTick;
            _nextExpectedTick = candidate.Outcome.Snapshot.NextExpectedTick;
            return candidate.Outcome;
        }

        private static bool CandidateEquals(BasicAttackCandidate left, BasicAttackCandidate right) =>
            left.Tick == right.Tick && Nullable.Equals(left.Sample, right.Sample) && Nullable.Equals(left.Press, right.Press) &&
            string.Equals(left.StagedHighlight, right.StagedHighlight, StringComparison.Ordinal) &&
            left.StagedCooldownEndTick == right.StagedCooldownEndTick && OutcomeEquals(left.Outcome, right.Outcome) &&
            ObservationsEqual(left.CopyObservations(), right.CopyObservations());

        private static bool ObservationsEqual(IReadOnlyList<BasicAttackCandidateObservation> left,
            IReadOnlyList<BasicAttackCandidateObservation> right)
        {
            if (left.Count != right.Count) return false;
            for (var index = 0; index < left.Count; index++)
            {
                var a = left[index]; var b = right[index];
                if (a.TargetId != b.TargetId || a.ObservationTick != b.ObservationTick || a.IsAttackable != b.IsAttackable ||
                    a.IsLineOfSightOpen != b.IsLineOfSightOpen || a.PlayerDistanceKey != b.PlayerDistanceKey ||
                    a.MouseInsideShape != b.MouseInsideShape || a.ScreenDistanceSquaredKey != b.ScreenDistanceSquaredKey ||
                    a.GamepadAngleKey != b.GamepadAngleKey) return false;
            }
            return true;
        }

        private static bool OutcomeEquals(BasicAttackTickOutcome left, BasicAttackTickOutcome right) =>
            left.Selection.Tick == right.Selection.Tick && left.Selection.TargetId == right.Selection.TargetId &&
            Nullable.Equals(left.Attempt, right.Attempt) && Nullable.Equals(left.DamageRequest, right.DamageRequest) &&
            left.Snapshot.Tick == right.Snapshot.Tick && left.Snapshot.HighlightedTargetId == right.Snapshot.HighlightedTargetId &&
            left.Snapshot.CooldownEndTick == right.Snapshot.CooldownEndTick &&
            left.Snapshot.ConsumedSequenceCount == right.Snapshot.ConsumedSequenceCount &&
            left.Snapshot.NextExpectedTick == right.Snapshot.NextExpectedTick;

        private static BasicAttackCandidateObservation[] CopyAndValidateObservations(SimulationTick tick, IReadOnlyList<BasicAttackCandidateObservation> observations)
        {
            if (observations == null) throw new ArgumentNullException(nameof(observations));
            var copy = new BasicAttackCandidateObservation[observations.Count];
            var ids = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < copy.Length; index++)
            {
                var observation = observations[index];
                ValidateObservation(observation);
                if (observation.ObservationTick != tick) throw new ArgumentException("Every attack observation must use the processing tick.", nameof(observations));
                if (!ids.Add(observation.TargetId)) throw new ArgumentException("Attack observation target IDs must be unique.", nameof(observations));
                copy[index] = observation;
            }

            return copy;
        }

        private static void ValidateObservation(BasicAttackCandidateObservation observation)
        {
            DamageValidation.Id(observation.TargetId, nameof(observation));
            if (observation.PlayerDistanceKey < 0 || observation.ScreenDistanceSquaredKey < 0 || observation.GamepadAngleKey < 0 || observation.GamepadAngleKey > 1800)
            {
                throw new ArgumentException("Attack observation keys are outside the frozen range.", nameof(observation));
            }
        }

        private static void ValidateSample(SimulationTick tick, AimSample? sample)
        {
            if (sample.HasValue && sample.Value.SampleTick != tick) throw new ArgumentException("Attack aim sample must use the processing tick.", nameof(sample));
        }

        private static void ValidatePress(SimulationTick tick, BasicAttackPressed? press)
        {
            if (press.HasValue && press.Value.Tick != tick) throw new ArgumentException("Attack press must use the processing tick.", nameof(press));
        }

        private static BasicAttackAttemptCode ResolveAttempt(SimulationTick tick, AimSample? sample, BasicAttackPressed press, BasicAttackSelectionSnapshot selection, int cooldownEndTick)
        {
            if (!sample.HasValue || sample.Value.SampleId != press.AimSampleId) return BasicAttackAttemptCode.StaleAim;
            if (sample.Value.Source == AimSource.MousePointer && !sample.Value.IsPointerInsideGameplayRect.GetValueOrDefault()) return BasicAttackAttemptCode.InvalidAim;
            if (tick.Value < cooldownEndTick) return BasicAttackAttemptCode.Cooldown;
            return selection.HasTarget ? BasicAttackAttemptCode.FiredHit : BasicAttackAttemptCode.FiredMiss;
        }

        private static DamageRequest BuildDamageRequest(long attackSequenceId, string targetId, SimulationTick tick)
        {
            var requestId = "Player.BasicAttack/" + attackSequenceId.ToString(CultureInfo.InvariantCulture);
            return new DamageRequest(requestId, "player", targetId, 3, DamageKind.BasicAttack, tick);
        }

        private static BasicAttackSelectionSnapshot Select(SimulationTick tick, AimSample sample, IReadOnlyList<BasicAttackCandidateObservation> observations, string previousHighlightedTargetId)
        {
            return sample.Source == AimSource.MousePointer
                ? SelectMouse(tick, sample, observations)
                : SelectGamepad(tick, observations, previousHighlightedTargetId);
        }

        private static BasicAttackSelectionSnapshot SelectMouse(SimulationTick tick, AimSample sample, IReadOnlyList<BasicAttackCandidateObservation> observations)
        {
            if (!sample.IsPointerInsideGameplayRect.GetValueOrDefault()) return new BasicAttackSelectionSnapshot(tick, string.Empty);
            BasicAttackCandidateObservation? best = null;
            for (var index = 0; index < observations.Count; index++)
            {
                var candidate = observations[index];
                if (!IsOtherwiseEligible(candidate) || (!candidate.MouseInsideShape && candidate.ScreenDistanceSquaredKey > 576)) continue;
                if (!best.HasValue || CompareMouse(candidate, best.Value) < 0) best = candidate;
            }

            return new BasicAttackSelectionSnapshot(tick, best.HasValue ? best.Value.TargetId : string.Empty);
        }

        private static BasicAttackSelectionSnapshot SelectGamepad(SimulationTick tick, IReadOnlyList<BasicAttackCandidateObservation> observations, string previousHighlightedTargetId)
        {
            BasicAttackCandidateObservation? retained = null;
            BasicAttackCandidateObservation? bestNew = null;
            for (var index = 0; index < observations.Count; index++)
            {
                var candidate = observations[index];
                if (!IsOtherwiseEligible(candidate)) continue;
                if (string.Equals(candidate.TargetId, previousHighlightedTargetId, StringComparison.Ordinal) && candidate.GamepadAngleKey <= 260) retained = candidate;
                if (candidate.GamepadAngleKey <= 180 && (!bestNew.HasValue || CompareGamepad(candidate, bestNew.Value) < 0)) bestNew = candidate;
            }

            if (!retained.HasValue) return new BasicAttackSelectionSnapshot(tick, bestNew.HasValue ? bestNew.Value.TargetId : string.Empty);
            if (!bestNew.HasValue || string.Equals(bestNew.Value.TargetId, retained.Value.TargetId, StringComparison.Ordinal)) return new BasicAttackSelectionSnapshot(tick, retained.Value.TargetId);
            return bestNew.Value.GamepadAngleKey + 40 <= retained.Value.GamepadAngleKey
                ? new BasicAttackSelectionSnapshot(tick, bestNew.Value.TargetId)
                : new BasicAttackSelectionSnapshot(tick, retained.Value.TargetId);
        }

        private static bool IsOtherwiseEligible(BasicAttackCandidateObservation candidate) =>
            candidate.IsAttackable && candidate.IsLineOfSightOpen && candidate.PlayerDistanceKey <= 36000;

        private static int CompareMouse(BasicAttackCandidateObservation left, BasicAttackCandidateObservation right)
        {
            var inside = (left.MouseInsideShape ? 0 : 1).CompareTo(right.MouseInsideShape ? 0 : 1);
            if (inside != 0) return inside;
            var screen = left.ScreenDistanceSquaredKey.CompareTo(right.ScreenDistanceSquaredKey);
            if (screen != 0) return screen;
            var distance = left.PlayerDistanceKey.CompareTo(right.PlayerDistanceKey);
            return distance != 0 ? distance : string.CompareOrdinal(left.TargetId, right.TargetId);
        }

        private static int CompareGamepad(BasicAttackCandidateObservation left, BasicAttackCandidateObservation right)
        {
            var angle = left.GamepadAngleKey.CompareTo(right.GamepadAngleKey);
            if (angle != 0) return angle;
            var distance = left.PlayerDistanceKey.CompareTo(right.PlayerDistanceKey);
            return distance != 0 ? distance : string.CompareOrdinal(left.TargetId, right.TargetId);
        }
    }
}
