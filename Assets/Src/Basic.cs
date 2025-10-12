#nullable enable
using System;
using System.Globalization;
using System.Linq;

using FUI.Gears;
using FUI.Modifiers;

using UnityEngine;
using UnityEngine.EventSystems;

using static FUI.Helpers;

namespace FUI {

    public static partial class Basic {
        
        public static Positioner DefaultControlPositioner => P.Up(Form.Current.Theme.LineHeight);

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
                }
                else if (Found != null) {
                    foreach (var m in Modifiers) {
                        m.Update(Found);
                    }
                }
            }

        }

        public static ElementSeed BeginCreateElement(GameObject? original = null, ModifiersList? modifiers = null) {
            var form = Form.Current;
            var stackItem = form.Stack.Peek();
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
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
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

        public static RectTransform Element(GameObject? original = null, params Modifier[] modifiers) {
            return Element(original, new ModifiersList(modifiers));
        }

        public static RectTransform Element(GameObject? original = null, ModifiersList? modifiers = null) {
            var seed = BeginCreateElement(original, modifiers ?? new ModifiersList());
            seed.ApplyModifiers();
            return (RectTransform)(seed.Created ?? seed.Found)!.transform;
        }

        public static RectTransform Element(Positioner positioner, GameObject? original = null, params Modifier[] modifiers) {
            var result = Element(original, modifiers);
            positioner?.Invoke(result, Form.Current.CurrentBorders, null);
            return result;
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

        public static void GapLeft(float pixels, float fraction = 0) {
            var form = Form.Current;
            form.CurrentBorders.Left.Increment(pixels, fraction);
        }
        public static void GapLeft() => GapLeft(Form.Current.Theme.DefaultGap);

        public static void GapRight(float pixels, float fraction = 0) {
            var form = Form.Current;
            form.CurrentBorders.Right.Increment(pixels, fraction);
        }
        public static void GapRight() => GapRight(Form.Current.Theme.DefaultGap);

        public static void GapTop(float pixels, float fraction = 0) {
            var form = Form.Current;
            form.CurrentBorders.Top.Increment(pixels, fraction);
        }
        public static void GapTop() => GapTop(Form.Current.Theme.DefaultGap);

        public static void GapBottom(float pixels, float fraction = 0) {
            var form = Form.Current;
            form.CurrentBorders.Bottom.Increment(pixels, fraction);
        }
        public static void GapBottom() => GapBottom(Form.Current.Theme.DefaultGap);



        public static void ShrinkContainer(bool x, bool y) {
            var form = Form.Current;
            var root = form.CurrentRoot;
            var size = root.sizeDelta;
            var collapsedSize = form.CurrentBorders.GetRigidSize();
            if (x) size.x = collapsedSize.x;
            if (y) size.y = collapsedSize.y;
            root.sizeDelta = size;
        }

        public static void Padding(float padding) {
            Padding(padding, padding, padding, padding);
        }
        public static void Padding(float left, float right, float top, float bottom) {
            var form = Form.Current;
            var borders = form.CurrentBorders;

            borders.Left.Increment(left, 0);
            borders.Right.Increment(right, 0);
            borders.Top.Increment(top, 0);
            borders.Bottom.Increment(bottom, 0);
        }


        public static Disposable TempPaddingHorizontal(float left, float right) {
            var form = Form.Current;
            var borders = form.CurrentBorders;

            borders.Left.Increment(left, 0);
            borders.Right.Increment(right, 0);

            return new Disposable(() => {
                borders.Left.Increment(-left, 0);
                borders.Right.Increment(-right, 0);
            });
        }
        public static Disposable TempPaddingVertical(float top, float bottom) {
            var form = Form.Current;
            var borders = form.CurrentBorders;

            borders.Top.Increment(top, 0);
            borders.Bottom.Increment(bottom, 0);

            return new Disposable(() => {
                borders.Top.Increment(-top, 0);
                borders.Bottom.Increment(-bottom, 0);
            });
        }

        public static Disposable<RectTransform> Group(Positioner positioner, params Modifier[] modifiers) {
            var form = Form.Current;
            var group = Element(null, modifiers);
            form.BeginControls(group);
            return new Disposable<RectTransform>(group, _ => {
                var innerSize = form.EndControls();
                positioner(group, form.CurrentBorders, () => innerSize);
            });
        }




        public static Disposable ScrollRectVertical(Positioner positioner) {

            RectTransform GetChildrenContainer(RectTransform rectTransform) {
                var redirect = rectTransform.GetComponent<ChildrenRedirect>();
                if (redirect)
                    return redirect.ChildrenContainer;
                return rectTransform;
            }

            var form = Form.Current;
            var element = Element(PrefabLibrary.Instance.ScrollRect);

            var content = GetChildrenContainer(element);
            form.BeginControls(content);

            return new Disposable(() => {
                var innerSize = form.EndControls();
                content.sizeDelta = new Vector2(0, innerSize.y);
                positioner(element, form.CurrentBorders, () => innerSize);
            });
        }



        public static RectTransform InputField<T>(T value, Action<T> returnAction, Positioner positioner, string toStringFormat = "", Func<string, T>? fromString = null) {
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

        public static RectTransform InputField(string value, FUI_InputField.TextDelegate returnAction, Positioner? positioner = null) {

            var transform = Element(PrefabLibrary.Instance.InputField, new AddFocusHoverHighlighter());

            var inputField = transform.GetComponent<FUI_InputField>();
            inputField.onEndEdit = returnAction;
            //inputField.onSubmit = returnAction;
            inputField.onValueChanged = returnAction;

            (positioner ?? DefaultControlPositioner)(transform, Form.Current.CurrentBorders, () => {
                var textSize = inputField.textComponent.GetPreferredValues("Hello");
                return textSize + new Vector2(16, 4);
            });

            var selected = EventSystem.current.currentSelectedGameObject == transform.gameObject;
            var editing = inputField.isFocused && selected;

            if (!editing) {
                inputField.SetTextWithoutNotify(value);
            }

            return transform;
        }


    }

}
