using System;
using System.Collections.Generic;
using AcadeGameMaker.Combat;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Profile;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace AcadeGameMaker.Input.Unity
{
    // REQ-UX-004/006/007/008/013. This is the only device callback owner. It
    // buffers render-time facts and atomically stages the next simulation frame.
    [DefaultExecutionOrder(-210)]
    [DisallowMultipleComponent]
    internal sealed class InputRouter : MonoBehaviour, IUiSemanticFrameSourceV1,
        GameInputActions.IGameplayActions, GameInputActions.IUIActions
    {
        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private TransferSimulationDriver _transfer;
        [SerializeField] private OrdanBossCombatSimulationDriver _combat;
        [SerializeField] private OrdanBossTerminalTeardown _terminalTeardown;
        [SerializeField] private MonoBehaviour _cameraProviderBehaviour;
        [SerializeField] private InputMode _initialMode = InputMode.Transition;
        // REQ-M5D7Q0-001. Missing serialized data remains false and therefore
        // preserves every historical full-simulation graph.
        [SerializeField] private bool _authoredHubUiOnly;
        // REQ-M5D7Q0-001/008. A second serialized intent marker prevents a
        // reflected clear of the public flag from restoring the legacy path.
        [SerializeField] private bool _hubUiOnlyIntentProof;

        private GameInputActions _actions;
        private IInputCameraSnapshotProvider _cameraProvider;
        private bool _initialized;
        private bool _combatRetired;
        // REQ-LPS-002/007: opt-in release/neutral barrier and actual disposal witness.
        [SerializeField] private bool _localRequireNeutralStart;
        private bool _localConfigured, _localNeutralPending;
        private uint _localEnableUpdate;
        private sealed class LocalClosureWitness { internal bool Closed; }
        private readonly LocalClosureWitness _localClosure = new LocalClosureWitness();
        private LocalPlayableInputLifetime _localLifetime;
        internal bool LocalPlayableRequiresNeutralStart => _localRequireNeutralStart;
        internal bool LocalPlayableStartReady => !_localNeutralPending;
        internal LocalPlayableInputLifetime LocalPlayableLifetime
        {
            get { if (_localLifetime == null) { var witness = _localClosure; _localLifetime = new LocalPlayableInputLifetime(() => witness.Closed); } return _localLifetime; }
        }
        internal void ConfigureLocalPlayableForAuthoring(bool requireNeutralStart)
        {
            if (_localConfigured || _initialized || _startEntered || _actions != null || IsHubUiOnlyVariant)
                throw new InvalidOperationException("Local playable configuration must be pristine and single-use.");
            _localConfigured = true; _localRequireNeutralStart = requireNeutralStart;
        }
        private bool ObserveLocalNeutralBoundary()
        {
            if (!_localNeutralPending) return false;
            // Read all bound controls, including the disabled UI map, through the sole wrapper.
            var update = UnityEngine.InputSystem.LowLevel.InputState.updateCount;
            if (update == _localEnableUpdate) return true;
            bool neutral = _actions.Gameplay.Get().enabled && _actions.Gameplay.Move.ReadValue<Vector2>() == Vector2.zero;
            foreach (var map in _actions.asset.actionMaps)
            foreach (var action in map.actions)
            {
                if (action.type == InputActionType.Button)
                    foreach (var control in action.controls)
                        if (control.EvaluateMagnitude() > .1f) neutral = false;
            }
            foreach (var control in _actions.Gameplay.Move.controls)
                if (control.EvaluateMagnitude() > 0f) neutral = false;
            foreach (var control in _actions.Gameplay.Aim.controls)
                if (control.EvaluateMagnitude() > 0f) neutral = false;
            foreach (var control in _actions.UI.Navigate.controls)
                if (control.EvaluateMagnitude() > 0f) neutral = false;
            ClearModeEpochBatch(); ClearCommittedUiBatch(false);
            if (neutral) { _localNeutralPending = false; _localEnableUpdate = update; }
            // Discard every edge from the observed release update; next update starts capture.
            return true;
        }
        [SerializeField] private bool _chapterRegular;
        [SerializeField] private CombatSimulationDriver _chapterCombat;
        private BarrierCombatSimulationDriver _chapterBarrier;
        private SimulationTick _barrierFirstTick;
        internal void ConfigureChapter1BarrierForAuthoring(PlayerMovementController player,TransferSimulationDriver transfer,BarrierCombatSimulationDriver combat,MonoBehaviour camera,SimulationTick firstTick)
        {
            if(gameObject.activeInHierarchy || _awakeEntered || _initialized || _chapterRegular || _combat!=null || _chapterBarrier!=null || player==null || transfer==null || combat==null || camera==null)
                throw new InvalidOperationException("봉쇄선 입력 구성이 유효하지 않습니다.");
            _player=player;_transfer=transfer;_chapterBarrier=combat;_cameraProviderBehaviour=camera;_barrierFirstTick=firstTick;_initialMode=InputMode.GameplayEnabled;
            BindChapter1ChoiceForAuthoring();
        }
        private bool _chapterEntryInstalled;
        private Chapter1EncounterEntry _chapterEntry;
        private Chapter1GameplayInputSink _chapterSink;
        private readonly object _chapterToken=new object();
        private sealed class ChapterClosureWitness { internal bool Destroyed,Closed; }
        private readonly ChapterClosureWitness _chapterClosure=new ChapterClosureWitness();
        private Chapter1InputLifetime _chapterLifetime;
        internal bool IsChapter1Regular => _chapterRegular;
        internal CombatSimulationDriver BoundChapter1Combat => _chapterCombat;
        internal Chapter1InputLifetime Chapter1Lifetime => _chapterLifetime ?? (_chapterLifetime=Chapter1InputLifetime.FromRouter(this));
        internal Func<bool> GetChapter1ClosureReader(){var witness=_chapterClosure;return ()=>witness.Destroyed && witness.Closed;}
        // REQ-C1B-INPUT-001/ENTRY-001: regular bindings and entry are separate inactive assignments.
        internal void ConfigureChapter1GameplayForAuthoring(PlayerMovementController player,TransferSimulationDriver transfer,CombatSimulationDriver combat,MonoBehaviour camera)
        {
            if(_chapterRegular || _initialized || _awakeEntered || _startEntered || _localRequireNeutralStart || _combat!=null ||
                _authoredHubUiOnly || player==null || transfer==null || combat==null || camera==null)
                throw new InvalidOperationException("Chapter regular bindings must be pristine and exclusive.");
            _chapterRegular=true;_player=player;_transfer=transfer;_chapterCombat=combat;_cameraProviderBehaviour=camera;_initialMode=InputMode.GameplayEnabled;
        }
        internal void ConfigureChapter1GameplayForAuthoring(Chapter1EncounterEntry entry,Chapter1EncounterLifetimeOwner owner)
        {
            if(_chapterEntryInstalled || _initialized || _awakeEntered || gameObject.activeInHierarchy ||
                _localRequireNeutralStart || owner==null || !ReferenceEquals(entry.Issuer,owner) || !owner.IsIssuedEntry(this,entry) ||
                _player.Chapter1ConfiguredFirstTick!=entry.FirstTick || _transfer.Chapter1ConfiguredFirstTick!=entry.FirstTick || (_chapterBarrier!=null?_barrierFirstTick:(_chapterRegular?_chapterCombat.Chapter1ConfiguredCarryTick:_combat.Chapter1ConfiguredCarryTick))!=entry.FirstTick)
                throw new InvalidOperationException("Chapter regular entry is foreign, late, or mismatched.");
            _chapterEntry=entry;_chapterEntryInstalled=true;if(_chapterRegular)_chapterSink=new Chapter1GameplayInputSink(this,_chapterCombat,_chapterToken,entry);
            FreezeChapter1Body();
            if(_chapterRegular || _chapterBarrier!=null)RegisterModeRequester(owner);
        }
        private Chapter1FailurePublication _chapterHealthPublication;
        private long _chapterHealthRevision;
        private CapsuleCollider2D _chapterBody;
        private Vector2 _chapterBodySize,_chapterBodyOffset;
        private Vector3 _chapterBodyScale;
        private bool _chapterHubNeutralReady;
        private Chapter1HubLifetimeOwner _chapterHubOwner;
        private uint _chapterHubArmUpdate,_chapterHubNeutralUpdate,_chapterHubFrameInputUpdate,_chapterHubSubmitUpdate,_chapterHubClickUpdate;
        private InputFrameCommitReceipt? _chapterHubArmReceipt,_chapterHubNeutralReceipt;
        internal bool Chapter1HubNeutralReady => _chapterHubNeutralReady && IsHubUiOnlyVariant && !_faulted && !_disposed;
        internal InputFrameCommitReceipt? Chapter1HubNeutralReceipt=>_chapterHubNeutralReceipt;
        internal uint Chapter1HubNeutralInputUpdate=>_chapterHubNeutralUpdate;
        internal uint Chapter1HubFrameInputUpdate=>_chapterHubFrameInputUpdate;
        internal bool Chapter1HubPreparationBlocked=>_faulted || _disposed || _actionsClosed || _recoveryReservation!=RecoveryReservationStateV1.None ||
            _resetCutoverTerminal || (_startEntered && _preparedLaunchState!=PreparedLaunchState.Initialized);
        internal bool Chapter1PreparedForUi(object owner)=>owner!=null && ReferenceEquals(owner,_preparedLaunchOwner) &&
            ReferenceEquals(owner,_preparedLaunchOwnerProof) && _preparedLaunchState==PreparedLaunchState.Initialized &&
            _hubUiOnlyInitialized && _initialized && !_faulted && !_disposed && !_actionsClosed && !_resetCutoverTerminal;
        internal Func<bool> GetChapter1HubClosureReader()=>GetChapter1ClosureReader();
        // REQ-C1B-SRETRY-003/004: Hub의 실제 동기 재무장 동안만 현재 영수증의 중립을 철회한다.
        internal void RearmChapter1HubNeutralAfterStorageFailure(Chapter1HubLifetimeOwner owner,Chapter1HubRequestToken token)
        {
            ValidatePreparedInvariantOrClose();ValidateHubUiOnlyRuntime();
            if(owner==null || token==null || !ReferenceEquals(owner,_chapterHubOwner) || !ReferenceEquals(GetComponent<Chapter1HubLifetimeOwner>(),owner) ||
                !ReferenceEquals(owner.Router,this) || !owner.IsArmingStorageRetry(this,token) || !isActiveAndEnabled || _disposed || _faulted ||
                !_chapterHubNeutralReady || !_currentUiFrame.HasValue || !_currentUiFrame.Value.Equals(token.Frame) ||
                !_currentReceipt.HasValue || !_currentReceipt.Value.Equals(token.Frame.Receipt) ||
                token.InputUpdate!=_chapterHubFrameInputUpdate || !token.NeutralReceipt.HasValue || !_chapterHubNeutralReceipt.HasValue ||
                !token.NeutralReceipt.Value.Equals(_chapterHubNeutralReceipt.Value) ||
                !MatchesPreparedLaunchCohortActions(owner.Adapter,_actions))
                throw new InvalidOperationException("실제 거점의 현재 소비 원문 재무장만 중립 관문을 철회합니다.");
            _chapterHubArmUpdate=UnityEngine.InputSystem.LowLevel.InputState.updateCount;_chapterHubArmReceipt=token.Frame.Receipt;
            _chapterHubNeutralReady=false;_chapterHubNeutralReceipt=null;ClearCommittedUiBatch(false);
        }
        internal bool IsChapter1HubIntentFrameAfterNeutral(Chapter1HubLifetimeOwner owner,UiSemanticFrameV1 frame)
        {
            return owner!=null && ReferenceEquals(owner,_chapterHubOwner) && ReferenceEquals(owner.Router,this) && _chapterHubNeutralReady &&
                _chapterHubNeutralReceipt.HasValue && _currentUiFrame.HasValue && _currentUiFrame.Value.Equals(frame) &&
                frame.Receipt.Tick.Value>_chapterHubNeutralReceipt.Value.Tick.Value && frame.Receipt.FrameOrdinal>_chapterHubNeutralReceipt.Value.FrameOrdinal &&
                _chapterHubFrameInputUpdate>_chapterHubNeutralUpdate &&
                ((frame.SubmitPressed && _chapterHubSubmitUpdate>_chapterHubNeutralUpdate) || (frame.ClickPressed && _chapterHubClickUpdate>_chapterHubNeutralUpdate));
        }
        private bool ObserveChapter1HubNeutralFrame(InputFrameCommitReceipt receipt,uint update)
        {
            if(_chapterHubNeutralReady)return update<=_chapterHubNeutralUpdate;
            var later=update>_chapterHubArmUpdate && (!_chapterHubArmReceipt.HasValue ||
                (receipt.Tick.Value>_chapterHubArmReceipt.Value.Tick.Value && receipt.FrameOrdinal>_chapterHubArmReceipt.Value.FrameOrdinal));
            var neutral=later;
            // 실제 결속 control을 모두 읽는다. edge 부재와 action latch는 물리 release를 대신하지 않는다.
            foreach(var control in _actions.UI.Submit.controls)if(control.EvaluateMagnitude()>0f)neutral=false;
            foreach(var control in _actions.UI.Click.controls)if(control.EvaluateMagnitude()>0f)neutral=false;
            foreach(var control in _actions.UI.Navigate.controls)if(control.EvaluateMagnitude()>0f)neutral=false;
            if(neutral){_chapterHubNeutralReady=true;_chapterHubNeutralUpdate=update;_chapterHubNeutralReceipt=receipt;}
            return true; // 중립을 처음 얻은 frame의 모든 edge도 폐기한다.
        }
        private void FreezeChapter1Body()
        {
            _chapterBody=_player.GetComponent<CapsuleCollider2D>();
            if(_chapterBody==null || _chapterBody.direction!=CapsuleDirection2D.Vertical || _player.transform.rotation!=Quaternion.identity)
                throw new InvalidOperationException("플레이어 완료 body 형상이 지정 capsule과 다릅니다.");
            _chapterBodySize=_chapterBody.size;_chapterBodyOffset=_chapterBody.offset;_chapterBodyScale=_player.transform.lossyScale;
            if(_chapterBodyScale.x<=0 || _chapterBodyScale.y<=0)throw new InvalidOperationException("플레이어 body scale이 유효하지 않습니다.");
        }
        internal bool OwnsChapter1HealthPublication(Chapter1FailurePublication value)=>value!=null && ReferenceEquals(value,_chapterHealthPublication) &&
            ReferenceEquals(value.Token,_chapterToken) && ReferenceEquals(value.Issuer,_chapterBarrier!=null?(object)_chapterBarrier:(_chapterRegular?(object)_chapterCombat:_combat)) && ReferenceEquals(value.Router,this);
        // REQ-C1B-G3OBS-002: 저장된 발행 원문만 읽으며 거절도 게임 상태를 바꾸지 않는다.
        internal bool TryReadChapter1PublishedObservationForAuthoring(out InputFrameCommitReceipt receipt, out Chapter1FailurePublication health)
        {
            receipt=default(InputFrameCommitReceipt);health=null;
            if(this==null || !isActiveAndEnabled || !_chapterRegular || !_chapterEntryInstalled || !_initialized ||
                IsHubUiOnlyVariant || _combat!=null || _chapterCombat==null || !_chapterCombat.isActiveAndEnabled ||
                _player==null || !_player.isActiveAndEnabled || _transfer==null || !_transfer.isActiveAndEnabled || _actions==null ||
                _faulted || _disposed || _actionsClosed || _executionGuardBlocked || _executionGuardClosed || _resetCutoverTerminal ||
                _chapterClosure.Destroyed || _chapterClosure.Closed || _preparedLaunchState!=PreparedLaunchState.Unreserved ||
                _recoveryReservation!=RecoveryReservationStateV1.None || _faultStage!=InputRouterFaultStage.None ||
                _preparedLaunchFaultStage!=PreparedLaunchFaultStage.None)return false;
            var issuer=_chapterEntry.Issuer;
            if(issuer==null || !issuer.isActiveAndEnabled || !issuer.IsIssuedEntry(this,_chapterEntry) ||
                string.IsNullOrEmpty(_chapterEntry.RunKey) || _chapterEntry.Generation<=0 || !_currentReceipt.HasValue ||
                !_uiPublicationProof.HasValue || !_currentReceipt.Value.Equals(_uiPublicationProof.Value) ||
                _currentUiFrame.HasValue || _uiFramePublicationProof.HasValue)return false;
            var stored=_currentReceipt.Value;
            if(stored.Mode!=InputMode.GameplayEnabled || _mode!=InputMode.GameplayEnabled || stored.Tick.Value<0 ||
                stored.FrameOrdinal<0 || stored.ModeEpoch<0 || stored.ModeEpoch!=_modeEpoch || _frameOrdinal<=0 ||
                stored.FrameOrdinal!=_frameOrdinal-1)return false;
            var published=_chapterHealthPublication;
            if(published==null || !OwnsChapter1HealthPublication(published) || published.Kind!=Chapter1FailureSourceKind.Health ||
                published.Revision<=0 || published.Revision!=_chapterHealthRevision || published.RunKey!=_chapterEntry.RunKey ||
                published.Generation!=_chapterEntry.Generation || !published.Receipt.Equals(stored) || _chapterBody==null ||
                _chapterBody.direction!=CapsuleDirection2D.Vertical || _chapterBody.size!=_chapterBodySize ||
                _chapterBody.offset!=_chapterBodyOffset || _player.transform.lossyScale!=_chapterBodyScale ||
                _player.transform.rotation!=Quaternion.identity || _chapterBodyScale.x<=0 || _chapterBodyScale.y<=0)return false;
            receipt=stored;health=published;return true;
        }
        internal bool TryGetChapter1CompletedEvidence(out Chapter1EncounterEvidenceView view)
        {
            view=default;if(!_chapterEntryInstalled || !_initialized || IsFaulted || !CurrentReceipt.HasValue)return false;
            Chapter1EncounterEvidenceView completed;int alive=0;object producer;
            if(_chapterBarrier!=null)
            {
                var publication=_chapterBarrier.LatestPublication;
                var receipt=CurrentReceipt.Value;var movement=_player.Snapshot;var transfer=_transfer.LatestPhasePublication;
                if(publication==null || !_chapterBarrier.ValidatePublished(publication) || publication.Tick!=receipt.Tick || movement.Tick!=receipt.Tick || !transfer.HasValue || transfer.Value.Tick!=receipt.Tick)return false;
                AcadeGameMaker.Combat.CombatantSnapshot? health=null;
                foreach(var item in publication.Combatants){if(item.TargetId=="player")health=item;else if(!item.IsDead)alive++;}
                if(!health.HasValue)throw new InvalidOperationException("봉쇄선 플레이어 완료 값이 없습니다.");
                completed=new Chapter1EncounterEvidenceView(_chapterEntry,receipt,new Vector2(movement.PositionXQ4096/4096f,movement.PositionYQ4096/4096f),
                    health.Value.CurrentHealth,health.Value.InvulnerabilityEndTick,transfer.Value.Snapshot.ActiveTargetId,transfer.Value.Snapshot.PlayerState==AcadeGameMaker.Transfer.TransferPlayerState.Lightweight);
                producer=_chapterBarrier;
            }
            else if(_chapterRegular)
            {
                if(!_chapterSink.TryReadCompleted(out completed))return false;producer=_chapterCombat;
                foreach(var item in _chapterCombat.LatestOutcomeForEncounterHandoff.Combatants)if(item.TargetId!="player" && !item.IsDead)alive++;
            }
            else
            {
                if(_combat==null || !_combat.HasLatestOutcome || _combatRetired)return false;
                var receipt=CurrentReceipt.Value;var motion=_player.Snapshot;var combat=_combat.LatestOutcome;var phase=_transfer.LatestPhasePublication;
                if(!phase.HasValue || phase.Value.Tick!=receipt.Tick || combat.Tick!=receipt.Tick || motion.Tick!=receipt.Tick ||
                    _player.NextExpectedTick.Value!=checked(receipt.Tick.Value+1))return false;
                AcadeGameMaker.Combat.CombatantSnapshot? health=null;
                foreach(var item in combat.Combatants){if(item.TargetId=="player"){if(health.HasValue)throw new InvalidOperationException("플레이어가 중복되었습니다.");health=item;}else if(!item.IsDead)alive++;}
                if(!health.HasValue)throw new InvalidOperationException("플레이어 완료 health가 없습니다.");
                var state=phase.Value.Snapshot;
                completed=new Chapter1EncounterEvidenceView(_chapterEntry,receipt,new Vector2(motion.PositionXQ4096/4096f,motion.PositionYQ4096/4096f),
                    health.Value.CurrentHealth,health.Value.InvulnerabilityEndTick,state.ActiveTargetId,state.PlayerState==AcadeGameMaker.Transfer.TransferPlayerState.Lightweight);
                producer=_combat;
            }
            if(_chapterBody==null || _chapterBody.size!=_chapterBodySize || _chapterBody.offset!=_chapterBodyOffset ||
                _player.transform.lossyScale!=_chapterBodyScale || _player.transform.rotation!=Quaternion.identity)throw new InvalidOperationException("완료 body 저작 형상이 변경되었습니다.");
            var size=Vector2.Scale(_chapterBodySize,new Vector2(_chapterBodyScale.x,_chapterBodyScale.y));
            var center=completed.Position+Vector2.Scale(_chapterBodyOffset,new Vector2(_chapterBodyScale.x,_chapterBodyScale.y));
            var body=new Rect(center-size/2,size);
            if(_chapterHealthPublication==null || !_chapterHealthPublication.Receipt.Equals(completed.Receipt))
            {
                if(_chapterHealthPublication!=null && completed.Tick.Value!=checked(_chapterHealthPublication.Receipt.Tick.Value+1))throw new InvalidOperationException("health 관측 tick이 연속하지 않습니다.");
                var revision=checked(_chapterHealthRevision+1);
                _chapterHealthPublication=new Chapter1FailurePublication(producer,_chapterToken,this,completed.RunKey,completed.Generation,
                    completed.Receipt,revision,Chapter1FailureSourceKind.Health,completed.Health==0);_chapterHealthRevision=revision;
            }
            view=new Chapter1EncounterEvidenceView(completed,body,alive,_chapterHealthPublication,producer,_chapterToken);return true;
        }
        // CH1 visual-only sampled facts. No second movement/combat owner and
        // no input reads: the output is derived from completed published state.
        // Exact collider identity prevents a room's extra actor from inheriting
        // the player animation. Does not alter combat/physics or acceptance.
        internal bool TryGetDoeonAnimationEvidence(
            Collider2D exactPlayer, out int tick, out int velocityXQ4096,
            out int velocityYQ4096, out bool grounded, out bool dashing,
            out int health, out long committedAttackOrdinal)
        {
            tick=velocityXQ4096=velocityYQ4096=health=0;
            grounded=dashing=false;committedAttackOrdinal=-1;
            if(exactPlayer==null || _player==null || IsFaulted ||
               !ReferenceEquals(_player.GetComponent<Collider2D>(),exactPlayer) ||
               !TryGetChapter1CompletedEvidence(out var completed))
                return false;
            var motion=_player.Snapshot;
            if(motion.Tick!=completed.Tick)return false;
            tick=motion.Tick.Value;
            velocityXQ4096=motion.VelocityXQ4096;
            velocityYQ4096=motion.VelocityYQ4096;
            grounded=motion.IsGrounded;
            dashing=motion.ActionState==MovementActionState.Dashing;
            health=completed.Health;
            committedAttackOrdinal=_nextAttackSequenceId;
            return true;
        }
        private void InitializeChapter1Regular()
        {
            if(!_chapterEntryInstalled || _chapterSink==null || _combat!=null || _localRequireNeutralStart || _authoredHubUiOnly ||
                _player==null || _transfer==null || _chapterCombat==null || _cameraProviderBehaviour==null)
                throw new InvalidOperationException("Chapter regular graph has no issued entry.");
            _cameraProvider=_cameraProviderBehaviour as IInputCameraSnapshotProvider;
            if(_cameraProvider==null || !ReferenceEquals(_cameraProvider.BoundPlayer,_player) || _chapterCombat.BoundPlayer!=_player || _chapterCombat.BoundTransferDriver!=_transfer || _transfer.BoundPlayer!=_player)
                throw new InvalidOperationException("Chapter regular graph is cross-wired.");
            var firstTick=_player.NextExpectedTick;
            if(firstTick!=_chapterEntry.FirstTick)throw new InvalidOperationException("Chapter motor did not consume its seed.");
            _transfer.InitializeForInputRouter();
            _player.ValidateCoordinatedInputRegistration(this,this,firstTick);_transfer.ValidateCoordinatedInputRegistration(this,this,firstTick);_chapterSink.ValidateRegistration(firstTick);
            if(_transfer.IsInputGateLocked!=_player.IsInputGateLocked || _chapterSink.IsInputGateLocked!=_player.IsInputGateLocked)throw new InvalidOperationException("Chapter initial gates differ.");
            _player.RegisterCoordinatedInputOwner(this,this,firstTick);_transfer.RegisterCoordinatedInputOwner(this,this,firstTick);_chapterSink.Register(firstTick);
            _mode=_initialMode;_frameOrdinal=0;_modeEpoch=0;
#if UNITY_EDITOR
            if(_reviewActionsPrepared)VerifyReviewActionsForAuthoring(_reviewInputOwner);else
#endif
            SwitchMaps(_mode,true);_initialized=true;
        }
        private void InitializeChapter1Barrier()
        {
            if(!_chapterEntryInstalled || _chapterBarrier==null || _player==null || _transfer==null || _combat!=null || _chapterCombat!=null)throw new InvalidOperationException("봉쇄선 graph가 혼합됐습니다.");
            _cameraProvider=_cameraProviderBehaviour as IInputCameraSnapshotProvider;
            if(_cameraProvider==null || !ReferenceEquals(_cameraProvider.BoundPlayer,_player))throw new InvalidOperationException("봉쇄선 카메라 결속이 다릅니다.");
            var tick=_player.NextExpectedTick;if(tick!=_chapterEntry.FirstTick)throw new InvalidOperationException("봉쇄선 첫 tick이 다릅니다.");
            _transfer.InitializeForInputRouter();_player.ValidateCoordinatedInputRegistration(this,this,tick);_transfer.ValidateCoordinatedInputRegistration(this,this,tick);
            _player.RegisterCoordinatedInputOwner(this,this,tick);_transfer.RegisterCoordinatedInputOwner(this,this,tick);
            _mode=_initialMode;_frameOrdinal=0;_modeEpoch=0;SwitchMaps(_mode,true);_initialized=true;
        }
        private bool _disposed;
        private bool _captureSuppressed;
        // REQ-M5D7PA-004. A UI Value action performs its initial-state check
        // on the next InputSystem update.  Keep the router's single callback
        // owner quarantined for that whole update so a held pre-entry control
        // (and any input sharing that update) cannot cross a mode epoch.
        private bool _uiEnableQuarantinePending;
        private bool _uiEnableQuarantineSubscribed;
        private bool _faulted;
        private long _frameOrdinal;
        private long _modeEpoch;
        // REQ-M5D7Q0-003. Hub ticks are local input-frame identities and are
        // never compared to, or sourced from, a gameplay simulation.
        private SimulationTick _hubNextTick;
        private Chapter1ClosingHostEntry _closingHost;
        private bool _closingPhysicalNeutral;
        private long _closingPhysicalOrdinal=-1;
        internal void ConfigureChapter1ClosingHostForAuthoring(Chapter1ClosingHostEntry entry)
        {
            if(entry==null || _closingHost!=null || gameObject.activeInHierarchy || _awakeEntered ||
                !entry.Owner.ValidatePendingChapter1ClosingHostEntry(entry))throw new InvalidOperationException("종결 호스트 입력 입구가 유효하지 않습니다.");
            ConfigureHubUiOnlyForAuthoring();_closingHost=entry;_hubNextTick=_hubNextTickProof=entry.FirstUiTick;
        }
        internal bool TryReadChapter1ClosingControlsNeutral(Chapter1ChoicePhaseAdapter source,Chapter1ClosingHostEntry entry,UiSemanticFrameV1 frame,out bool neutral)
        {
            neutral=false;
            if(source==null || entry==null || !ReferenceEquals(entry,_closingHost) || !ReferenceEquals(source.ClosingHostEntry,entry) ||
                !entry.Owner.OwnsChapter1ClosingHost(entry,this) || IsFaulted || !_currentUiFrame.HasValue || !_currentUiFrame.Value.Equals(frame) ||
                !_currentReceipt.HasValue || !_currentReceipt.Value.Equals(frame.Receipt) || _closingPhysicalOrdinal!=frame.Receipt.FrameOrdinal)return false;
            neutral=_closingPhysicalNeutral;return true;
        }
        // REQ-M5D7Q0-003/008. These independent exact-value proofs make a
        // reflected successor mutation observable before it can become a later
        // receipt identity.  They intentionally remain private hub evidence.
        private SimulationTick _hubNextTickProof;
        private long _hubFrameOrdinalProof;
        // Once the prepared hub branch has initialized, this non-serialized
        // runtime witness closes reflected mutation of the authored flag.
        private bool _hubUiOnlyInitialized;
        private InputMode _mode = InputMode.Transition;
        private InputFrameCommitReceipt? _currentReceipt;
        // REQ-M5D7PA-001/003/006. The UI publication is receipt-bound and has
        // an independent proof so a reflected receipt cannot silently replace
        // a valid non-UI row.
        private UiSemanticFrameV1? _currentUiFrame;
        // REQ-M5D7PA-003/006. This independent value proof prevents a whole,
        // otherwise-valid frame with the same receipt from replacing the
        // publication between commits.
        private UiSemanticFrameV1? _uiFramePublicationProof;
        private InputFrameCommitReceipt? _uiPublicationProof;
        private InputRouterFaultStage _faultStage;
        private InputRouterFaultStage? _testFaultStage;
        private InputRouterDiagnostic _lastDiagnostic;
        private InputCameraDisposition _cameraDisposition = InputCameraDisposition.NoAim;
        private object _modeRequester;
        private PendingModeRequest? _pendingModeRequest;
        // REQ-M5D7M-002/003. This is deliberately separate from the M5B5
        // runtime state: an unreserved router follows the historical path.
        private PreparedLaunchState _preparedLaunchState;
        private object _preparedLaunchOwner;
        // REQ-M5D7Q0-008. Independent prepared-lifecycle identity proofs keep
        // reflected replacement of the owner/action from inheriting a valid
        // reservation or adopted callback graph.
        private object _preparedLaunchOwnerProof;
        private GameInputActions _preparedActionsProof;
        // A second, independent non-serialized adopted-wrapper witness.  Hub
        // containment can still identify the original owner after one proof or
        // the mutable live reference has been reflected to a foreign wrapper.
        private GameInputActions _preparedActionsContainmentProof;
        private bool _awakeEntered;
        private bool _startEntered;
        private bool _gameplayCallbacksRegistered;
        private bool _uiCallbacksRegistered;
        // REQ-M5D7Q0-002/008/AC009. Registration bits are deliberately not
        // ownership evidence: reflection can clear either bit after the
        // generated map accepted this router.  These exact, non-serialized
        // wrapper witnesses are set only for the map whose AddCallbacks call
        // completed, and are cleared before any best-effort teardown call.
        private GameInputActions _gameplayCallbacksProof;
        private GameInputActions _uiCallbacksProof;
        private bool _actionsClosed;
        private PreparedLaunchFaultStage _preparedLaunchFaultStage;
        private PreparedLaunchFaultInjectionStage? _testPreparedLaunchFaultStage;
        // M5D7M reflection-only ownership evidence; never exposed to gameplay.
        private int _closeAttempts, _disposeAttempts, _gameplayUnsubscribeAttempts, _uiUnsubscribeAttempts;
        private GameInputActions _lastClosedActions;
        // Closure identity is written before external map/callback/dispose
        // calls.  It keeps a corrupt _actionsClosed flag or registration bits
        // from replaying teardown against the adopted Hub wrapper.
        private GameInputActions _closedActionsProof;
        // REQ-M5D7QC2-003/004. This is deliberately a terminal lane rather
        // than a general action hot-swap state.  It is reachable only from the
        // published Hub cohort through the adapter-owned capability.
        private bool _resetCutoverTerminal;
        private long _resetCutoverGeneration;
        private GameInputActions _resetOldActions;
        private GameInputActions _resetNewActions;
        private bool _resetOldDisposeAttempted;
        private bool _resetNewDisposeAttempted;
        private bool _resetCandidateClaimed;
        // Independent non-serialized terminal witnesses. A reflected lifecycle
        // bit cannot resurrect an input owner because every C2 getter and
        // transfer checks this witness as well as the public-compatible flag.
        private readonly object _resetCutoverMintWitness = new object();
        private ResetCutoverLifecycleV1 _resetCutoverLifecycle;
        // REQ-M5D7QC2R-001..003.  Restart recovery is a separate, actionless
        // reservation. It is never an ordinary prepared launch.
        private RecoveryReservationStateV1 _recoveryReservation;
        private object _recoveryOwner;
        private string _recoveryRoot;

        private Vector2 _move;
        private int _pointX, _pointY;
        private Vector2 _stick;
        private AimSource? _activeAimSource;
        private bool _jumpPressed, _jumpReleased, _dashPressed, _attackPressed, _transferPressed;
        private bool _attackFromMouse, _transferFromMouse;
        private bool _unboundGameplay, _unboundUi;
        private bool _chapterChoiceBound,_choicePressed;
        private long _choiceEdgeOrdinal;
        internal void BindChapter1ChoiceForAuthoring()
        {if(gameObject.activeInHierarchy||_awakeEntered||_chapterChoiceBound)throw new InvalidOperationException("선택 입력 결속이 늦거나 중복입니다.");_chapterChoiceBound=true;}
        // REQ-M5D7PA-002. Latest-value UI facts plus one frozen-frame batch.
        private int _uiNavigateXQ4096, _uiNavigateYQ4096;
        private bool _uiNavigateChanged, _uiHasPoint, _uiPointChanged;
        private int _uiPointXActualPx, _uiPointYActualPx;
        private bool _uiClickPressed, _uiHasClickPoint, _uiSubmitPressed, _uiCancelPressed;
        private int _uiClickPointXActualPx, _uiClickPointYActualPx;
        private long _uiScrollXQ4096, _uiScrollYQ4096;
        private long _callbackOrdinal, _pointOrdinal, _stickOrdinal;
        private long _nextAimSampleId, _nextAttackSequenceId;
        private bool _committedAim, _committedAttack;

        public InputFrameCommitReceipt? CurrentReceipt { get { if (TryReadExecutionHistory()) return _currentReceipt; RejectResetTerminal(); ValidateHubUiOnlyRuntimeOrFault(); ValidateUiPublicationOrFault(); return _currentReceipt; } }
        public UiSemanticFrameV1? CurrentUiFrame { get { if (TryReadExecutionHistory()) return _currentUiFrame; RejectResetTerminal(); ValidateHubUiOnlyRuntimeOrFault(); ValidateUiPublicationOrFault(); return _currentUiFrame; } }
        public bool IsFaulted => _faulted || (_transfer!=null && _transfer.Chapter1TechnicalBlocked);
        internal InputRouterDiagnostic LastDiagnostic => _lastDiagnostic;
        internal InputMode EffectiveMode => _mode;
        // REQ-M5D1-001. These are authoring/validation reads only. They do
        // not expose the router's pending batch, action maps, or consumers.
        internal PlayerMovementController BoundPlayerForAuthoring => _player;
        internal TransferSimulationDriver BoundTransferForAuthoring => _transfer;
        internal OrdanBossCombatSimulationDriver BoundCombatForAuthoring => _combat;
        internal OrdanBossTerminalTeardown BoundTerminalTeardownForAuthoring => _terminalTeardown;
        internal MonoBehaviour BoundCameraProviderForAuthoring => _cameraProviderBehaviour;
        // REQ-M5D7Q0-004/009. Keep editor-only validation within the Input
        // assembly boundary: consumers that do not reference the gameplay
        // adapter assemblies receive only the aggregate null-matrix result.
        internal bool HasAnyForbiddenHubReferenceForAuthoring => _player != null || _transfer != null || _combat != null ||
            _terminalTeardown != null || _cameraProviderBehaviour != null;
        internal InputMode AuthoredInitialMode => _initialMode;
        internal bool AuthoredHubUiOnly => _authoredHubUiOnly;
        internal bool HubUiOnlyIntentProofForAuthoring => _hubUiOnlyIntentProof;
        // C2R is intentionally narrower than an ordinary UIOnly initial mode:
        // only the authored Hub graph may select a recovery reservation.
        internal bool IsAuthoredHubUiOnlyForRecovery => _authoredHubUiOnly && _hubUiOnlyIntentProof;
        // A prepared hub reservation retains its identity proof until the
        // existing owner-bound failure path closes it.  Include that proof in
        // the discriminator so a reflected clear of both serialized markers
        // cannot make Reserved/Adopted look like a legacy full graph before
        // the prepared boundary converts it to the legal Failed row.
        private bool IsHubUiOnlyVariant => _authoredHubUiOnly || _hubUiOnlyIntentProof || _hubUiOnlyInitialized ||
            (_preparedLaunchState != PreparedLaunchState.Unreserved && _preparedLaunchOwnerProof != null);
        internal bool PreparedLaunchAwakeEntered => _awakeEntered;
        internal PreparedLaunchFaultStage PreparedLaunchFaultStageForTests => _preparedLaunchFaultStage;
        internal void SetPreparedLaunchFaultStageForTests(PreparedLaunchFaultInjectionStage? stage) { _testPreparedLaunchFaultStage = stage; }

        // REQ-UX-004/008. One identity-bound requester may stage exactly one
        // next-tick mode decision; this router deliberately owns no menu/Run effect.
        internal void RegisterModeRequester(object requester)
        {
            if (_resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this)) throw new InvalidOperationException("Reset terminal router accepts no mode requester.");
            // REQ-M5D7Q0-006. Reject before any player/reference dereference.
            if (IsHubUiOnlyVariant) { ValidateHubUiOnlyRuntimeOrFault(); throw new InvalidOperationException("HubUIOnly does not accept mode requesters."); }
            if (requester == null) throw new ArgumentNullException(nameof(requester));
            if (_modeRequester != null && !ReferenceEquals(_modeRequester, requester)) throw new InvalidOperationException("A different mode requester is registered.");
            _modeRequester = requester;
        }

        internal void RequestMode(object requester, InputMode mode, string reason, SimulationTick tick)
        {
            if (ExecutionSessionBlocked()) throw new InvalidOperationException("C4 guarded router accepts no mode request.");
            if (_resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this)) throw new InvalidOperationException("Reset terminal router accepts no mode request.");
            // REQ-M5D7Q0-006. A hub never changes epoch or mode.
            if (IsHubUiOnlyVariant) { ValidateHubUiOnlyRuntimeOrFault(); throw new InvalidOperationException("HubUIOnly does not accept mode requests."); }
            if (!_initialized || _faulted || requester == null || !ReferenceEquals(requester, _modeRequester) ||
                string.IsNullOrWhiteSpace(reason) || !Enum.IsDefined(typeof(InputMode), mode) || tick != _player.NextExpectedTick ||
                (_mode == InputMode.Ended && mode != InputMode.Ended)) throw new InvalidOperationException("Input mode request is not authorized or exact.");
            var proposed = new PendingModeRequest(mode, reason, tick);
            // AC-M5B5-002/008. The first globally committed frame establishes
            // the authored mode at epoch zero before any real mode transition.
            if (!_currentReceipt.HasValue && mode != _mode)
                throw new InvalidOperationException("BootstrapRequired: initial input mode must commit first.");
            if (_pendingModeRequest.HasValue)
            {
                if (_pendingModeRequest.Value.Equals(proposed)) return;
                throw new InvalidOperationException("A conflicting input mode request is already pending.");
            }
            _pendingModeRequest = proposed;
        }

        internal void ConfigureForAuthoring(PlayerMovementController player, TransferSimulationDriver transfer,
            OrdanBossCombatSimulationDriver combat, MonoBehaviour cameraProvider, InputMode initialMode)
        {
            ValidatePreparedInvariantOrClose();
            if (IsHubUiOnlyVariant || _initialized || _disposed || _faulted || _actionsClosed || _startEntered || _preparedLaunchState != PreparedLaunchState.Unreserved || _actions != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered) throw new InvalidOperationException("InputRouter cannot be reconfigured after initialization.");
            _player = player; _transfer = transfer; _combat = combat;
            _cameraProviderBehaviour = cameraProvider; _initialMode = initialMode;
        }

        internal void ConfigureTerminalTeardownForAuthoring(OrdanBossTerminalTeardown teardown)
        {
            ValidatePreparedInvariantOrClose();
            if (IsHubUiOnlyVariant || _initialized || _disposed || _faulted || _actionsClosed || _startEntered || _preparedLaunchState != PreparedLaunchState.Unreserved || _actions != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered) throw new InvalidOperationException("InputRouter cannot change terminal binding after initialization.");
            _terminalTeardown = teardown;
        }

        // REQ-M5D7Q0-001/004/009. Typed pre-lifecycle authoring seam: it never
        // clears a hybrid graph and is legal only on an inactive pristine root.
        internal void ConfigureHubUiOnlyForAuthoring()
        {
            ValidatePreparedInvariantOrClose();
            if (_authoredHubUiOnly || _hubUiOnlyIntentProof || gameObject.activeInHierarchy || _awakeEntered || _startEntered || _initialized || _disposed || _faulted || _actionsClosed ||
                _preparedLaunchState != PreparedLaunchState.Unreserved || _actions != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered ||
                _player != null || _transfer != null || _combat != null || _terminalTeardown != null || _cameraProviderBehaviour != null)
                throw new InvalidOperationException("HubUIOnly authoring requires an inactive pristine null-reference graph.");
            _authoredHubUiOnly = _hubUiOnlyIntentProof = true;
            _initialMode = InputMode.UIOnly;
        }

        // The following four methods are the only M5D7M ownership boundary.
        // They intentionally expose no action collection after adoption.
        internal void ReservePreparedHubLaunch(object owner)
        {
            ValidatePreparedInvariantOrClose();
            ValidateHubIntentBeforePreparedBoundary();
            if (owner == null || _startEntered || _initialized || _disposed || _faulted || _preparedLaunchState != PreparedLaunchState.Unreserved || _preparedLaunchOwner != null || _actions != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered)
                throw new InvalidOperationException("Prepared hub reservation is not available.");
            _preparedLaunchOwner = owner; _preparedLaunchOwnerProof = IsHubUiOnlyVariant ? owner : null; _preparedLaunchState = PreparedLaunchState.Reserved;
            ValidatePreparedInvariantOrClose();
        }
        internal void ReserveRecoveryNoActions(object owner, string root)
        {
            if (owner == null || string.IsNullOrEmpty(root) || _recoveryReservation != RecoveryReservationStateV1.None || _preparedLaunchState != PreparedLaunchState.Unreserved || _actions != null || _initialized || _faulted || _disposed || _awakeEntered || _startEntered || _gameplayCallbacksRegistered || _uiCallbacksRegistered || !IsHubUiOnlyVariant)
                throw new InvalidOperationException("Recovery reservation requires a pristine HubUIOnly router.");
            _recoveryOwner=owner; _recoveryRoot=root; _recoveryReservation=RecoveryReservationStateV1.Reserved;
        }
        // REQ-M5D7QC2R-001/003/005 startup-compatibility promotion.  The
        // adapter has already reserved this exact owner before its sole root
        // read; promotion irrevocably revokes every ordinary-launch boundary.
        internal void PromotePreparedReservationToRecovery(object owner,string root,bool repairBlocked)
        {
            if(owner==null || (!repairBlocked&&string.IsNullOrEmpty(root)) || _recoveryReservation!=RecoveryReservationStateV1.None || _preparedLaunchState!=PreparedLaunchState.Reserved || !ReferenceEquals(owner,_preparedLaunchOwner) || !ReferenceEquals(owner,_preparedLaunchOwnerProof) || _actions!=null || _initialized || _faulted || _disposed || _startEntered || _gameplayCallbacksRegistered || _uiCallbacksRegistered || !IsHubUiOnlyVariant)
                throw new InvalidOperationException("Prepared reservation cannot promote to recovery.");
            _recoveryOwner=owner; _recoveryRoot=repairBlocked?null:root; _captureSuppressed=true;
            _preparedLaunchOwner=null; _preparedLaunchOwnerProof=null; _preparedLaunchState=PreparedLaunchState.Failed;
            _recoveryReservation=repairBlocked?RecoveryReservationStateV1.RepairBlocked:RecoveryReservationStateV1.Reserved;
        }
        // REQ-M5D7QC2R-001/005.  An unclassifiable marker/root is still a
        // recovery selection: keep the authored router actionless and do not
        // fall through to legacy hub startup merely because no usable root
        // string exists to reserve.
        internal void ReserveRecoveryBlockedNoActions(object owner)
        {
            if(owner==null || _recoveryReservation!=RecoveryReservationStateV1.None || _preparedLaunchState!=PreparedLaunchState.Unreserved || _actions!=null || _initialized || _faulted || _disposed || _awakeEntered || _startEntered || _gameplayCallbacksRegistered || _uiCallbacksRegistered || !IsHubUiOnlyVariant)
                throw new InvalidOperationException("Recovery repair reservation requires a pristine HubUIOnly router.");
            _recoveryOwner=owner; _recoveryRoot=null; _captureSuppressed=true; _recoveryReservation=RecoveryReservationStateV1.RepairBlocked;
        }
        internal void ValidateRecoveryReservedAwake(object owner, string root)
        {
            if (owner == null || !ReferenceEquals(owner,_recoveryOwner) || !string.Equals(root,_recoveryRoot,StringComparison.OrdinalIgnoreCase) || _recoveryReservation != RecoveryReservationStateV1.Reserved || _actions != null || _initialized || _faulted || _disposed || _gameplayCallbacksRegistered || _uiCallbacksRegistered || !IsHubUiOnlyVariant)
                throw new InvalidOperationException("Recovery router reservation is malformed.");
        }
        internal void CloseRecoveryReservation(object owner)
        {
            if (owner!=null && ReferenceEquals(owner,_recoveryOwner) && (_recoveryReservation==RecoveryReservationStateV1.Reserved || _recoveryReservation==RecoveryReservationStateV1.RepairBlocked || _recoveryReservation==RecoveryReservationStateV1.Terminal)) { _recoveryReservation=RecoveryReservationStateV1.Closed; _captureSuppressed=true; }
        }
        internal ResetCutoverCapabilityV1 EnterRestartRecoveryTerminal(object owner, GameInputActions staged, ValidatedResetMemoryAuthorityV1 authority, IProfileResetMemoryCutoverControlV1 control)
        {
            ValidateRecoveryReservedAwake(owner, authority.Root);
            if (staged==null || control==null || _resetCutoverTerminal || _resetCandidateClaimed) throw new InvalidOperationException("Recovery terminal entry is malformed.");
            ValidateDisabledActions(staged); _resetCandidateClaimed=true; _resetNewActions=staged; _resetCutoverTerminal=true; _resetCutoverLifecycle=ResetCutoverLifecycleV1.Entered; _captureSuppressed=true; _recoveryReservation=RecoveryReservationStateV1.Terminal;
            control.InvokeOperation(ResetCutoverOperationV1.DisableNewGameplay, () => staged.Gameplay.Disable()); control.InvokeOperation(ResetCutoverOperationV1.DisableNewUi, () => staged.UI.Disable()); control.InvokeOperation(ResetCutoverOperationV1.RemoveNewGameplayCallbacks, () => staged.Gameplay.RemoveCallbacks(this)); control.InvokeOperation(ResetCutoverOperationV1.RemoveNewUiCallbacks, () => staged.UI.RemoveCallbacks(this));
            return ResetCutoverCapabilityV1.MintRestartRecoveryValidated((DesktopProfileLaunchAdapterV1)owner,this,staged,authority.Root,authority.TargetSha256,authority.TargetByteLength,checked(_resetCutoverGeneration+1),_resetCutoverMintWitness);
        }
        internal void TransferRestartRecoveryActions(ResetCutoverCapabilityV1 capability)
        {
            if (capability==null || !capability.IsRestartRecovery || !capability.IsFor(this,null,_resetNewActions) || !_resetCutoverTerminal || _faulted) throw new InvalidOperationException("Recovery transfer is malformed.");
            ValidateDisabledActions(_resetNewActions); capability.Consume(); _actions=_resetNewActions; _preparedActionsProof=_preparedActionsContainmentProof=_resetNewActions; _resetCutoverGeneration=capability.Generation; _resetCutoverLifecycle=ResetCutoverLifecycleV1.Completed; _recoveryReservation=RecoveryReservationStateV1.Completed;
        }
        internal bool ValidateRestartRecoveryPair(DesktopProfileLaunchAdapterV1 owner, GameInputActions actions, long generation)
        { return owner!=null && _recoveryReservation==RecoveryReservationStateV1.Completed && _resetCutoverTerminal && !_faulted && generation==_resetCutoverGeneration && ReferenceEquals(actions,_resetNewActions) && ReferenceEquals(actions,_actions); }

        internal void AdoptPreparedHubActions(object owner, GameInputActions actions)
        {
            ValidatePreparedInvariantOrClose();
            ValidateHubIntentBeforePreparedBoundary();
            if (owner == null || actions == null || !ReferenceEquals(owner, _preparedLaunchOwner) || _preparedLaunchState != PreparedLaunchState.Reserved || _actions != null || _startEntered || _gameplayCallbacksRegistered || _uiCallbacksRegistered)
                throw new InvalidOperationException("Prepared hub adoption is not authorized.");
            ValidateDisabledActions(actions);
            _actions = actions; _preparedActionsProof = IsHubUiOnlyVariant ? actions : null; _preparedActionsContainmentProof = IsHubUiOnlyVariant ? actions : null; _preparedLaunchState = PreparedLaunchState.Adopted;
            ValidatePreparedInvariantOrClose();
        }

        internal PreparedHubInitializationProofV1 InitializePreparedHub(object owner)
        {
            ValidatePreparedInvariantOrClose();
            ValidateHubIntentBeforePreparedBoundary();
            if (owner == null || !ReferenceEquals(owner, _preparedLaunchOwner) || _preparedLaunchState != PreparedLaunchState.Adopted || !_awakeEntered || !isActiveAndEnabled || _actions == null || _gameplayCallbacksRegistered || _uiCallbacksRegistered || _initialized || _actions.Gameplay.Get().enabled || _actions.UI.Get().enabled)
                throw new InvalidOperationException("Prepared hub initialization is not authorized.");
            var callbacksComplete = false;
            try
            {
                RegisterCallbacks();
                callbacksComplete = true;
                Initialize();
                if (_mode != InputMode.UIOnly || _actions.Gameplay.Get().enabled || !_actions.UI.Get().enabled) throw new InvalidOperationException("Prepared hub did not enter UI-only mode.");
                _preparedLaunchState = PreparedLaunchState.InitializedPendingPublication;
                ValidatePreparedInvariantOrClose();
                return new PreparedHubInitializationProofV1(owner, _mode, false, true, true, true);
            }
            catch
            {
                if (_preparedLaunchFaultStage == PreparedLaunchFaultStage.None) _preparedLaunchFaultStage = callbacksComplete ? PreparedLaunchFaultStage.Initialization : PreparedLaunchFaultStage.CallbackRegistration;
                // RegisterCallbacks can deliberately fault between the two
                // callback registrations. That is not a stable Adopted row,
                // so do not route its containment through Fail's stable-row
                // entry validator; close the exact owned candidate, publish a
                // valid Failed row, and preserve the initialization exception.
                _preparedLaunchState = PreparedLaunchState.Failed;
                _faulted = true;
                CloseActionsOnce();
                ValidatePreparedInvariantOrClose();
                throw;
            }
        }

        internal void ConfirmPreparedHubPublication(object owner)
        {
            ValidatePreparedInvariantOrClose();
            if (owner == null || !ReferenceEquals(owner, _preparedLaunchOwner) || _preparedLaunchState != PreparedLaunchState.InitializedPendingPublication || !_initialized || _actions == null || !_gameplayCallbacksRegistered || !_uiCallbacksRegistered)
                throw new InvalidOperationException("Prepared hub publication is not authorized.");
            _preparedLaunchState = PreparedLaunchState.Initialized;
            ValidatePreparedInvariantOrClose();
        }

        internal void FailPreparedHubLaunch(object owner)
        {
            ValidatePreparedInvariantOrClose();
            if (owner == null || !ReferenceEquals(owner, _preparedLaunchOwner) || (_preparedLaunchState != PreparedLaunchState.Reserved && _preparedLaunchState != PreparedLaunchState.Adopted && _preparedLaunchState != PreparedLaunchState.InitializedPendingPublication))
                throw new InvalidOperationException("Prepared hub failure is not authorized.");
            _preparedLaunchState = PreparedLaunchState.Failed; _faulted = true; if (_preparedLaunchFaultStage == PreparedLaunchFaultStage.None) _preparedLaunchFaultStage = PreparedLaunchFaultStage.Closed; CloseActionsOnce(); ValidatePreparedInvariantOrClose();
        }

        // REQ-M5D7QC2-003.  This is available only in the tiny interval after
        // real initialization and before publication confirmation so the
        // adapter can mint its immutable launch-cohort witness.
        internal GameInputActions GetPreparedActionsForLaunchCohort(object owner)
        {
            if (owner == null || !ReferenceEquals(owner, _preparedLaunchOwner) || _preparedLaunchState != PreparedLaunchState.InitializedPendingPublication || !_initialized || _actions == null ||
                !ReferenceEquals(_actions, _preparedActionsProof) || !ReferenceEquals(_actions, _preparedActionsContainmentProof))
                throw new InvalidOperationException("Prepared launch cohort actions are not exact.");
            return _actions;
        }
        internal bool MatchesPreparedLaunchCohortActions(object owner, GameInputActions actions)
        {
            return owner != null && actions != null && ReferenceEquals(owner, _preparedLaunchOwner) &&
                _preparedLaunchState == PreparedLaunchState.Initialized && _initialized && !_faulted && !_disposed &&
                ReferenceEquals(_actions, actions) && ReferenceEquals(_preparedActionsProof, actions) && ReferenceEquals(_preparedActionsContainmentProof, actions);
        }

        // REQ-M5D7QC2-003. The capability is unforgeable and is consumed only
        // by the paired transfer below.  Failure while disabling/removing is
        // terminal: no caller receives an interactive fallback.
        internal ResetCutoverCapabilityV1 EnterResetCutoverTerminal(DesktopProfileLaunchAdapterV1 owner, GameInputActions staged, ValidatedResetMemoryAuthorityV1 authority)
        {
            return EnterResetCutoverTerminal(owner, staged, authority, ProfileResetMemoryCutoverNoOpControlV1.Instance);
        }
        internal ResetCutoverCapabilityV1 EnterResetCutoverTerminal(DesktopProfileLaunchAdapterV1 owner, GameInputActions staged, ValidatedResetMemoryAuthorityV1 authority, IProfileResetMemoryCutoverControlV1 control)
        {
            if (owner == null || staged == null || authority == null || control == null || _resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortTerminal(this) || _faulted || _disposed ||
                _resetCutoverLifecycle != ResetCutoverLifecycleV1.None || _preparedLaunchState != PreparedLaunchState.Initialized || !ReferenceEquals(_preparedLaunchOwner, owner) ||
                !ReferenceEquals(_actions, _preparedActionsProof) || !ReferenceEquals(_actions, _preparedActionsContainmentProof) || !_resetCandidateClaimed || !ReferenceEquals(_resetNewActions, staged))
                throw new InvalidOperationException("Reset cutover router eligibility is not exact.");
            ValidateDisabledActions(staged);
            _resetCutoverTerminal = true; _resetCutoverLifecycle = ResetCutoverLifecycleV1.Entered; _resetOldActions = _actions;
            GameInputActions originalOld=null, originalNew=null; bool oldAttempted=false, newAttempted=false, newClaimed=false; IProfileResetMemoryCutoverControlV1 originalControl=null;
            if (!DesktopProfileLaunchAdapterV1.TryGetRouterTerminalOwnership(this, out originalOld, out originalNew, out oldAttempted, out newAttempted, out newClaimed, out originalControl) || !ReferenceEquals(originalOld, _resetOldActions) || !ReferenceEquals(originalNew, staged) || !ReferenceEquals(originalControl, control) || !newClaimed)
                throw new InvalidOperationException("Reset cutover original ownership is malformed.");
            DesktopProfileLaunchAdapterV1.RecordRouterTerminal(this, _resetOldActions, staged, 0, false, false, false);
            // Historical receipt/frame fields remain forensic evidence. Their
            // public getters reject terminal access; clearing them would erase
            // the immutable launch history C2 must preserve.
            _captureSuppressed = true; _pendingModeRequest = null;
            try
            {
                originalControl.InvokeOperation(ResetCutoverOperationV1.CancelUiEnableQuarantine, () => CancelUiEnableQuarantine());
                originalControl.InvokeOperation(ResetCutoverOperationV1.DisableNewGameplay, () => originalNew.Gameplay.Disable());
                originalControl.InvokeOperation(ResetCutoverOperationV1.DisableNewUi, () => originalNew.UI.Disable());
                originalControl.InvokeOperation(ResetCutoverOperationV1.RemoveNewGameplayCallbacks, () => originalNew.Gameplay.RemoveCallbacks(this));
                originalControl.InvokeOperation(ResetCutoverOperationV1.RemoveNewUiCallbacks, () => originalNew.UI.RemoveCallbacks(this));
                originalControl.InvokeOperation(ResetCutoverOperationV1.DisableOldGameplay, () => originalOld.Gameplay.Disable()); originalControl.InvokeOperation(ResetCutoverOperationV1.DisableOldUi, () => originalOld.UI.Disable());
                originalControl.InvokeOperation(ResetCutoverOperationV1.RemoveOldGameplayCallbacks, () => originalOld.Gameplay.RemoveCallbacks(this));
                originalControl.InvokeOperation(ResetCutoverOperationV1.RemoveOldUiCallbacks, () => originalOld.UI.RemoveCallbacks(this));
                _gameplayCallbacksRegistered = _uiCallbacksRegistered = false; _gameplayCallbacksProof = _uiCallbacksProof = null;
                var capability = ResetCutoverCapabilityV1.MintValidated(owner, this, originalOld, originalNew, authority.Root, authority.TargetSha256, authority.TargetByteLength, checked(_resetCutoverGeneration + 1), _resetCutoverMintWitness);
                DesktopProfileLaunchAdapterV1.SetOriginalResetGeneration(this, capability.Generation);
                DesktopProfileLaunchAdapterV1.RecordRouterTerminal(this, _resetOldActions, staged, capability.Generation, false, false, false);
                return capability;
            }
            catch { _resetCutoverLifecycle = ResetCutoverLifecycleV1.Faulted; _faulted = true; DesktopProfileLaunchAdapterV1.RecordRouterTerminal(this, originalOld, originalNew, _resetCutoverGeneration, _resetOldDisposeAttempted, _resetNewDisposeAttempted, true); DisposeClaimedResetCandidateOnce(control); throw; }
        }

        // Non-fallible ownership record preceding every terminal teardown.
        // After this succeeds Router, not Adapter/finally, owns candidate close.
        internal void ClaimResetCandidateForTerminal(DesktopProfileLaunchAdapterV1 owner, GameInputActions staged)
        {
            if (owner == null || staged == null || _resetCandidateClaimed || _resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortTerminal(this) || _faulted || _disposed || _resetCutoverLifecycle != ResetCutoverLifecycleV1.None || _preparedLaunchState != PreparedLaunchState.Initialized || !ReferenceEquals(_preparedLaunchOwner, owner) || !ReferenceEquals(_actions, _preparedActionsProof) || !ReferenceEquals(_actions, _preparedActionsContainmentProof)) throw new InvalidOperationException("Reset candidate claim is not authorized.");
            ValidateDisabledActions(staged); _resetNewActions = staged; _resetCandidateClaimed = true;
            DesktopProfileLaunchAdapterV1.MarkOriginalCandidateClaimed(owner, this, staged);
        }
        private void DisposeClaimedResetCandidateOnce(IProfileResetMemoryCutoverControlV1 control)
        {
            GameInputActions originalOld, originalNew; bool oldAttempted, newAttempted, newClaimed; IProfileResetMemoryCutoverControlV1 originalControl;
            if (!DesktopProfileLaunchAdapterV1.TryGetRouterTerminalOwnership(this, out originalOld, out originalNew, out oldAttempted, out newAttempted, out newClaimed, out originalControl) || !newClaimed || originalNew == null || originalControl == null) return;
            if (!DesktopProfileLaunchAdapterV1.TryBeginOriginalDispose(this, true)) return;
            _resetNewDisposeAttempted = true; DesktopProfileLaunchAdapterV1.RecordRouterTerminal(this, _resetOldActions, _resetNewActions, _resetCutoverGeneration, _resetOldDisposeAttempted, true, false);
            try { originalControl.InvokeOperation(ResetCutoverOperationV1.DisposeNew, () => DisposeResetOwnedActions(originalNew)); } catch { }
        }

        // REQ-M5D7QC2-004 / REQ-M5D7QC4-003. Keep the exact owner and one
        // disposal attempt while releasing generated assets synchronously in EditMode.
        private static void DisposeResetOwnedActions(GameInputActions actions)
        {
            if (actions == null) throw new ArgumentNullException(nameof(actions));
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(actions.asset);
                return;
            }
