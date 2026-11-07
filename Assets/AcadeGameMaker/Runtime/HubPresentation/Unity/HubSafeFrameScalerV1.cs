using UnityEngine;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // REQ-M5D7QA-006: integer-only viewport proof and safe-frame placement.
    [DisallowMultipleComponent]
    internal sealed class HubSafeFrameScalerV1 : MonoBehaviour
    {
        [SerializeField] private RectTransform _safeFrame;
        private int _width, _height, _scale;
        internal int IntegerScale { get { return _scale; } }
        internal bool IsSupported { get { return _scale > 0; } }
        internal Rect SafeRectActual { get { return new Rect((Screen.width - 640 * _scale) / 2, (Screen.height - 360 * _scale) / 2, 640 * _scale, 360 * _scale); } }
        internal void ConfigureForAuthoring(RectTransform safeFrame) { if (safeFrame == null) throw new System.ArgumentNullException(nameof(safeFrame)); _safeFrame = safeFrame; }
        internal bool Refresh()
        {
            var changed = _width != Screen.width || _height != Screen.height;
            _width = Screen.width; _height = Screen.height;
            _scale = Mathf.FloorToInt(Mathf.Min(_width / 640f, _height / 360f));
            if (_safeFrame == null) return false;
            _safeFrame.sizeDelta = new Vector2(640, 360);
            _safeFrame.localScale = _scale > 0 ? new Vector3(_scale, _scale, 1) : Vector3.one;
            _safeFrame.anchoredPosition = Vector2.zero;
            return changed;
        }
        private void Awake() { Refresh(); }
    }
}
