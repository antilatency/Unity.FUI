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
            new Item("Lemon","\uf094", new Color(1f,1f,0.3f)),
            new Item("Carrot","\uf787", new Color(1f,0.6f,0.2f)),
            new Item("Pepper","\uf816", new Color(1f,0.0f,0.1f)),
        };



        using (GroupBackground(Fill)) {

            using (GroupBackground(PushLeft(50),Theme.Instance.PanelBackgroundColor)) {

                for (int i = 0; i < palette.Length; i++) {
                    var item = palette[i];
                    IconButtonFontAwesome(item.Icon, 16, item.Color, () => { Items.Add(item); }, PushUp(50));
                }                
            }

            void Swap(int i) {
                var temp = Items[i];
                Items[i] = Items[i+1];
                Items[i + 1] = temp;
            }

            const int buttonSize = 24;
            using (ScrollRectVertical(Fill)) {
                Padding(4);

                for (int i = 0; i < Items.Count; i++) {
                    int index = i;
                    var item = Items[i];
                    using (GroupBackground(PushUp(24), Theme.Instance.PanelBackgroundColor, 4)) {
                        GapLeft(4);
                        IconFontAwesome(item.Icon, 16, item.Color, PushLeft(20));
                        GapLeft(10);

                        IconButtonFontAwesome("\uf2ed", 12, () => { Items.RemoveAt(index); }, PushRight(buttonSize));                        
                        GapRight(10);

                        if (i == (Items.Count - 1))
                            GapRight(buttonSize);
                        else
                            IconButtonFontAwesome("\uf063", 12, () => { Swap(index); }, PushRight(buttonSize));//Down

                        if (i == 0)
                            GapRight(buttonSize);
                        else
                            IconButtonFontAwesome("\uf062", 12, () => { Swap(index-1); }, PushRight(buttonSize));//Up
                        

                        Label(item.Name, Fill);
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
