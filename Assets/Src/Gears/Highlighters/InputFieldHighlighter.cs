using System.Linq;

using FUI.Gears;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Modifiers {
    public class AddInputFieldHighlighter : Modifier {
        public override void Create(GameObject gameObject) {
            var highlighter = gameObject.AddComponent<InputFieldHighlighter>();
            highlighter.UpdateBinding(Form.Current);
        }

        public override void Update(GameObject gameObject) {
            var highlighter = gameObject.GetComponent<InputFieldHighlighter>();
            highlighter?.UpdateBinding(Form.Current);
        }
    }
}


namespace FUI.Gears {
    //[RequireComponent(typeof(RoundedRectangleWithOutline))]
    public class InputFieldHighlighter : AbstractHighlighter, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler {


        Theme _theme;
        public Theme Theme { get { return _theme; } set { if (SetPropertyUtility.SetClass(ref _theme, value)) UpdateAppearance(); } }
        FUI_InputField input;
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

        public void ThemeChanged(Theme theme) {
            if (theme == _theme)
                UpdateAppearance();
        }

        public void UpdateBinding(Form form) {
            Theme = form.Theme;
            CheckComponent(ref input);
            input.onFocusChanged = (s, f) => {
                Focused = f;
            };
        }

        public void UpdateAppearance() {
            CheckComponent(ref background);
            background.SetAllCorners(_theme.Radius);
            background.OutlineWidth = _focused ? -_theme.OutlineThickness : 0;
            background.OutlineColor = _theme.InputOutlineColor;
            background.color = _hovered ? _theme.InputHoveredColor : _theme.InputColor;

            CheckComponent(ref input);
            input.selectionColor = _theme.InputSelectionColor;
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

