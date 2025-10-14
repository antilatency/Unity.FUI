using FUI.Gears;
using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FUI {
#nullable enable

    namespace Modifiers {

        public class ModifiersList : IEnumerable<Modifier> {
            private List<Modifier> _items;
            public ModifiersList(params Modifier[] items) {
                _items = new List<Modifier>(items);
            }
            public void Add(Modifier item) {
                _items.Add(item);
            }
            public void Add(IEnumerable<Modifier>? items) {
                if (items != null) {
                    _items.AddRange(items);
                }
            }
            public void Add(ModifiersList? items) {
                if (items != null) {
                    _items.AddRange(items);
                }
            }
            public T Get<T>() where T : Modifier {
                foreach (var item in _items) {
                    if (item is T t) {
                        return t;
                    }
                }
                throw new Exception($"Modifier of type {typeof(T).FullName} not found");
            }

            public IEnumerator<Modifier> GetEnumerator() {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }


        public abstract class Modifier {
            public virtual string Id => GetType().FullName;
            public abstract void Create(GameObject gameObject);
            public abstract void Update(GameObject gameObject);
        }

        public abstract class SetterModifier : Modifier {
            public abstract void Set(GameObject gameObject);
            public override void Create(GameObject gameObject) {
                Set(gameObject);
            }
            public override void Update(GameObject gameObject) {
                Set(gameObject);
            }
        }

        public abstract class SetterModifier<T> : Modifier  where T : Component {
            public abstract void Set(T component);
            public override void Create(GameObject gameObject) {
                Set(gameObject.GetComponent<T>());
            }
            public override void Update(GameObject gameObject) {
                Set(gameObject.GetComponent<T>());
            }
        }

        public abstract class ImmutableSetterModifier : Modifier {
            public abstract void Set(GameObject gameObject);
            public override void Create(GameObject gameObject) {
                Set(gameObject);
            }
            public override void Update(GameObject gameObject) { }
        }
        public abstract class ImmutableSetterModifier<T> : Modifier where T : Component {
            public abstract void Set(T component);
            public override void Create(GameObject gameObject) {
                Set(gameObject.GetComponent<T>());
            }
            public override void Update(GameObject gameObject) {}
        }


        public class SetColor : SetterModifier<Graphic> {
            public Color Color;
            public SetColor(Color color) {
                Color = color;
            }
            public override void Set(Graphic component) => component.color = Color;

        }

        public class SetRectangleCorners : SetterModifier<RoundedRectangle> {
            public float TopLeft;
            public float TopRight;
            public float BottomLeft;
            public float BottomRight;
            public SetRectangleCorners(float radius) {
                TopLeft = radius;
                TopRight = radius;
                BottomLeft = radius;
                BottomRight = radius;
            }
            public SetRectangleCorners(float topLeft, float topRight, float bottomLeft, float bottomRight) {
                TopLeft = topLeft;
                TopRight = topRight;
                BottomLeft = bottomLeft;
                BottomRight = bottomRight;
            }
            public override void Set(RoundedRectangle component) => component.SetCorners(TopLeft, TopRight, BottomLeft, BottomRight);
        }

        public class SetText : SetterModifier<TMP_Text> {
            public string Text;
            public SetText(string text) {
                Text = text;
            }
            public override void Set(TMP_Text component) => component.text = Text;
        }

        public class SetFontStyle : SetterModifier<TMP_Text> {
            public FontStyles FontStyle;
            public SetFontStyle(FontStyles fontStyle) {
                FontStyle = fontStyle;
            }
            public override void Set(TMP_Text component) => component.fontStyle = FontStyle;
        }

        public class SetFontSize : SetterModifier<TMP_Text> {
            public float FontSize;
            public SetFontSize(float fontSize) {
                FontSize = fontSize;
            }
            public override void Set(TMP_Text component) => component.fontSize = FontSize;
        }

        public class SetWordWrapping : SetterModifier<TMP_Text> {
            public bool Wrapping;
            public SetWordWrapping(bool wrapping) {
                Wrapping = wrapping;
            }
            public override void Set(TMP_Text component) => component.enableWordWrapping = Wrapping;
        }

        public class SetRichTextEnabled : SetterModifier<TMP_Text> {
            public bool RichText;
            public SetRichTextEnabled(bool richText) {
                RichText = richText;
            }
            public override void Set(TMP_Text component) => component.richText = RichText;
        }

        public class SetTextAlignment : SetterModifier<TMP_Text> {
            public HorizontalAlignmentOptions HorizontalAlignment;
            public VerticalAlignmentOptions VerticalAlignment;
            public SetTextAlignment(HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Left, VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Middle) {
                HorizontalAlignment = horizontalAlignment;
                VerticalAlignment = verticalAlignment;
            }
            public override void Set(TMP_Text component) {
                component.horizontalAlignment = HorizontalAlignment;
                component.verticalAlignment = VerticalAlignment;
            }
        }

        public class SetTextAlignmentCenterMiddle : ImmutableSetterModifier<TMP_Text> {
            public override void Set(TMP_Text component) {
                component.horizontalAlignment = HorizontalAlignmentOptions.Center;
                component.verticalAlignment = VerticalAlignmentOptions.Middle;
            }
        }
        public class SetTextAlignmentLeftMiddle : ImmutableSetterModifier<TMP_Text> {
            public override void Set(TMP_Text component) {
                component.horizontalAlignment = HorizontalAlignmentOptions.Left;
                component.verticalAlignment = VerticalAlignmentOptions.Middle;
            }
        }
        public class SetTextAlignmentRightMiddle : ImmutableSetterModifier<TMP_Text> {
            public override void Set(TMP_Text component) {
                component.horizontalAlignment = HorizontalAlignmentOptions.Right;
                component.verticalAlignment = VerticalAlignmentOptions.Middle;
            }
        }

        public class SetTextOverflow : SetterModifier<TMP_Text> {
            public TextOverflowModes OverflowMode;
            public SetTextOverflow(TextOverflowModes overflowMode) {
                OverflowMode = overflowMode;
            }
            public override void Set(TMP_Text component) => component.overflowMode = OverflowMode;
        }

        public class SetTextOverflowEllipsis : ImmutableSetterModifier<TMP_Text> {
            public override void Set(TMP_Text component) => component.overflowMode = TextOverflowModes.Ellipsis;
        }

        public class SetTextOverflowOverflow : ImmutableSetterModifier<TMP_Text> {
            public override void Set(TMP_Text component) => component.overflowMode = TextOverflowModes.Overflow;
        }

        public class SetTextMargin : SetterModifier<TMP_Text> {
            public float Left;
            public float Right;
            public float Top;
            public float Bottom;
            public SetTextMargin(float left = 0, float right = 0, float top = 0, float bottom = 0) {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }
            public override void Set(TMP_Text component) {
                component.margin = new Vector4(Left, Top, Right, Bottom);
            }
        }

        public class AddComponent<T> : Modifier where T : Component {
            public override string Id => $"AddComponent<{typeof(T).FullName}>";
            public override void Create(GameObject gameObject) {
                var component = gameObject.AddComponent<T>();
                if (component == null) {
                    throw new Exception($"Failed to add component of type {typeof(T).FullName}");
                }
            }
            public override void Update(GameObject gameObject) { }
        }

        public abstract class AddComponentConfigured<T> : Modifier where T : Component {
            public override void Create(GameObject gameObject) {
                var component = gameObject.AddComponent<T>();
                Configure(component);
            }
            public override void Update(GameObject gameObject) {
                var component = gameObject.GetComponent<T>();
                Configure(component);
            }
            public abstract void Configure(T component);
        }

        public class AddMask : AddComponentConfigured<Mask> {
            public bool ShowMaskGraphic;
            public AddMask(bool showMaskGraphic) {
                ShowMaskGraphic = showMaskGraphic;
            }
            public override void Configure(Mask component) {
                component.showMaskGraphic = ShowMaskGraphic;
            }
        }

        public class AddRectMask : AddComponent<RectMask2D> {
        }

        public class AddCircle : AddComponentConfigured<Circle> {
            public float Angle;
            public float StartAngle;
            public int NumSegments;
            public AddCircle(float angle = 1, float startAngle = 0, int numSegments = 64) {
                Angle = angle;
                StartAngle = startAngle;
                NumSegments = numSegments;
            }
            public override void Configure(Circle component) {
                component.NumSegments = NumSegments;
                component.StartAngle = StartAngle;
                component.Angle = Angle;
            }
        }

        public class AddCircleOutline : AddComponentConfigured<CircleOutline> {
            public float InnerThickness;
            public float OuterThickness;
            public float Angle;
            public float StartAngle;
            public int NumSegments;
            public AddCircleOutline(float innerThickness = 0, float outerThickness = 0, float angle = 1, float startAngle = 0, int numSegments = 64) {
                InnerThickness = innerThickness;
                OuterThickness = outerThickness;
                Angle = angle;
                StartAngle = startAngle;
                NumSegments = numSegments;
            }
            public override void Configure(CircleOutline component) {
                component.NumSegments = NumSegments;
                component.StartAngle = StartAngle;
                component.Angle = Angle;
                component.InnerThickness = InnerThickness;
                component.OuterThickness = OuterThickness;
            }
        }

        public class AddCubicSpline : AddComponentConfigured<CubicSpline> {
            public Vector2 PointA;
            public Vector2 TangentA;
            public Vector2 PointB;
            public Vector2 TangentB;
            public float InnerThickness;
            public float OuterThickness;
            public int NumSegments;
            public AddCubicSpline(
                Vector2 pointA, Vector2 tangentA,
                Vector2 pointB, Vector2 tangentB,
                float innerThickness = 0,
                float outerThickness = 0,
                int numSegments = 64
            ) {
                PointA = pointA;
                TangentA = tangentA;
                PointB = pointB;
                TangentB = tangentB;
                InnerThickness = innerThickness;
                OuterThickness = outerThickness;
                NumSegments = numSegments;
            }
            public override void Configure(CubicSpline component) {
                component.PointA = PointA;
                component.TangentA = TangentA;
                component.PointB = PointB;
                component.TangentB = TangentB;
                component.NumSegments = NumSegments;
                component.InnerThickness = InnerThickness;
                component.OuterThickness = OuterThickness;
            }
        }

        public class AddDraggable : AddComponentConfigured<Draggable> {
            public Action<GameObject, PointerEventData> DragAction;
            public PointerEventUtils.PointerEventFilter? EventFilter;
            public AddDraggable(Action<GameObject, PointerEventData> dragAction, PointerEventUtils.PointerEventFilter? eventFilter = null) {
                DragAction = dragAction;
                EventFilter = eventFilter;
            }
            public override void Configure(Draggable component) {
                component.DragAction = DragAction;
                component.EventFilter = EventFilter;
            }
        }

        public class AddPointerEventObserver : AddComponentConfigured<PointerEventObserver> {
            public Func<GameObject, PointerEventData, bool> Handler;
            public AddPointerEventObserver(Func<GameObject, PointerEventData, bool> handler) {
                Handler = handler;
            }
            public override void Configure(PointerEventObserver component) {
                component.Handler = Handler;
            }
        }

        public class AddClickHandler : AddComponentConfigured<PointerClickHandler> {
            public Action OnClick;
            public AddClickHandler(Action onClick) {
                OnClick = onClick;
            }
            public override void Configure(PointerClickHandler component) {
                component.OnClick = OnClick;
            }
        }

        public class AddClickHandlerEx : AddComponentConfigured<PointerClickHandlerEx> {
            public ButtonAction OnClick;
            public AddClickHandlerEx(ButtonAction onClick) {
                OnClick = onClick;
            }
            public override void Configure(PointerClickHandlerEx component) {
                component.OnClick = OnClick;
            }
        }

        public class AddPressReleaseHandler : AddComponentConfigured<PointerPressReleaseHandler> {
            public Action OnPress;
            public Action OnRelease;
            public AddPressReleaseHandler(Action onPress, Action onRelease) {
                OnPress = onPress;
                OnRelease = onRelease;
            }
            public override void Configure(PointerPressReleaseHandler component) {
                component.OnPress = OnPress;
                component.OnRelease = OnRelease;
            }
        }

        public class AddPressReleaseHandlerEx : AddComponentConfigured<PointerPressReleaseHandlerEx> {
            public Action<GameObject, PointerEventData> OnPress;
            public Action<GameObject, PointerEventData> OnRelease;
            public AddPressReleaseHandlerEx(Action<GameObject, PointerEventData> onPress, Action<GameObject, PointerEventData> onRelease) {
                OnPress = onPress;
                OnRelease = onRelease;
            }
            public override void Configure(PointerPressReleaseHandlerEx component) {
                component.OnPress = OnPress;
                component.OnRelease = OnRelease;
            }
        }

        public class SetCustomShader : AddComponentConfigured<CustomShader> {
            public string ShaderName;
            public (string name, object value)[] Parameters;
            public SetCustomShader(string shaderName, params (string name, object value)[] parameters) {
                ShaderName = shaderName;
                Parameters = parameters;
            }
            public override void Configure(CustomShader component) {
                component.ShaderName = ShaderName;
                component.SetParameters(Parameters);
            }
        }

        public class AddPressedHoveredHighlighter : AddComponentConfigured<PressedHoveredHighlighter> {
            public Color InitialColor;
            public Color HoveredColor;
            public Color PressedColor;
            public AddPressedHoveredHighlighter(Color initialColor, Color hoveredColor, Color pressedColor) {
                InitialColor = initialColor;
                HoveredColor = hoveredColor;
                PressedColor = pressedColor;
            }
            public override void Configure(PressedHoveredHighlighter component) {
                component.InitialColor = InitialColor;
                component.HoveredColor = HoveredColor;
                component.PressedColor = PressedColor;
            }
        }


        public class SetRaycastTarget : SetterModifier {
            public bool Value;
            public SetRaycastTarget(bool value) {
                Value = value;
            }
            public override void Set(GameObject gameObject) => gameObject.GetComponent<Graphic>().raycastTarget = Value;
        }


    }

    /*public struct Modifier {

        public delegate void Procedure(GameObject value);

        public string Id;
        public Procedure? Creator;
        public Procedure? Updater;

        public Modifier(string id, Procedure? creator, Procedure? updater) {
            Id = id;
            Creator = creator;
            Updater = updater;
        }
    }*/

/*
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

        public static Modifier SetTextMargin(float left = 0, float right = 0, float top = 0, float bottom = 0, bool mutable = true)
            => MakeSetter(mutable, x => {
                var text = x.GetComponent<TMP_Text>();
                text.margin = new Vector4(left, top, right, bottom);
            });

        public static Modifier AddComponent<T>() where T : Component {
            var result = new Modifier(
                $"AddComponent<{typeof(T).FullName}>",
                x => {
                    var component = x.gameObject.AddComponent<T>();
                    if (component == null) {
                        throw new Exception($"Failed to add component of type {typeof(T).FullName}");
                    }
                    if (component is IFormDependentComponent formDependent) {
                        formDependent.Form = Form.Current;
                    }
                },
                null
            );
            return result;
        }





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



        public static Modifier AddDraggable(Action<GameObject, PointerEventData> dragAction, PointerEventUtils.PointerEventFilter? eventFilter = null) =>
            new(
                "AddDraggable",
                x => x.AddComponent<Draggable>(),
                x => {
                    x.GetComponent<Draggable>().DragAction = dragAction;
                    x.GetComponent<Draggable>().EventFilter = eventFilter;
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


        public static Modifier SetRaycastTarget(bool value) =>
            new (
                "SetRaycastTarget",
                x => x.GetComponent<Graphic>().raycastTarget = value,
                null
                );

    }*/



}