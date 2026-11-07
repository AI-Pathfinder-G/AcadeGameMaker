using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Combat;
using NUnit.Framework;
namespace AcadeGameMaker.Tests.EditMode.CombatUnity
{
    // AC-C1B-HP-001: synthetic engine-free boundary fixtures, not actual play evidence.
    internal sealed class Chapter1CarriedPlayerTests
    {
        [Test] public void PreservesHealthAndInvulnerability()
        {
            var session=new CombatSession(new SimulationTick(20));session.RegisterCarriedPlayer(new CombatantRegistration("player",5,45),3,40);
            var value=session.Snapshot("player");Assert.That(value.CurrentHealth,Is.EqualTo(3));Assert.That(value.InvulnerabilityEndTick,Is.EqualTo(40));
            var expired=new CombatSession(new SimulationTick(20));expired.RegisterCarriedPlayer(new CombatantRegistration("player",5,45),2,19);
            Assert.That(expired.Snapshot("player").InvulnerabilityEndTick,Is.EqualTo(-1));
        }
        [Test] public void RejectsInvalidOrLateCarry()
        {
            var session=new CombatSession(new SimulationTick(20));var registration=new CombatantRegistration("player",5,45);
            Assert.Throws<InvalidOperationException>(()=>session.RegisterCarriedPlayer(registration,0,40));
            Assert.Throws<InvalidOperationException>(()=>session.RegisterCarriedPlayer(registration,6,40));
            Assert.Throws<InvalidOperationException>(()=>session.RegisterCarriedPlayer(registration,3,-2));
            session.RegisterCarriedPlayer(registration,3,40);Assert.Throws<InvalidOperationException>(()=>session.RegisterCarriedPlayer(registration,3,40));
            var late=new CombatSession(new SimulationTick(20));late.Process(new SimulationTick(20),Array.Empty<DamageRequest>());
            Assert.Throws<InvalidOperationException>(()=>late.RegisterCarriedPlayer(registration,3,40));
            Assert.Throws<InvalidOperationException>(()=>new CombatSession(new SimulationTick(20)).RegisterCarriedPlayer(new CombatantRegistration("walker",9,0),3,40));
        }
        [Test] public void LegacyRegistrationUnchanged()
        {var session=new CombatSession(new SimulationTick(0));session.Register(new CombatantRegistration("player",5,45));Assert.That(session.Snapshot("player").CurrentHealth,Is.EqualTo(5));Assert.That(session.Snapshot("player").InvulnerabilityEndTick,Is.EqualTo(-1));}
    }
}