using System;
using System.Globalization;
using System.Numerics;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat
{
    internal enum RegularEnemyThreatInputKind { Gameplay = 0, EncounterReset = 1 }

    internal readonly struct RegularEnemyThreatInput
    {
        private RegularEnemyThreatInput(RegularEnemyThreatInputKind kind, SimulationTick tick, bool playerFinalAlive,
            PositionQ4096 playerCenter, RegularEnemyBehaviorPairSnapshot behavior, RegularEnemyMotionPairSnapshot motion)
        {
            if (kind != RegularEnemyThreatInputKind.Gameplay && kind != RegularEnemyThreatInputKind.EncounterReset)
                throw new ArgumentOutOfRangeException(nameof(kind));
            if (tick.Value < 0) throw new ArgumentOutOfRangeException(nameof(tick));
            Kind = kind; Tick = tick; PlayerFinalAlive = playerFinalAlive; PlayerCenter = playerCenter;
            Behavior = behavior; Motion = motion;
        }

        internal static RegularEnemyThreatInput Gameplay(SimulationTick tick, bool playerFinalAlive,
            PositionQ4096 playerCenter, RegularEnemyBehaviorPairSnapshot behavior, RegularEnemyMotionPairSnapshot motion) =>
            new RegularEnemyThreatInput(RegularEnemyThreatInputKind.Gameplay, tick, playerFinalAlive, playerCenter, behavior, motion);

        internal static RegularEnemyThreatInput EncounterReset(SimulationTick tick, bool playerFinalAlive,
            RegularEnemyBehaviorPairSnapshot behavior, RegularEnemyMotionPairSnapshot motion) =>
            new RegularEnemyThreatInput(RegularEnemyThreatInputKind.EncounterReset, tick, playerFinalAlive, default, behavior, motion);

        internal RegularEnemyThreatInputKind Kind { get; }
        internal SimulationTick Tick { get; }
        internal bool PlayerFinalAlive { get; }
        internal PositionQ4096 PlayerCenter { get; }
        internal RegularEnemyBehaviorPairSnapshot Behavior { get; }
        internal RegularEnemyMotionPairSnapshot Motion { get; }
    }

    internal readonly struct RegularEnemyThreatSnapshot
    {
        internal RegularEnemyThreatSnapshot(SimulationTick tick, SimulationTick nextExpectedTick, bool playerDead,
            bool walkerDead, bool surveyorDead, bool hasPriorCenters, PositionQ4096 priorPlayerCenter,
            PositionQ4096 priorWalkerCenter, long expectedShotOrdinal, bool hasActiveLine, int lineCenterXQ4096,
            long lineShotOrdinal, int lineSpawnTick, int lineExclusiveEndTick)
        {
            Tick = tick; NextExpectedTick = nextExpectedTick; PlayerDead = playerDead; WalkerDead = walkerDead;
            SurveyorDead = surveyorDead; HasPriorCenters = hasPriorCenters; PriorPlayerCenter = priorPlayerCenter;
            PriorWalkerCenter = priorWalkerCenter; ExpectedShotOrdinal = expectedShotOrdinal; HasActiveLine = hasActiveLine;
            LineCenterXQ4096 = lineCenterXQ4096; LineShotOrdinal = lineShotOrdinal; LineSpawnTick = lineSpawnTick;
            LineExclusiveEndTick = lineExclusiveEndTick;
        }
        internal SimulationTick Tick { get; }
        internal SimulationTick NextExpectedTick { get; }
        internal bool PlayerDead { get; }
        internal bool WalkerDead { get; }
        internal bool SurveyorDead { get; }
        internal bool HasPriorCenters { get; }
        internal PositionQ4096 PriorPlayerCenter { get; }
        internal PositionQ4096 PriorWalkerCenter { get; }
        internal long ExpectedShotOrdinal { get; }
        internal bool HasActiveLine { get; }
        internal int LineCenterXQ4096 { get; }
        internal long LineShotOrdinal { get; }
        internal int LineSpawnTick { get; }
        internal int LineExclusiveEndTick { get; }
    }

    internal sealed class RegularEnemyThreatCandidate
    {
        private readonly DamageRequest[] _requests;
        internal RegularEnemyThreatCandidate(RegularEnemyThreatSnapshot snapshot, DamageRequest[] requests)
        {
            Snapshot = snapshot;
            _requests = requests == null ? throw new ArgumentNullException(nameof(requests)) : (DamageRequest[])requests.Clone();
        }
        internal RegularEnemyThreatSnapshot Snapshot { get; }
        internal int RequestCount => _requests.Length;
        internal DamageRequest GetRequest(int index) => _requests[index];
        internal DamageRequest[] CopyRequests() => (DamageRequest[])_requests.Clone();
    }

    // REQ-COM-001, REQ-COM-002, REQ-COM-004, REQ-COM-005. Engine-free, pair-atomic threat owner.
    internal sealed class RegularEnemyThreatSession
    {
        private const string PlayerId = "player";
        private const string WalkerId = "walker";
        private const string SurveyorId = "surveyor";
        private const int RequiredLineHalfWidth = 1024;
        private const int LineLifetimeTicks = 30;

        private readonly int _playerHalfWidth;
        private readonly int _playerHalfHeight;
        private readonly int _walkerHalfWidth;
        private readonly int _walkerHalfHeight;
        private readonly int _lineHalfWidth;
        private readonly long _expandedHalfWidth;
        private readonly long _expandedHalfHeight;
        private SimulationTick _nextExpectedTick;
        private bool _hasCompletedGameplay;
        private bool _playerDead;
        private bool _walkerDead;
        private bool _surveyorDead;
        private bool _hasPriorCenters;
        private PositionQ4096 _priorPlayerCenter;
        private PositionQ4096 _priorWalkerCenter;
        private long _expectedShotOrdinal;
        private bool _hasActiveLine;
        private int _lineCenterX;
        private long _lineShotOrdinal;
        private int _lineSpawnTick;
        private int _lineExclusiveEndTick;
        private RegularEnemyThreatSnapshot? _latestSnapshot;

        internal RegularEnemyThreatSession(SimulationTick firstExpectedTick, int playerHalfWidthQ4096,
            int playerHalfHeightQ4096, int walkerHalfWidthQ4096, int walkerHalfHeightQ4096,
            int denialLineHalfWidthQ4096)
        {
            if (firstExpectedTick.Value < 0 || firstExpectedTick.Value > int.MaxValue - 2)
                throw new ArgumentOutOfRangeException(nameof(firstExpectedTick));
            if (playerHalfWidthQ4096 <= 0 || playerHalfHeightQ4096 <= 0 || walkerHalfWidthQ4096 <= 0 || walkerHalfHeightQ4096 <= 0)
                throw new ArgumentOutOfRangeException("Half extents must be positive signed Q4096 values.");
            if (denialLineHalfWidthQ4096 != RequiredLineHalfWidth) throw new ArgumentOutOfRangeException(nameof(denialLineHalfWidthQ4096));
            _expandedHalfWidth = checked((long)playerHalfWidthQ4096 + walkerHalfWidthQ4096);
            _expandedHalfHeight = checked((long)playerHalfHeightQ4096 + walkerHalfHeightQ4096);
            if (_expandedHalfWidth <= 0 || _expandedHalfHeight <= 0) throw new ArgumentOutOfRangeException("Expanded extents must fit signed 64-bit values.");
            _nextExpectedTick = firstExpectedTick;
            _playerHalfWidth = playerHalfWidthQ4096; _playerHalfHeight = playerHalfHeightQ4096;
            _walkerHalfWidth = walkerHalfWidthQ4096; _walkerHalfHeight = walkerHalfHeightQ4096;
            _lineHalfWidth = denialLineHalfWidthQ4096;
        }

        internal SimulationTick NextExpectedTick => _nextExpectedTick;
        internal RegularEnemyThreatSnapshot? LatestSnapshot => _latestSnapshot;

        internal RegularEnemyThreatCandidate PreviewNext(RegularEnemyThreatInput input)
        {
            return input.Kind == RegularEnemyThreatInputKind.Gameplay ? PreviewGameplay(input) : PreviewReset(input);
        }

        internal RegularEnemyThreatSnapshot CommitNext(RegularEnemyThreatInput input, RegularEnemyThreatCandidate candidate)
        {
            if (candidate == null) throw new ArgumentNullException(nameof(candidate));
            var recomputed = PreviewNext(input);
            if (!CandidateEquals(recomputed, candidate)) throw new InvalidOperationException("Threat candidate does not match exact recomputation.");
            var snapshot = recomputed.Snapshot;
            _nextExpectedTick = snapshot.NextExpectedTick; _playerDead = snapshot.PlayerDead; _walkerDead = snapshot.WalkerDead;
            _surveyorDead = snapshot.SurveyorDead; _hasPriorCenters = snapshot.HasPriorCenters;
            _priorPlayerCenter = snapshot.PriorPlayerCenter; _priorWalkerCenter = snapshot.PriorWalkerCenter;
            _expectedShotOrdinal = snapshot.ExpectedShotOrdinal; _hasActiveLine = snapshot.HasActiveLine;
            _lineCenterX = snapshot.LineCenterXQ4096; _lineShotOrdinal = snapshot.LineShotOrdinal;
            _lineSpawnTick = snapshot.LineSpawnTick; _lineExclusiveEndTick = snapshot.LineExclusiveEndTick;
            _hasCompletedGameplay = input.Kind == RegularEnemyThreatInputKind.Gameplay;
            _latestSnapshot = snapshot;
            return snapshot;
        }

        private RegularEnemyThreatCandidate PreviewGameplay(RegularEnemyThreatInput input)
        {
            ValidateCommon(input, RegularEnemyThreatInputKind.Gameplay);
            ValidatePublications(input, false);
            var playerDead = _playerDead || !input.PlayerFinalAlive;
            var walkerDead = _walkerDead || !input.Behavior.Walker.IsAlive;
            var surveyorDead = _surveyorDead || !input.Behavior.Surveyor.IsAlive;
            if (_playerDead && input.PlayerFinalAlive) throw new InvalidOperationException("Player cannot resurrect before reset.");
            if (_walkerDead && input.Behavior.Walker.IsAlive) throw new InvalidOperationException("Walker cannot resurrect before reset.");
            if (_surveyorDead && input.Behavior.Surveyor.IsAlive) throw new InvalidOperationException("Surveyor cannot resurrect before reset.");

            var hasLine = _hasActiveLine;
            var lineCenter = _lineCenterX; var lineOrdinal = _lineShotOrdinal;
            var lineSpawn = _lineSpawnTick; var lineEnd = _lineExclusiveEndTick;
            if (hasLine && input.Tick.Value >= lineEnd) hasLine = false;

            var shot = input.Behavior.SurveyorShotIntent;
            var expectedOrdinal = _expectedShotOrdinal;
            ValidateShotPublication(input.Behavior.Surveyor, shot, expectedOrdinal);
            if (shot.HasValue)
            {
                if (hasLine) throw new InvalidOperationException("A new denial line cannot overlap an active line.");
                if (surveyorDead) throw new InvalidOperationException("A dead Surveyor cannot spawn a line.");
                lineCenter = ConvertQ1000ToQ4096(shot.Value.TargetXQ1000);
                lineOrdinal = shot.Value.ShotOrdinal;
                lineSpawn = input.Tick.Value;
                lineEnd = checked(input.Tick.Value + LineLifetimeTicks);
                expectedOrdinal = checked(expectedOrdinal + 1);
                hasLine = true;
            }

            var priorPlayer = _hasPriorCenters ? _priorPlayerCenter : input.PlayerCenter;
            var priorWalker = _hasPriorCenters ? _priorWalkerCenter : input.Motion.Walker.Position;
            var requests = new DamageRequest[2];
            var count = 0;
            var deliveryTick = new SimulationTick(checked(input.Tick.Value + 1));
            if (!playerDead && !walkerDead && SweptBoxesTouch(priorPlayer, input.PlayerCenter, priorWalker, input.Motion.Walker.Position))
                requests[count++] = CreateWalkerRequest(input.Tick.Value, deliveryTick);
            if (!playerDead && hasLine && SweptPlayerTouchesLine(priorPlayer.X, input.PlayerCenter.X, lineCenter))
                requests[count++] = CreateLineRequest(input.Tick.Value, lineOrdinal, deliveryTick);
            if (count != requests.Length) Array.Resize(ref requests, count);

            var snapshot = new RegularEnemyThreatSnapshot(input.Tick, new SimulationTick(checked(input.Tick.Value + 1)), playerDead,
                walkerDead, surveyorDead, true, input.PlayerCenter, input.Motion.Walker.Position, expectedOrdinal, hasLine,
                hasLine ? lineCenter : 0, hasLine ? lineOrdinal : 0, hasLine ? lineSpawn : -1, hasLine ? lineEnd : -1);
            return new RegularEnemyThreatCandidate(snapshot, requests);
        }

        private RegularEnemyThreatCandidate PreviewReset(RegularEnemyThreatInput input)
        {
            ValidateCommon(input, RegularEnemyThreatInputKind.EncounterReset);
            if (!_hasCompletedGameplay) throw new InvalidOperationException("Reset requires a committed gameplay step.");
            if (!input.PlayerFinalAlive) throw new InvalidOperationException("Reset requires an alive player.");
            ValidatePublications(input, true);
            var snapshot = new RegularEnemyThreatSnapshot(input.Tick, new SimulationTick(checked(input.Tick.Value + 1)), false,
                false, false, false, default, default, 0, false, 0, 0, -1, -1);
            return new RegularEnemyThreatCandidate(snapshot, Array.Empty<DamageRequest>());
        }

        private void ValidateCommon(RegularEnemyThreatInput input, RegularEnemyThreatInputKind kind)
        {
            if (input.Kind != kind) throw new ArgumentException("Wrong threat input discriminator.", nameof(input));
            if (input.Tick != _nextExpectedTick) throw new InvalidOperationException("Threat input must use the exact expected tick.");
            if (input.Tick.Value < 0 || input.Tick.Value > int.MaxValue - 2) throw new ArgumentOutOfRangeException(nameof(input));
        }

        private static void ValidatePublications(RegularEnemyThreatInput input, bool reset)
        {
            var next = checked(input.Tick.Value + 1);
            if (input.Behavior.Tick != input.Tick || input.Behavior.NextExpectedTick.Value != next ||
                input.Motion.Tick != input.Tick || input.Motion.NextExpectedTick.Value != next)
                throw new ArgumentException("Behavior and motion publications must be exact and consecutive.", nameof(input));
            if (!string.Equals(input.Motion.Walker.RoleId, WalkerId, StringComparison.Ordinal) ||
                !string.Equals(input.Motion.Surveyor.RoleId, SurveyorId, StringComparison.Ordinal))
                throw new ArgumentException("Motion roles must use literal ordering.", nameof(input));
            ValidatePhaseEnums(input.Behavior);
            ValidateMotionShape(input.Motion.Walker);
            ValidateMotionShape(input.Motion.Surveyor);
            if (input.Motion.Walker.Dead == input.Behavior.Walker.IsAlive || input.Motion.Surveyor.Dead == input.Behavior.Surveyor.IsAlive)
                throw new InvalidOperationException("Behavior and motion alive states disagree.");
            if (reset)
            {
                if (input.Behavior.Walker.Phase != WalkerBehaviorPhase.Reset || input.Behavior.Surveyor.Phase != SurveyorBehaviorPhase.Reset ||
                    !input.Behavior.Walker.IsAlive || !input.Behavior.Surveyor.IsAlive || input.Motion.Walker.Dead || input.Motion.Surveyor.Dead ||
                    input.Behavior.WalkerRecoveryIntent.HasValue || input.Behavior.SurveyorShotIntent.HasValue)
                    throw new InvalidOperationException("Reset requires exact paired reset publications.");
                ValidateResetShape(input.Behavior);
                if (!IsResting(input.Motion.Walker) || !IsResting(input.Motion.Surveyor))
                    throw new InvalidOperationException("Reset requires exact resting motion publications.");
            }
            else
            {
                ValidateGameplayShape(input.Tick, input.Behavior);
            }
        }

        private static void ValidatePhaseEnums(RegularEnemyBehaviorPairSnapshot behavior)
        {
            if (behavior.Walker.Phase < WalkerBehaviorPhase.Approach || behavior.Walker.Phase > WalkerBehaviorPhase.Reset ||
                behavior.Surveyor.Phase < SurveyorBehaviorPhase.Relocate || behavior.Surveyor.Phase > SurveyorBehaviorPhase.Reset)
                throw new ArgumentOutOfRangeException(nameof(behavior));
        }

        private static void ValidateResetShape(RegularEnemyBehaviorPairSnapshot behavior)
        {
            var w = behavior.Walker; var s = behavior.Surveyor;
            if (w.ExclusiveEndTick != -1 || w.Movement != RegularEnemyMovementIntent.None || w.Direction != 0 || w.GuardRaised || w.Telegraph ||
                w.LatchedDashDirection != 0 || w.AttackCycleOrdinal != 0 || s.ExclusiveEndTick != -1 ||
                s.Movement != RegularEnemyMovementIntent.None || s.Direction != 0 || s.Telegraph || s.HasLatchedTarget ||
                s.LatchedTargetXQ1000 != 0 || s.ShotOrdinal != 0)
                throw new InvalidOperationException("Malformed reset behavior publication.");
        }

        private static void ValidateGameplayShape(SimulationTick tick, RegularEnemyBehaviorPairSnapshot behavior)
        {
            var walker = behavior.Walker; var surveyor = behavior.Surveyor;
            if (walker.Phase == WalkerBehaviorPhase.Reset || surveyor.Phase == SurveyorBehaviorPhase.Reset)
                throw new InvalidOperationException("Gameplay cannot carry reset phases.");
            if (walker.Movement < RegularEnemyMovementIntent.None || walker.Movement > RegularEnemyMovementIntent.Downward ||
                surveyor.Movement < RegularEnemyMovementIntent.None || surveyor.Movement > RegularEnemyMovementIntent.Downward ||
                walker.AttackCycleOrdinal < 0 || surveyor.ShotOrdinal < 0)
                throw new ArgumentOutOfRangeException(nameof(behavior));
            if ((walker.Phase == WalkerBehaviorPhase.Dead) == walker.IsAlive || (surveyor.Phase == SurveyorBehaviorPhase.Dead) == surveyor.IsAlive)
                throw new InvalidOperationException("Alive state must agree with phase.");
            switch (walker.Phase)
            {
                case WalkerBehaviorPhase.Approach:
                    Require(walker.ExclusiveEndTick == -1 && walker.Movement == RegularEnemyMovementIntent.Horizontal && IsDirection(walker.Direction) &&
                        !walker.Telegraph && walker.LatchedDashDirection == 0, "Malformed Walker Approach."); break;
                case WalkerBehaviorPhase.DashTelegraph:
                    Require(walker.ExclusiveEndTick > tick.Value && walker.Movement == RegularEnemyMovementIntent.None && walker.Direction == 0 &&
                        walker.Telegraph && IsDirection(walker.LatchedDashDirection), "Malformed Walker DashTelegraph."); break;
                case WalkerBehaviorPhase.DashActive:
                    Require(walker.ExclusiveEndTick > tick.Value && walker.Movement == RegularEnemyMovementIntent.DashHorizontal &&
                        IsDirection(walker.Direction) && walker.Direction == walker.LatchedDashDirection && !walker.Telegraph,
                        "Malformed Walker DashActive."); break;
                case WalkerBehaviorPhase.Recovery:
                    Require(walker.ExclusiveEndTick > tick.Value && walker.Movement == RegularEnemyMovementIntent.None && walker.Direction == 0 &&
                        !walker.Telegraph && walker.LatchedDashDirection == 0, "Malformed Walker Recovery."); break;
                case WalkerBehaviorPhase.Dead:
                    Require(walker.ExclusiveEndTick == -1 && walker.Movement == RegularEnemyMovementIntent.None && walker.Direction == 0 &&
                        !walker.GuardRaised && !walker.Telegraph && walker.LatchedDashDirection == 0, "Malformed dead Walker publication."); break;
            }
            switch (surveyor.Phase)
            {
                case SurveyorBehaviorPhase.Relocate:
                    Require(surveyor.ExclusiveEndTick > tick.Value && surveyor.Movement == RegularEnemyMovementIntent.Horizontal &&
                        IsNonzeroDirection(surveyor.Direction) && !surveyor.Telegraph && !surveyor.HasLatchedTarget && surveyor.LatchedTargetXQ1000 == 0,
                        "Malformed Surveyor Relocate."); break;
                case SurveyorBehaviorPhase.FireTelegraph:
                    Require(surveyor.ExclusiveEndTick > tick.Value && surveyor.Movement == RegularEnemyMovementIntent.None && surveyor.Direction == 0 &&
                        surveyor.Telegraph && surveyor.HasLatchedTarget, "Malformed Surveyor FireTelegraph."); break;
                case SurveyorBehaviorPhase.Fire:
                    Require(surveyor.ExclusiveEndTick > tick.Value && surveyor.Movement == RegularEnemyMovementIntent.None && surveyor.Direction == 0 &&
                        !surveyor.Telegraph && surveyor.HasLatchedTarget, "Malformed Surveyor Fire."); break;
                case SurveyorBehaviorPhase.Cooldown:
                    Require(surveyor.ExclusiveEndTick > tick.Value && surveyor.Movement == RegularEnemyMovementIntent.None && surveyor.Direction == 0 &&
                        !surveyor.Telegraph && !surveyor.HasLatchedTarget && surveyor.LatchedTargetXQ1000 == 0, "Malformed Surveyor Cooldown."); break;
                case SurveyorBehaviorPhase.Suppressed:
                    Require(surveyor.ExclusiveEndTick == -1 && (surveyor.Movement == RegularEnemyMovementIntent.None || surveyor.Movement == RegularEnemyMovementIntent.Downward) &&
                        surveyor.Direction == 0 && !surveyor.Telegraph && !surveyor.HasLatchedTarget && surveyor.LatchedTargetXQ1000 == 0,
                        "Malformed Surveyor Suppressed."); break;
                case SurveyorBehaviorPhase.Dead:
                    Require(surveyor.ExclusiveEndTick == -1 && surveyor.Movement == RegularEnemyMovementIntent.None && surveyor.Direction == 0 &&
                        !surveyor.Telegraph && !surveyor.HasLatchedTarget && surveyor.LatchedTargetXQ1000 == 0, "Malformed dead Surveyor publication."); break;
            }
            var recoveryRequired = walker.IsAlive && walker.Phase == WalkerBehaviorPhase.DashActive && checked(tick.Value + 1) == walker.ExclusiveEndTick;
            if (behavior.WalkerRecoveryIntent.HasValue != recoveryRequired)
                throw new InvalidOperationException("Walker recovery intent must exist exactly on the final DashActive tick.");
            if (behavior.WalkerRecoveryIntent.HasValue)
            {
                var recovery = behavior.WalkerRecoveryIntent.Value;
                Require(walker.IsAlive && walker.Phase == WalkerBehaviorPhase.DashActive && tick.Value + 1 == walker.ExclusiveEndTick &&
                    string.Equals(recovery.TargetId, WalkerId, StringComparison.Ordinal) && recovery.ScheduledTick.Value == checked(tick.Value + 1) &&
                    recovery.AttackCycleOrdinal >= 0 && walker.AttackCycleOrdinal == checked(recovery.AttackCycleOrdinal + 1),
                    "Malformed Walker recovery intent publication.");
            }
        }

        private static void ValidateMotionShape(RegularEnemyMotionSnapshot motion)
        {
            if (motion.Dead && !IsResting(motion))
                throw new InvalidOperationException("Dead motion must be an exact resting snapshot.");
        }

        private static bool IsResting(RegularEnemyMotionSnapshot motion) => motion.Velocity.X == 0 && motion.Velocity.Y == 0 &&
            motion.XPositionRemainder == 0 && motion.YPositionRemainder == 0 && motion.GravityRemainder == 0;

        private static void ValidateShotPublication(SurveyorBehaviorSnapshot surveyor, SurveyorShotIntent? shot, long expectedOrdinal)
        {
            if (expectedOrdinal < 0) throw new InvalidOperationException("Expected shot ordinal is invalid.");
            var isFire = surveyor.IsAlive && surveyor.Phase == SurveyorBehaviorPhase.Fire;
            if (isFire != shot.HasValue) throw new InvalidOperationException("A shot intent exists exactly for alive Fire.");
            if (!shot.HasValue)
            {
                if (surveyor.ShotOrdinal != expectedOrdinal || surveyor.HasLatchedTarget && surveyor.Phase != SurveyorBehaviorPhase.FireTelegraph)
                    throw new InvalidOperationException("Non-Fire shot publication is inconsistent.");
                return;
            }
            var value = shot.Value;
            if (!string.Equals(value.TargetId, SurveyorId, StringComparison.Ordinal) || value.ShotOrdinal != expectedOrdinal || expectedOrdinal == long.MaxValue ||
                !surveyor.HasLatchedTarget || surveyor.LatchedTargetXQ1000 != value.TargetXQ1000 || surveyor.ShotOrdinal != checked(expectedOrdinal + 1))
                throw new InvalidOperationException("Fire shot publication is inconsistent.");
        }

        private bool SweptBoxesTouch(PositionQ4096 playerPrevious, PositionQ4096 playerCurrent,
            PositionQ4096 walkerPrevious, PositionQ4096 walkerCurrent)
        {
            var x0 = (long)playerPrevious.X - walkerPrevious.X; var y0 = (long)playerPrevious.Y - walkerPrevious.Y;
            var x1 = (long)playerCurrent.X - walkerCurrent.X; var y1 = (long)playerCurrent.Y - walkerCurrent.Y;
            var interval = RationalInterval.Unit;
            return ClipAxis(x0, x1, _expandedHalfWidth, ref interval) && ClipAxis(y0, y1, _expandedHalfHeight, ref interval);
        }

        private bool SweptPlayerTouchesLine(int previousX, int currentX, int centerX)
        {
            var sweptMin = Math.Min((long)previousX, currentX) - _playerHalfWidth;
            var sweptMax = Math.Max((long)previousX, currentX) + _playerHalfWidth;
            var lineMin = (long)centerX - _lineHalfWidth; var lineMax = (long)centerX + _lineHalfWidth;
            return sweptMax >= lineMin && sweptMin <= lineMax;
        }

        private static bool ClipAxis(long start, long end, long halfExtent, ref RationalInterval interval)
        {
            var delta = end - start;
            if (delta == 0) return start >= -halfExtent && start <= halfExtent;
            var first = Rational.Create(-halfExtent - start, delta);
            var second = Rational.Create(halfExtent - start, delta);
            if (Rational.Compare(first, second) > 0) { var swap = first; first = second; second = swap; }
            if (Rational.Compare(first, interval.Entry) > 0) interval.Entry = first;
            if (Rational.Compare(second, interval.Exit) < 0) interval.Exit = second;
            return Rational.Compare(interval.Entry, interval.Exit) <= 0;
        }

        private static int ConvertQ1000ToQ4096(int value)
        {
            var numerator = checked((long)value * 4096);
            var rounded = numerator >= 0 ? checked(numerator + 500) / 1000 : checked(numerator - 500) / 1000;
            return checked((int)rounded);
        }

        private static DamageRequest CreateWalkerRequest(int sourceTick, SimulationTick deliveryTick) =>
            new DamageRequest("enemy.contact/walker/" + sourceTick.ToString("D10", CultureInfo.InvariantCulture),
                WalkerId, PlayerId, 1, DamageKind.EnemyContact, deliveryTick);

        private static DamageRequest CreateLineRequest(int sourceTick, long ordinal, SimulationTick deliveryTick) =>
            new DamageRequest("enemy.line/surveyor/" + ordinal.ToString("D19", CultureInfo.InvariantCulture) + "/" +
                sourceTick.ToString("D10", CultureInfo.InvariantCulture), SurveyorId, PlayerId, 1, DamageKind.EnemyRanged, deliveryTick);

        private static bool CandidateEquals(RegularEnemyThreatCandidate left, RegularEnemyThreatCandidate right)
        {
            if (!SnapshotEquals(left.Snapshot, right.Snapshot) || left.RequestCount != right.RequestCount) return false;
            for (var i = 0; i < left.RequestCount; i++) if (!RequestEquals(left.GetRequest(i), right.GetRequest(i))) return false;
            return true;
        }

        private static bool SnapshotEquals(RegularEnemyThreatSnapshot a, RegularEnemyThreatSnapshot b) =>
            a.Tick == b.Tick && a.NextExpectedTick == b.NextExpectedTick && a.PlayerDead == b.PlayerDead && a.WalkerDead == b.WalkerDead &&
            a.SurveyorDead == b.SurveyorDead && a.HasPriorCenters == b.HasPriorCenters && PositionEquals(a.PriorPlayerCenter, b.PriorPlayerCenter) &&
            PositionEquals(a.PriorWalkerCenter, b.PriorWalkerCenter) && a.ExpectedShotOrdinal == b.ExpectedShotOrdinal &&
            a.HasActiveLine == b.HasActiveLine && a.LineCenterXQ4096 == b.LineCenterXQ4096 && a.LineShotOrdinal == b.LineShotOrdinal &&
            a.LineSpawnTick == b.LineSpawnTick && a.LineExclusiveEndTick == b.LineExclusiveEndTick;

        private static bool RequestEquals(DamageRequest a, DamageRequest b) => a.RequestId == b.RequestId && a.SourceId == b.SourceId &&
            a.TargetId == b.TargetId && a.Amount == b.Amount && a.Kind == b.Kind && a.Tick == b.Tick;
        private static bool PositionEquals(PositionQ4096 a, PositionQ4096 b) => a.X == b.X && a.Y == b.Y;
        private static bool IsDirection(int value) => value >= -1 && value <= 1;
        private static bool IsNonzeroDirection(int value) => value == -1 || value == 1;
        private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }

        private struct RationalInterval { internal Rational Entry; internal Rational Exit; internal static RationalInterval Unit => new RationalInterval { Entry = Rational.Zero, Exit = Rational.One }; }
        private readonly struct Rational
        {
            private Rational(BigInteger numerator, BigInteger denominator) { Numerator = numerator; Denominator = denominator; }
            internal BigInteger Numerator { get; }
            internal BigInteger Denominator { get; }
            internal static Rational Zero => new Rational(BigInteger.Zero, BigInteger.One);
            internal static Rational One => new Rational(BigInteger.One, BigInteger.One);
            internal static Rational Create(long numerator, long denominator)
            {
                var n = new BigInteger(numerator); var d = new BigInteger(denominator);
                if (d.Sign < 0) { n = BigInteger.Negate(n); d = BigInteger.Negate(d); }
                return new Rational(n, d);
            }
            internal static int Compare(Rational left, Rational right) =>
                (left.Numerator * right.Denominator).CompareTo(right.Numerator * left.Denominator);
        }
    }
}
