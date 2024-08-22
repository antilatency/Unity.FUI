using FUI;
using UnityEngine;

public class DialogTestForm : Form {

    public Vector2 G;

    protected override void Build() {
        Rectangle(Theme.Instance.WindowBackgroundColor, Fill);

        G = new Vector2(
            Slider(G.x,null, Color.black),
            Slider(G.y,null, Color.black)
            );


        Rectangle(Color.red, Gravity(G, Vector2.one * 100));

    }
}
