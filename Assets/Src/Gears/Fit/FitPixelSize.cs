using UnityEngine;


namespace FUI {
    /*public static partial class M {
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
    }*/

    namespace Gears {
        public class FitPixelSize : BaseAspectFit {
            protected override float FitScale => 1f;
        }
    }
    namespace Modifiers {
        public class AddFitPixelSize : Modifier {
            public Vector2 ContentSize;
            public float Scale;
            public Vector2 ViewportCenterInContent;
            public AddFitPixelSize(Vector2 contentSize, float scale, Vector2 viewportCenterInContent) {
                ContentSize = contentSize;
                Scale = scale;
                ViewportCenterInContent = viewportCenterInContent;
            }
            public override void Create(GameObject gameObject) {
                gameObject.AddComponent<Gears.FitPixelSize>();
            }
            public override void Update(GameObject gameObject) {
                var component = gameObject.GetComponent<Gears.FitPixelSize>();
                component.ContentSize = ContentSize;
                component.Scale = Scale;
                component.ViewportCenterInContent = ViewportCenterInContent;
            }
        }
    }

}


