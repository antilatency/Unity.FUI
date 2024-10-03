using FUI.Gears;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FUI {
#nullable enable

    public struct Modifier {

        public delegate void Procedure(GameObject value);

        public string Id;
        public Procedure? Creator;
        public Procedure? Updater;

        public Modifier(string id, Procedure? creator, Procedure? updater) {
            Id = id;
            Creator = creator;
            Updater = updater;
        }
    }


    public static partial class M {

        private static Modifier MakeSetter(bool mutable, Modifier.Procedure action, [System.Runtime.CompilerServices.CallerMemberName] string id = "") {
            return new Modifier(
                id,
                mutable ? null : action,
                mutable ? action : null
                );
        }



        public static Modifier SetColor(Color color, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<Graphic>().color = color; });

        public static Modifier SetRectangleCorners(float radius = 4, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<RoundedRectangle>().SetAllCorners(radius); });

        public static Modifier SetText(string text, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().text = text; });

        public static Modifier SetFontStyle(FontStyles fontStyle, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().fontStyle = fontStyle; });

        public static Modifier SetFontSize(float fontSize, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().fontSize = fontSize; });


        public static Modifier SetTextAlignment(HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Left, VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Capline, bool mutable = true)
            => MakeSetter(mutable, x => {
                x.GetComponent<TMP_Text>().horizontalAlignment = horizontalAlignment;
                x.GetComponent<TMP_Text>().verticalAlignment = verticalAlignment;
            });
        public static Modifier SetTextOverflow(TextOverflowModes overflowMode, bool mutable = true)
            => MakeSetter(mutable, x => {
                x.GetComponent<TMP_Text>().overflowMode = overflowMode;
            });

        public static Modifier AddComponent<T>() where T : Component =>
            new Modifier(
                $"AddComponent<{typeof(T).FullName}>",
                x => x.gameObject.AddComponent<T>(),
                null
                );

        public static Modifier AddMask(bool showMaskGraphic) =>
            new Modifier(
                $"AddMask",
                x => x.AddComponent<Mask>(),
                x => x.GetComponent<Mask>().showMaskGraphic = showMaskGraphic
                );


        public static Modifier AddCircle(float angle = 1, float startAngle = 0, int numSegments = 64) =>
            new Modifier(
                $"AddCircle",
                x => x.gameObject.AddComponent<Circle>(),
                x=> {
                    var circle = x.GetComponent<Circle>();
                    circle.NumSegments = numSegments;
                    circle.StartAngle = startAngle;
                    circle.Angle = angle;
                }
                );


        public static Modifier AddDraggable(Action<GameObject, PointerEventData> dragAction) =>
            new Modifier(
                "AddDraggable",
                x => x.AddComponent<Draggable>(),
                x => x.GetComponent<Draggable>().DragAction = dragAction
                );
        public static Modifier AddClickHandler(Action click) =>
            new Modifier(
                "AddClickHandler",
                x => x.AddComponent<PointerClickHandler>(),
                x => x.GetComponent<PointerClickHandler>().OnClick = click
                );

        public static Modifier AddClickHandlerEx(Action<GameObject, PointerEventData> click) =>
            new Modifier(
                "AddClickHandlerEx",
                x => x.AddComponent<PointerClickHandlerEx>(),
                x => x.GetComponent<PointerClickHandlerEx>().OnClick = click
                );

        public static Modifier SetCustomShader(string shaderName, params (string name, object value)[] parameters) =>
            new Modifier(
                "SetCustomShader",
                x => x.AddComponent<CustomShader>(),
                x => {
                    var component = x.GetComponent<CustomShader>();
                    component.ShaderName = shaderName;
                    component.SetParameters(parameters);
                }
                );


        public static Modifier AddPressedHoveredHighlighter(Color color, Color hoveredColor, Color pressedColor) =>
            new Modifier(
                "AddPressedHoveredHighlighter",
                x => {
                    x.AddComponent<ConfigurablePressedHoveredHighlighter>();
                },
                x => {
                    var highlighter = x.GetComponent<ConfigurablePressedHoveredHighlighter>();
                    highlighter.initialColor = color;
                    highlighter.pressedColor = pressedColor;
                    highlighter.hoveredColor = hoveredColor;
                }
            );

        public static Modifier SetFormToNotify(int NumExtraIterations) =>
            new Modifier(
                "SetFormToNotify",
                null,
                x => {
                    var notifier = x.GetComponent<AbstractUserInputNotifier>();
                    notifier.SetFormToNotify(Form.Current);
                    notifier.ExtraIterations = NumExtraIterations;
                }
                );

        public static Modifier DisableRaycastTarget() =>
            new Modifier(
                "DisableRaycastTarget",
                x => x.GetComponent<Graphic>().raycastTarget = false,
                null
                );

    }



}