// AC-M4B3B3-006, AC-M4B3B3-008.
using System;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    internal sealed class OrdanBossBalanceAuditPullContractTests
    {
        [Test]
        public void StableDirectiveIdentityIsBoundedAsciiAndTickQualified()
        {
            var id = OrdanBossBalanceAuditPullProducer.FormatDirectiveId(17, new SimulationTick(42));

            Assert.That(id, Is.EqualTo("ordan.audit-pull/0000000000000000017/0000000042"));
            Assert.That(id.Length, Is.LessThanOrEqualTo(96));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                OrdanBossBalanceAuditPullProducer.FormatDirectiveId(-1, new SimulationTick(0)));
        }

        [Test]
        public void HandoffSeparatesConsumedCurrentFromQueuedNext()
        {
            var tick = new SimulationTick(20);
            var projection = new BalanceAuditPullProjection(tick, 7, 5, 90);
            var currentId = OrdanBossBalanceAuditPullProducer.FormatDirectiveId(7, tick);
            var nextTick = new SimulationTick(21);
            var nextId = OrdanBossBalanceAuditPullProducer.FormatDirectiveId(7, nextTick);
            var outcome = OrdanBossBalanceAuditPullOutcome.Queued(tick, projection, currentId, nextId, nextTick);
            var receipt = new ExternalMovementDirectiveReceipt(tick, true, currentId, 300, -200);

            var digest = OrdanBossBalanceAuditPullDigest.From(tick, outcome, receipt);

            Assert.That(digest.ConsumedCurrentDirectiveId, Is.EqualTo(currentId));
            Assert.That(digest.QueuedNextDirectiveId, Is.EqualTo(nextId));
            Assert.That(digest.QueuedNextMovementTick, Is.EqualTo(nextTick));
            Assert.That(digest.AppliedDeltaXQ4096, Is.EqualTo(300));
            Assert.That(digest.AppliedDeltaYQ4096, Is.EqualTo(-200));
        }

        [Test]
        public void HandoffRejectsMissingMismatchedOrUnexpectedReceipts()
        {
            var tick = new SimulationTick(20);
            var projection = new BalanceAuditPullProjection(tick, 7, 5, 90);
            var currentId = OrdanBossBalanceAuditPullProducer.FormatDirectiveId(7, tick);
            var nextTick = new SimulationTick(21);
            var nextId = OrdanBossBalanceAuditPullProducer.FormatDirectiveId(7, nextTick);
            var queued = OrdanBossBalanceAuditPullOutcome.Queued(tick, projection, currentId, nextId, nextTick);
            var noProjection = OrdanBossBalanceAuditPullOutcome.NoProjection(tick);

            Assert.Throws<InvalidOperationException>(() => OrdanBossBalanceAuditPullDigest.From(
                tick, queued, new ExternalMovementDirectiveReceipt(tick, false, string.Empty, 0, 0)));
            Assert.Throws<InvalidOperationException>(() => OrdanBossBalanceAuditPullDigest.From(
                tick, queued, new ExternalMovementDirectiveReceipt(tick, true, "foreign", 0, 0)));
            Assert.Throws<InvalidOperationException>(() => OrdanBossBalanceAuditPullDigest.From(
                tick, noProjection, new ExternalMovementDirectiveReceipt(tick, true, currentId, 0, 0)));
        }
    }
}
