using UnityEngine;
using UnityEngine.EventSystems;
#nullable enable

namespace FUI.Gears
{
    public class Dimension : UIBehaviour, IFormNotifier
    {
        public enum DimensionsMask
        {
            None = 0,
            Width = 1 << 0,
            Height = 1 << 1,
            Both = Width | Height
        }

        [HideInInspector]
        [SerializeField]
        protected Form? FormToNotify;


        float _width = float.NaN;
        float _height = float.NaN;

        public float Width => _width;
        public float Height => _height;

        public DimensionsMask Mask = DimensionsMask.None;

        protected override void OnRectTransformDimensionsChange()
        {
            RectTransform rectTransform = (RectTransform)transform;
            Rect rect = rectTransform.rect;
            var size = rect.size;

            bool notify = false;
            if (Mask == DimensionsMask.None)
                return;

            if (Mask.HasFlag(DimensionsMask.Width))
            {
                if (!Mathf.Approximately(_width, size.x)) {
                    _width = size.x;
                    notify = true;
                }
            }
            if (Mask.HasFlag(DimensionsMask.Height))
            {
                if (!Mathf.Approximately(_height, size.y)) {
                    _height = size.y;
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
