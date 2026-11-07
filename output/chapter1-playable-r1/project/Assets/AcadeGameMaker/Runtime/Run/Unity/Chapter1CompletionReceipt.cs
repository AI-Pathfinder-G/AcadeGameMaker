using System;
using AcadeGameMaker.Profile;
using AcadeGameMaker.Input.Unity;

namespace AcadeGameMaker.Run.Unity
{
    // REQ-C1C-005, AC-CH1-006/007. 저장 문서에는 이 실행의 영수증을 기록하지 않는다.
    internal sealed class Chapter1DemoEndReceipt
    {
        internal Chapter1DemoEndReceipt(Chapter1PlayableFlowOwner owner, object runKey,
            long generation, ProfileBranch branch, long committedRunEndRevision)
        {
            if (owner == null || runKey == null) throw new ArgumentNullException();
            if (generation < 1 || committedRunEndRevision < 1 || !ReceiptValidation.IsBranch(branch)) throw new ArgumentOutOfRangeException();
            Owner = owner; RunKey = runKey; Generation = generation; Branch = branch;
            CommittedRunEndRevision = committedRunEndRevision;
        }
        internal Chapter1PlayableFlowOwner Owner { get; }
        internal object RunKey { get; }
        internal long Generation { get; }
        internal ProfileBranch Branch { get; }
        internal long CommittedRunEndRevision { get; }
    }

    internal sealed class Chapter1BranchSaveReceipt
    {
        internal Chapter1BranchSaveReceipt(Chapter1PlayableFlowOwner owner, Chapter1DemoEndReceipt demo,
            ProfileAtomicSaveResultV1 result, long expectedRevision)
        {
            if (owner == null || demo == null || !ReferenceEquals(demo.Owner, owner)) throw new ArgumentException();
            result.Validate();
            if (expectedRevision < 0 || result.CommittedRevision != expectedRevision ||
                !(result.Outcome == ProfileAtomicSaveOutcome.CommittedFirst || result.Outcome == ProfileAtomicSaveOutcome.CommittedReplacement))
                throw new InvalidOperationException("실제 분기 저장이 확정되지 않았습니다.");
            Owner = owner; Demo = demo; Outcome = result.Outcome; ExpectedRevision=expectedRevision; CommittedRevision = result.CommittedRevision;
        }
        internal Chapter1PlayableFlowOwner Owner { get; }
        internal Chapter1DemoEndReceipt Demo { get; }
        internal long ExpectedRevision { get; }
        internal ProfileAtomicSaveOutcome Outcome { get; }
        internal long CommittedRevision { get; }
    }

    internal sealed class Chapter1ClosingReceipt
    {
        internal Chapter1ClosingReceipt(object presenter, Chapter1DemoEndReceipt demo,
            Chapter1ClosingHostEntry hostEntry, string assetSha256, long playbackOrdinal, long finalAdvanceOrdinal)
        {
            if (presenter == null || demo == null || hostEntry==null) throw new ArgumentNullException();
            if(!ReferenceEquals(hostEntry.Demo,demo) || !ReferenceEquals(hostEntry.RunKey,demo.RunKey) ||
                hostEntry.OldDemoGeneration!=demo.Generation || hostEntry.NewUiGeneration!=checked(demo.Generation+1))
                throw new InvalidOperationException("완료 대사의 원정과 두 세대가 다릅니다.");
            ReceiptValidation.Sha256(assetSha256);
            if (playbackOrdinal < 1 || finalAdvanceOrdinal < 1) throw new ArgumentOutOfRangeException();
            Presenter = presenter; Demo = demo; HostEntry=hostEntry; DemoGeneration=demo.Generation; UiGeneration=hostEntry.NewUiGeneration; AssetSha256 = assetSha256;
            PlaybackOrdinal = playbackOrdinal; FinalAdvanceOrdinal = finalAdvanceOrdinal;
        }
        internal Chapter1ClosingHostEntry HostEntry { get; }
        internal long DemoGeneration { get; }
        internal long UiGeneration { get; }
        internal object Presenter { get; }
        internal Chapter1DemoEndReceipt Demo { get; }
        internal string AssetSha256 { get; }
        internal long PlaybackOrdinal { get; }
        internal long FinalAdvanceOrdinal { get; }
    }

    internal sealed class Chapter1CompletionReceipt
    {
        internal Chapter1CompletionReceipt(Chapter1PlayableFlowOwner owner, Chapter1DemoEndReceipt demo,
            Chapter1ClosingReceipt closing, Chapter1BranchSaveReceipt save)
        {
            if (owner == null || demo == null || closing == null || save == null ||
                !ReferenceEquals(owner, demo.Owner) || !ReferenceEquals(owner, save.Owner) ||
                !ReferenceEquals(demo, closing.Demo) || !ReferenceEquals(demo, save.Demo) ||
                !ReferenceEquals(closing.HostEntry.Demo,demo) || !ReferenceEquals(closing.HostEntry.BranchSave,save) ||
                !ReferenceEquals(closing.HostEntry.RunKey,demo.RunKey) || closing.DemoGeneration!=demo.Generation ||
                closing.UiGeneration!=checked(demo.Generation+1) || save.ExpectedRevision!=save.CommittedRevision)
                throw new InvalidOperationException("완료 영수증의 실행 소유자가 다릅니다.");
            Owner = owner; Demo = demo; Closing = closing; Save = save;
        }
        internal Chapter1PlayableFlowOwner Owner { get; }
        internal Chapter1DemoEndReceipt Demo { get; }
        internal Chapter1ClosingReceipt Closing { get; }
        internal Chapter1BranchSaveReceipt Save { get; }
    }

    internal static class ReceiptValidation
    {
        internal static bool IsBranch(ProfileBranch branch) => branch == ProfileBranch.Extraction || branch == ProfileBranch.Solidarity;
        internal static void Sha256(string value)
        {
            if (value == null || value.Length != 64) throw new ArgumentException("서사 원문 지문이 없습니다.");
            foreach (var c in value)
                if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F'))) throw new ArgumentException("서사 지문 형식이 다릅니다.");
        }
    }
}
