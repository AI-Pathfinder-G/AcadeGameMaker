using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-003, REQ-COM-004, REQ-WT-005. The boss-only -185 bridge
    // forwards completed -200/-190 values into M4A and the t+1 M1 lane.
    [DefaultExecutionOrder(-185)]
    [DisallowMultipleComponent]
    internal sealed class OrdanBossSimulationDriver : MonoBehaviour
    {
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private TransferSimulationDriver _transferDriver;
        [SerializeField] private OrdanBossCombatSimulationDriver _combatDriver;

        private OrdanBossSession _session;
        private OrdanBossSession _stagedSession;
        private bool _prepared;
        private bool _initialized;
        private bool _hasLatestOutcome;
        private OrdanBossBridgeOutcome _latestOutcome;
        private DamageRequest? _pendingPayload;

        internal bool HasLatestOutcome => _hasLatestOutcome;
        internal OrdanBossBridgeOutcome LatestOutcome => _hasLatestOutcome ? _latestOutcome : throw new InvalidOperationException("Boss bridge has not published.");
        // REQ-COM-003, REQ-WT-005. M4B2 verifies this identity only; it never
        // asks the bridge to replay or mutate Transfer.
        internal TransferSimulationDriver BoundTransferDriver => _transferDriver;

        private void Awake()
        {
            // Serialized production bindings are complete before Awake. Focused
            // tests configure an otherwise blank inactive component explicitly.
            if (_player != null || _transferDriver != null || _combatDriver != null) Initialize();
        }
        private void FixedUpdate() => AdvanceBridgePhase();

        internal void ConfigureForTests(PlayerMovementController player, TransferSimulationDriver transferDriver, OrdanBossCombatSimulationDriver combatDriver) =>
            Configure(player, transferDriver, combatDriver);
        private void Configure(PlayerMovementController player, TransferSimulationDriver transferDriver, OrdanBossCombatSimulationDriver combatDriver)
        {
            if (_prepared || _initialized) throw new InvalidOperationException("Boss bridge cannot be reconfigured after preparation.");
            _player = player; _transferDriver = transferDriver; _combatDriver = combatDriver;
        }

        // REQ-COM-001, REQ-COM-004. Authoring assigns only serialized graph
        // references; Awake remains the sole bootstrap owner.
        internal void ConfigureForAuthoring(PlayerMovementController player, TransferSimulationDriver transferDriver, OrdanBossCombatSimulationDriver combatDriver) =>
            Configure(player, transferDriver, combatDriver);

        internal void InitializeForTests() => Initialize();
        internal OrdanBossBridgeOutcome AdvanceBridgePhaseForTests() => AdvanceBridgePhase();

        // REQ-COM-004. Awake performs the one-time private lane transaction
        // before any -190 FixedUpdate; it has no scene discovery fallback.
        private void Initialize()
        {
            if (_prepared || _initialized) return;
            if (_player == null || _transferDriver == null || _combatDriver == null || _combatDriver.BoundPlayer != _player ||
                _combatDriver.BoundTransferDriver != _transferDriver || _combatDriver.BoundRegistry == null)
                throw new InvalidOperationException("Boss bridge requires exact shared player, Transfer, Combat, and registry bindings.");
            var first = _player.NextExpectedTick;
            // All newly-created engine-free state is staged before either owner
            // exposes a latest publication. Registration itself is one-time.
            var stagedSession = new OrdanBossSession(first);
            var roster = _combatDriver.PrepareBossDeliveryRegistration();
            if (roster.Count != 2 || roster[0].TargetId != "ordan" || roster[1].TargetId != "player") throw new InvalidOperationException("Boss bridge requires the exact Combat roster.");
            _combatDriver.RegisterBossDeliveryLane(first);
            _stagedSession = stagedSession; _prepared = true;
        }

        private OrdanBossBridgeOutcome AdvanceBridgePhase()
        {
            Initialize();
            try
            {
                var tick = _player.NextExpectedTick;
                var transfer = ExactTransferPublication(tick);
                var combat = ExactCombatPublication(tick);
                var combatState = ExtractCombatState(tick, combat, _pendingPayload);
                var edge = ProjectRelevantEdge(tick, transfer.Outcome.PressResult);
                var input = new OrdanBossTickInput(tick, combatState, edge);
                var workingSession = _initialized ? _session : _stagedSession;
                if (workingSession == null) throw new InvalidOperationException("Boss bridge has no staged or adopted M4A session.");
                var candidate = workingSession.PreviewNext(input);
                var futurePayload = ExtractExactFuturePayload(tick, candidate.Output);
                var delivery = _combatDriver.PreflightBossDelivery(tick, futurePayload);
                var stagedOutcome = new OrdanBossBridgeOutcome(tick, candidate.Output, candidate.Output.TransferExposureForecast, candidate.Output.AuditExposureForecast, candidate.Output.SeizureTrajectoryProjection, candidate.Output.BalanceAuditPullProjection, candidate.Output.Handoffs);
                if (!_combatDriver.HasLatestOutcome) _combatDriver.PreflightBootstrapPromotion(tick);
                // M4A candidate, t+1 delivery, bootstrap promotion and immutable
                // bridge publication are preflighted before either local owner commits.
                var output = workingSession.CommitNext(candidate, input);
                _combatDriver.CommitPreflightedBossDelivery(delivery);
                if (!_combatDriver.HasLatestOutcome) _combatDriver.PromoteBootstrap(tick);
                if (!_initialized)
                {
                    _session = workingSession;
                    _stagedSession = null;
                    _initialized = true;
                }
                _pendingPayload = futurePayload;
                _ = output; // CommitNext has already proven exact candidate equality.
                _latestOutcome = stagedOutcome; _hasLatestOutcome = true;
                return stagedOutcome;
            }
            catch
            {
                // REQ-COM-004. A rejection during the first pair leaves no
                // externally readable Combat publication or adopted sessions.
                if (!_combatDriver.HasLatestOutcome)
                {
                    _combatDriver.DiscardFailedBootstrap();
                    _stagedSession = null;
                }
                throw;
            }
        }

        private TransferPhasePublication ExactTransferPublication(SimulationTick tick)
        {
            var publication = _transferDriver.LatestPhasePublication;
            if (!publication.HasValue || publication.Value.Tick != tick) throw new InvalidOperationException("Boss bridge requires the exact current Transfer publication.");
            return publication.Value;
        }
        private OrdanBossCombatOutcome ExactCombatPublication(SimulationTick tick)
        {
            if (!_combatDriver.HasCurrentOutcome || _combatDriver.CurrentOutcomeForBridge.Tick != tick) throw new InvalidOperationException("Boss bridge requires the exact current Combat publication.");
            return _combatDriver.CurrentOutcomeForBridge;
        }

        private static OrdanCombatState ExtractCombatState(SimulationTick tick, OrdanBossCombatOutcome outcome, DamageRequest? pending)
        {
            if (outcome.Combatants.Count != 2 || outcome.Combatants[0].TargetId != "ordan" || outcome.Combatants[1].TargetId != "player")
                throw new InvalidOperationException("Boss Combat snapshot roster is not exact.");
            var ordan = outcome.Combatants[0]; var player = outcome.Combatants[1];
            if (ordan.CurrentHealth < 0 || ordan.CurrentHealth > 60 || ordan.IsDead != (ordan.CurrentHealth == 0) ||
                player.CurrentHealth < 0 || player.CurrentHealth > 5 || player.IsDead != (player.CurrentHealth == 0))
                throw new InvalidOperationException("Boss Combat snapshot health contract is invalid.");
            DamageResult? result = null;
            if (pending.HasValue)
            {
                var count = 0;
                for (var index = 0; index < outcome.CanonicalRequestTrace.Count; index++)
                {
                    var request = outcome.CanonicalRequestTrace[index]; var candidate = outcome.DamageResults[index];
                    if (request.RequestId != candidate.RequestId || request.TargetId != candidate.TargetId || request.Tick != candidate.Tick || request.Tick != tick)
                        throw new InvalidOperationException("Boss Combat canonical evidence is altered.");
                    if (request.RequestId == pending.Value.RequestId)
                    {
                        if (!request.Equals(pending.Value)) throw new InvalidOperationException("Pending payload request evidence is altered.");
                        result = candidate; count++;
                    }
                }
                if (count != 1) throw new InvalidOperationException("Pending payload request/result evidence must occur exactly once.");
            }
            return new OrdanCombatState(tick, "ordan", ordan.CurrentHealth, ordan.IsDead, result);
        }

        // REQ-COM-004, REQ-WT-005. The public immutable attempt value is the
        // narrowest seam required for projection; publication ownership/tick
        // remains validated by the caller before this method is reached.
        private static OrdanRelevantTransferEdge? ProjectRelevantEdge(SimulationTick tick, TransferAttemptResult? press)
        {
            if (!press.HasValue || !press.Value.Success || !press.Value.StateChanged.HasValue) return null;
            var changed = press.Value.StateChanged.Value;
            if (press.Value.Tick != tick || changed.Tick != tick || press.Value.SelectedTargetId != changed.TargetId ||
                changed.PlayerModifierId != "Player.Lightweight.v1" || changed.Revision <= 0) throw new InvalidOperationException("Transfer state-change proof is malformed.");
            OrdanTransferTargetKind kind;
            if (changed.TargetId == "BossWeight-0" || changed.TargetId == "BossWeight-1" || changed.TargetId == "BossWeight-2")
            {
                if (changed.TargetModifierId != "Target.BossPayload.ScriptedHeavy.v1") throw new InvalidOperationException("Boss payload modifier proof is mismatched.");
                kind = OrdanTransferTargetKind.BossPayload;
            }
            else if (changed.TargetId == "BossAuditBox-0")
            {
                if (changed.TargetModifierId != "Target.Box.Heavy.v1") throw new InvalidOperationException("Boss audit box modifier proof is mismatched.");
                kind = OrdanTransferTargetKind.Box;
            }
            else return null;
            return new OrdanRelevantTransferEdge(tick, changed.TargetId, kind, OrdanTransferState.Baseline, OrdanTransferState.Heavy, changed.Revision);
        }

        private static DamageRequest? ExtractExactFuturePayload(SimulationTick tick, OrdanBossTickOutput output)
        {
            DamageRequest? payload = null;
            for (var index = 0; index < output.Intents.Count; index++)
            {
                var intent = output.Intents[index];
                if (intent.Kind != OrdanBossIntentKind.PayloadDamageRequest) continue;
                if (payload.HasValue || !intent.FutureDamageRequest.HasValue || !intent.DeliveryTick.HasValue || intent.DeliveryTick.Value.Value != checked(tick.Value + 1))
                    throw new InvalidOperationException("M4A payload intent must be one exact next-tick request.");
                var request = intent.FutureDamageRequest.Value;
                if (request.Tick != intent.DeliveryTick.Value || request.RequestId != intent.Id || request.TargetId != "ordan" || request.Amount != 8 || request.Kind != DamageKind.BossPayloadTransfer)
                    throw new InvalidOperationException("M4A payload intent is malformed.");
                payload = request;
            }
            return payload;
        }
    }

    // REQ-COM-003, REQ-COM-004. Immutable bridge-only read model; later M4B
    // stages may consume it but cannot mutate M4A, Combat, or Transfer state.
    internal readonly struct OrdanBossBridgeOutcome
    {
        internal OrdanBossBridgeOutcome(SimulationTick sourceTick, OrdanBossTickOutput output, OrdanTransferExposureForecast forecast, OrdanAuditExposureForecast auditForecast, IReadOnlyList<OrdanBossHandoff> handoffs)
            : this(sourceTick, output, forecast, auditForecast, output.SeizureTrajectoryProjection, output.BalanceAuditPullProjection, handoffs) { }
        internal OrdanBossBridgeOutcome(SimulationTick sourceTick, OrdanBossTickOutput output, OrdanTransferExposureForecast forecast, OrdanAuditExposureForecast auditForecast, SeizureTrajectoryProjection? seizureTrajectoryProjection, IReadOnlyList<OrdanBossHandoff> handoffs)
            : this(sourceTick, output, forecast, auditForecast, seizureTrajectoryProjection, output.BalanceAuditPullProjection, handoffs) { }
        internal OrdanBossBridgeOutcome(SimulationTick sourceTick, OrdanBossTickOutput output, OrdanTransferExposureForecast forecast, OrdanAuditExposureForecast auditForecast, SeizureTrajectoryProjection? seizureTrajectoryProjection, BalanceAuditPullProjection? balanceAuditPullProjection, IReadOnlyList<OrdanBossHandoff> handoffs)
        {
            if (output.Snapshot.Tick != sourceTick || !forecast.Equals(output.TransferExposureForecast) || !auditForecast.Equals(output.AuditExposureForecast) || !Nullable.Equals(seizureTrajectoryProjection, output.SeizureTrajectoryProjection) || !Nullable.Equals(balanceAuditPullProjection, output.BalanceAuditPullProjection) || handoffs == null) throw new ArgumentException("Bridge output must retain exact M4A values.");
            SourceTick = sourceTick; Output = output; Forecast = forecast; AuditForecast = auditForecast; SeizureTrajectoryProjection = seizureTrajectoryProjection; BalanceAuditPullProjection = balanceAuditPullProjection;
            var copy = new OrdanBossHandoff[handoffs.Count]; for (var index = 0; index < copy.Length; index++) copy[index] = handoffs[index]; Handoffs = Array.AsReadOnly(copy);
        }
        internal SimulationTick SourceTick { get; } internal OrdanBossTickOutput Output { get; }
        internal OrdanTransferExposureForecast Forecast { get; } internal OrdanAuditExposureForecast AuditForecast { get; } internal SeizureTrajectoryProjection? SeizureTrajectoryProjection { get; } internal BalanceAuditPullProjection? BalanceAuditPullProjection { get; } internal IReadOnlyList<OrdanBossHandoff> Handoffs { get; }
    }
}
