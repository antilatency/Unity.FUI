using UnityEngine;
namespace FUI.Gears {
    public class DropdownHighlighter : ButtonHighlighter {



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
                ? PressedColor
                : hoveredState.Hovered
                    ? HoveredColor
                    : InitialColor;
        }
    }
}