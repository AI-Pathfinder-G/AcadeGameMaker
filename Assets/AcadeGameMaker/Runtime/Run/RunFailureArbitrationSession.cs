using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Run
{
    // REQ-M5D2-001..006. This file owns only the engine-free failure prefix.
    internal enum RunFailurePhase { NotStarted, Active, Failed }
    internal enum RunFailureCause { HealthDepleted, KillPlane, LethalCrush }
    [Flags]
    internal enum RunFailureCauseMask
    {
        None = 0,
        HealthDepleted = 1,
        KillPlane = 2,
        LethalCrush = 4,
        AllKnown = HealthDepleted | KillPlane | LethalCrush
    }
    internal enum RunFailureDisposition { Applied, Duplicate }
    internal enum RunResult { Succeeded, Failed }

    internal readonly struct RunEndRequested
    {
        internal RunEndRequested(RunResult result, RunFailureCause cause, SimulationTick tick)
        {
            if (result != RunResult.Failed || !RunFailureValidation.IsKnown(cause) || tick.Value < 0)
                throw new ArgumentException("Run end request must be an exact failed run payload.");
            Result = result;
            Cause = cause;
            Tick = tick;
        }

        internal RunResult Result { get; }
        internal RunFailureCause Cause { get; }
        internal SimulationTick Tick { get; }
    }

    internal readonly struct RunStartInput
    {
        internal RunStartInput(string runInstanceKey, SimulationTick startTick)
        {
            RunFailureValidation.ValidateKey(runInstanceKey);
            if (startTick.Value < 0 || startTick.Value == int.MaxValue)
                throw new ArgumentException("Run start tick must be non-negative and have a signed-32 successor.");
            RunInstanceKey = runInstanceKey;
            StartTick = startTick;
        }

        internal string RunInstanceKey { get; }
        internal SimulationTick StartTick { get; }
    }

    internal readonly struct RunFailureTickInput
    {
        internal RunFailureTickInput(string runInstanceKey, SimulationTick tick, RunFailureCauseMask presenceMask,
            RunFailureCauseMask triggeredMask)
        {
            RunFailureValidation.ValidateKey(runInstanceKey);
            RunFailureValidation.ValidateClosedBatch(tick, presenceMask, triggeredMask);
            RunInstanceKey = runInstanceKey;
            Tick = tick;
            PresenceMask = presenceMask;
            TriggeredMask = triggeredMask;
        }

        internal string RunInstanceKey { get; }
        internal SimulationTick Tick { get; }
        internal RunFailureCauseMask PresenceMask { get; }
        internal RunFailureCauseMask TriggeredMask { get; }
    }

    internal readonly struct AcceptedRunFailure
    {
        internal AcceptedRunFailure(RunFailureCause cause, SimulationTick tick, RunFailureCauseMask presenceMask,
            RunFailureCauseMask triggeredMask, IReadOnlyList<RunFailureCause> observedCauses)
        {
            RunFailureValidation.ValidateAccepted(cause, tick, presenceMask, triggeredMask, observedCauses);
            Cause = cause;
            Tick = tick;
            PresenceMask = presenceMask;
            TriggeredMask = triggeredMask;
            ObservedCauses = RunFailureValidation.CopyCauses(observedCauses);
        }

        internal RunFailureCause Cause { get; }
        internal SimulationTick Tick { get; }
        internal RunFailureCauseMask PresenceMask { get; }
        internal RunFailureCauseMask TriggeredMask { get; }
        internal IReadOnlyList<RunFailureCause> ObservedCauses { get; }
    }

    internal readonly struct RunFailureIntent
    {
        internal RunFailureIntent(RunEndRequested request, string runInstanceKey, long sourceRevision,
            RunFailureCauseMask presenceMask, RunFailureCauseMask triggeredMask, IReadOnlyList<RunFailureCause> observedCauses)
        {
            RunFailureValidation.ValidateKey(runInstanceKey);
            if (sourceRevision <= 0) throw new ArgumentOutOfRangeException(nameof(sourceRevision));
            if (request.Result != RunResult.Failed) throw new ArgumentException("Failure intent requires a failed run request.");
            RunFailureValidation.ValidateClosedBatch(request.Tick, presenceMask, triggeredMask);
            RunFailureValidation.ValidateAccepted(request.Cause, request.Tick, presenceMask, triggeredMask, observedCauses);
            Request = request;
            RunInstanceKey = runInstanceKey;
            SourceRevision = sourceRevision;
            PresenceMask = presenceMask;
            TriggeredMask = triggeredMask;
            ObservedCauses = RunFailureValidation.CopyCauses(observedCauses);
        }

        internal RunEndRequested Request { get; }
        internal string RunInstanceKey { get; }
        internal long SourceRevision { get; }
        internal RunFailureCauseMask PresenceMask { get; }
        internal RunFailureCauseMask TriggeredMask { get; }
        internal IReadOnlyList<RunFailureCause> ObservedCauses { get; }
    }

    internal readonly struct RunFailureSnapshot
    {
        internal RunFailureSnapshot(RunFailurePhase phase, string runInstanceKey, SimulationTick? startTick,
            SimulationTick? nextExpectedTick, AcceptedRunFailure? acceptedFailure, long revision)
        {
            if (!RunFailureValidation.IsKnown(phase) || revision < 0) throw new ArgumentOutOfRangeException(nameof(phase));
            if (phase == RunFailurePhase.NotStarted)
            {
                if (runInstanceKey != null || startTick.HasValue || nextExpectedTick.HasValue || acceptedFailure.HasValue || revision != 0)
                    throw new ArgumentException("Not-started snapshot cannot contain run state.");
            }
            else
            {
                RunFailureValidation.ValidateKey(runInstanceKey);
                if (!startTick.HasValue || startTick.Value.Value < 0 || startTick.Value.Value == int.MaxValue || revision == 0)
                    throw new ArgumentException("Started snapshot is malformed.");
                if (phase == RunFailurePhase.Active && (!nextExpectedTick.HasValue || acceptedFailure.HasValue))
                    throw new ArgumentException("Active snapshot must have only its next expected tick.");
                if (phase == RunFailurePhase.Failed && (nextExpectedTick.HasValue || !acceptedFailure.HasValue))
                    throw new ArgumentException("Failed snapshot must have exactly one accepted failure.");
            }
            Phase = phase;
            RunInstanceKey = runInstanceKey;
            StartTick = startTick;
            NextExpectedTick = nextExpectedTick;
            AcceptedFailure = acceptedFailure;
            Revision = revision;
            RunActive = phase == RunFailurePhase.Active;
        }

        internal RunFailurePhase Phase { get; }
        internal bool RunActive { get; }
        internal string RunInstanceKey { get; }
        internal SimulationTick? StartTick { get; }
        internal SimulationTick? NextExpectedTick { get; }
        internal AcceptedRunFailure? AcceptedFailure { get; }
        internal long Revision { get; }
    }

    internal readonly struct RunFailureResult
    {
        internal RunFailureResult(RunFailureDisposition disposition, RunFailureSnapshot snapshot,
            IReadOnlyList<RunFailureIntent> newlyEmittedIntents)
        {
            if (disposition != RunFailureDisposition.Applied && disposition != RunFailureDisposition.Duplicate)
                throw new ArgumentOutOfRangeException(nameof(disposition));
            Disposition = disposition;
            Snapshot = snapshot;
            NewlyEmittedIntents = CopyIntents(newlyEmittedIntents);
        }

        internal RunFailureDisposition Disposition { get; }
        internal RunFailureSnapshot Snapshot { get; }
        internal IReadOnlyList<RunFailureIntent> NewlyEmittedIntents { get; }

        private static IReadOnlyList<RunFailureIntent> CopyIntents(IReadOnlyList<RunFailureIntent> values)
        {
            var count = values == null ? 0 : values.Count;
            var copy = new RunFailureIntent[count];
            for (var index = 0; index < count; index++) copy[index] = values[index];
            return Array.AsReadOnly(copy);
        }
    }

    // REQ-M5D2-001..006. One instance owns exactly one active/failure prefix.
    internal sealed class RunFailureArbitrationSession
    {
        private RunFailurePhase _phase;
        private string _runInstanceKey;
        private SimulationTick _startTick;
        private SimulationTick _nextExpectedTick;
        private AcceptedRunFailure? _acceptedFailure;
        private long _revision;
        private RunStartInput? _startFingerprint;
        private RunFailureTickInput? _lastActiveFingerprint;
        private RunFailureTickInput? _terminalFingerprint;

        internal RunFailureSnapshot Snapshot => BuildSnapshot();

        internal RunFailureResult Start(RunStartInput input)
        {
            var normalized = Normalize(input);
            if (_startFingerprint.HasValue)
            {
                if (Same(_startFingerprint.Value, normalized)) return DuplicateResult();
                throw new InvalidOperationException("Run start conflicts with this instance-local run.");
            }
            if (_phase != RunFailurePhase.NotStarted) throw new InvalidOperationException("Run cannot be started from its current phase.");

            var snapshot = new RunFailureSnapshot(RunFailurePhase.Active, normalized.RunInstanceKey, normalized.StartTick,
                normalized.StartTick, null, 1);
            var result = new RunFailureResult(RunFailureDisposition.Applied, snapshot, Array.Empty<RunFailureIntent>());
            _runInstanceKey = normalized.RunInstanceKey;
            _startTick = normalized.StartTick;
            _nextExpectedTick = normalized.StartTick;
            _revision = 1;
            _startFingerprint = normalized;
            _phase = RunFailurePhase.Active;
            return result;
        }

        internal RunFailureResult ObserveTick(RunFailureTickInput input)
        {
            var normalized = Normalize(input);
            if (_terminalFingerprint.HasValue)
            {
                if (Same(_terminalFingerprint.Value, normalized)) return DuplicateResult();
                throw new InvalidOperationException("Failed run accepts no changed or subsequent failure batch.");
            }
            if (_phase != RunFailurePhase.Active) throw new InvalidOperationException("Failure tick requires an active run.");
            if (normalized.RunInstanceKey != _runInstanceKey) throw new InvalidOperationException("Failure tick belongs to a different run instance.");
            if (_lastActiveFingerprint.HasValue && normalized.Tick == _lastActiveFingerprint.Value.Tick)
            {
                if (Same(_lastActiveFingerprint.Value, normalized)) return DuplicateResult();
                throw new InvalidOperationException("Replayed active tick conflicts with its retained closed batch.");
            }
            if (normalized.Tick.Value < _nextExpectedTick.Value) throw new InvalidOperationException("Failure tick is stale.");
            if (normalized.Tick.Value > _nextExpectedTick.Value) throw new InvalidOperationException("Failure tick skips the exact sequential run tick.");

            // Tick and revision are intentionally separate checked numeric domains.
            var successorTick = new SimulationTick(checked(normalized.Tick.Value + 1));
            var successorRevision = checked(_revision + 1);
            if (normalized.TriggeredMask == RunFailureCauseMask.None)
            {
                var snapshot = new RunFailureSnapshot(RunFailurePhase.Active, _runInstanceKey, _startTick,
                    successorTick, null, successorRevision);
                var result = new RunFailureResult(RunFailureDisposition.Applied, snapshot, Array.Empty<RunFailureIntent>());
                _nextExpectedTick = successorTick;
                _revision = successorRevision;
                _lastActiveFingerprint = normalized;
                return result;
            }

            var observed = RunFailureValidation.OrderedObserved(normalized.TriggeredMask);
            var accepted = new AcceptedRunFailure(observed[0], normalized.Tick, normalized.PresenceMask,
                normalized.TriggeredMask, observed);
            var request = new RunEndRequested(RunResult.Failed, accepted.Cause, accepted.Tick);
            var intent = new RunFailureIntent(request, _runInstanceKey, successorRevision, normalized.PresenceMask,
                normalized.TriggeredMask, observed);
            var failedSnapshot = new RunFailureSnapshot(RunFailurePhase.Failed, _runInstanceKey, _startTick,
                null, accepted, successorRevision);
            var failedResult = new RunFailureResult(RunFailureDisposition.Applied, failedSnapshot, new[] { intent });
            _acceptedFailure = accepted;
            _revision = successorRevision;
            _terminalFingerprint = normalized;
            _phase = RunFailurePhase.Failed;
            return failedResult;
        }

        private RunFailureResult DuplicateResult() => new RunFailureResult(RunFailureDisposition.Duplicate, BuildSnapshot(), Array.Empty<RunFailureIntent>());

        private RunFailureSnapshot BuildSnapshot()
        {
            if (_phase == RunFailurePhase.NotStarted)
                return new RunFailureSnapshot(RunFailurePhase.NotStarted, null, null, null, null, 0);
            return _phase == RunFailurePhase.Active
                ? new RunFailureSnapshot(_phase, _runInstanceKey, _startTick, _nextExpectedTick, null, _revision)
                : new RunFailureSnapshot(_phase, _runInstanceKey, _startTick, null, _acceptedFailure, _revision);
        }

        private static RunStartInput Normalize(RunStartInput value) => new RunStartInput(value.RunInstanceKey, value.StartTick);
        private static RunFailureTickInput Normalize(RunFailureTickInput value) => new RunFailureTickInput(value.RunInstanceKey,
            value.Tick, value.PresenceMask, value.TriggeredMask);
        private static bool Same(RunStartInput left, RunStartInput right) => left.RunInstanceKey == right.RunInstanceKey && left.StartTick == right.StartTick;
        private static bool Same(RunFailureTickInput left, RunFailureTickInput right) => left.RunInstanceKey == right.RunInstanceKey &&
            left.Tick == right.Tick && left.PresenceMask == right.PresenceMask && left.TriggeredMask == right.TriggeredMask;
    }

    internal static class RunFailureValidation
    {
        private const string KeyCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._:/-";

        internal static bool IsKnown(RunFailurePhase value) => value >= RunFailurePhase.NotStarted && value <= RunFailurePhase.Failed;
        internal static bool IsKnown(RunFailureCause value) => value >= RunFailureCause.HealthDepleted && value <= RunFailureCause.LethalCrush;

        internal static void ValidateKey(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 96) throw new ArgumentException("Run instance key is invalid.");
            for (var index = 0; index < value.Length; index++)
                if (KeyCharacters.IndexOf(value[index]) < 0) throw new ArgumentException("Run instance key contains a non-authored character.");
        }

        internal static void ValidateClosedBatch(SimulationTick tick, RunFailureCauseMask presenceMask, RunFailureCauseMask triggeredMask)
        {
            if (tick.Value < 0 || presenceMask != RunFailureCauseMask.AllKnown ||
                (triggeredMask & ~RunFailureCauseMask.AllKnown) != 0)
                throw new ArgumentException("Failure batch is not a complete known-cause batch.");
        }

        internal static RunFailureCause[] OrderedObserved(RunFailureCauseMask triggeredMask)
        {
            if (triggeredMask == RunFailureCauseMask.None || (triggeredMask & ~RunFailureCauseMask.AllKnown) != 0)
                throw new ArgumentException("Observed failure causes must be non-empty and known.");
            var values = new List<RunFailureCause>(3);
            if ((triggeredMask & RunFailureCauseMask.HealthDepleted) != 0) values.Add(RunFailureCause.HealthDepleted);
            if ((triggeredMask & RunFailureCauseMask.KillPlane) != 0) values.Add(RunFailureCause.KillPlane);
            if ((triggeredMask & RunFailureCauseMask.LethalCrush) != 0) values.Add(RunFailureCause.LethalCrush);
            return values.ToArray();
        }

        internal static void ValidateAccepted(RunFailureCause cause, SimulationTick tick, RunFailureCauseMask presenceMask,
            RunFailureCauseMask triggeredMask, IReadOnlyList<RunFailureCause> observedCauses)
        {
            ValidateClosedBatch(tick, presenceMask, triggeredMask);
            var expected = OrderedObserved(triggeredMask);
            if (!IsKnown(cause) || observedCauses == null || observedCauses.Count != expected.Length || cause != expected[0])
                throw new ArgumentException("Accepted failure is not the canonical observed failure.");
            for (var index = 0; index < expected.Length; index++)
                if (observedCauses[index] != expected[index]) throw new ArgumentException("Observed failures are not in canonical order.");
        }

        internal static IReadOnlyList<RunFailureCause> CopyCauses(IReadOnlyList<RunFailureCause> values)
        {
            var copy = new RunFailureCause[values.Count];
            for (var index = 0; index < copy.Length; index++) copy[index] = values[index];
            return Array.AsReadOnly(copy);
        }
    }
}
