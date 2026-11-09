using System;
using AcadeGameMaker.Input.Unity;
using UnityEngine;

namespace AcadeGameMaker.Run.Unity
{
    // REQ-C1B-ROOM-001: 이 값은 도달성 수용 증거가 아니라 정확 저작 메타다.
    [DisallowMultipleComponent]
    internal sealed class Chapter1RoomInstance : MonoBehaviour
    {
        [SerializeField] private string _roomId;
        [SerializeField] private Rect[] _floors, _terrain;
        [SerializeField] private Vector2[] _requiredPath;
        [SerializeField] private string _basicMovementAcceptanceSha;
        private Collider2D[] _visualSources;
        private SpriteRenderer[] _markers;
        private SpriteRenderer[] _feedbackMarkers;
        private bool[] _playerFacingLeft;
        private float[] _previousVisualX;
        private SpriteRenderer _crushMarker;
        private Texture2D _texture;
        private Sprite _sprite;
        internal string RoomId => _roomId;
        internal Vector2 Entry => new Vector2(0,0.81f);
        internal Rect Exit => Rect.MinMaxRect(30,0,32,3);
        internal Rect CameraArea => Rect.MinMaxRect(-4,-10,36,14);
        internal Rect LeftBound => Rect.MinMaxRect(-4,-10,-3,14);
        internal Rect RightBound => Rect.MinMaxRect(35,-10,36,14);
        internal Vector2 RewardSocket => new Vector2(29,0);
        internal int GrantedRewardCount => 0;
        internal bool IsBasicMovementAccepted => IsSha(_basicMovementAcceptanceSha);
        internal Rect[] Floors => (Rect[])_floors.Clone();
        internal Rect[] Terrain => (Rect[])_terrain.Clone();
        internal Vector2[] RequiredPath => (Vector2[])_requiredPath.Clone();
        internal Chapter1HazardDefinition Hazard => new Chapter1HazardDefinition(_roomId,true);
        internal bool SupportsSocketPlan(string socket) => socket=="S"+_roomId.Substring(1)+"-A" ||
            (_roomId!="R01" && _roomId!="R06" && socket=="S"+_roomId.Substring(1)+"-B");
        internal Vector2 WalkerSocket(string socket) { ValidateSocket(socket);return new Vector2(WalkerX(_roomId)+(socket.EndsWith("-B",StringComparison.Ordinal)?1:0),0.81f); }
        internal Vector2 SurveyorSocket(string socket) { ValidateSocket(socket);return new Vector2(SurveyorX(_roomId)-(socket.EndsWith("-B",StringComparison.Ordinal)?1:0),0.81f); }
        internal void ConfigureForAuthoring(string roomId)
        {
            if(gameObject.activeInHierarchy || _roomId!=null)throw new InvalidOperationException("방 메타는 비활성 상태에서 한 번 구성합니다.");
            ValidateId(roomId);_roomId=roomId;_floors=ExpectedFloors(roomId);_terrain=ExpectedTerrain(roomId);_requiredPath=ExpectedPath(roomId);
        }
        internal void AcceptBasicMovementForAuthoring(string independentEvidenceSha)
        {
            if(gameObject.activeInHierarchy || IsBasicMovementAccepted || !IsSha(independentEvidenceSha))
                throw new InvalidOperationException("방 풀 편입은 별도 실제 기본 이동 검수 지문이 필요합니다.");
            ValidateStructure();_basicMovementAcceptanceSha=independentEvidenceSha;
        }
        internal void ValidateStructure()
        {
            ValidateId(_roomId);
            Equal(_floors,ExpectedFloors(_roomId));Equal(_terrain,ExpectedTerrain(_roomId));
            var path=ExpectedPath(_roomId);
            if(_requiredPath==null || _requiredPath.Length!=path.Length)throw new InvalidOperationException("필수 경로가 다릅니다.");
            for(var i=0;i<path.Length;i++)if(_requiredPath[i]!=path[i])throw new InvalidOperationException("필수 경로 좌표가 다릅니다.");
            if(_basicMovementAcceptanceSha!=null && _basicMovementAcceptanceSha.Length!=0 && !IsSha(_basicMovementAcceptanceSha))
                throw new InvalidOperationException("기본 이동 검수 지문이 손상되었습니다.");
        }
        private void Start()
        {
            ValidateStructure();
            // AC-DOEON-002/003: 완료 collider 위치에 정적 원본을 균등 표시한다. 나머지 임시 표현은 유지한다.
            _texture=new Texture2D(2,2,TextureFormat.RGBA32,false){filterMode=FilterMode.Point};
            _texture.SetPixels(new[]{Color.white,Color.white,Color.white,Color.white});_texture.Apply();
            _sprite=Sprite.Create(_texture,new Rect(0,0,2,2),new Vector2(.5f,.5f),2);
            _visualSources=GetComponentsInChildren<Collider2D>(true);_markers=new SpriteRenderer[_visualSources.Length];
            _feedbackMarkers=new SpriteRenderer[_visualSources.Length];
            _playerFacingLeft=new bool[_markers.Length];_previousVisualX=new float[_markers.Length];
            for(var i=0;i<_markers.Length;i++)
            {
                var playerArt=Chapter1DoeonArt.ForPlayer(_visualSources[i],false);
                _previousVisualX[i]=_visualSources[i].bounds.center.x;
                var art=playerArt!=null?playerArt:Chapter1IndustrialArt.ForEnvironment(_visualSources[i]);
                var marker=new GameObject((playerArt!=null?"DoeonStaticView":art!=null?"IndustrialEnvironmentView":"TemporaryActorView")+i);marker.transform.SetParent(transform,false);
                var renderer=marker.AddComponent<SpriteRenderer>();renderer.sprite=playerArt!=null?playerArt:art!=null?art:_sprite;
                if(art!=null && playerArt==null){renderer.drawMode=SpriteDrawMode.Tiled;renderer.tileMode=SpriteTileMode.Continuous;renderer.sortingOrder=-1;}
                renderer.color=playerArt!=null || art!=null?Color.white:_visualSources[i].gameObject.layer==8?new Color(.25f,.28f,.32f):
                    _visualSources[i].gameObject.name.Contains("Player")?new Color(.85f,.9f,1f):new Color(.8f,.35f,.25f);
                _markers[i]=renderer;
                if(playerArt!=null)
                {
                    var effectView=new GameObject("DoeonActionFeedback"+i);
                    effectView.transform.SetParent(marker.transform,false);
                    var effectRenderer=effectView.AddComponent<SpriteRenderer>();
                    effectRenderer.sortingOrder=renderer.sortingOrder+1;
                    effectRenderer.color=Color.white;
                    effectRenderer.enabled=false;
                    _feedbackMarkers[i]=effectRenderer;
                }
            }
            if(_roomId=="R05")
            {var marker=new GameObject("TemporaryLethalAreaView");marker.transform.SetParent(transform,false);_crushMarker=marker.AddComponent<SpriteRenderer>();_crushMarker.sprite=_sprite;
                marker.transform.localPosition=new Vector3(13,1,0);marker.transform.localScale=new Vector3(2,2,1);}
        }
        private void LateUpdate()
        {
            if(_markers==null)return;
            var visualRouter=GetComponentInChildren<InputRouter>(true);
            for(var i=0;i<_markers.Length;i++)
            {
                var source=_visualSources[i];var visible=source!=null && source.enabled && source.gameObject.activeInHierarchy;
                _markers[i].gameObject.SetActive(visible);
                if(visible)
                {
                    var bounds=source.bounds;
                    if(Chapter1DoeonArt.IsPlayer(source))
                    {
                        _playerFacingLeft[i]=Chapter1DoeonArt.FacingLeft(_previousVisualX[i],bounds.center.x,_playerFacingLeft[i]);
                        _previousVisualX[i]=bounds.center.x;
                        _markers[i].sprite=Chapter1DoeonAnimation.ForPlayerPose(source,_playerFacingLeft[i],visualRouter);
                        _markers[i].color=Chapter1DoeonAnimation.CurrentVisualTint;
                        if(_feedbackMarkers[i]!=null)
                        {
                            var effect=Chapter1DoeonAnimation.CurrentVisualEffect;
                            _feedbackMarkers[i].sprite=effect;
                            _feedbackMarkers[i].enabled=effect!=null;
                        }
                        _markers[i].transform.position=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
                        _markers[i].transform.localScale=Vector3.one;
                    }
                    else
                    {
                        _markers[i].transform.position=bounds.center;
                        if(_markers[i].drawMode==SpriteDrawMode.Tiled){_markers[i].transform.localScale=Vector3.one;_markers[i].size=bounds.size;}
                        else _markers[i].transform.localScale=bounds.size;
                    }
                }
            }
            if(_crushMarker!=null)
            {
                var router=GetComponentInChildren<InputRouter>(true);
                if(router!=null && router.TryGetChapter1CompletedEvidence(out var completed))
                {
                    // 입구 발급 first tick과 같은 정의를 쓰고 완료 source가 없는 프레임은 숨긴다.
                    var hazards=GetComponentInChildren<Chapter1HazardObservation>(true);
                    if(hazards!=null && hazards.TryGetCompleted(completed,out _,out var crush))
                        _crushMarker.color=crush.Triggered?new Color(1,0,0,.75f):hazards.IsClosedAt(completed.Tick)?new Color(.9f,.1f,.1f,.55f):new Color(.95f,.75f,.15f,.2f);
                    else _crushMarker.color=Color.clear;
                }
                else _crushMarker.color=Color.clear;
            }
        }
        private void OnDestroy(){if(_sprite!=null)Destroy(_sprite);if(_texture!=null)Destroy(_texture);}
        private void ValidateSocket(string socket){ValidateStructure();if(!SupportsSocketPlan(socket))throw new ArgumentException("미승인 소켓 계획입니다.");}
        private static bool IsSha(string value){if(value==null || value.Length!=64)return false;foreach(var c in value)if(!((c>='0'&&c<='9')||(c>='A'&&c<='F')))return false;return true;}
        private static void ValidateId(string id){if(id==null || id.Length!=3 || id[0]!='R' || id[1]!='0' || id[2]<'1' || id[2]>'6')throw new ArgumentException("지정 여섯 방 외에는 허용하지 않습니다.");}
        private static Rect R(float a,float b,float c,float d)=>Rect.MinMaxRect(a,b,c,d);
        private static void Equal(Rect[] actual,Rect[] expected){if(actual==null || actual.Length!=expected.Length)throw new InvalidOperationException("저작 지형 수가 다릅니다.");for(var i=0;i<actual.Length;i++)if(actual[i]!=expected[i])throw new InvalidOperationException("저작 지형 좌표가 다릅니다.");}
        internal static Rect[] ExpectedFloors(string id) => id=="R04"?new[]{R(-3,-1,12,0),R(14,-1,35,0)}:
            id=="R06"?new[]{R(-3,-1,10,0),R(12,-1,35,0)}:new[]{R(-3,-1,35,0)};
        internal static Rect[] ExpectedTerrain(string id)
        {
            switch(id){case "R02":return new[]{R(10,0,13,1),R(21,0,24,1)};
                case "R03":return new[]{R(8,0,12,1),R(12,0,16,2),R(16,0,20,1)};
                case "R04":return new[]{R(18,1,22,2)};case "R05":return new[]{R(10,2,16,3)};
                case "R06":return new[]{R(20,0,23,1)};default:return Array.Empty<Rect>();}
        }
        internal static Vector2[] ExpectedPath(string id)
        {
            // 단차 양끝 상단과 gap 이륙·착지점을 결정적으로 포함한다. 선택 발판은 제외한다.
            switch(id){case "R02":return new[]{new Vector2(0,0),new Vector2(6,0),new Vector2(10,1),new Vector2(13,1),new Vector2(14,0),new Vector2(18,0),new Vector2(21,1),new Vector2(24,1),new Vector2(31,0)};
                case "R03":return new[]{new Vector2(0,0),new Vector2(6,0),new Vector2(8,1),new Vector2(10,1),new Vector2(12,2),new Vector2(14,2),new Vector2(16,2),new Vector2(16,1),new Vector2(18,1),new Vector2(20,1),new Vector2(24,0),new Vector2(31,0)};
                case "R04":return new[]{new Vector2(0,0),new Vector2(6,0),new Vector2(10,0),new Vector2(12,0),new Vector2(14,0),new Vector2(18,0),new Vector2(24,0),new Vector2(31,0)};
                case "R06":return new[]{new Vector2(0,0),new Vector2(6,0),new Vector2(10,0),new Vector2(12,0),new Vector2(14,0),new Vector2(18,0),new Vector2(20,1),new Vector2(23,1),new Vector2(24,0),new Vector2(31,0)};
                default:return new[]{new Vector2(0,0),new Vector2(6,0),new Vector2(10,0),new Vector2(14,0),new Vector2(18,0),new Vector2(24,0),new Vector2(31,0)};}
        }
        private static int WalkerX(string id){switch(id){case "R01":return 8;case "R02":case "R05":return 6;case "R04":return 7;default:return 5;}}
        private static int SurveyorX(string id){switch(id){case "R01":return 20;case "R03":return 25;case "R05":return 26;default:return 27;}}
    }
}
