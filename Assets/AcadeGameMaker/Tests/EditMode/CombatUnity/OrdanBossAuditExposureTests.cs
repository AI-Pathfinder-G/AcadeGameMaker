// AC-COM-002, AC-COM-003; AC-WT-003, AC-WT-005.
using System;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    internal sealed class OrdanBossAuditExposureTests
    {
        // AC-COM-002. M4A candidate state, rather than Unity phase inspection,
        // owns the first and final legal audit Transfer horizons.
        [Test]
        public void AuditForecastBeginsAtExecuteEntryAndEndsBeforeRecovery()
        {
            var session = new OrdanBossSession(new SimulationTick(0));
            for (var tick = 0; tick <= 455; tick++) Step(session, tick);
            Assert.That(session.LatestOutput.AuditExposureForecast, Is.EqualTo(new OrdanAuditExposureForecast(new SimulationTick(455), new SimulationTick(456), "BossAuditBox-0")));
            for (var tick = 456; tick <= 544; tick++) Step(session, tick);
            Assert.That(session.LatestOutput.AuditExposureForecast.TargetId, Is.EqualTo("BossAuditBox-0"));
            Step(session, 545);
            Assert.That(session.LatestOutput.AuditExposureForecast.IsExposed, Is.False);
            Assert.That(session.LatestOutput.TransferExposureForecast.IsExposed, Is.False);
        }

        // AC-COM-002, AC-COM-003. Forecast horizons and mutually exclusive
        // outputs reject forged values without adding Unity ownership.
        [Test]
        public void AuditForecastAndOutputHaveStrictValueSemantics()
        {
            var audit = new OrdanAuditExposureForecast(new SimulationTick(3), new SimulationTick(4), "BossAuditBox-0");
            Assert.That(audit, Is.EqualTo(new OrdanAuditExposureForecast(new SimulationTick(3), new SimulationTick(4), "BossAuditBox-0")));
            Assert.Throws<ArgumentException>(() => new OrdanAuditExposureForecast(new SimulationTick(3), new SimulationTick(4), "foreign"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrdanAuditExposureForecast(new SimulationTick(3), new SimulationTick(5), string.Empty));

            var snapshot = new OrdanBossSnapshot(new SimulationTick(3), OrdanPatternSlot.BalanceAudit, OrdanBossPhase.Execute, 0,
                OrdanHealthStage.Stage1, OrdanHealthStage.Stage1, string.Empty, false, false, OrdanVulnerabilitySource.None, new SimulationTick(4));
            var payload = new OrdanTransferExposureForecast(new SimulationTick(3), new SimulationTick(4), "BossWeight-0");
            Assert.Throws<ArgumentException>(() => new OrdanBossTickOutput(snapshot, Array.Empty<OrdanBossIntent>(), Array.Empty<OrdanBossHandoff>(), payload, audit));
        }

        // AC-WT-005. The audit projection is independently immutable and
        // value-equal, preserving the legacy payload outcome's shape.
        [Test]
        public void AuditOutcomeCopiesInputsAndRejectsForgedOrMixedTerminalShapes()
        {
            var source = new SimulationTick(7); var transfer = new SimulationTick(8);
            var ends = new[] { "BossAuditBox-0" };
            var value = new OrdanBossAuditExposureOutcome(source, transfer, "BossAuditBox-0", string.Empty, ends, Array.Empty<string>(), false);
            Assert.That(value, Is.EqualTo(new OrdanBossAuditExposureOutcome(source, transfer, "BossAuditBox-0", string.Empty,
                new[] { "BossAuditBox-0" }, Array.Empty<string>(), false)));
            ends[0] = "foreign";
            Assert.That(value.TemporarilyEndedTargetIds, Is.EqualTo(new[] { "BossAuditBox-0" }));
            Assert.Throws<ArgumentException>(() => new OrdanBossAuditExposureOutcome(source, transfer, string.Empty, string.Empty,
                Array.Empty<string>(), new[] { "BossAuditBox-0" }, false));
            Assert.Throws<ArgumentException>(() => new OrdanBossAuditExposureOutcome(source, transfer, "BossAuditBox-0", string.Empty,
                new[] { "BossAuditBox-0" }, new[] { "BossAuditBox-0" }, true));
        }

        // AC-COM-003. The one registration lane accepts exactly typed legacy
        // three or production audit+three IDs and rejects malformed sequences
        // before its owner latch can be replaced.
        [Test]
        public void ExposureLaneRegistrationMatrixIsExactForLegacyAndProduction()
        {
            AssertRegistration(new[] { "BossWeight-0", "BossWeight-1", "BossWeight-2" }, false);
            AssertRegistration(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }, false);
            AssertRegistration(new[] { "BossAuditBox-0", "BossWeight-1", "BossWeight-0", "BossWeight-2" }, true);
            AssertRegistration(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1" }, true);
            AssertRegistration(new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "foreign" }, true);
        }

        private static void AssertRegistration(string[] ids, bool rejected)
        {
            var values = new System.Collections.Generic.List<GameObject>();
            try
            {
                var playerRoot = new GameObject("player"); values.Add(playerRoot); playerRoot.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; playerRoot.AddComponent<BoxCollider2D>();
                var player = playerRoot.AddComponent<AcadeGameMaker.Movement.PlayerMovementController>();
                var targets = new TransferTarget[4];
                var auditRoot = new GameObject("BossAuditBox-0"); values.Add(auditRoot); var auditCollider = auditRoot.AddComponent<BoxCollider2D>(); var auditSink = auditRoot.AddComponent<OrdanBossAuditTransferModifierSink>(); auditSink.ConfigureForTests("BossAuditBox-0");
                targets[0] = auditRoot.AddComponent<TransferTarget>(); targets[0].ConfigureForTests("BossAuditBox-0", TransferTargetKind.Box, 0, 0, 0, 0, 100, 100, OrdanBossAuditTransferModifierSink.RequiredBaseProfileId, auditCollider, auditRoot.transform, auditSink);
                for (var index = 0; index < 3; index++)
                {
                    var root = new GameObject("BossWeight-" + index); values.Add(root); var collider = root.AddComponent<BoxCollider2D>(); var sink = root.AddComponent<OrdanBossPayloadTransferModifierSink>(); sink.ConfigureForTests("BossWeight-" + index);
                    targets[index + 1] = root.AddComponent<TransferTarget>(); targets[index + 1].ConfigureForTests("BossWeight-" + index, TransferTargetKind.BossPayload, 0, 0, 0, 0, 100, 100, OrdanBossPayloadTransferModifierSink.RequiredBaseProfileId, collider, root.transform, sink);
                }
                var registryRoot = new GameObject("registry"); values.Add(registryRoot); var registry = registryRoot.AddComponent<TransferTargetRegistry>(); registry.ConfigureForTests(targets);
                var driverRoot = new GameObject("driver"); values.Add(driverRoot); var driver = driverRoot.AddComponent<TransferSimulationDriver>(); driver.ConfigureForTests(player, registry);
                if (rejected) Assert.Throws<InvalidOperationException>(() => driver.RegisterExposureEndLane(new object(), ids));
                else
                {
                    driver.RegisterExposureEndLane(new object(), ids);
                    Assert.Throws<InvalidOperationException>(() => driver.RegisterExposureEndLane(new object(), ids));
                }
            }
            finally { for (var index = values.Count - 1; index >= 0; index--) UnityEngine.Object.DestroyImmediate(values[index]); }
        }

        private static void Step(OrdanBossSession session, int tick) => session.Step(new OrdanBossTickInput(new SimulationTick(tick),
            new OrdanCombatState(new SimulationTick(tick), "ordan", 60, false, null), null));
    }
}
