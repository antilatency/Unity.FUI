using FUI.Gears;
using FUI.Modifiers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#nullable enable

namespace FUI {

    public delegate void ButtonAction(GameObject gameObject, PointerEventData eventData);

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

    

    [RequireComponent(typeof(RectTransform))]
    public abstract class Form : MonoBehaviour {
        [SerializeField]
        private Theme _theme;
        public Theme Theme {
            get {
                if (_theme == null) {
                    _theme = Resources.Load<Theme>("FUI.DefaultTheme");
                    if (_theme == null)
                        throw new Exception("Default theme (FUI.DefaultTheme) not found in resources.");
                }

                return _theme;
            }
            set {
                _theme = value;
                MakeDirty();
            }
        }

        public void ThemeChanged(Theme theme) {
            if (theme == _theme)
                MakeDirty();
        }

        public bool Lazy = true;
        public int MaxIterationsPerUpdate = 8;
        public static Form Current = null!;

        [NonSerialized]
        public Vector2 RigidSize = Vector2.zero;

        [NonSerialized]
        public int UpdateIterationsRequired = 1;
        public void MakeDirty(bool extraIteration = false) {
            UpdateIterationsRequired = Math.Max(UpdateIterationsRequired, extraIteration ? 2 : 1);
        }

        /*public void AssignAndMakeDirty<T>(ref T field, T value) where T : IEquatable<T> {
            if (!field.Equals(value)) {
                field = value;
                MakeDirty();
            }            
        } */
        public void AssignAndMakeDirty<T>(ref T field, T value) {
            if (!EqualityComparer<T>.Default.Equals(field, value)) {
                field = value;
                MakeDirty();
            }
        }
        public void AssignClampedAndMakeDirty<T>(ref T field, T value, T min, T max) where T : IComparable<T> {
            var clampedValue = value;
            if (clampedValue.CompareTo(min) < 0)
                clampedValue = min;
            else if (clampedValue.CompareTo(max) > 0)
                clampedValue = max;
            if (!EqualityComparer<T>.Default.Equals(field, clampedValue)) {
                field = clampedValue;
                MakeDirty();
            }
        }

        public PrefabLibrary library = null!;
        public PrefabLibrary Library {
            get {
                if (library == null)
                    library = Resources.Load<PrefabLibrary>("FUI.PrefabLibrary");
                return library;
            }
        }

        public class StackItem {
            public int FirstNotValidatedControlIndex = 0;
            public RectTransform Root;
            public Borders Borders = new();
            public StackItem(RectTransform root) {
                Root = root;

            }
        }

        public class ElementSeed {
            public GameObject? Original;
            public ModifiersList? Modifiers { get; }
            public string Identifier;

            public GameObject? Found = null;
            public GameObject? Created = null;

            public ElementSeed(GameObject? original, ModifiersList? modifiers = null) {
                Original = original;
                Modifiers = modifiers ?? new ModifiersList();
                Identifier = string.Join('~', Modifiers.Select(x => x.Id));
                if (original != null) {
                    Identifier = original.GetInstanceID().ToString() + Identifier;
                }
            }

            public void ApplyModifiers() {
                if (Modifiers == null) return;
                if (Created != null) {
                    foreach (var m in Modifiers) {
                        m.Create(Created);
                    }
                } else if (Found != null) {
                    foreach (var m in Modifiers) {
                        m.Update(Found);
                    }
                }
            }
            
        }

