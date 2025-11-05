using UnityEngine;

using System;
using System.Collections.Generic;
using FUI;

using static FUI.Shortcuts;
using static FUI.Basic;
using FUI.Modifiers;

internal class DebugForm : Form {

    //public ArtNetLightingDeviceDefinition.Model Data = new();

    public enum TestEnum {
        A = 2,
        B = 4,
        C = 8,
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

    public Texture TestTexture;
    public Texture TestTexture2;

    [Serializable]
    public class Item {
        public string Name;
        public string Icon;
        public Color Color;

        public Item(string name, string icon, Color color) {
            Name = name;
            Icon = icon;
            Color = color;
        }
    }

    public List<Item> Items;

    public struct ListItemRelocation {
        public int StartIndex;
    }


    void PopulateTabs(Color[] colors, int height = 150, float gap = 10) {
        float fraction = 1f / colors.Length;
        float gapCompensation = gap * (colors.Length - 1) / colors.Length;
        using (Group(P.Up(height))) {
            Rectangle(P.Fill, Color.white);
            for (int i = 0; i < colors.Length; i++) {
                var color = colors[i];
                Rectangle(P.Left(-gapCompensation, fraction), color);
                GapLeft(gap);
            }
        }
    }


    protected override void Build() {

        Item[] palette = new Item[] {
            new Item("Lemon","\ue079", new Color(1f,1f,0.3f)),
            new Item("Carrot","\uf787", new Color(1f,0.6f,0.2f)),
            new Item("Pepper","\uf816", new Color(1f,0.0f,0.1f)),
        };




        using (WindowBackground()) {



            using (Group(P.Right(50))) {
                Rectangle(P.Fill, Color.white);
                Padding(10);
                Rectangle(P.ColumnElement(3, 5, 2 * 10), Color.red);
                Rectangle(P.ColumnElement(3, 5, 2 * 10), Color.green);
                Rectangle(P.ColumnElement(3, 5, 2 * 10), Color.blue);
            }



            using (Panel(P.Left(50))) {

                for (int i = 0; i < palette.Length; i++) {
                    var item = palette[i];
                    Button(item.Icon, () => { Items.Add(item); MakeDirty(); }, P.Up(50));
                }


                Image_ToSRGB_IgnoreAlpha(P.Up(50), TestTexture);
                Image_ToSRGB_IgnoreAlpha(P.Up(50), TestTexture2, -Vector3.one);

                using (Group(P.Up(50))) {
                    Circle(P.Fill, Color.black, numSegments: 32);
                    Padding(4);
                    Circle(P.Fill, Color.white, TestFloat, numSegments: 32);
                }


                //CircleOutline(P.Up(100), Color.white, innerThickness: 1, numSegments: 32);
                CircleOutlineScreenSpaceThickness(P.Up(100), Color.white, screenSpaceThickness: 1, numSegments: 32);


            }



            GapTop(4);
            using (TempPaddingHorizontal(4, 4)) {




                using (TempPaddingHorizontal(10, 10)) {
                    PopulateTabs(new Color[] { Color.red, Color.green, Color.blue }, 50, 10);
                }

                using (Group(P.Up(50))) {
                    Padding(10);
                    Rectangle(P.RowElement(3, 10, 2 * 10), Color.red);
                    Rectangle(P.RowElement(3, 10, 2 * 10), Color.green);
                    Rectangle(P.RowElement(3, 10, 2 * 10), Color.blue);
                }


                /*if (ExpandableGroupHeader("ExpandableGroupHeader")) {
                    Label("Expanded");
                }*/
                GapTop(2);

                Button("Button", () => { }, P.Up(20));
                GapTop(2);



                LabeledCheckbox("Bool", TestBool, x => AssignAndMakeDirty(ref TestBool, x));
                GapTop(2);
                ToggleButton(TestBool, "Toggle", "Toggle", x => AssignAndMakeDirty(ref TestBool, x));
                GapTop(2);

                Label(TestInt.ToString());
                GapTop(2);

                LabeledInputField("Int", TestInt, x => AssignAndMakeDirty(ref TestInt, x));

                GapTop(2);
                LabeledInputFieldSpinbox("Float", TestFloat, x => AssignAndMakeDirty(ref TestFloat, x), 0.001f, null, "0.###");

                GapTop(2);
                LabeledInputFieldSpinbox("Double", TestDouble, x => AssignAndMakeDirty(ref TestDouble, x), 0.1f, null, "0.###");
                GapTop(2);

                LabeledDropdown("Enum", testEnum, x => AssignAndMakeDirty(ref testEnum, x));

                ToggleGroupButtons(testEnum, x => AssignAndMakeDirty(ref testEnum, x), null, 4, 0);

                Label($"Enum Value: {(int)testEnum}");



                GapTop(2);
                Label("The quick brown fox jumps over the lazy dog", P.Up(Theme.LineHeight), new SetTextOverflow(TMPro.TextOverflowModes.Linked));


                Label("Red text", P.Up(Theme.LineHeight), new SetColor(Color.red));
                Label("Bold text", P.Up(Theme.LineHeight), new SetFontStyle(TMPro.FontStyles.Bold));

                //Label($"Width: {GetWidth()}");
                var size = GetSize();
                Label($"Size: {size.x}x{size.y}");

                using (Group(P.Up(Theme.LineHeight))) {
                    ColorButton(new Color(0f, 0.6f, 0f), () => {
                        Debug.Log("Click!");
                    }, "Click me!", positioner: P.Left(150));
                    GapLeft(10);
                    ContentButton(new AddPressedHoveredHighlighter(Color.white.Alpha(0), Color.white.Alpha(0.05f), Color.white.Alpha(0.1f)), () => {
                        Text("Transparent Button", P.Fill, new SetColor(Color.white), new SetTextAlignmentCenterMiddle());
                    }, () => { Debug.Log("Click!"); }, P.Left(200));
                }
            }

            void Swap(int i) {
                var temp = Items[i];
                Items[i] = Items[i + 1];
                Items[i + 1] = temp;
            }

            const int buttonSize = 24;
            using (ScrollRectVertical(P.Fill)) {
                Padding(4);

                for (int i = 0; i < Items.Count; i++) {
                    int index = i;
                    var item = Items[i];
                    using (Panel(P.Up(24), 4)) {
                        GapLeft(4);
                        Label(item.Icon, P.Left(20));
                        GapLeft(10);

                        Button("\uf2ed", () => { Items.RemoveAt(index); MakeDirty(); }, P.Right(buttonSize));
                        GapRight(10);

                        if (i == (Items.Count - 1))
                            GapRight(buttonSize);
                        else
                            Button("\uf063", () => { Swap(index); MakeDirty(); }, P.Right(buttonSize));//Down

                        if (i == 0)
                            GapRight(buttonSize);
                        else
                            Button("\uf062", () => { Swap(index - 1); MakeDirty(); }, P.Right(buttonSize));//Up


                        Label(item.Name, P.Fill);
                        /*Button("ClickMe", () => Debug.Log("clicked"), PushRight());
                        Button("Delete", () => Debug.Log("clicked"), Color.red, PushRight());

                        IconButtonFontAwesome("\uf00c", 16, Color.white, () => Debug.Log("clicked"), PushRight(64));
                        */
                    }
                    GapTop(4);
                }
            }

        }

    }
}
