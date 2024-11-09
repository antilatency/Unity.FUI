using FUI.Gears;
using System;
using TMPro;
using UnityEngine;

namespace FUI {
#nullable enable

    public static partial class Shortcuts {
        public static void Label(string value, Positioner? positioner = null) {
            Form.Current.LabelModifiable(positioner??Form.DefaultControlPositioner, M.SetText(value));
        }

        private static Disposable Labeled(string label, Positioner? positioner = null) {
            var form = Form.Current;
            Disposable result = form.Group(positioner ?? Form.DefaultControlPositioner);
            Label(label, P.Left(0, 0.5f));
            return result;
        }

        public static T LabeledInputField<T>(string label, T value, Positioner? positioner = null, string toStringFormat = "", Func<string, T>? fromString = null, int numExtraIterations = 0) where T : notnull {
            var form = Form.Current;
            using (Labeled(label, positioner)) {
                return form.InputField(value, P.Fill, toStringFormat, fromString, numExtraIterations);
            }
        }

        public static T LabeledDropdown<T>(string label, T value, Positioner? positioner = null, int numExtraIterations = 0) where T : struct {
            var form = Form.Current;
            using (Labeled(label, positioner)) {
                return form.Dropdown(value, P.Fill, numExtraIterations);
            }
        }
        public static int LabeledDropdown(string label, int value, string[] options, Positioner? positioner = null, int numExtraIterations = 0) {
            var form = Form.Current;
            using (Labeled(label, positioner)) {
                return form.Dropdown(value, options, P.Fill, numExtraIterations);
            }
        }


        public static bool LabeledCheckbox(string label, bool value, Positioner? positioner = null, int numExtraIterations = 0) {
            using (Labeled(label, positioner)) {
                return Checkbox(value, P.Left(), numExtraIterations);
            }
        }

        public static bool Checkbox(bool value, Positioner? positioner = null, int numExtraIterations = 0) {
            var form = Form.Current;

            RectTransform background = form.Element(null
                , M.AddComponent<RoundedRectangle>()
                , M.AddComponent<ButtonHighlighter>()
                , M.AddComponent<BoolUserInputState>()
                , M.SetFormToNotify(numExtraIterations)
                , M.AddComponent<PointerClickHandler>()
                , M.SetRectangleCorners(4)
                , M.AddClickHandlerEx((go,e) => {
                    var input = go.GetComponent<BoolUserInputState>();
                    input.UserInput(!input.Value);
                })
            );


            var userInputState = background.GetComponent<BoolUserInputState>();
            bool result;
            if (!userInputState.NewUserInput) {
                userInputState.Value = value;
                result = value;
            } else {
                result = userInputState.Value;
            }

            form.BeginControls(background);

            if (result) {
                IconFontAwesome("\uf00c", Theme.Instance.LineHeight * 0.8f, P.Fill);
            }

            form.EndControls();

            (positioner??Form.DefaultControlPositioner)(background, form.CurrentBorders, () => new Vector2(Theme.Instance.LineHeight, Theme.Instance.LineHeight));

            return result;
        }


        public static int LabeledInputFieldSpinbox(string label, int value, int dragStepSize = 1, Positioner? positioner = null, string toStringFormat = "", Func<string, int>? fromString = null, int numExtraIterations = 0) {
            var form = Form.Current;
            using (Labeled(label, positioner)) {
                value = Spinbox(value, dragStepSize, P.Right(Theme.Instance.LineHeight));
                form.GapRight(2);
                return form.InputField(value, P.Fill, toStringFormat, fromString, numExtraIterations);
            }
        }

        public static float LabeledInputFieldSpinbox(string label, float value, float dragStepSize, Positioner? positioner = null, string toStringFormat = "", Func<string, float>? fromString = null, int numExtraIterations = 0) {
            var form = Form.Current;
            using (Labeled(label, positioner)) {
                value = Spinbox(value, dragStepSize, P.Right(Theme.Instance.LineHeight));
                form.GapRight(2);
                return form.InputField(value, P.Fill, toStringFormat, fromString, numExtraIterations);
            }
        }

        public static double LabeledInputFieldSpinbox(string label, double value, double dragStepSize, Positioner? positioner = null, string toStringFormat = "", Func<string, double>? fromString = null, int numExtraIterations = 0) {
            var form = Form.Current;
            using (Labeled(label, positioner)) {
                value = Spinbox(value, dragStepSize, P.Right(Theme.Instance.LineHeight));
                form.GapRight(2);
                return form.InputField(value, P.Fill, toStringFormat, fromString, numExtraIterations);
            }
        }

