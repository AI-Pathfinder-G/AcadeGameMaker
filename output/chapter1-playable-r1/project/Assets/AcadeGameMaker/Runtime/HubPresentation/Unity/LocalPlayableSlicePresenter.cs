using System;
using AcadeGameMaker.Core;
using AcadeGameMaker.Input.Unity;
using TMPro;
using UnityEngine.TextCore.LowLevel;
using UnityEngine;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // REQ-LPS-003/004/007: graph-external display and sole replacement owner.
    [DefaultExecutionOrder(200)]
    [DisallowMultipleComponent]
    internal sealed class LocalPlayableSlicePresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _graphPrefab;
        [SerializeField] private OrdanBossTerminalTransitionRequester _requester;
        [SerializeField] private InputRouter _router;
        [SerializeField] private TMP_FontAsset _regular, _bold;
        [SerializeField] private Font _regularSource, _boldSource;
        private TMP_FontAsset _runtimeRegular, _runtimeBold;
        internal TMP_FontAsset RuntimeRegularFont => _runtimeRegular;
        internal TMP_FontAsset RuntimeBoldFont => _runtimeBold;
        internal const string TitleCopy = "로컬 조작 체험 · 임시 표현";
        internal const string HelpCopy = "A/D 이동 · Space 점프 · Shift 대시 · 마우스 조준\n왼쪽 클릭 공격 · 오른쪽 클릭 전이/회수 · 결과: Enter 재시도";
        internal const string GlyphCopy = TitleCopy + HelpCopy + "도언 오르단 전이 기본 상태 오르단 제압 도언 쓰러짐 재시도 입력을 놓으면 시작합니다 이전 입력 정리 중 체험을 시작할 수 없습니다 기록을 확인해 주세요 감사 상자 중량 물체 0123456789/-[]: .\n";
        [SerializeField] private TMP_Text _title, _hud, _help, _result;
        [SerializeField] private RectTransform _retry;
        [SerializeField] private Transform[] _markers;
        private GameObject _graph, _oldGraph;
        private LocalPlayableInputLifetime _oldLifetime;
        private UiSemanticFrameCursorV1 _cursor;
        private LocalPlayableSliceView? _view;
        private long _generation = 1;
        private bool _configured, _started, _retryAuthorized, _waiting, _failed;
        private int _destroyFrame;
        private Sprite _sprite, _playerSprite, _transferSprite;
        private Texture2D _playerTexture, _transferTexture;
        private Texture2D _texture;
        private Transform[] _environmentMarkers;
        internal long Generation => _generation;
        internal bool RestartPending => _waiting;
        internal bool IsFailed => _failed;
        internal bool HasCompletedView => _view.HasValue;

        internal void ConfigureForAuthoring(GameObject graphPrefab, OrdanBossTerminalTransitionRequester requester,
            InputRouter router, TMP_FontAsset regular, TMP_FontAsset bold, Font regularSource, Font boldSource, TMP_Text title, TMP_Text hud,
            TMP_Text help, TMP_Text result, RectTransform retry, Transform[] markers)
        {
            if (_configured || _started) throw new InvalidOperationException("Presenter configuration is single-use.");
            _graphPrefab = graphPrefab; _requester = requester; _router = router; _regular = regular; _bold = bold; _regularSource = regularSource; _boldSource = boldSource;
            _title = title; _hud = hud; _help = help; _result = result; _retry = retry; _markers = markers;
            _configured = true;
        }
        internal void ValidateForAuthoring(GameObject graphPrefab, GameObject graph, TMP_FontAsset regular, TMP_FontAsset bold, Font regularSource, Font boldSource)
        {
            if (_started || _graphPrefab != graphPrefab || _router == null || _requester == null ||
                _router.transform.root.gameObject != graph || _requester.transform.root.gameObject != graph ||
                transform.root.gameObject == graph || !_router.LocalPlayableRequiresNeutralStart || !_requester.LocalPlayableEnabled ||
                _regular != regular || _bold != bold || regular == null || bold == null ||
                _regularSource != regularSource || _boldSource != boldSource || regularSource == null || boldSource == null ||
                _title == null || _hud == null || _help == null || _result == null || _retry == null ||
                _markers == null || _markers.Length != 9)
                throw new InvalidOperationException("Local presenter authoring references are incomplete.");
            foreach (var marker in _markers)
                if (marker == null || marker.transform.root == graph.transform || marker.GetComponent<SpriteRenderer>() == null)
                    throw new InvalidOperationException("Local marker must be outside the simulation graph.");
        }
        private void Start()
        {
            _started = true;
            try
            {
                if (_graphPrefab == null || _router == null || _requester == null || _regular == null || _bold == null ||
                    _regularSource == null || _boldSource == null ||
                    _title == null || _hud == null || _help == null || _result == null || _retry == null || _markers == null || _markers.Length != 9)
                    throw new InvalidOperationException("Local presentation is incomplete.");
                _graph = _router.transform.root.gameObject;
                if (_requester.transform.root.gameObject != _graph || transform.root.gameObject == _graph)
                    throw new InvalidOperationException("Local graph ownership mismatch.");
                // REQ-LPS-FONT-001/002: explicit included Font inputs, owned memory glyph atlases.
                _runtimeRegular = CreateRuntimeFont(_regularSource, _regular);
                _runtimeBold = CreateRuntimeFont(_boldSource, _bold);
                _title.font = _runtimeBold; _hud.font = _runtimeRegular; _help.font = _runtimeRegular; _result.font = _runtimeBold;
                _texture = new Texture2D(1, 1, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                _texture.SetPixel(0, 0, Color.white); _texture.Apply();
                _sprite = Sprite.Create(_texture, new Rect(0, 0, 1, 1), new Vector2(.5f, .5f), 1);
                foreach (var marker in _markers) marker.GetComponent<SpriteRenderer>().sprite = _sprite;
                _playerSprite = CreateRoleSprite(true, out _playerTexture);
                _transferSprite = CreateRoleSprite(false, out _transferTexture);
                _markers[0].GetComponent<SpriteRenderer>().sprite = _playerSprite;
                for (var i = 2; i <= 5; i++) _markers[i].GetComponent<SpriteRenderer>().sprite = _transferSprite;
                var environment = _graph.transform.GetChild(8);
                _environmentMarkers = new Transform[environment.childCount];
                for (var i = 0; i < environment.childCount; i++)
                {
                    var value = new GameObject("EnvironmentMarker-" + i);
                    value.transform.SetParent(_markers[8], false);
                    var renderer = value.AddComponent<SpriteRenderer>(); renderer.sprite = _sprite; renderer.color = new Color(.32f,.32f,.32f);
                    _environmentMarkers[i] = value.transform;
                }
                _markers[8].GetComponent<SpriteRenderer>().enabled = false;
                Cursor.visible = true; Cursor.lockState = CursorLockMode.None;
            }
            catch (Exception error) { Fail(error); }
        }
        private void FixedUpdate()
        {
            if (!_started || _failed) return;
            try
            {
                if (_waiting) { RestartAfterDisposal(); return; }
                if (_router == null || _router.IsFaulted || _requester == null) throw new InvalidOperationException("Local input graph failed.");
                var next = _requester.LatestLocalPlayableView;
                if (next.HasValue) _view = next.Value.WithGeneration(_generation);
                if (_requester.IsClosed && (!_view.HasValue || !_view.Value.UiOnlyTick.HasValue))
                    throw new InvalidOperationException("Local result authority closed before UI approval.");
                if (!_view.HasValue || !_view.Value.UiOnlyTick.HasValue) return;
                if (_cursor == null) { _cursor = UiSemanticFrameCursorV1.Create(_router); return; }
                if (!_cursor.TryAdvance(_router, out var frame)) return;
                frame.Validate();
                if (frame.Receipt.Mode != InputMode.UIOnly || frame.Receipt.Tick.Value <= _view.Value.UiOnlyTick.Value.Value)
                    throw new InvalidOperationException("Retry UI frame is outside the result epoch.");
                if (frame.SubmitPressed || (frame.ClickPressed && frame.HasClickPoint &&
                    RectTransformUtility.RectangleContainsScreenPoint(_retry,
                        new Vector2(frame.ClickPointXActualPx, frame.ClickPointYActualPx), null)))
                {
                    _retryAuthorized = true;
                    RequestRetry();
                }
            }
            catch (Exception error) { Fail(error); }
        }
        private void RequestRetry()
        {
            if (!_retryAuthorized || _waiting || !_view.HasValue || !_view.Value.UiOnlyTick.HasValue || _graph == null)
                throw new InvalidOperationException("Retry needs a newly consumed result UI frame.");
            _retryAuthorized = false; _oldGraph = _graph; _oldLifetime = _router.LocalPlayableLifetime;
            _waiting = true; _destroyFrame = Time.frameCount;
            _oldGraph.SetActive(false); Destroy(_oldGraph);
            _graph = null; _router = null; _requester = null; _cursor = null;
        }
        private void RestartAfterDisposal()
        {
            if (!_waiting || _oldLifetime == null) throw new InvalidOperationException("Missing old lifetime proof.");
            if (_oldGraph != null) return;
            if (!_oldLifetime.IsClosed)
            {
                if (Time.frameCount <= _destroyFrame + 1) return;
                throw new InvalidOperationException("Old input disposal was not confirmed.");
            }
            var fresh = Instantiate(_graphPrefab);
            _graph = fresh; _router = fresh.GetComponentInChildren<InputRouter>(true);
            _requester = fresh.GetComponentInChildren<OrdanBossTerminalTransitionRequester>(true);
            if (_router == null || _requester == null || !_router.LocalPlayableRequiresNeutralStart || !_requester.LocalPlayableEnabled)
                throw new InvalidOperationException("Fresh graph is not the approved local prefab.");
            _generation = checked(_generation + 1); _view = null; _waiting = false; _oldLifetime = null; _oldGraph = null;
        }
        private void LateUpdate()
        {
            if (!_started) return;
            if (_title != null) _title.text = TitleCopy;
            if (_help != null) _help.text = HelpCopy;
            if (_failed) { if (_result != null) _result.text = "체험을 시작할 수 없습니다. 기록을 확인해 주세요."; return; }
            if (_waiting) { _result.text = "이전 입력 정리 중"; return; }
            _result.text = "";
            if (!_view.HasValue) { _hud.text = "입력을 놓으면 시작합니다."; return; }
            var view = _view.Value;
            _hud.text = $"도언 {view.PlayerHealth}/{view.PlayerMaxHealth}   오르단 {view.BossHealth}/{view.BossMaxHealth}\n전이: {(view.Lightweight ? TransferName(view.TransferTarget) : "기본 상태")}";
            if (view.UiOnlyTick.HasValue) _result.text = (view.Result == LocalPlayableSliceResult.Succeeded ? "오르단 제압" : "도언 쓰러짐") + "\n[ 재시도 ]";
            if (_graph == null) return;
            for (var i = 0; i < _markers.Length; i++)
            {
                var source = _graph.transform.GetChild(i);
                var marker = _markers[i];
                marker.gameObject.SetActive(i != 6 && i != 7 && source.gameObject.activeInHierarchy);
                marker.position = source.position;
                var collider = source.GetComponent<Collider2D>();
                if (collider != null)
                { marker.position = collider.bounds.center; marker.localScale = collider.bounds.size; marker.gameObject.SetActive(collider.enabled); }
            }
            for (var i = 0; i < _environmentMarkers.Length; i++)
            {
                var collider = _graph.transform.GetChild(8).GetChild(i).GetComponent<Collider2D>();
                if (collider == null) continue;
                _environmentMarkers[i].position = collider.bounds.center; _environmentMarkers[i].localScale = collider.bounds.size;
            }
        }
        private Sprite CreateRoleSprite(bool diamond, out Texture2D texture)
        {
            texture = new Texture2D(16,16,TextureFormat.RGBA32,false) { filterMode=FilterMode.Point };
            for (var y=0;y<16;y++) for (var x=0;x<16;x++)
            {
                bool visible=diamond ? Mathf.Abs(x-7.5f)+Mathf.Abs(y-7.5f)<=7.5f : x<2 || y<2 || x>13 || y>13;
                texture.SetPixel(x,y,visible?Color.white:Color.clear);
            }
            texture.Apply();return Sprite.Create(texture,new Rect(0,0,16,16),new Vector2(.5f,.5f),16);
        }
        private static string TransferName(string id)
        {
            switch (id)
            {
                case "BossAuditBox-0": return "감사 상자";
                case "BossWeight-0": return "중량 물체 1";
                case "BossWeight-1": return "중량 물체 2";
                case "BossWeight-2": return "중량 물체 3";
                default: throw new InvalidOperationException("Unexpected transfer display target.");
            }
        }
        private TMP_FontAsset CreateRuntimeFont(Font source, TMP_FontAsset template)
        {
            if (template.material == null || template.material.shader == null || template.material.shader.name != "TextMeshPro/Mobile/Distance Field")
                throw new InvalidOperationException("Approved SDF shader reference is absent.");
            var font = TMP_FontAsset.CreateFontAsset(source,90,9,GlyphRenderMode.SDFAA,1024,1024,AtlasPopulationMode.Dynamic,false);
            if (font == null) throw new InvalidOperationException("Runtime included font could not load.");
            try
            {
                if (font.material == null || font.material.shader != template.material.shader)
                    throw new InvalidOperationException("Runtime font shader differs from the approved reference.");
                if (!font.TryAddCharacters(GlyphCopy,out string missing))
                    throw new InvalidOperationException("Runtime font is missing required glyphs: " + missing);
                return font;
            }
            catch { Destroy(font); throw; }
        }
        private void ReleaseOwnedFonts()
        {
            if (_title != null) _title.font = null; if (_hud != null) _hud.font = null;
            if (_help != null) _help.font = null; if (_result != null) _result.font = null;
            // TMP owns its generated atlas/material cleanup. Never destroy source assets.
            var regular = _runtimeRegular; var bold = _runtimeBold; _runtimeRegular = _runtimeBold = null;
            if (regular != null) Destroy(regular); if (bold != null) Destroy(bold);
        }
        private void Fail(Exception error)
        {
            _failed = true;
            ReleaseOwnedFonts();
            if (_graph != null) _graph.SetActive(false);
            Debug.LogError("Local playable closed: " + error.Message, this);
        }
        private void OnDestroy()
        {
            ReleaseOwnedFonts();
            if (_graph != null) { _graph.SetActive(false); Destroy(_graph); }
            if (_sprite != null) Destroy(_sprite); if (_playerSprite != null) Destroy(_playerSprite); if (_transferSprite != null) Destroy(_transferSprite);
            if (_texture != null) Destroy(_texture); if (_playerTexture != null) Destroy(_playerTexture); if (_transferTexture != null) Destroy(_transferTexture);
        }
    }
}
