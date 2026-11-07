using System;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Combat
{
    // REQ-COM-001, REQ-COM-004, REQ-COM-005, REQ-COM-006. Engine-free damage ABI owned by VD-03.
    public enum DamageKind
    {
        BasicAttack = 0,
        EnemyContact = 1,
        BossPattern = 2,
        HeavyImpact = 3,
        ChoiceSkill = 4,
        BossPayloadTransfer = 5,
        EnemyRanged = 6
    }

    public enum DamageResultCode
    {
        Applied = 0,
        Invulnerable = 1,
        Duplicate = 2,
        TargetDead = 3,
        Invalid = 4
    }

    public readonly struct DamageRequest
    {
        public DamageRequest(string requestId, string sourceId, string targetId, int amount, DamageKind kind, SimulationTick tick)
        {
            DamageValidation.Id(requestId, nameof(requestId));
            DamageValidation.Id(sourceId, nameof(sourceId));
            DamageValidation.Id(targetId, nameof(targetId));
            if (amount < 1 || amount > 999) throw new ArgumentOutOfRangeException(nameof(amount));
            DamageValidation.Kind(kind);
            RequestId = requestId;
            SourceId = sourceId;
            TargetId = targetId;
            Amount = amount;
            Kind = kind;
            Tick = tick;
        }

        public string RequestId { get; }
        public string SourceId { get; }
        public string TargetId { get; }
        public int Amount { get; }
        public DamageKind Kind { get; }
        public SimulationTick Tick { get; }
    }

    public readonly struct DamageResult
    {
        public DamageResult(string requestId, string targetId, int appliedAmount, DamageResultCode result, SimulationTick tick)
        {
            DamageValidation.Id(requestId, nameof(requestId));
            DamageValidation.Id(targetId, nameof(targetId));
            DamageValidation.Result(result);
            if (appliedAmount < 0 || appliedAmount > 999) throw new ArgumentOutOfRangeException(nameof(appliedAmount));
            if ((result == DamageResultCode.Applied) != (appliedAmount > 0)) throw new ArgumentException("Applied results require a positive amount; all other results require zero.", nameof(appliedAmount));
            RequestId = requestId;
            TargetId = targetId;
            AppliedAmount = appliedAmount;
            Result = result;
            Tick = tick;
        }

        public string RequestId { get; }
        public string TargetId { get; }
        public int AppliedAmount { get; }
        public DamageResultCode Result { get; }
        public SimulationTick Tick { get; }
    }

    internal static class DamageValidation
    {
        internal static void Id(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 96) throw new ArgumentException(parameterName);
            foreach (var character in value)
            {
                if (character > 127 || !((character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z') || (character >= '0' && character <= '9') || character == '.' || character == '_' || character == ':' || character == '/' || character == '-'))
                {
                    throw new ArgumentException(parameterName);
                }
            }
        }

        internal static void Kind(DamageKind value)
        {
            if (value < DamageKind.BasicAttack || value > DamageKind.EnemyRanged) throw new ArgumentOutOfRangeException(nameof(value));
        }

        internal static void Result(DamageResultCode value)
        {
            if (value < DamageResultCode.Applied || value > DamageResultCode.Invalid) throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}
