using System;
using System.Collections.Generic;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-001, REQ-COM-004: immutable, Combat-owned next-tick threat publication.
    internal sealed class RegularEnemyThreatDeliveryCandidate
    {
        private readonly DamageRequest[] _requests;
        private readonly object _owner;

        internal RegularEnemyThreatDeliveryCandidate(object owner, SimulationTick sourceTick,
            SimulationTick deliveryTick, IReadOnlyList<DamageRequest> requests)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            SourceTick = sourceTick;
            DeliveryTick = deliveryTick;
            _requests = Copy(requests);
        }

        internal SimulationTick SourceTick { get; }
        internal SimulationTick DeliveryTick { get; }
        internal int RequestCount => _requests.Length;
        internal DamageRequest GetRequest(int index) => _requests[index];
        internal DamageRequest[] CopyRequests() => (DamageRequest[])_requests.Clone();
        internal bool IsOwnedBy(object owner) => ReferenceEquals(_owner, owner);

        private static DamageRequest[] Copy(IReadOnlyList<DamageRequest> requests)
        {
            if (requests == null) throw new ArgumentNullException(nameof(requests));
            var copy = new DamageRequest[requests.Count];
            for (var index = 0; index < copy.Length; index++) copy[index] = requests[index];
            return copy;
        }
    }

    internal readonly struct RegularEnemyThreatDeliveryBatch
    {
        private readonly DamageRequest[] _requests;

        internal RegularEnemyThreatDeliveryBatch(SimulationTick sourceTick, SimulationTick deliveryTick,
            IReadOnlyList<DamageRequest> requests)
        {
            SourceTick = sourceTick;
            DeliveryTick = deliveryTick;
            if (requests == null) throw new ArgumentNullException(nameof(requests));
            _requests = new DamageRequest[requests.Count];
            for (var index = 0; index < _requests.Length; index++) _requests[index] = requests[index];
        }

        internal SimulationTick SourceTick { get; }
        internal SimulationTick DeliveryTick { get; }
        internal int RequestCount => _requests?.Length ?? 0;
        internal DamageRequest GetRequest(int index) => _requests[index];
        internal DamageRequest[] CopyRequests() => _requests == null ? Array.Empty<DamageRequest>() : (DamageRequest[])_requests.Clone();
    }
}
