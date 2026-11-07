using System;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    internal sealed class OrdanBossHostileGeometryTests
    {
        // AC-M4B3B2-001. The compiled descriptor is the exact approved Q4096 table.
        [Test]
        public void ExactDescriptorAndOccluderOrderAreFrozen()
        {
            Assert.That(OrdanBossHostileDamageProducer.ApprovedPlayerHalfWidth, Is.EqualTo(1639));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedPlayerHalfHeight, Is.EqualTo(3277));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedProjectileHalfExtent, Is.EqualTo(1024));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedSourceX, Is.EqualTo(25395));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedSourceY, Is.EqualTo(4915));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedDebtEndX, Is.EqualTo(-45056));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedArena, Is.EqualTo(new OrdanBossHostileDamageProducer.ClosedAabbQ4096(-47104, 0, 47104, 40960)));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedOrdanBody, Is.EqualTo(OrdanBossHostileDamageProducer.ClosedAabbQ4096.FromCenterHalf(28672, 4915, 3277, 4915)));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedDebtLineA, Is.EqualTo(new OrdanBossHostileDamageProducer.ClosedAabbQ4096(-45056, 3891, 25395, 5939)));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedDebtLineB, Is.EqualTo(new OrdanBossHostileDamageProducer.ClosedAabbQ4096(-45056, 11264, 25395, 13312)));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedSeizureStartXs, Is.EqualTo(new[] { 12288, 20480, 28672 }));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedSeizureStartY, Is.EqualTo(24576));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedSeizurePivotX, Is.EqualTo(8192));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedSeizurePivotY, Is.EqualTo(12288));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedSeizureEndX, Is.EqualTo(-28672));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedSeizureEndY, Is.EqualTo(4915));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedNormalPathPointCount, Is.EqualTo(3));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedAcceleratedPathPointCount, Is.EqualTo(2));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedNormalDurations, Is.EqualTo(new[] { 72, 66, 60 }));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedAcceleratedDurations, Is.EqualTo(new[] { 24, 22, 20 }));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedAuditCenterX, Is.EqualTo(28672));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedAuditCenterY, Is.EqualTo(4915));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedAuditInnerRadius, Is.EqualTo(49152));
            Assert.That(OrdanBossHostileDamageProducer.ApprovedAuditOuterRadius, Is.EqualTo(65536));
            var values = OrdanBossHostileDamageProducer.ApprovedOccluders;
            Assert.That(values, Has.Count.EqualTo(3));
            Assert.That(values[0], Is.EqualTo(OrdanBossHostileDamageProducer.ClosedAabbQ4096.FromCenterHalf(0, -2048, 49152, 2048)));
            Assert.That(values[1], Is.EqualTo(OrdanBossHostileDamageProducer.ClosedAabbQ4096.FromCenterHalf(-49152, 18432, 2048, 22528)));
            Assert.That(values[2], Is.EqualTo(OrdanBossHostileDamageProducer.ClosedAabbQ4096.FromCenterHalf(49152, 18432, 2048, 22528)));
            Assert.Throws<OverflowException>(() => OrdanBossHostileDamageProducer.ClosedAabbQ4096.FromCenterHalf(int.MaxValue, 0, 1, 0));
        }

        // AC-M4B3B2-001, AC-M4B3B2-004. Closed edges touch and the next Q4096 unit misses.
        [Test]
        public void DebtLineUsesInclusivePlayerBoundary()
        {
            var attack = new OrdanHostileAttack("DebtLineA", 0, string.Empty, null);
            var touching = Snapshot(0, 9216);
            var outside = Snapshot(0, 9217);
            Assert.That(OrdanBossHostileDamageProducer.EvaluateGeometryForTests(attack, touching).Contact, Is.True);
            Assert.That(OrdanBossHostileDamageProducer.EvaluateGeometryForTests(attack, outside).Contact, Is.False);
        }

        // AC-M4B3B2-001. Equal entry favors the first literal occluder.
        [Test]
        public void EqualEdgeOcclusionIsClosedAndUsesStableOrder()
        {
            var attack = new OrdanHostileAttack("DebtLineA", 2, string.Empty, null);
            var player = new OrdanBossHostileDamageProducer.ClosedAabbQ4096(50, -1, 60, 1);
            var blocker = new OrdanBossHostileDamageProducer.ClosedAabbQ4096(50, -1, 50, 1);
            var result = OrdanBossHostileDamageProducer.EvaluateSegmentForTests(attack, 0, 0, 100, 0, 0, 0,
                player, new[] { blocker, blocker });
            Assert.That(result.Contact, Is.True);
            Assert.That(result.Occluded, Is.True);
            Assert.That(result.OccluderIndex, Is.Zero);
        }

        // AC-M4B3B2-002. Every stage-dependent duration is representable and final endpoints contact.
        [TestCase(false, 72)] [TestCase(false, 66)] [TestCase(false, 60)]
        [TestCase(true, 24)] [TestCase(true, 22)] [TestCase(true, 20)]
        public void AllSixSeizureDurationsReachTheExactFinalEndpoint(bool accelerated, int duration)
        {
            var projection = new SeizureTrajectoryProjection(new SimulationTick(100), "BossWeight-0", 7,
                duration - 1, duration, accelerated);
            var attack = new OrdanHostileAttack("Seizure", 7, "BossWeight-0", projection);
            var result = OrdanBossHostileDamageProducer.EvaluateGeometryForTests(attack, Snapshot(-28672, 4915));
            Assert.That(result.Contact, Is.True);
            Assert.That(result.EndX, Is.EqualTo(-28672));
            Assert.That(result.EndY, Is.EqualTo(4915));
        }

        // AC-M4B3B2-003, AC-M4B3B2-007. Stable IDs and digest optional fields are exact.
        [Test]
        public void RequestIdsAndDigestMatrixAreExact()
        {
            var delivery = new SimulationTick(42);
            var debt = new OrdanHostileAttack("DebtLineA", 3, string.Empty, null);
            var debtB = new OrdanHostileAttack("DebtLineB", 6, string.Empty, null);
            var audit = new OrdanHostileAttack("Audit", 4, string.Empty, null);
            var projection = new SeizureTrajectoryProjection(new SimulationTick(41), "BossWeight-2", 5, 0, 24, true);
            var seizure = new OrdanHostileAttack("Seizure", 5, "BossWeight-2", projection);
            Assert.That(OrdanBossHostileDamageProducer.FormatRequestIdForTests(debt, delivery), Is.EqualTo("ordan.hostile/debt-a/0000000000000000003/0000000042"));
            Assert.That(OrdanBossHostileDamageProducer.FormatRequestIdForTests(debtB, delivery), Is.EqualTo("ordan.hostile/debt-b/0000000000000000006/0000000042"));
            Assert.That(OrdanBossHostileDamageProducer.FormatRequestIdForTests(audit, delivery), Is.EqualTo("ordan.hostile/audit/0000000000000000004/0000000042"));
            var id = OrdanBossHostileDamageProducer.FormatRequestIdForTests(seizure, delivery);
            Assert.That(id, Is.EqualTo("ordan.hostile/seizure/0000000000000000005/BossWeight-2/0000000042"));
            var request = new DamageRequest(id, "ordan", "player", 1, DamageKind.BossPattern, delivery);
            var digest = OrdanBossHostileDamageDigest.Queued(new SimulationTick(41), seizure, request);
            Assert.That(digest.Disposition, Is.EqualTo(OrdanBossHostileDisposition.Queued));
            Assert.That(digest.PayloadId, Is.EqualTo("BossWeight-2"));
            Assert.That(digest.RequestId, Is.EqualTo(id));
            Assert.That(digest.DeliveryTick, Is.EqualTo(delivery));
            Assert.That(OrdanBossHostileDamageDigest.NoAttack(new SimulationTick(0)).AttackOrdinal, Is.EqualTo(-1));
        }

        // AC-M4B3B2-003. A queued digest refuses any request whose exact source,
        // target, amount, kind, horizon, or attack-derived ID was forged.
        [Test]
        public void QueuedDigestRejectsForgedRequestFieldsAndIdentity()
        {
            var source = new SimulationTick(7);
            var delivery = new SimulationTick(8);
            var attack = new OrdanHostileAttack("DebtLineA", 9, string.Empty, null);
            var exactId = OrdanBossHostileDamageProducer.FormatRequestIdForTests(attack, delivery);
            Assert.Throws<ArgumentException>(() => OrdanBossHostileDamageDigest.Queued(source, attack,
                new DamageRequest(exactId, "other", "player", 1, DamageKind.BossPattern, delivery)));
            Assert.Throws<ArgumentException>(() => OrdanBossHostileDamageDigest.Queued(source, attack,
                new DamageRequest(exactId, "ordan", "other", 1, DamageKind.BossPattern, delivery)));
            Assert.Throws<ArgumentException>(() => OrdanBossHostileDamageDigest.Queued(source, attack,
                new DamageRequest(exactId, "ordan", "player", 2, DamageKind.BossPattern, delivery)));
            Assert.Throws<ArgumentException>(() => OrdanBossHostileDamageDigest.Queued(source, attack,
                new DamageRequest(exactId, "ordan", "player", 1, DamageKind.EnemyContact, delivery)));
            Assert.Throws<ArgumentException>(() => OrdanBossHostileDamageDigest.Queued(source, attack,
                new DamageRequest(exactId, "ordan", "player", 1, DamageKind.BossPattern, new SimulationTick(9))));
            Assert.Throws<ArgumentException>(() => OrdanBossHostileDamageDigest.Queued(source, attack,
                new DamageRequest("ordan.hostile/debt-b/0000000000000000009/0000000008", "ordan", "player", 1,
                    DamageKind.BossPattern, delivery)));
        }

        // AC-M4B3B2-003. Even an empty append is a committed owner action and
        // cannot be mistaken for a pristine pending delivery on a second pass.
        [Test]
        public void DeliveryBatchRetainsOneShotHostileAppendState()
        {
            var batch = new OrdanBossDeliveryBatch(new SimulationTick(1), new SimulationTick(2), null,
                Array.Empty<DamageRequest>(), true);
            Assert.That(batch.HostileAppendCommitted, Is.True);
            Assert.That(batch.HostileRequests, Is.Empty);
            Assert.That(batch.Equals(new OrdanBossDeliveryBatch(new SimulationTick(1), new SimulationTick(2), null)), Is.False);
        }

        private static PlayerMotionSnapshot Snapshot(int x, int y) => new PlayerMotionSnapshot(new SimulationTick(0),
            x, y, 0, 0, false, string.Empty, WallSide.None, MovementActionState.Normal, true);
    }
}
