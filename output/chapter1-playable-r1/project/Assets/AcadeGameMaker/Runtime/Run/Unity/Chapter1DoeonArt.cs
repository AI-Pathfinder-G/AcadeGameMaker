using System;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    // AC-DOEON-001/002/003: 원본 전체의 정적 표시만 소유한다. 동작/충돌은 소유하지 않는다.
    internal static class Chapter1DoeonArt
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
        private static Collider2D _guiSource;
        private static float _guiPreviousX;
        private static bool _guiLeft;

        internal static bool IsPlayer(Collider2D source) => source is CapsuleCollider2D &&
            (source.gameObject.name == "Player" || source.gameObject.name == "CombatEncounterPlayer");

        internal static bool FacingLeft(float previousX, float currentX, bool previousFacing)
        {
            var difference = currentX - previousX;
            return difference < -.001f || (difference <= .001f && previousFacing);
        }

        internal static Sprite ForPlayer(Collider2D source, bool left)
        {
            if (!IsPlayer(source)) return null;
            if (_right == null || _left == null)
            {
                var text = Resources.Load<TextAsset>("Chapter1Doeon/display-profile");
                if (text == null) throw new InvalidOperationException("도언 표시 프로필이 없습니다.");
                var profile = JsonUtility.FromJson<Profile>(text.text);
                if (profile == null || profile.profile != "master-direct-display" || profile.posesPerDirection != 1)
                    throw new InvalidOperationException("도언 정적 표시 프로필이 다릅니다.");
                var right = Make(profile.right, "Chapter1Doeon/Right");
                Sprite leftSprite;
                try { leftSprite = Make(profile.left, "Chapter1Doeon/Left"); }
                catch { UnityEngine.Object.Destroy(right); throw; }
                _right = right; _left = leftSprite;
            }
            return left ? _left : _right;
        }

        private static Sprite Make(Frame frame, string expectedResource)
        {
            if (frame == null || frame.resourceName != expectedResource || frame.width <= 0 || frame.height <= 0 ||
                frame.bodyHeightPx <= 0 || frame.bodyHeightPx > frame.height || frame.pixelsPerUnit <= 0 ||
                float.IsNaN(frame.pixelsPerUnit) || float.IsInfinity(frame.pixelsPerUnit) ||
                float.IsNaN(frame.pivotX) || float.IsInfinity(frame.pivotX) || float.IsNaN(frame.pivotY) || float.IsInfinity(frame.pivotY) ||
                frame.pivotX < 0 || frame.pivotX > 1 || frame.pivotY < 0 || frame.pivotY > 1 ||
                Mathf.Abs(frame.bodyHeightPx / frame.pixelsPerUnit - 32f / 18) > .0001f)
                throw new InvalidOperationException("도언 표시 크기와 발 기준이 다릅니다.");
            var texture = Resources.Load<Texture2D>(expectedResource);
            if (texture == null || texture.width != frame.width || texture.height != frame.height ||
                texture.filterMode != FilterMode.Point || texture.mipmapCount != 1)
                throw new InvalidOperationException("도언 원본 치수/점표본 설정이 다릅니다.");
            // crop·반사·픽셀 편집 없이 투명 여백까지 전체 원본을 표시한다.
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(frame.pivotX, frame.pivotY), frame.pixelsPerUnit, 0, SpriteMeshType.FullRect);
        }

        internal static bool DrawPlayer(Rect colliderRect, Collider2D source)
        {
            if (!IsPlayer(source)) return false;
            var bounds = source.bounds;
            if (!ReferenceEquals(_guiSource, source))
            { _guiSource = source; _guiPreviousX = bounds.center.x; _guiLeft = false; }
            _guiLeft = FacingLeft(_guiPreviousX, bounds.center.x, _guiLeft);
            _guiPreviousX = bounds.center.x;
            var sprite = ForPlayer(source, _guiLeft);
            if (bounds.size.y <= 0) return false;
            var pixelsPerWorldUnit = colliderRect.height / bounds.size.y;
            var width = sprite.rect.width / sprite.pixelsPerUnit * pixelsPerWorldUnit;
            var height = sprite.rect.height / sprite.pixelsPerUnit * pixelsPerWorldUnit;
            var left = Mathf.Round(colliderRect.center.x - width * sprite.pivot.x / sprite.rect.width);
            var top = Mathf.Round(colliderRect.yMax - height * (1 - sprite.pivot.y / sprite.rect.height));
            var old = GUI.color;
            try
            {
                GUI.color = Color.white;
                GUI.DrawTexture(new Rect(left, top, width, height), sprite.texture, ScaleMode.StretchToFill, true);
            }
            finally { GUI.color = old; }
            return true;
        }
    }
}
