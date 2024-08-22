using UnityEngine;

using System;
using System.Collections.Generic;
using FUI;
using FUI.Gears;

internal class DebugForm : Form {

    //public ArtNetLightingDeviceDefinition.Model Data = new();

    public enum TestEnum { 
        A,B,C
    }
    TestEnum testEnum;

    struct ChannelRelocation {
        public int SourceChannel;
    }

    public string TestText;
    public double TestDouble;
    public float TestFloat;
    public int TestInt;
    public bool TestBool;



    protected override void Build() {

        (string Name, Vector2 hueSaturation)[] colorShortcuts = new[] { 
            ("R",new Vector2(0.0f, 0.9f)),
            ("G",new Vector2(0.33f, 0.9f)),
            ("B",new Vector2(0.66f, 0.9f)),
            ("T",new Vector2(0.1f, 0.2f)),
            ("D",new Vector2(0.55f, 0.2f)),
        };



        using (GroupBackground(Fill)) {

            TestFloat = Slider(TestFloat, PushUp());

            if (ExpandableGroupHeader("Open me", PushUp())) {
                using (Group(PushUp())) {
                    Padding(20, 4, 4, 4);
                    Label("Opened");
                    TestBool = LabeledCheckbox("LabeledCheckbox", TestBool);
                }
            }


            using (DraggableGroup(() => 8, PushUp(50))) {
                Rectangle(Color.red, Fill);
            }



            using (ScrollRectVertical(Fill)) {
                Padding(4);
                TestInt = LabeledInputField("Count", TestInt);
                for (int i = 0; i < TestInt; i++) {

                    using (Group(PushUp(20))) {


                        Button("ClickMe", () => Debug.Log("clicked"), PushRight());
                        Button("Delete", () => Debug.Log("clicked"), Color.red, PushRight());

                        IconButtonFontAwesome("\uf00c", 16, Color.white, () => Debug.Log("clicked"), PushRight(64));
                        Label("Hello 6546546", Fill);
                    }
                    GapTop(2);
                    testEnum = Dropdown(testEnum, PushUp());
                    GapTop(5);
                }
                //Button("ClickMe", new Color(0.5f, 0.5f, 0.5f, 1), () => Debug.Log("clicked"), PushUp());
                GapTop(1);
                TestFloat = InputField(TestFloat, PushUp());
                GapTop(1);
                TestFloat = LabeledInputField("double", TestFloat);
            }

                

        }



    }
}