        public ElementSeed BeginCreateElement(GameObject? original = null, ModifiersList? modifiers = null) { 
            var stackItem = Stack.Peek();
            var parent = stackItem.Root;
            ElementSeed seed = new ElementSeed(original, modifiers);

            int indexFound = -1;
            Transform? transformFound = null;
            for (int i = stackItem.FirstNotValidatedControlIndex; i < parent.childCount; i++) {
                var child = parent.GetChild(i);
                if (child.name == seed.Identifier) {
                    indexFound = i;
                    transformFound = child;
                    break;
                }
            }

            //var index = stackItem.FirstNotValidatedControlIndex;

            if (indexFound != -1) {
                var numElementsToDelete = indexFound - stackItem.FirstNotValidatedControlIndex;
                for (int i = 0; i < numElementsToDelete; i++) {
                    var child = parent.GetChild(stackItem.FirstNotValidatedControlIndex);
                    DestroyImmediate(child.gameObject);
                }
                seed.Found = transformFound!.gameObject;

                if (transformFound is not RectTransform)
                    seed.Found.AddComponent<RectTransform>();

                stackItem.FirstNotValidatedControlIndex++;
            }
            else {
                if (original != null) {
                    seed.Created = InstantiatePrefab(original, stackItem.Root);
                    seed.Created.name = seed.Identifier;
                }
                else {
                    seed.Created = new GameObject(seed.Identifier, typeof(RectTransform));
                    seed.Created.transform.SetParent(parent, false);

                }


                /*foreach (var m in modifiers) {
                    m.Creator?.Invoke(gameObject);
                }*/

                //gameObject.AddComponent<Mark>().Identifier = markIdentifier;
                seed.Created.transform.SetSiblingIndex(stackItem.FirstNotValidatedControlIndex);


                stackItem.FirstNotValidatedControlIndex++;

            }

            return seed;
        }

        public RectTransform Element(GameObject? original = null, params Modifier[] modifiers) {
            return Element(original, new ModifiersList(modifiers));
        }

        public RectTransform Element(GameObject? original = null, ModifiersList? modifiers = null) {
            var seed = BeginCreateElement(original, modifiers ?? new ModifiersList());
            seed.ApplyModifiers();
            return (RectTransform)(seed.Created ?? seed.Found)!.transform;
        }
        
        public RectTransform Element(Positioner positioner, GameObject? original = null, params Modifier[] modifiers) {
            var result = Element(original, modifiers);
            positioner?.Invoke(result, Form.Current.CurrentBorders, null);
            return result;
        }


        /*public RectTransform Element(GameObject? original = null, params Modifier[] modifiers) {
            return Element(original, (IEnumerable<Modifier>)modifiers);
        }

        public RectTransform Element(GameObject? original, IEnumerable<Modifier> modifiers) {
            var stackItem = Stack.Peek();
            var parent = stackItem.Root;
            var index = stackItem.FirstNotValidatedControlIndex;

            var markIdentifier = string.Join('~', modifiers.Select(x => x.Id));
            if (original != null) {
                markIdentifier = original.GetInstanceID().ToString() + markIdentifier;
            }


            (int index, Mark? mark) FindIndex() {
                for (int i = stackItem.FirstNotValidatedControlIndex; i < parent.childCount; i++) {
                    var child = parent.GetChild(i);
                    var mark = child.GetComponent<Mark>();
                    if (mark?.Identifier == markIdentifier) {
                        return (i, mark);
                    }
                }
                return (-1, null);
            }

            var (indexFound, mark) = FindIndex();
            GameObject gameObject;
            RectTransform result;

            if (mark != null) {
                var numElementsToDelete = indexFound - stackItem.FirstNotValidatedControlIndex;
                for (int i = 0; i < numElementsToDelete; i++) {
                    var child = parent.GetChild(stackItem.FirstNotValidatedControlIndex);
                    DestroyImmediate(child.gameObject);
                }
                result = (mark.transform as RectTransform)!;
                gameObject = result.gameObject;

                stackItem.FirstNotValidatedControlIndex++;


            } else {
                if (original != null) {
                    gameObject = InstantiatePrefab(original, stackItem.Root);
                } else {
                    gameObject = new GameObject("fuiElement", typeof(RectTransform));
                    gameObject.transform.SetParent(stackItem.Root, false);

                }
                result = (RectTransform)gameObject.transform;

                foreach (var m in modifiers) {
                    m.Creator?.Invoke(gameObject);
                }

                gameObject.AddComponent<Mark>().Identifier = markIdentifier;
                result.SetSiblingIndex(index);


                stackItem.FirstNotValidatedControlIndex++;

            }

            foreach (var m in modifiers) {
                if (m.Updater != null)
                    m.Updater(gameObject);
            }

            //

            return result;

        }*/




        public Disposable<RectTransform> Group(Positioner positioner, params Modifier[] modifiers) {
            var group = Element(null, modifiers);
            BeginControls(group);
            return new Disposable<RectTransform>(group, _ => {
                var innerSize = EndControls();
                positioner(group, CurrentBorders, () => innerSize);
            });
        }


        

