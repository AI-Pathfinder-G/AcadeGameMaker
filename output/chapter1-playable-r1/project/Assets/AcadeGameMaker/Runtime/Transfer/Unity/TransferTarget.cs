using System;
using System.Collections.Generic;
using AcadeGameMaker.Transfer;
using UnityEngine;

namespace AcadeGameMaker.Transfer.Unity
{
    // REQ-WT-003, REQ-WT-005, REQ-WT-007. The only public Unity component added by VD-02 M2.
    [DisallowMultipleComponent]
    public sealed class TransferTarget : MonoBehaviour
    {
        [SerializeField] private string _targetId;
        [SerializeField] private TransferTargetKind _kind;
        [SerializeField] private int _localAimPointXQ1000;
        [SerializeField] private int _localAimPointYQ1000;
        [SerializeField] private int _localAimShapeCenterXQ1000;
        [SerializeField] private int _localAimShapeCenterYQ1000;
        [SerializeField] private int _localAimShapeHalfExtentXQ1000;
        [SerializeField] private int _localAimShapeHalfExtentYQ1000;
        [SerializeField] private string _baseModifierProfileId;
        [SerializeField] private Collider2D _targetCollider;
        [SerializeField] private Transform _targetPoseRoot;
        [SerializeField] private MonoBehaviour _targetOwnerSink;

        internal string TargetId => _targetId;
        internal Collider2D TargetCollider => _targetCollider;
        internal Transform TargetPoseRoot => _targetPoseRoot;
        internal ITransferTargetModifierSink TargetOwnerSink => _targetOwnerSink as ITransferTargetModifierSink;

        // REQ-WT-003, REQ-WT-005, REQ-COM-004. Cross-system authoring validation may read frozen geometry without observing runtime transfer availability.
        internal TransferTargetAuthoredGeometry FreezeAuthoredGeometryForCrossValidation()
        {
            ValidateForAuthoring();
            return new TransferTargetAuthoredGeometry(
                _targetId,
                _localAimPointXQ1000,
                _localAimPointYQ1000,
                _localAimShapeCenterXQ1000,
                _localAimShapeCenterYQ1000,
                _localAimShapeHalfExtentXQ1000,
                _localAimShapeHalfExtentYQ1000,
                _targetCollider,
                _targetPoseRoot);
        }

        internal TransferTargetDescriptor BuildDescriptor()
        {
            ValidateForAuthoring();
            return BuildDescriptorFromValidatedAuthoring();
        }

        // REQ-WT-003, REQ-WT-006. Per-tick capture consumes the registry that was validated during authoring/startup.
        internal TransferTargetDescriptor BuildDescriptorFromValidatedAuthoring()
        {
            return new TransferTargetDescriptor(
                _targetId,
                _kind,
                _localAimPointXQ1000,
                _localAimPointYQ1000,
                _localAimShapeCenterXQ1000,
                _localAimShapeCenterYQ1000,
                _localAimShapeHalfExtentXQ1000,
                _localAimShapeHalfExtentYQ1000,
                _baseModifierProfileId,
                TargetOwnerSink.IsStillAvailable());
        }

