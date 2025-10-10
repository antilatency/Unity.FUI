using FUI;
using FUI.Modifiers;

using System;
using TMPro;
using UnityEngine;
using static FUI.Shortcuts;

public class RichTextForm : Form
{
    protected override void Build() {

        using (ZoomPanViewport(P.Fill, new(640, 480))) {
            var text = @$"Hello <i>italic</i> {Hyperlink.Create("stackoverflow.com", new Uri("https://stackoverflow.com/"))}
NewLine
� first
� second
";
            Label(text, P.Up()
                , new SetRichTextEnabled(true)
                , new AddClickHandlerEx(Hyperlink.Handle));
        }
    }
}
