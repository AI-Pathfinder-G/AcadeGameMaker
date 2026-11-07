using System;
using AcadeGameMaker.Core;
using UnityEngine;
namespace AcadeGameMaker.Input.Unity
{
    // REQ-C1B-ENTRY-001/HP-001: issuer-bound value; no session or mutable gameplay authority.
    internal readonly struct Chapter1EncounterEntry
    {
        internal Chapter1EncounterEntry(Chapter1EncounterLifetimeOwner issuer,string runKey,long generation,SimulationTick firstTick,Vector2 position,int health,int invulnerabilityEndTick)
        {
            if(issuer==null || string.IsNullOrEmpty(runKey) || generation<1 || firstTick.Value<0 || firstTick.Value>int.MaxValue-600 ||
                health<1 || health>5 || invulnerabilityEndTick< -1 || float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsInfinity(position.x) || float.IsInfinity(position.y))
                throw new ArgumentException("Chapter entry is not finite, live, or bounded.");
            Issuer=issuer;RunKey=runKey;Generation=generation;FirstTick=firstTick;Position=position;Health=health;InvulnerabilityEndTick=invulnerabilityEndTick;
        }
        internal Chapter1EncounterLifetimeOwner Issuer { get; }
        internal string RunKey { get; }
        internal long Generation { get; }
        internal SimulationTick FirstTick { get; }
        internal Vector2 Position { get; }
        internal int Health { get; }
        internal int InvulnerabilityEndTick { get; }
        internal bool SameAs(Chapter1EncounterEntry other) => ReferenceEquals(Issuer,other.Issuer) && RunKey==other.RunKey && Generation==other.Generation &&
            FirstTick==other.FirstTick && Position==other.Position && Health==other.Health && InvulnerabilityEndTick==other.InvulnerabilityEndTick;
    }
    // Only an actual registered router can supply this read-only managed witness.
    internal sealed class Chapter1InputLifetime
    {
        private readonly Func<bool> _readClosed;
        private Chapter1InputLifetime(Func<bool> readClosed){_readClosed=readClosed;}
        internal static Chapter1InputLifetime FromRouter(InputRouter router)
        {
            if(router==null || !router.IsChapter1Regular)throw new InvalidOperationException("Chapter lifetime requires its regular router.");
            return new Chapter1InputLifetime(router.GetChapter1ClosureReader());
        }
        internal bool IsClosed => _readClosed();
    }
}
