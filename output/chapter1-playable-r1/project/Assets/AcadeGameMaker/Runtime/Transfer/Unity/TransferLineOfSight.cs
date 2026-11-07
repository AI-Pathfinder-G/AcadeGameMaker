using System;
using System.Collections.Generic;
using System.Globalization;
using AcadeGameMaker.Core;
using UnityEngine;

namespace AcadeGameMaker.Transfer.Unity
{
    // REQ-WT-003, REQ-WT-005, REQ-WT-007. Explicit layer-8 finite LOS with no discovery or instance-ID ordering.
    internal static class TransferLineOfSight
    {
        internal const int Layer = 8;
        internal const int Mask = 1 << Layer;
        private const int BufferSize = 64;
        private static readonly RaycastHit2D[] Hits = new RaycastHit2D[BufferSize];

        internal static TransferLineOfSightResult Evaluate(
            int playerOriginXQ1000,
            int playerOriginYQ1000,
            int targetAimXQ1000,
            int targetAimYQ1000,
            Transform playerRoot,
            Transform targetRoot,
            string targetId,
            SimulationTick tick)
        {
            if (playerRoot == null || targetRoot == null || string.IsNullOrEmpty(targetId))
            {
                throw new ArgumentException("LOS requires explicit player root, target root, and TargetId.");
            }

            var filter = new ContactFilter2D();
            filter.NoFilter();
            filter.useLayerMask = true;
            filter.layerMask = Mask;
            filter.useTriggers = false;
            var count = Physics2D.Linecast(
                new Vector2(playerOriginXQ1000 / 1000f, playerOriginYQ1000 / 1000f),
                new Vector2(targetAimXQ1000 / 1000f, targetAimYQ1000 / 1000f),
                filter,
                Hits);
            if (count >= BufferSize)
            {
                throw new InvalidOperationException($"TransferLineOfSight saturation for TargetId '{targetId}' at SimulationTick {tick.Value}.");
            }

            var diagnostics = new List<TransferLineOfSightDiagnostic>();
            var stableKeys = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < count; index++)
            {
                var hit = Hits[index];
                var collider = hit.collider;
                if (collider == null || collider.isTrigger || IsRootOrDescendant(collider.transform, playerRoot) || IsRootOrDescendant(collider.transform, targetRoot))
                {
                    continue;
                }

                var identity = ColliderIdentity.Create(collider);
                if (!stableKeys.Add(identity.CanonicalKey))
                {
                    throw new InvalidOperationException($"Duplicate stable collider key '{identity.CanonicalKey}' for TargetId '{targetId}' at SimulationTick {tick.Value}.");
                }

                diagnostics.Add(new TransferLineOfSightDiagnostic(FractionKey(hit.fraction), identity));
            }

            diagnostics.Sort(TransferLineOfSightDiagnostic.Compare);
            return new TransferLineOfSightResult(diagnostics);
        }

        internal static string StableColliderKey(Collider2D collider)
        {
            if (collider == null)
            {
                throw new ArgumentNullException(nameof(collider));
            }

            return ColliderIdentity.Create(collider).CanonicalKey;
        }

