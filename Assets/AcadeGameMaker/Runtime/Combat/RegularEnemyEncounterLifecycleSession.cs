using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat
{
    // REQ-COM-001, REQ-COM-002, REQ-COM-004, REQ-COM-005.  M3E1's engine-free
    // lifecycle owner deliberately consumes value contracts only.  It neither
    // observes Unity state nor owns M1 health, Transfer registration, rewards,
    // rooms, or scene lifetime.
    internal enum RegularEnemyEncounterLifecycleInputKind { Gameplay = 0, EncounterReset = 1 }

    internal readonly struct RegularEnemyEncounterRoleState
    {
        internal RegularEnemyEncounterRoleState(string targetId, int currentHealth, bool isDead)
        {
            DamageValidation.Id(targetId, nameof(targetId));
            if (currentHealth < 0 || currentHealth > MaxHealth(targetId) || isDead != (currentHealth == 0))
                throw new ArgumentException("Role health and death state are not canonical.");
            TargetId = targetId;
            CurrentHealth = currentHealth;
            IsDead = isDead;
        }

        internal string TargetId { get; }
        internal int CurrentHealth { get; }
        internal bool IsDead { get; }

        internal static int MaxHealth(string targetId)
        {
            if (targetId == "player") return 5;
            if (targetId == "surveyor") return 6;
            if (targetId == "walker") return 9;
            throw new ArgumentException("M3E1 has exactly player, surveyor, walker roles.", nameof(targetId));
        }
    }

    internal readonly struct RegularEnemyEncounterLifecycleInput
    {
        internal RegularEnemyEncounterLifecycleInput(SimulationTick tick, RegularEnemyEncounterLifecycleInputKind kind,
            IReadOnlyList<DamageRequest> canonicalRequestTrace, IReadOnlyList<DamageResult> damageResults,
            IReadOnlyList<RegularEnemyEncounterRoleState> roles)
        {
            if (canonicalRequestTrace == null || damageResults == null || roles == null) throw new ArgumentNullException();
            if (kind != RegularEnemyEncounterLifecycleInputKind.Gameplay && kind != RegularEnemyEncounterLifecycleInputKind.EncounterReset)
                throw new ArgumentOutOfRangeException(nameof(kind));
            if (canonicalRequestTrace.Count != damageResults.Count || roles.Count != 3) throw new ArgumentException("M3E1 input shape is not exact.");
            var requests = new DamageRequest[canonicalRequestTrace.Count];
            var results = new DamageResult[damageResults.Count];
            for (var i = 0; i < requests.Length; i++) { requests[i] = canonicalRequestTrace[i]; results[i] = damageResults[i]; }
            var states = new RegularEnemyEncounterRoleState[3];
            for (var i = 0; i < states.Length; i++) states[i] = roles[i];
            ValidateRoleShape(states);
            Tick = tick; Kind = kind; CanonicalRequestTrace = Freeze(requests); DamageResults = Freeze(results); Roles = Freeze(states);
        }

        internal SimulationTick Tick { get; }
        internal RegularEnemyEncounterLifecycleInputKind Kind { get; }
        internal ReadOnlyCollection<DamageRequest> CanonicalRequestTrace { get; }
        internal ReadOnlyCollection<DamageResult> DamageResults { get; }
        internal ReadOnlyCollection<RegularEnemyEncounterRoleState> Roles { get; }
        private static ReadOnlyCollection<T> Freeze<T>(T[] values) => new ReadOnlyCollection<T>((T[])values.Clone());
        private static void ValidateRoleShape(RegularEnemyEncounterRoleState[] states)
        {
            var expected = new[] { "player", "surveyor", "walker" };
            for (var i = 0; i < expected.Length; i++) if (states[i].TargetId != expected[i]) throw new ArgumentException("M3E1 input roster order is exact.");
        }
    }

    internal readonly struct RegularEnemyDefeatedNotice { internal RegularEnemyDefeatedNotice(string targetId, SimulationTick tick) { TargetId = targetId; Tick = tick; } internal string TargetId { get; } internal SimulationTick Tick { get; } }
    internal readonly struct RegularEnemyRewardRequest { internal RegularEnemyRewardRequest(string targetId, SimulationTick tick) { TargetId = targetId; Tick = tick; } internal string TargetId { get; } internal SimulationTick Tick { get; } }
    internal readonly struct RegularEnemyEncounterClearedNotice { internal RegularEnemyEncounterClearedNotice(SimulationTick tick) { Tick = tick; } internal SimulationTick Tick { get; } }
    internal readonly struct RegularEnemyRoomCompletionRequest { internal RegularEnemyRoomCompletionRequest(SimulationTick tick) { Tick = tick; } internal SimulationTick Tick { get; } }

    internal readonly struct RegularEnemyEncounterLifecycleSnapshot
    {
        internal RegularEnemyEncounterLifecycleSnapshot(SimulationTick tick, RegularEnemyEncounterLifecycleInputKind kind,
            IReadOnlyList<RegularEnemyEncounterRoleState> roles, SimulationTick nextExpectedTick)
        { Tick = tick; Kind = kind; Roles = Freeze(new[] { roles[0], roles[1], roles[2] }); NextExpectedTick = nextExpectedTick; }
        internal SimulationTick Tick { get; }
        internal RegularEnemyEncounterLifecycleInputKind Kind { get; }
        internal ReadOnlyCollection<RegularEnemyEncounterRoleState> Roles { get; }
        internal SimulationTick NextExpectedTick { get; }
        private static ReadOnlyCollection<T> Freeze<T>(T[] values) => new ReadOnlyCollection<T>((T[])values.Clone());
    }

    internal sealed class RegularEnemyEncounterLifecyclePreview
    {
        internal RegularEnemyEncounterLifecyclePreview(object owner, RegularEnemyEncounterLifecycleSnapshot snapshot,
            bool surveyorIgnoreCollision, bool walkerIgnoreCollision, string[] appliedHitTargetIds,
            RegularEnemyDefeatedNotice[] defeats, RegularEnemyRewardRequest[] rewards,
            RegularEnemyEncounterClearedNotice? cleared, RegularEnemyRoomCompletionRequest? completion)
        {
            _owner = owner; Snapshot = snapshot; SurveyorIgnoreCollision = surveyorIgnoreCollision; WalkerIgnoreCollision = walkerIgnoreCollision;
            AppliedHitTargetIds = Freeze(appliedHitTargetIds); Defeats = Freeze(defeats);
            Rewards = Freeze(rewards); Cleared = cleared; Completion = completion;
        }
        private readonly object _owner;
        internal bool IsOwnedBy(object owner) => ReferenceEquals(_owner, owner);
        internal RegularEnemyEncounterLifecycleSnapshot Snapshot { get; }
        internal bool SurveyorIgnoreCollision { get; }
        internal bool WalkerIgnoreCollision { get; }
        internal ReadOnlyCollection<string> AppliedHitTargetIds { get; }
        internal ReadOnlyCollection<RegularEnemyDefeatedNotice> Defeats { get; }
        internal ReadOnlyCollection<RegularEnemyRewardRequest> Rewards { get; }
        internal RegularEnemyEncounterClearedNotice? Cleared { get; }
        internal RegularEnemyRoomCompletionRequest? Completion { get; }
        private static ReadOnlyCollection<T> Freeze<T>(T[] values) => new ReadOnlyCollection<T>((T[])values.Clone());
    }

    internal sealed class RegularEnemyEncounterLifecycleSession
    {
        private readonly object _owner = new object();
        private SimulationTick _nextExpectedTick;
        private readonly int[] _health = { 5, 6, 9 }; // player, surveyor, walker: literal ordinal contract.
        private readonly bool[] _dead = new bool[3];
        private readonly bool[] _defeated = new bool[2]; // surveyor, walker
        private bool _cleared;
        private RegularEnemyEncounterLifecycleSnapshot? _latest;

        internal RegularEnemyEncounterLifecycleSession(SimulationTick firstExpectedTick)
        {
            if (firstExpectedTick.Value < 0 || firstExpectedTick.Value == int.MaxValue) throw new ArgumentOutOfRangeException(nameof(firstExpectedTick));
            _nextExpectedTick = firstExpectedTick;
        }

        internal SimulationTick NextExpectedTick => _nextExpectedTick;
        internal RegularEnemyEncounterLifecycleSnapshot? LatestSnapshot => _latest;

        internal RegularEnemyEncounterLifecyclePreview PreviewNext(RegularEnemyEncounterLifecycleInput input)
        {
            if (input.Tick != _nextExpectedTick) throw new InvalidOperationException("Encounter lifecycle requires the exact next global tick.");
            var roles = ExactRoles(input.Roles);
            if (input.Kind == RegularEnemyEncounterLifecycleInputKind.EncounterReset)
                return PreviewReset(input, roles);
            if (input.Kind != RegularEnemyEncounterLifecycleInputKind.Gameplay) throw new ArgumentOutOfRangeException(nameof(input));
            ValidateGameplayTrace(input, roles);

            var hits = new List<string>(3);
            for (var role = 0; role < 3; role++)
                if (HasAppliedFor(input.DamageResults, Roles[role])) hits.Add(Roles[role]);
            var defeats = new List<RegularEnemyDefeatedNotice>(2);
            var rewards = new List<RegularEnemyRewardRequest>(2);
            for (var enemy = 0; enemy < 2; enemy++)
            {
                var role = enemy + 1;
                if (!_dead[role] && roles[role].IsDead && !_defeated[enemy])
                { defeats.Add(new RegularEnemyDefeatedNotice(Roles[role], input.Tick)); rewards.Add(new RegularEnemyRewardRequest(Roles[role], input.Tick)); }
            }
            var allEnemiesDead = roles[1].IsDead && roles[2].IsDead;
            var cleared = allEnemiesDead && !_cleared ? new RegularEnemyEncounterClearedNotice?(new RegularEnemyEncounterClearedNotice(input.Tick)) : null;
            var completion = allEnemiesDead && !_cleared ? new RegularEnemyRoomCompletionRequest?(new RegularEnemyRoomCompletionRequest(input.Tick)) : null;
            return NewPreview(input, roles, hits.ToArray(), defeats.ToArray(), rewards.ToArray(), cleared, completion);
        }

        // REQ-COM-004, REQ-COM-005. Commit receives the exact pure input again
        // and recomputes the candidate.  A caller cannot turn an old, forged, or
        // collection-mutated preview into a state transition.
        internal void CommitNext(RegularEnemyEncounterLifecycleInput input, RegularEnemyEncounterLifecyclePreview preview)
        {
            if (preview == null || !preview.IsOwnedBy(_owner) || preview.Snapshot.Tick != _nextExpectedTick)
                throw new InvalidOperationException("Encounter lifecycle preview is forged or stale.");
            var committed = PreviewNext(input);
            if (!SamePreview(committed, preview))
                throw new InvalidOperationException("Encounter lifecycle preview does not match its exact input.");
            var roles = ExactRoles(committed.Snapshot.Roles);
            // Rechecking transition ownership here makes the state replacement atomic even for an internal caller.
            if (committed.Snapshot.Kind == RegularEnemyEncounterLifecycleInputKind.EncounterReset)
            {
                if (_dead[1] || _dead[2] || _defeated[0] || _defeated[1] || _cleared) throw new InvalidOperationException("Post-death same-instance reset is forbidden.");
                _health[0] = 5; _health[1] = 6; _health[2] = 9;
                _dead[0] = _dead[1] = _dead[2] = false;
            }
            else
            {
                for (var i = 0; i < 3; i++) { _health[i] = roles[i].CurrentHealth; _dead[i] = roles[i].IsDead; }
                for (var i = 0; i < committed.Defeats.Count; i++) _defeated[RoleIndex(committed.Defeats[i].TargetId) - 1] = true;
                if (committed.Cleared.HasValue) _cleared = true;
            }
            _latest = committed.Snapshot;
            _nextExpectedTick = committed.Snapshot.NextExpectedTick;
        }

        private static bool SamePreview(RegularEnemyEncounterLifecyclePreview left, RegularEnemyEncounterLifecyclePreview right)
        {
            if (left.Snapshot.Tick != right.Snapshot.Tick || left.Snapshot.Kind != right.Snapshot.Kind ||
                left.Snapshot.NextExpectedTick != right.Snapshot.NextExpectedTick ||
                left.SurveyorIgnoreCollision != right.SurveyorIgnoreCollision || left.WalkerIgnoreCollision != right.WalkerIgnoreCollision ||
                left.Cleared.HasValue != right.Cleared.HasValue || left.Completion.HasValue != right.Completion.HasValue ||
                !SameRoles(left.Snapshot.Roles, right.Snapshot.Roles) || !SameItems(left.AppliedHitTargetIds, right.AppliedHitTargetIds) ||
                !SameItems(left.Defeats, right.Defeats) || !SameItems(left.Rewards, right.Rewards)) return false;
            return (!left.Cleared.HasValue || left.Cleared.Value.Tick == right.Cleared.Value.Tick) &&
                (!left.Completion.HasValue || left.Completion.Value.Tick == right.Completion.Value.Tick);
        }

        private static bool SameRoles(IReadOnlyList<RegularEnemyEncounterRoleState> left, IReadOnlyList<RegularEnemyEncounterRoleState> right)
        {
            if (left.Count != right.Count) return false;
            for (var i = 0; i < left.Count; i++)
                if (left[i].TargetId != right[i].TargetId || left[i].CurrentHealth != right[i].CurrentHealth || left[i].IsDead != right[i].IsDead) return false;
            return true;
        }

        private static bool SameItems(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            if (left.Count != right.Count) return false;
            for (var i = 0; i < left.Count; i++) if (left[i] != right[i]) return false;
            return true;
        }

        private static bool SameItems(IReadOnlyList<RegularEnemyDefeatedNotice> left, IReadOnlyList<RegularEnemyDefeatedNotice> right)
        {
            if (left.Count != right.Count) return false;
            for (var i = 0; i < left.Count; i++) if (left[i].TargetId != right[i].TargetId || left[i].Tick != right[i].Tick) return false;
            return true;
        }

        private static bool SameItems(IReadOnlyList<RegularEnemyRewardRequest> left, IReadOnlyList<RegularEnemyRewardRequest> right)
        {
            if (left.Count != right.Count) return false;
            for (var i = 0; i < left.Count; i++) if (left[i].TargetId != right[i].TargetId || left[i].Tick != right[i].Tick) return false;
            return true;
        }

        private RegularEnemyEncounterLifecyclePreview PreviewReset(RegularEnemyEncounterLifecycleInput input, RegularEnemyEncounterRoleState[] roles)
        {
            if (_dead[1] || _dead[2] || _defeated[0] || _defeated[1] || _cleared || input.CanonicalRequestTrace.Count != 0 || input.DamageResults.Count != 0)
                throw new InvalidOperationException("M3E1 reset is a pre-death empty lifecycle clear only.");
            for (var i = 0; i < 3; i++) if (roles[i].CurrentHealth != new[] { 5, 6, 9 }[i] || roles[i].IsDead) throw new InvalidOperationException("Reset must carry exact full alive roles.");
            return NewPreview(input, roles, Array.Empty<string>(), Array.Empty<RegularEnemyDefeatedNotice>(), Array.Empty<RegularEnemyRewardRequest>(), null, null);
        }

        private RegularEnemyEncounterLifecyclePreview NewPreview(RegularEnemyEncounterLifecycleInput input, RegularEnemyEncounterRoleState[] roles,
            string[] hits, RegularEnemyDefeatedNotice[] defeats, RegularEnemyRewardRequest[] rewards,
            RegularEnemyEncounterClearedNotice? cleared, RegularEnemyRoomCompletionRequest? completion)
        {
            var next = new SimulationTick(checked(input.Tick.Value + 1));
            var snapshot = new RegularEnemyEncounterLifecycleSnapshot(input.Tick, input.Kind, roles, next);
            return new RegularEnemyEncounterLifecyclePreview(_owner, snapshot, roles[1].IsDead, roles[2].IsDead, hits, defeats, rewards, cleared, completion);
        }

        private void ValidateGameplayTrace(RegularEnemyEncounterLifecycleInput input, RegularEnemyEncounterRoleState[] roles)
        {
            var applied = new int[3];
            for (var i = 0; i < input.CanonicalRequestTrace.Count; i++)
            {
                var request = input.CanonicalRequestTrace[i]; var result = input.DamageResults[i];
                if (request.Tick != input.Tick || result.Tick != input.Tick || request.RequestId != result.RequestId || request.TargetId != result.TargetId)
                    throw new InvalidOperationException("Canonical M1 trace/result alignment is invalid.");
                if (result.Result == DamageResultCode.Applied)
                {
                    var index = RoleIndex(result.TargetId);
                    if (result.AppliedAmount < 0 || result.AppliedAmount > request.Amount) throw new InvalidOperationException("Applied damage exceeds its canonical request.");
                    applied[index] = checked(applied[index] + result.AppliedAmount);
                }
                else if (result.AppliedAmount != 0) throw new InvalidOperationException("Non-applied M1 result has damage.");
            }
            for (var i = 0; i < 3; i++)
            {
                if (_dead[i] && (!roles[i].IsDead || roles[i].CurrentHealth != 0)) throw new InvalidOperationException("Dead roles cannot resurrect.");
                if (applied[i] > _health[i] || roles[i].CurrentHealth != _health[i] - applied[i])
                    throw new InvalidOperationException("M3E1 role health does not conserve same-tick M1 applied damage.");
                if (!_dead[i] && roles[i].IsDead && applied[i] == 0) throw new InvalidOperationException("A death edge requires same-tick applied M1 damage.");
            }
        }

        private static bool HasAppliedFor(IReadOnlyList<DamageResult> results, string role)
        { for (var i = 0; i < results.Count; i++) if (results[i].Result == DamageResultCode.Applied && results[i].TargetId == role) return true; return false; }
        private static readonly string[] Roles = { "player", "surveyor", "walker" };
        private static RegularEnemyEncounterRoleState[] ExactRoles(IReadOnlyList<RegularEnemyEncounterRoleState> values)
        {
            if (values == null || values.Count != 3) throw new ArgumentException("M3E1 roster must be exact.");
            var copy = new RegularEnemyEncounterRoleState[3];
            for (var i = 0; i < 3; i++) { if (values[i].TargetId != Roles[i]) throw new ArgumentException("M3E1 roster order must be player, surveyor, walker."); copy[i] = values[i]; }
            return copy;
        }
        private static int RoleIndex(string targetId)
        { for (var i = 0; i < Roles.Length; i++) if (targetId == Roles[i]) return i; throw new InvalidOperationException("Applied M1 damage targets a foreign M3E1 role."); }
    }
}