#endif
            actions.Dispose();
        }

        // REQ-M5D7QC2-004. The old wrapper remains router-owned until this
        // method records its one close attempt before the real Dispose call.
        internal void TransferResetActions(ResetCutoverCapabilityV1 capability)
        {
            TransferResetActions(capability, ProfileResetMemoryCutoverNoOpControlV1.Instance);
        }
        internal void TransferResetActions(ResetCutoverCapabilityV1 capability, IProfileResetMemoryCutoverControlV1 control)
        {
            if (capability == null || control == null || !capability.IsFor(this, _resetOldActions, _resetNewActions) || !_resetCutoverTerminal || _faulted) throw new InvalidOperationException("Reset transfer is not authorized.");
            GameInputActions originalOld, originalNew; bool oldAttempted, newAttempted, newClaimed; IProfileResetMemoryCutoverControlV1 originalControl;
            if (!DesktopProfileLaunchAdapterV1.TryGetRouterTerminalOwnership(this, out originalOld, out originalNew, out oldAttempted, out newAttempted, out newClaimed, out originalControl) || !newClaimed || !ReferenceEquals(originalControl, control) || !capability.IsFor(this, originalOld, originalNew)) throw new InvalidOperationException("Reset transfer original ownership is malformed.");
            ValidateDisabledActions(originalOld); ValidateDisabledActions(originalNew);
            capability.Consume(); _actions = originalNew; _preparedActionsProof = _preparedActionsContainmentProof = originalNew;
            _resetCutoverGeneration = capability.Generation;
            _resetCutoverLifecycle = ResetCutoverLifecycleV1.Transferred;
            DesktopProfileLaunchAdapterV1.MarkRouterTransferred(this, _resetCutoverGeneration);
            originalControl.Checkpoint(ProfileResetMemoryCutoverCheckpointV1.BeforeOldDispose);
            if (!DesktopProfileLaunchAdapterV1.TryBeginOriginalDispose(this, false)) throw new InvalidOperationException("Reset old actions were already closed.");
            _resetOldDisposeAttempted = true; DesktopProfileLaunchAdapterV1.RecordRouterTerminal(this, originalOld, originalNew, _resetCutoverGeneration, true, _resetNewDisposeAttempted, false);
            try { originalControl.InvokeOperation(ResetCutoverOperationV1.DisposeOld, () => DisposeResetOwnedActions(originalOld)); }
            catch { _resetCutoverLifecycle = ResetCutoverLifecycleV1.Faulted; _faulted = true; throw; }
            originalControl.Checkpoint(ProfileResetMemoryCutoverCheckpointV1.AfterOldDispose);
            ValidateDisabledActions(_actions);
            _resetCutoverLifecycle = ResetCutoverLifecycleV1.Completed;
            DesktopProfileLaunchAdapterV1.MarkRouterTransferCompleted(this, _resetCutoverGeneration);
        }

        internal bool ValidateResetCutoverPair(DesktopProfileLaunchAdapterV1 owner, GameInputActions actions, long generation)
        {
            GameInputActions originalOld, originalNew; bool oldAttempted, newAttempted, newClaimed; IProfileResetMemoryCutoverControlV1 originalControl;
            if (owner == null || !_resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortTerminal(this) || _resetCutoverLifecycle != ResetCutoverLifecycleV1.Completed || _faulted || generation != _resetCutoverGeneration || !DesktopProfileLaunchAdapterV1.TryGetRouterTerminalOwnership(this,out originalOld,out originalNew,out oldAttempted,out newAttempted,out newClaimed,out originalControl) || !oldAttempted || !newClaimed || !ReferenceEquals(actions, originalNew) || !ReferenceEquals(_actions, originalNew) || _gameplayCallbacksRegistered || _uiCallbacksRegistered) return false;
            try { ValidateDisabledActions(actions); return true; } catch { _faulted = true; return false; }
        }
        internal bool IsResetCutoverMintWitness(object witness) { return witness != null && ReferenceEquals(witness, _resetCutoverMintWitness) && _resetCutoverTerminal && _resetCutoverLifecycle >= ResetCutoverLifecycleV1.Entered && _resetCutoverLifecycle <= ResetCutoverLifecycleV1.Completed; }
        internal void ValidateAllMapsDisabled(GameInputActions actions) { if (actions == null) throw new ArgumentNullException(nameof(actions)); ValidateDisabledActions(actions); }
        internal void LatchResetCutoverFailure()
        {
            ProfileNewGameConfirmationV1.RecordExecutionLifecycleFault(this);
            _resetCutoverTerminal = true; if (_resetCutoverLifecycle != ResetCutoverLifecycleV1.Disposed) _resetCutoverLifecycle = ResetCutoverLifecycleV1.Faulted; _captureSuppressed = true; _pendingModeRequest = null; _faulted = true;
            if (_recoveryReservation==RecoveryReservationStateV1.Terminal || _recoveryReservation==RecoveryReservationStateV1.Completed)
            {
                _recoveryReservation=RecoveryReservationStateV1.Closed;
                GameInputActions recoveryCandidate; IProfileResetMemoryCutoverControlV1 recoveryControl;
                if (DesktopProfileLaunchAdapterV1.TryBeginRestartRecoveryCandidateDispose(this,out recoveryCandidate,out recoveryControl)) { _resetNewDisposeAttempted=true; try { recoveryControl.InvokeOperation(ResetCutoverOperationV1.DisposeNew,()=>DisposeResetOwnedActions(recoveryCandidate)); } catch { } }
                return;
            }
            DesktopProfileLaunchAdapterV1.RecordRouterTerminal(this, _resetOldActions, _resetNewActions, _resetCutoverGeneration, _resetOldDisposeAttempted, _resetNewDisposeAttempted, true);
            // Only immutable reset ownership witnesses are touched. In
            // particular a reflected _actions field never causes foreign maps
            // or callbacks to be changed during C2 containment.
            // Before terminal entry there is no reset-new witness. The Hub
            // resolver is the existing immutable adopted-owner proof and is
            // deliberately safe against a reflected foreign _actions value.
            GameInputActions originalOld, originalNew; bool oldAttempted, newAttempted, newClaimed; IProfileResetMemoryCutoverControlV1 originalControl;
            var hasCohort = DesktopProfileLaunchAdapterV1.TryGetRouterTerminalOwnership(this, out originalOld, out originalNew, out oldAttempted, out newAttempted, out newClaimed, out originalControl);
            var owned = hasCohort ? (newClaimed ? originalNew : originalOld) : ResolveHubOwnedActions();
            try { owned?.Gameplay.Disable(); } catch { }
            try { owned?.UI.Disable(); } catch { }
            try { owned?.Gameplay.RemoveCallbacks(this); } catch { }
            try { owned?.UI.RemoveCallbacks(this); } catch { }
            if (hasCohort && originalOld != null && !ReferenceEquals(originalOld, owned))
            {
                try { originalOld.Gameplay.Disable(); } catch { }
                try { originalOld.UI.Disable(); } catch { }
                try { originalOld.Gameplay.RemoveCallbacks(this); } catch { }
                try { originalOld.UI.RemoveCallbacks(this); } catch { }
            }
            _gameplayCallbacksRegistered = _uiCallbacksRegistered = false; _gameplayCallbacksProof = _uiCallbacksProof = null;
            if (owned != null && newClaimed && DesktopProfileLaunchAdapterV1.TryBeginOriginalDispose(this, true)) { _resetNewDisposeAttempted = true; DesktopProfileLaunchAdapterV1.RecordRouterTerminal(this, originalOld, owned, _resetCutoverGeneration, oldAttempted, true, true); try { (originalControl ?? ProfileResetMemoryCutoverNoOpControlV1.Instance).InvokeOperation(ResetCutoverOperationV1.DisposeNew, () => DisposeResetOwnedActions(owned)); } catch { } }
            if (hasCohort && originalOld != null && DesktopProfileLaunchAdapterV1.TryBeginOriginalDispose(this, false)) { _resetOldDisposeAttempted = true; DesktopProfileLaunchAdapterV1.RecordRouterTerminal(this, originalOld, originalNew, _resetCutoverGeneration, true, _resetNewDisposeAttempted, true); try { originalOld.Gameplay.Disable(); } catch { } try { originalOld.UI.Disable(); } catch { } try { (originalControl ?? ProfileResetMemoryCutoverNoOpControlV1.Instance).InvokeOperation(ResetCutoverOperationV1.DisposeOld, () => DisposeResetOwnedActions(originalOld)); } catch { } }
        }
        private ProfileResetExecutionGuardContextV1 _executionGuard;
        private bool _executionGuardBlocked, _executionGuardClosed;
        private sealed class ExecutionGuardHalf
        {
            internal readonly ProfileResetExecutionGuardContextV1 Context;
            internal readonly bool Blocked, Closed;
            internal readonly ExecutionGuardHalf Previous;
            internal readonly InputFrameCommitReceipt? Receipt;
            internal readonly UiSemanticFrameV1? Frame;
            internal ExecutionGuardHalf(ProfileResetExecutionGuardContextV1 context,bool blocked,bool closed,ExecutionGuardHalf previous,InputFrameCommitReceipt? receipt,UiSemanticFrameV1? frame)
            { Context=context; Blocked=blocked; Closed=closed; Previous=previous; Receipt=receipt; Frame=frame; }
        }
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<InputRouter,ExecutionGuardHalf> ExecutionGuardHalves=new System.Runtime.CompilerServices.ConditionalWeakTable<InputRouter,ExecutionGuardHalf>();
        private bool ExecutionSessionBlocked()
        {
            if (!ExecutionGuardHalves.TryGetValue(this,out var half)) return _executionGuardBlocked || _executionGuardClosed;
            if (!ReferenceEquals(half.Context,_executionGuard) || half.Blocked != _executionGuardBlocked || half.Closed != _executionGuardClosed)
                throw new InvalidOperationException("C4 router guard projection is malformed.");
            return half.Blocked || half.Closed;
        }
        // REQ-M5D7QC4-002/004/006: native 작업은 인증된 원래 스레드에서만 수행한다.
        internal void ApplyExecutionGuard(ProfileResetExecutionGuardContextV1 context,ProfileResetExecutionGuardOperationV1 operation)
        {
            if (!(_preparedLaunchOwner is DesktopProfileLaunchAdapterV1 originalAdapter))
                throw new InvalidOperationException("C4 router has no original adapter owner.");
            ProfileNewGameConfirmationV1.ValidateExecutionGuardContext(context,originalAdapter,this,operation);
            ExecutionGuardHalves.TryGetValue(this,out var previous);
            if (operation == ProfileResetExecutionGuardOperationV1.Enter)
            {
                if (_executionGuardBlocked || _executionGuardClosed) throw new InvalidOperationException("C4 router guard already entered.");
                ValidateHubUiOnlyRuntimeOrFault(); ValidateUiPublicationOrFault();
                _executionGuard=context; _executionGuardBlocked=true;
            }
            else if (operation == ProfileResetExecutionGuardOperationV1.CompleteFresh)
            {
                if (!ReferenceEquals(_executionGuard,context) || !_executionGuardBlocked || _executionGuardClosed) throw new InvalidOperationException("C4 router guard cannot complete fresh.");
                _executionGuardBlocked=false;
            }
            else
            {
                _executionGuard=context; _executionGuardBlocked=true; _executionGuardClosed=true;
            }
            var half=new ExecutionGuardHalf(context,_executionGuardBlocked,_executionGuardClosed,previous,
                operation == ProfileResetExecutionGuardOperationV1.Enter ? _currentReceipt : previous?.Receipt,
                operation == ProfileResetExecutionGuardOperationV1.Enter ? _currentUiFrame : previous?.Frame);
            ExecutionGuardHalves.Remove(this); ExecutionGuardHalves.Add(this,half);
            if (operation == ProfileResetExecutionGuardOperationV1.Close &&
                (previous != null && previous.Closed || _resetCutoverLifecycle != ResetCutoverLifecycleV1.Completed || DesktopProfileLaunchAdapterV1.IsRouterCohortTerminal(this)))
                LatchResetCutoverFailure();
        }
        private bool TryReadExecutionHistory()
        {
            if (!ExecutionGuardHalves.TryGetValue(this,out var original) || !original.Blocked || original.Closed) return false;
            // 관리 원본 스레드 인증이 value/proof 및 Unity 조회보다 먼저다.
            if (!(_preparedLaunchOwner is DesktopProfileLaunchAdapterV1 originalAdapter) ||
                !ProfileNewGameConfirmationV1.IsExecutionGuardedPair(originalAdapter,this))
                throw new InvalidOperationException("C4 history has no live original context.");
            if (!ExecutionSessionBlocked() || !_currentReceipt.HasValue || !_currentUiFrame.HasValue ||
                !original.Receipt.HasValue || !original.Frame.HasValue || !_uiPublicationProof.HasValue || !_uiFramePublicationProof.HasValue ||
                !_currentReceipt.Value.Equals(original.Receipt.Value) || !_uiPublicationProof.Value.Equals(original.Receipt.Value) ||
                !_currentUiFrame.Value.Equals(original.Frame.Value) || !_uiFramePublicationProof.Value.Equals(original.Frame.Value))
                throw new InvalidOperationException("C4 original history pair is malformed.");
            ValidateReceipt(original.Receipt.Value); original.Frame.Value.Validate();
            if (!original.Frame.Value.Receipt.Equals(original.Receipt.Value)) throw new InvalidOperationException("C4 history pair disagrees.");
            return true;
        }
        private void RejectResetTerminal() { if (ExecutionSessionBlocked() || _resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this)) throw new InvalidOperationException("Reset terminal router exposes no semantic history."); }

