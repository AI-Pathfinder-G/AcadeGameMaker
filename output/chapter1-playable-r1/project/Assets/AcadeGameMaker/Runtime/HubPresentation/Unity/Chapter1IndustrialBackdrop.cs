using UnityEngine;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // Chapter 1 visual-only distant industrial skyline. No physics, colliders,
    // input capture, save state, scene mutation or asset imports are required.
    // Palette and hard 2px-equivalent pixel edges follow art-direction canon.
    internal static class Chapter1IndustrialBackdrop
    {
        private const int Width = 320;
        private const int Height = 180;
        private const string ChildName = "CH1-Distant-Industrial-Backdrop";
        private static Sprite _sprite;

        internal static void Ensure(Camera camera)
        {
            if (camera == null || !camera.orthographic) return;
            var existing = camera.transform.Find(ChildName);
            if (existing == null)
            {
                var root = new GameObject(ChildName);
                root.transform.SetParent(camera.transform, false);
                var renderer = root.AddComponent<SpriteRenderer>();
                renderer.sprite = Image();
                renderer.sortingOrder = -32000;
                existing = root.transform;
            }

            // Camera-local placement creates one consistent backdrop through
            // room transfers while leaving every authored collider unchanged.
            var depth = Mathf.Min(100f, camera.farClipPlane - 2f);
            existing.localPosition = new Vector3(0f, 0f, depth);
            var bounds = Image().bounds.size;
            var h = camera.orthographicSize * 2.03f;
            var w = h * camera.aspect;
            existing.localScale = new Vector3(w / bounds.x, h / bounds.y, 1f);
        }

        internal static Texture2D GetTexture(){return Image().texture;}

        private static Color32 C(byte r, byte g, byte b)
        {
            return new Color32(r, g, b, 255);
        }

        private static Sprite Image()
        {
            if (_sprite != null) return _sprite;

            var pixels = new Color32[Width * Height];
            var top = C(16, 24, 32);
            var bottom = C(44, 66, 75);
            for (var y = 0; y < Height; ++y)
            {
                var t = y / (float)(Height - 1);
                for (var x = 0; x < Width; ++x)
                {
                    var d = (x * 19 + y * 13) % 47 == 0 ? 2 : 0;
                    pixels[y * Width + x] = C(
                        (byte)Mathf.Lerp(bottom.r + d, top.r + d, t),
                        (byte)Mathf.Lerp(bottom.g + d, top.g + d, t),
                        (byte)Mathf.Lerp(bottom.b + d, top.b + d, t));
                }
            }

            // Far industrial blocks, not interactive scenery.
            for (var i = 0; i < 25; ++i)
            {
                var x = i * 14 - 6;
                var high = 70 + ((i * 29 + 17) % 57);
                Rect(pixels, x, 0, 12, high, C(23, 35, 44));
                Rect(pixels, x + 1, high - 3, 10, 3, C(32, 50, 60));
                if ((i % 3) == 0)
                {
                    Rect(pixels, x + 5, high, 2, 12, C(44, 66, 75));
                    Rect(pixels, x + 4, high + 12, 4, 1, C(82, 107, 112));
                }
                if ((i % 4) == 0) Rect(pixels, x + 2, high - 14, 8, 2, C(59, 85, 93));

                for (var windowY = 12; windowY < high - 8; windowY += 12)
                {
                    if ((windowY + i * 7) % 5 == 0) continue;
                    var glow = ((windowY / 12 + i) % 7 == 0);
                    Rect(pixels, x + 3, windowY, 2, 2, glow ? C(164, 126, 67) : C(59, 85, 93));
                    Rect(pixels, x + 8, windowY, 2, 2, glow ? C(123, 96, 53) : C(44, 66, 75));
                }
            }

            // Maintenance ducts, crossbeams and repeating rivets are kept
            // far behind active platform tiles and the player sprite.
            Rect(pixels, 0, 65, Width, 3, C(44, 66, 75));
            Rect(pixels, 0, 68, Width, 1, C(82, 107, 112));
            Rect(pixels, 0, 104, Width, 4, C(32, 50, 60));
            Rect(pixels, 0, 108, Width, 1, C(59, 85, 93));
            for (var x = 3; x < Width; x += 28)
            {
                Rect(pixels, x, 66, 2, 42, C(59, 85, 93));
                Rect(pixels, x - 2, 66, 6, 2, C(108, 128, 129));
                Rect(pixels, x - 2, 105, 6, 2, C(108, 128, 129));
                Rect(pixels, x + 1, 81, 1, 1, C(164, 126, 67));
                for (var diag = 0; diag < 11; ++diag)
                    Rect(pixels, x + 3 + diag, 72 + diag, 1, 1, C(59, 85, 93));
            }
            for (var x = 10; x < Width; x += 60)
            {
                Rect(pixels, x, 116, 2, 33, C(82, 107, 112));
                Rect(pixels, x - 4, 149, 10, 2, C(108, 128, 129));
                Rect(pixels, x - 2, 112, 6, 4, C(59, 85, 93));
                Rect(pixels, x, 111, 2, 1, C(255, 170, 43));
            }

            var tex = new Texture2D(Width, Height, TextureFormat.RGBA32, false)
            {
                name = "Chapter1IndustrialSkylinePixels",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave
            };
            tex.SetPixels32(pixels);
            tex.Apply(false, true);
            _sprite = Sprite.Create(tex, new Rect(0, 0, Width, Height),
                new Vector2(.5f, .5f), 32f, 0, SpriteMeshType.FullRect);
            _sprite.name = "Chapter1IndustrialSkyline";
            _sprite.hideFlags = HideFlags.DontSave;
            return _sprite;
        }

        private static void Rect(Color32[] pixels, int left, int bottom, int width, int height, Color32 color)
        {
            var right = Mathf.Min(Width, left + width);
            var top = Mathf.Min(Height, bottom + height);
            for (var y = Mathf.Max(0, bottom); y < top; ++y)
                for (var x = Mathf.Max(0, left); x < right; ++x)
                    pixels[y * Width + x] = color;
        }
    }
}