using System;

using FUI.Gears;
using FUI.Modifiers;
using static FUI.Basic;

using UnityEngine;
#nullable enable
namespace FUI {


    public abstract class Dialog : Form {

        private RectTransform? _windowTransform = null;

        protected virtual bool CloseOnEscape => true;
        protected virtual bool CloseOnClickOutside => true;

        public static T Create<T>() where T : Dialog {
            var dialog = FormStack.Instance.Push<T>();
            return dialog;
        }

        protected void Close() {
            FormStack.Instance.Pop();
        }

        public Disposable<RectTransform> Glass() {
            var result = Group(P.Fill
                , new AddComponent<RoundedRectangle>()
                , new SetColor(new Color(0, 0, 0, 0))
                , new AddClickHandlerEx((g, e) => {
                    if (CloseOnClickOutside) {
                        Close();
                    }
                }));
            return result;
        }

        protected virtual Disposable<RectTransform> WindowElement() {
            var result = Element();
            BeginControls(result);
            return new Disposable<RectTransform>(result, x => {
                EndControls();
            });
        }

        protected virtual void UpdatePosition(RectTransform window){}

        protected override void Update() {
            try {
                if (CloseOnEscape && Input.GetKeyDown(KeyCode.Escape)) {
                    Close();
                    return;
                }
                base.Update();
                if (_windowTransform != null)
                    UpdatePosition(_windowTransform);
            }
            catch (Exception e) {
                Debug.LogError(e);
                Close();
            }
        }

        protected override sealed void Build() {
            using (Glass()) {
                using (var window = WindowElement()) {
                    _windowTransform = window.Value;
                    Populate();
                }
            }
        }

        protected abstract void Populate();

        protected static void PositionWindowCentered(RectTransform window) {
            var half = new Vector2(0.5f, 0.5f);
            window.pivot = half;
            window.anchoredPosition = Vector2.zero;
            window.anchorMin = half;
            window.anchorMax = half;
        }

        protected static void PositionWindowUnder(RectTransform window, RectTransform control) {
            var canvas = control.GetComponentInParent<Canvas>();
            var canvasTransform = canvas.GetComponent<RectTransform>();

            var dialogSize = window.rect.size;

            var controlCorners = new Vector3[4];
            control.GetWorldCorners(controlCorners);
            for (int i = 0; i < 4; i++) {
                controlCorners[i] = canvasTransform.InverseTransformPoint(controlCorners[i]);
            }
            var canvasCorners = new Vector3[4];
            canvasTransform.GetLocalCorners(canvasCorners);

            window.pivot = new Vector2(0, 0);
            float GetPositionY(float canvasMin, float canvasMax, float controlMin, float controlMax, float dialogSize) {
                var position = controlMin - dialogSize;
                if (position >= canvasMin)
                    return position;
                position = controlMax;
                if (position + dialogSize <= canvasMax)
                    return position;
                return canvasMin;
            }
            var x = controlCorners[0].x;
            var y = GetPositionY(canvasCorners[0].y, canvasCorners[2].y, controlCorners[0].y, controlCorners[2].y, dialogSize.y);
            window.anchoredPosition = new Vector2(x, y);
            var controlWidth = controlCorners[2].x - controlCorners[0].x;
            window.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, controlWidth);
        }

        protected static void PositionWindowAround(RectTransform window, RectTransform control, float Width) {
            var canvas = control.GetComponentInParent<Canvas>();
            var canvasTransform = canvas.GetComponent<RectTransform>();

            var dialogSize = window.rect.size;

            var controlCorners = new Vector3[4];
            control.GetWorldCorners(controlCorners);
            for (int i = 0; i < 4; i++) {
                controlCorners[i] = canvasTransform.InverseTransformPoint(controlCorners[i]);
            }
            var canvasCorners = new Vector3[4];
            canvasTransform.GetLocalCorners(canvasCorners);

            window.pivot = new Vector2(0, 0);
            float GetPositionX(float canvasMin, float canvasMax, float controlMin, float controlMax, float dialogSize) {
                var position = controlMax - dialogSize;
                if (position >= canvasMin)
                    return position;
                position = controlMin;
                if (position + dialogSize <= canvasMax)
                    return position;
                return canvasMin;
            }
            float GetPositionY(float canvasMin, float canvasMax, float controlMin, float controlMax, float dialogSize) {
                var position = controlMin - dialogSize;
                if (position >= canvasMin)
                    return position;
                position = controlMax;
                if (position + dialogSize <= canvasMax)
                    return position;
                return canvasMin;
            }

            var x = GetPositionX(canvasCorners[0].x, canvasCorners[2].x, controlCorners[0].x, controlCorners[2].x, dialogSize.x);
            var y = GetPositionY(canvasCorners[0].y, canvasCorners[2].y, controlCorners[0].y, controlCorners[2].y, dialogSize.y);
            window.anchoredPosition = new Vector2(x, y);
            window.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Width);
        }

    }


    public abstract class Dialog<T> : Dialog {
        public T? Value;

        [SerializeField]
        private SerializableAction<T> _return = null!;

        public void SetReturn(Action<T> action) {
            _return = action;
        }
        public void Return(T value) {
            _return?.Invoke(value);
        }

        public void Configure(T value, Action<T> returnAction) {
            Value = value;
            SetReturn(returnAction);
        }

    }
    
    public abstract class DialogDinamicReturn<T> : Dialog {
        public T? Value;

        [SerializeField]
        private SerializableAction _return = null!;

        public void SetReturn(Delegate action) {
            _return = new SerializableAction(action);
        }
        public void Return(T value) {
            _return?.Invoke(value);
        }

        public void Configure(T value, Action<T> returnAction) {
            Value = value;
            SetReturn(returnAction);
        }


    }


}