#if UNITY_EDITOR
        private Chapter1EncounterLifetimeOwner _reviewInputOwner;
        private GameInputActions _reviewActionsProof;
        private bool _reviewActionsPrepared;
        private InputActionMap _reviewGameplayMapProof,_reviewUiMapProof;
        [Serializable] internal struct ReviewInputRoute
        {
            public bool available,ownerSame,wrapperSame,mapSame,registrationSame,gameplayMapEnabled,uiMapEnabled,initialized,startCompleted,faulted,disposed,closed,captureSuppressed;
            public string mode,unavailableReason;
        }
        internal ReviewInputRoute ReadReviewInputRouteForAuthoring(Chapter1EncounterLifetimeOwner owner)
        {
            var row=new ReviewInputRoute{available=true,ownerSame=owner!=null&&ReferenceEquals(owner,_reviewInputOwner),initialized=_initialized,startCompleted=_startEntered,faulted=_faulted,disposed=_disposed,closed=_actionsClosed,captureSuppressed=_captureSuppressed,mode=_mode.ToString(),unavailableReason=""};
            try{row.wrapperSame=_actions!=null&&ReferenceEquals(_actions,_reviewActionsProof);row.registrationSame=_gameplayCallbacksRegistered&&_uiCallbacksRegistered&&ReferenceEquals(_gameplayCallbacksProof,_actions)&&ReferenceEquals(_uiCallbacksProof,_actions);
                if(_actions!=null){var gameplay=_actions.Gameplay.Get();var ui=_actions.UI.Get();row.mapSame=ReferenceEquals(gameplay,_reviewGameplayMapProof)&&ReferenceEquals(ui,_reviewUiMapProof);row.gameplayMapEnabled=gameplay.enabled;row.uiMapEnabled=ui.enabled;}}
            catch{row.available=false;row.unavailableReason="route-read-failed";}return row;
        }
        internal bool ReviewStartCompletedForAuthoring=>_reviewActionsPrepared&&_startEntered&&_initialized&&!_faulted&&!_disposed;
        internal void PrepareReviewActionsForAuthoring(Chapter1EncounterLifetimeOwner owner,GameObject staging)
        {
            if(owner==null||staging==null||transform.root.gameObject!=staging||staging.activeInHierarchy||_awakeEntered||_startEntered||_initialized||_faulted||_disposed||_actionsClosed||_actions!=null||_reviewActionsPrepared||!_chapterRegular||!_chapterEntryInstalled||!owner.IsIssuedEntry(this,_chapterEntry)||_preparedLaunchState!=PreparedLaunchState.Unreserved||_gameplayCallbacksRegistered||_uiCallbacksRegistered)
                throw new InvalidOperationException("사전 입력 준비는 정확 비활성 pristine 검수 graph만 허용합니다.");
            _reviewInputOwner=owner;
            try{AllocateLegacyActionsAndCallbacks();_mode=InputMode.GameplayEnabled;SwitchMaps(_mode,true);_reviewActionsProof=_actions;_reviewGameplayMapProof=_actions.Gameplay.Get();_reviewUiMapProof=_actions.UI.Get();_reviewActionsPrepared=true;VerifyReviewActionsForAuthoring(owner);}
            catch{_faulted=true;CloseActionsOnce();throw;}
        }
        internal void VerifyReviewActionsForAuthoring(Chapter1EncounterLifetimeOwner owner)
        {
            if(!_reviewActionsPrepared||owner==null||!ReferenceEquals(owner,_reviewInputOwner)||!owner.IsIssuedEntry(this,_chapterEntry)||_actions==null||!ReferenceEquals(_actions,_reviewActionsProof)||_disposed||_faulted||_actionsClosed||!_gameplayCallbacksRegistered||!_uiCallbacksRegistered||!ReferenceEquals(_gameplayCallbacksProof,_actions)||!ReferenceEquals(_uiCallbacksProof,_actions)||!_actions.Gameplay.Get().enabled||_actions.UI.Get().enabled||_mode!=InputMode.GameplayEnabled||_preparedLaunchState!=PreparedLaunchState.Unreserved||_currentReceipt.HasValue)
                throw new InvalidOperationException("사전 입력 원래 wrapper/map/등록 증거가 다릅니다.");
        }
        internal void CloseReviewActionsForAuthoring(Chapter1EncounterLifetimeOwner owner)
        {if(!ReferenceEquals(owner,_reviewInputOwner))throw new InvalidOperationException("다른 사전 입력 소유자입니다.");_faulted=true;CloseActionsOnce();}
