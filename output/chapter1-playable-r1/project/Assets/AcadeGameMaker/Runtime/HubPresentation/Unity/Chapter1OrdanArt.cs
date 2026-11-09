using System;
using AcadeGameMaker.Run.Unity;
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
        private static Sprite[,] _rightPatternFrames;
        private static Sprite[,] _leftPatternFrames;
        private static bool _patternLoadAttempted;
        private static bool _leftPatternLoadAttempted;
        private const int PatternCell=128, PatternColumns=6, PatternRows=4;
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
        // Visual selector only. The four actual combat slots share three
        // source action rows (both DebtLine variants use the DebtLine row).
        // No fake death, vulnerable hit, or projectile pose is fabricated.
        internal static int SelectRightPatternCell(int slot,int phase,int age)
        {
            if(age<0 || phase<0 || phase>4)return -1;
            int row;
            switch(slot)
            {
                case 1:
                case 3:row=1;break; // DebtLineA/B
                case 2:row=2;break; // SeizureWeight
                case 4:row=3;break; // BalanceAudit
                default:return -1;
            }
            int key;
            switch(phase)
            {
                case 0:key=age<6?1:2;break; // actual telegraph
                case 1:key=age<6?3:4;break; // actual execute
                case 2:key=5;break;         // recovery
                default:return -1;         // unanimated, static fallback
            }
            return row*PatternColumns+key;
        }

        private static void LoadRightPattern()
        {
            if(_patternLoadAttempted)return;
            _patternLoadAttempted=true;
            var texture=Resources.Load<Texture2D>(
                "Chapter1Ordan/Candidate-OrdanAttackRight");
            var profile=Resources.Load<TextAsset>(
                "Chapter1Ordan/candidate-right-pattern-profile");
            if(texture==null || profile==null ||
               !profile.text.Contains("ch1-ordan-right-combat-phase-atlas-v1") ||
               !profile.text.Contains("provisional-not-manual-approved") ||
               texture.width!=PatternCell*PatternColumns ||
               texture.height!=PatternCell*PatternRows ||
               texture.filterMode!=FilterMode.Point || texture.mipmapCount!=1)
                return; // Never replace user-authored static art on failure.
            var frames=new Sprite[PatternRows,PatternColumns];
            for(var row=0;row<PatternRows;row++)
            for(var key=0;key<PatternColumns;key++)
            {
                var rect=new Rect(key*PatternCell,
                    texture.height-(row+1)*PatternCell,
                    PatternCell,PatternCell);
                frames[row,key]=Sprite.Create(texture,rect,
                    new Vector2(.52f,.024f),49f,0,SpriteMeshType.FullRect);
                frames[row,key].name="OrdanRight"+row+"_"+key;
            }
            _rightPatternFrames=frames;
        }

        // Authored LEFT master rig. No mirrored RIGHT sprites enter this loader.
        internal static Sprite LeftPhaseSprite(int row,int key)
        {
            LoadLeftPattern();
            if(_leftPatternFrames==null || row<0 || row>=PatternRows ||
               key<0 || key>=PatternColumns) return null;
            return _leftPatternFrames[row,key];
        }

        private static void LoadLeftPattern()
        {
            if(_leftPatternLoadAttempted)return;
            _leftPatternLoadAttempted=true;
            var texture=Resources.Load<Texture2D>(
                "Chapter1Ordan/Candidate-OrdanAttackLeft");
            var profile=Resources.Load<TextAsset>(
                "Chapter1Ordan/candidate-left-pattern-profile");
            if(texture==null || profile==null ||
               !profile.text.Contains("ch1-ordan-left-rig-from-owned-left-static-v1") ||
               !profile.text.Contains("provisional-cutout-rig-not-final-hand-authored") ||
               !profile.text.Contains("rightFramesMirrored") ||
               texture.width!=PatternCell*PatternColumns ||
               texture.height!=PatternCell*PatternRows ||
               texture.filterMode!=FilterMode.Point || texture.mipmapCount!=1)
                return; // The original independent LEFT static stays available.
            var frames=new Sprite[PatternRows,PatternColumns];
            for(var row=0;row<PatternRows;row++)
            for(var key=0;key<PatternColumns;key++)
            {
                var rect=new Rect(key*PatternCell,
                    texture.height-(row+1)*PatternCell,
                    PatternCell,PatternCell);
                frames[row,key]=Sprite.Create(texture,rect,
                    new Vector2(.446502f,.0223938f),47.4f,0,
                    SpriteMeshType.FullRect);
                frames[row,key].name="OrdanLeft"+row+"_"+key;
            }
            _leftPatternFrames=frames;
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
            if(Chapter1OrdanVisualEvidence.TryRead(graph,
                   out var publishedSlot,out var publishedPhase,out var age))
            {
                var cell=SelectRightPatternCell(publishedSlot,publishedPhase,age);
                if(cell>=0)
                {
                    var row=cell/PatternColumns;
                    var key=cell%PatternColumns;
                    if(_facingLeft)
                    {
                        var left=LeftPhaseSprite(row,key);
                        if(left!=null) sprite=left;
                    }
                    else
                    {
                        LoadRightPattern();
                        if(_rightPatternFrames!=null)
                            sprite=_rightPatternFrames[row,key];
                    }
                }
            }
            var screenScale = colliderRect.height / bounds.size.y;
            var width = sprite.rect.width / sprite.pixelsPerUnit * screenScale;
            var height = sprite.rect.height / sprite.pixelsPerUnit * screenScale;
            var x = Mathf.Round(colliderRect.center.x - sprite.pivot.x / sprite.pixelsPerUnit * screenScale);
            var y = Mathf.Round(colliderRect.yMax - (sprite.rect.height - sprite.pivot.y) / sprite.pixelsPerUnit * screenScale);
            var old = GUI.color;
            try
            {
                GUI.color = Color.white;
                // GUI.DrawTexture would draw the WHOLE 6x4 atlas; use the
                // selected Sprite UVs (also identical for static masters).
                var tile=sprite.textureRect;
                var uv=new Rect(tile.x/sprite.texture.width,
                    tile.y/sprite.texture.height,
                    tile.width/sprite.texture.width,
                    tile.height/sprite.texture.height);
                GUI.DrawTextureWithTexCoords(new Rect(x,y,width,height),
                    sprite.texture,uv,true);
            }
            finally { GUI.color = old; }
            return true;
        }
    }
}
