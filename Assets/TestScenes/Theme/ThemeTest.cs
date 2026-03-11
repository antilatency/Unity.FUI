using FUI;
using static FUI.Shortcuts;
using TMPro;
using static FUI.Basic;
using UnityEngine;
using FUI.Gears;
using FUI.Modifiers;
using System;

#nullable enable

public class ThemeTest : Form {
    public int SelectedIndex = 0;
    public bool Selected = false;
    public string StringValue = "Hello";
    public float FloatValue = 1.0f;
    public TextOverflowModes textOverflowModes = TextOverflowModes.Overflow;

    public Vector3 Vector3Value = new Vector3(1, 2, 3);


    class Vector3Form : ValueForm<Vector3> { 
        protected override void Build() {
            LabeledInputFieldSpinbox("X", Value.x, v => AssignAndReturn(ref Value.x, v));
            GapTop();
            LabeledInputFieldSpinbox("Y", Value.y, v => AssignAndReturn(ref Value.y, v));
            GapTop();
            LabeledInputFieldSpinbox("Z", Value.z, v => AssignAndReturn(ref Value.z, v));
        }
    }

    protected override void Build() {

        using (WindowBackground()) {

            using (Group(P.Left(0, 0.25f))) {
                Padding(4);
                Label("Buttons");
                GapTop(8);
                Button("Normal Button", () => { });
                GapTop(4);
                DisabledButton("Disabled Button");
                GapTop(4);
                var options = new[] { "A", "B", "C" };
                ToggleGroupButtons(SelectedIndex, options, x => AssignAndMakeDirty(ref SelectedIndex, x));
                GapTop(4);
                ToggleGroupButtons(2 - SelectedIndex, new string[] { "\ue448", "\uf4ba", "\uf6be" }, x => { SelectedIndex = 2 - x; MakeDirty(); });
                GapTop(4);


                ToggleButton(Selected, "Enabled", "Disabled", x => AssignAndMakeDirty(ref Selected, x));
                GapTop(4);
                Checkbox(Selected, x => AssignAndMakeDirty(ref Selected, x));

                GapTop(4);

                using (Group(P.Up(Theme.LineHeight))) {

                    Button("\uf0c7", () => { }, P.Left());

                    Button("\uf142", (g, e) => {
                        Dialog<MenuDialog>().Configure(g, SelectedIndex, (value) => {
                            SelectedIndex = value;
                            MakeDirty();
                        }, 50, options);
                    }, P.Right());

                }

                GapTop(4);
                Button("Show Yes/No Dialog", (g, e) => {
                    Dialog<MessageDialogYesNo>().Configure(x => {
                        Debug.Log("Dialog closed with result: " + x);
                    }, "Do you want to proceed?", "Proceed", "Cancel");
                });
            }

            using (Group(P.Left(0, 0.25f))) {
                Padding(4);
                Label("Inputs");
                InputField(StringValue, x => AssignAndMakeDirty(ref StringValue, x));
                GapTop();
                LabeledInputField("FloatValue", FloatValue, x => AssignAndMakeDirty(ref FloatValue, x));
                GapTop();
                LabeledInputFieldSpinbox("FloatValue", FloatValue, x => AssignAndMakeDirty(ref FloatValue, Mathf.Clamp(x, 0, 2)), 0.01f);
                GapTop();
                LabeledDropdown("Text Overflow Mode", textOverflowModes, x => AssignAndMakeDirty(ref textOverflowModes, x));
                GapTop();
                Dropdown(textOverflowModes, x => AssignAndMakeDirty(ref textOverflowModes, x));
                GapTop();
                LabeledCheckbox("Bool", Selected, x => AssignAndMakeDirty(ref Selected, x));
                GapTop();
                Slider(FloatValue / 2, x => AssignAndMakeDirty(ref FloatValue, Mathf.Clamp(2 * x, 0, 2)));
            }

            using (ScrollRectVertical(P.Left(0, 0.25f))) {
                Padding(4);
                for (int i = 0; i < 10; i++) {
                    if (i > 0) GapTop();
                    SubForm<Vector3Form>()
                    .Execute(Vector3Value, x => AssignAndMakeDirty(ref Vector3Value, x))
                    .ApplyPositioner(P.Up());

                }
            }



        }
    }
}