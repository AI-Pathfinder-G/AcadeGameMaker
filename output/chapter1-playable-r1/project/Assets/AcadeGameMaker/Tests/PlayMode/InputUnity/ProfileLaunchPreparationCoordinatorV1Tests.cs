using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AcadeGameMaker.Input;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using NUnit.Framework;
using UnityEngine.InputSystem;

namespace AcadeGameMaker.Tests.PlayMode.InputUnity
{
    // REQ-M5D7L-001..008; representative real-filesystem/generated-actions coverage for AC-M5D7L-001..011.
    internal sealed class ProfileLaunchPreparationCoordinatorV1Tests
    {
        // REQ-AUTOCH1: 공개 시험 lifecycle로만 재생에 진입하고 장면 파일은 저장하지 않는다.
        [SerializeField] private string[] _savedScenePaths;
        [SerializeField] private bool[] _savedSceneLoaded;
        [SerializeField] private bool[] _savedSceneActive;
        [SerializeField] private bool _ownsRuntimeFixture;
        [SerializeField] private bool _savedEmptyBatchScene;
        [SerializeField] private bool _ownedBatchFixture;
        [SerializeField] private string _ownedBatchRunId;
        private Scene _ownedCaseScene;
        private Scene _previousCaseScene;
        private bool ValidateOwnedBatchStart(SceneSetup[] setup)
        {
            if(setup.Length!=0)return false;
            var args=Environment.GetCommandLineArgs();var marked=Array.IndexOf(args,"-autonomousOwnedBatchFixture")>=0;
            if(!marked)return false;
            Assert.That(Application.isBatchMode,Is.True,"소유 배치 표식은 대화형 편집기에 적용하지 않습니다.");
            var index=Array.IndexOf(args,"-autonomousOwnedBatchRunId");Assert.That(index>=0 && index+1<args.Length,Is.True);_ownedBatchRunId=args[index+1];
            Assert.That(_ownedBatchRunId.Length,Is.InRange(1,128));foreach(var character in _ownedBatchRunId)Assert.That(char.IsLetterOrDigit(character)||character=='-'||character=='_',Is.True);
            var project=Path.GetFullPath(Path.Combine(Application.dataPath,"..")).TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);
            var expected=Path.GetFullPath("H:/GPT-workspace/AcadeGameMaker/output/chapter1-playable-r1/project").TrimEnd(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);
            Assert.That(string.Equals(project,expected,StringComparison.OrdinalIgnoreCase),Is.True,"정확 승인 프로젝트가 아닙니다.");
            using(var process=System.Diagnostics.Process.GetCurrentProcess())Debug.Log("[CH1Fixture] branch=owned-batch-before runId="+_ownedBatchRunId+" marked="+marked+" batch="+Application.isBatchMode+" project="+project+" pid="+process.Id+" birthFileTime="+process.StartTime.ToFileTimeUtc().ToString(System.Globalization.CultureInfo.InvariantCulture)+" setupCount="+setup.Length+" sceneCount="+SceneManager.sceneCount);
            Assert.That(SceneManager.sceneCount,Is.EqualTo(1));var scene=SceneManager.GetSceneAt(0);
            Debug.Log("[CH1Fixture] owned-start name="+scene.name+" path="+scene.path+" valid="+scene.IsValid()+" loaded="+scene.isLoaded+" active="+(SceneManager.GetActiveScene()==scene)+" dirty="+scene.isDirty+" roots="+scene.rootCount);
            Assert.That(scene.IsValid() && scene.isLoaded && SceneManager.GetActiveScene()==scene && string.IsNullOrEmpty(scene.path) && !scene.isDirty,Is.True);
            var roots=scene.GetRootGameObjects();Assert.That(roots.Length,Is.EqualTo(2));
            foreach(var root in roots)
            {
                var components=root.GetComponents<Component>();Assert.That(components.Length,Is.LessThanOrEqualTo(8));var raw=new StringBuilder();
                raw.Append("[CH1Fixture] owned-root name=").Append(root.name.Length>1024?root.name.Substring(0,1024):root.name).Append(" nameTruncated=").Append(root.name.Length>1024).Append(" hideFlags=").Append(root.hideFlags).Append(" childCount=").Append(root.transform.childCount).Append(" componentCount=").Append(components.Length);
                foreach(var component in components)raw.Append(" type=").Append(component==null?"<missing>":component.GetType().FullName);
                Assert.That(Encoding.UTF8.GetByteCount(raw.ToString()),Is.LessThanOrEqualTo(8192));Debug.Log(raw.ToString());
                Assert.That(root.transform.childCount,Is.Zero);
                foreach(var component in components)
                {Assert.That(component,Is.Not.Null);var type=component.GetType();Assert.That(component is MonoBehaviour,Is.False);Assert.That(type==typeof(Transform)||type==typeof(UnityEngine.Camera)||type==typeof(AudioListener)||type==typeof(Light),Is.True,"기본 시작 구성 외 컴포넌트입니다.");}
            }
            return true;
        }
        [UnityOneTimeSetUp] public IEnumerator EnterIsolatedRuntimeFixture()
        {
            Assert.That(Application.isPlaying,Is.False,"이미 재생 중인 환경은 이 fixture 소유가 아닙니다.");
            var setup=EditorSceneManager.GetSceneManagerSetup();_ownedBatchFixture=ValidateOwnedBatchStart(setup);if(!_ownedBatchFixture)Assert.That(setup.Length,Is.GreaterThan(0));
            _savedScenePaths=new string[setup.Length];_savedSceneLoaded=new bool[setup.Length];_savedSceneActive=new bool[setup.Length];
            _savedEmptyBatchScene=setup.Length==1 && string.IsNullOrEmpty(setup[0].path);
            if(_savedEmptyBatchScene)
            {
                Assert.That(Application.isBatchMode,Is.True);Assert.That(SceneManager.sceneCount,Is.EqualTo(1));var empty=SceneManager.GetSceneAt(0);
                Debug.Log("[CH1Fixture] branch=empty-before name="+empty.name+" count="+SceneManager.sceneCount+" valid="+empty.IsValid()+" loaded="+empty.isLoaded+" active="+(SceneManager.GetActiveScene()==empty)+" path="+empty.path+" dirty="+empty.isDirty+" roots="+empty.rootCount+" setupLoaded="+setup[0].isLoaded+" setupActive="+setup[0].isActive);
                Assert.That(empty.IsValid() && empty.isLoaded && SceneManager.GetActiveScene()==empty && string.IsNullOrEmpty(empty.path) && !empty.isDirty && empty.rootCount==0,Is.True,"미저장/내용 있는 장면은 빈 배치 예외가 아닙니다.");
            }
            for(var i=0;i<setup.Length && !_savedEmptyBatchScene;i++)
            {
                Assert.That(string.IsNullOrEmpty(setup[i].path),Is.False,"복원 가능한 저장 장면 경로가 필요합니다.");
                Assert.That(setup[i].path.StartsWith("Assets/",StringComparison.Ordinal) && setup[i].path.EndsWith(".unity",StringComparison.OrdinalIgnoreCase),Is.True,"현재 프로젝트의 저장 장면만 복원합니다.");
                Assert.That(UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(setup[i].path),Is.Not.Null,"복원할 SceneAsset가 존재해야 합니다.");
                var scene=SceneManager.GetSceneByPath(setup[i].path);Debug.Log("[CH1Fixture] branch=saved-before index="+i+" count="+setup.Length+" name="+(scene.IsValid()?scene.name:"unloaded")+" path="+setup[i].path+" valid="+scene.IsValid()+" loaded="+scene.isLoaded+" active="+(SceneManager.GetActiveScene()==scene)+" dirty="+(scene.IsValid()?scene.isDirty.ToString():"unobserved")+" roots="+(scene.IsValid()&&scene.isLoaded?scene.rootCount.ToString():"unobserved")+" setupLoaded="+setup[i].isLoaded+" setupActive="+setup[i].isActive);if(scene.IsValid())Assert.That(scene.isDirty,Is.False,"미저장 장면 변경은 폐기하지 않습니다.");
                _savedScenePaths[i]=setup[i].path;_savedSceneLoaded[i]=setup[i].isLoaded;_savedSceneActive[i]=setup[i].isActive;
            }
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);_ownsRuntimeFixture=true;
            yield return new EnterPlayMode();Assert.That(Application.isPlaying,Is.True);
            Debug.Log("[CH1Fixture] runtime-entered production-scenes-isolated");
        }
        [UnitySetUp] public IEnumerator PrepareNamedRuntimeCase()
        {
            Assert.That(Application.isPlaying && _ownsRuntimeFixture,Is.True);
            _previousCaseScene=SceneManager.GetActiveScene();
            _ownedCaseScene=SceneManager.CreateScene("CH1-proof-case-"+Guid.NewGuid().ToString("N"));
            Assert.That(_ownedCaseScene.IsValid() && _ownedCaseScene.isLoaded && !string.IsNullOrEmpty(_ownedCaseScene.name),Is.True);
            Assert.That(SceneManager.SetActiveScene(_ownedCaseScene),Is.True);yield return null;
            Assert.That(SceneManager.GetActiveScene(),Is.EqualTo(_ownedCaseScene));Debug.Log("[CH1Fixture] named-case="+_ownedCaseScene.name);
        }
        [UnityTearDown] public IEnumerator CloseNamedRuntimeCase()
        {
            Assert.That(Application.isPlaying,Is.True,"원래 Dispose는 재생 상태에서 완료되어야 합니다.");yield return null;
            if(_previousCaseScene.IsValid() && _previousCaseScene.isLoaded)Assert.That(SceneManager.SetActiveScene(_previousCaseScene),Is.True);
            if(_ownedCaseScene.IsValid() && _ownedCaseScene.isLoaded){var operation=SceneManager.UnloadSceneAsync(_ownedCaseScene);Assert.That(operation,Is.Not.Null);yield return operation;Assert.That(_ownedCaseScene.isLoaded,Is.False);}
            yield return null;Debug.Log("[CH1Fixture] named-case-unloaded delayed-destroy-observed");
        }
        [UnityOneTimeTearDown] public IEnumerator ExitAndRestoreRuntimeFixture()
        {
            if(!_ownsRuntimeFixture)yield break;
            if(Application.isPlaying)yield return new ExitPlayMode();Assert.That(Application.isPlaying,Is.False);
            Assert.That(_savedScenePaths,Is.Not.Null);if(!_ownedBatchFixture)Assert.That(_savedScenePaths.Length,Is.GreaterThan(0));
            var setup=new SceneSetup[_savedScenePaths.Length];for(var i=0;i<setup.Length;i++)setup[i]=new SceneSetup{path=_savedScenePaths[i],isLoaded=_savedSceneLoaded[i],isActive=_savedSceneActive[i]};
            if(_ownedBatchFixture)
            {
                var empty=SceneManager.GetActiveScene();Debug.Log("[CH1Fixture] branch=owned-batch-exited runId="+_ownedBatchRunId+" sceneCount="+SceneManager.sceneCount+" name="+empty.name+" path="+empty.path+" valid="+empty.IsValid()+" loaded="+empty.isLoaded+" active="+(SceneManager.GetActiveScene()==empty)+" dirty="+empty.isDirty+" roots="+empty.rootCount+" originalRootZeroRestored=false");
                Assert.That(SceneManager.sceneCount==1 && empty.IsValid() && empty.isLoaded && string.IsNullOrEmpty(empty.path) && !empty.isDirty && empty.rootCount==0,Is.True,"소유 시험의 빈 종료 장면이 아닙니다.");
                Debug.Log("[CH1Fixture] owned-batch-default-reconstruction-begin runId="+_ownedBatchRunId);
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects,NewSceneMode.Single);
                var reconstructed=EditorSceneManager.GetSceneManagerSetup();Assert.That(reconstructed.Length,Is.Zero);
                Assert.That(ValidateOwnedBatchStart(reconstructed),Is.True,"다음 fixture용 기본 장면 재구성 검증이 실패했습니다.");
                Debug.Log("[CH1Fixture] owned-batch-default-reconstruction-verified runId="+_ownedBatchRunId+" historicalSceneIdentityRestored=false");
            }
            else if(_savedEmptyBatchScene)
            {
                var empty=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                Debug.Log("[CH1Fixture] branch=empty-restored name="+empty.name+" count="+SceneManager.sceneCount+" valid="+empty.IsValid()+" loaded="+empty.isLoaded+" active="+(SceneManager.GetActiveScene()==empty)+" path="+empty.path+" dirty="+empty.isDirty+" roots="+empty.rootCount+" setupLoaded="+EditorSceneManager.GetSceneManagerSetup()[0].isLoaded+" setupActive="+EditorSceneManager.GetSceneManagerSetup()[0].isActive);
                Assert.That(SceneManager.sceneCount==1 && empty.IsValid() && empty.isLoaded && SceneManager.GetActiveScene()==empty && string.IsNullOrEmpty(empty.path) && !empty.isDirty && empty.rootCount==0,Is.True);
            }
            else
            {
                EditorSceneManager.RestoreSceneManagerSetup(setup);var actual=EditorSceneManager.GetSceneManagerSetup();Assert.That(actual.Length,Is.EqualTo(setup.Length));
                for(var i=0;i<setup.Length;i++){var restored=SceneManager.GetSceneByPath(actual[i].path);Debug.Log("[CH1Fixture] branch=saved-restored index="+i+" count="+actual.Length+" name="+(restored.IsValid()?restored.name:"unloaded")+" path="+actual[i].path+" valid="+restored.IsValid()+" loaded="+restored.isLoaded+" active="+(SceneManager.GetActiveScene()==restored)+" dirty="+(restored.IsValid()?restored.isDirty.ToString():"unobserved")+" roots="+(restored.IsValid()&&restored.isLoaded?restored.rootCount.ToString():"unobserved")+" setupLoaded="+actual[i].isLoaded+" setupActive="+actual[i].isActive);Assert.That(actual[i].path,Is.EqualTo(setup[i].path));Assert.That(actual[i].isLoaded,Is.EqualTo(setup[i].isLoaded));Assert.That(actual[i].isActive,Is.EqualTo(setup[i].isActive));}
            }
            _ownsRuntimeFixture=false;Debug.Log("[CH1Fixture] runtime-exited ownedBatch="+_ownedBatchFixture+" originalSceneSetupRestored="+(!_ownedBatchFixture));
        }

        private static readonly DateTimeOffset Utc = new DateTimeOffset(2026, 9, 13, 11, 12, 13, TimeSpan.Zero);

        [Test]
        public void AC001_DefaultBootstrapPersistsAndTransfersOneDisabledCollection()
        {
            WithRoot(root =>
            {
                var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root + Path.DirectorySeparatorChar, Utc);
                Assert.That(prepared.Source, Is.EqualTo(ProfileLaunchSource.Default)); Assert.That(prepared.SaveKind, Is.EqualTo(ProfileLaunchSaveKindV1.Ordinary)); Assert.That(prepared.PersistenceOutcome, Is.EqualTo(ProfileLaunchPersistenceOutcomeV1.CommittedFirst)); Assert.That(prepared.HubEntryAllowed, Is.True); Assert.That(prepared.PersistencePending, Is.False);
                var actions = prepared.TakeActions(); Assert.That(actions.asset.enabled, Is.False); prepared.Dispose(); Assert.Throws<InvalidOperationException>(() => prepared.TakeActions()); actions.Dispose();
                Assert.That(File.Exists(Path.Combine(root, "profile.json")), Is.True);
            });
        }

        [Test]
        public void AC001_CurrentEmptyPrimaryNeedsNoSaveAndDisposesBeforeTransfer()
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), Current(7, string.Empty).CanonicalFileBytes);
                var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc);
                Assert.That(prepared.Source, Is.EqualTo(ProfileLaunchSource.Primary)); Assert.That(prepared.SourceRevision, Is.EqualTo(7)); Assert.That(prepared.FinalRevision, Is.EqualTo(7)); Assert.That(prepared.SaveKind, Is.EqualTo(ProfileLaunchSaveKindV1.None)); Assert.That(prepared.PersistenceOutcome, Is.EqualTo(ProfileLaunchPersistenceOutcomeV1.NotRequired));
                prepared.Dispose(); prepared.Dispose(); Assert.Throws<InvalidOperationException>(() => prepared.TakeActions());
            });
        }

        [Test]
        public void AC003_RealStaleBindingRecoveryRepairsOnceAndUsesOrdinarySave()
        {
            WithRoot(root =>
            {
                File.WriteAllBytes(Path.Combine(root, "profile.json"), Current(3, StaleBindingOverride()).CanonicalFileBytes);
                var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc);
                Assert.That(prepared.Source, Is.EqualTo(ProfileLaunchSource.Primary)); Assert.That(prepared.FinalRevision, Is.EqualTo(4)); Assert.That(prepared.FinalRecoveryReasons, Is.EqualTo(ProfileRecoveryPlanReason.InputRepair)); Assert.That(prepared.SaveKind, Is.EqualTo(ProfileLaunchSaveKindV1.Ordinary)); Assert.That(prepared.PersistenceOutcome, Is.EqualTo(ProfileLaunchPersistenceOutcomeV1.CommittedReplacement));
                using (var actions = prepared.TakeActions()) Assert.That(actions.asset.enabled, Is.False);
            });
        }

        [Test]
        public void AC002_DeterministicPrimaryDecodeRecoveryDispatchesOnlyH()
        {
            WithRoot(root =>
            {
                var primary = Candidate(ProfileLoadFileRole.Primary, Document(8, "obsolete", string.Empty));
                var observation = new ProfileLaunchObservationProjectionV1(primary, ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)); var selection = ProfileLoadSelectorV1.Select(observation.Primary, observation.Previous, observation.Temp);
                var port = new CoordinatorPort(observation, null, new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.PrimaryInputRecovery, ProfileLaunchPersistenceOutcomeV1.CommittedPrimaryInputRecovery, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.ValidInputRecoveryRequired, ProfileStoredFileState.Missing, 9));
                var actions = new ActionsPort(); using (var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root + Path.DirectorySeparatorChar, Utc, port, actions)) { Assert.That(port.Calls, Is.EqualTo("G,K,H")); Assert.That(port.KRoot, Is.EqualTo(Path.GetFullPath(root))); Assert.That(port.KUtc, Is.EqualTo(Utc)); AssertObservationEqual(observation, port.KObservation); AssertSelectionEqual(selection, port.KSelection); AssertCandidateEqual(primary, port.HPrimary); Assert.That(port.HRoot, Is.EqualTo(Path.GetFullPath(root))); Assert.That(actions.AppliedCandidates[0], Is.SameAs(actions.Created[0])); AssertInputEqual(selection.InMemoryDocument.Snapshot.Input, actions.AppliedInputs[0]); Assert.That(prepared.SaveKind, Is.EqualTo(ProfileLaunchSaveKindV1.PrimaryInputRecovery)); Assert.That(prepared.CommittedRevision, Is.EqualTo(9)); }
            });
        }

        [Test]
        public void AC002_AC004_AC005_PreviousPromotionAndBlockedOrFailedSaveRemainTyped()
        {
            WithRoot(root =>
            {
                var previous = Candidate(ProfileLoadFileRole.Previous, Document(4, ProfileInputContract.CurrentInputActionsAssetId, string.Empty));
                var ordinary = new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, 5);
                var promotionObservation = new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary), previous, ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)); var promotionSelection = ProfileLoadSelectorV1.Select(promotionObservation.Primary, promotionObservation.Previous, promotionObservation.Temp); var promotionActions = new ActionsPort(); var promote = new CoordinatorPort(promotionObservation, null, ordinary); using (var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, promote, promotionActions)) { Assert.That(promote.Calls, Is.EqualTo("G,K,D")); Assert.That(prepared.Source, Is.EqualTo(ProfileLaunchSource.Previous)); Assert.That(promote.DRoot, Is.EqualTo(Path.GetFullPath(root))); AssertObservationEqual(promotionObservation, promote.KObservation); AssertSelectionEqual(promotionSelection, promote.KSelection); AssertDocumentEqual(promotionSelection.InMemoryDocument, promote.DDocument); Assert.That(promotionActions.AppliedCandidates[0], Is.SameAs(promotionActions.Created[0])); AssertInputEqual(promotionSelection.InMemoryDocument.Snapshot.Input, promotionActions.AppliedInputs[0]); }

                var blocked = new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.FailedBeforeMove, ProfileLaunchPreservationStateV1.NotRequired, false, ProfileLoadFileRole.Primary, ProfileFilePreservationReason.None, ProfileFilePreservationReason.UnreadablePrimary, ProfileFilePreservationReason.None, 1);
                var denied = new CoordinatorPort(new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Unreadable(ProfileLoadFileRole.Primary), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)), blocked, ordinary); using (var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, denied, new ActionsPort())) { Assert.That(denied.Calls, Is.EqualTo("G,K")); Assert.That(prepared.PersistenceOutcome, Is.EqualTo(ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation)); Assert.That(prepared.PersistencePending, Is.True); }

                var failed = new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, ProfileAtomicSaveStage.DirectoryPreparation, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, null);
                var failedPort = new CoordinatorPort(Empty(), null, failed); using (var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, failedPort, new ActionsPort())) { Assert.That(failedPort.Calls, Is.EqualTo("G,K,D")); Assert.That(prepared.PersistencePending, Is.True); Assert.That(prepared.HubEntryAllowed, Is.True); }
            });
        }

        [Test]
        public void AC003_AC006_AC008_RecoveryDisposesPoisonedCandidateAndRetainsBothBindingProofs()
        {
            WithRoot(root =>
            {
                var primary = Candidate(ProfileLoadFileRole.Primary, Document(2, ProfileInputContract.CurrentInputActionsAssetId, "[]"));
                var save = new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedReplacement, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, 3);
                var port = new CoordinatorPort(new ProfileLaunchObservationProjectionV1(primary, ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)), null, save);
                var actions = new ActionsPort { First = new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired, ProfileBindingOverrideApplyFailureV1.LoadFailed, ProfileBindingOverrideCandidateDispositionV1.Discard, "[]", null) };
                var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, port, actions);
                Assert.That(port.Calls, Is.EqualTo("G,K,D")); Assert.That(actions.Created.Count, Is.EqualTo(2)); Assert.That(actions.AppliedCandidates[0], Is.SameAs(actions.Created[0])); Assert.That(actions.AppliedCandidates[1], Is.SameAs(actions.Created[1])); AssertInputEqual(Document(2, ProfileInputContract.CurrentInputActionsAssetId, "[]").Snapshot.Input, actions.AppliedInputs[0]); Assert.That(actions.AppliedInputs[1].BindingOverrides.IsEmpty, Is.True); Assert.That(actions.Disposed.Count, Is.EqualTo(1)); Assert.That(actions.Disposed[0], Is.SameAs(actions.Created[0])); Assert.That(prepared.InitialBindingOutcome, Is.EqualTo(ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired)); Assert.That(prepared.FinalBindingOutcome, Is.EqualTo(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady)); Assert.That(prepared.FinalRevision, Is.EqualTo(3));
                prepared.Dispose(); Assert.That(actions.Disposed.Count, Is.EqualTo(2)); Assert.That(actions.Disposed[1], Is.SameAs(actions.Created[1])); prepared.Dispose(); Assert.Throws<InvalidOperationException>(() => prepared.TakeActions());
            });
        }

        [Test]
        public void AC007_PreservationProjectionMustMatchRecomputedSelection()
        {
            WithRoot(root =>
            {
                var invalidTemp = ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Temp, ProfileCanonicalDecoderV1.Decode(Encoding.UTF8.GetBytes("{}")));
                var wrong = new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, true, null, ProfileFilePreservationReason.None, ProfileFilePreservationReason.None, ProfileFilePreservationReason.None, 0);
                var port = new CoordinatorPort(new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), invalidTemp), wrong, new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, 0)); var actions = new ActionsPort();
                Assert.Throws<InvalidOperationException>(() => ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, port, actions)); Assert.That(port.Calls, Is.EqualTo("G,K")); Assert.That(actions.Created, Is.Empty);
            });
        }

        [Test]
        public void AC007_SaveKindMismatchAndDefaultProjectionsFailClosed()
        {
            Assert.Throws<InvalidOperationException>(() => default(ProfileLaunchSaveProjectionV1).Validate()); Assert.Throws<InvalidOperationException>(() => default(ProfileLaunchPreservationProjectionV1).Validate());
            WithRoot(root =>
            {
                var primary = Candidate(ProfileLoadFileRole.Primary, Document(1, "obsolete", string.Empty));
                var wrong = new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, 2);
                var port = new CoordinatorPort(new ProfileLaunchObservationProjectionV1(primary, ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)), null, wrong); var actions = new ActionsPort();
                Assert.Throws<InvalidOperationException>(() => ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, port, actions)); Assert.That(port.Calls, Is.EqualTo("G,K,H")); Assert.That(actions.Disposed.Count, Is.EqualTo(1));
            });
        }

        [Test]
        public void AC007_ApplyExceptionCleansExactlyOnceAndLaterCallIsIndependent()
        {
            WithRoot(root =>
            {
                var failing = new ActionsPort { ApplyError = new NotSupportedException("fault") }; var profile = new CoordinatorPort(Empty(), null, new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, 0));
                Assert.Throws<NotSupportedException>(() => ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, profile, failing)); Assert.That(failing.Created.Count, Is.EqualTo(1)); Assert.That(failing.Disposed.Count, Is.EqualTo(1));
                using (var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, new CoordinatorPort(Empty(), null, new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, 0)), new ActionsPort())) Assert.That(prepared.HubEntryAllowed, Is.True);
            });
        }

        [Test]
        public void AC006_WrongApplyTextSuppressesSaveAndDisposesCandidate()
        {
            WithRoot(root =>
            {
                var primary = Candidate(ProfileLoadFileRole.Primary, Document(2, ProfileInputContract.CurrentInputActionsAssetId, "[]")); var profile = new CoordinatorPort(new ProfileLaunchObservationProjectionV1(primary, ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)), null, new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, 2)); var actions = new ActionsPort { First = new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady, ProfileBindingOverrideApplyFailureV1.None, ProfileBindingOverrideCandidateDispositionV1.Retain, string.Empty, string.Empty) };
                Assert.Throws<InvalidOperationException>(() => ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, profile, actions)); Assert.That(profile.Calls, Is.EqualTo("G,K")); Assert.That(actions.Disposed.Count, Is.EqualTo(1));
            });
        }

        [Test]
        public void AC006_KFailureSuppressesApplyAndEverySave()
        {
            WithRoot(root =>
            {
                var profile = new CoordinatorPort(Empty(), null, new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, 0)) { PreserveError = new NotSupportedException("K") }; var actions = new ActionsPort();
                Assert.Throws<NotSupportedException>(() => ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, profile, actions)); Assert.That(profile.Calls, Is.EqualTo("G,K")); Assert.That(actions.Created, Is.Empty);
            });
        }

        [Test]
        public void AC008_TakeTransfersWithoutTokenDisposalThenClosesOnOwnerDispose()
        {
            WithRoot(root =>
            {
                var actions = new ActionsPort(); var profile = new CoordinatorPort(Empty(), null, new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, 0)); var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, profile, actions); var taken = prepared.TakeActions(); prepared.Dispose(); Assert.That(actions.Disposed, Is.Empty); Assert.Throws<InvalidOperationException>(() => prepared.TakeActions()); actions.Dispose(taken);
            });
        }

        [TestCaseSource(nameof(BlockedRoles))]
        public void AC004_EachPreservationStopBlocksOnlyPersistence(ProfileLoadFileRole stoppedRole)
        {
            WithRoot(root =>
            {
                ProfileLaunchObservationProjectionV1 observation; ProfileLaunchPreservationProjectionV1 preservation;
                if (stoppedRole == ProfileLoadFileRole.Temp) { var temp = ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Temp, ProfileCanonicalDecoderV1.Decode(Encoding.UTF8.GetBytes("{}"))); observation = new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), temp); preservation = new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.FailedBeforeMove, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, false, stoppedRole, ProfileFilePreservationReason.StaleTemp, ProfileFilePreservationReason.None, ProfileFilePreservationReason.None, 1); }
                else if (stoppedRole == ProfileLoadFileRole.Primary) { observation = new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Unreadable(ProfileLoadFileRole.Primary), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)); preservation = new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.FailedBeforeMove, ProfileLaunchPreservationStateV1.NotRequired, false, stoppedRole, ProfileFilePreservationReason.None, ProfileFilePreservationReason.UnreadablePrimary, ProfileFilePreservationReason.None, 1); }
                else { observation = new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary), ProfileLoadCandidateV1.Unreadable(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)); preservation = new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.FailedBeforeMove, false, stoppedRole, ProfileFilePreservationReason.None, ProfileFilePreservationReason.None, ProfileFilePreservationReason.UnreadablePrevious, 1); }
                var port = new CoordinatorPort(observation, preservation, OrdinaryFirst(0)); var actions = new ActionsPort(); using (var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, port, actions)) { Assert.That(port.Calls, Is.EqualTo("G,K")); Assert.That(actions.Created.Count, Is.EqualTo(1)); Assert.That(prepared.PersistenceOutcome, Is.EqualTo(ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation)); Assert.That(prepared.Preservation.StoppedRole, Is.EqualTo(stoppedRole)); Assert.That(prepared.PersistencePending, Is.True); }
            });
        }

        [Test]
        public void AC004_DirectPrimaryWithFailedStaleTempStaysNotRequired()
        {
            WithRoot(root =>
            {
                var current = Candidate(ProfileLoadFileRole.Primary, Document(1, ProfileInputContract.CurrentInputActionsAssetId, string.Empty)); var temp = ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Temp, ProfileCanonicalDecoderV1.Decode(Encoding.UTF8.GetBytes("{}"))); var preservation = new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.MoveOutcomeUncertain, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, false, ProfileLoadFileRole.Temp, ProfileFilePreservationReason.StaleTemp, ProfileFilePreservationReason.None, ProfileFilePreservationReason.None, 1); var port = new CoordinatorPort(new ProfileLaunchObservationProjectionV1(current, ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), temp), preservation, OrdinaryFirst(1)); using (var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, port, new ActionsPort())) { Assert.That(port.Calls, Is.EqualTo("G,K")); Assert.That(prepared.PersistenceOutcome, Is.EqualTo(ProfileLaunchPersistenceOutcomeV1.NotRequired)); Assert.That(prepared.PersistencePending, Is.False); }
            });
        }

        [TestCaseSource(nameof(HRecoveryRows))]
        public void AC002_BothPrimaryDecodeRecoveriesDispatchH(ProfileLoadCandidateV1 primary, long revision)
        {
            WithRoot(root =>
            {
                var port = new CoordinatorPort(new ProfileLaunchObservationProjectionV1(primary, ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)), null, new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.PrimaryInputRecovery, ProfileLaunchPersistenceOutcomeV1.CommittedPrimaryInputRecovery, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.ValidInputRecoveryRequired, ProfileStoredFileState.Missing, revision + 1)); using (var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, port, new ActionsPort())) { Assert.That(port.Calls, Is.EqualTo("G,K,H")); Assert.That(prepared.PersistenceOutcome, Is.EqualTo(ProfileLaunchPersistenceOutcomeV1.CommittedPrimaryInputRecovery)); }
            });
        }

        [TestCaseSource(nameof(SaveFailureRows))]
        public void AC005_AllTypedSaveFailureAndUncertaintyRowsRemainHubReady(bool inputRecovery, ProfileLaunchPersistenceOutcomeV1 outcome, ProfileAtomicSaveStage stage)
        {
            WithRoot(root =>
            {
                var primary = inputRecovery ? Candidate(ProfileLoadFileRole.Primary, Document(3, "obsolete", string.Empty)) : ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary); var kind = inputRecovery ? ProfileLaunchSaveKindV1.PrimaryInputRecovery : ProfileLaunchSaveKindV1.Ordinary; var nonProbed = outcome == ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit && !inputRecovery; var save = new ProfileLaunchSaveProjectionV1(kind, outcome, stage, nonProbed ? ProfileStoredFileState.NotProbed : ProfileStoredFileState.Missing, nonProbed ? ProfileStoredFileState.NotProbed : (inputRecovery ? ProfileStoredFileState.ValidInputRecoveryRequired : ProfileStoredFileState.Missing), nonProbed ? ProfileStoredFileState.NotProbed : ProfileStoredFileState.Missing, null); var port = new CoordinatorPort(new ProfileLaunchObservationProjectionV1(primary, ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)), null, save); using (var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, port, new ActionsPort())) { Assert.That(port.Calls, Is.EqualTo(inputRecovery ? "G,K,H" : "G,K,D")); Assert.That(prepared.PersistenceOutcome, Is.EqualTo(outcome)); Assert.That(prepared.PersistenceStage, Is.EqualTo(stage)); Assert.That(prepared.CommittedRevision, Is.Null); Assert.That(prepared.PersistencePending, Is.True); Assert.That(prepared.HubEntryAllowed, Is.True); }
            });
        }

        [Test]
        public void AC007_HostileActionsPortsSuppressSaveAndCleanupExactlyOnce()
        {
            WithRoot(root =>
            {
                foreach (var mode in new[] { "null-first", "enabled-first", "apply-first", "cleanup-after-apply" })
                {
                    var actions = new ActionsPort { NullCreateAt = mode == "null-first" ? 1 : 0, EnableCreateAt = mode == "enabled-first" ? 1 : 0, ApplyError = mode == "apply-first" || mode == "cleanup-after-apply" ? new NotSupportedException("apply") : null, DisposeError = mode == "cleanup-after-apply" ? new InvalidOperationException("cleanup") : null }; var port = new CoordinatorPort(Empty(), null, OrdinaryFirst(0));
                    var error = Assert.Catch<Exception>(() => ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, port, actions)); Assert.That(port.Calls, Is.EqualTo("G,K")); Assert.That(actions.Created.Count, Is.EqualTo(mode == "null-first" ? 0 : 1)); if (mode == "apply-first" || mode == "cleanup-after-apply") { Assert.That(error, Is.TypeOf<NotSupportedException>()); Assert.That(error.Message, Is.EqualTo("apply")); Assert.That(actions.Disposed.Count, Is.EqualTo(1)); Assert.That(actions.Disposed[0], Is.SameAs(actions.Created[0])); }
                }
                var primary = Candidate(ProfileLoadFileRole.Primary, Document(1, ProfileInputContract.CurrentInputActionsAssetId, "[]")); var recover = new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.RecoveryRequired, ProfileBindingOverrideApplyFailureV1.LoadFailed, ProfileBindingOverrideCandidateDispositionV1.Discard, "[]", null);
                foreach (var mode in new[] { "null-second", "enabled-second", "reuse-second", "apply-second", "dispose-poison" })
                {
                    var actions = new ActionsPort { First = recover, NullCreateAt = mode == "null-second" ? 2 : 0, EnableCreateAt = mode == "enabled-second" ? 2 : 0, ReuseFirstAtSecond = mode == "reuse-second", ApplyErrorAt = mode == "apply-second" ? 2 : 0, ApplyError = mode == "apply-second" ? new NotSupportedException("apply") : null, DisposeErrorAt = mode == "dispose-poison" ? 1 : 0, DisposeError = mode == "dispose-poison" ? new InvalidOperationException("dispose") : null }; var port = new CoordinatorPort(new ProfileLaunchObservationProjectionV1(primary, ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp)), null, OrdinaryFirst(2));
                    var error = Assert.Catch<Exception>(() => ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, port, actions), mode); Assert.That(port.Calls, Is.EqualTo("G,K"), mode); Assert.That(actions.Disposed.Count, Is.EqualTo(mode == "enabled-second" || mode == "apply-second" ? 2 : 1), mode); Assert.That(actions.Disposed[0], Is.SameAs(actions.Created[0]), mode); if (mode == "apply-second") { Assert.That(error, Is.TypeOf<NotSupportedException>(), mode); Assert.That(error.Message, Is.EqualTo("apply"), mode); Assert.That(actions.Disposed[1], Is.SameAs(actions.Created[1]), mode); } if (mode == "dispose-poison") { Assert.That(error, Is.TypeOf<InvalidOperationException>(), mode); Assert.That(error.Message, Is.EqualTo("dispose"), mode); }
                }
            });
        }

        [Test]
        public void ValidatedEvidence_MatchesEveryPublicFieldAndRevalidatesEachCall()
        {
            WithRoot(root=>
            {
                using(var prepared=ProfileLaunchPreparationCoordinatorV1.Prepare(root,Utc,new CoordinatorPort(Empty(),null,OrdinaryFirst(0)),new ActionsPort()))
                {
                    var evidence=prepared.ValidateAndReadEvidence();
                    Assert.That(evidence.Source,Is.EqualTo(prepared.Source),"Source");
                    Assert.That(evidence.SourceRevision,Is.EqualTo(prepared.SourceRevision),"SourceRevision");
                    Assert.That(evidence.FinalRevision,Is.EqualTo(prepared.FinalRevision),"FinalRevision");
                    Assert.That(evidence.FinalRecoveryReasons,Is.EqualTo(prepared.FinalRecoveryReasons),"FinalRecoveryReasons");
                    Assert.That(evidence.InitialBindingOutcome,Is.EqualTo(prepared.InitialBindingOutcome),"InitialBindingOutcome");
                    Assert.That(evidence.InitialBindingFailure,Is.EqualTo(prepared.InitialBindingFailure),"InitialBindingFailure");
                    Assert.That(evidence.InitialBindingDisposition,Is.EqualTo(prepared.InitialBindingDisposition),"InitialBindingDisposition");
                    Assert.That(evidence.FinalBindingOutcome,Is.EqualTo(prepared.FinalBindingOutcome),"FinalBindingOutcome");
                    Assert.That(evidence.FinalBindingFailure,Is.EqualTo(prepared.FinalBindingFailure),"FinalBindingFailure");
                    Assert.That(evidence.FinalDocument,Is.EqualTo(prepared.FinalDocument),"FinalDocument");
                    Assert.That(evidence.Preservation,Is.EqualTo(prepared.Preservation),"Preservation");
                    Assert.That(evidence.SaveKind,Is.EqualTo(prepared.SaveKind),"SaveKind");
                    Assert.That(evidence.PersistenceOutcome,Is.EqualTo(prepared.PersistenceOutcome),"PersistenceOutcome");
                    Assert.That(evidence.PersistenceStage,Is.EqualTo(prepared.PersistenceStage),"PersistenceStage");
                    Assert.That(evidence.PrimaryStoredState,Is.EqualTo(prepared.PrimaryStoredState),"PrimaryStoredState");
                    Assert.That(evidence.PreviousStoredState,Is.EqualTo(prepared.PreviousStoredState),"PreviousStoredState");
                    Assert.That(evidence.TempStoredState,Is.EqualTo(prepared.TempStoredState),"TempStoredState");
                    Assert.That(evidence.CommittedRevision,Is.EqualTo(prepared.CommittedRevision),"CommittedRevision");
                    var field=typeof(PreparedProfileLaunchV1).GetField("_finalRevision",BindingFlags.Instance|BindingFlags.NonPublic);var original=field.GetValue(prepared);
                    try{field.SetValue(prepared,checked((long)original+1));Assert.That(()=>prepared.ValidateAndReadEvidence(),Throws.InvalidOperationException);}
                    finally{field.SetValue(prepared,original);}
                }
            });
        }

        [Test]
        public void AC009_ProjectionAndTokenReflectionMutationsFailClosed()
        {
            var observation = Empty(); foreach (var field in new[] { "_primary", "_previous", "_temp" }) AssertInvalid(observation, field, default(ProfileLoadCandidateV1));
            var preservation = new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, true, null, ProfileFilePreservationReason.None, ProfileFilePreservationReason.None, ProfileFilePreservationReason.None, 0); foreach (var field in new[] { "_temp", "_primary", "_previous", "_mayPersist", "_stopped", "_tempIntent", "_primaryIntent", "_previousIntent", "_attempted" }) AssertInvalid(preservation, field, field == "_attempted" ? (object)-1 : field == "_mayPersist" ? (object)false : field == "_stopped" ? (object)ProfileLoadFileRole.Temp : field.EndsWith("Intent", StringComparison.Ordinal) ? (object)ProfileFilePreservationReason.StaleTemp : (object)default(ProfileLaunchPreservationStateV1));
            var save = new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.None, ProfileLaunchPersistenceOutcomeV1.NotRequired, ProfileAtomicSaveStage.None, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, ProfileStoredFileState.NotProbed, null); foreach (var field in new[] { "_kind", "_outcome", "_stage", "_primary", "_previous", "_temp", "_revision" }) AssertInvalid(save, field, field == "_revision" ? (object)(long?)0 : field == "_kind" ? (object)ProfileLaunchSaveKindV1.Ordinary : field == "_outcome" ? (object)ProfileLaunchPersistenceOutcomeV1.BlockedByPreservation : field == "_stage" ? (object)ProfileAtomicSaveStage.CommitCall : (object)ProfileStoredFileState.Missing);
            WithRoot(root => { foreach (var field in new[] { "_source", "_sourceRevision", "_finalRevision", "_document", "_reasons", "_observation", "_initialBinding", "_finalBinding", "_preservation", "_save", "_requiresSave", "_disposer", "_actions", "_ownership" }) { var actions = new ActionsPort(); var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, new CoordinatorPort(Empty(), null, OrdinaryFirst(0)), actions); AssertTokenInvalid(prepared, field, TokenBadValue(field)); actions.Dispose(actions.Created[0]); } });
        }

        [Test]
        public void AC010_LeasesSerializeAliasesAllowDifferentRootsAndReleaseAfterFault()
        {
            WithRoot(root => WithRoot(other =>
            {
                var same = new ObserveFaultPort(1); var first = Task.Run(() => CatchPrepare(root, same)); Assert.That(same.Entered.Wait(2000), Is.True); var second = Task.Run(() => CatchPrepare(root.ToUpperInvariant() + Path.DirectorySeparatorChar, same)); Thread.Sleep(50); Assert.That(same.MaxActive, Is.EqualTo(1)); same.Release.Set(); Assert.That(Task.WaitAll(new[] { first, second }, 2000), Is.True); Assert.That(first.IsCompleted && second.IsCompleted && !first.IsFaulted && !second.IsFaulted, Is.True); Assert.That(same.MaxActive, Is.EqualTo(1));
                var different = new ObserveFaultPort(2); var left = Task.Run(() => CatchPrepare(root, different)); var right = Task.Run(() => CatchPrepare(other, different)); Assert.That(different.Entered.Wait(2000), Is.True); Thread.Sleep(50); Assert.That(different.MaxActive, Is.EqualTo(2)); different.Release.Set(); Assert.That(Task.WaitAll(new[] { left, right }, 2000), Is.True); Assert.That(left.IsCompleted && right.IsCompleted && !left.IsFaulted && !right.IsFaulted, Is.True);
                using (var prepared = ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, new CoordinatorPort(Empty(), null, OrdinaryFirst(0)), new ActionsPort())) Assert.That(prepared.HubEntryAllowed, Is.True);
            }));
        }

        [Test]
        // REQ-M5D7QC-007; partial AC-M5D7QC-007. A present reset barrier
        // wins before ordinary selection can promote an old Previous leaf.
        public void AC_M5D7QC_007_ResetBarrierBeforeLaunchBlocksPreviousWithoutMutation()
        {
            WithRoot(root =>
            {
                var previousPath = Path.Combine(root, "profile.prev.json");
                var barrierPath = Path.Combine(root, "profile.reset.json");
                var oldBytes = Current(17, string.Empty).CanonicalFileBytes;
                File.WriteAllBytes(previousPath, oldBytes); File.WriteAllText(barrierPath, "{");

                var error = Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc));

                Assert.That(error.Outcome, Is.EqualTo(ProfileRootOperationOutcomeV1.ResetBarrierPresent));
                CollectionAssert.AreEqual(oldBytes, File.ReadAllBytes(previousPath));
                Assert.That(File.Exists(Path.Combine(root, "profile.json")), Is.False);
                Assert.That(File.ReadAllText(barrierPath), Is.EqualTo("{"));
            });
        }

        [Test]
        // REQ-M5D7QC-006/007; partial AC-M5D7QC-004/007. The explicit
        // held-root test seam proves one lock spans every launch phase.
        public void AC_M5D7QC_004_AC_M5D7QC_007_HeldRootSpansObservePreserveAndSave()
        {
            WithRoot(root =>
            {
                var port = new LockSpanPort(root);
                using (var prepared = ProfileLaunchPreparationCoordinatorV1.PrepareUnderRootLockForTest(root, Utc, port, new ActionsPort()))
                {
                    Assert.That(prepared.PersistenceOutcome, Is.EqualTo(ProfileLaunchPersistenceOutcomeV1.CommittedFirst));
                    Assert.That(port.Calls, Is.EqualTo("Observe,Preserve,Save"));
                }
                Assert.That(port.AllPhasesObservedExclusiveOwnership, Is.True);
            });
        }

        [Test]
        // REQ-M5D7QC-007; partial AC-M5D7QC-007. A competing root owner
        // reaches Busy without touching ordinary profile leaves.
        public void AC_M5D7QC_007_RootContentionReturnsBusyWithoutProfileMutation()
        {
            WithRoot(root =>
            {
                var primaryPath = Path.Combine(root, "profile.json"); var oldBytes = Current(9, string.Empty).CanonicalFileBytes;
                File.WriteAllBytes(primaryPath, oldBytes);
                using (ProfileRootOperationLockV1.Acquire(root))
                {
                    var error = Assert.Throws<ProfileRootOperationBlockedExceptionV1>(() => ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc));
                    Assert.That(error.Outcome, Is.EqualTo(ProfileRootOperationOutcomeV1.Busy));
                }
                CollectionAssert.AreEqual(oldBytes, File.ReadAllBytes(primaryPath));
                Assert.That(File.Exists(Path.Combine(root, "profile.prev.json")), Is.False);
                Assert.That(File.Exists(Path.Combine(root, "profile.tmp.json")), Is.False);
            });
        }

        private static ProfileCanonicalDocumentV1 Current(long revision, string overrides)
        {
            var input = new ProfileInputSnapshot(ProfileInputContract.CurrentInputActionsAssetId, ProfileInputContract.CurrentBindingSchemaVersion, ProfileBindingOverridesJson.Parse(overrides));
            var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), input, new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>()));
            return ProfileCanonicalEncoder.Encode(snapshot);
        }
        private static ProfileCanonicalDocumentV1 Document(long revision, string asset, string overrides)
        {
            var input = new ProfileInputSnapshot(asset, ProfileInputContract.CurrentBindingSchemaVersion, ProfileBindingOverridesJson.Parse(overrides)); var snapshot = new ProfileSnapshotV1(new ProfileSettingsSnapshot(ProfileWindowMode.Windowed, 500, 500, 500, false, false), input, new ProfileTutorialSnapshot(Array.Empty<string>()), new ProfileProgressionSnapshot(revision, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, Array.Empty<ProfileBranch>())); return ProfileCanonicalEncoder.Encode(snapshot);
        }
        private static ProfileLoadCandidateV1 Candidate(ProfileLoadFileRole role, ProfileCanonicalDocumentV1 value) => ProfileLoadCandidateV1.Decoded(role, ProfileCanonicalDecoderV1.Decode(value.CanonicalFileBytes));
        private static ProfileLaunchObservationProjectionV1 Empty() => new ProfileLaunchObservationProjectionV1(ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Primary), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Previous), ProfileLoadCandidateV1.Missing(ProfileLoadFileRole.Temp));
        private static ProfileLaunchSaveProjectionV1 OrdinaryFirst(long revision) => new ProfileLaunchSaveProjectionV1(ProfileLaunchSaveKindV1.Ordinary, ProfileLaunchPersistenceOutcomeV1.CommittedFirst, ProfileAtomicSaveStage.None, ProfileStoredFileState.ValidCurrentInput, ProfileStoredFileState.Missing, ProfileStoredFileState.Missing, revision);
        private static IEnumerable<TestCaseData> BlockedRoles { get { yield return new TestCaseData(ProfileLoadFileRole.Temp); yield return new TestCaseData(ProfileLoadFileRole.Primary); yield return new TestCaseData(ProfileLoadFileRole.Previous); } }
        private static IEnumerable<TestCaseData> HRecoveryRows { get { yield return new TestCaseData(Candidate(ProfileLoadFileRole.Primary, Document(3, "obsolete", string.Empty)), 3); var current = Document(4, ProfileInputContract.CurrentInputActionsAssetId, string.Empty); yield return new TestCaseData(ProfileLoadCandidateV1.Decoded(ProfileLoadFileRole.Primary, ProfileCanonicalDecoderV1.Decode(RebuildRawBinding(current, "{bad}"))), 4); } }
        private static IEnumerable<TestCaseData> SaveFailureRows { get { foreach (var recovery in new[] { false, true }) foreach (var stage in new[] { ProfileAtomicSaveStage.DirectoryPreparation, ProfileAtomicSaveStage.TempWriteFlushClose, ProfileAtomicSaveStage.TempReadValidation, ProfileAtomicSaveStage.ExistingPrimaryValidation }) yield return new TestCaseData(recovery, ProfileLaunchPersistenceOutcomeV1.FailedBeforeCommit, stage); foreach (var recovery in new[] { false, true }) foreach (var stage in new[] { ProfileAtomicSaveStage.CommitCall, ProfileAtomicSaveStage.PostCommitValidation }) yield return new TestCaseData(recovery, ProfileLaunchPersistenceOutcomeV1.CommitOutcomeUncertain, stage); } }
        private static byte[] RebuildRawBinding(ProfileCanonicalDocumentV1 source, string raw) { var payload = Encoding.UTF8.GetString(source.CanonicalPayloadBytes).Replace("\"bindingOverridesJson\":\"\"", "\"bindingOverridesJson\":\"" + raw + "\""); var bytes = Encoding.UTF8.GetBytes(payload); using (var sha = SHA256.Create()) { var hash = sha.ComputeHash(bytes); var text = new StringBuilder(64); foreach (var item in hash) text.Append(item.ToString("x2", CultureInfo.InvariantCulture)); return Encoding.UTF8.GetBytes(payload.Substring(0, payload.Length - 1) + ",\"integrity\":{\"payloadSha256\":\"" + text + "\"}}"); } }
        private static void AssertObservationEqual(ProfileLaunchObservationProjectionV1 expected, ProfileLaunchObservationProjectionV1 actual) { AssertCandidateEqual(expected.Primary, actual.Primary); AssertCandidateEqual(expected.Previous, actual.Previous); AssertCandidateEqual(expected.Temp, actual.Temp); }
        private static void AssertSelectionEqual(ProfileLoadSelectionPlanV1 expected, ProfileLoadSelectionPlanV1 actual) { Assert.That(actual.Source, Is.EqualTo(expected.Source)); Assert.That(actual.SourceRevision, Is.EqualTo(expected.SourceRevision)); Assert.That(actual.ResultRevision, Is.EqualTo(expected.ResultRevision)); Assert.That(actual.RequiresAtomicSave, Is.EqualTo(expected.RequiresAtomicSave)); Assert.That(actual.RecoveryReasons, Is.EqualTo(expected.RecoveryReasons)); Assert.That(actual.PrimaryPreservation, Is.EqualTo(expected.PrimaryPreservation)); Assert.That(actual.PreviousPreservation, Is.EqualTo(expected.PreviousPreservation)); Assert.That(actual.TempPreservation, Is.EqualTo(expected.TempPreservation)); AssertDocumentEqual(expected.InMemoryDocument, actual.InMemoryDocument); }
        private static void AssertCandidateEqual(ProfileLoadCandidateV1 expected, ProfileLoadCandidateV1 actual) { Assert.That(actual.Role, Is.EqualTo(expected.Role)); Assert.That(actual.Kind, Is.EqualTo(expected.Kind)); if (expected.Kind == ProfileLoadCandidateKind.Decoded) { Assert.That(actual.DecodeResult.Classification, Is.EqualTo(expected.DecodeResult.Classification)); AssertDocumentEqual(expected.DecodeResult.Document, actual.DecodeResult.Document); } }
        private static void AssertDocumentEqual(ProfileCanonicalDocumentV1 expected, ProfileCanonicalDocumentV1 actual) { CollectionAssert.AreEqual(expected.CanonicalFileBytes, actual.CanonicalFileBytes); }
        private static void AssertInputEqual(ProfileInputSnapshot expected, ProfileInputSnapshot actual) { Assert.That(actual.InputActionsAssetId, Is.EqualTo(expected.InputActionsAssetId)); Assert.That(actual.BindingSchemaVersion, Is.EqualTo(expected.BindingSchemaVersion)); Assert.That(actual.BindingOverrides.CanonicalText, Is.EqualTo(expected.BindingOverrides.CanonicalText)); }
        private static void AssertInvalid(ProfileLaunchObservationProjectionV1 value, string field, object replacement) { object boxed = value; typeof(ProfileLaunchObservationProjectionV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, replacement); Assert.Throws<InvalidOperationException>(() => ((ProfileLaunchObservationProjectionV1)boxed).Validate()); }
        private static void AssertInvalid(ProfileLaunchPreservationProjectionV1 value, string field, object replacement) { object boxed = value; typeof(ProfileLaunchPreservationProjectionV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, replacement); Assert.Throws<InvalidOperationException>(() => ((ProfileLaunchPreservationProjectionV1)boxed).Validate()); }
        private static void AssertInvalid(ProfileLaunchSaveProjectionV1 value, string field, object replacement) { object boxed = value; typeof(ProfileLaunchSaveProjectionV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, replacement); Assert.Throws<InvalidOperationException>(() => ((ProfileLaunchSaveProjectionV1)boxed).Validate()); }
        private static void AssertTokenInvalid(PreparedProfileLaunchV1 value, string field, object replacement) { typeof(PreparedProfileLaunchV1).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(value, replacement); Assert.Throws<InvalidOperationException>(() => value.Validate(), field); }
        private static object TokenBadValue(string field) { if (field == "_source") return (ProfileLaunchSource)99; if (field == "_sourceRevision") return 99L; if (field == "_finalRevision") return -1L; if (field == "_document") return default(ProfileCanonicalDocumentV1); if (field == "_reasons") return (ProfileRecoveryPlanReason)99; if (field == "_observation") return default(ProfileLaunchObservationProjectionV1); if (field == "_initialBinding" || field == "_finalBinding") return default(ProfileBindingOverrideApplyResultV1); if (field == "_preservation") return default(ProfileLaunchPreservationProjectionV1); if (field == "_save") return default(ProfileLaunchSaveProjectionV1); if (field == "_requiresSave") return false; if (field == "_disposer" || field == "_actions") return null; return 99; }
        private static void CatchPrepare(string root, IProfileLaunchPreparationProfilePortV1 profile) { try { ProfileLaunchPreparationCoordinatorV1.Prepare(root, Utc, profile, new ActionsPort()); } catch (NotSupportedException) { } }

        private static string StaleBindingOverride()
        {
            using (var source = new GameInputActions())
            {
                source.Gameplay.Jump.ApplyBindingOverride("<Keyboard>/k");
                var raw = source.asset.SaveBindingOverridesAsJson(); var marker = raw.IndexOf("\"id\":\"", StringComparison.Ordinal); Assert.That(marker, Is.GreaterThanOrEqualTo(0)); var valueStart = marker + 6; var valueEnd = raw.IndexOf('"', valueStart); var stale = raw.Substring(0, valueStart) + "11111111-2222-4333-8444-555555555555" + raw.Substring(valueEnd); return ProfileBindingOverridesJson.Parse(stale).CanonicalText;
            }
        }

        private static void WithRoot(Action<string> action)
        {
            var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-M5D7L-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            try { action(root); } finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }

        private sealed class CoordinatorPort : IProfileLaunchPreparationProfilePortV1
        {
            private readonly ProfileLaunchObservationProjectionV1 _observation; private readonly ProfileLaunchPreservationProjectionV1? _preservation; private readonly ProfileLaunchSaveProjectionV1 _save; internal string Calls = string.Empty; internal Exception PreserveError; internal string KRoot; internal DateTimeOffset KUtc; internal ProfileLaunchObservationProjectionV1 KObservation; internal ProfileLoadSelectionPlanV1 KSelection; internal string DRoot; internal ProfileCanonicalDocumentV1 DDocument; internal string HRoot; internal ProfileLoadCandidateV1 HPrimary;
            internal CoordinatorPort(ProfileLaunchObservationProjectionV1 observation, ProfileLaunchPreservationProjectionV1? preservation, ProfileLaunchSaveProjectionV1 save) { _observation = observation; _preservation = preservation; _save = save; }
            public ProfileLaunchObservationProjectionV1 Observe(string root) { Add("G"); return _observation; }
            public ProfileLaunchPreservationProjectionV1 Preserve(string root, ProfileLaunchObservationProjectionV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utc) { Add("K"); KRoot = root; KUtc = utc; KObservation = observation; KSelection = selection; if (PreserveError != null) throw PreserveError; if (_preservation.HasValue) return _preservation.Value; return new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, true, null, selection.TempPreservation, selection.PrimaryPreservation, selection.PreviousPreservation, 0); }
            public ProfileLaunchSaveProjectionV1 Save(string root, ProfileCanonicalDocumentV1 document) { Add("D"); DRoot = root; DDocument = document; return _save; }
            public ProfileLaunchSaveProjectionV1 SaveInputRecovery(string root, ProfileLoadCandidateV1 primary) { Add("H"); HRoot = root; HPrimary = primary; return _save; }
            private void Add(string value) { Calls = Calls.Length == 0 ? value : Calls + "," + value; }
        }
        private sealed class LockSpanPort : IProfileLaunchPreparationProfilePortV1
        {
            private readonly string _root; internal string Calls = string.Empty; internal bool AllPhasesObservedExclusiveOwnership = true;
            internal LockSpanPort(string root) { _root = root; }
            public ProfileLaunchObservationProjectionV1 Observe(string root) { AssertHeld(root); Add("Observe"); return Empty(); }
            public ProfileLaunchPreservationProjectionV1 Preserve(string root, ProfileLaunchObservationProjectionV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utc) { AssertHeld(root); Add("Preserve"); return new ProfileLaunchPreservationProjectionV1(ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, ProfileLaunchPreservationStateV1.NotRequired, true, null, selection.TempPreservation, selection.PrimaryPreservation, selection.PreviousPreservation, 0); }
            public ProfileLaunchSaveProjectionV1 Save(string root, ProfileCanonicalDocumentV1 document) { AssertHeld(root); Add("Save"); return OrdinaryFirst(document.Snapshot.ProfileRevision); }
            public ProfileLaunchSaveProjectionV1 SaveInputRecovery(string root, ProfileLoadCandidateV1 primary) { throw new AssertionException("Default bootstrap must not use input recovery."); }
            private void AssertHeld(string root)
            {
                Assert.That(root, Is.EqualTo(_root));
                try { using (new FileStream(Path.Combine(root, "profile.operation.lock"), FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { AllPhasesObservedExclusiveOwnership = false; } }
                catch (IOException) { }
            }
            private void Add(string phase) { Calls = Calls.Length == 0 ? phase : Calls + "," + phase; }
        }
        private sealed class ActionsPort : IProfileLaunchPreparationActionsPortV1
        {
            internal readonly List<GameInputActions> Created = new List<GameInputActions>(); internal readonly List<GameInputActions> Disposed = new List<GameInputActions>(); internal readonly List<ProfileInputSnapshot> AppliedInputs = new List<ProfileInputSnapshot>(); internal readonly List<GameInputActions> AppliedCandidates = new List<GameInputActions>(); internal ProfileBindingOverrideApplyResultV1? First; internal Exception ApplyError; internal int ApplyErrorAt; internal int NullCreateAt; internal int EnableCreateAt; internal bool ReuseFirstAtSecond; internal int DisposeErrorAt; internal Exception DisposeError; private int _applyCalls;
            public GameInputActions Create() { var call = Created.Count + 1; if (NullCreateAt == call) return null; if (ReuseFirstAtSecond && call == 2) return Created[0]; var result = new GameInputActions(); if (EnableCreateAt == call) result.Gameplay.Enable(); Created.Add(result); return result; }
            public ProfileBindingOverrideApplyResultV1 Apply(ProfileInputSnapshot input, GameInputActions candidate) { _applyCalls++; AppliedInputs.Add(input); AppliedCandidates.Add(candidate); if (ApplyError != null && (ApplyErrorAt == 0 || ApplyErrorAt == _applyCalls)) throw ApplyError; if (_applyCalls == 1 && First.HasValue) return First.Value; return new ProfileBindingOverrideApplyResultV1(ProfileBindingOverrideApplyOutcomeV1.DefaultsReady, ProfileBindingOverrideApplyFailureV1.None, ProfileBindingOverrideCandidateDispositionV1.Retain, string.Empty, string.Empty); }
            public void Dispose(GameInputActions candidate) { Disposed.Add(candidate); if (candidate.asset != null && candidate.asset.enabled) candidate.asset.Disable(); candidate.Dispose(); if (DisposeError != null && (DisposeErrorAt == 0 || DisposeErrorAt == Disposed.Count)) throw DisposeError; }
        }
        private sealed class ObserveFaultPort : IProfileLaunchPreparationProfilePortV1
        {
            private readonly int _target; private int _active; internal int MaxActive; internal readonly ManualResetEventSlim Entered = new ManualResetEventSlim(false); internal readonly ManualResetEventSlim Release = new ManualResetEventSlim(false);
            internal ObserveFaultPort(int target) { _target = target; }
            public ProfileLaunchObservationProjectionV1 Observe(string root) { var active = Interlocked.Increment(ref _active); if (active > MaxActive) MaxActive = active; if (active >= _target) Entered.Set(); Release.Wait(2000); Interlocked.Decrement(ref _active); throw new NotSupportedException("observe"); }
            public ProfileLaunchPreservationProjectionV1 Preserve(string root, ProfileLaunchObservationProjectionV1 observation, ProfileLoadSelectionPlanV1 selection, DateTimeOffset utc) { throw new NotSupportedException(); }
            public ProfileLaunchSaveProjectionV1 Save(string root, ProfileCanonicalDocumentV1 document) { throw new NotSupportedException(); }
            public ProfileLaunchSaveProjectionV1 SaveInputRecovery(string root, ProfileLoadCandidateV1 primary) { throw new NotSupportedException(); }
        }
    }
}
