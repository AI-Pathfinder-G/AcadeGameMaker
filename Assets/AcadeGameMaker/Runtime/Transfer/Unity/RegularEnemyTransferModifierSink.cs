using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using UnityEngine;

namespace AcadeGameMaker.Transfer.Unity
{
    // REQ-WT-002, REQ-WT-003, REQ-WT-005; affected REQ-COM-001. Owns only the
    // regular enemy Heavy modifier bit and accepted revision, never life or collision.
    [DisallowMultipleComponent]
    internal sealed class RegularEnemyTransferModifierSink : MonoBehaviour, ITransferTargetModifierSink
    {
        internal const string RequiredBaseProfileId = "Enemy.Regular.Baseline.v1";
        internal const string RequiredHeavyModifierId = "Target.Enemy.Heavy.v1";

        [SerializeField] private string _targetId;
        [SerializeField] private string _baseModifierProfileId = RequiredBaseProfileId;
        private bool _isHeavy;
        private int _lastAcceptedRevision;

        public string BoundTargetId => _targetId;
        internal string BaseModifierProfileId => _baseModifierProfileId;
        internal bool IsHeavy => _isHeavy;
        internal int LastAcceptedRevision => _lastAcceptedRevision;
        public bool IsStillAvailable() => isActiveAndEnabled && gameObject.activeInHierarchy;

        public bool TryApply(TransferModifierRequest request)
        {
            if (_isHeavy || request.Tick.Value < 0 || request.ProposedRevision <= _lastAcceptedRevision ||
                !string.Equals(_targetId, request.TargetId, StringComparison.Ordinal) ||
                request.Kind != TransferTargetKind.Enemy ||
                !string.Equals(_baseModifierProfileId, RequiredBaseProfileId, StringComparison.Ordinal) ||
                !string.Equals(request.BaseModifierProfileId, _baseModifierProfileId, StringComparison.Ordinal) ||
                !string.Equals(request.TargetModifierId, RequiredHeavyModifierId, StringComparison.Ordinal))
                return false;
            _isHeavy = true;
            _lastAcceptedRevision = request.ProposedRevision;
            return true;
        }

        public void Clear(TransferClearReason reason, SimulationTick tick, int revision)
        {
            if (!TransferValidation.Clear(reason) || tick.Value < 0)
                throw new ArgumentOutOfRangeException(nameof(reason));
            if (!_isHeavy) return;
            if (revision <= _lastAcceptedRevision)
                throw new ArgumentOutOfRangeException(nameof(revision));
            _isHeavy = false;
            _lastAcceptedRevision = revision;
        }

        internal void ConfigureForAuthoring(string targetId, string baseModifierProfileId)
        {
            if ((targetId != "walker" && targetId != "surveyor") ||
                !string.Equals(baseModifierProfileId, RequiredBaseProfileId, StringComparison.Ordinal))
                throw new ArgumentException("Regular enemy sink requires a literal role and exact base profile.");
            _targetId = targetId;
            _baseModifierProfileId = baseModifierProfileId;
            _isHeavy = false;
            _lastAcceptedRevision = 0;
        }
    }
}
