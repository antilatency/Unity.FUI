using UnityEngine;
using FUI;
using static FUI.Shortcuts;
using FUI.Modifiers;

#nullable enable

public class FontLinearOrGammaTest : Form {
	
    void ColoredLabel(string text, Color color) {
        Label(text, P.Up(), new SetColor(color));
    }

    public void PopulateLabels(Color backgroundColor) {
        var gap = 8f;
        Rectangle(P.Fill, backgroundColor);
        Padding(10);
        ColoredLabel("Brown Fox Jumped Over The Lazy Dog", Color.white);
        GapTop(gap);
        ColoredLabel("Brown Fox Jumped Over The Lazy Dog", Color.gray);
        GapTop(gap);
        ColoredLabel("Brown Fox Jumped Over The Lazy Dog", Color.black);
        GapTop(gap);
        ColoredLabel("Brown Fox Jumped Over The Lazy Dog", Color.blue);
        GapTop(gap);
        ColoredLabel("Brown Fox Jumped Over The Lazy Dog", Color.red);
        GapTop(gap);
        ColoredLabel("Brown Fox Jumped Over The Lazy Dog", Color.green);
    }

    protected override void Build() {
//TMPro.TMP_MaterialManager.GetMaterialForRendering
        using (Group(P.Up(0, 0.5f))) {
            using (Group(P.Left(0, 0.5f))) {
                PopulateLabels(Color.white);
            }

            using (Group(P.Right(0, 0.5f))) {
                PopulateLabels(Color.black);
            }
        }

        using (Group(P.Down(0, 0.5f))) {
            using (Group(P.Left(0, 0.5f))) {
                PopulateLabels(new Color(0.25f, 0.25f, 0.25f, 1f)); // Dark gray
            }

            using (Group(P.Right(0, 0.5f))) {
                PopulateLabels(new Color(0.75f, 0.75f, 0.75f, 1f)); // Light gray
            }
        }

    }
}