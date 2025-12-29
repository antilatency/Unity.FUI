using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#nullable enable

namespace FUI.Gears {
    public class Dimension : UIBehaviour, IFormNotifier {
        public enum DimensionsMask {
            None = 0,
            Width = 1 << 0,
            Height = 1 << 1,
            Both = Width | Height
        }

        [HideInInspector]
        [SerializeField]
        protected Form? FormToNotify;


        private float _lastWidth = float.NaN;
        private float _lastHeight = float.NaN;

        public float Width {
            get {
                if (!Mask.HasFlag(DimensionsMask.Width))
                    return float.NaN;

                _lastWidth = ((RectTransform)transform).rect.width;
                return _lastWidth;
            }
        }

        public float Height {
            get {
                if (!Mask.HasFlag(DimensionsMask.Height))
                    return float.NaN;

                _lastHeight = ((RectTransform)transform).rect.height;
                return _lastHeight;
            }
        }

        public DimensionsMask Mask = DimensionsMask.None;

        protected override void OnRectTransformDimensionsChange() {
            RectTransform rectTransform = (RectTransform)transform;
            Rect rect = rectTransform.rect;
            var size = rect.size;

            bool notify = false;
            if (Mask == DimensionsMask.None)
                return;

            if (Mask.HasFlag(DimensionsMask.Width)) {
                if (!Mathf.Approximately(_lastWidth, size.x)) {
                    _lastWidth = size.x;
                    notify = true;
                }
            }
            if (Mask.HasFlag(DimensionsMask.Height)) {
                if (!Mathf.Approximately(_lastHeight, size.y)) {
                    _lastHeight = size.y;
                    notify = true;
                }
            }

            if (notify)
                NotifyForm();
        }



        public void SetFormToNotify(Form? form) {
            FormToNotify = form;
        }

        private void NotifyForm() {
            FormToNotify?.MakeDirty();
        }
    }
}
