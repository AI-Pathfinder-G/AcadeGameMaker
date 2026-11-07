using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Run;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Run
{
    // REQ-C1C-004/006, REQ-CHOICE-004/006/007. evidenceClass=synthetic.
    // 격리 객체로 만든 context는 실제 driver-issued receipt/capture가 아니다.
    // 실제 UI/저장/held callback은 ClosingPersistence, 환경 피해/접촉/회수는 ChoiceBarrier의 생산 입력 시험이 담당한다.
    public sealed class Chapter1ChoiceSkillSessionTests
    {
        [Test] public void PairConsentCancelAndSaveGate()
        {
            var run=new object();
            Assert.Throws<ArgumentNullException>(()=>new ChoiceSkillSession(null,1,4,1));
            foreach(var pair in new[]{new[]{1,3,1},new[]{2,4,2},new[]{1,4,2},new[]{2,3,1},new[]{0,0,0}})
                Assert.Throws<ArgumentException>(()=>new ChoiceSkillSession(run,pair[0],pair[1],pair[2]));
            foreach(var skill in new[]{1,2})
            {
                var session=Session(run,skill);var context=Context(run,21,1,skill);
                Assert.That(session.IsOwnedByRun(run),Is.True);Assert.That(session.IsOwnedByRun(new object()),Is.False);
                Assert.That(session.Prepare(null),Is.Null);AssertUnchanged(session,0,0,0);
                var candidate=session.Prepare(context);Assert.That(candidate,Is.Not.Null);Assert.That(session.Validate(candidate),Is.True);
                Assert.That(session.Prepare(context),Is.Null,"미소비 후보와 경쟁하는 두 번째 준비는 허용하지 않습니다.");
                AssertUnchanged(session,0,0,0);session.Cancel(null);Assert.That(session.Validate(candidate),Is.True);
                session.Cancel(candidate);Assert.That(session.Validate(candidate),Is.False);Assert.Throws<InvalidOperationException>(()=>session.Commit(candidate));AssertUnchanged(session,0,0,0);
                var fresh=session.Prepare(context);Assert.That(fresh,Is.Not.SameAs(candidate));Assert.That(session.Validate(fresh),Is.True);
                var result=session.Commit(fresh);Assert.That(result.Skill,Is.EqualTo(skill));Assert.That(result.SkillOrdinal,Is.EqualTo(1));
                AssertUnchanged(session,1,21+(skill==1?300:420),skill==1?42:0);
                Assert.That(session.Validate(fresh),Is.False);Assert.Throws<InvalidOperationException>(()=>session.Commit(fresh));
                session.Cancel(fresh);Assert.That(session.SkillOrdinal,Is.EqualTo(1));
                Assert.That(session.Prepare(Context(run,1000,1,skill)),Is.Null,"소비된 edge는 시간 경과로 재사용되지 않습니다.");
            }
            // 승인 pair 검증만 순수 세션의 저장 경계다. 실제 V1 성공/실패/불확실 저장은 이 fixture가 발급하지 않는다.
            var extraction=Session(run,1);Assert.That(extraction.Prepare(Context(run,30,1,2)),Is.Null);
            Assert.Throws<ArgumentException>(()=>Context(run,30,1,1,consent:3));AssertUnchanged(extraction,0,0,0);
        }
        [Test] public void RecallAge20_21_22AndCooldown299_300()
        {
            var run=new object();var session=Session(run,1);
            Assert.That(session.Prepare(Context(run,20,1,1)),Is.Null);AssertUnchanged(session,0,0,0);
            // held는 생산 입력층에서 context를 새로 발급하지 않는다. 여기서는 그 부재의 무소비만 검증한다.
            Assert.That(session.Prepare(null),Is.Null);AssertUnchanged(session,0,0,0);
            var age21=session.Prepare(Context(run,21,2,1));Assert.That(age21,Is.Not.Null);Assert.That(session.Validate(age21),Is.True);
            var first=session.Commit(age21);Assert.That(first.RecallLockEndTick.Value,Is.EqualTo(42));Assert.That(first.EffectTick.Value,Is.EqualTo(57));Assert.That(first.CooldownEndTick.Value,Is.EqualTo(321));
            Assert.That(21+20,Is.LessThan(first.RecallLockEndTick.Value));Assert.That(21+21,Is.EqualTo(first.RecallLockEndTick.Value));
            Assert.That(session.Prepare(Context(run,320,3,1)),Is.Null,"기술 이후 299틱은 재사용 대기 중입니다.");
            Assert.That(session.Prepare(Context(run,321,2,1)),Is.Null,"300틱이 되어도 같은 edge는 다시 소비하지 않습니다.");
            var ready=session.Prepare(Context(run,321,3,1));Assert.That(ready,Is.Not.Null);var second=session.Commit(ready);
            Assert.That(second.SkillOrdinal,Is.EqualTo(2));Assert.That(second.Tick.Value,Is.EqualTo(321));AssertUnchanged(session,2,621,342);
            var other=Session(run,1);var age22=other.Prepare(Context(run,22,1,1));Assert.That(age22,Is.Not.Null);
            other.Cancel(age22);AssertUnchanged(other,0,0,0);Assert.That(other.Validate(age22),Is.False);
            // 회수 후 20/21의 절대 잠금값과 긴 기술 cooldown을 혼동하지 않는다.
            Assert.That(session.Prepare(Context(run,341,4,1)),Is.Null);Assert.That(session.Prepare(Context(run,342,4,1)),Is.Null);
            Assert.Throws<ArgumentException>(()=>new ChoicePhaseResult(new SimulationTick(21),1,1,new SimulationTick(320),new SimulationTick(57),new SimulationTick(42),default(SimulationTick),default(SimulationTick)));
        }
        [Test] public void Solidarity419_420AndSameTickTransfer()
        {
            var run=new object();var session=Session(run,2);var context=Context(run,30,1,2,started:30);
            var driver=context.Driver;var receipt=context.GlobalReceipt;var capture=context.Capture;
            var candidate=session.Prepare(context);Assert.That(candidate,Is.Not.Null);AssertUnchanged(session,0,0,0);
            var result=session.Commit(candidate);Assert.That(result.CooldownEndTick.Value,Is.EqualTo(450));Assert.That(result.EffectTick.Value,Is.EqualTo(30));
            Assert.That(result.RecallLockEndTick.Value,Is.Zero);Assert.That(result.ConsumesTransferPressed,Is.True);
            Assert.That(context.Driver,Is.SameAs(driver));Assert.That(context.GlobalReceipt,Is.SameAs(receipt));Assert.That(context.Capture,Is.SameAs(capture));
            Assert.That(session.RecallLockEndTick,Is.Zero,"순수 연대 세션은 전이 회수 권위를 만들지 않습니다.");
            Assert.That(session.Prepare(Context(run,449,2,2)),Is.Null);Assert.That(session.Prepare(Context(run,450,1,2)),Is.Null);
            Assert.That(default(ChoicePhaseResult).ConsumesTransferPressed,Is.False,"실패한 준비에는 전달할 성공 결과가 없습니다.");
            var next=session.Prepare(Context(run,450,2,2));Assert.That(next,Is.Not.Null);session.Cancel(next);AssertUnchanged(session,1,450,0);
            Assert.That(session.Validate(next),Is.False);var fresh=session.Prepare(Context(run,450,2,2));var second=session.Commit(fresh);
            Assert.That(second.SkillOrdinal,Is.EqualTo(2));AssertUnchanged(session,2,870,0);
            // 같은 tick TransferPressed의 실제 소비/실패 시 일반 처리는 Transfer driver의 생산 fixture가 확인한다.
        }
        [Test] public void Reservation35_36_37Hold59_60Platform149_150()
        {
            var run=new object();var extraction=Session(run,1);var compression=extraction.Commit(extraction.Prepare(Context(run,100,1,1)));
            Assert.That(compression.EffectTick.Value,Is.EqualTo(136));
            Assert.That(100+35,Is.LessThan(compression.EffectTick.Value));Assert.That(100+36,Is.EqualTo(compression.EffectTick.Value));Assert.That(100+37,Is.GreaterThan(compression.EffectTick.Value));
            Assert.That(compression.HoldEndTick.Value,Is.Zero);Assert.That(compression.PlatformEndTick.Value,Is.Zero);
            var solidarity=Session(run,2);var reference=solidarity.Commit(solidarity.Prepare(Context(run,100,1,2)));
            Assert.That(reference.HoldEndTick.Value,Is.EqualTo(160));Assert.That(100+59,Is.LessThan(reference.HoldEndTick.Value));Assert.That(100+60,Is.EqualTo(reference.HoldEndTick.Value));
            Assert.That(reference.PlatformEndTick.Value,Is.EqualTo(250));Assert.That(100+149,Is.LessThan(reference.PlatformEndTick.Value));Assert.That(100+150,Is.EqualTo(reference.PlatformEndTick.Value));
            // 같은 run의 새 graph generation에서도 session cooldown은 보존된다. 실제 환경 예약의 정리는 환경 소유자 책임이다.
            Assert.That(solidarity.Prepare(Context(run,519,2,2,generation:2)),Is.Null);var afterGraph=solidarity.Prepare(Context(run,520,2,2,generation:2));
            Assert.That(afterGraph,Is.Not.Null);solidarity.Cancel(afterGraph);AssertUnchanged(solidarity,1,520,0);
            Assert.Throws<ArgumentException>(()=>new ChoicePhaseResult(new SimulationTick(100),1,1,new SimulationTick(400),new SimulationTick(135),new SimulationTick(121),default(SimulationTick),default(SimulationTick)));
            Assert.Throws<ArgumentException>(()=>new ChoicePhaseResult(new SimulationTick(100),1,2,new SimulationTick(520),new SimulationTick(100),default(SimulationTick),new SimulationTick(159),new SimulationTick(250)));
            Assert.Throws<ArgumentException>(()=>new ChoicePhaseResult(new SimulationTick(100),1,2,new SimulationTick(520),new SimulationTick(100),default(SimulationTick),new SimulationTick(160),new SimulationTick(249)));
        }
        [Test] public void RejectForeignCopiedStaleConsumedAndOverflow()
        {
            object run=new string(new[]{'r','u','n'});object equalRun=new string(new[]{'r','u','n'});Assert.That(run.Equals(equalRun),Is.True);Assert.That(ReferenceEquals(run,equalRun),Is.False);
            var session=Session(run,1);var foreign=Session(run,1);var context=Context(run,21,1,1);
            Assert.That(session.Prepare(Context(equalRun,21,1,1)),Is.Null);AssertUnchanged(session,0,0,0);
            var candidate=session.Prepare(context);Assert.That(candidate,Is.Not.Null);Assert.That(session.Validate(null),Is.False);
            var other=foreign.Prepare(context);Assert.That(session.Validate(other),Is.False);session.Cancel(other);Assert.That(session.Validate(candidate),Is.True);
            var copied=new ChoiceSkillCandidate(session,context,candidate.Result,candidate.PriorOrdinal,candidate.PriorEdge,candidate.PriorCooldown,candidate.PriorRecallLock);
            Assert.That(session.Validate(copied),Is.False);Assert.Throws<InvalidOperationException>(()=>session.Commit(copied));session.Cancel(copied);Assert.That(session.Validate(candidate),Is.True);
            session.Cancel(candidate);var fresh=session.Prepare(context);Assert.That(session.Validate(candidate),Is.False);Assert.Throws<InvalidOperationException>(()=>session.Commit(candidate));
            session.Commit(fresh);Assert.That(session.Validate(fresh),Is.False);Assert.Throws<InvalidOperationException>(()=>session.Commit(fresh));AssertUnchanged(session,1,321,42);
            var overflow=Session(run,1);Assert.That(overflow.Prepare(Context(run,int.MaxValue,1,1)),Is.Null);AssertUnchanged(overflow,0,0,0);
            Assert.That(overflow.Prepare(Context(run,21,1,1,revision:int.MaxValue)),Is.Null);AssertUnchanged(overflow,0,0,0);
            Assert.Throws<OverflowException>(()=>new ChoicePhaseResult(new SimulationTick(int.MaxValue),1,1,new SimulationTick(int.MaxValue),new SimulationTick(int.MaxValue),default(SimulationTick),default(SimulationTick),default(SimulationTick)));
            var maximumEdge=overflow.Prepare(Context(run,21,long.MaxValue,1));Assert.That(maximumEdge,Is.Not.Null);overflow.Commit(maximumEdge);
            Assert.That(overflow.Prepare(Context(run,321,long.MaxValue,1)),Is.Null);AssertUnchanged(overflow,1,321,42);
            Assert.Throws<ArgumentOutOfRangeException>(()=>Context(run,21,1,1,generation:0));Assert.Throws<ArgumentOutOfRangeException>(()=>Context(run,21,1,1,revision:-1));
            Assert.Throws<ArgumentOutOfRangeException>(()=>Context(run,21,0,1));Assert.Throws<ArgumentOutOfRangeException>(()=>Context(run,21,1,1,started:22));
            Assert.Throws<ArgumentNullException>(()=>Context(null,21,1,1));Assert.Throws<ArgumentException>(()=>Context(run,21,1,1,mode:InputMode.UIOnly));
            // driver/adapter/receipt/capture/옛세대의 생산 발급 검증은 그 발급자의 pending 원본 검사이며 이 순수 세션의 책임으로 위장하지 않는다.
        }
        private static ChoiceSkillSession Session(object run,int skill)=>new ChoiceSkillSession(run,skill,skill==1?4:3,skill);
        private static ChoicePhaseContext Context(object run,int tick,long edge,int skill,int started=0,long generation=1,int revision=1,int consent=0,InputMode mode=InputMode.GameplayEnabled)
            =>new ChoicePhaseContext(new object(),new object(),new object(),new object(),run,generation,new SimulationTick(tick),revision,
                "BarrierCounterweight",edge,skill,consent==0?(skill==1?4:3):consent,skill,mode,new SimulationTick(started));
        private static void AssertUnchanged(ChoiceSkillSession session,long ordinal,int cooldown,int recall)
        {Assert.That(session.SkillOrdinal,Is.EqualTo(ordinal));Assert.That(session.CooldownEndTick,Is.EqualTo(cooldown));Assert.That(session.RecallLockEndTick,Is.EqualTo(recall));}
    }
}
