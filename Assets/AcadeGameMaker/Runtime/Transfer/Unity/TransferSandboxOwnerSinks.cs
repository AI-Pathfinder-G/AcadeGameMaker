using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using UnityEngine;

namespace AcadeGameMaker.Transfer.Unity
{
    // REQ-WT-002, REQ-WT-005. Box ownership retains and restores base Rigidbody2D state; transfer never derives or emits damage.
    internal sealed class TransferSandboxBoxOwnerSink : MonoBehaviour, ITransferTargetModifierSink
    {
        [SerializeField] private string _targetId;
        [SerializeField] private Rigidbody2D _body;
        private int _impactDamageMultiplierQ1000 = 1000;
        private bool _applied;
        private float _baseMass;
        private float _baseGravityScale;

        internal int ImpactDamageMultiplierQ1000 => _impactDamageMultiplierQ1000;
        internal bool IsAppliedForTests => _applied;
        public string BoundTargetId => _targetId;
        public bool IsStillAvailable() => _body != null;
        public bool TryApply(TransferModifierRequest request)
        {
            if (_applied || request.TargetId != _targetId || request.TargetModifierId != "Target.Box.Heavy.v1" || _body == null) return false;
            _baseMass = _body.mass; _baseGravityScale = _body.gravityScale;
            _body.mass = _baseMass * 3f; _body.gravityScale = _baseGravityScale * 2f; _impactDamageMultiplierQ1000 = 1500; _applied = true;
            return true;
        }
        public void Clear(TransferClearReason reason, SimulationTick tick, int revision)
        {
            if (!_applied) return;
            _body.mass = _baseMass; _body.gravityScale = _baseGravityScale; _impactDamageMultiplierQ1000 = 1000; _applied = false;
        }
        internal void ConfigureForTests(string targetId, Rigidbody2D body)
        {
            _targetId = targetId; _body = body; _impactDamageMultiplierQ1000 = 1000;
        }
    }

    // REQ-WT-002, REQ-WT-005. Isolated owner stub covers non-box modifier identity without taking AI, health, or damage ownership.
    internal sealed class TransferSandboxOwnerStub : MonoBehaviour, ITransferTargetModifierSink
    {
        [SerializeField] private string _targetId;
        [SerializeField] private string _acceptedModifierId;
        private bool _applied;
        internal bool IsAppliedForTests => _applied;
        public string BoundTargetId => _targetId;
        public bool IsStillAvailable() => true;
        public bool TryApply(TransferModifierRequest request)
        {
            if (_applied || request.TargetId != _targetId || request.TargetModifierId != _acceptedModifierId) return false;
            _applied = true; return true;
        }
        public void Clear(TransferClearReason reason, SimulationTick tick, int revision) => _applied = false;
        internal void ConfigureForTests(string targetId, string acceptedModifierId) { _targetId = targetId; _acceptedModifierId = acceptedModifierId; }
    }
}
