using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using UnityEngine;

namespace AcadeGameMaker.Transfer.Unity
{
    // REQ-AUTOCH1-009: production serialization of the existing reversible box modifier.
    [DisallowMultipleComponent]
    internal sealed class Chapter1CounterweightOwnerSink : MonoBehaviour,ITransferTargetModifierSink
    {
        [SerializeField] private string _targetId;
        [SerializeField] private Rigidbody2D _body;
        [SerializeField] private bool _configured;
        private bool _applied;
        private float _baseMass,_baseGravityScale;
        private int _impactDamageMultiplierQ1000=1000;
        public string BoundTargetId=>_targetId;
        internal int ImpactDamageMultiplierQ1000=>_impactDamageMultiplierQ1000;
        internal void ConfigureForAuthoring(string targetId,Rigidbody2D body)
        {
            if(gameObject.activeInHierarchy || _configured || targetId!="BarrierCounterweight" || body==null || body.gameObject!=gameObject)
                throw new InvalidOperationException("상자 수정자는 같은 비활성 저작 물체에 한 번 결속합니다.");
            _targetId=targetId;_body=body;_configured=true;
        }
        public bool IsStillAvailable()=>_configured&&_targetId=="BarrierCounterweight"&&_body!=null&&_body.gameObject==gameObject;
        public bool TryApply(TransferModifierRequest request)
        {
            if(_applied || !IsStillAvailable() || request.TargetId!=_targetId || request.TargetModifierId!="Target.Box.Heavy.v1")return false;
            _baseMass=_body.mass;_baseGravityScale=_body.gravityScale;
            _body.mass=_baseMass*3f;_body.gravityScale=_baseGravityScale*2f;_impactDamageMultiplierQ1000=1500;_applied=true;return true;
        }
        public void Clear(TransferClearReason reason,SimulationTick tick,int revision)
        {
            if(!_applied)return;
            if(!IsStillAvailable())throw new InvalidOperationException("상자 수정자 소유 물체가 소실됐습니다.");
            _body.mass=_baseMass;_body.gravityScale=_baseGravityScale;_impactDamageMultiplierQ1000=1000;_applied=false;
        }
    }
}
