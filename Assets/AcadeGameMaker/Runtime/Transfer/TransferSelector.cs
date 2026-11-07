using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Transfer
{
    // REQ-WT-004, REQ-WT-006, REQ-WT-008. Integer-only deterministic selector.
    internal static class TransferSelector
    {
        internal static int PlayerDistanceKey(int playerXQ1000, int playerYQ1000, int aimXQ1000, int aimYQ1000)
        {
            var dx = checked((long)aimXQ1000 - playerXQ1000); var dy = checked((long)aimYQ1000 - playerYQ1000);
            var numerator = checked(dx * dx + dy * dy); var result = checked((numerator + 500L) / 1000L);
            return checked((int)result);
        }

        internal static TransferSelectionSnapshot Select(SimulationTick tick, AimSample sample, IReadOnlyList<TransferCandidateObservation> observations, string previousHighlighted)
        {
            if (sample.SampleTick != tick) return None(tick, Source(sample.Source), TransferFailureReason.StaleAimSample);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            Candidate? best = null; Candidate? retained = null; var failure = TransferFailureReason.InvalidTarget;
            for (var index = 0; index < observations.Count; index++)
            {
                var observation = observations[index];
                TransferValidation.TargetId(observation.Descriptor.TargetId);
                if (observation.ObservationTick != tick || !seen.Add(observation.Descriptor.TargetId)) throw new ArgumentException("Observations must be unique and exact-tick.");
                var invalid = Invalid(observation);
                if (invalid != TransferFailureReason.None) { failure = Prefer(failure, invalid); continue; }
                if (sample.Source == AimSource.MousePointer)
                {
                    if (!sample.IsPointerInsideGameplayRect.GetValueOrDefault() || !(observation.MouseInsideShape || observation.ScreenDistanceSquaredKey <= 576)) { failure = Prefer(failure, TransferFailureReason.InvalidTarget); continue; }
                    var candidate = new Candidate(observation, observation.MouseInsideShape ? 0 : 1); if (!best.HasValue || CompareMouse(candidate, best.Value) < 0) best = candidate;
                }
                else
                {
                    var candidate = new Candidate(observation, -1);
                    if (observation.Descriptor.TargetId == previousHighlighted && observation.GamepadAngleKey <= 260) retained = candidate;
                    if (observation.GamepadAngleKey <= 180 && (!best.HasValue || CompareGamepad(candidate, best.Value) < 0)) best = candidate;
                }
            }
            if (sample.Source == AimSource.GamepadStick && retained.HasValue)
            {
                if (!best.HasValue || best.Value.Observation.Descriptor.TargetId == retained.Value.Observation.Descriptor.TargetId || checked(best.Value.Observation.GamepadAngleKey + 40) > retained.Value.Observation.GamepadAngleKey) best = retained;
            }
            return best.HasValue ? Snapshot(tick, Source(sample.Source), best.Value) : None(tick, Source(sample.Source), failure);
        }

        private static TransferFailureReason Invalid(TransferCandidateObservation o)
        {
            if (!o.Descriptor.IsAvailable) return TransferFailureReason.TargetUnavailable;
            if (o.IsTrigger) return TransferFailureReason.TriggerTarget;
            if (o.PlayerDistanceKey > 36000) return TransferFailureReason.OutOfRange;
            if (!o.IsLineOfSightOpen) return TransferFailureReason.LineOfSightBlocked;
            return TransferFailureReason.None;
        }
        private static TransferFailureReason Prefer(TransferFailureReason current, TransferFailureReason next) => Rank(next) < Rank(current) ? next : current;
        private static int Rank(TransferFailureReason value) { switch (value) { case TransferFailureReason.TargetUnavailable:return 0; case TransferFailureReason.TriggerTarget:return 1; case TransferFailureReason.OutOfRange:return 2; case TransferFailureReason.LineOfSightBlocked:return 3; default:return 4; } }
        private static int CompareMouse(Candidate a, Candidate b) { var v=a.InsideRank.CompareTo(b.InsideRank); if(v!=0)return v;v=a.Observation.ScreenDistanceSquaredKey.CompareTo(b.Observation.ScreenDistanceSquaredKey);if(v!=0)return v;v=a.Observation.PlayerDistanceKey.CompareTo(b.Observation.PlayerDistanceKey);return v!=0?v:string.CompareOrdinal(a.Observation.Descriptor.TargetId,b.Observation.Descriptor.TargetId); }
        private static int CompareGamepad(Candidate a, Candidate b) { var v=a.Observation.GamepadAngleKey.CompareTo(b.Observation.GamepadAngleKey);if(v!=0)return v;v=a.Observation.PlayerDistanceKey.CompareTo(b.Observation.PlayerDistanceKey);return v!=0?v:string.CompareOrdinal(a.Observation.Descriptor.TargetId,b.Observation.Descriptor.TargetId); }
        private static TransferSelectionSource Source(AimSource source) => source == AimSource.MousePointer ? TransferSelectionSource.MousePointer : TransferSelectionSource.GamepadStick;
        private static TransferSelectionSnapshot Snapshot(SimulationTick tick,TransferSelectionSource source,Candidate c) => new TransferSelectionSnapshot(tick,source,c.Observation.Descriptor.TargetId,c.InsideRank,source==TransferSelectionSource.MousePointer?c.Observation.ScreenDistanceSquaredKey:-1,c.Observation.PlayerDistanceKey,source==TransferSelectionSource.GamepadStick?c.Observation.GamepadAngleKey:-1,TransferFailureReason.None);
        private static TransferSelectionSnapshot None(SimulationTick tick,TransferSelectionSource source,TransferFailureReason reason) => new TransferSelectionSnapshot(tick,source,string.Empty,-1,-1,-1,-1,reason);
        private readonly struct Candidate { internal Candidate(TransferCandidateObservation observation,int insideRank){Observation=observation;InsideRank=insideRank;} internal TransferCandidateObservation Observation{get;}internal int InsideRank{get;} }
    }
}
