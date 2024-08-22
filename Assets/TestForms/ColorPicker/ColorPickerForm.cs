using UnityEngine;

using System;
using System.Collections.Generic;
using FUI;
using FUI.Gears;

internal class ColorPickerForm : Form {

    public string Hint;
    public Vector2 HueSaturation;


    Vector2 ColorPickerHueSaturation(Vector2 value, Positioner positioner) {
        var background = CreateControl(Library.Rectangle, "c4b537e2-a35a-4ab1-9f57-d253dda79a36", x => {
            x.gameObject.AddComponent<Slider>();
            x.gameObject.AddComponent<CustomShader>().ShaderName = "UI/HueSaturationRect";
        });
        var backgroundRectTransform = (RectTransform)background.transform;
        positioner(backgroundRectTransform, CurrentBorders, () => new Vector2(80, Theme.Instance.LineHeight));


        var handle = CreateSubControl(backgroundRectTransform, Library.Rectangle, x => {
            x.raycastTarget = false;
            x.color = Color.black;
            var rectTransform = (RectTransform)x.transform;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(4, 4);
        });

        var slider = background.GetComponent<Slider>();

        if (slider.NewUserInput) {
            value = slider.Value;
        }

        value.x = Mathf.Clamp01(value.x);
        value.y = Mathf.Clamp01(value.y);

        value = ToGrid(value, new Vector2Int(8, 8));

        if (!slider.Moving) {
            slider.Value = value;
        }


        var handleRectTransform = (RectTransform)handle.transform;

        handleRectTransform.anchorMin = value;
        handleRectTransform.anchorMax = value;
        handleRectTransform.pivot = value;
        return value;
    }

    Vector2 ToGrid(Vector2 value, Vector2Int grid) {
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

        using (GroupBackground(Fill)) {
            Padding(4);
            Hint = LabeledInputField("Hint", Hint);
            GapTop(4);
            using (Group(PushUp(100))) {
                Rectangle(Color.HSVToRGB(HueSaturation.x, HueSaturation.y, 1), PushLeft(20, 0.2f));
                GapLeft(4);
                HueSaturation = ColorPickerHueSaturation(HueSaturation, Fill);
                
            }
            GapTop(4);
            using (Group(PushUp(20))) {
                var width = 1f / colorShortcuts.Length;
                var gap = 4;
                var gapCompensation = -gap * (colorShortcuts.Length - 1) / colorShortcuts.Length;
                for (int i = 0; i < colorShortcuts.Length; i++) {
                    if (i > 0) GapLeft(gap);

                    var shortcut = colorShortcuts[i];
                    Button(shortcut.Name, () => {
                        HueSaturation = shortcut.hueSaturation;
                        Hint = shortcut.Name;
                    }, Color.HSVToRGB(shortcut.hueSaturation.x, shortcut.hueSaturation.y, 1), PushLeft(gapCompensation, width));

                }
            }
        }
    }

}