#endif
        private void Awake()
        {
            if (_awakeEntered) throw new InvalidOperationException("InputRouter Awake cannot repeat.");
            ValidatePreparedInvariantOrClose();
            _awakeEntered = true;
            if (_recoveryReservation == RecoveryReservationStateV1.Reserved) ValidateRecoveryReservedAwake(_recoveryOwner,_recoveryRoot);
            ValidatePreparedInvariantOrClose();
        }

        private void Start()
        {
            ValidatePreparedInvariantOrClose();
            if (_startEntered) return;
            if (_recoveryReservation == RecoveryReservationStateV1.Reserved || _recoveryReservation == RecoveryReservationStateV1.RepairBlocked || _recoveryReservation == RecoveryReservationStateV1.Terminal || _recoveryReservation == RecoveryReservationStateV1.Completed || _recoveryReservation == RecoveryReservationStateV1.Closed) { _startEntered=true; return; }
            if (_preparedLaunchState == PreparedLaunchState.Initialized || _initialized) { _startEntered = true; ValidatePreparedInvariantOrClose(); return; }
            // REQ-M5D7Q0-002. A hub cannot use the legacy allocation fallback.
            if (IsHubUiOnlyVariant && _preparedLaunchState == PreparedLaunchState.Unreserved)
            {
                _startEntered = true; LatchFault(InputRouterFaultStage.HubUiOnly); CloseActionsOnce(); ValidatePreparedInvariantOrClose(); return;
            }
            if (_preparedLaunchState == PreparedLaunchState.Reserved) { _startEntered = true; _preparedLaunchState = PreparedLaunchState.Failed; _faulted = true; _preparedLaunchFaultStage = PreparedLaunchFaultStage.Closed; CloseActionsOnce(); ValidatePreparedInvariantOrClose(); return; }
            if (_preparedLaunchState != PreparedLaunchState.Unreserved) { _startEntered = true; ValidatePreparedInvariantOrClose(); return; }
            try {
#if UNITY_EDITOR
                if(_reviewActionsPrepared)VerifyReviewActionsForAuthoring(_reviewInputOwner);else
#endif
                EnsureLegacyActionsAndCallbacks(); Initialize(); _startEntered = true; ValidatePreparedInvariantOrClose(); }
            catch { _startEntered = true; LatchFault(InputRouterFaultStage.Initialization); throw; }
        }

        internal void InitializeForTests()
        {
            ValidatePreparedInvariantOrClose();
            if (IsHubUiOnlyVariant) throw new InvalidOperationException("HubUIOnly must use the prepared adapter lifecycle.");
            if (!_awakeEntered || _preparedLaunchState != PreparedLaunchState.Unreserved) throw new InvalidOperationException("Prepared launch must use its owner initialization seam.");
            if (_faulted || _disposed) throw new InvalidOperationException("InputRouter has a closed lifecycle.");
            if (_initialized) { ValidatePreparedInvariantOrClose(); return; }
            try { EnsureLegacyActionsAndCallbacks(); Initialize(); ValidatePreparedInvariantOrClose(); }
            catch { LatchFault(InputRouterFaultStage.Initialization); ValidatePreparedInvariantOrClose(); throw; }
        }

        internal void StepForTests()
        {
            RejectResetTerminal();
            ValidatePreparedInvariantOrClose();
            if (!_initialized || _faulted) throw new InvalidOperationException("InputRouter test step requires an initialized live router.");
            CommitNextFrame(); ValidatePreparedInvariantOrClose();
        }

        internal void SetFaultStageForTests(InputRouterFaultStage? stage) { ValidatePreparedInvariantOrClose(); _testFaultStage = stage; }
        private void CheckInjectedFault(InputRouterFaultStage stage)
        {
            if (_testFaultStage.HasValue && _testFaultStage.Value == stage) throw new InvalidOperationException("Injected InputRouter fault.");
        }

        private void Initialize()
        {
            if (_initialized || _faulted || _disposed) throw new InvalidOperationException("InputRouter has a closed lifecycle.");
            if (IsHubUiOnlyVariant) { InitializeHubUiOnly(); return; }
            if(_chapterBarrier!=null){InitializeChapter1Barrier();return;}
            if(_chapterRegular){InitializeChapter1Regular();return;}
            if (_player == null || _transfer == null || _combat == null || _cameraProviderBehaviour == null ||
                !Enum.IsDefined(typeof(InputMode), _initialMode)) throw new InvalidOperationException("InputRouter requires exact authored graph references.");
            _cameraProvider = _cameraProviderBehaviour as IInputCameraSnapshotProvider;
            if (_cameraProvider == null || !ReferenceEquals(_cameraProvider.BoundPlayer, _player) ||
                !ReferenceEquals(_transfer.BoundPlayer, _player) || !ReferenceEquals(_combat.BoundPlayer, _player))
                throw new InvalidOperationException("InputRouter camera or consumer player binding is cross-wired.");

            var firstTick = _player.NextExpectedTick;
            _transfer.InitializeForInputRouter();
            // The entire preflight is read-only; registrations follow only after all pass.
            _player.ValidateCoordinatedInputRegistration(this, this, firstTick);
            _transfer.ValidateCoordinatedInputRegistration(this, this, firstTick);
            _combat.ValidateCoordinatedInputRegistration(this, this, firstTick);
            if (_transfer.IsInputGateLocked != _player.IsInputGateLocked || _combat.IsInputGateLocked != _player.IsInputGateLocked)
                throw new InvalidOperationException("InputRouter consumers have incoherent initial input gates.");
            _player.RegisterCoordinatedInputOwner(this, this, firstTick);
            _transfer.RegisterCoordinatedInputOwner(this, this, firstTick);
            _combat.RegisterCoordinatedInputOwner(this, this, firstTick);
            _mode = _initialMode; _frameOrdinal = 0; _modeEpoch = 0;
            SwitchMaps(_mode, true);
            if (_localRequireNeutralStart) { _localNeutralPending = true; _localEnableUpdate = UnityEngine.InputSystem.LowLevel.InputState.updateCount; }
            _initialized = true;
        }

        // REQ-M5D7Q0-002..005. This branch is intentionally before every
        // gameplay/camera dereference and registers no M5B5 consumer.
        private void InitializeHubUiOnly()
        {
            if (!_authoredHubUiOnly || !_hubUiOnlyIntentProof || _initialMode != InputMode.UIOnly || _player != null || _transfer != null || _combat != null || _terminalTeardown != null || _cameraProviderBehaviour != null ||
                _modeRequester != null || _pendingModeRequest.HasValue || _currentReceipt.HasValue || _currentUiFrame.HasValue || _uiFramePublicationProof.HasValue || _uiPublicationProof.HasValue ||
                _hubNextTick.Value != (_closingHost==null?0:_closingHost.FirstUiTick.Value) || _hubNextTickProof.Value != _hubNextTick.Value || _frameOrdinal != 0 || _hubFrameOrdinalProof != 0 || _modeEpoch != 0)
                throw new InvalidOperationException("HubUIOnly graph or publication baseline is malformed.");
            _mode = InputMode.UIOnly;
            SwitchMaps(_mode, true);
            _chapterHubOwner=GetComponent<Chapter1HubLifetimeOwner>();
            if(_chapterHubOwner!=null)
            {
                if(!ReferenceEquals(_chapterHubOwner.Router,this))throw new InvalidOperationException("CH1 거점의 실제 라우터 결속이 다릅니다.");
                _chapterHubArmUpdate=UnityEngine.InputSystem.LowLevel.InputState.updateCount;_chapterHubArmReceipt=null;
                _chapterHubNeutralReady=false;_chapterHubNeutralReceipt=null;
            }
            _hubUiOnlyInitialized = true;
            _initialized = true;
        }

        private void FixedUpdate()
        {
            if (_resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this))
            {
                // A completed UIOnlyBlocked lane is intentionally quiescent,
                // not a new fault on every fixed tick.  Only a reflected local
                // resurrection or an impossible lifecycle is contained.
                if (!_resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortTerminal(this) || _resetCutoverLifecycle < ResetCutoverLifecycleV1.Entered || _resetCutoverLifecycle > ResetCutoverLifecycleV1.Completed)
                    LatchResetCutoverFailure();
                return;
            }
            ValidatePreparedInvariantOrClose();
