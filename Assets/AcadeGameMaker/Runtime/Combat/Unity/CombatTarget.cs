using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AcadeGameMaker.Transfer.Unity;
using UnityEngine;

namespace AcadeGameMaker.Combat.Unity
{
    // REQ-COM-004, REQ-WT-003, REQ-WT-005. VD-03 M2B's only new public Unity type.
    [DisallowMultipleComponent]
    public sealed class CombatTarget : MonoBehaviour
    {
        [SerializeField] private string _targetId;
        [SerializeField] private int _maxHealth;
        [SerializeField] private int _invulnerabilityTicks;
        [SerializeField] private bool _canReceivePlayerBasicAttack;
        [SerializeField] private int _localAimPointXQ1000;
        [SerializeField] private int _localAimPointYQ1000;
        [SerializeField] private int _localAimShapeCenterXQ1000;
        [SerializeField] private int _localAimShapeCenterYQ1000;
        [SerializeField] private int _localAimShapeHalfExtentXQ1000;
        [SerializeField] private int _localAimShapeHalfExtentYQ1000;
        [SerializeField] private Collider2D _targetCollider;
        [SerializeField] private Transform _targetPoseRoot;
        [SerializeField] private TransferTarget _transferTarget;

        internal string TargetId => _targetId;

        // REQ-COM-004. Validation runs before either M1 or M2A session is created; it never attempts partial publication.
        internal FrozenCombatTarget Freeze()
        {
            ValidateForAuthoring();
            return new FrozenCombatTarget(
                _targetId,
                _maxHealth,
                _invulnerabilityTicks,
                _canReceivePlayerBasicAttack,
                _localAimPointXQ1000,
                _localAimPointYQ1000,
                _localAimShapeCenterXQ1000,
                _localAimShapeCenterYQ1000,
                _localAimShapeHalfExtentXQ1000,
                _localAimShapeHalfExtentYQ1000,
                _targetCollider,
                _targetPoseRoot,
                _transferTarget != null);
        }

        internal void ValidateForAuthoring()
        {
            DamageValidation.Id(_targetId, nameof(_targetId));
            _ = new CombatantRegistration(_targetId, _maxHealth, _invulnerabilityTicks);
            if (_targetCollider == null || _targetPoseRoot == null)
            {
                throw new InvalidOperationException("CombatTarget requires a target collider and pose root.");
            }

            if (_targetCollider.isTrigger)
            {
                throw new InvalidOperationException("CombatTarget collider must be non-trigger.");
            }

            if (_localAimShapeHalfExtentXQ1000 <= 0 || _localAimShapeHalfExtentYQ1000 <= 0)
            {
                throw new ArgumentOutOfRangeException("CombatTarget aim-shape half extents must be positive.");
            }

            if (_targetPoseRoot.localEulerAngles.z != 0f || _targetPoseRoot.localScale.x != 1f || _targetPoseRoot.localScale.y != 1f)
            {
                throw new InvalidOperationException("CombatTarget pose root requires zero Z rotation and unit XY scale.");
            }

            if (string.IsNullOrEmpty(_targetPoseRoot.gameObject.scene.name) || string.IsNullOrEmpty(TransferLineOfSight.HierarchyPath(_targetPoseRoot)))
            {
                throw new InvalidOperationException("CombatTarget authoring requires a named scene and hierarchy path.");
            }

            if (string.Equals(_targetId, "ordan", StringComparison.Ordinal) && _transferTarget != null)
            {
                throw new InvalidOperationException("Ordan body must not carry TransferTarget; only a scripted payload may be transferable.");
            }

            if (_transferTarget != null)
            {
                var transferGeometry = _transferTarget.FreezeAuthoredGeometryForCrossValidation();
                if (!string.Equals(_targetId, transferGeometry.TargetId, StringComparison.Ordinal) ||
                    _targetCollider != transferGeometry.TargetCollider ||
                    _targetPoseRoot != transferGeometry.TargetPoseRoot ||
                    _localAimPointXQ1000 != transferGeometry.LocalAimPointXQ1000 ||
                    _localAimPointYQ1000 != transferGeometry.LocalAimPointYQ1000 ||
                    _localAimShapeCenterXQ1000 != transferGeometry.LocalAimShapeCenterXQ1000 ||
                    _localAimShapeCenterYQ1000 != transferGeometry.LocalAimShapeCenterYQ1000 ||
                    _localAimShapeHalfExtentXQ1000 != transferGeometry.LocalAimShapeHalfExtentXQ1000 ||
                    _localAimShapeHalfExtentYQ1000 != transferGeometry.LocalAimShapeHalfExtentYQ1000)
                {
                    throw new InvalidOperationException("Co-authored CombatTarget and TransferTarget must have exact shared identity and geometry.");
                }
            }
        }

