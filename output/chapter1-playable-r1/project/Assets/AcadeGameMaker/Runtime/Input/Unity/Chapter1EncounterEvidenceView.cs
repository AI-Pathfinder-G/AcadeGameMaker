using System;
using AcadeGameMaker.Core;
using UnityEngine;
namespace AcadeGameMaker.Input.Unity
{
    // REQ-C1B-EVIDENCE-001: three completed producer snapshots, never a fake failure batch.
    internal readonly struct Chapter1EncounterEvidenceView
    {
        internal Chapter1EncounterEvidenceView(Chapter1EncounterEntry entry,InputFrameCommitReceipt receipt,Vector2 position,int health,int invulnerabilityEndTick,string transferTarget,bool lightweight)
        {
            if(receipt.Tick.Value<entry.FirstTick.Value || health<0 || health>5)throw new ArgumentException("Completed encounter evidence is malformed.");
            PlayerBody=default;AliveEnemyCount=0;HealthPublication=null;HealthProducer=null;HealthProducerToken=null;
            HasCompleted=true;RunKey=entry.RunKey;Generation=entry.Generation;Receipt=receipt;Position=position;
            Health=health;InvulnerabilityEndTick=invulnerabilityEndTick;TransferTarget=transferTarget??string.Empty;Lightweight=lightweight;
        }
        internal Chapter1EncounterEvidenceView(Chapter1EncounterEvidenceView completed,Rect body,int alive,
            Chapter1FailurePublication health,object producer,object token)
        {
            if(!completed.HasCompleted || alive<0 || health==null || producer==null || token==null)throw new ArgumentException();
            HasCompleted=completed.HasCompleted;RunKey=completed.RunKey;Generation=completed.Generation;Receipt=completed.Receipt;
            Position=completed.Position;Health=completed.Health;InvulnerabilityEndTick=completed.InvulnerabilityEndTick;
            TransferTarget=completed.TransferTarget;Lightweight=completed.Lightweight;PlayerBody=body;AliveEnemyCount=alive;
            HealthPublication=health;HealthProducer=producer;HealthProducerToken=token;
        }
        internal Rect PlayerBody {get;}
        internal int AliveEnemyCount {get;}
        internal Chapter1FailurePublication HealthPublication {get;}
        internal object HealthProducer {get;}
        internal object HealthProducerToken {get;}
        internal bool HasCompleted {get;}
        internal string RunKey {get;}
        internal long Generation {get;}
        internal InputFrameCommitReceipt Receipt {get;}
        internal SimulationTick Tick => Receipt.Tick;
        internal Vector2 Position {get;}
        internal int Health {get;}
        internal int InvulnerabilityEndTick {get;}
        internal string TransferTarget {get;}
        internal bool Lightweight {get;}
    }
}
