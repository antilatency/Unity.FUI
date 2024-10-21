using UnityEngine;

namespace FUI.Gears {
    public class FitInside : BaseAspectFit {
        public override float? CalcFitScale(Vector2 viewportSize, Vector2 contentSize) {
            if (ContentSize.x == 0 || ContentSize.y == 0)
                return 1f;
            var scale2d = ViewportSize / ContentSize;
            return Mathf.Min(scale2d.x, scale2d.y);
        }
    }
}