using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AcadeGameMaker.Core;
using AcadeGameMaker.Run;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Run
{
    // REQ-M5D2-001..006; AC-M5D2-001..008. Engine-free failure-prefix matrix.
    public sealed class RunFailureArbitrationSessionTests
    {
        private const string Key = "run/ordan-01";

        [Test]
        public void AC001_StartBoundariesDuplicatesConflictsAndIndependentInstancesAreExact()
        {
            AssertStart(new SimulationTick(0));
            AssertStart(new SimulationTick(int.MaxValue - 1));

            var session = new RunFailureArbitrationSession();
            var start = new RunStartInput(Key, new SimulationTick(0));
            var first = session.Start(start);
            var duplicate = session.Start(start);
            Assert.That(first.Disposition, Is.EqualTo(RunFailureDisposition.Applied));
            Assert.That(duplicate.Disposition, Is.EqualTo(RunFailureDisposition.Duplicate));
            Assert.That(duplicate.Snapshot.Revision, Is.EqualTo(1));
            Assert.That(duplicate.NewlyEmittedIntents.Count, Is.Zero);
            Assert.Throws<InvalidOperationException>(() => session.Start(new RunStartInput(Key, new SimulationTick(1))));
            AssertSnapshot(session.Snapshot, RunFailurePhase.Active, 0, 0, null, 1);

            Assert.Throws<ArgumentException>(() => new RunStartInput(Key, new SimulationTick(-1)));
            Assert.Throws<ArgumentException>(() => new RunStartInput(Key, new SimulationTick(int.MaxValue)));
            Assert.Throws<ArgumentException>(() => new RunStartInput("bad key", new SimulationTick(0)));
            Assert.Throws<ArgumentException>(() => new RunStartInput(new string('a', 97), new SimulationTick(0)));

            var left = Started(4, Key);
            var right = Started(4, Key);
            Assert.That(left.ObserveTick(Tick(Key, 4, RunFailureCauseMask.None)).Snapshot.Revision, Is.EqualTo(2));
            Assert.That(right.Snapshot.Revision, Is.EqualTo(1));
        }

        [Test]
        public void AC002_SequentialNoFailureTicksReplaysAndChangedReplaysRetainFingerprints()
        {
            var session = Started(10);
            var firstInput = Tick(Key, 10, RunFailureCauseMask.None);
            var first = session.ObserveTick(firstInput);
            Assert.That(first.Disposition, Is.EqualTo(RunFailureDisposition.Applied));
            Assert.That(first.NewlyEmittedIntents.Count, Is.Zero);
            AssertSnapshot(first.Snapshot, RunFailurePhase.Active, 10, 11, null, 2);

            var duplicate = session.ObserveTick(firstInput);
            Assert.That(duplicate.Disposition, Is.EqualTo(RunFailureDisposition.Duplicate));
            Assert.That(duplicate.NewlyEmittedIntents.Count, Is.Zero);
            AssertSnapshot(session.Snapshot, RunFailurePhase.Active, 10, 11, null, 2);

            Assert.Throws<InvalidOperationException>(() => session.ObserveTick(Tick(Key, 10, RunFailureCauseMask.HealthDepleted)));
            Assert.Throws<InvalidOperationException>(() => session.ObserveTick(Tick(Key, 9, RunFailureCauseMask.None)));
            Assert.Throws<InvalidOperationException>(() => session.ObserveTick(Tick(Key, 12, RunFailureCauseMask.None)));
            AssertSnapshot(session.Snapshot, RunFailurePhase.Active, 10, 11, null, 2);

            var second = session.ObserveTick(Tick(Key, 11, RunFailureCauseMask.None));
            AssertSnapshot(second.Snapshot, RunFailurePhase.Active, 10, 12, null, 3);
        }

        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(4, 2)]
        public void AC003_EachSingleCausePublishesOneCompleteImmutableFailureEnvelope(int triggeredValue, int expectedValue)
        {
            var triggered = (RunFailureCauseMask)triggeredValue;
            var expected = (RunFailureCause)expectedValue;
            var result = Started(7).ObserveTick(Tick(Key, 7, triggered));
            Assert.That(result.Disposition, Is.EqualTo(RunFailureDisposition.Applied));
            Assert.That(result.Snapshot.Phase, Is.EqualTo(RunFailurePhase.Failed));
            Assert.That(result.Snapshot.RunActive, Is.False);
            Assert.That(result.Snapshot.NextExpectedTick.HasValue, Is.False);
            Assert.That(result.Snapshot.Revision, Is.EqualTo(2));
            Assert.That(result.Snapshot.AcceptedFailure.HasValue, Is.True);
            AssertFailure(result.Snapshot.AcceptedFailure.Value, expected, 7, triggered, new[] { expected });
            Assert.That(result.NewlyEmittedIntents.Count, Is.EqualTo(1));
            AssertIntent(result.NewlyEmittedIntents[0], result.Snapshot, expected, 7, triggered, new[] { expected });
            Assert.Throws<NotSupportedException>(() => ((IList<RunFailureIntent>)result.NewlyEmittedIntents)[0] = default(RunFailureIntent));
            Assert.Throws<NotSupportedException>(() => ((IList<RunFailureCause>)result.Snapshot.AcceptedFailure.Value.ObservedCauses)[0] = RunFailureCause.KillPlane);
        }

        [Test]
        public void AC004_AllMultiCauseMasksUseCanonicalOrderingAndFirstCause()
        {
            var cases = new Dictionary<RunFailureCauseMask, RunFailureCause[]>
            {
                { RunFailureCauseMask.HealthDepleted | RunFailureCauseMask.KillPlane,
                    new[] { RunFailureCause.HealthDepleted, RunFailureCause.KillPlane } },
                { RunFailureCauseMask.HealthDepleted | RunFailureCauseMask.LethalCrush,
                    new[] { RunFailureCause.HealthDepleted, RunFailureCause.LethalCrush } },
                { RunFailureCauseMask.KillPlane | RunFailureCauseMask.LethalCrush,
                    new[] { RunFailureCause.KillPlane, RunFailureCause.LethalCrush } },
                { RunFailureCauseMask.AllKnown,
                    new[] { RunFailureCause.HealthDepleted, RunFailureCause.KillPlane, RunFailureCause.LethalCrush } }
            };
            foreach (var pair in cases)
            {
                var result = Started(3).ObserveTick(Tick(Key, 3, pair.Key));
                var failure = result.Snapshot.AcceptedFailure.Value;
                Assert.That(failure.Cause, Is.EqualTo(pair.Value[0]));
                Assert.That(failure.ObservedCauses, Is.EqualTo(pair.Value));
                Assert.That(result.NewlyEmittedIntents.Count, Is.EqualTo(1));
                AssertIntent(result.NewlyEmittedIntents[0], result.Snapshot, pair.Value[0], 3, pair.Key, pair.Value);
            }
        }

        [Test]
        public void AC005_TerminalReplaysAreDuplicatesAndAllOtherPostFailureInputsAreAtomicErrors()
        {
            var session = Started(2);
            var terminal = Tick(Key, 2, RunFailureCauseMask.KillPlane | RunFailureCauseMask.LethalCrush);
            var first = session.ObserveTick(terminal);
            var before = session.Snapshot;
            var duplicate = session.ObserveTick(terminal);
            Assert.That(duplicate.Disposition, Is.EqualTo(RunFailureDisposition.Duplicate));
            Assert.That(duplicate.NewlyEmittedIntents.Count, Is.Zero);
            Assert.That(duplicate.Snapshot.Revision, Is.EqualTo(before.Revision));
            Assert.That(duplicate.Snapshot.AcceptedFailure.Value.Cause, Is.EqualTo(before.AcceptedFailure.Value.Cause));

            Assert.Throws<InvalidOperationException>(() => session.ObserveTick(Tick(Key, 2, RunFailureCauseMask.KillPlane)));
            Assert.Throws<InvalidOperationException>(() => session.ObserveTick(Tick(Key, 3, RunFailureCauseMask.None)));
            Assert.Throws<InvalidOperationException>(() => session.ObserveTick(Tick("run/other", 2, RunFailureCauseMask.KillPlane | RunFailureCauseMask.LethalCrush)));
            Assert.That(session.Snapshot.Phase, Is.EqualTo(RunFailurePhase.Failed));
            Assert.That(session.Snapshot.Revision, Is.EqualTo(before.Revision));
            Assert.That(session.Snapshot.AcceptedFailure.Value.ObservedCauses, Is.EqualTo(first.Snapshot.AcceptedFailure.Value.ObservedCauses));
        }

        [Test]
        public void AC006_DefaultPartialUnknownAndTickBoundaryInputsAreRejectedWithoutPartialPublication()
        {
            var session = Started(0);
            var initial = session.Snapshot;
            Assert.Throws<ArgumentException>(() => session.ObserveTick(default(RunFailureTickInput)));
            foreach (var partial in new[]
            {
                RunFailureCauseMask.HealthDepleted, RunFailureCauseMask.KillPlane, RunFailureCauseMask.LethalCrush,
                RunFailureCauseMask.HealthDepleted | RunFailureCauseMask.KillPlane,
                RunFailureCauseMask.HealthDepleted | RunFailureCauseMask.LethalCrush,
                RunFailureCauseMask.KillPlane | RunFailureCauseMask.LethalCrush
            })
                Assert.Throws<ArgumentException>(() => session.ObserveTick(BypassTickInput(Key, 0, partial, RunFailureCauseMask.None)));
            Assert.Throws<ArgumentException>(() => session.ObserveTick(BypassTickInput(Key, 0, (RunFailureCauseMask)15, RunFailureCauseMask.None)));
            Assert.Throws<ArgumentException>(() => session.ObserveTick(BypassTickInput(Key, 0, RunFailureCauseMask.AllKnown, (RunFailureCauseMask)8)));
            Assert.Throws<ArgumentException>(() => session.ObserveTick(BypassTickInput(Key, -1, RunFailureCauseMask.AllKnown, RunFailureCauseMask.None)));
            Assert.Throws<ArgumentException>(() => session.ObserveTick(BypassTickInput("bad key", 0, RunFailureCauseMask.AllKnown, RunFailureCauseMask.None)));
            Assert.That(session.Snapshot.Revision, Is.EqualTo(initial.Revision));
            Assert.That(session.Snapshot.NextExpectedTick, Is.EqualTo(initial.NextExpectedTick));
            Assert.That(session.ObserveTick(Tick(Key, 0, RunFailureCauseMask.None)).Disposition, Is.EqualTo(RunFailureDisposition.Applied));

            var boundary = Started(int.MaxValue - 1);
            var lastRepresentable = boundary.ObserveTick(Tick(Key, int.MaxValue - 1, RunFailureCauseMask.None));
            AssertSnapshot(lastRepresentable.Snapshot, RunFailurePhase.Active, int.MaxValue - 1, int.MaxValue, null, 2);
            var beforeOverflow = boundary.Snapshot;
            Assert.Throws<OverflowException>(() => boundary.ObserveTick(Tick(Key, int.MaxValue, RunFailureCauseMask.HealthDepleted)));
            Assert.That(boundary.Snapshot.Revision, Is.EqualTo(beforeOverflow.Revision));
            Assert.That(boundary.Snapshot.NextExpectedTick, Is.EqualTo(beforeOverflow.NextExpectedTick));
            Assert.Throws<ArgumentException>(() => new RunStartInput(Key, new SimulationTick(int.MaxValue)));
            Assert.Throws<ArgumentException>(() => new RunStartInput(Key, new SimulationTick(-1)));
            Assert.Throws<ArgumentException>(() => new RunFailureArbitrationSession().Start(BypassStartInput(Key, int.MaxValue)));
        }

        [Test]
        public void AC006_InternalLongRevisionOverflowPreventsPublicationAndRevisionIsNotCallerInput()
        {
            var session = Started(4);
            SetPrivate(session, "_revision", long.MaxValue);
            var before = session.Snapshot;
            Assert.Throws<OverflowException>(() => session.ObserveTick(Tick(Key, 4, RunFailureCauseMask.HealthDepleted)));
            Assert.That(session.Snapshot.Revision, Is.EqualTo(long.MaxValue));
            Assert.That(session.Snapshot.Phase, Is.EqualTo(RunFailurePhase.Active));
            Assert.That(session.Snapshot.NextExpectedTick, Is.EqualTo(before.NextExpectedTick));
            Assert.That(typeof(RunFailureTickInput).GetProperties().Any(x => x.Name.IndexOf("Revision", StringComparison.OrdinalIgnoreCase) >= 0), Is.False);
            Assert.That(typeof(RunStartInput).GetProperties().Any(x => x.Name.IndexOf("Revision", StringComparison.OrdinalIgnoreCase) >= 0), Is.False);
        }

        [Test]
        public void AC007_CoreOnlySourceUsesSeparateCheckedSuccessorsAndNoRuntimeAuthority()
        {
            var assemblyNames = typeof(RunFailureArbitrationSession).Assembly.GetReferencedAssemblies().Select(x => x.Name).ToArray();
            Assert.That(assemblyNames.Any(x => x.StartsWith("UnityEngine", StringComparison.Ordinal)), Is.False);
            Assert.That(assemblyNames.Any(x => x.IndexOf("Combat", StringComparison.OrdinalIgnoreCase) >= 0 ||
                x.IndexOf("Transfer", StringComparison.OrdinalIgnoreCase) >= 0), Is.False);

            var source = File.ReadAllText("Assets/AcadeGameMaker/Runtime/Run/RunFailureArbitrationSession.cs");
            StringAssert.Contains("checked(normalized.Tick.Value + 1)", source);
            StringAssert.Contains("checked(_revision + 1)", source);
            StringAssert.Contains("new RunEndRequested(RunResult.Failed", source);
            foreach (var forbidden in new[] { "UnityEngine", "System.IO", "System.Net", "DateTime", "Random", "SceneManager", "InputRouter", "Profile", "callback" })
                Assert.That(source, Does.Not.Contain(forbidden), "Runtime source must not acquire forbidden authority: " + forbidden);
        }

        [Test]
        public void AC008_ThirtySixtyAndOneFortyFourGroupingTracesAreExact()
        {
            var traces = new[] { 30, 60, 144 }.Select(GroupedTrace).ToArray();
            Assert.That(traces[1], Is.EqualTo(traces[0]));
            Assert.That(traces[2], Is.EqualTo(traces[0]));
            TestContext.WriteLine("AC-M5D2-008 cadence=30,60,144 firstDivergence=none trace=" + traces[0]);
            StringAssert.Contains("Active@0/r2", traces[0]);
            StringAssert.Contains("Failed@2/r4/HealthDepleted", traces[0]);
            StringAssert.Contains("Duplicate@2/r4", traces[0]);
        }

        private static void AssertStart(SimulationTick tick)
        {
            var session = new RunFailureArbitrationSession();
            var result = session.Start(new RunStartInput(Key, tick));
            AssertSnapshot(result.Snapshot, RunFailurePhase.Active, tick.Value, tick.Value, null, 1);
            Assert.That(result.NewlyEmittedIntents.Count, Is.Zero);
        }

        private static RunFailureArbitrationSession Started(int tick, string key = Key)
        {
            var session = new RunFailureArbitrationSession();
            session.Start(new RunStartInput(key, new SimulationTick(tick)));
            return session;
        }

        private static RunFailureTickInput Tick(string key, int tick, RunFailureCauseMask triggered) =>
            new RunFailureTickInput(key, new SimulationTick(tick), RunFailureCauseMask.AllKnown, triggered);

        private static void AssertSnapshot(RunFailureSnapshot snapshot, RunFailurePhase phase, int startTick,
            int? nextTick, AcceptedRunFailure? accepted, long revision)
        {
            Assert.That(snapshot.Phase, Is.EqualTo(phase));
            Assert.That(snapshot.RunActive, Is.EqualTo(phase == RunFailurePhase.Active));
            Assert.That(snapshot.RunInstanceKey, Is.EqualTo(Key));
            Assert.That(snapshot.StartTick.Value.Value, Is.EqualTo(startTick));
            Assert.That(snapshot.NextExpectedTick.HasValue, Is.EqualTo(nextTick.HasValue));
            if (nextTick.HasValue) Assert.That(snapshot.NextExpectedTick.Value.Value, Is.EqualTo(nextTick.Value));
            Assert.That(snapshot.AcceptedFailure.HasValue, Is.EqualTo(accepted.HasValue));
            Assert.That(snapshot.Revision, Is.EqualTo(revision));
        }

        private static void AssertFailure(AcceptedRunFailure failure, RunFailureCause cause, int tick,
            RunFailureCauseMask triggered, IReadOnlyList<RunFailureCause> observed)
        {
            Assert.That(failure.Cause, Is.EqualTo(cause));
            Assert.That(failure.Tick.Value, Is.EqualTo(tick));
            Assert.That(failure.PresenceMask, Is.EqualTo(RunFailureCauseMask.AllKnown));
            Assert.That(failure.TriggeredMask, Is.EqualTo(triggered));
            Assert.That(failure.ObservedCauses, Is.EqualTo(observed));
        }

        private static void AssertIntent(RunFailureIntent intent, RunFailureSnapshot snapshot, RunFailureCause cause,
            int tick, RunFailureCauseMask triggered, IReadOnlyList<RunFailureCause> observed)
        {
            Assert.That(intent.Request.Result, Is.EqualTo(RunResult.Failed));
            Assert.That(intent.Request.Cause, Is.EqualTo(cause));
            Assert.That(intent.Request.Tick.Value, Is.EqualTo(tick));
            Assert.That(intent.RunInstanceKey, Is.EqualTo(snapshot.RunInstanceKey));
            Assert.That(intent.SourceRevision, Is.EqualTo(snapshot.Revision));
            Assert.That(intent.PresenceMask, Is.EqualTo(RunFailureCauseMask.AllKnown));
            Assert.That(intent.TriggeredMask, Is.EqualTo(triggered));
            Assert.That(intent.ObservedCauses, Is.EqualTo(observed));
        }

        private static string GroupedTrace(int cadence)
        {
            var session = Started(0);
            var fixedTick = 0;
            var remainder = 0;
            var trace = new List<string>();
            for (var frame = 0; frame < 300 && session.Snapshot.Phase == RunFailurePhase.Active; frame++)
            {
                remainder += 60;
                var count = remainder / cadence;
                remainder %= cadence;
                for (var index = 0; index < count && session.Snapshot.Phase == RunFailurePhase.Active; index++, fixedTick++)
                {
                    var triggered = fixedTick == 2
                        ? RunFailureCauseMask.HealthDepleted | RunFailureCauseMask.LethalCrush
                        : RunFailureCauseMask.None;
                    var result = session.ObserveTick(Tick(Key, fixedTick, triggered));
                    var cause = result.Snapshot.AcceptedFailure.HasValue ? "/" + result.Snapshot.AcceptedFailure.Value.Cause : string.Empty;
                    trace.Add(result.Snapshot.Phase + "@" + fixedTick + "/r" + result.Snapshot.Revision + cause);
                    if (result.Snapshot.Phase == RunFailurePhase.Failed)
                    {
                        var duplicate = session.ObserveTick(Tick(Key, fixedTick, triggered));
                        trace.Add(duplicate.Disposition + "@" + fixedTick + "/r" + duplicate.Snapshot.Revision);
                    }
                }
            }
            return string.Join("|", trace);
        }

        private static RunFailureTickInput BypassTickInput(string key, int tick, RunFailureCauseMask presence, RunFailureCauseMask triggered)
        {
            var value = default(RunFailureTickInput);
            value = RewriteAutoProperty(value, "<RunInstanceKey>k__BackingField", key);
            value = RewriteAutoProperty(value, "<Tick>k__BackingField", new SimulationTick(tick));
            value = RewriteAutoProperty(value, "<PresenceMask>k__BackingField", presence);
            return RewriteAutoProperty(value, "<TriggeredMask>k__BackingField", triggered);
        }

        private static RunStartInput BypassStartInput(string key, int tick)
        {
            var value = default(RunStartInput);
            value = RewriteAutoProperty(value, "<RunInstanceKey>k__BackingField", key);
            return RewriteAutoProperty(value, "<StartTick>k__BackingField", new SimulationTick(tick));
        }

        private static T RewriteAutoProperty<T>(T value, string field, object replacement) where T : struct
        {
            object boxed = value;
            var backing = typeof(T).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(backing, Is.Not.Null);
            backing.SetValue(boxed, replacement);
            return (T)boxed;
        }

        private static void SetPrivate(object target, string field, object value)
        {
            var targetField = target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(targetField, Is.Not.Null);
            targetField.SetValue(target, value);
        }
    }
}
