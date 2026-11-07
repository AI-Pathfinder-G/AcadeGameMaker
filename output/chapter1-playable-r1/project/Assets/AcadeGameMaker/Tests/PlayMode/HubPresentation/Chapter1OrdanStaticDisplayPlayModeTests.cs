// REQ-ART-005/008/013/014, REQ-CH1-008; AC-ORD-DISPLAY-001~004.
using System;
using System.Collections;
using AcadeGameMaker.Hub.Presentation.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

namespace AcadeGameMaker.Tests.PlayMode.HubPresentation
{
    internal sealed class Chapter1OrdanStaticDisplayPlayModeTests
    {
        [SerializeField] private AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope _fixture = new AcadeGameMaker.Tests.PlayMode.InputUnity.OwnedBatchRuntimeFixtureScope();
        [UnitySetUp] public IEnumerator Enter()
        { _fixture.PrepareEnter(); yield return new EnterPlayMode(); _fixture.ConfirmEntered(); yield return _fixture.RunBody(_fixture.BeginCase(false)); }
        [UnityTearDown] public IEnumerator Exit()
        {
            try { if (_fixture.OwnsRuntimeFixture && Application.isPlaying) yield return _fixture.RunBody(_fixture.CloseCase(), true); }
            finally { }
            if (_fixture.OwnsRuntimeFixture) { if (Application.isPlaying) yield return new ExitPlayMode(); _fixture.CaptureCleanupFailure(_fixture.CompleteExit); }
            _fixture.ThrowCleanupFailure();
        }
        private const string Prefab = "Assets/Prefabs/Combat/OrdanBossEncounterGraph.prefab";
        private static GameObject CreateGraph()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(Prefab);
            Assert.That(asset, Is.Not.Null);
            var root = UnityEngine.Object.Instantiate(asset);
            Assert.That(root.activeInHierarchy, Is.True);
            return root;
        }
        private static BoxCollider2D Body(GameObject graph)
        { var ordan = graph.transform.Find("Ordan"); Assert.That(ordan, Is.Not.Null); return ordan.GetComponent<BoxCollider2D>(); }
        [UnityTest] public IEnumerator AuthoredGraphOwnsOnlyOrdan() { yield return _fixture.RunBody(OwnerBody()); }
        private IEnumerator OwnerBody()
        {
            var root = CreateGraph();
            try
            {
                var body = Body(root); Assert.That(Chapter1OrdanArt.IsOwned(root.transform, body), Is.True);
                foreach (var other in root.GetComponentsInChildren<Collider2D>(true))
                    if (!ReferenceEquals(body, other)) Assert.That(Chapter1OrdanArt.IsOwned(root.transform, other), Is.False, other.name);
                // 현재 flow graph 제공은 호출자의 전제이며 임의 graph 인증 시험이 아니다.
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
            yield break;
        }
        [UnityTest] public IEnumerator RejectsDuplicateOrWrongCollider() { yield return _fixture.RunBody(DuplicateBody()); }
        private IEnumerator DuplicateBody()
        {
            var root = CreateGraph();
            try
            {
                var body = Body(root); var duplicate = new GameObject("Ordan"); duplicate.transform.SetParent(root.transform, false);
                Assert.That(Chapter1OrdanArt.IsOwned(root.transform, body), Is.False); UnityEngine.Object.DestroyImmediate(duplicate);
                var extra = body.gameObject.AddComponent<BoxCollider2D>();
                Assert.That(Chapter1OrdanArt.IsOwned(root.transform, body), Is.False); UnityEngine.Object.DestroyImmediate(extra);
                Assert.That(Chapter1OrdanArt.IsOwned(root.transform, null), Is.False);
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
            yield break;
        }
        [UnityTest] public IEnumerator RejectsWrongShapeOffsetAndGraph() { yield return _fixture.RunBody(ShapeBody()); }
        private IEnumerator ShapeBody()
        {
            var root = CreateGraph(); var other = new GameObject("UnrelatedGraph");
            try
            {
                var body = Body(root); var size = body.size;
                body.size = new Vector2(1.7f, 2.4f); Assert.That(Chapter1OrdanArt.IsOwned(root.transform, body), Is.False); body.size = size;
                var offset = body.offset;
                try
                {
                    body.offset = new Vector2(.125f, 0f);
                    var observedOffset = body.offset;
                    Assert.That(observedOffset.x, Is.EqualTo(.125f));
                    Assert.That(observedOffset.y, Is.EqualTo(0f));
                    Assert.That(Chapter1OrdanArt.IsOwned(root.transform, body), Is.False);
                }
                finally { body.offset = offset; }
                body.isTrigger = true; Assert.That(Chapter1OrdanArt.IsOwned(root.transform, body), Is.False); body.isTrigger = false;
                body.enabled = false; Assert.That(Chapter1OrdanArt.IsOwned(root.transform, body), Is.False); body.enabled = true;
                Assert.That(Chapter1OrdanArt.IsOwned(other.transform, body), Is.False);
                body.transform.SetParent(other.transform); Assert.That(Chapter1OrdanArt.IsOwned(root.transform, body), Is.False); body.transform.SetParent(root.transform);
                body.gameObject.SetActive(false); Assert.That(Chapter1OrdanArt.IsOwned(root.transform, body), Is.False); body.gameObject.SetActive(true);
                root.SetActive(false); Assert.That(Chapter1OrdanArt.IsOwned(root.transform, body), Is.False); root.SetActive(true);
            }
            finally { UnityEngine.Object.DestroyImmediate(root); UnityEngine.Object.DestroyImmediate(other); }
            yield break;
        }
        [Serializable] private sealed class Frame
        { public string resourceName; public int width, height, headTopY, contactY, bodyHeightPx; public float footCenterX, pixelsPerUnit, pivotX, pivotY; }
        [Serializable] private sealed class Profile
        { public string profile; public int posesPerDirection; public bool mirror, imagePixelsModified; public Frame right, left; }
        private static Profile ReadProfile()
        { var text = Resources.Load<TextAsset>("Chapter1Ordan/display-profile"); Assert.That(text, Is.Not.Null); return JsonUtility.FromJson<Profile>(text.text); }
        [UnityTest] public IEnumerator ImportedOriginalResourcesMatchProfile() { yield return _fixture.RunBody(ResourcesBody()); }
        private IEnumerator ResourcesBody()
        {
            var profile = ReadProfile(); Assert.That(profile.profile, Is.EqualTo("ordan-static-display-v1")); Assert.That(profile.posesPerDirection, Is.EqualTo(1));
            Assert.That(profile.mirror || profile.imagePixelsModified, Is.False);
            foreach (var frame in new[] { profile.right, profile.left })
            {
                var texture = Resources.Load<Texture2D>(frame.resourceName); Assert.That(texture, Is.Not.Null);
                Assert.That(texture.width, Is.EqualTo(1215)); Assert.That(texture.height, Is.EqualTo(1295));
                Assert.That(texture.width, Is.EqualTo(frame.width)); Assert.That(texture.height, Is.EqualTo(frame.height));
                Assert.That(texture.filterMode, Is.EqualTo(FilterMode.Point)); Assert.That(texture.mipmapCount, Is.EqualTo(1));
                var path = AssetDatabase.GetAssetPath(texture); Assert.That(path, Is.EqualTo("Assets/Resources/" + frame.resourceName + ".png"));
                var importer = (TextureImporter)AssetImporter.GetAtPath(path); Assert.That(importer, Is.Not.Null);
                Assert.That(importer.mipmapEnabled, Is.False); Assert.That(importer.textureCompression, Is.EqualTo(TextureImporterCompression.Uncompressed));
                Assert.That(importer.npotScale, Is.EqualTo(TextureImporterNPOTScale.None));
            }
            yield break;
        }
        [UnityTest] public IEnumerator BodyScaleAndFootPivotMatchApprovedCoordinates() { yield return _fixture.RunBody(ProfileBody()); }
        private IEnumerator ProfileBody()
        {
            var p = ReadProfile(); CheckFrame(p.right, "Chapter1Ordan/Right", 120, 1273, 661f); CheckFrame(p.left, "Chapter1Ordan/Left", 117, 1266, 542.5f);
            // 수치 검증이며 GUI.Draw/방향 화면/완주 증거가 아니다.
            yield break;
        }
        private static void CheckFrame(Frame f, string resource, int head, int foot, float x)
        {
            Assert.That(f.resourceName, Is.EqualTo(resource)); Assert.That(f.headTopY, Is.EqualTo(head)); Assert.That(f.contactY, Is.EqualTo(foot));
            Assert.That(f.footCenterX, Is.EqualTo(x)); Assert.That(f.bodyHeightPx, Is.EqualTo(foot-head));
            Assert.That(f.bodyHeightPx / f.pixelsPerUnit, Is.EqualTo(2.4f).Within(.0001f));
            Assert.That(f.pivotX, Is.EqualTo(x/1215f).Within(.000001f)); Assert.That(f.pivotY, Is.EqualTo((1295-foot)/1295f).Within(.000001f));
        }
    }
}
