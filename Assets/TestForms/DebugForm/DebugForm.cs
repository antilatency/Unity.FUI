using UnityEngine;

using System;
using System.Collections.Generic;
using FUI;
using FUI.Gears;
using static FUI.Shortcuts;

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





    protected override void Build() {

        Item[] palette = new Item[] { 
            new Item("Lemon","\ue079", new Color(1f,1f,0.3f)),
            new Item("Carrot","\uf787", new Color(1f,0.6f,0.2f)),
            new Item("Pepper","\uf816", new Color(1f,0.0f,0.1f)),
        };


        using (WindowBackground()) {

            
            

            using (Panel(P.Left(50))) {

                for (int i = 0; i < palette.Length; i++) {
                    var item = palette[i];
                    IconButtonFontAwesome(item.Icon, 16, item.Color, x => { Items.Add(item); }, P.Up(50));
                }
            }

            

            GapTop(4);
            using (TempPaddingHorizontal(4, 4)) {

                Button("Button", x => { }, P.Up(20));
                GapTop(2);

                ColoredButton("Button", x => { },null, P.Up(20));
                GapTop(2);

                TestBool = LabeledCheckbox("Bool", TestBool);
                GapTop(2);
                TestInt = LabeledInputField("Int", TestInt);
                GapTop(2);
                TestFloat = LabeledInputField("Float", TestFloat);
                GapTop(2);
                TestDouble = LabeledInputField("Double", TestDouble);
                GapTop(2);
                testEnum = LabeledDropdown("Enum", testEnum);
            }

            void Swap(int i) {
                var temp = Items[i];
                Items[i] = Items[i+1];
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
                        IconFontAwesome(item.Icon, 16, item.Color, P.Left(20));
                        GapLeft(10);

                        IconButtonFontAwesome("\uf2ed", 12, x => { Items.RemoveAt(index); }, P.Right(buttonSize));                        
                        GapRight(10);

                        if (i == (Items.Count - 1))
                            GapRight(buttonSize);
                        else
                            IconButtonFontAwesome("\uf063", 12, x => { Swap(index); }, P.Right(buttonSize));//Down

                        if (i == 0)
                            GapRight(buttonSize);
                        else
                            IconButtonFontAwesome("\uf062", 12, x => { Swap(index-1); }, P.Right(buttonSize));//Up
                        

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
