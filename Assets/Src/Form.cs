using FUI.Gears;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace FUI {


    


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

    


    public abstract class Form : MonoBehaviour {
        public bool Lazy = false;

        public static Form Current = null!;

        [NonSerialized]
        private bool UpToDate = false;
        public void MakeDirty() {
            UpToDate = false;
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



        public RectTransform Element(GameObject? original = null, params Modifier[] modifiers) {
            var stackItem = Stack.Peek();
            var parent = stackItem.Root;
            var index = stackItem.FirstNotValidatedControlIndex;

            var mark = string.Join('~', modifiers.Select(x => x.Id));
            if (original != null) {
                mark = original.GetInstanceID().ToString() + mark;
            }


            int FindIndex() {
                for (int i = stackItem.FirstNotValidatedControlIndex; i < parent.childCount; i++) {
                    var child = parent.GetChild(i);
                    if (child.GetComponent<Mark>()?.Value == mark) {
                        return i;
                    }
                }
                return -1;
            }

            var indexFound = FindIndex();
            GameObject gameObject;
            RectTransform result;

            if (indexFound > -1) {
                var numElementsToDelete = indexFound - stackItem.FirstNotValidatedControlIndex;
                for (int i = 0; i < numElementsToDelete; i++) {
                    var child = parent.GetChild(stackItem.FirstNotValidatedControlIndex);
                    DestroyImmediate(child.gameObject);
                }
                result = (RectTransform)parent.GetChild(stackItem.FirstNotValidatedControlIndex);

                gameObject = result.gameObject;

                stackItem.FirstNotValidatedControlIndex++;


            } else {
                if (original != null) {
                    gameObject = InstantiatePrefab(original, stackItem.Root);
                } else {
                    gameObject = new GameObject("fuiElement", typeof(RectTransform));
                    gameObject.transform.SetParent(stackItem.Root);
                    
                }
                result = (RectTransform)gameObject.transform;

                foreach (var m in modifiers) {
                    m.Creator?.Invoke(gameObject);
                }

                gameObject.AddComponent<Mark>().Value = mark;
                result.SetSiblingIndex(index);


                stackItem.FirstNotValidatedControlIndex++;

            }

            foreach (var m in modifiers) {
                if (m.Updater!=null)
                    m.Updater(gameObject);
            }

            //

            return result;

        }

        public RectTransform Element(Positioner positioner, GameObject original = null, params Modifier[] modifiers) {
            var result = Element(null, modifiers);
            positioner?.Invoke(result, Form.Current.CurrentBorders, null);
            return result;
        }


        public Disposable Group(Positioner positioner, params Modifier[] modifiers) {
            var group = Element(null, modifiers);
            BeginControls(group);
            return new Disposable(() => {
                var innerBorders = CurrentBorders;
                EndControls();
                positioner(group, CurrentBorders, innerBorders.GetRigidSize);
            });
        }


        public void LabelModifiable(Positioner positioner, params Modifier[] modifiers) {
            var result = Element(Form.Current.Library.Label, modifiers);
            positioner?.Invoke(result, Form.Current.CurrentBorders, () => {
                var size = result.GetComponent<TMP_Text>().GetPreferredValues();
                size.x = Mathf.Ceil(size.x);
                size.y = Mathf.Ceil(size.y);
                return size;
            });
        }


        public void RebuildIfNeeded() {
            if (!Lazy || !UpToDate)
                Rebuild();
        }

        public void Rebuild() {
            Current = this;
            BeginControls();
            try {
                Build();
            }
            finally {
                EndControls();
                Current = null;
                
            }
            UpToDate = true;
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

        public void EndControls() {
            var stackItem = Stack.Pop();
            var t = stackItem.Root;
            var index = stackItem.FirstNotValidatedControlIndex;

            var count = t.childCount - index;
            while (count > 0) {
                var child = t.GetChild(index);
                DestroyImmediate(child.gameObject);
                count--;
            }
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


/*        public T CreateSubControl<T>(Transform parent, T original, Action<T>? initializer = null, string guid = "") where T : UnityEngine.Object {
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

        }*/


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

            var transform = Element(Library.InputField.gameObject
                , M.SetFormToNotify()
                );
            var input = transform.GetComponent<InputFieldState>();

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

        public static Positioner DefaultControlPositioner => P.Up(Theme.Instance.LineHeight);

        

        public T Dropdown<T>(T value, Positioner? positioner = null) where T : struct {
            int intValue = Convert.ToInt32(value);
            string[] options = Enum.GetNames(typeof(T));
            int selectedValue = Dropdown(intValue, options, positioner);
            return (T)Enum.ToObject(typeof(T), selectedValue);
        }

        public int Dropdown(int value, string[] options, Positioner? positioner = null) {
            if (positioner == null)
                positioner = DefaultControlPositioner;

            var element = Element(Library.Dropdown
                , M.SetFormToNotify()
                );
            positioner(element, CurrentBorders, () => new Vector2(40, Theme.Instance.LineHeight));


            var dropdown = element.GetComponent<TMP_Dropdown>();

            dropdown.options = options.Select(x => new TMP_Dropdown.OptionData(x)).ToList();

            var state = dropdown.GetComponent<DropdownState>();
            if (state.NewUserInput) {
                return state.Value;
            } else {
                var editing = dropdown.IsExpanded;
                if (!editing) {
                    state.Value = value;
                }
            }
            return value;
        }



        



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
                var innerBorders = CurrentBorders;
                EndControls();
                var innerSize = innerBorders.GetRigidSize();
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