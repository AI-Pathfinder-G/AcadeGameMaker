using System;
using System.Collections.Generic;
using System.Globalization;
using AcadeGameMaker.Core;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-002, REQ-COM-003, REQ-COM-004; affected REQ-WT-003/005.
    // Engine-free -170 hostile contact owner. It queues only exact t+1 requests.
    [DefaultExecutionOrder(-170)]
    [DisallowMultipleComponent]
    internal sealed class OrdanBossHostileDamageProducer : MonoBehaviour
    {
        private const int PlayerHalfWidth = 1639;
        private const int PlayerHalfHeight = 3277;
        private const int ProjectileHalfExtent = 1024;
        private const int SourceX = 25395;
        private const int SourceY = 4915;
        private const int DebtEndX = -45056;
        private const int ArenaMinX = -47104;
        private const int ArenaMaxX = 47104;
        private const int ArenaMinY = 0;
        private const int ArenaMaxY = 40960;
        private const int OrdanCenterX = 28672;
        private const int OrdanCenterY = 4915;
        private const int OrdanHalfWidth = 3277;
        private const int OrdanHalfHeight = 4915;
        private const int DebtAHalfHeight = 1024;
        private const int DebtBY = 12288;
        private const int SeizureStartY = 24576;
        private const int SeizurePivotX = 8192;
        private const int SeizurePivotY = 12288;
        private const int SeizureEndX = -28672;
        private const int SeizureEndY = 4915;
        private const int AuditInnerRadius = 49152;
        private const int AuditOuterRadius = 65536;
        private static readonly int[] SeizureStartXs = { 12288, 20480, 28672 };
        private static readonly int[] NormalDurations = { 72, 66, 60 };
        private static readonly int[] AcceleratedDurations = { 24, 22, 20 };

        private static readonly ClosedAabbQ4096[] Occluders =
        {
            ClosedAabbQ4096.FromCenterHalf(0, -2048, 49152, 2048),
            ClosedAabbQ4096.FromCenterHalf(-49152, 18432, 2048, 22528),
            ClosedAabbQ4096.FromCenterHalf(49152, 18432, 2048, 22528)
        };

        [SerializeField] private PlayerMovementController _player;
        [SerializeField] private TransferSimulationDriver _transfer;
        [SerializeField] private OrdanBossCombatSimulationDriver _combat;
        [SerializeField] private OrdanBossSimulationDriver _bridge;
        [SerializeField] private OrdanBossTransferExposureScheduler _scheduler;

        private readonly HashSet<string> _latches = new HashSet<string>(StringComparer.Ordinal);
        private bool _terminal;
        private bool _hasLatest;
        private OrdanBossHostileDamageOutcome _latest;

        internal bool HasLatestOutcome => _hasLatest;
        internal OrdanBossHostileDamageOutcome LatestOutcome => _hasLatest
            ? _latest
            : throw new InvalidOperationException("Hostile producer has not published.");
        internal PlayerMovementController BoundPlayer => _player;
        internal TransferSimulationDriver BoundTransfer => _transfer;
        internal OrdanBossCombatSimulationDriver BoundCombat => _combat;
        internal OrdanBossSimulationDriver BoundBridge => _bridge;
        internal OrdanBossTransferExposureScheduler BoundScheduler => _scheduler;
        internal static int ApprovedPlayerHalfWidth => PlayerHalfWidth;
        internal static int ApprovedPlayerHalfHeight => PlayerHalfHeight;
        internal static int ApprovedProjectileHalfExtent => ProjectileHalfExtent;
        internal static int ApprovedSourceX => SourceX;
        internal static int ApprovedSourceY => SourceY;
        internal static int ApprovedDebtEndX => DebtEndX;
        internal static ClosedAabbQ4096 ApprovedArena => new ClosedAabbQ4096(ArenaMinX, ArenaMinY, ArenaMaxX, ArenaMaxY);
        internal static ClosedAabbQ4096 ApprovedOrdanBody => ClosedAabbQ4096.FromCenterHalf(OrdanCenterX, OrdanCenterY, OrdanHalfWidth, OrdanHalfHeight);
        internal static ClosedAabbQ4096 ApprovedDebtLineA => new ClosedAabbQ4096(DebtEndX, SourceY - DebtAHalfHeight, SourceX, SourceY + DebtAHalfHeight);
        internal static ClosedAabbQ4096 ApprovedDebtLineB => new ClosedAabbQ4096(DebtEndX, DebtBY - DebtAHalfHeight, SourceX, DebtBY + DebtAHalfHeight);
        internal static IReadOnlyList<int> ApprovedSeizureStartXs => Array.AsReadOnly((int[])SeizureStartXs.Clone());
        internal static int ApprovedSeizureStartY => SeizureStartY;
        internal static int ApprovedSeizurePivotX => SeizurePivotX;
        internal static int ApprovedSeizurePivotY => SeizurePivotY;
        internal static int ApprovedSeizureEndX => SeizureEndX;
        internal static int ApprovedSeizureEndY => SeizureEndY;
        internal static int ApprovedNormalPathPointCount => 3;
        internal static int ApprovedAcceleratedPathPointCount => 2;
        internal static IReadOnlyList<int> ApprovedNormalDurations => Array.AsReadOnly((int[])NormalDurations.Clone());
        internal static IReadOnlyList<int> ApprovedAcceleratedDurations => Array.AsReadOnly((int[])AcceleratedDurations.Clone());
        internal static int ApprovedAuditCenterX => OrdanCenterX;
        internal static int ApprovedAuditCenterY => OrdanCenterY;
        internal static int ApprovedAuditInnerRadius => AuditInnerRadius;
        internal static int ApprovedAuditOuterRadius => AuditOuterRadius;
        internal static IReadOnlyList<ClosedAabbQ4096> ApprovedOccluders => Array.AsReadOnly((ClosedAabbQ4096[])Occluders.Clone());

        internal void ConfigureForAuthoring(PlayerMovementController player, TransferSimulationDriver transfer,
            OrdanBossCombatSimulationDriver combat, OrdanBossSimulationDriver bridge,
            OrdanBossTransferExposureScheduler scheduler)
        {
            if (_hasLatest) throw new InvalidOperationException("Hostile producer cannot be reconfigured after publication.");
            _player = player;
            _transfer = transfer;
            _combat = combat;
            _bridge = bridge;
            _scheduler = scheduler;
        }

        private void FixedUpdate()
        {
            // Production remains quiescent until M4B3C tears down the graph;
            // the explicit test seam still rejects advances after terminal.
            if (!_terminal) Advance();
        }
        internal OrdanBossHostileDamageOutcome AdvanceForTests() => Advance();

        private OrdanBossHostileDamageOutcome Advance()
        {
            if (_terminal) throw new InvalidOperationException("Terminal hostile producer cannot advance.");
            ValidateBindingsAndPublications(out var tick, out var player, out var combatants);

            if (combatants[0].IsDead)
            {
                var terminal = OrdanBossHostileDamageDigest.Suppressed(tick, OrdanBossHostileDisposition.SuppressedBossDead, true);
                var outcome = BuildOutcome(tick, terminal, OrdanBossHostileGeometryObservation.None);
                _terminal = true;
                return Publish(outcome);
            }
            if (combatants[1].IsDead)
                return Publish(BuildOutcome(tick, OrdanBossHostileDamageDigest.Suppressed(tick, OrdanBossHostileDisposition.SuppressedPlayerDead, false), OrdanBossHostileGeometryObservation.None));
            if (_transfer.LatestPhasePublication.Value.Input.LifecycleReason.HasValue)
                return Publish(BuildOutcome(tick, OrdanBossHostileDamageDigest.Suppressed(tick, OrdanBossHostileDisposition.SuppressedLifecycle, false), OrdanBossHostileGeometryObservation.None));

            var attack = ReadExactAttack(_bridge.LatestOutcome);
            if (!attack.HasValue)
                return Publish(BuildOutcome(tick, OrdanBossHostileDamageDigest.NoAttack(tick), OrdanBossHostileGeometryObservation.None));

            var value = attack.Value;
            var latchKey = BuildLatchKey(value);
            if (_latches.Contains(latchKey))
            {
                var observation = EvaluateGeometry(value, player);
                return Publish(BuildOutcome(tick, OrdanBossHostileDamageDigest.ForAttack(tick,
                    OrdanBossHostileDisposition.AlreadyLatched, value), observation));
            }

            var geometry = EvaluateGeometry(value, player);
            if (!geometry.Contact)
                return Publish(BuildOutcome(tick, OrdanBossHostileDamageDigest.ForAttack(tick,
                    OrdanBossHostileDisposition.NoContact, value), geometry));
            if (geometry.Occluded)
                return Publish(BuildOutcome(tick, OrdanBossHostileDamageDigest.ForAttack(tick,
                    OrdanBossHostileDisposition.Occluded, value), geometry));

            var delivery = new SimulationTick(checked(tick.Value + 1));
            var request = new DamageRequest(FormatRequestId(value, delivery), "ordan", "player", 1,
                DamageKind.BossPattern, delivery);
            var candidate = _combat.PreflightHostileDeliveryAppend(this, tick, new[] { request });
            var digest = OrdanBossHostileDamageDigest.Queued(tick, value, request);
            var queued = BuildOutcome(tick, digest, geometry);
            _combat.CommitPreflightedHostileDeliveryAppend(candidate);
            if (!_latches.Add(latchKey)) throw new InvalidOperationException("Hostile latch commit was duplicated.");
            return Publish(queued);
        }

        private void ValidateBindingsAndPublications(out SimulationTick tick, out PlayerMotionSnapshot player,
            out IReadOnlyList<CombatantSnapshot> combatants)
        {
            if (_player == null || _transfer == null || _combat == null || _bridge == null || _scheduler == null ||
                _combat.BoundHostileProducer != this || _combat.BoundPlayer != _player || _combat.BoundTransferDriver != _transfer ||
                _bridge.BoundTransferDriver != _transfer || _scheduler.BoundBridge != _bridge || _scheduler.BoundTransferDriver != _transfer ||
                !_bridge.HasLatestOutcome || !_combat.HasLatestOutcome || !_scheduler.HasLatestOutcome ||
                !_scheduler.HasLatestAuditOutcome || !_transfer.LatestPhasePublication.HasValue)
                throw new InvalidOperationException("Hostile bindings or publications are missing or cross-wired.");

            tick = _bridge.LatestOutcome.SourceTick;
            player = _player.Snapshot;
            var firstTick = _player.Chapter1ConfiguredFirstTick ?? new SimulationTick(0);
            if (tick == firstTick)
            {
                if (player.Tick != firstTick || _player.NextExpectedTick != firstTick)
                    throw new InvalidOperationException("Hostile bootstrap snapshot is invalid.");
            }
            else if (player.Tick.Value != checked(tick.Value - 1) || _player.NextExpectedTick != tick)
                throw new InvalidOperationException("Hostile prior movement snapshot is invalid.");

            if (_combat.LatestOutcome.Tick != tick || _scheduler.LatestOutcome.SourceTick != tick ||
                _scheduler.LatestAuditOutcome.SourceTick != tick || _transfer.LatestPhasePublication.Value.Tick != tick)
                throw new InvalidOperationException("Hostile publication tick is stale or skipped.");
            combatants = _combat.LatestOutcome.Combatants;
            if (combatants.Count != 2 || combatants[0].TargetId != "ordan" || combatants[1].TargetId != "player")
                throw new InvalidOperationException("Hostile Combat roster is not exact.");
        }

        private static OrdanHostileAttack? ReadExactAttack(OrdanBossBridgeOutcome bridge)
        {
            OrdanHostileAttack? found = null;
            for (var index = 0; index < bridge.Output.Intents.Count; index++)
            {
                var intent = bridge.Output.Intents[index];
                if (intent.Kind != OrdanBossIntentKind.DebtLine && intent.Kind != OrdanBossIntentKind.BalanceAuditShockwave) continue;
                OrdanHostileAttack candidate;
                if (intent.Kind == OrdanBossIntentKind.DebtLine)
                {
                    if (intent.PatternSlot != OrdanPatternSlot.DebtLineA && intent.PatternSlot != OrdanPatternSlot.DebtLineB)
                        throw new InvalidOperationException("Debt intent slot is invalid.");
                    var key = intent.PatternSlot == OrdanPatternSlot.DebtLineA ? "DebtLineA" : "DebtLineB";
                    candidate = new OrdanHostileAttack(key, ParseExactOrdinal(intent.Id, "ordan.debt/"), string.Empty, null);
                }
                else
                {
                    if (intent.PatternSlot != OrdanPatternSlot.BalanceAudit)
                        throw new InvalidOperationException("Audit intent slot is invalid.");
                    candidate = new OrdanHostileAttack("Audit", ParseExactOrdinal(intent.Id, "ordan.audit-shockwave/"), string.Empty, null);
                }
                if (found.HasValue) throw new InvalidOperationException("A tick cannot contain two hostile attacks.");
                found = candidate;
            }

            if (bridge.SeizureTrajectoryProjection.HasValue)
            {
                if (found.HasValue) throw new InvalidOperationException("Seizure projection conflicts with a one-shot hostile intent.");
                var trajectory = bridge.SeizureTrajectoryProjection.Value;
                found = new OrdanHostileAttack("Seizure", trajectory.AttackOrdinal, trajectory.PayloadId, trajectory);
            }
            return found;
        }

        private static long ParseExactOrdinal(string id, string prefix)
        {
            if (id == null || !id.StartsWith(prefix, StringComparison.Ordinal) || id.Length != prefix.Length + 19)
                throw new InvalidOperationException("Hostile intent ID shape is invalid.");
            long value = 0;
            for (var index = prefix.Length; index < id.Length; index++)
            {
                var digit = id[index];
                if (digit < '0' || digit > '9') throw new InvalidOperationException("Hostile intent ordinal is not ASCII decimal.");
                value = checked(value * 10 + digit - '0');
            }
            return value;
        }

        // AC-M4B3B2-001/004. Closed integer geometry and rational segment clipping.
        private static OrdanBossHostileGeometryObservation EvaluateGeometry(OrdanHostileAttack attack,
            PlayerMotionSnapshot player)
        {
            var hurtbox = ClosedAabbQ4096.FromCenterHalf(player.PositionXQ4096, player.PositionYQ4096,
                PlayerHalfWidth, PlayerHalfHeight);
            if (attack.GeometryKey == "DebtLineA")
                return EvaluateSegment(attack, SourceX, SourceY, DebtEndX, SourceY, 0, DebtAHalfHeight, hurtbox);
            if (attack.GeometryKey == "DebtLineB")
                return EvaluateSegment(attack, SourceX, DebtBY, DebtEndX, DebtBY, 0, DebtAHalfHeight, hurtbox);
            if (attack.GeometryKey == "Audit")
            {
                var nearX = DistanceToClosedInterval(OrdanCenterX, hurtbox.MinX, hurtbox.MaxX);
                var nearY = DistanceToClosedInterval(OrdanCenterY, hurtbox.MinY, hurtbox.MaxY);
                var farX = Math.Max(Math.Abs(checked((long)OrdanCenterX - hurtbox.MinX)), Math.Abs(checked((long)OrdanCenterX - hurtbox.MaxX)));
                var farY = Math.Max(Math.Abs(checked((long)OrdanCenterY - hurtbox.MinY)), Math.Abs(checked((long)OrdanCenterY - hurtbox.MaxY)));
                var nearSquared = checked(checked(nearX * nearX) + checked(nearY * nearY));
                var farSquared = checked(checked(farX * farX) + checked(farY * farY));
                var contact = nearSquared <= checked((long)AuditOuterRadius * AuditOuterRadius) &&
                    farSquared >= checked((long)AuditInnerRadius * AuditInnerRadius);
                return new OrdanBossHostileGeometryObservation(attack.GeometryKey, contact, false,
                    OrdanCenterX, OrdanCenterY, OrdanCenterX, OrdanCenterY, -1);
            }
            if (attack.GeometryKey != "Seizure" || !attack.Trajectory.HasValue)
                throw new InvalidOperationException("Hostile geometry key is invalid.");
            ProjectSeizureSegment(attack.Trajectory.Value, out var startX, out var startY, out var endX, out var endY);
            return EvaluateSegment(attack, startX, startY, endX, endY, ProjectileHalfExtent,
                ProjectileHalfExtent, hurtbox);
        }
        internal static OrdanBossHostileGeometryObservation EvaluateGeometryForTests(OrdanHostileAttack attack,
            PlayerMotionSnapshot player) => EvaluateGeometry(attack, player);

        private static OrdanBossHostileGeometryObservation EvaluateSegment(OrdanHostileAttack attack,
            int startX, int startY, int endX, int endY, int expandX, int expandY, ClosedAabbQ4096 hurtbox)
            => EvaluateSegmentWithOccluders(attack, startX, startY, endX, endY, expandX, expandY, hurtbox, Occluders);

        private static OrdanBossHostileGeometryObservation EvaluateSegmentWithOccluders(OrdanHostileAttack attack,
            int startX, int startY, int endX, int endY, int expandX, int expandY, ClosedAabbQ4096 hurtbox,
            IReadOnlyList<ClosedAabbQ4096> occluders)
        {
            if (occluders == null) throw new ArgumentNullException(nameof(occluders));
            var expandedPlayer = hurtbox.Expand(expandX, expandY);
            if (!TrySegmentEntry(startX, startY, endX, endY, expandedPlayer, out var playerEntry))
                return new OrdanBossHostileGeometryObservation(attack.GeometryKey, false, false,
                    startX, startY, endX, endY, -1);
            for (var index = 0; index < occluders.Count; index++)
            {
                var blocker = occluders[index].Expand(expandX, expandY);
                if (TrySegmentEntry(startX, startY, endX, endY, blocker, out var blockerEntry) &&
                    RationalQ.Compare(blockerEntry, playerEntry) <= 0)
                    return new OrdanBossHostileGeometryObservation(attack.GeometryKey, true, true,
                        startX, startY, endX, endY, index);
            }
            return new OrdanBossHostileGeometryObservation(attack.GeometryKey, true, false,
                startX, startY, endX, endY, -1);
        }
        internal static OrdanBossHostileGeometryObservation EvaluateSegmentForTests(OrdanHostileAttack attack,
            int startX, int startY, int endX, int endY, int expandX, int expandY, ClosedAabbQ4096 hurtbox,
            IReadOnlyList<ClosedAabbQ4096> occluders) =>
            EvaluateSegmentWithOccluders(attack, startX, startY, endX, endY, expandX, expandY, hurtbox, occluders);

        private static void ProjectSeizureSegment(SeizureTrajectoryProjection trajectory,
            out int startX, out int startY, out int endX, out int endY)
        {
            var payloadX = trajectory.PayloadId == "BossWeight-0" ? SeizureStartXs[0] :
                trajectory.PayloadId == "BossWeight-1" ? SeizureStartXs[1] : SeizureStartXs[2];
            PointOnTrajectory(trajectory, Math.Max(0, trajectory.ElapsedTick - 1), payloadX, out startX, out startY);
            PointOnTrajectory(trajectory, trajectory.ElapsedTick, payloadX, out endX, out endY);
        }

        private static void PointOnTrajectory(SeizureTrajectoryProjection trajectory, int elapsed, int payloadX,
            out int x, out int y)
        {
            if (trajectory.Accelerated)
            {
                x = Interpolate(payloadX, SeizureEndX, elapsed, checked(trajectory.FrozenDuration - 1));
                y = Interpolate(SeizureStartY, SeizureEndY, elapsed, checked(trajectory.FrozenDuration - 1));
                return;
            }
            var pivot = checked((trajectory.FrozenDuration - 1) / 2);
            if (elapsed <= pivot)
            {
                x = Interpolate(payloadX, SeizurePivotX, elapsed, pivot);
                y = Interpolate(SeizureStartY, SeizurePivotY, elapsed, pivot);
            }
            else
            {
                var local = checked(elapsed - pivot);
                var duration = checked(trajectory.FrozenDuration - 1 - pivot);
                x = Interpolate(SeizurePivotX, SeizureEndX, local, duration);
                y = Interpolate(SeizurePivotY, SeizureEndY, local, duration);
            }
        }

        private static int Interpolate(int from, int to, int elapsed, int denominator)
        {
            if (elapsed < 0 || denominator <= 0 || elapsed > denominator) throw new ArgumentOutOfRangeException(nameof(elapsed));
            var numerator = checked(checked((long)to - from) * elapsed);
            var quotient = numerator / denominator;
            var remainder = numerator % denominator;
            var doubled = checked(Math.Abs(remainder) * 2);
            if (doubled >= denominator) quotient = checked(quotient + (numerator < 0 ? -1 : 1));
            return checked(from + checked((int)quotient));
        }

        private static bool TrySegmentEntry(int x0, int y0, int x1, int y1, ClosedAabbQ4096 box,
            out RationalQ entry)
        {
            var interval = RationalIntervalQ.Unit;
            if (!ClipAxis(x0, x1, box.MinX, box.MaxX, ref interval) ||
                !ClipAxis(y0, y1, box.MinY, box.MaxY, ref interval))
            {
                entry = default;
                return false;
            }
            entry = interval.Entry;
            return true;
        }

        private static bool ClipAxis(int start, int end, int minimum, int maximum, ref RationalIntervalQ interval)
        {
            var delta = checked((long)end - start);
            if (delta == 0) return start >= minimum && start <= maximum;
            var first = RationalQ.Create(checked((long)minimum - start), delta);
            var second = RationalQ.Create(checked((long)maximum - start), delta);
            if (RationalQ.Compare(first, second) > 0) { var swap = first; first = second; second = swap; }
            if (RationalQ.Compare(first, interval.Entry) > 0) interval.Entry = first;
            if (RationalQ.Compare(second, interval.Exit) < 0) interval.Exit = second;
            return RationalQ.Compare(interval.Entry, interval.Exit) <= 0;
        }

        private static long DistanceToClosedInterval(int value, int minimum, int maximum) =>
            value < minimum ? checked((long)minimum - value) : value > maximum ? checked((long)value - maximum) : 0;

        private static string BuildLatchKey(OrdanHostileAttack attack)
        {
            if (attack.GeometryKey == "Seizure")
            {
                var projection = attack.Trajectory.Value;
                return string.Format(CultureInfo.InvariantCulture, "Seizure/{0:D19}/{1}/{2:D3}/{3}",
                    attack.AttackOrdinal, attack.PayloadId, projection.FrozenDuration, projection.Accelerated ? 1 : 0);
            }
            return string.Format(CultureInfo.InvariantCulture, "{0}/{1:D19}", attack.GeometryKey, attack.AttackOrdinal);
        }

        internal static string FormatRequestId(OrdanHostileAttack attack, SimulationTick delivery)
        {
            string result;
            if (attack.GeometryKey == "Seizure")
                result = string.Format(CultureInfo.InvariantCulture, "ordan.hostile/seizure/{0:D19}/{1}/{2:D10}",
                    attack.AttackOrdinal, attack.PayloadId, delivery.Value);
            else
            {
                var stem = attack.GeometryKey == "DebtLineA" ? "debt-a" :
                    attack.GeometryKey == "DebtLineB" ? "debt-b" : attack.GeometryKey == "Audit" ? "audit" :
                    throw new InvalidOperationException("Hostile geometry key is invalid.");
                result = string.Format(CultureInfo.InvariantCulture, "ordan.hostile/{0}/{1:D19}/{2:D10}",
                    stem, attack.AttackOrdinal, delivery.Value);
            }
            if (result.Length > 96) throw new OverflowException("Hostile request ID exceeds its contract bound.");
            return result;
        }
        internal static string FormatRequestIdForTests(OrdanHostileAttack attack, SimulationTick delivery) => FormatRequestId(attack, delivery);

        private static OrdanBossHostileDamageOutcome BuildOutcome(SimulationTick tick,
            OrdanBossHostileDamageDigest digest, OrdanBossHostileGeometryObservation observation) =>
            new OrdanBossHostileDamageOutcome(tick, digest, observation);

        private OrdanBossHostileDamageOutcome Publish(OrdanBossHostileDamageOutcome outcome)
        {
            _latest = outcome;
            _hasLatest = true;
            return outcome;
        }

        internal readonly struct ClosedAabbQ4096 : IEquatable<ClosedAabbQ4096>
        {
            internal ClosedAabbQ4096(int minX, int minY, int maxX, int maxY)
            {
                if (minX > maxX || minY > maxY) throw new ArgumentException("Closed AABB bounds are inverted.");
                MinX = minX; MinY = minY; MaxX = maxX; MaxY = maxY;
            }
            internal int MinX { get; } internal int MinY { get; } internal int MaxX { get; } internal int MaxY { get; }
            internal static ClosedAabbQ4096 FromCenterHalf(int x, int y, int halfX, int halfY)
            {
                if (halfX < 0 || halfY < 0) throw new ArgumentOutOfRangeException(nameof(halfX));
                return new ClosedAabbQ4096(checked(x - halfX), checked(y - halfY), checked(x + halfX), checked(y + halfY));
            }
            internal ClosedAabbQ4096 Expand(int x, int y)
            {
                if (x < 0 || y < 0) throw new ArgumentOutOfRangeException(nameof(x));
                return new ClosedAabbQ4096(checked(MinX - x), checked(MinY - y), checked(MaxX + x), checked(MaxY + y));
            }
            public bool Equals(ClosedAabbQ4096 other) => MinX == other.MinX && MinY == other.MinY && MaxX == other.MaxX && MaxY == other.MaxY;
        }

        private readonly struct RationalQ
        {
            private RationalQ(long numerator, long denominator) { Numerator = numerator; Denominator = denominator; }
            internal long Numerator { get; } internal long Denominator { get; }
            internal static RationalQ Zero => new RationalQ(0, 1);
            internal static RationalQ One => new RationalQ(1, 1);
            internal static RationalQ Create(long numerator, long denominator)
            {
                if (denominator == 0) throw new DivideByZeroException();
                if (denominator < 0) { numerator = checked(-numerator); denominator = checked(-denominator); }
                return new RationalQ(numerator, denominator);
            }
            internal static int Compare(RationalQ left, RationalQ right) =>
                checked(left.Numerator * right.Denominator).CompareTo(checked(right.Numerator * left.Denominator));
        }

        private struct RationalIntervalQ
        {
            internal RationalQ Entry;
            internal RationalQ Exit;
            internal static RationalIntervalQ Unit => new RationalIntervalQ { Entry = RationalQ.Zero, Exit = RationalQ.One };
        }
    }

    internal enum OrdanBossHostileDisposition
    {
        NoAttack = 0, NoContact = 1, Occluded = 2, AlreadyLatched = 3, Queued = 4,
        SuppressedBossDead = 5, SuppressedPlayerDead = 6, SuppressedLifecycle = 7
    }

    internal readonly struct OrdanHostileAttack
    {
        internal OrdanHostileAttack(string geometryKey, long attackOrdinal, string payloadId,
            SeizureTrajectoryProjection? trajectory)
        {
            if (geometryKey != "DebtLineA" && geometryKey != "DebtLineB" && geometryKey != "Seizure" && geometryKey != "Audit" || attackOrdinal < 0)
                throw new ArgumentException("Hostile attack identity is invalid.");
            payloadId = payloadId ?? string.Empty;
            if ((geometryKey == "Seizure") != trajectory.HasValue ||
                (geometryKey == "Seizure" && (payloadId.Length == 0 || trajectory.Value.AttackOrdinal != attackOrdinal || trajectory.Value.PayloadId != payloadId)) ||
                (geometryKey != "Seizure" && payloadId.Length != 0))
                throw new ArgumentException("Hostile attack projection is inconsistent.");
            GeometryKey = geometryKey; AttackOrdinal = attackOrdinal; PayloadId = payloadId; Trajectory = trajectory;
        }
        internal string GeometryKey { get; } internal long AttackOrdinal { get; } internal string PayloadId { get; }
        internal SeizureTrajectoryProjection? Trajectory { get; }
    }

    internal readonly struct OrdanBossHostileDamageDigest : IEquatable<OrdanBossHostileDamageDigest>
    {
        private OrdanBossHostileDamageDigest(SimulationTick tick, bool terminal,
            OrdanBossHostileDisposition disposition, string geometryKey, long attackOrdinal, string payloadId,
            string requestId, SimulationTick? deliveryTick)
        {
            if (disposition < OrdanBossHostileDisposition.NoAttack || disposition > OrdanBossHostileDisposition.SuppressedLifecycle)
                throw new ArgumentOutOfRangeException(nameof(disposition));
            geometryKey = geometryKey ?? string.Empty; payloadId = payloadId ?? string.Empty; requestId = requestId ?? string.Empty;
            var attackDisposition = disposition == OrdanBossHostileDisposition.NoContact || disposition == OrdanBossHostileDisposition.Occluded ||
                disposition == OrdanBossHostileDisposition.AlreadyLatched || disposition == OrdanBossHostileDisposition.Queued;
            var queued = disposition == OrdanBossHostileDisposition.Queued;
            if (terminal != (disposition == OrdanBossHostileDisposition.SuppressedBossDead) ||
                attackDisposition != (geometryKey.Length != 0 && attackOrdinal >= 0) ||
                (!attackDisposition && (payloadId.Length != 0 || requestId.Length != 0 || deliveryTick.HasValue)) ||
                queued != (requestId.Length != 0 && deliveryTick.HasValue) ||
                (geometryKey == "Seizure") != (payloadId.Length != 0) ||
                (attackDisposition && geometryKey != "DebtLineA" && geometryKey != "DebtLineB" && geometryKey != "Seizure" && geometryKey != "Audit") ||
                (queued && deliveryTick.Value.Value != checked(tick.Value + 1)))
                throw new ArgumentException("Hostile digest field combination is invalid.");
            Tick = tick; Terminal = terminal; Disposition = disposition; GeometryKey = geometryKey;
            AttackOrdinal = attackOrdinal; PayloadId = payloadId; RequestId = requestId; DeliveryTick = deliveryTick;
        }
        internal SimulationTick Tick { get; } internal bool Terminal { get; } internal OrdanBossHostileDisposition Disposition { get; }
        internal string GeometryKey { get; } internal long AttackOrdinal { get; } internal string PayloadId { get; }
        internal string RequestId { get; } internal SimulationTick? DeliveryTick { get; }
        internal static OrdanBossHostileDamageDigest NoAttack(SimulationTick tick) =>
            new OrdanBossHostileDamageDigest(tick, false, OrdanBossHostileDisposition.NoAttack, string.Empty, -1, string.Empty, string.Empty, null);
        internal static OrdanBossHostileDamageDigest Suppressed(SimulationTick tick, OrdanBossHostileDisposition disposition, bool terminal) =>
            new OrdanBossHostileDamageDigest(tick, terminal, disposition, string.Empty, -1, string.Empty, string.Empty, null);
        internal static OrdanBossHostileDamageDigest ForAttack(SimulationTick tick, OrdanBossHostileDisposition disposition, OrdanHostileAttack attack) =>
            new OrdanBossHostileDamageDigest(tick, false, disposition, attack.GeometryKey, attack.AttackOrdinal, attack.PayloadId, string.Empty, null);
        internal static OrdanBossHostileDamageDigest Queued(SimulationTick tick, OrdanHostileAttack attack, DamageRequest request)
        {
            if (request.SourceId != "ordan" || request.TargetId != "player" || request.Amount != 1 ||
                request.Kind != DamageKind.BossPattern || request.Tick.Value != checked(tick.Value + 1) ||
                request.RequestId != OrdanBossHostileDamageProducer.FormatRequestId(attack, request.Tick))
                throw new ArgumentException("Queued hostile request does not match its source attack.");
            return new OrdanBossHostileDamageDigest(tick, false, OrdanBossHostileDisposition.Queued,
                attack.GeometryKey, attack.AttackOrdinal, attack.PayloadId, request.RequestId, request.Tick);
        }
        public bool Equals(OrdanBossHostileDamageDigest other) => Tick == other.Tick && Terminal == other.Terminal && Disposition == other.Disposition &&
            GeometryKey == other.GeometryKey && AttackOrdinal == other.AttackOrdinal && PayloadId == other.PayloadId && RequestId == other.RequestId && DeliveryTick == other.DeliveryTick;
        public override bool Equals(object obj) => obj is OrdanBossHostileDamageDigest other && Equals(other);
        public override int GetHashCode() => Tick.Value ^ (int)Disposition;
    }

    internal readonly struct OrdanBossHostileGeometryObservation : IEquatable<OrdanBossHostileGeometryObservation>
    {
        internal OrdanBossHostileGeometryObservation(string geometryKey, bool contact, bool occluded,
            int startX, int startY, int endX, int endY, int occluderIndex)
        {
            if (occluded && !contact || occluderIndex < -1 || occluded != (occluderIndex >= 0)) throw new ArgumentException();
            GeometryKey = geometryKey ?? string.Empty; Contact = contact; Occluded = occluded;
            StartX = startX; StartY = startY; EndX = endX; EndY = endY; OccluderIndex = occluderIndex;
        }
        internal string GeometryKey { get; } internal bool Contact { get; } internal bool Occluded { get; }
        internal int StartX { get; } internal int StartY { get; } internal int EndX { get; } internal int EndY { get; }
        internal int OccluderIndex { get; }
        internal static OrdanBossHostileGeometryObservation None =>
            new OrdanBossHostileGeometryObservation(string.Empty, false, false, 0, 0, 0, 0, -1);
        public bool Equals(OrdanBossHostileGeometryObservation other) => GeometryKey == other.GeometryKey && Contact == other.Contact &&
            Occluded == other.Occluded && StartX == other.StartX && StartY == other.StartY && EndX == other.EndX && EndY == other.EndY && OccluderIndex == other.OccluderIndex;
    }

    internal readonly struct OrdanBossHostileDamageOutcome
    {
        internal OrdanBossHostileDamageOutcome(SimulationTick tick, OrdanBossHostileDamageDigest digest,
            OrdanBossHostileGeometryObservation observation)
        {
            if (digest.Tick != tick) throw new ArgumentException("Hostile outcome tick is inconsistent.");
            Tick = tick; Digest = digest; Observation = observation;
        }
        internal SimulationTick Tick { get; } internal OrdanBossHostileDamageDigest Digest { get; }
        internal OrdanBossHostileGeometryObservation Observation { get; }
    }
}
