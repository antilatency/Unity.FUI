using UnityEngine;
using FUI;
using FUI.Gears;
using static FUI.Shortcuts;
using static FUI.Basic;
using FUI.Modifiers;

internal class ColorPickerForm : Form {

    public string Hint;
    public Vector2 HueSaturation;

    static Vector2 ColorPickerHueSaturation(Vector2 value, Positioner positioner, bool extraIteration = false) {
        var form = Form.Current;
        var theme = form.Theme;
        var background = Element(null
            ,new AddComponent<RoundedRectangle>()
            ,new AddComponent<Slider>()
            ,new SetCustomShader("UI/HueSaturationRect")
        );

        positioner(background, form.CurrentBorders, ()=>new Vector2(80, theme.LineHeight));

        form.BeginControls(background);
        var handle = Element(null
            , new AddComponent<RoundedRectangle>()
            , new SetColor(Color.black)
            , new SetRaycastTarget(false)
        );
        form.EndControls();
        throw new System.NotImplementedException("returnAction not implemented");
        /*var slider = background.GetComponent<Slider>();
        slider.SetFormToNotify(form, extraIteration);
        if (slider.NewUserInput) {
            value = slider.Value;
        }
        value.x = Mathf.Clamp01(value.x);
        value.y = Mathf.Clamp01(value.y);
        value = ToGrid(value, new Vector2Int(8, 8));
        if (!slider.Moving) {
            slider.Value = value;
        }*/

        handle.anchorMin = value;
        handle.anchorMax = value;
        handle.pivot = value;
        handle.anchoredPosition = Vector2.zero;
        handle.sizeDelta = new Vector2(4, 4);
        return value;
    }

    static Vector2 ToGrid(Vector2 value, Vector2Int grid) {
        value = value * grid;
        value.x = Mathf.RoundToInt(value.x);
        value.y = Mathf.RoundToInt(value.y);
        value = value / grid;
        return value;
    }

    protected override void Build() {

        (string Name, Vector2 hueSaturation)[] colorShortcuts = new[] {
            ("R",new Vector2(0.0f, 7f/8)),
            ("G",new Vector2(2f/8, 7f/8)),
            ("B",new Vector2(5f/8, 7f/8)),
            ("T",new Vector2(1f/8, 2f/8)),
            ("D",new Vector2(5f/8, 2f/8)),
        };

        using (WindowBackground()) {
            Padding(4);
            LabeledInputField("Hint", Hint, x => { Hint = x; MakeDirty(); });
            GapTop(4);
            using (Group(P.Up(100))) {
                Rectangle(P.Left(20, 0.2f), Color.HSVToRGB(HueSaturation.x, HueSaturation.y, 1));
                GapLeft(4);
                HueSaturation = ColorPickerHueSaturation(HueSaturation, P.Fill);
                
            }
            GapTop(4);
            using (Group(P.Up(20))) {
                var width = 1f / colorShortcuts.Length;
                var gap = 4;
                var gapCompensation = -gap * (colorShortcuts.Length - 1) / colorShortcuts.Length;
                for (int i = 0; i < colorShortcuts.Length; i++) {
                    if (i > 0) GapLeft(gap);

                    var shortcut = colorShortcuts[i];
                    ColorButton(Color.HSVToRGB(shortcut.hueSaturation.x, shortcut.hueSaturation.y, 1), () => {
                        HueSaturation = shortcut.hueSaturation;
                        Hint = shortcut.Name;
                        MakeDirty();
                    }
                    ,shortcut.Name, P.Left(gapCompensation, width));

                }
            }
        }
    }

}
