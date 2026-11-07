using System;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Transfer
{
    public enum TransferTargetKind { Box = 0, Enemy = 1, BossPayload = 2 }
    public enum TransferSelectionSource { None = 0, MousePointer = 1, GamepadStick = 2 }
    public enum TransferFailureReason { None = 0, InvalidTarget, OutOfRange, LineOfSightBlocked, TargetUnavailable, TriggerTarget, Cooldown, StaleAimSample, ModifierRejected }
    public enum TransferPlayerState { Baseline = 0, Lightweight = 1 }
    public enum TransferClearReason { ManualRecall, TargetRemoved, RoomLeaving, RunFailed, Cutscene, DemoCompleted }

    public readonly struct TransferTargetDescriptor
    {
        public TransferTargetDescriptor(string targetId, TransferTargetKind kind, int aimPointXQ1000, int aimPointYQ1000, int aimShapeCenterXQ1000, int aimShapeCenterYQ1000, int aimShapeHalfExtentXQ1000, int aimShapeHalfExtentYQ1000, string baseModifierProfileId, bool isAvailable)
        {
            TransferValidation.TargetId(targetId); if (!ValidKind(kind) || aimShapeHalfExtentXQ1000 <= 0 || aimShapeHalfExtentYQ1000 <= 0 || string.IsNullOrEmpty(baseModifierProfileId)) throw new ArgumentException();
            TargetId = targetId; Kind = kind; AimPointXQ1000 = aimPointXQ1000; AimPointYQ1000 = aimPointYQ1000; AimShapeCenterXQ1000 = aimShapeCenterXQ1000; AimShapeCenterYQ1000 = aimShapeCenterYQ1000; AimShapeHalfExtentXQ1000 = aimShapeHalfExtentXQ1000; AimShapeHalfExtentYQ1000 = aimShapeHalfExtentYQ1000; BaseModifierProfileId = baseModifierProfileId; IsAvailable = isAvailable;
        }
        public string TargetId { get; } public TransferTargetKind Kind { get; } public int AimPointXQ1000 { get; } public int AimPointYQ1000 { get; } public int AimShapeCenterXQ1000 { get; } public int AimShapeCenterYQ1000 { get; } public int AimShapeHalfExtentXQ1000 { get; } public int AimShapeHalfExtentYQ1000 { get; } public string BaseModifierProfileId { get; } public bool IsAvailable { get; }
        internal static bool ValidKind(TransferTargetKind value) => value == TransferTargetKind.Box || value == TransferTargetKind.Enemy || value == TransferTargetKind.BossPayload;
    }

    public readonly struct TransferCandidateObservation
    {
        public TransferCandidateObservation(TransferTargetDescriptor descriptor, SimulationTick observationTick, int worldAimPointXQ1000, int worldAimPointYQ1000, int worldAimShapeCenterXQ1000, int worldAimShapeCenterYQ1000, int worldAimShapeHalfExtentXQ1000, int worldAimShapeHalfExtentYQ1000, bool isTrigger, bool isLineOfSightOpen, int playerDistanceKey, bool mouseInsideShape, int screenDistanceSquaredKey, int gamepadAngleKey)
        {
            TransferValidation.TargetId(descriptor.TargetId); if (worldAimShapeHalfExtentXQ1000 <= 0 || worldAimShapeHalfExtentYQ1000 <= 0 || playerDistanceKey < 0 || screenDistanceSquaredKey < 0 || gamepadAngleKey < 0) throw new ArgumentOutOfRangeException();
            Descriptor = descriptor; ObservationTick = observationTick; WorldAimPointXQ1000 = worldAimPointXQ1000; WorldAimPointYQ1000 = worldAimPointYQ1000; WorldAimShapeCenterXQ1000 = worldAimShapeCenterXQ1000; WorldAimShapeCenterYQ1000 = worldAimShapeCenterYQ1000; WorldAimShapeHalfExtentXQ1000 = worldAimShapeHalfExtentXQ1000; WorldAimShapeHalfExtentYQ1000 = worldAimShapeHalfExtentYQ1000; IsTrigger = isTrigger; IsLineOfSightOpen = isLineOfSightOpen; PlayerDistanceKey = playerDistanceKey; MouseInsideShape = mouseInsideShape; ScreenDistanceSquaredKey = screenDistanceSquaredKey; GamepadAngleKey = gamepadAngleKey;
        }
        public TransferTargetDescriptor Descriptor { get; } public SimulationTick ObservationTick { get; } public int WorldAimPointXQ1000 { get; } public int WorldAimPointYQ1000 { get; } public int WorldAimShapeCenterXQ1000 { get; } public int WorldAimShapeCenterYQ1000 { get; } public int WorldAimShapeHalfExtentXQ1000 { get; } public int WorldAimShapeHalfExtentYQ1000 { get; } public bool IsTrigger { get; } public bool IsLineOfSightOpen { get; } public int PlayerDistanceKey { get; } public bool MouseInsideShape { get; } public int ScreenDistanceSquaredKey { get; } public int GamepadAngleKey { get; }
    }

    public readonly struct TransferSelectionSnapshot
    {
        public TransferSelectionSnapshot(SimulationTick tick, TransferSelectionSource source, string selectedTargetId, int insideRank, int screenDistanceSquaredKey, int playerDistanceKey, int gamepadAngleKey, TransferFailureReason failureReason)
        {
            if (source != TransferSelectionSource.None && source != TransferSelectionSource.MousePointer && source != TransferSelectionSource.GamepadStick) throw new ArgumentOutOfRangeException(nameof(source)); TransferValidation.Failure(failureReason); selectedTargetId = selectedTargetId ?? string.Empty;
            if (selectedTargetId.Length != 0) TransferValidation.TargetId(selectedTargetId); if (insideRank < -1 || insideRank > 1 || screenDistanceSquaredKey < -1 || playerDistanceKey < -1 || gamepadAngleKey < -1) throw new ArgumentOutOfRangeException();
            var empty = selectedTargetId.Length == 0;
            if ((source == TransferSelectionSource.None && !empty) || (empty && (insideRank != -1 || screenDistanceSquaredKey != -1 || playerDistanceKey != -1 || gamepadAngleKey != -1))) throw new ArgumentException();
            if (!empty && source == TransferSelectionSource.MousePointer && (insideRank < 0 || screenDistanceSquaredKey < 0 || playerDistanceKey < 0 || gamepadAngleKey != -1)) throw new ArgumentException();
            if (!empty && source == TransferSelectionSource.GamepadStick && (insideRank != -1 || screenDistanceSquaredKey != -1 || playerDistanceKey < 0 || gamepadAngleKey < 0)) throw new ArgumentException();
            Tick = tick; Source = source; SelectedTargetId = selectedTargetId; InsideRank = insideRank; ScreenDistanceSquaredKey = screenDistanceSquaredKey; PlayerDistanceKey = playerDistanceKey; GamepadAngleKey = gamepadAngleKey; FailureReason = failureReason;
        }
        public SimulationTick Tick { get; } public TransferSelectionSource Source { get; } public string SelectedTargetId { get; } public int InsideRank { get; } public int ScreenDistanceSquaredKey { get; } public int PlayerDistanceKey { get; } public int GamepadAngleKey { get; } public TransferFailureReason FailureReason { get; }
        internal bool IsReady => !string.IsNullOrEmpty(SelectedTargetId) && FailureReason == TransferFailureReason.None;
    }

    public readonly struct TransferSessionSnapshot { public TransferSessionSnapshot(SimulationTick tick,string highlightedTargetId,string activeTargetId,TransferPlayerState playerState,int transitionLockEndTick,TransferFailureReason lastFailureReason,int revision) { if (playerState != TransferPlayerState.Baseline && playerState != TransferPlayerState.Lightweight || transitionLockEndTick < -1 || revision < 0) throw new ArgumentOutOfRangeException(); TransferValidation.Failure(lastFailureReason); HighlightedTargetId=highlightedTargetId??string.Empty; ActiveTargetId=activeTargetId??string.Empty;if(HighlightedTargetId.Length>0)TransferValidation.TargetId(HighlightedTargetId);if(ActiveTargetId.Length>0)TransferValidation.TargetId(ActiveTargetId);if((playerState==TransferPlayerState.Baseline)!=(ActiveTargetId.Length==0)||(playerState==TransferPlayerState.Lightweight&&revision<=0))throw new ArgumentException(); Tick=tick; PlayerState=playerState; TransitionLockEndTick=transitionLockEndTick; LastFailureReason=lastFailureReason; Revision=revision; } public SimulationTick Tick{get;} public string HighlightedTargetId{get;} public string ActiveTargetId{get;} public TransferPlayerState PlayerState{get;} public int TransitionLockEndTick{get;} public TransferFailureReason LastFailureReason{get;} public int Revision{get;} }
    public readonly struct TransferStateChanged { public TransferStateChanged(string targetId,string playerModifierId,string targetModifierId,SimulationTick tick,int revision) { TransferValidation.TargetId(targetId); if(playerModifierId!="Player.Lightweight.v1"||!TransferValidation.TargetModifier(targetModifierId)||revision<=0)throw new ArgumentException(); TargetId=targetId;PlayerModifierId=playerModifierId;TargetModifierId=targetModifierId;Tick=tick;Revision=revision;} public string TargetId{get;}public string PlayerModifierId{get;}public string TargetModifierId{get;}public SimulationTick Tick{get;}public int Revision{get;} }
    public readonly struct TransferCleared { public TransferCleared(TransferClearReason reason,string previousTargetId,SimulationTick tick,int revision) { TransferValidation.TargetId(previousTargetId);if(revision<=0||!TransferValidation.Clear(reason))throw new ArgumentOutOfRangeException();Reason=reason;PreviousTargetId=previousTargetId;Tick=tick;Revision=revision;} public TransferClearReason Reason{get;}public string PreviousTargetId{get;}public SimulationTick Tick{get;}public int Revision{get;} }
    public readonly struct TransferAttemptResult { public TransferAttemptResult(SimulationTick tick,long? aimSampleId,string selectedTargetId,bool success,TransferFailureReason failureReason,TransferSessionSnapshot session,TransferStateChanged? stateChanged,TransferCleared? cleared) { if(aimSampleId.HasValue&&aimSampleId.Value<0)throw new ArgumentOutOfRangeException(); TransferValidation.Failure(failureReason); selectedTargetId=selectedTargetId??string.Empty; if(selectedTargetId.Length>0)TransferValidation.TargetId(selectedTargetId); if(tick!=session.Tick||success != (stateChanged.HasValue ^ cleared.HasValue) || (!success && (stateChanged.HasValue||cleared.HasValue||failureReason==TransferFailureReason.None)) || (success && failureReason!=TransferFailureReason.None))throw new ArgumentException();if(stateChanged.HasValue&&(stateChanged.Value.Tick!=tick||stateChanged.Value.Revision!=session.Revision||session.PlayerState!=TransferPlayerState.Lightweight||session.ActiveTargetId!=stateChanged.Value.TargetId))throw new ArgumentException();if(cleared.HasValue&&(cleared.Value.Tick!=tick||cleared.Value.Revision!=session.Revision||session.PlayerState!=TransferPlayerState.Baseline||session.ActiveTargetId.Length!=0))throw new ArgumentException(); Tick=tick;AimSampleId=aimSampleId;SelectedTargetId=selectedTargetId;Success=success;FailureReason=failureReason;Session=session;StateChanged=stateChanged;Cleared=cleared;} public SimulationTick Tick{get;}public long? AimSampleId{get;}public string SelectedTargetId{get;}public bool Success{get;}public TransferFailureReason FailureReason{get;}public TransferSessionSnapshot Session{get;}public TransferStateChanged? StateChanged{get;}public TransferCleared? Cleared{get;} }
    public readonly struct TransferModifierRequest { public TransferModifierRequest(string targetId,TransferTargetKind kind,string baseModifierProfileId,string targetModifierId,SimulationTick tick,int proposedRevision) { TransferValidation.TargetId(targetId);if(!TransferTargetDescriptor.ValidKind(kind)||string.IsNullOrEmpty(baseModifierProfileId)||targetModifierId!=TransferValidation.TargetModifier(kind)||proposedRevision<=0)throw new ArgumentException();TargetId=targetId;Kind=kind;BaseModifierProfileId=baseModifierProfileId;TargetModifierId=targetModifierId;Tick=tick;ProposedRevision=proposedRevision;}public string TargetId{get;}public TransferTargetKind Kind{get;}public string BaseModifierProfileId{get;}public string TargetModifierId{get;}public SimulationTick Tick{get;}public int ProposedRevision{get;} }
    public interface ITransferTargetModifierSink { string BoundTargetId { get; } bool IsStillAvailable(); bool TryApply(TransferModifierRequest request); void Clear(TransferClearReason reason, SimulationTick tick, int revision); }
    public readonly struct TransferTargetRemoved { public TransferTargetRemoved(string targetId,SimulationTick tick){TransferValidation.TargetId(targetId);TargetId=targetId;Tick=tick;}public string TargetId{get;}public SimulationTick Tick{get;} }
    // REQ-WT-001, REQ-WT-005. A scripted target may leave its temporary
    // targetable interval without being permanently removed from the frozen
    // Transfer registration.
    internal readonly struct TransferTargetExposureEnded
    {
        internal TransferTargetExposureEnded(string targetId, SimulationTick tick)
        {
            TransferValidation.TargetId(targetId); TargetId = targetId; Tick = tick;
        }
        internal string TargetId { get; }
        internal SimulationTick Tick { get; }
    }
    internal static class TransferValidation { internal static void TargetId(string value) { if(value==null||value.Length<1||value.Length>96)throw new ArgumentException(nameof(value)); foreach(var c in value) if(c>127 || !((c>='A'&&c<='Z')||(c>='a'&&c<='z')||(c>='0'&&c<='9')||c=='.'||c=='_'||c==':'||c=='/'||c=='-'))throw new ArgumentException(nameof(value)); } internal static void Failure(TransferFailureReason value){if(value<TransferFailureReason.None||value>TransferFailureReason.ModifierRejected)throw new ArgumentOutOfRangeException();}internal static bool Clear(TransferClearReason value)=>value>=TransferClearReason.ManualRecall&&value<=TransferClearReason.DemoCompleted;internal static string TargetModifier(TransferTargetKind kind){switch(kind){case TransferTargetKind.Box:return "Target.Box.Heavy.v1";case TransferTargetKind.Enemy:return "Target.Enemy.Heavy.v1";case TransferTargetKind.BossPayload:return "Target.BossPayload.ScriptedHeavy.v1";default:throw new ArgumentOutOfRangeException();}}internal static bool TargetModifier(string value)=>value=="Target.Box.Heavy.v1"||value=="Target.Enemy.Heavy.v1"||value=="Target.BossPayload.ScriptedHeavy.v1"; }
}
