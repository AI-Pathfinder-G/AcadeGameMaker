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

        // REQ-LPS-003/005/006. Opt-in local result; legacy authority remains unchanged.
        [SerializeField] private bool _localPlayable;
        private bool _localConfigured, _localStopped, _localHasExactBossTerminal;
        private LocalPlayableSliceView? _localView, _localPending;
        private SimulationTick? _localUiRequestTick;
        internal bool LocalPlayableEnabled => _localPlayable;
        internal LocalPlayableSliceView? LatestLocalPlayableView => _localView;
        internal void ConfigureLocalPlayableForAuthoring(bool enabled)
        {
            if (_chapterOwner!=null || _localConfigured || _initialized || _requestIssued || _closed)
                throw new InvalidOperationException("Local playable configuration must be pristine and single-use.");
            _localConfigured = true; _localPlayable = enabled;
        }
        private Chapter1EncounterLifetimeOwner _chapterOwner;
        private Chapter1EncounterEntry _chapterEntry;
        private IChapter1BossProgressionPort _chapterPort;
        private OrdanBossTerminalTeardown _chapterTeardown;
        private readonly object _chapterPublicationToken=new object();
        private Chapter1BossPublication _chapterDeath,_chapterCleanup;
        private Chapter1AcceptedFailureToken _chapterFailure;
        private InputFrameCommitReceipt? _chapterTransition;
        internal void ConfigureChapter1ProgressionForAuthoring(Chapter1EncounterLifetimeOwner owner,Chapter1EncounterEntry entry,
            IChapter1BossProgressionPort port,OrdanBossTerminalTeardown teardown)
        {
            if(gameObject.activeInHierarchy || _chapterOwner!=null || _initialized || _requestIssued || _closed || _localConfigured || _localPlayable ||
                owner==null || port==null || teardown==null || teardown.transform.root!=transform.root || !ReferencesShareRuntimeGraph() ||
                !ReferenceEquals(entry.Issuer,owner) || !owner.IsIssuedEntry(_router,entry) || entry.Generation<=0 ||
                _player.Chapter1ConfiguredFirstTick!=entry.FirstTick || _router.BoundTransferForAuthoring.Chapter1ConfiguredFirstTick!=entry.FirstTick ||
                _combat.Chapter1ConfiguredCarryTick!=entry.FirstTick)throw new InvalidOperationException("CH1 보스 비활성 결속이 다릅니다.");
            _chapterOwner=owner;_chapterEntry=entry;_chapterPort=port;_chapterTeardown=teardown;
        }
        internal bool TryGetChapter1Publication(out Chapter1BossPublication publication)
        {publication=_chapterCleanup??_chapterDeath;return publication!=null && OwnsChapter1Publication(publication);}
        internal bool OwnsChapter1Publication(Chapter1BossPublication publication)=>publication!=null &&
            (ReferenceEquals(publication,_chapterDeath)||ReferenceEquals(publication,_chapterCleanup)) && ReferenceEquals(publication.Issuer,this) &&
            ReferenceEquals(publication.Token,_chapterPublicationToken) && publication.RunKey==_chapterEntry.RunKey && publication.Generation==_chapterEntry.Generation &&
            _chapterOwner!=null && _chapterOwner.IsIssuedEntry(_router,_chapterEntry);
        internal void ObserveChapter1Completed(Chapter1EncounterEvidenceView completed)
        {
            if(_chapterOwner==null || !_initialized || !_chapterOwner.IsIssuedEntry(_router,_chapterEntry) || completed.RunKey!=_chapterEntry.RunKey ||
                completed.Generation!=_chapterEntry.Generation)throw new InvalidOperationException("CH1 완료 requester 원문이 다릅니다.");
            var receipt=_router.CurrentReceipt;
            if(_requestIssued && receipt.HasValue && receipt.Value.Tick==_requestedTick && receipt.Value.Mode==InputMode.Transition && receipt.Value.ModeEpoch>0)
            {
                _chapterTransition=receipt.Value;
                if(_chapterFailure==null)ObserveRequestedReceipt();
            }
            if(_chapterDeath==null && TryReadExactTerminalEvidence(out var death,out var next) && completed.Tick==death &&
                completed.Receipt.Equals(_router.CurrentReceipt.Value) && _router.TryGetChapter1CompletedEvidence(out var actual) && actual.Receipt.Equals(completed.Receipt))
            {
                var view=_handoff.LatestPresentationView.Value;
                if(view.Combatants.Count!=2 || view.Combatants[0].TargetId!="ordan" || view.Combatants[1].TargetId!="player" ||
                    view.Combatants[1].CurrentHealth!=completed.Health)throw new InvalidOperationException("보스 사망 roster가 다릅니다.");
                _chapterDeath=new Chapter1BossPublication(this,_chapterPublicationToken,completed.RunKey,completed.Generation,death,
                    view.Combatants[0].CurrentHealth,view.Combatants[0].IsDead,view.Combatants[1].CurrentHealth,view.Combatants[1].IsDead,
                    new[]{(int)view.Handoffs[0].Kind,(int)view.Handoffs[1].Kind,(int)view.Handoffs[2].Kind},null,false,null,false,false,false,false,false);
            }
            if(_chapterDeath==null || _chapterCleanup!=null || !_chapterTransition.HasValue)return;
            var cleanup=_router.BoundTransferForAuthoring.LatestBossTerminalCleanupReceipt;var teardown=_chapterTeardown.LatestReceipt;
            if(!cleanup.HasValue || !teardown.HasValue)return;
            var c=cleanup.Value;var t=teardown.Value;
            if(c.DeathSourceTick!=_chapterDeath.DeathTick || c.CleanupTick.Value!=checked(c.DeathSourceTick.Value+1) ||
                _chapterTransition.Value.Tick!=c.CleanupTick || _chapterTransition.Value.Mode!=InputMode.Transition || t.DeathSourceTick!=c.DeathSourceTick ||
                t.CleanupTick!=c.CleanupTick || !t.Cleanup.Equals(c))throw new InvalidOperationException("실제 보스 정리 receipt가 다릅니다.");
            Chapter1LifecycleKind? lifecycle=null;
            if(c.IsLifecycleSuperseded)
            {
                if(c.LifecycleReason==AcadeGameMaker.Transfer.TransferClearReason.RunFailed)lifecycle=Chapter1LifecycleKind.RunFailed;
                else if(c.LifecycleReason==AcadeGameMaker.Transfer.TransferClearReason.RoomLeaving)lifecycle=Chapter1LifecycleKind.RoomLeaving;
                else throw new InvalidOperationException("CH1 외 lifecycle 정리입니다.");
                if(!_chapterOwner.TryGetChapter1LifecyclePublication(lifecycle.Value,out var clear) || !clear.LockedReceipt.Equals(_chapterTransition.Value))
                    throw new InvalidOperationException("실제 우선 lifecycle publication이 없습니다.");
            }
            // BuildBossTerminalCleanupReceipt는 실제 session HasNoRegistrations를 검사한 뒤에만 발행한다.
            var removals=_router.BoundTransferForAuthoring.LatestCompletedInputRemovalIds;
            var absent=removals.Count==c.RequestedIds.Count && removals.Count==4;
            for(var i=0;i<removals.Count && absent;i++)if(removals[i]!=c.RequestedIds[i])absent=false;
            var disabled=t.DisabledComponents.Count==7 && t.DisabledColliders.Count==5 && !_combat.enabled && !_handoff.enabled && !_chapterTeardown.enabled;
            _chapterCleanup=new Chapter1BossPublication(this,_chapterPublicationToken,_chapterDeath.RunKey,_chapterDeath.Generation,_chapterDeath.DeathTick,
                _chapterDeath.BossHealth,_chapterDeath.BossDead,_chapterDeath.PlayerHealth,_chapterDeath.PlayerDead,_chapterDeath.OrderedHandoffs,c.CleanupTick,
                c.IsLifecycleSuperseded,lifecycle,absent,c.DurableRemovalCommitted,c.PlayerState==AcadeGameMaker.Transfer.TransferPlayerState.Baseline,
                string.IsNullOrEmpty(c.ActiveTargetAfter),disabled);
        }
        internal void RequestChapter1AcceptedFailure(Chapter1EncounterLifetimeOwner owner,Chapter1AcceptedFailureToken token)
        {
            if(_chapterOwner==null || !ReferenceEquals(owner,_chapterOwner) || !owner.OwnsPendingChapter1Failure(token) ||
                !owner.ValidateChapter1FailureSources(token) || !ValidateChapter1FailureSources(token) || !ReferenceEquals(token.Router,_router) || token.RunKey!=_chapterEntry.RunKey ||
                token.Generation!=_chapterEntry.Generation || _chapterFailure!=null || _requestIssued || _closed || !_initialized)
                throw new InvalidOperationException("보스 실패 token이 현재 owner의 pending 원문이 아닙니다.");
            var next=new SimulationTick(checked(token.SourceReceipt.Tick.Value+1));
            // REQ/AC-AUTOCH1-BOSSTERMINALMERGE-001: scheduler removal reservation precedes same-tick accepted failure.
            var simultaneousDeath=_chapterDeath!=null && OwnsChapter1Publication(_chapterDeath) && _chapterDeath.DeathTick==token.SourceReceipt.Tick && _chapterDeath.BossDead && _chapterDeath.PlayerDead;
            if(simultaneousDeath)_router.BoundTransferForAuthoring.MergeExactBossTerminalRunFailed(_router,next);
            else _router.BoundTransferForAuthoring.Submit(new AcadeGameMaker.Transfer.Unity.TransferSimulationInput(next,null,null,null,
                Array.Empty<AcadeGameMaker.Transfer.TransferTargetRemoved>(),AcadeGameMaker.Transfer.TransferClearReason.RunFailed));
            _router.RequestMode(this,InputMode.Transition,"chapter1-boss-run-failed",next);
            _chapterFailure=token;_deathTick=token.SourceReceipt.Tick;_requestedTick=next;_requestIssued=true;
        }
        private bool ValidateChapter1FailureSources(Chapter1AcceptedFailureToken token)
        {
            if(token==null || !ReferenceEquals(token.Issuer,_chapterPort) || !ReferenceEquals(token.Consumer,_chapterOwner) ||
                !_router.TryGetChapter1CompletedEvidence(out var completed) || !token.SourceReceipt.Equals(completed.Receipt) ||
                token.RunKey!=completed.RunKey || token.Generation!=completed.Generation || !ReferenceEquals(token.Health,completed.HealthPublication) ||
                !ReferenceEquals(token.Health.Issuer,_combat) || !ReferenceEquals(token.Health.Token,completed.HealthProducerToken) ||
                !_router.OwnsChapter1HealthPublication(token.Health) || token.Health.Triggered!=(completed.Health==0) ||
                !_chapterOwner.CurrentHazards.TryGetCompleted(completed,out var kill,out var crush) || !ReferenceEquals(token.Kill,kill) ||
                !ReferenceEquals(token.Crush,crush) || !_chapterOwner.CurrentHazards.Owns(kill) || !_chapterOwner.CurrentHazards.Owns(crush))return false;
            var sources=new[]{token.Health,kill,crush};
            for(var i=0;i<sources.Length;i++)if(sources[i].Kind!=(Chapter1FailureSourceKind)i || !ReferenceEquals(sources[i].Router,_router) ||
                sources[i].RunKey!=_chapterEntry.RunKey || sources[i].Generation!=_chapterEntry.Generation || !sources[i].Receipt.Equals(token.SourceReceipt))return false;
            for(var i=0;i<sources.Length;i++)if(sources[i].Triggered)return token.Cause==(Chapter1FailureSourceKind)i;
            return false;
        }
        private void AdvanceChapter1()
        {
            if(_requestIssued)return; // 실제 receipt 완료는 +200에서 관측한다.
            if(_chapterDeath==null || !_chapterPort.BlocksNewFailureRequests || _chapterFailure!=null ||
                !OwnsChapter1Publication(_chapterDeath) || !TryReadExactTerminalEvidence(out var source,out var next) || source!=_chapterDeath.DeathTick)return;
            try{_router.RequestMode(this,InputMode.Transition,TransitionReason,next);_deathTick=source;_requestedTick=next;_requestIssued=true;}
            catch{_closed=true;}
        }
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
            if (_chapterOwner!=null || _initialized || _requestIssued || _closed)
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
            if (!ReferencesShareRuntimeGraph() || (_chapterOwner!=null && (!_chapterOwner.IsIssuedEntry(_router,_chapterEntry) || _chapterPort==null || _chapterTeardown==null)))
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
            if (_initialized && (_localPlayable ? !_localStopped : !_closed)) Advance();
        }

        private void Advance()
        {
            if(_chapterOwner!=null){AdvanceChapter1();return;}
            if (_localPlayable) { AdvanceLocalPlayable(); return; }
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

        private void AdvanceLocalPlayable()
        {
            if (_router == null || _router.IsFaulted || !ReferencesShareRuntimeGraph())
            { _closed = true; _localStopped = true; return; }
            try
            {
                if (_localPending.HasValue)
                {
                    var receipt = _router.CurrentReceipt;
                    if (!receipt.HasValue) return;
                    if (!_localUiRequestTick.HasValue)
                    {
                        if (receipt.Value.Tick.Value < _requestedTick.Value) return;
                        if (receipt.Value.Tick != _requestedTick || receipt.Value.Mode != InputMode.Transition || receipt.Value.ModeEpoch <= 0)
                            throw new InvalidOperationException("Local Transition receipt mismatch.");
                        var next = new SimulationTick(checked(_requestedTick.Value + 1));
                        // A committed receipt is not yet a completed player tick in a deterministic phase fixture.
                        if (_player.NextExpectedTick == _requestedTick) return;
                        if (_player.NextExpectedTick != next) throw new InvalidOperationException("Local UI tick mismatch.");
                        // Preserve the original boss-only triplet proof, including simultaneous deaths.
                        if (_localHasExactBossTerminal) ObserveRequestedReceipt();
                        if (_localPending.Value.Result == LocalPlayableSliceResult.Succeeded && !_latestCompletion.HasValue)
                            throw new InvalidOperationException("Boss success has no terminal receipt.");
                        _localView = _localPending;
                        _router.RequestMode(this, InputMode.UIOnly, "local-playable-result", next);
                        _localUiRequestTick = next;
                        _closed = false; // local owner still awaits the UIOnly receipt; legacy false remains closed.
                        return;
                    }
                    if (receipt.Value.Tick.Value < _localUiRequestTick.Value.Value) return;
                    if (receipt.Value.Tick != _localUiRequestTick.Value || receipt.Value.Mode != InputMode.UIOnly)
                        throw new InvalidOperationException("Local UI receipt mismatch.");
                    _localView = _localPending.Value.WithUiOnlyTick(receipt.Value.Tick);
                    _localStopped = true; _closed = true;
                    return;
                }
                var handoff = _handoff.LatestPresentationView;
                var prior = _router.CurrentReceipt;
                if (!handoff.HasValue || !prior.HasValue || !IsExactStableRoster(handoff.Value) || !_combat.HasLatestOutcome) return;
                var view = handoff.Value;
                if (view.Tick != _combat.LatestOutcome.Tick || view.Tick != prior.Value.Tick || prior.Value.Mode != InputMode.GameplayEnabled) return;
                if (view.Combatants.Count != 2 || view.Combatants[0].TargetId != "ordan" || view.Combatants[1].TargetId != "player")
                    throw new InvalidOperationException("Local health roster mismatch.");
                var result = view.Combatants[1].IsDead ? LocalPlayableSliceResult.Failed : LocalPlayableSliceResult.Playing;
                var exactBoss = TryReadExactTerminalEvidence(out var sourceTick, out var requestTick);
                if (exactBoss && result != LocalPlayableSliceResult.Failed) result = LocalPlayableSliceResult.Succeeded;
                var snapshot = new LocalPlayableSliceView(1, view.Tick, view.Combatants[1].CurrentHealth,
                    view.Combatants[0].CurrentHealth, view.ActiveTransferTargetId,
                    view.TransferPlayerState == AcadeGameMaker.Transfer.TransferPlayerState.Lightweight, result, null);
                if (result == LocalPlayableSliceResult.Playing) { _localView = snapshot; return; }
                // Result is published only after the exact Transition proof; source health is latched now.
                if (!exactBoss) { sourceTick = view.Tick; requestTick = new SimulationTick(checked(view.Tick.Value + 1)); }
                if (_player.NextExpectedTick != requestTick) throw new InvalidOperationException("Local terminal tick mismatch.");
                _router.RequestMode(this, InputMode.Transition, TransitionReason, requestTick);
                _deathTick = sourceTick; _requestedTick = requestTick; _requestIssued = true; _localPending = snapshot; _localHasExactBossTerminal = exactBoss;
            }
            catch { _closed = true; _localStopped = true; }
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
            if (_localPlayable) _localStopped = true;
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
