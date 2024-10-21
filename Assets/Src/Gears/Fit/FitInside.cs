using UnityEngine;


namespace FUI {
    public static partial class M {
        public static Modifier AddFitInside(Vector2 contentSize, float scale, Vector2 viewportCenterInContent) =>
            new Modifier(
                "AddFitInside",
                x => x.AddComponent<Gears.FitInside>(),
                x => {
                    var component = x.GetComponent<Gears.FitInside>();
                    component.ContentSize = contentSize;
                    component.Scale = scale;
                    component.ViewportCenterInContent = viewportCenterInContent;
                }
                );
    }
}


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