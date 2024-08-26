using FUI.Gears;
using System;
using TMPro;
using UnityEngine;

namespace FUI {
#nullable enable

    public static class Shortcuts {
        public static void Label(string value, Positioner? positioner = null) {
            Form.Current.LabelModifiable(positioner??Form.DefaultControlPositioner, M.SetText(value));
        }

        private static Disposable Labeled(string label, Positioner? positioner = null) {
            var form = Form.Current;
            Disposable result = form.Group(positioner ?? Form.DefaultControlPositioner);
            Label(label, P.Left(0, 0.5f));
            return result;
        }

        public static T LabeledInputField<T>(string label, T value, Positioner? positioner = null, Func<string, T>? fromString = null, int numExtraIterations = 0) where T : notnull {
            var form = Form.Current;
            using (Labeled(label, positioner)) {
                return form.InputField(value, P.Fill, fromString, numExtraIterations);
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
                , M.AddClickHandler(x => {
                    var input = x.GetComponent<BoolUserInputState>();
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

        public static bool ExpandableGroupHeader(string label, Positioner? positioner = null, bool? opened = null) {
            var form = Form.Current;

            var background = form.Element(null
                , M.AddComponent<RoundedRectangle>()
                , M.SetRectangleCorners(4)
                , M.AddComponent<ButtonHighlighter>()
                , M.AddComponent<BoolUserInputState>()
                , M.AddClickHandler(x => {
                    var input = x.GetComponent<BoolUserInputState>();
                    input.UserInput(!input.Value);
                })
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

            var labelElement = form.Element(P.Fill, form.Library.Label
                , M.SetText(label)
                , M.SetColor(Theme.Instance.LabelColor)
                );

            var innerBorders = form.CurrentBorders;
            form.EndControls();

            (positioner?? Form.DefaultControlPositioner) (background, form.CurrentBorders, () => {
                var width = labelElement.GetComponent<TMP_Text>().GetPreferredValues().x + innerBorders.GetRigidSize().x + 4;
                return new Vector2(width, Theme.Instance.LineHeight);
            });

            return result;
        }


        public static void Rectangle(Positioner positioner, Color color, float radius = 0) {
            var form = Form.Current;
            var element = form.Element(null
                , M.AddComponent<RoundedRectangle>()
                , M.SetRectangleCorners(4)
                , M.SetColor(color)
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


        public static float Slider(float value, Positioner? positioner = null, Color? backgroundColor = null, Color? handleColor = null) {
            var form = Form.Current;

            var background = form.Element(null
                ,M.AddComponent<RoundedRectangle>()
                ,M.AddComponent<Slider>()
                ,M.SetColor(backgroundColor ?? new Color(0, 0, 0, 0))
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

        public static void IconButtonFontAwesome(string icon, float size, Action<GameObject> action, Positioner positioner) {
            IconButtonFontAwesome(icon, size, Theme.Instance.LabelColor, action, positioner);
        }
        public static void IconButtonFontAwesome(string icon, float size, Color color, Action<GameObject> action, Positioner positioner) {
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



        public static void ColoredButton(string text, Action<GameObject> action, Color? color = null, Positioner? positioner = null) {
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

        public static void Button(string text, Action<GameObject> action, Positioner? positioner = null) {
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



    }
}