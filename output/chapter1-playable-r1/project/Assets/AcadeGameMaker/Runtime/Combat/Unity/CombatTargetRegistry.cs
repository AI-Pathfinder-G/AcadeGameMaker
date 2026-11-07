using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-001, REQ-COM-004. The matching source-file identity makes this
    // explicit serialized registry a valid Unity component.
    [DisallowMultipleComponent]
    internal sealed class CombatTargetRegistry : MonoBehaviour
    {
        [SerializeField] private List<CombatTarget> _targets = new List<CombatTarget>();
        private ReadOnlyCollection<FrozenCombatTarget> _frozenTargets;

        internal IReadOnlyList<FrozenCombatTarget> FrozenTargets => _frozenTargets ?? throw new InvalidOperationException("CombatTargetRegistry has not been validated and frozen.");
        internal IReadOnlyList<FrozenCombatTarget> ReadValidatedAuthoringRoster() =>
            new ReadOnlyCollection<FrozenCombatTarget>(BuildValidatedRoster());

        internal void ValidateAndFreeze()
        {
            var staged = BuildValidatedRoster();
            _frozenTargets = new ReadOnlyCollection<FrozenCombatTarget>(staged);
        }

        private FrozenCombatTarget[] BuildValidatedRoster()
        {
            if (_targets == null) throw new InvalidOperationException("CombatTarget registry requires an explicit target list.");
            var ids = new HashSet<string>(StringComparer.Ordinal);
            var colliderKeys = new HashSet<string>(StringComparer.Ordinal);
            var staged = new FrozenCombatTarget[_targets.Count];
            string previousId = null;
            for (var index = 0; index < _targets.Count; index++)
            {
                var target = _targets[index] ?? throw new InvalidOperationException("CombatTarget registry cannot contain a missing target reference.");
                var frozen = target.Freeze();
                if (!ids.Add(frozen.TargetId)) throw new InvalidOperationException("CombatTarget registry contains a duplicate TargetId.");
                if (previousId != null && string.CompareOrdinal(previousId, frozen.TargetId) >= 0)
                    throw new InvalidOperationException("CombatTarget registry must be explicitly sorted by ordinal TargetId.");
                previousId = frozen.TargetId;
                if (!colliderKeys.Add(TransferLineOfSight.StableColliderKey(frozen.TargetCollider)))
                    throw new InvalidOperationException("CombatTarget registry contains a duplicate stable collider key.");
                staged[index] = frozen;
            }
            return staged;
        }

        internal void ConfigureForAuthoring(params CombatTarget[] targets) => Configure(targets);
        internal void ConfigureForTests(params CombatTarget[] targets) => Configure(targets);
        private void Configure(CombatTarget[] targets) => _targets = new List<CombatTarget>(targets);
    }
}