        // REQ-COM-001, REQ-COM-004. Editor authoring and focused tests share
        // the same pure serialized-field assignment implementation.
        internal void ConfigureForAuthoring(
            string targetId, int maxHealth, int invulnerabilityTicks, bool canReceivePlayerBasicAttack,
            int localAimPointXQ1000, int localAimPointYQ1000, int localAimShapeCenterXQ1000, int localAimShapeCenterYQ1000,
            int localAimShapeHalfExtentXQ1000, int localAimShapeHalfExtentYQ1000, Collider2D targetCollider, Transform targetPoseRoot,
            TransferTarget transferTarget = null)
        {
            Configure(targetId, maxHealth, invulnerabilityTicks, canReceivePlayerBasicAttack,
                localAimPointXQ1000, localAimPointYQ1000, localAimShapeCenterXQ1000, localAimShapeCenterYQ1000,
                localAimShapeHalfExtentXQ1000, localAimShapeHalfExtentYQ1000, targetCollider, targetPoseRoot, transferTarget);
        }

        internal void ConfigureForTests(
            string targetId, int maxHealth, int invulnerabilityTicks, bool canReceivePlayerBasicAttack,
            int localAimPointXQ1000, int localAimPointYQ1000, int localAimShapeCenterXQ1000, int localAimShapeCenterYQ1000,
            int localAimShapeHalfExtentXQ1000, int localAimShapeHalfExtentYQ1000, Collider2D targetCollider, Transform targetPoseRoot,
            TransferTarget transferTarget = null)
        {
            Configure(targetId, maxHealth, invulnerabilityTicks, canReceivePlayerBasicAttack,
                localAimPointXQ1000, localAimPointYQ1000, localAimShapeCenterXQ1000, localAimShapeCenterYQ1000,
                localAimShapeHalfExtentXQ1000, localAimShapeHalfExtentYQ1000, targetCollider, targetPoseRoot, transferTarget);
        }

        private void Configure(
            string targetId, int maxHealth, int invulnerabilityTicks, bool canReceivePlayerBasicAttack,
            int localAimPointXQ1000, int localAimPointYQ1000, int localAimShapeCenterXQ1000, int localAimShapeCenterYQ1000,
            int localAimShapeHalfExtentXQ1000, int localAimShapeHalfExtentYQ1000, Collider2D targetCollider, Transform targetPoseRoot,
            TransferTarget transferTarget)
        {
            _targetId = targetId;
            _maxHealth = maxHealth;
            _invulnerabilityTicks = invulnerabilityTicks;
            _canReceivePlayerBasicAttack = canReceivePlayerBasicAttack;
            _localAimPointXQ1000 = localAimPointXQ1000;
            _localAimPointYQ1000 = localAimPointYQ1000;
            _localAimShapeCenterXQ1000 = localAimShapeCenterXQ1000;
            _localAimShapeCenterYQ1000 = localAimShapeCenterYQ1000;
            _localAimShapeHalfExtentXQ1000 = localAimShapeHalfExtentXQ1000;
            _localAimShapeHalfExtentYQ1000 = localAimShapeHalfExtentYQ1000;
            _targetCollider = targetCollider;
            _targetPoseRoot = targetPoseRoot;
            _transferTarget = transferTarget;
        }
    }

    // REQ-COM-004. Immutable authoring copy: scalar targeting geometry cannot be reread from mutable components during reset/capture.
    internal readonly struct FrozenCombatTarget
    {
        internal FrozenCombatTarget(string targetId, int maxHealth, int invulnerabilityTicks, bool canReceivePlayerBasicAttack,
            int localAimPointXQ1000, int localAimPointYQ1000, int localAimShapeCenterXQ1000, int localAimShapeCenterYQ1000,
            int localAimShapeHalfExtentXQ1000, int localAimShapeHalfExtentYQ1000, Collider2D targetCollider, Transform targetPoseRoot, bool hasMatchingTransferTarget)
        {
            TargetId = targetId; MaxHealth = maxHealth; InvulnerabilityTicks = invulnerabilityTicks; CanReceivePlayerBasicAttack = canReceivePlayerBasicAttack;
            LocalAimPointXQ1000 = localAimPointXQ1000; LocalAimPointYQ1000 = localAimPointYQ1000;
            LocalAimShapeCenterXQ1000 = localAimShapeCenterXQ1000; LocalAimShapeCenterYQ1000 = localAimShapeCenterYQ1000;
            LocalAimShapeHalfExtentXQ1000 = localAimShapeHalfExtentXQ1000; LocalAimShapeHalfExtentYQ1000 = localAimShapeHalfExtentYQ1000;
            TargetCollider = targetCollider; TargetPoseRoot = targetPoseRoot; HasMatchingTransferTarget = hasMatchingTransferTarget;
        }
        internal string TargetId { get; } internal int MaxHealth { get; } internal int InvulnerabilityTicks { get; } internal bool CanReceivePlayerBasicAttack { get; }
        internal int LocalAimPointXQ1000 { get; } internal int LocalAimPointYQ1000 { get; } internal int LocalAimShapeCenterXQ1000 { get; } internal int LocalAimShapeCenterYQ1000 { get; }
        internal int LocalAimShapeHalfExtentXQ1000 { get; } internal int LocalAimShapeHalfExtentYQ1000 { get; }
        internal Collider2D TargetCollider { get; } internal Transform TargetPoseRoot { get; } internal bool HasMatchingTransferTarget { get; }
        internal CombatantRegistration CombatantRegistration => new CombatantRegistration(TargetId, MaxHealth, InvulnerabilityTicks);
    }
}
