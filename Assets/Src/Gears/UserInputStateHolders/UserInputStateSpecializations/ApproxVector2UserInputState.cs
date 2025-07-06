using UnityEngine;

namespace FUI.Gears {
    public class ApproxVector2UserInputState : UserInputState<UnityEngine.Vector2> {
        protected override bool Equals(UnityEngine.Vector2 a, UnityEngine.Vector2 b) {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y);
        }
    }
}
