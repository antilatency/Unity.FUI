using UnityEngine;


namespace FUI {
    public static partial class M {
        public static Modifier AddFitPixelSize(Vector2 contentSize, float scale, Vector2 viewportCenterInContent) =>
            new Modifier(
                "AddFitPixelSize",
                x => x.AddComponent<Gears.FitPixelSize>(),
                x => {
                    var component = x.GetComponent<Gears.FitPixelSize>();
                    component.ContentSize = contentSize;
                    component.Scale = scale;
                    component.ViewportCenterInContent = viewportCenterInContent;
                }
                );
    }
}


namespace FUI.Gears {
    public class FitPixelSize : BaseAspectFit {
        protected override float FitScale => 1f;
    }
}