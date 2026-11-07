// AC-COM-003, AC-COM-004; AC-WT-003, AC-WT-005.
using System;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    internal sealed class OrdanBossTransferExposureTests
    {
        [Test]
        public void SchedulerHasExactMinusOneEightyOrder()
        {
            var attribute = (DefaultExecutionOrder)Attribute.GetCustomAttribute(typeof(OrdanBossTransferExposureScheduler), typeof(DefaultExecutionOrder));
            Assert.That(attribute, Is.Not.Null); Assert.That(attribute.order, Is.EqualTo(-180));
        }

        [Test]
        public void PayloadSinkRequiresExposureAndExactScriptedRequest()
        {
            var objectValue = new GameObject("BossWeight-0");
            try
            {
                var sink = objectValue.AddComponent<OrdanBossPayloadTransferModifierSink>();
                sink.ConfigureForTests("BossWeight-0");
                var request = new TransferModifierRequest("BossWeight-0", TransferTargetKind.BossPayload,
                    OrdanBossPayloadTransferModifierSink.RequiredBaseProfileId, OrdanBossPayloadTransferModifierSink.RequiredModifierId, new SimulationTick(0), 1);
                Assert.That(sink.TryApply(request), Is.False);
                sink.CommitExposure(true);
                Assert.That(sink.TryApply(request), Is.True); Assert.That(sink.IsApplied, Is.True);
                sink.CommitExposure(false);
                sink.Clear(TransferClearReason.TargetRemoved, new SimulationTick(1), 2);
                Assert.That(sink.IsApplied, Is.False); Assert.That(sink.IsStillAvailable(), Is.False);
            }
            finally { UnityEngine.Object.DestroyImmediate(objectValue); }
        }

        // AC-WT-005. Clear accepts an increasing Transfer revision even if a
        // hidden sink has no bit left, and rejects a stale clear deterministically.
        [Test]
        public void HiddenSinkClearAdvancesRevisionWithoutRestoringAvailability()
        {
            var objectValue = new GameObject("BossWeight-1");
            try
            {
                var sink = objectValue.AddComponent<OrdanBossPayloadTransferModifierSink>(); sink.ConfigureForTests("BossWeight-1");
                sink.Clear(TransferClearReason.TargetRemoved, new SimulationTick(3), 1);
                Assert.That(sink.LastAcceptedRevision, Is.EqualTo(1)); Assert.That(sink.IsStillAvailable(), Is.False);
                Assert.Throws<ArgumentOutOfRangeException>(() => sink.Clear(TransferClearReason.TargetRemoved, new SimulationTick(4), 1));
            }
            finally { UnityEngine.Object.DestroyImmediate(objectValue); }
        }

    }
}