        public void RebuildIfNeeded() {
            if (Lazy && UpdateIterationsRequired == 0)
                return;

            var i = MaxIterationsPerUpdate;
            do {
                Rebuild();
                i--;
                if (i <= 0) {
                    return;
                }
            } while (UpdateIterationsRequired > 0);

           /*if (!Lazy || UpdateIterationsRequired>0)
                Rebuild();*/
        }

        public void Rebuild() {
            var prewCurrent = Current;
            Current = this;
            UpdateIterationsRequired = Math.Max(UpdateIterationsRequired - 1, 0);
            BeginControls();
            try {
                Build();
            }
            finally {
                RigidSize = EndControls();
                Current = prewCurrent;                
            }
            
            if (Stack.Count != 0)
                throw new InvalidOperationException("The stack is not empty after control operations.");
        }

        protected abstract void Build();


        protected virtual void Update() {
            RebuildIfNeeded();
        }


        public Stack<StackItem> Stack = new();
        public RectTransform CurrentRoot => Stack.Peek().Root;
        public Borders CurrentBorders => Stack.Peek().Borders;


        public void BeginControls() {
            BeginControls((RectTransform)transform);
        }
        public void BeginControls(RectTransform newRoot) {
            var newItem = new StackItem(newRoot);
            Stack.Push(newItem);
        }

        public Vector2 EndControls() {
            var stackItem = Stack.Pop();
            var t = stackItem.Root;
            var index = stackItem.FirstNotValidatedControlIndex;

            var count = t.childCount - index;
            while (count > 0) {
                var child = t.GetChild(index);
                DestroyImmediate(child.gameObject);
                count--;
            }
            return stackItem.Borders.GetRigidSize();
        }



        private static GameObject InstantiatePrefab(GameObject original, Transform parent) {
#if UNITY_EDITOR
            GameObject created = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(original, parent);
#else
            GameObject created = (GameObject)Instantiate(original, parent);
#endif
            if (created == null)
                throw new InvalidOperationException($"The created control is null.");

            return created;
        }

        /*public T ClampMakeDirty<T>(T value, T min, T max) where T : IComparable<T> {
            if (value.CompareTo(min) < 0)
            {
                MakeDirty();
                return min;
            }
            if (value.CompareTo(max) > 0)
            {
                MakeDirty();
                return max;
            }
            return value;
        }*/

        /*public float ClampMakeDirty(float value, float min, float max) {
            if (value < min) {
                MakeDirty();
                return min;
            }
            if (value > max) {
                MakeDirty();
                return max;
            }
            return value;
        }

        public double ClampMakeDirty(double value, double min, double max) {
            if (value < min) {
                MakeDirty();
                return min;
            }
            if (value > max) {
                MakeDirty();
                return max;
            }
            return value;
        }*/

        public void GapLeft(float pixels, float fraction = 0) {
            CurrentBorders.Left.Increment(pixels, fraction);
        }
        public void GapLeft() => GapLeft(Theme.DefaultGap);

        public void GapRight(float pixels, float fraction = 0) {
            CurrentBorders.Right.Increment(pixels, fraction);
        }
        public void GapRight() => GapRight(Theme.DefaultGap);

        public void GapTop(float pixels, float fraction = 0) {
            CurrentBorders.Top.Increment(pixels, fraction);
        }
        public void GapTop() => GapTop(Theme.DefaultGap);

        public void GapBottom(float pixels, float fraction = 0) {
            CurrentBorders.Bottom.Increment(pixels, fraction);
        }
        public void GapBottom() => GapBottom(Theme.DefaultGap);
       


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


        /*public T InputField<T>(T value, Positioner positioner, string toStringFormat = "", Func<string, T>? fromString = null, bool extraIteration = false) {

            var transform = Element(Library.InputField.gameObject);
            
            var input = transform.GetComponent<InputFieldState>();
            input.SetFormToNotify(Form.Current, extraIteration);

            positioner(transform, CurrentBorders, () => {
                var textSize = input.GetComponent<FUI_InputField>().textComponent.GetPreferredValues("Hello");
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
            var editing = input.GetComponent<FUI_InputField>().isFocused && selected;

            if (!editing) {
                if (value is IFormattable formattable) {
                    input.Value = formattable.ToString(toStringFormat, CultureInfo.CurrentCulture);
                } else {
                    input.Value = value?.ToString();
                }

                
            }

            return value;
        }*/

