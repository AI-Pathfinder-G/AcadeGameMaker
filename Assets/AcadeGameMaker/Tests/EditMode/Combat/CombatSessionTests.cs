using System;
using System.Collections.Generic;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Core;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Combat
{
    public sealed class CombatSessionTests
    {
        // REQ-COM-001, REQ-COM-004; partial AC-COM-003: public ABI rejects malformed inputs before session state exists.
        [Test]
        public void PublicPayloadConstructorsRejectMalformedFieldsAndUnknownEnums()
        {
            Assert.Throws<ArgumentException>(() => Request(string.Empty, "source", "target", 1, DamageKind.BasicAttack, 0));
            Assert.Throws<ArgumentException>(() => Request(new string('a', 97), "source", "target", 1, DamageKind.BasicAttack, 0));
            Assert.Throws<ArgumentException>(() => Request("bad space", "source", "target", 1, DamageKind.BasicAttack, 0));
            Assert.Throws<ArgumentException>(() => Request("request", "bad source", "target", 1, DamageKind.BasicAttack, 0));
            Assert.Throws<ArgumentException>(() => Request("request", "source", "bad target", 1, DamageKind.BasicAttack, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Request("request", "source", "target", 0, DamageKind.BasicAttack, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Request("request", "source", "target", 1000, DamageKind.BasicAttack, 0));
            foreach (DamageKind kind in Enum.GetValues(typeof(DamageKind))) Assert.DoesNotThrow(() => Request("request", "source", "target", 1, kind, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Request("request", "source", "target", 1, (DamageKind)(-1), 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Request("request", "source", "target", 1, (DamageKind)7, 0));
            Assert.Throws<ArgumentException>(() => new DamageResult("bad request", "target", 0, DamageResultCode.Duplicate, new SimulationTick(0)));
            Assert.Throws<ArgumentException>(() => new DamageResult("request", "bad target", 0, DamageResultCode.Duplicate, new SimulationTick(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DamageResult("request", "target", -1, DamageResultCode.Duplicate, new SimulationTick(0)));
            Assert.Throws<ArgumentException>(() => new DamageResult("request", "target", 0, DamageResultCode.Applied, new SimulationTick(0)));
            Assert.Throws<ArgumentException>(() => new DamageResult("request", "target", 1, DamageResultCode.Duplicate, new SimulationTick(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DamageResult("request", "target", 1000, DamageResultCode.Applied, new SimulationTick(0)));
            foreach (DamageResultCode code in Enum.GetValues(typeof(DamageResultCode))) Assert.DoesNotThrow(() => new DamageResult("request", "target", code == DamageResultCode.Applied ? 1 : 0, code, new SimulationTick(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DamageResult("request", "target", 0, (DamageResultCode)(-1), new SimulationTick(0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DamageResult("request", "target", 0, (DamageResultCode)5, new SimulationTick(0)));
        }

        // REQ-COM-004; partial AC-COM-003: results preserve the originating request occurrence exactly.
        [Test]
        public void DamageResultRetainsItsOwnRequestTargetAndTick()
        {
            var result = new DamageResult("request:7", "target/7", 2, DamageResultCode.Applied, Tick(19));
            Assert.That(result.RequestId, Is.EqualTo("request:7"));
            Assert.That(result.TargetId, Is.EqualTo("target/7"));
            Assert.That(result.AppliedAmount, Is.EqualTo(2));
            Assert.That(result.Result, Is.EqualTo(DamageResultCode.Applied));
            Assert.That(result.Tick.Value, Is.EqualTo(19));
        }

        // REQ-COM-004; partial AC-COM-003: authored health registrations are bounded and never discovery ordered.
        [Test]
        public void RegistrationValidationAndDuplicateProtectionAreExact()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CombatSession(new SimulationTick(-1)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CombatSession(new SimulationTick(int.MaxValue)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CombatantRegistration("walker", 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CombatantRegistration("walker", 1000, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CombatantRegistration("walker", 1, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new CombatantRegistration("walker", 1, 601));
            Assert.Throws<ArgumentException>(() => new CombatantRegistration(string.Empty, 1, 0));
            Assert.Throws<ArgumentException>(() => new CombatantRegistration("bad target", 1, 0));

            var session = Session(0, Walker());
            Assert.Throws<ArgumentException>(() => session.Register(Walker()));
            Assert.That(session.Snapshot("walker").CurrentHealth, Is.EqualTo(9));
        }

        // REQ-COM-005, REQ-COM-006; partial AC-COM-003: vertical-demo health and immunity registration constants are explicit.
        [Test]
        public void VerticalDemoCombatantRegistrationValuesArePinned()
        {
            var session = Session(0, Player(), Walker(), Surveyor(), Ordan());
            Assert.That(session.Snapshot("player").CurrentHealth, Is.EqualTo(5));
            Assert.That(session.Snapshot("player").InvulnerabilityEndTick, Is.EqualTo(-1));
            Assert.That(session.Snapshot("walker").CurrentHealth, Is.EqualTo(9));
            Assert.That(session.Snapshot("surveyor").CurrentHealth, Is.EqualTo(6));
            Assert.That(session.Snapshot("ordan").CurrentHealth, Is.EqualTo(60));
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-003: same-tick results use the frozen ordinal tuple, not registration or submission order.
        [Test]
        public void CanonicalSameTickOrderingAndDedupeIgnoreRegistrationAndSubmissionOrder()
        {
            var first = Session(0, Walker(), Player());
            var second = Session(0, Player(), Walker());
            var submitted = new[]
            {
                Request("b", "src", "walker", 2, DamageKind.BasicAttack, 0),
                Request("a", "z", "walker", 1, DamageKind.BasicAttack, 0),
                Request("a", "a", "missing", 1, DamageKind.BossPattern, 0),
                Request("c", "src", "walker", 1, DamageKind.HeavyImpact, 0)
            };
            var reversed = new[] { submitted[3], submitted[2], submitted[1], submitted[0] };

            var firstTrace = Results(first.Process(Tick(0), submitted));
            var secondTrace = Results(second.Process(Tick(0), reversed));

            Assert.That(firstTrace, Is.EqualTo(secondTrace));
            Assert.That(firstTrace, Is.EqualTo("a:missing:0:Invalid|a:walker:0:Duplicate|b:walker:2:Applied|c:walker:1:Applied"));
            Assert.That(first.Snapshot("walker").CurrentHealth, Is.EqualTo(6));
            Assert.That(second.Snapshot("walker").CurrentHealth, Is.EqualTo(6));
        }

        // REQ-COM-004; partial AC-COM-003: Invalid consumes an ID and duplicate precedence beats later target lookup.
        [Test]
        public void UnknownTargetIsInvalidAndConsumesTheRequestIdForTheEncounter()
        {
            var session = Session(0, Walker());
            var results = session.Process(Tick(0), new[]
            {
                Request("same", "a", "missing", 1, DamageKind.BasicAttack, 0),
                Request("same", "z", "walker", 1, DamageKind.BasicAttack, 0)
            });

            Assert.That(Results(results), Is.EqualTo("same:missing:0:Invalid|same:walker:0:Duplicate"));
            Assert.That(session.Snapshot("walker").CurrentHealth, Is.EqualTo(9));
        }

        // REQ-COM-004; partial AC-COM-003: dedupe survives ticks and is cleared only by an explicit encounter reset.
        [Test]
        public void CrossTickDuplicateAndEncounterResetHaveTheFrozenLifetime()
        {
            var session = Session(0, Walker());
            Assert.That(Results(session.Process(Tick(0), new[] { Request("once", "player", "walker", 2, DamageKind.BasicAttack, 0) })), Is.EqualTo("once:walker:2:Applied"));
            Assert.That(Results(session.Process(Tick(1), new[] { Request("once", "player", "walker", 2, DamageKind.BasicAttack, 1) })), Is.EqualTo("once:walker:0:Duplicate"));

            session.ResetEncounter();
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(2));
            session.Register(Walker());
            Assert.That(Results(session.Process(Tick(2), new[] { Request("once", "player", "walker", 2, DamageKind.BasicAttack, 2) })), Is.EqualTo("once:walker:2:Applied"));
        }

        // REQ-COM-004; partial AC-COM-003: player damage immunity accepts a fresh hit at the exact age-45 boundary.
        [Test]
        public void PlayerInvulnerabilityRejectsAge44AndAcceptsAge45()
        {
            var session = Session(0, Player());
            Assert.That(Results(session.Process(Tick(0), new[] { Request("hit-0", "enemy", "player", 1, DamageKind.EnemyContact, 0) })), Is.EqualTo("hit-0:player:1:Applied"));
            AdvanceEmpty(session, 1, 43);
            Assert.That(Results(session.Process(Tick(44), new[] { Request("hit-44", "enemy", "player", 1, DamageKind.EnemyContact, 44) })), Is.EqualTo("hit-44:player:0:Invulnerable"));
            Assert.That(Results(session.Process(Tick(45), new[] { Request("hit-45", "enemy", "player", 1, DamageKind.EnemyContact, 45) })), Is.EqualTo("hit-45:player:1:Applied"));
            Assert.That(session.Snapshot("player").CurrentHealth, Is.EqualTo(3));
        }

        // REQ-COM-004, REQ-COM-005, REQ-COM-006; partial AC-COM-003: zero-immunity combatants accept canonical multi-hit until one killing request.
        [Test]
        public void ZeroInvulnerabilityAllowsMultiHitThenClampsOverkillAndDiesOnce()
        {
            var session = Session(0, Walker());
            var results = session.Process(Tick(0), new[]
            {
                Request("c", "player", "walker", 999, DamageKind.BasicAttack, 0),
                Request("a", "player", "walker", 4, DamageKind.BasicAttack, 0),
                Request("b", "player", "walker", 3, DamageKind.BasicAttack, 0),
                Request("d", "player", "walker", 1, DamageKind.BasicAttack, 0)
            });

            Assert.That(Results(results), Is.EqualTo("a:walker:4:Applied|b:walker:3:Applied|c:walker:2:Applied|d:walker:0:TargetDead"));
            var snapshot = session.Snapshot("walker");
            Assert.That(snapshot.CurrentHealth, Is.EqualTo(0));
            Assert.That(snapshot.IsDead, Is.True);
            Assert.That(snapshot.AcceptedRequestCount, Is.EqualTo(3));
            Assert.That(snapshot.InvulnerabilityEndTick, Is.EqualTo(-1));

            Assert.That(Results(session.Process(Tick(1), new[] { Request("later", "player", "walker", 1, DamageKind.BasicAttack, 1) })), Is.EqualTo("later:walker:0:TargetDead"));
            Assert.That(session.Snapshot("walker").AcceptedRequestCount, Is.EqualTo(3));
        }

        // REQ-COM-004; partial AC-COM-003: malformed batch clocks fail before health, tombstones, or next-tick state mutate.
        [Test]
        public void WrongSkippedRepeatedAndNegativeTicksAreAtomic()
        {
            var session = Session(0, Walker());
            Assert.Throws<InvalidOperationException>(() => session.Process(Tick(-1), Array.Empty<DamageRequest>()));
            Assert.Throws<InvalidOperationException>(() => session.Process(Tick(1), Array.Empty<DamageRequest>()));
            Assert.Throws<ArgumentException>(() => session.Process(Tick(0), new[] { Request("wrong", "p", "walker", 1, DamageKind.BasicAttack, 1) }));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(0));
            Assert.That(session.Snapshot("walker").CurrentHealth, Is.EqualTo(9));

            session.Process(Tick(0), Array.Empty<DamageRequest>());
            Assert.Throws<InvalidOperationException>(() => session.Process(Tick(0), Array.Empty<DamageRequest>()));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(1));
            Assert.That(session.Snapshot("walker").CurrentHealth, Is.EqualTo(9));
        }

        // REQ-COM-004; partial AC-COM-003: checked arithmetic rejects invulnerability and global tick overflow before mutation.
        [Test]
        public void OverflowBoundariesAreAtomicAndTheMaxValueSentinelCannotCommit()
        {
            var invulnerabilityOverflow = Session(int.MaxValue - 1, Player());
            Assert.Throws<OverflowException>(() => invulnerabilityOverflow.Process(Tick(int.MaxValue - 1), new[] { Request("overflow", "enemy", "player", 1, DamageKind.EnemyContact, int.MaxValue - 1) }));
            Assert.That(invulnerabilityOverflow.NextExpectedTick.Value, Is.EqualTo(int.MaxValue - 1));
            Assert.That(invulnerabilityOverflow.Snapshot("player").CurrentHealth, Is.EqualTo(5));

            var sentinel = Session(int.MaxValue - 1, Walker());
            sentinel.Process(Tick(int.MaxValue - 1), Array.Empty<DamageRequest>());
            Assert.That(sentinel.NextExpectedTick.Value, Is.EqualTo(int.MaxValue));
            Assert.Throws<OverflowException>(() => sentinel.Process(Tick(int.MaxValue), new[] { Request("never", "p", "walker", 1, DamageKind.BasicAttack, int.MaxValue) }));
            Assert.That(sentinel.NextExpectedTick.Value, Is.EqualTo(int.MaxValue));
            Assert.That(sentinel.Snapshot("walker").CurrentHealth, Is.EqualTo(9));
        }

        // REQ-COM-004; partial AC-COM-003: reset is a post-tick boundary and keeps the global clock unchanged.
        [Test]
        public void ResetRequiresCompletedTickAndRetainsTheExactFollowingGlobalTick()
        {
            var session = Session(0, Walker());
            Assert.Throws<InvalidOperationException>(() => session.ResetEncounter());
            session.Process(Tick(0), Array.Empty<DamageRequest>());
            session.ResetEncounter();
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(1));
            Assert.Throws<ArgumentException>(() => session.Snapshot("walker"));
            Assert.Throws<InvalidOperationException>(() => session.Process(Tick(1), Array.Empty<DamageRequest>()));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(1));
            session.Register(Walker());
            Assert.That(Results(session.Process(Tick(1), new[] { Request("after-reset", "p", "walker", 1, DamageKind.BasicAttack, 1) })), Is.EqualTo("after-reset:walker:1:Applied"));
        }

        // REQ-COM-001, REQ-COM-004; partial AC-COM-003: render grouping cannot alter the same 60 Hz request trace.
        [Test]
        public void SameSixtyHertzScriptIsIdenticalAtThirtySixtyAndOneFortyFourRenderGrouping()
        {
            var atThirty = Replay(30);
            var atSixty = Replay(60);
            var atOneFortyFour = Replay(144);

            Assert.That(atThirty, Is.EqualTo(atSixty));
            Assert.That(atSixty, Is.EqualTo(atOneFortyFour));
        }

        private static string Replay(int renderFramesPerSecond)
        {
            var session = Session(0, Player(), Walker());
            var trace = new List<string>();
            var accumulator = 0;
            var tick = 0;
            while (tick < 60)
            {
                accumulator += 60;
                while (accumulator >= renderFramesPerSecond && tick < 60)
                {
                    accumulator -= renderFramesPerSecond;
                    IReadOnlyList<DamageRequest> requests = RequestsForTick(tick);
                    var results = session.Process(Tick(tick), requests);
                    var player = session.Snapshot("player");
                    var walker = session.Snapshot("walker");
                    trace.Add(tick + "=" + Results(results) + "/p" + player.CurrentHealth + ":" + player.IsDead + ":" + player.InvulnerabilityEndTick + ":" + player.AcceptedRequestCount + "/w" + walker.CurrentHealth + ":" + walker.IsDead + ":" + walker.InvulnerabilityEndTick + ":" + walker.AcceptedRequestCount + "/next" + session.NextExpectedTick.Value);
                    tick++;
                }
            }

            return string.Join(";", trace);
        }

        private static IReadOnlyList<DamageRequest> RequestsForTick(int tick)
        {
            if (tick == 0) return new[] { Request("p0", "enemy", "player", 1, DamageKind.EnemyContact, tick) };
            if (tick == 1) return new[] { Request("p1", "enemy", "player", 1, DamageKind.EnemyContact, tick), Request("w1", "player", "walker", 2, DamageKind.BasicAttack, tick) };
            if (tick == 45) return new[] { Request("p45", "enemy", "player", 1, DamageKind.EnemyContact, tick), Request("w45", "player", "walker", 7, DamageKind.BasicAttack, tick) };
            return Array.Empty<DamageRequest>();
        }

        private static CombatSession Session(int firstTick, params CombatantRegistration[] registrations)
        {
            var session = new CombatSession(Tick(firstTick));
            foreach (var registration in registrations) session.Register(registration);
            return session;
        }

        private static CombatantRegistration Player() => new CombatantRegistration("player", 5, 45);
        private static CombatantRegistration Walker() => new CombatantRegistration("walker", 9, 0);
        private static CombatantRegistration Surveyor() => new CombatantRegistration("surveyor", 6, 0);
        private static CombatantRegistration Ordan() => new CombatantRegistration("ordan", 60, 0);
        private static SimulationTick Tick(int value) => new SimulationTick(value);
        private static DamageRequest Request(string requestId, string sourceId, string targetId, int amount, DamageKind kind, int tick) => new DamageRequest(requestId, sourceId, targetId, amount, kind, Tick(tick));

        private static void AdvanceEmpty(CombatSession session, int firstTick, int lastTick)
        {
            for (var tick = firstTick; tick <= lastTick; tick++) session.Process(Tick(tick), Array.Empty<DamageRequest>());
        }

        private static string Results(IReadOnlyList<DamageResult> results)
        {
            var values = new string[results.Count];
            for (var index = 0; index < results.Count; index++) values[index] = results[index].RequestId + ":" + results[index].TargetId + ":" + results[index].AppliedAmount + ":" + results[index].Result;
            return string.Join("|", values);
        }
    }
}
