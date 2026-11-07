using System;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat
{
    internal enum RegularEnemyLocomotionInputKind { Gameplay = 0, EncounterReset = 1 }

    internal readonly struct PositionQ4096
    {
        internal PositionQ4096(int x, int y) { X = x; Y = y; }
        internal int X { get; }
        internal int Y { get; }
    }

    internal readonly struct RegularEnemyLocomotionPairInput
    {
        private RegularEnemyLocomotionPairInput(RegularEnemyLocomotionInputKind kind, SimulationTick tick,
            RegularEnemyBehaviorPairSnapshot behavior, RegularEnemyReactionSnapshot walkerReaction,
            RegularEnemyReactionSnapshot surveyorReaction)
        {
            if (kind != RegularEnemyLocomotionInputKind.Gameplay && kind != RegularEnemyLocomotionInputKind.EncounterReset)
                throw new ArgumentOutOfRangeException(nameof(kind));
            if (tick.Value < 0) throw new ArgumentOutOfRangeException(nameof(tick));
            Kind = kind; Tick = tick; Behavior = behavior; WalkerReaction = walkerReaction; SurveyorReaction = surveyorReaction;
        }

        internal static RegularEnemyLocomotionPairInput Gameplay(SimulationTick tick, RegularEnemyBehaviorPairSnapshot behavior,
            RegularEnemyReactionSnapshot walkerReaction, RegularEnemyReactionSnapshot surveyorReaction) =>
            new RegularEnemyLocomotionPairInput(RegularEnemyLocomotionInputKind.Gameplay, tick, behavior, walkerReaction, surveyorReaction);

        internal static RegularEnemyLocomotionPairInput EncounterReset(SimulationTick tick, RegularEnemyBehaviorPairSnapshot behavior,
            RegularEnemyReactionSnapshot walkerReaction, RegularEnemyReactionSnapshot surveyorReaction) =>
            new RegularEnemyLocomotionPairInput(RegularEnemyLocomotionInputKind.EncounterReset, tick, behavior, walkerReaction, surveyorReaction);

        internal static void AssertInvalidDiscriminantRejectedForTests() =>
            _ = new RegularEnemyLocomotionPairInput((RegularEnemyLocomotionInputKind)int.MaxValue, new SimulationTick(0), default, default, default);

        internal RegularEnemyLocomotionInputKind Kind { get; }
        internal SimulationTick Tick { get; }
        internal RegularEnemyBehaviorPairSnapshot Behavior { get; }
        internal RegularEnemyReactionSnapshot WalkerReaction { get; }
        internal RegularEnemyReactionSnapshot SurveyorReaction { get; }
    }

    internal readonly struct RegularEnemyMotionSnapshot
    {
        internal RegularEnemyMotionSnapshot(string roleId, PositionQ4096 position, PositionQ4096 velocity,
            long xPositionRemainder, long yPositionRemainder, long gravityRemainder, bool grounded, bool dead)
        {
            RoleId = roleId; Position = position; Velocity = velocity; XPositionRemainder = xPositionRemainder;
            YPositionRemainder = yPositionRemainder; GravityRemainder = gravityRemainder; Grounded = grounded; Dead = dead;
        }
        internal string RoleId { get; }
        internal PositionQ4096 Position { get; }
        internal PositionQ4096 Velocity { get; }
        internal long XPositionRemainder { get; }
        internal long YPositionRemainder { get; }
        internal long GravityRemainder { get; }
        internal bool Grounded { get; }
        internal bool Dead { get; }
    }

    internal readonly struct RegularEnemyMotionPairSnapshot
    {
        internal RegularEnemyMotionPairSnapshot(SimulationTick tick, RegularEnemyMotionSnapshot walker,
            RegularEnemyMotionSnapshot surveyor, SimulationTick nextExpectedTick)
        { Tick = tick; Walker = walker; Surveyor = surveyor; NextExpectedTick = nextExpectedTick; }
        internal SimulationTick Tick { get; }
        internal RegularEnemyMotionSnapshot Walker { get; }
        internal RegularEnemyMotionSnapshot Surveyor { get; }
        internal SimulationTick NextExpectedTick { get; }
    }

    internal readonly struct RegularEnemyMotionPrediction
    {
        internal RegularEnemyMotionPrediction(SimulationTick tick, string roleId, bool alive, PositionQ4096 startPosition,
            PositionQ4096 startVelocity, PositionQ4096 desiredVelocity, PositionQ4096 predictedPosition,
            long xPositionRemainder, long yPositionRemainder, long gravityRemainder, bool priorGrounded)
        {
            Tick = tick; RoleId = roleId; Alive = alive; StartPosition = startPosition; StartVelocity = startVelocity;
            DesiredVelocity = desiredVelocity; PredictedPosition = predictedPosition; XPositionRemainder = xPositionRemainder;
            YPositionRemainder = yPositionRemainder; GravityRemainder = gravityRemainder; PriorGrounded = priorGrounded;
        }
        internal SimulationTick Tick { get; }
        internal string RoleId { get; }
        internal bool Alive { get; }
        internal PositionQ4096 StartPosition { get; }
        internal PositionQ4096 StartVelocity { get; }
        internal PositionQ4096 DesiredVelocity { get; }
        internal PositionQ4096 PredictedPosition { get; }
        internal long XPositionRemainder { get; }
        internal long YPositionRemainder { get; }
        internal long GravityRemainder { get; }
        internal bool PriorGrounded { get; }
    }

    internal readonly struct RegularEnemyMotionPairPrediction
    {
        internal RegularEnemyMotionPairPrediction(SimulationTick tick, RegularEnemyMotionPrediction walker, RegularEnemyMotionPrediction surveyor)
        { Tick = tick; Walker = walker; Surveyor = surveyor; }
        internal SimulationTick Tick { get; }
        internal RegularEnemyMotionPrediction Walker { get; }
        internal RegularEnemyMotionPrediction Surveyor { get; }
    }

    internal readonly struct RegularEnemyCollisionResolution
    {
        internal RegularEnemyCollisionResolution(SimulationTick tick, string roleId, PositionQ4096 startPosition,
            PositionQ4096 predictedPosition, PositionQ4096 resolvedPosition, bool xBlocked, bool yBlocked,
            bool stationarySupportConfirmed, bool grounded)
        {
            Tick = tick; RoleId = roleId; StartPosition = startPosition; PredictedPosition = predictedPosition;
            ResolvedPosition = resolvedPosition; XBlocked = xBlocked; YBlocked = yBlocked;
            StationarySupportConfirmed = stationarySupportConfirmed; Grounded = grounded;
        }
        internal SimulationTick Tick { get; }
        internal string RoleId { get; }
        internal PositionQ4096 StartPosition { get; }
        internal PositionQ4096 PredictedPosition { get; }
        internal PositionQ4096 ResolvedPosition { get; }
        internal bool XBlocked { get; }
        internal bool YBlocked { get; }
        internal bool StationarySupportConfirmed { get; }
        internal bool Grounded { get; }
    }

    internal readonly struct RegularEnemyCollisionPairResolution
    {
        internal RegularEnemyCollisionPairResolution(RegularEnemyCollisionResolution walker, RegularEnemyCollisionResolution surveyor)
        { Walker = walker; Surveyor = surveyor; }
        internal RegularEnemyCollisionResolution Walker { get; }
        internal RegularEnemyCollisionResolution Surveyor { get; }
    }

    internal readonly struct RegularEnemyResolvedPairCandidate
    {
        internal RegularEnemyResolvedPairCandidate(RegularEnemyMotionPairSnapshot snapshot) { Snapshot = snapshot; }
        internal RegularEnemyMotionPairSnapshot Snapshot { get; }
    }

    internal readonly struct RegularEnemyResetCandidate
    {
        internal RegularEnemyResetCandidate(RegularEnemyMotionPairSnapshot snapshot) { Snapshot = snapshot; }
        internal RegularEnemyMotionPairSnapshot Snapshot { get; }
    }

    // REQ-COM-001, REQ-COM-002, REQ-COM-004, REQ-COM-005; affected REQ-WT-003, REQ-WT-005, REQ-WT-007.
    // Engine-free pair-atomic Q4096 locomotion owner. Unity collision truth belongs to M3C2.
    internal sealed class RegularEnemyLocomotionSession
    {
        private const string WalkerId = "walker";
        private const string SurveyorId = "surveyor";
        private const int ApproachRate = 12288;
        private const int DashRate = 49152;
        private const int RelocateRate = 10240;
        private const int GravityRate = 81920;
        private const int MaximumFallRate = 49152;
        private readonly PositionQ4096 _walkerSpawn;
        private readonly PositionQ4096 _surveyorSpawn;
        private readonly bool _walkerGroundSeed;
        private readonly bool _surveyorGroundSeed;
        private SimulationTick _nextExpectedTick;
        private RegularEnemyMotionSnapshot _walker;
        private RegularEnemyMotionSnapshot _surveyor;
        private bool _hasCompletedGameplay;
        private bool _walkerDead;
        private bool _surveyorDead;
        private RegularEnemyMotionPairSnapshot? _latestSnapshot;

        internal RegularEnemyLocomotionSession(SimulationTick firstExpectedTick, PositionQ4096 walkerSpawn,
            bool walkerInitiallyGrounded, PositionQ4096 surveyorSpawn, bool surveyorInitiallyGrounded)
        {
            if (firstExpectedTick.Value < 0 || firstExpectedTick.Value == int.MaxValue) throw new ArgumentOutOfRangeException(nameof(firstExpectedTick));
            _nextExpectedTick = firstExpectedTick; _walkerSpawn = walkerSpawn; _surveyorSpawn = surveyorSpawn;
            _walkerGroundSeed = walkerInitiallyGrounded; _surveyorGroundSeed = surveyorInitiallyGrounded;
            _walker = Rest(WalkerId, walkerSpawn, walkerInitiallyGrounded, false);
            _surveyor = Rest(SurveyorId, surveyorSpawn, surveyorInitiallyGrounded, false);
        }

        internal SimulationTick NextExpectedTick => _nextExpectedTick;
        internal RegularEnemyMotionPairSnapshot? LatestSnapshot => _latestSnapshot;

        internal RegularEnemyMotionPairPrediction PreviewNext(RegularEnemyLocomotionPairInput input)
        {
            ValidateGameplayInput(input);
            var walker = PredictWalker(input.Tick, input.Behavior.Walker, input.WalkerReaction, _walker);
            var surveyor = PredictSurveyor(input.Tick, input.Behavior.Surveyor, input.SurveyorReaction, _surveyor);
            return new RegularEnemyMotionPairPrediction(input.Tick, walker, surveyor);
        }

        internal RegularEnemyResolvedPairCandidate ValidateResolvedNext(RegularEnemyLocomotionPairInput input,
            RegularEnemyCollisionPairResolution resolution)
        {
            var prediction = PreviewNext(input);
            var nextTick = new SimulationTick(checked(input.Tick.Value + 1));
            var walker = Resolve(prediction.Walker, resolution.Walker);
            var surveyor = Resolve(prediction.Surveyor, resolution.Surveyor);
            return new RegularEnemyResolvedPairCandidate(new RegularEnemyMotionPairSnapshot(input.Tick, walker, surveyor, nextTick));
        }

        internal RegularEnemyMotionPairSnapshot CommitResolvedNext(RegularEnemyLocomotionPairInput input,
            RegularEnemyCollisionPairResolution resolution, RegularEnemyResolvedPairCandidate candidate)
        {
            var recomputed = ValidateResolvedNext(input, resolution);
            if (!PairEquals(recomputed.Snapshot, candidate.Snapshot)) throw new InvalidOperationException("Resolved candidate does not match the exact recomputation.");
            var snapshot = recomputed.Snapshot;
            _walker = snapshot.Walker; _surveyor = snapshot.Surveyor;
            _walkerDead = snapshot.Walker.Dead; _surveyorDead = snapshot.Surveyor.Dead;
            _hasCompletedGameplay = true; _latestSnapshot = snapshot; _nextExpectedTick = snapshot.NextExpectedTick;
            return snapshot;
        }

        internal RegularEnemyResetCandidate PreflightReset(RegularEnemyLocomotionPairInput input)
        {
            ValidateResetInput(input);
            var next = new SimulationTick(checked(input.Tick.Value + 1));
            return new RegularEnemyResetCandidate(new RegularEnemyMotionPairSnapshot(input.Tick,
                Rest(WalkerId, _walkerSpawn, _walkerGroundSeed, false), Rest(SurveyorId, _surveyorSpawn, _surveyorGroundSeed, false), next));
        }

        internal RegularEnemyMotionPairSnapshot CommitReset(RegularEnemyLocomotionPairInput input, RegularEnemyResetCandidate candidate)
        {
            var recomputed = PreflightReset(input);
            if (!PairEquals(recomputed.Snapshot, candidate.Snapshot)) throw new InvalidOperationException("Reset candidate does not match the exact recomputation.");
            var snapshot = recomputed.Snapshot;
            _walker = snapshot.Walker; _surveyor = snapshot.Surveyor; _walkerDead = false; _surveyorDead = false;
            _hasCompletedGameplay = false; _latestSnapshot = snapshot; _nextExpectedTick = snapshot.NextExpectedTick;
            return snapshot;
        }

        private void ValidateGameplayInput(RegularEnemyLocomotionPairInput input)
        {
            ValidateCommon(input, RegularEnemyLocomotionInputKind.Gameplay);
            ValidateBehaviorPublication(input);
            ValidateReaction(input.WalkerReaction, WalkerId, input.Tick, input.Behavior.Walker.IsAlive, true);
            ValidateReaction(input.SurveyorReaction, SurveyorId, input.Tick, input.Behavior.Surveyor.IsAlive, false);
            ValidateWalkerBehavior(input.Behavior.Walker, input.WalkerReaction);
            ValidateSurveyorBehavior(input.Behavior.Surveyor, input.SurveyorReaction);
            if (_walkerDead && input.Behavior.Walker.IsAlive) throw new InvalidOperationException("Walker cannot resurrect before reset.");
            if (_surveyorDead && input.Behavior.Surveyor.IsAlive) throw new InvalidOperationException("Surveyor cannot resurrect before reset.");
        }

        private void ValidateResetInput(RegularEnemyLocomotionPairInput input)
        {
            ValidateCommon(input, RegularEnemyLocomotionInputKind.EncounterReset);
            if (!_hasCompletedGameplay) throw new InvalidOperationException("Reset requires a committed gameplay step.");
            ValidateBehaviorPublication(input);
            var walker = input.Behavior.Walker;
            var surveyor = input.Behavior.Surveyor;
            if (walker.Phase != WalkerBehaviorPhase.Reset || walker.ExclusiveEndTick != -1 || !walker.IsAlive ||
                walker.Movement != RegularEnemyMovementIntent.None || walker.Direction != 0 || walker.GuardRaised || walker.Telegraph ||
                walker.LatchedDashDirection != 0 || walker.AttackCycleOrdinal != 0 ||
                surveyor.Phase != SurveyorBehaviorPhase.Reset || surveyor.ExclusiveEndTick != -1 || !surveyor.IsAlive ||
                surveyor.Movement != RegularEnemyMovementIntent.None || surveyor.Direction != 0 || surveyor.Telegraph ||
                surveyor.HasLatchedTarget || surveyor.LatchedTargetXQ1000 != 0 || surveyor.ShotOrdinal != 0 ||
                input.Behavior.WalkerRecoveryIntent.HasValue || input.Behavior.SurveyorShotIntent.HasValue)
                throw new InvalidOperationException("Reset requires exact upstream reset behavior.");
            ValidateResetReaction(input.WalkerReaction, WalkerId, input.Tick);
            ValidateResetReaction(input.SurveyorReaction, SurveyorId, input.Tick);
        }

        private void ValidateCommon(RegularEnemyLocomotionPairInput input, RegularEnemyLocomotionInputKind expectedKind)
        {
            if (input.Kind != expectedKind) throw new ArgumentException("Wrong locomotion input discriminator.", nameof(input));
            if (input.Tick != _nextExpectedTick) throw new InvalidOperationException("Locomotion input must use the exact expected tick.");
            if (input.Tick.Value < 0 || input.Tick.Value == int.MaxValue) throw new ArgumentOutOfRangeException(nameof(input));
        }

        private static void ValidateBehaviorPublication(RegularEnemyLocomotionPairInput input)
        {
            if (input.Behavior.Tick != input.Tick || input.Behavior.NextExpectedTick.Value != checked(input.Tick.Value + 1))
                throw new ArgumentException("Behavior publication must be exact and consecutive.", nameof(input));
        }

        private static void ValidateReaction(RegularEnemyReactionSnapshot value, string role, SimulationTick tick, bool alive, bool walker)
        {
            if (!string.Equals(value.TargetId, role, StringComparison.Ordinal) || value.Tick != tick || value.NextExpectedTick.Value != checked(tick.Value + 1))
                throw new ArgumentException("Reaction publication identity or tick is invalid.", nameof(value));
            if (value.IsAlive != alive) throw new InvalidOperationException("Behavior and reaction alive values disagree.");
            if (value.LastTransferRevision < -1 || value.ShieldWindowEndTick < -1 || value.FireLockEndTick < -1) throw new ArgumentOutOfRangeException(nameof(value));
            if (value.IsHeavy && value.LastTransferRevision < 1) throw new InvalidOperationException("Heavy requires a positive transfer revision.");
            if (walker)
            {
                if (value.FireLockEndTick != -1 || value.ForcedDescent || value.CanFire || value.RecoveryMultiplierQ1000 != (value.IsAlive && value.IsHeavy ? 1500 : 1000))
                    throw new InvalidOperationException("Walker reaction structure is invalid.");
            }
            else if (value.ShieldWindowEndTick != -1 || value.ReactionAllowsBasicAttack || value.RecoveryMultiplierQ1000 != 1000 || value.ForcedDescent != (value.IsAlive && value.IsHeavy))
                throw new InvalidOperationException("Surveyor reaction structure is invalid.");
        }

        private static void ValidateResetReaction(RegularEnemyReactionSnapshot value, string role, SimulationTick tick)
        {
            ValidateReaction(value, role, tick, true, string.Equals(role, WalkerId, StringComparison.Ordinal));
            if (value.IsHeavy || value.LastTransferRevision != -1 || value.ShieldWindowEndTick != -1 || value.FireLockEndTick != -1 ||
                value.ReactionAllowsBasicAttack || value.RecoveryMultiplierQ1000 != 1000 || value.ForcedDescent || value.CanFire)
                throw new InvalidOperationException("Reset requires an exact baseline reaction publication.");
        }

        private static void ValidateWalkerBehavior(WalkerBehaviorSnapshot value, RegularEnemyReactionSnapshot reaction)
        {
            if (value.Phase < WalkerBehaviorPhase.Approach || value.Phase > WalkerBehaviorPhase.Dead) throw new ArgumentOutOfRangeException(nameof(value));
            if (value.Movement < RegularEnemyMovementIntent.None || value.Movement > RegularEnemyMovementIntent.Downward) throw new ArgumentOutOfRangeException(nameof(value));
            if (value.Phase == WalkerBehaviorPhase.Dead)
            {
                if (value.IsAlive || value.Movement != RegularEnemyMovementIntent.None || value.Direction != 0) throw new InvalidOperationException("Malformed dead walker behavior.");
                return;
            }
            if (!value.IsAlive || !reaction.IsAlive) throw new InvalidOperationException("Live walker phase requires alive publications.");
            switch (value.Phase)
            {
                case WalkerBehaviorPhase.Approach:
                    Require(value.Movement == RegularEnemyMovementIntent.Horizontal && IsDirection(value.Direction), "Malformed walker Approach."); break;
                case WalkerBehaviorPhase.DashTelegraph:
                    Require(value.Movement == RegularEnemyMovementIntent.None && value.Direction == 0 && IsDirection(value.LatchedDashDirection), "Malformed walker DashTelegraph."); break;
                case WalkerBehaviorPhase.DashActive:
                    Require(value.Movement == RegularEnemyMovementIntent.DashHorizontal && IsDirection(value.Direction) && value.Direction == value.LatchedDashDirection, "Malformed walker DashActive."); break;
                case WalkerBehaviorPhase.Recovery:
                    Require(value.Movement == RegularEnemyMovementIntent.None && value.Direction == 0, "Malformed walker Recovery."); break;
            }
        }

        private static void ValidateSurveyorBehavior(SurveyorBehaviorSnapshot value, RegularEnemyReactionSnapshot reaction)
        {
            if (value.Phase < SurveyorBehaviorPhase.Relocate || value.Phase > SurveyorBehaviorPhase.Dead) throw new ArgumentOutOfRangeException(nameof(value));
            if (value.Movement < RegularEnemyMovementIntent.None || value.Movement > RegularEnemyMovementIntent.Downward) throw new ArgumentOutOfRangeException(nameof(value));
            if (value.Phase == SurveyorBehaviorPhase.Dead)
            {
                if (value.IsAlive || value.Movement != RegularEnemyMovementIntent.None || value.Direction != 0) throw new InvalidOperationException("Malformed dead surveyor behavior.");
                return;
            }
            if (!value.IsAlive || !reaction.IsAlive) throw new InvalidOperationException("Live surveyor phase requires alive publications.");
            switch (value.Phase)
            {
                case SurveyorBehaviorPhase.Relocate:
                    Require(value.Movement == RegularEnemyMovementIntent.Horizontal && (value.Direction == -1 || value.Direction == 1) && !reaction.IsHeavy && !reaction.ForcedDescent && reaction.CanFire, "Malformed surveyor Relocate."); break;
                case SurveyorBehaviorPhase.Suppressed:
                    if (value.Movement == RegularEnemyMovementIntent.Downward)
                        Require(value.Direction == 0 && reaction.IsHeavy && reaction.ForcedDescent && !reaction.CanFire, "Malformed surveyor forced descent.");
                    else Require(value.Movement == RegularEnemyMovementIntent.None && value.Direction == 0 && !reaction.IsHeavy && !reaction.ForcedDescent && !reaction.CanFire, "Malformed surveyor residual lock.");
                    break;
                case SurveyorBehaviorPhase.FireTelegraph:
                case SurveyorBehaviorPhase.Fire:
                case SurveyorBehaviorPhase.Cooldown:
                    Require(value.Movement == RegularEnemyMovementIntent.None && value.Direction == 0 && !reaction.IsHeavy && !reaction.ForcedDescent && reaction.CanFire, "Malformed surveyor firing phase."); break;
                default: throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        private static RegularEnemyMotionPrediction PredictWalker(SimulationTick tick, WalkerBehaviorSnapshot behavior,
            RegularEnemyReactionSnapshot reaction, RegularEnemyMotionSnapshot prior)
        {
            if (!behavior.IsAlive) return IdentityPrediction(tick, prior);
            var horizontalRate = behavior.Movement == RegularEnemyMovementIntent.DashHorizontal ? DashRate : behavior.Movement == RegularEnemyMovementIntent.Horizontal ? ApproachRate : 0;
            if (reaction.IsHeavy && horizontalRate != 0)
            {
                horizontalRate = RoundDivAwayFromZero(checked((long)horizontalRate * 750), 1000);
                if (!prior.Grounded) horizontalRate = RoundDivAwayFromZero(checked((long)horizontalRate * 350), 1000);
            }
            var velocityX = checked(behavior.Direction * horizontalRate);
            var xRemainder = velocityX == 0 ? 0 : prior.XPositionRemainder;
            var velocityY = prior.Velocity.Y;
            var gravityRemainder = prior.GravityRemainder;
            var yRemainder = prior.YPositionRemainder;
            if (prior.Grounded) { velocityY = 0; gravityRemainder = 0; yRemainder = 0; }
            else ApplyGravity(reaction.IsHeavy, ref velocityY, ref gravityRemainder);
            return Integrate(tick, WalkerId, prior, velocityX, velocityY, xRemainder, yRemainder, gravityRemainder);
        }

        private static RegularEnemyMotionPrediction PredictSurveyor(SimulationTick tick, SurveyorBehaviorSnapshot behavior,
            RegularEnemyReactionSnapshot reaction, RegularEnemyMotionSnapshot prior)
        {
            if (!behavior.IsAlive) return IdentityPrediction(tick, prior);
            var velocityX = behavior.Movement == RegularEnemyMovementIntent.Horizontal ? checked(behavior.Direction * RelocateRate) : 0;
            var xRemainder = velocityX == 0 ? 0 : prior.XPositionRemainder;
            var velocityY = 0;
            long yRemainder = 0, gravityRemainder = 0;
            if (behavior.Movement == RegularEnemyMovementIntent.Downward && !prior.Grounded)
            {
                velocityY = prior.Velocity.Y; yRemainder = prior.YPositionRemainder; gravityRemainder = prior.GravityRemainder;
                ApplyGravity(true, ref velocityY, ref gravityRemainder);
            }
            return Integrate(tick, SurveyorId, prior, velocityX, velocityY, xRemainder, yRemainder, gravityRemainder);
        }

        private static void ApplyGravity(bool heavy, ref int velocityY, ref long remainder)
        {
            var rate = heavy ? RoundDivAwayFromZero(checked((long)GravityRate * 2200), 1000) : GravityRate;
            var numerator = checked(remainder - rate);
            var delta = numerator / 60;
            remainder = numerator % 60;
            velocityY = checked(velocityY + checked((int)delta));
            if (velocityY <= -MaximumFallRate) { velocityY = -MaximumFallRate; remainder = 0; }
        }

        private static RegularEnemyMotionPrediction Integrate(SimulationTick tick, string role, RegularEnemyMotionSnapshot prior,
            int velocityX, int velocityY, long xRemainder, long yRemainder, long gravityRemainder)
        {
            var xNumerator = checked(xRemainder + velocityX);
            var yNumerator = checked(yRemainder + velocityY);
            var xDelta = xNumerator / 60; var yDelta = yNumerator / 60;
            var position = new PositionQ4096(checked(prior.Position.X + checked((int)xDelta)), checked(prior.Position.Y + checked((int)yDelta)));
            return new RegularEnemyMotionPrediction(tick, role, true, prior.Position, prior.Velocity,
                new PositionQ4096(velocityX, velocityY), position, xNumerator % 60, yNumerator % 60, gravityRemainder, prior.Grounded);
        }

        private static RegularEnemyMotionPrediction IdentityPrediction(SimulationTick tick, RegularEnemyMotionSnapshot prior) =>
            new RegularEnemyMotionPrediction(tick, prior.RoleId, false, prior.Position, prior.Velocity, new PositionQ4096(0, 0), prior.Position, 0, 0, 0, prior.Grounded);

        private static RegularEnemyMotionSnapshot Resolve(RegularEnemyMotionPrediction prediction, RegularEnemyCollisionResolution resolution)
        {
            if (resolution.Tick != prediction.Tick || !string.Equals(resolution.RoleId, prediction.RoleId, StringComparison.Ordinal) ||
                !PositionEquals(resolution.StartPosition, prediction.StartPosition) || !PositionEquals(resolution.PredictedPosition, prediction.PredictedPosition))
                throw new ArgumentException("Collision resolution does not match prediction identity.", nameof(resolution));
            if (!prediction.Alive)
            {
                if (!PositionEquals(resolution.ResolvedPosition, prediction.StartPosition) || resolution.XBlocked || resolution.YBlocked ||
                    resolution.StationarySupportConfirmed || resolution.Grounded != prediction.PriorGrounded)
                    throw new InvalidOperationException("Dead role requires the exact identity resolution.");
                return Rest(prediction.RoleId, prediction.StartPosition, prediction.PriorGrounded, true);
            }
            ValidateAxis(prediction.StartPosition.X, prediction.PredictedPosition.X, resolution.ResolvedPosition.X, resolution.XBlocked);
            ValidateAxis(prediction.StartPosition.Y, prediction.PredictedPosition.Y, resolution.ResolvedPosition.Y, resolution.YBlocked);
            var downwardBlocked = prediction.PredictedPosition.Y < prediction.StartPosition.Y && resolution.YBlocked;
            if (resolution.StationarySupportConfirmed && (!prediction.PriorGrounded || prediction.PredictedPosition.Y != prediction.StartPosition.Y || resolution.YBlocked))
                throw new InvalidOperationException("Stationary support confirmation is structurally invalid.");
            if (resolution.Grounded != (downwardBlocked || resolution.StationarySupportConfirmed))
                throw new InvalidOperationException("Grounded must equal downward block or valid stationary support.");
            return new RegularEnemyMotionSnapshot(prediction.RoleId, resolution.ResolvedPosition,
                new PositionQ4096(resolution.XBlocked ? 0 : prediction.DesiredVelocity.X, resolution.YBlocked ? 0 : prediction.DesiredVelocity.Y),
                resolution.XBlocked ? 0 : prediction.XPositionRemainder, resolution.YBlocked ? 0 : prediction.YPositionRemainder,
                resolution.YBlocked ? 0 : prediction.GravityRemainder, resolution.Grounded, false);
        }

        private static void ValidateAxis(int start, int predicted, int resolved, bool blocked)
        {
            var predictedDelta = checked((long)predicted - start);
            var resolvedDelta = checked((long)resolved - start);
            if (predictedDelta == 0)
            {
                if (resolvedDelta != 0 || blocked) throw new InvalidOperationException("An unmoved axis cannot change or be blocked.");
                return;
            }
            if ((predictedDelta > 0 && resolvedDelta < 0) || (predictedDelta < 0 && resolvedDelta > 0) || Abs(resolvedDelta) > Abs(predictedDelta))
                throw new InvalidOperationException("Resolved travel is outside the prediction sweep.");
            if (!blocked && resolved != predicted) throw new InvalidOperationException("Unblocked axis must reach its prediction.");
            if (Abs(resolvedDelta) < Abs(predictedDelta) && !blocked) throw new InvalidOperationException("Truncated travel must be blocked.");
        }

        private static long Abs(long value) => value < 0 ? checked(-value) : value;
        private static int RoundDivAwayFromZero(long numerator, int denominator)
        {
            var half = denominator / 2;
            return checked((int)(numerator >= 0 ? checked(numerator + half) / denominator : checked(numerator - half) / denominator));
        }
        private static bool IsDirection(int value) => value >= -1 && value <= 1;
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
        private static RegularEnemyMotionSnapshot Rest(string role, PositionQ4096 position, bool grounded, bool dead) =>
            new RegularEnemyMotionSnapshot(role, position, new PositionQ4096(0, 0), 0, 0, 0, grounded, dead);
        private static bool PositionEquals(PositionQ4096 a, PositionQ4096 b) => a.X == b.X && a.Y == b.Y;
        private static bool MotionEquals(RegularEnemyMotionSnapshot a, RegularEnemyMotionSnapshot b) =>
            string.Equals(a.RoleId, b.RoleId, StringComparison.Ordinal) && PositionEquals(a.Position, b.Position) && PositionEquals(a.Velocity, b.Velocity) &&
            a.XPositionRemainder == b.XPositionRemainder && a.YPositionRemainder == b.YPositionRemainder && a.GravityRemainder == b.GravityRemainder &&
            a.Grounded == b.Grounded && a.Dead == b.Dead;
        private static bool PairEquals(RegularEnemyMotionPairSnapshot a, RegularEnemyMotionPairSnapshot b) =>
            a.Tick == b.Tick && a.NextExpectedTick == b.NextExpectedTick && MotionEquals(a.Walker, b.Walker) && MotionEquals(a.Surveyor, b.Surveyor);
    }
}
