using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Transfer
{
    // REQ-WT-001~008, REQ-MOV-004. Pure owner of one reversible transfer session.
    internal sealed class TransferSession
    {
        internal const string PlayerLightweightModifierId = "Player.Lightweight.v1";
        private readonly Dictionary<string, Registration> _registrations = new Dictionary<string, Registration>(StringComparer.Ordinal);
        private string _highlighted = string.Empty, _active = string.Empty; private TransferPlayerState _state; private int _lockEnd = -1, _revision; private TransferFailureReason _failure; private AimSample? _sample; private TransferSelectionSnapshot _selection;
        internal TransferSessionSnapshot Snapshot(SimulationTick tick) => new TransferSessionSnapshot(tick,_highlighted,_active,_state,_lockEnd,_failure,_revision);
        // REQ-WT-003, REQ-WT-005; AC-WT-003, AC-WT-005 (partial). Read-only exact
        // registration proof for the authored fresh-instance gate; no registry escapes.
        internal bool HasExactEncounterRegistrations(string firstTargetId, string secondTargetId)
        {
            return _registrations.Count == 2 && _registrations.ContainsKey(firstTargetId) &&
                _registrations.ContainsKey(secondTargetId);
        }
        // REQ-WT-003, REQ-WT-005. Read-only terminal-cleanup effect proof.
        internal bool HasNoRegistrations(params string[] targetIds)
        {
            if (targetIds == null) throw new ArgumentNullException(nameof(targetIds));
            for (var index = 0; index < targetIds.Length; index++) if (_registrations.ContainsKey(targetIds[index])) return false;
            return true;
        }
        internal void Register(TransferTargetDescriptor descriptor,ITransferTargetModifierSink sink) { if(sink==null||!string.Equals(descriptor.TargetId,sink.BoundTargetId,StringComparison.Ordinal)||_registrations.ContainsKey(descriptor.TargetId))throw new ArgumentException();_registrations.Add(descriptor.TargetId,new Registration(descriptor,sink)); }
        internal TransferSelectionSnapshot Evaluate(SimulationTick tick,AimSample sample,IReadOnlyList<TransferCandidateObservation> observations) { var selection=TransferSelector.Select(tick,sample,observations,_highlighted);_sample=sample;_selection=selection;_highlighted=selection.SelectedTargetId;_failure=selection.FailureReason;return selection; }
        internal TransferAttemptResult Press(TransferPressed press)
        {
            var tick=press.Tick;
            if (!string.IsNullOrEmpty(_active)) return IsLocked(tick) ? Fail(tick,press.AimSampleId,_active,TransferFailureReason.Cooldown) : ClearActive(tick,TransferClearReason.ManualRecall,press.AimSampleId,true);
            if(IsLocked(tick))return Fail(tick,press.AimSampleId,_selection.SelectedTargetId,TransferFailureReason.Cooldown);
            if(!press.AimSampleId.HasValue)return Fail(tick,null,string.Empty,TransferFailureReason.InvalidTarget);
            if(!_sample.HasValue||_sample.Value.SampleTick!=tick||_sample.Value.SampleId!=press.AimSampleId.Value)return Fail(tick,press.AimSampleId,string.Empty,TransferFailureReason.StaleAimSample);
            if(!_selection.IsReady||_selection.Tick!=tick)return Fail(tick,press.AimSampleId,_selection.SelectedTargetId,_selection.FailureReason==TransferFailureReason.None?TransferFailureReason.InvalidTarget:_selection.FailureReason);
            if(!_registrations.TryGetValue(_selection.SelectedTargetId,out var registration)||!registration.Sink.IsStillAvailable())return Fail(tick,press.AimSampleId,_selection.SelectedTargetId,TransferFailureReason.TargetUnavailable);
            // All checked work and immutable event construction precede the externally mutating sink call.
            var targetModifier=TargetModifier(registration.Descriptor.Kind);var next=checked(_revision+1);var nextLockEnd=checked(tick.Value+21);var request=new TransferModifierRequest(registration.Descriptor.TargetId,registration.Descriptor.Kind,registration.Descriptor.BaseModifierProfileId,targetModifier,tick,next);var changed=new TransferStateChanged(registration.Descriptor.TargetId,PlayerLightweightModifierId,targetModifier,tick,next);
            if(!registration.Sink.TryApply(request))return Fail(tick,press.AimSampleId,_selection.SelectedTargetId,TransferFailureReason.ModifierRejected);
            _active=registration.Descriptor.TargetId;_state=TransferPlayerState.Lightweight;_revision=next;_lockEnd=nextLockEnd;_failure=TransferFailureReason.None;
            return new TransferAttemptResult(tick,press.AimSampleId,_active,true,TransferFailureReason.None,Snapshot(tick),changed,null);
        }
        internal TransferTickOutcome Process(SimulationTick tick,TransferClearReason? lifecycle,IReadOnlyList<TransferTargetRemoved> removals,AimSample? sample,IReadOnlyList<TransferCandidateObservation> observations,TransferPressed? press) => Process(tick,lifecycle,Array.Empty<TransferTargetExposureEnded>(),removals,sample,observations,press);
        // REQ-WT-001, REQ-WT-005. Temporary scripted withdrawal is deliberately
        // distinct from TransferTargetRemoved: the former clears an active
        // relation but retains the frozen registration for cyclic reuse.
        internal TransferTickOutcome Process(SimulationTick tick,TransferClearReason? lifecycle,IReadOnlyList<TransferTargetExposureEnded> exposureEnds,IReadOnlyList<TransferTargetRemoved> removals,AimSample? sample,IReadOnlyList<TransferCandidateObservation> observations,TransferPressed? press)
        {
            if(exposureEnds==null||removals==null)throw new ArgumentNullException();
            if(lifecycle.HasValue){if(lifecycle.Value!=TransferClearReason.RoomLeaving&&lifecycle.Value!=TransferClearReason.RunFailed&&lifecycle.Value!=TransferClearReason.Cutscene&&lifecycle.Value!=TransferClearReason.DemoCompleted)throw new ArgumentException();var clear=ClearForLifecycle(tick,lifecycle.Value);return new TransferTickOutcome(clear,null,Snapshot(tick));}
            TransferAttemptResult? removalResult=null;
            for(var i=0;i<exposureEnds.Count;i++)if(exposureEnds[i].Tick==tick){var ended=exposureEnds[i].TargetId;if(ended==_active){var activeResult=ClearActive(tick,TransferClearReason.TargetRemoved,null,false);if(!removalResult.HasValue)removalResult=activeResult;}if(ended==_highlighted)_highlighted=string.Empty;}
            for(var i=0;i<removals.Count;i++)if(removals[i].Tick==tick) {var removed=removals[i].TargetId;if(removed==_active){var activeResult=ClearActive(tick,TransferClearReason.TargetRemoved,null,false);if(!removalResult.HasValue)removalResult=activeResult;}if(removed==_highlighted)_highlighted=string.Empty;_registrations.Remove(removed);}
            if(sample.HasValue)Evaluate(tick,sample.Value,RegisteredObservations(observations));var pressResult=press.HasValue?Press(press.Value):(TransferAttemptResult?)null;return new TransferTickOutcome(removalResult,pressResult,Snapshot(tick));
        }
        private TransferAttemptResult? ClearForLifecycle(SimulationTick tick,TransferClearReason reason){if(string.IsNullOrEmpty(_active)){_registrations.Clear();_highlighted=string.Empty;_sample=null;_selection=default(TransferSelectionSnapshot);_lockEnd=-1;_state=TransferPlayerState.Baseline;_failure=TransferFailureReason.None;return null;}var previous=_active;var next=checked(_revision+1);var cleared=new TransferCleared(reason,previous,tick,next);var registration=_registrations.ContainsKey(previous)?_registrations[previous]:default(Registration);if(registration.Sink!=null)registration.Sink.Clear(reason,tick,next);_registrations.Clear();_highlighted=string.Empty;_sample=null;_selection=default(TransferSelectionSnapshot);_active=string.Empty;_state=TransferPlayerState.Baseline;_lockEnd=-1;_revision=next;_failure=TransferFailureReason.None;var result=new TransferAttemptResult(tick,null,string.Empty,true,TransferFailureReason.None,Snapshot(tick),null,cleared);return result;}
        private TransferAttemptResult ClearActive(SimulationTick tick,TransferClearReason reason,long? aimId,bool startLock){var previous=_active;var next=checked(_revision+1);var nextLockEnd=startLock?checked(tick.Value+21):_lockEnd;var cleared=new TransferCleared(reason,previous,tick,next);var registration=_registrations.ContainsKey(previous)?_registrations[previous]:default(Registration);if(registration.Sink!=null)registration.Sink.Clear(reason,tick,next);_active=string.Empty;_highlighted=string.Empty;_state=TransferPlayerState.Baseline;_revision=next;_lockEnd=nextLockEnd;_failure=TransferFailureReason.None;return new TransferAttemptResult(tick,aimId,string.Empty,true,TransferFailureReason.None,Snapshot(tick),null,cleared);}
        private TransferAttemptResult Fail(SimulationTick tick,long? aimId,string selected,TransferFailureReason reason){_failure=reason;return new TransferAttemptResult(tick,aimId,selected,false,reason,Snapshot(tick),null,null);} private bool IsLocked(SimulationTick tick)=>_lockEnd>=0&&tick.Value<_lockEnd;
        private IReadOnlyList<TransferCandidateObservation> RegisteredObservations(IReadOnlyList<TransferCandidateObservation> observations){var filtered=new List<TransferCandidateObservation>();for(var i=0;i<observations.Count;i++)if(_registrations.ContainsKey(observations[i].Descriptor.TargetId))filtered.Add(observations[i]);return filtered;}
        private static string TargetModifier(TransferTargetKind kind){switch(kind){case TransferTargetKind.Box:return "Target.Box.Heavy.v1";case TransferTargetKind.Enemy:return "Target.Enemy.Heavy.v1";case TransferTargetKind.BossPayload:return "Target.BossPayload.ScriptedHeavy.v1";default:throw new ArgumentOutOfRangeException();}}
        private readonly struct Registration { internal Registration(TransferTargetDescriptor descriptor,ITransferTargetModifierSink sink){Descriptor=descriptor;Sink=sink;} internal TransferTargetDescriptor Descriptor{get;}internal ITransferTargetModifierSink Sink{get;} }
    }
    internal readonly struct TransferTickOutcome { internal TransferTickOutcome(TransferAttemptResult? removalResult,TransferAttemptResult? pressResult,TransferSessionSnapshot snapshot){RemovalResult=removalResult;PressResult=pressResult;Snapshot=snapshot;}internal TransferAttemptResult? RemovalResult{get;}internal TransferAttemptResult? PressResult{get;}internal TransferSessionSnapshot Snapshot{get;} }
}
