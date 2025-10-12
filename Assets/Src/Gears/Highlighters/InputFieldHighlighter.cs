using System.Linq;

using FUI.Gears;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Modifiers {
    public class AddFocusHoverHighlighter : Modifier {
        public override void Create(GameObject gameObject) {
            var highlighter = gameObject.AddComponent<FocusHoverHighlighter>();
            highlighter.UpdateBinding(Form.Current);
        }

        public override void Update(GameObject gameObject) {
            var highlighter = gameObject.GetComponent<FocusHoverHighlighter>();
            highlighter?.UpdateBinding(Form.Current);
        }
    }

    public class SetInputFieldSelectionColor : SetterModifier {
        public Color SelectionColor;
        public SetInputFieldSelectionColor(Color selectionColor) {
            SelectionColor = selectionColor;
        }
        public override void Set(GameObject gameObject) {
            var inputField = gameObject.GetComponent<FUI_InputField>();
            if (inputField != null) {
                inputField.selectionColor = SelectionColor;
            }
        }
    }
}

namespace FUI {
    public interface IFocusHandler : IEventSystemHandler {
        void OnFocusChanged(bool focused);
    }
}
namespace FUI.Gears {
    //[RequireComponent(typeof(RoundedRectangleWithOutline))]
    public class FocusHoverHighlighter : AbstractHighlighter, IFocusHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler {


        Theme _theme;
        public Theme Theme { get { return _theme; } set { if (SetPropertyUtility.SetClass(ref _theme, value)) UpdateAppearance(); } }
        RoundedRectangleWithOutline background;
        bool _focused;
        public bool Focused { get { return _focused; } set { if (SetPropertyUtility.SetStruct(ref _focused, value)) UpdateAppearance(); } }

        /*protected override void OnEnable() {
            base.OnEnable();
            CheckComponent(ref input);
            CheckComponent(ref background);
            color = InitialColor;
        }*/

        bool _hovered;
        public bool Hovered { get { return _hovered; } set { if (SetPropertyUtility.SetStruct(ref _hovered, value)) UpdateAppearance(); } }


        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            Hovered = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            Hovered = false;
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData) {
            Hovered = !Enumerable.Range(0, 3).Any(x => Input.GetMouseButton(x)) && eventData.hovered.Contains(this.gameObject);
        }

        void IFocusHandler.OnFocusChanged(bool focused) {
            Focused = focused;
        }

        public void ThemeChanged(Theme theme) {
            if (theme == _theme)
                UpdateAppearance();
        }

        public void UpdateBinding(Form form) {
            Theme = form.Theme;
            /*CheckComponent(ref input);
            input.onFocusChanged = (s, f) => {
                Focused = f;
            };*/
        }

        public void UpdateAppearance() {
            CheckComponent(ref background);
            background.SetAllCorners(_theme.Radius);
            background.OutlineWidth = _focused ? -_theme.OutlineThickness : 0;
            background.OutlineColor = _theme.InputOutlineColor;
            background.color = _hovered ? _theme.InputHoveredColor : _theme.InputColor;
        }


        private void Update() {
            /*var selected = EventSystem.current.currentSelectedGameObject == input.gameObject;
            var editing = input.isFocused && selected;

            color = editing
                ? EditingColor
                : hoveredState.Hovered
                    ? HoveredColor
                    : InitialColor;*/
        }

        
    }

}

