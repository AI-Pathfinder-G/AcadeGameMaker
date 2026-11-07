using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Run
{
    internal enum PostBossPhase { AwaitingDeath, AwaitingCleanup, ChoiceRequired, BarrierPreparationRequired, Suppressed }
    internal enum PostBossDisposition { Applied, Duplicate }
    internal enum PostBossFailureCause { HealthDepleted, KillPlane, LethalCrush }
    internal enum PostBossChoice { None, Extraction, Solidarity }
    internal enum PostBossSkill { None, CompressionVerdict, CommonReferencePlane }
    internal enum PostBossHandoffKind { Defeated, RewardRequest, RoomCompletionRequest }
    internal enum PostBossCleanupDisposition { Removed, LifecycleSuperseded }
    internal enum PostBossLifecycleReason { RoomLeaving, RunFailed, Cutscene, DemoCompleted }
    internal enum PostBossIntentKind { TransitionRequested, RewardRequestPending, BossSegmentCompleted, PrepareChoice, PrepareBarrier }

    internal readonly struct PostBossFailure
    {
        internal PostBossFailure(PostBossFailureCause cause, SimulationTick tick)
        {
            if (!IsValid(cause) || tick.Value < 0)
                throw new ArgumentException($"Accepted failure needs known non-negative cause/tick; cause={cause}, tick={tick.Value}.");
            Cause = cause;
            Tick = tick;
        }

        internal PostBossFailureCause Cause { get; }
        internal SimulationTick Tick { get; }

        internal static bool IsValid(PostBossFailureCause value) =>
            value >= PostBossFailureCause.HealthDepleted && value <= PostBossFailureCause.LethalCrush;
    }

    internal readonly struct PostBossPair
    {
        internal PostBossPair(PostBossChoice choice, PostBossSkill skill)
        {
            if (!IsValid(choice, skill))
                throw new ArgumentException($"Persisted pair is not closed; choice={choice}, skill={skill}.");
            Choice = choice;
            Skill = skill;
        }

        internal PostBossChoice Choice { get; }
        internal PostBossSkill Skill { get; }

        internal static bool IsValid(PostBossChoice choice, PostBossSkill skill) =>
            (choice == PostBossChoice.None && skill == PostBossSkill.None)
            || (choice == PostBossChoice.Extraction && skill == PostBossSkill.CompressionVerdict)
            || (choice == PostBossChoice.Solidarity && skill == PostBossSkill.CommonReferencePlane);
    }

    internal readonly struct PostBossHandoff
    {
        internal PostBossHandoff(PostBossHandoffKind kind, SimulationTick tick)
        {
            if (!IsValid(kind) || tick.Value < 0)
                throw new ArgumentException($"Handoff needs known non-negative kind/tick; kind={kind}, tick={tick.Value}.");
            Kind = kind;
            Tick = tick;
        }

        internal PostBossHandoffKind Kind { get; }
        internal SimulationTick Tick { get; }

        internal static bool IsValid(PostBossHandoffKind value) =>
            value >= PostBossHandoffKind.Defeated && value <= PostBossHandoffKind.RoomCompletionRequest;
    }

    // REQ-COM-004, REQ-RUN-001, REQ-RUN-004, REQ-CHOICE-004, REQ-CHOICE-005.
    internal readonly struct PostBossDeathInput
    {
        internal PostBossDeathInput(string key, SimulationTick deathTick, IReadOnlyList<PostBossHandoff> handoffs, int bossHealth, bool bossDead, int playerHealth, bool playerDead, bool runActive, PostBossFailure? acceptedFailure, PostBossPair pair)
        {
            if (key != PostBossProgressionSession.OrdanEncounterKey)
                throw new ArgumentException($"Death key must be {PostBossProgressionSession.OrdanEncounterKey}; key={key ?? "<null>"}.");
            if (deathTick.Value < 0 || deathTick.Value == int.MaxValue)
                throw new ArgumentException($"Death tick must be non-negative and have a successor; tick={deathTick.Value}.");
            if (handoffs == null || handoffs.Count != 3
                || !Expected(handoffs, 0, PostBossHandoffKind.Defeated, deathTick)
                || !Expected(handoffs, 1, PostBossHandoffKind.RewardRequest, deathTick)
                || !Expected(handoffs, 2, PostBossHandoffKind.RoomCompletionRequest, deathTick))
                throw new ArgumentException($"Death handoffs must be ordered Defeated/RewardRequest/RoomCompletionRequest at tick={deathTick.Value}.");
            if (bossHealth != 0 || !bossDead)
                throw new ArgumentException($"Death evidence must report boss health=0 and dead=true; health={bossHealth}, dead={bossDead}.");
            if (playerHealth < 0 || playerHealth > 5 || playerDead != (playerHealth == 0))
                throw new ArgumentException($"Player evidence is inconsistent; health={playerHealth}, dead={playerDead}.");
            if (acceptedFailure.HasValue && (!PostBossFailure.IsValid(acceptedFailure.Value.Cause)
                || acceptedFailure.Value.Tick.Value < 0 || acceptedFailure.Value.Tick.Value > deathTick.Value))
                throw new ArgumentException($"Accepted failure must be known and no later than death tick={deathTick.Value}; cause={acceptedFailure.Value.Cause}, tick={acceptedFailure.Value.Tick.Value}.");
            if (!PostBossPair.IsValid(pair.Choice, pair.Skill))
                throw new ArgumentException($"Persisted pair is not closed; choice={pair.Choice}, skill={pair.Skill}.");

            Key = key;
            DeathTick = deathTick;
            Handoffs = CopyHandoffs(handoffs);
            BossHealth = bossHealth;
            BossDead = bossDead;
            PlayerHealth = playerHealth;
            PlayerDead = playerDead;
            RunActive = runActive;
            AcceptedFailure = acceptedFailure;
            Pair = pair;
        }

        internal string Key { get; }
        internal SimulationTick DeathTick { get; }
        internal IReadOnlyList<PostBossHandoff> Handoffs { get; }
        internal int BossHealth { get; }
        internal bool BossDead { get; }
        internal int PlayerHealth { get; }
        internal bool PlayerDead { get; }
        internal bool RunActive { get; }
        internal PostBossFailure? AcceptedFailure { get; }
        internal PostBossPair Pair { get; }

        private static bool Expected(IReadOnlyList<PostBossHandoff> handoffs, int index, PostBossHandoffKind kind, SimulationTick tick) =>
            handoffs[index].Kind == kind && handoffs[index].Tick == tick;

        private static IReadOnlyList<PostBossHandoff> CopyHandoffs(IReadOnlyList<PostBossHandoff> handoffs)
        {
            var copy = new PostBossHandoff[handoffs.Count];
            for (var index = 0; index < copy.Length; index++) copy[index] = handoffs[index];
            return Array.AsReadOnly(copy);
        }
    }

    internal readonly struct PostBossCleanupInput
    {
        internal PostBossCleanupInput(string key, SimulationTick deathTick, SimulationTick cleanupTick, PostBossCleanupDisposition disposition, PostBossLifecycleReason? lifecycle, bool registrationsAbsent, bool durableRemovalCommitted, bool playerBaseline, bool activeTargetEmpty, bool bossTeardownCommitted)
        {
            if (key != PostBossProgressionSession.OrdanEncounterKey)
                throw new ArgumentException($"Cleanup key must be {PostBossProgressionSession.OrdanEncounterKey}; key={key ?? "<null>"}.");
            if (deathTick.Value < 0 || cleanupTick.Value < 0)
                throw new ArgumentException($"Cleanup tick must equal death tick + 1; death={deathTick.Value}, cleanup={cleanupTick.Value}.");
            var expectedCleanupTick = checked(deathTick.Value + 1);
            if (cleanupTick.Value != expectedCleanupTick)
                throw new ArgumentException($"Cleanup tick must equal death tick + 1; death={deathTick.Value}, cleanup={cleanupTick.Value}.");
            if (!IsValid(disposition)) throw new ArgumentException($"Cleanup disposition is unknown: {disposition}.");
            if (!registrationsAbsent || !playerBaseline || !activeTargetEmpty || !bossTeardownCommitted)
                throw new ArgumentException("Cleanup evidence must prove absent registrations, baseline player, empty targets, and committed teardown.");
            if (disposition == PostBossCleanupDisposition.Removed && (lifecycle.HasValue || !durableRemovalCommitted))
                throw new ArgumentException("Removed cleanup requires no lifecycle reason and committed durable removal.");
            if (disposition == PostBossCleanupDisposition.LifecycleSuperseded
                && (durableRemovalCommitted || !lifecycle.HasValue || !IsValid(lifecycle.Value)))
                throw new ArgumentException("Lifecycle-superseded cleanup requires known lifecycle reason and no durable removal.");

            Key = key;
            DeathTick = deathTick;
            CleanupTick = cleanupTick;
            Disposition = disposition;
            Lifecycle = lifecycle;
            RegistrationsAbsent = registrationsAbsent;
            DurableRemovalCommitted = durableRemovalCommitted;
            PlayerBaseline = playerBaseline;
            ActiveTargetEmpty = activeTargetEmpty;
            BossTeardownCommitted = bossTeardownCommitted;
        }

        internal string Key { get; }
        internal SimulationTick DeathTick { get; }
        internal SimulationTick CleanupTick { get; }
        internal PostBossCleanupDisposition Disposition { get; }
        internal PostBossLifecycleReason? Lifecycle { get; }
        internal bool RegistrationsAbsent { get; }
        internal bool DurableRemovalCommitted { get; }
        internal bool PlayerBaseline { get; }
        internal bool ActiveTargetEmpty { get; }
        internal bool BossTeardownCommitted { get; }

        private static bool IsValid(PostBossCleanupDisposition value) =>
            value >= PostBossCleanupDisposition.Removed && value <= PostBossCleanupDisposition.LifecycleSuperseded;

        private static bool IsValid(PostBossLifecycleReason value) =>
            value >= PostBossLifecycleReason.RoomLeaving && value <= PostBossLifecycleReason.DemoCompleted;
    }

    internal readonly struct PostBossIntent
    {
        internal PostBossIntent(PostBossIntentKind kind, string key, SimulationTick deathTick, SimulationTick? cleanupTick, PostBossPair pair)
        {
            Kind = kind;
            Key = key;
            DeathTick = deathTick;
            CleanupTick = cleanupTick;
            Pair = pair;
            BlockNewFailureRequests = kind == PostBossIntentKind.TransitionRequested;
            SourceId = kind == PostBossIntentKind.RewardRequestPending ? "ordan" : string.Empty;
        }

        internal PostBossIntentKind Kind { get; }
        internal string Key { get; }
        internal SimulationTick DeathTick { get; }
        internal SimulationTick? CleanupTick { get; }
        internal PostBossPair Pair { get; }
        internal bool BlockNewFailureRequests { get; }
        internal string SourceId { get; }
    }

    internal readonly struct PostBossSnapshot
    {
        internal PostBossSnapshot(PostBossPhase phase, PostBossDeathInput? death, PostBossCleanupInput? cleanup)
        {
            Phase = phase;
            Death = death;
            Cleanup = cleanup;
            RewardRequestPending = phase == PostBossPhase.ChoiceRequired || phase == PostBossPhase.BarrierPreparationRequired;
            BossSegmentCompleted = RewardRequestPending;
        }

        internal PostBossPhase Phase { get; }
        internal PostBossDeathInput? Death { get; }
        internal PostBossCleanupInput? Cleanup { get; }
        internal bool RewardRequestPending { get; }
        internal bool BossSegmentCompleted { get; }
    }

    internal readonly struct PostBossResult
    {
        internal PostBossResult(PostBossDisposition disposition, PostBossSnapshot snapshot, IReadOnlyList<PostBossIntent> intents)
        {
            Disposition = disposition;
            Snapshot = snapshot;
            Intents = CopyIntents(intents);
        }

        internal PostBossDisposition Disposition { get; }
        internal PostBossSnapshot Snapshot { get; }
        internal IReadOnlyList<PostBossIntent> Intents { get; }

        private static IReadOnlyList<PostBossIntent> CopyIntents(IReadOnlyList<PostBossIntent> intents)
        {
            var count = intents == null ? 0 : intents.Count;
            var copy = new PostBossIntent[count];
            for (var index = 0; index < count; index++) copy[index] = intents[index];
            return Array.AsReadOnly(copy);
        }
    }

    // REQ-COM-004, REQ-RUN-001, REQ-RUN-004, REQ-CHOICE-004, REQ-CHOICE-005.
    internal sealed class PostBossProgressionSession
    {
        internal const string OrdanEncounterKey = "OrdanBossEncounterGraph";

        private PostBossPhase _phase = PostBossPhase.AwaitingDeath;
        private PostBossDeathInput? _death;
        private PostBossCleanupInput? _cleanup;

        internal PostBossSnapshot Snapshot => new PostBossSnapshot(_phase, _death, _cleanup);

        internal PostBossResult ObserveDeath(PostBossDeathInput input)
        {
            var normalized = Normalize(input);
            if (_death.HasValue)
            {
                if (Same(_death.Value, normalized)) return DuplicateResult();
                throw new InvalidOperationException($"Death conflicts with recorded value; key={normalized.Key}, tick={normalized.DeathTick.Value}.");
            }
            if (_phase != PostBossPhase.AwaitingDeath)
                throw new InvalidOperationException($"Death cannot be observed while phase={_phase}.");

            var suppressed = normalized.PlayerDead || normalized.AcceptedFailure.HasValue || !normalized.RunActive;
            var nextPhase = suppressed ? PostBossPhase.Suppressed : PostBossPhase.AwaitingCleanup;
            IReadOnlyList<PostBossIntent> intents = suppressed
                ? Array.Empty<PostBossIntent>()
                : new[] { new PostBossIntent(PostBossIntentKind.TransitionRequested, normalized.Key, normalized.DeathTick, null, normalized.Pair) };
            var snapshot = new PostBossSnapshot(nextPhase, normalized, null);
            var result = new PostBossResult(PostBossDisposition.Applied, snapshot, intents);

            _death = normalized;
            _phase = nextPhase;
            return result;
        }

        internal PostBossResult CompleteCleanup(PostBossCleanupInput input)
        {
            var normalized = Normalize(input);
            if (!_death.HasValue)
                throw new InvalidOperationException($"Cleanup has no recorded death; key={normalized.Key}, deathTick={normalized.DeathTick.Value}.");
            if (_cleanup.HasValue)
            {
                if (Same(_cleanup.Value, normalized)) return DuplicateResult();
                throw new InvalidOperationException($"Cleanup conflicts with recorded value; key={normalized.Key}, tick={normalized.CleanupTick.Value}.");
            }
            if (_phase != PostBossPhase.AwaitingCleanup || normalized.Key != _death.Value.Key || normalized.DeathTick != _death.Value.DeathTick)
                throw new InvalidOperationException($"Cleanup does not match awaiting death; phase={_phase}, key={normalized.Key}, deathTick={normalized.DeathTick.Value}.");

            var lifecycle = normalized.Disposition == PostBossCleanupDisposition.LifecycleSuperseded;
            var nextPhase = lifecycle ? PostBossPhase.Suppressed
                : _death.Value.Pair.Choice == PostBossChoice.None ? PostBossPhase.ChoiceRequired : PostBossPhase.BarrierPreparationRequired;
            IReadOnlyList<PostBossIntent> intents = lifecycle ? Array.Empty<PostBossIntent>() : CompletionIntents(_death.Value, normalized, nextPhase);
            var snapshot = new PostBossSnapshot(nextPhase, _death, normalized);
            var result = new PostBossResult(PostBossDisposition.Applied, snapshot, intents);

            _cleanup = normalized;
            _phase = nextPhase;
            return result;
        }

        private static PostBossIntent[] CompletionIntents(PostBossDeathInput death, PostBossCleanupInput cleanup, PostBossPhase phase)
        {
            return new[]
            {
                new PostBossIntent(PostBossIntentKind.RewardRequestPending, cleanup.Key, death.DeathTick, cleanup.CleanupTick, death.Pair),
                new PostBossIntent(PostBossIntentKind.BossSegmentCompleted, cleanup.Key, death.DeathTick, cleanup.CleanupTick, death.Pair),
                new PostBossIntent(phase == PostBossPhase.ChoiceRequired ? PostBossIntentKind.PrepareChoice : PostBossIntentKind.PrepareBarrier, cleanup.Key, death.DeathTick, cleanup.CleanupTick, death.Pair),
            };
        }

        private PostBossResult DuplicateResult() => new PostBossResult(PostBossDisposition.Duplicate, Snapshot, Array.Empty<PostBossIntent>());

        private static PostBossDeathInput Normalize(PostBossDeathInput value) => new PostBossDeathInput(value.Key, value.DeathTick, value.Handoffs, value.BossHealth, value.BossDead, value.PlayerHealth, value.PlayerDead, value.RunActive, value.AcceptedFailure, value.Pair);
        private static PostBossCleanupInput Normalize(PostBossCleanupInput value) => new PostBossCleanupInput(value.Key, value.DeathTick, value.CleanupTick, value.Disposition, value.Lifecycle, value.RegistrationsAbsent, value.DurableRemovalCommitted, value.PlayerBaseline, value.ActiveTargetEmpty, value.BossTeardownCommitted);

        private static bool Same(PostBossDeathInput left, PostBossDeathInput right) =>
            left.Key == right.Key && left.DeathTick == right.DeathTick && left.BossHealth == right.BossHealth && left.BossDead == right.BossDead
            && left.PlayerHealth == right.PlayerHealth && left.PlayerDead == right.PlayerDead && left.RunActive == right.RunActive
            && Same(left.AcceptedFailure, right.AcceptedFailure) && left.Pair.Choice == right.Pair.Choice && left.Pair.Skill == right.Pair.Skill
            && Same(left.Handoffs, right.Handoffs);

        private static bool Same(PostBossCleanupInput left, PostBossCleanupInput right) =>
            left.Key == right.Key && left.DeathTick == right.DeathTick && left.CleanupTick == right.CleanupTick && left.Disposition == right.Disposition
            && left.Lifecycle == right.Lifecycle && left.RegistrationsAbsent == right.RegistrationsAbsent && left.DurableRemovalCommitted == right.DurableRemovalCommitted
            && left.PlayerBaseline == right.PlayerBaseline && left.ActiveTargetEmpty == right.ActiveTargetEmpty && left.BossTeardownCommitted == right.BossTeardownCommitted;

        private static bool Same(PostBossFailure? left, PostBossFailure? right) => left.HasValue == right.HasValue && (!left.HasValue || (left.Value.Cause == right.Value.Cause && left.Value.Tick == right.Value.Tick));

        private static bool Same(IReadOnlyList<PostBossHandoff> left, IReadOnlyList<PostBossHandoff> right)
        {
            if (left.Count != right.Count) return false;
            for (var index = 0; index < left.Count; index++)
                if (left[index].Kind != right[index].Kind || left[index].Tick != right[index].Tick) return false;
            return true;
        }
    }
}
