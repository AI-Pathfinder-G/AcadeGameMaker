using System;
using System.Collections.Generic;
using System.Globalization;
using AcadeGameMaker.Core;
using UnityEngine;

namespace AcadeGameMaker.Movement
{
    // REQ-MOV-001~004, REQ-MOV-006~010. Unity is a kinematic cast adapter; the motor is the sole motion owner.
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public sealed class PlayerMovementController : MonoBehaviour
    {
        private const int SkinQ4096 = 82;
        private const int ProbeQ4096 = 205;
        private const int NormalQ4096 = 2867;
        private const float UnitsPerWorldUnit = 4096f;
        private const float BaselineCompatibilityGravityScale = 3.1315f;

        [SerializeField] private Rigidbody2D _body;
        [SerializeField] private Collider2D _collider;
        private readonly RaycastHit2D[] _hits = new RaycastHit2D[32];
        private SortedDictionary<int, MovementCommand> _commands = new SortedDictionary<int, MovementCommand>();
        private PlayerMovementMotor _motor;
        private SimulationTick? _chapterFirstTick;
        private Vector2 _chapterPosition;
        internal SimulationTick? Chapter1ConfiguredFirstTick => _chapterFirstTick;
        // REQ-C1B-ENTRY-001: inactive storage; never creates a motor.
        internal void ConfigureChapter1EntryForAuthoring(SimulationTick firstTick,Vector2 position)
        {
            if(_chapterFirstTick.HasValue || _motor!=null || gameObject.activeInHierarchy || firstTick.Value<0 || firstTick.Value>int.MaxValue-600 ||
                float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsInfinity(position.x) || float.IsInfinity(position.y))
                throw new InvalidOperationException("Movement entry must be pristine, inactive, and finite.");
            _chapterFirstTick=firstTick;_chapterPosition=position;
        }

        private PlayerMovementModifierKind _modifier = PlayerMovementModifierKind.Baseline;
        private bool _inputLocked;
        private object _inputGateOwner;
        private readonly object _inputFrameIssuer = new object();
        private long _inputGeneration;
        private IInputFrameCommitSource _coordinatedInputSource;
        private InputFrameCommitReceipt? _pendingCoordinatedReceipt;
        private long _lastConsumedCoordinatedOrdinal = -1;
        private long _lastCoordinatedModeEpoch;
        private bool _hasCoordinatedModeEpoch;
        private object _externalMovementDirectiveOwner;
        private QueuedExternalMovementDirective? _dueExternalMovementDirective;
        private QueuedExternalMovementDirective? _futureExternalMovementDirective;
        private long _externalMovementDirectiveQueueRevision;
        private ExternalMovementDirectiveReceipt _lastExternalMovementDirectiveReceipt;
        private bool _hasExternalMovementDirectiveReceipt;

        // REQ-MOV-001, REQ-MOV-005; AC-M4B3B3-003. Combat can retain this
        // opaque value but cannot manufacture one or redirect it to another player.
        internal sealed class ExternalMovementDirectiveCandidate
        {
            private readonly PlayerMovementController _controller;
            private readonly object _owner;
            private readonly ExternalMovementDirective _directive;
            private readonly long _attackOrdinal;
            private readonly long _queueRevision;
            private readonly int _expectedTick;

            internal ExternalMovementDirectiveCandidate(
                PlayerMovementController controller,
                object owner,
                ExternalMovementDirective directive,
                long attackOrdinal,
                long queueRevision,
                int expectedTick)
            {
                _controller = controller;
                _owner = owner;
                _directive = directive;
                _attackOrdinal = attackOrdinal;
                _queueRevision = queueRevision;
                _expectedTick = expectedTick;
            }

            internal bool Matches(
                PlayerMovementController controller,
                object owner,
                long queueRevision,
                int expectedTick) =>
                ReferenceEquals(_controller, controller) &&
                ReferenceEquals(_owner, owner) &&
                _queueRevision == queueRevision &&
                _expectedTick == expectedTick;

            internal ExternalMovementDirective Directive => _directive;
            internal long AttackOrdinal => _attackOrdinal;
        }

        // REQ-UX-004, REQ-UX-006, REQ-UX-008. This opaque value contains a
        // completely staged queue; validation deliberately has no side effects.
        internal sealed class InputFrameCandidate
        {
            private readonly PlayerMovementController _consumer;
            private readonly object _issuer;
            private readonly object _owner;
            private readonly SimulationTick _tick;
            private readonly long _generation;
            private readonly long _nextGeneration;
            private readonly SortedDictionary<int, MovementCommand> _commands;
            private readonly bool _locked;
            private readonly int _droppedCount;
            private readonly bool _resetLifecycle;
            private readonly InputFrameCommitReceipt? _coordinatedReceipt;
            private bool _committed;

            internal InputFrameCandidate(PlayerMovementController consumer, object issuer, object owner, SimulationTick tick,
                long generation, long nextGeneration, SortedDictionary<int, MovementCommand> commands,
                bool locked, int droppedCount, bool resetLifecycle, InputFrameCommitReceipt? coordinatedReceipt = null)
            {
                _consumer = consumer; _issuer = issuer; _owner = owner; _tick = tick; _generation = generation;
                _nextGeneration = nextGeneration; _commands = commands; _locked = locked;
                _droppedCount = droppedCount; _resetLifecycle = resetLifecycle; _coordinatedReceipt = coordinatedReceipt;
            }

            internal bool Matches(PlayerMovementController consumer, object issuer, object owner, SimulationTick tick, long generation) =>
                !_committed && ReferenceEquals(_consumer, consumer) && ReferenceEquals(_issuer, issuer) && ReferenceEquals(_owner, owner) &&
                _tick == tick && _generation == generation;
            internal SimulationTick Tick => _tick;
            internal int ApplyToMatchingConsumer(PlayerMovementController consumer, object issuer, object owner)
            {
                if (consumer == null)
                    throw new InvalidOperationException("Movement input-frame candidate has no consumer.");
                if (!ReferenceEquals(issuer, consumer._inputFrameIssuer))
                    throw new InvalidOperationException("Movement input-frame candidate issuer is invalid.");
                consumer.ValidateInputFrameCandidate(owner, this);
                consumer.ValidateProposedCoordinatedReceipt(owner, _tick, _locked, _coordinatedReceipt);
                if (!Matches(consumer, consumer._inputFrameIssuer, owner, consumer._motor.NextExpectedTick, consumer._inputGeneration))
                    throw new InvalidOperationException("Movement input-frame candidate is foreign, stale, consumed, or forged.");
                if (_resetLifecycle) {consumer._hasCompletedSupport=false;consumer._motor.ResetForLifecycle();}
                consumer._commands = _commands;
                consumer._inputLocked = _locked;
                consumer._inputGeneration = _nextGeneration;
                if (_coordinatedReceipt.HasValue) consumer._pendingCoordinatedReceipt = _coordinatedReceipt;
                _committed = true;
                return _droppedCount;
            }
        }

        // AC-MOV-001, AC-MOV-002. Unity device input is intentionally absent; commands arrive through an internal seam.
        internal PlayerMotionSnapshot Snapshot => EnsureMotor().Snapshot;
        internal SimulationTick NextExpectedTick => EnsureMotor().NextExpectedTick;
        // REQ-C1B-G3OBS-003: 기존 모터가 없으면 생성·물리 설정 없이 거절한다.
        internal bool TryReadExistingMotionForAuthoring(out PlayerMotionSnapshot motion, out SimulationTick nextExpectedTick)
        {
            motion=default(PlayerMotionSnapshot);nextExpectedTick=default(SimulationTick);
            if(this==null || !isActiveAndEnabled || _motor==null)return false;
            motion=_motor.Snapshot;nextExpectedTick=_motor.NextExpectedTick;return true;
        }
        // REQ-AUTOCH1-SUPPORT-001: retained original cast facts, never another query or a per-tick proof object.
        private bool _hasCompletedSupport;
        private CompletedSupport _completedSupport;
        private struct SupportGeometry
        {
            internal BoxCollider2D Collider;internal Vector2 Size,Offset;internal Vector3 Position,Scale;internal Quaternion Rotation;internal Bounds Bounds;internal UnityEngine.SceneManagement.Scene Scene;
            internal bool Matches()=>Collider!=null && Collider.enabled && Collider.gameObject.activeInHierarchy && Collider.attachedRigidbody==null && !Collider.isTrigger &&
                ExactSupport(Collider.size,Size) && ExactSupport(Collider.offset,Offset) && ExactSupport(Collider.transform.position,Position) && ExactSupport(Collider.transform.lossyScale,Scale) && ExactSupport(Collider.transform.rotation,Rotation) && ExactSupport(Collider.bounds,Bounds) && Collider.gameObject.scene==Scene && Collider.gameObject.scene.IsValid() && Collider.gameObject.scene.isLoaded;
        }
        private struct PlayerSupportGeometry
        {
            internal CapsuleCollider2D Collider;internal Vector2 Size,Offset;internal Vector3 Scale;internal Quaternion Rotation;internal UnityEngine.SceneManagement.Scene Scene;
            internal bool Matches()=>Collider!=null && Collider.enabled && Collider.gameObject.activeInHierarchy && Collider.direction==CapsuleDirection2D.Vertical && ExactSupport(Collider.size,Size) && ExactSupport(Collider.offset,Offset) &&
                ExactSupport(Collider.transform.lossyScale,Scale) && ExactSupport(Collider.transform.rotation,Rotation) && Collider.gameObject.scene==Scene && Collider.gameObject.scene.IsValid() && Collider.gameObject.scene.isLoaded;
        }
        private struct SupportObservation
        {internal bool Valid;internal SupportGeometry Geometry;internal int NormalY,Distance;}
        private struct CompletedSupport
        {internal SupportObservation Observation;internal PlayerSupportGeometry Player;internal InputFrameCommitReceipt Receipt;internal PlayerMotionSnapshot Motion;internal Rect Body;internal bool DownwardResolution;}
        // REQ-AUTOCH1-SUPPORT-001: Unity geometry operators are approximate; retained evidence requires exact components.
        private static bool ExactSupport(Vector2 a,Vector2 b)=>a.x==b.x&&a.y==b.y;
        private static bool ExactSupport(Vector3 a,Vector3 b)=>a.x==b.x&&a.y==b.y&&a.z==b.z;
        private static bool ExactSupport(Quaternion a,Quaternion b)=>a.x==b.x&&a.y==b.y&&a.z==b.z&&a.w==b.w;
        private static bool ExactSupport(Bounds a,Bounds b)=>ExactSupport(a.center,b.center)&&ExactSupport(a.extents,b.extents);
        private static bool Finite(float v)=>!float.IsNaN(v)&&!float.IsInfinity(v);
        private static bool Finite(Vector3 v)=>Finite(v.x)&&Finite(v.y)&&Finite(v.z);
        private PlayerSupportGeometry CapturePlayerSupportGeometry()
        {
            try{var capsule=_collider as CapsuleCollider2D;if(capsule==null)return default;
                return new PlayerSupportGeometry{Collider=capsule,Size=capsule.size,Offset=capsule.offset,Scale=capsule.transform.lossyScale,Rotation=capsule.transform.rotation,Scene=capsule.gameObject.scene};}catch{return default;}
        }
        private static SupportObservation CaptureSupportObservation(Hit? hit)
        {
            try
            {
                if(!hit.HasValue || !(hit.Value.hit.collider is BoxCollider2D box) || box.attachedRigidbody!=null || box.isTrigger || !box.enabled || !box.gameObject.activeInHierarchy)return default;
                var geometry=new SupportGeometry{Collider=box,Size=box.size,Offset=box.offset,Position=box.transform.position,Scale=box.transform.lossyScale,Rotation=box.transform.rotation,Bounds=box.bounds,Scene=box.gameObject.scene};
                if(!Finite(geometry.Position)||!Finite(geometry.Scale)||!Finite(geometry.Bounds.center)||!Finite(geometry.Bounds.size)||!ExactSupport(geometry.Rotation,Quaternion.identity) || geometry.Scale.x<=0 || geometry.Scale.y<=0)return default;
                return new SupportObservation{Valid=true,Geometry=geometry,NormalY=QuantizeFloor(hit.Value.hit.normal.y),Distance=hit.Value.distanceQ4096};
            }catch{return default;}
        }
        private static bool CompletedBodyOnSupport(Rect body,SupportObservation support)
        {
            if(!support.Valid || support.NormalY<NormalQ4096 || !support.Geometry.Matches())return false;
            var bounds=support.Geometry.Bounds;
            if(!Finite(body.xMin)||!Finite(body.xMax)||!Finite(body.yMin)||!Finite(body.yMax)||body.xMin>=bounds.max.x || body.xMax<=bounds.min.x || body.center.x<=bounds.min.x || body.center.x>=bounds.max.x)return false;
            var gap=(long)QuantizeRound(body.yMin)-QuantizeRound(bounds.max.y);
            return gap>=0 && gap<=SkinQ4096;
        }
        private void PublishCompletedSupport(InputFrameCommitReceipt? receipt,PlayerSupportGeometry player,SupportObservation ground,SupportObservation down,MovementPrediction prediction,PlayerMotionSnapshot motion)
        {
            // Optional evidence cannot turn successful ordinary movement into a new failure.
            try
            {
                if(!receipt.HasValue || receipt.Value.Mode!=InputMode.GameplayEnabled || receipt.Value.Tick!=motion.Tick || !player.Matches() || !ExactSupport(player.Rotation,Quaternion.identity) ||
                    !Finite(player.Scale) || !Finite(player.Size.x)||!Finite(player.Size.y)||!Finite(player.Offset.x)||!Finite(player.Offset.y)||player.Scale.x<=0||player.Scale.y<=0)return;
                var chosen=down.Valid?down:prediction.PredictedPositionYQ4096==prediction.StartPositionYQ4096 && motion.PositionYQ4096==prediction.StartPositionYQ4096 && motion.VelocityYQ4096==0 && motion.IsGrounded?ground:default;
                if(!chosen.Valid || motion.VelocityYQ4096>0 || chosen.Geometry.Scene!=player.Scene)return;
                var size=Vector2.Scale(player.Size,new Vector2(player.Scale.x,player.Scale.y));var center=new Vector2(ToWorld(motion.PositionXQ4096),ToWorld(motion.PositionYQ4096))+Vector2.Scale(player.Offset,new Vector2(player.Scale.x,player.Scale.y));
                var body=new Rect(center-size/2,size);if(!CompletedBodyOnSupport(body,chosen))return;
                _completedSupport=new CompletedSupport{Observation=chosen,Player=player,Receipt=receipt.Value,Motion=motion,Body=body,DownwardResolution=down.Valid};_hasCompletedSupport=true;
            }catch{_hasCompletedSupport=false;}
        }
        internal bool TryReadCompletedSupport(SimulationTick tick,Collider2D exactSupport,InputFrameCommitReceipt receipt)
        {
            try
            {
                if(!_hasCompletedSupport || !isActiveAndEnabled || _motor==null || _inputLocked || _coordinatedInputSource==null || _coordinatedInputSource.IsFaulted || !_coordinatedInputSource.CurrentReceipt.HasValue ||
                    _coordinatedInputSource.CurrentReceipt.Value!=receipt || receipt.Mode!=InputMode.GameplayEnabled || receipt.Tick!=tick || _completedSupport.Receipt!=receipt || _completedSupport.Motion.Tick!=tick ||
                    _motor.Snapshot.Tick!=tick || _motor.Snapshot.PositionXQ4096!=_completedSupport.Motion.PositionXQ4096 || _motor.Snapshot.PositionYQ4096!=_completedSupport.Motion.PositionYQ4096 ||
                    !ReferenceEquals(_completedSupport.Player.Collider,_collider) || !ReferenceEquals(_completedSupport.Observation.Geometry.Collider,exactSupport) || !_completedSupport.Player.Matches())return false;
                return CompletedBodyOnSupport(_completedSupport.Body,_completedSupport.Observation);
            }catch{return false;}
        }
        private void OnDisable(){_hasCompletedSupport=false;}
        internal bool IsInputGateLocked => _inputLocked;

        internal void ValidateCoordinatedInputRegistration(object owner, IInputFrameCommitSource source, SimulationTick firstTick)
        {
            if (owner == null || source == null || source.IsFaulted || !ReferenceEquals(owner, source) || _motor == null || firstTick != _motor.NextExpectedTick)
                throw new InvalidOperationException("Movement coordinated input registration is not ready or identity-bound.");
            if ((_inputGateOwner != null && !ReferenceEquals(_inputGateOwner, owner)) ||
                (_coordinatedInputSource != null && !ReferenceEquals(_coordinatedInputSource, source)))
                throw new InvalidOperationException("Movement coordinated input registration conflicts with an existing owner.");
        }

        internal void RegisterCoordinatedInputOwner(object owner, IInputFrameCommitSource source, SimulationTick firstTick)
        {
            ValidateCoordinatedInputRegistration(owner, source, firstTick);
            RegisterInputGateOwner(owner);
            _coordinatedInputSource = source;
        }

        private void Awake()
        {
            EnsureMotor();
        }

        private void FixedUpdate()
        {
            AdvanceOneFixedTick();
        }

        // REQ-MOV-001. Exact and future tick-indexed commands are accepted once; late and duplicate commands are rejected.
        internal void SubmitCommand(MovementCommand command)
        {
            if (_inputLocked)
            {
                throw new InvalidOperationException("Movement command submission is blocked by the active input gate.");
            }

            var expected = EnsureMotor().NextExpectedTick.Value;
            if (command.Tick.Value < expected)
            {
                throw new InvalidOperationException($"Command tick {command.Tick.Value} is late; next expected tick is {expected}.");
            }

            if (_commands.ContainsKey(command.Tick.Value))
            {
                throw new InvalidOperationException($"A command is already queued for SimulationTick {command.Tick.Value}.");
            }

            var nextGeneration = checked(_inputGeneration + 1);
            _commands.Add(command.Tick.Value, command);
            _inputGeneration = nextGeneration;
        }

        internal InputFrameCandidate PrepareInputFrame(object owner, SimulationTick tick, bool locked, MovementCommand? input) =>
            PrepareInputFrameInternal(owner, tick, locked, input, null);

        internal InputFrameCandidate PrepareInputFrame(object owner, SimulationTick tick, bool locked, MovementCommand? input, InputFrameCommitReceipt receipt) =>
            PrepareInputFrameInternal(owner, tick, locked, input, receipt);

        private InputFrameCandidate PrepareInputFrameInternal(object owner, SimulationTick tick, bool locked, MovementCommand? input, InputFrameCommitReceipt? receipt)
        {
            EnsureInputFrameOwner(owner);
            ValidateProposedCoordinatedReceipt(owner, tick, locked, receipt);
            var motor = _motor ?? throw new InvalidOperationException("Movement input frame requires an initialized motor.");
            if (tick != motor.NextExpectedTick) throw new InvalidOperationException("Movement input frame must target the exact unconsumed tick.");
            if (locked && input.HasValue) throw new InvalidOperationException("A locked movement input frame cannot carry a command.");
            var staged = CopyCommands();
            var changed = _inputLocked != locked;
            var dropped = 0;
            if (changed) { dropped = staged.Count; staged.Clear(); }
            if (input.HasValue)
            {
                var command = input.Value;
                if (locked || command.Tick != tick || staged.ContainsKey(tick.Value)) throw new InvalidOperationException("Movement input frame command is invalid or conflicts with the exact tick.");
                staged.Add(tick.Value, command);
                changed = true;
            }
            var next = checked(_inputGeneration + 1);
            return new InputFrameCandidate(this, _inputFrameIssuer, owner, tick, _inputGeneration, next, staged, locked, dropped, changed && locked, receipt);
        }

        internal void ValidateInputFrameCandidate(object owner, InputFrameCandidate candidate)
        {
            if (candidate == null || _motor == null || !candidate.Matches(this, _inputFrameIssuer, owner, _motor.NextExpectedTick, _inputGeneration))
                throw new InvalidOperationException("Movement input-frame candidate is foreign, stale, consumed, or forged.");
        }

        internal int CommitInputFrame(object owner, InputFrameCandidate candidate)
        {
            ValidateInputFrameCandidate(owner, candidate);
            return candidate.ApplyToMatchingConsumer(this, _inputFrameIssuer, owner);
        }

        internal void SetModifierForTests(PlayerMovementModifierKind modifier) => _modifier = modifier;
        internal void SetInputLockedForTests(bool inputLocked)
        {
            if (_inputGateOwner != null)
            {
                throw new InvalidOperationException("Test input locking cannot bypass a registered input-gate owner.");
            }

            if (_inputLocked != inputLocked)
            {
                var nextGeneration = checked(_inputGeneration + 1);
                _inputLocked = inputLocked;
                _inputGeneration = nextGeneration;
                return;
            }

            _inputLocked = inputLocked;
        }
        internal void ResetForLifecycleForTests() {_hasCompletedSupport=false;EnsureMotor().ResetForLifecycle();}

        // REQ-UX-004, REQ-UX-008, REQ-MOV-001, REQ-MOV-002, REQ-MOV-007,
        // REQ-MOV-008. A future InputMode owner has one instance-bound sink;
        // same-owner registration is deliberately idempotent.
        internal void RegisterInputGateOwner(object owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (_inputGateOwner != null && !ReferenceEquals(_inputGateOwner, owner))
            {
                throw new InvalidOperationException("A different movement input-gate owner is already registered.");
            }

            _inputGateOwner = owner;
        }

        // REQ-UX-004, REQ-UX-008, REQ-MOV-001, REQ-MOV-002, REQ-MOV-007,
        // REQ-MOV-008. The gate changes only this consumer's command queue and
        // existing motor lifecycle buffers; external directive lanes are retained.
        internal int ApplyInputGate(object owner, SimulationTick tick, bool locked)
        {
            if (_inputGateOwner == null || !ReferenceEquals(_inputGateOwner, owner))
            {
                throw new InvalidOperationException("The caller is not the registered movement input-gate owner.");
            }

            if (tick != EnsureMotor().NextExpectedTick)
            {
                throw new InvalidOperationException("Movement input-gate application must target the exact next tick.");
            }

            if (_inputLocked == locked)
            {
                return 0;
            }

            var droppedCount = _commands.Count;
            var nextGeneration = checked(_inputGeneration + 1);
            _commands.Clear();
            if (locked)
            {
                _hasCompletedSupport=false;EnsureMotor().ResetForLifecycle();
            }

            _inputLocked = locked;
            _inputGeneration = nextGeneration;
            return droppedCount;
        }

        private void EnsureInputFrameOwner(object owner)
        {
            if (owner == null || _inputGateOwner == null || !ReferenceEquals(_inputGateOwner, owner))
                throw new InvalidOperationException("The caller is not the registered movement input-frame owner.");
        }

        private SortedDictionary<int, MovementCommand> CopyCommands()
        {
            var staged = new SortedDictionary<int, MovementCommand>();
            var nextTick = _motor == null
                ? throw new InvalidOperationException("Movement input frame requires an initialized motor.")
                : _motor.NextExpectedTick.Value;
            foreach (var entry in _commands)
            {
                if (entry.Key != entry.Value.Tick.Value || entry.Key < nextTick)
                    throw new InvalidOperationException("Queued movement command key is invalid or late.");
                staged.Add(entry.Key, entry.Value);
            }
            return staged;
        }

        // REQ-MOV-001, REQ-MOV-005. A graph has exactly one producer identity;
        // reference identity, rather than Unity instance IDs, is the private lane key.
        internal void RegisterExternalMovementDirectiveProducer(object producerOwner)
        {
            if (producerOwner == null)
            {
                throw new ArgumentNullException(nameof(producerOwner));
            }

            if (_externalMovementDirectiveOwner != null && !ReferenceEquals(_externalMovementDirectiveOwner, producerOwner))
            {
                throw new InvalidOperationException("A different external movement directive producer is already registered.");
            }

            _externalMovementDirectiveOwner = producerOwner;
        }

        // REQ-MOV-001, REQ-MOV-005; AC-M4B3B3-003. This is mutation-free and
        // reserves nothing: a commit is the first operation that changes the queue.
        internal ExternalMovementDirectiveCandidate PreflightExternalMovementDirective(
            object producerOwner,
            long attackOrdinal,
            SimulationTick movementTick,
            string directiveId,
            int anchorXQ4096,
            int anchorYQ4096,
            int maximumStepQ4096)
        {
            var motor = EnsureMotor();
            EnsureExternalMovementDirectiveOwner(producerOwner);
            if (attackOrdinal < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attackOrdinal));
            }

