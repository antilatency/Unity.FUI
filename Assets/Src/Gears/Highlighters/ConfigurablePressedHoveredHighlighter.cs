using UnityEngine;
namespace FUI.Gears {
    public class ConfigurablePressedHoveredHighlighter : PressedHoveredHighlighter {

        public Color initialColor;
        public Color hoveredColor;
        public Color pressedColor;

        protected override Color InitialColor => initialColor;
        protected override Color HoveredColor => hoveredColor;
        protected override Color PressedColor => pressedColor;
    }
}