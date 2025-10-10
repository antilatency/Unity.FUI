using FUI.Gears;
using FUI.Modifiers;

using System;
using System.Linq;

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FUI {
#nullable enable

    public static partial class Shortcuts {
        /*public static void Label(string value, Positioner? positioner = null) {
            Form.Current.LabelModifiable(positioner ?? Form.DefaultControlPositioner, new SetText(value));
        }*/
        public static void Label(string text, Positioner? positioner = null, params Modifier[] additionalModifiers) {
            Label(text, positioner, new ModifiersList(additionalModifiers));
        }
        public static RectTransform Label(string text, Positioner? positioner = null, ModifiersList? additionalModifiers = null) {
            var form = Form.Current;
            var modifiers = new ModifiersList() {
                new AddComponent<TextMeshProUGUI>(),
                new SetText(text),
                new SetTextOverflowEllipsis(),
                additionalModifiers
            };

            var result = form.Element(null, modifiers);
            (positioner ?? form.DefaultControlPositioner).Invoke(result, form.CurrentBorders, () => {
                var component = result.GetComponent<TMP_Text>();
                var size = component.GetPreferredValues();
                size.x = Mathf.Ceil(size.x);
                size.y = Mathf.Ceil(size.y);
                return size;
            });
            return result;
        }

        public static Disposable<RectTransform> Labeled(string label, Positioner? positioner = null) {
            var form = Form.Current;
            Disposable<RectTransform> result = form.Group(positioner ?? form.DefaultControlPositioner);
            Label(label, P.Left(0, 0.5f));
            return result;
        }
        public static RectTransform LabeledInputField<T>(string label, T value, Action<T> returnAction, Positioner? positioner = null, string toStringFormat = "0.######", Func<string, T>? fromString = null) where T : notnull {
            var form = Form.Current;
            using (var result = Labeled(label, positioner)) {
                form.InputField(value, returnAction, P.Fill, toStringFormat, fromString);
                return result;
            }
        }

        public static RectTransform LabeledDropdown<T>(string label, T value, Action<T> returnAction, Positioner? positioner = null, string[]? options = null) {
            var form = Form.Current;
            using (var group = Labeled(label, positioner)) {
                Dropdown(value, returnAction, P.Fill, options);
                return group.Value;
            }
        }
        /*public static RectTransform LabeledDropdown(string label, int value, string[] options, Action<int> returnAction, Positioner? positioner = null) {
            var form = Form.Current;
            using (var group = Labeled(label, positioner)) {
                Dropdown(value, returnAction, P.Fill, options);
                return group.Value;
            }
        }*/

        public static RectTransform LabeledCheckbox(string label, bool value, Action<bool> returnAction, Positioner? positioner = null) {
            using (var result = Labeled(label, positioner)) {
                Checkbox(value, returnAction, P.Left(Form.Current.Theme.LineHeight));
                return result;
            }
        }


        

        public delegate RectTransform ToggleGroupElementDrawer(string text, bool first, bool last, bool selected, ButtonAction action, Positioner positioner);

        public static RectTransform ToggleGroup(ToggleGroupElementDrawer toggleGroupElementDrawer, int value, string[] options, Action<int> returnAction, Positioner? positioner = null, float gap = 1, float padding = 0) {

            var form = Form.Current;
            var theme = form.Theme;

            using (var group = form.Group(positioner ?? form.DefaultControlPositioner)) {
                form.Padding(padding, padding, 0, 0);
                for (int i = 0; i < options.Length; i++) {
                    int index = i;
                    var selected = value == index;
                    bool first = i == 0;
                    bool last = i == options.Length - 1;
                    toggleGroupElementDrawer(options[i], first, last, selected, (g, e) => {
                        if (value != index) {
                            returnAction(index);
                        }
                        /*if (value == index) {
                            return; // already selected
                        }
                        state.UserInput(index);*/
                    }, P.RowElement(options.Length, gap, 2 * padding));
                }
                return group.Value;
            }

            /*if (positioner == null)
                positioner = form.DefaultControlPositioner;

            var element = form.Element();

            form.BeginControls(element);
            
            }
            form.EndControls();
            (positioner ?? form.DefaultControlPositioner)
                (element, form.CurrentBorders, () => new Vector2(Theme.Instance.LineHeight, Theme.Instance.LineHeight));

            return element;*/
        }

        public static RectTransform ToggleGroupButtons(int value, string[] options, Action<int> returnAction, Positioner? positioner = null, float gap = 1, float padding = 0) {

            RectTransform Button(string text, bool first, bool last, bool selected, ButtonAction action, Positioner positioner) {
                var form = Form.Current;
                var theme = form.Theme;
                var radius = theme.Radius;
                var backgroundColor = selected ? theme.SelectedButtonColor : theme.ButtonColor;
                var backgroundColorHovered = selected ? theme.SelectedButtonHoveredColor : theme.ButtonHoveredColor;
                var backgroundColorPressed = selected ? theme.SelectedButtonPressedColor : theme.ButtonPressedColor;
                var textColor = selected ? theme.SelectedButtonTextColor : theme.ButtonTextColor;
                var buttonHorizontalPadding = theme.ButtonHorizontalPadding;
                using (var group = form.Group(positioner ?? P.Up(Theme.Instance.LineHeight)
                        , new AddComponent<RoundedRectangle>()
                        , new SetRectangleCorners(first ? radius : 0, last ? radius : 0, first ? radius : 0, last ? radius : 0)
                        , new AddPressedHoveredHighlighter(
                            backgroundColor,
                            backgroundColorHovered,
                            backgroundColorPressed
                            )
                        , new AddClickHandlerEx(action)
                        )
                        ) {
                    form.Padding(buttonHorizontalPadding, buttonHorizontalPadding, 0, 0);
                    Label(text
                        , P.Fill
                        , new SetColor(textColor)
                        , new SetTextAlignment(TMPro.HorizontalAlignmentOptions.Center)
                        );
                    return group.Value;
                }
            }

            return ToggleGroup(Button, value, options, returnAction, positioner, gap, padding);
        }

        public static void ToggleGroupButtons<T>(T value, Action<T> returnAction, Positioner? positioner = null
            , float gap = 0
            , float externalPaddingCompensation = 0) where T : struct, Enum {

            var helper = new EnumHelper<T>();
            var optionIndex = helper.ValueToIndex(value);
            ToggleGroupButtons(optionIndex
                , helper.NamesTrimmed
                , index => returnAction(helper.IndexToValue(index)), positioner, gap, externalPaddingCompensation);
        }

        public static RectTransform Checkbox(bool value, Action<bool> returnAction, Positioner? positioner = null) {
            return ToggleButton(value, "\uf00c", "", returnAction, positioner, false);
        }

        public static RectTransform ToggleButton(bool value, string onText, string offText
            , Action<bool> returnAction
            , Positioner? positioner = null, bool paddings = true) {

            var form = Form.Current;
            var theme = form.Theme;
            var buttonHorizontalPadding = theme.ButtonHorizontalPadding;
            var radius = theme.Radius;
            var backgroundColor = value ? theme.SelectedButtonColor : theme.ButtonColor;
            var hoveredColor = value ? theme.SelectedButtonHoveredColor : theme.ButtonHoveredColor;
            var pressedColor = value ? theme.SelectedButtonPressedColor : theme.ButtonPressedColor;

            var textColor = value ? theme.SelectedButtonTextColor : theme.ButtonTextColor;
            var text = value ? onText : (offText ?? onText);

            using (var group = form.Group(positioner ?? form.DefaultControlPositioner
                , new AddComponent<RoundedRectangle>()
                , new SetRectangleCorners(radius)
                , new AddPressedHoveredHighlighter(
                    backgroundColor,
                    hoveredColor,
                    pressedColor
                )
                , new AddClickHandlerEx((go, e) => {
                    returnAction(!value);
                })
            )
            ) {
                if (paddings)
                    form.Padding(buttonHorizontalPadding, buttonHorizontalPadding, 0, 0);
                Label(text, P.RigidFill
                    , new SetColor(textColor)
                    , new SetTextAlignment(HorizontalAlignmentOptions.Center)
                    );

                return group.Value;
            }


            /*RectTransform background = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new AddComponent<ConfigurablePressedHoveredHighlighter>()

                //, new AddComponent<BoolUserInputState>()
                //, new SetFormToNotify(extraIteration)
                , new AddComponent<PointerClickHandler>()
                , new SetRectangleCorners(radius)
                , new AddClickHandlerEx((go, e) => {
                    returnAction(!value);
                })
            );


            

            var highlighter = background.GetComponent<ConfigurablePressedHoveredHighlighter>();
            highlighter.initialColor = backgroundColor;
            highlighter.pressedColor = pressedColor;
            highlighter.hoveredColor = hoveredColor;

            form.BeginControls(background);

            form.Padding(buttonHorizontalPadding, buttonHorizontalPadding, 0, 0);
            Label(text, P.RigidFill
                , new SetColor(textColor)
                , new SetTextAlignment(HorizontalAlignmentOptions.Center)
                );

            form.EndControls();

            (positioner ?? form.DefaultControlPositioner)(background, form.CurrentBorders, () => new Vector2(Theme.Instance.LineHeight, Theme.Instance.LineHeight));

            return background;*/
        }


        /*public static bool Checkbox(bool value, Positioner? positioner = null, bool extraIteration = false) {
            var form = Form.Current;
            var theme = form.Theme;
            RectTransform background = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new AddComponent<ButtonHighlighter>()
                , new AddComponent<BoolUserInputState>()
                //, new SetFormToNotify(extraIteration)
                , new AddComponent<PointerClickHandler>()
                , new SetRectangleCorners(theme.Radius)
                , new AddClickHandlerEx((go, e) => {
                    var input = go.GetComponent<BoolUserInputState>();
                    input.UserInput(!input.Value);
                })
            );


            var userInputState = background.GetComponent<BoolUserInputState>();
            userInputState.SetFormToNotify(form, extraIteration);
            bool result;
            if (!userInputState.NewUserInput) {
                userInputState.Value = value;
                result = value;
            }
            else {
                result = userInputState.Value;
            }

            form.BeginControls(background);

            if (result) {
                IconFontAwesome("\uf00c", Theme.Instance.LineHeight * 0.8f, P.Fill);
            }

            form.EndControls();

            (positioner ?? Form.DefaultControlPositioner)(background, form.CurrentBorders, () => new Vector2(Theme.Instance.LineHeight, Theme.Instance.LineHeight));

            return result;
        }*/


        public static RectTransform LabeledInputFieldSpinbox(string label, int value, Action<int> returnAction, int dragStepSize = 1, Positioner? positioner = null, string toStringFormat = "", Func<string, int>? fromString = null) {
            var form = Form.Current;
            using (var result = Labeled(label, positioner)) {
                Spinbox(value, returnAction, dragStepSize, P.Right(Theme.Instance.LineHeight));
                form.GapRight(2);
                form.InputField(value, returnAction, P.Fill, toStringFormat, fromString);
                return result;
            }
        }

        public static RectTransform LabeledInputFieldSpinbox(string label, float value, Action<float> returnAction, float dragStepSize = 1, Positioner? positioner = null, string toStringFormat = "0.######", Func<string, float>? fromString = null) {
            var form = Form.Current;
            using (var result = Labeled(label, positioner)) {
                Spinbox(value, returnAction, dragStepSize, P.Right(Theme.Instance.LineHeight));
                form.GapRight(2);
                form.InputField(value, returnAction, P.Fill, toStringFormat, fromString);
                return result;
            }
        }

        public static RectTransform LabeledInputFieldSpinbox(string label, double value, Action<double> returnAction, double dragStepSize = 1, Positioner? positioner = null, string toStringFormat = "0.######", Func<string, double>? fromString = null) {
            var form = Form.Current;
            using (var result = Labeled(label, positioner)) {
                Spinbox(value, returnAction, dragStepSize, P.Right(Theme.Instance.LineHeight));
                form.GapRight(2);
                form.InputField(value, returnAction, P.Fill, toStringFormat, fromString);
                return result;
            }
        }


        public static RectTransform Spinbox(int value, Action<int> returnAction, int dragStepSize = 1, Positioner? positioner = null) {
            return Spinbox<int>(value, returnAction, positioner, (v, d) => v / dragStepSize * dragStepSize + dragStepSize * d);
        }
        public static RectTransform Spinbox(float value, Action<float> returnAction, float dragStepSize, Positioner? positioner = null) {
            return Spinbox<float>(value, returnAction, positioner, (v, d) => {
                var s = Math.Round(v / dragStepSize + d);
                return (float)(s * dragStepSize);
            });
        }
        public static RectTransform Spinbox(double value, Action<double> returnAction, double dragStepSize, Positioner? positioner = null) {
            return Spinbox<double>(value, returnAction, positioner
            , (v, d) => Math.Round(v / dragStepSize) * dragStepSize + dragStepSize * d);
        }

        public static RectTransform Spinbox<T>(T value, Action<T> returnAction, Positioner? positioner, Func<T, int, T> deltaToValue) where T : struct, System.IEquatable<T> {
            var form = Form.Current;
            var theme = form.Theme;
            var radius = theme.Radius;
            var backgroundColor = theme.ButtonColor;
            var backgroundColorHovered = theme.ButtonHoveredColor;
            var backgroundColorPressed = theme.ButtonPressedColor;
            var textColor = theme.ButtonTextColor;

            using (var group = form.Group(positioner ?? form.DefaultControlPositioner
                , new AddComponent<RoundedRectangle>()
                , new SetRectangleCorners(radius)
                , new AddPressedHoveredHighlighter(
                    backgroundColor,
                    backgroundColorHovered,
                    backgroundColorPressed
                )
                , new AddDraggable((go, e) => {
                    var multiplier = Input.GetKey(KeyCode.LeftShift) ? 10 : 1;
                    var newValue = deltaToValue(value, Mathf.RoundToInt(multiplier * (e.delta.x + e.delta.y)));
                    returnAction(newValue);
                })
            )) {
                Label("\uf424", P.Fill
                    ,new SetFontSize(theme.LineHeight * 0.6f)
                    , new SetTextAlignmentCenter()
                    );
                return group.Value;
            }
            

            /*RectTransform background = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new AddComponent<ButtonHighlighter>()

                //, new SetFormToNotify(extraIteration)
                , new AddComponent<PointerClickHandler>()
                , new SetRectangleCorners(theme.Radius)

                , new AddDraggable((go, e) => {
                    var multiplier = Input.GetKey(KeyCode.LeftShift) ? 10 : 1;
                    var newValue = deltaToValue(value, Mathf.RoundToInt(multiplier * (e.delta.x + e.delta.y)));
                    returnAction(newValue);
                })
            );*/

            /*var userInputState = background.GetComponent<S>();
            userInputState.SetFormToNotify(form, extraIteration);
            T result;
            if (!userInputState.NewUserInput) {
                userInputState.Value = value;
                result = value;
            }
            else {
                result = userInputState.Value;
            }*/
            /*form.BeginControls(background);
            
            form.EndControls();
            (positioner ?? form.DefaultControlPositioner)(background, form.CurrentBorders, () => new Vector2(Theme.Instance.LineHeight, Theme.Instance.LineHeight));
            return background;*/
        }


        [Obsolete]
        public static bool ExpandableGroupHeader(string label, Positioner? positioner = null, bool? opened = null) {
            var form = Form.Current;
            var theme = form.Theme;
            var background = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new SetRectangleCorners(theme.Radius)
                //, theme.GetButtonHighlighter()
                , new AddComponent<BoolUserInputState>()
                , new AddClickHandlerEx((go, e) => {
                    var input = go.GetComponent<BoolUserInputState>();
                    input.UserInput(!input.Value);
                })
            //, new SetFormToNotify()
            );

            var userInputState = background.GetComponent<BoolUserInputState>();
            userInputState.SetFormToNotify(form, false);
            bool result;
            if (!userInputState.NewUserInput && opened.HasValue) {
                result = opened.Value;
            }
            else {
                result = userInputState.Value;
            }

            form.BeginControls(background);

            var caretRight = "\uf0da";
            var caretDown = "\uf0d7";

            Label(result ? caretDown : caretRight, P.Left(Theme.Instance.LineHeight), new SetFontSize(Theme.Instance.LineHeight * 0.6f));

            Label(label, P.RigidFill);

            /*var labelElement = form.Element(P.Fill, form.Library.Label
                , new SetText(label)
                , new SetColor(Theme.Instance.LabelColor)
                );*/


            var innerSize = form.EndControls();

            (positioner ?? form.DefaultControlPositioner)(background, form.CurrentBorders, () => {
                return new Vector2(innerSize.x, Theme.Instance.LineHeight);
            });

            return result;
        }


        public static void Rectangle(Positioner positioner, Color color, float radius = 0) {
            var form = Form.Current;
            var element = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new SetRectangleCorners(radius)
                , new SetColor(color)
                );
            positioner(element, form.CurrentBorders, () => new Vector2(100, 100));
        }




        public static void Circle(Positioner positioner, Color color, float angle = 1, float startAngle = 0, int numSegments = 64) {
            var form = Form.Current;
            var element = form.Element(null
                , new AddCircle(angle, startAngle, numSegments)
                , new SetColor(color)
                );
            positioner(element, form.CurrentBorders, () => new Vector2(100, 100));
        }

        public static void CircleOutline(Positioner positioner, Color color, float angle = 1, float startAngle = 0, float innerThickness = 0, float outerThickness = 0, int numSegments = 64) {
            var form = Form.Current;
            var element = form.Element(null
                , new AddCircleOutline(innerThickness, outerThickness, angle, startAngle, numSegments)
                , new SetColor(color)
                );
            positioner(element, form.CurrentBorders, () => new Vector2(100, 100));
        }

        public static void CircleOutlineScreenSpaceThickness(Positioner positioner, Color color, float angle = 1, float startAngle = 0, float innerThickness = 0, float outerThickness = 0, float screenSpaceThickness = 1, int numSegments = 64) {
            var form = Form.Current;
            var element = form.Element(null
                , new AddCircleOutline(innerThickness, outerThickness, angle, startAngle, numSegments)
                , new SetColor(color)
                , new SetCustomShader("FUI/ScreenSpaceOffset", ("Thickness", screenSpaceThickness))
                );
            positioner(element, form.CurrentBorders, () => new Vector2(100, 100));
        }

        public static void CubicSpline(
            Positioner positioner,
            Color color,
            Vector2 pointA, Vector2 tangentA,
            Vector2 pointB, Vector2 tangentB,


            float innerThickness = 0,
            float outerThickness = 0,
            int numSegments = 64
        ) {
            var form = Form.Current;
            var element = form.Element(
                null,
                new AddCubicSpline(pointA, tangentA, pointB, tangentB, innerThickness, outerThickness, numSegments),
                new SetColor(color)
            );
            positioner(element, form.CurrentBorders, () => new Vector2(100, 100));
        }




        public static Disposable<RectTransform> Panel(Positioner positioner, float radius = 0) {
            var form = Form.Current;
            return form.Group(positioner
                    , new AddComponent<RoundedRectangle>()
                    , new SetRectangleCorners(radius)
                    , new SetColor(Theme.Instance.PanelBackgroundColor)
                    );
        }
        public static Disposable<RectTransform> WindowBackground(Positioner? positioner = null) {
            var form = Form.Current;
            var theme = form.Theme;
            return form.Group(positioner ?? P.Fill
                    , new AddComponent<RoundedRectangle>()
                    , new SetColor(theme.WindowBackgroundColor)
                    );
        }
        public static Disposable<RectTransform> PopupBackground(Positioner? positioner = null) {
            var form = Form.Current;
            var theme = form.Theme;

            return form.Group(positioner ?? P.Fill
                    , new AddComponent<RoundedRectangle>()
                    , new SetColor(theme.PopupBackgroundColor)
                    );
        }


        public static T SubForm<T>(Positioner positioner) where T : Form {
            var form = Form.Current;
            var subform = form.Element(null
                , new AddComponent<T>()
            );
            var result = subform.GetComponent<T>();
            positioner(subform, form.CurrentBorders, () => result.RigidSize);
            return result;
        }


        public static float Slider(float value, Positioner? positioner = null, Color? backgroundColor = null, Color? handleColor = null, bool extraIteration = false) {
            var form = Form.Current;

            var background = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new AddComponent<Slider>()
                //, new SetFormToNotify(extraIteration)
                , new SetColor(backgroundColor ?? new Color(0, 0, 0, 0))
            );


            (positioner ?? form.DefaultControlPositioner)(background, form.CurrentBorders, () => new Vector2(80, Theme.Instance.LineHeight));


            form.BeginControls(background);
            var handle = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new SetColor(handleColor ?? Color.white)
                , new SetRaycastTarget(false)
            );


            var slider = background.GetComponent<Gears.Slider>();
            slider.SetFormToNotify(form, extraIteration);

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

        /*public static void IconFontAwesome(string icon, float size, Positioner positioner) {
            IconFontAwesome(icon, size, Theme.Instance.LabelColor, positioner);
        }

        public static void IconFontAwesome(string icon, float size, Color color, Positioner positioner) {
            var form = Form.Current;
            var iconElement = form.Element(form.Library.FontAwesomeIcon
                , new SetColor(color)
                , new SetText(icon)
                , new SetFontSize(size)
                , new SetRaycastTarget(false)
                );

            positioner(iconElement, form.CurrentBorders, () => new Vector2(size, size));
        }

        public static void IconButtonFontAwesome(string icon, float size, Action action, Positioner positioner) {
            IconButtonFontAwesome(icon, size, Theme.Instance.LabelColor, action, positioner);
        }*/

        /*public static IDisposable TransparentButton(Action action, Positioner positioner) {
            var form = Form.Current;
            var group = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new AddComponent<TransparentButtonHighlighter>()
                , new AddClickHandler(action)
                , new SetRectangleCorners(4)
                );


            form.BeginControls(group);

            return new Disposable(() => {
                var innerSize = form.EndControls();
                positioner(group, form.CurrentBorders, () => innerSize);
            });
        }*/

        /*public static void IconButtonFontAwesome(string icon, float size, Color color, Action action, Positioner positioner) {
            using (TransparentButton(action, positioner)) {
                IconFontAwesome(icon, size, color, P.RigidFill);
            }

        }*/

        /*public static void ColoredButton(string text, Action action, Color? color = null, Positioner? positioner = null) {
            var form = Form.Current;
            var theme = form.Theme;

            var backgroundColor = color ?? Theme.Instance.PrimaryColor;
            var labelColor = backgroundColor.ContrastColor();

            using (form.Group(positioner ?? P.Up(Theme.Instance.LineHeight)
                    , new AddComponent<RoundedRectangle>()
                    , new SetRectangleCorners(4)
                    , new AddPressedHoveredHighlighter(
                        backgroundColor,
                        theme.HoverColor(backgroundColor),
                        theme.PressedColor(backgroundColor)
                        )
                    , new AddClickHandler(action)
                    )
                    ) {
                form.Padding(10, 10, 0, 0);
                form.LabelModifiable(P.Fill
                    , new SetText(text)
                    , new SetColor(labelColor)
                    , new SetTextAlignment(TMPro.HorizontalAlignmentOptions.Center)
                    );
            }
        }*/

        /*public static RectTransform Dropdown<T>(T value, Action<T> returnAction, Positioner? positioner = null) where T : struct, Enum {
            var helper = new EnumHelper<T>();
            var optionIndex = helper.ValueToIndex(value);
            //var serializableReturnAction = new SerializableAction<T>(returnAction);
            return Dropdown(optionIndex, helper.Names, x => returnAction(helper.IndexToValue(x)), positioner);
        }*/


        public static RectTransform Dropdown<T>(T value, Action<T> returnAction, Positioner? positioner = null, string[]? options = null) {
            var form = Form.Current;
            var theme = form.Theme;
            var buttonHorizontalPadding = theme.ButtonHorizontalPadding;
            var backgroundColor = theme.ButtonColor;
            var backgroundColorHovered = theme.ButtonHoveredColor;
            var backgroundColorPressed = theme.ButtonPressedColor;
            var textColor = theme.ButtonTextColor;

            string name;
            if (value is Enum) {
                var index = EnumHelper.ValueToIndex(value);
                var opts = options ?? EnumHelper.NamesTrimmed<T>();
                name = opts[index];
            } else {
                var index = Convert.ToInt32(value);
                var opts = options ?? throw new ArgumentNullException(nameof(options), "Options must be provided for non-enum types");
                name = opts[index];
            }
            //var name = options[value];

            using (var group = form.Group(positioner ?? form.DefaultControlPositioner
                , new AddComponent<RoundedRectangle>()
                , new SetRectangleCorners(theme.Radius)
                , new AddPressedHoveredHighlighter(
                    backgroundColor,
                    backgroundColorHovered,
                    backgroundColorPressed
                    )
                , new AddClickHandlerEx((go, e) => {
                    DropDownDialog.Open(go, value, returnAction, options);
                })
            )) {
                form.Padding(buttonHorizontalPadding, buttonHorizontalPadding, 0, 0);

                Label("\uf0d7", P.Right(), new SetColor(textColor), new SetTextAlignmentCenter());

                Label(name, P.RigidFill
                    , new SetColor(textColor)
                    , new SetTextAlignment(HorizontalAlignmentOptions.Left)
                    );
                return group.Value;
            }

            /*var background = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new SetRectangleCorners(theme.Radius)
                , new AddPressedHoveredHighlighter(
                    backgroundColor,
                    backgroundColorHovered,
                    backgroundColorPressed
                    )
                , new AddClickHandlerEx((go, e) => {
                    var mark = go.GetComponent<Mark>();
                    DropDownDialog.Open(go, value, options, returnAction);
                })
            );
            form.BeginControls(background);
            form.Padding(buttonHorizontalPadding, buttonHorizontalPadding, 0, 0);
            Label(name, P.RigidFill
                , new SetColor(textColor)
                , new SetTextAlignment(HorizontalAlignmentOptions.Left)
                );
            Label("\uf0d7", P.Right(), new SetColor(textColor), new SetFontSize(Theme.Instance.LineHeight * 0.6f), new SetTextAlignment(HorizontalAlignmentOptions.Right));
            form.EndControls();*/


            /*Button(name + " \uf0d7", (g, e) => {
                DropDownDialog.Open(g, value, options, (v) => {
                    Mark mark = g.GetComponent<Mark>();
                    mark.SetTemporaryState(v);
                    value = v;
                    form.MakeDirty();
                });
            }, positioner);

            Mark.

            var element = Element(Library.Dropdown);
            positioner(element, CurrentBorders, () => new Vector2(40, Theme.Instance.LineHeight));


            var dropdown = element.GetComponent<TMP_Dropdown>();

            dropdown.options = options.Select(x => new TMP_Dropdown.OptionData(x)).ToList();

            var state = dropdown.GetComponent<DropdownState>();
            state.SetFormToNotify(form, extraIteration);

            if (state.NewUserInput) {
                return state.Value;
            }
            else {
                var editing = dropdown.IsExpanded;
                if (!editing) {
                    state.Value = value;
                }
            }*/
            //return value;
        }


        public static void Button(string text, Action action, Positioner? positioner = null, bool paddings = true) {
            Button(text, (go, e) => action(), positioner, paddings);
        }

        public static void Button(string text, ButtonAction action, Positioner? positioner = null, bool paddings = true) {
            var form = Form.Current;
            var theme = form.Theme;
            var buttonHorizontalPadding = theme.ButtonHorizontalPadding;
            var backgroundColor = theme.ButtonColor;
            var backgroundColorHovered = theme.ButtonHoveredColor;
            var backgroundColorPressed = theme.ButtonPressedColor;
            var textColor = theme.ButtonTextColor;

            using (form.Group(positioner ?? form.DefaultControlPositioner
                    , new AddComponent<RoundedRectangle>()
                    , new SetRectangleCorners(theme.Radius)
                    , new AddPressedHoveredHighlighter(
                        backgroundColor,
                        backgroundColorHovered,
                        backgroundColorPressed
                        )
                    , new AddClickHandlerEx(action)
                    )
                    ) {
                if (paddings) {
                    form.Padding(buttonHorizontalPadding, buttonHorizontalPadding, 0, 0);
                }
                Label(text, P.RigidFill
                    , new SetColor(textColor)
                    , new SetTextAlignment(HorizontalAlignmentOptions.Center)

                    );
            }
        }

        public static void DisabledButton(string text, Positioner? positioner = null, bool paddings = true) {
            var form = Form.Current;
            var theme = form.Theme;
            var buttonHorizontalPadding = theme.ButtonHorizontalPadding;
            var radius = theme.Radius;
            var backgroundColor = theme.DisabledButtonColor;
            var textColor = theme.DisabledButtonTextColor;
            using (form.Group(positioner ?? P.Up(Theme.Instance.LineHeight)
                       , new AddComponent<RoundedRectangle>()
                       , new SetRectangleCorners(radius)
                       , new SetColor(backgroundColor)
                   )
                  ) {
                if (paddings) {
                    form.Padding(buttonHorizontalPadding, buttonHorizontalPadding, 0, 0);
                }
                Label(text, P.Fill
                    , new SetColor(textColor)
                    , new SetTextAlignment(HorizontalAlignmentOptions.Center)
                );
            }
        }

        public static void ColorButton(Color color, Action action, string? text = null, Positioner? positioner = null) {
            var form = Form.Current;
            var theme = form.Theme;
            var outlineColor = Color.white;
            var outlineHoveredColor = Color.white.Multiply(0.8f);
            var outlinePressedColor = Color.white.Multiply(0.6f);
            var thickness = theme.OutlineThickness;
            var radius = theme.Radius;

            using (form.Group(positioner ?? form.DefaultControlPositioner
                , new AddComponent<RoundedRectangle>()
                , new SetRectangleCorners(radius)
                , new AddPressedHoveredHighlighter(
                    outlineColor,
                    outlineHoveredColor,
                    outlinePressedColor
                    )
                , new AddClickHandler(action)
                )) {
                form.Padding(thickness);
                using (form.Group(P.Fill
                    , new AddComponent<RoundedRectangle>()
                    , new SetRectangleCorners(Mathf.Max(0, radius - thickness))
                    , new SetColor(color)
                    )) {
                    if (!string.IsNullOrEmpty(text)) {
                        Label(text!, P.Fill
                            , new SetColor(color.ContrastColor())
                            , new SetTextAlignment(HorizontalAlignmentOptions.Center)
                        );
                    }
                }                
            }
        }


        public static Disposable ZoomPanViewport(Positioner positioner, Vector2 contentSize, PointerEventUtils.PointerEventFilter? scrollFilter = null, PointerEventUtils.PointerEventFilter? dragFilter = null) {
            var form = Form.Current;
            var background = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new AddMask(false)
                , new AddClickHandlerEx((go, e) => {
                    if (e.clickCount == 2) {
                        var component = go.GetComponent<ZoomPanViewport>();
                        component.IntScale = 0;
                        component.ViewportCenterInContent = Vector2.zero;
                    }
                })
                , new AddZoomPanViewport(contentSize, scrollFilter, dragFilter)
            );

            positioner(background, form.CurrentBorders, () => contentSize);

            var content = background.GetComponent<ZoomPanViewport>().Content;
            form.BeginControls(content);

            return new Disposable(() => {
                form.EndControls();
            });

        }

        public static Disposable FitInside(Positioner positioner, Vector2 contentSize) {
            var form = Form.Current;
            var background = form.Element(null
                , new AddComponent<RoundedRectangle>()
                , new AddMask(false)
                , new AddComponent<FitInside>()
            );

            positioner(background, form.CurrentBorders, () => contentSize);

            var component = background.GetComponent<FitInside>();
            component.ContentSize = contentSize;
            var content = component.Content;
            form.BeginControls(content);

            return new Disposable(() => {
                form.EndControls();
            });
        }

        private static Vector2 GetDimension(Positioner? positioner, Dimension.DimensionsMask mask, bool extraIteration = false) {
            var form = Form.Current;

            var observer = form.Element(null
                , new AddComponent<Dimension>()
            //, new SetFormToNotify(extraIteration)
            );
            //var notifier = observer.GetComponent<IFormNotifier>();
            var component = observer.GetComponent<Dimension>();
            component.SetFormToNotify(Form.Current, extraIteration);

            component.Mask = mask;

            float dimension = mask.HasFlag(Dimension.DimensionsMask.Width) ? observer.rect.width : observer.rect.height;
            (positioner ?? P.Fill)(observer, form.CurrentBorders, () => new Vector2(0, 0));

            return new Vector2(component.Width, component.Height);
        }

        public static float GetWidth(Positioner? positioner = null, bool extraIteration = false) => GetDimension(positioner, Dimension.DimensionsMask.Width, extraIteration).x;
        public static float GetHeight(Positioner? positioner = null, bool extraIteration = false) => GetDimension(positioner, Dimension.DimensionsMask.Height, extraIteration).y;
        public static Vector2 GetSize(Positioner? positioner = null, bool extraIteration = false) => GetDimension(positioner, Dimension.DimensionsMask.Both, extraIteration);


        public static void Row(int count, Action<int, Positioner> elementBuilder, Positioner? positioner = null, float gap = 1, float padding = 0) {
            var form = Form.Current;
            using (form.Group(positioner ?? form.DefaultControlPositioner)) {
                form.Padding(padding);
                for (int i = 0; i < count; i++) {
                    elementBuilder(i, P.RowElement(count, gap, padding));
                }
            }
        }
    }

}