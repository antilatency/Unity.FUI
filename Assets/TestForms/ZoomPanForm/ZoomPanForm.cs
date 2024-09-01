using FUI;
using FUI.Gears;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FUI.Shortcuts;


public class ZoomPanForm : Form {

    

    public float TestFloat;

    protected override void Build() {

        using (WindowBackground()) {

            using (Panel(P.Left(50))) { 
            
            }

            using (ZoomPanViewport(P.Fill, new Vector2(640, 480))) {
                Rectangle(P.Absolute(Vector2.zero, 50, null, Vector2.zero, Vector2.one - Vector2.zero), Color.red);
                Rectangle(P.Absolute(Vector2.zero, 50, null, Vector2.up, Vector2.one - Vector2.up), Color.green);
                Rectangle(P.Absolute(Vector2.zero, 50, null, Vector2.one, Vector2.one - Vector2.one), Color.blue);
                Rectangle(P.Absolute(Vector2.zero, 50, null, Vector2.right, Vector2.one - Vector2.right), Color.white);

                using (Group(P.Absolute(Vector2.zero, 100, null, Vector2.right))) {
                    Label("Text");
                    Label("Text1");
                    Label("Text2");
                    Label("Text3");
                }

                TestFloat = LabeledInputField("input", TestFloat);
                Button("Button", () => { });
            }

        }  
        


    }
}
