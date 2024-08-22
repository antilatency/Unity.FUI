using UnityEngine;
namespace FUI.Gears {
    public class DropdownHighlighter : AbstractHighlighter {

        protected Color InitialColor => Theme.Instance.ButtonColor;
        protected Color HoveredColor => Theme.Instance.ButtonColorHovered;
        protected Color EditingColor => Theme.Instance.ButtonColorPressed;

        HoveredState hoveredState;



        protected override void OnEnable() {
            base.OnEnable();
            CheckComponent(ref hoveredState);
            color = InitialColor;
        }

        private void Update() {
            var input = GetComponent<TMPro.TMP_Dropdown>();
            var editing = input.IsExpanded;
            color = editing
                ? EditingColor
                : hoveredState.Hovered
                    ? HoveredColor
                    : InitialColor;
        }
    }
}