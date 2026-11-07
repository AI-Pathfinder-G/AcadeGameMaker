using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat
{
    // REQ-COM-004. Internal immutable authored registration; no Unity discovery or instance identity participates.
    internal readonly struct CombatantRegistration
    {
        internal CombatantRegistration(string targetId, int maxHealth, int invulnerabilityTicks)
        {
            DamageValidation.Id(targetId, nameof(targetId));
            if (maxHealth < 1 || maxHealth > 999) throw new ArgumentOutOfRangeException(nameof(maxHealth));
            if (invulnerabilityTicks < 0 || invulnerabilityTicks > 600) throw new ArgumentOutOfRangeException(nameof(invulnerabilityTicks));
            TargetId = targetId;
            MaxHealth = maxHealth;
            InvulnerabilityTicks = invulnerabilityTicks;
        }

        internal string TargetId { get; }
        internal int MaxHealth { get; }
        internal int InvulnerabilityTicks { get; }
    }

    // REQ-COM-004. Test-only read model; it is intentionally not a public gameplay snapshot contract.
    internal readonly struct CombatantSnapshot
    {
        internal CombatantSnapshot(string targetId, int currentHealth, bool isDead, int invulnerabilityEndTick, int acceptedRequestCount)
        {
            TargetId = targetId;
            CurrentHealth = currentHealth;
            IsDead = isDead;
            InvulnerabilityEndTick = invulnerabilityEndTick;
            AcceptedRequestCount = acceptedRequestCount;
        }

        internal string TargetId { get; }
        internal int CurrentHealth { get; }
        internal bool IsDead { get; }
        internal int InvulnerabilityEndTick { get; }
        internal int AcceptedRequestCount { get; }
    }

    // REQ-COM-001, REQ-COM-004, REQ-COM-005, REQ-COM-006. Sole engine-free owner of health, dedupe, invulnerability, and one-time death state.
    internal sealed class CombatSession
    {
        private readonly Dictionary<string, Combatant> _combatants = new Dictionary<string, Combatant>(StringComparer.Ordinal);
        private readonly HashSet<string> _consumedRequestIds = new HashSet<string>(StringComparer.Ordinal);
        private SimulationTick _nextExpectedTick;
        private bool _hasCompletedProcessingTick;
        private bool _requiresRegistrationAfterReset;

        internal CombatSession(SimulationTick firstExpectedTick)
        {
            if (firstExpectedTick.Value < 0 || firstExpectedTick.Value > int.MaxValue - 1) throw new ArgumentOutOfRangeException(nameof(firstExpectedTick));
            _nextExpectedTick = firstExpectedTick;
        }

        internal SimulationTick NextExpectedTick => _nextExpectedTick;

        internal void Register(CombatantRegistration registration)
        {
            if (_combatants.ContainsKey(registration.TargetId)) throw new ArgumentException("Duplicate combatant target ID.", nameof(registration));
            _combatants.Add(registration.TargetId, new Combatant(registration));
            _requiresRegistrationAfterReset = false;
        }

        internal IReadOnlyList<DamageResult> Process(SimulationTick processingTick, IReadOnlyList<DamageRequest> requests)
        {
            if (requests == null) throw new ArgumentNullException(nameof(requests));
            if (processingTick.Value < 0 || processingTick != _nextExpectedTick) throw new InvalidOperationException("Combat ticks must be exactly the next nonnegative global simulation tick.");
            if (_requiresRegistrationAfterReset) throw new InvalidOperationException("An encounter reset requires explicit combatant re-registration before processing resumes.");

            var copied = new DamageRequest[requests.Count];
            for (var index = 0; index < copied.Length; index++)
            {
                var request = requests[index];
                if (request.Tick != processingTick) throw new ArgumentException("Every request must use the processing tick.", nameof(requests));
                copied[index] = request;
            }

            Array.Sort(copied, CompareRequests);
            var nextExpected = new SimulationTick(checked(processingTick.Value + 1));
            var invulnerabilityEnds = PreflightInvulnerabilityEnds(processingTick);

            var results = new DamageResult[copied.Length];
            for (var index = 0; index < copied.Length; index++)
            {
                var request = copied[index];
                if (!_consumedRequestIds.Add(request.RequestId))
                {
                    results[index] = Result(request, 0, DamageResultCode.Duplicate);
                    continue;
                }

                if (!_combatants.TryGetValue(request.TargetId, out var combatant))
                {
                    results[index] = Result(request, 0, DamageResultCode.Invalid);
                    continue;
                }

                if (combatant.IsDead)
                {
                    results[index] = Result(request, 0, DamageResultCode.TargetDead);
                    continue;
                }

                if (combatant.InvulnerabilityEndTick >= 0 && processingTick.Value < combatant.InvulnerabilityEndTick)
                {
                    results[index] = Result(request, 0, DamageResultCode.Invulnerable);
                    continue;
                }

                var appliedAmount = Math.Min(request.Amount, combatant.CurrentHealth);
                combatant.CurrentHealth -= appliedAmount;
                combatant.AcceptedRequestCount++;
                if (combatant.CurrentHealth == 0)
                {
                    combatant.IsDead = true;
                    combatant.InvulnerabilityEndTick = -1;
                }
                else if (combatant.InvulnerabilityTicks > 0)
                {
                    combatant.InvulnerabilityEndTick = invulnerabilityEnds[combatant.TargetId];
                }

                results[index] = Result(request, appliedAmount, DamageResultCode.Applied);
            }

            _nextExpectedTick = nextExpected;
            _hasCompletedProcessingTick = true;
            return results;
        }

        internal CombatantSnapshot Snapshot(string targetId)
        {
            DamageValidation.Id(targetId, nameof(targetId));
            if (!_combatants.TryGetValue(targetId, out var combatant)) throw new ArgumentException("Unknown registered combatant.", nameof(targetId));
            return new CombatantSnapshot(combatant.TargetId, combatant.CurrentHealth, combatant.IsDead, combatant.InvulnerabilityEndTick, combatant.AcceptedRequestCount);
        }

        internal void ResetEncounter()
        {
            if (!_hasCompletedProcessingTick) throw new InvalidOperationException("An encounter may reset only after a completed processing tick.");
            _combatants.Clear();
            _consumedRequestIds.Clear();
            _requiresRegistrationAfterReset = true;
        }

        private Dictionary<string, int> PreflightInvulnerabilityEnds(SimulationTick processingTick)
        {
            var values = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var pair in _combatants)
            {
                if (pair.Value.InvulnerabilityTicks > 0)
                {
                    values.Add(pair.Key, checked(processingTick.Value + pair.Value.InvulnerabilityTicks));
                }
            }

            return values;
        }

        private static int CompareRequests(DamageRequest left, DamageRequest right)
        {
            var requestId = string.CompareOrdinal(left.RequestId, right.RequestId);
            if (requestId != 0) return requestId;
            var sourceId = string.CompareOrdinal(left.SourceId, right.SourceId);
            if (sourceId != 0) return sourceId;
            var targetId = string.CompareOrdinal(left.TargetId, right.TargetId);
            if (targetId != 0) return targetId;
            var kind = left.Kind.CompareTo(right.Kind);
            return kind != 0 ? kind : left.Amount.CompareTo(right.Amount);
        }

        private static DamageResult Result(DamageRequest request, int appliedAmount, DamageResultCode result) => new DamageResult(request.RequestId, request.TargetId, appliedAmount, result, request.Tick);

        private sealed class Combatant
        {
            internal Combatant(CombatantRegistration registration)
            {
                TargetId = registration.TargetId;
                CurrentHealth = registration.MaxHealth;
                InvulnerabilityTicks = registration.InvulnerabilityTicks;
                InvulnerabilityEndTick = -1;
            }

            internal string TargetId { get; }
            internal int CurrentHealth { get; set; }
            internal int InvulnerabilityTicks { get; }
            internal int InvulnerabilityEndTick { get; set; }
            internal bool IsDead { get; set; }
            internal int AcceptedRequestCount { get; set; }
        }
    }
}
