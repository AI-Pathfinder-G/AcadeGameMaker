using System;
using System.Collections.Generic;
using System.Linq;
using AcadeGameMaker.Core;
using AcadeGameMaker.Run;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Run
{
    // REQ-COM-004, REQ-RUN-001, REQ-RUN-004, REQ-CHOICE-004, REQ-CHOICE-005.
    // AC-M5A-001..009: engine-free, independent tests for the two-stage progression core.
    public sealed class PostBossProgressionSessionTests
    {
        private const string Key = "OrdanBossEncounterGraph";

        [Test]
        public void AC001_AliveDeathEmitsOneTransitionAndFreezesSource()
        {
            var handoffs = Handoffs();
            var pair = new PostBossPair(PostBossChoice.Extraction, PostBossSkill.CompressionVerdict);
            var session = new PostBossProgressionSession();
            var result = session.ObserveDeath(Death(10, 5, true, null, pair, handoffs));

            handoffs[0] = new PostBossHandoff(PostBossHandoffKind.RoomCompletionRequest, Tick(10));
            Assert.That(result.Disposition, Is.EqualTo(PostBossDisposition.Applied));
            Assert.That(result.Snapshot.Phase, Is.EqualTo(PostBossPhase.AwaitingCleanup));
            Assert.That(result.Snapshot.RewardRequestPending, Is.False);
            Assert.That(result.Snapshot.BossSegmentCompleted, Is.False);
            Assert.That(result.Intents.Count, Is.EqualTo(1));
            AssertIntent(result.Intents[0], PostBossIntentKind.TransitionRequested, 10, null, pair);
            Assert.That(result.Intents[0].BlockNewFailureRequests, Is.True);
            Assert.That(result.Intents[0].SourceId, Is.Empty);
            Assert.That(result.Snapshot.Death.Value.Handoffs[0].Kind, Is.EqualTo(PostBossHandoffKind.Defeated));
            Assert.That(result.Snapshot.Death.Value.Pair, Is.EqualTo(pair));
            Assert.That(result.Snapshot.Cleanup.HasValue, Is.False);
        }

        [Test]
        public void AC002_AC003_NormalCleanupEmitsOrderedThreeIntentsForAllRoutes()
        {
            var pairs = new[]
            {
                new PostBossPair(PostBossChoice.None, PostBossSkill.None),
                new PostBossPair(PostBossChoice.Extraction, PostBossSkill.CompressionVerdict),
                new PostBossPair(PostBossChoice.Solidarity, PostBossSkill.CommonReferencePlane)
            };

            foreach (var pair in pairs)
            {
                var session = new PostBossProgressionSession();
                session.ObserveDeath(Death(10, 5, true, null, pair));
                var result = session.CompleteCleanup(Cleanup(10));

                Assert.That(result.Disposition, Is.EqualTo(PostBossDisposition.Applied));
                Assert.That(result.Intents.Count, Is.EqualTo(3));
                Assert.That(result.Intents[0].Kind, Is.EqualTo(PostBossIntentKind.RewardRequestPending));
                Assert.That(result.Intents[1].Kind, Is.EqualTo(PostBossIntentKind.BossSegmentCompleted));
                Assert.That(result.Intents[2].Kind, Is.EqualTo(pair.Choice == PostBossChoice.None
                    ? PostBossIntentKind.PrepareChoice : PostBossIntentKind.PrepareBarrier));
                for (var i = 0; i < result.Intents.Count; i++)
                {
                    Assert.That(result.Intents[i].Key, Is.EqualTo(Key));
                    Assert.That(result.Intents[i].DeathTick.Value, Is.EqualTo(10));
                    Assert.That(result.Intents[i].CleanupTick.Value.Value, Is.EqualTo(11));
                    Assert.That(result.Intents[i].Pair, Is.EqualTo(pair));
                    Assert.That(result.Intents[i].BlockNewFailureRequests, Is.False);
                    Assert.That(result.Intents[i].SourceId, Is.EqualTo(i == 0 ? "ordan" : string.Empty));
                }
                Assert.That(result.Snapshot.Phase, Is.EqualTo(pair.Choice == PostBossChoice.None
                    ? PostBossPhase.ChoiceRequired : PostBossPhase.BarrierPreparationRequired));
                Assert.That(result.Snapshot.RewardRequestPending, Is.True);
                Assert.That(result.Snapshot.BossSegmentCompleted, Is.True);
            }
        }

        [Test]
        public void AC003_InvalidAndUnknownChoiceSkillPairsFailBeforeSessionMutation()
        {
            Assert.Throws<ArgumentException>(() => new PostBossPair(PostBossChoice.None, PostBossSkill.CompressionVerdict));
            Assert.Throws<ArgumentException>(() => new PostBossPair(PostBossChoice.Extraction, PostBossSkill.None));
            Assert.Throws<ArgumentException>(() => new PostBossPair(PostBossChoice.Solidarity, PostBossSkill.CompressionVerdict));
            Assert.Throws<ArgumentException>(() => new PostBossPair((PostBossChoice)99, PostBossSkill.None));
            Assert.Throws<ArgumentException>(() => new PostBossPair(PostBossChoice.None, (PostBossSkill)99));

            var session = new PostBossProgressionSession();
            Assert.Throws<ArgumentException>(() => session.ObserveDeath(default(PostBossDeathInput)));
            Assert.That(session.Snapshot.Phase, Is.EqualTo(PostBossPhase.AwaitingDeath));

            var explicitNone = new PostBossProgressionSession();
            Assert.DoesNotThrow(() => explicitNone.ObserveDeath(Death(10, 5, true, null, default(PostBossPair))));
        }

        [Test]
        public void AC004_FailurePrecedenceAndHealthFactsSuppressOrReject()
        {
            var suppressed = new List<PostBossDeathInput>
            {
                Death(10, 0, true),
                Death(10, 5, false),
                Death(10, 5, true, new PostBossFailure(PostBossFailureCause.HealthDepleted, Tick(9))),
                Death(10, 5, true, new PostBossFailure(PostBossFailureCause.KillPlane, Tick(10))),
                Death(10, 5, true, new PostBossFailure(PostBossFailureCause.LethalCrush, Tick(10)))
            };
            foreach (var input in suppressed)
            {
                var result = new PostBossProgressionSession().ObserveDeath(input);
                Assert.That(result.Disposition, Is.EqualTo(PostBossDisposition.Applied));
                Assert.That(result.Snapshot.Phase, Is.EqualTo(PostBossPhase.Suppressed));
                Assert.That(result.Intents, Is.Empty);
            }

            Assert.Throws<ArgumentException>(() => new PostBossFailure((PostBossFailureCause)99, Tick(1)));
            Assert.Throws<ArgumentException>(() => new PostBossFailure(PostBossFailureCause.HealthDepleted, Tick(-1)));
            Assert.Throws<ArgumentException>(() => new PostBossHandoff((PostBossHandoffKind)99, Tick(1)));
            Assert.Throws<ArgumentException>(() => new PostBossHandoff(PostBossHandoffKind.Defeated, Tick(-1)));
            Assert.Throws<ArgumentException>(() => Death(10, 4, true, new PostBossFailure(PostBossFailureCause.HealthDepleted, Tick(11))));
            Assert.Throws<ArgumentException>(() => Death(10, 0, true, null, null, null, false));
            Assert.Throws<ArgumentException>(() => new PostBossDeathInput(Key, Tick(10), Handoffs(10), 0, true, 5, true, true, null,
                new PostBossPair(PostBossChoice.None, PostBossSkill.None)));
        }

        [Test]
        public void AC005_AllLifecycleReasonsSuppressNormalCompletionAndRejectBadProof()
        {
            foreach (PostBossLifecycleReason reason in Enum.GetValues(typeof(PostBossLifecycleReason)))
            {
                var session = new PostBossProgressionSession();
                session.ObserveDeath(Death(10));
                var result = session.CompleteCleanup(LifecycleCleanup(10, reason));
                Assert.That(result.Disposition, Is.EqualTo(PostBossDisposition.Applied));
                Assert.That(result.Snapshot.Phase, Is.EqualTo(PostBossPhase.Suppressed));
                Assert.That(result.Intents, Is.Empty);
            }

            Assert.Throws<ArgumentException>(() => LifecycleCleanup(10, PostBossLifecycleReason.RoomLeaving, true));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(Key, Tick(10), Tick(11),
                PostBossCleanupDisposition.Removed, PostBossLifecycleReason.RoomLeaving, true, true, true, true, true));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(Key, Tick(10), Tick(11),
                PostBossCleanupDisposition.LifecycleSuperseded, null, true, false, true, true, true));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(Key, Tick(10), Tick(11),
                (PostBossCleanupDisposition)99, PostBossLifecycleReason.RoomLeaving, true, false, true, true, true));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(Key, Tick(10), Tick(11),
                PostBossCleanupDisposition.LifecycleSuperseded, (PostBossLifecycleReason)99, true, false, true, true, true));
        }

        [Test]
        public void AC006_ReplaysAreDuplicatesConflictsAreAtomicAndSessionsAreIndependent()
        {
            var death = Death(10);
            var session = new PostBossProgressionSession();
            var first = session.ObserveDeath(death);
            var replay = session.ObserveDeath(death);
            Assert.That(replay.Disposition, Is.EqualTo(PostBossDisposition.Duplicate));
            Assert.That(replay.Intents, Is.Empty);
            Assert.That(replay.Snapshot.Phase, Is.EqualTo(PostBossPhase.AwaitingCleanup));

            Assert.Throws<InvalidOperationException>(() => session.ObserveDeath(Death(10, 4)));
            Assert.That(session.Snapshot.Phase, Is.EqualTo(PostBossPhase.AwaitingCleanup));
            Assert.Throws<ArgumentException>(() => new PostBossDeathInput("other", Tick(10), Handoffs(10), 0, true, 5, false, true, null,
                new PostBossPair(PostBossChoice.None, PostBossSkill.None)));
            Assert.Throws<InvalidOperationException>(() => session.CompleteCleanup(Cleanup(9)));
            Assert.That(session.Snapshot.Phase, Is.EqualTo(PostBossPhase.AwaitingCleanup));
            var cleanup = Cleanup(10);
            var completed = session.CompleteCleanup(cleanup);
            var cleanupReplay = session.CompleteCleanup(cleanup);
            Assert.That(cleanupReplay.Disposition, Is.EqualTo(PostBossDisposition.Duplicate));
            Assert.That(cleanupReplay.Intents, Is.Empty);
            Assert.That(completed.Snapshot.Phase, Is.EqualTo(PostBossPhase.ChoiceRequired));
            Assert.Throws<InvalidOperationException>(() => session.CompleteCleanup(LifecycleCleanup(10, PostBossLifecycleReason.RoomLeaving)));
            var completedDeathReplay = session.ObserveDeath(death);
            Assert.That(completedDeathReplay.Disposition, Is.EqualTo(PostBossDisposition.Duplicate));
            Assert.That(completedDeathReplay.Intents, Is.Empty);
            Assert.That(session.Snapshot.Phase, Is.EqualTo(PostBossPhase.ChoiceRequired));

            var suppressedSession = new PostBossProgressionSession();
            suppressedSession.ObserveDeath(Death(10, 0));
            var lifecycle = LifecycleCleanup(10, PostBossLifecycleReason.RoomLeaving);
            Assert.Throws<InvalidOperationException>(() => suppressedSession.CompleteCleanup(lifecycle));
            Assert.That(suppressedSession.Snapshot.Phase, Is.EqualTo(PostBossPhase.Suppressed));
            var lifecycleApplied = new PostBossProgressionSession();
            lifecycleApplied.ObserveDeath(Death(10));
            lifecycleApplied.CompleteCleanup(lifecycle);
            var lifecycleReplay = lifecycleApplied.CompleteCleanup(lifecycle);
            Assert.That(lifecycleReplay.Disposition, Is.EqualTo(PostBossDisposition.Duplicate));
            Assert.That(lifecycleReplay.Intents, Is.Empty);

            var independent = new PostBossProgressionSession();
            Assert.That(independent.ObserveDeath(death).Disposition, Is.EqualTo(PostBossDisposition.Applied));
            Assert.That(first.Snapshot.Phase, Is.EqualTo(PostBossPhase.AwaitingCleanup));
        }

        [Test]
        public void AC007_DefaultOverflowFalseProofAndWrongTickInputsNeverPartiallyMutate()
        {
            Assert.Throws<ArgumentException>(() => new PostBossDeathInput(null, Tick(0), null, 0, true, 5, false, true, null,
                new PostBossPair(PostBossChoice.None, PostBossSkill.None)));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(null, Tick(0), Tick(1), PostBossCleanupDisposition.Removed, null, true, true, true, true, true));
            var ingress = new PostBossProgressionSession();
            Assert.Throws<ArgumentException>(() => ingress.CompleteCleanup(default(PostBossCleanupInput)));
            var pending = new PostBossProgressionSession();
            pending.ObserveDeath(Death(10));
            Assert.Throws<ArgumentException>(() => pending.CompleteCleanup(default(PostBossCleanupInput)));
            Assert.That(pending.Snapshot.Phase, Is.EqualTo(PostBossPhase.AwaitingCleanup));
            Assert.That(pending.Snapshot.RewardRequestPending, Is.False);
            Assert.That(pending.Snapshot.BossSegmentCompleted, Is.False);
            Assert.That(pending.CompleteCleanup(Cleanup(10)).Intents.Count, Is.EqualTo(3));
            var preDeath = new PostBossProgressionSession();
            Assert.Throws<InvalidOperationException>(() => preDeath.CompleteCleanup(Cleanup(10)));
            var wrongTickHandoffs = Handoffs(10);
            wrongTickHandoffs[2] = new PostBossHandoff(PostBossHandoffKind.RoomCompletionRequest, Tick(9));
            Assert.Throws<ArgumentException>(() => new PostBossDeathInput(Key, Tick(10), wrongTickHandoffs, 0, true, 5, false, true, null,
                new PostBossPair(PostBossChoice.None, PostBossSkill.None)));
            var wrongOrderHandoffs = Handoffs(10);
            wrongOrderHandoffs[0] = new PostBossHandoff(PostBossHandoffKind.RewardRequest, Tick(10));
            Assert.Throws<ArgumentException>(() => new PostBossDeathInput(Key, Tick(10), wrongOrderHandoffs, 0, true, 5, false, true, null,
                new PostBossPair(PostBossChoice.None, PostBossSkill.None)));
            Assert.Throws<ArgumentException>(() => Death(int.MaxValue));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(Key, Tick(-1), Tick(0), PostBossCleanupDisposition.Removed, null, true, true, true, true, true));
            Assert.Throws<OverflowException>(() => new PostBossCleanupInput(Key, Tick(int.MaxValue), Tick(int.MaxValue),
                PostBossCleanupDisposition.Removed, null, true, true, true, true, true));
            Assert.Throws<ArgumentException>(() => Cleanup(10, false));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(Key, Tick(10), Tick(11), PostBossCleanupDisposition.Removed, null, false, true, true, true, true));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(Key, Tick(10), Tick(11), PostBossCleanupDisposition.Removed, null, true, true, false, true, true));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(Key, Tick(10), Tick(11), PostBossCleanupDisposition.Removed, null, true, true, true, false, true));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(Key, Tick(10), Tick(11), PostBossCleanupDisposition.Removed, null, true, true, true, true, false));

            var session = new PostBossProgressionSession();
            session.ObserveDeath(Death(10));
            Assert.Throws<ArgumentException>(() => new PostBossCleanupInput(Key, Tick(10), Tick(12), PostBossCleanupDisposition.Removed, null, true, true, true, true, true));
            Assert.That(session.Snapshot.Phase, Is.EqualTo(PostBossPhase.AwaitingCleanup));
            var valid = session.CompleteCleanup(Cleanup(10));
            Assert.That(valid.Intents.Count, Is.EqualTo(3));
        }

        [Test]
        public void AC007_InputsAndResultsExposeDefensiveImmutableValues()
        {
            var sourceHandoffs = Handoffs();
            var session = new PostBossProgressionSession();
            var result = session.ObserveDeath(Death(10, 5, true, null, default(PostBossPair), sourceHandoffs));
            sourceHandoffs[1] = new PostBossHandoff(PostBossHandoffKind.Defeated, Tick(10));
            Assert.That(result.Snapshot.Death.Value.Handoffs[1].Kind, Is.EqualTo(PostBossHandoffKind.RewardRequest));

            var list = result.Intents as IList<PostBossIntent>;
            Assert.That(list, Is.Not.Null);
            Assert.Throws<NotSupportedException>(() => list[0] = default(PostBossIntent));
            Assert.That(result.Intents.Count, Is.EqualTo(1));
        }

        [Test]
        public void AC008_RunAssemblyHasNoUnityCombatOrTransferDependency()
        {
            var names = typeof(PostBossProgressionSession).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToArray();
            Assert.That(names.Any(x => x == "UnityEngine" || x.StartsWith("UnityEngine.")), Is.False);
            Assert.That(names.Any(x => x.IndexOf("Combat", StringComparison.OrdinalIgnoreCase) >= 0), Is.False);
            Assert.That(names.Any(x => x.IndexOf("Transfer", StringComparison.OrdinalIgnoreCase) >= 0), Is.False);
        }

        [Test]
        public void AC009_SyntheticThirtySixtyAndOneFortyFourCadencesHaveExactTrace()
        {
            var traces = new[] { 30, 60, 144 }.Select(RunTrace).ToArray();
            Assert.That(traces[1], Is.EqualTo(traces[0]));
            Assert.That(traces[2], Is.EqualTo(traces[0]));
            var firstDivergence = traces[1] == traces[0] && traces[2] == traces[0] ? "none" : "present";
            TestContext.WriteLine("AC-M5A-009 cadence=30,60,144 firstDivergence=" + firstDivergence +
                " tracesEqual=" + (traces[0] == traces[1] && traces[1] == traces[2]));
            TestContext.WriteLine("AC-M5A-009 finalTrace=" + traces[0]);
            StringAssert.Contains("TransitionRequested@10", traces[0]);
            StringAssert.Contains("RewardRequestPending@10/11", traces[0]);
            StringAssert.Contains("BossSegmentCompleted@10/11", traces[0]);
            StringAssert.Contains("PrepareChoice@10/11", traces[0]);
        }

        private static string RunTrace(int cadence)
        {
            var session = new PostBossProgressionSession();
            var trace = new List<string>();
            var fixedTick = 0;
            var renderRemainder = 0;
            PostBossResult? deathResult = null;
            PostBossResult? cleanupResult = null;
            PostBossResult? duplicateResult = null;
            for (var frame = 0; fixedTick <= 12 && frame < 200; frame++)
            {
                renderRemainder += 60;
                var fixedTicksThisFrame = renderRemainder / cadence;
                renderRemainder %= cadence;
                for (var i = 0; i < fixedTicksThisFrame && fixedTick <= 12; i++, fixedTick++)
                {
                    if (fixedTick == 10)
                    {
                        deathResult = session.ObserveDeath(Death(fixedTick));
                        trace.Add(Format(deathResult.Value));
                    }
                    else if (fixedTick == 11)
                    {
                        cleanupResult = session.CompleteCleanup(Cleanup(fixedTick - 1));
                        trace.AddRange(cleanupResult.Value.Intents.Select(Format));
                        duplicateResult = session.ObserveDeath(Death(10));
                        trace.Add("Duplicate=" + duplicateResult.Value.Disposition);
                    }
                }
            }
            Assert.That(deathResult.HasValue, Is.True);
            Assert.That(cleanupResult.HasValue, Is.True);
            Assert.That(duplicateResult.HasValue, Is.True);
            trace.Add("Phase=" + session.Snapshot.Phase);
            trace.Add("RewardPending=" + session.Snapshot.RewardRequestPending);
            trace.Add("SegmentCompleted=" + session.Snapshot.BossSegmentCompleted);
            trace.Add("FixedTick=" + fixedTick);
            return string.Join("|", trace);
        }

        private static string Format(PostBossIntent intent) => intent.Kind + "@" + intent.DeathTick.Value + "/" +
            (intent.CleanupTick.HasValue ? intent.CleanupTick.Value.Value.ToString() : "null");
        private static string Format(PostBossResult result) => result.Intents.Count == 0 ? result.Disposition.ToString() : Format(result.Intents[0]);

        private static PostBossDeathInput Death(int tick, int playerHealth = 5, bool runActive = true,
            PostBossFailure? failure = null, PostBossPair? pair = null, PostBossHandoff[] handoffs = null,
            bool? explicitPlayerDead = null)
        {
            var selectedPair = pair ?? new PostBossPair(PostBossChoice.None, PostBossSkill.None);
            var dead = explicitPlayerDead ?? (playerHealth == 0);
            return new PostBossDeathInput(Key, Tick(tick), handoffs ?? Handoffs(tick), 0, true, playerHealth, dead, runActive, failure, selectedPair);
        }

        private static PostBossCleanupInput Cleanup(int deathTick, bool durable = true) =>
            new PostBossCleanupInput(Key, Tick(deathTick), Tick(deathTick + 1), PostBossCleanupDisposition.Removed, null,
                true, durable, true, true, true);

        private static PostBossCleanupInput LifecycleCleanup(int deathTick, PostBossLifecycleReason reason, bool durable = false) =>
            new PostBossCleanupInput(Key, Tick(deathTick), Tick(deathTick + 1), PostBossCleanupDisposition.LifecycleSuperseded, reason,
                true, durable, true, true, true);

        private static PostBossHandoff[] Handoffs(int tick = 10) => new[]
        {
            new PostBossHandoff(PostBossHandoffKind.Defeated, Tick(tick)),
            new PostBossHandoff(PostBossHandoffKind.RewardRequest, Tick(tick)),
            new PostBossHandoff(PostBossHandoffKind.RoomCompletionRequest, Tick(tick))
        };

        private static void AssertIntent(PostBossIntent intent, PostBossIntentKind kind, int death, int? cleanup, PostBossPair pair)
        {
            Assert.That(intent.Kind, Is.EqualTo(kind));
            Assert.That(intent.Key, Is.EqualTo(Key));
            Assert.That(intent.DeathTick.Value, Is.EqualTo(death));
            Assert.That(intent.CleanupTick.HasValue, Is.EqualTo(cleanup.HasValue));
            if (cleanup.HasValue) Assert.That(intent.CleanupTick.Value.Value, Is.EqualTo(cleanup.Value));
            Assert.That(intent.Pair, Is.EqualTo(pair));
        }

        private static SimulationTick Tick(int value) => new SimulationTick(value);
    }
}
