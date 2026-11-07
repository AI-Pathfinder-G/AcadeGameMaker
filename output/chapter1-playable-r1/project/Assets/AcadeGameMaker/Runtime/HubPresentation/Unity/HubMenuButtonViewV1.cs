using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AcadeGameMaker.Hub.Presentation.Unity
{
    // REQ-M5D7QA-003/005: inert view; input is interpreted solely by presenter.
    [DisallowMultipleComponent]
    internal sealed class HubMenuButtonViewV1 : Selectable
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _focusRail;
        [SerializeField] private Image _border;
        [SerializeField] private Image _strike;
        internal RectTransform HitRect { get { return transform as RectTransform; } }
        internal void ConfigureForAuthoring(TMP_Text label, Image focusRail, Image border, Image strike) { _label = label; _focusRail = focusRail; _border = border; _strike = strike; navigation = Navigation.defaultNavigation; var n = navigation; n.mode = Navigation.Mode.None; navigation = n; }
        internal void Render(string text, bool interactableValue, bool focused, bool hovered)
        {
            if (_label != null) _label.text = text;
            interactable = interactableValue;
            if (_focusRail != null) _focusRail.enabled = focused;
            if (_border != null) _border.enabled = focused || hovered;
            if (_strike != null) _strike.enabled = !interactableValue;
        }
    }
}