        public RectTransform InputField<T>(T value, Action<T> returnAction, Positioner positioner, string toStringFormat = "", Func<string, T>? fromString = null) {
            string valueText;
            if (value is IFormattable formattable) {
                valueText = formattable.ToString(toStringFormat, CultureInfo.CurrentCulture);
            }
            else {
                valueText = value?.ToString() ?? "";
            }
            var transform = InputField(valueText, (s) => {
                try {
                    if (fromString == null) {
                        returnAction(ConvertFromString<T>(s));
                    }
                    else {
                        returnAction(fromString(s));
                    }
                }
                catch { }
            }, positioner);
            return transform;
        }


        public RectTransform InputField(string value, FUI_InputField.TextDelegate returnAction, Positioner? positioner = null) {

            var transform = Element(Library.InputField, new AddInputFieldHighlighter());



            var inputField = transform.GetComponent<FUI_InputField>();
            inputField.onEndEdit = returnAction;
            //inputField.onSubmit = returnAction;
            inputField.onValueChanged = returnAction;

            (positioner ?? DefaultControlPositioner)(transform, CurrentBorders, () => {
                var textSize = inputField.textComponent.GetPreferredValues("Hello");
                return textSize + new Vector2(16, 4);
            });

            /*if (input.NewUserInput) {
                try {
                    if (fromString == null) {
                        return ConvertFromString<T>(input.Value);
                    } else {
                        return fromString(input.Value);
                    }
                }
                catch { }
            }*/

            var selected = EventSystem.current.currentSelectedGameObject == transform.gameObject;
            var editing = inputField.isFocused && selected;

            if (!editing) {
                inputField.SetTextWithoutNotify(value);
                /*if (value is IFormattable formattable) {
                    input.Value = formattable.ToString(toStringFormat, CultureInfo.CurrentCulture);
                } else {
                    input.Value = value?.ToString();
                } */
            }

            return transform;
        }

        public Positioner DefaultControlPositioner => P.Up(Theme.LineHeight);

 

        



        



        public void ShrinkContainer(bool x, bool y) {
            var root = CurrentRoot;
            var size = root.sizeDelta;
            var collapsedSize = CurrentBorders.GetRigidSize();
            if (x) size.x = collapsedSize.x;
            if (y) size.y = collapsedSize.y;
            root.sizeDelta = size;
        }

        private static RectTransform GetChildrenContainer(RectTransform rectTransform) {
            var redirect = rectTransform.GetComponent<ChildrenRedirect>();
            if (redirect)
                return redirect.ChildrenContainer;
            return rectTransform;
        }

        public Disposable ScrollRectVertical(Positioner positioner) {
            var element = Element(Library.ScrollRect);

            var content = GetChildrenContainer(element);
            BeginControls(content);

            return new Disposable(() => {
                var innerSize = EndControls();
                content.sizeDelta = new Vector2(0, innerSize.y);
                positioner(element, CurrentBorders, () => innerSize);
            });
        }

        public void Padding(float padding) {
            Padding(padding, padding, padding, padding);
        }
        public void Padding(float left, float right, float top, float bottom) {
            var borders = CurrentBorders;

            borders.Left.Increment(left, 0);
            borders.Right.Increment(right, 0);
            borders.Top.Increment(top, 0);
            borders.Bottom.Increment(bottom, 0);
        }


        public Disposable TempPaddingHorizontal(float left, float right) {
            var borders = CurrentBorders;

            borders.Left.Increment(left, 0);
            borders.Right.Increment(right, 0);

            return new Disposable(() => {
                borders.Left.Increment(-left, 0);
                borders.Right.Increment(-right, 0);
            });
        }
        public Disposable TempPaddingVertical(float top, float bottom) {
            var borders = CurrentBorders;

            borders.Top.Increment(top, 0);
            borders.Bottom.Increment(bottom, 0);

            return new Disposable(() => {
                borders.Top.Increment(-top, 0);
                borders.Bottom.Increment(-bottom, 0);
            });
        }

        
    }
}