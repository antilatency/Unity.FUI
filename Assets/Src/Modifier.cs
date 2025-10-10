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
        public abstract class ImmutableSetterModifier : Modifier {
            public abstract void Set(GameObject gameObject);
            public override void Create(GameObject gameObject) {
                Set(gameObject);
            }
            public override void Update(GameObject gameObject) {}
        }


        public class SetColor : SetterModifier {
            public Color Color;
            public SetColor(Color color) {
                Color = color;
            }
            public override void Set(GameObject gameObject) => gameObject.GetComponent<Graphic>().color = Color;

        }

        public class SetRectangleCorners : SetterModifier {
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
            public override void Set(GameObject gameObject) => gameObject.GetComponent<RoundedRectangle>().SetCorners(TopLeft, TopRight, BottomLeft, BottomRight);
        }

        public class SetText : SetterModifier {
            public string Text;
            public SetText(string text) {
                Text = text;
            }
            public override void Set(GameObject gameObject) => gameObject.GetComponent<TMP_Text>().text = Text;
        }

        public class SetFontStyle : SetterModifier {
            public FontStyles FontStyle;
            public SetFontStyle(FontStyles fontStyle) {
                FontStyle = fontStyle;
            }
            public override void Set(GameObject gameObject) => gameObject.GetComponent<TMP_Text>().fontStyle = FontStyle;
        }

        public class SetFontSize : SetterModifier {
            public float FontSize;
            public SetFontSize(float fontSize) {
                FontSize = fontSize;
            }
            public override void Set(GameObject gameObject) => gameObject.GetComponent<TMP_Text>().fontSize = FontSize;
        }

        public class SetWordWrapping : SetterModifier {
            public bool Wrapping;
            public SetWordWrapping(bool wrapping) {
                Wrapping = wrapping;
            }
            public override void Set(GameObject gameObject) => gameObject.GetComponent<TMP_Text>().enableWordWrapping = Wrapping;
        }

        public class SetRichTextEnabled : SetterModifier {
            public bool RichText;
            public SetRichTextEnabled(bool richText) {
                RichText = richText;
            }
            public override void Set(GameObject gameObject) => gameObject.GetComponent<TMP_Text>().richText = RichText;
        }

        public class SetTextAlignment : SetterModifier {
            public HorizontalAlignmentOptions HorizontalAlignment;
            public VerticalAlignmentOptions VerticalAlignment;
            public SetTextAlignment(HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Left, VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Middle) {
                HorizontalAlignment = horizontalAlignment;
                VerticalAlignment = verticalAlignment;
            }
            public override void Set(GameObject gameObject) {
                var text = gameObject.GetComponent<TMP_Text>();
                text.horizontalAlignment = HorizontalAlignment;
                text.verticalAlignment = VerticalAlignment;
            }
        }

        public class SetTextAlignmentCenter : ImmutableSetterModifier {
            public override void Set(GameObject gameObject) {
                var text = gameObject.GetComponent<TMP_Text>();
                text.horizontalAlignment = HorizontalAlignmentOptions.Center;
                text.verticalAlignment = VerticalAlignmentOptions.Middle;
            }
        }

        public class SetTextOverflow : SetterModifier {
            public TextOverflowModes OverflowMode;
            public SetTextOverflow(TextOverflowModes overflowMode) {
                OverflowMode = overflowMode;
            }
            public override void Set(GameObject gameObject) => gameObject.GetComponent<TMP_Text>().overflowMode = OverflowMode;
        }

        public class SetTextOverflowEllipsis : ImmutableSetterModifier {
            public override void Set(GameObject gameObject) => gameObject.GetComponent<TMP_Text>().overflowMode = TextOverflowModes.Ellipsis;
        }

        public class SetTextMargin : SetterModifier {
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
            public override void Set(GameObject gameObject) {
                var text = gameObject.GetComponent<TMP_Text>();
                text.margin = new Vector4(Left, Top, Right, Bottom);
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

        public class AddMask : Modifier {
            public bool ShowMaskGraphic;
            public AddMask(bool showMaskGraphic) {
                ShowMaskGraphic = showMaskGraphic;
            }
            public override void Create(GameObject gameObject) {
                var mask = gameObject.AddComponent<Mask>();
                mask.showMaskGraphic = ShowMaskGraphic;
            }
            public override void Update(GameObject gameObject) {
                var mask = gameObject.GetComponent<Mask>();
                mask.showMaskGraphic = ShowMaskGraphic;
            }
        }

        public class AddCircle : Modifier {
            public float Angle;
            public float StartAngle;
            public int NumSegments;
            public AddCircle(float angle = 1, float startAngle = 0, int numSegments = 64) {
                Angle = angle;
                StartAngle = startAngle;
                NumSegments = numSegments;
            }
            public override void Create(GameObject gameObject) {
                var circle = gameObject.AddComponent<Circle>();
                circle.NumSegments = NumSegments;
                circle.StartAngle = StartAngle;
                circle.Angle = Angle;
            }
            public override void Update(GameObject gameObject) {
                var circle = gameObject.GetComponent<Circle>();
                circle.NumSegments = NumSegments;
                circle.StartAngle = StartAngle;
                circle.Angle = Angle;
            }
        }

        public class AddCircleOutline : Modifier {
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
            public override void Create(GameObject gameObject) {
                var circle = gameObject.AddComponent<CircleOutline>();
                circle.NumSegments = NumSegments;
                circle.StartAngle = StartAngle;
                circle.Angle = Angle;
                circle.InnerThickness = InnerThickness;
                circle.OuterThickness = OuterThickness;
            }
            public override void Update(GameObject gameObject) {
                var circle = gameObject.GetComponent<CircleOutline>();
                circle.NumSegments = NumSegments;
                circle.StartAngle = StartAngle;
                circle.Angle = Angle;
                circle.InnerThickness = InnerThickness;
                circle.OuterThickness = OuterThickness;
            }
        }

        public class AddCubicSpline : Modifier {
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
            public override void Create(GameObject gameObject) {
                var spline = gameObject.AddComponent<CubicSpline>();
                spline.PointA = PointA;
                spline.TangentA = TangentA;
                spline.PointB = PointB;
                spline.TangentB = TangentB;
                spline.NumSegments = NumSegments;
                spline.InnerThickness = InnerThickness;
                spline.OuterThickness = OuterThickness;
            }
            public override void Update(GameObject gameObject) {
                var spline = gameObject.GetComponent<CubicSpline>();
                spline.PointA = PointA;
                spline.TangentA = TangentA;
                spline.PointB = PointB;
                spline.TangentB = TangentB;
                spline.NumSegments = NumSegments;
                spline.InnerThickness = InnerThickness;
                spline.OuterThickness = OuterThickness;
            }
        }

        public class AddDraggable : Modifier {
            public Action<GameObject, PointerEventData> DragAction;
            public PointerEventUtils.PointerEventFilter? EventFilter;
            public AddDraggable(Action<GameObject, PointerEventData> dragAction, PointerEventUtils.PointerEventFilter? eventFilter = null) {
                DragAction = dragAction;
                EventFilter = eventFilter;
            }
            public override void Create(GameObject gameObject) {
                var draggable = gameObject.AddComponent<Draggable>();
                draggable.DragAction = DragAction;
                draggable.EventFilter = EventFilter;
            }
            public override void Update(GameObject gameObject) {
                var draggable = gameObject.GetComponent<Draggable>();
                draggable.DragAction = DragAction;
                draggable.EventFilter = EventFilter;
            }
        }

        public class AddPointerEventObserver : Modifier {
            public Func<GameObject, PointerEventData, bool> Handler;
            public AddPointerEventObserver(Func<GameObject, PointerEventData, bool> handler) {
                Handler = handler;
            }
            public override void Create(GameObject gameObject) {
                var receiver = gameObject.AddComponent<PointerEventObserver>();
                receiver.Handler = Handler;
            }
            public override void Update(GameObject gameObject) {
                var receiver = gameObject.GetComponent<PointerEventObserver>();
                receiver.Handler = Handler;
            }
        }

        public class AddClickHandler : Modifier {
            public Action OnClick;
            public AddClickHandler(Action onClick) {
                OnClick = onClick;
            }
            public override void Create(GameObject gameObject) {
                var handler = gameObject.AddComponent<PointerClickHandler>();
                handler.OnClick = OnClick;
            }
            public override void Update(GameObject gameObject) {
                var handler = gameObject.GetComponent<PointerClickHandler>();
                handler.OnClick = OnClick;
            }
        }

        public class AddClickHandlerEx : Modifier {
            public ButtonAction OnClick;
            public AddClickHandlerEx(ButtonAction onClick) {
                OnClick = onClick;
            }
            public override void Create(GameObject gameObject) {
                var handler = gameObject.AddComponent<PointerClickHandlerEx>();
                handler.OnClick = OnClick;
            }
            public override void Update(GameObject gameObject) {
                var handler = gameObject.GetComponent<PointerClickHandlerEx>();
                handler.OnClick = OnClick;
            }
        }

        public class AddPressReleaseHandler : Modifier {
            public Action OnPress;
            public Action OnRelease;
            public AddPressReleaseHandler(Action onPress, Action onRelease) {
                OnPress = onPress;
                OnRelease = onRelease;
            }
            public override void Create(GameObject gameObject) {
                var handler = gameObject.AddComponent<PointerPressReleaseHandler>();
                handler.OnPress = OnPress;
                handler.OnRelease = OnRelease;
            }
            public override void Update(GameObject gameObject) {
                var handler = gameObject.GetComponent<PointerPressReleaseHandler>();
                handler.OnPress = OnPress;
                handler.OnRelease = OnRelease;
            }
        }

        public class AddPressReleaseHandlerEx : Modifier {
            public Action<GameObject, PointerEventData> OnPress;
            public Action<GameObject, PointerEventData> OnRelease;
            public AddPressReleaseHandlerEx(Action<GameObject, PointerEventData> onPress, Action<GameObject, PointerEventData> onRelease) {
                OnPress = onPress;
                OnRelease = onRelease;
            }
            public override void Create(GameObject gameObject) {
                var handler = gameObject.AddComponent<PointerPressReleaseHandlerEx>();
                handler.OnPress = OnPress;
                handler.OnRelease = OnRelease;
            }
            public override void Update(GameObject gameObject) {
                var handler = gameObject.GetComponent<PointerPressReleaseHandlerEx>();
                handler.OnPress = OnPress;
                handler.OnRelease = OnRelease;
            }
        }

        public class SetCustomShader : Modifier {
            public string ShaderName;
            public (string name, object value)[] Parameters;
            public SetCustomShader(string shaderName, params (string name, object value)[] parameters) {
                ShaderName = shaderName;
                Parameters = parameters;
            }
            public override void Create(GameObject gameObject) {
                var component = gameObject.AddComponent<CustomShader>();
                component.ShaderName = ShaderName;
                component.SetParameters(Parameters);
            }
            public override void Update(GameObject gameObject) {
                var component = gameObject.GetComponent<CustomShader>();
                component.ShaderName = ShaderName;
                component.SetParameters(Parameters);
            }
        }

        public class AddPressedHoveredHighlighter : Modifier {
            public Color InitialColor;
            public Color HoveredColor;
            public Color PressedColor;
            public AddPressedHoveredHighlighter(Color initialColor, Color hoveredColor, Color pressedColor) {
                InitialColor = initialColor;
                HoveredColor = hoveredColor;
                PressedColor = pressedColor;
            }
            public override void Create(GameObject gameObject) {
                var highlighter = gameObject.AddComponent<ConfigurablePressedHoveredHighlighter>();
                highlighter.initialColor = InitialColor;
                highlighter.hoveredColor = HoveredColor;
                highlighter.pressedColor = PressedColor;
            }
            public override void Update(GameObject gameObject) {
                var highlighter = gameObject.GetComponent<ConfigurablePressedHoveredHighlighter>();
                highlighter.initialColor = InitialColor;
                highlighter.hoveredColor = HoveredColor;
                highlighter.pressedColor = PressedColor;
            }
        }

        /*public class SetFormToNotify : Modifier {
            public bool ExtraIteration;
            public SetFormToNotify(bool extraIteration = false) {
                ExtraIteration = extraIteration;
            }
            public override void Create(GameObject gameObject) {
                var notifier = gameObject.GetComponent<AbstractFormNotifier>();
                notifier.SetFormToNotify(Form.Current, ExtraIteration);
            }
            public override void Update(GameObject gameObject) {
                var notifier = gameObject.GetComponent<AbstractFormNotifier>();
                notifier.SetFormToNotify(Form.Current, ExtraIteration);
            }
        }*/
        
        public class SetRaycastTarget : Modifier {
            public bool Value;
            public SetRaycastTarget(bool value) {
                Value = value;
            }
            public override void Create(GameObject gameObject) {
                gameObject.GetComponent<Graphic>().raycastTarget = Value;
            }
            public override void Update(GameObject gameObject) {
                gameObject.GetComponent<Graphic>().raycastTarget = Value;
            }
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