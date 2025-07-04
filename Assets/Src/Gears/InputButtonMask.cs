using static UnityEngine.EventSystems.PointerEventData;

namespace FUI.Gears {
    public enum InputButtonMask {
        Left = 1 << InputButton.Left,
        Right = 1 << InputButton.Right,
        Middle = 1 << InputButton.Middle,
        All = Left | Right | Middle
    }
}
