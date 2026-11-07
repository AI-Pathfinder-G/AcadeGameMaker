using System;
using System.Collections.Generic;
using UnityEngine;

namespace AcadeGameMaker.Transfer.Unity
{
    // REQ-WT-003, REQ-WT-005; affected REQ-COM-001. The matching source-file
    // identity makes this explicit serialized registry a valid Unity component.
    [DisallowMultipleComponent]
    internal sealed class TransferTargetRegistry : MonoBehaviour
    {
        [SerializeField] private List<TransferTarget> _targets = new List<TransferTarget>();

        internal IReadOnlyList<TransferTarget> Targets => _targets;

        internal void ValidateForAuthoring()
        {
            var targetIds = new HashSet<string>(StringComparer.Ordinal);
            var colliderKeys = new HashSet<string>(StringComparer.Ordinal);
            string previousTargetId = null;
            foreach (var target in _targets)
            {
                if (target == null) throw new InvalidOperationException("TransferTarget registry cannot contain a missing target reference.");
                target.ValidateForAuthoring();
                if (!targetIds.Add(target.TargetId)) throw new InvalidOperationException("TransferTarget registry contains a duplicate TargetId.");
                if (previousTargetId != null && string.CompareOrdinal(previousTargetId, target.TargetId) >= 0)
                    throw new InvalidOperationException("TransferTarget registry must be explicitly sorted by ordinal TargetId.");
                previousTargetId = target.TargetId;
                if (!colliderKeys.Add(TransferLineOfSight.StableColliderKey(target.TargetCollider)))
                    throw new InvalidOperationException("TransferTarget registry contains a duplicate stable collider key.");
            }
        }

        internal void ConfigureForAuthoring(params TransferTarget[] targets) => Configure(targets);
        internal void ConfigureForTests(params TransferTarget[] targets) => Configure(targets);
        private void Configure(TransferTarget[] targets) => _targets = new List<TransferTarget>(targets);
    }
}
