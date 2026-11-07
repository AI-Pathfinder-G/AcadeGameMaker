using System;
using AcadeGameMaker.Core;

namespace AcadeGameMaker.Run
{
    // REQ-C1C-004, REQ-CHOICE-004. 전이·피해·저장 권한은 원래 생산 소유자에 남는다.
    internal sealed class ChoiceSkillSession
    {
        private readonly object _runKey;
        private readonly int _choice, _consent, _skill;
        private ChoiceSkillCandidate _pending;
        private long _lastCommittedEdge;
        private long _skillOrdinal;
        private int _cooldownEndTick;
        private int _recallLockEndTick;

        internal ChoiceSkillSession(object runKey, int choice, int consent, int skill)
        {
            if (!((choice == 1 && consent == 4 && skill == 1) || (choice == 2 && consent == 3 && skill == 2)))
                throw new ArgumentException("승인된 선택·동의·기술 조합이 아닙니다.");
            _runKey = runKey ?? throw new ArgumentNullException(nameof(runKey));
            _choice = choice; _consent = consent; _skill = skill;
        }
        internal int CooldownEndTick => _cooldownEndTick;
        internal int RecallLockEndTick => _recallLockEndTick;
        internal long SkillOrdinal => _skillOrdinal;
        internal bool IsOwnedByRun(object runKey) => ReferenceEquals(_runKey,runKey);

        internal ChoiceSkillCandidate Prepare(ChoicePhaseContext context)
        {
            if (_pending != null || !Matches(context) || context.ChoiceEdgeOrdinal <= _lastCommittedEdge ||
                context.Tick.Value < _cooldownEndTick || context.Tick.Value < _recallLockEndTick) return null;
            try
            {
                if (context.Skill == 1 && checked(context.Tick.Value - context.TransferStartedTick.Value) < 21) return null;
            }
            catch (OverflowException) { return null; }
            try
            {
                // 회수 이전에 모든 tick/revision/ordinal 넘침을 검사한다.
                if (context.Skill == 1) checked { var nextRevision = context.TransferRevision + 1; }
                var tick = context.Tick.Value;
                var result = new ChoicePhaseResult(context.Tick, checked(_skillOrdinal + 1), context.Skill,
                    new SimulationTick(checked(tick + (context.Skill == 1 ? 300 : 420))),
                    new SimulationTick(checked(tick + (context.Skill == 1 ? 36 : 0))),
                    new SimulationTick(context.Skill == 1 ? checked(tick + 21) : 0),
                    new SimulationTick(context.Skill == 2 ? checked(tick + 60) : 0),
                    new SimulationTick(context.Skill == 2 ? checked(tick + 150) : 0));
                _pending = new ChoiceSkillCandidate(this, context, result, _skillOrdinal, _lastCommittedEdge,
                    _cooldownEndTick, _recallLockEndTick);
                return _pending;
            }
            catch (OverflowException) { return null; }
        }
        internal bool Validate(ChoiceSkillCandidate candidate) => candidate != null &&
            ReferenceEquals(candidate, _pending) && ReferenceEquals(candidate.Owner, this) && Matches(candidate.Context) &&
            candidate.PriorOrdinal == _skillOrdinal && candidate.PriorEdge == _lastCommittedEdge &&
            candidate.PriorCooldown == _cooldownEndTick && candidate.PriorRecallLock == _recallLockEndTick;

        internal ChoicePhaseResult Commit(ChoiceSkillCandidate candidate)
        {
            if (!Validate(candidate)) throw new InvalidOperationException("현재 기술 후보가 아닙니다.");
            var result = candidate.Result;
            _pending = null;
            _skillOrdinal = result.SkillOrdinal;
            _lastCommittedEdge = candidate.Context.ChoiceEdgeOrdinal;
            _cooldownEndTick = result.CooldownEndTick.Value;
            if (result.Skill == 1) _recallLockEndTick = result.RecallLockEndTick.Value;
            return result;
        }
        internal void Cancel(ChoiceSkillCandidate candidate)
        {
            if (candidate != null && ReferenceEquals(_pending, candidate) && ReferenceEquals(candidate.Owner, this)) _pending = null;
        }
        private bool Matches(ChoicePhaseContext context) => context != null &&
            ReferenceEquals(context.RunKey, _runKey) &&
            context.Choice == _choice && context.Consent == _consent && context.Skill == _skill &&
            context.Mode == InputMode.GameplayEnabled;
    }

    internal sealed class ChoiceSkillCandidate
    {
        internal ChoiceSkillCandidate(ChoiceSkillSession owner, ChoicePhaseContext context, ChoicePhaseResult result,
            long priorOrdinal, long priorEdge, int priorCooldown, int priorRecallLock)
        {
            Owner = owner; Context = context; Result = result; PriorOrdinal = priorOrdinal; PriorEdge = priorEdge;
            PriorCooldown = priorCooldown; PriorRecallLock = priorRecallLock;
        }
        internal ChoiceSkillSession Owner { get; }
        internal ChoicePhaseContext Context { get; }
        internal ChoicePhaseResult Result { get; }
        internal long PriorOrdinal { get; }
        internal long PriorEdge { get; }
        internal int PriorCooldown { get; }
        internal int PriorRecallLock { get; }
    }
}
