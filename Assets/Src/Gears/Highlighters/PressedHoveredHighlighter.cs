using UnityEngine;
namespace FUI.Gears {
    public class PressedHoveredHighlighter : AbstractHighlighter {



        public Color InitialColor { get; set; }
        public Color HoveredColor { get; set; }
        public Color PressedColor { get; set; }


        PressedState pressedState;
        HoveredState hoveredState;

        protected override void OnEnable() {
            base.OnEnable();
            CheckComponent(ref pressedState);
            CheckComponent(ref hoveredState);
            color = InitialColor;
        }

        private void Update() {
            color = pressedState.Pressed
                ? PressedColor
                : hoveredState.Hovered
                    ? HoveredColor
                    : InitialColor;
        }
    }
}