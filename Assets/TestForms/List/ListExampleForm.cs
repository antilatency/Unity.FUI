using UnityEngine;

using System;
using System.Collections.Generic;
using FUI;
using static FUI.Shortcuts;

internal class ListExampleForm : Form {

    [Serializable]
    public class Item {
        public string Name;
        public string Icon;
        public Color Color;
        internal bool Selected;

        public Item(string name, string icon, Color color) {
            Name = name;
            Icon = icon;
            Color = color;
        }
    }

    public List<Item> Items = new();

    public struct ListItemRelocation {
        public int StartIndex;
    }

    protected override void Build() {
        throw new NotImplementedException();
        /*

        Item[] palette = new Item[] {
            new Item("Lemon","\uf094", new Color(1f,1f,0.3f)),
            new Item("Carrot","\uf787", new Color(1f,0.6f,0.2f)),
            new Item("Pepper","\uf816", new Color(1f,0.0f,0.1f)),
        };



        using (WindowBackground()) {

            using (Panel(P.Left(50))) {

                for (int i = 0; i < palette.Length; i++) {
                    var item = palette[i];
                    IconButtonFontAwesome(item.Icon, 16, item.Color, () => {
                        Items.Add(item);
                        MakeDirty();
                    }, P.Up(50));
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
                    using (Panel(P.Up(24))) {
                        GapLeft(4);
                        IconFontAwesome(item.Icon, 16, item.Color, P.Left(20));
                        GapLeft(10);

                        using (TempPaddingVertical(2, 2)) {
                            item.Selected = Checkbox(item.Selected, P.Left(20));
                        }

                        GapLeft(10);

                        IconButtonFontAwesome("\uf2ed", 12, () => { Items.RemoveAt(index); MakeDirty(); }, P.Right(buttonSize));//Delete
                        GapRight(10);

                        if (i == (Items.Count - 1))
                            GapRight(buttonSize);
                        else
                            IconButtonFontAwesome("\uf063", 12, () => { Swap(index); MakeDirty(); }, P.Right(buttonSize));//Down

                        if (i == 0)
                            GapRight(buttonSize);
                        else
                            IconButtonFontAwesome("\uf062", 12, () => { Swap(index - 1); MakeDirty(); }, P.Right(buttonSize));//Up
                        Label(item.Name, P.Fill);
                    }
                    GapTop(4);
                }
            }

        }*/

    }
}
