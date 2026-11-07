using System;
using System.IO;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;

namespace AcadeGameMaker.Run.Unity
{
    // REQ-CH1-007, REQ-C1B-RETURN-001: 전체 V1 보존 snapshot과 공개 저장 결과만 소비한다.
    internal sealed class Chapter1DevelopmentProfile
    {
        internal static string ExactRoot=>DesktopProfileLaunchAdapterV1.Chapter1ProfileRoot;
        private readonly string _root;
        private ProfileCanonicalDocumentV1 _document;
        private bool _ready;
        private sealed class PendingChoiceRetry
        {
            internal PendingChoiceRetry(ProfileCanonicalDocumentV1 before,ProfileCanonicalDocumentV1 candidate,int choice,int consent,int skill,ProfileAtomicSaveResultV1 failure){Before=before;Candidate=candidate;Choice=choice;Consent=consent;Skill=skill;OriginalFailure=failure;CurrentFailure=failure;}
            internal readonly ProfileCanonicalDocumentV1 Before,Candidate;
            internal readonly int Choice,Consent,Skill;
            internal readonly ProfileAtomicSaveResultV1 OriginalFailure;
            internal ProfileAtomicSaveResultV1 CurrentFailure;
        }
        private PendingChoiceRetry _pendingChoiceRetry;
        internal Chapter1DevelopmentProfile(Chapter1HubLifetimeOwner hub)
        {
            if(hub==null || hub.Adapter==null || !hub.Adapter.CurrentReceipt.HasValue)throw new InvalidOperationException("실제 prepared 거점이 필요합니다.");
            var receipt=hub.Adapter.CurrentReceipt.Value;receipt.Validate();
            _root=ValidateExactRoot(hub.Adapter.GetAuthenticatedLaunchRootForNewGame(hub.Router));
            if(receipt.PersistencePending)throw new InvalidOperationException("시작 저장이 확정되지 않았습니다.");
            var observed=ProfileLaunchObservationAdapterV1.Observe(_root);observed.Validate();
            var plan=ProfileLoadSelectorV1.Select(observed.Primary,observed.Previous,observed.Temp);plan.Validate();
            // 실제 prepared owner가 복구·보존·초기 저장을 끝내야 한다. 별도 복구 권위를 만들지 않는다.
            if(plan.RequiresAtomicSave || plan.PrimaryPreservation!=ProfileFilePreservationReason.None ||
                plan.PreviousPreservation!=ProfileFilePreservationReason.None || plan.TempPreservation!=ProfileFilePreservationReason.None ||
                !Bytes(plan.InMemoryDocument.CanonicalFileBytes,receipt.FinalDocument.CanonicalFileBytes))
                throw new InvalidOperationException("prepared 저장과 재관측 결과가 일치하지 않습니다. 실제 시작 복구가 필요합니다.");
            _document=RoundTrip(plan.InMemoryDocument.Snapshot);_ready=true;
        }
        internal ProfileSnapshotV1 Snapshot {get{if(!_ready)throw new InvalidOperationException("개발 저장이 중단되었습니다.");return _document.Snapshot;}}
        internal ProfileAtomicSaveResultV1 SaveForExpedition(int seed,bool explicitValidation)
        {
            if(seed!=101 && seed!=202 && seed!=303 && seed!=404)throw new ArgumentException("미승인 seed입니다.");
            return SavePreserved(explicitValidation?Snapshot.Progression.LastOfferedSeed:seed);
        }
        internal ProfileAtomicSaveResultV1 SaveAfterFailure()=>SavePreserved(Snapshot.Progression.LastOfferedSeed);
        internal ProfileAtomicSaveResultV1 SaveChapter1Choice(int choice,int consent,int skill)
        {
            if(_pendingChoiceRetry!=null)throw new InvalidOperationException("보관된 선택 실패는 새 재시도 입력으로만 처리합니다.");
            var before=Snapshot;var p=before.Progression;
            if(p.TryGetCommittedPair(out var existing) && ((int)existing.Choice!=choice || (int)existing.Skill!=skill || (int)p.ConsentState!=consent))
                throw new InvalidOperationException("확정된 선택 조합을 바꿀 수 없습니다.");
            var next=new ProfileProgressionSnapshot(checked(p.ProfileRevision+1),p.LastOfferedSeed,(ProfileChoice)choice,(ProfileConsentState)consent,(ProfileSkill)skill,p.CompletedBranches);
            var candidate=RoundTrip(new ProfileSnapshotV1(before.Settings,before.Input,before.Tutorial,next));var original=RoundTrip(before);
            var result=SaveProgression(before,next);
            if(result.Outcome==ProfileAtomicSaveOutcome.FailedBeforeCommit)_pendingChoiceRetry=new PendingChoiceRetry(original,candidate,choice,consent,skill,result);
            return result;
        }
        internal ProfileAtomicSaveResultV1 RetryChapter1Choice(int choice,int consent,int skill,out ProfileSnapshotV1 before,out string status)
        {
            var pending=_pendingChoiceRetry;if(pending==null || pending.Choice!=choice || pending.Consent!=consent || pending.Skill!=skill)throw new InvalidOperationException("현재 원래 선택의 재시도 후보가 없습니다.");
            pending.OriginalFailure.Validate();pending.CurrentFailure.Validate();
            if(pending.OriginalFailure.Outcome!=ProfileAtomicSaveOutcome.FailedBeforeCommit || pending.CurrentFailure.Outcome!=ProfileAtomicSaveOutcome.FailedBeforeCommit)throw new InvalidOperationException("불확실 저장은 이 경로로 재시도하지 않습니다.");
            before=pending.Before.Snapshot;
            var attempt=DesktopProfileLaunchAdapterV1.RetryChapter1ChoiceSave(ValidateExactRoot(_root),pending.Before,pending.Candidate,choice,consent,skill,pending.CurrentFailure);
            status=attempt.Disposition+":"+attempt.Reason;
            if(attempt.Disposition==DesktopProfileLaunchAdapterV1.Chapter1ChoiceRetryDisposition.AlreadyCommittedObserved)throw new InvalidOperationException("이미 저장된 후보의 중복 쓰기와 완료를 거절했습니다.");
            if(!attempt.Save.HasValue)return pending.CurrentFailure; // 이번 저장 미호출: 원래 실제 실패와 status를 분리한다.
            var result=attempt.Save.Value;result.Validate();
            if(IsExactCommit(result,pending.Candidate.Snapshot.ProfileRevision)){_document=pending.Candidate;_ready=true;_pendingChoiceRetry=null;}
            else{_ready=false;pending.CurrentFailure=result;}
            return result;
        }
        internal ProfileAtomicSaveResultV1 SaveChapter1CompletedBranch(ProfileBranch branch)
        {
            var before=Snapshot;var p=before.Progression;
            if(!p.TryGetCommittedPair(out var pair) || (int)pair.Choice!=(int)branch)throw new InvalidOperationException("현재 확정 선택과 완료 분기가 다릅니다.");
            var branches=new System.Collections.Generic.List<ProfileBranch>();foreach(var value in p.CompletedBranches)branches.Add(value);
            if(!branches.Contains(branch))branches.Add(branch);branches.Sort((a,b)=>((int)a).CompareTo((int)b));
            return SaveProgression(before,new ProfileProgressionSnapshot(checked(p.ProfileRevision+1),p.LastOfferedSeed,p.CommittedChoice,p.ConsentState,p.GrantedSkill,branches));
        }
        private ProfileAtomicSaveResultV1 SaveProgression(ProfileSnapshotV1 before,ProfileProgressionSnapshot progression)
        {
            ValidateExactRoot(_root);var document=RoundTrip(new ProfileSnapshotV1(before.Settings,before.Input,before.Tutorial,progression));
            var result=ProfileAtomicSaveServiceV1.Save(_root,document);result.Validate();
            if(IsExactCommit(result,progression.ProfileRevision))_document=document;
            else
            {
                _ready=false;
                if(result.Outcome==ProfileAtomicSaveOutcome.FailedBeforeCommit)
                {
                    var observed=ProfileLaunchObservationAdapterV1.Observe(_root);observed.Validate();
                    var plan=ProfileLoadSelectorV1.Select(observed.Primary,observed.Previous,observed.Temp);plan.Validate();
                    if(!plan.RequiresAtomicSave && plan.PrimaryPreservation==ProfileFilePreservationReason.None && plan.PreviousPreservation==ProfileFilePreservationReason.None && plan.TempPreservation==ProfileFilePreservationReason.None &&
                        Bytes(plan.InMemoryDocument.CanonicalFileBytes,_document.CanonicalFileBytes))_ready=true;
                }
            }
            return result;
        }
        private ProfileAtomicSaveResultV1 SavePreserved(int? lastSeed)
        {
            var before=Snapshot;var p=before.Progression;p.Validate();ValidateExactRoot(_root);
            var expected=checked(p.ProfileRevision+1);
            var next=new ProfileProgressionSnapshot(expected,lastSeed,p.CommittedChoice,p.ConsentState,p.GrantedSkill,p.CompletedBranches);
            var document=RoundTrip(new ProfileSnapshotV1(before.Settings,before.Input,before.Tutorial,next));
            var result=ProfileAtomicSaveServiceV1.Save(_root,document);result.Validate();
            if((result.Outcome==ProfileAtomicSaveOutcome.CommittedFirst || result.Outcome==ProfileAtomicSaveOutcome.CommittedReplacement) && result.CommittedRevision==expected)
                _document=document;
            else _ready=false; // 불확실 저장을 성공으로 간주하거나 같은 요청을 자동 재실행하지 않는다.
            return result;
        }
        internal static bool IsExactCommit(ProfileAtomicSaveResultV1 result,long expected)
        {result.Validate();return (result.Outcome==ProfileAtomicSaveOutcome.CommittedFirst || result.Outcome==ProfileAtomicSaveOutcome.CommittedReplacement) && result.CommittedRevision==expected;}
        internal static string ValidateExactRoot(string candidate)
        {
            if(string.IsNullOrWhiteSpace(candidate) || !Path.IsPathFullyQualified(candidate))throw new ArgumentException("개발 root는 지정 절대 경로여야 합니다.");
            var root=Path.GetFullPath(candidate).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);
            if(!string.Equals(root,ExactRoot,StringComparison.OrdinalIgnoreCase))throw new ArgumentException("지정 개발 root 외 경로는 허용하지 않습니다.");
            // 사용자 profile fallback과 링크 경유를 모두 닫는다. 실제 저장 API의 권위 관문도 유지한다.
            for(var current=root; !string.IsNullOrEmpty(current); current=Path.GetDirectoryName(current))
                if(Directory.Exists(current) && (File.GetAttributes(current)&FileAttributes.ReparsePoint)!=0)throw new IOException("개발 root의 링크 경유는 허용하지 않습니다.");
            return root;
        }
        private static ProfileCanonicalDocumentV1 RoundTrip(ProfileSnapshotV1 snapshot)
        {
            snapshot.Validate();snapshot.Progression.Validate();if(snapshot.Progression.TryGetCommittedPair(out var pair))pair.Validate();
            var encoded=ProfileCanonicalEncoder.Encode(snapshot);encoded.Validate();
            var decoded=ProfileCanonicalDecoderV1.Decode(encoded.CanonicalFileBytes);decoded.Validate();
            if(decoded.Classification!=ProfileCanonicalDecodeClassification.ValidCurrentInput || !Bytes(encoded.CanonicalFileBytes,decoded.Document.CanonicalFileBytes))
                throw new InvalidOperationException("개발 저장 V1 원문 왕복이 일치하지 않습니다.");
            return decoded.Document;
        }
        private static bool Bytes(byte[] a,byte[] b){if(a.Length!=b.Length)return false;for(var i=0;i<a.Length;i++)if(a[i]!=b[i])return false;return true;}
    }
}
