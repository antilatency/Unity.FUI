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

    public static class FormExtensions { 
        public static T ApplyPositioner<T> (this T _this, Positioner positioner) where T : Form {
            var form = Form.Current;
            positioner(_this.GetComponent<RectTransform>(), form.CurrentBorders, () => _this.RigidSize);
            return _this;
        }
    }

    [RequireComponent(typeof(RectTransform))]
    public abstract class Form : MonoBehaviour {
        [SerializeField]
        private Theme _theme;
        public Theme Theme {
            get {
                if (_theme == null) {
                    _theme = Theme.Default;
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
        public static Form Current = null!;

        [NonSerialized]
        public Vector2 RigidSize = Vector2.zero;

        [NonSerialized]
        private bool _isClean = false;
        public bool Dirty => !_isClean;
        public void MakeDirty() {
            _isClean = false;
        }

        /*public void AssignAndMakeDirty<T>(ref T field, T value) where T : IEquatable<T> {
            if (!field.Equals(value)) {
                field = value;
                MakeDirty();
            }            
        } */
        public void AssignAndMakeDirty<F>(ref F field, F value) {
            if (!EqualityComparer<F>.Default.Equals(field, value)) {
                field = value;
                MakeDirty();
            }
        }
        /*public void AssignClampedAndMakeDirty<F>(ref F field, F value, F min, F max) where F : IComparable<F> {
            var clampedValue = value;
            if (clampedValue.CompareTo(min) < 0)
                clampedValue = min;
            else if (clampedValue.CompareTo(max) > 0)
                clampedValue = max;
            if (!EqualityComparer<F>.Default.Equals(field, clampedValue)) {
                field = clampedValue;
                MakeDirty();
            }
        }*/

        

        public class StackItem {
            public int FirstNotValidatedControlIndex = 0;
            public RectTransform Root;
            public Borders Borders = new();
            public StackItem(RectTransform root) {
                Root = root;

            }
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









        public void RebuildIfNeeded() {
            if (Lazy && _isClean)
                return;

            Rebuild();

            /*var i = MaxIterationsPerUpdate;
            do {
                Rebuild();
                i--;
                if (i <= 0) {
                    return;
                }
            } while (Dirty > 0);*/

            /*if (!Lazy || UpdateIterationsRequired>0)
                 Rebuild();*/
        }

        public void Rebuild() {
            var prewCurrent = Current;
            Current = this;
            _isClean = true;
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

        

        

        
    }
}