        public static int Spinbox(int value, int dragStepSize = 1, Positioner? positioner = null, int numExtraIterations = 0) {
            return Spinbox<int, IntUserInputState>(value, positioner, numExtraIterations, (v, d) => v / dragStepSize * dragStepSize + dragStepSize * d);
        }
        public static float Spinbox(float value, float dragStepSize, Positioner? positioner = null, int numExtraIterations = 0) {            
            return Spinbox<float, FloatUserInputState>(value, positioner, numExtraIterations, (v, d) => {
                var s = Math.Round(v / dragStepSize + d);
                return (float)(s * dragStepSize);
            } );
        }
        public static double Spinbox(double value, double dragStepSize, Positioner? positioner = null, int numExtraIterations = 0) {
            return Spinbox<double, DoubleUserInputState>(value, positioner, numExtraIterations, (v, d) => Math.Round(v / dragStepSize) * dragStepSize + dragStepSize * d);
        }


        public static T Spinbox<T,S>(T value, Positioner? positioner, int numExtraIterations, Func<T,int,T> deltaToValue) where S: UserInputState<T> {
            var form = Form.Current;

            RectTransform background = form.Element(null
                , M.AddComponent<RoundedRectangle>()
                , M.AddComponent<ButtonHighlighter>()
                , M.AddComponent<S>()
                , M.SetFormToNotify(numExtraIterations)
                , M.AddComponent<PointerClickHandler>()
                , M.SetRectangleCorners(4)

                , M.AddDraggable((go,e) => {
                    var input = go.GetComponent<S>();

                    var m = Input.GetKey(KeyCode.LeftShift) ? 10 : 1;

                    input.UserInput(deltaToValue(input.Value, Mathf.RoundToInt(m * (e.delta.x + e.delta.y))));
                })
            );

            var userInputState = background.GetComponent<S>();
            T result;
            if (!userInputState.NewUserInput) {
                userInputState.Value = value;
                result = value;
            } else {
                result = userInputState.Value;
            }
            form.BeginControls(background);
            IconFontAwesome("\uf424", Theme.Instance.LineHeight * 0.6f, P.Fill);
            form.EndControls();
            (positioner ?? Form.DefaultControlPositioner)(background, form.CurrentBorders, () => new Vector2(Theme.Instance.LineHeight, Theme.Instance.LineHeight));
            return result;
        }



        public static bool ExpandableGroupHeader(string label, Positioner? positioner = null, bool? opened = null) {
            var form = Form.Current;

            var background = form.Element(null
                , M.AddComponent<RoundedRectangle>()
                , M.SetRectangleCorners(4)
                , M.AddComponent<ButtonHighlighter>()
                , M.AddComponent<BoolUserInputState>()
                , M.AddClickHandlerEx((go,e) => {
                    var input = go.GetComponent<BoolUserInputState>();
                    input.UserInput(!input.Value);
                })
                ,M.SetFormToNotify(0)
            );

            var userInputState = background.GetComponent<BoolUserInputState>();

            bool result;
            if (!userInputState.NewUserInput && opened.HasValue) {
                result = opened.Value;
            } else {
                result = userInputState.Value;
            }

            form.BeginControls(background);

            var caretRight = "\uf0da";
            var caretDown = "\uf0d7";

            IconFontAwesome(result ? caretDown : caretRight, Theme.Instance.LineHeight * 0.8f, P.Left(Theme.Instance.LineHeight));

            Label(label,P.RigidFill);
            
            /*var labelElement = form.Element(P.Fill, form.Library.Label
                , M.SetText(label)
                , M.SetColor(Theme.Instance.LabelColor)
                );*/


            var innerSize = form.EndControls();

            (positioner?? Form.DefaultControlPositioner) (background, form.CurrentBorders, () => {
                return new Vector2(innerSize.x, Theme.Instance.LineHeight);
            });

            return result;
        }


        public static void Rectangle(Positioner positioner, Color color, float radius = 0) {
            var form = Form.Current;
            var element = form.Element(null
                , M.AddComponent<RoundedRectangle>()
                , M.SetRectangleCorners(radius)
                , M.SetColor(color)
                );
            positioner(element, form.CurrentBorders, () => new Vector2(100, 100));
        }


