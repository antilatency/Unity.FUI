using UnityEngine;
using FUI;
using FUI.Gears;
using static FUI.Shortcuts;

#nullable enable

public class CanvasTest : Form {

    public float TestFloat = 1;
    protected override void Build() {

        

        using (WindowBackground()) {
            LabeledInputFieldSpinbox("Test Float",TestFloat, x => { TestFloat = x; MakeDirty(); }, 0.1f);

            Label("Hello, World!");
            VectorCanvas((context) => {
                context.Color = Color.red;
                context.Thickness = TestFloat;
                context.Line(new Vector2(0, 0), new Vector2(100, 100));
                context.Line(new Vector2(0, 100), new Vector2(100, 100));
                context.Color = Color.green;
                context.Circle(new Vector2(50, 50), 25);

                context.Color = Color.white;
                context.Rectangle(new Vector2(context.Size.x - 90, 10), new Vector2(80, 80));

                context.Thickness = 4f;
                context.CircleOutline(new Vector2(50, 50), 30);

                var canvasCenter = new Vector2(context.Size.x / 2, context.Size.y / 2);
                context.Ellipse(canvasCenter, new Vector2(40, 20), new Vector2(-20, 80));
                context.Color = Color.black;
                context.Thickness = TestFloat;
                context.EllipseOutline(canvasCenter, new Vector2(40, 20), new Vector2(-20, 80));
            });
        }
    }
}