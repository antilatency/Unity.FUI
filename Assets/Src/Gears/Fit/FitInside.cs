using UnityEngine;


namespace FUI {

    namespace Gears {
        public class FitInside : BaseAspectFit {

        }
    }
    namespace Modifiers {
        public class AddFitInside : Modifier {
            public Vector2 ContentSize;
            public float Scale;
            public Vector2 ViewportCenterInContent;
            public AddFitInside(Vector2 contentSize, float scale, Vector2 viewportCenterInContent) {
                ContentSize = contentSize;
                Scale = scale;
                ViewportCenterInContent = viewportCenterInContent;
            }
            public override void Create(GameObject gameObject) {
                gameObject.AddComponent<Gears.FitInside>();
            }
            public override void Update(GameObject gameObject) {
                var component = gameObject.GetComponent<Gears.FitInside>();
                component.ContentSize = ContentSize;
                component.Scale = Scale;
                component.ViewportCenterInContent = ViewportCenterInContent;
            }
        }

        /*public static partial class M {
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
        }*/
    }

}