        public static void Circle(Positioner positioner, Color color, float angle = 1, float startAngle = 0, int numSegments = 64) {
            var form = Form.Current;
            var element = form.Element(null
                , M.AddCircle(angle,startAngle,numSegments)
                , M.SetColor(color)
                );
            positioner(element, form.CurrentBorders, () => new Vector2(100, 100));
        }

        public static void CircleOutline(Positioner positioner, Color color, float angle = 1, float startAngle = 0, float innerThickness = 0, float outerThickness = 0, int numSegments = 64) {
            var form = Form.Current;
            var element = form.Element(null
                , M.AddCircleOutline(innerThickness, outerThickness, angle, startAngle, numSegments)
                , M.SetColor(color)
                );
            positioner(element, form.CurrentBorders, () => new Vector2(100, 100));
        }

        public static void CircleOutlineScreenSpaceThickness(Positioner positioner, Color color, float angle = 1, float startAngle = 0, float innerThickness = 0, float outerThickness = 0, float screenSpaceThickness = 1, int numSegments = 64) {
            var form = Form.Current;
            var element = form.Element(null
                , M.AddCircleOutline(innerThickness, outerThickness, angle, startAngle, numSegments)
                , M.SetColor(color)
                , M.SetCustomShader("FUI/ScreenSpaceOffset", ("Thickness", screenSpaceThickness))
                );
            positioner(element, form.CurrentBorders, () => new Vector2(100, 100));
        }


        public static Disposable Panel(Positioner positioner, float radius = 0) {
            var form = Form.Current;
            return form.Group(positioner
                    , M.AddComponent<RoundedRectangle>()
                    , M.SetRectangleCorners(radius)
                    , M.SetColor(Theme.Instance.PanelBackgroundColor)
                    );                    
        }
        public static Disposable WindowBackground(Positioner? positioner = null) {
            var form = Form.Current;
            return form.Group(positioner?? P.Fill
                    , M.AddComponent<RoundedRectangle>()
                    , M.SetColor(Theme.Instance.WindowBackgroundColor)
                    );
        }


        public static T SubForm<T>(Positioner positioner) where T : Form {
            var form = Form.Current;
            var subform = form.Element(null
                , M.AddComponent<T>()
            );
            var result = subform.GetComponent<T>();
            positioner(subform, form.CurrentBorders, () => result.RigidSize);
            return result;
        }


        public static float Slider(float value, Positioner? positioner = null, Color? backgroundColor = null, Color? handleColor = null, int numExtraIterations = 0) {
            var form = Form.Current;

            var background = form.Element(null
                , M.AddComponent<RoundedRectangle>()
                , M.AddComponent<Slider>()
                , M.SetFormToNotify(numExtraIterations)
                , M.SetColor(backgroundColor ?? new Color(0, 0, 0, 0))
            );


            (positioner?? Form.DefaultControlPositioner)(background, form.CurrentBorders, () => new Vector2(80, Theme.Instance.LineHeight));


            form.BeginControls(background);
            var handle = form.Element(null
                , M.AddComponent<RoundedRectangle>()
                , M.SetColor(handleColor ?? Color.white)
                , M.DisableRaycastTarget()
            );


            var slider = background.GetComponent<Gears.Slider>();
            if (slider.NewUserInput) {
                value = slider.Value.x;
            }
            value = Mathf.Clamp01(value);
            if (!slider.Moving) {
                slider.Value = new Vector2(value, 0);
            }

            var handleWidth = 2;
            form.GapLeft(-handleWidth * value, value);

            P.Left(handleWidth)(handle, form.CurrentBorders, () => new Vector2(10, 10));

            form.EndControls();

            return value;
        }

        public static void IconFontAwesome(string icon, float size, Positioner positioner) {
            IconFontAwesome(icon, size, Theme.Instance.LabelColor, positioner);
        }

        public static void IconFontAwesome(string icon, float size, Color color, Positioner positioner) {
            var form = Form.Current;
            var iconElement = form.Element(form.Library.FontAwesomeIcon
                , M.SetColor(color)
                , M.SetText(icon)
                , M.SetFontSize(size)
                , M.DisableRaycastTarget()
                );

            positioner(iconElement, form.CurrentBorders, () => new Vector2(size, size));
        }