        // REQ-WT-003, REQ-WT-005. Called only by explicit registry/authoring validation, never discovery.
        internal void ValidateForAuthoring()
        {
            if (string.IsNullOrEmpty(_targetId) || _targetCollider == null || _targetPoseRoot == null || _targetOwnerSink == null)
            {
                throw new InvalidOperationException("TransferTarget requires all frozen serialized bindings.");
            }

            var sink = TargetOwnerSink;
            if (sink == null || !string.Equals(sink.BoundTargetId, _targetId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("TransferTarget sink binding must implement ITransferTargetModifierSink for the exact authored TargetId.");
            }

            if (_targetCollider.isTrigger || _targetCollider.gameObject.layer == TransferLineOfSight.Layer)
            {
                throw new InvalidOperationException("TransferTarget collider cannot be a trigger or part of the TransferLineOfSight layer.");
            }

            if (_targetPoseRoot.localEulerAngles.z != 0f || _targetPoseRoot.localScale.x != 1f || _targetPoseRoot.localScale.y != 1f)
            {
                throw new InvalidOperationException("TransferTarget pose root requires zero Z rotation and unit XY scale.");
            }

            if (string.IsNullOrEmpty(_targetPoseRoot.gameObject.scene.name) || string.IsNullOrEmpty(TransferLineOfSight.HierarchyPath(_targetPoseRoot)))
            {
                throw new InvalidOperationException("TransferTarget authoring requires a named scene and hierarchy path.");
            }

            foreach (var collider in _targetPoseRoot.GetComponentsInChildren<Collider2D>(true))
            {
                if (collider.gameObject.layer == TransferLineOfSight.Layer)
                {
                    throw new InvalidOperationException("No target-root collider may be on the TransferLineOfSight layer.");
                }
            }

            // The descriptor constructor owns scalar ID, enum, extent, and base-profile validation.
            _ = new TransferTargetDescriptor(
                _targetId,
                _kind,
                _localAimPointXQ1000,
                _localAimPointYQ1000,
                _localAimShapeCenterXQ1000,
                _localAimShapeCenterYQ1000,
                _localAimShapeHalfExtentXQ1000,
                _localAimShapeHalfExtentYQ1000,
                _baseModifierProfileId,
                true);
        }

        // REQ-WT-003, REQ-WT-005; affected REQ-COM-001. Type-safe authoring and
        // focused tests share one assignment implementation; neither validates or registers.
        internal void ConfigureForAuthoring(
            string targetId,
            TransferTargetKind kind,
            int localAimPointXQ1000,
            int localAimPointYQ1000,
            int localAimShapeCenterXQ1000,
            int localAimShapeCenterYQ1000,
            int localAimShapeHalfExtentXQ1000,
            int localAimShapeHalfExtentYQ1000,
            string baseModifierProfileId,
            Collider2D targetCollider,
            Transform targetPoseRoot,
            MonoBehaviour targetOwnerSink)
        {
            Configure(targetId, kind, localAimPointXQ1000, localAimPointYQ1000,
                localAimShapeCenterXQ1000, localAimShapeCenterYQ1000,
                localAimShapeHalfExtentXQ1000, localAimShapeHalfExtentYQ1000,
                baseModifierProfileId, targetCollider, targetPoseRoot, targetOwnerSink);
        }

        internal void ConfigureForTests(
            string targetId, TransferTargetKind kind, int localAimPointXQ1000, int localAimPointYQ1000,
            int localAimShapeCenterXQ1000, int localAimShapeCenterYQ1000,
            int localAimShapeHalfExtentXQ1000, int localAimShapeHalfExtentYQ1000,
            string baseModifierProfileId, Collider2D targetCollider, Transform targetPoseRoot, MonoBehaviour targetOwnerSink)
        {
            Configure(targetId, kind, localAimPointXQ1000, localAimPointYQ1000,
                localAimShapeCenterXQ1000, localAimShapeCenterYQ1000,
                localAimShapeHalfExtentXQ1000, localAimShapeHalfExtentYQ1000,
                baseModifierProfileId, targetCollider, targetPoseRoot, targetOwnerSink);
        }

        private void Configure(
            string targetId, TransferTargetKind kind, int localAimPointXQ1000, int localAimPointYQ1000,
            int localAimShapeCenterXQ1000, int localAimShapeCenterYQ1000,
            int localAimShapeHalfExtentXQ1000, int localAimShapeHalfExtentYQ1000,
            string baseModifierProfileId, Collider2D targetCollider, Transform targetPoseRoot, MonoBehaviour targetOwnerSink)
        {
            _targetId = targetId;
            _kind = kind;
            _localAimPointXQ1000 = localAimPointXQ1000;
            _localAimPointYQ1000 = localAimPointYQ1000;
            _localAimShapeCenterXQ1000 = localAimShapeCenterXQ1000;
            _localAimShapeCenterYQ1000 = localAimShapeCenterYQ1000;
            _localAimShapeHalfExtentXQ1000 = localAimShapeHalfExtentXQ1000;
            _localAimShapeHalfExtentYQ1000 = localAimShapeHalfExtentYQ1000;
            _baseModifierProfileId = baseModifierProfileId;
            _targetCollider = targetCollider;
            _targetPoseRoot = targetPoseRoot;
            _targetOwnerSink = targetOwnerSink;
        }
    }

    internal readonly struct TransferTargetAuthoredGeometry
    {
        internal TransferTargetAuthoredGeometry(
            string targetId,
            int localAimPointXQ1000,
            int localAimPointYQ1000,
            int localAimShapeCenterXQ1000,
            int localAimShapeCenterYQ1000,
            int localAimShapeHalfExtentXQ1000,
            int localAimShapeHalfExtentYQ1000,
            Collider2D targetCollider,
            Transform targetPoseRoot)
        {
            TargetId = targetId;
            LocalAimPointXQ1000 = localAimPointXQ1000;
            LocalAimPointYQ1000 = localAimPointYQ1000;
            LocalAimShapeCenterXQ1000 = localAimShapeCenterXQ1000;
            LocalAimShapeCenterYQ1000 = localAimShapeCenterYQ1000;
            LocalAimShapeHalfExtentXQ1000 = localAimShapeHalfExtentXQ1000;
            LocalAimShapeHalfExtentYQ1000 = localAimShapeHalfExtentYQ1000;
            TargetCollider = targetCollider;
            TargetPoseRoot = targetPoseRoot;
        }

        internal string TargetId { get; }
        internal int LocalAimPointXQ1000 { get; }
        internal int LocalAimPointYQ1000 { get; }
        internal int LocalAimShapeCenterXQ1000 { get; }
        internal int LocalAimShapeCenterYQ1000 { get; }
        internal int LocalAimShapeHalfExtentXQ1000 { get; }
        internal int LocalAimShapeHalfExtentYQ1000 { get; }
        internal Collider2D TargetCollider { get; }
        internal Transform TargetPoseRoot { get; }
    }

}
