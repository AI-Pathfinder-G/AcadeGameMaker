using System;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using UnityEngine;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-M5D1-002/003/004. This is a local Ordan sandbox input-lock
    // requester, not a Run, reward, scene, or gameplay-lifecycle authority.
    [DefaultExecutionOrder(-211)]
    [DisallowMultipleComponent]
    internal sealed class OrdanBossTerminalTransitionRequester : MonoBehaviour
    {
        internal const string EncounterKey = "OrdanBossEncounterGraph";
        private const string TransitionReason = "ordan-boss-terminal-input-lock";

        [SerializeField] private InputRouter _router;
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private OrdanBossCombatSimulationDriver _combat;
        [SerializeField] private OrdanBossEncounterHandoffAdapter _handoff;

        private bool _initialized;
        private bool _requestIssued;
        private bool _closed;
        private SimulationTick _deathTick;
        private SimulationTick _requestedTick;
        private OrdanBossTerminalTransitionReceipt? _latestCompletion;

        internal OrdanBossTerminalTransitionReceipt? LatestCompletionReceipt => _latestCompletion;
        internal bool IsClosed => _closed;

        // REQ-M5D1-001. Validator-only identity proof; it has no lifecycle
        // effect and does not register, initialize, or inspect runtime state.
        internal void ValidateForAuthoring(GameObject root, InputRouter router, PlayerMovementController player,
            OrdanBossCombatSimulationDriver combat, OrdanBossEncounterHandoffAdapter handoff)
        {
            if (_initialized || _requestIssued || _closed || root == null || _router != router || _player != player ||
                _combat != combat || _handoff != handoff || !ReferencesShareExactAuthoredGraph())
                throw new InvalidOperationException("Terminal transition requester authored bindings are not exact.");
        }

        // The builder and focused fixtures share this assignment-only path.
        internal void ConfigureForAuthoring(InputRouter router, PlayerMovementController player,
            OrdanBossCombatSimulationDriver combat, OrdanBossEncounterHandoffAdapter handoff)
        {
            if (_initialized || _requestIssued || _closed)
                throw new InvalidOperationException("Terminal transition requester cannot be reconfigured.");
            _router = router;
            _player = player;
            _combat = combat;
            _handoff = handoff;
        }

        private void Start()
        {
            if (_initialized || _closed) return;
            InitializeAtRuntime();
        }

        internal void InitializeForTests()
        {
            if (_initialized || _closed) throw new InvalidOperationException("Terminal transition requester is closed.");
            InitializeForExplicitAuthoring();
        }

        internal void AdvanceForTests()
        {
            if (!_initialized || _closed) throw new InvalidOperationException("Terminal transition requester is unavailable.");
            Advance();
        }

        // Runtime prefab instances normally have Unity's "(Clone)" suffix.
        // They retain the same serialized graph but are not authoring roots.
        // A malformed runtime graph closes silently rather than leaking a
        // Start exception or acquiring requester authority.
        private void InitializeAtRuntime()
        {
            if (!ReferencesShareRuntimeGraph())
            {
                _closed = true;
                return;
            }
            try
            {
                _router.RegisterModeRequester(this);
                _initialized = true;
            }
            catch
            {
                _closed = true;
            }
        }

        private void InitializeForExplicitAuthoring()
        {
            if (_initialized || _closed) throw new InvalidOperationException("Terminal transition requester has a closed lifecycle.");
            ValidateAuthoringGraph();
            try
            {
                _router.RegisterModeRequester(this);
                _initialized = true;
            }
            catch
            {
                _closed = true;
                throw;
            }
        }

        private void FixedUpdate()
        {
            if (_initialized && !_closed) Advance();
        }

        private void Advance()
        {
            if (_requestIssued)
            {
                ObserveRequestedReceipt();
                return;
            }

            if (!TryReadExactTerminalEvidence(out var sourceTick, out var requestTick)) return;
            try
            {
                _router.RequestMode(this, InputMode.Transition, TransitionReason, requestTick);
                _deathTick = sourceTick;
                _requestedTick = requestTick;
                _requestIssued = true;
            }
            catch
            {
                // A foreign/pending/faulted router is not repaired or retried.
                _closed = true;
            }
        }

        private bool TryReadExactTerminalEvidence(out SimulationTick sourceTick, out SimulationTick requestTick)
        {
            sourceTick = default(SimulationTick);
            requestTick = default(SimulationTick);
            if (_router == null || _player == null || _combat == null || _handoff == null || _router.IsFaulted ||
                !ReferencesShareRuntimeGraph()) return false;

            var view = _handoff.LatestPresentationView;
            var prior = _router.CurrentReceipt;
            if (!view.HasValue || !prior.HasValue || !view.Value.IsTerminal || !IsExactStableRoster(view.Value) ||
                !IsExactTerminalTriplet(view.Value) || !_combat.HasLatestOutcome || !_combat.HasCompletedBossDeath)
                return false;

            sourceTick = view.Value.Tick;
            if (sourceTick.Value < 0 || _combat.LatestOutcome.Tick != sourceTick || prior.Value.Mode != InputMode.GameplayEnabled ||
                prior.Value.Tick != sourceTick) return false;
            try { requestTick = new SimulationTick(checked(sourceTick.Value + 1)); }
            catch (OverflowException) { _closed = true; return false; }
            return _player.NextExpectedTick == requestTick;
        }

        private void ObserveRequestedReceipt()
        {
            if (_router.IsFaulted)
            {
                _closed = true;
                return;
            }
            var receipt = _router.CurrentReceipt;
            if (!receipt.HasValue) return;
            // A direct test seam (or an accidental duplicate callback) may
            // observe before -210 has committed the requested tick. That is
            // not a second request and cannot close/relabel the proof.
            if (receipt.Value.Tick.Value < _requestedTick.Value) return;
            if (receipt.Value.Tick != _requestedTick || receipt.Value.Mode != InputMode.Transition ||
                receipt.Value.ModeEpoch <= 0)
            {
                _closed = true;
                return;
            }
            _latestCompletion = new OrdanBossTerminalTransitionReceipt(_deathTick, _requestedTick, receipt.Value);
            _closed = true;
        }

        // This intentionally validates only serialized identity and published
        // immutable evidence. It never searches for, initializes, or repairs an
        // encounter owner.
        private bool ReferencesShareExactAuthoredGraph()
        {
            var root = transform.root;
            return root != null && root.gameObject.name == EncounterKey && ReferencesShareRuntimeGraph();
        }

        private bool ReferencesShareRuntimeGraph()
        {
            if (_router == null || _player == null || _combat == null || _handoff == null) return false;
            var root = transform.root;
            var transfer = _router.BoundTransferForAuthoring;
            return root != null && _router.transform.root == root &&
                _player.transform.root == root && _combat.transform.root == root && _handoff.transform.root == root &&
                _router.BoundPlayerForAuthoring == _player && _router.BoundCombatForAuthoring == _combat &&
                transfer != null && transfer.transform.root == root && transfer.BoundPlayer == _player &&
                _router.AuthoredInitialMode == InputMode.GameplayEnabled && _combat.BoundPlayer == _player;
        }

        private void ValidateAuthoringGraph()
        {
            if (!ReferencesShareExactAuthoredGraph())
                throw new InvalidOperationException("Terminal transition requester bindings are cross-wired.");
        }

        private static bool IsExactStableRoster(OrdanBossEncounterPresentationView view)
        {
            var ids = view.StableIds;
            return ids.Count == 6 && ids[0] == "ordan" && ids[1] == "player" && ids[2] == "BossAuditBox-0" &&
                ids[3] == "BossWeight-0" && ids[4] == "BossWeight-1" && ids[5] == "BossWeight-2";
        }

        private static bool IsExactTerminalTriplet(OrdanBossEncounterPresentationView view)
        {
            return view.Handoffs.Count == 3 && view.Handoffs[0].Kind == OrdanBossHandoffKind.Defeated &&
                view.Handoffs[1].Kind == OrdanBossHandoffKind.RewardRequest &&
                view.Handoffs[2].Kind == OrdanBossHandoffKind.RoomCompletionRequest &&
                view.Handoffs[0].Tick == view.Tick && view.Handoffs[1].Tick == view.Tick && view.Handoffs[2].Tick == view.Tick;
        }

        private void OnDisable()
        {
            if (_initialized && !_latestCompletion.HasValue) _closed = true;
        }

        private void OnEnable()
        {
            if (_initialized) throw new InvalidOperationException("Terminal transition requester cannot re-enable.");
        }
    }

    // REQ-M5D1-004. Immutable proof of the router's final authority; this is
    // deliberately not a Run-success or room-completion receipt.
    internal readonly struct OrdanBossTerminalTransitionReceipt
    {
        internal OrdanBossTerminalTransitionReceipt(SimulationTick deathSourceTick, SimulationTick requestedTick,
            InputFrameCommitReceipt committedReceipt)
        {
            if (deathSourceTick.Value < 0 || requestedTick.Value != checked(deathSourceTick.Value + 1) ||
                committedReceipt.Tick != requestedTick || committedReceipt.Mode != InputMode.Transition)
                throw new ArgumentException("Terminal transition receipt is malformed.");
            DeathSourceTick = deathSourceTick;
            RequestedTick = requestedTick;
            CommittedTick = committedReceipt.Tick;
            Mode = committedReceipt.Mode;
            ModeEpoch = committedReceipt.ModeEpoch;
            EncounterKey = OrdanBossTerminalTransitionRequester.EncounterKey;
        }

        internal SimulationTick DeathSourceTick { get; }
        internal SimulationTick RequestedTick { get; }
        internal SimulationTick CommittedTick { get; }
        internal InputMode Mode { get; }
        internal long ModeEpoch { get; }
        internal string EncounterKey { get; }
    }
}