            var expectedDeliveryTick = checked(motor.NextExpectedTick.Value + 1);
            if (movementTick.Value != expectedDeliveryTick)
            {
                throw new InvalidOperationException("An external movement directive must target exactly the next future movement tick.");
            }

            if (_futureExternalMovementDirective.HasValue)
            {
                throw new InvalidOperationException("Only one future external movement directive may be pending.");
            }

            var directive = new ExternalMovementDirective(
                movementTick,
                directiveId,
                anchorXQ4096,
                anchorYQ4096,
                maximumStepQ4096);
            ValidateAuditPullDescriptor(directive, attackOrdinal);
            return new ExternalMovementDirectiveCandidate(
                this,
                producerOwner,
                directive,
                attackOrdinal,
                _externalMovementDirectiveQueueRevision,
                motor.NextExpectedTick.Value);
        }

        internal void CommitPreflightedExternalMovementDirective(
            object producerOwner,
            ExternalMovementDirectiveCandidate candidate)
        {
            if (candidate == null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            var motor = EnsureMotor();
            EnsureExternalMovementDirectiveOwner(producerOwner);
            if (!candidate.Matches(this, producerOwner, _externalMovementDirectiveQueueRevision, motor.NextExpectedTick.Value) ||
                _futureExternalMovementDirective.HasValue ||
                candidate.Directive.MovementTick.Value != checked(motor.NextExpectedTick.Value + 1))
            {
                throw new InvalidOperationException("The external movement directive candidate is stale, cross-wired, duplicate, or forged.");
            }

            _futureExternalMovementDirective = new QueuedExternalMovementDirective(candidate.Directive, candidate.AttackOrdinal);
            _externalMovementDirectiveQueueRevision = checked(_externalMovementDirectiveQueueRevision + 1);
        }

        // REQ-MOV-001, REQ-MOV-005; AC-M4B3B3-003. Cancellation can affect only
        // the unconsumed current-tick directive owned by this exact producer.
        internal void CancelExternalMovementDirective(
            object producerOwner,
            long attackOrdinal,
            SimulationTick movementTick,
            string directiveId)
        {
            var motor = EnsureMotor();
            EnsureExternalMovementDirectiveOwner(producerOwner);
            if (attackOrdinal < 0 || movementTick != motor.NextExpectedTick)
            {
                throw new InvalidOperationException("Only the exact current external movement directive may be cancelled.");
            }

            if (!_dueExternalMovementDirective.HasValue)
            {
                throw new InvalidOperationException("There is no current external movement directive to cancel.");
            }

            var due = _dueExternalMovementDirective.Value;
            if (due.AttackOrdinal != attackOrdinal || due.Directive.MovementTick != movementTick ||
                !string.Equals(due.Directive.DirectiveId, directiveId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("External movement directive cancellation identity does not match the pending directive.");
            }

            _dueExternalMovementDirective = null;
            _externalMovementDirectiveQueueRevision = checked(_externalMovementDirectiveQueueRevision + 1);
        }

        // REQ-MOV-001, REQ-MOV-005. Handoff reads a receipt only for the exact
        // committed tick; stale or missing receipts are never inferred from state.
        internal bool TryGetExternalMovementDirectiveReceipt(
            SimulationTick movementTick,
            out ExternalMovementDirectiveReceipt receipt)
        {
            if (_hasExternalMovementDirectiveReceipt && _lastExternalMovementDirectiveReceipt.MovementTick == movementTick)
            {
                receipt = _lastExternalMovementDirectiveReceipt;
                return true;
            }

            receipt = default;
            return false;
        }

        // REQ-WT-006, REQ-MOV-004. The transfer phase checks tick ownership without publishing a second movement state.
        internal void PreflightTransferModifier(SimulationTick tick)
        {
            var motor = _motor ?? throw new InvalidOperationException("Transfer modifier preflight requires the initialized movement controller.");
            if (motor.NextExpectedTick != tick)
            {
                throw new InvalidOperationException($"Transfer tick {tick.Value} does not match next expected movement tick {motor.NextExpectedTick.Value}.");
            }
        }

        // REQ-WT-006, REQ-MOV-004. After successful preflight, only the frozen existing modifier is reflected before motor BeginTick.
        internal void ApplyPreflightedTransferModifier(PlayerMovementModifierKind modifier, SimulationTick tick)
        {
            // A successful preflight plus -200 same-phase ownership proves the retained tick; final reflection must not reject after a target mutation.
            _ = tick;
            _modifier = modifier;
            ApplyCompatibilityGravityScale();
        }

        // AC-MOV-001, AC-MOV-002, AC-MOV-005. Test-only deterministic entry point; production calls FixedUpdate.
        internal PlayerMotionSnapshot AdvanceOneFixedTickForTests()
        {
            AdvanceOneFixedTick();
            return Snapshot;
        }

        internal static string GetStableAuthoredId(Collider2D collider) => StableAuthoredId(collider);

        private void AdvanceOneFixedTick()
        {
            _hasCompletedSupport=false;
            try
            {
            var motor = _coordinatedInputSource == null
                ? EnsureMotor()
                : _motor ?? throw new InvalidOperationException("Coordinated Movement requires an initialized motor.");
            var tick = motor.NextExpectedTick;
            var supportReceipt=_pendingCoordinatedReceipt;
            ConsumeCoordinatedReceipt(tick);
            var playerSupport=CapturePlayerSupportGeometry();
            ValidateExternalMovementDirectiveState(tick);
            var dueDirective = _dueExternalMovementDirective;
            var externalMovement = dueDirective.HasValue
                ? motor.PreflightExternalMovementDirective(dueDirective.Value.Directive)
                : null;
            var command = ConsumeCommandOrNeutral(tick);
            var dashDirection = motor.GetDashDirectionForCommand(command);
            var contacts = ReadValidatedContacts(dashDirection, motor.IsDashScheduledOrRequested(command),out var groundSupport);
            ApplyCompatibilityGravityScale();
            var prediction = motor.BeginTick(command, contacts, _modifier, _inputLocked, externalMovement);
            var resolution = ResolvePrediction(prediction,out var downSupport);
            var snapshot = motor.CommitTick(resolution);
            _body.MovePosition(new Vector2(ToWorld(snapshot.PositionXQ4096), ToWorld(snapshot.PositionYQ4096)));

            if (dueDirective.HasValue)
            {
                _dueExternalMovementDirective = null;
                _lastExternalMovementDirectiveReceipt = new ExternalMovementDirectiveReceipt(
                    tick,
                    true,
                    dueDirective.Value.Directive.DirectiveId,
                    externalMovement.Delta.XQ4096,
                    externalMovement.Delta.YQ4096);
            }
            else
            {
                _lastExternalMovementDirectiveReceipt = new ExternalMovementDirectiveReceipt(tick, false, string.Empty, 0, 0);
            }

            _hasExternalMovementDirectiveReceipt = true;
            PromoteFutureExternalMovementDirective(motor.NextExpectedTick);
            PublishCompletedSupport(supportReceipt,playerSupport,groundSupport,downSupport,prediction,snapshot);
            }catch{_hasCompletedSupport=false;throw;}
        }

        private void ValidateProposedCoordinatedReceipt(object owner, SimulationTick tick, bool locked, InputFrameCommitReceipt? receipt)
        {
            if (_coordinatedInputSource == null)
            {
                if (receipt.HasValue) throw new InvalidOperationException("Movement is not registered for coordinated input.");
                return;
            }
            if (!receipt.HasValue || !ReferenceEquals(owner, _coordinatedInputSource) || _coordinatedInputSource.IsFaulted || receipt.Value.Tick != tick ||
                receipt.Value.FrameOrdinal != checked(_lastConsumedCoordinatedOrdinal + 1) || (!_hasCoordinatedModeEpoch && receipt.Value.ModeEpoch != 0) ||
                (_hasCoordinatedModeEpoch && (receipt.Value.ModeEpoch < _lastCoordinatedModeEpoch || (receipt.Value.ModeEpoch != _lastCoordinatedModeEpoch && (_lastCoordinatedModeEpoch == long.MaxValue || receipt.Value.ModeEpoch != _lastCoordinatedModeEpoch + 1)))) || (locked != (receipt.Value.Mode != InputMode.GameplayEnabled)))
                throw new InvalidOperationException("Movement coordinated input receipt is invalid.");
        }

        private void ConsumeCoordinatedReceipt(SimulationTick tick)
        {
            if (_coordinatedInputSource == null) return;
            if (_coordinatedInputSource.IsFaulted || !_pendingCoordinatedReceipt.HasValue || !_coordinatedInputSource.CurrentReceipt.HasValue ||
                _pendingCoordinatedReceipt.Value != _coordinatedInputSource.CurrentReceipt.Value || _pendingCoordinatedReceipt.Value.Tick != tick ||
                _pendingCoordinatedReceipt.Value.FrameOrdinal != checked(_lastConsumedCoordinatedOrdinal + 1) ||
                (!_hasCoordinatedModeEpoch && _pendingCoordinatedReceipt.Value.ModeEpoch != 0) ||
                (_hasCoordinatedModeEpoch && (_pendingCoordinatedReceipt.Value.ModeEpoch < _lastCoordinatedModeEpoch ||
                    (_pendingCoordinatedReceipt.Value.ModeEpoch != _lastCoordinatedModeEpoch &&
                     (_lastCoordinatedModeEpoch == long.MaxValue || _pendingCoordinatedReceipt.Value.ModeEpoch != _lastCoordinatedModeEpoch + 1)))) ||
                _inputLocked != (_pendingCoordinatedReceipt.Value.Mode != InputMode.GameplayEnabled))
                throw new InvalidOperationException("Movement coordinated input proof is missing or stale.");
            _lastConsumedCoordinatedOrdinal = _pendingCoordinatedReceipt.Value.FrameOrdinal;
            _lastCoordinatedModeEpoch = _pendingCoordinatedReceipt.Value.ModeEpoch;
            _hasCoordinatedModeEpoch = true;
            _pendingCoordinatedReceipt = null;
        }

        private void ValidateExternalMovementDirectiveState(SimulationTick currentTick)
        {
            if (_dueExternalMovementDirective.HasValue && _dueExternalMovementDirective.Value.Directive.MovementTick != currentTick)
            {
                throw new InvalidOperationException("The pending external movement directive is stale or has the wrong tick horizon.");
            }

            if (_futureExternalMovementDirective.HasValue &&
                _futureExternalMovementDirective.Value.Directive.MovementTick.Value != checked(currentTick.Value + 1))
            {
                throw new InvalidOperationException("The future external movement directive has the wrong tick horizon.");
            }
        }

        private void PromoteFutureExternalMovementDirective(SimulationTick nextTick)
        {
            if (!_futureExternalMovementDirective.HasValue)
            {
                return;
            }

            if (_dueExternalMovementDirective.HasValue || _futureExternalMovementDirective.Value.Directive.MovementTick != nextTick)
            {
                throw new InvalidOperationException("External movement directive promotion would overwrite or skip a pending tick.");
            }

            _dueExternalMovementDirective = _futureExternalMovementDirective;
            _futureExternalMovementDirective = null;
        }

        private MovementCommand ConsumeCommandOrNeutral(SimulationTick tick)
        {
            if (_commands.TryGetValue(tick.Value, out var command))
            {
                var nextGeneration = checked(_inputGeneration + 1);
                _commands.Remove(tick.Value);
                _inputGeneration = nextGeneration;
                return command;
            }

            // A neutral command is irrevocably consumed for this exact tick before the motor begins it.
            return new MovementCommand(tick, 0, 0, false, false, false);
        }

        private MovementContacts ReadValidatedContacts(int dashDirection, bool shouldProbeDash,out SupportObservation groundSupport)
        {
            var groundHit = FindBestHit(Vector2.down, ProbeQ4096, hit => QuantizeFloor(hit.normal.y) >= NormalQ4096);
            groundSupport=CaptureSupportObservation(groundHit);var grounded=groundHit.HasValue;
            var right = FindBestHit(Vector2.right, ProbeQ4096, hit => QuantizeFloor(-hit.normal.x) >= NormalQ4096);
            var left = FindBestHit(Vector2.left, ProbeQ4096, hit => QuantizeFloor(hit.normal.x) >= NormalQ4096);
            var wall = SelectWall(right, left);
            var dashBlocked = shouldProbeDash &&
                FindBestHit(dashDirection < 0 ? Vector2.left : Vector2.right, DashProbeQ4096(), _ => true).HasValue;
            return wall.HasValue
                ? new MovementContacts(grounded, StableAuthoredId(wall.Value.collider.collider), wall.Value.side, dashBlocked)
                : new MovementContacts(grounded, string.Empty, WallSide.None, dashBlocked);
        }

        private MovementCollisionResolution ResolvePrediction(MovementPrediction prediction,out SupportObservation downSupport)
        {
            var x = ResolveAxis(
                prediction.StartPositionXQ4096,
                prediction.PredictedPositionXQ4096,
                new Vector2(ToWorld(prediction.StartPositionXQ4096), ToWorld(prediction.StartPositionYQ4096)),
                true,out _);
            var y = ResolveAxis(
                prediction.StartPositionYQ4096,
                prediction.PredictedPositionYQ4096,
                new Vector2(ToWorld(x.positionQ4096), ToWorld(prediction.StartPositionYQ4096)),
                false,out downSupport);
            return new MovementCollisionResolution(prediction.Tick, x.positionQ4096, y.positionQ4096, x.blocked, y.blocked);
        }

        private (int positionQ4096, bool blocked) ResolveAxis(int startQ4096, int targetQ4096, Vector2 castOrigin, bool horizontal,out SupportObservation downSupport)
        {
            downSupport=default;
            var delta = targetQ4096 - startQ4096;
            if (delta == 0)
            {
                return (targetQ4096, false);
            }

            var direction = horizontal
                ? delta < 0 ? Vector2.left : Vector2.right
                : delta < 0 ? Vector2.down : Vector2.up;
            var hit = FindBestHitAt(castOrigin, direction, Math.Abs(delta) + SkinQ4096, _ => true);
            if (!hit.HasValue)
            {
                return (targetQ4096, false);
            }

            if(!horizontal && delta<0)downSupport=CaptureSupportObservation(hit);
            var allowed = Math.Max(0, hit.Value.distanceQ4096 - SkinQ4096);
            var signedAllowed = delta < 0 ? -allowed : allowed;
            return (checked(startQ4096 + signedAllowed), true);
        }

        private (RaycastHit2D collider, WallSide side, int distanceQ4096)? SelectWall(Hit? right, Hit? left)
        {
            if (!right.HasValue && !left.HasValue)
            {
                return null;
            }

            if (!right.HasValue)
            {
                return (left.Value.hit, WallSide.Left, left.Value.distanceQ4096);
            }

            if (!left.HasValue)
            {
                return (right.Value.hit, WallSide.Right, right.Value.distanceQ4096);
            }

            return CompareHits(right.Value, left.Value) <= 0
                ? (right.Value.hit, WallSide.Right, right.Value.distanceQ4096)
                : (left.Value.hit, WallSide.Left, left.Value.distanceQ4096);
        }

        private Hit? FindBestHit(Vector2 direction, int distanceQ4096, Func<RaycastHit2D, bool> predicate)
        {
            return FindBestHitAt(_body.position, direction, distanceQ4096, predicate);
        }

        // The temporary kinematic pose is query-only: it is restored before CommitTick and the sole published move is MovePosition.
        private Hit? FindBestHitAt(Vector2 origin, Vector2 direction, int distanceQ4096, Func<RaycastHit2D, bool> predicate)
        {
            var filter = new ContactFilter2D();
            filter.NoFilter();
            filter.useTriggers = false;
            var original = _body.position;
            var movedForQuery = original != origin;
            if (movedForQuery)
            {
                _body.position = origin;
                Physics2D.SyncTransforms();
            }

            try
            {
                var count = _collider.Cast(direction, filter, _hits, ToWorld(distanceQ4096));
                Hit? best = null;
                for (var index = 0; index < count; index++)
                {
                    var hit = _hits[index];
                    if (hit.collider == null || hit.collider == _collider || hit.collider.isTrigger)
                    {
                        continue;
                    }

                    // REQ-MOV-001, REQ-MOV-004, REQ-MOV-006; AC-MOV-001, AC-MOV-004,
                    // AC-MOV-005. Unity 6000.3 raw attached-collider casts retain exact
                    // ignored pairs, so Movement filters pair truth before predicate/math/order.
                    if (Physics2D.GetIgnoreCollision(_collider, hit.collider))
                    {
                        continue;
                    }

                    if (!predicate(hit))
                    {
                        continue;
                    }

                    var candidate = new Hit(hit, Math.Max(0, QuantizeFloor(hit.distance)));
                    if (!best.HasValue || CompareHits(candidate, best.Value) < 0)
                    {
                        best = candidate;
                    }
                }

                return best;
            }
            finally
            {
                if (movedForQuery)
                {
                    _body.position = original;
                    Physics2D.SyncTransforms();
                }
            }
        }

        private static int CompareHits(Hit left, Hit right)
        {
            var distance = left.distanceQ4096.CompareTo(right.distanceQ4096);
            if (distance != 0)
            {
                return distance;
            }

            var path = string.CompareOrdinal(HierarchyPath(left.hit.collider.transform), HierarchyPath(right.hit.collider.transform));
            return path != 0 ? path : string.CompareOrdinal(left.hit.collider.gameObject.scene.name, right.hit.collider.gameObject.scene.name);
        }

        private int DashProbeQ4096() => checked((int)Math.Ceiling((double)FixedQ4096.DashVelocity / FixedQ4096.TicksPerSecond) + SkinQ4096);

        private PlayerMovementMotor EnsureMotor()
        {
            if (_motor != null)
            {
                return _motor;
            }

            _body = _body == null ? GetComponent<Rigidbody2D>() : _body;
            _collider = _collider == null ? GetComponent<Collider2D>() : _collider;
            if (_body == null || _collider == null)
            {
                throw new InvalidOperationException("PlayerMovementController requires Rigidbody2D and Collider2D components.");
            }

            _body.bodyType = RigidbodyType2D.Kinematic;
            _body.useFullKinematicContacts = false;
            _collider.isTrigger = false;
            ApplyCompatibilityGravityScale();
            if(_chapterFirstTick.HasValue){_body.position=_chapterPosition;transform.position=new Vector3(_chapterPosition.x,_chapterPosition.y,transform.position.z);}
            _motor = new PlayerMovementMotor(_chapterFirstTick ?? new SimulationTick(0));
            _motor.SeedAuthoritativePosition(QuantizeRound(_body.position.x), QuantizeRound(_body.position.y));
            return _motor;
        }

        // REQ-MOV-004, REQ-MOV-006. Kinematic compatibility/inspection value only; motor gravity remains authoritative.
        private void ApplyCompatibilityGravityScale()
        {
            _body.gravityScale = _modifier == PlayerMovementModifierKind.Lightweight
                ? BaselineCompatibilityGravityScale * 0.65f
                : BaselineCompatibilityGravityScale;
        }

        private void EnsureExternalMovementDirectiveOwner(object producerOwner)
        {
            if (producerOwner == null || !ReferenceEquals(_externalMovementDirectiveOwner, producerOwner))
            {
                throw new InvalidOperationException("The caller is not the registered external movement directive producer.");
            }
        }

        private static void ValidateAuditPullDescriptor(ExternalMovementDirective directive, long attackOrdinal)
        {
            if (directive.AnchorXQ4096 != 28672 || directive.AnchorYQ4096 != 4915 || directive.MaximumStepQ4096 != 410)
            {
                throw new InvalidOperationException("The BalanceAudit pull descriptor does not match its frozen authored anchor or step.");
            }

            var expectedId = string.Format(
                CultureInfo.InvariantCulture,
                "ordan.audit-pull/{0:D19}/{1:D10}",
                attackOrdinal,
                directive.MovementTick.Value);
            if (!string.Equals(directive.DirectiveId, expectedId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("The BalanceAudit pull directive ID does not match its ordinal and delivery tick.");
            }
        }

        private static string StableAuthoredId(Collider2D collider)
        {
            if (collider == null)
            {
                throw new ArgumentNullException(nameof(collider));
            }

            var sceneName = collider.gameObject.scene.name;
            var path = HierarchyPath(collider.transform);
            if (string.IsNullOrEmpty(sceneName) || string.IsNullOrEmpty(path))
            {
                throw new InvalidOperationException("Movement collision authoring requires non-empty scene and hierarchy names.");
            }

            return sceneName + ":" + path;
        }

        private static string HierarchyPath(Transform transform)
        {
            var names = new List<string>();
            for (var current = transform; current != null; current = current.parent)
            {
                if (string.IsNullOrEmpty(current.name))
                {
                    throw new InvalidOperationException("Movement collision authoring requires non-empty transform names.");
                }

                names.Add(current.name);
            }

            names.Reverse();
            return string.Join("/", names);
        }

        private static int QuantizeFloor(float value) => checked((int)Math.Floor(value * UnitsPerWorldUnit));
        private static int QuantizeRound(float value) => checked((int)Math.Round(value * UnitsPerWorldUnit, MidpointRounding.AwayFromZero));
        private static float ToWorld(int valueQ4096) => valueQ4096 / UnitsPerWorldUnit;

        private readonly struct Hit
        {
            internal Hit(RaycastHit2D hit, int distanceQ4096)
            {
                this.hit = hit;
                this.distanceQ4096 = distanceQ4096;
            }

            internal readonly RaycastHit2D hit;
            internal readonly int distanceQ4096;
        }

        private readonly struct QueuedExternalMovementDirective
        {
            internal QueuedExternalMovementDirective(ExternalMovementDirective directive, long attackOrdinal)
            {
                Directive = directive;
                AttackOrdinal = attackOrdinal;
            }

            internal ExternalMovementDirective Directive { get; }
            internal long AttackOrdinal { get; }
        }
    }
}
