using FUI;
using System;
using TMPro;
using UnityEngine;
using static FUI.Shortcuts;

public class RichTextForm : Form
{
    protected override void Build() {

        using (ZoomPanViewport(P.Fill, new(640, 480))) {
            LabelModifiable(P.Up()
                , M.SetText(@$"Hello <i>italic</i> {Hyperlink.Create("stackoverflow.com", new Uri("https://stackoverflow.com/"))}
NewLine
• first
• second
")
                , M.SetRichTextEnabled(true)
                , M.AddClickHandlerEx(Hyperlink.Handle));
        }
    }
}
