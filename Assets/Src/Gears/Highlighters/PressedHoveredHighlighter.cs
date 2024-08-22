using UnityEngine;
namespace FUI.Gears {
    public abstract class PressedHoveredHighlighter : AbstractHighlighter {

        protected abstract Color InitialColor { get; }
        protected abstract Color HoveredColor { get; }
        protected abstract Color PressedColor { get; }


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