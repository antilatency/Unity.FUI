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


    public static partial class M{

        private static Modifier MakeSetter(bool mutable, Modifier.Procedure action,
            [System.Runtime.CompilerServices.CallerMemberName] string id = ""){
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
        
        public static Modifier SetRectangleCorners(float topLeft, float topRight, float bottomLeft, float bottomRight, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<RoundedRectangle>().SetCorners(topLeft, topRight, bottomLeft, bottomRight); });

        public static Modifier SetText(string text, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().text = text; });




        public static Modifier SetFontStyle(FontStyles fontStyle, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().fontStyle = fontStyle; });

        public static Modifier SetFontSize(float fontSize, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().fontSize = fontSize; });

        public static Modifier SetWordWrapping(bool wrapping, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().enableWordWrapping = wrapping; });


        public static Modifier SetRichTextEnabled(bool richText, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().richText = richText; });


        public static Modifier SetTextAlignment(
            HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Left,
            VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Middle, bool mutable = true)
            => MakeSetter(mutable, x => {
                x.GetComponent<TMP_Text>().horizontalAlignment = horizontalAlignment;
                x.GetComponent<TMP_Text>().verticalAlignment = verticalAlignment;
            });

        public static Modifier SetTextOverflow(TextOverflowModes overflowMode, bool mutable = true)
            => MakeSetter(mutable, x => { x.GetComponent<TMP_Text>().overflowMode = overflowMode; });

        public static Modifier AddComponent<T>() where T : Component =>
            new(
                $"AddComponent<{typeof(T).FullName}>",
                x => x.gameObject.AddComponent<T>(),
                null
            );

        public static Modifier AddMask(bool showMaskGraphic) =>
            new(
                $"AddMask",
                x => x.AddComponent<Mask>(),
                x => x.GetComponent<Mask>().showMaskGraphic = showMaskGraphic
            );


        public static Modifier AddCircle(float angle = 1, float startAngle = 0, int numSegments = 64) =>
            new(
                $"AddCircle",
                x => x.gameObject.AddComponent<Circle>(),
                x => {
                    var circle = x.GetComponent<Circle>();
                    circle.NumSegments = numSegments;
                    circle.StartAngle = startAngle;
                    circle.Angle = angle;
                }
            );

        public static Modifier AddCircleOutline(float innerThickness = 0, float outerThickness = 0, float angle = 1,
            float startAngle = 0, int numSegments = 64) =>
            new(
                $"AddCircleOutline",
                x => x.gameObject.AddComponent<CircleOutline>(),
                x => {
                    var circle = x.GetComponent<CircleOutline>();
                    circle.NumSegments = numSegments;
                    circle.StartAngle = startAngle;
                    circle.Angle = angle;
                    circle.InnerThickness = innerThickness;
                    circle.OuterThickness = outerThickness;
                }
            );

        public static Modifier AddCubicSpline(
            Vector2 pointA, Vector2 tangentA,
            Vector2 pointB, Vector2 tangentB,
            float innerThickness = 0,
            float outerThickness = 0,
            int numSegments = 64
        ) => new(
            "AddCubicSpline",
            x => x.gameObject.AddComponent<CubicSpline>(),
            x => {
                var spline = x.GetComponent<CubicSpline>();
                spline.PointA = pointA;
                spline.TangentA = tangentA;
                spline.PointB = pointB;
                spline.TangentB = tangentB;
                spline.NumSegments = numSegments;
                spline.InnerThickness = innerThickness;
                spline.OuterThickness = outerThickness;
            }
        );



        public static Modifier AddDraggable(Action<GameObject, PointerEventData> dragAction, InputButtonMask allowedButtons = InputButtonMask.All) =>
            new(
                "AddDraggable",
                x => x.AddComponent<Draggable>(),
                x => {
                    x.GetComponent<Draggable>().DragAction = dragAction;
                    x.GetComponent<Draggable>().AllowedButtons = allowedButtons;
                }
            );

        public static Modifier AddPointerEventObserver(Func<GameObject, PointerEventData, bool> handler) =>
            new(
                "AddPointerEventReceiver",
                x => x.AddComponent<PointerEventObserver>(),
                x => {
                    var receiver = x.GetComponent<PointerEventObserver>();
                    receiver.Handler = handler;
                }
            );



        public static Modifier AddClickHandler(Action click) =>
            new(
                "AddClickHandler",
                x => x.AddComponent<PointerClickHandler>(),
                x => x.GetComponent<PointerClickHandler>().OnClick = click
            );

        public static Modifier AddClickHandlerEx(Action<GameObject, PointerEventData> click) =>
            new(
                "AddClickHandlerEx",
                x => x.AddComponent<PointerClickHandlerEx>(),
                x => x.GetComponent<PointerClickHandlerEx>().OnClick = click
            );

        public static Modifier AddPressReleaseHandler(Action onPress, Action onRelease) =>
            new(
                "AddPressReleaseHandler",
                x => x.AddComponent<PointerPressReleaseHandler>(),
                x => {
                    x.GetComponent<PointerPressReleaseHandler>().OnPress = onPress;
                    x.GetComponent<PointerPressReleaseHandler>().OnRelease = onRelease;
                }
            );
        
        public static Modifier AddPressReleaseHandlerEx(Action<GameObject, PointerEventData> onPress, Action<GameObject, PointerEventData> onRelease) =>
            new(
                "AddPressReleaseHandlerEx",
                x => x.AddComponent<PointerPressReleaseHandlerEx>(),
                x => {
                    x.GetComponent<PointerPressReleaseHandlerEx>().OnPress = onPress;
                    x.GetComponent<PointerPressReleaseHandlerEx>().OnRelease = onRelease;
                }
            );
    

    public static Modifier SetCustomShader(string shaderName, params (string name, object value)[] parameters) =>
            new (
                "SetCustomShader",
                x => x.AddComponent<CustomShader>(),
                x => {
                    var component = x.GetComponent<CustomShader>();
                    component.ShaderName = shaderName;
                    component.SetParameters(parameters);
                }
            );


        public static Modifier AddPressedHoveredHighlighter(Color color, Color hoveredColor, Color pressedColor) =>
            new (
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

        /*public static Modifier SetFormToNotify(bool extraIteration = false) =>
            new (
                "SetFormToNotify",
                null,
                x => {
                    var notifier = x.GetComponent<AbstractFormNotifier>();
                    notifier.SetFormToNotify(Form.Current,extraIteration);
                }
                );*/

        public static Modifier DisableRaycastTarget() =>
            new (
                "DisableRaycastTarget",
                x => x.GetComponent<Graphic>().raycastTarget = false,
                null
                );

    }



}