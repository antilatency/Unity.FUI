using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI.Gears {
    public class InputFieldHighlighter : AbstractHighlighter {
        protected Color InitialColor => Theme.Instance.InputColor;
        protected Color HoveredColor => Theme.Instance.InputColorHovered;
        protected Color EditingColor => Theme.Instance.InputColorEditing;

        HoveredState hoveredState;
        FUI_InputField input;

        protected override void OnEnable() {
            base.OnEnable();
            CheckComponent(ref hoveredState);
            CheckComponent(ref input);
            color = InitialColor;
        }

        private void Update() {
            var selected = EventSystem.current.currentSelectedGameObject == input.gameObject;
            var editing = input.isFocused && selected;

            color = editing
                ? EditingColor
                : hoveredState.Hovered
                    ? HoveredColor
                    : InitialColor;
        }
    }

}

