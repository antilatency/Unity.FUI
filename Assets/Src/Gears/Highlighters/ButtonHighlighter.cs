using System;

using UnityEngine;
#nullable enable
namespace FUI.Gears {
    [Obsolete]
    public interface IFormDependentComponent {
        Form Form { get; set; }
    }

    [Obsolete]
    public class ButtonHighlighter : PressedHoveredHighlighter, IFormDependentComponent {
        public Form Form { get; set; } = null!;
        protected override Color InitialColor => Form.Theme.ButtonColor;
        protected override Color HoveredColor => Form.Theme.HoverColor(InitialColor);
        protected override Color PressedColor => Form.Theme.PressedColor(InitialColor);
    }
}