        internal static string HierarchyPath(Transform transform)
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            return ColliderIdentity.CreateHierarchy(transform).CanonicalPath;
        }

        private static bool IsRootOrDescendant(Transform value, Transform root) => value == root || value.IsChildOf(root);

        private static int FractionKey(float fraction) => Mathf.Clamp(
            checked((int)Math.Round(fraction * 1000000d, MidpointRounding.AwayFromZero)),
            0,
            1000000);
    }

    internal readonly struct TransferLineOfSightDiagnostic
    {
        internal TransferLineOfSightDiagnostic(int fractionKey, ColliderIdentity identity)
        {
            FractionKey = fractionKey;
            SceneName = identity.SceneName;
            HierarchySegments = identity.HierarchySegments;
            ColliderTypeFullName = identity.ColliderTypeFullName;
            ColliderComponentOrdinal = identity.ColliderComponentOrdinal;
            StableColliderKey = identity.CanonicalKey;
        }

        internal int FractionKey { get; }
        internal string SceneName { get; }
        internal IReadOnlyList<string> HierarchySegments { get; }
        internal string ColliderTypeFullName { get; }
        internal int ColliderComponentOrdinal { get; }
        internal string StableColliderKey { get; }

        internal static int Compare(TransferLineOfSightDiagnostic left, TransferLineOfSightDiagnostic right)
        {
            var fraction = left.FractionKey.CompareTo(right.FractionKey);
            if (fraction != 0)
            {
                return fraction;
            }

            var scene = string.CompareOrdinal(left.SceneName, right.SceneName);
            if (scene != 0)
            {
                return scene;
            }

            var hierarchy = ColliderIdentity.CompareHierarchy(left.HierarchySegments, right.HierarchySegments);
            if (hierarchy != 0)
            {
                return hierarchy;
            }

            var type = string.CompareOrdinal(left.ColliderTypeFullName, right.ColliderTypeFullName);
            return type != 0 ? type : left.ColliderComponentOrdinal.CompareTo(right.ColliderComponentOrdinal);
        }
    }

    internal sealed class ColliderIdentity
    {
        internal ColliderIdentity(string sceneName, IReadOnlyList<string> hierarchySegments, string colliderTypeFullName, int colliderComponentOrdinal)
        {
            SceneName = sceneName;
            HierarchySegments = hierarchySegments;
            ColliderTypeFullName = colliderTypeFullName;
            ColliderComponentOrdinal = colliderComponentOrdinal;
            CanonicalPath = CanonicalizeSegments(hierarchySegments);
            CanonicalKey = CanonicalizeSegments(new[]
            {
                sceneName,
                CanonicalPath,
                colliderTypeFullName,
                colliderComponentOrdinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        internal string SceneName { get; }
        internal IReadOnlyList<string> HierarchySegments { get; }
        internal string ColliderTypeFullName { get; }
        internal int ColliderComponentOrdinal { get; }
        internal string CanonicalPath { get; }
        internal string CanonicalKey { get; }

        internal static ColliderIdentity Create(Collider2D collider)
        {
            if (collider == null)
            {
                throw new ArgumentNullException(nameof(collider));
            }

            var sceneName = collider.gameObject.scene.name;
            if (string.IsNullOrEmpty(sceneName))
            {
                throw new InvalidOperationException("LOS collider authoring requires a non-empty scene and hierarchy path.");
            }

            var hierarchy = CreateHierarchy(collider.transform);
            var colliders = collider.GetComponents<Collider2D>();
            var componentIndex = -1;
            for (var index = 0; index < colliders.Length; index++)
            {
                if (colliders[index] == collider)
                {
                    componentIndex = index;
                    break;
                }
            }

            if (componentIndex < 0)
            {
                throw new InvalidOperationException("LOS collider is not an authored component of its GameObject.");
            }

            return new ColliderIdentity(sceneName, hierarchy.Segments, collider.GetType().FullName, componentIndex);
        }

        internal static ColliderHierarchy CreateHierarchy(Transform transform)
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var names = new List<string>();
            for (var current = transform; current != null; current = current.parent)
            {
                if (string.IsNullOrEmpty(current.name))
                {
                    throw new InvalidOperationException("LOS collider authoring requires named hierarchy transforms.");
                }

                names.Add(current.name);
            }

            names.Reverse();
            return new ColliderHierarchy(names.ToArray());
        }

        internal static int CompareHierarchy(IReadOnlyList<string> left, IReadOnlyList<string> right)
        {
            var count = Math.Min(left.Count, right.Count);
            for (var index = 0; index < count; index++)
            {
                var comparison = string.CompareOrdinal(left[index], right[index]);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return left.Count.CompareTo(right.Count);
        }

        private static string CanonicalizeSegments(IReadOnlyList<string> segments)
        {
            var value = segments.Count.ToString(CultureInfo.InvariantCulture) + "|";
            for (var index = 0; index < segments.Count; index++)
            {
                var segment = segments[index] ?? throw new ArgumentException("Canonical identity segments cannot be null.");
                value += segment.Length.ToString(CultureInfo.InvariantCulture) + ":" + segment;
            }

            return value;
        }
    }

    internal readonly struct ColliderHierarchy
    {
        internal ColliderHierarchy(IReadOnlyList<string> segments)
        {
            Segments = segments;
            CanonicalPath = ColliderIdentityCanonicalPath(segments);
        }

        internal IReadOnlyList<string> Segments { get; }
        internal string CanonicalPath { get; }

        private static string ColliderIdentityCanonicalPath(IReadOnlyList<string> segments)
        {
            var value = segments.Count.ToString(CultureInfo.InvariantCulture) + "|";
            for (var index = 0; index < segments.Count; index++)
            {
                value += segments[index].Length.ToString(CultureInfo.InvariantCulture) + ":" + segments[index];
            }

            return value;
        }
    }

    internal sealed class TransferLineOfSightResult
    {
        internal TransferLineOfSightResult(IReadOnlyList<TransferLineOfSightDiagnostic> diagnostics)
        {
            Diagnostics = diagnostics;
        }

        internal IReadOnlyList<TransferLineOfSightDiagnostic> Diagnostics { get; }
        internal bool IsOpen => Diagnostics.Count == 0;
    }
}
