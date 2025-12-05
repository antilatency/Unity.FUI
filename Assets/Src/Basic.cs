#nullable enable
using System;
using System.Globalization;
using System.Linq;

using FUI.Gears;
using FUI.Modifiers;

using TMPro;

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

        public static RectTransform Element(GameObject? original, ModifiersList modifiers) {
            var seed = BeginCreateElement(original, modifiers);
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
            GameObject created = UnityEngine.Object.Instantiate(original, parent);
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
                positioner.Invoke(group, form.CurrentBorders, () => innerSize);
            });
        }

        public static T Dialog<T>() where T : Dialog {
            var dialog = FormStack.Instance.Push<T>();
            return dialog;
        }

        public static RectTransform ScrollBarVertical(Positioner positioner, float minHandleSize = 8) {
            var h = minHandleSize / 2;
            var form = Form.Current;
            var theme = form.Theme;
            using (var scrollBar = Group(positioner
                , new AddComponent<UnityEngine.UI.Scrollbar>()
                )
            ) {
                Padding(0, 0, h, h);
                using (var scrollingArea = Group(P.Fill)) {
                    //Padding(0, 0, -h, -h);
                    var handle = Element(null
                        , new AddComponent<RoundedRectangle>()
                        , new SetColor(theme.ButtonColor)
                        , new SetRectangleCorners(theme.Radius)
                        , new SetRaycastTarget(true)
                    );
                    handle.pivot = Vector2.zero;
                    handle.offsetMin = new Vector2(0, -h);
                    handle.offsetMax = new Vector2(0, h);

                    var scrollBarComponent = scrollBar.Value.GetComponent<UnityEngine.UI.Scrollbar>();
                    scrollBarComponent.handleRect = handle;
                    scrollBarComponent.direction = UnityEngine.UI.Scrollbar.Direction.BottomToTop;
                    scrollBarComponent.transition = UnityEngine.UI.Selectable.Transition.None;
                    //scrollBarComponent.size = 0.5f;
                }
                return scrollBar.Value;
            }
        }

        public static Disposable<RectTransform> ScrollRectVertical(Positioner positioner) {
            var form = Form.Current;
            var theme = form.Theme;
            var scrollRect = Element(null
                , new AddComponent<UnityEngine.UI.ScrollRect>()
                , new AddComponent<RoundedRectangle>()
                , new SetColor(Color.clear)
            );
            var scrollRectComponent = scrollRect.GetComponent<UnityEngine.UI.ScrollRect>();

            form.BeginControls(scrollRect);

            var scrollBar = ScrollBarVertical(P.Right(8));
            scrollRectComponent.verticalScrollbar = scrollBar.GetComponent<UnityEngine.UI.Scrollbar>();
            scrollRectComponent.verticalScrollbarSpacing = 2;

            var viewport = Element(null
                , new AddRectMask()
            );
            viewport.pivot = Vector2.zero;
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            form.BeginControls(viewport);


            var content = Element();
            content.pivot = new Vector2(0, 1);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.sizeDelta = new Vector2(0, 0);


            scrollRectComponent.viewport = viewport;
            scrollRectComponent.content = content;
            scrollRectComponent.horizontal = false;
            scrollRectComponent.vertical = true;
            scrollRectComponent.movementType = UnityEngine.UI.ScrollRect.MovementType.Elastic;
            scrollRectComponent.elasticity = 0.1f;
            scrollRectComponent.scrollSensitivity = 30;
            scrollRectComponent.verticalScrollbarVisibility = UnityEngine.UI.ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRectComponent.inertia = true;
            scrollRectComponent.decelerationRate = 0.135f;

            //scrollRectComponent.verticalScrollbar

            form.BeginControls(content);

            return new Disposable<RectTransform>(scrollRect, _ => {
                ShrinkContainer(false, true);
                var contentSize = form.EndControls();
                var viewportSize = form.EndControls();
                var scrollRectSize = form.EndControls();

                positioner.Invoke(scrollRect, form.CurrentBorders, () => new Vector2(contentSize.x, Math.Min(contentSize.y, form.Theme.LineHeight * 10)));
            });
        }


        /*public static Disposable ScrollRectVertical(Positioner positioner) {

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
        }*/



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


        public static RectTransform TextElement(Positioner? positioner, ModifiersList? additionalModifiers = null) {
            var form = Form.Current;
            var modifiers = new ModifiersList() {
                new AddComponent<TextMeshProUGUI>(),
                additionalModifiers
            };

            var result = Element(null, modifiers);
            (positioner ?? DefaultControlPositioner).Invoke(result, form.CurrentBorders, () => {
                var component = result.GetComponent<TMP_Text>();
                var size = component.GetPreferredValues();
                size.x = Mathf.Ceil(size.x);
                size.y = Mathf.Ceil(size.y);
                return size;
            });
            return result;
        }




        
        /*public static RectTransform InputField(string value, FUI_InputField.TextDelegate returnAction, Positioner? positioner = null) {

            using (var inputField = Group(positioner ?? DefaultControlPositioner
                , new AddComponent<FUI_InputField>()
                , new AddFocusHoverHighlighter()
                
                )) {
                var inputFieldComponent = inputField.Value.GetComponent<FUI_InputField>();
                inputFieldComponent.onEndEdit = returnAction;
                //inputField.onSubmit = returnAction;
                inputFieldComponent.onValueChanged = returnAction;
                Padding(2, 2, 0,0);
                var text = TextElement(P.Fill
                    , new ModifiersList { new SetTextAlignmentLeftMiddle() }
                );
                var textComponent = text.GetComponent<TMP_Text>();
                inputFieldComponent.textComponent = textComponent;

                var selected = EventSystem.current.currentSelectedGameObject == inputField.Value.gameObject;
                var editing = inputFieldComponent.isFocused && selected;

                if (!editing) {
                    inputFieldComponent.SetTextWithoutNotify(value);
                }

                return inputField.Value;
            }
        }*/


    }

}
