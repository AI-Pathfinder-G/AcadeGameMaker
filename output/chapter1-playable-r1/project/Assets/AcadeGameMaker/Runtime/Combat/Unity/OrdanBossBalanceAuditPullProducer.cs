using System;
using System.Collections.Generic;
using System.Globalization;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-MOV-001, REQ-MOV-005, REQ-COM-003, REQ-COM-004; AC-M4B3B3-001/003/007/008.
    // This -165 producer owns only the one-tick directive transaction. Movement
    // remains the sole owner of vector math, collision, snapshot, and receipt.
    [DefaultExecutionOrder(-165)]
    [DisallowMultipleComponent]
    internal sealed class OrdanBossBalanceAuditPullProducer : MonoBehaviour
    {
        internal const int ApprovedAnchorXQ4096 = 28672;
        internal const int ApprovedAnchorYQ4096 = 4915;
        internal const int ApprovedMaximumStepQ4096 = 410;

        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private TransferSimulationDriver _transfer;
        [SerializeField] private OrdanBossCombatSimulationDriver _combat;
        [SerializeField] private OrdanBossSimulationDriver _bridge;
        [SerializeField] private OrdanBossTransferExposureScheduler _scheduler;
        [SerializeField] private int _anchorXQ4096 = ApprovedAnchorXQ4096;
        [SerializeField] private int _anchorYQ4096 = ApprovedAnchorYQ4096;
        [SerializeField] private int _maximumStepQ4096 = ApprovedMaximumStepQ4096;

        private readonly HashSet<string> _committedLatches = new HashSet<string>(StringComparer.Ordinal);
        private bool _registered;
        private bool _terminal;
        private bool _hasLatestOutcome;
        private OrdanBossBalanceAuditPullOutcome _latestOutcome;
        private PullDirectiveIdentity? _currentDirective;

        internal bool HasLatestOutcome => _hasLatestOutcome;
        internal OrdanBossBalanceAuditPullOutcome LatestOutcome => _hasLatestOutcome
            ? _latestOutcome
            : throw new InvalidOperationException("BalanceAudit pull producer has not published.");
        internal PlayerMovementController BoundPlayer => _player;
        internal TransferSimulationDriver BoundTransfer => _transfer;
        internal OrdanBossCombatSimulationDriver BoundCombat => _combat;
        internal OrdanBossSimulationDriver BoundBridge => _bridge;
        internal OrdanBossTransferExposureScheduler BoundScheduler => _scheduler;
        internal int AuthoredAnchorXQ4096 => _anchorXQ4096;
        internal int AuthoredAnchorYQ4096 => _anchorYQ4096;
        internal int AuthoredMaximumStepQ4096 => _maximumStepQ4096;

        private void Awake()
        {
            if (_player != null || _transfer != null || _combat != null || _bridge != null || _scheduler != null)
            {
                EnsureRegistered();
            }
        }

        private void FixedUpdate()
        {
            if (!_terminal)
            {
                Advance();
            }
        }

        internal void ConfigureForAuthoring(
            PlayerMovementController player,
            TransferSimulationDriver transfer,
            OrdanBossCombatSimulationDriver combat,
            OrdanBossSimulationDriver bridge,
            OrdanBossTransferExposureScheduler scheduler)
        {
            if (_registered || _hasLatestOutcome)
            {
                throw new InvalidOperationException("BalanceAudit pull producer cannot be reconfigured after registration or publication.");
            }

            _player = player;
            _transfer = transfer;
            _combat = combat;
            _bridge = bridge;
            _scheduler = scheduler;
            _anchorXQ4096 = ApprovedAnchorXQ4096;
            _anchorYQ4096 = ApprovedAnchorYQ4096;
            _maximumStepQ4096 = ApprovedMaximumStepQ4096;
        }

        internal void InitializeForTests() => EnsureRegistered();
        internal OrdanBossBalanceAuditPullOutcome AdvanceForTests() => Advance();

        private OrdanBossBalanceAuditPullOutcome Advance()
        {
            if (_terminal)
            {
                throw new InvalidOperationException("Terminal BalanceAudit pull producer cannot advance.");
            }

            ValidateBindingsAndPublications(out var tick, out var combatants, out var transferPublication);
            if ((!_hasLatestOutcome && tick != (_player.Chapter1ConfiguredFirstTick ?? new SimulationTick(0))) ||
                (_hasLatestOutcome && tick.Value != checked(_latestOutcome.SourceTick.Value + 1)))
            {
                throw new InvalidOperationException("BalanceAudit pull publication is duplicate, stale, or skipped.");
            }
            ValidateCurrentDirectiveHorizon(tick);
            var expectedCurrentId = _currentDirective.HasValue ? _currentDirective.Value.DirectiveId : string.Empty;

            if (combatants[0].IsDead)
            {
                CancelCurrentIfPresent(tick);
                _terminal = true;
                return Publish(OrdanBossBalanceAuditPullOutcome.Suppressed(
                    tick,
                    true,
                    OrdanBossBalanceAuditPullDisposition.SuppressedBossDead,
                    expectedCurrentId));
            }

            if (combatants[1].IsDead)
            {
                CancelCurrentIfPresent(tick);
                return Publish(OrdanBossBalanceAuditPullOutcome.Suppressed(
                    tick,
                    false,
                    OrdanBossBalanceAuditPullDisposition.SuppressedPlayerDead,
                    expectedCurrentId));
            }

            var transferPhaseCommand = transferPublication.Input;
            if (IsLifecycleSuppression(transferPhaseCommand.LifecycleReason))
            {
                CancelCurrentIfPresent(tick);
                return Publish(OrdanBossBalanceAuditPullOutcome.Suppressed(
                    tick,
                    false,
                    OrdanBossBalanceAuditPullDisposition.SuppressedLifecycle,
                    expectedCurrentId));
            }

            var bridge = _bridge.LatestOutcome;
            var projection = bridge.BalanceAuditPullProjection;
            var interrupted = HasExactBalanceAuditInterrupt(bridge.Output, tick);
            if (interrupted)
            {
                CancelCurrentIfPresent(tick);
                return Publish(OrdanBossBalanceAuditPullOutcome.Interrupted(tick, expectedCurrentId));
            }

            if (!projection.HasValue)
            {
                if (_currentDirective.HasValue)
                {
                    CancelCurrentIfPresent(tick);
                    return Publish(OrdanBossBalanceAuditPullOutcome.Interrupted(tick, expectedCurrentId));
                }

                return Publish(OrdanBossBalanceAuditPullOutcome.NoProjection(tick));
            }

            ValidateProjection(bridge, projection.Value, tick, interrupted);
            if (projection.Value.ElapsedTick >= checked(projection.Value.FrozenDuration - 1))
            {
                _currentDirective = null;
                return Publish(OrdanBossBalanceAuditPullOutcome.FinalExecuteNoQueue(
                    tick,
                    projection.Value,
                    expectedCurrentId));
            }

            var deliveryTick = new SimulationTick(checked(tick.Value + 1));
            var next = new PullDirectiveIdentity(
                projection.Value.AttackOrdinal,
                deliveryTick,
                FormatDirectiveId(projection.Value.AttackOrdinal, deliveryTick));
            var latchKey = BuildLatchKey(next.AttackOrdinal, next.MovementTick);
            if (_committedLatches.Contains(latchKey))
            {
                throw new InvalidOperationException("BalanceAudit pull directive was already committed for this ordinal and delivery tick.");
            }

            var candidate = _player.PreflightExternalMovementDirective(
                this,
                next.AttackOrdinal,
                next.MovementTick,
                next.DirectiveId,
                _anchorXQ4096,
                _anchorYQ4096,
                _maximumStepQ4096);
            _player.CommitPreflightedExternalMovementDirective(this, candidate);
            if (!_committedLatches.Add(latchKey))
            {
                throw new InvalidOperationException("BalanceAudit pull latch could not commit exactly once.");
            }

            _currentDirective = next;
            return Publish(OrdanBossBalanceAuditPullOutcome.Queued(
                tick,
                projection.Value,
                expectedCurrentId,
                next.DirectiveId,
                next.MovementTick));
        }

        private void EnsureRegistered()
        {
            if (_registered)
            {
                return;
            }

            if (_player == null || _transfer == null || _combat == null || _bridge == null || _scheduler == null)
            {
                throw new InvalidOperationException("BalanceAudit pull producer requires all explicit graph bindings.");
            }

            if (_anchorXQ4096 != ApprovedAnchorXQ4096 || _anchorYQ4096 != ApprovedAnchorYQ4096 ||
                _maximumStepQ4096 != ApprovedMaximumStepQ4096)
            {
                throw new InvalidOperationException("BalanceAudit pull authored descriptor is not exact.");
            }

            _player.RegisterExternalMovementDirectiveProducer(this);
            _registered = true;
        }

        private void ValidateBindingsAndPublications(
            out SimulationTick tick,
            out IReadOnlyList<CombatantSnapshot> combatants,
            out TransferPhasePublication transferPublication)
        {
            EnsureRegistered();
            if (_combat.BoundPlayer != _player || _combat.BoundTransferDriver != _transfer ||
                _bridge.BoundTransferDriver != _transfer || _scheduler.BoundBridge != _bridge ||
                _scheduler.BoundTransferDriver != _transfer || !_bridge.HasLatestOutcome ||
                !_combat.HasLatestOutcome || !_scheduler.HasLatestOutcome ||
                !_scheduler.HasLatestAuditOutcome || !_transfer.LatestPhasePublication.HasValue)
            {
                throw new InvalidOperationException("BalanceAudit pull bindings or exact upstream publications are missing or cross-wired.");
            }

            var bridge = _bridge.LatestOutcome;
            tick = bridge.SourceTick;
            transferPublication = _transfer.LatestPhasePublication.Value;
            if (_combat.LatestOutcome.Tick != tick || transferPublication.Tick != tick ||
                _scheduler.LatestOutcome.SourceTick != tick || _scheduler.LatestAuditOutcome.SourceTick != tick)
            {
                throw new InvalidOperationException("BalanceAudit pull upstream publication is stale, skipped, or mismatched.");
            }

            var player = _player.Snapshot;
            var firstTick = _player.Chapter1ConfiguredFirstTick ?? new SimulationTick(0);
            if (tick == firstTick)
            {
                if (player.Tick != firstTick || _player.NextExpectedTick != firstTick)
                {
                    throw new InvalidOperationException("BalanceAudit pull tick-zero movement bootstrap is invalid.");
                }
            }
            else if (player.Tick.Value != checked(tick.Value - 1) || _player.NextExpectedTick != tick)
            {
                throw new InvalidOperationException("BalanceAudit pull requires the exact prior completed movement snapshot.");
            }

            combatants = _combat.LatestOutcome.Combatants;
            if (combatants.Count != 2 || combatants[0].TargetId != "ordan" || combatants[1].TargetId != "player")
            {
                throw new InvalidOperationException("BalanceAudit pull Combat roster is not exact.");
            }
        }

        private void ValidateCurrentDirectiveHorizon(SimulationTick tick)
        {
            if (_currentDirective.HasValue && _currentDirective.Value.MovementTick != tick)
            {
                throw new InvalidOperationException("BalanceAudit pull current directive is stale, skipped, or has the wrong horizon.");
            }
        }

        private void CancelCurrentIfPresent(SimulationTick tick)
        {
            if (!_currentDirective.HasValue)
            {
                return;
            }

            var current = _currentDirective.Value;
            _player.CancelExternalMovementDirective(this, current.AttackOrdinal, tick, current.DirectiveId);
            _currentDirective = null;
        }

        private static bool IsLifecycleSuppression(TransferClearReason? reason) =>
            reason == TransferClearReason.RoomLeaving || reason == TransferClearReason.RunFailed ||
            reason == TransferClearReason.Cutscene || reason == TransferClearReason.DemoCompleted;

        private static bool HasExactBalanceAuditInterrupt(OrdanBossTickOutput output, SimulationTick tick)
        {
            var count = 0;
            for (var index = 0; index < output.Intents.Count; index++)
            {
                var intent = output.Intents[index];
                if (intent.Kind != OrdanBossIntentKind.BalanceAuditInterrupt)
                {
                    continue;
                }

                if (intent.Tick != tick || intent.PatternSlot != OrdanPatternSlot.BalanceAudit || intent.DamageAmount != 0 ||
                    intent.DeliveryTick.HasValue || intent.FutureDamageRequest.HasValue)
                {
                    throw new InvalidOperationException("BalanceAudit interrupt intent is malformed.");
                }

                count++;
            }

            if (count > 1)
            {
                throw new InvalidOperationException("BalanceAudit has duplicate interrupt intents.");
            }

            return count == 1;
        }

        private static void ValidateProjection(
            OrdanBossBridgeOutcome bridge,
            BalanceAuditPullProjection projection,
            SimulationTick tick,
            bool interrupted)
        {
            var snapshot = bridge.Output.Snapshot;
            if (interrupted || projection.SourceTick != tick || bridge.SourceTick != tick ||
                snapshot.Tick != tick || snapshot.PatternSlot != OrdanPatternSlot.BalanceAudit ||
                snapshot.Phase != OrdanBossPhase.Execute || projection.ElapsedTick != snapshot.PhaseAge ||
                projection.ElapsedTick < 0 || projection.ElapsedTick >= projection.FrozenDuration ||
                (projection.FrozenDuration != 90 && projection.FrozenDuration != 82 && projection.FrozenDuration != 75) ||
                projection.AttackOrdinal < 0)
            {
                throw new InvalidOperationException("BalanceAudit pull projection is not the exact active M4A projection.");
            }
        }

        internal static string FormatDirectiveId(long attackOrdinal, SimulationTick deliveryTick)
        {
            if (attackOrdinal < 0 || deliveryTick.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attackOrdinal));
            }

            var id = string.Format(
                CultureInfo.InvariantCulture,
                "ordan.audit-pull/{0:D19}/{1:D10}",
                attackOrdinal,
                deliveryTick.Value);
            if (id.Length > 96)
            {
                throw new OverflowException("BalanceAudit pull directive ID exceeds its contract bound.");
            }

            return id;
        }

        private static string BuildLatchKey(long attackOrdinal, SimulationTick deliveryTick) =>
            string.Format(CultureInfo.InvariantCulture, "{0:D19}/{1:D10}", attackOrdinal, deliveryTick.Value);

        private OrdanBossBalanceAuditPullOutcome Publish(OrdanBossBalanceAuditPullOutcome outcome)
        {
            _latestOutcome = outcome;
            _hasLatestOutcome = true;
            return outcome;
        }

        private readonly struct PullDirectiveIdentity
        {
            internal PullDirectiveIdentity(long attackOrdinal, SimulationTick movementTick, string directiveId)
            {
                AttackOrdinal = attackOrdinal;
                MovementTick = movementTick;
                DirectiveId = directiveId;
            }

            internal long AttackOrdinal { get; }
            internal SimulationTick MovementTick { get; }
            internal string DirectiveId { get; }
        }
    }

    internal enum OrdanBossBalanceAuditPullDisposition
    {
        NoProjection = 0,
        Queued = 1,
        FinalExecuteNoQueue = 2,
        ZeroDistancePending = 3,
        Interrupted = 4,
        SuppressedBossDead = 5,
        SuppressedPlayerDead = 6,
        SuppressedLifecycle = 7
    }

    // REQ-MOV-001, REQ-MOV-005. Value-only source-tick outcome for +110.
    internal readonly struct OrdanBossBalanceAuditPullOutcome
    {
        private OrdanBossBalanceAuditPullOutcome(
            SimulationTick sourceTick,
            bool terminal,
            OrdanBossBalanceAuditPullDisposition disposition,
            long attackOrdinal,
            int elapsedTick,
            int frozenDuration,
            string cancelledCurrentDirectiveId,
            string expectedCurrentDirectiveId,
            string queuedNextDirectiveId,
            SimulationTick? queuedNextMovementTick)
        {
            SourceTick = sourceTick;
            Terminal = terminal;
            Disposition = disposition;
            AttackOrdinal = attackOrdinal;
            ElapsedTick = elapsedTick;
            FrozenDuration = frozenDuration;
            CancelledCurrentDirectiveId = cancelledCurrentDirectiveId ?? string.Empty;
            ExpectedCurrentDirectiveId = expectedCurrentDirectiveId ?? string.Empty;
            QueuedNextDirectiveId = queuedNextDirectiveId ?? string.Empty;
            QueuedNextMovementTick = queuedNextMovementTick;
        }

        internal SimulationTick SourceTick { get; }
        internal bool Terminal { get; }
        internal OrdanBossBalanceAuditPullDisposition Disposition { get; }
        internal long AttackOrdinal { get; }
        internal int ElapsedTick { get; }
        internal int FrozenDuration { get; }
        internal string CancelledCurrentDirectiveId { get; }
        internal string ExpectedCurrentDirectiveId { get; }
        internal string QueuedNextDirectiveId { get; }
        internal SimulationTick? QueuedNextMovementTick { get; }

        internal static OrdanBossBalanceAuditPullOutcome NoProjection(SimulationTick tick) =>
            new OrdanBossBalanceAuditPullOutcome(tick, false, OrdanBossBalanceAuditPullDisposition.NoProjection, -1, -1, -1, string.Empty, string.Empty, string.Empty, null);

        internal static OrdanBossBalanceAuditPullOutcome Queued(
            SimulationTick tick,
            BalanceAuditPullProjection projection,
            string expectedCurrentDirectiveId,
            string queuedNextDirectiveId,
            SimulationTick queuedNextTick) =>
            new OrdanBossBalanceAuditPullOutcome(tick, false, OrdanBossBalanceAuditPullDisposition.Queued,
                projection.AttackOrdinal, projection.ElapsedTick, projection.FrozenDuration, string.Empty,
                expectedCurrentDirectiveId, queuedNextDirectiveId, queuedNextTick);

        internal static OrdanBossBalanceAuditPullOutcome FinalExecuteNoQueue(
            SimulationTick tick,
            BalanceAuditPullProjection projection,
            string expectedCurrentDirectiveId) =>
            new OrdanBossBalanceAuditPullOutcome(tick, false, OrdanBossBalanceAuditPullDisposition.FinalExecuteNoQueue,
                projection.AttackOrdinal, projection.ElapsedTick, projection.FrozenDuration, string.Empty,
                expectedCurrentDirectiveId, string.Empty, null);

        internal static OrdanBossBalanceAuditPullOutcome Interrupted(SimulationTick tick, string cancelledCurrentDirectiveId) =>
            new OrdanBossBalanceAuditPullOutcome(tick, false, OrdanBossBalanceAuditPullDisposition.Interrupted, -1, -1, -1,
                cancelledCurrentDirectiveId, string.Empty, string.Empty, null);

        internal static OrdanBossBalanceAuditPullOutcome Suppressed(
            SimulationTick tick,
            bool terminal,
            OrdanBossBalanceAuditPullDisposition disposition,
            string cancelledCurrentDirectiveId) =>
            new OrdanBossBalanceAuditPullOutcome(tick, terminal, disposition, -1, -1, -1,
                cancelledCurrentDirectiveId, string.Empty, string.Empty, null);
    }
}
