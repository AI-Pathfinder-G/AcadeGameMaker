using System;

namespace AcadeGameMaker.Core
{
    // REQ-C1C-004, REQ-CHOICE-004. 생산 발급자와 소비자는 각 소유자의 pending 객체를 대조한다.
    public interface IChapter1ChoicePhasePort
    {
        ChoicePhaseCandidate PrepareChoice(ChoicePhaseContext context);
        bool ValidateChoice(ChoicePhaseCandidate candidate);
        ChoicePhaseResult CommitChoice(ChoicePhaseCandidate candidate);
        void CancelChoice(ChoicePhaseCandidate candidate);
    }

    public sealed class ChoicePhaseContext
    {
        internal ChoicePhaseContext(object driver, object adapter, object receipt, object capture,
            object runKey, long generation, SimulationTick tick, int transferRevision,
            string targetId, long choiceEdgeOrdinal, int choice, int consent, int skill,
            InputMode mode, SimulationTick transferStartedTick)
        {
            if (driver == null || adapter == null || receipt == null || capture == null || runKey == null)
                throw new ArgumentNullException();
            if (generation < 1 || tick.Value < 0 || transferRevision < 0 || choiceEdgeOrdinal < 1 ||
                string.IsNullOrEmpty(targetId) || transferStartedTick.Value < 0 || transferStartedTick.Value > tick.Value)
                throw new ArgumentOutOfRangeException();
            if (!((choice == 1 && consent == 4 && skill == 1) || (choice == 2 && consent == 3 && skill == 2)))
                throw new ArgumentException("선택·동의·기술이 승인된 조합이 아닙니다.");
            if (mode != InputMode.GameplayEnabled) throw new ArgumentException("기술 입력 단계가 아닙니다.");
            Driver = driver; Adapter = adapter; GlobalReceipt = receipt; Capture = capture; RunKey = runKey;
            Generation = generation; Tick = tick; TransferRevision = transferRevision; TargetId = targetId;
            ChoiceEdgeOrdinal = choiceEdgeOrdinal; Choice = choice; Consent = consent; Skill = skill;
            Mode = mode; TransferStartedTick = transferStartedTick;
        }
        public object Driver { get; }
        public object Adapter { get; }
        public object GlobalReceipt { get; }
        public object Capture { get; }
        public object RunKey { get; }
        public long Generation { get; }
        public SimulationTick Tick { get; }
        public int TransferRevision { get; }
        public string TargetId { get; }
        public long ChoiceEdgeOrdinal { get; }
        public int Choice { get; }
        public int Consent { get; }
        public int Skill { get; }
        public InputMode Mode { get; }
        public SimulationTick TransferStartedTick { get; }
    }

    public sealed class ChoicePhaseCandidate
    {
        internal ChoicePhaseCandidate(object owner, ChoicePhaseContext context, object sessionProof,
            object recallProof, object environmentProof)
        {
            if (owner == null || context == null || sessionProof == null || environmentProof == null)
                throw new ArgumentNullException();
            if ((context.Skill == 1) != (recallProof != null)) throw new ArgumentException("회수 후보가 기술과 다릅니다.");
            Owner = owner; Context = context; SessionProof = sessionProof; RecallProof = recallProof;
            EnvironmentProof = environmentProof;
        }
        public object Owner { get; }
        public ChoicePhaseContext Context { get; }
        public object SessionProof { get; }
        public object RecallProof { get; }
        public object EnvironmentProof { get; }
    }

    // 결과는 관측값이며 실제 환경 통과나 저장 권한을 발급하지 않는다.
    public readonly struct ChoicePhaseResult
    {
        public ChoicePhaseResult(SimulationTick tick, long skillOrdinal, int skill,
            SimulationTick cooldownEndTick, SimulationTick effectTick, SimulationTick recallLockEndTick,
            SimulationTick holdEndTick, SimulationTick platformEndTick)
        {
            if (tick.Value < 0 || skillOrdinal < 1 || (skill != 1 && skill != 2)) throw new ArgumentOutOfRangeException();
            var cooldown = checked(tick.Value + (skill == 1 ? 300 : 420));
            if (cooldownEndTick.Value != cooldown || effectTick.Value != checked(tick.Value + (skill == 1 ? 36 : 0)) ||
                recallLockEndTick.Value != (skill == 1 ? checked(tick.Value + 21) : 0) ||
                holdEndTick.Value != (skill == 2 ? checked(tick.Value + 60) : 0) ||
                platformEndTick.Value != (skill == 2 ? checked(tick.Value + 150) : 0))
                throw new ArgumentException("기술 종료 경계가 승인값과 다릅니다.");
            Tick = tick; SkillOrdinal = skillOrdinal; Skill = skill; CooldownEndTick = cooldownEndTick;
            EffectTick = effectTick; RecallLockEndTick = recallLockEndTick; HoldEndTick = holdEndTick;
            PlatformEndTick = platformEndTick;
        }
        public SimulationTick Tick { get; }
        public long SkillOrdinal { get; }
        public int Skill { get; }
        public SimulationTick CooldownEndTick { get; }
        public SimulationTick EffectTick { get; }
        public SimulationTick RecallLockEndTick { get; }
        public SimulationTick HoldEndTick { get; }
        public SimulationTick PlatformEndTick { get; }
        public bool ConsumesTransferPressed => SkillOrdinal > 0;
    }
}
