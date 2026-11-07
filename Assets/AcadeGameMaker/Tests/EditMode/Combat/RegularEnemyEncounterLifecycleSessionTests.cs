using System;
using System.Collections;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Core;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Combat
{
    public sealed class RegularEnemyEncounterLifecycleSessionTests
    {
        // REQ-COM-001, REQ-COM-002, REQ-COM-004; partial AC-COM-001 and AC-COM-003:
        // every regular-enemy death edge emits once and the second edge clears once.
        [Test]
        public void DefeatRewardAndClearHandoffsAreOneShotAndRoleOrdered()
        {
            var session = new RegularEnemyEncounterLifecycleSession(Tick(0));
            var surveyor = Input(0, "surveyor", 6, States(5, 0, 9));
            var first = session.PreviewNext(surveyor);
            Assert.That(first.AppliedHitTargetIds, Is.EqualTo(new[] { "surveyor" }));
            Assert.That(first.Defeats.Count, Is.EqualTo(1));
            Assert.That(first.Defeats[0].TargetId, Is.EqualTo("surveyor"));
            Assert.That(first.Cleared.HasValue, Is.False);
            session.CommitNext(surveyor, first);

            var walker = Input(1, "walker", 9, States(5, 0, 0));
            var second = session.PreviewNext(walker);
            Assert.That(second.Defeats.Count, Is.EqualTo(1));
            Assert.That(second.Defeats[0].TargetId, Is.EqualTo("walker"));
            Assert.That(second.Cleared.HasValue, Is.True);
            Assert.That(second.Completion.HasValue, Is.True);
            session.CommitNext(walker, second);

            var repeated = session.PreviewNext(Empty(2, States(5, 0, 0)));
            Assert.That(repeated.Defeats, Is.Empty);
            Assert.That(repeated.Rewards, Is.Empty);
            Assert.That(repeated.Cleared.HasValue, Is.False);
            Assert.That(repeated.Completion.HasValue, Is.False);
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003: malformed conservation and
        // post-death reset are rejected before the lifecycle owner changes state.
        [Test]
        public void InvalidDamageAndPostDeathResetPreserveLifecycleState()
        {
            var session = new RegularEnemyEncounterLifecycleSession(Tick(0));
            var death = Input(0, "walker", 9, States(5, 6, 0));
            var kill = session.PreviewNext(death);
            session.CommitNext(death, kill);
            var expected = session.NextExpectedTick;
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Empty(1, States(5, 6, 1))));
            Assert.That(session.NextExpectedTick, Is.EqualTo(expected));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(new RegularEnemyEncounterLifecycleInput(Tick(1),
                RegularEnemyEncounterLifecycleInputKind.EncounterReset, Array.Empty<DamageRequest>(), Array.Empty<DamageResult>(), States(5, 6, 9))));
            Assert.That(session.NextExpectedTick, Is.EqualTo(expected));
        }

        // REQ-COM-001, REQ-COM-002; partial AC-COM-001: simultaneous enemy
        // deaths retain literal surveyor then walker notice order and clear once.
        [Test]
        public void SimultaneousDeathsUseSurveyorWalkerOrderAndPlayerDeathNeverCompletes()
        {
            var session = new RegularEnemyEncounterLifecycleSession(Tick(0));
            var both = Multi(0, new[] { Damage("surveyor", 6, 0), Damage("walker", 9, 0) }, States(5, 0, 0));
            var preview = session.PreviewNext(both);
            Assert.That(preview.Defeats.Count, Is.EqualTo(2));
            Assert.That(preview.Defeats[0].TargetId, Is.EqualTo("surveyor"));
            Assert.That(preview.Defeats[1].TargetId, Is.EqualTo("walker"));
            Assert.That(preview.Cleared.HasValue, Is.True);
            session.CommitNext(both, preview);

            var playerOnly = new RegularEnemyEncounterLifecycleSession(Tick(0));
            var player = playerOnly.PreviewNext(Input(0, "player", 5, States(0, 6, 9)));
            Assert.That(player.Defeats, Is.Empty);
            Assert.That(player.Cleared.HasValue, Is.False);
            Assert.That(player.Completion.HasValue, Is.False);
        }

        // REQ-COM-004; partial AC-COM-003: reset is an empty, pre-death view
        // clear and lifecycle candidates cannot be replayed or crossed between owners.
        [Test]
        public void PreviewIsImmutableOwnerBoundAndPreDeathResetIsEmpty()
        {
            var session = new RegularEnemyEncounterLifecycleSession(Tick(0));
            var neutral = Empty(0, States(5, 6, 9));
            var preview = session.PreviewNext(neutral);
            Assert.Throws<NotSupportedException>(() => ((IList)preview.AppliedHitTargetIds).Add("walker"));
            Assert.Throws<NotSupportedException>(() => ((IList)preview.Defeats).Add(default(RegularEnemyDefeatedNotice)));
            var foreign = new RegularEnemyEncounterLifecycleSession(Tick(0));
            Assert.Throws<InvalidOperationException>(() => foreign.CommitNext(neutral, preview));
            session.CommitNext(neutral, preview);
            Assert.Throws<InvalidOperationException>(() => session.CommitNext(neutral, preview));
            var resetInput = new RegularEnemyEncounterLifecycleInput(Tick(1),
                RegularEnemyEncounterLifecycleInputKind.EncounterReset, Array.Empty<DamageRequest>(), Array.Empty<DamageResult>(), States(5, 6, 9));
            var reset = session.PreviewNext(resetInput);
            Assert.That(reset.AppliedHitTargetIds, Is.Empty);
            Assert.That(reset.Defeats, Is.Empty);
            session.CommitNext(resetInput, reset);
        }

        // REQ-COM-004, REQ-COM-005; partial AC-COM-003: all collection edges
        // are defensive copies, so caller mutation cannot change a preview or commit.
        [Test]
        public void InputSnapshotAndPreviewCollectionsCannotBeCastMutated()
        {
            var states = States(5, 0, 9);
            var request = Damage("surveyor", 6, 0);
            var input = Multi(0, new[] { request }, states);
            states[1] = new RegularEnemyEncounterRoleState("surveyor", 6, false);
            request[0] = Damage("walker", 9, 0)[0];
            Assert.Throws<NotSupportedException>(() => ((IList)input.Roles)[1] = States(5, 6, 9)[1]);
            Assert.Throws<NotSupportedException>(() => ((IList)input.CanonicalRequestTrace)[0] = Damage("walker", 9, 0));
            var session = new RegularEnemyEncounterLifecycleSession(Tick(0));
            var preview = session.PreviewNext(input);
            var mismatched = session.PreviewNext(Input(0, "walker", 1, States(5, 6, 8)));
            Assert.Throws<InvalidOperationException>(() => session.CommitNext(input, mismatched));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(0)));
            session.CommitNext(input, preview);
            Assert.That(session.LatestSnapshot.Value.Roles[1].IsDead, Is.True);
            Assert.Throws<NotSupportedException>(() => ((IList)session.LatestSnapshot.Value.Roles)[1] = States(5, 6, 9)[1]);
        }

        // REQ-COM-004; partial AC-COM-003: malformed roster, alignment,
        // conservation, resurrection, and skipped tick inputs preserve ownership.
        [Test]
        public void MalformedInputsAndTickBoundaryRejectAtomically()
        {
            var session = new RegularEnemyEncounterLifecycleSession(Tick(0));
            Assert.Throws<ArgumentException>(() => new RegularEnemyEncounterLifecycleInput(Tick(0), RegularEnemyEncounterLifecycleInputKind.Gameplay,
                Array.Empty<DamageRequest>(), Array.Empty<DamageResult>(), new[] { States(5, 6, 9)[0], States(5, 6, 9)[2], States(5, 6, 9)[1] }));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Empty(1, States(5, 6, 9))));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(0)));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(Empty(0, States(5, 5, 9))));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(0)));
            var terminal = new RegularEnemyEncounterLifecycleSession(new SimulationTick(int.MaxValue - 1));
            var terminalInput = Empty(int.MaxValue - 1, States(5, 6, 9));
            var terminalPreview = terminal.PreviewNext(terminalInput);
            terminal.CommitNext(terminalInput, terminalPreview);
            Assert.That(terminal.NextExpectedTick.Value, Is.EqualTo(int.MaxValue));
        }

        // REQ-COM-004; partial AC-COM-003: canonical duplicate results are
        // non-applied and malformed request/result alignment cannot mutate state.
        [Test]
        public void DuplicateAndMalformedCanonicalTraceCasesAreDeterministic()
        {
            var session = new RegularEnemyEncounterLifecycleSession(Tick(0));
            var duplicateRequest = new DamageRequest("duplicate/0", "player", "walker", 1, DamageKind.BasicAttack, Tick(0));
            var duplicate = new DamageResult("duplicate/0", "walker", 0, DamageResultCode.Duplicate, Tick(0));
            var duplicateInput = new RegularEnemyEncounterLifecycleInput(Tick(0), RegularEnemyEncounterLifecycleInputKind.Gameplay,
                new[] { duplicateRequest }, new[] { duplicate }, States(5, 6, 9));
            var preview = session.PreviewNext(duplicateInput);
            Assert.That(preview.AppliedHitTargetIds, Is.Empty);
            session.CommitNext(duplicateInput, preview);
            var malformed = new DamageResult("wrong/1", "walker", 0, DamageResultCode.Duplicate, Tick(1));
            Assert.Throws<InvalidOperationException>(() => session.PreviewNext(new RegularEnemyEncounterLifecycleInput(Tick(1),
                RegularEnemyEncounterLifecycleInputKind.Gameplay, new[] { new DamageRequest("right/1", "player", "walker", 1, DamageKind.BasicAttack, Tick(1)) },
                new[] { malformed }, States(5, 6, 9))));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(1)));
        }

        private static RegularEnemyEncounterLifecycleInput Input(int tick, string target, int amount, RegularEnemyEncounterRoleState[] roles)
        {
            var request = new DamageRequest("m3e1/" + target + "/" + tick, "player", target, amount, DamageKind.BasicAttack, Tick(tick));
            var result = new DamageResult(request.RequestId, target, amount, DamageResultCode.Applied, Tick(tick));
            return new RegularEnemyEncounterLifecycleInput(Tick(tick), RegularEnemyEncounterLifecycleInputKind.Gameplay,
                new[] { request }, new[] { result }, roles);
        }

        private static DamageRequest[] Damage(string target, int amount, int tick) => new[]
        {
            new DamageRequest("m3e1/" + target + "/" + tick, "player", target, amount, DamageKind.BasicAttack, Tick(tick))
        };

        private static RegularEnemyEncounterLifecycleInput Multi(int tick, DamageRequest[][] requests, RegularEnemyEncounterRoleState[] roles)
        {
            var flattened = new DamageRequest[requests.Length];
            var results = new DamageResult[requests.Length];
            for (var i = 0; i < requests.Length; i++) { flattened[i] = requests[i][0]; results[i] = new DamageResult(flattened[i].RequestId, flattened[i].TargetId, flattened[i].Amount, DamageResultCode.Applied, Tick(tick)); }
            return new RegularEnemyEncounterLifecycleInput(Tick(tick), RegularEnemyEncounterLifecycleInputKind.Gameplay, flattened, results, roles);
        }

        private static RegularEnemyEncounterLifecycleInput Empty(int tick, RegularEnemyEncounterRoleState[] roles) =>
            new RegularEnemyEncounterLifecycleInput(Tick(tick), RegularEnemyEncounterLifecycleInputKind.Gameplay,
                Array.Empty<DamageRequest>(), Array.Empty<DamageResult>(), roles);
        private static RegularEnemyEncounterRoleState[] States(int player, int surveyor, int walker) => new[]
        {
            new RegularEnemyEncounterRoleState("player", player, player == 0),
            new RegularEnemyEncounterRoleState("surveyor", surveyor, surveyor == 0),
            new RegularEnemyEncounterRoleState("walker", walker, walker == 0)
        };
        private static SimulationTick Tick(int value) => new SimulationTick(value);
    }
}
