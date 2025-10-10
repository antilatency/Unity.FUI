using FUI;
using static FUI.Shortcuts;
using TMPro;
using FUI.Gears;
using System;

#nullable enable

public class ThemeTest : Form {
    public int SelectedIndex = 0;
    public bool Selected = false;
    public string StringValue = "Hello";
    public float FloatValue = 1.0f;
    public TextOverflowModes textOverflowModes = TextOverflowModes.Overflow;




    protected override void Build() {


        //TMP_Text text = new TMP_Text();
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
                ToggleGroupButtons(SelectedIndex, options, x => { SelectedIndex = x; MakeDirty(); });
                GapTop(4);
                ToggleGroupButtons(2 - SelectedIndex, new string[] { "\ue448", "\uf4ba", "\uf6be" }, x => { SelectedIndex = 2 - x; MakeDirty(); });
                GapTop(4);


                ToggleButton(Selected, "On", "Off", x => { Selected = x; MakeDirty(); });
                GapTop(4);
                Checkbox(Selected, x => { Selected = x; MakeDirty(); });

                GapTop(4);
                /*Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta };

                Row(colors.Length, (i, p) => {
                    Color color = colors[i];
                    ColorButton(color, () => { }, i.ToString(), p);
                });*/

                using (Group(P.Up(Theme.LineHeight))) {

                    Button("\uf0c7", () => { }, P.Left());

                    Button("\uf142", (g, e) => {
                        MenuDialog.Open(g, SelectedIndex, (value) => {
                            SelectedIndex = value;
                            MakeDirty();
                        }, 50, options);
                    }, P.Right());
                }
                //EnumHelper
                Dropdown(textOverflowModes, x => { textOverflowModes = x; MakeDirty(); });

                /*Button("Text Overflow Modes", (g, e) => {
                    DropDownDialog.Open(g, textOverflowModes, (value) => {
                        textOverflowModes = value;
                        MakeDirty();
                    });
                });*/

            }

            using (Group(P.Left(0, 0.25f))) {
                Padding(4);
                Label("Inputs");

                InputField(StringValue, x => AssignAndMakeDirty(ref StringValue, x));

                GapTop();
                LabeledInputField("FloatValue", FloatValue, x => AssignAndMakeDirty(ref FloatValue, x));
                GapTop();
                LabeledInputFieldSpinbox("FloatValue", FloatValue, x => AssignClampedAndMakeDirty(ref FloatValue, x, 0, 1), 0.01f);
                GapTop();
                LabeledDropdown("Text Overflow Mode", textOverflowModes, x => { textOverflowModes = x; MakeDirty(); });
                GapTop();
                LabeledCheckbox("Bool", Selected, x => { Selected = x; MakeDirty(); });

            }
        }
    }
}