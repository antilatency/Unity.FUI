using FUI.Gears;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace FUI {
#nullable enable


    public abstract class Form : MonoBehaviour {

        public PrefabLibrary library = null!;
        public PrefabLibrary Library {
            get {
                if (library == null)
                    library = Resources.Load<PrefabLibrary>("FUI.PrefabLibrary");
                return library;
            }
        }

        public delegate void Positioner(UnityEngine.RectTransform rectTransform, Borders borders, System.Func<UnityEngine.Vector2>? sizeGetter);


        [Serializable]
        public struct Border {
            public float Pixels;
            public float Fraction;
            public void Increment(float pixels, float fraction) {
                Pixels += pixels;
                Fraction += fraction;
            }
        }


        public class Borders {
            public Border Left;
            public Border Right;
            public Border Top;
            public Border Bottom;

            public Vector2 GetRigidSize() {
                var fractionLeftX = 1 - Left.Fraction - Right.Fraction;
                var fractionLeftY = 1 - Top.Fraction - Bottom.Fraction;

                return new Vector2(
                    (Left.Pixels + Right.Pixels) / fractionLeftX,
                    (Top.Pixels + Bottom.Pixels) / fractionLeftY);
            }
        }

        public static Positioner PushLeft(float? size = null, float fraction = 0) {
            return (rectTransform, borders, sizeGetter) => {
                var offset = size ?? sizeGetter?.Invoke().x ?? 0;

                rectTransform.anchorMin = new Vector2(borders.Left.Fraction, borders.Bottom.Fraction);
                rectTransform.anchorMax = new Vector2(borders.Left.Fraction + fraction, 1 - borders.Top.Fraction);

                rectTransform.offsetMin = new Vector2(borders.Left.Pixels, borders.Bottom.Pixels);
                rectTransform.offsetMax = new Vector2(borders.Left.Pixels + offset, -borders.Top.Pixels);

                borders.Left.Increment(offset, fraction);
            };
        }


        public static Positioner PushRight(float? size = null, float fraction = 0) {
            return (rectTransform, borders, sizeGetter) => {
                var offset = size ?? sizeGetter?.Invoke().x ?? 0;

                rectTransform.anchorMin = new Vector2(1 - borders.Right.Fraction - fraction, borders.Bottom.Fraction);
                rectTransform.anchorMax = new Vector2(1 - borders.Right.Fraction, 1 - borders.Top.Fraction);

                rectTransform.offsetMin = new Vector2(-borders.Right.Pixels - offset, borders.Bottom.Pixels);
                rectTransform.offsetMax = new Vector2(-borders.Right.Pixels, -borders.Top.Pixels);

                borders.Right.Increment(offset, fraction);
            };
        }

        public static Positioner PushDown(float? size = null, float fraction = 0) {
            return (rectTransform, borders, sizeGetter) => {
                var offset = size ?? sizeGetter?.Invoke().y ?? 0;

                rectTransform.anchorMin = new Vector2(borders.Left.Fraction, borders.Bottom.Fraction);
                rectTransform.anchorMax = new Vector2(1 - borders.Right.Fraction, borders.Bottom.Fraction + fraction);

                rectTransform.offsetMin = new Vector2(borders.Left.Pixels, borders.Bottom.Pixels);
                rectTransform.offsetMax = new Vector2(-borders.Right.Pixels, borders.Bottom.Pixels + offset);

                borders.Bottom.Increment(offset, fraction);
            };
        }


        public static Positioner PushUp(float? size = null, float fraction = 0) {
            return (rectTransform, borders, sizeGetter) => {
                var offset = size ?? sizeGetter?.Invoke().y ?? 0;

                rectTransform.anchorMin = new Vector2(borders.Left.Fraction, 1 - borders.Top.Fraction - fraction);
                rectTransform.anchorMax = new Vector2(1 - borders.Right.Fraction, 1 - borders.Top.Fraction);

                rectTransform.offsetMin = new Vector2(borders.Left.Pixels, -borders.Top.Pixels - offset);
                rectTransform.offsetMax = new Vector2(-borders.Right.Pixels, -borders.Top.Pixels);

                borders.Top.Increment(offset, fraction);
            };
        }

        public static Positioner Fill => (rectTransform, borders, sizeGetter) => {

            rectTransform.anchorMin = new Vector2(borders.Left.Fraction, borders.Bottom.Fraction);
            rectTransform.anchorMax = new Vector2(1 - borders.Right.Fraction, 1 - borders.Top.Fraction);

            rectTransform.offsetMin = new Vector2(borders.Left.Pixels, borders.Bottom.Pixels);
            rectTransform.offsetMax = new Vector2(-borders.Right.Pixels, -borders.Top.Pixels);
        };


        public static Positioner Gravity(Vector2 normalizedPosition, float? width = null, float? height = null) {
            return (rectTransform, borders, sizeGetter) => {
                rectTransform.anchorMin = normalizedPosition;
                rectTransform.anchorMax = normalizedPosition;
                var size = rectTransform.sizeDelta;

                if (width.HasValue)
                    size.x = width.Value;
                if (height.HasValue)
                    size.y = height.Value;
                rectTransform.sizeDelta = size;

                rectTransform.pivot = normalizedPosition;

            };
        }



        /*public struct Alignment {
            public Direction Direction;
            public float? Pixels;
            public float Fraction;

            public static readonly Alignment Fill = new Alignment { Direction = Direction.Fill };
            public static Alignment Left(float? pixels = null, float fraction = 0) => new Alignment { Direction = Direction.Left, Pixels = pixels, Fraction = fraction};
            public static Alignment Right(float? pixels = null, float fraction = 0) => new Alignment { Direction = Direction.Right, Pixels = pixels, Fraction = fraction};
            public static Alignment Up(float? pixels = null, float fraction = 0) => new Alignment { Direction = Direction.Up, Pixels = pixels, Fraction = fraction};
            public static Alignment Down(float? pixels = null, float fraction = 0) => new Alignment { Direction = Direction.Down, Pixels = pixels, Fraction = fraction};
        }*/


        public class StackItem {
            public int FirstNotValidatedControlIndex = 0;
            public RectTransform Root;
            public Borders Borders = new();
            public StackItem(RectTransform root) {
                Root = root;

            }
        }



        protected abstract void Build();

        [Obsolete("Remove virtual")]
        protected virtual void Update() {
            BeginControls();
            try {
                Build();
            }
            finally {
                EndControls();
            }

            if (Stack.Count != 0)
                throw new InvalidOperationException("The stack is not empty after control operations.");
        }


        public Stack<StackItem> Stack = new();
        public RectTransform CurrentRoot => Stack.Peek().Root;
        public Borders CurrentBorders => Stack.Peek().Borders;


        protected void BeginControls() {
            BeginControls((RectTransform)transform);
        }
        protected void BeginControls(RectTransform newRoot) {
            var newItem = new StackItem(newRoot);
            Stack.Push(newItem);
        }

        protected void EndControls() {
            var stackItem = Stack.Pop();
            var t = stackItem.Root;
            var index = stackItem.FirstNotValidatedControlIndex;

            var count = t.childCount - index;
            while (count > 0) {
                var child = t.GetChild(index);
                DestroyImmediate(child.gameObject);
                count--;
            }


            /*var children = Enumerable.Range(index, t.childCount - index).Select(x => t.GetChild(x)).ToList();
            foreach (var i in children) {
                //i.SetParent(null);
                DestroyImmediate(i.gameObject);
            }*/
        }




        /*public T CreateControl<T>(Action<T>? initialization = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) where T : MonoBehaviour {
            var stackItem = ParentStack.Peek();
            return CreateControl(() => {
                var gameObject = new GameObject(typeof(T).Name);
                var result = gameObject.AddComponent<T>();
                gameObject.transform.SetParent(stackItem.Root);
                return result;

            }, initialization, filePath, lineNumber);
        }*/


        /*public T CreateControl<T>(T original, Action<T>? initialization = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) where T : UnityEngine.Object {

            var stackItem = Stack.Peek();
            return CreateControl(() => {

        }*/


        private T InstantiatePrefab<T>(T original, Transform parent, bool throwIfFailed = true) where T : UnityEngine.Object {
#if UNITY_EDITOR
            T created = (T)UnityEditor.PrefabUtility.InstantiatePrefab(original, parent);
#else
        T created =  Instantiate(original, stackItem.Root);
#endif
            if (throwIfFailed && created == null)
                throw new InvalidOperationException($"The created control is null. {typeof(T).FullName}");

            return created;
        }


        public T CreateSubControl<T>(Transform parent, T original, Action<T>? initializer = null, string guid = "") where T : UnityEngine.Object {
            var objectRefs = parent.GetComponents<ObjectRef>();
            var objectRef = objectRefs.FirstOrDefault(x => x.Guid == guid && x.TypeName == typeof(T).FullName);
            if (objectRef)
                return (T)objectRef.Value;

            var created = InstantiatePrefab(original, parent);
            initializer?.Invoke(created);
            objectRef = parent.gameObject.AddComponent<ObjectRef>();
            objectRef.Value = created;
            objectRef.Guid = guid;
            objectRef.TypeName = typeof(T).FullName;
            return created;
        }



        public T CreateControl<T>(T original, string guid, Action<T>? initializer = null) where T : UnityEngine.Object {

            var stackItem = Stack.Peek();
            var parent = stackItem.Root;
            var index = stackItem.FirstNotValidatedControlIndex;

            //var currentChildren = Enumerable.Range(index, parent.childCount - index).Select(x => parent.GetChild(x)).ToList();

            int FindIndex() {
                for (int i = stackItem.FirstNotValidatedControlIndex; i < parent.childCount; i++) {
                    var child = parent.GetChild(i);
                    if (child.GetComponent<Mark>()?.Value == guid) {
                        return i;
                    }
                }
                return -1;
            }

            /*Transform Find() {
                var count = parent.childCount - index;
                while (count > 0) {
                    var child = parent.GetChild(index);                
                    if (child.GetComponent<Mark>()?.Value == guid) {
                        return child;
                    }
                    DestroyImmediate(child.gameObject);
                    count--;
                }
                return null;
            }*/
            var indexFound = FindIndex();

            if (indexFound > -1) {
                var numElementsToDelete = indexFound - stackItem.FirstNotValidatedControlIndex;
                for (int i = 0; i < numElementsToDelete; i++) {
                    var child = parent.GetChild(stackItem.FirstNotValidatedControlIndex);
                    DestroyImmediate(child.gameObject);
                }
                var result = parent.GetChild(stackItem.FirstNotValidatedControlIndex).GetComponent<T>();
                stackItem.FirstNotValidatedControlIndex++;
                return result;

            } else {

                T created = InstantiatePrefab(original, stackItem.Root);
                initializer?.Invoke(created);

                GameObject gameObject;
                if (created is GameObject g) gameObject = g;
                else if (created is Component c) gameObject = c.gameObject;
                else throw new InvalidOperationException($"The object of type {typeof(T).FullName} is not a GameObject or a Component.");

                gameObject.AddComponent<Mark>().Value = guid;
                gameObject.transform.SetSiblingIndex(index);


                stackItem.FirstNotValidatedControlIndex++;
                return created;
            }

        }


        public void GapLeft(float pixels = 0, float fraction = 0) {
            CurrentBorders.Left.Increment(pixels, fraction);
        }

        public void GapRight(float pixels = 0, float fraction = 0) {
            CurrentBorders.Right.Increment(pixels, fraction);
        }

        public void GapTop(float pixels = 0, float fraction = 0) {
            CurrentBorders.Top.Increment(pixels, fraction);
        }

        public void GapBottom(float pixels = 0, float fraction = 0) {
            CurrentBorders.Bottom.Increment(pixels, fraction);
        }


        private static void CheckInitializerGuid(object? initializer, string? guid) {
            if (initializer != null && guid == null) throw new InvalidOperationException("The 'guid' parameter must be set when the 'initializer' parameter is provided.");
        }

        /*protected T Edit<T>(Edit<T> original, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
            var control = CreateControl(original, null, filePath, lineNumber);
            return control.Value;
        }


        protected T Edit<E, T>([CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) where E : Edit<T> {
            var control = CreateControl<E>(null, filePath, lineNumber);
            return control.Value;
        }*/




        /*public string InputField(Action<TMP_InputField>? initialization = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {        
            var control = CreateControl(Library.InputField, initialization, filePath, lineNumber);
            return control.text;
        }

        public bool Toggle(Action<UnityEngine.UI.Toggle>? initialization = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
            var control = CreateControl(Library.CheckBox);
            return control.isOn;
        }*/




        /*protected void Horizontal(string value, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
            var control = CreateControl(Library.Label, null, filePath, lineNumber);
            control.text = value;
        }*/


        /*public bool LabeledCheckBox(string label, bool value) {
            var control = CreateControl(Library.LabeledCheckBox);
            control.GetComponent<LabelRef>().Value.text = label;
            var input = control.GetComponent<ToggleRef>();
            if (input.NewUserInput) {
                return input.Value;
            } else {
                input.Value = value;
                return value;
            }
        }

        public T LabeledDropdown<T>(string label, T value) where T: struct {
            int intValue = Convert.ToInt32(value);
            string[] options = Enum.GetNames(typeof(T));
            int selectedValue = LabeledDropdown(label, intValue, options);
            return (T)Enum.ToObject(typeof(T), selectedValue);
        }

        public int LabeledDropdown(string label, int value, string[] options) {
            var control = CreateControl(Library.LabeledDropdown);
            control.GetComponent<LabelRef>().Value.text = label;
            var input = control.GetComponent<DropdownRef>();
            input.Options = options;
            if (input.NewUserInput) {
                return input.Value;
            } else {
                input.Value = value;
                return value;
            }
        }

        private InputFieldRef BeginLabeledInputField(RectTransform origin, string label) {
            var control = CreateControl(origin);
            var labelRef = control.GetComponent<LabelRef>();
            labelRef.Value.text = label;
            return control.GetComponent<InputFieldRef>();
        }

        public string LabeledInputField(string label, string value) {
            var input = BeginLabeledInputField(Library.LabeledInputField, label);
            if (input.NewUserInput) {
                return input.Value;
            } else {
                input.Value = value;
                return value;
            }        
        }

        public int LabeledInputFieldInteger(string label, int value) {
            var input = BeginLabeledInputField(Library.LabeledInputFieldInteger, label);

            int FromString(string s) {
                if (!int.TryParse(s, out var result))
                    result = default;
                return result;
            }

            if (input.NewUserInput) {
                return FromString(input.Value);
            } else {
                if (FromString(input.Value) != value)
                    input.Value = value.ToString();
                return value;
            }
        }
        public double LabeledInputFieldDouble(string label, double value) {
            var input = BeginLabeledInputField(Library.LabeledInputFieldDouble, label);
            double FromString(string s) {
                if (!double.TryParse(s, out var result))
                    result = default;
                return result;
            }

            if (input.NewUserInput) {
                return FromString(input.Value);
            } else {
                if (FromString(input.Value) != value)
                    input.Value = value.ToString();
                return value;
            }
        }

        public Vector2 LabeledColorPickerHueSaturation(string label, Vector2 value) {
            var control = CreateControl(Library.ColorPickerHueSaturation);
            var labelRef = control.GetComponent<LabelRef>();
            labelRef.Value.text = label;
            var input =  control.GetComponent<ColorPickerHueSaturationRef>();

            if (input.NewUserInput) {
                return input.Value;
            } else {
                input.Value = value;
                return value;
            }        
        }*/


        /*public string LabeledInputField(string label, string value, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
            var control = CreateControl(Library.LabeledInputField, initialization, filePath, lineNumber);
            control.Label.text = label;
            return control.InputField.text;
        }*/

        /*public int LabeledInputFieldInteger(string label, int initialValue, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
            return LabeledInputFieldInteger(label, x => x.InputField.text = initialValue.ToString(), filePath, lineNumber);
        }*/

        /*public int LabeledInputFieldInteger(string label, Action<LabeledInputField>? initialization = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {

            var control = CreateControl(Library.LabeledInputField, x => {
                initialization?.Invoke(x);
                x.InputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            }, filePath, lineNumber);
            control.Label.text = label;
            if (!int.TryParse(control.InputField.text, out var result))
                result = default;
            return result;
        }*/

        /*public double LabeledInputFieldDouble(string label, double initialValue, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
            return LabeledInputFieldDouble(label, x => x.InputField.text = initialValue.ToString(), filePath, lineNumber);
        }*/

        /*public double LabeledInputFieldDouble(string label, Action<LabeledInputField>? initialization = null, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
            var control = CreateControl(Library.LabeledInputField, x => {
                initialization?.Invoke(x);
                x.InputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            }, filePath, lineNumber);
            control.Label.text = label;
            if (!double.TryParse(control.InputField.text, out var result))
                result = default;
            return result;
        }*/
        public void Rectangle(Color color, Positioner positioner, Action<RoundedRectangle>? initializer = null, string? guid = null) {
            Rectangle(color, 0, positioner, initializer, guid);
        }

        public void Rectangle(Color color, float radius, Positioner positioner, Action<RoundedRectangle>? initializer = null, string? guid = null) {
            Rectangle(color, radius, radius, radius, radius, positioner, initializer, guid);
        }



        public void Rectangle(Color color,
            float topLeftRadius,
            float topRightRadius,
            float bottomLeftRadius,
            float bottomRightRadius,
            Positioner positioner, Action<RoundedRectangle>? initializer = null, string? guid = null) {

            CheckInitializerGuid(initializer, guid);

            var stackItem = Stack.Peek();
            var element = CreateControl(Library.Rectangle, guid ?? "8dcd5378-7e74-4fa8-9b33-20760093568b", initializer);
            element.color = color;
            element.TopLeft = topLeftRadius;
            element.TopRight = topRightRadius;
            element.BottomLeft = bottomLeftRadius;
            element.BottomRight = bottomRightRadius;
            positioner(element.GetComponent<RectTransform>(), stackItem.Borders, () => new Vector2(100, 100));
        }


        /*public void Label(string value, Positioner? positioner = null) {
            Label(value, positioner, Theme.Instance.LabelColor);
        }*/

        public void Label(string value, Positioner? positioner = null, Color? color = null,
            HorizontalAlignmentOptions horizontalAlignment = HorizontalAlignmentOptions.Left,
            VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Capline,
            Action<TMP_Text>? initializer = null, string? guid = null) {

            CheckInitializerGuid(initializer, guid);
            if (positioner == null)
                positioner = DefaultControlPositioner;

            var element = CreateControl(Library.Label, guid ?? "4200c085-cd11-43b7-8608-162651f7ad33", initializer);
            element.text = value;
            element.color = color.HasValue ? color.Value : Theme.Instance.LabelColor;
            element.horizontalAlignment = horizontalAlignment;
            element.verticalAlignment = verticalAlignment;

            positioner(element.GetComponent<RectTransform>(), CurrentBorders, () => element.GetPreferredValues());
        }

        public void Button(string text, Action action, Positioner? positioner = null) {
            Button(text, action, Theme.Instance.ButtonColor, positioner, Theme.Instance.LabelColor, Theme.Instance.ButtonColorHovered, Theme.Instance.ButtonColorPressed);
        }

        public void Button(string text, Action action, Color color, Positioner? positioner = null, Color? labelColor = null, Color? hoveredColor = null, Color? pressedColor = null) {

            if (positioner == null)
                positioner = DefaultControlPositioner;


            hoveredColor ??= color.MultiplySaturationBrightness(1.1f, 1.1f);
            pressedColor ??= color.MultiplySaturationBrightness(1.3f, 1.3f);

            var root = CreateControl(Library.Empty, "a5435824-60ff-4998-ab69-14408c0f6ac4",
                x => {
                    var g = x.gameObject;
                    g.AddComponent<PointerClickHandler>();
                    g.AddComponent<CanvasRenderer>();

                    var background = g.AddComponent<SimpleRoundedRectangle>();

                    g.AddComponent<ConfigurablePressedHoveredHighlighter>();
                    background.Radius = 4;
                }
            );

            labelColor ??= color.ContrastColor();

            var label = CreateSubControl(root, Library.Label, x => {
                x.horizontalAlignment = HorizontalAlignmentOptions.Center;
            });
            label.text = text;
            label.color = labelColor.Value;
            var labelSize = label.GetPreferredValues();

            positioner(root, CurrentBorders, () => labelSize + new Vector2(16, 4));


            var mouseHandler = root.GetComponent<PointerClickHandler>();
            mouseHandler.OnClick = action;

            var highlighter = root.GetComponent<ConfigurablePressedHoveredHighlighter>();
            highlighter.initialColor = color;
            highlighter.pressedColor = pressedColor.Value;
            highlighter.hoveredColor = hoveredColor.Value;

        }
        public void IconButtonFontAwesome(string icon, float size, Action action, Positioner positioner) {
            IconButtonFontAwesome(icon, size, Theme.Instance.LabelColor, action, positioner);
        }
        public void IconButtonFontAwesome(string icon, float size, Color color, Action action, Positioner positioner) {

            var background = CreateControl(Library.Rectangle, "f8158f9c-75a4-48da-9782-c9a991259199", x => {
                x.gameObject.AddComponent<TransparentButtonHighlighter>();
                x.gameObject.AddComponent<PointerClickHandler>();
                x.SetAllCorners(4);
            });

            var iconElement = CreateSubControl(background.transform, Library.FontAwesomeIconSolid);

            var mouseHandler = background.GetComponent<PointerClickHandler>();
            mouseHandler.OnClick = action;


            var text = iconElement.GetComponent<TMP_Text>();
            text.text = icon;
            text.fontSize = size;
            text.color = color;

            positioner((RectTransform)background.transform, CurrentBorders, () => new Vector2(size, size));
        }

        public void IconFontAwesome(string icon, float size, Positioner positioner) {
            IconFontAwesome(icon, size, Theme.Instance.LabelColor, positioner);
        }

        public void IconFontAwesome(string icon, float size, Color color, Positioner positioner) {

            var iconElement = CreateControl(Library.FontAwesomeIconSolid, "51876c2f-2019-448f-be77-2b1f3e199498", null);

            iconElement.text = icon;
            iconElement.fontSize = size;
            iconElement.color = color;

            positioner((RectTransform)iconElement.transform, CurrentBorders, () => new Vector2(size, size));
        }



        public float Slider(float value, Positioner? positioner = null, Color? backgroundColor = null, Color? handleColor = null) {
            if (positioner == null)
                positioner = DefaultControlPositioner;

            var background = CreateControl(Library.Rectangle, "c4b537e2-a35a-4ab1-9f57-d253dda79a36", x=> {
                x.gameObject.AddComponent<Gears.Slider>();
            });
            var backgroundRectTransform = (RectTransform)background.transform;
            positioner(backgroundRectTransform, CurrentBorders, () => new Vector2(80, Theme.Instance.LineHeight));
            background.color = backgroundColor.GetValueOrDefault(new Color(0, 0, 0, 0));

            BeginControls(backgroundRectTransform);
            var handle = CreateControl(Library.Rectangle, "04d4bac2-0843-49f8-a29e-7ae2f65d15c5", x=> {
                x.raycastTarget = false;
            });
            handle.color = handleColor.GetValueOrDefault(Color.white);

            var slider = background.GetComponent<Gears.Slider>();

            if (slider.NewUserInput) {
                value = slider.Value.x;
            }
            value = Mathf.Clamp01(value);

            if (!slider.Moving) {
                slider.Value = new Vector2(value,0);
            }

            var handleRectTransform = (RectTransform)handle.transform;
            var handleWidth = 2;
            GapLeft(-handleWidth* value, value);
            PushLeft(handleWidth)(handleRectTransform, CurrentBorders, () => new Vector2(10, 10));

            EndControls();

            

            return value;
        }


        public static double DoubleFromString(string value) {
            if (!double.TryParse(value, out var result))
                result = default;
            return result;
        }

        public static int IntFromString(string value) {
            if (!int.TryParse(value, out var result))
                result = default;
            return result;
        }

        public static T ConvertFromString<T>(string value) {
            if (typeof(T) == typeof(string)) return (T)(object)value;
            if (string.IsNullOrEmpty(value)) return default!;

            if (typeof(T) == typeof(int)) return (T)(object)int.Parse(value);
            if (typeof(T) == typeof(double)) return (T)(object)double.Parse(value);
            if (typeof(T) == typeof(float)) return (T)(object)float.Parse(value);

            throw new InvalidOperationException($"Unsupported type: {typeof(T)}");
        }

        public T InputField<T>(T value, Positioner positioner, Func<string, T>? fromString = null) {

            var input = CreateControl(Library.InputField, "4f2c03fa-3d39-48ea-b5df-ad785f0c7844");
            var transform = input.GetComponent<RectTransform>();

            positioner(transform, CurrentBorders, () => {
                var textSize = input.GetComponent<TMP_InputField>().textComponent.GetPreferredValues("Hello");
                return textSize + new Vector2(16, 4);
            });


            if (input.NewUserInput) {
                try {
                    if (fromString == null) {
                        return ConvertFromString<T>(input.Value);
                    } else {
                        return fromString(input.Value);
                    }
                }
                catch { }
            }

            var selected = EventSystem.current.currentSelectedGameObject == input.gameObject;
            var editing = input.GetComponent<TMP_InputField>().isFocused && selected;

            if (!editing) {
                input.Value = value?.ToString()??"";
            }

            return value;
        }

        public static Positioner DefaultControlPositioner = PushUp(20);

        public T LabeledInputField<T>(string label, T value, Positioner? positioner = null, Func<string, T>? fromString = null) where T : notnull {
            using (Labeled(label, positioner)) {
                return InputField(value, Fill, fromString);
            }
        }

        private Disposable Labeled(string label, Positioner? positioner = null) {

            if (positioner == null)
                positioner = DefaultControlPositioner;

            Disposable result = Group(positioner);
            Label(label, PushLeft(0, 0.5f));
            return result;
        }

        public T LabeledDropdown<T>(string label, T value, Positioner? positioner = null) where T : struct {
            using (Labeled(label, positioner)) {
                return Dropdown(value, Fill);
            }
        }
        public int LabeledDropdown(string label, int value, string[] options, Positioner? positioner = null) {
            using (Labeled(label, positioner)) {
                return Dropdown(value, options, Fill);
            }
        }

        public T Dropdown<T>(T value, Positioner? positioner = null) where T : struct {
            int intValue = Convert.ToInt32(value);
            string[] options = Enum.GetNames(typeof(T));
            int selectedValue = Dropdown(intValue, options, positioner);
            return (T)Enum.ToObject(typeof(T), selectedValue);
        }

        public int Dropdown(int value, string[] options, Positioner? positioner = null) {
            if (positioner == null)
                positioner = DefaultControlPositioner;

            var input = CreateControl(Library.Dropdown, "a9604168-e35a-4b06-90cf-0ed642a3517e");


            input.options = options.Select(x => new TMP_Dropdown.OptionData(x)).ToList();

            var transform = input.GetComponent<RectTransform>();

            positioner(transform, CurrentBorders, () => new Vector2(40, 20));


            var state = input.GetComponent<DropdownState>();
            if (state.NewUserInput) {
                return state.Value;
            } else {
                var editing = input.IsExpanded;
                if (!editing) {
                    state.Value = value;
                }
            }


            return value;
        }






        /*public Disposable Group(GameObject prefab) {
            var group = CreateControl(prefab);        
            BeginControls(group.transform);
            return new Disposable(()=> {
                EndControls();
            });
        }*/
        /*public Disposable Group<T>(T prefab, Positioner positioner) where T : Component {
            var group = CreateControl(prefab);
            var newRoot = group.transform;
            var redirect = group.GetComponent<ChildrenRedirect>();
            if (redirect)
                newRoot = redirect.ChildrenContainer;

            BeginControls(newRoot);
            return new Disposable(() => {
                EndControls();
                var stackItem = Stack.Peek();
                positioner(group.GetComponent<RectTransform>(), stackItem.Borders);
            });
        }*/


        private static RectTransform GetChildrenContainer(RectTransform rectTransform) {
            var redirect = rectTransform.GetComponent<ChildrenRedirect>();
            if (redirect)
                return redirect.ChildrenContainer;
            return rectTransform;
        }

        public Disposable Group(Positioner positioner, Action<RectTransform>? initializer = null, string? guid = null, Action<RectTransform>? updater = null) {
            CheckInitializerGuid(initializer, guid);
            var group = CreateControl(Library.Empty, guid ?? "92e9c94f-a9ee-4cf6-bb09-9730712d3f47", initializer);
            updater?.Invoke(group);
            BeginControls(GetChildrenContainer(group));
            return new Disposable(() => {
                var innerBorders = CurrentBorders;
                EndControls();
                positioner(group, CurrentBorders, innerBorders.GetRigidSize);
            });
        }


        public Disposable GroupBackground(Positioner positioner, Color? color = null, float radius = 0, Action<RoundedRectangle>? initializer = null, string? guid = null, Action<RoundedRectangle>? updater = null) {
            CheckInitializerGuid(initializer, guid);
            var group = CreateControl(Library.Rectangle, guid ?? "96a0ce8e-c73f-405b-8dc9-274581969441", initializer);
            group.color = color.GetValueOrDefault(Theme.Instance.WindowBackgroundColor);
            group.SetAllCorners(radius);
            var groupRectTransform = (RectTransform)group.transform;
            updater?.Invoke(group);
            BeginControls(GetChildrenContainer(groupRectTransform));
            return new Disposable(() => {
                var innerBorders = CurrentBorders;
                EndControls();
                positioner(groupRectTransform, CurrentBorders, innerBorders.GetRigidSize);
            });
        }


        public Disposable DraggableGroup(Func<object> dragFunc, Positioner positioner) {
            return Group(positioner, x => {
                x.gameObject.AddComponent<Draggable>();
            }, "0ec1ee98-bd5e-4aa9-b7d6-6d224b3cd58a",
            x => {
                x.GetComponent<Draggable>().DragFunc = dragFunc;
            });
        }



        public void ShrinkContainer(bool x, bool y) {
            var root = CurrentRoot;
            var size = root.sizeDelta;
            var collapsedSize = CurrentBorders.GetRigidSize();
            if (x) size.x = collapsedSize.x;
            if (y) size.y = collapsedSize.y;
            root.sizeDelta = size;
        }


        public Disposable ScrollRectVertical(Positioner positioner, Action<ScrollRect>? initializer = null, string? guid = null) {

            CheckInitializerGuid(initializer, guid);

            var element = CreateControl(Library.ScrollRect, guid ?? "fd4aa876-7c8b-4f41-8708-32c2f8d05184", initializer);
            var rectTransform = (RectTransform)element.transform;
            var content = GetChildrenContainer(rectTransform);
            BeginControls(content);

            return new Disposable(() => {
                var innerBorders = CurrentBorders;
                EndControls();
                var innerSize = innerBorders.GetRigidSize();
                content.sizeDelta = new Vector2(0, innerSize.y);

                positioner(rectTransform, CurrentBorders, () => innerSize);
            });
        }

        public bool LabeledCheckbox(string label, bool value, Positioner? positioner = null) {
            using (Labeled(label, positioner)) {
                return Checkbox(value, PushLeft());
            }
        }
        public bool Checkbox(bool value, Positioner? positioner = null) {
            if (positioner == null)
                positioner = DefaultControlPositioner;


            var background = CreateControl(Library.Rectangle, "73c31c6e-5828-49f8-bbc7-c962726c8ede", x => {
                x.gameObject.AddComponent<ButtonHighlighter>();
                x.gameObject.AddComponent<BoolUserInputState>();
                x.gameObject.AddComponent<PointerClickHandler>();
                x.SetAllCorners(4);
            });
            var backgroundRectTransform = (RectTransform)background.transform;

            var clickHandler = background.GetComponent<PointerClickHandler>();
            var userInputState = background.GetComponent<BoolUserInputState>();
            clickHandler.OnClick = () => {
                userInputState.UserInput(!userInputState.Value);
            };

            bool result;
            if (!userInputState.NewUserInput) {
                userInputState.Value = value;
                result = value;
            } else {
                result = userInputState.Value;
            }

            BeginControls(backgroundRectTransform);

            if (result) {
                IconFontAwesome("\uf00c", Theme.Instance.LineHeight * 0.8f, Fill);
            }

            EndControls();

            positioner(backgroundRectTransform, CurrentBorders, () => new Vector2(Theme.Instance.LineHeight, Theme.Instance.LineHeight));

            return result;
        }


        public bool ExpandableGroupHeader(string label, Positioner? positioner = null, bool? opened = null) {
            if (positioner == null)
                positioner = DefaultControlPositioner;


            var background = CreateControl(Library.Rectangle, "ac0d2e61-025f-476d-b4f1-09fe4ea85ab3", x => {
                x.gameObject.AddComponent<ButtonHighlighter>();
                x.gameObject.AddComponent<BoolUserInputState>();
                x.gameObject.AddComponent<PointerClickHandler>();
                x.SetAllCorners(4);
            });
            var backgroundRectTransform = (RectTransform)background.transform;

            var clickHandler = background.GetComponent<PointerClickHandler>();
            var userInputState = background.GetComponent<BoolUserInputState>();
            clickHandler.OnClick = () => {
                userInputState.UserInput(!userInputState.Value);
            };


            bool result;
            if (!userInputState.NewUserInput && opened.HasValue) {
                result = opened.Value;
            } else {
                result = userInputState.Value;
            }

            BeginControls(backgroundRectTransform);

            var caretRight = "\uf0da";
            var caretDown = "\uf0d7";

            IconFontAwesome(result ? caretDown : caretRight, Theme.Instance.LineHeight * 0.8f, PushLeft(Theme.Instance.LineHeight));

            var labelElement = CreateControl(Library.Label, "", null);
            labelElement.text = label;
            labelElement.color = Theme.Instance.LabelColor;
            var labelWidth = labelElement.GetPreferredValues().x;
            Fill(labelElement.GetComponent<RectTransform>(), CurrentBorders, null);

            var innerWidth = CurrentBorders.GetRigidSize().x + labelWidth + 4;

            EndControls();

            positioner(backgroundRectTransform, CurrentBorders, () => new Vector2(innerWidth, Theme.Instance.LineHeight));

            return result;
        }


        public void Padding(float padding) {
            Padding(padding, padding, padding, padding);
        }
        public void Padding(float left, float right, float top, float bottom) {
            var stackItem = Stack.Peek();

            stackItem.Borders.Left.Increment(left, 0);
            stackItem.Borders.Right.Increment(right, 0);
            stackItem.Borders.Top.Increment(top, 0);
            stackItem.Borders.Bottom.Increment(bottom, 0);
        }


        /*public Disposable DraggableVerticalPanel(Func<object> dragFunc) {
            var control = CreateControl(Library.DraggableVerticalPanel);
            var newRoot = control.transform;
            var redirect = control.GetComponent<ChildrenRedirect>();
            if (redirect)
                newRoot = redirect.ChildrenContainer;

            control.Draggable.DragFunc = dragFunc;
            BeginControls(newRoot);
            return new Disposable(() => {
                EndControls();
            });
        }

        public Disposable ExpandableVerticalPanel(string label) {
            var control = CreateControl(Library.ExpandableVerticalPanel);
            var newRoot = control.transform;
            var redirect = control.GetComponent<ChildrenRedirect>();
            if (redirect)
                newRoot = redirect.ChildrenContainer;

            var labelRef = control.GetComponent<LabelRef>();
            labelRef.Value.text = label;

            BeginControls(newRoot);
            return new Disposable(() => {
                EndControls();
            });
        }

        public void ListDropArea<T>(Action<T> dropAction) {
            var control = CreateControl(Library.ListDropArea);
            control.AcceptPredicate = x => x is T;
            control.DropAction = x => dropAction((T)x);
        }

        public void ListDropArea<T0,T1>(Action<T0> dropAction0, Action<T1> dropAction1) {
            var control = CreateControl(Library.ListDropArea);
            control.AcceptPredicate = x => (x is T0 or T1);
            control.DropAction = x => {
                switch (x){
                    case T0 t: dropAction0(t);
                        break;
                    case T1 t: dropAction1(t);
                        break;
                    default: throw new Exception();
                }
            };
        }

        public void Button(string label, Action onClick, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0) {
            var control = CreateControl(Library.Button, filePath, lineNumber);
            control.GetComponent<LabelRef>().Value.text = label;
            control.GetComponent<ActionRef>().Action = onClick;
        }*/

        /*public void AddDropArea() {
            var targetIndex = Stack.Peek().FirstNotValidatedControlIndex - 1;
            if (targetIndex == -1) {

            } else { 

            }
        }*/


    }
}