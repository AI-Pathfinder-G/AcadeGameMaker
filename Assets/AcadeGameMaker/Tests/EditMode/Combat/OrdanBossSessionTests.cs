using System;
using System.Collections.Generic;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Core;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Combat
{
    internal sealed class OrdanBossSessionTests
    {
        // AC-COM-002, AC-COM-003: stable four-slot cycle and payload handles do not use Unity identity or creation order.
        [Test]
        public void CursorAndPayloadHandlesFollowTheFrozenFourSlotAndZeroOneTwoSequence()
        {
            var session = Session();
            Assert.That(Step(session, 0).Snapshot.PatternSlot, Is.EqualTo(OrdanPatternSlot.DebtLineA));
            AdvanceThrough(session, 107); var first = Step(session, 108);
            Assert.That(first.Snapshot.PatternSlot, Is.EqualTo(OrdanPatternSlot.SeizureWeight)); Assert.That(first.Snapshot.ActiveHandleId, Is.EqualTo("BossWeight-0"));
            AdvanceThrough(session, 701); var second = Step(session, 702);
            Assert.That(second.Snapshot.PatternSlot, Is.EqualTo(OrdanPatternSlot.SeizureWeight)); Assert.That(second.Snapshot.ActiveHandleId, Is.EqualTo("BossWeight-1"));
        }

        // AC-COM-002: exact exclusive endpoints pin the Stage 1 table.
        [Test]
        public void StageOneDebtLineHasFortyEightTwentyOneAndThirtyNineExactTicks()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 47);
            Assert.That(Step(session, 48).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Execute));
            AdvanceThrough(session, 68); Assert.That(Step(session, 69).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Recovery));
            AdvanceThrough(session, 107); Assert.That(Step(session, 108).Snapshot.PatternSlot, Is.EqualTo(OrdanPatternSlot.SeizureWeight));
        }

        // AC-COM-002: stage scaling is promoted only at the following Telegraph and preserves ceil timing.
        [Test]
        public void StageTwoAndThreeTimingPromotionUsesTheFrozenCeilTable()
        {
            var stageTwo = Session(); Step(stageTwo, 0); Step(stageTwo, 1, 41); AdvanceThrough(stageTwo, 107, 41);
            var seizureTwo = Step(stageTwo, 108, 41); Assert.That(seizureTwo.Snapshot.ActiveStage, Is.EqualTo(OrdanHealthStage.Stage2));
            AdvanceThrough(stageTwo, 167, 41); Assert.That(Step(stageTwo, 168, 41).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Execute));
            AdvanceThrough(stageTwo, 233, 41); Assert.That(Step(stageTwo, 234, 41).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Recovery));
            var stageThree = Session(); Step(stageThree, 0); Step(stageThree, 1, 20); AdvanceThrough(stageThree, 107, 20);
            Assert.That(Step(stageThree, 108, 20).Snapshot.ActiveStage, Is.EqualTo(OrdanHealthStage.Stage3));
            AdvanceThrough(stageThree, 167, 20); AdvanceThrough(stageThree, 227, 20);
            Assert.That(Step(stageThree, 228, 20).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Recovery));
        }

        // AC-COM-003: exact threshold values do not promote; strict crossings do, and one hit may pend Stage 3.
        [Test]
        public void HealthThresholdsAreStrictMonotonicAndNextPatternOnly()
        {
            var session = Session(); Step(session, 0); var at42 = Step(session, 1, 42);
            Assert.That(at42.Snapshot.ActiveStage, Is.EqualTo(OrdanHealthStage.Stage1)); Assert.That(at42.Snapshot.PendingStage, Is.EqualTo(OrdanHealthStage.Stage1));
            var at41 = Step(session, 2, 41); Assert.That(at41.Snapshot.ActiveStage, Is.EqualTo(OrdanHealthStage.Stage1)); Assert.That(at41.Snapshot.PendingStage, Is.EqualTo(OrdanHealthStage.Stage2));
            var direct = Session(); Step(direct, 0); Assert.That(Step(direct, 1, 20).Snapshot.PendingStage, Is.EqualTo(OrdanHealthStage.Stage3));
        }

        // AC-COM-002: DebtLine A and B each produce exactly one semantic intent and ignore transfer absence.
        [Test]
        public void DebtLineAAndBEmitOneIntentAtExecuteAgeZero()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 47); var a = Step(session, 48);
            AssertIntent(a, OrdanBossIntentKind.DebtLine, "ordan.debt/0000000000000000000");
            AdvanceThrough(session, 335); var b = Step(session, 336);
            AssertIntent(b, OrdanBossIntentKind.DebtLine, "ordan.debt/0000000000000000002");
        }

        // AC-COM-004, AC-WT-005: Telegraph endpoints accept one rising payload edge, freeze 24 ticks, and queue exact t+1 damage.
        [Test]
        public void PayloadEdgeAtTelegraphEndpointAcceleratesAndProducesAlignedRequest()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 107);
            AdvanceThrough(session, 166); var edge = PayloadEdge(167, "BossWeight-0", 0); var accelerated = Step(session, 167, 60, edge);
            AssertIntent(accelerated, OrdanBossIntentKind.PayloadAcceleration, "ordan.payload-accelerate/0000000000000000000");
            var execute = Step(session, 168); Assert.That(execute.Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Execute));
            AdvanceThrough(session, 190); var request = Step(session, 191);
            AssertIntent(request, OrdanBossIntentKind.PayloadDamageRequest, "ordan.payload/0000000000000000000/0000000192");
            Assert.That(request.Intents[0].FutureDamageRequest.Value.Amount, Is.EqualTo(8));
            var applied = new DamageResult(request.Intents[0].Id, "ordan", 8, DamageResultCode.Applied, Tick(192));
            var vulnerable = Step(session, 192, 52, null, applied);
            Assert.That(vulnerable.Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Vulnerable)); Assert.That(vulnerable.Snapshot.VulnerabilitySource, Is.EqualTo(OrdanVulnerabilitySource.SeizureWeightTransfer));
        }

        // AC-COM-003, AC-WT-005: stale, duplicate and Execute payload edges reject before state publication.
        [Test]
        public void InvalidPayloadEdgeLeavesCompletePriorStateUntouched()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 107); AdvanceThrough(session, 167);
            var before = session.LatestOutput; Assert.Throws<InvalidOperationException>(() => Step(session, 168, 60, PayloadEdge(168, "BossWeight-0", 0)));
            Assert.That(session.LatestOutput, Is.EqualTo(before)); Assert.That(session.NextExpectedTick.Value, Is.EqualTo(168));
        }

        // AC-COM-004: 120 snapshot vulnerability emits no hostile intents and returns to Recovery only at its exclusive end.
        [Test]
        public void PayloadVulnerabilityLastsExactlyOneHundredTwentyTicks()
        {
            var session = AccelerateToVulnerability();
            AdvanceThrough(session, 311, 52); var last = Step(session, 312, 52);
            Assert.That(last.Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Recovery)); Assert.That(last.Intents.Count, Is.EqualTo(0));
        }

        // AC-COM-002, AC-WT-005: audit first/last Execute priorities distinguish pull, interrupt and shockwave.
        [Test]
        public void BalanceAuditInterruptWinsOverShockwaveAndStartsNinetyTickWindow()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 455); var first = Step(session, 456, 60, AuditEdge(456, 0));
            Assert.That(first.Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Vulnerable)); Assert.That(first.Intents.Count, Is.EqualTo(2)); Assert.That(first.Intents[0].Kind, Is.EqualTo(OrdanBossIntentKind.BalanceAuditPull)); Assert.That(first.Intents[1].Kind, Is.EqualTo(OrdanBossIntentKind.BalanceAuditInterrupt));
            AdvanceThrough(session, 545); Assert.That(Step(session, 546).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Recovery));
            var uninterrupted = Session(); Step(uninterrupted, 0); AdvanceThrough(uninterrupted, 544); var shockwave = Step(uninterrupted, 545); AssertIntent(shockwave, OrdanBossIntentKind.BalanceAuditShockwave, "ordan.audit-shockwave/0000000000000000003");
        }

        // AC-COM-003: current Combat death wins over phase and transfer, and all three future handoffs are one-shot.
        [Test]
        public void DeathWinsSameTickAndCannotResurrectOrReset()
        {
            var session = Session(); Step(session, 0); var dead = Step(session, 1, 0);
            Assert.That(dead.Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Defeated)); Assert.That(dead.Handoffs.Count, Is.EqualTo(3));
            var repeated = Step(session, 2, 0); Assert.That(repeated.Handoffs.Count, Is.EqualTo(0));
            Assert.Throws<InvalidOperationException>(() => Step(session, 3, 1));
        }

        // AC-COM-003: forged previews, wrong clocks and missing/partial payload results are all atomic.
        [Test]
        public void ForgedPreviewAndMalformedResultAreRejectedWithoutMutation()
        {
            var session = Session(); Step(session, 0); var before = session.LatestOutput;
            Assert.Throws<InvalidOperationException>(() => session.CommitNext(default(OrdanBossTickCandidate), Input(1, 60)));
            Assert.That(session.LatestOutput, Is.EqualTo(before)); Assert.Throws<InvalidOperationException>(() => Step(session, 2));
            var payload = Session(); Step(payload, 0); AdvanceThrough(payload, 107); Step(payload, 108, 60, PayloadEdge(108, "BossWeight-0", 0)); AdvanceThrough(payload, 190); var request = Step(payload, 191);
            Assert.Throws<InvalidOperationException>(() => Step(payload, 192, 53, null, new DamageResult(request.Intents[0].Id, "ordan", 7, DamageResultCode.Applied, Tick(192))));
            Assert.That(payload.NextExpectedTick.Value, Is.EqualTo(192));
        }

        // AC-COM-003: checked phase horizon overflow rejects before mutation.
        [Test]
        public void ConstructorAndFuturePhaseOverflowAreAtomic()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrdanBossSession(Tick(int.MaxValue - 119)));
            var session = new OrdanBossSession(Tick(int.MaxValue - 120));
            var start = session.NextExpectedTick.Value; for (var tick = start; tick < int.MaxValue - 12; tick++) Step(session, tick);
            var before = session.LatestOutput; Assert.Throws<OverflowException>(() => Step(session, int.MaxValue - 12)); Assert.That(session.LatestOutput, Is.EqualTo(before));
        }

        // AC-COM-002, AC-COM-003: immutable outputs resist caller collection mutation and repeated scripts exactly match.
        [Test]
        public void OutputIsImmutableAndThreeEquivalentScriptsMatch()
        {
            var a = Trace(); var b = Trace(); var c = Trace(); Assert.That(a, Is.EqualTo(b)); Assert.That(b, Is.EqualTo(c));
            var session = Session(); var output = Step(session, 0); Assert.Throws<NotSupportedException>(() => ((IList<OrdanBossIntent>)output.Intents).Add(default(OrdanBossIntent)));
        }

        // AC-COM-002, AC-WT-005: M4A, not Unity, forecasts the full next
        // Transfer tick. This pins Recovery->Seizure age zero and the final
        // Telegraph->Execute clear without exposing payload ordinal state.
        [Test]
        public void NextTransferExposureForecastPinsAgeZeroAndTelegraphExclusiveEnd()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 106);
            var beforeSeizure = Step(session, 107);
            Assert.That(beforeSeizure.TransferExposureForecast.SourceTick, Is.EqualTo(Tick(107)));
            Assert.That(beforeSeizure.TransferExposureForecast.TransferTick, Is.EqualTo(Tick(108)));
            Assert.That(beforeSeizure.TransferExposureForecast.TargetId, Is.EqualTo("BossWeight-0"));
            AdvanceThrough(session, 166);
            Assert.That(Step(session, 167).TransferExposureForecast.TargetId, Is.EqualTo(string.Empty));
        }

        // AC-COM-002, AC-WT-005: M4A owns all Telegraph forecast ages and the
        // private 0/1/2/0 payload cycle; no Unity table or ordinal participates.
        [Test]
        public void ForecastCoversTelegraphAgesAndRepeatsPayloadHandlesZeroOneTwoZero()
        {
            var ages = Session(); Step(ages, 0); AdvanceThrough(ages, 107);
            Assert.That(ages.LatestOutput.TransferExposureForecast.TargetId, Is.EqualTo("BossWeight-0")); // next age 0
            Assert.That(Step(ages, 108).TransferExposureForecast.TargetId, Is.EqualTo("BossWeight-0")); // next age 1
            AdvanceThrough(ages, 165); Assert.That(Step(ages, 166).TransferExposureForecast.TargetId, Is.EqualTo("BossWeight-0")); // next age 59
            Assert.That(Step(ages, 167).TransferExposureForecast.IsExposed, Is.False); // next Execute age 0

            var cycle = Session();
            var forecastTicks = new[] { 107, 701, 1295, 1889 };
            var ids = new[] { "BossWeight-0", "BossWeight-1", "BossWeight-2", "BossWeight-0" };
            for (var index = 0; index < forecastTicks.Length; index++)
            {
                AdvanceThrough(cycle, forecastTicks[index]);
                Assert.That(cycle.LatestOutput.TransferExposureForecast.TargetId, Is.EqualTo(ids[index]));
            }
        }

        // AC-COM-002: every unaccelerated Seizure and Audit stage-table duration has an exact exclusive end.
        [Test]
        public void CompleteSeizureAndAuditTimingTableMatchesAllThreeStages()
        {
            AssertPhaseEnd(EnterSeizure(60), 168, 240, OrdanBossPhase.Recovery); // normal 72
            AssertPhaseEnd(EnterSeizure(41), 168, 234, OrdanBossPhase.Recovery); // normal 66
            AssertPhaseEnd(EnterSeizure(20), 168, 228, OrdanBossPhase.Recovery); // normal 60
            AssertAuditEnds(60, 456, 546, 594); // 90 / 48
            AssertAuditEnds(41, 442, 524, 568); // 82 / 44
            AssertAuditEnds(20, 427, 502, 542); // 75 / 40
        }

        // AC-COM-004, AC-WT-005: accelerated Stage 2/3 timing and complete future-request ABI are exact.
        [Test]
        public void AcceleratedStageTwoAndThreeAndPayloadRequestFieldsAreExact()
        {
            AssertAcceleratedEnd(41, 190); // ceil(24 * 100 / 110) = 22
            AssertAcceleratedEnd(20, 188); // ceil(24 * 100 / 120) = 20
            var session = Session(); Step(session, 0); AdvanceThrough(session, 107); Step(session, 108, 60, PayloadEdge(108, "BossWeight-0", 0)); AdvanceThrough(session, 190);
            var output = Step(session, 191); var request = output.Intents[0].FutureDamageRequest.Value;
            Assert.That(request.RequestId, Is.EqualTo("ordan.payload/0000000000000000000/0000000192")); Assert.That(request.SourceId, Is.EqualTo("BossWeight-0"));
            Assert.That(request.TargetId, Is.EqualTo("ordan")); Assert.That(request.Amount, Is.EqualTo(8)); Assert.That(request.Kind, Is.EqualTo(DamageKind.BossPayloadTransfer)); Assert.That(request.Tick, Is.EqualTo(Tick(192)));
        }

        // AC-COM-002, AC-WT-005: a last Execute audit edge suppresses the final shockwave and retains its fresh revision.
        [Test]
        public void AuditLastExecuteEdgeInterruptsInsteadOfShockwave()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 544); var output = Step(session, 545, 60, AuditEdge(545, 0));
            Assert.That(output.Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Vulnerable)); Assert.That(output.Intents.Count, Is.EqualTo(1)); Assert.That(output.Intents[0].Kind, Is.EqualTo(OrdanBossIntentKind.BalanceAuditInterrupt));
        }

        // AC-COM-003, AC-WT-005: invalid transfer edges do not consume their revision; a legal retry at the same tick remains possible.
        [Test]
        public void WrongStaleAndDuplicateTransferEdgesAreAtomicAndDoNotConsumeRevision()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 107); var before = session.LatestOutput;
            Assert.Throws<InvalidOperationException>(() => Step(session, 108, 60, PayloadEdge(108, "BossWeight-1", 0)));
            Assert.That(session.LatestOutput, Is.EqualTo(before)); var accepted = Step(session, 108, 60, PayloadEdge(108, "BossWeight-0", 0)); AssertIntent(accepted, OrdanBossIntentKind.PayloadAcceleration, "ordan.payload-accelerate/0000000000000000000");
            Assert.Throws<InvalidOperationException>(() => Step(session, 109, 60, PayloadEdge(109, "BossWeight-0", 0))); Assert.That(session.NextExpectedTick.Value, Is.EqualTo(109));
            Assert.Throws<InvalidOperationException>(() => Step(session, 109, 60, AuditEdge(109, 1))); Assert.That(session.NextExpectedTick.Value, Is.EqualTo(109));
        }

        // AC-COM-003, AC-COM-004: every malformed pending payload result preserves the pending request and exact next tick.
        [Test]
        public void MissingWrongDuplicateAndMisalignedPayloadResultsAreAtomic()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 107); Step(session, 108, 60, PayloadEdge(108, "BossWeight-0", 0)); AdvanceThrough(session, 190); var request = Step(session, 191).Intents[0].FutureDamageRequest.Value;
            Assert.Throws<InvalidOperationException>(() => Step(session, 192, 52));
            Assert.Throws<InvalidOperationException>(() => Step(session, 192, 52, null, new DamageResult("ordan.payload/wrong", "ordan", 8, DamageResultCode.Applied, Tick(192))));
            Assert.Throws<InvalidOperationException>(() => Step(session, 192, 52, null, new DamageResult(request.RequestId, "ordan", 0, DamageResultCode.Duplicate, Tick(192))));
            Assert.Throws<InvalidOperationException>(() => Step(session, 192, 52, null, new DamageResult(request.RequestId, "ordan", 8, DamageResultCode.Applied, Tick(191))));
            Assert.That(session.NextExpectedTick.Value, Is.EqualTo(192)); Step(session, 192, 52, null, new DamageResult(request.RequestId, "ordan", 8, DamageResultCode.Applied, Tick(192)));
        }

        // AC-COM-003: repeated global ticks and checked encounter-local payload ordinal overflow never publish a partial state.
        [Test]
        public void RepeatedTickAndPayloadOrdinalOverflowAreAtomic()
        {
            var repeated = Session(); Step(repeated, 0); var output = repeated.LatestOutput; Assert.Throws<InvalidOperationException>(() => Step(repeated, 0)); Assert.That(repeated.LatestOutput, Is.EqualTo(output));
            var overflow = Session(); Step(overflow, 0); var field = typeof(OrdanBossSession).GetField("_state", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic); var state = (OrdanBossState)field.GetValue(overflow); state.PayloadOrdinal = long.MaxValue;
            AdvanceThrough(overflow, 106); var before = overflow.LatestOutput; Assert.Throws<OverflowException>(() => Step(overflow, 107)); Assert.That(overflow.LatestOutput, Is.EqualTo(before));
        }

        // AC-COM-003, AC-COM-004: M1's legal lethal clamp and TargetDead payload outcomes enter Defeated exactly once.
        [TestCase(DamageResultCode.Applied, 1)]
        [TestCase(DamageResultCode.Applied, 8)]
        [TestCase(DamageResultCode.TargetDead, 0)]
        public void PendingPayloadLethalResultsAreAcceptedAndDefeatIsOneShot(DamageResultCode code, int amount)
        {
            var session = PendingPayloadSession(out var request); var result = new DamageResult(request.RequestId, "ordan", amount, code, Tick(192));
            var dead = Step(session, 192, 0, null, result); Assert.That(dead.Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Defeated)); Assert.That(dead.Handoffs.Count, Is.EqualTo(3));
            Assert.That(Step(session, 193, 0).Handoffs.Count, Is.EqualTo(0));
        }

        // AC-COM-003: lethal payload results outside M1's permitted clamp never consume the pending result or publish defeat.
        [Test]
        public void PendingPayloadIllegalLethalAmountIsAtomic()
        {
            var session = PendingPayloadSession(out var request); var before = session.LatestOutput;
            Assert.Throws<InvalidOperationException>(() => Step(session, 192, 0, null, new DamageResult(request.RequestId, "ordan", 9, DamageResultCode.Applied, Tick(192))));
            Assert.That(session.LatestOutput, Is.EqualTo(before)); Assert.That(session.NextExpectedTick.Value, Is.EqualTo(192));
        }

        // AC-COM-003, AC-WT-005: global and per-handle revisions remain consumed, while a rejected stale edge permits a higher legal retry.
        [Test]
        public void ConsumedRevisionRejectsAtLaterTelegraphAndHigherRevisionRetriesLegally()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 107); Step(session, 108, 60, PayloadEdge(108, "BossWeight-0", 0));
            AdvanceThrough(session, 190); var request = Step(session, 191).Intents[0].FutureDamageRequest.Value; Step(session, 192, 52, null, new DamageResult(request.RequestId, "ordan", 8, DamageResultCode.Applied, Tick(192)));
            AdvanceThrough(session, 773, 52); var before = session.LatestOutput;
            Assert.Throws<InvalidOperationException>(() => Step(session, 774, 52, PayloadEdge(774, "BossWeight-1", 0)));
            Assert.That(session.LatestOutput, Is.EqualTo(before)); var retry = Step(session, 774, 52, PayloadEdge(774, "BossWeight-1", 1));
            AssertIntent(retry, OrdanBossIntentKind.PayloadAcceleration, "ordan.payload-accelerate/0000000000000000001");
        }

        // AC-COM-003, AC-WT-005: death suppresses both an otherwise legal transfer edge and the DebtLine Execute-age-zero hostile intent.
        [Test]
        public void SameTickDeathWinsOverTransferAndHostileIntent()
        {
            var payload = Session(); Step(payload, 0); AdvanceThrough(payload, 107); var payloadDeath = Step(payload, 108, 0, PayloadEdge(108, "BossWeight-0", 0));
            Assert.That(payloadDeath.Intents.Count, Is.EqualTo(0)); Assert.That(payloadDeath.Handoffs.Count, Is.EqualTo(3));
            var debt = Session(); Step(debt, 0); AdvanceThrough(debt, 47); var debtDeath = Step(debt, 48, 0);
            Assert.That(debtDeath.Intents.Count, Is.EqualTo(0)); Assert.That(debtDeath.Handoffs.Count, Is.EqualTo(3));
        }

        // AC-COM-004: every active vulnerability tick has no hostile output; only the exact exclusive end enters Recovery.
        [Test]
        public void EveryVulnerabilityTickSuppressesHostileIntentsUntilExclusiveEnd()
        {
            var session = AccelerateToVulnerability();
            for (var tick = 193; tick <= 311; tick++) { var output = Step(session, tick, 52); Assert.That(output.Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Vulnerable)); Assert.That(output.Intents.Count, Is.EqualTo(0)); }
            Assert.That(Step(session, 312, 52).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Recovery));
        }

        // AC-COM-002, AC-COM-004: reflection pins that M4A contains no basic-attack gate or damage-multiplier state.
        [Test]
        public void CoreHasNoBasicAttackGateOrDamageMultiplierMember()
        {
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
            foreach (var field in typeof(OrdanBossSession).GetFields(flags)) Assert.That(field.Name.IndexOf("BasicAttack", StringComparison.OrdinalIgnoreCase), Is.EqualTo(-1));
            foreach (var field in typeof(OrdanBossState).GetFields(flags)) { Assert.That(field.Name.IndexOf("BasicAttack", StringComparison.OrdinalIgnoreCase), Is.EqualTo(-1)); Assert.That(field.Name.IndexOf("Multiplier", StringComparison.OrdinalIgnoreCase), Is.EqualTo(-1)); }
        }

        // AC-COM-003: attack ordinal and payload ordinal overflow are independently checked before the next Telegraph is published.
        [Test]
        public void AttackAndPayloadOrdinalOverflowAreIndependentlyAtomic()
        {
            AssertOrdinalOverflow("AttackOrdinal"); AssertOrdinalOverflow("PayloadOrdinal");
        }

        // AC-M4B3B2-002. M4A alone publishes the exact Execute projection for
        // all three normal and all three accelerated frozen duration tables.
        [TestCase(60, false, 72)] [TestCase(41, false, 66)] [TestCase(20, false, 60)]
        [TestCase(60, true, 24)] [TestCase(41, true, 22)] [TestCase(20, true, 20)]
        public void SeizureProjectionMatchesEveryFrozenDuration(int health, bool accelerated, int duration)
        {
            var session = Session(); Step(session, 0);
            if (health != 60) Step(session, 1, health);
            AdvanceThrough(session, 107, health);
            var telegraph = Step(session, 108, health, accelerated ? PayloadEdge(108, "BossWeight-0", 0) : null);
            Assert.That(telegraph.SeizureTrajectoryProjection.HasValue, Is.False);
            AdvanceThrough(session, 167, health);
            var first = Step(session, 168, health);
            AssertProjection(first, 168, 0, duration, accelerated);
            AdvanceThrough(session, checked(168 + duration - 1), health);
            AssertProjection(session.LatestOutput, checked(168 + duration - 1), duration - 1, duration, accelerated);
            if (!accelerated)
                Assert.That(Step(session, checked(168 + duration), health).SeizureTrajectoryProjection.HasValue, Is.False);
        }

        // AC-M4B3B2-002. A caller cannot replace the preview's otherwise
        // well-formed trajectory duration before commit.
        [Test]
        public void ForgedSeizureProjectionCandidateRejectsWithoutMutation()
        {
            var session = Session(); Step(session, 0); AdvanceThrough(session, 167);
            var input = Input(168);
            var exact = session.PreviewNext(input);
            var output = exact.Output;
            var forgedProjection = new SeizureTrajectoryProjection(Tick(168), "BossWeight-0", 1, 0, 66, false);
            var before = session.LatestOutput;
            Assert.Throws<ArgumentException>(() => new OrdanBossTickOutput(output.Snapshot, output.Intents, output.Handoffs,
                output.TransferExposureForecast, output.AuditExposureForecast, forgedProjection));
            Assert.That(session.LatestOutput, Is.EqualTo(before));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(168)));
        }

        // AC-M4B3B3-001. M4A alone projects every committed BalanceAudit
        // Execute age with the exact frozen Stage 1/2/3 duration.
        [TestCase(60, 456, 90)]
        [TestCase(41, 442, 82)]
        [TestCase(20, 427, 75)]
        public void BalanceAuditPullProjectionMatchesEveryFrozenDuration(
            int health,
            int executeStart,
            int duration)
        {
            var session = Session();
            Step(session, 0);
            if (health != 60) Step(session, 1, health);
            AdvanceThrough(session, executeStart - 1, health);

            var first = Step(session, executeStart, health);
            AssertAuditPullProjection(first, executeStart, 0, duration);

            AdvanceThrough(session, checked(executeStart + duration - 1), health);
            AssertAuditPullProjection(
                session.LatestOutput,
                checked(executeStart + duration - 1),
                duration - 1,
                duration);

            Assert.That(
                Step(session, checked(executeStart + duration), health).BalanceAuditPullProjection.HasValue,
                Is.False);
        }

        // AC-M4B3B3-001, AC-M4B3B3-007. The M4A trace may retain the
        // age-zero Pull intent, but a same-tick interrupt has no active pull projection.
        [Test]
        public void BalanceAuditAgeZeroInterruptSuppressesPullProjection()
        {
            var session = Session();
            Step(session, 0);
            AdvanceThrough(session, 455);

            var output = Step(session, 456, 60, AuditEdge(456, 0));

            Assert.That(output.Intents.Count, Is.EqualTo(2));
            Assert.That(output.Intents[0].Kind, Is.EqualTo(OrdanBossIntentKind.BalanceAuditPull));
            Assert.That(output.Intents[1].Kind, Is.EqualTo(OrdanBossIntentKind.BalanceAuditInterrupt));
            Assert.That(output.BalanceAuditPullProjection.HasValue, Is.False);
        }

        // AC-M4B3B3-001, AC-M4B3B3-003. Projection identity participates
        // in preview/commit equality, so a well-shaped forged ordinal is atomic.
        [Test]
        public void ForgedBalanceAuditPullProjectionRejectsWithoutMutation()
        {
            var session = Session();
            Step(session, 0);
            AdvanceThrough(session, 455);
            var input = Input(456);
            var exact = session.PreviewNext(input);
            var output = exact.Output;
            var projection = output.BalanceAuditPullProjection.Value;
            var forgedOutput = new OrdanBossTickOutput(
                output.Snapshot,
                output.Intents,
                output.Handoffs,
                output.TransferExposureForecast,
                output.AuditExposureForecast,
                output.SeizureTrajectoryProjection,
                new BalanceAuditPullProjection(
                    projection.SourceTick,
                    checked(projection.AttackOrdinal + 1),
                    projection.ElapsedTick,
                    projection.FrozenDuration));
            var before = session.LatestOutput;

            Assert.Throws<InvalidOperationException>(() =>
                session.CommitNext(new OrdanBossTickCandidate(forgedOutput), input));
            Assert.That(session.LatestOutput, Is.EqualTo(before));
            Assert.That(session.NextExpectedTick, Is.EqualTo(Tick(456)));
        }

        private static OrdanBossSession Session() => new OrdanBossSession(Tick(0));

        private static void AssertAuditPullProjection(
            OrdanBossTickOutput output,
            int tick,
            int elapsed,
            int duration)
        {
            Assert.That(output.BalanceAuditPullProjection.HasValue, Is.True);
            var projection = output.BalanceAuditPullProjection.Value;
            Assert.That(projection.SourceTick, Is.EqualTo(Tick(tick)));
            Assert.That(projection.AttackOrdinal, Is.EqualTo(3));
            Assert.That(projection.ElapsedTick, Is.EqualTo(elapsed));
            Assert.That(projection.FrozenDuration, Is.EqualTo(duration));
        }

        private static void AssertProjection(OrdanBossTickOutput output, int tick, int elapsed, int duration, bool accelerated)
        {
            Assert.That(output.SeizureTrajectoryProjection.HasValue, Is.True);
            var projection = output.SeizureTrajectoryProjection.Value;
            Assert.That(projection.SourceTick, Is.EqualTo(Tick(tick)));
            Assert.That(projection.PayloadId, Is.EqualTo("BossWeight-0"));
            Assert.That(projection.AttackOrdinal, Is.EqualTo(1));
            Assert.That(projection.ElapsedTick, Is.EqualTo(elapsed));
            Assert.That(projection.FrozenDuration, Is.EqualTo(duration));
            Assert.That(projection.Accelerated, Is.EqualTo(accelerated));
        }
        private static OrdanBossSession PendingPayloadSession(out DamageRequest request) { var session = Session(); Step(session, 0); AdvanceThrough(session, 107); Step(session, 108, 60, PayloadEdge(108, "BossWeight-0", 0)); AdvanceThrough(session, 190); request = Step(session, 191).Intents[0].FutureDamageRequest.Value; return session; }
        private static void AssertOrdinalOverflow(string fieldName) { var session = Session(); Step(session, 0); var stateField = typeof(OrdanBossSession).GetField("_state", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic); var state = (OrdanBossState)stateField.GetValue(session); typeof(OrdanBossState).GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(state, long.MaxValue); AdvanceThrough(session, 106); var before = session.LatestOutput; Assert.Throws<OverflowException>(() => Step(session, 107)); Assert.That(session.LatestOutput, Is.EqualTo(before)); Assert.That(session.NextExpectedTick.Value, Is.EqualTo(107)); }
        private static OrdanBossSession EnterSeizure(int health) { var session = Session(); Step(session, 0); if (health != 60) Step(session, 1, health); AdvanceThrough(session, 107, health); Step(session, 108, health); AdvanceThrough(session, 167, health); Step(session, 168, health); return session; }
        private static void AssertPhaseEnd(OrdanBossSession session, int executeStart, int exclusiveEnd, OrdanBossPhase nextPhase) { Assert.That(session.LatestOutput.Snapshot.Tick.Value, Is.EqualTo(executeStart)); AdvanceThrough(session, exclusiveEnd - 1, session.LatestOutput.Snapshot.ActiveStage == OrdanHealthStage.Stage1 ? 60 : session.LatestOutput.Snapshot.ActiveStage == OrdanHealthStage.Stage2 ? 41 : 20); Assert.That(Step(session, exclusiveEnd, session.LatestOutput.Snapshot.ActiveStage == OrdanHealthStage.Stage1 ? 60 : session.LatestOutput.Snapshot.ActiveStage == OrdanHealthStage.Stage2 ? 41 : 20).Snapshot.Phase, Is.EqualTo(nextPhase)); }
        private static void AssertAcceleratedEnd(int health, int exclusiveEnd) { var session = Session(); Step(session, 0); if (health != 60) Step(session, 1, health); AdvanceThrough(session, 107, health); Step(session, 108, health, PayloadEdge(108, "BossWeight-0", 0)); AdvanceThrough(session, 167, health); Step(session, 168, health); AdvanceThrough(session, exclusiveEnd - 1, health); var request = session.LatestOutput.Intents[0].FutureDamageRequest.Value; Assert.That(Step(session, exclusiveEnd, health, null, new DamageResult(request.RequestId, "ordan", 8, DamageResultCode.Applied, Tick(exclusiveEnd))).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Vulnerable)); }
        private static void AssertAuditEnds(int health, int executeStart, int executeEnd, int recoveryEnd) { var session = Session(); Step(session, 0); if (health != 60) Step(session, 1, health); AdvanceThrough(session, executeStart - 1, health); Assert.That(Step(session, executeStart, health).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Execute)); AdvanceThrough(session, executeEnd - 1, health); Assert.That(Step(session, executeEnd, health).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Recovery)); AdvanceThrough(session, recoveryEnd - 1, health); Assert.That(Step(session, recoveryEnd, health).Snapshot.Phase, Is.EqualTo(OrdanBossPhase.Telegraph)); }
        private static OrdanBossTickOutput Step(OrdanBossSession session, int tick, int health = 60, OrdanRelevantTransferEdge? edge = null, DamageResult? result = null) => session.Step(Input(tick, health, edge, result));
        private static OrdanBossTickInput Input(int tick, int health = 60, OrdanRelevantTransferEdge? edge = null, DamageResult? result = null) => new OrdanBossTickInput(Tick(tick), new OrdanCombatState(Tick(tick), "ordan", health, health == 0, result), edge);
        private static void AdvanceThrough(OrdanBossSession session, int inclusiveTick, int health = 60) { for (var tick = session.NextExpectedTick.Value; tick <= inclusiveTick; tick++) Step(session, tick, health); }
        private static OrdanRelevantTransferEdge PayloadEdge(int tick, string id, int revision) => new OrdanRelevantTransferEdge(Tick(tick), id, OrdanTransferTargetKind.BossPayload, OrdanTransferState.Baseline, OrdanTransferState.Heavy, revision);
        private static OrdanRelevantTransferEdge AuditEdge(int tick, int revision) => new OrdanRelevantTransferEdge(Tick(tick), "BossAuditBox-0", OrdanTransferTargetKind.Box, OrdanTransferState.Baseline, OrdanTransferState.Heavy, revision);
        private static OrdanBossSession AccelerateToVulnerability() { var session = Session(); Step(session, 0); AdvanceThrough(session, 107); Step(session, 108, 60, PayloadEdge(108, "BossWeight-0", 0)); AdvanceThrough(session, 190); var request = Step(session, 191); Step(session, 192, 52, null, new DamageResult(request.Intents[0].Id, "ordan", 8, DamageResultCode.Applied, Tick(192))); return session; }
        private static void AssertIntent(OrdanBossTickOutput output, OrdanBossIntentKind kind, string id) { Assert.That(output.Intents.Count, Is.EqualTo(1)); Assert.That(output.Intents[0].Kind, Is.EqualTo(kind)); Assert.That(output.Intents[0].Id, Is.EqualTo(id)); }
        private static SimulationTick Tick(int value) => new SimulationTick(value);
        private static string Trace() { var session = Session(); var values = new List<string>(); for (var tick = 0; tick < 109; tick++) { var output = Step(session, tick); values.Add(output.Snapshot.PatternSlot + "/" + output.Snapshot.Phase + "/" + output.Snapshot.PhaseAge + "/" + output.Snapshot.ActiveHandleId + "/" + output.Intents.Count); } return string.Join(";", values); }
    }
}
