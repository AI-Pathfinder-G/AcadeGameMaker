using System;
using UnityEngine;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // AC-ORD-DISPLAY-001~004: 전체 원본의 표시만 소유하며 전투/충돌은 변경하지 않는다.
    internal static class Chapter1OrdanArt
    {
        [Serializable] private sealed class Frame
        {
            public string resourceName;
            public int width, height, bodyHeightPx;
            public float pixelsPerUnit, pivotX, pivotY;
        }
        [Serializable] private sealed class Profile
        {
            public string profile;
            public int posesPerDirection;
            public Frame right, left;
        }
        private static Sprite _right, _left;
        private static Transform _graph;
        private static Collider2D _source;
        private static float _previousX;
        private static bool _facingLeft = true;

        internal static bool IsOwned(Transform graph, Collider2D source)
        {
            if (graph == null || source == null || !graph.gameObject.activeInHierarchy) return false;
            Transform ordan = null;
            for (var i = 0; i < graph.childCount; i++)
            {
                var child = graph.GetChild(i);
                if (child.name != "Ordan") continue;
                if (ordan != null) return false;
                ordan = child;
            }
            if (ordan == null || !ordan.gameObject.activeInHierarchy || source.transform != ordan) return false;
            var colliders = ordan.GetComponents<Collider2D>();
            if (colliders.Length != 1 || !ReferenceEquals(colliders[0], source)) return false;
            var box = source as BoxCollider2D;
            return box != null && box.enabled && !box.isTrigger &&
                box.size.x == 1.6f && box.size.y == 2.4f && box.offset.x == 0f && box.offset.y == 0f;
        }

        private static void Load()
        {
            if (_right != null && _left != null) return;
            var text = Resources.Load<TextAsset>("Chapter1Ordan/display-profile");
            if (text == null) throw new InvalidOperationException("오르단 표시 프로필이 없습니다.");
            var profile = JsonUtility.FromJson<Profile>(text.text);
            if (profile == null || profile.profile != "ordan-static-display-v1" || profile.posesPerDirection != 1)
                throw new InvalidOperationException("오르단 정적 표시 프로필이 다릅니다.");
            var right = Make(profile.right, "Chapter1Ordan/Right");
            Sprite left;
            try { left = Make(profile.left, "Chapter1Ordan/Left"); }
            catch { UnityEngine.Object.Destroy(right); throw; }
            _right = right; _left = left;
        }
        private static Sprite Make(Frame frame, string expected)
        {
            if (frame == null || frame.resourceName != expected || frame.width != 1215 || frame.height != 1295 ||
                frame.bodyHeightPx <= 0 || frame.bodyHeightPx > frame.height ||
                float.IsNaN(frame.pixelsPerUnit) || float.IsInfinity(frame.pixelsPerUnit) || frame.pixelsPerUnit <= 0 ||
                float.IsNaN(frame.pivotX) || float.IsInfinity(frame.pivotX) || frame.pivotX < 0 || frame.pivotX > 1 ||
                float.IsNaN(frame.pivotY) || float.IsInfinity(frame.pivotY) || frame.pivotY < 0 || frame.pivotY > 1 ||
                Mathf.Abs(frame.bodyHeightPx / frame.pixelsPerUnit - 2.4f) > .0001f)
                throw new InvalidOperationException("오르단 몸높이와 발 기준이 다릅니다.");
            var texture = Resources.Load<Texture2D>(expected);
            if (texture == null || texture.width != frame.width || texture.height != frame.height ||
                texture.filterMode != FilterMode.Point || texture.mipmapCount != 1)
                throw new InvalidOperationException("오르단 원본 치수/점표본 설정이 다릅니다.");
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(frame.pivotX, frame.pivotY), frame.pixelsPerUnit, 0, SpriteMeshType.FullRect);
        }
        // 현재 flow의 허용 보스 단계와 router 직계 부모 graph에서만 호출한다.
        internal static bool Draw(Rect colliderRect, Transform graph, Collider2D source)
        {
            if (!IsOwned(graph, source)) return false;
            var bounds = source.bounds;
            if (bounds.size.y <= 0) return false;
            if (!ReferenceEquals(_graph, graph) || !ReferenceEquals(_source, source))
            { _graph = graph; _source = source; _previousX = bounds.center.x; _facingLeft = true; }
            var dx = bounds.center.x - _previousX;
            if (dx < -.001f) _facingLeft = true;
            else if (dx > .001f) _facingLeft = false;
            _previousX = bounds.center.x;
            Load();
            var sprite = _facingLeft ? _left : _right;
            var screenScale = colliderRect.height / bounds.size.y;
            var width = sprite.rect.width / sprite.pixelsPerUnit * screenScale;
            var height = sprite.rect.height / sprite.pixelsPerUnit * screenScale;
            var x = Mathf.Round(colliderRect.center.x - sprite.pivot.x / sprite.pixelsPerUnit * screenScale);
            var y = Mathf.Round(colliderRect.yMax - (sprite.rect.height - sprite.pivot.y) / sprite.pixelsPerUnit * screenScale);
            var old = GUI.color;
            try { GUI.color = Color.white; GUI.DrawTexture(new Rect(x, y, width, height), sprite.texture, ScaleMode.StretchToFill, true); }
            finally { GUI.color = old; }
            return true;
        }
    }
}