#if UNITY_EDITOR
            if(_reviewActionsPrepared)_reviewInputOwner.RequireReviewProductionForAuthoring(this);
#endif
            if (!_initialized || _faulted) return;
            CommitNextFrame(); ValidatePreparedInvariantOrClose();
        }

        private void CommitNextFrame()
        {
            if (ExecutionSessionBlocked()) return;
            if (IsHubUiOnlyVariant) { CommitHubUiOnlyFrame(); return; }
            // REQ-M5D7PA-006. A prior reflected publication is terminal before
            // a new candidate can consume any pending UI batch.
            ValidateUiPublicationOrFault();
            _committedAim = _committedAttack = false;
            _faultStage = InputRouterFaultStage.Preflight;
            var tick = _player.NextExpectedTick;
            if (!_combatRetired && _terminalTeardown != null && _terminalTeardown.MatchesRetiredInputGraph(_player, _transfer, _combat, tick))
                _combatRetired = true;
            var requestedMode = _pendingModeRequest.HasValue ? _pendingModeRequest.Value.Mode : _mode;
            if (requestedMode == InputMode.GameplayEnabled && (_combatRetired || (!_chapterRegular && _chapterBarrier==null && _combat.HasCompletedBossDeath)))
            {
                _faultStage = InputRouterFaultStage.TransitionRequired;
                throw new InvalidOperationException("TransitionRequired: completed boss death or retirement requires a locked mode.");
            }
            var localBarrier = _localRequireNeutralStart && (ObserveLocalNeutralBoundary() || UnityEngine.InputSystem.LowLevel.InputState.updateCount == _localEnableUpdate);
            var locked = requestedMode != InputMode.GameplayEnabled;
            if (localBarrier) { ClearModeEpochBatch(); ClearCommittedUiBatch(false); }
            var modeChanging = requestedMode != _mode;
            var suppressCommands = locked || modeChanging || localBarrier;
            InputFrameCommitReceipt receipt;
            try
            {
                var proposedEpoch = requestedMode == _mode ? _modeEpoch : checked(_modeEpoch + 1);
                receipt = new InputFrameCommitReceipt(tick, _frameOrdinal, proposedEpoch, requestedMode);
                var movement = BuildMovement(tick, suppressCommands);
                SimulationCameraPoseSnapshot? camera = null;
                var cameraRejected = false;
                AimSample? aim = null;
                _cameraDisposition = InputCameraDisposition.NoAim;
                if (!suppressCommands) aim = BuildAim(tick, out camera, out cameraRejected);
                var transfer = BuildTransfer(tick, suppressCommands, aim, camera);
                var combat = BuildCombat(tick, suppressCommands, aim, camera);
                // Every checked successor is established before any consumer mutation.
                var nextFrameOrdinal = checked(_frameOrdinal + 1);
                var nextAimSampleId = _committedAim ? checked(_nextAimSampleId + 1) : _nextAimSampleId;
                var nextAttackSequenceId = _committedAttack ? checked(_nextAttackSequenceId + 1) : _nextAttackSequenceId;
                var nextDiagnostic = new InputRouterDiagnostic(requestedMode, tick, InputRouterFaultStage.None,
                    _cameraDisposition, requestedMode == InputMode.GameplayEnabled, requestedMode == InputMode.UIOnly, _unboundGameplay, _unboundUi);
                // REQ-M5D7PA-003. The candidate is fully immutable and
                // validated before any Movement/Transfer/Combat mutation.
                var uiCandidate = BuildUiSemanticFrame(receipt, modeChanging || localBarrier);

                _faultStage = InputRouterFaultStage.PrepareMovement; CheckInjectedFault(_faultStage);
                var movementCandidate = _player.PrepareInputFrame(this, tick, locked, movement, receipt);
                _faultStage = InputRouterFaultStage.PrepareTransfer; CheckInjectedFault(_faultStage);
                var transferCandidate = _transfer.PrepareInputFrame(this, tick, locked, transfer, receipt);
                OrdanBossCombatSimulationDriver.InputFrameCandidate combatCandidate = null;
                CombatSimulationDriver.InputFrameCandidate regularCandidate = null;
                BarrierInputCandidate barrierCandidate=null;
                if(_chapterBarrier!=null)barrierCandidate=_chapterBarrier.PrepareInputFrame(this,tick,locked,receipt);
                if(_chapterRegular){_faultStage=InputRouterFaultStage.PrepareCombat;CheckInjectedFault(_faultStage);regularCandidate=_chapterSink.Prepare(tick,locked,combat,receipt);}
                if (!_chapterRegular && _chapterBarrier==null && !_combatRetired) { _faultStage = InputRouterFaultStage.PrepareCombat; CheckInjectedFault(_faultStage); combatCandidate = _combat.PrepareInputFrame(this, tick, locked, combat, receipt); }
                _faultStage = InputRouterFaultStage.ValidateMovement; CheckInjectedFault(_faultStage); _player.ValidateInputFrameCandidate(this, movementCandidate);
                _faultStage = InputRouterFaultStage.ValidateTransfer; CheckInjectedFault(_faultStage); _transfer.ValidateInputFrameCandidate(this, transferCandidate);
                if(_chapterRegular){_faultStage=InputRouterFaultStage.ValidateCombat;CheckInjectedFault(_faultStage);_chapterSink.Validate(regularCandidate);}
                if(_chapterBarrier!=null)_chapterBarrier.ValidateInputFrame(this,barrierCandidate);
                if (!_chapterRegular && _chapterBarrier==null && !_combatRetired) { _faultStage = InputRouterFaultStage.ValidateCombat; CheckInjectedFault(_faultStage); _combat.ValidateInputFrameCandidate(this, combatCandidate); }
                _faultStage = InputRouterFaultStage.CommitMovement; CheckInjectedFault(_faultStage); _player.CommitInputFrame(this, movementCandidate);
                _faultStage = InputRouterFaultStage.AfterMovementCommit; CheckInjectedFault(_faultStage);
                _faultStage = InputRouterFaultStage.CommitTransfer; CheckInjectedFault(_faultStage); _transfer.CommitInputFrame(this, transferCandidate);
                _faultStage = InputRouterFaultStage.AfterTransferCommit; CheckInjectedFault(_faultStage);
                if(_chapterRegular){_faultStage=InputRouterFaultStage.CommitCombat;CheckInjectedFault(_faultStage);_chapterSink.Commit(regularCandidate);}
                if(_chapterBarrier!=null)_chapterBarrier.CommitInputFrame(this,barrierCandidate);
                if (!_chapterRegular && _chapterBarrier==null && !_combatRetired) { _faultStage = InputRouterFaultStage.CommitCombat; CheckInjectedFault(_faultStage); _combat.CommitInputFrame(this, combatCandidate); }
                if (!_combatRetired) { _faultStage = InputRouterFaultStage.AfterCombatCommit; CheckInjectedFault(_faultStage); }
                _faultStage = InputRouterFaultStage.MapSwitch; CheckInjectedFault(_faultStage);
                SwitchMaps(requestedMode, false);

                // No fallible operation follows this point (AC-M5B5-002/007).
                if (requestedMode != _mode) ClearModeEpochBatch();
                ClearFrozenBatch(nextAimSampleId, nextAttackSequenceId);
                ClearCommittedUiBatch(requestedMode == _mode && requestedMode == InputMode.UIOnly);
                _mode = requestedMode; _modeEpoch = proposedEpoch; _pendingModeRequest = null;
                _frameOrdinal = nextFrameOrdinal; _lastDiagnostic = nextDiagnostic;
                _unboundGameplay = _unboundUi = false; _faultStage = InputRouterFaultStage.None;
                // REQ-M5D7PA-003/006. These assignments cannot throw: frame
                // was validated above, then the independent proof precedes the
                // authoritative receipt, which remains globally last.
                _currentUiFrame = uiCandidate;
                _uiFramePublicationProof = uiCandidate;
                _uiPublicationProof = receipt;
                _currentReceipt = receipt; // The globally authoritative assignment is deliberately last.
            }
            catch
            {
                // Preparation failures are retryable. A partial commit or map failure is terminal.
                if (_faultStage >= InputRouterFaultStage.CommitMovement) LatchFault(_faultStage);
                throw;
            }
        }

        // REQ-M5D7Q0-003/004/005/008. One local, UI-only receipt/frame
        // transaction. No gameplay candidate, consumer, barrier, or camera is
        // touched; all fallible work precedes pending-batch consumption.
        private void CommitHubUiOnlyFrame()
        {
            try
            {
                ValidateHubUiOnlyRuntime();
                ValidateUiPublicationOrFault();
                var receipt = new InputFrameCommitReceipt(_hubNextTick, _frameOrdinal, 0, InputMode.UIOnly);
                var frame = BuildUiSemanticFrame(receipt, false);
                if (!frame.HasValue) throw new InvalidOperationException("HubUIOnly failed to construct a semantic frame.");
                // REQ-M5D7Q0-008 / AC-M5D7Q0-007. A checked successor is a
                // terminal hub-graph fault, exposed through the router's
                // established InvalidOperationException boundary rather than
                // leaking the arithmetic implementation detail to callers.
                SimulationTick nextTick;
                long nextOrdinal;
                try
                {
                    nextTick = new SimulationTick(checked(_hubNextTick.Value + 1));
                    nextOrdinal = checked(_frameOrdinal + 1);
                }
                catch (OverflowException exception)
                {
                    throw new InvalidOperationException("HubUIOnly frame successor overflowed.", exception);
                }
                var diagnostic = new InputRouterDiagnostic(InputMode.UIOnly, _hubNextTick, InputRouterFaultStage.None,
                    InputCameraDisposition.NoAim, false, true, false, false);

                var chapterDiscard=false;
                var inputUpdate=UnityEngine.InputSystem.LowLevel.InputState.updateCount;
                if(_chapterHubOwner!=null)
                {
                    chapterDiscard=ObserveChapter1HubNeutralFrame(receipt,inputUpdate);
                    if(chapterDiscard)frame=UiSemanticFrameV1.Empty(receipt);
                    else
                    {
                        var actual=frame.Value;
                        var click=actual.ClickPressed && _chapterHubClickUpdate>_chapterHubNeutralUpdate;
                        frame=new UiSemanticFrameV1(receipt,actual.NavigateXQ4096,actual.NavigateYQ4096,actual.NavigateChanged,
                            actual.HasPoint,actual.PointXActualPx,actual.PointYActualPx,actual.PointChanged,
                            click,click && actual.HasClickPoint,click?actual.ClickPointXActualPx:0,click?actual.ClickPointYActualPx:0,
                            actual.ScrollXQ4096,actual.ScrollYQ4096,actual.SubmitPressed && _chapterHubSubmitUpdate>_chapterHubNeutralUpdate,actual.CancelPressed);
                        frame.Value.Validate();
                    }
                }
                else if(!_chapterHubNeutralReady)
                {
                    var neutral=!_actions.UI.Submit.IsPressed() && !_actions.UI.Click.IsPressed();
                    foreach(var control in _actions.UI.Navigate.controls)if(control.EvaluateMagnitude()>0f)neutral=false;
                    if(neutral)_chapterHubNeutralReady=true;
                }
                ClearCommittedUiBatch(!chapterDiscard);
                _hubNextTick = nextTick; _hubNextTickProof = nextTick;
                _frameOrdinal = nextOrdinal; _hubFrameOrdinalProof = nextOrdinal;
                _lastDiagnostic = diagnostic; _faultStage = InputRouterFaultStage.None;
                if(_chapterHubOwner!=null)_chapterHubFrameInputUpdate=inputUpdate;
                _currentUiFrame = frame; _uiFramePublicationProof = frame; _uiPublicationProof = receipt;
                _currentReceipt = receipt; // authoritative publication is last.
            }
            catch
            {
                if (!_faulted) LatchFault(InputRouterFaultStage.HubUiOnly);
                throw;
            }
        }

        private MovementCommand? BuildMovement(SimulationTick tick, bool locked)
        {
            if (locked) return null;
            return new MovementCommand(tick, QuantizeAxis(_move.x), QuantizeAxis(_move.y), _jumpPressed, _jumpReleased, _dashPressed);
        }

        // REQ-M5D7PA-002/003. A UI candidate is a value-only snapshot; it
        // neither reads action state nor consumes the live pending batch.
        private UiSemanticFrameV1? BuildUiSemanticFrame(InputFrameCommitReceipt receipt, bool modeChanging)
        {
            if (receipt.Mode != InputMode.UIOnly) return null;
            if(_closingHost!=null)
            {
                var neutral=!modeChanging && !_uiEnableQuarantinePending;
                foreach(var action in _actions.UI.Get().actions)
                    if(action.type==InputActionType.Button || action==_actions.UI.Navigate)
                        foreach(var control in action.controls)if(control.EvaluateMagnitude()>0f)neutral=false;
                _closingPhysicalNeutral=neutral;_closingPhysicalOrdinal=receipt.FrameOrdinal;
            }
            var frame = modeChanging
                ? UiSemanticFrameV1.Empty(receipt)
                : new UiSemanticFrameV1(receipt, _uiNavigateXQ4096, _uiNavigateYQ4096, _uiNavigateChanged,
                    _uiHasPoint, _uiPointXActualPx, _uiPointYActualPx, _uiPointChanged,
                    _uiClickPressed, _uiHasClickPoint, _uiClickPointXActualPx, _uiClickPointYActualPx,
                    _uiScrollXQ4096, _uiScrollYQ4096, _uiSubmitPressed, _uiCancelPressed);
            frame.Validate();
            return frame;
        }

        private void ClearCommittedUiBatch(bool retainCurrentValues)
        {
            // REQ-M5D7PA-004. Only a same-epoch UI success retains current
            // Navigate/Point; all transients are one fixed-frame facts.
            _uiNavigateChanged = _uiPointChanged = false;
            _uiClickPressed = _uiHasClickPoint = _uiSubmitPressed = _uiCancelPressed = false;
            _uiClickPointXActualPx = _uiClickPointYActualPx = 0;
            _uiScrollXQ4096 = _uiScrollYQ4096 = 0;
            if (retainCurrentValues) return;
            _uiNavigateXQ4096 = _uiNavigateYQ4096 = 0;
            _uiHasPoint = false; _uiPointXActualPx = _uiPointYActualPx = 0;
        }

        private static void ValidateReceipt(InputFrameCommitReceipt receipt)
        {
            var canonical = new InputFrameCommitReceipt(receipt.Tick, receipt.FrameOrdinal, receipt.ModeEpoch, receipt.Mode);
            if (!canonical.Equals(receipt)) throw new InvalidOperationException("Input receipt is malformed.");
        }

        // REQ-M5D7Q0-003/006/008. This runtime witness is intentionally
        // checked at source reads and commits; it preserves forensic receipt/
        // frame data while a malformed hub graph immediately closes maps.
        private void ValidateHubUiOnlyRuntimeOrFault()
        {
            // REQ-M5D7Q0-008. Either half of the closed discriminator selects
            // hub validation: clearing the serialized flag after successful
            // initialization cannot reveal a stale semantic publication.
            if (!IsHubUiOnlyVariant) return;
            // REQ-M5D7Q0-008. The fields survive a post-publication fault for
            // forensics, but the semantic source is no longer readable.
            if (_faulted) throw new InvalidOperationException("HubUIOnly semantic source is invalidated by fault.");
            try { ValidateHubUiOnlyLifecycle(); }
            catch
            {
                // Before confirmation no caller may revive a corrupted adopted
                // candidate: contain through the existing failed owner row.
                if (_preparedLaunchState == PreparedLaunchState.Reserved || _preparedLaunchState == PreparedLaunchState.Adopted || _preparedLaunchState == PreparedLaunchState.InitializedPendingPublication)
                {
                    _preparedLaunchState = PreparedLaunchState.Failed;
                    _faulted = true;
                    _preparedLaunchFaultStage = PreparedLaunchFaultStage.Closed;
                    CloseActionsOnce();
                }
                else if (!_faulted)
                {
                    LatchFault(InputRouterFaultStage.HubUiOnly);
                    // An invalid pre-Awake/pre-reservation hub has no adopted
                    // candidate, but must still occupy M5D7M's legal closed
                    // action-free fault row rather than an impossible live one.
                    if (_preparedLaunchState == PreparedLaunchState.Unreserved && !_initialized) CloseActionsOnce();
                }
                throw;
            }
        }

        // REQ-M5D7Q0-001/002/008. The adapter reaches Reserve before it can
        // prepare a profile or take a candidate. A one-sided marker is thus a
        // closed action-free terminal before every external launch operation.
        private void ValidateHubIntentBeforePreparedBoundary()
        {
            if (!IsHubUiOnlyVariant || (_authoredHubUiOnly && _hubUiOnlyIntentProof)) return;
            // REQ-M5D7Q0-002/008.  Reserve runs before an owner exists, so a
            // malformed intent must occupy the historical action-free closed
            // Unreserved row.  Once an exact owner exists, use the M5D7M
            // owner-bound failure transition instead: it produces Failed and
            // closes an adopted candidate exactly once, never a faulted
            // Reserved/Adopted row.
            if (_preparedLaunchState == PreparedLaunchState.Reserved ||
                _preparedLaunchState == PreparedLaunchState.Adopted ||
                _preparedLaunchState == PreparedLaunchState.InitializedPendingPublication)
            {
                var owner = _preparedLaunchOwner;
                if (owner != null && ReferenceEquals(owner, _preparedLaunchOwnerProof))
                {
                    FailPreparedHubLaunch(owner);
                    throw new InvalidOperationException("HubUIOnly authoring markers are malformed.");
                }
            }

            LatchFault(InputRouterFaultStage.HubUiOnly);
            if (_preparedLaunchState == PreparedLaunchState.Unreserved && !_initialized) CloseActionsOnce();
            throw new InvalidOperationException("HubUIOnly authoring markers are malformed.");
        }

        // REQ-M5D7Q0-002/004/008. The hub discriminator is checked before
        // initialization as well as after it, while retaining the legitimate
        // M5D7M Reserved/Adopted/Pending rows.
        private void ValidateHubUiOnlyLifecycle()
        {
            if (!_authoredHubUiOnly || !_hubUiOnlyIntentProof || _initialMode != InputMode.UIOnly || _player != null || _transfer != null || _combat != null ||
                _terminalTeardown != null || _cameraProviderBehaviour != null || _cameraProvider != null || _modeRequester != null || _pendingModeRequest.HasValue ||
                _modeEpoch != 0 || ((_preparedLaunchOwner == null) != (_preparedLaunchOwnerProof == null)) ||
                (_preparedLaunchOwner != null && !ReferenceEquals(_preparedLaunchOwner, _preparedLaunchOwnerProof)))
                throw new InvalidOperationException("HubUIOnly lifecycle graph is malformed.");
            if (_preparedLaunchState == PreparedLaunchState.Unreserved)
            {
                if (_hubUiOnlyInitialized || _initialized || _actions != null || _preparedActionsProof != null || _preparedActionsContainmentProof != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered || _gameplayCallbacksProof != null || _uiCallbacksProof != null || _mode != InputMode.Transition)
                    throw new InvalidOperationException("HubUIOnly unreserved lifecycle is malformed.");
                return;
            }
            if (_preparedLaunchState == PreparedLaunchState.Reserved)
            {
                if (_hubUiOnlyInitialized || _initialized || _preparedLaunchOwner == null || _actions != null || _preparedActionsProof != null || _preparedActionsContainmentProof != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered || _gameplayCallbacksProof != null || _uiCallbacksProof != null || _mode != InputMode.Transition)
                    throw new InvalidOperationException("HubUIOnly reserved lifecycle is malformed.");
                return;
            }
            if (_preparedLaunchState == PreparedLaunchState.Adopted)
            {
                if (_hubUiOnlyInitialized || _initialized || _preparedLaunchOwner == null || _actions == null || !ReferenceEquals(_actions, _preparedActionsProof) || !ReferenceEquals(_actions, _preparedActionsContainmentProof) || _gameplayCallbacksRegistered || _uiCallbacksRegistered || _gameplayCallbacksProof != null || _uiCallbacksProof != null || _mode != InputMode.Transition)
                    throw new InvalidOperationException("HubUIOnly adopted lifecycle is malformed.");
                return;
            }
            if (_preparedLaunchState == PreparedLaunchState.InitializedPendingPublication)
            {
                if (!_hubUiOnlyInitialized || !_initialized || _preparedLaunchOwner == null || _actions == null || !ReferenceEquals(_actions, _preparedActionsProof) || !ReferenceEquals(_actions, _preparedActionsContainmentProof) || !_gameplayCallbacksRegistered || !_uiCallbacksRegistered || !ReferenceEquals(_gameplayCallbacksProof, _preparedActionsProof) || !ReferenceEquals(_uiCallbacksProof, _preparedActionsProof) || _mode != InputMode.UIOnly || _actions.Gameplay.Get().enabled || !_actions.UI.Get().enabled)
                    throw new InvalidOperationException("HubUIOnly pending-publication lifecycle is malformed.");
                return;
            }
            if (_preparedLaunchState == PreparedLaunchState.Initialized) { ValidateHubUiOnlyRuntime(); return; }
            throw new InvalidOperationException("HubUIOnly lifecycle is terminal or unknown.");
        }

        private void ValidateHubUiOnlyRuntime()
        {
            if (!_hubUiOnlyInitialized || !_authoredHubUiOnly || !_hubUiOnlyIntentProof || !_initialized || _preparedLaunchState != PreparedLaunchState.Initialized ||
                _initialMode != InputMode.UIOnly || _mode != InputMode.UIOnly || _modeEpoch != 0 || _player != null || _transfer != null || _combat != null ||
                _terminalTeardown != null || _cameraProviderBehaviour != null || _cameraProvider != null || _modeRequester != null || _pendingModeRequest.HasValue ||
                _hubNextTick.Value != _hubNextTickProof.Value || _frameOrdinal != _hubFrameOrdinalProof ||
                _preparedLaunchOwner == null || !ReferenceEquals(_preparedLaunchOwner, _preparedLaunchOwnerProof) || _actions == null || !ReferenceEquals(_actions, _preparedActionsProof) || !ReferenceEquals(_actions, _preparedActionsContainmentProof) || !_gameplayCallbacksRegistered || !_uiCallbacksRegistered || !ReferenceEquals(_gameplayCallbacksProof, _preparedActionsProof) || !ReferenceEquals(_uiCallbacksProof, _preparedActionsProof))
                throw new InvalidOperationException("HubUIOnly runtime matrix is malformed.");
            if (!_faulted && (_actions.Gameplay.Get().enabled || !_actions.UI.Get().enabled))
                throw new InvalidOperationException("HubUIOnly map state is malformed.");
        }

        private void ValidateUiPublicationOrFault()
        {
            // REQ-M5D7PA-006. Terminal faults preserve their original stage,
            // but do not turn the receipt/proof/frame getter into a blind spot.
            // A later reflected corruption remains observable and throws.
            var wasFaulted = _faulted;
            try
            {
                if (!_currentReceipt.HasValue && !_uiPublicationProof.HasValue && !_currentUiFrame.HasValue && !_uiFramePublicationProof.HasValue) return;
                if (!_currentReceipt.HasValue || !_uiPublicationProof.HasValue) throw new InvalidOperationException("UI publication presence is malformed.");
                var receipt = _currentReceipt.Value;
                ValidateReceipt(receipt); ValidateReceipt(_uiPublicationProof.Value);
                if (!receipt.Equals(_uiPublicationProof.Value)) throw new InvalidOperationException("UI publication proof does not match receipt.");
                if (receipt.Mode == InputMode.UIOnly)
                {
                    if (!_currentUiFrame.HasValue || !_uiFramePublicationProof.HasValue) throw new InvalidOperationException("UI receipt has no semantic frame.");
                    _currentUiFrame.Value.Validate();
                    _uiFramePublicationProof.Value.Validate();
                    if (!_currentUiFrame.Value.Receipt.Equals(receipt)) throw new InvalidOperationException("UI frame receipt does not match publication.");
                    if (!_uiFramePublicationProof.Value.Receipt.Equals(receipt) || !_currentUiFrame.Value.Equals(_uiFramePublicationProof.Value)) throw new InvalidOperationException("UI frame proof does not match publication.");
                }
                else if (_currentUiFrame.HasValue || _uiFramePublicationProof.HasValue) throw new InvalidOperationException("Non-UI receipt has a semantic frame.");
            }
            catch
            {
                // REQ-M5D7PA-006. Retain forensic fields; never manufacture a
                // replacement publication after a reflected invariant breach.
                // Do not overwrite an earlier terminal fault's stage/diagnostic.
                if (!wasFaulted) LatchFault(InputRouterFaultStage.UiCapture);
                throw;
            }
        }

        private TransferSimulationInput? BuildTransfer(SimulationTick tick, bool locked, AimSample? aim, SimulationCameraPoseSnapshot? camera)
        {
            if (locked) return null;
            TransferPressed? press = null;
            if (_transferPressed && (!_transferFromMouse || (aim.HasValue && aim.Value.Source == AimSource.MousePointer && aim.Value.IsPointerInsideGameplayRect == true)))
                press = new TransferPressed(aim.HasValue ? (long?)aim.Value.SampleId : null, tick);
            if(!aim.HasValue && !press.HasValue && !_choicePressed)return null;
            var input=new TransferSimulationInput(tick,camera,aim,press,Array.Empty<AcadeGameMaker.Transfer.TransferTargetRemoved>(),null);
            return _choicePressed?input.WithChoice(_choiceEdgeOrdinal):input;
        }

        private OrdanBossCombatInput? BuildCombat(SimulationTick tick, bool locked, AimSample? aim, SimulationCameraPoseSnapshot? camera)
        {
            if (locked || !aim.HasValue) return null;
            BasicAttackPressed? press = null;
            if (_attackPressed && (!_attackFromMouse || aim.Value.IsPointerInsideGameplayRect == true))
            {
                press = new BasicAttackPressed(_nextAttackSequenceId, aim.Value.SampleId, tick);
                _committedAttack = true;
            }
            return new OrdanBossCombatInput(tick, camera, aim, press, Array.Empty<DamageRequest>());
        }

        private AimSample? BuildAim(SimulationTick tick, out SimulationCameraPoseSnapshot? camera, out bool cameraRejected)
        {
            camera = null; cameraRejected = false;
            if (!_activeAimSource.HasValue) { _cameraDisposition = InputCameraDisposition.NoAim; return null; }
            if (tick.Value <= 0) { _cameraDisposition = InputCameraDisposition.BootstrapPending; cameraRejected = true; return null; }
            var expectedPose = new SimulationTick(checked(tick.Value - 1));
            var snapshot = _player.Snapshot;
            if (!ReferenceEquals(_cameraProvider.BoundPlayer, _player) || snapshot.Tick != expectedPose)
            { _cameraDisposition = InputCameraDisposition.CameraRejected; cameraRejected = true; return null; }
            if (!_cameraProvider.TryGetCompletedCamera(expectedPose, out var completed))
            { _cameraDisposition = InputCameraDisposition.CameraUnavailable; cameraRejected = true; return null; }
            try
            {
                completed = new SimulationCameraPoseSnapshot(completed.CameraPoseTick, completed.PositionXQ100, completed.PositionYQ100, completed.OrthoSizeQ1000, completed.RotationQ10, completed.ViewportWidth, completed.ViewportHeight, completed.GameplayRectX, completed.GameplayRectY, completed.GameplayRectWidth, completed.GameplayRectHeight, completed.IntegerScale);
                if (completed.CameraPoseTick != expectedPose || completed.OrthoSizeQ1000 != TransferFixedMath.OrthoSizeQ1000 || completed.RotationQ10 != TransferFixedMath.RotationQ10) throw new InvalidOperationException();
            }
            catch { _cameraDisposition = InputCameraDisposition.CameraRejected; cameraRejected = true; return null; }
            if (_activeAimSource.Value == AimSource.GamepadStick)
            {
                var normalized = NormalizeStick(_stick);
                if (normalized.x == 0 && normalized.y == 0) return null;
                camera = completed;
                _cameraDisposition = InputCameraDisposition.Valid; _committedAim = true;
                return new AimSample(_nextAimSampleId, AimSource.GamepadStick, null, null, null, normalized.x, normalized.y, expectedPose, tick);
            }
            var px = _pointX; var py = _pointY;
            var inside = px >= completed.GameplayRectX && px < checked(completed.GameplayRectX + completed.GameplayRectWidth) &&
                py >= completed.GameplayRectY && py < checked(completed.GameplayRectY + completed.GameplayRectHeight);
            var dxPixels = Math.Max(0L, Math.Min((long)completed.GameplayRectWidth - 1L, checked((long)px - completed.GameplayRectX)));
            var dyPixels = Math.Max(0L, Math.Min((long)completed.GameplayRectHeight - 1L, checked((long)py - completed.GameplayRectY)));
            var sx = checked((int)TransferFixedMath.RoundDivAway(checked(dxPixels * 1919L), Math.Max(1, completed.GameplayRectWidth - 1)));
            var sy = checked((int)TransferFixedMath.RoundDivAway(checked(dyPixels * 1079L), Math.Max(1, completed.GameplayRectHeight - 1)));
            sx = Mathf.Clamp(sx, 0, 1919); sy = Mathf.Clamp(sy, 0, 1079);
            var world = TransferFixedMath.InverseGridToWorld(completed, sx, sy);
            var originX = TransferObservationBuilder.Q4096ToQ1000(snapshot.PositionXQ4096);
            var originY = TransferObservationBuilder.Q4096ToQ1000(snapshot.PositionYQ4096);
            var directionX = checked((int)checked((long)world.xQ1000 - originX)); var directionY = checked((int)checked((long)world.yQ1000 - originY));
            if (!TransferFixedMath.HasMouseDirection(directionX, directionY)) return null;
            var direction = TransferFixedMath.NormalizeMouseDirectionQ4096(directionX, directionY);
            if (direction.xQ4096 == 0 && direction.yQ4096 == 0) return null;
            camera = completed;
            _cameraDisposition = InputCameraDisposition.Valid; _committedAim = true;
            return new AimSample(_nextAimSampleId, AimSource.MousePointer, sx, sy, inside, direction.xQ4096, direction.yQ4096, expectedPose, tick);
        }

        private void SwitchMaps(InputMode mode, bool force)
        {
            if (_resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this)) throw new InvalidOperationException("Reset terminal router cannot switch maps.");
            var gameplay = _actions.Gameplay.Get(); var ui = _actions.UI.Get();
            var wantGameplay = mode == InputMode.GameplayEnabled; var wantUi = mode == InputMode.UIOnly;
            if (!force && gameplay.enabled == wantGameplay && ui.enabled == wantUi) return;
            _captureSuppressed = true;
            try
            {
                if (gameplay.enabled) gameplay.Disable();
                if (ui.enabled) { ui.Disable(); CancelUiEnableQuarantine(); }
                if (!force) { _faultStage = InputRouterFaultStage.MapsDisabled; CheckInjectedFault(_faultStage); }
                if (wantGameplay) gameplay.Enable();
                else if (wantUi) { BeginUiEnableQuarantine(); ui.Enable(); }
                if (!force) { _faultStage = InputRouterFaultStage.TargetMapEnabled; CheckInjectedFault(_faultStage); }
            }
            finally { if (!_uiEnableQuarantinePending) _captureSuppressed = false; }
        }

        private void BeginUiEnableQuarantine()
        {
            if (_uiEnableQuarantinePending || _uiEnableQuarantineSubscribed) throw new InvalidOperationException("UI enable quarantine is already active.");
            _captureSuppressed = true;
            _uiEnableQuarantinePending = _uiEnableQuarantineSubscribed = true;
            InputSystem.onAfterUpdate += ReleaseUiEnableQuarantineAfterInputUpdate;
        }

        private void ReleaseUiEnableQuarantineAfterInputUpdate()
        {
            // This is a one-shot ownership hook, not another Input callback
            // subscriber.  Its only job is to release the map-enable epoch
            // barrier after every callback in that InputSystem update was
            // deliberately ignored by Accept.
            if (!_uiEnableQuarantineSubscribed) return;
            InputSystem.onAfterUpdate -= ReleaseUiEnableQuarantineAfterInputUpdate;
            _uiEnableQuarantineSubscribed = _uiEnableQuarantinePending = false;
            if (!_faulted && !_disposed && _actions != null && _actions.UI.Get().enabled && _mode == InputMode.UIOnly)
                _captureSuppressed = false;
        }

        private void CancelUiEnableQuarantine()
        {
            if (_uiEnableQuarantineSubscribed) InputSystem.onAfterUpdate -= ReleaseUiEnableQuarantineAfterInputUpdate;
            _uiEnableQuarantineSubscribed = _uiEnableQuarantinePending = false;
        }

        private void ClearFrozenBatch(long nextAimSampleId, long nextAttackSequenceId)
        {
            _choicePressed=false;
            _nextAttackSequenceId = nextAttackSequenceId;
            _nextAimSampleId = nextAimSampleId;
            _committedAim = _committedAttack = false;
            _jumpPressed = _jumpReleased = _dashPressed = _attackPressed = _transferPressed = false;
            _attackFromMouse = _transferFromMouse = false;
        }

        private void ClearModeEpochBatch()
        {
            _choicePressed=false;
            _jumpPressed = _jumpReleased = _dashPressed = _attackPressed = _transferPressed = false;
            _attackFromMouse = _transferFromMouse = false;
            _activeAimSource = null; _pointOrdinal = _stickOrdinal = _callbackOrdinal = 0;
            _move = _stick = Vector2.zero; _pointX = _pointY = 0;
        }

        // REQ-M5D7Q0-008/AC009. Choose only an action wrapper supported by two
        // of the three independent references.  A single reflected proof/live
        // substitution therefore cannot make the router operate on a foreign
        // caller-owned wrapper; three-way disagreement has no safe owner.
        private GameInputActions ResolveHubOwnedActions()
        {
            if (ReferenceEquals(_preparedActionsProof, _preparedActionsContainmentProof))
                return _preparedActionsProof;
            if (ReferenceEquals(_actions, _preparedActionsProof)) return _preparedActionsProof;
            if (ReferenceEquals(_actions, _preparedActionsContainmentProof)) return _preparedActionsContainmentProof;
            return null;
        }

        private void LatchFault(InputRouterFaultStage stage)
        {
            CancelUiEnableQuarantine();
            _faulted = true; _faultStage = stage; _captureSuppressed = true;
            // A Hub may have had its mutable live field replaced by a caller-
            // owned wrapper.  Its adopted proof is the only map owner this
            // router may disable while containing a Hub fault.
            var actions = IsHubUiOnlyVariant ? ResolveHubOwnedActions() : _actions;
            try
            {
                // AC-M5B5-007. Shutdown is best effort, not rollback, and must
                // not replace the original commit failure with a callback error.
                try { actions?.Gameplay.Disable(); } catch { }
                try { actions?.UI.Disable(); } catch { }
            }
            finally
            {
                _captureSuppressed = false;
                // REQ-M5D7Q0-008. Hub diagnostics retain its isolated local
                // tick rather than manufacturing a gameplay tick from null.
                var tick = IsHubUiOnlyVariant ? _hubNextTick : (_player == null ? default(SimulationTick) : _player.NextExpectedTick);
                _lastDiagnostic = new InputRouterDiagnostic(_mode, tick, stage, _cameraDisposition,
                    actions != null && actions.Gameplay.Get().enabled, actions != null && actions.UI.Get().enabled, _unboundGameplay, _unboundUi);
            }
        }


        private void OnDisable()
        {
            ProfileNewGameConfirmationV1.RecordExecutionLifecycleFault(this);
            if (_recoveryReservation==RecoveryReservationStateV1.Reserved || _recoveryReservation==RecoveryReservationStateV1.RepairBlocked || _recoveryReservation==RecoveryReservationStateV1.Terminal || _recoveryReservation==RecoveryReservationStateV1.Closed) { CloseRecoveryReservation(_recoveryOwner); return; }
            if (_resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this)) { LatchResetCutoverFailure(); return; }
            CancelUiEnableQuarantine();
            if (_preparedLaunchState == PreparedLaunchState.Reserved || _preparedLaunchState == PreparedLaunchState.Adopted || _preparedLaunchState == PreparedLaunchState.InitializedPendingPublication) { _preparedLaunchState = PreparedLaunchState.Failed; _faulted = true; _preparedLaunchFaultStage = PreparedLaunchFaultStage.Closed; CloseActionsOnce(); ValidatePreparedInvariantOrClose(); return; }
            // REQ-M5D7Q0-008/012. A disabled post-publication hub is a graph
            // terminal: close its exact adopted owner now.  The full-simulation
            // path retains its historical deferred ownership behavior.
            if (IsHubUiOnlyVariant)
            {
                if (!_faulted) LatchFault(InputRouterFaultStage.HubUiOnly);
                CloseActionsOnce();
                ValidatePreparedInvariantOrClose();
                return;
            }
            if (_initialized && !_faulted) LatchFault(InputRouterFaultStage.Disabled);
            if (_actions != null) { if (_gameplayCallbacksRegistered) _actions.Gameplay.RemoveCallbacks(this); if (_uiCallbacksRegistered) _actions.UI.RemoveCallbacks(this); _gameplayCallbacksRegistered = _uiCallbacksRegistered = false; _gameplayCallbacksProof = _uiCallbacksProof = null; } ValidatePreparedInvariantOrClose();
        }
        private void OnEnable() { if (_executionGuardBlocked || _executionGuardClosed || DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this)) { LatchResetCutoverFailure(); throw new InvalidOperationException("A terminal reset router cannot be re-enabled."); } if (_initialized) throw new InvalidOperationException("An initialized InputRouter cannot be re-enabled after disable."); }
        private void OnDestroy()
        {
            _chapterClosure.Destroyed=true;
            ProfileNewGameConfirmationV1.RecordExecutionLifecycleFault(this);
            if (_recoveryReservation==RecoveryReservationStateV1.Reserved || _recoveryReservation==RecoveryReservationStateV1.RepairBlocked || _recoveryReservation==RecoveryReservationStateV1.Terminal || _recoveryReservation==RecoveryReservationStateV1.Closed) { CloseRecoveryReservation(_recoveryOwner); return; }
            if (_resetCutoverTerminal || DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this))
            {
                _disposed = true; _resetCutoverLifecycle = ResetCutoverLifecycleV1.Disposed;
                LatchResetCutoverFailure();
                return;
            }
            CancelUiEnableQuarantine();
            if (_disposed)
            {
                // REQ-M5D7Q0-010 / AC-M5D7Q0-001. Preserve the verified
                // full-simulation corruption boundary: a reflected disposed
                // legacy router must not turn a retained action collection into
                // a silently valid teardown row. The invariant helper contains
                // the collection before rethrowing the original failure.
                ValidatePreparedInvariantOrClose();
                CloseActionsOnce();
                return;
            }
            _disposed = true;
            if (_preparedLaunchState == PreparedLaunchState.Reserved || _preparedLaunchState == PreparedLaunchState.Adopted || _preparedLaunchState == PreparedLaunchState.InitializedPendingPublication)
            {
                _preparedLaunchState = PreparedLaunchState.Failed;
                _faulted = true;
                _preparedLaunchFaultStage = PreparedLaunchFaultStage.Closed;
            }
            CloseActionsOnce();
            ValidatePreparedInvariantOrClose();
        }

        private void RegisterCallbacks()
        {
            if (_actions == null || _gameplayCallbacksRegistered || _uiCallbacksRegistered) throw new InvalidOperationException("InputRouter callback registration is malformed.");
            // Each injection boundary has an exact completed-registration
            // witness: after Gameplay and before UI only gameplay is owned;
            // after UI both are owned.  Teardown uses these witnesses even if
            // a reflected test clears either compatibility bit.
            _actions.Gameplay.AddCallbacks(this);
            _gameplayCallbacksProof = _actions; _gameplayCallbacksRegistered = true;
            CheckPreparedLaunchFault(PreparedLaunchFaultInjectionStage.AfterGameplayCallbackRegistration);
            CheckPreparedLaunchFault(PreparedLaunchFaultInjectionStage.BeforeUiCallbackRegistration);
            _actions.UI.AddCallbacks(this);
            _uiCallbacksProof = _actions; _uiCallbacksRegistered = true;
            CheckPreparedLaunchFault(PreparedLaunchFaultInjectionStage.AfterUiCallbackRegistration);
        }

        // The verified M5B5 path owns this lazy allocation. Prepared launch
        // states must never call it, so Awake remains observation-only.
        private void EnsureLegacyActionsAndCallbacks()
        {
            if (_preparedLaunchState != PreparedLaunchState.Unreserved || !_awakeEntered || _startEntered || _faulted || _disposed || _actionsClosed || _actions != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered || _gameplayCallbacksProof != null || _uiCallbacksProof != null)
                throw new InvalidOperationException("Legacy InputRouter actions are not pristine.");
            AllocateLegacyActionsAndCallbacks();
        }
        private void AllocateLegacyActionsAndCallbacks()
        {
            try { _actions = new GameInputActions(); RegisterCallbacks(); }
            catch { CloseActionsOnce(); _preparedLaunchState = PreparedLaunchState.Unreserved; _preparedLaunchFaultStage = PreparedLaunchFaultStage.None; throw; }
        }

        private void CloseActionsOnce()
        {
            CancelUiEnableQuarantine();
            var hub = IsHubUiOnlyVariant;
            // Hub containment selects its immutable adopted proof, never the
            // mutable live field.  A substituted live wrapper remains caller-
            // owned and is never disabled, unsubscribed, or disposed here.
            var actions = hub ? ResolveHubOwnedActions() : _actions;
            // A safe majority result is the authoritative post-close forensic
            // identity.  This restores a singly-corrupted witness to the
            // adopted wrapper before closure validation observes the row.
            if (hub && actions != null) _preparedActionsProof = _preparedActionsContainmentProof = actions;
            if (actions != null && ReferenceEquals(_closedActionsProof, actions))
            {
                _actionsClosed = true; _actions = null;
                _gameplayCallbacksRegistered = _uiCallbacksRegistered = false;
                _gameplayCallbacksProof = _uiCallbacksProof = null;
                return;
            }
            // A reflected closed flag cannot suppress cleanup while an action
            // collection remains owned.
            if (actions == null)
            {
                _actionsClosed = true; _actions = null;
                _gameplayCallbacksRegistered = _uiCallbacksRegistered = false;
                _gameplayCallbacksProof = _uiCallbacksProof = null;
                return;
            }
            var gameplayCallbacks = _gameplayCallbacksProof;
            var uiCallbacks = _uiCallbacksProof;
            var removeGameplayCallbacks = _gameplayCallbacksRegistered || ReferenceEquals(gameplayCallbacks, actions);
            var removeUiCallbacks = _uiCallbacksRegistered || ReferenceEquals(uiCallbacks, actions) ||
                // REQ-M5D7Q0-010 / AC-M5D7Q0-001. The legacy prepared-launch
                // test seam records the boundary immediately before UI
                // registration as an attempted UI cleanup. Keep that
                // historical observable only for the non-hub test injection;
                // HubUIOnly remains strictly witness-based.
                (!hub && _testPreparedLaunchFaultStage == PreparedLaunchFaultInjectionStage.BeforeUiCallbackRegistration);
            // Establish all closure/idempotence state before any external
            // map, callback, or disposal operation can fault or re-enter.
            _actionsClosed = true; _closedActionsProof = actions; _actions = null;
            _gameplayCallbacksRegistered = _uiCallbacksRegistered = false;
            _gameplayCallbacksProof = _uiCallbacksProof = null;
            _lastClosedActions = actions;
            _closeAttempts++;
            bool chapterClean=true;
            try { actions.Gameplay.Disable(); } catch { chapterClean=false; }
            try { actions.UI.Disable(); } catch { chapterClean=false; }
            if (removeGameplayCallbacks) { _gameplayUnsubscribeAttempts++; try { actions.Gameplay.RemoveCallbacks(this); } catch { chapterClean=false; } }
            if (removeUiCallbacks) { _uiUnsubscribeAttempts++; try { actions.UI.RemoveCallbacks(this); } catch { chapterClean=false; } }
            _disposeAttempts++;
            try { actions.Dispose(); if (_localRequireNeutralStart && _disposed) _localClosure.Closed = true;
                if(chapterClean)_chapterClosure.Closed=true; } catch { }

        }

        private void ValidatePreparedInvariantOrClose()
        {
            if (DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this))
            {
                if (!_resetCutoverTerminal || _resetCutoverLifecycle < ResetCutoverLifecycleV1.Entered || _resetCutoverLifecycle > ResetCutoverLifecycleV1.Completed || DesktopProfileLaunchAdapterV1.IsRouterCohortTerminal(this)) LatchResetCutoverFailure();
                return;
            }
            try { ValidatePreparedInvariant(); }
            // Keep the corrupt state and owner as forensic evidence. Closing is
            // containment only; it must not manufacture a valid Failed row.
            catch
            {
                // A Hub action substitution before confirmation still has an
                // exact owner proof.  Move it through the existing Failed row
                // before closure; never leave a corrupt Reserved/Adopted/Pending
                // row merely because strict validation detected it first.
                if (IsHubUiOnlyVariant &&
                    (_preparedLaunchState == PreparedLaunchState.Reserved || _preparedLaunchState == PreparedLaunchState.Adopted || _preparedLaunchState == PreparedLaunchState.InitializedPendingPublication) &&
                    _preparedLaunchOwner != null && ReferenceEquals(_preparedLaunchOwner, _preparedLaunchOwnerProof))
                {
                    _preparedLaunchState = PreparedLaunchState.Failed;
                    _preparedLaunchFaultStage = PreparedLaunchFaultStage.Closed;
                }
                // AC-M5D7Q0-012: after publication, invariant detection is a
                // map/consumption fault only.  The exact adopted proof remains
                // live until Disable/Destroy; strict validation itself must not
                // turn a substituted mutable _actions field into eager closure.
                if (IsHubUiOnlyVariant && _preparedLaunchState == PreparedLaunchState.Initialized && _initialized && !_disposed)
                {
                    if (!_faulted) LatchFault(InputRouterFaultStage.HubUiOnly);
                    throw;
                }
                _faulted = true; CloseActionsOnce(); throw;
            }
        }
        private void ValidatePreparedInvariant()
        {
#if UNITY_EDITOR
            if(_reviewActionsPrepared&&!_initialized&&!_faulted&&!_disposed&&!_actionsClosed)
            {VerifyReviewActionsForAuthoring(_reviewInputOwner);if(_startEntered)throw new InvalidOperationException("사전 입력 Start 증거가 미완료입니다.");return;}
#endif
            if (_recoveryReservation == RecoveryReservationStateV1.Reserved || _recoveryReservation == RecoveryReservationStateV1.RepairBlocked || _recoveryReservation == RecoveryReservationStateV1.Terminal || _recoveryReservation == RecoveryReservationStateV1.Completed || _recoveryReservation == RecoveryReservationStateV1.Closed)
            {
                var privateUnknownRootRepair=DesktopProfileLaunchAdapterV1.IsPrivateRepairRecoveryReservation(this,_recoveryOwner);
                if (!IsHubUiOnlyVariant || _recoveryOwner==null || ((_recoveryReservation!=RecoveryReservationStateV1.RepairBlocked && _recoveryReservation!=RecoveryReservationStateV1.Closed) && string.IsNullOrEmpty(_recoveryRoot)) || (_recoveryReservation==RecoveryReservationStateV1.Closed && string.IsNullOrEmpty(_recoveryRoot) && !privateUnknownRootRepair) || _gameplayCallbacksRegistered || _uiCallbacksRegistered) throw new InvalidOperationException("Restart recovery lifecycle is malformed.");
                if (_recoveryReservation==RecoveryReservationStateV1.Reserved && (_actions!=null || _initialized || _faulted || _resetCutoverTerminal)) throw new InvalidOperationException("Restart recovery reservation allocated input.");
                if (_recoveryReservation==RecoveryReservationStateV1.RepairBlocked && (_actions!=null || _initialized || _faulted || _resetCutoverTerminal)) throw new InvalidOperationException("Restart recovery repair reservation allocated input.");
                if (_recoveryReservation==RecoveryReservationStateV1.Closed && (_actions!=null || _initialized || _gameplayCallbacksRegistered || _uiCallbacksRegistered)) throw new InvalidOperationException("Closed restart recovery reservation allocated input.");
                if (_recoveryReservation==RecoveryReservationStateV1.Completed && (!_resetCutoverTerminal || _actions==null || !ReferenceEquals(_actions,_resetNewActions))) throw new InvalidOperationException("Restart recovery completion is malformed.");
                return;
            }
            if (DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this))
            {
                if (!_resetCutoverTerminal || _resetCutoverLifecycle < ResetCutoverLifecycleV1.Entered || _resetCutoverLifecycle > ResetCutoverLifecycleV1.Completed || DesktopProfileLaunchAdapterV1.IsRouterCohortTerminal(this)) throw new InvalidOperationException("Reset terminal lifecycle is malformed.");
                return;
            }
            if (_uiEnableQuarantinePending != _uiEnableQuarantineSubscribed) throw new InvalidOperationException("UI enable quarantine subscription is malformed.");
            if (_uiEnableQuarantinePending && (!_captureSuppressed || _faulted || _disposed || _actions == null || !_actions.UI.Get().enabled)) throw new InvalidOperationException("UI enable quarantine state is malformed.");
            if (_preparedLaunchState < PreparedLaunchState.Unreserved || _preparedLaunchState > PreparedLaunchState.Failed) throw new InvalidOperationException("Prepared launch state is unknown.");
            if (_preparedLaunchFaultStage < PreparedLaunchFaultStage.None || _preparedLaunchFaultStage > PreparedLaunchFaultStage.Closed) throw new InvalidOperationException("Prepared launch fault stage is unknown.");
            if (_startEntered && !_awakeEntered) throw new InvalidOperationException("Prepared launch Start precedes Awake.");
            if (_gameplayCallbacksRegistered != (_gameplayCallbacksProof != null) || _uiCallbacksRegistered != (_uiCallbacksProof != null) ||
                (_gameplayCallbacksProof != null && !ReferenceEquals(_gameplayCallbacksProof, _actions)) ||
                (_uiCallbacksProof != null && !ReferenceEquals(_uiCallbacksProof, _actions)))
                throw new InvalidOperationException("Prepared callback ownership is malformed.");
            if (_actionsClosed && (_actions != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered)) throw new InvalidOperationException("Prepared action closure is malformed.");
            if (_resetCutoverTerminal)
            {
                // A pre-entry staging/reauthentication failure has no staged
                // candidate. It is nevertheless a terminal contained row:
                // the immutable Hub owner has been quarantined by
                // LatchResetCutoverFailure and must never be treated as a
                // successful pair.
                if (_resetCutoverLifecycle == ResetCutoverLifecycleV1.Faulted && _resetNewActions == null)
                {
                    if (_gameplayCallbacksRegistered || _uiCallbacksRegistered) throw new InvalidOperationException("Faulted reset router still publishes callbacks.");
                    return;
                }
                if (!IsHubUiOnlyVariant || _preparedLaunchState != PreparedLaunchState.Initialized || !_initialized || _resetNewActions == null || !ReferenceEquals(_actions, _resetNewActions) ||
                    !ReferenceEquals(_preparedActionsProof, _resetNewActions) || !ReferenceEquals(_preparedActionsContainmentProof, _resetNewActions) || _resetCutoverLifecycle < ResetCutoverLifecycleV1.Entered || _resetCutoverLifecycle > ResetCutoverLifecycleV1.Disposed || _gameplayCallbacksRegistered || _uiCallbacksRegistered)
                    throw new InvalidOperationException("Reset terminal router state is malformed.");
                ValidateDisabledActions(_resetNewActions); return;
            }
            if (_preparedLaunchState == PreparedLaunchState.Unreserved)
            {
                if (_preparedLaunchOwner != null || _preparedLaunchOwnerProof != null || _preparedActionsProof != null || _preparedActionsContainmentProof != null || _preparedLaunchFaultStage != PreparedLaunchFaultStage.None) throw new InvalidOperationException("Unreserved launch state is malformed.");
                if (_disposed)
                {
                    if (!_actionsClosed || _actions != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered) throw new InvalidOperationException("Disposed legacy launch state is malformed.");
                    return; // Historical initialized/faulted destroy rows remain valid.
                }
                var actionFree = _actions == null && !_gameplayCallbacksRegistered && !_uiCallbacksRegistered;
                var live = _actions != null;
                var pristine = actionFree && !_faulted && !_actionsClosed && !_initialized && !_startEntered;
                var initializedLive = live && _initialized && !_faulted && !_actionsClosed && _gameplayCallbacksRegistered && _uiCallbacksRegistered;
                var faultedInitializedLive = live && _initialized && _faulted && !_actionsClosed;
                var initializationFaultLive = live && !_initialized && _faulted && !_actionsClosed && _faultStage == InputRouterFaultStage.Initialization && _gameplayCallbacksRegistered && _uiCallbacksRegistered;
                var disabledInitializationFaultLive = live && !_initialized && _faulted && !_actionsClosed && _faultStage == InputRouterFaultStage.Initialization && !_gameplayCallbacksRegistered && !_uiCallbacksRegistered;
                var closedFault = actionFree && _faulted && _actionsClosed;
                if ((pristine && (!_awakeEntered && _startEntered)) ||
                    (initializedLive && !_awakeEntered) ||
                    (faultedInitializedLive && !_awakeEntered) ||
                    (initializationFaultLive && !_awakeEntered) ||
                    (disabledInitializationFaultLive && !_awakeEntered) ||
                    !(pristine || initializedLive || faultedInitializedLive || initializationFaultLive || disabledInitializationFaultLive || closedFault))
                    throw new InvalidOperationException("Unreserved launch state is malformed.");
                return;
            }
            if (_preparedLaunchState == PreparedLaunchState.Reserved)
            { if (_preparedLaunchOwner == null || (IsHubUiOnlyVariant && !ReferenceEquals(_preparedLaunchOwner, _preparedLaunchOwnerProof)) || (!IsHubUiOnlyVariant && _preparedLaunchOwnerProof != null) || _preparedActionsProof != null || _preparedActionsContainmentProof != null || _actions != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered || _initialized || _startEntered || _disposed || _faulted || _actionsClosed || _preparedLaunchFaultStage != PreparedLaunchFaultStage.None) throw new InvalidOperationException("Reserved launch state is malformed."); return; }
            if (_preparedLaunchState == PreparedLaunchState.Adopted)
            { if (_preparedLaunchOwner == null || (IsHubUiOnlyVariant && (!ReferenceEquals(_preparedLaunchOwner, _preparedLaunchOwnerProof) || !ReferenceEquals(_actions, _preparedActionsProof) || !ReferenceEquals(_actions, _preparedActionsContainmentProof))) || (!IsHubUiOnlyVariant && (_preparedLaunchOwnerProof != null || _preparedActionsProof != null || _preparedActionsContainmentProof != null)) || _actions == null || _gameplayCallbacksRegistered || _uiCallbacksRegistered || _initialized || _disposed || _faulted || _actionsClosed || _preparedLaunchFaultStage != PreparedLaunchFaultStage.None) throw new InvalidOperationException("Adopted launch state is malformed."); ValidateDisabledActions(_actions); return; }
            if (_preparedLaunchState == PreparedLaunchState.InitializedPendingPublication || _preparedLaunchState == PreparedLaunchState.Initialized)
            {
                var postPublicationClosed = _preparedLaunchState == PreparedLaunchState.Initialized && _faulted && _actionsClosed && _actions == null && !_gameplayCallbacksRegistered && !_uiCallbacksRegistered;
                if (_preparedLaunchOwner == null || (IsHubUiOnlyVariant && (!ReferenceEquals(_preparedLaunchOwner, _preparedLaunchOwnerProof) || (_actions != null && (!ReferenceEquals(_actions, _preparedActionsProof) || !ReferenceEquals(_actions, _preparedActionsContainmentProof))) || (_actions == null && !ReferenceEquals(_preparedActionsProof, _preparedActionsContainmentProof)))) || (!IsHubUiOnlyVariant && (_preparedLaunchOwnerProof != null || _preparedActionsProof != null || _preparedActionsContainmentProof != null)) || !_awakeEntered || (_actions == null && !_disposed && !postPublicationClosed) || (_actions == null && (_gameplayCallbacksRegistered || _uiCallbacksRegistered))) throw new InvalidOperationException("Initialized launch state is malformed.");
                if (_preparedLaunchState == PreparedLaunchState.InitializedPendingPublication && (!_initialized || !_gameplayCallbacksRegistered || !_uiCallbacksRegistered || _disposed || _faulted || _actionsClosed || _preparedLaunchFaultStage != PreparedLaunchFaultStage.None || _mode != InputMode.UIOnly || !_actions.UI.Get().enabled || _actions.Gameplay.Get().enabled)) throw new InvalidOperationException("Pending publication state is malformed.");
                // Post-publication follows historical M5B5 disable/destroy rows:
                // a live initialized router has callbacks; a faulted/disabled one
                // may have removed callbacks or closed actions during destroy.
                if (_preparedLaunchState == PreparedLaunchState.Initialized && (!_initialized || (!_faulted && !_disposed && (!_gameplayCallbacksRegistered || !_uiCallbacksRegistered)))) throw new InvalidOperationException("Initialized launch state is malformed.");
                return;
            }
            if (_preparedLaunchState == PreparedLaunchState.Failed && (_preparedLaunchOwner == null || (IsHubUiOnlyVariant && (!ReferenceEquals(_preparedLaunchOwner, _preparedLaunchOwnerProof) || !ReferenceEquals(_preparedActionsProof, _preparedActionsContainmentProof))) || (!IsHubUiOnlyVariant && (_preparedLaunchOwnerProof != null || _preparedActionsProof != null || _preparedActionsContainmentProof != null)) || !_faulted || !_actionsClosed || _preparedLaunchFaultStage == PreparedLaunchFaultStage.None || _actions != null || _gameplayCallbacksRegistered || _uiCallbacksRegistered)) throw new InvalidOperationException("Failed launch state is malformed.");
        }

        private static void ValidateDisabledActions(GameInputActions actions)
        {
            if (actions.asset == null || actions.asset.enabled) throw new InvalidOperationException("Prepared actions must be disabled.");
            foreach (var map in actions.asset.actionMaps) if (map.enabled) throw new InvalidOperationException("Prepared actions must be disabled.");
        }
        private void CheckPreparedLaunchFault(PreparedLaunchFaultInjectionStage stage)
        { if (_testPreparedLaunchFaultStage.HasValue && _testPreparedLaunchFaultStage.Value == stage) throw new InvalidOperationException("Injected prepared launch callback fault."); }

        private bool Accept(InputAction.CallbackContext context, InputAction expected, InputActionMap expectedMap, bool gameplay)
        {
            return (!_localRequireNeutralStart || (!_localNeutralPending && UnityEngine.InputSystem.LowLevel.InputState.updateCount != _localEnableUpdate)) && !ExecutionSessionBlocked() && !_resetCutoverTerminal && !DesktopProfileLaunchAdapterV1.IsRouterCohortInputClosed(this) && !_captureSuppressed && !_faulted && ReferenceEquals(context.action, expected) && ReferenceEquals(context.action.actionMap, expectedMap) && expectedMap.enabled &&
                context.control != null && context.control.device != null && context.control.device.added && context.control.device.enabled &&
                (gameplay ? _mode == InputMode.GameplayEnabled : _mode == InputMode.UIOnly);
        }
        private bool Gameplay(InputAction.CallbackContext c, InputAction expected) => Accept(c, expected, _actions.Gameplay.Get(), true);
        private bool Ui(InputAction.CallbackContext c, InputAction expected) => Accept(c, expected, _actions.UI.Get(), false);
        private static int QuantizeAxis(float value)
        {
            // REQ-M5D7PA-002: finite Navigate clamps before scaling; a very
            // large finite stick value is therefore a valid endpoint, not an
            // intermediate integer-overflow fault.
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new InvalidOperationException("Input is non-finite.");
            return RoundToIntAway(Mathf.Clamp(value, -1f, 1f) * 4096f);
        }
        private static int RoundToIntAway(float value) { if (float.IsNaN(value) || float.IsInfinity(value)) throw new InvalidOperationException("Input is non-finite."); return checked((int)Math.Round(value, MidpointRounding.AwayFromZero)); }
        private static long RoundToLongAway(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new InvalidOperationException("Input is non-finite.");
            return checked((long)Math.Round((double)value * 4096d, MidpointRounding.AwayFromZero));
        }
        private static (int x, int y) NormalizeStick(Vector2 value)
        {
            var x = (double)value.x; var y = (double)value.y; if (double.IsNaN(x) || double.IsNaN(y) || double.IsInfinity(x) || double.IsInfinity(y) || x * x + y * y < .04d) return (0, 0);
            var length = Math.Sqrt(x * x + y * y); return (Mathf.Clamp(checked((int)Math.Round(x / length * 4096d, MidpointRounding.AwayFromZero)), -4096, 4096), Mathf.Clamp(checked((int)Math.Round(y / length * 4096d, MidpointRounding.AwayFromZero)), -4096, 4096));
        }
        private void MarkGameplayButton(InputAction.CallbackContext c, InputAction expected, ref bool target) { if (Gameplay(c, expected) && c.performed) target = true; }
        public void OnMove(InputAction.CallbackContext c) { if (Gameplay(c, _actions.Gameplay.Move)) _move = c.canceled ? Vector2.zero : c.ReadValue<Vector2>(); }
        public void OnAim(InputAction.CallbackContext c) { if (!Gameplay(c, _actions.Gameplay.Aim) || c.canceled) return; var control = c.control as StickControl; if (control == null) return; var raw = control.ReadUnprocessedValue(); var x=(double)raw.x; var y=(double)raw.y; if (double.IsNaN(x)||double.IsNaN(y)||double.IsInfinity(x)||double.IsInfinity(y)||x*x+y*y<.04d) return; var ordinal = checked(_callbackOrdinal + 1); _stick = raw; _callbackOrdinal = ordinal; _stickOrdinal = ordinal; if (!_activeAimSource.HasValue || _stickOrdinal >= _pointOrdinal) _activeAimSource = AimSource.GamepadStick; }
        public void OnPoint(InputAction.CallbackContext c) { if (Gameplay(c, _actions.Gameplay.Point) && !c.canceled) { var value = c.ReadValue<Vector2>(); var x = RoundToIntAway(value.x); var y = RoundToIntAway(value.y); var ordinal = checked(_callbackOrdinal + 1); _pointX = x; _pointY = y; _callbackOrdinal = ordinal; _pointOrdinal = ordinal; if (_pointOrdinal >= _stickOrdinal) _activeAimSource = AimSource.MousePointer; } }
        public void OnJump(InputAction.CallbackContext c) { if (!Gameplay(c, _actions.Gameplay.Jump)) return; if (c.performed) _jumpPressed = true; if (c.canceled) _jumpReleased = true; }
        public void OnDash(InputAction.CallbackContext c) => MarkGameplayButton(c, _actions.Gameplay.Dash, ref _dashPressed);
        public void OnInteract(InputAction.CallbackContext c) { if (Gameplay(c, _actions.Gameplay.Interact) && c.performed) _unboundGameplay = true; }
        public void OnChoiceSkill(InputAction.CallbackContext c)
        {if(Gameplay(c,_actions.Gameplay.ChoiceSkill)&&c.performed){if(_chapterChoiceBound){if(!_choicePressed){_choiceEdgeOrdinal=checked(_choiceEdgeOrdinal+1);_choicePressed=true;}}else _unboundGameplay=true;}}
        public void OnAttack(InputAction.CallbackContext c) { if (Gameplay(c, _actions.Gameplay.Attack) && c.performed) { _attackPressed = true; _attackFromMouse = c.control.device is Mouse; } }
        public void OnTransfer(InputAction.CallbackContext c) { if (Gameplay(c, _actions.Gameplay.Transfer) && c.performed) { _transferPressed = true; _transferFromMouse = c.control.device is Mouse; } }
        public void OnPause(InputAction.CallbackContext c) { if (Gameplay(c, _actions.Gameplay.Pause) && c.performed) _unboundGameplay = true; }
        // REQ-M5D7PA-001/002. The generated wrapper still calls only this
        // router.  Each callback stages a local checked change before writing
        // the pending semantic batch; no callback touches _unboundUi.
        public void OnNavigate(InputAction.CallbackContext c)
        {
            if (!Ui(c, _actions.UI.Navigate) || (!c.performed && !c.canceled)) return;
            try
            {
                var value = c.canceled ? Vector2.zero : c.ReadValue<Vector2>();
                var x = QuantizeAxis(value.x); var y = QuantizeAxis(value.y);
                var ordinal = checked(_callbackOrdinal + 1);
                var changed = x != _uiNavigateXQ4096 || y != _uiNavigateYQ4096;
                _uiNavigateXQ4096 = x; _uiNavigateYQ4096 = y; _uiNavigateChanged |= changed; _callbackOrdinal = ordinal;
            }
            catch { LatchFault(InputRouterFaultStage.UiCapture); }
        }

        void GameInputActions.IUIActions.OnPoint(InputAction.CallbackContext c)
        {
            if (!Ui(c, _actions.UI.Point) || c.canceled || !c.performed) return;
            try
            {
                var value = c.ReadValue<Vector2>(); var x = RoundToIntAway(value.x); var y = RoundToIntAway(value.y);
                var ordinal = checked(_callbackOrdinal + 1);
                var changed = !_uiHasPoint || x != _uiPointXActualPx || y != _uiPointYActualPx;
                _uiPointXActualPx = x; _uiPointYActualPx = y; _uiHasPoint = true; _uiPointChanged |= changed; _callbackOrdinal = ordinal;
            }
            catch { LatchFault(InputRouterFaultStage.UiCapture); }
        }

        public void OnClick(InputAction.CallbackContext c)
        {
            // A foreign/disabled callback is ignored.  A live callback for the
            // exact action/map has a production binding obligation instead.
            if (!Ui(c, _actions.UI.Click)) return;
            try
            {
                var mouse = c.control == null ? null : c.control.device as Mouse;
                if (mouse == null || !ReferenceEquals(c.control, mouse.leftButton)) throw new InvalidOperationException("UI.Click must be Mouse.leftButton.");
                if (!c.performed || _uiClickPressed) return;
                var value = mouse.position.ReadValue(); var x = RoundToIntAway(value.x); var y = RoundToIntAway(value.y);
                var ordinal = checked(_callbackOrdinal + 1);
                var changed = !_uiHasPoint || x != _uiPointXActualPx || y != _uiPointYActualPx;
                _uiPointXActualPx = x; _uiPointYActualPx = y; _uiHasPoint = true; _uiPointChanged |= changed;
                _uiClickPointXActualPx = x; _uiClickPointYActualPx = y; _uiClickPressed = _uiHasClickPoint = true; _callbackOrdinal = ordinal;
                if(_chapterHubOwner!=null)_chapterHubClickUpdate=UnityEngine.InputSystem.LowLevel.InputState.updateCount;
            }
            catch { LatchFault(InputRouterFaultStage.UiCapture); }
        }

        public void OnScrollWheel(InputAction.CallbackContext c)
        {
            if (!Ui(c, _actions.UI.ScrollWheel) || !c.performed) return;
            try
            {
                var value = c.ReadValue<Vector2>(); var x = RoundToLongAway(value.x); var y = RoundToLongAway(value.y);
                var nextX = checked(_uiScrollXQ4096 + x); var nextY = checked(_uiScrollYQ4096 + y); var ordinal = checked(_callbackOrdinal + 1);
                _uiScrollXQ4096 = nextX; _uiScrollYQ4096 = nextY; _callbackOrdinal = ordinal;
            }
            catch { LatchFault(InputRouterFaultStage.UiCapture); }
        }

        public void OnSubmit(InputAction.CallbackContext c) { MarkUiEdge(c, _actions.UI.Submit, ref _uiSubmitPressed); }
        public void OnCancel(InputAction.CallbackContext c) { MarkUiEdge(c, _actions.UI.Cancel, ref _uiCancelPressed); }
        private void MarkUiEdge(InputAction.CallbackContext c, InputAction expected, ref bool target)
        {
            if (!Ui(c, expected) || !c.performed || target) return;
            try { var ordinal = checked(_callbackOrdinal + 1); target = true; _callbackOrdinal = ordinal;
                if(_chapterHubOwner!=null && ReferenceEquals(expected,_actions.UI.Submit))_chapterHubSubmitUpdate=UnityEngine.InputSystem.LowLevel.InputState.updateCount; }
            catch { LatchFault(InputRouterFaultStage.UiCapture); }
        }

        private readonly struct PendingModeRequest : IEquatable<PendingModeRequest>
        {
            internal PendingModeRequest(InputMode mode, string reason, SimulationTick tick) { Mode = mode; Reason = reason; Tick = tick; }
            internal InputMode Mode { get; } private string Reason { get; } private SimulationTick Tick { get; }
            public bool Equals(PendingModeRequest other) => Mode == other.Mode && Tick == other.Tick && string.Equals(Reason, other.Reason, StringComparison.Ordinal);
        }
        internal enum PreparedLaunchFaultStage { None = 0, CallbackRegistration = 1, Initialization = 2, Publication = 3, Closed = 4 }
        internal enum PreparedLaunchFaultInjectionStage { AfterGameplayCallbackRegistration = 1, BeforeUiCallbackRegistration = 2, AfterUiCallbackRegistration = 3 }
        private enum PreparedLaunchState { Unreserved = 0, Reserved = 1, Adopted = 2, InitializedPendingPublication = 3, Initialized = 4, Failed = 5 }
        private enum ResetCutoverLifecycleV1 { None = 0, Entered = 1, Transferred = 2, Completed = 3, Faulted = 4, Disposed = 5 }
        private enum RecoveryReservationStateV1 { None = 0, Reserved = 1, RepairBlocked = 2, Terminal = 3, Completed = 4, Closed = 5 }
    }
}