        public static void IconButtonFontAwesome(string icon, float size, Action action, Positioner positioner) {
            IconButtonFontAwesome(icon, size, Theme.Instance.LabelColor, action, positioner);
        }
        public static void IconButtonFontAwesome(string icon, float size, Color color, Action action, Positioner positioner) {
            var form = Form.Current;
            var background = form.Element(null
                ,M.AddComponent<RoundedRectangle>()
                ,M.AddComponent<TransparentButtonHighlighter>()
                ,M.AddClickHandler(action)
                ,M.SetRectangleCorners(4)
                );
            

            form.BeginControls(background);
            IconFontAwesome(icon, size, color, P.RigidFill);

            form.EndControls();

            positioner(background, form.CurrentBorders, () => new Vector2(size, size));


            /*var iconElement = CreateSubControl(background.transform, Library.FontAwesomeIconSolid);

            var mouseHandler = background.GetComponent<PointerClickHandler>();
            mouseHandler.OnClick = action;


            var text = iconElement.GetComponent<TMP_Text>();
            text.text = icon;
            text.fontSize = size;
            text.color = color;

            positioner(background, CurrentBorders, () => new Vector2(size, size));*/
        }



        public static void ColoredButton(string text, Action action, Color? color = null, Positioner? positioner = null) {
            var form = Form.Current;

            var backgroundColor = color ?? Theme.Instance.PrimaryColor;
            var labelColor = backgroundColor.ContrastColor();


            using (form.Group(positioner ?? P.Up(Theme.Instance.LineHeight)
                    , M.AddComponent<RoundedRectangle>()
                    , M.SetRectangleCorners(4)
                    , M.AddPressedHoveredHighlighter(
                        backgroundColor,
                        backgroundColor.MultiplySaturationBrightness(1.1f, 1.1f),
                        backgroundColor.MultiplySaturationBrightness(1.3f, 1.3f)
                        )
                    , M.AddClickHandler(action)
                    )
                    ) {
                form.Padding(10, 10, 0, 0);
                form.LabelModifiable(P.Fill
                    , M.SetText(text)
                    , M.SetColor(labelColor)
                    , M.SetTextAlignment(TMPro.HorizontalAlignmentOptions.Center)
                    );
            }
        }

        public static void Button(string text, Action action, Positioner? positioner = null) {
            var form = Form.Current;
            using (form.Group(positioner ?? P.Up(Theme.Instance.LineHeight)
                    , M.AddComponent<RoundedRectangle>()
                    , M.SetRectangleCorners(4)
                    , M.AddComponent<ButtonHighlighter>()
                    , M.AddClickHandler(action)
                    )
                    ) {
                form.Padding(10, 10, 0, 0);
                form.LabelModifiable(P.Fill
                    , M.SetText(text)
                    , M.SetColor(Theme.Instance.LabelColor)
                    , M.SetTextAlignment(TMPro.HorizontalAlignmentOptions.Center)
                    );
            }
        }
        
        public static void DisabledButton(string text, Positioner? positioner = null) {
            var form = Form.Current;
            using (form.Group(positioner ?? P.Up(Theme.Instance.LineHeight)
                       , M.AddComponent<RoundedRectangle>()
                       , M.SetRectangleCorners(4)
                       , M.SetColor(Theme.Instance.ButtonColor * 0.75f)
                   )
                  ) {
                form.Padding(10, 10, 0, 0);
                form.LabelModifiable(P.Fill
                    , M.SetText(text)
                    , M.SetColor(Theme.Instance.LabelColor * 0.75f)
                    , M.SetTextAlignment(TMPro.HorizontalAlignmentOptions.Center)
                );
            }
        }

        public static Disposable ZoomPanViewport(Positioner positioner, Vector2 contentSize) {
            var form = Form.Current;
            var background = form.Element(null
                , M.AddComponent<RoundedRectangle>()
                , M.AddMask(false)
                , M.AddClickHandlerEx((go, e) => {
                    if (e.clickCount == 2) {
                        var component = go.GetComponent<ZoomPanViewport>();
                        component.IntScale = 0;
                        component.ViewportCenterInContent = Vector2.zero;
                    }
                })
                , M.AddZoomPanViewport(contentSize)
            );

            positioner(background, form.CurrentBorders, () => contentSize);

            var content = background.GetComponent<ZoomPanViewport>().Content;
            form.BeginControls(content);

            return new Disposable(() => {
                form.EndControls();
            });

        }

    }
}