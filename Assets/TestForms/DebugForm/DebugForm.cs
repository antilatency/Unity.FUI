﻿using UnityEngine;

using System;
using System.Collections.Generic;
using FUI;
using FUI.Gears;
using static FUI.Shortcuts;

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





    protected override void Build() {

        Item[] palette = new Item[] { 
            new Item("Lemon","\ue079", new Color(1f,1f,0.3f)),
            new Item("Carrot","\uf787", new Color(1f,0.6f,0.2f)),
            new Item("Pepper","\uf816", new Color(1f,0.0f,0.1f)),
        };




        using (WindowBackground()){


            using (Panel(P.Left(50))) {

                for (int i = 0; i < palette.Length; i++) {
                    var item = palette[i];
                    IconButtonFontAwesome(item.Icon, 16, item.Color, () => { Items.Add(item); MakeDirty(); }, P.Up(50));
                }


                Image_ToSRGB_IgnoreAlpha(P.Up(50), TestTexture);
                Image_ToSRGB_IgnoreAlpha(P.Up(50), TestTexture2, -Vector3.one);

                using (Group(P.Up(50))) {
                    Circle(P.Fill, Color.black,numSegments:32);
                    Padding(4);
                    Circle(P.Fill, Color.white, TestFloat, numSegments: 32);
                }


                //CircleOutline(P.Up(100), Color.white, innerThickness: 1, numSegments: 32);
                CircleOutlineScreenSpaceThickness(P.Up(100), Color.white, screenSpaceThickness: 1, numSegments: 32);


            }

            

            GapTop(4);
            using (TempPaddingHorizontal(4, 4)) {
                if (ExpandableGroupHeader("ExpandableGroupHeader")){
                    Label("Expanded");
                }
                GapTop(2);
                
                Button("Button", () => { }, P.Up(20));
                GapTop(2);

                ColoredButton("ColoredButton", () => { },null, P.Up(20));
                GapTop(2);

                TestBool = LabeledCheckbox("Bool", TestBool);
                GapTop(2);

                Label(TestInt.ToString());
                GapTop(2);

                TestInt = LabeledInputField("Int", TestInt, numExtraIterations: 1);   
                GapTop(2);


                TestFloat = ClampMakeDirty(
                    LabeledInputFieldSpinbox("Float",TestFloat,0.001f, null, "0.###")
                    ,0
                    ,1);

                GapTop(2);
                TestDouble = LabeledInputFieldSpinbox("Double", TestDouble, 0.1f, null, "0.###");
                GapTop(2);                

                testEnum = LabeledDropdown("Enum", testEnum);
                Label($"Enum Value: {(int)testEnum}");

                GapTop(2);
                LabelModifiable(P.Up(Theme.Instance.LineHeight), M.SetText("The quick brown fox jumps over the lazy dog"), M.SetTextOverflow(TMPro.TextOverflowModes.Linked));


                LabelModifiable(P.Up(Theme.Instance.LineHeight), M.SetText("Red text"), M.SetColor(Color.red));
                LabelModifiable(P.Up(Theme.Instance.LineHeight), M.SetText("Bold text"), M.SetFontStyle(TMPro.FontStyles.Bold));



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

                        IconButtonFontAwesome("\uf2ed", 12, () => { Items.RemoveAt(index); MakeDirty(); }, P.Right(buttonSize));                        
                        GapRight(10);

                        if (i == (Items.Count - 1))
                            GapRight(buttonSize);
                        else
                            IconButtonFontAwesome("\uf063", 12, () => { Swap(index); MakeDirty(); }, P.Right(buttonSize));//Down

                        if (i == 0)
                            GapRight(buttonSize);
                        else
                            IconButtonFontAwesome("\uf062", 12, () => { Swap(index-1); MakeDirty(); }, P.Right(buttonSize));//Up
                        

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
