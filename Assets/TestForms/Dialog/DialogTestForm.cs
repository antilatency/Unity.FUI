using FUI;
using static FUI.Shortcuts;

using UnityEngine;

public class DialogTestForm : Form {

    public Vector2 G;

    protected override void Build() {
        Rectangle(P.Fill, Theme.Instance.WindowBackgroundColor);

        G = new Vector2(
            Slider(G.x,null, Color.black),
            Slider(G.y,null, Color.black)
            );


        Rectangle(P.Absolute(Vector2.zero, 100, 100, G), Color.red);

    }
}
