using System;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    // REQ-CH1-008, REQ-ART-002/005/008; AC-CH1ART-002/003.
    // 표시만 소유한다. layer 8은 LOS이므로 환경 판별에 사용하지 않는다.
    internal static class Chapter1IndustrialArt
    {
        private const int Cell = 18;
        private static Texture2D _atlas;
        private static Sprite _ground;

        private static Texture2D Atlas
        {
            get
            {
                if (_atlas == null)
                {
                    _atlas = Resources.Load<Texture2D>("Chapter1Industrial/IndustrialTiles");
                    if (_atlas == null || _atlas.width != 288 || _atlas.height != 126)
                        throw new InvalidOperationException("승인된 산업 타일시트가 없습니다.");
                }
                return _atlas;
            }
        }

        private static bool Numbered(string name, string prefix)
        {
            if (!name.StartsWith(prefix, StringComparison.Ordinal) || name.Length == prefix.Length) return false;
            for (var i = prefix.Length; i < name.Length; i++) if (name[i] < '0' || name[i] > '9') return false;
            return true;
        }

        private static int Kind(Collider2D source)
        {
            if (source == null || source.isTrigger) return -1;
            var name = source.gameObject.name;
            if (name.IndexOf("Player", StringComparison.OrdinalIgnoreCase) >= 0 ||
                name == "Ordan" || name == "Walker" || name == "Surveyor") return -1;
            var parent = source.transform.parent;
            if (parent == null) return -1;
            if (parent.name == "Environment" && (name == "Ground" || name == "LeftBound" ||
                name == "RightBound" || Numbered(name, "Floor") || Numbered(name, "Terrain"))) return 0;
            // 정확한 기존 객체 이름과 소유 부모만 사용한다.
            if (parent.name == "Barrier")
            {
                if (name == "LeftFloor" || name == "RightFloor" || name == "Bridge" ||
                    name == "Platform" || name == "CounterweightSupport") return 0;
            }
            // 상자·봉쇄선 후보 셀은 의미 검수에서 보류됐다. 기존 식별 표시를 보존한다.
            return -1;
        }

        // 원본 packed atlas의 좌상단 cell 좌표. 원본 바이트·팔레트는 보존한다.
        private static readonly Vector2Int GroundCell = new Vector2Int(1, 0);

        internal static Sprite ForEnvironment(Collider2D source)
        {
            var kind = Kind(source);
            if (kind < 0) return null;
            if (_ground == null)
            {
                var atlas = Atlas;
                var cell = GroundCell;
                _ground = Sprite.Create(atlas, new Rect(cell.x * Cell, atlas.height - (cell.y + 1) * Cell,
                    Cell, Cell), new Vector2(.5f, .5f), Cell, 0, SpriteMeshType.FullRect);
                _ground.name = "Chapter1IndustrialGround";
            }
            return _ground;
        }

        internal static bool DrawEnvironment(Rect screenRect, Collider2D source)
        {
            var kind = Kind(source);
            if (kind < 0) return false;
            var atlas = Atlas;
            var cell = GroundCell;
            var bounds = source.bounds;
            if (bounds.size.x <= 0 || bounds.size.y <= 0) return false;
            // 한 world unit = 18 source px. 화면의 각 셀 경계만 정수 반올림한다.
            // collider 크기에 셀 하나를 늘리거나 atlas 전체를 반복하지 않는다.
            var unitX = screenRect.width / bounds.size.x;
            var unitY = screenRect.height / bounds.size.y;
            var old = GUI.color;
            GUI.color = Color.white;
            try
            {
                for (var y = 0; y < Mathf.CeilToInt(bounds.size.y); y++)
                for (var x = 0; x < Mathf.CeilToInt(bounds.size.x); x++)
                {
                    var left = Mathf.Round(screenRect.x + x * unitX);
                    var top = Mathf.Round(screenRect.y + y * unitY);
                    var right = Mathf.Round(screenRect.x + Mathf.Min(x + 1, bounds.size.x) * unitX);
                    var bottom = Mathf.Round(screenRect.y + Mathf.Min(y + 1, bounds.size.y) * unitY);
                    if (right <= left || bottom <= top) continue;
                    var fractionX = Mathf.Min(1, bounds.size.x - x);
                    var fractionY = Mathf.Min(1, bounds.size.y - y);
                    var uv = new Rect(cell.x * Cell / (float)atlas.width,
                        1 - (cell.y + fractionY) * Cell / atlas.height,
                        fractionX * Cell / atlas.width, fractionY * Cell / atlas.height);
                    GUI.DrawTextureWithTexCoords(Rect.MinMaxRect(left, top, right, bottom), atlas, uv, true);
                }
            }
            finally { GUI.color = old; }
            return true;
        }
    }
}
