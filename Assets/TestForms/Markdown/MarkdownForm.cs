using FUI;
using System.IO;
using TMPro;
using UnityEngine;
using static FUI.Shortcuts;

namespace FUI {
}


public class MarkdownForm : Form {

    public string markdownText;

    protected override void Build() {

        try {
            string relativePath = "TestForms/Markdown/README.md";
            string fullPath = Path.Combine(Application.dataPath, relativePath);
            markdownText = File.ReadAllText(fullPath);
        }
        catch { }


        var unityText = new MarkdownConverter().Convert(markdownText);

        using (WindowBackground()) {

            using (ScrollRectVertical(P.Fill)) {
                Padding(4);
                LabelModifiable(P.Up()
                    , M.SetText(unityText)
                    , M.SetRichTextEnabled(true, false)
                    , M.SetWordWrapping(true, false)
                    , M.AddClickHandlerEx(Hyperlink.Handle));

            }
        }
    }
}
