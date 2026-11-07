using System;
using System.Collections;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Run.Unity;
using AcadeGameMaker.Tests.PlayMode.RunUnity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace AcadeGameMaker.Run.Unity.PlayMode.Tests
{
    // REQ-C1C-006: 정상 입력 경로와 명시 격리 물리/오류 시험의 증거를 구분한다.
    internal sealed class Chapter1ChoiceBarrierPlayModeTests : InputTestFixture
    {
        private Chapter1AuthoredInputTrace _trace;
        [SetUp] public void IsolateProfile()=>DesktopProfileLaunchAdapterV1.ConfigureChapter1TestRun("barrier-"+Guid.NewGuid().ToString("N"));
        public override void TearDown(){Chapter1AuthoredInputTrace.LogStage("postboss.fixture-restore-deferred");}
        [UnityTearDown] public IEnumerator Close(){try{if(_trace!=null)yield return _trace.Close();}finally{_trace=null;Chapter1AuthoredInputTrace.LogStage("postboss.fixture-restore-before");base.TearDown();Chapter1AuthoredInputTrace.LogStage("postboss.fixture-restore-after");}}
        private IEnumerator Open(int branch){_trace=new Chapter1AuthoredInputTrace();yield return _trace.OpenBarrier(branch);}
        [UnityTest] public IEnumerator AuthoredExtractionOneValidSkillPasses()
        {
            yield return Open(1);yield return _trace.UseBarrierSkill();var graph=_trace.BarrierGraph;
            var environment=graph.GetComponentInChildren<Chapter1ChoiceEnvironmentOwner>(true);var bridge=graph.transform.Find("Bridge").GetComponent<BoxCollider2D>();
            for(var i=0;i<60 && !environment.GateOpened;i++)yield return new WaitForFixedUpdate();
            Assert.That(environment.GateOpened,Is.True);Assert.That(bridge.enabled,Is.True);Assert.That(_trace.Flow.CurrentChapter1CompletionReceipt,Is.Null);
            var driver=graph.GetComponentInChildren<BarrierCombatSimulationDriver>(true);Assert.That(driver.LatestPublication,Is.Not.Null);
            foreach(var combatant in driver.LatestPublication.Combatants)
            {if(combatant.TargetId=="BarrierCounterweight")Assert.That(combatant.CurrentHealth,Is.EqualTo(4));if(combatant.TargetId=="BarrierGate-Fractured")Assert.That(combatant.CurrentHealth,Is.Zero);}
            yield return _trace.PassBarrier();Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Closing));
        }
        [UnityTest] public IEnumerator AuthoredSolidarityOneValidSkillPasses()
        {
            yield return Open(2);yield return _trace.UseBarrierSkill();var graph=_trace.BarrierGraph;var environment=graph.GetComponentInChildren<Chapter1ChoiceEnvironmentOwner>(true);
            Assert.That(graph.transform.Find("Platform").GetComponent<BoxCollider2D>().enabled,Is.True);Assert.That(environment.HasActualSkill,Is.True);
            var driver=graph.GetComponentInChildren<BarrierCombatSimulationDriver>(true);Assert.That(driver.LatestPublication.Combatants.Count,Is.EqualTo(2));
            // AC-AUTOCH1-SUPPORT-001: actual kinematic/static support, without enabling engine contacts.
            var player=_trace.Flow.Lifetime.CurrentRouter.BoundPlayerForAuthoring;var platform=graph.transform.Find("Platform").GetComponent<BoxCollider2D>();
            Assert.That(player.GetComponent<Rigidbody2D>().bodyType,Is.EqualTo(RigidbodyType2D.Kinematic));Assert.That(player.GetComponent<Rigidbody2D>().useFullKinematicContacts,Is.False);Assert.That(platform.attachedRigidbody,Is.Null);
            var gateObserved=false;
            for(var i=0;i<90;i++)
            {
                Assert.That(_trace.Flow.Lifetime.TryGetCompletedEvidence(out var completed),Is.True);
                if(environment.GateOpened){Assert.That(player.TryReadCompletedSupport(completed.Tick,platform,completed.Receipt),Is.True);gateObserved=true;_trace.Keyboard();break;}
                _trace.Keyboard(Key.D);_trace.Pointer(Vector2.zero,false);yield return new WaitForFixedUpdate();
            }
            Assert.That(gateObserved,Is.True,"현재 완료 영수증의 정확 발판 지지로 문이 열리지 않았습니다.");
            yield return _trace.PassBarrier();Assert.That(_trace.Flow.RunEndCommittedRevision,Is.EqualTo(1));Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.Closing));
        }
        // AC-AUTOCH1-SUPPORT-001: rejection observations do not replace a successful playthrough.
        [UnityTest] public IEnumerator RejectStaleForeignOrExpiredCompletedSupportBeforeGateOpen()
        {
            yield return Open(2);var graph=_trace.BarrierGraph;var owner=_trace.Flow.Lifetime;var router=owner.CurrentRouter;
            var environment=graph.GetComponentInChildren<Chapter1ChoiceEnvironmentOwner>(true);var player=router.BoundPlayerForAuthoring;
            var platform=graph.transform.Find("Platform").GetComponent<BoxCollider2D>();var floor=graph.transform.Find("LeftFloor").GetComponent<BoxCollider2D>();
            Assert.That(owner.TryGetCompletedEvidence(out var before),Is.True);Assert.That(platform.enabled,Is.False);
            Assert.That(player.TryReadCompletedSupport(before.Tick,platform,before.Receipt),Is.False);
            environment.ObserveCompletedPlayerContact(before.Tick,new object(),before.Receipt);Assert.That(environment.GateOpened,Is.False);
            yield return _trace.TransferToCounterweight();for(var i=0;i<24;i++)yield return new WaitForFixedUpdate();
            // Actual input makes the newly created plane remain below an ascending capsule, then lands on the left floor.
            _trace.Keyboard(Key.Q,Key.Space,Key.A);yield return new WaitForFixedUpdate();
            for(var i=0;i<9;i++){_trace.Keyboard(Key.A,Key.Space);yield return new WaitForFixedUpdate();}
            for(var i=0;i<10;i++){_trace.Keyboard(Key.D,Key.Space);yield return new WaitForFixedUpdate();}
            _trace.Keyboard();_trace.Pointer(Vector2.zero,false);Assert.That(environment.HasActualSkill,Is.True);Assert.That(environment.GateOpened,Is.False);
            var floorObserved=false;var old=before;
            for(var i=0;i<160;i++)
            {
                yield return new WaitForFixedUpdate();Assert.That(owner.CurrentRouter,Is.SameAs(router));Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.AtBarrier));Assert.That(environment.GateOpened,Is.False);
                Assert.That(owner.TryGetCompletedEvidence(out var actual),Is.True);
                Assert.That(player.TryReadCompletedSupport(old.Tick,platform,old.Receipt),Is.False);
                environment.ObserveCompletedPlayerContact(old.Tick,new object(),old.Receipt);Assert.That(environment.GateOpened,Is.False);
                if(!floorObserved && platform.enabled && player.TryReadCompletedSupport(actual.Tick,floor,actual.Receipt))
                {
                    floorObserved=true;Assert.That(player.TryReadCompletedSupport(actual.Tick,platform,actual.Receipt),Is.False,"다른 collider를 발판 지지로 대체했습니다.");
                    object copiedView=actual;environment.ObserveCompletedPlayerContact(actual.Tick,copiedView,actual.Receipt);Assert.That(environment.GateOpened,Is.False);
                    var wrong=new AcadeGameMaker.Core.InputFrameCommitReceipt(actual.Tick,actual.Receipt.FrameOrdinal+1,actual.Receipt.ModeEpoch,actual.Receipt.Mode);
                    Assert.That(player.TryReadCompletedSupport(actual.Tick,floor,wrong),Is.False);
                    environment.ObserveCompletedPlayerContact(actual.Tick,copiedView,wrong);Assert.That(environment.GateOpened,Is.False);
                }
                old=actual;
            }
            Assert.That(floorObserved,Is.True,"정상 입력으로 왼쪽 바닥의 원본 지지에 착지하지 못했습니다.");
            Assert.That(platform.enabled,Is.False);Assert.That(environment.GateOpened,Is.False);Assert.That(owner.TryGetCompletedEvidence(out var expired),Is.True);
            Assert.That(player.TryReadCompletedSupport(expired.Tick,platform,expired.Receipt),Is.False);
            environment.ObserveCompletedPlayerContact(expired.Tick,new object(),expired.Receipt);Assert.That(environment.GateOpened,Is.False);
        }
        [UnityTest] public IEnumerator CounterweightTransferRecallChangesPhysics()
        {
            yield return Open(1);var graph=_trace.BarrierGraph;var weight=graph.transform.Find("Counterweight");var body=weight.GetComponent<Rigidbody2D>();var box=weight.GetComponent<BoxCollider2D>();
            Assert.That(body.bodyType,Is.EqualTo(RigidbodyType2D.Dynamic));Assert.That(body.constraints,Is.EqualTo(RigidbodyConstraints2D.FreezeRotation));
            Assert.That(body.mass,Is.EqualTo(1));Assert.That(body.gravityScale,Is.EqualTo(1));Assert.That(box.size,Is.EqualTo(Vector2.one));Assert.That(box.offset,Is.EqualTo(Vector2.zero));
            // 승인된 Y1.5 방출은 격리 물리 비교이며 정상 완주의 위치 입력으로 기록하지 않는다.
            body.position=new Vector2(4.5f,1.5f);body.linearVelocity=Vector2.zero;
            for(var i=0;i<12;i++)yield return new WaitForFixedUpdate();var baseFall=1.5f-body.position.y;
            yield return _trace.TransferToCounterweight();Assert.That(body.mass,Is.EqualTo(3));Assert.That(body.gravityScale,Is.EqualTo(2));
            body.position=new Vector2(4.5f,1.5f);body.linearVelocity=Vector2.zero;
            for(var i=0;i<12;i++)yield return new WaitForFixedUpdate();var transferredFall=1.5f-body.position.y;Assert.That(transferredFall,Is.GreaterThan(baseFall));
            for(var i=0;i<12;i++)yield return new WaitForFixedUpdate();
            var camera=graph.GetComponentInChildren<UnityEngine.Camera>(true);_trace.Pointer(camera.WorldToScreenPoint(weight.position),false,true);yield return new WaitForFixedUpdate();_trace.Pointer(Vector2.zero,false);
            for(var i=0;i<30 && body.mass!=1;i++)yield return new WaitForFixedUpdate();Assert.That(body.mass,Is.EqualTo(1));Assert.That(body.gravityScale,Is.EqualTo(1));
            body.position=new Vector2(4.5f,1.5f);body.linearVelocity=Vector2.zero;
            for(var i=0;i<12;i++)yield return new WaitForFixedUpdate();Assert.That(1.5f-body.position.y,Is.EqualTo(baseFall).Within(.03f));
            Assert.That(_trace.Flow.RunEndCommittedRevision,Is.Zero);
        }
        [UnityTest] public IEnumerator RadiusBoundaryAndDamageRoster()
        {
            Assert.That(Chapter1ChoiceEnvironmentOwner.IsWithinCompressionRadiusQ1000(0,0,1499,0),Is.True);
            Assert.That(Chapter1ChoiceEnvironmentOwner.IsWithinCompressionRadiusQ1000(0,0,1500,0),Is.True);
            Assert.That(Chapter1ChoiceEnvironmentOwner.IsWithinCompressionRadiusQ1000(0,0,1501,0),Is.False);
            Assert.Throws<OverflowException>(()=>Chapter1ChoiceEnvironmentOwner.IsWithinCompressionRadiusQ1000(int.MinValue,int.MinValue,int.MaxValue,int.MaxValue));
            yield return Open(1);yield return _trace.UseBarrierSkill();var driver=_trace.BarrierGraph.GetComponentInChildren<BarrierCombatSimulationDriver>(true);
            var requestCount=0;var centerDamage=0;var gateDamage=0;
            for(var i=0;i<45;i++)
            {
                yield return new WaitForFixedUpdate();var publication=driver.LatestPublication;Assert.That(publication,Is.Not.Null);
                foreach(var request in publication.Requests)
                {requestCount++;Assert.That(request.SourceId,Is.EqualTo("BarrierCounterweight"));Assert.That(request.TargetId,Is.Not.EqualTo("player"));
                    if(request.TargetId=="BarrierCounterweight")centerDamage+=request.Amount;else if(request.TargetId=="BarrierGate-Fractured")gateDamage+=request.Amount;else Assert.Fail("승인 roster 밖 피해가 발급됐습니다.");}
            }
            Assert.That(requestCount,Is.EqualTo(2));Assert.That(centerDamage,Is.EqualTo(4));Assert.That(gateDamage,Is.EqualTo(2));
            Assert.That(driver.LatestPublication.Combatants.Count,Is.EqualTo(3));
            var copy=driver.LatestPublication;Assert.That(driver.ValidatePublished(copy),Is.True);yield return new WaitForFixedUpdate();Assert.That(driver.ValidatePublished(copy),Is.False);
        }
        [UnityTest] public IEnumerator NonlethalFallReturnsFreshGraphWithoutRunFailure()
        {
            yield return Open(1);Assert.That(_trace.Flow.Lifetime.TryGetCompletedEvidence(out var before),Is.True);var old=_trace.Flow.Lifetime.CurrentRouter;
            for(var i=0;i<600;i++)
            {
                if(_trace.Flow.Lifetime.CurrentRouter!=old && _trace.Flow.View.Phase==Chapter1RunPhase.AtBarrier)
                {
                    Assert.That(_trace.Flow.Lifetime.TryGetCompletedEvidence(out var after),Is.True);Assert.That(after.Generation,Is.GreaterThan(before.Generation));
                    Assert.That(after.Health,Is.EqualTo(before.Health));Assert.That(after.InvulnerabilityEndTick,Is.EqualTo(before.InvulnerabilityEndTick));
                    Assert.That(after.RunKey,Is.EqualTo(before.RunKey));Assert.That(after.Position.x,Is.EqualTo(0).Within(.05f));
                    Assert.That(_trace.Flow.RunEndCommittedRevision,Is.Zero);Assert.That(_trace.Flow.AcceptedFailureKind.HasValue,Is.False);_trace.Keyboard();yield break;
                }
                _trace.Keyboard(Key.D);_trace.Pointer(Vector2.zero,false);yield return new WaitForFixedUpdate();
                Assert.That(_trace.Flow.View.Phase,Is.Not.EqualTo(Chapter1RunPhase.Diagnostic),_trace.Flow.View.Error);
            }
            Assert.Fail("실제 틈 낙하에서 새 세대 비치명 복귀를 관측하지 못했습니다.");
        }
        private static readonly InputRouterFaultStage[] TerminalStages={InputRouterFaultStage.AfterTransferCommit,InputRouterFaultStage.MapSwitch};
        [UnityTest] public IEnumerator AtomicInputFailureAndSingleCapture([ValueSource(nameof(TerminalStages))] InputRouterFaultStage terminal)
        {
            yield return Open(1);var router=_trace.Flow.Lifetime.CurrentRouter;var driver=_trace.BarrierGraph.GetComponentInChildren<BarrierCombatSimulationDriver>(true);
            var prior=router.CurrentReceipt;var publication=driver.LatestPublication;
            // 기존 공개 시험 입구를 쓰는 격리 음성 사례다. 정상 입력 완주와 구별한다.
            foreach(var fault in new[]{InputRouterFaultStage.PrepareMovement,InputRouterFaultStage.PrepareTransfer})
            {router.SetFaultStageForTests(fault);Assert.Throws<InvalidOperationException>(()=>router.StepForTests());Assert.That(router.IsFaulted,Is.False);Assert.That(router.CurrentReceipt,Is.EqualTo(prior));Assert.That(driver.LatestPublication,Is.SameAs(publication));router.SetFaultStageForTests(null);}
            yield return new WaitForFixedUpdate();var next=driver.LatestPublication;Assert.That(next.Tick.Value,Is.EqualTo(publication.Tick.Value+1));
            Assert.That(next.Receipt,Is.EqualTo(router.CurrentReceipt.Value));Assert.That(next.Capture,Is.Not.SameAs(publication.Capture));Assert.That(driver.ValidatePublished(publication),Is.False);
            prior=router.CurrentReceipt;router.SetFaultStageForTests(terminal);Assert.Throws<InvalidOperationException>(()=>router.StepForTests());
            Assert.That(router.IsFaulted,Is.True);Assert.That(router.CurrentReceipt,Is.EqualTo(prior));Assert.That(driver.LatestPublication,Is.SameAs(next));Assert.That(driver.ValidatePublished(next),Is.False);
            // 오류 후 추가 생산 틱을 진행하지 않고 해당 장면을 닫는다.
            yield return _trace.Close();_trace=null;
        }
    }
}
