using FUI;
using FUI.Modifiers;

using System.IO;
using UnityEngine;
using static FUI.Shortcuts;



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
                Label(unityText, P.Up()
                    , new SetRichTextEnabled(true)
                    , new SetWordWrapping(true)
                    , new AddClickHandlerEx(Hyperlink.Handle));

            }
        }
    }
}
