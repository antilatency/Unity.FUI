using UnityEngine;
namespace FUI.Gears {
    public class ButtonHighlighter : PressedHoveredHighlighter {

        protected override Color InitialColor => Theme.Instance.ButtonColor;
        protected override Color HoveredColor => Theme.Instance.ButtonColorHovered;
        protected override Color PressedColor => Theme.Instance.ButtonColorPressed;
    }
}