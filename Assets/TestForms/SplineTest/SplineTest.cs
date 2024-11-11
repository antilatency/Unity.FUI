using FUI;
using UnityEngine;
using static FUI.Shortcuts;

public class SplineTest : Form
{
    protected override void Build() {
        using (ZoomPanViewport(P.Fill, new(500, 200))) {
            float thickness = 10;
            Vector2 a = Vector2.zero;
            Vector2 b = Vector2.one;
            var hSize = b.x - a.x;
            var hSizeThirs = hSize/3.0f;


            Circle(P.Absolute(Vector2.zero, 8* thickness, 4* thickness, b,Vector2.one * 0.5f), Color.white, 0.5f, -0.25f, numSegments: 2);


            CubicSpline(P.Fill, Color.white,
                Vector2.zero, hSizeThirs * Vector2.right,
                Vector2.one, -hSizeThirs * Vector2.right,
                0.5f*thickness, 0.5f*thickness,
                128
                );

            Circle(P.Absolute(Vector2.zero, thickness, thickness, a, Vector2.one * 0.5f), Color.white, 0.5f, 0.25f,  16);
        }
    }
}
