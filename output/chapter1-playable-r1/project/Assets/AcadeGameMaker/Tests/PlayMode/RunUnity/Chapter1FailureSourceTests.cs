using System;
using System.Collections;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Run.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Tests.PlayMode.RunUnity
{
    internal sealed class Chapter1FailureSourceTests : InputTestFixture
    {
        private Chapter1AuthoredInputTrace _trace;
        [UnityTearDown] public IEnumerator Close(){if(_trace!=null)yield return _trace.Close();_trace=null;}
        [UnityTest] public IEnumerator HealthFromCompletedCombat()
        {
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();yield return _trace.Start(101);
            Assert.That(_trace.Flow.Lifetime.TryGetCompletedEvidence(out var view),Is.True);
            Assert.That(view.HealthPublication,Is.Not.Null);Assert.That(view.HealthPublication.Kind,Is.EqualTo(Chapter1FailureSourceKind.Health));
            Assert.That(view.HealthPublication.Issuer,Is.SameAs(view.HealthProducer));Assert.That(view.HealthPublication.Token,Is.SameAs(view.HealthProducerToken));
            Assert.That(view.HealthPublication.Receipt,Is.EqualTo(view.Receipt));Assert.That(view.HealthPublication.Triggered,Is.EqualTo(view.Health==0));
        }
        [Test] public void KillPlaneBoundary()
        {
            var definition=new Chapter1HazardDefinition("R01",true);
            Assert.That(definition.KillPlane(-8),Is.False);Assert.That(definition.KillPlane(-8.0001f),Is.True);
        }
        [Test] public void LethalCrushPhaseAndOverlap()
        {
            var definition=new Chapter1HazardDefinition("R05",true);var body=Rect.MinMaxRect(12.5f,.5f,13.5f,1.5f);
            Assert.That(definition.Crush(59,0,body),Is.False);Assert.That(definition.Crush(60,0,body),Is.True);
            Assert.That(definition.Crush(119,0,body),Is.True);Assert.That(definition.Crush(120,0,body),Is.False);
            Assert.That(definition.Crush(60,0,Rect.MinMaxRect(14,0,15,1)),Is.False);
            Assert.That(new Chapter1HazardDefinition("R04",true).Crush(60,0,body),Is.False);
            Assert.That(definition.IsClosed(80,20),Is.True);Assert.Throws<ArgumentException>(()=>new Chapter1HazardDefinition("R05",false));
        }
        [UnityTest] public IEnumerator RejectMissingOrForeignPublication()
        {
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();yield return _trace.Start(101);
            var owner=_trace.Flow.Lifetime;Assert.That(owner.TryGetCompletedEvidence(out var view),Is.True);
            var hazard=owner.CurrentHazards;Assert.That(hazard.TryGetCompleted(view,out var kill,out var crush),Is.True);
            var foreign=new Chapter1FailurePublication(new object(),new object(),owner.CurrentRouter,view.RunKey,view.Generation,view.Receipt,1,Chapter1FailureSourceKind.KillPlane,kill.Triggered);
            Assert.That(hazard.Owns(foreign),Is.False);Assert.That(hazard.Owns(kill),Is.True);Assert.That(hazard.Owns(crush),Is.True);
            Assert.That(hazard.TryGetCompleted(default,out _,out _),Is.False);
        }
        [UnityTest] public IEnumerator RejectCopiedOrForeignAcceptedFailureToken()
        {
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();yield return _trace.Start(101);yield return _trace.PlayUntilFailureAccepted();
            var owner=_trace.Flow.Lifetime;Assert.That(owner.TryGetCompletedEvidence(out var view),Is.True);
            Assert.That(owner.CurrentHazards.TryGetCompleted(view,out var kill,out var crush),Is.True);
            var copy=new Chapter1AcceptedFailureToken(_trace.Flow,owner,owner.CurrentRouter,view.RunKey,view.Generation,view.Receipt,
                _trace.Flow.FailureArbitrationRevision,Chapter1FailureSourceKind.Health,view.HealthPublication,kill,crush);
            Assert.That(owner.ValidateChapter1FailureSources(copy),Is.True); // 발행원 값은 실제지만 pending 객체는 다르다.
            Assert.That(_trace.Flow.ValidatePendingFailure(owner,copy),Is.False);Assert.That(owner.OwnsPendingChapter1Failure(copy),Is.False);
            Assert.Throws<InvalidOperationException>(()=>owner.RequestChapter1FailureTransition(_trace.Flow,copy));
            var foreignObject=new GameObject("ForeignFailureOwner");var foreign=foreignObject.AddComponent<Chapter1EncounterLifetimeOwner>();
            try
            {
                var token=new Chapter1AcceptedFailureToken(_trace.Flow,foreign,owner.CurrentRouter,view.RunKey,view.Generation,view.Receipt,
                    copy.ArbitrationRevision,copy.Cause,view.HealthPublication,kill,crush);
                Assert.That(owner.ValidateChapter1FailureSources(token),Is.False);Assert.That(_trace.Flow.ValidatePendingFailure(foreign,token),Is.False);
                var foreignPort=new ForeignFailurePort();var other=new Chapter1AcceptedFailureToken(foreignPort,owner,owner.CurrentRouter,view.RunKey,view.Generation,
                    view.Receipt,copy.ArbitrationRevision,copy.Cause,view.HealthPublication,kill,crush);
                Assert.That(owner.ValidateChapter1FailureSources(other),Is.False);
            }
            finally{UnityEngine.Object.DestroyImmediate(foreignObject);}
        }
        [UnityTest] public IEnumerator RejectStaleOrAllFalseFailureBatch()
        {
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();yield return _trace.Start(101);
            var owner=_trace.Flow.Lifetime;Assert.That(owner.TryGetCompletedEvidence(out var view),Is.True);
            Assert.That(owner.CurrentHazards.TryGetCompleted(view,out var kill,out var crush),Is.True);
            Assert.That(view.HealthPublication.Triggered || kill.Triggered || crush.Triggered,Is.False);
            var token=new Chapter1AcceptedFailureToken(_trace.Flow,owner,owner.CurrentRouter,view.RunKey,view.Generation,view.Receipt,
                _trace.Flow.FailureArbitrationRevision,Chapter1FailureSourceKind.Health,view.HealthPublication,kill,crush);
            Assert.That(owner.ValidateChapter1FailureSources(token),Is.False);
            var sources=new Chapter1FailureSources(owner.CurrentRouter,owner.CurrentHazards,view.RunKey,view.Generation);
            sources.CloseCompleted(view);Assert.Throws<InvalidOperationException>(()=>sources.CloseCompleted(view));
            yield return new WaitForFixedUpdate();Assert.That(owner.ValidateChapter1FailureSources(token),Is.False);
            Assert.That(owner.CurrentRouter.OwnsChapter1HealthPublication(view.HealthPublication),Is.False);
        }
        [UnityTest] public IEnumerator AcceptedFailureConsumedOnceBeforeTransition()
        {
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();yield return _trace.Start(101);yield return _trace.PlayUntilFailureAccepted();
            var owner=_trace.Flow.Lifetime;Assert.That(owner.TryGetCompletedEvidence(out var view),Is.True);
            Assert.That(view.Receipt.Mode,Is.EqualTo(AcadeGameMaker.Core.InputMode.GameplayEnabled));Assert.That(_trace.Flow.RunEndCommittedRevision,Is.Zero);
            Assert.That(owner.CurrentHazards.TryGetCompleted(view,out var kill,out var crush),Is.True);
            var token=new Chapter1AcceptedFailureToken(_trace.Flow,owner,owner.CurrentRouter,view.RunKey,view.Generation,view.Receipt,
                _trace.Flow.FailureArbitrationRevision,Chapter1FailureSourceKind.Health,view.HealthPublication,kill,crush);
            Assert.Throws<InvalidOperationException>(()=>_trace.Flow.ConsumePendingFailure(owner,token));
            yield return _trace.WaitReturned();Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));Assert.That(_trace.Flow.ReturnedToHubRevision,Is.EqualTo(1));
            Assert.Throws<InvalidOperationException>(()=>_trace.Flow.ConsumePendingFailure(owner,token));
        }
        [UnityTest] public IEnumerator SameTickFailurePrecedesBossTransition()
        {
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();yield return _trace.Start(101);yield return _trace.PlayToBoss();
            yield return _trace.PlayUntilFailureAccepted();var owner=_trace.Flow.Lifetime;
            Assert.That(owner.TryGetCompletedEvidence(out var view),Is.True);
            Assert.That(view.HealthPublication.Triggered,Is.True);Assert.That(view.Receipt.Mode,Is.EqualTo(AcadeGameMaker.Core.InputMode.GameplayEnabled));
            Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Failing));
            if(owner.TryGetChapter1BossPublication(out var death))
            {Assert.That(death.DeathTick,Is.EqualTo(view.Tick));Assert.That(death.PlayerDead,Is.True);Assert.That(_trace.Flow.BlocksNewFailureRequests,Is.False);}
            yield return _trace.WaitReturned();Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));
            // 실제 동시 사망 객체가 없는 실행은 보스 구간 health 실패 검사이며 동시 사망 AC로 집계하지 않는다.
        }
        [UnityTest] public IEnumerator SameTickFailurePrecedesBossDeath()
        {
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();yield return _trace.Start(101);yield return _trace.PlayToBoss();
            yield return _trace.PlayUntilBossAndPlayerDeath();var owner=_trace.Flow.Lifetime;
            Assert.That(owner.TryGetCompletedEvidence(out var view),Is.True);Assert.That(owner.TryGetChapter1BossPublication(out var death),Is.True);
            Assert.That(death.DeathTick,Is.EqualTo(view.Tick));Assert.That(death.BossDead && death.PlayerDead,Is.True);
            Assert.That(view.HealthPublication.Triggered,Is.True);Assert.That(_trace.Flow.BlocksNewFailureRequests,Is.False);
            Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Failing));
            yield return _trace.WaitReturned();Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));Assert.That(_trace.Flow.ReturnedToHubRevision,Is.EqualTo(1));
        }
        private sealed class ForeignFailurePort:IChapter1BossProgressionPort
        {
            public bool BlocksNewFailureRequests=>false;
            public bool ValidatePendingFailure(Chapter1EncounterLifetimeOwner consumer,Chapter1AcceptedFailureToken token)=>true;
            public void ConsumePendingFailure(Chapter1EncounterLifetimeOwner consumer,Chapter1AcceptedFailureToken token){}
        }
    }
}
