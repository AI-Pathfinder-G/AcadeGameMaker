using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Transfer;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-004, REQ-WT-002, REQ-WT-005. Owns only one cyclic scripted
    // payload's reversible heavy bit and availability; Transfer owns player
    // state, revisions in its session, and clear publication.
    [DisallowMultipleComponent]
    internal sealed class OrdanBossPayloadTransferModifierSink : MonoBehaviour, ITransferTargetModifierSink
    {
        internal const string RequiredBaseProfileId = "Boss.Payload.Scripted.v1";
        internal const string RequiredModifierId = "Target.BossPayload.ScriptedHeavy.v1";

        [SerializeField] private string _targetId;
        [SerializeField] private string _baseModifierProfileId = RequiredBaseProfileId;
        private bool _isExposed;
        private bool _isApplied;
        private int _lastAcceptedRevision;

        public string BoundTargetId => _targetId;
        internal bool IsExposed => _isExposed;
        internal bool IsApplied => _isApplied;
        internal int LastAcceptedRevision => _lastAcceptedRevision;
        internal string BaseModifierProfileId => _baseModifierProfileId;

        public bool IsStillAvailable() => _isExposed;

        public bool TryApply(TransferModifierRequest request)
        {
            if (!_isExposed || _isApplied || request.Tick.Value < 0 || request.ProposedRevision <= _lastAcceptedRevision ||
                !string.Equals(request.TargetId, _targetId, StringComparison.Ordinal) || request.Kind != TransferTargetKind.BossPayload ||
                !string.Equals(_baseModifierProfileId, RequiredBaseProfileId, StringComparison.Ordinal) ||
                !string.Equals(request.BaseModifierProfileId, _baseModifierProfileId, StringComparison.Ordinal) ||
                !string.Equals(request.TargetModifierId, RequiredModifierId, StringComparison.Ordinal)) return false;
            _isApplied = true; _lastAcceptedRevision = request.ProposedRevision;
            return true;
        }

        public void Clear(TransferClearReason reason, SimulationTick tick, int revision)
        {
            if (!TransferValidation.Clear(reason) || tick.Value < 0 || revision <= _lastAcceptedRevision)
                throw new ArgumentOutOfRangeException(nameof(revision));
            // A Transfer-owned clear is an accepted revision even when an
            // already-hidden sink has no applied bit left to remove.
            _isApplied = false; _lastAcceptedRevision = revision;
        }

        // Called only after the scheduler has validated the complete desired
        // three-target plan. It intentionally does not clear an applied bit.
        internal void CommitExposure(bool exposed) => _isExposed = exposed;

        // REQ-COM-001, REQ-COM-004. Authoring and tests deliberately share one
        // pristine assignment path; it performs no runtime availability change.
        internal void ConfigureForAuthoring(string targetId, string baseModifierProfileId = RequiredBaseProfileId) =>
            Configure(targetId, baseModifierProfileId);
        internal void ConfigureForTests(string targetId, string baseModifierProfileId = RequiredBaseProfileId) =>
            Configure(targetId, baseModifierProfileId);
        private void Configure(string targetId, string baseModifierProfileId)
        {
            if (targetId != "BossWeight-0" && targetId != "BossWeight-1" && targetId != "BossWeight-2" ||
                !string.Equals(baseModifierProfileId, RequiredBaseProfileId, StringComparison.Ordinal))
                throw new ArgumentException("Payload sink requires an exact scripted payload identity.");
            _targetId = targetId; _baseModifierProfileId = baseModifierProfileId;
            _isExposed = false; _isApplied = false; _lastAcceptedRevision = 0;
        }
    }
}
