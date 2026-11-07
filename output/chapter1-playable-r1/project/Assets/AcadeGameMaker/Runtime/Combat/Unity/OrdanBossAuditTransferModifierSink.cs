using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-003, REQ-COM-004, REQ-WT-003, REQ-WT-005. M4B3A deliberately
    // owns only the dormant audit-box modifier bit; M4B3B owns exposure policy.
    [DisallowMultipleComponent]
    internal sealed class OrdanBossAuditTransferModifierSink : MonoBehaviour, ITransferTargetModifierSink
    {
        internal const string RequiredTargetId = "BossAuditBox-0";
        internal const string RequiredBaseProfileId = "Boss.AuditBox.Scripted.v1";
        internal const string RequiredModifierId = "Target.Box.Heavy.v1";

        [SerializeField] private string _targetId = RequiredTargetId;
        [SerializeField] private string _baseModifierProfileId = RequiredBaseProfileId;
        private bool _isExposed;
        private bool _isApplied;
        private int _lastAcceptedRevision;

        public string BoundTargetId => _targetId;
        internal string BaseModifierProfileId => _baseModifierProfileId;
        internal bool IsExposed => _isExposed;
        internal bool IsApplied => _isApplied;
        internal int LastAcceptedRevision => _lastAcceptedRevision;

        public bool IsStillAvailable() => _isExposed;

        public bool TryApply(TransferModifierRequest request)
        {
            if (!_isExposed || _isApplied || request.Tick.Value < 0 || request.ProposedRevision <= _lastAcceptedRevision ||
                !string.Equals(request.TargetId, RequiredTargetId, StringComparison.Ordinal) || request.Kind != TransferTargetKind.Box ||
                !string.Equals(_targetId, RequiredTargetId, StringComparison.Ordinal) ||
                !string.Equals(_baseModifierProfileId, RequiredBaseProfileId, StringComparison.Ordinal) ||
                !string.Equals(request.BaseModifierProfileId, RequiredBaseProfileId, StringComparison.Ordinal) ||
                !string.Equals(request.TargetModifierId, RequiredModifierId, StringComparison.Ordinal)) return false;
            _isApplied = true; _lastAcceptedRevision = request.ProposedRevision;
            return true;
        }

        public void Clear(TransferClearReason reason, SimulationTick tick, int revision)
        {
            if (!TransferValidation.Clear(reason) || tick.Value < 0 || revision <= _lastAcceptedRevision)
                throw new ArgumentOutOfRangeException(nameof(revision));
            _isApplied = false; _lastAcceptedRevision = revision;
        }

        // M4B3B is the only future caller allowed to change availability.
        internal void CommitExposureForM4B3B(bool exposed) => _isExposed = exposed;

        // REQ-COM-004. Assignment-only authoring seam: it neither validates nor initializes.
        internal void ConfigureForAuthoring(string targetId, string baseModifierProfileId)
        {
            if (targetId != RequiredTargetId || !string.Equals(baseModifierProfileId, RequiredBaseProfileId, StringComparison.Ordinal))
                throw new ArgumentException("Audit sink requires exact authored identity.");
            _targetId = targetId; _baseModifierProfileId = baseModifierProfileId;
            _isExposed = false; _isApplied = false; _lastAcceptedRevision = 0;
        }

        internal void ConfigureForTests(string targetId, string baseModifierProfileId = RequiredBaseProfileId) =>
            ConfigureForAuthoring(targetId, baseModifierProfileId);
    }
}
