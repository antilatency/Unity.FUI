using FUI;
using FUI.Gears;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FUI.Shortcuts;


public class ZoomPanForm : Form {

    

    public float TestFloat;
    
    public InputButtonMask AllowedButtons = InputButtonMask.All;

    protected override void Build() {

        using (WindowBackground()) {

            using (Panel(P.Left(150))) {
                Padding(4);
                Label("Allowed Buttons");
                GapTop(4);
                AllowedButtons = Dropdown(AllowedButtons);
            }

            using (ZoomPanViewport(P.Fill, new Vector2(640, 480), AllowedButtons)) {
                Rectangle(P.Absolute(Vector2.zero, 50, null, Vector2.zero, Vector2.one - Vector2.zero), Color.red);
                Rectangle(P.Absolute(Vector2.zero, 50, null, Vector2.up, Vector2.one - Vector2.up), Color.green);
                Rectangle(P.Absolute(Vector2.zero, 50, null, Vector2.one, Vector2.one - Vector2.one), Color.blue);
                Rectangle(P.Absolute(Vector2.zero, 50, null, Vector2.right, Vector2.one - Vector2.right), Color.white);

                using (Group(P.Absolute(Vector2.zero, 100, 100, Vector2.zero))) {
                    Element(P.Fill, null
                    , M.AddComponent<RoundedRectangle>()
                    , M.SetColor(Color.yellow)
                    , M.AddPointerEventObserver((g, e) => {
                        //check Ctrl key
                        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                            Debug.Log($"Ctrl + Left Click on {g.name}");
                            return false; // Propagate the event
                        }
                        
                        Debug.Log($"UV on {g.name} = {PointerEventObserver.GetPointerUV(g, e)}");
                        if (e.dragging) {
                            Debug.Log($"Dragging on {g.name}");
                            return false;
                        }
                        return true;
                    }));
                }

                var subForm = SubForm<ListExampleForm>(P.Absolute(Vector2.zero, 300, 200, Vector2.right));

                CircleOutline(
                    P.Absolute(new Vector2(-30, 50), 50, 50)
                    , Color.white, innerThickness: 1, numSegments: 32);
                CircleOutlineScreenSpaceThickness(
                    P.Absolute(new Vector2(30, 50), 50, 50)
                    , Color.red, outerThickness: 0, screenSpaceThickness: 20, numSegments: 64); ;



                TestFloat = LabeledInputField("input", TestFloat);
                Button("Button", () => { });

                Label($"subForm Num Items = {subForm.Items?.Count ?? 0}");
            }

        }



    }
}
