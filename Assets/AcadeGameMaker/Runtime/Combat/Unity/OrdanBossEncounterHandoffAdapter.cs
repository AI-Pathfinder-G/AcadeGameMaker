using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-001, REQ-COM-003, REQ-COM-004, REQ-COM-006. M4B3A's +110
    // reader is deliberately a value-only handoff; it has no gameplay authority.
    [DefaultExecutionOrder(110)]
    [DisallowMultipleComponent]
    internal sealed class OrdanBossEncounterHandoffAdapter : MonoBehaviour
    {
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private TransferSimulationDriver _transfer;
        [SerializeField] private OrdanBossCombatSimulationDriver _combat;
        [SerializeField] private OrdanBossSimulationDriver _bridge;
        [SerializeField] private OrdanBossTransferExposureScheduler _scheduler;
        [SerializeField] private OrdanBossHostileDamageProducer _hostileProducer;
        [SerializeField] private OrdanBossBalanceAuditPullProducer _balanceAuditPullProducer;
        [SerializeField] private CombatTarget _ordan;
        [SerializeField] private TransferTarget _auditTarget;
        [SerializeField] private OrdanBossAuditTransferModifierSink _auditSink;
        [SerializeField] private List<TransferTarget> _payloadTargets = new List<TransferTarget>();
        [SerializeField] private List<OrdanBossPayloadTransferModifierSink> _payloadSinks = new List<OrdanBossPayloadTransferModifierSink>();
        [SerializeField] private List<Collider2D> _payloadColliders = new List<Collider2D>();

        private OrdanBossEncounterPresentationView? _latest;

        internal OrdanBossEncounterPresentationView? LatestPresentationView => _latest;

        private void FixedUpdate() => AdvanceHandoff();

        // REQ-COM-001, REQ-COM-004. Builder-only assignment seam; it neither
        // initializes upstream owners nor publishes a view.
        internal void ConfigureForAuthoring(PlayerMovementController player, TransferSimulationDriver transfer,
            OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge, OrdanBossTransferExposureScheduler scheduler,
            CombatTarget ordan, TransferTarget auditTarget, OrdanBossAuditTransferModifierSink auditSink,
            TransferTarget[] payloadTargets, OrdanBossPayloadTransferModifierSink[] payloadSinks, Collider2D[] payloadColliders)
        {
            if (_latest.HasValue) throw new InvalidOperationException("Boss handoff cannot be reconfigured after publication.");
            _player = player; _transfer = transfer; _combat = combat; _bridge = bridge; _scheduler = scheduler; _ordan = ordan;
            _auditTarget = auditTarget; _auditSink = auditSink;
            _payloadTargets = payloadTargets == null ? null : new List<TransferTarget>(payloadTargets);
            _payloadSinks = payloadSinks == null ? null : new List<OrdanBossPayloadTransferModifierSink>(payloadSinks);
            _payloadColliders = payloadColliders == null ? null : new List<Collider2D>(payloadColliders);
        }
        // REQ-COM-004. Authoring binds the sole -170 producer without publishing.
        internal void ConfigureHostileProducerForAuthoring(OrdanBossHostileDamageProducer producer)
        { if (_latest.HasValue) throw new InvalidOperationException("Boss handoff cannot reconfigure hostile binding after publication."); _hostileProducer = producer; }
        // REQ-MOV-001, REQ-MOV-005. Authoring binds the sole -165 producer
        // without granting this value-only reader any movement authority.
        internal void ConfigureBalanceAuditPullProducerForAuthoring(OrdanBossBalanceAuditPullProducer producer)
        { if (_latest.HasValue) throw new InvalidOperationException("Boss handoff cannot reconfigure BalanceAudit pull binding after publication."); _balanceAuditPullProducer = producer; }

        internal void ValidateEncounterGraph(GameObject root, PlayerMovementController player, TransferSimulationDriver transfer,
            OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge, OrdanBossTransferExposureScheduler scheduler,
            CombatTarget ordan, TransferTarget auditTarget, OrdanBossAuditTransferModifierSink auditSink,
            TransferTarget[] payloadTargets, OrdanBossPayloadTransferModifierSink[] payloadSinks, Collider2D[] payloadColliders)
        {
            var expectedRoot = transform.root;
            var transferRegistry = transfer == null ? null : transfer.BoundRegistry;
            var combatRegistry = combat == null ? null : combat.BoundRegistry;
            if (root == null || expectedRoot == null || root.transform != expectedRoot ||
                _player != player || _transfer != transfer || _combat != combat || _bridge != bridge || _scheduler != scheduler ||
                _ordan != ordan || _auditTarget != auditTarget || _auditSink != auditSink || _hostileProducer == null || _balanceAuditPullProducer == null || _payloadTargets == null || _payloadSinks == null || _payloadColliders == null ||
                _payloadTargets.Count != 3 || _payloadSinks.Count != 3 || _payloadColliders.Count != 3 || _scheduler.BoundBridge != bridge ||
                _scheduler.BoundTransferDriver != transfer || _scheduler.BoundAuditTarget != auditTarget || _scheduler.BoundAuditSink != auditSink ||
                _scheduler.BoundAuditCollider != auditTarget.TargetCollider || _bridge.BoundTransferDriver != transfer || _combat.BoundPlayer != player ||
                _combat.BoundHostileProducer != _hostileProducer || _hostileProducer.BoundPlayer != player || _hostileProducer.BoundTransfer != transfer ||
                _hostileProducer.BoundCombat != combat || _hostileProducer.BoundBridge != bridge || _hostileProducer.BoundScheduler != scheduler ||
                _balanceAuditPullProducer.BoundPlayer != player || _balanceAuditPullProducer.BoundTransfer != transfer ||
                _balanceAuditPullProducer.BoundCombat != combat || _balanceAuditPullProducer.BoundBridge != bridge ||
                _balanceAuditPullProducer.BoundScheduler != scheduler ||
                _combat.BoundTransferDriver != transfer || transferRegistry == null || combatRegistry == null ||
                transfer.gameObject != gameObject || combat.gameObject != gameObject || bridge.gameObject != gameObject || scheduler.gameObject != gameObject || _hostileProducer.gameObject != gameObject || _balanceAuditPullProducer.gameObject != gameObject ||
                transferRegistry.gameObject != gameObject || combatRegistry.gameObject != gameObject ||
                !BelongsToRoot(player, expectedRoot) || !BelongsToRoot(transfer, expectedRoot) || !BelongsToRoot(combat, expectedRoot) ||
                !BelongsToRoot(bridge, expectedRoot) || !BelongsToRoot(scheduler, expectedRoot) || !BelongsToRoot(ordan, expectedRoot) ||
                !BelongsToRoot(auditTarget, expectedRoot) || !BelongsToRoot(auditSink, expectedRoot))
                throw new InvalidOperationException("Boss handoff bindings are cross-wired.");
            var registeredTransfers = transferRegistry.Targets;
            if (registeredTransfers.Count != 4 || registeredTransfers[0] != auditTarget || registeredTransfers[1] != payloadTargets[0] ||
                registeredTransfers[2] != payloadTargets[1] || registeredTransfers[3] != payloadTargets[2])
                throw new InvalidOperationException("Boss handoff Transfer registry is not exact.");
            var registeredCombatants = combatRegistry.ReadValidatedAuthoringRoster();
            if (registeredCombatants.Count != 2 || registeredCombatants[0].TargetId != "ordan" || registeredCombatants[1].TargetId != "player" ||
                registeredCombatants[0].TargetCollider != ordan.Freeze().TargetCollider || registeredCombatants[1].TargetPoseRoot != player.transform)
                throw new InvalidOperationException("Boss handoff Combat registry is not exact.");
            for (var i = 0; i < 3; i++)
            {
                if (_payloadTargets[i] != payloadTargets[i] || _payloadSinks[i] != payloadSinks[i] || _payloadColliders[i] != payloadColliders[i] ||
                    _payloadTargets[i] == null || _payloadSinks[i] == null || _payloadColliders[i] == null ||
                    _payloadTargets[i].TargetId != "BossWeight-" + i || _payloadSinks[i].BoundTargetId != "BossWeight-" + i ||
                    _payloadTargets[i].TargetCollider != _payloadColliders[i] || !BelongsToRoot(_payloadTargets[i], expectedRoot) ||
                    !BelongsToRoot(_payloadSinks[i], expectedRoot) || _payloadColliders[i].transform.root != expectedRoot)
                    throw new InvalidOperationException("Boss handoff payload binding is not exact.");
            }
            if (_auditTarget == null || _auditSink == null || _auditTarget.TargetId != OrdanBossAuditTransferModifierSink.RequiredTargetId ||
                !ReferenceEquals(_auditTarget.TargetOwnerSink, _auditSink) || _auditTarget.TargetCollider == null ||
                _auditTarget.TargetCollider.gameObject != _auditTarget.gameObject || !(_auditTarget.TargetCollider is BoxCollider2D) ||
                _auditTarget.TargetCollider.transform.root != expectedRoot ||
                _ordan == null || _player == null || _hostileProducer == null || _balanceAuditPullProducer == null ||
                !BelongsToRoot(_hostileProducer, expectedRoot) || !BelongsToRoot(_balanceAuditPullProducer, expectedRoot))
                throw new InvalidOperationException("Boss handoff audit or body binding is not exact.");
        }

        private void AdvanceHandoff()
        {
            ValidateEncounterGraph(transform.root.gameObject, _player, _transfer, _combat, _bridge, _scheduler, _ordan, _auditTarget, _auditSink,
                _payloadTargets.ToArray(), _payloadSinks.ToArray(), _payloadColliders.ToArray());
            // The movement owner advances at the default execution order after
            // the -200..-180 boss phases. The completed publication therefore
            // belongs to the committed player snapshot tick, while
            // NextExpectedTick already points at the following phase.
            var tick = _player.Snapshot.Tick;
            if (_player.NextExpectedTick.Value != checked(tick.Value + 1))
                throw new InvalidOperationException("Boss handoff requires the exact completed player publication.");
            var expectedPublicationTick = _latest.HasValue ? checked(_latest.Value.Tick.Value + 1) : 0;
            if (tick.Value != expectedPublicationTick)
                throw new InvalidOperationException("Boss handoff publication is duplicate, stale, or skipped.");
            var transfer = _transfer.LatestPhasePublication;
            if (!transfer.HasValue || transfer.Value.Tick != tick || !_combat.HasLatestOutcome || _combat.LatestOutcome.Tick != tick ||
                !_bridge.HasLatestOutcome || _bridge.LatestOutcome.SourceTick != tick || !_scheduler.HasLatestOutcome || _scheduler.LatestOutcome.SourceTick != tick || !_hostileProducer.HasLatestOutcome || _hostileProducer.LatestOutcome.Tick != tick ||
                !_balanceAuditPullProducer.HasLatestOutcome || _balanceAuditPullProducer.LatestOutcome.SourceTick != tick ||
                !_scheduler.HasLatestAuditOutcome || _scheduler.LatestAuditOutcome.SourceTick != tick)
                throw new InvalidOperationException("Boss handoff requires exact completed current publications.");
            var bridge = _bridge.LatestOutcome; var scheduler = _scheduler.LatestOutcome; var combat = _combat.LatestOutcome;
            var audit = _scheduler.LatestAuditOutcome;
            var hostile = _hostileProducer.LatestOutcome;
            var pull = _balanceAuditPullProducer.LatestOutcome;
            if (!_player.TryGetExternalMovementDirectiveReceipt(tick, out var pullReceipt))
                throw new InvalidOperationException("Boss handoff requires the exact completed external-movement receipt.");
            var pullDigest = OrdanBossBalanceAuditPullDigest.From(tick, pull, pullReceipt);
            if (bridge.Forecast.SourceTick != tick || bridge.Forecast.TransferTick != scheduler.TransferTick || scheduler.TransferTick.Value != checked(tick.Value + 1) ||
                bridge.AuditForecast.SourceTick != tick || bridge.AuditForecast.TransferTick != audit.TransferTick || bridge.AuditForecast.TargetId != audit.ExposedTargetId ||
                bridge.Forecast.TargetId != scheduler.ExposedTargetId || combat.Combatants.Count != 2 || combat.Combatants[0].TargetId != "ordan" || combat.Combatants[1].TargetId != "player")
                throw new InvalidOperationException("Boss handoff publication identity is inconsistent.");
            var candidate = new OrdanBossEncounterPresentationView(tick, bridge.Output.Snapshot.NextExpectedTick, bridge.Output.Snapshot,
                bridge.Output.Intents, bridge.Handoffs, combat.Combatants, transfer.Value.Outcome.Snapshot.ActiveTargetId,
                transfer.Value.Outcome.Snapshot.PlayerState, bridge.Forecast.TargetId, scheduler.ExposedTargetId, bridge.AuditForecast.TargetId, audit.ExposedTargetId, audit.TemporarilyEndedTargetIds, audit.PermanentlyRemovedTargetIds,
                _auditSink.IsStillAvailable(), _auditSink.IsApplied, PayloadAvailability(), scheduler.IsTerminal,
                new[] { "ordan", "player", "BossAuditBox-0", "BossWeight-0", "BossWeight-1", "BossWeight-2" }, hostile.Digest, pullDigest);
            _latest = candidate;
        }

        private static bool BelongsToRoot(Component value, Transform expectedRoot) =>
            value != null && value.transform.root == expectedRoot;

        private bool[] PayloadAvailability()
        {
            var values = new bool[3];
            for (var i = 0; i < values.Length; i++)
            {
                if (_payloadTargets[i].TargetCollider.enabled != _payloadSinks[i].IsStillAvailable())
                    throw new InvalidOperationException("Payload collider and availability drifted.");
                values[i] = _payloadSinks[i].IsStillAvailable();
            }
            return values;
        }
    }

    // REQ-COM-003, REQ-COM-004. Immutable, Unity-reference-free M4B3A view.
    internal readonly struct OrdanBossEncounterPresentationView
    {
        internal OrdanBossEncounterPresentationView(SimulationTick tick, SimulationTick nextExpectedTick, OrdanBossSnapshot snapshot,
            IReadOnlyList<OrdanBossIntent> intents, IReadOnlyList<OrdanBossHandoff> handoffs, IReadOnlyList<CombatantSnapshot> combatants,
            string activeTransferTargetId, TransferPlayerState transferPlayerState, string forecastPayloadId, string exposedPayloadId, string auditForecastId, string auditExposedId, IReadOnlyList<string> auditTemporaryEnds, IReadOnlyList<string> auditPermanentRemovals,
            bool auditAvailable, bool auditApplied, IReadOnlyList<bool> payloadAvailable, bool terminal, IReadOnlyList<string> stableIds)
            : this(tick, nextExpectedTick, snapshot, intents, handoffs, combatants, activeTransferTargetId,
                transferPlayerState, forecastPayloadId, exposedPayloadId, auditForecastId, auditExposedId,
                auditTemporaryEnds, auditPermanentRemovals, auditAvailable, auditApplied, payloadAvailable,
                terminal, stableIds, OrdanBossHostileDamageDigest.NoAttack(tick)) { }
        internal OrdanBossEncounterPresentationView(SimulationTick tick, SimulationTick nextExpectedTick, OrdanBossSnapshot snapshot,
            IReadOnlyList<OrdanBossIntent> intents, IReadOnlyList<OrdanBossHandoff> handoffs, IReadOnlyList<CombatantSnapshot> combatants,
            string activeTransferTargetId, TransferPlayerState transferPlayerState, string forecastPayloadId, string exposedPayloadId, string auditForecastId, string auditExposedId, IReadOnlyList<string> auditTemporaryEnds, IReadOnlyList<string> auditPermanentRemovals,
            bool auditAvailable, bool auditApplied, IReadOnlyList<bool> payloadAvailable, bool terminal, IReadOnlyList<string> stableIds,
            OrdanBossHostileDamageDigest hostileDamageDigest)
            : this(tick, nextExpectedTick, snapshot, intents, handoffs, combatants, activeTransferTargetId,
                transferPlayerState, forecastPayloadId, exposedPayloadId, auditForecastId, auditExposedId,
                auditTemporaryEnds, auditPermanentRemovals, auditAvailable, auditApplied, payloadAvailable,
                terminal, stableIds, hostileDamageDigest, OrdanBossBalanceAuditPullDigest.NoDirective(tick)) { }
        internal OrdanBossEncounterPresentationView(SimulationTick tick, SimulationTick nextExpectedTick, OrdanBossSnapshot snapshot,
            IReadOnlyList<OrdanBossIntent> intents, IReadOnlyList<OrdanBossHandoff> handoffs, IReadOnlyList<CombatantSnapshot> combatants,
            string activeTransferTargetId, TransferPlayerState transferPlayerState, string forecastPayloadId, string exposedPayloadId, string auditForecastId, string auditExposedId, IReadOnlyList<string> auditTemporaryEnds, IReadOnlyList<string> auditPermanentRemovals,
            bool auditAvailable, bool auditApplied, IReadOnlyList<bool> payloadAvailable, bool terminal, IReadOnlyList<string> stableIds,
            OrdanBossHostileDamageDigest hostileDamageDigest, OrdanBossBalanceAuditPullDigest balanceAuditPullDigest)
        {
            if (intents == null || handoffs == null || combatants == null || payloadAvailable == null || stableIds == null || auditTemporaryEnds == null || auditPermanentRemovals == null ||
                snapshot.Tick != tick || nextExpectedTick != snapshot.NextExpectedTick || payloadAvailable.Count != 3 || stableIds.Count != 6 ||
                hostileDamageDigest.Tick != tick || hostileDamageDigest.Terminal != terminal || balanceAuditPullDigest.Tick != tick)
                throw new ArgumentException("Boss presentation view is malformed.");
            Tick = tick; NextExpectedTick = nextExpectedTick; Snapshot = snapshot; ActiveTransferTargetId = activeTransferTargetId ?? string.Empty;
            TransferPlayerState = transferPlayerState; ForecastPayloadId = forecastPayloadId ?? string.Empty; ExposedPayloadId = exposedPayloadId ?? string.Empty; AuditForecastId = auditForecastId ?? string.Empty; AuditExposedId = auditExposedId ?? string.Empty;
            AuditAvailable = auditAvailable; AuditApplied = auditApplied; IsTerminal = terminal;
            HostileDamageDigest = hostileDamageDigest;
            BalanceAuditPullDigest = balanceAuditPullDigest;
            Intents = Copy(intents); Handoffs = Copy(handoffs); Combatants = Copy(combatants); PayloadAvailable = Copy(payloadAvailable); StableIds = Copy(stableIds); AuditTemporaryEndedTargetIds = Copy(auditTemporaryEnds); AuditPermanentlyRemovedTargetIds = Copy(auditPermanentRemovals);
        }
        internal SimulationTick Tick { get; } internal SimulationTick NextExpectedTick { get; } internal OrdanBossSnapshot Snapshot { get; }
        internal IReadOnlyList<OrdanBossIntent> Intents { get; } internal IReadOnlyList<OrdanBossHandoff> Handoffs { get; }
        internal IReadOnlyList<CombatantSnapshot> Combatants { get; } internal string ActiveTransferTargetId { get; }
        internal TransferPlayerState TransferPlayerState { get; } internal string ForecastPayloadId { get; } internal string ExposedPayloadId { get; }
        internal string AuditForecastId { get; } internal string AuditExposedId { get; } internal IReadOnlyList<string> AuditTemporaryEndedTargetIds { get; } internal IReadOnlyList<string> AuditPermanentlyRemovedTargetIds { get; }
        internal bool AuditAvailable { get; } internal bool AuditApplied { get; } internal IReadOnlyList<bool> PayloadAvailable { get; }
        internal bool IsTerminal { get; } internal IReadOnlyList<string> StableIds { get; }
        internal OrdanBossHostileDamageDigest HostileDamageDigest { get; }
        internal OrdanBossBalanceAuditPullDigest BalanceAuditPullDigest { get; }
        private static IReadOnlyList<T> Copy<T>(IReadOnlyList<T> source)
        { var copy = new T[source.Count]; for (var i = 0; i < copy.Length; i++) copy[i] = source[i]; return new ReadOnlyCollection<T>(copy); }
    }

    // REQ-MOV-001, REQ-MOV-005; AC-M4B3B3-008. Immutable handoff evidence
    // separates the directive consumed at t from the directive queued for t+1.
    internal readonly struct OrdanBossBalanceAuditPullDigest
    {
        private OrdanBossBalanceAuditPullDigest(
            SimulationTick tick,
            OrdanBossBalanceAuditPullDisposition disposition,
            string consumedCurrentDirectiveId,
            int appliedDeltaXQ4096,
            int appliedDeltaYQ4096,
            string cancelledCurrentDirectiveId,
            string queuedNextDirectiveId,
            SimulationTick? queuedNextMovementTick,
            bool terminal)
        {
            Tick = tick;
            Disposition = disposition;
            ConsumedCurrentDirectiveId = consumedCurrentDirectiveId ?? string.Empty;
            AppliedDeltaXQ4096 = appliedDeltaXQ4096;
            AppliedDeltaYQ4096 = appliedDeltaYQ4096;
            CancelledCurrentDirectiveId = cancelledCurrentDirectiveId ?? string.Empty;
            QueuedNextDirectiveId = queuedNextDirectiveId ?? string.Empty;
            QueuedNextMovementTick = queuedNextMovementTick;
            Terminal = terminal;
        }

        internal SimulationTick Tick { get; }
        internal OrdanBossBalanceAuditPullDisposition Disposition { get; }
        internal string ConsumedCurrentDirectiveId { get; }
        internal int AppliedDeltaXQ4096 { get; }
        internal int AppliedDeltaYQ4096 { get; }
        internal string CancelledCurrentDirectiveId { get; }
        internal string QueuedNextDirectiveId { get; }
        internal SimulationTick? QueuedNextMovementTick { get; }
        internal bool Terminal { get; }

        internal static OrdanBossBalanceAuditPullDigest NoDirective(SimulationTick tick) =>
            new OrdanBossBalanceAuditPullDigest(tick, OrdanBossBalanceAuditPullDisposition.NoProjection,
                string.Empty, 0, 0, string.Empty, string.Empty, null, false);

        internal static OrdanBossBalanceAuditPullDigest From(
            SimulationTick tick,
            OrdanBossBalanceAuditPullOutcome outcome,
            ExternalMovementDirectiveReceipt receipt)
        {
            if (outcome.SourceTick != tick || receipt.MovementTick != tick)
                throw new InvalidOperationException("BalanceAudit pull handoff tick identity is inconsistent.");

            var expectedCurrent = outcome.ExpectedCurrentDirectiveId;
            if (string.IsNullOrEmpty(expectedCurrent))
            {
                if (receipt.HadDirective)
                    throw new InvalidOperationException("BalanceAudit pull consumed an unexpected current directive.");
            }
            else if (!receipt.HadDirective || !string.Equals(receipt.ConsumedDirectiveId, expectedCurrent, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("BalanceAudit pull current-directive receipt is missing or mismatched.");
            }

            if ((!string.IsNullOrEmpty(outcome.CancelledCurrentDirectiveId) && !string.IsNullOrEmpty(expectedCurrent)) ||
                (string.IsNullOrEmpty(outcome.QueuedNextDirectiveId) != !outcome.QueuedNextMovementTick.HasValue) ||
                (outcome.QueuedNextMovementTick.HasValue && outcome.QueuedNextMovementTick.Value.Value != checked(tick.Value + 1)))
                throw new InvalidOperationException("BalanceAudit pull producer outcome is malformed.");

            return new OrdanBossBalanceAuditPullDigest(
                tick,
                outcome.Disposition,
                receipt.HadDirective ? receipt.ConsumedDirectiveId : string.Empty,
                receipt.AppliedDeltaXQ4096,
                receipt.AppliedDeltaYQ4096,
                outcome.CancelledCurrentDirectiveId,
                outcome.QueuedNextDirectiveId,
                outcome.QueuedNextMovementTick,
                outcome.Terminal);
        }
    }
}
