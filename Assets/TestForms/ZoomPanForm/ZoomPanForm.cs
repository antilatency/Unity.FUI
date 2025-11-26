using FUI;
using FUI.Gears;
using FUI.Modifiers;

using UnityEngine;
using static FUI.Basic;
using static FUI.Shortcuts;
using static UnityEngine.EventSystems.PointerEventData;


public class ZoomPanForm : Form {

    public enum InputButtonMask {
        Left = 1 << InputButton.Left,
        Right = 1 << InputButton.Right,
        Middle = 1 << InputButton.Middle,
        All = Left | Right | Middle
    }

    public float TestFloat;
    
    public InputButtonMask AllowedButtons = InputButtonMask.All;

    protected override void Build() {

        using (WindowBackground()) {

            using (Panel(P.Left(150))) {
                Padding(4);
                Label("Allowed Buttons");
                GapTop(4);
                Dropdown(AllowedButtons, x => AssignAndMakeDirty(ref AllowedButtons, x));
            }

            using (ZoomPanViewport(P.Fill, new Vector2(640, 480), 
                scrollFilter: (g, e) => {
                    var anyControl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    return !anyControl;
                },
                dragFilter: (g, e) => {
                    var allowed = ((int)AllowedButtons & (1 << (int)e.button)) != 0;
                    return allowed;
                }
            )) {
                Rectangle(P.Absolute(Vector2.zero, 50, 50, Vector2.zero, Vector2.one - Vector2.zero), Color.red);
                Rectangle(P.Absolute(Vector2.zero, 50, 50, Vector2.up, Vector2.one - Vector2.up), Color.green);
                Rectangle(P.Absolute(Vector2.zero, 50, 50, Vector2.one, Vector2.one - Vector2.one), Color.blue);
                Rectangle(P.Absolute(Vector2.zero, 50, 50, Vector2.right, Vector2.one - Vector2.right), Color.white);

                using (Group(P.Absolute(Vector2.zero, 100, 100, Vector2.zero))) {
                    Element(P.Fill, null
                    , new AddComponent<RoundedRectangle>()
                    , new SetColor(Color.yellow)
                    , new AddPointerEventObserver((g, e) => {
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

                SubForm<ListExampleForm>().ApplyPositioner(P.Absolute(Vector2.zero, 300, 200, Vector2.right));

                CircleOutline(
                    P.Absolute(new Vector2(-30, 50), 50, 50)
                    , Color.white, innerThickness: 1, numSegments: 32);
                CircleOutlineScreenSpaceThickness(
                    P.Absolute(new Vector2(30, 50), 50, 50)
                    , Color.red, outerThickness: 0, screenSpaceThickness: 20, numSegments: 64); ;



                LabeledInputField("input", TestFloat, x => AssignAndMakeDirty(ref TestFloat, x));
                Button("Button", () => { });

                //Label($"subForm Num Items = {subForm.Items?.Count ?? 0}");
            }

        }



    }
}
