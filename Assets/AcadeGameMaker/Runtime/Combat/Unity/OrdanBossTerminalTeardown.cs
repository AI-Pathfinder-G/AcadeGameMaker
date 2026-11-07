using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-004, REQ-WT-003, REQ-WT-005. The sole -195 terminal seam.
    [DefaultExecutionOrder(-195)]
    [DisallowMultipleComponent]
    internal sealed class OrdanBossTerminalTeardown : MonoBehaviour
    {
        [SerializeField] private TransferSimulationDriver _transfer;
        [SerializeField] private OrdanBossCombatSimulationDriver _combat;
        [SerializeField] private OrdanBossSimulationDriver _bridge;
        [SerializeField] private OrdanBossTransferExposureScheduler _scheduler;
        [SerializeField] private OrdanBossHostileDamageProducer _hostile;
        [SerializeField] private OrdanBossBalanceAuditPullProducer _pull;
        [SerializeField] private OrdanBossEncounterHandoffAdapter _handoff;
        [SerializeField] private CombatTarget _ordan;
        [SerializeField] private Collider2D _ordanCollider;
        [SerializeField] private List<Collider2D> _targetColliders = new List<Collider2D>();
        private readonly object _owner = new object();
        private OrdanBossTerminalTeardownReceipt? _latest;
        private InputFrameCommitReceipt? _retiredInputReceipt;

        internal OrdanBossTerminalTeardownReceipt? LatestReceipt => _latest;

        internal bool MatchesRetiredInputGraph(PlayerMovementController player, TransferSimulationDriver transfer,
            OrdanBossCombatSimulationDriver combat, SimulationTick nextTick)
        {
            return _latest.HasValue && _retiredInputReceipt.HasValue && _transfer == transfer && _combat == combat &&
                transfer != null && combat != null && player != null && transfer.BoundPlayer == player && combat.BoundPlayer == player &&
                !combat.enabled && combat.IsCoordinatedInputLaneClosed && player.NextExpectedTick == nextTick &&
                _retiredInputReceipt.Value.Tick == _latest.Value.Cleanup.CleanupTick && _retiredInputReceipt.Value.Tick.Value < nextTick.Value &&
                _retiredInputReceipt.Value.Mode != InputMode.GameplayEnabled && transfer.IsInputGateLocked && player.IsInputGateLocked && combat.IsInputGateLocked;
        }
        private void FixedUpdate() => Advance();

        internal void ConfigureForAuthoring(TransferSimulationDriver transfer, OrdanBossCombatSimulationDriver combat,
            OrdanBossSimulationDriver bridge, OrdanBossTransferExposureScheduler scheduler,
            OrdanBossHostileDamageProducer hostile, OrdanBossBalanceAuditPullProducer pull,
            OrdanBossEncounterHandoffAdapter handoff, CombatTarget ordan, Collider2D ordanCollider,
            Collider2D[] targetColliders)
        {
            if (_latest.HasValue) throw new InvalidOperationException("Terminal teardown cannot be reconfigured.");
            _transfer = transfer; _combat = combat; _bridge = bridge; _scheduler = scheduler; _hostile = hostile; _pull = pull;
            _handoff = handoff; _ordan = ordan; _ordanCollider = ordanCollider;
            _targetColliders = targetColliders == null ? null : new List<Collider2D>(targetColliders);
        }

        internal void ValidateForAuthoring(GameObject root)
        {
            if (root == null || transform.root.gameObject != root || _transfer == null || _combat == null || _bridge == null || _scheduler == null ||
                _hostile == null || _pull == null || _handoff == null || _ordan == null || _ordanCollider == null || _targetColliders == null || _targetColliders.Count != 4 ||
                _transfer.transform.root.gameObject != root || _combat.transform.root.gameObject != root || _bridge.transform.root.gameObject != root ||
                _scheduler.transform.root.gameObject != root || _hostile.transform.root.gameObject != root || _pull.transform.root.gameObject != root || _handoff.transform.root.gameObject != root ||
                _transfer.gameObject != gameObject || _combat.gameObject != gameObject || _bridge.gameObject != gameObject || _scheduler.gameObject != gameObject ||
                _hostile.gameObject != gameObject || _pull.gameObject != gameObject || _handoff.gameObject != gameObject || _combat.BoundHostileProducer != _hostile ||
                _ordan.transform.root.gameObject != root || _ordanCollider.transform.root.gameObject != root || _ordan.Freeze().TargetId != "ordan" || _ordan.Freeze().TargetCollider != _ordanCollider ||
                _scheduler.BoundTransferDriver != _transfer || _scheduler.BoundBridge != _bridge || _combat.BoundTransferDriver != _transfer ||
                _combat.BoundPlayer != _transfer.BoundPlayer || _transfer.BoundPlayer == null || !_transfer.BoundPlayer.gameObject.activeInHierarchy ||
                _transfer.BoundRegistry == null || _combat.BoundRegistry == null || !_transfer.enabled || !_transfer.BoundPlayer.enabled ||
                !_transfer.BoundRegistry.enabled || !_combat.BoundRegistry.enabled)
                throw new InvalidOperationException("Terminal teardown bindings are cross-wired.");
            var transferTargets = _transfer.BoundRegistry.Targets;
            var combatTargets = _combat.BoundRegistry.ReadValidatedAuthoringRoster();
            if (transferTargets.Count != 4 || combatTargets.Count != 2 || combatTargets[0].TargetId != "ordan" ||
                combatTargets[0].TargetCollider != _ordanCollider || combatTargets[1].TargetId != "player" ||
                transferTargets[0].TargetCollider != _targetColliders[0] || transferTargets[1].TargetCollider != _targetColliders[1] ||
                transferTargets[2].TargetCollider != _targetColliders[2] || transferTargets[3].TargetCollider != _targetColliders[3])
                throw new InvalidOperationException("Terminal teardown registry collider identity is not exact.");
            var ids = new[] { "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" };
            for (var index = 0; index < ids.Length; index++)
                if (_targetColliders[index] == null || _targetColliders[index].transform.root.gameObject != root || _targetColliders[index].gameObject.name != ids[index])
                    throw new InvalidOperationException("Terminal teardown target collider is not exact.");
            var auditSink = transferTargets[0].TargetOwnerSink as OrdanBossAuditTransferModifierSink;
            var payloadSinks = new OrdanBossPayloadTransferModifierSink[3];
            for (var index = 0; index < payloadSinks.Length; index++)
                payloadSinks[index] = transferTargets[index + 1].TargetOwnerSink as OrdanBossPayloadTransferModifierSink;
            _handoff.ValidateEncounterGraph(root, _transfer.BoundPlayer, _transfer, _combat, _bridge, _scheduler, _ordan,
                transferTargets[0], auditSink, new[] { transferTargets[1], transferTargets[2], transferTargets[3] }, payloadSinks,
                new[] { _targetColliders[1], _targetColliders[2], _targetColliders[3] });
        }

        private void Advance()
        {
            if (_latest.HasValue) throw new InvalidOperationException("Terminal teardown is one-shot.");
            if (_scheduler == null || !_scheduler.IsTerminal) return;
            var cleanup = _transfer == null ? null : _transfer.LatestBossTerminalCleanupReceipt;
            var handoff = _handoff == null ? null : _handoff.LatestPresentationView;
            if (!cleanup.HasValue || (!cleanup.Value.IsLifecycleSuperseded && !cleanup.Value.DurableRemovalCommitted) || !handoff.HasValue || !IsExactTerminalHandoff(handoff.Value, cleanup.Value.DeathSourceTick) ||
                cleanup.Value.CleanupTick != _transfer.BoundPlayer.NextExpectedTick || cleanup.Value.CleanupTick.Value != checked(cleanup.Value.DeathSourceTick.Value + 1))
                throw new InvalidOperationException("Terminal teardown requires exact cleanup and final handoff.");
            ValidateForAuthoring(transform.root.gameObject);
            if (!transform.root.gameObject.activeInHierarchy || !_transfer.gameObject.activeInHierarchy || !_transfer.BoundPlayer.gameObject.activeInHierarchy ||
                !_transfer.BoundRegistry.gameObject.activeInHierarchy || !_combat.BoundRegistry.gameObject.activeInHierarchy)
                throw new InvalidOperationException("Terminal teardown cannot quiesce an inactive authored graph.");
            var coordinatedReceipt = _combat.CurrentCoordinatedReceipt;
            var discard = _combat.PreflightTerminalDeliveryDiscard(_owner, cleanup.Value.DeathSourceTick, coordinatedReceipt);
            PreflightEnabledState();
            var stagedReceipt = new OrdanBossTerminalTeardownReceipt(cleanup.Value, discard.Batch,
                new[] { OrdanBossHandoffKind.Defeated, OrdanBossHandoffKind.RewardRequest, OrdanBossHandoffKind.RoomCompletionRequest },
                new[] { "combat", "bridge", "scheduler", "hostile", "pull", "handoff", "ordan" },
                new[] { "ordan", "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" });
            _combat.CommitPreflightedTerminalDeliveryDiscard(discard, _owner);
            _combat.enabled = false; _bridge.enabled = false; _scheduler.enabled = false; _hostile.enabled = false; _pull.enabled = false; _handoff.enabled = false; _ordan.enabled = false;
            _ordanCollider.enabled = false;
            for (var index = 0; index < _targetColliders.Count; index++) _targetColliders[index].enabled = false;
            _latest = stagedReceipt;
            _retiredInputReceipt = coordinatedReceipt;
            enabled = false;
        }

        private void PreflightEnabledState()
        {
            if (!_combat.enabled || !_bridge.enabled || !_scheduler.enabled || !_hostile.enabled || !_pull.enabled || !_handoff.enabled || !_ordan.enabled ||
                !_ordanCollider.enabled) throw new InvalidOperationException("Terminal teardown target is already disabled.");
            for (var index = 0; index < _targetColliders.Count; index++)
            {
                var collider = _targetColliders[index];
                if (collider == null || collider.enabled) throw new InvalidOperationException("Terminal target collider did not remain hidden.");
                if (index == 0)
                {
                    var auditSink = _transfer.BoundRegistry.Targets[index].TargetOwnerSink as OrdanBossAuditTransferModifierSink;
                    if (auditSink == null || auditSink.IsStillAvailable()) throw new InvalidOperationException("Terminal audit sink did not remain unavailable.");
                }
                else
                {
                    var payloadSink = _transfer.BoundRegistry.Targets[index].TargetOwnerSink as OrdanBossPayloadTransferModifierSink;
                    if (payloadSink == null || payloadSink.IsStillAvailable()) throw new InvalidOperationException("Terminal payload sink did not remain unavailable.");
                }
            }
        }
        private static bool IsExactTerminalHandoff(OrdanBossEncounterPresentationView view, SimulationTick deathTick)
        {
            return view.Tick == deathTick && view.IsTerminal && view.Handoffs.Count == 3 &&
                view.Handoffs[0].Kind == OrdanBossHandoffKind.Defeated && view.Handoffs[1].Kind == OrdanBossHandoffKind.RewardRequest &&
                view.Handoffs[2].Kind == OrdanBossHandoffKind.RoomCompletionRequest && view.Handoffs[0].Tick == deathTick &&
                view.Handoffs[1].Tick == deathTick && view.Handoffs[2].Tick == deathTick;
        }
    }

    internal readonly struct OrdanBossTerminalTeardownReceipt
    {
        internal OrdanBossTerminalTeardownReceipt(BossTerminalTransferCleanupReceipt cleanup, OrdanBossDeliveryBatch discarded,
            IReadOnlyList<OrdanBossHandoffKind> handoffKinds, IReadOnlyList<string> disabledComponents, IReadOnlyList<string> disabledColliders)
        {
            if (discarded.SourceTick != cleanup.DeathSourceTick || discarded.DeliveryTick != cleanup.CleanupTick || discarded.Request.HasValue ||
                discarded.HostileRequests.Count != 0 || discarded.HostileAppendCommitted || handoffKinds == null || handoffKinds.Count != 3 || disabledComponents == null || disabledColliders == null)
                throw new ArgumentException("Terminal teardown receipt is malformed.");
            DeathSourceTick = cleanup.DeathSourceTick; CleanupTick = cleanup.CleanupTick; Cleanup = cleanup; DiscardedDelivery = discarded; EncounterKey = "OrdanBossEncounterGraph";
            if (handoffKinds[0] != OrdanBossHandoffKind.Defeated || handoffKinds[1] != OrdanBossHandoffKind.RewardRequest || handoffKinds[2] != OrdanBossHandoffKind.RoomCompletionRequest) throw new ArgumentException("Terminal handoff kinds are not exact.");
            HandoffKinds = CopyKinds(handoffKinds);
            DisabledComponents = Copy(disabledComponents); DisabledColliders = Copy(disabledColliders);
        }
        internal SimulationTick DeathSourceTick { get; }
        internal SimulationTick CleanupTick { get; }
        internal BossTerminalTransferCleanupReceipt Cleanup { get; }
        internal OrdanBossDeliveryBatch DiscardedDelivery { get; }
        internal string EncounterKey { get; }
        internal IReadOnlyList<OrdanBossHandoffKind> HandoffKinds { get; }
        internal IReadOnlyList<string> DisabledComponents { get; }
        internal IReadOnlyList<string> DisabledColliders { get; }
        internal bool PlayerPreserved => true;
        internal bool Terminal => true;
        private static IReadOnlyList<string> Copy(IReadOnlyList<string> values) { var copy = new string[values.Count]; for (var index = 0; index < copy.Length; index++) copy[index] = values[index]; return Array.AsReadOnly(copy); }
        private static IReadOnlyList<OrdanBossHandoffKind> CopyKinds(IReadOnlyList<OrdanBossHandoffKind> values) { var copy = new OrdanBossHandoffKind[values.Count]; for (var index = 0; index < copy.Length; index++) copy[index] = values[index]; return Array.AsReadOnly(copy); }
    }
}
